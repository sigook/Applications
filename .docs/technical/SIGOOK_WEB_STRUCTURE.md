# Sigook.Web Codebase Structure

Vue 3 SPA with three logged-in portals (Agency, Company, Worker) plus a public landing site. Stack: Vite, TypeScript, Pinia, Vue Router 4, `@ntohq/buefy-next`, VeeValidate 4 + Yup, oidc-client-ts, Bootstrap 5 CSS.

---

## Project Root

```
Sigook.Web/
├── src/
│   ├── api/                # Plain function API wrappers (see SIGOOK_WEB_API_MAP.md)
│   ├── assets/             # Images, fonts, SCSS
│   ├── components/         # Reusable Vue components by domain
│   ├── composables/        # Composition API utilities
│   ├── constants/          # Enums and static constants
│   ├── data/               # Static JSON for landing pages
│   ├── directives/         # Custom Vue directives
│   ├── filters/            # Formatter functions (imported, not Vue 2 filters)
│   ├── lang/               # VeeValidate rules + English error messages
│   ├── pages/              # Routable page components
│   ├── resolvers/          # Route resolvers (pre-load data)
│   ├── router/             # Vue Router config
│   ├── security/           # Auth (oidc-client-ts), API service, roles, menu
│   ├── stores/             # Pinia stores (FLAT — no modules/ subfolder)
│   ├── types/              # TypeScript interfaces
│   ├── utils/              # Utility functions
│   ├── App.vue             # Root component (layout switch)
│   ├── main.ts             # App entry point
│   └── varaibles.ts        # App-wide global constants (note misspelled filename)
├── public/                 # Static assets (data/, fonts/, images/, robots.txt, sitemap.xml, version.json)
├── CLAUDE.md
├── package.json            # pnpm; scripts: dev, build, staging, production, type-check, lint
├── vite.config.ts          # envPrefix: 'VUE_APP_'
├── nginx.conf
└── Dockerfile              # Node 22 + pnpm build → nginx
```

---

## src/api/ — API Layer (24 files)

Plain TypeScript functions wrapping HTTP calls to Covenant.Api. All import the `api` wrapper object from `@/security/apiService` (`api.get/post/put/patch/del`), which unwraps `response.data`. Full endpoint tables in `SIGOOK_WEB_API_MAP.md`.

| File | Scope |
|------|-------|
| accountApi.ts | Email change, account deactivation |
| agencyApi.ts | Agency profile, personnel, locations, agency switching, assignable roles |
| agencyCandidateApi.ts | Candidate CRUD, phones, skills, docs, bulk upload, convert to worker |
| agencyCompanyApi.ts | Company (agency view): CRUD, locations, contacts, job positions, docs, settings, users |
| agencyInvoiceApi.ts | Invoice list/create/preview/delete/PDF/email |
| agencyNoteApi.ts | Notes on workers, candidates, companies, requests, request-workers |
| agencyPayStubApi.ts | PayStub CRUD, generation, subcontractor report, skip numbers |
| agencyReportApi.ts | T4, CRA, timesheet, hours, payment, payroll Excel |
| agencyRequestApi.ts | Request CRUD, workers, applicants, skills, shift, sources (job boards) |
| agencyRunnerApi.ts | Runners (recruiting pipeline per request): list, create, status, interviews |
| agencyTimeSheetApi.ts | TimeSheet CRUD per request/worker, usages |
| agencyWorkerApi.ts | Worker (agency view): list, flags (DNU, contractor), tax, email, holidays |
| catalogApi.ts | Enums: gender, ID type, availability, skills, industries, sources, tax categories |
| companyApi.ts | Company portal: profile, requests, workers, timesheet, users, contacts, invoices |
| downloadApi.ts | Invoice PDF, payroll Excel (various groupings) |
| locationApi.ts | Countries, provinces, cities, provincial settings, location tax |
| notificationApi.ts | Aggregated agency notification bell payload |
| requestApi.ts | Shift lookup only |
| salesApi.ts | Sales-scoped request/company lists + Excel export |
| sharedApi.ts | Email preferences unsubscribe |
| userNotificationApi.ts | In-app user notifications |
| websiteApi.ts | Public: job search, contact form, candidate apply |
| weeklyBoardApi.ts | Recruiting weekly board: assignments, runners |
| workerApi.ts | Worker portal: profile build, job apply, timesheet, wage/shift history |

---

## src/types/ — TypeScript Interfaces (11 files)

| File | Contains |
|------|----------|
| common.ts | `PaginatedList`, `Country`, `Province`, `City`, catalog item types, `CovenantFileModel`, `UserNotificationItem`, `UnsubscribeRequest`, `LocationTax` |
| agency.ts | `AgencyDetail`, `AgencyLocation*`, `AgencyPersonnel*`, `AgencyRequest*`, `AgencyWorker*`, `AgencyCompany*`, `Note*`, invoice notes/recipients models |
| accounting.ts | `PayStub*`, `AgencyInvoice*`, `CreateAgencyInvoiceModel`, `InvoiceSummaryModel`, `PayrollSubContractor*`, `SkipPayrollNumber*`, `AgencyReportFilter`, `WeeklyPayrollItem` |
| candidate.ts | `Candidate`, `CandidateDocument`, `AgencyCandidateFilter`, phone/skill models |
| company.ts | `CompanyProfile*`, `CompanyRequest*`, `TimeSheet*`, `ClockIn*`, `CompanyUser*`, `CompanyContactPerson*`, `CompanyInvoice*` |
| notification.ts | `NotificationsResponse`, `AppNotification`, `NotificationGroup`, `NotificationType` |
| runner.ts | `RunnerListItem`, `RunnerDetail`, `CreateRunnerModel`, `ChangeRunnerStatusModel`, interview models, `RunnerStartingToday` |
| security.ts | `ChangeEmailRequest`, `GetEmailResponse`, `UserProfile` |
| website.ts | `JobSearchFilter`, `JobViewModel`, `ContactForm` |
| weeklyBoard.ts | `WeeklyBoard`, `RecruiterWeeklyBoard`, assignment/runner payloads |
| worker.ts | `WorkerProfile`, worker profile section models, `WorkerRequest*`, `WorkerTimeSheet*`, wage/timesheet history |

---

## src/stores/ — Pinia (FLAT, 6 files)

Created in `src/stores/index.ts` with `pinia-plugin-persistedstate`. Stores hold filters + auth + small UI state only; API responses are never cached in stores.

| Store | File | State |
|-------|------|-------|
| `useAgencyStore` | agency.ts | `agency: AgencyDetail` (empty-shell default; `usaAgency`/`masterAgency` derived in `setAgency`), `personnelAgencies`, list filters: `agencyRequestFilter`, `agencyCandidateFilter`, `agencyWorkerProfileFilter`, `agencyCompanyProfileFilter`, `agencyInvoiceFilter`, `agencyPayStubFilter`, `agencyListFilter` |
| `useCompanyStore` | company.ts | `companyRequestFilter` |
| `useWorkerStore` | worker.ts | `workerProfile: Partial<WorkerProfile>` |
| `useSecurityStore` | security.ts | `user`, `userRoles`, `isReady`; actions: `setUser`, `getUser`, `signIn`, `silentSignin` |
| `useAppStore` | app.ts | `isMobile`, `currentDate` |

---

## src/router/ — Routes

| File | Prefixes | Notes |
|------|----------|-------|
| index.ts | `/callback`, `/silent-refresh`, `/unauthorized`, `/email-preferences`, 404 catch-all | Auth guard (requiresAuth + `meta.role` group), scroll behavior, canonical link, page titles |
| routesAgency.ts | `/recruiting/*`, `/sales/*`, `/accounting/*`, `/agency-profile` | Legacy `/agency-*` paths kept as redirects |
| routesCompany.ts | `/company-requests`, `/company-invoices`, `/company-profile`, `/company-user-profile` | |
| routesWorker.ts | `/register-worker`, `/worker-requests`, `/punch-card`, `/timesheet`, `/worker-history`, `/worker-profile`, `/worker-apply` | |
| routesLanding.ts | `/`, `/open-positions`, `/industries`, `/about`, `/employers`, `/talents`, `/special-projects`, `/partner`, `/apply`, `/privacy-policy`, `/terms-and-conditions`, `/disclaimer` | `meta: { layout: 'landing', requiresAuth: false }`; old URLs (`/home`, `/jobSeekers`, `/business`, `/about-us`, `/atas`, `/v2/*`, ...) redirect |

Agency route map (from `routesAgency.ts`):
- `/recruiting/requests[/create/:companyProfileId | /update/:companyProfileId/:requestId | /:id]`
- `/recruiting/weekly-board`, `/recruiting/attendance-review`
- `/recruiting/workers[/register | /:id]`, `/recruiting/candidates`
- `/recruiting/companies[/create | /update/:companyProfileId | /:id]`
- `/sales/requests[...]`, `/sales/companies[...]`, `/sales/agencies[/create | /:id]`
- `/accounting/invoices[/create]`, `/accounting/paystubs[/create]`, `/accounting/reports`

**Auth guard:** routes declare `meta: { requiresAuth: true, role: [...] }` with role groups from `src/security/roles.ts`; guard redirects to `/unauthorized`.

**Route resolvers** (`src/resolvers/agencyResolvers.ts`): `loadAgencyCompaniesResolver`, `loadAgencyRequestToUpdateResolver`, `loadCompanyToUpdateResolver` — pre-fetch data before route entry.

---

## src/pages/ — Routable Views

### Agency (`src/pages/agency/`)

| Page | Purpose |
|------|---------|
| Requests.vue / Request.vue / AgencyCreateRequest.vue | Request list, detail (workers, applicants, runners, notes), create/edit |
| WeeklyBoard.vue | Recruiting weekly board (admin + recruiter views) |
| AttendanceReview.vue | Workers starting recently — attendance follow-up |
| Workers.vue / DetailWorker.vue | Worker roster and detail (flags, holidays, history, notes) |
| Companies.vue / CreateCompany.vue / DetailCompany.vue | Client companies list, create/edit, detail |
| Candidates.vue | Candidate pool; convert to worker, bulk import |
| Agencies.vue / CreateAgency.vue / DetailAgency.vue | Sub-agencies (sales) |
| AgencyProfile.vue | Own agency profile, locations, personnel |
| accounting/Invoices.vue / accounting/CreateInvoice.vue | Invoice list and creation (preview → generate) |
| accounting/PayStubs.vue / accounting/CreatePayStub.vue | Pay stub list and manual creation |
| accounting/Reports.vue | T4, CRA, hours worked, payment, payroll export |

### Company (`src/pages/company/`)

Requests.vue, Request.vue, CreateRequest.vue, CompanyReports.vue (invoices), CompanyProfile.vue, CompanyUserProfile.vue

### Worker (`src/pages/worker/`)

Register.vue, Requests.vue, Request.vue, RequestApplied.vue, PunchCard.vue, TimeSheet.vue, History.vue, WorkerProfile.vue, WorkerApply.vue

### Landing (`src/pages/landing/` — subfolder per section)

```
landing/
├── About/AboutUs.vue
├── Apply/Apply.vue
├── Employers/Employers.vue
├── Home/Home.vue
├── Industries/Industries.vue
├── Legal/TermsAndConditions.vue, PrivacyPolicy.vue, Disclaimer.vue
├── OpenPositions/OpenPositions.vue
├── Partner/Partner.vue
├── SpecialProjects/SpecialProjects.vue
├── Talents/Talents.vue
└── ComingSoon.vue
```

### Shared (`src/pages/`)

Callback.vue (OAuth callback), SilentRefresh.vue (hidden iframe token renew), Unauthorized.vue, NotFound.vue, EmailPreferences.vue (unsubscribe, no auth)

---

## src/components/ — Reusable Components

Domain folders + shared root-level components. Components take function refs (e.g., an API function as a prop) rather than store dispatch strings.

| Folder | Contents |
|--------|----------|
| (root) | Address, CollapseSection, Comments, CompanyCreateUserModal, CropImage, DataEntryTerms, DefaultImage, DialogWorkerComment, EmailCard, Export, FormSkillAdd, Paginator, PhoneInput, PreviewImage, ProvinceSettingsModal, Searcher, SidebarLogged, UploadFiles, UserNotification |
| agency/ | Personnel modal/list, AgencyRequests, AgencyWorkers(+List), worker request history, BulkData, ContainerRequest, DialogContactWorker, ModalTimesheet, PayrollSubcontractor, agency profile sections (ProfileAccountInformation/Billing/Business/Contact) |
| agency_accounting/ | CRAPayroll, DeleteInvoice, GeneratePayStubs, HoursWorkedReport, PaymentReport, PreviewInvoice, SendInvoiceEmail, SkipPayrollNumber, SubcontractorsReport, T4 |
| agency_company/ | CompanyDetailTab, CompanyNotes, CompanyRequests, CompanySettings, CompanyUpdateLogo, CompanyWorkers, contact info/person forms + lists, Documents(+Form), EditVaccinationRequired, JobPositionForm/List, LocationDetail/Form, RequestJobPositionForm, RolesShiftDetail, UserList |
| agency_request/ | AgencyRequestDetail, AgencyRequestSkills, timesheet detail/modal, AgencyShiftDetail, Applicants, ManageApplicantsModal, ContactListModal, DatepickerModal, EditTextarea, JobBoardsModal, MassivePunchCard, punch-card container, ReportTo, RequestedBy, RequestNotes(+Table), Runners, TableRequests, WorkerStatusFilter |
| calendar/ | CalendarPunchCard |
| candidate/ | CreateCandidate, DetailAddress, DetailCandidate, ModalCandidateRequests, ModalDocuments |
| company/ | CompanyCancelList, CompanyInvoices, CompanyUsers(+Update), DialogCompanyUpdateEmail, DialogReplaceWorker, DialogRequestWorker, ProfileBusiness/Contact/Location |
| company_request/ | CompanyRequestDetail, punch card components, timesheet detail/modal, CompanyRequestWorkers |
| landing/ | Section components per page (About/, Employers/, Home/, Industries/, OpenPositions/, Partner/, SpecialProjects/, Talents/) + `shared/` (cards, forms incl. CandidateApplyForm/Modal + WorkerRegisterForm, hero, icons, layout Header/Footer/GlobalBackground/AppVersionToast, sections, ui) |
| notes/ | ColorPicker, ModalNotes, NoteForm, NotesPopover |
| notifications/ | NotificationBell (agency sidebar bell, uses `useNotifications`) |
| request/ | ButtonSort, RequestDetail, RequestLocation, ShiftDetail, ShiftEditModal, ShiftsForm |
| runner/ | CreateRunner, RunnerActionsDropdown + RunnerActionModals (shared runner menu, used by the Runners tab and the weekly board), RunnerHistoryModal, RunnerInterviewModal, RunnerStatusModal |
| weekly_board/ | AdminWeeklyBoard, RecruiterWeeklyBoard, AssignRecruiterModal (adding runners reuses `runner/CreateRunner.vue`) |
| worker/ | Profile section Detail/Form pairs (basic info, contact, emergency, availability, days, times, languages, licenses, lifts, skills, SIN, resume, certificates, documents, other docs, experience, image, email, location preferences), Notes, ProfileComments, ProfileExperience, ProfilePersonal, ProfilePreferences, RequestDetail, TimeSheetHistory, WorkerAccountSecurity, WorkerSettings, WorkWageHistory |

---

## src/security/ — Auth & API Config

### apiService.ts

Axios instance (`http`, default export) with `baseURL = import.meta.env.VUE_APP_URL_API`, qs param serializer, and interceptors:
- **Request:** `Authorization: {token_type} {access_token}` from `useSecurityStore.getUser()`; `accept-language` from localStorage
- **Response:** 401 → one retry after `securityStore.silentSignin()`, else `signIn()`; 403 → alert; 500 → alert (special rejection for blob responses)

Exports the **`api` wrapper** (`get`, `post`, `put`, `patch`, `del`) that returns `response.data` directly — all `src/api/*.ts` files use this, not the raw axios instance.

### roles.ts

7 role strings mirroring backend `CovenantConstants.Role` (`superadmin`, `admin`, `recruiting`, `sales`, `company`, `companyUser`, `worker`) plus route-guard groups: `recruitingAccess`, `agencyStaff`, `salesAccess`, `adminAccess`. See `.docs/business/ROLES_PERMISSIONS.md`.

### securityService.ts

oidc-client-ts `UserManager` configured from `VUE_APP_SECURITY_SERVER` / `VUE_APP_CLIENT`; user loaded/unloaded/token-expired events wired to the security store in `main.ts`.

### menu.ts

Sidebar navigation per role group: `recruitingMenu`, `salesMenu`, `accountingMenu` — admin/superadmin get all three; recruiting and sales get only theirs.

---

## src/utils/ (15 files)

| File | Purpose |
|------|---------|
| buefyProgrammatic.ts | Registers buefy programmatic components (dialog/toast/etc.) on the app |
| buildWorkerFormData.ts | Builds multipart FormData for worker registration |
| compressFile.ts | File compression before upload |
| directHiring.ts | Direct-hire specific logic |
| distributeHours.ts | Timesheet hour distribution |
| downloadFile.ts | Blob download helper |
| fileUpload.ts | File upload helper |
| filters.ts | Formatter helpers |
| locationLabel.ts | Location display label formatting |
| phoneFormat.ts | Phone number formatting |
| recaptcha.ts | reCAPTCHA site key (`VUE_APP_RE_CAPTCHA_SITE_KEY`) |
| timeSheetApprove.ts | Timesheet approval workflow |
| toast.ts | Toast notification helper (replaces the old toastMixin) |
| validation.ts | Shared validation helpers |
| workerStatus.ts | Worker status helpers |

---

## src/composables/ (13 files)

| File | Purpose |
|------|---------|
| useAdmin.ts | `isAdmin` check (superadmin, admin) |
| useBodyScrollLock.ts | `lockScroll`/`unlockScroll` for modals |
| useCarousel.ts | Generic carousel state (landing) |
| useCreateWorker.ts | Worker registration flow |
| useFocusTrap.ts | Focus trap for modal accessibility |
| useJobs.ts | Public job search state (landing) |
| useModuleBase.ts | Resolves `/sales` vs `/recruiting` path prefix for shared pages |
| useNotifications.ts | Loads notification bell payload; maps typed lists → `AppNotification[]` grouped by type |
| usePubSub.ts | Pub/sub event system |
| useRecruitingAccess.ts | `hasRecruitingAccess` check (superadmin, admin, recruiting) |
| useRunnerActions.ts | Runner menu state (status/interview/history modals) + delete with confirm; shared by the Runners tab and the weekly board |
| useRevealOnScroll.ts | Reveal-on-scroll animation (landing) |
| useStickyForm.ts | Persists in-progress form state |

---

## src/filters/ (19 formatter functions, one per file)

Plain functions imported where needed (Vue 3 removed template filters):
- **Dates/time:** dateFilter, dateTimeFilter, dateFromNow, dateHHmm, dateHHmmss, dateMonth, timeFilter, hourMinutes, fixedHoursFilter
- **Money:** currencyFilter, currencyCadFilter
- **Text:** capitalizeFilter, breakWord, splitCapital, avatarLetters, emailName, fileNameFilter
- **Domain:** agencyTypeFilter, sinFilter

---

## src/directives/, src/constants/, src/lang/, src/data/

- **directives/**: `status-directive.ts` only (registered as `v-status` in main.ts) — status badge rendering.
- **constants/**: `enums.ts` (6 numeric enums used by agency/company pages, incl. `ClockType`), `catalog.ts` (`maximumHoursPerDay` from `VUE_APP_MAXIMUM_HOURS_DAY`, `residencyList`), `workerFeatures.ts` (worker status feature list).
- **lang/**: NOT i18n translations — `validator.ts` registers VeeValidate rules (built-in + custom phone via google-libphonenumber), `en_error.ts` holds English validation messages, `utils.ts` has helpers. The app is English-only; no vue-i18n.
- **data/**: `landing/` static JSON (historyMilestones.json, industries.json, teamMembers.json).

---

## src/assets/

```
assets/
├── fonts/open-sans/
├── images/
│   ├── default/         # Placeholders (error.svg, loading.svg for vue-lazyload)
│   └── landing/         # Landing imagery
└── scss/                # Global partials: base, buefy-overrides, calendar, candidiates,
    │                    # company, container-request, detail-worker, fonts, master, notes,
    │                    # profile, requests, tables, time-sheet, tokens, variables, weekly-board
    └── worker/          # Worker portal styles
```

---

## Top-Level Files

| File | Purpose |
|------|---------|
| App.vue | Layout switch: callback → bare; logged in → `SidebarLogged` + router-view; else web/landing layout (landing Header/Footer/GlobalBackground when `route.meta.layout === 'landing'`) |
| main.ts | Registers validation rules, app globals, `v-status` directive, global components (defaultImage, QuillEditor), router, pinia, oidc event wiring, Buefy (+programmatic), VueScrollTo, VueLazyload |
| varaibles.ts | (misspelled filename, kept) `appGlobals` object — request/worker status strings, sort keys, regexes, user type strings, agency types — registered on `app.config.globalProperties` via `registerAppGlobals` |

**Environment variables:** read as `import.meta.env.VUE_APP_*` — the legacy Vue-CLI prefix is preserved under Vite via `envPrefix: 'VUE_APP_'` in `vite.config.ts` (NOT `VITE_*`). Used: `VUE_APP_URL_API`, `VUE_APP_SECURITY_SERVER`, `VUE_APP_CLIENT`, `VUE_APP_RE_CAPTCHA_SITE_KEY`, `VUE_APP_MAXIMUM_HOURS_DAY`. Files: `.env.development.local`, `.env.staging`, `.env.production`.

---

## Global Plumbing

### Authentication

1. Login via IdentityServer (oidc-client-ts); `/callback` completes sign-in, `/silent-refresh` renews tokens in a hidden iframe.
2. On 401, `apiService` retries once after `silentSignin`; on failure redirects to login.
3. Role-based routing via `meta.role` groups; component-level checks via security store / `useRecruitingAccess` / `useAdmin`.

### Patterns

- **No API data caching in Pinia** — components fetch directly via `src/api` functions; stores keep list filters so pagination/search survive route changes.
- **Forms:** VeeValidate 4 + Yup schemas; toasts via `src/utils/toast.ts`.
- **Reusable components take function props** (API functions passed in) instead of dispatch strings.
- **Styling:** Bootstrap 5 CSS + `@ntohq/buefy-next` (Bulma-based) + global SCSS partials in `src/assets/scss/`.
- Naming conventions: see `Sigook.Web/CLAUDE.md`.

### Build & Deploy

- `pnpm run staging` / `pnpm run production` → vue-tsc type-check + Vite build (pipeline gate: 0 vue-tsc errors).
- pnpm hardening: `ignore-scripts=true` in `.npmrc` + `allowBuilds` allowlist in `pnpm-workspace.yaml`.
- Docker multi-stage (Node 22 + pnpm → nginx); nginx serves `index.html` for SPA history-mode routing.
