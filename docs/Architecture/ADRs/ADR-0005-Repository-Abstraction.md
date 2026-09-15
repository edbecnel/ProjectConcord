# ADR-0005: Repository Abstraction

## Status

Accepted

## Date

2026-09-15

## Context

MVP opens local Git working trees. Future web hosting may use server-side clones. EDF project identity is not marked by a single file (GAP-001).

## Decision

1. Introduce **`IProjectRepository`** (or equivalent) abstracting read/write/list/watch on files relative to a **project root**.
2. MVP implementation: **local filesystem** only.
3. **EDF project detection** uses documented heuristics (see EDF Gap Register GAP-001) with explicit confidence in UI — no silent treat-as-EDF for arbitrary folders.
4. **EDF framework path** is application setting pointing to local EDF clone for scripts.

## Alternatives Considered

### Hard-coded project layout checks only

- Advantages: Simple code.
- Disadvantages: Breaks when EDF evolves profiles.
- Reason not selected: Must read adoption YAML and capabilities.

## Consequences

### Positive

- Web adapter can swap filesystem for server clone API later.

### Negative

- Abstraction layer before second implementation (YAGNI risk) — keep interface minimal.

## References

- [EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-001, GAP-002
