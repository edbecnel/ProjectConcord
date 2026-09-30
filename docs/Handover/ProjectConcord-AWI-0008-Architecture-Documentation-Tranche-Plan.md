[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › AWI-0008 Architecture Documentation Tranche

# AWI-0008 — Architecture Documentation Tranche Plan

## Document Metadata

| Field | Value |
|---|---|
| **Tranche** | **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-10-01) |
| **Architectural result** | **ACCEPTED** |
| **Baseline** | `8f1e5de2be82eca2a931d8a99b83a965b61a302f` on `main` |
| **Publication** | Authorized by Project Architect — commit/push in publication tranche |

## Closeout disposition

| Item | Status |
|---|---|
| [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) | **Accepted** |
| SI policy | ProjectConcord **Preferred** (Super Intelligence / SI) within defined semantic scope for EDF `super-intelligence` |
| SI enforcement | **None / not authorized** |
| [AWI-0008](../Architecture/Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) | **Active** — partial architectural promotion (terminology + context-transfer); not Closed |
| [ADR-0014](../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) | **Proposed** — unchanged; not accepted in this tranche |
| [GAP-047](../Development/EDF_Gap_Register.md#gap-047--terminology-projection-and-optional-enforcement) | Open — projection/enforcement implementation deferred |
| `src/` implementation | **Not performed** |

## Validation

| Check | Result |
|---|---|
| `dotnet build -c Release` / `dotnet test -c Release` | **Not run** — environment lacks .NET SDK **10.0.401** required by `global.json` (only .NET 8 available); **does not block** this documentation-only tranche per PA disposition |
| Manual | No `src/` changes; no bulk AI→SI migration; historical ADRs not rewritten; EDF vs ProjectConcord ADR-0011 references qualified; documentation/index consistency reviewed |

## Scope delivered

| # | Deliverable | Record |
|---|---|---|
| 1 | Adopter terminology policy ADR | [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) **Accepted** |
| 2 | EDF terminology dependency pins | [EDF Terminology Governance handover](EDF-Terminology-Governance-Architecture-Handover.md) |
| 3 | ProjectConcord glossary policy rows | [Glossary](../Reference/Glossary.md) |
| 4 | AWI-0008 architecture update | Terminology disposition + governed context-transfer / packages |
| 5 | Overview and indexes | [System Architecture Overview](../Architecture/System_Architecture_Overview.md); [PROJECT_INDEX](../../PROJECT_INDEX.md); ADR index |
| 6 | Gap register | [GAP-047](../Development/EDF_Gap_Register.md) |

## Explicitly not delivered

- Terminology enforcement or living-doc SI migration
- Terminology projection, search, or UI implementation
- Work-package / review-package generation in `src/`
- AWI-0008 **Closed** (shell implementation items remain Active)
- A2 documentation drift repair; A3; A4
- ADR-0014 acceptance

## Parent

- [Handover](README.md)
