# Roles & Permissions

Defines the roles of the platform, what each one can reach, and the rules that scope the data they see.

Roles live in IdentityServer (one role per user) and are mirrored in:

| Layer | Source of truth |
|-------|-----------------|
| Backend | `Covenant.Common/Constants/CovenantConstants.cs` → `Role` |
| Policies | `Covenant.Api/Authorization/PolicyConfiguration.cs` |
| Frontend | `Sigook.Web/src/security/roles.ts` |

## The 7 roles

| Role | Who | Scope |
|------|-----|-------|
| `superadmin` | Platform owner | Everything, across agencies |
| `admin` | Agency owner / manager | Everything of their agency, including Accounting |
| `recruiting` | Recruiter | Recruiting module of their agency |
| `sales` | Sales representative | Sales module of their agency, scoped to their own clients and orders |
| `company` | Client account | Their own company |
| `company.user` | Client's employee | Their own company, limited |
| `worker` | Worker (mobile app) | Their own jobs |

An **agency owner** gets `admin`. There is no `agency` role: what resolves a user's agency is their
`AgencyPersonnel` row, not a role.

## Role groups

| Group | Roles | Meaning |
|-------|-------|---------|
| `RecruitingAccess` | superadmin, admin, recruiting | Recruiting module |
| `AgencyStaff` | RecruitingAccess + sales | Anyone who belongs to an agency |
| `SalesAccess` | superadmin, admin, sales | Sales module |
| `AdminAccess` | superadmin, admin | Invoices, pay stubs, reports, and user management (policy name: `Admin`) |

`AgencyStaff` is also the **identity** group: those are the users for whom `AgencyIdFilter` and
`AgencyPersonnelIdFilter` inject the `agencyId` / `agencyPersonnelId` claims.

## Core rule: sales is scoped by the list, not by the detail

**A sales user only sees their own orders and clients in the lists. Once inside a record, they behave
exactly like a recruiter.**

The reasoning: if a sales rep is looking at an order, it is because it is assigned to them or they
created it — so they should be able to do the same things a recruiter can do with it.

Concretely:

- **Lists are scoped.** `GET api/agency/sales/requests` and `GET api/agency/sales/companyprofiles`
  filter by the caller's `AgencyPersonnelId` (`RequestComission.AgencyPersonnelId` for orders,
  `CompanyProfile.SalesRepresentativeId` for clients). Admin and superadmin hit the same endpoints
  **unscoped** — the scoping is per-caller, not per-endpoint.
- **The unscoped lists are closed to sales.** `GET api/agency/recruiting/requests` and
  `GET api/agency/recruiting/companyprofiles` (plus `/all` and `/File` on requests, and `/File` and
  `/FileWithDetails` on companyprofiles — there is no companyprofiles `/all`) require Policy
  `Recruiting`. Otherwise a sales user would just call those and see everything.
  **One deliberate exception:** `GET api/agency/companyprofiles/companies-list` (Policy `Agency`) is a
  name-only typeahead, capped at 50 rows, that is **not** sales-scoped — it feeds the client pickers of
  the sales dashboard's deal and interaction modals, so a sales user can log activity against any
  company of the agency, not only their own. Accepted on purpose; the scoped lists it sits beside
  (`api/agency/sales/companyprofiles`) still drive every actual client listing.
- **Details are not scoped.** Policy `Agency` = `AgencyStaff`, so sales reaches the same detail,
  edit, workers, timesheets and applicants endpoints a recruiter reaches. A sales user who
  knows the id of another rep's order can open and edit it. **This is accepted on purpose** — the
  list is the boundary, not the record.

### Exception: deals & interactions are owner-scoped end-to-end

The sales module's deals and company interactions (`api/agency/sales/deals`,
`api/agency/sales/companyinteractions`) do **not** follow the list-is-the-boundary rule: a sales
user lists, updates and deletes only the records they own, and `OwnerId` is overwritten server-side
on create. Admin and superadmin hit the same endpoints unscoped. Controllers:
`Covenant.Api/Covenant.Api/Controllers/Sigook/Agency/Sales/{DealsController,CompanyInteractionsController}.cs`
(Policy `Sales`). Business meaning of deals and interactions: `SALES_MODULE.md`; entities: `.docs/technical/ENTITIES_RELATIONSHIPS.md`.

## Sales auto-assignment

**A sales user who creates an order or a client is always assigned to themselves as the sales
representative.** The value is never trusted from the client: it is overwritten server-side.

Any other role leaves the field blank for manual assignment, and can reassign it freely.

This rule lives in `RequestService.CreateRequest` and `AgencyService.CreateCompany` — **not** in a
sales-only controller. It has to, because sales calls the ordinary create endpoints: a rule enforced
only in a separate controller would be bypassed by posting to `api/agency/requests` directly.

Updating does **not** re-force the assignment: a sales rep editing an order keeps whatever
representative it already had.

## What stays closed to sales

| Area | Policy | Why |
|------|--------|-----|
| Weekly Board | `Recruiting` | Recruiting planning tool |
| Unscoped order / client lists | `Recruiting` | Would defeat the sales scoping |
| Runners | `Recruiting` | Recruiting pipeline (`RunnersController.cs:17`) |
| Invoices, pay stubs, reports | `Admin` | Financial data |
| Create / edit / delete agency users | `Admin` | User management |
| Bulk recruiter assignment | `Admin` | `PUT api/agency/requests/bulk-recruiters` |

> `PayStubsController`, `InvoicesController` and `ReportsController` enforce
> `[Authorize(Policy = PolicyConfiguration.Admin)]`. `DeductionsController` keeps a bare
> `[Authorize]` on purpose — it is called by the `CraTableUploaded` Azure Function with its own
> credentials, not by an agency user. Note there is no global `FallbackPolicy`: a controller
> without `[Authorize]` is anonymous, so new accounting controllers must declare their policy
> explicitly.

## Other policies

Besides `Agency`, `Recruiting`, `Sales` and `Admin` (`PolicyConfiguration.cs`):

- `SuperAdmin` — superadmin only.
- `Company` — `company` or `company.user`.
- `Worker` — `worker` only.

Every policy requires a role: there is no authenticated-only policy and no cross-actor policy. An
endpoint two actors need is exposed once per actor (`Controllers/Sigook/Agency/`, `Controllers/Sigook/Company/`,
`WorkerModule/`), each under its own policy, and the shared logic lives in a `Covenant.Core.BL`
service. The request shift is the reference case: `GET api/agency/requests/{requestId}/Shift`
(policy `Agency`) and `GET api/company/requests/{requestId}/Shift` (policy `Company`) both delegate to
`IRequestService.GetRequestShift`.

On the frontend, route guards live in `Sigook.Web/src/router/routesAgency.ts` and the
`useAdmin` / `useRecruitingAccess` composables — UI mirrors, not defenses.

## User creation

Only `admin` and `superadmin` create agency users (`POST api/agency/personnel`, Policy `Admin`).

The role is chosen at creation time and **validated server-side** against what the caller is allowed
to assign:

| Caller | Can assign |
|--------|-----------|
| `admin` | admin, recruiting, sales |
| `superadmin` | superadmin, admin, recruiting, sales |

`GET api/agency/personnel/Roles` returns that set for the current caller, and the `POST` rejects
anything outside it. Hiding a role from the dropdown is not a defense — the check is the defense.

If the email already belongs to an existing user (adding them to a second agency), the submitted role
is **ignored** and the user keeps their current role: the role is global, not per-agency, so changing
it would also change it in their first agency.

Only existing **agency** users can be added to a second agency: if the email belongs to a worker or
company user, `AgencyService` rejects it with `EmailAlreadyTaken` (`AgencyService.cs:625-628`).

## User editing

`PUT api/agency/personnel/{id}` (Policy `Admin`) changes an agency user's **name, email and role**.
It is the same `AgencyPersonnelModel` the `POST` takes, validated by `AgencyPersonnelModelValidator`.

- The role must be inside the caller's assignable set — same check as creation.
- **Nobody can change their own role.** A caller editing their own record may change the name and the
  email, but a role change is rejected server-side; the UI also disables the select. Without this an
  admin could demote themselves and lose access to user management.
- **The role is global**, so changing it changes it in *every* agency the user belongs to. This is
  allowed on purpose, and the modal warns about it.
- The new role only reaches the user's token on their **next sign-in**: `CustomProfileService` emits
  roles from the session principal instead of re-reading them.
- The email moves `Email` + `UserName` in IdentityServer plus the local `Users` row, reusing
  `IIdentityServerService.UpdateUserEmail` — which rejects an email that already belongs to another
  user with `EmailAlreadyTaken`. `EmailConfirmed` is untouched, so the user keeps their password and
  signs in with the new address.

The role is not stored in the Covenant.Api database: `GET api/agency/personnel` fills it by asking
IdentityServer (`POST /UserAdministration/UsersRoles`, ids in the body). If that call fails, the list
is still returned with a null role instead of failing the request.
