[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › PCON-0005

# PCON-0005: ProjectConcord Workflow Framework

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural framework record |
| **Normative** | Yes |
| **Status** | **Accepted** (2026-10-03 — Project Architect publication) |
| **Record ID** | PCON-0005 |
| **Date** | 2026-10-03 |
| **Owner** | ProjectConcord |
| **Implementation** | **Not authorized** — architecture and documentation only |
| **Companion (first prescribed workflow)** | [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03) |
| **Research support (non-normative)** | [Workflow Framework — Comparative Research Support](Analysis/Workflow_Framework_Comparative_Research_Support.md) |

---

## 1. Purpose

ProjectConcord is architected as a **governed workflow framework** capable of hosting **multiple prescribed workflows**.

**Operational reality today:** exactly **one** ProjectConcord-native prescribed workflow applies — the [Governed Engineering Workflow (GEW)](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md).

**Architectural reality:** the framework must support **future** prescribed workflows (including EDF-consumed workflows such as GMFP) **without** implying that those workflows already exist as product behavior.

Principle:

```text
Many workflows architecturally;
one applicable ProjectConcord-native workflow operationally today.
```

This record defines the **general framework**. GEW-specific semantics live in [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md). Software Development **B-layer** entities, relay boundaries, and normative GEW requirements (PC-AIGOV) are in [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) and [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (**Draft**, GEW/PC-AIGOV requirements reconciled — published 2026-10-03).

---

## 2. Prescribed Workflow

### 2.1 Identity

A **Prescribed Workflow** is a versioned, registered definition of a governed process: purpose, invariants, topology, optional profiles, and configuration schema.

A **separate** prescribed workflow is warranted when governing semantics **materially** differ, particularly when any of the following hold:

- different **governing purpose**;
- different **completion / evidence contract**;
- **incompatible authorization law** (rules that cannot safely coexist as profiles of one workflow);
- **lifecycle / topology semantics** that are not safe variants of the same workflow.

**Gate count alone does not establish workflow identity.**

### 2.2 Distinctions (canonical)

| Concept | Definition |
|---|---|
| **Prescribed Workflow** | What governed process is being executed (`WorkflowId`, definition version). |
| **Workflow Profile** | A governed variant **within** one prescribed workflow (rigor/compression topology — not a global Concord mode). |
| **Bounded Configuration** | Explicitly exposed parameters whose legal values are constrained by workflow/profile/policy. |
| **Conditional Branch / Topology** | Policy- or context-selected paths **inside** one workflow definition (including repeatable review/validation loops). |
| **Workflow Instance State** | Current execution position, loops, STOP, pending imports — **not** configuration. |

### 2.3 Workflow definition and version

Each prescribed workflow definition **SHALL** be identifiable by:

- stable **`WorkflowId`** (URI-like string; registry assigns);
- **`DefinitionVersion`** (semver or monotonic integer — chosen at implementation time).

Compatibility rules **SHALL** reject combinations of definition version, profile, and configuration that the definition does not allow. Invalid configuration **MUST NOT** corrupt the last valid governed state.

### 2.4 Workflow registry / discovery

The framework **SHALL** maintain a conceptual **prescribed workflow registry**:

- built-in Concord definitions (e.g. GEW);
- optional **consumed** definitions (e.g. EDF GMFP topology referenced by id, not redefined).

Discovery **MAY** be static (bundled manifests) initially. A workflow-selection UI is **not** required when only one native workflow applies.

### 2.5 Workflow instance

A **Workflow Instance** binds a unit of governed work to:

- `WorkflowId` + `DefinitionVersion`;
- optional `ProfileId`;
- bounded configuration overrides (work-unit scope);
- **instance state** (derived + persisted operational facts as required).

Instances **SHALL** correlate with **DevelopmentWorkAuthorization** and relay packages where Software Development extension applies ([ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)).

### 2.6 Workflow topology

Topology describes **SynchronizationPoints**, **AuthorizedExecutionIntervals**, **EvidenceRequirements**, and **Controls** (§7) and how they relate — including **non-linear** graphs:

- conditional paths;
- repeatable validation/review;
- remediation loops;
- policy-inserted requirements.

The framework **MUST NOT** assume a linear stage list. **Stages** are workflow-specific **presentation labels** only; there is **no** global reusable Stage library (§8).

### 2.7 Durable topology position (definition, traversal, orthogonal state)

[FW-8](#5-framework-invariants) and [ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15 require recoverable **workflow-instance** state without PA/EA chat history. The following **SHALL** be distinguished — they **MUST NOT** be collapsed into a single mutually exclusive “position” label:

| Layer | Meaning | Canonical? |
|---|---|---|
| **A. Workflow-definition topology** | For a prescribed workflow `WorkflowId` + `DefinitionVersion`: stable **topology places** and **permitted transitions** (including branches, loops, and merge points). Framework **primitives** (§6) **MAY** be **associated** with places or transitions — they are **not** substitutes for place identity. | Definition registry / manifest |
| **B. Workflow-instance traversal state** | Where a particular **WorkflowInstance** is in that definition graph: **`TopologyPlaceId`** plus **`TraversalOccurrenceId`** — an **opaque stable identity** established whenever the instance **enters or re-enters** a topology place. Re-entry to the same place **SHALL** yield a **new** occurrence identity. Occurrence identities **MUST NOT** be interpreted as first/second ordering, per-place counters, global sequence, or progress ordinals. | **Yes** (operational persistence) |
| **C. Orthogonal governed state** | Facts that affect advancement and recovery but **are not** topology places — for example **STOP**; **synchronization** requirement/satisfaction state; **AuthorizedExecutionInterval** active/applicable state; future **DevelopmentWorkAuthorization** / **Control** correlation ([GAP-027](../Development/EDF_Gap_Register.md)). Multiple dimensions **MAY** apply simultaneously (for example at place *P* while STOP is active and an AEI is open). | **Yes** — semantic categories; physical storage is implementation-defined |
| **D. Derived projections** | Operator labels, relay package-kind “stages” ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-023), “blocked”, actionable frontier, operator focus/selection — **MUST NOT** become topology places merely because they are useful to display ([ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §2, §8). | **No** |

**GEW v1** normative places and transitions are defined in [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §9–§10. [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) audit timelines and conversational PA/EA sequences **SHALL NOT** be promoted to normative topology.

**No global or per-place synchronization/traversal ordinal** is required by this framework. Loops, revisitation, branching, merging, multiple active instances, and future workflow **origin** and **blocking** relationships **SHALL** remain compatible with place + occurrence identity ([ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §12–§13).

**Persistence authority (current phase):** governed operational facts **SHALL** be durable at the **running installation’s** local operational persistence boundary ([ADR-0019](ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)); domain semantics **SHALL NOT** depend unnecessarily on SQLite physical ownership so a future synchronization/shared-authority layer ([GAP-049](../Development/EDF_Gap_Register.md)) can be introduced without redefining `WorkflowInstance` semantics. Implementation: [GAP-054](../Development/EDF_Gap_Register.md).

---

## 3. Operational configuration classes

| Class | Meaning |
|---|---|
| **Invariant** | Rules that cannot be altered within the effective governed workflow. May originate at framework, prescribed workflow, or selected profile level. |
| **Bounded Configuration** | Explicitly exposed settings with types/ranges/enumerations constrained by governing workflow/profile semantics. |
| **Policy-Derived** | Behavior from authoritative governed policy/context (F-layer, risk, scope, linked MVR, repo facts) — not manually selected. |
| **Instance State** | Topology traversal (§2.7 **B**); orthogonal STOP/sync/AEI/control state (§2.7 **C**) — **not** configuration. |

There is **no** unconstrained “workflow-configurable” class. A lower layer **MUST NOT** weaken an inherited governance constraint.

---

## 4. Effective configuration

Effective configuration **SHALL** be **deterministically derived** when possible from:

1. framework invariants;
2. prescribed-workflow invariants and topology;
3. selected profile constraints and defaults;
4. project-level governed policy defaults (**D** — Working Environment / policy facet);
5. work-unit bounds (typically **DevelopmentWorkAuthorization** scope and capabilities);
6. policy-derived requirements.

Separate persisted artifacts for every conceptual layer are **not** required. Implementations **MAY** cache derived effective configuration; cache **MUST** be reconstructible from authoritative inputs.

**Invalid configuration:**

- **rejected**;
- **fail-closed** where authorization/governance is affected;
- preserves **last valid** governed state;
- surfaces **Attention** when human resolution is required ([ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)).

**No silent** profile or workflow changes.

---

## 5. Framework invariants

The following apply to **all** prescribed workflows (unless a future PA decision explicitly documents a narrower framework — which is discouraged):

| ID | Invariant |
|---|---|
| **FW-1** | Configuration **cannot** weaken inherited invariants. |
| **FW-2** | Invalid configuration is rejected; last valid governed state preserved. |
| **FW-3** | Effective configuration derivation is deterministic from authoritative inputs. |
| **FW-4** | Workflow/profile resolution does not solicit information already known from authoritative state. |
| **FW-5** | Authorization failure is **fail-closed** (STOP / no implied permission). |
| **FW-6** | UI projections are **derived**; the shell is not an independent workflow engine or authority store ([ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)). |
| **FW-7** | Each prescribed workflow **SHALL** preserve provenance sufficient to reconstruct the governed decisions **that workflow requires**, without treating full conversation or chat history as the authoritative record. |
| **FW-8** | **Workflow-instance** identity, **instance state**, and **synchronization/topology position** **SHALL** be durably recoverable (directly or deterministically derivable from persisted operational inputs) sufficient to support operator recovery projections ([ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15) and **non-regression** on relay import ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §19). Redundant storage of derivable facts is **not** required. Implementation: [GAP-054](../Development/EDF_Gap_Register.md). |

Framework invariants are **not** engineering-specific. GEW-specific invariants (including binding to PC-AIGOV-016 where applicable) are in [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md).

---

## 6. Declarative primitive vocabulary

Initial reusable vocabulary (declarative constraints only — **no** runtime types in this tranche):

| Primitive | Role |
|---|---|
| **SynchronizationPoint** | Human or organizational decision required before proceed. |
| **AuthorizedExecutionInterval** | Work may proceed without additional human sync until interval ends or policy interrupts. |
| **EvidenceRequirement** | Condition to satisfy (manual, automated, or hybrid satisfaction policy). |
| **Control** | Capability-bounded permitted/prohibited operations — operational **DevelopmentWorkAuthorization** expresses Software Development **Control** semantics; do not duplicate. |

**Gate** may be used as a **composite / human-facing** grouping of sync + evidence + roles; it is **not** required as a fifth canonical primitive.

Evidence satisfaction **MAY** be manual, automated, or hybrid per prescribed workflow rules.

---

## 7. Stages and composition

- **No** global Stage library.
- Reusable semantics live in §6 primitives.
- Workflow-specific **stage** names **MAY** appear in operator stepper projections ([ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §5).

---

## 8. Workflow and profile resolution

### 8.1 Workflow

| Situation | Behavior |
|---|---|
| Zero applicable workflows | Attention — cannot proceed governed |
| Exactly one applicable | Automatic resolution |
| Multiple, policy-deterministic | Automatic resolution |
| Multiple, judgment required | Governed human/PA resolution |

### 8.2 Profile

| Situation | Behavior |
|---|---|
| Authoritative policy/state determines profile | Automatic resolution |
| Otherwise | PA / governed selection |
| Ambiguity | Attention — **does not** silently select High Assurance or any other profile |
| No authoritative profile determination | PA / governed selection required — **does not** implicitly select Standard (or any profile) merely because Accelerated or High Assurance was not forced |

A governed policy **MAY** establish an explicit default profile (for example Standard for GEW); that is **policy**, not an intrinsic property of a profile identifier.

---

## 9. Canonical versus derived state

| Concern | Authoritative | Derived |
|---|---|---|
| `WorkflowId`, `DefinitionVersion`, `ProfileId` | Workflow instance + DWA correlation | — |
| Bounded overrides | Instance / DWA | — |
| Project policy defaults | **D** policy store (when implemented) | — |
| Effective configuration | — | Deterministic resolver (cacheable) |
| Instance state | Operational store | Attention, Next Action, stepper |
| Handover / relay package | Derived snapshot ([ADR-0004](ADRs/ADR-0004-Derived-Data-and-Cache.md)) | Carries refs to workflow/profile/effective config |

### 9.1 Operation baseline and repository HEAD

An active governed operation **SHALL** retain or reference the **applicable authoritative baseline** (for example commit or governed baseline pointer) relevant to that operation.

**Current repository HEAD** and the operation's **governed baseline** **SHALL NOT** be assumed identical after interruption or elapsed time.

A changed HEAD **SHALL NOT** silently rewrite the operation's recorded baseline.

Applicable workflow, profile, and policy **SHALL** determine whether reconciliation, **Attention**, **STOP**, reauthorization, or continuation is required when baseline and current repository state diverge. Universal Git conflict policy is **out of scope** for this framework record.

Broader pause/resume reconciliation when work was intentionally suspended is deferred to [PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md) (**Proposed**, [AWI-0002](Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md)).

---

## 10. Extensibility

Future prescribed workflows **SHALL** register without redesigning:

- primitive vocabulary;
- configuration class model;
- effective-configuration resolver;
- resolution rules.

EDF-consumed workflows (e.g. GMFP) **SHALL** reference EDF semantics; ProjectConcord **MUST NOT** redefine them ([AWI-0004](Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md)).

---

## 11. UI projection boundary

Architectural configurability **does not** imply a UI control.

- No workflow chooser required when only one native workflow applies.
- No UI for reserved extension points (organization matrices, environment promotion, etc.).
- Configuration UI **only** for meaningful exposed bounded configuration ([ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §GEW surface).

---

## 12. YAGNI boundary (framework)

Excluded from current architecture/runtime commitment:

- arbitrary workflow designer; BPMN engine; drag-and-drop topology construction;
- topology scripting; N-of-M approval engine; complete role/organization matrix;
- environment-promotion workflow; generalized emergency workflow;
- unrestricted workflow configuration; global reusable Stage library.

The framework **MUST** avoid unnecessary blocking of future extension but **MUST NOT** design these now.

---

## 13. Governed Interactive Engineering Session (GIES)

Framework-level **Governed Interactive Engineering Session** architecture is defined in [PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) (**Accepted** 2026-10-03) with layer allocation in [ADR-0025](ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) (**Accepted** 2026-10-03). Routing **SHALL** be able to depend on prescribed workflow, profile, effective configuration, instance state, and authoritative evidence. **Implementation** remains unauthorized ([AWI-0011](Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md), [GAP-055](../Development/EDF_Gap_Register.md)).

---

## 13.1 Governed Work Disposition and interaction consequence

How arbitrary interaction and reasoning become authoritative governed consequences — without modeling every conversational step — is defined in [PCON-0008](PCON-0008-Governed-Work-Disposition-and-Interaction-Consequence.md) (**Accepted** 2026-10-06). **Governed Work Disposition** is a semantic/protocol boundary pattern; it is **not** a fifth framework primitive. **Implementation** of disposition intake and extended dependency granularity remains unauthorized ([GAP-054](../Development/EDF_Gap_Register.md)).

---

## 14. Related documents

- [ADR-0024 — Governed Engineering Workflow](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)
- [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) (historical full-loop reference — not the only topology)
- [PCON-0008](PCON-0008-Governed-Work-Disposition-and-Interaction-Consequence.md) (interaction → disposition → governed state; **Accepted** 2026-10-06)
- [AWI-0010](Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md), [GAP-054](../Development/EDF_Gap_Register.md#gap-054--workflow-framework-runtime-and-effective-configuration)
- [M7a-WF-1-doc tranche plan](../Handover/ProjectConcord-M7a-WF-1-doc-GEW-Topology-Documentation-Tranche-Plan.md)

---

## 15. Publication disposition

**Accepted** and published on `main` (2026-10-03). This record **does not** authorize implementation, A3, or changes to A4/T7/MVR disposition.
