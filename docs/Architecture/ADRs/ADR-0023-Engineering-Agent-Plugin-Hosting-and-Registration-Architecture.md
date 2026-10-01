# ADR-0023: Engineering Agent Plugin Hosting and Registration Architecture

## Status

Accepted

## Date

2026-10-01 (Accepted by Project Architect — canonical documentation tranche; **no** `src/` implementation authorized)

## Context

[ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) (**Accepted**) defines the provider-neutral **Engineering Agent provider plugin contract** and assigns **application/infrastructure** ownership of plugin **registration, discovery, lifecycle, capability exposure, and selection** ([ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §14). Physical mechanics — dynamic loading, process isolation, supply-chain controls, filesystem discovery — were **deferred** as implementation concerns without reopening [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary).

[ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) (**Accepted**) defines automated **transport orchestration** and transport-operation semantics and assumes a **selected provider plugin** can be invoked after governed render/eligibility. It does **not** norm physical hosting.

A2 **P0 manual** governed relay is **published**. [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport) and [AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md) are **closed / satisfied**. **A4** implementation and **A4 implementation plan** remain **not authorized**.

This tranche canonicalizes the **smallest provider-neutral hosting and registration architecture** needed so a future A4 implementation planner does not invent system architecture. **A3** governed workflow MVP is **not** a prerequisite for this ADR.

## Decision

### 1. Scope and non-goals

**In scope:** Provider-neutral **plugin hosting and registration** — catalog, enablement, selection, initialization, invocation boundary, capability exposure, operational lifecycle semantics, trust/enablement minimum, configuration and credential ownership at the hosting boundary, failure reporting to operator projections.

**Out of scope (this ADR):** Governed relay semantics, DWA, STOP, package validation, transport-operation ledger schema, provider-specific transport (ACP/CLI/MCP), marketplace, enterprise distribution, code-signing PKI, auto-update, generalized OS sandbox, **A4** implementation, plugin loader **code**, concrete provider plugins.

**Subordinate to:** [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md), [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md), [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md), [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md), [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md).

### 2. Hosting ownership

**Application / infrastructure** owns provider-neutral plugin hosting facilities:

- Register known plugin implementations in a **plugin catalog**
- Enable or disable eligible implementations per operational policy
- Select an implementation for the **active Project**
- Initialize, invoke, and shut down hosted implementations through a **provider-neutral hosting boundary**
- Expose capability and availability information to transport orchestration
- Report hosting failures in forms suitable for [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)

**Core (A)** and **Software Development (B)** **MUST NOT** own provider-specific hosting mechanics or load provider implementations into domain governance assemblies.

Hosting **MUST NOT** reinterpret relay eligibility, validation, STOP, DWA, or [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) transport-operation semantics.

### 3. Registration and catalog

A plugin implementation becomes **known** to ProjectConcord only through **explicit registration** in the hosting **plugin catalog**.

**Initial architecture:** **Explicit / static registration is sufficient** — including first-party built-in implementations and, when separately authorized, **explicitly configured** external implementations.

**Deferred (not required):** Filesystem scanning, reflection-based discovery, marketplace or package-store discovery, arbitrary dynamic third-party loading without explicit registration and enablement.

### 4. Discovery policy

**Runtime dynamic discovery is not required** for the initial single-user desktop product architecture. The catalog is populated by **registration** (build-time composition and/or explicit configuration). Future dynamic discovery **MAY** be added in a separate tranche without changing the provider-neutral contract or transport semantics.

### 5. Provider plugin identity

Hosting **SHALL** use a stable conceptual **Provider Plugin Identity** for registration, configuration, selection, and operational attribution.

**Provider Plugin Identity MUST remain distinct from:** ProjectConcord Project ID, `PackageId`, `GovernedCorrelationId`, `TransportOperationId`, and noncanonical provider session handles ([ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) §11).

**Representation** (string, GUID, schema field, type name, or other) is **implementation design** — this ADR does not norm a concrete representation.

### 6. Capability declaration

Plugins **SHALL** expose **provider-neutral capability information** sufficient for hosting and transport orchestration, including facets aligned with [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §5 and [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) §9, such as:

- Manual paste / automated transport availability
- Supported **routing intent** adequacy (PLAN, AGENT, DEBUG)
- Relay / render **protocol compatibility** (major version)
- Provider availability / health and authentication readiness where applicable

**Declaration first:** Capabilities are declared at registration and **MAY** be refreshed after initialization. A generalized capability-negotiation protocol is **not required**; future negotiation **MAY** be added per [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-019 in a separate tranche.

### 7. Compatibility (conceptual dimensions)

Hosting **SHALL** evaluate three **distinct** compatibility dimensions:

| Dimension | Question |
|---|---|
| **A — Host / plugin contract** | Can this implementation participate in the ProjectConcord hosting and [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) contract boundary? |
| **B — Relay / render protocol** | Can this implementation participate in the required governed relay representation (for example `projectconcord-relay-v1`)? |
| **C — Provider capability** | Can this provider perform the requested operation and **routing intent** (including DEBUG adequacy rules in [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) §9)? |

Incompatible implementations **MUST** be treated as **unavailable** for the requested use and surfaced to operators — not as governance rejection.

**Not required by this ADR:** Semantic versioning tuples, specific compatibility data structures, or negotiation wire protocols (implementation design).

### 8. Lifecycle semantics (operational)

Hosting **SHALL** distinguish operational lifecycle semantics sufficient for safe use, for example:

- Known / registered
- Enabled vs disabled
- Initializing
- Ready / available vs degraded
- Failed / unavailable
- Shutting down
- Inactive
- **Active for a transport operation** (bound to orchestration — not a governance lifecycle)

Exact names, enums, and persistence are **implementation design**. Plugin-hosting lifecycle **MUST NOT** compete with governed relay or Software Development workflow lifecycles.

### 9. Selection

For the **active Project**, hosting **SHALL** resolve an eligible plugin using:

- Explicit configuration / **selected Provider Plugin Identity**
- Availability and compatibility (§7)
- Required capabilities for the requested path (manual vs automated transport, routing intent)

**No capable plugin** for automated transport is **neutral provider/hosting unavailability** — **not** package invalidity, STOP, DWA, or governance rejection. When P0 manual relay remains eligible, [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) P0 fallback applies.

### 10. Provider substitution

Per [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §8 and [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) §6:

- Changing selected plugin **MUST NOT** rewrite governed packages, correlation history, validation dispositions, or transport history.
- Mid-operation substitution **MUST** follow ADR-0022 transport rules (operator confirmation, new transport operation, P0 where applicable).

Hosting **MUST NOT** silently redirect an **active transport operation** to a different plugin implementation.

### 11. Process model and fault containment

This ADR **does not** select a single physical mechanism (in-process only, out-of-process only, or a specific IPC technology).

**Required:**

- Provider-specific implementation **MUST** remain outside **Core (A)** and **Software Development (B)** domain ownership.
- Provider failure **MUST NOT** corrupt governed relay validation state or package authority.
- Provider runtime failure **MUST** be containable and reportable through hosting.
- Provider execution **MUST** remain scoped to the **active Project** context and applicable **Project Root** policy.
- Secrets and provider-specific operational state **MUST NOT** become canonical relay or EDF state.

**Permitted realizations** (non-exhaustive, implementation design): in-process infrastructure **adapter**; subprocess provider runtime; separate host process; provider SDK/API; future equivalent — provided §11 requirements are met.

**This ADR MUST NOT** require out-of-process runtime merely because a reference provider may use one. **This ADR MUST NOT** norm gRPC, named pipes, sockets, stdio, or a specific IPC library as the provider-neutral architecture.

### 12. Trust boundary

A provider implementation **MUST** be **explicitly registered** and **explicitly trusted/authorized for enablement** before ProjectConcord may invoke it for operator-configured use.

The initial architecture **does not** require a formal multi-level trust-tier taxonomy. Registration **SHOULD** carry enough **provenance and identity** for operator understanding and future policy, conceptually including:

- Provider Plugin Identity
- Implementation identity and version (representation deferred)
- Source / publisher provenance where available
- Compatibility facets (§7)

**Deferred:** Code-signing PKI, plugin marketplace, package repository, auto-update system, generalized sandbox platform, enterprise distribution system.

### 13. Scope and access

Provider invocation **MUST** bind to the **active ProjectConcord Project** ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) Single-Project Focus). Plugin or provider session state **MUST NOT** determine which Project is authoritative.

Project Root access, execution context, credentials, and network use **MUST** respect the hosting trust boundary and applicable authorization scope — without defining a generalized OS sandbox in this ADR.

### 14. Configuration and credential ownership

| Layer | Content |
|---|---|
| **A — Canonical EDF / Project (F)** | No provider credentials; no provider session authority |
| **B — ProjectConcord operational configuration** | Non-secret plugin selection, enablement, preferences — per [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) partitioning |
| **C — Provider / plugin-local or secure storage** | Secrets, tokens, OAuth, API keys ([ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §9) |

Credentials **MUST NOT** be stored as authoritative fields in relay packages, transport-operation records, or canonical Git content.

### 15. Failure handling and ADR-0020

Hosting failures include conceptually: initialization failure; provider unavailable; authentication unavailable or expired; capability incompatibility; provider crash; hang or timeout.

These are **operational / provider** failures. They **MUST NOT** mutate governed package validity.

Per [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) and [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md): failures **SHOULD** appear in **Attention**; when P0 manual relay is eligible, manual governed relay **SHOULD** be **Recommended** Next Action. No parallel provider workflow UI lifecycle **SHALL** be introduced.

### 16. Transport ledger relationship

[ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) remains authoritative for transport-operation semantics and invariants.

Hosting **MAY** supply orchestration with: resolved **Provider Plugin Identity**; availability; capability and compatibility outcomes; information needed for recovery, substitution, and health projection.

Hosting **MUST NOT** redefine transport states or persistence schema. Exact ledger schema remains **implementation design**.

### 17. P0 manual fallback

P0 manual relay remains permanent ([ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §13, [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md)). Hosting unavailability or disabled plugins **MUST NOT** invalidate otherwise eligible governed packages.

### 18. Deferred implementation choices

The following remain **implementation design** unless a future PA tranche elevates a specific choice:

- DI container, reflection, manifest formats, assembly layout
- Process launch APIs and IPC libraries
- Catalog and configuration persistence schema
- Concrete C# types and hosting loader code
- Supply-chain signing, marketplace, auto-update

### 19. A4 and implementation authority

**A4** implementation, **A4** implementation plan, plugin loader code, plugin catalog implementation, provider plugins, Cursor ACP/CLI integration, MCP implementation, transport ledger implementation, and persistence schema implementation remain **not authorized** by this ADR.

Acceptance of this ADR **does not** authorize implementation; separate PA authorization is required for each implementation tranche.

## Consequences

- Completes the **architecture** for [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §14 deferred **hosting mechanics** at the conceptual level — **not** implementation.
- Enables a future PA decision to authorize **A4 implementation planning** without inventing hosting architecture — **does not** authorize A4 planning or implementation by itself.
- [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) history remains **closed / resolved**; this ADR cites §14 deferral as provenance only.
- **No new gap register entry** is created by this tranche (PA disposition: new gap not required).

## Related Documents

- [ADR-0021 — Engineering Agent provider plugin contract](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md)
- [ADR-0022 — Engineering Agent automated transport architecture](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md)
- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md), [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
