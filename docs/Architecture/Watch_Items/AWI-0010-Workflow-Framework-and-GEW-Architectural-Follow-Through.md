# AWI-0010 — Workflow Framework and GEW Architectural Follow-Through

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0010

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-10-03 |
| **Revisit Trigger** | Before M7a workflow runtime; GMFP consumption implementation; material SPEC-004 amendment after 2026-10-03 GEW publication |
| **Discovery source** | [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) (**Accepted** 2026-10-03), [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03) |
| **Gap** | [GAP-054](../../Development/EDF_Gap_Register.md#gap-054--workflow-framework-runtime-and-effective-configuration), [GAP-027](../../Development/EDF_Gap_Register.md#gap-027--developmentworkauthorization-schema-and-identity) |

---

## Objective

Track **implementation and normative follow-through** for the accepted-in-principle Workflow Framework and Governed Engineering Workflow (GEW) architecture — **without** treating this watch item as authorization to implement.

## Scope

While **Active**, this item:

- **Does** record open work: runtime registry, effective-configuration resolver, DWA schema extensions ([GAP-054](../../Development/EDF_Gap_Register.md), [GAP-027](../../Development/EDF_Gap_Register.md)), GMFP registry consumption.
- **Does not** authorize `src/` implementation, A3, or changes to A4/T7/MVR disposition.
- **Does not** supersede [AWI-0004](AWI-0004-Governed-Maintenance-Fast-Path.md) (GMFP consumption semantics remain EDF-owned).

## Promotion path

```text
AWI-0010 (this item)
    -> PA acceptance of PCON-0005 / ADR-0024 (publication)
    -> SPEC-004 GEW reconciliation (published 2026-10-03)
    -> implementation (M7a / separately governed)
```

## Related Documents

- [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md)
- [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)
- [SPEC-004 reconciliation tranche plan](../../Handover/ProjectConcord-SPEC-004-Workflow-Framework-Reconciliation-Tranche-Plan.md)
