# ProjectConcord A2-T6 — Implementation Notes

**Tranche:** A2-T6 — Engineering Agent P0 manual bridge
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-30)
**Provider-neutral remediation:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-30)
**Original T6 publication commit:** `f9d0360ad5a93862798f31f5baacf0f6ef6dd26f` (preserved in history; not amended)
**Prior implementation baseline (pre-remediation):** `d051cc43bee0e0d9f25f1e01f6b75399de123ef6`
**Publication:** See [A2 plan §32–§33](ProjectConcord-A2-Implementation-Plan.md#32-a2-t6-closeout-2026-09-30).

## Provider-neutral remediation (2026-09-30)

Project Architect review after T6 publication required **provider-neutral** source architecture. **Engineering Agent** is the architectural role. Production and test source (`src/**/*.cs`, `tests/**/*.cs`) contain **zero** references to any specific commercial engineering-agent product name (case-insensitive audit invariant).

| Area | Canonical state |
|------|-----------------|
| Bridge seam | `IEngineeringAgentRelayBridge` / `EngineeringAgentManualRelayBridge` — `src/Edf.Application/Relay/EngineeringAgent/` |
| Export kind | `GovernedPackageKind.EngineeringAgentHandoverExport` (= **2**) |
| Machine JSON kind | `engineeringAgentHandoverExport` |
| Bridge diagnostics | `relay.engineering_agent.*` |
| Eligibility API | `IsEligibleForValidatedEngineeringAgentHandover` |
| v1 rendered fields | `Engineering-Agent-Mode`, `Engineering-Agent-Chat`, `Engineering-Agent-Mode-Transition` |
| PA outbound reminder | `PaEngineeringAgentReminder` |
| Format fence | `projectconcord-relay-v1` |

Git history for `f9d0360` is **not** rewritten.

## Scope delivered

| Area | Location |
|------|----------|
| `IEngineeringAgentRelayBridge` + P0 manual implementation | `src/Edf.Application/Relay/EngineeringAgent/*` |
| PC-PAR-014 / PC-PAR-022 seam | `EngineeringAgentManualRelayBridge`, `RelayValidatedHandoverEligibility` (T1, consumed) |
| Focused unit tests | `tests/Edf.Application.Tests/Relay/EngineeringAgentManualRelayBridgeTests.cs` |

## Architecture

- **Boundary:** Validated governed relay (`RelayValidationState.Valid` per `RelayValidatedHandoverEligibility`) → **`EngineeringAgentManualRelayBridge`** → rendered governed handover → **manual** human copy to an engineering agent. Optional return path: thin `EngineeringResultImport` paste parsed as untrusted input.
- **Format:** Reuses T5 `projectconcord-relay-v1` via `GovernedRelayV1Renderer` / `GovernedRelayV1Importer`. No alternate provider-specific governance envelope.
- **Export kind:** Eligible handovers project to `GovernedPackageKind.EngineeringAgentHandoverExport`, then substantive re-validation with `SoftwareDevelopmentRelayProfileValidator.Instance`, then render.
- **Capabilities:** `SupportsManualPaste = true`, `SupportsAutomatedTransport = false`.
- **Authorization:** Bridge does not create implementation authority; consumes T4/T5 Software Development governance boundary.
- **Provenance:** T3 `IGovernedInteractionRelayService` remains canonical; no T6-specific provenance types or full engineering-agent transcript storage.
- **Automation:** No clipboard, IDE, MCP/ACP, or shell automation.

## Active STOP (PA decision — 2026-09-30)

| Concept | Meaning |
|---------|---------|
| **Valid** | Package is structurally/governantly valid at the relay boundary |
| **STOP Active** | Progression/execution is presently prohibited |

A package may be **Valid** and still **not** be prepared for engineering-agent handover when `RelayStopState.Active` is set.

- Active STOP **blocks** `IsReadyForManualTransfer` via diagnostic `relay.engineering_agent.handover.stop_active`.
- STOP does **not** reclassify package validation as **`Incomplete`** or **`RejectedMalformed`** at the bridge outcome; **`Valid != actionable while STOP is active`**.
- STOP does not create or revoke underlying DWA; it blocks progression while active.

## Engineering result return path

`TryParseEngineeringResult` accepts thin pasted evidence when machine block declares `EngineeringResultImport`. Wrong kind → explicit `relay.engineering_agent.engineering_result.kind.mismatch`. Result/evidence import is not implementation authorization.

## Test evidence

| Suite | Filter / scope |
|-------|----------------|
| Focused T6 | `FullyQualifiedName~EngineeringAgentManualRelayBridge` |
| T5 regression | `ProjectArchitectManualAdapter` |
| T4 regression | `FullyQualifiedName~SoftwareDevelopment` |

Command: `dotnet test -c Release tests/Edf.Application.Tests/Edf.Application.Tests.csproj --filter "FullyQualifiedName~EngineeringAgentManualRelayBridge"`

## Explicit non-implementation (T6)

No T7 Desktop relay workflow UI, no Migration003, no automated engineering-agent transport, no MVR/T8, no A3/A4.

## Authorization boundary

**A2-T7 through A2-T8:** NOT AUTHORIZED. **A3 / A4:** NOT AUTHORIZED. **A2 overall:** not complete.

**Next governance decision:** Whether to authorize **A2-T7 only** (Desktop P0 relay workflow). **Do not** infer T8 authorization.
