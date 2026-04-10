# Request State Management

How a `Request` (job order) transitions between states throughout its lifecycle.

**Source of truth:** `Covenant.Api/Covenant.Common/Entities/Request/Request.cs`

---

## RequestStatus Enum

```csharp
public enum RequestStatus
{
    Open = 1,        // Active order with available capacity (with or without workers)
    Filled = 3,      // All positions filled
    Cancelled = 4    // Cancelled
}
```

There are exactly **three** explicit states. There is no `IsOpen` flag, no `Requested` state, and no `InProgress` state — those are legacy concepts that were removed during the state-management refactor.

> Enum values `2` is intentionally skipped to preserve the original numbering and avoid database migrations after `InProgress` was removed.

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

### 2. `AddWorker(workerId, startWorking)`

Books a worker on the request. Triggers an automatic transition:

```csharp
if (WorkersQuantityWorking >= WorkersQuantity)
    Status = RequestStatus.Filled;
```

Pre-conditions:
- The request must be `IsAvailableToApply` (i.e. `Status == Open`).
- The request must not already be at full capacity.

### 3. `RejectWorker(workerId, detail)`

Rejects an assigned worker. Triggers:

```csharp
if (Status == RequestStatus.Filled && WorkersQuantityWorking < WorkersQuantity)
    Status = RequestStatus.Open;
```

### 4. `Cancel(now)`

Cancels the request. Two strict pre-conditions:

```csharp
if (Status != RequestStatus.Open)
    return Result.Fail("Only orders in Open status can be cancelled");

if (WorkersQuantityWorking > 0)
    return Result.Fail("Cannot cancel orders with workers assigned. Please remove all workers first.");
```

In other words, cancellation is only valid for an `Open` request that has zero booked workers. To cancel a request that already has assignees, the agency must first remove every worker (which transitions a `Filled` order back to `Open`).

### 5. `Open(now)` (reopen)

Reopens a cancelled request. The resulting state depends on the current capacity:

```csharp
Status = WorkersQuantityWorking >= WorkersQuantity
    ? RequestStatus.Filled
    : RequestStatus.Open;
```

### 6. `IncreaseWorkersQuantityByOne()` / `DecreaseWorkersQuantityByOne()`

Capacity changes can flip the status between `Open` and `Filled`:

- Increasing capacity on a `Filled` order → `Open`.
- Decreasing capacity on an `Open` order such that working workers now meet the (smaller) capacity → `Filled`.

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
- `CannotCancelOrdersWithWorkers()` — Validates the cancellation restriction.

---

## Key Endpoints

- `GET    /api/AgencyRequest` — List requests (filterable by `Status`)
- `GET    /api/AgencyRequest/{id}` — Request detail
- `POST   /api/AgencyRequest/{requestId}/Worker` — Assign a worker (automatic transition)
- `PUT    /api/AgencyRequest/{id}/Cancel` — Cancel (only valid when `Open` + no workers)
- `PUT    /api/AgencyRequest/{id}/Open` — Reopen
- `GET    /api/CompanyRequest` — Same listing from the company's perspective
