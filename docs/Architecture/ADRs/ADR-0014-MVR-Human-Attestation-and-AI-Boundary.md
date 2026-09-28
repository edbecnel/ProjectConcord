# ADR-0014: MVR Human Attestation and AI Boundary

## Status

Proposed

## Date

2026-09-28

## Context

EDF published **Manual Verification Records (MVR)** at framework commit `192fe5c1c6254c51e257d24aefc09e127ce72464` ([MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md), [EDF ADR-0010](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Architecture/ADRs/ADR-0010-Manual-Verification-Records.md)). ProjectConcord must consume MVR semantics without redefining EDF governance ([MVR handover](../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md)).

[ADR-0006](ADR-0006-AI-Boundary.md) requires human approval for canonical writes and prohibits AI from self-authorizing governed lifecycle transitions. MVR adds **human-executed manual verification** outcomes that MUST NOT be inferred from implementation completion, automated tests, or inspection.

**Naming collision:** This ADR is **ProjectConcord ADR-0014**. **EDF ADR-0010** is Manual Verification Records — not [ADR-0010](ADR-0010-Single-User-Administrator-Default-Model.md) (Single-User Administrator).

## Decision

### 1. Human-Attestation Authority Boundary

ProjectConcord defines a **Human-Attestation Authority Boundary** between:

- operations that **prepare, explain, or propose** MVR content; and
- operations that **assert human manual verification outcomes** in canonical Markdown.

AI-originated or AI-automated paths MUST be **incapable of independently asserting** the attestation authority required for protected transitions (§2).

### 2. Protected transitions

The following canonical MVR state changes REQUIRE **authorized human interaction** with explicit **human-attestation context** (see [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md)):

| Transition | Canonical location |
|---|---|
| MVT **Result** = `Pass` | Execution record table |
| MVT **Result** = `Fail` | Execution record table |
| MVT **Result** = `Blocked` | Execution record table |
| **Human execution status** = `Complete` | Identity metadata |

Setting MVT **Result** to `Pending` via human edit MAY be permitted when correcting records; AI MUST NOT use `Pending` to bypass attestation rules.

### 3. AI permitted operations

AI assistants and automated services MAY:

- draft or maintain MVR structure and MVT procedures;
- explain tests and surface pending manual QA;
- propose observations and evidence references;
- navigate users to pending verification work;
- prepare proposed Markdown changes for human review;
- persist outcomes **explicitly supplied through authorized human interaction** (for example values entered and confirmed in a human attestation dialog).

### 4. AI prohibited operations

AI assistants and automated services MUST NOT:

- set protected transitions (§2) based solely on implementation completion, automated test success, code or document inspection, model inference, expected behavior, or other non-human evidence;
- independently attest that human manual verification occurred;
- set **Human execution status** to **Complete** because a governing record waived verification;
- treat checkbox state as sufficient evidence of **Pass** or **Complete**.

### 5. Canonical write path

Protected transitions MUST flow through a **human-attestation command** in the application layer that:

1. requires explicit human confirmation in the client;
2. records **Executor** identity (M1–M5: Administrator session + Executor name per [ADR-0010](ADR-0010-Single-User-Administrator-Default-Model.md));
3. performs surgical write-back to canonical MVR Markdown;
4. runs consistency validation ([HumanExecutionStatusValidator](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) — name per SPEC-005) before save.

General AI proposal pipelines ([ADR-0006](ADR-0006-AI-Boundary.md)) MUST NOT bypass this command for protected transitions.

### 6. Waiver separation

Governance **waivers** belong on governing records (for example EGR). They MUST NOT manufacture MVT **Pass** or **Human execution status Complete** in the MVR. MVR **Notes** MAY reference a waiver for traceability only.

### 7. Relationship to SPEC-003

Broader canonical integrity, authorized state transitions, external change detection, and reconciliation remain governed by [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) and [ADR-0011](ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md). This ADR adds MVR-specific attestation rules without duplicating SPEC-003.

## Consequences

### Positive

- Clear, enforceable boundary for QA trust and EDF MVR-0001 human authority rules.
- Aligns desktop client behavior with EDF anti-impersonation requirements.

### Negative

- Additional confirmation UX for MVT results and Complete status.

## References

- [SPEC-005 — MVR consumption](../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
- [MVR handover](../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md)
- [ADR-0006 — AI Boundary](ADR-0006-AI-Boundary.md)
- [ProjectConcord MVR Adoption Architecture Plan](../Handover/ProjectConcord-MVR-Adoption-Architecture-Plan.md)
