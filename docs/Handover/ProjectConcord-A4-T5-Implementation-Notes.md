# ProjectConcord A4-T5 — Implementation Notes

**Tranche:** A4-T5 — Operator projections, P0 regression, minimal Desktop  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** (2026-10-01)  
**Pre-T5 baseline:** `12aa29924897f565ac3c670503495d114da22c6f` (A4-T4 publication on `main`)  
**T5 publication:** Recorded on `main` (2026-10-01 tranche — see plan [§32](ProjectConcord-A4-Implementation-Plan.md#32-a4-t5-closeout-2026-10-01))

## Authorization

| Item | Status |
|------|--------|
| A4-T5 implementation | **PA AUTHORIZED** (2026-10-01) |
| A4-T5 acceptance / publication | **CLOSED / PA ACCEPTED / PUBLISHED** (2026-10-01) |
| A4-T6 … A4-T7 | **NOT AUTHORIZED** |

## Operator projections (Application)

| Artifact | Role |
|----------|------|
| `Edf.Application/Operator/*` | Derived Attention / Next Action DTOs and stable codes (ADR-0020) |
| `IOperatorAttentionContributor` | Contributor port; no canonical state mutation |
| `EngineeringAgentTransportOperatorAttentionContributor` | Maps plugin/hosting/transport outcomes to Attention (ADR-0022 §14) |
| `GovernedRelayManualP0NextActionContributor` | **Recommended** manual P0 when eligible and automation unavailable/failed |
| `RelayWorkflowOperatorProjectionService` | Aggregates contributors; status summary; forward eligibility flag |

**Derivation rules (binding T5 minimum):**

| Condition | Projection |
|-----------|------------|
| Plugin unavailable / not registered / disabled | Attention (`plugin-unavailable`) |
| Host/render/routing incompatibility | Attention (`plugin-incompatible`) |
| Auth / unhealthy plugin | Attention (`authentication-failure`) |
| Init / not initialized | Attention (`initialization-failure`) |
| Forward failure | Attention (`forward-failure`) |
| Result import rejection / retrieval failure | Attention (`result-retrieval-failure`) — derived report of governed import outcome only |
| Ambiguous / cancel-ambiguous / recoverable ambiguous states | Attention (`ambiguous-transport` / `recoverable-transport`) |
| P0 manual relay eligible + automation attention or failed transport | **Recommended** Next Action (`manual-p0`) — not Required |

Projections are **derived**; transport operational state is **not** promoted to package governance validity.

## Desktop integration

| Surface | Change |
|---------|--------|
| `RelayWorkflowViewModel` | Injects `IEngineeringAgentAutomatedTransportService` + `RelayWorkflowOperatorProjectionService`; **no** orchestration duplication |
| Commands | `ForwardAutomatedHandoverCommand`, `CancelAutomatedTransportCommand` |
| Properties | `AttentionItems`, `AutomatedTransportStatus`, `RecommendedManualRelaySummary`, automated command gating |
| `MainWindow.axaml` | Minimal automated-transport section (status, attention, forward/cancel) |
| `ApplicationCompositionRoot` | `DesktopApplicationServices.RelayOperatorProjections` |

P0 **Prepare / Copy / Import** paths unchanged; existing tests remain green.

## P0 fallback / recovery / provider boundaries

- Production catalog remains **empty** (`EngineeringAgentPluginCatalog.CreateEmpty()`).
- No Cursor/ACP/provider subprocess in T5.
- `FakeEngineeringAgentProviderPlugin` remains **tests-only**.
- No startup/background recovery, auto-retry, or auto-import introduced.
- T4 `IEngineeringAgentTransportRecoveryService` unchanged; not wired to Desktop auto-exec.

## Tests

| Suite | Focus |
|-------|--------|
| `EngineeringAgentA4T5OperatorProjectionTests` | **6** — attention, recommended P0, auth/forward/ambiguous, production composition |
| `RelayWorkflowViewModelAutomatedTransportTests` | **2** — VM attention/status; manual prepare after failed automated forward |
| Full regression | See validation table |

## Final validation (publication)

| Item | Value |
|------|--------|
| SDK | **.NET 10.0.401** (`DOTNET_ROOT=$HOME/.dotnet`) |
| Build | `dotnet build -c Release` — **0** errors; **12× NU1903** (GAP-045 / AWI-0007) |
| Test | `dotnet test -c Release` — **233** passed, **0** failed (T4 baseline **225**; **+8** T5) |

## STOP

**A4-T6 NOT AUTHORIZED** pending separate Project Architect authorization. No Cursor provider, MVR-0003, or A3 in T5 scope.
