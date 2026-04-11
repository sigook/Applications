# Workflows - Covenant/Sigook Platform

## 🔄 End-to-End Flows

This document details the main system workflows with each technical step.

---

## 1️⃣ Worker Registration Flow

### Overview
Worker registers from the Flutter app, the Agency approves, and the Worker can start working.

### Actors
- **Worker** (via Flutter app)
- **Agency** (via Web app)
- **System** (backend API, IdentityServer)

---

### Step-by-Step

#### STEP 1: Worker starts registration in the app

**Actor:** Worker (Flutter app)

**Action:**
```dart
// Navigate to RegistrationScreen
context.push('/registration');
```

**App loads catalogs:**
```
GET /api/Catalog/Countries
GET /api/Catalog/Provinces/CA
GET /api/Catalog/Cities/{provinceId}
GET /api/Catalog/Genders
GET /api/Catalog/IdentificationTypes
GET /api/Catalog/Skills
GET /api/Catalog/Languages
```

**UI:** Multi-step form with 8 sections
1. Basic Info
2. Contact Info
3. Personal Info (SIN, IDs)
4. Professional Info (Skills, Languages)
5. Availability
6. Preferences (Locations)
7. Documents (Photos, Files)
8. Account (Email, Password)

---

#### STEP 2: Worker fills out the form

**Actor:** Worker (Flutter app)

**Validations (client-side):**
- Email format (EmailValidator)
- Phone format (PhoneNumber value object)
- Password strength (PasswordValidator)
- Required fields

**State Management:**
```dart
// RegistrationViewModel (Riverpod)
class RegistrationViewModel extends StateNotifier<RegistrationState> {
  void updateSection(SectionData data) {
    // Save to local storage (draft)
    // Update state
  }
}
```

---

#### STEP 3: Worker submits registration

**Actor:** Worker (Flutter app)

**API Call:**
```http
POST /api/WorkerProfile
Content-Type: multipart/form-data

Form Data:
- registration_form: {JSON}
- profile_photo: {file}
- sin_document: {file}
- id_document_1: {file}
- id_document_2: {file}
```

**registration_form JSON:**
```json
{
  "basicInfo": {
    "firstName": "John",
    "lastName": "Doe",
    "birthDay": "1990-01-01",
    "genderId": "guid",
    "hasVehicle": true
  },
  "contactInfo": {
    "mobileNumber": "+14165550100",
    "phone": null,
    "location": {
      "streetNumber": "123",
      "streetName": "Main St",
      "unit": "101",
      "cityId": "guid",
      "postalCode": "M1A1A1"
    }
  },
  "personalInfo": {
    "socialInsurance": "123456789",
    "socialInsuranceDueDate": "2030-01-01",
    "identificationNumber1": "A1234567",
    "identificationType1Id": "guid",
    "identificationNumber2": "D1234567890",
    "identificationType2Id": "guid"
  },
  "professionalInfo": {
    "skillIds": ["guid1", "guid2"],
    "languages": [
      {
        "languageId": "guid",
        "proficiency": "Advanced"
      }
    ]
  },
  "availabilityInfo": {
    "availabilityType": "FullTime",
    "availabilities": [
      {
        "dayOfWeek": "Monday",
        "startTime": "07:00:00",
        "endTime": "15:00:00"
      }
    ]
  },
  "preferencesInfo": {
    "cityIds": ["guid1", "guid2"]
  },
  "accountInfo": {
    "email": "john@example.com",
    "password": "SecurePassword123!"
  }
}
```

---

#### STEP 4: Backend processes the registration

**Actor:** System (Covenant.Api)

**Backend Flow:**

**4.1 - Create User (IdentityServer)**
```csharp
// 1. Create User in IdentityServer
var user = new User
{
    Email = request.AccountInfo.Email,
    UserName = request.AccountInfo.Email,
    Enabled = true
};
await _userManager.CreateAsync(user, request.AccountInfo.Password);

// 2. Assign role
await _userManager.AddToRoleAsync(user, "Worker");
```

**4.2 - Create Location**
```csharp
// Geocode address
var geocoded = await _geocodeService.GeocodeAsync(
    request.ContactInfo.Location.FullAddress);

var location = new Location
{
    StreetNumber = request.ContactInfo.Location.StreetNumber,
    StreetName = request.ContactInfo.Location.StreetName,
    Unit = request.ContactInfo.Location.Unit,
    CityId = request.ContactInfo.Location.CityId,
    PostalCode = request.ContactInfo.Location.PostalCode,
    Latitude = geocoded.Latitude,
    Longitude = geocoded.Longitude
};
await _locationRepository.AddAsync(location);
```

**4.3 - Upload Files to Azure Storage**
```csharp
// Upload profile photo
var profileImageUrl = await _azureStorageService.UploadAsync(
    container: "worker-photos",
    file: request.ProfilePhoto,
    fileName: $"{userId}_profile.jpg"
);

// Upload SIN document
var sinFileId = await _fileService.UploadAsync(
    file: request.SinDocument,
    fileType: FileType.SIN
);

// Upload ID documents
var id1FileId = await _fileService.UploadAsync(
    file: request.IdDocument1,
    fileType: FileType.Identification
);
```

**4.4 - Create WorkerProfile**
```csharp
var workerProfile = new WorkerProfile
{
    UserId = user.Id,
    AgencyId = agencyId, // From context
    FirstName = request.BasicInfo.FirstName,
    LastName = request.BasicInfo.LastName,
    BirthDay = request.BasicInfo.BirthDay,
    GenderId = request.BasicInfo.GenderId,
    HasVehicle = request.BasicInfo.HasVehicle,

    MobileNumber = request.ContactInfo.MobileNumber,
    Phone = request.ContactInfo.Phone,
    LocationId = location.Id,

    SocialInsurance = request.PersonalInfo.SocialInsurance,
    SocialInsuranceDueDate = request.PersonalInfo.SocialInsuranceDueDate,
    SocialInsuranceFileId = sinFileId,
    IdentificationNumber1 = request.PersonalInfo.IdentificationNumber1,
    IdentificationType1 = request.PersonalInfo.IdentificationType1Id,
    IdentificationType1FileId = id1FileId,

    ProfileImage = profileImageUrl,

    // Important: Not approved yet
    ApprovedToWork = false,
    Dnu = false,
    IsSubcontractor = false,
    IsContractor = false
};

await _workerRepository.AddAsync(workerProfile);
```

**4.5 - Add Skills, Languages, Availabilities**
```csharp
// Skills
foreach (var skillId in request.ProfessionalInfo.SkillIds)
{
    var workerSkill = new WorkerSkill
    {
        WorkerProfileId = workerProfile.Id,
        SkillId = skillId
    };
    await _workerSkillRepository.AddAsync(workerSkill);
}

// Languages
foreach (var lang in request.ProfessionalInfo.Languages)
{
    var workerLanguage = new WorkerLanguage
    {
        WorkerProfileId = workerProfile.Id,
        LanguageId = lang.LanguageId,
        Proficiency = lang.Proficiency
    };
    await _workerLanguageRepository.AddAsync(workerLanguage);
}

// Availabilities
foreach (var avail in request.AvailabilityInfo.Availabilities)
{
    var workerAvailability = new WorkerAvailability
    {
        WorkerProfileId = workerProfile.Id,
        DayOfWeek = avail.DayOfWeek,
        StartTime = avail.StartTime,
        EndTime = avail.EndTime
    };
    await _workerAvailabilityRepository.AddAsync(workerAvailability);
}
```

**4.6 - Send Notifications**
```csharp
// Email to worker (welcome)
await _emailService.SendAsync(
    to: user.Email,
    subject: "Welcome to Sigook!",
    template: "WorkerRegistrationComplete",
    data: new { FirstName = workerProfile.FirstName }
);

// Notification to agency (new worker pending approval)
await _teamsWebhookService.SendAsync(
    message: $"New worker registered: {workerProfile.FirstName} {workerProfile.LastName}",
    actionUrl: $"/agency/workers/{workerProfile.Id}"
);
```

**Response:**
```json
{
  "id": "worker-profile-guid",
  "userId": "user-guid",
  "approvedToWork": false,
  "message": "Registration successful. Awaiting agency approval."
}
```

---

#### STEP 5: Agency reviews the worker profile

**Actor:** Agency (Web app)

**Agency navigates to:**
```
/agency/workers      → List of all workers
/agency/workers/{id} → Worker detail
```

**Agency reviews:**
- Personal information
- Documents (SIN, IDs)
- Skills and experience
- Availability
- Background check (external)

---

#### STEP 6: Agency approves the worker

**Actor:** Agency (Web app)

**API Call:**
```http
PUT /api/AgencyWorkerProfile/{id}/ApproveToWork
{
  "approvedToWork": true
}
```

**Backend:**
```csharp
// Update worker profile
workerProfile.ApprovedToWork = true;
workerProfile.UpdatedAt = DateTime.UtcNow;
await _workerRepository.UpdateAsync(workerProfile);

// Send notification to worker
await _pushNotificationService.SendAsync(
    userId: workerProfile.UserId,
    title: "Profile Approved!",
    body: "You can now apply to jobs",
    data: { type: "ProfileApproved" }
);

await _emailService.SendAsync(
    to: workerProfile.User.Email,
    subject: "Your Sigook profile has been approved",
    template: "WorkerApproved",
    data: new { FirstName = workerProfile.FirstName }
);
```

---

#### STEP 7: Worker can now apply to jobs

**Actor:** Worker (Flutter app)

**Worker receives push notification:**
```
Title: "Profile Approved!"
Body: "You can now apply to jobs"
```

**Worker can now:**
- Browse available jobs
- Apply to jobs
- View assigned jobs

---

## 2️⃣ Job Creation & Matching Flow

### Overview
Company creates job order → Agency assigns workers or Workers apply → Assignment confirmed.

---

### STEP 1: Company creates a Request

**Actor:** Company (Web app)

**UI:** Request creation form

**API Call:**
```http
POST /api/CompanyRequest
{
  "jobTitle": "Warehouse Worker",
  "description": "Loading and unloading trucks, inventory management",
  "requirements": "Forklift license required",
  "workersQuantity": 5,
  "startAt": "2026-02-01T00:00:00Z",
  "finishAt": null,
  "isAsap": false,
  "durationTerm": "LongTerm",
  "employmentType": "FullTime",
  "jobPositionRateId": "guid",
  "jobLocationId": "guid",
  "shiftStart": "07:00:00",
  "shiftEnd": "15:00:00",
  "durationBreak": "00:30:00",
  "incentive": 100,
  "incentiveDescription": "$100 signing bonus"
}
```

**Backend:**
```csharp
// Validations
if (request.WorkersQuantity < 1)
    throw new ValidationException("Must request at least 1 worker");

if (request.JobTitle.Length > 500)
    throw new ValidationException("Job title too long");

// Get JobPositionRate (to populate rates)
var jobPositionRate = await _jobPositionRateRepository
    .GetByIdAsync(request.JobPositionRateId);

// Create Request (initial Status = Open)
var jobRequest = new Request
{
    AgencyId = company.AgencyId,
    CompanyId = company.Id,
    JobTitle = request.JobTitle,
    Description = request.Description,
    Requirements = request.Requirements,
    WorkersQuantity = request.WorkersQuantity,
    StartAt = request.StartAt,
    FinishAt = request.FinishAt,
    IsAsap = request.IsAsap,
    DurationTerm = request.DurationTerm,
    EmploymentType = request.EmploymentType,
    JobLocationId = request.JobLocationId,
    JobPositionRateId = request.JobPositionRateId,
    WorkerRate = jobPositionRate.WorkerRate,
    AgencyRate = jobPositionRate.AgencyRate,
    Incentive = request.Incentive,
    IncentiveDescription = request.IncentiveDescription,
    CreatedAt = DateTime.UtcNow
};
// Status defaults to RequestStatus.Open

await _requestRepository.AddAsync(jobRequest);

// Notify agency
await _teamsWebhookService.SendAsync(
    message: $"New request: {jobRequest.JobTitle} ({jobRequest.WorkersQuantity} workers needed)",
    actionUrl: $"/agency/requests/{jobRequest.Id}"
);
```

---

### STEP 2A: Agency assigns workers (Proactive)

**Actor:** Agency (Web app)

**Agency searches available workers:**
```http
GET /api/AgencyWorkerProfile?approvedToWork=true&skillIds=guid1,guid2&cityId=guid
```

**Agency assigns a worker:**
```http
POST /api/AgencyRequest/{requestId}/Worker
{
  "workerProfileId": "guid",
  "startWorking": "2026-02-01T00:00:00Z"
}
```

**Backend:**
```csharp
// Validations
if (!worker.ApprovedToWork)
    throw new BusinessException("Worker not approved to work");

if (worker.Dnu)
    throw new BusinessException("Worker marked as Do Not Use");

if (request.WorkersQuantityWorking >= request.WorkersQuantity)
    throw new BusinessException("Request already filled");

// AddWorker handles state transitions automatically
var result = request.AddWorker(worker.Id, startWorking);
if (!result.IsSuccess) throw new BusinessException(result.Error);

// If capacity is reached, request.Status becomes Filled
await _requestRepository.UpdateAsync(request);

// Notify worker
await _pushNotificationService.SendAsync(
    userId: worker.UserId,
    title: "New Job Assignment",
    body: $"You've been assigned to: {request.JobTitle}",
    data: { type: "JobAssigned", requestId: request.Id }
);
```

---

### STEP 2B: Worker applies (Reactive)

**Actor:** Worker (Flutter app)

**Worker browses jobs:**
```http
GET /api/WorkerRequest/Available?cityId=guid&page=1&pageSize=20
```

**Worker applies:**
```http
POST /api/WorkerRequest/Apply
{
  "requestId": "guid"
}
```

**Backend:** (Similar to Agency assign, but the workerId is taken from the authenticated user)

---

### STEP 3: Request gets filled

**Backend (automatic in `Request.AddWorker`):**
```csharp
// After each worker assignment
if (WorkersQuantityWorking >= WorkersQuantity)
{
    Status = RequestStatus.Filled;
}
```

After the request transitions to `Filled`, the company can be notified:
```csharp
await _emailService.SendAsync(
    to: company.Email,
    subject: $"Request filled: {request.JobTitle}",
    body: $"All {request.WorkersQuantity} workers have been assigned."
);
```

---

## 3️⃣ Time Tracking Flow (Punch Card)

### Overview
Worker clocks in/out daily → Agency approves → System calculates totals.

---

### STEP 1: Worker clocks in

**Actor:** Worker (Flutter app)

**Action:** Worker taps "Clock In" button on the job

**API Call:**
```http
POST /api/WorkerRequest/{requestId}/TimeSheet
{
  "clockIn": "2026-02-01T07:05:23Z",
  "latitude": 43.6532,
  "longitude": -79.3832,
  "isHoliday": false
}
```

**Backend:**
```csharp
// Validations
var existingToday = await _timeSheetRepository
    .GetByWorkerRequestAndDateAsync(workerRequestId, clockIn.Date);
if (existingToday != null)
    throw new BusinessException("Already clocked in today");

// Round clock in time
var clockInRounded = RoundToNearest15Minutes(clockIn);

// Create TimeSheet
var timeSheet = new TimeSheet
{
    WorkerRequestId = workerRequestId,
    Date = clockIn.Date,
    ClockIn = clockIn,
    ClockInRounded = clockInRounded,
    TimeIn = clockIn.Date, // Midnight
    TimeOut = null, // Pending clock out
    IsHoliday = isHoliday,
    CreatedAt = DateTime.UtcNow
};

await _timeSheetRepository.AddAsync(timeSheet);

// Store GPS location (optional, for verification)
await _timeSheetLocationRepository.AddAsync(new TimeSheetLocation
{
    TimeSheetId = timeSheet.Id,
    Type = LocationType.ClockIn,
    Latitude = latitude,
    Longitude = longitude
});
```

**Response:**
```json
{
  "id": "timesheet-guid",
  "date": "2026-02-01",
  "clockIn": "2026-02-01T07:05:23Z",
  "clockInRounded": "2026-02-01T07:00:00Z",
  "status": "InProgress"
}
```

**UI:** Shows "Clocked In at 7:05 AM" with elapsed time counter

---

### STEP 2: Worker clocks out

**Actor:** Worker (Flutter app)

**Action:** Worker taps "Clock Out" button

**API Call:**
```http
POST /api/WorkerRequest/{requestId}/TimeSheet
{
  "clockOut": "2026-02-01T15:08:12Z",
  "latitude": 43.6532,
  "longitude": -79.3832
}
```

**Backend:**
```csharp
// Find TimeSheet for today
var timeSheet = await _timeSheetRepository
    .GetByWorkerRequestAndDateAsync(workerRequestId, clockOut.Date);
if (timeSheet == null)
    throw new BusinessException("No clock in found for today");

if (timeSheet.ClockOut != null)
    throw new BusinessException("Already clocked out");

// Validate minimum time
var elapsed = clockOut - timeSheet.ClockIn.Value;
if (elapsed.TotalMinutes < 3)
    throw new BusinessException("Must work at least 3 minutes");

// Round clock out time
var clockOutRounded = RoundToNearest15Minutes(clockOut);

// Calculate duration (hours from TimeIn)
var duration = clockOutRounded - timeSheet.ClockInRounded.Value;

// Update TimeSheet
timeSheet.ClockOut = clockOut;
timeSheet.ClockOutRounded = clockOutRounded;
timeSheet.TimeOut = timeSheet.TimeIn.Add(duration);

await _timeSheetRepository.UpdateAsync(timeSheet);

// Store GPS location
await _timeSheetLocationRepository.AddAsync(new TimeSheetLocation
{
    TimeSheetId = timeSheet.Id,
    Type = LocationType.ClockOut,
    Latitude = latitude,
    Longitude = longitude
});
```

**Response:**
```json
{
  "id": "timesheet-guid",
  "date": "2026-02-01",
  "clockIn": "2026-02-01T07:05:23Z",
  "clockOut": "2026-02-01T15:08:12Z",
  "clockInRounded": "2026-02-01T07:00:00Z",
  "clockOutRounded": "2026-02-01T15:00:00Z",
  "duration": "08:00:00",
  "status": "PendingApproval"
}
```

---

### STEP 3: Agency approves the timesheet

**Actor:** Agency (Web app)

**Agency reviews timesheets:**
```http
GET /api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet?date=2026-02-01
```

**Agency approves:**
```http
PUT /api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet/{timesheetId}/Approve
{
  "timeInApproved": "2026-02-01T07:00:00Z",
  "timeOutApproved": "2026-02-01T15:00:00Z"
}
```

**Backend:**
```csharp
// Validations
if (timeInApproved.Date != timeOutApproved.Date)
    throw new ValidationException("Times must be same date");

if (timeInApproved > timeOutApproved)
    throw new ValidationException("Time in must be before time out");

// Update TimeSheet
timeSheet.TimeInApproved = timeInApproved;
timeSheet.TimeOutApproved = timeOutApproved;
await _timeSheetRepository.UpdateAsync(timeSheet);

// Calculate TimeSheetTotal
await _timeSheetTotalService.CalculateAsync(timeSheet.Id);

// Notify worker
await _pushNotificationService.SendAsync(
    userId: timeSheet.WorkerRequest.WorkerProfile.UserId,
    title: "Timesheet Approved",
    body: $"Your timesheet for {timeSheet.Date:MMM dd} has been approved",
    data: { type: "TimesheetApproved", timesheetId: timeSheet.Id }
);
```

---

### STEP 4: System calculates TimeSheetTotal

**Actor:** System (automatic after approval)

**See:** `.docs/business/TIMESHEET_RULES.md` for the detailed calculation logic

**Summary:**
```csharp
var total = new TimeSheetTotal
{
    TimeSheetId = timeSheet.Id,
    TotalHours = timeSheet.TimeOutApproved - timeSheet.TimeInApproved,
    AccumulateWeekHours = await CalculateWeekHoursAsync(timeSheet),
    RegularHours = CalculateRegularHours(...),
    OvertimeHours = CalculateOvertimeHours(...),
    NightShiftHours = CalculateNightShiftHours(...),
    HolidayHours = timeSheet.IsHoliday ? TotalHours : TimeSpan.Zero
};

await _timeSheetTotalRepository.AddAsync(total);
```

---

## 4️⃣ Payroll Processing Flow

### Overview
Agency generates pay stubs for the period → System calculates deductions → PDF generated → Worker is notified.

**See:** `.docs/business/PAYROLL_RULES.md` and `.docs/business/PAYSTUB_GENERATION.md` for detailed payroll calculations.

---

### STEP 1: Agency initiates payroll

**Actor:** Agency (Web app)

**Agency selects workers for payroll:**
```
Week ending: Feb 7, 2026
Workers: [Select from list of workers with approved timesheets]
```

**API Call (per worker):**
```http
POST /api/v4/Accounting/PayStub
{
  "workerProfileId": "guid",
  "paymentDate": "2026-02-07",
  "weekEnding": "2026-02-07"
}
```

---

### STEP 2: System calculates earnings

**Backend:**
```csharp
// Get all approved timesheets for the period
var timesheets = await _timeSheetRepository
    .GetForPayrollAsync(workerProfileId, weekEnding);

decimal regularWage = 0;
decimal overtimeWage = 0;
decimal nightShiftWage = 0;
decimal holidayWage = 0;

foreach (var ts in timesheets)
{
    var total = ts.Total;
    var rate = ts.WorkerRequest.Request.WorkerRate;

    regularWage    += (decimal)total.RegularHours.TotalHours    * rate;
    overtimeWage   += (decimal)total.OvertimeHours.TotalHours   * (rate * 1.5m);
    nightShiftWage += (decimal)total.NightShiftHours.TotalHours * (rate * 1.15m);
    holidayWage    += (decimal)total.HolidayHours.TotalHours    * (rate * 1.5m);
}

var grossPayment  = regularWage + overtimeWage + nightShiftWage + holidayWage;
var vacations     = grossPayment * 0.04m; // 4% Canada
var totalEarnings = grossPayment + vacations;
```

---

### STEP 3: System calculates deductions

**Backend:**
```csharp
// Get tax info
var taxCategory = worker.TaxCategory;
var province = worker.Location.City.Province.Code;

// Calculate CPP
var cpp = _cppCalculator.Calculate(
    grossPayment,
    PayFrequency.Weekly);

// Calculate EI
var ei = _eiCalculator.Calculate(
    grossPayment,
    PayFrequency.Weekly);

// Calculate Federal Tax
var federalTax = _federalTaxCalculator.Calculate(
    grossPayment,
    PayFrequency.Weekly,
    taxCategory.FederalCategory);

// Calculate Provincial Tax
var provincialTax = _provincialTaxCalculator.Calculate(
    grossPayment,
    PayFrequency.Weekly,
    province,
    taxCategory.ProvincialCategory);

var totalDeductions = cpp + ei + federalTax + provincialTax;
var totalPaid = totalEarnings - totalDeductions;
```

---

### STEP 4: Create the PayStub entity

**Backend:**
```csharp
// Get next pay stub number
var payStubNumber = await _payStubRepository
    .GetNextPayStubNumberAsync(DateTime.Now.Year);

var payStub = new PayStub
{
    WorkerProfileId = workerProfileId,
    PayStubNumber = payStubNumber,
    PayStubNumberId = DateTime.Now.Year,
    TypeOfWork = "General Labor",
    DateWorkBegins = timesheets.Min(t => t.Date),
    DateWorkEnd = timesheets.Max(t => t.Date),
    PaymentDate = paymentDate,

    RegularWage = regularWage,
    GrossPayment = grossPayment,
    Vacations = vacations,
    PublicHolidayPay = 0,
    TotalEarnings = totalEarnings,

    Cpp = cpp,
    Ei = ei,
    FederalTax = federalTax,
    ProvincialTax = provincialTax,
    OtherDeductions = 0,
    TotalDeductions = totalDeductions,

    TotalPaid = totalPaid,
    CreatedAt = DateTime.UtcNow
};

await _payStubRepository.AddAsync(payStub);
```

---

### STEP 5: Generate PDF

**Backend:**
```csharp
// Generate PDF
var pdf = await _payStubPdfGenerator.GenerateAsync(payStub.Id);

// Upload to Azure Storage
var url = await _azureStorageService.UploadAsync(
    container: "paystubs",
    file: pdf,
    fileName: $"PS-{payStub.PayStubNumber:0000}-{payStub.PayStubNumberId:00}.pdf"
);

payStub.PdfUrl = url;
await _payStubRepository.UpdateAsync(payStub);
```

---

### STEP 6: Notify the worker

**Backend:**
```csharp
// Email with PDF attached
await _emailService.SendAsync(
    to: worker.User.Email,
    subject: $"Pay Stub PS-{payStub.PayStubNumber:0000}-{payStub.PayStubNumberId:00}",
    template: "PayStubReady",
    data: new
    {
        WorkerName = $"{worker.FirstName} {worker.LastName}",
        PayStubNumber = $"PS-{payStub.PayStubNumber:0000}-{payStub.PayStubNumberId:00}",
        TotalPaid = payStub.TotalPaid,
        PaymentDate = payStub.PaymentDate
    },
    attachments: new[] { pdf }
);

// Push notification
await _pushNotificationService.SendAsync(
    userId: worker.UserId,
    title: "Pay Stub Available",
    body: $"Your pay stub for week ending {weekEnding:MMM dd} is ready",
    data: { type: "PayStubReady", payStubId: payStub.Id }
);
```

---

## 5️⃣ Invoicing Flow

### Overview
Agency generates an invoice for a company for the period → System calculates totals with markup → PDF generated → Company is notified.

**See:** `.docs/business/BILLING_RULES.md` for detailed billing calculations.

---

### STEP 1: Agency initiates the invoice

**Actor:** Agency (Web app)

**Agency selects:**
- Company
- Week ending date
- Worker requests to include

**API Call:**
```http
POST /api/v4/Accounting/Invoice
{
  "companyProfileId": "guid",
  "weekEnding": "2026-02-07",
  "workerRequestIds": ["guid1", "guid2", "guid3"]
}
```

---

### STEP 2: System calculates invoice totals

**Backend:**
```csharp
var invoiceTotals = new List<InvoiceTotal>();

foreach (var workerRequestId in workerRequestIds)
{
    var timesheets = await _timeSheetRepository
        .GetForInvoiceAsync(workerRequestId, weekEnding);

    var workerRequest = await _workerRequestRepository
        .GetByIdAsync(workerRequestId);

    var rate = workerRequest.Request.AgencyRate; // Company pays this

    decimal regularAmount = 0;
    decimal overtimeAmount = 0;
    decimal nightShiftAmount = 0;
    decimal holidayAmount = 0;
    TimeSpan regularHours = TimeSpan.Zero;
    TimeSpan overtimeHours = TimeSpan.Zero;
    TimeSpan nightShiftHours = TimeSpan.Zero;
    TimeSpan holidayHours = TimeSpan.Zero;

    foreach (var ts in timesheets)
    {
        var total = ts.Total;
        regularHours    += total.RegularHours;
        overtimeHours   += total.OvertimeHours;
        nightShiftHours += total.NightShiftHours;
        holidayHours    += total.HolidayHours;

        regularAmount    += (decimal)total.RegularHours.TotalHours    * rate;
        overtimeAmount   += (decimal)total.OvertimeHours.TotalHours   * (rate * 1.5m);
        nightShiftAmount += (decimal)total.NightShiftHours.TotalHours * (rate * 1.15m);
        holidayAmount    += (decimal)total.HolidayHours.TotalHours    * (rate * 1.5m);
    }

    var invoiceTotal = new InvoiceTotal
    {
        WorkerRequestId = workerRequestId,
        WorkerName = $"{workerRequest.WorkerProfile.FirstName} {workerRequest.WorkerProfile.LastName}",
        RegularHours = regularHours,
        RegularAmount = regularAmount,
        OvertimeHours = overtimeHours,
        OvertimeAmount = overtimeAmount,
        NightShiftHours = nightShiftHours,
        NightShiftAmount = nightShiftAmount,
        HolidayHours = holidayHours,
        HolidayAmount = holidayAmount,
        Total = regularAmount + overtimeAmount + nightShiftAmount + holidayAmount
    };

    invoiceTotals.Add(invoiceTotal);
}

// Calculate totals
var subTotal = invoiceTotals.Sum(t => t.Total);
var vacations = subTotal * 0.04m; // 4%
var hstRate = GetHstRateByProvince(company.BillingAddress.Province);
var hst = (subTotal + vacations) * hstRate;
var totalNet = subTotal + vacations + hst;
```

---

### STEP 3: Create the Invoice entity

**Backend:**
```csharp
// Get next invoice number
var invoiceNumber = await _invoiceRepository
    .GetNextInvoiceNumberAsync(DateTime.Now.Year);

var invoice = new Invoice
{
    CompanyProfileId = companyProfileId,
    InvoiceNumber = invoiceNumber,
    NightShiftRate = 1.15m,
    HolidayRate = 1.5m,
    OverTimeRate = 1.5m,
    VacationsRate = 0.04m,
    HstRate = hstRate,
    BonusRate = 0,
    SubTotal = subTotal,
    Hst = hst,
    TotalNet = totalNet,
    CreatedAt = DateTime.UtcNow
};

await _invoiceRepository.AddAsync(invoice);

// Add invoice totals
foreach (var total in invoiceTotals)
{
    total.InvoiceId = invoice.Id;
    await _invoiceTotalRepository.AddAsync(total);
}
```

---

### STEP 4: Generate PDF and send

**Backend:**
```csharp
// Generate PDF
var pdf = await _invoicePdfGenerator.GenerateAsync(invoice.Id);

// Upload to Azure Storage
var url = await _azureStorageService.UploadAsync(
    container: "invoices",
    file: pdf,
    fileName: $"AI-{invoice.InvoiceNumber:0000}-{DateTime.Now:yy}.pdf"
);

invoice.PdfUrl = url;
await _invoiceRepository.UpdateAsync(invoice);

// Get recipients
var recipients = await _companyProfileInvoiceRecipientRepository
    .GetByCompanyAsync(companyProfileId);

// Send email to all recipients
foreach (var recipient in recipients)
{
    await _emailService.SendAsync(
        to: recipient.Email,
        subject: $"Invoice AI-{invoice.InvoiceNumber:0000}-{DateTime.Now:yy}",
        template: "InvoiceReady",
        data: new
        {
            CompanyName = company.BusinessName,
            InvoiceNumber = $"AI-{invoice.InvoiceNumber:0000}-{DateTime.Now:yy}",
            TotalNet = invoice.TotalNet,
            DueDate = invoice.CreatedAt.AddDays(30)
        },
        attachments: new[] { pdf }
    );
}
```

---

## 🎯 Key Takeaways

### Async Processing

Many operations can be done asynchronously:
- Document generation (PDF)
- File uploads (Azure Storage)
- Notifications (email, push, Teams)
- Use Azure Service Bus for heavy operations

### Error Handling

All workflows should handle:
- Validation errors → 400 Bad Request
- Authorization errors → 403 Forbidden
- Business rule violations → 409 Conflict
- System errors → 500 Internal Server Error

### Audit Trail

Log all critical operations:
- Worker registration
- Approval/rejection
- Timesheet creation/approval
- PayStub generation
- Invoice generation

### Notifications

Keep all parties informed:
- Workers: Profile approved, Job assigned, Timesheet approved, Pay stub ready
- Companies: Request filled, Invoice ready
- Agency: New registrations, New requests, Unusual activity

### Idempotency

Ensure operations are idempotent where possible:
- Use unique constraints (prevent duplicate timesheets)
- Check existing records before creating
- Use sequential numbering (PayStub, Invoice)
