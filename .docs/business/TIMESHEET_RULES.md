# Timesheet Rules - Covenant/Sigook Platform

How worked hours are captured (clock in/out, manual entry), approved, and broken down into pay/bill categories.

**Source of truth:**
- `Covenant.Api/Covenant.Common/Entities/Requests/TimeSheet.cs` — entity + clock/approval invariants
- `Covenant.Api/Covenant.Core.BL/Services/Requests/TimesheetService.cs` (`TimesheetService` class) — clock in/out, CRUD, geofence
- `Covenant.Api/Covenant.Core.BL/Services/Accounting/TimesheetCalculatorService.cs` — **all hours-breakdown calculation** (not TimesheetService)
- Consumers: `PayStubService.GeneratePayStubForWorker` (payroll) and the invoice services (billing) — see `PAYROLL_RULES.md` / `BILLING_RULES.md`

---

## TimeSheet Entity (real fields)

`Covenant.Common/Entities/Requests/TimeSheet.cs`:

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
| `Comment` | string | The `Comments` sent by the agency/company form when present; otherwise the audit text `"Created and approved by {user}"` / `"Updated and approved by {user}"` |
| `WorkerRequestId`, `TimeSheetTotal`, `TimeSheetTotalPayroll` | | Links to the worker booking and computed totals |

`BreakIsPaid`, `DurationBreak`, `HolidayIsPaid`, `OvertimeStartsAfter`, `WorkerRate` are **not** on the entity — they are joined from the Request/CompanyProfile into the calculation models (`TimeSheetApprovedPayrollModel`, etc.). `OvertimeStartsAfter` defaults to `CompanyProfile.OvertimeStartsAfter` (44 h), overridable per `CompanyProfileJobPositionRate`, with `ProvinceSetting.OvertimeStartsAfter` in between (fallback chain `JobPositionRate → ProvinceSetting → CompanyProfile`).

---

## Creation Paths

All entry points are in `TimesheetService` (`Covenant.Core.BL/Services/Requests/TimesheetService.cs`):

### 1. `Register(requestId, workerLocationModel)` — worker mobile punch

1. **Booking check**: `WorkerRequestRepository.GetWorkerRequestInfo` must find the worker booked on the request with `StartWorking <= now`; otherwise rejected ("You are not assigned to this request yet").
2. **GPS geofence** (`TimesheetService.ValidateDistance`), always active in every environment (the old `ValidateLocation` flag is gone; to test locally, set the request pin to your own coordinates):
   - Worker coordinates missing → rejected ("Your location is invalid").
   - Request has no coordinates → rejected.
   - Distance worker↔job pin greater than `PunchConfiguration.CheckpointRadiusMeters` (**100 m** in `appsettings.json` and as the `PunchConfiguration__CheckpointRadiusMeters` App Setting of `sigook-api` and `sigook-api-staging`; the class default is also 100) → rejected. The message tells the worker how far and in which cardinal direction they are from the pin ("You are too far from check point: 165 m to the south. Please get closer."); distances are metres below 1 km, one decimal up to 10 km ("2.5 km") and whole kilometres beyond ("875 km"), and the clients' live strips format the same way. In Sigook.App the punch button is disabled while the live fix is clearly outside the radius (`distance − accuracy > radius`), and the strip then shows the worker number, name and request number so a screenshot is enough for support; with a borderline or missing fix the button stays enabled and the API decides.
   - Both clients (mobile app and worker portal) show that message as-is in a dismissible red card under the punch button (no modal or toast, so the job pin, the live strip and the worker's coordinates stay visible in one screenshot) with a reference line (worker number id + name, request number id, local time) that identifies the case; the card clears on the next punch attempt, on Retry and after a successful punch.
   - Every distance comparison emits a `TimesheetLocationDistanceCheck` telemetry event with `RequestId`, `RequestNumberId`, `WorkerId`, `WorkerNumberId`, `WorkerFullName`, `WorkerEmail`, both coordinate pairs and the `DistanceMeters` measurement — it fires only when both worker and request coordinates are present; the two rejection branches above return without telemetry.
   - **Punch status in one call**: `GET api/worker/requests/{id}/timesheet/punch-status/{lat}/{lng}` (`TimesheetService.GetPunchStatus` → `PunchStatusModel`: `clockType`, `clockIn`, `clockOut`, `dayHours`, `weekHours`, with "today" and "Sunday" in the job's time zone) is everything the punch card shows: the next punch, today's Clock in / Clock out stamps (the open punch of the last 14 h, else today's entry) and a "This week: N h" total (entries from Sunday), refreshed after every punch and on Retry. The timesheet list is only fetched by the Time Sheet tab. The older `clock-type` endpoint is `[Obsolete]` and stays only for mobile builds released before `punch-status`.
   - **Live distance before the punch**: `WorkerService.GetRequestDetail` (`GET api/worker/requests/{id}`) fills `punchCheckpoint` (pin latitude/longitude + `radiusMeters` from the same constant) when the pin has coordinates; otherwise it is `null`. The clients never hardcode the radius. Both clients watch the device position and show a strip under the clock using the same haversine and 8-point bearing as the API, in three bands: green "At the check point" when `distance + GPS accuracy <= radius`; amber "About 90 m from the check point (GPS ±25 m). Move a bit closer to be safe." when the distance passes but the accuracy margin does not; red "About 165 m south of the check point. Get closer before you clock in." when it would be rejected. When the worker punches, the strip adopts the fresh fix that was sent, so dialog and strip agree. The strip is advisory: the button stays enabled and `Register` remains the only gate.
   - **Diagnosable from one screenshot**: both strips print the worker's live coordinates with the GPS accuracy ("37.697173, -77.669072 (±5 m)"), and the request's pin coordinates are printed under the job address (app job header on the Detail and Punch Card tabs; portal `RequestLocationCard`). Both clients re-fetch the request detail (and therefore the pin) when they return to the foreground / the tab becomes visible, on Retry and after every punch, so a moved pin is picked up without leaving the page. The worker portal (`Sigook.Web` `PunchCard.vue`, `src/shared/utils/geo.ts`, browser `watchPosition` in high accuracy) shows a "Status unavailable / Retry" state when the status call fails instead of "Nothing to punch". The app's single-position-stream constraint is documented in `.docs/technical/SIGOOK_APP_STRUCTURE.md` (Core Services → `LocationService`).
3. **Clock-in vs clock-out decision** (`TimesheetService.NextPunch`, shared with `GetClockType` so the button and the punch always agree): `TimesheetRepository.GetLatestTimesheet(workerId, requestId)` fetches the worker's latest punched timesheet on the request (by `ClockIn` desc, no date filter).
   - None, or not `IsClockOutValid(now)` → new clock in (creates timesheet via `TimeSheet.WorkerClockIn`, `IsHoliday` set from the `Holiday` catalog by date + country). A punch left open for more than 14 h stays open for the agency to fix; the next punch starts a new day.
   - **Closed today** (latest punch has `ClockOut` and its `Date` equals today in the job's time zone) → `None`: the day is finished, `Register` rejects with "You already clocked out today." and no second timesheet is created for the day. A punch closed on another date (yesterday's day shift, or last night's shift that ended this morning and is dated by its clock-in) lets the worker clock in again. The clients render this state as "Shift completed" (app: disabled green button; portal: `done` phase) with today's stamps and `dayHours` still filled.
   - `IsClockOutValid(now)` (has ClockIn, no ClockOut, and elapsed ≤ `TimeLimits.DefaultTimeLimits.MaximumHoursDay` = **14 h**, a hardcoded static (`TimeLimits.cs:5`); the `TimeLimits:MaximumHoursDay` config key (12 in appsettings.json) does **not** affect this check — TimeSheet.cs:90-94) → clock out. Night shifts need no date selection: the open punch from the previous calendar day is found the same way.
   - Open but already approved → rejected ("already approved"); `GetClockType` returns `None` for that state.
   - "Now" is evaluated in the job site's time zone (coordinates of `Request.JobLocation`), falling back to the caller's coordinates, never in server time.

### 2. `AddClockIn(requestId, workerId, clockIn)` — agency enters a clock-in time

- Time must not be in the future.
- Duplicate check is **in-memory**: `entity.TimeSheets.Any(a => a.Date == clockInDate.Date)` (TimesheetService.cs:65-66). There is no DB unique constraint on Worker + Date.

### 3. `CreateTimesheet(workerId, requestId, model)` — agency manual entry (pre-approved)

- Uses `TimeSheet.CreateTimeSheet` (TimeSheet.cs:139-160): sets `TimeIn` = midnight, `TimeOut = TimeIn + hours`, and immediately sets `TimeInApproved`/`TimeOutApproved` to the same values.
- Validations: `hours < 24` (TimeSheet.cs:141 — there is **no** minimum-hours rule) and `date` not older than 1 year.
- Duplicate check is in-memory via `workerRequest.ContainsTimeSheet(timeSheet)` (TimesheetService.cs:98).
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
var totalHours = (timeOut - timeIn).Subtract(breakIsPaid ? TimeSpan.Zero : durationBreak);
if (totalHours < TimeSpan.Zero) totalHours = TimeSpan.Zero;
```

`DurationBreak` is subtracted only when the break is **not** paid (`BreakIsPaid = false`). A paid break stays inside the paid hours, which is what the request form ("Duration break is paid") promises. A day whose span is shorter than the break (for example an unworked holiday recorded as 00:00–00:00) yields zero hours, never a negative value.

Pay stubs, invoices, subcontractor reports and the holiday-pay base all pass the request's `BreakIsPaid`, so billed hours and paid hours always agree.

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
