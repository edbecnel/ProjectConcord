# AWI-0010 — Workflow Framework and GEW Architectural Follow-Through

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0010

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-10-03 |
| **Revisit Trigger** | Before M7a workflow runtime; GMFP consumption implementation; material SPEC-004 amendment after 2026-10-03 GEW/GIES consumption publication |
| **Discovery source** | [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) (**Accepted** 2026-10-03), [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03) |
| **Gap** | [GAP-054](../../Development/EDF_Gap_Register.md#gap-054--workflow-framework-runtime-and-effective-configuration), [GAP-027](../../Development/EDF_Gap_Register.md#gap-027--developmentworkauthorization-schema-and-identity) |

---

## Objective

Track **implementation and normative follow-through** for the accepted-in-principle Workflow Framework and Governed Engineering Workflow (GEW) architecture — **without** treating this watch item as authorization to implement.

## Scope

While **Active**, this item:

- **Does** record open work: **M7a-WF-1** + **M7a-WF-1b** + **M7a-WF-1c** backend foundations **published** (**M7a-WF-1c** **closed** at `78bfde9155ee61ece052ac179a3d0cfe24a81dd2`): Migration004/005/006; instance + origin/blocking graph; **GEW v1** effective-configuration resolver (derived; grant-time guard only — not topology/lifecycle/STOP/supersede); durable **DevelopmentWorkAuthorization** grant/supersede ([GAP-027](../../Development/EDF_Gap_Register.md) partial); **WorkflowInstance**-scoped orthogonal **STOP**; recovery/projection extensions (effective-config availability, STOP, applicable DWA kinds — **not** Next Action / actionable frontier); **deferred**: PC-PAR-025 enforcement; synchronization / AEI instance persistence; production relay binding and governed-advancement integration; GMFP registry consumption; **Hierarchical Workflow View** UI ([AWI-0008](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) §B.1); GIES cross-track ([AWI-0011](AWI-0011-Governed-Interactive-Engineering-Session.md)); **PA Exchange** runtime ([§B.2](#b2-project-architect-exchange-contract-deferred--capture-2026-10-03)); **GEW v1 topology** normative docs — [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §2.7, [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §9–§13 ([M7a-WF-1-doc](../../Handover/ProjectConcord-M7a-WF-1-doc-GEW-Topology-Documentation-Tranche-Plan.md) — **published**).
- **Does not** authorize `src/` implementation, A3, or changes to A4/T7/MVR disposition.
- **Does not** supersede [AWI-0004](AWI-0004-Governed-Maintenance-Fast-Path.md) (GMFP consumption semantics remain EDF-owned).
- **Operator UX (deferred):** Future **Hierarchical Workflow View** over the authoritative instance/relationship graph is captured in [AWI-0008 §B.1](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md#b1-hierarchical-workflow-view-deferred--capture-2026-10-03) — **not** part of M7a-WF-1b backend scope; no Desktop visualization in current tranche.
- **Project Architect exchange (deferred):** Preferred **Human ↔ ProjectConcord ↔ Project Architect provider** interaction, provider-neutral **PA Exchange Contract**, transport separation, and canonical request/response identity — [§B.2](#b2-project-architect-exchange-contract-deferred--capture-2026-10-03) (handover captures `8ca11fb9`, `730c89c8`).
- **Conditional alternate synchronization satisfaction (deferred):** Human-selected **conditional alternate satisfaction** of eligible **SynchronizationPoint**s — applications **Conditional Publication Authorization** ([§B.3.1](#b31-conditional-publication-authorization)) and **Conditional Closure Authorization** ([§B.3.2](#b32-conditional-closure-authorization)) — [§B.3](#b3-human-selected-conditional-alternate-synchronization-satisfaction-deferred--capture-2026-10-06) (handovers `c7d8d85d-47c1-4e98-8649-864c5843b920`, `b8e23f19-e813-44e8-9c98-b934b91a0d17`, `e6f8137c-22cb-40f9-b07c-271da098ad86`, documentation `0de6ed2b-24ef-441c-b20d-10ef2c14709d`).

## B.2 Project Architect Exchange Contract (deferred — capture 2026-10-03)

**Handover capture:** `8ca11fb9-65a1-4f7f-895f-c454495691c2`, refined `730c89c8-0fc1-4e38-a9ad-88a96bd21f85` — **documentation only**; **does not** authorize `src/` implementation.

### Preferred interaction model

```text
Human  <->  ProjectConcord  <->  Project Architect provider
```

The human normally submits an ordinary message to **ProjectConcord**, not a hand-built protocol envelope. ProjectConcord composes the **canonical PA Request Package** from: human message; current project; applicable `WorkflowInstance`(s); workflow state; baseline; governed context; correlation/request identity; applicable PA instructions; expected response contract — then relays or prepares it for the configured **PA provider**.

Target operator experience (future): **Ask Project Architect** with optional message type defaulting to **Auto / Not sure**, plus a conversational message field; ProjectConcord supplies protocol and context behind the scenes ([AWI-0008](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) product-shell cross-track).

### Layer separation (workflow semantics independent of transport)

| Layer | Role |
|--------|------|
| **PA Exchange Contract** | Semantic request/response protocol between ProjectConcord and a PA-capable provider |
| **PA Provider** | ChatGPT, OpenAI API, or other future PA implementation |
| **PA Transport** | Manual clipboard relay, browser-assisted relay, API transport, future integrated transport |

**Workflow Framework / GEW semantics SHALL NOT depend on transport mechanism.**

```text
                    PA Exchange Contract
                           |
          +----------------+----------------+
          |                |                |
    Manual Relay     Browser Relay       API Relay
          |                |                |
       ChatGPT          ChatGPT         PA Provider
```

### Manual ChatGPT relay (permanent supported mode)

When ProjectConcord cannot invoke the provider directly: human message in ProjectConcord → canonical PA Request Package → user copies → paste into ChatGPT → human-readable reply plus optional **canonical PA Response/Directive** → user imports into ProjectConcord → **deterministic validation** → proposed governed operations handled per governance. The human must not manually construct the protocol envelope.

### Browser-assisted transport (future investigation)

Browser extension, companion, bookmarklet, or similar **transport adapters only** — may reduce copy/paste. They **SHALL NOT** become authoritative workflow-state interpreters. The PA Exchange Contract must remain usable without them. **No** commitment to ChatGPT DOM or other brittle browser internals.

### API transport (future)

OpenAI/API integration uses the **same conceptual** PA Exchange Contract; may strengthen system instructions, structured output, schema validation, correlation, and parsing — without making GEW semantics depend on API transport.

### Canonical PA request identity (investigation)

Durable/correlatable request identity should eventually carry at least: protocol version; PA request/correlation ID; project identity; applicable workflow instance id(s); baseline/context identity; user-declared message intent (if any); provenance to associate response with request. Exact schema **deferred**.

### Canonical PA response / directive

A PA-aware response may include: **(A)** human-readable explanation; **(B)** machine-readable ProjectConcord response/directive envelope. ProjectConcord **deterministically locates and validates** the structured portion. Human-readable prose **SHALL NOT** alone become authoritative operational state. The directive **proposes** semantic/governed consequences; ProjectConcord validates against authoritative state and governance.

### Preferred vs fallback paths

| Path | Flow |
|------|------|
| **Preferred** | ProjectConcord-generated canonical PA request → PA → canonical PA response/directive → deterministic validation |
| **Fallback** | Arbitrary conversational PA prose → interpretation/classification → proposed operation → clarification and/or human confirmation → governed mutation |

Arbitrary LLM interpretation of conversational prose is **not** the preferred operational path. Interactive clarification may later align with [AWI-0011](AWI-0011-Governed-Interactive-Engineering-Session.md) / GIES — **not** authorized here.

### Optional user-declared message type (hint only)

Future composer may allow optional intent selection (default **Auto / Not sure**): e.g. Question/Advice, Proposed Change, Problem/Issue, New Requirement/Idea, Review/Decision, Clarification, Other — **conceptual** labels; **not** internal workflow mutation primitives.

**UserDeclaredIntent** describes the human’s declared communication intent; it **SHALL NOT** directly authorize workflow mutation. Preserve both **UserDeclaredIntent** and **PAClassifiedIntent** / PA disposition where useful — do not silently collapse. Message type may tailor PA request focus (exact prompts **deferred**).

### Explicit non-goals (current tranches)

**No** authorization for: PA request composer UI; OpenAI/ChatGPT integration; browser extension/bookmarklet/clipboard automation; PA directive parser; PA message classifier; automatic workflow creation; GIES clarification runtime; user message-type selector UI.

## B.3 Human-selected conditional alternate synchronization satisfaction (deferred — capture 2026-10-06)

**Handover / review provenance:** `c7d8d85d-47c1-4e98-8649-864c5843b920` (CPA capture); `b8e23f19-e813-44e8-9c98-b934b91a0d17` (CPA plan review); `e6f8137c-22cb-40f9-b07c-271da098ad86` (CCA investigation plan); `0de6ed2b-24ef-441c-b20d-10ef2c14709d` (documentation authorization) — **documentation only**; **does not** authorize `src/` implementation or amend accepted [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) / [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md). Formal runtime design **deferred**.

### Parent pattern (capture only)

The Workflow Framework **SHALL** eventually support an **optional** pattern: **human-selected conditional alternate satisfaction of an eligible SynchronizationPoint**.

For an eligible **SynchronizationPoint**, the human operator in charge **MAY** elect an **alternate governed satisfaction path** in which predefined objective **EvidenceRequirements**, **Controls**, authorization conditions, and other applicable requirements replace the normally required **interactive PA satisfaction** path for that synchronization requirement.

The governed synchronization requirement is **not** simply skipped. If alternate criteria fail, cannot be established, or become indeterminate, **normal interactive synchronization** is required again (**fail-closed**).

**Conditional Publication Authorization (CPA)** and **Conditional Closure Authorization (CCA)** are **named applications** of this pattern — capability/application labels, **not** new canonical Workflow Framework primitives.

Prefer the term **conditional alternate synchronization satisfaction** over “governance bypass” or “synchronization bypass.”

### Eligibility

**Prescribed workflow definition** and **applicable policy** determine which **SynchronizationPoint**s are eligible for alternate satisfaction and under what constraints.

A runtime **registry** (or other implementation) **MAY** eventually resolve or enforce that information; binding architectural responsibility to a registry at this stage is **not** authorized.

### Human election

The human operator **SHALL** explicitly elect use of an available alternate satisfaction path when it applies.

Alternate satisfaction **SHALL NOT** be silently selected by: Project Architect judgment; Engineering Agent judgment; workflow profile alone; model inference; implementation size; successful tests; apparent simplicity; or “narrow correction” classification.

The election **SHALL** require **governed provenance**. How the election is represented and durably retained **remains unresolved**. This capture **does not** bind election to **DevelopmentWorkAuthorization** or any other persistence record.

### Distinct governance concepts

The following **SHALL** remain distinct (do **not** collapse into DWA):

1. Engineering / bounded-work authorization.
2. Publication authorization.
3. Human election to use alternate synchronization satisfaction.
4. Evidence establishing objective requirements.
5. Closure authority / applicable governed transition.

Successful publication **does not** independently create closure authority.

### Evidence

Alternate synchronization satisfaction **does not** eliminate evidence. Required evidence **SHALL** still be generated, retained, associated with the applicable governed context, and recoverable as required. The architecture changes the **permitted method of satisfying** an eligible synchronization requirement, not the existence of evidence requirements.

### Recovery / provenance (semantic requirements)

ProjectConcord must ultimately recover enough authoritative state to determine:

- whether the human elected alternate satisfaction;
- which **SynchronizationPoint** occurrence/context it applied to;
- whether workflow/policy permitted it;
- which objective requirements applied;
- whether those requirements were satisfied;
- whether normal synchronization became required instead;
- whether publication occurred;
- whether closure remains pending or legitimately completed.

Persistence schema is **not** designed in this capture.

### Workflow Framework primitives

Retain the four existing primitives ([PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §6): **SynchronizationPoint**, **AuthorizedExecutionInterval**, **EvidenceRequirement**, **Control**. **No** fifth primitive is authorized or currently justified.

These primitives **appear sufficient** for this deferred architectural model. Final runtime sufficiency **remains subject** to later formal runtime design.

| Primitive | Role in this pattern (conceptual, non-normative) |
|---|---|
| **SynchronizationPoint** | Identifies the synchronization requirement; default satisfaction = interactive PA path; alternate satisfaction when elected and criteria met — **orthogonally** to topology place ([ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §11) |
| **EvidenceRequirement** | Objective criteria that may satisfy the sync point on the alternate path |
| **Control** | Permitted operations when alternate satisfaction holds (e.g. commit/push; governed closure transition) |
| **AuthorizedExecutionInterval** | May span work between human sync events while alternate criteria are evaluated |

### GEW profiles and GMFP

CPA and CCA are **not** equivalent to Accelerated, Standard, or High Assurance; **not** automatically selected by a profile; and **not** a quality or assurance downgrade.

Future workflow or profile policy **MAY** permit, prohibit, constrain eligible **SynchronizationPoint**s, or impose evidence/control floors. Human election remains a **separate** concept unless later architecture explicitly decides otherwise.

[GMFP](AWI-0004-Governed-Maintenance-Fast-Path.md) remains a **separate prescribed workflow** with EDF semantics. Conditional alternate synchronization satisfaction is **not** implicit GMFP selection and is **not** a substitute for GMFP.

### Workflow rigor follow-through (deferred)

This pattern relates to the deferred investigation of workflow **rigor** / **compression** ([PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) workflow profile; [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) open per-profile sync/evidence floors). That investigation **SHOULD** eventually distinguish at least:

- full interactive synchronization satisfaction;
- human-elected objective alternate satisfaction at declared eligible synchronization points;
- profile/policy-defined minimum synchronization and evidence floors;
- any additional intermediate or specialized governed configurations later found necessary.

Complete rigor architecture is **not** designed here.

### Relation to M7a-WF-1c

**M7a-WF-1c** is **published** and **closed** at commit `78bfde9155ee61ece052ac179a3d0cfe24a81dd2`. Neither CPA nor CCA **governed** that publication, **retroactively alter** **M7a-WF-1c**, or alter accepted **M7a-WF-1c** governance history. See [§B.3.2](#b32-conditional-closure-authorization) for the motivating example only.

### Explicit non-goals (current tranches)

**No** authorization for: runtime enforcement; sync/AEI instance persistence; relay binding; profile binding; UI for election; automatic narrow-change routing; registry-as-eligibility-owner; amendment of accepted PCON-0005 / ADR-0024 / SPEC-004; or operational **Attention** / **STOP** representation for CCA fail-closed (deferred).

### B.3.1 Conditional Publication Authorization

**CPA** — pre-publication application of §B.3.

After PA authorizes a bounded correction, the human **MAY** elect alternate satisfaction of an eligible **pre-publication** **SynchronizationPoint**, allowing commit/push without another interactive pre-publication PA synchronization/review **only while** required validation, bounded scope, predefined conditions, applicable manual verification, and other applicable controls remain satisfied.

**Normal path (conceptual):**

```text
implementation/correction
    ->
validation
    ->
evidence
    ->
PA review
    ->
publication authorization
    ->
commit/push
```

**Optional path with CPA (conceptual):**

```text
bounded correction authorization
    ->
human elects alternate satisfaction (CPA)
    ->
correction
    ->
validation/evidence
    ->
objective alternate satisfaction of eligible pre-publication synchronization
    ->
commit/push
    ->
publication evidence retained/returned
```

The normal interactive PA path remains available and becomes required again when alternate criteria fail or become indeterminate.

**CPA fail-closed:** Alternate satisfaction failure normally occurs **before** publication; **SHALL** return to the normal interactive PA path **without commit/push** when, at minimum: required validation fails; unexpected test regressions; scope expansion beyond the bounded correction; new architectural ambiguity; incomplete required human verification; unexpected or unrelated files in the publication set; repository or baseline drift from authorization; or inability to establish that alternate satisfaction still applies.

Which GEW transition’s pre-publication **SynchronizationPoint** is affected, and evidence timing relative to existing publication-evidence patterns, **remain unresolved**.

### B.3.2 Conditional Closure Authorization

**CCA** — post-publication / pre-closure application of §B.3.

After PA **publication authorization**, the human **MAY** elect alternate satisfaction of an eligible **pre-closure** **SynchronizationPoint**, allowing governed closure when separately authorized/permitted **without** another interactive post-publication PA synchronization/review **only while** predefined objective closure/publication verification criteria are satisfied.

**Normal path (conceptual):**

```text
PA publication authorization
    ->
EA commit/push
    ->
EA verifies publication
    ->
EA returns publication evidence
    ->
PA reviews publication evidence
    ->
PA declares tranche/work closed
```

**Optional path with CCA (conceptual):**

```text
publication authorization
    ->
human elects alternate satisfaction (CCA)
    ->
commit/push
    ->
publication verification/evidence
    ->
objective alternate satisfaction of eligible pre-closure synchronization
    ->
governed closure if separately authorized/permitted
```

**Motivating example (not normative):** The **M7a-WF-1c** publication at `78bfde9155ee61ece052ac179a3d0cfe24a81dd2` illustrated objective publication facts (authorized parent SHA, expected publication commit, successful non-force push, local `main` aligned with `origin/main`, clean working tree, no unrelated files, applicable validation evidence still in force). That case was **not** governed by CCA and is **not** a normative precedent.

**CCA fail-closed asymmetry:** Alternate satisfaction failure **MAY** occur **after** publication. CCA fail-closed behavior **does not** imply rollback or reversal of an already-successful publication. Instead: publication remains a historical/operational fact; closure is **not** automatically declared; the applicable synchronization remains **unresolved**; normal PA review/synchronization is required. Later architecture **MAY** define **Attention**, **STOP**, or other operational representation — **not** designed here.

Candidate revert themes (not normative runtime rules): commit/push failure before closure; force or unauthorized publication mechanism; unexpected parent/baseline or publication contents; unrelated files; dirty working tree when cleanliness required; local/remote divergence; required validation evidence absent or no longer applicable; publication hook/build failure; authorization or election no longer applicable; inability to establish a required closure condition; ambiguity requiring PA judgment.

**Closure multiplicity (open):** “Closure” is **not** yet one universal operation. Future architecture **MUST** distinguish as applicable: **WorkflowInstance** lifecycle completion; governed tranche/work closeout; relay/package/operation closeout; other workflow-specific terminal dispositions. CCA **MUST NOT** silently equate these concepts. **Not** resolved in this capture.

## Promotion path

```text
AWI-0010 (this item)
    -> PA acceptance of PCON-0005 / ADR-0024 (publication)
    -> SPEC-004 GEW reconciliation (published 2026-10-03)
    -> implementation (M7a / separately governed)
```

## Related Documents

- [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md)
- [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)
- [SPEC-004 reconciliation tranche plan](../../Handover/ProjectConcord-SPEC-004-Workflow-Framework-Reconciliation-Tranche-Plan.md)
- [M7a-WF-1-doc GEW topology tranche plan](../../Handover/ProjectConcord-M7a-WF-1-doc-GEW-Topology-Documentation-Tranche-Plan.md)
- [AWI-0008 — Hierarchical Workflow View (deferred UX)](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md#b1-hierarchical-workflow-view-deferred--capture-2026-10-03)
- [AWI-0011 — GIES (clarification / interactive resolution)](AWI-0011-Governed-Interactive-Engineering-Session.md)
