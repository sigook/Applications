# Entities and Relationships - Covenant/Sigook Platform

> Code samples in this document are simplified projections of the actual entities. They show the most relevant fields, not every property. The source of truth is `Covenant.Api/Covenant.Common/Entities/`.

## 📊 Main Relationship Diagram

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
│ CompanyId (FK)       │    │ Email           │
│ AgencyId (FK)        │    │ Enabled         │
│ JobTitle             │    └─────────────────┘
│ WorkersQuantity      │
│ Status               │
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
│ WorkerId (FK)                │
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
│ Cpp, Ei                      │
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

### Agency

**Table:** `Agency`
**Location:** `Covenant.Common/Entities/Agency/Agency.cs`

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

**Table:** `AgencyLocation`

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

**Table:** `AgencyPersonnel`

```csharp
public class AgencyPersonnel
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public PersonnelType Type { get; set; }           // Recruiter, SalesRep, etc.
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

**Table:** `CompanyProfile`
**Location:** `Covenant.Common/Entities/Company/CompanyProfile.cs`

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
    public bool RequiresPermissionToSeeRequests { get; set; }
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

**Table:** `CompanyProfileLocation`

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

**Table:** `CompanyProfileJobPositionRate`

**CRITICAL:** Defines the rates that drive pricing.

```csharp
public class CompanyProfileJobPositionRate
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string JobTitle { get; set; }

    // Rates
    public decimal WorkerRate { get; set; }            // Paid to the worker
    public decimal AgencyRate { get; set; }            // Charged to the company
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

**Table:** `CompanyProfileContactPerson`

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

**Table:** `CompanyUser`

**Purpose:** Links a User to a CompanyProfile for access control.

```csharp
public class CompanyUser
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public Guid UserId { get; set; }
    public bool CanSeeRequests { get; set; }
    public bool IsActive { get; set; }

    // Navigation
    public CompanyProfile CompanyProfile { get; set; }
    public User User { get; set; }
}
```

---

## 👷 WORKER Module

### WorkerProfile

**Table:** `WorkerProfile`
**Location:** `Covenant.Common/Entities/Worker/WorkerProfile.cs`

```csharp
public class WorkerProfile
{
    public Guid Id { get; set; }
    public int NumberId { get; set; }                  // Sequential worker number
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
    public bool ApprovedToWork { get; set; }           // Critical: can the worker work?
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

> **Note:** `WorkerProfile.NumberId` (not `PayStub`) is the canonical source for the worker's sequential number used in pay stubs and reports.

---

### WorkerProfileTaxCategory

**Table:** `WorkerProfileTaxCategory`

**Purpose:** Tax claim codes for tax calculations.

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

### WorkerSkill / WorkerLanguage / WorkerLicense / WorkerCertificate / WorkerAvailability / WorkerLocationPreference

Standard child tables linking a `WorkerProfile` to its skills, languages, licenses, certificates, available days/hours, and preferred cities. Each follows the same shape: a primary key, a `WorkerProfileId` foreign key, and the relevant attributes (e.g. `LicenseName`, `ExpiryDate`, `Proficiency`, etc.).

See `Covenant.Common/Entities/Worker/` for the full definitions.

---

## 📋 REQUEST Module

### Request

**Table:** `Request`
**Location:** `Covenant.Common/Entities/Request/Request.cs`

```csharp
public class Request
{
    public Guid Id { get; set; }
    public int NumberId { get; set; }
    public Guid AgencyId { get; set; }
    public Guid CompanyId { get; set; }                // FK to User (the company user)

    // Job Details
    public string JobTitle { get; set; }
    public string BillingTitle { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }
    public string InternalRequirements { get; set; }
    public string Responsibilities { get; set; }

    // Workers
    public int WorkersQuantity { get; set; }           // How many workers needed
    public int WorkersQuantityWorking { get; private set; } // Currently assigned

    // Timing
    public DateTime? StartAt { get; set; }
    public DateTime? FinishAt { get; set; }
    public bool IsAsap { get; set; }
    public DurationTerm DurationTerm { get; set; }     // LongTerm, ShortTerm

    // Employment
    public EmploymentType EmploymentType { get; set; } // FullTime, PartTime, Contractor

    // Location
    public Guid JobLocationId { get; set; }
    public Location JobLocation { get; set; }
    public bool JobIsOnBranchOffice { get; set; }

    // Rates
    public Guid? JobPositionRateId { get; set; }
    public CompanyProfileJobPositionRate JobPositionRate { get; set; }
    public decimal? WorkerRate { get; set; }
    public decimal? AgencyRate { get; set; }
    public decimal? WorkerSalary { get; set; }

    // Shift
    public Guid? ShiftId { get; set; }
    public Shift Shift { get; set; }
    public TimeSpan DurationBreak { get; set; }
    public bool BreakIsPaid { get; set; }
    public bool HolidayIsPaid { get; set; }
    public bool PunchCardOptionEnabled { get; set; }

    // Incentive
    public decimal? Incentive { get; set; }
    public string IncentiveDescription { get; set; }

    // Status (single source of truth — no IsOpen flag)
    public RequestStatus Status { get; private set; }

    // Derived
    public bool CanBeUpdated => Status != RequestStatus.Cancelled;
    public bool IsAvailableToApply => Status == RequestStatus.Open;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Agency Agency { get; set; }
    public User Company { get; set; }                  // The user that owns this request on the company side
    public IReadOnlyCollection<WorkerRequest> Workers { get; }
    public IReadOnlyCollection<RequestRecruiter> Recruiters { get; }
}
```

> The `Request` entity exposes only `Status` — there is **no** `IsOpen` property. State transitions happen automatically through methods such as `AddWorker`, `RejectWorker`, `Cancel`, `Open`, `IncreaseWorkersQuantityByOne`, and `DecreaseWorkersQuantityByOne`. See `.docs/business/REQUEST_STATE_MANAGEMENT.md` for the full transition rules.
>
> The link from `Request.CompanyId` is to the company-side `User`, not directly to `CompanyProfile`.

**RequestStatus Enum:**
```csharp
public enum RequestStatus
{
    Open = 1,        // Active request with available capacity
    Filled = 3,      // All positions filled
    Cancelled = 4    // Cancelled
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

**Table:** `WorkerRequest`

**Purpose:** Links a Worker to a Request (assignment).

```csharp
public class WorkerRequest
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid WorkerId { get; set; }
    public WorkerRequestStatus WorkerRequestStatus { get; set; }
    public DateTime StartWorking { get; set; }
    public int WeekStartWorking { get; set; }
    public string RejectComments { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Request Request { get; set; }
    public WorkerProfile WorkerProfile { get; set; }
    public ICollection<TimeSheet> TimeSheets { get; set; }

    public bool IsRejected => WorkerRequestStatus == WorkerRequestStatus.Rejected;
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

### RequestSource (Job Board Posting)

**Table:** `RequestSource`

**Purpose:** Links a Request to a Source (job board / platform) where it is published. Many-to-many between `Request` and `Source`. Only sources flagged with `IsAvailableForRequests = true` are exposed to the agency UI (today: `Indeed`, `Zip Recruiter`, `Social Media`).

```csharp
public class RequestSource
{
    public Guid RequestId { get; set; }
    public Guid SourceId { get; set; }
    public DateTime? PublishedAt { get; set; }   // optional
    public string ExternalUrl { get; set; }       // optional
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Request Request { get; set; }
    public Source Source { get; set; }
}
```

**Primary key:** composite `(RequestId, SourceId)`.

**Source flag:** `Source.IsAvailableForRequests` (bool, default `false`) decides whether the source is selectable as a job board. The existing candidate-source listing keeps every value; the new `GET api/Catalog/source/requests` endpoint returns only the ones flagged ON.

---

### Runner

**Table:** `Runners` (entities under `Entities/Request/Runners/`)

**Purpose:** A Candidate or Worker actively submitted to a specific Request (order), tracked through a recruiting pipeline with full status history and interviews. The same person can be a Runner on multiple requests at once, each with independent status and history. Like `RequestApplicant`, a runner references **either** a `WorkerProfile` **or** a `Candidate` (mutually exclusive) and preserves the link to the original record — no personal data is duplicated.

```csharp
public class Runner
{
    public Guid Id { get; private set; }
    public long NumberId { get; private set; }
    public Guid AgencyId { get; private set; }
    public Guid RequestId { get; private set; }           // the order (OrderId)
    public Guid? WorkerProfileId { get; private set; }    // worker XOR candidate
    public Guid? CandidateId { get; private set; }
    public RunnerType Type { get; private set; }          // Active / Passive
    public RunnerStatus Status { get; private set; }      // current stage
    public DateTime? StartDate { get; private set; }      // set only when status = Hired (required to hire)
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Backing-field collections (append-only history + interviews)
    public IEnumerable<RunnerStatusHistory> StatusHistory { get; }
    public IEnumerable<RunnerInterview> Interviews { get; }

    // Domain methods (single source of truth for the rules below)
    public static Result<Runner> CreateFromWorker(...);
    public static Result<Runner> CreateFromCandidate(...);
    public static bool CanAddInterview(RunnerStatus status);
    public Result ChangeStatus(RunnerStatus next, string changedBy, string comments = null, DateTime? startDate = null);
    public Result<RunnerInterview> AddInterview(...);
    public Result RescheduleInterview(Guid interviewId, DateTime newDate, string rescheduledBy);
}
```

**Business rules (enforced in the domain entity):**
- Created with `Status = SentToClient` and an initial `RunnerStatusHistory` entry (`PreviousStatus = null`).
- Status transitions are **unrestricted** (any → any) **except**: a `Hired` runner is terminal — its status cannot change.
- Every status change **appends** (never overwrites) a `RunnerStatusHistory` row.
- Moving to `Hired` **requires** a `StartDate`; the transition is rejected without it. `StartDate` is captured only on this transition.
- A worker/candidate cannot be a Runner **twice on the same request** (`IRunnerRepository.RunnerExists` guard, regardless of current status).
- Interviews can be **added or rescheduled only** when status is `InterviewScheduled` or `InterviewRescheduled` (`Runner.CanAddInterview`). Rescheduling auto-transitions the runner to `InterviewRescheduled`.

**RunnerType Enum:** `Active = 1` (reached the order on their own initiative), `Passive = 2` (sourced by a recruiter).

**RunnerStatus Enum:** `SentToClient = 1`, `InterviewScheduled = 2`, `InterviewRescheduled = 3`, `NoLongerAvailable = 4`, `NoShow = 5`, `WaitingForInterviewFeedback = 6`, `WaitingForFinalDecision = 7`, `Rejected = 8`, `InOnboardingProcess = 9`, `Hired = 10`. Stored as text via `EnumToStringConverter`.

**Child entities:**

```csharp
public class RunnerStatusHistory          // Table: RunnerStatusHistories (append-only)
{
    public Guid Id { get; }
    public Guid RunnerId { get; }
    public RunnerStatus? PreviousStatus { get; }   // null for the initial entry
    public RunnerStatus NewStatus { get; }
    public string ChangedBy { get; }
    public DateTime ChangedAt { get; }
    public string Comments { get; }
}

public class RunnerInterview               // Table: RunnerInterviews
{
    public Guid Id { get; }
    public Guid RunnerId { get; }
    public DateTime ScheduledDate { get; }
    public InterviewType Type { get; }             // Phone / Video / Onsite
    public string Interviewer { get; }
    public InterviewStatus Status { get; }         // Scheduled / Rescheduled
    public string Feedback { get; }
    public string Notes { get; }
    public int RescheduleCount { get; }
    public DateTime CreatedAt { get; }
    public string CreatedBy { get; }
    public DateTime? RescheduledAt { get; }
    public string RescheduledBy { get; }
}
```

**API:** `RunnersController` at `api/agency/requests/{requestId}/Runners` (Agency policy + `AgencyIdFilter`): list/detail, create, `PUT {id}/Status`, `POST {id}/Interview`, `PUT {id}/Interview/{interviewId}/Reschedule`, and `GET {requestId}/Runners/Search?searchTerm=` (workers/candidates that can be added as runners, **excluding those already runners** on the request — backed by `IRequestRepository.SearchRunnerProspects`, which reuses the applicant-search predicates with a runner-based exclusion). Enums are serialized as numbers (no `JsonStringEnumConverter`).

---

## ⏱️ TIMESHEET Module

### TimeSheet

**Table:** `TimeSheet`
**Location:** `Covenant.Common/Entities/Request/TimeSheet.cs`

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

**Table:** `TimeSheetTotal`

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

**Table:** `PayStub`
**Location:** `Covenant.Common/Entities/Accounting/PayStub.cs`

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

**Table:** `PayStubItem`

**Purpose:** Daily breakdown for the pay stub.

```csharp
public class PayStubItem
{
    public Guid Id { get; set; }
    public Guid PayStubId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Hours { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }               // "Regular", "Overtime", etc.

    // Navigation
    public PayStub PayStub { get; set; }
}
```

---

### PayStubWageDetail

**Table:** `PayStubWageDetail`

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

**Table:** `Invoice`
**Location:** `Covenant.Common/Entities/Accounting/Invoice.cs`

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

**Table:** `InvoiceTotal`

**Purpose:** Per-worker breakdown in an invoice.

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

**Table:** `Location`

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

**Table:** `City`

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

**Table:** `Province`

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

**Table:** `Country`

```csharp
public class Country
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }               // "CA", "US"
}
```

---

## 🔗 Key Relationships

### 1:N Relationships

```
Agency → CompanyProfile (1:N)
Agency → WorkerProfile (1:N)
Agency → Request (1:N)
CompanyProfile → Request (1:N)             (via Request.CompanyId → User → CompanyProfile)
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
CREATE INDEX idx_request_agency  ON Request(AgencyId);
CREATE INDEX idx_request_company ON Request(CompanyId);
CREATE INDEX idx_request_status  ON Request(Status);

-- Worker filtering
CREATE INDEX idx_worker_agency   ON WorkerProfile(AgencyId);
CREATE INDEX idx_worker_approved ON WorkerProfile(ApprovedToWork, Dnu);

-- TimeSheet queries
CREATE INDEX idx_timesheet_workerrequest ON TimeSheet(WorkerRequestId);
CREATE INDEX idx_timesheet_date          ON TimeSheet(Date);

-- Accounting queries
CREATE INDEX idx_paystub_worker  ON PayStub(WorkerProfileId);
CREATE INDEX idx_invoice_company ON Invoice(CompanyProfileId);
```
