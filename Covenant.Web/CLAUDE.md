# Covenant.Web — Vue 3 Agency Portal

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
