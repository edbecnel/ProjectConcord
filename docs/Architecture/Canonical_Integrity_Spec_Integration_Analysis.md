[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › Canonical Integrity Integration Analysis

# Canonical Integrity Specification — Integration Analysis

> **Status:** Draft — for [EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) review  
> **Date:** 2026-09-15  
> **Source:** [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)

## Purpose

Satisfy SPEC-003 handover §62–§67: affected documents, investigation answers, gaps, implementation stages, and validation strategy before major implementation.

## Documents amended

| Document | Change summary |
|---|---|
| [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) | Permanent normative spec (from root handover) |
| [ADR-0011](ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md) | Trusted state, fingerprints, external edit, reconciliation boundary |
| [System Architecture Overview](System_Architecture_Overview.md) | Integrity pipeline and illustrative services |
| [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) | Boundary vs SPEC-003 |
| [SPEC-001](../Specifications/features/SPEC-001-mvp-edf-desktop-client.md) | M5 integrity hooks note |
| [ADR-0006](ADRs/ADR-0006-AI-Boundary.md), [ADR-0007](ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md) | Cross-references |
| [Implementation Roadmap](../Development/Implementation_Roadmap.md) | M5–M7 phasing |
| [EDF Gap Register](../Development/EDF_Gap_Register.md) | GAP-022–GAP-025 |
| [CRA ↔ ProjectConcord Gap Analysis](../Development/CRA_ProjectConcord_Gap_Analysis.md) | CRA-G6–CRA-G8 |
| [CRA Alignment](CRA_Alignment_and_Responsibility_Boundaries.md) | Integrity responsibility paragraph |
| [EDF_BOOTSTRAP_REPORT.md](../../EDF_BOOTSTRAP_REPORT.md) | SPEC-003 mapping row |

## Investigation answers (§62, from ProjectConcord docs)

| Topic | Finding |
|---|---|
| EDF canonical artifacts | Markdown/YAML under `docs/` per [ADR-0002](ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md); adoption via `edf-adoption.yaml` / `edf-project-context.yaml` |
| Artifact identification | ID conventions (ADR, SPEC, EGR, …); [ADR-0007](ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md) + [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) |
| Lifecycle / governance fields | Partially in templates; **GAP-004**, **GAP-022**, **GAP-023** |
| Legal transitions | Not fully machine-readable; **GAP-023**, **GAP-024** |
| Relationships | Link graph + inferred edges; **GAP-005**, **GAP-016**; typed semantics deferred |
| CRA identity / relationships | CRA-0001–0003 Draft; aligned at principle level per [CRA gap analysis](../Development/CRA_ProjectConcord_Gap_Analysis.md) |
| Fingerprints / trust | Not in CRA; **ADR-0011** + operational store (**ADR-0009**) |
| Integrity service ownership | `Edf.Integrity` + application orchestration per [System Architecture Overview](System_Architecture_Overview.md) |

## EDF gaps (§65 → register)

| Handover theme | Register ID |
|---|---|
| Governed field registry | GAP-022 |
| Lifecycle transition rules | GAP-023 (extends GAP-004) |
| Prerequisites / acceptance | GAP-024 |
| Trusted integrity portability | GAP-025 |

Proposed upstream EDF feedback: artifact schema, governed fields, transition rules, relationship cardinality, supersession/deletion rules (see SPEC-003 §65).

## CRA gaps (§47, §66 → analysis)

| ID | Topic |
|---|---|
| CRA-G6 | Authorized trust state |
| CRA-G7 | Representation / semantic fingerprints |
| CRA-G8 | Governed lifecycle operation vocabulary |

## Implementation stages (§9)

| Stage | Scope |
|---|---|
| M5 | Identity/metadata validation, duplicate ID, external-change detection hook, authoring integration |
| M6 | Full SPEC-002 registry/index + SPEC-003 lifecycle/governed-field validation, fingerprints, reconciliation-required states |
| M6+ | Shared validation boundary with multi-user platform |
| M7+ | Engineering-intent reconciliation layer; CI/server enforcement deferred |

## Validation strategy (§10)

| Layer | Approach |
|---|---|
| Unit | ID parsers, fingerprint computation, transition rule tables (where defined), duplicate detection |
| Integration | Fixture EDF repos with intentional external edits (IDE-style), Git baseline deltas, scenarios §61 A–C |
| Concurrency | Deferred M6+ with change-set fixtures |
| Lifecycle | Manual status change scenarios must not auto-trust without evaluation |
| Reconciliation | Separate test suite from canonical integrity (M7+) |

## Gate impact

[EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) must review SPEC-003, this analysis, ADR-0011, and materially amended architecture documents before **Gate satisfied**.

## Parent

- [Architecture](README.md)

## Related Documents

- [Multi-User Amendment Analysis](Multi_User_Amendment_Affected_Document_Analysis.md)
