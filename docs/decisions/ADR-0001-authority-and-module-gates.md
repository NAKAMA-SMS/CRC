# ADR-0001: Authority, decision timing and module acceptance boundaries

Date: 2026-10-01
Status: Accepted for Module 00 preparation under the instruction to close foundation blockers. No implementation or product expansion authorized by this record.

## Context and rationale

AGENTS.md section 2 and PROJECT_CONTEXT.md section 48 establish a hierarchy that conflicts with the roadmap's former subordination sentence. Context section 51 could require every later design before any implementation, while architecture sections 97-98 and roadmap section 39 permit scoped technical decisions. Sequential module freezing also appears to require integrations owned by later modules. These contradictions block a trustworthy foundation plan.

## Decisions

### Authority

Preserve the existing hierarchy, in this order: AGENTS.md; PROJECT_CONTEXT.md; PROJECT_STATE.md; DEVELOPMENT_ROADMAP.md; ARCHITECTURE.md; REQUIREMENTS.md; TESTING_STRATEGY.md; SECURITY_REQUIREMENTS.md; applicable accepted ADRs; module documentation; specialist docs; source/tests.

This is a reading and conflict-escalation order, not permission to discard requirements. PROJECT_STATE records facts and current scope; it cannot invent or waive requirements. The roadmap allocates delivery, not product/security policy. Every applicable mandatory security requirement remains binding. An ADR records a resolution and its rationale; affected higher-level passages must be updated with the resolution or an explicit reference. A lower-level example, implementation or state entry cannot silently override a higher-level requirement. Unresolved conflicts block the affected work.

### Decision timing

Resolve decisions before implementing the work they affect. Module 00 requires ADR-0002 and its module documents, not finished identity, CBT, synchronization, OCR or AI designs. A deferral must name its owner/gate, missing information and prohibited assumption in PROJECT_STATE. Reopen it earlier if a foundation change would depend on it. Technical dependency patch pins are implementation evidence; choosing a different framework/provider family requires an ADR update.

### Sequence and freeze semantics

Keep modules 00 through 09 in their existing order. A module's scoped baseline can be frozen only when all its allocated acceptance criteria pass. Whole-system architecture definitions of done remain open until all constituent modules and integrations pass. Every cross-module obligation remains tracked; it must never be recorded as passed using a mock or as delivered merely because a preceding baseline is frozen.

| Obligation | Delivery and verification boundary |
|---|---|
| Local host/API and SQLite | 00 delivers the executable host, configuration and database foundations for online/local profiles. 05 adds production orchestration, installer, watchdog and synchronization; it does not introduce local persistence for the first time. |
| Identity online/local | 01 implements both environments' authentication/account enforcement. Secure provisioning/session decisions are required before 01. Test fixtures may initialize isolated test identities; they are not a production bootstrap or sync mechanism. Credential transport/reconciliation integration completes in 05. |
| Academic scope in identity | 01 tests authorization policy boundaries using explicit synthetic relationships. 02 supplies real academic assignment/placement integration and repeats authorization regression. No placeholder academic schema or guessed relationship is required in 00. |
| Offline CBT | 03 retains its real offline exam, answer persistence and recovery gate using the local host from 00, identity from 01 and academic content from 02. It cannot defer offline correctness to 05. 05 repeats it through managed installation and sync recovery. |
| Teacher review and ingestion | 02 owns lifecycle/storage; 04 owns authorized teacher review; 06 adds extraction and completes source-upload-to-publication integration. Reviewed synthetic fixtures test earlier modules, without claiming OCR exists. |
| Teacher and AI analytics | 04 owns teacher workflow and authoritative result visibility. Derived class/topic-gap views and their teacher-screen integration complete in 08; 07 delivers content-grounded tutoring, with analytics-grounded interpretations integrated in 08. Do not show fabricated metrics or claim those later capabilities at earlier freezes. |
| Offline AI | 05 tests core operation with AI absent; 07 adds and validates local AI, active-CBT denial and failure isolation. The full offline architecture definition of done remains open until that integration passes. |
| Final acceptance | 09 repeats integrated release/installation/security/recovery acceptance. Each preceding module still validates its own security, integrity and regression obligations. |

This is an explicit allocation of cross-module acceptance, not a waiver of product requirements. The detailed 01-09 plans must include their applicable incoming/outgoing obligations before their implementation.

### Design and illustrative material

CRC is the product, NAKAMA the provider and Christian Royal College the customer. Product attribution remains CRC powered by NAKAMA. No corporate website is in scope.

The design guide defines presentation of approved functionality; attendance, timetables, assignments/submissions, disciplinary workflows, announcements, manual grading/authoring and other blueprint-only features do not become requirements. Its repository tree is illustrative; ADR-0002 defines the foundation layout. Markdown design values take precedence over the reference PNG where they differ; missing branding/tokens are deferred to the affected UI work. No full component library is required in 00.

Offline messages must distinguish client-unsent changes, school-server commits and pending cloud synchronization. AI cannot mutate authoritative records even with a confirmation button. No design text relaxes those boundaries.

API endpoint lists, entity sketches and state diagrams remain illustrative except for contracts expressly established in an accepted decision or module specification. ADR-0002 establishes only foundation-wide conventions and foundation endpoints.

## Deliberately unresolved

Score-display timing versus configurable release, detailed first-login policy coverage, multi-school/installation cardinality, business schemas and later operational choices are not settled by precedence alone. PROJECT_STATE records their BLOCKED decision gates. They do not block a foundation with no users, academic data, sessions or tenant model.

## Consequences and affected scope

This preserves established topology, product scope and sequence, while allowing truthful module baselines and explicit later integration gates. Canonical governance, roadmap, testing, design and architecture passages must point here. The cost is a persistent integration-obligation ledger; no module can silently drop a later obligation. No completed module or existing implementation is being reclassified: none exists.
