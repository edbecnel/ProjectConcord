[Home](README.md) › [Project Index](PROJECT_INDEX.md) › Project Charter

# Project Charter

> **Status:** Draft  
> **Owner:** ProjectConcord  
> **Last Reviewed:** 2026-09-15

## Mission

Build an **EDF Project Management System** that makes the [Engineering Documentation Framework](https://github.com/edbecnel/Engineering-Documentation-Framework) visible, navigable, measurable, editable, actionable, and machine-interpretable for engineers working in Git-native repositories — without replacing EDF as the canonical source of engineering knowledge.

## Goals

- Provide a cross-platform **desktop application** (C# / .NET / Avalonia) as the **first client** for operating EDF-managed projects.
- Implement a **reusable EDF Engine** that discovers projects, interprets profiles/capabilities, validates conformance, and exposes a semantic project model.
- Support **canonical EDF authoring** with validation and safe persistence to prescribed locations.
- Enable **semantic navigation** across artifacts, relationships, and project status.
- Establish a **multi-user platform from the architectural foundation** (membership, roles, shared project services, operational persistence) while delivering an achievable desktop MVP ([ADR-0009](docs/Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)).
- Ensure **solo engineers** default to **Administrator** with full project access without parallel single-user architecture ([ADR-0010](docs/Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)).
- Add a future **web client** as an additional consumer of the same application services — not as the first multi-user release.
- Feed **EDF improvements** back when deterministic interpretation requires clearer specifications.

## Non-Goals (initial releases)

The product is not Jira, GitHub, a full IDE, a universal Markdown editor, a general PM platform, a full AI coding agent, or a mandatory CRA/CKES implementation (see [SPEC-001](docs/Specifications/features/SPEC-001-mvp-edf-desktop-client.md)).

## Success Criterion (major milestone)

A user opens an existing EDF repository in the desktop app and, without manual project configuration, can understand project identity, EDF profile, major artifacts, conformance status, relationships, and recent changes — then create and validate at least one canonical artifact through structured authoring (PCON-0000 §59).

## Scope

### In Scope

- EDF repository discovery and profile interpretation
- Conformance validation via EDF tooling integration
- Semantic navigation and relationship-aware browsing
- Canonical artifact CRUD for selected types (MVP: specifications)
- Git change awareness and foundation for documentation reconciliation
- Optional AI-assisted drafting with human approval (post-MVP foundation)

### Out of Scope

- Enterprise identity federation, full distributed microservices, and real-time collaborative editing in the initial MVP ([SPEC-001](docs/Specifications/features/SPEC-001-mvp-edf-desktop-client.md))
- Full Agile/work-management layer as canonical EDF
- Proprietary database as **source of canonical engineering state** (operational/collaboration stores are permitted per [ADR-0009](docs/Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md))

## Stakeholders

| Role | Interest |
|---|---|
| Project owner / architect | Architecture, EDF alignment, roadmap |
| Solo / team engineer | Daily EDF authoring and validation; optional collaboration on shared projects |
| EDF maintainers | Gap feedback and reference implementation patterns |

## Constraints

- **EDF remains canonical** — repository Markdown/YAML and Git are source of truth ([ADR-0002](docs/Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)).
- **Technology** — modern .NET; Avalonia for desktop UI; core logic UI-agnostic.
- **Platforms** — Windows, macOS, Linux (desktop targets).
- **Dependencies** — local clone of EDF for script-based validation until in-process parity exists.

## Assumptions

- Users maintain projects that adopt or align with EDF structure (`docs/`, adoption YAML).
- A local EDF framework clone is available for validation scripts.
- Single-user desktop deployment suffices for MVP.

## Related Documents

- [Project Index](PROJECT_INDEX.md)
- [PCON-0000](docs/Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)
- [System Architecture Overview](docs/Architecture/System_Architecture_Overview.md)
- [SPEC-001](docs/Specifications/features/SPEC-001-mvp-edf-desktop-client.md)
- [NFR](docs/Specifications/NFR.md)
