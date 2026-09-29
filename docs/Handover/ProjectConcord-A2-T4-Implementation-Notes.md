# ProjectConcord A2-T4 — Implementation Notes

**Tranche:** A2-T4 — Software Development relay profile + boundary rules  
**Implementation baseline:** `75d894bc19c5e5dbed13f75f98b24dc8cc356358`  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-30)

## Scope delivered

| Area | Location |
|------|----------|
| B-layer profile model | `src/Edf.Application/Relay/SoftwareDevelopment/SoftwareDevelopmentProfilePayload.cs` |
| Profile JSON seam | `SoftwareDevelopmentProfilePayloadSerializer.cs` |
| `ISoftwareDevelopmentRelayProfileValidator` | `SoftwareDevelopmentRelayProfileValidator.cs` |
| Validation codes | `src/Edf.Application/Relay/RelayValidationCodes.cs` (B-specific extensions) |
| Tests | `tests/Edf.Application.Tests/Relay/SoftwareDevelopmentRelayProfileValidatorTests.cs`, profile round-trip in `RelayOperationalPersistenceTests` |

## Profile model (B-owned)

`SoftwareDevelopmentProfilePayload` (version 1 JSON in `GovernedRelayPackage.ProfilePayload` / T3 `profile_payload` BLOB):

- `handoverContext` — inherited context only (`isInheritedContextOnly`); does not grant execution authorization.
- `authorizationDisposition` — structured disposition mirrored from governance-critical presence; not DWA.
- `workContext` — requested tranche/work summary when tranche-directed.
- `developmentWorkAuthorization` — **relay-bound projection** of permitted Software Development execution (planning vs implementation, authorized tranche, scope markers, reference). **Not** the complete canonical DWA domain model or entity store.
- `architecturalReview` — explicit PA disposition / acceptance correlation; does not imply implementation authorization alone.
- `projectWorkRecord` / `humanInitiatedWorkItem` — optional correlation only; flags that treat them as authorization are rejected.
- `authorityGrant` — detected and rejected (deferred generic model).

Generic relay envelope (`Edf.Domain/Relay`) unchanged. Software Development-specific types remain in `Edf.Application/Relay/SoftwareDevelopment/`.

## DWA projection boundary (PA accepted for A2)

`DevelopmentWorkAuthorizationProjection` is **accepted** as the minimum A2 relay projection. It is **not**:

- the full DWA schema or lifecycle ([GAP-027](../../Development/EDF_Gap_Register.md) remains **open / deferred**);
- generic Core authorization, PWR, AuthorityGrant, handover, or architectural acceptance.

## Authorization distinctions (accepted)

| Distinction | Enforced |
|-------------|----------|
| Handover ≠ implementation authorization | Yes |
| Planning ≠ implementation authorization | Yes |
| PA architectural acceptance ≠ implementation authorization | Yes |
| STOP / STOP acknowledgment ≠ implementation authorization | Yes |
| Tranche N ≠ tranche N+1 authorization | Yes |
| Git state ≠ authorization | Yes |
| Tier-0 metadata ≠ authorization | Yes |
| Relay package existence ≠ authorization | Yes |

No prose inference manufactures these relationships.

## Rules implemented (canonical alignment)

| Rule | Behavior |
|------|----------|
| PC-AIGOV-003 | Handover context cannot merge with DWA; handover does not satisfy implementation authorization. |
| PC-AIGOV-004 | Planning DWA projection cannot satisfy implementation-directed packages. |
| PC-AIGOV-007 | STOP acknowledgment cannot create DWA; active STOP blocks implicit implementation authorization without explicit reference. |
| PC-AIGOV-014 | Requested tranche must match authorized tranche on DWA projection. |
| Disposition / work-context alignment | Governance-critical flags must match profile sections. |
| PWR / HIW / AuthorityGrant | Correlation only; conflation rejected; HIW normative naming preserved. |
| Tier-0 | Observational input only; validator does not infer authorization from Git HEAD or roadmap metadata. |

## Null vs substantive validator (binding downstream requirement)

`GovernedRelayPackageValidator` parameterless constructor retains `NullSoftwareDevelopmentRelayProfileValidator` for Core isolation and T1 tests.

**Binding:** Any production Software Development relay composition path **must** inject `SoftwareDevelopmentRelayProfileValidator` and **must not** silently use the null seam. T4 does not implement T5/T6/T7 composition; a later authorized tranche must demonstrate substantive validator wiring.

## Persistence

Uses existing T3 `profile_payload` BLOB only. **No Migration003.**

## Test evidence (PA accepted)

| Suite | Result |
|-------|--------|
| `SoftwareDevelopmentRelayProfileValidatorTests` | **17 passed** |
| `SoftwareDevelopmentProfilePayload_RoundTrips_OnPackage` | **Passed** |
| `Edf.Application.Tests` (Release) | 47 passed |
| `Edf.ProjectServices.Tests` (Release) | 43 passed |
| `Edf.Desktop.Tests` (Release) | 4 passed |
| **Total Release** | **94 passed** |
| `dotnet build -c Release` | 0 errors (pre-existing NU1903 on `SQLitePCLRaw.lib.e_sqlite3`) |

Commands:

- `dotnet test -c Release tests/Edf.Application.Tests --filter "FullyQualifiedName~SoftwareDevelopmentRelayProfileValidator"`
- `dotnet test -c Release tests/Edf.ProjectServices.Tests --filter "FullyQualifiedName~SoftwareDevelopmentProfilePayload_RoundTrips"`
- `dotnet test -c Release`

## Explicit non-implementation

No T5 provider/serialization, T6 Cursor bridge, T7 relay UI, MVR, full DWA entity store, PWR implementation, HIW → OperationalIntakeRecord rename, or A3/A4 work.

## Authorization boundary

**A2-T5 through A2-T8:** NOT AUTHORIZED. **A3 / A4:** NOT AUTHORIZED. **A2 overall:** not complete.
