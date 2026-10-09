# Sigook.Web Codebase Structure

Vue 3 SPA with three logged-in portals (Agency, Company, Worker) plus a public landing site. Stack: Vite, TypeScript, Pinia, Vue Router 4, `buefy` 3.x (Bulma 1.x), VeeValidate 4 + Yup, oidc-client-ts.

The source tree mirrors the navigation — **portal → menu → feature** — and everything shared between portals lives in an explicit `shared/<concept>/` folder named after the concept it comes from.

---

## Project Root

```
Sigook.Web/
├── src/
│   ├── main.ts             # Entry point referenced by index.html
│   ├── app/                # SPA shell (router, security, layout, stores, system pages)
│   ├── shared/             # Code used by two or more portals, one folder per concept
│   ├── modules/            # One folder per portal: agency, company, worker, landing
│   ├── assets/             # Images, fonts, global SCSS partials
│   ├── lang/               # validator.ts — VeeValidate rules + English messages (no i18n)
│   └── vite-env.d.ts       # typed VUE_APP_* env vars (vue-tsc provides the .vue module and macro typings)
├── public/                 # Static assets (images/v2/, favicon.ico, robots.txt, sitemap.xml, version.json)
├── index.html
├── CLAUDE.md
├── package.json            # pnpm; scripts: dev, build, staging, production, type-check, lint, preview, format
├── tsconfig.json           # paths: @/* → src/*
├── eslint.config.mjs       # includes the module-boundary rules (see below)
├── vite.config.ts          # envPrefix: 'VUE_APP_', alias @ → src
├── nginx.conf
└── Dockerfile              # nginx serving the prebuilt wwwroot/ (built by the pipeline)
```

---

## src/app/ — SPA shell

| Path | Contents |
|------|----------|
| `App.vue` | Layout switch: callback → bare; logged in → `layout/SidebarLogged` + router-view; else landing layout (`modules/landing/layout/*`) |
| `globals.ts` | `appGlobals` constants (worker-facing request/worker status strings, address regex, agency types); imported explicitly where needed, nothing on `globalProperties` |
| `router/index.ts` | Creates the router from `modules/{agency,company,worker,landing}/routes.ts`; auth guard (`requiresAuth` + `meta.role` groups → `/login?returnUrl=` or `/unauthorized`), scroll behavior, canonical link, page titles; owns `/login`, `/forgot-password`, `/confirm-email`, `/create-password`, `/callback`, `/silent-refresh`, `/unauthorized`, `/email-preferences` and the 404 |
| `security/` | `apiService.ts` (axios instance + the `api` wrapper every `api.ts` uses), `securityService.ts` (oidc-client-ts, password grant, Microsoft 365), `roles.ts` (7 roles + route groups), `menu.ts` (`getMenu(userRoles, agency)` builds the sidebar), `authErrors.ts`, `authApi.ts` (token/userinfo/password endpoints) |
| `stores/` | `index.ts` (pinia + persistedstate), `app.ts` (`isMobile`, `currentDate`), `security.ts` (`user`, `userRoles`, sign-in/out). Portal stores live in `modules/<portal>/store.ts` |
| `layout/` | `SidebarLogged.vue` (sidebar + mobile topbar, user menu, staff clock-in button, notifications bell), `UserNotification.vue` |
| `pages/` | `Callback`, `SilentRefresh`, `Unauthorized`, `NotFound`, `EmailPreferences`, `auth/{Login,ForgotPassword,ConfirmEmail,CreatePassword}` |

`app/` may import from `modules/*` (it composes them). Nothing else may.

---

## src/shared/ — cross-portal code

| Folder | Contents |
|--------|----------|
| `ui/` | SigookGrid (the only `b-table` wrapper), PageHeader, Breadcrumbs (+ `breadcrumbs.ts`), PhoneInput, Address, ProvinceSettingsModal, PreviewImage, CropImage, DefaultImage (globally registered in `main.ts`), SearchSelect, Export, FormSkillAdd, EditTextarea, EyebrowPill, MobileFiltersPanel, SheetPanel |
| `detail-page/` | DetailHeader, DetailLayout, DetailCard, DetailFields + `types.ts` — frame of the request and company detail pages |
| `request-detail/` | RequestHeader, RequestDetailTab, RequestStaffingCard, RequestLocationCard, ShiftDetail, AgencyShiftDetail, CompanyCancelList, `useRequestStatus`, `requestDetail.ts` (KPI builders, maps helpers), `types.ts` (request detail, `RequestComplianceItem`, `CreateAgencyRequestModel`, `RequestShiftModel`), `requestApplicant.ts` |
| `worker-profile/` | `cards/` (WorkerProfileHeader, ProfileCard, PersonalCard, ContactCard, DocumentsCard, PreferencesCard, SkillsCard, ExperienceCard, CommentsCard, ProfileIndex, NeedsAttention), `forms/` (the `Work*Form` modals, WorkImageDetail, WorkEmailForm, DialogWorkerComment), `useWorkerProfileStatus`, `api.ts` (profile section endpoints), `types.ts`. Used by the agency worker detail and the worker's own profile |
| `punch-card/` | CalendarPunchCard, DataEntryTerms, `distributeHours`, `timeSheetApprove`, `directHiring`, `types.ts` (TimeSheetModel, PunchCard*) |
| `company-profile/` | CompanyLocationsCard, CompanyMainContactCard, LocationForm, CompanyCreateUserModal, ProfileAccountInformation, `useCompanyDetail`, `types.ts` (CompanyProfileDetail, CompanyUserModel, …) |
| `api/` | `catalogApi`, `locationApi`, `accountApi`, `userNotificationApi`, `sharedApi`, `reportApi` (`downloadAgencyReport`, the generic Excel download used by SigookGrid) |
| `composables/` | useAdmin, useRecruitingAccess, useSalesAccess, useSuperAdmin, useBreakpoint, useBodyScrollLock, useFocusTrap, useGoBack, useStickyForm, usePubSub, useDropdownReveal, useElementSize, useSwipe |
| `utils/` | toast, downloadFile, compressFile, fileNaming, fileValidation, multipart, validation, phoneFormat, locationLabel, buefyProgrammatic |
| `format/` | Date/money/text formatters (one per file) re-exported by `index.ts`: `import { date, currency } from '@/shared/format'` |
| `constants/` | `enums.ts` (RequestStatus, ClockType, CompanyStatus, …), `catalog.ts` |
| `types/` | `common.ts` (PaginatedList, catalogs, CovenantFileModel, GridHandle, …), `security.ts`, `invoice.ts` (InvoiceSummaryModel shared by agency and company) |

Shared components never import from a module. When behaviour differs per portal they receive it as a prop: `CompanyCreateUserModal :save`, `LocationForm :save`, `CommentsCard :create-comment`, `PersonalCard :update-email`.

---

## src/modules/ — one folder per portal

Every portal has `routes.ts` (its `RouteRecordRaw[]`) and `store.ts` (its Pinia store with list filters). Each feature folder holds `pages/`, `components/`, `api.ts`, `types.ts` and its own composables/utils as plain files. A feature may import other features of the same portal; it may never import another portal (ESLint `no-restricted-imports`; `routes.ts` is the only exception).

### modules/agency/

| Folder | Routes | Contents |
|--------|--------|----------|
| `shared/` | — | Cross-menu pieces: `notes/` (ModalNotes, NotesPopover, NoteForm, ColorPicker, `api.ts`, `types.ts`), `notifications/` (bell payload: `api.ts`, `types.ts`, `useNotifications`), `useModuleBase` (`/sales` vs `/recruiting` prefix), `useSalesOwners`, AgencyRequests, AgencyWorkers(+List) |
| `profile/` | `/agency-profile` | AgencyProfile page; ProfileBusiness/Billing/Contact, AgencyPersonnel(+Modal), Attendance{ClockButton,EditModal,ReportModal}, `attendance.ts`; `api.ts` (profile, personnel, locations, attendance), `types.ts` (AgencyDetail, personnel, attendance) |
| `recruiting/requests/` | `/recruiting/requests[/create/:companyProfileId \| /update/… \| /duplicate/… \| /:id]` and the same under `/sales/requests` | Requests, Request, CreateRequest (create/update/duplicate share it and the lookup resolver); TableRequests, Applicants, ManageApplicantsModal, Runners, MassivePunchCard, punch-card container, ReportTo, RequestedBy, RequestNotes, Skills, Shift*, JobBoardsModal, …; `runners/` (CreateRunner, RunnerActions*, Runner*Modal, `useRunnerActions`, `api.ts`, `types.ts`); `api.ts`, `timeSheetApi.ts`, `types.ts` |
| `recruiting/applicants/` | `/recruiting/applicants` | Applicants board, ApplicantCompliance*, RequestComplianceModal, `useApplicantStatusActions`, `compliance.ts`, `types.ts` |
| `recruiting/weekly-board/` | `/recruiting/weekly-board` | WeeklyBoard (admin + recruiter views), AssignRecruiterModal, `api.ts`, `types.ts` |
| `recruiting/candidates/` | `/recruiting/candidates` | Candidates, CreateCandidate, DetailCandidate, documents modals, BulkData, `api.ts`, `types.ts` |
| `recruiting/workers/` | `/recruiting/workers[/register \| /:id]` | Workers, WorkerDetail; agency-only cards (Notes, TimeSheetHistory, WorkWageHistory, WorkerSettings), request history; `workerFeatures.ts`, `workerStatus.ts`, `api.ts`, `types.ts`. `/register` mounts `modules/worker/register` |
| `recruiting/clients/` | `/recruiting/companies[/create \| /update/:companyProfileId \| /:id]` and the same under `/sales/companies` | Clients, CreateClient, ClientDetail; company detail cards (CompanyDetailTab, Info/Documents/Invoicing/Settings/Notes cards, CompanyLocationsGrid), contacts, job positions, users, documents, logo; `api.ts`, `types.ts` |
| `sales/` | `/sales/requests` (list, no sidebar entry) | `api.ts` (sales-scoped request/company lists + Excel) |
| `sales/dashboard/` | `/sales/dashboard` | Dashboard page, DashboardCard/List, InteractionList, ClientList, DealList, BarChart, MeterList, RangeTabs, ClientForm/Modal, ClientInteractionsModal, `useCurrentAgent`, `format.ts`, `api.ts`, `types.ts` |
| `sales/clients/` | tabs of the client detail (`?tab=Interactions\|Deals`) | CompanyInteractions, CompanyDeals, InteractionForm/Modal, DealForm/Modal, `api.ts`, `types.ts` (enums + labels) |
| `sales/agencies/` | `/sales/agencies[/create \| /:id]` | Agencies, CreateAgency, AgencyDetail, `api.ts`, `types.ts` |
| `accounting/invoices/` | `/accounting/invoices[/create]` | Invoices, CreateInvoice, DeleteInvoice, PreviewInvoice, SendInvoiceEmail, `api.ts`, `types.ts` |
| `accounting/paystubs/` | `/accounting/paystubs[/create]` | PayStubs, CreatePayStub, GeneratePayStubs, SkipPayrollNumber, SubcontractorsReport, `api.ts`, `types.ts` |
| `accounting/reports/` | `/accounting/reports` | Reports, T4, CRAPayroll, HoursWorkedReport, PaymentReport, TimesheetsReport, `api.ts`, `types.ts` |

Sidebar (`app/security/menu.ts`): Recruiting {Requests, Weekly Board, Applicants, Candidates, Workers, Clients}, Sales {Dashboard, Clients, Agencies (master agency admins)}, Accounting {Invoices, Reports, Pay Stubs (non-USA)}. Admin/superadmin see all three; recruiting and sales only theirs. Default home: superadmin/admin → `/recruiting/requests`, recruiting → `/recruiting/weekly-board`, sales → `/sales/requests`. `/agency-*` legacy paths redirect.

### modules/company/

| Folder | Routes | Contents |
|--------|--------|----------|
| `requests/` | `/company-requests[/create \| /:id]` | Requests, Request (shared `request-detail/` header + Detail tab, Workers and Punch Card tabs), CreateRequest; CompanyRequestWorkers, punch card + timesheet components, DialogRequestWorker, `usePunchCardCalendar`; `api.ts`, `types.ts` |
| `invoices/` | `/company-invoices` | Invoices page, CompanyInvoices grid, `api.ts`, `types.ts` |
| `profile/` | `/company-profile`, `/company-user-profile` | CompanyProfile (tabs Profile, Locations, Contacts, Users, Account security, Notifications), CompanyUserProfile; CompanyProfileTab, CompanyUsers(+Update), ProfileBusiness/Contact/Location; `api.ts`, `types.ts` |

### modules/worker/

| Folder | Routes | Contents |
|--------|--------|----------|
| `register/` | `/register-worker[/:requestId]` (also mounted by the agency route `/recruiting/workers/register`) | Register page, `useCreateWorker`, `buildWorkerFormData`, `api.ts` |
| `requests/` | `/worker-requests[/applied/:id \| /:id]`, `/timesheet`, `/worker-apply` | Requests, Request, RequestApplied, TimeSheet, Apply; PunchCard component; `useWorkerRequestSummary`; `api.ts`, `types.ts` |
| `history/` | `/worker-history` | History page, `api.ts` |
| `profile/` | `/worker-profile` | WorkerProfile (same cards as the agency detail from `shared/worker-profile/`, read-only comments), WorkerAccountSecurity, ApprovalStatusCard, `api.ts` |

### modules/landing/

`routes.ts` (`/`, `/open-positions`, `/industries`, `/about`, `/employers`, `/talents`, `/special-projects`, `/partner`, `/apply`, legal pages, ComingSoon placeholders, legacy redirects; `meta.layout: 'landing'`), `pages/<Section>/`, `components/<Section>/` + `components/shared/{cards,forms,hero,icons,sections,ui}`, `layout/` (Header, Footer, GlobalBackground, AppVersionToast — mounted by `app/App.vue`), `data/` (static JSON), `composables/` (useJobs, useCarousel, useRevealOnScroll, useCandidateApplyModal), `api.ts`, `types.ts`.

---

## src/assets/

```
assets/
├── fonts/open-sans/
├── images/            # default/, landing/
└── scss/              # Global partials loaded by master.scss (variables, breakpoints, tokens, base, buefy-overrides, tables, calendar, requests, applicants, notes, weekly-board, worker-profile*, page-header, form-layout, auth, responsive-cards, …)
```

Component styles are `<style scoped>`; the few feature partials that are imported scoped (`worker-profile.scss`, `worker-profile-layout.scss`) still live here.

---

## Conventions

- **Imports are always `@/…`** — no relative paths, including SCSS `@import` inside `<style>` and asset `src=` / `url()`.
- **File names carry no module prefix** (`recruiting/clients/pages/Clients.vue`, not `AgencyCompanies.vue`); the folder gives the context.
- **One `api.ts` per feature**, plain functions over the `api` wrapper; never merge them per portal. Endpoints in `SIGOOK_WEB_API_MAP.md`.
- **`types.ts` per feature**; `shared/types/common.ts` only for transversal shapes.
- **No API data caching in Pinia** — stores keep list filters and auth only.
- **Grids:** every table is a `shared/ui/SigookGrid` (server mode with `:fetch` + `v-model:params`, client mode with `:data`; toolbar, Excel export through `downloadAgencyReport`, `#mobile-card` on touch). No page uses `b-table` directly.
- **Forms:** VeeValidate 4 + Yup; toasts via `shared/utils/toast`.
- **Reusable components take function props** (the API function is passed in) instead of importing a portal's API.
- **Environment:** `import.meta.env.VUE_APP_*` (`VUE_APP_URL_API`, `VUE_APP_SECURITY_SERVER`, `VUE_APP_CLIENT`, `VUE_APP_RE_CAPTCHA_SITE_KEY`, `VUE_APP_MAXIMUM_HOURS_DAY`).

### Authentication

1. Login happens in the SPA at `/login` (`app/pages/auth/Login.vue`): password grant against the API's token endpoint, or Microsoft 365 redirect completed at `/callback`. `/silent-refresh` renews tokens in a hidden iframe. `/forgot-password`, `/confirm-email`, `/create-password` are the account pages.
2. On 401, `app/security/apiService` retries once after `silentSignin`; on failure sends the browser to `/login?returnUrl=`.
3. Logout (`securityStore.signOut`) revokes the refresh token, clears the local user and routes to `/`.
4. Role-based routing via `meta.role` groups (`app/security/roles.ts`); component-level checks via the security store, `useRecruitingAccess`, `useSalesAccess`, `useAdmin`.

### Build & Deploy

- `pnpm run staging` / `pnpm run production` → vue-tsc type-check + Vite build (pipeline gate: 0 vue-tsc errors, 0 ESLint errors).
- pnpm hardening: `ignore-scripts=true` in `.npmrc` + `allowBuilds` allowlist in `pnpm-workspace.yaml`.
- Docker multi-stage (Node 22 + pnpm → nginx); nginx serves `index.html` for SPA history-mode routing.
