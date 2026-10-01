# ProjectConcord A4-T3 — Implementation Notes

**Tranche:** A4-T3 — Provider-neutral automated transport orchestration (single operation)  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** (2026-10-01)  
**Pre-T3 baseline:** `06c3b196bbd5d996c202ad3a2d1efc3df5ecbba6` (A4-T2 publication on `main`)  
**Publication:** A4-T3 publication commit on `main` (2026-10-01 tranche)

## Authorization

| Item | Status |
|------|--------|
| A4-T3 implementation | **PA AUTHORIZED** (2026-10-01) |
| A4-T3 acceptance / publication | **CLOSED / PA ACCEPTED / PUBLISHED** (2026-10-01) |
| A4-T4 … A4-T7 | **NOT AUTHORIZED** |
| A4 overall | **Not complete** — T4+ not authorized |

## Application service / orchestrator

| Surface | Role |
|---------|------|
| **`IEngineeringAgentAutomatedTransportService`** | Application-facing automated transport |
| **`IEngineeringAgentTransportOrchestrator`** | Same contract; **`EngineeringAgentTransportOrchestrator`** |
| **`ForwardGovernedHandoverAsync`** | Single-operation forward path (see ordering below) |
| **`CancelTransportOperationAsync`** | Cancel one operation by **`TransportOperationId`** |

**Composition:** **`DesktopApplicationServices.AutomatedTransport`** via **`ApplicationCompositionRoot`**.

## Governance-first flow

1. Load/validate governed source package; P0 **`PrepareEngineeringAgentHandover`**
2. **No provider init** until governance succeeds (tests: ineligible → **`InitializeAsyncCallCount == 0`**)

## Provider preflight (Stage A — bounded T1 refinement)

**`EngineeringAgentPluginPreflightEvaluation`** + **`EvaluatePreflightForAutomatedTransport`**

Static gates: selected, registered, enabled, automated transport support, host/render/routing compatibility.

**Not required:** host initialized, runtime health, authentication.

Preflight rejections use **`EngineeringAgentPluginSelectionUnavailableReason`** except **`PluginNotInitialized`** / **`PluginUnhealthy`**.

## Provider initialization

After preflight only: **`EngineeringAgentPluginHost.InitializePluginAsync`**.

Init failure → **`ProviderUnavailable`**; **no** **`TransportOperation`**; **no** **`Forward`**; no retry.

## Final runtime readiness (Stage B)

**`EvaluateRuntimeReadinessForAutomatedTransport`** — host initialized + provider health initialized + authenticated.

**`EvaluateForAutomatedTransport`** delegates to runtime readiness (T1 tests unchanged). Final Forward gate **not weakened**.

Post-init readiness failure → **`ProviderUnavailable`**; **no** operation; **no** **`Forward`**. Host does **not** auto-shutdown on readiness failure.

## Bounded T1 refinement record

T3 separated static preflight from runtime readiness in **`EngineeringAgentPluginSelectionService`**.

This does **not** reopen, republish, or weaken **A4-T1** (remains **CLOSED / PA ACCEPTED / PUBLISHED**).

## TransportOperation timing

Created **only after** full runtime readiness.

**`TransportOperationId`** distinct from package/correlation ids; **`Attempt = 1`**.

## Persistence before Forward

**`CreatedNotForwarded`** **`Save`** before **`IEngineeringAgentProviderPlugin.Forward`**. Persistence failure → **`PersistenceFailed`**; Forward not invoked.

## Forward / failure / ambiguity

Neutral **`EngineeringAgentForwardRequest`** / **`EngineeringAgentForwardResult`**. Immediate failure and **`AmbiguousOutcome`** persisted deterministically. No retry, replay, substitution, or implicit P0.

## Session handle

Opaque **`ProviderSessionHint`** on operation — operational continuity only; noncanonical.

## Result candidate / import

**`TryGetResultCandidate`** — **untrusted** until **`ImportEngineeringResult`**. Valid → **`ImportCompleted`** + **`ResultImportPackageId`**; invalid → **`ImportRejected`**. No recovered-result confirmation (T4 / ADR-0022 AF-1).

## Cancellation

**`operation.ProviderPluginId`** only — no selection/substitution. Host init for **that** plugin if needed to invoke **`Cancel`**. **`ProviderSessionHint`** preserved on row. Ambiguous cancel → **`Ambiguous`** state.

## Project scoping

Caller supplies **`ProjectConcordProjectId`** + **`SourcePackageId`**. No **`ProjectConcordProjectId`** on **`TransportOperation`**; no Project-wide store queries.

## Production providers

Catalog **empty** (0 production providers). **`FakeEngineeringAgentProviderPlugin`** — **tests only**.

## P0

**`IGovernedRelayP0WorkflowService`** unchanged; P0 viable when automation unavailable; no automatic P0 on transport failure.

## PA ordering correction (accepted before publication)

Removed pre-selection host init before static eligibility. Published ordering matches §2 of T3 acceptance handover.

## Tests

**`EngineeringAgentA4T3OrchestrationTests`** — **25** tests (ordering + orchestration; fake plugin in test harness only).

## Validation

| Command | Result |
|---------|--------|
| `DOTNET_ROOT=$HOME/.dotnet PATH=$DOTNET_ROOT:$PATH dotnet build -c Release` | **0** errors; **12× NU1903** (GAP-045 / AWI-0007) |
| `dotnet test -c Release` | **211** passed, **0** failed |

## GAP / AWI

No new gap. **NU1903** remains **GAP-045** / **AWI-0007**.

## STOP

**A4-T4 NOT AUTHORIZED.** Next PA decision: authorize **A4-T4 only** (recovery/retry policy) when ready.
