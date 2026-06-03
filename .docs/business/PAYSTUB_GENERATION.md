# Pay Stub Generation

How `PayStubService.GeneratePayStubForWorker` builds a pay stub from approved timesheets.

## Entry Point

`PayStubService.Generate(agencyIds, workerIds)` iterates over each worker and calls `GeneratePayStubForWorker(agencyIds, workerId)` for each one. A `SemaphoreSlim` ensures only one generation runs at a time.

**Key file:** `Covenant.Api/Covenant.Core.BL/Services/PayStubService.cs`

## Step-by-Step Flow

### Step 1: Get Approved Timesheets
- Source: `timeSheetRepository.GetTimeSheetForCreatingPayStubs(agencyIds, workerId)`
- Returns all approved timesheets for the worker across the given agencies
- If none exist, returns early

### Step 2: Get Next PayStub Number
- Sequential number from `payStubRepository.GetNextPayStubNumbers(1)`
- Used for the payroll number identifier (format: `PS-0000-YY`)

### Step 3: Process Timesheets (the main loop)

Timesheets are processed in a nested loop structure:

```
Week groups (ordered by week number)
  └── Request groups (grouped by RequestId within each week)
       └── Days (ordered by date within each request)
```

**Why group by week then by request?** Overtime is accumulated per request within each week. The `accumulatedHours` ref variable resets for each request group.

#### Step 3.2: For Each Day

1. **Calculate hours breakdown** via `calculatorService.CalculateHoursBreakdown(...)`:
   - Regular hours, Other regular hours, Overtime hours, Holiday hours
   - Uses `ref accumulatedHours` to track weekly overtime threshold
   - Uses `overtimeStartsAfter` from the first timesheet and `timeLimits.MaxHoursWeek`

2. **Calculate wage amounts** for each hour type:
   - `regularAmount = workerRate * regularHours`
   - `otherRegularAmount = workerRate * otherRegularHours`
   - `overtimeAmount = workerRate * overtimeMultiplier * overtimeHours`
   - `holidayAmount = workerRate * holidayMultiplier * holidayHours`
   - `missingAmount = missingRate * missingHours`
   - `missingOvertimeAmount = missingRate * overtimeMultiplier * missingOvertimeHours`
   - `missingRate` defaults to `workerRate` if not set or <= 0
   - All amounts rounded via `DefaultMoneyRound()`

3. **Create TimeSheetTotalPayroll entity** via `calculatorService.CreateTimeSheetTotalPayrollEntity(...)`:
   - Links back to the original TimeSheet
   - Stores hours breakdown and accumulated hours

4. **Accumulate hours by rate** into dictionaries (for PayStubItem creation later):
   - `regularByRate[workerRate] += regularHours`
   - `otherRegularByRate[workerRate] += otherRegularHours`
   - `overtimeByRate[overtimeMultiplier * workerRate] += overtimeHours`
   - `workedHolidayByRate[holidayMultiplier * workerRate] += holidayHours`
   - `missingByRate[missingRate] += missingHours`
   - `missingOvertimeByRate[overtimeMultiplier * missingRate] += missingOvertimeHours`
   - Only accumulates if the corresponding amount > 0
   - Hours are grouped by rate so workers with multiple rates get separate line items

5. **Accumulate extras** (also inside the main loop, per day):
   - **Bonuses** — grouped by description into `bonusGroups` dictionary
   - **Reimbursements** — grouped by description into `reimbursementGroups` dictionary
   - **Other deductions** — one `PayStubOtherDeduction` per timesheet entry with `DeductionsOthers > 0`

6. **Create PayStubWageDetail** linked to the TimeSheetTotalPayroll:
   - One per day/timesheet — preserves per-day traceability
   - Used downstream for queries that navigate PayStub → WageDetail → TimeSheetTotal → TimeSheet → WorkerRequest

#### Step 3.3: Public Holidays

For each week, looks up statutory holidays (`catalogRepository.GetHolidaysInWeek`) and, for each holiday, resolves the public holiday pay (paid to entitled workers who did **not** work the holiday):

1. **Entitlement flags** from `payStubRepository.GetWorkerRegularWages(workerProfileId, holiday, qualifyingDays)`: `HolidayWasPaid` (already paid in a prior pay stub), `CustomPublicHolidayValue` (agency override), `IsEntitledToReceiveHolidayPay` (worked at least one qualifying day around the holiday).
2. **Base wage** from `calculatorService.CalculateHolidayPayBase(workerProfileId, lookbackStart, holidayWeekEnd)`: the worker's gross earnings (regular + other-regular + overtime + worked-holiday + missing) **plus vacation pay** over the worked timesheets of the four work weeks before the holiday's week. The window is computed with `holiday.GetEnd()` (last Saturday of the week before the holiday's Sunday–Saturday week) and `.GetStart()` (four weeks earlier). Source is the worked timesheets, not previously generated pay stubs, so the amount does not depend on pay stub generation order.
3. **Resolution** via `RegularWageWorker.CalculateAmount()`: `0` if already paid or not entitled, the custom value when present, otherwise `base / 20`.

The resulting amount (when `> 0`) becomes a `PayStubPublicHoliday` and a `StatutoryHoliday` `PayStubItem`.

### Step 4: Create PayStubItems

Converts the accumulated dictionaries into `PayStubItem` entities. Each dictionary entry (rate → total hours) becomes one line item on the pay stub:

| Type | Source | Rate Key | Example |
|------|--------|----------|---------|
| Regular | `regularByRate` | `workerRate` | "Regular Hours: 40h @ $20" |
| OtherRegular | `otherRegularByRate` | `workerRate` | "Other Regular Hours: 5h @ $20" |
| Overtime | `overtimeByRate` | `overtimeMultiplier * workerRate` | "Overtime Hours: 8h @ $30" |
| StatutoryWorkedHoliday | `workedHolidayByRate` | `holidayMultiplier * workerRate` | "Statutory Worked Holiday Pay: 8h @ $40" |
| Missing | `missingByRate` | `missingRate` | "Missing Hours: 2h @ $18" |
| MissingOvertime | `missingOvertimeByRate` | `overtimeMultiplier * missingRate` | "Missing Overtime Hours: 1h @ $27" |
| StatutoryHoliday | `publicHolidays` | total amount (qty=1) | "Statutory Holiday: $150" |
| BonusOthers | `bonusGroups` | total amount (qty=1) | "Bonus: $100" |
| Reimbursement | `reimbursementGroups` | total amount (qty=1) | "Reimbursement: $50" |

Zero-total items are filtered out after creation.

### Step 5: Calculate Totals

```
grossPayment   = sum of payStubItems totals WHERE type != Reimbursement
vacations      = grossPayment * vacationRate
totalEarnings  = grossPayment + vacations
```

Public holiday pay is included in `grossPayment` because `StatutoryHoliday` items are regular `PayStubItem`s (not excluded from the sum).

### Step 6: Calculate Deductions

Determines the **payment period** from the work date range (adjusted to week boundaries: Sunday–Saturday). The number of weeks is passed to `calculatorService.CalculateDeductions(totalEarnings, numberOfWeeks, year, workerProfileId)` which internally handles:
- **CPP** (Canada Pension Plan) — can be overridden by worker's tax category
- **EI** (Employment Insurance) — can be overridden
- **Federal Tax** — looked up by earnings + tax category
- **Provincial Tax** — looked up by earnings + tax category

```
totalDeductions = CPP + EI + federalTax + provincialTax + otherDeductions
```

### Step 7: Calculate Net Pay

```
reimbursementTotal = sum of payStubItems WHERE type == Reimbursement
totalPaid = totalEarnings - totalDeductions + reimbursementTotal
```

Reimbursements are added back because they are not taxable income.

### Step 8: Build PayStub Entity

Creates the `PayStub` entity and attaches:
- `PayStubItem`s (all line items including reimbursements)
- `PayStubWageDetail`s (per-day traceability to timesheets)
- `PayStubPublicHoliday`s
- `PayStubOtherDeduction`s

Payment date: if wage details exist → `GetPaymentDateForExternalWorkers()`, otherwise → `GetPaymentDateForInternalWorkers()`.

### Step 9: Save

Persists to database via `payStubRepository.Create(payStub)` and `SaveChangesAsync()`.

## Key Entities and Their Purpose

| Entity | Granularity | Purpose |
|--------|-------------|---------|
| `PayStub` | One per worker per pay period | The pay stub itself with all totals |
| `PayStubItem` | One per type per unique rate | Line items shown on the pay stub (consolidated) |
| `PayStubWageDetail` | One per day/timesheet | Per-day breakdown for traceability back to timesheets |
| `TimeSheetTotalPayroll` | One per day/timesheet | Hours breakdown stored per timesheet |
| `PayStubPublicHoliday` | One per statutory holiday | Holiday pay calculated from regular wages |
| `PayStubOtherDeduction` | One per timesheet with deductions | Additional deductions from timesheets |

## Key Dependencies of PayStubService

| Dependency | Purpose |
|------------|---------|
| `ITimesheetCalculatorService` | Hours breakdown, amount calculations, TimeSheetTotal creation, deductions calculation |
| `ITimeSheetRepository` | Fetch approved timesheets |
| `IPayStubRepository` | PayStub CRUD, next number, regular wages lookup |
| `ICatalogRepository` | Public holidays lookup |
| `Rates` (config) | Overtime multiplier, holiday multiplier, vacation rate |
| `TimeLimits` (config) | Max weekly hours for overtime threshold |

## Data Flow Diagram

```
Approved Timesheets
       │
       ▼
┌─────────────────────────────────────┐
│  Loop: Week → Request → Day        │
│                                     │
│  For each day:                      │
│  ├─ CalculateHoursBreakdown()       │
│  ├─ Calculate amounts per type      │
│  ├─ CreateTimeSheetTotalPayroll()   │
│  ├─ Accumulate hours by rate ───────┼──► Dictionaries (rate → hours)
│  ├─ Accumulate extras ──────────────┼──► bonusGroups, reimbursementGroups, otherDeductions
│  └─ Create PayStubWageDetail ───────┼──► allWageDetails (per-day trace)
│                                     │
│  For each week:                     │
│  └─ Get public holidays ────────────┼──► publicHolidays
└─────────────────────────────────────┘
       │
       ▼
Dictionaries + extras ──► PayStubItems (consolidated line items)
       │
       ▼
Calculate: gross (excl. reimbursements) → vacations → totalEarnings
       │
       ▼
calculatorService.CalculateDeductions(totalEarnings, weeks, year, worker)
       │
       ▼
Net pay = totalEarnings - totalDeductions + reimbursements
       │
       ▼
Build PayStub entity with all children → Save
```
