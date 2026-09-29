# ProjectConcord A2-T1 — Implementation Notes

**Tranche:** A2-T1 — Core domain + validation  
**Implementation baseline:** `916ba9509d7beded5e57ee94d83be32b073d64bc`  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-29)

## Scope delivered

| Area | Location |
|------|----------|
| Neutral relay envelope and value types | `src/Edf.Domain/Relay/*` |
| Core validation + B-layer validator seam | `src/Edf.Application/Relay/*` |
| Unit tests | `tests/Edf.Application.Tests/Relay/GovernedRelayPackageValidatorTests.cs` |

## Project Architect accepted implementation choices (2026-09-29)

### GovernedRelayStructuralAgreement

Accepted as the neutral structural-validation seam for later serialization/import (A2-T5). T1 validates structural agreement flags/state only; T5 import populates/wires agreement. T1 does not implement renderer/parser.

### RelayGovernanceDirectiveFlags

Accepted. `DirectsImplementationWork` and `DirectsTrancheWork` decide which governance requirements apply. These flags are explicit assembly/import state. The validator does not infer them from prose, package text, likely intent, STOP state, or surrounding workflow context.

### PC-PAR-020 package-kind validation policy

Accepted for T1. PC-PAR-020 governance-critical validation is scoped to handover-oriented package kinds where those fields are semantically required (`PaHandoverImport`, `CursorHandoverExport` in `GovernedRelayPackageValidator`). Not every future `GovernedPackageKind` automatically requires the identical PC-PAR-020 field set. New kinds must be deliberately classified when added; they must not silently bypass governance validation by omission.

## Naming notes (non-binding)

- `EngineeringAgentMode` — neutral engineering-agent mode (provider may render as `Cursor-Mode`).
- `RelayValidatedHandoverEligibility` — PC-PAR-014 gating predicate (A2-T6 consumes).
- `GovernedRelayStructuralAgreement` — machine/projection agreement for structured validation.

## Explicit non-implementation (T1)

No SQLite/Migration002, relay tables, Tier-0 provider, Git/filesystem probing for relay, persistence, Desktop relay UI, `IProjectArchitectProvider`, Cursor bridge, serialization v1 render/parse, clipboard transport, MVR execution, PWR, AuthorityGrant, Working Environment implementation, PCON-0002 resolution, A3, or A4.

## Test evidence (PA accepted)

| Suite | Result |
|-------|--------|
| Focused relay tests (`FullyQualifiedName~Relay`) | **11 passed**, 0 failed |
| `Edf.Application.Tests` (Release) | 30 passed |
| `Edf.ProjectServices.Tests` (Release) | 13 passed |
| `Edf.Desktop.Tests` (Release) | 4 passed |
| **Total** | **47 passed** |
| `dotnet build -c Release` | 0 errors (pre-existing NU1903 on `SQLitePCLRaw.lib.e_sqlite3` — not a T1 blocker) |

Command: `dotnet test -c Release tests/Edf.Application.Tests/Edf.Application.Tests.csproj --filter "FullyQualifiedName~Relay"`

## Authorization boundary

**A2-T2 through A2-T8:** NOT AUTHORIZED. **A3 / A4:** NOT AUTHORIZED.
