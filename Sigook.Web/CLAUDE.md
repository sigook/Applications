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
- `<script setup>` + Composition API for new/modernized components. Props are destructured (Vue 3.5 reactive destructure): `const { a, b = 1 } = defineProps<P>()`, no `withDefaults`, no `props.x`; watch them as `watch(() => a, …)` and pass `() => a` to composables. In templates, `v-slot="props"` is the slot scope, not the component props. Template refs use `useTemplateRef<T>('name')`
- API layer = plain TS functions in each feature's `api.ts` (and `src/shared/api/*.ts`), no dispatch strings
- Forms: VeeValidate 4 + Yup schemas
- Unit tests: Vitest + `@vue/test-utils` on `happy-dom` (`vitest.config.ts` merges `vite.config.ts`, so the `@` alias works). Specs live next to the file they test as `<name>.spec.ts`, import `describe/it/expect/vi` explicitly (no globals), and are type-checked by `vue-tsc` and linted like any source file. Composables run inside `effectScope()`; time-based code uses `vi.useFakeTimers()` and the suite runs with `TZ=America/Toronto` (set in `vitest.config.ts`), so local-time dates are deterministic on any machine. Components mount through `mountWithPlugins` (`src/testing/`, Buefy + memory router); `src/testing/setup.ts` unmounts every wrapper and empties `document.body` after each test, so `append-to-body` dropdowns and teleports never leak between tests
- Coverage target is **80%** (statements, branches, functions, lines). It is reached as a ratchet: `coverage.thresholds` in `vitest.config.ts` holds the current floor, `test:ci` fails below it, and every batch of new tests raises it to the new measured value (rounded down) — it never goes down. `types.ts`, `routes.ts`, `main.ts` and `src/testing/` are excluded from coverage CI runs `pnpm run test:ci` and blocks the PR on failure
- Fully typed — `tsconfig.json` has `strict: true` (null guards `if (!x) return` / `?.` / `??` over `!`; payload types mirror backend nullability), `any` is an ESLint error (`@typescript-eslint/no-explicit-any`); missing types go in the feature's `types.ts` (or `src/shared/*/types.ts`), form shapes for `useStickyForm<…>` may live in the `.vue`
- Type-aware ESLint (`parserOptions.project`, not `projectService` — that is ~10× slower on `.vue`): promises are always awaited or explicitly `void` (`void router.push(...)`), `.then` chains are written as `async/await` + `try/catch/finally`, `showAlertError` is sync (never await it), no guards on values the types say are non-null (if API data can be null, make the type `| null` instead), `??` over `||` except for strings. `*.config.ts` is linted without type info
