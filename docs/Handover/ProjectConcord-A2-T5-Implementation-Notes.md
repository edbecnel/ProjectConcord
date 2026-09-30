# ProjectConcord A2-T5 — Implementation Notes

**Tranche:** A2-T5 — Manual Project Architect adapter and relay serialization v1  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-30)  
**Implementation baseline:** `b139ff418bdc48b661404c2e00d2c3cbbee9a79c`  
**Publication:** Recorded on `main` at T5 closeout commit (see [A2 plan §31](ProjectConcord-A2-Implementation-Plan.md#31-a2-t5-closeout-2026-09-30)).

## Scope delivered

| Area | Location |
|------|----------|
| Application port | `src/Edf.Application/Relay/ProjectArchitect/IProjectArchitectProvider.cs` |
| Capabilities / import result | `ProjectArchitectProviderCapabilities.cs`, `PaHandoverImportResult.cs` |
| P0 manual adapter | `src/Edf.Application/Relay/ProjectArchitect/ProjectArchitectManualAdapter.cs` |
| Format constants | `src/Edf.Application/Relay/Serialization/GovernedRelayV1Format.cs` |
| Machine envelope DTO / mapper | `GovernedRelayEnvelopeV1Dto.cs`, `GovernedRelayEnvelopeV1Mapper.cs` |
| Human projections | `GovernedRelayGovernanceProjections.cs` |
| Render v1 | `GovernedRelayV1Renderer.cs` |
| Import v1 | `GovernedRelayV1Importer.cs` |
| Validation codes (T5) | `RelayValidationCodes` — render/schema/machine JSON codes |

## Manual relay format (v1) — accepted

1. First line: `ProjectConcord-Relay-Render: 1`
2. Fenced block with language tag **`projectconcord-relay-v1`** containing **plain JSON** of `GovernedRelayEnvelopeV1Dto` (**authoritative**).
3. Markdown projections: `## Governance-Critical`, `## STOP`, optional `## Authorization-Disposition`, `## Work-Context`, `## EDF-Correlation`.
4. Outbound PA reminder (non-governance): Cursor-Mode / transition reminder per A2 plan §12.2.

**No** base64url, opaque binary, or alternate hidden governance encoding.

## Authority and structural agreement

| Rule | Behavior |
|------|----------|
| Machine JSON | Authoritative structured governance representation |
| Human sections | Deterministic projections only — not a second authority |
| Machine/projection disagreement | **`RejectedMalformed`** — never silently prefer machine or prose |
| Structural agreement | Computed on import from parsed representations; **not** a trusted serialized boolean |

Import sequence: render version → machine JSON → human projections → compare → `GovernedRelayStructuralAgreement` → Core + B validation.

## Incomplete vs RejectedMalformed

| State | Meaning |
|-------|---------|
| **Incomplete** | Structurally interpretable, non-contradictory; required governance absent or insufficient |
| **RejectedMalformed** | Structurally unsafe — missing/invalid machine block, unsupported versions, machine/human mode/STOP/auth/work-context mismatch, etc. |

## No prose inference

Free prose (e.g. “approved”, “go ahead”, “STOP acknowledged”) cannot manufacture or alter PA acceptance, DWA, implementation/tranche authorization, STOP, or session intent. Only structured governed fields and required projections count.

## STOP

STOP is represented in machine state and `## STOP` projection; mismatch → **`RejectedMalformed`**. STOP does not create authorization, satisfy DWA, broaden tranche authority, or waive gates.

## T4 Software Development validator composition (accepted)

`ProjectArchitectManualAdapter` uses:

```csharp
new GovernedRelayPackageValidator(SoftwareDevelopmentRelayProfileValidator.Instance)
```

The production Software Development manual PA path **must not** silently use `NullSoftwareDevelopmentRelayProfileValidator` (null remains Core isolation/default only).

T4 semantics preserved: handover ≠ implementation authorization; planning ≠ implementation; PA acceptance ≠ implementation; STOP ≠ implementation; tranche N ≠ tranche N+1.

## Tier-0 / EDF correlation

`Tier0RelaySnapshot` and `EdfGovernanceCorrelation` round-trip in the machine envelope; context/traceability only — no independent authorization, PA acceptance, EDF conformance, or gate satisfaction.

## Schema / render versions

- Render major **1** (`RelayRenderVersion.V1`).
- Schema **1.0** (`RelaySchemaVersion.Current`).
- Unsupported required versions fail explicitly (`relay.render.version.unsupported`, `relay.schema.version.unsupported`).

## PC-PAR-014 eligibility

Only `RelayValidationState.Valid` is eligible for later validated Cursor handover (`RelayValidatedHandoverEligibility`). **Incomplete** and **RejectedMalformed** are not eligible. T5 does not implement Cursor transport.

## Manual P0 workflow

ProjectConcord renders governed package → human copy/paste → ChatGPT Project Architect → human copy/paste response → ProjectConcord parse / structural validation / governance validation. **No** automated ChatGPT API transport.

## Persistence

T3 operational persistence unchanged. **No Migration003.**

## Tests (Release)

| Suite | Result |
|-------|--------|
| `ProjectArchitectManualAdapterTests` | 22 focused T5 cases |
| `SoftwareDevelopmentRelayProfileValidatorTests` | T4 regression (17 cases) |
| Full Release | 116 passed at acceptance (Application 69, ProjectServices 43, Desktop 4) |

Filter examples:

- `dotnet test -c Release --filter "FullyQualifiedName~ProjectArchitectManualAdapter"`
- `dotnet test -c Release --filter "FullyQualifiedName~SoftwareDevelopmentRelayProfileValidator"`

## Explicit non-implementation

**A2-T6** (Cursor P0 manual bridge), **A2-T7** (Desktop relay UI), **A2-T8** (MVR/closeout), **A3**, **A4**, automated Cursor send, OpenAI/API integration, MVR execution. **A2 overall:** not complete.

## Authorization boundary

**A2-T6 through A2-T8:** NOT AUTHORIZED. **A3 / A4:** NOT AUTHORIZED. Next governance decision: whether to authorize **A2-T6 only**.
