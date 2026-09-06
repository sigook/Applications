# Sigook.Web API Map

Maps every file in `src/api/*.ts` (24 files) to its backend endpoints, request/response types, and Pinia store integrations.

**Key Patterns (stated once, apply everywhere):**
- API functions are plain TypeScript functions — components import and call them directly; there are no store dispatches for HTTP.
- All HTTP calls go through the `api` wrapper object from `@/security/apiService` (`api.get/post/put/patch/del`), which returns `response.data` directly. The raw axios instance handles auth headers and the 401 silent-refresh retry.
- Request/response types live in `src/types/*.ts`.
- Pinia stores (`src/stores/`, flat: `agency.ts`, `company.ts`, `worker.ts`, `security.ts`, `app.ts`) hold list **filters** and auth only — never API response data.
- List endpoints return `PaginatedList<T>`; filters are passed as `params: { ...filter }` (qs-serialized).
- File uploads use `FormData` + `multipart/form-data`; PDF/Excel downloads use `responseType: 'blob'`.
- Backend is Covenant.Api (.NET 8). Agency endpoints use lowercase module bases: `/api/agency/requests`, `/api/agency/companyprofiles`, `/api/agency/recruiting/...`, `/api/agency/sales/...`, `/api/agency/accounting/...`.

---

## 1. accountApi.ts

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `changeEmail(model)` | POST | `/api/Account/ChangeEmail` | `ChangeEmailRequest` | `void` | |
| `getEmail()` | GET | `/api/Account/GetEmail` | — | `GetEmailResponse` | Fetch current email |
| `deactivateAccount()` | PATCH | `/identity` | — | `void` | Account deactivation |

**Types:** `ChangeEmailRequest`, `GetEmailResponse` (`src/types/security`)

---

## 2. agencyApi.ts

Agency profile, personnel and agency switching.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyProfile()` | GET | `/api/Agency/Profile` | — | `AgencyDetail` | Current logged-in agency |
| `getAgency(id)` | GET | `/api/Agency/{id}` | — | `AgencyDetail` | |
| `getAgenciesList(filter)` | GET | `/api/Agency` | `AgencyListFilter` (params) | `PaginatedList<AgencyListItem>` | |
| `createAgency(model)` | POST | `/api/Agency` | `CreateAgencyModel` | `{ id: string }` | |
| `updateAgency(agency)` | PUT | `/api/Agency` | `AgencyDetail` | `void` | |
| `getAgencyPersonnel()` | GET | `/api/agency/personnel` | — | `AgencyPersonnelListItem[]` | Back-office users; `role` comes from IdentityServer and is null if that call fails |
| `createAgencyPersonnel(model)` | POST | `/api/agency/personnel` | `AgencyPersonnelCreateModel` | `void` | |
| `updateAgencyPersonnel(id, model)` | PUT | `/api/agency/personnel/{id}` | `AgencyPersonnelCreateModel` | `void` | Name, email and role. Policy `Admin` |
| `getAssignableRoles()` | GET | `/api/agency/personnel/Roles` | — | `string[]` | Roles current user may assign |
| `deleteAgencyPersonnel(id)` | DELETE | `/api/agency/personnel/{id}` | — | `void` | |
| `getAgencyLocations()` | GET | `/api/Agency/Location` | — | `AgencyLocationDetail[]` | |
| `createAgencyLocation(model)` | POST | `/api/Agency/Location` | `AgencyLocationDetail` | `{ id: string }` | |
| `updateAgencyLocation(id, model)` | PUT | `/api/Agency/Location/{id}` | `AgencyLocationDetail` | `void` | |
| `deleteAgencyLocation(id)` | DELETE | `/api/Agency/Location/{id}` | — | `void` | |
| `getPersonnelAgencies()` | GET | `/api/agency/personnel/Agencies` | — | `PersonnelAgencyItem[]` | Agencies user has access to |
| `switchPersonnelAgency(id)` | PUT | `/api/agency/personnel/Agencies/{id}` | — | `void` | Switch active agency context |

**Types:** `AgencyDetail`, `AgencyListFilter`, `AgencyListItem`, `AgencyLocationDetail`, `AgencyPersonnelCreateModel`, `AgencyPersonnelListItem`, `CreateAgencyModel`, `PersonnelAgencyItem` (`src/types/agency`)

**Pinia:** `useAgencyStore` holds `agency` + `agencyListFilter`; `personnelAgencies` for agency switching.

---

## 3. agencyCandidateApi.ts

Candidate pool (recruitment funnel before conversion to Worker).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCandidates(filter)` | GET | `/api/agency/candidates` | `AgencyCandidateFilter` (params) | `PaginatedList<Candidate>` | |
| `getAgencyCandidate(id)` | GET | `/api/agency/candidates/{id}` | — | `Candidate` | |
| `createAgencyCandidate(model, resume?)` | POST | `/api/agency/candidates` | `CreateCandidateModel` (multipart: `data` + resume file) | `{ id: string }` | |
| `updateAgencyCandidate(id, model)` | PUT | `/api/agency/candidates/{id}` | `CreateCandidateModel` | `void` | |
| `deleteAgencyCandidate(id)` | DELETE | `/api/agency/candidates/{id}` | — | `void` | |
| `updateAgencyCandidateRecruiter(id)` | PUT | `/api/agency/candidates/{id}/Recruiter` | null | `void` | Assign recruiter |
| `convertCandidateToWorker(id)` | POST | `/api/agency/candidates/{id}/convert-to-worker` | — | `{ id: string }` | Promote to worker |
| `addCandidatePhoneNumber(id, model)` | POST | `/api/agency/candidates/{id}/PhoneNumbers` | `CandidatePhoneNumberModel` | `{ id: string }` | |
| `deleteCandidatePhoneNumber(id, numberId)` | DELETE | `/api/agency/candidates/{id}/PhoneNumbers/{numberId}` | — | `void` | |
| `addCandidateSkill(id, model)` | POST | `/api/agency/candidates/{id}/Skills` | `CandidateSkillModel` | `{ id: string }` | |
| `deleteCandidateSkill(id, skillId)` | DELETE | `/api/agency/candidates/{id}/Skills/{skillId}` | — | `void` | |
| `getCandidateDocuments(id)` | GET | `/api/agency/candidates/{id}/Documents` | — | `PaginatedList<CandidateDocument>` | |
| `addCandidateDocument(id, model, file)` | POST | `/api/agency/candidates/{id}/Documents` | `CreateCandidateDocumentPayload` (multipart: `data` + file) | `string` (document id) | Callers refetch the list |
| `deleteCandidateDocument(id, docId)` | DELETE | `/api/agency/candidates/{id}/Documents/{docId}` | — | `void` | |
| `bulkAgencyCandidates(agencyId, file)` | POST | `/api/agency/candidates/bulk/{agencyId}` | FormData (multipart) | Blob | Excel import → error report |

**Types:** `Candidate`, `CandidateDocument`, `CreateCandidateDocumentPayload`, `AgencyCandidateFilter`, `CreateCandidateModel`, `CandidatePhoneNumberModel`, `CandidateSkillModel` (`src/types/candidate`)

**Pinia:** `agencyCandidateFilter` in `useAgencyStore`.

---

## 4. agencyCompanyApi.ts

**Largest API file.** Agency-side management of client companies. Bases: `companyProfilesUrl = /api/agency/companyprofiles`, list via `/api/agency/recruiting/companyprofiles`.

### Company CRUD
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createAgencyCompany(company)` | POST | `/api/agency/companyprofiles` | `Partial<CompanyProfileDetail>` | `{ id: string }` | |
| `getAgencyCompanies(filter)` | GET | `/api/agency/recruiting/companyprofiles` | `AgencyCompanyFilter` (params) | `PaginatedList<AgencyCompanyListItem>` | Recruiting-scoped list |
| `getAgencyCompany(id)` | GET | `/api/agency/companyprofiles/{id}` | — | `CompanyProfileDetail` | |
| `updateAgencyCompany(id, company)` | PUT | `/api/agency/companyprofiles/{id}` | `Partial<CompanyProfileDetail>` | `void` | |
| `updateCompanyVaccinationRequired(id, model)` | PUT | `/api/agency/companyprofiles/{id}/VaccinationRequired` | `VaccinationRequiredModel` | `void` | |
| `updateAgencyCompanyEmail(id, model)` | PUT | `/api/agency/companyprofiles/{id}/Email` | `{ newEmail: string }` | `void` | |
| `updateAgencyCompanyProfileLogo(id, model)` | PUT | `/api/agency/companyprofiles/{id}/Logo` | `Partial<CovenantFileModel>` | `void` | |
| `getAgencyCompanyProfileWithRequests()` | GET | `/api/agency/companyprofiles/company-with-requests` | — | `CompanyProfileListItem[]` | Companies + their requests |
| `getAgencyCompaniesList(searchTerm?)` | GET | `/api/agency/companyprofiles/companies-list` | `searchTerm` (query, optional) | `CatalogItem[]` | Typeahead for the sales dashboard client pickers; every company of the agency, ordered by name, scoped to the caller's agency (**not** sales-scoped) |
| `bulkAgencyCompanies(agencyId, file)` | POST | `/api/agency/companyprofiles/bulk/{agencyId}` | FormData (multipart) | Blob | Excel import |
| `getCompanyDeletionCheck(id)` | GET | `/api/agency/companyprofiles/{id}/deletion-check` | — | `CompanyDeletionCheck` | **superadmin only**; lists the related records that block deletion |
| `deleteAgencyCompany(id)` | DELETE | `/api/agency/companyprofiles/{id}` | — | `void` | **superadmin only**; 400 with the blocker detail when the company still has related records |

### Contact People
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyContactPerson(id)` | GET | `/api/agency/companyprofiles/{id}/ContactPeople` | — | `AgencyCompanyContactPerson[]` | |
| `createAgencyCompanyContactPerson(id, model)` | POST | `/api/agency/companyprofiles/{id}/ContactPeople` | `AgencyCompanyContactPerson` | `{ id: string }` | |
| `updateAgencyCompanyContactPerson(id, personId, model)` | PUT | `/api/agency/companyprofiles/{id}/ContactPeople/{personId}` | `AgencyCompanyContactPerson` | `void` | |
| `deleteAgencyCompanyContactPerson(id, personId)` | DELETE | `/api/agency/companyprofiles/{id}/ContactPeople/{personId}` | — | `void` | |

### Locations
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyLocation(id)` | GET | `/api/agency/companyprofiles/{id}/Locations` | — | `AgencyCompanyLocationModel[]` | |
| `createAgencyCompanyLocation(id, model)` | POST | `/api/agency/companyprofiles/{id}/Locations` | `AgencyCompanyLocationModel` | `{ id: string }` | |
| `updateAgencyCompanyLocation(id, locId, model)` | PUT | `/api/agency/companyprofiles/{id}/Locations/{locId}` | `AgencyCompanyLocationModel` | `void` | |
| `deleteAgencyCompanyLocation(id, locId)` | DELETE | `/api/agency/companyprofiles/{id}/Locations/{locId}` | — | `void` | |
| `updateAgencyCompanyContactInformation(id, model)` | PUT | `/api/agency/companyprofiles/{id}/ContactInformation` | `Partial<CompanyProfileDetail>` | `void` | |

### Job Positions
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyJobPositions(id)` | GET | `/api/agency/companyprofiles/{id}/JobPositions` | — | `AgencyCompanyJobPosition[]` | Roles + rates at company |
| `getAgencyCompanyJobPositionById(profileId, id)` | GET | `/api/agency/companyprofiles/{profileId}/JobPositions/{id}` | — | `AgencyCompanyJobPosition` | |
| `createAgencyCompanyJobPosition(id, model)` | POST | `/api/agency/companyprofiles/{id}/JobPositions` | `AgencyCompanyJobPosition` | `{ id: string }` | |
| `updateAgencyCompanyJobPosition(profileId, id, model)` | PUT | `/api/agency/companyprofiles/{profileId}/JobPositions/{id}` | `AgencyCompanyJobPosition` | `void` | |
| `deleteAgencyCompanyJobPosition(profileId, id)` | DELETE | `/api/agency/companyprofiles/{profileId}/JobPositions/{id}` | — | `void` | |
| `petitionAgencyCompanyJobPosition(id, model)` | POST | `/api/agency/companyprofiles/{id}/JobPositions/Petition` | `PetitionJobPositionPayload` | `void` | Request new position type |
| `deleteAgencyJobPosition(companyId, posId)` | DELETE | `/api/AgencyJobPosition/{companyId}/{posId}` | — | `void` | Alternate delete path |

### Company Documents
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyDocument(id, pagination)` | GET | `/api/agency/companyprofiles/{id}/Documents?PageSize={size}&PageIndex={page}` | — | `PaginatedList<CompanyProfileDocumentModel>` | |
| `createAgencyCompanyDocument(id, model, file)` | POST | `/api/agency/companyprofiles/{id}/Documents` | `CompanyProfileDocumentModel` (multipart: `data` + file) | `string` (document id) | Callers refetch the list |
| `deleteAgencyCompanyDocument(id, docId)` | DELETE | `/api/agency/companyprofiles/{id}/Documents/{docId}` | — | `void` | |

### Invoice Notes & Recipients
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getInvoiceNotes(id)` | GET | `/api/agency/companyprofiles/{id}/InvoiceNotes` | — | `InvoiceNotesModel` | Invoice footer text |
| `postInvoiceNotes(id, model)` | PUT | `/api/agency/companyprofiles/{id}/InvoiceNotes` | `InvoiceNotesModel` | `void` | |
| `getCompanyInvoiceRecipients(id)` | GET | `/api/agency/companyprofiles/{id}/InvoiceRecipients` | — | `InvoiceRecipientModel[]` | Email CCs for invoices |
| `postCompanyInvoiceRecipient(id, model)` | POST | `/api/agency/companyprofiles/{id}/InvoiceRecipients` | `InvoiceRecipientModel` | `{ id: string }` | |
| `updateCompanyInvoiceRecipient(id, recipId, model)` | PUT | `/api/agency/companyprofiles/{id}/InvoiceRecipients/{recipId}` | `InvoiceRecipientModel` | `void` | |
| `deleteCompanyInvoiceRecipient(id, recipId)` | DELETE | `/api/agency/companyprofiles/{id}/InvoiceRecipients/{recipId}` | — | `void` | |

### Company Settings
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `updatePermissionToSeeRequests(id, settings)` | PATCH | `/api/agency/companyprofiles/{id}/RequiresPermissionToSeeRequests` | `CompanyProfileSettingsUpdate` | `void` | Restrict request visibility |
| `updatePaidHolidays(id, settings)` | PATCH | `/api/agency/companyprofiles/{id}/PaidHolidays` | `CompanyProfileSettingsUpdate` | `void` | |
| `updateOvertime(id, settings)` | PATCH | `/api/agency/companyprofiles/{id}/Overtime` | `CompanyProfileSettingsUpdate` | `void` | |

### Company Users (Agency-Managed)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyUsers(id)` | GET | `/api/agency/companyprofiles/{id}/Users` | — | `CompanyUserModel[]` | |
| `createCompanyProfileUser(id, user)` | POST | `/api/agency/companyprofiles/{id}/Users` | `CreateCompanyUserModel` | `{ id: string }` | |
| `deleteCompanyProfileUser(id, userId)` | DELETE | `/api/agency/companyprofiles/{id}/Users/{userId}` | — | `void` | |

### Cross-Cutting
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `updateIsAsapRequests(model)` | PUT | `/api/agency/requests/is-asap` | `UpdateIsAsapRequestsPayload` | `void` | Mark requests as ASAP |

**Types:** `AgencyCompanyFilter`, `AgencyCompanyListItem`, `AgencyCompanyContactPerson`, `AgencyCompanyLocationModel`, `AgencyCompanyJobPosition`, `VaccinationRequiredModel`, `InvoiceNotesModel`, `InvoiceRecipientModel`, `PetitionJobPositionPayload`, `UpdateIsAsapRequestsPayload` (`src/types/agency`); `CompanyProfileDetail`, `CompanyProfileDocumentModel`, `CompanyProfileListItem`, `CompanyProfileSettingsUpdate`, `CompanyUserModel`, `CreateCompanyUserModel` (`src/types/company`)

**Pinia:** `agencyCompanyProfileFilter` in `useAgencyStore`.

**Business Logic:**
- Job positions define roles + wage rates (AgencyRate vs WorkerRate markup); used when creating requests.
- Invoice recipients auto-CC company contacts when the agency emails invoices.

---

## 5. agencyInvoiceApi.ts

Agency → company billing.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyInvoices(filter)` | GET | `/api/agency/accounting/Invoices` | `AgencyInvoiceFilter` (params) | `AgencyInvoiceListResponse` | |
| `previewAgencyInvoice(payload)` | POST | `/api/agency/accounting/Invoices/Preview` | `CreateAgencyInvoiceModel` | `InvoiceSummaryModel` | Preview before creation |
| `createAgencyInvoice(payload)` | POST | `/api/agency/accounting/Invoices` | `CreateAgencyInvoiceModel` | `void` | |
| `deleteAgencyInvoice(payload)` | DELETE | `/api/agency/accounting/Invoices/{invoiceId}` | `DeleteInvoicePayload` (in body via `data`) | `void` | |
| `downloadInvoicePdf(id)` | GET | `/api/agency/accounting/Invoices/{id}/pdf` | — | Blob | |
| `getPayStubsByInvoice(id)` | GET | `/api/agency/accounting/Invoices/{id}/paystubs` | — | `PayStubDeleteWarningItem[]` | Linked pay stubs (delete warning) |
| `sendInvoiceEmail(payload)` | POST | `/api/agency/accounting/Invoices/{invoiceId}/email` | FormData (multipart) | `void` | Email invoice + attachments |

**Types:** `AgencyInvoiceFilter`, `AgencyInvoiceListResponse`, `InvoiceSummaryModel`, `CreateAgencyInvoiceModel`, `DeleteInvoicePayload`, `PayStubDeleteWarningItem`, `SendInvoiceEmailPayload` (`src/types/accounting`)

**Pinia:** `agencyInvoiceFilter` in `useAgencyStore`.

---

## 6. agencyNoteApi.ts

Notes attached to Workers, Candidates, Companies, Requests and request workers. Every route hangs off the `/api/agency/...` module bases.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| **Worker Notes** | | | | | Read + Create only |
| `getWorkerProfileNotes(id, pagination)` | GET | `/api/agency/workers/{id}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createWorkerProfileNote(id, model)` | POST | `/api/agency/workers/{id}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| **Candidate Notes** | | | | | |
| `getCandidateNotes(id, pagination)` | GET | `/api/agency/candidates/{id}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createCandidateNote(id, model)` | POST | `/api/agency/candidates/{id}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| `deleteCandidateNote(id, noteId)` | DELETE | `/api/agency/candidates/{id}/Notes/{noteId}` | — | `void` | |
| **Company Notes** | | | | | Full CRUD |
| `getAgencyCompanyNotes(id, pagination)` | GET | `/api/agency/companyprofiles/{id}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createAgencyCompanyNote(id, model)` | POST | `/api/agency/companyprofiles/{id}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| `updateAgencyCompanyNote(id, noteId, model)` | PUT | `/api/agency/companyprofiles/{id}/Notes/{noteId}` | `NoteModel` | `void` | |
| `deleteAgencyCompanyNote(id, noteId)` | DELETE | `/api/agency/companyprofiles/{id}/Notes/{noteId}` | — | `void` | |
| **Request Notes** | | | | | Full CRUD |
| `getAgencyRequestNotes(id, pagination)` | GET | `/api/agency/requests/{id}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createAgencyRequestNote(id, model)` | POST | `/api/agency/requests/{id}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| `updateAgencyRequestNote(id, noteId, model)` | PUT | `/api/agency/requests/{id}/Notes/{noteId}` | `NoteModel` | `void` | |
| `deleteAgencyRequestNote(id, noteId)` | DELETE | `/api/agency/requests/{id}/Notes/{noteId}` | — | `void` | |
| **Request → Worker Notes** | | | | | Full CRUD (nested) |
| `getAgencyRequestWorkerNotes(requestId, workerId, pagination)` | GET | `/api/agency/requests/{requestId}/Workers/{workerId}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createAgencyRequestWorkerNote(requestId, workerId, model)` | POST | `/api/agency/requests/{requestId}/Workers/{workerId}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| `updateAgencyRequestWorkerNote(requestId, workerId, noteId, model)` | PUT | `/api/agency/requests/{requestId}/Workers/{workerId}/Notes/{noteId}` | `NoteModel` | `void` | |
| `deleteAgencyRequestWorkerNote(requestId, workerId, noteId)` | DELETE | `/api/agency/requests/{requestId}/Workers/{workerId}/Notes/{noteId}` | — | `void` | |

**Types:** `NoteModel`, `NoteItem`, `NotePagination`, `CreateNoteResponse` (`src/types/agency`)

---

## 7. agencyPayStubApi.ts

Pay stub generation and payroll administration.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyPayStubs(filter)` | GET | `/api/agency/accounting/PayStubs` | `AgencyPayStubFilter` (params) | `PaginatedList<AgencyPayStubListItem>` | |
| `downloadPayStubPdf(id)` | GET | `/api/agency/accounting/PayStubs/{id}/pdf` | — | Blob | |
| `deleteAgencyPayStub(id)` | DELETE | `/api/agency/accounting/PayStubs/{id}` | — | `void` | |
| `sendPayStubEmail(id)` | POST | `/api/agency/accounting/PayStubs/{id}/email` | — | `void` | Email to worker |
| `sendPayStubEmailBulk(payStubIds)` | POST | `/api/agency/accounting/PayStubs/email/bulk` | `{ payStubIds: string[] }` | `void` | Bulk email |
| `createAgencyPayStub(payload)` | POST | `/api/agency/accounting/PayStubs` | `CreatePayStubPayload` | `void` | Manual creation |
| `getWorkersReadyForPayStub()` | GET | `/api/agency/accounting/PayStubs/WorkersReadyForPayStub` | — | `WorkerReadyForPayStubModel[]` | Workers with approved timesheets |
| `generatePayStubs(workerIds)` | POST | `/api/agency/accounting/PayStubs/generate` | `string[]` | `void` | Batch generate from timesheets |
| `getPayrollSubcontractors(filter)` | GET | `/api/agency/accounting/reports/subcontractors` | `SubcontractorPayrollFilter` (params) | `PaginatedList<PayrollSubContractorListItem>` | |
| `downloadSubcontractorReport(weekEnding)` | GET | `/api/agency/accounting/reports/subcontractors/file` | `weekEnding` (param) | Blob | Excel |
| `deleteSubcontractorReport(weekEnding)` | DELETE | `/api/agency/accounting/reports/subcontractors` | `weekEnding` (param) | `void` | Deletes the whole week; releases its timesheets |
| `getSkipPayrollNumbers(filter)` | GET | `/api/agency/accounting/PayStubs/skip-payroll-number` | `{ searchTerm? }` (params) | `SkipPayrollNumberItem[]` | |
| `addSkipPayrollNumber(payload)` | POST | `/api/agency/accounting/PayStubs/skip-payroll-number` | `CreateSkipPayrollNumberPayload` | `void` | |

**Types:** `AgencyPayStubFilter`, `AgencyPayStubListItem`, `CreatePayStubPayload`, `CreateSkipPayrollNumberPayload`, `PayrollSubContractorListItem`, `SkipPayrollNumberItem`, `SubcontractorPayrollFilter`, `WorkerReadyForPayStubModel` (`src/types/accounting`)

**Pinia:** `agencyPayStubFilter` in `useAgencyStore`.

---

## 8. agencyReportApi.ts

Report generation and blob downloads.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `downloadAgencyReport(url, filter)` | GET | (dynamic url) | `ReportQueryParams` (params) | Blob | Generic blob downloader |
| `getWorkersReportDocument(requestId)` | GET | `/api/WorkersReportDocument/{requestId}/Document` | — | Blob | |
| `getJobPositionsHoursWorked(filter)` | GET | `/api/agency/accounting/reports/{companyProfileId}/job-positions` | `AgencyReportFilter & { companyProfileId }` (params) | `AgencyCompanyJobPosition[]` | Hours per position |
| `getHoursWorkedReport(filter)` | GET | `/api/agency/accounting/reports/hours-worked` | `AgencyReportFilter` (params) | `HoursWorkedResume` | |
| `getTimesheetsReport(filter)` | GET | `/api/agency/accounting/reports/timesheets/file` | `AgencyReportFilter` (params) | Blob | USA agencies |
| `getT4Report(filter)` | GET | `/api/agency/accounting/reports/t4` | `AgencyReportFilter` (params) | Blob | Tax form export |
| `getCraPayrollReport(filter)` | GET | `/api/agency/accounting/reports/cra-payroll` | `AgencyReportFilter` (params) | Blob | CRA export |
| `getPaymentReport(filter)` | GET | `/api/agency/accounting/reports/payments` | `AgencyReportFilter` (params) | `PaginatedList<WeeklyPayrollItem>` | Weekly payroll summary |
| `downloadWeeklyPayrollReport(weekEnding)` | GET | `/api/agency/accounting/reports/payments/file` | `weekEnding` (param) | Blob | Excel |

**Types:** `ReportQueryParams`, `AgencyReportFilter`, `AgencyCompanyJobPosition`, `HoursWorkedResume`, `WeeklyPayrollItem` (`src/types/agency`, `src/types/accounting`)

> Backend-only: `/api/agency/accounting/Reports/hours-worked/file` exists in the backend but has no frontend wrapper.

---

## 9. agencyRequestApi.ts

Core job request lifecycle. Bases: `requestsUrl = /api/agency/requests`, lists via `recruitingRequestsUrl = /api/agency/recruiting/requests`.

### Request CRUD
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `postAgencyRequest(model)` | POST | `/api/agency/requests` | `CreateAgencyRequestModel` | `AgencyRequestDetail` | |
| `getAgencyRequests(filter)` | GET | `/api/agency/recruiting/requests` | `AgencyRequestFilter` (params) | `AgencyRequestsPagedResponse` | Recruiting-scoped list |
| `getAllAgencyRequests(filter)` | GET | `/api/agency/recruiting/requests/all` | `AgencyRequestFilter` (params) | `AgencyRequestListItem[]` | Unpaged |
| `getAgencyRequest(id)` | GET | `/api/agency/requests/{id}` | — | `AgencyRequestDetail` | |
| `updateAgencyRequest(id, model)` | PUT | `/api/agency/requests/{id}` | `CreateAgencyRequestModel` | `AgencyRequestDetail` | |
| `cancelAgencyRequest(id, payload)` | PUT | `/api/agency/requests/{id}/Cancel` | `CancelRequestPayload` | `void` | Cancel + reason |
| `bulkCancelRequests(payload)` | PUT | `/api/agency/requests/bulk-cancel` | `BulkCancelRequestsPayload` | `BulkCancelRequestsResult` | Cancel many at once |
| `bulkUpdateRecruiters(payload)` | PUT | `/api/agency/requests/bulk-recruiters` | `BulkUpdateRecruitersPayload` | `void` | Replace recruiters on many at once; empty list unassigns. Admin/SuperAdmin only |
| `agencyRequestOpen(id)` | PUT | `/api/agency/requests/{id}/Open` | id (in body) | `void` | Reopen |
| `agencyRequestSendInvitation(id)` | POST | `/api/agency/requests/{id}/SendInvitation` | — | `void` | 120s timeout |
| `updateAgencyRequestIsAsap(id)` | PUT | `/api/agency/requests/{id}/IsAsap` | — | `void` | Toggle ASAP |
| `updateAgencyPunchCardVisibilityStatusInApp(id)` | PUT | `/api/agency/requests/{id}/PunchCardVisibilityStatusInApp` | — | `void` | |
| `getAgencyRequestShift(id)` | GET | `/api/agency/requests/{id}/Shift` | — | `RequestShiftModel` | |
| `updateAgencyRequestShift(id, model)` | PUT | `/api/agency/requests/{id}/Shift` | `RequestShiftModel` | `{ id; displayShift? }` | |
| `increaseWorkersQuantityByOne(id)` | PUT | `/api/agency/requests/{id}/IncreaseWorkersQuantityByOne` | — | `void` | |
| `reduceWorkersQuantityByOne(id)` | PUT | `/api/agency/requests/{id}/ReduceWorkersQuantityByOne` | — | `void` | |

### Request → Workers
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestsWorkers(filter)` | GET | `/api/agency/requests/{filter.requestId}/Workers` | `AgencyRequestWorkerFilter` (params) | `PaginatedList<AgencyRequestWorker>` | |
| `bookAgencyRequestWorker(requestId, workerId, model)` | POST | `/api/agency/requests/{requestId}/Workers/{workerId}/Book` | `BookWorkerModel` | `{ id: string }` | Assign worker |
| `updateAgencyRequestWorkerStartDate(requestId, id, model)` | PUT | `/api/agency/requests/{requestId}/Workers/{id}` | `BookWorkerModel` | `void` | |
| `rejectAgencyRequestWorker(requestId, workerId, model)` | PUT | `/api/agency/requests/{requestId}/Workers/{workerId}/Reject` | `RejectWorkerModel` | `void` | |

### Applicants
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `searchAgencyRequestApplicants(id, searchTerm)` | GET | `/api/agency/requests/{id}/Applicants/Search` | `searchTerm` (param) | `ApplicantSearchResult[]` | |
| `getAgencyRequestApplicant(filter)` | GET | `/api/agency/requests/{filter.requestId}/Applicants` | `AgencyRequestApplicantFilter` (params) | `PaginatedList<AgencyRequestApplicant>` | |
| `postAgencyRequestApplicant(id, model)` | POST | `/api/agency/requests/{id}/Applicants` | `CreateRequestApplicantModel` | `AgencyRequestApplicant` | |
| `deleteAgencyRequestApplicant(id, applicantId)` | DELETE | `/api/agency/requests/{id}/Applicants/{applicantId}` | — | `void` | |
| `updateAgencyRequestApplicant(id, applicantId, model)` | PUT | `/api/agency/requests/{id}/Applicants/{applicantId}` | `UpdateApplicantCommentsPayload` | `void` | |

### Request Contact People (RequestedBy / ReportTo)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestRequestedBy(id)` | GET | `/api/agency/requests/{id}/RequestedBy` | — | `PaginatedList<AgencyRequestPersonItem>` | Who requested (company side) |
| `postAgencyRequestRequestedBy(id, personId)` | POST | `/api/agency/requests/{id}/RequestedBy/{personId}` | — | `void` | |
| `deleteAgencyRequestRequestedBy(id, personId)` | DELETE | `/api/agency/requests/{id}/RequestedBy/{personId}` | — | `void` | |
| `getAgencyRequestReportTo(id)` | GET | `/api/agency/requests/{id}/ReportTo` | — | `PaginatedList<AgencyRequestPersonItem>` | Worker's supervisor |
| `postAgencyRequestReportTo(id, personId)` | POST | `/api/agency/requests/{id}/ReportTo/{personId}` | — | `void` | |
| `deleteAgencyRequestReportTo(id, personId)` | DELETE | `/api/agency/requests/{id}/ReportTo/{personId}` | — | `void` | |

> Recruiter assignment lives in the **Recruiting → Weekly Board** feature (per work day). See §20.

### Skills
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestSkill(id)` | GET | `/api/agency/requests/{id}/Skills` | — | `{ id, skill }[]` | |
| `postAgencyRequestSkill(id, model)` | POST | `/api/agency/requests/{id}/Skills` | `AgencyRequestSkillModel` | `{ id: string }` | |
| `deleteAgencyRequestSkill(id, skillId)` | DELETE | `/api/agency/requests/{id}/Skills/{skillId}` | — | `void` | |

### Sources (Job Boards)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestSources(id)` | GET | `/api/agency/requests/{id}/sources` | — | `RequestJobBoard[]` | Where the order is posted |
| `setAgencyRequestSources(id, items)` | PUT | `/api/agency/requests/{id}/sources` | `SetRequestJobBoardItem[]` | `void` | |

**Types:** `AgencyRequestFilter`, `AgencyRequestsPagedResponse`, `AgencyRequestListItem`, `AgencyRequestDetail`, `CreateAgencyRequestModel`, `RequestShiftModel`, `CancelRequestPayload`, `BulkCancelRequestsPayload`, `BulkCancelRequestsResult`, `AgencyRequestWorkerFilter`, `AgencyRequestWorker`, `BookWorkerModel`, `RejectWorkerModel`, `AgencyRequestApplicantFilter`, `AgencyRequestApplicant`, `ApplicantSearchResult`, `CreateRequestApplicantModel`, `UpdateApplicantCommentsPayload`, `AgencyRequestSkillModel`, `AgencyRequestPersonItem`, `RequestJobBoard`, `SetRequestJobBoardItem` (`src/types/agency`)

**Pinia:** `agencyRequestFilter` in `useAgencyStore`.

---

## 10. agencyRunnerApi.ts

Runners — recruiting pipeline of prospects per request (list, status transitions, interviews). Base: `/api/agency/requests/{requestId}/Runners`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRunners(requestId, filter)` | GET | `/api/agency/requests/{requestId}/Runners` | `AgencyRunnerFilter` (params) | `PaginatedList<RunnerListItem>` | |
| `searchAgencyRunnerProspects(requestId, searchTerm)` | GET | `/api/agency/requests/{requestId}/Runners/Search` | `searchTerm` (param) | `ApplicantSearchResult[]` | Search workers to add (workers only) |
| `getAgencyRunner(requestId, id)` | GET | `/api/agency/requests/{requestId}/Runners/{id}` | — | `RunnerDetail` | |
| `createAgencyRunner(requestId, model)` | POST | `/api/agency/requests/{requestId}/Runners` | `CreateRunnerModel` | `string` (id) | |
| `deleteAgencyRunner(requestId, id)` | DELETE | `/api/agency/requests/{requestId}/Runners/{id}` | — | `void` | Deletes the runner with its history and interviews; used by the Runners tab **and** the weekly board |
| `changeRunnerStatus(requestId, id, model)` | PUT | `/api/agency/requests/{requestId}/Runners/{id}/Status` | `ChangeRunnerStatusModel` | `void` | Pipeline transition |
| `createRunnerInterview(requestId, id, model)` | POST | `/api/agency/requests/{requestId}/Runners/{id}/Interview` | `CreateRunnerInterviewModel` | `string` (id) | Schedule interview |
| `rescheduleRunnerInterview(requestId, id, interviewId, model)` | PUT | `/api/agency/requests/{requestId}/Runners/{id}/Interview/{interviewId}/Reschedule` | `RescheduleRunnerInterviewModel` | `void` | |

**Types:** `AgencyRunnerFilter`, `RunnerListItem`, `RunnerDetail`, `CreateRunnerModel`, `ChangeRunnerStatusModel`, `CreateRunnerInterviewModel`, `RescheduleRunnerInterviewModel` (`src/types/runner`); `ApplicantSearchResult` (`src/types/agency`)

**UI:** `components/agency_request/Runners.vue` + `components/runner/*` (CreateRunner, RunnerStatusModal, RunnerInterviewModal, RunnerHistoryModal). See WORKFLOWS.md → Runner Pipeline Flow.

---

## 11. agencyTimeSheetApi.ts

Timesheets per request/worker. Base: `/api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyWorkerTimeSheet(requestId, workerId)` | GET | `/api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets` | — | `TimeSheetListItem[]` | All entries |
| `getAgencyWorkerTimeSheetByDate(requestId, workerId, date)` | GET | `/api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets` | `{ startDate, endDate }` (params) | `TimeSheetListItem[]` | Date range |
| `postAgencyWorkerTimeSheet(requestId, workerId, model)` | POST | `/api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets` | `TimeSheetModel` | `{ id: string }` | |
| `updateAgencyWorkerTimeSheet(requestId, workerId, id, model)` | PUT | `/api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}` | `TimeSheetModel` | `void` | |
| `deleteAgencyWorkerTimeSheet(requestId, workerId, id)` | DELETE | `/api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}` | — | `void` | |
| `getAgencyTimeSheetUsages(requestId, workerId, id)` | GET | `/api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}/Usages` | — | `TimeSheetUsagesModel` | Which invoices/pay stubs use it |

**Types:** `TimeSheetListItem`, `TimeSheetModel`, `TimeSheetUsagesModel` (`src/types/company`)

---

## 12. agencyWorkerApi.ts

Worker profile management from the agency's perspective.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyWorkers(filter)` | GET | `/api/agency/workers` | `AgencyWorkerFilter` (params) | `PaginatedList<AgencyWorkerListItem>` | |
| `getAgencyWorkersDropdown(filter)` | GET | `/api/agency/workers/Dropdown` | `{ searchTerm }` (params) | `AgencyWorkerDropdownItem[]` | Autocomplete¹ |
| `getAgencyWorker(id)` | GET | `/api/agency/workers/{id}` | — | `WorkerProfile` | |
| `updateApprovedToWork(id)` | PUT | `/api/agency/workers/{id}/ApprovedToWork` | — | `void` | Toggle |
| `updateAgencyWorkerProfileDNU(id)` | PUT | `/api/agency/workers/{id}/Dnu` | — | `void` | Toggle Do-Not-Use |
| `updateAgencyWorkerContractor(id)` | PUT | `/api/agency/workers/{id}/IsContractor` | — | `void` | Toggle |
| `updateAgencyWorkerSubContractor(id)` | PUT | `/api/agency/workers/{id}/IsSubcontractor` | — | `void` | Toggle |
| `updateWorkerProfileTaxCategory(payload)` | PUT | `/api/agency/workers/{id}/tax-category` | `UpdateWorkerProfileFieldsPayload` | `void` | |
| `updateWorkerProfileTaxRate(payload)` | PUT | `/api/agency/workers/{id}/tax-rate` | `UpdateWorkerProfileFieldsPayload` | `void` | |
| `updateWorkerProfileExternalId(payload)` | PUT | `/api/agency/workers/{id}/ExternalId` | `UpdateWorkerProfileFieldsPayload` | `void` | |
| `updateWorkerProfileWcCode(payload)` | PUT | `/api/agency/workers/{id}/WcCode` | `UpdateWorkerProfileFieldsPayload` | `void` | |
| `updateAgencyWorkerEmail(id, model)` | PUT | `/api/agency/workers/{id}/Email` | `UpdateWorkerEmailModel` | `void` | |
| `agencyCommentWorker(id, comment)` | POST | `/api/agency/workers/{id}/Comments` | `AgencyWorkerCommentModel` | `void` | |
| `getAgencyWorkerProfileRequestHistory(id, pagination)` | GET | `/api/agency/workers/{id}/RequestHistory?PageSize={size}&PageIndex={page}` | — | `PaginatedList<AgencyWorkerRequestHistoryItem>` | Past assignments |
| `getAgencyWorkerProfileHolidays(id)` | GET | `/api/agency/workers/{id}/Holidays` | — | `AgencyWorkerHoliday[]` | |
| `addUpdateAgencyWorkerProfileHolidays(id, data)` | POST | `/api/agency/workers/{id}/Holidays` | `AgencyWorkerHoliday` | `void` | |
| `addNewHoliday(id, payload)` | POST | `/api/agency/workers/{id}/Holidays/new-holiday` | `AddNewHolidayPayload` | `void` | Bulk holiday add |

**Types:** `AgencyWorkerFilter`, `AgencyWorkerListItem`, `AgencyWorkerDropdownItem`, `AgencyWorkerCommentModel`, `UpdateWorkerEmailModel`, `UpdateWorkerProfileFieldsPayload`, `AgencyWorkerHoliday`, `AddNewHolidayPayload`, `AgencyWorkerRequestHistoryItem` (`src/types/agency`); `WorkerProfile` (`src/types/worker`)

**Pinia:** `agencyWorkerProfileFilter` in `useAgencyStore`.

**Business Logic:** contractor/subcontractor flags affect payroll treatment; tax category/rate drive withholdings; holidays feed holiday pay.

¹ The URL string at `agencyWorkerApi.ts:23` is missing its leading slash (`'api/agency/workers/Dropdown'`) — it only works because axios concatenates it onto the baseURL.

---

## 13. catalogApi.ts

Reference data (lookup tables).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getGenders()` | GET | `/api/Catalog/gender` | — | `Gender[]` | |
| `getIdentificationTypes()` | GET | `/api/Catalog/identificationType` | — | `IdentificationType[]` | |
| `getAvailability()` | GET | `/api/Catalog/availability` | — | `Availability[]` | Full-time, Part-time, ... |
| `getAvailabilityTimes()` | GET | `/api/Catalog/availabilityTime` | — | `AvailabilityTime[]` | Morning, Evening, ... |
| `getDays()` | GET | `/api/Catalog/day` | — | `Day[]` | |
| `fetchLifts()` | GET | `/api/Catalog/lift` | — | `Lift[]` | |
| `fetchLanguages()` | GET | `/api/Catalog/language` | — | `Language[]` | |
| `getWsibGroups()` | GET | `/api/Catalog/wsibgroup` | — | `WsibGroup[]` | WSIB classifications (Canada) |
| `getSkills()` | GET | `/api/Catalog/skills` | — | `Skill[]` | |
| `getIndustries()` | GET | `/api/Catalog/industry` | — | `Industry[]` | |
| `getReasonCancellationRequest()` | GET | `/api/Catalog/reasonCancellationRequest` | — | `CancellationReason[]` | |
| `getCompanyStatus()` | GET | `/api/Catalog/companyStatus` | — | `CatalogItem<number>[]` | |
| `getSources()` | GET | `/api/Catalog/source` | — | `Source[]` | Candidate/worker sources |
| `getSourcesForRequests()` | GET | `/api/Catalog/source/requests` | — | `Source[]` | Job boards for requests |
| `getTaxCategories()` | GET | `/api/Catalog/tax-categories` | — | `TaxCategory[]` | |
| `addIndustry(industry)` | POST | `/api/Catalog/industry` | `{ id?, value }` | `Industry` | Add custom industry |

**Types:** from `src/types/common`.

---

## 14. companyApi.ts

Company portal (client) view of their profile, requests and workers — plus the agency sales module's deals & company-interactions CRUD (bases `/api/agency/sales/deals`, `/api/agency/sales/companyinteractions`).

### Profile & Locations
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyProfile()` | GET | `/api/Company` | — | `CompanyProfileDetail` | Current company |
| `updateProfile(id, company)` | PUT | `/api/Company/{id}` | `CompanyProfileDetail` | `void` | |
| `registerCompany(company)` | POST | `/api/Company` | `CompanyProfileDetail` | `void` | |
| `getProfileLocations()` | GET | `/api/company/profile/Locations` | — | `CompanyProfileLocationDetail[]` | |
| `createProfileLocation(model)` | POST | `/api/company/profile/Locations` | `CompanyProfileLocationDetail` | `void` | |
| `updateProfileLocation(id, model)` | PUT | `/api/company/profile/Locations/{id}` | `CompanyProfileLocationDetail` | `void` | |
| `deleteProfileLocation(id)` | DELETE | `/api/company/profile/Locations/{id}` | — | `void` | |

### Job Positions
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyJobPositions()` | GET | `/api/company/profile/JobPositions` | — | `CompanyProfileJobPositionRate[]` | Roles + rates |
| `getCompanyJobPositionById(id)` | GET | `/api/company/profile/JobPositions/{id}` | — | `CompanyProfileJobPositionRate` | |
| `requestNewPosition(data)` | POST | `/api/company/profile/JobPositions/request-new-position` | `{ title, name, email, phone, message, subject }` | `void` | |

### Requests
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getRequests(filter)` | GET | `/api/company/requests` | `CompanyRequestFilter` (params) | `PaginatedList<CompanyRequestListItem>` | |
| `getRequest(id)` | GET | `/api/company/requests/{id}` | — | `CompanyRequestListItem` | |
| `createRequest(request)` | POST | `/api/company/requests` | `CreateAgencyRequestModel` | `{ id: string }` | |
| `editRequest(id, model)` | PUT | `/api/company/requests/{id}` | `{ requirements }` | `void` | |
| `getRequestShift(id)` | GET | `/api/company/requests/{id}/Shift` | — | `RequestShiftModel` | |
| `cancelRequest(id, reasonId, otherReason)` | PUT | `/api/company/requests/{id}/Cancel` | `{ cancellationReasonId, otherCancellationReason }` | `void` | |

### Request Workers
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getRequestWorkers(filter)` | GET | `/api/company/requests/{filter.requestId}/Workers` | `CompanyRequestWorkerFilter` (params) | `PaginatedList<CompanyRequestWorker>` | |
| `getRequestWorker(requestId, workerId)` | GET | `/api/company/requests/{requestId}/Workers/{workerId}` | — | `CompanyRequestWorker` | |
| `rejectCompanyRequestWorker(requestId, workerId, model)` | PUT | `/api/company/requests/{requestId}/Workers/{workerId}/Reject` | `CommentsModel` | `void` | |
| `requestAnotherWorker(requestId, comment)` | POST | `/api/company/requests/{requestId}/Workers/RequestNewWorker` | `CommentsModel` | `void` | Ask for replacement |

### TimeSheet
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyWorkerTimeSheetByDate(requestId, workerId, date)` | GET | `/api/company/requests/{requestId}/Workers/{workerId}/TimeSheets` | `{ startDate, endDate }` (params) | `TimeSheetListItem[]` | |
| `postCompanyWorkerTimeSheet(requestId, workerId, model)` | POST | `/api/company/requests/{requestId}/Workers/{workerId}/TimeSheets` | `TimeSheetModel` | `{ id: string }` | |
| `validateHoursTimeSheet(requestId, workerId, id, model)` | PUT | `/api/company/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}` | `TimeSheetModel` | `void` | |
| `validateAllHoursTimeSheet(requestId, workerId)` | PUT | `/api/company/requests/{requestId}/Workers/{workerId}/TimeSheets` | — | `void` | Validate all |
| `updateCompanyRequestWorkerTimeSheet(requestId, workerId, id, model)` | PUT | `/api/company/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}` | `TimeSheetModel` | `void` | |
| `deleteCompanyWorkerTimeSheet(requestId, workerId, id)` | DELETE | `/api/company/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}` | — | `void` | |
| `companyTimeSheetClockIn(requestId, workerId, model)` | POST | `/api/company/requests/{requestId}/Workers/{workerId}/TimeSheets/ClockIn` | `ClockInModel` | `ClockInResult` | GPS clock-in |

### Comments / Users / Contacts / Invoices
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `companyCommentWorker(id, comment)` | POST | `/api/company/workers/{id}/Comments` | `CommentsModel` | `void` | |
| `getCompanyUser()` | GET | `/api/company/Users` | — | `CompanyUserModel[]` | |
| `getCompanyUserDetail()` | GET | `/api/company/Users/detail` | — | `CompanyUserModel` | Current user |
| `createCompanyUser(model)` | POST | `/api/company/Users` | `CreateCompanyUserModel` | `void` | |
| `updateCompanyUser(id, user)` | PUT | `/api/company/Users/{id}` | `CompanyUserModel` | `void` | |
| `deleteCompanyUser(id)` | DELETE | `/api/company/Users/{id}` | — | `void` | |
| `getContactPeople()` | GET | `/api/company/profile/ContactPeople` | — | `CompanyContactPersonModel[]` | |
| `saveContactPerson(model)` | POST | `/api/company/profile/ContactPeople` | `CompanyContactPersonModel` | `void` | Create/update |
| `deleteContactPerson(id)` | DELETE | `/api/company/profile/ContactPeople/{id}` | — | `void` | |
| `getCompanyInvoice(filter)` | GET | `/api/company/accounting/Invoices` | `CompanyInvoiceFilter` (params) | `PaginatedList<CompanyInvoiceListItem>` | |
| `getCompanyInvoiceDetail(id)` | GET | `/api/company/accounting/Invoices/{id}` | — | `InvoiceSummaryModel` | |
| `getCompanyRequestTimeSheetFile(requestId)` | GET | `/api/company/requests/{requestId}/TimeSheets/File` | — | Blob | Excel punch-card export, ownership-checked server-side |

### Sales — Deals
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getDeals(filter)` | GET | `/api/agency/sales/deals` | `DealFilter` (params) | `PaginatedList<Deal>` | Sales users only see deals they own; admin/superadmin unscoped, with optional `ownerId` filter |
| `createDeal(model, file?)` | POST | `/api/agency/sales/deals` | `CreateDealModel` (multipart: `data` + optional document) | `string` (id) | `OwnerId` forced server-side |
| `updateDeal(id, model)` | PUT | `/api/agency/sales/deals/{id}` | `UpdateDealModel` | `void` | Owner-checked for sales; admin/superadmin may edit any |
| `deleteDeal(id)` | DELETE | `/api/agency/sales/deals/{id}` | — | `void` | Owner-checked for sales; admin/superadmin may delete any |

### Sales — Company Interactions
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyInteractions(filter)` | GET | `/api/agency/sales/companyinteractions` | `CompanyInteractionFilter` (params) | `PaginatedList<CompanyInteraction>` | Sales users only see interactions they own; admin/superadmin unscoped, with optional `ownerId` filter |
| `createCompanyInteraction(model)` | POST | `/api/agency/sales/companyinteractions` | `CreateCompanyInteractionModel` | `string` (id) | `OwnerId` forced server-side |
| `updateCompanyInteraction(id, model)` | PUT | `/api/agency/sales/companyinteractions/{id}` | `UpdateCompanyInteractionModel` | `void` | Owner-checked for sales; admin/superadmin may edit any |
| `deleteCompanyInteraction(id)` | DELETE | `/api/agency/sales/companyinteractions/{id}` | — | `void` | Owner-checked for sales; admin/superadmin may delete any |

**Types:** from `src/types/company` (+ `InvoiceSummaryModel` from `src/types/accounting`); deals/interactions enums + models at `src/types/company.ts:344-589`.

> **Enum mirror gotcha.** The API serializes enums as **ints** (System.Text.Json, no `JsonStringEnumConverter`), so `DealType`, `DealStatus`, `InteractionType`, `InteractionPurpose` and `InteractionStatus` in `src/types/company.ts` must match `Covenant.Common/Enums/` **numerically** — adding or reordering a member on one side without the other silently mislabels records. Label/color/icon maps (`DEAL_TYPE_LABELS`, `INTERACTION_TYPE_ICONS`, …) live next to the enums; `InteractionType.Mail` is labelled "Email" in the UI. Meaning of each value: `.docs/business/SALES_MODULE.md`.

**Pinia:** `companyRequestFilter` in `useCompanyStore`.

**UI (sales sections):** `pages/agency/Dashboard.vue`, `pages/agency/SalesDeals.vue`, `pages/agency/SalesInteractions.vue`. All three create/edit through `components/sales_dashboard/SalesCreateModal.vue` — a Buefy modal that switches between three forms by `kind` (`SalesCreateKind = 'interaction' | 'client' | 'deal'`, `src/types/sales.ts`), shows create vs edit titles, offers Delete behind a confirm dialog in edit mode, and emits `saved` so the host page reloads its lists:

| Kind | Form | API functions | Notes |
|------|------|---------------|-------|
| `interaction` | `SalesInteractionForm` | `createCompanyInteraction` / `updateCompanyInteraction` / `deleteCompanyInteraction` (above) | Client picker via `getAgencyCompaniesList` (agencyCompanyApi.ts); client is read-only when editing |
| `deal` | `SalesDealForm` | `createDeal` / `updateDeal` / `deleteDeal` (above) | Create is `multipart/form-data` with an optional document (`utils/multipart.ts` + `utils/fileNaming.ts`) |
| `client` | `SalesClientForm` | `createAgencyCompany` (agencyCompanyApi.ts) | Create-only from the modal; catalogs via `getIndustries` / `getCompanyStatus` (catalogApi.ts); ordinary company endpoint, so sales auto-assignment applies |

The deals/interactions CRUD lives here in `companyApi.ts`, **not** in `salesApi.ts`. Backend: `Covenant.Api/Covenant.Api/Controllers/Sigook/Agency/Sales/{DealsController,CompanyInteractionsController}.cs`, `[Authorize(Policy = PolicyConfiguration.Sales)]`; entities in ENTITIES_RELATIONSHIPS.md ("Sales entities").

**Business Logic:** timesheet validation by the company feeds invoicing; clock-in captures GPS + time. Deals/interactions are owner-scoped end-to-end for sales users (admin/superadmin unscoped) — stricter than the list-only scoping of orders/clients (ROLES_PERMISSIONS.md). Concepts, catalogs and deal lifecycle: `.docs/business/SALES_MODULE.md`.

---

## 15. locationApi.ts

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCountries()` | GET | `/api/Location/country` | — | `Country[]` | |
| `getProvinces(countryId)` | GET | `/api/Location/province/{countryId}` | — | `Province[]` | |
| `getCities(provinceId)` | GET | `/api/Location/city/{provinceId}` | — | `City[]` | |
| `createCity(city)` | POST | `/api/Location/city` | `{ value, code?, province: { id } }` | `City` | Add custom city |
| `updateProvinceSettings(provinceId, settings)` | PUT | `/api/Location/province/{provinceId}/settings` | `ProvinceSettings` | `void` | Global provincial rules; Policy `Admin` |
| `getLocationTax(locationId)` | GET | `/api/Location/{locationId}/tax` | — | `LocationTax \| null` | Tax % per location (admin) |
| `upsertLocationTax(locationId, model)` | PUT | `/api/Location/{locationId}/tax` | `LocationTax` | `void` | |

**Types:** `Country`, `Province`, `City`, `LocationTax` (`src/types/common`)

Province settings (`ProvinceSettingsModal.vue`) persist through `updateProvinceSettings` directly — the modal calls the API on save and then emits to `Address.vue`, which only refreshes the local display model. They are **global per province** (admin-only), not part of the company-location save.

---

## 16. notificationApi.ts

In-app notification bell (agency roles). A single aggregated call returns every notification kind in one payload.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getNotifications()` | GET | `/api/agency/Notifications` | — | `NotificationsResponse` | Grouped by type (today only `workersToReview: RunnerStartingToday[]`) |

**Types:** `NotificationsResponse`, `AppNotification`, `NotificationGroup`, `NotificationType` (`src/types/notification`); `RunnerStartingToday` (`src/types/runner`)

**Composable:** `useNotifications` loads once, maps each typed list to generic `AppNotification[]` grouped by `NotificationType`.

**UI:** `SidebarLogged.vue` owns the load; the user avatar at the sidebar footer shows a red dot and the user menu opens with a "Notifications" section (per-type count, or "Nothing to review"); `WorkerAttendanceReview` type links to `/recruiting/attendance-review`, each row links to the order's Punch Card.

**Extensibility:** a new kind = new list on backend `NotificationsModel` + new `NotificationType`/label/route + a mapper in `useNotifications`. See WORKFLOWS.md → Runner Pipeline Flow → STEP 5.

---

## 17. salesApi.ts

Sales-role-scoped lists (parallel to the recruiting-scoped lists in agencyRequestApi/agencyCompanyApi). Bases: `/api/agency/sales/requests`, `/api/agency/sales/companyprofiles`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getSalesRequests(filter)` | GET | `/api/agency/sales/requests` | `AgencyRequestFilter` (params) | `AgencyRequestsPagedResponse` | |
| `getSalesRequestsFile(filter)` | GET | `/api/agency/sales/requests/File` | `AgencyRequestFilter` (params) | Blob | Excel export |
| `getSalesCompanies(filter)` | GET | `/api/agency/sales/companyprofiles` | `AgencyCompanyFilter` (params) | `PaginatedList<AgencyCompanyListItem>` | |
| `getSalesCompaniesFile(filter)` | GET | `/api/agency/sales/companyprofiles/File` | `AgencyCompanyFilter` (params) | Blob | Excel export |

**Types:** `AgencyRequestFilter`, `AgencyRequestsPagedResponse`, `AgencyCompanyFilter`, `AgencyCompanyListItem` (`src/types/agency`)

**Usage:** `/sales/requests` and `/sales/companies` pages; shared detail pages resolve their base path via `useModuleBase`. The sales dashboard's interaction/deal client pickers use `getAgencyCompaniesList` (agencyCompanyApi.ts) instead — note that endpoint is **not** sales-scoped, so a sales user picks from every company of the agency.

---

## 18. salesDashboardApi.ts

**Static — no backend endpoint.** Serves the sales dashboard summary from `src/data/sales/salesDashboard.json`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getSalesDashboard()` | — | — (resolves `src/data/sales/salesDashboard.json`) | — | `SalesDashboardModel` | `GET /api/agency/sales/dashboard` does **not** exist in the backend; the equivalent call sits commented in the file header |

**Types:** `SalesDashboardModel` + blocks (`src/types/sales.ts:56-64`) — `period`, `clients`, `deals`, `dealsClosed.{week,month,quarter}`, `goal`, `pipeline[]` (by `DealStatus`), `activity[]` (by `InteractionType`); enum fields as numeric values.

**UI:** `pages/agency/Dashboard.vue` (layout in SIGOOK_WEB_STRUCTURE.md). KPI definitions: `.docs/business/SALES_MODULE.md`.

### Static vs live

The payload above (period label, Clients card, "Deals closed" chart, goal donut, pipeline/activity meters) is frozen at Q3 2026 (`asOf` 2026-07-12). Only the Interactions and Deals lists and every create/edit/delete flow hit the backend:

```
Dashboard.vue onMounted (Dashboard.vue:209-221)
├─ getSalesDashboard()             → src/data/sales/salesDashboard.json        [STATIC]
├─ getCompanyInteractions({...6})  → GET /api/agency/sales/companyinteractions [LIVE]
├─ getDeals({...6})                → GET /api/agency/sales/deals               [LIVE]
└─ useCurrentAgent.loadAgentName() → GET /api/agency/personnel                 [LIVE]

SalesCreateModal @saved → onSaved (Dashboard.vue:204-207)
└─ reloads interactions + deals only — the static blocks never refresh
```

> **Half-live refresh.** Saving a deal or interaction does not move the pipeline meters, the goal donut, the "Deals closed" chart or the Clients card — those render the frozen JSON. The range tabs are client-side only: they index into the pre-baked `dealsClosed.{week,month,quarter}` arrays; no request is made.

### Where the summary comes from

`src/api/salesDashboardApi.ts` holds the whole static path:

```ts
import dashboardData from '@/data/sales/salesDashboard.json';
import type { SalesDashboardModel } from '@/types/sales';

export function getSalesDashboard(): Promise<SalesDashboardModel> {
  return Promise.resolve(dashboardData as unknown as SalesDashboardModel);
}
```

The file header also carries the equivalent `api.get<SalesDashboardModel>('/api/agency/sales/dashboard')` call, commented out; the JSON is shaped exactly like that response, so the function signature and every caller are independent of which body is active.

`Covenant.Api/Covenant.Api/Controllers/Sigook/Agency/Sales/` contains only `CompanyProfilesController`, `RequestsController`, `DealsController` and `CompanyInteractionsController`; the dashboard endpoint is absent from `openapi.json`. The payload shape fixes two contract decisions: `dealsClosed` ships all three ranges pre-aggregated (no range query param), and `period` is computed server-side.

**Consequences of the static payload:**
- Period label and every chart render the same numbers regardless of today.
- The quarterly target cannot be stored or configured anywhere.
- Saves refresh only the two live lists; creating a client refreshes nothing (the Clients card reads the static JSON).
- Range tabs never hit the network.
- The client form is create-only from the modal (no edit/delete path).

---

## 19. sharedApi.ts

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `unsubscribe(model)` | POST | `/api/EmailPreferences/Unsubscribe` | `UnsubscribeRequest` | `void` | No auth required |

---

## 20. weeklyBoardApi.ts — Recruiting Weekly Board

Board where the agency assigns orders to recruiters per work day, and each recruiter records the runners they sent. It is the only place recruiters are assigned. Admin board (`getWeeklyBoard`) shows all recruiters with counts; recruiter board (`getRecruiterWeeklyBoard`) is scoped to the recruiter from the token and includes the runners sent. Base: `/api/agency/recruiting/WeeklyBoard`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getWeeklyBoard(filter)` | GET | `/api/agency/recruiting/WeeklyBoard` | `WeeklyBoardFilter` (params) | `WeeklyBoard` | Admin board grouped by recruiter |
| `getRecruiterWeeklyBoard(filter)` | GET | `/api/agency/recruiting/WeeklyBoard/mine` | `WeeklyBoardFilter` (params) | `RecruiterWeeklyBoard` | Current recruiter's board + sent workers |
| `getRequestRunners(requestId)` | GET | `/api/agency/recruiting/WeeklyBoard/{requestId}/runners` | — | `WeeklyBoardRunner[]` | All runners sent to an order across every recruiter/day (whole history) |
| `assignRecruiters(payload)` | POST | `/api/agency/recruiting/WeeklyBoard` | `AssignRecruitersPayload` | `void` | Assign recruiter(s) to an order per day |
| `unassignRecruiter(payload)` | DELETE | `/api/agency/recruiting/WeeklyBoard` | `UnassignRecruiterPayload` (params) | `void` | Remove a day assignment |
| `moveAssignment(payload)` | POST | `/api/agency/recruiting/WeeklyBoard/move` | `MoveAssignmentPayload` | `void` | Move assignment to another recruiter/day (keeps its runners; drag & drop) |
| `addRunner(payload)` | POST | `/api/agency/recruiting/WeeklyBoard/runner` | `AddRunnerPayload` | `void` | Recruiter sends a runner (delegates to `IRunnerService.CreateRunner`); adds a request note `"{workerName} was sent"` |

**Types:** `WeeklyBoard`, `RecruiterWeeklyBoard`, `WeeklyBoardRecruiterRow`, `WeeklyBoardAssignment`, `WeeklyBoardDispatch`, `WeeklyBoardFilter`, `AssignRecruitersPayload`, `UnassignRecruiterPayload`, `MoveAssignmentPayload`, `DispatchWorkersPayload`, `RemoveWorkerPayload` (`src/types/weeklyBoard`)

**UI:** `pages/agency/WeeklyBoard.vue` + `components/weekly_board/*`.

---

## 21. userNotificationApi.ts

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getUserNotifications()` | GET | `/api/UserNotification` | — | `UserNotificationItem[]` | In-app inbox |
| `updateUserNotification(model)` | PUT | `/api/UserNotification` | `UserNotificationItem` | `void` | Mark read/update |

**Types:** `UserNotificationItem` (`src/types/common`)

---

## 22. websiteApi.ts

Public landing site endpoints (no auth).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getJobs(filter)` | GET | `/api/WebSite/jobs` | `JobSearchFilter` (params; `countries` defaults to `['USA','CA']`) | `JobViewModel[]` | Public job search |
| `submitContactForm(contact)` | POST | `/api/WebSite/contact` | `ContactForm` | `void` | |
| `submitCandidate(formData)` | POST | `/api/WebSite/candidate` | FormData (multipart) | `void` | Public candidate apply |

**Types:** `JobSearchFilter`, `JobViewModel`, `ContactForm` (`src/types/website`)

---

## 23. workerApi.ts

**Large.** Worker portal: profile build, applications, timesheet.

### Requests (Job Applications)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getJobs(filter)` | GET | `/api/WorkerRequest` | `WorkerRequestFilter` (params) | `PaginatedList<WorkerRequestListItem>` | Available jobs |
| `getWorkerRequest(id)` | GET | `/api/WorkerRequest/{id}` | — | `WorkerRequestDetail` | |
| `workerRequestApplySelf(requestId, model)` | POST | `/api/WorkerRequest/{requestId}/Apply/` | `WorkerRequestApplyModel` | `void` | Self-apply; the worker comes from the token, `email` in the body is ignored |
| `requestApplyByEmail(numberId, email)` | POST | `/api/WorkerRequest/Apply` | `WorkerRequestApplyModel` | `void` | Anonymous invitation apply (`/worker-apply?n=&e=`): resolves the email to a worker of the request's agency first, then to a candidate (city-validated) |
| `workerRequestDecline(id)` | DELETE | `/api/WorkerRequest/Decline/{id}` | — | `void` | Decline offer |

### TimeSheet
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `workerRegisterTime(requestId, lat, lon)` | POST | `/api/WorkerRequest/{requestId}/TimeSheet` | `{ latitude, longitude }` | `void` | Clock-in with GPS |
| `workerGetTimeSheet(requestId)` | GET | `/api/WorkerRequest/{requestId}/TimeSheet` | — | `WorkerTimeSheetItem[]` | |
| `getClockType(requestId, latitude, longitude, date)` | GET | `/api/WorkerRequest/{requestId}/TimeSheet/clock-type/{latitude}/{longitude}` | `date` (param) | `ClockType` (enum, `src/constants/enums`) | Can clock in/out? Coordinates resolve the time zone |

### Comments
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCommentsWorker(filter)` | GET | `/api/worker/{filter.workerId}/comment` | `WorkerCommentFilter` (params) | `WorkerCommentList` | Feedback on worker |

### Profile
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getMyProfile()` | GET | `/api/WorkerProfile/me` | — | `WorkerProfile` | |
| `registerWorker(payload)` | POST | `/api/WorkerProfile` | FormData (multipart) | `string` (profile id) | Registration |
| `uploadWorker(id, worker)` | PUT | `/api/WorkerProfile/{id}` | `WorkerProfile` | `void` | Update profile |

### Request History
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getWorkerRequestHistory(filter)` | GET | `/api/WorkerRequestHistory` | `WorkerRequestFilter` (params) | `PaginatedList<WorkerRequestListItem>` | |
| `getWorkerRequestHistoryDetail(id)` | GET | `/api/WorkerRequestHistory/{id}` | — | `WorkerRequestDetail` | |

### Job Experience
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createWorkerWorkExperience(id, model)` | POST | `/api/WorkerProfile/{id}/JobExperience` | `WorkerJobExperienceModel` | `void` | |
| `editWorkerWorkExperience(id, expId, model)` | PUT | `/api/WorkerProfile/{id}/JobExperience/{expId}` | `WorkerJobExperienceModel` | `void` | |
| `deleteWorkerWorkExperience(id, expId)` | DELETE | `/api/WorkerProfile/{id}/JobExperience/{expId}` | — | `void` | |

### SIN / Documents (all FormData multipart)
| Function | HTTP Method | Endpoint | Notes |
|----------|------------|----------|-------|
| `createWorkerSin(id, formData)` | POST | `/api/WorkerProfile/{id}/SinInformation` | Canadian SIN |
| `createWorkerDocuments(id, formData)` | POST | `/api/WorkerProfile/{id}/Documents` | |
| `createWorkerResume(id, formData)` | POST | `/api/WorkerProfile/{id}/Resume` | |
| `createWorkerLicenses(id, formData)` | POST | `/api/WorkerProfile/{id}/Licenses` | |
| `deleteWorkerLicenses(id, licenseId)` | DELETE | `/api/WorkerProfile/{id}/Licenses/{licenseId}` | |
| `createWorkerCertificates(id, formData)` | POST | `/api/WorkerProfile/{id}/Certificates` | |
| `deleteWorkerCertificates(id, certId)` | DELETE | `/api/WorkerProfile/{id}/Certificates/{certId}` | |
| `createWorkerOtherDocuments(id, formData)` | POST | `/api/WorkerProfile/{id}/OtherDocument` | |
| `deleteWorkerOtherDocuments(id, docId)` | DELETE | `/api/WorkerProfile/{id}/OtherDocument/{docId}` | |
| `createWorkerImage(id, formData)` | POST | `/api/WorkerProfile/{id}/ProfileImage` | Profile picture |

### Profile Sections
| Function | HTTP Method | Endpoint | Request Type |
|----------|------------|----------|--------------|
| `createWorkerBasicInformation(id, model)` | POST | `/api/WorkerProfile/{id}/BasicInformation` | `WorkerBasicInformationModel` |
| `createWorkerContactInformation(id, model)` | POST | `/api/WorkerProfile/{id}/ContactInformation` | `WorkerContactInformationModel` |
| `createWorkerEmergencyInformation(id, model)` | POST | `/api/WorkerProfile/{id}/EmergencyInformation` | `WorkerEmergencyInformationModel` |
| `createWorkerOther(id, model)` | POST | `/api/WorkerProfile/{id}/OtherInformation` | `WorkerOtherInformationModel` |

### Preferences & Skills
| Function | HTTP Method | Endpoint | Request Type |
|----------|------------|----------|--------------|
| `createWorkerAvailabilities(id, model)` | POST | `/api/WorkerProfile/{id}/Availabilities` | `WorkerCatalogItem[]` |
| `createWorkerAvailabilityTimes(id, model)` | POST | `/api/WorkerProfile/{id}/AvailabilityTimes` | `WorkerCatalogItem[]` |
| `createWorkerAvailabilityDays(id, model)` | POST | `/api/WorkerProfile/{id}/AvailabilityDays` | `WorkerCatalogItem[]` |
| `createWorkerLocationPreferences(id, model)` | POST | `/api/WorkerProfile/{id}/LocationPreferences` | `WorkerCatalogItem[]` |
| `createWorkerLanguages(id, model)` | POST | `/api/WorkerProfile/{id}/Languages` | `WorkerCatalogItem[]` |
| `createWorkerSkills(id, model)` | POST | `/api/WorkerProfile/{id}/Skills` | `string[]` |

### Wage & TimeSheet History
| Function | HTTP Method | Endpoint | Request Type | Response Type |
|----------|------------|----------|--------------|---------------|
| `getWorkerProfileWageHistory(filter)` | GET | `/api/WorkerProfile/{profileId}/WageHistory` | `WageHistoryFilter` (params) | `PaginatedList<WorkerWageHistoryItem>` |
| `getWorkerProfileWageHistoryAccumulated(id, rowNumber)` | GET | `/api/WorkerProfile/{id}/WageHistory/{rowNumber}` | — | `WorkerWageHistoryItem` |
| `getWorkerProfileTimeSheetHistory(filter)` | GET | `/api/WorkerProfile/{profileId}/TimeSheetHistory` | `TimeSheetHistoryFilter` (params) | `PaginatedList<WorkerTimeSheetHistoryItem>` |
| `getWorkerProfileTimeSheetHistoryAccumulated(id, rowNumber)` | GET | `/api/WorkerProfile/{id}/TimeSheetHistory/{rowNumber}` | — | `WorkerTimeSheetHistoryItem` |

**Types:** from `src/types/worker`; `ClockType` enum from `src/constants/enums`.

**Pinia:** `workerProfile` (partial) in `useWorkerStore`.

**Business Logic:** registration is multipart (file uploads); profile is split into sections for partial completion; wage/timesheet history gives earnings transparency.

---

## Summary Table: API Files by Entity

| Entity | Primary File(s) | Notable Features |
|--------|----------------|------------------|
| **Agency** | agencyApi.ts | Multi-location, personnel, agency switching, assignable roles |
| **Candidate** | agencyCandidateApi.ts | Convert to worker, skills, documents, bulk upload |
| **Company** (agency view) | agencyCompanyApi.ts | Locations, job positions, contacts, documents, settings, users |
| **Company** (self) | companyApi.ts | Requests, workers, timesheet validation, users, invoices |
| **Request** | agencyRequestApi.ts | Workers, applicants, skills, shift, sources, bulk cancel |
| **Runner** | agencyRunnerApi.ts | Recruiting pipeline per request: status, interviews |
| **WeeklyBoard** | weeklyBoardApi.ts | Recruiter day assignments + runners sent |
| **Sales** | salesApi.ts, salesDashboardApi.ts, companyApi.ts (deals/interactions) | Sales-scoped lists + Excel export; dashboard summary from static JSON; deals + interactions CRUD (owner-scoped) |
| **Worker** (agency view) | agencyWorkerApi.ts | Flags (DNU, contractor), tax, holidays, request history |
| **Worker** (self) | workerApi.ts | Profile build, applications, timesheet, wage history |
| **Invoice** | agencyInvoiceApi.ts | Preview, PDF, email, linked pay stubs |
| **PayStub** | agencyPayStubApi.ts | Generation, bulk email, subcontractor report, skip numbers |
| **TimeSheet** | agencyTimeSheetApi.ts | Per request/worker, date range, usages |
| **Note** | agencyNoteApi.ts | Worker notes read+create only; others full CRUD |
| **Location** | locationApi.ts | Country > province > city, provincial settings, location tax |
| **Catalog** | catalogApi.ts | Reference data incl. sources and tax categories |
| **Report** | agencyReportApi.ts | T4, CRA, hours worked, payments, payroll Excel |
| **Account** | accountApi.ts | Email change, deactivation |
| **Website** | websiteApi.ts | Public job search, contact form, candidate apply |
| **Notification** | notificationApi.ts, userNotificationApi.ts | Agency bell (aggregated) / user inbox |
| **Shared** | sharedApi.ts | Unsubscribe |

---

## 24. authApi.ts — IdentityServer (not Covenant.Api)

The only API file that targets `VUE_APP_SECURITY_SERVER` instead of `VUE_APP_URL_API`. Uses its own bare axios instance (no auth interceptor, no 401 retry). Consumed by `security/securityService.ts` and `pages/auth/*`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `requestPasswordToken(email, password)` | POST | `/connect/token` | form-urlencoded `grant_type=password`, `client_id`, `scope`, `username`, `password` | `TokenResponse` | No `id_token`. 400 → `TokenErrorResponse` with `error_description` ∈ `invalid_credentials`, `inactive_user`, `email_not_confirmed`, `locked_out` |
| `fetchUserInfo(tokenType, accessToken)` | GET | `/connect/userinfo` | Bearer header | `UserInfoResponse` | `role` is string or string[] |
| `requestPasswordResetCode(email)` | POST | `/Password/forgot` | `{ email }` | `void` | Always 202; 60-s resend cooldown server-side |
| `resetPasswordWithCode(payload)` | POST | `/Password/reset` | `ResetPasswordWithCodePayload` | `void` | 400 → `PasswordResetErrorResponse` with `error` ∈ `invalid_code`, `code_expired`, `too_many_attempts`, `password_policy` (+ `messages`) |
| `resendConfirmationLink(email)` | POST | `/Account/ResendConfirmationLink?userName=` | query param | `void` | Called from the `email_not_confirmed` login error |

**Types:** `TokenResponse`, `TokenErrorResponse`, `UserInfoResponse`, `ResetPasswordWithCodePayload`, `PasswordResetErrorResponse` (`src/types/security`)