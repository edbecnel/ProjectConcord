# AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0003

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-22 |
| **Revisit Trigger** | Future Project Architect review of primary orchestration and external AI/engineering-tool integration; after relevant disposition per [PCR-0001](../../Development/PCR-0001-Project-Continuation-and-Pause-Record.md) |
| **Discovery source** | [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md) |
| **Related ADRs** | [ADR-0013](../ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (Proposed) |
| **Related specs** | [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (Draft / not implemented) |
| **Cross-reference only** | [AWI-0001](AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md) (Active); [AWI-0002](AWI-0002-Governed-Pause-Continuation-and-Resume.md) (Active) — compatibility; **separate** initiatives |

---

# Primary Orchestration and External AI/Engineering Tool Integration

## Objective

Investigate whether ProjectConcord should operate as the **primary governance and orchestration user interface**, coordinating governed work with external Project Architect providers and engineering agents (Cursor, GitHub Copilot, future tools) through provider-neutral adapters—while retaining structured **manual handover** as a permanent supported mode.

## Scope and non-goals

This watch item:

- Does **not** merge into [AWI-0001](AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md) or [AWI-0002](AWI-0002-Governed-Pause-Continuation-and-Resume.md).
- Does **not** authorize implementation of orchestration transport, MCP servers, ACP clients, API integrations, or Cursor extensions.
- Does **not** add PC-AIGOV-060–071 to SPEC-004 as normative requirements ([GAP-039](../../Development/EDF_Gap_Register.md)).
- Does **not** change ADR-0013 or SPEC-004 disposition.
- Does **not** select ACP, MCP, CLI, extension APIs, or any provider as normative architecture.

While **Active**, non-authoritative for implementation.

## Context

[PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) and the integrated AI-governance documentation tranche describe workflows that today rely heavily on manual context transfer. [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md) records a candidate long-term direction: ProjectConcord orchestrates authorized tranches and receives structured results/evidence; specialized IDEs and agents remain workbenches; manual paste handover remains bootstrap, fallback, and recovery; transport (manual vs direct API/ACP/MCP/CLI/extension) should not redefine governance semantics. This depends on [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) Actor/Role separation and remains compatible with [PCON-0003](../PCON-0003-Governed-Pause-Continuation-and-Resume.md) continuation models.

## Investigation themes

| Theme | Deferred question |
|---|---|
| Primary orchestration UX | What governed actions originate in ProjectConcord vs external workbenches |
| Manual vs direct adapters | Same semantics across Manual Conversation and Direct Provider paths |
| Transport suitability | ACP, MCP, CLI, extensions—capabilities, security, licensing, lifecycle |
| Provider-neutral boundaries | EngineeringAgentAdapter / ProjectArchitectAdapter without provider lock-in |
| Structured exchange | Contracts for authorization, context, results, evidence, deviations |
| Governed provider surface | Whether ProjectConcord exposes operations to external agents (illustrative only in PCON-0004) |
| IDE extension role | Context display and evidence submission without making IDE the governance source of truth |
| Capability vs permission | Technical ability vs Work Authorization constraints |
| Non-Cursor agents | GitHub Copilot and future engineering agents alongside Cursor |
| Core vs profile | Software Engineering Profile vs Core for IDE/Git-specific integration |
| Security and audit | Credentials, identity, freshness, isolation, provenance, failure modes (queued in PCON-0004 §15) |

## Signals to watch

- Implementation assumes ChatGPT or Cursor as fixed governance participants rather than assigned Actors/Roles.
- A transport protocol (MCP/ACP/API) is treated as defining authorization or acceptance semantics.
- Manual handover path regresses when direct integration is added.
- Engineering-domain profiles cannot adopt non-IDE artifact systems without Core changes.

## Promotion criteria

Explicit Project Architect disposition required; promotion to ADR/SPEC/implementation not implied.

## Open questions

All deferred — see [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md).

---

## Parent

- [Architectural Watch Items](README.md)

## Related Documents

- [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md)
- [PCR-0001](../../Development/PCR-0001-Project-Continuation-and-Pause-Record.md)
- [PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md)
- [AI Governance Workflow Integration Analysis](../AI_Governance_Workflow_Integration_Analysis.md)
- [EDF Gap Register](../../Development/EDF_Gap_Register.md) (GAP-039)
