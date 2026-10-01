# CRC Project State

Updated: 2026-10-01
Current phase: Module 00 preparation complete; implementation NOT STARTED.
Product: CRC, powered by NAKAMA; Christian Royal College is the customer context. Corporate website and unrelated company systems remain outside this repository.

## Verified baseline and work performed

Starting implementation snapshot: 4755c48, documentation-only, clean working tree at inspection. The Engineering Readiness Report at `../Report/CRC-Engineering-Readiness-Report.md` was the starting audit, not authority over canonical requirements. Findings were checked against targeted repository sections. Official vendor documents were consulted only for technical feasibility of newly selected tools; sources are in ADR-0002.

No application, manifests, lockfiles, database schema/migrations, tests, CI or installer has been implemented. No module is complete or frozen. Module directories 01-09 have no specifications. This preparation pass created documentation and made targeted consistency corrections only; no dependencies installed, tests executed, production actions or commits performed.

## Accepted foundation decisions

- [ADR-0001](docs/decisions/ADR-0001-authority-and-module-gates.md): existing document precedence preserved and clarified; decisions close before affected implementation; scoped module freezes and cross-module acceptance ownership; design/examples cannot add requirements.
- [ADR-0002](docs/decisions/ADR-0002-module-00-technical-foundation.md): .NET/ASP.NET Core 10, C# 14, React 19.3/TypeScript 6.0/Vite 8.3, Node 24/npm 11 for builds; EF Core 10 with separate PostgreSQL/SQLite contexts and migrations; explicit host/API/config/logging/health contracts; xUnit/Playwright/GitHub Actions; self-contained Windows publishing foundation.
- [Module plan](modules/00-foundation/MODULE_PLAN.md) and [acceptance criteria](modules/00-foundation/ACCEPTANCE_CRITERIA.md) define the concrete implementation handoff. All F00 criteria remain NOT EXECUTED.

No remaining architectural/product decision blocks the bounded Module 00 scope. Exact dependency patch resolution and executable validation are its first tasks, not already-proven results. Compatibility problems must reopen the relevant ADR instead of changing the stack silently.

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

Repository origin is GitHub. Hosted Actions permissions, a clean Windows validation VM and selected SDK/packages are not yet verified. These are execution prerequisites/acceptance risks, not evidence of passed checks. If unavailable during implementation, record the exact missing access/tool as BLOCKED and continue only independent work. Required checks cannot be silently skipped.

Preparation validation: targeted document/diff review completed; local Markdown links in all six new/updated handoff and decision files resolve; git diff --check passed after correcting one trailing-space finding. Scope inventory confirms documentation-only changes. Application tests and integration execution are NOT RUN because application/test infrastructure does not exist. Selected package compatibility and CI/Windows runtime evidence remain unverified until Module 00.

## Exact next action

The next separately assigned task is to implement Module 00 only, starting with the MODULE_PLAN preflight and locked toolchain/project skeleton. Follow F00-01 through F00-17, update this record with actual evidence and freeze only the verified foundation. Do not begin Module 01 until Module 00 passes and D01/D02 are explicitly resolved. This preparation task ends before any implementation.
