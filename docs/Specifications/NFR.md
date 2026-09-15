[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Specifications](README.md) › NFR

# Non-Functional Requirements

> **Status:** Draft  
> **Owner:** ProjectConcord  
> **Last Reviewed:** 2026-09-15

## Purpose

Non-functional requirements for the EDF Project Management System MVP and architectural boundaries.

## Platform and Technology

| ID | Requirement |
|---|---|
| NFR-001 | Desktop client SHALL target Windows, macOS, and Linux via Avalonia. |
| NFR-002 | Core EDF logic SHALL NOT depend on Avalonia or other UI framework. |
| NFR-003 | Implementation language SHALL be C# on modern .NET. |

## Architecture Boundaries

| ID | Requirement |
|---|---|
| NFR-010 | Canonical engineering state SHALL remain in the Git repository (EDF artifacts). |
| NFR-011 | Derived indexes, caches, UI state, and **shared operational data** (membership, audit, change-set metadata) MAY use local or shared storage and SHALL NOT replace canonical EDF state in Git ([ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)). |
| NFR-012 | The system SHALL NOT silently invent EDF policy; gaps SHALL be logged ([EDF Gap Register](../Development/EDF_Gap_Register.md)). |
| NFR-013 | Code changes SHALL NOT automatically overwrite accepted specifications or ADRs. |

## Performance and Scale (MVP)

| ID | Requirement |
|---|---|
| NFR-020 | Open and scan a medium EDF repo (&lt; 2000 Markdown files) within 30 seconds on developer hardware (target). |
| NFR-021 | Conformance validation MAY block UI with progress; full run acceptable for MVP. |

## Security (phased)

| ID | Requirement |
|---|---|
| NFR-030 | **M1–M5:** Client MAY run locally with a degenerate single-member project (default Administrator). Network authentication and shared operational services are not required for SPEC-001 acceptance. |
| NFR-032 | **Platform:** Multiple authenticated users on shared projects SHALL be supported by application/project services; authorization SHALL be enforced before canonical writes when shared deployment is enabled. |
| NFR-031 | External process invocation (EDF scripts) SHALL use validated paths and fixed script names. |

## Maintainability

| ID | Requirement |
|---|---|
| NFR-040 | EDF validation integration SHALL prefer invoking official EDF scripts before duplicating rules ([ADR-0003](../Architecture/ADRs/ADR-0003-EDF-Validation-Strategy.md)). |
| NFR-041 | Automated tests SHALL cover Engine and validation parsing. |

## Future Web Readiness

| ID | Requirement |
|---|---|
| NFR-050 | Core assemblies SHALL be consumable from a future ASP.NET Core host without rewrite of EDF Engine. |
| NFR-051 | Domain model for projects, membership, and roles SHALL NOT assume a single global user ([ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [ADR-0010](../Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)). |

## Parent

- [Specifications](README.md)

## Related Documents

- [SPEC-001](features/SPEC-001-mvp-edf-desktop-client.md)
- [System Architecture Overview](../Architecture/System_Architecture_Overview.md)
