# Workflows - Covenant/Sigook Platform

The main system workflows, step by step, with the real entry points. Every route below is verified against its controller (controllers declare routes via `public const string RouteName = "api/..."` — grep for `RouteName =` to locate one). Format used throughout:

```
VERB route  →  Controller.Action  →  Service.Method
```

Controllers live under `Covenant.Api/Covenant.Api/`, services under `Covenant.Api/Covenant.Core.BL/Services/`.

---

## 1. Worker Registration Flow

Worker registers from the Flutter app → Agency reviews → Agency approves → Worker can apply to jobs.

### Step 1: Worker submits registration

```
POST api/WorkerProfile              (multipart/form-data, [AllowAnonymous])
→ WorkerProfileController.Post      (WorkerModule/WorkerProfile/Controllers/WorkerProfileController.cs)
→ WorkerService.CreateWorker(requestId?)
```

- The multipart form carries a serialized `WorkerProfileCreateModel` plus document files; `WorkerService.CreateWorker` reads it from `HttpContext.Request.Form` and validates it with `workerProfileValidator` before creating the profile.
- The optional `requestId` query parameter links the registration to a specific job order (registration-via-invitation).
- The profile is created with `ApprovedToWork = false`.

The agency can also register a worker on the worker's behalf:

```
POST api/agency/workers        (multipart/form-data)
→ WorkersController.CreateWorkerProfile → WorkerService.CreateWorker
```

### Step 2: Agency reviews and approves

```
GET api/agency/workers                     → WorkersController.Get (paginated list, GetWorkerProfileFilter)
GET api/agency/workers/{id}                → WorkersController.GetById
PUT api/agency/workers/{id}/ApprovedToWork → WorkersController.UpdateApprovedToWork
```

Approval calls the domain method `WorkerProfile.UpdateApprovedToWork(now)`, which enforces that required documents are complete. Related toggles on the same controller:

```
PUT api/agency/workers/{id}/Dnu             (Do Not Use — WorkerProfile.UpdateDnu)
PUT api/agency/workers/{id}/IsContractor
PUT api/agency/workers/{id}/IsSubcontractor (short-circuits ApprovedToWork)
PUT api/agency/workers/{id}/ExternalId
PUT api/agency/workers/{id}/WcCode
PUT api/agency/workers/{id}/tax-category
PUT api/agency/workers/{id}/tax-rate
```

### Step 3: Worker uses the app

Once approved, the worker sees their profile(s) and available jobs:

```
GET api/WorkerProfile/{profileId}   → WorkerProfileController.GetById
GET api/WorkerProfile/me            → WorkerProfileController.GetMyProfile
```

---

## 2. Job Order Creation & Matching Flow

### Step 1: Request is created

By the agency:

```
POST api/agency/requests            → RequestsController.Post (Controllers/Sigook/Agency/Requests/RequestsController.cs)
→ RequestService.CreateRequest(RequestCreateModel)
```

By the company:

```
POST api/CompanyRequest             → CompanyRequestController.Post (CompanyModule/CompanyRequest/Controllers/CompanyRequestController.cs)
→ RequestService (company-side create)
```

Key facts about the `Request` entity (`Covenant.Common/Entities/Request/Request.cs`):

- Rates (`AgencyRate`, `WorkerRate`) come from the selected `CompanyProfileJobPositionRate`.
- A new request starts as `RequestStatus.Open`. The enum has exactly three values: `Open = 1`, `Filled = 3`, `Cancelled = 4` (value 2 intentionally skipped).
- `WorkerSalary` (`Request.cs:53`) set = **Direct Hiring** order (permanent placement); changes billing and attendance behavior (see §6).
- State transitions (fill on capacity, reopen, cancel restrictions) are automatic inside the entity — **do not re-implement them**; the authoritative description is `.docs/business/REQUEST_STATE_MANAGEMENT.md`.

Request management endpoints on the same controller:

```
PUT api/agency/requests/{id}                              → RequestService.UpdateRequest
PUT api/agency/requests/{id}/Cancel                       → RequestService.CancelRequest   (only Open + zero workers)
PUT api/agency/requests/{id}/Open                         → RequestService.OpenRequest     (reopen)
PUT api/agency/requests/{id}/IncreaseWorkersQuantityByOne → AgencyService.IncreaseWorkersQuantityByOne
PUT api/agency/requests/{id}/ReduceWorkersQuantityByOne   → RequestService.ReduceWorkerQuantityByOne
POST api/agency/requests/{id}/SendInvitation              → RequestService.SendInvitation  (queues a Service Bus job inviting matching workers)
```

Role-scoped listings: `GET api/agency/recruiting/requests` (recruiting) and `GET api/agency/sales/requests` (sales rep sees only their own companies' orders).

### Step 2A: Worker browses and applies (reactive)

```
GET  api/WorkerRequest                      → WorkerRequestController.Get (WorkerModule/WorkerRequest/Controllers/WorkerRequestController.cs)
                                              → IRequestRepository.GetRequestsForWorker
GET  api/WorkerRequest/{id}                 → WorkerRequestController.GetById
POST api/WorkerRequest/{requestId}/Apply    → WorkerRequestController.Apply → WorkerService.Apply(requestId, model)
```

There is no `/Available` suffix — the plain `GET api/WorkerRequest` already returns only requests the authenticated worker can apply to. An anonymous variant `POST api/WorkerRequest/{workerId}/{requestId}/Apply` exists for invitation links.

### Step 2B: Agency tracks applicants

Applicants (workers or candidates who applied / were sourced) are managed per request:

```
GET    api/agency/requests/{requestId}/Applicants          → ApplicantsController.Get
POST   api/agency/requests/{requestId}/Applicants          → ApplicantsController.Post   (candidate or worker; duplicate-guarded)
GET    api/agency/requests/{requestId}/Applicants/Search   → IRequestRepository.SearchApplicants
DELETE api/agency/requests/{requestId}/Applicants/{id}
```

---

## 3. Worker Assignment (Booking) Flow

### Booking

```
POST api/agency/requests/{requestId}/Workers/{workerProfileId}/Book
→ WorkersController.Post (Controllers/Sigook/Agency/Requests/WorkersController.cs)
→ AgencyService.BookWorker(requestId, workerProfileId, AgencyBookWorkerModel)
```

`BookWorker` validates the worker profile (approved, not DNU) and calls `Request.AddWorker`, which creates the `WorkerRequest` with `WorkerRequestStatus.Booked` and auto-transitions the request to `Filled` when capacity is reached (see `REQUEST_STATE_MANAGEMENT.md`).

`WorkerRequestStatus` (`Covenant.Common/Enums/WorkerRequestStatus.cs`) has two values: `Rejected = 2` and `Booked`.

### Managing booked workers

```
GET api/agency/requests/{requestId}/Workers                        → workers assigned to the request
GET api/agency/requests/{requestId}/Workers/{id}                   → single worker request
PUT api/agency/requests/{requestId}/Workers/{id}                   → WorkerRequest.UpdateStartWorking (change start date)
PUT api/agency/requests/{requestId}/Workers/{workerProfileId}/Reject → RequestService.RejectWorker (may reopen a Filled request)
```

The company sees its assigned workers via `api/CompanyRequest/{requestId}/Worker` (`CompanyModule/CompanyRequestWorker/Controllers/CompanyRequestWorkerController.cs`).

---

## 4. Time Tracking Flow (Punch Card)

### Step 1: Worker clocks in / out

A single punch endpoint; the service decides whether the punch is a clock-in or a clock-out:

```
POST api/WorkerRequest/{requestId}/TimeSheet
→ WorkerRequestTimeSheetController.Post (WorkerModule/WorkerRequestTimeSheet/Controllers/WorkerRequestTimeSheetController.cs)
→ TimesheetService.Register(requestId, WorkerLocationModel)     (payload = GPS location of the punch)

GET api/WorkerRequest/{requestId}/TimeSheet/clock-type/{latitude}/{longitude}
→ TimesheetService.GetClockType     (next expected punch: clock-in or clock-out)
  The day boundary is evaluated in the job site's time zone (Request.JobLocation), falling back
  to the caller's coordinates when the job has no location. Never in server time.

GET api/WorkerRequest/{requestId}/TimeSheet
→ ITimesheetRepository.GetTimeSheetsForWorker   (worker's own timesheet list)
```

### Step 2: Agency reviews and edits the punch card

```
GET    api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets
→ WorkerTimeSheetsController.Get (Controllers/Sigook/Agency/Requests/WorkerTimeSheetsController.cs)

POST   api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets        → TimesheetService.CreateTimesheet   (manual entry, e.g. attendance "0")
PUT    api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}   → TimesheetService.UpdateTimesheet
DELETE api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}   → TimesheetService.RemoveTimeSheet
GET    api/agency/requests/{requestId}/Workers/{workerId}/TimeSheets/{id}/Usages  → where the timesheet is used (pay stub/invoice)
```

A request-wide view exists at `GET api/agency/requests/{requestId}/TimeSheets` (`TimeSheetsController.cs`).

### Step 3: Hours breakdown

Regular / overtime / holiday hour classification is computed by `TimesheetCalculatorService` (`Covenant.Core.BL/Services/Accounting/Shared/TimesheetCalculatorService.cs`) when pay stubs and invoices are generated. Rules (44-hour OT threshold, holiday handling, `HolidayIsPaid`) are documented in `.docs/business/TIMESHEET_RULES.md`. Night shift is deprecated — it is never computed anywhere; do not add night-shift logic.

---

## 5. Payroll & Invoicing Flow

Both run from the agency accounting screens. Calculation detail is NOT duplicated here — see `.docs/business/PAYROLL_RULES.md` and `.docs/business/BILLING_RULES.md`.

### 5.1 Pay stub generation

```
GET  api/agency/accounting/PayStubs/WorkersReadyForPayStub
→ PayStubsController (Controllers/Sigook/Agency/Accounting/PayStubsController.cs)
→ ITimesheetRepository.GetWorkersReadyForPayStub   (workers with unpaid approved timesheets)

POST api/agency/accounting/PayStubs/generate        (body: worker profile ids)
→ PayStubsController.GeneratePayStubs → PayStubService.Generate(agencyIds, workerIds)
```

`PayStubService.Generate` iterates workers and calls `GeneratePayStubForWorker`, which aggregates the worker's pending timesheets and computes deductions via `TimesheetCalculatorService.CalculateDeductions(totalEarnings, numberOfWeeks, year, workerProfileId)`.

Deductions are **database table lookups** (CPP, Federal and Provincial tax ranges via `DeductionsRepository`, by earnings and year); EI is the only computed value (`totalEarnings × rates.EmploymentInsurance`). There are no calculator classes. Subcontractor tax-category overrides zero out deductions. Deduction tables are maintained through `api/Accounting/Deduction` (`DeductionsController`): both the CPP and the income tax tables are imported from the CRA PDFs dropped in the `cra-tables` blob container (blob trigger → `POST .../Cpp/Blob`, `POST .../Tax/Blob`), and one income tax PDF carries the federal and the provincial tables. See [PAYROLL_RULES](PAYROLL_RULES.md#tax-table-maintenance).

Delivery and management:

```
GET    api/agency/accounting/PayStubs                    → PayStubService.GetPayStubs (filtered list)
GET    api/agency/accounting/PayStubs/file               → filtered list as file export
GET    api/agency/accounting/PayStubs/{payStubId}/pdf    → PayStubService.GetPayStubPdf
POST   api/agency/accounting/PayStubs/{payStubId}/email  → PayStubService.SendPayStubEmail
POST   api/agency/accounting/PayStubs/email/bulk         → queues BulkPayStubEmailJob on Azure Service Bus (ISigookBusClient)
DELETE api/agency/accounting/PayStubs/{id}               → PayStubService.DeletePayStub
POST   api/agency/accounting/PayStubs                    → PayStubService.CreateManualPayStub (obsolete)
```

The controller also exposes skip-payroll-number endpoints, and accounting reports live under
`api/agency/accounting/Reports/*` (`ReportsController`: t4, cra-payroll, payments, subcontractors,
hours-worked, timesheets).

### 5.2 Invoicing

```
POST api/agency/accounting/Invoices/Preview   → IInvoiceService.PreviewInvoice   (no persistence)
POST api/agency/accounting/Invoices           → IInvoiceService.CreateInvoice
→ InvoicesController (Controllers/Sigook/Agency/Accounting/InvoicesController.cs)
```

The controller resolves the service through `InvoiceServiceFactory` (`Covenant.Core.BL/Services/Accounting/Invoices/`), which picks `CanadaInvoiceService` or `UsaInvoiceService`.

Key facts (full rules in `BILLING_RULES.md`):

- Invoice subtotal = timesheets (at `AgencyRate`) + additional items + holidays − discounts.
- Vacations and bonus are **not** billed — `VacationsRate`/`BonusRate` exist on the entity but never enter the totals (vacation 4% is a pay-stub concept).
- HST is a single global configuration rate, not per-province.
- Worked holidays are always billed at holiday rate on invoices (invoices hardcode `holidayIsPaid: true`), while pay stubs honor the timesheet's `HolidayIsPaid` flag.

Delivery and management:

```
GET    api/agency/accounting/Invoices                       → IInvoiceService.GetInvoices (list with totals)
GET    api/agency/accounting/Invoices/{invoiceId}/pdf       → IInvoiceService.GetInvoicePdf
POST   api/agency/accounting/Invoices/{invoiceId}/email     → IInvoiceService.SendInvoiceEmail (multipart, optional attachments)
GET    api/agency/accounting/Invoices/{invoiceId}/paystubs  → pay stubs linked to the invoice (delete warnings)
DELETE api/agency/accounting/Invoices/{id}                  → IInvoiceService.DeleteInvoice (cascades selected pay stubs)
```

---

## 6. Runner Pipeline Flow

A **Runner** is a Worker actively submitted to a specific order (Request). Candidates cannot be runners — they must be converted to a worker first (`CandidateService.ConvertToWorker`).

**`Request.UsesRunners`** (set when the order is created, default `true`) decides whether the order works with runners. When it is `false` the Runners tab is still visible but **read-only**, and the weekly board hides its "Add runner" button; `RunnerService` rejects create/status/interview on such an order. The recruiter creates the runner, advances it through the recruiting pipeline and schedules interviews, until the client hires or rejects it. Runners live in a tab inside the order detail (next to Applicants/Workers).

Orders **without** runners instead configure a **compliance checklist** — see [§6.1](#61-compliance-checklist-orders-without-runners).

Controller: `Controllers/Sigook/Agency/Requests/RunnersController.cs` (`[Authorize(Policy = Recruiting)]`) → `RunnerService`. Domain rules live in the `Runner` entity.

### Step 1: Recruiter adds a runner to the order

The recruiter searches a Worker and picks **Type** = `Active` (applied on their own) or `Passive` (sourced by a recruiter) — `RunnerType` enum. The search uses a dedicated runner-prospect endpoint that returns **only workers** and excludes those already runners on this request:

```
GET  api/agency/requests/{requestId}/Runners/Search?searchTerm=...   → IRequestRepository.SearchRunnerProspects
POST api/agency/requests/{requestId}/Runners                         → RunnerService.CreateRunner
     { "workerProfileId", "type": 1 }
```

The runner is created with `Status = SentToClient` and an initial status-history entry (`previousStatus = null`). The same worker cannot be added twice on the same request (rejected by the `RunnerExists` guard).

**Second entry point — the recruiting weekly board.** A recruiter can send a runner straight from their day card; the web reuses the same `CreateRunner.vue` modal:

```
POST   api/agency/recruiting/WeeklyBoard/runner   → WeeklyBoardService.AddRunner → IRunnerService.CreateRunner
DELETE api/agency/requests/{requestId}/Runners/{id} → RunnerService.DeleteRunner   (shared with the Runners tab)
```

The board resolves the recruiter from the token, finds their `RequestRecruiter` assignment for that work day and stamps it on `Runner.RequestRecruiterId`, so the card can count and list its runners. Runners created from the order's Runners tab leave that FK null. Sending a runner also appends the request note `"{workerName} was sent"`.

Each runner on a board card exposes the **same actions as the Runners tab** (change status, add interview, view history, delete) through the same modals, and shows its current status as a chip in front of the worker name. Deleting removes the runner with its status history and interviews.

### Step 2: Recruiter advances the status

```
PUT api/agency/requests/{requestId}/Runners/{id}/Status              → RunnerService.ChangeStatus
    { "status": 2, "comments": "Client requested an interview" }
```

- Any status can move to any other (no fixed order) **except** a `Hired` runner, which is terminal and rejects further changes.
- Each change appends a row to the status history (previous → new, who, when, comments) — never overwrites.
- Moving to `Hired` **requires** a `StartDate` (the date the runner would begin working); the transition is rejected without it.
- Every runner is a worker (`WorkerProfileId` is required at creation), so a hire always surfaces in attendance review (which is per worker).

**Pipeline states** (`Covenant.Common/Enums/RunnerStatus.cs`): `SentToClient(1)`, `InterviewScheduled(2)`, `InterviewRescheduled(3)`, `NoLongerAvailable(4)`, `NoShow(5)`, `WaitingForInterviewFeedback(6)`, `WaitingForFinalDecision(7)`, `Rejected(8)`, `InOnboardingProcess(9)`, `Hired(10)`.

### Step 3: Recruiter schedules interviews

Only allowed while the runner is in `InterviewScheduled` or `InterviewRescheduled`:

```
POST api/agency/requests/{requestId}/Runners/{id}/Interview          → RunnerService.AddInterview
     { "scheduledDate", "type": 1, "interviewer", "notes" }
```

A runner can have multiple interviews over time. Rescheduling an interview updates its date and **auto-transitions** the runner to `InterviewRescheduled`:

```
PUT api/agency/requests/{requestId}/Runners/{id}/Interview/{interviewId}/Reschedule   → RunnerService.RescheduleInterview
    { "newDate" }
```

### Step 4: View history

`GET api/agency/requests/{requestId}/Runners/{id}` returns the runner detail with the full status timeline (latest first) and the list of interviews. The "Add interview" and "Reschedule" actions are hidden/disabled outside the two interview-enabled states, and "Change status" is hidden once the runner is `Hired`.

All of these rules are enforced in the `Runner` domain entity (`CanAddInterview`, the Hired-terminal guard, append-only history), so the API is the source of truth; the UI only mirrors them.

### Step 5: Attendance-review notification (first days after hire)

Once a runner is `Hired` with a `StartDate`, the recruiter who hired them is reminded to confirm the worker showed up, during the worker's **first 3 days** (Day 1 = the `StartDate`).

```
GET api/agency/Notifications      → NotificationsController → NotificationService.GetNotifications
                                  → { workersToReview: [ { ...worker, dayNumber }, ... ] }
```

- **Per-user:** only the recruiter who performed the hire sees it. The hire stamps `Runner.UpdatedBy` with the acting user's id (`User.GetUserId()`); the query filters by it (`Hired` is terminal, so `UpdatedBy` = the hirer). No nickname/`StatusHistory` involved.
- **Scope:** excludes **Direct Hiring** orders (`Request.WorkerSalary` set).
- **3-day window:** the DB does a generous prefilter; `DayNumber = (today − StartDate).Days + 1` is computed in the service and is authoritative (kept only when `1..3`). This avoids a `timestamptz` timezone off-by-one between the window and the day count.
- **Aggregated, multi-type:** a single endpoint returns `NotificationsModel` (a container with one list per notification kind — today only `WorkersToReview`). The web bell shows a per-type summary + count; clicking opens the **Attendance Review** page (`/recruiting/attendance-review`), and each row links to that order's **Punch Card** tab where the recruiter enters `0` to mark attendance.
- **Punch card gating:** on the agency punch card, the per-day hours input is disabled and the edit icon hidden for users without admin access (superadmin/admin — `useAdmin` composable, used in `AgencyPunchCardWorkerContainer.vue` via `v-if="isAdmin"` / `:disabled="!isAdmin || ..."`); the attendance `0` is entered by whoever may edit.

### 6.1 Compliance checklist (orders without runners)

When `Request.UsesRunners == false`, the order instead carries a **compliance checklist**: the requirements a worker must meet to be placed on it. Each item is a free-text name plus a mandatory/optional flag plus a **document target** (which worker-profile section a document uploaded for it lands in), so the list is fully dynamic — the agency can add anything beyond the defaults.

The agency configures it from the **Configure** link next to the *Uses Runners* switch on the create/edit order page (`AgencyCreateRequest.vue` → `RequestComplianceModal.vue`). The link is hidden while the order uses runners; the items are kept, not deleted, if the switch is turned back on.

**Default list**, pre-filled the first time the switch is turned off (`Sigook.Web/src/constants/compliance.ts`; frontend-only — the API persists exactly what it receives and never seeds):

| Requirement | Default | Document target |
|---|---|---|
| ID | Mandatory | Identification 1 |
| SSN | Mandatory | Social insurance |
| WP | Optional | Other documents |
| W4 | Mandatory | Other documents |
| I9 | Mandatory | Other documents |
| Banking | Mandatory | Other documents |

Storage is a child table `RequestComplianceItems` (`RequestComplianceItem` entity, cascade-deleted with the order), not a JSON column. There is no dedicated endpoint: the items ride inside `RequestCreateModel.ComplianceItems` on create and update, and come back on `AgencyRequestDetailModel.ComplianceItems`.

```
POST api/agency/requests        → RequestService.CreateRequest  (creates the items)
PUT  api/agency/requests/{id}   → RequestService.UpdateRequest  (reconciles the items)
```

On update the list is **reconciled by id, never wiped and recreated**: items sent with their `id` are updated in place, items sent without one are created, and items missing from the payload are deleted (together with any per-applicant completions referencing them). Item ids therefore stay stable across edits — per-applicant completions reference them. Validation (`RequestCreateModelValidator`): name required, max 200 chars, max 50 items, names unique case-insensitively, document target a valid enum value.

Per-applicant fulfilment is tracked through the applicant lifecycle — see [§6.2](#62-applicant-states--per-applicant-compliance).

### 6.2 Applicant states & per-applicant compliance

Each `RequestApplicant` carries a **status** (`RequestApplicantStatus`, stored as string with DB default `Pending`) and a set of **completions** (`RequestApplicantComplianceItems`: one row per checklist item fulfilled by that applicant, with `CompletedAt`/`CompletedBy`; unique per applicant+item, cascade-deleted with the applicant). Detailed state rules live in `REQUEST_STATE_MANAGEMENT.md` → *Request Applicant States*.

Initial status by origin:

| Origin | Status |
|---|---|
| Worker applies (web/app/anonymous apply) | Pending |
| Website form via bus (`NewCandidateConsumer`) | Pending |
| Manual add from the portal | In progress |
| CSV bulk import (`CandidateAdapter`) | In progress |

Everything runs through `RequestApplicantService` (the `ApplicantsController` only delegates):

```
PUT    api/agency/requests/{requestId}/Applicants/{id}/Status                    → change status (Start / Cancel / Reopen / Confirm)
GET    api/agency/requests/{requestId}/Applicants/{id}/ComplianceItems           → checklist + per-applicant completion state
POST   api/agency/requests/{requestId}/Applicants/{id}/ComplianceItems/{itemId}  → complete an item (multipart; optional document)
DELETE api/agency/requests/{requestId}/Applicants/{id}/ComplianceItems/{itemId}  → uncheck an item (document stays on the profile)
```

In the portal the **Compliance** action on an applicant row opens the modal (`ApplicantComplianceModal.vue`) which is the single hub: checks, uploads and the status buttons (Start / Cancel applicant / Reopen / Confirm) all live there. The applicants list shows a status tag column and a multi-status filter.

**The checkbox is the submit.** Attaching a file and typing the number/type only fills a local draft — no request is sent, and the file can be attached with the inputs still empty (or replaced/removed before saving). The `POST` fires when the agent ticks the checkbox, carrying the file plus whatever the draft holds; unticking fires the `DELETE`, and the checkbox reverts if the call fails. A **mandatory** item keeps its checkbox disabled until its draft satisfies the service rules (document, plus number/type for identification or the SIN number), with a tooltip naming what is missing; optional items are always tickable.

Completing an item can attach a document that lands in the worker-profile section named by the item's `DocumentTarget` (WP/W4/I9/Banking-style items land in *Other Documents* with the item name as description). For **worker** applicants, a **mandatory** item with a document target can **only** be completed by uploading the document, and mandatory identification/social-insurance items additionally require their number (and type) — enforced in the service and mirrored in the modal before it posts. **Optional items require nothing**: neither the document nor the inputs, and an optional identification saved without a number keeps the number/type already on the profile. Numbers are duplicate-checked whenever one is present. Candidates can check items but never upload — convert to worker first. **Confirm** requires all mandatory items completed (optional ones don't block) and a worker applicant.

**SIN/SSN-typed identifications** (identification type with `IdentificationTypeCode.SinSsn`): completing an Identification item with that type validates the number as a SIN (9-15 chars, not another profile's SIN, not different from the profile's existing SIN) and **also fills the worker's `SocialInsurance` + SIN document from the same upload** (one blob, no double attachment), then **auto-completes** any pending checklist item with target *Social insurance* for that applicant. The same auto-fill applies at worker registration (web and app) when a SIN/SSN-typed identification is provided — SIN is never captured directly at registration. SIN duplicate validation runs in every flow that sets it: registration, the worker documents/SIN forms, and both compliance branches.

Note: `BookWorker` still deletes the applicant row; its completions go with it by cascade.
