# NAKAMA — Christian Royal College Integrated Academic System

# Requirements Specification

## 1. Document Purpose

This document defines the functional and non-functional requirements for the NAKAMA system.

It converts the product context into precise, testable requirements.

The requirements in this document define:

* What the system must do
* What users must be able to do
* What the system must prevent
* How offline operation must behave
* How synchronization must behave
* How assessment must behave
* How AI must behave
* How security must behave
* How the system must be validated

Requirements are identified using stable requirement IDs so they can be referenced by implementation tasks, tests, issues, ADRs, and acceptance criteria.

---

# 2. Requirement Priority

Each requirement has one of the following priorities:

| Priority | Meaning                                                   |
| -------- | --------------------------------------------------------- |
| MUST     | Required for the system to satisfy its intended behavior  |
| SHOULD   | Required unless a documented technical reason prevents it |
| MAY      | Optional enhancement that does not block the core system  |

V1 implementation must satisfy all `MUST` requirements.

---

# 3. System Actors

NAKAMA V1 contains five primary application roles:

1. Super Admin / IT
2. Principal
3. Teacher
4. Parent
5. Student

Additional internal technical services are treated as system actors where necessary:

* Online Master System
* Local Server
* Synchronization Engine
* Content Processing Engine
* Local AI Service
* Watchdog/Recovery Service

---

# 4. Identity and Account Requirements

## ID-001 — No Public Registration

**Priority:** MUST

The system shall not provide public self-registration.

All user accounts shall be created or provisioned by an authorized Super Admin.

---

## ID-002 — Supported Roles

**Priority:** MUST

The system shall support exactly these V1 application roles:

* Super Admin
* Principal
* Teacher
* Parent
* Student

The system shall not introduce additional application roles without an approved requirements/architecture change.

---

## ID-003 — Super Admin Account Management

**Priority:** MUST

A Super Admin shall be able to:

* Create accounts
* Edit accounts
* Activate accounts
* Deactivate accounts
* Reset passwords
* Assign roles
* Manage academic relationships
* View account status

---

## ID-004 — First Login Password Change

**Priority:** MUST

Student, Teacher, and Parent accounts provisioned with an initial password shall be required to change that password on first login.

The system shall prevent normal application use until the required password change is completed.

---

## ID-005 — Secure Password Storage

**Priority:** MUST

Passwords shall never be stored in plaintext.

Passwords shall be stored using a modern password hashing algorithm with an appropriate work factor.

---

## ID-006 — Account Deactivation

**Priority:** MUST

A deactivated account shall not be able to authenticate successfully.

Deactivation shall not delete historical academic records associated with the account.

---

## ID-007 — Password Reset

**Priority:** MUST

An authorized Super Admin shall be able to reset a user's password.

Password-reset operations shall be auditable.

---

## ID-008 — Role Enforcement

**Priority:** MUST

Role permissions shall be enforced server-side.

Client-side visibility shall never be treated as the security boundary.

---

## ID-009 — MFA

**Priority:** MUST

MFA shall not be required for V1.

The architecture should avoid preventing the future introduction of MFA.

---

# 5. Student Identity Requirements

## STU-001 — Student Identity

**Priority:** MUST

Each student shall have a unique student identifier.

The student identifier shall be usable for identifying the student within the academic system.

---

## STU-002 — Student Academic Placement

**Priority:** MUST

A student shall belong to an active academic class/arm assignment.

---

## STU-003 — Historical Placement

**Priority:** MUST

Changes to a student's current class/arm shall not destroy historical academic records.

---

## STU-004 — Student Dashboard

**Priority:** MUST

Students shall have access to a dashboard showing appropriate academic information including:

* Progress
* Practice history
* Assessment activity
* Results
* Weak topics
* Approved academic content

---

# 6. Parent Requirements

## PAR-001 — Parent Provisioning

**Priority:** MUST

Parent accounts shall be provisioned by the Super Admin.

---

## PAR-002 — Explicit Parent-Student Relationship

**Priority:** MUST

Parent-student relationships shall be explicitly stored.

The system shall not infer parent relationships from names, email addresses, classes, or other indirect information.

---

## PAR-003 — One Parent Per Student

**Priority:** MUST

A student shall be associated with no more than one parent account in V1.

---

## PAR-004 — Multiple Students Per Parent

**Priority:** MUST

A parent account shall be able to have multiple explicitly linked students.

---

## PAR-005 — Parent Access Isolation

**Priority:** MUST

A parent shall only be able to access information belonging to students explicitly linked to that parent account.

---

## PAR-006 — Parent Dashboard

**Priority:** MUST

The parent dashboard shall provide appropriate access to:

* Child performance
* Child progress
* Weak subjects

---

# 7. Teacher Requirements

## TCH-001 — Teacher Provisioning

**Priority:** MUST

Teacher accounts shall be provisioned by the Super Admin.

---

## TCH-002 — Teacher Assignment

**Priority:** MUST

The Super Admin shall be able to assign teachers to:

* Subjects
* Classes
* Arms

---

## TCH-003 — Multiple Assignments

**Priority:** MUST

A teacher may have multiple class and subject assignments.

---

## TCH-004 — Assignment-Based Visibility

**Priority:** MUST

Teacher dashboards and APIs shall expose only the academic data the teacher is authorized to access through their assignments.

---

## TCH-005 — Teacher Dashboard

**Priority:** MUST

The teacher dashboard shall support appropriate visibility into:

* Assigned classes
* Assigned subjects
* Class performance
* Student performance
* Weak students
* Topic gaps
* Assessment activity
* Question/content review

---

# 8. Principal Requirements

## PRN-001 — Dedicated Principal Role

**Priority:** MUST

The Principal shall have a distinct application role.

---

## PRN-002 — Principal Dashboard

**Priority:** MUST

The Principal shall have a dedicated management dashboard.

The dashboard shall provide appropriate access to:

* School performance
* Class comparison
* Learning gaps
* Academic trends
* High-level academic intelligence

---

## PRN-003 — Principal/Super Admin Separation

**Priority:** MUST

Principal access shall not automatically grant Super Admin privileges.

---

# 9. Super Admin Requirements

## ADM-001 — Administrative Dashboard

**Priority:** MUST

The Super Admin shall have a dedicated administrative dashboard.

---

## ADM-002 — User Management

**Priority:** MUST

The Super Admin shall be able to manage application accounts.

---

## ADM-003 — Academic Configuration

**Priority:** MUST

The Super Admin shall be able to configure the academic structure.

---

## ADM-004 — Teacher Assignment Management

**Priority:** MUST

The Super Admin shall be able to create, update, and remove teacher assignments.

---

## ADM-005 — CBT Configuration

**Priority:** MUST

The Super Admin shall be able to configure CBT parameters.

---

## ADM-006 — Promotion Control

**Priority:** MUST

The Super Admin shall be able to activate the student promotion process.

---

## ADM-007 — System Health

**Priority:** MUST

The Super Admin shall be able to view system-health information appropriate to their environment.

---

## ADM-008 — Synchronization Monitoring

**Priority:** MUST

The Super Admin shall be able to monitor synchronization.

---

# 10. Academic Structure Requirements

## ACAD-001 — Academic Session

**Priority:** MUST

The system shall support academic sessions.

---

## ACAD-002 — Terms

**Priority:** MUST

An academic session shall support terms.

---

## ACAD-003 — Classes

**Priority:** MUST

The system shall support configurable classes.

---

## ACAD-004 — Arms

**Priority:** MUST

Classes shall support configurable arms.

---

## ACAD-005 — Subjects

**Priority:** MUST

The system shall support subjects associated with academic structures.

---

## ACAD-006 — Topics

**Priority:** MUST

Subjects shall support topics.

---

## ACAD-007 — Questions

**Priority:** MUST

Topics shall support questions.

---

## ACAD-008 — No Hardcoded Academic Structure

**Priority:** MUST

The application shall not hardcode the school's current class/arm/stream structure.

---

## ACAD-009 — Historical Academic Context

**Priority:** MUST

Academic records shall retain their historical session, term, class, arm, subject, and relevant assessment context.

---

# 11. Academic Promotion Requirements

## PROM-001 — Promotion Detection

**Priority:** MUST

The system shall be able to identify students requiring promotion at the appropriate academic-session transition.

---

## PROM-002 — Promotion Preview

**Priority:** MUST

The system shall provide an administrative preview of the proposed promotion operation before execution.

---

## PROM-003 — Explicit Activation

**Priority:** MUST

Promotion shall not execute automatically without Super Admin activation.

---

## PROM-004 — Manual Promotion

**Priority:** MUST

The Super Admin shall be able to manually promote students.

---

## PROM-005 — Historical Preservation

**Priority:** MUST

Promotion shall not modify or delete historical academic results.

---

## PROM-006 — Configurable Progression

**Priority:** MUST

Class/arm progression rules shall be configurable rather than hardcoded.

---

# 12. Question Bank Requirements

## QST-001 — MCQ Support

**Priority:** MUST

V1 shall support Multiple Choice Questions.

---

## QST-002 — Data-Driven Questions

**Priority:** MUST

Questions shall be stored as data.

Questions shall not be embedded directly into application source code.

---

## QST-003 — Rich Question Content

**Priority:** MUST

Questions shall support:

* Rich text
* Images
* Diagrams
* Mathematical notation
* Mathematical expressions
* Structured content where necessary

---

## QST-004 — Question Options

**Priority:** MUST

Each MCQ shall support configurable answer options.

---

## QST-005 — Correct Answer

**Priority:** MUST

Each published MCQ shall have an authoritative correct-answer definition.

---

## QST-006 — Question Import

**Priority:** MUST

Teachers shall be able to import questions using the supported import mechanism.

---

## QST-007 — Import Processing

**Priority:** MUST

Imported questions shall enter a processing/review lifecycle.

---

## QST-008 — Review Required

**Priority:** MUST

Imported or machine-processed questions shall require appropriate teacher review before publication.

---

## QST-009 — Question Approval

**Priority:** MUST

A teacher shall be able to approve a reviewed question.

---

## QST-010 — Publication Control

**Priority:** MUST

Only approved/published questions shall be eligible for assessment use.

---

## QST-011 — Unapproved Question Protection

**Priority:** MUST

Unapproved questions shall not appear in student assessment question pools.

---

# 13. Question Lifecycle Requirements

The system shall support the following logical states:

```text id="b70t9a"
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

## QST-012 — State Integrity

**Priority:** MUST

Question state transitions shall be controlled by the appropriate authorized actions.

---

# 14. Practice Requirements

## PRA-001 — Practice Mode

**Priority:** MUST

Students shall be able to complete practice assessments.

---

## PRA-002 — Shared Assessment Engine

**Priority:** MUST

Practice shall use the same fundamental assessment engine as CBT.

---

## PRA-003 — Practice Navigation

**Priority:** MUST

Students shall be able to:

* Move forward
* Move backward
* Skip questions
* Change answers

---

## PRA-004 — Practice Answer Saving

**Priority:** MUST

Answers shall be automatically persisted.

---

## PRA-005 — Practice Scoring

**Priority:** MUST

The system shall calculate practice scores deterministically.

---

# 15. CBT Requirements

## CBT-001 — CBT Availability

**Priority:** MUST

Students shall be able to participate in configured CBT assessments.

---

## CBT-002 — Configurable Duration

**Priority:** MUST

CBT duration shall be configurable.

---

## CBT-003 — Configurable Attempts

**Priority:** MUST

The number of permitted attempts shall be configurable.

---

## CBT-004 — Configurable Question Count

**Priority:** MUST

The number of questions presented shall be configurable.

---

## CBT-005 — Question Randomization

**Priority:** MUST

The assessment engine shall support question randomization.

---

## CBT-006 — Option Randomization

**Priority:** MUST

The assessment engine shall support option randomization where configured.

---

## CBT-007 — Navigation

**Priority:** MUST

Students shall be able to navigate backward and forward.

---

## CBT-008 — Skip

**Priority:** MUST

Students shall be able to skip questions.

---

## CBT-009 — Change Answer

**Priority:** MUST

Students shall be able to change previously selected answers while the assessment remains active and the rules permit it.

---

## CBT-010 — Automatic Saving

**Priority:** MUST

Answers shall be automatically saved during an active assessment.

---

## CBT-011 — Submission

**Priority:** MUST

Students shall be able to submit their assessment.

---

## CBT-012 — Automatic Completion

**Priority:** MUST

The system shall handle assessment expiration when the configured duration ends.

---

## CBT-013 — Immediate Score

**Priority:** MUST

The student shall receive the calculated score immediately after submission where the assessment configuration permits immediate result display.

---

# 16. CBT State Requirements

## CBT-014 — Authoritative Attempt State

**Priority:** MUST

The local server shall be authoritative for active CBT attempt state during offline operation.

---

## CBT-015 — Attempt Identity

**Priority:** MUST

Each active CBT attempt shall have a unique identifier.

---

## CBT-016 — Attempt Persistence

**Priority:** MUST

Active attempt state shall survive temporary client interruptions.

---

## CBT-017 — Timer Authority

**Priority:** MUST

The authoritative assessment timer shall be derived from server-side attempt state rather than relying exclusively on the browser clock.

---

# 17. CBT Recovery Requirements

## REC-001 — Browser Crash Recovery

**Priority:** MUST

A student shall be able to resume an active assessment after an unexpected browser crash where the attempt remains valid.

---

## REC-002 — Temporary LAN Interruption

**Priority:** MUST

A temporary LAN interruption shall not automatically invalidate an active assessment.

---

## REC-003 — Reconnection

**Priority:** MUST

The system shall detect reconnection and restore the student's active attempt where valid.

---

## REC-004 — Answer Reconciliation

**Priority:** MUST

The recovery process shall reconcile client-side recovery data with server-side attempt state.

---

## REC-005 — Duplicate Attempt Prevention

**Priority:** MUST

Recovery shall not create an unintended duplicate active attempt.

---

## REC-006 — Watchdog

**Priority:** MUST

The local system shall provide a watchdog/recovery mechanism for active CBT sessions.

---

# 18. Offline Requirements

## OFF-001 — Offline Authentication

**Priority:** MUST

Students and required local users shall be able to authenticate against the local system without internet access.

---

## OFF-002 — Offline Practice

**Priority:** MUST

Practice shall operate without internet access.

---

## OFF-003 — Offline CBT

**Priority:** MUST

CBT shall operate without internet access.

---

## OFF-004 — Offline Scoring

**Priority:** MUST

Scoring shall operate without internet access.

---

## OFF-005 — Offline Progress

**Priority:** MUST

Student progress shall be recorded locally without internet access.

---

## OFF-006 — Offline Academic Content

**Priority:** MUST

Required approved academic content shall be available locally where required for offline operation.

---

## OFF-007 — Internet Independence

**Priority:** MUST

Loss of internet connectivity shall not prevent critical student academic operations.

---

# 19. Local Server Requirements

## SRV-001 — Windows Support

**Priority:** MUST

The school local server application shall support the designated Windows server environment.

---

## SRV-002 — Executable Distribution

**Priority:** MUST

The local server shall be distributable as a Windows executable/application installation.

---

## SRV-003 — Server Manager

**Priority:** MUST

The local environment shall include a server-management interface.

---

## SRV-004 — Start/Stop

**Priority:** MUST

Authorized administrators shall be able to start and stop local services.

---

## SRV-005 — Restart

**Priority:** MUST

Authorized administrators shall be able to restart required local services.

---

## SRV-006 — Server Status

**Priority:** MUST

The server manager shall show service status.

---

## SRV-007 — Network Information

**Priority:** MUST

The server manager shall expose relevant LAN address and port information.

---

## SRV-008 — Connected Devices

**Priority:** MUST

The server manager shall provide appropriate visibility into connected client devices.

---

## SRV-009 — Database Health

**Priority:** MUST

The server manager shall provide database-health information.

---

## SRV-010 — AI Health

**Priority:** MUST

The server manager shall provide AI-service status.

---

## SRV-011 — Sync Health

**Priority:** MUST

The server manager shall provide synchronization status.

---

## SRV-012 — Logs

**Priority:** MUST

The server manager shall provide access to relevant local logs.

---

## SRV-013 — Configuration

**Priority:** MUST

Authorized administrators shall be able to manage supported local-server configuration.

---

# 20. Server Configuration Requirements

The server configuration model shall support appropriate settings including:

* LAN binding/interface
* Port
* Database location
* Backup location
* Backup schedule
* Online master URL
* Synchronization settings
* AI settings
* AI model configuration
* Logging settings
* Storage settings
* Session settings
* Startup behavior

## SRV-014 — Secret Protection

**Priority:** MUST

Sensitive credentials and secrets shall not be displayed as plaintext in administrative configuration screens or logs.

---

# 21. Network Security Requirements

## NET-001 — LAN-Only Exposure

**Priority:** MUST

The local server shall not require public internet exposure for normal school operation.

---

## NET-002 — Firewall

**Priority:** MUST

The deployment shall use host/network firewall controls to restrict access to required services.

---

## NET-003 — Interface Binding

**Priority:** MUST

Services shall bind only to required interfaces.

---

## NET-004 — No Unnecessary Ports

**Priority:** MUST

The deployment shall minimize exposed ports and services.

---

## NET-005 — Secure Local Naming

**Priority:** SHOULD

The deployment should use a private LAN hostname/local DNS or equivalent controlled naming mechanism rather than exposing a public hostname.

---

## NET-006 — Transport Security

**Priority:** SHOULD

LAN traffic carrying credentials, sessions, or sensitive academic information should use TLS/HTTPS where practical within the deployment architecture.

---

# 22. Synchronization Requirements

## SYNC-001 — Bidirectional Synchronization

**Priority:** MUST

The system shall support synchronization of relevant data between the online master environment and local school environment.

---

## SYNC-002 — Change Tracking

**Priority:** MUST

Synchronizable records shall have sufficient change-tracking metadata.

---

## SYNC-003 — Versioning

**Priority:** MUST

The synchronization system shall support record versioning/revision tracking.

---

## SYNC-004 — Idempotency

**Priority:** MUST

Repeated delivery of the same synchronization operation shall not create duplicate logical results.

---

## SYNC-005 — Retry

**Priority:** MUST

Failed synchronization operations shall be retryable.

---

## SYNC-006 — Queue

**Priority:** MUST

Pending synchronization operations shall be represented in a durable synchronization queue or equivalent mechanism.

---

## SYNC-007 — Conflict Detection

**Priority:** MUST

The synchronization system shall detect conflicts rather than silently overwriting conflicting data.

---

## SYNC-008 — Conflict Resolution

**Priority:** MUST

The synchronization system shall use an explicit conflict-resolution strategy.

The strategy shall be defined per relevant data category where necessary.

---

## SYNC-009 — Sync Audit

**Priority:** MUST

Synchronization operations shall be auditable.

---

## SYNC-010 — Sync Status

**Priority:** MUST

The system shall expose synchronization status to authorized administrators.

---

## SYNC-011 — Last Successful Sync

**Priority:** MUST

The system shall record and display the last successful synchronization time.

---

## SYNC-012 — Failed Sync

**Priority:** MUST

The system shall record failed synchronization attempts and relevant errors.

---

## SYNC-013 — Pending Changes

**Priority:** MUST

The system shall show pending synchronization operations.

---

## SYNC-014 — Retry Status

**Priority:** MUST

The system shall provide visibility into synchronization retry status.

---

# 23. Data Authority Requirements

## DATA-001 — Active Local CBT Authority

**Priority:** MUST

The local server shall be authoritative for active offline CBT attempts.

---

## DATA-002 — Online Master

**Priority:** MUST

The online environment shall serve as the master synchronized environment for appropriate central academic data.

---

## DATA-003 — Entity-Level Authority

**Priority:** MUST

The synchronization architecture shall explicitly define authority and conflict behavior for each synchronizable entity where authority can differ.

---

# 24. Content Processing Requirements

## CONT-001 — Online Processing

**Priority:** MUST

Content processing shall occur in the online environment.

---

## CONT-002 — PDF

**Priority:** MUST

The content-processing system shall support PDF input.

---

## CONT-003 — DOCX

**Priority:** MUST

The content-processing system shall support DOCX input.

---

## CONT-004 — Image Input

**Priority:** MUST

The content-processing system shall support image input.

---

## CONT-005 — OCR

**Priority:** MUST

The content-processing pipeline shall support OCR.

---

## CONT-006 — Layout Analysis

**Priority:** MUST

The processing pipeline shall support appropriate layout analysis.

---

## CONT-007 — Structure Extraction

**Priority:** MUST

The system shall be capable of extracting relevant document structures.

---

## CONT-008 — Question Extraction

**Priority:** MUST

The processing pipeline shall support extraction of question/option structures where applicable.

---

## CONT-009 — Human Review

**Priority:** MUST

Machine-processed content shall support human review before publication.

---

# 25. Local AI Requirements

## AI-001 — Ollama

**Priority:** MUST

The local academic AI environment shall use Ollama as the local model-serving layer.

---

## AI-002 — Student Tutoring

**Priority:** MUST

Students shall be able to use approved AI tutoring functionality where enabled.

---

## AI-003 — Explanation

**Priority:** MUST

The AI shall be able to provide explanations for academic content where sufficient approved context exists.

---

## AI-004 — Study Assistance

**Priority:** MUST

The AI shall support appropriate study-assistance interactions.

---

## AI-005 — Teacher Intelligence

**Priority:** SHOULD

Teachers should receive AI-assisted academic intelligence.

---

## AI-006 — Principal Intelligence

**Priority:** SHOULD

The Principal should receive AI-assisted academic intelligence.

---

## AI-007 — Learning-Gap Interpretation

**Priority:** SHOULD

The AI may assist in interpreting learning gaps from authoritative analytics.

---

## AI-008 — Approved Context

**Priority:** MUST

The AI should prioritize approved school content and authorized academic context.

---

## AI-009 — CBT Disablement

**Priority:** MUST

AI assistance shall be unavailable during formal CBT/examination mode.

---

## AI-010 — Business Logic Separation

**Priority:** MUST

AI shall not control deterministic business rules.

---

## AI-011 — Grade Protection

**Priority:** MUST

AI shall not modify authoritative grades or assessment scores.

---

## AI-012 — Permission Protection

**Priority:** MUST

AI shall not modify roles, permissions, or account access.

---

## AI-013 — Publication Protection

**Priority:** MUST

AI shall not independently publish academic questions or materials.

---

## AI-014 — Synchronization Protection

**Priority:** MUST

AI shall not control authoritative synchronization operations.

---

# 26. AI Retrieval Requirements

## RAG-001 — Content Chunking

**Priority:** MUST

Approved content shall be transformed into retrieval-appropriate chunks.

---

## RAG-002 — Embeddings

**Priority:** MUST

Retrieval content shall support embeddings.

---

## RAG-003 — Local Retrieval

**Priority:** MUST

Relevant content shall be retrievable locally.

---

## RAG-004 — Context Injection

**Priority:** MUST

Relevant retrieved context shall be supplied to the AI model when appropriate.

---

## RAG-005 — Storage Efficiency

**Priority:** MUST

The AI storage model shall avoid unnecessary duplication of large content/context payloads.

---

## RAG-006 — Retrieval Performance

**Priority:** SHOULD

Retrieval should remain responsive on the target local server hardware.

---

# 27. Analytics Requirements

## ANA-001 — Student Progress

**Priority:** MUST

The system shall calculate and present appropriate student progress information.

---

## ANA-002 — Weak Topics

**Priority:** MUST

The system shall identify appropriate weak-topic indicators.

---

## ANA-003 — Practice History

**Priority:** MUST

The system shall retain practice history.

---

## ANA-004 — Class Performance

**Priority:** MUST

The system shall support class-level performance analytics.

---

## ANA-005 — Student Performance

**Priority:** MUST

The system shall support student-level performance analytics for authorized users.

---

## ANA-006 — Learning Gaps

**Priority:** MUST

The system shall support learning-gap identification.

---

## ANA-007 — Principal Analytics

**Priority:** MUST

The Principal shall have access to appropriate school-level academic analytics.

---

# 28. Audit Requirements

## AUD-001 — Authentication Events

**Priority:** MUST

Important authentication events shall be logged.

---

## AUD-002 — Administrative Changes

**Priority:** MUST

Important administrative changes shall be auditable.

---

## AUD-003 — Academic Changes

**Priority:** MUST

Important academic configuration and content changes shall be auditable.

---

## AUD-004 — Assessment Events

**Priority:** MUST

Relevant assessment lifecycle events shall be auditable.

---

## AUD-005 — Promotion

**Priority:** MUST

Promotion operations shall be auditable.

---

## AUD-006 — Synchronization

**Priority:** MUST

Synchronization operations shall be auditable.

---

## AUD-007 — Security Events

**Priority:** MUST

Relevant security events shall be logged.

---

# 29. Security Requirements

## SEC-001 — OWASP-Aligned Security

**Priority:** MUST

The application shall follow a recognized application-security baseline aligned with OWASP practices.

---

## SEC-002 — Authorization

**Priority:** MUST

Every protected server-side operation shall enforce authorization.

---

## SEC-003 — Input Validation

**Priority:** MUST

Untrusted input shall be validated.

---

## SEC-004 — Injection Protection

**Priority:** MUST

The system shall use appropriate controls against SQL injection and other injection attacks.

---

## SEC-005 — XSS Protection

**Priority:** MUST

The system shall implement appropriate cross-site scripting protections.

---

## SEC-006 — CSRF Protection

**Priority:** MUST

CSRF protections shall be applied where the authentication/session architecture makes CSRF relevant.

---

## SEC-007 — Security Headers

**Priority:** MUST

Appropriate security headers shall be configured.

---

## SEC-008 — Content Security Policy

**Priority:** SHOULD

A restrictive Content Security Policy should be implemented where technically compatible with the application.

---

## SEC-009 — File Upload Security

**Priority:** MUST

Uploaded files shall be validated and securely handled.

---

## SEC-010 — Secrets

**Priority:** MUST

Secrets shall not be committed to source control.

---

## SEC-011 — Error Handling

**Priority:** MUST

Production errors shall not unnecessarily expose secrets, credentials, stack traces, or internal implementation details.

---

## SEC-012 — Dependency Security

**Priority:** MUST

Dependencies shall be monitored for known vulnerabilities.

---

## SEC-013 — Penetration Testing

**Priority:** MUST

The completed system shall undergo penetration testing before production acceptance.

---

## SEC-014 — Security Audit

**Priority:** MUST

A formal security review/audit shall be completed before final production acceptance.

---

# 30. Database Requirements

## DB-001 — PostgreSQL Online

**Priority:** MUST

The online production environment shall use PostgreSQL.

---

## DB-002 — SQLite Local

**Priority:** MUST

The local server shall use SQLite.

---

## DB-003 — WAL

**Priority:** MUST

The local SQLite database shall use WAL mode.

---

## DB-004 — Data Integrity

**Priority:** MUST

Database constraints and application logic shall enforce critical data-integrity rules.

---

## DB-005 — Historical Preservation

**Priority:** MUST

Historical academic records shall not be destroyed by ordinary lifecycle operations.

---

# 31. Reliability Requirements

## REL-001 — Graceful Failure

**Priority:** MUST

Non-critical service failures shall not unnecessarily terminate core academic operation.

---

## REL-002 — Recovery

**Priority:** MUST

The local system shall support recovery from appropriate service interruptions.

---

## REL-003 — Durable Academic State

**Priority:** MUST

Critical assessment and academic state shall be durably stored.

---

## REL-004 — No Silent Data Loss

**Priority:** MUST

The system shall not silently discard successfully received academic transactions.

---

# 32. Backup Requirements

## BAK-001 — Local Backup

**Priority:** MUST

The local system shall support backup of required local data.

---

## BAK-002 — Backup Configuration

**Priority:** MUST

Authorized administrators shall be able to configure backup behavior.

---

## BAK-003 — Backup Protection

**Priority:** MUST

Backups shall be protected against unauthorized access.

---

## BAK-004 — Restore Process

**Priority:** MUST

A documented restoration procedure shall exist.

---

## BAK-005 — Restore Validation

**Priority:** SHOULD

Restore procedures should be periodically tested.

---

# 33. Observability Requirements

## OBS-001 — Structured Logs

**Priority:** MUST

Important system events shall use structured logging.

---

## OBS-002 — Log Levels

**Priority:** MUST

Logs shall support appropriate severity levels including:

* Info
* Warning
* Error
* Critical

---

## OBS-003 — Log Categories

**Priority:** MUST

Logs should be filterable by relevant categories including:

* Authentication
* Database
* Synchronization
* AI
* Assessment
* Security
* Application

---

## OBS-004 — Log Search

**Priority:** SHOULD

Authorized administrators should be able to search logs.

---

## OBS-005 — Log Filtering

**Priority:** SHOULD

Authorized administrators should be able to filter logs by time and category.

---

## OBS-006 — Log Export

**Priority:** SHOULD

Relevant logs should be exportable for diagnostics.

---

# 34. Performance Requirements

Performance targets shall be finalized against the actual deployment hardware and expected school load.

However, the system must satisfy the following principles:

## PERF-001 — Local Responsiveness

**Priority:** MUST

Core LAN academic operations shall remain responsive under the expected school workload.

---

## PERF-002 — Assessment Responsiveness

**Priority:** MUST

Question navigation and answer persistence shall remain responsive during normal CBT load.

---

## PERF-003 — Recovery Responsiveness

**Priority:** MUST

CBT recovery shall occur without unnecessary delay after reconnection.

---

## PERF-004 — AI Isolation

**Priority:** MUST

Slow or unavailable AI processing shall not block core academic workflows.

---

# 35. Scalability Requirements

## SCALE-001 — Configurable Academic Structure

**Priority:** MUST

The system shall support expansion of classes, arms, subjects, topics, and question volumes without code changes.

---

## SCALE-002 — User Growth

**Priority:** SHOULD

The architecture should support growth in student, teacher, parent, and administrative accounts without fundamental redesign.

---

## SCALE-003 — Question Growth

**Priority:** MUST

The question bank shall support continued growth without requiring questions to be hardcoded.

---

# 36. Maintainability Requirements

## MAIN-001 — Modular Architecture

**Priority:** MUST

The system shall be modular.

---

## MAIN-002 — Separation of Concerns

**Priority:** MUST

Authentication, academic logic, assessment, synchronization, AI, analytics, and infrastructure responsibilities shall remain appropriately separated.

---

## MAIN-003 — Automated Testing

**Priority:** MUST

Critical business logic shall have automated tests.

---

## MAIN-004 — Documentation

**Priority:** MUST

Significant architectural and operational behavior shall be documented.

---

## MAIN-005 — Dependency Discipline

**Priority:** MUST

Dependencies shall be introduced only when justified.

---

# 37. Testing Requirements

## TEST-001 — Unit Testing

**Priority:** MUST

Business-critical logic shall have unit tests.

---

## TEST-002 — Integration Testing

**Priority:** MUST

Important cross-component behavior shall have integration tests.

---

## TEST-003 — End-to-End Testing

**Priority:** MUST

Critical user journeys shall have end-to-end tests.

---

## TEST-004 — Offline Testing

**Priority:** MUST

Offline academic workflows shall be tested without internet connectivity.

---

## TEST-005 — Synchronization Testing

**Priority:** MUST

Synchronization behavior shall be tested under:

* Normal operation
* Retry
* Duplicate delivery
* Failure
* Conflict
* Recovery
* Offline periods

---

## TEST-006 — CBT Recovery Testing

**Priority:** MUST

CBT recovery shall be tested under:

* Browser crash
* Refresh
* LAN interruption
* Reconnection
* Server restart where applicable
* Duplicate/replay conditions

---

## TEST-007 — Security Testing

**Priority:** MUST

Security controls shall be tested before acceptance.

---

## TEST-008 — Regression Testing

**Priority:** MUST

Existing functionality shall be regression-tested after significant changes.

---

# 38. Acceptance Requirements

A feature shall not be considered complete solely because implementation exists.

A completed feature must have:

* Implementation
* Required database changes
* Required API behavior
* Required UI behavior
* Unit tests
* Integration tests where applicable
* E2E tests where applicable
* Security validation
* Error handling
* Documentation
* Acceptance criteria
* Regression validation

---

# 39. Definition of Done

A requirement is considered complete only when:

1. The required behavior is implemented.
2. The implementation follows the approved architecture.
3. Authorization is enforced.
4. Data validation exists where applicable.
5. Errors are handled correctly.
6. Relevant tests exist and pass.
7. Regression tests pass.
8. Security implications have been reviewed.
9. Documentation is updated.
10. The requirement can be demonstrated against its acceptance criteria.

---

# 40. Scope-Control Requirements

## SCOPE-001 — No Unauthorized Scope Expansion

**Priority:** MUST

Implementation shall not introduce unrelated functionality simply because it is technically convenient.

---

## SCOPE-002 — No Future Feature Leakage

**Priority:** MUST

Future-module functionality shall not be prematurely implemented unless required by the current module's architecture.

---

## SCOPE-003 — Architecture Before Implementation

**Priority:** MUST

Major architectural decisions shall be documented before implementation of dependent functionality.

---

# 41. Security and Privacy Principles

The system shall follow these principles:

* Least privilege
* Secure defaults
* Explicit authorization
* Data minimization
* Controlled access
* Secure storage
* Auditability
* Secure synchronization
* Protected backups
* No unnecessary public exposure
* No plaintext credentials
* No secrets in source control

---

# 42. Explicitly Prohibited Behaviors

The system must not:

1. Allow public registration.
2. Allow unauthorized users to access protected data.
3. Allow parents to infer/access unrelated students.
4. Allow teachers to access unassigned academic scopes.
5. Allow unapproved questions into formal assessments.
6. Allow AI to determine authoritative grades.
7. Allow AI to modify permissions.
8. Allow AI to publish academic content independently.
9. Require internet connectivity for critical offline student academic workflows.
10. Silently overwrite synchronization conflicts.
11. Destroy historical academic results during promotion.
12. Automatically promote students without Super Admin activation.
13. Expose the local server unnecessarily to the public internet.
14. Store passwords in plaintext.
15. Store application secrets in source control.
16. Treat client-side authorization as sufficient security.
17. Depend on browser-only state for authoritative CBT recovery.
18. Hardcode the school's current academic class/arm structure.

---

# 43. Requirement Traceability

Every implementation module should reference the relevant requirement IDs.

For example:

```text
Module 01
    → ID-001
    → ID-002
    → ID-003
    → ID-004
    → ID-005
    → ...
```

Tests should reference the requirements they validate where practical.

This allows the project to maintain traceability:

```text
Requirement
    ↓
Architecture
    ↓
Implementation
    ↓
Test
    ↓
Acceptance
```

---

# 44. Requirements Change Process

Requirements may change as the project evolves.

A change that materially affects:

* Product scope
* User roles
* Data ownership
* Security
* Offline operation
* Synchronization
* Assessment behavior
* AI boundaries
* Database architecture

must be documented and reviewed before implementation.

Material architectural changes should receive an ADR.

---

# 45. Requirements Completion Status

The requirements in this document represent the current V1 baseline.

Technical implementation details that do not materially change product behavior may be finalized during architecture and technical design.

Examples include:

* Exact API framework
* Exact frontend framework
* Exact synchronization transport
* Exact vector database/index
* Exact embedding model
* Exact local AI model
* Exact Windows service-wrapper implementation
* Exact logging library
* Exact backup implementation

These technical choices must still satisfy all applicable requirements.

---

# 46. Final Requirement Principle

The system must be built around one fundamental operational guarantee:

> **Christian Royal College must be able to continue its core academic activities even when the internet is unavailable, while maintaining secure, reliable, auditable synchronization with the online master system when connectivity returns.**

All architecture and implementation decisions must preserve this guarantee.
