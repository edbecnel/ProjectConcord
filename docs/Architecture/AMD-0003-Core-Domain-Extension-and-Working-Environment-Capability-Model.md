[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › AMD-0003

# AMD-0003: Core, Domain Extension, and Working Environment Capability Model

> **Status:** Architectural Amendment — integrated 2026-09-29 (Project Architect accepted)  
> **Document ID:** AMD-0003  
> **Date:** 2026-09-29  
> **Owner:** ProjectConcord  
> **Normative decisions:** [ADR-0016](ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) (Accepted 2026-09-29)  
> **Companion:** [Stage 1 Documentation Tranche Plan](../Handover/ProjectConcord-Stage-1-Documentation-Tranche-Plan.md)

Integrated architectural amendment establishing the **Stage 1** capability model for ProjectConcord Core, engineering **domain/extensions**, and **Working Environments**. Normative decision summary: ADR-0016. This document does **not** authorize implementation (including PAR track **A2**).

---

## 1. Purpose

ProjectConcord must support multiple engineering disciplines within one **Project** without conflating project identity with domain, methodology, tools, or working environment. Software Development is the first substantial **domain extension** reference, not the definition of what a Project is.

This amendment records accepted Stage 1 architecture after repository investigation and Project Architect disposition (2026-09-29). It reconciles prior PAR-centric documentation with decomposed ownership while preserving historical terminology where needed.

## 2. Architectural invariants

| Invariant | Statement |
|---|---|
| **I1** | `Project != Domain` |
| **I2** | `Project != Extension` |
| **I3** | `Project != Methodology` |
| **I4** | `Project != Working Environment` |
| **I5** | A **ProjectConcord Project** represents the engineering undertaking. |
| **I6** | One project may enable multiple domains/extensions without changing Project identity. |
| **I7** | **Enabled** domain/extension is not the same as **Activated** for the current working context. |
| **I8** | Disabling a domain must not erase or reinterpret historical domain work, evidence, provenance, or configuration context. |
| **I9** | Current configuration is mutable; historical governance/work provenance is durable and must remain interpretable under the **effective configuration context** at the time of the event (conceptual — persistence design deferred). |
| **I10** | ProjectConcord **Core** must remain ignorant of domain-specific engineering semantics. |
| **I11** | Extensions may consume or project **EDF** governance facts; they must not redefine EDF semantics ([ADR-0002](ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)). |
| **I12** | Provider/tool transport is separate from governance semantics. ChatGPT and Cursor are **reference** providers/tools, not universal architectural identities. |

## 3. Domain and extension lifecycle

Conceptual states (final names and persistence deferred):

| State | Meaning |
|---|---|
| **Available** | Domain/extension exists in product catalog or could be offered. |
| **Enabled (for project)** | Project has opted in; configuration and history may exist. |
| **Activated (for working context)** | Domain/extension is in focus for the user's current working context (runtime/UI load deferred). |
| **Disabled (for project)** | No longer offered for new work; historical records retained. |

**Activation cardinality:** A working context may activate **one or many** enabled domains simultaneously. Multi-domain activation is **permitted**, not mandated as the default (default UX/policy deferred).

**Lazy activation:** ProjectConcord must not require every enabled domain to fully initialize on application start (physical loading mechanics deferred).

## 4. Multi-domain and cross-domain projects

Projects may combine domains (for example Electrical + Firmware/Software + Mechanical). Cross-domain work shares **project-level** EDF canonical state in Git. Domain-specific operational data remains partitioned by domain extension semantics under the same **ProjectConcord Project ID**.

Full cross-domain workflow and acceptance semantics are **deferred**; Stage 1 only ensures identity and lifecycle semantics do not foreclose them.

## 5. Working Environment

**Working Environment** is distinct from **Domain/Extension**.

A Working Environment is architecturally a **composable configuration of capabilities/facets** (methodology, work management, workflow, authority/delegation, supervision, escalation, org/project policy, verification behavior, tool bindings, domain-specific capabilities). It is **not** a single mutually exclusive enum (for example Scrum | Kanban | Architect/Developer | Custom).

- **Built-in environments** are **predefined compositions/presets**.
- **Custom** environments are project/user-defined compositions.
- Compositions may mix facets (for example Kanban visualization + Architect/Developer supervision + selected Scrum planning practices).

Object model, persistence, and UI remain **deferred**.

## 6. Capability ownership (working A–F classification)

| Category | Label | Scope |
|---|---|---|
| **A** | ProjectConcord Core — engineering-domain neutral | Project identity, locator reconciliation, neutral domain lifecycle, operational partitioning, EDF read projections, generic **Governed Interaction Relay**, conceptual UI host, generic delegation primitive (semantics only — see §8) |
| **B** | Software Development / Engineering Extension | Software execution authorization profile, software governance package content, repository/Git evidence semantics, software Tier-0+ snapshot rules |
| **C** | Methodology-specific capability/configuration | Sprint cadence, ceremony semantics, Kanban column/WIP rules when methodology-specific |
| **D** | Organization/project policy | Supervision intensity, assignment policy, escalation thresholds |
| **E** | Provider/tool adapter/integration | ChatGPT/Cursor field rendering, manual transport formatting, future MCP/CLI |
| **F** | EDF-governed upstream fact projected into ProjectConcord | EGR, ADR, AAR, MVR, GDO, GMR, etc. |

**Deferred:** Universal **Work Item** ownership — requires bounded Stage 2 analysis; do not assume software backlog semantics generalize to all engineering disciplines ([PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) investigation themes).

## 7. Core responsibility boundary (summary)

Core owns the application/project **shell**, **ProjectConcord Project ID**, per-user operational store partitioning, neutral **enabled/activated/disabled** lifecycle facts, canonical vs operational boundary enforcement, **Governed Interaction Relay** (package/correlation identity, generic validation/provenance, relay-boundary STOP hooks), EDF governance **consumption** (not redefinition), and **conceptual** surfaces for domain UI contribution (physical composition API deferred).

Core does **not** own software-specific implementation authorization semantics, repository-agent routing profiles, or methodology-specific work-management entities.

## 8. Software Development / Engineering Extension boundary (summary)

The extension owns software **execution authorization** semantics (today often discussed under **DevelopmentWorkAuthorization** — repository terminology until a governed migration), software **governance package/profile** content, handoff/execution semantics, and software-appropriate evidence correlation. It consumes Core relay and project identity services.

## 9. PAR decomposition and terminology

**Project Architect Relay (PAR)** remains acceptable as **historical/umbrella** terminology in legacy documents.

Accepted target decomposition:

| Owner | Responsibility |
|---|---|
| **Core** | **Governed Interaction Relay** — generic relay, package/correlation identity, generic validation/provenance, applicable relay-boundary semantics |
| **Software Development Extension** | Software governance package/profile content; software handoff/execution semantics |
| **Working Environment / project policy** | Supervision, checkpoints, escalation behavior where configured |
| **Provider adapter** | ChatGPT-, Cursor-, or other provider-specific transport/rendering |

Avoid destructive terminology replacement; map historical PAR references to this decomposition in amended normative docs.

## 10. DevelopmentWorkAuthorization decomposition

**DevelopmentWorkAuthorization** remains existing repository terminology until a separately governed migration.

Architectural decomposition:

| Layer | Concern |
|---|---|
| **Core (generic)** | Domain-neutral delegation/authority primitive — grant/delegation, authority source, scope, bounds, duration/expiry where applicable, evidence obligations, constraints, applicable operation capabilities, revocation/change, escalation relationship, independence/separation constraints (**working name** *AuthorityGrant* — **not** a finalized type name; **no schema** in this tranche) |
| **Software extension** | Software-specific execution authorization |
| **Working Environment / policy** | Supervision, escalation, checkpoint configuration |
| **Provider adapter** | Rendered execution instructions for agents |

Acceptance of [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) remains a **separate** PA decision; ADR-0016 does not subsume ADR-0013.

## 11. EDF governance consumption

Extensions and Core **project** applicable EDF facts (category **F**). Mandatory gate semantics, MVR human attestation rules, GDO dimensions, and canonical artifact integrity remain governed by EDF and ProjectConcord specs aligned to EDF — not redefined by domain extensions.

## 12. UI contribution boundary

Core may **host** domain-contributed UI conceptually. Physical plugin APIs, extension discovery, manifests, and rendering contracts remain **deferred**.

## 13. Physical extension mechanics (explicitly deferred)

Plugin/extension loader, package format, binary modularization, discovery, and runtime loading are **out of scope** for Stage 1 and this documentation tranche.

## 14. PAR track A2 (documentation reframe only)

Milestone **A2** identity is retained. **Implementation not authorized.**

Reframed scope: **manual P0 governed interaction relay** with decomposed ownership (Core relay + software package profile + provider manual transport). See [Implementation Roadmap](../Development/Implementation_Roadmap.md) and [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md).

## 15. Reconciliation with discovery records

[PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) remains **Proposed** discovery — referenced for Actor/Role and multidisciplinary themes, **not** promoted wholesale as this Stage 1 artifact.

[ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) and [ADR-0015](ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) remain **Proposed**; reconciled in place without status elevation in this tranche.

Closed governance evidence (EGR-G0/G1, AAR-0001, A1 closeout) is **not** retroactively rewritten.

## 16. Related documents

- [ADR-0016](ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)
- [System Architecture Overview](System_Architecture_Overview.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [PAR Workflow Architecture Plan](../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md)
- [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md)

## Maintenance

Stage 1 documentation tranche **closed / Project Architect accepted** (2026-09-29). Update when Stage 2 bounded analyses complete or consequential architecture changes.
