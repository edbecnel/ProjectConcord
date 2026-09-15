# ADR-0011: Canonical Artifact Integrity and Trusted State

## Status

Accepted

## Date

2026-09-15

## Context

[SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) requires ProjectConcord to distinguish **canonical representation**, **structural validity**, **semantic validity**, and **authorized trusted state** for EDF Markdown in Git. Users and tools (IDEs, Git, Cursor) may edit canonical files outside the application. [SPEC-002](../../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) and [ADR-0007](ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md) address identity and relationships but not lifecycle authorization or trusted state. EDF lacks fully machine-readable governed fields and transition rules ([EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-004, GAP-022–GAP-025).

## Decision

1. **Open repository** — Canonical EDF artifacts remain human-readable files in Git with direct filesystem access preserved; external editing is expected, not exceptional.

2. **Layered integrity** — Evaluate artifacts through layered checks (structural → semantic structural → relationship/lifecycle → authorization → engineering-intent reconciliation). Lower layers MUST be predominantly deterministic; AI MAY assist only where [SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) §53 permits.

3. **Governed vs ordinary content** — Fields that affect engineering governance (identity, type, lifecycle status, formal relationships, supersession, acceptance, gate state, etc.) are **governed** when EDF defines them or when logged as gaps. Prefer **semantic operations** (for example `AcceptArtifact`) over undetected raw text edits for governed fields when authoring through ProjectConcord.

4. **Validity vs authorization vs trust**
   - **Validity** — Artifact conforms to EDF structural and semantic rules ProjectConcord can determine.
   - **Authorization** — Requested transition or write is permitted for the actor ([ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [ADR-0010](ADR-0010-Single-User-Administrator-Default-Model.md)).
   - **Trust** — ProjectConcord records that the current governed state is consistent with last known authorized baseline or explicit adoption/reconciliation.
   Authorization does not override invalid transitions; valid Markdown does not imply trusted governed state.

5. **Fingerprints** — **Representation** and **governed semantic** fingerprints detect external change; fingerprints are evidence, not authority. Git commit alone does not imply authorized governed state.

6. **Trusted Integrity Record** — Operational metadata (last trusted fingerprints, integrity status, reconciliation flags) MAY persist in shared or local operational store per [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) and MUST NOT replace Git as canonical source ([ADR-0002](ADR-0002-EDF-Canonical-Source-of-Truth.md)).

7. **External changes** — Detect, classify (content vs governed-field delta), and surface for reconciliation or revalidation. No silent canonicalization; no silent rejection of legitimate external edits ([SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) §55–§56).

8. **AI and Cursor** — Treat Cursor and other external editors as external repository modifiers unless integrated via formal ProjectConcord APIs. AI MUST NOT self-authorize governed transitions ([ADR-0006](ADR-0006-AI-Boundary.md)).

9. **Reconciliation boundary** — **Canonical artifact integrity** (is the EDF artifact valid and legitimately transitioned?) is separate from **implementation reconciliation** (does documented intent match code and validation evidence?). The latter remains M7+ architecture; both may appear on the project dashboard.

10. **EDF authority** — ProjectConcord MUST NOT invent permanent governed-field or transition policy; ambiguities feed the EDF gap register and EDF improvement feedback.

## Alternatives Considered

### Trust Git commit as authorization

- Advantages: Simple; matches many workflows.
- Disadvantages: Manual status edits and broken relationships become “trusted” without governance.
- Reason not selected: SPEC-003 §36.

### Sealed canonical files (ProjectConcord-only writes)

- Advantages: Strong integrity.
- Disadvantages: Breaks EDF openness and toolchains.
- Reason not selected: SPEC-003 §5.

## Consequences

### Positive

- Clear pipeline for authoring, external edit, and multi-user shared validation.

### Negative

- Additional services (`IntegrityService`, transition evaluation, external change monitoring) beyond CRUD.

### Risks

- Over-reliance on AI for integrity — mitigate via SPEC-003 §53 and deterministic-first roadmap phasing.

## References

- [SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)
- [SPEC-002](../../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)
- [System Architecture Overview](../System_Architecture_Overview.md)
- [Canonical Integrity Integration Analysis](../Canonical_Integrity_Spec_Integration_Analysis.md)
