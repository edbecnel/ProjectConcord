# ADR-0019: Local-First Operational Persistence, Service Boundary, Synchronization, and Concurrency

## Status

Accepted

## Date

2026-10-01

## Context

[AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) product-shell investigation and the AWI-0008 Persistence Architecture PLAN (2026-10-01) established implementation and documentation gaps: single active Project focus per application instance, local-first operational persistence, a mandatory service boundary before database technology, future cloud synchronization, and operational concurrency without collapsing canonical Git/EDF authority.

[ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) already classifies canonical vs operational data and anticipates optimistic concurrency at a high level. [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) defines Project ID, Project Root, and per-user operational SQLite direction. A1/A2 published local persistence and relay operational stores in `Edf.ProjectServices` behind Application ports.

This ADR records **architecture only**. It does **not** authorize synchronization implementation, cloud deployment, schema changes, or concurrency enforcement in `src/`.

## Decision

### 1. Single-Project Focus and concurrent instances (Core product shell)

**Single-Project Focus:** A running ProjectConcord application instance SHALL maintain **one active Project context** at a time (one **ProjectConcord Project ID** and its bound **Project Root** session locator). This constraint is **intentional** — it preserves operator focus, workflow continuity, implementation simplicity, and clear Project-scoped governance boundaries. ProjectConcord SHALL **not** require a single application instance to host **multiple simultaneously active** Project contexts (for example project tabs or a multi-project workspace shell).

**Simultaneous active contexts** is the invariant; it does **not** prohibit closing a Project, returning to a launcher-like state, or opening a **different** Project **after** the prior active context has been cleared.

**Concurrent Project Work:** ProjectConcord SHALL support **multiple concurrently running application instances**. Each instance independently maintains the active context of **one** Project.

**Cross-project awareness SHALL NOT imply multi-project workspace ownership.** References, dependencies, governed handovers, and recent-project launch mechanisms are compatible; they do not require the originating instance to load another Project as an simultaneously active workspace.

### 2. Inter-Project Governed Handover continuity

When work in one Project identifies a dependency requiring governed work in another Project, the **originating** instance SHALL be capable of **preserving** its current operational context while a governed inter-project handover is transferred to an **independently active** Project context (typically another application instance) responsible for the dependent Project.

Resolution of dependent work SHALL be capable of returning governed evidence/result state to the originating Project so that blocked or suspended workflow may **resume** without requiring the originating instance to adopt the dependent Project as its active context.

**Software Development semantics** for **InterProjectHandover** and **CrossProjectDependency** remain in [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) / [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md). **Core Governed Interaction Relay** packages ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)) and AWI-0008 work/review package direction provide transport and projection mechanisms — this ADR does **not** introduce a competing inter-project protocol. Originating workflow continuity (blocked state, correlation identity, return expectation) is an **architectural requirement**; lifecycle names and schemas remain deferred unless governed elsewhere ([GAP-033](../../Development/EDF_Gap_Register.md#gap-033--interprojecthandover-operational-schema), [GAP-034](../../Development/EDF_Gap_Register.md#gap-034--crossprojectdependency-and-source-notification)).

### 3. Governed packages and instance scope

Engineering Agent **work packages**, Project Architect **review packages**, and governed **inter-project** request/result projections SHOULD be derived from **canonical Project state (Git/EDF)** plus **persisted governed operational state**, not from full external provider transcripts ([AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)).

Packages produced by an instance are scoped to that instance's **active Project** unless explicitly identified as governed **inter-project** transport.

### 4. ProjectConcord operational service boundary

ProjectConcord application and domain consumers SHALL interact with **operational persistence** through defined **ProjectConcord service/application contracts** (semantic, use-case-oriented ports and orchestration), **not** by depending on a database technology or SQL directly.

For local desktop operation, contracts MAY be satisfied **entirely in-process**. “Service API” does **not** imply HTTP, REST, gRPC, or a separate local server.

Conceptual layering:

```text
ProjectConcord Application (Desktop)
          │
          ▼
Application / Domain Services
          │
          ▼
ProjectConcord Service Contracts
          │
          ▼
Local ProjectServices implementation
          │
          ▼
SQLite (current local store technology)
```

Generic database abstractions such as `ExecuteQuery(...)` are **not** the intended application contract shape.

**Known debt:** Parts of `Edf.Application` composition still construct or reference the concrete SQLite store type in `Edf.ProjectServices` rather than depending only on contract interfaces — tracked as [GAP-051](../../Development/EDF_Gap_Register.md#gap-051--application-layer-persistenceservice-port-isolation).

### 5. Local-first operational persistence

ProjectConcord SHALL use a **local-first operational persistence** architecture. **Local-only** operation (no cloud) SHALL remain a complete supported model.

**Operational mutations** become **durably local** through the ProjectConcord service boundary **independently of cloud availability**. Eligible operational state MAY subsequently participate in **governed synchronization** when configured.

**SQLite** in OS application data (`user-state.db`, partitioned by Project ID per [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)) is the **currently accepted** local implementation. This ADR does **not** mandate a specific database engine write ordering for every future operation.

Cloud capability is **additive**. Cloud availability SHALL **not** be required for ordinary local ProjectConcord operation.

### 6. Cloud-connected operation (architecture direction)

When cloud synchronization is configured, the desktop client SHALL **not** connect directly to a cloud database. It SHALL use a **ProjectConcord Cloud Service API** (contract unspecified here) that owns cloud-side validation, authentication/authorization where applicable, concurrency acceptance, server-side transactions, and shared operational persistence.

```text
Application
    │
    ▼
Service contracts
    │
    ▼
Local operational persistence
    │
    ▼
Synchronization subsystem (future)
    │
    ▼
ProjectConcord Cloud Service API (future)
    │
    ▼
Cloud operational store (future)
```

**Routine connected synchronization** (automatic coordination while connected) and **reconnection reconciliation** (after partition/offline divergence) are **distinct** architectural concerns and MUST NOT be collapsed into a single conceptual operation. Routine sync SHOULD be **automatic** — users SHOULD NOT be required to perform Git-like manual Push/Pull for normal operational-state synchronization.

Loss of cloud connectivity SHALL NOT break the local-first persistence path; eligible state remains locally durable and may be marked synchronization-pending conceptually.

### 7. Connected multi-user concurrency

Synchronizable **operational** state SHALL **not** suffer **silent lost updates** during connected multi-user use. **Last-writer-wins** SHALL **not** be the default for governed operational resources.

**Versioned optimistic concurrency** is the **default architectural direction**: clients submit changes with an **expected resource version**; the service accepts only when the expected version matches the current version and advances the version on commit; mismatch yields an explicit concurrency condition rather than silent overwrite.

Canonical **Git/EDF** artifacts and **change-set** semantics ([SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)) remain a **separate** authority and concurrency domain from operational-resource versioning.

Exact version fields, tokens, wire representation, and SQL schema are **deferred**.

### 8. Resource-aware concurrency semantics

Synchronizable operational resource **categories** SHALL have **declared concurrency semantics** when implemented. Architecture MUST be capable of distinguishing concepts such as:

- version-checked mutable state;
- append-only streams;
- state safely mergeable only under an explicit governed rule;
- state requiring temporary exclusivity;
- state requiring human conflict resolution.

A formal taxonomy of policy class names is **deferred** unless required for a future implementation gate ([GAP-050](../../Development/EDF_Gap_Register.md#gap-050--resource-level-operational-concurrency-policy-and-version-model)).

### 9. Reconciliation after divergence

After disconnected or partitioned operation, reconciliation MAY use a conceptual **BASE / LOCAL / REMOTE** model. Outcomes include: no conflict; concurrent but non-overlapping change; deterministic governed automatic resolution; and human-required semantic conflict. Automatic reconciliation is permitted only where resource semantics and governed policy make it demonstrably safe. ProjectConcord SHALL NOT guess between conflicting governance-sensitive decisions.

### 10. Scoped leases (where justified)

Architecture MAY permit future **resource-scoped**, **operation-scoped**, **time-bounded**, **renewable/expiring** leases for operations that genuinely require exclusive control.

Architecture SHALL **not** require a **global cloud lock** before ordinary local persistence. Local-first operation MUST remain viable without cloud connectivity.

Lease protocols are **not** defined here.

### 11. Provenance for synchronization outcomes

Concurrency and reconciliation architecture MUST preserve sufficient **provenance** to explain significant outcomes (conceptually: Project ID, actor, base/version context, local change identity, remote version, acceptance/rejection, reconciliation rule applied, conflict identity, human resolution, resulting state/version). Physical storage is deferred.

### 12. Canonical Git/EDF vs operational persistence

Canonical governed Project artifacts remain authoritative per [ADR-0002](ADR-0002-EDF-Canonical-Source-of-Truth.md) and EDF/Git governance. Operational persistence remains **operational**.

Cloud synchronization of operational state SHALL **not**:

- promote local or cloud operational snapshots into canonical EDF truth;
- redefine Git governance, MVR/AAR/ADR acceptance, or provider transcripts as canonical ([AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md));
- conflate **canonical Git synchronization** with **operational-state synchronization**.

### 13. Multi-process local store (same installation/user)

Multiple ProjectConcord processes on the same host may share the per-user operational SQLite file ([ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)). ProjectConcord has **not** yet explicitly designed, validated, or governed operational semantics for that situation ([GAP-052](../../Development/EDF_Gap_Register.md#gap-052--multi-process-local-operational-store-concurrency-strategy-and-validation)). This ADR does **not** characterize shared-file access as inherently unsafe; investigation of transaction behavior, coordination, shared preferences, and validation strategy is deferred.

Opening the **same Project ID** from multiple local processes for the **same user/installation** is an open policy question ([GAP-048](../../Development/EDF_Gap_Register.md#gap-048--same-project-id-multiple-local-application-processes)); it does **not** imply a general prohibition on future multi-client collaborative access to a Project.

## Alternatives Considered

### Multi-project workspace in one application instance

- Advantages: Single-window switching between Projects.
- Disadvantages: Operator focus dilution; higher shell/state complexity; conflicts with PA single-Project Focus product direction.
- Reason not selected: Superseded by single active Project per instance plus multi-instance concurrent work ([AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) tranche 2026-10-01).

### Direct desktop-to-cloud-database access

- Advantages: Simpler early cloud prototype.
- Disadvantages: Violates [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) layering; couples clients to storage technology.
- Reason not selected: Cloud Service API boundary required.

### Last-writer-wins as default operational concurrency

- Advantages: Simple implementation.
- Disadvantages: Silent loss of governed operational updates.
- Reason not selected: Explicit PA and ADR-0009 direction toward conflict detection and optimistic concurrency.

## Consequences

### Positive

- Clear product-shell invariant aligned with current `LocalProjectRuntime` behavior.
- Local-first and cloud-additive model preserves offline and solo operation.
- Service boundary preserves evolvability of storage and sync transport.
- Inter-project continuity without multi-project workspace.

### Negative

- Canonical documents that referenced “multi-project application context” require reconciliation ([ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md), [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)).
- GAP-048–052 remain open through implementation phases.

### Risks

- Premature sync or OCC implementation before policy gaps close — mitigated by explicit gap register and non-authorization of `src/` changes in this tranche.

## Explicit non-goals (this ADR)

- REST vs gRPC vs other transport; cloud database vendor; hosting topology
- Event sourcing, CRDTs, vector clocks, operational transforms
- Exact merge algorithms, change journals, lease protocols, conflict UI
- SQLite WAL, per-instance DB files, or process-lock **prescription**
- Implementation authorization for synchronization, cloud services, or concurrency enforcement

## References

- [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md), [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)
- [SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md), [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
- [EDF Gap Register](../../Development/EDF_Gap_Register.md) — GAP-032, GAP-048–052
- [Developer Handbook §02](../../Developer_Handbook/02_Per_User_Application_State.md)
- [AWI-0008 Persistence documentation tranche plan](../../Handover/ProjectConcord-AWI-0008-Persistence-Sync-Documentation-Tranche-Plan.md)

## Maintenance

**Accepted** 2026-10-01 (Project Architect). Documentation tranche closed per [AWI-0008 Persistence documentation tranche plan](../../Handover/ProjectConcord-AWI-0008-Persistence-Sync-Documentation-Tranche-Plan.md). Update when persistence, synchronization, or concurrency **implementation** is separately authorized or when cloud service contracts are first specified. Acceptance does **not** authorize `src/` implementation.
