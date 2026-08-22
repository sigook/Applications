# Business Model - Covenant/Sigook Platform

Covenant/Sigook is a staffing and recruitment platform for the Canadian market. It connects temporary staffing agencies with companies that need workers and manages the full lifecycle: worker recruitment, job matching, time tracking, payroll (CPP, EI, Federal/Provincial taxes), invoicing, and compliance.

---

## Main Actors

### 1. AGENCY (Staffing Agency)

Intermediary that connects Companies with Workers and operates the platform: recruits and approves workers, manages client companies, creates and staffs requests, approves timesheets, runs payroll, and bills companies.

**Agency types** (`Covenant.Common/Enums/AgencyType.cs`):

| Type | Value | Meaning |
|------|-------|---------|
| `Master` | 1 | Main agency with sub-agencies |
| `Regular` | 2 | Standard independent agency |
| `BusinessPartner` | 3 | Business partner with limited access |

An Agency has physical locations (`AgencyLocation`, with the billing address), internal personnel (`AgencyPersonnel`), and tax registration data (`BusinessNumber`, `HstNumber`). Sub-agencies hang under a `Master` agency via `Agency.AgencyParentId`.

There is no "agency" role anymore: agency staff authenticate with one of the platform roles below (`admin`, `recruiting`, `sales`), scoped to their agency.

### 2. COMPANY (Client)

Agency client that needs temporary or permanent staff. Defines job positions with rates, creates requests, and receives invoices.

- Status pipeline (`Covenant.Common/Enums/CompanyStatus.cs`): `Lead(1) → Potential(2) → Prospect(3) → Quoted(4) → Client(5)`, plus `Blocked(6)` / `Inactive(7)`.
- Structure: `CompanyProfile` (managed by an Agency) with locations (`CompanyProfileLocation`), contacts (`CompanyProfileContactPerson`), internal users (`CompanyUser`), and job positions with rates (`CompanyProfileJobPositionRate`):
  - **WorkerRate** — what the worker is paid.
  - **AgencyRate** — what the agency bills the company (includes markup).

### 3. WORKER

Job seeker using the Flutter mobile app. Entity: `Covenant.Common/Entities/Worker/WorkerProfile.cs`.

A worker **User** has exactly **one** `WorkerProfile`, bound to exactly one agency (`WorkerProfile.AgencyId` is non-nullable, and `WorkerId` has a single-column unique index); companies follow the same pattern (`CompanyProfile.CompanyId` unique). Only agency staff genuinely span agencies, via multiple `AgencyPersonnel` rows (the `agencyIds` claim).

Key flags on `WorkerProfile`:
- `ApprovedToWork` — set by the agency after reviewing documents; gates applying/booking.
- `Dnu` (Do Not Use) — blacklisted by the agency.
- `IsSubcontractor` / `IsContractor` — different tax treatment (subcontractor tax-category overrides zero out deductions).

Profile holds personal data (SIN with file + expiry, IDs), contact/location, professional data (skills, languages, licenses, certificates, experience), availability, and tax data (`TaxCategory` claim codes + province, which drive payroll deductions).

### 4. CANDIDATE

Prospect managed by the agency that does NOT yet have a user account (`Covenant.Common/Entities/Candidate/`). A Candidate exists only in the agency's system; a Worker has an associated User and can use the app. Candidates can be applicants on a Request, but **not** Runners: they must be converted to a Worker first to enter the recruiting pipeline (see `WORKFLOWS.md` section 6).

---

## Authorization Model — 7 Roles

Defined in `Covenant.Api/Covenant.Common/Constants/CovenantConstants.cs` (`CovenantConstants.Role`). Role strings are lowercase; always reference the constants, never literals.

| Role constant | String | Who |
|---------------|--------|-----|
| `Role.SuperAdmin` | `superadmin` | Platform owner |
| `Role.Admin` | `admin` | Agency administrator |
| `Role.Recruiting` | `recruiting` | Agency recruiter |
| `Role.Sales` | `sales` | Agency sales rep (data scoped to own companies/requests) |
| `Role.Company` | `company` | Company main account |
| `Role.CompanyUser` | `company.user` | Company internal user |
| `Role.Worker` | `worker` | Worker (mobile app) |

Composite groups in the same file: `RecruitingAccess`, `SalesAccess`, `AgencyStaff`, `AdminAccess` (superadmin + admin), `AgencyAssignable`, `SuperAdminAssignable`. The old `agency` / `agency.personnel` roles were deleted; there is no "Account Manager" role.

---

## Revenue Model

Agency profit = markup between the two rates on `CompanyProfileJobPositionRate` (copied onto each `Request` as `AgencyRate` / `WorkerRate`):

```
AgencyRate ($25/hr billed to Company) − WorkerRate ($18/hr paid to Worker) = $7/hr markup
```

Agency costs against that markup: employer CPP/EI contributions, insurance, overhead.

**Direct Hiring:** a Request with `WorkerSalary` set (`Covenant.Common/Entities/Request/Request.cs:53`) is a permanent-placement order — the company hires the worker directly for a salary. These orders change billing and attendance behavior (no punch-card billing; excluded from attendance-review notifications — see `WORKFLOWS.md` section 6).

---

## Business Lifecycle

Details and verified endpoints in `.docs/business/WORKFLOWS.md`:

| Phase | Summary | See |
|-------|---------|-----|
| Setup | Agency registers Company + job position rates; Worker registers via app; Agency approves | WORKFLOWS.md §1 |
| Order creation | Company or Agency creates a Request (job order) | WORKFLOWS.md §2 |
| Matching & assignment | Worker applies or Agency books; Request fills automatically | WORKFLOWS.md §2–3 |
| Time tracking | Worker clocks in/out; Agency reviews the punch card | WORKFLOWS.md §4 |
| Payroll & billing | Pay stubs with deductions; invoices with markup | WORKFLOWS.md §5 |
| Recruiting pipeline | Runners: sent to client → interviews → hired | WORKFLOWS.md §6 |

Request state machine (Open/Filled/Cancelled — `RequestStatus` values 1, 3, 4; value 2 intentionally skipped): `.docs/business/REQUEST_STATE_MANAGEMENT.md`.

## Related Documents

- `.docs/business/BILLING_RULES.md` — invoice composition, HST
- `.docs/business/PAYROLL_RULES.md` — deductions and pay stub flow
- `.docs/business/TIMESHEET_RULES.md` — hours breakdown (OT, holiday)
- `.docs/technical/ENTITIES_RELATIONSHIPS.md` — data model
