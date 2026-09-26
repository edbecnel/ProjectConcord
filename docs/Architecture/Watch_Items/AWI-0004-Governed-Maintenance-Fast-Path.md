# AWI-0004-Governed-Maintenance-Fast-Path

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0004

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-26 |
| **Revisit Trigger** | EDF GMFP consumed in ProjectConcord; future PCON discovery for workflow-profile abstraction; before normative ADR-0013 / SPEC-004 amendment |
| **Discovery source** | [GMFP handover](../../Handover/EDF-Governed-Maintenance-Fast-Path-Architecture-Handover.md) |
| **Related ADRs** | [ADR-0013](../ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (Proposed) — **do not amend in this tranche** |
| **Related specs** | [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (Draft / not implemented) |
| **Gap** | [GAP-041](../../Development/EDF_Gap_Register.md#gap-041--gmfp-gmr-consumption-and-workflow-profile-representation) |

---

# Governed Maintenance Fast Path (GMFP)

## Objective

Observe and track ProjectConcord’s obligation to **consume and project** EDF **Governed Maintenance Fast Path (GMFP)** — an optional governed capability for **bounded corrective maintenance** — without redefining EDF semantics, without premature implementation, and without hard-coding a single human-gate topology for all governed work.

## Scope and non-goals

This watch item:

- **Does not** implement GMR discovery, parsers, workflow engines, GMFP UI, eligibility validators, or DWA GMFP profiles.
- **Does not** hard-code workflow enums, GMR lifecycle states, or `enum WorkflowKind { Normal, GMFP }` (or equivalent).
- **Does not** amend [ADR-0013](../ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) or [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) normatively in this tranche.
- **Does not** extend or modify canonical **EDF GMR** semantics from ProjectConcord.
- **Does not** treat GMFP as satisfying EGR, GDO, AAR, ADR, or specification obligations by default.

While **Active**, non-authoritative for implementation.

## Context

EDF published GMFP as an optional governed capability at commit `b158f4a382dfbea941435eeacd96beb729443687` ([EDF ADR-0009](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Architecture/ADRs/ADR-0009-Governed-Maintenance-Fast-Path.md), [GMFP-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Specifications/GMFP-0001-Governed-Maintenance-Fast-Path.md)). ProjectConcord is a consumer/projection tool; EDF remains canonical.

The motivating problem: excessive governance synchronization for bounded corrective maintenance (for example test-isolation fixes with no production, schema, API, or architecture change). GMFP provides **two human governance gates** and an **authorized execution interval** between them.

## Topology ProjectConcord must eventually represent

**Three workflow stages; two human governance gates.**

```text
GMFP-1  AUTHORIZATION GATE (human)
            |
            v
GMFP-2  AUTHORIZED MAINTENANCE EXECUTION INTERVAL
        (NOT a human approval gate)
            |
            v
GMFP-3  ACCEPTANCE + PUBLICATION GATE (human)
            |
            v
        PUSH / VERIFY / COMPLETE
        (no routine post-push architecture-authority gate)
```

Do **not** conflate workflow stages with human approvals.

## Binding architectural observations

1. **Workflow profile principle** — ProjectConcord MUST NOT assume all EDF-governed work uses one fixed human-gate topology. Eventually distinguish at least conceptually: workflow classification/profile; workflow/instance state; human governance synchronization points; authorized execution intervals; canonical governance artifacts; operational execution authorization.

2. **Gate 1 semantics (Project Architect binding)** — GMFP Gate 1 is **one** bounded maintenance authorization (diagnose, classify, root cause/evidence, bounded scope, validation strategy, eligibility, authorize maintenance execution). It is **not** separate human planning authorization plus implementation authorization. GMFP-2 may include implementation, validation, evidence, maintenance documentation, and appropriate local commits without another human gate. [PC-AIGOV-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) must **not** be interpreted to reconstruct the intermediate approval gates GMFP intentionally removes; any future synthesis is deferred.

3. **Canonical vs operational** — **EDF GMR** is canonical authority in Git. ProjectConcord may eventually **project** authorization into **DevelopmentWorkAuthorization** (or successor operational model). Do **not** require DWA identifiers in canonical GMRs. Bidirectional identity, if needed later, requires explicit EDF/ProjectConcord architecture — not assumption now.

4. **Distinct from EGR / GDO / AAR** — Completed GMFP does **not** by default satisfy, close, or waive an EGR; satisfy a GDO obligation; close an AAR finding; or replace ADR/specification obligations. UI should represent these dimensions separately; relationships only when canonical artifacts explicitly establish them.

5. **Implementation risk** — [PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) diagrams a full Snaptara-style development loop. M7+ implementers must not encode that as the **only** workflow topology.

## Investigation themes (deferred)

| Theme | Deferred question |
|---|---|
| Workflow-profile abstraction | Generic model before GMFP-specific hard coding |
| GMR discovery | Parse/index `docs/Program/Maintenance_Records/GMR-*.md` |
| Integrity transitions | Governed fields per EDF when machine-readable rules exist |
| Operational projection | DWA/profile bound to GMR without embedding DWA IDs in GMR |
| Escalation | STOP GMFP → Escalated → normal EDF governance |
| Visualization | Two human gates + execution interval timeline |
| PC-AIGOV-004 synthesis | Reconcile full dev workflow vs GMFP profile without ad hoc SPEC-004 edits |

## Signals to watch

- M7 design treats every commit, doc edit, or validation run as requiring a separate architecture-authority gate.
- GMFP conflated with EGR satisfaction or GDO Non-Blocking disposition.
- Provisional GMR states or eligibility rules invented in ProjectConcord ahead of EDF.
- GMFP used as governance bypass when scope expands (missing Escalated / STOP).

## Promotion path

```text
AWI-0004 (this item)
    -> architectural discovery / PCON (future)
    -> normative ADR/SPEC amendment (deferred)
    -> implementation (separately governed)
```

Explicit Project Architect disposition required for promotion; not implied by this watch item.

## Open questions

Deferred to future PCON discovery — see [GMFP handover](../../Handover/EDF-Governed-Maintenance-Fast-Path-Architecture-Handover.md) §Decisions before implementation.

---

## Parent

- [Architectural Watch Items](README.md)

## Related Documents

- [EDF Governed Maintenance Fast Path — Architecture Handover](../../Handover/EDF-Governed-Maintenance-Fast-Path-Architecture-Handover.md)
- [EDF Gap Register](../../Development/EDF_Gap_Register.md) (GAP-041)
- [GDO handover](../../Handover/EDF-Governed-Dependency-Override-Architecture-Handover.md) (parallel consumption pattern)
- [PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [AI Governance Workflow Integration Analysis](../AI_Governance_Workflow_Integration_Analysis.md)
