# Sigook.Web — Vue 3 Agency Portal (Main Platform)

Migrated from Vue 2 → Vue 3 in PR #108. Stack: Vite + TypeScript + Pinia + Vue Router 4 + `buefy` 3.x + VeeValidate 4 + Yup + oidc-client-ts.

## Buefy 3

UI library is `buefy` 3.x (official Vue 3 release, successor of `@ntohq/buefy-next`). It bundles Bulma 1.x.

- CSS entry: `buefy/dist/css/buefy.css` (imported in `src/main.ts`)
- `index.html` pins `<html data-theme="light">` — Bulma 1 auto-switches to dark on `prefers-color-scheme: dark`, which this app is not designed for
- Component docs: use the `buefy` MCP server (`buefy_search`, `buefy_get_component`) instead of guessing props

## Reference Docs (read these first)

- `.docs/technical/SIGOOK_WEB_API_MAP.md` — every `src/api/*.ts` file → backend endpoint, types, Pinia wiring
- `.docs/technical/SIGOOK_WEB_STRUCTURE.md` — folder layout, routes, views by feature, global plumbing

## Code Navigation

```
Components:      src/components/{domain}/
Pages:           src/pages/
Store (Pinia):   src/stores/          (flat: agency.ts, company.ts, worker.ts, security.ts, app.ts)
Auth:            src/security/
i18n:            src/lang/
Composables:     src/composables/
```

## Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Component | `PascalCase.vue` | `ProfileForm.vue`, `HeroSection.vue` |
| Pinia store | `camelCase.ts` | `agency.ts`, `security.ts` |

## Patterns

- Pinia for state management (stores hold filters + auth only; no API response caching)
- `<script setup>` + Composition API for new/modernized components
- API layer = plain TS functions in `src/api/*.ts` (no dispatch strings)
- Forms: VeeValidate 4 + Yup schemas
- Fully typed — no `any` in API contracts or store state
