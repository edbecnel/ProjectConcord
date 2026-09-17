[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Development](README.md) › CRA ↔ ProjectConcord Gap Analysis

# CRA ↔ ProjectConcord Gap Analysis

> **Status:** Draft  
> **Owner:** ProjectConcord  
> **Last Reviewed:** 2026-09-15  
> **CRA source:** Local clone `/Users/edbecnel/Development/GitHub/Canonical-Representation-Architecture` (CRA-0001–0003 Draft v1.0)

## Purpose

Compare **Canonical Representation Architecture (CRA)** normative specifications with **ProjectConcord** requirements for identity, relationships, referential integrity, and authoring—without inventing CRA policy where the architecture is silent.

## ProjectConcord requirement sources

| Document | Role |
|---|---|
| [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) | Product behavior (registry, index, resolvers, move/rename) |
| [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) | Integrity, transitions, external change, trusted state |
| [CRA Alignment and Responsibility Boundaries](../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md) | CRA → EDF → ProjectConcord layering |
| [ADR-0007](../Architecture/ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md) | ID-first, derived registry/index |
| [ADR-0008](../Architecture/ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md) | CRA-aligned; no mandatory CKES for MVP |
| [EDF Gap Register](EDF_Gap_Register.md) | EDF-layer gaps (GAP-005, 015, 016, 017) |

## CRA normative baseline (in scope for this analysis)

| Spec | Status | Covers |
|---|---|---|
| [CRA-0001](file:///Users/edbecnel/Development/GitHub/Canonical-Representation-Architecture/docs/Specifications/CRA-0001.md) | Draft | FP-1…FP-5: canonical vs derived, identity vs org, relationships vs navigation, governed designation, preservation |
| [CRA-0002](file:///Users/edbecnel/Development/GitHub/Canonical-Representation-Architecture/docs/Specifications/CRA-0002.md) | Draft | IM-1…IM-7: identifiers, locators, equivalence, relocation, versioning, lineage, registry obligation |
| [CRA-0003](file:///Users/edbecnel/Development/GitHub/Canonical-Representation-Architecture/docs/Specifications/CRA-0003.md) | Draft | RF-1…RF-7: fidelity, distinction preservation, navigation as derived view |
| CRA-0000 | Discovery only | Motivation—not normative for implementation |

**Explicitly deferred in CRA (not gaps in CRA repo—gaps for ProjectConcord if it needs them now):**

- Full **canonical relationship model** (beyond navigation fidelity) — CRA-0003 §Deferred  
- Validation tooling / certification — all three specs  
- Publication pipeline, identifier registry **implementation technology** — CRA-0002  
- CKES / delegated authority / pragmatic canonicalization — [AWI-0005](file:///Users/edbecnel/Development/GitHub/Canonical-Representation-Architecture/docs/Architecture/Watch_Items/AWI-0005-delegated-authority-and-pragmatic-canonicalization.md) and related AWIs  

---

## Summary matrix

| Status | Count | Meaning |
|---|---:|---|
| **Aligned** | 9 | CRA principles/specs directly support ProjectConcord design |
| **Partial** | 8 | CRA direction clear; mechanics or vocabulary incomplete for deterministic build |
| **CRA silent / deferred** | 7 | ProjectConcord must use EDF + interim policy or wait for future CRA |
| **Out of CRA scope** | 5 | Application/EDF/Git/UI concerns—not CRA deficiencies |

---

## Requirement mapping

### Aligned (CRA supports ProjectConcord)

| ProjectConcord requirement | CRA basis |
|---|---|
| Artifact identity ≠ path/filename/link | FP-2; IM-2, IM-4 |
| Derived Artifact Registry (non-canonical DB) | FP-1; CRA-0002 §Architectural Obligations #1 (registry binds ID within scope) — compatible with **derived** registry per [ADR-0002](../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md) |
| Treat Markdown links as representations, not identity | FP-3; RF-7 |
| Move/rename without semantic change preserves identity | IM-4; RF-4 |
| Semantic edit → version/lineage, not silent overwrite | IM-5; RF-5 |
| Superseded artifacts remain resolvable | IM-6 |
| Navigation / PROJECT_INDEX / link graph as derived | FP-3; RF-7 |
| Lossy vs fidelity-preserving views (e.g. UI summaries, search index) | RF-2, RF-3 — `.projectconcord/` caches are lossy or fidelity-preserving by purpose |
| Hierarchy CRA → EDF → ProjectConcord | [CRA Alignment doc](../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md); EDF [CRA_CKES_EDF_Boundaries](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/CRA_CKES_EDF_Boundaries.md) |

### Partial (design allowed; deterministic implementation incomplete)

| ProjectConcord requirement | CRA state | Gap |
|---|---|---|
| **Stable IDs** (`ADR-0017`, `SPEC-0032`) | IM-1 syntax profile flexible; opacity/scope required | EDF defines ID **conventions** (filename/heading), not CRA **designation** ceremony—ProjectConcord must map EDF types to CRA “canonical artifact within scope” without a single EDF `canonical identifier` field |
| **Equivalence determination** (same artifact / version / different artifact / derived) | IM-3 normative | Engine must implement IM-3 logic; CRA gives no algorithm for Markdown/YAML manifests |
| **Relationship Index** with typed edges (`governed-by`, `implements`) | FP-3 separates relationships from navigation; **no typed relationship catalog** in CRA-0001–0003 | Relationship **vocabulary** is EDF/project convention today; CRA defers “full canonical relationship model” |
| **Relationship definitions vs instances** (per CRA alignment handover) | Not specified in CRA-0001–0003 | ProjectConcord SPEC-002 describes intent; CRA does not normativize definition/instance split |
| **Committed distinctions** for fidelity checks | RF-2 requires scope policy to list them | EDF templates imply distinctions (Status, Spec ID) but no machine-readable “committed distinction” set per artifact type |
| **Governed designation / scope** | FP-4; IM-1 at designation time | EDF scope = repo + `edf-project-context.yaml`; not framed as CRA governance designation—**semantic alignment, procedural gap** |
| **Reciprocal / required relationships** (SPEC-002 §21) | Not in CRA-0001–0003 | EDF gap GAP-016; CRA RF-7 only addresses navigation vs relationships |
| **Authorized canonical state / trust** | FP-1, IM-5 imply lineage; no **trust record** or **authorized transition** model | SPEC-003 + ADR-0011; operational store — see **CRA-G6** |
| **Representation vs semantic fingerprints** | RF-2 committed distinctions partial | Detection heuristics in ProjectConcord; **CRA-G7** |
| **Governed transition semantics** | IM-5 versioning; not lifecycle **Accept/Supersede** ops | EDF GAP-023/024; **CRA-G8** |
| **Split / merge / cross-artifact equivalence** | IM-7 requires governed acts | EDF move/rename/archive rules partial (GAP-009); no CRA tooling |

### CRA silent or deferred (do not treat as CRA bugs—track as upstream or interim)

| ProjectConcord requirement | Notes |
|---|---|
| **Identity Resolver / Link Resolver APIs** | Application architecture; CRA specifies obligations, not service names |
| **Referential Integrity Service** orchestration | ProjectConcord; CRA does not define multi-file repair workflows |
| **Transactional change sets** (SPEC-002 §22) | Not in CRA; Git + human approval is ProjectConcord/EDF workflow |
| **External move detection / stale link repair** | Operational; CRA IM-4 principle applies, not detection heuristics |
| **Broken reference taxonomy** (invalid, stale, missing required, reciprocal) | Partially related to IM-3 + RF-2; taxonomy is ProductConcord/EDF |
| **CKES runtime** | AWI-0005, CKES workstreams; [ADR-0008](../Architecture/ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md) defers mandatory dependency |
| **AI-assisted designation / pragmatic canonicalization** | AWI-0003, AWI-0005; outside CRA-0001–0003 |
| **Validation / conformance scoring for CRA** | All CRA specs defer tooling—ProjectConcord uses EDF Framework Advisor for EDF layer only |

### Out of CRA scope (expected at application layer)

| Topic | Owner |
|---|---|
| Avalonia UI, EDF script invocation, Git client features | ProjectConcord product |
| SPEC/ADR/EGR file layout and templates | EDF + ProjectConcord profile |
| Program gates (EGR-G0) | EDF EGR-0001 + ProjectConcord |
| Code ↔ documentation reconciliation | ProjectConcord (PCON-0000); not CRA Core |
| Milestones, gates, AWIs in project graph | EDF domain semantics |
| **Architectural Audit Records (AAR)** | EDF + ProjectConcord workflow ([ADR-0012](../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md)); not CRA scope |

---

## Layered gap view

```text
CRA (CRA-0001..0003)     Partial typed relationships, designation ceremony,
                         validation methodology, delegation (AWIs)

EDF                      Artifact types, link syntax, lifecycle, gates (EGR)
                         — see EDF_Gap_Register GAP-005, 009, 015, 016

ProjectConcord (SPEC-002) Resolvers, move/rename UX,
                         transactional edits

ProjectConcord (SPEC-003) Integrity layers, transitions,
                         external change, trusted state

ProjectConcord (SPEC-002/003) External change detection
```

ProjectConcord **must not** fill CRA deferrals by inventing **global** canonical semantics. It **may** implement **scope-local** behavior for EDF-managed Git repositories, documented in ADRs and the EDF gap register, until CRA publishes relationship and validation specs.

---

## Highest-risk gaps (implementation blockers for M6 referential integrity)

| ID | Risk | Mitigation |
|---|---|---|
| **CRA-G1** | No normative **relationship type** model | Start with EDF-link + user-confirmed types; map to FP-3; propose CRA follow-on spec |
| **CRA-G2** | **Designation** vs “file exists in `docs/`” | Document scope = EDF project root; designation = accepted artifact status + ID parse; align FP-4 narratively |
| **CRA-G3** | **Committed distinctions** undefined per artifact | Start from EDF templates; extend `edf-project-context` or local policy table |
| **CRA-G4** | IM-3 **equivalence** across external edits | Heuristics (ID in content, rename pairs) + human confirm; log low confidence |
| **CRA-G5** | CRA **Draft** status | Track CRA spec versions; re-run this analysis when CRA-0001–0003 move to Accepted |
| **CRA-G6** | No **authorized trust state** model | SPEC-003 + ADR-0011 operational records; propose CRA workstream on governed transitions |
| **CRA-G7** | **Fingerprint** semantics for Markdown/YAML | ProjectConcord representation + governed semantic hashes; align with RF-2 distinctions |
| **CRA-G8** | **Lifecycle operation** vocabulary (Accept, Supersede) | EDF schema feedback (GAP-023/024); do not invent global CRA ops in code |

---

## Conformance posture (recommended)

ProjectConcord should **not** claim full **CRA-0003 conformance** at MVP.

Recommended statement:

> ProjectConcord is **CRA-aligned** at the principles and identity-model intent (CRA-0001 FP-1–FP-5, CRA-0002 IM-1–IM-7 where applicable to EDF artifact IDs), implements **EDF-canonical** storage per ADR-0002, and tracks gaps in this document and [EDF Gap Register](EDF_Gap_Register.md). Full CRA conformance and optional CKES integration are **post-MVP** ([ADR-0008](../Architecture/ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md)).

Optional future work: CRA adoption report (similar to [EGLS CRA Adoption Report](file:///Users/edbecnel/Development/GitHub/Canonical-Representation-Architecture/docs/Development/EGLS_Adoption/CRA_Adoption_Report_EGLS-001.md)) after M6 prototype.

---

## Upstream contributions (CRA / EDF)

| Proposal | Beneficiary |
|---|---|
| **CRA-0004** (or workstream): Canonical **relationship definitions** and instances for engineering docs | ProjectConcord, EDF |
| **EDF**: Machine-readable artifact ID + optional `edf-relations` | ProjectConcord Engine |
| **CRA validation methodology**: Principles-level checklist automation | All adopters |
| **AWI-0005 resolution**: Delegated designation for AI-assisted authoring | ProjectConcord + CKES path |

---

## Related documents

- [CRA Alignment and Responsibility Boundaries](../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md)
- [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)
- [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)
- [Implementation Roadmap](Implementation_Roadmap.md) — milestones M5–M6
- [EGR-G0 — Architecture Planning Gate](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) (Gate G0 review item)

## Parent

- [Development](README.md)
