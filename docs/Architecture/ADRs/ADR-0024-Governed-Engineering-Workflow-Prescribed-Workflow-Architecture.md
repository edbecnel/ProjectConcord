# ADR-0024: Governed Engineering Workflow — Prescribed Workflow Architecture

## Status

Accepted

## Date

2026-10-03 (Accepted by Project Architect — workflow framework publication)

## Context

[PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) establishes the ProjectConcord Workflow Framework. [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (**Accepted**) defines Software Development **B-layer** boundaries (DevelopmentWorkAuthorization, handover vs authorization, operational vs canonical). [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (**Draft**, not implemented) encodes PC-AIGOV-001–076 including GEW/profile hooks from the [SPEC-004 reconciliation tranche](../../Handover/ProjectConcord-SPEC-004-Workflow-Framework-Reconciliation-Tranche-Plan.md) (reconciled — published on `main` 2026-10-03). [PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) describes a full Snaptara-style loop as **reference** practice — not the only permitted topology.

Human PA ↔ Engineering Agent practice (including PAR track A4) demonstrated **profile-like** rigor differences (tranche authorization, MVR depth) without replacing the framework model.

This ADR records **GEW as the first ProjectConcord-native prescribed workflow** and its **three initial profiles**. It does **not** implement runtime workflow engines. Normative Software Development / GEW product requirements are reconciled in [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) via the [reconciliation tranche plan](../../Handover/ProjectConcord-SPEC-004-Workflow-Framework-Reconciliation-Tranche-Plan.md) (published on `main` 2026-10-03); this ADR records **architectural** placement only.

## Decision

### 1. Governed Engineering Workflow (GEW) registration

| Field | Value |
|---|---|
| **WorkflowId** | `concord.governed-engineering` (working identifier until implementation registry) |
| **Role** | First **ProjectConcord-native** prescribed workflow |
| **Operational today** | **Yes** — the only native workflow that applies in current product/process scope |
| **Normative product requirements** | [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (**Draft**, GEW reconciliation **published** 2026-10-03); this ADR records **architectural** placement |

GEW **SHALL** be interpreted under [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) configuration classes and effective-configuration rules.

### 2. GEW profiles (workflow-specific — not global modes)

GEW defines **three** initial profiles. They are **governed topology/configuration variants** of GEW. They are **not** quality rankings and **not** global ProjectConcord modes.

| ProfileId (working) | Name | Intent |
|---|---|---|
| `gew.accelerated` | **Accelerated** | Bounded, reversible, low-uncertainty engineering where permitted synchronization points and artifacts may be compressed or combined. |
| `gew.standard` | **Standard** | Normal substantive governed engineering. |
| `gew.high-assurance` | **High Assurance** | Cross-cutting, novel, high-consequence, security/persistence/identity-sensitive, human-QA-heavy, external-contract-sensitive, or otherwise high-risk engineering. |

Profile selection **SHALL** follow [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §8. Authoritative policy or state **MAY** resolve the profile automatically; otherwise governed or PA selection is required. Ambiguity **MUST** produce Attention and **MUST NOT** silently select High Assurance or any other profile. The absence of a forced Accelerated or High Assurance profile **does not** by itself constitute authoritative selection of Standard. A future governed policy **MAY** explicitly establish Standard (or another profile) as a default; that is policy, not an intrinsic property of the Standard profile.

**Provenance (GEW):** GEW satisfies framework [FW-7](../PCON-0005-ProjectConcord-Workflow-Framework.md) through applicable Software Development requirements, including **PC-AIGOV-016** (see §3 table).

### 3. GEW invariants (engineering-specific)

In addition to [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) framework invariants, GEW **SHALL** preserve applicable Software Development governance, including:

| Theme | Source |
|---|---|
| Explicit bounded authorization (**DevelopmentWorkAuthorization**) | ADR-0013, SPEC-004, PCON-0001 |
| Handover ≠ authorization | PC-AIGOV-003, ADR-0013 §2 |
| Planning ≠ implementation authorization (for profiles that define both) | PC-AIGOV-004 |
| STOP / fail-closed | PC-AIGOV-007, SPEC-006 |
| Provenance | PC-AIGOV-016 |
| Validation honesty | PC-AIGOV-011–012 |
| Human MVR attestation boundary | ADR-0014 (Proposed), SPEC-005 |
| Project identity partition | ADR-0015, SPEC-006 |
| Truthful status (partial vs complete) | Operational practice / A4-T7 |
| Scope / drift handling | PC-AIGOV-009, 010, 019 |
| Publication / architectural acceptance authority | PC-AIGOV-026, ADR-0006 |

These invariants **MUST NOT** be universalized to every future prescribed workflow.

### 4. GEW v1 configuration surface

Deliberately **small**. Each item classified:

| Item | Class | Notes |
|---|---|---|
| `profileId` | Invariant materialization at selection | Determines profile-fixed sync/evidence floors |
| DWA scope / **authorized operations** | Bounded configuration | Paths, commit/push, docs-only, etc. |
| **Baseline reference** | Bounded configuration | PC-AIGOV-009 |
| Profile-required **synchronization set** | Profile-fixed | Which human sync points cannot be omitted |
| **Evidence-class floors** | Profile-fixed + bounded | Minimum evidence categories per profile |
| Linked **MVR** / human verification | Policy-derived | F-layer; not optional when linked |
| STOP / instance position | Instance state | Not configuration |
| Investigation / read-only work | **Not a profile** — see §5 |

No speculative knobs (reviewer counts, N-of-M, org CAB) in v1 architecture.

### 5. Investigation and research (not a profile)

Read-only investigation **SHALL NOT** be modeled as a GEW rigor profile.

Represent through existing concepts:

- **RESEARCH** / **PLANNING** authorization capability on **Control** / DWA ([PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md));
- workflow **instance state** / phase where relevant;
- Engineering Agent **PLAN** routing intent ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md), `EngineeringAgentMode`).

No new prescribed workflow solely for read-only work.

### 6. Configuration scope (initial)

| Scope | Status |
|---|---|
| Project-level governed policy defaults (e.g. profile ceiling) | **Supported** — **D** facet when implemented |
| Work-unit / DWA bounds | **Supported** |
| Profile selection | **Supported** — PA/policy at intake |
| Organization-level configuration | **Reserved extension** |

### 7. GMFP relationship (separate prescribed workflow)

EDF **Governed Maintenance Fast Path (GMFP)** is architecturally a **separate prescribed workflow** consumed/integrated by ProjectConcord — **not** a GEW profile.

**Rationale (semantics):**

- **Purpose:** bounded corrective maintenance vs governed engineering delivery.
- **Evidence contract:** canonical **GMR** (EDF) vs GEW submissions/MVR/Git evidence packages.
- **Authorization law:** GMFP Gate 1 **single** bounded maintenance authorization vs GEW planning/implementation separation (PC-AIGOV-004) for Standard/High Assurance profiles.
- **Topology:** GMFP-1 / GMFP-2 interval / GMFP-3 — distinct from GEW loop.

GMFP **MAY** reuse framework primitives ([PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §6). EDF semantics **MUST NOT** be redefined ([AWI-0004](../Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md), [GAP-041](../../Development/EDF_Gap_Register.md)).

Working **WorkflowId** for registry: `edf.governed-maintenance-fast-path` (consumption — **not implemented**).

### 8. Relay and operator projections

GEW instances **SHALL** consume [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) relay boundary semantics. Operator **Attention**, **Next Action**, and workflow-local stepper **SHALL** remain derived ([ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)).

Handover packages **MAY** include `WorkflowId`, `ProfileId`, and effective-configuration **references** for transport; authority remains DWA + canonical F-layer.

## Consequences

### Positive

- Clear placement of GEW and profiles under a multi-workflow framework without implying extra product workflows today.
- GMFP disposition consistent with distinct authorization law while sharing primitives.

### Negative

- SPEC-004 GEW/profile normative hooks are reconciled in **Draft** (published 2026-10-03); runtime/registry gaps remain ([GAP-054](../../Development/EDF_Gap_Register.md), [GAP-027](../../Development/EDF_Gap_Register.md)).
- Runtime registry, resolver, and DWA schema extensions remain open ([GAP-054](../../Development/EDF_Gap_Register.md), [GAP-027](../../Development/EDF_Gap_Register.md)).

### Risks

- Implementers encode PCON-0001 full loop as the only topology — mitigated by this ADR + PCON-0005 YAGNI/stage rules.

## References

- [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md)
- [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [AWI-0004](../Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md), [AWI-0010](../Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md)
