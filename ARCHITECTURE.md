# NAKAMA — Christian Royal College Integrated Academic System

# System Architecture

## 1. Document Purpose

This document defines the technical architecture of the NAKAMA platform.

It establishes:

* System topology
* Runtime environments
* Service boundaries
* Application boundaries
* Database architecture
* Authentication architecture
* Authorization architecture
* Academic domain architecture
* Assessment architecture
* CBT recovery architecture
* Offline architecture
* Synchronization architecture
* Content-processing architecture
* Local AI architecture
* Analytics architecture
* Security architecture
* Observability
* Backup and recovery
* Deployment architecture
* Failure behavior
* Architectural constraints

This document is the primary technical architecture reference for implementation.

Detailed implementation decisions that materially affect architecture should be recorded as ADRs.

---

# 2. Architectural Goals

NAKAMA architecture is designed around the following goals:

1. Reliable offline academic operation.
2. Secure school-LAN operation.
3. Reliable synchronization with the online master system.
4. Strong data integrity.
5. Recoverable CBT sessions.
6. Clear separation of business responsibilities.
7. Deterministic academic logic.
8. Safe use of local AI.
9. Maintainable modular implementation.
10. Strong observability.
11. Controlled deployment and updates.
12. Long-term extensibility without unnecessary complexity.

---

# 3. Primary Architecture Model

NAKAMA uses a hybrid online/offline architecture.

```text id="5l4ywx"
                         INTERNET
                            │
                            │
                            ▼
              ┌─────────────────────────┐
              │     ONLINE MASTER       │
              │                         │
              │ Web Application         │
              │ API                     │
              │ PostgreSQL              │
              │ Content Processing      │
              │ OCR / Structure Engine  │
              │ Sync Coordination       │
              │ Online Analytics        │
              └────────────┬────────────┘
                           │
                    Secure Synchronization
                           │
                           ▼
              ┌─────────────────────────┐
              │   SCHOOL LOCAL SERVER   │
              │                         │
              │ Server Manager          │
              │ Local Application/API   │
              │ SQLite + WAL            │
              │ Sync Engine             │
              │ Local AI                │
              │ RAG / Vector Index      │
              │ Watchdog                │
              │ Background Jobs         │
              │ Logging / Monitoring    │
              └────────────┬────────────┘
                           │
                         LAN
                           │
          ┌────────────────┼────────────────┐
          │                │                │
          ▼                ▼                ▼
      Student PCs     Teacher PCs      Admin PCs
```

The internet is not part of the critical local academic execution path.

---

# 4. Architectural Domains

The platform is divided into major domains.

```text id="4j4z9r"
Identity & Access
Academic Core
Content Management
Question Bank
Assessment
CBT
Practice
Analytics
Synchronization
Offline Runtime
Content Processing
Local AI
Administration
Observability
Security
Deployment
```

Each domain owns its business responsibilities.

Cross-domain communication must use defined interfaces rather than direct uncontrolled access to another domain's internal implementation.

---

# 5. Runtime Environments

NAKAMA has two primary runtime environments.

## 5.1 Online Runtime

Responsible for:

* Central application access
* Online dashboards
* Master data
* PostgreSQL
* Content processing
* OCR
* Document analysis
* Synchronization coordination
* Online administrative operations
* Online analytics
* Online parent access
* Online principal access
* Online student access where enabled

---

## 5.2 Local Runtime

Responsible for:

* Offline authentication
* Student academic operation
* Teacher local academic operation
* Practice
* CBT
* Local scoring
* Local progress recording
* Local academic content
* Synchronization
* Local AI
* Local RAG
* Server management
* Monitoring
* Watchdog/recovery

---

# 6. Local Server Architecture

The local server is not a single monolithic application process.

The conceptual architecture is:

```text id="2s2a7a"
┌───────────────────────────────────────────┐
│           NAKAMA SERVER MANAGER            │
│                                           │
│ Configuration │ Health │ Logs │ Controls  │
└──────────────────────┬────────────────────┘
                       │
          ┌────────────┼────────────┐
          │            │            │
          ▼            ▼            ▼
     Application     Sync        Background
       Runtime       Worker        Workers
          │            │            │
          ▼            ▼            ▼
       SQLite       Sync DB      Job State
          │
          ├───────────────┐
          │               │
          ▼               ▼
      Assessment        Local AI
        Engine          Runtime
          │               │
          ▼               ▼
      CBT State        RAG Index
```

The exact process/service decomposition may vary during implementation, but responsibilities must remain separated.

---

# 7. Local Server Manager

The Server Manager is the administrative control plane for the local server.

It is responsible for:

* Starting services
* Stopping services
* Restarting services
* Health checks
* Configuration
* Diagnostics
* Log access
* Backup controls
* Service status
* Network information
* AI status
* Synchronization status

The Server Manager must not contain core academic business logic.

---

# 8. Local Application Runtime

The Local Application Runtime provides:

* Authentication
* Authorization
* Student application
* Teacher application
* CBT
* Practice
* Academic content
* Progress
* Assessment APIs
* Administrative APIs required locally

It communicates with the local database through controlled data-access/domain boundaries.

---

# 9. Database Architecture

## 9.1 Online Database

The online master database is:

**PostgreSQL**

It provides:

* Transactional storage
* Referential integrity
* Relational modeling
* Indexing
* Constraints
* Reporting/query support
* Durable persistence

---

## 9.2 Local Database

The local database is:

**SQLite with WAL**

SQLite is selected because the local server is expected to be a single controlled school server rather than a distributed database cluster.

WAL provides better concurrent read/write behavior for the local workload.

---

# 10. Local Database Responsibilities

The local database stores operational data required for offline operation.

This includes, where applicable:

* User accounts required locally
* Role information
* Academic structure
* Approved academic content
* Questions
* Assessment definitions
* Assessment attempts
* Answers
* Scores
* Student progress
* Sync metadata
* Local audit data
* AI/RAG metadata
* Recovery state
* Configuration metadata

The local database must contain enough information to operate the required academic workflow without internet access.

---

# 11. Online and Local Schema Relationship

The local and online databases are not required to be identical physical schemas.

They must instead share explicit synchronization contracts.

This allows:

* Local performance optimization
* Offline-specific state
* Sync metadata
* Local recovery data
* Reduced unnecessary data transfer

The synchronization architecture defines which entities synchronize and how.

---

# 12. Domain Architecture

The application should use a modular domain structure.

Conceptually:

```text id="x9i1nt"
Identity
   │
   ├── Accounts
   ├── Roles
   └── Sessions

Academic
   │
   ├── Sessions
   ├── Terms
   ├── Classes
   ├── Arms
   ├── Subjects
   └── Topics

Content
   │
   ├── Questions
   ├── Question Options
   ├── Media
   └── Publications

Assessment
   │
   ├── Practice
   ├── CBT
   ├── Attempts
   ├── Answers
   └── Scoring

Analytics
   │
   ├── Progress
   ├── Performance
   └── Learning Gaps

Infrastructure
   │
   ├── Sync
   ├── AI
   ├── Jobs
   ├── Logging
   └── Monitoring
```

---

# 13. Identity Architecture

Identity is separated from authorization.

The authentication layer determines:

> Who is this user?

The authorization layer determines:

> What is this user allowed to do?

The application must never use role information from the client as an authoritative authorization decision.

---

# 14. Authentication Model

Authentication must support:

* Username/identifier
* Password
* Session/token management
* Account status
* First-login state
* Password-change requirement
* Login failure handling
* Session expiration
* Logout
* Local authentication

The exact token/session implementation is a technical stack decision, but it must provide secure server-side validation.

---

# 15. Offline Authentication

The local server maintains the authentication information required for offline operation.

When the internet is unavailable:

```text id="n5hz6v"
Student
   ↓
Local Login
   ↓
Local Authentication
   ↓
Local Session
   ↓
Local Application
```

No online authentication dependency may exist in the critical offline login path.

---

# 16. Authorization Architecture

Authorization follows:

```text id="0p1nvi"
User
 ↓
Role
 ↓
Permissions
 ↓
Academic Scope
 ↓
Resource
```

Examples:

Teacher access may depend on:

* Role = Teacher
* Assigned subject
* Assigned class
* Assigned arm

Parent access may depend on:

* Role = Parent
* Explicit student relationship

Super Admin access is administrative.

Principal access is management/academic rather than system-administrative.

---

# 17. Academic Domain Model

The core academic hierarchy is:

```text id="d2j9te"
Academic Session
    │
    └── Term
          │
          └── Class
                │
                └── Arm
                      │
                      └── Subject
                            │
                            └── Topic
                                  │
                                  └── Question
```

The actual relational implementation may include intermediate entities where necessary.

---

# 18. Student Academic Placement

A student has an active academic placement.

Historical placements must be retained.

A promotion operation creates the appropriate new academic placement rather than overwriting historical records.

---

# 19. Teacher Assignment Model

Teacher assignments should be represented explicitly.

Conceptually:

```text id="2i9dyc"
Teacher
   │
   ├── Subject Assignment
   │       │
   │       └── Class/Arm Scope
   │
   └── Additional Assignments
```

Authorization checks should resolve against these assignments.

---

# 20. Parent Relationship Model

Parent relationships must be explicit.

Conceptually:

```text id="7p9t9z"
Parent
   │
   ├── Student A
   ├── Student B
   └── Student C
```

A student may have only one parent account in V1.

No relationship should be inferred from:

* Surname
* Email
* Phone number
* Class
* Address
* Other indirect information

---

# 21. Question Architecture

Questions are versioned academic content entities.

A question should have:

* Identity
* Academic scope
* Content
* Options
* Correct answer
* Status
* Version/revision information
* Source information
* Review information
* Publication state

Question content must remain independent from assessment instances.

---

# 22. Question Versioning

Published questions must be protected from uncontrolled modification.

If an existing published question requires a substantive change, the architecture should support a new revision/version rather than silently changing the historical meaning of an already-used question.

This is important for assessment integrity.

---

# 23. Question Publication

Question states should be modeled explicitly.

Recommended state model:

```text id="4y9j0e"
IMPORTED
   ↓
PROCESSING
   ↓
REVIEW_REQUIRED
   ↓
IN_REVIEW
   ↓
APPROVED
   ↓
PUBLISHED
```

Only published questions enter eligible assessment pools.

---

# 24. Assessment Architecture

Practice and CBT share the same assessment foundation.

Conceptually:

```text id="6t5zga"
Assessment Definition
        │
        ├───────────────┐
        ▼               ▼
    Practice           CBT
        │               │
        └───────┬───────┘
                ▼
             Attempt
                │
                ▼
             Answers
                │
                ▼
             Scoring
                │
                ▼
             Results
```

This prevents duplicated assessment logic.

---

# 25. Assessment Definition

An assessment definition describes:

* Assessment type
* Subject
* Academic scope
* Question count
* Duration
* Attempt limit
* Randomization
* Option randomization
* Availability
* Result behavior

The definition is separate from a student's actual attempt.

---

# 26. Assessment Attempt

An attempt represents one student's execution of an assessment.

It should contain:

* Attempt ID
* Student ID
* Assessment ID
* Start time
* Server-authoritative end time
* Status
* Question set
* Answer state
* Submission state
* Score
* Result state

---

# 27. CBT State Machine

The assessment engine should use an explicit state machine.

Conceptually:

```text id="q3jz4p"
CREATED
   ↓
READY
   ↓
STARTED
   ↓
IN_PROGRESS
   │
   ├──────────────┐
   │              │
   ▼              ▼
SUBMITTED       EXPIRED
   │              │
   └──────┬───────┘
          ▼
       SCORED
          ↓
      COMPLETED
```

Invalid state transitions must be rejected.

---

# 28. CBT Timer Architecture

The server creates the authoritative timing boundaries.

The client may display a local countdown for responsiveness, but the client countdown is not authoritative.

The server determines whether:

* An attempt is still active
* A submission is valid
* The attempt has expired

This prevents client clock manipulation from extending an examination.

---

# 29. CBT Answer Persistence

Answers should be persisted incrementally rather than only at final submission.

Conceptually:

```text id="q5v5m6"
Student selects answer
        ↓
Client state update
        ↓
Local request
        ↓
Server validation
        ↓
Persist answer
        ↓
Acknowledge
```

The client should retain a temporary recovery buffer for situations where a request cannot immediately reach the local server.

---

# 30. CBT Recovery Architecture

Recovery is based on server-authoritative state plus client recovery state.

```text id="q3a9eq"
             ACTIVE ATTEMPT
                  │
          ┌───────┴───────┐
          │               │
          ▼               ▼
     Server State     Client Buffer
          │               │
          └───────┬───────┘
                  ▼
             INTERRUPTION
                  │
                  ▼
              RECONNECT
                  │
                  ▼
           State Reconciliation
                  │
                  ▼
           Resume Attempt
```

The reconciliation process must determine the authoritative state of each answer using defined ordering/version rules.

---

# 31. CBT Watchdog

The watchdog monitors active assessment sessions.

Responsibilities include:

* Detecting stale client sessions
* Monitoring heartbeat state
* Detecting interruption
* Supporting recovery
* Preventing duplicate active attempts
* Detecting abnormal session behavior

The watchdog must not arbitrarily terminate valid attempts because of short-lived connectivity interruptions.

---

# 32. Scoring Architecture

Scoring must be deterministic.

For an MCQ:

```text id="1i4drm"
Submitted Answer
      ↓
Question Version
      ↓
Authoritative Correct Answer
      ↓
Deterministic Comparison
      ↓
Score
```

AI must never participate in authoritative scoring.

---

# 33. Randomization Architecture

Question randomization must be deterministic enough to reproduce an attempt when required.

An assessment instance should retain the question sequence assigned to that attempt.

The system should not regenerate a different question sequence merely because the student refreshes the browser.

Where option randomization is enabled, the effective option ordering for the attempt should also be persisted.

---

# 34. Offline Assessment

Offline CBT follows:

```text id="0r3l5n"
Local Assessment Definition
        ↓
Local Attempt Creation
        ↓
Local Question Set
        ↓
Local Answer Persistence
        ↓
Local Scoring
        ↓
Local Result
        ↓
Sync Queue
        ↓
Online Master
```

The assessment must remain usable even if synchronization is unavailable.

---

# 35. Synchronization Architecture

Synchronization is treated as a dedicated subsystem.

It is not a database-copy operation.

The architecture uses:

* Change records
* Revision metadata
* Durable queue
* Operation identifiers
* Idempotency
* Conflict detection
* Conflict resolution
* Retry
* Backoff
* Audit events

---

# 36. Synchronization Flow

Normal synchronization follows:

```text id="i8b6ak"
Local Change
    ↓
Change Record
    ↓
Durable Sync Queue
    ↓
Sync Worker
    ↓
Secure Transport
    ↓
Online Sync Endpoint
    ↓
Validation
    ↓
Conflict Check
    ↓
Apply / Resolve
    ↓
Acknowledgement
    ↓
Local Sync State Update
```

The reverse direction follows the same controlled mechanism.

---

# 37. Change Records

A synchronization change record should contain sufficient metadata to identify:

* Entity type
* Entity identifier
* Operation type
* Revision/version
* Origin environment
* Operation ID
* Timestamp
* Dependencies where required
* Payload/reference
* Sync status

The exact schema belongs in the database/synchronization design.

---

# 38. Idempotency

Every synchronization operation must have an idempotent identity.

If the same operation is received more than once, the receiving system must recognize it and avoid applying the logical change multiple times.

This is essential for retry safety.

---

# 39. Conflict Detection

Conflicts occur when independent environments modify overlapping authoritative data.

The synchronization system must detect conflicts before applying a destructive overwrite.

Potential conflict classes include:

* Same record changed in both environments
* Relationship changed in both environments
* Academic configuration changed in both environments
* Question revision changed in both environments

---

# 40. Conflict Resolution

Conflict resolution must be defined by data domain.

The system must not use an unsafe global rule such as:

> Last write always wins.

For authoritative configuration, content, and academic records, conflict behavior must be explicitly defined.

Where automatic resolution is unsafe, the conflict should be surfaced for administrative resolution.

---

# 41. Sync Authority

Different entities may have different authority rules.

For example:

* Central account configuration may be controlled by the online master.
* Local CBT attempt state is authoritative locally while active.
* Local assessment results are produced locally and synchronized upward.
* Approved content originates from the controlled content workflow.
* Synchronization metadata is maintained by the synchronization subsystem.

The final entity authority matrix must be documented before synchronization implementation.

---

# 42. Sync Retry

Failed synchronization operations should use controlled retry behavior.

Retry logic should support:

* Retry count
* Backoff
* Temporary failure detection
* Permanent failure detection
* Dead-letter/error state
* Administrative retry

Repeated failures must not cause uncontrolled request loops.

---

# 43. Sync Integrity

Synchronization must validate:

* Authentication
* Authorization
* Payload integrity
* Entity identity
* Revision/version
* Operation identity
* Schema compatibility
* Required relationships

Invalid synchronization payloads must be rejected safely.

---

# 44. Synchronization Dashboard

The Super Admin interface should show:

* Online connection status
* Local sync status
* Last successful sync
* Last failed sync
* Pending operations
* Failed operations
* Retry status
* Error details
* Synchronization history
* Local server health

---

# 45. Content Processing Architecture

Content processing is online-only.

The conceptual pipeline is:

```text id="3b1l8q"
Upload
  ↓
File Validation
  ↓
Secure Storage
  ↓
Document Classification
  ↓
OCR / Parsing
  ↓
Layout Analysis
  ↓
Structure Extraction
  ↓
Question Extraction
  ↓
Normalization
  ↓
Validation
  ↓
Review Queue
  ↓
Teacher Review
  ↓
Approval
  ↓
Publication
```

---

# 46. Content Processing Components

The online processing subsystem may contain:

* Upload service
* File validator
* Document parser
* PaddleOCR/PP-OCR
* PP-Structure
* Layout processor
* Content normalizer
* Question extractor
* Validation engine
* Review workflow
* Publication service

---

# 47. Original Source Preservation

The original uploaded source material should be retained independently from extracted/normalized content where retention policy permits.

This allows:

* Reprocessing
* Verification
* Audit
* Teacher comparison
* Recovery from processing errors

---

# 48. Local AI Architecture

The local AI architecture is separate from the deterministic application core.

```text id="8s9f5q"
Student / Teacher / Principal
             ↓
        AI Gateway
             ↓
      Authorization
             ↓
       Context Builder
             ↓
       Retrieval Layer
             ↓
        Vector Index
             ↓
      Relevant Context
             ↓
         Ollama
             ↓
       Response Guard
             ↓
          Client
```

---

# 49. AI Gateway

All application AI requests should pass through a controlled AI gateway.

The gateway handles:

* Authentication
* Authorization
* Role checks
* Feature availability
* CBT blocking
* Context selection
* Rate limiting
* Logging
* Model selection
* Response handling

Clients should not directly control the Ollama runtime.

---

# 50. AI Retrieval Architecture

The RAG pipeline is:

```text id="rj8xye"
Approved Content
      ↓
Document Segmentation
      ↓
Chunking
      ↓
Metadata
      ↓
Embedding
      ↓
Vector Index
      ↓
Query Embedding
      ↓
Similarity Retrieval
      ↓
Context Selection
      ↓
Ollama
```

Metadata should allow retrieval by:

* Subject
* Topic
* Class
* Academic session
* Content type
* Publication status

This reduces irrelevant retrieval.

---

# 51. AI Context Restrictions

The AI gateway must ensure users receive only context they are authorized to access.

For example:

A student must not receive teacher-only information through an AI retrieval request.

A parent must not retrieve another student's academic records through AI.

AI retrieval must therefore inherit application authorization boundaries.

---

# 52. AI Storage Architecture

AI conversations should be stored using structured records rather than repeatedly storing entire retrieved documents.

A conversation may contain:

```text id="0d0c2b"
Conversation
    │
    ├── Message
    │      ├── Role
    │      ├── Content
    │      ├── Timestamp
    │      └── Metadata
    │
    └── Retrieval References
           ├── Content ID
           ├── Chunk ID
           └── Retrieval Metadata
```

This minimizes duplication while preserving traceability.

---

# 53. AI Safety Boundaries

The AI system must not have direct database-write authority over:

* Grades
* Student records
* Permissions
* Accounts
* Academic configuration
* Question publication
* Synchronization state

AI tools, if introduced later, must use narrowly scoped application commands with explicit authorization.

---

# 54. AI Failure Isolation

If Ollama is:

* stopped
* unavailable
* overloaded
* misconfigured
* out of memory

the following must continue functioning:

* Login
* Practice
* CBT
* Scoring
* Academic content
* Progress recording
* Synchronization

AI is an auxiliary service, not a core availability dependency.

---

# 55. Analytics Architecture

Analytics should be based on authoritative transactional data.

The basic pipeline is:

```text id="7z7a3s"
Academic Events
      ↓
Assessment Results
      ↓
Progress Records
      ↓
Aggregation
      ↓
Metrics
      ↓
Learning-Gap Analysis
      ↓
Dashboards
```

AI may interpret analytics but must not replace authoritative metric calculation.

---

# 56. Learning-Gap Architecture

Learning gaps should be derived from measurable academic evidence.

Potential evidence includes:

* Question performance
* Topic performance
* Subject performance
* Practice history
* Assessment history
* Repeated incorrect answers
* Progress trends

The exact formulas and thresholds will be defined in analytics specifications.

---

# 57. Audit Architecture

Audit logging should be separate from ordinary application debugging logs.

Audit records should capture appropriate:

* Actor
* Action
* Resource
* Resource ID
* Timestamp
* Result
* Relevant metadata
* Origin environment

Audit records should be protected from ordinary user modification.

---

# 58. Application Logging

Application logs are intended for:

* Diagnostics
* Operations
* Troubleshooting
* Performance investigation
* Failure analysis

They should support:

* Severity
* Timestamp
* Component
* Correlation ID
* Event/message
* Relevant structured metadata

Sensitive secrets must never be logged.

---

# 59. Correlation IDs

Requests crossing application boundaries should support correlation identifiers.

This allows administrators and developers to trace:

```text id="qj4l9w"
User Action
    ↓
API Request
    ↓
Database Operation
    ↓
Background Job
    ↓
Sync Operation
    ↓
Remote Operation
```

without exposing secrets.

---

# 60. Security Architecture

Security is layered.

```text id="c5m6k0"
Network Security
       ↓
Transport Security
       ↓
Authentication
       ↓
Session Security
       ↓
Authorization
       ↓
Input Validation
       ↓
Business Rules
       ↓
Data Protection
       ↓
Audit
```

No single layer is considered sufficient.

---

# 61. Local Network Security

The local server deployment should use:

* Windows firewall
* Restricted listening interfaces
* Minimum required ports
* No public port forwarding
* Controlled LAN access
* Secure credentials
* Strong administrator authentication
* HTTPS/TLS where supported by the final deployment architecture

---

# 62. Application Security

The application must implement:

* Server-side authorization
* Secure password hashing
* Session security
* Input validation
* Output encoding
* Injection prevention
* Secure file handling
* CSRF protection where applicable
* Security headers
* Secure error handling
* Rate limiting
* Audit logging

---

# 63. File Security

Uploaded files must be treated as untrusted input.

The file pipeline should validate:

* File type
* File extension
* MIME type
* File size
* File structure
* Processing compatibility

Files should be stored outside executable/source-code paths.

---

# 64. Secrets Management

Secrets must not be embedded in:

* Source code
* Client bundles
* Git repositories
* Logs
* Screenshots
* Public configuration

The final deployment architecture must provide an appropriate secure secret-storage mechanism.

---

# 65. Backup Architecture

The backup system should support:

```text id="s5j5wz"
Operational Database
        ↓
Backup Process
        ↓
Protected Backup Storage
        ↓
Retention Policy
        ↓
Restore Procedure
        ↓
Validation
```

Backups must not be considered valid merely because a backup file exists.

Restore validation is part of operational readiness.

---

# 66. Failure Domains

The architecture separates failure domains.

### Online failure

Does not stop:

* Local login
* Practice
* CBT
* Local scoring
* Local progress

### Sync failure

Does not stop:

* Local academic operation

Pending synchronization remains queued.

### AI failure

Does not stop:

* CBT
* Practice
* Login
* Scoring
* Academic content

### Content-processing failure

Does not affect:

* Existing published academic content
* Student CBT
* Practice

### Browser failure

Should not destroy:

* Active CBT attempt

---

# 67. Network Failure Model

The system distinguishes:

1. Internet unavailable
2. Online server unavailable
3. Local LAN unavailable
4. Individual client disconnected
5. Temporary packet/request failure
6. Local server unavailable

Each failure class requires different handling.

Internet failure should be treated as normal operational state rather than exceptional catastrophic failure.

---

# 68. Local Server Failure

If the local server itself becomes unavailable:

* Local academic clients cannot continue until the server is restored.
* The server must recover persisted state.
* Active CBT recovery must be evaluated against persisted attempt state.
* Services should restart safely.
* No database corruption should result from ordinary service interruption.

---

# 69. Online/Local Separation

The online and local systems should not depend on each other's runtime availability for their independent core responsibilities.

This reduces cascading failures.

---

# 70. Deployment Topology

## Online

```text id="u6kqzq"
Internet
   │
   ▼
Reverse Proxy / Load Boundary
   │
   ▼
Application Services
   │
   ▼
PostgreSQL
```

Additional online processing workers/services may be deployed separately.

---

## School

```text id="m3y0nz"
School LAN
    │
    ▼
Windows Server PC
    │
    ├── NAKAMA Server Manager
    ├── Local Application
    ├── SQLite
    ├── Sync Worker
    ├── Watchdog
    └── Ollama
```

---

# 71. Local Server Network Policy

The local server should:

* Use a fixed/reserved LAN address
* Use controlled local naming
* Restrict inbound traffic to required LAN clients
* Avoid internet-facing bindings
* Avoid unnecessary services
* Avoid unnecessary ports

The exact hostname, port, and Windows networking implementation belong in deployment documentation.

---

# 72. Application Packaging

The local application should be packaged so that deployment can be performed by an authorized administrator.

Installation should support:

* Prerequisite validation
* Application installation
* Configuration
* Database initialization
* Service registration where required
* Firewall configuration where appropriate
* Initial administrator setup
* Health verification

---

# 73. Update Architecture

Updates to the local system must be controlled.

An update process should support:

* Version identification
* Compatibility checking
* Backup before migration
* Database migration
* Application replacement
* Service restart
* Health verification
* Rollback strategy where feasible

An update must not silently destroy local academic state.

---

# 74. Database Migration

Database schema changes must use explicit versioned migrations.

Migrations must be:

* Reproducible
* Ordered
* Tested
* Reviewable

Production schema changes must never rely on manually editing the database.

---

# 75. Configuration Management

Configuration must be separated from application code.

Configuration categories include:

* Environment
* Database
* Network
* Authentication
* Sync
* AI
* Logging
* Storage
* Backup

Secrets must remain separate from ordinary non-secret configuration.

---

# 76. Environment Separation

At minimum, the project should distinguish:

```text id="2m0g4h"
Development
Testing
Staging / Acceptance
Production
```

The local school production environment must not be treated as a development environment.

---

# 77. Testing Architecture

Testing occurs at multiple levels.

```text id="qk3y3m"
Unit
  ↓
Integration
  ↓
API
  ↓
Component
  ↓
End-to-End
  ↓
Offline
  ↓
Synchronization
  ↓
Security
  ↓
Acceptance
```

Critical workflows require more than unit tests.

---

# 78. Critical End-to-End Workflows

The following journeys must receive end-to-end coverage:

### Student

```text
Login
 → Dashboard
 → Practice
 → Assessment
 → Answer
 → Submit
 → Score
 → Progress
```

### CBT Recovery

```text
Login
 → Start CBT
 → Answer
 → Connection interruption
 → Reconnect
 → Resume
 → Submit
 → Score
```

### Teacher

```text
Login
 → Assigned Class
 → Review Imported Question
 → Edit
 → Approve
 → Publish
```

### Parent

```text
Login
 → Child Selection
 → Performance
 → Progress
```

### Principal

```text
Login
 → Management Dashboard
 → School Performance
 → Learning Gaps
```

### Super Admin

```text
Login
 → User Management
 → Academic Configuration
 → Teacher Assignment
 → CBT Configuration
 → Sync Monitoring
```

---

# 79. Security Testing Architecture

Security testing must include:

* Authentication testing
* Authorization testing
* Privilege escalation testing
* Session testing
* Input validation testing
* File upload testing
* API testing
* Injection testing
* XSS testing
* CSRF testing where applicable
* Rate-limit testing
* Local network exposure testing
* Synchronization authorization testing
* Backup access testing

A formal penetration test is required before final acceptance.

---

# 80. Performance Architecture

Performance optimization should prioritize the critical local path.

Priority order:

1. Authentication
2. Question delivery
3. Answer persistence
4. CBT navigation
5. Scoring
6. Progress recording
7. Synchronization
8. Analytics
9. AI

AI must never consume enough local resources to starve CBT or core application services.

---

# 81. Resource Isolation

The local server must account for resource contention between:

* SQLite
* Application/API
* Sync workers
* Background jobs
* Ollama
* Vector search
* Logging

AI workloads should be constrained so they cannot destabilize core academic services.

---

# 82. Data Lifecycle

Data should follow a controlled lifecycle:

```text id="3l9h5z"
Created
 ↓
Active
 ↓
Updated / Versioned
 ↓
Published
 ↓
Used
 ↓
Archived
```

Deletion should be restricted for records that are required for academic history or auditability.

---

# 83. Historical Integrity

Historical academic data is immutable in principle.

Changes to current configuration must not rewrite historical meaning.

Examples:

* Changing a student's class does not rewrite previous results.
* Changing a question does not silently rewrite an old attempt.
* Promotion does not erase previous academic placement.
* Teacher reassignment does not erase historical teaching records.

---

# 84. Concurrency

The architecture must safely handle concurrent activity.

Examples include:

* Multiple students submitting answers
* Multiple students taking assessments
* Teacher reviewing content
* Synchronization running
* Analytics processing
* AI requests

Critical database writes must use appropriate transaction boundaries.

---

# 85. Transaction Boundaries

Operations that must remain atomic should execute inside controlled transactions.

Examples:

* Creating an assessment attempt
* Persisting an answer
* Completing an assessment
* Applying a promotion
* Creating a synchronization operation
* Applying a conflict resolution

---

# 86. API Architecture

The system should expose versioned application APIs.

API boundaries should distinguish:

* Authentication
* Users
* Academic structure
* Questions
* Assessments
* Attempts
* Progress
* Analytics
* Synchronization
* AI
* Administration
* Health/diagnostics

The exact API framework is a technical implementation decision.

---

# 87. API Authorization

Every protected API endpoint must verify authorization.

Authorization must consider both:

* User role
* User's authorized academic/resource scope

An endpoint must never rely solely on a frontend route guard.

---

# 88. API Error Model

APIs should return structured errors.

Errors should provide enough information for the client to respond correctly without exposing sensitive internal information.

The architecture should distinguish:

* Validation errors
* Authentication errors
* Authorization errors
* Resource-not-found errors
* Conflict errors
* Rate-limit errors
* Server errors
* Dependency errors

---

# 89. Health Architecture

The system should expose controlled health checks.

Health checks should distinguish:

* Application health
* Database health
* Sync health
* AI health
* Background worker health
* Storage health

Health endpoints must not expose sensitive configuration or secrets.

---

# 90. Background Jobs

Long-running or asynchronous tasks should be handled by background workers.

Examples:

* Synchronization
* Content processing
* Analytics aggregation
* AI indexing
* Backup
* Maintenance

Background jobs should have:

* Job identity
* Status
* Retry policy
* Error state
* Logging
* Idempotency where appropriate

---

# 91. Job Failure Handling

A failed background job must not silently disappear.

It should enter a visible failure state.

Depending on job type, the system should support:

* Retry
* Manual retry
* Failure inspection
* Dead-letter state
* Administrative resolution

---

# 92. Observability Architecture

Observability consists of:

```text id="p1h1x6"
Logs
+
Metrics
+
Health
+
Audit
+
Tracing/Correlation
```

The implementation may use different tooling in different environments, but these concerns must remain architecturally distinct.

---

# 93. Documentation Architecture

Documentation is part of the product engineering process.

The repository contains dedicated documentation for:

* Architecture
* Database
* Authentication
* API
* Synchronization
* Offline operation
* Content processing
* AI
* Analytics
* Deployment
* Decisions

Implementation changes that affect these areas must update the relevant documentation.

---

# 94. Module Boundaries

The implementation follows these major modules:

```text id="p6y8hx"
00 Foundation
01 Identity
02 Academic Core
03 Student CBT
04 Teacher
05 Offline & Synchronization
06 Content Processing
07 Local AI
08 Analytics
09 Hardening & Acceptance
```

Modules may depend on earlier modules.

Circular module dependencies should be avoided.

---

# 95. Module Dependency Direction

The preferred dependency direction is:

```text id="l8x3dz"
Foundation
    ↓
Identity
    ↓
Academic Core
    ↓
Assessment / CBT
    ↓
Teacher / Analytics
    ↓
Offline / Sync
    ↓
Content Processing / AI
    ↓
Hardening / Acceptance
```

Infrastructure concerns may support multiple modules without becoming owners of their business logic.

---

# 96. Architectural Rules

The following rules are mandatory:

1. Do not put business logic in UI components.
2. Do not use AI for deterministic business decisions.
3. Do not trust client authorization.
4. Do not hardcode academic structure.
5. Do not duplicate assessment logic between Practice and CBT.
6. Do not treat browser state as authoritative CBT state.
7. Do not copy databases blindly during synchronization.
8. Do not silently overwrite synchronization conflicts.
9. Do not expose the local server publicly.
10. Do not allow unapproved questions into assessments.
11. Do not destroy historical academic state.
12. Do not make core academic operation dependent on internet connectivity.
13. Do not allow AI to modify authoritative records.
14. Do not introduce architecture changes without documenting them.
15. Do not introduce unnecessary dependencies.

---

# 97. Architectural Decision Areas

The following areas require dedicated ADRs before or during their implementation:

* Technology stack
* Authentication/session strategy
* API architecture
* Local server packaging
* Sync protocol
* Sync conflict resolution
* Entity authority matrix
* CBT recovery ordering
* Vector index/storage
* Embedding model
* Ollama model
* Backup strategy
* Deployment/update mechanism
* TLS certificate strategy
* Observability tooling

---

# 98. Recommended Technical Direction

The final technology stack should be selected based on:

* Windows compatibility
* Local deployment reliability
* Offline operation
* PostgreSQL compatibility
* SQLite support
* Strong TypeScript or equivalent type safety
* Mature API ecosystem
* Testing ecosystem
* Maintainability
* Long-term support
* Local AI integration
* Packaging capability

Technology selection must be recorded in an ADR before implementation of the affected modules.

---

# 99. Architectural Quality Attributes

The architecture is optimized for:

| Attribute           | Priority |
| ------------------- | -------- |
| Offline reliability | Critical |
| Data integrity      | Critical |
| Security            | Critical |
| CBT reliability     | Critical |
| Recoverability      | Critical |
| Maintainability     | High     |
| Observability       | High     |
| Performance         | High     |
| Scalability         | Medium   |
| AI capability       | Medium   |
| Visual flexibility  | High     |

---

# 100. Final Architectural Principle

NAKAMA is not fundamentally an online website with an offline mode.

It is a **distributed academic system with an online master environment and an independently operational school-local academic environment**.

The architecture must preserve that distinction.

The local environment must be capable of carrying the school's critical academic workload independently.

The online environment provides centralized management, synchronization, content processing, online dashboards, and broader system capabilities.

Synchronization connects the two environments without making either dependent on the immediate availability of the other.

The architecture therefore prioritizes:

```text id="l4l7wp"
RELIABILITY
    ↓
DATA INTEGRITY
    ↓
SECURITY
    ↓
OFFLINE OPERATION
    ↓
RECOVERY
    ↓
OBSERVABILITY
    ↓
MAINTAINABILITY
    ↓
EXTENSIBILITY
```

Every future implementation decision should be evaluated against these principles.
