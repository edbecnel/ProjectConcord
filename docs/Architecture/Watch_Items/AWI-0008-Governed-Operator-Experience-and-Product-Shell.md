# AWI-0008 — Governed Operator Experience and Product Shell

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0008

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-30 |
| **Revisit Trigger** | Before post-A2 product-shell or governed-workflow UX tranche authorization; PA request to analyze ownership |
| **Discovery source** | [MVR-0002](../../Verification/Records/MVR-0002-a2-p0-manual-governed-relay-workflow.md) human execution (2026-09-30); A2-T8 documentation closeout — **not** A2 implementation defects |
| **Related ADRs** | [ADR-0016](../ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md); [ADR-0015](../ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md); [ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) (**Accepted** 2026-10-01); [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) (**Accepted** 2026-10-01); [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) (**Proposed** — attestation direction only, not acceptance) |
| **Last architecture update** | 2026-10-01 — terminology + context-transfer; single-Project Focus, inter-project continuity, local-first/sync/concurrency ([ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) **Accepted**) |
| **Related specs** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md); [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md) |
| **Cross-reference** | [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md); [AWI-0005](AWI-0005-Manual-Verification-Records.md); [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md) |

---

## Objective

Capture **follow-on product and operator-experience requirements** identified during A2 P0 manual governed relay verification. These items guide future architectural analysis and implementation tranches. They **do not** retroactively classify A2-T7/T8 delivery as defective.

## Scope and non-goals

This watch item:

- **Does not** authorize `src/` changes, UI redesign, persistence changes, Markdown rendering, form runtime, or plugin work.
- **Does not** decide whether capabilities belong to Core, a domain extension, EDF integration, or Software Engineering extension — see **Architectural ownership (deferred)** below.
- **Does not** authorize storing full external chat/provider transcripts as canonical ProjectConcord content.
- **Does not** commit ProjectConcord to a general-purpose Markdown editor without separate architectural analysis.

While **Active**, requirements here are **prospective** unless promoted by PA into specifications or ADRs.

**Partial promotion (2026-10-01):** Terminology policy and projection architecture are **Accepted** in [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md). Governed **context-transfer minimization**, work-package/review-package direction, and state-source model are documented in §**Architectural disposition** below.

**Partial promotion (2026-10-01, persistence tranche):** **Single-Project Focus**, **Concurrent Project Work**, **inter-project governed handover continuity**, local-first operational persistence, service boundary, and future sync/concurrency direction are **Accepted** in [ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) and §**Product shell instance and continuity** below.

Shell **implementation** (UI, package generation, sync, terminology projection in `src/`) remains **not authorized**. This watch item stays **Active** until remaining requirements (orientation, navigation, Markdown, form runtime, implementation) are promoted or closed.

## Requirements (PA disposition — A2-T8 closeout 2026-09-30)

### A. Governed workflow orientation

The user must be able to understand:

- where they currently are in a governed workflow;
- what has already been completed;
- what is currently awaiting action;
- what action is available next;
- what is blocking progression.

The production UI must not require the user to infer workflow state from a collection of controls or raw relay data alone.

Workflow stages must not be assumed to be universally hard-coded Core semantics. Their architectural ownership requires analysis.

### B. Structured workflow navigation

The P0 long vertically scrollable relay panel is **not** an acceptable target production UX.

Future architecture should evaluate wizard-style workflow navigation, tabbed/workspace navigation, or a combination. The UI should guide the user through governed work rather than expose the entire workflow as one long form.

### C. Durable governed relay/package working state

Useful governed handover/package working material should survive application close/reopen where appropriate.

Future architecture must distinguish at least:

- canonical governed artifacts;
- persisted operational relay/package state;
- provenance;
- ephemeral UI state;
- external chat/provider conversation content.

This requirement does **not** authorize storing full external chat transcripts as canonical ProjectConcord content.

### D. Markdown rendered / source view

ProjectConcord needs native Markdown viewing. For applicable Markdown/governed content, the UI should support a clear user-facing toggle such as **Rendered | Source**. Rendered Markdown should be available without requiring an external Markdown application. This does **not** yet authorize a general Markdown editor.

### E. Governed form runtime

Future architecture should evaluate a reusable runtime-generated Avalonia form capability for governed structured user input.

MVR execution is an initial motivating use case. Instead of requiring manual editing of MVR Markdown tables, ProjectConcord could present purpose-built controls (for example Result: Pending / Pass / Fail / Blocked; Executor; Date; Test data / fixture; Evidence) and write governed results to canonical Markdown.

Initial forms should be purpose-built / ProjectConcord-defined. Do **not** assume arbitrary user-defined form design in the initial architecture. Potential future extension-defined forms may be analyzed separately.

### F. Future project-aware constrained Markdown editing

Retain as a future architectural possibility: **project-aware constrained Markdown editing**. For recognized governed artifacts, editing could understand artifact type, permitted values, known actors, references, validation constraints, and governed state transitions.

Do not commit ProjectConcord to becoming a general-purpose Markdown editor without architectural analysis.

### G. Canonicality invariant

**Canonical Markdown remains primary.** Rendered views, structured forms, and constrained editing interfaces must be projections over canonical governed artifacts and must not silently create competing sources of truth.

### H. User-facing terminology

Avoid unnecessary acronyms in the ProjectConcord user interface. In particular, display **Project Architect** rather than **PA** where the term is presented to the user. Repository and internal architectural documentation may retain established acronyms where appropriate and unambiguous.

**Disposition (2026-10-01):** Layered terminology architecture and ProjectConcord **Preferred** Super Intelligence (SI) for EDF `super-intelligence` are **Accepted** in [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md). EDF **recommends**; ProjectConcord **prefers**; **enforcement not authorized**. Presentation projection in UI remains future work. See [EDF Terminology Governance handover](../../Handover/EDF-Terminology-Governance-Architecture-Handover.md).

## Architectural disposition (PA — 2026-10-01)

The following records **accepted architecture** for operator experience and governed interaction. It **does not** authorize implementation.

### Governed context-transfer minimization

**Requirement:** ProjectConcord SHALL minimize repeated conversational context transfer between governed actors (Project Architect ↔ Engineering Agent) by deriving **Engineering Agent work packages** and **Project Architect review packages**, where practicable, from **canonical project state** plus **persisted governed operational state**.

Human-readable handovers remain required for transport, inspection, fallback, copy/paste, and audit-friendly projection — but their role shifts from **manually reconstructed container of nearly all context** toward **concise projection of already-governed state**.

Human authority and explicit authorization boundaries are preserved. ProjectConcord is the **governed intermediary** for relevant state; it is **not** the architectural decision maker.

### Target governed interaction model

```text
Project Architect  ↔  ProjectConcord  ↔  Engineering Agent
```

ProjectConcord supplies (when implemented): canonical context retrieval; governed operational continuity; work-package and review-package construction; authorization representation; STOP boundaries; provenance; evidence intake/projection. Authority remains with applicable human governance roles.

Provider-neutral concepts only: Project Architect, Engineering Agent, provider adapter, session, work package, review package, governed interaction, transport ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md); [ADR-0016](../ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)). Concrete provider products remain in **E** adapters.

### Governed state sources

| Source | Examples | Canonical? |
|---|---|---|
| **A — Canonical project state** | Accepted ADRs, specs, MVRs, EGRs, plans, gaps/watch items, terminology policy, Project ID, Git Markdown ([ADR-0002](../ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)) | **Yes** (Git) |
| **B — Persisted governed operational state** | Relay/package records, continuity, provenance events, validation/STOP, workflow position, pending review/evidence ([ADR-0015](../ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md); A2 `Migration002`) | **Operational** (per-user SQLite by Project ID) |
| **C — External provider state** | Provider chat threads, session handles | **Not** ProjectConcord canonical; full transcripts **must not** become canonical to avoid handover effort |

### Engineering Agent work package (conceptual)

Future derivation from **A + B** (not from full **C**). May conceptually include: ProjectConcord Project ID; operation/work identity; canonical baseline pointers; relevant accepted decisions; explicit scope and exclusions; mode/capability; authorization; STOP condition; evidence/result contract; continuity/provenance references.

Aligns with existing **Governed Interaction Relay** packages ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)) without mandating a new provider-specific schema in this tranche.

### Project Architect review package (conceptual)

Future derivation from **A + B + Engineering Agent result/evidence** (structured operational intake, not raw provider transcript). Must support understanding: what was authorized; baseline used; what was investigated or changed; evidence; deviations; open questions; authorization boundaries; STOP state; proposed next action.

### Human-readable handovers

Handovers remain **valid** as projection/transport representations. They are **not** obsolete. Increasing concision is enabled when authoritative context is recoverable from **A** and **B**.

### Product shell instance and continuity (PA — 2026-10-01)

**Single-Project Focus:** One application instance maintains **one active** Project context at a time — intentional for operator focus, workflow clarity, and Project-scoped governance ([ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)). Closing a Project and opening another **after** clearing the active context remains compatible; simultaneous multi-Project tabs/workspace are **not** the architecture target.

**Concurrent Project Work:** Multiple ProjectConcord processes MAY run concurrently; each owns one active Project.

**Cross-project awareness SHALL NOT imply multi-project workspace ownership.**

**Inter-Project Governed Handover:** When work in the active Project requires governed work in another Project, the originating instance SHOULD preserve operational continuity (blocked/suspended work, correlation identity, return expectation) while dependent work proceeds in another **independently active** Project context (typically another instance). Return of governed evidence/results SHOULD allow resumption **without** adopting the dependent Project as the originating instance's active context. **B-layer** semantics: **InterProjectHandover**, **CrossProjectDependency** ([ADR-0013](../ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)); transport via governed relay/packages — not a separate ad-hoc protocol.

**Package scope:** Work packages, review packages, and inter-project request/result projections are scoped to the instance's **active Project** unless explicitly identified as governed **inter-project** transport.

**Persistence (architecture):** Operational state via **service contracts** → local persistence (SQLite today) → optional future **synchronization** to **Cloud Service API** — see [ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md). Implementation **not authorized** here.

### Ownership summary (accepted direction)

| Concern | Owner |
|---|---|
| Product shell, navigation surfaces, orientation projection | **Core (A)** |
| Relay validation, packages, provenance | **Core Governed Interaction Relay (A)** |
| Software authorization, profile payloads | **Software Development (B)** |
| EDF artifact semantics | **F** — consume only |
| Provider sessions and transport rendering | **E** |
| Terminology policy | [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) |
| MVR attestation write path | [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) (**Proposed** — dependency direction for form runtime; not Accepted) |

## Architectural ownership (remaining open items)

**Resolved at architecture level (2026-10-01):** terminology layers and policy ([ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md)); context-transfer requirement; state-source model; work/review package direction; single-Project Focus; multi-instance model; inter-project continuity; local-first/service/sync/concurrency principles ([ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) **Accepted**).

**Still requires promotion or implementation authorization:**

- generic Markdown viewing — Core vs shell detail;
- governed form runtime placement;
- EDF-specific vs extension-specific forms;
- constrained Markdown editing boundary;
- workflow navigation pattern (wizard vs workspace);
- durable paste/draft working state persistence rules beyond A2 relay tables.

Subsequent PA-governed tranches are required before `src/` implementation.

## Signals to watch

- Workflow stage semantics embedded in a single extension without Core boundary review.
- Operational relay paste buffers treated as canonical artifacts.
- MVR or gate state mutated without human-attestation boundary ([ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)).
- User-facing shorthand (PA, EA, PAR) proliferating in Desktop without glossary or expansion.

## Parent

- [Watch Items](README.md)

## Related Documents

- [A2 Implementation Plan](../../Handover/ProjectConcord-A2-Implementation-Plan.md) §35–§36
- [A2-T8 verification evidence](../../Handover/ProjectConcord-A2-T8-Verification-Evidence.md)
- [MVR-0002](../../Verification/Records/MVR-0002-a2-p0-manual-governed-relay-workflow.md)
- [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) / [AWI-0006](AWI-0006-PAR-Cursor-Bridge-Transport.md) — Engineering Agent plugin boundary unchanged
- [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md); [ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); [AWI-0008 Persistence documentation tranche](../../Handover/ProjectConcord-AWI-0008-Persistence-Sync-Documentation-Tranche-Plan.md); [EDF Terminology Governance handover](../../Handover/EDF-Terminology-Governance-Architecture-Handover.md); [GAP-047](../../Development/EDF_Gap_Register.md#gap-047--terminology-projection-and-optional-enforcement); [GAP-048](../../Development/EDF_Gap_Register.md#gap-048--same-project-id-multiple-local-application-processes)–[GAP-052](../../Development/EDF_Gap_Register.md#gap-052--multi-process-local-operational-store-concurrency-strategy-and-validation)
