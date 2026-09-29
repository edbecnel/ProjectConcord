# ProjectConcord A2-T3 — Implementation Notes

**Tranche:** A2-T3 — Relay continuity, persistence, and provenance  
**Implementation baseline:** `1af4dac7a49118d498c8f56948a0c683fb2ce9d0`  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-30)

## Scope delivered

| Area | Location |
|------|----------|
| Provenance / continuity domain types | `src/Edf.Domain/Relay/RelayAgentRole.cs`, `RelayProvenanceEvent*.cs`, `PersistedGovernedRelayPackage.cs` |
| Application ports + orchestrator (no render/UI) | `src/Edf.Application/Relay/IRelayOperationalStore.cs`, `IGovernedInteractionRelayService.cs`, `GovernedInteractionRelayService.cs`, `RelayValidationResultMapper.cs` |
| SQLite + in-memory persistence | `SqliteRelayOperationalStoreAdapter`, `InMemoryRelayOperationalStore`, `SqliteUserApplicationStateStore.Relay.cs`, `Persistence/Relay/RelayPersistenceJson.cs` |
| Migration002 | `src/Edf.ProjectServices/Persistence/Migrations/Migration002RelayOperational.cs` |
| Integration tests | `tests/Edf.ProjectServices.Tests/Relay/RelayOperationalPersistenceTests.cs` |

## Migration

- **Name:** `Migration002RelayOperational`
- **Version:** `2` (`SchemaVersions.Current`)
- **Runner:** Fresh databases apply `Migration001Initial` then forward migrations to 002; version-1 databases upgrade forward-only

## Schema (operational SQLite)

1. **`relay_continuity`** — PK `(project_id, agent_role)`; nullable `user_intent`; `advisory_json`; `updated_utc`
2. **`relay_package`** — envelope + `governance_critical_json`, `tier0_json`, `profile_payload` BLOB, `validation_state`, `validation_diagnostics_json`, `structural_agreement_json`, optional `rendered_body_hash`
3. **`relay_provenance_event`** — append-only; `project_id`; optional `package_id` / `correlation_id`; `event_type`; minimal `payload_json`

Partition key: **`ProjectConcordProjectId`** (`project_id` FK to `managed_projects`). Locator relocation does not rewrite durable package `project_id`.

## Accepted semantics (PA 2026-09-30)

- **Continuity:** Separate PA / engineering intent and advisory; intent nullable; no manufactured NEW/CONTINUE; advisories do not override intent.
- **Validation:** `Valid` / `Incomplete` / `RejectedMalformed` round-trip without promotion or repair; missing governance fields stay missing.
- **STOP:** `RelayStopMetadata` persisted without conflating STOP with authorization disposition.
- **Provenance:** PC-PAR-021 event types; order `recorded_utc ASC`, `event_id ASC`; no canonical chat transcripts.
- **Tier-0:** `Tier0RelaySnapshot` on packages when captured at produce time; observational context only.

## Explicit non-implementation (T3)

No substantive T4 Software Development profile; no T5 serialization/provider; no T6 Cursor bridge; no T7 relay UI; no MVR; no governed-repository persistence; no `.projectconcord/` creation; no A3/A4.

## Test evidence (PA accepted)

| Suite | Result |
|-------|--------|
| `RelayOperationalPersistenceTests` | **17 passed** |
| `Edf.Application.Tests` (Release) | 30 passed |
| `Edf.ProjectServices.Tests` (Release) | 42 passed |
| `Edf.Desktop.Tests` (Release) | 4 passed |
| **Total** | **76 passed** |
| `dotnet build -c Release` | 0 errors (pre-existing NU1903 on `SQLitePCLRaw.lib.e_sqlite3`) |

Commands:

- `dotnet test -c Release tests/Edf.ProjectServices.Tests --filter "FullyQualifiedName~RelayOperational"`
- `dotnet test -c Release`

## Authorization boundary

**A2-T4 through A2-T8:** NOT AUTHORIZED. **A3 / A4:** NOT AUTHORIZED. **A2 overall:** not complete.
