[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › EDF Terminology Governance

# EDF Terminology Governance — ProjectConcord Architecture Handover

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architecture handover (inbound from EDF) |
| **Normative** | No — **EDF owns terminology governance semantics**; ProjectConcord implements consumption and records **adopter terminology policy** separately |
| **Status** | Active |
| **Date** | 2026-10-01 |
| **Owner** | ProjectConcord |
| **Source framework** | [Engineering Documentation Framework](https://github.com/edbecnel/Engineering-Documentation-Framework) |

## Canonical authority statement

**EDF is canonical** for terminology governance rules ([TGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Specifications/TGR-0001-Terminology-Governance.md), [EDF ADR-0011 — Terminology Governance](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Architecture/ADRs/ADR-0011-Terminology-Governance.md)). ProjectConcord **consumes** EDF terminology metadata and **must not** redefine TGR semantics or invent a competing USL-wide policy.

**ProjectConcord adoption:** EDF **Recommended** labels become ProjectConcord **Preferred** only through explicit ProjectConcord governance ([ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md)).

**Naming collision:** ProjectConcord [ADR-0011](../Architecture/ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md) (Canonical Artifact Integrity) is **not** [EDF ADR-0011 — Terminology Governance](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Architecture/ADRs/ADR-0011-Terminology-Governance.md). Always qualify repository when citing either record.

---

## 1. Pinned EDF publication baselines

| Subject | Commit on `main` | Notes |
|---|---|---|
| Terminology governance (TGR-0001 v1.1, EDF ADR-0011) | `49d6b805af5f4a557c526a5c8f08e43fbab38601` | Establish EDF terminology governance |
| `super-intelligence` glossary recommendation | `32fa70b0b9944b217c70c6d0f6316c76d0891552` | Recommend Super Intelligence terminology in EDF |

When EDF and this handover diverge on **framework** terminology rules, **EDF at the pinned baseline wins** until ProjectConcord explicitly updates its pin through governed documentation.

---

## 2. Authoritative EDF sources (read first)

| Artifact | Role |
|---|---|
| [EDF TGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Specifications/TGR-0001-Terminology-Governance.md) | **Normative** terminology governance requirements |
| [EDF ADR-0011 — Terminology Governance](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Architecture/ADRs/ADR-0011-Terminology-Governance.md) | Architectural decision — **Accepted** at governance pin |
| [EDF Glossary](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/32fa70b0b9944b217c70c6d0f6316c76d0891552/docs/Reference/Glossary.md) | Framework terms + `super-intelligence` recommendation block |
| [EDF Reference README](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/32fa70b0b9944b217c70c6d0f6316c76d0891552/docs/Reference/README.md) | Glossary authority pointer |

---

## 3. Minimum consumption contract (ProjectConcord)

Per TGR-0001 §34, ProjectConcord SHOULD resolve shared concepts using:

- EDF glossary file path + **glossary term reference** (immutable slug)
- EDF **terminology disposition** (Recommended, Abbreviation, Alias, Legacy, External, and so on)
- Equivalence scope and non-equivalence boundaries
- Provenance that a label is EDF **recommendation** versus ProjectConcord **Preferred** policy

ProjectConcord **need not** duplicate the full EDF glossary. Policy elevation is recorded in [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) and [ProjectConcord Glossary](../Reference/Glossary.md).

---

## 4. Out of scope for this handover

- Terminology **enforcement** (lint, CI, bulk migration) — not authorized
- Terminology **projection** implementation in `src/`
- Modification of EDF repositories from ProjectConcord
- Living-document SI migration — separately governed

---

## Parent

- [Handover](README.md)

## Related Documents

- [ADR-0018 — Adopter terminology policy and projection architecture](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md)
- [AWI-0008 — Governed operator experience](../Architecture/Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
- [ProjectConcord Glossary](../Reference/Glossary.md)
