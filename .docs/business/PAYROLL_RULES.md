# Payroll Rules - Covenant/Sigook Platform (Canada)

How pay stubs are generated and how earnings and deductions are actually computed.

**Source of truth:**
- `Covenant.Api/Covenant.Core.BL/Services/PayStubService.cs` — pay stub generation (`Generate` → `GeneratePayStubForWorker`, `CreateManualPayStub`)
- `Covenant.Api/Covenant.Core.BL/Services/Shared/TimesheetCalculatorService.cs` — hours breakdown, amounts, deductions
- `Covenant.Api/Covenant.Infrastructure/Repositories/Accounting/DeductionsRepository.cs` — CPP/tax lookup tables
- `Covenant.Api/Covenant.Common/Configuration/Rates.cs` + `Covenant.Api/appsettings.json` (`Rates` section) — multipliers
- Hours/timesheet rules (breaks, holiday gate, overtime thresholds, statutory holiday catalog): see `TIMESHEET_RULES.md`

---

## Generation Flow

`PayStubService.GeneratePayStubForWorker`:

1. Load approved timesheets for the worker (`TimesheetRepository.GetTimeSheetForCreatingPayStubs`).
2. Group by week (Sunday–Saturday), then by `RequestId`. Overtime accumulates **per request per week**, never across requests.
3. For each timesheet, `TimesheetCalculatorService.CalculateHoursBreakdown` splits hours into Regular / OtherRegular / Overtime / Holiday (see `TIMESHEET_RULES.md`).
4. Hours are accumulated into dictionaries **keyed by rate**; one `PayStubItem` is created per (item type, rate) pair.
5. Public-holiday entitlement is evaluated per week (see below).
6. Totals, deductions, net pay; save.

Pay stub number format: `PS-{number:0000}-{yy}` (`PayStubService.GeneratePayStubNumber`, PayStubService.cs:835).

---

## Earnings (PayStubItem types)

| Item | Rate used | Source |
|------|-----------|--------|
| Regular | `WorkerRate` | Hours up to both thresholds |
| OtherRegular | `WorkerRate` (straight rate) | Hours beyond `TimeLimits.MaxHoursWeek` (44 h) that are not yet overtime |
| Overtime | `WorkerRate × Rates.OverTime` (1.5) | Hours beyond the request's `OvertimeStartsAfter` |
| StatutoryWorkedHoliday | `WorkerRate × Rates.Holiday` (1.5) | All hours of a timesheet with `IsHoliday && HolidayIsPaid` |
| StatutoryHoliday | Flat amount | Public-holiday pay for a holiday the worker did not work |
| Missing | `MissingRateWorker` (falls back to `WorkerRate` when ≤ 0) | `TimeSheet.MissingHours` |
| MissingOvertime | `MissingRateWorker × Rates.OverTime` | `TimeSheet.MissingHoursOvertime` |
| BonusOthers | Flat amount | `TimeSheet.BonusOrOthers`, grouped by description |
| Reimbursement | Flat amount | `TimeSheet.Reimbursements` — **excluded from gross**, added back into net |

**Night shift is deprecated**: `CalculateHoursBreakdown` does not compute it (TimesheetCalculatorService.cs:90 comment) and `GeneratePayStubForWorker` hardcodes `nightShift: 0` (PayStubService.cs:335).

### Two-tier hours example (verified against `CalculateHoursBreakdown`)

`MaxHoursWeek = 44`, request configured with `OvertimeStartsAfter = 50`, worker logs 55 h in one week on one request:

- Regular: 44 h × WorkerRate
- OtherRegular: 6 h (44→50) × WorkerRate (straight rate, separate line item)
- Overtime: 5 h (50→55) × WorkerRate × 1.5

When `OvertimeStartsAfter == MaxHoursWeek` (the common case, default 44), OtherRegular is zero and everything past 44 h is overtime.

---

## Gross, Vacations, Total Earnings

From PayStubService.cs:399-404:

```
GrossPayment  = Σ all PayStubItems except type Reimbursement
Vacations     = GrossPayment × Rates.Vacations   (0.04)
TotalEarnings = GrossPayment + Vacations
```

- Gross therefore includes Regular, OtherRegular, Overtime, worked-holiday, Missing, MissingOvertime, statutory-holiday and bonus items. Public-holiday pay is **already inside gross** — never add it again on top of TotalEarnings.
- Vacation pay is a flat 4% (`Rates:Vacations` in appsettings.json). There is no "6% after 5 years" logic.

---

## Public Holiday Pay (holiday not worked)

Evaluated per week in `GeneratePayStubForWorker` (PayStubService.cs:345-365) against the `Holiday` catalog table (`CatalogRepository.GetHolidaysInWeek`).

Decision order (`RegularWageWorker.CalculateAmount`, Covenant.Common/Models/Accounting/PayStub/RegularWageWorker.cs):

1. Holiday already paid on a previous pay stub → $0.
2. Custom per-worker value configured (`WorkerProfileHoliday.StatPaidWorker`) → that amount.
3. Worker did not work any qualifying day (day before, the holiday, or day after — `DateExtensions.GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay`) → $0, not entitled.
4. Otherwise: `Amount = HolidayPayBase / 20`.

`HolidayPayBase` = gross earnings + 4% vacation over the **four work weeks before** the holiday's work week, recomputed from approved timesheets by `TimesheetCalculatorService.CalculateHolidayPayBase` (not from previous pay stubs, so it does not depend on generation order).

Example: worker earned $3,000 gross + $120 vacation in the prior four weeks → $3,120 / 20 = **$156.00**.

Working ON the holiday is a separate flow: those hours are paid as StatutoryWorkedHoliday at 1.5× (see table above).

---

## Deductions

All computed by `TimesheetCalculatorService.CalculateDeductions` (TimesheetCalculatorService.cs:39-79) on **TotalEarnings**.

### Payment frequency

Derived from the pay stub's week span (`DateExtensions.GetNumberOfWeeksIn` between `dateWorkBegins` and `dateWorkEnd`):

| numberOfWeeks | `PayPeriod` used |
|---------------|------------------|
| 1 | `Weekly` |
| 2 | `BiWeekly` |
| 3 | `SemiMonthly` |
| other | `Monthly` |

### CPP — database lookup, no formula

`DeductionsRepository.GetCpp(earnings, year, payPeriod)`: single row of `CppDeductions` where `earnings >= From && earnings <= To`, returns the stored `Cpp` amount. There is **no** 5.95% formula, no basic exemption, no annual maximum in code. `Rates.CanadaPensionPlan` (0.051) exists in configuration but is **not used** for the CPP pay stub deduction.

### EI — the only formula

```
Ei = TotalEarnings × Rates.EmploymentInsurance   (0.0163 in appsettings.json)
```

No maximum insurable earnings cap is implemented.

### Federal / Provincial tax — database lookup

`DeductionsRepository.GetTax(earnings, year, payPeriod, taxType, category)`: single row of `TaxDeductions` where `earnings >= From && earnings < To` for the pay-period earnings **directly** (no annualization), returning the column for the worker's claim code (`TaxCategory` Cc0–Cc10, default `Cc1`). Rows are keyed by year, `PayPeriod` and `TaxType`; provincial rows have no province column (not per-province in code).

### Per-worker overrides — `WorkerProfileTaxCategory`

`Covenant.Common/Entities/Worker/WorkerProfileTaxCategory.cs` (1:1 with WorkerProfile), applied in `CalculateDeductions`:

| Field | Effect |
|-------|--------|
| `Cpp` (decimal?) | Replaces the CPP table lookup with a fixed amount (e.g. 0 for subcontractors) |
| `Ei` (decimal?) | Replaces the EI formula with a fixed amount |
| `FederalCategory` / `ProvincialCategory` | Claim-code column used in the tax lookups (`Cc0` yields $0 rows) |

This is the real subcontractor / no-deduction mechanism. There are **no** age-based CPP/EI exceptions in code.

### Other deductions and net pay

From PayStubService.cs:414-422:

```
TotalDeductions = Cpp + Ei + FederalTax + ProvincialTax + Σ DeductionsOthers (from timesheets)
TotalPaid       = TotalEarnings − TotalDeductions + Σ Reimbursements
```

`TimeSheet.AddDeductionsOthers` validates each deduction to the 0–1000 range.

---

## Payment Date

PayStubService.cs:425-427: if the pay stub has wage details (timesheet-driven) → `DateExtensions.GetPaymentDateForExternalWorkers` (Friday of the week **after** the last work week); otherwise `GetPaymentDateForInternalWorkers` (Friday of the work week itself).

---

## Tax Table Maintenance

CPP and tax brackets are **database rows** in two consolidated tables (`CppDeduction` and `TaxDeduction` entities in `Covenant.Common/Entities/Accounting/Deductions/`), keyed by year plus a `PayPeriod` discriminator (`Weekly`, `BiWeekly`, `SemiMonthly`, `Monthly`); `TaxDeduction` adds a `TaxType` discriminator (`Federal`, `Provincial`) — not code constants.

Bracket lookups differ by concept and that is intentional: CPP matches `earnings >= From && earnings <= To`, taxes match `earnings >= From && earnings < To`.

### CPP: from the CRA PDF, through blob storage

The CPP table is loaded from the PDF the CRA publishes (T4032), with no manual transcription in between:

1. The PDF is dropped in the `cra-tables` blob container, named `CPP <WEEKLY|BIWEEKLY|SEMIMONTHLY|MONTHLY> <YYYY>.pdf` (space, `_` or `-` as separator, case-insensitive).
2. The `CraTableUploaded` blob trigger in `Sigook.Functions` reads the pay period and the year from that name. A name that does not follow the convention is reported to Teams and never reaches the API.
3. The function calls `POST api/Accounting/Deduction/Cpp/Blob` with the blob name, the pay period and the year, authenticated with the same client credentials as the scheduled tasks.
4. `CppDeductionImportService` downloads the blob, `CppPdfParser` (PdfPig) reads the four `From - To  CPP` blocks printed on every line, and `CppTableValidator` checks the result before anything is written: the brackets must start at `0.00`, be contiguous (`From == previous To + 0.01`), never overlap and never lower the contribution. A table that fails validation is rejected and the stored one is left untouched.
5. `DeductionsRepository.ReplaceCpp(year, payPeriod, rows)` swaps the whole table for that year and pay period in a single transaction.

The 2026 weekly table is 8,928 brackets, from `0.00 - 67.30` to `9344.62 - 9354.61` ($552.30).

Federal and provincial tax tables are still loaded from an Excel upload (`POST api/Accounting/Deduction/{FederalTax|ProvincialTax}/{period}/Excel`, ClosedXML).

---

## T4

`PayStubService.GenerateT4(from, to)` only produces an **Excel summary** of pay stubs (ClosedXML workbook), exposed via `ReportsController` (Covenant.Api/Controllers/Sigook/Agency/Accounting/ReportsController.cs:76). No CRA slip generation or T4A/contractor handling exists.
