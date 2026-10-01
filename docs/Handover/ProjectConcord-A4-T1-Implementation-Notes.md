# ProjectConcord A4-T1 — Implementation Notes

**Tranche:** A4-T1 — Static catalog, hosting, selection, compatibility, lifecycle  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** (2026-10-01)  
**Pre-T1 baseline:** `112667ef78f1ed5e123e66910d7b92891feb8d78` (A4-T0 publication on `main`)  
**Publication:** A4-T1 publication commit on `main` (2026-10-01 tranche)

## Authorization

| Item | Status |
|------|--------|
| A4-T1 implementation | **PA AUTHORIZED** (2026-10-01) |
| A4-T1 acceptance / publication | **CLOSED / PA ACCEPTED / PUBLISHED** (2026-10-01) |
| A4-T2 … A4-T7 | **NOT AUTHORIZED** |
| A4 overall | **Not complete** — T2+ not authorized |
| A4-T0 | **Remains CLOSED** — bounded contract refinement recorded below; not a T0 republication |

## Baseline verification (publication)

| Check | Value |
|-------|--------|
| Pre-T1 `HEAD` | `112667ef78f1ed5e123e66910d7b92891feb8d78` |
| Post-T1 | T1 publication commit on `main` (2026-10-01) |

## T0 bounded contract refinement (publication record)

T1 publication adds provider-neutral lifecycle operations to the **published T0** `IEngineeringAgentProviderPlugin` surface:

- `Task<EngineeringAgentProviderInitializeResult> InitializeAsync(CancellationToken)`
- `Task<EngineeringAgentProviderShutdownResult> ShutdownAsync(CancellationToken)`

Supporting result types: `EngineeringAgentProviderInitializeResult`, `EngineeringAgentProviderShutdownResult`. Host port `IEngineeringAgentPluginHost` exposes `InitializePluginAsync` / `ShutdownPluginAsync`.

This refinement completes ADR-0023 host lifecycle semantics. **A4-T0 is not reopened** and remains architecturally valid; canonical contract after T1 includes lifecycle operations.

## Changed files (T1 publication)

| Area | Paths |
|------|--------|
| Hosting | `src/Edf.Application/Relay/EngineeringAgent/Hosting/*.cs` (7 files) |
| Plugin contract (bounded T0 refinement) | `IEngineeringAgentProviderPlugin.cs`, `IEngineeringAgentPluginHost.cs`, `EngineeringAgentProviderInitializeResult.cs`, `EngineeringAgentProviderShutdownResult.cs` |
| Composition | `src/Edf.Application/Composition/EngineeringAgentPluginHostingFactory.cs` (new), `ApplicationCompositionRoot.cs` |
| SQLite preferences | `src/Edf.Application/Projects/Sqlite/SqliteEngineeringAgentOperationalPreferenceBacking.cs` (new), `SqliteUserApplicationStatePersistence.cs` |
| ProjectServices | `src/Edf.ProjectServices/Persistence/SqliteUserApplicationStateStore.cs` (`GetUserPreference` / `SetUserPreference`) |
| Tests | `tests/Edf.Application.Tests/Relay/EngineeringAgent/EngineeringAgentA4T1HostingTests.cs` (new), `FakeEngineeringAgentProviderPlugin.cs`, `EngineeringAgentA4T0ContractTests.cs` |
| Docs | `docs/Handover/ProjectConcord-A4-Implementation-Plan.md`, this file |

**Not changed:** `IEngineeringAgentRelayBridge`, transport orchestration, `Migration003`, Desktop UI, Cursor/ACP, Core/B governance.

## Static catalog (`EngineeringAgentPluginCatalog`)

- Implements `IEngineeringAgentPluginCatalog` with **explicit** registration only (`CreateEmpty`, `Register`, `FromRegistrations`).
- **No** filesystem/assembly/reflection discovery.
- **Duplicate identity:** throws `EngineeringAgentPluginCatalogDuplicateIdentityException` (tested).
- **`RegisteredPluginIds`:** ordered by `id.Value` with `StringComparer.Ordinal` (deterministic).

## PA lifecycle correction (2026-10-01)

**Issue:** Initial T1 host called `GetHealth().IsInitialized` instead of invoking provider lifecycle operations — providers could appear “initialized” without host-coordinated `Initialize`.

**Correction:** Bounded refinement of published T0 **`IEngineeringAgentProviderPlugin`** and **`IEngineeringAgentPluginHost`** (authorized for ADR-0023 implementability).

### Provider lifecycle contract

| Operation | Signature | Result |
|-----------|-----------|--------|
| Initialize | `Task<EngineeringAgentProviderInitializeResult> InitializeAsync(CancellationToken)` | `Health` + optional `Failure`; `Succeeded` when `Failure` is null and `Health.IsInitialized` |
| Shutdown | `Task<EngineeringAgentProviderShutdownResult> ShutdownAsync(CancellationToken)` | `IsAcknowledged` + optional `Failure` |

### Sync/async rationale

- **`Task` + `CancellationToken`** on lifecycle methods: forward/cancel transport calls remain synchronous with cancellation, but future Cursor provider lifecycle may perform external/process I/O; async aligns with Desktop UI patterns and avoids a later breaking contract change.
- Host methods **`InitializePluginAsync` / `ShutdownPluginAsync`** mirror the provider surface.

### Host semantics

| Case | Behavior |
|------|----------|
| Unknown plugin (init) | No provider call; unavailable health; host not initialized |
| Init success | Provider `InitializeAsync` → on `Succeeded`, host marks initialized |
| Init failure | Host **not** initialized; failure returned to caller |
| Repeat init (host already initialized) | Idempotent: returns current `GetHealth()` without second provider init |
| Shutdown (host not initialized) | Idempotent success; no provider call |
| Shutdown (initialized) | Provider `ShutdownAsync`; host marker **always cleared** after attempt (failure still clears host tracking; failure returned) |
| Repeat shutdown | Idempotent when host already not initialized |

**Health vs lifecycle:** `GetHealth()` observes state only; initialization/shutdown perform work via `InitializeAsync` / `ShutdownAsync`.

**Authentication:** No credentials or auth implementation in T1; fake may report `IsAuthenticated` after lifecycle init for selection readiness tests only.

## Plugin host (`EngineeringAgentPluginHost`)

- Implements `IEngineeringAgentPluginHost`; tracks host-side initialized set (separate from transport lifecycle).
- Coordinates provider **`InitializeAsync` / `ShutdownAsync`** as above (not health-gated init).

## Enablement and Project-scoped selection

- `IEngineeringAgentPluginProjectPreferencesStore` + `EngineeringAgentPluginProjectPreferencesStore`.
- Per-Project: selected `EngineeringAgentProviderPluginId` or none; enablement set (disabled plugins excluded from automated selection).
- **Operational state only** — not EDF canonical content, relay authority, or governance.

## Selection persistence decision

- Reuses existing **`user_preferences`** table via `SqliteUserApplicationStateStore.GetUserPreference` / `SetUserPreference`.
- Keys: `engineering_agent.plugin.selected.{projectId:D}`, `engineering_agent.plugin.enabled.{projectId:D}` (comma-separated plugin ids).
- In-memory composition (`InMemoryUserApplicationStatePersistence`) uses store without backing (empty preferences until set in-process).
- **No** Migration003, **no** transport-operation tables, **no** new generalized persistence subsystem.

## Compatibility (`EngineeringAgentPluginCompatibilityEvaluator`)

Three-part assessment (not collapsed):

1. **Host contract** — `SupportsAutomatedTransport`, `IsAvailableForSelection`
2. **Render protocol** — `IsCompatibleWithRenderProtocol(requiredMajor)` vs `RelayRenderVersion.V1.Major` default in selection
3. **Routing** — `RoutingIntentSupport.SupportsRoutingIntent(EngineeringAgentMode)` for PLAN / AGENT / DEBUG (DEBUG not aliased to AGENT)

## Selection evaluation (`EngineeringAgentPluginSelectionService`)

- `EngineeringAgentPluginSelectionEvaluation` + `EngineeringAgentPluginSelectionUnavailableReason` (no ADR-0020 Attention objects).
- Checks: selection → registration → enablement → compatibility triad → host initialized → health/authenticated.

## Production composition

- `EngineeringAgentPluginHostingFactory.Create` wires empty catalog, host, preferences (SQLite backing when applicable), selection service.
- `DesktopApplicationServices` exposes `EngineeringAgentPluginHosting`.
- **Zero** production provider registrations until A4-T6.
- **`FakeEngineeringAgentProviderPlugin`** — tests only.

## Explicitly not implemented (T1 boundary)

| Item | Disposition |
|------|-------------|
| `IEngineeringAgentTransportOrchestrator` | **A4-T3** |
| `IEngineeringAgentAutomatedTransportService` | **A4-T3** |
| `ITransportOperationStore` / SQLite adapter | **A4-T2** |
| Migration003 | **A4-T2** |
| Cursor / ACP / subprocess | **A4-T6+** |
| Desktop automated-transport UI | **Later tranches** |
| A3 | **NOT AUTHORIZED** |

## Tests added

`EngineeringAgentA4T1HostingTests` — **17** tests (including lifecycle invocation, init/shutdown failure, idempotency).  
`EngineeringAgentA4T0ContractTests` — added fake lifecycle contract test (bounded T0 reconciliation).

## Validation environment

| Setting | Value |
|---------|--------|
| SDK | **10.0.401** (`global.json`) |
| Invocation | `DOTNET_ROOT=$HOME/.dotnet` `PATH=$DOTNET_ROOT:$PATH` |

| Command | Exit | Result |
|---------|------|--------|
| `dotnet build -c Release` | **0** | Success, **0 errors** |
| Warnings | — | **12× NU1903** `SQLitePCLRaw.lib.e_sqlite3` — pre-existing |
| `dotnet test -c Release` | **0** | **174** passed (Application **122**, ProjectServices **43**, Desktop **9**); **0** failed |
| A2/P0 relay regression | — | **Pass** (included in full suite) |

## Deviations

| Topic | Notes |
|-------|--------|
| T0 provider/host ports | Bounded lifecycle methods added post–PA review (not a T0 republication) |
| Shutdown failure | Host tracking cleared after shutdown attempt even when provider reports failure |
| `EngineeringAgentPluginHostingFactory` | Public for composition and tests (minimal surface) |

## GAP / AWI

No new architecture gap from T1. **NU1903** / `SQLitePCLRaw.lib.e_sqlite3` remains governed by **GAP-045** / **AWI-0007** (pre-existing).

## STOP

**A4-T2 NOT AUTHORIZED.** Next PA decision: authorize **A4-T2 only** (transport-operation persistence / Migration003).
