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
| `Accounting` | superadmin, admin | Invoices, pay stubs, reports, and user management |

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
- **The unscoped lists are closed to sales.** `GET api/AgencyRequest` and
  `GET api/v2/AgencyCompanyProfile` (plus their `/all` and `/File` variants) require Policy
  `Recruiting`. Otherwise a sales user would just call those and see everything.
- **Details are not scoped.** Policy `Agency` = `AgencyStaff`, so sales reaches the same detail,
  edit, workers, timesheets, applicants and runners endpoints a recruiter reaches. A sales user who
  knows the id of another rep's order can open and edit it. **This is accepted on purpose** — the
  list is the boundary, not the record.

## Sales auto-assignment

**A sales user who creates an order or a client is always assigned to themselves as the sales
representative.** The value is never trusted from the client: it is overwritten server-side.

Any other role leaves the field blank for manual assignment, and can reassign it freely.

This rule lives in `RequestService.CreateRequest` and `AgencyService.CreateCompany` — **not** in a
sales-only controller. It has to, because sales calls the ordinary create endpoints: a rule enforced
only in a separate controller would be bypassed by posting to `api/AgencyRequest` directly.

Updating does **not** re-force the assignment: a sales rep editing an order keeps whatever
representative it already had.

## What stays closed to sales

| Area | Policy | Why |
|------|--------|-----|
| Weekly Board | `Recruiting` | Recruiting planning tool |
| Unscoped order / client lists | `Recruiting` | Would defeat the sales scoping |
| Invoices, pay stubs, reports | `Accounting` | Financial data |
| Create / delete agency users | `Accounting` | User management |

## User creation

Only `admin` and `superadmin` create agency users (`POST api/AgencyPersonnel`, Policy `Accounting`).

The role is chosen at creation time and **validated server-side** against what the caller is allowed
to assign:

| Caller | Can assign |
|--------|-----------|
| `admin` | admin, recruiting, sales |
| `superadmin` | superadmin, admin, recruiting, sales |

`GET api/AgencyPersonnel/Roles` returns that set for the current caller, and the `POST` rejects
anything outside it. Hiding a role from the dropdown is not a defense — the check is the defense.

If the email already belongs to an existing user (adding them to a second agency), the submitted role
is **ignored** and the user keeps their current role: the role is global, not per-agency, so changing
it would also change it in their first agency.
