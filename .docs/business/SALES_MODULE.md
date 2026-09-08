# Sales Module - Deals, Interactions & Dashboard

The Sales module is where an agency's sales representatives work their client pipeline: prospect companies, log every touchpoint, track the proposals (deals) they send, and follow their numbers on a dashboard. It complements the recruiting side (orders, runners) described in `WORKFLOWS.md`.

Who uses it: `sales` reps (scoped to their own records) and `admin` / `superadmin` (unscoped). Role definitions and the scoping rules are in `ROLES_PERMISSIONS.md`.

---

## Concepts

### Client

A client in the sales module is an ordinary `CompanyProfile` — the same entity recruiting and accounting use. Sales sees it through its status pipeline (`Lead → Potential → Prospect → Quoted → Client`, see `BUSINESS_MODEL.md`), and can create new ones from the dashboard.

Creating a client from the sales module goes through the **ordinary company creation**, so the **sales auto-assignment rule applies**: a sales user who creates a company is set as its sales representative server-side, whatever the form sent (`ROLES_PERMISSIONS.md`, "Sales auto-assignment").

### Interaction

A logged touchpoint between a sales rep and a client company (`CompanyInteraction`). One record per contact made.

| Field | Meaning |
|-------|---------|
| Client | The `CompanyProfile` contacted. Fixed once the interaction is created (cannot be moved to another client). |
| Description | Free-text note of what happened |
| Type | The channel used — see catalog below |
| Purpose | Where in the sales conversation this touchpoint sits — see catalog below |
| Status | Progress of the touchpoint itself (e.g. a scheduled call not yet made) |
| Owner | The user who logged it. Set server-side, never chosen by the client. |

**Type** (`InteractionType`): Call, Email, SMS, LinkedIn.

**Purpose** (`InteractionPurpose`) — the stages of a sales conversation, in order:

| Purpose | Meaning |
|---------|---------|
| Intro | First contact with the company |
| Follow-up | Keeping the conversation alive after the intro |
| Proposal | Presenting an offer / quote |
| Negotiation | Working out terms |
| Closing | Getting the final yes/no |

**Status** (`InteractionStatus`): Not started → In progress → Completed. New interactions default to *Not started*.

### Deal

A commercial proposal made to a client company (`Deal`): what was offered, for how much, and whether the client accepted it.

| Field | Meaning |
|-------|---------|
| Title | Short name of the proposal |
| Client | The `CompanyProfile` the deal is for |
| Date | Business date of the deal (not the creation timestamp) |
| Value | Monetary value of the proposal. Feeds the dashboard's pipeline value and "deals closed" figures. |
| Type | Kind of staffing being sold — see below |
| Status | Where the proposal is — see lifecycle below |
| Document | Optional attached file (the proposal / contract). Attached at creation. |
| Owner | The user who created it. Set server-side. |

**Type** (`DealType`) mirrors the kinds of placement the agency sells:

| Type | Meaning |
|------|---------|
| Temporal | Temporary staffing billed by the hour (regular orders with `AgencyRate` / `WorkerRate`) |
| Permanent | Direct hire — the company hires the worker on a salary (see "Direct Hiring" in `BUSINESS_MODEL.md`) |
| Temp to Perm | Starts as temporary, converts to a direct hire |

**Status lifecycle** (`DealStatus`):

```
To Send ──► Sent ──► Accepted   (closed-won)
                └──► Rejected   (closed-lost)
```

- **To Send** — drafted, not yet presented to the client.
- **Sent** — delivered to the client; waiting on an answer.
- **Accepted** — the client took the deal. Counts as a closed deal.
- **Rejected** — the client declined.

The status is set by the rep by hand; nothing moves it automatically. A deal does not create an order (`Request`) — orders are created through the recruiting/company flows in `WORKFLOWS.md` §2.

---

## Ownership rule

Deals and interactions are **owner-scoped end-to-end**: a sales rep only lists, edits and deletes the ones they own, and the owner is stamped server-side on creation. Admin and superadmin see and manage everyone's.

This is stricter than the rest of the sales module (orders and clients), where only the *list* is scoped and the detail is open. Rationale and endpoint references: `ROLES_PERMISSIONS.md`, "Exception: deals & interactions are owner-scoped end-to-end".

---

## Sales Dashboard

Landing page of the module (`/sales/dashboard`, reached from the sidebar — signing in as sales lands on the orders list, not here). It gives a rep a one-screen view of their activity and shortcuts to log an interaction, create a client or create a deal.

| Card | What it answers |
|------|-----------------|
| Log Interactions | What were my last touchpoints? (6 most recent, newest first) |
| Clients | Who are my clients? How many are active, how many are new this month? |
| Deals | What are my latest proposals and where are they? (6 most recent) |
| Deals by status | How many deals are To Send, Sent, Under Review, Accepted, Rejected, Closed or Completed for today / this week / this month? |
| This quarter | Two breakdowns: **pipeline by status** (how many deals sit in each `DealStatus` this quarter) and **activity this week** (how many interactions per channel) |

### KPI definitions

Served live by `GET api/agency/sales/dashboard/*` (see `.docs/technical/SIGOOK_WEB_API_MAP.md` §18).

| KPI | Definition |
|-----|------------|
| Active clients | Companies in an active status (not Blocked / Inactive) |
| New this month | Companies created in the current calendar month |
| Deals by status | Count of deals per `DealStatus`, plus the summed `Value` per status, over deals whose `Date` falls in the selected period |
| Pipeline by status | Count of deals per `DealStatus` over deals whose `Date` falls in the current quarter |
| Activity this week | Count of interactions per `InteractionType` whose `CreatedAt` falls in the current week |

Every window is resolved **server-side in UTC**, whatever the server's own time zone is: the
instant is converted with `now.UtcDateTime` before the calendar math. The week runs **Sunday to
Saturday** — the same day-of-week boundary payroll uses, but anchored to UTC midnight, not to a
business time zone. The response always carries one row per status (or per requested status),
zero-filled, so the chart keeps a stable column order.

> **There is no quarterly goal.** No entity stores a target amount, so the dashboard shows no goal
> donut. Adding one needs a new column plus a screen to set it.

### Where things are decided

- The period selector (Today / This week / This month) **re-queries** — each tab is one request.
- The period label ("Q3 2026", "Sep 6 - Sep 12, 2026") is **computed server-side**, not by the browser.
- Column colors and labels live in the **front end** (`DEAL_STATUS_COLORS`, `DEAL_STATUS_LABELS`); the
  endpoint returns data only, so it stays chart-agnostic.
- `Deal.Date` is captured as the browser's local midnight in ISO form, so a rep east of UTC can file
  a deal one UTC day early. Bucketing cannot fix that; capture would have to.

---

## Full pages

Beyond the dashboard, the module has full paginated lists for **Interactions** (`/sales/interactions`) and **Deals** (`/sales/deals`) with the same create / edit / delete actions, plus the sales-scoped **Clients** (`/sales/companies`) and **Orders** (`/sales/requests`) lists described in `ROLES_PERMISSIONS.md`.

---

## Related Documents

- `ROLES_PERMISSIONS.md` — sales scoping, auto-assignment, owner-scoped exception
- `BUSINESS_MODEL.md` — company status pipeline, direct hiring
- `WORKFLOWS.md` — order creation and the recruiting pipeline that follows a won deal
- `.docs/technical/SIGOOK_WEB_STRUCTURE.md` — routes, dashboard layout, `sales_dashboard/` components
- `.docs/technical/SIGOOK_WEB_API_MAP.md` — deals / interactions endpoints and modal wiring (§14), static dashboard summary (§18)
- `.docs/technical/ENTITIES_RELATIONSHIPS.md` — `Deal` / `CompanyInteraction` entities
