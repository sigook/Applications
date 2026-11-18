---
name: rate-quote
title: Compare and update role rates from the rate quote
description: compare the role rates of a client (Epsilon, Moca) against the rate quote Excel in Teams and update the roles the user confirms.
---

# Role rates vs the rate quote

The rate quote is an Excel file in the private channel **Management** of the team **Covenant** (`Final - Epsilon Quote Detail <year>.xlsx` today). Each client has one sheet with one row per US location (`ST - City`) and a pay rate + bill rate per category. In Sigook every role of a company has `workerRate` (pay) and `rate` (bill); requests read both from the role, so updating a role updates its requests, including hours not yet paid or invoiced. Pay stubs and invoices already generated keep their rates.

## Steps

1. `sigook_whoami`. Confirm the environment with the user when it is production. The Teams tools need an admin account.
2. `find_teams_files` with team `Covenant`, channel `Management`, name `Quote Detail`. Take the most recent file unless the user names another; tell the user which one and its last modified date.
3. `download_teams_file` with the exact name. Keep the returned `path`.
4. `read_workbook` with the client sheet, `firstRow: 1`, `take: 5` to confirm the header (rows 3 and 4) still matches the layout below. If the columns moved, stop and show the user the header; do not guess.
5. `find_companies` with the client name. Use the USA profile (its locations are in the USA); ask when there is more than one.
6. `compare_role_rates` with the profile, the path and the layout of the sheet.
7. Show the user a table of `different` roles: role, current pay/bill, quote pay/bill, difference, quote location. Then list `ambiguous`, `no-quote` and `unparsed` roles briefly with their reason; they are never updated automatically.
8. Ask which roles to update (all `different`, some, or none). For an `ambiguous` role the user may pick one of its `candidates`.
9. `update_role_rates` with only the confirmed roles, in batches of up to 100, using the quote pay as `workerRate` and the quote bill as `rate`.
10. Report updated, unchanged and failed roles. Never retry a failed role with other values.

## Sheet layouts

`locationColumn` is `A` and `firstRow` is `5` in both sheets. Bill rates in the sheet are not rounded; the tool rounds them to cents as Excel shows them.

| Client | Sheet | general-labor | skilled-labor | forklift | supervisor |
|---|---|---|---|---|---|
| Epsilon Builders (USA) | `QUOTE EPSILON` | pay B, bill D | pay E, bill F | pay G, bill H | pay I, bill J |
| Moca Logistics (USA) | `QUOTE MOCA` | pay B, bill D | pay E, bill G | pay H, bill J | pay K, bill M |

In `QUOTE MOCA` the columns titled `BILL RATE EPSILON` (C, F, I, L) are the Epsilon reference; Moca bills the last `BILL RATE` column of each category (D, G, J, M), which applies the multiplier in J1.

## How roles are matched

Role names are free text, e.g. `General Labor - CA Anaheim`, `Forklift Warehouse - AZ Mesa`, `General Labor CA-Ukiah`.

- Category from the name: `supervisor`, `forklift`, `skilled`, `general labor` / `warehouse labor` / `GL`. Team Lead, Cleaner and Scissor Lift have no column in the quote → `unparsed`.
- State: the uppercase two-letter US state code in the name. Without one, a city that exists in only one state of the quote decides it.
- City: the longest quote city of that state contained in the name (`San Diego Metro` beats `San Diego`), then a close spelling (`fuzzy-city`, check it). `LA` and `NYC` in the quote also match Los Angeles and New York.
- A role with only a state (`General Labor - FL`) is `equal`/`different` when every city of that state has the same rates, otherwise `ambiguous`.

## Rules

- Never update a role the user did not confirm, and never create or rename roles.
- Do not touch Canadian profiles; the quote is USA only.
- `missingInSigook` counts quote rows with no role; list them (`includeMissing: true`) only when the user asks.
