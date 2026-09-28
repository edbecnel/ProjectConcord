# AWI-0007-SQLite-Transitive-NuGet-Advisory

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0007

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-28 |
| **Revisit Trigger** | `dotnet list package --vulnerable` reports resolution change; before mandatory dependency remediation gate; PA request |
| **Discovery source** | A1b publication build (`NU1903` on Release) |
| **Cross-reference** | [EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-045 |

---

## Recorded advisory (A1b baseline)

| Field | Value |
|---|---|
| **NuGet warning** | `NU1903` — known high severity vulnerability |
| **Direct package** | `Microsoft.Data.Sqlite` **9.0.3** (`Edf.ProjectServices`) |
| **Vulnerable transitive** | `SQLitePCLRaw.lib.e_sqlite3` **2.1.10** |
| **Severity** | High |
| **Advisory** | [GHSA-2m69-gcr7-jv3q](https://github.com/advisories/GHSA-2m69-gcr7-jv3q) |

## Scope and non-goals

- **Does not** authorize A1b scope expansion or general dependency remediation.
- **Does not** require immediate package upgrades unless a separate PA/security gate mandates it.
- **Does not** move SQLite out of `Edf.ProjectServices`.

## Interim policy

Monitor upstream `Microsoft.Data.Sqlite` / `SQLitePCLRaw` releases for a non-vulnerable transitive chain. Re-evaluate under GDO/EGR process if policy requires remediation before a future milestone gate.

## Parent

- [Watch Items](README.md)

## Related Documents

- [ProjectConcord A1 Implementation Plan](../../Handover/ProjectConcord-A1-Implementation-Plan.md) — A1b SQLite persistence
- [EDF Gap Register](../../Development/EDF_Gap_Register.md) — GAP-045
