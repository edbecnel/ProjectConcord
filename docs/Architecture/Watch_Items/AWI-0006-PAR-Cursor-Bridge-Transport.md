# AWI-0006-PAR-Cursor-Bridge-Transport

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0006

| | |
|---|---|
| **Status** | Closed / Satisfied |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-28 |
| **Closed** | 2026-10-01 — transport architecture **Accepted** in [ADR-0022](../ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) |
| **Revisit Trigger** | Before **A4** implementation authorization (architecture prerequisite satisfied; **A4 not authorized**) |
| **Discovery source** | [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md); [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) |
| **Related ADRs** | [ADR-0015](../ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Accepted** 2026-09-29); [ADR-0021](../ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) (**Accepted** 2026-10-01); [ADR-0022](../ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) (**Accepted** 2026-10-01) |
| **Related specs** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) (**Accepted**; PC-PAR-022c–022d reconciled) |
| **Cross-reference** | [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md) — broader orchestration |

---

## Objective

Investigate **automated Engineering Agent transport** options after **P0 manual** governed relay is **published** (A2), without embedding provider transport in Core or the Software Engineering extension.

**Outcome (2026-10-01):** Provider-neutral automated transport architecture **Accepted** in [ADR-0022](../ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md). Reference **Cursor** realization: ACP client → Cursor CLI `agent acp` (non-normative); MCP scoped to tool/context integration, not primary relay transport. [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport) **closed / resolved**.

## Scope and non-goals

- Did **not** authorize extension, MCP, ACP, CLI, plugin loader, or provider SDK **implementation**.
- Did **not** merge into AWI-0003 (compatibility only).
- **Does not** authorize A4 implementation or an A4 implementation plan.

## PA disposition — transport architecture publication (2026-10-01)

- **AWI-0006:** **Closed / Satisfied** — [ADR-0022](../ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) **Accepted**.
- **A4 implementation** remains **not authorized**.

## Parent

- [Watch Items](README.md)

## Related Documents

- [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md)
- [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md)
- [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport)
