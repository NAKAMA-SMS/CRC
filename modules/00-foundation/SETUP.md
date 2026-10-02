# Module 00 setup and verification

This is the CRC foundation only. No identity or business capability is present. See the acceptance evidence before treating an artifact as accepted; a local smoke is not a production installation.

## Toolchain

Install .NET SDK **10.0.401** (global.json), Node **24.16.0**, npm **11.21.0**, PowerShell **7.6.6** (scripts require PowerShell 7), and Gitleaks **8.30.1**. Node/npm are build tools only. The implementation session used checksum-verified portable .NET, PowerShell and Gitleaks under the operating-system temporary directory; those local paths are not repository dependencies. Add your installed tools to PATH. Do not use the system npm 12 with this lockfile.

Exact NuGet packages are centrally pinned in Directory.Packages.props and six packages.lock.json files. npm's single package-lock.json contains exact packages and integrity values. Playwright **1.63.0** locks Chromium **153.0.8010.12**, revision **1243**; tests select its full Chromium executable in headless mode. PostgreSQL integration uses the pinned PostgreSQL 17 image digest in scripts/Test-Postgres.ps1. Native SQLite and PostgreSQL versions are captured by integration tests into artifacts/evidence.

From the repository root:

```powershell
dotnet tool restore
dotnet restore CRC.sln --locked-mode
npm ci
npx --no-install playwright install chromium
npm run build --workspace apps/web
dotnet build CRC.sln -c Release --no-restore
```

Linux browser preparation additionally needs Playwright's system dependencies (`playwright install --with-deps chromium`). Download/browser failure is a failed prerequisite, never permission to switch browser revisions. In the implementation session Playwright's downloader timed out, but curl downloaded the identical official archive successfully; browser tests subsequently used the locked revision.

## Isolated local operation

Create a restricted writable data directory outside the checkout/published binaries. Restrict its ACL to the current test identity. Supply configuration through the current shell, not application command-line arguments. Use only disposable data with loopback HTTP:

```powershell
$env:CRC_Environment = 'Test'
$env:CRC_Profile = 'Local'
$env:CRC_Provider = 'Sqlite'
$env:CRC_DataRoot = 'C:\absolute\restricted\disposable-data'
$env:CRC_BindAddress = '127.0.0.1'
$env:CRC_Port = '5080'
dotnet run --project src/CRC.DbMigrator -c Release --no-build -- apply
dotnet run --project src/CRC.DbMigrator -c Release --no-build -- status
dotnet run --project src/CRC.Api -c Release --no-build
```

Use your actual protected directory; the path above is illustrative. The host reads built configuration/assets beside its executable. Build the client before building/publishing the host. Browse `/`; health routes are `/health/live` and `/health/ready`. Stop console operation with Ctrl+C. The API rejects incompatible/missing migration history and performs no schema DDL. Local creation/WAL initialization belong to the migrator. No cloud credentials, Node service, OCR or AI service are required at runtime.

Online operation requires `CRC_Profile=Online`, `CRC_Provider=Postgres` and a securely injected `CRC_ConnectionString`. The API receives only the runtime role; run the migrator separately with the deployment role. Never put credentials in commands, checked-in JSON or browser variables. The disposable PostgreSQL script generates credentials and test-only roles automatically; no real credentials are needed.

PostgreSQL transport requires `SSL Mode=VerifyFull`. Only Development/Test with a literal loopback database host permits the disposable plaintext fixture. Staging/Production and non-loopback database connections reject unverified transport. Certificate provisioning remains D07; no production TLS deployment or handshake is claimed by loopback fixture tests.

## Configuration and security

Precedence is appsettings.json → environment-specific JSON → Development user-secrets → CRC-prefixed environment → protected key-per-file secrets for Staging/Production. `CRC_Environment` is required and cannot be changed by a later source. Deployment profile is independent of lifecycle environment. Required keys are Environment, Profile, Provider, absolute DataRoot, and Online ConnectionString. Binding defaults are loopback/5080. JSON array keys use environment names such as `CRC_AllowedHosts__0`. Missing required values, wildcard/public binding, cleartext non-loopback binding, mismatched providers, unknown environments and unsafe data paths are rejected. TrustedProxies must remain empty until a deployment decision defines proxy trust.

Staging/Production require `CRC_SecretsDirectory` pointing to protected mounted files; file names map configuration keys (`__` separates nested keys). The operator must enforce file/directory permissions and approved secret protection. This does not settle D07 production provisioning/encryption/TLS policy or authorize deployment. HTTPS development uses an explicitly trusted development certificate. Do not disable certificate validation. Only isolated Test/Development loopback may use HTTP.

No authentication scheme or dummy principal exists. Protected endpoints default to denial; only health and enumerated static assets are public. Test controllers and test migration classes are in the test assembly only. Operational JSON logs exclude raw exception messages, request values and provider SQL. Logs are not an audit store. Data is never served as a static directory. Service hosting support is compiled in, but no service is installed or registered.

## Checks and evidence

```powershell
pwsh -File scripts/Install-Scanner.ps1
pwsh -File scripts/Verify-Foundation.ps1
```

The scanner install verifies pinned SHA-256 digests. The aggregate verifier requires Docker for a disposable, loopback-only PostgreSQL 17 container; it runs the real provider suite and removes only the container it created. `dotnet test` without the explicit disposable PostgreSQL input fails its PostgreSQL cases. Windows CI intentionally runs SQLite/HTTP cases; the Linux CI job owns PostgreSQL and also repeats SQLite/HTTP. This is allocation between required jobs, not a skipped provider gate.

Other entry points: `scripts/Test-Postgres.ps1`, `scripts/Verify-Browser.ps1`, `scripts/Verify-WindowsPublish.ps1`, and `scripts/Write-DependencyInventory.ps1`. The Windows script publishes both self-contained win-x64 executables in `artifacts/Windows Release/`, launches from a path with spaces, restricts data ACLs, tests the built browser and listener, and verifies an abrupt restart without baseline mutation. It does **not** prove graceful shutdown, clean-VM runtime absence, non-admin execution or OS-level Internet denial. Follow F00-13's manual procedure for those gates. Do not install/uninstall runtimes or change the user's firewall to simulate a clean VM.

Evidence is written under ignored `artifacts/evidence/`: TRX, browser JUnit, OpenAPI 3.1, database versions, log captures, artifact hashes, dependency inventories and scans. Persist the reviewed evidence with the release/CI run before freeze. Generated files, credentials, databases and downloaded tools are ignored by Git. The workflow has no deployment credentials, uses read-only permissions and SHA-pinned actions. A workflow file does not constitute a successful hosted run.

`THIRD_PARTY_REVIEW.md` records dependency-license review scope. Production release still requires preservation of applicable third-party notices in its distribution.
