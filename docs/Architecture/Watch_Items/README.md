# Architectural Watch Items

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › Architectural Watch Items

## Purpose

This directory contains **Architectural Watch Items (AWIs)** — deferred architectural initiatives intentionally outside the current roadmap.

Watch items are **non-authoritative for implementation** while Active. They record open questions, longer-horizon evolution, or architectural uncertainty until promoted to ADRs or explicitly closed.

EDF convention: `docs/Architecture/Watch_Items/AWI-NNNN-Short-Title.md` ([EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-007).

## Watch Item Index

| ID | Initiative | Status |
|---|---|---|
| [AWI-0001](AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md) | Actor–Role abstraction, capability separation, engineering-domain profiles, and reconciliation with AI governance artifacts | Active |
| [AWI-0002](AWI-0002-Governed-Pause-Continuation-and-Resume.md) | First-class governed pause/continuation/resume capability (separate from AWI-0001) | Active |
| [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md) | Primary orchestration UI and external AI/engineering-tool integration (separate from AWI-0001/0002) | Active |
| [AWI-0004](AWI-0004-Governed-Maintenance-Fast-Path.md) | EDF Governed Maintenance Fast Path (GMFP) consumption, workflow-profile risk, and deferred implementation | Active |

## Lifecycle

| Status | Meaning |
|---|---|
| **Active** | Under observation; non-authoritative for implementation |
| **Promoted** | Elevated to one or more ADRs or implementation plans |
| **Closed** | Resolved or explicitly withdrawn |

Promotion path: AWI → Proposed ADR(s) → Accepted ADR(s). See EDF [Architectural Watch Items](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/Watch_Items/README.md).

## Parent

- [Architecture](../README.md)

## Related Documents

- [PCON-0002 — Actor–Role model and domain neutrality](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md)
- [PCON-0003 — Governed pause, continuation, and resume](../PCON-0003-Governed-Pause-Continuation-and-Resume.md)
- [PCON-0004 — Primary orchestration and external integration](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md)
- [EDF Governed Maintenance Fast Path — Architecture Handover](../../Handover/EDF-Governed-Maintenance-Fast-Path-Architecture-Handover.md)
- [Architecture README](../README.md)
- [Architecture Decision Records](../ADRs/README.md)
