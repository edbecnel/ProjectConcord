# ADR-0018: Adopter Terminology Policy and Projection Architecture

## Status

Accepted

## Date

2026-10-01

## Context

The Engineering Documentation Framework published terminology governance ([EDF ADR-0011 — Terminology Governance](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Architecture/ADRs/ADR-0011-Terminology-Governance.md), [EDF TGR-0001 v1.1](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Specifications/TGR-0001-Terminology-Governance.md)) and a **Recommended** label for glossary term reference `super-intelligence` ([EDF Glossary](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/32fa70b0b9944b217c70c6d0f6316c76d0891552/docs/Reference/Glossary.md), pin documented in [EDF Terminology Governance handover](../../Handover/EDF-Terminology-Governance-Architecture-Handover.md)).

[AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) captured operator-experience requirements including human-readable terminology. Architectural investigation concluded that terminology must separate **stable concept identity** from **presentation labels**, consume EDF metadata without silent inheritance, and defer enforcement.

**Naming collision:** This record is **ProjectConcord ADR-0018**. [ProjectConcord ADR-0011](ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md) is Canonical Artifact Integrity. [EDF ADR-0011 — Terminology Governance](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Architecture/ADRs/ADR-0011-Terminology-Governance.md) is a separate EDF record — always qualify the repository when citing either ADR-0011.

## Decision

### 1. Layered terminology authority (four layers — MUST NOT collapse)

| Layer | Name | Role | SI status (2026-10-01) |
|---|---|---|---|
| **A** | Source recommendation | EDF (or other publishing glossary) terminology metadata | EDF **Recommended** Super Intelligence (SI) for `super-intelligence` |
| **B** | Local policy | ProjectConcord adopter terminology policy via governance | **Preferred** Super Intelligence (SI) in declared PC semantic scope |
| **C** | Presentation | Derived human-facing labels per surface/locale | Architecture defined; **implementation not authorized** |
| **D** | Enforcement | Require/forbid labels in scoped contexts | **None / not authorized** |

**Provenance rule:** EDF **recommends** → ProjectConcord **explicitly adopts** as **Preferred** → **does not enforce** until a separate PA authorization elevates layer D.

Silent inference `EDF Recommended ⇒ ProjectConcord Preferred` is **forbidden** for future recommendations.

### 2. ProjectConcord Preferred terminology — `super-intelligence`

ProjectConcord adopts **Super Intelligence (SI)** as **Preferred** terminology for the EDF governed concept:

- **Glossary term reference:** `super-intelligence`
- **Qualified reference:** `docs/Reference/Glossary.md#super-intelligence` on the EDF repository at pin `32fa70b0b9944b217c70c6d0f6316c76d0891552` (see handover)

**Semantic scope (aligned with EDF recommendation):** Preferred SI applies when ProjectConcord living documentation or future presentation refers to the **governed technology concept** of machine-supported reasoning, generation, automation, and related computational capabilities in engineering/systems contexts, within EDF’s declared **overlapping** (not universal) relationship to Artificial Intelligence / AI.

**Explicit non-goals of this adoption:**

- EDF does **not** mandate SI for ProjectConcord conformance.
- ProjectConcord does **not** assert universal **SI == AI**.
- This adoption does **not** authorize bulk replacement, identifier renames, or historical rewrites.

### 3. Separate concepts and protected terminology

| Concept | EDF term reference | ProjectConcord rule |
|---|---|---|
| **AI-assisted engineering** | `ai-assisted-engineering` | **Separate** concept — **not** renamed to SI-assisted engineering |
| **super-intelligence** | `super-intelligence` | PC **Preferred** SI per §2 |

**Protected without SI substitution:** historical artifact wording; external/provider/API/standards terminology; stable identifiers (for example `ADR-0006-AI-Boundary`, `SPEC-004-ai-assisted-...`, `docs/AI/` paths, PC-AIGOV IDs, schema keys, source symbols); [ADR-0014](ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) **Proposed** text as written.

### 4. Ownership

| Concern | Owner |
|---|---|
| Terminology governance rules | **EDF** (TGR-0001) |
| Terminology **recommendations** | **EDF glossary** (and other publishing glossaries) |
| **Adopter terminology policy** | **ProjectConcord governance** (this ADR, [ProjectConcord Glossary](../../Reference/Glossary.md)) |
| Generic **terminology projection** capability | **Core / product shell (A)** per [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) |
| Domain-specific labels | **Domain extensions (B, …)** — reference EDF term refs + PC policy |
| Provider/external labels | **Adapters (E)** — **External** disposition; not overridden by PC Preferred |
| Search aliases, resolved strings, render caches | **Derived** infrastructure ([ADR-0004](ADR-0004-Derived-Data-and-Cache.md)) |

Core **MUST NOT** hard-code AI/SI special cases when a generic projection keyed by glossary term reference suffices.

### 5. Consumption model (no unnecessary persistence)

1. **Pin** EDF baselines in [EDF Terminology Governance handover](../../Handover/EDF-Terminology-Governance-Architecture-Handover.md).
2. **Record** ProjectConcord policy elevation in this ADR and ProjectConcord Glossary.
3. **Project** presentation at use time from EDF metadata + PC policy overlay (future `src/` — not authorized).
4. **Optional derived index** for search aliases — rebuildable, non-canonical.

Policy lives in **governance artifacts**, not in operational SQLite.

### 6. Historical preservation and living documents

- **Provenance-sensitive** records (accepted ADRs, AARs, MVRs, EGRs, closeout evidence, handovers) **MUST NOT** be rewritten solely for SI modernization (TGR-0001 §25).
- **Living** documentation migration **MAY** occur only under separately authorized, reviewed tranches — not by this ADR.

### 7. Search and discovery (architecture only)

Future discovery **SHOULD** index by **glossary term reference** with scoped aliases (SI, Super Intelligence, AI, Artificial Intelligence where equivalent in scope). Token **AI** **MUST NOT** be treated as universally synonymous with `super-intelligence` (false positives with `ai-assisted-engineering`, identifiers, and unrelated prose).

### 8. Relationship to operator experience

Human-facing terminology projection supports [AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) (for example **Project Architect** rather than **PA** in UI). Terminology architecture is independent of governed package/context-transfer architecture documented in the same watch item; both are **not implemented** in `src/` by this ADR.

## Alternatives Considered

### Duplicate full EDF terminology registry inside ProjectConcord

- Advantages: Offline resolution.
- Disadvantages: Drift from EDF; violates consumption-over-duplication principle.
- Reason not selected: TGR-0001 minimum contract + pins suffice for v1.

### Bulk AI → SI migration at policy adoption

- Advantages: Immediate label consistency.
- Disadvantages: Violates historical preservation; conflates concepts and identifiers.
- Reason not selected: Rejected by Project Architect disposition.

## Consequences

### Positive

- Clear EDF → ProjectConcord → presentation → enforcement separation.
- SI adoption recorded without falsifying history or renaming identifiers.
- Generic projection path for future labels.

### Negative

- Multiple parallel vocabularies (identifiers, EDF Recommended, PC Preferred) until presentation layer exists.
- Documentation readers must understand layered provenance.

### Risks

- Implementers may hard-code string replacement — mitigated by term-reference projection requirement.
- Confusion between ProjectConcord ADR-0011 and EDF ADR-0011 — mitigated by qualified citations.

## References

- [EDF Terminology Governance handover](../../Handover/EDF-Terminology-Governance-Architecture-Handover.md)
- [ProjectConcord Glossary](../../Reference/Glossary.md)
- [AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
- [ADR-0016](ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)
- [ADR-0004](ADR-0004-Derived-Data-and-Cache.md)
- [GAP-047](../../Development/EDF_Gap_Register.md#gap-047--terminology-projection-and-optional-enforcement)

**Acceptance does not authorize:** terminology enforcement; living-document SI migration; terminology projection implementation; UI changes; bulk terminology replacement.
