# CRC — Christian Royal College Integrated Academic System

## 1. Document Purpose

This document is the authoritative high-level context for the CRC project.

It defines what the system is, why it exists, who uses it, the boundaries of the product, its major capabilities, its operating model, and the architectural principles that govern implementation.

All implementation work must remain consistent with this document.

Detailed technical behavior belongs in the appropriate requirements, architecture, technical design, and module documentation.

---

# 2. Project Identity

**Product Name:** CRC

**School / Customer / Deployment Context:** Christian Royal College

**System Type:** Integrated Academic Learning, Assessment, Practice, Analytics, Content Processing, and Offline School Computing Platform

**Product:** CRC is the Academic Learning, Assessment, Practice, Analytics, and Offline School Computing System developed in this repository.

**Company / Technology Provider:** NAKAMA develops/provides CRC.

**Product Attribution:** CRC powered by NAKAMA

**Repository Scope:** CRC LMS / Management System and its documented technical systems only. The NAKAMA corporate website is a separate future project outside this scope. Corporate marketing, service catalogs, pricing pages, blogs/resources, lead-generation architecture, unrelated NAKAMA products/services, and company management systems are not included.

**Design-System Scope:** CRC Product Design System, with NAKAMA attribution where appropriate. The future NAKAMA corporate website requires its own separate design system.

**Primary Environment:** School academic environment

**Primary Operational Model:** Offline-first school LAN with an online/cloud master system and synchronization layer

---

# 3. Project Objective

CRC is an academic technology platform designed to provide Christian Royal College with a reliable system for:

* Academic content management
* Digital question-bank management
* Student practice
* Computer-Based Testing (CBT)
* Student academic progress tracking
* Teacher academic monitoring
* Parent academic visibility
* Principal/management intelligence
* Learning-gap identification
* Online academic content processing
* Offline school-LAN operation
* Local academic AI assistance
* Academic analytics
* Controlled synchronization between the school's offline environment and the online master system

The system is designed around the operational reality that the school must be able to conduct academic activities even when internet connectivity is unavailable.

Internet connectivity is therefore not a dependency for the student's core academic workflow.

---

# 4. Core Product Principle

The most important operational principle is:

> **The school must be able to continue academic operations without internet access.**

The local school server is therefore a real operational system, not merely a browser cache or offline copy of the online application.

The local environment must independently support the critical academic workflow, including:

* Student authentication
* Practice
* CBT
* Question delivery
* Answer recording
* Scoring
* Progress recording
* Local academic content access
* Local AI assistance where permitted

Internet connectivity is primarily used for synchronization and online functionality.

---

# 5. Product Architecture at a Glance

CRC consists of two major operational environments.

## 5.1 Online / Cloud Environment

The online environment acts as the master academic system and provides services such as:

* Online academic dashboards
* Parent dashboard
* Principal dashboard
* Master academic data
* Content processing
* OCR and document normalization
* Synchronization coordination
* Centralized data storage
* Administrative functions requiring online access
* Cloud-based system management

The online production database is PostgreSQL.

---

## 5.2 School Local Environment

The school has a dedicated local server PC running the CRC Local Server application.

The local server provides the operational academic environment over the school's LAN.

It contains:

* Local API
* Local web application
* Local authentication
* Local database
* Practice engine
* CBT engine
* Local progress recording
* Synchronization engine
* Local AI services
* AI/RAG storage
* Background workers
* Watchdog/recovery services
* Logging
* Monitoring
* Server configuration
* Health checks

The local database is SQLite configured for WAL operation.

---

# 6. Local Server Application

The school local server will be delivered as a Windows application.

The local server application will be packaged as an executable installer/application and installed on the designated school server PC.

The server application will provide a professional administration and monitoring interface for the Super Admin/IT.

The local server application must support:

* Start server
* Stop server
* Restart server
* Server status monitoring
* Service status monitoring
* Database status
* AI service status
* Synchronization status
* Connected-client monitoring
* Server logs
* Error logs
* Health information
* Endpoint visibility
* Server configuration
* Network configuration
* Port configuration
* Backup configuration
* Storage configuration
* AI configuration
* Synchronization configuration
* Service recovery
* System diagnostics

The server application acts as the operational control plane for the local school environment.

---

# 7. Local Network Operation

Student and teacher computers connect to the local server through the school's LAN.

The local server is not intended to be publicly exposed to the internet.

LAN access will use a secure local addressing/hostname strategy with appropriate Windows firewall restrictions and network binding.

The server should bind only to the interfaces required for school-LAN operation.

Public exposure of the local server is not part of the normal operating model.

---

# 8. User Roles

CRC V1 contains five primary roles.

## 8.1 Super Admin / IT

The Super Admin is the primary system administrator.

Responsibilities include:

* Account provisioning
* Account management
* Password resets
* Account activation/deactivation
* Student management
* Parent management
* Teacher management
* Principal management
* Academic configuration
* Class/arm configuration
* Subject configuration
* Teacher assignments
* CBT configuration
* Assessment configuration
* Content management oversight
* Synchronization management
* Local server management
* System health monitoring
* Security administration
* Audit review
* Promotion management
* System configuration

There is no public registration process.

The Super Admin controls account creation and lifecycle management.

MFA is not required for V1.

---

## 8.2 Principal

The Principal is a dedicated management role with a dedicated dashboard.

The Principal represents the management/leadership academic view of the system.

The Principal dashboard focuses on:

* School performance
* Class comparison
* Learning gaps
* Academic trends
* High-level student performance
* Academic intelligence

The Principal is not equivalent to the Super Admin.

The Super Admin controls system administration.

The Principal consumes and interprets academic information.

---

## 8.3 Teacher

Teachers use the system for academic activities assigned to them.

Teacher access is determined by Super Admin assignments.

A teacher may be assigned:

* Multiple classes
* Multiple arms
* Multiple subjects

A teacher only sees the academic areas they are authorized to access.

The teacher dashboard focuses on:

* Class performance
* Weak students
* Topic gaps
* Assigned subjects/classes
* Assessment activity
* Question/content review
* Student academic performance

---

## 8.4 Parent

Parents have provisioned accounts.

Parents cannot self-register.

A parent account may be linked to multiple students.

Each student may have only one parent account in V1.

Parent-child relationships are explicit and are established using student identifiers.

Parents see only students explicitly linked to their account.

The parent dashboard focuses on:

* Child performance
* Child progress
* Weak subjects

---

## 8.5 Student

Students use the system primarily for:

* Academic practice
* CBT
* Assessments
* Academic content
* Progress tracking
* Approved local AI tutoring/explanations

Students do not self-register.

Student accounts are provisioned by the Super Admin.

Students use their assigned Student ID as their identity within the academic system.

Students receive an initial password and must change it on first login.

---

# 9. Account Provisioning

There is no public registration.

All accounts are provisioned by the Super Admin.

The system must support administrative account creation and appropriate bulk import workflows.

Student and staff data may be imported using supported structured data files such as CSV, subject to the finalized import specification.

Account management includes:

* Create
* Update
* Activate
* Deactivate
* Reset password
* Assign role
* Assign academic relationships
* Audit changes

---

# 10. First Login

Students, teachers, and parents must change their initial password during their first successful login.

The initial password is not considered the permanent user credential.

The system must prevent normal continued use of an account requiring first-login password change until the required password change has been completed.

---

# 11. Academic Structure

The academic model follows:

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

The system must remain flexible enough to accommodate changes in school structure.

---

# 12. Class and Arm Structure

The system must not hardcode the current school class structure.

The class/arm model must support flexible configuration.

For example:

```text
JSS 1
 ├── A
 └── B

JSS 2
 ├── A
 ├── B
 └── C

JSS 3
 ├── A
 ├── B
 └── C
```

Senior Secondary classes may be organized into streams such as:

```text
SS 1
 ├── Science
 ├── Arts
 └── Commercial

SS 2
 ├── Science
 ├── Arts
 └── Commercial

SS 3
 ├── Science
 ├── Arts
 └── Commercial
```

The actual available classes, arms, and streams are configurable by the Super Admin.

A student belongs to an assigned class/arm.

---

# 13. Teacher Assignment

Teachers may teach multiple classes and subjects.

However, teacher access is assignment-based.

A teacher must be explicitly assigned by the Super Admin to the relevant academic scope.

Teacher assignments determine what appears in the teacher's dashboard and what data the teacher can access.

---

# 14. Academic Session Promotion

CRC must support academic-session promotion.

At the end of an academic session, the system can identify students eligible for promotion to the next academic class/arm.

However:

> **Promotion must never occur automatically without Super Admin activation.**

The system may detect and prepare promotion operations automatically.

The Super Admin must explicitly activate/approve the promotion process.

If the Super Admin does not activate the promotion operation, students are not automatically promoted.

Manual promotion must remain available.

Historical academic records must remain associated with the previous session/class context.

---

# 15. Question Bank

V1 supports Multiple Choice Questions (MCQ).

Questions must be data-driven and must never be hardcoded into application source code.

Questions must support rich presentation requirements including:

* Question text
* Answer options
* Images
* Diagrams
* Mathematical notation
* Mathematical expressions
* Rich structured content where required

The question renderer must be designed so that academic content is stored as data and rendered dynamically.

---

# 16. Question Import

Teachers will primarily import questions rather than manually hardcode questions into the application.

Imported questions enter a controlled review workflow.

The workflow is:

```text
Imported
    ↓
Processed
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

Unapproved questions must not become available for student assessments.

---

# 17. Practice and CBT

Practice Mode and CBT Mode use the same fundamental assessment engine.

The purpose of Practice Mode is to prepare students for tests and examinations.

The system should therefore maintain consistency between practice and formal assessment behavior.

The assessment engine must support:

* Question navigation
* Backward navigation
* Forward navigation
* Skipping questions
* Changing answers
* Automatic answer persistence
* Configurable question counts
* Configurable duration
* Configurable attempts
* Question randomization
* Option randomization where configured
* Automatic scoring
* Immediate result delivery

---

# 18. CBT Configuration

The Super Admin controls CBT configuration.

Configuration must support different academic sections, including Junior and Senior Secondary environments.

Configuration may include:

* Subject
* Target class/arm
* Question count
* Duration
* Attempt count
* Question selection
* Randomization
* Assessment availability

The system must not hardcode assessment configuration.

---

# 19. CBT Recovery and Watchdog

CBT must be resilient against:

* Browser crashes
* Browser refresh
* Computer failure
* Temporary LAN interruption
* Application interruption

The local server is the authoritative source of active CBT attempt state.

The client maintains appropriate temporary recovery state to prevent loss of unsent information.

A watchdog/recovery mechanism monitors active assessment sessions and supports seamless recovery.

When a student reconnects, the system should:

1. Identify the existing attempt.
2. Validate the session.
3. Reconcile available answer state.
4. Restore the attempt.
5. Restore the appropriate timer state.
6. Allow the student to continue without unnecessarily restarting the assessment.

The system must prevent duplicate or conflicting attempt states.

---

# 20. Offline-First Academic Operation

The student academic workflow must operate without internet connectivity.

The following must work locally:

* Login
* Practice
* CBT
* Question delivery
* Answer submission
* Scoring
* Progress recording
* Approved academic content
* Permitted local AI functionality

Internet access must not be required for these core workflows.

---

# 21. Cloud and Local Relationship

The online environment is the master environment for synchronized academic data.

The local school environment is the operational environment for offline academic activity.

The relationship is conceptually:

```text
Online Master
      ↕
Synchronization Layer
      ↕
School Local Server
      ↓
School LAN
      ↓
Student / Teacher Computers
```

The synchronization architecture must be designed to prevent uncontrolled overwrites and inconsistent state.

---

# 22. Synchronization

Synchronization must use a professional change-tracking architecture rather than blindly copying databases.

The synchronization system should support:

* Change tracking
* Record versioning
* Idempotent operations
* Retry handling
* Failure recovery
* Conflict detection
* Conflict resolution
* Sync queues
* Sync status
* Auditability
* Integrity validation

The final synchronization protocol and conflict-resolution implementation will be defined in the dedicated synchronization architecture documentation.

The Super Admin must be able to monitor:

* Current sync status
* Last successful synchronization
* Failed synchronization
* Pending operations
* Retry status
* Sync errors
* Synchronization history

---

# 23. Online Content Processing

Content processing is strictly an online/cloud responsibility.

Ollama must not be used as the primary content-normalization engine.

The content-processing pipeline may use:

* PaddleOCR
* PP-OCR
* PP-Structure
* Appropriate document parsers
* Layout analysis
* Content extraction
* Normalization services

Supported source content should include at minimum:

* PDF
* DOCX
* Images

The processing pipeline should be capable of extracting and preserving where applicable:

* Text
* Headings
* Tables
* Images
* Diagrams
* Questions
* Options
* Answers
* Mathematical content
* Structural relationships

---

# 24. Content Review

Extracted or generated academic content must not automatically become trusted production content.

Teachers must be able to review and edit processed content before publication.

The workflow is:

```text
Source Material
      ↓
Processing
      ↓
Extraction
      ↓
Normalization
      ↓
Teacher Review
      ↓
Correction/Edit
      ↓
Approval
      ↓
Publication
      ↓
Synchronization
```

---

# 25. Local Academic AI

Ollama is reserved for local academic AI functionality.

The local AI may support:

### Student

* Tutoring
* Academic assistance
* Question explanations
* Study assistance

### Teacher

* Academic intelligence
* Learning-gap interpretation
* Class-level academic insights

### Principal

* Academic intelligence
* School-level performance interpretation
* Learning-gap interpretation

AI must not replace deterministic business logic.

AI must not:

* Calculate authoritative scores
* Decide answer correctness
* Modify grades
* Modify student records
* Change permissions
* Publish content
* Control synchronization
* Override security controls

---

# 26. AI During CBT

AI assistance must be disabled during formal CBT/examination mode.

The assessment engine remains deterministic and independent of the AI system.

AI failure must never prevent CBT or practice from functioning.

---

# 27. AI Knowledge and Retrieval

Local AI should primarily operate using approved school academic content and relevant student academic context.

The intended architecture is retrieval-augmented:

```text
Approved Academic Content
          ↓
       Chunking
          ↓
      Embeddings
          ↓
    Local Vector Index
          ↓
   Relevant Retrieval
          ↓
     Ollama Model
          ↓
     AI Response
```

The system should avoid unnecessary duplication of source content and conversational context.

AI storage must prioritize efficient retrieval and reasonable storage consumption.

The exact embedding model, vector technology, Ollama model, context strategy, and storage format will be frozen in the AI architecture specification.

---

# 28. AI Data Retention

AI interactions may be stored to support appropriate academic continuity, auditing, and improvement.

Storage should be structured efficiently rather than repeatedly duplicating large context payloads.

Retention requirements will be defined in the data-retention/security specifications.

---

# 29. Student Dashboard

The student dashboard focuses on the student's academic journey.

Core areas include:

* Progress
* Weak topics
* Practice history
* Assessments
* Results
* Approved academic content
* Permitted AI academic assistance

---

# 30. Teacher Dashboard

The teacher dashboard focuses on:

* Assigned classes
* Assigned subjects
* Class performance
* Weak students
* Topic gaps
* Assessment activity
* Question/content review
* Student performance

Teacher visibility is restricted according to Super Admin assignments and permissions.

---

# 31. Parent Dashboard

The parent dashboard focuses on:

* Child performance
* Child progress
* Weak subjects

A parent can access only explicitly linked students.

A single parent account may be associated with multiple students.

A student may have only one parent account in V1.

---

# 32. Principal Dashboard

The Principal dashboard is the management dashboard.

It focuses on:

* School performance
* Class comparison
* Learning gaps
* Academic trends
* High-level academic intelligence

The Principal does not receive Super Admin system-management privileges merely because of the Principal role.

---

# 33. Super Admin Dashboard

The Super Admin dashboard focuses on operational and administrative control.

Core areas include:

* User management
* Academic configuration
* Teacher assignments
* CBT configuration
* Promotion management
* System health
* Server status
* Synchronization status
* Logs
* Diagnostics
* Security/audit
* Configuration

---

# 34. Local Server Monitoring

The local server application must provide visibility into the health of the local environment.

Monitoring should include:

* Server status
* API status
* Database status
* AI service status
* Synchronization status
* Background workers
* Connected devices
* Storage
* Logs
* Errors
* Service health

---

# 35. Endpoint Visibility

The local server management application should expose an authenticated diagnostic view of the application's API endpoints.

The endpoint view is intended for Super Admin/IT diagnostics and development/maintenance.

It must not expose sensitive administrative operations to unauthorized users.

---

# 36. Logging

The system must provide structured logging.

Relevant logging areas include:

* Authentication
* Authorization
* Application
* Database
* CBT
* Practice
* Synchronization
* AI
* Content processing
* Background jobs
* Server
* Security
* Errors

Logs must be appropriately protected and retained according to the finalized retention policy.

---

# 37. Security Baseline

Security is a first-class requirement.

The system will follow a professional application-security baseline aligned with OWASP application-security practices and a formal penetration-testing process.

Security controls include, where applicable:

* Secure password hashing
* Authentication security
* Server-side authorization
* RBAC
* Session security
* Rate limiting
* Input validation
* Output encoding
* CSRF protection where applicable
* Security headers
* Content Security Policy
* SQL injection prevention
* XSS protection
* Secure file uploads
* File validation
* Malware scanning where applicable
* Secure secrets handling
* Audit logging
* Backup protection
* Synchronization security
* API security
* Dependency vulnerability management
* Secure error handling
* Penetration testing
* Security audit

---

# 38. Database Architecture

## Online

PostgreSQL is the authoritative online production database.

## Local

SQLite with WAL is the local operational database.

The local database is optimized for reliable school-LAN operation.

The local and online schemas may differ where necessary to satisfy their respective operational requirements, provided that the synchronization contract remains explicit and reliable.

---

# 39. Data Integrity

Critical business rules must be enforced deterministically.

The following must never depend on AI:

* Authentication
* Authorization
* User identity
* Student-parent relationships
* Teacher assignments
* Question approval
* Assessment state
* Answer persistence
* Score calculation
* Promotion activation
* Synchronization state
* Audit events

---

# 40. Auditability

Important administrative and academic actions must be auditable.

Audit coverage will include appropriate events such as:

* Login
* Failed login
* Logout
* Account creation
* Account modification
* Account deactivation
* Password reset
* Role changes
* Academic configuration
* Parent/student linking
* Content processing
* Content approval
* Question publication
* Assessment configuration
* Score modification
* Promotion operations
* Synchronization
* Security events
* Administrative actions

The detailed audit schema and retention period will be defined in the security architecture.

---

# 41. Deployment Model

The school local server is deployed on a dedicated Windows PC.

The deployment includes:

* CRC Local Server executable/application
* Local database
* Local API
* Local academic web interface
* Sync engine
* Watchdog/recovery system
* Local AI services
* Logging and monitoring

Client computers access the local application over the school LAN using a controlled local address/hostname and configured port.

---

# 42. Service Isolation

The local server should be architected as cooperating services/components rather than one monolithic process.

Conceptually:

```text
CRC Local Server Manager
          │
          ├── LMS Application/API
          ├── Database
          ├── Sync Worker
          ├── AI Service
          ├── Background Jobs
          ├── Watchdog
          └── Monitoring/Logging
```

Failure of one non-critical component must not unnecessarily bring down the entire academic system.

The CBT engine, in particular, must not become dependent on AI availability.

---

# 43. Backup and Recovery

The system must support controlled backups of important local and online data.

Backup strategy, retention, encryption, restore testing, and disaster-recovery procedures will be specified in the deployment and security documentation.

---

# 44. Scope Boundaries

The CRC project is an academic platform.

The following are outside the core scope unless explicitly added later:

* School fees/payment processing
* Payment gateway
* HR/payroll
* Admissions management
* Replacement of the school's entire general-purpose portal
* SMS infrastructure
* Email infrastructure
* General enterprise resource planning

The LMS may integrate with external systems in the future where technically and commercially appropriate.

---

# 45. Architectural Principles

The project follows these principles.

### 45.1 Offline First

The critical school academic workflow must remain functional without internet connectivity.

### 45.2 Deterministic Core

Business-critical behavior must not depend on probabilistic AI.

### 45.3 Explicit Relationships

Relationships such as parent/student and teacher/class assignments must be explicitly stored.

### 45.4 Least Privilege

Users only receive the access required for their role and assignments.

### 45.5 Server Authority

The local server is authoritative for active local CBT state.

### 45.6 Cloud Master

The online environment is the master synchronized academic environment.

### 45.7 Review Before Publication

Machine-extracted or machine-generated academic content requires appropriate human review before publication.

### 45.8 AI as an Assistant

AI supports learning and interpretation; it does not control authoritative academic records.

### 45.9 Failure Isolation

Failure of AI, synchronization, or other non-critical services must not unnecessarily stop core academic operation.

### 45.10 Auditability

Important system and academic operations must be traceable.

### 45.11 Security by Design

Security is incorporated into architecture and implementation rather than added after development.

### 45.12 Configuration Over Hardcoding

Academic structures, classes, arms, subjects, assessments, and operational settings must be configurable rather than hardcoded.

---

# 46. Development Philosophy

CRC will be developed as a sequence of independently testable vertical modules.

The development lifecycle is:

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
Unit Test
    ↓
Integration Test
    ↓
E2E Test where applicable
    ↓
Security Validation
    ↓
Regression Test
    ↓
Document
    ↓
Freeze
    ↓
Next Module
```

A module is not considered complete merely because its code compiles or its UI works.

Completion requires implementation, automated testing, integration validation, security checks, documentation, acceptance criteria, and regression validation.

---

# 47. Module Roadmap

The current module structure is:

```text
00 — Foundation & Engineering Infrastructure
01 — Identity, Account Provisioning & Access Control
02 — Academic Core & Content Management
03 — Student CBT & Practice Engine
04 — Teacher Dashboard & Assessment Management
05 — Offline School System & Synchronization
06 — Content Processing & Academic Material Intelligence
07 — Local Academic AI
08 — Analytics, Learning Gaps & Management Intelligence
09 — Security, Deployment, Hardening & Acceptance
```

Modules are vertical business capabilities.

A module should contain all necessary frontend, backend, database, testing, documentation, and supporting infrastructure required for its scope.

---

# 48. Source of Truth

The repository is the source of truth for the project.

The implementation must not depend on undocumented chat history.

The hierarchy of authority is:

1. `AGENTS.md`
2. `PROJECT_CONTEXT.md`
3. `PROJECT_STATE.md`
4. `DEVELOPMENT_ROADMAP.md`
5. `ARCHITECTURE.md`
6. `REQUIREMENTS.md`
7. `TESTING_STRATEGY.md`
8. `SECURITY_REQUIREMENTS.md`
9. Applicable ADRs
10. Applicable module documentation
11. Technical documentation
12. Source code and tests

Where documents conflict, the conflict must be identified and resolved rather than silently choosing one.

---

# 49. Change Management

Major architectural decisions must be documented through Architecture Decision Records (ADRs).

Examples include:

* Database selection
* Offline architecture
* Synchronization strategy
* Local AI architecture
* Authentication model
* CBT recovery strategy
* Deployment architecture
* Academic promotion model

Changes to confirmed architecture must not be introduced casually.

---

# 50. Current Architectural Direction

The following decisions are currently considered confirmed:

| Area                        | Decision                                                      |
| --------------------------- | ------------------------------------------------------------- |
| Registration                | No public registration                                        |
| Account provisioning        | Super Admin                                                   |
| Roles                       | Student, Parent, Teacher, Super Admin, Principal              |
| First-login password change | Required for students, teachers and parents                   |
| MFA                         | Not required for V1                                           |
| Academic hierarchy          | Session → Term → Class → Arm → Subject → Topic → Question     |
| Question type               | MCQ for V1                                                    |
| Question media              | Images, diagrams and mathematical notation supported          |
| Question source             | Imported/data-driven, not hardcoded                           |
| Question approval           | Required before publication                                   |
| Practice/CBT                | Shared assessment engine                                      |
| CBT navigation              | Back, forward, skip, change answer                            |
| Answer persistence          | Automatic                                                     |
| CBT randomization           | Supported                                                     |
| CBT configuration           | Super Admin                                                   |
| CBT results                 | Immediate                                                     |
| Offline operation           | Required                                                      |
| Local server                | Windows                                                       |
| Local server application    | Executable/application                                        |
| Online database             | PostgreSQL                                                    |
| Local database              | SQLite + WAL                                                  |
| Online content processing   | Required                                                      |
| OCR                         | PaddleOCR/PP-OCR/PP-Structure where appropriate               |
| Ollama content processing   | No                                                            |
| Local AI                    | Ollama                                                        |
| AI during CBT               | Disabled                                                      |
| Parent relationship         | One parent per student; one parent may have multiple students |
| Teacher assignment          | Super Admin controlled                                        |
| Promotion                   | System-assisted, Super Admin activated                        |
| CBT recovery                | Required                                                      |
| Watchdog                    | Required                                                      |
| Sync monitoring             | Required                                                      |
| Principal                   | Dedicated management role/dashboard                           |

---

# 51. Remaining Technical Design Work

The major product decisions are now sufficiently defined.

The remaining work is primarily technical design rather than additional product discovery.

The next architecture work must define:

* Exact technology stack
* Online/cloud deployment topology
* Local server component architecture
* Local server packaging and service management
* API architecture
* Database schema
* Synchronization protocol
* Synchronization conflict resolution
* Authentication/session architecture
* CBT state machine
* CBT watchdog architecture
* Content-processing pipeline
* AI/RAG architecture
* Vector storage strategy
* Logging/observability
* Backup/recovery architecture
* Network security
* API security
* Deployment/update strategy
* Testing architecture
* CI/CD strategy

These decisions should be documented before implementation begins.

---

# 52. Implementation Standard

CRC must be built as a production-quality system suitable for real school operation.

The implementation must prioritize:

* Reliability
* Security
* Maintainability
* Testability
* Offline resilience
* Data integrity
* Observability
* Clear separation of responsibilities
* Controlled synchronization
* Graceful failure
* Professional user experience
* Long-term maintainability

No implementation should be introduced merely because it is convenient.

Technical decisions should be justified by the operational requirements of the system.
