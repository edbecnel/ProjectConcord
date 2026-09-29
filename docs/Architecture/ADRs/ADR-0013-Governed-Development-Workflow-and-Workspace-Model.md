# ADR-0013: Software Development Governed Workflow and Cross-Project Governance

> **Filename note:** Path `ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md` is retained for repository link stability. The document title was reconciled post–Stage 2 (2026-09-29); **“Workspace Model”** is removed from the title because *workspace* is architecturally ambiguous (see § Terminology).

## Status

Accepted

## Date

2026-09-21 (integrated 2026-09-21; reconciled 2026-09-29; **Accepted by Project Architect 2026-09-29** following Stage 1 and Stage 2 architecture publication and post–Stage 2 reconciliation review)

## Scope

This ADR records **Software Development / Engineering extension (B-layer)** governance decisions for AI-assisted repository execution workflow: canonical vs operational boundaries for **B** entities, naming invariants, intake vs EDF watch items, execution authorization, submissions and evidence packages, **Software Development inter-project governance**, and provider-neutral semantics consumption.

This ADR **does not** define, own, or supersede accepted **Core (A)** capabilities. Generic capabilities are defined in [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md), [AMD-0003](../AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md), and [AMD-0004](../AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md). Normative product requirements remain in [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md). Relay and Project Root binding remain in [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) and [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Proposed**).

**Multi-project application capability** (ProjectConcord hosting multiple **ProjectConcord Project IDs**) is a **Core/product-shell** concern, independent of Software Development enablement. This ADR governs **Software Development workflow operating across projects**, not generic multi-project application architecture.

## Context

[PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) generalizes a Snaptara-style workflow: canonical governance state is primary; AI chats and provider prompts are derived transport. Software Development adds **DevelopmentWorkAuthorization**, **InterProjectHandover**, **CrossProjectDependency**, and related **B-layer** operational entities distinct from EDF artifact lifecycle rules in [SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md).

[ADR-0006](ADR-0006-AI-Boundary.md) limits AI to proposals for canonical EDF writes. This ADR addresses **governed Software Development repository execution** without authorizing implementation.

Accepted Stage 1 and Stage 2 architecture assign **Project Work Record**, **Governed Interaction Relay**, generic project identity, and Working Environment boundaries elsewhere. This reconciliation promotes the A/B/D/E/F split into the normative Decision body below.

## Terminology (not synonyms)

| Term | Layer | Meaning |
|---|---|---|
| **Multi-project application context** | **Core / product shell (A)** | One ProjectConcord application instance may host/manage multiple **ProjectConcord Project IDs**. Not defined by this ADR. See [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md). |
| **Working Environment** | **D** | Composable capability/policy facets per [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §5 — **not** multi-project hosting. |
| **Project Root / session locator** | Core binding (**A**); detail **ADR-0015 Proposed**, [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) | Currently selected filesystem locator for session context. |
| **Git worktree / branch** | **B** execution/evidence context | Repository-scoped Software Development detail. |
| **Provider / agent session** | **E** | Adapter/session rendering (for example Cursor chat continuity). |
| **Persona / UI workspace** | UI / glossary (**GAP-036**) | UX layout facet; not governance architecture. |

## Decision

### 1. Canonical vs operational (Software Development governance)

- **EDF artifacts** in Git remain canonical engineering intent per [ADR-0002](ADR-0002-EDF-Canonical-Source-of-Truth.md) (**F**).
- The following **Software Development extension** entities default to **operational** persistence per [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), partitioned by **ProjectConcord Project ID** when implemented:
  - **DevelopmentWorkAuthorization**
  - **ArchitecturalReviewSubmission**
  - **HumanInitiatedWorkItem**
  - **InterProjectHandover**
  - **CrossProjectDependency**
  - Software Development **implementation evidence / validation records** (payload semantics **B**; optional correlation to **A** references on [Project Work Record](ADR-0017-Project-Work-Record-Core-Boundary.md))
- **Handover** content and provider message packages are **derived** operational snapshots ([ADR-0004](ADR-0004-Derived-Data-and-Cache.md)); generic relay envelope and validation are **Core Governed Interaction Relay (A)** per [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md); software-specific package/profile fields are **B**.

**Project Work Record (A)** is **not** part of the pre–Stage 2 undifferentiated “operational work entity” family. PWR is a thin Core coordination identity ([ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md)); it may persist in the operational store partition without adopting backlog, lifecycle, authorization, or methodology semantics.

### 2. Handover vs authorization

Inherited project context (**handover**) and **DevelopmentWorkAuthorization** (permitted Software Development execution) are **distinct** even when rendered in one UI message (PC-AIGOV-003). Handover MUST NOT imply execution authorization.

### 3. Planning vs implementation

Planning authorization does not imply implementation authorization (PC-AIGOV-004).

### 4. DevelopmentWorkAuthorization (B-layer execution authorization)

- **DevelopmentWorkAuthorization** is **Software Development / Engineering extension** execution authorization — **not** generic Core work identity, **not** generic Core lifecycle, **not** methodology state, **not** handover/inherited context.
- Use the name **DevelopmentWorkAuthorization** in normative docs to avoid collision with SPEC-003 **authorized state transitions** on artifacts.
- DWA **MAY** reference **Project Work Record** identifiers and generic Core authority/delegation records for correlation when those exist. The domain-neutral delegation primitive remains the working name *AuthorityGrant* (**A**, **no accepted schema**). This ADR does **not** define *AuthorityGrant* and does **not** require its schema for DWA semantics.
- Supervision, checkpoints, and escalation policy configured for authorization behavior belong to **Working Environment / project policy (D)**. Provider-rendered execution instructions belong to **provider adapters (E)**.

### 5. HumanInitiatedWorkItem vs AWI vs Project Work Record

- **HumanInitiatedWorkItem** (normative name in [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)) is **Software Development workflow intake/capture/triage (B)**, **distinct** from EDF Architectural Watch Items (**F** / EDF AWI).
- **HumanInitiatedWorkItem** is **not** the generic Core **Project Work Record** ([ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md)). **OperationalIntakeRecord** is preferred **working terminology only** — not an accepted canonical type; **generic intake Core-vs-extension ownership remains deferred**. This ADR does **not** decide future generic intake ownership.
- An intake record **may**: create a Project Work Record; associate with an existing Project Work Record; merge into existing work/intake; route toward another governed process; or be rejected/dispositioned **without** creating a Project Work Record.
- A HIW may promote to an EDF AWI or escalate to **InterProjectHandover**.

### 6. Submissions and evidence (B vs F)

- **ArchitecturalReviewSubmission** is a **B-layer** operational artifact for plan or implementation return-for-review. It is **not** generic Core work identity and does **not** define a generic Core lifecycle.
- Software Development **implementation evidence packages** and validation records are **B-layer** workflow artifacts, traceable to DWA and repository state (PC-AIGOV-008–013). **Project Work Record** may carry optional evidence **references (A)** without introducing a generic Core evidence payload ([ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md) §8).
- **EDF artifacts**, gates, **AAR**, **MVR**, and canonical architectural/governance **acceptance** remain **F-layer** ([ADR-0012](ADR-0012-Adopt-EDF-Architectural-Audit-Records.md), EDF specifications). Git branch/commit/PR mechanisms remain contribution/evidence, **not** architectural acceptance (PC-AIGOV-026).

### 7. Software Development inter-project governance (B)

When the Software Development extension is enabled, governed workflow **may** span multiple ProjectConcord projects:

- **InterProjectHandover** — operational **B** event; preserves source provenance; **destination project governance** controls disposition and acceptance (PC-AIGOV-025).
- **CrossProjectDependency** — operational **B** ongoing relationship; **must not** be generalized directly into the universal Core Project Work Record relationship model ([ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md) §12). Typed cross-project / PWR relationship taxonomy remains **deferred**.
- **InterProjectHandover** and **CrossProjectDependency** are **distinct** (PC-AIGOV-027).
- Traceability from source through target acceptance and back to dependent source work is required when implemented (PC-AIGOV-028).

Generic **multi-project application context** is prerequisite infrastructure owned by **Core (A)**, not by this ADR. PC-AIGOV-022–023 product requirements are specified in [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) and assume accepted Core identity/partitioning ([ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) **Proposed**).

### 8. M1–M5 non-lock-in (cross-reference)

Foundational M1–M5 code **MUST NOT** irreversibly assume one runtime ↔ one project/repository. That constraint protects future **multi-project application context (A)** and Software Development workflows alike. Mechanisms (`ProjectId`, scoping APIs, schema partitions) are **not** mandated here; see [Implementation Roadmap](../../Development/Implementation_Roadmap.md), [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), and [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Proposed**).

### 9. AAR vs submission review

[ADR-0012](ADR-0012-Adopt-EDF-Architectural-Audit-Records.md) AARs remain appropriate for **significant** architectural/milestone/gate conformance reviews (**F**). Routine implementation tranche review may use **ArchitecturalReviewSubmission** (**B**) without requiring an AAR each time.

### 10. EDF upstream

**Do not** propose PC-AIGOV-022–028 to EDF in this tranche. Record EDF candidates in [EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-035 for later review.

### 11. Provider neutrality (E)

Manual clipboard and integrated providers are **adapters (E)** to the same **B-layer** governance semantics ([SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)). Governance semantics MUST NOT depend on a specific provider product.

### 12. Core capabilities consumed (reference only)

This ADR **consumes** without redefining:

| Capability | Owner | Canonical source |
|---|---|---|
| Project Work Record | **A** | [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md), [AMD-0004](../AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md) |
| Governed Interaction Relay | **A** | [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §7, [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) |
| Project identity / multi-project application shell | **A** | [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Proposed**) |
| Working Environment | **D** | [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §5 |
| AuthorityGrant (conceptual) | **A** — no schema | [AMD-0003](../AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md) §10 |

### 13. Actor / Role dependencies (unresolved)

Normative **Actor**, **Role**, and **RoleAssignment** semantics remain **deferred** ([PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) **Proposed**). Until disposition:

- Relay packages MAY use **provisional transport attribution** (for example “Project Architect”, “Engineering Agent”) per [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §13.
- Provenance requirements (PC-AIGOV-016) **depend on** eventual actor/role modeling for full “who authorized/validated” semantics.
- This ADR does **not** introduce default Developer/QA roles into Core.

[PCR-0001](../../Development/PCR-0001-Project-Continuation-and-Pause-Record.md) Step 1 still recommends PCON-0002 disposition for broader AI-governance sequencing; **that sequencing is not waived**. **Project Architect disposition (2026-09-29):** unresolved [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) Actor/Role semantics **did not block** ADR-0013 acceptance; full actor-attributed provenance remains **deferred**.

## Alternatives Considered

### Store DevelopmentWorkAuthorization as canonical EDF artifacts

- Advantages: Git-auditable authorization text.
- Disadvantages: High churn; conflates execution workflow with engineering intent documents; awkward for inter-project operational state.
- Reason not selected: Operational default; export to EDF evidence optional later.

### Retain ADR-0013 as broad “workspace model” owner

- Advantages: Single document for early PAR-era narrative.
- Disadvantages: Conflicts with accepted Stage 1/2 decomposition; conflates Working Environment, multi-project shell, and Git/provider sessions.
- Reason not selected: Post–Stage 2 PA reconciliation (2026-09-29).

### Merge HIW with AWI

- Advantages: Fewer entity types.
- Disadvantages: Contaminates EDF watch-item semantics with inbox/triage noise.
- Reason not selected: Architect disposition — remain distinct.

## Consequences

### Positive

- Clear **B-layer** boundary for Software Development governance without redefining Core PWR, relay, or Working Environment.
- Inter-project Software Development principles preserved without generalizing IPH/CPD into Core relationship taxonomy.

### Negative

- Additional **B-layer** operational schema design (extends GAP-018 / GAP-027 families).
- Terminology discipline across six “workspace-adjacent” concepts (see Terminology table).

### Risks

- Premature implementation of inter-project or PR automation before M7 — mitigated by roadmap phasing and SPEC-004 “not implemented” status.

## Historical reconciliation notes (informational)

The following appendices record tranche integration history. **Normative placement** is in the Decision sections above.

### PAR integration (2026-09-28)

[ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Proposed**) and [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) introduce Project Root, stable Project ID, and relay validation without accepting ADR-0013. Historical **PAR** maps to Core relay + **B** software package + **D** policy + **E** adapters ([ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) §9).

### Stage 1 integration (2026-09-29)

[ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) **Accepted** — ADR-0016 does **not** accept ADR-0013.

### Stage 2 integration (2026-09-29)

[ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md) **Accepted** — resolves generic Work Item deferral; PWR is **A**, not **B**.

### Post–Stage 2 reconciliation tranche (2026-09-29)

Documentation-only reconciliation per [ADR-0013 Reconciliation Documentation Tranche Plan](../../Handover/ProjectConcord-ADR-0013-Reconciliation-Documentation-Tranche-Plan.md) — **CLOSED / PROJECT ARCHITECT ACCEPTED**.

### ADR-0013 acceptance (2026-09-29)

**Accepted** by Project Architect following reconciliation review and [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) consistency confirmation. **Acceptance does not authorize implementation**, PAR track **A2**, or resolution of deferred Actor/Role, intake, PWR schema, or **AuthorityGrant** semantics.

## References

- [PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [AI Governance Workflow Integration Analysis](../AI_Governance_Workflow_Integration_Analysis.md)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [ADR-0006](ADR-0006-AI-Boundary.md), [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [ADR-0012](ADR-0012-Adopt-EDF-Architectural-Audit-Records.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
- [ADR-0015](ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Proposed**), [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [AMD-0003](../AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)
- [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md), [AMD-0004](../AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md)
- [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) (**Proposed**)
- [PCR-0001](../../Development/PCR-0001-Project-Continuation-and-Pause-Record.md)

## Maintenance

**Accepted** 2026-09-29 (Project Architect). Reconciliation tranche closed per [ADR-0013 Reconciliation Documentation Tranche Plan](../../Handover/ProjectConcord-ADR-0013-Reconciliation-Documentation-Tranche-Plan.md). Update when SPEC-004, relay architecture, or Software Development governance semantics materially change. Acceptance does **not** authorize **A2** or M7+ implementation.
