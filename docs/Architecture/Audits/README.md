[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › Audits

# Architectural Audits

## Purpose

One **Architectural Audit Record (AAR)** Markdown file per **implementation conformance review**, per EDF [AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md) and [ADR-0012](../ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md).

AARs record how code or reference implementation aligns with authoritative ADRs and normative specifications — gaps, violations, and conformant areas. They are **not** normative requirements themselves.

## Distinction from other reviews

| Artifact | Evaluates |
|---|---|
| AAR (this folder) | **Implementation** vs architectural **requirements** |
| [EGR — Gate Reviews](../../Program/Gate_Reviews/) | Human **program gate** approval over authoritative documents |
| Framework Advisor reports | Automated **documentation** scoring under `reports/conformance/` |
| Operational audit (SPEC-003 / ADR-0009) | Runtime collaboration and integrity **events** in operational stores — not AAR |

## Index

| Audit ID | File | Status |
|---|---|---|
| AAR-0001 | [AAR-0001-m1-solution-skeleton-conformance.md](AAR-0001-m1-solution-skeleton-conformance.md) | **Complete** — Project Architect **accepted** (M1b-B publication authorized) |

Add rows when AAR files are created. First audit is required **Complete** before [EGR-G1](../../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md) **Gate satisfied**.

Use [Architectural_Audit_Record_Template.md](../../Templates/Architectural_Audit_Record_Template.md) when creating new AAR files.

## Parent

- [Architecture](../README.md)

## Related Documents

- [ADR-0012 — Adopt EDF Architectural Audit Records](../ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
- [EDF AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md)
