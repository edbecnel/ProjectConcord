[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › AWI-0008 A+C Documentation Tranche

# AWI-0008 — Operator Workflow + Workspace/Navigation (A+C) Documentation Tranche Plan

## Document Metadata

| Field | Value |
|---|---|
| **Tranche** | **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-10-01) |
| **Architectural result** | **ACCEPTED** — [ADR-0020](../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) **Accepted** 2026-10-01 |
| **Starting baseline** | `455bf7e30bc68110bc075bbb8d2de0ee3c9cbfaa` on `main` |
| **Authorization** | PA handover — AWI-0008 A+C canonical documentation tranche (2026-10-01) |
| **Publication** | PA-authorized closeout on `main` (2026-10-01); commit message begins *Canonicalize AWI-0008 operator and workspace architecture* |

## Tranche purpose

Canonicalize accepted **governed operator workflow**, **workspace/navigation**, and **product shell** architecture from the completed AWI-0008 A+C investigation — without UI, persistence, or structured-authoring implementation.

## Investigation and PA dispositions (summary)

- Operator projections (**Overview**, **Current Work**, **Attention**, **Next Action**) are **derived** — not new work entities or a hidden workflow engine.
- Multiple concurrent governed operations per Project; primary Current Work focus is **presentation only**.
- One Project-level **Attention** projection with contextual facets; not a competing lifecycle.
- **Required**, **Available**, and **Recommended** actions remain distinct.
- Workflow-local staged presentation over existing relay/package semantics ([SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17); raw transport via progressive disclosure only.
- Semantic/workflow IA — not filesystem-primary navigation.
- Capability-graded projections; no fabricated authoritative state.
- **Operational draft** — noncanonical; ephemeral or durable per interaction contract (**which contracts** remain deferred).
- **Single-Project Focus** preserved; **replaceable active Project context** is architectural accommodation only — not Project switching implementation.
- Inter-project dependency execution distinct from active-Project switching; target UX may open dependent Project in **another instance**.

## Closeout disposition

| Item | Status |
|---|---|
| [ADR-0020](../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) | **Accepted** 2026-10-01 |
| [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17 (PC-PAR-023) | Amended — operator-stage presentation binding |
| [AWI-0009](../Architecture/Watch_Items/AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) | **Active** — deferred, capture-only (included in publication); **not** architectural acceptance of structured authoring |
| [AWI-0008](../Architecture/Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) | **Active** — A+C architecture promoted; shell/`src/` implementation **not authorized** |
| [PCON-0000](../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) §37–38 | Reconciled with Single-Project Focus and ADR-0020 |
| [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) | Cross-reference — replaceable active Project context |
| `src/` implementation | **Not performed** — Project switching, durable draft, shell UI, Form Runtime **not authorized** |

## Scope delivered (documentation only)

| # | Deliverable |
|---|---|
| 1 | [ADR-0020](../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) (**Accepted**) |
| 2 | [AWI-0009](../Architecture/Watch_Items/AWI-0009-Governed-Structured-Authoring-Form-Runtime-and-Form-Editor.md) — capture + indexes |
| 3 | [AWI-0008](../Architecture/Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) — A+C promotion; structured authoring → AWI-0009 |
| 4 | [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17 |
| 5 | [PCON-0000](../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) §37–38 reconciliation |
| 6 | [System Architecture Overview](../Architecture/System_Architecture_Overview.md) |
| 7 | Indexes — [ARCHITECTURE_DECISIONS.md](../../ARCHITECTURE_DECISIONS.md), [ADRs README](../Architecture/ADRs/README.md), [PROJECT_INDEX.md](../../PROJECT_INDEX.md), [Watch Items README](../Architecture/Watch_Items/README.md) |

## AWI-0008 / AWI-0009 separation

| Concern | Owner |
|---|---|
| Product shell, navigation, operator projections, artifact host **Form \| Rendered \| Source** accommodation | **AWI-0008** + **ADR-0020** |
| Form definitions, Form Runtime, round-trip, external editing, Form Editor, AI/SI paths | **AWI-0009** (deferred) |

## Explicitly not delivered

- `src/` product shell, Project switching, operational draft persistence, Form Runtime, Form Editor
- ADR-0014 acceptance; GAP-048–052 resolution
- A3, A4, Engineering Agent plugins
- Selection of which interaction contracts require **durable** draft persistence

## Validation

| Check | Result |
|---|---|
| `dotnet build` / `dotnet test` | **Not run** — documentation-only tranche; SDK constraint unchanged |
| ADR-0020 status consistency | Index grep — **Accepted** throughout |
| No `src/` changes | Verified |
| Relay governance / package kinds | SPEC-006 §17 non-goals; no new kinds |

## Publication evidence

| Field | Value |
|---|---|
| **Message** | Canonicalize AWI-0008 operator and workspace architecture (ADR-0020). |

## Parent

- [Handover](README.md)

## Related Documents

- [AWI-0008 Persistence/Sync tranche plan](ProjectConcord-AWI-0008-Persistence-Sync-Documentation-Tranche-Plan.md)
- [AWI-0008](../Architecture/Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
