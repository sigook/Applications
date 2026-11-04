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

| Column | `import_candidate` parameter | How |
|---|---|---|
| `Name` | `name` | as is |
| `Email` | `email` | as is |
| `Phone Number` | `phone` | raw (`+1 4452510680`); the tool normalizes it |
| `City` + `State` | `city`, `provinceOrState`, `countryCode: "USA"` | as is |
| `Job` | `requestNumberId` | the number after the last ` - ` is the Sigook request NumberId (`... - 2549` → 2549) |
| `Job` | `skills` | the title before ` – ` (e.g. `Warehouse Worker`), plus the resume skills. The API adds the request job title by itself when the candidate is applied |
| `Questions and Answers` | `residencyStatus` + `applyComments` | if an answer to "authorized to work" is Yes → residencyStatus `Authorized to work in the U.S.`. Whole Q&A text goes to `applyComments` |
| `Apply Date`, `Status`, `Labels`, `Rating` | `note` | `ZipRecruiter: applied 10/26/26, status Awaiting Review, labels …` (omit empty parts) |
| `Candidate Overview` | `resumePath` | the `app_contact=<hash>` in the URL is the `<contactHash>` suffix of the PDF file name |
| `Resume` | ignore | protected URL, not downloadable |
| `External Id`, `Notes`, `Deal-Breaker Questions Correct` | ignore | |

`sourceName`: `Zip Recruiter` (with a space); the tool falls back to `Other` when missing.

## Resume matching

From the `Candidate Overview` URL take `app_contact=XXXXXXXX`. Find the document whose `fileName` ends with `-XXXXXXXX.pdf`. Rows without a match get no resume; report them.

## Address

ZipRecruiter gives no street address. Use `"<City>, <State>"` as `address` so the candidate shows a location in Sigook lists.
