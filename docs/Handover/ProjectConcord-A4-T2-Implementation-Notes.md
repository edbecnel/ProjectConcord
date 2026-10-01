# ProjectConcord A4-T2 — Implementation Notes

**Tranche:** A4-T2 — Durable transport-operation persistence  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** (2026-10-01)  
**Pre-T2 baseline:** `2656d93a1e94b307bcb93655673d7b542b365747` (A4-T1 publication on `main`)  
**Publication:** A4-T2 publication commit on `main` (2026-10-01 tranche)

## Authorization

| Item | Status |
|------|--------|
| A4-T2 implementation | **PA AUTHORIZED** (2026-10-01) |
| A4-T2 acceptance / publication | **CLOSED / PA ACCEPTED / PUBLISHED** (2026-10-01) |
| A4-T3 … A4-T7 | **NOT AUTHORIZED** |
| A4 overall | **Not complete** — T3+ not authorized |

## Persistence architecture reused

- **`SchemaMigrationRunner`** forward migrations (ADR-0019)
- **`SqliteUserApplicationStateStore`** partial class + command/lock conventions
- **`IUserApplicationStatePersistence`** port surface (mirrors **`RelayOperational`**)
- **`SqliteTransportOperationStoreAdapter`** / **`InMemoryTransportOperationStore`** → **`ITransportOperationStore`**
- **`PersistedTransportOperation`** row DTO in **`Edf.ProjectServices`** (Application types mapped in adapter only)
- **`UserApplicationStatePersistenceFactory`**

## Migration003

| Item | Value |
|------|--------|
| Class | **`Migration003TransportOperations`** |
| Version | **3** (`SchemaVersions.Current = 3`) |
| Ordering | **001 → 002 → 003** (additive; prior migrations unchanged) |
| Table | **`transport_operation`** |

### Schema summary

| Column | Mapping |
|--------|---------|
| `transport_operation_id` (PK) | `TransportOperationId` Guid `"D"` |
| `source_package_id` | `GovernedPackageId` |
| `correlation_id` | `GovernedCorrelationId` |
| `provider_plugin_id` | `EngineeringAgentProviderPluginId.Value` (TEXT; implementation mapping only) |
| `attempt` | `int` |
| `lifecycle_state` | `TransportOperationLifecycleState` as INTEGER |
| `provider_session_hint` | nullable opaque **`EngineeringAgentProviderSessionHandle.Value`** (never **`ToString()`**) |
| `result_import_package_id` | nullable `GovernedPackageId` linkage only |
| `created_utc` / `updated_utc` | UTC ISO 8601 `"O"` |

## Save / Get semantics

- **Save:** `INSERT … ON CONFLICT(transport_operation_id) DO UPDATE` — one logical row per **`TransportOperationId`** (persistence idempotency only; not transport retry policy)
- **Get:** by **`TransportOperationId`**; missing → `null`
- **No** generalized query API added

## Identity separation

Distinct columns and round-trip types for operation id, source package id, correlation id, plugin id string, session opaque value, and optional result import package id. No identity aliasing.

## Project scoping (PA boundary note)

**`TransportOperation`** does **not** contain **`ProjectConcordProjectId`**. This is **acceptable** for the current **`Save` / `Get(TransportOperationId)`** store contract; Project context remains **indirect** via governed package/correlation references.

If a later tranche (orchestration, restart recovery) requires discovering operations **by Project**, unresolved/in-flight state, lifecycle filters, or other dimensions, **do not** silently add Project id columns, project-indexed queries, or recovery query surfaces. **STOP for PA evaluation** in the responsible tranche unless explicitly authorized. This note is a **boundary/watch condition**, not a new GAP/AWI.

## Operational vs canonical

Transport rows are **ProjectConcord operational state** only — not canonical EDF content, governance acceptance, provider authority, or validated engineering results.

## Result / untrusted output

T2 persists optional **`ResultImportPackageId`** as **governed package linkage** only. No provider result text; no **`EngineeringResultImport`**; no treated-as-accepted results.

## Lifecycle

**`TransportOperationLifecycleState`** round-trips faithfully, including **`Ambiguous`** — not normalized to failed/closed/completed in the persistence layer.

## Restart / migration evidence

- **`Restart_Readback_ReconstructsOperation`:** save → dispose → reopen same DB → faithful **`Get`**
- **`Migration003_ExistingUserPreferencesSurvive`:** v2 DB + preference → v3 + preference retained

## Composition

- **`IUserApplicationStatePersistence.TransportOperations`**
- **`DesktopApplicationServices.TransportOperations`**
- Does **not** trigger Forward, polling, import, or orchestration

## Tests

**`TransportOperationPersistenceTests`** (12 tests). Schema version expectations updated in **`FreshDatabase_CreatesCurrentSchemaVersion`** and **`Migration002_UpgradesSchemaVersionOneDatabase`** for **`Current = 3`**.

## Validation

| Command | Result |
|---------|--------|
| `DOTNET_ROOT=$HOME/.dotnet PATH=$DOTNET_ROOT:$PATH dotnet build -c Release` | **0** errors; **12× NU1903** (GAP-045 / AWI-0007) |
| `dotnet test -c Release` | **187** passed, **0** failed |

## Deviations

| Topic | Notes |
|-------|--------|
| Row DTO in ProjectServices | Avoids ProjectServices → Application reference |
| Schema tests | Updated for migration chain ending at **v3** |

## GAP / AWI

No new architecture gap from T2. **NU1903** remains **GAP-045** / **AWI-0007**.

## STOP

**A4-T3 NOT AUTHORIZED.** Next PA decision: authorize **A4-T3 only** (orchestration with test fake provider).
