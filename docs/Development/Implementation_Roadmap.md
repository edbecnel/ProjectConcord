[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Development](README.md) › Implementation Roadmap

# Implementation Roadmap

> **Status:** Draft  
> **Owner:** ProjectConcord  
> **Last Reviewed:** 2026-09-15

## Purpose

Incremental delivery plan for ProjectConcord aligned with PCON-0000 §51, [AMD-0001](../Architecture/AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md), and [SPEC-001](../Specifications/features/SPEC-001-mvp-edf-desktop-client.md). **No `src/` or Avalonia code until [EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) gate decision is satisfied.**

**Platform note:** Multi-user architecture is fixed at M0 ([ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)). M1–M5 may run as a local solo Administrator project; shared services and concurrent collaboration incrementally follow M6+.

## Gates

Authoritative approval: complete checkboxes in the EGR files (EDF [EGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md)).

| Gate | EGR record | Unblocks |
|---|---|---|
| **G0** | [EGR-G0 — Architecture Planning Gate](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) | M1 solution creation |
| **G1** | [EGR-G1 — MVP Implementation Gate](../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md) | M1 implementation in earnest |

## Milestones

| ID | Name | Scope | Exit criteria |
|---|---|---|---|
| **M0** | Architecture planning | Docs, ADRs, SPEC-001, bootstrap report, EGR-G0 | EGR-G0 satisfied |
| **M1** | Solution skeleton | `ProjectConcord.sln`, `Edf.Domain`, `Edf.Engine`, `Edf.Application`, `Edf.ProjectServices` (stubs), `Edf.Identity` (local degenerate), `Edf.Desktop`, tests; open-folder stub | Builds on macOS; domain types not single-user-only |
| **M2** | Discovery | Profile/capability resolution, artifact scan | Open ProjectConcord or EDF clone; list domains/artifacts |
| **M3** | Validation | Invoke EDF conformance scripts; display scores | Matches `run_conformance_validation.sh` output |
| **M4** | Navigation | PROJECT_INDEX, domain READMEs, link following | Semantic browse without tree-only UX |
| **M5** | Authoring | SPEC create/edit; validate; save; initial referential checks per [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) | SPEC-001 + move/rename impact preview (minimal) |
| **M6** | Referential integrity | Artifact Registry, Relationship Index, safe move/rename | SPEC-002 core scenarios |
| **M6+** | Shared project platform | Auth, membership, basic roles, shared operational store, repo access coordination, change-set / concurrency basics | Multiple desktop users on one project without canonical DB replacement |
| **M7+** | Reconciliation & AI | Git impact, Roslyn, AI proposals, Agile | Separate specs; out of MVP |

## Priority Order (from PCON-0000 §51)

1. EDF project bootstrap (M0–M1)  
2. Reusable EDF Engine (M2)  
3. Repository discovery & profile (M2)  
4. Validation (M3)  
5. Semantic model & navigation (M4)  
6. Canonical artifact CRUD (M5)  
7. Referential integrity ([SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)) (M6)  
8. Git, change impact, AI, Agile (M7+)

## Current Status

| Milestone | State |
|---|---|
| M0 | **Complete** — [EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) **Open** (re-review after multi-user amendments) |
| M1–M5 | Blocked on G0/G1 |
| M6–M7+ | Deferred |

## Gate G0 Review

**Authoritative record:** [EGR-G0 — Architecture Planning Gate](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) — complete **Reviewed**, **Approved**, ADR disposition, and **Gate satisfied** checkboxes there.

Summary deliverables are linked from the EGR document. Implementation remains blocked until EGR-G0 gate decision is satisfied.

## Assumptions

- Local EDF clone at user-configured path.
- M1–M5 solo use: creating user is default **Administrator** ([ADR-0010](../Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)).
- Bash available for script invocation on macOS/Linux; Windows strategy TBD at M1.

## Open Questions

- Minimum .NET SDK version for Avalonia target framework.
- Whether to gitignore `.projectconcord/` via template in M1.

## Parent

- [Development](README.md)

## Related Documents

- [tasks/README.md](../../tasks/README.md)
- [PCON-0000](../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)
