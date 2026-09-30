# ProjectConcord A2-T6 — Implementation Notes

**Tranche:** A2-T6 — Cursor P0 manual bridge  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-30)  
**Implementation baseline:** `d051cc43bee0e0d9f25f1e01f6b75399de123ef6`  
**Publication:** Recorded on `main` at T6 closeout commit (see [A2 plan §32](ProjectConcord-A2-Implementation-Plan.md#32-a2-t6-closeout-2026-09-30)).

## Scope delivered

| Area | Location |
|------|----------|
| `ICursorRelayBridge` + P0 manual implementation | `src/Edf.Application/Relay/Cursor/*` |
| PC-PAR-014 / PC-PAR-022 seam | `CursorManualRelayBridge`, `RelayValidatedHandoverEligibility` (T1, consumed) |
| Focused unit tests | `tests/Edf.Application.Tests/Relay/CursorManualRelayBridgeTests.cs` |

## Architecture

- **Boundary:** Validated governed relay (`RelayValidationState.Valid` per `RelayValidatedHandoverEligibility`) → **`CursorManualRelayBridge`** → rendered governed handover → **manual** human copy → Cursor. Optional return path: thin `EngineeringResultImport` paste parsed as untrusted input.
- **Format:** Reuses T5 `projectconcord-relay-v1` via `GovernedRelayV1Renderer` / `GovernedRelayV1Importer`. No `projectconcord-cursor-v1` or alternate governance envelope.
- **Export kind:** Eligible handovers project to `GovernedPackageKind.CursorHandoverExport`, then substantive re-validation with `SoftwareDevelopmentRelayProfileValidator.Instance`, then render.
- **Capabilities:** `SupportsManualPaste = true`, `SupportsAutomatedTransport = false`.
- **Authorization:** Bridge does not create implementation authority; consumes T4/T5 Software Development governance boundary.
- **Provenance:** T3 `IGovernedInteractionRelayService` remains canonical; no T6-specific provenance types or full Cursor transcript storage.
- **Automation:** No clipboard, IDE, MCP/ACP, or shell automation.

## Active STOP (PA decision — 2026-09-30)

| Concept | Meaning |
|---------|---------|
| **Valid** | Package is structurally/governantly valid at the relay boundary |
| **STOP Active** | Progression/execution is presently prohibited |

A package may be **Valid** and still **not** be prepared for Cursor handover when `RelayStopState.Active` is set.

- Active STOP **blocks** `IsReadyForManualTransfer` via diagnostic `relay.cursor.handover.stop_active`.
- STOP does **not** reclassify package validation as **`Incomplete`** or **`RejectedMalformed`** at the bridge outcome; **`Valid != actionable while STOP is active`**.
- STOP does not create or revoke underlying DWA; it blocks progression while active.

## Engineering result return path

`TryParseEngineeringResult` accepts thin pasted evidence when machine block declares `EngineeringResultImport`. Wrong kind → explicit `relay.cursor.engineering_result.kind.mismatch`. Result/evidence import is not implementation authorization.

## Test evidence (PA accepted)

| Suite | Result |
|-------|--------|
| Focused T6 (`FullyQualifiedName~CursorManualRelayBridge`) | See T6 closeout commit — 17 passed at acceptance |
| T5 regression (`ProjectArchitectManualAdapter`) | 22 passed |
| T4 regression (`FullyQualifiedName~SoftwareDevelopment`) | 20 passed |
| `dotnet build -c Release` | 0 errors (pre-existing NU1903 on `SQLitePCLRaw.lib.e_sqlite3`) |
| Full `dotnet test -c Release` | See T6 closeout commit |

Command: `dotnet test -c Release tests/Edf.Application.Tests/Edf.Application.Tests.csproj --filter "FullyQualifiedName~CursorManualRelayBridge"`

## Explicit non-implementation (T6)

No T7 Desktop relay workflow UI, no Migration003, no automated Cursor transport, no MVR/T8, no A3/A4.

## Authorization boundary

**A2-T7 through A2-T8:** NOT AUTHORIZED. **A3 / A4:** NOT AUTHORIZED. **A2 overall:** not complete.

**Next governance decision:** Whether to authorize **A2-T7 only** (Desktop P0 relay workflow). **Do not** infer T8 authorization.
