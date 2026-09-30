# Glossary

[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Reference](README.md) › Glossary

## Purpose

ProjectConcord **adopter terminology policy** and product-specific governed concepts. EDF framework terminology remains authoritative in the [Engineering Documentation Framework](https://github.com/edbecnel/Engineering-Documentation-Framework); ProjectConcord **consumes** EDF metadata per [EDF Terminology Governance handover](../Handover/EDF-Terminology-Governance-Architecture-Handover.md) and [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md).

## Terminology governance

| Layer | Authority |
|---|---|
| EDF recommendation | [EDF TGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Specifications/TGR-0001-Terminology-Governance.md), [EDF Glossary](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/32fa70b0b9944b217c70c6d0f6316c76d0891552/docs/Reference/Glossary.md) |
| ProjectConcord policy | [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) |
| Presentation / enforcement | Architecture only — **not implemented** / **not authorized** |

## Adopter policy entries

### `super-intelligence` (EDF concept — PC Preferred elevation)

| Field | Value |
|---|---|
| **EDF glossary term reference** | `super-intelligence` |
| **EDF disposition** | **Recommended** — Super Intelligence; **Abbreviation** — SI |
| **ProjectConcord disposition** | **Preferred** — Super Intelligence (SI) in [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) semantic scope |
| **Enforcement** | None / not authorized |
| **Provenance** | EDF recommends → ProjectConcord adopts (Option C, 2026-10-01) |
| **Not equivalent** | `ai-assisted-engineering`; identifier uses of `AI`; universal SI == AI |

### `ai-assisted-engineering` (EDF concept — no PC rename)

| Field | Value |
|---|---|
| **EDF glossary term reference** | `ai-assisted-engineering` |
| **ProjectConcord rule** | **Separate concept** — do not rename to SI-assisted engineering |
| **Protected identifiers** | `SPEC-004-ai-assisted-...`, `docs/AI/`, PC-AIGOV family, [ADR-0006](../Architecture/ADRs/ADR-0006-AI-Boundary.md) filename |

## Parent

- [Reference](README.md)

## Related Documents

- [EDF Terminology Governance handover](../Handover/EDF-Terminology-Governance-Architecture-Handover.md)
- [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md)
