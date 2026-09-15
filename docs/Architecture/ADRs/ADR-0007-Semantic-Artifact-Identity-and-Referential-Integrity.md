# ADR-0007: Semantic Artifact Identity and Referential Integrity

## Status

Accepted

## Date

2026-09-15

## Context

[SPEC-002](../../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) requires ProjectConcord to maintain semantic relationships among EDF artifacts across move, rename, external edit, and Git operations. EDF does not yet fully specify stable artifact identity or formal relationship syntax ([EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-005, GAP-015, GAP-016).

## Decision

1. **Semantic identity** — Independently referenceable artifacts (ADR, SPEC, AWI, EGR, etc.) are identified primarily by **stable artifact IDs** (for example `ADR-0017`, `SPEC-0032`) parsed from EDF conventions, not by filesystem path or Markdown link target alone.
2. **Representation** — Filename, directory, and relative Markdown links are **representations** that may change; the Engine resolves ID → current canonical location via an **Artifact Registry** (derived, per [ADR-0004](ADR-0004-Derived-Data-and-Cache.md)).
3. **Relationship index** — A derived **Relationship Index** records semantic edges (for example `governed-by`, `implements`) separately from incidental navigation links; inferred edges are flagged with confidence.
4. **Mutating operations** — Move, rename, and supersede operations MUST run **referential impact analysis** before commit; user approval for bulk link updates; no silent rewrite of canonical files without explicit authorization ([ADR-0006](ADR-0006-AI-Boundary.md)).
5. **EDF authority** — ProjectConcord MUST NOT invent permanent EDF identity or relationship policy; ambiguities are logged to the gap register and surfaced in UI.

6. **Integrity pipeline** — Artifact Registry and Relationship Index from this ADR feed layered integrity checks in [SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) ([ADR-0011](ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md)). Lifecycle **authorization** and **trusted state** are out of scope for ADR-0007.

## Alternatives Considered

### Path-as-identity

- Advantages: Simple implementation.
- Disadvantages: Breaks on rename; contradicts SPEC-002 and CRA-aligned principles.
- Reason not selected: Rejected.

### Canonical identity database replacing Git docs

- Advantages: Strong referential integrity.
- Disadvantages: Violates [ADR-0002](ADR-0002-EDF-Canonical-Source-of-Truth.md).
- Reason not selected: Rejected; registry remains derived.

## Consequences

### Positive

- Safe CRUD and authoring; foundation for project graph and reconciliation.

### Negative

- Engine complexity; dependency on EDF ID conventions until EDF schemas mature.

### Risks

- Mis-parsed IDs — mitigate with validation and human review on low confidence.

## References

- [SPEC-002](../../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)
- [CRA Alignment and Responsibility Boundaries](../CRA_Alignment_and_Responsibility_Boundaries.md)
- [System Architecture Overview](../System_Architecture_Overview.md)
- [SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)
