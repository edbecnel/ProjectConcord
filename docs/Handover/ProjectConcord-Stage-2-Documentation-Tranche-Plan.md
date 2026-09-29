[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › Stage 2 Documentation Tranche Plan

# ProjectConcord — Stage 2 Documentation Tranche Plan

## Governance status

| Item | Status |
|---|---|
| **Stage 1** | **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** |
| **Stage 2 architecture** | **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** (2026-09-29) |
| **Stage 2 documentation** | **PUBLISHED** (2026-09-29) |
| **Publication commit** | `c968a88303c09d38e8f95e0c63f95c76662fc52f` |
| **ADR-0017** | **ACCEPTED** — Project Architect — 2026-09-29 |
| **AMD-0004** | **INTEGRATED / PUBLISHED** |
| **ADR-0013 / ADR-0015** | Remain **PROPOSED** |
| **Stage 2 implementation** | **NOT AUTHORIZED** |
| **A2 implementation** | **NOT AUTHORIZED** |
| **Scope** | Documentation only — **no `src/` changes** |

## Purpose

Canonicalize Stage 2 **Project Work Record** architecture (thin Core coordination identity) per PA dispositions and **Accepted ADR-0017**.

## Authorized artifacts

| Artifact | Path | Status |
|---|---|---|
| AMD-0004 | [docs/Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md](../Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md) | Integrated / published 2026-09-29 |
| ADR-0017 | [docs/Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md](../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) | **Accepted** 2026-09-29 |
| This plan | [docs/Handover/ProjectConcord-Stage-2-Documentation-Tranche-Plan.md](ProjectConcord-Stage-2-Documentation-Tranche-Plan.md) | Published |

## Closeout record

- **Project Architect acceptance:** 2026-09-29 (Stage 2 architecture + ADR-0017)
- **Publication date:** 2026-09-29
- **Primary publication commit:** `c968a88303c09d38e8f95e0c63f95c76662fc52f`
- Stage 2 capability architecture published via AMD-0004 + ADR-0017 and consequential doc amendments
- Implementation, A2, ADR-0013/0015 acceptance, and deferred semantics listed below remain **not authorized**

## Binding architecture (ADR-0017 Accepted)

Thin Core **`ProjectWorkRecord`** (working name only); essential semantics (identity, title, Project ID, provenance); optional coordination/reference capabilities; 0/1/n domain association with historical preservation; authority/evidence/methodology/EDF separations; durable PWR-to-PWR relationship **capability** (types deferred); effective-configuration provenance on material governed events; domain extension specialization; explicit exclusion of universal task/backlog PM semantics.

## Explicitly deferred (unchanged by publication)

Final type names; OperationalIntakeRecord canonical type; intake Core-vs-**B** ownership; AuthorityGrant/DWA schemas; Core lifecycle states; relationship taxonomy/schemas; priority/assignment/ranking ownership; persistence, APIs, UI; plugins; PCON-0002; generic Core evidence payload; ADR-0013/0015 acceptance; A2/A3/A4 implementation.

## Parent

- [Handover](README.md)
- [Stage 1 Documentation Tranche Plan](ProjectConcord-Stage-1-Documentation-Tranche-Plan.md)
