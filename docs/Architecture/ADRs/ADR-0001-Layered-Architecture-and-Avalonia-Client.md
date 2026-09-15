# ADR-0001: Layered Architecture and Avalonia Client

## Status

Proposed

## Date

2026-09-15

## Context

ProjectConcord must ship a desktop client quickly while preserving shared application services for additional clients ([ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)). UI logic must not entangle with EDF interpretation.

## Decision

Adopt a **layered architecture**:

1. **Edf.* core assemblies** — UI-agnostic domain, engine, validation, authoring, repository, Git.
2. **Edf.Application** — use-case orchestration shared by clients.
3. **Edf.Desktop** — Avalonia UI as the **first client only**.

Future web UI will call the same application layer via HTTP without rewriting EDF Engine rules. Multi-user semantics are defined at the application/project-service layer, not introduced by the web client alone.

## Alternatives Considered

### Monolithic Avalonia app

- Advantages: Simpler initial repo.
- Disadvantages: High cost to add web; untestable core.
- Reason not selected: Conflicts with long-term product vision.

### Web-first (Blazor)

- Advantages: Single UI stack online.
- Disadvantages: Delays achievable desktop MVP; conflates multi-user platform with web deployment.
- Reason not selected: PCON-0000 prioritizes desktop first.

## Consequences

### Positive

- Clear test boundaries; reusable core for CLI/CI later.

### Negative

- More projects to maintain in solution.

### Risks

- Over-fragmentation — mitigate by merging assemblies until boundaries hurt.

## References

- [System Architecture Overview](../System_Architecture_Overview.md)
- [PCON-0000](../PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)
- [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)
