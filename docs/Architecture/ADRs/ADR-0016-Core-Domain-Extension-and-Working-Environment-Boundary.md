# ADR-0016: Core, Domain Extension, and Working Environment Boundary

## Status

Accepted

## Date

2026-09-29

## Context

Stage 1 architecture analysis and repository investigation established that ProjectConcord must remain engineering-domain neutral at Core while supporting multiple domains/extensions per project, composable Working Environments, and decomposed relay/authorization concerns previously bundled under **PAR** and **DevelopmentWorkAuthorization**. See [AMD-0003](../AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md).

[ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) **Accepted** 2026-09-29; [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) remains **Proposed**; this ADR does not accept ADR-0015.

## Decision

1. **Project identity** — A ProjectConcord Project represents the engineering undertaking. Project identity is independent of engineering domain, extension, methodology, working environment, tools, and providers. Project types such as `SoftwareProject` / `ElectricalProject` as fundamental identity are **rejected**.

2. **Multi-domain projects** — A project may **enable** multiple domains/extensions without creating a new ProjectConcord Project ID. Enablement is **mutable**. Disabling a domain must not erase or reinterpret historical domain work, evidence, provenance, or configuration context.

3. **Domain lifecycle (conceptual)** — Distinguish **Available**, **Enabled (for project)**, **Activated (for working context)**, and **Disabled (for project)**. **Enabled** and **Activated** are not equivalent. Physical extension loading and persistence are deferred.

4. **Working context activation** — A working context may activate one or many enabled domains simultaneously. Multi-domain activation is **permitted**, not required as the default.

5. **Working Environment** — Working Environment is separate from Domain/Extension. It is modeled as a **composable configuration of capabilities/facets**, not a single mutually exclusive methodology enum. Built-in environments are **presets**; custom compositions are permitted.

6. **Configuration mutability and provenance** — Current working-environment and domain-enablement configuration may change without changing Project identity. Historical governance and work events must remain interpretable under the effective configuration context at the time of the event (implementation deferred).

7. **Core boundary** — Core provides domain-neutral project identity, operational partitioning, neutral domain lifecycle state, EDF governance consumption (without redefinition), **Governed Interaction Relay** (generic package/correlation, validation, provenance, relay-boundary semantics), and conceptual UI hosting. Core must not depend on domain-specific engineering semantics.

8. **Software Development / Engineering Extension boundary** — Software-specific execution authorization (repository term **DevelopmentWorkAuthorization** until migrated), software governance package/profile content, and repository-scoped handoff/execution semantics belong to the extension, consuming Core services.

9. **PAR decomposition** — **Project Architect Relay (PAR)** may remain historical terminology. Architecturally, relay concerns split across: Core **Governed Interaction Relay**; Software Development package/profile; working-environment/policy supervision; provider adapters. No single component owner for the historical PAR umbrella.

10. **Delegation decomposition** — A domain-neutral generic delegation/authority primitive (working name *AuthorityGrant* — **not** a finalized type name; **no schema** in this tranche) is conceptually **Core**. Software execution authorization, supervision/checkpoint policy, and provider-rendered instructions are **not** Core semantics.

11. **Work Item** — Generic Work Item deferral is **architecturally resolved** by Stage 2 ([ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md) **Accepted** 2026-09-29, [AMD-0004](../AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md)): Core owns a thin **Project Work Record** coordination capability (`ProjectWorkRecord` — **working name only**), **not** a universal task-management entity. Persistence, APIs, UI, lifecycle schema, and relationship taxonomy remain **deferred** (implementation not authorized).

12. **Provider neutrality** — Governance semantics must not depend on ChatGPT, Cursor, or OpenAI API. Reference adapters are permitted.

13. **Deferred mechanics** — Physical extension/plugin loader, manifests, discovery, UI composition APIs, and environment persistence schemas remain deferred.

14. **A2** — PAR track milestone **A2** is reframed in documentation as manual P0 **governed interaction relay** with decomposed ownership. **A2 implementation is not authorized** by this ADR.

## Alternatives Considered

### Single-owner PAR in Core

- Advantages: Matches pre–Stage 1 documentation simplicity.
- Disadvantages: Collapses software, policy, and adapter concerns; conflicts with domain-neutral Core.
- Reason not selected: Rejected by Stage 1 PA disposition.

### Project type per engineering discipline

- Advantages: Simple filtering and UX.
- Disadvantages: Breaks multi-domain undertakings; conflates identity with domain.
- Reason not selected: Rejected by Stage 1 PA disposition.

## Consequences

### Positive

- Clear path for additional engineering domains without Core fork.
- Relay and authorization concerns can evolve independently.
- EDF remains authoritative; extensions operationalize without redefinition.

### Negative

- Documentation migration burden (SPEC-006, PAR plan, roadmap).
- Multiple Proposed ADRs until separate acceptance tranches.

### Risks

- Implementers may prematurely build monolithic PAR/DWA before decomposition lands in code — mitigated by explicit A2 not authorized.

## References

- [AMD-0003](../AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)
- [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (**Accepted** 2026-09-29)
- [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (Proposed)
- [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) (Proposed discovery)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [Stage 1 Documentation Tranche Plan](../../Handover/ProjectConcord-Stage-1-Documentation-Tranche-Plan.md)
- [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md), [AMD-0004](../AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md), [Stage 2 Documentation Tranche Plan](../../Handover/ProjectConcord-Stage-2-Documentation-Tranche-Plan.md)
