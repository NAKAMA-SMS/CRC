# CRC Project State

Updated: 2026-10-02
Current phase: Module 00 implementation complete; remaining validation DEFERRED under the approved ADR-0001 gate exception; NOT FROZEN.
Product: CRC, powered by NAKAMA; Christian Royal College is the customer context. Corporate website and unrelated company systems remain outside this repository.

## Verified baseline and work performed

The preparation audit began at 4755c48. This implementation task initially inspected 28e1c12 with preparation documentation changes; those changes subsequently appeared in commit e8e1d48. Existing preparation work was preserved. The readiness report remains historical audit context, not authority over the accepted ADRs/module specification. The agent has not committed or pushed the implementation.

Module 00 now contains five production projects (API, Foundation, PostgreSQL, SQLite, DbMigrator), one xUnit project, the static React client, Playwright tests, exact tool/package locks, independent baseline migrations, configuration/security/error/logging/health behavior, validation scripts and a two-platform GitHub Actions workflow. Windows API and migrator artifacts were published and exercised. No domain tables, identity/users, academic/CBT workflows, sync, OCR, AI, analytics, installer/manager or production deployment exists. No module is complete or frozen. Module 01 has not started.

## Accepted foundation decisions

- [ADR-0001](docs/decisions/ADR-0001-authority-and-module-gates.md): existing document precedence preserved and clarified; decisions close before affected implementation; scoped module freezes and cross-module acceptance ownership; design/examples cannot add requirements.
- [ADR-0002](docs/decisions/ADR-0002-module-00-technical-foundation.md): .NET/ASP.NET Core 10, C# 14, React 19.3/TypeScript 6.0/Vite 8.3, Node 24/npm 11 for builds; EF Core 10 with separate PostgreSQL/SQLite contexts and migrations; explicit host/API/config/logging/health contracts; xUnit/Playwright/GitHub Actions; self-contained Windows publishing foundation.
- [Module plan](modules/00-foundation/MODULE_PLAN.md), [acceptance criteria](modules/00-foundation/ACCEPTANCE_CRITERIA.md), [setup](modules/00-foundation/SETUP.md) and [evidence](modules/00-foundation/EVIDENCE.md) define the implemented foundation and remaining acceptance gates.

The selected stack lines were preserved and exact packages resolved. ADR-0002 was clarified for an explicit Windows publication runtime graph, fail-closed PostgreSQL transport, and a distinct bounded cold-start budget; it does not change product scope or waive the two-second health-probe budget. No new ADR was necessary. Remaining Module 00 gaps are execution evidence. The owner-approved ADR-0001 exception below changes their timing, without waiving pass conditions or authorizing later implementation.

## Deferred decision register

BLOCKED below means the named later decision cannot safely be closed from existing evidence; it blocks that affected behavior, not unrelated Module 00 infrastructure. No owner name is invented: ownership is assigned to the indicated module's planning/approval gate.

| ID / status | Decision / missing evidence | Required before / prohibited assumption |
|---|---|---|
| D01 BLOCKED | Single-school versus multi-tenant operation; installation count and trust boundaries. Customer is known, cardinality is not. | 01 identity/scope schema; revisit earlier if a foundation table/API needs a school identifier. No SaaS tenant model or hardcoded school identity in 00. |
| D02 BLOCKED | First-login reset coverage conflicts in context/requirements versus security/authentication; exact bootstrap, credentials, sessions, role multiplicity, password/rate policies and offline credential reconciliation. Needs explicit security policy resolution. | 01, with secure transport/reconciliation contract closed before 05. No cookie/JWT choice, user model, dummy credentials or production bootstrap in 00. |
| D03 DEFERRED | Academic relationships, promotion eligibility/history workflows, question rich-content/schema/version/publication permissions; authoritative contracts must replace illustrations. | 02; full promotion ownership assigned in its plan. No business schema or final state machine in 00. |
| D04 BLOCKED | Immediate student scores versus configurable release (context 17/50, CBT-013/design 32.4); needs explicit product release policy. Marking, expiry/replay/concurrency and answer-key rules also require a complete assessment contract. | 03. No inferred result visibility, timing extension, score formula or state ordering. |
| D05 DEFERRED | Final teacher/report metrics and minimum evidence; imported review integration. | 04 plan for its workflows; 06 ingestion integration; 08 formulas/derived views. No invented metrics in earlier UI. |
| D06 DEFERRED | Complete sync payload/entity matrix, security conflict workflow, installation registration/replacement, tombstone/dedup retention and version compatibility. | 05; category authority and durable/idempotent principles remain binding. No queue/outbox or sync schema in 00. |
| D07 BLOCKED | School Windows edition/hardware/load, private DNS/TLS certificate provisioning, production secret protection/provisioning, installer/manager UI, service registration/signing/update/backup and recovery objectives. Actual operational environment not supplied. | 05 production installation decisions and 09 acceptance; 00 uses isolated validation targets, not a school hardware assumption. No production exposure approved. |
| D08 DEFERRED | OCR worker/runtime/versions, import schemas, file limits, quality corpus and fidelity thresholds. | 06; existing online PaddleOCR direction preserved. No alternative OCR/model invented. |
| D09 DEFERRED | Ollama/embedding/vector choices, resource budgets, conversation retention, evaluations and any parent AI scope. | 07; analytics-grounded interpretation completes with 08. No AI business authority. |
| D10 DEFERRED | Complete analytics formulas, populations/windows/thresholds and historical rules. | 08, or earlier if a 04 feature actually consumes a metric. No absence-of-evidence-as-failure assumption. |
| D11 DEFERRED | Final design assets, missing tokens and full component packages. | First affected UI module; Markdown controls conflicting board values. No full design system in 00. |
| D12 DEFERRED | Production cloud provider/topology, durable logging/retention, broader privacy retention, ASVS version/level and formal penetration-test plan. | Relevant security/data module before storing governed data; deployment before live service; full release before 09 acceptance. No global retention policy inferred from technical examples. |

## Persistent integration obligations

| Earlier baseline | Later required integration / status |
|---|---|
| 00 local host/storage | 01 local identity; 03 real offline CBT; 05 managed installation/sync — all PENDING |
| 01 authorization | 02 real academic assignments/relationships and regression; 05 credential sync/revocation — PENDING |
| 02 content lifecycle / 04 review | 06 actual upload/extraction-to-publication — PENDING |
| 03 offline assessment | 05 managed-installation/reconnection regression — PENDING |
| 04 teacher / 07 content AI | 08 derived analytics UI and evidence-grounded interpretation — PENDING |
| 05 offline environment | 07 configured AI/failure isolation; whole offline definition remains open — PENDING |
| All scoped baselines | 09 complete security/deployment/recovery/production acceptance — PENDING |

## Environment and evidence status

Verified locally on Windows 11 Pro 10.0.26200 with a non-elevated token: .NET 10.0.401/runtime 10.0.12, Node 24.16.0/npm 11.21.0, PowerShell 7.6.6, PostgreSQL 17.11 (disposable container), SQLite 3.53.3 and pinned Chromium revision 1243. Portable tooling was provisioned for this task; setup instructions do not depend on its temporary installation paths.

Final environment recheck: the temporary SDK was no longer available after the successful full verification cycle; a post-documentation formatter repeat could not start. Cause unresolved. Reprovision the pinned development tools before local reproduction; do not interpret earlier test evidence as a claim that those temporary tools remain installed. No application source changed after the passing cycle.

Passed: locked restores; .NET formatting/Release build with zero warnings; frontend lint/format/type/build; **38 tests, zero failures/skips**, including real-provider transactions, migration locks/failed upgrades, runtime DDL denial, HTTP/configuration/redaction and a real blocked PostgreSQL readiness query; **one published browser test**; self-contained Windows API/migrator publication, concurrent migrator processes, repeat/status, loopback-only listener, protected external data and abrupt restart. Advisory scans reported no vulnerabilities; redacted Git-history/working-tree scans reported no leaks. License inventory/review and OpenAPI artifact exist. Raw local evidence is under ignored artifacts/evidence; the criterion ledger is in EVIDENCE.md.

Fixed defects: test bootstrap/controller discovery, test cancellation compliance, SQLite fixture pooling, publication-induced lockfile drift, cold-start timeout scoping and provider transport defaults. No known local failure is being waived. Required remaining evidence: fresh hosted Linux/Windows runs (F00-01/F00-15), clean Windows VM with SDK/runtime/Node absent and graceful shutdown (F00-13), OS-level Internet denial for the host (F00-02), and the resulting overall handoff gate (F00-17). No hosted CI run or clean-VM validation has occurred. A VM availability question was sent; no environment has been supplied. Later decision gates and integration obligations above are unchanged.

## Module 00 closure pass — 2026-10-02

The implementation is now committed at `8adf1ab06ec457578a09925ddf8d34685f70c8f8` on main, matching origin/main at inspection; the initial worktree was clean. Earlier statements about unavailable hosted evidence above describe the implementation session and are superseded by this closure record.

Verified the existing [GitHub Actions run 36969033339](https://github.com/NAKAMA-SMS/CRC/actions/runs/36969033339), attempt 1, completed successfully at 05:31:56 UTC for that exact commit. Both jobs passed locked restores, quality checks, builds and security checks. Linux: 38 tests passed, zero failed/skipped, real PostgreSQL 17.11 and SQLite 3.53.3; Windows: 33 tests passed, zero failed/skipped, plus self-contained publication/migrations/abrupt restart. Each job passed one browser test. Downloaded both evidence ZIPs, independently verified their SHA-256 against GitHub, and inspected TRX/JUnit, versions, OpenAPI and Windows smoke reports. EVIDENCE.md contains job/artifact links and hashes; archives are retained under ignored `artifacts/evidence/hosted-36969033339/`. This pass reviewed actual hosted executions; no local suite rerun or new CI trigger is claimed.

F00-01 and F00-15 are now PASS. F00-03 through F00-12, F00-14 and F00-16 remain PASS. F00-02, F00-13 and F00-17 remain BLOCKED. Module 00 is NOT FROZEN. No implementation or ADR change was needed for the successful hosted checks. Action-runtime deprecation warnings are recorded in evidence; no CI failure was suppressed.

VM access preflight failed: `Get-VM` returned a Hyper-V permission error on BATURE. This does not establish whether a suitable guest exists. A clean Windows 11 x64 VM name/access method has been requested; none has been verified. SDK/runtime/Node absence, non-admin guest operation, graceful shutdown/restart and OS-level outbound isolation remain unexecuted. Host firewall and installed runtimes were not changed. Access to the isolated guest is the remaining external prerequisite; the Windows Server CI runner cannot satisfy this criterion.

## Approved validation deferral ? 2026-10-02

The owner confirmed that no VM is available and approved documenting the specific gate exception in ADR-0001. F00-02, F00-13 and F00-17 are now **DEFERRED / NOT PASSED**, not waived or satisfied; earlier BLOCKED entries above describe the pre-exception state. Module 00 implementation is complete, acceptance is incomplete, and the module remains NOT FROZEN. Hosted F00-01/F00-15 evidence and the other passing evidence are unchanged. No new test execution is claimed by this documentation update.

| Outstanding evidence | Owner and hard deadline |
|---|---|
| F00-02: Local host with OS-level outbound Internet denied | Module 00 acceptance workstream; before Module 05 starts |
| F00-13: clean Windows 11 x64 VM without SDK/runtime/Node; non-admin operation, graceful stop/restart and persistence/integrity | Module 00 acceptance workstream; before Module 05 starts |
| F00-17: final evidence reconciliation and foundation freeze | Module 00 acceptance workstream; after F00-02/F00-13 pass and before Module 05 starts |

Modules 01-04 may progress in sequence under this limited exception, with their own decision, scope, acceptance and regression gates intact. Module 03 offline CBT/recovery remains mandatory. The risk is unverified clean-host/network-isolation behavior and potential rework in dependent modules. Any newly exposed foundation defect blocks affected work and requires correction; no production deployment is approved. Module 05 cannot start until real evidence closes all three criteria and Module 00 is frozen.

## Exact next action

Prepare Module 01 by resolving D01 (school/installation scope) and D02 (identity/bootstrap/session/security policy) in canonical documentation, then obtain separate implementation authorization. This task documents only the gate exception; Module 01 has not started. In parallel, arrange access to the required clean Windows 11 VM and execute the unchanged F00-02/F00-13 procedure against identified artifacts, recording commit/hash, guest inventory, commands and outcomes. Revalidate affected foundation behavior after changes and close F00-17 before Module 05 starts.
