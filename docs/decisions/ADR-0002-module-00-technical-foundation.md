# ADR-0002: Module 00 technical foundation

Date: 2026-10-01
Status: Accepted technical baseline; implementation and compatibility evidence pending.
Scope: Module 00 infrastructure only. Product requirements remain repository-defined.

## Context and rationale

ARCHITECTURE.md section 98 permits a type-safe stack selected for Windows deployment, PostgreSQL/SQLite support, offline reliability and maintainability. No previous framework or library decision exists. Choose a modular application with shared backend behavior and two explicit deployment profiles, not separate online/offline business implementations or microservices.

.NET offers a supported Windows service path, self-contained deployment and first-party hosting/configuration/testing integration. EF Core provides a common access discipline with distinct PostgreSQL and SQLite providers/migrations. A statically built React client avoids a Node production dependency on the school server. Costs include two development languages, provider-specific migrations, and responsibility for testing actual SQLite concurrency/limitations. An all-TypeScript backend is viable but would add a separate Windows service/packaging selection; no existing code or team constraint makes it preferable. No new broker, cache, container orchestrator or generic repository layer is justified for 00.

## Runtime, language and tools

| Area | Selected baseline |
|---|---|
| Backend | .NET 10 LTS, ASP.NET Core 10, C# 14, target net10.0; nullable enabled, compiler/analyzer warnings treated as errors |
| API host | ASP.NET Core controllers with explicit DTOs, System.Text.Json and built-in dependency injection/configuration/health abstractions |
| Frontend | React/React DOM 19.3, TypeScript 6.0 strict mode, Vite 8.3 with React plugin; static client assets |
| Frontend build only | Node.js 24 LTS and npm 11; npm workspaces and one package-lock.json |
| Relational access | EF Core 10; Npgsql.EntityFrameworkCore.PostgreSQL 10; Microsoft.EntityFrameworkCore.Sqlite 10; matching dotnet-ef 10 local tool |
| Database baseline | PostgreSQL 17 supported patch release; SQLite 3 native runtime supplied by the locked Microsoft SQLite provider dependency graph, with its actual version captured in evidence |
| Backend tests | xUnit.net v3, Microsoft.AspNetCore.Mvc.Testing 10, compatible test SDK/adapter; dotnet test via VSTest |
| Browser tests | Playwright Test 1.x with its locked Chromium revision; real built client and host |
| Quality | .NET analyzers/dotnet format; ESLint with compatible typescript-eslint and React rules, Prettier; tsc type checking separately from Vite |
| CI | GitHub Actions, consistent with the existing GitHub origin; Ubuntu 24.04 job for PostgreSQL/client checks and Windows Server 2022 runner for win-x64/local checks |

These are intentional version lines, not assertions that a lockfile exists or the combination has already passed. At the first implementation step resolve stable, non-preview, compatible security-patched releases within these lines; pin exact SDK, npm/Node, direct/transitive package and browser versions in global.json, central NuGet versions/packages.lock.json, tool manifest, package.json/package-lock.json and toolchain documentation. CI uses locked restore and npm ci. Capture native SQLite version and license/advisory review. Do not silently cross a chosen major/minor line to solve an incompatibility: document it and update this decision first. No dependency installation occurs in this preparation pass.

## Repository and dependency structure

Planned artifacts; none is created by this ADR:

| Path | Responsibility |
|---|---|
| CRC.sln | Backend solution |
| src/CRC.Api/ | Executable HTTP/static-asset host; explicit Online or Local profile |
| src/CRC.Foundation/ | Configuration validation, safe API errors, correlation and hosting conventions; no future domain framework |
| src/CRC.Persistence.Postgres/ | Online EF context/provider and migrations |
| src/CRC.Persistence.Sqlite/ | Local EF context/provider and migrations |
| src/CRC.DbMigrator/ | Explicit non-HTTP migration executable; provider/profile chosen by configuration |
| apps/web/ | Single React/TypeScript client; minimal foundation screen only |
| tests/CRC.Foundation.Tests/ | Unit, host and provider integration tests, categorized so missing DB prerequisites fail required checks |
| tests/e2e/ | Playwright foundation browser tests, npm workspace |
| scripts/ | Reproducible PowerShell 7 validation/publish orchestration introduced during 00 |
| .github/workflows/ | Validation only; no automatic deployment |

The host composes foundation and provider adapters. Persistence does not depend on the host/UI. Domain code arrives in its owning module; do not create empty future-module projects or a universal business repository abstraction. No apps/offline fork, design packages, AI services or sync workers are created in 00. Extract shared UI packages only when an actual later requirement justifies them. Existing docs/ and modules/ remain canonical.

Online profile requires PostgreSQL; Local requires SQLite. Invalid/mismatched combinations fail configuration validation. There is no automatic switch/failover between databases and no synchronization-by-database-copy. A profile is not a tenant, user role or authorization boundary.

## Persistence and migrations

Use parameterized EF queries; raw SQL, where unavoidable, is parameterized and reviewed. Keep provider contexts and migration histories separate. Both databases are tested directly; in-memory EF is not acceptance evidence.

Module 00 creates only a baseline migration/history in each provider: no user, school, question, audit, outbox or other domain table. Transaction/constraint/failure tests may use isolated test-only tables never shipped in production migrations. A second disposable test migration must verify upgrade and rollback-of-failed-transaction behavior without inventing a product entity.

Migrations run through CRC.DbMigrator with deployment credentials; the API never auto-migrates or receives PostgreSQL DDL credentials. Startup checks expected migration state and fails closed on missing, older or unknown/newer schema. The migrator reports safe errors and nonzero exit on failure; a repeated apply is a no-op. Concurrent migrators are serialized with a PostgreSQL advisory lock or exclusive local migration lock. Normal local writers must be stopped during migrations; the future manager invokes this executable in 05.

For SQLite use a protected absolute local-disk path, not a network share or public web directory; WAL and foreign_keys ON, synchronous FULL, initial busy timeout 5 seconds. These are conservative technical defaults, not performance promises. Do not retry an entire non-idempotent business transaction automatically. Verify constraints, committed persistence, transaction rollback, write contention and integrity_check with the actual engine. A live WAL file must not be backed up by copying only the main database. Production backup/upgrade orchestration remains 05/09; test-only recovery in 00 uses isolated disposable data.

## API and host contracts

Adopt /api/v1 for business JSON APIs; explicit DTOs, camelCase properties and UTF-8 JSON. Use ISO-8601 UTC for technical timestamps. Business identifiers, date-only fields, money/score formats, pagination, idempotency and domain endpoints remain owned by later modules.

Success bodies use data plus meta objects; an empty meta is allowed. HTTP 204 has no body. Errors use an error object with code, safe message, details array and requestId. This explicitly adopts the shape illustrated in API_ARCHITECTURE.md sections 18/20, not every example endpoint/code. Error details may contain field and code only; never echo rejected secrets or arbitrary submitted values. Map malformed/invalid foundation input to 400 VALIDATION_FAILED, unauthenticated/denied future security middleware to 401 AUTHENTICATION_REQUIRED / 403 FORBIDDEN, unknown API paths to 404 RESOURCE_NOT_FOUND, method mismatch to 405 METHOD_NOT_ALLOWED, oversized input to 413 REQUEST_TOO_LARGE, unsupported media to 415 UNSUPPORTED_MEDIA_TYPE, unexpected errors to 500 INTERNAL_ERROR, and unavailable dependencies to 503 SERVICE_UNAVAILABLE. Endpoint-specific conflicts and business errors are deferred.

Generate a server-side correlation identifier per request and return X-Request-ID; do not trust a client-supplied value as a log identifier. Use Activity tracing internally. Bound input sizes and timeouts centrally: initial ordinary JSON ceiling 1 MiB, request-header timeout 15 seconds, database readiness probe budget 2 seconds. Upload/sync/exam limits are not established here. Validate and sanitize all logged input.

Only these anonymous foundation routes are delivered:

- GET /health/live: 200 with data.status = alive and meta = {}; proves host responsiveness, not DB health.
- GET /health/ready: 200 with data.status = ready and meta = {} when the selected DB is reachable and expected schema is present; otherwise 503 using the safe error envelope. No dependency names, versions, paths or connection details in the response. No mutation or migration. Initial incompatible schema prevents normal startup; a database/schema failure detected after startup makes readiness fail while liveness remains available.
- GET / and static assets: minimal CRC foundation screen, no academic data. Unknown /api paths must not fall through to the SPA.

Health probes are idempotent, require no request body, have no domain audit effect and produce bounded operational logs. Cache-Control is no-store on probes/API errors. Test-only validation/throw/authorization probe endpoints may be injected by the test host; they must not exist in a published application. Set a deny-all fallback authorization policy until 01 supplies real authentication/policies; only the enumerated safe routes are anonymous. No dummy user, permissive auth handler, signup or diagnostic explorer.

Generate OpenAPI 3.1 via Microsoft.AspNetCore.OpenApi as a build/test artifact for the implemented contracts. Do not expose a production explorer or detailed diagnostic endpoint before authorization exists. Same-origin client/API; no permissive CORS. Development uses the Vite proxy on loopback. Session/cookie/CSRF decisions are deferred to 01, before any authenticated mutation exists.

## Configuration, secrets and observability

Use typed .NET options validated before serving traffic. Deployment profile (Online/Local) and lifecycle environment (Development/Test/Staging/Production) are separate required values. Unknown environment/profile, invalid port/interface/path, writable-data path under the static web root, profile/provider mismatch or missing required secret fails closed. The application must not create future module secrets/configuration to satisfy startup.

Non-secret defaults live in appsettings.json and environment-specific settings. Document precedence: defaults, environment file, developer user-secrets in Development only, then CRC-prefixed environment settings. Production secrets are injected from an operator-managed secret store as protected mounted files via the .NET key-per-file provider; those values override non-secret sources. No secret command-line arguments. Development user-secrets are not encrypted production storage. Production secret files are restricted to the service identity; no values are exported to browser build variables, logs or diagnostics. Actual cloud secret-store provider and Windows provisioning/at-rest wrapping are deferred to deployment before real credentials are installed; no production deployment is approved by 00.

Configuration includes explicit bind address/port, allowed hosts, trusted proxies (empty by default), absolute data root and selected database settings. Loopback is the default development binding. A local LAN test uses an explicitly selected private interface and restricted firewall scope; no wildcard/public binding, port forwarding or disabled certificate validation. Production HTTPS/certificate provisioning remains a deployment gate. For 00 use trusted development certificates on test hosts; HTTP is permitted only on loopback for isolated developer/CI smoke tests with no credentials or academic data.

Use Microsoft.Extensions.Logging structured JSON to stdout with UTC timestamp, level, category, event ID, correlation ID and safe diagnostic code. Do not log request/response bodies, cookies, authorization headers, connection strings, secret configuration, SQL parameter values or raw exception text that can embed them. Unexpected failures produce a generic response and a sanitized internal event identifying exception type and safe stack locations. Redaction tests must inject recognizable secret markers and prove their absence from both response and captured logs. Business audit storage arrives with its owning module; operational logs are not audit records.

The online process supervisor and eventual local manager own durable log capture/rotation. Module 00 captures logs for validation; it does not choose production retention or implement log search/export. No external metrics backend, message broker or tracing collector is required. Core readiness depends only on the selected database/schema; absent Internet, AI, OCR or synchronization must not make the local foundation unready.

Serve a restrictive production-build CSP: default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; connect-src 'self'; object-src 'none'; base-uri 'none'; frame-ancestors 'none'. Add X-Content-Type-Options: nosniff and Referrer-Policy: no-referrer; HTTPS-only HSTS is environment-aware. Any development HMR relaxation is Development-only and tested absent from publish output. Static assets are bundled locally; no CDN/fonts/telemetry dependency. This establishes a safe empty shell, not rich-content rendering rules.

## Windows deployment foundation

Publish CRC.Api as self-contained win-x64, untrimmed, ordinary directory output (not Native AOT or a single-file bundle). Static React assets ship with it; Node/npm and a .NET SDK must not be needed on the school runtime. Publish CRC.DbMigrator as a separate self-contained win-x64 executable in the same release artifact so clean-host migration does not require the SDK. Use Microsoft.Extensions.Hosting.WindowsServices for API service-host compatibility, plus console mode for development. A self-contained artifact must be republished for runtime security updates.

Module 00 validates start, graceful stop, restart and persistence from a path containing spaces, under a non-administrator account, with mutable data outside binaries. It does not install a privileged service, change machine firewall rules, create an installer, choose the manager UI toolkit or implement LMSServer.exe. That remains the 05 orchestrator, separate from CRC.Api. Windows service registration/recovery policy, school OS edition/hardware, DNS/TLS, signing and installation identity are decisions before 05. Windows 11 x64 and the Windows CI runner are validation targets, not an assertion about school hardware already available.

The online foundation runs the same host in Online profile; Linux compatibility is checked in CI. Production cloud hosting, reverse proxy and release automation remain deferred; no cloud account is needed for 00.

## Testing and CI boundary

CI runs on pull requests and pushes without deployment credentials: locked restores, formatting/lint/type checks, Release build, xUnit configuration/API/provider tests, built-client Playwright smoke, PostgreSQL 17 integration on Linux and SQLite/published win-x64 checks on Windows. PostgreSQL integration may use a disposable CI service container; Docker is a developer convenience, not a school runtime requirement. Missing required test prerequisites fail the relevant gate rather than silently skip tests.

Use GitHub Actions with read-only default permissions, commit-SHA-pinned third-party actions, no secrets on untrusted pull requests and no pull_request_target execution of untrusted code. Preserve sanitized test and dependency-audit evidence. Use NuGet audit and npm audit plus a pinned secret scanner (Gitleaks); review licenses and transitive runtime dependencies. Unresolved critical/high exploitable vulnerabilities block foundation acceptance; record dispositions rather than suppressing checks. CI access and runner availability are not assumed verified in this pass.

The module acceptance file defines exact intended commands and evidence. Patch resolution, test execution and artifact verification are implementation tasks; no test success is claimed here.

## Deferred scope

No identity/session provider, credential protocol, tenant schema, school cardinality, academic entity, assessment state/score policy, sync protocol payload, OCR worker runtime, vector store/model, analytics formula, full design library, installer or production hosting decision is made. Consult PROJECT_STATE for their gates. If any becomes necessary to implement 00, stop and reopen its decision rather than add a placeholder.

## Evidence for technical feasibility

Repository requirements are authoritative for CRC behavior. The following official technical documentation was checked on 2026-10-01 only to validate the proposed tooling, not to introduce product requirements:

- [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy) supports choosing the .NET 10 LTS line.
- [C# 14](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14) identifies its .NET 10 baseline.
- [Windows service hosting](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-10.0) documents ASP.NET service integration.
- [Npgsql EF 10](https://www.npgsql.org/efcore/release-notes/10.0.html) documents the PostgreSQL provider release.
- [Multiple-provider migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers) and [SQLite limitations](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations) support separate provider validation/migrations; SQLite support is not database parity.
- [PostgreSQL version policy](https://www.postgresql.org/support/versioning/) identifies 17 as a supported line.
- [Node releases](https://nodejs.org/en/about/previous-releases), [React versions](https://react.dev/versions), [Vite support](https://vite.dev/releases) and [TypeScript 6.0](https://www.typescriptlang.org/docs/handbook/release-notes/typescript-6-0.html) establish the selected frontend/build lines.
- [OpenAPI support](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-10.0), [xUnit v3](https://xunit.net/docs/getting-started/v3/getting-started) and [Playwright](https://playwright.dev/docs/intro) document the chosen contract/test tooling.

## Consequences

This is sufficient to implement a bounded foundation without selecting later business rules. It does not prove package compatibility, hardware capacity or production readiness. Module 00 must provide that scope's executable evidence. The initial ADRs are intentionally two records: governance/gates and the cohesive foundation baseline; later independent architecture changes receive their own records when actually needed.
