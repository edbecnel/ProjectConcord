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
| **Last Reviewed** | 2026-09-21 |
| **Governing decisions** | [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (Proposed), [ADR-0006](../../Architecture/ADRs/ADR-0006-AI-Boundary.md), [ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) |
| **Discovery source** | [PCON-0001](../../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) |

**Integration closeout (2026-09-21):** PCON-0001 handover integrated; documentation tranche Project Architect **accepted** at commit `b728e2896992b58ee785d406ac93a6badf29c8c8` per [AI Governance Workflow Integration Analysis](../../Architecture/AI_Governance_Workflow_Integration_Analysis.md). This specification remains **Draft** and **not implemented**. [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) remains **Proposed**. Implementation gated per [Implementation Roadmap](../../Development/Implementation_Roadmap.md) M7+.

## Problem

Engineering teams using an Architectural AI and a repository execution agent (for example GPT and Cursor) need **governed** coordination: explicit authorization, evidence, and human authority — without making chat transcripts or provider prompts canonical. Work may span **multiple projects** with destination-controlled acceptance and traceable cross-project dependencies.

## Goals

- Model canonical governance state separately from derived AI/provider transport.
- Separate handover context from DevelopmentWorkAuthorization.
- Support manual and integrated provider modes with identical semantics.
- Support multi-project workspace direction without implying current MVP delivery.
- Preserve cross-project provenance and destination governance authority.

## Non-Goals

- Replace Git, IDEs, or AI providers (PCON-0001 §45).
- Autonomous architectural acceptance or gate closure.
- Mandatory AAR for every implementation tranche ([ADR-0012](../../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md) scope unchanged).
- Upstreaming PC-AIGOV-022–028 to EDF in this tranche.
- MVP (M1–M5) delivery of governance UI or inter-project features.

## M1–M5 architectural constraint

When M1–M5 foundational code is implemented, it **MUST NOT** establish an implicit or irreversible assumption that one ProjectConcord runtime corresponds to exactly one project/repository. A practical migration path to multiple independently governed projects in one workspace **MUST** remain feasible.

Specific identity, API scoping, and persistence mechanisms are **not** prescribed by this spec; see [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md).

## Logical entities (when implemented)

| Entity | Default store | Purpose |
|---|---|---|
| DevelopmentWorkAuthorization | Operational | Capability-bounded permitted work per project |
| Handover package | Derived / operational snapshot | Inherited project context for agents |
| ArchitecturalReviewSubmission | Operational | Plan or implementation return for review |
| HumanInitiatedWorkItem | Operational | Inbox/triage; distinct from AWI |
| InterProjectHandover | Operational | Governed cross-project event |
| CrossProjectDependency | Operational | Ongoing cross-project relationship |
| Evidence / validation records | Operational | Linked to authorization and Git state |

Destination **EDF artifacts** remain canonical in the target project Git repository only through destination governance.

## Requirements (PC-AIGOV-001–028)

### Authority and canonical state

| ID | Requirement |
|---|---|
| **PC-AIGOV-001** | AI recommendations and execution SHALL NOT implicitly become project authorization or acceptance. |
| **PC-AIGOV-002** | Governance state SHALL be represented independently of AI chat transcripts and provider prompts. |
| **PC-AIGOV-003** | Inherited project context (handover) and permitted work (DevelopmentWorkAuthorization) SHALL be distinct concepts. |
| **PC-AIGOV-004** | Planning authorization SHALL NOT imply implementation authorization. |
| **PC-AIGOV-014** | Acceptance of one stage/tranche SHALL NOT implicitly authorize the next. |
| **PC-AIGOV-016** | Governed work SHALL retain provenance sufficient to reconstruct authorization, execution, validation, and acceptance. |

### Provider modes and execution

| ID | Requirement |
|---|---|
| **PC-AIGOV-005** | Provider integrations SHALL adapt to a provider-neutral governance protocol. |
| **PC-AIGOV-006** | Manual copy/paste workflow SHALL preserve the same governance semantics as direct integration. |
| **PC-AIGOV-007** | Repository agents SHALL be able to enter an explicit STOP state and request disposition without continuing unauthorized mutation. |
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

### Cross-project and workspace

| ID | Requirement |
|---|---|
| **PC-AIGOV-021** | Work routed to another project or framework SHALL preserve origin without becoming part of the originating project's active gate. |
| **PC-AIGOV-022** | Architecture SHALL support multiple independently governed projects within one ProjectConcord workspace (future implementation). |
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

## Acceptance criteria (future — not applicable until implementation authorized)

When implementation is authorized, acceptance tests SHALL demonstrate at minimum: handover/authorization separation; planning vs implementation separation; manual-mode parity for one governance cycle; STOP semantics; no fabricated operator validation; HIW distinct from AWI; destination authority on inter-project handover (simulated or manual).

## Open questions

Remain OPEN per [AI Governance Workflow Integration Analysis](../../Architecture/AI_Governance_Workflow_Integration_Analysis.md): identity representation, cloud sync, permissions, events, unloaded targets, dependency addressing, provider enforcement.

## Parent

- [Specifications](../README.md)

## Related Documents

- [PCON-0001](../../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [AI Governance Workflow Integration Analysis](../../Architecture/AI_Governance_Workflow_Integration_Analysis.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
