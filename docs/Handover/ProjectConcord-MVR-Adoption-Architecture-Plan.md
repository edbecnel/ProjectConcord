[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › ProjectConcord MVR Adoption Architecture Plan

# ProjectConcord MVR Adoption — Architecture Plan

## Governance status

| Item | Status |
|---|---|
| **STOP-1** | **CLOSED** — Project Architect accepted (2026-09-28) |
| **S0** | **Complete** (2026-09-28) — documentation and `docs/Verification/` bootstrap |
| **S1–S5 application work** | **Not authorized** |
| **STOP-2** (execution MVR instance) | **Binding** — deferred to S3 / PA-directed acceptance |
| **TRV CC-4B** | **Paused / untouched** |

## Purpose

Authoritative architecture plan for adopting EDF **Manual Verification Records (MVR)** in ProjectConcord. EDF baseline: `192fe5c1c6254c51e257d24aefc09e127ce72464`.

## Normative ProjectConcord artifacts (post-S0)

| Artifact | Role |
|---|---|
| [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) | Normative MVR consumption behavior |
| [ADR-0014](../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) | Human-Attestation Authority Boundary |
| [MVR handover](EDF-Manual-Verification-Record-Architecture-Handover.md) | EDF consumption guide |
| [GAP-042](../Development/EDF_Gap_Register.md) | Engine implementation gap |
| [AWI-0005](../Architecture/Watch_Items/AWI-0005-Manual-Verification-Records.md) | Deferred implementation watch |

## Accepted architectural decisions (summary)

- EDF Markdown canonical; derived projections rebuildable and non-authoritative.
- MVR candidate discovery by path; **canonical Record ID from Identity metadata only** — no silent filename inference.
- **No optional/required MVT inference** from prose; obligation = MVTs in canonical MVR.
- **Human execution status** is canonical; Validator/Evaluator recommend and block invalid **Complete**; waiver never manufactures Complete.
- **MVT Result** authoritative; checkbox **representational sync** on human-attested save only.
- **Human-Attestation Authority Boundary** — AI cannot independently set Pass/Fail/Blocked/Complete.
- **ATTENTION** minimum pending-QA UX (implementation M5).
- **EGR-G1 / milestones** — no separate MVR-adoption gate; no auto EGR mutation from MVR.
- **M1** — no speculative empty MVR types; no MVR workflow code.

## Implementation phasing

See [Implementation Roadmap](../Development/Implementation_Roadmap.md) — S1 (M2) parser through S3 (M5) attestation UX; S4 read-only gate hints.

## Working copy

Detailed plan (PA-amended): Cursor workspace plan `projectconcord_mvr_adoption_d106c47d.plan.md`. This repository document is the **persistent** acceptance record for S0.

## Parent

- [Handover](README.md)

## Related Documents

- [System Architecture Overview](../Architecture/System_Architecture_Overview.md)
