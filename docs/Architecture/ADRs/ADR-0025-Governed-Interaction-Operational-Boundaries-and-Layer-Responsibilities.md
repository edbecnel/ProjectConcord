# ADR-0025: Governed Interaction — Operational Boundaries and Layer Responsibilities

## Status

Accepted

## Date

2026-10-03 (Accepted by Project Architect — GIES publication)

## Context

[PCON-0006](../PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) defines the **Governed Interactive Engineering Session (GIES)** framework: interaction context, set/item model, hold, pre-resolution, routing, provenance, and failure dispositions.

A4 human QA and relay practice demonstrated that Engineering Agent operations may require **human observation**, **PA judgment**, or **authorization** mid-flight without abandoning the governed operation ([MVR-0003](../../Verification/Records/MVR-0003-a4-engineering-agent-automated-transport.md), [AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)).

[PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §13 deferred GIES design; that deferral is addressed by PCON-0006 — **without** changing Accepted Workflow Framework decisions.

[SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) owns relay validation, STOP, and transport; [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) owns derived operator projections.

This ADR records **layer allocation** only. It does **not** authorize implementation, relay package schemas, or persistence APIs.

## Decision

### 1. GIES is framework infrastructure (A), not a primitive

GIES **SHALL** be implemented as **workflow-instance interaction infrastructure** under the Workflow Framework ([PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md)), composing primitives and instance state — **not** as a fifth primitive or separate prescribed workflow.

### 2. Layer responsibility matrix

| Layer | Responsibility for GIES |
|---|---|
| **Workflow Framework (PCON-0005 + PCON-0006)** | Hold semantics; set/item model; pre-resolution (GIES-1); routing rules shape; provenance categories; framework invariants; failure disposition vocabulary |
| **Prescribed workflow / GEW (ADR-0024, future SPEC-004 hooks)** | Whether interaction permitted; respondent/authority rules; mediation requirements; evidence floors after resolution — **profile detail deferred** |
| **Software Development B (SPEC-004, ADR-0013)** | Authorization law (PC-AIGOV-004); STOP; DWA/Control; provenance (PC-AIGOV-016) — **constraints** on GIES under GEW |
| **Core relay A (SPEC-006)** | **Carries** interaction-related payloads when automated; structural validation; INCOMPLETE; STOP hooks; render/parse — **does not own** interaction governance semantics |
| **Provider E (ADR-0021, ADR-0022)** | Optional native Q&A adapters; transport recovery; **transport-operation** records distinct from governance provenance |
| **Operator projection (ADR-0020)** | Derived Awaiting-* states; Attention facets; Next Action — **not** authority store |
| **Persistence (ADR-0019)** | Project-scoped operational facts for hold/context — **mechanism** not designed here ([GAP-055](../../Development/EDF_Gap_Register.md)) |
| **F-layer artifacts** | Durable evidence (MVR, etc.) — canonical when promoted; operational records **reference** |

### 3. Transport carries; governance owns

Relay and provider layers **MAY** transport interaction needs and answers in governed packages.

**Semantic ownership** of need classification, routing, hold, authorization effect, and provenance **SHALL** remain with ProjectConcord framework + prescribed-workflow policy — **not** with the provider session or unstructured model output.

### 4. Distinction from STOP and PCON-0003 pause

| Mechanism | Role |
|---|---|
| **Interaction Hold (GIES)** | Short-to-medium hold on a **governed operation** pending interaction resolution |
| **STOP (PC-AIGOV-007, relay)** | Fail-closed halt on unauthorized or indeterminate progress |
| **PCON-0003 pause** | Broader continuation/pause across project or tranche scope |

These **MAY compose**; they **SHALL NOT** be conflated in implementation planning.

### 5. Future normative amendments (identified, not executed)

| Artifact | Expected amendment |
|---|---|
| [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) | Small GEW/GIES applicability cross-reference and interaction constraints under GEW — **separate tranche** |
| [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) | Interaction need/answer package profiles, validation rules — **after** hold model stable |
| [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) | Optional Attention facet reconciliation — **if** PA directs |

### 6. Watch and gaps

- [AWI-0011](../Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md) — normative publication and runtime follow-through
- [GAP-055](../../Development/EDF_Gap_Register.md) — GIES runtime, persistence, routing implementation
- Related: [GAP-027](../../Development/EDF_Gap_Register.md), [GAP-054](../../Development/EDF_Gap_Register.md), [GAP-038](../../Development/EDF_Gap_Register.md) / [AWI-0002](../Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md)

## Consequences

### Positive

- Clear separation of interaction governance from transport and UI.
- Reusable framework capability for GEW and future prescribed workflows.

### Negative

- Runtime, persistence, and relay package work remain open.
- GEW profile-specific interaction policy still to be specified.

### Risks

- Implementers conflate provider Q&A with governance — mitigated by GIES-6 and SPEC-006 validation discipline.

## References

- [PCON-0006](../PCON-0006-Governed-Interactive-Engineering-Session-Framework.md)
- [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md), [ADR-0024](ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md), [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md), [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md), [ADR-0022](ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md)
- [PCON-0003](../PCON-0003-Governed-Pause-Continuation-and-Resume.md)
