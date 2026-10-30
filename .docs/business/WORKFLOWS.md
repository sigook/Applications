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
POST api/worker/profile              (multipart/form-data, [AllowAnonymous])
→ ProfileController.Post      (Controllers/Workers/Profile/ProfileController.cs)
→ WorkerService.CreateWorker(requestId?)
```

- The multipart form carries a serialized `WorkerProfileCreateModel` plus document files; `WorkerService.CreateWorker` reads it from `HttpContext.Request.Form` and validates it with `workerProfileValidator` before creating the profile.
- The optional `requestId` query parameter links the registration to a specific job order (registration-via-invitation).
- The profile is created with `ApprovedToWork = false`.

The agency can also register a worker on the worker's behalf:

```
POST api/agency/recruiting/workers        (multipart/form-data)
→ WorkersController.CreateWorkerProfile → WorkerService.CreateWorker
```

### Step 2: Agency reviews and approves

```
GET api/agency/recruiting/workers                     → WorkersController.Get (paginated list, GetWorkerProfileFilter)
GET api/agency/recruiting/workers/{id}                → WorkersController.GetById
PUT api/agency/recruiting/workers/{id}/ApprovedToWork → WorkersController.UpdateApprovedToWork
```

Approval calls `WorkerProfile.UpdateApprovedToWork(missingRequirements)` with the list computed by `IWorkerRequirementService.GetMissing(profile)`: profile photo, tax category (Canada only), SIN number + document (not expired), and at least one identification (number + file on the same slot). The resume is not required. Every missing item comes back as an error, so the agency sees the full list at once. Subcontractors and contractors skip the list. Booking (`WorkerProfile.CanBeBook`) only re-checks the SIN items (missing or expired) on top of `ApprovedToWork`, so workers approved before a requirement was added can still be booked. Turning the contractor or subcontractor flag **off** requires the full document list (`GetMissingDocuments`). Related toggles on the same controller:

```
PUT api/agency/recruiting/workers/{id}/Dnu             (Do Not Use — WorkerProfile.UpdateDnu)
PUT api/agency/recruiting/workers/{id}/IsContractor
PUT api/agency/recruiting/workers/{id}/IsSubcontractor (short-circuits ApprovedToWork)
PUT api/agency/recruiting/workers/{id}/ExternalId
PUT api/agency/recruiting/workers/{id}/WcCode
PUT api/agency/recruiting/workers/{id}/tax-category
PUT api/agency/recruiting/workers/{id}/tax-rate
```

### Step 3: Worker uses the app

Once approved, the worker sees their profile(s) and available jobs:

```
GET api/worker/profile/{profileId}   → ProfileController.GetById
GET api/worker/profile/me            → ProfileController.GetMyProfile → WorkerService.GetProfileDetail
```

`GetProfileDetail` (also behind `GET api/agency/recruiting/workers/{id}`) fills `missingRequirements` with the codes from `IWorkerRequirementService`. Sigook.Web (jobs banner, profile "needs attention" list) and Sigook.App (jobs banner) render that list; neither client recomputes the rule.

---

## 2. Job Order Creation & Matching Flow

### Step 1: Request is created

By the agency:

```
POST api/agency/recruiting/requests            → RequestsController.Post (Controllers/Agencies/Recruiting/Requests/RequestsController.cs)
→ RequestService.CreateRequest(RequestCreateModel)
```

By the company:

```
POST api/company/requests             → RequestsController.Post (Controllers/Companies/Requests/RequestsController.cs)
→ RequestService (company-side create)
```

Key facts about the `Request` entity (`Covenant.Common/Entities/Requests/Request.cs`):

- Rates (`AgencyRate`, `WorkerRate`) come from the selected `CompanyProfileJobPositionRate`.
- **Roles without rates.** While creating a request, the company (`POST api/company/profile/jobpositions`) and a non-admin agency user (`POST api/agency/recruiting/clients/{profileId}/jobpositions/WithoutRates`) can create a role with only name, description and shift; it is stored with `Rate = WorkerRate = 0` (`CompanyProfileJobPositionRate.CompanyCreate`). Agency admins create roles with rates (`POST …/jobpositions`) and set the rates of a pending role with the pencil next to the role tag (Create/Review Request page and the Rate KPI of the request detail), which calls `PUT …/jobpositions/{id}`. That update copies the new rates to every request of the role that still has no rates (`IRequestRepository.GetRequestsPendingRates`); requests that already have rates keep them.
- **Duplicate role.** Next to the pencil, every agency user sees a duplicate button that opens the same role form prefilled from the selected role in create mode: admins get the full form (rates copied), recruiters the basic one (saved through `WithoutRates`, so it starts with rates pending). On the Create/Review Request page the new role becomes the request's role; on the Rate KPI of the request detail it only creates the role (the request keeps its current role).
- **No duplicate roles.** A client can have several roles with the same name as long as their rates differ; two active roles with the same name (trimmed, case-insensitive) AND the same `Rate` and `WorkerRate` are rejected (deleted roles don't count). Two roles without rates with the same name therefore collide, while a role without rates may share the name of one with rates. Every create and update path (`AgencyService.CreateCompanyJobPosition`/`UpdateCompanyJobPosition`, `CompanyService.CreateJobPositionWithoutRates`) checks `ICompanyRepository.JobPositionExists` and returns 400 `JobPositionAlreadyExists`.
- **No rates, no work.** `Request.HasRates` = `WorkerSalary` set, or `AgencyRate > 0` and `WorkerRate > 0`. Without rates the request cannot be approved, booked (`AddWorker`), applied to (`IsAvailableToApply`) or invited (`SendInvitation`), and it is hidden from the worker job board and the worker's request list.
- A request created by the agency starts as `RequestStatus.Open`; one created by the company starts as `PendingReview` and is published (website, worker app, invitations, bookings) only after admin/recruiting approves it. The enum has four values: `Open = 1`, `Filled = 3`, `Cancelled = 4`, `PendingReview = 5` (value 2 intentionally skipped).
- `WorkerSalary` (`Request.cs:53`) set = **Direct Hiring** order (permanent placement); changes billing and attendance behavior (see §6).
- State transitions (fill on capacity, reopen, cancel restrictions) are automatic inside the entity — **do not re-implement them**; the authoritative description is `.docs/business/REQUEST_STATE_MANAGEMENT.md`.

Request management endpoints on the same controller:

```
PUT api/agency/recruiting/requests/{id}                              → RequestService.UpdateRequest
PUT api/agency/recruiting/requests/{id}/Approve                      → RequestService.ApproveRequest  (PendingReview → Open + saves the form; policy Recruiting)
PUT api/agency/recruiting/requests/{id}/Cancel                       → RequestService.CancelRequest   (only Open/PendingReview + zero workers)
PUT api/agency/recruiting/requests/{id}/Open                         → RequestService.OpenRequest     (reopen)
PUT api/agency/recruiting/requests/{id}/IncreaseWorkersQuantityByOne → AgencyService.IncreaseWorkersQuantityByOne
PUT api/agency/recruiting/requests/{id}/ReduceWorkersQuantityByOne   → RequestService.ReduceWorkerQuantityByOne
POST api/agency/recruiting/requests/{id}/SendInvitation              → RequestService.SendInvitation  (queues a Service Bus job inviting matching workers AND candidates)
```

Role-scoped listings: `GET api/agency/recruiting/requests` (recruiting) and `GET api/agency/sales/requests` (sales rep sees only their own companies' orders).

### Step 2A: Worker browses and applies (reactive)

```
GET  api/worker/requests                      → RequestsController.Get (Controllers/Workers/Requests/RequestsController.cs)
                                              → IRequestRepository.GetRequestsForWorker
GET  api/worker/requests/{id}                 → RequestsController.GetById
POST api/worker/requests/{requestId}/Apply    → RequestsController.Apply → WorkerService.Apply(model, requestId)
```

The worker's history (`GET api/worker/history[/{id}]` → `Controllers/Workers/History/HistoryController.cs` → `IRequestRepository.GetRequestsHistoryForWorker`) lists one row per `WorkerRequest` of the worker, ordered by `StartWorking ?? CreatedAt` descending. Each row carries a worker-facing status: `Booked` → **Active**, `Finished` with reason `ContractCompleted` → **Completed**, anything else (other finish reasons, rejected) → **Ended**; the worker never sees "Rejected". `?status=` filters by that label; `?status=Past` returns Completed and Ended together (everything but Active). `HoursWorked` = approved hours minus one `DurationBreak` per timesheet when the break is unpaid. See section 4, "Worker view: Activity".

There is no `/Available` suffix — the plain `GET api/worker/requests` already returns only requests the authenticated worker can apply to. The list is built in one SQL query (`GetWorkerRequestsFilter`): a worker with at least one `Booked` assignment sees only their booked requests; otherwise they see the agency's `Open` requests they were not rejected from (`status` = `null`) plus their own booked ones (`status` = `"Booked"`). Query parameters: `sortBy` (`NumberId`, the default, or `Rate`), `isDescending` (defaults to `true`, so the plain call returns newest first), `numberId` (exact match) and `jobTitle` (case-insensitive contains). There is no status filter on purpose: because of the rule above a worker only ever sees one kind of row, so filtering by status would be a no-op. Sigook.App maps its four sort options (newest / oldest / highest rate / lowest rate) to the two sort parameters and has one search box: an all-digit query is sent as `numberId`, anything else as `jobTitle`. Sigook.Web sorts through the `numberId` and `workerRate` column headers and searches through the searchable `numberId`/`jobTitle` columns on desktop, and through the `MobileFiltersPanel` sheet on touch widths.

Both apply routes share one service method, `WorkerService.Apply(WorkerRequestApplyModel model, Guid? requestId = null)`, and one payload, `WorkerRequestApplyModel { NumberId?, Email, Comments }`. The route decides how the applicant is resolved: with `requestId` the worker comes from the token and any `Email` in the body is ignored; without it the worker is looked up by `Email` within the request's agency. Once resolved, `Apply` branches into `ApplyAsWorker` or `ApplyAsCandidate`, each of which owns its duplicate guard, its `RequestApplicant` factory and its notification. `ApplyAsWorker` rejects workers with `ApprovedToWork = false` (`WorkerNotApprovedToWork`); Sigook.Web hides the Apply button and Sigook.App shows the "not approved" card instead of it, but the API is the gate. Sending the recruiter email is `IRequestApplicantNotificationService` (`NEW_APPLICANT` SendGrid template, one overload per applicant kind); it only notifies — the row is always created by the caller beforehand.

Invitation emails (workers and candidates) carry a unified anonymous link `/worker-apply?n={requestNumberId}&e={email}` handled by `POST api/worker/requests/Apply` (`[AllowAnonymous]`) → `RequestsController.ApplyFromInvitation`: the request is resolved by its public `NumberId`, the email is matched (case/whitespace-insensitive) against workers of the request's agency first, then candidates. Candidates are invited and allowed to apply **only if their free-text address contains the request's city** (accent/case-insensitive containment; a request without a city accepts no candidates), are excluded when DNU, and become `RequestApplicant` rows (`Pending`, created by "Sigook"). The invitation consumer (`InvitationConsumer`) sends one batch to workers whose profile location is in the request's city (`Location.CityId`) plus city-matched candidates of the agency (`Candidate.CityId`, free-text address containment only for legacy candidates without a city), deduplicated by email. Both audiences get the same SendGrid template and the same unsubscribe link shape, built from the single `UnsubscribeUrl` setting: `/email-preferences?email={email}`. `POST api/emailpreferences/Unsubscribe` receives the email address and an optional `TypeId` (the `NotificationType` id; when absent it defaults to `NewRequestNotifyWorker`, the only type these emails opt out of), and resolves the audience itself: if the email matches a `User` the opt-out is a `UserNotificationType` row for that type with every channel off, otherwise it falls back to the candidate with that email and stores `Candidate.EmailUnsubscribed`, which excludes them from later invitation batches.

### Step 2B: Agency tracks applicants

Applicants (workers or candidates who applied / were sourced) are managed per request:

```
GET    api/agency/recruiting/requests/{requestId}/applicants          → ApplicantsController.Get
POST   api/agency/recruiting/requests/{requestId}/applicants          → ApplicantsController.Post   (candidate or worker; duplicate-guarded)
GET    api/agency/recruiting/requests/{requestId}/applicants/Search   → IRequestRepository.SearchApplicants
DELETE api/agency/recruiting/requests/{requestId}/applicants/{id}
```

---

## 3. Worker Assignment (Booking) Flow

### Booking

```
POST api/agency/recruiting/requests/{requestId}/workers/{workerProfileId}/Book
→ WorkersController.Post (Controllers/Agencies/Recruiting/Requests/WorkersController.cs)
→ AgencyService.BookWorker(requestId, workerProfileId, AgencyBookWorkerModel)
```

`BookWorker` validates the worker profile (approved, not DNU) and calls `Request.AddWorker`, which creates the `WorkerRequest` with `WorkerRequestStatus.Booked` and auto-transitions the request to `Filled` when capacity is reached (see `REQUEST_STATE_MANAGEMENT.md`).

`WorkerRequestStatus` (`Covenant.Common/Enums/WorkerRequestStatus.cs`) has three values: `Rejected = 2` (removed for cause), `Booked = 3` and `Finished = 4` (the assignment ended normally: contract completed, client reduced staff, worker resigned, other).

### Managing booked workers

```
GET api/agency/recruiting/requests/{requestId}/workers                        → workers assigned to the request
GET api/agency/recruiting/requests/{requestId}/workers/{id}                   → single worker request
PUT api/agency/recruiting/requests/{requestId}/workers/{id}                   → WorkerRequest.UpdateStartWorking (change start date)
PUT api/agency/recruiting/requests/{requestId}/workers/{workerProfileId}/Reject → RequestService.RejectWorker (for cause; may reopen a Filled request)
PUT api/agency/recruiting/requests/{requestId}/workers/{workerProfileId}/Finish → RequestService.FinishWorker (end of assignment; may reopen a Filled request)
PUT api/agency/recruiting/requests/{requestId}/workers/Finish                   → RequestService.FinishAllWorkers (finishes every booked worker)
```

`Finish` takes `FinishAssignmentModel { LastDay, Reason, Comments }` (validated by `FinishAssignmentModelValidator`). Finishing an assignment sends no email to the worker. A finished or rejected worker can still get hours added until `LimitDateToAddTimeSheet` (last day / rejection date + 1 month) but can't punch. Rebooking a finished worker reuses the same row. The company has the single-worker `PUT api/company/requests/{requestId}/workers/{workerProfileId}/Finish`.

The company sees its assigned workers via `api/company/requests/{requestId}/workers` (`Controllers/Companies/Requests/WorkersController.cs`).

---

## 4. Time Tracking Flow (Punch Card)

### Step 1: Worker clocks in / out

The worker reaches the punch card from Jobs: while booked, Jobs shows only the worker's active assignments (`GET api/worker/history?status=Active`), each with an **OPEN PUNCH CARD** button that opens the job page on its Punch Card tab (Sigook.Web `/worker-requests/applied/{id}?tab=punch`, Sigook.App `jobDetails` with `JobPageTab.punchCard`). A booked worker cannot apply to other jobs, so the open-jobs list and filter are hidden. Jobs is the only place that shows active assignments. Punching itself only happens on the Punch Card tab (job title, distance badge and coordinates live there).

A single punch endpoint; the service decides whether the punch is a clock-in or a clock-out:

```
POST api/worker/requests/{requestId}/timesheet
→ TimeSheetsController.Post (Controllers/Workers/TimeSheets/TimeSheetsController.cs)
→ TimesheetService.Register(requestId, WorkerLocationModel)     (payload = GPS location of the punch)

GET api/worker/requests/{requestId}/timesheet/punch-status/{latitude}/{longitude}
→ TimesheetService.GetPunchStatus   (everything the punch card shows, in one call)
  clockType = next expected punch. No date parameter: it depends only on the worker's latest
  punch for the request (open and under 14 h → clock-out, otherwise clock-in; None when the
  worker is not booked yet or the open punch was already approved). clockIn/clockOut/dayHours =
  the open punch, else today's entry. weekHours = entries since Sunday. "Today", "Sunday" and
  elapsed hours are evaluated in the job site's time zone (Request.JobLocation), falling back
  to the caller's coordinates. Never in server time.

GET api/worker/requests/{requestId}/timesheet/clock-type/{latitude}/{longitude}
→ TimesheetService.GetClockType     [Obsolete] clockType only; kept for mobile builds released
  before punch-status. New clients never call it.

GET api/worker/requests/{requestId}/timesheet
→ ITimesheetRepository.GetTimeSheetsForWorker   (per-job timesheet list; kept only for app builds
  released before Activity. Current clients read timesheets from api/worker/activity/timesheets)
```

### Step 2: Agency reviews and edits the punch card

```
GET    api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets
→ WorkerTimeSheetsController.Get (Controllers/Agencies/Recruiting/Requests/WorkerTimeSheetsController.cs)

POST   api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets        → TimesheetService.CreateTimesheet   (manual entry, e.g. attendance "0")
PUT    api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets/{id}   → TimesheetService.UpdateTimesheet
DELETE api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets/{id}   → TimesheetService.RemoveTimeSheet
GET    api/agency/recruiting/requests/{requestId}/workers/{workerId}/TimeSheets/{id}/Usages  → where the timesheet is used (pay stub/invoice)
```

A request-wide view exists at `GET api/agency/recruiting/requests/{requestId}/timesheets` (`TimeSheetsController.cs`).

On the agency punch card the per-day hours input is disabled and the edit icon hidden for users without admin access (superadmin/admin — `useAdmin` in `AgencyPunchCardWorkerContainer.vue`); the attendance `0` is entered by whoever may edit.

### Step 3: Hours breakdown

Regular / overtime / holiday hour classification is computed by `TimesheetCalculatorService` (`Covenant.Core.BL/Services/Accounting/TimesheetCalculatorService.cs`) when pay stubs and invoices are generated. Rules (44-hour OT threshold, holiday handling, `HolidayIsPaid`) are documented in `.docs/business/TIMESHEET_RULES.md`. Night shift is never computed anywhere; do not add night-shift logic.

### Worker view: Activity

Jobs shows what the worker does now (active assignments with the punch card); Activity shows the record: Sigook.Web `/worker-activity`, Sigook.App `/activity` (`/worker-my-work`, `/worker-history` and `/history` redirect there), with tabs Pay Stubs · Timesheets · History. History lists only past assignments (`?status=Past`, chips Completed / Ended); active ones never appear in Activity. Pay stubs are generated only for Canadian workers, so a worker whose profile location is in the USA (`location.city.province.country.code == "USA"`) gets neither the Pay Stubs tab nor the "Last pay" summary card, and opens on Timesheets. The job page only has Detail and Punch Card.

```
GET api/worker/activity/summary?weekStart=YYYY-MM-DD    → ActivityController → WorkerActivityService.GetSummary (last pay stub + week hours)
GET api/worker/activity/timesheets?weekStart=YYYY-MM-DD → ActivityController → WorkerActivityService.GetWeekTimeSheets
GET api/worker/history?status=Past|Completed|Ended      → HistoryController (see Step 2A)
GET api/worker/paystubs?year=                           → PayStubsController (Controllers/Workers/PayStubs/) → IPayStubRepository.GetPayStubsForWorker
GET api/worker/paystubs/years                           → years with pay stubs, newest first
GET api/worker/paystubs/ytd/{year}                      → earnings / deductions / net for the year
GET api/worker/paystubs/{payStubId}/pdf                 → PayStubService.GetPayStubPdfForWorker (404 if not the worker's)
```

- **Week**: Sunday–Saturday, sent by the client as its local Sunday (`weekStart`); the service normalises any date to that Sunday.
- **Day status**: Paid (has a `TimeSheetTotalPayroll`) → Approved (approved in/out present) → Pending (clocked out) → Open. The clients show an Open day before today as **No clock-out**.
- **Hours**: regular / overtime / holiday per day come from `TimesheetCalculatorService.CalculateHoursBreakdown`, accumulated per request across the week (approved times when present, else the worked duration).
- **Clock times**: `ClockInRounded ?? ClockIn` and `ClockOutRounded ?? ClockOut`. A timesheet created or entered by the agency has no clock times and is shown as **Manual entry**; `TimeIn`/`TimeOut` and the approved times are durations from 00:00, never wall-clock times, and are never displayed as such.
- **Day detail** (tap a day): hours (in–out, break, regular, overtime, total) and, when present, adjustments: missing hours / overtime, bonus or others, deductions, reimbursements with their descriptions.
- **Summary**: last pay stub, this week's hours (total, regular, OT, pending approval) and the current Active assignment.

---

## 5. Payroll & Invoicing Flow

Both run from the agency accounting screens. Calculation detail is NOT duplicated here — see `.docs/business/PAYROLL_RULES.md` and `.docs/business/BILLING_RULES.md`.

### 5.1 Pay stub generation

```
GET  api/agency/accounting/paystubs/WorkersReadyForPayStub
→ PayStubsController (Controllers/Agencies/Accounting/PayStubsController.cs)
→ ITimesheetRepository.GetWorkersReadyForPayStub   (workers with unpaid approved timesheets)

POST api/agency/accounting/paystubs/generate        (body: worker profile ids)
→ PayStubsController.GeneratePayStubs → PayStubService.Generate(agencyIds, workerIds)
```

`PayStubService.Generate` iterates workers and calls `GeneratePayStubForWorker`, which aggregates the worker's pending timesheets and computes deductions via `TimesheetCalculatorService.CalculateDeductions(totalEarnings, numberOfWeeks, year, workerProfileId)`.

Deductions are **database table lookups** (CPP, Federal and Provincial tax ranges via `DeductionsRepository`, by earnings and year); EI is the only computed value (`totalEarnings × rates.EmploymentInsurance`). There are no calculator classes. Subcontractor tax-category overrides zero out deductions. Deduction tables are maintained through `api/jobs/deductions` (`DeductionsController`): both the CPP and the income tax tables are imported from the CRA PDFs dropped in the `cra-tables` blob container (blob trigger → `POST .../Cpp/Blob`, `POST .../Tax/Blob`), and one income tax PDF carries the federal and the provincial tables. See [PAYROLL_RULES](PAYROLL_RULES.md#tax-table-maintenance).

Delivery and management:

```
GET    api/agency/accounting/paystubs                    → PayStubService.GetPayStubs (filtered list)
GET    api/agency/accounting/paystubs/file               → filtered list as file export
GET    api/agency/accounting/paystubs/{payStubId}/pdf    → PayStubService.GetPayStubPdf
POST   api/agency/accounting/paystubs/{payStubId}/email  → PayStubService.SendPayStubEmail
POST   api/agency/accounting/paystubs/email/bulk         → queues BulkPayStubEmailJob on Azure Service Bus (ISigookBusClient)
DELETE api/agency/accounting/paystubs/{id}               → PayStubService.DeletePayStub
POST   api/agency/accounting/paystubs                    → PayStubService.CreateManualPayStub (obsolete)
```

The controller also exposes skip-payroll-number endpoints, and accounting reports live under
`api/agency/accounting/reports/*` (`ReportsController`: t4, cra-payroll, payments, subcontractors,
hours-worked, timesheets).

### 5.2 Invoicing

```
POST api/agency/accounting/invoices/Preview   → IInvoiceService.PreviewInvoice   (no persistence)
POST api/agency/accounting/invoices           → IInvoiceService.CreateInvoice
→ InvoicesController (Controllers/Agencies/Accounting/InvoicesController.cs)
```

The controller injects `IInvoiceService` (one `InvoiceService` in `Covenant.Core.BL/Services/Accounting/Invoices/`). Per invoice, the service resolves an `IInvoiceCountryStrategy` (`CanadaInvoiceStrategy` or `UsaInvoiceStrategy`) through `InvoiceCountryStrategyResolver` from the billed company's agency billing location; the strategy decides the number prefix (`AI`/`US`), the tax (`HST` global rate vs per-location tax) and whether unworked statutory holidays are charged.

Key facts (full rules in `BILLING_RULES.md`):

- Invoice subtotal = timesheets (at `AgencyRate`) + additional items + holidays − discounts; tax = `strategy.CalculateTax`.
- Vacations and bonus are **not** billed — the invoice has no such columns (vacation 4% is a pay-stub concept).
- HST is a single global configuration rate, not per-province.
- Worked holidays are always billed at holiday rate on invoices (invoices hardcode `holidayIsPaid: true`), while pay stubs honor the timesheet's `HolidayIsPaid` flag.

Delivery and management:

```
GET    api/agency/accounting/invoices                       → IInvoiceService.GetInvoices (list with totals)
GET    api/agency/accounting/invoices/{invoiceId}/pdf       → IInvoiceService.GetInvoicePdf
POST   api/agency/accounting/invoices/{invoiceId}/email     → IInvoiceService.SendInvoiceEmail (multipart, optional attachments)
GET    api/agency/accounting/invoices/{invoiceId}/paystubs  → pay stubs linked to the invoice (delete warnings)
DELETE api/agency/accounting/invoices/{id}                  → IInvoiceService.DeleteInvoice (cascades selected pay stubs)
PUT    api/agency/accounting/invoices/{invoiceId}/status    → IInvoiceService.ChangeInvoiceStatus (Paid with payment date / Pending)
GET    api/agency/accounting/invoices/file                  → Excel list (N° Invoice, Created At, Company, Total, Status)
GET    api/agency/accounting/invoices/payments-report       → payments follow-up Excel (B4 Tax, Amount, Net, Due Date, Due Days, Payment Date)
POST   api/jobs/scheduletasks/MarkOverdueInvoices           → nightly job: Pending → Due once CreatedAt + CompanyProfile.PaymentTermDays < today
```

Status rules (Pending / Due / Paid, payment terms per company) are in `BILLING_RULES.md` → *Invoice Status & Payment Terms*.

---

## 6. Runner Pipeline Flow

A **Runner** is a Worker actively submitted to a specific order (Request). Candidates cannot be runners — they must be converted to a worker first (`CandidateService.ConvertToWorker`).

**`Request.UsesRunners`** (set when the order is created, default `true`) decides whether the order works with runners. When it is `false` the Runners tab is still visible but **read-only**, and the weekly board hides its "Add runner" button; `RunnerService` rejects create/status/interview on such an order. The recruiter creates the runner, advances it through the recruiting pipeline and schedules interviews, until the client hires or rejects it. Runners live in a tab inside the order detail (next to Applicants/Workers).

Orders **without** runners instead configure a **compliance checklist** — see [§6.1](#61-compliance-checklist-orders-without-runners).

Controller: `Controllers/Agencies/Recruiting/Requests/RunnersController.cs` (`[Authorize(Policy = Recruiting)]`) → `RunnerService`. Domain rules live in the `Runner` entity.

### Step 1: Recruiter adds a runner to the order

The recruiter searches a Worker and picks **Type** = `Active` (applied on their own) or `Passive` (sourced by a recruiter) — `RunnerType` enum. The search uses a dedicated runner-prospect endpoint that returns **only workers** and excludes those already runners on this request:

```
GET  api/agency/recruiting/requests/{requestId}/runners/Search?searchTerm=...   → IRequestRepository.SearchRunnerProspects
POST api/agency/recruiting/requests/{requestId}/runners                         → RunnerService.CreateRunner
     { "workerProfileId", "type": 1 }
```

The runner is created with `Status = SentToClient` and an initial status-history entry (`previousStatus = null`). The same worker cannot be added twice on the same request (rejected by the `RunnerExists` guard).

**Second entry point — the recruiting weekly board.** A recruiter can send a runner straight from their day card; the web reuses the same `CreateRunner.vue` modal:

```
POST   api/agency/recruiting/weeklyboard/runner   → WeeklyBoardService.AddRunner → IRunnerService.CreateRunner
DELETE api/agency/recruiting/requests/{requestId}/runners/{id} → RunnerService.DeleteRunner   (shared with the Runners tab)
```

The board resolves the recruiter from the token, finds their `RequestRecruiter` assignment for that work day and stamps it on `Runner.RequestRecruiterId`, so the card can count and list its runners. Runners created from the order's Runners tab leave that FK null. Sending a runner also appends the request note `"{workerName} was sent"`.

Each runner on a board card exposes the **same actions as the Runners tab** (change status, add interview, view history, delete) through the same modals, and shows its current status as a chip in front of the worker name. Deleting removes the runner with its status history and interviews.

### Step 2: Recruiter advances the status

```
PUT api/agency/recruiting/requests/{requestId}/runners/{id}/Status              → RunnerService.ChangeStatus
    { "status": 2, "comments": "Client requested an interview" }
```

- Any status can move to any other (no fixed order) **except** a `Hired` runner, which is terminal and rejects further changes.
- Each change appends a row to the status history (previous → new, who, when, comments) — never overwrites.
- Moving to `Hired` **requires** a `StartDate` (the date the runner would begin working); the transition is rejected without it.
- Every runner is a worker (`WorkerProfileId` is required at creation).

**Pipeline states** (`Covenant.Common/Enums/RunnerStatus.cs`): `SentToClient(1)`, `InterviewScheduled(2)`, `InterviewRescheduled(3)`, `NoLongerAvailable(4)`, `NoShow(5)`, `WaitingForInterviewFeedback(6)`, `WaitingForFinalDecision(7)`, `Rejected(8)`, `InOnboardingProcess(9)`, `Hired(10)`.

### Step 3: Recruiter schedules interviews

Only allowed while the runner is in `InterviewScheduled` or `InterviewRescheduled`:

```
POST api/agency/recruiting/requests/{requestId}/runners/{id}/Interview          → RunnerService.AddInterview
     { "scheduledDate", "type": 1, "interviewer", "notes" }
```

A runner can have multiple interviews over time. Rescheduling an interview updates its date and **auto-transitions** the runner to `InterviewRescheduled`:

```
PUT api/agency/recruiting/requests/{requestId}/runners/{id}/Interview/{interviewId}/Reschedule   → RunnerService.RescheduleInterview
    { "newDate" }
```

### Step 4: View history

`GET api/agency/recruiting/requests/{requestId}/runners/{id}` returns the runner detail with the full status timeline (latest first) and the list of interviews. The "Add interview" and "Reschedule" actions are hidden/disabled outside the two interview-enabled states, and "Change status" is hidden once the runner is `Hired`.

All of these rules are enforced in the `Runner` domain entity (`CanAddInterview`, the Hired-terminal guard, append-only history), so the API is the source of truth; the UI only mirrors them.

### Step 5: Work every request's runners from one board

**Recruiting → Runners** (`/recruiting/runners`, `Controllers/Agencies/Recruiting/Runners/RunnersController.cs`) lists the runners of every open request of the agency, grouped by request, with the same shell as the Applicants board: filters (request #, name/email, client, position, type, status, start date), *Assigned to me* (requests the caller is recruiter of), expand/collapse, Excel export and a selection bar.

```
GET api/agency/recruiting/runners            → RunnerService.GetAgencyRunners      (page of REQUESTS; a request never splits across pages; only requests with runners appear)
GET api/agency/recruiting/runners/File       → GenerateAgencyRunnersReport          (one row per runner of the filter)
PUT api/agency/recruiting/runners/Status     → RunnerService.ChangeRunnersStatus    { "runnerIds", "status", "comments", "startDate" }
```

Each request header shows `hired/workersQuantity` and whether it still uses runners (a request switched to the compliance checklist shows its runners read-only). Each runner row expands inline into the same history timeline + interviews the Runners tab and the weekly board show, with *Change status*, *Add interview* and *Delete*.

The bulk status change applies the entity rules per runner and never fails as a whole: hired runners, runners of requests without runners and unknown ids are returned in `skipped[]` with the reason, the rest are updated with one history entry each carrying the comments. `InterviewRescheduled` is never a bulk target (it is only set by rescheduling an interview) and `Hired` requires `startDate` for the whole batch.

### 6.1 Compliance checklist (orders without runners)

When `Request.UsesRunners == false`, the order instead carries a **compliance checklist**: the requirements a worker must meet to be placed on it. Each item is a free-text name plus a mandatory/optional flag plus a **document target** (which worker-profile section a document uploaded for it lands in), so the list is fully dynamic — the agency can add anything beyond the defaults.

The agency configures it from the **Configure** link next to the *Uses Runners* switch on the create/edit order page (`AgencyCreateRequest.vue` → `RequestComplianceModal.vue`). The link is hidden while the order uses runners; the items are kept, not deleted, if the switch is turned back on.

**Default list**, pre-filled the first time the switch is turned off (`Sigook.Web/src/modules/agency/recruiting/applicants/compliance.ts`; frontend-only — the API persists exactly what it receives and never seeds):

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
POST api/agency/recruiting/requests        → RequestService.CreateRequest  (creates the items)
PUT  api/agency/recruiting/requests/{id}   → RequestService.UpdateRequest  (reconciles the items)
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
PUT    api/agency/recruiting/requests/{requestId}/applicants/{id}/Status                    → change status (Start / Cancel / Reopen / Confirm)
GET    api/agency/recruiting/requests/{requestId}/applicants/{id}/ComplianceItems           → checklist + per-applicant completion state
POST   api/agency/recruiting/requests/{requestId}/applicants/{id}/ComplianceItems/{itemId}  → complete an item (multipart; optional document)
DELETE api/agency/recruiting/requests/{requestId}/applicants/{id}/ComplianceItems/{itemId}  → uncheck an item (document stays on the profile)
```

In the portal an applicant row expands into its compliance detail row (`ApplicantComplianceDetail.vue`), which is the single hub: checks, uploads and the status buttons (Start / Cancel applicant / Reopen / Confirm) all live there. The same component serves the order's Applicants tab and the cross-order Applicants workspace, and both lists show a status tag column, a compliance progress column (completed / total) and a multi-status filter.

**The checkbox is the submit.** Attaching a file and typing the number/type only fills a local draft — no request is sent, and the file can be attached with the inputs still empty (or replaced/removed before saving). The `POST` fires when the agent ticks the checkbox, carrying the file plus whatever the draft holds; unticking fires the `DELETE`, and the checkbox reverts if the call fails. A **mandatory** item keeps its checkbox disabled until its draft satisfies the service rules (document, plus number/type for identification or the SIN number), with a tooltip naming what is missing; optional items are always tickable. An item whose target slot already holds a document on the profile (`ExistingFileUrl`) is tickable as is — it shows an *On file* link to view the current document and its upload button reads *Replace*, so nothing has to be downloaded and re-attached to complete the item.

Completing an item can attach a document that lands in the worker-profile section named by the item's `DocumentTarget` (WP/W4/I9/Banking-style items land in *Other Documents* with the item name as description). For **worker** applicants, a **mandatory** item with a document target can **only** be completed by uploading the document — unless the profile already holds a document in that target slot, in which case the check alone completes it and the stored file is left untouched — and mandatory identification/social-insurance items additionally require their number (and type) whenever a file is being uploaded — enforced in the service and mirrored in the modal before it posts. **Optional items require nothing**: neither the document nor the inputs, and an optional identification saved without a number keeps the number/type already on the profile. Numbers are duplicate-checked whenever one is present. Candidates can check items but never upload — convert to worker first. **Confirm** requires all mandatory items completed (optional ones don't block) and a worker applicant.

**SIN/SSN-typed identifications** (`IdentificationTypeCode.SinSsn`): completing an Identification item with that type also fills the worker's `SocialInsurance` + SIN document from the same upload (one blob) and auto-completes pending *Social insurance* items. The same auto-fill applies at worker registration (web and app) — SIN is never captured directly at registration. Validation, duplicate checks and replacement rules: `REQUEST_STATE_MANAGEMENT.md` → *Per-applicant compliance & document routing*.

Note: `BookWorker` deletes the applicant row; its completions go with it by cascade.

---

## 7. Staff Attendance Flow (agency users)

Agency staff (superadmin, admin, recruiting, sales) clock themselves in and out, independently of the
worker punch card. Data lives in `UserAttendance`, one row per **user** per day (not per agency).

### Step 1: Clock in / clock out
- The button sits next to the user's name at the bottom of the sidebar (`AttendanceClockButton.vue`):
  play (not started) → stop with elapsed time (clocked in) → check (day closed).
- `POST api/agency/profile/attendance` toggles: no row today → clock in; open row → clock out; closed row → error.
  One clock-in and one clock-out per calendar day.
- "Now" and "today" are the user's own local time: the browser sends its IANA time zone
  (`?timeZone=America/Bogota`) and the API resolves it with `TimeProvider.GetLocalNow(timeZoneId)` (`TimeProviderExtensions` in Common).
  An unknown time zone is rejected. The agency's location plays no part.
- A day left open stays as "missing clock-out" (0 hours) until an admin fixes it.

### Step 2: Hours breakdown
- Each attendance stores its own lunch (`LunchMinutes`): 60 when clocking in Mon–Fri, 0 on Sat/Sun.
  Only an admin can change it, from the punch correction.
- worked = clock-out − clock-in − lunch (capped at the elapsed time).
- Mon–Fri: the first 8 worked hours are regular, the rest is overtime. Sat/Sun: every worked hour is overtime.
- Computed on read in `AgencyService.CalculateAttendanceHours` (mirrored for previews in `Sigook.Web/src/modules/agency/profile/attendance.ts`).

### Step 3: Report and corrections (admins)
- Agency Profile → Users → "Attendance report": filter by user and date range (defaults to the current week, Mon–Sun; max one year),
  totals (worked, regular, overtime), per-day grid (with the lunch deducted) and Excel export.
- The Users grid shows each user's status today.
- Admins edit a punch from the report row (`PUT api/agency/profile/attendance/{id}`): clock-in, clock-out and lunch
  (0–240 min, never longer than the shift), with a mandatory reason;
  the row keeps who edited it and is tagged "Edited".
- The clock-out can be cleared (sent as `null`): the punch reopens and the user can clock out again that day.
  This is the fix for an accidental clock-out.

