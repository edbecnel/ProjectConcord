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
| [AWI-0005](AWI-0005-Manual-Verification-Records.md) | EDF Manual Verification Records (MVR) consumption, attestation boundary, and deferred implementation | Active |
| [AWI-0006](AWI-0006-PAR-Cursor-Bridge-Transport.md) | Engineering Agent automated transport — **Closed / Satisfied** 2026-10-01 ([ADR-0022](../ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) **Accepted**) |
| [AWI-0007](AWI-0007-SQLite-Transitive-NuGet-Advisory.md) | `Microsoft.Data.Sqlite` transitive `SQLitePCLRaw.lib.e_sqlite3` NU1903 / GHSA-2m69-gcr7-jv3q (A1b) | Active |
| [AWI-0008](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) | Governed operator shell; A+C → ADR-0020 **Accepted**; terminology → ADR-0018; structured authoring → AWI-0009; `src/` shell deferred | Active (partial promotion 2026-10-01) |
| [AWI-0009](AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) | Governed structured authoring, Form Runtime, form definitions, Form Editor (future) | Active — **deferred**, **capture-only** (2026-10-01) |
| [AWI-0010](AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md) | Workflow Framework and GEW architectural follow-through (runtime, SPEC-004 reconciliation) | Active |

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
- [EDF Manual Verification Records — Architecture Handover](../../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md)
- [Architecture README](../README.md)
- [Architecture Decision Records](../ADRs/README.md)
