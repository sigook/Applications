# Request State Management

How a `Request` transitions between states throughout its lifecycle.

**Source of truth:** `Covenant.Api/Covenant.Common/Entities/Request/Request.cs`

---

## RequestStatus Enum

```csharp
public enum RequestStatus
{
    Open = 1,        // Active request with available capacity (with or without workers)
    Filled = 3,      // All positions filled
    Cancelled = 4    // Cancelled
}
```

There are exactly **three** explicit states. Value `2` is intentionally skipped — do not add an enum member for it.

---

## State Diagram

```
┌────────┐    Capacity reached     ┌────────┐
│  Open  │ ──────────────────────▶ │ Filled │
│        │ ◀────────────────────── │        │
└────────┘    Worker rejected /    └────────┘
    │         capacity increased
    │
    │ Cancel() — only allowed when there are no workers assigned
    ▼
┌───────────┐
│ Cancelled │ ──── Open() reopens to Open or Filled
└───────────┘       depending on current capacity
```

---

## Transition Rules

### 1. Initial state

A new `Request` is created with `Status = RequestStatus.Open` (default in the constructor).

### 2. `AddWorker(Guid workerProfileId, DateTime startWorking, string createdBy = null)` (`Request.cs:94`)

Books a worker on the request. Triggers an automatic transition:

```csharp
if (WorkersQuantityWorking >= WorkersQuantity)
    Status = RequestStatus.Filled;
```

Pre-conditions:
- The request must be `IsAvailableToApply` (i.e. `Status == Open`).
- The request must not already be at full capacity.

### 3. `RejectWorker(Guid workerProfileId, string detail, string rejectedBy = null)` (`Request.cs:125`)

Rejects an assigned worker. Triggers:

```csharp
if (Status == RequestStatus.Filled && WorkersQuantityWorking < WorkersQuantity)
    Status = RequestStatus.Open;
```

### 4. `Cancel(now)`

Cancels the request. Three strict pre-conditions, in order (`Request.cs:143`):

```csharp
if (!CanBeUpdated)
    return Result.Fail(TheRequestCanNotBeChanged);

if (Status != RequestStatus.Open)
    return Result.Fail("Only requests in Open status can be cancelled");

if (WorkersQuantityWorking > 0)
    return Result.Fail("Cannot cancel requests with workers assigned. Please remove all workers first.");
```

`CanBeUpdated => Status != RequestStatus.Cancelled` (`Request.cs:78`), so cancelling an already-cancelled request fails first with the generic "can't be changed" message, not the Open-only one.

In other words, cancellation is only valid for an `Open` request that has zero booked workers. To cancel a request that already has assignees, the agency must first remove every worker (which transitions a `Filled` request back to `Open`).

Note: after the guards, `Cancel()` runs `foreach (WorkerRequest worker in Workers) worker.Reject(...)` (`Request.cs:153-157`), but this loop is effectively unreachable for booked workers — the `WorkersQuantityWorking > 0` guard above already rejected any request with them. Do not "fix" that guard assuming the loop handles booked workers.

### 5. `Open(now)` (reopen)

Reopens a cancelled request. Calling it on an `Open` or `Filled` request is a silent no-op (returns `Result.Ok()`). For a `Cancelled` request, the resulting state depends on the current capacity, and the duration term is reset:

```csharp
Status = WorkersQuantityWorking >= WorkersQuantity
    ? RequestStatus.Filled
    : RequestStatus.Open;
```

Reopening also sets `DurationTerm = DurationTerm.LongTerm` (`Request.cs:179`), regardless of the original term.

### 6. `IncreaseWorkersQuantityByOne()` / `DecreaseWorkersQuantityByOne()`

Capacity changes can flip the status between `Open` and `Filled`:

- Increasing capacity on a `Filled` request → `Open`.
- Decreasing capacity on an `Open` request such that working workers now meet the (smaller) capacity → `Filled`.

---

## Business Rules

### Cancellation is restricted

**Rule:** A request can only be cancelled while it is `Open` and has zero booked workers.

**Why:** This protects already-committed workers. If workers are assigned, the agency must explicitly reject each one — which gives a clear audit trail and ensures workers don't lose a shift silently.

### `IsAvailableToApply`

```csharp
public bool IsAvailableToApply => Status == RequestStatus.Open;
```

Workers can only apply to (and the agency can only assign to) requests in the `Open` state. Filled and Cancelled requests reject new bookings.

### Automatic transitions

State changes happen automatically inside the entity when workers are added/removed or capacity is adjusted. Callers should not set `Status` directly.

---

## Frontend Display

The frontend should map `Status` directly to a single label/badge:

| Status     | Display    | Color  |
|------------|------------|--------|
| Open       | Open       | yellow |
| Filled     | Filled     | green  |
| Cancelled  | Cancelled  | red    |

The "Cancel" action should only be visible when `status === 'Open'` (and ideally only when there are no workers assigned, to mirror the backend rule).

---

## Test Coverage

Relevant tests in `Covenant.Tests/Request/RequestTest.cs`:

- `WorkersQuantityWorking()` — Validates automatic transitions when adding/removing workers.
- `StatusTransitions()` — Validates all states and transitions.
- `CannotCancelRequestsWithWorkers()` — Validates the cancellation restriction.

---

## Key Endpoints

Agency request controllers live in `Covenant.Api/Controllers/Sigook/Agency/Requests/` (shared detail/actions) with role-scoped lists under `Recruiting/` and `Sales/`.

- `GET    /api/agency/recruiting/requests` — List requests (recruiting-scoped; sales users have their own scoped list)
- `GET    /api/agency/requests/{id}` — Request detail (`Requests/RequestsController`)
- `POST   /api/agency/requests/{requestId}/Workers/{workerId}/Book` — Assign a worker (automatic transition; `Requests/WorkersController` → `AgencyService.BookWorker`)
- `PUT    /api/agency/requests/{id}/Cancel` — Cancel (only valid when `Open` + no workers; also `PUT /bulk-cancel`)
- `PUT    /api/agency/requests/{id:guid}/Open` — Reopen
- `GET    /api/CompanyRequest` — Same listing from the company's perspective (`CompanyModule/CompanyRequest`)

---

## Request Applicant States

Each applicant on an order (`RequestApplicant`, worker or candidate) has its own lifecycle, independent of the order's `RequestStatus`. Enum `RequestApplicantStatus` (stored as string, column default `Pending`):

| Value | Int | Meaning |
|---|---|---|
| `Pending` | 1 | Applied on their own (web/app/anonymous apply, website form via bus); nobody has reviewed them yet |
| `InProgress` | 2 | Being worked by the agency; compliance items can be checked and documents uploaded |
| `Confirmed` | 3 | Manually confirmed; terminal unless deleted |
| `Cancelled` | 4 | Discarded, reversible |

### State diagram

```
                 (apply / bus)               (manual add / CSV bulk)
                      │                                │
                      ▼                                ▼
                  ┌─────────┐   Start            ┌────────────┐
                  │ Pending │ ────────────────►  │ InProgress │
                  └─────────┘                    └────────────┘
                      │                            │        ▲ │
                      │ Cancel              Cancel │ Reopen │ │ Confirm
                      ▼                            ▼        │ ▼
                  ┌───────────┐◄───────────────────┘   ┌───────────┐
                  │ Cancelled │ ───────────────────────│ Confirmed │
                  └───────────┘        (none)          └───────────┘
```

### Transition rules (`RequestApplicant`, validated by `RequestApplicantService.ChangeStatus`)

- `MoveToInProgress()` — from `Pending` (Start) or `Cancelled` (Reopen). Compliance checks are kept across Cancel/Reopen.
- `Cancel()` — from `Pending` or `InProgress`; fails from `Confirmed`/`Cancelled`.
- `Confirm()` — only from `InProgress`, only for **worker** applicants (candidates must be converted to worker first), and the service additionally requires **every mandatory compliance item completed** (optional items don't block). `Pending` is never a valid target (validator rejects it).

### Per-applicant compliance & document routing

Completions live in `RequestApplicantComplianceItems` (unique per applicant+item; cascade with the applicant, restrict on the checklist item — deleting an item from the order's checklist explicitly deletes its completions during reconcile). Completing an item may upload a document routed by the item's `ComplianceDocumentTarget` to the worker profile:

| Target | Lands in | Extra data required |
|---|---|---|
| `Identification1` / `Identification2` | Identification slot 1 / 2 | Number + identification type — **only when the item is mandatory** (always duplicate-checked when a number is present) |
| `SocialInsurance` | SIN document | SIN number — **only when the item is mandatory** |
| `Resume` | Resume | — |
| `PoliceCheck` | Police check (sets the flag) | — |
| `OtherDocument` | Other Documents (description = item name) | — |
| `None` | No upload allowed | — |

For worker applicants a **mandatory** item with target ≠ `None` **requires** the document to be completed (manual check rejected); optional items and candidates complete by check alone. Unchecking deletes the completion row; documents already uploaded stay on the profile. All of it is driven from the portal's Compliance modal (see `WORKFLOWS.md` §6.2).

**Optional items never force the extra inputs.** On an optional item the document can be attached with the number/type fields empty: the identification slot then keeps the number and type already on the profile (they are never wiped) and only the file is patched. Conversely the profile is also patched when the inputs are filled **without** a file, so nothing typed is silently dropped — the completion runs the routing branch whenever there is a file **or** any identification/SIN value in the payload. Duplicate and SIN checks only run when a number is actually resolved.

When an Identification item is completed with a **SIN/SSN-typed** identification (`IdentificationTypeCode.SinSsn` on the catalog row), the upload also fills the worker's `SocialInsurance` + SIN document (same blob) after validating the number as a SIN — length 9-15 and not owned by another profile. If the profile already holds a **different** SIN it is **replaced** and a `WorkerProfileNote` records the change (masked old → masked new, author = the acting user); this applies to the documents form flow too. Pending checklist items with target `SocialInsurance` are **auto-completed** in the same save (same `CompletedBy`). Each compliance item row also exposes `ExistingFileUrl` — the document currently on the profile for that slot — so the agent can review the current file before replacing it.

### Test coverage

- `Covenant.Tests/Request/RequestApplicantTest.cs` — entity transitions.
- `Covenant.Tests/Request/RequestApplicantServiceTest.cs` — mandatory gating, document routing, idempotence.
- `Covenant.Integration.Tests/.../ApplicantsControllerTest.cs` — endpoints, status filter, candidate-confirm rejection.
