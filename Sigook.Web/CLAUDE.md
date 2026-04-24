# Sigook.Web — Vue 3 Agency Portal (Main Platform)

Migrated from Vue 2 → Vue 3 in PR #108. Stack: Vite + TypeScript + Pinia + Vue Router 4 + `@ntohq/buefy-next` + VeeValidate 4 + Yup + oidc-client-ts.

## Reference Docs (read these first)

- `.docs/technical/SIGOOK_WEB_API_MAP.md` — every `src/api/*.ts` file → backend endpoint, types, Pinia wiring
- `.docs/technical/SIGOOK_WEB_STRUCTURE.md` — folder layout, routes, views by feature, global plumbing

## Code Navigation

```
Components:      src/components/{domain}/
Pages:           src/pages/
Store (Pinia):   src/store/modules/
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
