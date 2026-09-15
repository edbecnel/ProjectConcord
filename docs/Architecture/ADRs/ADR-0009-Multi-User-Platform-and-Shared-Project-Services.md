# ADR-0009: Multi-User Platform and Shared Project Services

## Status

Accepted

## Date

2026-09-15

## Context

[PCON-0000](PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) and early ProjectConcord docs described a single-user desktop first, with multi-user capabilities arriving with a web client. [AMD-0001](../AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md) amends that model.

ProjectConcord must support multiple authenticated users working concurrently on shared projects without replacing EDF Git artifacts as canonical engineering state ([ADR-0002](ADR-0002-EDF-Canonical-Source-of-Truth.md)).

## Decision

1. **Product model** — ProjectConcord is **desktop-first** but **multi-user from its architectural foundation**. The first client remains Avalonia; a future web UI is an **additional client**, not the prerequisite for multi-user operation.

2. **Layering** — Desktop (and future web) clients MUST NOT bind directly to a shared operational database. They call **ProjectConcord application / project services** that orchestrate EDF Engine operations, authorization, membership, and operational persistence.

3. **Data classification**
   - **Canonical EDF engineering state** — Markdown/YAML (and prescribed paths) in the Git repository; authoritative per ADR-0002.
   - **Operational / collaboration state** — identities, membership, roles, notifications, change-set metadata, audit events, derived indexes when shared — MAY live in a **shared operational store** (local or cloud) without becoming canonical EDF.

4. **Concurrency** — Prefer change sets, optimistic concurrency, and conflict detection over crude permanent file locks for canonical artifact editing (detailed behavior deferred to post-MVP specs).

5. **MVP scope boundary** — Multi-user **architecture** is decided now; full shared-service deployment is **incremental**. [SPEC-001](../../Specifications/features/SPEC-001-mvp-edf-desktop-client.md) M1–M5 MAY use a degenerate single-member project (default Administrator per [ADR-0010](ADR-0010-Single-User-Administrator-Default-Model.md)) with local-only wiring, but domain types and service boundaries MUST NOT assume a single global user.

6. **CRA** — User/membership relationships and operational edges MUST NOT be modeled as CRA canonical engineering relationships ([AMD-0001](../AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md) §26; [CRA Alignment](../CRA_Alignment_and_Responsibility_Boundaries.md)).

## Alternatives Considered

### Defer multi-user until web client

- Advantages: Simpler M1–M5 code paths.
- Disadvantages: Forces rewrite of security, membership, and persistence; contradicts collaboration needs on desktop.
- Reason not selected: Superseded by AMD-0001.

### Store all project state in shared database

- Advantages: Single persistence model.
- Disadvantages: Violates EDF canonical Git model; complicates audit and open-source workflows.
- Reason not selected: ADR-0002 stands; operational DB is supplementary.

## Consequences

### Positive

- Desktop and web share one domain and service layer.
- Team collaboration does not require waiting for web UI.

### Negative

- M1 solution structure must reserve assemblies/interfaces for project services and identity even before they are fully deployed.

### Risks

- Over-building distributed infrastructure early — mitigate via roadmap phasing (M1–M5 local degenerate case; shared services M6+).

## References

- [AMD-0001](../AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md)
- [System Architecture Overview](../System_Architecture_Overview.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
- [NFR](../../Specifications/NFR.md)
