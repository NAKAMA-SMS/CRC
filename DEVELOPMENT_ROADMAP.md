# CRC Development Roadmap

## 1. Purpose

This document defines the controlled development sequence for the CRC system.

This roadmap covers CRC only. The NAKAMA corporate website is a separate future project; unrelated company products and services are outside this roadmap.

It establishes:

* implementation order;
* module dependencies;
* development gates;
* integration requirements;
* testing requirements;
* documentation requirements;
* security gates;
* release gates;
* regression requirements;
* completion criteria.

Document precedence follows AGENTS.md section 2, clarified in `docs/decisions/ADR-0001-authority-and-module-gates.md`. This roadmap allocates delivery and acceptance; it cannot waive product or mandatory security requirements. Conflicts require an explicit documented resolution.

The roadmap must not introduce product requirements that are not defined elsewhere.

---

# 2. Source of Truth

Development decisions must be based on the repository documentation.

Primary project documents include:

```text
PROJECT_CONTEXT.md
REQUIREMENTS.md
ARCHITECTURE.md
SECURITY_REQUIREMENTS.md
TESTING_STRATEGY.md
PROJECT_STATE.md
AGENTS.md
```

Detailed technical specifications are maintained under:

```text
docs/
├── ai/
├── analytics/
├── api/
├── authentication/
├── content-processing/
├── database/
├── decisions/
├── deployment/
├── offline/
└── synchronization/
```

The module documentation under:

```text
modules/
```

provides implementation-specific planning and execution detail.

Chat history is not a substitute for repository documentation.

---

# 3. Development Philosophy

CRC is developed as a controlled vertical system rather than as a collection of disconnected features.

The development cycle is:

```text
Understand
    ↓
Document
    ↓
Architect
    ↓
Plan
    ↓
Implement
    ↓
Test
    ↓
Integrate
    ↓
Regression Test
    ↓
Document
    ↓
Freeze
    ↓
Next Module
```

A module is not considered complete merely because its code compiles or its primary UI exists.

---

# 4. Development Principles

Development must follow these principles:

1. Build against documented requirements.
2. Follow the approved architecture.
3. Do not guess unresolved business requirements.
4. Make technical decisions where the architecture permits technical discretion.
5. Do not implement future functionality prematurely.
6. Keep domain boundaries explicit.
7. Keep authentication separate from authorization.
8. Enforce authorization server-side.
9. Preserve historical academic data.
10. Treat offline operation as a first-class requirement.
11. Keep synchronization explicit and durable.
12. Keep AI separate from deterministic business logic.
13. Test changes before integration.
14. Run regression tests after significant changes.
15. Update documentation when implementation changes the approved design.
16. Do not silently change established architectural decisions.
17. Record significant architectural changes through the decisions process.

---

# 5. Development Units

The project is divided into ten major implementation modules:

```text
00 Foundation & Engineering Infrastructure
01 Identity, Account Provisioning & Access Control
02 Academic Core & Content Management
03 Student CBT & Practice Engine
04 Teacher Dashboard & Assessment Management
05 Offline School System & Synchronization
06 Content Processing & Academic Material Intelligence
07 Local Academic AI
08 Analytics, Learning Gaps & Management Intelligence
09 Security, Deployment, Hardening & Acceptance
```

The numbering represents the intended development sequence.

It does not mean every implementation task inside a module must be completed in strict file-by-file order.

---

# 6. Module Dependency Model

The high-level dependency chain is:

```text
00 Foundation
      ↓
01 Identity
      ↓
02 Academic Core
      ↓
03 Student CBT
      ↓
04 Teacher
      ↓
05 Offline & Synchronization
      ↓
06 Content Processing
      ↓
07 Local AI
      ↓
08 Analytics
      ↓
09 Security, Deployment & Acceptance
```

Cross-module acceptance is allocated explicitly in ADR-0001. A module freezes only its fully tested allocated baseline; whole-system architecture definitions of done remain open until all required integrations pass. Local host/SQLite start in 00, local identity in 01 and real offline CBT in 03; 05 adds managed installation/synchronization. Teacher derived analytics and analytics-grounded AI integration complete in 08. No later obligation may be recorded as passed at an earlier freeze.

The owner-approved Module 00 gate exception dated 2026-10-02 in ADR-0001 permits conditional progression through Modules 01-04 while F00-02/F00-13/F00-17 evidence is deferred. Module 00 remains NOT FROZEN; those criteria must pass and Module 00 must freeze before Module 05 starts. Other module decisions, authorization, regression and acceptance gates remain unchanged.

The dependency chain must not be interpreted as permission to bypass required prerequisites.

---

# 7. Module 00 — Foundation & Engineering Infrastructure

## Objective

Establish the technical foundation required for the rest of the system.

## Scope

This module establishes the project engineering foundation, including the infrastructure required for:

* application structure;
* development environments;
* configuration management;
* database access;
* API foundations;
* logging;
* error handling;
* testing infrastructure;
* migrations;
* code quality;
* observability foundations;
* shared technical utilities.

The foundation choices are established in ADR-0002. The executable scope and evidence gates are `modules/00-foundation/MODULE_PLAN.md` and `modules/00-foundation/ACCEPTANCE_CRITERIA.md`; these include both Online/PostgreSQL and Local/SQLite foundations, without future business functionality.

## Dependencies

None.

## Completion Gate

Module 00 must establish a stable foundation on which subsequent modules can be developed without repeatedly rebuilding core infrastructure.

## Required Validation

* application starts correctly;
* test infrastructure operates;
* database connectivity works;
* migrations operate;
* logging operates;
* errors are handled consistently;
* configuration is environment-aware;
* development workflow is reproducible.

---

# 8. Module 01 — Identity, Account Provisioning & Access Control

## Objective

Implement the identity and access foundation for all five approved V1 roles.

## Roles

```text
Super Admin / IT
Principal
Teacher
Parent
Student
```

## Scope

This module includes:

* user accounts;
* account states;
* Super Admin provisioning;
* first-login password change;
* authentication;
* sessions;
* logout;
* password management;
* password reset;
* role-based access control;
* server-side authorization;
* object-level authorization;
* parent-student relationships;
* teacher assignment-based access;
* principal management access;
* administrative account management;
* audit events related to identity.

No public self-registration is introduced.

MFA is not required for V1.

## Dependencies

Module 00.

## Completion Gate

All five roles must be able to authenticate and access only the resources permitted to them.

## Required Validation

* valid login;
* invalid login;
* first-login password change;
* logout;
* session expiry;
* account deactivation;
* authorization enforcement;
* parent access restrictions;
* teacher assignment restrictions;
* principal scope;
* Super Admin controls.

---

# 9. Module 02 — Academic Core & Content Management

## Objective

Implement the academic structure on which learning, assessments, analytics, and reporting depend.

## Academic Hierarchy

```text
Academic Session
    ↓
Term
    ↓
Class
    ↓
Arm
    ↓
Subject
    ↓
Topic
    ↓
Question
```

## Scope

This module includes:

* academic sessions;
* terms;
* flexible classes;
* flexible arms;
* subjects;
* topics;
* student placement;
* teacher assignments;
* parent relationships as required by the academic model;
* promotion data structures;
* question bank;
* question versions;
* question options;
* rich question content;
* publication lifecycle.

## Question Lifecycle

```text
Imported
    ↓
Processing
    ↓
Review Required
    ↓
Teacher Review/Edit
    ↓
Teacher Approval
    ↓
Published
    ↓
Available for Assessment
```

Unapproved questions must not be available for assessments.

## Dependencies

Module 01.

## Completion Gate

The academic structure and question-bank foundation must support the later CBT, teacher, content-processing, synchronization, and analytics modules without structural redesign.

---

# 10. Module 03 — Student CBT & Practice Engine

## Objective

Implement the shared assessment engine used by student Practice Mode and CBT/Exam Mode.

## Scope

The engine must support the approved assessment behavior, including:

* assessment configuration;
* configurable duration;
* configurable attempts;
* configurable question count;
* question selection;
* question randomization;
* configurable option randomization;
* forward navigation;
* backward navigation;
* skipping;
* changing answers;
* automatic answer saving;
* submission;
* deterministic scoring;
* immediate score availability;
* attempt state;
* recovery;
* watchdog behavior.

## Shared Engine Principle

Practice and CBT must use the same fundamental assessment engine and core assessment rules.

The modes may differ in configuration and operational restrictions, but the underlying assessment mechanics must not be duplicated unnecessarily.

## CBT Recovery

The active attempt state must be authoritative on the local server during offline CBT.

The system must support recovery after:

* browser crash;
* client interruption;
* LAN interruption;
* temporary connection failure.

The server-authoritative timer must not depend solely on browser state.

## AI Restriction

AI assistance must be unavailable during CBT/Exam Mode.

## Dependencies

Modules 01 and 02.

## Completion Gate

A student must be able to complete a complete assessment offline with reliable answer persistence, recovery, and deterministic scoring.

---

# 11. Module 04 — Teacher Dashboard & Assessment Management

## Objective

Provide teachers with the tools required to manage their assigned academic work and assessments.

## Scope

This module includes:

* teacher dashboard;
* assigned classes;
* assigned subjects;
* question review;
* question editing where permitted;
* question approval;
* assessment configuration;
* assessment management;
* class performance;
* student performance;
* weak-topic visibility;
* relevant academic monitoring.

Teacher access must be restricted by the assignments configured by Super Admin.

## Dependencies

Modules 01, 02, and 03.

## Completion Gate

A teacher must be able to perform the approved assessment-management workflow for assigned classes and subjects without accessing unauthorized academic data.

---

# 12. Module 05 — Offline School System & Synchronization

## Objective

Establish the production-grade local school environment and synchronization mechanism.

## Scope

This module includes:

* Windows local server;
* `LMSServer.exe`;
* local API;
* SQLite with WAL;
* local authentication;
* LAN access;
* local server manager;
* watchdog;
* local health monitoring;
* durable synchronization queue;
* push/pull synchronization;
* acknowledgements;
* retries;
* idempotency;
* revision tracking;
* conflict detection;
* conflict resolution;
* synchronization audit trail;
* Super Admin synchronization dashboard.

## Offline Guarantee

When internet access is unavailable, critical academic operation must continue locally.

This includes:

* student login;
* practice;
* CBT;
* scoring;
* progress recording;
* local academic operation.

## Dependencies

Modules 01–04 and the approved offline/synchronization architecture.

## Completion Gate

The school-local system must continue operating correctly when the internet is unavailable and reconcile correctly when connectivity returns.

---

# 13. Module 06 — Content Processing & Academic Material Intelligence

## Objective

Implement the online content-processing pipeline for transforming source academic materials into reviewable academic content.

## Scope

The approved pipeline includes:

* PDF processing;
* DOCX processing;
* image processing;
* OCR;
* layout analysis;
* text extraction;
* table extraction;
* image/diagram extraction;
* question extraction;
* answer extraction;
* normalization;
* processing status;
* teacher review;
* teacher editing;
* teacher approval.

The content-processing system operates online.

Ollama is not used as the deterministic content-normalization engine.

## Dependencies

Module 02 and the online infrastructure established by earlier modules.

## Completion Gate

Uploaded academic material can be processed into reviewable content while preserving the review and approval lifecycle.

---

# 14. Module 07 — Local Academic AI

## Objective

Provide controlled local AI capabilities using Ollama.

## Scope

Approved AI use cases include:

* student tutoring;
* question explanations;
* study assistance;
* teacher academic intelligence;
* principal/management academic intelligence;
* learning-gap interpretation.

## RAG

The approved architecture is:

```text
Approved Content
      ↓
Chunking
      ↓
Embeddings
      ↓
Local Vector Index
      ↓
Relevant Context Retrieval
      ↓
Ollama
      ↓
Response
```

## AI Restrictions

AI must not:

* score examinations;
* determine official correctness;
* change grades;
* modify student records;
* modify permissions;
* publish questions;
* control synchronization;
* replace deterministic business logic.

AI is disabled during CBT/Exam Mode.

## Conversation Storage

Conversation storage must remain efficient and retrieval-friendly.

Repeated context should not be stored unnecessarily with every message.

## Dependencies

Modules 01, 02, 05, and 06 where approved content is involved.

## Completion Gate

AI operates through the controlled AI Gateway, respects authorization boundaries, uses approved context appropriately, and cannot mutate deterministic LMS state.

---

# 15. Module 08 — Analytics, Learning Gaps & Management Intelligence

## Objective

Provide role-appropriate academic analytics and learning-gap intelligence.

## Scope

### Student

* progress;
* weak topics;
* practice history.

### Teacher

* class performance;
* weak students;
* topic gaps.

### Parent

* child performance;
* progress;
* weak subjects.

### Principal

* school performance;
* class comparison;
* learning gaps.

### Super Admin

* system-level operational and academic visibility where authorized.

## Analytics Principles

Analytics are derived from authoritative academic records.

They must not replace authoritative assessment, academic, or identity data.

## Dependencies

Modules 01–05 and the data generated by subsequent academic workflows.

## Completion Gate

Analytics must be traceable to authoritative records and preserve historical academic integrity.

---

# 16. Module 09 — Security, Deployment, Hardening & Acceptance

## Objective

Prepare the complete system for controlled production deployment.

## Scope

This module includes:

* security hardening;
* OWASP ASVS-aligned verification;
* dependency/security checks;
* application security testing;
* local server hardening;
* deployment validation;
* backup verification;
* restore verification;
* monitoring;
* logging;
* operational readiness;
* penetration testing;
* acceptance testing;
* production commissioning.

## Penetration Testing

Security validation must include appropriate penetration testing before production acceptance.

The penetration test is part of the security acceptance process.

## Completion Gate

The system is not production-ready until all mandatory security, deployment, recovery, testing, and acceptance requirements have passed.

---

# 17. Cross-Module Development Rules

Every module must account for the technical layers affected by its functionality.

Where applicable, implementation must consider:

```text
Frontend
Backend/API
Database
Authentication
Authorization
Offline Behavior
Synchronization
Audit
Observability
Testing
Documentation
```

Not every module requires every layer, but affected layers must be addressed explicitly.

---

# 18. Module Lifecycle

Every module follows the same lifecycle.

## Phase 1 — Understand

Review:

* requirements;
* architecture;
* security requirements;
* related technical specifications;
* existing implementation;
* dependencies.

No implementation begins until the scope is understood.

---

## Phase 2 — Document

Define:

* module objective;
* functional requirements;
* technical boundaries;
* data requirements;
* API requirements;
* UI requirements;
* security requirements;
* testing requirements;
* integration requirements.

---

## Phase 3 — Architect

Confirm:

* component boundaries;
* data ownership;
* API boundaries;
* authorization;
* persistence;
* failure behavior;
* offline behavior where applicable;
* synchronization behavior where applicable.

Architectural changes must be documented through the appropriate decision process.

---

## Phase 4 — Plan

Break the module into implementation work.

The plan must preserve the module's boundaries and dependencies.

Do not create unrelated work simply because it is technically convenient.

---

## Phase 5 — Implement

Implementation must follow the approved plan.

Code must remain within the current module unless an explicit dependency or shared foundation requires a change elsewhere.

---

## Phase 6 — Test

Run appropriate:

* unit tests;
* integration tests;
* API tests;
* database tests;
* frontend tests;
* end-to-end tests;
* security tests;
* offline tests;
* synchronization tests.

The exact tests depend on the module.

---

## Phase 7 — Integrate

Integrate the completed functionality with existing modules.

Integration must verify:

* API contracts;
* database contracts;
* authentication;
* authorization;
* events;
* synchronization;
* UI behavior;
* error handling.

---

## Phase 8 — Regression Test

Existing functionality must be tested after significant changes.

A module cannot be marked complete if it introduces unresolved regressions into previously completed modules.

---

## Phase 9 — Document

Update affected documentation.

Documentation must remain consistent with the implementation.

---

## Phase 10 — Freeze

Once the module satisfies its completion criteria:

* scope is frozen;
* required tests pass;
* documentation is updated;
* known defects are recorded;
* integration is verified.

Only then should development proceed to the next major module.

---

# 19. Definition of Done

A module is complete only when all applicable conditions are satisfied.

### Requirements

* [ ] Approved requirements implemented
* [ ] No undocumented behavior added
* [ ] Prohibited behavior not implemented

### Architecture

* [ ] Approved architecture followed
* [ ] Boundaries respected
* [ ] Data ownership respected
* [ ] Authorization boundaries respected

### Implementation

* [ ] Backend complete
* [ ] Frontend complete
* [ ] Database changes complete
* [ ] API contracts complete
* [ ] Error handling complete

Only applicable layers need to be marked complete.

### Security

* [ ] Authentication verified
* [ ] Authorization verified
* [ ] Input validation verified
* [ ] Sensitive data protected
* [ ] Security logging implemented where required

### Testing

* [ ] Unit tests
* [ ] Integration tests
* [ ] End-to-end tests where applicable
* [ ] Regression tests
* [ ] Security tests where applicable

### Documentation

* [ ] Technical documentation updated
* [ ] API documentation updated where applicable
* [ ] Database documentation updated where applicable
* [ ] Operational documentation updated where applicable

### Integration

* [ ] Existing modules remain functional
* [ ] Required integrations verified
* [ ] No unresolved blocking defects

---

# 20. Regression Policy

Regression testing is mandatory throughout development.

A completed module must not be treated as isolated from the rest of the system.

Changes affecting shared infrastructure must trigger regression testing across all dependent modules.

Particular attention must be given to:

* authentication;
* authorization;
* database migrations;
* API contracts;
* assessment engine;
* offline operation;
* synchronization;
* shared UI components;
* analytics calculations.

---

# 21. Database Change Policy

Database changes must follow the approved database architecture.

Changes must:

* use migrations;
* preserve existing data;
* preserve historical academic records;
* maintain referential integrity;
* include required indexes;
* account for online PostgreSQL;
* account for local SQLite where applicable.

Production databases must never be modified manually as a normal development procedure.

---

# 22. API Change Policy

API changes must follow the API architecture.

Changes must consider:

* authentication;
* authorization;
* validation;
* versioning;
* compatibility;
* error responses;
* idempotency;
* pagination;
* synchronization;
* offline clients.

Breaking changes require explicit planning and documentation.

---

# 23. Offline Development Gate

Any functionality required to operate offline must be tested without internet access.

The test must distinguish:

```text
LAN Available + Internet Available
LAN Available + Internet Unavailable
LAN Unavailable
```

The application must behave according to the documented failure model for each condition.

---

# 24. Synchronization Development Gate

Synchronization functionality must be tested for:

* successful synchronization;
* retry;
* duplicate delivery;
* duplicate application;
* partial failure;
* conflict detection;
* conflict resolution;
* reconnect;
* revision handling;
* tombstones;
* schema compatibility.

A successful first sync is not sufficient validation.

---

# 25. Assessment Development Gate

Any change affecting assessments must verify:

* question selection;
* randomization;
* answer persistence;
* navigation;
* timer;
* submission;
* scoring;
* attempt state;
* recovery;
* offline behavior;
* CBT AI restriction.

Assessment correctness is a critical system requirement.

---

# 26. Security Development Gate

Security must be addressed during development rather than deferred entirely to the final security module.

Each module must evaluate:

* authentication;
* authorization;
* privilege boundaries;
* input validation;
* sensitive data;
* file handling;
* logging;
* error disclosure;
* dependency risk.

Module 09 performs final hardening and acceptance rather than introducing security for the first time.

---

# 27. AI Development Gate

AI functionality must remain isolated from deterministic LMS operations.

Any AI implementation must verify:

* authorization;
* approved context;
* prompt-injection resistance;
* model/resource controls;
* conversation storage;
* logging;
* failure isolation;
* CBT restriction;
* no unauthorized mutation.

AI output must not become authoritative academic data without an explicit deterministic workflow.

---

# 28. Content Processing Development Gate

Content processing must maintain the approved workflow:

```text
Upload
  ↓
Processing
  ↓
Review Required
  ↓
Teacher Review/Edit
  ↓
Teacher Approval
  ↓
Published
```

Processing output must not bypass teacher approval.

---

# 29. Promotion Development Gate

Promotion must preserve historical records.

The system must:

1. identify promotion candidates;
2. prepare the promotion operation;
3. require Super Admin activation;
4. execute the approved promotion;
5. preserve historical academic records.

Promotion must not silently occur without the required administrative activation.

---

# 30. Release Sequence

The project should progress through:

```text
Foundation Complete
       ↓
Identity Complete
       ↓
Academic Core Complete
       ↓
Assessment Engine Complete
       ↓
Teacher Workflows Complete
       ↓
Offline & Synchronization Complete
       ↓
Content Processing Complete
       ↓
Local AI Complete
       ↓
Analytics Complete
       ↓
Security & Deployment Complete
       ↓
Production Acceptance
```

Each stage depends on the successful completion of its required predecessors.

---

# 31. Integration Milestones

The project should have major integration checkpoints.

### Foundation Integration

Verify the core application, database, API, configuration, testing, and observability foundations.

### Identity Integration

Verify all five roles and authorization boundaries.

### Academic Integration

Verify academic structure, assignments, students, teachers, and question-bank foundations.

### Assessment Integration

Verify student practice and CBT workflows.

### Offline Integration

Verify local server operation, LAN access, offline authentication, CBT, recovery, and synchronization.

### Content Integration

Verify content processing into the approved question/content workflow.

### AI Integration

Verify local AI, RAG, authorization, and CBT restrictions.

### Analytics Integration

Verify derived analytics against authoritative academic data.

### Production Integration

Verify security, deployment, backup, restore, monitoring, recovery, and acceptance.

---

# 32. Change Control

Changes to requirements or architecture after implementation has begun must be evaluated before implementation.

A change should identify:

* affected requirements;
* affected architecture;
* affected modules;
* database impact;
* API impact;
* security impact;
* offline impact;
* synchronization impact;
* testing impact;
* documentation impact.

Significant architectural changes must be recorded in:

```text
docs/decisions/
```

---

# 33. Scope Protection

The following must not be added merely because they are common features in other school systems:

* school fees/payment processing;
* payment gateway;
* HR/payroll;
* admissions;
* SMS services;
* email services;
* unrelated school-management features.

These are outside the approved project scope unless requirements are formally changed.

---

# 34. Future Functionality Protection

Future functionality must not be implemented early merely because the architecture could support it.

Examples include:

* MFA;
* additional question types;
* additional AI actions;
* additional integrations;
* unrelated portals;
* additional school-management modules.

Architecture may remain extensible without implementing those capabilities.

---

# 35. Testing Strategy Alignment

All module testing must comply with:

`TESTING_STRATEGY.md`

The roadmap does not replace the testing strategy.

It determines **when and where testing gates occur** during development.

---

# 36. Security Strategy Alignment

All security implementation must comply with:

`SECURITY_REQUIREMENTS.md`

The roadmap does not replace the security requirements.

It establishes security gates throughout development and the final security acceptance stage.

---

# 37. Documentation Synchronization

When implementation changes an approved technical behavior, the affected documentation must be updated.

At minimum, review:

* project context;
* requirements;
* architecture;
* API architecture;
* database architecture;
* authentication architecture;
* offline architecture;
* synchronization architecture;
* AI architecture;
* analytics architecture;
* deployment architecture;
* security requirements;
* module documentation.

Documentation drift must be treated as a project defect.

---

# 38. Stop Conditions

Development must stop and the issue must be resolved when:

* a requirement is contradictory;
* an architectural dependency is missing;
* implementation would require guessing a business rule;
* authorization behavior is unclear;
* data ownership is unclear;
* synchronization authority is unclear;
* a migration could cause data loss;
* security requirements cannot be satisfied;
* an existing completed module would be broken;
* a critical test fails;
* required infrastructure is unavailable.

The correct response is to resolve the uncertainty, not silently invent behavior.

---

# 39. Technical Decision Principle

When a decision is purely technical and does not change product behavior, the implementation team may select the appropriate technical solution within the approved architecture.

When a decision changes:

* user behavior;
* business rules;
* product scope;
* permissions;
* academic rules;
* data ownership;
* security posture;

the decision must be explicitly resolved and documented.

---

# 40. Production Readiness Gate

Before production acceptance, the complete system must demonstrate:

### Functional Readiness

* [ ] Approved requirements implemented
* [ ] Five approved roles operational
* [ ] Academic workflows operational
* [ ] Practice operational
* [ ] CBT operational
* [ ] Teacher workflows operational
* [ ] Parent workflows operational
* [ ] Principal workflows operational
* [ ] Super Admin workflows operational

### Offline Readiness

* [ ] Local server operational
* [ ] LAN access operational
* [ ] Offline login operational
* [ ] Offline practice operational
* [ ] Offline CBT operational
* [ ] Offline scoring operational
* [ ] Recovery operational

### Synchronization Readiness

* [ ] Initial sync operational
* [ ] Incremental sync operational
* [ ] Retry operational
* [ ] Conflict handling operational
* [ ] Audit trail operational

### AI Readiness

* [ ] Ollama operational where enabled
* [ ] RAG operational where required
* [ ] Authorization verified
* [ ] CBT restriction verified
* [ ] Failure isolation verified

### Security Readiness

* [ ] Security requirements verified
* [ ] Security testing completed
* [ ] Penetration testing completed
* [ ] Findings addressed or formally accepted

### Deployment Readiness

* [ ] Production build verified
* [ ] Local installation verified
* [ ] Backup verified
* [ ] Restore verified
* [ ] Monitoring verified
* [ ] Recovery procedures verified

---

# 41. Final Acceptance

Production acceptance requires evidence that the complete system satisfies:

```text
Requirements
    +
Architecture
    +
Implementation
    +
Testing
    +
Security
    +
Offline Operation
    +
Synchronization
    +
Backup & Recovery
    +
Deployment
    +
Documentation
```

A feature being implemented is not equivalent to the system being accepted.

---

# 42. Roadmap Completion Principle

The CRC development roadmap is complete when the implementation has progressed through all approved modules and the resulting system has passed the required functional, integration, security, offline, synchronization, recovery, deployment, and acceptance gates.

The project must prioritize **correctness, security, reliability, maintainability, and documented behavior over development speed**.

> **Build each module completely, verify it, integrate it, protect it against regression, document it, freeze it, and only then move to the next major module, subject only to the explicit Module 00 timing exception in ADR-0001.**
