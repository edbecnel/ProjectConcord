[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › EDF Governed Dependency Override

# EDF Governed Dependency Override — ProjectConcord Architecture Handover

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architecture handover (inbound from EDF) |
| **Normative** | No — EDF owns semantics; ProjectConcord implements consumption |
| **Status** | Active |
| **Date** | 2026-09-22 |
| **Owner** | ProjectConcord |
| **Source framework** | [Engineering Documentation Framework](https://github.com/edbecnel/Engineering-Documentation-Framework) |
| **Source commit** | `0016f15` on `main` |

## Purpose

Hand ProjectConcord architects and implementers the **normative semantics** for EDF **Governed Dependency Override (GDO)** so Concord can incorporate the capability into its gate model, dependency evaluation, authorization workflow, visualization, validation, and EGR round-trip — without redefining EDF.

**Subject:** Governed Dependency Override / non-blocking dependency while program gates remain Open.

**Mode:** Architecture and product design (execution is ProjectConcord’s responsibility).

---

## 1. Why ProjectConcord needs this

EDF now defines a controlled way to leave a **program gate obligation Open** while **authorizing a bounded downstream activity** without falsely closing the gate or skipping governance.

The motivating pattern (observed in adopting architecture programs): substantive architecture is **Accepted**, but documentation gates (indexing, ADR transcription, reconciliation) remain **required** yet should not block a **named implementation tranche** or implementation planning when they do not materially affect that work.

ProjectConcord must **visualize**, **evaluate**, **authorize**, and **track** this behavior. EDF does **not** define ProjectConcord UI; it defines **normative semantics** ProjectConcord must implement faithfully.

---

## 2. Authoritative EDF sources (read first)

| Artifact | Role |
|----------|------|
| [EGR-0001 v1.1](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md) | **Normative** requirements, conformance, worked example |
| [ADR-0007 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/ADRs/ADR-0007-Engineering-Gate-Review-Records.md) | Architectural decision context |
| [Gate_Review_Record_Template](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Templates/Gate_Review_Record_Template.md) | Instance shape (GDO table, waiver/supersession, prerequisite overrides) |
| [Glossary](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Reference/Glossary.md) | GDO, Authorized Downstream Scope, Dependency Disposition, etc. |
| [AAR-0001 v1.1](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md) | Audit evidence when implementation ran under Active GDOs |

**Naming collision:** ProjectConcord [ADR-0007](../Architecture/ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md) is **not** EDF ADR-0007. When citing gate/GDO rules, refer to **EDF ADR-0007** and **EGR-0001**.

**Note:** EGR-0001 and EDF ADR-0007 remain **Proposed** in EDF; treat them as the current adoptable contract unless EDF later Accepts with changes.

**Out of scope for EDF (ProjectConcord’s job):** tool UI, dependency graph storage, real-time validation engine, sync strategy details.

---

## 3. Core design rule — three independent dimensions

Do **not** overload a single “gate status” field.

| Dimension | Values | Meaning |
|-----------|--------|---------|
| **Gate Status** | Open, Satisfied, Rejected, Deferred, Waived, Superseded | Lifecycle of the **obligation** |
| **Dependency Disposition** | Blocking (default), Non-Blocking | For a **specific** downstream activity vs this prerequisite |
| **Override Status** | Active, Reactivated, Closed | Lifecycle of a **GDO** on the source EGR |

**Critical distinctions**

- `Deferred` = gate **decision postponed** — **not** Non-Blocking authorization.
- `Satisfied` / `Waived` / `Superseded` = closed gate outcomes — **not** a GDO.
- GDO = source gate stays **`Open`**, obligation **still required**, dependency **Non-Blocking only for named scope**.

ProjectConcord should display, for example:

```text
G4  Gate Status: OPEN
    Dependency disposition (vs "Tranche A implementation planning"): NON-BLOCKING
    GDO-1: Active
```

---

## 4. Governed Dependency Override (GDO) — semantics

- **Only** mechanism that may set Non-Blocking while source gate is Open.
- Recorded **on the source EGR** (Markdown table today); local IDs **`GDO-1`**, **`GDO-2`** per file — **no** global `GDO-NNNN` namespace.
- **Not** completion, waiver, supersession, or informal bypass.
- Human activation only; tools may draft, not activate without explicit human direction.

### Required GDO fields (mirror EDF template)

Source gate / remaining obligation; **Authorized Downstream Scope**; Dependency Disposition = Non-Blocking; Authority; Date; Rationale; **Safety basis**; Conditions/constraints; Reactivation conditions; Affected declared dependencies; Override Status.

**Expediency alone is insufficient** — safety basis must stand on its own; process cost/value may appear only after that.

### Binding amendment — Authorized Downstream Scope

Scope is **not** limited to another EGR or gate.

A GDO **MAY** authorize a clearly bounded:

- downstream gate
- implementation tranche
- milestone
- release activity
- named work package
- other explicitly identified governed activity

The scope **MUST** be precise enough to decide what is in and out of authorization. **Work outside scope remains Blocking.**

If the authorized activity has its own EGR, that EGR **SHOULD** reference the source GDO. **A downstream EGR is not required** merely because a GDO exists.

---

## 5. Dependency evaluation (ProjectConcord core logic)

For each edge **prerequisite P → downstream activity D**:

1. If P is **Satisfied** (or Waived/Superseded per project rules), D may proceed per normal rules.
2. If P is **Open** and **no** Active GDO on P includes **D** in Authorized Downstream Scope → **BLOCKING** — D must not start.
3. If P is **Open** and an **Active** GDO on P **explicitly** includes D in scope → **NON-BLOCKING** for that D only; P remains Open and visible (**governance debt**).
4. If GDO is **Reactivated** → treat as Blocking for affected scope until resolved or a new constrained GDO is granted.
5. If D is **not named** in any Active GDO → **Blocking** even if another activity was authorized.

ProjectConcord should support **scoped** edges (same Open gate Blocking for G8 but Non-Blocking for “Tranche A planning”), not global “gate unlocked.”

**BVG (bootstrap G1–G6):** separate namespace; GDO must **not** relax BVG.

Relate to ProjectConcord [DevelopmentWorkAuthorization](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) and [ADR-0013](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md): **handover** and **authorization** remain distinct from EGR GDO; GDO governs **gate prerequisite blocking**, not provider clipboard packages.

---

## 6. Authority and eligibility (workflow / RBAC)

- Authority = whoever owns the **governance dependency** (EGR Owner, Decision maker, project architecture authority from context — **not** hard-coded to “Project Architect” only).
- Multi-domain obligations: if safety/security/privacy/compliance/destructive controls are in scope, **domain owner** must co-approve or GDO is **ineligible**.
- Eligibility test: unfinished work must **not** contain an unresolved matter that **materially affects** the authorized downstream activity (EDF gives guidance examples; decision is case-by-case).

Suggested controlled operations (names illustrative):

- **Authorize non-blocking dependency** (create/activate GDO with full evidence)
- **Reactivate blocking dependency** (invalidate assumption; pause affected downstream work)
- **Close override** (source gate Satisfied/Waived/Superseded or scope consumed)

---

## 7. Governance debt and closeout

- **Governance debt** = Open EGRs with **Active** GDOs (indexed; no separate EDF debt register). EDF expects **Active overrides** column on [Gate Reviews](../Program/Gate_Reviews/README.md) index.
- On program/release **closeout**: each such gate must be Satisfied, Waived, Superseded, or **explicitly carried forward** with a still-valid Active GDO.
- ProjectConcord should surface debt dashboards and closeout blockers/warnings per project policy.

---

## 8. Relationships ProjectConcord must not confuse

| EDF artifact | Use for GDO? |
|--------------|------------|
| **EGR + GDO table** | **Yes** — primary |
| **AWI** | **No** — deferred *initiatives* off roadmap; do not auto-create AWI for GDO |
| **AAR “Deferred” finding** | **No** — implementation vs requirements in an audit |
| **EGR `Deferred` outcome** | **No** — postponed gate decision |
| **Accepted exceptions** | Pattern for **Waived** gate evidence, not GDO |
| **PCR / pause records** | Separate Concord domain; do not conflate with GDO |

**AAR linkage:** audits **SHOULD** note whether implementation proceeded under Active GDOs; GDO does **not** complete an audit or hide gaps. Conflicting docs discovered later → Gap/Violation + **Reactivate** relevant GDO. See [ADR-0012](../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md).

---

## 9. Persistence and interchange (recommended for Concord)

EDF today: Markdown tables on `docs/Program/Gate_Reviews/*.md`. ProjectConcord will likely need an internal model plus **round-trip** or **projection** to EGR files (align with [ADR-0002](../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)).

Suggested conceptual properties (not an EDF schema mandate):

- `gateId`, `gateStatus`
- `dependencyDisposition` (per scope key)
- `overrides[]`: `localId`, `authorizedDownstreamScope`, `authority`, `date`, `rationale`, `safetyBasis`, `conditions`, `reactivationConditions`, `affectedDependencies`, `overrideStatus`
- `prerequisiteOverrides[]` on downstream EGR when applicable

**Must distinguish without prose parsing:**

- incomplete + blocking
- incomplete + explicitly non-blocking (valid Active GDO)

Framework Advisor **does not** parse EGR/GDO yet ([Analyzer_Compliance](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Governance/Analyzer_Compliance.md) — future class). Concord may lead validation UX before EDF automates checks. Implementation scope recorded in [EDF Gap Register](../Development/EDF_Gap_Register.md) **GAP-040** (extends GAP-006).

---

## 10. Validation rules (implement in Concord)

Nonconformant / blocked:

- Downstream start while prerequisite Open + Blocking (no covering GDO)
- Open obligation represented as Satisfied only to unblock work
- GDO missing authority, date, safety basis, or **precise** scope
- Downstream work outside Authorized Downstream Scope
- Reactivated GDO still treated as Non-Blocking
- Closeout with unreconciled Active GDOs when policy requires reconciliation
- Treating missing downstream EGR as error when only GDO authorizes a non-gate activity

Conformant:

- Downstream start under Active GDO that **explicitly** includes that activity

---

## 11. Anti-patterns (do not build)

- “Mark everything non-blocking” bulk override without per-scope evidence
- Equating GDO with gate complete or ADR Accepted
- Global unlock of a gate for all future work from one GDO
- Silent bypass without EGR update
- Requiring a downstream EGR for every GDO
- New global GDO record type competing with source EGR (unless Concord has a **projection** layer that still writes EDF-shaped Markdown)

---

## 12. Suggested ProjectConcord work breakdown

1. **Domain model** — Gate Status, Dependency Disposition, GDO, scope identity for downstream activities (gate ID, tranche ID, work package ID, etc.).
2. **Dependency engine** — Evaluate Blocking vs Non-Blocking per activity; support partial scopes on one Open gate.
3. **Authorization UI** — GDO wizard with mandatory fields and eligibility prompts; co-approval for domain constraints.
4. **Visualization** — Dual badges (Open + Non-Blocking for scope X); governance debt list; closeout view; extend Gate Reviews index columns per EDF.
5. **EGR sync** — Read/write `Governed Dependency Overrides` and `Prerequisite overrides` sections per EDF template.
6. **Reactivation flow** — User or audit finding triggers Reactivated → pause + authority review.
7. **AAR integration** — Optional metadata: Active GDOs in implementation scope.
8. **Tests** — Conformance cases from EGR-0001 §Conformance and worked example (indexing open, tranche authorized, conflict → reactivate).

---

## 13. EDF status for Concord planners

- Implemented on EDF `main` (`0016f15`); EGR **1.1**, AAR **1.1** touch GDO.
- ProjectConcord already has instance EGRs ([G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md), [G1](../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md)); GDO fields are not yet modeled in UI or engine.
- Normative EDF text uses generic tranche/gate examples (no adopting-project names).

---

## 14. Decisions before implementation

1. Canonical store: EGR Markdown only, operational DB with export, or hybrid?
2. How downstream activities are **identified** in the graph (stable IDs for tranches/work packages).
3. Whether closeout policy is per-project configurable on top of EDF minimums.
4. How Reactivation couples to issue/AAR workflows and [DevelopmentWorkAuthorization](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md).

**EDF remains normative for semantics; ProjectConcord owns execution, UX, and evaluation.**

---

## Parent

- [Handover](README.md)

## Related Documents

- [Program](../Program/README.md)
- [Gate Reviews](../Program/Gate_Reviews/README.md)
- [EDF Gap Register](../Development/EDF_Gap_Register.md) — GAP-006 (gates), GAP-040 (GDO engine / UX)
- [Implementation Roadmap](../Development/Implementation_Roadmap.md)
- [ADR-0012 — Adopt EDF AAR](../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md)
- [SPEC-003 — Canonical artifact integrity](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)
