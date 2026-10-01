[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › System Architecture Overview

# System Architecture Overview

> **Status:** Draft — [EGR-G0](../../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) **Satisfied** (2026-09-15)  
> **Owner:** ProjectConcord  
> **Applies To:** EDF Project Management System  
> **Last Reviewed:** 2026-09-15 (multi-user amendments)  
> **Authoritative:** Yes

## Purpose

Define the technical architecture for ProjectConcord: a cross-platform desktop-first client that interprets, validates, navigates, and authors EDF-managed repositories without replacing EDF as canonical source of truth. Derived from [PCON-0000](PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md), [AMD-0001](AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md), [AMD-0002](AMD-0002-Single-User-Administrator-Model.md), and aligned with accepted/proposed ADRs.

**Current product principle (supersedes PCON-0000 single-user-first wording):** ProjectConcord is a **multi-user engineering project-management platform** whose first client is a native desktop application. Multiple authenticated users may participate in shared projects through shared project services and operational data; a future web client joins the same services ([ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)).

## Scope

### In scope

- Layered component model (desktop client, application services, EDF Engine, repository/Git)
- MVP vs deferred capabilities
- EDF version/profile interpretation strategy
- Validation, authoring, graph, reconciliation (design)
- Testing approach
- Desktop-to-web evolution constraints

### Out of scope

- Detailed UI mockups
- Mandatory CRA/CKES runtime services for MVP ([ADR-0008](ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md))
- Production deployment topology

---

## Architectural Context

```mermaid
flowchart TB
  subgraph clients [Clients]
    Desktop[Avalonia Desktop]
    FutureWeb[Future Web Client]
  end
  subgraph app [Application Layer]
    AppSvc[Application / Project Services]
    Identity[Identity and Authorization]
  end
  subgraph core [Core UI agnostic]
    Engine[EDF Engine]
    Authoring[Canonical Authoring]
    Validation[EDF Validation]
    Graph[Relationship Graph]
    RepoSvc[Repository Services]
    GitSvc[Git Services]
    ChangeImpact[Change Impact]
    Reconcile[Documentation Reconciliation]
  end
  subgraph data [Persistence]
    GitRepo[Git — Canonical EDF]
    OpStore[Operational Store — shared optional]
  end
  subgraph optional [Later]
    Agile[Agile Work Management]
    AI[AI Semantic Services]
  end
  Desktop --> AppSvc
  FutureWeb --> AppSvc
  AppSvc --> Identity
  AppSvc --> Engine
  AppSvc --> Authoring
  Engine --> RepoSvc
  Engine --> Validation
  Engine --> Graph
  RepoSvc --> GitRepo
  AppSvc --> OpStore
  GitSvc --> ChangeImpact
  ChangeImpact --> Reconcile
  AI -.-> Authoring
  Agile -.-> AppSvc
```

**Principle:** EDF repository artifacts remain canonical. The application provides interpretation, validation, navigation, authoring assistance, collaboration, and derived analysis ([ADR-0002](ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md), [ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)).

---

## Multi-User Platform Foundation

| Concern | Rule |
|---|---|
| Clients | Desktop first; web is another client of the same application services |
| Authorization | Project membership and roles enforced in application layer; EDF Engine stays auth-agnostic where practical |
| Solo projects | Creating user is default **Administrator** with full project permissions ([ADR-0010](ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)) |
| Canonical data | SPECs, ADRs, gates, and other EDF-prescribed artifacts remain in Git |
| Operational data | Membership, audit, notifications, change-set metadata, shared derived indexes MAY use an operational store |
| UI coupling | Clients MUST NOT talk directly to operational DB; use project services ([AMD-0001](AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md) §14) |
| Personas | UI workspaces/dashboards are not authorization roles; Administrators may use all perspectives without role spam |
| EDF gates | Independent review required by EDF/EGR is not bypassed by Administrator privilege |

**MVP phasing:** M1–M5 ([SPEC-001](../Specifications/features/SPEC-001-mvp-edf-desktop-client.md)) delivers local EDF discovery, validation, navigation, and authoring. Implementation MAY use a single-member project and local-only operational persistence, but assemblies and APIs MUST reflect the multi-user model (no forked “single-user mode”).

**Post-MVP:** Shared project services, membership, basic roles, shared operational database, controlled repository access, and concurrent-change detection — sequenced in [Implementation Roadmap](../Development/Implementation_Roadmap.md).

---

## Stage 1 capability model — Core, domains, working environment

**Status:** Documented 2026-09-29 ([AMD-0003](AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md) integrated; [ADR-0016](ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) Accepted). **Not implemented** in `src/`.

| Concern | Rule |
|---|---|
| Project identity | **ProjectConcord Project ID** represents the engineering undertaking — not a domain type ([ADR-0016](ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)) |
| Invariants | `Project != Domain != Extension != Methodology != Working Environment` |
| Multi-domain | Projects may enable multiple domains/extensions; enablement mutable without new Project ID |
| Domain lifecycle | **Available → Enabled → Activated** (working context); **Disabled** retains history |
| Activation | One or many enabled domains may be active in a working context; not required as default |
| Working Environment | Composable capability facets; built-in presets + custom compositions — separate from domain |
| Core | Domain-neutral shell, identity, lifecycle, operational partition, EDF consumption, **Governed Interaction Relay**, conceptual UI host |
| Software Development extension | Software execution authorization profile, software governance packages, repo handoff semantics ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)) |
| **Project Work Record** (`ProjectWorkRecord` — working name) | **Accepted** Core boundary ([ADR-0017](ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md)); capability model [AMD-0004](AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md); **not implemented** in `src/` |
| Operational intake vs PWR | **HumanInitiatedWorkItem** (SPEC-004 normative name) = intake/triage, not PWR; **OperationalIntakeRecord** = preferred working terminology only; intake ownership deferred |
| PWR relationships | Core durable relationship capability — typed semantics deferred |
| Physical extensions | Plugin loader, manifests, discovery — **deferred** |

---

## Project Root, relay, and PAR track (A0 / A2 architecture)

**Status:** Project Root + A1 identity/SQLite **implemented**; A2 P0 **Governed Interaction Relay** persistence, validation, and Desktop workflow **published** ([SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) **Accepted** 2026-09-29, [ADR-0015](ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) **Accepted** 2026-09-29). Cloud synchronization **not implemented**.

| Concern | Rule |
|---|---|
| **Single-Project Focus** | One **active** Project per application instance ([ADR-0019](ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) **Accepted** 2026-10-01) |
| **Concurrent Project Work** | Multiple instances MAY run; each owns one active Project; cross-project awareness ≠ multi-project workspace |
| Session context | User explicitly selects a **Project Root** (filesystem locator) for the active Project |
| Logical identity | Stable **ProjectConcord Project ID** — path is not durable identity ([ADR-0015](ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)) |
| Per-user state | Recent Project Roots, relay operational records — OS app data SQLite, partitioned by Project ID (A1/A2 subset implemented) |
| Project-local derived | `.projectconcord/` only when an authorized feature requires it — **not** on open/select |
| Governed Interaction Relay (Core) | Package/correlation, validation, provenance, relay-boundary STOP — A2 P0 subset **implemented** ([ADR-0016](ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)) |
| Inter-project continuity | Originating instance MAY preserve context while dependent Project work runs elsewhere ([ADR-0019](ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); IPH/CPD **B** — not fully implemented) |
| Historical **PAR** term | Umbrella for relay + software package + policy + adapters — see [AMD-0003](AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md) §9 |
| Project Architect | `IProjectArchitectProvider` boundary; manual ChatGPT product is one adapter (**E**) |
| Cursor | `CursorBridge` abstraction; P0 manual transport first ([AWI-0006](Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)) (**E**) |
| Tier 0 awareness | Shallow Git/governance metadata pre-M2 — not a second EDF parser |
| ADR-0013 / ADR-0015 | [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) **Accepted** 2026-09-29 (**B-layer** Software Development governance); [ADR-0015](ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) **Accepted** 2026-09-29 |

Phasing: PAR track A0–A4 in [Implementation Roadmap](../Development/Implementation_Roadmap.md) (A2 reframed — manual P0 governed interaction relay); separate from M2 EDF engine milestones.

---

## Stage 2 capability model — Project Work Record

**Status:** Documented 2026-09-29 ([AMD-0004](AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md) integrated; [ADR-0017](ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) **Accepted** 2026-09-29). Pre-publication closeout; **not published** until PA publication commit. **Not implemented** in `src/`.

| Concern | Rule |
|---|---|
| Core identity | **ProjectWorkRecord** (working name) — thin coordination primitive; **not** universal task/backlog ontology |
| Essential semantics | Stable id, declaration/title, Project ID scope, lightweight provenance |
| Domain association | 0, 1, or n domains; historical associations retained when domain disabled |
| Authority | Work record does **not** grant authority; **AuthorityGrant** / **DWA** remain separate |
| Lifecycle | No canonized Core states in Stage 2 — names/model **deferred** |
| Relationships | Durable PWR-to-PWR capability — relationship **types** deferred |
| Intake | **OperationalIntakeRecord** (working name) ≠ PWR; legacy **HumanInitiatedWorkItem** |
| A2 | **Not authorized**; no material dependency on PWR ontology |

---

## Local-first operational persistence and service boundary (ADR-0019)

**Status:** Architecture **Accepted** 2026-10-01 ([ADR-0019](ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)). Local SQLite **implemented** (A1/A2 subset); **synchronization and cloud Service API not implemented**.

| Concern | Rule |
|---|---|
| Service boundary | Application uses **semantic service contracts**; database technology isolated in `Edf.ProjectServices` ([GAP-051](../Development/EDF_Gap_Register.md#gap-051--application-layer-persistence--service-port-isolation)) |
| Local-first | Operational mutations durably local through service boundary **without** requiring cloud; local-only remains fully supported |
| Cloud (future) | Additive sync subsystem → **Cloud Service API** → cloud operational store; desktop **must not** connect to cloud DB directly |
| Sync modes | **Routine connected synchronization** (automatic while online) distinct from **reconnection reconciliation** after partition/offline |
| Concurrency (future) | No silent lost updates on synchronizable operational state; **optimistic versioning** default direction ([GAP-050](../Development/EDF_Gap_Register.md#gap-050--resource-level-operational-concurrency-policy-and-version-model)) |
| Canonical vs operational | Git/EDF canonical authority unchanged; operational sync ≠ Git sync |
| Multi-process local store | Shared per-user DB across processes — semantics **not yet governed** ([GAP-052](../Development/EDF_Gap_Register.md#gap-052--multi-process-local-operational-store-concurrency-strategy-and-validation)) |

---

## Governed operator experience and terminology (AWI-0008)

**Status:** Architecture documented 2026-10-01 ([AWI-0008](Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md), [ADR-0018](ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) **Accepted**, [ADR-0019](ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) **Accepted** 2026-10-01). Shell UX and sync **not fully implemented** in `src/`.

| Concern | Rule |
|---|---|
| Context-transfer minimization | Derive **work packages** and **review packages** from canonical Git state (**A**) + persisted operational relay state (**B**); concise handovers are **projections/transport**, not the sole continuity store |
| Instance-scoped packages | Packages default to the instance **active Project** unless explicitly inter-project transport |
| Governed intermediary | Project Architect ↔ ProjectConcord ↔ Engineering Agent; PC holds governed state, not decision authority |
| External provider state (**C**) | Not canonical; full chat transcripts must not substitute for **A**/**B** |
| Terminology layers | EDF **recommends** → ProjectConcord **prefers** ([ADR-0018](ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md)) → presentation (deferred) → enforcement (**not authorized**) |
| SI Preferred scope | EDF `super-intelligence` only; **not** `ai-assisted-engineering`; identifiers/history/external wording protected |
| EDF consumption | [EDF Terminology Governance handover](../Handover/EDF-Terminology-Governance-Architecture-Handover.md); qualify **EDF ADR-0011** vs **ProjectConcord ADR-0011** |
| MVR forms / attestation | [ADR-0014](ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) remains **Proposed** — direction for form runtime, not Accepted authority |

---

## Solution Structure

Illustrative .NET layout (assemblies may be merged if boundaries stay clear):

```text
ProjectConcord.sln
src/
    Edf.Domain/           # Semantic model, value types, artifact IDs
    Edf.Engine/           # Discovery, classification, profile resolution
    Edf.Validation/       # Script invocation + in-process rules
    Edf.Authoring/        # CRUD, templates, lifecycle guards
    Edf.Relationships/    # Link graph, inferred edges
    Edf.Repository/       # Filesystem abstraction
    Edf.Git/              # Git status, diff, blame (LibGit2Sharp or similar)
    Edf.ChangeAnalysis/   # Change summaries (Roslyn later)
    Edf.Integrity/          # Layered integrity, transitions, external change (M5+)
    Edf.Application/      # Use cases orchestrating Engine + UI-agnostic APIs
    Edf.ProjectServices/  # Membership, roles, change sets, operational persistence (M1: interfaces/stubs)
    Edf.Identity/         # Auth abstractions; local degenerate case for MVP
    Edf.Desktop/          # Avalonia shell, views, view models
tests/
    Edf.Engine.Tests/
    Edf.Validation.Tests/
    Edf.Integration.Tests/  # Fixture EDF repos
```

| Assembly | Responsibility |
|---|---|
| `Edf.Domain` | `Project`, `Artifact`, `Profile`, `ConformanceReport`, relationship types |
| `Edf.Engine` | Open project root, resolve capabilities, scan artifacts |
| `Edf.Validation` | Run EDF shell tools; parse output ([ADR-0003](ADRs/ADR-0003-EDF-Validation-Strategy.md)) |
| `Edf.Authoring` | Create/edit SPEC/ADR from templates; safe save paths |
| `Edf.Relationships` | Parse Markdown links → graph with confidence |
| `Edf.Repository` | IFileSystem, path guards, watch (optional) |
| `Edf.Git` | Detect changes; no full Git client |
| `Edf.Application` | Commands/queries for desktop and future API |
| `Edf.ProjectServices` | Project membership, roles, operational store, change-set coordination ([ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)) |
| `Edf.Identity` | Authentication/authorization abstractions; Engine remains policy-agnostic |
| `Edf.Desktop` | Avalonia only — no EDF rules here |

Core EDF logic MUST NOT depend on Avalonia (PCON-0000 §4, §54.8).

---

## Semantic Project Model

### MVP entities

| Entity | Source | MVP |
|---|---|---|
| Project | Root + adoption files | Yes |
| Project Profile / Capabilities | YAML | Yes |
| Artifact | Markdown/YAML under `docs/` | Yes |
| Architecture Decision (ADR) | `docs/Architecture/ADRs/` | Discover |
| Specification (SPEC) | `docs/Specifications/` | Discover + author (M5) |
| Watch Item (AWI) | `docs/Architecture/Watch_Items/` | Discover |
| Discovery Record | `docs/Architecture/` (e.g. PCON-0000) | Discover |
| Conformance Report | Framework Advisor output | Yes (M3) |
| Manual Verification Record (MVR) | `docs/Verification/Records/MVR-NNNN-*.md` | Discover (M2+); attestation UX (M5) per [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) |

### Deferred entities

Milestone, Gate, Sprint, Task — require EDF taxonomy or project extensions ([EDF Gap Register](../Development/EDF_Gap_Register.md) GAP-006, GAP-007). MVR **consumption implementation** tracked under GAP-042; architecture baseline in S0.

Engine exposes semantic APIs (PCON-0000 §7), e.g. `GetProjectProfile()`, `GetArtifacts()`, `GetConformanceSummary()`, and (when implemented) `GetPendingManualVerification()`, not raw directory walks in UI.

### Manual Verification Records (design)

Normative product behavior: [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md). EDF semantics: [MVR handover](../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md) at commit `192fe5c`.

| Concern | Rule |
|---|---|
| Canonical source | MVR Markdown in Git ([ADR-0002](ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)) |
| Identity | Record ID from Identity metadata; filename discovers **candidates** only — no silent identity inference |
| Derived state | Rebuildable projection per [ADR-0004](ADRs/ADR-0004-Derived-Data-and-Cache.md); implementation-neutral (no dedicated MVR-only index required by architecture) |
| Human execution status | Canonical field; validated for consistency; **Complete** only via human attestation ([ADR-0014](ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)) |
| Pending QA | ATTENTION dashboard when Manual QA required and execution unresolved |
| Gates | Read-only hints later; **no** automatic EGR mutation from MVR state |

---

## EDF Version and Profile Interpretation

1. **Project root** — user selects filesystem path ([ADR-0005](ADRs/ADR-0005-Repository-Abstraction.md)).
2. **Adoption config** — read `edf-adoption.yaml` (`profile`) and `edf-project-context.yaml` (`capabilities`, `legacy_profile`, `repository_role`).
3. **Framework reference** — user settings: path to local EDF clone; Engine invokes scripts from that path (GAP-002).
4. **Required directories** — resolve using same rules as EDF `edf_profile.sh` (ported or subprocess).
5. **Never hard-code** a single layout as “the” EDF (PCON-0000 §9).

---

## Validation

| Layer | Mechanism |
|---|---|
| Primary | Invoke `run_conformance_validation.sh` / `analyze_project_structure.sh` against project root with configured EDF path |
| Secondary | In-process checks where unambiguous (broken relative links, SPEC/ADR filename patterns) |
| Non-goal | Reimplement entire Framework Advisor in C# before M3 proves script wrapping |

Parse text output initially; contribute JSON format to EDF when stable (GAP-010).

---

## Conformance and review artifacts

| Artifact | Evaluates | Location |
|---|---|---|
| **EGR** | Human approval of authoritative **documents** | [docs/Program/Gate_Reviews/](../../Program/Gate_Reviews/) |
| **AAR** | **Implementation** vs Accepted ADRs / normative SPECs | [docs/Architecture/Audits/](Audits/README.md) ([ADR-0012](ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md), EDF [AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md)) |
| **Framework Advisor** | **Documentation** structure and navigation | `reports/conformance/` (transient) |
| **MVR** | **Human-executed** manual verification procedure and execution record | [docs/Verification/Records/](../../Verification/Records/README.md) ([SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md), GAP-042) |
| **Operational audit** | Collaboration / integrity **events** (not AAR) | Operational store per [ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) |

Post-M5, the Engine MAY discover EGR and AAR Markdown files for dashboard display (GAP-006, GAP-026). MVR pending-QA and attestation UX target M5 per SPEC-005; parser/discovery from M2.

### Governed Dependency Override (GDO)

EDF [EGR-0001 v1.1](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md) defines **Governed Dependency Override**: a source gate may remain **Open** while a named downstream activity is **Non-Blocking** only when an **Active** GDO is recorded on the source EGR. ProjectConcord MUST NOT collapse these into a single status field:

| Dimension | Role |
|---|---|
| **Gate Status** | Lifecycle of the gate obligation (Open, Satisfied, …) |
| **Dependency Disposition** | Blocking vs Non-Blocking for a **specific** downstream activity |
| **Override Status** | Lifecycle of the GDO row (Active, Reactivated, Closed) |

Normative semantics and consumption guidance: [GDO handover](../Handover/EDF-Governed-Dependency-Override-Architecture-Handover.md) ([GAP-040](../Development/EDF_Gap_Register.md)). **GDO** governs **program gate prerequisite blocking**; it is distinct from AI-governance **handover** packages and **DevelopmentWorkAuthorization** ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)). When citing EDF gate decisions, use **EDF ADR-0007** — not ProjectConcord [ADR-0007](ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md).

---

## Canonical Authoring and Lifecycle

- **Authoring** — structured forms driven by EDF templates; emit canonical Markdown to prescribed paths (M5: one artifact type, SPEC).
- **Integrity-aware authoring** — governed field changes SHOULD use semantic operations where [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) applies ([ADR-0011](ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md)).
- **Raw Markdown** — supported for power users; validation on save (PCON-0000 §15).
- **Lifecycle** — status values from templates; warn on unknown; no silent deletion — archive pattern per DIA.
- **Move/rename** — user-initiated; scan inbound/outbound links; update indexes when user confirms.

---

## Canonical Integrity Pipeline

Normative behavior: [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md). Decision: [ADR-0011](ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md).

```text
StructuralIntegrity
        |
        v
SemanticStructuralIntegrity
        |
        v
RelationshipAndLifecycleIntegrity  <-- SPEC-002 registry/index feeds this layer
        |
        v
AuthorizationIntegrity             <-- roles / EGR independent review
        |
        v
EngineeringIntentConsistency       <-- M7+ reconciliation (separate concern)
```

| Service (illustrative) | Role |
|---|---|
| `IntegrityService` | Orchestrate layered checks; artifact integrity status |
| `TransitionEvaluator` | Governed lifecycle operations vs raw text edits |
| `ExternalChangeMonitor` | Git/filesystem baseline; classify external deltas |
| `TrustedIntegrityStore` | Operational fingerprints and trust records ([ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)) |

**Multi-user:** Shared project services validate concurrent changes against the same integrity model before treating operational state as trusted ([SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) §38–§39). Change-set coordination aligns with [AMD-0001](AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md).

## Relationship Graph

Normative behavior: [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md). Decision: [ADR-0007](ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md). Lifecycle authorization and trusted state: [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md).

Derived from:

- Stable artifact IDs (ADR, SPEC, AWI, EGR, …) resolved to current paths via Artifact Registry
- Relative Markdown links and domain README indexes
- Optional metadata tables (Status, Spec ID)

Edges tagged **explicit** (link), **inferred** (index listing), or **semantic** (typed relationship when EDF or user confirms). Used for navigation, move/rename impact analysis, and reconciliation hints — not automatic lifecycle changes.

Stored in derived cache ([ADR-0004](ADRs/ADR-0004-Derived-Data-and-Cache.md)).

---

## Git Integration and Reconciliation

### Design (full vision)

Triad (PCON-0000 §25):

```text
EDF Intent  ←→  Code Changes  ←→  Validation Evidence
```

1. **Git Services** — detect changed files since baseline (commit or session).
2. **Change Impact** — map code paths to specs/ADRs via conventions + graph (later Roslyn summaries).
3. **Reconciliation** — propose doc updates; **human approval required** before canonical writes (PCON-0000 §29).
4. Code does **not** override accepted architecture automatically.

### MVP slice

After M5: read-only “files changed” + link to related SPECs if path/heuristic match. Full reconciliation workflow M6+.

---

## AI Boundary

- AI generates **proposals** only ([ADR-0006](ADRs/ADR-0006-AI-Boundary.md)).
- Deterministic validation runs before save.
- User approves canonical writes.
- Economical AI: structured prompts with minimal context (PCON-0000 §20).

---

## Agile Boundary

Optional work management (PCON-0000 §32) is **not** canonical EDF unless a future capability defines it. Deferred past MVP.

---

## Desktop-to-Web Evolution

Multi-user operation precedes the web client ([ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)). Web v1 reuses application and project services; it does not introduce multi-user semantics for the first time.

| Component | Web v1 expectation |
|---|---|
| `Edf.Domain`, `Edf.Engine`, `Edf.Validation`, `Edf.Authoring`, `Edf.Relationships` | Reuse unchanged |
| `Edf.Repository` | Adapt backend (filesystem → server-side repo clone) |
| `Edf.Application` | Expose as ASP.NET Core API |
| `Edf.Desktop` | Replace with web UI |
| Authentication | Application/Identity layer; Engine stays auth-agnostic |

**Success criterion:** Web client does not require rewriting validation or EDF interpretation rules.

---

## CRA, CKES, and EDF Boundaries

Normative for ProjectConcord: [CRA Alignment and Responsibility Boundaries](CRA_Alignment_and_Responsibility_Boundaries.md).

```text
CRA  →  foundational canonical representation (consume, do not redefine)
EDF  →  engineering documentation semantics (canonical repo artifacts)
ProjectConcord  →  operational UI, Engine, derived indexes
```

- [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) implements referential integrity at the EDF application layer.
- [ADR-0008](ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md) — no mandatory CKES/CRA service for MVP.
- EDF framework reference: [CRA_CKES_EDF_Boundaries](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/CRA_CKES_EDF_Boundaries.md).

---

| Level | Focus |
|---|---|
| Unit | Profile resolution, artifact classification, parsers |
| Integration | Open fixture repo (minimal EDF tree + EDF clone path); compare validation output to golden script output |
| UI | Smoke tests on desktop (later) |

Fixture repos under `tests/fixtures/` (created at M1/M2).

---

## Security Notes (phased)

- **M1–M5 (SPEC-001):** Local desktop MVP MAY use simplified auth (single default Administrator, local operational cache). Network identity federation is out of scope.
- **Platform:** Authentication, project membership, and role checks apply at application/project-service boundary before canonical writes ([ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)).
- AI API keys in OS secure storage when AI milestone arrives.
- Script invocation: validate paths; no shell injection from repo content.

---

## Related Decisions

| ADR | Topic |
|---|---|
| [ADR-0001](ADRs/ADR-0001-Layered-Architecture-and-Avalonia-Client.md) | Layers and Avalonia |
| [ADR-0002](ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md) | Canonical vs derived |
| [ADR-0003](ADRs/ADR-0003-EDF-Validation-Strategy.md) | Validation |
| [ADR-0004](ADRs/ADR-0004-Derived-Data-and-Cache.md) | Caches |
| [ADR-0005](ADRs/ADR-0005-Repository-Abstraction.md) | Repository |
| [ADR-0006](ADRs/ADR-0006-AI-Boundary.md) | AI |
| [ADR-0007](ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md) | Referential integrity |
| [ADR-0008](ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md) | CRA/CKES dependency |
| [ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) | Multi-user platform |
| [ADR-0010](ADRs/ADR-0010-Single-User-Administrator-Default-Model.md) | Administrator default |
| [ADR-0011](ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md) | Canonical integrity and trusted state |
| [ADR-0012](ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md) | Adopt EDF AAR |

---

## Parent

- [Architecture](README.md)

## Related Documents

- [AMD-0001 — Multi-User Amendment](AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md)
- [Multi-User Amendment Analysis](Multi_User_Amendment_Affected_Document_Analysis.md)
- [SPEC-001 MVP](../Specifications/features/SPEC-001-mvp-edf-desktop-client.md)
- [SPEC-003 Integrity](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)
- [EDF Gap Register](../Development/EDF_Gap_Register.md)
- [Implementation Roadmap](../Development/Implementation_Roadmap.md)
