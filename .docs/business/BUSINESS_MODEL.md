# Business Model - Covenant/Sigook Platform

## 🎯 Value Proposition

Covenant/Sigook is an **end-to-end staffing and recruitment platform** that connects temporary staffing agencies with companies that need workers, managing the full lifecycle from recruitment to payment.

### Problem It Solves

**Manage the complete temporary/permanent staffing flow including:**
- ✅ Worker registration and certification
- ✅ Matching workers with job orders
- ✅ Schedule and timesheet management with punch card
- ✅ Payroll processing with complex Canadian taxes (CPP, EI, Federal, Provincial)
- ✅ Automated invoicing for companies
- ✅ Regulatory compliance (documents, certifications, insurance)
- ✅ Legal document generation (pay stubs, invoices)

---

## 👥 Main Actors

### 1. AGENCY (Staffing Agency)

**Role:** Intermediary that connects Companies with Workers and manages the entire process.

**Agency Types:**
- **Master** — Main agency with sub-agencies
- **Regular** — Standard independent agency
- **BusinessPartner** — Business partner with limited access

**Responsibilities:**
- Recruit and approve Workers
- Manage Companies (clients)
- Create and assign job orders (Requests)
- Approve timesheets
- Process payroll for workers
- Bill companies
- Maintain regulatory compliance

**Structure:**
- Has physical locations (AgencyLocation) with billing address
- Has internal personnel (AgencyPersonnel):
  - Recruiters
  - Sales Representatives
  - Account Managers
- Has BusinessNumber, HstNumber (tax registration)

---

### 2. COMPANY (Client)

**Role:** Agency client that needs temporary or permanent staff.

**Status Pipeline:**
```
Lead → Potential → Prospect → Quoted → Client → Blocked/Inactive
```

**Responsibilities:**
- Define job positions with rates
- Create job orders (Requests)
- Review and approve candidates
- Review timesheets (optional)
- Receive billing for services

**Structure:**
- Has a profile (CompanyProfile) managed by an Agency
- Multiple locations (CompanyProfileLocation)
- Job positions with defined rates (CompanyProfileJobPositionRate):
  - **WorkerRate** — What is paid to the worker
  - **AgencyRate** — What the agency charges (includes markup)
- Contacts (CompanyProfileContactPerson)
- Internal users (CompanyUser) to manage orders

**Key data:**
- BusinessName, DbaName
- BusinessNumber, HstNumber
- Billing address and shipping addresses
- RequiresPermissionToSeeOrders (access control)

---

### 3. WORKER

**Role:** Professional looking for temporary or permanent employment through the platform.

**States and Flags:**
- `ApprovedToWork` — Approved by the agency to work (requires complete documents)
- `Dnu` (Do Not Use) — Marked as unavailable
- `IsSubcontractor` — Works as a subcontractor (different tax rules)
- `IsContractor` — Works as an independent contractor

**Responsibilities:**
- Complete registration with full information
- Keep documents current (SIN, IDs, licenses, certificates)
- Apply to job orders
- Complete timesheets (clock in/out)
- Receive pay stubs

**Profile Structure (WorkerProfile):**

**Personal Information:**
- FirstName, LastName, BirthDay, Gender
- SocialInsurance (SIN) with file and expiration date
- IdentificationNumber1/2 with files (Passport, Driver License, etc.)
- ProfileImage

**Contact Information:**
- MobileNumber, Phone, Email
- Location (Address, City, Province, PostalCode)
- HasVehicle

**Professional Information:**
- Skills (multiple skills)
- Languages (with proficiency level)
- Licenses (professional licenses with expiration)
- Certificates (certifications with expiration)
- JobExperience (work history)

**Availability:**
- AvailabilityType (FullTime, PartTime, Flexible)
- AvailabilityTime (available days and hours)
- LocationPreferences (preferred cities)

**Tax Information:**
- TaxCategory (FederalCategory, ProvincialCategory) — Claim codes for tax calculation
- Province — Determines which provincial tax table to use

---

### 4. CANDIDATE

**Role:** Prospect managed by the agency that does NOT yet have a user account in the system.

**Difference from Worker:**
- **Candidate** — Only exists in the agency's system, no associated User
- **Worker** — Has an associated User (email, authentication), can use the app

**Transition:**
```
Candidate (managed by agency) → Worker (registers in Flutter app)
```

**Use:**
- Agency manually registers Candidates
- Agency tracks and recruits them
- When the Candidate registers in the system, they become a Worker

---

## 🔄 End-to-End Business Flow

### PHASE 1: SETUP

```
┌─────────────────────────────────────────────────────────┐
│ 1. AGENCY registers COMPANY                             │
│    - CompanyProfile with BusinessName, locations        │
│    - Job Positions with rates (worker rate, agency rate)│
│    - Contact persons and users                          │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. WORKER registers via Flutter app                     │
│    - Personal info, contact, address                    │
│    - Documents (SIN, IDs, certificates)                 │
│    - Skills, languages, experience                      │
│    - Availability (days, hours, locations)              │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. AGENCY approves WORKER                               │
│    - Reviews documents and profile                      │
│    - Sets ApprovedToWork = true                         │
│    - Worker can see and apply to jobs                   │
└─────────────────────────────────────────────────────────┘
```

### PHASE 2: ORDER CREATION

```
┌─────────────────────────────────────────────────────────┐
│ COMPANY or AGENCY creates REQUEST                       │
│                                                          │
│ Key information:                                         │
│  - JobTitle, Description, Requirements                   │
│  - WorkersQuantity (how many needed)                    │
│  - JobLocation (where the work is performed)            │
│  - JobPositionRate (defines rates)                      │
│  - Shift (e.g. 7:00 AM - 3:00 PM)                       │
│  - DurationTerm (LongTerm/ShortTerm)                    │
│  - EmploymentType (FullTime/PartTime/Contractor)        │
│  - StartAt, FinishAt (dates)                            │
│  - Incentive (optional bonus)                           │
│                                                          │
│ Status:                                                  │
│  - Open: Active order with available capacity           │
│  - Filled: All positions filled                         │
│  - Cancelled: Cancelled                                 │
└─────────────────────────────────────────────────────────┘
```

### PHASE 3: MATCHING AND ASSIGNMENT

```
┌─────────────────────────────────────────────────────────┐
│ 1. WORKERS see Requests in Flutter app                  │
│    GET /api/WorkerRequest/Available                     │
│    - Filter by city, job type, rate                     │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. WORKER applies or AGENCY assigns                     │
│    POST /api/WorkerRequest/Apply                        │
│    POST /api/AgencyRequest/{requestId}/Worker           │
│                                                          │
│    Creates WORKERREQUEST:                                │
│     - Status: Booked                                    │
│     - StartWorking: Start date                          │
│     - WeekStartWorking: Start week                      │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. REQUEST gets filled                                  │
│    WorkersQuantityWorking >= WorkersQuantity            │
│    → Status transitions to Filled                       │
└─────────────────────────────────────────────────────────┘
```

### PHASE 4: WORK AND TIME TRACKING

```
┌─────────────────────────────────────────────────────────┐
│ 1. WORKER does daily Clock In/Out                       │
│    POST /api/WorkerRequest/{requestId}/TimeSheet        │
│    - ClockIn: 2026-02-01T07:05:23Z (real time)          │
│    - ClockInRounded: 2026-02-01T07:00:00Z (rounded)     │
│    - ClockOut: 2026-02-01T15:08:12Z                     │
│    - ClockOutRounded: 2026-02-01T15:00:00Z              │
│                                                          │
│    Creates one TIMESHEET per day worked                 │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. AGENCY approves TIMESHEET                            │
│    - TimeInApproved: 2026-02-01T07:00:00Z               │
│    - TimeOutApproved: 2026-02-01T15:00:00Z              │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. SYSTEM calculates TIMESHEETTOTAL                     │
│    - TotalHours = TimeOutApproved - TimeInApproved      │
│    - RegularHours (first 44 hrs/week)                   │
│    - OvertimeHours (after 44 hrs)                       │
│    - NightShiftHours (11 PM - 7 AM)                     │
│    - HolidayHours (if IsHoliday = true)                 │
│    - AccumulateWeekHours (weekly sum)                   │
└─────────────────────────────────────────────────────────┘
```

### PHASE 5: PAYROLL

```
┌─────────────────────────────────────────────────────────┐
│ 1. AGENCY selects workers for payroll                   │
│    POST /api/v4/Accounting/PayStub                      │
│    - Worker, PaymentDate, WeekEnding                    │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. SYSTEM calculates EARNINGS                           │
│    - RegularHours × WorkerRate = RegularWage            │
│    - OvertimeHours × (Rate × 1.5) = OvertimeWage        │
│    - NightShiftHours × NightShiftRate                   │
│    - HolidayHours × HolidayRate                         │
│    - GrossPayment = sum of all wages                    │
│    - Vacations = GrossPayment × 4% (mandatory in Canada)│
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. SYSTEM calculates DEDUCTIONS (see PAYROLL_RULES.md)  │
│    - CPP (Canada Pension Plan): 5.95%                   │
│    - EI (Employment Insurance): 1.66%                   │
│    - FederalTax (lookup tables)                         │
│    - ProvincialTax (per province)                       │
│    - TotalDeductions = sum                              │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 4. PAYSTUB GENERATED                                    │
│    - PayStubNumber: PS-0001-26                          │
│    - TotalEarnings = GrossPayment + Vacations           │
│    - TotalPaid = TotalEarnings - TotalDeductions        │
│    - Generates PDF and sends to Worker                  │
└─────────────────────────────────────────────────────────┘
```

### PHASE 6: BILLING

```
┌─────────────────────────────────────────────────────────┐
│ 1. AGENCY generates INVOICE for COMPANY                 │
│    POST /api/v4/Accounting/Invoice                      │
│    - CompanyProfile, WeekEnding, WorkerRequests         │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. SYSTEM calculates INVOICE TOTALS (see BILLING_RULES) │
│    Per Worker:                                           │
│     - RegularHours × AgencyRate                         │
│     - OvertimeHours × (AgencyRate × 1.5)                │
│     - NightShiftHours × NightShiftRate                  │
│     - HolidayHours × HolidayRate                        │
│    SubTotal = sum of all workers                        │
│    Vacations = SubTotal × 4%                            │
│    HST/GST = (SubTotal + Vacations) × TaxRate           │
│    TotalNet = SubTotal + Vacations + HST                │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. INVOICE GENERATED                                    │
│    - InvoiceNumber: AI-0001-26                          │
│    - InvoiceTotals (per-worker breakdown)               │
│    - Generates PDF and sends to Company recipients      │
└─────────────────────────────────────────────────────────┘
```

---

## 💰 Revenue Model (Agency)

### Revenue = Markup on Worker Rates

```
AgencyRate - WorkerRate = Agency Profit (Markup)

Example:
- AgencyRate: $25/hr (charged to the Company)
- WorkerRate: $18/hr (paid to the Worker)
- Markup: $7/hr (28% profit margin)

For 40 hours/week:
- Agency charges Company: $1,000
- Agency pays Worker: $720
- Agency profit: $280/week per worker
```

### Agency Costs:
- Payroll processing (CPP, EI employer contributions)
- Insurance and liability
- Overhead (staff, office, software)
- Marketing and recruitment

---

## 🎯 Competitive Differentiators

### 1. Full Automation
- From job posting to invoice generation
- Automatic payroll calculations (complex Canadian taxes)
- Document generation (PDF pay stubs, invoices)

### 2. Multi-Jurisdiction
- Canada (CPP, EI, Federal/Provincial taxes)
- USA (Federal, State, FICA) — prepared for expansion
- Up-to-date tax tables

### 3. Mobile-First for Workers
- Native Flutter app for iOS/Android
- Clock in/out with GPS
- Real-time job search
- Document upload

### 4. Compliance and Tracking
- Document expiry tracking
- License/certificate validation
- Full audit trail
- Legal document generation

### 5. Cloud-Native
- Azure Storage for documents
- Azure Service Bus for async processing
- Scalable and resilient
