# Module 00 acceptance evidence

Recorded: 2026-10-02. Status: **implementation complete; remaining validation DEFERRED under ADR-0001; NOT FROZEN**.

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

## Hosted acceptance verification — 2026-10-02

Reviewed the existing push-triggered [run 36969033339, attempt 1](https://github.com/NAKAMA-SMS/CRC/actions/runs/36969033339) for exact commit `8adf1ab06ec457578a09925ddf8d34685f70c8f8`. It completed successfully at 05:31:56 UTC. This closure pass retrieved job logs and downloaded both evidence archives; it did not rerun or claim to have triggered those checks. The worktree was clean at inspection.

| Hosted job | Actual results |
|---|---|
| [Ubuntu 24.04 / 110718923859](https://github.com/NAKAMA-SMS/CRC/actions/runs/36969033339/job/110718923859) | Fresh checkout, exact tool setup, locked restores, formatting/lint/type checks and Release builds passed (zero compiler warnings/errors). TRX: 38 executed/passed, zero failed/not-executed. Real PostgreSQL 17.11 and SQLite 3.53.3. Browser JUnit: 1 test, zero failures/errors/skips. |
| [Windows Server 2022 / 110718923660](https://github.com/NAKAMA-SMS/CRC/actions/runs/36969033339/job/110718923660) | Same restore/quality/build gates passed (zero compiler warnings/errors). TRX: 33 executed/passed, zero failed/not-executed; PostgreSQL cases are allocated to Linux. Self-contained publication, migrations, loopback browser and abrupt restart passed. Browser JUnit: 1 test, zero failures/errors/skips. This is not a clean Windows 11 VM. |

Both logs report no vulnerable NuGet packages, zero npm vulnerabilities and no Gitleaks findings. Downloaded Gitleaks reports are empty arrays; license inventories are present. OpenAPI artifacts report 3.1.1 with only root, bundled asset and the two health paths. Job logs confirm read-only Contents/Metadata permissions and pinned action execution; the workflow contains no deployment secret references. Platform-specific steps run on their designated job, not as substitutes for missing tests.

Downloaded ZIPs are preserved under ignored `artifacts/evidence/hosted-36969033339/`, alongside extracted reports. Independently computed SHA-256 values match GitHub's artifact digests:

| Artifact | SHA-256 |
|---|---|
| [foundation-ubuntu-24.04 / 11211021577](https://github.com/NAKAMA-SMS/CRC/actions/runs/36969033339/artifacts/11211021577) | `b2239f2d94ce033eadbdd7faf79c0b43c9c067678ce5454f5354c040c01f8b13` |
| [foundation-windows-2022 / 11210723602](https://github.com/NAKAMA-SMS/CRC/actions/runs/36969033339/artifacts/11210723602) | `1afbc4dcaf90eff6e1487a6aa97e9e605002db7b1e30b1d53dc9b3dfdd3631a8` |

Reproduce by checking out the exact commit and running `.github/workflows/foundation.yml` on its two specified runners; retrieve the linked artifacts, verify their hashes, and inspect `tests/*.trx`, `browser.xml`, provider versions, OpenAPI, security/license reports and Windows smoke/log/hash files. Audit output is in the job logs. Both jobs emitted action-runtime deprecation warnings, but no failing check; this is maintenance information, not evidence of a CRC compiler or security failure.

VM access preflight: `Get-VM` on host `BATURE` failed with `VirtualizationException`: the current account does not have the required permission. No guest was accessed and no clean-VM, graceful-shutdown or OS-level network-isolation test was executed. A clean Windows 11 x64 VM name/access method has been requested. Do not infer that no VM exists from this permission failure. No host firewall or runtime installation was changed.

## Criterion ledger

PASS below means the named scoped behavior has local execution evidence. The cross-platform/clean-environment criteria remain BLOCKED where their required environment has not been exercised.

| Criterion | State | Evidence / remaining condition |
|---|---|---|
| F00-01 | PASS | Exact committed source passed fresh hosted Linux/Windows checkout, locked restore, quality and build gates in run 36969033339; logs and downloaded evidence verified above. |
| F00-02 | DEFERRED / NOT PASSED, profile tests pass | Both real-provider host profiles verified. Browser external requests were blocked, but OS-level host Internet denial must still be exercised in the isolated VM. |
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
| F00-13 | DEFERRED / NOT PASSED, local publication smoke passes | No clean VM with SDK/runtime/Node absent has been supplied. Manual graceful stop/restart and host-level Internet denial remain required. Abrupt restart is not graceful-shutdown evidence. |
| F00-14 | PASS for foundation scope | Windows service-compatible host runs in console mode under the observed non-admin token and listens only on configured loopback. Actual SCM installation/recovery remains 05. |
| F00-15 | PASS | Both hosted jobs succeeded for 8adf1ab; actual TRX/browser/publication reports downloaded and verified. Workflow permissions, pinned actions and failure propagation reviewed. |
| F00-16 | PASS for current lock graph | Advisory/secret results and package-license review above. Re-run when dependencies change; production distribution notices remain a release obligation. |
| F00-17 | DEFERRED / NOT PASSED | OpenAPI 3.1 artifact and local contracts/docs checked; cannot close the overall handoff/freeze while the environment gates above are open. |

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

## Approved timing exception ? 2026-10-02

The owner approved deferring the outstanding F00-02/F00-13/F00-17 evidence, as recorded in ADR-0001. No new test was run and no criterion became PASS through this decision. Module 00 remains NOT FROZEN. The Module 00 acceptance workstream must supply the unchanged clean-VM, graceful-shutdown/persistence and OS-level network-isolation evidence and close F00-17 **before Module 05 starts**. Conditional progression through Modules 01-04 is allowed with their own gates and foundation regression checks intact; Module 03 offline CBT validation is not deferred. This accepts the risk of later clean-environment defects and dependent rework, not deployment readiness.

## Exact next action

Resolve PROJECT_STATE D01/D02 for Module 01 preparation; implementation requires separate authorization. Arrange the required clean Windows 11 x64 VM in parallel and execute the existing F00-02/F00-13 procedure. Record exact tested commit/artifact hashes, guest inventory, commands and outcomes, fix defects and rerun affected checks. Close F00-17 and freeze Module 00 before Module 05 starts. Existing hosted PASS evidence applies to 8adf1ab, not automatically to later code.
