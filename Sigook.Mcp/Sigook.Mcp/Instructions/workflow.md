# Import candidates into Sigook

You drive the `sigook` MCP server. It talks to the Sigook API as the signed-in person; it never decides mappings or loops on its own. You do.

## Before touching data

1. `sigook_whoami`. Confirm the environment with the user when it is production. If `signedIn` is false, call `sigook_login` and tell the user a browser window opened for their Microsoft 365 account.
2. `read_export` with the folder or zip. Keep `take` small (20) on the first call to look at headers and a sample; then page with `skip`/`take`.
3. Pick the source mapping from the headers (`get_import_instructions` lists every mapping with the headers that identify it). If none matches, derive the mapping from the headers, show it to the user and get a yes before writing.
4. `list_skills` once and keep the vocabulary. Sources are matched by name inside `import_candidate`.

## Per row

1. `read_document` on the matched resume (from the `documents` list of `read_export`, matched as the mapping says). From the text take: up to 10 skills (prefer names from `list_skills`; certifications such as forklift, WHMIS, OSHA count as skills), street address and postal code when the row lacks them, and a one-line experience summary. Empty text (scanned PDF) → no extraction, note it in the report.
2. `import_candidate` with everything in one call: name, raw phone, email, city + provinceOrState + countryCode, sourceName, residencyStatus, address, postalCode, skills (job-board job title + resume skills), resumePath, requestNumberId, applyComments (screening answers) and note (apply date, status, labels, experience summary). The tool normalizes the phone, skips creation when the phone already exists (and still applies and notes the existing candidate), resolves the city and the request, and caches catalogs.
3. Read the result. `step: "phone"` or `"resume"` → skip the row and report it. `step: "city"` → pick the right one from `alternatives` and call again, or call again without city and report it. `created` with `applied: false` or `noteAdded: false` → retry only that step with `apply_candidate_to_request` / `add_candidate_note` using the returned `candidateId`; never import the row again.

The granular tools (`normalize_phone`, `find_candidates`, `resolve_city`, `find_request`, `create_candidate`, `apply_candidate_to_request`, `add_candidate_note`) stay available for one-off fixes.

## Rules

- Never invent data. Empty cell → omit the field.
- One row = one candidate. Stop and ask after 3 consecutive API errors; something systemic is wrong.
- The API rejects duplicated emails/phones with a 400; report those rows as skipped, do not retry with altered data.
- Do not create requests, companies or workers. Out of scope.

## Report at the end

A table: row, name, result (created / existed / skipped), candidate id, request number, resume (yes/no), error text. Then counts.
