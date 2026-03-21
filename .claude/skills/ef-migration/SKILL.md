---
name: ef-migration
description: Automatically generate EF Core migrations after modifying entity classes, DbContext configurations, or any file that affects the database schema. Use this skill proactively whenever you add, remove, or change properties on entities, modify entity configurations, or alter relationships — even if the user doesn't explicitly ask for a migration. This includes adding new fields, changing column types, adding indexes, modifying foreign keys, or any other schema-affecting change. Do NOT ask the user for confirmation before generating the migration.
---

# EF Core Auto-Migration

When you modify any file that affects the database schema (entities, configurations, DbContext), you must generate an EF Core migration automatically as part of the same workflow. The user should never have to remind you or ask you to do this.

## When to generate a migration

A migration is needed after any of these changes:
- Adding, removing, or renaming a property on an entity class
- Changing a property's type or nullability
- Adding or modifying entity configurations (indexes, constraints, relationships)
- Adding a new entity or removing one
- Changing a DbSet in the DbContext

## How to generate

Run from the repository root (`C:\Src\Covenant\Applications`):

```bash
dotnet ef migrations add <MigrationName> -p Covenant.Api/Covenant.Infrastructure -s Covenant.Api/Covenant.Api --context CovenantContext
```

The `--context CovenantContext` flag is required because the project has multiple DbContexts (`CovenantContext` and `MyKeysContext`).

### Migration naming convention

Use PascalCase descriptive names that summarize the change:
- Adding a field: `Add<FieldName>To<Entity>` (e.g., `AddExternalIdToWorkerProfile`)
- Removing a field: `Remove<FieldName>From<Entity>`
- Adding an entity: `Add<EntityName>`
- Multiple changes: use a summary name (e.g., `UpdateWorkerProfileSchema`)

## After generating

1. Read the generated migration file to verify it contains the expected changes (e.g., `AddColumn`, `DropColumn`, `CreateIndex`)
2. If the migration looks wrong or contains unexpected changes, delete it and investigate before retrying
3. Continue with the rest of the workflow — do not stop to ask the user about the migration
