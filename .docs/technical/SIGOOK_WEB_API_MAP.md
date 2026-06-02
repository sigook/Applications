# Sigook.Web API Map

This document maps every API file in `src/api/*.ts` to its backend endpoints, request/response types, and Pinia store integrations.

**Key Patterns:**
- API functions are plain TypeScript functions (NOT store dispatches)
- All HTTP calls use the `http` instance from `@/security/apiService`
- Request/response types live in `src/types/*.ts`
- Pinia stores in `src/store/modules/` occasionally wrap state for filters only (not direct API calls)
- Backend is Covenant.Api (.NET 8) with REST conventions
- Core actors: Agency, Company, Worker, Candidate

---

## 1. accountApi.ts

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `changeEmail()` | POST | `/api/Account/ChangeEmail` | `ChangeEmailRequest` | `void` | Account/security ops |
| `getEmail()` | GET | `/api/Account/GetEmail` | — | `GetEmailResponse` | Fetch current email |
| `deactivateAccount()` | PATCH | `/identity` | — | `void` | Account deactivation |

**Types:** `ChangeEmailRequest`, `GetEmailResponse` (from `src/types/security`)

**Pinia:** None

**Usage:** Account management pages; email change workflows

---

## 2. agencyApi.ts

Core agency profile and personnel management.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyProfile()` | GET | `/api/Agency/Profile` | — | `AgencyDetail` | Current logged-in agency |
| `getAgency(id)` | GET | `/api/Agency/{id}` | — | `AgencyDetail` | Fetch single agency |
| `getAgenciesList(filter)` | GET | `/api/Agency` | `AgencyListFilter` (params) | `PaginatedList<AgencyListItem>` | Paginated list with filtering |
| `createAgency(model)` | POST | `/api/Agency` | `CreateAgencyModel` | `{ id: string }` | Create new agency |
| `updateAgency(agency)` | PUT | `/api/Agency` | `AgencyDetail` | `void` | Update agency profile |
| `getAgencyPersonnel()` | GET | `/api/AgencyPersonnel` | — | `AgencyPersonnelListItem[]` | Users in agency back-office |
| `createAgencyPersonnel(model)` | POST | `/api/AgencyPersonnel` | `AgencyPersonnelCreateModel` | `void` | Add personnel user |
| `deleteAgencyPersonnel(id)` | DELETE | `/api/AgencyPersonnel/{id}` | — | `void` | Remove personnel user |
| `getAgencyLocations()` | GET | `/api/Agency/Location` | — | `AgencyLocationDetail[]` | Billing/office locations |
| `createAgencyLocation(model)` | POST | `/api/Agency/Location` | `AgencyLocationDetail` | `{ id: string }` | Add location |
| `updateAgencyLocation(id, model)` | PUT | `/api/Agency/Location/{id}` | `AgencyLocationDetail` | `void` | Update location |
| `deleteAgencyLocation(id)` | DELETE | `/api/Agency/Location/{id}` | — | `void` | Remove location |
| `getPersonnelAgencies()` | GET | `/api/PersonnelAgency` | — | `PersonnelAgencyItem[]` | Agencies user has access to |
| `switchPersonnelAgency(id)` | PUT | `/api/PersonnelAgency/{id}` | — | `void` | Switch active agency context |

**Types:** `AgencyDetail`, `AgencyListFilter`, `AgencyListItem`, `AgencyLocationDetail`, `AgencyPersonnelCreateModel`, `AgencyPersonnelListItem`, `CreateAgencyModel`, `PersonnelAgencyItem` (from `src/types/agency`)

**Pinia:** 
- Store `agency` holds `AgencyProfile` and `agencyListFilter`
- Used in: Agencies page, Agency profile, Personnel management

**Business Logic:**
- Agency personnel are distinct from Worker/Candidate; they manage the back-office
- Each agency can have multiple locations for billing/operations
- Personnel switching allows multi-agency workflows

---

## 3. agencyCandidateApi.ts

Candidate management (recruitment pool before conversion to Worker).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| **Candidates CRUD** | | | | | |
| `getAgencyCandidates(filter)` | GET | `/api/AgencyCandidate` | `AgencyCandidateFilter` (params) | `PaginatedList<Candidate>` | List candidates |
| `getAgencyCandidate(id)` | GET | `/api/AgencyCandidate/{id}` | — | `Candidate` | Fetch candidate details |
| `createAgencyCandidate(model)` | POST | `/api/AgencyCandidate` | `CreateCandidateModel` | `{ id: string }` | Add candidate to pool |
| `updateAgencyCandidate(id, model)` | PUT | `/api/AgencyCandidate/{id}` | `CreateCandidateModel` | `void` | Update candidate info |
| `deleteAgencyCandidate(id)` | DELETE | `/api/AgencyCandidate/{id}` | — | `void` | Remove candidate |
| `updateAgencyCandidateRecruiter(id)` | PUT | `/api/AgencyCandidate/{id}/Recruiter` | null | `void` | Assign recruiter |
| `convertCandidateToWorker(id)` | POST | `/api/AgencyCandidate/{id}/convert-to-worker` | — | `{ id: string }` | Promote candidate to worker |
| **Phone Numbers** | | | | | |
| `addCandidatePhoneNumber(id, model)` | POST | `/api/AgencyCandidate/{id}/PhoneNumber` | `CandidatePhoneNumberModel` | `{ id: string }` | Add phone |
| `deleteCandidatePhoneNumber(id, numberId)` | DELETE | `/api/AgencyCandidate/{id}/PhoneNumber/{numberId}` | — | `void` | Remove phone |
| **Skills** | | | | | |
| `addCandidateSkill(id, model)` | POST | `/api/AgencyCandidate/{id}/Skill` | `CandidateSkillModel` | `{ id: string }` | Add skill |
| `deleteCandidateSkill(id, skillId)` | DELETE | `/api/AgencyCandidate/{id}/Skill/{skillId}` | — | `void` | Remove skill |
| **Documents** | | | | | |
| `getCandidateDocuments(id)` | GET | `/api/AgencyCandidate/{id}/Document` | — | `PaginatedList<CandidateDocument>` | Fetch docs (resume, certs) |
| `addCandidateDocument(id, model)` | POST | `/api/AgencyCandidate/{id}/Document` | `CreateCandidateDocumentPayload` | `CandidateDocument` | Upload document |
| `deleteCandidateDocument(id, docId)` | DELETE | `/api/AgencyCandidate/{id}/Document/{id}` | — | `void` | Remove document |
| **Bulk Operations** | | | | | |
| `bulkAgencyCandidates(agencyId, file)` | POST | `/api/AgencyCandidate/bulk/{agencyId}` | FormData (multipart file) | Blob | Excel import → error report |

**Types:** `Candidate`, `CandidateDocument`, `CreateCandidateDocumentPayload`, `AgencyCandidateFilter`, `CreateCandidateModel`, `CandidatePhoneNumberModel`, `CandidateSkillModel` (from `src/types/candidate`)

**Pinia:**
- Filter stored: `agencyCandidateFilter` in `agency` module
- Used in: Candidates page

**Business Logic:**
- Candidates are applicants in recruitment funnel
- `convertCandidateToWorker()` moves person from candidate pool to worker roster
- Candidates can have skills, documents, phone numbers tracked
- Bulk upload parses Excel for batch candidate import

---

## 4. agencyCompanyApi.ts

**Largest API file.** Agency-side management of all company profiles (clients).

### Company CRUD
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createAgencyCompany(company)` | POST | `/api/v2/AgencyCompanyProfile` | `Partial<CompanyProfileDetail>` | `{ id: string }` | Create company |
| `getAgencyCompanies(filter)` | GET | `/api/v2/AgencyCompanyProfile` | `AgencyCompanyFilter` (params) | `PaginatedList<AgencyCompanyListItem>` | List companies |
| `getAgencyCompany(id)` | GET | `/api/v2/AgencyCompanyProfile/{id}` | — | `CompanyProfileDetail` | Fetch company details |
| `updateAgencyCompany(id, company)` | PUT | `/api/v2/AgencyCompanyProfile/{id}` | `Partial<CompanyProfileDetail>` | `void` | Update company |
| `updateCompanyVaccinationRequired(id, model)` | PUT | `/api/v2/AgencyCompanyProfile/{id}/VaccinationRequired` | `VaccinationRequiredModel` | `void` | Vaccination policy |
| `updateAgencyCompanyEmail(id, model)` | PUT | `/api/v2/AgencyCompanyProfile/{id}/Email` | `{ newEmail: string }` | `void` | Update email |
| `updateAgencyCompanyProfileLogo(id, model)` | PUT | `/api/AgencyCompanyProfile/{id}/Logo` | `Partial<CovenantFileModel>` | `void` | Logo upload |
| `getAgencyCompanyProfileWithRequests()` | GET | `/api/v2/AgencyCompanyProfile/company-with-requests` | — | `CompanyProfileListItem[]` | Companies + their requests |
| `bulkAgencyCompanies(agencyId, file)` | POST | `/api/v2/AgencyCompanyProfile/bulk/{agencyId}` | FormData (multipart file) | Blob | Excel import |

### Contact Persons
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyContactPerson(id)` | GET | `/api/AgencyCompanyProfile/{id}/ContactPerson` | — | `AgencyCompanyContactPerson[]` | Hiring contacts at company |
| `createAgencyCompanyContactPerson(id, model)` | POST | `/api/AgencyCompanyProfile/{id}/ContactPerson` | `AgencyCompanyContactPerson` | `{ id: string }` | Add contact |
| `updateAgencyCompanyContactPerson(id, personId, model)` | PUT | `/api/AgencyCompanyProfile/{id}/ContactPerson/{personId}` | `AgencyCompanyContactPerson` | `void` | Update contact |
| `deleteAgencyCompanyContactPerson(id, personId)` | DELETE | `/api/AgencyCompanyProfile/{id}/ContactPerson/{personId}` | — | `void` | Remove contact |

### Locations
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyLocation(id)` | GET | `/api/AgencyCompanyProfile/{id}/Location` | — | `AgencyCompanyLocationModel[]` | Company sites/offices |
| `createAgencyCompanyLocation(id, model)` | POST | `/api/AgencyCompanyProfile/{id}/Location` | `AgencyCompanyLocationModel` | `{ id: string }` | Add location |
| `updateAgencyCompanyLocation(id, locId, model)` | PUT | `/api/AgencyCompanyProfile/{id}/Location/{locId}` | `AgencyCompanyLocationModel` | `void` | Update location |
| `deleteAgencyCompanyLocation(id, locId)` | DELETE | `/api/AgencyCompanyProfile/{id}/Location/{locId}` | — | `void` | Remove location |
| `updateAgencyCompanyContactInformation(id, model)` | PUT | `/api/AgencyCompanyProfile/{id}/ContactInformation` | `Partial<CompanyProfileDetail>` | `void` | Update contact info |

### Job Positions
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyJobPositions(id)` | GET | `/api/AgencyCompanyProfile/{id}/JobPosition` | — | `AgencyCompanyJobPosition[]` | All roles available at company |
| `getAgencyCompanyJobPositionById(profileId, id)` | GET | `/api/AgencyCompanyProfile/{profileId}/JobPosition/{id}` | — | `AgencyCompanyJobPosition` | Single job position |
| `createAgencyCompanyJobPosition(id, model)` | POST | `/api/AgencyCompanyProfile/{id}/JobPosition` | `AgencyCompanyJobPosition` | `{ id: string }` | Define new position/rate |
| `updateAgencyCompanyJobPosition(id, posId, model)` | PUT | `/api/AgencyCompanyProfile/{id}/JobPosition/{id}` | `AgencyCompanyJobPosition` | `void` | Update position |
| `deleteAgencyCompanyJobPosition(id, posId)` | DELETE | `/api/AgencyCompanyProfile/{id}/JobPosition/{id}` | — | `void` | Remove position |
| `petitionAgencyCompanyJobPosition(id, model)` | POST | `/api/AgencyCompanyProfile/{id}/JobPosition/Petition` | `PetitionJobPositionPayload` | `void` | Request new position type |
| `deleteAgencyJobPosition(companyId, posId)` | DELETE | `/api/AgencyJobPosition/{companyId}/{posId}` | — | `void` | Alternate delete path |

### Job Position Documents
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyJobPositionDocuments(profileId, posId, pagination)` | GET | `/api/AgencyCompanyProfile/{profileId}/JobPosition/{posId}/Document?PageSize={size}&PageIndex={page}` | — | `PaginatedList<CompanyProfileDocumentModel>` | Position-specific docs |
| `createAgencyCompanyJobPositionDocuments(profileId, posId, model)` | POST | `/api/AgencyCompanyProfile/{profileId}/JobPosition/{posId}/Document` | `CompanyProfileDocumentModel` | `{ id: string }` | Add position doc |
| `deleteAgencyCompanyJobPositionDocuments(profileId, posId, id)` | DELETE | `/api/AgencyCompanyProfile/{profileId}/JobPosition/{posId}/Document/{id}` | — | `void` | Remove position doc |

### Company Documents
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyDocument(id, pagination)` | GET | `/api/AgencyCompanyProfile/{id}/Document?PageSize={size}&PageIndex={page}` | — | `PaginatedList<CompanyProfileDocumentModel>` | Company-level docs |
| `createAgencyCompanyDocument(id, model)` | POST | `/api/AgencyCompanyProfile/{id}/Document` | `CompanyProfileDocumentModel` | `{ id: string; pathFile: string }` | Upload company doc |
| `deleteAgencyCompanyDocument(id, docId)` | DELETE | `/api/AgencyCompanyProfile/{id}/Document/{id}` | — | `void` | Remove document |

### Invoice Notes & Recipients
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getInvoiceNotes(id)` | GET | `/api/CompanyProfile/{id}/InvoiceNotes` | — | `InvoiceNotesModel` | Company invoice footer text |
| `postInvoiceNotes(id, model)` | PUT | `/api/CompanyProfile/{id}/InvoiceNotes` | `InvoiceNotesModel` | `void` | Update invoice notes |
| `getCompanyInvoiceRecipients(id)` | GET | `/api/CompanyProfile/{id}/InvoiceRecipient` | — | `InvoiceRecipientModel[]` | Email recipients for invoices |
| `postCompanyInvoiceRecipient(id, model)` | POST | `/api/CompanyProfile/{id}/InvoiceRecipient` | `InvoiceRecipientModel` | `{ id: string }` | Add invoice recipient |
| `deleteCompanyInvoiceRecipient(id, recipId)` | DELETE | `/api/CompanyProfile/{id}/InvoiceRecipient/{id}` | — | `void` | Remove recipient |
| `updateCompanyInvoiceRecipient(id, recipId, model)` | PUT | `/api/CompanyProfile/{id}/InvoiceRecipient/{id}` | `InvoiceRecipientModel` | `void` | Update recipient |

### Company Settings
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `updatePermissionToSeeRequests(id, settings)` | PATCH | `/api/V2/AgencyCompanyProfile/{id}/RequiresPermissionToSeeRequests` | `CompanyProfileSettingsUpdate` | `void` | Restrict request visibility |
| `updatePaidHolidays(id, settings)` | PATCH | `/api/V2/AgencyCompanyProfile/{id}/PaidHolidays` | `CompanyProfileSettingsUpdate` | `void` | Enable/disable paid holidays |
| `updateOvertime(id, settings)` | PATCH | `/api/V2/AgencyCompanyProfile/{id}/Overtime` | `CompanyProfileSettingsUpdate` | `void` | Overtime tracking setting |

### Company Users (Agency-Managed)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyUsers(id)` | GET | `/api/V2/AgencyCompanyProfile/{id}/CompanyUsers` | — | `CompanyUserModel[]` | Users at this company |
| `getCompanyProfileUsers(id)` | GET | `/api/agency-company-profile-user/{id}` | — | `CompanyUserModel[]` | Alternative endpoint |
| `createCompanyProfileUser(id, user)` | POST | `/api/agency-company-profile-user/{id}` | `CreateCompanyUserModel` | `{ id: string }` | Create company user |
| `deleteCompanyProfileUser(id, userId)` | DELETE | `/api/agency-company-profile-user/{id}/users/{userId}` | — | `void` | Remove company user |

### Cross-Cutting
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `updateIsAsapRequests(model)` | PUT | `/api/AgencyRequest/is-asap` | `UpdateIsAsapRequestsPayload` | `void` | Mark requests as ASAP |

**Types:** `AgencyCompanyFilter`, `AgencyCompanyListItem`, `AgencyCompanyContactPerson`, `AgencyCompanyLocationModel`, `AgencyCompanyJobPosition`, `VaccinationRequiredModel`, `InvoiceNotesModel`, `InvoiceRecipientModel`, `PetitionJobPositionPayload`, `UpdateIsAsapRequestsPayload` (from `src/types/agency`); `CompanyProfileDetail`, `CompanyProfileDocumentModel`, `CompanyProfileListItem`, `CompanyProfileSettingsUpdate`, `CompanyUserModel`, `CreateCompanyUserModel` (from `src/types/company`)

**Pinia:**
- Filter stored: `agencyCompanyProfileFilter` in `agency` module
- Used in: Companies page, Company detail, Job position management

**Business Logic:**
- Companies have multiple locations where workers are deployed
- Job positions define roles + wage rates; used for staffing requests
- Vaccination requirement is company-wide policy
- Invoice recipients allow agency to auto-CC company contacts when sending invoices

---

## 5. agencyInvoiceApi.ts

Invoicing for agency → company billing.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyInvoices(filter)` | GET | `/api/agency/accounting/Invoices` | `AgencyInvoiceFilter` (params) | `AgencyInvoiceListResponse` | List invoices |
| `previewAgencyInvoice(payload)` | POST | `/api/agency/accounting/Invoices/Preview` | `CreateAgencyInvoiceModel` | `InvoiceSummaryModel` | Preview before creation |
| `createAgencyInvoice(payload)` | POST | `/api/agency/accounting/Invoices` | `CreateAgencyInvoiceModel` | `void` | Generate invoice |
| `deleteAgencyInvoice(payload)` | DELETE | `/api/v4/Accounting/Invoice/{id}` | `DeleteInvoicePayload` (in body) | `void` | Remove invoice |
| `downloadInvoicePdf(id)` | GET | `/api/v4/Accounting/Invoice/{id}/Document/PDF` | — | Blob | PDF download |
| `sendInvoiceVerificationCode(id)` | POST | `/api/v4/Accounting/Invoice/{id}/SendVerificationCode` | — | `void` | 2FA code for verification |
| `getPayStubsByInvoice(id)` | GET | `/api/v4/Accounting/Invoice/{id}/PayStub` | — | `PayStubDeleteWarningItem[]` | Linked pay stubs |
| `sendInvoiceEmail(payload)` | POST | `/api/v4/Accounting/Invoice/{id}/Document/Email` | FormData (multipart) | `void` | Email invoice with attachments |

**Types:** `AgencyInvoiceFilter`, `AgencyInvoiceListResponse`, `InvoiceSummaryModel`, `CreateAgencyInvoiceModel`, `DeleteInvoicePayload`, `PayStubDeleteWarningItem`, `SendInvoiceEmailPayload` (from `src/types/accounting`)

**Pinia:**
- Filter stored: `agencyInvoiceFilter` in `agency` module
- Used in: Invoices page (accounting)

**Business Logic:**
- Invoices aggregate work (timesheets) into billing
- Preview allows confirmation before actual generation
- Invoices reference paystubs; deletion blocked if paystubs exist
- Email sends with CC recipients stored in CompanyApi

---

## 6. agencyNoteApi.ts

Notes attached to Workers, Candidates, Companies, Requests.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| **Worker Notes** | | | | | Read + Create only |
| `getWorkerProfileNotes(id, pagination)` | GET | `/api/AgencyWorkerProfile/{id}/Note?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | Fetch notes |
| `createWorkerProfileNote(id, model)` | POST | `/api/AgencyWorkerProfile/{id}/Note` | `NoteModel` | `CreateNoteResponse` | Add note |
| **Candidate Notes** | | | | | Full CRUD |
| `getCandidateNotes(id, pagination)` | GET | `/api/AgencyCandidate/{id}/Note?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | Fetch notes |
| `createCandidateNote(id, model)` | POST | `/api/AgencyCandidate/{id}/Note` | `NoteModel` | `CreateNoteResponse` | Add note |
| `deleteCandidateNote(id, noteId)` | DELETE | `/api/AgencyCandidate/{id}/Note/{id}` | — | `void` | Remove note |
| **Company Notes** | | | | | Full CRUD |
| `getAgencyCompanyNotes(id, pagination)` | GET | `/api/AgencyCompanyProfile/{id}/Note?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | Fetch notes |
| `createAgencyCompanyNote(id, model)` | POST | `/api/AgencyCompanyProfile/{id}/Note` | `NoteModel` | `CreateNoteResponse` | Add note |
| `updateAgencyCompanyNote(id, noteId, model)` | PUT | `/api/AgencyCompanyProfile/{id}/Note/{id}` | `NoteModel` | `void` | Update note |
| `deleteAgencyCompanyNote(id, noteId)` | DELETE | `/api/AgencyCompanyProfile/{id}/Note/{id}` | — | `void` | Remove note |
| **Request Notes** | | | | | Full CRUD |
| `getAgencyRequestNotes(id, pagination)` | GET | `/api/AgencyRequest/{id}/Note?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | Fetch notes |
| `createAgencyRequestNote(id, model)` | POST | `/api/AgencyRequest/{id}/Note` | `NoteModel` | `CreateNoteResponse` | Add note |
| `updateAgencyRequestNote(id, noteId, model)` | PUT | `/api/AgencyRequest/{id}/Note/{id}` | `NoteModel` | `void` | Update note |
| `deleteAgencyRequestNote(id, noteId)` | DELETE | `/api/AgencyRequest/{id}/Note/{id}` | — | `void` | Remove note |
| **Request → Worker Notes** | | | | | Full CRUD (nested) |
| `getAgencyRequestWorkerNotes(requestId, workerId, pagination)` | GET | `/api/AgencyRequest/{requestId}/Worker/{workerId}/Note?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | Fetch notes |
| `createAgencyRequestWorkerNote(requestId, workerId, model)` | POST | `/api/AgencyRequest/{requestId}/Worker/{workerId}/Note` | `NoteModel` | `CreateNoteResponse` | Add note |
| `updateAgencyRequestWorkerNote(requestId, workerId, noteId, model)` | PUT | `/api/AgencyRequest/{requestId}/Worker/{workerId}/Note/{id}` | `NoteModel` | `void` | Update note |
| `deleteAgencyRequestWorkerNote(requestId, workerId, noteId)` | DELETE | `/api/AgencyRequest/{requestId}/Worker/{workerId}/Note/{id}` | — | `void` | Remove note |

**Types:** `NoteModel`, `NoteItem`, `NotePagination`, `CreateNoteResponse` (from `src/types/agency`)

**Pinia:** None

**Business Logic:**
- Notes are internal annotations by agency staff
- Worker notes read-only; others full CRUD
- Request/Worker notes allow tracking communication per assignment
- Used in: Detail modals and tab views throughout

---

## 7. agencyPayStubApi.ts

Pay stub generation and payroll administration.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| **PayStubs CRUD** | | | | | |
| `getAgencyPayStubs(filter)` | GET | `/api/agency/accounting/PayStubs` | `AgencyPayStubFilter` (params) | `PaginatedList<AgencyPayStubListItem>` | List pay stubs |
| `downloadPayStubPdf(id)` | GET | `/api/v4/Accounting/PayStub/{id}/Document/PDF` | — | Blob | PDF download |
| `deleteAgencyPayStub(id)` | DELETE | `/api/v4/Accounting/PayStub/{id}` | — | `void` | Remove pay stub |
| `sendPayStubEmail(id)` | POST | `/api/v4/Accounting/PayStub/{id}/Document/Email` | — | `void` | Email to worker |
| `createAgencyPayStub(payload)` | POST | `/api/v4/accounting/PayStub` | `CreatePayStubPayload` | `void` | Create pay stub |
| **Generation** | | | | | |
| `getWorkersReadyForPayStub()` | GET | `/api/agency/accounting/PayStubs/WorkersReadyForPayStub` | — | `WorkerReadyForPayStubModel[]` | Workers with approved timesheets |
| `generatePayStubs(workerIds)` | POST | `/api/agency/accounting/PayStubs/generate` | string[] (worker IDs) | `void` | Batch generate from timesheets |
| **Subcontractors Report** | | | | | |
| `getPayrollSubcontractors(filter)` | GET | `/api/agency/accounting/reports/subcontractors` | `SubcontractorPayrollFilter` (params) | `PaginatedList<PayrollSubContractorListItem>` | Subcontractor payroll data |
| `downloadSubcontractorReport(weekEnding)` | GET | `/api/agency/accounting/reports/subcontractors/file` | `weekEnding` (param) | Blob | Excel export |
| **Skip Payroll Numbers** | | | | | |
| `getSkipPayrollNumbers(filter)` | GET | `/api/agency/accounting/PayStubs/skip-payroll-number` | `{ searchTerm? }` (params) | `SkipPayrollNumberItem[]` | Skipped payroll #s |
| `addSkipPayrollNumber(payload)` | POST | `/api/agency/accounting/PayStubs/skip-payroll-number` | `CreateSkipPayrollNumberPayload` | `void` | Add skip entry |

**Types:** `AgencyPayStubFilter`, `AgencyPayStubListItem`, `CreatePayStubPayload`, `CreateSkipPayrollNumberPayload`, `PayrollSubContractorListItem`, `SkipPayrollNumberItem`, `SubcontractorPayrollFilter`, `WorkerReadyForPayStubModel` (from `src/types/accounting`)

**Pinia:**
- Filter stored: `agencyPayStubFilter` in `agency` module
- Used in: Pay Stubs page (accounting)

**Business Logic:**
- `generatePayStubs()` bulk-creates from approved timesheets
- Subcontractors (flagged workers) appear in separate payroll report
- Skip payroll numbers allow gaps in numbering (e.g., voided pay stubs)

---

## 8. agencyReportApi.ts

Report generation and downloads (PDFs, Excel, data exports).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| **Generic Report Downloader** | | | | | |
| `downloadAgencyReport(url, filter)` | GET | (dynamic) | `ReportQueryParams` (params) | Blob | Generic blob downloader |
| **Timesheet Document** | | | | | |
| `getRequestTimeSheetDocument(requestId)` | GET | `/api/Request/{requestId}/TimeSheet/Document` | — | Blob | PDF/Excel per request |
| **Workers Report** | | | | | |
| `getWorkersReportDocument(requestId)` | GET | `/api/WorkersReportDocument/{requestId}/Document` | — | Blob | PDF/Excel per request |
| **Job Positions Hours** | | | | | |
| `getJobPositionsHoursWorked(filter & companyId)` | GET | `/api/agency/accounting/reports/{companyId}/job-positions` | `AgencyReportFilter` (params) | `AgencyCompanyJobPosition[]` | Hours per position |
| **Hours Worked Report** | | | | | |
| `getHoursWorkedReport(filter)` | GET | `/api/agency/accounting/reports/hours-worked` | `AgencyReportFilter` (params) | `HoursWorkedResume` | Aggregated hours |
| **T4 Report** | | | | | |
| `getT4Report(filter)` | GET | `/api/agency/accounting/reports/t4` | `AgencyReportFilter` (params) | Blob | Tax form export |
| **CRA Payroll Report** | | | | | |
| `getCraPayrollReport(filter)` | GET | `/api/agency/accounting/reports/cra-payroll` | `AgencyReportFilter` (params) | Blob | Canada Revenue Agency export |
| **Payment Report** | | | | | |
| `getPaymentReport(filter)` | GET | `/api/agency/accounting/reports/payments` | `AgencyReportFilter` (params) | `PaginatedList<WeeklyPayrollItem>` | Weekly payroll summary |
| **Weekly Payroll Export** | | | | | |
| `downloadWeeklyPayrollReport(weekEnding)` | GET | `/api/agency/accounting/reports/payments/file` | `weekEnding` (param) | Blob | Excel export |

**Types:** `ReportQueryParams`, `AgencyReportFilter`, `AgencyCompanyJobPosition`, `HoursWorkedResume`, `WeeklyPayrollItem` (from `src/types/agency`)

**Pinia:** None

**Business Logic:**
- Reports used by agency for compliance (T4, CRA), payroll admin, and internal analytics
- Generic downloader enables flexible report URL passing
- Timesheet documents per request show worker hours
- CRA export for tax filing

---

## 9. agencyRequestApi.ts

**Complex.** Core job request lifecycle (create, assign workers, track applicants, manage skills).

### Request CRUD
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `postAgencyRequest(model)` | POST | `/api/AgencyRequest` | `CreateAgencyRequestModel` | `AgencyRequestDetail` | Create request |
| `getAgencyRequests(filter)` | GET | `/api/AgencyRequest` | `AgencyRequestFilter` (params) | `PaginatedList<AgencyRequestListItem>` | List requests (own agency) |
| `getAllAgencyRequests(filter)` | GET | `/api/AgencyRequest/all` | `AgencyRequestFilter` (params) | `PaginatedList<AgencyRequestListItem>` | List requests (all agencies) |
| `getAgencyRequest(id)` | GET | `/api/AgencyRequest/{id}` | — | `AgencyRequestDetail` | Fetch request details |
| `updateAgencyRequest(id, model)` | PUT | `/api/AgencyRequest/{id}` | `CreateAgencyRequestModel` | `AgencyRequestDetail` | Update request |
| `cancelAgencyRequest(id, payload)` | PUT | `/api/AgencyRequest/{id}/Cancel` | `CancelRequestPayload` | `void` | Cancel request + reason |
| `agencyRequestOpen(id)` | PUT | `/api/AgencyRequest/{id}/Open` | id (in body) | `void` | Reopen request |
| `agencyRequestSendInvitation(id)` | POST | `/api/AgencyRequest/{id}/SendInvitation` | — | `void` | Send invite to candidates |
| `updateAgencyRequestIsAsap(id)` | PUT | `/api/AgencyRequest/{id}/IsAsap` | — | `void` | Toggle ASAP flag |
| `updateAgencyPunchCardVisibilityStatusInApp(id)` | PUT | `/api/AgencyRequest/{id}/PunchCardVisibilityStatusInApp` | — | `void` | Control punch card visibility |
| `updateAgencyRequestShift(id, model)` | PUT | `/api/AgencyRequest/{id}/Shift` | `RequestShiftModel` | `{ id: string; displayShift? }` | Update shift times |
| `increaseWorkersQuantityByOne(id)` | PUT | `/api/AgencyRequest/{id}/IncreaseWorkersQuantityByOne` | — | `void` | +1 worker needed |
| `reduceWorkersQuantityByOne(id)` | PUT | `/api/AgencyRequest/{id}/ReduceWorkersQuantityByOne` | — | `void` | -1 worker needed |

### Request → Workers
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestsWorkers(filter)` | GET | `/api/AgencyRequest/{requestId}/Worker` | `AgencyRequestWorkerFilter` (params) | `PaginatedList<AgencyRequestWorker>` | Assigned workers |
| `bookAgencyRequestWorker(requestId, workerId, model)` | POST | `/api/AgencyRequest/{requestId}/Worker/{workerId}/Book` | `BookWorkerModel` | `{ id: string }` | Assign worker to request |
| `updateAgencyRequestWorkerStartDate(requestId, id, model)` | PUT | `/api/AgencyRequest/{requestId}/Worker/{id}` | `BookWorkerModel` | `void` | Update start date |
| `rejectAgencyRequestWorker(requestId, workerId, model)` | PUT | `/api/AgencyRequest/{requestId}/Worker/{workerId}/Reject` | `RejectWorkerModel` | `void` | Reject worker + reason |

### Applicants (workers applying to request)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `searchAgencyRequestApplicants(id, searchTerm)` | GET | `/api/AgencyRequest/{id}/Applicant/Search` | `searchTerm` (param) | `AgencyRequestApplicant[]` | Search applicants |
| `getAgencyRequestApplicant(filter)` | GET | `/api/AgencyRequest/{requestId}/Applicant` | `AgencyRequestApplicantFilter` (params) | `PaginatedList<AgencyRequestApplicant>` | List applicants |
| `postAgencyRequestApplicant(id, model)` | POST | `/api/AgencyRequest/{id}/Applicant` | `CreateRequestApplicantModel` | `AgencyRequestApplicant` | Add applicant |
| `deleteAgencyRequestApplicant(id, applicantId)` | DELETE | `/api/AgencyRequest/{id}/Applicant/{id}` | — | `void` | Remove applicant |
| `updateAgencyRequestApplicant(id, applicantId, model)` | PUT | `/api/AgencyRequest/{id}/Applicant/{id}` | `UpdateApplicantCommentsPayload` | `void` | Update applicant status/comments |

### Request Contact Persons (RequestedBy / ReportTo)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestRequestedBy(id)` | GET | `/api/AgencyRequest/{id}/RequestedBy` | — | `PaginatedList<AgencyRequestPersonItem>` | Who requested (from company) |
| `postAgencyRequestRequestedBy(id, personId)` | POST | `/api/AgencyRequest/{id}/RequestedBy/{personId}` | — | `void` | Add requester |
| `deleteAgencyRequestRequestedBy(id, personId)` | DELETE | `/api/AgencyRequest/{id}/RequestedBy/{personId}` | — | `void` | Remove requester |
| `getAgencyRequestReportTo(id)` | GET | `/api/AgencyRequest/{id}/ReportTo` | — | `PaginatedList<AgencyRequestPersonItem>` | Who supervises worker |
| `postAgencyRequestReportTo(id, personId)` | POST | `/api/AgencyRequest/{id}/ReportTo/{personId}` | — | `void` | Add supervisor |
| `deleteAgencyRequestReportTo(id, personId)` | DELETE | `/api/AgencyRequest/{id}/ReportTo/{personId}` | — | `void` | Remove supervisor |

### Recruiters (agency personnel assigned)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestRecruiter(id)` | GET | `/api/AgencyRequest/{id}/Recruiter` | — | `PaginatedList<AgencyRequestRecruiterItem>` | Assigned recruiters |
| `postAgencyRequestRecruiter(id, model)` | POST | `/api/AgencyRequest/{id}/Recruiter` | `AgencyRequestRecruiterModel` | `void` | Assign recruiter |
| `deleteAgencyRequestRecruiter(id, recruiterId)` | DELETE | `/api/AgencyRequest/{id}/Recruiter/{id}` | — | `void` | Unassign recruiter |

### Skills
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestSkill(id)` | GET | `/api/AgencyRequest/{id}/Skill` | — | `{ id, skill }[]` | Required skills |
| `postAgencyRequestSkill(id, model)` | POST | `/api/AgencyRequest/{id}/Skill` | `AgencyRequestSkillModel` | `{ id: string }` | Add required skill |
| `deleteAgencyRequestSkill(id, skillId)` | DELETE | `/api/AgencyRequest/{id}/Skill/{id}` | — | `void` | Remove skill requirement |

**Types:** `AgencyRequestFilter`, `AgencyRequestListItem`, `AgencyRequestDetail`, `CreateAgencyRequestModel`, `RequestShiftModel`, `CancelRequestPayload`, `AgencyRequestWorkerFilter`, `AgencyRequestWorker`, `BookWorkerModel`, `RejectWorkerModel`, `AgencyRequestApplicantFilter`, `AgencyRequestApplicant`, `CreateRequestApplicantModel`, `UpdateApplicantCommentsPayload`, `AgencyRequestRecruiterModel`, `AgencyRequestRecruiterItem`, `AgencyRequestSkillModel`, `AgencyRequestPersonItem` (from `src/types/agency`)

**Pinia:**
- Filter stored: `agencyRequestFilter` in `agency` module
- Used in: Requests page, Request detail, Board view

**Business Logic:**
- Request = job request from company; defines role, rate, location, shift
- Applicants = workers who applied; can be promoted to booked workers
- Recruiters = agency personnel assigned to fill request
- RequestedBy = company contact who made the request
- ReportTo = supervisor at company (workers report to them)
- Skills are job requirements; can be searched/matched
- ASAP toggles priority
- Board view shows weekly staffing snapshot

---

## 10. agencyTimeSheetApi.ts

Timesheet management for workers on requests (hours tracked).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyWorkerTimeSheet(requestId, workerId)` | GET | `/api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet` | — | `TimeSheetListItem[]` | All timesheets |
| `getAgencyWorkerTimeSheetByDate(requestId, workerId, date)` | GET | `/api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet` | `{ startDate, endDate }` (params) | `TimeSheetListItem[]` | Filtered by date range |
| `postAgencyWorkerTimeSheet(requestId, workerId, model)` | POST | `/api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet` | `TimeSheetModel` | `{ id: string }` | Create timesheet entry |
| `updateAgencyWorkerTimeSheet(requestId, workerId, id, model)` | PUT | `/api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet/{id}` | `TimeSheetModel` | `void` | Update timesheet |
| `deleteAgencyWorkerTimeSheet(requestId, workerId, id)` | DELETE | `/api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet/{id}` | — | `void` | Remove timesheet entry |
| `getAgencyTimeSheetUsages(requestId, workerId, id)` | GET | `/api/v2/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet/{id}/Usages` | — | `TimeSheetUsagesModel` | Where timesheet is used |

**Types:** `TimeSheetListItem`, `TimeSheetModel`, `TimeSheetUsagesModel` (from `src/types/company`)

**Pinia:** None

**Business Logic:**
- Timesheets record hours per worker per request
- Used to generate pay stubs (aggregated wages)
- Date filtering allows weekly/period review
- Usages show which invoices/pay stubs include the timesheet

---

## 11. agencyWorkerApi.ts

Worker profile management from agency perspective.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| **Workers List** | | | | | |
| `getAgencyWorkers(filter)` | GET | `/api/AgencyWorkerProfile` | `AgencyWorkerFilter` (params) | `PaginatedList<AgencyWorkerListItem>` | List workers |
| `getAgencyWorkersDropdown(filter)` | GET | `/api/AgencyWorkerProfile/Dropdown` | `{ searchTerm }` (params) | `AgencyWorkerDropdownItem[]` | Autocomplete search |
| **Single Worker** | | | | | |
| `getAgencyWorker(id)` | GET | `/api/AgencyWorkerProfile/{id}` | — | `WorkerProfile` | Fetch full worker |
| **Flags** | | | | | |
| `updateApprovedToWork(id)` | PUT | `/api/AgencyWorkerProfile/{id}/ApprovedToWork` | — | `void` | Toggle approved-to-work |
| `updateAgencyWorkerProfileDNU(id)` | PUT | `/api/AgencyWorkerProfile/{id}/Dnu` | — | `void` | Toggle DNU (Do Not Use) |
| `updateAgencyWorkerContractor(id)` | PUT | `/api/AgencyWorkerProfile/{id}/IsContractor` | — | `void` | Toggle contractor flag |
| `updateAgencyWorkerSubContractor(id)` | PUT | `/api/AgencyWorkerProfile/{id}/IsSubcontractor` | — | `void` | Toggle subcontractor flag |
| **Tax / External ID** | | | | | |
| `updateWorkerProfileTaxCategory(payload)` | PUT | `/api/AgencyWorkerProfile/{id}/tax-category` | `UpdateWorkerProfileFieldsPayload` | `void` | Update tax category |
| `updateWorkerProfileTaxRate(payload)` | PUT | `/api/AgencyWorkerProfile/{id}/tax-rate` | `UpdateWorkerProfileFieldsPayload` | `void` | Update tax rate |
| `updateWorkerProfileExternalId(payload)` | PUT | `/api/AgencyWorkerProfile/{id}/ExternalId` | `UpdateWorkerProfileFieldsPayload` | `void` | External system ID |
| **Email** | | | | | |
| `updateAgencyWorkerEmail(id, model)` | PUT | `/api/AgencyWorkerProfile/{id}/Email` | `UpdateWorkerEmailModel` | `void` | Update worker email |
| **Comments** | | | | | |
| `agencyCommentWorker(id, comment)` | POST | `/api/AgencyWorker/{id}/Comment` | `AgencyWorkerCommentModel` | `void` | Add agency comment |
| **History & Holidays** | | | | | |
| `getAgencyWorkerProfileRequestHistory(id, pagination)` | GET | `/api/AgencyWorkerProfile/{id}/RequestHistory?PageSize={size}&PageIndex={page}` | — | `PaginatedList<AgencyWorkerRequestHistoryItem>` | Worker's past requests |
| `getAgencyWorkerProfileHolidays(id)` | GET | `/api/agency-worker-profile-holiday/{id}` | — | `AgencyWorkerHoliday[]` | Holidays off |
| `addUpdateAgencyWorkerProfileHolidays(id, data)` | POST | `/api/agency-worker-profile-holiday/{id}` | `AgencyWorkerHoliday` | `void` | Add/update holiday |
| `addNewHoliday(payload)` | POST | `/api/agency-worker-profile-holiday/new-holiday` | `AddNewHolidayPayload` | `void` | Bulk holiday add |

**Types:** `AgencyWorkerFilter`, `AgencyWorkerListItem`, `AgencyWorkerDropdownItem`, `AgencyWorkerCommentModel`, `UpdateWorkerEmailModel`, `UpdateWorkerProfileFieldsPayload`, `AgencyWorkerHoliday`, `AddNewHolidayPayload`, `AgencyWorkerRequestHistoryItem` (from `src/types/agency`); `WorkerProfile` (from `src/types/worker`)

**Pinia:**
- Filter stored: `agencyWorkerProfileFilter` in `agency` module
- Used in: Workers page

**Business Logic:**
- DNU = "Do Not Use" flag (worker unavailable/removed)
- Contractor/Subcontractor flags affect payroll (W2 vs 1099 equivalent in Canada)
- Tax category/rate used for withholdings
- Request history shows all past assignments
- Holidays block worker from new bookings

---

## 12. catalogApi.ts

Reference data (enums, lookup tables).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getGenders()` | GET | `/api/Catalog/gender` | — | `Gender[]` | Male, Female, Other, etc. |
| `getIdentificationTypes()` | GET | `/api/Catalog/identificationType` | — | `IdentificationType[]` | Passport, Driver's License, etc. |
| `getAvailability()` | GET | `/api/Catalog/availability` | — | `Availability[]` | Full-time, Part-time, etc. |
| `getAvailabilityTimes()` | GET | `/api/Catalog/availabilityTime` | — | `AvailabilityTime[]` | Morning, Afternoon, Evening, Night |
| `getDays()` | GET | `/api/Catalog/day` | — | `Day[]` | Mon–Sun |
| `fetchLifts()` | GET | `/api/Catalog/lift` | — | `Lift[]` | Forklift, Crane, etc. |
| `fetchLanguages()` | GET | `/api/Catalog/language` | — | `Language[]` | English, French, Spanish, etc. |
| `getWsibGroups()` | GET | `/api/Catalog/wsibgroup` | — | `WsibGroup[]` | WSIB classifications (Canada) |
| `getJobPositions()` | GET | `/api/Catalog/jobPosition` | — | `JobPosition[]` | Cashier, Warehouse, etc. |
| `getSkills()` | GET | `/api/Catalog/skills` | — | `Skill[]` | Forklift, Communication, etc. |
| `getIndustries()` | GET | `/api/Catalog/industry` | — | `Industry[]` | Retail, Manufacturing, etc. |
| `getReasonCancellationRequest()` | GET | `/api/Catalog/reasonCancellationRequest` | — | `CancellationReason[]` | Why request was cancelled |
| `getCompanyStatus()` | GET | `/api/Catalog/companyStatus` | — | `CatalogItem<number>[]` | Active, Inactive, etc. |
| `getTaxCategories()` | GET | `/api/Catalog/tax-categories` | — | `TaxCategory[]` | Employee, Contractor, etc. |
| `addIndustry(industry)` | POST | `/api/Catalog/industry` | `{ id?, value }` | `Industry` | Add custom industry |

**Types:** `Gender`, `IdentificationType`, `Availability`, `AvailabilityTime`, `Day`, `Lift`, `Language`, `WsibGroup`, `Industry`, `JobPosition`, `Skill`, `CancellationReason`, `CatalogItem`, `TaxCategory` (from `src/types/common`)

**Pinia:** None

**Business Logic:**
- Static lookup tables
- WSIB groups are Canadian workplace safety classifications
- Used in dropdowns throughout app
- `addIndustry()` allows dynamic custom industries

---

## 13. companyApi.ts

Company-side (client) view of their requests and workers.

### Profile & Locations
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyProfile()` | GET | `/api/CompanyProfile` | — | `CompanyProfileDetail` | Current company |
| `updateProfile(id, company)` | PUT | `/api/CompanyProfile/{id}` | `CompanyProfileDetail` | `void` | Update profile |
| `registerCompany(company)` | POST | `/api/CompanyProfile` | `CompanyProfileDetail` | `void` | Register new company |
| `getProfileLocations()` | GET | `/api/CompanyProfile/Location` | — | `CompanyProfileLocationDetail[]` | Company sites |
| `createProfileLocation(model)` | POST | `/api/CompanyProfile/Location` | `CompanyProfileLocationDetail` | `void` | Add site |
| `updateProfileLocation(id, model)` | PUT | `/api/CompanyProfile/Location/{id}` | `CompanyProfileLocationDetail` | `void` | Update site |
| `deleteProfileLocation(id)` | DELETE | `/api/CompanyProfile/Location/{id}` | — | `void` | Remove site |
| `getLocations()` | GET | `/api/CompanyLocation` | — | `CompanyProfileLocationDetail[]` | All locations |

### Job Positions
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyJobPositions()` | GET | `/api/CompanyJobPosition` | — | `CompanyProfileJobPositionRate[]` | Available roles + rates |
| `getCompanyJobPositionById(id)` | GET | `/api/CompanyJobPosition/{id}` | — | `CompanyProfileJobPositionRate` | Single position |
| `requestNewPosition(data)` | POST | `/api/CompanyJobPosition/request-new-position` | `{ title, name, email, phone, message, subject }` | `void` | Request new role |

### Requests
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getRequests(filter)` | GET | `/api/CompanyRequest` | `CompanyRequestFilter` (params) | `PaginatedList<CompanyRequestListItem>` | List requests |
| `getRequest(id)` | GET | `/api/CompanyRequest/{id}` | — | `CompanyRequestListItem` | Fetch request |
| `createRequest(request)` | POST | `/api/CompanyRequest` | `CreateAgencyRequestModel` | `{ id: string }` | Create request |
| `editRequest(id, model)` | PUT | `/api/CompanyRequest/{id}` | `{ requirements }` | `void` | Update requirements |
| `cancelRequest(id, reasonId, otherReason)` | PUT | `/api/CompanyRequest/{id}/Cancel` | `{ cancellationReasonId, otherCancellationReason }` | `void` | Cancel + reason |

### Request Workers
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getRequestWorkers(filter)` | GET | `/api/CompanyRequest/{requestId}/Worker` | `CompanyRequestWorkerFilter` (params) | `PaginatedList<CompanyRequestWorker>` | Assigned workers |
| `getRequestWorker(requestId, workerId)` | GET | `/api/CompanyRequest/{requestId}/Worker/{workerId}` | — | `CompanyRequestWorker` | Single worker |
| `rejectCompanyRequestWorker(requestId, workerId, model)` | PUT | `/api/CompanyRequest/{requestId}/Worker/{workerId}/Reject` | `CommentsModel` | `void` | Reject worker + comment |
| `requestAnotherWorker(requestId, comment)` | POST | `/api/CompanyRequest/{requestId}/Worker/RequestNewWorker` | `CommentsModel` | `void` | Ask for replacement |

### TimeSheet
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyWorkerTimeSheetByDate(requestId, workerId, date)` | GET | `/api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet` | `{ startDate, endDate }` (params) | `TimeSheetListItem[]` | Timesheet by date |
| `postCompanyWorkerTimeSheet(requestId, workerId, model)` | POST | `/api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet` | `TimeSheetModel` | `{ id: string }` | Create entry |
| `validateHoursTimeSheet(requestId, workerId, id, model)` | PUT | `/api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet/{id}` | `TimeSheetModel` | `void` | Validate hours |
| `validateAllHoursTimeSheet(requestId, workerId)` | PUT | `/api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet` | — | `void` | Validate all |
| `updateCompanyRequestWorkerTimeSheet(requestId, workerId, id, model)` | PUT | `/api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet/{id}` | `TimeSheetModel` | `void` | Update entry |
| `deleteCompanyWorkerTimeSheet(requestId, workerId, id)` | DELETE | `/api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet/{id}` | — | `void` | Remove entry |
| `companyTimeSheetClockIn(requestId, workerId, model)` | POST | `/api/v2/CompanyRequest/{requestId}/Worker/{workerId}/TimeSheet/ClockIn` | `ClockInModel` | `ClockInResult` | Mobile clock-in |

### Comments
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `companyCommentWorker(id, comment)` | POST | `/api/CompanyWorker/{id}/Comment` | `CommentsModel` | `void` | Add comment to worker |

### Company Users
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyUser()` | GET | `/api/CompanyUser` | — | `CompanyUserModel[]` | List users |
| `getCompanyUserDetail()` | GET | `/api/CompanyUser/detail` | — | `CompanyUserModel` | Current user |
| `createCompanyUser(model)` | POST | `/api/CompanyUser` | `CreateCompanyUserModel` | `void` | Add user |
| `updateCompanyUser(id, user)` | PUT | `/api/CompanyUser/{id}` | `CompanyUserModel` | `void` | Update user |
| `deleteCompanyUser(id)` | DELETE | `/api/CompanyUser/{id}` | — | `void` | Remove user |

### Contact People
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getContactPeople()` | GET | `/api/CompanyProfileContactPerson` | — | `CompanyContactPersonModel[]` | Contact list |
| `saveContactPerson(model)` | POST | `/api/CompanyProfileContactPerson` | `CompanyContactPersonModel` | `void` | Create/update contact |
| `deleteContactPerson(id)` | DELETE | `/api/CompanyProfileContactPerson/{id}` | — | `void` | Remove contact |

### Invoices
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyInvoice(filter)` | GET | `/api/CompanyInvoice` | `CompanyInvoiceFilter` (params) | `PaginatedList<CompanyInvoiceListItem>` | List invoices |
| `getCompanyInvoiceDetail(id)` | GET | `/api/CompanyInvoice/{id}` | — | `InvoiceSummaryModel` | Invoice details |

**Types:** `CompanyProfileDetail`, `CompanyProfileLocationDetail`, `CompanyProfileJobPositionRate`, `CompanyRequestFilter`, `CompanyRequestListItem`, `CompanyRequestWorkerFilter`, `CompanyRequestWorker`, `TimeSheetListItem`, `TimeSheetModel`, `ClockInModel`, `ClockInResult`, `CompanyUserModel`, `CreateCompanyUserModel`, `CompanyContactPersonModel`, `CompanyInvoiceFilter`, `CompanyInvoiceListItem`, `CommentsModel` (from `src/types/company`)

**Pinia:**
- Store `company` holds only `companyRequestFilter`
- Used in: Company portal pages

**Business Logic:**
- Company view is "read mostly" — they see their requests + assigned workers
- Timesheet validation triggers invoicing
- Clock-in captures GPS + time for mobile workers
- Contact people are company staff who interact with agency

---

## 14. downloadApi.ts

File downloads (blobs).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `fetchInvoicePdf(id)` | GET | `/api/Invoice/{id}/Document/PDF` | — | Blob | Invoice PDF |
| `downloadPayrollSubcontractor(weekEnding)` | GET | `/api/PayrollSubcontractor/{weekEnding}/Document/EXCEL` | — | Blob | Subcontractor Excel |
| `downloadWeeklyPayrollExcel(weekEnding)` | GET | `/api/WeeklyPayroll/{weekEnding}/Document/EXCEL` | — | Blob | Weekly payroll Excel |
| `downloadWeeklyPayrollExcelByWeekEnding(date)` | GET | `/api/WeeklyPayroll/{date}/Document/EXCEL/ByWeekEnding` | — | Blob | Payroll by week ending |
| `downloadWeeklyPayrollExcelByPaymentDate(date)` | GET | `/api/WeeklyPayroll/{date}/Document/EXCEL/ByPaymentDate` | — | Blob | Payroll by payment date |

**Pinia:** None

**Business Logic:**
- Generic blob downloads
- Week ending vs payment date offer different groupings for payroll

---

## 15. locationApi.ts

Geographic lookup (countries, provinces, cities).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCountries()` | GET | `/api/Location/country` | — | `Country[]` | All countries |
| `getProvinces(countryId)` | GET | `/api/Location/province/{countryId}` | — | `Province[]` | Provinces/states |
| `getCities(provinceId)` | GET | `/api/Location/city/{provinceId}` | — | `City[]` | Cities |
| `createCity(city)` | POST | `/api/Location/city` | `{ value, code?, province: { id } }` | `City` | Add custom city |
| `addProvinceSetting(provinceId, settings)` | POST | `/api/Location/province/{provinceId}/settings` | `{ paidHolidays?, overtimeStartsAfter? }` | `void` | Configure provincial rules |
| `getLocationTax(locationId)` | GET | `/api/Location/{locationId}/tax` | — | `LocationTax \| null` | Tax (%) configured for a location (admin) |
| `upsertLocationTax(locationId, model)` | PUT | `/api/Location/{locationId}/tax` | `LocationTax` | `void` | Create/update a location's tax (%) (admin) |

**Types:** `Country`, `Province`, `City`, `LocationTax` (from `src/types/common`)

**Pinia:** None

**Business Logic:**
- Hierarchical: Country > Province > City
- Provincial settings control payroll rules (Canada-specific)

---

## 16. requestApi.ts

Minimal. Request shift lookup.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `fetchRequestShift(id)` | GET | `/api/Request/{id}/Shift` | — | `RequestShiftModel` | Fetch shift times |

**Types:** `RequestShiftModel` (from `src/types/agency`)

**Pinia:** None

---

## 17. sharedApi.ts

Email preferences (unsubscribe).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `unsubscribe(model)` | POST | `/api/EmailPreferences/Unsubscribe` | `UnsubscribeRequest` | `void` | Unsubscribe from emails |

**Types:** `UnsubscribeRequest` (from `src/types/common`)

**Pinia:** None

---

## 18. userNotificationApi.ts

User notification management.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getUserNotifications()` | GET | `/api/UserNotification` | — | `UserNotificationItem[]` | Fetch notifications |
| `updateUserNotification(model)` | PUT | `/api/UserNotification` | `UserNotificationItem` | `void` | Mark read/update |

**Types:** `UserNotificationItem` (from `src/types/common`)

**Pinia:** None

**Business Logic:**
- In-app notification inbox

---

## 19. websiteApi.ts

Public website (landing page) endpoints.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getJobs(filter)` | GET | `/api/WebSite/jobs` | `JobSearchFilter` (params, auto-adds countries: ['USA', 'CA']) | `JobViewModel[]` | Job search (public) |
| `getLandingJobPositions()` | GET | `/data/job-positions.json` | — | `LandingJobPositions` | Static JSON (local) |
| `submitContactForm(contact)` | POST | `/api/WebSite/contact` | `ContactForm` | `void` | Contact form submission |
| `submitCandidate(formData)` | POST | `/api/WebSite/candidate` | FormData (multipart) | `void` | Public candidate apply |

**Types:** `JobSearchFilter`, `JobViewModel`, `ContactForm`, `LandingJobPositions` (from `src/types/website`)

**Pinia:** None

**Business Logic:**
- Public-facing; no auth required
- `getLandingJobPositions()` is static JSON bundled in app, not API call
- Job search hardcoded to USA/Canada

---

## 20. workerApi.ts

**Large.** Worker-side profile + application + timesheet management.

### Requests (Job Applications)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getJobs(filter)` | GET | `/api/WorkerRequest` | `WorkerRequestFilter` (params) | `PaginatedList<WorkerRequestListItem>` | Available jobs |
| `getWorkerRequest(id)` | GET | `/api/WorkerRequest/{id}` | — | `WorkerRequestDetail` | Job details |
| `workerRequestApplySelf(requestId, model)` | POST | `/api/WorkerRequest/{requestId}/Apply` | `WorkerRequestApplyModel` | `void` | Self-apply to job |
| `workerRequestApply(workerId, requestId, model)` | POST | `/api/WorkerRequest/{workerId}/{requestId}/Apply` | `WorkerRequestApplyModel` | `void` | (Third-party apply?) |
| `workerRequestDecline(id)` | DELETE | `/api/WorkerRequest/Decline/{id}` | — | `void` | Decline offer |

### TimeSheet
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `workerRegisterTime(requestId, lat, lon)` | POST | `/api/WorkerRequest/{requestId}/TimeSheet` | `{ latitude, longitude }` | `void` | Clock-in with GPS |
| `workerGetTimeSheet(requestId)` | GET | `/api/WorkerRequest/{requestId}/TimeSheet` | — | `WorkerTimeSheetItem[]` | Worker's shifts |
| `getClockType(requestId, date)` | GET | `/api/WorkerRequest/{requestId}/TimeSheet/clock-type` | `date` (param) | `ClockTypeResult` | Can clock in/out? |

### Comments
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCommentsWorker(filter)` | GET | `/api/worker/{workerId}/comment` | `WorkerCommentFilter` (params: size, pageIndex) | `WorkerCommentList` | Feedback on worker |

### Profile
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getMyProfile()` | GET | `/api/WorkerProfile/me` | — | `WorkerProfile` | Current user's profile |
| `registerWorker(payload)` | POST | `/api/WorkerProfile` | FormData (multipart) | string (profile ID) | Worker registration |
| `uploadWorker(id, worker)` | PUT | `/api/WorkerProfile/{id}` | `WorkerProfile` | `void` | Update profile |

### Request History
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getWorkerRequestHistory(filter)` | GET | `/api/WorkerRequestHistory` | `WorkerRequestFilter` (params) | `PaginatedList<WorkerRequestListItem>` | Past job applications |
| `getWorkerRequestHistoryDetail(id)` | GET | `/api/WorkerRequestHistory/{id}` | — | `WorkerRequestDetail` | Past job detail |

### Job Experience
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createWorkerWorkExperience(id, model)` | POST | `/api/WorkerProfile/{id}/JobExperience` | `WorkerJobExperienceModel` | `void` | Add work history |
| `editWorkerWorkExperience(id, expId, model)` | PUT | `/api/WorkerProfile/{id}/JobExperience/{id}` | `WorkerJobExperienceModel` | `void` | Update history |
| `deleteWorkerWorkExperience(id, expId)` | DELETE | `/api/WorkerProfile/{id}/JobExperience/{id}` | — | `void` | Remove history |

### SIN / Documents
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createWorkerSin(id, formData)` | POST | `/api/WorkerProfile/{id}/SinInformation` | FormData (multipart) | `void` | Upload SIN (Canadian ID) |
| `createWorkerDocuments(id, formData)` | POST | `/api/WorkerProfile/{id}/Documents` | FormData (multipart) | `void` | Upload docs |
| `createWorkerResume(id, formData)` | POST | `/api/WorkerProfile/{id}/Resume` | FormData (multipart) | `void` | Upload resume |
| `createWorkerLicenses(id, formData)` | POST | `/api/WorkerProfile/{id}/Licenses` | FormData (multipart) | `void` | Upload licenses |
| `deleteWorkerLicenses(id, licenseId)` | DELETE | `/api/WorkerProfile/{id}/Licenses/{licenseId}` | — | `void` | Remove license |
| `createWorkerCertificates(id, formData)` | POST | `/api/WorkerProfile/{id}/Certificates` | FormData (multipart) | `void` | Upload certs |
| `deleteWorkerCertificates(id, certId)` | DELETE | `/api/WorkerProfile/{id}/Certificates/{certId}` | — | `void` | Remove cert |
| `createWorkerOtherDocuments(id, formData)` | POST | `/api/WorkerProfile/{id}/OtherDocument` | FormData (multipart) | `void` | Upload other docs |
| `deleteWorkerOtherDocuments(id, docId)` | DELETE | `/api/WorkerProfile/{id}/OtherDocument/{docId}` | — | `void` | Remove other doc |
| `createWorkerImage(id, formData)` | POST | `/api/WorkerProfile/{id}/ProfileImage` | FormData (multipart) | `void` | Upload profile pic |

### Profile Sections
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createWorkerBasicInformation(id, model)` | POST | `/api/WorkerProfile/{id}/BasicInformation` | `WorkerBasicInformationModel` | `void` | Demographics (name, DOB, etc) |
| `createWorkerContactInformation(id, model)` | POST | `/api/WorkerProfile/{id}/ContactInformation` | `WorkerContactInformationModel` | `void` | Phone, email |
| `createWorkerEmergencyInformation(id, model)` | POST | `/api/WorkerProfile/{id}/EmergencyInformation` | `WorkerEmergencyInformationModel` | `void` | Emergency contact |
| `createWorkerOther(id, model)` | POST | `/api/WorkerProfile/{id}/OtherInformation` | `WorkerOtherInformationModel` | `void` | WSIB group, etc |

### Preferences & Skills
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createWorkerAvailabilities(id, model)` | POST | `/api/WorkerProfile/{id}/Availabilities` | `WorkerCatalogItem[]` | `void` | Full-time, Part-time, etc |
| `createWorkerAvailabilityTimes(id, model)` | POST | `/api/WorkerProfile/{id}/AvailabilityTimes` | `WorkerCatalogItem[]` | `void` | Morning, Evening, etc |
| `createWorkerAvailabilityDays(id, model)` | POST | `/api/WorkerProfile/{id}/AvailabilityDays` | `WorkerCatalogItem[]` | `void` | M–F, Weekends, etc |
| `createWorkerLocationPreferences(id, model)` | POST | `/api/WorkerProfile/{id}/LocationPreferences` | `WorkerCatalogItem[]` | `void` | Preferred cities/regions |
| `createWorkerLanguages(id, model)` | POST | `/api/WorkerProfile/{id}/Languages` | `WorkerCatalogItem[]` | `void` | Languages spoken |
| `createWorkerSkills(id, model)` | POST | `/api/WorkerProfile/{id}/Skills` | string[] | `void` | Skills list |

### Wage & TimeSheet History
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getWorkerProfileWageHistory(filter)` | GET | `/api/WorkerProfile/{profileId}/WageHistory` | `WageHistoryFilter` (params) | `PaginatedList<WorkerWageHistoryItem>` | Earnings over time |
| `getWorkerProfileWageHistoryAccumulated(id, rowNumber)` | GET | `/api/WorkerProfile/{id}/WageHistory/{rowNumber}` | — | `WorkerWageHistoryItem` | Cumulative earnings |
| `getWorkerProfileTimeSheetHistory(filter)` | GET | `/api/WorkerProfile/{profileId}/TimeSheetHistory` | `TimeSheetHistoryFilter` (params) | `PaginatedList<WorkerTimeSheetHistoryItem>` | Shifts over time |
| `getWorkerProfileTimeSheetHistoryAccumulated(id, rowNumber)` | GET | `/api/WorkerProfile/{id}/TimeSheetHistory/{rowNumber}` | — | `WorkerTimeSheetHistoryItem` | Cumulative hours |

**Types:** `WorkerProfile`, `WorkerRequestFilter`, `WorkerRequestApplyModel`, `WorkerCommentFilter`, `WorkerCommentList`, `WageHistoryFilter`, `TimeSheetHistoryFilter`, `ClockTypeResult`, `WorkerCatalogItem`, `WorkerBasicInformationModel`, `WorkerContactInformationModel`, `WorkerEmergencyInformationModel`, `WorkerOtherInformationModel`, `WorkerJobExperienceModel`, `WorkerRequestListItem`, `WorkerRequestDetail`, `WorkerTimeSheetItem`, `WorkerWageHistoryItem`, `WorkerTimeSheetHistoryItem` (from `src/types/worker`)

**Pinia:**
- Store `worker` holds only `workerProfile` (partial)
- Used in: Worker portal pages

**Business Logic:**
- Worker = job seeker; registers profile, applies to jobs, tracks hours
- Registration is FormData (multipart) due to file upload
- SIN (Social Insurance Number) is Canada-specific tax ID
- Profile split into sections to allow partial completion
- Wage/timesheet history provides earnings transparency

---

## Summary Table: API Endpoints by Entity

| Entity | Primary File(s) | CRUD Operations | Notable Features |
|--------|----------------|-----------------|------------------|
| **Agency** | agencyApi.ts | Full | Multi-location, personnel, agency switching |
| **Candidate** | agencyCandidateApi.ts | Full (CRUD) | Convert to worker, skills, documents, bulk upload |
| **Company** | agencyCompanyApi.ts | Full | Locations, job positions, contact persons, documents, settings |
| **Request** | agencyRequestApi.ts | Full | Workers, applicants, recruiters, skills, shift management |
| **Worker** (Agency) | agencyWorkerApi.ts | Read + flags | DNU, contractor, tax category, holidays, request history |
| **Worker** (Self) | workerApi.ts | Full | Profile build, job applications, timesheet, wage history |
| **Invoice** | agencyInvoiceApi.ts | Full | Preview, PDF, email, verification code |
| **PayStub** | agencyPayStubApi.ts | Full | Generation, subcontractor report, skip numbers |
| **TimeSheet** | agencyTimeSheetApi.ts | Full | By request/worker, date range, usages |
| **Note** | agencyNoteApi.ts | Mixed (CRUD) | Worker read-only; others full CRUD; nested by entity |
| **Location** | locationApi.ts | Read + create | Hierarchical (country > province > city), provincial settings |
| **Catalog** | catalogApi.ts | Read + add | Reference data (genders, jobs, skills, industries, tax categories) |
| **Report** | agencyReportApi.ts | Read | T4, CRA, timesheet, hours worked, payment, weekly payroll |
| **Account** | accountApi.ts | Limited | Email change, account deactivation |
| **Website** | websiteApi.ts | Limited | Public job search, contact form, candidate apply |
| **Notification** | userNotificationApi.ts | Read + update | In-app inbox |

---

## Key Design Patterns

1. **No Service Locator**: API functions are plain functions, not store dispatches. Components import and call directly.
2. **Type Safety**: All request/response types are TypeScript interfaces in `src/types/`.
3. **Pagination**: Most list endpoints return `PaginatedList<T>` with `PageSize` and `PageIndex` params.
4. **Filters**: Filters (e.g., `AgencyRequestFilter`) are passed as query params via `params: { ...filter }`.
5. **Nested Resources**: RESTful nesting, e.g., `/api/AgencyRequest/{requestId}/Worker/{workerId}/TimeSheet`.
6. **FormData for Uploads**: File uploads use `FormData` with `multipart/form-data` headers.
7. **Blob Downloads**: PDF/Excel returns are `Blob` with `responseType: 'blob'`.
8. **Auth Interception**: `apiService.ts` handles JWT refresh on 401; no auth calls needed in components.
9. **Pinia State**: Pinia stores mostly **filters only**, not full API data (data cached in component state or temp store).

---

## Important Notes for Future Development

- **API Versioning**: Endpoints use `/api/v2/`, `/api/v4/` prefixes; watch for endpoint duplication across versions.
- **Inconsistent Endpoints**: Some POST endpoints lack leading slash (e.g., `api/AgencyRequest` vs `/api/AgencyRequest`); axios baseURL handles this but watch in requests.
- **Missing Type Validation**: Some endpoints accept generic objects; refer to backend documentation or type files for valid fields.
- **Pagination Inconsistency**: Some endpoints use query params (`?PageSize={size}&PageIndex={page}`), others use standard `params: {}`. Standardize if refactoring.