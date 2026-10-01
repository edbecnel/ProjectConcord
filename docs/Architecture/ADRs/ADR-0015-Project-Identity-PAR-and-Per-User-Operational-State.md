# ADR-0015: Project Identity, Project Root, and Per-User Operational State

> **Filename note:** Path `ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md` is retained for repository link stability. The document title was reconciled post–Stage 1/2 and **Accepted ADR-0013** (2026-09-29); **“PAR Boundaries”** is removed from the title because historical **Project Architect Relay (PAR)** is not a monolithic component ([ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §9).

## Status

Accepted

## Date

2026-09-28 (integrated 2026-09-28; reconciled 2026-09-29; **Accepted by Project Architect 2026-09-29**)

## Scope

This ADR specializes **Core (A) application-shell** identity, session locator, and per-user operational persistence for ProjectConcord:

1. **ProjectConcord Project ID** — stable logical identity and operational partitioning key;
2. **Project Root** — filesystem **session locator** for the current working context;
3. Separation of Project ID from path, Git/repository labels, domain, Working Environment, provider session, and persona/UI workspace;
4. Locator **reconciliation** and relocation principles;
5. **Per-user operational state** direction (Recent Project Roots, session provenance, advisories);
6. **SQLite / OS application-data** persistence direction (solo degenerate case);
7. **`.projectconcord/`** creation discipline ([ADR-0004](ADR-0004-Derived-Data-and-Cache.md));
8. **Tier 0** shallow canonical-awareness boundary (not an EDF engine).

**What a Project represents** semantically (engineering undertaking; `Project != Domain != Working Environment`) is normative in [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md). This ADR does **not** redefine that model; it binds **identity mechanics**, **session locator**, and **operational store partitioning**.

This ADR **does not** own:

- monolithic **PAR** or **Governed Interaction Relay** ([ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §7, §9);
- Software Development governance packages, **DevelopmentWorkAuthorization**, **HumanInitiatedWorkItem**, submissions, IPH/CPD ([ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) **Accepted** 2026-09-29);
- **Working Environment** facets (**D**);
- provider/tool governance semantics (**E** — see [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md));
- **Actor / Role / RoleAssignment** ([PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) **Proposed**);
- **Project Work Record** or generic work identity ([ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md));
- **single-Project Focus** product-shell rules and multi-instance launch (see [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md), ADR-0016, [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)).

Normative product requirements for relay behavior remain in [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md). Software Development workflow requirements remain in [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md).

## Context

[M1 / EGR-G1](https://github.com/edbecnel/ProjectConcord/commit/c08af261ff323a0ddd54a84bd5c8b990a49fa84f) delivered a solution skeleton with in-memory [Project Root](../../src/Edf.Domain/Projects/ProjectRoot.cs) selection only. [AAR-0001](../Audits/AAR-0001-m1-solution-skeleton-conformance.md) Finding 14 deferred Recent Project Root history and relay/workflow to future architecture.

Project Architect disposition (**2026-09-28**) accepts the [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md) **with binding amendments** and authorizes **architecture/documentation canonicalization only** for that tranche. **That tranche did not authorize A1–A4 implementation**, M2+, SQLite implementation, `.projectconcord/` creation on open, or Cursor bridge implementation.

Subsequently, **PAR track A1** was **separately authorized, implemented, and published** (2026-09-28; see [Implementation Roadmap](../../Development/Implementation_Roadmap.md), [GAP-043](../../Development/EDF_Gap_Register.md)). Eventual **acceptance of this ADR** would **ratify** the architectural direction already exercised by A1; it does **not** retroactively authorize A1.

[ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) and [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md) **Accepted** 2026-09-29. [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) **Accepted** 2026-09-29. This ADR was **Accepted by the Project Architect on 2026-09-29** following reconciliation review.

## Terminology (not synonyms)

| Term | Layer | Meaning in this ADR |
|---|---|---|
| **ProjectConcord Project ID** | **A** | Stable logical project identity ([ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §1); operational partition key |
| **Project Root** | **A** session | Currently selected absolute filesystem **session locator** — not durable identity |
| **Single-Project Focus** | **A** product shell | One application instance maintains **one active** Project context at a time ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)) |
| **Concurrent Project Work** | **A** product shell | Multiple application instances MAY run concurrently; each instance owns one active Project ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)) |
| **Cross-project awareness** | **A** / **B** | References, dependencies, and governed inter-project handovers — **not** multi-project workspace ownership ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md), [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) §7) |
| **Working Environment** | **D** | Composable policy/methodology facets — **not** Project Root or multi-project hosting |
| **Provider / agent session** | **E** | Adapter session state ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)) |
| **Persona / UI workspace** | UI | UX layout — not governance identity |
| **Historical PAR** | Umbrella term | Maps to relay **A** + software package **B** + policy **D** + adapters **E** — not owned by this ADR |

## Decision

### 1. Identifier separation (binding)

ProjectConcord MUST treat the following as **distinct** concepts — not interchangeable:

| Concept | Role |
|---|---|
| **ProjectConcord Project ID** | Stable logical project identity for operational state, workflow continuity, and recent-project history |
| **Project Root** | Currently selected filesystem path (**session locator**) |
| **Repository identity** | VCS/working-copy identity (for example Git work tree) |
| **Git remote URL** | Network location hint; may change or multiply |
| **Repository / workspace name** | Human or tool label; not authoritative identity |

Filesystem path is a **locator**, not durable project identity. Repository move, rename, or reopen MUST NOT destroy workflow continuity when the user (or reconciliation flow) associates the new locator with the same ProjectConcord Project ID.

Identity persistence and mapping MUST NOT require creating [`.projectconcord/`](ADR-0004-Derived-Data-and-Cache.md) merely to establish Project ID.

**Relationship to [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md):** ADR-0016 defines what a **Project** is (engineering undertaking; independent of domain, methodology, Working Environment, tools, providers). This section defines **identifier and locator binding** and **operational partitioning** — not domain enablement or Working Environment configuration.

### 2. Per-user operational persistence (architecture direction)

For M1–M5 degenerate (local solo) operation, **per-user ProjectConcord application data** SHALL host:

- a **SQLite** database (architecture direction — **A1 implemented and published** under separate authorization; acceptance of this ADR ratifies direction, not retro-authorization);
- partitions for **project-scoped operational state** keyed by **ProjectConcord Project ID**;
- per-user records such as Recent Project Roots, session/provenance metadata, and advisories.

The database MUST NOT live inside the opened repository. Migration, schema versioning, backup, and recovery implications MUST be documented before further implementation tranches ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)).

This extends [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) operational data classification; shared cloud operational store remains M6+.

**Project Work Record:** Operational records may eventually correlate with [Project Work Record](ADR-0017-Project-Work-Record-Core-Boundary.md) identifiers within the same Project ID partition. This ADR does **not** define PWR schema, lifecycle, relationships, or APIs.

### 3. `.projectconcord/` creation (binding)

Opening or selecting a Project Root MUST NOT create `.projectconcord/`.

M2 discovery/validation is **not** an automatic creation trigger.

`.projectconcord/` MAY be created only when an **explicitly authorized feature** requires project-local derived state and that feature owns and justifies the state ([ADR-0004](ADR-0004-Derived-Data-and-Cache.md)). Optional explicit user initialization MAY be added later.

### 4. Historical PAR and canonical component owners (reference only)

**Project Architect Relay (PAR)** remains acceptable as **historical/umbrella** terminology in [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) and legacy plans. **This ADR does not define monolithic PAR.**

Accepted Stage 1 decomposition ([ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [AMD-0003](../AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)):

| Layer | Owner | Responsibility |
|---|---|---|
| **A — Core** | [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) | **Governed Interaction Relay** — generic package/correlation identity, validation, provenance, relay-boundary STOP hooks |
| **B — Software Development** | [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) **Accepted** | Software governance package/profile; handoff vs **DevelopmentWorkAuthorization**; software execution context in packages |
| **D — Working Environment / policy** | [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §5 | Supervision, checkpoints, escalation where configured |
| **E — Provider adapters** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) | `IProjectArchitectProvider`, manual ChatGPT formatting, `CursorBridge`, transport rendering |

**Governance / workflow semantics** (canonical vs operational, STOP, handover vs authorization) are defined in [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) and [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (**B** / **F** where EDF applies). This ADR **consumes** those boundaries at relay/session boundaries; it does **not** redefine **DWA**, **HIW**, **ArchitecturalReviewSubmission**, **IPH**, or **CPD**.

**Core governance semantics MUST NOT depend on the ChatGPT product or OpenAI API.** Human-mediated ChatGPT is a **current provider configuration**, not a permanent architectural constraint.

### 5. Canonical Markdown awareness — Tier 0

Before M2 EDF engine depth, **Tier 0** awareness is deliberately shallow: Git HEAD/baseline, known canonical paths, document existence, narrowly defined governance/status metadata, recognized governance documents, and metadata needed to support **safe relay/session context** and governed workflow packages when authorized.

Tier 0:

- MAY provide enough repository/governance context for relay/session safety;
- MUST NOT become a competing EDF parser/validator or second canonical interpretation engine;
- MUST NOT duplicate M2+ EDF Engine discovery, profile resolution, or conformance validation ([SPEC-001](../../Specifications/features/SPEC-001-mvp-edf-desktop-client.md)).

Product behavior for Tier 0 when implemented is specified in [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) (PC-PAR-T0).

### 6. Relationship to ADR-0013 (Accepted)

[ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) is **Accepted** (2026-09-29). Software Development governed workflow semantics — including **DevelopmentWorkAuthorization**, **HumanInitiatedWorkItem** (normative SPEC-004 name), **ArchitecturalReviewSubmission**, software implementation evidence, **InterProjectHandover**, and **CrossProjectDependency** — are owned by **ADR-0013** / **SPEC-004**, not this ADR.

This ADR provides **Project ID**, **Project Root session**, and **per-user operational partitioning** that those workflows **consume** when implemented. **Accepted** 2026-09-29 (Project Architect).

### 7. Actor / Role dependencies (deferred)

Normative **Actor**, **Role**, and **RoleAssignment** semantics remain **deferred** ([PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) **Proposed**). **Unresolved PCON-0002 did not block** reconciliation or **Acceptance** of this ADR (Project Architect disposition, 2026-09-29).

Until disposition:

- Packages MAY use **provisional transport attribution** (for example “Project Architect”, “Engineering Agent”) per [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §13;
- Full actor-attributed provenance for authorization/validation remains **deferred**;
- This ADR does **not** define multi-user membership, roles, or separation-of-duty policy.

### 8. Implementation staging (planning only — historical + current)

| Stage | Scope | Authorized by **this ADR document** (2026-09-28 tranche) | Subsequent status |
|---|---|---|---|
| **A0** | Architecture / canonical docs | Documentation only | **Closed** |
| **A1** | Per-user app state + Recent Project Roots | **No** — separate PAR tranche | **Published** 2026-09-28 under separate PA authorization ([A1 plan](../../Handover/ProjectConcord-A1-Implementation-Plan.md)) |
| **A2** | Manual P0 governed interaction relay | **No** | **Not authorized** |
| **A3** | Governed workflow MVP (manual) | **No** | **Not authorized** |
| **A4** | Cursor bridge P1+ | **No** | **Not authorized** |
| **M2** | EDF discovery track | **No** | Separate governance |

Acceptance of **this ADR** does **not** authorize A2–A4 or M2+.

## Alternatives Considered

### Path-keyed operational partitions only

- Advantages: Simple M1 mapping.
- Disadvantages: Breaks continuity on rename/move; conflicts with PA binding amendment.
- Reason not selected: Rejected by Project Architect disposition.

### Store operational SQLite inside repository

- Advantages: Portable with clone.
- Disadvantages: Commingling with Git; collaborator merge noise; blurs canonical boundary.
- Reason not selected: Rejected — per-user app data per ADR-0009 degenerate case.

### Retain “PAR Boundaries” as ADR title scope

- Advantages: Matches legacy PAR plan naming.
- Disadvantages: Implies monolithic PAR ownership after Stage 1 decomposition.
- Reason not selected: Reconciled post–Stage 1/2 (2026-09-29).

## Consequences

### Positive

- Clear **Project ID vs Project Root session locator** model for recent roots, reconciliation, **single-Project Focus**, and **multi-instance** concurrent work ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)).
- Provider-neutral boundaries remain specified in SPEC-006; this ADR does not collapse them into a single PAR component.
- Preserves PA-3 / M1 `.projectconcord/` discipline.

### Negative

- Identity reconciliation UX and extended schema design remain deferred beyond A1.
- SQLite migration/recovery documentation obligations continue for future tranches.

### Risks

- Premature Actor/Role modeling — mitigated by explicit PCON-0002 deferral and provisional attribution ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)).

## References

- [SPEC-006 — Project Root, identity, and governed workflow relay](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [ProjectConcord PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md)
- [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [AMD-0003](../AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)
- [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (**Accepted** 2026-09-29)
- [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md) (PWR — no material dependency)
- [ADR-0004](ADR-0004-Derived-Data-and-Cache.md), [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) (**Proposed**)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
- [ADR-0015 Reconciliation Documentation Tranche Plan](../../Handover/ProjectConcord-ADR-0015-Reconciliation-Documentation-Tranche-Plan.md)
- [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) (**Accepted** 2026-10-01 — product shell instance model, local-first/sync/concurrency; reconciles terminology with this ADR)

### ADR-0015 acceptance (2026-09-29)

**Accepted** by Project Architect following reconciliation review. Acceptance **ratifies** the identity and per-user operational direction exercised by published PAR track **A1**; it does **not** retroactively authorize A1, **A2**, or SPEC-006 reconciliation.

## Maintenance

**Accepted** 2026-09-29 (Project Architect). Reconciliation tranche closed per [ADR-0015 Reconciliation Documentation Tranche Plan](../../Handover/ProjectConcord-ADR-0015-Reconciliation-Documentation-Tranche-Plan.md). Update when identity/persistence architecture materially changes. Acceptance does **not** authorize **A2**, SPEC-006 amendment, or PCON-0002 resolution.
