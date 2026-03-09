# Entidades y Relaciones - Covenant/Sigook Platform

## 📊 Diagrama de Relaciones Principal

```
                    ┌──────────────────┐
                    │     AGENCY       │
                    │ ──────────────── │
                    │ Id               │
                    │ FullName         │
                    │ BusinessNumber   │
                    │ HstNumber        │
                    │ AgencyType       │
                    └────┬─────────┬───┘
                         │         │
          ┌──────────────┘         └──────────────┐
          │ manages                        employs │
          ▼                                        ▼
┌──────────────────────┐               ┌──────────────────────┐
│  COMPANYPROFILE      │               │   WORKERPROFILE      │
│ ──────────────────── │               │ ──────────────────── │
│ Id                   │               │ Id                   │
│ AgencyId (FK)        │               │ AgencyId (FK)        │
│ BusinessName         │               │ FirstName, LastName  │
│ CompanyStatus        │               │ SocialInsurance      │
└────┬─────────────────┘               │ ApprovedToWork       │
     │                                 │ Dnu, IsContractor    │
     │ creates                         └──────────┬───────────┘
     │                                            │
     │                              ┌─────────────┘
     │                              │ has user
     │                              │
     ▼                              ▼
┌──────────────────────┐    ┌─────────────────┐
│      REQUEST         │    │      USER       │
│ ──────────────────── │    │ ─────────────── │
│ Id                   │    │ Id              │
│ CompanyProfileId (FK)│    │ Email           │
│ AgencyId (FK)        │    │ Enabled         │
│ JobTitle             │    └─────────────────┘
│ WorkersQuantity      │
│ RequestStatus        │
│ JobLocationId (FK)   │
└────┬─────────────────┘
     │
     │ has many
     │
     ▼
┌──────────────────────────────┐
│      WORKERREQUEST           │
│ ──────────────────────────── │
│ Id                           │
│ RequestId (FK)               │
│ WorkerProfileId (FK)         │
│ WorkerRequestStatus          │
│ StartWorking                 │
└────┬─────────────────────────┘
     │
     │ has many
     │
     ▼
┌──────────────────────────────┐
│       TIMESHEET              │
│ ──────────────────────────── │
│ Id                           │
│ WorkerRequestId (FK)         │
│ Date                         │
│ TimeIn, TimeOut              │
│ ClockIn, ClockOut            │
│ TimeInApproved, TimeOutApproved│
│ IsHoliday                    │
└────┬─────────────────────────┘
     │
     │ has one
     │
     ▼
┌──────────────────────────────┐
│     TIMESHEETTOTAL           │
│ ──────────────────────────── │
│ Id                           │
│ TimeSheetId (FK)             │
│ TotalHours                   │
│ RegularHours                 │
│ OvertimeHours                │
│ NightShiftHours              │
│ HolidayHours                 │
│ AccumulateWeekHours          │
└──────────────────────────────┘

┌──────────────────────────────┐
│        PAYSTUB               │
│ ──────────────────────────── │
│ Id                           │
│ WorkerProfileId (FK)         │
│ PayStubNumber                │
│ RegularWage                  │
│ GrossPayment                 │
│ Vacations                    │
│ CPP, EI                      │
│ FederalTax, ProvincialTax    │
│ TotalPaid                    │
└──────────────────────────────┘

┌──────────────────────────────┐
│        INVOICE               │
│ ──────────────────────────── │
│ Id                           │
│ CompanyProfileId (FK)        │
│ InvoiceNumber                │
│ SubTotal                     │
│ Hst                          │
│ TotalNet                     │
└──────────────────────────────┘
```

---

## 🏢 AGENCY Module

### Agency (Agencia de Personal)

**Tabla:** `Agency`
**Ubicación:** `Covenant.Common/Entities/Agency/Agency.cs`

```csharp
public class Agency
{
    public Guid Id { get; set; }
    public string FullName { get; set; }                 // Business name
    public string BusinessNumber { get; set; }           // Tax registration
    public string HstNumber { get; set; }                // HST/GST number
    public AgencyType AgencyType { get; set; }           // Master/Regular/BusinessPartner
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<AgencyLocation> Locations { get; set; }
    public ICollection<AgencyPersonnel> Personnel { get; set; }
    public ICollection<CompanyProfile> Companies { get; set; }
    public ICollection<WorkerProfile> Workers { get; set; }
    public ICollection<Request> Requests { get; set; }
}
```

**AgencyType Enum:**
```csharp
public enum AgencyType
{
    Master = 1,           // Main agency with sub-agencies
    Regular = 2,          // Standard independent agency
    BusinessPartner = 3   // Partner agency with limited access
}
```

---

### AgencyLocation

**Tabla:** `AgencyLocation`

```csharp
public class AgencyLocation
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public string Name { get; set; }
    public Guid LocationId { get; set; }              // FK to Location
    public bool IsBillingAddress { get; set; }
    public bool IsActive { get; set; }

    // Navigation
    public Agency Agency { get; set; }
    public Location Location { get; set; }
}
```

---

### AgencyPersonnel (Employees)

**Tabla:** `AgencyPersonnel`

```csharp
public class AgencyPersonnel
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public PersonnelType Type { get; set; }           // Recruiter, SalesRep, etc
    public bool IsActive { get; set; }

    // Navigation
    public Agency Agency { get; set; }
}
```

**PersonnelType Enum:**
```csharp
public enum PersonnelType
{
    Recruiter = 1,
    SalesRepresentative = 2,
    AccountManager = 3,
    Administrator = 4
}
```

---

## 🏢 COMPANY Module

### CompanyProfile

**Tabla:** `CompanyProfile`
**Ubicación:** `Covenant.Common/Entities/Company/CompanyProfile.cs`

```csharp
public class CompanyProfile
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public string BusinessName { get; set; }
    public string DbaName { get; set; }                    // "Doing Business As"
    public string BusinessNumber { get; set; }
    public string HstNumber { get; set; }
    public CompanyStatus CompanyStatus { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }
    public bool RequiresPermissionToSeeOrders { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Agency Agency { get; set; }
    public ICollection<CompanyProfileLocation> Locations { get; set; }
    public ICollection<CompanyProfileJobPositionRate> JobPositionRates { get; set; }
    public ICollection<CompanyProfileContactPerson> ContactPersons { get; set; }
    public ICollection<CompanyUser> CompanyUsers { get; set; }
    public ICollection<Request> Requests { get; set; }
    public ICollection<Invoice> Invoices { get; set; }
}
```

**CompanyStatus Enum:**
```csharp
public enum CompanyStatus
{
    Lead = 1,          // Initial contact
    Potential = 2,     // Qualified lead
    Prospect = 3,      // In discussions
    Quoted = 4,        // Proposal sent
    Client = 5,        // Active client
    Blocked = 6,       // Temporarily suspended
    Inactive = 7       // No longer active
}
```

---

### CompanyProfileLocation

**Tabla:** `CompanyProfileLocation`

```csharp
public class CompanyProfileLocation
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string Name { get; set; }
    public Guid LocationId { get; set; }
    public bool IsActive { get; set; }

    // Navigation
    public CompanyProfile CompanyProfile { get; set; }
    public Location Location { get; set; }
}
```

---

### CompanyProfileJobPositionRate

**Tabla:** `CompanyProfileJobPositionRate`

**CRÍTICO:** Define las tarifas que determinan pricing.

```csharp
public class CompanyProfileJobPositionRate
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string JobTitle { get; set; }

    // Rates
    public decimal WorkerRate { get; set; }            // Lo que se paga al worker
    public decimal AgencyRate { get; set; }            // Lo que se cobra a la company
    // Markup = AgencyRate - WorkerRate (profit)

    // Shift premiums (multipliers)
    public decimal? NightShiftRate { get; set; }       // e.g., 1.15 (15% extra)
    public decimal? HolidayRate { get; set; }          // e.g., 1.5 (50% extra)
    public decimal? OvertimeRate { get; set; }         // e.g., 1.5 (time and a half)

    public string Currency { get; set; }               // CAD, USD
    public bool IsActive { get; set; }

    // Navigation
    public CompanyProfile CompanyProfile { get; set; }
}
```

---

### CompanyProfileContactPerson

**Tabla:** `CompanyProfileContactPerson`

```csharp
public class CompanyProfileContactPerson
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Position { get; set; }
    public bool IsPrimaryContact { get; set; }

    // Navigation
    public CompanyProfile CompanyProfile { get; set; }
}
```

---

### CompanyUser

**Tabla:** `CompanyUser`

**Purpose:** Links User to CompanyProfile for access control.

```csharp
public class CompanyUser
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public Guid UserId { get; set; }
    public bool CanSeeOrders { get; set; }
    public bool IsActive { get; set; }

    // Navigation
    public CompanyProfile CompanyProfile { get; set; }
    public User User { get; set; }
}
```

---

## 👷 WORKER Module

### WorkerProfile

**Tabla:** `WorkerProfile`
**Ubicación:** `Covenant.Common/Entities/Worker/WorkerProfile.cs`

```csharp
public class WorkerProfile
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public Guid? UserId { get; set; }                  // Link to User (authentication)

    // Personal Info
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDay { get; set; }
    public Guid GenderId { get; set; }
    public string ProfileImage { get; set; }           // URL to Azure Storage

    // Identification
    public string SocialInsurance { get; set; }        // SIN (Canada) or SSN (USA)
    public Guid? SocialInsuranceFileId { get; set; }
    public DateTime? SocialInsuranceDueDate { get; set; }
    public string IdentificationNumber1 { get; set; }
    public Guid? IdentificationType1 { get; set; }
    public Guid? IdentificationType1FileId { get; set; }
    public string IdentificationNumber2 { get; set; }
    public Guid? IdentificationType2 { get; set; }
    public Guid? IdentificationType2FileId { get; set; }

    // Contact Info
    public string MobileNumber { get; set; }
    public string Phone { get; set; }
    public Guid? LocationId { get; set; }
    public bool HasVehicle { get; set; }

    // Status Flags
    public bool ApprovedToWork { get; set; }           // Critical: Can worker work?
    public bool Dnu { get; set; }                      // Do Not Use
    public bool IsSubcontractor { get; set; }
    public bool IsContractor { get; set; }

    // Tax Info
    public Guid? WorkerProfileTaxCategoryId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Agency Agency { get; set; }
    public User User { get; set; }
    public Location Location { get; set; }
    public WorkerProfileTaxCategory TaxCategory { get; set; }
    public ICollection<WorkerSkill> Skills { get; set; }
    public ICollection<WorkerLanguage> Languages { get; set; }
    public ICollection<WorkerLicense> Licenses { get; set; }
    public ICollection<WorkerCertificate> Certificates { get; set; }
    public ICollection<WorkerJobExperience> JobExperiences { get; set; }
    public ICollection<WorkerAvailability> Availabilities { get; set; }
    public ICollection<WorkerLocationPreference> LocationPreferences { get; set; }
    public ICollection<WorkerRequest> WorkerRequests { get; set; }
    public ICollection<PayStub> PayStubs { get; set; }
}
```

---

### WorkerProfileTaxCategory

**Tabla:** `WorkerProfileTaxCategory`

**Purpose:** Tax claim codes para cálculo de impuestos.

```csharp
public class WorkerProfileTaxCategory
{
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public int FederalCategory { get; set; }           // Federal claim code (1-10)
    public int ProvincialCategory { get; set; }        // Provincial claim code (varies)

    // Navigation
    public WorkerProfile WorkerProfile { get; set; }
}
```

---

### WorkerSkill

**Tabla:** `WorkerSkill`

```csharp
public class WorkerSkill
{
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public Guid SkillId { get; set; }                  // FK to Skill catalog
    public int ExperienceYears { get; set; }

    // Navigation
    public WorkerProfile WorkerProfile { get; set; }
    public Skill Skill { get; set; }
}
```

---

### WorkerLanguage

**Tabla:** `WorkerLanguage`

```csharp
public class WorkerLanguage
{
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public Guid LanguageId { get; set; }
    public LanguageProficiency Proficiency { get; set; } // Basic, Intermediate, Advanced, Native

    // Navigation
    public WorkerProfile WorkerProfile { get; set; }
    public Language Language { get; set; }
}
```

---

### WorkerLicense

**Tabla:** `WorkerLicense`

```csharp
public class WorkerLicense
{
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public string LicenseName { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid? FileId { get; set; }

    // Navigation
    public WorkerProfile WorkerProfile { get; set; }
    public File File { get; set; }
}
```

---

### WorkerCertificate

**Tabla:** `WorkerCertificate`

```csharp
public class WorkerCertificate
{
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public string CertificateName { get; set; }
    public string CertificateNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid? FileId { get; set; }

    // Navigation
    public WorkerProfile WorkerProfile { get; set; }
    public File File { get; set; }
}
```

---

### WorkerAvailability

**Tabla:** `WorkerAvailability`

```csharp
public class WorkerAvailability
{
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    // Navigation
    public WorkerProfile WorkerProfile { get; set; }
}
```

---

### WorkerLocationPreference

**Tabla:** `WorkerLocationPreference`

```csharp
public class WorkerLocationPreference
{
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public Guid CityId { get; set; }

    // Navigation
    public WorkerProfile WorkerProfile { get; set; }
    public City City { get; set; }
}
```

---

## 📋 REQUEST Module (Job Orders)

### Request

**Tabla:** `Request`
**Ubicación:** `Covenant.Common/Entities/Request/Request.cs`

```csharp
public class Request
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public Guid CompanyProfileId { get; set; }

    // Job Details
    public string JobTitle { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }

    // Workers
    public int WorkersQuantity { get; set; }           // How many workers needed
    public int WorkersQuantityWorking { get; set; }    // Currently assigned

    // Timing
    public DateTime StartAt { get; set; }
    public DateTime? FinishAt { get; set; }
    public bool IsAsap { get; set; }
    public DurationTerm DurationTerm { get; set; }     // LongTerm, ShortTerm

    // Employment
    public EmploymentType EmploymentType { get; set; } // FullTime, PartTime, Contractor

    // Location
    public Guid JobLocationId { get; set; }

    // Rates
    public Guid? JobPositionRateId { get; set; }
    public decimal? WorkerRate { get; set; }
    public decimal? AgencyRate { get; set; }
    public string Currency { get; set; }

    // Shift
    public TimeSpan? ShiftStart { get; set; }
    public TimeSpan? ShiftEnd { get; set; }
    public TimeSpan? DurationBreak { get; set; }

    // Incentive
    public decimal? Incentive { get; set; }
    public string IncentiveDescription { get; set; }

    // Status
    public RequestStatus RequestStatus { get; set; }
    public bool IsOpen { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Agency Agency { get; set; }
    public CompanyProfile CompanyProfile { get; set; }
    public Location JobLocation { get; set; }
    public CompanyProfileJobPositionRate JobPositionRate { get; set; }
    public ICollection<WorkerRequest> WorkerRequests { get; set; }
    public ICollection<RequestRecruiter> Recruiters { get; set; }
}
```

**RequestStatus Enum:**
```csharp
public enum RequestStatus
{
    Requested = 1,      // Just created
    InProcess = 2,      // Workers assigned, in progress
    Cancelled = 3       // Cancelled
}
```

**DurationTerm Enum:**
```csharp
public enum DurationTerm
{
    LongTerm = 1,       // Permanent or long contract
    ShortTerm = 2       // Temporary, short contract
}
```

**EmploymentType Enum:**
```csharp
public enum EmploymentType
{
    FullTime = 1,
    PartTime = 2,
    Contractor = 3,
    Temporary = 4
}
```

---

### WorkerRequest (Assignment)

**Tabla:** `WorkerRequest`

**Purpose:** Links Worker to Request (assignment).

```csharp
public class WorkerRequest
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid WorkerProfileId { get; set; }
    public WorkerRequestStatus Status { get; set; }
    public DateTime StartWorking { get; set; }
    public int WeekStartWorking { get; set; }
    public string RejectComments { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Request Request { get; set; }
    public WorkerProfile WorkerProfile { get; set; }
    public ICollection<TimeSheet> TimeSheets { get; set; }
}
```

**WorkerRequestStatus Enum:**
```csharp
public enum WorkerRequestStatus
{
    Booked = 1,         // Assigned and active
    Rejected = 2        // Rejected (can be re-booked)
}
```

---

## ⏱️ TIMESHEET Module

### TimeSheet

**Tabla:** `TimeSheet`
**Ubicación:** `Covenant.Common/Entities/Request/TimeSheet.cs`

```csharp
public class TimeSheet
{
    public Guid Id { get; set; }
    public Guid WorkerRequestId { get; set; }
    public DateTime Date { get; set; }

    // Punch card times (actual)
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public DateTime? ClockInRounded { get; set; }
    public DateTime? ClockOutRounded { get; set; }

    // Normalized times (for calculations)
    public DateTime? TimeIn { get; set; }              // Normalized to midnight
    public DateTime? TimeOut { get; set; }             // Hours from TimeIn

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

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public WorkerRequest WorkerRequest { get; set; }
    public TimeSheetTotal Total { get; set; }
}
```

---

### TimeSheetTotal

**Tabla:** `TimeSheetTotal`

**Purpose:** Calculated hours breakdown.

```csharp
public class TimeSheetTotal
{
    public Guid Id { get; set; }
    public Guid TimeSheetId { get; set; }

    // Calculated hours
    public TimeSpan TotalHours { get; set; }
    public TimeSpan RegularHours { get; set; }
    public TimeSpan OvertimeHours { get; set; }
    public TimeSpan NightShiftHours { get; set; }
    public TimeSpan HolidayHours { get; set; }
    public TimeSpan OtherRegularHours { get; set; }

    // Weekly accumulation (for overtime calculation)
    public TimeSpan AccumulateWeekHours { get; set; }

    // Navigation
    public TimeSheet TimeSheet { get; set; }
}
```

---

## 💰 ACCOUNTING Module

### PayStub

**Tabla:** `PayStub`
**Ubicación:** `Covenant.Common/Entities/Accounting/PayStub.cs`

```csharp
public class PayStub
{
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public int PayStubNumber { get; set; }             // Sequential number
    public int PayStubNumberId { get; set; }           // Year
    public string TypeOfWork { get; set; }

    // Period
    public DateTime DateWorkBegins { get; set; }
    public DateTime DateWorkEnd { get; set; }
    public DateTime PaymentDate { get; set; }

    // Earnings
    public decimal RegularWage { get; set; }
    public decimal GrossPayment { get; set; }
    public decimal Vacations { get; set; }             // 4% in Canada
    public decimal PublicHolidayPay { get; set; }
    public decimal TotalEarnings { get; set; }

    // Deductions
    public decimal Cpp { get; set; }                   // Canada Pension Plan
    public decimal Ei { get; set; }                    // Employment Insurance
    public decimal FederalTax { get; set; }
    public decimal ProvincialTax { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions { get; set; }

    // Net Pay
    public decimal TotalPaid { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public WorkerProfile WorkerProfile { get; set; }
    public ICollection<PayStubItem> Items { get; set; }
    public ICollection<PayStubWageDetail> WageDetails { get; set; }
}
```

---

### PayStubItem

**Tabla:** `PayStubItem`

**Purpose:** Daily breakdown for pay stub.

```csharp
public class PayStubItem
{
    public Guid Id { get; set; }
    public Guid PayStubId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Hours { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }               // "Regular", "Overtime", etc

    // Navigation
    public PayStub PayStub { get; set; }
}
```

---

### PayStubWageDetail

**Tabla:** `PayStubWageDetail`

**Purpose:** Breakdown by wage type.

```csharp
public class PayStubWageDetail
{
    public Guid Id { get; set; }
    public Guid PayStubId { get; set; }
    public string Type { get; set; }               // "Regular", "Overtime", "Night Shift"
    public TimeSpan Hours { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }

    // Navigation
    public PayStub PayStub { get; set; }
}
```

---

### Invoice

**Tabla:** `Invoice`
**Ubicación:** `Covenant.Common/Entities/Accounting/Invoice.cs`

```csharp
public class Invoice
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public int InvoiceNumber { get; set; }

    // Rates (multipliers)
    public decimal NightShiftRate { get; set; }
    public decimal HolidayRate { get; set; }
    public decimal OverTimeRate { get; set; }
    public decimal VacationsRate { get; set; }         // Typically 0.04 (4%)
    public decimal HstRate { get; set; }               // 0.13 in Ontario
    public decimal BonusRate { get; set; }

    // Totals
    public decimal SubTotal { get; set; }
    public decimal Hst { get; set; }
    public decimal TotalNet { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public CompanyProfile CompanyProfile { get; set; }
    public ICollection<InvoiceTotal> InvoiceTotals { get; set; }
    public ICollection<InvoiceHoliday> Holidays { get; set; }
    public ICollection<InvoiceDiscount> Discounts { get; set; }
    public ICollection<InvoiceAdditionalItem> AdditionalItems { get; set; }
}
```

---

### InvoiceTotal

**Tabla:** `InvoiceTotal`

**Purpose:** Per-worker breakdown in invoice.

```csharp
public class InvoiceTotal
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid WorkerRequestId { get; set; }
    public string WorkerName { get; set; }

    // Hours breakdown
    public TimeSpan RegularHours { get; set; }
    public TimeSpan OvertimeHours { get; set; }
    public TimeSpan NightShiftHours { get; set; }
    public TimeSpan HolidayHours { get; set; }

    // Amounts
    public decimal RegularAmount { get; set; }
    public decimal OvertimeAmount { get; set; }
    public decimal NightShiftAmount { get; set; }
    public decimal HolidayAmount { get; set; }
    public decimal Total { get; set; }

    // Navigation
    public Invoice Invoice { get; set; }
    public WorkerRequest WorkerRequest { get; set; }
}
```

---

## 📍 SHARED Entities

### Location

**Tabla:** `Location`

```csharp
public class Location
{
    public Guid Id { get; set; }
    public string FullAddress { get; set; }
    public string StreetNumber { get; set; }
    public string StreetName { get; set; }
    public string Unit { get; set; }
    public Guid CityId { get; set; }
    public string PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Navigation
    public City City { get; set; }
}
```

### City

**Tabla:** `City`

```csharp
public class City
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ProvinceId { get; set; }

    // Navigation
    public Province Province { get; set; }
}
```

### Province

**Tabla:** `Province`

```csharp
public class Province
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }               // "ON", "BC", "QC"
    public Guid CountryId { get; set; }

    // Navigation
    public Country Country { get; set; }
}
```

### Country

**Tabla:** `Country`

```csharp
public class Country
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }               // "CA", "US"
}
```

---

## 🔗 Relaciones Clave

### 1:N Relationships

```
Agency → CompanyProfile (1:N)
Agency → WorkerProfile (1:N)
Agency → Request (1:N)
CompanyProfile → Request (1:N)
CompanyProfile → CompanyProfileLocation (1:N)
CompanyProfile → CompanyProfileJobPositionRate (1:N)
CompanyProfile → Invoice (1:N)
Request → WorkerRequest (1:N)
WorkerProfile → WorkerRequest (1:N)
WorkerProfile → PayStub (1:N)
WorkerRequest → TimeSheet (1:N)
```

### 1:1 Relationships

```
TimeSheet → TimeSheetTotal (1:1)
WorkerProfile → WorkerProfileTaxCategory (1:1)
```

---

## 🔍 Key Indexes

**Performance-critical indexes:**

```sql
-- Request filtering
CREATE INDEX idx_request_agency ON Request(AgencyId);
CREATE INDEX idx_request_company ON Request(CompanyProfileId);
CREATE INDEX idx_request_status ON Request(RequestStatus, IsOpen);

-- Worker filtering
CREATE INDEX idx_worker_agency ON WorkerProfile(AgencyId);
CREATE INDEX idx_worker_approved ON WorkerProfile(ApprovedToWork, Dnu);

-- TimeSheet queries
CREATE INDEX idx_timesheet_workerrequest ON TimeSheet(WorkerRequestId);
CREATE INDEX idx_timesheet_date ON TimeSheet(Date);

-- Accounting queries
CREATE INDEX idx_paystub_worker ON PayStub(WorkerProfileId);
CREATE INDEX idx_invoice_company ON Invoice(CompanyProfileId);
```
