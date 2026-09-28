# API Architecture

**Project:** NAKAMA / Christian Royal College LMS
**Document:** API Architecture and Standards
**Status:** Baseline
**Scope:** V1

---

# 1. Purpose

This document defines the API architecture and standards for NAKAMA.

It establishes the rules for:

* API boundaries
* endpoint organization
* authentication
* authorization
* request validation
* response formats
* error handling
* pagination
* filtering
* sorting
* resource ownership
* idempotency
* concurrency
* assessment APIs
* offline APIs
* synchronization APIs
* content-processing APIs
* AI APIs
* administrative APIs
* API versioning
* observability
* testing
* security

The API layer MUST implement the requirements defined in:

* `PROJECT_CONTEXT.md`
* `REQUIREMENTS.md`
* `ARCHITECTURE.md`
* `SECURITY_REQUIREMENTS.md`
* `docs/database/DATABASE_ARCHITECTURE.md`

---

# 2. API Architecture Principles

The API layer MUST follow these principles:

1. APIs are contracts, not direct database proxies.
2. Authentication and authorization are mandatory for protected resources.
3. Business rules belong in application/domain services.
4. Clients MUST NOT directly manipulate databases.
5. API responses MUST expose only authorized data.
6. APIs MUST remain stable across compatible client versions.
7. State-changing operations MUST be validated server-side.
8. Critical operations MUST be transactional.
9. APIs MUST support safe retries where required.
10. Errors MUST be predictable and machine-readable.
11. Sensitive information MUST never be returned unnecessarily.
12. Offline APIs MUST support operation without Internet connectivity.
13. Synchronization APIs MUST be treated as security-sensitive infrastructure.
14. AI APIs MUST not bypass ordinary authorization.
15. API behavior MUST be observable and auditable.

---

# 3. API Boundaries

NAKAMA has several API boundaries.

```text id="uwl2tv"
                    INTERNET
                       │
             ┌─────────▼─────────┐
             │  ONLINE API       │
             │  Master System    │
             └─────────┬─────────┘
                       │
                Sync Protocol
                       │
             ┌─────────▼─────────┐
             │ LOCAL API         │
             │ School Server     │
             └─────────┬─────────┘
                       │
                     LAN
             ┌─────────┼─────────┐
             ▼         ▼         ▼
          Student   Teacher    Admin
          Client    Client     Client
```

Additional internal service boundaries include:

* content-processing service
* AI gateway
* synchronization worker
* local server manager
* background jobs

---

# 4. API Types

The project SHOULD distinguish between:

### Public/Online Application API

Used by:

* student dashboard
* parent dashboard
* teacher dashboard
* principal dashboard
* Super Admin interface

### Local School API

Used by:

* student clients
* teacher clients
* local administrative clients
* local server services

### Synchronization API

Used only by authorized local installations and the online synchronization service.

### Internal Service APIs

Used between controlled backend services.

These APIs MUST NOT automatically become externally accessible.

---

# 5. API Technology

The implementation MAY use a REST-style HTTP API as the primary application interface.

The architecture MUST support:

* HTTP/HTTPS
* JSON request/response payloads
* standard HTTP semantics
* explicit API versioning

Real-time communication MAY be introduced where required for operational dashboards or server status, but it MUST NOT be required for ordinary academic functionality.

---

# 6. API Base Paths

APIs SHOULD use explicit versioned paths.

Example:

```text
/api/v1/
```

Domain resources SHOULD then use resource-oriented paths.

Examples:

```text
/api/v1/auth
/api/v1/users
/api/v1/students
/api/v1/teachers
/api/v1/parents
/api/v1/academic
/api/v1/questions
/api/v1/assessments
/api/v1/attempts
/api/v1/practice
/api/v1/progress
/api/v1/analytics
/api/v1/sync
/api/v1/content
/api/v1/ai
/api/v1/admin
```

The exact endpoint inventory is defined during module implementation.

---

# 7. Versioning

The public API MUST be versioned.

V1 SHOULD use:

```text
/api/v1/
```

Breaking changes MUST require a new major API version.

Backward-compatible additions MAY remain within the same major version.

---

# 8. API Contract

Every endpoint MUST have a documented contract covering:

* HTTP method
* path
* authentication requirement
* authorization requirement
* request schema
* response schema
* validation rules
* errors
* idempotency behavior
* pagination behavior where applicable
* audit requirements
* rate limits where applicable

---

# 9. HTTP Methods

The implementation SHOULD follow conventional HTTP semantics:

| Method | Typical Use                      |
| ------ | -------------------------------- |
| GET    | Retrieve resource                |
| POST   | Create resource / execute action |
| PUT    | Replace resource                 |
| PATCH  | Partial update                   |
| DELETE | Delete/deactivate resource       |

Business actions MAY use explicit action endpoints when that produces a clearer contract.

Examples:

```text
POST /api/v1/assessments/{id}/publish
POST /api/v1/attempts/{id}/submit
POST /api/v1/promotions/{id}/execute
POST /api/v1/sync/retry
```

---

# 10. Authentication

Protected APIs MUST require authentication.

The authentication mechanism MUST be implemented centrally rather than independently by every endpoint.

Authentication MUST establish:

* user identity
* session/authentication state
* account status

Authentication MUST NOT automatically grant authorization.

---

# 11. Authentication Context

Each authenticated request MUST have a server-side security context containing sufficient information to determine:

* user ID
* roles
* account status
* session state
* relevant installation context where applicable

The application MUST obtain authorization information from trusted server-side state.

---

# 12. Authorization

Every protected endpoint MUST perform authorization.

Authorization MUST consider:

* role
* resource ownership
* academic scope
* teacher assignments
* parent-student relationship
* assessment ownership/access
* installation context
* operational state

---

# 13. Object-Level Authorization

Resource IDs MUST never be treated as authorization.

For example:

```text
GET /api/v1/students/{studentId}
```

MUST verify that the requesting user may access that particular student.

Changing:

```text
studentId=A
```

to:

```text
studentId=B
```

MUST NOT bypass authorization.

---

# 14. Role Scope

The API MUST enforce the following high-level boundaries.

### Super Admin

System-wide administration.

### Principal

Management and academic intelligence within principal permissions.

### Teacher

Assigned academic scope.

### Parent

Explicitly linked children.

### Student

Own academic scope.

These boundaries MUST be enforced on the backend.

---

# 15. Request Validation

Every request MUST be validated.

Validation MUST include:

* data type
* required fields
* string length
* numeric limits
* enum values
* identifier format
* date format
* relationship validity
* business constraints

Malformed requests MUST return controlled validation errors.

---

# 16. Request Size Limits

APIs MUST enforce request size limits.

Different limits SHOULD exist for:

* ordinary JSON
* bulk imports
* file uploads
* synchronization payloads

Oversized requests MUST be rejected before expensive processing.

---

# 17. Content Type

JSON APIs MUST explicitly validate supported content types.

The server MUST NOT assume a request is safe because it declares an expected MIME type.

Uploaded files require independent content validation.

---

# 18. Response Format

Successful API responses SHOULD use a consistent structure.

Example:

```json
{
  "data": {},
  "meta": {}
}
```

Collections MAY use:

```json
{
  "data": [],
  "meta": {
    "page": 1,
    "pageSize": 25,
    "total": 120
  }
}
```

The exact response envelope MUST be standardized before implementation begins.

---

# 19. Resource Representation

API resources SHOULD expose stable public fields rather than database implementation details.

The API MUST NOT automatically serialize database entities directly.

A dedicated API response model SHOULD be used.

This prevents accidental exposure of:

* password hashes
* internal IDs where inappropriate
* security metadata
* internal database fields
* synchronization internals
* private notes

---

# 20. Error Format

Errors MUST use a consistent machine-readable structure.

Recommended format:

```json
{
  "error": {
    "code": "RESOURCE_NOT_FOUND",
    "message": "The requested resource could not be found.",
    "details": [],
    "requestId": "..."
  }
}
```

---

# 21. Error Codes

Error codes MUST be stable.

Examples:

```text
AUTHENTICATION_REQUIRED
AUTHENTICATION_FAILED
ACCOUNT_DISABLED
PASSWORD_CHANGE_REQUIRED
FORBIDDEN
RESOURCE_NOT_FOUND
VALIDATION_FAILED
CONFLICT
INVALID_STATE
RATE_LIMITED
REQUEST_TOO_LARGE
SYNC_CONFLICT
SYNC_FAILED
ASSESSMENT_EXPIRED
ATTEMPT_NOT_FOUND
AI_UNAVAILABLE
INTERNAL_ERROR
```

The frontend MUST use error codes rather than parsing human-readable messages.

---

# 22. HTTP Status Codes

The implementation SHOULD use standard HTTP status codes.

Examples:

| Status | Meaning                                  |
| ------ | ---------------------------------------- |
| 200    | Successful request                       |
| 201    | Resource created                         |
| 202    | Accepted for asynchronous processing     |
| 204    | Successful request with no response body |
| 400    | Invalid request                          |
| 401    | Authentication required/failed           |
| 403    | Authenticated but not authorized         |
| 404    | Resource not found                       |
| 409    | Conflict                                 |
| 413    | Payload too large                        |
| 422    | Validation/business-rule failure         |
| 429    | Rate limited                             |
| 500    | Internal server error                    |
| 503    | Service temporarily unavailable          |

The implementation MUST avoid returning `500` for ordinary client validation errors.

---

# 23. Request IDs

Every API request SHOULD receive a unique request/correlation ID.

The ID MUST:

* appear in logs
* be returned to clients where appropriate
* propagate across internal services
* assist troubleshooting

The request ID MUST NOT expose sensitive information.

---

# 24. Distributed Correlation

Operations spanning:

* API
* database
* sync worker
* content processing
* AI gateway

SHOULD propagate a correlation ID.

This enables operators to trace a request across service boundaries.

---

# 25. Pagination

Collection endpoints MUST implement controlled pagination when results may become large.

Example:

```text
?page=1&pageSize=25
```

or cursor-based pagination where appropriate.

The API MUST enforce maximum page sizes.

Clients MUST NOT request unlimited collections.

---

# 26. Sorting

Sorting MUST be restricted to supported fields.

The API MUST NOT accept arbitrary SQL expressions through sorting parameters.

Example:

```text
?sort=createdAt
?order=desc
```

Only allowlisted sort fields may be used.

---

# 27. Filtering

Filtering MUST use explicit supported fields.

Example:

```text
?status=published
?subjectId=...
?topicId=...
```

User input MUST NOT be directly translated into arbitrary database query expressions.

---

# 28. Search

Search endpoints MUST:

* validate query length
* enforce result limits
* avoid arbitrary query execution
* protect against expensive unbounded searches

Search behavior MUST be documented.

---

# 29. Bulk Operations

Bulk operations MAY be provided for:

* account provisioning
* question import
* academic setup
* data import

Bulk APIs MUST:

* validate each item
* provide item-level errors
* prevent accidental partial corruption
* support controlled batch sizes
* log administrative operations

Large jobs SHOULD be asynchronous.

---

# 30. Idempotency

Operations that may be retried MUST support idempotency where appropriate.

Examples:

* account provisioning
* promotion execution
* assessment submission
* synchronization
* bulk imports

An idempotency key SHOULD be provided through a request header or equivalent mechanism.

---

# 31. Idempotency Storage

The backend MUST retain sufficient information to determine whether a retried operation has already completed.

A duplicate request MUST return the original logical result where appropriate rather than executing the operation twice.

---

# 32. Concurrency Control

State-changing APIs MUST protect against stale updates.

The implementation SHOULD use:

* entity revisions
* ETags
* optimistic concurrency
* explicit state validation

where appropriate.

---

# 33. Optimistic Concurrency

Administrative editing APIs SHOULD support revision-aware updates.

Example:

```text
Client reads revision 8.
Another user changes resource → revision 9.
Client submits update for revision 8.
Server rejects stale update.
```

This prevents silent overwrites.

---

# 34. Authentication APIs

Authentication endpoints SHOULD include operations for:

```text
POST /api/v1/auth/login
POST /api/v1/auth/logout
POST /api/v1/auth/change-password
POST /api/v1/auth/refresh
POST /api/v1/auth/reset-password
GET  /api/v1/auth/session
```

Exact implementation depends on the selected authentication architecture.

---

# 35. First Login API Behavior

After first login:

1. Authentication succeeds.
2. Server identifies the account as requiring password change.
3. Normal application access remains restricted.
4. Password-change endpoint is made available.
5. User changes password.
6. Temporary credential becomes invalid.
7. Normal authorization becomes available.

---

# 36. User Administration APIs

Super Admin APIs SHOULD cover:

```text
GET    /api/v1/users
POST   /api/v1/users
GET    /api/v1/users/{id}
PATCH  /api/v1/users/{id}
POST   /api/v1/users/{id}/deactivate
POST   /api/v1/users/{id}/activate
POST   /api/v1/users/{id}/reset-password
```

Every administrative action MUST be authorized and audited.

---

# 37. Academic APIs

Academic APIs SHOULD cover resources including:

* sessions
* terms
* classes
* arms
* subjects
* topics
* placements
* assignments
* promotion

The implementation MUST maintain clear ownership between academic configuration and user-specific operations.

---

# 38. Teacher Assignment APIs

Assignment APIs SHOULD support:

```text
GET  /api/v1/assignments
POST /api/v1/assignments
PATCH /api/v1/assignments/{id}
POST /api/v1/assignments/{id}/deactivate
```

Teacher visibility MUST be derived from active assignments.

---

# 39. Parent Relationship APIs

Parent-student relationships MUST be managed through authorized administrative operations.

The API MUST enforce the one-parent-per-student rule.

Example:

```text
POST /api/v1/parents/{parentId}/students
DELETE /api/v1/parents/{parentId}/students/{studentId}
```

Deleting a relationship MUST NOT delete either account or the student's academic history.

---

# 40. Question APIs

Question APIs SHOULD support:

```text
GET  /api/v1/questions
POST /api/v1/questions/import
GET  /api/v1/questions/{id}
PATCH /api/v1/questions/{id}
GET  /api/v1/questions/{id}/versions
POST /api/v1/questions/{id}/approve
POST /api/v1/questions/{id}/publish
POST /api/v1/questions/{id}/unpublish
```

Teacher access MUST be limited by assignment and question ownership rules.

---

# 41. Question Import API

Question imports SHOULD normally create an asynchronous processing job.

Example:

```text
POST /api/v1/questions/import
```

Response:

```text
202 Accepted
```

with a processing job identifier.

The API MUST NOT require a browser request to remain open for long OCR operations.

---

# 42. Content Processing APIs

Content-processing APIs SHOULD expose controlled job management.

Example:

```text
POST /api/v1/content/jobs
GET  /api/v1/content/jobs/{id}
POST /api/v1/content/jobs/{id}/cancel
```

The processing system MUST NOT expose arbitrary filesystem operations.

---

# 43. Assessment APIs

Assessment APIs SHOULD include:

```text
GET  /api/v1/assessments
POST /api/v1/assessments
GET  /api/v1/assessments/{id}
PATCH /api/v1/assessments/{id}
POST /api/v1/assessments/{id}/publish
POST /api/v1/assessments/{id}/unpublish
```

Access MUST follow academic and role boundaries.

---

# 44. Assessment Attempt APIs

Core attempt APIs SHOULD include:

```text
POST /api/v1/assessments/{id}/attempts
GET  /api/v1/attempts/{id}
PATCH /api/v1/attempts/{id}/answers
POST /api/v1/attempts/{id}/heartbeat
POST /api/v1/attempts/{id}/recover
POST /api/v1/attempts/{id}/submit
```

The final endpoint set MUST be designed around the server-authoritative CBT state machine.

---

# 45. Start Attempt

Starting an attempt MUST:

1. verify student authorization,
2. verify assessment availability,
3. verify attempt limits,
4. select questions,
5. persist the question set,
6. calculate the authoritative deadline,
7. create the attempt,
8. return the attempt state.

These operations MUST be transactional.

---

# 46. Answer Save API

Answer updates MUST be designed for frequent operation.

The endpoint MUST:

* verify the attempt belongs to the user
* verify the attempt is active
* verify the question belongs to the attempt
* validate the option
* persist the answer
* update relevant timestamps
* return authoritative state

The API MUST reject answer changes after submission or expiration.

---

# 47. Heartbeat API

The heartbeat exists to support recovery and operational monitoring.

It MAY report:

* client activity
* connection state
* client timestamp

The server remains authoritative for:

* deadline
* attempt state
* answers
* submission status

The client MUST NOT extend the deadline through heartbeat manipulation.

---

# 48. Recovery API

Recovery MUST locate the existing active attempt.

The server MUST return:

* current attempt state
* authoritative remaining time
* saved answers
* question sequence
* submission state
* relevant recovery metadata

The API MUST NOT create a duplicate attempt merely because the client reconnects.

---

# 49. Submission API

Submission MUST be idempotent.

Repeated submission requests MUST NOT create multiple results.

The server MUST:

1. verify authorization,
2. verify attempt state,
3. finalize answers,
4. finalize timing,
5. score deterministically,
6. persist the result,
7. mark the attempt complete,
8. return the result.

---

# 50. Practice APIs

Practice uses the same assessment engine.

Practice APIs SHOULD allow:

* starting practice
* saving answers
* resuming practice
* submitting practice
* retrieving practice history
* retrieving topic progress

Practice MUST NOT bypass the same fundamental integrity rules used by assessment.

---

# 51. Student Dashboard APIs

Student APIs SHOULD expose:

* profile
* current placement
* progress
* weak topics
* practice history
* assessment history
* available learning content

Only the student's authorized data may be returned.

---

# 52. Teacher Dashboard APIs

Teacher APIs SHOULD expose:

* assigned classes
* assigned subjects
* class performance
* student performance
* question-review queue
* assessment management
* learning gaps

All data MUST be scoped by assignment.

---

# 53. Parent Dashboard APIs

Parent APIs SHOULD expose:

* linked children
* child progress
* child performance
* subject performance
* weak subjects/topics
* relevant assessment history

Parent APIs MUST never accept an arbitrary student ID as sufficient authorization.

---

# 54. Principal Dashboard APIs

Principal APIs SHOULD expose:

* school performance
* class comparisons
* subject performance
* learning gaps
* assessment trends
* academic summaries

Principal access MUST not grant Super Admin privileges.

---

# 55. Super Admin APIs

Super Admin APIs SHOULD provide:

* user administration
* academic configuration
* assignments
* promotion control
* synchronization monitoring
* local server status where applicable
* configuration
* audit access
* system diagnostics

Sensitive endpoints MUST have explicit authorization checks.

---

# 56. Promotion API

Promotion MUST be a controlled operation.

Possible endpoints:

```text
GET  /api/v1/promotions/preview
POST /api/v1/promotions/prepare
POST /api/v1/promotions/{id}/execute
```

The implementation MUST separate:

* preview
* validation
* activation
* execution

Super Admin MUST explicitly activate the promotion run.

---

# 57. Promotion Safety

The API MUST prevent:

* duplicate promotion
* invalid destination classes
* missing academic structures
* partial execution
* unauthorized execution

Promotion execution MUST be auditable.

---

# 58. Analytics APIs

Analytics APIs SHOULD distinguish between:

* raw records
* derived metrics
* cached aggregates

Analytics endpoints MUST not expose records outside the requesting user's scope.

Large analytics queries SHOULD use precomputed aggregates or optimized query strategies where necessary.

---

# 59. Synchronization API

Synchronization APIs are infrastructure APIs.

They MUST NOT be available to ordinary users.

Example structure:

```text
POST /api/v1/sync/handshake
POST /api/v1/sync/push
POST /api/v1/sync/pull
POST /api/v1/sync/ack
GET  /api/v1/sync/status
POST /api/v1/sync/retry
```

Exact protocol MAY differ after implementation design.

---

# 60. Sync Handshake

The handshake SHOULD establish:

* installation identity
* authentication
* protocol version
* supported capabilities
* current sync state
* compatibility

Incompatible clients MUST fail safely.

---

# 61. Sync Push

Push requests MUST contain controlled change operations.

Each operation SHOULD contain:

* operation ID
* entity type
* entity ID
* revision
* operation type
* origin
* payload
* metadata

The server MUST validate every operation.

---

# 62. Sync Pull

Pull requests SHOULD use cursors or revision checkpoints.

The client MUST be able to resume after interruption.

The server MUST NOT require the client to restart the entire synchronization process after a transient failure.

---

# 63. Sync Acknowledgement

The client and server SHOULD explicitly acknowledge successfully applied changes.

Acknowledgement MUST be idempotent.

A missing acknowledgement MUST result in safe retry behavior rather than data duplication.

---

# 64. Sync Conflict API

Conflicts SHOULD be exposed only to authorized administrative interfaces.

Example:

```text
GET  /api/v1/sync/conflicts
GET  /api/v1/sync/conflicts/{id}
POST /api/v1/sync/conflicts/{id}/resolve
```

Resolution MUST follow entity-specific conflict rules.

---

# 65. Local Server API

The local API MUST support operation without Internet access.

Critical local endpoints include:

* login
* session
* student dashboard
* teacher dashboard
* question retrieval
* assessment start
* answer persistence
* heartbeat
* recovery
* submission
* scoring
* practice
* local progress

These endpoints MUST NOT depend on successful cloud connectivity during normal offline operation.

---

# 66. Offline API Rules

Offline endpoints MUST:

* use local authoritative data
* remain available during Internet outages
* queue required synchronization changes
* expose connection state where appropriate
* avoid blocking core workflows on cloud requests

---

# 67. Cloud Connectivity

The local API MAY communicate with the online system through the synchronization worker.

Normal student/teacher academic requests SHOULD NOT directly depend on Internet connectivity.

This prevents cloud outages from becoming school-wide academic outages.

---

# 68. Server Status API

The local server manager SHOULD expose authenticated operational endpoints for:

* server status
* database health
* sync status
* AI status
* connected clients
* storage health
* service health
* version information

Sensitive diagnostics MUST require administrative authorization.

---

# 69. AI API

The AI subsystem SHOULD be exposed through a controlled gateway.

Example:

```text
POST /api/v1/ai/conversations
GET  /api/v1/ai/conversations
POST /api/v1/ai/conversations/{id}/messages
GET  /api/v1/ai/conversations/{id}
```

The gateway MUST enforce authorization before retrieval or model execution.

---

# 70. AI Context Authorization

Before retrieving context, the AI gateway MUST establish:

1. authenticated user,
2. role,
3. academic scope,
4. permitted content,
5. permitted student data.

Only authorized context may enter the model prompt.

---

# 71. AI Business Logic Restriction

AI APIs MUST NOT expose direct mutation operations for:

* grades
* student placement
* permissions
* users
* assessments
* promotion
* question publication
* synchronization

AI output MUST remain advisory unless a separately authorized deterministic workflow consumes it.

---

# 72. AI Failure Response

If the AI service is unavailable, the API SHOULD return a controlled service-unavailable response.

The failure MUST NOT affect:

* login
* assessment
* practice
* scoring
* synchronization
* academic records

---

# 73. File APIs

File APIs MUST enforce:

* file size limits
* content validation
* authorization
* secure storage
* controlled download access

Example:

```text
POST /api/v1/files
GET  /api/v1/files/{id}
DELETE /api/v1/files/{id}
```

The actual endpoint set depends on the content architecture.

---

# 74. Secure Downloads

Downloads MUST verify authorization before serving files.

A user MUST NOT gain access to a file merely by changing its identifier.

Temporary signed download URLs MAY be used where appropriate.

---

# 75. Rate Limiting

Rate limits SHOULD be applied according to endpoint sensitivity.

Stricter limits SHOULD apply to:

* login
* password reset
* administrative operations
* file uploads
* AI requests
* synchronization
* expensive analytics

CBT answer-saving endpoints require special treatment because excessive rate limiting could disrupt legitimate assessment operation.

---

# 76. Caching

Sensitive personalized responses MUST NOT be cached in shared caches incorrectly.

Caching MUST respect:

* authentication
* user identity
* authorization scope
* data freshness

Assessment state MUST not be served from stale shared caches.

---

# 77. API Security Headers

Web-facing API/application responses SHOULD use appropriate security headers where applicable.

The exact header set MUST align with the frontend architecture and deployment environment.

---

# 78. CORS

CORS MUST use an explicit allowlist.

Wildcard origins MUST NOT be used for authenticated APIs.

Allowed origins MUST be environment-specific.

The local school server MUST not automatically allow arbitrary Internet origins.

---

# 79. API Documentation

All production APIs MUST have machine-readable documentation.

OpenAPI is the preferred format.

API documentation MUST describe:

* authentication
* endpoints
* request schemas
* response schemas
* errors
* authorization expectations

---

# 80. API Documentation Security

Internal administrative and infrastructure endpoints MUST not automatically be publicly discoverable.

Production API documentation SHOULD distinguish:

* public application APIs
* authenticated APIs
* administrative APIs
* internal APIs
* synchronization APIs

---

# 81. API Deprecation

Deprecated endpoints MUST have:

* documented replacement
* deprecation date
* migration guidance
* removal policy

Breaking changes MUST NOT be silently introduced.

---

# 82. API Observability

The API layer MUST expose enough telemetry to diagnose:

* latency
* errors
* authentication failures
* authorization failures
* rate limiting
* database failures
* sync failures
* service failures

Metrics MUST avoid storing sensitive request payloads.

---

# 83. API Logging

Logs SHOULD include:

* request ID
* endpoint
* method
* status
* duration
* authenticated principal where appropriate
* service/component
* error category

Logs MUST NOT contain:

* passwords
* tokens
* private keys
* full sensitive payloads

---

# 84. API Testing

Every API module MUST include:

### Contract Tests

Verify request/response schemas.

### Authentication Tests

Verify authenticated and unauthenticated behavior.

### Authorization Tests

Verify every role boundary.

### Validation Tests

Verify malformed input handling.

### Security Tests

Verify injection, object-level authorization and abuse resistance.

### Integration Tests

Verify database and service behavior.

### Failure Tests

Verify controlled behavior during dependency failures.

---

# 85. API Regression Requirements

Existing API contracts MUST be regression-tested before release.

Changes to:

* authentication
* resource schemas
* assessment APIs
* synchronization
* local offline APIs

MUST trigger appropriate regression suites.

---

# 86. API Definition of Done

An API feature is complete only when:

1. Endpoint contract is documented.
2. Authentication behavior is defined.
3. Authorization behavior is defined.
4. Request validation exists.
5. Response schema exists.
6. Error behavior exists.
7. Database transaction behavior is defined.
8. Idempotency is addressed where required.
9. Audit requirements are addressed.
10. Security tests exist.
11. Integration tests pass.
12. Regression tests pass.
13. OpenAPI documentation is updated.
14. Frontend consumers are updated.
15. No undocumented breaking behavior exists.

---

# 87. API Non-Negotiables

The following MUST NOT be violated:

1. No protected API without authentication.
2. No authorization based solely on client-provided identifiers.
3. No direct client database access.
4. No unrestricted object lookup.
5. No arbitrary SQL through query parameters.
6. No unrestricted CORS.
7. No secrets in API responses.
8. No passwords or tokens in logs.
9. No duplicate execution of non-idempotent operations.
10. No browser authority over CBT state.
11. No cloud dependency for critical offline workflows.
12. No AI bypass of authorization.
13. No synchronization API access for ordinary users.
14. No undocumented breaking API changes.
15. No production API without security and integration tests.

---

# 88. Final API Principle

The API layer is the enforcement boundary between clients and the authoritative system.

Clients may request actions.

Clients may provide data.

Clients may display results.

But:

> **The server decides what is valid, what is authorized, what state is authoritative, and what operation is allowed.**

The API architecture MUST therefore preserve:

* security
* determinism
* offline operation
* recoverability
* synchronization integrity
* auditability
* backward compatibility
* maintainability

The API MUST remain a controlled contract between system components rather than becoming a thin, unrestricted wrapper around the database.
