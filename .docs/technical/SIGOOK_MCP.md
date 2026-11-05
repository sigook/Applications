# Sigook MCP

`Sigook.Mcp/` is a Model Context Protocol server (stdio) that lets an AI harness such as Claude Code operate Sigook through the public API, as the signed-in person. It holds no business rules: every write goes through the same endpoints and validators Sigook.Web uses.

## Stack

| Item | Value |
|---|---|
| Runtime | Node 22+, TypeScript, `@modelcontextprotocol/sdk` + `zod` (API client in `src/api/sigookApi.ts`, native `fetch` with the bearer token) |
| Project | `Sigook.Mcp/package.json`, package `@sigook/mcp` (independent from the other applications; talks HTTP only) |
| Transport | stdio; logs go to stderr |
| Registration | `.mcp.json` → server `sigook`, `node Sigook.Mcp/dist/index.js` |
| Packaging | npm package with `bin`, command `sigook-mcp`; `dist/` and `instructions/` are the only shipped folders |

## Environments

Selected with `SIGOOK_ENV` (default `staging`).

| `SIGOOK_ENV` | API | Authority |
|---|---|---|
| `local` | `https://localhost:44307` | same (invalid certificates accepted) |
| `staging` | `https://staging.api.sigook.ca` | `https://staging.accounts.sigook.ca` |
| `production` | `https://api.sigook.com` | `https://accounts.sigook.com` |

`SIGOOK_API_URL`, `SIGOOK_AUTHORITY` and `SIGOOK_INSECURE=1` override the defaults.

## Authentication

Authorization code + PKCE against OpenIddict, client `sigook.mcp` (public, native, redirect `http://localhost:53421/callback`). The client is seeded by `OpenIddictSeeder.EnsureMcpClient` in every environment.

1. `sigook_login` opens the browser on `/connect/authorize`; the person signs in with their Microsoft 365 account (same `ExternalController` flow as Sigook.Web).
2. The loopback listener receives the code; the server exchanges it and stores access + refresh token in `%LOCALAPPDATA%\Sigook.Mcp\tokens\<env>.bin`, encrypted with AES-256-GCM. The key lives in the OS credential store (`@napi-rs/keyring`: Windows Credential Manager, macOS Keychain, Linux Secret Service). Without a credential store (non-Windows only) the file stays in clear text with mode 600.
3. Tokens refresh silently for 30 days. Every call carries the person's role and agency; `AgencyIdFilter` scopes data as usual.
4. `sigook_logout` revokes the refresh token and deletes the file.

Each environment keeps its own token file.

## Tools

| Tool | Endpoint | Purpose |
|---|---|---|
| `sigook_login` / `sigook_logout` / `sigook_whoami` | `connect/*` | Session and environment check |
| `get_import_instructions` | embedded | Workflow + source mapping, auto-detect by headers |
| `read_export` | local | Reads a folder or zip: CSV rows as raw header/value maps + local path of every pdf/doc/docx. Zips extract once to `%LOCALAPPDATA%\Sigook.Mcp\exports\<hash>` |
| `read_document` | local | Text of a pdf/docx/txt (`unpdf`, `mammoth`) so the harness can extract skills, address and experience from a resume. Scanned PDFs return empty text |
| `normalize_phone` | local | Any NA phone → `### ###-####` |
| `find_request` | `GET agency/recruiting/requests?NumberId=` | Request id by number |
| `resolve_city` | `GET location/province/{CA\|USA}`, `GET location/city/{provinceId}` | City id by name + state; accent/case-insensitive, suggests alternatives |
| `list_sources` / `list_genders` / `list_skills` | `GET catalog/source`, `GET catalog/gender`, `GET catalog/skills` | Catalog ids and the skill vocabulary |
| `find_candidates` | `GET agency/recruiting/candidates?Name=&Phone=` | Duplicate check |
| `import_candidate` | all of the below, in one call | One row = one call: normalizes the phone, duplicate check by phone, city + source + request lookups (cached in `CatalogCache`), create with resume, apply to request, note. Reports each step so the harness retries only what failed |
| `create_candidate` | `POST agency/recruiting/candidates` (multipart `data` + resume) | Candidate with optional resume |
| `add_candidate_document` | `POST .../candidates/{id}/documents` | Extra file |
| `add_candidate_note` | `POST .../candidates/{id}/notes` | Free text |
| `apply_candidate_to_request` | `POST agency/recruiting/requests/{id}/applicants` | Candidate as applicant, comments included |

`import_candidate` writes nothing when the phone is invalid, the city is unknown or the resume path is missing (`step` says which). Write tools return `{ success, ... }` or `{ success: false, statusCode, error }` with the API body untouched, so the harness can report validation errors (duplicated email/phone, bad format) row by row.

## Import instructions travel with the server

The server does not know ZipRecruiter or Indeed in code. The workflow and every column mapping are Markdown files shipped in the package (`Sigook.Mcp/instructions/`), so any client that installs it gets them without cloning this repo.

| File | Role |
|---|---|
| `instructions/workflow.md` | Generic step-by-step import workflow |
| `instructions/<source>.md` | One per job board. Front matter: `name`, `title`, `description`, `detect` (CSV headers that identify the source, comma-separated) |

Exposed three ways, so every MCP client can reach them:

| Mechanism | Id | Notes |
|---|---|---|
| Prompt | `import-candidates(path, source?)` | Claude Code: `/mcp__sigook__import-candidates`. Without `source` the prompt tells the model to auto-detect |
| Resources | `sigook://instructions/import-candidates`, `sigook://mappings/{source}` | Markdown |
| Tool | `get_import_instructions(source?, headers?)` | Fallback for clients without prompts/resources; with `headers` it auto-detects the source |

The server `instructions` sent on `initialize` point the model at `get_import_instructions`.

New job board = add `instructions/<source>.md` and republish the package. No code change.

## Installing (people without the repo)

The package is published to the Azure Artifacts feed `sigook-tools` by `SigookMcp-CICD` (see PIPELINES.md). Only `@sigook/*` comes from that feed; dependencies come from npmjs. Requirements: Node 22+, Claude Code and an account in the Azure DevOps org `sigook` (same Microsoft 365 login).

Claude Web cannot use this server: it only accepts remote connectors, and this one runs on the person's machine over stdio.

```powershell
npm config set "@sigook:registry" https://pkgs.dev.azure.com/sigook/sigook/_packaging/sigook-tools/npm/registry/
npm install -g vsts-npm-auth
vsts-npm-auth -config "$HOME\.npmrc"
npm install -g @sigook/mcp@dev
claude mcp add sigook -s user -e SIGOOK_ENV=production -- sigook-mcp
```

`vsts-npm-auth` opens the browser once for the feed login and writes a token to the user `.npmrc`. The token is only needed to install or update; the installed server keeps working when it expires. Use `@sigook/mcp` without `@dev` to take only stable versions published from `main`. Update by running the same `npm install -g`, after `vsts-npm-auth -config "$HOME\.npmrc" -F` if the token expired.

## Running from the repo

```bash
cd Sigook.Mcp && npm ci && npm test
```

`npm test` compiles to `dist/` and runs the `*.test.ts` files. `npm run build` only compiles; restart the server afterwards.

Claude Code starts the server from `.mcp.json` (`dist/index.js`, `SIGOOK_ENV=staging`). To target production for a session, override the variable: `SIGOOK_ENV=production`.

Smoke test without a client: send JSON-RPC lines (`initialize`, `tools/list`, `tools/call`) to the process stdin; `sigook_whoami`, `normalize_phone`, `read_export` and `get_import_instructions` work without signing in.
