# ADR-0006: AI Boundary

## Status

Proposed

## Date

2026-09-15

## Context

PCON-0000 envisions AI-assisted authoring and reconciliation. AI must not silently change canonical engineering intent (§29, §54.7).

## Decision

1. AI services produce **proposals** (draft text, suggested link fixes, reconciliation comments) — not direct canonical writes.
2. **Deterministic validation** (EDF scripts + in-process rules) runs before user accepts a proposal.
3. **Human explicit approval** required to persist changes to canonical repository files.
4. AI provider keys and prompts stored in user settings; no sending entire repo without user-configured scope (economical AI principle).

## Alternatives Considered

### Autonomous AI commit to docs on CI

- Advantages: Speed.
- Disadvantages: Violates human authority over architecture.
- Reason not selected: Explicit PCON-0000 rejection.

## Consequences

### Positive

- Trustworthy EDF operations; audit trail via Git user commits.

### Negative

- Extra confirmation steps in UI.

## References

- [PCON-0000](../PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) §18–20, §29
