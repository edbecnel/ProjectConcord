# AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0001

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-21 |
| **Revisit Trigger** | Future Project Architect review of AI governance / workspace architecture; after disposition of [ADR-0013](../ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) and [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) relative to [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) |
| **Discovery source** | [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) (post-closeout; tranche closed at `2dfdc97c84c5c464ce7fe7263bbe400e6ba3dcdc`) |
| **Related ADRs** | [ADR-0013](../ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (Proposed) |
| **Related specs** | [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (Draft / not implemented) |
| **Related discovery** | [PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) (Proposed) |

---

# Actor–Role Abstraction and Engineering Domain Profiles

## Objective

Investigate whether ProjectConcord governance, workspace, and evidence models should be expressed through **Role**, **Actor**, and **RoleAssignment** abstractions (with explicit **authority/capability** separation), supported by **engineering-domain profiles** and Core semantics that remain **discipline-neutral** beyond the initial software-engineering reference implementation.

## Scope and non-goals

This watch item:

- Does **not** authorize implementation of role management, domain profiles, multi-project features, provider integration, or Git/PR automation.
- Does **not** change [ADR-0013](../ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) from **Proposed** or [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) from **Draft / not implemented**.
- Does **not** add PC-AIGOV-029–051 to SPEC-004 as normative requirements (candidates recorded in PCON-0002 §8 and [GAP-037](../../Development/EDF_Gap_Register.md)).
- Does **not** rename or redesign existing concepts (for example DevelopmentWorkAuthorization, Repository Execution Agent) — only flags them for later review.

While **Active**, this document is **non-authoritative for implementation**.

## Context

[PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) and the integrated AI-governance documentation tranche ([integration analysis](../AI_Governance_Workflow_Integration_Analysis.md), commits `b728e2896992b58ee785d406ac93a6badf29c8c8` / closeout `2dfdc97c84c5c464ce7fe7263bbe400e6ba3dcdc`) describe governance partly through concrete participants (architect AI, repository agent, human authority).

Immediately after closeout, [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) records a broader model: roles as logical responsibilities; actors as assignable entities; explicit role assignments; separation of duties; distinct implementation / automated test / QA / review / acceptance responsibilities; separate software vs end-user documentation roles; and a candidate Core + **Engineering Domain Profile** layering for multidisciplinary and non-software disciplines.

## Investigation themes

| Theme | Question for future architecture |
|---|---|
| Actor / Role / RoleAssignment | Canonical representation, lifecycle, and UI surfacing |
| Capability vs role membership | How privileged operations (acceptance, gates, merge, tranche authorization) bind to capabilities |
| Separation of duty | Policy expression when one actor holds multiple roles |
| Verification lanes | Preserve distinctions; automated test pass ≠ QA / architectural / gate acceptance |
| Evidence attribution | Actor + Role on attestations and validation evidence |
| Documentation roles | Software Documentation Engineer vs End-User Documentation Specialist vs implementer/architect |
| Engineering-domain neutrality | What belongs in Core vs Software Engineering Profile vs other profiles |
| Multidisciplinary projects | Multiple profiles per project without forced single-discipline modeling |
| Software/Git/repository terminology | Review DevelopmentWorkAuthorization, Git-centric evidence, repository execution agent naming for Core vs profile specialization |
| Reconciliation | Align or supersede participant-centric narrative in PCON-0001, ADR-0013, SPEC-004 without silent retroactive amendment of closed tranche |

## Signals to watch

- Implementation planning for M7+ governance assumes fixed participant types (ChatGPT, Cursor) rather than roles.
- SPEC-004 or operational schemas conflate automated test success with acceptance authority.
- New disciplines or artifact systems cannot map to governance without Core changes.
- Role taxonomy work ([GAP-019](../../Development/EDF_Gap_Register.md)) diverges from AI governance models.

## Promotion criteria

Elevation to Proposed ADR(s), SPEC amendments, or implementation planning requires explicit Project Architect disposition and must not be inferred from this watch item alone.

## Open questions (deferred)

All resolution deferred — see PCON-0002 §§1–7 and candidate requirements PC-AIGOV-029–051.

---

## Parent

- [Architectural Watch Items](README.md)

## Related Documents

- [PCON-0002](../PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md)
- [PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [AI Governance Workflow Integration Analysis](../AI_Governance_Workflow_Integration_Analysis.md)
- [ADR-0013](../ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [EDF Gap Register](../../Development/EDF_Gap_Register.md) (GAP-019, GAP-037)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
