# Synchronization Architecture

## 1. Purpose

This document defines the architecture and operational rules for synchronization between the NAKAMA online master system and each school-local NAKAMA server.

The synchronization system exists to maintain controlled consistency between:

* the online master system;
* the school-local server;
* local academic activity;
* online dashboards;
* configuration and reference data;
* assessment results;
* progress and practice records;
* audit information;
* system health and synchronization state.

Synchronization is not implemented as database copying.

The system uses an application-level synchronization protocol based on:

* globally unique identifiers;
* revision tracking;
* durable change queues;
* idempotent operations;
* explicit data authority;
* dependency-aware ordering;
* conflict detection;
* entity-specific conflict resolution;
* acknowledgements;
* retry and backoff;
* synchronization cursors;
* auditability;
* integrity verification.

The objective is reliable convergence without corrupting historical academic data or allowing one side of the system to silently overwrite authoritative information.

---

# 2. Scope

This document covers:

* online-to-local synchronization;
* local-to-online synchronization;
* synchronization identity;
* entity authority;
* change tracking;
* revisions;
* durable outbox/inbox processing;
* push/pull synchronization;
* acknowledgement;
* idempotency;
* dependency ordering;
* batching;
* retries;
* conflict detection;
* conflict resolution;
* tombstones;
* schema compatibility;
* partial failures;
* integrity verification;
* security;
* synchronization observability;
* Super Admin synchronization management;
* local server synchronization management;
* disaster recovery;
* synchronization testing.

This document does not define:

* general authentication architecture;
* the complete database schema;
* the complete API specification;
* content-processing internals;
* AI/RAG implementation;
* general application analytics.

Those systems integrate with synchronization through the interfaces defined here.

---

# 3. Synchronization Principles

The synchronization system MUST follow these principles.

### 3.1 No database replication by file copying

The local SQLite database MUST NOT be synchronized by copying database files to or from the online PostgreSQL database.

Synchronization operates through application-level records and operations.

### 3.2 No blind overwrites

A synchronization operation MUST NOT blindly replace an existing record because it is newer according to a client clock.

### 3.3 Explicit authority

Every synchronized entity MUST have a defined authority model.

The system MUST know whether:

* the online system is authoritative;
* the local system is authoritative;
* both sides may modify the entity;
* the entity is append-only;
* the entity is derived and should be recomputed.

### 3.4 At-least-once delivery

Transport MUST assume that messages may be delivered more than once.

Application processing MUST therefore be idempotent.

### 3.5 Historical academic data is protected

Completed assessments, scores, published versions, audit events, and other historical records MUST NOT be silently overwritten during synchronization.

### 3.6 Synchronization must survive interruption

A process interrupted by:

* network failure;
* application crash;
* server restart;
* Windows shutdown;
* timeout;
* power failure;

MUST resume safely.

### 3.7 Every synchronization operation must be traceable

The system MUST be able to determine:

* what was sent;
* what was received;
* when it happened;
* which installation initiated it;
* whether it succeeded;
* whether it was retried;
* whether it conflicted;
* how the conflict was resolved.

---

# 4. Synchronization Topology

The system uses the following topology:

```text
                    INTERNET
                       │
                       ▼
             ┌─────────────────────┐
             │   ONLINE MASTER     │
             │                     │
             │ PostgreSQL          │
             │ Sync API            │
             │ Sync Processor      │
             │ Change Journal      │
             └──────────┬──────────┘
                        │
                  Secure Sync
                        │
             ┌──────────▼──────────┐
             │  SCHOOL LOCAL       │
             │      SERVER         │
             │                     │
             │ SQLite WAL          │
             │ Sync Engine         │
             │ Durable Outbox      │
             │ Local Change Log    │
             └──────────┬──────────┘
                        │
                     SCHOOL LAN
                        │
             ┌──────────┼──────────┐
             ▼          ▼          ▼
          Student     Teacher    Admin
          Devices     Devices    Devices
```

Each school installation has a unique synchronization identity.

The online system treats each local server as a registered installation belonging to a specific school/tenant.

---

# 5. Installation Identity

Every local installation MUST have a globally unique `installation_id`.

The installation identity MUST NOT depend on:

* computer hostname;
* IP address;
* Windows username;
* MAC address alone;
* local database filename.

The installation identity survives normal server restarts and network changes.

The registration record SHOULD contain:

* installation ID;
* school ID;
* installation status;
* installation version;
* schema version;
* synchronization protocol version;
* registration timestamp;
* last successful synchronization;
* last failed synchronization;
* server health;
* credential status;
* installation metadata required for diagnostics.

An installation MUST be explicitly registered before it is allowed to synchronize.

---

# 6. Data Authority Model

Every synchronized entity MUST belong to one of the following authority categories.

## 6.1 Online-authoritative

The online master is authoritative.

Examples include:

* global account lifecycle;
* academic configuration;
* Super Admin configuration;
* teacher assignments;
* published configuration;
* school-wide administrative settings.

Local systems may receive these changes but cannot silently override them.

---

## 6.2 Local-authoritative

The school local server is authoritative for defined operational records while offline.

The primary example is an active CBT attempt.

For an active assessment:

```text
Student Device
      ↓
Local Server
      ↓
Active Attempt State
```

The local server is authoritative until the attempt is completed and synchronized.

---

## 6.3 Shared-authority

Both sides may create or modify records under controlled rules.

Examples may include:

* selected progress events;
* practice activity;
* locally generated operational events;
* permitted configuration changes.

Shared entities MUST use revision checks and conflict handling.

---

## 6.4 Append-only

Some records MUST never be updated by replacement.

Examples:

* audit events;
* assessment event history;
* synchronization events;
* immutable result events.

These records are synchronized by unique event identity.

---

## 6.5 Derived

Derived data is not treated as authoritative synchronization data.

Examples:

* dashboards;
* aggregates;
* learning-gap summaries;
* reporting metrics;
* cached statistics.

Where possible, derived values SHOULD be recomputed from authoritative source records instead of resolving conflicting aggregate values.

---

# 7. Synchronizable Entity Categories

Synchronization MUST support the following broad entity categories.

### Identity

* users;
* account states;
* role assignments;
* password/credential metadata required for offline operation;
* parent-student relationships.

### Academic

* sessions;
* terms;
* classes;
* arms;
* subjects;
* topics;
* student placements;
* teacher assignments;
* promotion records.

### Question and Content

* question definitions;
* question versions;
* answer options;
* media references;
* approved content;
* publication state;
* content-processing metadata where required.

### Assessment

* assessment configuration;
* assessment questions;
* attempts;
* answers;
* attempt events;
* submissions;
* scores;
* completion records.

### Learning Activity

* practice sessions;
* practice answers;
* progress events;
* learning activity records.

### Operational

* synchronization records;
* installation metadata;
* server health;
* security events;
* audit records.

### Derived

* analytics;
* dashboard summaries;
* learning-gap calculations.

Derived records SHOULD generally be recalculated rather than treated as independently authoritative data.

---

# 8. Global Identifiers

Every synchronized business entity MUST have a globally unique identifier.

UUIDs SHOULD be used.

The identifier MUST remain stable across:

* online synchronization;
* local storage;
* retries;
* migration;
* backup restoration;
* replication.

An entity MUST NOT receive a new identifier merely because it moves from local to online storage.

---

# 9. Revision Model

Mutable synchronized entities MUST maintain revision information.

A revision SHOULD contain:

* entity ID;
* entity type;
* revision number;
* originating installation;
* originating operation ID;
* creation/update timestamp;
* schema version;
* previous revision reference where applicable.

Example:

```text
Entity:
  question_id = Q123

Revision:
  revision = 7
  previous_revision = 6
  origin = ONLINE
  operation_id = OP-789
```

Revision numbers MUST be generated by a trusted persistence layer and MUST NOT depend solely on client clocks.

---

# 10. Change Tracking

Every synchronizable mutation MUST generate a change record.

The change record SHOULD contain:

```text
operation_id
entity_id
entity_type
operation_type
source_installation_id
source_revision
base_revision
created_at
payload_reference
dependency_information
schema_version
checksum
```

Supported operation types include:

* CREATE;
* UPDATE;
* DELETE;
* PUBLISH;
* UNPUBLISH;
* COMPLETE;
* APPEND;
* PROMOTE;
* REVOKE.

Only valid operations defined for the entity are permitted.

---

# 11. Durable Local Outbox

Every local change that requires synchronization MUST first be persisted into a durable outbox.

The transaction MUST ensure that:

```text
Business Change
      +
Outbox Entry
```

are committed atomically.

This prevents the following failure:

```text
Database updated
       ↓
Server crashes
       ↓
Sync operation forgotten
```

The correct sequence is:

```text
Transaction
 ├── Apply business change
 └── Create outbox record
          ↓
       Commit
          ↓
   Sync worker processes
```

Outbox records MUST survive:

* restart;
* application crash;
* temporary network loss;
* sync worker failure.

---

# 12. Online Change Journal

The online master MUST maintain a durable change journal for changes that local installations need to receive.

The journal provides the basis for incremental downstream synchronization.

The online system MUST NOT require every local server to repeatedly download the entire database.

Changes SHOULD be addressable using:

* cursor;
* sequence;
* revision;
* entity ID;
* installation scope.

---

# 13. Synchronization Handshake

Before synchronization begins, the local server performs a handshake.

The handshake MUST establish:

* installation identity;
* school identity;
* authentication;
* protocol version;
* schema version;
* application version;
* supported capabilities;
* last known upstream cursor;
* last known downstream acknowledgement;
* synchronization state.

Conceptually:

```text
LOCAL → ONLINE
HELLO
INSTALLATION_ID
PROTOCOL_VERSION
SCHEMA_VERSION
CURSOR
CAPABILITIES
```

The online server validates the installation before accepting synchronization operations.

---

# 14. Protocol Version Negotiation

The synchronization protocol MUST be versioned independently from the application.

Example:

```text
Sync Protocol: v1
Application: 1.8.0
Database Schema: 14
```

A protocol mismatch that could corrupt data MUST cause synchronization to stop safely.

Compatible versions MAY continue operating through backward-compatible protocol rules.

---

# 15. Push/Pull Synchronization

Synchronization operates as a controlled push/pull cycle.

### Phase 1 — Handshake

Authenticate installation and negotiate compatibility.

### Phase 2 — Push

Local outbox operations are uploaded.

### Phase 3 — Process

Online server validates and applies acceptable operations.

### Phase 4 — Acknowledge

Each operation receives an outcome.

Possible outcomes:

* ACCEPTED;
* ALREADY_APPLIED;
* CONFLICT;
* REJECTED;
* INVALID;
* RETRYABLE_FAILURE.

### Phase 5 — Pull

Local server requests changes after its last confirmed cursor.

### Phase 6 — Apply

Remote changes are applied transactionally to the local database.

### Phase 7 — Acknowledge

Local server confirms successful application.

### Phase 8 — Advance Cursor

The synchronization cursor advances only after successful application.

---

# 16. Acknowledgements

Acknowledgements MUST be explicit.

An acknowledgement SHOULD identify:

```text
operation_id
entity_id
status
server_revision
error_code
conflict_id
processed_at
```

The local server MUST NOT delete an outbox record merely because the request was transmitted.

The record is removed or marked completed only after a valid acknowledgement is received.

---

# 17. Idempotency

Every synchronization operation MUST have a unique `operation_id`.

The receiving system MUST maintain an idempotency record.

If the same operation is received twice:

```text
First request  → Applied
Second request → ALREADY_APPLIED
```

The second request MUST NOT create a duplicate entity or apply the mutation twice.

This allows safe retries after ambiguous failures.

---

# 18. Exactly-Once vs At-Least-Once

The transport layer MUST use at-least-once delivery semantics.

The system MUST NOT depend on network-level exactly-once delivery.

Instead:

```text
At-least-once transport
        +
Idempotent processing
        =
Effectively-once business application
```

This is the required synchronization reliability model.

---

# 19. Dependency Ordering

Operations MUST respect entity dependencies.

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
      ↓
Assessment
      ↓
Attempt
```

A dependent operation MUST NOT be applied before the required parent entity exists.

If an operation arrives before its dependency:

* it may be deferred;
* placed in a dependency queue;
* retried after the dependency arrives.

It MUST NOT be silently discarded.

---

# 20. Batching

Synchronization SHOULD use batches to reduce network overhead.

A batch MAY contain:

* multiple operations;
* multiple entities;
* multiple event records.

However, the system MUST maintain per-operation status.

A batch failure MUST NOT automatically imply that every operation failed.

Example:

```text
Batch
 ├── OP-001 → ACCEPTED
 ├── OP-002 → CONFLICT
 ├── OP-003 → ACCEPTED
 └── OP-004 → RETRYABLE_FAILURE
```

The local server must preserve the individual states.

---

# 21. Retry Strategy

Retryable failures MUST use controlled retry.

Recommended strategy:

```text
Initial retry
      ↓
Exponential backoff
      ↓
Jitter
      ↓
Maximum retry interval
      ↓
Dead-letter / manual intervention
```

Retries MUST NOT create duplicate business operations because of idempotency protection.

Permanent failures MUST not be retried indefinitely.

---

# 22. Connectivity States

The local synchronization engine SHOULD expose states such as:

```text
ONLINE
CONNECTING
AUTHENTICATING
SYNCING
PARTIALLY_SYNCED
OFFLINE
BACKOFF
ERROR
BLOCKED
```

The state must be visible to the local server manager.

The system MUST distinguish:

* no internet;
* authentication failure;
* server unavailable;
* protocol mismatch;
* data conflict;
* permanent rejection.

These are different operational conditions.

---

# 23. Conflict Detection

A conflict exists when the incoming operation cannot safely be applied against the current entity state.

For mutable records, optimistic concurrency SHOULD use the expected base revision.

Example:

```text
Online revision = 10

Local change based on revision = 9
```

If revision 10 has already changed the entity, the local update cannot simply overwrite it.

The operation becomes:

```text
CONFLICT
```

and is handled according to the entity-specific policy.

---

# 24. Conflict Resolution Strategy

There is no universal conflict resolver.

The system MUST use entity-specific conflict policies.

The following policies apply.

---

## 24.1 Immutable Events

For immutable events:

```text
operation_id/event_id
        ↓
deduplicate
        ↓
append
```

There is no overwrite conflict.

Duplicate events are ignored through idempotency.

---

## 24.2 Online-Authoritative Configuration

For centrally managed configuration:

```text
ONLINE = authoritative
LOCAL = replica
```

Local unauthorized changes are rejected.

The local server receives the current authoritative configuration.

---

## 24.3 Active CBT Attempts

For an active CBT attempt:

```text
LOCAL SERVER = authoritative
```

while the attempt is active.

Student devices communicate with the local server.

The online system must not overwrite active local attempt state.

After completion:

```text
Completed Attempt
       ↓
Immutable Result Package
       ↓
Sync to Online
```

---

## 24.4 Completed Assessment Results

Completed assessment results are treated as protected historical records.

A synchronized result MUST NOT be silently replaced.

If correction is legitimately required:

```text
Original Result
      ↓
Correction Event
      ↓
Audit Record
      ↓
Revised Derived State
```

The original state remains historically traceable.

---

## 24.5 Published Questions

Published question versions are immutable.

Changes create a new version.

Example:

```text
Question Q1
   ├── Version 1
   ├── Version 2
   └── Version 3
```

A published version cannot be silently rewritten.

---

## 24.6 Shared Editable Records

Where both sides are legitimately allowed to edit an entity:

```text
Compare base revision
        ↓
No divergence → apply
        ↓
Divergence → conflict
```

The conflict record preserves:

* local version;
* remote version;
* base version;
* source;
* timestamps;
* operation IDs.

Resolution requires an explicit strategy.

---

## 24.7 Progress and Practice

Progress should preferably be represented as events or deterministic activity records.

For example:

```text
Practice Started
Question Answered
Practice Completed
Topic Attempted
```

These events can be merged by unique event ID.

Aggregated progress values SHOULD be recalculated from authoritative activity where practical.

---

## 24.8 Analytics

Analytics are derived.

Conflicting analytics values SHOULD NOT be manually merged.

Instead:

```text
Authoritative Events
        ↓
Analytics Pipeline
        ↓
Recomputed Metrics
```

---

# 25. Account and Credential Synchronization

Identity synchronization requires special handling.

The online system is authoritative for global account lifecycle.

The local server maintains the minimum credential information necessary for authorized offline login.

The system MUST NOT synchronize plaintext passwords.

Credential changes MUST be represented as protected credential revisions or environment-specific verifiers.

Credential metadata MUST be:

* encrypted in transit;
* protected at rest;
* access-controlled;
* excluded from ordinary synchronization logs.

If an account is globally deactivated while the local server is offline, the local server cannot know immediately.

Therefore:

* local emergency disable MUST be possible where required;
* the disable event MUST be recorded;
* the state MUST synchronize upstream when connectivity returns;
* centrally authoritative account state takes precedence after reconciliation unless a documented security override applies.

---

# 26. Academic Configuration Synchronization

Academic configuration is primarily online-authoritative.

This includes:

* sessions;
* terms;
* classes;
* arms;
* subjects;
* topics;
* teacher assignments;
* student placements;
* promotion configuration.

The local server receives approved changes through synchronization.

Academic configuration changes MUST NOT invalidate active CBT attempts unexpectedly.

Where a change would affect active assessments, the system MUST apply controlled versioning rather than mutating the active configuration.

---

# 27. Question and Content Synchronization

Question synchronization MUST be version-based.

A question publication package SHOULD identify:

* question ID;
* version ID;
* content;
* answer options;
* correct answer representation;
* media references;
* mathematical/rich content;
* topic;
* subject;
* publication state;
* content version;
* integrity checksum.

Only approved/published versions may become available for student assessment.

The local server MUST retain enough version information to reproduce the assessment accurately.

---

# 28. Assessment Configuration Synchronization

Assessment configuration SHOULD be versioned.

Configuration may include:

* assessment type;
* subject;
* class/arm scope;
* question count;
* duration;
* attempt limit;
* randomization;
* option randomization;
* navigation rules;
* scoring rules;
* availability window.

Once an assessment has active attempts, its effective configuration MUST be immutable for those attempts.

New configuration changes apply to a new assessment/configuration revision.

---

# 29. CBT Attempt Synchronization

CBT synchronization follows this lifecycle:

```text
Assessment Configuration
        ↓
Local Attempt Created
        ↓
Local Answer Events
        ↓
Local Autosave
        ↓
Local Submission
        ↓
Deterministic Scoring
        ↓
Immutable Completion Record
        ↓
Durable Outbox
        ↓
Online Synchronization
```

The active attempt itself is local-authoritative.

After completion, the result package becomes synchronization-safe and historically protected.

A completed attempt MUST contain sufficient information to reproduce:

* assessment identity;
* configuration/version;
* question versions;
* selected answers;
* answer events where required;
* submission time;
* score;
* scoring version;
* integrity metadata.

---

# 30. Practice and Progress Synchronization

Practice records should be represented using durable activity events.

Example:

```text
PracticeSessionCreated
QuestionPresented
QuestionAnswered
QuestionCompleted
PracticeSessionCompleted
```

The system can then derive:

* attempt counts;
* accuracy;
* topic progress;
* subject progress;
* weak-topic indicators.

This reduces conflict risk compared with synchronizing repeatedly overwritten aggregate counters.

---

# 31. Promotion Synchronization

Promotion is an administrative operation.

A promotion run MUST be represented as an explicit operation with:

* promotion run ID;
* academic session;
* source class/arm;
* destination class/arm;
* affected students;
* activation user;
* timestamp;
* configuration version.

The operation MUST be transactionally applied.

Historical placement records MUST remain intact.

Synchronization MUST NOT cause a promotion to execute twice.

Idempotency is mandatory for promotion operations.

---

# 32. Tombstones and Deletions

Physical deletion of synchronized records SHOULD be avoided where historical integrity matters.

Instead, the system SHOULD use tombstones.

Example:

```text
Entity exists
      ↓
DELETE requested
      ↓
Tombstone created
      ↓
Tombstone synchronized
      ↓
All replicas mark deleted
```

Tombstones MUST remain long enough to prevent an old replica from accidentally resurrecting deleted data.

Historical academic records SHOULD generally be retained rather than physically deleted.

---

# 33. Schema Compatibility

A local server may temporarily run a different application or schema version from the online master.

The synchronization protocol MUST detect incompatibility before applying unsafe changes.

Compatibility should be based on:

* protocol version;
* schema version;
* feature capability;
* entity version;
* migration state.

Unsupported operations MUST fail safely.

They MUST NOT be partially applied.

---

# 34. Synchronization Migrations

Schema migrations affecting synchronized entities MUST follow the project's migration strategy.

Where possible:

```text
Expand
  ↓
Deploy compatible version
  ↓
Backfill
  ↓
Synchronize
  ↓
Adopt new structure
  ↓
Contract
```

A migration MUST NOT require simultaneous destructive changes across every local installation.

Local servers may reconnect after a period of being offline.

The synchronization system MUST therefore tolerate supported version skew.

---

# 35. Partial Failure Handling

Synchronization MUST be designed around partial failure.

Examples:

### Request sent but response lost

The operation is retried using the same `operation_id`.

### Server processed request but local server crashed

Retry returns:

```text
ALREADY_APPLIED
```

### Batch partially processed

Only failed operations are retried.

### Local database transaction commits but process crashes before acknowledgement

The operation remains recoverable.

### Remote change applied locally but acknowledgement is lost

The remote change is safely replayed using its identifier/cursor.

---

# 36. Transaction Boundaries

Synchronization application MUST use database transactions.

For incoming local changes:

```text
Validate
   ↓
Authorize
   ↓
Check idempotency
   ↓
Check dependency
   ↓
Check revision
   ↓
Apply mutation
   ↓
Record synchronization result
   ↓
Commit
```

The business mutation and synchronization bookkeeping MUST be committed atomically where practical.

For incoming remote changes:

```text
Validate
   ↓
Check duplicate
   ↓
Apply entity change
   ↓
Record applied remote operation
   ↓
Advance applicable cursor
   ↓
Commit
```

The cursor MUST NOT advance independently of successful application.

---

# 37. Integrity Verification

Synchronization payloads SHOULD contain integrity metadata.

Possible mechanisms include:

* SHA-256 checksums;
* canonical payload hashing;
* signed metadata where required;
* content-addressed media identifiers.

The receiving side MUST verify the payload before applying it.

A failed integrity check MUST cause the operation to be rejected or retried.

It MUST NOT be partially applied.

---

# 38. Security of Synchronization

Synchronization is a security-sensitive system boundary.

All online synchronization traffic MUST use secure encrypted transport.

The minimum expected controls include:

* HTTPS/TLS;
* installation authentication;
* scoped credentials;
* credential rotation;
* server certificate validation;
* replay protection;
* request expiration where appropriate;
* idempotency keys;
* authorization by installation and school;
* input validation;
* payload size limits;
* rate limiting;
* audit logging.

The online system MUST never trust a local server simply because it presents a valid school identifier.

---

# 39. Installation Credentials

Each local server requires credentials specifically for synchronization.

These credentials MUST be distinct from:

* Super Admin passwords;
* student passwords;
* teacher passwords;
* parent passwords.

Credentials SHOULD support:

* rotation;
* revocation;
* expiration where appropriate;
* installation-specific scope.

Secrets MUST NOT be stored in plaintext configuration files.

---

# 40. Replay Protection

The synchronization service MUST protect against replayed requests.

Protection SHOULD combine:

* operation IDs;
* timestamps;
* nonce/request identifiers where appropriate;
* server-side idempotency records;
* authenticated transport.

An already processed operation MUST never be applied again.

---

# 41. Audit Logging

Synchronization events MUST generate auditable records.

Important events include:

* installation registration;
* authentication;
* synchronization start;
* synchronization completion;
* operation accepted;
* operation rejected;
* operation retried;
* conflict detected;
* conflict resolved;
* credential rotation;
* synchronization blocked;
* protocol mismatch;
* integrity failure;
* manual reprocess;
* manual conflict resolution.

Logs MUST include correlation identifiers.

---

# 42. Super Admin Synchronization Dashboard

Super Admin MUST have a dedicated synchronization management dashboard.

The dashboard SHOULD provide:

### Overall Status

* synchronization health;
* online availability;
* number of connected installations;
* installations currently offline;
* stale installations.

### Installation Status

For each school installation:

* school;
* installation ID;
* application version;
* schema version;
* protocol version;
* current status;
* last successful sync;
* last failed sync;
* pending operations;
* conflicts;
* failed operations;
* server health.

### Queue Status

* pending outbound;
* pending inbound;
* processing;
* retrying;
* failed;
* dead-lettered;
* completed.

### Conflict Status

* total conflicts;
* unresolved conflicts;
* conflict type;
* affected entity;
* source versions;
* resolution state.

### Throughput

* operations processed;
* records per batch;
* average synchronization duration;
* retry count;
* failure rate;
* backlog size.

---

# 43. Local Server Synchronization Dashboard

The Local Server Manager MUST expose synchronization state.

It SHOULD show:

```text
Connection: Connected
Status: Synchronizing
Last Success: 10:42:12
Pending Uploads: 12
Pending Downloads: 4
Conflicts: 0
Failed Operations: 1
Next Retry: 10:45:00
```

The dashboard SHOULD provide:

* sync now;
* pause synchronization;
* resume synchronization;
* retry failed operations;
* inspect errors;
* inspect recent operations;
* view server connectivity;
* view installation identity;
* view protocol/schema compatibility.

Manual controls MUST be permission-protected.

---

# 44. Manual Conflict Resolution

Conflicts MUST NOT be silently hidden.

Where manual resolution is permitted, the administrator SHOULD see:

```text
Entity
Base Version
Local Version
Remote Version
Changed Fields
Source
Timestamp
Recommended Action
```

The administrator may be permitted to:

* accept local;
* accept remote;
* create a corrected version;
* defer resolution.

The selected resolution MUST itself be audited.

For protected historical records, resolution MUST use a correction/versioning workflow rather than destructive overwrite.

---

# 45. Manual Reprocessing

Super Admin MAY retry or reprocess a failed synchronization operation.

Reprocessing MUST:

* preserve the original operation ID where appropriate;
* create an audit record;
* maintain the original failure reason;
* avoid duplicate application;
* respect current authorization and schema rules.

Administrators MUST NOT be given a generic "force database overwrite" button.

---

# 46. Observability

Synchronization metrics SHOULD include:

* synchronization attempts;
* successful synchronizations;
* failed synchronizations;
* operations sent;
* operations received;
* operations accepted;
* operations rejected;
* conflicts;
* retry counts;
* backlog size;
* queue age;
* synchronization latency;
* batch size;
* throughput;
* payload size;
* integrity failures;
* authentication failures.

Important alerts include:

* prolonged synchronization failure;
* growing queue backlog;
* stale installation;
* repeated conflicts;
* repeated authentication failures;
* protocol mismatch;
* schema incompatibility;
* database storage pressure;
* integrity verification failures.

---

# 47. Backpressure

The synchronization engine MUST protect the local server and online service from uncontrolled backlog processing.

When a backlog is large, the system SHOULD:

* batch operations;
* limit concurrency;
* prioritize critical operations;
* apply rate limits;
* monitor queue age;
* prevent memory exhaustion.

Critical academic results SHOULD receive appropriate priority over low-priority telemetry.

---

# 48. Synchronization Priority

Where prioritization is necessary, operations MAY be classified as:

### Critical

* completed CBT results;
* assessment completion;
* security events;
* account security changes.

### High

* academic configuration;
* teacher assignments;
* student placement changes.

### Normal

* practice activity;
* progress events.

### Low

* derived analytics;
* non-critical telemetry.

Priority MUST NOT violate dependency ordering or transactional integrity.

---

# 49. Disaster Recovery

Synchronization state MUST be recoverable after local server restoration.

Backups MUST include the information necessary to reconstruct:

* local data;
* outbox state;
* synchronization cursor;
* installation identity;
* relevant synchronization metadata.

Restoring a backup MUST NOT cause previously synchronized operations to be applied twice.

This is another reason operation IDs and durable idempotency records are mandatory.

---

# 50. Server Replacement

If a school server PC is replaced:

```text
Old Installation
      ↓
Retire/disable
      ↓
New Installation
      ↓
Register
      ↓
Secure synchronization bootstrap
      ↓
Initial controlled synchronization
```

The replacement MUST NOT simply clone the identity of another installation without an explicit recovery procedure.

The online system MUST be able to identify the replacement installation.

---

# 51. Initial Synchronization

A new local installation requires a controlled bootstrap.

The process SHOULD be:

```text
Register Installation
        ↓
Authenticate
        ↓
Validate Versions
        ↓
Download Required Academic Configuration
        ↓
Download Authorized Content
        ↓
Download Required Identity Data
        ↓
Build Local Indexes
        ↓
Verify Integrity
        ↓
Mark Installation Ready
```

The installation MUST NOT become available for student operation until required baseline data is present and verified.

---

# 52. Media Synchronization

Large files such as:

* images;
* diagrams;
* PDFs;
* processed content assets;

SHOULD be synchronized separately from ordinary JSON entity records.

Metadata can identify the required asset:

```text
asset_id
checksum
size
mime_type
version
```

The local server downloads the asset only when required.

Checksums MUST be verified after transfer.

---

# 53. Synchronization of AI/RAG Data

AI indexes are derived data.

Approved content and its authoritative versions synchronize first.

The local server then performs or receives the appropriate indexing process:

```text
Approved Content
      ↓
Local Content Store
      ↓
Chunking
      ↓
Embedding
      ↓
Vector Index
```

The vector index SHOULD NOT be treated as the authoritative source.

If an index becomes corrupted:

```text
Authoritative Content
      ↓
Rebuild Index
```

This avoids synchronizing large, fragile vector databases unnecessarily.

---

# 54. Synchronization and AI Conversations

AI conversation history is not a primary academic authority.

Where conversation history is synchronized, it MUST be:

* scoped to the appropriate user;
* access-controlled;
* minimized;
* stored efficiently;
* excluded from business-critical synchronization paths.

Failure to synchronize AI conversation history MUST NOT prevent academic synchronization.

---

# 55. Synchronization and Analytics

Analytics SHOULD be generated from synchronized authoritative events.

Example:

```text
Assessment Results
       +
Practice Events
       +
Academic Structure
       ↓
Analytics Processing
       ↓
Dashboard Metrics
```

A dashboard calculation failure MUST NOT block core academic synchronization.

---

# 56. Security Failure Behavior

If synchronization security checks fail:

```text
Security Failure
      ↓
Reject Operation
      ↓
Record Audit Event
      ↓
Do Not Apply Mutation
      ↓
Alert / Retry if appropriate
```

The system MUST fail closed for unauthorized operations.

It MUST NOT bypass authorization simply because the local server is offline or trusted by network location.

---

# 57. Synchronization Failure Isolation

Synchronization failure MUST NOT stop core local academic operation.

If internet connectivity is lost:

```text
Local CBT       → continues
Local Practice  → continues
Local Login     → continues
Local Scoring   → continues
Synchronization → pauses
```

When connectivity returns, synchronization resumes from durable state.

---

# 58. Data Convergence

The system aims for controlled eventual consistency between online and local systems.

Convergence means:

* all valid operations are eventually delivered;
* duplicates are eliminated;
* conflicts are resolved according to entity policy;
* derived data is recalculated;
* historical data remains intact.

The system MUST NOT equate convergence with indiscriminate overwrite.

---

# 59. Testing Strategy

Synchronization testing MUST cover unit, integration, end-to-end, failure, security, and recovery scenarios.

## Unit Tests

Test:

* revision comparisons;
* idempotency;
* conflict detection;
* conflict policies;
* dependency ordering;
* retry calculation;
* checksum validation;
* cursor advancement;
* tombstones;
* serialization/deserialization.

## Integration Tests

Test:

* local database → outbox;
* outbox → online API;
* online journal → local inbox;
* transaction boundaries;
* acknowledgements;
* retry behavior;
* conflict records;
* schema compatibility.

## End-to-End Tests

Test:

```text
Local Change
    ↓
Sync
    ↓
Online
    ↓
Remote Change
    ↓
Sync
    ↓
Local
```

## Failure Tests

Test:

* network loss;
* DNS failure;
* TLS failure;
* server unavailable;
* timeout;
* duplicate request;
* response loss;
* process crash;
* database restart;
* Windows restart;
* power failure;
* partial batch;
* malformed payload;
* invalid credential;
* revoked installation;
* schema mismatch;
* protocol mismatch;
* large backlog;
* clock skew;
* simultaneous edits.

---

# 60. Required Synchronization Failure Matrix

| Failure                    | Expected Behavior                      |
| -------------------------- | -------------------------------------- |
| Internet unavailable       | Local operation continues; sync queues |
| Online server unavailable  | Retry with backoff                     |
| Request timeout            | Retry same operation ID                |
| Response lost after commit | Retry receives `ALREADY_APPLIED`       |
| Duplicate operation        | Idempotent no-op                       |
| Conflict                   | Create conflict record                 |
| Invalid payload            | Reject and audit                       |
| Dependency missing         | Defer operation                        |
| Schema incompatible        | Block unsafe synchronization           |
| Credential revoked         | Stop sync and alert                    |
| Integrity failure          | Reject payload                         |
| Local crash                | Resume from durable state              |
| Power loss                 | Recover from SQLite WAL/outbox         |
| Partial batch failure      | Retry only failed operations           |
| Corrupted derived index    | Rebuild from authoritative data        |
| Large backlog              | Backpressure and controlled batching   |

---

# 61. Performance Requirements

Synchronization SHOULD be optimized for:

* low bandwidth;
* intermittent connectivity;
* school internet instability;
* large question/content libraries;
* potentially large assessment-result backlogs.

The system SHOULD use:

* incremental synchronization;
* compression where appropriate;
* batching;
* cursor-based pull;
* checksums;
* delta metadata;
* separate media transfer;
* controlled concurrency.

The system MUST avoid repeatedly transmitting unchanged records.

---

# 62. Storage Requirements

Synchronization metadata MUST be storage-efficient.

The system SHOULD avoid storing complete duplicated payloads indefinitely.

Instead, it SHOULD retain:

* identifiers;
* operation metadata;
* revisions;
* compact error information;
* required audit information;
* payload references where practical.

Completed synchronization records SHOULD follow defined retention policies.

---

# 63. Monitoring and Health Checks

The synchronization engine MUST expose health information to the Local Server Manager.

Health SHOULD include:

* worker status;
* queue health;
* last successful sync;
* last failed sync;
* queue age;
* database connectivity;
* online connectivity;
* authentication state;
* protocol compatibility;
* installation registration state.

Health checks MUST distinguish between:

```text
Process Running
```

and:

```text
Synchronization Actually Healthy
```

A running worker does not necessarily mean synchronization is healthy.

---

# 64. Operational Alerts

The system SHOULD alert Super Admin when:

* a local server has been stale beyond a configured threshold;
* synchronization backlog exceeds a threshold;
* repeated operations fail;
* conflicts remain unresolved;
* credentials are invalid or revoked;
* protocol compatibility fails;
* database integrity checks fail;
* synchronization repeatedly crashes.

Alerts MUST be actionable and contain enough diagnostic context to investigate the problem.

---

# 65. Definition of Done

Synchronization architecture is complete only when:

* online/local topology is implemented;
* installation identity exists;
* synchronization authentication exists;
* data authority rules are encoded;
* revisions exist;
* durable local outbox exists;
* online change journal exists;
* push/pull protocol exists;
* acknowledgements exist;
* idempotency exists;
* dependency ordering exists;
* retry/backoff exists;
* conflict detection exists;
* entity-specific conflict policies exist;
* tombstones exist where required;
* cursor-based synchronization exists;
* schema compatibility is enforced;
* integrity verification exists;
* synchronization audit logging exists;
* Super Admin synchronization dashboard exists;
* local synchronization dashboard exists;
* manual retry is controlled and audited;
* disaster recovery behavior is tested;
* large backlog behavior is tested;
* security testing is complete;
* failure recovery tests pass;
* documentation and API contracts are synchronized with implementation.

---

# 66. Non-Negotiable Rules

The following rules MUST NOT be violated:

1. Do not copy databases between online and local systems.
2. Do not use blind last-write-wins for academic records.
3. Do not synchronize plaintext passwords.
4. Every synchronization operation must have a unique operation ID.
5. Synchronization processing must be idempotent.
6. Mutable synchronized records must use revision/concurrency protection.
7. Active CBT attempts are local-server authoritative.
8. Completed academic results must remain historically traceable.
9. Published question versions must not be silently overwritten.
10. Derived analytics must not become an authority for source data.
11. Outbox records must be durable.
12. Cursor advancement must occur only after successful application.
13. Partial batch failure must be handled per operation.
14. Synchronization failure must not stop offline academic operation.
15. Security failures must fail closed.
16. Manual conflict resolution must be audited.
17. Synchronization must be resumable after crashes and power loss.
18. Unsupported schema/protocol combinations must not perform unsafe mutations.
19. There must be no generic "force overwrite everything" mechanism.
20. Every critical synchronization action must be observable and auditable.

---

# 67. Final Synchronization Principle

NAKAMA synchronization is not a database-copy mechanism.

It is a controlled distributed-data system in which:

```text
Online Master
      +
School Local Server
      +
Explicit Data Authority
      +
Durable Change Tracking
      +
At-Least-Once Delivery
      +
Idempotent Processing
      +
Revision Control
      +
Entity-Specific Conflict Resolution
      +
Auditability
      +
Failure Recovery
      ↓
Reliable System Convergence
```

The most important rule is:

> **Never resolve distributed-system uncertainty by silently overwriting academic truth.**

The synchronization layer must preserve data integrity, maintain historical correctness, survive unreliable connectivity, and allow the school to continue operating normally while disconnected from the internet.
