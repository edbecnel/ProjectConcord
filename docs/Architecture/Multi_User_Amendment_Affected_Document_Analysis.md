[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › Multi-User Amendment Analysis

# Multi-User and Administrator Model — Affected Document Analysis

> **Status:** Draft — for [EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) re-review  
> **Date:** 2026-09-15  
> **Sources:** [AMD-0001](AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md), [AMD-0002](AMD-0002-Single-User-Administrator-Model.md)

## Purpose

Satisfy AMD-0001 §44–§46: record which documents were amended, key decisions, and open items before major implementation.

## Documents Amended (normative)

| Document | Change summary |
|---|---|
| [System Architecture Overview](System_Architecture_Overview.md) | Multi-user platform section; solution structure; security phasing |
| [PROJECT_CHARTER](../../../PROJECT_CHARTER.md) | Goals, scope, non-goals aligned with desktop-first multi-user |
| [NFR](../Specifications/NFR.md) | Operational store, auth phasing, web readiness |
| [SPEC-001](../Specifications/features/SPEC-001-mvp-edf-desktop-client.md) | MVP scope vs platform foundation |
| [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) | Clarify MVP single-writer vs platform multi-user |
| [Implementation Roadmap](../Development/Implementation_Roadmap.md) | M1 platform seams; M6+ shared services |
| [EDF Gap Register](../Development/EDF_Gap_Register.md) | GAP-018–GAP-021 |
| [ADR-0001](ADRs/ADR-0001-Layered-Architecture-and-Avalonia-Client.md) | Context cross-reference |
| [ARCHITECTURE_DECISIONS](../../../ARCHITECTURE_DECISIONS.md) | ADR-0009, ADR-0010 |

## Documents retained (non-normative rationale)

| Document | Role |
|---|---|
| [AMD-0001](AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md) | Full amendment text and evaluation prompts |
| [AMD-0002](AMD-0002-Single-User-Administrator-Model.md) | Administrator / solo-project requirements |
| [PCON-0000](PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) | Historical discovery; superseded for product model where contradicted by ADR-0009 |

## ADRs introduced

| ID | Topic |
|---|---|
| [ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) | Multi-user platform, shared services, canonical vs operational data |
| [ADR-0010](ADRs/ADR-0010-Single-User-Administrator-Default-Model.md) | Default Administrator; no parallel single-user architecture |

## Selected answers (AMD-0001 §44)

| Question area | Decision |
|---|---|
| Desktop vs multi-user | Desktop-first client; multi-user platform from foundation ([ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)) |
| Web client role | Additional client of same application services; not multi-user enabler |
| Canonical EDF location | Git repository artifacts remain authoritative ([ADR-0002](ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)) |
| Shared database | Operational/collaboration data only; technology choice deferred (architecture-first) |
| UI ↔ database | Clients → Application/Project Services → Engine + operational store |
| Solo user UX | Default Administrator; same architecture ([ADR-0010](ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)) |
| MVP delivery | M1–M5 EDF desktop MVP may use local degenerate membership; interfaces reserved in M1 |

## Open items (implementation / later specs)

- Minimum shared-service topology for first multi-desktop concurrent project (hosting, auth provider).
- Detailed change-set and concurrent Markdown editing protocol.
- Persona catalog and role-to-permission matrix (beyond Administrator).
- Offline desktop sync strategy.
- Operational database technology selection ([GAP-018](../Development/EDF_Gap_Register.md)).

## Gate impact

[EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) MUST be re-reviewed for amended documents and new ADRs before **Gate satisfied** closes M0.

## Parent

- [Architecture](README.md)

## Related Documents

- [CRA Alignment and Responsibility Boundaries](CRA_Alignment_and_Responsibility_Boundaries.md)
