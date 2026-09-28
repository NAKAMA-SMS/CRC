# NAKAMA Testing Strategy

## 1. Purpose

This document defines the testing strategy for the NAKAMA system.

It establishes:

* testing levels;
* testing responsibilities;
* test environments;
* test data principles;
* module testing requirements;
* integration testing;
* end-to-end testing;
* offline testing;
* synchronization testing;
* CBT recovery testing;
* security testing;
* AI testing;
* database testing;
* API testing;
* performance testing;
* regression testing;
* acceptance testing;
* release gates.

The objective is to ensure that NAKAMA is not only functionally correct, but also secure, reliable, recoverable, maintainable, and capable of operating correctly in its online and offline environments.

---

# 2. Testing Principles

Testing must follow these principles:

1. Test against documented requirements.
2. Test behavior, not implementation assumptions.
3. Test both successful and failed operations.
4. Test authorization independently from authentication.
5. Test critical workflows end-to-end.
6. Treat offline operation as a first-class test requirement.
7. Treat synchronization as a failure-prone distributed system.
8. Treat CBT integrity as a critical requirement.
9. Test recovery, not only normal operation.
10. Test security throughout development.
11. Test database migrations before deployment.
12. Run regression tests after significant changes.
13. Use deterministic test data where possible.
14. Never use real production credentials in automated tests.
15. Do not allow AI output to become an unverified source of deterministic business truth.
16. Record and investigate failures rather than weakening tests to make them pass.

---

# 3. Testing Pyramid

Testing should generally follow:

```text
                 End-to-End
                /           \
          Integration      Acceptance
          /                       \
       API / Service / Database Tests
      /                             \
             Unit Tests
```

The lower levels should provide broad, fast coverage.

Higher-level tests should validate critical user workflows and cross-system behavior.

Not every behavior requires a full end-to-end test, but every critical workflow requires appropriate end-to-end coverage.

---

# 4. Test Levels

NAKAMA uses the following test levels:

1. Static analysis and code quality checks
2. Unit testing
3. Component testing
4. API testing
5. Database testing
6. Integration testing
7. End-to-end testing
8. Offline testing
9. Synchronization testing
10. Security testing
11. Performance testing
12. Recovery testing
13. User acceptance testing
14. Production smoke testing

---

# 5. Static Analysis and Code Quality

Before functional tests run, the codebase should pass applicable automated checks.

These may include:

* formatting;
* linting;
* type checking;
* static analysis;
* dependency checks;
* build verification.

The exact tooling is determined by the implementation stack.

A failed required static check must not be ignored without an explicit reason.

---

# 6. Unit Testing

Unit tests validate isolated business logic and utilities.

Examples include:

* validation;
* password-policy logic;
* permission evaluation;
* academic hierarchy rules;
* question selection;
* randomization;
* scoring;
* timer calculations;
* promotion calculations;
* synchronization state transitions;
* conflict detection;
* analytics calculations;
* data transformations.

Unit tests should be deterministic.

External services should normally be mocked or replaced with controlled test implementations.

---

# 7. Component Testing

Component tests validate individual application components and their internal interactions.

Examples:

* authentication service;
* authorization service;
* assessment service;
* question service;
* synchronization worker;
* content-processing service;
* AI Gateway;
* analytics processor;
* Local Server Manager components.

Component tests should verify both normal and failure behavior.

---

# 8. API Testing

All externally accessible APIs must be tested.

API tests should verify:

* authentication;
* authorization;
* request validation;
* response structure;
* error handling;
* status codes;
* pagination;
* filtering;
* sorting;
* idempotency;
* concurrency behavior;
* rate limiting where applicable;
* object-level authorization.

Security-sensitive endpoints require negative authorization tests.

---

# 9. Authentication Testing

Authentication tests must cover:

* valid credentials;
* invalid credentials;
* inactive accounts;
* locked accounts;
* first-login password change;
* password reset;
* session creation;
* session expiration;
* logout;
* session revocation;
* repeated failed authentication;
* local offline authentication;
* synchronization of account state.

Passwords must never appear in test output or logs.

---

# 10. Authorization Testing

Authentication proves identity.

Authorization determines what the authenticated user may access.

Both must be tested independently.

At minimum, test every role:

```text
Super Admin / IT
Principal
Teacher
Parent
Student
```

For protected resources, tests must verify:

* allowed access;
* denied access;
* cross-user access;
* cross-role access;
* cross-class access;
* cross-student access;
* assignment restrictions.

---

# 11. Parent Access Testing

Parent relationships must be explicitly tested.

Required cases include:

* parent accessing their assigned student;
* parent accessing multiple assigned students;
* parent attempting to access an unrelated student;
* parent relationship creation;
* parent relationship removal;
* deactivated parent;
* deactivated student.

The system must not infer parent relationships from names, email addresses, or other unrelated fields.

---

# 12. Teacher Scope Testing

Teacher access must be restricted according to Super Admin assignments.

Test cases must include:

* assigned class;
* assigned subject;
* multiple assignments;
* unassigned class;
* unassigned subject;
* unauthorized student;
* assignment removal.

A teacher must not gain access merely because a record exists in the system.

---

# 13. Principal Access Testing

Principal access must be tested against the approved management scope.

Tests must verify:

* school-level academic visibility;
* class comparisons;
* learning-gap visibility;
* management analytics;
* unauthorized administrative operations.

Principal access must not automatically grant Super Admin privileges.

---

# 14. Super Admin Testing

Super Admin functionality must be tested for:

* account provisioning;
* account editing;
* account deactivation;
* password reset;
* academic configuration;
* teacher assignments;
* promotion activation;
* system configuration;
* synchronization monitoring;
* server health visibility.

Privileged operations must generate appropriate audit records.

---

# 15. Database Testing

Database tests must verify:

* schema correctness;
* migrations;
* constraints;
* foreign keys;
* uniqueness;
* indexes;
* transactions;
* rollback behavior;
* historical integrity;
* soft deletion;
* tombstones where applicable;
* concurrent operations.

Both supported database environments must be considered:

```text
Online: PostgreSQL
Local: SQLite + WAL
```

---

# 16. Database Migration Testing

Every database migration must be tested.

At minimum:

```text
Existing Database
      ↓
Backup
      ↓
Migration
      ↓
Integrity Check
      ↓
Application Tests
```

Migration testing must verify:

* schema changes;
* data preservation;
* constraints;
* indexes;
* rollback/recovery where supported;
* application compatibility.

No migration should be deployed without testing against a representative existing schema.

---

# 17. Academic Structure Testing

Test the complete academic hierarchy:

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

Tests must verify:

* creation;
* editing;
* relationships;
* deletion rules;
* historical preservation;
* student placement;
* teacher assignment.

The system must not depend on hardcoded class/arm structures.

---

# 18. Question Bank Testing

Question-bank tests must cover:

* question creation through approved import/processing flows;
* rich text;
* images;
* diagrams;
* mathematical notation;
* options;
* correct answer;
* versioning;
* review;
* approval;
* publication;
* withdrawal where supported.

Most importantly:

> An unapproved question must never become available for an assessment.

---

# 19. Question Lifecycle Testing

The approved lifecycle must be tested:

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

Test unauthorized transitions.

For example:

* Processing → Published without approval;
* Review Required → Assessment;
* Imported → Assessment.

These transitions must be rejected.

---

# 20. Assessment Engine Testing

Practice and CBT use the same fundamental assessment engine.

Tests must cover:

* assessment configuration;
* question selection;
* question count;
* duration;
* attempts;
* randomization;
* option randomization;
* navigation;
* skipping;
* answer changes;
* autosave;
* submission;
* scoring.

---

# 21. Deterministic Scoring Testing

Scoring must be deterministic.

Given the same:

```text
Assessment Configuration
+
Question Set
+
Student Answers
```

the scoring engine must produce the same result.

AI must not participate in deterministic MCQ scoring.

---

# 22. CBT Timer Testing

The timer must be server-authoritative.

Test:

* normal countdown;
* client clock manipulation;
* browser refresh;
* browser crash;
* reconnect;
* temporary LAN interruption;
* delayed requests;
* submission near expiry;
* expiry while disconnected.

The client must not be able to extend an assessment by manipulating local time.

---

# 23. CBT Answer Persistence Testing

Test that answers survive:

* navigation;
* refresh;
* temporary connection interruption;
* browser crash;
* client reconnection.

The latest accepted answer state must be recoverable.

---

# 24. CBT Recovery Testing

Recovery is a mandatory test category.

Test:

```text
Active Attempt
     ↓
Answer Questions
     ↓
Browser Crash
     ↓
Restart Browser
     ↓
Reconnect
     ↓
Resume Existing Attempt
```

Also test:

```text
Active Attempt
     ↓
LAN Disconnect
     ↓
Reconnect
     ↓
State Reconciliation
```

No duplicate attempt should be created unless explicitly required by the assessment rules.

---

# 25. CBT Submission Testing

Test:

* normal submission;
* submission at timer expiry;
* repeated submit requests;
* duplicate network requests;
* reconnect during submission;
* server restart recovery;
* incomplete answer sets.

Submission must be idempotent where applicable.

---

# 26. Practice Mode Testing

Practice Mode must be tested independently for:

* question selection;
* answer submission;
* immediate feedback where configured;
* progress recording;
* practice history;
* repeated practice;
* offline practice;
* synchronization of practice records.

Practice must not accidentally inherit exam restrictions that are not intended for Practice Mode.

---

# 27. Offline Testing

Offline operation is a core acceptance requirement.

Testing must explicitly simulate:

```text
LAN + Internet
LAN without Internet
No LAN
Temporary connectivity
Server restart
Client restart
```

The expected behavior for each condition must be documented.

---

# 28. Offline Login Testing

When internet connectivity is unavailable but the school LAN is operational:

* authorized users must be able to log in;
* local authentication must operate;
* local authorization must operate;
* unauthorized users must remain blocked.

The system must not require a round trip to the online system for every local login.

---

# 29. Offline CBT Testing

A complete CBT must be executable without internet access.

Test:

* login;
* assessment start;
* question loading;
* navigation;
* answer saving;
* timer;
* submission;
* scoring;
* result persistence.

This is a production-critical test.

---

# 30. Local Server Testing

The Windows local server must be tested for:

* startup;
* shutdown;
* crash recovery;
* service restart;
* watchdog;
* database availability;
* API availability;
* LAN access;
* configuration;
* logs;
* health checks;
* backup;
* restore.

---

# 31. Synchronization Testing

Synchronization must be tested as a distributed system.

Required scenarios include:

* initial sync;
* incremental sync;
* offline changes;
* reconnect;
* duplicate delivery;
* duplicate processing;
* failed operation;
* retry;
* partial batch failure;
* conflict;
* conflict resolution;
* tombstone synchronization;
* revision mismatch;
* schema mismatch.

---

# 32. Synchronization Idempotency Testing

The same synchronization operation may be delivered more than once.

Tests must verify that repeated delivery does not create duplicate records or duplicate business effects.

Examples:

* repeated student update;
* repeated answer event;
* repeated practice record;
* repeated promotion operation;
* repeated audit event where idempotency applies.

---

# 33. Synchronization Conflict Testing

Conflicts must be deliberately generated during testing.

Tests must verify the documented entity-specific conflict policy.

A generic last-write-wins strategy must not be assumed where it could cause academic or security data loss.

Conflicts that require administrative resolution must appear in the appropriate synchronization interface.

---

# 34. Synchronization Recovery Testing

Test:

```text
Sync Running
    ↓
Network Failure
    ↓
Partial Operation
    ↓
Reconnect
    ↓
Retry
    ↓
Resume
```

Verify:

* no lost changes;
* no duplicate changes;
* correct revisions;
* correct acknowledgements;
* correct audit records.

---

# 35. Content Processing Testing

Test supported source formats including:

* PDF;
* DOCX;
* images.

Testing should cover:

* successful extraction;
* OCR;
* layout handling;
* tables;
* diagrams;
* mathematical content;
* malformed files;
* oversized files;
* unsupported files;
* processing failure;
* retry;
* teacher review;
* editing;
* approval.

---

# 36. Content Security Testing

Uploaded files must be tested against:

* invalid file types;
* misleading extensions;
* oversized files;
* malformed documents;
* malicious payloads;
* path traversal attempts;
* unsafe filenames.

The system must not execute uploaded content.

---

# 37. AI Gateway Testing

The AI Gateway must be tested as a controlled boundary.

Test:

* authentication;
* authorization;
* prompt validation;
* context selection;
* model invocation;
* response handling;
* rate limits;
* resource limits;
* error handling;
* audit events.

---

# 38. RAG Testing

RAG tests must verify:

* approved content retrieval;
* relevant retrieval;
* authorization filtering;
* student scope;
* teacher scope;
* principal scope;
* unavailable content;
* deleted content;
* updated content;
* index rebuild;
* stale index handling.

---

# 39. AI Safety Testing

Test that AI cannot:

* change grades;
* publish questions;
* modify permissions;
* modify student records;
* control synchronization;
* score CBT;
* bypass authorization.

Prompt injection attempts must be tested against retrieved and uploaded content.

---

# 40. AI Failure Testing

Simulate:

* Ollama unavailable;
* model unavailable;
* insufficient resources;
* vector index unavailable;
* retrieval failure;
* timeout;
* malformed model response.

Expected behavior:

> Core LMS functionality continues operating.

---

# 41. AI Conversation Storage Testing

Test:

* conversation creation;
* message retrieval;
* authorization;
* pagination;
* retention behavior;
* metadata;
* context references.

Conversation storage must avoid unnecessarily duplicating large retrieved contexts.

---

# 42. Analytics Testing

Analytics must be tested against known datasets.

Verify:

* totals;
* averages;
* percentages;
* completion rates;
* trends;
* topic performance;
* class performance;
* student progress;
* learning gaps.

Analytics must be reproducible from authoritative source records.

---

# 43. Analytics Historical Integrity Testing

When academic records change through approved correction workflows, analytics must update appropriately without destroying historical truth.

Promotion must not rewrite historical results.

---

# 44. Dashboard Testing

Dashboard testing must verify that each role sees the correct information.

### Student

* progress;
* weak topics;
* practice history.

### Teacher

* assigned classes;
* class performance;
* weak students;
* topic gaps.

### Parent

* assigned child/children;
* child performance;
* progress;
* weak subjects.

### Principal

* school performance;
* class comparison;
* learning gaps.

### Super Admin

* users;
* system health;
* synchronization;
* administrative information.

---

# 45. Security Testing

Security testing must follow `SECURITY_REQUIREMENTS.md`.

Testing should cover:

* authentication;
* authorization;
* session security;
* password security;
* rate limiting;
* input validation;
* output encoding;
* CSRF where applicable;
* security headers;
* file upload security;
* API security;
* object-level authorization;
* secret handling;
* audit logging;
* dependency vulnerabilities;
* synchronization security;
* local-server security.

---

# 46. Penetration Testing

A production release must undergo appropriate penetration testing.

The scope should include, where applicable:

* online application;
* APIs;
* authentication;
* authorization;
* file uploads;
* local server interface;
* LAN exposure;
* synchronization endpoints;
* administrative interfaces.

Findings must be:

* recorded;
* classified;
* remediated or formally accepted;
* retested where appropriate.

---

# 47. Performance Testing

Performance testing should focus on critical workflows.

Test:

* login;
* dashboard loading;
* question loading;
* answer saving;
* assessment navigation;
* assessment submission;
* synchronization;
* analytics queries;
* AI inference where enabled.

Testing should use realistic data volumes and concurrent-user scenarios where practical.

---

# 48. Load Testing

Load testing should evaluate expected school usage rather than arbitrary internet-scale traffic.

Important scenarios include:

* many students starting an assessment;
* concurrent answer saving;
* simultaneous submission;
* teacher dashboard access;
* synchronization after prolonged offline operation.

The expected operational capacity must be established from the actual deployment requirements.

---

# 49. Storage Testing

Test storage behavior for:

* database growth;
* uploaded files;
* logs;
* backups;
* AI models;
* embeddings/vector indexes;
* conversation history.

Low-storage conditions must produce controlled warnings and failures.

---

# 50. Backup Testing

Backups must be tested for:

* successful creation;
* scheduling;
* retention;
* integrity;
* encryption where required;
* failure detection;
* restoration.

A backup that cannot be restored must not be considered a valid backup.

---

# 51. Restore Testing

Restore tests must verify recovery of:

* database;
* required configuration;
* academic data;
* user data;
* assessment data;
* synchronization state where applicable.

After restore, the system must pass integrity and application smoke tests.

---

# 52. Disaster Recovery Testing

The project must periodically test the ability to recover from:

* local server failure;
* database failure;
* storage failure;
* corrupted installation;
* lost configuration;
* synchronization interruption.

Recovery testing must follow the approved deployment and backup architecture.

---

# 53. Regression Testing

Regression testing is mandatory after changes to:

* shared components;
* authentication;
* authorization;
* database schema;
* API contracts;
* assessment engine;
* synchronization;
* offline infrastructure;
* analytics calculations;
* AI Gateway;
* deployment infrastructure.

A previously passing test must not be removed simply because it becomes inconvenient.

---

# 54. Test Data

Test data must be synthetic or specifically approved for testing.

Do not use real student passwords or unnecessary real personal information in development/test environments.

Test datasets should cover:

* multiple classes;
* multiple arms;
* multiple subjects;
* multiple teachers;
* multiple students;
* parent relationships;
* multiple assessment attempts;
* incomplete data;
* edge cases.

---

# 55. Test Environment Separation

Development, testing, staging, and production environments must remain logically separated.

Production data must not be copied into development environments without an approved sanitization process.

Credentials must be environment-specific.

---

# 56. Test Isolation

Automated tests should avoid depending on:

* test execution order;
* external personal accounts;
* uncontrolled internet services;
* developer machines;
* manually modified databases.

Tests should create or reset their required state deterministically.

---

# 57. Test Naming and Organization

Tests should clearly identify:

* feature;
* scenario;
* expected result.

Critical business rules should have tests that directly communicate the rule being protected.

---

# 58. Defect Classification

Defects should be classified according to impact.

### Critical

Examples:

* data corruption;
* unauthorized access;
* incorrect exam scoring;
* loss of active CBT answers;
* security bypass;
* catastrophic synchronization corruption.

### High

Examples:

* major workflow failure;
* incorrect academic data;
* major offline failure;
* inability to recover assessments.

### Medium

Examples:

* significant non-critical workflow failure;
* incorrect non-critical analytics;
* degraded functionality with a workaround.

### Low

Examples:

* minor UI issue;
* cosmetic defect;
* low-impact usability issue.

Critical and high-impact defects must be resolved or formally dispositioned before the affected release is accepted.

---

# 59. Test Failure Policy

When a test fails:

1. reproduce the failure;
2. identify the affected component;
3. determine whether the issue is implementation, test, environment, or requirement;
4. fix the underlying issue;
5. rerun the failed test;
6. run relevant regression tests.

Tests must not be weakened merely to produce a passing build.

---

# 60. Module Testing Gates

Each development module has a testing gate.

## Module 00

* foundation tests;
* database tests;
* API tests;
* configuration tests;
* build checks.

## Module 01

* authentication;
* authorization;
* account lifecycle;
* role testing.

## Module 02

* academic hierarchy;
* assignments;
* question bank;
* publication lifecycle.

## Module 03

* assessment;
* scoring;
* timer;
* autosave;
* recovery;
* offline operation.

## Module 04

* teacher workflows;
* assignment scoping;
* assessment management.

## Module 05

* local server;
* offline operation;
* synchronization;
* recovery;
* conflict handling.

## Module 06

* file processing;
* OCR;
* extraction;
* review/approval lifecycle.

## Module 07

* AI Gateway;
* RAG;
* authorization;
* AI restrictions;
* failure isolation.

## Module 08

* analytics accuracy;
* learning gaps;
* dashboard access;
* historical integrity.

## Module 09

* security;
* penetration testing;
* deployment;
* backup;
* restore;
* disaster recovery;
* final acceptance.

---

# 61. End-to-End Critical Workflows

The following workflows require end-to-end testing.

### Student Assessment

```text
Login
 ↓
Select Assessment
 ↓
Start Attempt
 ↓
Load Questions
 ↓
Answer
 ↓
Navigate
 ↓
Autosave
 ↓
Submit
 ↓
Score
 ↓
Record Result
```

### Teacher Question Workflow

```text
Question Import
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
Assessment Availability
```

### Offline CBT Workflow

```text
Local Login
 ↓
Start CBT
 ↓
Answer
 ↓
Connection Interruption
 ↓
Recovery
 ↓
Continue
 ↓
Submit
 ↓
Score
 ↓
Queue for Sync
```

### Synchronization Workflow

```text
Local Change
 ↓
Outbox
 ↓
Sync
 ↓
Online Processing
 ↓
Acknowledgement
 ↓
Local State Update
```

### Parent Workflow

```text
Parent Login
 ↓
Authorized Child Selection
 ↓
Performance
 ↓
Progress
 ↓
Weak Subjects
```

---

# 62. Production Smoke Tests

Immediately after production deployment, run a controlled smoke test.

At minimum:

* application availability;
* login;
* authorization;
* student dashboard;
* teacher dashboard;
* parent dashboard;
* principal dashboard;
* Super Admin access;
* database health;
* local server health;
* synchronization;
* backup status.

For local deployment, also verify LAN access and offline-critical functionality.

---

# 63. Release Gate

A release may proceed only when:

* required tests pass;
* critical defects are resolved or formally accepted;
* migrations are verified;
* security checks pass;
* required regression tests pass;
* documentation is synchronized;
* deployment artifacts are verified.

---

# 64. Production Acceptance Gate

Production acceptance requires evidence that:

```text
Functional Tests
+
Integration Tests
+
End-to-End Tests
+
Offline Tests
+
Synchronization Tests
+
Security Tests
+
Performance Tests
+
Recovery Tests
+
Backup/Restore Tests
+
Acceptance Tests
```

have passed at the required level.

---

# 65. Test Evidence

For important release gates, retain appropriate evidence such as:

* test results;
* CI results;
* security reports;
* penetration-test reports;
* migration results;
* backup/restore results;
* offline acceptance results;
* synchronization results;
* performance measurements;
* defect records.

Evidence must not contain unnecessary credentials or sensitive personal information.

---

# 66. Testing and Documentation

Testing must remain synchronized with requirements.

When a requirement changes:

```text
Requirement
    ↓
Architecture
    ↓
Implementation
    ↓
Tests
    ↓
Documentation
```

must be reviewed for consistency.

A requirement without corresponding validation should be treated as a traceability gap.

---

# 67. Requirements Traceability

Critical requirements should be traceable to tests.

The traceability relationship is:

```text
Requirement ID
     ↓
Implementation
     ↓
Test Case
     ↓
Test Result
```

This is particularly important for:

* authentication;
* authorization;
* CBT;
* offline operation;
* synchronization;
* scoring;
* security;
* backups;
* recovery.

---

# 68. Testing Non-Negotiables

The following are mandatory:

1. No production release without required regression testing.
2. No CBT release without recovery testing.
3. No offline release without offline testing.
4. No synchronization release without conflict and retry testing.
5. No authentication release without authorization testing.
6. No database migration without migration testing.
7. No production deployment without backup verification.
8. No production acceptance without required security testing.
9. No AI release without authorization and AI-boundary testing.
10. No question-bank release that allows unapproved questions into assessments.
11. No test bypass merely to achieve a passing build.
12. No undocumented critical behavior changes.

---

# 69. Final Testing Principle

Testing is part of development, not a final activity performed after development is finished.

The project follows:

```text
Build
 ↓
Test
 ↓
Integrate
 ↓
Regression Test
 ↓
Verify
 ↓
Document
 ↓
Release
```

For NAKAMA, correctness means more than displaying the right screen.

The system must demonstrate that:

* users can access only what they are authorized to access;
* academic data remains correct;
* assessments score deterministically;
* CBT attempts survive expected failures;
* the school can operate without internet access;
* synchronization can recover from distributed-system failures;
* AI remains within its defined boundaries;
* backups can actually restore the system;
* security controls withstand appropriate testing.

> **A feature is not complete when it works once. It is complete when its required behavior has been verified, its failure modes have been tested, its integrations have been validated, and it can safely coexist with the rest of the system.**
