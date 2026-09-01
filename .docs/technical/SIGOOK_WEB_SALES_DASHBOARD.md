# Sigook.Web Sales Dashboard

The sales activity home of the Sales module (`/sales/dashboard`): snapshot cards plus the create/edit entry points for interactions, clients and deals.

> **Hybrid prototype.** The summary payload (period label, Clients card, "Deals closed" chart, goal donut, pipeline/activity meters) is served from static JSON frozen at Q3 2026 (`asOf` 2026-07-12). Only the Interactions and Deals lists and every create/edit/delete flow hit the live backend. See "Going Live (the swap seam)".

---

## Access & Navigation

| Route | Name | Page | Guard |
|-------|------|------|-------|
| `/sales/dashboard` | `sales-dashboard` | `pages/agency/Dashboard.vue` | `requiresAuth` + `salesAccess` |
| `/sales/interactions` | `sales-interactions` | `pages/agency/SalesInteractions.vue` | `requiresAuth` + `salesAccess` |
| `/sales/deals` | `sales-deals` | `pages/agency/SalesDeals.vue` | `requiresAuth` + `salesAccess` |

Routes in `Sigook.Web/src/router/routesAgency.ts:169-194` (lazy imports at `:21-23`). `salesAccess` = superadmin, admin, sales (`src/security/roles.ts:15`); anyone else is redirected to `/unauthorized` by the guard in `src/router/index.ts`.

Sidebar: **Dashboard** is the first item of the Sales group (`src/security/menu.ts:91-95`, icon `view-dashboard-outline`), followed by Interactions, Clients (`/sales/companies`) and Deals — plus Agencies for admins of a master agency. Admin/superadmin get the Sales group next to Recruiting and Accounting; sales users get only Sales.

The dashboard is **not** a default home page: sales lands on `/sales/requests` after sign-in (`menu.ts:205-206`) — a route with no sidebar entry of its own — so the dashboard is reached through the menu.

---

## Page Layout

`src/pages/agency/Dashboard.vue` (`<script setup>`; all state in component-local refs — no Pinia store, consistent with the stores-hold-filters-only rule). Two CSS grids: a 3-card top row and a 2-card bottom row. Below 1215px the grids collapse to 2/1 columns; below 768px to a single column, and the period label hides.

| Card | Content | Data source | Actions |
|------|---------|-------------|---------|
| Log Interactions | `SalesInteractionList` — 6 most recent interactions, icon per type, relative timestamps | **Live** — `getCompanyInteractions` (pageSize 6, newest first) | "+ Log interaction"; row click opens edit |
| Clients | `SalesClientList` — client rows (initials avatar + industry); subtitle "N active · N new this month" | **Static** — `clients` block | "+ Create client" |
| Deals | `SalesDealList` — 6 most recent deals: status pill, optional document link, compact value | **Live** — `getDeals` (pageSize 6, newest first) | "+ Create deal"; row click opens edit |
| Deals closed | `SalesBarChart` (responsive SVG, d3-scale) + `SalesRangeTabs` (Week / Month / Quarter) | **Static** — `dealsClosed` series | Range tabs only |
| This quarter | `SalesGoalDonut` (d3 arc, animated) + two `SalesMeterList`s: "Pipeline by status" and "Activity this week" | **Static** — `goal`, `pipeline`, `activity` blocks | — |

Card titles link to the full pages (`/sales/interactions`, `/sales/companies`, `/sales/deals`). The header shows "Sales Dashboard · {agent name}" (via `useCurrentAgent`) and the frozen period label.

---

## Data Flow

```
Dashboard.vue onMounted (Dashboard.vue:209-221)
├─ getSalesDashboard()             → src/data/sales/salesDashboard.json        [STATIC]
├─ getCompanyInteractions({...6})  → GET /api/agency/sales/companyinteractions [LIVE]
├─ getDeals({...6})                → GET /api/agency/sales/deals               [LIVE]
└─ useCurrentAgent.loadAgentName() → GET /api/agency/personnel                 [LIVE]

SalesCreateModal @saved → onSaved (Dashboard.vue:204-207)
└─ reloads interactions + deals only — the static blocks never refresh
```

> **Half-live refresh.** Saving a deal or interaction does not move the pipeline meters, the goal donut, the "Deals closed" chart or the Clients card — those render the frozen JSON. The range tabs are client-side only: they index into the pre-baked `dealsClosed.{week,month,quarter}` arrays; no request is made.

---

## Going Live (the swap seam)

`src/api/salesDashboardApi.ts` is the entire seam:

```ts
import dashboardData from '@/data/sales/salesDashboard.json';
import type { SalesDashboardModel } from '@/types/sales';

// ---------------------------------------------------------------------------
// Sales dashboard
//
// Static prototype: the payload is served from src/data/sales/salesDashboard.json
// and shaped exactly like the future endpoint response. To go live, replace the
// body with the commented call below — the signature and every caller stay as is.
//
//   import { api } from '@/security/apiService';
//   export function getSalesDashboard(): Promise<SalesDashboardModel> {
//     return api.get<SalesDashboardModel>('/api/agency/sales/dashboard');
//   }
// ---------------------------------------------------------------------------

export function getSalesDashboard(): Promise<SalesDashboardModel> {
  return Promise.resolve(dashboardData as unknown as SalesDashboardModel);
}
```

> **`GET /api/agency/sales/dashboard` does not exist yet.** `Covenant.Api/Covenant.Api/Controllers/Sigook/Agency/Sales/` contains only `CompanyProfilesController`, `RequestsController`, `DealsController` and `CompanyInteractionsController`, and the endpoint is absent from `openapi.json`.

The JSON is shaped exactly like the future response (`SalesDashboardModel`, `src/types/sales.ts:56-64`), with enum fields as numeric values. Going live = implement the backend endpoint, then swap the function body for the commented call; every caller and type stays as is. Two decisions the shape already implies: `dealsClosed` ships all three ranges pre-aggregated (no range query param), and `period` is computed server-side.

---

## CRUD Flows

All creation and editing runs through `SalesCreateModal` (`src/components/sales_dashboard/SalesCreateModal.vue`): a Buefy modal that switches between three forms by `kind` (`SalesCreateKind = 'interaction' | 'client' | 'deal'`), shows create vs edit titles, and in edit mode offers Delete behind a confirm dialog. It emits `saved`, which the host page uses to reload its lists.

| Kind | Form | API functions | Endpoint | Notes |
|------|------|---------------|----------|-------|
| `interaction` | `SalesInteractionForm` | `createCompanyInteraction` / `updateCompanyInteraction` / `deleteCompanyInteraction` (`companyApi.ts:226-243`) | `/api/agency/sales/companyinteractions` | Client picker via `getAgencyCompaniesList`; client is read-only when editing |
| `deal` | `SalesDealForm` | `createDeal` / `updateDeal` / `deleteDeal` (`companyApi.ts:203-224`) | `/api/agency/sales/deals` | Create is `multipart/form-data` with an optional document (`buildMultipartFormData` + `generateFileName`) |
| `client` | `SalesClientForm` | `createAgencyCompany` (`agencyCompanyApi.ts`) | `POST /api/agency/companyprofiles` | Create-only from the modal; catalogs via `getIndustries` / `getCompanyStatus`; it is the ordinary company endpoint, so sales auto-assignment applies |

The deals/interactions CRUD lives in the sales section of `companyApi.ts` (`:203-243`), **not** in `salesApi.ts`.

The modal is reused outside the dashboard: `/sales/interactions` and `/sales/deals` are full paginated Buefy tables (sorting via `useGridSort`) that open the same modal for create/edit/delete.

---

## Business Semantics

The enums mirror the backend **numerically** — the API serializes enums as ints (System.Text.Json, no `JsonStringEnumConverter`), so the values in `src/types/company.ts:344-589` must match `Covenant.Common` exactly. Label/color/icon maps live next to the enums.

| Enum | Values (labels) |
|------|-----------------|
| `DealType` | Temporal, Permanent, Temp to Perm |
| `DealStatus` | To Send, Sent, Rejected, Accepted, Under Review, Closed, Completed |
| `InteractionType` | Call, Email, SMS, LinkedIn |
| `InteractionPurpose` | Intro, Follow-up, Proposal, Negotiation, Closing |
| `InteractionStatus` | Not started, In progress, Completed |

`DealStatus` values above are listed in numeric order (0–6, the int contract). Display order in the form select (`DEAL_STATUSES`) and the dashboard pipeline meters is lifecycle order instead: To Send → Sent → Under Review → Accepted → Rejected → Closed → Completed. Table sort-by-status remains numeric, so Under Review (4) groups after Accepted (3).

**Ownership scoping.** Deals and interactions are owner-scoped **end-to-end**: a sales user lists, updates and deletes only the records they own, and `OwnerId` is forced server-side on create (admin/superadmin are unscoped). Controllers: `Covenant.Api/Covenant.Api/Controllers/Sigook/Agency/Sales/{DealsController,CompanyInteractionsController}.cs`, `[Authorize(Policy = PolicyConfiguration.Sales)]`. This is stricter than the orders/clients rule, where only the list is scoped — see the note in `../business/ROLES_PERMISSIONS.md`.

Backend data model: `Covenant.Common/Entities/Company/{Deal,CompanyInteraction}.cs`; migration `Covenant.Api/Covenant.Infrastructure/Migrations/20260811133554_AddDealsAndInteractions.cs`.

---

## File Inventory

### Pages

| File | Purpose |
|------|---------|
| `src/pages/agency/Dashboard.vue` | The dashboard |
| `src/pages/agency/SalesInteractions.vue` | Full interactions grid, reuses `SalesCreateModal` |
| `src/pages/agency/SalesDeals.vue` | Full deals grid, reuses `SalesCreateModal` |

### Components — `src/components/sales_dashboard/` (15 files)

| Group | Files |
|-------|-------|
| Card shell & lists | `SalesCard` (icon chip, linked title, action button, body slot), `SalesList` (scroll + empty state), `SalesListRow` (row primitive), `SalesInteractionList`, `SalesClientList`, `SalesDealList` |
| Charts | `SalesBarChart` (d3-scale SVG bars, `useElementSize`), `SalesGoalDonut` (d3 arc, `useTween`), `SalesMeterList` (horizontal meters), `SalesRangeTabs` (week/month/quarter `v-model`) |
| Create/edit | `SalesCreateModal` (kind switcher + delete), `SalesInteractionForm`, `SalesDealForm` (file upload), `SalesClientForm` (full client creation: logo, industry with add-new, status, sales rep, contact info), `SearchSelect` (generic autocomplete used by the client pickers) |

### API layer

| File | Role |
|------|------|
| `src/api/salesDashboardApi.ts` | Static summary payload (the swap seam) |
| `src/api/companyApi.ts:203-243` | Deals + interactions CRUD |
| `src/api/agencyCompanyApi.ts` | `createAgencyCompany` (client form); `getAgencyCompaniesList` (client pickers: 50 on open, searches from 3 characters) |
| `src/api/catalogApi.ts` | `getIndustries`, `getCompanyStatus`, `addIndustry` |
| `src/api/agencyApi.ts` | `getAgencyPersonnel` (header agent name, sales-rep picker) |

### Types, composables, utils, data

| File | Contents |
|------|----------|
| `src/types/sales.ts` | `SalesDashboardModel` + blocks, `SalesRangeKey`, `SalesCreateKind`, `SALES_RANGE_TABS` |
| `src/types/company.ts:344-589` | Deal/interaction enums, label/color/icon maps, `Deal`, `CompanyInteraction`, filters, create/update models |
| `src/composables/` | `useCurrentAgent`, `useElementSize`, `useTween`, `useDropdownReveal`, `useGridSort` (added with this feature); reuses `useStickyForm`, `useAdmin`, `usePubSub` |
| `src/utils/salesDashboardFormat.ts` | `compactMoney`, `relativeTime`, `shortDate`, `initialsOf`, `ratioOf` |
| `src/data/sales/salesDashboard.json` | The frozen summary payload |

---

## Known Gaps

- The summary payload is frozen at Q3 2026 (`asOf` 2026-07-12); the period label and every chart render the same numbers regardless of today.
- `GET /api/agency/sales/dashboard` has no backend implementation.
- Saves refresh only the two live lists; static blocks stay stale until the endpoint exists.
- The range tabs never hit the network (pre-baked series).
- Creating a client refreshes nothing on the dashboard — the Clients card reads the static JSON.
- The client form is create-only from the modal (no edit/delete path).
- All copy is hardcoded English — consistent with the app, which has no vue-i18n.

---

## Related Documents

- [SIGOOK_WEB_STRUCTURE.md](./SIGOOK_WEB_STRUCTURE.md) — overall folder layout and routes
- [SIGOOK_WEB_API_MAP.md](./SIGOOK_WEB_API_MAP.md) — full endpoint tables (`companyApi.ts`, `salesApi.ts`, `salesDashboardApi.ts`)
- [ROLES_PERMISSIONS.md](../business/ROLES_PERMISSIONS.md) — sales scoping rules
- `openapi.json` — deals/interactions endpoint contracts
