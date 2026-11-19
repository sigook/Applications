---
name: release
description: Cut a production release of Sigook (create `release/YYYY.MM.DD` from `main`, watch `Sigook-Production`, queue the manual production pipelines on the release branch) or ship a hotfix to the active release branch. Use whenever the user says they want to release / deploy to production / "hacer el release" / "sacar a producción" / "hotfix a producción", or invokes /release.
---

# Release to production

Branch model (see `.docs/technical/PIPELINES.md` → *Cutting a release*): `main` = staging. `release/YYYY.MM.DD` cut from `main` = production. Release branches are never merged back and never receive PRs; only cherry-picked hotfixes.

Production deploys are outward-facing and hard to reverse. Never create the branch or queue a pipeline without the user's explicit "sí" to the plan in step 3.

## Constants

- GitHub repo: `sigook/Applications` (use `gh`, already logged in)
- Azure DevOps: org `sigook`, project `sigook`, via `az rest --resource 499b84ac-1321-427f-aa17-267ca6975798`
- In Git Bash always `export MSYS_NO_PATHCONV=1 PYTHONIOENCODING=utf-8` and strip `\r` from `-o tsv` output (`tr -d '\r'`)
- Base URL: `https://dev.azure.com/sigook/sigook/_apis`. Single-quote URLs containing `$top` so the shell does not expand it

| App | Pipeline (definition id) | How it reaches production | Paths that mean "has changes" |
|-----|-----|-----|-----|
| Covenant.Api + Sigook.Web | `Sigook-Production` (53) | Automatic on branch creation | `Covenant.Api/**` (minus Functions), `Sigook.Web/**` |
| Covenant.Web | `CovenantGroup-CICD` (43) | Manual run on release branch | `Covenant.Web/**` |
| Sigook.App | `SigookApp-CICD` (38) | Manual run on release branch (Play production + App Store review submission) | `Sigook.App/**` |
| Sigook.Mcp | `SigookMcp-CICD` (52) | Manual run on release branch (stable npm version) | `Sigook.Mcp/**` |
| Sigook.Functions | `SigookFunctions-CICD` (46) | Manual run on release branch | `Covenant.Api/Sigook.Functions/**`, `Covenant.Api/Covenant.Common/**` |
| CognitiveServices | `CognitiveServices-CICD` (47) | Manual run on release branch | `Sigook.CognitiveServices/**` |

Staging pipeline for API + Web on `main`: `Sigook-Staging` (51, file `sigook-staging-pipeline.yml`).

If an id call fails, re-resolve ids: `GET $BASE/build/definitions?api-version=7.1` then `GET .../definitions/{id}` → `process.yamlFilename`.

## Mode A — Cut a release

### 1. Preflight

1. `git fetch origin --prune`
2. `MAIN_SHA=$(git rev-parse origin/main)`
3. Last release branch: `git branch -r --list 'origin/release/*' --sort=-committerdate | head -1`. If none, compare against the previous production state the user names, or list only the last 30 commits.
4. Changes since last release: `git log --oneline <last>..origin/main` and `git diff --name-only <last>..origin/main`. Bucket the files into the app table above.
5. Staging health on `MAIN_SHA`: `GET $BASE/build/builds?definitions=51&branchName=refs/heads/main&$top=5&api-version=7.1` → for the build whose `sourceVersion == MAIN_SHA` (or the newest one) report `status`/`result`. Also check the latest `main` run of each pipeline whose app has changes. A `failed` or `inProgress` run is a blocker to report, not to ignore.
6. Branch name: `release/$(date +%Y.%m.%d)`. If `git ls-remote --heads origin <name>` already returns it, stop and ask (hotfix to it via Mode B, or a different name).
7. Sigook.Mcp only: compare `version` in `Sigook.Mcp/package.json` with the last release branch; if unchanged, warn that the release run will fail (feed already has that version) and offer to bump it on `main` first.

### 2. Present the plan

Show, in Spanish and short:

- Branch name and `MAIN_SHA` (short) + commit subject
- Staging status (green / red / running)
- Commits since last release (max ~15 lines, grouped by app)
- Per app: "se despliega automáticamente" (API+Web) or "propongo correr" / "sin cambios, no corro" for each manual pipeline
- Sigook.App: remind that it publishes to Play production and submits iOS for review

### 3. Confirm

Use AskUserQuestion: confirm the release + multiSelect of the manual pipelines to run (pre-recommend those with changes). Without a "sí", stop.

### 4. Create the branch (on GitHub, from the exact SHA)

```bash
gh api repos/sigook/Applications/git/refs -f ref="refs/heads/$BRANCH" -f sha="$MAIN_SHA"
```

Do not create it locally and push; do not check it out.

### 5. Confirm `Sigook-Production` started

Poll `GET $BASE/build/builds?definitions=53&branchName=refs/heads/$BRANCH&api-version=7.1` (up to ~2 min). If no run appears, queue it:

```bash
cat > "$SCRATCH/run.json" <<EOF
{"resources":{"repositories":{"self":{"refName":"refs/heads/$BRANCH"}}}}
EOF
az rest --method post --resource $R --headers "Content-Type=application/json" \
  --url "$BASE/pipelines/53/runs?api-version=7.1" --body "@<windows-path-to>/run.json"
```

(`--body` needs a Windows-style path like `C:/Users/...`, not `/c/...`.)

### 6. Queue the selected manual pipelines

Same `POST $BASE/pipelines/{id}/runs` with the release `refName`. For `SigookApp-CICD` add `"templateParameters":{"uploadStoreScreenshots":"true"}` only if the user asked for screenshots. Report each run's `id` and web link `https://dev.azure.com/sigook/sigook/_build/results?buildId={id}`.

### 7. Watch

Use Monitor (or a background Bash loop) polling `GET $BASE/build/builds/{id}?api-version=7.1` every ~60 s until every run has `status == completed`. Do not foreground-sleep.

On failure: `GET $BASE/build/builds/{id}/timeline?api-version=7.1` → records with `result == failed` → fetch their `log.url` and show the last relevant lines with cause + proposed fix. Do not re-run without asking.

### 8. Report

One table: app · pipeline · resultado · duración · link. First line = whether production is fully updated.

## Mode B — Hotfix to the active release

1. The fix must already be committed and pushed on `origin/main` (if not, do that first, with the user's OK to commit).
2. Active release = newest `origin/release/*`. Confirm with the user: commits (SHAs + subjects) to cherry-pick and target branch.
3. Worktree in a short path (scratchpad hits Windows MAX_PATH):
   ```bash
   git worktree add C:/Src/Covenant/hotfix-wt origin/$BRANCH -b hotfix-tmp
   cd C:/Src/Covenant/hotfix-wt && git cherry-pick -x <sha...>
   git push origin HEAD:$BRANCH
   cd - && git worktree remove --force C:/Src/Covenant/hotfix-wt && git branch -D hotfix-tmp
   ```
   Conflict → stop, show the conflicting files, ask.
4. The push auto-starts `Sigook-Production` (API + Web). If the fix touches another app's paths, propose re-running that pipeline on the branch (step A.6).
5. Watch and report as in A.7–A.8.

## Rules

- Never force-push, delete or rewrite a `release/*` branch.
- Never merge a release branch into `main`.
- Never run a production pipeline that the user did not select.
- Run everything from the repo root without changing the user's checked-out branch or working tree.
