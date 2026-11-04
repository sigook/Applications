# Sigook MCP

`Sigook.Mcp/` is a Model Context Protocol server (stdio) that lets an AI harness such as Claude Code operate Sigook through the public API, as the signed-in person. It holds no business rules: every write goes through the same endpoints and validators Sigook.Web uses.

## Stack

| Item | Value |
|---|---|
| Runtime | .NET 10, `ModelContextProtocol` 2.3.1, `Refit` 16 (API client declared as `ISigookApi` interface; bearer token added by `BearerHandler`) |
| Solution | `Sigook.Mcp/Sigook.Mcp.slnx` (independent from `Covenant.Api.slnx`; talks HTTP only) |
| Transport | stdio; logs go to stderr |
| Registration | `.mcp.json` → server `sigook`, `dotnet Sigook.Mcp/Sigook.Mcp/bin/Debug/net10.0/Sigook.Mcp.dll` |
| Packaging | `dotnet pack` produces a global tool, command `sigook-mcp` |

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
2. The loopback listener receives the code; the server exchanges it and stores access + refresh token in `%LOCALAPPDATA%\Sigook.Mcp\tokens\<env>.bin`, DPAPI-protected on Windows.
3. Tokens refresh silently for 30 days. Every call carries the person's role and agency; `AgencyIdFilter` scopes data as usual.
4. `sigook_logout` revokes the refresh token and deletes the file.

Each environment keeps its own token file.

## Tools

| Tool | Endpoint | Purpose |
|---|---|---|
| `sigook_login` / `sigook_logout` / `sigook_whoami` | `connect/*` | Session and environment check |
| `get_import_instructions` | embedded | Workflow + source mapping, auto-detect by headers |
| `read_export` | local | Reads a folder or zip: CSV rows as raw header/value maps + local path of every pdf/doc/docx. Zips extract once to `%LOCALAPPDATA%\Sigook.Mcp\exports\<hash>` |
| `read_document` | local | Text of a pdf/docx/txt (PdfPig, OpenXml) so the harness can extract skills, address and experience from a resume. Scanned PDFs return empty text |
| `normalize_phone` | local | Any NA phone → `### ###-####` |
| `find_request` | `GET agency/recruiting/requests?NumberId=` | Request id by number |
| `resolve_city` | `GET location/province/{CA\|USA}`, `GET location/city/{provinceId}` | City id by name + state; accent/case-insensitive, suggests alternatives |
| `list_sources` / `list_genders` / `list_skills` | `GET catalog/source`, `GET catalog/gender`, `GET catalog/skills` | Catalog ids and the skill vocabulary |
| `find_candidates` | `GET agency/recruiting/candidates?Name=&Phone=` | Duplicate check |
| `create_candidate` | `POST agency/recruiting/candidates` (multipart `data` + resume) | Candidate with optional resume |
| `add_candidate_document` | `POST .../candidates/{id}/documents` | Extra file |
| `add_candidate_note` | `POST .../candidates/{id}/notes` | Free text |
| `apply_candidate_to_request` | `POST agency/recruiting/requests/{id}/applicants` | Candidate as applicant, comments included |

Write tools return `{ success, ... }` or `{ success: false, statusCode, error }` with the API body untouched, so the harness can report validation errors (duplicated email/phone, bad format) row by row.

## Import instructions travel with the server

The server does not know ZipRecruiter or Indeed in code. The workflow and every column mapping are Markdown files embedded in the assembly (`Sigook.Mcp/Instructions/`), so any client that installs the tool gets them without cloning this repo.

| File | Role |
|---|---|
| `Instructions/workflow.md` | Generic step-by-step import workflow |
| `Instructions/<source>.md` | One per job board. Front matter: `name`, `title`, `description`, `detect` (CSV headers that identify the source, comma-separated) |

Exposed three ways, so every MCP client can reach them:

| Mechanism | Id | Notes |
|---|---|---|
| Prompt | `import-candidates(path, source?)` | Claude Code: `/mcp__sigook__import-candidates`. Without `source` the prompt tells the model to auto-detect |
| Resources | `sigook://instructions/import-candidates`, `sigook://mappings/{source}` | Markdown |
| Tool | `get_import_instructions(source?, headers?)` | Fallback for clients without prompts/resources; with `headers` it auto-detects the source |

The server `instructions` sent on `initialize` point the model at `get_import_instructions`.

New job board = add `Instructions/<source>.md` and republish the tool. No C# change.

## Installing (people without the repo)

The tool is published to the Azure Artifacts feed `sigook-tools` by `SigookMcp-CICD` (see PIPELINES.md). Requirements: .NET 10 SDK, Claude Code, an account in the Azure DevOps org `sigook` (same Microsoft 365 login) and the [Azure Artifacts Credential Provider](https://github.com/microsoft/artifacts-credprovider).

```powershell
iex "& { $(irm https://aka.ms/install-artifacts-credprovider.ps1) }"
dotnet tool install -g Sigook.Mcp --add-source https://pkgs.dev.azure.com/sigook/sigook/_packaging/sigook-tools/nuget/v3/index.json --interactive --prerelease
claude mcp add sigook -s user -e SIGOOK_ENV=production -- sigook-mcp
```

`--interactive` opens the browser once for the feed login. Drop `--prerelease` to take only stable versions published from `main`. Update with `dotnet tool update -g Sigook.Mcp` plus the same `--add-source`/`--interactive` flags.

## Running from the repo

```bash
cd Sigook.Mcp && dotnet build Sigook.Mcp.slnx
```

Claude Code starts the server from `.mcp.json` (Debug dll, `SIGOOK_ENV=staging`). To target production for a session, override the variable: `SIGOOK_ENV=production`.

Smoke test without a client: send JSON-RPC lines (`initialize`, `tools/list`, `tools/call`) to the process stdin; `sigook_whoami`, `normalize_phone`, `read_export` and `get_import_instructions` work without signing in.
