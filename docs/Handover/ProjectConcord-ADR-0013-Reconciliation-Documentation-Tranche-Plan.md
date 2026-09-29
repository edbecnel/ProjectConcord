[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › ADR-0013 Reconciliation Documentation Tranche Plan

# ProjectConcord — ADR-0013 Reconciliation Documentation Tranche Plan

## Governance status

| Item | Status |
|---|---|
| **Stage 1** | **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** |
| **Stage 2** | **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** |
| **ADR-0013 reconciliation tranche** | **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-29) |
| **ADR-0013** | **ACCEPTED** — Project Architect — **2026-09-29** |
| **ADR-0015** | Remains **PROPOSED** |
| **PCON-0002** | **Proposed** / unresolved — **did not block** ADR-0013 acceptance (PA 2026-09-29) |
| **A2 implementation** | **NOT AUTHORIZED** |
| **Implementation (PWR, M7, etc.)** | **NOT AUTHORIZED** |
| **Scope** | Documentation only — **no `src/` changes** |

## Purpose

Reconcile and accept [ADR-0013](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) with accepted Stage 1 ([ADR-0016](../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [AMD-0003](../Architecture/AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)) and Stage 2 ([ADR-0017](../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md), [AMD-0004](../Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md)) architecture.

ADR-0013 is a **Software Development / Engineering extension (B-layer) governance ADR** that **consumes** generic Core capabilities rather than defining them.

## Authorized artifacts

| Artifact | Path | Status |
|---|---|---|
| ADR-0013 | [docs/Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) | Reconciled; **Accepted** 2026-09-29 |
| SPEC-004 | [docs/Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) | Consistency review **Accepted** |
| Consequential indexes | `ARCHITECTURE_DECISIONS.md`, [ADRs/README.md](../Architecture/ADRs/README.md), [PROJECT_INDEX.md](../../PROJECT_INDEX.md), [System Architecture Overview](../Architecture/System_Architecture_Overview.md), [docs/AI/README.md](../AI/README.md) | Updated for acceptance |
| This plan | This file | Closeout record |

## Baseline and publication

| Field | Value |
|---|---|
| **Pre-reconciliation HEAD** | `e9b54a32e538fb98f099c17052205aa04bee651f` |
| **Stage 2 publication** | `c968a88303c09d38e8f95e0c63f95c76662fc52f` |
| **Publication commit** | Recorded at tranche closeout on `main` (see git log after push) |

## Closeout record

- **Reconciliation:** Project Architect **accepted with qualifications** (2026-09-29)
- **ADR-0013 acceptance:** Project Architect **2026-09-29** (following Stage 1/2 publication and reconciliation review)
- **PCON-0002:** Unresolved; actor/role dependencies exposed in ADR-0013 §13; **not** an acceptance blocker per PA disposition
- **ADR-0015:** **Accepted** 2026-09-29 (separate closeout after this tranche)

## Maintenance (2026-09-29)

[ADR-0015](../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) **Accepted** by Project Architect (2026-09-29). The **Explicitly not authorized** list above remains an accurate record of that tranche.
- **A2 / implementation:** **Not authorized** by this tranche

## Binding dispositions (summary)

- **Multi-project application context** = Core (**A**); not owned by ADR-0013.
- **Software Development inter-project governance** (IPH, CPD, PC-AIGOV-021–028 principles) = **B**.
- **ProjectWorkRecord** = **A**; ADR-0013 does not own generic work identity.
- **DevelopmentWorkAuthorization** = **B** execution authorization.
- **HumanInitiatedWorkItem** = normative SPEC-004 name; generic intake ownership **deferred**.
- **Workspace** terminology disambiguated in ADR-0013; **“Workspace Model”** removed from ADR title.

## Explicitly not authorized (unchanged by acceptance)

ADR-0015 acceptance; AuthorityGrant schema; PWR lifecycle/relationship taxonomy; generic intake ownership; PCON-0002/0003/0004 resolution; A2/A3/A4 implementation; `src/` changes.

## Parent

- [Handover](README.md)
- [Stage 2 Documentation Tranche Plan](ProjectConcord-Stage-2-Documentation-Tranche-Plan.md)
