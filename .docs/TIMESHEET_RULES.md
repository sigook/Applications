# Timesheet Rules - Covenant/Sigook Platform

## ⏱️ Timesheet System Overview

El sistema de timesheets rastrea las horas trabajadas por cada worker, con funcionalidad de punch card (clock in/out) y aprobación por la agencia.

**Ubicación del código:**
- `Covenant.Api/Covenant.Core.BL/Services/TimeSheetService.cs`
- `Covenant.Api/Covenant.TimeSheetTotal/` - Cálculos de horas
- `Covenant.Common/Entities/Request/TimeSheet.cs`

---

## 📋 TimeSheet Entity Structure

### Core Fields

```csharp
public class TimeSheet
{
    public Guid Id { get; set; }
    public Guid WorkerRequestId { get; set; }
    public DateTime Date { get; set; }

    // Punch card times (actual worker clock in/out)
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public DateTime? ClockInRounded { get; set; }
    public DateTime? ClockOutRounded { get; set; }

    // Normalized times (for calculations)
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }

    // Approved times (by agency)
    public DateTime? TimeInApproved { get; set; }
    public DateTime? TimeOutApproved { get; set; }

    // Special flags
    public bool IsHoliday { get; set; }

    // Adjustments
    public decimal? DeductionsOthers { get; set; }
    public decimal? BonusOrOthers { get; set; }
    public decimal? Reimbursements { get; set; }
    public string Comment { get; set; }
}
```

---

## 🕐 Time Representation Explained

### 1. Clock Times (Raw Punch Card)

**ClockIn / ClockOut:**
- Actual timestamp when worker clocks in/out
- Includes full date and time with seconds
- Example: `2026-02-01T07:05:23Z`

**ClockInRounded / ClockOutRounded:**
- Rounded to nearest 15 minutes (configurable)
- Used for payroll fairness
- Example: `2026-02-01T07:00:00Z` (7:05 → 7:00)

**Rounding Rules:**
```
0-7 minutes → Round down
8-22 minutes → Round to 15
23-37 minutes → Round to 30
38-52 minutes → Round to 45
53-60 minutes → Round up to next hour
```

---

### 2. Normalized Times (For Calculations)

**TimeIn / TimeOut:**
- Normalized representation for calculations
- **TimeIn:** Always midnight of the date (`Date T00:00:00Z`)
- **TimeOut:** Hours offset from TimeIn
- Makes duration calculations easier

**Example:**
```
Worker clocks: 7:05 AM to 3:08 PM on Feb 1
ClockIn: 2026-02-01T07:05:23Z
ClockOut: 2026-02-01T15:08:12Z

After rounding:
ClockInRounded: 2026-02-01T07:00:00Z
ClockOutRounded: 2026-02-01T15:00:00Z

Normalized:
TimeIn: 2026-02-01T00:00:00Z (midnight)
TimeOut: 2026-02-01T08:00:00Z (8 hours from midnight)

Duration = TimeOut - TimeIn = 8 hours
```

---

### 3. Approved Times (Final for Payroll)

**TimeInApproved / TimeOutApproved:**
- Agency reviews and approves
- May differ from clock times (adjustments)
- **These are used for payroll and invoicing**

**Example:**
```
Worker clocked: 7:05 AM - 3:08 PM (8h 3min)
Rounded: 7:00 AM - 3:00 PM (8h)
Agency approves: 7:00 AM - 2:30 PM (7.5h)
  Reason: Worker left early, only worked 7.5h

TimeInApproved: 2026-02-01T07:00:00Z
TimeOutApproved: 2026-02-01T14:30:00Z
```

---

## 🎯 Timesheet Creation Methods

### Method 1: Worker Punch Card (Mobile App)

#### Clock In
```http
POST /api/WorkerRequestTimeSheet/ClockIn
{
  "workerRequestId": "guid",
  "clockIn": "2026-02-01T07:05:23Z",
  "isHoliday": false
}
```

**Backend Process:**
1. Validate worker is assigned to request
2. Validate no existing timesheet for today
3. Create TimeSheet:
   ```
   Date = 2026-02-01
   ClockIn = 2026-02-01T07:05:23Z
   ClockInRounded = 2026-02-01T07:00:00Z (rounded)
   TimeIn = 2026-02-01T00:00:00Z (midnight)
   TimeOut = null (pending clock out)
   ```

---

#### Clock Out
```http
POST /api/WorkerRequestTimeSheet/ClockOut
{
  "workerRequestId": "guid",
  "clockOut": "2026-02-01T15:08:12Z"
}
```

**Backend Process:**
1. Find TimeSheet for today
2. Validate has ClockIn
3. Validate minimum 3 minutes since ClockIn
4. Update TimeSheet:
   ```
   ClockOut = 2026-02-01T15:08:12Z
   ClockOutRounded = 2026-02-01T15:00:00Z
   TimeOut = 2026-02-01T08:00:00Z (8h from TimeIn)
   ```

**Validations:**
- Cannot clock in twice same day
- Cannot clock out without clock in
- Cannot clock out within 3 minutes of clock in
- Maximum 23:59 hours per day

---

### Method 2: Agency Manual Entry

```http
POST /api/AgencyRequestTimeSheet
{
  "workerRequestId": "guid",
  "date": "2026-02-01",
  "hours": 8.5,
  "isHoliday": false,
  "comment": "Manual entry - forgot to clock"
}
```

**Backend Process:**
1. Create TimeSheet pre-approved:
   ```
   Date = 2026-02-01
   TimeIn = 2026-02-01T00:00:00Z
   TimeOut = 2026-02-01T08:30:00Z (8.5h)
   TimeInApproved = 2026-02-01T00:00:00Z
   TimeOutApproved = 2026-02-01T08:30:00Z
   Comment = "Manual entry - forgot to clock"
   ```

**Use Cases:**
- Worker forgot to clock in/out
- Corrections
- Historical entry

---

## ✅ Approval Process

### Agency Approves Timesheet

```http
PUT /api/AgencyRequestTimeSheet/{id}/Approve
{
  "timeInApproved": "2026-02-01T07:00:00Z",
  "timeOutApproved": "2026-02-01T15:00:00Z"
}
```

**Validations:**
- TimeInApproved and TimeOutApproved must be same date
- TimeInApproved <= TimeOutApproved
- TimeInApproved.Date must equal TimeSheet.Date
- Hours < 24

**Effect:**
- Sets TimeInApproved and TimeOutApproved
- Triggers TimeSheetTotal calculation
- Makes timesheet ready for payroll/billing

---

## 🧮 TimeSheetTotal Calculation

### Overview

**TimeSheetTotal** breaks down hours into categories for payroll and billing:
- Regular hours
- Overtime hours
- Night shift hours
- Holiday hours

**Ubicación:** `Covenant.Api/Covenant.TimeSheetTotal/TimeSheetTotalCalculator.cs`

---

### Calculation Rules

#### 1. Total Hours

```
TotalHours = TimeOutApproved - TimeInApproved
```

**Example:**
```
TimeInApproved: 2026-02-01T07:00:00Z
TimeOutApproved: 2026-02-01T15:00:00Z
TotalHours: 08:00:00
```

---

#### 2. Weekly Accumulation

**Purpose:** Track total hours worked in the week (Sunday-Saturday) to determine when overtime starts.

```
AccumulateWeekHours = Sum of TotalHours for all timesheets in same week
```

**Week Definition:**
- Start: Sunday 00:00:00
- End: Saturday 23:59:59

**Example:**
```
Week of Feb 1-7, 2026:
Monday (Feb 2): 8h → AccumulateWeekHours = 8h
Tuesday (Feb 3): 8h → AccumulateWeekHours = 16h
Wednesday (Feb 4): 8h → AccumulateWeekHours = 24h
Thursday (Feb 5): 8h → AccumulateWeekHours = 32h
Friday (Feb 6): 8h → AccumulateWeekHours = 40h
Saturday (Feb 7): 8h → AccumulateWeekHours = 48h
```

---

#### 3. Regular Hours vs Overtime

**Canada Federal Rule:**
- Overtime starts after **44 hours per week**
- Some provinces differ (see variations below)

**Cálculo:**
```csharp
if (AccumulateWeekHours <= 44)
{
    RegularHours = TotalHours
    OvertimeHours = 0
}
else if (AccumulateWeekHours - TotalHours < 44)
{
    // This day crosses the 44h threshold
    RegularHours = 44 - (AccumulateWeekHours - TotalHours)
    OvertimeHours = TotalHours - RegularHours
}
else
{
    // Already over 44h, all hours are overtime
    RegularHours = 0
    OvertimeHours = TotalHours
}
```

**Example 1: Within 44h**
```
Day: Friday
TotalHours: 8h
AccumulateWeekHours (before this day): 32h
AccumulateWeekHours (including this day): 40h

Since 40h <= 44h:
RegularHours: 8h
OvertimeHours: 0h
```

**Example 2: Crosses 44h threshold**
```
Day: Saturday
TotalHours: 8h
AccumulateWeekHours (before this day): 40h
AccumulateWeekHours (including this day): 48h

Crosses threshold at 44h:
RegularHours: 4h (44 - 40 = 4)
OvertimeHours: 4h (8 - 4 = 4)
```

**Example 3: All overtime**
```
Day: Sunday (next week or long shift)
TotalHours: 8h
AccumulateWeekHours (before this day): 48h
AccumulateWeekHours (including this day): 56h

Already over 44h:
RegularHours: 0h
OvertimeHours: 8h
```

---

#### 4. Night Shift Hours

**Rule:**
- Hours between **11:00 PM (23:00) and 7:00 AM (07:00)**
- Can overlap with regular or overtime hours

**Cálculo:**
```csharp
NightShiftHours = 0

for each hour in (TimeInApproved to TimeOutApproved)
{
    if (hour >= 23:00 OR hour < 07:00)
    {
        NightShiftHours += 1 hour
    }
}
```

**Example 1: Day shift (no night hours)**
```
TimeInApproved: 07:00
TimeOutApproved: 15:00
NightShiftHours: 0h
```

**Example 2: Night shift**
```
TimeInApproved: 23:00 (11 PM)
TimeOutApproved: 07:00 (next day)
NightShiftHours: 8h (23:00-07:00)
```

**Example 3: Evening shift (partial night)**
```
TimeInApproved: 19:00 (7 PM)
TimeOutApproved: 03:00 (next day)
NightShiftHours: 4h (23:00-03:00)
```

**Note:** Night shift hours are billed/paid separately with premium rate.

---

#### 5. Holiday Hours

**Rule:**
- If `IsHoliday = true`, **all hours** are holiday hours
- Statutory holidays in Canada (varies by province)

**Cálculo:**
```csharp
if (IsHoliday)
{
    HolidayHours = TotalHours
    RegularHours = 0
    OvertimeHours = 0
}
else
{
    HolidayHours = 0
}
```

**Example:**
```
Date: December 25 (Christmas)
IsHoliday: true
TotalHours: 8h

HolidayHours: 8h
RegularHours: 0h
OvertimeHours: 0h
```

**Statutory Holidays (Federal + Common Provincial):**
- New Year's Day (Jan 1)
- Good Friday
- Easter Monday (some provinces)
- Victoria Day (May, before May 25)
- Canada Day (July 1)
- Labour Day (first Monday of September)
- Thanksgiving (second Monday of October)
- Remembrance Day (Nov 11, some provinces)
- Christmas Day (Dec 25)
- Boxing Day (Dec 26)

---

#### 6. Complete TimeSheetTotal Example

**Scenario:**
```
Date: Saturday, Feb 7, 2026
Worker clocked: 7:00 AM - 3:00 PM (8 hours)
AccumulateWeekHours (before): 40h
IsHoliday: false
```

**Calculation:**
```
TotalHours: 8h

AccumulateWeekHours (before this day): 40h
AccumulateWeekHours (including): 48h

Regular vs Overtime:
  Since crosses 44h threshold:
  RegularHours: 4h (44 - 40)
  OvertimeHours: 4h (8 - 4)

Night Shift:
  TimeIn: 7:00 AM
  TimeOut: 3:00 PM
  NightShiftHours: 0h (no hours between 11 PM - 7 AM)

Holiday:
  IsHoliday: false
  HolidayHours: 0h

Result:
  TotalHours: 08:00:00
  RegularHours: 04:00:00
  OvertimeHours: 04:00:00
  NightShiftHours: 00:00:00
  HolidayHours: 00:00:00
  AccumulateWeekHours: 48:00:00
```

---

## 🔍 Validations & Business Rules

### TimeSheet Creation

**1. Date Restrictions:**
- Cannot create timesheet more than 1 year in the past
- Date must be >= WorkerRequest.StartWorking
- Cannot create future timesheets (configurable)

**2. Duplicate Prevention:**
- Only one timesheet per Worker + Date
- Enforced at database level (unique constraint)

**3. Hours Limits:**
- Minimum: 0.25 hours (15 minutes)
- Maximum: 23:59 hours (cannot exceed 24h)
- Realistic check: typically < 16 hours

---

### Clock In/Out Rules

**Clock In:**
- Cannot clock in if already clocked in today
- Cannot clock in for dates in the past (must be today)
- GPS location optional (for verification)

**Clock Out:**
- Must have ClockIn first
- Must wait at least 3 minutes after ClockIn
- Can clock out multiple times within 5 minutes (correction window)
- After 5 minutes, ClockOut is locked

**Break Handling:**
- Breaks not automatically deducted
- Agency can adjust approved times to account for breaks
- Or use `DeductionsOthers` field

---

### Approval Rules

**1. Time Validation:**
- TimeInApproved and TimeOutApproved must be same date
- TimeInApproved must be <= TimeOutApproved
- TimeInApproved.Date must equal TimeSheet.Date
- Total approved hours must be < 24

**2. Authorization:**
- Only Agency users can approve
- Cannot approve own timesheets (if agency user is also worker)

**3. Locking:**
- Once timesheet used in PayStub, cannot modify
- Once timesheet used in Invoice, cannot modify
- Must void PayStub/Invoice first

---

## 📊 Timesheet States

**Implicit State Machine:**

```
1. Created (ClockIn done, ClockOut pending)
   - Has ClockIn
   - ClockOut is null

2. Completed (ClockOut done, approval pending)
   - Has ClockIn and ClockOut
   - TimeInApproved is null

3. Approved (Ready for payroll/billing)
   - Has TimeInApproved and TimeOutApproved
   - TimeSheetTotal calculated

4. Used in Payroll
   - PayStub created referencing this timesheet
   - Cannot modify

5. Used in Invoice
   - Invoice created referencing this timesheet
   - Cannot modify
```

---

## 🌎 Provincial Variations

### Overtime Rules by Province

| Province | Overtime Threshold | Notes |
|----------|-------------------|-------|
| Federal  | 44 hrs/week       | Standard |
| ON       | 44 hrs/week       | Same as federal |
| BC       | 8 hrs/day OR 40 hrs/week | Daily threshold |
| QC       | 40 hrs/week       | Lower than federal |
| AB       | 44 hrs/week OR 8 hrs/day | Both thresholds |
| SK       | 40 hrs/week       | |
| MB       | 40 hrs/week OR 8 hrs/day | |
| NB       | 44 hrs/week       | |
| NS       | 48 hrs/week       | Higher threshold |
| PEI      | 48 hrs/week       | |
| NL       | 40 hrs/week       | |

**Implementation:**
- Currently uses federal standard (44 hrs/week)
- Provincial rules can be added via configuration
- Configurable per CompanyProfile or Request

---

## 🔄 Corrections & Adjustments

### Manual Corrections

**Agency can:**
1. Create timesheet manually (if worker forgot to clock)
2. Adjust approved times (different from clock times)
3. Add comments explaining changes

**Example:**
```
Worker clocked: 7:00 AM - 3:00 PM (8h)
But actually left at 2:00 PM

Agency approves: 7:00 AM - 2:00 PM (7h)
Comment: "Worker left early - verified with supervisor"
```

---

### Financial Adjustments

**DeductionsOthers:**
- Deduct amount from pay
- Example: Broken equipment, advances

**BonusOrOthers:**
- Add amount to pay
- Example: Performance bonus, tips

**Reimbursements:**
- Add amount to pay (non-taxable)
- Example: Mileage, meals, tools

**Applied in PayStub calculation:**
```
GrossPayment = (Hours × Rate) + BonusOrOthers + Reimbursements - DeductionsOthers
```

---

## 🧪 Testing Scenarios

### Test Case 1: Simple Day Shift

```
Date: Monday
ClockIn: 07:00
ClockOut: 15:00
AccumulateWeekHours (before): 0h
IsHoliday: false

Expected:
  TotalHours: 8h
  RegularHours: 8h
  OvertimeHours: 0h
  NightShiftHours: 0h
  HolidayHours: 0h
  AccumulateWeekHours: 8h
```

---

### Test Case 2: Night Shift

```
Date: Tuesday
ClockIn: 23:00 (11 PM Monday)
ClockOut: 07:00 (Tuesday morning)
AccumulateWeekHours (before): 8h
IsHoliday: false

Expected:
  TotalHours: 8h
  RegularHours: 8h
  OvertimeHours: 0h
  NightShiftHours: 8h
  HolidayHours: 0h
  AccumulateWeekHours: 16h
```

---

### Test Case 3: Overtime Day

```
Date: Saturday
ClockIn: 07:00
ClockOut: 15:00
AccumulateWeekHours (before): 40h
IsHoliday: false

Expected:
  TotalHours: 8h
  RegularHours: 4h (to reach 44h)
  OvertimeHours: 4h
  NightShiftHours: 0h
  HolidayHours: 0h
  AccumulateWeekHours: 48h
```

---

### Test Case 4: Holiday

```
Date: December 25 (Christmas)
ClockIn: 07:00
ClockOut: 15:00
AccumulateWeekHours (before): 32h
IsHoliday: true

Expected:
  TotalHours: 8h
  RegularHours: 0h
  OvertimeHours: 0h
  NightShiftHours: 0h
  HolidayHours: 8h
  AccumulateWeekHours: 40h
```

---

### Test Case 5: Long Night Shift with Overtime

```
Date: Saturday (after 44h week)
ClockIn: 19:00 (7 PM Friday)
ClockOut: 03:00 (Saturday 3 AM)
AccumulateWeekHours (before): 44h
IsHoliday: false

Expected:
  TotalHours: 8h
  RegularHours: 0h (already at 44h)
  OvertimeHours: 8h
  NightShiftHours: 4h (23:00-03:00)
  HolidayHours: 0h
  AccumulateWeekHours: 52h
```

---

## 💡 Best Practices

### For Workers

1. **Clock in/out accurately**
   - Use the app every day
   - Don't forget to clock out

2. **Report issues immediately**
   - If forgot to clock, notify agency same day
   - Easier to correct fresh memories

3. **Review timesheets**
   - Check approved hours match worked hours
   - Dispute if incorrect

---

### For Agencies

1. **Review timesheets daily**
   - Don't wait until end of week
   - Catch issues early

2. **Approve promptly**
   - Workers expect timely approval
   - Delays affect payroll

3. **Document adjustments**
   - Always add comment when adjusting times
   - Audit trail important

4. **Verify unusual hours**
   - Very long shifts (>12h)
   - Night shifts
   - Holidays

---

### For Developers

1. **Always use TimeSpan for durations**
   - Not decimal hours
   - Precision matters

2. **Test week boundaries**
   - Sunday-Saturday transitions
   - Overnight shifts

3. **Test overtime calculations**
   - At 44h threshold
   - Before and after

4. **Handle timezones carefully**
   - Store all times in UTC
   - Convert to local for display

5. **Lock timesheets after use**
   - Once in PayStub, immutable
   - Once in Invoice, immutable
