# Billing Rules - Covenant/Sigook Platform (Canada)

## 💰 Invoice Generation System

Covenant's invoicing system charges Companies for staffing services, applying rates, premiums, and taxes appropriately.

**Code location:**
- `Covenant.Api/Covenant.Core.BL/Services/Invoices/` — Invoicing logic (BaseInvoiceService, CanadaInvoiceService, UsaInvoiceService)
- `Covenant.Api/Covenant.Core.BL/Services/AccountingService.cs` — Orchestration
- `Covenant.Api/Covenant.Common/Entities/Accounting/Invoice/` — Invoice entities
- `Covenant.Api/Covenant.Documents/` — PDF generation

---

## 💵 Rate Structure

### Agency Rate vs Worker Rate

**Core concept:**
```
AgencyRate = What the Agency charges the Company
WorkerRate = What the Agency pays the Worker
Markup    = AgencyRate - WorkerRate (Agency's profit)
```

**Example:**
```
AgencyRate: $25.00/hr
WorkerRate: $18.50/hr
Markup:     $6.50/hr (26% profit margin)
```

**Defined in:**
- `CompanyProfileJobPositionRate` (table)
- Per job position, per company

---

### Base Rates

**Regular Hour Rate:**
```
RegularAmount = RegularHours × AgencyRate
```

**Example:**
```
40 hours × $25.00/hr = $1,000.00
```

---

### Premium Rates

#### 1. Overtime Rate

**Rule:**
- Typically 1.5x (time and a half)
- Defined in `CompanyProfileJobPositionRate.OvertimeRate`

**Calculation:**
```
OvertimeAmount = OvertimeHours × (AgencyRate × OvertimeRate)
```

**Example:**
```
OvertimeRate = 1.5
4 hours × ($25.00 × 1.5) = 4 × $37.50 = $150.00
```

---

#### 2. Night Shift Premium

**Rule:**
- Additional premium for night hours (11 PM - 7 AM)
- Typically 1.0 - 1.2 (0% - 20% extra)
- Defined in `CompanyProfileJobPositionRate.NightShiftRate`

**Calculation:**
```
NightShiftAmount = NightShiftHours × (AgencyRate × NightShiftRate)
```

**Example:**
```
NightShiftRate = 1.15 (15% premium)
8 hours × ($25.00 × 1.15) = 8 × $28.75 = $230.00
```

**Note:**
- Some companies charge a flat per-hour premium instead of a multiplier
- Some don't charge a night shift premium at all (NightShiftRate = 1.0)

---

#### 3. Holiday Rate

**Rule:**
- Statutory holidays
- Typically 1.5x - 2.0x
- Defined in `CompanyProfileJobPositionRate.HolidayRate`

**Calculation:**
```
HolidayAmount = HolidayHours × (AgencyRate × HolidayRate)
```

**Example:**
```
HolidayRate = 1.5
8 hours × ($25.00 × 1.5) = 8 × $37.50 = $300.00
```

---

#### 4. Missing Hours

**Rule:**
- Hours not punched but approved by the agency (e.g. forgotten clock in/out, manual adjustments)
- Stored per-timesheet in `TimeSheet.MissingHours` and `TimeSheet.MissingHoursOvertime`
- Billed at `TimeSheet.MissingRateAgency`; falls back to `AgencyRate` when missing rate is `<= 0`
- Missing overtime applies the same `OvertimeRate` multiplier on top of the missing rate

**Calculation:**
```
MissingRate         = TimeSheet.MissingRateAgency > 0 ? TimeSheet.MissingRateAgency : AgencyRate
MissingAmount       = MissingHours × MissingRate
MissingOvertimeAmt  = MissingOvertimeHours × (MissingRate × OvertimeRate)
```

**Example:**
```
MissingRateAgency = $22.00, OvertimeRate = 1.5
2h missing                 → 2 × $22.00 = $44.00
1h missing overtime        → 1 × ($22.00 × 1.5) = $33.00
```

---

### Vacation Rate

**Federal Rule (Canada):**
- **Mandatory 4%** of total wages
- Added to invoice
- Defined in `Invoice.VacationsRate`

**Calculation:**
```
Vacations = SubTotal × VacationsRate
```

**Example:**
```
SubTotal:  $4,500.00
Vacations: $4,500.00 × 0.04 = $180.00
```

---

## 🧾 Invoice Calculation

### Per-Worker Breakdown

**InvoiceTotal (one per worker):**

```
1. Regular hours charge:
   RegularAmount = RegularHours × AgencyRate

2. Overtime charge:
   OvertimeAmount = OvertimeHours × (AgencyRate × OvertimeRate)

3. Night shift charge:
   NightShiftAmount = NightShiftHours × (AgencyRate × NightShiftRate)

4. Holiday charge:
   HolidayAmount = HolidayHours × (AgencyRate × HolidayRate)

5. Missing charge:
   MissingRate         = TimeSheet.MissingRateAgency > 0 ? TimeSheet.MissingRateAgency : AgencyRate
   MissingAmount       = MissingHours × MissingRate
   MissingOvertimeAmt  = MissingOvertimeHours × (MissingRate × OvertimeRate)

6. Worker total:
   WorkerTotal = RegularAmount + OvertimeAmount + NightShiftAmount + HolidayAmount
                 + MissingAmount + MissingOvertimeAmt
```

**Example (Worker: John Doe):**
```
Regular:  40h × $25.00 = $1,000.00
Overtime:  4h × $37.50 = $  150.00
Night:     0h          = $    0.00
Holiday:   0h          = $    0.00
────────────────────────────────
Worker Total:           $1,150.00
```

---

### Invoice Totals

**Step 1: SubTotal**
```
SubTotal = sum of all WorkerTotal
```

**Step 2: Vacations**
```
Vacations = SubTotal × VacationsRate
```

**Step 3: Bonus (if applicable)**
```
BonusAmount = SubTotal × BonusRate
```

**Step 4: Discounts (if applicable)**
```
DiscountAmount = sum of all discounts
```

**Step 5: Additional Items (if applicable)**
```
AdditionalItemsAmount = sum of additional items
```

**Step 6: Taxable Amount**
```
TaxableAmount = SubTotal + Vacations + BonusAmount + AdditionalItemsAmount - DiscountAmount
```

**Step 7: HST/GST**
```
HST = TaxableAmount × HstRate
```

**Step 8: Total Net**
```
TotalNet = TaxableAmount + HST
```

---

### Complete Invoice Example

**Invoice AI-0001-26**
**Company:** ABC Logistics
**Week Ending:** February 7, 2026

**Worker Breakdown:**

| Worker      | Regular | Overtime | Night | Holiday | Total      |
|-------------|---------|----------|-------|---------|------------|
| John Doe    | 1,000.00| 150.00   | 0.00  | 0.00    | 1,150.00   |
| Jane Smith  | 1,000.00| 0.00     | 0.00  | 0.00    | 1,000.00   |
| Bob Johnson | 1,000.00| 0.00     | 230.00| 0.00    | 1,230.00   |
| Alice Brown | 1,000.00| 120.00   | 0.00  | 0.00    | 1,120.00   |

**Totals:**

```
SubTotal:                           $4,500.00
Vacations (4%):                     $  180.00
Bonus:                              $    0.00
Additional Items:                   $    0.00
Discounts:                          $    0.00
─────────────────────────────────────────────
Taxable Amount:                     $4,680.00
HST (13% - Ontario):                $  608.40
─────────────────────────────────────────────
TOTAL NET:                          $5,288.40
═════════════════════════════════════════════
```

---

## 💸 Tax Calculations (HST/GST)

### Provincial Tax Rates (Canada)

| Province              | Tax            | Rate   | Notes                        |
|-----------------------|----------------|--------|------------------------------|
| Ontario (ON)          | HST            | 13%    | Harmonized Sales Tax         |
| British Columbia (BC) | GST + PST      | 12%    | 5% GST + 7% PST              |
| Alberta (AB)          | GST            | 5%     | No provincial tax            |
| Quebec (QC)           | GST + QST      | 14.975%| 5% GST + 9.975% QST          |
| Nova Scotia (NS)      | HST            | 15%    |                              |
| New Brunswick (NB)    | HST            | 15%    |                              |
| PEI                   | HST            | 15%    |                              |
| Newfoundland (NL)     | HST            | 15%    |                              |
| Manitoba (MB)         | GST + PST      | 12%    | 5% GST + 7% PST              |
| Saskatchewan (SK)     | GST + PST      | 11%    | 5% GST + 6% PST              |

**Determined by:**
- Company billing address province
- Stored in `Invoice.HstRate`

---

### Tax Exemptions

**Exempt Items:**
- Basic groceries
- Prescription drugs
- Most health services
- Educational services

**For Staffing Services:**
- **Not exempt** — Fully taxable at HST/GST rate

---

## 💼 Additional Charges

### Bonus Charges

**Purpose:** Special bonuses or incentives

**Calculation:**
```
BonusAmount = SubTotal × BonusRate
```

**Example:**
```
SubTotal:    $4,500.00
BonusRate:   0.05 (5%)
BonusAmount: $4,500.00 × 0.05 = $225.00
```

---

### Additional Items

**Purpose:** Custom line items (equipment rental, transportation, etc.)

**Structure:**
```csharp
public class InvoiceAdditionalItem
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
}
```

**Example:**
```
Description: "Equipment Rental - Forklift"
Amount: $500.00
```

**Added to the Taxable Amount.**

---

### Discounts

**Purpose:** Volume discounts, promotions, adjustments

**Structure:**
```csharp
public class InvoiceDiscount
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
}
```

**Example:**
```
Description: "Volume Discount (>100 hours)"
Amount: -$200.00
```

**Subtracted from the Taxable Amount.**

---

## 📅 Billing Frequency

### Weekly (Most Common)
- Invoice generated every week
- Week ending: typically Saturday or Sunday
- Due date: typically Net 30 days

### Bi-Weekly
- Every 2 weeks

### Monthly
- End of month
- Covers all work in the month

**Defined by:**
- Agreement between Agency and Company
- Configurable per company

---

## 📄 Invoice Format

### Invoice Number Format

```
AI-{number:0000}-{year:00}

Examples:
AI-0001-26  (First invoice of 2026)
AI-0123-26  (123rd invoice of 2026)
```

**Sequential numbering:**
- Per year
- Cannot skip numbers (audit requirement)
- `Invoice.InvoiceNumber` (int)

---

### Invoice Sections

**1. Header:**
- Agency info (name, address, business number, HST number)
- Company info (business name, billing address)
- Invoice number and date
- Week ending date
- Due date

**2. Worker Hours Breakdown:**
- Table with each worker
- Regular, Overtime, Night Shift, Holiday hours and amounts
- Subtotal per worker

**3. Totals:**
- SubTotal (sum of all workers)
- Vacations (4%)
- Bonus (if applicable)
- Additional items (if any)
- Discounts (if any)
- Taxable amount
- HST/GST
- **TOTAL NET (amount due)**

**4. Payment Terms:**
- Payment due date
- Accepted payment methods
- Late payment policy

**5. Notes:**
- Any special notes or comments

---

## 🔍 Special Cases

### Multi-Province Invoicing

**Scenario:** Company has locations in multiple provinces

**Rule:**
- Apply tax rate based on **work location** (where the service was performed)
- Not the billing address

**Example:**
```
Company HQ: Toronto, ON
Work Location 1: Toronto, ON → 13% HST
Work Location 2: Calgary, AB → 5% GST

Generate separate invoices or split tax calculations.
```

---

### Zero-Rated Services

**Scenario:** Export of services (international)

**Rule:**
- 0% GST/HST for services exported outside Canada
- Requires documentation

**Not applicable for domestic staffing.**

---

### Subcontractor Billing

**Scenario:** Worker is a subcontractor (not an employee)

**Differences:**
- No vacation pay (4%)
- Different tax treatment
- May invoice the agency directly

**Invoice structure:**
- SubTotal only (no vacations)
- Apply GST/HST
- Issue to subcontractor

---

### Contractors

**Scenario:** Independent contractors

**Similar to subcontractors:**
- No vacation pay
- Simplified invoice structure

---

## 💳 Payment Processing

### Payment Methods

**Accepted:**
- Bank transfer (EFT)
- Credit card
- Cheque

**Payment Terms:**
- Net 30 days (standard)
- Net 15 days (some clients)
- Net 45 days (negotiated)

---

### Late Payment

**Policy:**
- Interest charged after due date
- Typically 1.5% - 2% per month
- Service suspension after 60 days overdue

---

### Payment Tracking

**ChargeStatus (Stripe/Payment Gateway):**
```csharp
public enum ChargeStatus
{
    Pending,
    Succeeded,
    Failed
}
```

**Invoice.ChargeStatus** tracks the payment status.

---

## 📊 Reporting

### Invoice Reports

**Available Reports:**

**1. Invoice Summary Report (Excel):**
- All invoices for a period
- Company, Date, Amount, Status

**2. Accounts Receivable Report:**
- Outstanding invoices
- Aging (30, 60, 90+ days)

**3. Revenue Report:**
- Total revenue by period
- Breakdown by company
- Breakdown by job position

---

## 🧪 Calculation Validation

### Test Cases

```csharp
[Fact]
public void Calculate_Invoice_Standard()
{
    // Arrange
    var workerRequests = new[]
    {
        new WorkerRequest
        {
            TimeSheets = CreateTimeSheets(
                regularHours: 40,
                overtimeHours: 4
            )
        }
    };
    var agencyRate = 25m;
    var overtimeRate = 1.5m;
    var vacationsRate = 0.04m;
    var hstRate = 0.13m;

    // Act
    var invoice = InvoiceService.Calculate(
        workerRequests,
        agencyRate,
        overtimeRate,
        vacationsRate,
        hstRate);

    // Assert
    var expectedSubTotal = (40 * 25) + (4 * 25 * 1.5);  // 1000 + 150 = 1150
    var expectedVacations = 1150 * 0.04;                 // 46
    var expectedTaxable = 1150 + 46;                     // 1196
    var expectedHst = 1196 * 0.13;                       // 155.48
    var expectedTotal = 1196 + 155.48;                   // 1351.48

    Assert.Equal(expectedSubTotal, invoice.SubTotal);
    Assert.Equal(expectedVacations, invoice.Vacations, 2);
    Assert.Equal(expectedHst, invoice.Hst, 2);
    Assert.Equal(expectedTotal, invoice.TotalNet, 2);
}
```

---

## ⚠️ Compliance & Audit

### Record Keeping

**Requirements:**
- Keep all invoices for 7 years (CRA requirement)
- Maintain audit trail of all calculations
- Store PDFs in Azure Storage

### Invoice Numbering

**Critical:**
- Must be sequential
- Cannot skip numbers
- Cannot reuse numbers
- Audit requirement for CRA

**Handling Errors:**
- If an invoice is deleted, mark it as void (don't reuse the number)
- Use the `SkipInvoiceNumber` flag if needed

---

## 🔐 Authorization

### Who Can Generate Invoices

**Agency Users:**
- Admin role
- Accounting role
- Can generate invoices for their agency's companies

**Company Users:**
- Cannot generate invoices
- Can only view received invoices

**Workers:**
- Cannot access invoices

---

## 📧 Invoice Delivery

### Email Recipients

**Defined in:**
- `CompanyProfileInvoiceRecipient` (table)
- Multiple recipients per company

**Email Content:**
```
Subject: Invoice AI-0001-26 from [Agency Name]
Body: HTML template with invoice summary
Attachments: invoice.pdf
```

**Sent via:**
- SendGrid integration
- `Covenant.Notifications/EmailNotificationService`

---

## 🎯 Business Rules

### Critical Validations

**1. Cannot create an invoice if:**
- No approved timesheets for the period
- Company status is Blocked
- No job position rate defined

**2. An invoice must have:**
- At least one worker with hours
- Valid HST rate (based on province)
- Sequential invoice number

**3. Cannot edit an invoice after:**
- Payment received
- 30 days elapsed (configurable)

**4. Cannot delete an invoice:**
- Use the void flag instead
- Maintain audit trail

---

## 💡 Best Practices

### For Agencies

1. **Review timesheets before invoicing**
   - Ensure all hours are approved
   - Check for anomalies

2. **Generate invoices promptly**
   - Weekly schedule
   - Consistent billing cycle

3. **Follow up on overdue invoices**
   - Automated reminders at 7, 14, 30 days

### For Developers

1. **Always use decimal for money**
   - Never use float or double
   - Round to 2 decimal places

2. **Validate tax rates**
   - Ensure correct province
   - Update rates annually

3. **Test edge cases**
   - Zero hours
   - Very large hours
   - Multiple workers
   - Discounts exceeding subtotal (should fail)

4. **Maintain audit trail**
   - Log all invoice generations
   - Track modifications
   - Record voiding
