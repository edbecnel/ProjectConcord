# ProjectConcord A2-T2 — Implementation Notes

**Tranche:** A2-T2 — Tier-0 snapshot capability  
**Implementation baseline:** `05cdd85568408d6e71052d09b397c994b9004f18`  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED** (2026-09-30)

## Scope delivered

| Area | Location |
|------|----------|
| Application port | `Edf.Application/Relay/ITier0RelaySnapshotProvider.cs`, `Tier0RelaySnapshotProvider.cs` |
| Infrastructure capture | `Edf.ProjectServices/Relay/*` |
| Unit/integration tests | `tests/Edf.ProjectServices.Tests/Relay/Tier0RelaySnapshotProviderTests.cs` |

Reuses accepted T1 `Tier0RelaySnapshot` without envelope/validation redesign.

## Duplicate `Tier0RelaySnapshotProvider` type name (PA accepted)

| Type | Role |
|------|------|
| `Edf.Application.Relay.Tier0RelaySnapshotProvider` | Application port adapter; existing dependency/delegation pattern (like SQLite persistence adapters) |
| `Edf.ProjectServices.Relay.Tier0RelaySnapshotProvider` | Infrastructure read-only capture |

Namespaces distinguish the two types. No rename during T2 closeout. Future composition may revisit naming only if the distinction becomes materially ambiguous.

`IGitHeadCommitResolver` remains a ProjectServices infrastructure/testability seam; it is not promoted to Application.

## Tier-0 semantic boundary

Shallow observational context only (Git HEAD when available, fixed known paths, authorized narrow metadata). Not EDF conformance, architectural acceptance, authorization, gate satisfaction, profile semantics, or document correctness. Missing observed values remain absent (no governance inference).

## Known-path set (fixed)

| Relative path |
|---------------|
| `ARCHITECTURE_DECISIONS.md` |
| `PROJECT_INDEX.md` |
| `docs/Program/Gate_Reviews/` |

`KnownPathsPresent` lists only paths that exist at capture time (file or directory). No expansion during T2 closeout.

## Narrow metadata (single field — PA accepted)

| Key | Source file | Extraction |
|-----|-------------|------------|
| `docs/Development/Implementation_Roadmap.md:Status` | `docs/Development/Implementation_Roadmap.md` | First `> **Status:** {value}` within first 64 lines |

No additional metadata fields in T2. Not Markdown/EDF parsing, discovery, profile resolution, or conformance interpretation.

## Git HEAD behavior

- When `.git` exists and `git rev-parse HEAD` succeeds: `GitHeadCommit` is the trimmed SHA.
- Non-Git roots: valid snapshot, `GitHeadCommit` null.
- Git missing or command failure: `GitHeadCommit` null; snapshot still returned.

Read-only: no checkout, fetch, pull, commit, reset, stage, or other Git mutation.

## Read-only / non-mutation

No repository file writes, no `.projectconcord/` creation, no arbitrary traversal, no relay persistence.

## Explicit non-implementation (T2)

No Migration002, relay persistence, `IGovernedInteractionRelayService`, PA/Cursor providers, serialization, Desktop relay UI, MVR, A3, or A4. No `Edf.Engine` Tier-0 code.

## Test evidence (PA accepted)

| Suite | Result |
|-------|--------|
| Focused Tier-0 (`FullyQualifiedName~Tier0Relay`) | **12 passed**, 0 failed |
| `Edf.Application.Tests` (Release) | 30 passed |
| `Edf.ProjectServices.Tests` (Release) | 25 passed |
| `Edf.Desktop.Tests` (Release) | 4 passed |
| **Total** | **59 passed** |
| `dotnet build -c Release` | 0 errors (pre-existing NU1903 — not a T2 blocker) |

Command: `dotnet test -c Release tests/Edf.ProjectServices.Tests/Edf.ProjectServices.Tests.csproj --filter "FullyQualifiedName~Tier0Relay"`

Disposable temp directories/repos only.

## Authorization boundary

**A2-T3 through A2-T8:** NOT AUTHORIZED. **A3 / A4:** NOT AUTHORIZED.
