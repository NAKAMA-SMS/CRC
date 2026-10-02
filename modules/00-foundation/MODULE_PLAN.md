# Module 00 — Foundation Implementation Plan

Status: IMPLEMENTATION COMPLETE; remaining validation DEFERRED; NOT FROZEN.
Updated: 2026-10-02
Decision baseline: [ADR-0001](../../docs/decisions/ADR-0001-authority-and-module-gates.md), [ADR-0002](../../docs/decisions/ADR-0002-module-00-technical-foundation.md).
Acceptance: [ACCEPTANCE_CRITERIA.md](ACCEPTANCE_CRITERIA.md).

## Objective and authorized boundary

Deliver a reproducible CRC technical foundation that starts in Online/PostgreSQL and Local/SQLite profiles, serves a locally bundled minimal client, and demonstrates configuration, migrations, safe API behavior, observability and Windows publication. CRC is the product; NAKAMA is its technology provider. Implementation was separately authorized and has been performed within this boundary. See [EVIDENCE.md](EVIDENCE.md) for actual results and remaining gates; the plan does not itself prove acceptance.

No accounts, sessions, login UI, school/tenant tables, academic tables, student/teacher dashboards, assessment engine, audit store, outbox, synchronization, OCR, Ollama integration, analytics or product-design library. No public registration, corporate website, installer, service registration, deployment to production or real school data. No future placeholder models or permissive authentication stubs.

## Required inputs and preflight

Read AGENTS.md, PROJECT_STATE.md, the two ADRs and this module's acceptance file; consult targeted canonical sections as needed. Check current Git status and preserve unrelated changes. Verify the selected SDK/build tools, package availability and Windows/Linux validation access. Resolve compatible patched versions within ADR-0002 and record exact pins and licensing/advisory evidence; this is the first implementation task, not an unrecorded stack selection. If compatibility or an inaccessible required runner blocks validation, record BLOCKED and the exact missing prerequisite. Do not claim the module frozen using partial evidence.

Production school hardware, deployment secrets and a cloud provider are unnecessary for a disposable foundation. Both hosted jobs passed for commit 8adf1ab in run 36969033339; results and artifact hashes are recorded in EVIDENCE.md. Clean Windows 11 VM access and its required execution evidence remain acceptance gates.

The owner-approved 2026-10-02 gate exception in ADR-0001 allows conditional progression through Modules 01-04 while the remaining F00-02/F00-13/F00-17 evidence is deferred. Module 00 acceptance owns closure; all three must pass and Module 00 must freeze before Module 05 starts. No pass condition is waived and no later implementation is authorized by this plan update.

## Work sequence

1. **Reproducible skeleton and quality configuration.** Create only the ADR-0002 host/foundation/provider/migrator projects, minimal web client and tests that have immediate responsibilities. Pin SDK/tools/packages, establish strict compiler/type/lint settings, ignore secrets/data/build outputs and document fresh-clone setup. No empty domain projects.
2. **Explicit configuration and host composition.** Separate lifecycle environment from Online/Local profile; validate required bindings, paths and selected provider settings before serving. Introduce secret injection and safe logging without production credentials. Make invalid/missing/mismatched configuration fail closed.
3. **Provider and migration foundation.** Add independent PostgreSQL/SQLite contexts and baseline migration histories with no domain tables. Implement explicit migrator/status operation; API startup verifies schema without running DDL. Validate WAL, FK enforcement, contention and persistence on SQLite; PostgreSQL application/migration roles remain separate. Use disposable test-only schema to exercise constraints/transactions/failed migration, never production product entities.
4. **API/security/observability foundation.** Implement the exact health and error conventions in ADR-0002, safe request correlation, minimal public endpoints, deny-all fallback and no production diagnostic explorer. Test invalid input and unexpected failures through test-only host injections. Add built-output CSP/headers, strict same-origin behavior and safe structured logs.
5. **Minimal client and Windows artifact.** Serve a simple CRC foundation screen and readiness feedback with locally bundled assets. Use only established design values where needed; no dashboard blueprints or invented tokens. Publish the untrimmed, self-contained win-x64 host with client assets; verify no Node/.NET SDK runtime dependency, non-admin execution, paths with spaces, restart and durable local data. Keep data outside binaries. Implement service-host compatibility without installing the production manager/service.
6. **Automated checks and CI.** Run xUnit against both real providers, built-client Playwright tests and Windows publish smoke. Configure GitHub Actions validation without deployment credentials. Implement the command contract below; required prerequisite absence is a failing check, not a skipped pass.
7. **Acceptance and handoff.** Retain sanitized command outputs, test reports, locked toolchain and artifact details. Update the module acceptance evidence and PROJECT_STATE with actual results, remaining defects and next module gate. Freeze only after every applicable criterion passes; do not declare identity or production deployment ready.

## Intended command contract

These commands are implemented. Execute from repository root after configuring disposable databases through documented protected inputs; [SETUP.md](SETUP.md) describes prerequisites and safe fixture wrappers. No command embeds a secret. Exact executed results are in EVIDENCE.md.

| Command | Required result |
|---|---|
| dotnet tool restore | Restore pinned local EF tools |
| dotnet restore CRC.sln --locked-mode | Restore exact NuGet dependency graph |
| npm ci | Restore locked frontend/test dependency graph |
| dotnet format CRC.sln --verify-no-changes --no-restore | Formatting/analyzer compliance |
| npm run check | Workspace ESLint, Prettier check and tsc --noEmit |
| dotnet build CRC.sln -c Release --no-restore | Strict backend build |
| npm run build --workspace apps/web | Production client bundle |
| dotnet test CRC.sln -c Release --no-build --logger trx | Foundation, HTTP and real-provider tests with required prerequisites present |
| dotnet run --project src/CRC.DbMigrator -c Release --no-build -- status | Read-only selected-profile migration status, nonzero for mismatch |
| dotnet run --project src/CRC.DbMigrator -c Release --no-build -- apply | Explicit selected-profile migration apply; repeat safely |
| npm run test:e2e | Built host/client Playwright checks; fixtures start isolated hosts and databases |
| pwsh -File scripts/Verify-Foundation.ps1 | Orchestrate required checks, safe fixture setup and evidence; nonzero on failures/missing prerequisites |
| pwsh -File scripts/Verify-WindowsPublish.ps1 | Build client, publish win-x64, test artifact and local durability as an ordinary user; no service/firewall installation |
| dotnet list CRC.sln package --vulnerable --include-transitive | NuGet advisory evidence (also enforce restore audit policy) |
| npm audit | npm advisory evidence, reviewed under ADR-0002 policy |
| gitleaks git . --redact | Pinned secret-scanner check; no secret values in artifacts |

The Windows wrapper includes dotnet publish of src/CRC.Api with -c Release -r win-x64 --self-contained true, RestoreLockedMode=true, trimming and single-file disabled. It also publishes CRC.DbMigrator self-contained for win-x64, without trimming/single-file packaging, so a clean runtime host can apply the baseline. Explicit runtime identifiers keep ordinary and publication lock graphs consistent (ADR-0002 implementation clarification). It stages apps/web build output into the host's static assets and tests the result outside the source directory. Scripts document their prerequisite versions and clean only their own verified disposable fixture paths. Database tests cannot point at production/staging or use default guessed credentials.

Test-runner adapter details, exact patch versions and script implementation are checked during implementation and recorded in setup/evidence. Any necessary change to this public command contract must update both plan and acceptance documents before handoff.

## Requirements traceability and integration

| Basis | Module 00 responsibility | Deferred completion |
|---|---|---|
| Roadmap section 7; Testing section 60 | Startup, configuration, logging/errors, migrations, DB/API/test/build foundation | None for the scoped foundation |
| DB-001/002/003/004 | Real PostgreSQL/SQLite connectivity, WAL, migration and transaction discipline | Domain invariants/history in owning modules |
| SEC-002/003/004/005/007/008/010/011/012 | Deny fallback, foundation validation, parameterization, safe shell/headers, secret and error handling, dependency checks | Real-user authorization/CSRF in 01; uploads/rich content in 02/06; full audit/pentest before release |
| OBS-001/002/003 | Safe structured logging, severity/category/correlation | Audit/event store, admin search/export and retention in owning modules |
| SRV-001; NET-001/003/004; OFF-007; PERF-004 | Windows artifact, controlled binding, locally bundled shell and no Internet/AI dependency | LMSServer.exe, firewall commissioning, production LAN operation in 05; AI isolation integration in 07 |
| MAIN-001 through MAIN-005; TEST-002/007/008 | Modular responsibilities, documented toolchain, integration/security/regression checks | Later business tests when behavior exists |

Integration verification in 00 concerns real host-to-provider, migrator-to-schema, client-to-host and published Windows-to-local-storage behavior. It cannot establish business workflows that do not exist. ADR-0001 preserves incoming/outgoing obligations for later modules.
