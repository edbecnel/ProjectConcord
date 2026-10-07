[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [ADRs](README.md) › ADR-0026

# ADR-0026: Guided Work Surface, Work Continuity, and Relay Technical Surface

## Status

Accepted

## Date

2026-10-07 (Accepted by Project Architect — partial AWI-0013 promotion; architecture/documentation tranche handover `0191d293`; publication disposition handover `c74997db`)

## Context

Human verification during [MVR-0005](../../Verification/Records/MVR-0005-operator-mvp-real-ui-1-human-interactive.md) demonstrated that operators cannot reliably understand **what work is active**, **what already happened**, or **what meaningful action to take next** when the primary experience is organized around **governed relay mechanics** (session intent, package generation, copy/paste, validation, mode selection) rather than around **meaningful work progression**.

The operator rejected incremental remediation that adds explanatory text to the legacy **Governed Exchange** relay control panel, stating that ProjectConcord should not force an intuitive guided workflow into a surface that was not intended to serve that role.

Prior accepted architecture already anticipates this separation:

- [AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) §B — P0 long scrollable relay panel is **not** acceptable target production UX; workflow-local staged presentation; raw relay via progressive disclosure only.
- [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §5 — workflow-local progression; raw transport **must not** be primary production UX.
- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17 (PC-PAR-023) — stage binding to package legs; PC-PAR-023c — raw transport not primary surface.
- [AWI-0013](../Watch_Items/AWI-0013-Governed-Work-Continuity-Human-Readable-AI-Views-and-Hierarchical-Workflow-Navigator.md) — governance continuity **and** subject-matter continuity; human-readable AI views; Workflow Navigator (deferred scope).

Desktop already implements **human-centered guided exchanges** for bounded flows (Planning Entry, Planning Authorization) while labeling the legacy relay panel **advanced/diagnostic**. Post–Planning-DWA **planning-region continuation** currently routes operators to the **legacy Exchange** as primary — inconsistent with this ADR and with human evidence.

Investigation handover `70f8c80b` classified the gap as **Category B**: partial resolution of AWI-0013 direction is required before meaningful guided-work implementation or resumption of MVR-0005 human execution.

This ADR records **architecture only**. It does **not** authorize `src/` implementation, persistence schema finalization, UI layout, Workflow Navigator implementation, GIES runtime, out-of-band protocol, or attachment architecture.

## Decision

### 1. Two presentation layers (information architecture)

The product shell **SHALL** distinguish two operator presentation responsibilities. These **MAY** coexist in one workspace (primary + disclosed/advanced regions); literal two-screen implementation is **not** required.

| Layer | Name | Role |
|---|---|---|
| **A** | **Guided Work Surface** | Primary **ordinary-operator** experience organized around meaningful work, continuity, governance summary, and **human-meaningful actions**. |
| **B** | **Relay Technical Surface** | Subordinate **advanced / diagnostic / recovery / inspection** experience exposing governed relay mechanics, canonical packages, validation diagnostics, transport, and troubleshooting. |

The **Core Governed Interaction Relay** ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)) remains **authoritative** for package semantics, validation, correlation, provenance, STOP hooks, and consumption rules. The Guided Work Surface **MUST NOT** become a second governance truth.

### 2. Guided Work Surface — responsibilities

The Guided Work Surface **SHALL** help an ordinary operator answer, in human-meaningful terms:

- What are we working on?
- What happened (recent meaningful outcomes)?
- Where are we now (governed position and work progression)?
- Why are we here?
- What authorization exists and what is **not** authorized?
- What is blocked (including **STOP**)?
- What happens next, and why?
- What, if anything, must **I** do now (primary human action)?

It **SHALL** present:

- **Active work focus** (subject/title and progression state — see §4).
- **Work continuity** (recent episode summary and resumption target — see §5).
- **Meaningful human actions** (see §7) that **orchestrate** relay operations where safe.
- **Human-readable AI communication views** where appropriate (see §8) — distinct from continuity narrative.
- Access to **history/context** without requiring reconstruction from package kinds and control labels alone.

It **MAY** reuse patterns already proven in bounded guided exchanges (step titles, gated commands, technical detail under disclosure).

### 3. Relay Technical Surface — responsibilities

The Relay Technical Surface **SHALL** continue to support, without redefining relay authority:

- Canonical governed package generation, render, import, validation, and normalization.
- Session intent (NEW/CONTINUE), Engineering Agent mode and mode transition metadata.
- Correlation identity, provenance listings, validation diagnostics.
- Manual copy/paste transport, correction/recovery paths, automated transport status when integrated.
- MVR/development harness and expert inspection.

It **SHALL** be available for **advanced, diagnostic, recovery, governance inspection, troubleshooting, and fallback** paths. It **MUST NOT** be architecturally designated as the **ordinary primary** guided experience for routine planning-region continuation after Planning DWA (see §10).

### 4. Human-in-the-loop vs protocol mechanics

**Human-in-the-loop** does **not** mean the operator must manually perform every protocol operation.

The Guided Work Surface **MAY** orchestrate relay mechanics behind meaningful human actions (for example preparing outbound content for the Project Architect, transferring content to an external conversation, bringing back a response, preparing Engineering Agent handover) **when** doing so does not hide or fabricate a governance decision.

The following **MUST** remain **explicit** human-visible or human-confirmed operations where governing policy requires them (non-exhaustive):

- Recording or revoking **DevelopmentWorkAuthorization** and other durable **B**-layer authorization outcomes.
- **STOP** acknowledgment or governed handling when policy requires human attestation.
- Import/paste attestation when the operator is declaring completeness of an external response.
- Authorization disposition acceptance when separated from transport.
- Workflow topology or lifecycle mutations requiring explicit consent.
- Mode transition metadata when policy requires prior-mode declaration.

Primary workflow choices **need not** be labeled as “generate package”, “copy package”, or “validate package” merely because the relay service exposes those operations separately.

### 5. Projections are not authorization

Guided Work projections (including recommended next step, primary action emphasis, and human-readable summaries) are **derived presentation** per [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md).

They **MUST NOT**:

- Imply implementation or planning authorization beyond governed **B**-layer state and relay rules.
- Substitute for consumed handover facts or DWA records.
- Override **STOP** or eligibility fail-closed behavior.

### 6. Human-readable summaries and provenance

Human-readable continuity text and human-readable package views are **not** canonical governance truth.

Where summaries are shown, the architecture **SHOULD** preserve provenance class distinctions where applicable:

- Project Architect judgment (from governed packages).
- Engineering Agent report (from governed packages or results).
- Human observation (MVR, explicit operator notes where authorized).
- Governed artifact references (package IDs, correlation IDs).
- Workflow-derived state (topology, DWA presence, eligibility).
- Derived presentation (UI narrative).

Generated or model-produced prose **MUST NOT** become authoritative merely because it appears in the Guided Work Surface.

### 7. Meaningful human actions (illustrative — not frozen labels)

The architecture **SHALL** distinguish:

| Class | Examples (illustrative) |
|---|---|
| **Meaningful human action** | Send work to Project Architect; bring back Project Architect response; send work to Engineering Agent; review outcome; record authorization when policy requires; resume suspended work. |
| **Orchestrated mechanics** | Produce governed review export; render human-readable view; copy to clipboard; parse/normalize paste; prepare handover when eligibility already holds. |
| **Explicit governance decision** | Record Planning/Implementation DWA; acknowledge STOP; confirm authorization disposition; explicit topology transition. |

Exact command labels are product discretion provided semantics remain clear and non-misleading.

### 8. Human-readable AI communication view (partial AWI-0013 resolution)

1. Canonical governed packages remain **authoritative** ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)).
2. The **human-readable AI communication view** is an **alternate read-only projection** of a specific exchange/package.
3. It **SHOULD** be **deterministically derived** from canonical/governed content wherever practical.
4. It **MUST NOT** silently omit governance-significant information in a way that misrepresents the package.
5. Canonical/technical package detail **SHALL** remain inspectable (Relay Technical Surface or progressive disclosure).

**Guided-work continuity narrative** answers: *what are we doing, what happened, what’s next?*

**Human-readable package view** answers: *what does this AI exchange say?*

These are **different** projections. Neither replaces the other.

Renderer implementation is **not** authorized by this ADR.

### 9. Workflow Navigator binding (minimum — full navigator deferred)

The full **Hierarchical Workflow Navigator** remains **deferred** ([AWI-0013](../Watch_Items/AWI-0013-Governed-Work-Continuity-Human-Readable-AI-Views-and-Hierarchical-Workflow-Navigator.md)).

This ADR **SHALL** bind minimum work-continuity architecture so a future navigator can be a **derived projection**:

- `node visible ≠ node mutable` ([AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) §B.1).
- Selection/inspection **MUST NOT** mutate governed workflow state.
- Resume/reopen/mutate **MUST** be separate governed commands, not side effects of selection.
- The navigator **MUST NOT** become an ungoverned workflow editor.

### 10. Post–Planning-DWA planning-region continuation

Ordinary post–Planning-DWA **planning-region** continuation **SHALL** route into a **Guided Work context** (new or extended guided exchange surface) capable of presenting §2 responsibilities.

The legacy **Governed Exchange** relay control panel **MUST NOT** remain the architecturally designated **primary** destination for that continuation.

Minimum responsibilities for that guided context (non-layout):

- Active subject/focus and recent meaningful outcome.
- Governed position and authorization summary (Planning DWA present; implementation not authorized unless **B**-layer state changes).
- **STOP**/blockers.
- Next meaningful work step, reason, and primary human action.
- Optional human-readable exchange content; access to Relay Technical Surface.

Implementation is **separately authorized**.

#### 10.1 Minimum shell placement (first implementation tranche — PA disposition)

For the **minimum** implementation tranche authorized after this ADR is published, the product shell **SHALL** extend or reuse **`OperatorGuidedContext`** with a **`PlanningRegionWork`** guided context (same pattern as existing Planning Entry and Planning Authorization guided exchanges).

The first planning-region guided implementation **MUST NOT** require a **new top-level workspace area** solely for that tranche. Broader shell organization **MAY** evolve later with the Workflow Navigator and multi-work-item architecture.

This records **implementation direction** only; it does **not** authorize `src/` changes in the architecture publication commit.

### 11. Work continuity — minimum conceptual model (design candidates)

Exact persistence schema is **not** frozen by this ADR. The following **conceptual responsibilities** are **accepted** for guided work:

#### 11.1 Active work focus

A durable **active work focus** (per Project, and potentially per workflow instance) **SHALL** support:

| Responsibility | Semantics |
|---|---|
| **Stable work identity** | Identifier distinct from relay correlation alone; survives restart. |
| **Human-readable subject** | Operator-meaningful title/label for “what this work is about”. |
| **Subject provenance** | How the subject was established: operator-declared, inherited from governed artifact, linked reference (MVT/MVR/AWI/spec id), or derived label — **without** making AI prose authoritative. |
| **Anchors** | References to workflow instance, primary correlation/package lineage as applicable. |
| **Progression state** | Meaningful phase (illustrative: awaiting PA, awaiting EA, verification, remediation) — **derived** where possible from relay + workflow facts. |
| **Resumption target** | What the operator should return to after interruption. |

**Authority:** Work focus records are authoritative **for operator continuity and navigation context only**. They **do not** grant DWA, **do not** override STOP, **do not** replace canonical package content, and **do not** silently record governance decisions.

**Subject establishment (PA disposition):** The Guided Work Surface **MUST NOT** silently invent a work subject. Use the strongest available explicit or governed source in this **conceptual priority**:

1. Existing **active work focus** when resuming continuity.
2. An explicit **governed work reference** that already defines the subject (for example active MVR/MVT, feature, specification, AWI, or work-item reference).
3. **Operator-confirmed** subject when no sufficiently trustworthy source exists and subject identity is necessary for meaningful continuation.

A machine-derived human-readable label **MAY** be **proposed** from available governed context; generated prose is **not** authoritative merely because the system or an AI produced it. For example, [MVR-0005](../../Verification/Records/MVR-0005-operator-mvp-real-ui-1-human-interactive.md) and its active verification thread **MAY** anchor subject identity without redundant free-form subject entry when that governed reference is already active.

This disposition defines **semantics**, not an exact schema or UI interaction.

#### 11.2 Work episodes (append-only operational facts)

An **append-only work episode log** (operational **B**/shell layer per [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)) **SHALL** record **meaningful events** with:

- Event kind (open taxonomy — not frozen exhaustive enum in this ADR).
- Timestamp and actor class (human / PA / EA / system).
- **Pointers** to canonical evidence (package IDs, correlation IDs, DWA mutation records, MVR references, publication SHAs where governed).
- Optional short **operator note** flagged non-authoritative.

Illustrative event meanings: work started; PA exchange completed; PA decision recorded; planning authorization recorded; EA work dispatched; EA result returned; publication; human verification observation; failure/remediation; pause; resume; completion.

**Derivation vs persistence (PA disposition):** **Prefer derivation** when meaningful continuity can be reconstructed reliably from existing durable authoritative evidence. **Persist** a continuity or episode fact only when it **cannot** be reconstructed reliably from retained authoritative records and is **required** for meaningful restart, recovery, or resumption.

Implementations **MUST NOT**:

- Duplicate canonical package bodies in episode records.
- Persist every low-level relay operation merely because it occurred.
- Create parallel copies of DWA or workflow truth.
- Turn technical transport noise into human work history.

The episode model records **meaningful work continuity**, not a second relay event log. **Artifact pointers** **SHALL** be used wherever authoritative evidence already exists.

#### 11.3 Canonical artifact pointers

Episode and focus records **SHALL** reference canonical governed artifacts rather than duplicate package bodies.

Appropriate pointer classes include: `GovernedPackageId`, `GovernedCorrelationId`, workflow `WorkflowInstanceId`, DWA operational records, MVR/MVT identifiers, and future PWR/HIW correlations when adopted — without conflating correlation with authorization ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md), [ADR-0017](ADR-0017-Project-Work-Record-Core-Boundary.md)).

#### 11.4 Persistence and recovery (minimum)

Guided work **SHALL** be recoverable after ProjectConcord restart at least to the extent of:

- Active work focus (subject + anchors + resumption target).
- Recent episode tail sufficient to answer “what just happened”.
- Current authorization/blocker projection from authoritative workflow + relay + DWA inputs ([ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15).

Recovery **MUST NOT** reconstruct Next Action from PA/EA chat history alone.

Provider chat restart **does not** replace durable in-ProjectConcord continuity; external chat remains supplementary.

#### 11.5 Future out-of-band (OOB) compatibility

Architecture **SHALL** allow future representation of:

- Primary active work focus.
- **Suspended** primary focus.
- Temporary side work with separate correlation/context.
- Explicit return/resume without contaminating primary workflow ([SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-017).

OOB protocol is **not** defined here. Architecture **MUST NOT** assume a single immutable active work focus for all time without suspend/resume.

### 12. Provider neutrality and workflow rigor

Guided Work presentation **MUST** remain provider-neutral ([ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md)).

The Guided Work abstraction **MUST NOT** hard-code today’s maximum manual P0 relay ceremony as the only human workflow model. Prescribed workflow **profiles** and future rigor/compression levels ([AWI-0010](../Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md), [ADR-0024](ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)) **MAY** vary lower-level ceremony while presenting a **stable meaningful progression** to the operator.

### 13. Relationship to GIES ([ADR-0025](ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md))

GIES interaction hold, routing, and Attention facets **SHALL** compose with Guided Work: active operation context and Awaiting-* states are **projections**, not a replacement for work focus.

GIES runtime implementation remains deferred ([AWI-0011](../Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md)).

### 14. Cancelled remediation direction

Bounded legacy-Exchange progression remediation (investigation handover `531d1df4`) **MUST NOT** be pursued as the primary guided-work strategy.

Incremental context strips on the relay panel **MAY** remain useful on the Relay Technical Surface but **do not** satisfy this ADR.

## Consequences

### Positive

- Aligns product architecture with human verification evidence and AWI-0008 intent.
- Preserves invested relay core, validators, and tests as technical foundation.
- Enables MVR-0005 to resume against a coherent target once minimum guided implementation exists.

### Negative

- Requires new guided surfaces, operational persistence for work focus/episodes, and projection services — separately authorized.
- Legacy Exchange tab semantics must be repositioned in operator mental model and routing.

### Follow-on (not authorized here)

- Implementation tranche: planning-region Guided Work context + persistence APIs — **separately authorized** from the published ADR-0026 baseline; **MUST NOT** be combined with architecture publication in a single commit.
- Human-readable package renderer.
- Workflow Navigator UI.
- SPEC/operator binding refinements beyond PC-PAR-026 (if needed).

## Related Documents

- [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) (amended 2026-10-07 — §5 cross-reference)
- [AWI-0013](../Watch_Items/AWI-0013-Governed-Work-Continuity-Human-Readable-AI-Views-and-Hierarchical-Workflow-Navigator.md) (partial promotion)
- [AWI-0008](../Watch_Items/AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §17 (PC-PAR-023, PC-PAR-026)
- [MVR-0005](../../Verification/Records/MVR-0005-operator-mvp-real-ui-1-human-interactive.md) (human execution paused)
- Investigation: handover `70f8c80b`; architecture tranche: `0191d293`
