# ProjectConcord A4-T6 — Corrective Remediation Notes (local / uncommitted)

**Authorization:** A4-T6 corrective remediation (post–MVT-7 failure); **not** a governed MVR retry or T7 `src/` authorization.  
**Baseline HEAD:** `eecf0538bd4623d837c93d06439a39b2b6c0abe0` (`main`, synced with `origin/main`).  
**A4-T7:** **Blocked on external Cursor ACP provider capability** — see [closeout preparation](#a4-t6--t7--closeout-preparation-pa-2026-10-02).  
**A4-T6 (local):** **Implementation complete** for required model-selection contract; **live required-configuration verification blocked** on Cursor ACP `2026.09.28-64d2043`. **Not** published or finally closed.  
**Commit / push:** **None** (per STOP).

| Item | Status |
|------|--------|
| Corrective Remediation #6 | **PA ACCEPTED** (2026-10-02) |
| Release validation (local tree) | **Build 0 errors**; **298 tests passed** / 0 failed |
| Automated result profile | **THIN** (authority-safe) |
| Cursor model implementation | **PA ACCEPTED** (default Composer 2.5, `fast=false`) |
| Cursor ACP live non-fast | **PROVIDER CAPABILITY BLOCKER** |

## Root cause (MVT-7)

Published T6 `CursorAcpClient` sent an invalid ACP `initialize` payload (`clientName` / `clientVersion` only). Live `agent acp` requires `protocolVersion`, `clientCapabilities`, and `clientInfo` per [Cursor CLI ACP](https://cursor.com/docs/cli/acp). Live failure before remediation: missing/invalid `protocolVersion`.

## Source remediation (summary)

| Area | Change |
|------|--------|
| `CursorAcpProtocol.cs` | Wire helpers: `protocolVersion: 1`, initialize/authenticate/session/prompt shapes |
| `CursorAcpSessionContext.cs` | `cwd` for `session/new` from environment |
| `CursorAcpClient.cs` | Correct initialize/authenticate; parse negotiated `protocolVersion`; session/prompt/update alignment; I/O timeout on requests |
| `EngineeringAgentTransportOperatorAttentionContributor.cs` | Emit **last transport result** Attention before runtime readiness (narrow diagnostic visibility) |

## Live CLI verification (2026-10-02, post–`agent login`)

| Step | Outcome |
|------|---------|
| `agent --version` | `2026.09.28-64d2043` |
| `agent status` | **Logged in** (human + remediation shell) |
| ACP `initialize` (corrected payload) | **Accepted** — `result.protocolVersion: 1`, `agentCapabilities`, `authMethods` includes `cursor_login` |
| Parse initialize result | **OK** in `CursorAcpClient` / contract tests |
| ACP `authenticate` `{ "methodId": "cursor_login" }` | **Error** — `Internal error` / `[unauthenticated] Error` despite CLI logged-in status |
| `session/new` without successful auth | **Error** — `Failed to initialize session services` |
| Provider `InitializeAndAuthenticateAsync` | **Does not complete** — returns `AuthenticationUnavailable` after successful initialize |
| Next wire step (`session/new`) | **Not exercised** on success path (blocked at authenticate) |

**PA disposition:** Further provider/protocol changes beyond the scoped **initialize** contract fix are **not** applied here. Authenticate vs CLI login mismatch requires Project Architect / Cursor CLI reconciliation before additional patching.

## Authentication boundary (MVR / procedure)

- Cursor CLI authentication is a **human-controlled environment prerequisite** (`agent login`); ProjectConcord must **not** automate interactive browser account login or store human credentials.
- **No API key** introduced for this remediation.
- **`agent login` + `agent status` (logged in)** establishes the CLI prerequisite but is **not sufficient** for live ACP `authenticate` success in this verification environment.

## Post–MVT-6 Desktop restart (operator procedure)

After quit/relaunch of Desktop on the disposable MVR root:

1. Re-open the same disposable project root.
2. **Re-import** the PA handover (in-session PA import state is **not** persisted).
3. **Re-prepare** relay eligibility before forward.
4. Confirm sqlite provider preferences for `cursor-acp-reference` remain (MVT-6).
5. Do **not** treat MVT-7 as Pass until governed forward succeeds under PA rules.

## Tests

| Item | Value |
|------|--------|
| New | `CursorAcpInitializeContractTests` (3) |
| Updated | `EngineeringAgentA4T6CursorProviderTests`, `CursorAcpNdjsonCodecTests` |
| Live | `[Trait("RequiresCursor")]` `LiveCursorAcp_InitializeHandshake_Succeeds` — passes on initialize; tolerates `AuthenticationUnavailable` on auth |

## Release validation (with remediation, uncommitted)

| Item | Result |
|------|--------|
| `dotnet build -c Release` | **0** errors; **NU1903** warnings unchanged |
| `dotnet test -c Release` | **252** passed, **0** failed (Application **185**, ProjectServices **56**, Desktop **11**) |

## ACP authentication reconciliation (2026-10-02, diagnostic-only)

**Disposition: C — environment / credential-scope (effective API session invalid; not a ProjectConcord wire delta).**

### `agent status` (no auth state change)

```
✓ Login successful!
Logged in (unable to fetch user details)
```

### TEST 1 — documented minimal flow (standalone)

Tool: `/Users/edbecnel/tmp/ProjectConcord-A4-ACP-Diagnostic/acp_documented_minimal.py` (not ProjectConcord).  
Agent: `~/.local/bin/agent` **2026.09.28-64d2043**.  
Disposable cwd: `/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A`.

| Step | Result |
|------|--------|
| `initialize` | **Success** — `protocolVersion: 1`, `authMethods` → `cursor_login` |
| `authenticate` `{ "methodId": "cursor_login" }` | **Error** — `-32603` / `data.details`: `[unauthenticated] Error` |
| `session/new` | **Not sent** (blocked at authenticate) |

No extra stdout notifications between initialize and authenticate error. `stderr` empty.

### TEST 2 — ProjectConcord wire equivalence

DotNet probe mirroring `CursorAcpNdjsonCodec` + `CursorAcpProtocol` (`/Users/edbecnel/tmp/ProjectConcord-A4-ACP-Diagnostic/Program.cs`):

| Wire line | Payload |
|-----------|---------|
| initialize | `{"protocolVersion":1,"clientCapabilities":{"fs":{"readTextFile":false,"writeTextFile":false},"terminal":false},"clientInfo":{"name":"ProjectConcord","version":"A4-T6"}}` |
| authenticate | `{"methodId":"cursor_login"}` |

**Same outcomes** as TEST 1 (initialize OK, authenticate `[unauthenticated] Error`).

**Non-differences (relevant):** executable `agent acp`; inherited env; JSON-RPC NDJSON framing; request id sequencing; authenticate params; initialize field set matches docs.  
**Minor deltas (not on failure path):** `CursorAcpSubprocessTransport` does not set `WorkingDirectory` (inherits parent); `session/new` in ProjectConcord adds `mode` (only after auth). ProjectConcord does not consume `stderr` during requests (no stderr observed here).

### Environment / auth storage (no secret contents)

| Context | `HOME` | `PATH` includes `~/.local/bin` | Config locations (existence only) |
|---------|--------|--------------------------------|-----------------------------------|
| Human / diagnostic shell | `/Users/edbecnel` | yes | `~/.cursor`, `~/Library/Application Support/Cursor`, `~/.local/share/cursor-agent` |
| `agent acp` child (inherited env) | `/Users/edbecnel` | yes | same |
| ProjectConcord Desktop (inferred) | same user home on macOS | may be shorter unless `PROJECTCONCORD_CURSOR_AGENT_PATH` set (MVR used full path) | same on-disk stores |

`CURSOR_API_KEY` unset. `agent-cli-state.json` top-level keys: `hasClearedLegacyStatsigFields`, `version`, `workerIdsByDisplayName`. `cli-config.json` includes `authInfo` (contents not inspected).

### Credential validity cross-check (non-ACP)

From MVR disposable root:

```
agent -p --trust --output-format text "Reply with exactly: OK"
→ Your stored authentication is invalid. Please log in again.
```

So **`agent status` “logged in” is not authoritative** for API/ACP use in this environment.

### Smallest human/environment correction (no ProjectConcord `src/` change)

1. `agent logout` (optional, if PA/operator approves auth state change).
2. `agent login` until **`agent status` / `agent whoami` can fetch user details** (not only “unable to fetch user details”).
3. Gate: `agent -p --trust` in disposable root succeeds without “invalid authentication”.
4. Re-run documented ACP sequence (TEST 1); only then re-evaluate governed forward / MVT-7.

### Source change implication

**No additional ProjectConcord source change** is indicated for this failure mode. Initialize remediation remains valid in principle; auth blockage is **environment/Cursor CLI credential validity**, not wire incompatibility.

## Post-reauthentication ACP verification (2026-10-02)

After human `agent logout` / `agent login`, `agent status` reports **Logged in as** (account email redacted in operator record). Human attested `agent -p --trust` → **AUTH_OK**.

### Standalone documented flow (retest)

| Step | Result |
|------|--------|
| `initialize` | **Success** — `protocolVersion: 1` |
| `authenticate` `{ "methodId": "cursor_login" }` | **Success** — `result: {}` |
| `session/new` (`cwd` = MVR Root-A, `mcpServers: []`) | **Success** — `sessionId` returned (opaque id; modes/models metadata present) |

No `session/prompt` sent.

### ProjectConcord provider-level (not Desktop Forward)

| Check | Result |
|-------|--------|
| `[RequiresCursor]` `LiveCursorAcp_InitializeHandshake_Succeeds` | **Pass** (~2s) — initialize + authenticate |
| `CursorEngineeringAgentProviderPlugin.InitializeAsync` (tmp probe) | **Pass** — `InitSucceeded=True`, `HealthInitialized=True`, `HealthAuthenticated=True` |
| `session/new` during `InitializeAsync` | **Not invoked** (by design; occurs on forward / `RunPromptAsync`) |

### Additional source defect

**None discovered** in this continuation.

## MVT-7 Retest #1 forward stall investigation (2026-10-02)

**PA disposition:** MVT-7 Retest #1 **Fail**; A4-T7 **paused for remediation**; publication **not authorized**. Diagnosis only — no `src/` changes.

### A. Post-`ForwardInProgress` production call path

1. `EngineeringAgentTransportOrchestrator.ForwardGovernedHandoverAsync` — after `TryPersistForwardInProgress`, builds `EngineeringAgentForwardRequest` (no project filesystem path).
2. `selection.ResolvedPlugin.Forward(forwardRequest)` — **synchronous**; blocks calling thread.
3. `CursorEngineeringAgentProviderPlugin.Forward` — `RunPromptAsync(...).GetAwaiter().GetResult()` (**sync over async**).
4. `CursorAcpClient.RunPromptAsync` — `session/new` (if needed) → `session/prompt` → `CollectPromptUpdatesAsync`.
5. `CursorAcpSubprocessTransport` — `agent acp` stdio; stderr not read during reads (can fill on some OS paths).
6. Lifecycle DB update to `ForwardAcknowledged` / `ForwardFailed` / etc. occurs **only after `Forward()` returns** to the orchestrator.

**Threading (Retest #1 Desktop):** `RelayWorkflowViewModel.ForwardAutomatedHandoverAsync` uses `ConfigureAwait(true)`; orchestrator resumes on **UI thread**; sync `Forward()` blocks UI until the full ACP prompt path completes or times out.

### B. First blocking point explaining Retest #1

**Primary (implementation):** After `session/prompt`, `CollectPromptUpdatesAsync` waits for `session/update` with `prompt_complete` / `end_turn`. Live Cursor can complete via **`session/prompt` JSON-RPC `result.stopReason: end_turn`** while streaming `session/update` chunks **before** that response. `SendRequestAsync` **discards** intervening notifications; they are not buffered for `CollectPromptUpdatesAsync`. Standalone trace (Root-A, harmless prompt): **12× `session/update`** then **`jsonrpc_response` `stopReason: end_turn`** — **no `prompt_complete` update**. Client then waits on an empty stream until **`_ioTimeout` (default 5 minutes)**.

**Secondary (implementation):** `SendRequestAsync` ignores notifications and **does not handle inbound JSON-RPC server requests** (`method` + `id`, e.g. `session/request_permission`). Unanswered server requests can stall the matching `SendRequestAsync` until I/O timeout. Governed handover may trigger tools/permissions; simple diagnostic prompt did not.

**Tertiary (UX/projection):** UI Attention / status refresh runs **after** `ForwardGovernedHandoverAsync` returns — during blocking `Forward()`, operator sees **stale** `initialization-failure` readiness Attention.

### C–D. Standalone ACP `session/prompt` diagnostic (Root-A)

Script: `/Users/edbecnel/tmp/ProjectConcord-A4-ACP-Diagnostic/acp_prompt_trace.py` (answers permission requests if present).

**Message order observed:**

1. `initialize` → response  
2. `authenticate` → response  
3. `session/new` → response (`sessionId`)  
4. `session/prompt` → **12× notification `session/update`** (`session_info_update`, `available_commands_update`, `agent_thought_chunk`, `agent_message_chunk`, …)  
5. **`jsonrpc_response` id=prompt** with `result.stopReason: end_turn`  
6. No further stdout lines within 15s drain  

### E. `session/new` `cwd`

| Item | Finding |
|------|---------|
| Source | `CursorAcpSessionContext.FromEnvironment()` → **`Environment.CurrentDirectory`** (`CursorAcpSessionContext.cs`) |
| Retest #1 expected cwd | **`/Users/edbecnel/Development/GitHub/ProjectConcord`** (Desktop launched via `dotnet run` from dev repo) |
| Governed disposable root | **`/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A`** |
| Authoritative root earlier in pipeline | `IProjectWorkspaceService.CurrentRoot` / `ProjectRoot` when project open — used by P0/Tier0 paths; **not passed** on `EngineeringAgentForwardRequest` |
| Contract gap | Neutral forward request carries package/correlation/continuity but **no workspace path**; orchestrator knows `ProjectConcordProjectId` only. Supplying correct ACP `cwd` requires **contract + hosting injection** (or provider-neutral session context port), not orchestrator Cursor specifics. **No change in this investigation.** |

### F. Attention `initialization-failure`

Maps to `PluginNotInitialized` readiness when host has not marked plugin initialized **or** projection not refreshed. During Retest #1, persistence proves host **did** initialize before `ForwardInProgress`. Visible `initialization-failure` is **projection/coherency**: `RefreshOperatorProjections` runs only **after** blocked `Forward()` completes; readiness Attention from **before** forward can remain. **Not** evidence that Retest #1 failed at the original ACP initialize defect. Recoverable-transport Attention for `ForwardInProgress` may be suppressed by `DeduplicateByCode` if not shown.

### G. `ForwardInProgress` durability (T4)

- Row is **recoverable** (`ListRecoverableTransportOperations` includes `ForwardInProgress`).
- `TransportOperationRecoverySemantics.ClassifyDisposition` → **`ProviderReconciliationRequired`** (not auto-resume).
- `AssessOperation_ForwardInProgress_NotSafeToResume` — **no automatic redispatch** on assess/discover (`EngineeringAgentA4T4RecoveryTests`).
- Recovery service is **explicitly invoked** — **no startup auto-forward** (A4-T4 notes).
- On restart: operation remains **`ForwardInProgress`** until operator reconciliation or future authorized action; **does not** auto-complete forward or import.

### H. Defect classification

| Class | Applies |
|-------|---------|
| **Implementation defect** | **Yes** — ACP prompt completion handling (`CollectPromptUpdatesAsync` vs `stopReason`; notification discard in `SendRequestAsync`; optional server-request handling) |
| **Contract defect** | **Yes** — missing governed **workspace path** for provider session `cwd` |
| **Projection defect** | **Yes** — stale Attention/status while sync forward blocks UI |
| **Environment** | **Partial** — wrong `cwd` may worsen trust/tool behavior; not sole explanation for stall |

### I. Smallest architecturally correct remediation boundary (PA authorization required)

1. **Provider-internal ACP adapter (`CursorAcpClient`)** — treat `session/prompt` RPC completion (`stopReason`) as terminal; buffer/handle `session/update` across RPC phases; handle inbound server requests during read loops; do not discard permission requests.  
2. **Provider hosting** — construct `CursorAcpSessionContext` from **opened Project Root** at init/forward (injected via neutral host/session context, not orchestrator Cursor knowledge).  
3. **Desktop/orchestration (separate tranche)** — avoid blocking UI on long sync `Forward()`; refresh operator projections while forward in flight.

---

## A4-T6 Corrective Remediation #2 (2026-10-02, local / uncommitted)

**Authorization:** PA-authorized **Remediation #2** only — message-pump + governed workspace context. **Not** MVT-7 Retest #2, **not** governed Desktop Forward, **not** MVT-8+. **No commit / push.**

### Root causes addressed

| # | Defect | Remediation |
|---|--------|-------------|
| 1 | ACP `SendRequestAsync` discarded `session/update` during outstanding RPC; completion waited on `prompt_complete` after live `stopReason: end_turn` | Message-pump read loop: accumulate notifications in `SendRequestAsync`; terminal `stopReason` on `session/prompt` response completes `RunPromptAsync` without extra `prompt_complete` |
| 2 | `session/request_permission` (server JSON-RPC with `id`) not answered during client RPC wait | `HandleServerRequestAsync` + `CursorAcpNdjsonCodec.SerializeResponse`; decisions via existing `CursorAcpPermissionPolicy` (deny default, bounded allow-once) |
| 3 | ACP `session/new.cwd` from `Environment.CurrentDirectory` | `ProjectLocator GovernedProjectRoot` on neutral `EngineeringAgentForwardRequest` / `EngineeringAgentAutomatedForwardRequest`; orchestrator validates and passes through; recovery resolves `RegisteredLocator` from project registry; Cursor maps to `session/new.params.cwd` |

**Explicitly out of scope:** Desktop async Forward, ViewModel threading, general operator Attention refresh while sync `Forward()` blocks (watch item unchanged).

### Provider-neutral contract (narrowest boundary)

**Extended type:** `EngineeringAgentForwardRequest` (+ mirrored `EngineeringAgentAutomatedForwardRequest` at orchestrator ingress).

**Field:** `ProjectLocator GovernedProjectRoot` — same domain type already used for registered/open project roots; carries **only** the authoritative filesystem root for provider execution workspace, not Cursor types.

**Rationale:** The forward contract is the last provider-neutral surface before `IEngineeringAgentProviderPlugin.Forward`; orchestrator already owns `ProjectConcordProjectId` and governance but must not reference ACP. Desktop/recovery supply the locator from `IProjectWorkspaceService` / `ProjectRegistry` respectively.

### Implementation summary

- **`CursorAcpClient`:** `RunPromptAsync(..., governedProjectRootAbsolutePath, ...)`; `SendRequestAsync` optional `CursorAcpPromptStreamCollector`; server-request handling during RPC; `end_turn` terminal completion; tail `CollectPromptUpdatesAfterRpcAsync` only when RPC lacks terminal stop reason.
- **`CursorEngineeringAgentProviderPlugin`:** passes `request.GovernedProjectRoot.NormalizedAbsolutePath` into `RunPromptAsync`.
- **`CursorAcpSessionContext.FromEnvironmentForDiagnostics`:** internal only — **not** used on production forward path.
- **T4 recovery:** unchanged disposition semantics; recovery forward now requires registry `RegisteredLocator` (tests register via `InMemoryProjectRegistry.RegisterProjectForTests`).

### Tests (automated)

| Area | Coverage |
|------|----------|
| A–H | `CursorAcpInitializeContractTests`, `CursorAcpPromptPumpTests`, `EngineeringAgentA4T6CursorProviderTests` (prompt order, permissions, governed cwd) |
| I–L | `EngineeringAgentA4T3OrchestrationTests` + `EngineeringAgentTestWorkspace.DefaultLocator`; T4 recovery tests unchanged semantics |
| New helpers | `RecordingCursorAcpTransport`, `CursorAcpFixtures.PromptLiveCursorOrderScript` |

### Release validation (Remediation #2, uncommitted)

| Item | Result |
|------|--------|
| `dotnet build -c Release` | **0** errors (NU1903 / existing warnings unchanged) |
| `dotnet test -c Release` | **257** passed, **0** failed — Application **190**, ProjectServices **56**, Desktop **11** |

### Live diagnostic (outside governed Forward; Root-A only)

Filter: `LiveCursorAcp_RunPrompt_CompletesOnEndTurn` with `PROJECTCONCORD_CURSOR_AGENT_PATH=$HOME/.local/bin/agent`.

| Check | Result |
|-------|--------|
| initialize / authenticate | **OK** |
| `session/new` wire contains Root-A | **OK** (`/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A`) |
| Harmless prompt `Reply with exactly: PROJECTCONCORD_ACP_OK` | **OK** |
| `RunPromptAsync` returns (no hang) | **OK** (~7s) |

**Not** MVT-7 Retest #2.

### Historical evidence preserved

- Original **MVT-7 = Fail** and **MVT-7 Retest #1 = Fail** unchanged in MVR-0003.
- Transport row `cac3c9c8-6817-4a4c-9c01-6c9d42cc1937` (`ForwardInProgress`) — **not** mutated or deleted.

---

## A4-T6 Corrective Remediation #3 (2026-10-02, local / uncommitted)

**Authorization:** Option **C** — provider-neutral Engineering Result **response contract** composed at automated execution boundary. **Not** MVT-7 Retest #3; **no** governed Forward; **no** publication.

### Architectural selection

- **Gap:** Retest #2 failed at import because the **response instruction was absent**, not because Cursor ignored one.
- **P0 equivalence:** `TryRenderValidatedHandover` output **unchanged**; automated path uses `GovernedRelayAutomatedExecutionPrompt.Compose(handover, exportPackage)`.
- **Ownership:** `GovernedRelayEngineeringResultResponseInstruction` + `GovernedRelayAutomatedExecutionPrompt` (`Edf.Application/Relay/Serialization/`); surfaced on `IEngineeringAgentRelayBridge` for relay parity.
- **Orchestration:** `EngineeringAgentTransportOrchestrator` and `EngineeringAgentTransportRecoveryService` pass **composed** text as opaque `RenderedHandoverBody` to plugins.
- **Providers:** No schema logic in Cursor plugin; no synthesis/repair of results; `TryParseEngineeringResult` unchanged.

### Tests / validation

| Item | Result |
|------|--------|
| `dotnet build -c Release` | **0** errors |
| `dotnet test -c Release` | **264** passed, **0** failed — Application **197**, ProjectServices **56**, Desktop **11** |
| New | `GovernedRelayAutomatedExecutionPromptTests`, orchestration composition test, live `LiveCursorAcp_ComposedExecutionPrompt_*` |

### Live Cursor diagnostic (composed prompt; Root-A; not Retest #3)

| Stage | Observation |
|-------|-------------|
| ACP transport | **Completed** (`RunPromptAsync` returned; ~2–3 min on full handover+instruction) |
| Returned shape | **Not** a recognizable complete `engineeringResultImport` relay document in primary diagnostic run |
| `TryParseEngineeringResult` | **`RejectedMalformed`** — `relay.render.version.missing` (Render version marker missing) |
| Substantive validation | **Not reached** (no `Valid` package) |
| Compensation | **None** — no host synthesis |

**PA implication:** Remediation #3 supplies the missing contract; **live Cursor compliance** with governed result shape remains **unproven** and may require Retest #3 / prompt iteration / external EA behavior — **not** importer weakening.

**PA disposition (2026-10-02):** Remediation #3 **implementation architecture accepted locally**; **live effectiveness not yet accepted** (structural parse failed). **MVT-7 Retest #3 not authorized.**

### Live result-conformance follow-up investigation (2026-10-02, investigation only)

#### A — Actual live Cursor output

| Item | Finding |
|------|---------|
| **Exact response text retained?** | **No.** `LiveCursorAcp_ComposedExecutionPrompt_AcpCompletes_AndParseReportsValidation` does not log or persist `promptResult.ResultText`; only classification via `TryParseEngineeringResult`. |
| **Preserved diagnostic** | **`RejectedMalformed`** — code **`relay.render.version.missing`** / message *"Render version marker is missing."* (`GovernedRelayV1Importer.TryParseRenderVersion`). |
| **Inference** | Importer found **no line** beginning with `ProjectConcord-Relay-Render:` anywhere in the candidate string. Failure occurred **before** machine-block extraction. |
| **Fenced `projectconcord-relay-v1` block** | **Not established** (failure precedes fence check; no positive evidence of a block). |
| **`engineeringResultImport` JSON** | **Not established.** |
| **Partial vs ignored** | **Cannot distinguish** prose-only vs malformed relay fragment without stored output; **`render.version.missing`** is consistent with **ordinary agent prose** (or markdown) **without** the required render header — not with a complete thin fixture-shaped document. |

**Live test input note:** Diagnostic uses `RelaySerializationFixtures.ValidImplementationHandover()` (synthetic PA handover), **not** the persisted MVR Root-A `relay_package`. `projectId` / `correlationId` in the response instruction are **runtime-generated GUIDs** per test execution.

#### B — Current response instruction shape

`GovernedRelayEngineeringResultResponseInstruction.Render` produces **descriptive requirements + brief reference shape** — **not** a complete example, **not** an exact fill-in template. See source: `GovernedRelayEngineeringResultResponseInstruction.cs`.

#### C — Delta vs `engineering-result-import-thin.relay.txt`

Instruction **names** requirements; fixture **demonstrates** full wire document. Gaps include: exact render header line; full fenced JSON example; `packageId` / `schemaVersion*` / `createdUtc` / `updatedUtc`; complete `governanceCritical` JSON; `tier0Snapshot`; `softwareDevelopmentProfile`; human **`## Governance-Critical`** … **`## STOP`** projections; Pa reminder line; machine/projection agreement rules.

#### D — Renderer template capability

`GovernedRelayV1Renderer` can render a structurally valid **`EngineeringResultImport`** skeleton from a `GovernedRelayPackage` (see `EngineeringAgentManualRelayBridgeTests.ThinEngineeringResultImport_CanParseWithoutFullImplementationAuthorization`). **Wire-format template** with handover-derived `projectId`/`correlationId` and explicit **placeholders** for EA-owned fields is **feasible** without claiming substantive engineering work — **if** the template is clearly non-authoritative, not auto-imported, and EA must replace placeholders before return.

#### E — Output isolation

Importer: render-version line may appear **anywhere** in the document (line scan); machine block via **first** regex match — **leading/trailing prose can coexist** if render header + valid block exist. **`relay.render.version.missing`** on live run implies failure at **first** structural gate. **Exact-output-only** response policy would improve LLM determinism and is **compatible** with importer when the document is well-formed.

#### F — Contract strength recommendation

| Option | Assessment |
|--------|------------|
| 1 Descriptive only (current) | Provider-neutral; **insufficient** live reliability (evidence). |
| 2 + known-valid example | Better; still copy-error prone. |
| 3 Template + placeholders | **Narrowest robust** candidate — renderer-owned skeleton, handover-bound IDs, EA fills placeholders. |
| 4 Skeleton + “return only this document” | Add to **3** for automated path; low normative risk. |

**Recommend:** **3 + 4** (renderer-generated skeleton + isolation instruction), composed at automated boundary only; P0 manual unchanged.

#### G — Latency (available evidence)

| Factor | Evidence |
|--------|----------|
| Composed prompt size | Full implementation handover + instruction vs ~7s one-line diagnostic — **much larger task**. |
| Model time | **Likely dominant** (~2–3 min observed). |
| Permissions | **No evidence** in test (no recording); not assessed. |
| Client pump | Aggregates chunks until `end_turn`; **not indicated** as primary delay. |
| 5-minute operator window | **Still sufficient** (test used 180s I/O timeout; completed within ~2–3 min). |

#### H — Normative impact

Strengthening to a **renderer-owned response template** appended at automated execution is **implementation of existing** PC-PAR-022 / ADR-0021 §6 (same parse path) / ADR-0022 §3 (untrusted intake) — **not** a change to validation rules or package kinds. **No normative STOP required** provided: template is **instruction-only**, never auto-imported, and EA substantive content remains **untrusted** until `TryParseEngineeringResult` succeeds. **Would require PA authorization** for a **future implementation tranche** (not done in this investigation).

### Historical MVT / transport

- **MVT-7 / Retest #1 / Retest #2 = Fail** preserved in MVR-0003.
- Transport rows `cac3c9c8-…` and `82d9d6ce-…` **not** mutated.

---

## A4-T6 Corrective Remediation #3B (2026-10-02, local / uncommitted)

**Authorization:** Replace descriptive Remediation #3 response instruction with a **complete provider-neutral Engineering Result output contract** (renderer-generated structural template + field semantics + output isolation). **Not** MVT-7 Retest #3; **no** governed Forward; **no** publication; **no** commit/push.

### Architectural principle

- **Zero prior knowledge:** External Engineering Agent receives the full wire shape from the execution prompt — not “follow relay-v1 semantics” without demonstration.
- **Single source of structural truth:** `GovernedRelayV1Renderer` renders the instructional template from `GovernedRelayEngineeringResultOutputContract.CreateResultImportSkeleton(handoverExport)`; placeholders applied post-render (`__REPLACE_WITH_NEW_PACKAGE_ID_UUID__`, `__REPLACE_WITH_UTC_ISO8601_TIMESTAMP__`).
- **Field ownership:** Documented in contract table (ProjectConcord-fixed, EA-generated, governance-constrained, enumerated, required-empty).
- **P0 unchanged:** `TryRenderValidatedHandover` output does not include the output contract.
- **Providers:** Opaque transport only; no schema synthesis/repair.

### Implementation

| Component | Role |
|-----------|------|
| `GovernedRelayEngineeringResultOutputContract.cs` | Complete contract + template + semantics + isolation |
| `GovernedRelayEngineeringResultResponseInstruction.cs` | Delegates to output contract (same bridge entry point) |
| `GovernedRelayAutomatedExecutionPrompt.cs` | Unchanged composition: handover + contract |
| `GovernedRelayEngineeringResultOutputContractTests.cs` | Template not `Valid`; completed example `Valid`; contract coverage |

### Automated validation

| Item | Result |
|------|--------|
| `dotnet build -c Release` | **0** errors (NU1903 / existing warnings unchanged) |
| `dotnet test -c Release --filter RequiresCursor!=true` | **265** passed, **0** failed — Application **198**, ProjectServices **56**, Desktop **11** |
| Template `TryParseEngineeringResult` | **Not** `Valid` (placeholders / invalid ids) |
| `RenderCompletedValidExample` | **`Valid`** via real `TryParseEngineeringResult` |

### Live Cursor diagnostic (#3B; one shot; not Retest #3)

Production composition path: `ComposeAutomatedExecutionPrompt` + minimal no-file-change task suffix. **Root-A** cwd. Body captured: `/Users/edbecnel/tmp/ProjectConcord-A4-ACP-Diagnostic/remediation-3b-live-result.txt`.

| Stage | Result |
|-------|--------|
| ACP init / auth / session / prompt | **Completed** (~**30.7 s**) |
| `RunPromptAsync` | **Succeeded** |
| Returned body captured | **Yes** |
| `TryParseEngineeringResult` (unchanged) | **`RejectedMalformed`** |
| Diagnostic | **`relay.render.version.missing`** — render marker not on any line-start (preamble violated output isolation; `ProjectConcord-Relay-Render: 1` appeared **mid-line** after prose, not as line 1) |
| Substantive / machine-projection validation | **Not reached** |
| `EngineeringResultImport` package | **Absent** |

**Counterfactual (not applied at import boundary):** Suffix of captured body beginning at a line-start `ProjectConcord-Relay-Render: 1` includes a complete `projectconcord-relay-v1` machine block and governance projections matching handover-bound `projectId` / `correlationId` — suggests **contract/template comprehension improved** but **live effectiveness criterion (#3B) not met** because the host must not strip preamble.

**#3B effectiveness status:** **FAIL** (live unchanged parse ≠ `Valid`). **STOP TO PA.** No second live iteration. **MVT-7 Retest #3 not authorized.**

### Historical MVT / transport

- **MVT-7 / Retest #1 / Retest #2 = Fail** preserved.
- Transport rows `cac3c9c8-…` and `82d9d6ce-…` **not** mutated.

### Process note — accidental second live invocation (#3B)

The authorized #3B live diagnostic was **invoked twice** before `PROJECTCONCORD_RUN_LIVE_CURSOR` gating was added: once during an unfiltered `dotnet test` run (~30.7 s, first failure recorded) and again during a subsequent full Release test run (~35 s). Both overwrote the same capture path. The file analyzed for #3C is the **on-disk capture** at `remediation-3b-live-result.txt` (second-run body; package id `6f1c8a24-3e7b-4d91-a5c0-9b2e4f18d673`). **#3B live effectiveness remains FAIL** for unchanged full-body import.

---

## A4-T6 Corrective Remediation #3C (2026-10-02, investigation only)

**Authorization:** Investigate provider-neutral **untrusted governed-document extraction** between raw provider output and `TryParseEngineeringResult`. **No** `src/` or `tests/` changes in this tranche. **No** new live Cursor call.

### A — #3B captured-output framing analysis

| Observation | Detail |
|---------------|--------|
| Preamble | Lines 1–5 conversational; violates #3B output-isolation contract |
| Render marker in **raw** buffer | **Once**, at index 713, **not** at line start (`raw[idx-1]` is `.` from prose ending `...fences.`) |
| Physical line 6 | Prose and `ProjectConcord-Relay-Render: 1` share one line in the provider stream |
| Machine block | **One** ` ```projectconcord-relay-v1` fence pair |
| Projections | `## Governance-Critical`, `## STOP`, `## EDF-Correlation`, PA reminder present |
| Unchanged import | **`RejectedMalformed`** / `relay.render.version.missing` (correct for full body) |

### B — Exact contiguous substring validity (read-only probe)

Using built `EngineeringAgentManualRelayBridge` against the capture **outside** the repo (no tree changes):

| Candidate | `TryParseEngineeringResult` |
|-----------|------------------------------|
| Full `remediation-3b-live-result.txt` | **`RejectedMalformed`** (`relay.render.version.missing`) |
| `raw.Substring(indexOf("ProjectConcord-Relay-Render:"))` — **exact bytes, no mutation** | **`Valid`**, `EngineeringResultImport` package present |

So a **complete importable relay document exists** as a contiguous suffix of the provider response, but **not** as the full response under current rules.

### C — Substring at mid-line marker: extraction vs repair

| Interpretation | Assessment |
|----------------|------------|
| **Repair (prohibited)** | Inserting newline, moving header onto its own line in the **raw** buffer, or editing JSON/projections — **not** what suffix selection does |
| **Exact extraction** | Selecting `[index..]` copies relay bytes verbatim; extracted buffer’s **first line** is `ProjectConcord-Relay-Render: 1` |
| **Line-boundary semantics** | Importer defines render recognition on **logical lines** (`Split('\n')` + `StartsWith` prefix). Mid-line marker in **raw** text is **invalid framing**; suffix selection **excludes** preamble bytes and **does not** include the prose prefix that shared line 6 |
| **PA decision fork** | **(C1)** Extraction START only when marker is at **raw** line start → **rejects** this capture (no repair). **(C2)** Extraction START at **first marker occurrence** (byte index) → **accepts** this capture for import unchanged. **(C2) is not line-boundary repair** but **is** a different document boundary rule than “full provider body is the document.” |

### D — Relay-v1 start/end boundary (current code + renderer)

**START (de facto today):** First logical line (after trim) starting with `ProjectConcord-Relay-Render:` anywhere in the string passed to `GovernedRelayV1Importer.Import`.

**END:** **Not normatively defined** for import. `GovernedRelayV1Importer` scans the **entire** string for first machine-block regex match and projection sections (`IndexOf` headings). Trailing prose **after** a well-formed relay tail is **not** explicitly rejected if a line-start render marker exists elsewhere. Renderer **convention** ends with `GovernedRelayV1Format.PaEngineeringAgentReminder` (see `GovernedRelayV1Renderer`).

**Multiple documents:** Not forbidden by format; importer uses **first** matching render line and **first** machine-block regex. Safe extraction should **reject** ambiguous multiples rather than pick “best.”

### E — Surrounding-text behavior (verified)

`GovernedRelayV1Importer`: render version = line scan; machine block = first regex match; projections = section `IndexOf` — **leading/trailing prose is tolerated only if a line-start render marker exists**. Mid-line marker **fails** render scan even when a valid suffix exists. **Not documented as a product feature** in SPEC-006; **incidental** parser behavior.

### F — Trust model

Exact byte-range extraction **before** `TryParseEngineeringResult` is analogous to **envelope stripping**: extractor grants **no** trust; importer still performs structural + substantive validation. Consistent with ADR-0021 §2 (untrusted provider text through same parse path; no field manufacture) and ADR-0022 §3 inbound lifecycle **if** extraction is defined as **selection only**. Inconsistent with a strict reading of “provider body = relay document” without PA normative clarification.

### G — Provider neutrality

Extraction rules must depend only on **relay-v1 framing markers** (`ProjectConcord-Relay-Render:`, ` ```projectconcord-relay-v1`, `## Governance-Critical`, etc.) — **not** Cursor/ACP heuristics or conversational phrases.

### H — Manual P0 impact (analysis only)

| Placement | Effect |
|-----------|--------|
| **A — Transport only** (`ImportEngineeringResult` on `UntrustedImportText` in orchestrator/recovery) | P0 paste path unchanged; automated path gains robustness |
| **B — Shared bridge** (`TryParseEngineeringResult`) | Manual paste with accidental preamble could import if extraction rules match — **behavior change** for P0 unless gated |

Prefer **one canonical trust model**; if extraction is authorized, PA should decide **scope** (automated-only vs shared) explicitly.

### I — Options A/B/C/D

| Option | Summary | #3B capture |
|--------|---------|-------------|
| **A — Strict full body** | Current production behavior | **Fail** (correct) |
| **B — Exact framed subdocument** | Locate relay bytes; pass substring unchanged to importer | **Depends on START rule** (C1 fail / C2 pass probe) |
| **C — Machine-block reconstruction** | Rebuild document from JSON | **Synthesis** — conflicts with §6 / ADR-0021 |
| **D — Structured provider channel** | Separate payload for relay | **Not present** in `CursorAcpClient` — only aggregates `session/update` text into `ResultText` |

### J — Multiple-candidate safety (recommended if B authorized)

| Case | Recommended behavior |
|------|----------------------|
| Zero candidates | No extraction; import full body (or fail) |
| Exactly one candidate per deterministic rules | Extract exact range → importer |
| Multiple line-start render markers or multiple machine fences | **Reject extraction** (fail closed) |
| Malformed then valid | **Reject** unless rules define unambiguous single span |
| Overlapping spans | **Reject** |

### K — Recommended architecture (investigation conclusion)

**PA decision required before implementation.**

1. **Keep #3B output contract** (agents must still return only the relay document).
2. If extraction is authorized: add a **provider-neutral, untrusted extraction** step **immediately before** `TryParseEngineeringResult` on automated transport candidates (preferred scope), with rules documented in ADR/SPEC follow-on.
3. Define **START** explicitly (C1 vs C2 above) and **END** (e.g. after PA reminder line constant, or “shortest suffix that importer accepts” — latter risks coupling to incidental tolerance).
4. **Do not** implement Option C or provider-specific stripping.

### L — Thin vs rich `EngineeringResultImport`

| Topic | Finding |
|-------|---------|
| Thin path | Valid and used in fixtures (`engineering-result-import-thin.relay.txt`, #3B template) |
| Rich path | Supported by schema/validator when `authorizationDispositionPresent` / directives / profile sections align (see `SoftwareDevelopmentRelayProfileValidator`, `ThinEngineeringResultImport_*` vs `Bridge_UsesSubstantiveSoftwareDevelopmentProfileValidator`) |
| #3B template | **Intentionally thin** for instructional skeleton; **does not** normatively forbid rich results |
| A4 risk | Automated contract always renders **thin** skeleton from handover export; implementation handovers with `directsImplementationWork: true` may need **richer** result reporting for substantive `Valid` — **potential normative/product gap**; report to PA separately; **not** expanded in #3C |

### M — `PROJECTCONCORD_RUN_LIVE_CURSOR` guard

| Question | Answer |
|----------|--------|
| Test-only? | **Yes** — early `return` in `LiveCursorAcp_ComposedExecutionPrompt_*` |
| Default Release tests | **No live Cursor** unless env `=1` |
| vs `[Trait("RequiresCursor", "true")]` | **Dual gate** — trait labels tests; env prevents accidental execution |
| Production dependency | **None** |

---

## A4-T6 Corrective Remediation #3D (2026-10-02, local / uncommitted)

**Authorization:** Option **B** exact framed subdocument extraction; **START** rule **C2** (marker-byte); **END** at `GovernedRelayV1Format.PaEngineeringAgentReminder`; **automated transport only**; P0 manual parse **unchanged**. **No** live Cursor in this tranche.

### Implementation

| Component | Role |
|-----------|------|
| `GovernedRelayAutomatedResultDocumentExtractor` | Provider-neutral START/END framing; fail-closed ambiguity rules |
| `EngineeringAgentTransportOrchestrator` | Extract → `ImportEngineeringResult` on success |
| `EngineeringAgentTransportRecoveryService` | Same extraction on `ConfirmRecoveredResultImportAsync` |
| `RelayValidationCodes` | `relay.transport.extraction.*` diagnostics |
| Tests | `GovernedRelayAutomatedResultDocumentExtractorTests`, mid-line fixture, A4-T3 enveloped import |

### Validation

| Item | Result |
|------|--------|
| `dotnet build -c Release` | **0** errors |
| `dotnet test -c Release` | **285** passed — Application **218**, ProjectServices **56**, Desktop **11** |
| Importer / `TryParseEngineeringResult` | **Unchanged** |
| #3B live effectiveness | **Still FAIL** (historical; unchanged) |

### Normative clarification

[ADR-0022 §3 inbound lifecycle](../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) updated for automated extraction envelope vs authoritative parse.

### Pending (not implemented in #3D)

- **Thin vs rich** automated result contract gap (#3C) — investigated in **#4** below.
- **Cursor Engineering Agent model selection** — **mandatory** configurable; default **Composer 2.5** — investigated in **#4** below.

---

## A4-T6 Corrective Remediation #4 (2026-10-02, investigation only)

**Authorization:** Thin vs rich `EngineeringResultImport` profile + Cursor ACP model/capability investigation. **No** `src/` / production `tests/` changes. **No** governed Forward, live engineering prompt, Retest #3, commit, or push. **#3D extraction unchanged.**

### A — Thin vs rich validator analysis (authoritative)

`SoftwareDevelopmentRelayProfileValidator` applies to `EngineeringResultImport` (`RequiresProfileValidation`). Rules are evaluated on the **result package itself**, not by diffing to the source handover at parse time.

| Result governance / profile signal | Validator consequence |
|-----------------------------------|---------------------|
| `directsImplementationWork == false` and `directsTrancheWork == false` | `ValidateImplementationAuthorization` / tranche rules largely **no-op**; empty `payloadVersion: 1` profile can be **`Valid`** (thin fixture + `ThinEngineeringResultImport_*`) |
| `directsImplementationWork == true` | **`DevelopmentWorkAuthorization` required** (`ImplementationAuthorizationMissing` if absent); planning DWA cannot satisfy implementation (`PlanningCannotSatisfyImplementation`) |
| `authorizationDispositionPresent == true` | Profile **`AuthorizationDisposition` required** (`AuthorizationDispositionPayloadMissing`) |
| `workContextPresent` + `directsTrancheWork` | **`WorkContext` payload** required / aligned (`WorkContextPayloadMissing`, tranche scope rules) |
| `HandoverContext` without `IsInheritedContextOnly` | **`Incomplete`** (`HandoverDevelopmentWorkAuthorizationConflation`) |
| `directsImplementation` + handover context only (no DWA) | **`Incomplete`** (`HandoverDoesNotImplyAuthorization`) |
| Active **STOP** + implementation DWA without explicit auth reference | **`Incomplete`** (`StopBlocksImplicitImplementationAuthorization`) |

**Implementation-directed handover (export)** may carry `directsImplementationWork: true` and rich PA authorization. **That does not force the Engineering Result import to repeat those flags.** A **thin** result with directive flags **false** and empty software profile can still reach **`Valid`** (`EngineeringAgentManualRelayBridgeTests.ThinEngineeringResultImport_*`). Conversely, `Bridge_UsesSubstantiveSoftwareDevelopmentProfileValidator` shows a **rich-shaped** result (directives true, disposition present, empty payload) → **`Incomplete`** until profile catches up.

### B — Incoming handover → required result profile (matrix)

| Incoming handover characteristics | Required result profile for `Valid` | Thin (#3B skeleton) valid? | Rich required? | Why |
|----------------------------------|-----------------------------------|----------------------------|----------------|-----|
| Investigation / minimal task; no implementation directive on **result** | Thin (flags false, empty profile) | **Yes** | No | No substantive profile triggers |
| Planning handover; result does not claim implementation | Thin | **Yes** | No | Same |
| Implementation-authorized **handover**; EA returns **compliance-safe** completion without claiming new authority | Thin (inherit mode/session/stop/EDF; **force** auth/disposition/work flags false on result) | **Yes** | No | Validator does not require echoing handover directives on result |
| EA must **report** implementation/tranche work on result (`directsImplementationWork` or `directsTrancheWork` true on result) | Rich: DWA + optional disposition/work context per flags | **No** | **Yes** | `ValidateImplementationAuthorization`, disposition/work alignment |
| EA sets `authorizationDispositionPresent: true` on result | Rich disposition block | **No** | **Yes** | `AuthorizationDispositionPayloadMissing` if absent |
| STOP-active handover; result claims implementation without disposition | Depends on STOP + DWA rules | Thin may **fail** substantive | Often **rich** + explicit disposition | `StopBlocksImplicitImplementationAuthorization` |

### C — #3B template adequacy

| Workflow | Adequacy |
|----------|----------|
| A — investigation-only / minimal automated task | **Adequate** (thin skeleton + isolation + #3D extraction) |
| B — planning handover | **Adequate** if result remains thin |
| C — implementation-authorized handover | **Adequate for `Valid` thin completion** that does not re-assert implementation authority; **inadequate** if PA expects rich **work reporting** on the result package |
| D — STOP results | **Partially adequate** — skeleton copies handover `stop`; substantive rules still apply if result escalates implementation under STOP |
| E — other governed dispositions | **Derive per matrix**; #3B intentionally **forces** `authorizationDispositionPresent` / `workContextPresent` / directive flags **false** to prevent EA authority invention |

### D — Deterministic profile derivation (recommendation)

**Preferred:** `validated handover export` → `required EngineeringResultImport profile class` (thin vs rich reporting) → `GovernedRelayV1Renderer`-owned output contract. Default automated path should remain **thin + authority-preserving** unless handover + workflow explicitly require **rich reporting** fields; do **not** ask the external EA to choose profile class.

### E — Authority preservation (rich design)

| Field class | Rule |
|-------------|------|
| `projectId`, `correlationId`, schema/render version | **Fixed** from handover |
| Mode, session continuity, EDF correlation, inherited STOP | **Governance-constrained** copy/report |
| `packageId`, timestamps, EA narrative in profile | **EA-generated** |
| `DevelopmentWorkAuthorization`, disposition grants, directive flags | **Must not be invented** by EA; rich **reporting** only when legitimately present in completed work and aligned with incoming authority — never to bypass PA |

### F — Cursor model requirement (mandatory; not implemented)

- Model selection **must** be configurable; **initial/default = Composer 2.5** (PA mandatory).
- **No silent substitution** when an explicit model is unavailable — prefer **`session/set_model` error surfacing** (see below).

### G — Cursor ACP model-selection mechanism (local protocol evidence)

Environment: Cursor CLI `agent` **2026.09.28-64d2043** (`~/.local/bin/agent`). Read-only NDJSON probes (no governed ProjectConcord Forward, no file-changing prompt).

| Mechanism | Finding |
|-----------|---------|
| `initialize` | Returns `agentCapabilities` (MCP, prompt, session list). **No** model list; **no** structured-output schema capability advertised |
| `authenticate` | `methodId: cursor_login` |
| **`session/new`** | Returns `sessionId`, `modes`, and **`models.currentModelId` + `models.availableModels[]`** (`modelId`, `name`). Accepts **`modelId`** parameter (observed: `modelId: "composer-2.5[fast=true]"` → `currentModelId` matches). **`model`** key also accepted in probe |
| **`session/set_model`** | **Supported** — `{"sessionId","modelId"}` → `{}` on success; **invalid model** → JSON-RPC **`-32602` / `Invalid model value: …`** (fail-closed) |
| `session/prompt` | Text prompt array; completion via `stopReason` (`end_turn`); streamed text via **`session/update`** `agent_message_chunk` (existing ProjectConcord client) |
| CLI non-ACP | `agent --model <model>` and `agent --list-models` (separate from ACP wire) |

**Not found:** `session/setModel`, `session/configure` (method not found). `session/update` is **server→client** streaming, not a host RPC for model config.

### H — Composer 2.5 identifiers

| | Value |
|--|--------|
| **Display name** | `Composer 2.5` (from `agent --list-models`) |
| **CLI slug** | `composer-2.5` |
| **ACP `modelId` (this environment)** | `composer-2.5[fast=true]` (`session/new` `availableModels` entry name `composer-2.5`) |

Parameterized bracket suffixes may vary by Cursor release; implementation should **match against `availableModels` from `session/new`** rather than assuming a single string forever.

### I — Model discovery & confirmation

- **Discovery:** `session/new` → `models.availableModels` (per session). CLI `agent --list-models` for operator/diagnostic parity (non-ACP).
- **Confirmation:** `session/new` / post-`session/set_model` **`models.currentModelId`** is the protocol-reported active model. **`session/prompt` result** in probe returned only `stopReason` — **no** model metadata on prompt completion. ProjectConcord should treat **`currentModelId` after explicit set** as the evidence boundary unless/until Cursor adds prompt-result model fields.

### J — Configuration ownership & persistence (recommendation)

| Concern | Recommendation |
|---------|----------------|
| Ownership | **Cursor-provider-specific** preference store (parallel to `EngineeringAgentPluginProjectPreferencesStore`), not Core relay |
| Scope | **Per ProjectConcord project** (mirror `engineering_agent.plugin.selected.{projectId}` pattern) e.g. `engineering_agent.cursor.model.{projectId}` |
| Persistence | Existing **`user_preferences`** via `IEngineeringAgentOperationalPreferenceBacking` — **no new subsystem** |
| Default | On first read when key absent, use configured default wire id targeting **Composer 2.5** (resolve to concrete `modelId` from `availableModels`) |
| Update | Explicit user/operator change writes preference; **never** overwrite non-empty stored value with default on restart |
| Disable/re-enable plugin | Model preference **retained**; re-apply on next `session/new` / `session/set_model` |

### K — Unavailable / invalid model semantics

| Path | Behavior (observed) |
|------|---------------------|
| `session/set_model` invalid | **Error** `-32602` — suitable for **no silent fallback** |
| `session/new` with invalid `modelId` | **Still returned session** with `currentModelId: composer-2.5[fast=true]` in probe — **does not fail closed**; treat as **risk**: implementation must **verify** `currentModelId` matches resolved configured id or fail transport |

### L — Structured output capability

**No provider-enforced structured Engineering Result channel found** in ACP probe: `initialize.agentCapabilities.promptCapabilities` lists audio/image/embeddedContext only; no JSON Schema / constrained relay payload channel. Final results remain **text** (`session/update` chunks). **Prompt-requested JSON ≠ enforced structured output.**

**Consequence:** **#3B complete output contract + #3D extraction remain appropriate** defensive architecture. A future enforced structured channel would be a **separate PA tranche**; do not replace #3B/#3D on prompt-only JSON.

### M — Normative / spec gaps (for future PA tranche)

- ADR-0022 documents extraction (#3D); **no** ADR text yet for **automated result profile derivation** (thin vs rich reporting) or **Cursor model preference** — recommend small ADR amendment or SPEC-006 PC-PAR-022 footnote when implementing.
- Mandatory **Composer 2.5 default** is a **new PA requirement** — not yet reflected in ADR-0021 plugin contract.

### N — #3D preservation

**Confirmed unchanged** — no investigation changes to extraction code or scope.

### O — Live work

**No** governed Forward. **No** full engineering prompt. Minimal ACP probes only (`initialize`, `authenticate`, `session/new`, `session/set_model` error probe, one-line `Reply OK` prompt for stopReason shape).

---

## A4-T6 Corrective Remediation #5 (2026-10-02, implementation)

**Authorization:** Cursor model configuration (Composer 2.5, **fast disabled** default) + deterministic thin/rich result-profile derivation. **No** commit/push, **no** MVT-7 Retest #3, **no** governed Forward engineering work.

### A — Non-fast Composer 2.5 ACP evidence (this environment)

| Item | Evidence |
|------|----------|
| Human display | `Composer 2.5` (`agent --list-models`) |
| Performance mode intent | **Fast = false** (ProjectConcord default) |
| CLI non-fast slug | `composer-2.5` (distinct from `composer-2.5-fast`) |
| ACP fast variant | `composer-2.5[fast=true]` in `availableModels` |
| ACP non-fast in `availableModels` | **Not advertised** on Cursor CLI `2026.09.28-64d2043` in prior probe — only fast composer entry listed |
| Wire id when non-fast is advertised | Resolve from `availableModels` via exact family + `fast=false` match (e.g. bare `composer-2.5` or `composer-2.5[fast=false]` if Cursor adds it) — **do not** treat `composer-2.5[fast=true]` as non-fast |

**Operational consequence:** Default transport **fails closed** with `ModelConfigurationFailed` when only fast composer is advertised (no silent fast fallback).

### B — Model configuration (implemented)

| Concern | Implementation |
|---------|----------------|
| Durable intent | `CursorEngineeringAgentModelSelection` (family slug + `FastEnabled`) |
| Persistence | `CursorEngineeringAgentModelPreferencesStore` → `engineering_agent.cursor.model.family.{projectId}` + `engineering_agent.cursor.model.fast.{projectId}` on `IEngineeringAgentOperationalPreferenceBacking` |
| Default | `composer-2.5`, `FastEnabled: false` when keys absent |
| Resolution | `CursorAcpModelResolver` after discovery `session/new` |
| Session flow | Discovery `session/new` → resolve → `session/new` with `modelId` → verify `currentModelId` → `session/prompt` (resume: `session/load` + `session/set_model`) |
| Verification | Exact wire id equality + parsed fast semantics before prompt |
| Failure | `EngineeringAgentProviderFailureKind.ModelConfigurationFailed` → `operator.attention.engineering-agent.model-configuration-failure` |
| Neutrality | `EngineeringAgentForwardRequest.ProjectId` only; orchestrator has no Composer/fast/modelId knowledge |

### C — Result profile derivation (implemented)

| Path | Rule |
|------|------|
| Default automated | **Thin** (`GovernedRelayEngineeringResultReportingRequirements.Thin`) |
| Implementation-authorized handover | Still **thin** on result (does not copy handover directives) |
| Rich | **Superseded by #6** — automated transport uses **thin only**; rich derivation rules from early #5 removed |
| #3B | `GovernedRelayEngineeringResultOutputContract` skeleton + semantics from derived reporting requirements |
| #3D | Unchanged (profile-agnostic extraction) |

### D — Validation (Release)

`dotnet build -c Release` — 0 errors. `dotnet test -c Release` — live Cursor tests gated by `PROJECTCONCORD_RUN_LIVE_CURSOR=1`.

### E — Historical MVT preservation

MVT-7 = **Fail**; Retest #1/#2 = **Fail**; #3B live = **Fail**; MVT-8 **not executed**; no Retest #3 record.

---

## A4-T6 Corrective Remediation #6 (2026-10-02, ACP diagnostic + thin profile)

**Build:** `2026.09.28-64d2043` (`/Users/edbecnel/.local/bin/agent acp`). **Probe script:** `/tmp/projectconcord-acp-remediation6-probe.py` (cwd: MVR Root-A).

### ACP — `composer-2.5[fast=false]`

| Step | Result |
|------|--------|
| `session/new` with `modelId: "composer-2.5[fast=false]"` | Returns session (no JSON-RPC error) |
| `models.currentModelId` after configured `session/new` | **`composer-2.5[fast=true]`** |
| `configOptions[id=model].currentValue` | **`composer-2.5[fast=true]`** |
| `session/set_model` with `composer-2.5[fast=false]` | **Rejected** `-32602` — `Invalid model value: composer-2.5[fast=false]` |
| Verify `session/new` (same modelId) | `currentModelId` still **`composer-2.5[fast=true]`** |

### ACP — `composer-2.5[]`

| Step | Result |
|------|--------|
| `session/new` with `modelId: "composer-2.5[]"` | Returns session |
| `models.currentModelId` | **`composer-2.5[fast=true]`** |
| `session/set_model` with `composer-2.5[]` | **Rejected** `-32602` — `Invalid model value: composer-2.5[]` |

### CLI control

`agent --trust --model 'composer-2.5[fast=false]' -p 'Reply with exactly: PC_MODEL_PROBE_OK'` from MVR Root-A → **`PC_MODEL_PROBE_OK`** (non-ACP path accepts the flag string; **does not** establish verifiable non-fast ACP session state).

### PA disposition (#6)

**PA ACCEPTED** (2026-10-02). Automated validation on local tree: **Release build 0 errors**; **298 tests passed** / 0 failed. Result-profile reconciliation (**always THIN** for automated transport) **accepted**.

### PA determination (provider)

**CURSOR ACP PROVIDER CAPABILITY BLOCKER** — documented non-fast ACP identifiers are **not** accepted for `session/set_model`, and configured `session/new` **does not** yield verifiable non-fast `currentModelId` (runtime reports **FAST**). ProjectConcord **must not** substitute `composer-2.5[fast=true]`.

### ProjectConcord changes (#6)

- **No** resolver change to treat non-fast as available on this build (fail-closed #5 behavior remains correct).
- **Profile:** `GovernedRelayEngineeringResultProfileDerivation` reverted to **always thin** for automated transport (#5 automatic thin→rich rules removed).

---

## A4-T6 / T7 — Closeout preparation (PA 2026-10-02)

Documentation-only tranche. **No** further production/test changes. **No** new live ACP calls (existing probe evidence retained).

### Corrective remediation status matrix

| # | Scope | Local state | Notes |
|---|--------|-------------|--------|
| **#1** | ACP `initialize` / authentication wire correction | **Done** (local) | Original MVT-7 initialize defect remediated |
| **#2** | Prompt pump, completion, governed Project Root on forward | **Done** (local) | Retest #2 validated transport path to provider return |
| **#3 / #3B** | Explicit Engineering Result output contract | **Done** (local) | #3B live parse diagnostic remains **Fail** (historical) |
| **#3C** | Exact framed-document extraction investigation | **Done** (investigation) | Option B + C2 recommended; P0/importer unchanged |
| **#3D** | Provider-neutral automated exact extraction | **Done** (local) | Orchestrator/recovery only |
| **#4** | Model + result-profile investigation | **Done** (investigation) | No production change in #4 tranche |
| **#5** | Configurable verified Cursor model selection | **Done** (local) | Default Composer 2.5 / `fast=false`; fail-closed verification |
| **#6** | Thin profile + non-fast ACP proof | **PA ACCEPTED** | External ACP blocker recorded; thin-only derivation |

Historical MVT / live rows are **not** erased: original MVT-7 **Fail**, Retest #1 **Fail**, Retest #2 **Fail**, #3B live **Fail**.

### T6 implementation vs live verification

| Dimension | Status |
|-----------|--------|
| **Implementation complete locally** | Cursor automated transport model contract (#5–#6), #3B contract, #3D extraction, ACP session protocol stack |
| **Live required-configuration verification** | **Blocked externally** — installed Cursor ACP cannot establish verifiable Composer 2.5 **non-fast** session |
| **T6 published / finally closed** | **No** — local uncommitted tree only |

### External provider capability blocker (canonical)

ProjectConcord's Cursor automated transport implementation is **locally complete** for the required model-selection contract (default **Composer 2.5**, **fast disabled**; distinguish fast/non-fast; resolve advertised models where applicable; verify `currentModelId`; reject `fast=true` when `fast=false` is configured; no silent fallback; stop before `session/prompt` on mismatch).

**Live verification is blocked** because the installed Cursor ACP provider (`agent` **2026.09.28-64d2043**) does **not** expose or honor a verifiable Composer 2.5 **non-fast** configuration via ACP (`session/set_model` rejects documented non-fast wire ids; `session/new` coerces to `composer-2.5[fast=true]`).

This is **not**:

- a successful MVT-7 or Retest #3
- permission to use Fast or weaken verification
- a ProjectConcord defect requiring further speculative ACP workaround investigation

### MVT-7 / Retest history

| Entry | Result |
|-------|--------|
| Original MVT-7 | **Fail** |
| Retest #1 | **Fail** |
| Retest #2 | **Fail** |
| #3B live diagnostic | **Fail** |
| **Retest #3** | **NOT EXECUTED** — **BLOCKED** |

**Retest #3 blocked reason:** Required Composer 2.5 non-fast configuration cannot be established and verified through installed Cursor ACP. **Do not** create a Pass/Fail execution record for Retest #3 until the external blocker is resolved and PA authorizes execution.

**MVT-8:** **NOT EXECUTED** — not authorized in this tranche.

### T7 activity dependency (concise)

**Dependent on successful Cursor non-fast transport (blocked today):**

- MVR-0003 **Group A** — governed automated Forward to valid `EngineeringResultImport` under required model policy
- MVT-7 **Retest #3** (when PA authorizes) — end-to-end forward + import after #3B/#3D + model verification
- MVT-8+ human paths that assume cost-controlled Composer 2.5 non-fast ACP sessions
- Any closeout attestation that “live Cursor reference transport” satisfies PA mandatory default model

**Independent of provider blocker (can proceed without non-fast ACP):**

- Release build/test evidence on local tree (298 tests)
- Scripted ACP unit/integration tests (no live Cursor)
- MVR-0003 **Groups B–D** where they do not require a **successful** Group A forward (e.g. recovery UI/Attention semantics, P0 fallback regression in automation, scope/governance audits)
- Documentation, MVR record maintenance, Cursor escalation submission
- Publication decision / commit packaging (PA-gated; **not** authorized here)

### Cursor provider issue report (reproducible, no ProjectConcord required)

**Product:** Cursor Agent CLI  
**Version:** `2026.09.28-64d2043` (`agent --version`)  
**Invocation:** `agent acp` (NDJSON JSON-RPC on stdio)

**Expected (per Cursor documentation):** Composer 2.5 standard / non-fast selectable via ACP using documented wire forms such as `composer-2.5[fast=false]` or `composer-2.5[]`; session model state should reflect non-fast when those forms are requested.

**Actual:**

1. After `initialize` and successful `authenticate` (`methodId: cursor_login`), call `session/new` with `cwd`, `mode: agent`, and `modelId: "composer-2.5[fast=false]"`.
   - RPC may return a session id without error.
   - `result.models.currentModelId` is **`composer-2.5[fast=true]`** (not non-fast).
   - `configOptions` model `currentValue` is **`composer-2.5[fast=true]`**.

2. `session/set_model` with `modelId: "composer-2.5[fast=false]"` → JSON-RPC **`-32602`** — `Invalid model value: composer-2.5[fast=false]`.

3. Same pattern for `modelId: "composer-2.5[]"` — `session/set_model` **rejected**; `currentModelId` remains **`composer-2.5[fast=true]`**.

4. `models.availableModels` lists Composer 2.5 only as **`composer-2.5[fast=true]`** (display name `composer-2.5`).

**CLI control (non-ACP):** `agent --trust --model 'composer-2.5[fast=false]' -p 'Reply with exactly: PC_MODEL_PROBE_OK'` from a trusted workspace can succeed — does **not** fix ACP session model state above.

**Impact:** Integrations that must verify **non-fast** Composer 2.5 on ACP before automation (cost-controlled agent workflows) cannot proceed; accepting `fast=true` would violate product policy. No further undocumented model spellings are pursued pending a Cursor fix or documented supported mechanism.

**Evidence artifact (local):** probe script `/tmp/projectconcord-acp-remediation6-probe.py`; workspace cwd used: disposable MVR root (not ProjectConcord dev repo).

### No further ACP workaround investigation

STOP alternate model spellings or undocumented workarounds. Resume ACP model investigation only if Cursor ships a fix, documents another supported mechanism, or a newer build materially changes discovery/selection.

---

## MVT-7 Retest #1 post-execution (2026-10-02, evidence collection only)

See [MVR-0003 MVT-7 Retest #1 execution record](../Verification/Records/MVR-0003-a4-engineering-agent-automated-transport.md#mvt-7-retest-1--forward-after-a4-t6-corrective-remediation). Summary: `transport_operation` row created (`ForwardInProgress`); original MVT-7 had **0** rows; UI still showed `initialization-failure` Attention. **Disposition pending PA review** (Engineering Agent recommends **Fail** for incomplete forward; not Pass).

## STOP

No commit, no push, no governed Desktop **Forward handover (automated)** retry, no MVT-8. Return package delivered to PA in Engineering Agent chat.
