# Timesheet Rules - Covenant/Sigook Platform

How worked hours are captured (clock in/out, manual entry), approved, and broken down into pay/bill categories.

**Source of truth:**
- `Covenant.Api/Covenant.Common/Entities/Request/TimeSheet.cs` — entity + clock/approval invariants
- `Covenant.Api/Covenant.Core.BL/Services/TimeSheetService.cs` (`TimesheetService` class) — clock in/out, CRUD, geofence
- `Covenant.Api/Covenant.Core.BL/Services/Accounting/Shared/TimesheetCalculatorService.cs` — **all hours-breakdown calculation** (not TimesheetService)
- Consumers: `PayStubService.GeneratePayStubForWorker` (payroll) and the invoice services (billing) — see `PAYROLL_RULES.md` / `BILLING_RULES.md`

---

## TimeSheet Entity (real fields)

`Covenant.Common/Entities/Request/TimeSheet.cs`:

| Field | Type | Notes |
|-------|------|-------|
| `Date` | DateTime | Work day |
| `IsHoliday` | bool | Set automatically from the `Holiday` catalog at creation |
| `TimeIn` / `TimeOut` | DateTime / DateTime? | Normalized: `TimeIn` is always midnight of `Date`, `TimeOut = TimeIn + hours` |
| `TimeInApproved` / `TimeOutApproved` | DateTime? | What payroll/billing actually use |
| `ClockIn` / `ClockOut` | DateTime? | Raw punch timestamps |
| `ClockInRounded` / `ClockOutRounded` | DateTime? | **Optional caller-supplied inputs; the backend never computes rounding** (used instead of raw clocks in `AddClockOut` when both present) |
| `MissingHours` / `MissingHoursOvertime` | TimeSpan | Extra paid hours (e.g. minimum-call rules), entered by agency |
| `MissingRateWorker` / `MissingRateAgency` | decimal | Rate for missing hours; `MissingRateWorker` ≤ 0 falls back to `WorkerRate`; `MissingRateAgency` is never consumed (invoices bill missing hours at `AgencyRate`) |
| `DeductionsOthers` + `DeductionsOthersDescription` | decimal (non-nullable) | Validated 0–1000 in `AddDeductionsOthers` |
| `BonusOrOthers` + `BonusOrOthersDescription` | decimal (non-nullable) | Ignored when ≤ 0 |
| `Reimbursements` + `ReimbursementsDescription` | decimal | Non-taxable; excluded from gross on the pay stub |
| `Comment` | string | E.g. `"Created and approved by {user}"` |
| `WorkerRequestId`, `TimeSheetTotal`, `TimeSheetTotalPayroll` | | Links to the worker booking and computed totals |

`BreakIsPaid`, `DurationBreak`, `HolidayIsPaid`, `OvertimeStartsAfter`, `WorkerRate` are **not** on the entity — they are joined from the Request/CompanyProfile into the calculation models (`TimeSheetApprovedPayrollModel`, etc.). `OvertimeStartsAfter` defaults to `CompanyProfile.OvertimeStartsAfter` (44 h), overridable per `CompanyProfileJobPositionRate`, with `ProvinceSetting.OvertimeStartsAfter` in between (fallback chain `JobPositionRate → ProvinceSetting → CompanyProfile`).

---

## Creation Paths

All entry points are in `TimesheetService` (`Covenant.Core.BL/Services/TimeSheetService.cs`):

### 1. `Register(requestId, workerLocationModel)` — worker mobile punch

1. **GPS geofence** (TimeSheetService.cs:155-197), active only when the `ValidateLocation` configuration flag is true:
   - Worker coordinates missing → rejected ("Your location is invalid").
   - Request has no coordinates → rejected.
   - Distance worker↔job pin `>= 101` meters → rejected ("You are too far from check point").
   - Every distance comparison emits a `TimesheetLocationDistanceCheck` telemetry event (`TimeSheetService.cs:165-179`) — it fires only when both worker and request coordinates are present; the two rejection branches above return without telemetry.
2. **Clock-in vs clock-out decision**: `TimesheetRepository.GetTimeSheeFromTheLast14Hours` fetches the latest timesheet in a 14-hour window.
   - None found → clock in (creates timesheet via `TimeSheet.WorkerClockIn`, `IsHoliday` set from the `Holiday` catalog by date + country).
   - Found but already approved → rejected.
   - Found and `IsClockOutValid(now)` (has ClockIn, no ClockOut, and elapsed ≤ `TimeLimits.DefaultTimeLimits.MaximumHoursDay` = **14 h**, a hardcoded static (`TimeLimits.cs:5`); the `TimeLimits:MaximumHoursDay` config key (12 in appsettings.json) does **not** affect this check — TimeSheet.cs:90-94) → clock out.
   - Otherwise → new clock in.

### 2. `AddClockIn(requestId, workerId, clockIn)` — agency enters a clock-in time

- Time must not be in the future.
- Duplicate check is **in-memory**: `entity.TimeSheets.Any(a => a.Date == clockInDate.Date)` (TimeSheetService.cs:65-66). There is no DB unique constraint on Worker + Date.

### 3. `CreateTimesheet(workerId, requestId, model)` — agency manual entry (pre-approved)

- Uses `TimeSheet.CreateTimeSheet` (TimeSheet.cs:139-160): sets `TimeIn` = midnight, `TimeOut = TimeIn + hours`, and immediately sets `TimeInApproved`/`TimeOutApproved` to the same values.
- Validations: `hours < 24` (TimeSheet.cs:141 — there is **no** minimum-hours rule) and `date` not older than 1 year.
- Duplicate check is in-memory via `workerRequest.ContainsTimeSheet(timeSheet)` (TimeSheetService.cs:98).
- Rejected workers can still get timesheets until `WorkerRequest.LimitDateToAddTimeSheet`.
- Also sets MissingHours/rates, DeductionsOthers, BonusOrOthers, Reimbursements.

---

## Clock-Out Rules (`TimeSheet.AddClockOut`, TimeSheet.cs:50-88)

- Requires an existing `ClockIn`; `clockOut` must be after it.
- Must wait **3 minutes** after clock-in (`TotalOfMinutesToWaitToClockOut`).
- Re-clock-out allowed only within **5 minutes** of the previous clock-out (`TotalOfMinutesToAllowClockOutAgain`).
- Duration is capped at 23:59; then normalized: `TimeIn = date at 00:00`, `TimeOut = TimeIn + totalHours` (overnight shifts are folded into a single date, preserving the hour count).
- If `ClockInRounded` and a caller-supplied `clockOutRounded` both exist, those are used for the duration instead of the raw clocks. No rounding table (nearest-15-minutes etc.) exists anywhere in the backend.

---

## Approval

`TimeSheet.AddApprovedTime` (TimeSheet.cs:96-104), called from `TimesheetService.UpdateTimesheet`:

- `TimeInApproved` and `TimeOutApproved` must be on the same date.
- `TimeInApproved <= TimeOutApproved`.
- `TimeInApproved.Date` must equal `TimeIn.Date`.

Deletion/locking: `TimesheetService.RemoveTimeSheet` refuses to delete a timesheet already used by an invoice or pay stub (`TimesheetRepository.TimesheetUsedByAccounting`); the invoice/pay stub must be deleted first.

---

## Hours Breakdown

Single implementation for payroll, invoicing and holiday-pay base: `TimesheetCalculatorService.CalculateHoursBreakdown` (TimesheetCalculatorService.cs:82-138).

### 1. Total hours and breaks

```csharp
var totalHours = (timeOut - timeIn).Subtract(breakIsPaid ? durationBreak : TimeSpan.Zero);
```

`DurationBreak` is subtracted **when `BreakIsPaid` is `true`** (TimesheetCalculatorService.cs:96). The flag name reads inverted relative to its effect — treat `BreakIsPaid = true` as "deduct the break from paid hours" when reasoning about this code.

### 2. Holiday gate

Hours become `HolidayHours` only when `IsHoliday && HolidayIsPaid` (TimesheetCalculatorService.cs:99). In that case all hours of the day are holiday hours and are **not** added to the weekly accumulator. A holiday with `HolidayIsPaid = false` is processed as a normal day. (Invoicing hardcodes `holidayIsPaid: true`; pay stubs honor the flag.)

### 3. Weekly accumulation — per Worker + Week + Request

Callers group timesheets by week (Sunday–Saturday) then by `RequestId` and pass a `ref accumulatedHours` per group. Overtime therefore accumulates **per request**: 40 h on Request A plus 40 h on Request B in the same week produces no overtime.

### 4. Two thresholds → three buckets

| Bucket | Condition |
|--------|-----------|
| `OvertimeHours` | Accumulated hours beyond `ts.OvertimeStartsAfter` (per-timesheet value from CompanyProfile/job position; **not** a hardcoded 44) |
| `OtherRegularHours` | Accumulated hours beyond `TimeLimits.MaxHoursWeek` (config, 44 h) that are not overtime — straight rate, separate pay/bill line |
| `RegularHours` | Everything else |

Invoices and `CalculateHolidayPayBase` use the per-timesheet `OvertimeStartsAfter` value as-is; pay stubs read it once from `timesheets.First()` (`PayStubService.cs:237`) and reuse it for the whole batch.

Excess is computed by `CalculateExcessHours` (TimesheetCalculatorService.cs:144-155): a day that crosses a threshold is split; a day starting past it is entirely excess.

**Example** (`OvertimeStartsAfter = 44`): Saturday shift of 8 h with 40 h already accumulated → Regular 4 h, Overtime 4 h, accumulated 48 h.

### 5. Night shift — not computed

`CalculateHoursBreakdown` explicitly does not support it (TimesheetCalculatorService.cs:80) and downstream consumers pass 0. The `NightShift` columns/fields exist and are always zero. Do not add night-shift logic.

---

## Statutory Holidays

Holidays are **database catalog rows**, not code: `Holiday` table queried by date + country code (`CatalogRepository.IsHoliday`, CatalogRepository.cs:150; `GetHolidaysInWeek`, :152-160). `TimeSheet.IsHoliday` is stamped automatically at creation/clock-in.

Canadian statutory holidays typically seeded in the catalog: New Year's Day, Good Friday, Victoria Day, Canada Day, Civic Holiday, Labour Day, Thanksgiving, Christmas Day, Boxing Day (plus provincial ones like Family Day). The DB table is authoritative — verify there before assuming a date is a holiday.

Pay consequences (worked at 1.5×, not-worked "/20" entitlement) are documented in `PAYROLL_RULES.md`.

---

## Provincial Overtime Thresholds (legal reference only)

The code applies a single weekly `OvertimeStartsAfter` threshold plus the global 44 h `MaxHoursWeek`. The threshold is resolved per timesheet as `JobPositionRate → ProvinceSetting → CompanyProfile` (`TimeSheetRepository.cs:181-186, :227-231, :275-279, :324-328`): provinces **are** modeled via `ProvinceSetting`, but only as a single weekly threshold — no daily rules. `ProvinceSetting.PaidHolidays` participates in the same fallback for payroll (`TimeSheetRepository.cs:236-237`). The table below is legal reference for seeding `ProvinceSetting`:

| Province | Weekly OT threshold |
|----------|---------------------|
| ON, NB, Federal | 44 h |
| BC, QC, SK, NL | 40 h |
| AB, MB | 44 h / 40 h (+ daily rules not supported by the system) |
| NS, PEI | 48 h |

Daily overtime thresholds (BC/AB/MB) are not supported.

---

## Financial Adjustments Flow

| Field | Pay stub effect (see `PAYROLL_RULES.md`) |
|-------|------------------------------------------|
| `BonusOrOthers` | Added to gross, grouped by description |
| `Reimbursements` | Excluded from gross/deductions, added to net pay |
| `DeductionsOthers` | Added to total deductions (0–1000 per timesheet) |
| `MissingHours` / `MissingHoursOvertime` | Paid at `MissingRateWorker` (×1.5 for the overtime variant); invoices bill them at `agencyRate` (`InvoiceService.cs:343, :363`) — `MissingRateAgency` is projected but never consumed |
