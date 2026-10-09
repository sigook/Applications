# Sigook.Web — Vue 3 Agency Portal (Main Platform)

Migrated from Vue 2 → Vue 3 in PR #108. Stack: Vite + TypeScript + Pinia + Vue Router 4 + `buefy` 3.x + VeeValidate 4 + Yup + oidc-client-ts.

## Buefy 3

UI library is `buefy` 3.x (official Vue 3 release, successor of `@ntohq/buefy-next`). It bundles Bulma 1.x.

- CSS entry: `buefy/dist/css/buefy.css` (imported in `src/main.ts`)
- `index.html` pins `<html data-theme="light">` — Bulma 1 auto-switches to dark on `prefers-color-scheme: dark`, which this app is not designed for
- Component docs: use the `buefy` MCP server (`buefy_search`, `buefy_get_component`) instead of guessing props

## Reference Docs (read these first)

- `.docs/technical/SIGOOK_WEB_API_MAP.md` — every feature `api.ts` → backend endpoint, types, Pinia wiring
- `.docs/technical/SIGOOK_WEB_STRUCTURE.md` — folder layout, routes, views by feature (incl. sales dashboard layout), global plumbing
- `.docs/business/SALES_MODULE.md` — sales module concepts (deals, interactions, dashboard KPIs); the dashboard aggregate endpoints are in SIGOOK_WEB_API_MAP.md §18

## Code Navigation

The tree mirrors the navigation: **portal → menu → feature**, and everything shared between portals is explicit.

```
src/main.ts                      entry (index.html)
src/app/                         SPA shell: App.vue, globals.ts, router/index.ts (guard + composes modules/*/routes.ts), security/ (apiService, securityService, roles, menu, authErrors, authApi), stores/ (index, app, security), layout/ (SidebarLogged, UserNotification), pages/ (Callback, SilentRefresh, Unauthorized, NotFound, EmailPreferences, auth/)
src/shared/<concept>/            used by 2+ portals; the folder name is the concept it comes from:
                                 ui/ (SigookGrid, PageHeader, Breadcrumbs, PhoneInput, Address, PreviewImage, SearchSelect, Export, EyebrowPill, MobileFiltersPanel, SheetPanel…), detail-page/, request-detail/, worker-profile/{cards,forms}/ (+ api.ts, types.ts), punch-card/, company-profile/ (+ types.ts, useCompanyDetail), api/ (catalog, location, account, userNotification, shared, reportApi = generic Excel download), composables/, utils/, format/ (date/money/text formatters, import from '@/shared/format'), constants/ (enums, catalog), types/ (common, security, invoice)
src/modules/<portal>/            routes.ts + store.ts + one folder per menu/feature:
  agency/                        shared/ (notes/, notifications/, useModuleBase, AgencyRequests, AgencyWorkers), profile/, recruiting/{requests (+ runners/, timeSheetApi.ts), applicants, weekly-board, candidates, workers, clients}/, sales/{dashboard, clients, agencies}/ (+ sales/api.ts), accounting/{invoices, paystubs, reports}/
  company/                       requests/, invoices/, profile/
  worker/                        register/, requests/, history/, profile/
  landing/                       pages/, components/, layout/ (Header, Footer, GlobalBackground, AppVersionToast — App.vue mounts them), data/, composables/, api.ts, types.ts
  each feature:                  pages/ components/ api.ts types.ts + its composables/utils as plain files
src/assets/ src/lang/            unchanged (global SCSS partials, VeeValidate rules)
```

Rules enforced by ESLint (`no-restricted-imports` in `eslint.config.mjs`):
- A module never imports another module (`@/modules/company/*` from agency, etc.). The only exception is `routes.ts` (the agency route that mounts `modules/worker/register`).
- `src/shared` never imports from `@/modules/*`. A shared component receives the portal-specific function or component as a prop (`CompanyCreateUserModal :save`, `LocationForm :save`, `CommentsCard :create-comment`, `PersonalCard :update-email`).
- Imports are always `@/…` (no relative paths), including SCSS `@import` in `<style>` and asset `src=`/`url()`.
- File names carry no module prefix: `modules/agency/recruiting/clients/pages/Clients.vue`, not `AgencyCompanies.vue`.
- One `api.ts` per feature; never merge them into a portal-wide file. Backend URLs follow `api/{portal}/{menu}/{feature}` (see `.docs/technical/SIGOOK_WEB_API_MAP.md`).
- Recruiting owns Clients and Requests; the `/sales/*` routes reuse those pages and `sales/clients/` only adds the Deals/Interactions tabs.

## Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Component | `PascalCase.vue` | `ProfileForm.vue`, `HeroSection.vue` |
| Pinia store | `camelCase.ts` | `agency.ts`, `security.ts` |

## Patterns

- Pinia for state management (stores hold filters + auth only; no API response caching)
- `<script setup>` + Composition API for new/modernized components
- API layer = plain TS functions in each feature's `api.ts` (and `src/shared/api/*.ts`), no dispatch strings
- Forms: VeeValidate 4 + Yup schemas
- Fully typed — `any` is an ESLint error (`@typescript-eslint/no-explicit-any`); missing types go in the feature's `types.ts` (or `src/shared/*/types.ts`), form shapes for `useStickyForm<…>` may live in the `.vue`
