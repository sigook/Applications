# Pay Stub Generation

How `PayStubService.GeneratePayStubForWorker` builds a pay stub from approved timesheets.

## Entry Point

`PayStubService.GeneratePayStubs(agencyIds)` fetches all workers with approved timesheets, then calls `GeneratePayStubForWorker(agencyIds, workerId)` for each one.

**Key file:** `Covenant.Api/Covenant.Core.BL/Services/PayStubService.cs`

## Step-by-Step Flow

### Step 1: Get Approved Timesheets
- Source: `timeSheetRepository.GetTimeSheetForCreatingPayStubs(agencyIds, workerId)`
- Returns all approved timesheets for the worker across the given agencies
- If none exist, returns early

### Step 2: Get Next PayStub Number
- Sequential number from `payStubRepository.GetNextPayStubNumbers(1)`
- Used for the payroll number identifier

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

2. **Calculate wage amounts** for each hour type:
   - `regularAmount = workerRate * regularHours`
   - `overtimeAmount = workerRate * overtimeMultiplier * overtimeHours`
   - `holidayAmount = workerRate * holidayMultiplier * holidayHours`
   - `missingAmount = missingRate * missingHours`
   - All amounts rounded via `DefaultMoneyRound()`

3. **Create TimeSheetTotalPayroll entity** via `calculatorService.CreateTimeSheetTotalPayrollEntity(...)`:
   - Links back to the original TimeSheet
   - Stores hours breakdown and accumulated hours

4. **Accumulate hours by rate** into dictionaries (for PayStubItem creation later):
   - `regularByRate[workerRate] += regularHours`
   - `overtimeByRate[overtimeRate] += overtimeHours`
   - `holidayByRate[holidayRate] += holidayHours`
   - `missingByRate[missingRate] += missingHours`
   - Only accumulates if the corresponding amount > 0
   - Hours are grouped by rate so workers with multiple rates get separate line items

5. **Create PayStubWageDetail** linked to the TimeSheetTotalPayroll:
   - One per day/timesheet — preserves per-day traceability
   - Used downstream for queries that navigate PayStub → WageDetail → TimeSheetTotal → TimeSheet → WorkerRequest

#### Step 3.3: Public Holidays

For each week, looks up statutory holidays and calculates holiday pay based on the worker's regular wages from `payStubRepository.GetWorkerRegularWages(...)`.

### Step 4: Create PayStubItems

Converts the accumulated dictionaries into `PayStubItem` entities. Each dictionary entry (rate → total hours) becomes one line item on the pay stub:

| Type | Rate Key | Example |
|------|----------|---------|
| Regular | `workerRate` | "Regular hours: 40h @ $20" |
| OtherRegular | `workerRate` | "Other Regular hours: 5h @ $20" |
| Overtime | `overtimeMultiplier * workerRate` | "Overtime hours: 8h @ $30" |
| HolidayPremiumPay | `holidayMultiplier * workerRate` | "Statutory worked holiday pay: 8h @ $40" |
| Missing | `missingRate` | "Missing hours: 2h @ $18" |
| MissingOvertime | `overtimeMultiplier * missingRate` | "Missing overtime hours: 1h @ $27" |

### Step 5: Process Extras

From the raw timesheets (not the loop), consolidates:
- **Bonuses** — grouped by description, added to `payStubItems`
- **Reimbursements** — grouped by description, kept in separate `reimbursementItems` list (excluded from gross, added to net)
- **Other deductions** — one `PayStubOtherDeduction` per timesheet entry

Zero-total items are filtered out.

### Step 6: Calculate Totals

```
grossPayment   = sum of all payStubItems totals
vacations      = grossPayment * vacationRate
publicHolidayPay = sum of public holiday amounts
totalEarnings  = grossPayment + vacations + publicHolidayPay
```

### Step 7: Calculate Deductions

Determines the **payment period** based on the number of weeks spanned:
- 1 week → Weekly tables
- 2 weeks → Bi-weekly tables
- 3 weeks → Semi-monthly tables
- 4+ weeks → Monthly tables

Looks up from deduction tables:
- **CPP** (Canada Pension Plan) — can be overridden by worker's tax category
- **EI** (Employment Insurance) — `totalEarnings * eiRate`, can be overridden
- **Federal Tax** — looked up by earnings + tax category (Cc1 default)
- **Provincial Tax** — looked up by earnings + tax category (Cc1 default)

```
totalDeductions = CPP + EI + federalTax + provincialTax + otherDeductions
```

### Step 8: Calculate Net Pay

```
totalPaid = totalEarnings - totalDeductions + reimbursements
```

Reimbursements are added back because they are not taxable income.

### Step 9: Build PayStub Entity

Creates the `PayStub` entity and attaches:
- `PayStubItem`s (line items + reimbursements)
- `PayStubWageDetail`s (per-day traceability to timesheets)
- `PayStubPublicHoliday`s
- `PayStubOtherDeduction`s

Payment date is calculated differently for external vs. internal workers.

### Step 10: Save

Persists to database via `payStubRepository.Create(payStub)`.

## Key Entities and Their Purpose

| Entity | Granularity | Purpose |
|--------|-------------|---------|
| `PayStub` | One per worker per pay period | The pay stub itself with all totals |
| `PayStubItem` | One per type per unique rate | Line items shown on the pay stub (consolidated) |
| `PayStubWageDetail` | One per day/timesheet | Per-day breakdown for traceability back to timesheets |
| `TimeSheetTotalPayroll` | One per day/timesheet | Hours breakdown stored per timesheet |
| `PayStubPublicHoliday` | One per statutory holiday | Holiday pay calculated from regular wages |
| `PayStubOtherDeduction` | One per timesheet with deductions | Additional deductions from timesheets |

## Key Services and Repositories

| Dependency | Purpose |
|------------|---------|
| `ITimesheetCalculatorService` | Hours breakdown, amount calculations, TimeSheetTotal creation |
| `ITimeSheetRepository` | Fetch approved timesheets |
| `IPayStubRepository` | PayStub CRUD, next number, regular wages lookup |
| `IDeductionsRepository` | CPP, EI, Federal/Provincial tax table lookups |
| `ICatalogRepository` | Public holidays lookup |
| `IWorkerRepository` | Worker tax category |

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
│  └─ Create PayStubWageDetail ───────┼──► allWageDetails (per-day trace)
│                                     │
│  For each week:                     │
│  └─ Get public holidays ────────────┼──► publicHolidays
└─────────────────────────────────────┘
       │
       ▼
Dictionaries ──► PayStubItems (consolidated line items)
       │
       ▼
Process extras (bonus, reimbursements, deductions)
       │
       ▼
Calculate: gross → vacations → totalEarnings
       │
       ▼
Lookup deductions: CPP, EI, Federal Tax, Provincial Tax
       │
       ▼
Net pay = totalEarnings - totalDeductions + reimbursements
       │
       ▼
Build PayStub entity with all children → Save
```
