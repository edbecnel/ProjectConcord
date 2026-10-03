[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Specifications](../README.md) › SPEC-004

# SPEC-004: AI-Assisted Development Governance Workflow

## Metadata

| Field | Value |
|---|---|
| **Spec ID** | SPEC-004 |
| **Status** | Draft |
| **Owner** | ProjectConcord |
| **Normative** | Yes — product behavior requirements when implemented |
| **Implementation** | **Not implemented** — requirements define future M7+ capability unless separately authorized |
| **Last Reviewed** | 2026-10-03 (GEW v1 topology / durable traversal — [M7a-WF-1-doc](../../Handover/ProjectConcord-M7a-WF-1-doc-GEW-Topology-Documentation-Tranche-Plan.md); GEW/GIES consumption published earlier same day) |
| **Governing decisions** | [ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) (Accepted), [ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) (Accepted 2026-09-29), [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (Accepted 2026-09-29 — Software Development governed workflow), [ADR-0019](../../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) (Accepted 2026-10-01 — product shell / local-first / sync), [ADR-0006](../../Architecture/ADRs/ADR-0006-AI-Boundary.md), [ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) (**Accepted** 2026-10-03), [ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03), [PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) (**Accepted** 2026-10-03), [ADR-0025](../../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) (**Accepted** 2026-10-03 — GIES layer allocation) |
| **Architecture scope** | **Software Development / Engineering Extension** — not ProjectConcord Core neutrality ([AMD-0003](../../Architecture/AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)) |
| **Discovery source** | [PCON-0001](../../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) |

**Integration closeout (2026-09-21):** PCON-0001 handover integrated; documentation tranche Project Architect **accepted** at commit `b728e2896992b58ee785d406ac93a6badf29c8c8` per [AI Governance Workflow Integration Analysis](../../Architecture/AI_Governance_Workflow_Integration_Analysis.md). This specification remains **Draft** and **not implemented**. [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) **Accepted** 2026-09-29 (Software Development governance architecture). Implementation gated per [Implementation Roadmap](../../Development/Implementation_Roadmap.md) M7+.

## Problem

Engineering teams using an Architectural AI and a repository execution agent (for example GPT and Cursor) need **governed** coordination: explicit authorization, evidence, and human authority — without making chat transcripts or provider prompts canonical. Work may span **multiple projects** with destination-controlled acceptance and traceable cross-project dependencies.

## Goals

- Model canonical governance state separately from derived AI/provider transport.
- Separate handover context from DevelopmentWorkAuthorization.
- Support manual and integrated provider modes with identical semantics.
- Support **cross-project** Software Development governance (IPH, CPD, traceability) with **Single-Project Focus** per application instance and **Concurrent Project Work** across instances ([ADR-0019](../../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)); **not** Working Environment ([ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)).
- Preserve cross-project provenance and destination governance authority.

## Non-Goals

- Replace Git, IDEs, or AI providers (PCON-0001 §45).
- Autonomous architectural acceptance or gate closure.
- Mandatory AAR for every implementation tranche ([ADR-0012](../../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md) scope unchanged).
- Upstreaming PC-AIGOV-022–028 to EDF in this tranche.
- MVP (M1–M5) delivery of governance UI or inter-project features.
- Defining workflow runtime APIs, registry implementation, or effective-configuration resolver ([GAP-054](../../Development/EDF_Gap_Register.md)).
- Copying or redefining EDF GMFP/GMR normative text ([AWI-0004](../../Architecture/Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md)).
- Operator UI for workflow/profile selection beyond derived projections ([ADR-0020](../../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)).
- GIES runtime, Interaction Hold persistence, or routing implementation ([PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [GAP-055](../../Development/EDF_Gap_Register.md)).
- SPEC-006 interaction request/answer packages or relay schemas ([ADR-0025](../../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) §5).

## Terminology (workspace-adjacent — not synonyms)

| Term | Owner | Role in this spec |
|---|---|---|
| **Single-Project Focus** | **Core (A)** | One **active** Project per application instance ([ADR-0019](../../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)). |
| **Concurrent Project Work** | **Core (A)** | Multiple instances; each owns one active Project ([ADR-0019](../../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)). |
| **Working Environment** | **D** | Composable policy/methodology facets — **not** multi-project hosting. |
| **Project Root / session locator** | **A** (detail [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) **Accepted** 2026-09-29, [SPEC-006](SPEC-006-par-project-root-and-governed-workflow-relay.md)) | Session binding to a filesystem locator. |
| **Git worktree / branch** | **B** | Software Development execution/evidence context. |
| **Provider / agent session** | **E** | Transport and adapter session state. |
| **Persona / UI workspace** | UI ([GAP-036](../../Development/EDF_Gap_Register.md)) | UX-only; not governance semantics. |
| **Prescribed Workflow / Workflow Profile** | Framework + GEW ([PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md), [ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)) | **GEW** (`concord.governed-engineering`) is the applicable ProjectConcord-native prescribed workflow for Software Development governance modeled here. **Profiles** (`gew.accelerated`, `gew.standard`, `gew.high-assurance`) are GEW-local topology variants — not global Concord modes or quality rankings. |
| **Effective configuration** | Derived ([PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §4) | Deterministic from authoritative inputs when possible; not a user-editable superset of governance rules. |
| **GMFP (consumed workflow)** | EDF authoritative; ProjectConcord integrates | Separate prescribed workflow `edf.governed-maintenance-fast-path` — not a GEW profile ([ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §7). |
| **GIES (framework capability)** | [PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [ADR-0025](../../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) | Governed Interactive Engineering Session — reusable interaction infrastructure; **not** a competing “session” identity. GEW **consumes** GIES per § GEW consumption of GIES; semantics **not** redefined here. |

## M1–M5 architectural constraint

When M1–M5 foundational code is implemented, it **MUST NOT** establish implicit or irreversible **global** operational state — persistence and APIs **MUST** remain **Project-ID-scoped** and compatible with **multiple application instances** and inter-project workflows ([ADR-0019](../../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)). This does **not** require a multi-project workspace in one process.

Specific identity, API scoping, and persistence mechanisms are **not** prescribed by this spec; see [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (non-lock-in cross-reference), [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Accepted** 2026-09-29), and [Implementation Roadmap](../../Development/Implementation_Roadmap.md).

## Stage 1 ownership (summary)

| Concern | Owner (A–F) |
|---|---|
| PC-AIGOV-001–004, 007, 014–016, 072–078 (governance invariants + GEW/GIES placement) | **A** relay + **B** Software Development semantics under GEW (`concord.governed-engineering`); GIES framework per [PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md); enforcement via [SPEC-006](SPEC-006-par-project-root-and-governed-workflow-relay.md) at relay boundary |
| PC-AIGOV-022–023 (cross-project / per-project partitions) | **A** instance model per [ADR-0019](../../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); **B** requirements assume per-project SD partitions when extension enabled |
| **ProjectWorkRecord** (working name; Core per [ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) Accepted) | **A** — coordination identity; not task/backlog ontology ([AMD-0004](../../Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md)) |
| DevelopmentWorkAuthorization, submissions, inter-project entities, **HumanInitiatedWorkItem** | **B** — Software Development extension (operational store) |
| Generic delegation primitive | **A** — conceptual only; working name *AuthorityGrant*; **no schema**; repository term **DevelopmentWorkAuthorization** until migration |
| Provider manual/integration modes (PC-AIGOV-005–006) | **E** |
| Backlog / sprint / story / epic / task / ranking (methodology) | **C** — not Core; operates over work identities when implemented |

## Logical entities (when implemented)

| Entity | Default store | Purpose |
|---|---|---|
| DevelopmentWorkAuthorization | Operational | Capability-bounded **Control** for Software Development per project (**B**); correlates conceptually with workflow instance (`WorkflowId`, `DefinitionVersion` where applicable, `ProfileId`, `WorkflowInstanceId`), bounded configuration, and effective-configuration provenance — **schema/API not prescribed** ([GAP-027](../../Development/EDF_Gap_Register.md)) |
| Handover package | Derived / operational snapshot | Inherited project context for agents |
| ArchitecturalReviewSubmission | Operational | Plan or implementation return for review |
| HumanInitiatedWorkItem | Operational | Inbox/triage; distinct from AWI and from generic Core **ProjectWorkRecord** (see § Stage 2 intake semantics) |
| InterProjectHandover | Operational | Governed cross-project event |
| CrossProjectDependency | Operational | Ongoing cross-project relationship |
| Evidence / validation records | Operational (**B**) | Software implementation evidence packages; linked to DWA and Git state; optional PWR correlation (**A** references only) |

Destination **EDF artifacts** remain canonical in the target project Git repository only through destination governance (**F**). Architectural acceptance is **F-layer** governance — not Git PR merge alone (PC-AIGOV-026).

## Stage 2 intake semantics (documentation — not normative rename)

Stage 2 architecture ([AMD-0004](../../Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md), [ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) Accepted) establishes that **HumanInitiatedWorkItem** represents **intake/capture/triage**, not the generic Core **ProjectWorkRecord** abstraction. **`OperationalIntakeRecord`** is **preferred working terminology only** for future architecture; final name, type identity, and Core-versus-extension ownership remain **deferred**. This spec **retains HumanInitiatedWorkItem** as normative entity naming until a separately governed migration.

An intake record (HIW) may result in: creation of a Project Work Record; association with an existing Project Work Record; merging into existing work/intake; routing toward another governed process; or rejection/disposition **without** creating a Project Work Record.

## Governed Engineering Workflow (GEW) scope

Software Development governance requirements in this specification **SHALL** be interpreted as requirements on work under the **Governed Engineering Workflow (GEW)** — the first ProjectConcord-native **prescribed workflow** registered as:

| Field | Value |
|---|---|
| **WorkflowId** | `concord.governed-engineering` |
| **Architecture** | [ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted**) |
| **Framework** | [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) (**Accepted**) |

GEW is the **applicable** native prescribed workflow for the governance modeled by PC-AIGOV-001–028 and PC-AIGOV-072–076. ProjectConcord **MAY** host additional prescribed workflows (including EDF-consumed workflows); GEW is **not** asserted to be the only workflow the platform can ever host.

Read-only investigation and research **SHALL NOT** be modeled as a GEW profile; represent through **Control** / DWA capability (for example RESEARCH / PLANNING), instance state, and relay routing ([ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §5; [SPEC-006](SPEC-006-par-project-root-and-governed-workflow-relay.md)).

### GEW profiles

GEW defines three initial **workflow profiles** (working identifiers):

| ProfileId | Name | Role in this spec |
|---|---|---|
| `gew.accelerated` | Accelerated | Bounded, reversible, low-uncertainty engineering; permitted **synchronization points**, **evidence requirements**, and related **artifacts** **MAY** be compressed or combined where profile and authoritative policy permit — **without** weakening GEW invariants, **authorization law** (PC-AIGOV-004), or inherited framework constraints. Compression of sync/evidence/artifacts is **not** the same as collapsing planning and implementation **authorization** unless explicit profile or policy semantics allow a combined authorization boundary (**open** — see PC-AIGOV-004). |
| `gew.standard` | Standard | Normal substantive governed engineering. |
| `gew.high-assurance` | High Assurance | Cross-cutting, novel, high-consequence, or otherwise high-risk engineering; stricter synchronization and evidence floors **when** profile-fixed rules require them. |

Profiles are **GEW-local**, **not** global ProjectConcord modes, and **not** quality rankings. **Standard is not an implicit fallback:** profile resolution **SHALL** follow [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §8.2 — authoritative policy or state **MAY** resolve deterministically; otherwise governed or PA selection is required; ambiguity **SHALL** produce Attention and **SHALL NOT** silently select High Assurance, Standard, or any other profile.

**Open (normative detail deferred):** per-profile minimum synchronization sets and evidence-class floors beyond what GEW invariants already require are **not** fully enumerated in this specification; whether **Accelerated** normatively permits a combined planning/implementation **authorization boundary** for some bounded work units (distinct from sync/evidence compression) is **not** fully specified here — future governed policy or amendments **MAY** define that without using bounded configuration to invent authorization law.

### GEW v1 topology and durable position (recovery cross-reference)

Normative **GEW v1** definition places, transitions, and **workflow-instance traversal** (`TopologyPlaceId`, `TraversalOccurrenceId`) are defined in [ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §9–§13. Framework separation of definition topology, instance traversal, orthogonal STOP/sync/AEI state, and derived projections is in [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §2.7.

When implemented, durable recovery of governed work state **SHALL** use those semantics for **FW-8** / operator recovery ([ADR-0020](../../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15) — **not** PA/EA chat history. This specification **does not** prescribe runtime registry APIs or SQLite layout ([GAP-054](../../Development/EDF_Gap_Register.md)).

## Configuration classes and effective configuration

For GEW work units, operational semantics **SHALL** align with [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §3:

| Class | SPEC-004 touchpoints (examples) |
|---|---|
| **Invariant** | PC-AIGOV-001–004 (where applicable), 007, 014, 016; framework FW-1–FW-7 |
| **Bounded Configuration** | DWA scope and authorized operations within limits already established by workflow, profile, and policy; baseline reference (PC-AIGOV-009) — **SHALL NOT** weaken, remove, invent, or silently redefine **authorization law** (PC-AIGOV-004) |
| **Policy-Derived** | Linked MVR / human verification when F-layer policy requires; risk- or scope-derived evidence |
| **Instance State** | Topology traversal and orthogonal STOP/sync/AEI/control state per [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §2.7; **Governed Interaction Hold** ([PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md)); pending disposition — **not** configuration knobs |

There is **no** unrestricted workflow-configuration surface. A lower layer **MUST NOT** weaken an inherited governance constraint.

Effective configuration **SHALL** be deterministically derived from authoritative inputs when possible ([PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §4; PC-AIGOV-075). Operator projections (Attention, Next Action, workflow-local step labels) **SHALL** remain derived ([ADR-0020](../../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)); this spec **does not** define a global Stage library.

## GMFP boundary (separate prescribed workflow)

**Governed Maintenance Fast Path (GMFP)** is architecturally a **separate prescribed workflow** consumed by ProjectConcord, with working **WorkflowId** `edf.governed-maintenance-fast-path`. GMFP is **not** a GEW profile, **not** Accelerated GEW, **not** a bypass around unrelated GEW/EGR/GDO/AAR/ADR/SPEC obligations, and **not** a substitute for destination governance.

EDF remains **authoritative** for GMFP and **GMR** semantics. This specification **does not** copy EDF GMFP normative text. PC-AIGOV-004 and related GEW planning/implementation separation **SHALL NOT** be reinterpreted to impose an intermediate GEW planning gate on GMFP work that EDF does not require ([ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §7; [AWI-0004](../../Architecture/Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md)).

## GEW consumption of Governed Interactive Engineering Session (GIES)

[PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) defines **GIES**; [ADR-0025](../../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) allocates layers. This section states **GEW-specific** constraints when GIES is used during governed Software Development. It does **not** copy the full PCON-0006 model.

### Applicability

GEW **MAY** use GIES during an active **governed operation** when an information, observation, judgment, authorization, or escalation need cannot be satisfied immediately **and** the interaction must participate in governed operation state, routing, provenance, authorization, evidence, or resume semantics.

GIES is **not** mandatory for every GEW operation. Not every informal question **SHALL** be modeled as an **Interaction Item**.

Conceptual structure (see PCON-0006): **Governed Operation** → **Governed Interaction Context** → **Interaction Set** → **Interaction Item(s)**. Unresolved **blocking** items **MAY** place the operation in **Governed Interaction Hold**.

### GEW / GIES contract (minimum)

| Theme | GEW obligation |
|---|---|
| **Pre-resolution (GIES-1)** | Before PA or human solicitation, consult applicable authoritative artifacts, deterministic Concord state, and policy-derived state — including applicable MVR or equivalent F-layer evidence. Do **not** ask humans to repeat observations already authoritative in such artifacts. |
| **Routing (GIES-2)** | Policy-governed per-item routing; **no** fixed EA→PA→Human→PA→EA chain. Direct human observation **MAY** be permitted when workflow/policy establishes observation-only, no implied authorization, and no required PA interpretation — **no** universal Standard/HA/Accelerated default. |
| **Authorization (GIES-3, PC-AIGOV-004)** | GIES **SHALL NOT** bypass GEW authorization law. Interaction answers are **not** authorization unless recorded through applicable **Control** / **DevelopmentWorkAuthorization** / workflow governance. GIES **SHALL NOT** resolve Accelerated combined planning/implementation authorization (**open**). |
| **Acceptance (PC-AIGOV-014)** | Resolving an Interaction Item or Set does **not** authorize another work unit or implementation step. |
| **Provenance (GIES-4, PC-AIGOV-016)** | Operational interaction provenance required; chat/provider transcript **not** authoritative. Durable human observation **SHOULD** use governed evidence mechanisms (for example [SPEC-005](SPEC-005-manual-verification-record-consumption.md) MVR); later steps **SHOULD** reference/consume that evidence. |
| **Hold (GIES-5, PC-AIGOV-078)** | Interaction Hold **≠** **STOP** (PC-AIGOV-007). Hold **SHALL NOT** silently expand DWA scope, authorized operations, profile, or authorization boundaries. What mutation remains permitted during hold is determined by **DWA/Control** and policy — the hold neither grants nor widens authorization. On resolution, reconcile authorization, STOP, Attention, scope, and risk before forward progress. Hold **MAY** lead to STOP when governance requires (denial, scope/risk, unresolved conflict, etc.). |

### GEW profile boundary (GIES)

Profile and authoritative policy **MAY** impose additional mediation, synchronization, or evidence requirements around GIES consistent with Accepted architecture. This specification **does not** define profile-specific GIES mediation, evidence floors, or synchronization rules for `gew.accelerated`, `gew.standard`, or `gew.high-assurance` (**open**).

### Operator projections

Derived states (for example Awaiting Human Input, Awaiting PA Input, Awaiting Authorization, Interaction Conflict, Interaction Incomplete) **SHALL** remain projections per [ADR-0020](../../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) — not defined as UI requirements here.

### Future SPEC-006 reconciliation (identified, not authorized)

When authorized, [SPEC-006](SPEC-006-par-project-root-and-governed-workflow-relay.md) **MAY** add interaction need/answer package profiles and relay validation. Transport **carries** interaction information; it does **not** own GIES governance semantics ([ADR-0025](../../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md)).

## PC-AIGOV applicability by GEW profile (summary)

| Applicability | PC-AIGOV IDs | Notes |
|---|---|---|
| **GEW invariant (all profiles)** | 001–003, 005–008, 010–013, 015–016, 017–028 | Apply to Software Development governance under GEW unless a row below qualifies |
| **Authorization law (planning vs implementation)** | 004 | Whether distinct planning and implementation **authorizations** are required is governed by prescribed workflow + **GEW profile** + **authoritative policy** — **not** bounded configuration. `gew.standard` and `gew.high-assurance`: distinct boundaries apply per profile semantics. `gew.accelerated`: sync/evidence/artifact compression **≠** authorization collapse; combined authorization boundary only via explicit profile/policy (**open**). **Not** universal to every prescribed workflow. |
| **Acceptance vs next authorization** | 014 | Invariant across profiles: accepting one bounded unit/tranche does **not** authorize the next. Profile **MAY** shape which sync/evidence/**artifacts** record acceptance for a unit; **MUST NOT** weaken mandatory authorization or acceptance semantics; bounded configuration **MUST NOT** substitute for those semantics. |
| **Synchronization / evidence / artifact variation** | 008–009, 011 | **Distinct from authorization law.** Accelerated **MAY** compress or combine **permitted** synchronization points and evidence artifacts; High Assurance **MAY** require additional profile-fixed sync/evidence when governed policy defines them — without weakening PC-AIGOV-004/014 invariants. |
| **Provenance (GEW binding)** | 016 | Satisfies framework provenance invariant FW-7 for GEW via Software Development requirements; **not** asserted as a universal framework requirement for all future workflows |
| **Profile resolution** | 073 | Ambiguity → Attention; no silent High Assurance, Standard, or other profile; no implicit Standard ([PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §8.2) |
| **STOP / instance state** | 007 | Instance state — not a profile selector |
| **Cross-project (B-layer)** | 021–028 | Unchanged; assume per-project GEW instances unless work is explicitly under another prescribed workflow |
| **GMFP work units** | 004 (qualified), 076 | GMFP authorization law per EDF; no forced GEW planning/implementation gate |
| **GIES under GEW** | 004, 007, 014, 016, 075, 077–078 | Consume [PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) when applicable; hold ≠ STOP; answers ≠ authorization; profile GIES detail **open** |

## Requirements (PC-AIGOV-001–078)

### Workflow framework (GEW placement)

| ID | Requirement |
|---|---|
| **PC-AIGOV-072** | Software Development governance requirements in this specification **SHALL** apply to work executed under GEW (`concord.governed-engineering`) as the applicable ProjectConcord-native prescribed workflow, without implying that no other prescribed workflow may govern other work units or future scope ([PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §1). |
| **PC-AIGOV-073** | A GEW workflow instance **SHALL** use ProfileId `gew.accelerated`, `gew.standard`, or `gew.high-assurance` when profile determination is required. Profile resolution **SHALL** follow [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §8.2 (no silent profile selection; no implicit Standard). |
| **PC-AIGOV-074** | GEW bounded configuration **SHALL NOT** introduce unrestricted workflow configuration; classes **SHALL** align with Invariant, Bounded Configuration, Policy-Derived, and Instance State ([PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §3). Bounded configuration **SHALL NOT** weaken, remove, invent, or silently redefine **authorization law** (PC-AIGOV-004) established by prescribed workflow, profile, or authoritative policy. |
| **PC-AIGOV-075** | When authoritative inputs suffice, the system **SHALL** derive effective configuration per [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §4 and **SHALL NOT** require operators to re-enter information already available from authoritative governed state (resolver implementation: [GAP-054](../../Development/EDF_Gap_Register.md)). |
| **PC-AIGOV-076** | Work explicitly under `edf.governed-maintenance-fast-path` (GMFP) **SHALL NOT** be classified as GEW or as a GEW profile. GMFP semantics remain EDF-authoritative; this spec does not redefine GMFP. |
| **PC-AIGOV-077** | When GEW uses **Governed Interactive Engineering Session** ([PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md)) for a governed operation, GEW **SHALL** conform to framework invariants **GIES-1** through **GIES-6** and the GEW consumption constraints in § GEW consumption of GIES. GIES is **not** required for every GEW operation — only when interaction must participate in governed state, routing, provenance, authorization, evidence, or resume semantics. |
| **PC-AIGOV-078** | A **Governed Interaction Hold** on a GEW governed operation **SHALL NOT** silently expand **DevelopmentWorkAuthorization** scope, authorized operations, **ProfileId**, or authorization boundaries. The hold **SHALL NOT** grant or widen authorization. Before the governed operation resumes forward progress after hold resolution, the system **SHALL** reconcile applicable authorization, **STOP**, **Attention**, scope, and risk per GEW requirements. |

### Authority and canonical state

| ID | Requirement |
|---|---|
| **PC-AIGOV-001** | AI recommendations and execution SHALL NOT implicitly become project authorization or acceptance. |
| **PC-AIGOV-002** | Governance state SHALL be represented independently of AI chat transcripts and provider prompts. |
| **PC-AIGOV-003** | Inherited project context (handover) and permitted work (DevelopmentWorkAuthorization) SHALL be distinct concepts. |
| **PC-AIGOV-004** | Planning authorization SHALL NOT imply implementation authorization. **GIES:** An Interaction Item answer or resolution **SHALL NOT** silently constitute planning or implementation **authorization** — authorization **SHALL** be recorded through applicable **Control** / **DevelopmentWorkAuthorization** / workflow governance (PC-AIGOV-077, [PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) GIES-3). **Authorization law** — whether planning and implementation require **distinct authorizations** — is determined by the applicable **prescribed workflow**, **GEW profile**, and **authoritative policy**, not by operator-selectable **bounded configuration**. Bounded configuration (including DWA scope and authorized operations) **SHALL NOT** weaken, remove, invent, or silently redefine that authorization law. **GEW:** For `gew.standard` and `gew.high-assurance`, distinct planning and implementation authorization boundaries apply as those profiles’ authorization semantics require. For `gew.accelerated`, compression or combination of **synchronization points**, **evidence requirements**, and related **artifacts** where permitted does **not** by itself eliminate distinct planning and implementation authorization; any **combined authorization boundary** for a bounded work unit requires explicit **profile or authoritative policy** semantics (**open:** whether Accelerated permits such a combined boundary for some work units is not fully enumerated in this specification). **Not universal:** This requirement is **not** a universal rule for every ProjectConcord prescribed workflow. **GMFP:** Work under `edf.governed-maintenance-fast-path` follows EDF authorization law and SHALL NOT be forced through a GEW-style planning/implementation gate that EDF does not require ([ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §7). |
| **PC-AIGOV-014** | Acceptance of one stage/tranche SHALL NOT implicitly authorize the next. **GIES:** Resolution of an Interaction Item or Interaction Set **SHALL NOT** implicitly authorize a subsequent work unit or implementation step. **GEW applicability:** Invariant across GEW profiles. A profile **MAY** determine which **synchronization points**, **evidence**, or **artifacts** record acceptance of a bounded unit of work; such variation **SHALL NOT** weaken mandatory **authorization** or **acceptance** semantics established by the workflow, profile, and authoritative policy. **Bounded configuration** **SHALL NOT** substitute for those semantics or imply authorization for a subsequent unit. |
| **PC-AIGOV-016** | Governed work SHALL retain provenance sufficient to reconstruct authorization, execution, validation, and acceptance. **GIES:** GEW **SHALL** satisfy operational interaction provenance per [PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) GIES-4 when GIES is used; provider/chat transcript **SHALL NOT** be authoritative; durable observations **SHOULD** bind to F-layer evidence (for example MVR per [SPEC-005](SPEC-005-manual-verification-record-consumption.md)). **GEW applicability:** Binds GEW to framework provenance expectations ([PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) FW-7 via [ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)); does **not** by itself establish PC-AIGOV-016 as a universal requirement on non–Software-Development prescribed workflows. |

### Provider modes and execution

| ID | Requirement |
|---|---|
| **PC-AIGOV-005** | Provider integrations SHALL adapt to a provider-neutral governance protocol. |
| **PC-AIGOV-006** | Manual copy/paste workflow SHALL preserve the same governance semantics as direct integration. |
| **PC-AIGOV-007** | Repository agents SHALL be able to enter an explicit STOP state and request disposition without continuing unauthorized mutation. **GIES:** **Governed Interaction Hold** is **not** equivalent to **STOP**; a hold **MAY** precede or lead to **STOP** when governance requires (PC-AIGOV-078). |
| **PC-AIGOV-010** | The system SHOULD identify work performed outside authorized scope (scope conformance). |

### Evidence, baseline, validation

| ID | Requirement |
|---|---|
| **PC-AIGOV-008** | Implementation evidence SHALL be traceable to authorization and repository state. |
| **PC-AIGOV-009** | Authorizations SHALL be associated with a baseline sufficient to detect material repository drift. |
| **PC-AIGOV-011** | Automated, agent-interactive, and human/operator validation SHALL be distinguishable. |
| **PC-AIGOV-012** | An agent SHALL NOT record validation as passed when it lacked capability to perform that validation. |
| **PC-AIGOV-013** | Unrelated repository work SHALL NOT be silently committed, discarded, or absorbed into governed work. |
| **PC-AIGOV-015** | Sufficient AI context SHALL be reconstructable from canonical state without complete historical chat transcripts. |

### Human intervention and deviations

| ID | Requirement |
|---|---|
| **PC-AIGOV-017** | The human authority SHALL capture work and observations outside the active governed workflow without silently modifying that workflow. |
| **PC-AIGOV-018** | Human-initiated work SHALL be classifiable and routable (current project, future work, other project, EDF, ProjectConcord, cross-project review). |
| **PC-AIGOV-019** | Out-of-band repository or canonical changes SHALL be surfaced; reconciliation SHALL be required before affected governed work proceeds. |
| **PC-AIGOV-020** | Repository-agent scope deviations SHALL be preserved for review, not silently normalized into authorization. |

### Cross-project governance and inter-project continuity

Requirements PC-AIGOV-021–028 are **Software Development extension (B)** inter-project governance flows. They assume **Project-ID-scoped** partitioning and **inter-project governed continuity** per [ADR-0019](../../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) and [ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md); they do **not** require a multi-project workspace in one application instance ([ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) §7). **InterProjectHandover** and **CrossProjectDependency** MUST NOT be generalized into universal Core Project Work Record relationship types ([ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) §12).

| ID | Requirement |
|---|---|
| **PC-AIGOV-021** | Work routed to another project or framework SHALL preserve origin without becoming part of the originating project's active gate. |
| **PC-AIGOV-022** | Architecture SHALL support multiple independently governed **ProjectConcord projects** concurrently via **multiple application instances** (each instance with **Single-Project Focus** — **A**; [ADR-0019](../../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)). Cross-project awareness SHALL NOT imply multi-project workspace ownership. |
| **PC-AIGOV-023** | Each managed project SHALL retain its own canonical EDF state, repository state, authorization partition, governance lifecycle, and authority boundaries. |
| **PC-AIGOV-024** | A project SHALL be able to originate a governed inter-project handover while preserving source provenance. |
| **PC-AIGOV-025** | Inter-project handover SHALL NOT bypass destination governance; disposition and acceptance occur under destination rules. |
| **PC-AIGOV-026** | Inter-project contributions MAY use target branches, commits, and pull requests; those mechanisms are evidence/collaboration, NOT architectural acceptance. |
| **PC-AIGOV-027** | Inter-project handover events SHALL be distinguished from ongoing cross-project dependencies. |
| **PC-AIGOV-028** | Traceability SHALL be preserved from source discovery through target acceptance and back to dependent source work. |

## Relationship to other specifications

- [SPEC-003](SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) — canonical **artifact** integrity and lifecycle; external IDE edits.
- [SPEC-001](SPEC-001-mvp-edf-desktop-client.md) — M1–M5 MVP does not implement this spec.
- [ADR-0006](../../Architecture/ADRs/ADR-0006-AI-Boundary.md) — AI proposals for EDF writes; complementary.
- [SPEC-006](SPEC-006-par-project-root-and-governed-workflow-relay.md) — Project Root, identity, Core Governed Interaction Relay, provider adapters (**Accepted** 2026-09-29; A2 P0 relay subset **published**; further PAR/A3+ not authorized). SPEC-006 enforces at relay boundary; PC-AIGOV-001–078 remain defined here. Future GIES interaction packages — § GEW consumption of GIES.
- [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md), [ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) — prescribed workflow, profiles, configuration classes (architecture **Accepted**; this spec carries normative Software Development requirements).
- [PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [ADR-0025](../../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) — GIES framework (**Accepted**); GEW consumption in § GEW consumption of GIES.

## Acceptance criteria (future — not applicable until implementation authorized)

When implementation is authorized, acceptance tests SHALL demonstrate at minimum: handover/authorization separation; planning vs implementation separation under GEW where profile and authoritative policy require distinct authorizations (including Standard/High Assurance cases); bounded configuration does not weaken authorization law; GMFP work unit **not** subject to spurious GEW planning gate; profile ambiguity surfaces Attention without silent Standard/High Assurance selection; GIES pre-resolution from authoritative MVR/state without duplicate human observation where applicable; Interaction Hold does not widen DWA; interaction resolution does not imply next-unit authorization; hold distinct from STOP; manual-mode parity for one governance cycle; STOP semantics; no fabricated operator validation; HIW distinct from AWI; destination authority on inter-project handover (simulated or manual).

## Open questions

Remain OPEN per [AI Governance Workflow Integration Analysis](../../Architecture/AI_Governance_Workflow_Integration_Analysis.md): identity representation, cloud sync, permissions, events, unloaded targets, dependency addressing, provider enforcement.

**Workflow / GEW:** Enumerated per-profile minimum synchronization sets and evidence-class floors beyond GEW invariants; explicit project-level default profile policy encoding; DWA persistence fields ([GAP-027](../../Development/EDF_Gap_Register.md), [GAP-054](../../Development/EDF_Gap_Register.md)); Accelerated combined planning/implementation authorization (**open**).

**GIES / GEW (published consumption tranche 2026-10-03; runtime open):** GEW profile-specific GIES mediation, evidence floors, and synchronization; timeout/waiver defaults; multiple concurrent governed-operation interaction behavior; shared persistence with [PCON-0003](../../Architecture/PCON-0003-Governed-Pause-Continuation-and-Resume.md) pause; actor/role attribution ([PCON-0002](../../Architecture/PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md)); SPEC-006 interaction package shapes; GIES runtime ([GAP-055](../../Development/EDF_Gap_Register.md), [AWI-0011](../../Architecture/Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md)).

## Parent

- [Specifications](../README.md)

## Related Documents

- [PCON-0001](../../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [PCON-0005](../../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md), [ADR-0024](../../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)
- [AWI-0004](../../Architecture/Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md), [AWI-0010](../../Architecture/Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md), [AWI-0011](../../Architecture/Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md)
- [PCON-0006](../../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [ADR-0025](../../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md), [SPEC-005](SPEC-005-manual-verification-record-consumption.md)
- [SPEC-004 GIES consumption tranche plan](../../Handover/ProjectConcord-SPEC-004-GIES-Consumption-Reconciliation-Tranche-Plan.md)
- [AI Governance Workflow Integration Analysis](../../Architecture/AI_Governance_Workflow_Integration_Analysis.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
- [SPEC-006](SPEC-006-par-project-root-and-governed-workflow-relay.md), [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)
- [ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md), [AMD-0004](../../Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md)
