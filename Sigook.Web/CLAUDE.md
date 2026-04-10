# Sigook.Web — Vue 2 Agency Portal (Main Platform)

## Code Navigation

```
Components:      src/components/{domain}/
Pages:           src/pages/
Store:           src/store/modules/
Auth:            src/security/
i18n:            src/lang/
```

## Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Component | `PascalCase.vue` | `ProfileForm.vue`, `HeroSection.vue` |
| Store module (Vuex) | `camelCase.js` | `workers.js`, `jobs.js` |

## Patterns

- Vuex for state management (modules pattern)
- Component-based architecture organized by domain
- i18n support with language files in `src/lang/`
