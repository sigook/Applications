# Sigook.Web API Map

Maps every API module (one `api.ts` per feature under `src/modules/{portal}/{menu}/{feature}/`, plus the cross-portal ones in `src/shared/api/` and `src/app/security/authApi.ts`) to its backend endpoints. Section numbers keep the historical file names; the arrow points to where the functions live now, request/response types, and Pinia store integrations.

**Key Patterns (stated once, apply everywhere):**
- API functions are plain TypeScript functions — components import and call them directly; there are no store dispatches for HTTP.
- All HTTP calls go through the `api` wrapper object from `@/app/security/apiService` (`api.get/post/put/patch/del`), which returns `response.data` directly. The raw axios instance handles auth headers and the 401 silent-refresh retry.
- Request/response types live next to their API module (`types.ts` in the same feature folder) or in `src/shared/*/types.ts` when two portals share them.
- Pinia stores (`src/app/stores/{app,security}.ts` and one `store.ts` per portal module) hold list **filters** and auth only — never API response data.
- List endpoints return `PaginatedList<T>`; filters are passed as `params: { ...filter }` (qs-serialized).
- File uploads use `FormData` + `multipart/form-data`; PDF/Excel downloads use `responseType: 'blob'`.
- Backend is Covenant.Api (.NET 8). Agency endpoints use lowercase module bases: `/api/agency/recruiting/requests`, `/api/agency/recruiting/clients`, `/api/agency/recruiting/...`, `/api/agency/sales/...`, `/api/agency/accounting/...`.

---

## 1. accountApi.ts → `src/shared/api/accountApi.ts`
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `changeEmail(model)` | POST | `/api/account/ChangeEmail` | `ChangeEmailRequest` | `void` | |
| `getEmail()` | GET | `/api/account/GetEmail` | — | `GetEmailResponse` | Fetch current email |
| `deactivateAccount()` | PATCH | `/identity` | — | `void` | Account deactivation |

**Types:** `ChangeEmailRequest`, `GetEmailResponse` (`src/shared/types/security`)

---

## 2. agencyApi.ts → `src/modules/agency/profile/api.ts` (profile, personnel, locations, attendance) + `src/modules/agency/sales/agencies/api.ts` (agencies CRUD)
Agency profile, personnel and agency switching.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyProfile()` | GET | `/api/agency/profile` | — | `AgencyDetail` | Current logged-in agency |
| `getAgency(id)` | GET | `/api/agency/sales/agencies/{id}` | — | `AgencyDetail` | |
| `getAgenciesList(filter)` | GET | `/api/agency/sales/agencies` | `AgencyListFilter` (params) | `PaginatedList<AgencyListItem>` | |
| `createAgency(model)` | POST | `/api/agency/sales/agencies` | `CreateAgencyModel` | `{ id: string }` | |
| `updateAgency(agency)` | PUT | `/api/agency/sales/agencies` | `AgencyDetail` | `void` | |
| `getAgencyPersonnel()` | GET | `/api/agency/profile/personnel` | — | `AgencyPersonnelListItem[]` | Back-office users; `role` comes from IdentityServer and is null if that call fails |
| `createAgencyPersonnel(model)` | POST | `/api/agency/profile/personnel` | `AgencyPersonnelCreateModel` | `void` | |
| `updateAgencyPersonnel(id, model)` | PUT | `/api/agency/profile/personnel/{id}` | `AgencyPersonnelCreateModel` | `void` | Name, email and role. Policy `Admin` |
| `getAssignableRoles()` | GET | `/api/agency/profile/personnel/Roles` | — | `string[]` | Roles current user may assign |
| `deleteAgencyPersonnel(id)` | DELETE | `/api/agency/profile/personnel/{id}` | — | `void` | |
| `getAgencyLocations()` | GET | `/api/agency/profile/locations` | — | `AgencyLocationDetail[]` | |
| `createAgencyLocation(model)` | POST | `/api/agency/profile/locations` | `AgencyLocationDetail` | `{ id: string }` | |
| `updateAgencyLocation(id, model)` | PUT | `/api/agency/profile/locations/{id}` | `AgencyLocationDetail` | `void` | |
| `deleteAgencyLocation(id)` | DELETE | `/api/agency/profile/locations/{id}` | — | `void` | |
| `getPersonnelAgencies()` | GET | `/api/agency/profile/personnel/agencies` | — | `PersonnelAgencyItem[]` | Agencies user has access to |
| `switchPersonnelAgency(id)` | PUT | `/api/agency/profile/personnel/agencies/{id}` | — | `void` | Switch active agency context |
| `getAttendanceToday()` | GET | `/api/agency/profile/attendance/today` | `timeZone` (params, device IANA zone) | `UserAttendanceToday` | Current user's clock state today (sidebar button) |
| `toggleAttendance()` | POST | `/api/agency/profile/attendance` | `timeZone` (params, device IANA zone) | `UserAttendanceToday` | Clock in, or out when today is open |
| `getAttendancesTodayForUsers()` | GET | `/api/agency/profile/attendance/today/users` | `timeZone` (params, device IANA zone) | `UserAttendanceToday[]` | "Today" column of the Users grid. Policy `Admin` |
| `getAttendanceReport(filter)` | GET | `/api/agency/profile/attendance/report` | `UserAttendanceReportFilter` (params) | `UserAttendanceReport` | Per-day hours + totals. Policy `Admin` |
| `downloadAttendanceReport(filter)` | GET | `/api/agency/profile/attendance/report/file` | `UserAttendanceReportFilter` (params) | `Blob` | Excel. Policy `Admin` |
| `updateAttendance(id, model)` | PUT | `/api/agency/profile/attendance/{id}` | `UpdateUserAttendanceModel` | `void` | Admin punch correction |

**Types:** `AgencyDetail`, `AgencyListFilter`, `AgencyListItem`, `AgencyLocationDetail`, `AgencyPersonnelCreateModel`, `AgencyPersonnelListItem`, `CreateAgencyModel`, `PersonnelAgencyItem`, `AttendanceStatus`, `UserAttendance*`, `UpdateUserAttendanceModel` (the feature's `types.ts` (profile, recruiting/*, sales/*, accounting/reports, shared/notes))

**Pinia:** `useAgencyStore` holds `agency` + `agencyListFilter`; `personnelAgencies` for agency switching.

---

## 3. agencyCandidateApi.ts → `src/modules/agency/recruiting/candidates/api.ts`
Candidate pool (recruitment funnel before conversion to Worker).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCandidates(filter)` | GET | `/api/agency/recruiting/candidates` | `AgencyCandidateFilter` (params) | `PaginatedList<Candidate>` | |
| `getAgencyCandidate(id)` | GET | `/api/agency/recruiting/candidates/{id}` | — | `Candidate` | |
| `createAgencyCandidate(model, resume?)` | POST | `/api/agency/recruiting/candidates` | `CreateCandidateModel` (multipart: `data` + resume file) | `{ id: string }` | |
| `updateAgencyCandidate(id, model)` | PUT | `/api/agency/recruiting/candidates/{id}` | `CreateCandidateModel` | `void` | |
| `deleteAgencyCandidate(id)` | DELETE | `/api/agency/recruiting/candidates/{id}` | — | `void` | |
| `updateAgencyCandidateRecruiter(id)` | PUT | `/api/agency/recruiting/candidates/{id}/Recruiter` | null | `void` | Assign recruiter |
| `convertCandidateToWorker(id)` | POST | `/api/agency/recruiting/candidates/{id}/convert-to-worker` | — | `{ id: string }` | Promote to worker |
| `addCandidatePhoneNumber(id, model)` | POST | `/api/agency/recruiting/candidates/{id}/PhoneNumbers` | `CandidatePhoneNumberModel` | `{ id: string }` | |
| `deleteCandidatePhoneNumber(id, numberId)` | DELETE | `/api/agency/recruiting/candidates/{id}/PhoneNumbers/{numberId}` | — | `void` | |
| `addCandidateSkill(id, model)` | POST | `/api/agency/recruiting/candidates/{id}/Skills` | `CandidateSkillModel` | `{ id: string }` | |
| `deleteCandidateSkill(id, skillId)` | DELETE | `/api/agency/recruiting/candidates/{id}/Skills/{skillId}` | — | `void` | |
| `getCandidateDocuments(id)` | GET | `/api/agency/recruiting/candidates/{id}/Documents` | — | `PaginatedList<CandidateDocument>` | |
| `addCandidateDocument(id, model, file)` | POST | `/api/agency/recruiting/candidates/{id}/Documents` | `CreateCandidateDocumentPayload` (multipart: `data` + file) | `string` (document id) | Callers refetch the list |
| `deleteCandidateDocument(id, docId)` | DELETE | `/api/agency/recruiting/candidates/{id}/Documents/{docId}` | — | `void` | |
| `bulkAgencyCandidates(agencyId, file)` | POST | `/api/agency/recruiting/candidates/bulk/{agencyId}` | FormData (multipart) | Blob | Excel import → error report |

**Types:** `Candidate`, `CandidateDocument`, `CreateCandidateDocumentPayload`, `AgencyCandidateFilter`, `CreateCandidateModel`, `CandidatePhoneNumberModel`, `CandidateSkillModel` (`src/modules/agency/recruiting/candidates/types`)

**Pinia:** `agencyCandidateFilter` in `useAgencyStore`.

---

## 4. agencyCompanyApi.ts → `src/modules/agency/recruiting/clients/api.ts` + `src/modules/agency/sales/clients/api.ts` (deals, interactions)
**Largest API file.** Agency-side management of client companies. Bases: `companyProfilesUrl = /api/agency/recruiting/clients`, list via `/api/agency/recruiting/clients`.

### Company CRUD
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createAgencyCompany(company)` | POST | `/api/agency/recruiting/clients` | `Partial<CompanyProfileDetail>` | `{ id: string }` | |
| `getAgencyCompanies(filter)` | GET | `/api/agency/recruiting/clients` | `AgencyCompanyFilter` (params) | `PaginatedList<AgencyCompanyListItem>` | Recruiting-scoped list |
| `getAgencyCompany(id)` | GET | `/api/agency/recruiting/clients/{id}` | — | `AgencyCompanyProfileDetail` | `summary` = header counters (open/ASAP requests, workers assigned, roles, contacts, users, locations, documents, sales owner name) |
| `updateAgencyCompany(id, company)` | PUT | `/api/agency/recruiting/clients/{id}` | `Partial<CompanyProfileDetail>` | `void` | |
| `updateCompanyVaccinationRequired(id, model)` | PUT | `/api/agency/recruiting/clients/{id}/VaccinationRequired` | `VaccinationRequiredModel` | `void` | |
| `updateAgencyCompanyEmail(id, model)` | PUT | `/api/agency/recruiting/clients/{id}/Email` | `{ newEmail: string }` | `void` | |
| `updateAgencyCompanyProfileLogo(id, file)` | PUT | `/api/agency/recruiting/clients/{id}/Logo` | multipart: one part named with the generated blob name (`Logo_<guid>.ext`) | `void` | Replaces the logo and deletes the previous blob; called after create/update when a logo was picked |
| `getAgencyCompanyProfileWithRequests()` | GET | `/api/agency/recruiting/clients/company-with-requests` | — | `CompanyProfileListItem[]` | Companies + their requests |
| `getAgencyCompaniesList(searchTerm?)` | GET | `/api/agency/recruiting/clients/companies-list` | `searchTerm` (query, optional) | `CatalogItem[]` | Typeahead for the sales dashboard client pickers; every company of the agency, ordered by name, scoped to the caller's agency (**not** sales-scoped) |
| `bulkAgencyCompanies(agencyId, file)` | POST | `/api/agency/recruiting/clients/bulk/{agencyId}` | FormData (multipart) | Blob | Excel import |
| `getCompanyDeletionCheck(id)` | GET | `/api/agency/recruiting/clients/{id}/deletion-check` | — | `CompanyDeletionCheck` | **superadmin only**; lists the related records that block deletion |
| `deleteAgencyCompany(id)` | DELETE | `/api/agency/recruiting/clients/{id}` | — | `void` | **superadmin only**; 400 with the blocker detail when the company still has related records |

### Contact People
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyContactPerson(id)` | GET | `/api/agency/recruiting/clients/{id}/ContactPeople` | — | `AgencyCompanyContactPerson[]` | |
| `createAgencyCompanyContactPerson(id, model)` | POST | `/api/agency/recruiting/clients/{id}/ContactPeople` | `AgencyCompanyContactPerson` | `{ id: string }` | |
| `updateAgencyCompanyContactPerson(id, personId, model)` | PUT | `/api/agency/recruiting/clients/{id}/ContactPeople/{personId}` | `AgencyCompanyContactPerson` | `void` | |
| `deleteAgencyCompanyContactPerson(id, personId)` | DELETE | `/api/agency/recruiting/clients/{id}/ContactPeople/{personId}` | — | `void` | |

### Locations
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyLocation(id)` | GET | `/api/agency/recruiting/clients/{id}/Locations` | — | `AgencyCompanyLocationModel[]` | |
| `createAgencyCompanyLocation(id, model)` | POST | `/api/agency/recruiting/clients/{id}/Locations` | `AgencyCompanyLocationModel` | `{ id: string }` | |
| `updateAgencyCompanyLocation(id, locId, model)` | PUT | `/api/agency/recruiting/clients/{id}/Locations/{locId}` | `AgencyCompanyLocationModel` | `void` | |
| `deleteAgencyCompanyLocation(id, locId)` | DELETE | `/api/agency/recruiting/clients/{id}/Locations/{locId}` | — | `void` | |
| `updateAgencyCompanyContactInformation(id, model)` | PUT | `/api/agency/recruiting/clients/{id}/ContactInformation` | `Partial<CompanyProfileDetail>` | `void` | |

### Job Positions
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyJobPositions(id)` | GET | `/api/agency/recruiting/clients/{id}/JobPositions` | — | `AgencyCompanyJobPosition[]` | Roles + rates at company |
| `getAgencyCompanyJobPositionById(profileId, id)` | GET | `/api/agency/recruiting/clients/{profileId}/jobpositions/{id}` | — | `AgencyCompanyJobPosition` | |
| `createAgencyCompanyJobPosition(id, model)` | POST | `/api/agency/recruiting/clients/{id}/JobPositions` | `AgencyCompanyJobPosition` | `{ id: string }` | |
| `updateAgencyCompanyJobPosition(profileId, id, model)` | PUT | `/api/agency/recruiting/clients/{profileId}/jobpositions/{id}` | `AgencyCompanyJobPosition` | `void` | |
| `deleteAgencyCompanyJobPosition(profileId, id)` | DELETE | `/api/agency/recruiting/clients/{profileId}/jobpositions/{id}` | — | `void` | |
| `petitionAgencyCompanyJobPosition(id, model)` | POST | `/api/agency/recruiting/clients/{id}/JobPositions/Petition` | `PetitionJobPositionPayload` | `void` | Request new position type |

### Company Documents
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyCompanyDocument(id, pagination)` | GET | `/api/agency/recruiting/clients/{id}/Documents?PageSize={size}&PageIndex={page}` | — | `PaginatedList<CompanyProfileDocumentModel>` | |
| `createAgencyCompanyDocument(id, model, file)` | POST | `/api/agency/recruiting/clients/{id}/Documents` | `CompanyProfileDocumentModel` (multipart: `data` + file) | `string` (document id) | Callers refetch the list |
| `deleteAgencyCompanyDocument(id, docId)` | DELETE | `/api/agency/recruiting/clients/{id}/Documents/{docId}` | — | `void` | |

### Invoice Notes & Recipients
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getInvoiceNotes(id)` | GET | `/api/agency/recruiting/clients/{id}/InvoiceNotes` | — | `InvoiceNotesModel` | Invoice footer text |
| `postInvoiceNotes(id, model)` | PUT | `/api/agency/recruiting/clients/{id}/InvoiceNotes` | `InvoiceNotesModel` | `void` | |
| `getCompanyInvoiceRecipients(id)` | GET | `/api/agency/recruiting/clients/{id}/InvoiceRecipients` | — | `InvoiceRecipientModel[]` | Email CCs for invoices |
| `postCompanyInvoiceRecipient(id, model)` | POST | `/api/agency/recruiting/clients/{id}/InvoiceRecipients` | `InvoiceRecipientModel` | `{ id: string }` | |
| `deleteCompanyInvoiceRecipient(id, recipId)` | DELETE | `/api/agency/recruiting/clients/{id}/InvoiceRecipients/{recipId}` | — | `void` | |

### Company Settings
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `updatePermissionToSeeRequests(id, settings)` | PATCH | `/api/agency/recruiting/clients/{id}/RequiresPermissionToSeeRequests` | `CompanyProfileSettingsUpdate` | `void` | Restrict request visibility |
| `updatePaidHolidays(id, settings)` | PATCH | `/api/agency/recruiting/clients/{id}/PaidHolidays` | `CompanyProfileSettingsUpdate` | `void` | |
| `updateOvertime(id, settings)` | PATCH | `/api/agency/recruiting/clients/{id}/Overtime` | `CompanyProfileSettingsUpdate` | `void` | |

### Company Users (Agency-Managed)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyUsers(id)` | GET | `/api/agency/recruiting/clients/{id}/Users` | — | `CompanyUserModel[]` | |
| `createCompanyProfileUser(id, user)` | POST | `/api/agency/recruiting/clients/{id}/Users` | `CreateCompanyUserModel` | `{ id: string }` | |
| `deleteCompanyProfileUser(id, userId)` | DELETE | `/api/agency/recruiting/clients/{id}/Users/{userId}` | — | `void` | |

### Interactions & Deals (sales)
Policy `Sales` (sales, admin, superadmin). Owner-scoped: a sales user lists, updates and deletes only the rows they own, `OwnerId` is forced server-side on create; admin/superadmin are unscoped and may pass `ownerId`. The client comes from the route — never from the body or the filter — and update/delete also check that the record belongs to that client (otherwise "not found").

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyInteractions(profileId, filter)` | GET | `/api/agency/sales/clients/{profileId}/interactions` | `CompanyInteractionFilter` (params) | `PaginatedList<CompanyInteraction>` | Sort: `CreatedAt` / `Status` |
| `createCompanyInteraction(profileId, model)` | POST | `/api/agency/sales/clients/{profileId}/interactions` | `CreateCompanyInteractionModel` | `string` (id) | |
| `updateCompanyInteraction(profileId, id, model)` | PUT | `/api/agency/sales/clients/{profileId}/interactions/{id}` | `UpdateCompanyInteractionModel` | `void` | |
| `deleteCompanyInteraction(profileId, id)` | DELETE | `/api/agency/sales/clients/{profileId}/interactions/{id}` | — | `void` | |
| `getDeals(profileId, filter)` | GET | `/api/agency/sales/clients/{profileId}/deals` | `DealFilter` (params) | `PaginatedList<Deal>` | Sort: `Date` / `Value` / `Status` |
| `createDeal(profileId, model, file?)` | POST | `/api/agency/sales/clients/{profileId}/deals` | `CreateDealModel` (multipart: `data` + optional document) | `string` (id) | |
| `updateDeal(profileId, id, model, file?)` | PUT | `/api/agency/sales/clients/{profileId}/deals/{id}` | `UpdateDealModel` (multipart: `data` + optional document) | `void` | A new file replaces the current document and deletes the previous one |
| `deleteDeal(profileId, id)` | DELETE | `/api/agency/sales/clients/{profileId}/deals/{id}` | — | `void` | |

Backend: `Covenant.Api/Covenant.Api/Controllers/Sigook/Agency/CompanyProfiles/{InteractionsController,DealsController}.cs` → `SalesService`; entities in ENTITIES_RELATIONSHIPS.md ("Sales entities"). The dashboard's cross-client "latest 6" lists come from `salesDashboardApi.ts` (§18), not from here.

**UI:** the client detail's Interactions / Deals tabs (`components/agency_company/CompanyInteractions.vue` / `CompanyDeals.vue`) — rendered only when the route is the sales view **and** the user has a sales-access role (`useModuleBase().isSalesView` + `useSalesAccess()`); delete is a row action behind a confirm dialog. Create/edit go through one modal per entity — `agency_company/InteractionModal`, `agency_company/DealModal` (and `sales_dashboard/ClientModal` for new clients) — each a plain `b-modal custom-content-class="card"` wrapping its form, emitting `saved` so the host reloads:

| Form | API functions | Notes |
|------|---------------|-------|
| `InteractionForm` | `createCompanyInteraction` / `updateCompanyInteraction` | Client picker via `getAgencyCompaniesList`; read-only when editing (the id comes from the record); preselected (`initialClient`) from a client's tab or `ClientInteractionsModal` |
| `DealForm` | `createDeal` / `updateDeal` | Same client picker rules; create is `multipart/form-data` with an optional document (`utils/multipart.ts` + `utils/fileNaming.ts`) |
| `ClientForm` | `createAgencyCompany` | Create-only; catalogs via `getIndustries` / `getCompanyStatus` (catalogApi.ts); ordinary company endpoint, so sales auto-assignment applies |

### Cross-Cutting
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `updateIsAsapRequests(model)` | PUT | `/api/agency/recruiting/requests/is-asap` | `UpdateIsAsapRequestsPayload` | `void` | Mark requests as ASAP |

**Types:** `AgencyCompanyFilter`, `AgencyCompanyListItem`, `AgencyCompanyContactPerson`, `AgencyCompanyLocationModel`, `AgencyCompanyJobPosition`, `VaccinationRequiredModel`, `InvoiceNotesModel`, `InvoiceRecipientModel`, `PetitionJobPositionPayload`, `UpdateIsAsapRequestsPayload` (the feature's `types.ts` (profile, recruiting/*, sales/*, accounting/reports, shared/notes)); `CompanyProfileDetail`, `AgencyCompanyProfileDetail`, `CompanyProfileSummary`, `CompanyProfileDocumentModel`, `CompanyProfileListItem`, `CompanyProfileSettingsUpdate`, `CompanyUserModel`, `CreateCompanyUserModel` (`src/shared/company-profile/types`, `company/{profile,requests,invoices}/types`, `agency/recruiting/clients/types`, `agency/sales/clients/types`, `src/shared/punch-card/types`)

**Pinia:** `agencyCompanyProfileFilter` in `useAgencyStore`.

**Business Logic:**
- Job positions define roles + wage rates (AgencyRate vs WorkerRate markup); used when creating requests.
- Invoice recipients auto-CC company contacts when the agency emails invoices.

---

## 5. agencyInvoiceApi.ts → `src/modules/agency/accounting/invoices/api.ts`
Agency → company billing.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyInvoices(filter)` | GET | `/api/agency/accounting/invoices` | `AgencyInvoiceFilter` (params) | `AgencyInvoiceListResponse` | |
| `previewAgencyInvoice(payload)` | POST | `/api/agency/accounting/invoices/Preview` | `CreateAgencyInvoiceModel` | `InvoiceSummaryModel` | Preview before creation |
| `createAgencyInvoice(payload)` | POST | `/api/agency/accounting/invoices` | `CreateAgencyInvoiceModel` | `void` | |
| `deleteAgencyInvoice(payload)` | DELETE | `/api/agency/accounting/invoices/{invoiceId}` | `DeleteInvoicePayload` (in body via `data`) | `void` | |
| `downloadInvoicePdf(id)` | GET | `/api/agency/accounting/invoices/{id}/pdf` | — | Blob | |
| `getPayStubsByInvoice(id)` | GET | `/api/agency/accounting/invoices/{id}/paystubs` | — | `PayStubDeleteWarningItem[]` | Linked pay stubs (delete warning) |
| `sendInvoiceEmail(payload)` | POST | `/api/agency/accounting/invoices/{invoiceId}/email` | FormData (multipart) | `void` | Email invoice + attachments |
| `changeInvoiceStatus(id, payload)` | PUT | `/api/agency/accounting/invoices/{invoiceId}/status` | `ChangeInvoiceStatusModel` | `void` | Pending ↔ Paid; backend records `UpdatedAt`/`UpdatedBy` |

**Types:** `AgencyInvoiceFilter` (incl. `status`), `AgencyInvoiceListResponse`, `InvoiceStatus` enum + `INVOICE_STATUS_LABELS`/`INVOICE_STATUSES`/`invoiceStatusTagType`, `ChangeInvoiceStatusModel`, `InvoiceSummaryModel`, `CreateAgencyInvoiceModel`, `DeleteInvoicePayload`, `PayStubDeleteWarningItem`, `SendInvoiceEmailPayload` (`accounting/{invoices,paystubs}/types.ts` + `src/shared/types/invoice`)

**Pinia:** `agencyInvoiceFilter` in `useAgencyStore`.

---

## 6. agencyNoteApi.ts → `src/modules/agency/shared/notes/api.ts`
Notes attached to Workers, Candidates, Companies, Requests and request workers. Every route hangs off the `/api/agency/...` module bases.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| **Worker Notes** | | | | | Read + Create only |
| `getWorkerProfileNotes(id, pagination)` | GET | `/api/agency/recruiting/workers/{id}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createWorkerProfileNote(id, model)` | POST | `/api/agency/recruiting/workers/{id}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| **Candidate Notes** | | | | | |
| `getCandidateNotes(id, pagination)` | GET | `/api/agency/recruiting/candidates/{id}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createCandidateNote(id, model)` | POST | `/api/agency/recruiting/candidates/{id}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| `deleteCandidateNote(id, noteId)` | DELETE | `/api/agency/recruiting/candidates/{id}/Notes/{noteId}` | — | `void` | |
| **Company Notes** | | | | | Full CRUD |
| `getAgencyCompanyNotes(id, pagination)` | GET | `/api/agency/recruiting/clients/{id}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createAgencyCompanyNote(id, model)` | POST | `/api/agency/recruiting/clients/{id}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| `updateAgencyCompanyNote(id, noteId, model)` | PUT | `/api/agency/recruiting/clients/{id}/Notes/{noteId}` | `NoteModel` | `void` | |
| `deleteAgencyCompanyNote(id, noteId)` | DELETE | `/api/agency/recruiting/clients/{id}/Notes/{noteId}` | — | `void` | |
| **Request Notes** | | | | | Full CRUD |
| `getAgencyRequestNotes(id, pagination)` | GET | `/api/agency/recruiting/requests/{id}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createAgencyRequestNote(id, model)` | POST | `/api/agency/recruiting/requests/{id}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| `updateAgencyRequestNote(id, noteId, model)` | PUT | `/api/agency/recruiting/requests/{id}/Notes/{noteId}` | `NoteModel` | `void` | |
| `deleteAgencyRequestNote(id, noteId)` | DELETE | `/api/agency/recruiting/requests/{id}/Notes/{noteId}` | — | `void` | |
| **Request → Worker Notes** | | | | | Full CRUD (nested) |
| `getAgencyRequestWorkerNotes(requestId, workerId, pagination)` | GET | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/Notes?PageSize={size}&PageIndex={page}` | — | `PaginatedList<NoteItem>` | |
| `createAgencyRequestWorkerNote(requestId, workerId, model)` | POST | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/Notes` | `NoteModel` | `CreateNoteResponse` | |
| `updateAgencyRequestWorkerNote(requestId, workerId, noteId, model)` | PUT | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/Notes/{noteId}` | `NoteModel` | `void` | |
| `deleteAgencyRequestWorkerNote(requestId, workerId, noteId)` | DELETE | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/Notes/{noteId}` | — | `void` | |

**Types:** `NoteModel`, `NoteItem`, `NotePagination`, `CreateNoteResponse` (the feature's `types.ts` (profile, recruiting/*, sales/*, accounting/reports, shared/notes))

---

## 7. agencyPayStubApi.ts → `src/modules/agency/accounting/paystubs/api.ts`
Pay stub generation and payroll administration.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyPayStubs(filter)` | GET | `/api/agency/accounting/paystubs` | `AgencyPayStubFilter` (params) | `PaginatedList<AgencyPayStubListItem>` | |
| `downloadPayStubPdf(id)` | GET | `/api/agency/accounting/paystubs/{id}/pdf` | — | Blob | |
| `deleteAgencyPayStub(id)` | DELETE | `/api/agency/accounting/paystubs/{id}` | — | `void` | |
| `sendPayStubEmail(id)` | POST | `/api/agency/accounting/paystubs/{id}/email` | — | `void` | Email to worker |
| `sendPayStubEmailBulk(payStubIds)` | POST | `/api/agency/accounting/paystubs/email/bulk` | `{ payStubIds: string[] }` | `void` | Bulk email |
| `createAgencyPayStub(payload)` | POST | `/api/agency/accounting/paystubs` | `CreatePayStubPayload` | `void` | Manual creation |
| `getWorkersReadyForPayStub()` | GET | `/api/agency/accounting/paystubs/WorkersReadyForPayStub` | — | `WorkerReadyForPayStubModel[]` | Workers with approved timesheets |
| `generatePayStubs(workerIds)` | POST | `/api/agency/accounting/paystubs/generate` | `string[]` | `void` | Batch generate from timesheets |
| `getPayrollSubcontractors(filter)` | GET | `/api/agency/accounting/reports/subcontractors` | `SubcontractorPayrollFilter` (params) | `PaginatedList<PayrollSubContractorListItem>` | |
| `downloadSubcontractorReport(weekEnding)` | GET | `/api/agency/accounting/reports/subcontractors/file` | `weekEnding` (param) | Blob | Excel |
| `deleteSubcontractorReport(weekEnding)` | DELETE | `/api/agency/accounting/reports/subcontractors` | `weekEnding` (param) | `void` | Deletes the whole week; releases its timesheets |
| `getSkipPayrollNumbers(filter)` | GET | `/api/agency/accounting/paystubs/skip-payroll-number` | `{ searchTerm? }` (params) | `SkipPayrollNumberItem[]` | |
| `addSkipPayrollNumber(payload)` | POST | `/api/agency/accounting/paystubs/skip-payroll-number` | `CreateSkipPayrollNumberPayload` | `void` | |

**Types:** `AgencyPayStubFilter`, `AgencyPayStubListItem`, `CreatePayStubPayload`, `CreateSkipPayrollNumberPayload`, `PayrollSubContractorListItem`, `SkipPayrollNumberItem`, `SubcontractorPayrollFilter`, `WorkerReadyForPayStubModel` (`accounting/{invoices,paystubs}/types.ts` + `src/shared/types/invoice`)

**Pinia:** `agencyPayStubFilter` in `useAgencyStore`.

---

## 8. agencyReportApi.ts → `src/shared/api/reportApi.ts` (`downloadAgencyReport` only) + `src/modules/agency/accounting/reports/api.ts` + `src/modules/agency/recruiting/requests/api.ts` (`getWorkersReportDocument`)
Report generation and blob downloads.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `downloadAgencyReport(url, filter)` | GET | (dynamic url) | `ReportQueryParams` (params) | Blob | Generic blob downloader |
| `getWorkersReportDocument(requestId)` | GET | `/api/agency/recruiting/requests/{requestId}/WorkersReport` | — | Blob | Excel export of the request's workers |
| `getJobPositionsHoursWorked(filter)` | GET | `/api/agency/accounting/reports/{companyProfileId}/job-positions` | `AgencyReportFilter & { companyProfileId }` (params) | `AgencyCompanyJobPosition[]` | Hours per position |
| `getHoursWorkedReport(filter)` | GET | `/api/agency/accounting/reports/hours-worked` | `AgencyReportFilter` (params) | `HoursWorkedResume` | |
| `getTimesheetsReport(filter)` | GET | `/api/agency/accounting/reports/timesheets/file` | `AgencyReportFilter` (params) | Blob | USA agencies |
| `getT4Report(filter)` | GET | `/api/agency/accounting/reports/t4` | `AgencyReportFilter` (params) | Blob | Tax form export |
| `getCraPayrollReport(filter)` | GET | `/api/agency/accounting/reports/cra-payroll` | `AgencyReportFilter` (params) | Blob | CRA export |
| `getPaymentReport(filter)` | GET | `/api/agency/accounting/reports/payments` | `AgencyReportFilter` (params) | `PaginatedList<WeeklyPayrollItem>` | Weekly payroll summary |
| `downloadWeeklyPayrollReport(weekEnding)` | GET | `/api/agency/accounting/reports/payments/file` | `weekEnding` (param) | Blob | Excel |

**Types:** `ReportQueryParams`, `AgencyReportFilter`, `AgencyCompanyJobPosition`, `HoursWorkedResume`, `WeeklyPayrollItem` (the feature's `types.ts` (profile, recruiting/*, sales/*, accounting/reports, shared/notes), `accounting/{invoices,paystubs}/types.ts` + `src/shared/types/invoice`)

> Backend-only: `/api/agency/accounting/reports/hours-worked/file` exists in the backend but has no frontend wrapper.

---

## 9. agencyRequestApi.ts → `src/modules/agency/recruiting/requests/api.ts`
Core job request lifecycle. Bases: `requestsUrl = /api/agency/recruiting/requests`, lists via `recruitingRequestsUrl = /api/agency/recruiting/requests`.

### Request CRUD
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `postAgencyRequest(model)` | POST | `/api/agency/recruiting/requests` | `CreateAgencyRequestModel` | `AgencyRequestDetail` | |
| `getAgencyRequestLookup(companyProfileId, requestId?)` | GET | `/api/agency/recruiting/requests/lookup` | `companyProfileId`, `requestId?` (params) | `AgencyRequestLookup` | Everything the request form needs in one call: job positions, locations, personnel, company users and — when `requestId` is sent — the request detail. Feeds create, edit and duplicate. With `requestId` the company profile is taken from the request itself |
| `duplicateAgencyRequest(sourceRequestId, model)` | POST | `/api/agency/recruiting/requests/{sourceRequestId}/Duplicate` | `CreateAgencyRequestModel` | `AgencyRequestDetail` | Creates a new request from the edited form; the backend copies the shift (unless the payload carries one), skills, requested-by / report-to contacts and job boards from the source request |
| `getAgencyRequests(filter)` | GET | `/api/agency/recruiting/requests` | `AgencyRequestFilter` (params) | `AgencyRequestsPagedResponse` | Recruiting-scoped list |
| `getAllAgencyRequests(filter)` | GET | `/api/agency/recruiting/requests/all` | `AgencyRequestFilter` (params) | `AgencyRequestListItem[]` | Unpaged |
| `getAgencyRequest(id)` | GET | `/api/agency/recruiting/requests/{id}` | — | `AgencyRequestDetail` | Carries `applicantsCount`, `runnersCount` and `workersCount` (all `WorkerRequests`, booked and rejected) for the tab counters, plus `skills`, `requestedBy` and `reportTo` so the detail page loads them in the same call |
| `updateAgencyRequest(id, model)` | PUT | `/api/agency/recruiting/requests/{id}` | `CreateAgencyRequestModel` | `AgencyRequestDetail` | |
| `cancelAgencyRequest(id, payload)` | PUT | `/api/agency/recruiting/requests/{id}/Cancel` | `CancelRequestPayload` | `void` | Cancel + reason |
| `bulkCancelRequests(payload)` | PUT | `/api/agency/recruiting/requests/bulk-cancel` | `BulkCancelRequestsPayload` | `BulkCancelRequestsResult` | Cancel many at once |
| `bulkUpdateRecruiters(payload)` | PUT | `/api/agency/recruiting/requests/bulk-recruiters` | `BulkUpdateRecruitersPayload` | `void` | Replace recruiters on many at once; empty list unassigns. Admin/SuperAdmin only |
| `agencyRequestOpen(id)` | PUT | `/api/agency/recruiting/requests/{id}/Open` | id (in body) | `void` | Reopen |
| `agencyRequestSendInvitation(id)` | POST | `/api/agency/recruiting/requests/{id}/SendInvitation` | — | `void` | 120s timeout |
| `updateAgencyRequestIsAsap(id)` | PUT | `/api/agency/recruiting/requests/{id}/IsAsap` | — | `void` | Toggle ASAP |
| `updateAgencyPunchCardVisibilityStatusInApp(id)` | PUT | `/api/agency/recruiting/requests/{id}/PunchCardVisibilityStatusInApp` | — | `void` | |
| `getAgencyRequestShift(id)` | GET | `/api/agency/recruiting/requests/{id}/Shift` | — | `RequestShiftModel` | |
| `updateAgencyRequestShift(id, model)` | PUT | `/api/agency/recruiting/requests/{id}/Shift` | `RequestShiftModel` | `{ id; displayShift? }` | |
| `increaseWorkersQuantityByOne(id)` | PUT | `/api/agency/recruiting/requests/{id}/IncreaseWorkersQuantityByOne` | — | `void` | |
| `reduceWorkersQuantityByOne(id)` | PUT | `/api/agency/recruiting/requests/{id}/ReduceWorkersQuantityByOne` | — | `void` | |

### Request → Workers
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRequestsWorkers(filter)` | GET | `/api/agency/recruiting/requests/{filter.requestId}/Workers` | `AgencyRequestWorkerFilter` (params) | `PaginatedList<AgencyRequestWorker>` | |
| `bookAgencyRequestWorker(requestId, workerId, model)` | POST | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/Book` | `BookWorkerModel` | `{ id: string }` | Assign worker |
| `updateAgencyRequestWorkerStartDate(requestId, id, model)` | PUT | `/api/agency/recruiting/requests/{requestId}/workers/{id}` | `BookWorkerModel` | `void` | |
| `rejectAgencyRequestWorker(requestId, workerId, model)` | PUT | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/Reject` | `RejectWorkerModel` | `void` | |

### Applicants
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `searchAgencyRequestApplicants(id, searchTerm)` | GET | `/api/agency/recruiting/requests/{id}/Applicants/Search` | `searchTerm` (param) | `ApplicantSearchResult[]` | |
| `getAgencyRequestApplicant(filter)` | GET | `/api/agency/recruiting/requests/{filter.requestId}/Applicants` | `AgencyRequestApplicantFilter` (params) | `PaginatedList<AgencyRequestApplicant>` | Every row carries its compliance counters (`complianceTotal`, `complianceCompleted`, `mandatoryPending`) |
| `getAgencyApplicants(filter)` | GET | `/api/agency/recruiting/applicants` | `AgencyApplicantsFilter` (params) | `AgencyApplicantsPagedResponse` | Applicants of every request of the agency. The page is a page of REQUESTS (`pageSize` counts requests) and each one carries all its matching applicants, so a request is never split; `totalApplicants` counts the whole filter |
| `postAgencyRequestApplicant(id, model)` | POST | `/api/agency/recruiting/requests/{id}/Applicants` | `CreateRequestApplicantModel` | `AgencyRequestApplicant` | |
| `deleteAgencyRequestApplicant(id, applicantId)` | DELETE | `/api/agency/recruiting/requests/{id}/Applicants/{applicantId}` | — | `void` | |
| `updateAgencyRequestApplicant(id, applicantId, model)` | PUT | `/api/agency/recruiting/requests/{id}/Applicants/{applicantId}` | `UpdateApplicantCommentsPayload` | `void` | |
| `changeApplicantStatus(id, applicantId, model)` | PUT | `/api/agency/recruiting/requests/{id}/Applicants/{applicantId}/Status` | `ChangeRequestApplicantStatusModel` | `void` | Fails when the transition is not valid for that applicant |
| `changeAgencyApplicantsStatus(model)` | PUT | `/api/agency/recruiting/applicants/Status` | `ChangeApplicantsStatusModel` | `ChangeApplicantsStatusResult` | Bulk change across requests: applies what it can and returns `skipped[]` with the reason for each one it left untouched |

### Request Contact People (RequestedBy / ReportTo)
The lists come inside `getAgencyRequest`; these endpoints only add and remove.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `postAgencyRequestRequestedBy(id, personId)` | POST | `/api/agency/recruiting/requests/{id}/RequestedBy/{personId}` | — | `void` | Who requested (company side) |
| `deleteAgencyRequestRequestedBy(id, personId)` | DELETE | `/api/agency/recruiting/requests/{id}/RequestedBy/{personId}` | — | `void` | |
| `postAgencyRequestReportTo(id, personId)` | POST | `/api/agency/recruiting/requests/{id}/ReportTo/{personId}` | — | `void` | Worker's supervisor |
| `deleteAgencyRequestReportTo(id, personId)` | DELETE | `/api/agency/recruiting/requests/{id}/ReportTo/{personId}` | — | `void` | |

> Recruiter assignment lives in the **Recruiting → Weekly Board** feature (per work day). See §20.

### Skills
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `postAgencyRequestSkill(id, model)` | POST | `/api/agency/recruiting/requests/{id}/Skills` | `AgencyRequestSkillModel` | `{ id: string }` | |
| `deleteAgencyRequestSkill(id, skillId)` | DELETE | `/api/agency/recruiting/requests/{id}/Skills/{skillId}` | — | `void` | |

### Sources (Job Boards)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `setAgencyRequestSources(id, items)` | PUT | `/api/agency/recruiting/requests/{id}/sources` | `SetRequestJobBoardItem[]` | `void` | |

**Types:** `AgencyRequestFilter`, `AgencyRequestsPagedResponse`, `AgencyRequestListItem`, `AgencyRequestDetail`, `CreateAgencyRequestModel`, `RequestShiftModel`, `CancelRequestPayload`, `BulkCancelRequestsPayload`, `BulkCancelRequestsResult`, `AgencyRequestWorkerFilter`, `AgencyRequestWorker`, `BookWorkerModel`, `RejectWorkerModel`, `AgencyRequestApplicantFilter`, `AgencyRequestApplicant`, `AgencyApplicantsFilter`, `AgencyApplicant`, `AgencyRequestApplicants`, `AgencyApplicantsPagedResponse`, `ChangeApplicantsStatusModel`, `SkippedApplicant`, `ChangeApplicantsStatusResult`, `ApplicantSearchResult`, `CreateRequestApplicantModel`, `UpdateApplicantCommentsPayload`, `AgencyRequestSkillModel`, `AgencyRequestPersonItem`, `RequestJobBoard`, `SetRequestJobBoardItem` (the feature's `types.ts` (profile, recruiting/*, sales/*, accounting/reports, shared/notes))

**Pinia:** `agencyRequestFilter` in `useAgencyStore`.

---

## 10. agencyRunnerApi.ts → `src/modules/agency/recruiting/requests/runners/api.ts`
Runners — recruiting pipeline of prospects per request (list, status transitions, interviews). Base: `/api/agency/recruiting/requests/{requestId}/runners`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyRunners(requestId, filter)` | GET | `/api/agency/recruiting/requests/{requestId}/runners` | `AgencyRunnerFilter` (params) | `PaginatedList<RunnerListItem>` | |
| `searchAgencyRunnerProspects(requestId, searchTerm)` | GET | `/api/agency/recruiting/requests/{requestId}/runners/Search` | `searchTerm` (param) | `ApplicantSearchResult[]` | Search workers to add (workers only) |
| `getAgencyRunner(requestId, id)` | GET | `/api/agency/recruiting/requests/{requestId}/runners/{id}` | — | `RunnerDetail` | |
| `createAgencyRunner(requestId, model)` | POST | `/api/agency/recruiting/requests/{requestId}/runners` | `CreateRunnerModel` | `string` (id) | |
| `deleteAgencyRunner(requestId, id)` | DELETE | `/api/agency/recruiting/requests/{requestId}/runners/{id}` | — | `void` | Deletes the runner with its history and interviews; used by the Runners tab **and** the weekly board |
| `changeRunnerStatus(requestId, id, model)` | PUT | `/api/agency/recruiting/requests/{requestId}/runners/{id}/Status` | `ChangeRunnerStatusModel` | `void` | Pipeline transition |
| `createRunnerInterview(requestId, id, model)` | POST | `/api/agency/recruiting/requests/{requestId}/runners/{id}/Interview` | `CreateRunnerInterviewModel` | `string` (id) | Schedule interview |
| `rescheduleRunnerInterview(requestId, id, interviewId, model)` | PUT | `/api/agency/recruiting/requests/{requestId}/runners/{id}/Interview/{interviewId}/Reschedule` | `RescheduleRunnerInterviewModel` | `void` | |

**Types:** `AgencyRunnerFilter`, `RunnerListItem`, `RunnerDetail`, `CreateRunnerModel`, `ChangeRunnerStatusModel`, `CreateRunnerInterviewModel`, `RescheduleRunnerInterviewModel` (`src/modules/agency/recruiting/requests/runners/types`); `ApplicantSearchResult` (the feature's `types.ts` (profile, recruiting/*, sales/*, accounting/reports, shared/notes))

**UI:** `components/agency_request/Runners.vue` + `components/runner/*` (CreateRunner, RunnerStatusModal, RunnerInterviewModal, RunnerHistoryModal). See WORKFLOWS.md → Runner Pipeline Flow.

---

## 11. agencyTimeSheetApi.ts → `src/modules/agency/recruiting/requests/timeSheetApi.ts`
Timesheets per request/worker. Base: `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyWorkerTimeSheetByDate(requestId, workerId, date)` | GET | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets` | `{ startDate, endDate }` (params) | `TimeSheetListItem[]` | Date range |
| `postAgencyWorkerTimeSheet(requestId, workerId, model)` | POST | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets` | `TimeSheetModel` | `{ id: string }` | |
| `updateAgencyWorkerTimeSheet(requestId, workerId, id, model)` | PUT | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets/{id}` | `TimeSheetModel` | `void` | |
| `deleteAgencyWorkerTimeSheet(requestId, workerId, id)` | DELETE | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets/{id}` | — | `void` | |
| `getAgencyTimeSheetUsages(requestId, workerId, id)` | GET | `/api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets/{id}/Usages` | — | `TimeSheetUsagesModel` | Which invoices/pay stubs use it |

**Types:** `TimeSheetListItem`, `TimeSheetModel`, `TimeSheetUsagesModel` (`src/shared/company-profile/types`, `company/{profile,requests,invoices}/types`, `agency/recruiting/clients/types`, `agency/sales/clients/types`, `src/shared/punch-card/types`)

---

## 12. agencyWorkerApi.ts → `src/modules/agency/recruiting/workers/api.ts`
Worker profile management from the agency's perspective.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getAgencyWorkers(filter)` | GET | `/api/agency/recruiting/workers` | `AgencyWorkerFilter` (params) | `PaginatedList<AgencyWorkerListItem>` | |
| `getAgencyWorkersDropdown(filter)` | GET | `/api/agency/recruiting/workers/Dropdown` | `{ searchTerm }` (params) | `AgencyWorkerDropdownItem[]` | Autocomplete¹ |
| `getAgencyWorker(id)` | GET | `/api/agency/recruiting/workers/{id}` | — | `WorkerProfile` | |
| `updateApprovedToWork(id)` | PUT | `/api/agency/recruiting/workers/{id}/ApprovedToWork` | — | `void` | Toggle |
| `updateAgencyWorkerProfileDNU(id)` | PUT | `/api/agency/recruiting/workers/{id}/Dnu` | — | `void` | Toggle Do-Not-Use |
| `updateAgencyWorkerContractor(id)` | PUT | `/api/agency/recruiting/workers/{id}/IsContractor` | — | `void` | Toggle |
| `updateAgencyWorkerSubContractor(id)` | PUT | `/api/agency/recruiting/workers/{id}/IsSubcontractor` | — | `void` | Toggle |
| `updateWorkerProfileTaxCategory(payload)` | PUT | `/api/agency/recruiting/workers/{id}/tax-category` | `UpdateWorkerProfileFieldsPayload` | `void` | |
| `updateWorkerProfileTaxRate(payload)` | PUT | `/api/agency/recruiting/workers/{id}/tax-rate` | `UpdateWorkerProfileFieldsPayload` | `void` | |
| `updateWorkerProfileExternalId(payload)` | PUT | `/api/agency/recruiting/workers/{id}/ExternalId` | `UpdateWorkerProfileFieldsPayload` | `void` | |
| `updateWorkerProfileWcCode(payload)` | PUT | `/api/agency/recruiting/workers/{id}/WcCode` | `UpdateWorkerProfileFieldsPayload` | `void` | |
| `updateAgencyWorkerEmail(id, model)` | PUT | `/api/agency/recruiting/workers/{id}/Email` | `UpdateWorkerEmailModel` | `void` | |
| `agencyCommentWorker(id, comment)` | POST | `/api/agency/recruiting/workers/{id}/Comments` | `AgencyWorkerCommentModel` | `void` | |
| `getAgencyWorkerProfileRequestHistory(id, pagination)` | GET | `/api/agency/recruiting/workers/{id}/RequestHistory?PageSize={size}&PageIndex={page}` | — | `PaginatedList<AgencyWorkerRequestHistoryItem>` | Past assignments |
| `getAgencyWorkerProfileHolidays(id)` | GET | `/api/agency/recruiting/workers/{id}/Holidays` | — | `AgencyWorkerHoliday[]` | |
| `addUpdateAgencyWorkerProfileHolidays(id, data)` | POST | `/api/agency/recruiting/workers/{id}/Holidays` | `AgencyWorkerHoliday` | `void` | |
| `addNewHoliday(id, payload)` | POST | `/api/agency/recruiting/workers/{id}/Holidays/new-holiday` | `AddNewHolidayPayload` | `void` | Bulk holiday add |

**Types:** `AgencyWorkerFilter`, `AgencyWorkerListItem`, `AgencyWorkerDropdownItem`, `AgencyWorkerCommentModel`, `UpdateWorkerEmailModel`, `UpdateWorkerProfileFieldsPayload`, `AgencyWorkerHoliday`, `AddNewHolidayPayload`, `AgencyWorkerRequestHistoryItem` (the feature's `types.ts` (profile, recruiting/*, sales/*, accounting/reports, shared/notes)); `WorkerProfile` (`src/shared/worker-profile/types`, `worker/requests/types`, `agency/recruiting/workers/types`)

**Pinia:** `agencyWorkerProfileFilter` in `useAgencyStore`.

**Business Logic:** contractor/subcontractor flags affect payroll treatment; tax category/rate drive withholdings; holidays feed holiday pay.

¹ The URL string at `agencyWorkerApi.ts:23` is missing its leading slash (`'api/agency/workers/Dropdown'`) — it only works because axios concatenates it onto the baseURL.

---

## 13. catalogApi.ts → `src/shared/api/catalogApi.ts`
Reference data (lookup tables).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getGenders()` | GET | `/api/catalog/gender` | — | `Gender[]` | |
| `getIdentificationTypes()` | GET | `/api/catalog/identificationType` | — | `IdentificationType[]` | |
| `getAvailability()` | GET | `/api/catalog/availability` | — | `Availability[]` | Full-time, Part-time, ... |
| `getAvailabilityTimes()` | GET | `/api/catalog/availabilityTime` | — | `AvailabilityTime[]` | Morning, Evening, ... |
| `getDays()` | GET | `/api/catalog/day` | — | `Day[]` | |
| `fetchLifts()` | GET | `/api/catalog/lift` | — | `Lift[]` | |
| `fetchLanguages()` | GET | `/api/catalog/language` | — | `Language[]` | |
| `getWsibGroups()` | GET | `/api/catalog/wsibgroup` | — | `WsibGroup[]` | WSIB classifications (Canada) |
| `getSkills()` | GET | `/api/catalog/skills` | — | `Skill[]` | |
| `getIndustries()` | GET | `/api/catalog/industry` | — | `Industry[]` | |
| `getReasonCancellationRequest()` | GET | `/api/catalog/reasonCancellationRequest` | — | `CancellationReason[]` | |
| `getCompanyStatus()` | GET | `/api/catalog/companyStatus` | — | `CatalogItem<number>[]` | |
| `getSources()` | GET | `/api/catalog/source` | — | `Source[]` | Candidate/worker sources |
| `getSourcesForRequests()` | GET | `/api/catalog/source/requests` | — | `Source[]` | Job boards for requests |
| `getTaxCategories()` | GET | `/api/catalog/tax-categories` | — | `TaxCategory[]` | |
| `addIndustry(industry)` | POST | `/api/catalog/industry` | `{ id?, value }` | `Industry` | Add custom industry |

**Types:** from `src/shared/types/common`.

---

## 14. companyApi.ts → `src/modules/company/profile/api.ts` + `src/modules/company/requests/api.ts` + `src/modules/company/invoices/api.ts`
Company portal (client) view of their profile, requests and workers.

### Profile & Locations
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyProfile()` | GET | `/api/company/profile` | — | `CompanyProfileDetail` | Current company |
| `updateProfile(id, company)` | PUT | `/api/company/profile/{id}` | `CompanyProfileDetail` | `void` | |
| `getProfileLocations()` | GET | `/api/company/profile/locations` | — | `CompanyProfileLocationDetail[]` | |
| `createProfileLocation(model)` | POST | `/api/company/profile/locations` | `CompanyProfileLocationDetail` | `void` | |
| `updateProfileLocation(id, model)` | PUT | `/api/company/profile/locations/{id}` | `CompanyProfileLocationDetail` | `void` | |
| `deleteProfileLocation(id)` | DELETE | `/api/company/profile/locations/{id}` | — | `void` | |

### Job Positions
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyJobPositions()` | GET | `/api/company/profile/jobpositions` | — | `CompanyProfileJobPositionRate[]` | Roles + rates |

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
| `rejectCompanyRequestWorker(requestId, workerId, model)` | PUT | `/api/company/requests/{requestId}/workers/{workerId}/Reject` | `CommentsModel` | `void` | |
| `requestAnotherWorker(requestId, comment)` | POST | `/api/company/requests/{requestId}/workers/RequestNewWorker` | `CommentsModel` | `void` | Ask for replacement |

### TimeSheet
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyWorkerTimeSheetByDate(requestId, workerId, date)` | GET | `/api/company/requests/{requestId}/workers/{workerId}/TimeSheets` | `{ startDate, endDate }` (params) | `TimeSheetListItem[]` | |
| `postCompanyWorkerTimeSheet(requestId, workerId, model)` | POST | `/api/company/requests/{requestId}/workers/{workerId}/TimeSheets` | `TimeSheetModel` | `{ id: string }` | |
| `validateHoursTimeSheet(requestId, workerId, id, model)` | PUT | `/api/company/requests/{requestId}/workers/{workerId}/TimeSheets/{id}` | `TimeSheetModel` | `void` | |
| `updateCompanyRequestWorkerTimeSheet(requestId, workerId, id, model)` | PUT | `/api/company/requests/{requestId}/workers/{workerId}/TimeSheets/{id}` | `TimeSheetModel` | `void` | |
| `deleteCompanyWorkerTimeSheet(requestId, workerId, id)` | DELETE | `/api/company/requests/{requestId}/workers/{workerId}/TimeSheets/{id}` | — | `void` | |
| `companyTimeSheetClockIn(requestId, workerId, model)` | POST | `/api/company/requests/{requestId}/workers/{workerId}/TimeSheets/ClockIn` | `ClockInModel` | `ClockInResult` | GPS clock-in |

### Comments / Users / Contacts / Invoices
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCompanyUser()` | GET | `/api/company/profile/users` | — | `CompanyUserModel[]` | |
| `getCompanyUserDetail()` | GET | `/api/company/profile/users/detail` | — | `CompanyUserModel` | Current user |
| `createCompanyUser(model)` | POST | `/api/company/profile/users` | `CreateCompanyUserModel` | `void` | |
| `updateCompanyUser(id, user)` | PUT | `/api/company/profile/users/{id}` | `CompanyUserModel` | `void` | |
| `deleteCompanyUser(id)` | DELETE | `/api/company/profile/users/{id}` | — | `void` | |
| `getContactPeople()` | GET | `/api/company/profile/contactpeople` | — | `CompanyContactPersonModel[]` | |
| `saveContactPerson(model)` | POST | `/api/company/profile/contactpeople` | `CompanyContactPersonModel` | `void` | Create/update |
| `deleteContactPerson(id)` | DELETE | `/api/company/profile/contactpeople/{id}` | — | `void` | |
| `getCompanyInvoice(filter)` | GET | `/api/company/invoices` | `CompanyInvoiceFilter` (params) | `PaginatedList<CompanyInvoiceListItem>` | |
| `getCompanyRequestTimeSheetFile(requestId)` | GET | `/api/company/requests/{requestId}/TimeSheets/File` | — | Blob | Excel punch-card export, ownership-checked server-side |

**Types:** from `src/shared/company-profile/types`, `company/{profile,requests,invoices}/types`, `agency/recruiting/clients/types`, `agency/sales/clients/types`, `src/shared/punch-card/types` (+ `InvoiceSummaryModel` from `accounting/{invoices,paystubs}/types.ts` + `src/shared/types/invoice`). The sales deals/interactions enums + models also live in `src/types/company.ts`, but their API calls are in `agencyCompanyApi.ts` (§4).

> **Enum mirror gotcha.** The API serializes enums as **ints** (System.Text.Json, no `JsonStringEnumConverter`), so `DealType`, `DealStatus`, `InteractionType`, `InteractionPurpose` and `InteractionStatus` in `src/types/company.ts` must match `Covenant.Common/Enums/` **numerically** — adding or reordering a member on one side without the other silently mislabels records. Label/color/icon maps (`DEAL_TYPE_LABELS`, `INTERACTION_TYPE_ICONS`, …) live next to the enums; `InteractionType.Mail` is labelled "Email" in the UI. Meaning of each value: `.docs/business/SALES_MODULE.md`.

**Pinia:** `companyRequestFilter` in `useCompanyStore`.

**Business Logic:** timesheet validation by the company feeds invoicing; clock-in captures GPS + time. Deals/interactions are owner-scoped end-to-end for sales users (admin/superadmin unscoped) — stricter than the list-only scoping of orders/clients (ROLES_PERMISSIONS.md). Concepts, catalogs and deal lifecycle: `.docs/business/SALES_MODULE.md`.

---

## 15. locationApi.ts → `src/shared/api/locationApi.ts`
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getCountries()` | GET | `/api/location/country` | — | `Country[]` | |
| `getProvinces(countryId)` | GET | `/api/location/province/{countryId}` | — | `Province[]` | |
| `getCities(provinceId)` | GET | `/api/location/city/{provinceId}` | — | `City[]` | |
| `createCity(city)` | POST | `/api/location/city` | `{ value, code?, province: { id } }` | `City` | Add custom city |
| `updateProvinceSettings(provinceId, settings)` | PUT | `/api/location/province/{provinceId}/settings` | `ProvinceSettings` | `void` | Global provincial rules; Policy `Admin` |
| `getLocationTax(locationId)` | GET | `/api/location/{locationId}/tax` | — | `LocationTax \| null` | Tax % per location (admin) |
| `upsertLocationTax(locationId, model)` | PUT | `/api/location/{locationId}/tax` | `LocationTax` | `void` | |

**Types:** `Country`, `Province`, `City`, `LocationTax` (`src/shared/types/common`)

Province settings (`ProvinceSettingsModal.vue`) persist through `updateProvinceSettings` directly — the modal calls the API on save and then emits to `Address.vue`, which only refreshes the local display model. They are **global per province** (admin-only), not part of the company-location save.

---

## 16. notificationApi.ts → `src/modules/agency/shared/notifications/api.ts`
In-app notification bell (agency roles). A single aggregated call returns every notification kind in one payload.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getNotifications()` | GET | `/api/agency/notifications` | — | `NotificationsResponse` | Grouped by type; no notification kinds are defined, so the payload is empty |

**Types:** `NotificationsResponse`, `AppNotification`, `NotificationGroup`, `NotificationType` (`src/modules/agency/shared/notifications/types`)

**Composable:** `useNotifications` loads once, maps each typed list to generic `AppNotification[]` grouped by `NotificationType`.

**UI:** `SidebarLogged.vue` owns the load; the user avatar at the sidebar footer shows a red dot when there are notifications and the user menu opens with a "Notifications" section (per-type count, or "Nothing to review").

**Extensibility:** a new kind = new list on backend `NotificationsModel` + new `NotificationType`/label/route + a mapper in `useNotifications`.

---

## 17. salesApi.ts → `src/modules/agency/sales/api.ts`
Sales-role-scoped lists (parallel to the recruiting-scoped lists in agencyRequestApi/agencyCompanyApi). Bases: `/api/agency/sales/requests`, `/api/agency/sales/companyprofiles`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getSalesRequests(filter)` | GET | `/api/agency/sales/requests` | `AgencyRequestFilter` (params) | `AgencyRequestsPagedResponse` | |
| `getSalesCompanies(filter)` | GET | `/api/agency/sales/companyprofiles` | `AgencyCompanyFilter` (params) | `PaginatedList<AgencyCompanyListItem>` | |

**Types:** `AgencyRequestFilter`, `AgencyRequestsPagedResponse`, `AgencyCompanyFilter`, `AgencyCompanyListItem` (the feature's `types.ts` (profile, recruiting/*, sales/*, accounting/reports, shared/notes))

**Usage:** `/sales/requests` and `/sales/companies` pages; shared detail pages resolve their base path via `useModuleBase`. The sales dashboard's interaction/deal client pickers use `getAgencyCompaniesList` (agencyCompanyApi.ts) instead — note that endpoint is **not** sales-scoped, so a sales user picks from every company of the agency.

---

## 18. salesDashboardApi.ts → `src/modules/agency/sales/dashboard/api.ts`
Live sales dashboard aggregates. Base: `/api/agency/sales/dashboard` (backend `DashboardController` in
`Covenant.Api/Controllers/Sigook/Agency/Sales/`). Owner-scoped exactly like deals and interactions: a
sales user always sees only their own rows, admin/superadmin see the whole agency and may pass `ownerId`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getDealsByStatus(filter)` | GET | `/api/agency/sales/dashboard/deals-by-status` | `DealsByStatusFilter` | `DealsByStatusModel` | Feeds the "Deals by status" column chart |
| `getSalesDashboardSummary()` | GET | `/api/agency/sales/dashboard/summary` | — | `SalesDashboardSummary` | Feeds the "This quarter" meters and the header period label |
| `getRecentClients()` | GET | `/api/agency/sales/dashboard/recent-clients` | — | `SalesRecentClient[]` | Feeds the "Clients" card: the 10 clients with the most recent interaction, newest first; no `ownerId` param |
| `getRecentInteractions()` | GET | `/api/agency/sales/dashboard/recent-interactions` | — | `CompanyInteraction[]` | Feeds the "Log Interactions" card: the 6 newest interactions across all clients; no `ownerId` param |
| `getRecentDeals()` | GET | `/api/agency/sales/dashboard/recent-deals` | — | `Deal[]` | Feeds the "Deals" card: the 6 deals with the latest `Date` across all clients; no `ownerId` param |

**Query params (`DealsByStatusFilter`):** `period` (`SalesPeriod` 0 Day / 1 Week / 2 Month / 3 Quarter,
default Week), `statuses` (optional `DealStatus[]`, serialized `statuses[0]=0&statuses[1]=3` by the qs
`indices` format), `ownerId` (honored only for admin/superadmin).

**Types** (`src/modules/agency/sales/dashboard/types.ts`): `SalesPeriod`, `SalesPeriodRange`, `DealStatusSummary`,
`InteractionTypeSummary`, `DealsByStatusFilter`, `DealsByStatusModel`, `SalesDashboardSummary`, `SalesRecentClient`,
`SalesBarPoint`, `SalesMeter`, `SALES_PERIOD_TABS`. Enum fields travel as numeric values.

**UI:** `pages/agency/Dashboard.vue` (layout in SIGOOK_WEB_STRUCTURE.md). KPI definitions:
`.docs/business/SALES_MODULE.md`.

### Response shape

```jsonc
// GET /api/agency/sales/dashboard/deals-by-status?period=1
{
  "period": { "period": 1, "from": "2026-09-06T00:00:00", "to": "2026-09-12T00:00:00",
              "label": "Sep 6 - Sep 12, 2026" },
  "totalCount": 9,
  "totalValue": 41500.00,
  "items": [ { "status": 0, "count": 3, "totalValue": 12000.00 } ]  // one row per status, zero-filled
}

// GET /api/agency/sales/dashboard/summary
{
  "asOf": "2026-09-09T15:00:00Z",
  "quarter": { "period": 3, "from": "2026-07-01T00:00:00", "to": "2026-09-30T00:00:00", "label": "Q3 2026" },
  "week":    { "period": 1, "from": "2026-09-06T00:00:00", "to": "2026-09-12T00:00:00", "label": "Sep 6 - Sep 12, 2026" },
  "pipeline": [ { "status": 0, "count": 24, "totalValue": 180000.00 } ],  // 7 rows, enum order
  "activity": [ { "type": 0, "count": 42 } ]                              // 4 rows, enum order
}

// GET /api/agency/sales/dashboard/recent-clients
[
  { "id": "…", "fullName": "Acme", "email": "ops@acme.com", "lastInteractionAt": "2026-09-10T12:00:00" }
]  // up to 10, newest interaction first; clients without interactions never appear
```

`from`/`to` are **UTC calendar dates**, serialized without an offset so the browser renders them
verbatim; `to` is inclusive. Windows are resolved server-side by
`SalesService.GetPeriodWindow` (UTC, week Sunday–Saturday).

### Refresh behavior

```
Dashboard.vue onMounted
├─ getSalesDashboardSummary()      → GET .../dashboard/summary               [LIVE]
├─ getDealsByStatus({period})      → GET .../dashboard/deals-by-status       [LIVE]
├─ getRecentInteractions()         → GET .../dashboard/recent-interactions   [LIVE]
├─ getRecentClients()              → GET .../dashboard/recent-clients        [LIVE]
├─ getRecentDeals()                → GET .../dashboard/recent-deals          [LIVE]
└─ useCurrentAgent.loadAgentName() → GET /api/agency/profile/personnel                 [LIVE]

period tab change / status filter change → getDealsByStatus() only
{Interaction,Deal,Client,ClientInteractions}Modal @saved → onSaved → all five data loaders re-run
```

The whole dashboard is live; nothing is cached and there is no static JSON left. `loadDealsByStatus`
carries a request counter so a fast tab switch cannot render a stale response.

**Known limits:**
- An unbindable `period` (e.g. `?period=99`) falls back to the default week rather than returning 400 —
  the API sets `SuppressModelStateInvalidFilter`, so this matches every other filter. A value that
  *does* bind but is outside the enum is rejected by `GetDealsByStatusFilterValidator` with a 400.
- No quarterly goal exists in the schema, so the dashboard has no goal donut.
- The client form is create-only from the modal (no edit/delete path).

---

## 19. sharedApi.ts → `src/shared/api/sharedApi.ts`
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `unsubscribe(model)` | POST | `/api/emailpreferences/Unsubscribe` | `UnsubscribeRequest` (`email` + optional `typeId`) | `void` | No auth required; the backend resolves whether the email belongs to a user or a candidate, and defaults `typeId` to `NewRequestNotifyWorker` (10) when omitted |

---

## 20. weeklyBoardApi.ts — Recruiting Weekly Board

Board where the agency assigns orders to recruiters per work day, and each recruiter records the runners they sent. It is the only place recruiters are assigned. Admin board (`getWeeklyBoard`) shows all recruiters with counts; recruiter board (`getRecruiterWeeklyBoard`) is scoped to the recruiter from the token and includes the runners sent. Base: `/api/agency/recruiting/weeklyboard`.

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getWeeklyBoard(filter)` | GET | `/api/agency/recruiting/weeklyboard` | `WeeklyBoardFilter` (params) | `WeeklyBoard` | Admin board grouped by recruiter |
| `getRecruiterWeeklyBoard(filter)` | GET | `/api/agency/recruiting/weeklyboard/mine` | `WeeklyBoardFilter` (params) | `RecruiterWeeklyBoard` | Current recruiter's board + sent workers |
| `getRequestRunners(requestId)` | GET | `/api/agency/recruiting/weeklyboard/{requestId}/runners` | — | `WeeklyBoardRunner[]` | All runners sent to an order across every recruiter/day (whole history) |
| `assignRecruiters(payload)` | POST | `/api/agency/recruiting/weeklyboard` | `AssignRecruitersPayload` | `void` | Assign recruiter(s) to an order per day |
| `unassignRecruiter(payload)` | DELETE | `/api/agency/recruiting/weeklyboard` | `UnassignRecruiterPayload` (params) | `void` | Remove a day assignment |
| `moveAssignment(payload)` | POST | `/api/agency/recruiting/weeklyboard/move` | `MoveAssignmentPayload` | `void` | Move assignment to another recruiter/day (keeps its runners; drag & drop) |
| `addRunner(payload)` | POST | `/api/agency/recruiting/weeklyboard/runner` | `AddRunnerPayload` | `void` | Recruiter sends a runner (delegates to `IRunnerService.CreateRunner`); adds a request note `"{workerName} was sent"` |

**Types:** `WeeklyBoard`, `RecruiterWeeklyBoard`, `WeeklyBoardRecruiterRow`, `WeeklyBoardAssignment`, `WeeklyBoardDispatch`, `WeeklyBoardFilter`, `AssignRecruitersPayload`, `UnassignRecruiterPayload`, `MoveAssignmentPayload`, `DispatchWorkersPayload`, `RemoveWorkerPayload` (`src/modules/agency/recruiting/weekly-board/types`)

**UI:** `pages/agency/WeeklyBoard.vue` + `components/weekly_board/*`.

---

## 21. userNotificationApi.ts → `src/shared/api/userNotificationApi.ts`
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getUserNotifications()` | GET | `/api/usernotification` | — | `UserNotificationItem[]` | In-app inbox |
| `updateUserNotification(model)` | PUT | `/api/usernotification` | `UserNotificationItem` | `void` | Mark read/update |

**Types:** `UserNotificationItem` (`src/shared/types/common`)

---

## 22. websiteApi.ts → `src/modules/landing/api.ts`
Public landing site endpoints (no auth).

| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getJobs(filter)` | GET | `/api/website/jobs` | `JobSearchFilter` (params; `countries` defaults to `['USA','CA']`) | `JobViewModel[]` | Public job search |
| `submitContactForm(contact)` | POST | `/api/website/contact` | `ContactForm` | `void` | |
| `submitCandidate(formData)` | POST | `/api/website/candidate` | FormData (multipart) | `void` | Public candidate apply |

**Types:** `JobSearchFilter`, `JobViewModel`, `ContactForm` (`src/modules/landing/types`)

---

## 23. workerApi.ts → `src/modules/worker/requests/api.ts` + `src/modules/worker/profile/api.ts` + `src/modules/worker/register/api.ts` + `src/modules/worker/history/api.ts` + `src/shared/worker-profile/api.ts` (profile sections) + `src/modules/agency/recruiting/workers/api.ts` (wage/timesheet history)
**Large.** Worker portal: profile build, applications, timesheet.

### Requests (Job Applications)
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getJobs(filter)` | GET | `/api/worker/requests` | `WorkerRequestFilter` (params) | `PaginatedList<WorkerRequestListItem>` | Available jobs |
| `getWorkerRequest(id)` | GET | `/api/worker/requests/{id}` | — | `WorkerRequestDetail` | Includes `responsibilities`, `displayShift` and `skills` (names) |
| `workerRequestApplySelf(requestId, model)` | POST | `/api/worker/requests/{requestId}/Apply/` | `WorkerRequestApplyModel` | `void` | Self-apply; the worker comes from the token, `email` in the body is ignored |
| `requestApplyByEmail(numberId, email)` | POST | `/api/worker/requests/Apply` | `WorkerRequestApplyModel` | `void` | Anonymous invitation apply (`/worker-apply?n=&e=`): resolves the email to a worker of the request's agency first, then to a candidate (city-validated) |

### TimeSheet
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `workerRegisterTime(requestId, lat, lon)` | POST | `/api/worker/requests/{requestId}/timesheet` | `{ latitude, longitude }` | `void` | Clock-in with GPS |
| `workerGetTimeSheet(requestId)` | GET | `/api/worker/requests/{requestId}/timesheet` | — | `WorkerTimeSheetItem[]` | |
| `getClockType(requestId, latitude, longitude, date)` | GET | `/api/worker/requests/{requestId}/timesheet/clock-type/{latitude}/{longitude}` | `date` (param) | `ClockType` (enum, `src/constants/enums`) | Can clock in/out? Coordinates resolve the time zone |

### Comments
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getMyComments(filter)` | GET | `/api/worker/comments` | `WorkerCommentFilter` (params) | `WorkerCommentList` | Feedback written about the signed-in worker |

### Profile
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getMyProfile()` | GET | `/api/worker/profile/me` | — | `WorkerProfileDetail` | |
| `registerWorker(payload, requestId?)` | POST | `/api/worker/profile?requestId=` | FormData (multipart) | `string` (profile id) | Registration; `requestId` (request number from `/register-worker/:requestId`) also adds the new worker as applicant |

### Request History
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `getWorkerRequestHistory(filter)` | GET | `/api/worker/history` | `WorkerRequestFilter` (params) | `PaginatedList<WorkerRequestListItem>` | |
| `getWorkerRequestHistoryDetail(id)` | GET | `/api/worker/history/{id}` | — | `WorkerRequestDetail` | |

### Job Experience
| Function | HTTP Method | Endpoint | Request Type | Response Type | Notes |
|----------|------------|----------|--------------|---------------|-------|
| `createWorkerWorkExperience(id, model)` | POST | `/api/worker/profile/{id}/JobExperience` | `WorkerJobExperienceModel` | `void` | |
| `editWorkerWorkExperience(id, expId, model)` | PUT | `/api/worker/profile/{id}/JobExperience/{expId}` | `WorkerJobExperienceModel` | `void` | |
| `deleteWorkerWorkExperience(id, expId)` | DELETE | `/api/worker/profile/{id}/JobExperience/{expId}` | — | `void` | |

### SIN / Documents (all FormData multipart)
| Function | HTTP Method | Endpoint | Notes |
|----------|------------|----------|-------|
| `createWorkerSin(id, formData)` | POST | `/api/worker/profile/{id}/SinInformation` | Canadian SIN |
| `createWorkerDocuments(id, formData)` | POST | `/api/worker/profile/{id}/Documents` | |
| `createWorkerResume(id, formData)` | POST | `/api/worker/profile/{id}/Resume` | |
| `createWorkerLicenses(id, formData)` | POST | `/api/worker/profile/{id}/Licenses` | |
| `deleteWorkerLicenses(id, licenseId)` | DELETE | `/api/worker/profile/{id}/Licenses/{licenseId}` | |
| `createWorkerCertificates(id, formData)` | POST | `/api/worker/profile/{id}/Certificates` | |
| `deleteWorkerCertificates(id, certId)` | DELETE | `/api/worker/profile/{id}/Certificates/{certId}` | |
| `createWorkerOtherDocuments(id, formData)` | POST | `/api/worker/profile/{id}/OtherDocument` | |
| `deleteWorkerOtherDocuments(id, docId)` | DELETE | `/api/worker/profile/{id}/OtherDocument/{docId}` | |
| `createWorkerImage(id, formData)` | POST | `/api/worker/profile/{id}/ProfileImage` | Profile picture |

### Profile Sections
| Function | HTTP Method | Endpoint | Request Type |
|----------|------------|----------|--------------|
| `createWorkerBasicInformation(id, model)` | POST | `/api/worker/profile/{id}/BasicInformation` | `WorkerBasicInformationModel` |
| `createWorkerContactInformation(id, model)` | POST | `/api/worker/profile/{id}/ContactInformation` | `WorkerContactInformationModel` |
| `createWorkerEmergencyInformation(id, model)` | POST | `/api/worker/profile/{id}/EmergencyInformation` | `WorkerEmergencyInformationModel` |
| `createWorkerOther(id, model)` | POST | `/api/worker/profile/{id}/OtherInformation` | `WorkerOtherInformationModel` |

### Preferences & Skills
| Function | HTTP Method | Endpoint | Request Type |
|----------|------------|----------|--------------|
| `createWorkerAvailabilities(id, model)` | POST | `/api/worker/profile/{id}/Availabilities` | `WorkerCatalogItem[]` |
| `createWorkerAvailabilityTimes(id, model)` | POST | `/api/worker/profile/{id}/AvailabilityTimes` | `WorkerCatalogItem[]` |
| `createWorkerAvailabilityDays(id, model)` | POST | `/api/worker/profile/{id}/AvailabilityDays` | `WorkerCatalogItem[]` |
| `createWorkerLocationPreferences(id, model)` | POST | `/api/worker/profile/{id}/LocationPreferences` | `WorkerCatalogItem[]` |
| `createWorkerLanguages(id, model)` | POST | `/api/worker/profile/{id}/Languages` | `WorkerCatalogItem[]` |
| `createWorkerSkills(id, model)` | POST | `/api/worker/profile/{id}/Skills` | `string[]` |

### Wage & TimeSheet History
| Function | HTTP Method | Endpoint | Request Type | Response Type |
|----------|------------|----------|--------------|---------------|
| `getWorkerProfileWageHistory(filter)` | GET | `/api/agency/recruiting/workers/{profileId}/WageHistory` | `WageHistoryFilter` (params) | `PaginatedList<WorkerWageHistoryItem>` |
| `getWorkerProfileWageHistoryAccumulated(id, rowNumber)` | GET | `/api/agency/recruiting/workers/{id}/WageHistory/{rowNumber}` | — | `WorkerWageHistoryItem` |
| `getWorkerProfileTimeSheetHistory(filter)` | GET | `/api/agency/recruiting/workers/{profileId}/TimeSheetHistory` | `TimeSheetHistoryFilter` (params) | `PaginatedList<WorkerTimeSheetHistoryItem>` |
| `getWorkerProfileTimeSheetHistoryAccumulated(id, rowNumber)` | GET | `/api/agency/recruiting/workers/{id}/TimeSheetHistory/{rowNumber}` | — | `WorkerTimeSheetHistoryItem` |

**Types:** from `src/shared/worker-profile/types`, `worker/requests/types`, `agency/recruiting/workers/types`; `ClockType` enum from `src/constants/enums`.

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
| **Sales** | salesApi.ts, salesDashboardApi.ts, companyApi.ts (deals/interactions) | Sales-scoped lists + Excel export; live dashboard aggregates (deals by status, quarter summary); deals + interactions CRUD (owner-scoped) |
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
| `requestPasswordResetCode(email)` | POST | `/Password/forgot` | `{ email }` | `void` | 202 even when nothing is sent (60-s cooldown, 3 codes/hour and 6/day per user server-side); 429 past 10 requests per IP in 10 min |
| `resetPasswordWithCode(payload)` | POST | `/Password/reset` | `ResetPasswordWithCodePayload` | `void` | 400 → `PasswordResetErrorResponse` with `error` ∈ `invalid_code`, `code_expired`, `too_many_attempts`, `password_policy` (+ `messages`); 429 past 10 requests per IP in 10 min (shared with `/Password/forgot`) |
| `resendConfirmationLink(email)` | POST | `/Account/ResendConfirmationLink?userName=` | query param | `void` | Called from the `email_not_confirmed` login error; always 200 |
| `confirmEmail(payload)` | POST | `/Account/ConfirmEmail` | `ConfirmEmailPayload` | `void` | `/confirm-email` page; 400 → `PasswordResetErrorResponse` with `error = invalid_token`; already-confirmed users get 200 |
| `createPassword(payload)` | POST | `/Account/CreatePassword` | `CreatePasswordPayload` | `void` | `/create-password` page; also confirms the email. 400 → `error` ∈ `invalid_token`, `password_policy` (+ `messages`) |

**Types:** `TokenResponse`, `TokenErrorResponse`, `UserInfoResponse`, `ResetPasswordWithCodePayload`, `ConfirmEmailPayload`, `CreatePasswordPayload`, `PasswordResetErrorResponse` (`src/shared/types/security`)
