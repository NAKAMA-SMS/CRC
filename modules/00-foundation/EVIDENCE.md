# Module 00 acceptance evidence

Recorded: 2026-10-02. Status: **implementation delivered; acceptance BLOCKED; NOT FROZEN**.

Scope: the foundation host, explicit migration executable, real PostgreSQL/SQLite adapters, static React shell, safe configuration/API/logging, tests and validation workflow. No Module 01 or later functionality is implemented.

## Validation environment

- Windows 11 Pro 10.0.26200; the validation token was checked and was **not** an elevated administrator token. This was a development machine, not the required clean VM.
- .NET SDK 10.0.401 / published runtime 10.0.12; C# 14; Node 24.16.0; npm 11.21.0; PowerShell 7.6.6; Gitleaks 8.30.1.
- React 19.3.0, TypeScript 6.0.3, Vite 8.3.2, Playwright 1.63.0; Chromium 153.0.8010.12 / revision 1243.
- Real PostgreSQL **17.11** in an isolated Docker container; real file-backed SQLite **3.53.3**. Disposable credentials and database roles were generated for tests. No school data or production credentials were used.
- Git documentation baseline is now `e8e1d48`. The initial inspection saw `28e1c12` plus preparation changes; the preparation commit appeared during the implementation session. This implementation was not committed or pushed by the agent.

## Executed checks

| Command / procedure | Verified result |
|---|---|
| `dotnet tool restore` | Pinned EF tool restored. |
| `dotnet restore CRC.sln --locked-mode` | Passed. The final publication cycle also checks restore afterward and compares all six lockfile hashes. |
| `npm ci` with npm 11.21.0 | Passed. npm 12 was rejected by the exact engine guard; PATH was corrected. |
| `dotnet format CRC.sln --verify-no-changes --no-restore` | Passed. |
| `npm run check` | ESLint, Prettier and strict TypeScript passed. |
| `dotnet build CRC.sln -c Release --no-restore` | Passed with zero warnings/errors. |
| `npm run build --workspace apps/web` | Production bundle passed. |
| `pwsh -File scripts/Test-Postgres.ps1` | **38 passed, 0 failed, 0 skipped** on the final source; wraps `dotnet test ... --no-build --logger trx`. Real PostgreSQL/SQLite, HTTP, configuration, transport rejection, locks, failed upgrades and redaction are included. |
| `pwsh -File scripts/Verify-WindowsPublish.ps1` | Self-contained API and migrator published with locked restore; concurrent migrator processes, repeated apply/status, explicit loopback listener, protected external data path, path with spaces, built browser and abrupt restart exercised. |
| `npm run test:e2e` against published host | **1 passed**; local asset/readiness/header checks, blocked external browser requests, absent test/diagnostic endpoints. |
| `dotnet list CRC.sln package --vulnerable --include-transitive` | No vulnerable packages reported by the configured source. |
| `npm audit` | Zero vulnerabilities reported. |
| `gitleaks git . --redact` and `gitleaks dir . --redact` | No leaks reported; history and uncommitted source both checked. Generated dependencies/tools/build output are explicitly excluded from source scanning. |
| `pwsh -File scripts/Write-DependencyInventory.ps1` | All third-party lock entries have license metadata; review is in THIRD_PARTY_REVIEW.md. |
| Structured published-process log inspection | JSON timestamp, level, category, event ID, diagnostic code and request correlation observed. Secret-marker exclusion is separately asserted by tests. |

`Verify-Foundation.ps1` completed a full local aggregate run. Later targeted reruns cover the final transport/startup/reproducibility fixes; no earlier aggregate output substitutes for those later results.

## Criterion ledger

PASS below means the named scoped behavior has local execution evidence. The cross-platform/clean-environment criteria remain BLOCKED where their required environment has not been exercised.

| Criterion | State | Evidence / remaining condition |
|---|---|---|
| F00-01 | BLOCKED, local checks pass | Locked Windows restore/build/quality and pinned tooling verified; fresh hosted Linux/Windows checkout runs remain required. |
| F00-02 | BLOCKED, profile tests pass | Both real-provider host profiles verified. Browser external requests were blocked, but OS-level host Internet denial must still be exercised in the isolated VM. |
| F00-03 | PASS | Invalid configuration/path/port/provider/secret markers; source precedence; guarded PostgreSQL transport. Production secret provisioning is not claimed. |
| F00-04 | PASS | PostgreSQL 17.11 and SQLite 3.53.3; WAL, FK ON, FULL, busy timeout and integrity assertions. |
| F00-05 | PASS | Baseline/repeat/status, missing/unknown history, startup refusal; no domain tables in baseline. |
| F00-06 | PASS | Separate real PG migration/runtime roles; DDL denial; concurrent advisory/file locks and published migrator processes; failing test-only upgrades roll back DDL/history. |
| F00-07 | PASS | Real rollback/FK/unique tests, bounded SQLite contention, reopening committed fixtures; published process restart preserves committed baseline/history. |
| F00-08 | PASS | Exact health envelopes; schema divergence, actual PG connection denial and an actual locked history table; bounded readiness failure with liveness available. |
| F00-09 | PASS | Safe correlated errors, malformed JSON, missing routes, method/media/size/exception cases; no SPA fallback. |
| F00-10 | PASS | Default denial without a dummy scheme, explicit public routes, same-origin behavior, absent published test/explorer endpoints. |
| F00-11 | PASS | Captured-log secret tests plus published JSON inspection; no raw exception messages or request values. |
| F00-12 | PASS | Published Chromium smoke with all non-origin browser requests denied. This is not host-level network isolation. |
| F00-13 | BLOCKED, local publication smoke passes | No clean VM with SDK/runtime/Node absent has been supplied. Manual graceful stop/restart and host-level Internet denial remain required. Abrupt restart is not graceful-shutdown evidence. |
| F00-14 | PASS for foundation scope | Windows service-compatible host runs in console mode under the observed non-admin token and listens only on configured loopback. Actual SCM installation/recovery remains 05. |
| F00-15 | BLOCKED | SHA-pinned, read-only Linux/Windows workflow exists; no hosted run links/results. Source has not been pushed by the agent. |
| F00-16 | PASS for current lock graph | Advisory/secret results and package-license review above. Re-run when dependencies change; production distribution notices remain a release obligation. |
| F00-17 | BLOCKED | OpenAPI 3.1 artifact and local contracts/docs checked; cannot close the overall handoff/freeze while the environment gates above are open. |

## Artifact locations

All raw artifacts are retained locally under ignored `artifacts/evidence/`, rather than committing generated binaries/logs:

- `final-verification.txt`: final build/test/publication and post-publication locked-restore transcript.
- `tests/*.trx`: each run, including failures and final passing runs. Use the latest run, not a stale report.
- `browser.xml`, `openapi.json`, `sqlite-version.txt`, `postgres-version.txt`.
- `windows-smoke.txt`, `windows-artifact-sha256.csv`, `windows-*.stdout.jsonl`, `windows-*.stderr.txt`, `migrator-*.stdout.txt`/`*.stderr.txt`.
- `nuget-audit.json`, `npm-audit.json`, `gitleaks-history.json`, `gitleaks-worktree.json`, `dependency-licenses.csv`.

Hosted CI is configured to upload its evidence. Preserve the reviewed run artifacts with the accepted commit before freeze. Local evidence does not establish a future commit's results.

## Defects encountered and disposition

Fixed: cancellation analyzer errors; test-host bootstrap/controller discovery; pooled SQLite test handle during cleanup; implicit RID mutation of lockfiles; over-broad cold-start deadline; unverified PostgreSQL transport default. No tests or security controls were suppressed. ADR-0002 records the runtime graph, guarded transport and startup-budget clarifications; the selected framework/provider/version lines are unchanged.

Environment interruptions: browser downloader timed out (the identical official archive was downloaded successfully with curl); a restore exhausted disk space (only task-owned downloaded archives were removed). These are recorded as interruptions, not passing tests. No unresolved local failure is waived by this report.

After the successful final code verification, a post-documentation formatter repeat could not start: the temporary .NET SDK executable/directory was no longer available. Its disappearance was not caused by a recorded implementation command; the cause is unresolved. This last attempt is BLOCKED, not passed. The application source is unchanged since the successful full cycle; the subsequent `.gitattributes` addition enforces the existing LF formatting policy across checkouts. Reprovision the pinned development toolchain before another local build. The retained successful transcript and test reports remain evidence of their actual executions, not proof that tools are currently installed.

## Exact next action

Review the implementation and run the committed source through both hosted jobs; supply the isolated Windows VM and execute F00-13's manual procedure, including OS-level network denial for F00-02. Attach the actual evidence, resolve any failures, and only then freeze Module 00. Module 01 remains prohibited until that freeze and explicit closure of D01/D02 in PROJECT_STATE.
