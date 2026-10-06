[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › PCON-0007

# PCON-0007: Governed Synchronization Review

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural framework record |
| **Normative** | Yes |
| **Status** | **Accepted** (2026-10-06 — Project Architect publication) |
| **Record ID** | PCON-0007 |
| **Date** | 2026-10-06 |
| **Owner** | ProjectConcord |
| **Implementation** | **Not authorized** — architecture and documentation only |
| **Companion** | [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md), [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md), [PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) |
| **Investigation** | [Handover record](../Handover/ProjectConcord-Governed-Synchronization-Review-Investigation.md) |

---

## 1. Purpose and scope

**Governed Synchronization Review** is the provider-neutral capability for **incremental evidence acquisition** and **partial disposition** while satisfying an applicable **SynchronizationPoint** under prescribed-workflow policy.

A **Governed Synchronization Review Context** is the durable operational correlation of **one synchronization-review episode** (for example implementation return review while a workflow instance occupies a post-submission governance region).

This record **SHALL**:

- compose with [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) primitives and instance state;
- use [PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) for interaction mechanics where incremental acquisition or judgment requires governed interaction;
- preserve provenance without making chat transcripts authoritative ([PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) FW-7; [ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15).

This record is **not**:

- a new prescribed workflow;
- a fifth Workflow Framework primitive;
- a replacement for GIES;
- a parallel session or chat subsystem.

**Incremental review is not reduced assurance.** It reduces unnecessary re-review overhead while preserving required governance.

---

## 2. Terminology

| Term | Meaning |
|---|---|
| **Governed Synchronization Review** | The governed capability/pattern |
| **Governed Synchronization Review Context** | Durable **B-layer** operational correlation of one review episode |
| **Review Subject** | Governed thing, claim, implementation area, or other review concern that receives a **disposition** |
| **Review Evidence** | Versioned, fingerprinted artifact or record supporting disposition of one or more subjects |
| **Review frontier** | The set of subjects and pending evidence needs the **required reviewer/authority** has opened or required next — reviewer-controlled |
| **Disposition** | Judgment outcome recorded for a **Review Subject**, citing supporting evidence and provenance |
| **Required reviewer / authority** | Policy-selected role or authority class (provider-neutral — not a product name) |

---

## 3. Architectural composition

```text
Prescribed workflow / GEW policy
  + SynchronizationPoint (obligation; orthogonal satisfaction)
  + EvidenceRequirement (objective criteria; MVR when policy requires)
  + Control / DevelopmentWorkAuthorization (bounded correction envelope)
  + GIES (interaction items, hold, provenance)
  + Governed Synchronization Review Context (B-layer operational state)
  + Derived operator projections (ADR-0020)
```

| Primitive / capability | Role in Governed Synchronization Review |
|---|---|
| **SynchronizationPoint** | Declares the synchronization obligation; **satisfaction** is recorded orthogonally to topology place ([ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §11) |
| **EvidenceRequirement** | Objective evidence that may satisfy the point on an **alternate** path, or that must exist before satisfaction |
| **Control / DWA** | Bounded execution and **correction** authorization; scope is impact **envelope** (§8) |
| **GIES** | Acquire evidence, elicit judgment, hold — **not** aggregate sync satisfaction by answer text |
| **ArchitecturalReviewSubmission** | Operational anchor for plan/implementation return ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)) |

---

## 4. Review Subject vs Review Evidence

**Review Subject** and **Review Evidence** **SHALL NOT** be conflated.

- A **Review Subject** is what receives a disposition (for example “DWA occurrence applicability”, “MVR-0004 readiness”, “operator Next Action projection semantics”).
- **Review Evidence** is what was inspected (for example a source file at fingerprint X, a test run R, an MVR document at fingerprint Z).

One subject **MAY** cite multiple evidence items over time. A disposition **SHALL** be recorded against the **subject** and **SHALL** reference the evidence set (fingerprints, baseline, provenance) that supported it.

**Examples (illustrative):**

| Review Subject | Possible Review Evidence |
|---|---|
| DWA occurrence applicability | Applicability module @ fingerprint X; tests @ Y; release test run R |
| MVR readiness | MVR file @ Z; applicable MVR requirement refs; prior implementation-review disposition |

Persistence shapes for subjects and evidence are **not** prescribed in this tranche.

---

## 5. Reviewer-controlled frontier

The **required reviewer/authority** (per policy) **SHALL** control which subject or evidence to inspect next.

An Engineering Agent or operator **return package** does **not** by itself establish that review evidence is sufficient for **SynchronizationPoint** satisfaction.

The review frontier **MAY** grow, shrink, or reorder while the review context is **Open** (§6).

---

## 6. Review-context lifecycle and orthogonal state

### 6.1 Context lifecycle (minimal)

| State | Meaning |
|---|---|
| **Open** | Review context active for a synchronization occurrence; dispositions and frontier may evolve |
| **Closed** | Context closed: synchronization satisfied (interactive or alternate path), or cancelled/superseded per policy |
| **Historical** | Retained for provenance; not actionable for advancement |

These states **SHALL NOT** be merged into a single large mutually exclusive machine with concurrent review facts.

### 6.2 Orthogonal dimensions (concurrent while Open)

| Dimension | Role |
|---|---|
| **Review frontier** | Subjects and evidence needs under active review |
| **Per-subject disposition history** | Latest disposition plus historical records |
| **Review Evidence set/history** | Versioned evidence supplied over time |
| **GIES interactions / Interaction Hold** | Blocking or advisory acquisition and judgment |
| **Correction episode** | Bounded DWA, submission supersession/patch, invalidation results |
| **Aggregate SynchronizationPoint state** | Unsatisfied vs satisfied for the applicable occurrence |

**Provenance** across **Closed** and **Historical** **SHALL** be preserved ([PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) FW-7).

### 6.3 Episode correlation (minimum)

Implementations **SHOULD** correlate at minimum:

- `WorkflowInstanceId`, `TopologyPlaceId`, `TraversalOccurrenceId`;
- `SynchronizationPoint` identity and **SynchronizationOccurrenceId** (opaque; distinct on re-entry — analogous to traversal occurrence);
- `ArchitecturalReviewSubmission` identity (or supersession chain);
- optional **Governed Interaction Context** identifier when GIES participates.

---

## 7. Partial dispositions

Individual **Review Subject** dispositions **SHALL NOT** by themselves satisfy the **SynchronizationPoint** ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-079).

Disposition vocabulary (exact enum names **open** — §16) **MAY** include concepts such as accepted, accepted pending later evidence, correction required, insufficient evidence, and invalidated.

Profiles and policy **MAY** constrain which interim dispositions are permitted (§14).

---

## 8. Evidence reuse and deterministic invalidation

### 8.1 Reuse (normative direction)

A Governed Synchronization Review **SHALL** support incremental evidence acquisition and partial **Review Subject** disposition without requiring **accepted, unaffected** subjects to be re-reviewed after a bounded correction, **provided** the authoritative facts, subject/evidence versions, scope, provenance, applicable policy, and declared/prescribed dependencies supporting the prior disposition remain valid.

Re-review **SHALL** occur when deterministic invalidation establishes that a prior disposition is no longer valid.

**Conversational or LLM inference is not authoritative** for invalidation.

### 8.2 Invalidation triggers (deterministic)

| Trigger | Typical effect |
|---|---|
| **Review Evidence** fingerprint/version change for evidence cited by a disposition | Evidence stale; dependent subject disposition invalidated or re-evaluated per policy |
| **Authoritative fact** change that a disposition depended on | Affected disposition invalidated/re-evaluated |
| **Review Subject** scope/definition change | Prior disposition for that subject invalidated/re-evaluated |
| **Declared or prescribed dependency** | Propagate invalidation to dependents |
| **TraversalOccurrenceId** or **SynchronizationOccurrenceId** change | Prior context non-actionable per [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §19 / policy |
| **Baseline drift** | Per [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) §9.1 and PC-AIGOV-009 — **not** universal invalidation on repository HEAD advance alone |
| **Superseded** submission | Historical dispositions; reconcile unchanged evidence fingerprints per policy |

### 8.3 DWA correction scope (impact envelope)

**DevelopmentWorkAuthorization** correction scope is the authorized **possible impact envelope**. Scope membership **SHALL NOT** universally invalidate a prior disposition.

Scope **MAY**:

- identify **candidate** subjects for impact evaluation after correction;
- bound what changes were authorized;
- trigger **conservative re-review** when profile/policy requires.

**Forbidden:** LLM interpretation of correction prose as authoritative invalidation.

---

## 9. Aggregate SynchronizationPoint satisfaction

### 9.1 Interactive path

Where policy requires **interactive reviewer judgment**, individual Review Subject dispositions **do not** automatically satisfy the **SynchronizationPoint**.

**Aggregate satisfaction** requires the applicable **governed reviewer/authority outcome** recorded through workflow / **Control** mechanisms — not GIES answer text alone ([PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) GIES-3).

### 9.2 Conditional alternate synchronization path

[AWI-0010 §B.3](Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md) **conditional alternate synchronization satisfaction** remains valid.

Where workflow/policy establishes eligibility, applicability, required objective evidence, and successful criteria, an eligible **SynchronizationPoint** **MAY** be satisfied through the **authorized alternate path** without another interactive reviewer directive.

This is **alternate governed synchronization satisfaction** — **not** automatic reviewer approval and **not** conflation with per-subject dispositions.

**Synchronization path disposition** (for example RequireInteractiveSynchronization) remains an operational fact, **not** a fifth primitive.

---

## 10. MVR relationship

Governed Synchronization Review does **not** universally require a **Manual Verification Record (MVR)** for aggregate implementation-review synchronization satisfaction.

An MVR is an **EvidenceRequirement** only where applicable workflow, profile, or policy requires it ([SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md)).

Distinct synchronization or authorization obligations **MAY** carry different evidence requirements. A sequence in which implementation review passes while human MVR execution remains outstanding is **illustrative only** — not universal GEW law.

---

## 11. GIES relationship

GIES **SHALL** be used for interaction that must participate in governed state, routing, provenance, or resume — per [PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) and [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-077.

Reference mapping for Governed Synchronization Review:

| Need | Typical GIES class |
|---|---|
| Supply or confirm **Review Evidence** | Information / fact; human observation when required |
| Record **Review Subject** disposition | Architectural / governance judgment |
| **Aggregate sync satisfaction**; **bounded correction DWA** | Authorization / decision — effected via **Control**/workflow |

GIES **owns interaction mechanics**, not review policy, review frontier policy, or aggregate synchronization satisfaction law.

See [PCON-0006 §13.1](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) (reference use case).

---

## 12. Correction and resume

1. Reviewer records disposition requiring correction (and optional GIES item).
2. Reviewer issues **bounded** corrective authorization via **Control** / DWA — without silent scope expansion (PC-AIGOV-078).
3. Operator or agent returns evidence; **ArchitecturalReviewSubmission** may supersede or patch per policy.
4. Apply §8 invalidation; re-review **invalidated** or **newly required** subjects only.
5. **Resume** the same **Governed Synchronization Review Context** and synchronization occurrence where policy permits — rather than reconstructing review state from chat.

---

## 13. Pause, resume, and recovery

Governed work-state recovery **SHALL NOT** depend on provider chat ([ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15).

Authoritative inputs **SHALL** include persisted review context, subject dispositions, evidence fingerprints, GIES hold/items when active, and orthogonal synchronization satisfaction state ([GAP-054](../Development/EDF_Gap_Register.md), [GAP-055](../Development/EDF_Gap_Register.md)).

Human copy/paste or upload **MAY** transport evidence; Concord **SHALL** record supplied evidence and provenance — transport is not canonical ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-006).

**Interaction Hold** ≠ **STOP** ≠ [PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md) pause.

---

## 14. Profile and policy extension points

Applicable workflow, **GEW profile**, and policy **MAY** constrain:

- required **Review Subjects**;
- **EvidenceRequirements** (including MVR when required);
- provisional/interim disposition availability;
- review depth and independent-review requirements;
- invalidation conservatism after correction;
- alternate synchronization eligibility.

This record **does not** define profile-specific rules for `gew.standard`, `gew.high-assurance`, or `gew.accelerated` unless separately established.

---

## 15. Operator projections

Operator surfaces **SHALL** derive from authoritative **A + B** (+ **F** consumption) — [ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md), [AWI-0008](Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) §B.3.

Examples (non-exhaustive):

| Projection | Inputs |
|---|---|
| **Current Work** | Instance in review region + open review context |
| **Waiting On** | Blocking GIES items; subjects lacking required evidence; unsatisfied sync occurrence |
| **Next Action** | Satisfy evidence request; import relay package; run validation — distinct from sync satisfied |
| **Attention** | Baseline drift; conflicting dispositions; valid-but-not-actionable relay packages |

Review packages and frontier presentation **SHALL NOT** substitute for authoritative review state.

---

## 16. Open questions (intentionally unresolved)

1. Exact provisional/interim disposition vocabulary.
2. Dependency authoring model (explicit edges vs prescribed checklist vs hybrid).
3. Review-subject checklist source (DWA scope vs profile-fixed sets vs frontier only).
4. Fingerprint rules by EvidenceKind.
5. AAR vs B-layer submission-review boundary ([ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) §9).
6. Authority-exchange field split ([AWI-0010 §B.2](Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md)).
7. Dirty working tree vs governed baseline evidence policy.
8. Profile/policy-specific conservative re-review after correction.

---

## 17. Explicit non-goals (this tranche)

- `src/` review-context persistence, invalidation engine, or sync satisfaction runtime ([GAP-054](../Development/EDF_Gap_Register.md));
- GIES runtime ([GAP-055](../Development/EDF_Gap_Register.md));
- CPA/CCA runtime ([AWI-0010 §B.3](Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md));
- PA Exchange / SPEC-006 wire schemas;
- Desktop UI; PC-PAR-025 enforcement;
- New gap register entry (GAP-056) — follow-through remains under GAP-054/GAP-055.

---

## 18. Publication disposition

**Accepted** and published on `main` (2026-10-06 — handover `2af49fa4-b592-49e1-81e9-f711071b27df`). **Does not** authorize implementation.

---

## 19. Related documents

- [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md), [PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md), [ADR-0025](ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [AWI-0010](Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md), [AWI-0011](Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md), [AWI-0008](Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
- [Investigation handover](../Handover/ProjectConcord-Governed-Synchronization-Review-Investigation.md)
