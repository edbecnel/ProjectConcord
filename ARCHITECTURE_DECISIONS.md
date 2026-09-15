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

## Related Documents

- [Project Index](PROJECT_INDEX.md)
- [System Architecture Overview](docs/Architecture/System_Architecture_Overview.md)
- [Architecture](docs/Architecture/README.md)
