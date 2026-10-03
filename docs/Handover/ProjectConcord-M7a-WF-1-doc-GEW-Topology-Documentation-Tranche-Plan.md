[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › M7a-WF-1-doc GEW topology

# ProjectConcord — M7a-WF-1-doc GEW v1 Topology Documentation Tranche Plan

## Governance status

| Item | Status |
|---|---|
| **Tranche** | **M7a-WF-1-doc** — minimum GEW v1 topology and durable traversal semantics |
| **Handover UUID** | `c90e98a3-df9e-4f36-b81d-a5ecb4cba31c` (continues `91de09f5-b267-45cb-97fc-3cc12c70d81b`) |
| **Baseline** | `4ce63602ef6b0c9224464d8a2512abb8ad679a60` |
| **Architecture work** | **Authorized** — documentation only |
| **M7a-WF-1 runtime** (`src/`, Migration004, tests) | **Not authorized** |
| **Publication** | **Pending PA review** — uncommitted tranche preferred until PA publication authorization |

## Purpose

Make **GEW v1** definition topology, **workflow-instance traversal** (`TopologyPlaceId`, `TraversalOccurrenceId`), and **orthogonal** STOP / synchronization / AEI semantics **governed architecture** before workflow-instance persistence or recovery implementation ([GAP-054](../Development/EDF_Gap_Register.md)).

## Documents touched

- [PCON-0005](../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) §2.7 — definition vs traversal vs orthogonal vs projection
- [ADR-0024](../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §9–§13 — GEW v1 places/transitions, traversal occurrence, orthogonal state, multi-instance/origin/blocking principles, PC-PAR-025 compatibility
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) — recovery cross-reference (Draft)
- [EDF Gap Register](../Development/EDF_Gap_Register.md) — GAP-054 interim policy
- [AWI-0010](../Architecture/Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md) — progress note

## Explicitly out of scope

- `src/`, tests, Migration004, `workflow_instance` table
- PC-PAR-025 runtime, DWA, GIES runtime, workflow relationship tables, A3 UI
- Shared remote DB, synchronization, replication ([GAP-049](../Development/EDF_Gap_Register.md) remains deferred)
- Global or per-place synchronization/traversal ordinals
- Primitive-as-topology-place identifiers (rejected in prior PLAN)

## Expected sequence after this tranche

```text
M7a-WF-1-doc (this tranche)
    -> PA review
    -> publication authorization
    -> publication commit
    -> M7a-WF-1 implementation authorization
```

## Parent

- [Handover README](README.md)
