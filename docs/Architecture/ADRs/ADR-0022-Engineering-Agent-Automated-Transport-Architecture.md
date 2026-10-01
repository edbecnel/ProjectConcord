# ADR-0022: Engineering Agent Automated Transport Architecture

## Status

Accepted

## Date

2026-10-01 (Accepted by Project Architect — canonical documentation tranche; **no** `src/` implementation authorized)

## Context

[GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport) identified the need to select an **automated Engineering Agent transport architecture** that preserves governed relay invariants while enabling future provider plugins. [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) (**Accepted**) defines the **provider-neutral plugin contract** and separates relay render/parse from automated provider behavior. A2 **P0 manual** governed relay is **published** ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)).

[AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md) recorded transport mechanism investigation; primary Cursor documentation now confirms viable **ACP** and headless **CLI** facilities for a **reference** Cursor plugin realization (non-normative — §16).

Binding architecture includes [ADR-0013](ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) (Single-Project Focus; local-first operational state), and [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) (Attention / Next Action).

Physical plugin loading, transport ledger implementation, concrete provider plugins, and **A4** implementation remain **not authorized** by this ADR.

## Decision

### 1. Provider-neutral automated transport capability

ProjectConcord defines a provider-neutral **Engineering Agent automated transport capability** realized through the [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) **provider plugin contract** (conceptual operations; implementation types deferred).

The capability **MUST NOT** be named or specified as Cursor, ACP, MCP, CLI, subprocess, or IDE extension at the provider-neutral boundary.

| Layer | Responsibility |
|---|---|
| **Core (A)** | Governed Interaction Relay — validation, eligibility, provenance, STOP; unchanged |
| **Software Development (B)** | Software relay profile semantics at boundary; unchanged |
| **Application / infrastructure** | Transport **orchestration**; transport **operation** recovery records; plugin selection and invocation |
| **Provider plugin contract (E)** | Automated transport capability — forward, result retrieval, cancel, health, routing-intent mapping |
| **Relay bridge (E)** | `IEngineeringAgentRelayBridge` — **render / parse only** |
| **Concrete provider plugins (E)** | Provider-specific protocols, auth, sessions, subprocess/API/ACP/IDE mechanics |

**`IEngineeringAgentRelayBridge` MUST NOT** absorb ACP, CLI, MCP, provider authentication, or provider session control.

### 2. Outbound governed lifecycle

Automated forward **MUST** preserve the published P0 governance path:

1. Governed package at relay boundary (**A** / **B** as applicable).
2. **[PC-PAR-014](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)** / **PC-PAR-022b** eligibility — **INCOMPLETE**, **RejectedMalformed**, and applicable STOP / **Valid != actionable** rules block **automated** forward (same as P0 manual handover preparation).
3. **`IEngineeringAgentRelayBridge.TryRenderValidatedHandover`** — produce validated **`RenderedHandover`** (byte-equivalent to P0 manual for the same package).
4. **`IGovernedInteractionRelayService.RecordProducedPackage`** for the export package — unchanged provenance semantics.
5. Application/infrastructure creates or updates a **transport operation** record (§5) and invokes the selected plugin automated transport capability with the rendered handover and neutral routing metadata.
6. On forward failure, governed package validation state **MUST** remain unchanged except for optional non-authoritative operator/audit facets ([ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §6).

Orchestration **MUST NOT** delegate eligibility reinterpretation to the plugin. Plugins **MAY** refuse forward defensively; they **MUST NOT** bypass eligibility rules.

### 3. Inbound governed lifecycle

All provider output remains **UNTRUSTED**.

Terminal path:

1. Plugin obtains candidate import text (or explicit not-ready / failure).
2. Application/infrastructure invokes **`TryParseEngineeringResult`** (existing workflow).
3. Package kind **`EngineeringResultImport`**, structural and profile validation, then **`RecordConsumedPackage`**.

Provider completion, ACP **`session/update`** streams, CLI stdout, MCP payloads, or IDE buffers **MUST NOT** bypass this path or become canonical Project records.

Transport operation records **MAY** link a successful consume to **`ResultImportPackageId`**; transport state **MUST NOT** alter relay validation dispositions ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-022d).

### 4. Transport operation identity

A **governed handover package** (`SourcePackageId` / produce event) and an **automated transport operation** are **distinct concepts**.

**MUST NOT** equate **`TransportOperationId`** with **`PackageId`**.

Each transport operation **SHALL** retain at minimum these semantic relationships:

| Concept | Role |
|---|---|
| **`TransportOperationId`** | Stable identity for one logical automated forward cycle |
| **`SourcePackageId`** | Governed handover export package being forwarded |
| **`GovernedCorrelationId`** | Copied from source package — **unchanged** on retry within the same correlation |
| **Attempt** | Monotonic retry counter within an operation or explicit new operation per policy (§8) |
| **Provider session hint** | Optional opaque handle — noncanonical (§11) |
| **Result linkage** | Optional reference to consumed import package after successful parse |

**One** `SourcePackageId` **MUST NOT** receive concurrent active forward attempts for the same transport operation. A **new** transport operation or explicit new attempt **SHALL** be used for retry after unknown or ambiguous provider state (§8).

Exact enum names, database schema, and persistence types are **deferred**; §5–§9 specify required **semantic distinctions and invariants** only.

### 5. Transport operation lifecycle semantics

Implementation **SHALL** distinguish at least the following **semantic states** (names illustrative):

| Semantic distinction | Meaning |
|---|---|
| **Created / not yet forwarded** | Operation recorded; safe to cancel before provider acceptance |
| **Forward in progress / acknowledged** | Provider accepted work; duplicate forward prohibited for this operation |
| **Forward failed** | Neutral failure; package validation unchanged |
| **Awaiting result** | Provider may still produce output |
| **Result candidate received** | Untrusted import candidate available — **not** governance acceptance |
| **Import completed / rejected** | Terminal after **`TryParseEngineeringResult`** / consume path |
| **Cancelled** | Transport cancel acknowledged — **not** STOP |
| **Timed out** | Neutral timeout |
| **Ambiguous** | Crash-after-send or unknown provider state |
| **Closed** | Terminal; no further automated action without new operation / operator policy |

States **MUST NOT** manufacture **INCOMPLETE**, **RejectedMalformed**, DWA, STOP clearance, or package authority.

### 6. Idempotency, duplicate, replay, and substitution

| Concern | Rule |
|---|---|
| Duplicate forward | At most one active forward per **`TransportOperationId`**; new forward requires new attempt or new operation per §8 |
| Duplicate result | Detect duplicate import for same operation (e.g. content hash / package id); default **reject** second completion |
| Stale / late response | Discard candidates after operation **Closed** / **Cancelled** with audit |
| Crash after send | On restart, prefer **Ambiguous** or **Awaiting result**; **do not** infer governance outcome |
| Retry after unknown state | **Operator confirmation required by default** before retry |
| Repeated provider completion | First successful governed import wins; duplicates logged |
| Provider substitution | **MUST NOT** rewrite governed packages, correlation history, or validation dispositions ([ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §8); new plugin **MAY** require new provider session and new transport operation |

### 7. Restart and recovery

| Scenario | Architectural behavior |
|---|---|
| Restart before forward | Operation may proceed or cancel |
| After forward, before provider ack | Recover via ledger + plugin; may become **Ambiguous** |
| While provider working | **Awaiting result**; poll/resume via plugin + optional session hints |
| Provider done, before import | **Result candidate received** — see §10 |
| Lost provider session | New provider session; **`AgentSessionIntent.Continue`** mapping; Attention |
| Auth expired | Plugin reports failure; Attention; P0 recommended when eligible |
| Stale provider session | Plugin rebind or new session; hints noncanonical |
| Ledger recovery | Rebuild Attention from transport records + governed packages — **never** infer acceptance from provider alone |

### 8. Cancellation, timeout, and retry

**Provider cancellation** is a **transport** operation. It is **NOT** relay **STOP**, rejection, governance reversal, or package invalidation.

Orchestration **SHALL** support neutral reporting of: cancellation requested, cancellation acknowledged, cancellation failed, timeout.

**Automatic retry** **MUST NOT** occur without an explicit idempotency guarantee and architectural policy; default policy: **operator confirmation** when prior state was **Forward acknowledged**, **Awaiting result**, or **Ambiguous**.

### 9. PLAN / AGENT / DEBUG routing intent

**PLAN**, **AGENT**, and **DEBUG** remain provider-neutral ProjectConcord routing intent ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §9–11).

Provider plugins **SHALL** map routing intent to provider capabilities **without** escalating authority (no DWA, no STOP override, no expanded scope).

**DEBUG:** Plugins **SHALL declare** whether they support a **semantically adequate** mapping for neutral **DEBUG** intent. If unsupported, **automated DEBUG transport = unsupported**; ProjectConcord **MUST** preserve fallback including **P0 manual** relay where applicable. **DEBUG MUST NOT** silently degrade to another routing intent (for example **AGENT** or **Ask**).

Reference **Cursor** products expose **Agent**, **Plan**, and **Ask** — not a distinct **DEBUG** mode; reference mapping for **PLAN** / **AGENT** only is documented in §16. **DEBUG** reference mapping is **not** defined as **Agent** or **Ask**.

### 10. Result candidate recovery after restart (AF-1)

When transport persistence indicates **result candidate received** after ProjectConcord restart, **operator confirmation is required by default** before invoking **`ImportEngineeringResult`**.

Transport recovery **MUST NOT** imply governance acceptance.

A future explicit user-configured auto-import policy **MAY** be authorized in a separate tranche; it is **not** part of the initial default architecture.

### 11. Provider session hints (noncanonical)

Opaque provider session handles (for example Cursor/ACP session identifiers) **MAY** be mirrored in ProjectConcord **operational persistence** as **noncanonical restart hints**, keyed by Project ID and linked to **`TransportOperationId`** / correlation.

They **MUST NOT** appear as canonical relay identity in Core provenance ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-021).

If hints disappear, ProjectConcord **MUST** remain reconstructible from governed packages and **`RelaySessionContinuity`**.

### 12. MCP versus governed relay transport

Two responsibilities **MUST** remain separate:

| Channel | Role |
|---|---|
| **A — Governed Engineering Agent relay transport** | Automated transport capability — handover forward and result retrieval for the import pipeline |
| **B — Tool / context integration** | MCP and equivalent — repository context, ProjectConcord tools, controlled external resources where providers invoke **external** tools |

MCP **MUST NOT** be the **primary** governed relay transport architecture merely because a provider supports MCP.

MCP tool output **MUST NOT** automatically become **`EngineeringResultImport`**. MCP **MUST NOT** issue DWA, override STOP, bypass relay validation, manufacture package authority, or redefine canonical Project state.

MCP **MAY** appear inside a concrete plugin’s provider workspace configuration; that use is **orthogonal** to governed package authority.

### 13. P0 manual fallback

P0 manual relay (copy/export, paste/import) **SHALL** remain a permanent overlay sharing eligibility, renderer, parser, validation, and provenance with automated transport.

Automated failure **MUST NOT** damage governed package validation state. When automated transport is unavailable, unsupported, failed, or ambiguous, **P0** remains viable ([ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §13).

### 14. Operator projections (ADR-0020)

Per [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) and [ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §11:

Authentication failure, provider unavailable, transport initialization failure, forward failure, result retrieval failure, and ambiguous in-flight operation **SHOULD** appear in **Attention**.

When P0 manual relay remains eligible, manual governed relay **SHOULD** appear as **Recommended** Next Action — **not** **Required**.

Transport failure alone **MUST NOT** manufacture a governance-**Required** action. No parallel provider workflow lifecycle **SHALL** compete with governed relay lifecycle.

### 15. Security

Provider credentials **MUST** remain outside Core/B canonical state ([ADR-0021](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §9).

Architectural requirements include: trustworthy provider executable discovery; **Project Root**-scoped working directory; stdio/log separation for import candidates; secret exclusion from transport ledger and relay packages; auditable permission decisions.

**ACP permission policy (architectural default):** **Deny-by-default** for scope or authority expansion. For an explicitly eligible operation requiring a provider tool permission, **`allow-once`** **MAY** be used per operator/orchestration policy. **`allow-always`** **MUST NOT** be the ProjectConcord architectural default.

Provider permission approval is **operational permission only** — not DWA, STOP override, governance authority, or package acceptance.

Plugin loader, supply-chain, and sandboxing remain **deferred** ([GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary)); this ADR **does not** reopen GAP-030.

### 16. Reference realization — Cursor (non-normative)

The following is **reference evidence only** for a first **Cursor** provider plugin. It is **not** normative for the provider-neutral contract.

**Preferred reference path:**

```text
Application/infrastructure transport orchestration
  → Cursor provider plugin
  → ACP client
  → Cursor CLI `agent acp` (stdio, newline-delimited JSON-RPC 2.0)
```

Primary Cursor documentation describes ACP facilities including **`initialize`**, **`authenticate`**, **`session/new`**, **`session/load`**, **`session/prompt`**, streamed **`session/update`**, **`session/request_permission`**, and **`session/cancel`**, with modes including **Agent**, **Plan**, and **Ask** ([Cursor CLI ACP](https://cursor.com/docs/cli/acp), retrieved 2026-10-01).

**Reference mappings (non-normative):** neutral **PLAN** → Cursor **Plan**; neutral **AGENT** → Cursor **Agent**. No canonical **DEBUG** → Cursor mapping (§9).

**Headless CLI** (`agent -p` / non-interactive print mode per [Using Agent in CLI](https://cursor.com/docs/cli/using)) **MAY** be used **inside** the Cursor plugin only as secondary, compatibility, fallback, or diagnostic mechanism — **not** as the ProjectConcord semantic transport contract.

### 17. Single-Project Focus

Automated transport operations **MUST** scope to the **active ProjectConcord Project ID** ([ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)). Provider sessions **MUST NOT** determine which Project is authoritative.

### 18. Artifact ownership

Transport mechanisms **do not** redefine artifact ownership. Large payloads **SHOULD** use governed references; canonical ownership remains EDF / existing ProjectConcord architecture. Provider-generated files **MUST NOT** become canonical without **`EngineeringResultImport`** validation.

### 19. A4 and implementation

**A4** implementation, **A4** implementation plan, plugin hosting loader, transport ledger **implementation**, Cursor ACP/CLI integration, MCP server/client implementation, and persistence schema implementation remain **not authorized** by this ADR.

## Consequences

- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §11 reconciled (PC-PAR-022c, PC-PAR-022d).
- [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport) **closed / resolved** by this ADR (architecture only — not implementation).
- [AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md) **closed / satisfied**.
- Future implementation requires separate PA authorization for plugin hosting **implementation**, transport persistence, provider plugins, and A4.

## Related Documents

- [ADR-0021 — Engineering Agent provider plugin contract](ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md)
- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [ADR-0020](ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md), [ADR-0019](ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)
- [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport), [AWI-0006](../Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)
- [A2 Implementation Plan](../../Handover/ProjectConcord-A2-Implementation-Plan.md) (P0 published)
- [ADR-0023 — Engineering Agent plugin hosting and registration architecture](ADR-0023-Engineering-Agent-Plugin-Hosting-and-Registration-Architecture.md) (**Accepted** 2026-10-01)
