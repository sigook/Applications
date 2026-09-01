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
| Deals closed | How much value have I closed, by day / week / month, for the current week / month / quarter? |
| This quarter | Progress toward the quarterly value goal, plus two breakdowns: **pipeline by status** (how many deals sit in each `DealStatus`) and **activity this week** (how many interactions per channel) |

### KPI definitions (proposed — confirm when the endpoint is built)

The live endpoint does not exist yet, so these are the intended semantics inferred from the cards and the payload shape, not implemented rules. The sample payload does not even distinguish some of them (goal `actual` equals `pipelineValue`).

| KPI | Definition |
|-----|------------|
| Active clients | Companies in an active status (not Blocked / Inactive) |
| New this month | Companies created in the current calendar month |
| Pipeline value | Sum of `Value` over open deals (To Send + Sent) |
| Deals closed | Sum of `Value` over deals that reached **Accepted**, bucketed by the deal `Date` |
| Quarterly goal | Closed value in the quarter vs. a target amount |
| Pipeline by status | Count of deals per `DealStatus` |
| Activity this week | Count of interactions per `InteractionType` in the current week |

> **The summary cards are not live.** Clients, Deals closed, This quarter and the period label render **frozen sample data for Q3 2026**; only the Interactions and Deals lists, and every create/edit/delete action, hit the backend. The quarterly target cannot be set anywhere. Technical detail: `.docs/technical/SIGOOK_WEB_API_MAP.md` §18.

### Where things are decided

Two shape decisions the dashboard already commits to, which the backend must honor when it goes live:

- The "Deals closed" chart ships **all three ranges** (week / month / quarter) at once; switching tabs never re-queries.
- The period label ("Q3 2026") is **computed server-side**, not by the browser.

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
