# Implement Feature Workflow

This workflow orchestrates the following agents:

1. Architect Agent
2. Implementation Agent
3. Reviewer Agent
4. Test Agent

---

## Stage 1 - Architecture

Use:
- .github/agents/architect.agent.md
- .github/architecture/system-overview.md
- .github/architecture/coding-standards.md

Responsibilities:
- analyze requirements
- define design
- define interfaces
- identify risks
- define project structure

Output:
- implementation plan
- service structure
- class responsibilities

---

## Stage 2 - Implementation

Use:
- .github/agents/implementation.agent.md

Responsibilities:
- implement production code
- implement dependency injection
- implement logging
- implement retry handling

Requirements:
- async/await
- clean architecture
- proper disposal
- structured logging

---

## Stage 3 - Review

Use:
- .github/agents/reviewer.agent.md

Responsibilities:
- review generated code
- identify defects
- identify performance issues
- identify maintainability concerns

Review Areas:
- async correctness
- memory usage
- exception handling
- naming clarity
- SQL connection safety

---

## Stage 4 - Testing

Use:
- .github/agents/test.agent.md

Responsibilities:
- generate unit tests
- identify edge cases
- test retry handling
- test failure scenarios

Testing Rules:
- use xUnit
- use mocks
- avoid real database dependencies

---

## Final Output Requirements

The final solution must include:
- production-ready code
- dependency injection setup
- structured logging
- retry handling
- unit tests
- review findings
- implementation summary