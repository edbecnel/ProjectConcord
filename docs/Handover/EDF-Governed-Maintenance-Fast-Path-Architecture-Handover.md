[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › EDF Governed Maintenance Fast Path

# EDF Governed Maintenance Fast Path — ProjectConcord Architecture Handover

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architecture handover (inbound from EDF) |
| **Normative** | No — **EDF owns GMFP semantics**; ProjectConcord implements consumption and projection |
| **Status** | Active |
| **Date** | 2026-09-26 |
| **Owner** | ProjectConcord |
| **Source framework** | [Engineering Documentation Framework](https://github.com/edbecnel/Engineering-Documentation-Framework) |
| **Source commit** | `b158f4a382dfbea941435eeacd96beb729443687` on `main` |

## Canonical authority statement

**EDF is canonical.** ProjectConcord **consumes and projects** GMFP; it **must not** redefine GMFP eligibility, GMR states, escalation semantics, governance law, or maintenance boundaries. When EDF and this handover diverge, **EDF wins**.

**Out of scope for EDF (ProjectConcord’s job):** tool UI, workflow-engine implementation, operational schema, eligibility automation UX, sync strategy details — unless later authorized and aligned with EDF.

---

## 1. Why ProjectConcord needs this

GMFP addresses **disproportionate governance synchronization** for **bounded corrective maintenance** — work that restores already-accepted behavior without changing architecture or product semantics (for example test isolation, narrow regressions, localized edge-case fixes).

ProjectConcord must eventually **recognize**, **visualize**, and **orchestrate projection of** GMFP without forcing maintenance through the full normal development gate topology (separate approvals for every mechanical implement / validate / document / commit step).

GMFP is **optional** governed EDF capability. It does **not** replace normal EDF governance. Automatic **escalation** to normal governance when scope expands is normative in EDF — ProjectConcord must not treat GMFP as a bypass.

---

## 2. Authoritative EDF sources (read first)

| Artifact | Role |
|----------|------|
| [GMFP-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Specifications/GMFP-0001-Governed-Maintenance-Fast-Path.md) | **Normative** requirements, eligibility, workflow, conformance |
| [ADR-0009 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Architecture/ADRs/ADR-0009-Governed-Maintenance-Fast-Path.md) | Architectural decision — **Accepted / Published** at source commit |
| [Governed_Maintenance_Record_Template](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Templates/Governed_Maintenance_Record_Template.md) | Instance shape for **GMR** |
| [Maintenance_Records program domain](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Program/Maintenance_Records/README.md) | Expected location pattern (for example `docs/Program/Maintenance_Records/GMR-NNNN-<short-title>.md`) |
| [Glossary (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Reference/Glossary.md) | GMFP, GMR, related terms |

**Naming collision:** ProjectConcord [ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) is **not** EDF ADR-0009. When citing GMFP rules, refer to **EDF ADR-0009** and **GMFP-0001**.

---

## 3. Workflow topology — stages vs human gates

GMFP uses **three workflow stages** and **two human governance gates**.

| Stage | Role | Human gate? |
|-------|------|-------------|
| **GMFP-1** | Authorization — diagnose, classify, bound, eligibility, validation strategy, **authorize maintenance execution** | **Yes** — Gate 1 |
| **GMFP-2** | Authorized maintenance execution interval — implement, validate, evidence, maintenance documentation, appropriate **local commits** within authorized scope | **No** — not an approval gate |
| **GMFP-3** | Acceptance + publication — review completed work and evidence; authorize push / verify / complete | **Yes** — Gate 2 |

After Gate 2, push and repository verification proceed **without** a routine third architecture-authority gate unless failure, unexpected repository state, or material scope/risk change requires **STOP**.

ProjectConcord must eventually represent **GMFP-2 as an execution interval**, not as a human synchronization point.

---

## 4. Gate 1 — one bounded maintenance authorization (Project Architect binding)

For GMFP, ProjectConcord must **not** require separate human **planning authorization** and **implementation authorization**.

Gate 1 is **one** bounded maintenance authorization encompassing, conceptually:

- defect and reproduction;
- attribution and root cause (or strongly evidenced cause);
- bounded scope and eligibility;
- validation strategy;
- **GMFP IMPLEMENTATION AUTHORIZED** (or equivalent EDF disposition).

That authorization **permits GMFP-2**. It must be representable as a **workflow profile** even when ProjectConcord’s normal development workflow uses additional authorization stages.

**PC-AIGOV-004** (planning authorization shall not imply implementation authorization) applies to the **normal** governed development profile. It must **not** be interpreted to reconstruct intermediate approval gates GMFP intentionally removes. Reconciliation, if needed, is a **future architectural synthesis** item — not an ad hoc SPEC-004 edit in this tranche.

---

## 5. Governed Maintenance Record (GMR)

- **Canonical authority:** EDF **GMR** in the adopting project Git repository (expected under `docs/Program/Maintenance_Records/` per EDF program domain).
- **Purpose:** Consolidate maintenance evidence in one instance artifact rather than multiplying ad hoc documents.
- **States and fields:** Defined by EDF template and GMFP-0001 — ProjectConcord **must not** invent alternative state machines or eligibility rules.

**Operational projection:** ProjectConcord may eventually map canonical GMFP authorization into [DevelopmentWorkAuthorization](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (or successor) as a **profile**.

- Do **not** require **DWA identifiers** embedded in canonical GMRs.
- Do **not** modify EDF GMR semantics from ProjectConcord.
- Any future bidirectional identity/reference relationship requires explicit EDF/ProjectConcord architecture.

---

## 6. GMFP vs EGR, GDO, AAR, and other EDF governance (Project Architect binding)

A successfully completed GMFP maintenance item does **not**, by default:

- satisfy, close, or waive an **EGR**;
- satisfy a **GDO** obligation or treat an Open gate as Satisfied;
- close an **AAR** finding;
- replace an **ADR** or **specification** obligation;
- bypass **Architectural Watch Items** or program gate independent review requirements prescribed elsewhere.

These dimensions remain **distinct**. ProjectConcord UI should represent them separately. A relationship may be shown **only** when canonical artifacts **explicitly** establish one (for example cross-reference in GMR or EGR text).

| Mechanism | Problem GMFP solves | Do not conflate |
|-----------|---------------------|-----------------|
| **EGR** | Program/milestone document gates | GMFP ≠ EGR satisfaction |
| **GDO** | Non-blocking prerequisite while source gate Open | GMFP ≠ Non-Blocking dependency override |
| **AAR** | Implementation vs Accepted ADRs/specs | GMFP ≠ audit closure |
| **DevelopmentWorkAuthorization** | Operational execution authorization | GMR canonical; DWA operational projection |
| **PCR / pause** | Project pause/resume guidance | Separate domain ([PCR-0001](../Development/PCR-0001-Project-Continuation-and-Pause-Record.md)) |

See [GDO handover](EDF-Governed-Dependency-Override-Architecture-Handover.md) for GDO-specific semantics — GMFP is **not** another GDO.

---

## 7. Escalation and STOP

If investigation or implementation discovers scope beyond the authorized maintenance boundary (architecture change, schema, contracts, security/RBAC, new capability, etc.):

- **STOP GMFP** per EDF;
- maintenance record becomes **Escalated** (or equivalent EDF state);
- preserve evidence;
- return to **normal EDF governance**;
- GMFP authorization does **not** authorize expanded work.

ProjectConcord eventual orchestration should align with existing **STOP** semantics in [PCON-0001](../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) and PC-AIGOV-007 — **not implemented in this tranche**.

---

## 8. Workflow profile principle (binding observation)

ProjectConcord **MUST NOT** assume that all EDF-governed work uses one fixed human-gate topology.

Eventually distinguish at least conceptually:

- workflow **classification / profile**;
- workflow / instance **state**;
- **human governance synchronization points**;
- **authorized execution intervals**;
- **canonical governance artifacts** (GMR, EGR, …);
- **operational execution authorization** (DWA, evidence, submissions).

Do **not** implement a provisional `enum WorkflowKind { Normal, GMFP }` during documentation integration. Generic workflow-profile architecture belongs to **future PCON discovery**, then normative [ADR-0013](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) / [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) amendment (deferred).

---

## 9. Relationships ProjectConcord must not confuse

| Artifact / concept | Use for GMFP? |
|--------------------|---------------|
| **GMR + GMFP-0001** | **Yes** — primary canonical maintenance record |
| **EGR** | **No** — program gates; GMFP completion ≠ gate satisfied |
| **GDO** | **No** — prerequisite blocking override on EGR |
| **AAR** | **No** — implementation audit; optional cross-note only per EDF |
| **AWI** | **No** — [AWI-0004](../Architecture/Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md) tracks Concord **consumption gap**, not GMFP instance |
| **DevelopmentWorkAuthorization** | **Operational projection only** — not canonical GMFP law |

---

## 10. Persistence and interchange (future Concord)

EDF today: GMR Markdown under program Maintenance Records. ProjectConcord will likely need discovery, optional internal projection, and integrity-aware authoring ([ADR-0002](../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md), [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)) — **not implemented now**.

Implementation scope tracked in [EDF Gap Register](../Development/EDF_Gap_Register.md) **GAP-041** and watch item [AWI-0004](../Architecture/Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md).

Framework Advisor GMFP/GMR checks evolve in EDF ([Analyzer_Compliance](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Governance/Analyzer_Compliance.md)); Concord may lead UX before full EDF automation.

---

## 11. Anti-patterns (do not build)

- Treating GMFP as skipping all governance or closing Open EGRs
- Requiring a human architecture-authority gate for every commit/doc/validation step **inside** authorized GMFP-2
- Hard-coding GMFP eligibility or GMR states ahead of EDF
- Embedding ProjectConcord DWA IDs into canonical GMR Markdown as if normative
- Modeling GMFP as GDO, EGR waiver, or AAR closure
- Single universal workflow state machine copied from PCON-0001 §11 without workflow profiles

---

## 12. Suggested ProjectConcord work breakdown (future — not authorized now)

1. **Discovery** — Index/parse GMR instances; artifact registry ([SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)).
2. **Integrity** — Governed fields and transitions when EDF defines machine-readable rules ([SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)).
3. **Workflow profile** — Generic abstraction (PCON discovery), then GMFP profile: Gate 1 bounded auth → GMFP-2 interval → Gate 2 acceptance/publication.
4. **Operational projection** — DWA/profile without GMR embedding; escalation to Escalated + STOP.
5. **Visualization** — Two human gates + execution interval; separate from EGR/GDO dashboards.
6. **Evidence** — Consolidated package at Gate 2; publication verification without routine post-push PA gate.
7. **Tests** — EDF GMFP-0001 conformance cases — not Concord-invented rules.

---

## 13. EDF status for Concord planners

- GMFP **Accepted / Published** on EDF at commit `b158f4a382dfbea941435eeacd96beb729443687`.
- ProjectConcord has **no** GMR instances yet; **no** engine/UI implementation.
- This handover is **Active** consumption guidance only.

---

## 14. Decisions before implementation

1. Generic **workflow profile** model (PCON discovery) before GMFP-specific types.
2. Canonical store: GMR Markdown only vs operational DB with projection ([ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) operational default for DWA).
3. How GMFP visualization coexists with EGR/GDO governance debt UX ([GAP-040](../Development/EDF_Gap_Register.md)).
4. Whether manual (clipboard) and integrated AI modes share GMFP semantics ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) M7+).
5. Optional correlation GMR ↔ commits without scope conformance replacing EDF eligibility.

**EDF remains normative for GMFP semantics; ProjectConcord owns execution, UX, and faithful projection.**

---

## 15. Promotion path (documentation)

```text
AWI-0004  ->  PCON discovery (future)  ->  ADR/SPEC amendment (deferred)  ->  implementation (separately governed)
```

Do **not** prematurely amend ADR-0013 or SPEC-004 with a final GMFP workflow profile design.

---

## Parent

- [Handover](README.md)

## Related Documents

- [AWI-0004 — Governed Maintenance Fast Path](../Architecture/Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md)
- [EDF Gap Register](../Development/EDF_Gap_Register.md) — GAP-041
- [GDO handover](EDF-Governed-Dependency-Override-Architecture-Handover.md)
- [Implementation Roadmap](../Development/Implementation_Roadmap.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [ADR-0013](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)
