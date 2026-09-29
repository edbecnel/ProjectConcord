[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › AMD-0004

# AMD-0004: Project Work Record and Coordination Capability Model

> **Status:** Architectural Amendment — integrated 2026-09-29 (Project Architect accepted)  
> **Document ID:** AMD-0004  
> **Date:** 2026-09-29  
> **Owner:** ProjectConcord  
> **Normative decisions:** [ADR-0017](ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) (Accepted 2026-09-29)  
> **Companion:** [Stage 2 Documentation Tranche Plan](../Handover/ProjectConcord-Stage-2-Documentation-Tranche-Plan.md)

Integrated architectural amendment establishing the **Stage 2** capability model for **Project Work Record** coordination in ProjectConcord Core. Normative decision summary: ADR-0017. This document does **not** authorize implementation (including schemas, APIs, UI, or PAR track **A2**).

---

## 1. Purpose

Stage 1 ([AMD-0003](AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md), [ADR-0016](ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)) deferred universal Work Item ownership. Stage 2 ([ADR-0017](ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) Accepted) resolves that deferral architecturally with a **thin** Core abstraction — not a task manager — so multi-domain projects can coordinate undertakings without software or methodology vocabulary in Core.

## 2. Project Work Record — semantic boundary

**Working name:** `ProjectWorkRecord`

A **Project Work Record** is a project-scoped, domain-neutral operational identity representing a declared unit of project undertaking or intended outcome that may coordinate activity across **zero, one, or multiple** engineering domains **without** defining:

- domain-specific meaning of the undertaking;
- methodology (backlog, sprint, Kanban, and similar);
- authorization or delegation;
- evidence semantics or payloads;
- scheduling;
- acceptance criteria.

It is **not** a universal task-management abstraction.

## 3. Essential vs optional vs excluded semantics

### 3.1 Essential Core (invariants)

| Semantic | Statement |
|---|---|
| Stable identity | Durable identifier within the ProjectConcord Project ID operational partition |
| Declaration | Concise human-meaningful title or declaration |
| Project scope | Bound to **ProjectConcord Project ID** |
| Creation provenance | Lightweight source/timestamp (and actor when Actor/Role model exists) |

### 3.2 Optional Core capabilities (deferred implementation)

| Semantic | Statement |
|---|---|
| Long-form description / scope | Extended narrative or structured scope reference |
| Configuration context provenance | Reference or snapshot of effective working/configuration context when needed for interpretability |
| Work-to-work relationships | Durable edges in a domain-neutral relationship capability (typed semantics deferred — see §7) |
| EDF references | Links to canonical or discovered EDF artifact identities — **reference only**, no SPEC-003 lifecycle on the work record |
| Evidence references | Opaque or typed refs; payloads owned outside Core |
| Authority references | Links to delegation/authority records — **no grant implied** |

### 3.3 Explicitly excluded from universal Core work ontology

Backlog semantics; sprint membership; story/epic/task types; story points; percent complete; default Developer/QA roles; priority/ranking; assignment/ownership; scheduling; domain-specific acceptance criteria; software repository/execution semantics.

Priority, assignment, and ranking are **not** Core invariants; their eventual A–F ownership remains **subject to later architecture** (not permanently assigned to Working Environment policy in Stage 2).

## 4. Domain association

A Project Work Record may:

| Pattern | Permitted |
|---|---|
| No domain association | Yes |
| Single domain association | Yes |
| Multiple domain associations | Yes |
| Historical association after domain disabled | Yes — retained for provenance |

**Enabled** domain state may constrain **present** operations (for example new software execution detail on a disabled extension). Disabling a domain must **not** erase or reinterpret historical work identity or provenance ([AMD-0003](AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md) **I8**).

## 5. Lifecycle and state

Stage 2 does **not** canonize Core lifecycle states (for example draft/active/closed/cancelled). Whether Core eventually requires minimal operative/historical state semantics, and the names of any states, remain **deferred**. Domain extensions and methodology facets may define richer states **over** Project Work Record identities without redefining Core type identity.

## 6. Boundaries vs authority, evidence, EDF, methodology

| Concern | Boundary |
|---|---|
| **Authority** | Work record = undertaking **exists**; **AuthorityGrant** (working name, no schema) and **DevelopmentWorkAuthorization** (**B**) = permission. Record **must not** grant authority; authority **not** required for record existence. |
| **Evidence** | Produced by or used to evaluate work; **not** work identity. Core references only; no generic Core evidence payload in Stage 2. |
| **EDF governance** | Gates, ADRs, SPECs, MVR, GDO, GMR, AWI, etc. remain **F**. Work records **reference**; they do not become governance artifacts. |
| **Methodology** | Facets (**C**) operate **on** record IDs (backlog column, sprint bucket) without defining Core record type. |

## 7. Work-to-work relationships

Core **accepts** a domain-neutral capability for **durable relationships between** Project Work Records.

**Deferred** (do not conflate in Stage 2):

- parent/child or decomposition;
- blocks/requires;
- sequencing;
- informational association;
- cross-domain relationship;
- inter-project dependency/routing.

**CrossProjectDependency** ([ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), **B** today) must **not** be generalized directly into the universal Core relationship model.

## 8. Operational intake vs Project Work Record

| Concept | Role |
|---|---|
| **HumanInitiatedWorkItem** (normative name in [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)) | Intake/capture/triage — **not** the generic Core work abstraction |
| **OperationalIntakeRecord** | Preferred **working terminology only** for future docs — **not** an accepted canonical type name |

Intake may: create a Project Work Record; associate with an existing record; merge into existing work; route elsewhere; or reject **without** creating a Project Work Record.

**OperationalIntakeRecord** Core-versus-extension ownership remains **deferred**. Stage 2 does **not** define a complete Core intake architecture.

## 9. Domain specialization pattern

```text
ProjectWorkRecord (Core — identity + coordination)
    |
    +-- SoftwareWorkDetail (B) — paths, branches, DWA, agent/execution, commits
    +-- Future EE / ME / ... extension details
```

Extension-specific detail is **not** required for a Core record to exist.

## 10. Effective-configuration provenance

**Mandatory architectural capability:** Material governed events whose historical interpretation depends on effective configuration **must** be able to preserve sufficient provenance to reconstruct that interpretation ([ADR-0017](ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md)).

- Not every Project Work Record mutation requires a full Working Environment snapshot.
- Mechanism (reference id, snapshot artifact, event log) — **deferred** schema design.

Aligns with Stage 1 configuration mutability ([ADR-0016](ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §6).

## 11. Updated A–F placement (work cluster)

| Concept | Category |
|---|---|
| **ProjectWorkRecord** | **A** |
| **PWR-to-PWR relationships** (generic capability) | **A** (typed semantics deferred) |
| **OperationalIntakeRecord** | **Deferred** (Core vs **B**) |
| **AuthorityGrant** (conceptual) | **A** — no schema |
| **DevelopmentWorkAuthorization**, software work detail, submissions | **B** |
| **InterProjectHandover**, **CrossProjectDependency** | **B** (not universal Core dependency) |
| Sprint, Kanban, backlog, WIP, story, epic, task, ranking | **C** — not Core |
| Priority, assignment (eventual) | Not Core — owner **deferred** |
| Provider work-list rendering | **E** |
| EDF artifacts, gates, MVR, etc. | **F** |

## 12. Multi-domain coordination (conceptual)

Example: embedded product change spanning EE, ME, and Software.

1. One **parent** Project Work Record (Core) states the cross-domain undertaking.
2. Per-domain extension details attach without forcing Scrum/task vocabulary in Core.
3. Relationship edges (deferred types) link parent/children or peers.
4. EDF program gates may reference multiple records without merging them into one gate artifact.

## 13. PAR, A2, and implementation

- **Governed Interaction Relay** may carry **optional** correlation to Project Work Record or authority ids in the **Software Development** package profile (**B**).
- **A2** has **no material dependency** on a completed generic work ontology; **A2 implementation remains not authorized**.

## 14. Reconciliation with Proposed ADRs

| ADR | Stage 2 effect |
|---|---|
| [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) | Remains **Proposed** — reconciliation appendix added; acceptance requires subsequent governed step |
| [ADR-0015](ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) | Remains **Proposed** — compatible with PWR keyed by Project ID |

## 15. Explicitly deferred

Final type names; persistence; APIs; UI; plugin mechanics; Actor/Role; AuthorityGrant/DWA schemas; Core lifecycle state names; relationship type taxonomy; OperationalIntakeRecord ownership; generic Core evidence payload; ADR-0013/0015 acceptance; A2 authorization.

## 16. Related documents

- [ADR-0017](ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md)
- [AMD-0003](AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)
- [ADR-0016](ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [System Architecture Overview](System_Architecture_Overview.md)
- [EDF Gap Register](../Development/EDF_Gap_Register.md) (GAP-046+)

## Maintenance

Stage 2 documentation tranche — **ADR-0017 Accepted** (2026-09-29); **pre-publication closeout** pending PA publication commit. Update when implementation is separately authorized.
