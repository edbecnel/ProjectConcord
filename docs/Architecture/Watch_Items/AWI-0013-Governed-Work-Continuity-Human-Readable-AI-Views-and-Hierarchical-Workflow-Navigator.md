# AWI-0013 — Governed Work Continuity, Human-Readable AI Views, and Hierarchical Workflow Navigator

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0013

| | |
|---|---|
| **Status** | Active — **deferred**, **capture-only** |
| **Disposition** | **NOT architectural acceptance** — **NOT implementation authorization** |
| **Owner** | ProjectConcord |
| **Created** | 2026-10-07 |
| **Revisit Trigger** | Before any operator continuity / workflow-navigator / human-readable relay-view tranche; PA request to promote to ADR/specification; material change to operator projection architecture ([ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)) |
| **Discovery source** | Human MVT-5 operator verification (2026-10-07); operator dogfooding observations during Governed Exchange and Engineering Agent mode selection; PA handover `d876dd66-e301-499e-b88f-fc98c6245806` |
| **Related ADRs** | [ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) (operator projections — illustrative only); [ADR-0025](../ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) (governed interaction boundaries) |
| **Related specs** | [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (PC-AIGOV-017 — work outside active workflow); [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) (canonical relay packages) |
| **Cross-reference** | [AWI-0008 §B.1](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md#b1-hierarchical-workflow-view-deferred--capture-2026-10-03) (**Hierarchical Workflow View** — **incorporated and expanded here**, not duplicated); [AWI-0011](AWI-0011-Governed-Interactive-Engineering-Session.md) (GIES — session continuity); [AWI-0012](AWI-0012-Governed-AI-Handover-Attachments.md) (governed attachments — future node/view references); [AWI-0010](AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md) (GEW runtime and operator deferred UX); [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md) (orchestration and external AI) |

---

## Objective

Capture **deferred architectural investigation** for a future ProjectConcord capability that gives operators **human-readable governed work continuity** — including **subject-matter context**, **human-readable views of governed AI communication**, and a **hierarchical Workflow Navigator** — so that guided governed workflow is **more comprehensible** than working directly with separate Project Architect and Engineering Agent conversations, **not less**.

This watch item records **open questions** and **product direction**. It does **not** select data models, persistence schemas, UI frameworks, graph libraries, summarization models, or provider APIs.

## Product-value principle

ProjectConcord should provide value **beyond** acting as a governance wrapper around separate PA and EA conversations.

A principal value proposition should be a **persistent, governed, human-readable model** of the engineering effort that allows an operator to understand, without routinely reconstructing history from separate chats, Git archaeology, or opaque relay payloads:

- what we are building and why;
- what **specific subject** is active;
- what was planned, decided, approved, and authorized;
- what was implemented and verified;
- what failed and what remediation replaced it;
- what remains unresolved;
- where we are now;
- what happens next and why.

**Governance principle:** Additional governance **must not** require the human operator to sacrifice the conversational continuity available when working directly with PA and EA. ProjectConcord should add **durable context, provenance, navigability, and governed continuation** on top of AI collaboration.

## Motivating evidence (human observations — preserve)

During real human verification (MVT-5 context, 2026-10-07), after post-Planning-DWA **Current Work** remediation the operator understood the next action (“I will click on Open Governed Exchange”). At **Governed Exchange**, the operator reported uncertainty: whether to use **Plan** or **Agent**, and more broadly **where we are in the workflow**.

The operator clarified that with PA and Cursor directly they can see what was done last (in chat and in the IDE), infer where work stopped, and ask for guidance — whereas the **guided workflow** left them “pretty much blind” without explicit guidance on what finished and what is next.

Workflow **stage alone** was insufficient: the operator needed the **context of what was being planned or approved** (specific subject matter, not only topology/stage).

The operator proposed a **hierarchical graph in a side panel** with **click-to-inspect** nodes as a compelling reason to use ProjectConcord versus dual-AI operation.

These observations are **architectural/product evidence**. They **motivate** this AWI. They **do not** prescribe that the Workflow Navigator alone fixes the immediate **Plan/Agent** ambiguity; the shortest safe current remediation remains a **separate** investigation (`72c38a7e-7155-45ee-80e4-7593c6ab725b`).

## Central architectural question

How should ProjectConcord **derive, persist, and present** human-readable **governed work continuity** and **subject-matter continuity** — including a **hierarchical Workflow Navigator** and **human-readable AI communication views** — from authoritative governed state and artifacts, without creating parallel sources of truth that can silently diverge?

## Current baseline (explicit non-claims)

While **Active** and **capture-only**:

- Operator continuity today relies heavily on **external** PA/EA conversation history, manual correlation, and partial in-app projections (workflow stage, relay packages, Current Work, etc.).
- **No** canonical work-context model, **no** Workflow Navigator implementation, and **no** first-class human-readable relay view are authorized by this AWI.
- **No** `src/` changes, prototypes, or UI tranches are authorized.

## Scope and non-goals

This watch item:

- **Does not** authorize implementation of the Hierarchical Workflow View deferred in [AWI-0008 §B.1](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md#b1-hierarchical-workflow-view-deferred--capture-2026-10-03) — it **owns the expanded continuity + navigator + human-view direction** and **cross-references** §B.1 rather than duplicating it.
- **Does not** merge into [AWI-0011](AWI-0011-Governed-Interactive-Engineering-Session.md) (GIES) unless future canonical architecture requires it.
- **Does not** define governed attachment identity or transport ([AWI-0012](AWI-0012-Governed-AI-Handover-Attachments.md)).
- **Does not** define the **out-of-band side question** protocol during an active governed workflow (see [Relationship to out-of-band side requests](#relationship-to-out-of-band-side-requests)).
- **Does not** prescribe the immediate fix for Governed Exchange **Plan/Agent** mode selection.

## Capability 1 — Governance / workflow continuity

Future ProjectConcord should present **human-readable** answers derived from **governed state** (not informal guesses), including:

| Question class | Examples |
|---|---|
| Position | Where are we in the governed workflow? |
| Recent completion | What governed operation did we just complete? |
| Authorization | What authorization currently exists? What does **not** exist? |
| Control | Is **STOP** active? What is blocking progress? |
| Continuation | What is expected next? Why is that the next operation? |

This complements structured facets already projected from workflow runtime (e.g. **Waiting On**, workflow **Next Action**) but must not be limited to stage labels alone.

## Capability 2 — Subject-matter / work-context continuity

Workflow stage alone is **insufficient**. ProjectConcord must preserve and present the **actual subject matter** of active engineering work.

Illustrative subject-matter dimensions (non-mandatory schema):

- feature, problem, defect, or decision under investigation;
- specific capability being planned;
- proposal the PA approved or rejected;
- implementation the Engineering Agent was asked to perform and the result returned;
- human observation that triggered remediation;
- verification in progress;
- unresolved questions;
- specific work that should resume next.

Illustrative operator-facing contrast (not a mandated UI):

```text
Insufficient:  "Governed Planning"

Better:        Current subject: MVT-5 verification — Governed Exchange mode/context ambiguity
               Previous completed: Post-Planning-DWA Current Work continuation remediation
               Current operation: Read-only architectural investigation (EA mode selection)
               Next expected: PA review of investigation result
```

## Capability 3 — Human-readable AI communication view

Canonical governed PA/EA communication remains **machine-oriented** and **authoritative** ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) packages and related governed artifacts).

Future architecture should add a **human-readable alternative view** of those exchanges.

**Critical rule:** The human-readable view is **not** a substitute for the canonical governed AI communication. Prefer, where architecture permits, **deterministic derivation** of the human-readable representation from canonical governed data so independent sources of truth cannot silently diverge.

The human view should help answer, where applicable:

- who sent this and who it is for;
- what the exchange is about;
- what was requested; what was decided or reported;
- what was authorized and what was explicitly **not** authorized;
- relevant artifacts/evidence;
- what the operator needs to do next.

The canonical payload remains available for diagnostics, traceability, and machine exchange.

A future UX may distinguish views such as **Human** vs **Technical/Governed** — exact labels are **not** prescribed here.

## Capability 4 — Hierarchical Workflow Navigator

Investigate a **persistent side-panel** hierarchical **graph or tree** representing governed work **history** and **current work**.

This is **more** than a static workflow-definition diagram. It should combine, as projections permit:

- governed workflow structure;
- executed workflow history and current position;
- work-context / subject hierarchy;
- PA/EA exchanges, decisions, and authorizations;
- implementation operations;
- verification, failures, remediations, and resumptions.

The presentation should make the **active node** and historical relationships understandable. Conceptual example (illustrative only — **not** frozen architecture):

```text
Verification
  -> MVT-5
    -> Planning authorization
      -> Post-DWA Current Work defect
        -> Investigation -> Implementation -> Publication -> Human retest
          -> Governed Exchange mode/context ambiguity
```

**Reconciliation with [AWI-0008 §B.1](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md#b1-hierarchical-workflow-view-deferred--capture-2026-10-03):** The authoritative model remains the **workflow instance and relationship graph** ([PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §2.7; [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md)). The navigator is a **derived projection** — not a tree-as-source-of-truth. This AWI **extends** §B.1 with **subject-matter continuity**, **AI communication human views**, **work-episode history**, and **recovery-oriented continuity** — not only `WorkflowInstance` inspection.

### Node inspection

Selecting a workflow node should be **observational**. It may reveal human-readable details such as (where applicable):

- node/work-item subject; purpose/objective;
- governed status; workflow/topology location;
- parent/child or relationship context (presentation may use reference nodes for DAG-like graphs);
- what preceded and followed it;
- PA request/decision; EA request/result;
- relevant authorization and DWA;
- STOP state; human observations;
- artifacts/handovers; verification evidence;
- commit/publication identity; outcome;
- unresolved issues; expected continuation.

Not every node contains every field.

### Read vs mutation

**Critical safety requirement:**

- Selecting or viewing a node **MUST NOT** mutate governed workflow state. Historical exploration must be **safe**.
- Modification or deletion is available **only** when governing workflow/state explicitly permits that operation.
- The navigator **must not** become an ungoverned workflow editor.

Investigate explicit distinction among **inspect**, **navigate/resume**, **modify**, **delete**, and **perform governed action** — commands are **not** defined here.

**Inherited from AWI-0008 §B.1:** `node visible ≠ node mutable`; `node selected ≠ mutation authorized`. Command availability from **governed operation policy**, not display alone.

### Current vs historical context

The operator must inspect **historical** work without losing or silently changing the **currently active** workflow context. The UI should clearly distinguish **active context** from **historical inspection**. Resuming historical work, if permitted, must be an **explicit governed operation** — not a side effect of selection.

## Persistence, recovery, and provenance

Investigate **durable persistence** sufficient for an operator to leave and later return and recover:

- what was being worked on; where work stopped;
- what had just completed; outstanding decisions/questions;
- current authorization; next expected continuation.

This should survive application restart where appropriate, consistent with [AWI-0008 §C](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md#c-durable-governed-relaypackage-working-state) (durable relay/package working state — **cross-reference**, do not duplicate).

Human-readable continuity must retain **provenance**. ProjectConcord should distinguish, where relevant:

- PA judgment; EA report; human observation;
- governed artifact; workflow-derived state;
- automated verification; repository/publication evidence.

Human-readable summaries **must not** erase those distinctions.

## Relationship to GIES / AWI-0011

[GIES](AWI-0011-Governed-Interactive-Engineering-Session.md) intersects this AWI on pause/resume, active operation context, PA question routing, human verification, answer provenance, persistence/recovery, and session continuity. **Do not merge** this AWI into GIES unless canonical architecture clearly requires it. Future design should **reuse** GIES session artifacts where appropriate and **avoid duplicate mechanisms**.

## Relationship to governed attachments / AWI-0012

Workflow nodes and human-readable communication views may eventually **reference** governed attachments ([AWI-0012](AWI-0012-Governed-AI-Handover-Attachments.md)). Attachment identity, storage, integrity, and transport are **out of scope** here.

## Relationship to out-of-band side requests

Separately planned requirement: the operator may temporarily ask a **side question or request** during an active governed workflow without losing the primary workflow, advancing it incorrectly, contaminating correlation/session, or treating the side request as the next formal PA/EA response.

Related normative direction: [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) **PC-AIGOV-017** (capture work outside active workflow without silently modifying it).

The Workflow Navigator / continuity model may provide a place to show **suspended primary context** and **temporary side context**. The out-of-band protocol is **not** defined in this AWI.

## Relationship to current Plan/Agent ambiguity

Governed Exchange **Engineering Agent mode** (Plan vs Agent) ambiguity is **motivating evidence** only. This AWI defines **longer-horizon** direction so isolated UI patches do not accumulate without a continuity model. The paused investigation `72c38a7e-7155-45ee-80e4-7593c6ab725b` remains responsible for the **shortest safe current remediation**.

## Deferred architectural questions (explicit — no premature choices)

The future investigation **should** leave open at least:

| Theme | Open questions |
|---|---|
| Work context | Canonical data model; whether context is a workflow node, work item, operation, episode, projection, or combination; how subject matter is extracted or declared |
| Human-readable summaries | Generation, refresh, canonical vs derived projections |
| Navigator shape | Hierarchy vs DAG vs timeline; branches, retries, superseded work, remediations, resumptions |
| Correlation | PA exchanges, EA exchanges, commits, verification, and workflow nodes |
| Operations | Retention/archival; performance for long-running projects; search/filter/navigation |
| Governance | Provenance display; permissions for node mutation; provider neutrality |
| Scope | Relationship to project roots and multiple GEWs |
| Integration | GIES; out-of-band work; governed attachments |
| Recovery | Restart/crash; operator questions (“What are we doing?”, “What happened?”, “What do I do next?”) and how such requests are **governed** |

**Do not prematurely choose:** persistence schema; database tables; package kinds; exact UI framework; exact graph library; exact node schema; exact summarization model; AI provider; automatic mutation behavior.

## Promotion path

```text
Deferred capture (this AWI)
    -> PA-governed architecture investigation
    -> Proposed ADR(s) / SPEC amendment(s) (separate)
    -> Implementation tranches (separately authorized)
```

## Parent

- [Watch Items](README.md)

## Related Documents

- [AWI-0008 §B.1 — Hierarchical Workflow View (deferred UX)](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md#b1-hierarchical-workflow-view-deferred--capture-2026-10-03)
- [ADR-0020 — Operator projections and workspace navigation](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md)
- [PCON-0005 — Workflow Framework](../PCON-0005-ProjectConcord-Workflow-Framework.md)
- [AWI-0011 — Governed Interactive Engineering Session](AWI-0011-Governed-Interactive-Engineering-Session.md)
- [AWI-0012 — Governed AI handover attachments](AWI-0012-Governed-AI-Handover-Attachments.md)
- [AWI-0010 — Workflow Framework and GEW follow-through](AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md)
- [SPEC-006 — Governed workflow relay](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
