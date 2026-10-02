# LMS Engineering Operating Contract

## 1. Purpose

This repository contains the source code, architecture, documentation, tests, and engineering decisions for the CRC LMS project.

All AI coding agents, including Codex, must treat this repository as the primary source of truth for the project.

The objective is to build production-quality software through controlled, testable, documented engineering processes.

This project must not be developed through assumptions, improvisation, or dependency on conversational memory.

## Product and Repository Boundary

CRC is the product/system developed in this repository. NAKAMA is the technology company developing/providing CRC. Christian Royal College is the school/customer/deployment context.

CRC may use the attribution **CRC powered by NAKAMA**.

This repository covers the CRC LMS / Management System and its documented technical systems only. The NAKAMA corporate website is a separate future project outside this repository. Do not introduce corporate website pages, architecture, modules, or requirements here. Unrelated NAKAMA products, services, and company management systems require explicit scope approval.

Design-system work in this repository is the **CRC Product Design System**, with NAKAMA attribution where appropriate. The future corporate website requires its own separate design-system project.

---

# 2. Source of Truth

The repository is the authoritative source of project context.

AI agents MUST NOT rely on previous chat sessions as authoritative project documentation.

When working on this repository, agents must consult the appropriate project documents before making implementation decisions.

The general context hierarchy is:

1. `AGENTS.md`
2. `PROJECT_CONTEXT.md`
3. `PROJECT_STATE.md`
4. `DEVELOPMENT_ROADMAP.md`
5. `ARCHITECTURE.md`
6. `REQUIREMENTS.md`
7. `TESTING_STRATEGY.md`
8. `SECURITY_REQUIREMENTS.md`
9. Relevant Architecture Decision Records in `docs/decisions/`
10. Relevant module documentation under `modules/`
11. Relevant technical documentation under `docs/`
12. Existing source code and tests

If two documents conflict, the conflict must be identified and resolved through an explicit documented decision.

Do not silently choose one interpretation. The explicit precedence, decision-timing and cross-module acceptance clarification is recorded in `docs/decisions/ADR-0001-authority-and-module-gates.md`. The hierarchy above remains unchanged; project state and delivery sequencing cannot waive product or mandatory security requirements.

---

# 3. Core Engineering Principle

The project must be developed as a sequence of independently testable vertical modules.

A module represents a complete business capability.

A module may contain:

* frontend functionality
* backend functionality
* database changes
* API changes
* offline functionality
* synchronization functionality
* AI functionality
* infrastructure
* automated tests

Frontend, backend, database, AI, and offline functionality must NOT automatically be treated as separate development modules.

The objective is to produce complete, testable capabilities rather than disconnected technical components.

---

# 4. No Guessing

AI agents MUST NOT invent requirements.

If a requirement is:

* missing
* ambiguous
* contradictory
* architecturally significant
* security-sensitive
* data-sensitive
* destructive
* difficult to reverse

the agent must stop and identify the issue before implementing it.

The agent must not fabricate:

* user behavior
* business rules
* API contracts
* database relationships
* security policies
* authentication behavior
* authorization rules
* synchronization rules
* AI behavior
* data retention policies
* deployment assumptions

When clarification is required, clearly state:

1. What is known
2. What is unknown
3. Why the uncertainty matters
4. What decision is required

---

# 5. Do Not Depend on Chat History

Chat history may provide development context, but it is not the authoritative project specification.

Important decisions must be represented in repository documentation.

If a requirement appears in conversation but is not yet represented in the repository, it must be documented before being treated as a permanent architectural requirement.

---

# 6. Respect Existing Decisions

An AI agent MUST inspect existing documentation and Architecture Decision Records before changing an established design.

Do not replace an existing architecture simply because another approach appears simpler.

Any significant architectural change requires:

1. identification of the existing decision
2. explanation of the proposed change
3. analysis of consequences
4. explicit approval
5. updated Architecture Decision Record
6. updates to affected documentation
7. implementation
8. regression testing

---

# 7. Scope Discipline

The entire project architecture must be understood before implementation.

However, understanding the entire project does not authorize implementation of the entire project.

Agents must implement only the current assigned module or task.

Do not prematurely implement future modules.

Do not introduce future functionality merely because it is visible in the project roadmap.

Future requirements may influence interfaces and architecture where necessary, but future functionality must not be implemented without authorization.

---

# 8. Module Development Lifecycle

Every module follows this lifecycle:

1. Read project context
2. Read current project state
3. Read roadmap
4. Read module specification
5. Read relevant architecture decisions
6. Inspect existing implementation
7. Produce an implementation plan
8. Validate the plan against requirements
9. Implement
10. Write/update tests
11. Run unit tests
12. Run integration tests
13. Run end-to-end tests where applicable
14. Run regression tests
15. Perform security validation where applicable
16. Verify acceptance criteria
17. Update documentation
18. Update project state
19. Mark the module complete only when all required checks pass

A module is NOT complete merely because the implementation compiles or the UI appears functional.

---

# 9. Definition of Done

A task is complete only when:

* implementation is complete
* requirements are satisfied
* appropriate automated tests exist
* tests pass
* integration behavior is verified
* existing functionality remains functional
* security requirements are satisfied
* documentation is updated
* architectural changes are documented
* acceptance criteria are satisfied

Where a requirement cannot be automatically tested, a documented manual verification procedure must exist.

---

# 10. Regression Safety

Every completed module becomes part of the project's stable baseline.

When a new module is introduced, previously completed modules must remain functional.

The expected progression is:

Module 00
→ test
→ freeze

Module 01
→ test Module 01
→ regression test Module 00
→ integration test 00 + 01
→ freeze

Module 02
→ test Module 02
→ regression test 00 + 01
→ integration test 00 + 01 + 02
→ freeze

The same principle applies throughout the project. The owner-approved Module 00 gate exception dated 2026-10-02 in `docs/decisions/ADR-0001-authority-and-module-gates.md` permits conditional progression through Modules 01-04 while F00-02/F00-13/F00-17 evidence is deferred. Module 00 remains NOT FROZEN; all three criteria must pass and Module 00 must freeze before Module 05 starts. Other module decisions, authorization, regression and acceptance requirements remain binding.

---

# 11. Database Discipline

Database changes must be deliberate and documented.

Agents must:

* avoid destructive schema changes without explicit authorization
* use migrations
* preserve data integrity
* define relationships explicitly
* define constraints intentionally
* consider indexing requirements
* consider uniqueness requirements
* consider deletion behavior
* consider auditability
* update database documentation when schema changes

Never modify production-oriented schemas through ad-hoc manual changes.

---

# 12. API Discipline

API contracts must be explicit.

Agents must consider:

* endpoint purpose
* request schema
* response schema
* authentication requirements
* authorization requirements
* validation
* error responses
* idempotency where applicable
* pagination where applicable
* rate limiting where applicable
* versioning strategy where applicable

Breaking API changes require explicit documentation and regression testing.

---

# 13. Authentication and Authorization

Authentication and authorization are separate concerns.

Agents must never assume that successful authentication grants unrestricted access.

Every protected operation must be evaluated against the user's authorized role and relationship to the requested data.

The LMS uses administrator-controlled account provisioning.

There is no public self-registration flow.

Users are provisioned through authorized administrative processes.

The exact authentication and authorization model is defined by the project context and identity module documentation.

Agents must not introduce public registration unless the project requirements are explicitly changed.

---

# 14. Data Ownership and Relationships

Academic data relationships must be explicit.

In particular:

* students have unique student identities
* parents may be associated with multiple students
* parent-child relationships must be represented explicitly
* access to a child's data must be authorization-controlled
* one parent's access must not expose another student's information without authorization

Never infer relationships from names, email addresses, or other unreliable identifiers when a canonical identifier exists.

---

# 15. Offline-First Requirements

Offline functionality is a core architectural concern of this project.

Agents must distinguish between:

* online-only functionality
* offline-capable functionality
* synchronization functionality

Offline behavior must not be implemented as an afterthought.

When implementing functionality that must operate offline, the agent must explicitly consider:

* local persistence
* network absence
* retry behavior
* synchronization
* conflict handling
* data integrity
* recovery after interruption
* local authentication/session requirements
* stale data behavior

---

# 16. AI Engineering Rules

AI functionality must be deterministic where deterministic behavior is required.

AI must not be used as a replacement for deterministic business logic.

Examples of functionality that should remain deterministic include:

* scoring
* permissions
* authentication
* role enforcement
* data validation
* question-answer correctness
* synchronization state
* audit events

AI may assist with appropriate functions such as:

* academic explanations
* tutoring
* contextual assistance
* authorized intelligence and analysis

AI architecture must follow the project context.

Content processing and academic AI are separate concerns.

Online document/content processing may use the approved OCR and document-processing pipeline.

Local Ollama functionality is reserved for the defined offline academic AI use cases.

Agents must not silently substitute one AI system for another.

---

# 17. Security by Default

Security is a requirement throughout development, not a final-stage activity.

Agents must consider:

* authentication
* authorization
* input validation
* output handling
* injection prevention
* session security
* credential handling
* secrets management
* data exposure
* logging
* auditability
* dependency security
* file-upload security
* API security
* offline device security
* synchronization security

Never place secrets in source code.

Never commit credentials.

Never disable security controls merely to make a feature work.

If a security control must temporarily be bypassed during development, it must be explicit, documented, isolated, and removed before production.

---

# 18. File Uploads

The LMS will support administrative data import and academic material processing.

File uploads must therefore be treated as security-sensitive operations.

Agents must validate:

* file type
* file size
* file structure
* encoding
* required fields
* malformed records
* duplicate records
* dangerous content
* parsing failures

Never trust uploaded filenames, MIME types, or client-side validation alone.

---

# 19. Observability

Important system behavior must be observable.

Agents should implement appropriate:

* structured logging
* error reporting
* audit events
* health checks
* synchronization status
* import status
* processing status

Logs must not expose passwords, secrets, or unnecessary sensitive information.

---

# 20. Error Handling

Errors must be handled deliberately.

Do not silently swallow errors.

Do not expose internal stack traces or sensitive implementation details to end users.

Errors should:

* be logged appropriately
* provide useful user-facing messages
* preserve technical diagnostic information for authorized operators
* maintain system integrity
* support recovery where possible

---

# 21. Testing Requirements

Tests must be written alongside implementation.

Appropriate testing levels include:

* unit tests
* integration tests
* API tests
* database tests
* component tests
* end-to-end tests
* offline tests
* synchronization tests
* security tests
* regression tests

Do not remove or weaken tests merely to make a build pass.

If a test exposes a legitimate defect, fix the defect rather than modifying the test to hide it.

---

# 22. Dependency Discipline

Do not introduce a dependency merely because it is convenient.

Before adding a dependency, consider:

* whether existing project dependencies already solve the problem
* maintenance status
* security history
* license compatibility
* bundle/runtime impact
* ecosystem compatibility
* long-term maintainability

New dependencies must be justified when they materially affect architecture or security.

---

# 23. Code Quality

Code must prioritize:

* readability
* maintainability
* explicitness
* type safety
* separation of concerns
* predictable behavior
* testability
* appropriate abstraction

Do not over-engineer simple functionality.

Do not create abstractions without a clear reason.

Do not duplicate complex business logic across frontend and backend.

The backend must remain authoritative for security-sensitive and business-critical operations.

---

# 24. Documentation Synchronization

When implementation changes an architectural behavior, update the relevant documentation.

Documentation must not describe behavior that the software does not implement.

When a significant decision changes:

1. update the relevant ADR
2. update architecture documentation
3. update requirements if necessary
4. update module documentation
5. update project state

Documentation drift is considered an engineering defect.

---

# 25. Git Discipline

Changes should be logically grouped.

Commit messages should clearly communicate the change.

Avoid mixing unrelated work in a single commit.

Do not rewrite shared history unless explicitly authorized.

Do not commit:

* secrets
* credentials
* private keys
* generated sensitive data
* unnecessary build artifacts
* local machine state

---

# 26. Change Management

Before making a significant change, determine:

* what depends on the existing behavior
* what could break
* whether database migration is required
* whether API contracts change
* whether tests must change
* whether documentation must change
* whether an ADR is required

Prefer small, reversible changes.

---

# 27. Agent Reporting

After completing an assigned task, report:

## Implemented

What was changed.

## Files Changed

Relevant files.

## Tests

Commands executed and their results.

## Integration

What existing functionality was verified.

## Documentation

What documentation was updated.

## Known Issues

Anything unresolved.

## Next Step

The next authorized engineering step.

Do not claim a test passed unless it was actually executed.

Do not claim a feature works if it was not verified.

---

# 28. Stop Conditions

The agent must stop and request clarification when:

* requirements conflict
* an architectural decision is missing
* a destructive database change is required
* a security-sensitive behavior is undefined
* required external credentials are unavailable
* an implementation would violate an established project decision
* a requested feature conflicts with the current module scope
* the correct behavior cannot be determined from project documentation and source code

Never resolve important ambiguity through invention.

---

# 29. Professional Engineering Standard

This project must be treated as production software.

The goal is not merely to produce code that runs.

The goal is to produce software that is:

* correct
* secure
* testable
* maintainable
* observable
* documented
* deployable
* recoverable
* understandable by future engineers

Every implementation decision should be made with the long-term lifecycle of the system in mind.

---

# 30. Final Rule

Before writing code, understand the requirement.

Before changing architecture, understand the existing architecture.

Before declaring completion, test the implementation.

Before starting the next module, verify the current module.

When uncertain, do not guess.

**Read. Verify. Plan. Implement. Test. Integrate. Document.**
