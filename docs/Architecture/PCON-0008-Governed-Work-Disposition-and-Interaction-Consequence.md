[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › PCON-0008

# PCON-0008: Governed Work Disposition and Interaction Consequence

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural framework record |
| **Normative** | Yes |
| **Status** | **Accepted** (2026-10-06 — Project Architect publication) |
| **Record ID** | PCON-0008 |
| **Date** | 2026-10-06 |
| **Owner** | ProjectConcord |
| **Implementation** | **Not authorized** — architecture and documentation only |
| **Investigation** | Handovers `f4e42b75-28ad-4c39-bf75-fd9605370e3e`, `9f3e78d8-bd61-4a74-ae54-403c779ca979`; documentation authorization `c14bb671-46b9-41f7-8544-bcd43cc264a0` |
| **Companion** | [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md), [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md), [PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [PCON-0007](PCON-0007-Governed-Synchronization-Review.md) |

---

## 1. Purpose and scope

This record defines how **arbitrary interaction and reasoning** relate to **authoritative governed state** and **governed work topology** in ProjectConcord.

It **SHALL**:

- compose with the Workflow Framework ([PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md)) and multi-instance graph semantics ([ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §12);
- preserve **GIES** as interaction infrastructure ([PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md)) and **GSR** as specialized synchronization review ([PCON-0007](PCON-0007-Governed-Synchronization-Review.md));
- preserve provenance without making chat transcripts authoritative ([PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) FW-7; [ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15).

This record is **not**:

- a fifth Workflow Framework primitive;
- a prescribed workflow;
- authorization to implement runtime disposition ingestion, extended dependencies, or new registry entries in this tranche.

---

## 2. Core principle

ProjectConcord **SHALL NOT** model every conversational exchange or reasoning path.

ProjectConcord **MUST** model **consequential changes to authoritative governed state** and **consequential units of governed work**.

Illustrative shorthand (non-substitute for normative rules):

> Govern the consequences of interaction, not every interaction.

---

## 3. Interaction → disposition → governed state

```text
arbitrary PA / EA / human interaction
                |
                v
    bounded Governed Work Disposition
                |
                v
      validated authority / policy
                |
                v
   authoritative governed consequences
```

**Conversation**, **handover prose**, and **LLM reasoning** are **not** authoritative workflow state merely because they exist.

A governed consequence **SHALL** cross an **authorized or validated governance boundary** before authoritative mutation.

Typical boundary mechanisms (composition, not a single engine):

- deterministic ProjectConcord evaluation;
- validated relay import ([SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md));
- recorded human or Project Architect judgment with provenance ([PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) GIES-3, GIES-4);
- future PA Exchange canonical directive ([AWI-0010](Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md) §B.2);
- F-layer artifact commit where policy establishes authority ([ADR-0006](ADRs/ADR-0006-AI-Boundary.md)).

**Engineering Agent or provider assertions** remain **non-authoritative** until validated or imported per applicable rules.

---

## 4. Governed Work Disposition

**Governed Work Disposition** is a **semantic and protocol boundary pattern** representing a **bounded consequential outcome** of reasoning or interaction.

It is **currently not**:

| Not | Meaning |
|---|---|
| Fifth Workflow Framework primitive | Primitives remain §6 of [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) |
| Persisted domain entity | No mandated store shape in this tranche |
| Required database table | — |
| Mandatory universal runtime orchestrator | Existing mutation surfaces remain authoritative |
| Replacement for mutation authorities | DWA/**Control**, STOP, relationships, evidence, sync satisfaction |

**Authoritative consequences** continue to be recorded through **existing mechanisms** (for example `WorkflowInstance`, `WorkflowOrigin`, `WorkflowDependency`, **DevelopmentWorkAuthorization**, instance **STOP**, **EvidenceRequirement** satisfaction, **SynchronizationPoint** satisfaction when implemented, GSR/GIES B-layer correlates).

Disposition **protocol** shapes (wire fields, intake APIs) are **deferred** ([GAP-054](../Development/EDF_Gap_Register.md)).

---

## 5. Governed work identity versus authorization

**Workflow / work-obligation identity** and **authorization to perform work** are **orthogonal**.

| Concern | Question |
|---|---|
| **Identity** | Is this the same governed work obligation or a distinct one? |
| **Authorization** | Is execution permitted under **Control** / DWA and policy? |

Rules:

- Work **MAY** remain inside **Workflow A** while requiring **new or expanded** DWA/**Control** or other authorization.
- **Exceeding current DWA scope** **SHALL NOT** by itself create **Workflow B**.
- **Workflow B** (`WorkflowInstance`) is required when a **distinct governed work obligation** exists under applicable **prescribed workflow** and **policy** semantics ([§7](#7-when-work-remains-in-a-versus-workflow-b)).

---

## 6. Distinct governed work must not remain hidden

If interaction discovers a **distinct governed work obligation**, that work **MUST** ultimately be represented by a governed **`WorkflowInstance`** under an **applicable prescribed workflow**.

Substantial governed work **SHALL NOT** remain only in:

- PA/EA chat;
- handover prose;
- provenance without instance;
- unrelated DWA;
- informal correction instructions.

ProjectConcord **SHALL NOT** introduce a parallel **quasi-workflow** or **gate-equivalent obligation** mechanism that bypasses `WorkflowInstance` and prescribed-workflow semantics.

---

## 7. Discovery versus execution

**Discovery** of a distinct governed obligation and **authorization/execution** of that obligation are **separate governance events**.

```text
issue discovered
      |
      v
governed disposition
      |
      v
distinct obligation B identified
      |
      v
applicable prescribed workflow resolved?
      |
      +-- YES --> instantiate / authorize per applicable policy
      |
      +-- NO  --> unresolved workflow applicability;
                  governed execution of B withheld
```

**Unresolved prescribed-workflow applicability** is **fail-closed** for **governed execution** of B.

It is **not** authorization to execute B.

It is **not** a quasi-workflow substitute.

It is the statement that a governed obligation is recognized but its proper **prescribed execution path** has not yet been resolved.

Storage and runtime representation of the unresolved condition are **not** prescribed in this tranche ([§12](#12-workflow-applicability-requirement)).

---

## 8. When work remains in A versus Workflow B

### 8.1 Remain in Workflow A

Work stays in **Workflow A** when it is the **same governed work obligation** under the same prescribed-workflow purpose, including:

- reassessment, evidence reclassification, withdrawn attestations, coverage gaps, and corrected verification within A’s closeout;
- authorization expansion via DWA/**Control**, STOP, or sync/evidence paths **without** distinct obligation identity.

### 8.2 Create Workflow B

Create **Workflow B** when policy and prescribed-workflow semantics identify a **distinct governed work obligation** with its own completion criteria and lifecycle, including when a **different prescribed workflow** applies (compare GEW vs GMFP — [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §7).

**Unexpected** discovery does **not** by itself imply B; **distinct obligation identity** does.

---

## 9. Workflow applicability requirement

Every distinct governed work obligation that requires **tracked governed execution** **SHALL** be associated with an **applicable prescribed workflow** before governed execution begins.

This tranche **SHALL NOT**:

- force architecture/documentation work into **GEW v1** merely because GEW is the only current runtime registry entry;
- define a new prescribed architecture workflow;
- assume GEW cannot eventually support such work through legitimate policy or definition extension.

**Unresolved prescribed-workflow applicability** for newly discovered obligations is recorded as an architecture/runtime gap ([GAP-054](../Development/EDF_Gap_Register.md)).

---

## 10. WorkflowOrigin and WorkflowDependency

Multi-instance topology remains a **graph**, not a parent/child tree ([ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §12).

### 10.1 Normative rule

**WorkflowOrigin alone SHALL NOT imply WorkflowDependency.**

| Construct | Role |
|---|---|
| **WorkflowOrigin** | Why/how a derived obligation arose (provenance) |
| **WorkflowDependency** | Whether progression of one instance depends on another |

**Dependency establishment** requires applicable **workflow/policy** semantics and an **authorized governed disposition**.

**Absence** of an established dependency means only that **no dependency has been established**. It **MUST NOT** be interpreted as a universal architectural default that discovered work is inherently non-blocking.

### 10.2 Preserved graph properties

- multiple origins where valid;
- multiple blockers; shared blockers;
- recursive discovery;
- cycle rejection where dependency semantics require satisfiability;
- no globally primary Active workflow;
- actionable frontier **derived**, not persisted as authority.

---

## 11. Dependency granularity gap

Distinguish:

| Layer | Meaning |
|---|---|
| **A** | Previously valid/accepted governed state |
| **B** | Future progression currently withheld |
| **C** | Governed condition that must be resolved before that progression becomes eligible |

**Current runtime** (as of documentation tranche): `WorkflowDependency` is **instance-level**; `WorkflowDependencySatisfactionCondition` supports **`RequiredInstanceLifecycleCompleted`** only.

This tranche **SHALL NOT** claim runtime can precisely represent:

> implementation review remains valid, but publication progression is withheld pending B

as a **gate-specific** dependency unless implementation evidence proves otherwise.

**Gate-specific withholding** versus **instance-level blocking** is an architecture/runtime gap ([GAP-054](../Development/EDF_Gap_Register.md)). **No** extension is implemented here.

---

## 12. Dependency satisfaction

`WorkflowDependencySatisfactionCondition` is the existing **extension point**.

`RequiredInstanceLifecycleCompleted` **MAY** be **insufficient** for future governance relationships.

This tranche **SHALL NOT** define a speculative taxonomy of additional conditions or select a first new condition.

**Invariant:** dependency **satisfaction** or **release** **NEVER** grants authorization to the dependent workflow. It changes dependency eligibility and **triggers or requires reevaluation** only.

---

## 13. Shared discovered work — governed obligation identity

When **A** and **B** independently discover apparently similar work **C**, the architectural question is **governed obligation identity**:

- **one** obligation with **multiple** `WorkflowOrigin` records; or
- **distinct** obligations (distinct `WorkflowInstance` records).

**Text similarity**, **LLM inference**, or **conversational resemblance** **MUST NOT** silently merge `WorkflowInstance` records.

The identity decision **SHALL** be **authoritative/governed** (disposition, policy, human/PA determination).

Correlation schema for disposition intake is **deferred**.

---

## 14. Reassessment and fail-closed premise invalidation

When a previously relied-upon governed premise becomes uncertain or is **authoritatively** challenged, affected governed conclusions **MUST NOT** remain actionable solely because they were previously accepted.

They remain non-actionable until deterministically **reaffirmed**, **corrected**, **superseded**, **invalidated**, or otherwise **resolved** under applicable policy.

Invalidation **SHALL** be scoped as narrowly as reasonably possible.

This principle **SHALL NOT** universally invalidate workflow history, unrelated evidence, unrelated instances, or repository HEAD-derived state without policy.

Preserve distinctions among:

| Concern | Typical mechanism |
|---|---|
| Evidence validity | **EvidenceRequirement**, MVR, GSR evidence fingerprints |
| Package applicability | [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §19, PC-PAR-025 |
| DWA applicability | DWA occurrence binding, supersede |
| GSR review disposition | [PCON-0007](PCON-0007-Governed-Synchronization-Review.md) §8 |
| Synchronization satisfaction | **SynchronizationPoint** orthogonal state |
| Workflow dependency | `WorkflowDependency` status |

---

## 15. Relationship to GIES, GSR, and PC-PAR-025

| Capability | Role |
|---|---|
| **GIES** ([PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md)) | Interaction envelope: question/answer, provenance, Interaction Hold, pause/resume at governed-operation boundary — **not** workflow topology; **not** a catalog of every conversation |
| **GSR** ([PCON-0007](PCON-0007-Governed-Synchronization-Review.md)) | Specialized governed synchronization review: subjects, evidence, dispositions, deterministic invalidation — **not** a universal workflow engine |
| **PC-PAR-025** ([SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §19.4) | Relay package **applicability/staleness** — one application of fail-closed applicability; **not** generic evidence validity |

---

## 16. Recovery

Durable recovery **SHALL NOT** require replaying chat or handover history ([ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15).

Authoritative or deterministically derivable recovery **SHALL** ultimately account for:

- Active `WorkflowInstance` records and traversal state;
- applicable operation baseline;
- `WorkflowOrigin` and `WorkflowDependency`;
- DWA/**Control** and instance **STOP**;
- applicable evidence and synchronization satisfaction state;
- unresolved governed applicability or reassessment conditions where policy requires them;
- GIES/GSR operational correlates when implemented.

**CandidateFrontier**, **Waiting On**, **Next Action**, and similar projections remain **derived** where possible.

Persistence schema for new facts is **not** prescribed in this tranche.

---

## 17. Validation case — M7a-WF-1d and architecture obligation B

Illustrative application only — not universal law.

### 17.1 Inside Workflow A (M7a-WF-1d)

- verification classification correction;
- withdrawal/reclassification of false human verification records;
- automated verification evidence and coverage gaps;
- eventual corrected verification evidence when directed.

### 17.2 Distinct obligation B

Interaction → Governed Consequence → Workflow Topology architecture (investigation chain `f4e42b75`, `9f3e78d8`, documentation `c14bb671`).

**B** is a **distinct governed work obligation**.

Current architecture/runtime does **not** yet establish the **correct prescribed workflow** under which **B** should execute.

**B SHALL NOT** be falsely instantiated as **GEW** merely because GEW is the only registered runtime workflow today.

### 17.3 WF-1d state (PA determination)

| Fact | Treatment |
|---|---|
| WF-1d instance | Remains **Active** |
| Implementation review PASS | Remains **valid** — not retroactively invalidated |
| Valid automated evidence | Remains valid per **actual** coverage |
| MVR-0004 | Remains **stopped** — not resumed by this record |
| Publication | **Not authorized**; future publication progression **withheld** while architecture issue unresolved |
| Instance-level `WorkflowDependency` | **SHALL NOT** be claimed to precisely encode gate-specific publication withholding without implementation proof ([§11](#11-dependency-granularity-gap)) |

---

## 18. Architectural composition

```text
Prescribed workflow / policy
  + WorkflowInstance / graph (origin, dependency)
  + Primitives: SynchronizationPoint, AEI, EvidenceRequirement, Control
  + GIES (interaction envelope, when used)
  + GSR (sync review episodes, when used)
  + Governed Work Disposition (intake boundary pattern)
  + Validated relay / PA Exchange / F-layer evidence
  + Derived operator projections (ADR-0020)
```

---

## 19. Related documents

- [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md), [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)
- [ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15
- [PCON-0006](PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [PCON-0007](PCON-0007-Governed-Synchronization-Review.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [AWI-0010](Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md), [GAP-054](../Development/EDF_Gap_Register.md#gap-054--workflow-framework-runtime-and-effective-configuration)

---

## 20. Publication disposition

**Accepted** and published on `main` (2026-10-06 — handover `d669580d-886a-48ba-a13d-2b86bf0dff20`; documentation tranche `c14bb671-46b9-41f7-8544-bcd43cc264a0`).

This record **does not** authorize runtime implementation, registry changes, dependency extensions, or MVR/WF-1d verification resumption.
