# AWI-0008 — Governed Operator Experience and Product Shell

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0008

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-30 |
| **Revisit Trigger** | Before post-A2 product-shell or governed-workflow UX tranche authorization; PA request to analyze ownership |
| **Discovery source** | [MVR-0002](../../Verification/Records/MVR-0002-a2-p0-manual-governed-relay-workflow.md) human execution (2026-09-30); A2-T8 documentation closeout — **not** A2 implementation defects |
| **Related ADRs** | [ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) (**Accepted** 2026-10-01 — operator projections, shell, navigation); [ADR-0016](../ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md); [ADR-0015](../ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md); [ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) (**Accepted** 2026-10-01); [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) (**Accepted** 2026-10-01); [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) (**Proposed** — attestation direction only, not acceptance) |
| **Last architecture update** | 2026-10-01 — A+C operator/shell/navigation **Accepted** in [ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md); structured authoring → [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) |
| **Related specs** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md); [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md) |
| **Cross-reference** | [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) (structured authoring — deferred); [AWI-0010](AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md) (workflow graph / GEW runtime — backend); [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md); [AWI-0005](AWI-0005-Manual-Verification-Records.md); [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md) |

---

## Objective

Capture **follow-on product and operator-experience requirements** identified during A2 P0 manual governed relay verification. These items guide future architectural analysis and implementation tranches. They **do not** retroactively classify A2-T7/T8 delivery as defective.

## Scope and non-goals

This watch item:

- **Does not** authorize `src/` changes, UI redesign, persistence changes, Markdown rendering, or plugin work (structured authoring → [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md)).
- **Does not** decide whether capabilities belong to Core, a domain extension, EDF integration, or Software Engineering extension — see **Architectural ownership (deferred)** below.
- **Does not** authorize storing full external chat/provider transcripts as canonical ProjectConcord content.
- **Does not** commit ProjectConcord to a general-purpose Markdown editor without separate architectural analysis.

While **Active**, requirements here are **prospective** unless promoted by PA into specifications or ADRs.

**Partial promotion (2026-10-01):** Terminology policy and projection architecture are **Accepted** in [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md). Governed **context-transfer minimization**, work-package/review-package direction, and state-source model are documented in §**Architectural disposition** below.

**Partial promotion (2026-10-01, persistence tranche):** **Single-Project Focus**, **Concurrent Project Work**, **inter-project governed handover continuity**, local-first operational persistence, service boundary, and future sync/concurrency direction are **Accepted** in [ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) and §**Product shell instance and continuity** below.

**Partial promotion (2026-10-01, A+C tranche):** Operator projections (Overview, Current Work, Attention, Next Action), workspace/navigation separation, progressive disclosure, capability-graded projections, operational draft state, replaceable active Project context, inter-project operator UX direction, and relay operator-stage binding are **Accepted** in [ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) and [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17 (PC-PAR-023). Closeout: [A+C tranche plan](../../Handover/ProjectConcord-AWI-0008-A+C-Documentation-Tranche-Plan.md).

**Partial promotion (2026-10-03, recovery binding tranche — published):** Governed work-state recovery authoritative inputs ([ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15) and relay stale-import non-regression ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §19). [Tranche plan](../../Handover/ProjectConcord-Durable-Governed-Work-State-Recovery-Binding-Tranche-Plan.md).

Shell **implementation** (UI, Markdown rendering, sync, terminology projection in `src/`) remains **not authorized**. Structured authoring (Form Runtime, form definitions, Form Editor) is tracked in [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) — **not** here. This watch item stays **Active** until shell implementation is authorized and delivered or explicitly closed.

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

**Disposition (2026-10-01, A+C tranche):** Workflow-local **staged/stepper** presentation within a broader Project **workspace** — not a modal linear wizard requirement ([ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §5; [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17). Raw relay transport remains progressive disclosure only.

### B.1 Hierarchical Workflow View (deferred — capture 2026-10-03)

**Handover capture:** `c3b1d909-7a0e-4a32-b486-fd1c12afab17` (supplements operator UX direction; **does not** authorize implementation).

**Future capability:** An operator-visible **Hierarchical Workflow View** that helps users navigate **WorkflowInstances** and their **governed relationships** that arose during engineering work (conceptual example: A → B → D and A → C). The view **may** surface origin/discovery links, blocking dependencies, **Satisfied** / **Released** dependency history, lifecycle/topology state, and later relationship kinds.

**Authoritative model vs visualization:**

```text
Authoritative workflow graph (instances + relationship records)
    -> governed operator projections ([ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md); [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §2.7)
    -> hierarchical / tree-like visualization (derived only)
```

The graph is **not** necessarily a tree. The UI **SHALL NOT** redefine authoritative relationships as parent/child **ownership**. Because instances may have **multiple origins**, **multiple blockers**, and **shared dependencies**, the presentation **may** require repeated/reference nodes, cross-links, relationship indicators, and expansion/navigation — not a single parent per node.

**Node inspection (future):** Selecting a node opens inspection of that **WorkflowInstance** (identity, definition/version, lifecycle, topology place, traversal occurrence, baseline, origins, dependencies and status, **Waiting On**, provenance/history, and later governed dimensions). **Inspection ≠ mutation:** visibility and selection **do not** grant authority to edit, delete, release, advance, or otherwise mutate; command availability must be derived from **governed operation policy** (Available / Unavailable / Requires authorization / Requires attention — or equivalent), not from display alone (`node visible ≠ node mutable`; `node selected ≠ mutation authorized`).

**Delete semantics (future investigation):** Do **not** assume arbitrary physical deletion of governed **WorkflowInstance** rows. History and provenance are preserved; “delete” in the UI may map to governed operations such as cancel, supersede, abandon, archive, or draft removal — **not** convenience erasure of historical state.

**Relationship operations (future):** Governed transitions such as dependency **Pending → Released** (when policy permits) **mutate relationship state**; they **do not** imply physical deletion of **Satisfied** / **Released** records where inspection/provenance requires retention ([M7a-WF-1b](../../Development/EDF_Gap_Register.md#gap-054--workflow-framework-runtime-and-effective-configuration) backend scope).

**Visual status (deferred design):** Eventually distinguish Active, dependency-blocked vs dependency-unblocked, Completed, future Suspended/Cancelled/Superseded if defined, Pending / Satisfied / Released dependencies, and Attention — exact visuals TBD.

**Explicit non-goals for current tranches:** **No** Desktop workflow tree, graph editor, drag/drop graph editing, delete controls, lifecycle command UI, or visualization implementation in **M7a-WF-1b** (backend graph/projection only). Implementation requires a **separate** PA-governed product-shell / UX tranche after authoritative graph data exists ([AWI-0010](AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md)).

### B.2 Synchronization path disposition (deferred — capture 2026-10-06)

**Handover capture:** `476976f4-8b0d-4a4d-b1ef-2035fb9bb034` (reviews plan `8f3b0af2-b327-4dca-8ef3-78d13d742f87`) — **documentation only**; **does not** authorize `src/` implementation or amend [ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md).

**Future operator UX follow-through** for [AWI-0010 §B.3](AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md#b3-conditional-alternate-synchronization-satisfaction-deferred--capture-2026-10-06) **conditional alternate synchronization satisfaction** and **synchronization path disposition** (**OptimizedAlternate** default vs **RequireInteractiveSynchronization** override).

When an eligible/applicable optimized path is active, the operator experience **SHOULD**:

- make the **active disposition** visible (optimized default vs require-interactive override);
- provide a clear, **occurrence-scoped** mechanism to require full interactive Project Architect synchronization for the applicable synchronization occurrence;
- **not** treat presentation or inspection as establishing eligibility or applicability ([ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) — inspection/display **≠** governed mutation; command availability from governed operation policy).

Operator UI **SHALL NOT** substitute for prescribed workflow definition, applicable policy, authorization, **Controls**, or objective **EvidenceRequirements** when determining eligibility.

Exact control placement, wording, and stepper/Attention integration remain **deferred**. **No** UI implementation in current tranches.

### B.3 Governed Synchronization Review — review frontier and packages (capture — 2026-10-06)

**Handover capture:** Governed Synchronization Review documentation tranche `a88bc129-87c9-4df6-949c-b56b5881eb39` — **documentation only**; **does not** authorize `src/` implementation.

**Architecture:** [PCON-0007](../PCON-0007-Governed-Synchronization-Review.md) (**Accepted**); operator rules in [ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15.

Future **review packages**, **review frontier** presentation, and related **Attention** / **Next Action** facets **SHALL** be **derived projections** of authoritative operational state (**B**) — including **Governed Synchronization Review Context**, **Review Subject** dispositions, cited **Review Evidence** fingerprints, open GIES items, and orthogonal **SynchronizationPoint** satisfaction — plus canonical **A** and structured agent results.

Review packages **SHALL NOT** be the authoritative store of review state. Human-readable handovers remain valid **transport** ([SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-006).

Exact UI for frontier ordering, subject/evidence drill-down, and disposition history remains **deferred**.

### C. Durable governed relay/package working state

Useful governed handover/package working material should survive application close/reopen where appropriate.

Future architecture must distinguish at least:

- canonical governed artifacts;
- persisted operational relay/package state;
- provenance;
- ephemeral UI state;
- external chat/provider conversation content.

This requirement does **not** authorize storing full external chat transcripts as canonical ProjectConcord content.

### D. Artifact detail host — Markdown and future structured views

ProjectConcord needs native Markdown viewing. For applicable Markdown/governed content, the artifact detail host should support a clear user-facing toggle such as **Rendered | Source**. Rendered Markdown should be available without requiring an external Markdown application. This does **not** yet authorize a general Markdown editor.

The artifact detail host must be architecturally capable of accommodating future **Form | Rendered | Source** interaction ([ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §12) without assuming manual Markdown source editing is the primary human interaction.

**Structured authoring** (form definitions, Form Runtime, round-trip behavior, external editing, built-in forms, Form Editor, AI/SI interoperability, canonicality rules for forms) is owned by [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) — **deferred**, **capture-only**. Do not duplicate that architecture here.

### E. User-facing terminology

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
Human  ↔  ProjectConcord  ↔  Project Architect provider
ProjectConcord  ↔  Engineering Agent
```

**Preferred PA path (deferred — capture `730c89c8`):** The human asks ProjectConcord in ordinary language; ProjectConcord composes the canonical **PA Exchange Contract** request and handles import/validation of structured PA responses. Manual ChatGPT copy/paste remains a supported **transport**, not a separate governance architecture. Normative follow-through: [AWI-0010 §B.2](AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md#b2-project-architect-exchange-contract-deferred--capture-2026-10-03). **Not authorized** in current tranches.

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
| MVR attestation write path | [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) (**Proposed**); form/attestation UI direction → [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) |

## Architectural ownership (remaining open items)

**Resolved at architecture level (2026-10-01):** terminology layers and policy ([ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md)); context-transfer requirement; state-source model; work/review package direction; single-Project Focus; multi-instance model; inter-project continuity; local-first/service/sync/concurrency principles ([ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) **Accepted**); operator projections, shell IA, workflow-local progression, attention/next-action model, operational draft state, replaceable active Project context ([ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) **Accepted**); relay operator-stage binding ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17).

**Still requires implementation authorization:**

- `src/` product shell, navigation, projections, Markdown rendering, terminology projection;
- operational draft persistence schema and behavior;
- structured authoring per [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md);
- **Hierarchical Workflow View** (§B.1) — deferred; depends on workflow instance + relationship runtime ([AWI-0010](AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md)).

Subsequent PA-governed tranches are required before `src/` implementation.

## Signals to watch

- Workflow stage semantics embedded in a single extension without Core boundary review.
- Operational relay paste buffers treated as canonical artifacts.
- MVR or gate state mutated without human-attestation boundary ([ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)).
- User-facing shorthand (PA, EA, PAR) proliferating in Desktop without glossary or expansion.
- Hierarchical workflow UI treated as **authoritative** structure (tree-as-source-of-truth) rather than a **projection** over the workflow graph ([PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §2.7; [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §12).

## Parent

- [Watch Items](README.md)

## Related Documents

- [A2 Implementation Plan](../../Handover/ProjectConcord-A2-Implementation-Plan.md) §35–§36
- [A2-T8 verification evidence](../../Handover/ProjectConcord-A2-T8-Verification-Evidence.md)
- [MVR-0002](../../Verification/Records/MVR-0002-a2-p0-manual-governed-relay-workflow.md)
- [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) **closed / resolved** ([ADR-0021](../ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) 2026-10-01); [AWI-0006](AWI-0006-PAR-Cursor-Bridge-Transport.md) **Active** (transport — [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport))
- [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md); [ADR-0019](../ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); [ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md); [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md); [AWI-0008 A+C documentation tranche](../../Handover/ProjectConcord-AWI-0008-A+C-Documentation-Tranche-Plan.md); [AWI-0008 Persistence documentation tranche](../../Handover/ProjectConcord-AWI-0008-Persistence-Sync-Documentation-Tranche-Plan.md); [EDF Terminology Governance handover](../../Handover/EDF-Terminology-Governance-Architecture-Handover.md); [GAP-047](../../Development/EDF_Gap_Register.md#gap-047--terminology-projection-and-optional-enforcement); [GAP-048](../../Development/EDF_Gap_Register.md#gap-048--same-project-id-multiple-local-application-processes)–[GAP-052](../../Development/EDF_Gap_Register.md#gap-052--multi-process-local-operational-store-concurrency-strategy-and-validation)
