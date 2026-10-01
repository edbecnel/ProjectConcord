# ProjectConcord A4-T0 — Implementation Notes

**Tranche:** A4-T0 — Contracts and neutral implementation skeleton  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** (2026-10-01)  
**Plan baseline:** `56b0e272a2da82f3377578dd00c3bb409cfe736f` (A4 implementation plan publication)  
**Publication:** A4-T0 publication commit on `main` (2026-10-01 tranche)

## Authorization

| Item | Status |
|------|--------|
| A4-T0 implementation | **PA AUTHORIZED** (2026-10-01) |
| A4-T0 acceptance / publication | **CLOSED / PA ACCEPTED / PUBLISHED** (2026-10-01) |
| A4-T1 … A4-T7 | **NOT AUTHORIZED** |
| A4 overall | **Not complete** — T1+ not authorized |

## Accepted implementation scope

Provider-neutral **`IEngineeringAgentProviderPlugin`** and related types; **`IEngineeringAgentPluginCatalog`**, **`IEngineeringAgentPluginHost`**, **`ITransportOperationStore`** (ports only); Application-layer **`TransportOperation`** model; permission policy; T0 contract tests; test-only fake plugin. **No** catalog/host/store implementations, **no** orchestration, **no** Cursor/ACP, **no** Desktop changes.

## Changed files (T0 publication)

| Area | Paths |
|------|--------|
| Application contracts | `src/Edf.Application/Relay/EngineeringAgent/Plugins/*.cs` (17 files) |
| Transport operational types | `src/Edf.Application/Relay/EngineeringAgent/Transport/*.cs` (5 files) |
| Tests | `tests/Edf.Application.Tests/Relay/EngineeringAgent/*.cs` (2 files) |
| Plan / notes | `docs/Handover/ProjectConcord-A4-Implementation-Plan.md`, this file |
| Indexes | `docs/Handover/README.md`, `PROJECT_INDEX.md`, `docs/Development/Implementation_Roadmap.md` (T0 status only) |

**Not changed:** `IEngineeringAgentRelayBridge`, `EngineeringAgentManualRelayBridge`, Core/B domain governance, Desktop, ProjectServices migrations.

## Type placement

| Concept | Location |
|---------|----------|
| `TransportOperationId`, `TransportOperation`, lifecycle enum, identity rules | `Edf.Application` … `/Transport/` |
| Plugin contract + ports | `Edf.Application` … `/Plugins/` |
| Governed package/correlation ids | **`Edf.Domain`** (unchanged, referenced only) |

## Deferred orchestration interfaces (PA accepted)

| Interface | Disposition |
|-----------|-------------|
| `IEngineeringAgentTransportOrchestrator` | **Not in T0** — **A4-T3** |
| `IEngineeringAgentAutomatedTransportService` | **Not in T0** — **A4-T3** |

## Provider Plugin Identity

`EngineeringAgentProviderPluginId` — `readonly record struct` over **non-empty string** (`Parse`). T0 implementation choice only; ADR-0023 does **not** require string representation for all future implementations.

## Validation environment

| Setting | Value |
|---------|--------|
| SDK | **10.0.401** (`global.json`) |
| Invocation | `DOTNET_ROOT=$HOME/.dotnet` `PATH=$DOTNET_ROOT:$PATH` |

| Command | Exit | Result |
|---------|------|--------|
| `dotnet build -c Release` | **0** | Success, **0 errors** |
| Warnings | — | **12× NU1903** `SQLitePCLRaw.lib.e_sqlite3` — pre-existing ([GAP-045](../Development/EDF_Gap_Register.md), [AWI-0007](../Architecture/Watch_Items/AWI-0007-SQLite-Transitive-NuGet-Advisory.md)) |
| `dotnet test -c Release tests/Edf.Application.Tests/Edf.Application.Tests.csproj --filter "FullyQualifiedName~EngineeringAgentA4T0Contract"` | **0** | **13** passed, 0 failed, 0 skipped |
| `dotnet test -c Release` | **0** | **156** passed (Application **104**, ProjectServices **43**, Desktop **9**); A2/P0 relay regression **pass** |

## Accepted deviations

| Topic | Deviation |
|-------|-----------|
| Orchestrator/service interfaces | Removed from T0 (deferred T3) vs original empty skeleton |
| `TransportOperationId` | Application layer, not Domain |

## STOP

**A4-T1 NOT AUTHORIZED.** Next PA decision: authorize **A4-T1 only** (static catalog / hosting / selection).
