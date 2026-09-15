# ADR-0008: CRA and CKES Dependency Boundary

## Status

Proposed

## Date

2026-09-15

## Context

[CRA Alignment and Responsibility Boundaries](../CRA_Alignment_and_Responsibility_Boundaries.md) and [SPEC-002](../../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) overlap concepts being developed in **Canonical Representation Architecture (CRA)** and **CKES**. ProjectConcord must not fork foundational canonical semantics (PCON-0000 §12, CRA handover §3–6).

EDF documents boundaries in [CRA, CKES, and EDF Boundaries](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/CRA_CKES_EDF_Boundaries.md).

## Decision

1. **Hierarchy** — CRA (foundational representation principles) → EDF (engineering documentation semantics) → ProjectConcord (operational application). ProjectConcord consumes applicable CRA/EDF concepts; it does not define a competing general-purpose canonical architecture.
2. **No mandatory CKES/CRA runtime for MVP** — Opening, authoring, validating, and navigating an EDF repository MUST NOT require a CKES or CRA network service unless a later ADR justifies it with explicit scope.
3. **Conformance posture** — ProjectConcord SHOULD remain **CRA-aligned** in identity and relationship modeling ([ADR-0007](ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md)) without claiming to implement full CRA or CKES.
4. **Future integration** — Optional CKES integration MAY be added when CRA/CKES specifications are stable; architecture MUST NOT block that path (see CRA alignment doc §19).
5. **Gaps** — Where CRA is undefined, report gaps ([EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-017); do not silently invent CRA policy inside ProjectConcord.

## Alternatives Considered

### Embed CKES client in MVP

- Advantages: Strongest semantic identity.
- Disadvantages: Scope explosion; immature dependency.
- Reason not selected: Violates incremental MVP (PCON-0000 §53).

### Ignore CRA entirely

- Advantages: Faster short-term coding.
- Disadvantages: Duplicate semantics; future rework.
- Reason not selected: Rejected per CRA alignment handover.

## Consequences

### Positive

- Clear dependency story for reviewers and EDF/CRA maintainers.

### Negative

- Some relationship semantics remain interim until CRA/EDF mature.

## References

- [CRA Alignment and Responsibility Boundaries](../CRA_Alignment_and_Responsibility_Boundaries.md)
- [SPEC-002](../../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)
