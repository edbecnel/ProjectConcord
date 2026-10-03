[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › Work-state recovery binding

# ProjectConcord — Durable Governed Work-State Recovery Binding Tranche Plan

## Governance status

| Item | Status |
|---|---|
| **Architecture sufficiency investigation** | **Accepted** (2026-10-03) — conclusion **B — SMALL RECONCILIATION REQUIRED** |
| **This binding tranche** | **Published** (2026-10-03) — ADR-0020 §15, PCON-0005 FW-8 / §9.1, SPEC-006 §19 / PC-PAR-025; tranche **closed** |
| **Implementation** | **Not authorized** — [GAP-027](../Development/EDF_Gap_Register.md), [GAP-043](../Development/EDF_Gap_Register.md), [GAP-054](../Development/EDF_Gap_Register.md), [GAP-055](../Development/EDF_Gap_Register.md) |

## Purpose

Close cross-layer semantic bindings so ProjectConcord can recover governed work state and operator projections from **authoritative persisted operational state** and **canonical evidence** — without PA/EA chat history — and **fail closed** on stale/superseded relay imports.

## Documents touched

- [ADR-0020](../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15 — recovery invariant and minimum authoritative inputs
- [PCON-0005](../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) **FW-8**, §9 — durable instance/sync position; operation baseline vs repository HEAD
- [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §19, PC-PAR-013/014 — **Valid** vs **actionable** applicability; non-regression

## Out of scope

- [PCON-0003](../Architecture/PCON-0003-Governed-Pause-Continuation-and-Resume.md) promotion; PC-AIGOV-052–057
- GIES wire schema; SPEC-006 §18 reopening; `src/` / tests/
- New GAP; new governed-operation identity primitive

## Parent

- [Handover README](README.md)
