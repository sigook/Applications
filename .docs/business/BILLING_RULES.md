# Billing Rules — Invoices (Canada + USA)

How the platform bills Companies for staffing services: line-item generation from approved timesheets, holiday charges, totals, and tax.

**Source of truth:**

| Concern | File |
|---------|------|
| Orchestration, timesheet processing, subcontractor reports | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Invoices/InvoiceService.cs` (abstract base) |
| Canadian invoice creation, holiday pay, HST | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Invoices/CanadaInvoiceService.cs` |
| USA invoice creation, per-location tax | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Invoices/UsaInvoiceService.cs` |
| Country routing | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Invoices/InvoiceServiceFactory.cs` |
| Hours breakdown + amount math | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Shared/TimesheetCalculatorService.cs` |
| Holiday-pay look-back query | `Covenant.Api/Covenant.Infrastructure/Repositories/Accounting/InvoiceRepository.cs` → `GetCompanyRegularCharges` |
| Global rate multipliers | `Covenant.Api/Covenant.Common/Configuration/Rates.cs` |
| Invoice entity | `Covenant.Api/Covenant.Common/Entities/Accounting/Invoice/Invoice.cs` |

**Entry points:** `InvoicesController` (`api/agency/accounting/Invoices`) resolves the service through `InvoiceServiceFactory.Resolve()`, which returns `UsaInvoiceService` when the agency's billing location `IsUSA`, otherwise `CanadaInvoiceService`. `POST .../Invoices/Preview` → `PreviewAsync`; `POST .../Invoices` → `CreateAsync`. Both funnel into `CreateInvoiceInternal` in the country-specific service.

---

## Rate Structure

```
AgencyRate = What the Agency charges the Company (per hour)
WorkerRate = What the Agency pays the Worker (per hour)
Markup     = AgencyRate − WorkerRate (Agency's profit)
```

- `AgencyRate` comes per timesheet (`TimeSheetApprovedBillingModel.AgencyRate`), sourced from the Request's `AgencyRate` (`TimeSheetRepository.cs:189`), not directly from the company's job position rate.
- Multipliers are **global configuration**, not per company: the `Rates` object injected everywhere (`Covenant.Common/Configuration/Rates.cs`). Defaults (`Rates.DefaultRates`): `OverTime = 1.5`, `Holiday = 1.5`, `Vacations = 0.04`, `Hst = 0.13`.
- The invoice snapshots the config rates at creation (`CanadaInvoiceService.CreateInvoiceInternal`): `HolidayRate`, `OverTimeRate`, `VacationsRate`, `HstRate`, `BonusRate`. Of these, **only `OverTimeRate`, `HolidayRate` and `HstRate` participate in the math** — see "Not billed" below.

**No double billing:**

The holidays of an invoice come from the date range of the timesheets being invoiced, so a late-approved timesheet from the holiday's week pulls that holiday back into a later invoice. To keep it from being charged twice, `GetCompanyRegularCharges` excludes any worker that already has an `InvoiceHoliday` row for the same holiday date under that company profile. Scope is **per company**: a worker placed at two companies of the same agency is entitled to holiday pay from each.

> This is the invoice counterpart of `PayStubRepository.GetWorkerRegularWages` → `HolidayWasPaid`, which guards the pay-stub side against the same duplicate (`PayStubPublicHolidays` lookup). Both flows must keep their own guard — see `PAYROLL_RULES.md`.

---

## Canadian Invoice Pipeline

`CanadaInvoiceService.CreateInvoiceInternal` (used by both `PreviewAsync` and `CreateAsync`):

1. Fetch approved timesheets: `ITimesheetRepository.GetTimeSheetForCreatingInvoice`. Fails with "No approved timesheets found for the selected period" unless `model.DirectHiring`.
2. Fetch statutory holidays in the timesheet date range: `InvoiceService.GetHolidaysForPeriod` → `ICatalogRepository.GetHolidaysInWeek` per week.
3. Build timesheet line items: `InvoiceService.ProcessTimesheets<InvoiceTotal>` (skipped for DirectHiring).
4. Build not-worked holiday charges: `GetInvoiceHolidaysAsync` (skipped for DirectHiring).
5. Map manual `AdditionalItems` and `Discounts` from the request model (quantity × unit price each).
6. Compute totals (verified source, `CanadaInvoiceService.CreateInvoiceInternal`):

```csharp
var subtotal = timesheetsSubtotal + additionalItemsSubtotal + holidaysSubtotal - discountsSubtotal;
var hst = subtotal * rates.Hst;
var totalNet = subtotal + hst;
```

7. Get the next sequential number (`IInvoiceRepository.GetNextInvoiceNumber`) and build the `Invoice` entity with `NightShiftRate = 0` and `WeekEnding = max(timesheet date).GetWeekEndingCurrentWeek()` (`null` for DirectHiring).
8. `CreateAsync` persists the invoice, then always calls `CreateSubcontractorReportsAsync` (it no-ops when the company has no subcontractor timesheets pending).

There is no separate "taxable amount" step: additional items, holiday pay and discounts are all inside the single subtotal, and HST applies to that whole block.

### Worked example

```
Timesheet items:      40h × $25.00          = $1,000.00
                       4h × $25.00 × 1.5    = $  150.00   (overtime)
Not-worked holiday:   1.85h × $25.00        = $   46.25   (look-back average, see below)
Additional item:      1 × $500.00           = $  500.00
Discount:             1 × $200.00           = $ (200.00)
                                              ─────────
subtotal                                    = $1,496.25
hst = subtotal × 0.13                       = $  194.51
totalNet                                    = $1,690.76
```

No vacation line, no bonus line, no per-worker tax splits — one subtotal, one HST amount.

> Note: HST is not rounded in code (`CanadaInvoiceService.cs:175` applies no `DefaultMoneyRound()`); the rounded figures above are presentation-only.

---

## Invoice Entity Graph

`Invoice` (`Covenant.Common/Entities/Accounting/Invoice/`) aggregates:

| Child | Built by | Feeds |
|-------|----------|-------|
| `InvoiceTotal` (one per non-zero hour bucket per timesheet) | `ProcessTimesheets<InvoiceTotal>` | `timesheetsSubtotal` |
| `InvoiceHoliday` (one per worker per not-worked holiday) | `GetInvoiceHolidaysAsync` | `holidaysSubtotal` |
| `InvoiceAdditionalItem` | `model.AdditionalItems` (manual) | `additionalItemsSubtotal` |
| `InvoiceDiscount` | `model.Discounts` (manual) | `discountsSubtotal` (subtracted) |
| `InvoiceAdditionalDetail` | `model.ClientSiteAddress` | display only |

Each `InvoiceTotal` also carries a `TimeSheetTotal` navigation (created via `TimesheetCalculatorService.CreateTimeSheetTotalEntity`, cascade-inserted on save). That link is what marks a timesheet as billed — it drives both the holiday no-double-billing guard and re-billability after invoice deletion.

---

## Timesheet Line Items — `InvoiceService.ProcessTimesheets<T>`

Timesheets are grouped by **`Week + WorkerId + RequestId`** and processed in date order with a running `accumulatedRegularHours` per group. Hours come from `TimesheetCalculatorService.CalculateHoursBreakdown`, called via `InvoiceService.CalculateHoursForItem` with `breakIsPaid: false` and `holidayIsPaid: true` (see holiday asymmetry below). Timesheets without both `TimeInApproved` and `TimeOutApproved` produce an empty breakdown (no charge).

One line item (`InvoiceTotal`) is emitted per non-zero bucket per timesheet, each linked to a `TimeSheetTotal` entity:

| Item | Hours source | Amount |
|------|-------------|--------|
| Regular | `RegularHours + OtherRegularHours` | `hours × AgencyRate` |
| Overtime | `OvertimeHours` | `hours × AgencyRate × rates.OverTime` |
| Holiday (worked) | `HolidayHours` | `hours × AgencyRate × rates.Holiday` |
| Missing Hours | `TimeSheet.MissingHours` | `hours × AgencyRate` |
| Missing Overtime | `TimeSheet.MissingHoursOvertime` | `hours × AgencyRate × rates.OverTime` |

Notes:

- `OtherRegularHours` (hours past `timeLimits.MaxHoursWeek` but under the overtime threshold — the two-threshold system) are billed at the plain regular rate, merged into the Regular line.
- Missing hours are billed at the timesheet's `AgencyRate`. The `TimeSheet.MissingRateAgency` column exists but the current invoice pipeline does not read it (the pay-stub side has the analogous `MissingRateWorker` fallback).
- `timesheetsSubtotal = Σ item.TotalGross` across all items.

### Hours breakdown algorithm

`TimesheetCalculatorService.CalculateHoursBreakdown` (shared by invoices, pay stubs and subcontractor reports):

1. `totalHours = TimeOut − TimeIn`, minus `DurationBreak` when `BreakIsPaid` is **true** (the flag name reads inverted relative to its effect — see `TIMESHEET_RULES.md`). Invoices hardcode `breakIsPaid: false` (`InvoiceService.cs:207`), so the break is **never** deducted on the billing side.
2. If `isHoliday && holidayIsPaid`: **all** hours become `HolidayHours`; the day does not feed the overtime accumulator; return.
3. Otherwise add `totalHours` to the running per-group accumulator.
4. `OvertimeHours` = the portion of this day's hours past the `OvertimeStartsAfter` threshold (accumulated basis — `CalculateExcessHours`).
5. `OtherRegularHours` = hours past `timeLimits.MaxHoursWeek` that are not already overtime (the two-threshold system). Billed at the plain regular rate.
6. `RegularHours = totalHours − OvertimeHours − OtherRegularHours`.

`CalculateExcessHours` handles the boundary day: if the accumulator was already past the threshold before this timesheet, the whole day is excess; if the threshold is crossed mid-day, only the hours beyond it are.

### Overtime accumulation scope

Overtime accumulates **per Worker + Week + Request**, not per worker globally. A worker on two requests in the same week is evaluated against `OvertimeStartsAfter` independently per request: 40h on Request A + 40h on Request B produces **no** overtime. Same rule on pay stubs (see `PAYROLL_RULES.md`) and subcontractor reports (where the accumulator resets per request inside the worker-week group).

---

## Holidays — Two Separate Flows

### a) Worked holiday (worker clocked in on the holiday)

`TimesheetCalculatorService.CalculateHoursBreakdown`: when `isHoliday && holidayIsPaid`, **all** of the day's hours go to `HolidayHours`, nothing feeds the overtime accumulator. `ProcessTimesheets` bills them at `AgencyRate × rates.Holiday`.

**Asymmetry with pay stubs:** invoices hardcode `holidayIsPaid: true` (`InvoiceService.CalculateHoursForItem`), so worked-holiday hours are *always* billed at the holiday multiplier regardless of the company's `HolidayIsPaid` setting. Pay stubs honor the timesheet's `ts.HolidayIsPaid` flag (`PayStubService`), so a worker may be paid those hours as regular while the company is still billed at the holiday rate.

### b) Public Holiday Pay (statutory holiday NOT worked)

Charged even though nobody clocked in — the ESA labour benefit passed through to the company. `CanadaInvoiceService.GetInvoiceHolidaysAsync`:

- Only when the company profile has paid holidays enabled (`PaidHolidays` flag, read from the first timesheet) and there are holidays in the invoice period.
- Per holiday: look-back window = the four work weeks before the holiday's week (`holiday.GetEnd()` / `.GetStart()`); qualifying days from `GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay()` (ESA "last and first scheduled day" test).
- `InvoiceRepository.GetCompanyRegularCharges(Guid companyProfileId, DateTime holiday, DateTime start, DateTime end, IEnumerable<DateTime> qualifyingDays)` (`InvoiceRepository.cs:276`) returns one `CompanyRegularChargesByWorker` per qualifying worker; the `holiday` argument drives the `alreadyBilled` `InvoiceHoliday` exclusion:
  - **Qualifying worker** = has timesheets on the qualifying days with `TimeInApproved != null && TimeOutApproved != null` and `TimeSheetTotal == null` (not yet billed) (`InvoiceRepository.cs:282-288`). The explicit **no-double-billing guard** is the `alreadyBilled` subquery, which excludes workers that already have an `InvoiceHoliday` row for the same holiday (`InvoiceRepository.cs:278-280`, applied at `:295`); `TimeSheetTotal == null` additionally keeps already-billed timesheets out of qualification.
  - **Charge base** = `Regular + OtherRegular` amounts from *previously invoiced* `InvoiceTotals` in the look-back window. Overtime, worked-holiday and missing charges are excluded — a **narrower base than the pay-stub side** (`TimesheetCalculatorService.CalculateHolidayPayBase` uses full worker gross plus 4% vacations).
- Amount math (`CompanyRegularChargesByWorker`, `CovenantConstants.PublicHolidays`):

```
TotalHours  = Regular / AgencyRate            (capped at 176 = 44 × 4)
HoursToPay  = TotalHours / 20
AmountToPay = HoursToPay × AgencyRate         (≈ Regular charges / 20)
```

- Results become `InvoiceHoliday` entities (`Invoice.AddHolidays`), summed into `holidaysSubtotal`; HST applies. Preview shows them as `"Charge for Holiday {date}"` lines.
- Only Canadian invoices carry this charge — `UsaInvoiceService` never builds `InvoiceHoliday` items.

**Example:** worker's look-back Regular charges = $925.00 at AgencyRate $25.00 → `TotalHours = 37` (under the 176 cap) → `HoursToPay = 1.85` → `AmountToPay = $46.25` billed for the holiday.

Both flows mirror the dual-guard design on the payroll side — see `PAYROLL_RULES.md`. Summary of the invoice/pay-stub differences for holidays:

| Aspect | Invoice (company charge) | Pay stub (worker pay) |
|--------|--------------------------|----------------------|
| Worked holiday honors `HolidayIsPaid`? | No — hardcoded `holidayIsPaid: true` | Yes — uses `ts.HolidayIsPaid` |
| Not-worked base | `Regular + OtherRegular` **company charges** (AgencyRate) / 20, capped 176h | Full worker gross incl. OT/holiday/missing + 4% vacations, / 20 |
| Rate preserved | AgencyRate (markup kept) | WorkerRate |
| Double-billing guard | `alreadyBilled` `InvoiceHoliday` exclusion (plus `TimeSheetTotal == null` on qualifying timesheets) | Dual guard on payroll side (see `PAYROLL_RULES.md`) |

---

## Not Billed on Invoices

| Concept | Reality in code |
|---------|-----------------|
| Vacation pay (4%) | **Never enters invoice totals.** `Invoice.VacationsRate` is stored as a snapshot but no invoice math reads it. Vacation 4% is a pay-stub concept (`rates.Vacations`, `PayStubService`). |
| Bonus | `Invoice.BonusRate` stored only; no bonus line or multiplier in any total. |
| Night shift | Deprecated platform-wide. `CanadaInvoiceService` sets `NightShiftRate = 0` and `TimesheetCalculatorService.CalculateHoursBreakdown` explicitly does not support it. Do not add night-shift logic. |
| Per-province HST | HST is the **single global config rate** `rates.Hst` (default 0.13) applied to every Canadian invoice, regardless of province. There is no provincial tax table in the Canadian path. Per-location tax exists only in the USA path. |

---

## Direct Hiring (Canada)

`model.DirectHiring = true` builds an invoice with **manual items only**:

- No timesheet requirement (the empty-timesheets guard is bypassed), `ProcessTimesheets` and `GetInvoiceHolidaysAsync` are skipped.
- Subtotal = additional items − discounts; HST applies as usual.
- `WeekEnding = null`.

---

## Subcontractor Reports

Created by `InvoiceService.CreateSubcontractorReportsAsync`, invoked from `CreateAsync` right after saving the invoice in both country services (`CanadaInvoiceService` and `UsaInvoiceService.cs:86-87`). Pulls subcontractor timesheets (`GetTimeSheetForCreatingReportsSubcontractor`) and builds one `ReportSubcontractor` per **Worker + Week**:

- Hours use the same `CalculateHoursForItem` breakdown, with the overtime accumulator reset **per request** inside the worker-week group.
- Amounts use **WorkerRate** (what the subcontractor is owed), with the same global multipliers: regular, overtime (`rates.OverTime`), worked holiday (`rates.Holiday`). Missing hours and night shift are hardcoded `0` in the wage detail.
- Totals: `Gross = regular + overtime + holiday`; `PublicHolidayPay = 0` (subcontractors get no statutory holiday pay); `Earnings = TotalNet = Gross`, minus nothing except explicit `DeductionsOthers` rows appended per timesheet.

---

## USA Invoices

`UsaInvoiceService` shares `ProcessTimesheets` (emitting `InvoiceUSAItem`) but taxes differently:

- **Per-location tax, accumulated per timesheet** inside `ProcessTimesheets`: `tax += timesheetGross * timesheet.Tax`, where `timesheet.Tax` is the rate configured for the request's job location (`LocationTaxes` table, managed from the Location "Configure tax" UI — entered as a percentage, stored as a rate). An invoice spanning locations taxes each timesheet at its own rate.
- **Direct hiring:** no timesheets, so the caller sends `TaxPercentage`; tax = `subTotal × TaxPercentage / 100`.
- The computed tax is stored in `InvoiceUSA.Tax` and surfaced through the shared summary's `Hst` field.
- No public-holiday-pay items. The USA `CreateAsync` path does call `CreateSubcontractorReportsAsync` (`UsaInvoiceService.cs:86-87`), just like Canada.

---

## Invoice Number

- Canada: `AI-{InvoiceNumber:D4}-{yy}` (`Invoice.PrefixInvoiceNumber = "AI"`), formatted in `InvoiceRepository` list/summary queries via the helper `Invoice.BuildInvoiceNumber` (`Invoice.cs:61`); `InvoiceNumber` is sequential via `IInvoiceRepository.GetNextInvoiceNumber()`.
- USA: prefix `US` (`InvoiceUSA.PrefixInvoiceNumber`); the number is built and persisted at creation (`UsaInvoiceService.cs:145`) using `GetNextInvoiceUSANumber()` (`UsaInvoiceService.cs:126`), not `GetNextInvoiceNumber()`.

---

## Invoice Deletion

`InvoiceService.DeleteInvoice` → country-specific `DeleteInvoiceData`. The Canadian implementation deletes the invoice **and its subcontractor reports** (`DeleteInvoiceAndReportsSubcontractor`) and optionally the pay stubs listed in the request; blob PDFs are removed and a Teams warning is posted to the Accounting channel.

Deleting an invoice clears the `TimeSheetTotal` link, which re-qualifies its timesheets for billing — including the not-worked holiday guard above.

---

## PDF & Email

- PDF: rendered from `/Views/Billing/Invoice/Invoice.cshtml` via `IRazorViewToStringRenderer` + `IPdfGeneratorService`, cached in the invoices blob container (`InvoiceService.GetInvoicePdf` / `UploadInvoicePdf`). Regenerate (delete blob) after template fixes.
- Email: `InvoiceService.SendInvoiceEmail` attaches the PDF plus caller-supplied files and sends through `IEmailService` using the body template `/Views/Billing/Invoice/InvoiceEmail.cshtml`.

---

## Gotchas for Implementers

- **Preview and Create share `CreateInvoiceInternal`** — any billing change automatically affects both; only persistence, subcontractor reports and numbering differ.
- **Multipliers are global, not per company.** Changing `rates.OverTime` / `rates.Holiday` / `rates.Hst` changes every future invoice; there is no per-company override in the pipeline.
- **Snapshot fields lie.** `Invoice.VacationsRate`, `BonusRate`, `NightShiftRate` exist on the entity and are populated, but the totals never use them. Do not "fix" totals to include them.
- **`TimeSheetTotal == null` means "not yet billed".** Creating an invoice attaches `TimeSheetTotal` rows; deleting it detaches them. Both the timesheet fetch and the holiday-pay qualification depend on this marker.
- **Holiday hours bypass overtime.** A worked holiday's hours never enter the overtime accumulator, so they can't also generate overtime for the week.
- **`GetInvoiceHolidaysAsync` reads `PaidHolidays` from the first timesheet** of the batch — the flag is a company-profile setting projected onto every timesheet row.
- **USA vs Canada is decided by the agency's billing location** (`InvoiceServiceFactory`), not by the company being billed.

---

## Related Documents

- `PAYROLL_RULES.md` — worker-side flow and CPP/EI/tax deductions (pay-stub only, never on invoices); shares the hours breakdown, the per-request overtime scope, and the two-flow holiday design (with a *worker-wage* holiday base instead of company charges).
- `TIMESHEET_RULES.md` — how approved hours, breaks, missing hours and the two-threshold system are captured.
