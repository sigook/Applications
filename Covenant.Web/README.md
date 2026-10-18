# Covenant.Web

Public marketing website of Covenant Group (Vue 3 + Vite + TypeScript + Pinia + Vuetify). Hosted on Azure Static Web Apps; the agency portal is a separate app (`Sigook.Web`).

## Prerequisites

- Node `^20.19.0 || >=22.12.0`
- pnpm (pinned in `package.json` → `packageManager`; enable with `corepack enable`)

## Commands

```bash
pnpm install
pnpm run dev                 # Vite dev server
pnpm run type-check          # vue-tsc
pnpm run lint                # ESLint
pnpm run build:staging       # dist/ for staging (no type-check)
pnpm run build:production    # dist/ for production (no type-check)
```

`build:staging` / `build:production` skip `vue-tsc`; the pipeline runs `type-check` as a separate step.

## Layout

```
src/
├── components/{feature}/   # feature-based components
├── views/                  # routed pages
├── router/                 # Vue Router
├── stores/                 # Pinia
├── composables/
├── services/               # API calls
└── plugins/                # Vuetify and other plugins
```

Conventions: [`CLAUDE.md`](CLAUDE.md). Pipelines and hosting: [`.docs/technical/PIPELINES.md`](../.docs/technical/PIPELINES.md).
