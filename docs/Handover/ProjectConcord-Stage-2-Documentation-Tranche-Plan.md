[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › Stage 2 Documentation Tranche Plan

# ProjectConcord — Stage 2 Documentation Tranche Plan

## Governance status

| Item | Status |
|---|---|
| **Canonical baseline (Stage 1 published)** | `ecab82b3d0dff68a053c0c0ece2ed863352cbc86` |
| **Stage 1** | **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** |
| **Stage 2 architecture analysis** | **PROJECT ARCHITECT ACCEPTED WITH MODIFICATIONS** |
| **Stage 2 documentation drafting** | **COMPLETE / PROJECT ARCHITECT REVIEWED** |
| **ADR-0017** | **ACCEPTED — PROJECT ARCHITECT — 2026-09-29** |
| **AMD-0004** | **Integrated** (normative summary: Accepted ADR-0017) |
| **Stage 2 documentation** | **PRE-PUBLICATION CLOSEOUT / VALIDATION** — **not PUBLISHED** until publication commit |
| **Stage 2 implementation** | **NOT AUTHORIZED** |
| **A2 implementation** | **NOT AUTHORIZED** |
| **ADR-0013 / ADR-0015** | Remain **PROPOSED** |
| **Scope** | Documentation only — **no `src/` changes** |
| **Commit / push** | **NOT performed** — awaiting PA **publication** authorization |

## Purpose

Canonicalize Stage 2 **Project Work Record** architecture (thin Core coordination identity) per PA dispositions and **Accepted ADR-0017**.

## Authorized artifacts

| Artifact | Path | Status |
|---|---|---|
| AMD-0004 | [docs/Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md](../Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md) | Integrated 2026-09-29 |
| ADR-0017 | [docs/Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md](../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) | **Accepted** 2026-09-29 |
| This plan | [docs/Handover/ProjectConcord-Stage-2-Documentation-Tranche-Plan.md](ProjectConcord-Stage-2-Documentation-Tranche-Plan.md) | Pre-publication closeout |

## Binding architecture (ADR-0017 Accepted)

Thin Core **`ProjectWorkRecord`** (working name only); essential semantics (identity, title, Project ID, provenance); optional coordination/reference capabilities; 0/1/n domain association with historical preservation; authority/evidence/methodology/EDF separations; durable PWR-to-PWR relationship **capability** (types deferred); effective-configuration provenance on material governed events; domain extension specialization; explicit exclusion of universal task/backlog PM semantics.

## Explicitly deferred (unchanged by ADR acceptance)

Final type names; OperationalIntakeRecord canonical type; intake Core-vs-**B** ownership; AuthorityGrant/DWA schemas; Core lifecycle states; relationship taxonomy/schemas; priority/assignment/ranking ownership; persistence, APIs, UI; plugins; PCON-0002; generic Core evidence payload; ADR-0013/0015 acceptance; A2/A3/A4 implementation.

## Validation

Pre-publication validation performed at closeout (internal links, ADR status consistency, scope checks). **DTP-7:** not present as an executable script in this repository; Stage 1 referenced DTP-7 at EDF/framework closeout — equivalent checklist executed manually for Stage 2 (see PA publication evidence package).

## Publication sequence (next step — not executed)

1. PA authorizes publication commit on `main`.
2. Record publication commit hash in this plan (**PUBLISHED**).
3. DTP-7 or framework validation at commit time if PA directs EDF tooling path.

## STOP

Do not commit, push, implement, or authorize A2 without separate PA direction.

## Parent

- [Handover](README.md)
- [Stage 1 Documentation Tranche Plan](ProjectConcord-Stage-1-Documentation-Tranche-Plan.md)
