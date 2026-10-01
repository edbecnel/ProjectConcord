# Architecture Decisions

[Home](README.md) › [Project Index](PROJECT_INDEX.md) › Architecture Decisions

## Purpose

This document indexes significant architecture decisions for ProjectConcord.

## ADR Location

Individual ADRs live in [docs/Architecture/ADRs/](docs/Architecture/ADRs/README.md).

## Decision Index

| ID | Decision | Status | Date |
|---|---|---|---|
| [ADR-0001](docs/Architecture/ADRs/ADR-0001-Layered-Architecture-and-Avalonia-Client.md) | Layered architecture; Avalonia as first client | Accepted | 2026-09-15 |
| [ADR-0002](docs/Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md) | EDF repository remains canonical; no proprietary engineering DB | Accepted | 2026-09-15 |
| [ADR-0003](docs/Architecture/ADRs/ADR-0003-EDF-Validation-Strategy.md) | Invoke EDF scripts; supplement with in-process rules | Accepted | 2026-09-15 |
| [ADR-0004](docs/Architecture/ADRs/ADR-0004-Derived-Data-and-Cache.md) | Derived caches under `.projectconcord/`; invalidation rules | Accepted | 2026-09-15 |
| [ADR-0005](docs/Architecture/ADRs/ADR-0005-Repository-Abstraction.md) | Filesystem repository abstraction; EDF detection heuristics | Accepted | 2026-09-15 |
| [ADR-0006](docs/Architecture/ADRs/ADR-0006-AI-Boundary.md) | AI proposals only; human approval for canonical writes | Accepted | 2026-09-15 |
| [ADR-0007](docs/Architecture/ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md) | Semantic artifact identity; derived registry; referential integrity | Accepted | 2026-09-15 |
| [ADR-0008](docs/Architecture/ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md) | CRA alignment; no mandatory CKES runtime for MVP | Accepted | 2026-09-15 |
| [ADR-0009](docs/Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) | Multi-user platform; shared project services; canonical vs operational data | Accepted | 2026-09-15 |
| [ADR-0010](docs/Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md) | Default Administrator; no parallel single-user architecture | Accepted | 2026-09-15 |
| [ADR-0011](docs/Architecture/ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md) | Canonical artifact integrity and trusted state | Accepted | 2026-09-15 |
| [ADR-0012](docs/Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md) | Adopt EDF Architectural Audit Records (AAR) | Accepted | 2026-09-17 |
| [ADR-0013](docs/Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) | Software Development governed workflow and cross-project governance (B-layer) | Accepted | 2026-09-29 |
| [ADR-0014](docs/Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) | MVR human attestation and AI boundary | Proposed | 2026-09-28 |
| [ADR-0015](docs/Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) | Project identity, Project Root, per-user operational state (SQLite direction) | Accepted | 2026-09-29 |
| [ADR-0016](docs/Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) | Core, domain extension, and working environment boundary (Stage 1) | Accepted | 2026-09-29 |
| [ADR-0017](docs/Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) | Project Work Record Core boundary (Stage 2) | Accepted | 2026-09-29 |
| [ADR-0018](docs/Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) | Adopter terminology policy and projection architecture | Accepted | 2026-10-01 |
| [ADR-0019](docs/Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) | Local-first operational persistence, service boundary, synchronization, and concurrency | Accepted | 2026-10-01 |
| [ADR-0020](docs/Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) | Operator projections, product shell, and workspace navigation (AWI-0008 A+C) | Accepted | 2026-10-01 |
| [ADR-0021](docs/Architecture/ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) | Engineering Agent provider plugin contract (A4 / GAP-030 / AWI-0006) | Accepted | 2026-10-01 |
| [ADR-0022](docs/Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) | Engineering Agent automated transport architecture (GAP-044 / AWI-0006) | Accepted | 2026-10-01 |

## Related Documents

- [Project Index](PROJECT_INDEX.md)
- [System Architecture Overview](docs/Architecture/System_Architecture_Overview.md)
- [Architecture](docs/Architecture/README.md)
