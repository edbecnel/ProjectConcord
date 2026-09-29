# ADR-0017: Project Work Record Core Boundary

## Status

Accepted

## Date

2026-09-29

## Context

[ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) deferred generic **Work Item** ownership pending bounded Stage 2 analysis. Stage 2 investigation and Project Architect disposition (**2026-09-29**) established that ProjectConcord requires a thin, engineering-domain-neutral operational identity for project undertakings without adopting a universal task-management ontology. This ADR was **accepted by the Project Architect on 2026-09-29** following Stage 2 documentation review. Companion capability model: [AMD-0004](../AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md). Tranche record: [Stage 2 Documentation Tranche Plan](../../Handover/ProjectConcord-Stage-2-Documentation-Tranche-Plan.md).

[ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) and [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) remain **Proposed**; this ADR does not accept them.

**ADR numbering:** `ADR-0017` is the ProjectConcord decision in [ADR index](README.md). Example `ADR-0017` identifiers in other documents (for example SPEC-003 illustrations) are unrelated fictional samples.

## Decision

1. **Project Work Record (working name)** — ProjectConcord **Core** adopts **`ProjectWorkRecord`** as a **working name only** for a project-scoped, domain-neutral operational identity representing a declared unit of project undertaking or intended outcome that may coordinate activity across zero, one, or multiple engineering domains **without** defining domain-specific meaning, methodology, authorization, evidence, scheduling, or acceptance semantics. It is **not** a universal task-management abstraction.

2. **Essential Core semantics** — Each Project Work Record SHALL support at minimum: stable project-scoped identity; concise declaration/title; **ProjectConcord Project ID** scope; lightweight creation/provenance. Persistence schema and APIs remain **deferred**.

3. **Optional Core capabilities** — Core MAY later support (not required for existence): long-form description/scope; effective working/configuration context provenance on records; durable relationships between Project Work Records; references to EDF artifacts; references to evidence; references to authority/delegation. Typed relationship semantics remain **deferred**.

4. **Explicit non-goals (Core work ontology)** — The following are **not** universal Core Project Work Record semantics: backlog; sprint membership; story/epic/task types; story points; percent complete; default Developer/QA roles; priority/ranking; assignment/ownership; scheduling; domain-specific acceptance criteria; software repository/execution semantics.

5. **Domain association** — A Project Work Record may have no domain association, one domain association, or multiple domain associations, and may retain historical associations with domains/extensions subsequently disabled. Current domain enablement may constrain present operations; disabling a domain must not rewrite or invalidate historical work identity or provenance ([ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) invariant alignment).

6. **Lifecycle** — Core does **not** canonize a generic lifecycle (for example draft/active/closed/cancelled). Whether Core requires minimal operative/historical state semantics, and the names of any such states, remain **deferred**.

7. **Authority separation** — A Project Work Record describes that an undertaking exists; it **must not** grant authority. Authority is **not** required for a Project Work Record to exist. **DevelopmentWorkAuthorization** remains Software Development extension semantics. The domain-neutral delegation primitive remains a **working name** *AuthorityGrant* with **no accepted schema**.

8. **Evidence** — Evidence may be produced by, associated with, or used to evaluate work; evidence is not the identity of the work. Core may reference evidence; domain and governance layers own evidence semantics and payloads. This ADR does **not** introduce a generic Core evidence payload.

9. **Methodology** — Methodology capabilities operate over Project Work Record identities without defining them. Sprint, backlog, Kanban, WIP, story, epic, task, ranking, and similar concepts are **not** Core semantics. Eventual ownership of priority, assignment, and related concerns outside Core is **deferred**.

10. **Effective-configuration provenance** — Material governed events whose historical interpretation depends on effective configuration **must** be architecturally capable of preserving sufficient effective-configuration provenance to reconstruct that interpretation. This does **not** require every Project Work Record mutation to embed a complete Working Environment snapshot; durable references or snapshots remain subject to later schema design.

11. **Intake distinction** — **HumanInitiatedWorkItem** (normative name in [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)) is **not** the generic Core project-work abstraction; it is primarily intake/capture/triage semantics. **OperationalIntakeRecord** is **preferred working terminology only** — not an accepted canonical type name; final Core-versus-extension ownership of intake remains **deferred**. An intake record may result in creation of, association with, merging into, routing toward, or rejection without creation of a Project Work Record.

12. **Relationships** — Core requires a domain-neutral capability for **durable relationships between** Project Work Records. Existing **CrossProjectDependency** must **not** be generalized directly into a universal Core dependency entity; specific relationship types (parent/child, blocks/requires, sequencing, informational association, cross-domain, inter-project routing) remain **deferred**.

13. **Domain specialization** — Domain extensions own domain-specific work semantics (for example Software Development repository paths, branches, DWA linkage, agent/execution semantics, commit correlation). Future engineering domains may add their own detail without changing Core.

14. **Implementation** — **Acceptance of this ADR does not authorize implementation.** Persistence schemas, APIs, UI, physical extension mechanics, Actor/Role schemas, authority schemas, generic lifecycle schemas, detailed relationship schemas, and **A2** remain **not authorized**.

15. **ADR-0016 §11** — Generic Work Item deferral in [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) is **architecturally resolved** by this decision and [AMD-0004](../AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md). Implementation of Project Work Record capabilities remains **deferred**.

## Alternatives Considered

### No universal Core work identity

- Advantages: Minimal Core surface; software-only coordination via DWA and EDF artifacts.
- Disadvantages: Weak cross-domain coordination; conflates permission with undertaking description.
- Reason not selected: Rejected by Stage 2 Project Architect disposition.

### Full task-management ontology in Core

- Advantages: Familiar PM UX patterns.
- Disadvantages: Violates engineering-domain neutrality; embeds software/methodology vocabulary in Core.
- Reason not selected: Rejected by Stage 2 Project Architect disposition.

## Consequences

### Positive

- Clear boundary between coordination identity, authority, evidence, methodology, and EDF governance artifacts.
- Multi-domain projects can share a neutral parent/coordination record without Scrum/Kanban in Core.
- ADR-0013 reconciliation can proceed in a subsequent governed step against documented A/B split.

### Negative

- Documentation migration (SPEC-004, legacy HIW naming, PCON-0001 references).
- Relationship and lifecycle schemas still require future tranches.

### Risks

- Implementers may prematurely build backlog/sprint tables on Core work IDs — mitigated by explicit non-goals and deferred schemas.

## References

- [AMD-0004](../AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md)
- [AMD-0003](../AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)
- [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)
- [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (Proposed)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [Stage 2 Documentation Tranche Plan](../../Handover/ProjectConcord-Stage-2-Documentation-Tranche-Plan.md)
