---
name: pre-work-checklist
description: Run through this Agile pre-work checklist before starting ANY coding or development task on the Covenant/Sigook monorepo. Use this skill whenever the user is about to begin a feature, bug fix, refactor, or schema change — before writing code. It covers framing the task in the Agile board (Epic, Sprint, Story Points), reading the right business docs, branching, scoping affected apps, and planning. Trigger on phrases like "let's start working on", "I need to implement", "before I code", "new feature", "new ticket", "pick up a task", or any time work begins without prior planning.
---

# Pre-Work Checklist (Covenant/Sigook · Agile)

Run through these steps before writing any code on the Covenant/Sigook monorepo. This keeps every task starting with the right Agile context, the right branch, and a clear plan — so work doesn't get redone.

## 1. Frame the task in the Agile board

Every piece of work is a **Task** in the Agile Project Management workspace (Notion). Before coding, make sure the Task is properly set up:

- It exists in the **Tasks** database with a clear, one-sentence Name.
- It is linked to an **Epic** and a **Project**.
- It has a **Priority** (High / Medium / Low) and an **Area** (which monorepo app it touches).
- It has a **Story Points** estimate (rough effort — see the scale below).
- It is pulled into the **current Sprint**, and its Status is set to **In progress**.

If any of that is missing, fix it before starting — don't begin untracked work.

### Story Points scale (1-week sprints)

| Points | Meaning |
|--------|---------|
| 1 | Trivial — under an hour |
| 2 | Small — a couple of hours |
| 3 | Half a day |
| 5 | A full day |
| 8 | Most of the sprint — consider splitting |
| 13 | Too big — split into multiple Tasks |

## 2. Understand the task

- Restate the task in one sentence: what changes, and why.
- Identify the type of change: feature, bug fix, refactor, or schema change.
- If anything is ambiguous, ask one clarifying question before continuing.

## 3. Read the relevant business doc

Before touching code, read the matching document from `.docs/` so the work respects existing business rules:

| Area of work | Read first |
|---|---|
| Business model, actors (Agency/Company/Worker/Candidate) | `.docs/business/BUSINESS_MODEL.md` |
| Rates, markup, HST/GST, invoicing | `.docs/business/BILLING_RULES.md` |
| Payroll, CPP, EI, federal/provincial tax | `.docs/business/PAYROLL_RULES.md` |
| Pay stub generation | `.docs/business/PAYSTUB_GENERATION.md` |
| Timesheets, overtime, night, holiday | `.docs/business/TIMESHEET_RULES.md` |
| Request lifecycle / job orders | `.docs/business/REQUEST_STATE_MANAGEMENT.md` |
| Step-by-step workflows | `.docs/business/WORKFLOWS.md` |
| Architecture & stack | `.docs/technical/ARCHITECTURE.md` |
| Data model & relationships | `.docs/technical/ENTITIES_RELATIONSHIPS.md` |

If the task changes a business rule, update the corresponding `.docs/` file as part of the work.

## 4. Identify the affected application(s)

Pin down which part(s) of the monorepo the task touches — this is the **Area** on the Task.

| Application | Stack | Scope |
|---|---|---|
| `Covenant.Api` | .NET 8 | Backend API |
| `SigookApp` | Flutter | Worker mobile app |
| `Sigook.Web` | Vue 3 | Agency web portal |
| `Covenant.Web` | Vue 3 | Marketing website |
| `Covenant.IdentityServer` | .NET 6 | Authentication server |
| `Sigook.CognitiveServices` | .NET | AI/ML services |
| `Sigook.Functions` | Azure Functions (.NET 8) | Background jobs |

Read that project's own `CLAUDE.md` for project-specific patterns. A change crossing apps (e.g. a new API field used by the Flutter app) needs both sides scoped.

## 5. Set up the branch

Feature branches start from `dev`.

```bash
git checkout dev && git pull
git checkout -b feature/<short-description>
```

PRs go to `dev`; `dev` merges to `main` for production.

## 6. Check for schema impact

If the task adds, removes, or changes entities, configurations, or a `DbContext`, an EF Core migration is required — use the `ef-migration` skill. Flag this now so it isn't forgotten at the end.

## 7. Plan before large changes

If the work will touch 3 or more files, write out the plan and review it before executing — list the files, the order of changes, and any migration or doc updates. For an 8-point task, consider splitting it into smaller Tasks first.

## 8. Confirm before you start

Before writing code, you should be able to answer:

- What is the smallest change that satisfies the task?
- Which existing patterns (repository pattern, DI, service layer) does it follow?
- Which `.docs/` file, if any, will need updating?
- How will it be tested?

## After the task — close the Sprint loop

- Run tests before considering the work done: `dotnet test` for .NET projects, `flutter test` for `SigookApp`.
- All code, comments, variable names, and commit messages must be in English.
- Update the relevant `.docs/` file if a business rule changed.
- Do NOT auto-commit — commit only when the user explicitly asks.
- Move the Task to **Done** on the board. If the real effort differed a lot from the estimate, update its **Story Points** so future estimates improve.
- Anything unfinished rolls into the next Sprint at sprint review.

## Build & test reference

```bash
# Covenant.Api (.NET 8)
dotnet build Covenant.Api/Covenant.Api.sln
dotnet test Covenant.Api/Covenant.Tests/Covenant.Tests.csproj

# SigookApp (Flutter)
cd SigookApp && flutter pub get && flutter analyze && flutter test

# Sigook.Web / Covenant.Web (Vue 3, pnpm)
cd Sigook.Web && pnpm install && pnpm run type-check && pnpm run lint

# Covenant.IdentityServer (.NET 6)
dotnet test Covenant.IdentityServer/Covenant.IdentityServer.Tests
```
