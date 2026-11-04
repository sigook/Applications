---
name: ziprecruiter
title: ZipRecruiter
description: Candidate-Bulk-Export-*.zip (CSV) + ZipRecruiter-Resumes-*.zip (PDFs) downloaded from the ZipRecruiter employer portal.
detect: Candidate Overview, Deal-Breaker Questions Correct, Questions and Answers
---

# ZipRecruiter export → Sigook

The export is two zips in one folder. `read_export` on the folder reads both.

- `Candidate-Bulk-Export-*.zip` → one CSV.
- `ZipRecruiter-Resumes-*.zip` → `<JobSlug>-<jobHash>/<First>_<Last>-<contactHash>.pdf`.

## CSV columns

| Column | Sigook | How |
|---|---|---|
| `Name` | name | as is |
| `Email` | email | as is, lowercase |
| `Phone Number` | phone | `normalize_phone` (`+1 4452510680` → `445 251-0680`) |
| `City` + `State` | cityId | `resolve_city(city, state, "USA")` |
| `Job` | request | the number after the last ` - ` is the Sigook request NumberId (`... - 2549` → 2549). `find_request(2549)` |
| `Job` | skills | the title before ` – ` (e.g. `Warehouse Worker`). The API adds the request job title by itself when the candidate is applied |
| `Questions and Answers` | residencyStatus + comments | if an answer to "authorized to work" is Yes → residencyStatus `Authorized to work in the U.S.`. Whole Q&A text goes to `comments` of `apply_candidate_to_request` |
| `Apply Date`, `Status`, `Labels`, `Rating` | note | one `add_candidate_note`: `ZipRecruiter: applied 10/26/26, status Awaiting Review, labels …` (omit empty parts) |
| `Candidate Overview` | resume match | the `app_contact=<hash>` in the URL is the `<contactHash>` suffix of the PDF file name |
| `Resume` | ignore | protected URL, not downloadable |
| `External Id`, `Notes`, `Deal-Breaker Questions Correct` | ignore | |

Source: `Zip Recruiter` (with a space) from `list_sources`; if missing, `Other`.

## Resume matching

From the `Candidate Overview` URL take `app_contact=XXXXXXXX`. Find the document whose `fileName` ends with `-XXXXXXXX.pdf`. Rows without a match get no resume; report them.

## Address

ZipRecruiter gives no street address. Use `"<City>, <State>"` as `address` so the candidate shows a location in Sigook lists.
