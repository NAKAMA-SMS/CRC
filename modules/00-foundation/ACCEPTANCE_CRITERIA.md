# Module 00 — Acceptance Criteria

Status: NOT EXECUTED. All criteria below are pending implementation.
Date: 2026-10-01
Scope and command contract: [MODULE_PLAN.md](MODULE_PLAN.md).
Technical contract: [ADR-0002](../../docs/decisions/ADR-0002-module-00-technical-foundation.md).

A documentation-ready module is not an implemented module. No checkbox is passed by this preparation pass. Retain an evidence table with criterion ID, exact command/manual procedure, environment/tool versions, result and sanitized artifact reference when execution occurs. A missing runner/database/tool blocks its required check; mocks and skipped tests are not substitutes.

## Gates

| ID | Pass condition | Verification / evidence |
|---|---|---|
| F00-01 | Fresh clone restores exact supported toolchain/dependencies, builds and passes format/lint/type checks; no unpinned action or secret/data artifact committed. | Plan restore/check/build commands on Linux and Windows; lockfiles, SDK/Node/npm/browser/native SQLite versions, dependency licenses and action SHAs recorded. |
| F00-02 | Host starts independently in Online/PostgreSQL and Local/SQLite profiles using separate environment configuration. Local profile has no online credentials or runtime Internet/AI dependency. | xUnit process/host tests for both profiles; local startup while outbound Internet is denied after packages/assets are provisioned. |
| F00-03 | Invalid environment/profile, profile/provider mismatch, missing required secret, unsafe binding/data path or invalid port fails before serving. Secret precedence works as documented. | Parameterized configuration tests including invalid paths, forbidden web-root storage and recognizable secret values; nonzero exit and safe diagnostics. |
| F00-04 | Real PostgreSQL 17 and file-backed SQLite connect. SQLite verifies WAL, foreign_keys ON, synchronous FULL and bounded busy timeout. | Provider integration queries and versions in sanitized results; no in-memory substitute. |
| F00-05 | Each provider applies its baseline migration once; reapplying is safe; status detects missing/older/newer schema. API never migrates and rejects incompatible schema. | Empty disposable DB -> apply -> repeat -> status -> host; mutate migration state in isolated fixtures and assert refusal. Verify no product tables beyond migration history. |
| F00-06 | Migrator is explicit and serializes concurrent applies. Failed migration cannot leave normal startup reporting ready. PostgreSQL runtime credential cannot execute DDL. | Disposable test migration failure and parallel runner tests; role-permission tests. Test-only tables/migrations must not ship in production assemblies. |
| F00-07 | Committed data survives process restart; transaction rollback and FK/unique constraints work on both providers; SQLite contention ends within configured bounds with a controlled failure. | Test-only tables in disposable databases, two-connection contention, host/process restart and SQLite integrity_check. No academic schema invented for this test. |
| F00-08 | GET /health/live and /health/ready return exact ADR shapes/statuses; DB failure leaves liveness available but readiness 503, without leaking dependency details. | HTTP tests with healthy DB, subsequent disconnection/schema divergence, plus process-start refusal for initially incompatible schema; readiness dependency probe has the specified timeout. Repeated probes do not mutate data. |
| F00-09 | API validation, unknown routes, method mismatch, size/media failures and unexpected exceptions use safe stable errors and correlation IDs; unknown API routes do not return SPA HTML. | API tests; test-only injected endpoints exercise exceptions/validation. Inspect final publish route inventory to prove those endpoints are absent. |
| F00-10 | Anonymous exposure is limited to static shell/assets and minimal health; fallback denies every other mapped endpoint. CORS is not permissive; production explorer/diagnostics absent. | Endpoint inventory plus negative tests with test-host-only protected routes. No dummy authentication scheme; genuine identity testing deferred to 01. |
| F00-11 | Logs contain timestamp/severity/category/event/correlation and useful safe failure diagnostics. Secret markers in configuration, headers, query/body and exception messages never appear in logs or responses. | Captured structured-log assertions; X-Request-ID correlation; production exceptions omit stack/SQL/config details in responses. Operational logs are not represented as audit storage. |
| F00-12 | Production client loads all assets locally, uses same-origin API and required CSP/security headers, and makes no external content/telemetry/font requests. | Playwright against built artifacts with outbound network blocked; DOM/readiness smoke and request/header inspection. No login/dashboard/business workflow is claimed. |
| F00-13 | Self-contained win-x64 artifact includes API, separate migrator and static assets, runs without SDK/Node, supports graceful stop/restart under a non-admin account and preserves local data outside binaries. | Verify-WindowsPublish.ps1 plus a documented clean Windows 11 x64 VM run with SDK/Node absent, path containing spaces and explicit data-root ACLs. Hosted runner alone cannot prove absence of globally installed runtimes. |
| F00-14 | Service-host integration can coexist with console mode; no admin dependency during normal run and no accidental wildcard/public listener. | Windows process/binding checks; no production service installation/firewall changes required in 00. Actual SCM installation/recovery remains 05. |
| F00-15 | Both CI jobs run required checks with no deploy secrets, least permissions and pinned actions; failures propagate. | GitHub Actions run links/results and sanitized xUnit/Playwright/publish reports. If permissions/runners are unavailable, record BLOCKED; do not claim CI passed. |
| F00-16 | Dependency/secret checks complete; critical/high exploitable findings resolved; other dispositions documented. | NuGet audit, npm audit, Gitleaks reports, license review and dependency inventory. No suppression solely to make checks pass. |
| F00-17 | OpenAPI documents actual implemented contracts; docs, run commands and published artifact behavior agree. All required criteria have evidence and no unresolved foundation defect. | Contract checks, setup reproduction, review of actual changed files; update PROJECT_STATE with results and scoped freeze status. |

## Minimal manual procedure where automation is insufficient

For F00-13, use an isolated supported Windows 11 x64 VM with no Node or .NET SDK/runtime preinstalled. Transfer the already-built self-contained artifact and create a restricted writable data directory outside binaries. Configure Local/Test with loopback binding and no secrets/real school data. Apply the baseline through the explicit migrator, launch as an ordinary account, verify health and client loading, stop and restart, and verify fixture persistence/integrity. Block Internet access while retaining loopback. Record OS/runtime inventory, commands, listener binding and sanitized outcomes. Never simulate success by uninstalling tools from an uncontrolled user machine.

A later 03 offline exam test must use real school-LAN-style client/server communication, identity/content fixtures and actual recovery behavior. The foundation's loopback/VM smoke is not that acceptance test. The 05 commissioning gate still requires the production-style manager, networking, secrets, backup/restore and synchronization.

## Completion rule

Every F00 criterion must be passed with evidence, or its applicability must be resolved through an explicit documented scope change before freeze. An unavailable required environment is BLOCKED, not not-applicable. No blanket percentage-coverage substitute or assumed success is allowed. Tests must exercise failure paths and real integration rather than only mirror implementation details.

After passing: update this file with evidence and PROJECT_STATE with Module 00's scoped freeze, defects/dispositions and pending Module 01 decision gates. Do not declare CRC production-ready, the whole offline architecture complete or any later module delivered.
