# Sigook.Web Codebase Structure

Overview of Vue 3 agency/worker portal for Covenant/Sigook staffing platform. Stack: Vite, TypeScript, Pinia, Vue Router 4, `@ntohq/buefy-next`, VeeValidate 4 + Yup, oidc-client-ts.

---

## Project Root

```
Sigook.Web/
├── src/
│   ├── api/                # Plain function API wrappers
│   ├── assets/             # Images, fonts, SCSS
│   ├── components/         # Reusable Vue components by domain
│   ├── composables/        # Composition API utilities
│   ├── constants/          # Constants
│   ├── directives/         # Custom Vue directives
│   ├── filters/            # Vue filters
│   ├── lang/               # i18n translations
│   ├── mixins/             # Vue mixins
│   ├── pages/              # Page/view components (routable)
│   ├── resolvers/          # Route resolvers (pre-load data)
│   ├── router/             # Vue Router config
│   ├── security/           # Auth, API service, roles
│   ├── store/              # Pinia stores
│   ├── types/              # TypeScript interfaces
│   ├── utils/              # Utility functions
│   ├── App.vue             # Root component
│   ├── main.ts             # App entry point
│   └── varaibles.ts        # Global app config
├── public/                 # Static assets
├── node_modules/
├── CLAUDE.md               # Original notes (to be complemented by this doc)
├── package.json
├── tsconfig.json
├── vite.config.ts
└── Dockerfile
```

---

## src/api/ — API Layer

**Purpose:** Plain TypeScript functions wrapping HTTP calls to Covenant.Api backend.

**Pattern:** Direct imports, no service locator. Each function is a dedicated endpoint wrapper.

### File Organization

| File | Entity | Scope |
|------|--------|-------|
| **accountApi.ts** | Account | Email, deactivation |
| **agencyApi.ts** | Agency | Profile, personnel, locations, agency switching |
| **agencyCandidateApi.ts** | Candidate | CRUD, phone, skills, docs, bulk upload |
| **agencyCompanyApi.ts** | Company (Agency view) | CRUD, locations, contacts, job positions, docs, settings, users |
| **agencyInvoiceApi.ts** | Invoice (Agency) | List, create, preview, delete, PDF, verify, email |
| **agencyNoteApi.ts** | Notes | Worker/Candidate/Company/Request notes (mixed CRUD) |
| **agencyPayStubApi.ts** | PayStub (Agency) | CRUD, generation, subcontractor report, skip numbers |
| **agencyReportApi.ts** | Reports | T4, CRA, timesheet, hours, payment, payroll Excel |
| **agencyRequestApi.ts** | Request | CRUD, workers, applicants, recruiters, skills, shift |
| **agencyTimeSheetApi.ts** | TimeSheet (Agency) | CRUD, by date range, usages |
| **agencyWorkerApi.ts** | Worker (Agency view) | List, flags (DNU, contractor), tax, email, holidays |
| **catalogApi.ts** | Reference data | Enums: gender, ID type, availability, jobs, skills, industries, tax categories |
| **companyApi.ts** | Company (Company view) | Profile, requests, workers, timesheet, users, contacts, invoices |
| **downloadApi.ts** | Downloads | Invoice PDF, payroll Excel (various formats) |
| **locationApi.ts** | Location | Countries, provinces, cities, provincial settings |
| **requestApi.ts** | Request | Minimal; shift lookup only |
| **sharedApi.ts** | Email Preferences | Unsubscribe |
| **userNotificationApi.ts** | Notification | In-app notifications |
| **websiteApi.ts** | Website (Public) | Job search, contact form, candidate apply, static job positions |
| **workerApi.ts** | Worker (Worker view) | Profile complete, job apply, timesheet, wage/shift history |

**Key Design:**
- All functions use `http` instance from `@/security/apiService`
- Return types are TypeScript (Promise<T>)
- No store dispatches; components call functions directly
- Pinia only stores **filters** for pagination/search and auth state

---

## src/types/ — TypeScript Interfaces

**Purpose:** Type definitions for API request/response payloads.

| File | Contains |
|------|----------|
| **common.ts** | `PaginatedList`, `Country`, `Province`, `City`, `Gender`, `IdentificationType`, `Availability`, `AvailabilityTime`, `Day`, `Lift`, `Language`, `WsibGroup`, `Industry`, `JobPosition`, `Skill`, `CancellationReason`, `CatalogItem`, `CovenantFileModel`, etc. |
| **agency.ts** | `AgencyProfile`, `AgencyLocation`, `AgencyPersonnel`, `AgencyRequest*`, `AgencyWorker*`, `AgencyCompany*`, `AgencyCandidate*`, `Note*`, `CreateCandidateDocumentPayload`, `VaccinationRequiredModel`, `InvoiceNotesModel`, `InvoiceRecipientModel`, `CompanyProvinceWithTaxes`, etc. |
| **accounting.ts** | `PayStub*`, `CreatePayStubPayload`, `AgencyInvoice*`, `CreateAgencyInvoiceModel`, `InvoiceSummaryModel`, `DeleteInvoicePayload`, `PayrollSubContractor*`, `SkipPayrollNumber*`, `WorkerReadyForPayStub*`, `AgencyReportFilter`, `HoursWorkedResume`, `WeeklyPayrollItem` |
| **company.ts** | `CompanyProfile*`, `CompanyProfileLocation*`, `CompanyProfileJobPosition*`, `CompanyRequest*`, `CompanyRequestWorker*`, `TimeSheet*`, `ClockInModel`, `ClockInResult`, `CompanyUser*`, `CompanyContactPerson*`, `CompanyInvoice*`, `CommentsModel` |
| **worker.ts** | `WorkerProfile`, `WorkerBasicInformation*`, `WorkerContactInformation*`, `WorkerEmergencyInformation*`, `WorkerOtherInformation*`, `WorkerJobExperience*`, `WorkerRequest*`, `WorkerTimeSheet*`, `WorkerWageHistory*`, `WorkerCommentList`, `ClockTypeResult` |
| **candidate.ts** | `Candidate`, `CandidateDocument`, `CreateCandidateDocumentPayload`, `AgencyCandidateFilter`, `CreateCandidateModel`, `CandidatePhoneNumberModel`, `CandidateSkillModel` |
| **security.ts** | `ChangeEmailRequest`, `GetEmailResponse` |
| **website.ts** | `JobSearchFilter`, `JobViewModel`, `ContactForm`, `LandingJobPositions` |

**Convention:** Plural names for arrays/lists (e.g., `AgencyPersonnelListItem[]`). Singular for detail objects.

---

## src/router/ — Route Configuration

**Purpose:** Vue Router setup; lazy-loaded pages by feature area.

### File Organization

| File | Routes | Components |
|------|--------|-----------|
| **index.ts** | Root routes, auth guard, scroll behavior | NotFound, SilentRefresh, Unauthorized, EmailPreferences, Callback |
| **routesAgency.ts** | `/agency-*`, `/accounting/*`, etc. | Agency portal (requests, workers, companies, candidates, invoices, paystubs, reports) |
| **routesCompany.ts** | `/company-*`, `/request/*` | Company portal (requests, reports, profile, users) |
| **routesWorker.ts** | `/register-worker`, `/worker-*`, `/punch-card`, `/timesheet` | Worker portal (profile, job search, applications, timesheet, history) |
| **routesLanding.ts** | `/home`, `/jobSeekers`, `/business`, `/about-us`, `/contact` | Public landing pages |

**Auth Guard:**
```typescript
// Requires auth + specific role(s)
meta: { requiresAuth: true, role: [agency, agencyPersonnel] }

// Public
meta: { requiresAuth: false, layout: "web" }
```

**Route Resolvers:** Pre-load data before entering route (e.g., `loadAgencyCompaniesResolver` fetches company list).

---

## src/pages/ — Routable Views

**Organization:** Feature-based folders. Each folder = feature area.

### Agency Portal (`src/pages/agency/`)

**Core Pages:**
- **Requests.vue** — List job requests; filter, search, pagination
- **Request.vue** — Request detail; workers, applicants, shift, notes, recruiters
- **AgencyCreateRequest.vue** — Create/edit request; company, position, shift, workers quantity, requirements
- **Workers.vue** — Worker roster; list, search, profile access, flags (DNU, contractor)
- **DetailWorker.vue** — Worker detail; profile, request history, notes, holidays, email
- **Companies.vue** — Company client list; search, create, bulk import
- **CreateCompany.vue** — Create/edit company; profile, locations, contacts, job positions, settings
- **DetailCompany.vue** — Company detail; locations, contacts, job positions, requests, docs, notes
- **Candidates.vue** — Candidate pool; list, search, convert to worker, bulk import
- **Agencies.vue** — Sub-agencies (if master agency); list, create, detail
- **AgencyProfile.vue** — Current agency profile; edit, locations, personnel, email
- **accounting/Invoices.vue** — Invoice list; filter, create, delete, preview, email, PDF download
- **accounting/CreateInvoice.vue** — Invoice creation; select timesheets, preview, generate
- **accounting/PayStubs.vue** — Pay stub list; filter, create, generate, delete, email, PDF, subcontractor report
- **accounting/CreatePayStub.vue** — Pay stub creation; worker, period, hours, deductions
- **accounting/Reports.vue** — Report generation; T4, CRA, timesheet, hours worked, payment, payroll export

### Company Portal (`src/pages/company/`)

- **Requests.vue** — Company's job requests; list, search, status
- **Request.vue** — Request detail from company POV; workers, timesheet, comments
- **CreateRequest.vue** — Submit new job request
- **CompanyReports.vue** — Company invoices + reports
- **CompanyProfile.vue** — Company profile; edit, locations, contact people, users
- **CompanyUserProfile.vue** — Current user's profile within company

### Worker Portal (`src/pages/worker/`)

- **Register.vue** — Worker registration form; multi-step profile build
- **Requests.vue** — Job search; list available requests, filter, apply
- **Request.vue** — Job detail; description, apply
- **RequestApplied.vue** — Applied job status
- **PunchCard.vue** — Mobile punch clock-in/out (GPS)
- **TimeSheet.vue** — Worker's timesheet; view hours, daily breakdown
- **History.vue** — Past jobs, applications, wage history
- **WorkerProfile.vue** — Profile view/edit; skills, experience, availability, documents
- **WorkerApply.vue** — Public job application (pre-registration)

### Landing Pages (`src/pages/landing/`)

- **Home.vue** — Hero, featured jobs, CTA
- **JobSeekers.vue** — Job search for workers
- **JobSeekersJobPosition.vue** — Jobs by position (parameterized)
- **Business.vue** — Staffing for companies
- **BusinessJobPosition.vue** — Staffing info by position
- **DirectHiring.vue** — Direct hire service info
- **AboutUs.vue** — Company info
- **Contact.vue** — Contact form
- **Atas.vue** — Terms/compliance (ATAS = something specific to Sigook)
- **TermsAndConditions.vue** — T&C
- **PrivacyPolicy.vue** — Privacy

### Shared Pages

- **Callback.vue** — OAuth callback (redirect from identity provider)
- **SilentRefresh.vue** — Hidden iframe for token refresh
- **Unauthorized.vue** — 403 error
- **NotFound.vue** — 404 error
- **EmailPreferences.vue** — Unsubscribe from emails (no auth required)

---

## src/components/ — Reusable Components

**Organization:** Feature-based domain folders + shared root-level components.

### Root-Level (Shared Across App)

- **Address.vue** — Address input form
- **Comments.vue** — Comment display/interaction
- **CompanyCreateUserModal.vue** — Modal to add company user
- **CropImage.vue** — Image cropping tool
- **DataEntryTerms.vue** — Terms agreement checkbox
- **DefaultImage.vue** — Placeholder image
- **DialogWorkerComment.vue** — Modal for worker comments
- **EmailCard.vue** — Email display card
- **Export.vue** — Report export (generic)

### `agency/` — Agency-specific Components

**Purpose:** Components for agency portal operations.

- **AgencyCreatePersonnelModal.vue** — Add agency staff
- **AgencyPersonnel.vue** — Personnel list + management
- **AgencyPunchCard.vue** — Punch card UI for workers
- **AgencyRequests.vue** — Request list view
- **AgencyWorkerRequestHistory.vue** — Worker's past requests (list)
- **AgencyWorkerRequestHistoryContainer.vue** — Container for history
- **AgencyWorkers.vue** — Worker list main component
- **AgencyWorkersList.vue** — Worker list sub-component
- **BulkData.vue** — Bulk upload/import interface
- **ConfirmProfileMembership.vue** — Confirm agency access
- (More specific sub-components for detail views, forms, modals...)

### `agency_accounting/` — Accounting Components

- **CRAPayroll.vue** — CRA payroll report
- **DeleteInvoice.vue** — Invoice deletion confirmation
- **GeneratePayStubs.vue** — Pay stub batch generation
- **HoursWorkedReport.vue** — Hours worked summary
- **PaymentReport.vue** — Payment history
- **PreviewInvoice.vue** — Invoice preview before creation
- **SendInvoiceEmail.vue** — Email invoice modal
- **SkipPayrollNumber.vue** — Manage skipped payroll numbers
- **SubcontractorsReport.vue** — Subcontractor payroll
- **T4.vue** — T4 tax report

### `agency_company/` — Company Management Components

- **CompanyDetailTab.vue** — Company detail tabbed layout
- **CompanyNotes.vue** — Company notes
- **CompanyRequests.vue** — Company's requests (from agency view)
- **CompanySettings.vue** — Company settings (holidays, overtime, permissions)
- **CompanyUpdateLogo.vue** — Logo upload
- **CompanyWorkers.vue** — Workers at company
- **ContactInformation.vue** — Contact info display
- **ContactInformationForm.vue** — Contact info form
- **ContactPersonForm.vue** — Individual contact person form
- **ContactPersonList.vue** — Contact persons list
- (More location, document, job position components...)

### `agency_request/` — Request Management Components

- **AgencyPunchCardWorkerContainer.vue** — Punch card container for agency context
- **AgencyRequestDetail.vue** — Request detail main component
- **AgencyRequestSkills.vue** — Request skills list + management
- **AgencyRequestTimeSheetDetail.vue** — Timesheet display
- **AgencyRequestTimeSheetModal.vue** — Timesheet entry modal
- **AgencyShiftDetail.vue** — Shift info display
- **Applicants.vue** — Request applicants list
- **ContactListModal.vue** — Modal to select contacts (RequestedBy/ReportTo)
- **DatepickerModal.vue** — Date picker
- (More worker assignment, recruiter, note components...)

### `calendar/` — Calendar Components

- **CalendarPunchCard.vue** — Calendar view for punch card

### `candidate/` — Candidate Components

- **CreateCandidate.vue** — Candidate registration form
- **DetailAddress.vue** — Candidate address detail
- **DetailCandidate.vue** — Candidate profile detail
- **ModalCandidateRequests.vue** — Candidate's job applications modal
- **ModalDocuments.vue** — Candidate documents modal

### `company/` — Company Portal Components

- **CompanyCancelList.vue** — Cancelled requests
- **CompanyInvoices.vue** — Company invoice list
- **CompanyUsers.vue** — Company user management
- **CompanyUserUpdate.vue** — Update company user
- **ContainerWorkerCo.vue** — Worker container (company view)
- **DialogCompanyUpdateEmail.vue** — Company email update modal
- **DialogReplaceWorker.vue** — Replace worker modal
- **DialogRequestWorker.vue** — Request replacement worker modal
- **ProfileBusiness.vue** — Company profile (business info)
- **ProfileContact.vue** — Company profile (contact info)
- (More invoicing, timesheet components...)

### `company_request/` — Company Request Components

- **CompanyPunchCardWorkerContainer.vue** — Punch card container (company view)
- **CompanyRequestDetail.vue** — Request detail (company view)
- **CompanyRequestPunchCard.vue** — Punch card display
- **CompanyRequestTimeSheetDetail.vue** — Timesheet (company view)
- **CompanyRequestTimeSheetModal.vue** — Timesheet modal
- **CompanyRequestWorkers.vue** — Assigned workers (company view)

### `landing/` — Landing Page Components

- **ApplyNow.vue** — CTA button for applications
- **Footer.vue** — Site footer
- **Header.vue** — Site header/nav
- **JobPosition.vue** — Job position card
- **JobSearch.vue** — Job search form
- **NeedStaff.vue** — CTA section for companies
- **SigookVideo.vue** — Video embed
- **SubMenu.vue** — Navigation submenu

### `notes/` — Notes Components

- **ColorPicker.vue** — Note color selector
- **ModalNotes.vue** — Notes display modal
- **NoteForm.vue** — Note creation/edit form

### `request/` — Generic Request Components

- **ButtonSort.vue** — Sort button
- **RequestDetail.vue** — Request detail (generic)
- **RequestLocation.vue** — Request location info
- **ShiftDetail.vue** — Shift display
- **ShiftEditModal.vue** — Shift edit modal
- **ShiftsForm.vue** — Shifts form

### `worker/` — Worker Portal Components

- **Notes.vue** — Worker's notes
- **ProfileComments.vue** — Comments on worker
- **ProfileExperience.vue** — Work experience list
- **ProfilePersonal.vue** — Personal info section
- **ProfilePreferences.vue** — Availability + location preferences
- **RequestDetail.vue** — Job detail (worker view)
- **TimeSheetHistory.vue** — Past timesheets
- **WorkAvailabilitiesDetail.vue** — Availability display
- **WorkAvailabilitiesForm.vue** — Availability form
- **WorkAvailabilityDaysDetail.vue** — Work days display
- (More skill, language, document components...)

**Pattern:** Components accept `function` refs (e.g., `onSave={agencyCandidateApi.updateAgencyCandidate}`) rather than store dispatch strings. Generic components are reusable across features.

---

## src/store/ — Pinia State Management

**Organization:** One store per domain. Minimal state (mostly filters for pagination). Persistence via `pinia-plugin-persistedstate`.

### Root Setup (`src/store/index.ts`)

Creates the Pinia instance and registers the persisted-state plugin; individual stores self-register via `defineStore`.

### Stores (`src/store/modules/`)

| Store | State | Purpose |
|-------|-------|---------|
| **agency.ts** | `agency: AgencyProfile`, `personnelAgencies: []`, `agencyRequestFilter`, `agencyCandidateFilter`, `agencyWorkerProfileFilter`, `agencyCompanyProfileFilter`, `agencyInvoiceFilter`, `agencyPayStubFilter`, `agencyListFilter` | Agency context + list filters |
| **company.ts** | `companyRequestFilter` | Company context + request filter |
| **worker.ts** | `workerProfile: Partial<WorkerProfile>` | Worker context + partial profile |
| **security.ts** | (Auth state) | User, token, roles |

**Design Philosophy:**
- **Filters only**: Pinia stores UI state (pagination, search filters) so lists don't reset on route changes
- **No data caching**: API responses stored in component state or computed from live data
- **Auth as exception**: Security store manages JWT + user roles (app-wide requirement)

---

## src/security/ — Authentication & API Config

### apiService.ts

**Axios instance** with interceptors:
- **Request:** Adds `Authorization` header with JWT token
- **Response:** 
  - 401 → Silent refresh via the security Pinia store's `silentSignin` action
  - 403 → Alert "not authorized"
  - 500 → Alert error + handle blob responses
  - Adds `accept-language` header from localStorage

**Key:** Auth is transparent to components; API functions don't manage tokens.

### roles.ts

Enum of role strings for route guards:
- `agency`, `agencyPersonnel`, `payroll`, `admin` (agency-side)
- `company`, `companyUser` (company-side)
- `worker` (worker-side)

### securityService.ts

User/auth service (delegates to store).

### menu.ts

Navigation structure by role (agency, company, worker menus).

---

## src/utils/ — Utility Functions

| File | Purpose |
|------|---------|
| **compressFile.ts** | File compression before upload |
| **confirmationGuard.ts** | Unsaved changes warning |
| **directHiring.ts** | Direct hire specific logic |
| **distributeHours.ts** | Timesheet hour distribution logic |
| **downloadFile.ts** | File download helper |
| **fileUpload.ts** | File upload helper |
| **filters.ts** | Vue filters (formatters) |
| **recaptcha.ts** | reCAPTCHA integration |
| **timeSheetApprove.ts** | Timesheet approval workflow |
| **workerStatus.ts** | Worker status helpers |

---

## src/composables/ — Vue Composition Utilities

| File | Purpose |
|------|---------|
| **useBillingAdmin.ts** | Billing admin role check |
| **useCreateWorker.ts** | Worker registration composable |
| **usePubSub.ts** | Pub/sub event system |

---

## src/lang/ — i18n Localization

**Files:** `*.json` for each language (en-US, fr-CA, etc.)

**Usage:** `{{ $t('key.path') }}` in templates

**Coverage:** UI labels, error messages, placeholder text

---

## src/assets/ — Static Resources

```
assets/
├── fonts/
│   └── open-sans/      # Web fonts
├── images/
│   ├── banners/        # Hero images
│   ├── default/        # Placeholders
│   ├── home_carousel/  # Landing carousel
│   ├── main_banner/    # Main banner
│   └── positions/      # Job position icons
│       ├── business/
│       └── jobSeekers/
└── scss/
    └── worker/         # Worker portal styles
```

---

## src/directives/ — Custom Vue Directives

- **status-directive.ts** — Status badge rendering (color-coded)
- **cleave-directive.ts** — Input formatting (phone, dates, etc.)

---

## src/mixins/ — Legacy Mixins (being phased out)

Vue 3 discourages mixins; remaining ones act as composable-style helpers until fully migrated.

- **toastMixin.ts** — Toast notification helper (app-wide)

---

## src/constants/ — Constants

Global constants, enums, configuration values.

---

## src/resolvers/ — Route Resolvers

Pre-load data before route entry (route guard).

**Examples:**
- `loadAgencyCompaniesResolver` → `getAgencyCompanies(filter)` before `/agency-companies`
- `loadCompanyToUpdateResolver` → `getAgencyCompany(id)` before `/update-company/:id`
- `loadAgencyRequestToUpdateResolver` → `getAgencyRequest(id)` before `/agency-update-request/:id`

---

## Top-Level Files

| File | Purpose |
|------|---------|
| **App.vue** | Root layout (`<script setup>`); main nav, sidebar, router-view |
| **main.ts** | Vue entry point; installs Pinia, Vue Router 4, `@ntohq/buefy-next`, i18n, VeeValidate, VueRecaptcha, etc. |
| **varaibles.ts** | Global config (API URLs, app version, feature flags, etc.) |
| **CLAUDE.md** | Original dev notes |

---

## Global Plumbing

### Authentication Flow

1. **Login:** Via OAuth (identity provider)
2. **Silent Refresh:** On 401, refresh JWT silently via `silentSignin` action
3. **Role-Based Routing:** `meta.role` check in router guard
4. **Token Storage:** localStorage (handled by store)

### API Error Handling

- **401 Unauthorized:** Auto-refresh; if fails, redirect to login
- **403 Forbidden:** Alert user + reject promise
- **500 Server Error:** Alert user; for blob responses, reject with error message

### Layout System

- **Layout modes:** `web` (landing), default (portal)
- **Sidebar:** Only in portal layouts (agency, company, worker)
- **Header:** Global header with user menu + language selector

### i18n

- **Default:** English (en-US)
- **Switchable:** Via header language dropdown
- **Storage:** localStorage `language` key
- **Validator:** Custom i18n messages for form validation

### Styling

- **Framework:** Bootstrap 4 (CSS, legacy — being phased down)
- **Component UI:** `@ntohq/buefy-next` (Buefy port for Vue 3, Bulma-based)
- **Custom SCSS:** `/src/assets/scss/worker/` + component-level styles
- **BEM Convention:** Class naming (likely)

---

## Feature Flows (High-Level)

### Recruitment → Staffing → Payroll

1. **Company** requests workers (CompanyApi.createRequest → Requests.vue)
2. **Agency** posts job (AgencyRequestApi.postAgencyRequest → AgencyCreateRequest.vue)
3. **Candidates** apply (WorkerApi.workerRequestApplySelf)
4. **Agency** books workers (AgencyRequestApi.bookAgencyRequestWorker)
5. **Workers** track hours (AgencyTimeSheetApi / CompanyApi timesheet endpoints)
6. **Agency** generates paystubs (AgencyPayStubApi.generatePayStubs)
7. **Agency** invoices company (AgencyInvoiceApi.createAgencyInvoice)
8. **Workers** view wage history (WorkerApi.getWorkerProfileWageHistory)
9. **Companies** view invoices (CompanyApi.getCompanyInvoice)

### Worker Profile Build

1. **Public Register** (WorkerApi.registerWorker → Register.vue)
2. **Fill Sections:**
   - Basic info → WorkerApi.createWorkerBasicInformation
   - Contact → WorkerApi.createWorkerContactInformation
   - Experience → WorkerApi.createWorkerWorkExperience
   - Skills → WorkerApi.createWorkerSkills
   - Availability → WorkerApi.createWorkerAvailabilities
   - Documents → WorkerApi.createWorkerDocuments
   - etc.
3. **View Profile** (WorkerApi.getMyProfile → WorkerProfile.vue)

### Candidate Conversion

1. **Agency imports** candidates (AgencyCandidateApi.bulkAgencyCandidates)
2. **Agency adds** candidate (AgencyCandidateApi.createAgencyCandidate)
3. **Agency converts** to worker (AgencyCandidateApi.convertCandidateToWorker)
4. **Worker** can now apply to jobs

---

## Key Decision Points

### No Central Cache for API Data

- Components fetch directly via API functions
- Filters cached in Pinia (pagination state)
- Data flows via component `data()` or `computed`
- **Pro:** Less boilerplate, easier to trace data flow
- **Con:** Harder to share state across distant components (solved by passing props or event bus)

### Pagination via Query Params

- Routes include `page`, `size`, `sort` in URL
- Store remembers last filter for UX (e.g., return to page 3 after detail view)
- List components read from store and re-fetch on param change

### Role-Based UI (Not Just Route Guards)

- `meta.role` restricts route access
- Components conditionally render via the security Pinia store's `role`
- E.g., payroll staff see "Generate Pay Stubs" button; recruiter doesn't

### Form Pattern

- Multi-step forms in register workflows
- Direct API calls on submit (no store dispatch)
- Toast notifications on success/error (toastMixin)
- Validation via `vee-validate` 4 + Yup schemas, with i18n messages

### Reusable Components via Function Props

```vue
<DetailWorker @save="agencyWorkerApi.updateAgencyWorker" />
```

Rather than:
```vue
<DetailWorker @save="store.updateWorker" />
```

Allows generic components to work across different backends.

---

## Testing & Development Notes

- **Build Tool:** Vite (vite.config.ts)
- **Type Checking:** TypeScript (`npm run type-check`, tsconfig.json)
- **Linting:** ESLint (`npm run lint`)
- **Package Manager:** npm (package-lock.json)
- **Docker:** Dockerfile included (multi-stage Node build → Nginx)
- **nginx.conf:** Routing config for SPA (history mode)

---

## Deployment

1. **Build:** `npm run staging` / `npm run production` → `/wwwroot/` (Vite)
2. **Docker:** `docker build -t sigook-web .`
3. **Environment:** `.env.staging`, `.env.production` — Vite `VITE_*` vars (e.g. `VITE_URL_API`)
4. **Server:** nginx; serves `index.html` for all 404s (SPA routing)

---

## Known Patterns & Conventions

| Aspect | Pattern | Example |
|--------|---------|---------|
| **Component Naming** | PascalCase.vue | DetailWorker.vue, AgencyRequests.vue |
| **Pinia Store Naming** | camelCase.ts | agency.ts, company.ts, worker.ts |
| **API Function Naming** | camelCase, verb-first | getAgencyWorkers(), postAgencyRequest(), updateAgencyRequest() |
| **Route Naming** | kebab-case | `/agency-requests`, `/company-profile` |
| **Type Naming** | PascalCase, entity + suffix | `AgencyRequestDetail`, `CompanyProfileListItem`, `CreatePayStubPayload` |
| **i18n Keys** | dot notation | `form.labels.workerName`, `errors.unauthorized` |
| **Class Names** | BEM or utility-based | TBD (inspect components) |

---

## Areas for Future Investigation

1. **Real-time Updates:** WebSocket usage for notifications? (Check `usePubSub.ts`)
2. **File Upload Progress:** How are large file uploads handled?
3. **Offline Mode:** Service Worker setup?
4. **Performance:** Code splitting, lazy loading status?
5. **Accessibility:** a11y compliance level?
6. **Testing:** Unit/E2E test coverage?
7. **Error Tracking:** Sentry or similar?
8. **Analytics:** GA or custom tracking?

---

## Summary

**Sigook.Web** is a Vue 3 SPA for a staffing platform with three portals (Agency, Company, Worker). It follows a functional API layer pattern with minimal Pinia state (filters + auth only), role-based routing, and reusable components. The architecture prioritizes developer ergonomics over strict patterns, with direct API imports and inline callbacks reducing boilerplate. Internationalization, form validation (VeeValidate + Yup), and toast notifications are app-wide; styling is Bootstrap + `@ntohq/buefy-next` with custom SCSS. Built with Vite.

**Key Strengths:**
- Clear separation of concerns (api, components, types, routes)
- Type-safe API layer (no `any` types)
- Flexible, reusable component structure

**Key Challenges:**
- No centralized data store for API responses (scattered component state)
- Inconsistent endpoint naming/versioning in backend
- Limited test coverage (likely)