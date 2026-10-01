# ProjectConcord A4-T4 — Implementation Notes

**Tranche:** A4-T4 — Restart recovery, idempotency, in-flight reconciliation  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** (2026-10-01)  
**Pre-T4 baseline:** `90c29b5c0dd56900c5ba44d6cd6f9df7dd19a07a` (A4-T3 publication on `main`)  
**T4 publication:** Recorded on `main` (2026-10-01 tranche — see plan [§31](ProjectConcord-A4-Implementation-Plan.md#31-a4-t4-closeout-2026-10-01))

## Authorization

| Item | Status |
|------|--------|
| A4-T4 implementation | **PA AUTHORIZED** (2026-10-01) |
| A4-T4 acceptance / publication | **CLOSED / PA ACCEPTED / PUBLISHED** (2026-10-01) |
| A4-T5 … A4-T7 | **NOT AUTHORIZED** |

## PA pre-dispatch durability correction (bounded T3 refinement)

**Original crash window:** Persisting only **`CreatedNotForwarded`** before provider **`Forward`** allowed restart classification as safe resume after a crash in the post-Forward persistence window — risking duplicate dispatch.

**Correction:** Live transport and explicit resume now follow:

1. Create **`TransportOperation`**
2. Persist **`CreatedNotForwarded`**
3. Transition to **`ForwardInProgress`** and persist **`ForwardInProgress`** (durable uncertainty boundary **before** external **`Forward`**)
4. Provider **`Forward`**
5. Persist resulting lifecycle state

If **`ForwardInProgress`** save fails after **`CreatedNotForwarded`** succeeds, provider **`Forward`** **must not** occur (tests: **`ForwardCallCount == 0`**).

**Semantics:**

- **`CreatedNotForwarded`** — ProjectConcord has **not** durably crossed the external Forward boundary; may map to **`SafeToResumeBeforeDispatch`**; resume remains explicit, same-operation, nonautomatic.
- **`ForwardInProgress`** — boundary after which provider **`Forward`** **may** have occurred; **not** proof of provider receipt; recovery **must not** blindly redispatch; **`ProviderReconciliationRequired`** or equivalent conservative handling.

**A4-T3** remains **CLOSED / PA ACCEPTED / PUBLISHED** — this is a bounded implementation refinement, not a T3 republish.

## Recovery service

| Surface | Role |
|---------|------|
| **`IEngineeringAgentTransportRecoveryService`** | Explicit restart recovery (no startup auto-execution) |
| **`DiscoverRecoverableOperations`** | Project-scoped discovery |
| **`AssessOperationAsync`** | Classify disposition; optional untrusted candidate text (no import) |
| **`ResumeCreatedNotForwardedAsync`** | Explicit resume — same **`TransportOperationId`**, same row; persists **`ForwardInProgress`** before **`Forward`**; **`Attempt = 1`** |
| **`ReconcileProviderStateAsync`** | Neutral **`TryGetResultCandidate`**; may persist **`ResultCandidateReceived`** — **no import** |
| **`ConfirmRecoveredResultImportAsync`** | Operator-confirmed import path (AF-1) via **`ImportEngineeringResult`** |

**Composition:** **`DesktopApplicationServices.TransportRecovery`**

## Recovery query (authorized in T4)

**`ITransportOperationStore.GetRecoverableOperations(ProjectConcordProjectId)`**

- SQLite: **`INNER JOIN relay_package`** on **`source_package_id`**, filter recoverable lifecycle values
- In-memory: filter by lifecycle + relay package **`ProjectId`**
- **Not** **`GetAll`**, search, query, or transport history API

## Project scoping

No **`ProjectConcordProjectId`** field on **`TransportOperation`**. Scope: **`transport_operation.source_package_id` → relay_package → ProjectId`**. Caller supplies active Project id.

## Recoverable vs terminal lifecycle

**Recoverable (discovery):** **`CreatedNotForwarded`**, **`ForwardInProgress`**, **`ForwardAcknowledged`**, **`AwaitingResult`**, **`ResultCandidateReceived`**, **`Ambiguous`**, **`TimedOut`**

**Terminal (excluded):** **`ImportCompleted`**, **`ImportRejected`**, **`Cancelled`**, **`Closed`**, **`ForwardFailed`**

## Recovery disposition model

**`EngineeringAgentTransportRecoveryDisposition`**: **`SafeToResumeBeforeDispatch`**, **`ProviderReconciliationRequired`**, **`AmbiguousRequiresOperatorDecision`**, **`ResultCandidateRequiresConfirmation`**, **`ProviderUnavailable`**, **`RecoveryFailed`**, **`NoActionRequired`**

Key mappings: **`CreatedNotForwarded`** → safe resume; **`ForwardInProgress`** / **`ForwardAcknowledged`** → reconciliation; **`Ambiguous`** → operator decision; **`ResultCandidateReceived`** → confirmation required.

## Idempotency and provider identity

- Assessment/reconcile/discover **do not** **`Forward`** for uncertain states
- **`ResumeCreatedNotForwardedAsync`** only for **`CreatedNotForwarded`**; preserves **`TransportOperationId`**, **`GovernedPackageId`**, **`GovernedCorrelationId`**, **`ProviderPluginId`**
- No provider substitution; missing/disabled recorded provider → disposition, not re-selection
- No generalized retry/replay engine

## AF-1 recovered result

**`ResultCandidateReceived`** → **`ResultCandidateRequiresConfirmation`**; **`RequiresOperatorConfirmationBeforeImport`**. **`ImportEngineeringResult`** only via **`ConfirmRecoveredResultImportAsync`**.

Candidate body via **`TryGetResultCandidate`** (untrusted); not persisted as canonical content.

## Provider / session

**`EngineeringAgentPluginHost`** for recorded **`ProviderPluginId`** when init required. **`ProviderSessionHint`**: opaque, noncanonical; neutral code does not parse.

## Startup / background / P0

No automatic recovery, import, cancel, or retry at startup. No periodic polling or background worker. P0 relay unchanged and not auto-invoked from recovery.

## Tests

**`EngineeringAgentA4T4RecoveryTests`** — **14** tests. T3 orchestration tests cover pre-dispatch ordering and persistence failure. Fake provider harness only.

## Final validation (publication)

| Item | Value |
|------|--------|
| SDK | **.NET 10.0.401** (`DOTNET_ROOT=$HOME/.dotnet`) |
| Build | `DOTNET_ROOT=$HOME/.dotnet PATH=$DOTNET_ROOT:$PATH dotnet build -c Release` — **0** errors; **12× NU1903** |
| Test | `dotnet test -c Release` — **225** passed, **0** failed |

## GAP / AWI

No new gap. **NU1903** remains **GAP-045** / **AWI-0007**.

## STOP

**A4-T5 NOT AUTHORIZED** pending separate Project Architect authorization. No automated-transport UI, Cursor provider, or A3 in T4 scope.
