# Security Requirements

**Project:** CRC LMS
**Document:** Security Requirements Specification
**Status:** Baseline
**Scope:** V1
**Security Baseline:** OWASP ASVS-aligned secure application engineering

---

## 1. Purpose

This document defines the security requirements, controls, threat model, security boundaries, and acceptance criteria for the CRC system.

Security applies across:

* Online master system
* School local server
* Windows server host
* School LAN
* Student devices
* Teacher devices
* Principal devices
* Super Admin interfaces
* Parent dashboards
* Authentication
* Authorization
* APIs
* Databases
* Synchronization
* Content processing
* File uploads
* CBT and practice
* Local AI
* Backups
* Logging
* Deployment
* Updates
* Third-party dependencies

Security is a system-wide requirement and must not be treated as a separate feature added after implementation.

---

# 2. Security Objectives

CRC MUST provide:

1. Confidentiality of protected academic and account data.
2. Integrity of academic records and assessment results.
3. Availability of critical offline school workflows.
4. Strong authentication.
5. Strict authorization.
6. Protection against privilege escalation.
7. Protection against unauthorized modification of student records.
8. Protection against assessment manipulation.
9. Protection against unauthorized local-network access.
10. Secure synchronization between online and local systems.
11. Secure processing of uploaded academic materials.
12. Safe use of local AI.
13. Traceability of security-sensitive actions.
14. Secure backup and recovery.
15. Controlled administrative access.
16. Protection against common web and API vulnerabilities.
17. Detection and investigation of security incidents.
18. Security verification before production acceptance.

---

# 3. Security Principles

The implementation MUST follow these principles.

## 3.1 Least Privilege

Every user, service, process, API and database account MUST have only the permissions required for its function.

## 3.2 Deny by Default

Access MUST be denied unless explicitly permitted.

## 3.3 Server-Side Authorization

Security decisions MUST be enforced server-side.

Client-side role checks MUST NOT be treated as security controls.

## 3.4 Separation of Duties

Administrative operations that have significant impact MUST be separated from ordinary academic operations.

## 3.5 Defense in Depth

Security MUST rely on multiple independent controls rather than a single mechanism.

## 3.6 Secure by Default

New features, endpoints, accounts, files, services and configurations MUST default to the safest reasonable state.

## 3.7 Fail Securely

When security validation fails, the system MUST reject the operation rather than continue in a partially authorized state.

## 3.8 Complete Mediation

Every protected operation MUST be authorized.

Authorization MUST NOT be assumed because an earlier request was authorized.

## 3.9 Minimize Sensitive Data

The system MUST store only information required for legitimate functionality, auditability and recovery.

## 3.10 Explicit Trust Boundaries

The system MUST treat the following as separate trust boundaries:

* Internet
* Online application
* Online database
* School local server
* School LAN
* Student device
* Teacher device
* Principal device
* Super Admin device
* Parent device
* Content-processing services
* AI services

---

# 4. Security Baseline

CRC application security MUST be designed against the principles and control areas of the OWASP Application Security Verification Standard.

The implementation MUST address, where applicable:

* Authentication
* Session management
* Access control
* Input validation
* Output encoding
* Cryptography
* API security
* File handling
* Error handling
* Logging
* Data protection
* Configuration security
* Dependency security
* Communication security
* Secure architecture

Security requirements in this document are mandatory regardless of whether a specific control is implemented through application code, infrastructure, database configuration or deployment configuration.

---

# 5. Threat Model

## 5.1 Threat Actors

The system MUST consider at least the following threat actors.

### Unauthenticated Internet User

May attempt:

* account enumeration
* credential attacks
* API abuse
* vulnerability exploitation
* malicious file uploads
* injection attacks
* denial-of-service
* session attacks

### Compromised User Account

A legitimate account may be compromised and used to:

* access unauthorized records
* extract information
* modify permitted data maliciously
* abuse APIs
* attempt privilege escalation

### Malicious Student

May attempt:

* accessing another student's account
* manipulating CBT answers
* modifying browser state
* bypassing timers
* accessing teacher/admin endpoints
* viewing restricted questions
* extracting question banks
* exploiting LAN services

### Malicious or Compromised Teacher

May attempt:

* accessing unauthorized classes
* modifying unauthorized academic data
* exporting restricted information
* manipulating questions or assessments
* abusing privileged APIs

### Compromised Parent Account

May attempt to:

* access another student's information
* manipulate account relationships
* access teacher/admin functionality
* enumerate students

### Malicious Insider / Administrator

Must be considered because administrators have elevated access.

Controls MUST include:

* audit logging
* authorization boundaries
* sensitive-action logging
* account lifecycle controls
* least privilege
* database access restrictions
* operational monitoring

### Compromised Local Device

A device connected to the school LAN may attempt:

* service discovery
* unauthorized API access
* credential attacks
* endpoint enumeration
* data extraction
* malicious traffic

### Compromised Local Server

A compromised server represents a high-impact security event.

The architecture MUST therefore minimize:

* exposed services
* unnecessary privileges
* stored secrets
* administrative interfaces
* outbound connections

### Malicious Uploaded Content

Uploaded PDFs, DOCX files, images and other content MUST be considered potentially hostile.

---

# 6. Trust Boundaries

## 6.1 Online System

The online system is exposed to potentially hostile Internet traffic.

All public endpoints MUST therefore be treated as untrusted entry points.

## 6.2 School LAN

The LAN MUST NOT automatically be considered trusted merely because it is private.

Any device connected to the LAN MUST authenticate before accessing protected functionality.

## 6.3 Local Server

The local server is a trusted application environment but MUST still enforce:

* authentication
* authorization
* input validation
* API protection
* audit logging
* service isolation

## 6.4 Client Applications

Browsers and client-side applications MUST be considered untrusted.

The server MUST NOT rely on:

* hidden fields
* JavaScript checks
* browser timers
* local role flags
* client-side validation
* local storage alone

for security decisions.

---

# 7. Authentication

## 7.1 Account Provisioning

There MUST be no public self-registration in V1.

Accounts MUST be provisioned by Super Admin.

Supported roles:

1. Super Admin / IT
2. Principal
3. Teacher
4. Parent
5. Student

## 7.2 Initial Credentials

Provisioned users MUST receive an initial credential or temporary password.

Initial passwords MUST:

* be generated securely
* not be predictable
* not be reused across accounts
* not be stored in plaintext

## 7.3 First Login

Users MUST be required to change their initial password on first successful login.

The temporary credential MUST become invalid after the required password-change process.

## 7.4 Password Storage

Passwords MUST NEVER be stored as plaintext.

Passwords MUST use a modern adaptive password hashing algorithm such as:

* Argon2id, or
* another approved memory-hard password hashing mechanism.

Password hashes MUST include unique salts.

Plaintext password recovery MUST NOT be possible.

## 7.5 Password Requirements

The password policy MUST enforce sufficient password strength without relying solely on arbitrary complexity rules.

The system SHOULD support:

* minimum password length
* breached/common-password detection where practical
* rejection of obvious weak passwords
* password history where justified for privileged accounts

## 7.6 Password Reset

Password reset operations MUST:

* require appropriate authorization
* generate secure temporary credentials or recovery tokens
* invalidate previous reset tokens
* expire recovery tokens
* log the security-sensitive operation

Super Admin password resets MUST be auditable.

---

# 8. Multi-Factor Authentication

MFA is **not required for V1**.

The architecture SHOULD nevertheless avoid preventing future MFA implementation.

Authentication components SHOULD therefore be designed so MFA can be introduced later without redesigning the entire identity system.

---

# 9. Session Security

Sessions MUST:

* use secure random identifiers
* expire according to configured policy
* be invalidated on logout
* be invalidated after appropriate credential-reset events
* use secure cookie attributes where cookie sessions are used

Where cookies are used, security attributes SHOULD include:

* `Secure`
* `HttpOnly`
* appropriate `SameSite`

Session identifiers MUST NOT be placed in URLs.

The system MUST protect against session fixation.

Privileged sessions SHOULD have stricter inactivity and absolute expiration policies.

---

# 10. Rate Limiting and Abuse Protection

Authentication and security-sensitive endpoints MUST be rate limited.

At minimum:

* login
* password reset
* account recovery
* administrative authentication
* sensitive API operations

The system SHOULD detect repeated failures and temporarily throttle abusive clients.

Rate limiting MUST not create an easy denial-of-service mechanism against legitimate users.

---

# 11. Account Locking

The system SHOULD avoid permanent automatic account locking caused solely by failed authentication attempts.

Instead it SHOULD use:

* progressive throttling
* temporary lockouts where appropriate
* security event logging
* administrative intervention for confirmed abuse

Super Admin MUST be able to deactivate an account immediately.

---

# 12. Authorization

Authentication answers:

> Who is this user?

Authorization answers:

> What is this user allowed to do?

The two MUST remain separate.

Every protected backend operation MUST perform authorization.

---

# 13. Role-Based Access Control

The system MUST implement RBAC for:

* Super Admin
* Principal
* Teacher
* Parent
* Student

Permissions MUST be represented explicitly.

The application MUST NOT rely only on role names scattered throughout frontend code.

---

# 14. Super Admin Security

Super Admin has the highest application privilege.

Super Admin functionality MUST include:

* user provisioning
* account management
* academic configuration
* teacher assignment
* promotion control
* system configuration
* synchronization administration
* operational monitoring

Because of this privilege level:

* all sensitive operations MUST be logged
* destructive actions MUST require explicit confirmation
* authorization MUST be enforced server-side
* admin APIs MUST NOT be exposed anonymously
* admin endpoints MUST be separated from ordinary user functionality where practical

Super Admin MUST NOT be able to silently bypass audit logging.

---

# 15. Principal Security

Principal access MUST be restricted to management and academic intelligence functionality assigned to the role.

Principal MUST NOT automatically inherit Super Admin privileges.

Principal MUST NOT be able to:

* modify system infrastructure configuration
* manage arbitrary user credentials
* change synchronization infrastructure
* alter security configuration

unless explicitly granted through a documented permission.

---

# 16. Teacher Authorization

Teachers MUST only access:

* classes assigned to them
* subjects assigned to them
* assessments they are authorized to manage
* question content within their permitted academic scope
* student academic information required for their teaching responsibilities

A teacher MUST NOT be able to access another teacher's unrestricted academic workspace merely by modifying an identifier in a request.

Authorization MUST validate ownership or assignment server-side.

---

# 17. Parent Authorization

Parent access MUST be determined exclusively from explicit parent-student relationships.

The system MUST NOT infer parent relationships from:

* names
* email addresses
* phone numbers
* class membership
* matching surnames

A parent MUST only access students explicitly linked to that parent account.

A parent MUST NOT be able to alter the relationship through client-side requests.

---

# 18. Student Authorization

Students MUST only access their own:

* profile
* academic progress
* practice history
* assessment attempts
* results
* permitted learning content
* AI tutoring context

Students MUST NOT access:

* other students' records
* teacher administration
* question-management interfaces
* answer keys before permitted release
* administrative APIs

---

# 19. API Security

All protected APIs MUST:

* authenticate requests
* authorize requested actions
* validate input
* validate resource ownership
* enforce request size limits
* return safe errors
* avoid exposing internal implementation details

APIs MUST NOT trust:

* client-provided role
* client-provided user ID
* client-provided class ownership
* client-provided permissions
* client-provided assessment status

---

# 20. Object-Level Authorization

Every request referencing a resource identifier MUST verify that the authenticated principal has permission to access that specific resource.

Examples include:

* student ID
* parent ID
* class ID
* subject ID
* question ID
* assessment ID
* attempt ID
* uploaded-file ID
* sync operation ID

Changing an identifier in a request MUST NOT allow access to another resource.

---

# 21. Input Validation

All external input MUST be validated.

Validation MUST occur server-side.

Inputs include:

* JSON
* form data
* query parameters
* path parameters
* uploaded files
* imported question data
* synchronization payloads
* AI prompts
* configuration values

Validation SHOULD use allowlists where practical.

---

# 22. Output Encoding

User-controlled content MUST be safely encoded according to its output context.

The system MUST protect against:

* stored XSS
* reflected XSS
* DOM-based XSS
* HTML injection

Rich academic content MUST use a controlled sanitization pipeline.

Raw HTML MUST NOT be blindly rendered.

---

# 23. Content Security Policy

The online and local web applications SHOULD implement a restrictive Content Security Policy.

CSP MUST be designed around the actual frontend architecture rather than copied blindly.

Unsafe inline scripts SHOULD be avoided.

Third-party scripts MUST be minimized and explicitly controlled.

---

# 24. CSRF Protection

If cookie-based authentication is used, state-changing requests MUST implement appropriate CSRF protection.

SameSite cookies alone MUST NOT automatically be treated as the only CSRF defense where stronger protection is required.

---

# 25. SQL Injection Protection

Database queries MUST use parameterized queries or safe ORM/query-builder mechanisms.

Dynamic SQL MUST NOT concatenate untrusted input.

Database credentials MUST not be embedded in source code.

---

# 26. File Upload Security

Uploaded files MUST be treated as untrusted.

Supported uploads include:

* PDF
* DOCX
* images

The system MUST:

* validate file type
* validate file size
* validate file signature where appropriate
* reject unexpected file formats
* generate controlled storage names
* prevent path traversal
* prevent executable content from being served as executable
* isolate uploaded files from application code
* scan files where appropriate
* log processing failures

Original filenames MUST NOT determine storage paths.

---

# 27. Content Processing Security

The online content-processing pipeline MUST be isolated from the main application as appropriate.

Processing services MUST NOT receive unrestricted database access.

The processing pipeline MUST treat:

* PDFs
* DOCX
* images
* OCR output
* extracted text
* extracted HTML
* embedded objects

as untrusted input.

Processing failures MUST be contained.

A malicious document MUST NOT be able to compromise the main application through normal processing.

---

# 28. Question Import Security

Imported questions MUST remain in a non-published state until reviewed and approved.

Processing status MUST NOT be interpreted as publication authorization.

Required lifecycle:

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
```

Only published questions may enter permitted assessment pools.

---

# 29. Academic Content Integrity

Question versions MUST be immutable once used by a completed assessment.

Editing a question MUST create a new version where required rather than silently changing historical assessment content.

Historical results MUST remain reproducible.

---

# 30. CBT Security

CBT is a high-integrity subsystem.

The local server MUST be authoritative for:

* attempt state
* timer state
* answer state
* question sequence
* submission state
* scoring state

The browser MUST NOT be authoritative.

---

# 31. CBT Timer Protection

The client-side timer MUST be treated as a display mechanism.

The server MUST maintain authoritative timing information.

A student MUST NOT gain additional assessment time by:

* changing system time
* refreshing the browser
* modifying JavaScript
* manipulating local storage
* disconnecting from the LAN

---

# 32. CBT Answer Integrity

Answers MUST be persisted server-side during an active attempt.

The system MUST support recovery after:

* browser crash
* device restart
* temporary LAN loss
* page refresh
* client application failure

The recovery process MUST prevent the student from creating an alternate conflicting attempt.

---

# 33. CBT Question Integrity

The selected question set and ordering MUST be persisted for the attempt.

If randomization is enabled, the resulting assignment MUST be recoverable.

A student reconnecting to an existing attempt MUST receive the same authoritative assessment state.

---

# 34. CBT Mode AI Restriction

Local AI MUST be unavailable during active CBT/exam mode.

AI MUST NOT:

* answer assessment questions
* provide hints during restricted exams
* modify answers
* influence scoring
* alter timer state

This restriction MUST be enforced server-side.

---

# 35. Synchronization Security

Synchronization occurs across a security boundary.

All synchronization traffic MUST use authenticated and encrypted communication.

The implementation MUST support:

* TLS
* authenticated sync clients
* credential rotation
* request validation
* replay protection
* idempotency
* revision tracking
* authorization
* audit logging

---

# 36. Sync Authentication

The local server MUST authenticate to the online synchronization service using dedicated machine/service credentials.

Ordinary user credentials MUST NOT be reused as machine synchronization credentials.

Sync credentials MUST be stored securely.

They MUST NOT appear in:

* logs
* source code
* configuration exports
* error messages
* UI screens

---

# 37. Sync Payload Validation

Synchronization payloads MUST be validated before application.

Validation MUST cover:

* schema
* entity type
* revision
* identifiers
* operation type
* authorization
* timestamps
* payload size
* expected state transitions

Malformed synchronization payloads MUST be rejected.

---

# 38. Replay Protection

Synchronization requests MUST not be reusable indefinitely.

The sync protocol MUST support mechanisms such as:

* request identifiers
* sequence/revision information
* timestamps where appropriate
* idempotency keys
* server-side duplicate detection

---

# 39. Conflict Security

Conflict resolution MUST never silently overwrite authoritative records.

Conflicts MUST be:

1. detected,
2. classified,
3. resolved according to entity-specific rules,
4. recorded.

Security-sensitive conflicts MUST fail closed and require controlled resolution.

---

# 40. Local Server Network Security

The local server MUST be private to the school LAN.

It MUST NOT be intentionally exposed to the public Internet.

The deployment MUST NOT require:

* public DNS
* public port forwarding
* direct Internet exposure
* publicly accessible administrative ports

The preferred access model is a local school hostname backed by controlled LAN DNS or equivalent local name resolution.

---

# 41. Firewall Requirements

The Windows host MUST use a firewall configuration that:

* permits only required services
* restricts access to the school LAN
* blocks unnecessary inbound connections
* prevents accidental Internet exposure
* limits administrative services

Firewall rules MUST be documented.

---

# 42. Network Binding

The local application MUST bind only to required interfaces.

Administrative or internal services MUST NOT automatically listen on every network interface.

Unused ports MUST remain closed.

---

# 43. Local HTTPS

HTTPS/TLS SHOULD be used for protected LAN traffic where deployment allows practical certificate management.

If TLS certificates are used:

* certificates MUST be validated
* private keys MUST be protected
* expiration MUST be monitored
* certificate replacement MUST be supported

The implementation MUST NOT disable certificate validation merely to make development easier.

---

# 44. Local Hostname

The school-local hostname MUST resolve only within the intended school network.

The local server MUST not be exposed through public DNS.

No Internet-facing port forwarding MUST be required.

---

# 45. Windows Host Hardening

The local server PC MUST be treated as production infrastructure.

Recommended baseline:

* supported Windows version
* current security updates
* automatic patching where operationally safe
* Windows Firewall enabled
* antivirus/endpoint protection enabled
* unnecessary services disabled
* least-privilege administrator accounts
* secure screen-lock policy
* controlled physical access
* secure boot where supported
* disk encryption where practical
* restricted remote administration

---

# 46. Local Server Service Isolation

The local server manager MUST not run every component with unrestricted operating-system privileges.

Services SHOULD use dedicated accounts or restricted execution contexts where practical.

The server manager MUST supervise:

* backend
* database
* sync worker
* AI service
* background jobs

without granting every component unrestricted host access.

---

# 47. Administrative Interface Security

The local server management dashboard MUST require authentication.

Sensitive management functions MUST NOT be accessible through an unauthenticated LAN endpoint.

Diagnostic endpoints MUST be reviewed individually.

Health checks MAY expose minimal non-sensitive availability information where required.

Detailed diagnostics MUST require authorization.

---

# 48. Endpoint Explorer Security

The local server endpoint explorer MUST:

* require authentication
* expose only approved endpoints
* hide secrets
* avoid arbitrary request execution
* prevent unrestricted access to destructive endpoints
* distinguish safe diagnostic operations from administrative operations

It MUST NOT become a generic internal API attack surface.

---

# 49. Secrets Management

Secrets MUST NOT be committed to source control.

Secrets include:

* database credentials
* API keys
* sync credentials
* signing keys
* encryption keys
* service credentials
* certificates/private keys

Secrets MUST be injected through secure configuration mechanisms appropriate to the deployment environment.

---

# 50. Secret Exposure Prevention

Secrets MUST NOT appear in:

* logs
* stack traces
* API responses
* screenshots generated by the application
* diagnostics
* exported configuration
* Git repositories

Configuration interfaces MUST mask secret values.

---

# 51. Database Security

## Online PostgreSQL

The online database MUST:

* require authentication
* restrict network access
* use dedicated application credentials
* avoid public exposure where possible
* use encrypted connections where supported
* use least-privilege database roles
* maintain backups

## Local SQLite

The local SQLite database MUST:

* be stored in a protected application-data directory
* use filesystem permissions restricting unauthorized access
* use WAL safely
* be included in backup strategy
* never be exposed directly through HTTP

---

# 52. Database Privilege Separation

The application database account MUST NOT automatically receive unnecessary administrative privileges.

Database administration credentials MUST be separate from application credentials.

Migrations MUST run through controlled deployment mechanisms.

---

# 53. Data Protection

Protected information includes:

* user identities
* credentials
* student academic data
* assessment attempts
* scores
* parent-student relationships
* teacher assignments
* audit information
* AI conversation data
* uploaded academic materials
* synchronization records

Access to this information MUST follow role and data-scope authorization.

---

# 54. Encryption at Rest

Sensitive backups SHOULD be encrypted.

Where the Windows deployment supports it, full-disk encryption SHOULD be enabled.

Sensitive stored secrets MUST use appropriate encryption or operating-system secret storage rather than plaintext files.

---

# 55. Backup Security

Backups MUST:

* be protected against unauthorized access
* be integrity-checked
* be retained according to documented policy
* be tested through restoration procedures
* avoid exposing credentials
* be stored separately from the primary database where practical

Backup files MUST not be publicly accessible.

---

# 56. Backup Restoration

A backup is not considered reliable until restoration has been tested.

The project MUST define:

* backup frequency
* retention
* restoration procedure
* recovery verification
* corruption detection
* responsible administrator

---

# 57. Audit Logging

Security-sensitive events MUST be auditable.

At minimum, audit events SHOULD include:

* login success
* login failure
* logout
* password changes
* password resets
* account creation
* account deactivation
* role changes
* permission changes
* teacher assignments
* parent-student relationship changes
* question approval
* question publication
* assessment creation
* assessment configuration changes
* assessment submission
* promotion execution
* synchronization events
* conflict resolution
* backup operations
* configuration changes
* administrative actions
* security failures

---

# 58. Audit Log Integrity

Audit logs MUST be protected against ordinary users modifying or deleting them.

The application MUST distinguish:

* operational logs
* security logs
* audit records

Audit records SHOULD contain:

* timestamp
* actor
* action
* target
* result
* source/device information where appropriate
* correlation/request ID
* relevant metadata

Sensitive values MUST NOT be logged unnecessarily.

---

# 59. Logging Security

Logs MUST NOT contain:

* passwords
* access tokens
* refresh tokens
* private keys
* full authentication secrets
* unnecessary personal information

Exception messages MUST be useful to operators without exposing internal security details to users.

---

# 60. AI Security

The local AI subsystem MUST be treated as an untrusted reasoning component.

AI MUST NOT have direct authority over:

* grades
* permissions
* users
* synchronization
* promotion
* assessment scoring
* publication
* security settings

AI access MUST occur through a controlled AI gateway.

---

# 61. AI Data Access

AI retrieval MUST enforce the requesting user's authorization.

A student MUST NOT retrieve:

* another student's data
* teacher-private information
* principal-private information
* administrative information

merely because the information exists in the local database or vector index.

---

# 62. AI Prompt Injection

Academic content MUST be treated as potentially adversarial input.

The AI architecture MUST reduce prompt-injection risk through:

* trusted system instructions
* content/source separation
* retrieval filtering
* permission-aware retrieval
* tool restrictions
* no direct database mutation
* explicit output handling

Retrieved content MUST NOT be automatically treated as an instruction to the AI system.

---

# 63. AI Conversation Storage

AI conversations MUST be stored efficiently.

The system SHOULD avoid repeatedly storing complete retrieved documents inside every message.

Conversation storage SHOULD use:

* message records
* references to content chunks
* model/version metadata
* timestamps
* compact retrieval metadata

This reduces storage duplication while preserving reproducibility and auditability.

---

# 64. AI Failure Isolation

If Ollama or another local AI service fails:

* student login MUST continue
* CBT MUST continue
* practice MUST continue
* scoring MUST continue
* synchronization MUST continue where otherwise available
* core academic functionality MUST continue

AI failure MUST NOT become a core-system failure.

---

# 65. Content Retrieval Security

Vector indexes and embeddings MUST follow the same authorization boundaries as their source content.

Deleting or unpublishing source content MUST trigger appropriate index invalidation or access control.

Draft/unapproved questions MUST NOT become retrievable as approved student content.

---

# 66. Dependency Security

Dependencies MUST be:

* tracked
* version controlled
* reviewed
* updated according to security risk
* scanned where practical

Known critical vulnerabilities MUST be investigated before release.

Unused dependencies SHOULD be removed.

---

# 67. Supply Chain Security

The project SHOULD use:

* lockfiles
* reproducible dependency resolution where practical
* trusted package registries
* dependency integrity verification
* controlled build environments

Build artifacts MUST be traceable to source revisions.

---

# 68. Secure Build and Release

Production builds MUST NOT contain:

* development credentials
* debug secrets
* test accounts
* verbose development diagnostics
* disabled security controls

Release artifacts SHOULD have identifiable versions.

---

# 69. Error Handling

Production errors MUST fail safely.

User-facing errors SHOULD provide useful information without revealing:

* SQL queries
* filesystem paths
* stack traces
* credentials
* internal service addresses
* cryptographic details

Detailed diagnostics belong in protected logs.

---

# 70. Security Monitoring

The system SHOULD monitor:

* repeated authentication failures
* unusual administrative activity
* repeated authorization failures
* sync failures
* unexpected service crashes
* suspicious file-processing failures
* abnormal API activity
* database failures
* backup failures

The local server manager MUST provide operational visibility into security-relevant failures.

---

# 71. Security Event Severity

Security events SHOULD be classified at minimum as:

* Informational
* Warning
* High
* Critical

Critical events SHOULD trigger immediate administrative attention.

---

# 72. Incident Response

The project MUST define an incident-response procedure covering:

1. Detection
2. Triage
3. Containment
4. Investigation
5. Eradication
6. Recovery
7. Verification
8. Post-incident review

At minimum, procedures MUST exist for:

* compromised administrator account
* compromised local server
* leaked credentials
* suspected database compromise
* malicious uploaded file
* synchronization compromise
* assessment integrity incident
* ransomware or host compromise

---

# 73. Account Compromise Response

When an account is suspected to be compromised, administrators MUST be able to:

* deactivate the account
* reset credentials
* invalidate sessions
* review audit activity
* restore access after verification

---

# 74. Local Server Compromise Response

If the local server is suspected to be compromised:

1. isolate the server from the network where appropriate,
2. preserve relevant logs,
3. stop affected services,
4. investigate the host,
5. rotate affected credentials,
6. restore from a trusted state if necessary,
7. validate database integrity,
8. re-establish synchronization,
9. verify the environment before returning it to service.

---

# 75. Assessment Integrity Incident

A suspected CBT integrity incident MUST preserve:

* attempt state
* server timestamps
* answer history
* relevant audit records
* synchronization state
* error logs

Historical assessment records MUST NOT be silently rewritten to hide an incident.

---

# 76. Penetration Testing

A formal penetration test MUST be completed before production acceptance.

The test MUST cover, at minimum:

### Online System

* authentication
* authorization
* RBAC
* API security
* object-level authorization
* session management
* injection
* XSS
* CSRF where applicable
* file uploads
* information disclosure
* rate limiting
* privilege escalation

### Local Server

* LAN exposure
* service enumeration
* authentication
* authorization
* API security
* management dashboard
* endpoint explorer
* configuration access
* filesystem exposure
* database exposure

### Synchronization

* authentication
* authorization
* replay
* tampering
* malformed payloads
* conflict abuse
* credential exposure

### CBT

* timer manipulation
* answer manipulation
* attempt manipulation
* question leakage
* unauthorized access
* recovery abuse
* client tampering

### AI

* prompt injection
* unauthorized retrieval
* data leakage
* tool abuse
* content-boundary bypass

---

# 77. Vulnerability Management

Discovered vulnerabilities MUST be classified by severity and tracked to resolution.

Critical vulnerabilities MUST block production release until appropriately remediated or formally accepted by an authorized project decision.

High-risk vulnerabilities SHOULD normally block production release.

Risk acceptance MUST be documented.

---

# 78. Security Testing

Security testing MUST occur throughout development.

Required categories include:

* unit security tests
* integration security tests
* API authorization tests
* authentication tests
* permission-boundary tests
* file-upload tests
* sync-security tests
* CBT integrity tests
* AI authorization tests
* dependency scans
* penetration testing

---

# 79. Authorization Test Matrix

Automated tests MUST verify that each role cannot access unauthorized functionality.

At minimum:

| Operation                          | Super Admin |         Principal |        Teacher |          Parent | Student |
| ---------------------------------- | ----------: | ----------------: | -------------: | --------------: | ------: |
| Manage users                       |         Yes |                No |             No |              No |      No |
| Manage system configuration        |         Yes |                No |             No |              No |      No |
| Manage promotion                   |         Yes |                No |             No |              No |      No |
| View management analytics          |         Yes |               Yes |         Scoped |    Child-scoped |    Self |
| Manage assigned assessments        |         Yes |      Configurable |            Yes |              No |      No |
| Manage questions                   |         Yes |      Configurable |         Scoped |              No |      No |
| View child records                 |         Yes | Appropriate scope | Assigned scope | Linked children |      No |
| Take practice                      |        Yes* |              Yes* |           Yes* |              No |     Yes |
| Take CBT                           |        Yes* |              Yes* |           Yes* |              No |     Yes |
| Access local server administration |         Yes |                No |             No |              No |      No |

`*` Any non-student access to assessment functionality MUST be explicitly defined by the implementation rather than assumed.

---

# 80. Data Boundary Tests

Automated tests MUST verify:

* Student A cannot read Student B data.
* Parent A cannot read Parent B's child data.
* Teacher A cannot access Teacher B's restricted class data.
* Teachers cannot access unassigned classes.
* Users cannot modify resource IDs to bypass authorization.
* AI retrieval respects the same boundaries.
* Synchronization cannot create unauthorized relationships.

---

# 81. Security Acceptance Criteria

The system MUST NOT be accepted for production unless:

* authentication is secure
* passwords are securely hashed
* authorization is enforced server-side
* role boundaries are tested
* object-level authorization is tested
* file uploads are protected
* local server exposure is restricted
* administrative endpoints are protected
* synchronization is authenticated and encrypted
* CBT state is server-authoritative
* AI cannot modify protected business data
* audit logging works
* backups are tested
* production secrets are protected
* dependency security checks are complete
* penetration testing has been performed
* identified critical security issues are resolved or formally accepted

---

# 82. Security Non-Negotiables

The following MUST NOT be violated:

1. No plaintext passwords.
2. No public self-registration.
3. No client-only authorization.
4. No public exposure of the local school server.
5. No unauthenticated administration endpoints.
6. No direct browser authority over CBT timing.
7. No AI authority over deterministic academic records.
8. No unapproved questions in published assessment pools.
9. No unrestricted parent-to-student access.
10. No unrestricted teacher-to-class access.
11. No secrets committed to source control.
12. No credentials in logs.
13. No silent destructive synchronization.
14. No silent historical-result mutation.
15. No production deployment without security verification.

---

# 83. Security Documentation

The following MUST remain documented:

* security architecture
* trust boundaries
* authentication design
* authorization matrix
* network topology
* firewall rules
* secret-management strategy
* backup strategy
* incident-response procedures
* penetration-test scope
* vulnerability register
* security decisions
* accepted security risks

---

# 84. Security Change Management

Security-sensitive architectural changes MUST be documented before implementation when they affect:

* authentication
* authorization
* trust boundaries
* encryption
* synchronization
* data ownership
* CBT integrity
* AI permissions
* administrative privileges
* network exposure

Significant changes SHOULD be recorded through architecture decision records.

---

# 85. Requirement Traceability

Security implementation MUST remain traceable to:

* `REQUIREMENTS.md`
* `ARCHITECTURE.md`
* `PROJECT_CONTEXT.md`
* module specifications
* testing strategy
* security testing results
* penetration-testing findings

Security requirements MUST NOT exist only in source code or developer assumptions.

---

# 86. Definition of Done — Security

A security-sensitive feature is complete only when:

1. Security requirements are documented.
2. Trust boundaries are identified.
3. Authentication requirements are implemented where applicable.
4. Authorization rules are implemented.
5. Negative authorization tests exist.
6. Input validation exists.
7. Error handling is safe.
8. Audit requirements are satisfied.
9. Security-sensitive logs are reviewed.
10. Relevant automated tests pass.
11. Regression tests pass.
12. Documentation is updated.
13. No known critical vulnerability remains unresolved.

---

# 87. Final Security Principle

CRC MUST be designed under the assumption that:

* clients can be modified,
* browsers can be manipulated,
* LAN devices can be compromised,
* uploaded documents can be malicious,
* credentials can be stolen,
* APIs can be probed,
* AI can produce incorrect or adversarial output,
* synchronization can encounter conflicting or malicious data.

Therefore, security MUST be enforced at the system boundaries and authoritative services rather than delegated to client behavior.

The local school environment MUST remain private and hardened.

The online system MUST remain securely isolated from the local environment.

Academic records MUST remain authoritative and auditable.

Assessment integrity MUST remain server-controlled.

AI MUST remain an assistive subsystem.

Administrative actions MUST remain traceable.

Security MUST be continuously tested rather than assumed.

**Security is a release requirement, not a post-release feature.**
