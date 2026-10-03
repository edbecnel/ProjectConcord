# ADR-0020: Operator Projections, Product Shell, and Workspace Navigation

## Status

Accepted

## Date

2026-10-01 (Accepted by Project Architect)

**Amended:** 2026-10-03 — governed work-state recovery binding **published** ([tranche plan](../../Handover/ProjectConcord-Durable-Governed-Work-State-Recovery-Binding-Tranche-Plan.md))

## Context

[AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) captured operator-experience requirements from A2 P0 manual governed relay verification. Partial promotions established terminology ([ADR-0018](ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md)), instance and persistence architecture ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)), and governed interaction direction (work/review packages, state sources **A** / **B** / **C**).

The Project Architect accepted the AWI-0008 **Operator Workflow + Workspace/Navigation** investigation (2026-10-01) and authorized this documentation tranche to canonicalize **governed operator workflow**, **workspace/navigation**, and **product shell** semantics without UI implementation.

[PCON-0000](../PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) §37–38 describe a long-horizon dashboard and semantic navigation vision. This ADR reconciles that intent with **Single-Project Focus** ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)) and separates presentation projections from authoritative governance entities ([ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md), [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)).

Structured authoring (form definition, Form Runtime, Form Editor, round-trip Markdown) is **deferred** in [AWI-0009](../Watch_Items/AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md). This ADR records only the product-shell accommodation for **Form | Rendered | Source** on the artifact detail host.

This ADR records **architecture only**. It does **not** authorize `src/` implementation, UI, persistence schema for operational drafts, Project switching implementation, or Form Runtime.

## Decision

### 1. Derived operator projections (no competing lifecycle)

The following are **derived operator projections** produced by the Core product shell (or shell services). They are **not** canonical work entities, **not** persisted governed lifecycle records, and **not** an independent workflow engine:

| Projection | Role |
|---|---|
| **Project Overview** | Concise situational summary for the active Project |
| **Current Work** | Operator-facing focus on one or more in-flight governed operations |
| **Attention** | Faceted aggregation of conditions requiring or inviting operator awareness |
| **Next Action** | Deterministic presentation of legitimate actions from authoritative state |

ProjectConcord **SHALL NOT** introduce for presentation convenience:

- a `CurrentWork` canonical or operational entity type;
- a generic `WorkItemStatus` or generic ProjectConcord work lifecycle;
- hidden transition rules that substitute for governed package consumption, **DevelopmentWorkAuthorization**, STOP, validation, or EDF semantics.

Existing distinctions **MUST** be preserved in derivation inputs and in operator copy:

| Concept | Layer | Role |
|---|---|---|
| **ProjectWorkRecord** | **A** | Work **identity** only — not authority ([ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md)) |
| **HumanInitiatedWorkItem** | **B** | Intake/triage — not PWR, not EDF AWI |
| **DevelopmentWorkAuthorization** | **B** | Execution **authorization** — distinct from handover and PA review |
| **Relay continuity / packages** | **A** (+ **B** profile) | Governed interaction state — not a generic task system |
| **MVR / gates / EGR** | **F** | Canonical governance semantics — consumed, not redefined |
| **Architectural review / submissions** | **B** | Operational return-for-review — not DWA by default |
| **EDF AWIs / gaps** | **F** / register | Architectural concern tracking — not HIW |
| **InterProjectHandover / CrossProjectDependency** | **B** | Inter-project governance — distinct from intra-project relay legs |

**Work identity does not confer authority.**

### 2. Current Work

A Project **MAY** have **multiple concurrent governed operations** (for example distinct relay interaction chains, intake items, or cross-project blocked work).

**Current Work** is a **derived** presentation of active governed operations relevant to the operator. The shell **MAY** designate one operation as the operator's **primary focused** Current Work while keeping other active operations **discoverable**.

**Primary focus** is **presentation / user-focus state** (for example a selected card or pinned operation). It is **not** governance state. ProjectConcord **SHALL NOT** invent governance priority semantics solely to choose which operation appears primary.

When **ProjectWorkRecord** exists, it remains **identity** correlation only. **DevelopmentWorkAuthorization** remains distinct execution authority. Relay package kinds and validation state remain governed-interaction authority at the relay boundary.

Operator-facing **Current State**, **Waiting On**, and **Baseline** (illustrative labels) are **derived projections** over the same authoritative inputs as Current Work and Attention — **not** duplicate canonical or operational records stored for UI convenience.

**Waiting On** **MAY** reflect distinct underlying conditions (for example Engineering Agent action, Project Architect review, human observation or authorization, provider response, external capability unavailability, **Governed Interaction Hold**, or other workflow-required actor/action) without introducing a universal actor framework in this ADR.

### 3. Attention

ProjectConcord **SHALL** define one conceptual **Project-level Attention** projection.

Attention **MAY** aggregate independently authoritative conditions from sources including:

- governed relay (validation, STOP, pending imports, continuity advisories);
- MVR pending manual QA ([SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md));
- gates / EGR ([F]);
- architectural review and authorization boundaries (**B**);
- **InterProjectHandover** / **CrossProjectDependency** (**B**);
- other governed sources as capabilities permit.

Attention is **derived**. It is **not** a competing canonical lifecycle and **not** a substitute store for relay, MVR, or EDF state.

Each attention item **SHOULD** support facets where legitimately derivable, including:

- **source / domain** (relay, verification, architecture, dependency, …);
- **responsible actor** (operator, Project Architect, Engineering Agent, external Project, …);
- **blocking** vs **non-blocking**;
- **required** vs **recommended** vs **informational**;
- severity or urgency **only** when derivable from authoritative rules — not invented rankings.

Contextual areas (Architecture, Verification, Development, …) **MAY** display **filtered subsets** of the same underlying Attention conditions. Those areas **MUST NOT** become separate authoritative queues with independent lifecycle semantics.

### 4. Next Action

**Next Action** **SHALL** be **deterministically derived** from applicable authoritative canonical and operational state.

The shell **SHALL** preserve explicit distinctions among:

| Class | Meaning |
|---|---|
| **Required Action** | Progression or compliance blocked until this action is satisfied per governing rules |
| **Available Action** | Legitimate action the operator may perform now without implying completion of a required step |
| **Recommended Action** | Suggested action that does not carry required semantics (for example copy transport text) |

These classes **MUST NOT** be silently collapsed.

**Constraints** on what may be presented include: active **STOP**; missing or invalid **DevelopmentWorkAuthorization** when implementation-directed work is claimed; review requirements; validation failures; inter-project blocks; human-attestation boundaries for MVR ([ADR-0014](ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) **Proposed**).

ProjectConcord **MUST NOT** imply **implementation authorization** merely because work exists, a Project Architect review package was produced, an operation is displayed as Current Work, or a Recommended Action is shown.

No hidden workflow engine is authorized. Legitimate actions are those already permitted by relay consumption/production rules, B-layer authorization, and F-layer governance — not new transitions invented in the shell.

**Recovery binding:** Next Action **SHALL NOT** be reconstructed from PA/EA conversational history, model memory, handover prose, or ephemeral UI state. It **SHALL** be derived only from authoritative inputs listed in [§15](#15-governed-work-state-recovery-authoritative-inputs).

### 5. Governed workflow navigation (workspace + visible progression)

For governed interactions whose stages are **structurally known** (for example Core Governed Interaction Relay package legs per [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17), the product shell **SHALL** support a **workflow-local** staged or stepper presentation within the broader Project workspace.

Target model:

```text
workspace navigation
      +
visible governed progression (workflow-local)
```

This is **not** required to be a modal linear wizard. The operator **MAY** move among global Project areas while a workflow-local progression indicator reflects relay stage completion and eligibility.

**Raw relay/package transport** (machine JSON, fenced blocks, full rendered package text) **SHALL** remain available through **progressive disclosure** for diagnostics, expert inspection, manual transfer, and interoperability. Raw transport **MUST NOT** remain the **primary** production operator workflow UX ([AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) §B).

### 6. Product shell navigation layers

The architecture **SHALL** separate:

| Layer | Scope |
|---|---|
| **Project-level navigation** | Persistent areas for the active Project (Overview, Attention, Governed Work, domain-facing areas, …) |
| **Workflow-local navigation** | Stage/stepper or equivalent within one governed operation |
| **Artifact/document navigation** | Semantic artifact identity, relationships, and detail host (including future view modes) |

**Information architecture** follows **governance and workflow semantics**. It **MUST NOT** simply reproduce the repository filesystem tree as the primary navigation model. Filesystem paths **MAY** appear as **secondary locators** ([ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)).

Normative **responsibility** areas (user-facing labels may vary):

| Area | Primary architectural basis |
|---|---|
| **Overview** | Derived Project Overview + Current Work summary |
| **Attention** | Derived Attention projection |
| **Governed Work** | Relay continuity, B-layer operations, workflow-local progression |
| **Architecture** | F-layer ADRs, AWIs, architectural analysis — semantic index |
| **Specifications** | F-layer specs and requirements |
| **Verification** | MVR and verification records ([SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)) |
| **Development** | B-layer execution context and evidence (when enabled) |
| **Dependencies** | **CrossProjectDependency** / **InterProjectHandover** projections |
| **Artifacts** | Canonical browse by semantic identity |
| **History** | Provenance, activity, audit-friendly timelines |

### 7. Progressive disclosure

**Summary before detail** is normative for primary operator surfaces.

Primary surfaces **SHOULD** expose information needed to: understand Project state; understand Current Work; identify required Attention; understand blockers; understand who or what is awaited; determine legitimate Next Actions.

Supporting detail **SHALL** remain inspectable, including: canonical identifiers; provenance; validation diagnostics; raw packages; machine representations; source Markdown; histories; evidence.

**Human meaning** (titles, statuses, role names per [ADR-0018](ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md)) **SHOULD** normally precede implementation and governance shorthand in operator-facing presentation.

### 8. Capability-graded projections

Target projection **contracts** (Overview, Attention, Current Work, Next Action, navigation bindings) are defined by this ADR **without** requiring completion of the future full EDF artifact index or M2+ engine ([SPEC-001](../../Specifications/features/SPEC-001-mvp-edf-desktop-client.md)).

Implementations **SHALL** grade projections according to **authoritative sources currently available** (for example relay operational store today; MVR index tomorrow).

When ProjectConcord lacks an authoritative source for a datum, it **SHALL** represent that datum as **unavailable**, **unknown**, or **absent** per semantics — **MUST NOT** infer or fabricate authoritative state.

Current **relay-only** capability **MUST NOT** redefine the target shell architecture; it only limits what projections can populate until further sources exist.

### 9. Operational draft state (noncanonical)

**Uncommitted / import-pending operator input** (for example paste buffers, partial form entries not yet written to canonical Markdown) is **noncanonical draft state**.

Draft state **MAY** be:

- **ephemeral** (session-only); or
- **durably operational** (survives restart),

according to the applicable **interaction contract** for that surface.

Persisting draft content **DOES NOT**:

- make it canonical;
- make it a governed relay package;
- constitute successful import;
- create provenance as a successful governed interaction;
- confer authority.

Exact draft persistence schema, retention, cleanup, security, and implementation remain **deferred**.

### 10. Single-Project Focus and replaceable active Project context

**Single-Project Focus** ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) §1) is preserved: one application instance maintains **at most one active Project context** at a time. ProjectConcord **SHALL NOT** introduce multi-project tabs, simultaneously active Project workspaces, or multi-project workspace ownership semantics in one instance.

**Replaceable active Project context:** Single-Project Focus **does not** require that the active Project remain fixed for the **lifetime of the application process**. The product shell and Project-scoped application architecture **MUST NOT** assume permanent process-lifetime attachment to one Project.

Project-specific state **MUST** remain explicitly **Project-scoped** so that a future capability may conceptually:

```text
deactivate Project A
  → persist/dispose applicable Project A operational context
activate Project B
  → load Project B canonical + operational state
  → reconstruct derived Project B projections
  → restore the Project B workspace
```

This is an **architectural accommodation only**. It **does not** authorize Project switching implementation, simultaneous multiple active Projects, or any particular persistence representation (no prescribed JSON/SQLite/filesystem layout).

Application- or user-global state **MAY** legitimately remain outside Project scope (for example recent Project registry, application preferences, window presentation preferences, provider configuration) per applicable architecture ([ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md), [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)).

### 11. Inter-project operator UX

Inter-project governed work **SHALL** continue to use **CrossProjectDependency** and **InterProjectHandover** ([ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)) with transport via governed relay/packages — not a new protocol.

The **originating** instance **retains** its active Project context while dependent governed work proceeds in another **independently active** Project context (typically another instance).

Target product architecture **MAY** provide an action conceptually equivalent to **Open dependent Project in new ProjectConcord instance**. **Project switching** in the originating instance is **not** the required mechanism for cross-project dependency **execution**. Switching active Projects and handling inter-project dependencies are **separate** concerns.

Manual launch documentation may suffice interim; it is **not** the target UX. Process-launch mechanics remain implementation detail.

### 12. Structured authoring separation (shell integration only)

[AWI-0009](../Watch_Items/AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) owns deferred structured-authoring architecture.

The Core **artifact detail host** **SHALL** be architecturally capable of accommodating **Form | Rendered | Source** without assuming humans primarily author governed artifacts by manually editing Markdown source. Form Runtime architecture is **out of scope** for this ADR.

### 13. Architectural ownership

| Concern | Owner |
|---|---|
| Product shell, global navigation, Overview / Attention / Current Work / Next Action projections, terminology projection in UI | **Core (A)** |
| Relay envelope, validation, provenance, package kinds | **Core Governed Interaction Relay (A)** |
| DWA, HIW, IPH, CPD, submissions, software profile payloads | **Software Development (B)** |
| EDF artifact and governance semantics | **F** — consume/project only |
| Provider transport and session integration | **E** |
| Working Environment policy facets | **D** — separate from engineering Domain |
| Pure visual layout and theming | Presentation |

Software Development **authorization** semantics **MUST NOT** move into Core for dashboard convenience.

### 14. State classification summary

| Kind | Examples |
|---|---|
| **Canonical (Git/EDF)** | ADRs, specs, MVR files, accepted gate state in repo |
| **Persisted operational** | Relay packages, continuity, provenance; future B entities; optional durable **draft** (noncanonical) |
| **Derived projections** | Overview, Attention, Current Work, Next Action, workflow stage presentation |
| **Ephemeral UI** | Scroll position, panel expansion, transient focus unless persisted as user preference |
| **External (C)** | Provider chat — not canonical |

### 15. Governed work-state recovery (authoritative inputs)

#### 15.1 Recovery invariant

ProjectConcord **SHALL NOT** require PA/EA chat history, model memory, handover prose, or UI state to determine the current governed work state after restart, interruption, provider-session loss, or operator return.

Recoverable governed state **SHALL** derive from:

1. **authoritative canonical/project evidence** where applicable ([F] — Git/EDF, gates, MVR, applicable baseline references); and  
2. **authoritative persisted operational state** ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md), [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)).

Operator projections (Overview, Current Work, Attention, Next Action, workflow-local stage presentation) are **derived** from those sources. They are **not** themselves authority.

**LLM inference** **MUST NOT** substitute for missing authoritative inputs ([§8](#8-capability-graded-projections)).

#### 15.2 Minimum authoritative facts (binding, not a new entity model)

Sufficient durable state **SHALL** exist — stored directly or **deterministically derivable** — to support recovery of the applicable:

| Fact category | Primary architecture |
|---|---|
| Project identity | [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md), PC-PAR-001–004 |
| Prescribed workflow identity/version | [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §2, [ADR-0024](ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) |
| Workflow profile / effective configuration | PCON-0005 §4 — [GAP-054](../../Development/EDF_Gap_Register.md) |
| Workflow-instance identity and instance state | PCON-0005 §2.5, §9 — **FW-8** |
| Current synchronization / topology position | PCON-0005 §2.6, §6 — **FW-8** |
| DWA / **Control** authorization boundary | [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [GAP-027](../../Development/EDF_Gap_Register.md) |
| Operation baseline vs current repository HEAD | PCON-0005 §9.1 |
| STOP / Attention conditions | PC-AIGOV-007, §3 Attention inputs |
| Outstanding required actor/action | Derived from instance state, relay, GIES, MVR, inter-project (**B**) |
| Relay produce/consume provenance position | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-021, §19 |
| GIES context / hold / blocking items (when active) | [PCON-0006](../PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) — [GAP-055](../../Development/EDF_Gap_Register.md) |
| Provider correlation (transport recovery) | [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) — not governance identity |

Facts **MAY** be distributed across canonical and operational stores per [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md). Redundant persistence of derivable facts is **not** required.

#### 15.3 Pause / continuation boundary

Ordinary work-state recovery **does not** depend on [PCON-0003](../PCON-0003-Governed-Pause-Continuation-and-Resume.md) (**Proposed**) or candidate PC-AIGOV-052–057. Broader governed pause, continuation, resume, and long-duration suspension remain on [AWI-0002](../Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md).

**Governed Interaction Hold** ([PCON-0006](../PCON-0006-Governed-Interactive-Engineering-Session-Framework.md)) is **not** governed pause under PCON-0003.

#### 15.4 Stale packages (cross-reference)

Relay import **non-regression** and **Valid** vs **actionable** applicability are normatively bound in [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §19 (PC-PAR-013, PC-PAR-014). Workflow-instance durability for applicability is bound in PCON-0005 **FW-8**.

## Consequences

- Future shell implementation tranches **MUST** implement projections as read models over **A** + **B** (+ **F** consumption), not as new lifecycle stores.
- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) operator-stage binding (§17) aligns relay package kinds with workflow-local presentation without altering relay authority.
- [AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) remains **Active** for implementation tracking until shell UX is authorized and delivered; operator projection semantics are **promoted** here.
- Relay governance invariants (machine JSON authority, malformed rejection, STOP, validation, authorization boundaries, provider neutrality) remain unchanged.

## Related Documents

- [AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
- [AWI-0009](../Watch_Items/AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md)
- [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)
- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [PCON-0000](../PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) §37–38 (reconciled)
- [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) (**Accepted** 2026-10-03 — workflow framework; operator projections remain derived)
- [Work-state recovery binding tranche plan](../../Handover/ProjectConcord-Durable-Governed-Work-State-Recovery-Binding-Tranche-Plan.md)
- [ADR-0024](ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03 — GEW prescribed workflow)
