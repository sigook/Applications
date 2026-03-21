# Payroll Rules - Covenant/Sigook Platform (Canadá)

## 🇨🇦 Canadian Payroll System

El sistema de nómina de Covenant está diseñado para cumplir con las regulaciones fiscales canadienses, incluyendo CPP, EI, e impuestos federales y provinciales.

**Ubicación del código:**
- `Covenant.Api/Covenant.PayStubs/` - Generación de pay stubs
- `Covenant.Api/Covenant.Deductions/` - Cálculos de deducciones
- `Covenant.Api/Covenant.Core.BL/Services/PayStubService.cs` - Orquestación

---

## 💰 Earnings (Ingresos)

### 1. Regular Wage

**Cálculo:**
```
RegularWage = RegularHours × WorkerRate
```

**Ejemplo:**
```
40 hours × $18.50/hr = $740.00
```

---

### 2. Overtime Wage

**Regla Federal (Canadá):**
- Overtime starts after **44 hours per week**
- Week = Sunday to Saturday
- Rate = WorkerRate × 1.5 (time and a half)

**Cálculo:**
```
OvertimeWage = OvertimeHours × (WorkerRate × OvertimeRate)
```

**Ejemplo:**
```
48 hours worked in a week:
- RegularHours: 44 hours × $18.50 = $814.00
- OvertimeHours: 4 hours × ($18.50 × 1.5) = 4 × $27.75 = $111.00
Total Wage: $925.00
```

**Nota Provincial:**
- Algunas provincias tienen diferentes thresholds:
  - **BC:** Overtime after 8 hrs/día o 40 hrs/semana
  - **QC:** Overtime after 40 hrs/semana
  - **ON:** Overtime after 44 hrs/semana (federal standard)

---

### 3. Night Shift Premium

**Regla:**
- Hours between **11:00 PM - 7:00 AM**
- Typically 10-15% premium

**Cálculo:**
```
NightShiftWage = NightShiftHours × (WorkerRate × NightShiftRate)
```

**Ejemplo:**
```
NightShiftRate = 1.15 (15% premium)
8 hours × ($18.50 × 1.15) = 8 × $21.28 = $170.24
```

---

### 4. Holiday Pay

**Regla:**
- Statutory holidays in Canada
- Typically 1.5x - 2x rate

**Statutory Holidays (Canada):**
- New Year's Day
- Good Friday
- Canada Day (July 1)
- Labour Day
- Thanksgiving
- Christmas Day
- Boxing Day
- Provincial holidays (varies)

**Cálculo:**
```
HolidayWage = HolidayHours × (WorkerRate × HolidayRate)
```

**Ejemplo:**
```
HolidayRate = 1.5
8 hours × ($18.50 × 1.5) = 8 × $27.75 = $222.00
```

---

### 5. Vacation Pay

**Regla Federal (Canadá):**
- **Minimum 4%** of gross wages
- After 5 years: 6% (varies by province)
- Accrued but not necessarily paid each period

**Cálculo:**
```
Vacations = GrossPayment × 0.04
```

**Ejemplo:**
```
GrossPayment: $1,000
Vacations: $1,000 × 0.04 = $40.00
```

**Importante:**
- Vacations se suman a TotalEarnings
- Se usan para calcular deducciones (base imponible)

---

### 6. Public Holiday Pay

**Regla:**
- If worker is entitled to the holiday but works
- Additional pay beyond the holiday premium

**Ejemplo:**
- Worker works Christmas (holiday)
- Gets regular pay + holiday premium (1.5x) + public holiday pay

---

### 7. Gross Payment & Total Earnings

**Cálculo:**
```
GrossPayment = RegularWage + OvertimeWage + NightShiftWage + HolidayWage

TotalEarnings = GrossPayment + Vacations + PublicHolidayPay
```

**Ejemplo:**
```
RegularWage:      $740.00
OvertimeWage:     $111.00
NightShiftWage:   $  0.00
HolidayWage:      $  0.00
─────────────────────────
GrossPayment:     $851.00
Vacations (4%):   $ 34.04
PublicHolidayPay: $  0.00
─────────────────────────
TotalEarnings:    $885.04
```

---

## 📉 Deductions (Deducciones)

### 1. CPP - Canada Pension Plan

**2025 Rates:**
- **Rate:** 5.95% on pensionable earnings
- **Basic Exemption:** $3,500 per year
- **Maximum Pensionable Earnings:** $68,500 per year
- **Maximum Contribution:** $3,867.50 per year

**Cálculo:**

**Annual:**
```
CPP = (AnnualEarnings - BasicExemption) × 0.0595
CPP = min(CPP, MaxContribution)
```

**Per Pay Period:**

**Weekly:**
```
WeeklyExemption = $3,500 / 52 = $67.31
WeeklyCPP = max(0, (WeeklyEarnings - $67.31) × 0.0595)
```

**Bi-Weekly:**
```
BiWeeklyExemption = $3,500 / 26 = $134.62
BiWeeklyCPP = max(0, (BiWeeklyEarnings - $134.62) × 0.0595)
```

**Monthly:**
```
MonthlyExemption = $3,500 / 12 = $291.67
MonthlyCPP = max(0, (MonthlyEarnings - $291.67) × 0.0595)
```

**Ejemplo (Weekly):**
```
GrossPayment: $1,000
CPP = ($1,000 - $67.31) × 0.0595
CPP = $932.69 × 0.0595
CPP = $55.50
```

**Excepciones:**
- No CPP for workers under 18
- No CPP for workers over 70
- No CPP for self-employed (handled separately)

**Implementación:**
```csharp
// Covenant.Deductions/CppCalculator.cs
public decimal Calculate(decimal earnings, PayFrequency frequency)
{
    var exemption = GetExemption(frequency);
    var pensionableEarnings = Math.Max(0, earnings - exemption);
    var cpp = pensionableEarnings * 0.0595m;
    return Math.Round(cpp, 2);
}
```

---

### 2. EI - Employment Insurance

**2025 Rates:**
- **Rate:** 1.66% on insurable earnings
- **Maximum Insurable Earnings:** $63,200 per year
- **Maximum Contribution:** $1,049.12 per year

**Cálculo:**

**Per Pay Period:**
```
EI = Earnings × 0.0166
```

**Weekly:**
```
MaxWeeklyInsurable = $63,200 / 52 = $1,215.38
WeeklyEI = min(Earnings, $1,215.38) × 0.0166
```

**Ejemplo (Weekly):**
```
GrossPayment: $1,000
EI = $1,000 × 0.0166 = $16.60
```

**Excepciones:**
- No EI for self-employed (contractors)
- No EI for certain types of employment (family business)

**Implementación:**
```csharp
// Covenant.Deductions/EiCalculator.cs
public decimal Calculate(decimal earnings, PayFrequency frequency)
{
    var maxInsurable = GetMaxInsurable(frequency);
    var insurableEarnings = Math.Min(earnings, maxInsurable);
    var ei = insurableEarnings * 0.0166m;
    return Math.Round(ei, 2);
}
```

---

### 3. Federal Tax

**2025 Federal Tax Brackets (Progressive):**

| Taxable Income              | Tax Rate |
|-----------------------------|----------|
| Up to $55,867               | 15%      |
| $55,867 to $111,733         | 20.5%    |
| $111,733 to $173,205        | 26%      |
| $173,205 to $246,752        | 29%      |
| Over $246,752               | 33%      |

**Basic Personal Amount (BPA):** ~$15,705 (2025)

**Claim Codes:**
- **Code 1:** Basic personal amount (~$15,705)
- **Code 2-10:** Additional amounts (spouse, dependents, etc)

**Cálculo:**

El sistema usa **lookup tables** basadas en:
- Annual income estimated
- Claim code
- Pay frequency

**Lookup Tables:**
- `FederalTaxWeekly`
- `FederalTaxBiWeekly`
- `FederalTaxSemiMonthly`
- `FederalTaxMonthly`

**Ejemplo (Weekly, Code 1):**
```
GrossPayment: $1,000/week
Annual estimated: $1,000 × 52 = $52,000

Lookup FederalTaxWeekly table:
- Income range: $50,000 - $55,000
- Claim Code: 1
- Tax: ~$120.50/week
```

**Implementación:**
```csharp
// Covenant.Deductions/FederalTaxCalculator.cs
public decimal Calculate(
    decimal earnings,
    PayFrequency frequency,
    int claimCode)
{
    var annualEstimate = EstimateAnnual(earnings, frequency);
    var tax = LookupTable.FederalTax(annualEstimate, claimCode, frequency);
    return Math.Round(tax, 2);
}
```

---

### 4. Provincial Tax

**Provincial Tax Rates (2025 - Examples):**

**Ontario:**
| Taxable Income      | Tax Rate |
|---------------------|----------|
| Up to $51,446       | 5.05%    |
| $51,446 to $102,894 | 9.15%    |
| $102,894 to $150,000| 11.16%   |
| $150,000 to $220,000| 12.16%   |
| Over $220,000       | 13.16%   |

**British Columbia:**
| Taxable Income      | Tax Rate |
|---------------------|----------|
| Up to $47,937       | 5.06%    |
| $47,937 to $95,875  | 7.70%    |
| $95,875 to $110,076 | 10.50%   |
| $110,076 to $133,664| 12.29%   |
| Over $133,664       | 14.70%   |

**Quebec:**
| Taxable Income      | Tax Rate |
|---------------------|----------|
| Up to $51,780       | 14%      |
| $51,780 to $103,545 | 19%      |
| $103,545 to $126,000| 24%      |
| Over $126,000       | 25.75%   |

**Provincial Basic Personal Amounts:**
- ON: ~$11,865
- BC: ~$12,580
- QC: ~$17,183

**Cálculo:**

Similar to federal, uses **lookup tables per province**:
- `ProvincialTaxWeekly[ON]`
- `ProvincialTaxWeekly[BC]`
- `ProvincialTaxWeekly[QC]`
- etc.

**Ejemplo (Ontario, Weekly, Code 1):**
```
GrossPayment: $1,000/week
Province: Ontario
Annual estimated: $52,000

Lookup ProvincialTaxWeekly[ON] table:
- Income range: $50,000 - $55,000
- Claim Code: 1
- Tax: ~$45.20/week
```

**Implementación:**
```csharp
// Covenant.Deductions/ProvincialTaxCalculator.cs
public decimal Calculate(
    decimal earnings,
    PayFrequency frequency,
    string provinceCode,
    int claimCode)
{
    var annualEstimate = EstimateAnnual(earnings, frequency);
    var tax = LookupTable.ProvincialTax(
        annualEstimate,
        provinceCode,
        claimCode,
        frequency);
    return Math.Round(tax, 2);
}
```

---

### 5. Other Deductions

**Optional Deductions:**
- Union dues
- Pension contributions (beyond CPP)
- Health insurance premiums
- Wage garnishments (court orders)

**Cálculo:**
```
OtherDeductions = sum of all optional deductions
```

---

### 6. Total Deductions & Net Pay

**Cálculo:**
```
TotalDeductions = CPP + EI + FederalTax + ProvincialTax + OtherDeductions

TotalPaid (Net Pay) = TotalEarnings - TotalDeductions
```

**Ejemplo Completo (Ontario, Weekly):**
```
EARNINGS:
─────────────────────────
Regular Wage (40h):    $740.00
Overtime Wage (4h):    $111.00
─────────────────────────
Gross Payment:         $851.00
Vacations (4%):        $ 34.04
─────────────────────────
Total Earnings:        $885.04

DEDUCTIONS:
─────────────────────────
CPP (5.95%):           $ 55.50
EI (1.66%):            $ 14.73
Federal Tax:           $120.50
Provincial Tax (ON):   $ 45.20
Other Deductions:      $  0.00
─────────────────────────
Total Deductions:      $235.93

NET PAY:               $649.11
═════════════════════════
```

---

## 🧮 Pay Frequencies

El sistema soporta múltiples frecuencias de pago:

### Weekly (Semanal)
- 52 pay periods per year
- Most common in staffing

### Bi-Weekly (Quincenal)
- 26 pay periods per year
- Every 2 weeks

### Semi-Monthly (Semi-mensual)
- 24 pay periods per year
- Twice per month (1st and 15th)

### Monthly (Mensual)
- 12 pay periods per year

**Importante:**
- Tax tables son diferentes para cada frequency
- Exemptions (CPP, EI) se prorratan según frequency

---

## 📋 PayStub Format

**PayStub Number Format:**
```
PS-{number:0000}-{year:00}

Examples:
PS-0001-26  (First pay stub of 2026)
PS-0123-26  (123rd pay stub of 2026)
```

**PayStub Sections:**

**1. Header:**
- Agency info (name, address, business number)
- Worker info (name, SIN)
- Pay period (DateWorkBegins - DateWorkEnd)
- Payment date

**2. Earnings:**
- Regular hours and wage
- Overtime hours and wage
- Other earnings
- Gross payment
- Vacations
- Total earnings

**3. Deductions:**
- CPP, EI
- Federal tax, Provincial tax
- Other deductions
- Total deductions

**4. Net Pay:**
- Total paid (take-home)

**5. Year-to-Date (YTD):**
- YTD earnings
- YTD deductions
- YTD net pay

---

## 🔍 Special Cases

### Subcontractors

**Tax Treatment:**
- **No CPP deductions** (self-employed)
- **No EI deductions**
- **No income tax withheld**
- Issue **T4A** instead of T4

**PayStub:**
```
Gross Payment: $1,000
Deductions: $0
Net Pay: $1,000
```

---

### Contractors

**Tax Treatment:**
- Similar to subcontractors
- Independent business
- No deductions

---

### Workers with Multiple Jobs

**Consideration:**
- CPP and EI max contributions are annual
- System tracks YTD to avoid over-deduction
- If worker reaches max, stop deducting

---

### New Workers (Part-Year)

**Consideration:**
- Pro-rated annual amounts
- First pay period may have adjustments

---

## 🗓️ Tax Year Boundaries

**January 1st:**
- Reset CPP and EI YTD counters
- Update tax tables for new year
- New tax brackets and rates
- New basic personal amounts

**Year-End:**
- Generate T4 slips (before end of February)
- Submit to CRA (Canada Revenue Agency)

---

## 📊 Tax Tables Maintenance

**Location:**
- `Covenant.Deductions/Tables/`

**Tables to Update Annually:**
1. `CppWeekly.cs`, `CppBiWeekly.cs`, etc.
2. `FederalTaxWeekly.cs`, `FederalTaxBiWeekly.cs`, etc.
3. `ProvincialTaxWeekly[Province].cs` (per province)

**Update Process:**
1. CRA publishes new tables in December
2. Update code with new rates and brackets
3. Test calculations
4. Deploy before January 1st

---

## ⚠️ Compliance & Audit

**Record Keeping:**
- Keep all pay stubs for 7 years (CRA requirement)
- Store in Azure Storage with redundancy
- Maintain audit trail of all calculations

**Reporting:**
- T4 slips (annual) - employees
- T4A slips (annual) - subcontractors
- ROE (Record of Employment) when worker leaves
- Remittances to CRA (monthly or quarterly)

**Penalties for Non-Compliance:**
- Late remittances: 3-10% penalty + interest
- Incorrect T4s: $100-$2,500 per slip
- Failure to deduct: Employer liable for amounts

---

## 🧪 Testing Calculations

**Test Cases:**

```csharp
[Fact]
public void Calculate_Weekly_Standard()
{
    // Arrange
    var earnings = 1000m;
    var province = "ON";
    var claimCode = 1;

    // Act
    var cpp = CppCalculator.Calculate(earnings, PayFrequency.Weekly);
    var ei = EiCalculator.Calculate(earnings, PayFrequency.Weekly);
    var federalTax = FederalTaxCalculator.Calculate(earnings, PayFrequency.Weekly, claimCode);
    var provincialTax = ProvincialTaxCalculator.Calculate(earnings, PayFrequency.Weekly, province, claimCode);

    // Assert
    Assert.Equal(55.50m, cpp, 2);
    Assert.Equal(16.60m, ei, 2);
    Assert.InRange(federalTax, 115m, 125m);
    Assert.InRange(provincialTax, 40m, 50m);
}
```

**Validation:**
- Use CRA online calculator to verify
- Compare with payroll service providers (ADP, Ceridian)
- Test edge cases (max contributions, low income, etc)
