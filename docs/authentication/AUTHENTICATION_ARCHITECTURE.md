# Authentication Architecture

## 1. Purpose

This document defines the authentication architecture for the CRC LMS deployed at Christian Royal College.

It establishes how users are:

* identified;
* provisioned;
* authenticated;
* granted and denied access;
* required to change initial credentials;
* maintained across online and offline environments;
* signed in and signed out;
* recovered after credential or session problems;
* synchronized between the online master system and the school local server;
* audited for security-relevant authentication activity.

This document is an implementation-level architectural specification.

It must be read together with:

* `PROJECT_CONTEXT.md`
* `REQUIREMENTS.md`
* `ARCHITECTURE.md`
* `SECURITY_REQUIREMENTS.md`
* `docs/database/DATABASE_ARCHITECTURE.md`
* `docs/api/API_ARCHITECTURE.md`

Where a lower-level implementation decision conflicts with this document, the documented architecture and approved ADRs take precedence.

---

# 2. Authentication Objectives

The authentication system MUST provide:

1. secure identification of every user;
2. strict account provisioning;
3. secure first-login handling;
4. secure password storage;
5. secure session management;
6. account-state enforcement;
7. role-aware access control;
8. object-level authorization integration;
9. offline authentication for supported school LAN workflows;
10. secure synchronization of identity state;
11. credential revocation;
12. authentication event auditing;
13. brute-force protection;
14. session invalidation after security-sensitive changes;
15. predictable failure behavior;
16. recovery mechanisms that do not bypass security controls.

Authentication MUST NOT be treated as authorization.

A successfully authenticated user MUST still be checked against:

* role;
* permissions;
* account state;
* resource ownership;
* teacher assignment scope;
* parent-student relationship;
* academic context;
* operation-specific policy.

---

# 3. Authentication Model

CRC uses a centralized identity model with two operational authentication environments:

```text
                         ONLINE MASTER
                              │
                    Authoritative Identity
                              │
                       Secure Identity
                       Synchronization
                              │
                              ▼
                    SCHOOL LOCAL SERVER
                              │
                  Local Identity Replica
                              │
                         SCHOOL LAN
                    ┌─────────┼─────────┐
                    ▼         ▼         ▼
                 Student    Teacher    Admin
                  Device     Device    Device
```

The online system is the authoritative source for:

* user identity;
* account lifecycle;
* role assignment;
* global account status;
* provisioning;
* administrative identity management.

The local server maintains the minimum secure identity state required to support approved offline school operations.

The local server MUST NOT become an independent identity universe.

---

# 4. Supported Roles

V1 supports exactly five application roles:

| Role             | Primary Authentication Context                                        |
| ---------------- | --------------------------------------------------------------------- |
| Super Admin / IT | Online and local administration                                       |
| Principal        | Online management dashboard and authorized local management workflows |
| Teacher          | Online dashboard and local academic workflows                         |
| Parent           | Online parent dashboard                                               |
| Student          | Online dashboard and local academic workflows                         |

No public self-registration exists in V1.

Users MUST be provisioned by the Super Admin.

---

# 5. Identity Principles

The identity architecture follows these principles:

### 5.1 One User Identity

A person MUST have one logical CRC user identity.

Separate accounts MUST NOT be created merely because the user accesses:

* online services;
* the local server;
* student workflows;
* teacher workflows;
* management dashboards.

The same identity record governs all supported environments.

### 5.2 Stable Global Identifier

Every user MUST have a globally unique immutable identifier.

Recommended implementation:

```text
UUID
```

The identifier MUST NOT depend on:

* username;
* email address;
* class;
* admission number;
* role;
* display name.

Identifiers MUST remain stable when user information changes.

### 5.3 Human-Readable Login Identifier

The system SHOULD use a stable login identifier appropriate to the school environment.

Examples may include:

```text
student ID
staff ID
admin username
```

The exact identifier format is configuration and implementation detail.

Login identifiers MUST be unique within the relevant identity namespace.

The system MUST NOT rely on a person's display name as an authentication identifier.

---

# 6. Account Provisioning

All V1 user accounts are provisioned by the Super Admin.

Provisioning MUST create:

1. identity record;
2. role assignment;
3. required profile record;
4. account state;
5. initial credential state;
6. audit record;
7. synchronization metadata.

Provisioning MUST NOT automatically grant unrelated permissions.

Example:

```text
Create Teacher
      │
      ├── User Identity
      ├── Teacher Profile
      ├── Teacher Role
      ├── Initial Credential State
      └── Audit Event
```

Teacher access to classes and subjects is controlled separately through teacher assignments.

Creating a teacher account MUST NOT automatically grant access to every class or subject.

---

# 7. Provisioning Workflow

The standard provisioning lifecycle is:

```text
Unprovisioned
      │
      ▼
Account Created
      │
      ▼
Initial Credential Assigned
      │
      ▼
Provisioned
      │
      ▼
First Login Required
      │
      ▼
Password Changed
      │
      ▼
Active
```

An account MAY instead transition to:

```text
Suspended
Disabled
Locked
Archived
```

depending on the reason for restricted access.

Every administrative account-state transition MUST be audited.

---

# 8. Account States

The system MUST support explicit account states.

Recommended states:

```text
PROVISIONED
ACTIVE
SUSPENDED
LOCKED
DISABLED
ARCHIVED
```

## 8.1 PROVISIONED

The account exists and has been provisioned but the user has not completed required first-login credential setup.

The user MAY authenticate only as permitted by the first-login flow.

## 8.2 ACTIVE

Normal authentication and authorization are permitted.

## 8.3 SUSPENDED

Authentication is denied.

Suspension is an administrative security or operational control.

## 8.4 LOCKED

Authentication is temporarily or administratively blocked.

This state may be triggered by:

* repeated failed authentication attempts;
* administrative action;
* security response.

## 8.5 DISABLED

The account is permanently or indefinitely prevented from normal authentication.

## 8.6 ARCHIVED

The account is retained for historical/audit purposes but is no longer an operational account.

Archived accounts MUST NOT authenticate.

---

# 9. Account-State Enforcement

Account state MUST be checked during authentication.

The system MUST NOT authenticate a disabled, suspended, or archived account merely because the password is correct.

Authorization middleware MUST also be capable of re-checking account state where required by the security model.

A cached authentication session MUST NOT indefinitely bypass account-state changes.

---

# 10. Initial Password

Super Admin provisioning creates an initial credential state.

The initial password MUST:

* be randomly generated or securely established;
* never be stored in plaintext;
* never be written to application logs;
* never be included in ordinary audit events;
* be transmitted only through approved secure mechanisms;
* require a password change on first successful login.

The system MUST NOT use a universal default password.

The system MUST NOT create predictable passwords based on:

* student name;
* admission number;
* date of birth;
* username;
* class;
* school name.

---

# 11. First Login

Users provisioned with an initial credential MUST be marked:

```text
must_change_password = true
```

The first-login flow is:

```text
Login
  │
  ▼
Credential Verification
  │
  ▼
Account State Check
  │
  ▼
First-Login Requirement
  │
  ▼
Force Password Change
  │
  ▼
Invalidate Initial Credential State
  │
  ▼
Create Normal Session
```

A user MUST NOT gain normal application access before completing the required password change.

The first-login password change MUST use the same password-security controls as normal password changes.

---

# 12. Password Policy

V1 uses password-based authentication.

Passwords MUST meet the security policy defined by the security requirements.

The implementation MUST prioritize:

* sufficient password length;
* resistance to credential stuffing;
* resistance to brute-force attacks;
* password hashing using a modern password hashing algorithm;
* rate limiting;
* secure password reset;
* prevention of known-compromised passwords where practical.

The recommended password hashing algorithm is:

```text
Argon2id
```

with parameters selected according to the production hardware and documented security benchmark.

The exact Argon2id parameters MUST be established during implementation and performance testing rather than hardcoded arbitrarily.

Passwords MUST NEVER be stored using:

* plaintext;
* reversible encryption;
* MD5;
* SHA-1;
* unsalted SHA-256;
* unsalted SHA-512;
* other general-purpose hashes used as password hashes.

---

# 13. Password Hashing

A password verifier MUST include an independent salt.

The stored password representation MUST contain all parameters necessary for verification and future rehashing.

Conceptually:

```text
Password
   │
   ▼
Argon2id
   │
   ▼
Password Verifier
   │
   ▼
Credential Store
```

The plaintext password MUST exist only in protected application memory for the minimum period required for authentication or password change.

It MUST NOT be:

* logged;
* persisted;
* sent to analytics;
* stored in audit records;
* included in error messages.

---

# 14. Password Rehashing

The authentication system SHOULD support transparent password rehashing.

When a user successfully authenticates using an older acceptable hash configuration:

```text
Verify Existing Hash
        │
        ▼
Hash Configuration Outdated?
        │
       YES
        │
        ▼
Rehash Password
        │
        ▼
Store New Verifier
```

The user should not be required to perform a separate migration action.

---

# 15. Password Changes

A password change MUST:

1. verify the authenticated user's authority to change the password;
2. validate the new password;
3. securely hash the new password;
4. replace the previous credential verifier;
5. increment the credential revision;
6. invalidate appropriate existing sessions;
7. generate an audit event;
8. synchronize the credential-state change where applicable.

Changing a password MUST NOT change the user's:

* role;
* profile;
* academic placement;
* assignments;
* parent relationships;
* historical records.

---

# 16. Password Reset

Password reset is an administrative operation in V1.

The Super Admin MAY reset a user's credentials.

A reset MUST:

* invalidate existing sessions;
* invalidate the previous credential;
* generate a new temporary credential state;
* require password change;
* create an audit event.

A reset MUST NOT silently reveal the user's previous password.

The Super Admin MUST NOT be able to retrieve an existing password.

---

# 17. Self-Service Password Recovery

V1 does not require public email-based password recovery.

This is intentional because:

* accounts are provisioned by the school;
* the system is intended for controlled school environments;
* not every account necessarily has a verified email address;
* local offline operation must remain possible.

If self-service recovery is introduced later, it MUST be specified as a separate security architecture and MUST NOT bypass the existing identity controls.

---

# 18. Authentication Flow

The normal authentication flow is:

```text
Client
  │
  │ Login Identifier + Password
  ▼
Authentication API
  │
  ├── Validate Request
  ├── Rate Limit Check
  ├── Account Lookup
  ├── Credential Verification
  ├── Account State Check
  ├── Credential Revision Check
  └── Authentication Event
          │
          ▼
      Session Creation
          │
          ▼
      Authenticated Client
```

Authentication failures MUST return generic failure responses.

The system MUST NOT disclose whether:

* a username exists;
* a user is a particular role;
* a password was almost correct;
* an account exists but is disabled;
* another user has a matching identifier.

Detailed internal failure reasons may be logged securely for authorized operational use.

---

# 19. Login Rate Limiting

Authentication endpoints MUST be rate limited.

Rate limiting SHOULD consider:

* account identifier;
* source address;
* local device where appropriate;
* authentication environment;
* repeated failures.

The system MUST protect against:

* brute-force attacks;
* credential stuffing;
* rapid automated login attempts;
* password spraying.

Rate limiting MUST avoid creating an easy denial-of-service mechanism against legitimate users.

---

# 20. Lockout Strategy

The system SHOULD use progressive authentication protection rather than a simplistic permanent lockout.

Possible controls include:

```text
Repeated failures
      │
      ▼
Increasing delay
      │
      ▼
Temporary lock
      │
      ▼
Administrative recovery when required
```

The exact thresholds MUST be configurable and documented during implementation.

Administrative accounts SHOULD receive stricter protection.

---

# 21. Session Architecture

After successful authentication, the system MUST create a secure authenticated session.

The implementation SHOULD use secure, short-lived access credentials with controlled renewal rather than long-lived bearer credentials that cannot be revoked.

Where cookies are used, authentication cookies MUST use appropriate:

* `Secure`;
* `HttpOnly`;
* `SameSite`

attributes.

Session identifiers MUST be:

* cryptographically random;
* non-predictable;
* sufficiently long;
* unique;
* invalidated on logout.

---

# 22. Session Binding

Sessions MUST be associated with:

* user identity;
* session identifier;
* authentication environment;
* issued timestamp;
* expiry;
* credential revision;
* relevant security metadata.

The system SHOULD record sufficient metadata to detect suspicious session behavior without unnecessarily collecting sensitive device information.

---

# 23. Session Expiration

Sessions MUST expire according to configured security policy.

Different session classes MAY have different lifetimes.

For example:

* ordinary student sessions;
* teacher sessions;
* parent sessions;
* principal sessions;
* Super Admin sessions.

Long-lived unattended administrative sessions MUST NOT be permitted by default.

---

# 24. Session Revocation

Sessions MUST be revocable.

A session MUST be invalidated when appropriate after:

* password reset;
* credential change;
* account disablement;
* account suspension;
* security incident;
* administrative session termination.

Credential revisioning SHOULD be used as an efficient mechanism for invalidating sessions created under older credentials.

---

# 25. Logout

Logout MUST invalidate the current session.

The client SHOULD also clear:

* local authentication state;
* cached session information;
* temporary authentication tokens.

Logout MUST NOT be treated as merely a client-side navigation event.

---

# 26. Authorization Boundary

Authentication answers:

> Who is this user?

Authorization answers:

> What is this authenticated user allowed to do?

The system MUST keep these responsibilities separate.

Example:

```text
Teacher successfully authenticates
              │
              ▼
      Teacher role verified
              │
              ▼
   Teacher assignment checked
              │
              ▼
     Requested class checked
              │
              ▼
       Operation permitted
```

A valid session MUST NOT automatically grant access to all teacher resources.

---

# 27. Role-Based Access Control

The application uses role-based access control.

V1 roles:

```text
SUPER_ADMIN
PRINCIPAL
TEACHER
PARENT
STUDENT
```

Permissions MUST be derived from role and contextual authorization rules.

The system SHOULD avoid scattering hardcoded role checks throughout application code.

Authorization should use centralized policy services or policy functions.

---

# 28. Super Admin Authorization

Super Admin has the highest application-level administrative authority.

Super Admin may:

* provision users;
* modify user accounts;
* deactivate accounts;
* reset credentials;
* assign roles;
* manage academic configuration;
* manage teacher assignments;
* manage promotion;
* inspect system health;
* inspect synchronization state;
* manage relevant configuration.

Super Admin access MUST still be subject to:

* authentication;
* session controls;
* audit logging;
* server-side authorization;
* security controls.

The application MUST NOT treat Super Admin as an implicit bypass of all security mechanisms.

---

# 29. Principal Authorization

The Principal has management-level access appropriate to school academic operations.

Principal authorization MUST be explicitly defined by permission rather than implemented as:

```text
role != student
```

The Principal MUST NOT automatically inherit Super Admin capabilities.

Examples of potentially permitted management functions include:

* school performance dashboards;
* class performance;
* academic analytics;
* learning-gap information;
* management reports.

Administrative infrastructure operations remain restricted to Super Admin unless explicitly authorized.

---

# 30. Teacher Authorization

Teacher access is constrained by assignment.

A teacher MAY access academic resources only when:

```text
Teacher
+
Assigned Subject
+
Assigned Class/Arm
+
Permitted Operation
```

are all satisfied.

Teacher authentication alone MUST NOT expose:

* unrelated classes;
* unrelated subjects;
* another teacher's private resources;
* administrative settings;
* student records outside authorized scope.

---

# 31. Parent Authorization

Parent authorization is based on the explicit parent-student relationship.

A parent MAY access information only for students explicitly linked to that parent account.

The system MUST NOT infer parent relationships from:

* surname;
* phone number;
* email;
* address;
* class;
* household information.

The authoritative relationship is the explicit database relationship.

V1 requires:

```text
One Student → One Parent Account
One Parent Account → Multiple Students
```

---

# 32. Student Authorization

Students may access resources associated with:

* their own identity;
* their academic placement;
* their permitted subjects;
* their assigned assessments;
* their own practice history;
* their own progress;
* authorized learning content.

A student MUST NOT access another student's:

* answers;
* attempts;
* results;
* progress;
* profile;
* private AI conversation;
* academic records.

---

# 33. Object-Level Authorization

Every protected resource MUST perform object-level authorization where applicable.

For example:

```text
GET /api/v1/students/{studentId}/progress
```

MUST verify that the authenticated user is authorized to access that exact student.

Checking only:

```text
authenticated == true
```

is insufficient.

Likewise, checking only:

```text
role == TEACHER
```

is insufficient for teacher-scoped resources.

---

# 34. Teacher Assignment Authorization

Teacher authorization MUST reference the assignment domain.

Conceptually:

```text
Teacher
  │
  ├── Subject Assignment
  │       │
  │       └── Subject
  │
  └── Class/Arm Assignment
          │
          └── Class/Arm
```

Authorization checks MUST verify the requested resource against the active assignment.

Assignment changes MUST take effect according to the synchronization and authorization consistency rules.

---

# 35. Parent-Student Authorization

Parent access MUST evaluate the explicit relationship table.

Conceptually:

```text
Parent Account
      │
      ▼
Parent-Student Relationship
      │
      ▼
Student
      │
      ▼
Authorized Academic Data
```

Deleting or disabling the relationship MUST immediately prevent new access once the relevant environment receives the change.

---

# 36. Offline Authentication

Offline operation is a core requirement.

When internet connectivity is unavailable, authorized users MUST still be able to authenticate for supported local workflows.

The local server therefore maintains a secure local identity replica.

```text
ONLINE IDENTITY
      │
      │ Secure Sync
      ▼
LOCAL IDENTITY REPLICA
      │
      ▼
LAN Authentication
```

The local identity replica MUST contain only the information required for local operation.

---

# 37. Offline Credential Architecture

Plaintext passwords MUST NEVER be synchronized.

The architecture uses environment-specific credential verifier records.

Conceptually:

```text
                    User Password
                         │
                ┌────────┴────────┐
                ▼                 ▼
       Online Credential     Local Credential
          Verifier              Verifier
                │                 │
                ▼                 ▼
        Online Authentication   Offline Authentication
```

Credential verifier records are highly sensitive authentication material.

They MUST:

* be securely generated;
* be encrypted in transit;
* be protected at rest;
* never be logged;
* never be exposed through ordinary APIs;
* be versioned;
* support revocation.

The exact credential-verifier synchronization mechanism MUST be implemented using a secure authenticated synchronization channel.

---

# 38. Offline Account State

The local server maintains the latest synchronized account state.

For offline authentication:

```text
Local Account Exists?
       │
       ▼
Account Active Locally?
       │
       ▼
Credential Valid?
       │
       ▼
Local Session Created
```

The local server MUST reject accounts whose last synchronized state is:

* suspended;
* disabled;
* archived;
* locked.

---

# 39. Offline Revocation Limitation

Offline systems have an unavoidable consistency boundary.

If an account is disabled in the online master while the school server has no connection, the local server cannot know about the change until synchronization occurs.

Therefore:

```text
Online Revocation
      │
      ▼
Pending Synchronization
      │
      ▼
Local Revocation
```

The architecture MUST explicitly track revocation propagation.

The system MUST NOT falsely claim that online administrative changes can instantly revoke offline access while the school server is completely disconnected.

Where necessary, the Super Admin may disable an account directly on the local server.

---

# 40. Local Emergency Account Controls

The local server MUST provide authorized administrative controls for urgent local access management.

These controls SHOULD allow authorized Super Admin operations such as:

* locally disable an account;
* terminate active local sessions;
* inspect local authentication status;
* force local credential reset;
* restore permitted access after administrative action.

Local emergency changes MUST be recorded and synchronized back to the online system.

---

# 41. Online/Local Identity Synchronization

Identity synchronization MUST be revision-based.

Each identity-sensitive record SHOULD have:

* global identifier;
* revision number;
* updated timestamp;
* origin;
* synchronization metadata.

Changes are propagated through the synchronization system rather than through direct database copying.

Example:

```text
Online Identity Change
        │
        ▼
Revision Created
        │
        ▼
Sync Queue
        │
        ▼
Local Server
        │
        ▼
Identity Revision Applied
```

The reverse direction applies to approved local-origin changes.

---

# 42. Credential Synchronization

Credential changes are security-sensitive synchronization operations.

A credential update MUST include:

* user identifier;
* credential revision;
* credential-verifier payload;
* origin environment;
* operation identifier;
* integrity/authentication metadata.

Credential payloads MUST NOT be included in:

* ordinary application APIs;
* analytics;
* debug logs;
* audit event details;
* client-side application state.

The synchronization subsystem MUST apply additional protections to credential operations.

---

# 43. Credential Conflict Resolution

Credential changes require special handling.

If online and local environments both modify a user's credential before synchronization:

```text
Online Credential Revision
          +
Local Credential Revision
          │
          ▼
Credential Conflict
```

The system MUST NOT silently choose one password based solely on timestamp.

Credential conflicts MUST enter an explicit security-sensitive resolution workflow.

The selected resolution strategy MUST ensure that:

* no credential is accidentally exposed;
* stale credentials cannot remain valid indefinitely;
* all affected sessions can be invalidated;
* the final credential revision is unambiguous.

---

# 44. Session Environment

Sessions are environment-specific.

A user may have:

```text
Online Session
Local Session
```

at the same time.

Logging out of one environment SHOULD NOT require the other environment to be online before the current session can be terminated.

However, security-sensitive global revocation SHOULD propagate when connectivity is available.

---

# 45. Local CBT Authentication

Before entering a CBT attempt, the local server MUST authenticate the user.

The authentication chain is:

```text
Student Login
     │
     ▼
Local Identity Verification
     │
     ▼
Account State Check
     │
     ▼
Student Authorization
     │
     ▼
Assessment Authorization
     │
     ▼
Attempt Creation
```

Authentication alone MUST NOT permit a student to access arbitrary assessments.

---

# 46. CBT Session Security

The CBT session MUST be bound to:

* authenticated student;
* active attempt;
* local server;
* assessment;
* attempt identifier;
* credential/session context.

The active attempt remains authoritative on the local server.

Browser state MUST NOT be the authoritative source of:

* remaining time;
* score;
* submission status;
* answer history.

---

# 47. Authentication During CBT Recovery

If a student browser crashes or disconnects:

```text
Student Device
      │
      X
   Connection Lost
      │
      ▼
Local Server Retains Attempt
      │
      ▼
Student Reconnects
      │
      ▼
Authentication / Session Recovery
      │
      ▼
Attempt Revalidated
      │
      ▼
Assessment Resumes
```

Recovery MUST preserve the authoritative server-side attempt state.

A student MUST NOT be able to use recovery mechanisms to access another student's attempt.

---

# 48. Device Trust

The local server MUST NOT treat an arbitrary LAN device as trusted merely because it is connected to the school network.

Authentication remains mandatory.

The local network is a controlled environment, not an inherently trusted security boundary.

---

# 49. Local Network Security

Authentication traffic on the school LAN SHOULD use HTTPS/TLS where technically practical.

The local server MUST:

* bind only to the intended LAN interface;
* avoid unnecessary network exposure;
* use Windows firewall rules;
* avoid public port forwarding;
* expose only required services;
* restrict administrative endpoints;
* maintain secure server configuration.

The local hostname should be private to the school network.

---

# 50. Authentication API

Authentication endpoints MUST follow the API architecture defined in:

`docs/api/API_ARCHITECTURE.md`

Typical endpoints include:

```text
POST /api/v1/auth/login
POST /api/v1/auth/logout
POST /api/v1/auth/change-password
POST /api/v1/auth/reset-password
GET  /api/v1/auth/session
POST /api/v1/auth/refresh
```

Exact endpoint naming MAY be refined during API implementation, but all authentication endpoints MUST follow the established API conventions.

---

# 51. Authentication Error Handling

Authentication errors MUST use stable machine-readable error codes.

Examples:

```text
AUTH_INVALID_CREDENTIALS
AUTH_ACCOUNT_DISABLED
AUTH_ACCOUNT_SUSPENDED
AUTH_ACCOUNT_LOCKED
AUTH_PASSWORD_CHANGE_REQUIRED
AUTH_SESSION_EXPIRED
AUTH_SESSION_REVOKED
AUTH_RATE_LIMITED
AUTH_INVALID_REQUEST
```

Responses MUST avoid leaking sensitive account information.

---

# 52. Authentication Audit Events

Authentication events MUST be auditable.

Relevant events include:

* login success;
* login failure;
* logout;
* password change;
* password reset;
* first login;
* account creation;
* account activation;
* account suspension;
* account disablement;
* account lock;
* account unlock;
* session revocation;
* credential revision;
* authentication anomaly;
* local emergency disable;
* credential synchronization failure.

Each event SHOULD include:

* event ID;
* timestamp;
* user ID where known;
* actor ID where applicable;
* environment;
* operation;
* result;
* correlation ID;
* source metadata appropriate to the security policy.

Passwords and credential verifier contents MUST NOT appear in audit records.

---

# 53. Authentication Logging

Application logs and audit records have different purposes.

### Application logs

Used for:

* diagnostics;
* operational troubleshooting;
* performance analysis;
* failure investigation.

### Audit records

Used for:

* security investigation;
* accountability;
* administrative traceability;
* compliance evidence.

Authentication events SHOULD be represented in both where appropriate, but sensitive credential information MUST be excluded from both.

---

# 54. Administrative Credential Operations

Super Admin credential operations are security-sensitive.

The system MUST audit:

```text
Who performed the operation
        │
        ▼
Which account was affected
        │
        ▼
What operation occurred
        │
        ▼
When it occurred
        │
        ▼
From which environment
```

A Super Admin resetting another user's password MUST NOT be able to retrieve the old password.

---

# 55. Authentication and Data Privacy

The authentication subsystem SHOULD minimize personal information.

It does not need to store arbitrary profile information merely for authentication.

Authentication data should be limited to:

* identity identifiers;
* credential verifier metadata;
* account state;
* session metadata;
* security events;
* required recovery/security information.

Academic information belongs to academic domains rather than the authentication subsystem.

---

# 56. Authentication and Database Boundaries

Authentication data MUST be separated logically from unrelated academic data.

The database model should distinguish:

```text
Identity
Credentials
Sessions
Roles
Permissions
Security Events
```

from:

```text
Student Academic Records
Teacher Assignments
Questions
Assessments
Results
Analytics
```

This separation reduces accidental authorization coupling.

---

# 57. Account Deactivation

Deactivation MUST be a controlled lifecycle operation.

When an account is disabled:

1. account state changes;
2. active sessions are revoked;
3. future authentication is rejected;
4. local identity state is updated through synchronization;
5. security event is recorded;
6. historical academic records remain intact.

Disabling a student MUST NOT delete the student's historical:

* results;
* attempts;
* progress;
* promotion history;
* audit records.

---

# 58. Account Deletion

Hard deletion of operational user identities SHOULD generally be avoided.

Where a user must cease operational access, the preferred approach is:

```text
Disable / Archive
```

rather than deleting identity records that are referenced by historical academic or audit data.

Any permanent deletion capability MUST be separately specified with explicit retention and privacy rules.

---

# 59. Role Changes

Role changes MUST be explicit administrative operations.

Example:

```text
Teacher
   │
   ▼
Role Change
   │
   ▼
Principal
```

A role change MUST:

* update authorization state;
* invalidate affected sessions where necessary;
* update local identity state through synchronization;
* preserve historical audit records;
* preserve relevant academic history.

Role changes MUST NOT silently duplicate the user identity.

---

# 60. Role and Assignment Separation

Role and assignment are distinct concepts.

For example:

```text
Teacher Role
      ≠
Teacher Assignment
```

A teacher may have the role:

```text
TEACHER
```

but only be assigned:

```text
JSS2A Mathematics
JSS3A Mathematics
```

Authorization must consider both.

---

# 61. Authentication and Promotion

Student promotion MUST NOT change authentication identity.

When a student moves from:

```text
JSS2 → JSS3
```

the same user identity remains.

Only academic placement changes.

Therefore:

* user ID remains unchanged;
* credentials remain unchanged;
* historical results remain associated with the same user;
* academic placement is updated;
* authorization is recalculated against the new placement.

---

# 62. Authentication and Parent Relationships

Changing a student's parent relationship MUST NOT create a new student identity.

The explicit parent-student relationship is changed independently.

Authentication identities remain stable.

---

# 63. Authentication and Synchronization Failures

If identity synchronization fails:

* the local server MUST continue using its last valid synchronized identity state for approved offline operation;
* failed synchronization MUST be visible to Super Admin;
* the failure MUST be logged;
* retry MUST be automatic where safe;
* security-sensitive changes MUST receive appropriate priority.

The system MUST NOT silently discard identity changes.

---

# 64. Authentication and Clock Integrity

Authentication and session management depend on reliable time.

The local server SHOULD maintain a reliable system clock.

Where practical, synchronization should detect significant clock drift.

The system MUST NOT rely exclusively on client-provided time for:

* session expiry;
* password-reset expiry;
* CBT timers;
* authentication event timestamps.

Server-side time is authoritative.

---

# 65. Authentication Security Threats

The implementation MUST explicitly defend against:

* credential stuffing;
* brute-force login attempts;
* password spraying;
* session fixation;
* session hijacking;
* token theft;
* insecure logout;
* privilege escalation;
* account enumeration;
* object-level authorization bypass;
* stolen local credentials;
* malicious LAN devices;
* replay of authentication requests;
* replay of credential synchronization;
* credential synchronization tampering;
* stale local authorization;
* compromised local server;
* compromised administrator credentials.

---

# 66. Compromised Local Server Consideration

The local server is a high-value trust boundary.

If compromised, an attacker may potentially access:

* local academic data;
* local identity replicas;
* local authentication material;
* assessment data;
* synchronization credentials.

Therefore the deployment MUST include:

* Windows hardening;
* least-privilege service accounts where practical;
* firewall restrictions;
* encrypted secrets;
* protected backups;
* secure updates;
* audit logging;
* local server monitoring;
* penetration testing;
* recovery procedures.

The architecture MUST NOT assume that physical access to the school server is harmless.

---

# 67. Secret Management

Authentication secrets MUST NOT be hardcoded.

This includes:

* session signing secrets;
* API credentials;
* synchronization credentials;
* encryption keys;
* service credentials.

Secrets MUST be stored using an appropriate protected mechanism for the Windows deployment environment.

Secrets MUST NOT be committed to source control.

---

# 68. Credential Rotation

The architecture SHOULD support rotation of:

* synchronization credentials;
* signing keys;
* encryption keys;
* service credentials.

Rotation MUST be designed so that valid production sessions and synchronization operations can be migrated safely.

---

# 69. Future MFA Readiness

MFA is **not required for V1**.

However, the authentication architecture MUST avoid making future MFA impossible.

The authentication pipeline should therefore conceptually support:

```text
Primary Authentication
        │
        ▼
Additional Authentication Factor
        │
        ▼
Session Creation
```

Future MFA MAY include methods such as authenticator applications or hardware-backed mechanisms, subject to a separate security and product decision.

V1 MUST NOT pretend to implement MFA when it does not.

---

# 70. Authentication Testing

Authentication MUST have dedicated automated tests.

## 70.1 Unit Tests

At minimum:

* password hashing;
* password verification;
* password policy;
* credential revisioning;
* account-state evaluation;
* session expiration;
* session revocation;
* authorization policy helpers;
* login throttling;
* role evaluation.

## 70.2 Integration Tests

At minimum:

* login;
* logout;
* password change;
* password reset;
* first-login flow;
* account disablement;
* role changes;
* teacher assignment authorization;
* parent-student authorization;
* local identity synchronization;
* credential synchronization;
* session revocation;
* offline authentication.

## 70.3 Security Tests

At minimum:

* brute-force resistance;
* account enumeration;
* session fixation;
* session replay;
* privilege escalation;
* object-level authorization;
* broken access control;
* CSRF where applicable;
* token/session theft scenarios;
* malicious credential synchronization;
* replayed sync messages;
* stale authorization;
* local LAN attacks.

## 70.4 End-to-End Tests

Representative scenarios MUST include:

```text
Super Admin provisions student
        ↓
Student performs first login
        ↓
Student changes password
        ↓
Student logs in normally
        ↓
Internet becomes unavailable
        ↓
Student logs into local server
        ↓
Student starts practice
        ↓
Student starts CBT
        ↓
Student disconnects
        ↓
Student reconnects
        ↓
Student resumes attempt
        ↓
Internet returns
        ↓
Identity and academic state synchronize
```

---

# 71. Authentication Acceptance Criteria

Authentication is complete only when:

* all five V1 roles can authenticate appropriately;
* accounts cannot self-register;
* Super Admin provisioning works;
* first-login password change works;
* passwords are securely hashed;
* disabled accounts cannot authenticate;
* suspended accounts cannot authenticate;
* locked accounts cannot authenticate;
* sessions expire correctly;
* sessions can be revoked;
* logout invalidates sessions;
* authentication is rate limited;
* object-level authorization is enforced;
* teacher assignment restrictions work;
* parent-student restrictions work;
* student data isolation works;
* offline authentication works for supported users;
* identity synchronization works;
* credential synchronization is secure;
* credential conflicts are handled safely;
* authentication events are audited;
* no plaintext passwords are stored or logged;
* authentication survives expected local network failures;
* security tests pass;
* regression tests pass.

---

# 72. Definition of Done

The authentication architecture is considered implemented only when:

### Architecture

* authentication boundaries are documented;
* online/local identity behavior is implemented;
* account lifecycle is defined;
* authorization integration is complete.

### Backend

* authentication services are implemented;
* credential management is implemented;
* account-state enforcement is implemented;
* session management is implemented;
* authorization middleware is integrated.

### Database

* identity schema is implemented;
* credential metadata is protected;
* account-state constraints exist;
* session records/state are implemented where required;
* audit records exist.

### Offline

* local identity replica works;
* offline login works;
* local account-state enforcement works;
* credential synchronization works securely;
* local revocation controls exist.

### Security

* passwords are never stored plaintext;
* secrets are protected;
* rate limiting is enabled;
* session security is implemented;
* authorization is enforced server-side;
* security logging is implemented;
* penetration testing covers authentication and authorization.

### Testing

* unit tests pass;
* integration tests pass;
* E2E tests pass;
* offline tests pass;
* synchronization tests pass;
* security tests pass;
* regression tests pass.

### Documentation

* API documentation is updated;
* database documentation is updated;
* security documentation is updated;
* operational recovery procedures are documented.

---

# 73. Non-Negotiable Authentication Rules

The following rules MUST NOT be violated:

1. No public self-registration in V1.
2. Super Admin provisions all accounts.
3. Passwords are never stored in plaintext.
4. Passwords are never logged.
5. Initial passwords require change on first login.
6. Disabled accounts cannot authenticate.
7. Authentication does not imply authorization.
8. Role checks alone are insufficient for scoped resources.
9. Teacher access is assignment-scoped.
10. Parent access is relationship-scoped.
11. Students cannot access another student's private academic data.
12. Offline authentication is supported for approved local workflows.
13. Plaintext passwords are never synchronized.
14. Local identity state is synchronized through the synchronization architecture.
15. Credential changes are security-sensitive synchronization operations.
16. Account deactivation must preserve historical academic records.
17. Sessions must be revocable.
18. Authentication events must be auditable.
19. Authentication secrets must not be hardcoded.
20. The local network must not be treated as inherently trusted.
21. CBT authorization remains separate from basic authentication.
22. AI must not bypass authentication or authorization.
23. Authentication must never be used as a shortcut around business rules.
24. Security controls must fail closed where authorization cannot be established safely.

---

# 74. Final Identity Principle

CRC authentication is built around one stable identity operating across two controlled environments:

```text
                   ONE USER IDENTITY
                         │
          ┌──────────────┴──────────────┐
          │                             │
   ONLINE AUTHENTICATION        LOCAL AUTHENTICATION
          │                             │
          ▼                             ▼
   Online Master State          Local Identity Replica
          │                             │
          └──────────────┬──────────────┘
                         │
                 Secure Synchronization
```

Authentication establishes identity.

Authorization establishes permitted access.

Academic assignments, parent relationships, account state, assessment rules, and other business rules determine what that identity may actually do.

The system must preserve this separation across online operation, school LAN operation, synchronization, assessment recovery, and administrative workflows.

**Identity is persistent. Credentials are replaceable. Sessions are revocable. Authorization is contextual. Historical academic records remain intact.**
