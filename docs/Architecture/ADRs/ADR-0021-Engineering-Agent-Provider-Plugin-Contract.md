# ADR-0021: Engineering Agent Provider Plugin Contract

## Status

Accepted

## Date

2026-10-01 (Accepted by Project Architect — canonical documentation tranche; **no** `src/` implementation authorized)

## Context

PAR track **A4**, [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary), [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport), and [AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md) require reconciliation before any automated Engineering Agent transport implementation is **considered** for authorization.

A2 delivered **P0 manual** governed relay ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)) with a provider-neutral **Engineering Agent relay bridge** (`IEngineeringAgentRelayBridge` / `EngineeringAgentManualRelayBridge`) and **no** automated provider integration. [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) identified the need for multiple concrete Engineering Agent products without embedding product-specific integration in **Core (A)** or the **Software Development / Engineering extension (B)**.

Binding architecture already in force includes [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (B-layer governance; provider-neutral semantics), [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) (layering; Governed Interaction Relay in **A**), [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md) (work identity ≠ authority), [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) (local-first operational persistence; Single-Project Focus), and [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) (Attention / Next Action projections).

The Project Architect authorized this tranche to **canonicalize architecture and documentation only**. Physical plugin loading, provider SDK integration, and A4 implementation remain **not authorized**.

[PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) remains **Proposed** and does **not** block this ADR. Provisional transport attribution (**Project Architect**, **Engineering Agent**) per [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §13 remains in effect.

## Decision

### 1. Architectural layering

Engineering Agent integration follows this provider-neutral stack:

```text
ProjectConcord Core (A)
  → Governed Interaction Relay, validation, provenance, relay-boundary STOP
Software Development / Engineering extension (B)
  → Software relay profile, DWA/handover semantics at boundary (consume SPEC-004)
Application / infrastructure (hosting — not A or B domain logic)
  → Provider-neutral plugin registration, discovery, lifecycle, capability exposure, selection
Provider-neutral Engineering Agent provider plugin contract (E)
  → Automated transport/session operations + capability declaration
Engineering Agent relay bridge (E — render/parse)
  → Governed package representation for manual and automated paths
Concrete Engineering Agent provider plugins (E — product-specific)
  → SDKs, auth, IDE integration, provider mode mapping — names only inside plugin
```

**Core** and the **Software Development extension** MUST NOT depend on commercial provider names, APIs/SDKs, authentication mechanisms, IDE integration, provider-specific mode mappings, or provider-specific configuration.

### 2. Contract responsibilities (provider plugin)

The **Engineering Agent provider plugin contract** (working name; implementation types deferred) defines **automated provider behavior** separate from relay governance.

The contract **SHALL** support, at minimum:

| Responsibility | Rule |
|---|---|
| Capability declaration | Expose static or negotiated capabilities (manual support, automated transport, render protocol version, health/auth status facets as applicable) |
| Provider selection identity | Stable plugin identity for configuration and substitution (not a governance package id) |
| Automated forward | When eligible per [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-014, accept **already-validated** relay outputs and deliver them through provider transport without altering governance-critical fields |
| Automated result intake | Return **untrusted** provider text or structured payloads only through the same parse/validate path as manual import (relay bridge or shared importer) |
| Session mapping | Map governed **Engineering Agent** routing intent and session continuity to provider UI/session mechanics |
| Failure reporting | Report transport unavailability, authentication failure, forward failure, and equivalent errors in a provider-neutral form suitable for shell projection |
| Configuration boundary | Own provider-specific settings and secrets within plugin scope |

The contract **MUST NOT**:

- Issue, modify, or imply **DevelopmentWorkAuthorization**
- Override or clear relay **STOP**
- Bypass Core relay validation or substitute **INCOMPLETE** / **RejectedMalformed** dispositions
- Manufacture or infer governance-critical package fields from free prose or model output
- Treat provider chats or transcripts as canonical Project records or ProjectConcord source of truth
- Redefine relay package kinds, correlation identity, or provenance rules

### 3. Explicit non-responsibilities (remain in A / B / F)

| Concern | Owner |
|---|---|
| Package kinds, structural validation, correlation, provenance, relay STOP hooks | **A** — Governed Interaction Relay |
| Software profile validation, handover vs DWA, authorization disposition semantics | **B** — [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) / [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) |
| EDF canonical acceptance, gates, MVR attestation | **F** — consume only |
| Operator Attention / Next Action derivation rules | **A** shell projections — [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) |
| Generic plugin physical hosting (loaders, sandboxes) | **Deferred** — see §15 |

### 4. Relay bridge vs provider plugin (split)

**`IEngineeringAgentRelayBridge` MUST NOT become a permanent catch-all provider interface.**

| Concern | Owner | Examples |
|---|---|---|
| Relay representation, render, parse | **Engineering Agent relay bridge** (`IEngineeringAgentRelayBridge`) | `TryRenderValidatedHandover`, `TryParseEngineeringResult`; `projectconcord-relay-v1` |
| Provider capabilities | **Provider plugin contract** | Automated transport support, auth status, protocol features |
| Automated transport / session operations | **Provider plugin contract** | Forward validated handover to provider; receive result payload for import pipeline |

The relay bridge **MAY** be **composed** by a provider plugin implementation or invoked by application orchestration after Core validation. **Governance relay semantics MUST remain independent** of provider transport.

**P0 manual reference adapter:** `EngineeringAgentManualRelayBridge` MAY remain in `Edf.Application` as an **interim built-in reference P0 adapter** until plugin hosting is implemented. Relocation to a first-party plugin package is a **future** implementation decision, not required in this tranche.

### 5. Provider capability model

Capabilities are **provider-neutral** declarations consumed by application/infrastructure hosting and shell projections:

- **Manual paste path** supported (P0 fallback)
- **Automated transport** supported (A4+ when authorized)
- **Supported governed render protocol** major version (for example relay v1)
- Optional: authentication configured, transport reachable, provider session active

Capabilities **MUST NOT** encode governance authority. **Valid** package state **MUST NOT** be conflated with **actionable** when STOP is active ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md); A2 P0 behavior).

Runtime negotiation beyond static declaration remains **deferred** unless a future tranche extends [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-019.

### 6. Automated transport boundary

Automated Engineering Agent relay **MAY** occur only when **[A]** relay validation permits automated forward per PC-PAR-014 (including: not **INCOMPLETE** for governance-critical metadata; not **RejectedMalformed**; STOP and B-layer rules respected) **and** **[E]** the selected plugin declares automated transport capability.

The plugin **MUST** deliver the **same** rendered governed representation (or byte-equivalent contract) that manual P0 would produce for the same validated package — transport **MUST NOT** weaken machine JSON authority or inference-safety rules.

On automated forward failure, the governed package and validation state in operational persistence **MUST** remain unchanged except for optional **non-authoritative** operator/audit records defined in a future implementation tranche.

### 7. Provider session vs governed continuity

| Governed ProjectConcord continuity (**A**, persisted) | Provider session (**E**, external) |
|---|---|
| `RelaySessionContinuity`, `AgentSessionIntent`, advisories | Provider chat threads, IDE agent sessions |
| `GovernedCorrelationId`, package produce/consume events | Not canonical ([ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §14) |
| ProjectConcord-generated identifiers | Provider conversation IDs optional; never required in Core |

Provider plugins **MAY** map governed routing intent to provider-specific modes or UI. They **MUST NOT** write governance-critical relay fields from provider prose.

### 8. Provider selection and substitution

- **Selection:** Application/infrastructure chooses the active Engineering Agent provider plugin from registered plugins using capability match and configuration (per-user or application-global per [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §10).
- **Substitution:** Changing plugins **MUST NOT** rewrite persisted relay packages, correlation IDs, or validation history. Mid-correlation substitution **MUST** continue to use existing `GovernedCorrelationId` for the active Project.
- **Reference candidate:** A concrete product (for example Cursor IDE agent integration) **MAY** be the first **P1 reference** plugin for transport investigation ([AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)) without defining the architectural contract.

**Do not** create artificial multi-provider stub implementations solely to demonstrate neutrality.

### 9. Authentication, configuration, and security

- Provider credentials, OAuth tokens, API keys, and IDE auth **MUST** remain inside the **plugin / E** boundary or OS secure storage accessed only by the plugin host — **not** in Core or B-layer domain assemblies.
- Configuration **MAY** be application-global or per-Project per applicable architecture; it **MUST NOT** be stored as canonical Git/EDF content.
- Repo scope for automated operations **MUST** respect the active **Project Root** locator and future **B** authorization scope — plugins **MUST NOT** expand scope silently.
- Prompt-injection and untrusted paste risks apply to **all** import paths; automated intake **MUST** use the same structural validation pipeline as manual import.

### 10. Project scoping and local-first

- Automated and manual Engineering Agent operations **MUST** run in the context of the **active Project** ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) Single-Project Focus).
- Relay operational state **MUST** remain durable local-first through the service boundary ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)) without requiring provider availability.
- Provider-side storage is **not** ProjectConcord source of truth.

### 11. Failure / unavailability → Attention / Next Action

Per [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md):

- Transport unavailability, authentication failure, automated-forward failure, and equivalent plugin-reported conditions **SHOULD** appear in **Attention** (relay / provider facet).
- When P0 manual relay remains eligible, manual copy/export/import **SHOULD** normally appear as **Recommended** Next Action — not **Required**.
- Transport failure alone **MUST NOT** manufacture a governance-**Required** action.
- **Required** vs **Available** vs **Recommended** classes **MUST NOT** be collapsed.

### 12. PLAN / AGENT / DEBUG mapping

- **Governed routing intent** uses provider-neutral **Engineering Agent** modes: PLAN, AGENT, DEBUG ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §9–11).
- Provider plugins **MAY** map these intents to provider-specific capabilities (for example IDE agent modes).
- Plugins **MUST NOT** manufacture governance authority or escalate modes beyond validated package state.
- Operator UI requirements for DEBUG selection are **out of scope** for this tranche; domain may define DEBUG while a given P0 UI may expose a subset only — document at implementation time without new shell requirements here.

### 13. P0 manual fallback

P0 manual relay (human copy/export and paste/import) **SHALL** remain the provider-independent fallback and recovery path ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-014; [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md) discovery — manual fallback principle).

Automated transport **MUST** be optional per plugin capabilities. Degraded operation **MUST** preserve governed relay state and recommend manual relay when eligible.

### 14. Plugin hosting boundary (application / infrastructure)

Provider-neutral **plugin hosting** concerns — registration, discovery, lifecycle, capability exposure, provider selection — are **application/infrastructure** responsibilities.

Hosting **MUST NOT** be placed in **Core (A)** domain logic or **Software Development (B)** extension semantics.

This ADR **MAY** name conceptual hosting interfaces. **Physical mechanics remain deferred**, including:

- dynamic assembly loading
- process isolation and sandboxing
- supply-chain controls
- physical discovery mechanisms (filesystem, manifests, stores)

**No loader implementation** is authorized by this ADR.

### 15. Project Architect provider symmetry

Redesign of the **Project Architect** provider path ([`IProjectArchitectProvider`](../../Handover/ProjectConcord-A2-Implementation-Plan.md), manual `ChatGPT-Chat` serialization) is **out of scope**. Future symmetry **MAY** be recorded as a watch item; it does **not** block Engineering Agent plugin architecture.

### 16. A4 scope neutrality

**A4** architecture **MUST** remain provider-neutral. A concrete product **MAY** serve as the first/reference **P1** transport candidate ([AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)) without defining the contract.

**A4 implementation** and **A4 implementation plan** remain **not authorized** by this ADR.

## Consequences

- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §11 and PC-PAR-014 are reconciled to reference this contract for automated paths.
- [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) is **closed / resolved** by this ADR (architectural boundary and contract). Physical plugin hosting, loaders, and concrete provider implementations are **deferred implementation** concerns — not unresolved GAP-030 deficiencies — and require separate PA authorization.
- [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport) remains **open** for transport mechanism selection (P1/P2).
- [AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md) remains **Active** for reference-provider transport investigation under the reframed scope.
- Future `src/` work requires separate PA authorization for plugin hosting, provider plugins, and A4.

## Related Documents

- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md), [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md), [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)
- [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary), [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport), [AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)
- [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md) (historical provenance)
- [A2 Implementation Plan](../../Handover/ProjectConcord-A2-Implementation-Plan.md) (P0 delivered; automation deferred)
