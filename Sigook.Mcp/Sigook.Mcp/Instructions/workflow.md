# Import candidates into Sigook

You drive the `sigook` MCP server. It talks to the Sigook API as the signed-in person; it never decides mappings or loops on its own. You do.

## Before touching data

1. `sigook_whoami`. Confirm the environment with the user when it is production. If `signedIn` is false, call `sigook_login` and tell the user a browser window opened for their Microsoft 365 account.
2. `read_export` with the folder or zip. Keep `take` small (20) on the first call to look at headers and a sample; then page with `skip`/`take`.
3. Pick the source mapping from the headers (`get_import_instructions` lists every mapping with the headers that identify it). If none matches, derive the mapping from the headers, show it to the user and get a yes before writing.
4. `list_sources` once and keep the id of the source (fallback `Other`). `list_skills` once and keep the vocabulary.

## Per row

1. `normalize_phone` on the raw phone. If it fails, skip the row and report it.
2. `find_candidates` by phone. If one exists, do not create; reuse its id only to apply it to the request when the mapping says so.
3. `resolve_city` (city, state, country). Cache results per city+state; call once per distinct pair. If not found, create the candidate without a city and report it.
4. `find_request` when the row names a request number. Cache per number.
5. `read_document` on the matched resume (from the `documents` list of `read_export`, matched as the mapping says). From the text take: up to 10 skills (prefer names from `list_skills`; certifications such as forklift, WHMIS, OSHA count as skills), street address and postal code when the row lacks them, and a one-line experience summary. Empty text (scanned PDF) → no extraction, note it in the report.
6. `create_candidate` with: name, phone, email, address, postalCode, cityId/cityName, sourceId, residencyStatus, skills (job-board job title + resume skills; the API adds the request job title when the candidate is applied), `resumePath`.
7. `apply_candidate_to_request` when a request was found. Put screening answers in `comments`.
8. `add_candidate_note` for data without a Sigook field (apply date, status on the board, labels, experience summary from the resume).

## Rules

- Never invent data. Empty cell → omit the field.
- One row = one candidate. Stop and ask after 3 consecutive API errors; something systemic is wrong.
- The API rejects duplicated emails/phones with a 400; report those rows as skipped, do not retry with altered data.
- Do not create requests, companies or workers. Out of scope.

## Report at the end

A table: row, name, result (created / existed / skipped), candidate id, request number, resume (yes/no), error text. Then counts.
