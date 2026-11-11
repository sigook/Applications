# Covenant.Web — Vue 3 Marketing Website (Public-Facing)

## Code Navigation

```
Components:      src/components/{feature}/
Views:           src/views/
Stores:          src/stores/
Composables:     src/composables/
Services:        src/services/
```

## Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Component | `PascalCase.vue` | `ProfileForm.vue`, `HeroSection.vue` |
| Store (Pinia) | `camelCase.ts` | `workers.ts`, `jobs.ts` |

## Patterns

- Pinia for state management
- Composition API with composables for reusable logic
- Service layer for API calls in `src/services/`
- Feature-based component organization
- `<script setup>` + Composition API. Props are destructured (Vue 3.5 reactive destructure): `const { a, b = 1 } = defineProps<P>()`, no `withDefaults`, no `props.x`; watch them as `watch(() => a, …)` and pass `() => a` to composables. Template refs use `useTemplateRef<T>('name')`
- Forms: VeeValidate 4 + Yup schemas
- Unit tests: Vitest + `@vue/test-utils` on `happy-dom` (`vitest.config.ts` merges `vite.config.ts`, so the `@` alias works; it sets `VITE_API_BASE_URL` so `services/api.ts` loads). Specs live next to the file they test as `<name>.spec.ts`, import `describe/it/expect/vi` explicitly (no globals), and are type-checked by `vue-tsc` and linted like any source file. Services are tested by spying on the `api` axios instance (`vi.spyOn(api, 'get')`) or swapping `api.defaults.adapter`; composables that need a component context (VeeValidate `useForm`) run inside a mounted `defineComponent`. The suite runs with `TZ=America/Toronto`. Components mount through `mountWithPlugins` (`src/testing/`, Vuetify + memory router); `src/testing/setup.ts` unmounts every wrapper and empties `document.body` after each test
- Coverage target is **80%** (statements, branches, functions, lines). It is reached as a ratchet: `coverage.thresholds` in `vitest.config.ts` holds the current floor, `test:ci` fails below it, and every batch of new tests raises it to the new measured value (rounded down) — it never goes down. `*.types.ts`, `main.ts` and `src/testing/` are excluded from coverage. CI runs `pnpm run test:ci` and blocks the pipeline on failure
- Fully typed — `tsconfig.json` has `strict: true` (null guards `if (!x) return` / `?.` / `??` over `!`; payload types mirror backend nullability), `any` is an ESLint error (`@typescript-eslint/no-explicit-any`); missing types go in `src/services/types/*.types.ts`
- Type-aware ESLint (`parserOptions.project`, not `projectService`): promises are always awaited or explicitly `void` (`void router.push(...)`), `.then` chains are written as `async/await` + `try/catch/finally`, catch variables are `unknown` (narrow with `instanceof`), no guards on values the types say are non-null (if API data can be null, make the type `| null` instead), `??` over `||` except for strings. `*.config.ts` is linted without type info
