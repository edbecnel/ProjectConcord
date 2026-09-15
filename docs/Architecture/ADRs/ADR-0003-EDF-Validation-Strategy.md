# ADR-0003: EDF Validation Strategy

## Status

Accepted

## Date

2026-09-15

## Context

EDF ships Framework Advisor and conformance scripts (`analyze_project_structure.sh`, `run_conformance_validation.sh`). Reimplementing all rules in C# duplicates EDF maintenance and drifts from authoritative behavior (GAP-010).

## Decision

1. **Primary validation** invokes EDF shell scripts from a user-configured local EDF clone path.
2. ProjectConcord **parses script output** for display and automation.
3. **In-process validation** supplements scripts only where EDF rules are unambiguous (e.g., filename patterns, known config YAML keys) and is versioned with the app.
4. Contribute **JSON output** proposal upstream to EDF; do not fork rule semantics silently.

## Alternatives Considered

### Full in-process Framework Advisor port

- Advantages: No shell dependency; faster on Windows without bash.
- Disadvantages: Duplicate logic; lag behind EDF updates.
- Reason not selected: Premature before M3 learns integration pain.

### Validation-only via manual user running scripts

- Advantages: Zero integration work.
- Disadvantages: Fails product value proposition.
- Reason not selected: Unacceptable for MVP.

## Consequences

### Positive

- Stays aligned with EDF releases user already trusts.

### Negative

- Requires bash-compatible environment or future EDF .NET validator.

## References

- [EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-010
