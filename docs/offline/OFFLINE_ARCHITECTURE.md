# Offline Architecture

## 1. Purpose

This document defines the architecture for CRC's offline school operation.

The offline system allows the school to continue performing critical academic activities when internet connectivity is unavailable.

The primary objective is:

> Loss of internet connectivity must not stop critical school-local academic operations.

The offline architecture covers:

* school-local server;
* Windows deployment;
* LAN access;
* local authentication;
* local database;
* student learning;
* practice mode;
* CBT/examinations;
* teacher academic workflows;
* local synchronization;
* local AI;
* server health;
* watchdogs;
* failure recovery;
* backup and restore;
* reconnection;
* synchronization after connectivity returns.

---

# 2. Offline Operating Principle

CRC uses a hybrid architecture:

```text
                         INTERNET
                            │
                            ▼
                  ┌──────────────────┐
                  │   ONLINE MASTER  │
                  │   CLOUD SYSTEM   │
                  └────────┬─────────┘
                           │
                     Synchronization
                           │
                           ▼
              ┌────────────────────────┐
              │   SCHOOL LOCAL SERVER  │
              │                        │
              │  Local API             │
              │  SQLite / WAL          │
              │  Sync Engine           │
              │  Local AI              │
              │  Watchdog              │
              │  Monitoring            │
              └───────────┬────────────┘
                          │
                       SCHOOL LAN
          ┌───────────────┼────────────────┐
          ▼               ▼                ▼
     Student PCs      Teacher PCs      Admin PCs
```

The local server is the operational authority for supported offline workflows.

The online system remains the master system for globally synchronized data.

---

# 3. Offline Scope

Offline operation MUST support the critical workflows required for school continuity.

At minimum:

* student login;
* teacher login where local teacher workflows are supported;
* authorized local administration;
* student dashboard;
* learning content access;
* practice mode;
* CBT/examination;
* answer saving;
* scoring;
* progress recording;
* assessment recovery;
* local academic data access;
* local AI tutoring where configured;
* local server monitoring;
* synchronization preparation.

The following are primarily online:

* cloud dashboards;
* parent dashboard;
* content processing;
* online master administration;
* cloud analytics;
* external services.

---

# 4. Offline Guarantee

The system MUST NOT depend on internet access for:

* local authentication;
* starting approved CBT attempts;
* answering questions;
* saving answers;
* submitting attempts;
* scoring completed assessments;
* recording local progress;
* recovering interrupted CBT sessions.

If an internet outage occurs during an active CBT:

```text id="u4j7hc"
Internet Failure
      │
      ▼
Local Server Continues
      │
      ▼
CBT Continues
      │
      ▼
Answers Saved Locally
      │
      ▼
Attempt Completed
      │
      ▼
Results Stored Locally
      │
      ▼
Sync Later
```

---

# 5. Local Server

The school-local server is a Windows PC running the CRC local server application.

The application is distributed as a Windows executable:

```text id="1w1m75"
LMSServer.exe
```

The executable acts as a local server manager/orchestrator rather than a single monolithic application process.

---

# 6. Local Server Components

Conceptually:

```text id="xk0bqa"
LMSServer.exe
    │
    ├── Local Server Manager
    ├── LMS Backend/API
    ├── SQLite Database
    ├── Sync Worker
    ├── Local AI Gateway
    ├── AI/RAG Services
    ├── Watchdog
    ├── Background Jobs
    ├── Logging
    ├── Monitoring
    └── Backup Manager
```

Components MAY run as separate processes or services.

The packaging and installation experience should remain unified for the school administrator.

---

# 7. Windows Deployment

The local server MUST support deployment on a supported Windows version.

Installation SHOULD:

1. install required application components;
2. create required directories;
3. initialize configuration;
4. initialize the local database;
5. configure required Windows services/processes;
6. configure firewall rules;
7. generate or provision local server identity;
8. establish secure online synchronization credentials;
9. perform health checks;
10. provide administrator diagnostics.

The installer MUST NOT silently expose the server to the public internet.

---

# 8. Local Server Manager

The server manager is the primary operational interface for the school server.

It SHOULD provide:

* Start Server;
* Stop Server;
* Restart Server;
* server status;
* LAN address;
* configured port;
* hostname;
* connected devices;
* database health;
* sync status;
* AI status;
* service status;
* logs;
* diagnostics;
* backups;
* configuration;
* endpoint diagnostics;
* update status.

---

# 9. Server Lifecycle

The local server has an explicit lifecycle:

```text id="wzv0ig"
Installed
   ↓
Configured
   ↓
Initialized
   ↓
Healthy
   ↓
Running
   ↓
Stopping
   ↓
Stopped
```

Failure states include:

```text id="kq3r2d"
DEGRADED
FAILED
RECOVERING
```

The manager MUST expose the current state.

---

# 10. Startup Sequence

A normal startup SHOULD follow:

```text id="5r0xpr"
Windows Starts
      │
      ▼
Local Server Manager
      │
      ▼
Configuration Validation
      │
      ▼
Database Integrity Check
      │
      ▼
Local Services Start
      │
      ▼
Health Checks
      │
      ▼
LAN Endpoint Available
      │
      ▼
Ready
```

The server MUST NOT expose the application as healthy before required dependencies are ready.

---

# 11. Shutdown Sequence

Normal shutdown SHOULD:

1. stop accepting new work;
2. allow short-lived operations to complete where safe;
3. flush pending database writes;
4. persist sync state;
5. persist monitoring state;
6. stop workers;
7. close database connections;
8. stop services;
9. record shutdown event.

The shutdown process MUST avoid corrupting SQLite state.

---

# 12. Unexpected Server Shutdown

The system MUST tolerate:

* power loss;
* Windows restart;
* process crash;
* application crash;
* forced shutdown.

SQLite MUST use WAL mode.

On restart:

```text id="y00d3y"
Server Restart
      │
      ▼
SQLite Recovery
      │
      ▼
Integrity Check
      │
      ▼
Pending Jobs Recovered
      │
      ▼
Active/Interrupted State Evaluated
      │
      ▼
Services Restarted
```

The system MUST NOT silently discard committed data.

---

# 13. Local Database

The local operational database is:

```text id="m7j1c4"
SQLite + WAL
```

It is the primary local persistence layer.

The local database MUST support:

* academic data;
* users required for offline authentication;
* question data;
* assessment configuration;
* active attempts;
* answers;
* results;
* progress;
* sync state;
* audit data;
* local AI metadata where applicable;
* system configuration metadata.

---

# 14. SQLite WAL

Write-Ahead Logging is required for local operation.

WAL improves:

* concurrent reads;
* write reliability;
* recovery;
* operational performance.

The implementation MUST configure SQLite appropriately for the expected school workload.

The database MUST be regularly checked for integrity.

---

# 15. Local Data Authority

Not all data has the same authority.

The local architecture uses explicit authority categories.

### Local-authoritative data

Examples:

* active CBT attempts;
* local answer events;
* local assessment completion state;
* local practice events generated while offline.

### Online-authoritative data

Examples:

* global account lifecycle;
* global configuration;
* published online content;
* centralized administrative state.

### Shared synchronized data

Examples:

* student progress;
* assessment results;
* audit-related synchronization events;
* permitted teacher activity.

### Derived data

Examples:

* analytics;
* dashboards;
* learning-gap summaries;
* aggregated statistics.

The exact entity authority rules are defined by the synchronization architecture.

---

# 16. Local Identity Replica

The local server maintains the identity information required for offline authentication.

This includes, as appropriate:

* user ID;
* login identifier;
* role;
* account state;
* credential verifier;
* credential revision;
* required profile information;
* synchronization revision.

Plaintext passwords MUST never be stored.

---

# 17. Offline Login

The local login flow is:

```text id="4jkv7k"
Student Device
      │
      ▼
Local Login Endpoint
      │
      ▼
Local Identity Lookup
      │
      ▼
Credential Verification
      │
      ▼
Account State Check
      │
      ▼
Role Authorization
      │
      ▼
Local Session
```

The local server MUST NOT redirect the user to the online system merely because the internet is unavailable.

---

# 18. Offline Session

Local sessions are managed by the local server.

The browser/client MUST NOT be the sole authority for authentication state.

The local server controls:

* session validity;
* expiry;
* revocation;
* role;
* account state;
* authorization.

---

# 19. Offline Account Revocation

The local server uses its latest synchronized identity state.

If internet access is unavailable, online account changes cannot immediately reach the local server.

Therefore:

```text id="x6n31v"
Online Disable
      │
      │ Internet unavailable
      ▼
Local Account Still Uses Last Valid State
      │
      │ Sync restored
      ▼
Local Disable Applied
```

Super Admin MUST have a local emergency mechanism to disable accounts when necessary.

---

# 20. LAN Access

The local server is accessible only through the school LAN.

A private hostname SHOULD be used.

Example:

```text id="u3z0d8"
http(s)://lms.local
```

The actual hostname is deployment configuration.

The server MUST NOT require public DNS.

---

# 21. Network Binding

The local server SHOULD bind only to the intended school LAN interface.

It MUST NOT unnecessarily bind to:

```text id="gl7q4y"
0.0.0.0
```

if doing so exposes services to unintended interfaces.

Administrative endpoints SHOULD be more restricted than ordinary student endpoints where practical.

---

# 22. Windows Firewall

The installer SHOULD create the minimum required Windows Firewall rules.

Rules MUST:

* allow required LAN traffic;
* block unnecessary inbound traffic;
* avoid public exposure;
* restrict administrative services where possible.

No port forwarding should be required.

---

# 23. Local TLS

HTTPS/TLS SHOULD be used for LAN application traffic where practical.

The deployment architecture should support a locally trusted certificate strategy appropriate for school-managed devices.

If HTTP is temporarily used in a controlled development environment, production deployment MUST have an explicit security decision and documented compensating controls.

---

# 24. LAN Device Trust

LAN access is not authentication.

Any device connected to the school network must still authenticate.

The system MUST assume that:

* an unauthorized device may join the LAN;
* a student may attempt to access administrative endpoints;
* a compromised workstation may exist;
* network traffic may be inspected or manipulated.

Server-side authentication and authorization therefore remain mandatory.

---

# 25. Local Content

Only approved content required for local operation should be synchronized.

The local server SHOULD maintain:

* approved questions;
* assessment configurations;
* relevant academic materials;
* necessary subject/topic metadata;
* student academic context;
* teacher assignments.

Unnecessary processing artifacts should remain online.

---

# 26. Local Student Dashboard

The student dashboard MUST operate without internet connectivity for supported local functionality.

It MAY display:

* assigned learning content;
* practice;
* practice history;
* available assessments;
* assessment history;
* progress;
* weak topics based on locally available data.

Data not yet synchronized to the online system remains local until synchronization succeeds.

---

# 27. Local Teacher Dashboard

Teacher workflows supported offline SHOULD include:

* assigned classes;
* assigned subjects;
* available question content;
* practice/assessment monitoring where supported;
* local class performance;
* local assessment results;
* permitted academic operations.

Teacher access MUST remain assignment-scoped.

---

# 28. Parent Offline Scope

Parent dashboards are primarily online.

The local server does not require parents to operate locally unless a future requirement explicitly introduces a local parent workflow.

Parent data remains synchronized to the online master system.

---

# 29. Principal Offline Scope

The Principal may have selected local management capabilities where required by school operations.

These capabilities MUST be explicitly defined rather than assuming the full online Principal dashboard is replicated locally.

Local Principal access remains subject to authorization.

---

# 30. Practice Mode

Practice mode MUST work fully offline when its required content is available locally.

The practice engine uses the same fundamental assessment infrastructure as CBT.

It may support:

* question navigation;
* answer changes;
* answer saving;
* randomization;
* scoring;
* progress recording.

Practice data is stored locally and synchronized later.

---

# 31. CBT Offline Operation

CBT is a primary offline workflow.

The local server is authoritative for the active assessment attempt.

```text id="cq6p8f"
Student
  │
  ▼
Local Authentication
  │
  ▼
Assessment Authorization
  │
  ▼
Attempt Created
  │
  ▼
Server-Authoritative Timer
  │
  ▼
Answers Persisted Locally
  │
  ▼
Submission
  │
  ▼
Deterministic Scoring
  │
  ▼
Local Result
```

No internet connection is required during this process.

---

# 32. CBT Content Availability

A CBT can only start offline if the required assessment configuration and question versions have already been synchronized to the local server.

The system MUST NOT begin an assessment with incomplete required content.

Before the assessment window:

```text id="4nj4hb"
Assessment Configuration
+
Question Set
+
Question Versions
+
Scoring Rules
+
Eligibility
        │
        ▼
Local Readiness Check
        │
        ▼
Assessment Available
```

---

# 33. Local Assessment Readiness

The local server SHOULD provide an assessment readiness check.

It should verify:

* assessment exists;
* assessment is published;
* required questions exist;
* question versions exist;
* scoring configuration exists;
* student eligibility is known;
* local database is healthy;
* sufficient storage exists.

---

# 34. Server-Authoritative Timer

The local server is authoritative for CBT time.

The client MUST NOT be trusted to determine:

* start time;
* elapsed time;
* remaining time;
* submission deadline.

The local server records the attempt's timing state.

---

# 35. Answer Persistence

Answers MUST be persisted locally as the student works.

A browser-only answer cache is insufficient.

The local server should record:

* attempt ID;
* question ID/version;
* selected option;
* sequence/event metadata;
* timestamp;
* revision.

The latest valid server-side answer state is authoritative.

---

# 36. CBT Watchdog

The local server MUST monitor active attempts.

The watchdog SHOULD detect:

* missed client heartbeats;
* device disconnection;
* stale sessions;
* abnormal attempt state;
* server-side inconsistencies.

A lost heartbeat MUST NOT automatically submit the assessment.

---

# 37. Client Crash Recovery

If the student browser crashes:

```text id="f5c8b6"
Browser Crash
    │
    ▼
Attempt Remains on Local Server
    │
    ▼
Student Reopens Application
    │
    ▼
Authenticates
    │
    ▼
Existing Attempt Detected
    │
    ▼
Attempt Revalidated
    │
    ▼
Resume
```

The student MUST NOT receive a new attempt merely because the browser restarted unless assessment configuration explicitly allows another attempt.

---

# 38. Network Drop Recovery

If the student's LAN connection drops temporarily:

```text id="k9a7xd"
LAN Disconnect
      │
      ▼
Server Retains Attempt
      │
      ▼
Client Reconnects
      │
      ▼
Session Revalidated
      │
      ▼
Attempt State Reconciled
      │
      ▼
Resume
```

Answers already committed to the local server MUST remain safe.

The client may retain a short-lived local recovery buffer for unsent data.

---

# 39. Recovery Reconciliation

On reconnect, the client and server reconcile state.

The server remains authoritative.

Conceptually:

```text id="4af2lq"
Client Recovery Buffer
        +
Server Attempt State
        │
        ▼
Reconciliation
        │
        ▼
Authoritative Attempt State
```

If the same question has conflicting unsent/client and server values, the reconciliation rule must be deterministic and documented.

---

# 40. Active Attempt State

The local server MUST persist enough state to recover an active attempt.

At minimum:

* attempt ID;
* student ID;
* assessment ID;
* question sequence;
* question versions;
* selected answers;
* timing state;
* status;
* last activity;
* submission state.

---

# 41. Assessment Submission

Submission is performed against the local server.

The server MUST:

1. authenticate the student/session;
2. validate attempt ownership;
3. validate attempt state;
4. finalize answers;
5. finalize timing;
6. calculate score;
7. persist result;
8. mark attempt completed;
9. generate audit/event records;
10. enqueue synchronization.

The result MUST be durable before the client receives a successful completion response.

---

# 42. Deterministic Scoring

Scoring is local and deterministic.

The local server MUST NOT require AI to calculate:

* score;
* correctness;
* marks;
* completion status.

Scoring rules must come from the assessment configuration and question data.

---

# 43. Randomization

Question and option randomization MUST be reproducible.

The assessment attempt SHOULD persist the generated question order and option order.

This prevents a reconnect from generating a different assessment.

---

# 44. Offline AI

Local AI is optional but supported where configured.

The local AI stack uses Ollama and associated retrieval infrastructure.

It may provide:

* tutoring;
* explanations;
* study assistance;
* learning-gap interpretation;
* teacher academic intelligence.

It MUST NOT control:

* authentication;
* authorization;
* scoring;
* grades;
* assessment timing;
* assessment correctness;
* account management;
* synchronization.

---

# 45. AI During CBT

AI MUST be disabled during CBT/exam mode.

The local AI gateway MUST enforce this server-side.

The client UI alone MUST NOT be trusted to disable AI.

If a student attempts an AI request during an active restricted assessment:

```text id="7o0r3w"
AI Request
    │
    ▼
Local AI Gateway
    │
    ▼
Assessment State Check
    │
    ▼
CBT Active
    │
    ▼
Request Rejected
```

---

# 46. Local RAG

Local AI should use approved school content where appropriate.

The local retrieval pipeline is:

```text id="j8xwqz"
Approved Content
      │
      ▼
Local Index
      │
      ▼
Relevant Context
      │
      ▼
Ollama
      │
      ▼
Student/Teacher Response
```

The local server should not require the internet for configured local retrieval.

---

# 47. Local AI Storage

AI conversations should be stored efficiently.

The architecture SHOULD avoid repeatedly storing large copies of retrieved context.

Instead, conversations should reference:

* message content;
* content IDs;
* retrieval references;
* model ID;
* prompt/version metadata;
* timestamps.

This reduces storage consumption and improves retrieval performance.

---

# 48. Local Storage Management

The local server MUST monitor:

* database size;
* file storage;
* AI storage;
* logs;
* backups;
* available disk space.

Low storage MUST trigger warnings.

Critical storage conditions MUST prevent unsafe operations where necessary.

---

# 49. Local Cache

Caches MUST be treated as disposable.

The system MUST distinguish:

```text id="v6jjqj"
Authoritative Data
      ≠
Cached Data
```

Deleting a cache MUST NOT destroy academic records.

---

# 50. Local Audit Trail

Security and important academic actions occurring offline MUST be recorded locally.

Examples:

* login;
* failed login;
* logout;
* account state changes;
* assessment start;
* answer events where required;
* assessment submission;
* scoring;
* local administrative actions;
* synchronization operations.

These records SHOULD synchronize to the online master system.

---

# 51. Offline Audit Integrity

Audit events SHOULD include:

* event ID;
* user ID;
* actor;
* timestamp;
* local server ID;
* event type;
* operation ID;
* correlation ID;
* revision.

Events MUST be designed to prevent duplicate synchronization.

---

# 52. Sync Queue

Offline changes are stored in a durable synchronization queue.

```text id="ph0t3n"
Local Operation
      │
      ▼
Durable Sync Queue
      │
      ├── Pending
      ├── Processing
      ├── Succeeded
      ├── Retry
      └── Conflict
```

The queue MUST survive:

* application restart;
* Windows restart;
* power loss;
* temporary internet outage.

---

# 53. Synchronization Trigger

Synchronization SHOULD occur:

* when internet connectivity is detected;
* at configured intervals;
* after important local operations where appropriate;
* when manually initiated by authorized administrators.

Synchronization MUST NOT block normal offline academic workflows.

---

# 54. Connectivity Detection

The local server SHOULD distinguish between:

```text id="3v7xg6"
LAN Available
Internet Available
Online Master Reachable
Synchronization Healthy
```

These are different states.

A working LAN does not imply internet connectivity.

Internet connectivity does not imply that the online master is healthy.

---

# 55. Sync Health States

The local server SHOULD expose:

```text id="v4hjcv"
ONLINE
DEGRADED
OFFLINE
SYNCING
SYNC_ERROR
CONFLICT
```

The Super Admin dashboard should clearly communicate the current state.

---

# 56. Reconnection

When internet connectivity returns:

```text id="q1n0je"
Internet Returns
      │
      ▼
Online Master Reachability Check
      │
      ▼
Secure Sync Handshake
      │
      ▼
Upload Local Changes
      │
      ▼
Download Remote Changes
      │
      ▼
Conflict Resolution
      │
      ▼
Acknowledgement
      │
      ▼
Local State Updated
```

Reconnection MUST NOT require manual database copying.

---

# 57. Sync Ordering

Synchronization MUST preserve required dependency order.

For example:

```text id="q8k5kz"
Identity
   ↓
Academic Configuration
   ↓
Content
   ↓
Assessment Configuration
   ↓
Assessment Activity
   ↓
Results / Progress
```

Actual ordering is entity-specific and must be defined by the synchronization architecture.

---

# 58. Conflict Handling

The local system MUST NOT resolve every conflict using:

```text id="6d4q55"
last write wins
```

Conflict strategy is entity-specific.

Examples:

* immutable assessment events → append/idempotent reconciliation;
* account state → authoritative administrative source;
* active CBT attempt → local server authority;
* published content → versioned authoritative content;
* derived analytics → recompute;
* editable configuration → explicit revision conflict handling.

Conflicts MUST be visible to Super Admin where human intervention is required.

---

# 59. Sync Dashboard

The local server manager MUST expose synchronization status.

At minimum:

* current status;
* last successful sync;
* last failed sync;
* pending changes;
* failed operations;
* retry count;
* conflicts;
* error details;
* local server health;
* online server connectivity.

---

# 60. Watchdog Architecture

The local server SHOULD have a watchdog independent enough to detect failures in critical services.

```text id="11v4nj"
                Watchdog
                   │
       ┌───────────┼────────────┐
       ▼           ▼            ▼
     API        Database       Sync
       │           │            │
       ▼           ▼            ▼
      Health     Health        Health
```

The watchdog may restart failed non-critical services automatically.

It MUST avoid restart loops.

---

# 61. Watchdog Escalation

A recommended escalation pattern:

```text id="2v8z5e"
Failure Detected
      │
      ▼
Health Check
      │
      ▼
Automatic Recovery
      │
      ▼
Retry
      │
      ▼
Persistent Failure
      │
      ▼
Critical Alert
```

Persistent failures MUST be visible to the administrator.

---

# 62. Server Health Checks

Health checks SHOULD cover:

* API;
* database;
* filesystem;
* disk space;
* sync worker;
* AI service;
* queue;
* backup subsystem;
* network binding.

Health checks MUST distinguish between:

```text id="5db7cf"
Healthy
Degraded
Failed
```

---

# 63. Local Server Logs

Logs MUST be structured and filterable.

Recommended categories:

* application;
* authentication;
* database;
* assessment;
* synchronization;
* AI;
* backup;
* watchdog;
* network;
* security.

Administrators SHOULD be able to filter by:

* severity;
* date/time;
* category;
* service;
* correlation ID.

---

# 64. Endpoint Diagnostics

The server manager SHOULD provide an authenticated endpoint diagnostics view.

It may expose:

* endpoint name;
* method;
* service;
* health status;
* latency;
* last failure.

It MUST NOT expose:

* secrets;
* passwords;
* session tokens;
* credential verifier data.

Dangerous administrative endpoints MUST NOT be callable merely because they appear in a diagnostic interface.

---

# 65. Backup Architecture

The local server MUST support backups.

Backups SHOULD include:

* SQLite database;
* important local configuration;
* sync metadata;
* required local content metadata;
* relevant audit records.

Large cache files SHOULD not be included unless required.

---

# 66. Backup Scheduling

The server manager SHOULD support configurable backup scheduling.

At minimum:

* manual backup;
* scheduled backup;
* backup destination;
* retention configuration;
* backup status;
* restore verification.

The exact schedule is deployment configuration.

---

# 67. Backup Safety

Backups MUST:

* be protected from unauthorized access;
* use integrity verification;
* be distinguishable by version/time;
* avoid overwriting the only known-good backup;
* be tested for restoration.

Encrypted backups SHOULD be used for sensitive production data.

---

# 68. Restore Architecture

Restore MUST be treated as a controlled administrative operation.

A restore process SHOULD:

1. stop affected services;
2. validate backup;
3. create a safety snapshot if possible;
4. restore database;
5. validate integrity;
6. reconcile server identity;
7. validate sync state;
8. restart services;
9. perform health checks.

Restoring an old database MUST NOT accidentally duplicate or replay historical synchronization operations.

---

# 69. Local Server Identity

Every school installation MUST have a unique local server identity.

Example:

```text id="f0n1j8"
school_installation_id
local_server_id
```

This identity is used by:

* synchronization;
* audit events;
* conflict handling;
* diagnostics;
* device registration.

It MUST NOT be reused across independent installations.

---

# 70. Server Registration

During initial setup, the local server SHOULD be securely registered with the online master.

Registration establishes:

* school installation identity;
* local server identity;
* synchronization credentials;
* supported capabilities;
* deployment metadata.

Registration MUST use authenticated secure communication.

---

# 71. Configuration

The local server configuration SHOULD include:

```text id="7j5v3z"
Server
 ├── Port
 ├── LAN Interface
 ├── Hostname
 └── Startup Behavior

Database
 ├── Database Path
 ├── Backup Path
 └── Retention

Online
 ├── Master URL
 └── Sync Settings

AI
 ├── Ollama Endpoint
 ├── Model
 └── Retrieval Settings

Logging
 ├── Level
 ├── Retention
 └── Storage

Security
 ├── Session Policy
 ├── Rate Limits
 └── Certificate Configuration
```

Secrets MUST never be displayed in plaintext.

---

# 72. Configuration Validation

Invalid configuration MUST be rejected before service startup.

Examples:

* invalid port;
* inaccessible database path;
* invalid backup path;
* invalid online endpoint;
* missing synchronization credentials;
* invalid certificate;
* unsupported AI configuration.

The server manager SHOULD provide actionable validation errors.

---

# 73. Local Server Updates

Updates SHOULD be delivered through a controlled deployment process.

An update MUST account for:

* application binaries;
* database migrations;
* configuration compatibility;
* local content compatibility;
* synchronization compatibility.

Updates MUST NOT silently destroy local data.

---

# 74. Database Migrations

Local schema migrations MUST be versioned.

Migration process:

```text id="w9b8x2"
Current Schema
      │
      ▼
Migration Check
      │
      ▼
Backup / Safety Check
      │
      ▼
Migration
      │
      ▼
Integrity Validation
      │
      ▼
Application Startup
```

Failed migrations MUST prevent unsafe startup rather than leaving the server in an unknown state.

---

# 75. Offline Failure Domains

The architecture distinguishes:

### Client failure

Examples:

* browser crash;
* device shutdown;
* temporary LAN disconnect.

Expected response:

* recover active session/attempt from server.

### Local server failure

Examples:

* process crash;
* Windows restart;
* power failure.

Expected response:

* restart;
* SQLite recovery;
* resume durable state.

### Internet failure

Expected response:

* continue offline;
* queue synchronization;
* synchronize later.

### Online master failure

Expected response:

* continue local operation;
* retry synchronization later.

### Database failure

Expected response:

* stop unsafe operations;
* report failure;
* recover/restore according to procedure.

---

# 76. Critical Failure Principle

The system MUST distinguish between:

```text id="6njw9c"
Cannot synchronize
```

and:

```text id="v8zv5j"
Cannot operate locally
```

A synchronization failure MUST NOT automatically disable local academic operation.

Local operation should continue unless the failure directly compromises data integrity or security.

---

# 77. Degraded Mode

The local server SHOULD support degraded operation.

Examples:

```text id="q6xq4p"
Internet unavailable
→ Offline academic operation

AI unavailable
→ Academic system continues without AI

Sync unavailable
→ Local changes queue

Analytics unavailable
→ Core academic workflows continue

Backup unavailable
→ Warning + operational policy
```

Optional services MUST NOT unnecessarily become single points of failure for critical academic workflows.

---

# 78. AI Failure Isolation

If Ollama fails:

* student learning must continue;
* practice must continue;
* CBT must continue;
* scoring must continue;
* synchronization must continue.

The AI service is not a dependency of the deterministic academic core.

---

# 79. Synchronization Failure Isolation

If synchronization fails:

* student login remains available;
* practice remains available;
* CBT remains available;
* local results remain stored;
* the sync queue retains pending changes.

The system MUST clearly communicate synchronization degradation to authorized administrators.

---

# 80. Storage Failure Protection

If available disk space falls below a critical threshold:

* warnings MUST be displayed;
* non-essential jobs MAY be paused;
* AI indexing MAY be paused;
* log retention MAY be enforced;
* unnecessary cache data MAY be cleaned.

The system MUST avoid deleting authoritative academic data automatically merely to reclaim space.

---

# 81. Offline Security

Offline security controls remain active.

The system MUST still enforce:

* authentication;
* authorization;
* account state;
* session security;
* audit logging;
* rate limiting;
* file access controls;
* assessment restrictions.

Offline mode is not a security bypass.

---

# 82. Physical Security

The school local server is a high-value asset.

Deployment SHOULD include:

* controlled physical location;
* restricted physical access;
* reliable power;
* surge protection;
* UPS where appropriate;
* controlled administrator access;
* regular backup verification.

Physical server compromise must be included in the threat model.

---

# 83. Power Failure

The system SHOULD be designed for abrupt power loss.

Recommended operational protections include:

* UPS;
* WAL-enabled SQLite;
* periodic backups;
* safe shutdown;
* automatic service restart;
* integrity checks.

After power restoration, the server SHOULD automatically recover where configured.

---

# 84. LAN Failure

If the school LAN itself fails:

* clients cannot reach the local server;
* the local server should retain all durable state;
* active attempts remain stored;
* clients reconnect once LAN connectivity returns.

The system cannot provide LAN-based operation when the underlying LAN is physically unavailable.

---

# 85. Client Offline Buffer

Clients MAY maintain a short-lived local recovery buffer for:

* unsent answer changes;
* current UI state;
* pending request identifiers.

This buffer is a recovery aid.

It MUST NOT replace the local server as the authoritative academic state.

---

# 86. Idempotent Local Operations

Important local operations SHOULD use idempotency keys.

Examples:

* answer save;
* attempt submission;
* progress event;
* synchronization operation.

This prevents retries from creating duplicate records.

---

# 87. Offline Event Model

Where appropriate, local activity SHOULD be represented as durable events.

Example:

```text id="q9n6d3"
Student Answered Question
        │
        ▼
Local Event
        │
        ▼
Attempt State Updated
        │
        ▼
Sync Queue
        │
        ▼
Online Master
```

Events MUST be replay-safe.

---

# 88. Local/Online Data Reconciliation

After reconnection, the synchronization engine MUST reconcile:

* identities;
* academic placements;
* content;
* assessment configurations;
* attempts;
* answers;
* results;
* progress;
* audit events.

Each entity uses its defined authority and conflict policy.

---

# 89. Result Synchronization

Completed CBT results MUST be durable locally before synchronization.

The result synchronization SHOULD include:

* attempt ID;
* student ID;
* assessment ID;
* question version references;
* answers;
* score;
* completion timestamp;
* local server ID;
* revision;
* integrity metadata.

The online master MUST be able to verify that the result is structurally valid.

---

# 90. Historical Integrity

Offline operation MUST preserve historical records.

Once an assessment attempt is completed:

* the attempt remains immutable;
* the question versions remain identifiable;
* the score remains reproducible;
* synchronization must not rewrite the historical attempt.

Corrections, if ever required, MUST use explicit correction mechanisms.

---

# 91. Local Analytics

The local server MAY calculate operational analytics required for local workflows.

Examples:

* student progress;
* practice performance;
* local class performance;
* assessment results;
* weak topics.

Derived analytics SHOULD be reproducible from authoritative local records.

The online system may recompute broader analytics after synchronization.

---

# 92. Promotion and Offline Operation

Promotion is primarily an administrative operation.

The local server may display promotion readiness information if synchronized academic data is available.

Final promotion authority remains governed by the approved promotion architecture.

A disconnected local server MUST NOT independently execute an unauthorized global promotion.

---

# 93. Offline Content Processing

Content processing does not run locally in V1.

The local server receives approved content through synchronization.

This keeps:

* OCR workloads online;
* large processing dependencies online;
* local server simpler;
* local attack surface smaller;
* school hardware requirements manageable.

---

# 94. Local AI Model Management

The local server manager SHOULD expose:

* Ollama availability;
* configured model;
* model health;
* storage usage;
* AI service status.

Model downloads/updates SHOULD be controlled and must not block core academic operation.

---

# 95. Observability

The local server MUST expose operational observability.

At minimum:

* service health;
* database health;
* sync health;
* disk usage;
* active users;
* active assessments;
* connected clients;
* AI status;
* queue depth;
* errors.

---

# 96. Local Server Security Events

The system MUST audit important local events:

* server start;
* server stop;
* service failure;
* administrator login;
* configuration change;
* account disablement;
* backup;
* restore;
* update;
* database migration;
* synchronization;
* security failure.

---

# 97. Monitoring Thresholds

The system SHOULD define thresholds for:

* disk usage;
* database growth;
* sync backlog;
* repeated sync failure;
* service restart loops;
* excessive authentication failures;
* CPU/memory exhaustion;
* AI resource consumption.

Thresholds SHOULD produce warnings before critical failure.

---

# 98. School Operational Workflow

A normal school day may look like:

```text id="9t7jv8"
Server Starts
      │
      ▼
Health Check
      │
      ▼
Students Connect
      │
      ▼
Students Study
      │
      ▼
Practice
      │
      ▼
CBT / Examination
      │
      ▼
Results Stored Locally
      │
      ▼
Teachers Review Local Results
      │
      ▼
Internet Available
      │
      ▼
Synchronization
      │
      ▼
Online Dashboards Updated
```

The school must not need to manually copy database files between systems.

---

# 99. Internet Restoration Workflow

When internet access returns:

1. connectivity is detected;
2. online master is reached;
3. secure synchronization begins;
4. local pending changes are uploaded;
5. remote changes are downloaded;
6. conflicts are detected;
7. conflict resolution is applied;
8. successful operations are acknowledged;
9. failed operations remain queued;
10. dashboard status is updated.

---

# 100. Synchronization Retry

Transient synchronization failures SHOULD use bounded exponential backoff.

The system SHOULD avoid:

* continuous retry storms;
* duplicate operations;
* blocking the local application.

Administrators MUST be able to trigger a controlled manual retry.

---

# 101. Offline Definition of Done

The offline architecture is complete only when:

### Server

* Windows installer works;
* local server starts reliably;
* server manager works;
* health checks work;
* watchdog works.

### Network

* LAN access works;
* private hostname works;
* firewall rules work;
* unnecessary public exposure is prevented.

### Database

* SQLite WAL works;
* recovery works;
* integrity checks work;
* backups work;
* restore works.

### Authentication

* offline login works;
* account state is enforced;
* local sessions work;
* emergency local disable works.

### Academic

* student study works;
* practice works;
* CBT works;
* scoring works;
* progress is recorded.

### CBT Recovery

* browser crash recovery works;
* LAN disconnect recovery works;
* answer persistence works;
* timer remains server-authoritative;
* duplicate attempts are prevented.

### AI

* local AI works when configured;
* AI failure does not break academic workflows;
* AI is disabled during CBT.

### Synchronization

* local changes queue correctly;
* reconnection works;
* retries work;
* conflicts are handled;
* results synchronize correctly.

### Reliability

* power-loss recovery works;
* process-crash recovery works;
* database recovery works;
* service restart works.

### Security

* LAN is not treated as trusted;
* authentication remains mandatory;
* authorization remains enforced;
* secrets remain protected;
* administrative endpoints remain protected.

### Testing

* unit tests pass;
* integration tests pass;
* E2E tests pass;
* offline tests pass;
* failure/recovery tests pass;
* synchronization tests pass;
* security tests pass.

---

# 102. Non-Negotiable Rules

1. Offline academic operation is a core requirement.
2. The school local server is the authority for active offline workflows.
3. Internet failure must not stop critical local academic operations.
4. SQLite MUST use WAL mode.
5. The local server is a Windows application.
6. LAN access does not replace authentication.
7. The local server must not be publicly exposed.
8. No public port forwarding is required.
9. Offline authentication must use secure local identity data.
10. Plaintext passwords must never be stored or synchronized.
11. CBT state must be server-authoritative.
12. The browser must never be the authoritative source for CBT timing or scoring.
13. Student answers must be durably stored locally.
14. Browser crashes must not automatically destroy active attempts.
15. LAN disconnects must not automatically destroy active attempts.
16. Deterministic scoring must work without AI.
17. AI failure must not stop academic operation.
18. Content processing is online-only in V1.
19. Synchronization failures must not stop local operation.
20. Pending synchronization data must survive server restart.
21. Historical assessment records must remain reproducible.
22. Local administrative actions must be audited.
23. Backups must be tested for restoration.
24. Local server failure must fail safely rather than silently corrupt data.
25. Offline operation must never become an authorization bypass.

---

# 103. Final Offline Principle

The CRC offline architecture is designed around one central rule:

> **The school must be able to continue teaching, practicing, assessing, scoring, and recording academic activity even when the internet disappears.**

The local server therefore acts as a resilient school-local academic runtime:

```text id="9zz7kg"
                    ONLINE MASTER
                         │
                         │
                  Secure Synchronization
                         │
                         ▼
                 SCHOOL LOCAL SERVER
                         │
          ┌──────────────┼──────────────┐
          │              │              │
          ▼              ▼              ▼
       Identity       Academic        AI
       Replica         Runtime       Runtime
          │              │              │
          └──────────────┼──────────────┘
                         │
                      SCHOOL LAN
                         │
          ┌──────────────┼──────────────┐
          ▼              ▼              ▼
       Students       Teachers       Admins
```

The online system provides global coordination.

The local server provides operational continuity.

The synchronization system reconnects the two without requiring manual database replacement or compromising historical integrity.

**Internet availability is an enhancement to the school system—not a prerequisite for its critical academic operation.**
