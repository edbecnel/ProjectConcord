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

- **Does** record open work: **M7a-WF-1** + **M7a-WF-1b** backend foundations **implemented** (Migration004/005, instance + origin/blocking graph, governed mutation provenance, dependency release/satisfy, recovery, dependency-blocked projection); **deferred**: effective-configuration resolver; DWA ([GAP-027](../../Development/EDF_Gap_Register.md)); actionable frontier; PC-PAR-025 enforcement; STOP/sync/AEI instance persistence; production governed-advancement integration; GMFP registry consumption; **Hierarchical Workflow View** UI ([AWI-0008](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) §B.1); GIES cross-track ([AWI-0011](AWI-0011-Governed-Interactive-Engineering-Session.md)); **GEW v1 topology** — [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) §2.7, [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §9–§13 ([M7a-WF-1-doc](../../Handover/ProjectConcord-M7a-WF-1-doc-GEW-Topology-Documentation-Tranche-Plan.md) — **published**).
- **Does not** authorize `src/` implementation, A3, or changes to A4/T7/MVR disposition.
- **Does not** supersede [AWI-0004](AWI-0004-Governed-Maintenance-Fast-Path.md) (GMFP consumption semantics remain EDF-owned).
- **Operator UX (deferred):** Future **Hierarchical Workflow View** over the authoritative instance/relationship graph is captured in [AWI-0008 §B.1](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md#b1-hierarchical-workflow-view-deferred--capture-2026-10-03) — **not** part of M7a-WF-1b backend scope; no Desktop visualization in current tranche.
- **Project Architect exchange (deferred):** Preferred **Human ↔ ProjectConcord ↔ Project Architect provider** interaction, provider-neutral **PA Exchange Contract**, transport separation, and canonical request/response identity — [§B.2](#b2-project-architect-exchange-contract-deferred--capture-2026-10-03) (handover captures `8ca11fb9`, `730c89c8`).

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
