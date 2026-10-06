[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › PCON-0006

# PCON-0006: Governed Interactive Engineering Session Framework

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural framework record |
| **Normative** | Yes |
| **Status** | **Accepted** (2026-10-03 — Project Architect publication) |
| **Record ID** | PCON-0006 |
| **Date** | 2026-10-03 |
| **Owner** | ProjectConcord |
| **Implementation** | **Not authorized** — architecture and documentation only |
| **Companion** | [ADR-0025](ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) (**Accepted** 2026-10-03 — layer allocation) |
| **Workflow framework** | [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) (**Accepted** 2026-10-03) |

---

## 1. Purpose and scope

**Governed Interactive Engineering Session (GIES)** is the ProjectConcord capability for **interactive information, observation, judgment, and authorization exchanges** while a **governed operation** (for example an active Engineering Agent relay leg under **DevelopmentWorkAuthorization**) is in progress.

GIES **SHALL**:

- preserve governed continuity of the parent operation;
- route each need by **required authority** and applicable **prescribed-workflow policy**;
- satisfy needs from **authoritative state** before soliciting humans or the Project Architect;
- record **answer provenance** without making chat transcripts authoritative;
- remain **provider-neutral**;
- compose with [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) primitives and instance state — **not** replace them.

GIES is **framework-level interaction infrastructure**. It is **not** a fifth Workflow Framework primitive, **not** a prescribed workflow, and **not** a provider session identity.

**“Session”** in GIES names the **capability**; it does **not** introduce a canonical runtime identity competing with provider session, workflow instance, governed operation, or relay operation/leg identifiers.

---

## 2. Terminology and conceptual model

| Term | Role |
|---|---|
| **Governed operation** | An in-flight unit of governed work (for example relay leg + workflow instance + **Control** / DWA scope) |
| **Governed Interaction Context** | Correlates one or more interaction sets to a parent governed operation and workflow instance |
| **Governed Interaction Set** | Bounded group of related **Interaction Items** under one context |
| **Governed Interaction Item** | One classified need with routing, status, answer, and provenance |
| **Governed Interaction Hold** | Instance state: parent governed operation **must not progress** past the relevant boundary while **blocking** items remain unresolved |

Conceptual structure (not a mandated object model):

```text
Governed Operation
    |
    +-- Governed Interaction Context
            |
            +-- Interaction Set
                    |
                    +-- Interaction Item(s)
```

An unresolved **blocking** Interaction Set **MAY** place the parent operation into an **Interaction Hold**.

---

## 3. Governed Interaction Hold

An **Interaction Hold** is **workflow-instance infrastructure** at the framework level.

It is **NOT**:

- a Workflow Framework primitive;
- a separate prescribed workflow;
- a provider session;
- a new **DevelopmentWorkAuthorization**;
- equivalent to **STOP**;
- equivalent to the broader project/tranche **pause** in [PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md).

While a hold is active, the affected governed operation **SHALL NOT** progress past the governed boundary associated with the hold until blocking items are **resolved**, **superseded**, **cancelled**, or escalated per §11.

Whether **non-gated** activity may continue is determined by **workflow authorization and policy** — not by bounded configuration.

An Interaction Hold **SHALL NOT** silently widen authorization scope.

---

## 4. Interaction Set and Interaction Item

### 4.1 Interaction Set

An **Interaction Set** groups related items for one context. Sets **MAY** be split by respondent class or dependency; they are **not** a forms engine or survey.

### 4.2 Interaction Item (conceptual fields)

Each **Interaction Item** **SHOULD** support at minimum:

| Aspect | Meaning |
|---|---|
| **Need classification** | See §5 |
| **Required authority / respondent class** | See §7 |
| **Blocking vs advisory** | Blocking items participate in hold; advisory items surface Attention but need not block |
| **Dependencies** | Optional ordering between items |
| **Status** | Open, answered, waived, superseded, obsolete, failed, etc. |
| **Answer / result** | Governed operational record — not raw chat as authority |
| **Provenance** | See §9 |
| **Authoritative backing** | Link to F-layer artifact or deterministic state when applicable |
| **Supersession / obsolescence** | When needs or answers are replaced |

---

## 5. Need classification

Interaction Items **SHALL** distinguish at least:

| Class | Meaning |
|---|---|
| **Information / fact** | Value obtainable from artifacts or Concord state when authoritative |
| **Human observation** | Sensory, UI, or physical facts only a human can supply |
| **Architectural / governance judgment** | PA (or delegated role) interpretation not reducible to lookup |
| **Authorization / decision** | Permission or acceptance with governance effect — **not** mere information |
| **Escalation** | Routed when policy or failure requires higher authority |

These classes are **not** interchangeable.

---

## 6. Source and authority taxonomy

Answer **source** categories (for provenance and pre-resolution) include at minimum:

| Source | Authority weight |
|---|---|
| **Authoritative governed artifact (F)** | Canonical facts in Git/EDF (for example MVR, ADR) |
| **Deterministic ProjectConcord state (A/B)** | Relay disposition, effective configuration derivation inputs, validation state |
| **Policy-derived state** | F-layer or **D** policy outcomes already committed |
| **Project Architect disposition** | Recorded PA judgment — not chat transcript |
| **Human observation** | Attested observation with provenance; may bind to MVR |
| **Human authorization** | Human decision with authority — distinct from observation |
| **Engineering Agent assertion** | **Non-authoritative** until validated/imported |
| **External / provider evidence** | Requires import/evidence rules |

---

## 7. Deterministic pre-resolution

**GIES-1 (Pre-resolution):** Before soliciting PA or human input, ProjectConcord **SHALL** attempt to satisfy the need from applicable **authoritative governed artifacts**, **deterministic ProjectConcord state**, and **authoritative policy-derived state** ([PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) FW-4; [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-075).

ProjectConcord **SHALL NOT** solicit information already available authoritatively.

**Example:** If an MVR already contains the authoritative human observation required by a later Engineering Agent step, ProjectConcord **SHOULD** consume or reference that evidence rather than asking the human to repeat the observation.

This principle does **not** require generalized inference engines or semantic-vector machinery — only governed lookup, correlation, and policy rules.

---

## 8. Routing

**GIES-2 (No fixed chain):** The architecture **SHALL NOT** require the fixed path:

```text
EA -> ProjectConcord -> PA -> Human -> PA -> EA
```

Each Interaction Item **SHALL** be routed according to **required authority** and applicable **prescribed-workflow / policy** rules.

The framework **SHALL** permit:

- deterministic resolution without external interaction;
- direct PA routing;
- **direct human observation** routing where workflow/policy permits;
- PA-mediated human response where interpretation or governance requires;
- explicit **authorization** routing;
- **mixed** sets split by respondent/authority.

**Direct human observation** is permitted when applicable policy establishes that the item is **observation-only**, **no authorization is implied**, and **no PA interpretation** is required before acceptance of the observation.

The framework **SHALL NOT** establish “Standard GEW always allows direct human routing” or “PA must always mediate.” **Prescribed workflow and policy** determine that behavior ([ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) profile semantics remain separately governable).

---

## 9. Authorization is not an answer

**GIES-3 (Authorization boundary):** An authorization Interaction Item may obtain an **authorization decision**, but **answer text SHALL NOT** silently mutate authorization state.

Authorization changes **SHALL** be effected through applicable **Control** / **DevelopmentWorkAuthorization** / workflow mechanisms ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-004).

**Bounded configuration SHALL NOT** determine or weaken authorization law.

GIES **SHALL NOT** resolve whether **Accelerated** GEW may support a combined planning/implementation authorization boundary ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) — **open**).

---

## 10. Provenance and evidence

**GIES-4 (Provenance):** Operational records **SHALL** preserve sufficient provenance to establish:

- which need was resolved;
- source/authority category;
- respondent class where applicable;
- authoritative backing/reference;
- relationship to the parent governed operation;
- supersession/obsolescence where applicable.

Full provider or chat transcript **SHALL NOT** be the authoritative governance record.

Where an observation or decision requires **durable engineering evidence**, it **SHOULD** be **bound or promoted** to the applicable **F-layer** artifact (for example MVR per [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md)). Operational state **SHOULD** reference canonical evidence rather than duplicate competing truth.

---

## 11. Hold survival, resume, and recovery

**GIES-5 (Continuity):** An Interaction Hold **SHALL** survive ordinary application or provider interruption sufficiently to preserve **governed** continuity.

Architectural state **SHOULD** be sufficient to correlate at minimum:

- parent governed operation;
- workflow instance and applicable **ProfileId**;
- **Control** / DWA reference;
- interaction context, set(s), item(s);
- resolved answers and provenance;
- provider-session **reference** (correlation only);
- validation / **STOP** / **Attention** state.

**Provider-session recovery** is a **transport** concern ([ADR-0022](ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md)). **Governance recovery** is reconstruction of hold/context — **not** merely “continue the chat.”

Persistence mechanisms are **not** prescribed here ([ADR-0019](ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); [GAP-055](../Development/EDF_Gap_Register.md)).

---

## 12. Failure and disposition model

Conceptual dispositions (detailed algorithms are **not** prescribed):

| Condition | Typical disposition |
|---|---|
| Unanswered item | **Attention**; optional policy timeout; may **STOP** |
| Malformed answer | Reject; retry/re-request; do not advance operation |
| Conflicting answers | **Attention**; PA if judgment required |
| Conflict with authoritative artifact | Fail-closed; **Attention**; supersession or correction |
| Scope expansion | **STOP** or new authorization — no silent widen |
| Newly discovered risk | Profile/policy re-check → **Attention** |
| Denied authorization | **STOP**; record denial provenance |
| Obsolete need | Supersede item; release hold if no blockers |
| Authoritative answer appears while waiting | Auto-resolve from artifact/state |
| Provider loss | Transport recovery; hold preserved |
| Application restart | Reconstruct hold from operational store |

Reuse **Attention**, **STOP**, supersession, cancellation, retry, and new authorization as appropriate.

**Timeout and waiver** semantics are **workflow/policy** concerns unless a future PA decision establishes a framework invariant.

---

## 13. Relationship to Workflow Framework primitives

GIES **composes** [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) §6 primitives and instance state:

| Primitive | Relationship |
|---|---|
| **SynchronizationPoint** | Declarative requirement; GIES **fulfills** runtime sync/information needs during an interval |
| **AuthorizedExecutionInterval** | Parent operation; hold pauses **forward progress** within the interval |
| **EvidenceRequirement** | Answers may satisfy evidence; observations may feed MVR |
| **Control / DWA** | Authorization items affect **Control** — not informal text |

**Gate** may group items for operator presentation; it is not a separate primitive.

### 13.1 Governed Synchronization Review (reference use case)

[PCON-0007](PCON-0007-Governed-Synchronization-Review.md) (**Accepted**) defines **Governed Synchronization Review** — incremental evidence acquisition and **Review Subject** disposition while satisfying an applicable **SynchronizationPoint**.

GIES **SHALL** supply **interaction mechanics** for that pattern (evidence requests, judgment, hold, provenance, resume). GIES **does not** own review policy, reviewer-controlled frontier rules, or **aggregate SynchronizationPoint satisfaction** (recorded via workflow / **Control** — GIES-3).

Typical mapping: **Review Evidence** acquisition → information/fact or human observation items; **Review Subject** disposition → architectural/governance judgment; aggregate satisfaction or bounded correction → authorization/decision items that **mutate** state only through **Control** / **DevelopmentWorkAuthorization** / workflow governance.

---

## 14. Prescribed workflow policy

Prescribed workflows (including [GEW](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)) determine:

- whether interaction is permitted;
- required respondent/authority;
- whether PA mediation is mandatory for a class of need;
- authorization semantics;
- evidence requirements after resolution;
- permitted routing;
- disposition after resolution.

GEW profile-specific GIES behavior (sync floors, mediation rules) is **not** fully specified in this record — see §15.

---

## 15. GEW / profile boundary (this tranche)

GEW profiles **MAY** later define different synchronization requirements, evidence floors, and mediation requirements — subject to GEW invariants and [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) authorization law.

This record **does not** establish:

- a Standard direct-routing default;
- a High Assurance mandatory PA-review rule;
- an Accelerated authorization shortcut.

Those remain **future normative profile work** where required.

---

## 16. Operator projection (reference)

Operator-facing states (for example **Awaiting Human Input**, **Awaiting PA Input**, **Awaiting Authorization**, **Interaction Conflict**, **Interaction Incomplete**) **SHALL** be **derived projections** per [ADR-0020](ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) — **Current Work**, **Attention**, **Next Action** — not a second workflow engine.

Provider-native question UI is **not** required.

---

## 17. Provider and transport neutrality

**GIES-6 (Provider neutrality):** Provider-native structured Q&A is an **optional adapter** ([ADR-0021](ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md), [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)).

**Prompt conformance is not protocol conformance.** Where interaction crosses relay boundaries, ProjectConcord **SHALL** use deterministic framing, extraction, validation, and fail-closed handling when implemented.

Relay **package schemas** for interaction payloads are **not** defined in this tranche ([ADR-0025](ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md)).

---

## 18. Relationship to PCON-0003 pause

[PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md) addresses **broader** project/tranche pause, continuation identity, and resume reconciliation.

**Interaction Hold** is **distinct**: tied to an active governed operation awaiting interaction resolution.

The two capabilities **MAY compose** but **SHALL NOT** be merged in this tranche. Future architecture **MAY** unify underlying continuation persistence if evidence supports it ([AWI-0002](Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md)).

---

## 19. Framework invariants (summary)

| ID | Invariant |
|---|---|
| **GIES-1** | Pre-resolution from authoritative artifact/state/policy before external solicitation |
| **GIES-2** | Policy-governed routing — no mandatory fixed EA→PA→Human chain |
| **GIES-3** | Authorization changes only through Control/DWA/workflow — not answer text; bounded config does not define auth law |
| **GIES-4** | Operational provenance; no chat-as-authority; promote durable evidence to F-layer when required |
| **GIES-5** | Hold survives ordinary interruption; governance recovery ≠ resume chat |
| **GIES-6** | Provider-neutral; validated relay framing when crossing boundaries |

Lower layers **MUST NOT** weaken [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md) FW-1–FW-7 or applicable Software Development requirements.

---

## 20. YAGNI exclusions

This framework **does not** authorize:

- generic chat platform, ticketing, or survey/forms engine;
- BPM designer or universal approval matrix;
- full provider conversation archive as governance authority;
- provider-specific workflow architecture;
- resolution of Accelerated combined authorization ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md));
- [GAP-054](../Development/EDF_Gap_Register.md) effective-configuration resolver implementation;
- [GAP-027](../Development/EDF_Gap_Register.md) DWA schema implementation;
- [GAP-055](../Development/EDF_Gap_Register.md) GIES runtime/persistence implementation.

---

## 21. Publication disposition

**Accepted** and published on `main` (2026-10-03). **Does not** authorize `src/` implementation, A3, or changes to A4/T7/MVR disposition.

---

## 22. Related documents

- [ADR-0025](ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md)
- [PCON-0005](PCON-0005-ProjectConcord-Workflow-Framework.md), [ADR-0024](ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md), [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [AWI-0011](Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md), [GAP-055](../Development/EDF_Gap_Register.md)
- [PCON-0007](PCON-0007-Governed-Synchronization-Review.md) — Governed Synchronization Review reference use case (§13.1)
- [PCON-0008](PCON-0008-Governed-Work-Disposition-and-Interaction-Consequence.md) — interaction envelope vs authoritative consequences (**Accepted** 2026-10-06)
