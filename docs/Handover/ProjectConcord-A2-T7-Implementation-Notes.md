# ProjectConcord A2-T7 — Implementation Notes

**Tranche:** A2-T7 — Desktop P0 governed relay workflow  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-30)  
**Implementation baseline:** `3a3a239f7d0da8941046cc39206d3cd414b84f20`  
**Publication:** See [A2 plan §34](ProjectConcord-A2-Implementation-Plan.md#34-a2-t7-closeout-2026-09-30) (commit SHA recorded at closeout).

## Scope delivered

| Area | Location |
|------|----------|
| P0 workflow orchestration (Application; no UI) | `src/Edf.Application/Relay/IGovernedRelayP0WorkflowService.cs`, `GovernedRelayP0WorkflowService.cs` |
| Desktop composition (shared persistence) | `src/Edf.Application/Composition/ApplicationCompositionRoot.cs` (`DesktopApplicationServices`) |
| Relay workflow VM | `src/Edf.Desktop/ViewModels/RelayWorkflowViewModel.cs` |
| Provenance list item VM | `src/Edf.Desktop/ViewModels/RelayProvenanceItemViewModel.cs` |
| Main window integration | `src/Edf.Desktop/ViewModels/MainWindowViewModel.cs`, `MainWindow.axaml`, `MainWindow.axaml.cs` |
| Focused tests | `tests/Edf.Desktop.Tests/RelayWorkflowViewModelTests.cs`, `tests/Edf.Application.Tests/Relay/GovernedRelayP0WorkflowServiceTests.cs` |

## Canonical UI §16 (A2 plan)

Implemented in Desktop when a project is open:

- Generate PA review package (Application assembly + render + persist)
- Explicit **Copy** for rendered PA review (clipboard adapter only)
- Paste import PA handover (parse + validate + persist consume)
- Validation state + diagnostics (Valid / Incomplete / RejectedMalformed)
- PA + Engineering Agent session NEW/CONTINUE (persisted via T3 continuity)
- Engineering Agent mode + optional prior mode (explicit; not inferred from prose)
- Prepare Engineering Agent handover only when Application reports Valid eligibility
- Explicit **Copy** for rendered Engineering Agent handover
- Optional engineering result paste import
- Minimal provenance list (last N events, PC-PAR-021)

## Application services consumed

- `IGovernedRelayP0WorkflowService` — P0 step orchestration
- `IGovernedInteractionRelayService` (via workflow) — continuity, produce/consume, provenance
- `IProjectArchitectProvider` / `ProjectArchitectManualAdapter` — render + PA import
- `IEngineeringAgentRelayBridge` / `EngineeringAgentManualRelayBridge` — handover prep + result import
- `GovernedRelayPackageValidator` + `SoftwareDevelopmentRelayProfileValidator`
- `ITier0RelaySnapshotProvider` — Tier-0 on PA review export
- `IProjectWorkspaceService` — active project root / project id

Desktop ViewModels do **not** reimplement validation rules or eligibility.

## Provider-neutral Engineering Agent boundary

Production and test C# sources contain **zero** case-insensitive commercial engineering-agent product-name matches (verified via `rg -i cursor` on `src/**/*.cs`, `tests/**/*.cs`, and applicable AXAML/JSON/scripts).

Core, Software Engineering Extension (Application relay/profile), and Desktop use **Engineering Agent** terminology only in normative implementation source.

## Valid-only readiness (PC-PAR-014)

- Engineering Agent handover **Prepare** uses `RelayValidationResult.IsEligibleForValidatedEngineeringAgentHandover` from the imported PA package.
- **Incomplete** and **RejectedMalformed** imports are surfaced as not ready; handover text is not prepared as transferable.
- Active **STOP** on a **Valid** import blocks readiness without reclassifying validity (T6 semantics preserved).

## STOP behavior

- UI presents bridge/orchestrator diagnostics when STOP blocks handover preparation.
- Desktop does not clear STOP, infer acknowledgment, or convert STOP into authorization.

## Manual P0 / clipboard

- **No** automated provider transport, IDE APIs, MCP/ACP, shell invocation, or auto paste/send.
- Clipboard is used only on explicit user **Copy** commands for already-rendered governed text (PA review or Engineering Agent handover).
- Generating or importing packages does **not** auto-copy.

## Rendered contract

Outbound/inbound presentation uses **`projectconcord-relay-v1`** and governed projection labels including `Engineering-Agent-Mode`, `Engineering-Agent-Chat`, `Engineering-Agent-Mode-Transition`, and `ChatGPT-Chat`.

## Authorization boundary

Desktop copy includes an explicit notice: handover ≠ implementation authorization. UI does not grant tranche or implementation authority.

## Provenance / persistence

- Reuses T3 operational store and provenance events only.
- **No Migration003.** No Desktop-specific provenance schema. No canonical chat transcript persistence.

## PC-PAR-021 visibility

Recent provenance events for the active project are listed in the relay panel (event type, package/correlation ids, UTC).

## Engineering Agent plugin requirement (PA 2026-09-30)

Project Architect closeout **identified** (does **not** authorize implementation):

- Multiple concrete Engineering Agent products must be supportable via **separately bounded plugins**.
- Core and Software Engineering Extension source remain **provider-neutral**.
- A provider-neutral Engineering Agent plugin contract/capability boundary is required in the Software Engineering domain.
- Generic plugin hosting infrastructure ownership remains **deferred** (do not conflate with plugin semantics).
- **A4** automated transport must be reconciled against this requirement before A4 implementation authorization.

Recorded in [GAP-030](../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) and [AWI-0006](../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md) (A4 reconciliation). **No** plugin interfaces, loaders, registries, or provider SDK work in T7.

## Tests (Release at acceptance)

| Suite | Notes |
|-------|--------|
| `RelayWorkflowViewModelTests` | 5 focused T7 VM cases |
| `GovernedRelayP0WorkflowServiceTests` | Session gate + PA review export smoke |
| Regression | Engineering Agent bridge, PA adapter, Software Development profile, serialization/validator (full Application relay suite) |
| Full solution | `dotnet test -c Release` — **143 passed** at T7 publication validation |

## T8 MVR dependency

Residual **macOS Avalonia ComboBox/session UX** smoke verification is accepted as a **T8 MVR** concern, not a T7 source defect. T7 does not execute MVR.

## Explicit non-goals

- **A2-T8** MVR execution / A2 closeout attestation — **NOT AUTHORIZED**
- **A3** governed workflow UI
- **A4** automated Engineering Agent transport — **NOT AUTHORIZED**
- Engineering Agent plugin architecture implementation
- AuthorityGrant / PWR / Working Environment persistence

## Authorization boundary (forward)

**A2-T8 / MVR / A3 / A4:** NOT AUTHORIZED.

**Next governance decision:** Whether to authorize **A2-T8 only** (verification, MVR, A2 documentation closeout). **Do not** infer authorization to modify `src/` during T8 without separate PA remediation tranche if defects are found.
