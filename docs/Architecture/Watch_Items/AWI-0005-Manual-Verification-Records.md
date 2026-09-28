# AWI-0005-Manual-Verification-Records

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0005

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-28 |
| **Revisit Trigger** | Before S1 M2 MVR parser work; before M5 attestation UI; EDF MVR baseline change |
| **Discovery source** | [MVR handover](../../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md) |
| **Related ADRs** | [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) (Proposed); [ADR-0006](../ADRs/ADR-0006-AI-Boundary.md) (Accepted) |
| **Related specs** | [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md) (Draft) |
| **Gap** | [GAP-042](../../Development/EDF_Gap_Register.md) |

---

# Manual Verification Records (MVR)

## Objective

Observe and track ProjectConcord’s obligation to **consume and project** EDF **Manual Verification Records (MVR)** — governed human manual verification procedures and execution records — without redefining EDF semantics, without premature implementation, and without treating MVR as a second gate system.

## Scope and non-goals

This watch item:

- **Does not** implement `MvrMarkdownParser`, pending-QA queries, attestation write-back, ATTENTION UI, or derived indexing.
- **Does not** create a separate MVR-adoption EGR gate.
- **Does not** auto-mutate EGR state from MVR state.
- **Does not** authorize TRV CC-4B migration.
- **Does not** create the ProjectConcord **execution MVR** for feature acceptance (STOP-2).

While **Active**, non-authoritative for implementation except where promoted specs/ADRs already normative ([SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md), [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)).

## Context

EDF published MVR at commit `192fe5c1c6254c51e257d24aefc09e127ce72464` ([MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md), [EDF ADR-0010](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Architecture/ADRs/ADR-0010-Manual-Verification-Records.md)). S0 reconciled ProjectConcord `docs/Verification/` and architecture documentation. Application work is phased M2–M5 per [Implementation Roadmap](../../Development/Implementation_Roadmap.md).

## Binding architectural observations

1. **Canonical Markdown** — MVR files in Git are authoritative; projections are rebuildable ([ADR-0004](../ADRs/ADR-0004-Derived-Data-and-Cache.md)).
2. **Human-Attestation Authority Boundary** — AI must not set MVT Pass/Fail/Blocked or Human execution status Complete without authorized human interaction ([ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)).
3. **Pending QA visibility** — Required manual QA must surface in ATTENTION, not only in planning documents ([SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)).
4. **Waiver separation** — Waivers on governing records; MVR preserves factual execution state.
5. **No optional MVT inference** — Obligation set is MVTs in canonical MVR unless EDF defines optional semantics deterministically.

## Investigation themes (deferred)

| Theme | Deferred question |
|---|---|
| Derived projection placement | General artifact index vs cache vs in-memory when Engine lands |
| Gate UI hints | Read-only blocking when EGR parse exists (S4) |
| QA Tester role | M6+ role gating vs M1–M5 Administrator + Executor |
| Framework Advisor | EDF-deferred MVR structural checks |

## Promotion path

When implementation begins: AWI themes close or promote into accepted ADR amendments, SPEC-005 revisions, and roadmap exit criteria — not ad hoc code paths.

## Parent

- [Watch Items](README.md)

## Related Documents

- [ProjectConcord MVR Adoption Architecture Plan](../../Handover/ProjectConcord-MVR-Adoption-Architecture-Plan.md)
