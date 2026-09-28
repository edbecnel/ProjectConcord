# ADR-0015: Project Identity, PAR Boundaries, and Per-User Operational State

## Status

Proposed

## Date

2026-09-28

## Context

[M1 / EGR-G1](https://github.com/edbecnel/ProjectConcord/commit/c08af261ff323a0ddd54a84bd5c8b990a49fa84f) delivered a solution skeleton with in-memory [Project Root](../../src/Edf.Domain/Projects/ProjectRoot.cs) selection only. [AAR-0001](../Audits/AAR-0001-m1-solution-skeleton-conformance.md) Finding 14 deferred Recent Project Root history and PAR/workflow to future architecture.

Project Architect disposition (**2026-09-28**) accepts the [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md) **with binding amendments** and authorizes **architecture/documentation canonicalization only**. **A1–A4, M2+, SQLite implementation, `.projectconcord/`, and Cursor bridge implementation are not authorized.**

[ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) remains **Proposed** — reconciled here without acceptance. Normative product behavior for PAR is defined in [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md). [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) remains the broad governed-development workflow spec; SPEC-006 specializes PAR without duplicating PC-AIGOV-001–028.

[PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) remains a **dependency** for normative Actor/Role semantics. PAR may use **provisional transport attribution** (for example “Project Architect”, “Engineering Agent”) until PCON-0002 is dispositioned.

## Decision

### 1. Identifier separation (binding)

ProjectConcord MUST treat the following as **distinct** concepts — not interchangeable:

| Concept | Role |
|---|---|
| **ProjectConcord Project ID** | Stable logical project identity for operational state, workflow continuity, and recent-project history |
| **Project Root** | Currently selected filesystem path (workspace locator) |
| **Repository identity** | VCS/working-copy identity (for example Git work tree) |
| **Git remote URL** | Network location hint; may change or multiply |
| **Repository / workspace name** | Human or tool label; not authoritative identity |

Filesystem path is a **locator**, not durable project identity. Repository move, rename, or reopen MUST NOT destroy workflow continuity when the user (or reconciliation flow) associates the new locator with the same ProjectConcord Project ID.

Identity persistence and mapping MUST NOT require creating [`.projectconcord/`](ADR-0004-Derived-Data-and-Cache.md) merely to establish Project ID.

### 2. Per-user operational persistence (architecture direction)

For M1–M5 degenerate (local solo) operation, **per-user ProjectConcord application data** SHALL host:

- a **SQLite** database (architecture direction — **implementation not authorized** by this ADR);
- partitions for **project-scoped operational state** keyed by **ProjectConcord Project ID**;
- per-user records such as Recent Project Roots, session/provenance metadata, and advisories.

The database MUST NOT live inside the opened repository. Migration, schema versioning, backup, and recovery implications MUST be documented before implementation ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)).

This extends [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) operational data classification; shared cloud operational store remains M6+.

### 3. `.projectconcord/` creation (binding)

Opening or selecting a Project Root MUST NOT create `.projectconcord/`.

M2 discovery/validation is **not** an automatic creation trigger.

`.projectconcord/` MAY be created only when an **explicitly authorized feature** requires project-local derived state and that feature owns and justifies the state ([ADR-0004](ADR-0004-Derived-Data-and-Cache.md)). Optional explicit user initialization MAY be added later.

### 4. Architectural component boundaries

| Layer | Responsibility |
|---|---|
| **Governance / workflow semantics** | Canonical vs operational rules; STOP; authorization vs handover; ties to SPEC-004 / ADR-0013 when accepted |
| **Project Architect Relay (PAR)** | Package assembly, validation, relay-boundary STOP, evidence correlation, Cursor handover validation, workflow provenance |
| **Project Architect provider boundary** | Provider-neutral exchange contract (`IProjectArchitectProvider` or equivalent); capability negotiation |
| **Provider implementations** | Manual ChatGPT product adapter, future OpenAI API adapter, other providers — **transport only** |
| **Engineering agent bridge** | `CursorBridge` abstraction; P0 manual first ([AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)) |

**Core governance semantics MUST NOT depend on the ChatGPT product or OpenAI API.** Human-mediated ChatGPT is a **current provider configuration**, not a permanent architectural constraint.

### 5. Canonical Markdown awareness — Tier 0

Before M2 EDF engine depth, **Tier 0** awareness is deliberately shallow: Git HEAD/baseline, known canonical paths, document existence, narrowly defined governance/status metadata, recognized governance documents, and metadata needed to construct governed workflow packages.

Tier 0 MUST NOT become a competing EDF parser/validator. Deep semantic discovery belongs to EDF Engine / M2+ ([SPEC-001](../../Specifications/features/SPEC-001-mvp-edf-desktop-client.md)).

### 6. ADR-0013 disposition

ADR-0013 is **reconciled** with this ADR and SPEC-006 but **not accepted** in this tranche. Acceptance remains blocked until PCON-0002 Actor/Role dependencies are examined sufficiently to avoid freezing premature semantics.

### 7. Implementation staging (planning only)

| Stage | Scope | Authorized by this ADR |
|---|---|---|
| **A0** | Architecture / canonical docs (this tranche) | Documentation only |
| **A1** | Per-user app state + Recent Project Roots | **No** — separate PAR tranche |
| **A2** | PAR manual packages (P0 transport) | **No** |
| **A3** | Governed workflow MVP (manual) | **No** |
| **A4** | Cursor bridge P1+ | **No** |
| **M2** | EDF discovery track | **No** — separate governance |

## Alternatives Considered

### Path-keyed operational partitions only

- Advantages: Simple M1 mapping.
- Disadvantages: Breaks continuity on rename/move; conflicts with PA binding amendment.
- Reason not selected: Rejected by Project Architect disposition.

### Store operational SQLite inside repository

- Advantages: Portable with clone.
- Disadvantages: Commingling with Git; collaborator merge noise; blurs canonical boundary.
- Reason not selected: Rejected — per-user app data per ADR-0009 degenerate case.

## Consequences

### Positive

- Clear identity model for PAR, recent roots, and future multi-root workspaces.
- Provider-neutral Project Architect boundary enables future API adapters without PAR redesign.
- Preserves PA-3 / M1 `.projectconcord/` discipline.

### Negative

- Identity reconciliation UX and schema design deferred to implementation tranches.
- SQLite migration/recovery must be designed before A1.

### Risks

- Premature Actor/Role modeling — mitigated by PCON-0002 dependency and provisional attribution labeling.

## References

- [SPEC-006 — PAR, Project Root, and governed workflow relay](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [ProjectConcord PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md)
- [ADR-0004](ADR-0004-Derived-Data-and-Cache.md), [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
