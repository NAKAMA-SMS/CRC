# Architecture decisions

Repository authority is defined in AGENTS.md section 2 and clarified by ADR-0001. Accepted records establish design direction; they do not claim implementation or tests have passed. Proposed/BLOCKED entries cannot be implemented as if accepted.

| Record | Status | Scope |
|---|---|---|
| [ADR-0001](ADR-0001-authority-and-module-gates.md) | Accepted | Document precedence, decision timing, module integration gates and design/example boundaries |
| [ADR-0002](ADR-0002-module-00-technical-foundation.md) | Accepted | Concrete Module 00 stack, layout, persistence, API/configuration/security conventions, validation and Windows publishing foundation |

Only two ADRs are necessary for this pass. Later decisions are tracked with their gates in [PROJECT_STATE.md](../../PROJECT_STATE.md); do not generate speculative ADRs for future modules. Changing an accepted decision requires rationale, affected scope and synchronized canonical documentation under AGENTS.md section 6.
