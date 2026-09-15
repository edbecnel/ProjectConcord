# ADR-0004: Derived Data and Cache

## Status

Proposed

## Date

2026-09-15

## Context

Semantic graphs, search indexes, and parsed conformance reports are derived from canonical repo content (PCON-0000 §54.2). EDF does not specify cache location (GAP-012).

## Decision

1. Store derived data under **`.projectconcord/`** in the opened project (gitignored by default) and/or OS application data for global settings.
2. **Never commit** derived caches as canonical artifacts unless user explicitly exports a report to `reports/`.
3. **Invalidate** caches on: project root change, Git HEAD change affecting tracked docs, explicit user refresh, successful canonical authoring save.
4. UI must label derived vs canonical sources.

## Alternatives Considered

### Commit derived index to repo

- Advantages: Faster clone for collaborators.
- Disadvantages: Noise; merge conflicts; blurs canon.
- Reason not selected: Conflicts with ADR-0002.

## Consequences

### Positive

- Clean Git history; clear semantics.

### Negative

- Cold-start rebuild time after invalidation.

## References

- [System Architecture Overview](../System_Architecture_Overview.md)
