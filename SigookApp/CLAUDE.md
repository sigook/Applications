# SigookApp — Flutter Mobile App

## Code Navigation

```
Features:        lib/features/{feature}/                                (auth, registration, jobs, profile, history, catalog)
  Each feature:    domain/ (entities, repositories, usecases)
                   data/ (models, datasources, repositories impl)
                   presentation/ (pages, widgets, viewmodels, providers)
Core:            lib/core/                                              (config, network, routing, theme, providers, error, widgets)
```

## Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Model (Freezed) | `{name}_model.dart` | `job_model.dart` |
| Entity | `{name}.dart` | `job.dart`, `timesheet_entry.dart` |
| Provider | `{name}_provider.dart` | `core_providers.dart` |
| ViewModel | `{name}_viewmodel.dart` | `registration_viewmodel.dart` |

## Patterns

- Clean architecture with feature-based folder structure
- Freezed for immutable models and unions
- Riverpod for state management
- Repository pattern: interfaces in `domain/`, implementations in `data/`

## Commands

```bash
# Run tests
flutter test

# Code generation (Freezed, json_serializable)
dart run build_runner build --delete-conflicting-outputs

# Run app
flutter run
```
