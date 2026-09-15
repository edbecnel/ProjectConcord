# ADR-0002: EDF Canonical Source of Truth

## Status

Accepted

## Date

2026-09-15

## Context

The product must operate EDF without replacing it (PCON-0000 §3). Users and other tools must be able to use the repository without ProjectConcord.

## Decision

1. **Canonical engineering state** resides only in EDF-prescribed repository artifacts (Markdown, YAML, code) under Git.
2. The application **MUST NOT** require a proprietary database to interpret canonical engineering state.
3. Derived data (search indexes, relationship graphs, UI layout, cached conformance parses) is **non-canonical** and optional for repository understanding.

## Alternatives Considered

### Central product database as source of truth

- Advantages: Faster queries; easier multi-user sync.
- Disadvantages: Violates EDF principle; lock-in.
- Reason not selected: Explicit non-goal in PCON-0000.

## Consequences

### Positive

- Git-native workflow preserved; EDF remains portable.

### Negative

- Full-graph queries require rebuild from repo.

### Risks

- Users confuse cache with canon — mitigate with UI labeling.

## References

- [System Architecture Overview](../System_Architecture_Overview.md)
