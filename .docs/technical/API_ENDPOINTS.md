# API Endpoints - Covenant/Sigook Platform

## 🎯 Base URL

**Staging:** `https://sigook-api-staging.azurewebsites.net/api`
**Production:** `https://sigook-api.azurewebsites.net/api`

> Routes are declared via `RouteName` constants on each controller. Folders live under `Covenant.Api/{Module}Module/{Resource}/Controllers/`. The list below shows the actual route templates used in code, not theoretical paths.

---

## 🔐 Authentication

All endpoints require authentication via **Bearer token** (except registration and public endpoints).

```http
Authorization: Bearer {access_token}
```

**Get a token:**
- Via IdentityServer4 (OpenID Connect)
- Staging: `https://sigook-accounts-staging.azurewebsites.net`
- Production: `https://sigook-accounts.azurewebsites.net`

---

## 🏢 AGENCY MODULE

### Agency Requests Management

#### List Agency Requests
```http
GET /api/AgencyRequest
```

**Query Parameters:**
- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 20)
- `status` (RequestStatus): Filter by status (`Open`, `Filled`, `Cancelled`)
- `companyProfileId` (guid): Filter by company
- `searchTerm` (string): Search in job title/description

**Response:**
```json
{
  "items": [
    {
      "id": "guid",
      "jobTitle": "Warehouse Worker",
      "companyName": "ABC Logistics",
      "workersQuantity": 5,
      "workersQuantityWorking": 2,
      "status": "Open",
      "startAt": "2026-02-01T00:00:00Z",
      "location": "Toronto, ON"
    }
  ],
  "totalCount": 47,
  "pageNumber": 1,
  "pageSize": 20
}
```

---

#### Get Request Detail
```http
GET /api/AgencyRequest/{id}
```

**Response:**
```json
{
  "id": "guid",
  "jobTitle": "Warehouse Worker",
  "description": "Full job description...",
  "requirements": "Requirements...",
  "workersQuantity": 5,
  "workersQuantityWorking": 2,
  "status": "Open",
  "startAt": "2026-02-01T00:00:00Z",
  "finishAt": null,
  "durationTerm": "LongTerm",
  "employmentType": "FullTime",
  "workerRate": 18.50,
  "agencyRate": 25.00,
  "shift": {
    "start": "07:00:00",
    "end": "15:00:00",
    "break": "00:30:00"
  },
  "jobLocation": {
    "fullAddress": "123 Main St, Toronto, ON M1A 1A1",
    "city": "Toronto",
    "province": "Ontario"
  },
  "company": {
    "id": "guid",
    "businessName": "ABC Logistics"
  },
  "assignedWorkers": [
    {
      "id": "guid",
      "firstName": "John",
      "lastName": "Doe",
      "status": "Booked",
      "startWorking": "2026-02-01T00:00:00Z"
    }
  ]
}
```

---

#### Create Request
```http
POST /api/AgencyRequest
```

**Request Body:**
```json
{
  "companyProfileId": "guid",
  "jobTitle": "Warehouse Worker",
  "description": "Job description...",
  "requirements": "Requirements...",
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

**Response:** `201 Created` with the Request object

---

#### Update Request
```http
PUT /api/AgencyRequest/{id}
```

**Request Body:** Same as Create.

---

#### Cancel Request
```http
PUT /api/AgencyRequest/{id}/Cancel
```

Only valid when the request is `Open` and has zero booked workers.

**Response:** `204 No Content`

---

#### Reopen Request
```http
PUT /api/AgencyRequest/{id}/Open
```

**Response:** `204 No Content`

---

#### Send Invitation to Workers
```http
POST /api/AgencyRequest/{id}/SendInvitation
```

**Request Body:**
```json
{
  "workerIds": ["guid1", "guid2", "guid3"]
}
```

**Response:** `200 OK` — Push notifications sent

---

### Agency Worker Management

#### List Workers
```http
GET /api/AgencyWorkerProfile
```

**Query Parameters:**
- `page`, `pageSize`
- `approvedToWork` (bool)
- `dnu` (bool)
- `searchTerm` (string)
- `skillIds` (guid[]): Filter by skills
- `cityId` (guid): Filter by location

**Response:**
```json
{
  "items": [
    {
      "id": "guid",
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      "mobileNumber": "+1-416-555-0100",
      "approvedToWork": true,
      "dnu": false,
      "city": "Toronto",
      "skills": ["Forklift", "Warehouse"],
      "hasVehicle": true
    }
  ],
  "totalCount": 150
}
```

---

#### Get Worker Detail
```http
GET /api/AgencyWorkerProfile/{id}
```

**Response:** Complete WorkerProfile with all sections.

---

#### Approve Worker to Work
```http
PUT /api/AgencyWorkerProfile/{id}/ApproveToWork
```

**Request Body:**
```json
{ "approvedToWork": true }
```

**Response:** `204 No Content`

---

#### Mark Worker as Do Not Use
```http
PUT /api/AgencyWorkerProfile/{id}/Dnu
```

**Request Body:**
```json
{
  "dnu": true,
  "reason": "Failed background check"
}
```

---

### Agency Worker Assignment

Worker assignments are nested under the request: `api/AgencyRequest/{requestId}/Worker`.

#### Assign Worker to Request
```http
POST /api/AgencyRequest/{requestId}/Worker
```

**Request Body:**
```json
{
  "workerProfileId": "guid",
  "startWorking": "2026-02-01T00:00:00Z"
}
```

**Response:** `201 Created` with the WorkerRequest object.

---

#### Remove Worker from Request
```http
DELETE /api/AgencyRequest/{requestId}/Worker/{workerRequestId}
```

**Response:** `204 No Content`

---

### Agency Timesheet Management

Timesheets are nested under the request worker:
`api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet`.

#### Create Timesheet (Manual)
```http
POST /api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet
```

**Request Body:**
```json
{
  "date": "2026-02-01",
  "hours": 8.5,
  "isHoliday": false,
  "comment": "Manual entry"
}
```

**Response:** `201 Created` with the TimeSheet object (pre-approved).

---

#### Approve Timesheet
```http
PUT /api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet/{id}/Approve
```

**Request Body:**
```json
{
  "timeInApproved": "2026-02-01T07:00:00Z",
  "timeOutApproved": "2026-02-01T15:00:00Z"
}
```

**Response:** `204 No Content`

---

### Agency Company Management

#### List Companies
```http
GET /api/AgencyCompanyProfile
```

(There is also a `v2` route: `GET /api/v2/AgencyCompanyProfile`.)

**Query Parameters:**
- `page`, `pageSize`
- `status` (CompanyStatus)
- `searchTerm` (string)

**Response:**
```json
{
  "items": [
    {
      "id": "guid",
      "businessName": "ABC Logistics",
      "status": "Client",
      "email": "contact@abclogistics.com",
      "phoneNumber": "+1-416-555-0200",
      "locationsCount": 3,
      "activeRequestsCount": 5
    }
  ],
  "totalCount": 25
}
```

---

#### Get Company Detail
```http
GET /api/AgencyCompanyProfile/{id}
```

**Response:** Complete CompanyProfile with locations, rates, contacts.

---

#### Create Company
```http
POST /api/AgencyCompanyProfile
```

**Request Body:**
```json
{
  "businessName": "ABC Logistics",
  "dbaName": "ABC",
  "businessNumber": "123456789",
  "hstNumber": "987654321RT0001",
  "email": "contact@abclogistics.com",
  "phoneNumber": "+1-416-555-0200",
  "website": "https://abclogistics.com"
}
```

---

#### Update Company
```http
PUT /api/AgencyCompanyProfile/{id}
```

---

### Agency Candidate Management

#### List Candidates
```http
GET /api/AgencyCandidate
```

**Response:** List of Candidates (pre-workers without a User).

---

#### Create Candidate
```http
POST /api/AgencyCandidate
```

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane@example.com",
  "mobileNumber": "+1-416-555-0300",
  "notes": "Met at job fair"
}
```

Candidate sub-resources (notes, documents, skills, phone numbers) live under
`api/AgencyCandidate/{candidateId}/{Resource}`.

---

## 🏢 COMPANY MODULE

### Company Requests

#### List My Requests
```http
GET /api/CompanyRequest
```

**Query Parameters:**
- `page`, `pageSize`
- `status` (RequestStatus)

**Response:** Similar to the Agency request list, scoped to the authenticated company.

---

#### Create Request
```http
POST /api/CompanyRequest
```

**Request Body:** Same as the Agency Create Request.

---

#### Get Request Detail
```http
GET /api/CompanyRequest/{id}
```

---

#### Update Request
```http
PUT /api/CompanyRequest/{id}
```

---

### Company Profile

#### Get My Profile
```http
GET /api/CompanyProfile
```

#### Update Profile
```http
PUT /api/CompanyProfile
```

---

### Company Locations

```http
GET  /api/CompanyLocation
POST /api/CompanyLocation
```

**Create body:**
```json
{
  "name": "Toronto Warehouse",
  "locationId": "guid"
}
```

---

### Company Job Positions

```http
GET  /api/CompanyJobPosition
POST /api/CompanyJobPosition
```

**Create body:**
```json
{
  "jobTitle": "Warehouse Worker",
  "workerRate": 18.50,
  "agencyRate": 25.00,
  "overtimeRate": 1.5,
  "nightShiftRate": 1.15,
  "holidayRate": 1.5,
  "currency": "CAD"
}
```

---

### Company Worker View

#### View Assigned Workers
```http
GET /api/CompanyRequest/{requestId}/Worker
```

#### View Timesheets
```http
GET /api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet
```

**Query Parameters:**
- `weekEnding` (date): Filter by week

---

## 👷 WORKER MODULE

### Worker Profile

#### Get My Profiles
```http
GET /api/WorkerProfile
```

**Response:** List of WorkerProfiles for the authenticated user.

---

#### Create Profile (Registration)
```http
POST /api/WorkerProfile
Content-Type: multipart/form-data
```

**Form Data:**
```
registration_form: {JSON object}
profile_photo: {file}
sin_document: {file}
id_document_1: {file}
id_document_2: {file}
```

See `.docs/business/WORKFLOWS.md` for the full registration JSON shape.

**Response:** `201 Created` with the WorkerProfile ID.

---

#### Get Profile Detail
```http
GET /api/WorkerProfile/{id}
```

#### Update Profile sections
```http
PUT /api/WorkerProfile/{profileId}    (and section-specific sub-routes)
```

---

### Worker Job Search

#### List Available Jobs
```http
GET /api/WorkerRequest/Available
```

**Query Parameters:**
- `page`, `pageSize`
- `cityId` (guid)
- `jobTitle` (string)
- `minRate` (decimal)

---

#### Apply to Job
```http
POST /api/WorkerRequest/Apply
```

**Request Body:**
```json
{ "requestId": "guid" }
```

**Response:** `201 Created` — WorkerRequest created with `Status = Booked`.

---

#### List My Current Jobs
```http
GET /api/WorkerRequest/MyRequests
```

---

### Worker Timesheets

The worker timesheet controller is mounted at
`api/WorkerRequest/{requestId}/TimeSheet`.

#### Clock In / Clock Out
```http
POST /api/WorkerRequest/{requestId}/TimeSheet
```

**Request Body (clock in):**
```json
{
  "clockIn": "2026-02-01T07:05:23Z",
  "isHoliday": false
}
```

**Request Body (clock out):**
```json
{
  "clockOut": "2026-02-01T15:08:12Z"
}
```

#### Get My Timesheets
```http
GET /api/WorkerRequest/{requestId}/TimeSheet
```

**Query Parameters:**
- `startDate` (date)
- `endDate` (date)

**Response:**
```json
{
  "items": [
    {
      "id": "guid",
      "date": "2026-02-01",
      "clockIn": "2026-02-01T07:05:23Z",
      "clockOut": "2026-02-01T15:08:12Z",
      "timeInApproved": "2026-02-01T07:00:00Z",
      "timeOutApproved": "2026-02-01T15:00:00Z",
      "isHoliday": false,
      "totalHours": "08:00:00",
      "regularHours": "08:00:00",
      "overtimeHours": "00:00:00"
    }
  ]
}
```

---

## 💰 ACCOUNTING MODULE

### PayStub Management

Routes live under `api/v4/Accounting/PayStub`.

#### Create PayStub
```http
POST /api/v4/Accounting/PayStub
```

**Request Body:**
```json
{
  "workerProfileId": "guid",
  "paymentDate": "2026-02-07",
  "weekEnding": "2026-02-07"
}
```

**Response:** `201 Created` — PayStub object.

---

#### Get PayStub Detail
```http
GET /api/v4/Accounting/PayStub/{id}
```

**Response:**
```json
{
  "id": "guid",
  "payStubNumber": "PS-0001-26",
  "workerName": "John Doe",
  "dateWorkBegins": "2026-02-01",
  "dateWorkEnd": "2026-02-07",
  "paymentDate": "2026-02-07",
  "earnings": {
    "regularWage": 1000.00,
    "grossPayment": 1040.00,
    "vacations": 41.60,
    "publicHolidayPay": 0,
    "totalEarnings": 1081.60
  },
  "deductions": {
    "cpp": 59.50,
    "ei": 16.60,
    "federalTax": 120.50,
    "provincialTax": 45.20,
    "totalDeductions": 241.80
  },
  "totalPaid": 839.80
}
```

---

#### Delete PayStub
```http
DELETE /api/v4/Accounting/PayStub/{id}
```

**Response:** `204 No Content`

---

#### Download PayStub PDF
```http
GET /api/v4/Accounting/PayStub/{payStubId}/Document
```

**Response:** PDF file stream.

---

### Invoice Management

Routes live under `api/v4/Accounting/Invoice`.

#### Create Invoice
```http
POST /api/v4/Accounting/Invoice
```

**Request Body:**
```json
{
  "companyProfileId": "guid",
  "weekEnding": "2026-02-07",
  "workerRequestIds": ["guid1", "guid2", "guid3"]
}
```

**Response:** `201 Created` — Invoice object.

---

#### List Invoices
```http
GET /api/v4/Accounting/Invoice
```

**Query Parameters:**
- `page`, `pageSize`
- `companyProfileId` (guid)
- `startDate`, `endDate` (date)

---

#### Get Invoice Detail
```http
GET /api/v4/Accounting/Invoice/{id}
```

**Response:**
```json
{
  "id": "guid",
  "invoiceNumber": "AI-0001-26",
  "companyName": "ABC Logistics",
  "weekEnding": "2026-02-07",
  "rates": {
    "nightShiftRate": 1.0,
    "holidayRate": 1.5,
    "overtimeRate": 1.5,
    "vacationsRate": 0.04,
    "hstRate": 0.13
  },
  "totals": [
    {
      "workerName": "John Doe",
      "regularHours": "40:00:00",
      "regularAmount": 1000.00,
      "overtimeHours": "04:00:00",
      "overtimeAmount": 150.00,
      "nightShiftHours": "00:00:00",
      "nightShiftAmount": 0,
      "holidayHours": "00:00:00",
      "holidayAmount": 0,
      "total": 1150.00
    }
  ],
  "subTotal": 4500.00,
  "vacations": 180.00,
  "hst": 608.40,
  "totalNet": 5288.40
}
```

---

#### Download Invoice PDF
```http
GET /api/v4/Accounting/Invoice/{invoiceId}/Document
```

**Response:** PDF file stream.

---

#### List PayStubs included in an Invoice
```http
GET /api/v4/Accounting/Invoice/{invoiceId}/PayStub
```

---

### Deductions Calculators

Direct calculator endpoints:

```http
api/Accounting/Deduction/Cpp
api/Accounting/Deduction/FederalTax
api/Accounting/Deduction/ProvincialTax
```

---

## 📚 CATALOG / SHARED Endpoints

### Catalogs

```http
GET /api/Catalog/Countries
GET /api/Catalog/Provinces/{countryId}
GET /api/Catalog/Cities/{provinceId}
GET /api/Catalog/JobPositions
GET /api/Catalog/Skills
GET /api/Catalog/Languages
GET /api/Catalog/Genders
GET /api/Catalog/IdentificationTypes
```

**Response format:**
```json
{
  "items": [
    { "id": "guid", "name": "Item Name" }
  ]
}
```

---

### Location

#### Get Location Detail
```http
GET /api/Location/{id}
```

#### Create Location
```http
POST /api/Location
```

**Request Body:**
```json
{
  "streetNumber": "123",
  "streetName": "Main St",
  "unit": "Suite 100",
  "cityId": "guid",
  "postalCode": "M1A 1A1"
}
```

**Response:** `201 Created` — Location with geocoded lat/lng.

---

## ⚠️ Error Responses

### Standard Error Format

```json
{
  "error": {
    "code": "ValidationError",
    "message": "Validation failed",
    "details": [
      {
        "field": "workerRate",
        "message": "Worker rate must be greater than 0"
      }
    ]
  }
}
```

### HTTP Status Codes

- `200 OK` — Success
- `201 Created` — Resource created
- `204 No Content` — Success with no response body
- `400 Bad Request` — Validation error
- `401 Unauthorized` — Authentication required
- `403 Forbidden` — Insufficient permissions
- `404 Not Found` — Resource not found
- `409 Conflict` — Business rule violation
- `500 Internal Server Error` — Server error
