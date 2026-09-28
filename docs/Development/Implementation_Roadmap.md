[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Development](README.md) › Implementation Roadmap

# Implementation Roadmap

> **Status:** Draft  
> **Owner:** ProjectConcord  
> **Last Reviewed:** 2026-09-28

## Purpose

Incremental delivery plan for ProjectConcord aligned with PCON-0000 §51, [AMD-0001](../Architecture/AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md), and [SPEC-001](../Specifications/features/SPEC-001-mvp-edf-desktop-client.md). **[EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) satisfied** — M1 solution skeleton may begin **after S0 MVR architecture reconciliation**; full MVP implementation intensity remains gated on [EGR-G1](../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md).

**MVR adoption:** Architecture and `docs/Verification/` bootstrap complete in **S0** ([SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md)). MVR **application** work is **M2–M5** only — not M1. **No separate MVR-adoption gate**; use existing EGR-G1 / milestone sequence.

**Platform note:** Multi-user architecture is fixed at M0 ([ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)). M1–M5 may run as a local solo Administrator project; shared services and concurrent collaboration incrementally follow M6+.

## Gates

Authoritative approval: complete checkboxes in the EGR files (EDF [EGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md)).

| Gate | EGR record | Unblocks |
|---|---|---|
| **G0** | [EGR-G0 — Architecture Planning Gate](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) | **Satisfied** — M1 solution creation |
| **G1** | [EGR-G1 — MVP Implementation Gate](../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md) | Open — intensive MVP after **Complete** AAR-0001 and G1 satisfied |

### Deferred — GDO (EGR-0001 v1.1)

[EDF Governed Dependency Override](../Handover/EDF-Governed-Dependency-Override-Architecture-Handover.md) §12 maps to Concord milestones after gate instances exist: **M5+** EGR parse/display (GDO tables, Active overrides index); **M6+** dependency evaluation engine, validation rules, governance-debt UX ([GAP-040](EDF_Gap_Register.md), extends [GAP-006](EDF_Gap_Register.md)). EDF semantics are normative; Concord does not redefine GDO via AWI or post-closeout PCON.

### MVR consumption ([SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md), GAP-042)

| Stage | Milestone | Scope |
|---|---|---|
| **S0** | Architecture (pre-M1) | Handover, SPEC-005, ADR-0014, GAP-042, `docs/Verification/` bootstrap — **no** execution MVR instance |
| **S1** | M2 | MVR discovery, parser, identity validation |
| **S2** | M4 | MVR navigation / governing backlinks |
| **S3** | M5 | ATTENTION pending QA, human attestation write-back |
| **S4** | M5+ | Read-only gate/AAR/GMFP blocking hints — no EGR auto-mutation |

M1 skeleton: **no** MVR workflow code; **no** speculative empty MVR domain types unless a concrete M1 contract requires them.

## Milestones

| ID | Name | Scope | Exit criteria |
|---|---|---|---|
| **M0** | Architecture planning | Docs, ADRs, SPEC-001, bootstrap report, EGR-G0 | EGR-G0 satisfied |
| **M1** | Solution skeleton | `ProjectConcord.sln`, `Edf.Domain`, `Edf.Engine`, `Edf.Application`, `Edf.ProjectServices` (stubs), `Edf.Identity` (local degenerate), `Edf.Desktop`, tests; open-folder stub | Builds on macOS; architecture includes accepted MVR placement ([SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md)); **no** MVR implementation in M1; **Complete** [AAR-0001](../Architecture/Audits/) (M1 vs Accepted ADR-0001–0011 + SPEC-001 skeleton); [EGR-G1](../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md) for intensive MVP |
| **M2** | Discovery | Profile/capability resolution, artifact scan | Open ProjectConcord or EDF clone; list domains/artifacts |
| **M3** | Validation | Invoke EDF conformance scripts; display scores | Matches `run_conformance_validation.sh` output |
| **M4** | Navigation | PROJECT_INDEX, domain READMEs, link following | Semantic browse without tree-only UX |
| **M5** | Authoring + integrity hooks | SPEC create/edit; validate; save; identity/metadata validation; duplicate ID detection; basic external-change detection per [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) §60 | SPEC-001 + minimal referential preview |
| **M6** | Referential + integrity core | Artifact Registry, Relationship Index, safe move/rename ([SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)); lifecycle/governed-field validation, fingerprints, reconciliation-required states ([SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)) | SPEC-002 + SPEC-003 core deterministic scenarios |
| **M6+** | Shared project platform | Auth, membership, basic roles, shared operational store, repo access coordination, change-set / concurrency basics | Multiple desktop users on one project without canonical DB replacement |
| **M7+** | Reconciliation & AI governance | Git impact, Roslyn, EDF reconciliation; [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) governed dev workflow (manual mode first); Agile optional; CI/server integrity deferred from SPEC-003 §60 | **Not started** — see M7 phasing below |

## Priority Order (from PCON-0000 §51)

1. EDF project bootstrap (M0–M1)  
2. Reusable EDF Engine (M2)  
3. Repository discovery & profile (M2)  
4. Validation (M3)  
5. Semantic model & navigation (M4)  
6. Canonical artifact CRUD (M5)  
7. Deterministic integrity hooks ([SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)) (M5–M6)  
8. Referential integrity ([SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)) (M6)  
9. Git, change impact, AI governance ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)), Agile (M7+)

## M7+ phasing ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [PCON-0001](../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md))

| Sub-phase | Scope | Notes |
|---|---|---|
| **M7a** | Single-project manual governance | DevelopmentWorkAuthorization, handover packages, clipboard import/export, submissions |
| **M7b** | Structured submission & Git correlation | Scope conformance, drift, baseline defects |
| **M7c** | Provider adapters | Same semantics as manual mode |
| **M7d / M8** | Inter-project workspace | InterProjectHandover, CrossProjectDependency, traceability — **not MVP** |
| **M8+** | PR automation, permissions | Target branch/PR under authorization |

**M1–M5 constraint:** Foundational architecture must not irreversibly assume one runtime ↔ one repository ([ADR-0013](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) Proposed). Specific seams are not prescribed in M1–M5 docs.

## Current Status

| Milestone | State |
|---|---|
| M0 | **Complete** — [EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) **Satisfied** (2026-09-15) |
| **S0** | **Complete** (2026-09-28) — MVR architecture docs; see [MVR handover](../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md) |
| **Pre-M1** | **Complete** (2026-09-28) — C2/C4 doc reconciliation; [M1 / EGR-G1 plan](../Handover/ProjectConcord-M1-EGR-G1-Implementation-Plan.md) published |
| M1 | **Ready for M1a** — after PA authorizes M1a; skeleton per plan; **Complete** AAR-0001 then [EGR-G1](../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md) |
| M2–M5 | Blocked on G1 for implementation in earnest |
| M6–M7+ | Deferred |

## Gate G0 Review

**Decision:** **Satisfied** — recorded in [EGR-G0 — Architecture Planning Gate](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) (Gate Status **Satisfied**, all review items and ADR-0001–ADR-0011 accepted).

**M1:** May proceed with .NET solution skeleton (`src/`). **G1** remains the gate for intensive MVP implementation (SPEC-001 delivery).

## Assumptions

- Local EDF clone at user-configured path.
- M1–M5 solo use: creating user is default **Administrator** ([ADR-0010](../Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)).
- Bash available for script invocation on macOS/Linux; Windows strategy TBD at M1.

## Open Questions

- [x] Minimum supported .NET version for Avalonia LTS — **.NET 10 LTS** (`net10.0`); Avalonia **12** stable (see [Developer Handbook](../Developer_Handbook/01_Development_Environment.md))
- [ ] Ship embedded EDF script runner vs require user-configured EDF path only
- [ ] Whether to gitignore `.projectconcord/` via template in M1 — deferred until first milestone that produces derived/local ProjectConcord state ([M1 plan](../Handover/ProjectConcord-M1-EGR-G1-Implementation-Plan.md) PA-3)

## Parent

- [Development](README.md)

## Related Documents

- [tasks/README.md](../../tasks/README.md)
- [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)
- [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) (MVR consumption — M2–M5 implementation)
- [PCON-0000](../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)
- [PCON-0001](../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [AI Governance Integration Analysis](../Architecture/AI_Governance_Workflow_Integration_Analysis.md)
