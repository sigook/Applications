# Billing Rules — Invoices (Canada + USA)

How the platform bills Companies for staffing services: line-item generation from approved timesheets, holiday charges, totals, and tax. One invoice entity and one service serve every agency; the few Canada/USA differences are isolated in a country strategy.

**Source of truth:**

| Concern | File |
|---------|------|
| Orchestration, timesheet processing, totals, numbering, deletion, PDF, subcontractor reports | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Invoices/InvoiceService.cs` |
| Country strategy contract | `Covenant.Api/Covenant.Core.BL/Interfaces/Accounting/Invoices/IInvoiceCountryStrategy.cs` |
| Canadian rules (HST, unworked statutory holidays, `AI` prefix) | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Invoices/CanadaInvoiceStrategy.cs` |
| USA rules (per-location tax, `US` prefix) | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Invoices/UsaInvoiceStrategy.cs` |
| Country routing | `Covenant.Api/Covenant.Core.BL/Services/Accounting/Invoices/InvoiceCountryStrategyResolver.cs` |
| Hours breakdown + amount math | `Covenant.Api/Covenant.Core.BL/Services/Accounting/TimesheetCalculatorService.cs` |
| Holiday-pay look-back query, invoice numbering query | `Covenant.Api/Covenant.Infrastructure/Repositories/Accounting/InvoiceRepository.cs` (`GetCompanyRegularCharges`, `GetNextInvoiceNumber`; raw SQL in `Covenant.Infrastructure/SqlQueries.resx`) |
| Global rate multipliers | `Covenant.Api/Covenant.Common/Configuration/Rates.cs` |
| Invoice entity | `Covenant.Api/Covenant.Common/Entities/Accounting/Invoice/Invoice.cs` |

**Entry points:** `InvoicesController` (`api/agency/accounting/invoices`) injects `IInvoiceService` directly. `POST .../Invoices/Preview` → `PreviewInvoice`; `POST .../Invoices` → `CreateInvoice`. Both funnel into `BuildInvoice`, which resolves the country strategy per invoice from the billed company's agency.

---

## Country Strategy

`IInvoiceCountryStrategy` is the only place where Canada and the USA differ. `IInvoiceCountryStrategyResolver.ResolveForAgency(agencyId)` returns `UsaInvoiceStrategy` when `AgencyRepository.GetBillingLocation(agencyId)?.IsUSA == true`, otherwise `CanadaInvoiceStrategy`. The agency is always `Invoice.CompanyProfile.AgencyId` — the invoice row carries neither `Country` nor `AgencyId`.

| Member | Canada | USA |
|--------|--------|-----|
| `IsUSA` | `false` | `true` |
| `NumberPrefix` | `AI` | `US` |
| `TaxName` | `HST` | `Tax` |
| `BillsUnworkedHolidays` | `true` | `false` |
| `CalculateTax(subTotal, timesheetsTax, model)` | `subTotal × rates.Hst` (single global config rate, not per province) | Σ per-timesheet `gross × LocationTax.Tax1` accumulated during `ProcessTimesheets`; `subTotal × TaxPercentage / 100` when `DirectHiring` |
| `BuildInvoiceNumber(number, date)` | `AI-{number:0000}-{yy}` | `US-{number:0000}-{yy}` |
| `Decorate(summary)` | `TaxName = HST`, Covenant color/payroll block | `TaxName = Tax`, Sigook color/payroll block, clears `HstNumber` |

> `Location.IsUSA` is a null-conditional chain over `City.Province.Country`; the resolver relies on `GetBillingLocation`, which includes that chain. Loading the location any other way silently yields `false` → Canadian rules.

---

## Rate Structure

```
AgencyRate = What the Agency charges the Company (per hour)
WorkerRate = What the Agency pays the Worker (per hour)
Markup     = AgencyRate − WorkerRate (Agency's profit)
```

- `AgencyRate` comes per timesheet (`TimeSheetApprovedBillingModel.AgencyRate`), sourced from the Request's `AgencyRate` (`TimeSheetRepository`), not directly from the company's job position rate.
- Multipliers are **global configuration**, not per company: the `Rates` object injected everywhere (`Covenant.Common/Configuration/Rates.cs`). Defaults (`Rates.DefaultRates`): `OverTime = 1.5`, `Holiday = 1.5`, `Vacations = 0.04`, `Hst = 0.13`.
- The invoice does **not** snapshot rates. Only `rates.OverTime`, `rates.Holiday` and (Canada) `rates.Hst` participate in the math, read from configuration at creation time — see "Not billed" below.

**No double billing:**

The holidays of an invoice come from the date range of the timesheets being invoiced, so a late-approved timesheet from the holiday's week pulls that holiday back into a later invoice. To keep it from being charged twice, `GetCompanyRegularCharges` excludes any worker that already has an `InvoiceHoliday` row for the same holiday date under that company profile. Scope is **per company**: a worker placed at two companies of the same agency is entitled to holiday pay from each.

> This is the invoice counterpart of `PayStubRepository.GetWorkerRegularWages` → `HolidayWasPaid`, which guards the pay-stub side against the same duplicate (`PayStubPublicHolidays` lookup). Both flows must keep their own guard — see `PAYROLL_RULES.md`.

---

## Invoice Pipeline

`InvoiceService.BuildInvoice` (used by both `PreviewInvoice` and `CreateInvoice`):

1. Load the `CompanyProfile` by `model.CompanyProfileId`; `agencyId = companyProfile.AgencyId`; resolve the country strategy for that agency.
2. Fetch approved timesheets: `ITimesheetRepository.GetTimeSheetForCreatingInvoice`. Fails with "No approved timesheets found for the selected period" unless `model.DirectHiring`.
3. Fetch statutory holidays in the timesheet date range: `GetHolidaysForPeriod` → `ICatalogRepository.GetHolidaysInWeek` per week.
4. Build timesheet line items: `ProcessTimesheets` (one `InvoiceTotal` per non-zero bucket per timesheet; also accumulates `timesheetsTax` for the USA strategy). Skipped for DirectHiring.
5. Build not-worked holiday charges: `GetInvoiceHolidaysAsync`, only when `strategy.BillsUnworkedHolidays && !model.DirectHiring`.
6. Map manual `AdditionalItems` and `Discounts` from the request model (quantity × unit price each).
7. Compute totals:

```csharp
var subtotal = timesheetsSubtotal + additionalItemsSubtotal + holidaysSubtotal - discountsSubtotal;
var tax = strategy.CalculateTax(subtotal, timesheetsTax, model);
var totalNet = subtotal + tax;
```

8. Get the next number for the agency (`IInvoiceRepository.GetNextInvoiceNumber(agencyId)`), format it with `strategy.BuildInvoiceNumber`, and build the `Invoice` entity with `WeekEnding = max(timesheet date).GetWeekEndingCurrentWeek()` (`null` for DirectHiring), then `AddInvoiceTotals` / `AddHolidays` / `AddAdditionalItems` / `AddDiscounts` / `SetAdditionalDetail`.
9. `CreateInvoice` persists the invoice, then always calls `CreateSubcontractorReportsAsync` (it no-ops when the company has no subcontractor timesheets pending).

There is no separate "taxable amount" step: additional items, holiday pay and discounts are all inside the single subtotal, and the tax applies to that whole block.

`PreviewInvoice` returns `InvoicePreviewModel { SubTotal, Tax, TaxName, Total, Items, Discounts }`; the web shows `taxName` as the tax label.

### Worked example (Canada)

```
Timesheet items:      40h × $25.00          = $1,000.00
                       4h × $25.00 × 1.5    = $  150.00   (overtime)
Not-worked holiday:   1.85h × $25.00        = $   46.25   (look-back average, see below)
Additional item:      1 × $500.00           = $  500.00
Discount:             1 × $200.00           = $ (200.00)
                                              ─────────
subtotal                                    = $1,496.25
tax = subtotal × 0.13                       = $  194.51
totalNet                                    = $1,690.76
```

No vacation line, no bonus line, no per-worker tax splits — one subtotal, one tax amount.

> Note: tax is not rounded in code (no `DefaultMoneyRound()` in `CalculateTax`); the rounded figures above are presentation-only.

---

## Invoice Entity Graph

`Invoice` (`Covenant.Common/Entities/Accounting/Invoice/`, table `Invoices`) is the single invoice entity for every agency. Columns: `Id`, `NumberId` (identity, sort key only), `InvoiceNumberId` (numeric part of the series), `InvoiceNumber` (stored formatted text, e.g. `AI-0333-19` or `US-1001-23`), `CompanyProfileId`, `Email` (optional recipient override), `SubTotal`, `Tax`, `TotalNet`, `CreatedAt`, `WeekEnding` (null for direct hiring), `Status`, `UpdatedAt`, `UpdatedBy`. Bill-to / bill-from data (company address, phone, fax; agency address, phone, HST number; invoice notes) is **not** stored on the invoice — it is read live through `Invoice.CompanyProfile` and `CompanyProfile.Agency` when the summary/PDF is built.

| Child | Built by | Feeds |
|-------|----------|-------|
| `InvoiceTotal` (one per non-zero hour bucket per timesheet, `TimeSheetTotalId` required) | `ProcessTimesheets` | `timesheetsSubtotal` |
| `InvoiceHoliday` (one per worker per not-worked holiday; Canada only) | `GetInvoiceHolidaysAsync` | `holidaysSubtotal` |
| `InvoiceAdditionalItem` (manual line, no timesheet) | `model.AdditionalItems` | `additionalItemsSubtotal` |
| `InvoiceDiscount` | `model.Discounts` (manual) | `discountsSubtotal` (subtracted) |
| `InvoiceAdditionalDetail` (1:1, `InvoiceId`) | `model.ClientSiteAddress` | display only |

Each `InvoiceTotal` carries `Quantity`, `UnitPrice`, `Description`, `Total` plus the amount buckets (`Regular`, `Overtime`, `Holiday`, `Missing`, `MissingOvertime`, `TotalGross`, `TotalNet`) and a required `TimeSheetTotal` navigation (created via `TimesheetCalculatorService.CreateTimeSheetTotalEntity`, cascade-inserted on save). That link is what marks a timesheet as billed — it drives both the holiday no-double-billing guard and re-billability after invoice deletion.

Some older invoices have `InvoiceTotals` rows with zero quantity/amounts and a null `Description`: they exist only to keep the timesheet link. The summary groups null-description rows together, so the PDF renders them as before.

---

## Timesheet Line Items — `InvoiceService.ProcessTimesheets`

Timesheets are grouped by **`Week + WorkerId + RequestId`** and processed in date order with a running `accumulatedRegularHours` per group. Hours come from `TimesheetCalculatorService.CalculateHoursBreakdown`, called via `InvoiceService.CalculateHoursForItem` with the request's `BreakIsPaid` and `holidayIsPaid: true` (see holiday asymmetry below). Timesheets without both `TimeInApproved` and `TimeOutApproved` produce an empty breakdown (no charge).

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
- For every timesheet, `timesheetsTax += timesheetGross × timesheet.Tax` (the request's job-location `LocationTax.Tax1`). Only the USA strategy reads this accumulator; Canada ignores it.

### Hours breakdown algorithm

`TimesheetCalculatorService.CalculateHoursBreakdown` (shared by invoices, pay stubs and subcontractor reports):

1. `totalHours = TimeOut − TimeIn`, minus `DurationBreak` when the break is **not** paid (`BreakIsPaid = false`), clamped at zero (see `TIMESHEET_RULES.md`). Invoices read the request's `BreakIsPaid` (`InvoiceService.CalculateHoursForItem`), so an unpaid break is deducted from both the invoice and the pay stub, and a paid break is billed and paid in full.
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

`TimesheetCalculatorService.CalculateHoursBreakdown`: when `isHoliday && holidayIsPaid`, **all** of the day's hours go to `HolidayHours`, nothing feeds the overtime accumulator. `ProcessTimesheets` bills them at `AgencyRate × rates.Holiday`. Applies to every agency, Canada and USA alike.

**Asymmetry with pay stubs:** invoices hardcode `holidayIsPaid: true` (`InvoiceService.CalculateHoursForItem`), so worked-holiday hours are *always* billed at the holiday multiplier regardless of the company's `HolidayIsPaid` setting. Pay stubs honor the timesheet's `ts.HolidayIsPaid` flag (`PayStubService`), so a worker may be paid those hours as regular while the company is still billed at the holiday rate.

### b) Public Holiday Pay (statutory holiday NOT worked)

Charged even though nobody clocked in — the ESA labour benefit passed through to the company. `InvoiceService.GetInvoiceHolidaysAsync`, gated by `strategy.BillsUnworkedHolidays` (Canada only — USA invoices never get `InvoiceHoliday` rows):

- Only when the company profile has paid holidays enabled (`PaidHolidays` flag, read from the first timesheet) and there are holidays in the invoice period.
- Per holiday: look-back window = the four work weeks before the holiday's week (`holiday.GetEnd()` / `.GetStart()`); qualifying days from `GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay()` (ESA "last and first scheduled day" test).
- `InvoiceRepository.GetCompanyRegularCharges(Guid companyProfileId, DateTime holiday, DateTime start, DateTime end, IEnumerable<DateTime> qualifyingDays)` returns one `CompanyRegularChargesByWorker` per qualifying worker; the `holiday` argument drives the `alreadyBilled` `InvoiceHoliday` exclusion:
  - **Qualifying worker** = has timesheets on the qualifying days with `TimeInApproved != null && TimeOutApproved != null` and `TimeSheetTotal == null` (not yet billed). The explicit **no-double-billing guard** is the `alreadyBilled` subquery, which excludes workers that already have an `InvoiceHoliday` row for the same holiday; `TimeSheetTotal == null` additionally keeps already-billed timesheets out of qualification.
  - **Charge base** = `Regular + OtherRegular` amounts from *previously invoiced* `InvoiceTotals` in the look-back window. Overtime, worked-holiday and missing charges are excluded — a **narrower base than the pay-stub side** (`TimesheetCalculatorService.CalculateHolidayPayBase` uses full worker gross plus 4% vacations).
- Amount math (`CompanyRegularChargesByWorker`, `CovenantConstants.PublicHolidays`):

```
TotalHours  = Regular / AgencyRate            (capped at 176 = 44 × 4)
HoursToPay  = TotalHours / 20
AmountToPay = HoursToPay × AgencyRate         (≈ Regular charges / 20)
```

- Results become `InvoiceHoliday` entities (`Invoice.AddHolidays`), summed into `holidaysSubtotal`; HST applies. Preview shows them as `"Charge for Holiday {date}"` lines.

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
| Vacation pay (4%) | **Never enters invoice totals.** The invoice has no vacation column; vacation 4% is a pay-stub concept (`rates.Vacations`, `PayStubService`). |
| Bonus | No bonus column, line or multiplier anywhere in the invoice. |
| Night shift | Not billable anywhere. The invoice has no night-shift rate column, `ProcessTimesheets` emits no night-shift bucket and `TimesheetCalculatorService.CalculateHoursBreakdown` explicitly does not support it. Do not add night-shift logic. |
| Per-province HST | HST is the **single global config rate** `rates.Hst` (default 0.13) applied by `CanadaInvoiceStrategy` to every Canadian invoice, regardless of province. There is no provincial tax table. Per-location tax exists only in `UsaInvoiceStrategy`. |

---

## Direct Hiring

`model.DirectHiring = true` builds an invoice with **manual items only**:

- No timesheet requirement (the empty-timesheets guard is bypassed), `ProcessTimesheets` and `GetInvoiceHolidaysAsync` are skipped.
- Subtotal = additional items − discounts. Canada taxes it with `rates.Hst`; the USA has no timesheet taxes to sum, so the caller sends `TaxPercentage` and `tax = subTotal × TaxPercentage / 100`.
- `WeekEnding = null`.

---

## Subcontractor Reports

Created by `InvoiceService.CreateSubcontractorReportsAsync`, invoked from `CreateInvoice` right after saving the invoice, for every agency. Pulls subcontractor timesheets (`GetTimeSheetForCreatingReportsSubcontractor`) and builds one `ReportSubcontractor` per **Worker + Week**:

- Hours use the same `CalculateHoursForItem` breakdown, with the overtime accumulator reset **per request** inside the worker-week group.
- Amounts use **WorkerRate** (what the subcontractor is owed), with the same global multipliers: regular, overtime (`rates.OverTime`), worked holiday (`rates.Holiday`). Missing hours and night shift are hardcoded `0` in the wage detail.
- Totals: `Gross = regular + overtime + holiday`; `PublicHolidayPay = 0` (subcontractors get no statutory holiday pay); `Earnings = TotalNet = Gross`, minus nothing except explicit `DeductionsOthers` rows appended per timesheet.

### Deletion

Two paths remove subcontractor reports:

- **With the invoice** — `DeleteInvoiceAndReportsSubcontractor`, see *Invoice Deletion* below.
- **Standalone, by week** — `DELETE api/agency/accounting/reports/subcontractors?weekEnding=` → `AccountingService.DeleteSubcontractorReport` → `ISubcontractorRepository.DeleteReportsByWeekEnding`. Deletes every `ReportSubcontractor` of that week belonging to the caller's agency (children cascade) **plus** their `TimeSheetTotalPayroll` rows. Removing those rows is what re-qualifies the timesheets: `GetTimeSheetForCreatingReportsSubcontractor` only picks timesheets with `TimeSheetTotalPayroll == null`, so the week is rebuilt on the next invoice creation for that company. The invoice itself is untouched. A week with no reports returns `400`.

---

## Invoice Number

- Format: `{Prefix}-{InvoiceNumberId:0000}-{yy}` via `strategy.BuildInvoiceNumber` — `AI` for Canadian agencies, `US` for USA agencies. The formatted string is stored in `Invoice.InvoiceNumber` at creation; lists, timesheet usages (`TimeSheetUsagesModel.InvoiceNumber`) and the PDF read the stored text, nothing reformats it later.
- Series: `IInvoiceRepository.GetNextInvoiceNumber(agencyId)` is a raw SQL query (`SqlQueries.resx`) that returns the **lowest unused** `InvoiceNumberId` over `Invoices` joined to `CompanyProfiles` filtered by `AgencyId`. Each agency has its own series, and gaps left by deleted invoices are filled first.
- `IX_Invoices_InvoiceNumber` is a non-unique index: two agencies may legitimately hold the same formatted number.

---

## Invoice Status & Payment Terms

`Invoice` carries a payment status, `InvoiceStatus` (`Covenant.Common/Enums/InvoiceStatus.cs`): `Pending = 1` (default, column default `'Pending'`), `Paid = 2` and `Due = 3`. Stored as a string (`EnumToStringConverter`, max 20).

### Payment terms (per company)

- `CompanyProfile.PaymentTermDays` (int, default 30, column default `30`) is the number of days the company has to pay an invoice — the "Net" of accounting's spreadsheet. Range 1–365, validated by `CompanyProfilePaymentTermsUpdateModelValidator`.
- Edited from the client's Settings card: `PATCH api/agency/recruiting/clients/{id}/PaymentTerms` with `CompanyProfilePaymentTermsUpdateModel { PaymentTermDays }`.
- **Due date** = `Invoice.CreatedAt.Date + CompanyProfile.PaymentTermDays`. It is never stored: `InvoiceListModel.DueDate` is computed from the live company setting, so changing the terms changes the due date of every invoice of that company, past ones included.
- **Due days** = `today − DueDate` when positive, `0` otherwise and always `0` for `Paid` invoices (`InvoiceListModel.SetDueDays`, filled by `InvoiceService` with `TimeProvider`).

### Transitions

| From | To | Who | Effect |
|------|----|-----|--------|
| `Pending` / `Due` | `Paid` | Agency admin (`PUT …/status`) | `PaidAt` = the instant sent in `ChangeInvoiceStatusModel.PaidAt` (the web sends the picked day's local midnight as ISO, same convention as `CreatedAt`), or now when omitted |
| `Paid` | `Pending` | Agency admin | `PaidAt` cleared. The nightly job may move it to `Due` again if the term already expired |
| `Pending` | `Due` | `MarkOverdueInvoices` job only | Bulk `ExecuteUpdate` (`IInvoiceRepository.MarkPendingInvoicesAsDue`): `Status = 'Pending'` and `DueDate < today`. `UpdatedAt` = run time, `UpdatedBy` = null (no user). Returns the invoice numbers it marked, ordered by `NumberId` |

- `Due` cannot be set by hand: `ChangeInvoiceStatusModelValidator` rejects it, and `PaidAt` is only accepted together with `Paid`.
- `Invoice.ChangeStatus` rejects a change to the status the invoice already has (`"Invoice is already {status}"`).
- The job never moves `Due` back to `Pending` (e.g. after the company's terms are extended); fix those by hand.
- **Who/when:** every manual change writes `UpdatedAt` and `UpdatedBy` (FK to `Users`, `UpdatedByUser` navigation) on the invoice row. There is no status history table.
- **Endpoint:** `PUT api/agency/accounting/invoices/{invoiceId}/status` → `InvoiceService.ChangeInvoiceStatus`, which loads the invoice through `GetInvoiceForAgency`, scoped to the caller's agencies, so an invoice of another agency answers "Invoice not found".
- **Job:** `POST api/jobs/scheduletasks/MarkOverdueInvoices` (`ScheduleTasksController`, returns the number of invoices updated) called by the Azure Function `MarkOverdueInvoices` (`Sigook.Functions/Functions/ScheduleTasks.cs`, timer `0 0 1 * * *`, daily at 01:00) through the usual client-credentials flow; the Function posts the HTTP result to Teams like the other scheduled tasks. When at least one invoice was marked, `InvoiceService.MarkOverdueInvoices` also posts a warning to the Accounting Teams channel (the same one that receives invoice deletions): `"Invoices marked as Due"` / `"Invoices {numbers} are Due"` (`"Invoice {number} is Due"` for a single one), so accounting sees exactly which invoices changed. The text lists at most 20 invoice numbers; beyond that it becomes `"{count} invoices are Due: {first 20} and {remaining} more"`, because Teams rejects cards above ~28 KB. A failed webhook call is logged as a warning (`Teams notification for {Count} invoices marked as Due failed`) and never fails the job. Nothing is posted when no invoice was marked.
- **Status does not affect billing:** it never touches totals, timesheet links (`TimeSheetTotal`), pay stubs or the PDF. Deleting an invoice ignores the status.

### List & exports

- `InvoiceListModel` exposes `Status`, `PaymentTermDays`, `DueDate`, `DueDays`, `PaidAt`, `SubTotal`, `Tax`, `UpdatedAt` and `UpdatedByName` (the `AgencyPersonnel.Name` of the user in the invoice's agency, falling back to the user email). `GetInvoicesFilter.Status` filters (`Due` included), `GetInvoicesFilterSortBy.Status` sorts.
- Agency grid (`Sigook.Web` → Invoices): the status tag tooltip shows `Due Date (Net N) · Due Days · Updated by/at`; a `Payment Date` column shows `PaidAt`. "Mark as Paid" opens `MarkInvoicePaidModal` (payment date, defaults to today, max today); "Mark as Pending" is a confirm.
- Two Excel exports, both over the grid's current filter:
  - `GET …/invoices/file` (`GenerateInvoicesReport`): N° Invoice, Created At, Company, Total, Status.
  - `GET …/invoices/payments-report` (`GenerateInvoicesPaymentsReport`, grid action "Payments report"): Invoice Date, Invoice, Company, B4 Tax (`SubTotal`), Tax, Amount (`TotalNet`), Status, Net (`PaymentTermDays`), Due Date, Due Days, Payment Date — mirrors accounting's payments follow-up spreadsheet.
- The company portal list does not show the status.

---

## Invoice Deletion

`InvoiceService.DeleteInvoice` is the same for every agency: `DeleteInvoiceAndReportsSubcontractor` removes the invoice, its `TimeSheetTotal` rows (so the timesheets become billable again), the related `ReportSubcontractor` rows and their `TimeSheetTotalPayroll` rows; then the pay stubs listed in the request are deleted, the cached PDF blob is removed and a Teams warning is posted to the Accounting channel.

Clearing the `TimeSheetTotal` link re-qualifies the timesheets for billing — including the not-worked holiday guard above.

---

## PDF & Email

- Summary: `IInvoiceRepository.GetInvoiceSummaryById` projects the invoice plus live company/agency data into `InvoiceSummaryModel` (`Tax`, `AgencyId` — the latter only to resolve the strategy); `InvoiceService.GetInvoicePdf` then applies `strategy.Decorate` (tax label, Covenant/Sigook color and payroll block, HST number hidden for the USA) before rendering.
- PDF: rendered in-process with QuestPDF by the MediatR handler `GenerateInvoicePdf` (`Covenant.Documents/Services/`), cached in the invoices blob container keyed by invoice `Id` (`InvoiceService.GetInvoicePdf` / `UploadInvoicePdf`). The company's invoice notes (Quill HTML) go through `Covenant.Documents/Pdf/HtmlNotes.cs`, which only understands `p`, `div`, `li`, `br`, `strong`/`b`, `em`/`i`, `u`, `a` and inline `color` styles; any other tag is dropped and its text kept. Regenerate (delete blob) after layout fixes.
- Email: `InvoiceService.SendInvoiceEmail` attaches the PDF plus caller-supplied files and sends through `IEmailService` using the body template `/Views/Billing/Invoice/InvoiceEmail.cshtml`. The summary's email is `Invoice.Email` when set, otherwise `CompanyProfile.Company.Email`.
- Company portal: `GET api/company/invoices` → `CompanyService.GetCompanyInvoices` (no country branch); `GET api/company/invoices/{id}` → `CompanyService.GetCompanyInvoice` → `IInvoiceRepository.GetInvoiceSummaryForCompany(companyId, id)`, which enforces that the invoice belongs to the calling company.

---

## Gotchas for Implementers

- **Preview and Create share `BuildInvoice`** — any billing change automatically affects both; only persistence and subcontractor reports differ.
- **Country differences live only in the strategy.** Adding a Canada/USA branch anywhere else (service, repository, controller) is wrong; extend `IInvoiceCountryStrategy` instead.
- **Multipliers are global, not per company.** Changing `rates.OverTime` / `rates.Holiday` / `rates.Hst` changes every future invoice; there is no per-company override in the pipeline.
- **No rate snapshots on the invoice.** There are no vacation, bonus, night-shift or HST rate columns. Do not "fix" totals to include vacations or bonus.
- **`TimeSheetTotal == null` means "not yet billed".** Creating an invoice attaches `TimeSheetTotal` rows; deleting it detaches them. Both the timesheet fetch and the holiday-pay qualification depend on this marker.
- **Holiday hours bypass overtime.** A worked holiday's hours never enter the overtime accumulator, so they can't also generate overtime for the week.
- **`GetInvoiceHolidaysAsync` reads `PaidHolidays` from the first timesheet** of the batch — the flag is a company-profile setting projected onto every timesheet row.
- **USA vs Canada is decided by the agency's billing location** (`InvoiceCountryStrategyResolver.ResolveForAgency(Invoice.CompanyProfile.AgencyId)`), not by the company being billed. The invoice stores no country.
- **Invoice numbers are per agency and gap-filling.** Never assume `InvoiceNumber` is globally unique or monotonic.

---

## Related Documents

- `PAYROLL_RULES.md` — worker-side flow and CPP/EI/tax deductions (pay-stub only, never on invoices); shares the hours breakdown, the per-request overtime scope, and the two-flow holiday design (with a *worker-wage* holiday base instead of company charges).
- `TIMESHEET_RULES.md` — how approved hours, breaks, missing hours and the two-threshold system are captured.
