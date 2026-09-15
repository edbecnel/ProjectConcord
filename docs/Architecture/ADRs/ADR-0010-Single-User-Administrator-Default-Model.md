# ADR-0010: Single-User Administrator Default Model

## Status

Accepted

## Date

2026-09-15

## Context

[ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) establishes a multi-user platform. [AMD-0002](../AMD-0002-Single-User-Administrator-Model.md) requires that individual engineers MUST NOT manage a parallel single-user architecture or assign themselves every functional role for routine work.

## Decision

1. **No separate single-user product** — A one-person project is the simplest configuration of the same identity, membership, authorization, and project model used for teams.

2. **Default Administrator** — The user who creates a ProjectConcord project (project owner) SHALL receive the **Administrator** project role by default unless project configuration explicitly specifies otherwise.

3. **Administrator permissions** — Administrator SHALL imply full project-level permissions for normal ProjectConcord operations (view/create/edit/move artifacts, manage membership when collaboration is enabled, configure project settings) without requiring separate assignments for functional personas (Architect, Developer, PM, QA, etc.).

4. **Personas vs roles** — **Authorization roles** (who may perform an action) remain distinct from **personas / workspaces** (UI perspectives, dashboards, default views). An Administrator working alone MAY use multiple personas without separate role grants.

5. **EDF governance** — Where EDF or an accepted gate record requires **independent review** (for example, [EGR-G0](../../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md)), ProjectConcord MUST NOT bypass that requirement solely because the user is Administrator. The app MAY streamline UX for a solo engineer but MUST surface when a second party is required.

6. **Transition to teams** — Adding members and narrower roles extends the same project; there is no conversion from a “single-user project type” to a “team project type.”

## Alternatives Considered

### Implicit single-user mode with no roles

- Advantages: Minimal M1 types.
- Disadvantages: Second codebase or migration when adding members.
- Reason not selected: AMD-0002 §49.

### Require role assignment for every persona

- Advantages: Uniform RBAC UX.
- Disadvantages: Unacceptable friction for solo engineers.
- Reason not selected: AMD-0002 §48.

## Consequences

### Positive

- Solo and team projects share one security model and test matrix.

### Negative

- UI must explain optional personas without implying missing permissions for Administrators.

## References

- [AMD-0002](../AMD-0002-Single-User-Administrator-Model.md)
- [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)
- [PROJECT_CHARTER](../../../PROJECT_CHARTER.md)
