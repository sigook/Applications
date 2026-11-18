---
name: import-candidates
title: Import candidates
description: import candidates from a job-board export (folder or zip with CSV + resumes).
---

# Import candidates into Sigook

You drive the `sigook` MCP server. It talks to the Sigook API as the signed-in person; it never decides mappings or loops on its own. You do.

## Before touching data

1. `sigook_whoami`. Confirm the environment with the user when it is production. If `signedIn` is false, call `sigook_login` and tell the user a browser window opened for their Microsoft 365 account.
2. `read_export` with the folder or zip. Keep `take` small (20) on the first call to look at headers and a sample; then page with `skip`/`take`.
3. Pick the source mapping from the headers (`get_instructions` with topic `import-candidates` lists every mapping with the headers that identify it). If none matches, derive the mapping from the headers, show it to the user and get a yes before writing.
4. `list_skills` once and keep the vocabulary. Sources are matched by name inside `import_candidates`.

## Per batch

Work in batches of about 10 rows.

1. `read_documents` with the matched resumes of the batch (from the `documents` list of `read_export`, matched as the mapping says). From each text take: up to 10 skills (prefer names from `list_skills`; certifications such as forklift, WHMIS, OSHA count as skills), street address and postal code when the row lacks them, and a one-line experience summary. Empty text (scanned PDF) or an `error` entry → no extraction, note it in the report. A truncated resume is usually enough; call `read_document` with a larger `maxChars` only when the cut part matters.
2. `import_candidates` with every row of the batch, each one with everything: name, raw phone, email, city + provinceOrState + countryCode, sourceName, residencyStatus, address, postalCode, skills (job-board job title + resume skills), resumePath, requestNumberId, applyComments (screening answers) and note (apply date, status, labels, experience summary). For each row the tool normalizes the phone, skips creation when the phone already exists (and still applies and notes the existing candidate), resolves the city and the request, and caches catalogs. Rows sharing a phone inside the batch are handled one after another, so they do not create duplicates.
3. Read `results`; each entry has its `row` index. `step: "phone"` or `"resume"` → skip the row and report it. `step: "city"` → pick the right one from `alternatives` and send the row again (alone or in the next batch), or without city and report it. `step: "error"` → transient failure; send the row again once. `created` with `applied: false` or `noteAdded: false` → retry only that step with `apply_candidate_to_request` / `add_candidate_note` using the returned `candidateId`; never import a row with a `candidateId` again.

`read_document` and `import_candidate` do the same for a single row. The granular tools (`normalize_phone`, `find_candidates`, `resolve_city`, `find_request`, `create_candidate`, `apply_candidate_to_request`, `add_candidate_note`) stay available for one-off fixes.

## Rules

- Never invent data. Empty cell → omit the field.
- One row = one candidate. Stop and ask when a whole batch fails or after 3 consecutive rows with API errors; something systemic is wrong.
- The API rejects duplicated emails/phones with a 400; report those rows as skipped, do not retry with altered data.
- Do not create requests, companies or workers. Out of scope.

## Report at the end

A table: row, name, result (created / existed / skipped), candidate id, request number, resume (yes/no), error text. Then counts.
