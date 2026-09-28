# Database Architecture

**Project:** CRC LMS
**Document:** Database Architecture Specification
**Status:** Baseline
**Scope:** V1
**Primary Online Database:** PostgreSQL
**Primary Local Database:** SQLite with WAL

---

# 1. Purpose

This document defines the database architecture for CRC.

It establishes:

* data ownership
* database boundaries
* entity responsibilities
* relationships
* identifiers
* lifecycle rules
* versioning
* auditability
* synchronization requirements
* local/offline data requirements
* PostgreSQL requirements
* SQLite requirements
* indexing strategy
* migration strategy
* retention principles
* backup requirements
* integrity rules

This document is the database-level companion to:

* `PROJECT_CONTEXT.md`
* `REQUIREMENTS.md`
* `ARCHITECTURE.md`
* `SECURITY_REQUIREMENTS.md`

The implementation MUST conform to these documents.

---

# 2. Database Architecture Principles

The database architecture MUST follow these principles:

1. Referential integrity MUST be enforced.
2. Business-critical invariants MUST be enforced server-side and, where practical, at database level.
3. Historical academic records MUST remain reproducible.
4. Assessment attempts MUST be immutable after finalization except through controlled correction workflows.
5. User relationships MUST be explicit.
6. Local and online databases MUST NOT be treated as interchangeable copies.
7. Synchronization MUST operate on records and revisions rather than blind database replacement.
8. Deletes MUST be used carefully.
9. Auditability MUST be preserved.
10. Database migrations MUST be versioned.
11. Queries MUST be designed around actual access patterns.
12. Sensitive information MUST be protected.
13. The local database MUST support complete offline academic operation.
14. The online database MUST support cloud dashboards, synchronization and management functions.

---

# 3. Database Topology

CRC uses two primary database environments.

```text
                         INTERNET
                            │
                            ▼
                  ┌───────────────────┐
                  │ ONLINE APPLICATION│
                  └─────────┬─────────┘
                            │
                            ▼
                  ┌───────────────────┐
                  │    POSTGRESQL     │
                  │  ONLINE MASTER    │
                  └─────────┬─────────┘
                            │
                     Synchronization
                            │
                            ▼
                  ┌───────────────────┐
                  │ SCHOOL LOCAL APP  │
                  └─────────┬─────────┘
                            │
                            ▼
                  ┌───────────────────┐
                  │      SQLITE       │
                  │       WAL         │
                  └───────────────────┘
```

The two databases have related domain models but MUST NOT be assumed to be identical physical implementations.

---

# 4. Online Database

PostgreSQL is the authoritative persistent database for the online/master system.

It supports:

* online dashboards
* account management
* academic configuration
* content management
* analytics
* synchronization coordination
* online reporting
* content-processing results
* approved AI knowledge sources
* audit records
* historical academic records

PostgreSQL MUST be deployed as a managed and protected service appropriate to the production environment.

---

# 5. Local Database

SQLite with WAL is the primary local database.

It supports:

* local authentication
* student learning
* practice
* CBT
* teacher academic workflows
* local academic content
* active assessment state
* local analytics required for operation
* synchronization queue
* recovery state
* local audit records
* AI/RAG metadata required for local operation

The local database MUST remain operational without Internet access.

---

# 6. Local Database Design Goal

The local database MUST prioritize:

* reliability
* low operational overhead
* fast reads
* predictable writes
* recoverability
* safe concurrent access
* compact storage
* deterministic behavior

SQLite WAL MUST be used to support concurrent readers and controlled writer behavior.

The application MUST NOT rely on unsupported concurrent-write patterns.

---

# 7. Database Ownership Model

Every major entity MUST have a defined ownership model.

Possible authority states include:

* Online authoritative
* Local authoritative
* Shared with controlled synchronization
* Derived
* Ephemeral

An entity MUST NOT have ambiguous authority.

---

# 8. Authority Categories

## 8.1 Online-Authoritative

Examples include:

* global account provisioning
* global system configuration
* cloud-level user lifecycle
* master academic configuration
* published content metadata

## 8.2 Local-Authoritative During Offline Operation

Examples include:

* active CBT attempts
* local answer persistence
* local timer state
* local practice sessions
* local recovery state

These records MUST later synchronize according to defined rules.

## 8.3 Shared/Synchronized

Examples include:

* student progress
* assessment results
* practice history
* relevant academic activity
* synchronization metadata

These require revision and conflict handling.

## 8.4 Derived

Examples:

* dashboard aggregates
* analytics summaries
* learning-gap indicators
* cached statistics

Derived data MAY be rebuilt from authoritative records.

---

# 9. Global Identifier Strategy

Entities that cross the online/local boundary MUST use globally unique identifiers.

UUIDs are the preferred identifier strategy.

Identifiers MUST:

* be unique across environments
* remain stable through synchronization
* never be regenerated simply because a record moves between systems

Local-only ephemeral records MAY use locally generated identifiers where they never leave the local environment.

---

# 10. Identifier Requirements

Primary identifiers MUST NOT depend on:

* student names
* usernames
* class names
* sequential human-facing numbers
* email addresses
* phone numbers

Human-readable identifiers MAY exist as separate fields.

---

# 11. Core Entity Groups

The database is logically divided into the following groups:

```text
Identity
Academic Structure
Academic Membership
Teacher Assignment
Parent Relationships
Content
Question Bank
Assessment
Assessment Attempts
Practice
Progress
Analytics
Synchronization
Content Processing
AI / RAG
Audit
System Configuration
Backup / Operational Metadata
```

---

# 12. Identity Domain

Core identity entities include:

* User
* Role
* UserRole
* Credential
* Session
* AccountStatus
* PasswordReset
* SecurityEvent

---

# 13. User Entity

The user entity represents an authenticated person.

A user SHOULD contain information such as:

* `id`
* `username` or login identifier
* display name
* account status
* role relationship
* first-login state
* created timestamp
* updated timestamp
* deactivated timestamp where applicable
* last successful login
* last failed login information where required

Passwords MUST NOT be stored directly in the user record as plaintext.

---

# 14. Role Model

Roles MUST be represented explicitly.

Supported V1 roles:

```text
SUPER_ADMIN
PRINCIPAL
TEACHER
PARENT
STUDENT
```

Role membership MUST be stored independently from profile information.

The system SHOULD avoid embedding complex permission logic directly into user rows.

---

# 15. Student Profile

A student profile MUST reference the corresponding user account.

The student record SHOULD contain:

* student ID
* user ID
* current academic placement
* enrollment status
* admission/enrollment metadata required by the LMS
* timestamps

A student MUST have one current class/arm placement at a given point in time.

Historical placement MUST be preserved where required.

---

# 16. Teacher Profile

A teacher profile MUST reference the corresponding user account.

It SHOULD contain:

* teacher ID
* user ID
* employment/display information required by the LMS
* status
* timestamps

Teaching assignments MUST NOT be embedded as arbitrary fields on the teacher row.

---

# 17. Parent Profile

A parent profile MUST reference a user account.

The database MUST support:

```text
One Parent → Multiple Students
One Student → One Parent
```

The parent-student relationship MUST be explicit.

---

# 18. Parent-Student Relationship

The relationship SHOULD be represented through a dedicated relationship record.

It MUST contain at minimum:

* relationship ID
* parent ID
* student ID
* status
* created timestamp
* updated timestamp

A database uniqueness constraint MUST prevent a student from being linked to multiple active parent accounts.

The design SHOULD allow future relationship metadata without changing the fundamental ownership model.

---

# 19. Academic Structure

The academic hierarchy is:

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

The physical schema MUST preserve these relationships.

---

# 20. Academic Session

An academic session represents a school academic year/session.

It SHOULD include:

* ID
* name/code
* start date
* end date
* status
* creation/update timestamps

Only one session SHOULD normally be marked current for a given school context.

Historical sessions MUST remain queryable.

---

# 21. Term

A term belongs to an academic session.

A term SHOULD include:

* ID
* session ID
* name/sequence
* start date
* end date
* status

Term dates MUST be validated against their parent session.

---

# 22. Class

Class represents an academic level such as:

* JSS1
* JSS2
* JSS3
* SS1
* SS2
* SS3

Class MUST NOT assume a fixed number of arms.

---

# 23. Arm

An arm belongs to a class.

Examples:

```text
JSS1 A
JSS1 B

SS1 Arts
SS1 Science
SS1 Commercial
```

The system MUST allow additional arms without code changes.

---

# 24. Class/Arm Flexibility

The database MUST NOT encode assumptions such as:

```text
JSS1 always has A/B
SS1 always has Arts/Science/Commercial
```

These are configuration examples, not database constraints.

---

# 25. Subject

Subjects MUST be independently represented.

A subject SHOULD contain:

* ID
* code
* name
* description
* status

Subject assignments MUST be separate from the subject definition.

---

# 26. Topic

A topic belongs to a subject.

Topics SHOULD support ordering and hierarchy where needed.

A topic SHOULD contain:

* ID
* subject ID
* name
* description
* sequence/order
* status

The design MAY support subtopics later without invalidating the V1 structure.

---

# 27. Student Academic Placement

A student's class/arm membership MUST be represented explicitly.

The system SHOULD maintain placement history:

```text
Student
    ↓
Academic Placement
    ↓
Session / Term
    ↓
Class / Arm
```

This is required for historical reporting.

---

# 28. Promotion History

Promotion MUST NOT overwrite historical placement.

A promotion operation SHOULD create a new academic placement while preserving the previous placement.

The system SHOULD record:

* student
* source session
* source class/arm
* destination session
* destination class/arm
* promotion status
* executed by
* execution timestamp

---

# 29. Teacher Assignment

Teacher assignments MUST be represented explicitly.

An assignment SHOULD link:

* teacher
* class/arm
* subject
* academic session
* term where applicable
* status

This assignment controls teacher visibility.

---

# 30. Assignment Integrity

A teacher MUST only receive access to classes and subjects represented by active assignments.

Removing an assignment MUST NOT delete historical teaching records.

The assignment record SHOULD support effective dates/status.

---

# 31. Question Bank

The question bank MUST support:

* questions
* options
* correct answer
* subject
* topic
* academic scope
* question version
* import source
* processing status
* review status
* publication status

---

# 32. Question Entity

A question SHOULD include:

* ID
* current version reference
* subject
* topic
* academic level/scope
* question type
* status
* source/import metadata
* timestamps

V1 question type:

```text
MCQ
```

The schema SHOULD allow additional types in future without forcing a destructive redesign.

---

# 33. Question Version

Question content MUST be versioned.

A question version SHOULD contain:

* version ID
* question ID
* version number
* question body
* rich-content representation
* explanation where applicable
* author/source metadata
* created timestamp
* approval metadata
* publication metadata

Once a version has been used by a completed assessment, it MUST remain reproducible.

---

# 34. Question Options

Options MUST be represented separately from the question version.

Each option SHOULD contain:

* ID
* question version ID
* option key
* content
* display order
* correctness indicator

Correctness MUST remain protected from unauthorized student access.

---

# 35. Rich Question Content

The database MUST support question content containing:

* formatted text
* images
* mathematical expressions
* LaTeX
* diagrams
* tables
* embedded references

Content SHOULD be stored in a structured representation rather than forcing all content into plain text.

Binary assets SHOULD be stored separately with references from question content.

---

# 36. Question Publication

Publication status MUST be explicit.

Possible states include:

```text
DRAFT
IMPORTED
PROCESSING
REVIEW_REQUIRED
APPROVED
PUBLISHED
UNPUBLISHED
ARCHIVED
```

Only appropriate published versions may be selected for student assessments.

---

# 37. Content Processing Records

Content processing MUST maintain processing metadata.

A processing record SHOULD include:

* source file
* processing job ID
* processing status
* OCR engine/version
* processing timestamps
* error information
* extracted entities
* reviewer information

Processing history MUST remain traceable.

---

# 38. Uploaded Materials

Uploaded source files SHOULD have dedicated records containing:

* file ID
* original filename
* MIME type
* size
* checksum
* storage reference
* uploader
* processing status
* created timestamp

The database SHOULD NOT store large binary documents directly unless there is a specific architectural reason.

---

# 39. Assessment Domain

Core assessment entities include:

* Assessment
* Assessment Configuration
* Assessment Question
* Assessment Attempt
* Attempt Answer
* Attempt Event
* Assessment Result

---

# 40. Assessment

An assessment represents a configured student activity.

It SHOULD contain:

* ID
* name
* type
* subject
* academic scope
* configuration
* publication state
* availability window
* creator
* timestamps

Assessment types include:

```text
PRACTICE
CBT
```

Both use the shared assessment engine.

---

# 41. Assessment Configuration

Assessment configuration SHOULD include:

* duration
* question count
* attempts allowed
* randomization
* option randomization
* navigation rules
* scoring configuration
* availability
* review rules

Configuration MUST be versioned or snapshotted where necessary so historical attempts remain reproducible.

---

# 42. Assessment Question Selection

An assessment MUST have a deterministic record of the questions selected for an active attempt.

The database MUST NOT require regeneration of the question set during recovery.

---

# 43. Assessment Attempt

An attempt represents one student's participation in an assessment.

It SHOULD contain:

* attempt ID
* assessment ID
* student ID
* status
* start timestamp
* server-authoritative deadline
* submission timestamp
* score
* attempt number
* question-set reference
* recovery metadata

---

# 44. Attempt State

Attempt state SHOULD follow a controlled state machine.

Example:

```text
CREATED
    ↓
READY
    ↓
IN_PROGRESS
    ↓
SUBMITTED
    ↓
SCORING
    ↓
COMPLETED
```

Failure or recovery states MAY be represented where required.

Invalid state transitions MUST be rejected.

---

# 45. Attempt Answers

Each answer MUST reference:

* attempt
* question/version
* selected option
* answer state
* timestamps

Answers MUST support repeated changes before final submission.

The latest valid answer MUST be recoverable.

---

# 46. Attempt Event History

Important assessment events SHOULD be recorded.

Examples:

* attempt started
* question viewed
* answer changed
* answer saved
* connection lost
* session recovered
* submitted
* timer expired
* scoring completed

Event history MUST be designed with storage efficiency in mind.

---

# 47. Assessment Results

Assessment results SHOULD be derived from authoritative attempt data.

Results SHOULD contain:

* attempt
* raw score
* percentage
* grading data where applicable
* completion status
* calculated timestamp

Results MUST NOT replace the underlying attempt record.

---

# 48. Scoring Integrity

Scoring MUST be deterministic.

The scoring engine MUST use:

* the question version used in the attempt
* the recorded answer
* the assessment scoring rules

AI MUST NOT determine official assessment scores.

---

# 49. Practice Sessions

Practice uses the same core assessment engine.

Practice records SHOULD preserve:

* student
* assessment
* attempt
* score
* topic/subject context
* timestamp

Practice history MUST remain available for progress analytics.

---

# 50. Student Progress

Progress data MAY include:

* subject progress
* topic progress
* practice frequency
* assessment history
* accuracy
* weak-topic indicators

Derived progress values SHOULD be recalculable from underlying academic events where practical.

---

# 51. Analytics Data

Analytics SHOULD distinguish between:

* authoritative source records
* calculated aggregates
* cached dashboard values

Derived analytics MUST NOT become the only source of truth for academic history.

---

# 52. Learning Gaps

Learning-gap records SHOULD reference:

* student
* subject
* topic
* supporting observations
* calculation/version
* timestamp

Learning-gap indicators are analytical outputs, not permanent academic facts.

They MUST be recalculable when analytical logic changes.

---

# 53. Synchronization Domain

Synchronization MUST use dedicated metadata.

Core synchronization entities SHOULD include:

* Sync Job
* Sync Operation
* Sync Cursor
* Change Record
* Conflict Record
* Sync Error
* Device/Installation Identity

---

# 54. Local Installation Identity

Each school local installation MUST have a stable installation identifier.

This identifier MUST distinguish one school/local server installation from another.

It MUST NOT be based solely on:

* IP address
* hostname
* Windows username
* machine display name

---

# 55. Change Tracking

Synchronizable entities MUST expose enough metadata to identify changes.

Change metadata SHOULD include:

* entity ID
* entity type
* revision
* operation
* origin
* timestamp
* actor where applicable

---

# 56. Revision Numbers

Synchronizable records SHOULD use monotonic revisions or equivalent version identifiers.

Revision metadata allows the system to determine:

* whether a change is newer
* whether an update is stale
* whether concurrent changes occurred
* whether an operation was already applied

---

# 57. Idempotency

Synchronization operations MUST be idempotent.

If the same operation is delivered twice, the final database state MUST remain correct.

This is essential for:

* retries
* network interruptions
* duplicate delivery
* server restarts

---

# 58. Conflict Records

Conflicts MUST be stored explicitly when automatic resolution cannot safely determine the correct state.

A conflict SHOULD contain:

* conflict ID
* entity
* local revision
* remote revision
* conflict type
* detected timestamp
* resolution state
* resolution actor
* resolution timestamp

---

# 59. Sync Queue

The local database MUST contain a durable synchronization queue or equivalent durable change-outbox mechanism.

Queue records MUST survive:

* application restart
* server restart
* temporary Internet loss

The queue MUST support retry state.

---

# 60. Sync Queue States

Example states:

```text
PENDING
PROCESSING
SUCCEEDED
RETRY_WAIT
FAILED
CONFLICT
CANCELLED
```

Failed records MUST retain enough information to diagnose the problem without exposing sensitive secrets.

---

# 61. Audit Domain

Audit data SHOULD be separate from ordinary business entities.

Audit records MUST identify:

* actor
* action
* target
* timestamp
* result
* request/correlation identifier
* relevant metadata

Audit records MUST not be casually deleted as part of ordinary entity deletion.

---

# 62. Soft Deletion

Soft deletion SHOULD be used where historical integrity requires preservation.

Examples:

* users
* academic structures
* questions
* assessments
* assignments
* relationships

Hard deletion SHOULD be restricted to records where there is a clear retention and privacy reason.

---

# 63. Historical Integrity

Historical academic data MUST remain queryable after:

* promotion
* class changes
* teacher reassignment
* question updates
* assessment configuration changes
* academic-session changes

Historical records MUST reference the historical entities or snapshots required to reproduce their meaning.

---

# 64. Configuration Data

System configuration SHOULD be stored separately from ordinary academic data.

Examples include:

* local server settings
* synchronization configuration
* assessment defaults
* AI configuration
* logging configuration
* backup configuration

Sensitive configuration values MUST use secure secret storage rather than ordinary database fields where appropriate.

---

# 65. AI Data Model

AI-related records SHOULD include:

* conversation
* message
* model metadata
* retrieval reference
* content chunk reference
* timestamp
* user ownership

AI conversation data MUST be separated from official academic records.

---

# 66. RAG Data

The RAG subsystem SHOULD maintain:

```text
Content Source
    ↓
Content Version
    ↓
Content Chunk
    ↓
Embedding
    ↓
Vector Index
```

The exact vector implementation MAY be selected during technical implementation.

The data model MUST preserve the relationship between retrieved content and its original approved source.

---

# 67. RAG Authorization

Every content chunk used by retrieval MUST have sufficient metadata to determine whether the requesting user may access it.

Draft or restricted content MUST NOT become globally retrievable merely because it has an embedding.

---

# 68. Database Constraints

Where practical, the database MUST enforce:

* primary keys
* foreign keys
* uniqueness
* required fields
* valid enum/state values
* non-negative numeric constraints
* date consistency
* relationship cardinality

Application-level validation MUST complement database constraints rather than replace them.

---

# 69. Transaction Boundaries

Operations that modify multiple related records MUST use transactions.

Examples:

* creating a user and required profile
* approving a question version
* starting an assessment attempt
* finalizing an assessment
* executing promotion
* applying a synchronization operation

Partial state MUST NOT be left behind after failed transactional operations.

---

# 70. Promotion Transactions

Promotion MUST be treated as a controlled transactional operation.

The operation SHOULD:

1. validate eligible students,
2. validate destination classes/arms,
3. create new placements,
4. preserve previous placements,
5. record promotion history,
6. record administrator authorization,
7. commit atomically.

If a promotion run fails, the system MUST avoid partially promoted data.

---

# 71. Indexing Strategy

Indexes MUST be created based on actual query patterns.

Expected indexed fields include combinations involving:

* user login identifier
* student ID
* parent ID
* teacher ID
* academic session
* term
* class
* arm
* subject
* topic
* assessment
* student + assessment
* attempt status
* sync status
* revision
* timestamps

Indexes MUST be reviewed against production-like query patterns.

---

# 72. Over-Indexing

The project MUST avoid creating indexes for every field automatically.

Excessive indexes increase:

* storage
* write cost
* migration complexity
* maintenance overhead

Indexes SHOULD be justified by real query or constraint requirements.

---

# 73. Query Performance

Database queries MUST avoid:

* unnecessary full-table scans
* N+1 access patterns
* unbounded result sets
* repeated expensive aggregation
* fetching unused columns
* loading large binary content unnecessarily

Pagination MUST be used for potentially large collections.

---

# 74. Data Pagination

APIs returning collections MUST use controlled pagination where datasets may grow.

Pagination SHOULD support stable ordering.

Offset pagination MAY be used for small administrative datasets.

Cursor/keyset pagination SHOULD be considered for high-volume activity streams.

---

# 75. SQLite WAL Requirements

The local application MUST:

* enable WAL appropriately
* configure busy handling
* keep transactions short
* avoid long-running write transactions
* monitor database health
* checkpoint WAL safely
* handle unexpected shutdown

SQLite corruption MUST be treated as a critical operational condition.

---

# 76. Local Database Recovery

The local server MUST be able to detect:

* missing database
* corrupted database
* invalid migration state
* failed integrity checks
* unavailable storage

The system MUST fail safely rather than silently continuing with corrupt data.

---

# 77. Database Integrity Checks

The local system SHOULD periodically perform appropriate integrity checks.

Backups SHOULD also be verified where practical.

Database integrity failures MUST generate visible operational alerts.

---

# 78. PostgreSQL Transactions and Isolation

Online transactional operations MUST use appropriate PostgreSQL transaction isolation.

The implementation MUST explicitly identify operations that require stronger consistency guarantees.

Concurrent updates MUST not silently overwrite one another.

---

# 79. Concurrency Control

The application MUST use appropriate mechanisms for concurrent modifications.

Possible mechanisms include:

* optimistic concurrency
* revision checks
* row locking
* transactional updates

The chosen mechanism MUST match the business operation.

---

# 80. Migration Strategy

Database schema changes MUST be version-controlled.

Each migration MUST:

* have a unique version
* be deterministic
* be reviewable
* support controlled deployment
* avoid destructive data loss unless explicitly approved

---

# 81. Migration Compatibility

For production changes where necessary, migrations SHOULD follow:

```text
Expand
    ↓
Migrate
    ↓
Adopt
    ↓
Contract
```

This reduces downtime and protects compatibility during rolling updates.

---

# 82. Local Database Migrations

The local server manager MUST control local database migrations.

A local application update MUST:

1. detect current schema version,
2. verify migration compatibility,
3. back up where required,
4. apply migrations,
5. validate database integrity,
6. start the application only after successful migration.

Failed migrations MUST stop normal startup rather than leaving an unknown schema state.

---

# 83. Online Database Migrations

Online migrations MUST be executed through controlled deployment processes.

Production database migrations MUST NOT be performed manually through arbitrary SQL commands unless the operation is documented and approved.

---

# 84. Data Retention

Retention policies MUST be defined for:

* audit records
* login/security events
* assessment events
* AI conversations
* uploaded source documents
* processing artifacts
* synchronization records
* backups

Retention MUST balance:

* operational requirements
* auditability
* storage consumption
* privacy
* recovery requirements

---

# 85. Personal Data Minimization

The LMS MUST avoid storing unnecessary personal information.

The database MUST NOT become a general-purpose HR or admissions database.

Only data required by the defined LMS scope should be stored.

---

# 86. Data Export

Administrative exports MUST:

* require authorization
* be logged
* use controlled formats
* avoid exposing unrelated records
* protect sensitive information

Large exports SHOULD be generated asynchronously.

---

# 87. Data Import

Imports MUST:

* validate structure
* validate references
* detect duplicates
* preserve source metadata
* produce actionable errors
* avoid partial corruption

Bulk imports SHOULD use transactional batches where appropriate.

---

# 88. Referential Integrity During Synchronization

Synchronization MUST respect dependency order.

For example:

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

Dependent records MUST NOT be applied before their required references exist.

---

# 89. Synchronization Tombstones

Where hard deletion is synchronized, the system MUST retain sufficient tombstone information to prevent deleted records from being unintentionally recreated by stale peers.

Deletion handling MUST therefore be designed explicitly.

---

# 90. Data Conflict Rules

Different entities MAY use different conflict strategies.

Examples:

| Entity                      | Expected Strategy                          |
| --------------------------- | ------------------------------------------ |
| User account                | Controlled administrative authority        |
| Role assignment             | Administrative authority                   |
| Academic structure          | Online/master authority                    |
| Teacher assignment          | Administrative authority                   |
| Question publication        | Controlled content authority               |
| Active CBT attempt          | Local authoritative during offline attempt |
| Completed assessment result | Preserve immutable result                  |
| Practice history            | Merge/idempotent event strategy            |
| Analytics                   | Recalculate where practical                |
| Audit record                | Append-only                                |

These are architectural defaults and MUST be validated during synchronization implementation.

---

# 91. Immutability

The following records SHOULD be treated as immutable after completion:

* finalized assessment attempts
* finalized assessment results
* audit records
* historical question versions used by completed assessments
* promotion history

Corrections MUST use controlled correction records rather than silent mutation.

---

# 92. Correction Records

When an authorized correction is required, the system SHOULD record:

* original value
* corrected value
* reason
* actor
* timestamp
* approval where required

This maintains an audit trail.

---

# 93. Database Backup Architecture

The online system SHOULD use:

* scheduled PostgreSQL backups
* point-in-time recovery where supported and justified
* backup integrity checks

The local system SHOULD use:

* scheduled SQLite backups
* safe-copy procedures
* retention rotation
* restoration verification

---

# 94. Backup During CBT

Backups MUST NOT interfere with active CBT operation.

The local server MUST prioritize:

1. assessment integrity
2. answer persistence
3. transaction completion
4. normal operation

Backup operations SHOULD be designed to avoid long write locks.

---

# 95. Storage Management

The local server manager SHOULD monitor:

* database size
* WAL size
* backup size
* available disk space
* uploaded content storage
* AI/RAG storage

Low disk space MUST produce an operational warning before the system reaches failure conditions.

---

# 96. Database Observability

Database monitoring SHOULD include:

* connection health
* query latency
* failed queries
* transaction failures
* database size
* storage availability
* WAL behavior
* migration state
* backup state

Sensitive SQL parameters MUST NOT be logged.

---

# 97. Database Security

Database services MUST NOT be exposed unnecessarily.

PostgreSQL SHOULD be accessible only by authorized application and administrative networks.

SQLite MUST be protected by operating-system filesystem permissions.

Database credentials MUST be stored securely.

---

# 98. Testing Requirements

Database implementation MUST include:

### Unit Tests

* validation
* state transitions
* domain constraints

### Integration Tests

* transactions
* relationships
* authorization
* migrations
* synchronization

### Data Integrity Tests

* foreign keys
* uniqueness
* promotion integrity
* parent-student cardinality
* assessment integrity

### Performance Tests

* expected dashboard queries
* student history
* assessment retrieval
* question selection
* synchronization queues

### Recovery Tests

* interrupted transactions
* database restart
* application crash
* local server restart
* failed migration
* backup restoration

---

# 99. Database Definition of Done

The database layer is complete only when:

1. Entities are documented.
2. Ownership is documented.
3. Relationships are defined.
4. Constraints are implemented.
5. Migrations exist.
6. Indexes are justified.
7. Authorization boundaries are tested.
8. Synchronization metadata is implemented where required.
9. Historical integrity is verified.
10. Backup and restoration are tested.
11. Local offline workflows work without Internet access.
12. Database failure scenarios are tested.
13. Performance is tested using realistic data.
14. Documentation matches implementation.

---

# 100. Database Non-Negotiables

The following MUST NOT be violated:

1. PostgreSQL remains the online master database.
2. SQLite with WAL remains the local operational database.
3. Local and online databases are not treated as blind replicas.
4. Synchronization uses explicit records/revisions.
5. Parent-student relationships are explicit.
6. One student cannot have multiple active parent accounts.
7. Academic placement history is preserved.
8. Question versions used by completed assessments remain reproducible.
9. CBT state remains recoverable.
10. Assessment scoring remains deterministic.
11. AI data remains separate from authoritative academic records.
12. Audit records remain protected.
13. Database migrations are version-controlled.
14. Backups are tested through restoration.
15. No destructive schema change is introduced without a controlled migration.

---

# 101. Final Database Principle

The database is not merely a storage layer.

It is part of the system's integrity boundary.

CRC MUST therefore treat data modeling, transactions, constraints, revisions, history, synchronization and recovery as core system behavior.

The final architecture MUST ensure that:

> **No network failure, client failure, synchronization failure, application restart or administrative operation can silently destroy the integrity of authoritative academic records.**
