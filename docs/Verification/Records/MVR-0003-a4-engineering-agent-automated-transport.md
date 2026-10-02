# MVR-0003: A4 Engineering Agent automated transport and P0 fallback

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Verification](../../Verification/README.md) › [Records](README.md) › MVR-0003

## Identity

| Field                      | Value                     |
| -------------------------- | ------------------------- |
| **Record ID**              | MVR-0003                  |
| **Manual QA**              | Required                  |
| **Human execution status** | In progress               |
| **Owner**                  | Project owner (Ed Becnel) |
| **Verification date**      | 2026-10-02 (blocker-independent human tranche; Group A / B remainder blocked) |

**Human execution status:** Set **Complete** only when governed human manual verification was executed by an authorized human with outcomes that satisfy the verification obligation (typically **Pass** in the execution record for applicable MVTs). Do not set **Complete** because automated preparation or Release tests ran.

## Verification basis / obligation

| Requirement | Link | Notes |
|---|---|---|
| A4 automated transport + P0 coexistence | [A4 Implementation Plan](../../Handover/ProjectConcord-A4-Implementation-Plan.md) §16–§18, §20, §24 A4-T7 | PA reconfirmed scope 2026-10-01 |
| AF-1 recovery confirmation | [ADR-0022](../../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) §10; [A4-T4 notes](../../Handover/ProjectConcord-A4-T4-Implementation-Notes.md) | Transport recovery ≠ governance acceptance |
| Disposable workspace | [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md) | **MUST NOT** use ProjectConcord dev repo as subject |
| Machine verification (supporting only) | [A4-T7 verification evidence](../../Handover/ProjectConcord-A4-T7-Verification-Evidence.md) | Not a substitute for this MVR |
| A4-T6 corrective remediation (local) | [A4-T6 corrective remediation notes](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md) | **Uncommitted**; PA accepted locally for MVT-7 Retest #1 and **Remediation #2** for governed Retest #2 — **not** publication |

### MVT execution summary (governed)

| MVT | Result | Notes |
| --- | --- | --- |
| MVT-1–6 | **Pass** | Historical execution 2026-10-02 |
| **MVT-7** | **Fail** | **Original** governed forward — record preserved; **not** rewritten by retest |
| **MVT-7 Retest #1** | **Fail** | Single Forward 2026-10-02; `ForwardInProgress` stall — see execution record + [stall investigation notes](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#mvt-7-retest-1-forward-stall-investigation-2026-10-02) |
| **MVT-7 Retest #2** | **Fail** | Forward **returned** (Remediation #2 transport validated); terminal **`ImportRejected`** at governed result boundary — see [execution record](#mvt-7-retest-2--forward-after-corrective-remediation-2) and [governed result contract investigation](#mvt-7-retest-2--governed-engineering-result-contract-investigation-2026-10-02) |
| **MVT-7 Retest #3** | **Not executed** | **BLOCKED** — required Composer 2.5 **non-fast** cannot be established/verified on Cursor ACP `2026.09.28-64d2043`; see [A4-T6 closeout / blocker](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#a4-t6--t7--closeout-preparation-pa-2026-10-02). **Not** a Pass or Fail execution — no governed Forward record. |
| MVT-8–11 | **Blocked** | External Cursor ACP Composer 2.5 **non-fast** capability — no governed Forward path for Group A |
| MVT-12–14 | **Blocked** | Depends on Group A forward-in-progress / recovery chain |
| MVT-15 | **Not executed** | Human AF-1 follow-up — **blocked** on MVT-12 chain |
| MVT-16–18 | **Pass** | Human execution 2026-10-02 — P0 Prepare / Copy / Import (independent of Group A) |
| MVT-19 | **Pass** | Human execution 2026-10-02 — automation unavailable with project-scoped provider **enabled** preference cleared (see execution record) |
| MVT-20–22, MVT-24 | **Pass** | Human execution 2026-10-02 — Group D regression / boundary (operator-visible) |
| MVT-23 | **Blocked** | Requires MVT-8 live forward (external provider capability) |

### Remediation and retest context (does not alter MVT-7 Fail)

1. **Original MVT-7 failure cause:** Published T6 `CursorAcpClient` sent an ACP `initialize` payload incompatible with live `agent acp` (missing / invalid **`protocolVersion`** contract per Cursor ACP docs). Governed forward returned **`ProviderUnavailable`** / Attention **`initialization-failure`**; no `transport_operation` row.
2. **Corrective remediation:** Exists **locally and uncommitted** in the ProjectConcord development repository (A4-T6 corrective scope — initialize/authenticate wire alignment, narrow operator Attention ordering, tests). **Not** authorized for publication by this record.
3. **Subsequent authentication blockage (post–initialize fix):** Independently traced to **invalid/stale Cursor CLI credentials** (`agent status` misleading vs `agent -p` / ACP `authenticate`). **Not** classified as an unresolved ProjectConcord defect after human **`agent logout`** / **`agent login`** and authenticated service proof (**AUTH_OK**).
4. **Machine / standalone ACP verification (supporting):** After valid login — `initialize` → `authenticate` (`cursor_login`) → `session/new` succeed; `CursorEngineeringAgentProviderPlugin.InitializeAsync` succeeds (see remediation notes).
5. **MVT-7 Retest #1** is a **new** human execution entry. It does **not** change the original MVT-7 **Fail** row.
6. **A4-T6 Corrective Remediation #2 (2026-10-02, local):** PA-authorized message-pump + governed `ProjectLocator` on forward contract; live diagnostic (not governed Forward). Evidence: [Remediation #2 section](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#a4-t6-corrective-remediation-2-2026-10-02-local--uncommitted). **Does not** alter MVT-7 / Retest #1 **Fail** rows.
7. **Remediation #2 PA acceptance (2026-10-02):** Accepted **locally for governed MVT Retest #2** only. Retest #1 transport row `cac3c9c8-6817-4a4c-9c01-6c9d42cc1937` remains **historical evidence** — do not mutate, delete, reconcile, or reuse its operation identity for Retest #2.
8. **MVT-7 Retest #2 disposition (2026-10-02):** **Fail** — transport remediation **validated**; failure at governed **`EngineeringResultImport`** import boundary. See [investigation](#mvt-7-retest-2--governed-engineering-result-contract-investigation-2026-10-02). **A4-T7 paused**; **MVT-8+ not executed**; **publication not authorized**.
9. **A4-T6 Corrective Remediation #3 (2026-10-02, local):** Provider-neutral **Engineering Result response contract** composed at automated execution boundary (Option C). Evidence: [Remediation #3](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#a4-t6-corrective-remediation-3-2026-10-02-local--uncommitted). **Does not** alter MVT-7 / Retest #1–2 **Fail** rows; **no** Retest #3 execution record. **Live effectiveness:** architecture accepted locally; structural parse **failed** on live composed-prompt diagnostic — see [follow-up investigation](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#live-result-conformance-follow-up-investigation-2026-10-02-investigation-only). **Retest #3 not authorized.**
10. **A4-T6 Corrective Remediation #3B (2026-10-02, local):** **Complete** renderer-generated output contract + instructional template (Remediation #3 descriptive contract superseded at automated boundary). Evidence: [Remediation #3B](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#a4-t6-corrective-remediation-3b-2026-10-02-local--uncommitted). Automated tests **pass** (excluding `RequiresCursor`). **One** provider-level live diagnostic: ACP **OK** (~31s); `TryParseEngineeringResult` **still `RejectedMalformed`** (`relay.render.version.missing` — preamble / render header not line-start). **#3B live effectiveness: FAIL.** **Retest #3 not authorized.**
11. **A4-T6 Corrective Remediation #3D (2026-10-02, local):** Provider-neutral **automated-only** exact document extraction (C2 START, PA reminder END) before unchanged `TryParseEngineeringResult`. Evidence: [Remediation #3D](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#a4-t6-corrective-remediation-3d-2026-10-02-local--uncommitted). **Does not** alter #3B live FAIL record or MVT rows. **Retest #3 not authorized.**
12. **A4-T6 Corrective Remediation #4–#5 (2026-10-02, local):** Investigation (#4); configurable verified Cursor model selection + profile derivation (#5). Evidence: [Remediation notes](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md). **Does not** rewrite MVT **Fail** rows.
13. **A4-T6 Corrective Remediation #6 (2026-10-02, local):** **PA ACCEPTED** — thin automated result profile; proof of **Cursor ACP provider capability blocker** for Composer 2.5 non-fast. **Retest #3 NOT EXECUTED / BLOCKED** on external provider — not a successful MVT-7. See [closeout preparation](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#a4-t6--t7--closeout-preparation-pa-2026-10-02).

### PA-reconfirmed scope (A4-T7)

| Group | Intent |
|---|---|
| **A** | Automated Cursor forward where environment supports authenticated CLI |
| **B** | Recovery confirmation — no automatic import; explicit follow-up required |
| **C** | Permanent P0 manual Prepare / Copy / Import smoke |
| **D** | Regression / governance-boundary checks (operator-visible, T0–T6 scope only) |

## Implementation scope

| Anchor | Value |
|---|---|
| Repository paths | `src/Edf.Desktop/`, `src/Edf.Application/Relay/`, `src/Edf.Application/Operator/`, `src/Edf.Application/Relay/EngineeringAgent/Providers/Cursor/` |
| Branch / commit / tag | **Historical MVT-1–7:** `main` @ **`eecf0538bd4623d837c93d06439a39b2b6c0abe0`**. **MVT-7 Retest #1:** local working tree at `/Users/edbecnel/Development/GitHub/ProjectConcord` with **uncommitted** A4-T6 corrective remediation (HEAD still `eecf0538…` until published) |
| Environment or build | macOS arm64; .NET SDK **10.0.401**; Cursor **`agent`** CLI with **`agent acp`** when exercising Group A |
| Out of scope (explicit) | A3; new Desktop plugin-selection UI; `src/` remediation under T7; MVR subject = ProjectConcord development repository |

## Operator environment and test data

Normative disposable-workspace semantics: [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md).

Fixture and plugin-preference setup: [docs/Verification/Fixtures/MVR-0003/README.md](../Fixtures/MVR-0003/README.md).

### Preconditions (operator — verify before MVT-1)

| Precondition | Expected state |
|---|---|
| ProjectConcord repository | Checked out at **`eecf0538bd4623d837c93d06439a39b2b6c0abe0`** (or equivalent published T6/T7 baseline) |
| Disposable Project Root | **`/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A`** exists; **no** `.projectconcord/` before first open |
| .NET SDK | **10.0.401** (`global.json`; use `DOTNET_ROOT=$HOME/.dotnet` if needed) |
| Paste fixtures | Reuse [MVR-0002 fixtures](../Fixtures/MVR-0002/) paths listed in MVR-0003 fixture README |
| Forbidden MVR subjects | ProjectConcord **development** repository; production EDF repositories |

### Prepare and launch ProjectConcord (required — first operator action)

From the **ProjectConcord development repository root** (not the disposable Project Root):

**MVT-1–6 (historical):** Debug launch at published baseline.

**MVT-7 Retest #1 (binding):** **Quit** every running ProjectConcord Desktop instance — including the process used for the **original MVT-7 Fail** (stale pre-remediation binary). **Do not** reuse that process. **Do not** launch a stale installed `.app` without the environment below. Run from the **current uncommitted corrective** tree:

```bash
cd /Users/edbecnel/Development/GitHub/ProjectConcord
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$HOME/.local/bin:$PATH"
export PROJECTCONCORD_CURSOR_AGENT_PATH="$HOME/.local/bin/agent"
dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Release
```

Optional explicit build first: `dotnet build -c Release --project src/Edf.Desktop/Edf.Desktop.csproj` then `dotnet run … -c Release --no-build`.

**Expected:** Build succeeds; ProjectConcord window opens. NU1903 SQLite warnings alone are not MVT failure.

**Before Retest #1 forward:** Confirm `agent status` shows logged in and `agent -p --trust` can reach the service (human attested **AUTH_OK** after re-login). No API key.

**MVT-7 Retest #2 (binding — when PA authorizes human Forward):** **Quit** every running ProjectConcord Desktop instance — including the process used for **Retest #1**. **Do not** reuse any prior MVT-7 Desktop process. Launch from the **current uncommitted** tree containing **Corrective Remediation #1 and #2**:

```bash
cd /Users/edbecnel/Development/GitHub/ProjectConcord
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$HOME/.local/bin:$PATH"
export PROJECTCONCORD_CURSOR_AGENT_PATH="$HOME/.local/bin/agent"
dotnet build -c Release --project src/Edf.Desktop/Edf.Desktop.csproj
dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Release --no-build
```

Optional single-step: omit `dotnet build` only if a Release build was just produced from this tree.

**Before Retest #2 forward:** Run read-only prechecks in [Retest #2 preparation](#mvt-7-retest-2--preparation-2026-10-02). Human auth precheck: `agent status` (re-login only if invalid). No API key.

### Recording results

- For each MVT: perform the procedure, compare to **Expected result**, update **Execution record** (**Pass** / **Fail** / **Blocked** / **N/A** only).
- Check the MVT checkbox **only after** **Result** is **Pass**.
- On **Fail** or **Blocked**: **STOP** further MVTs in the affected group; return to Project Architect (T7 does **not** authorize `src/` remediation).
- Do **not** mark Group A **Pass** if Cursor CLI cannot be authenticated when forward is attempted — record **Blocked** with evidence.

## Manual verification — human execution required

### Group A — Automated Cursor forward

- [x] [[#^mvt-1-record|MVT-1]] — Launch Desktop

  **Procedure:** Confirm preconditions; launch per **Prepare and launch** above.

  **Expected result:** Main window; **File → Open Project Folder…** available; no unhandled startup exception.

- [x] [[#^mvt-2-record|MVT-2]] — Open disposable Project Root

  **Procedure:** Open **`/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A`**; record **Project ID** (GUID).

  **Expected result:** Relay panel enabled; governed relay controls visible.

- [x] [[#^mvt-3-record|MVT-3]] — Import valid PA handover

  **Procedure:** Paste entire contents of [pa-handover-valid.relay.txt](../Fixtures/MVR-0002/pa-handover-valid.relay.txt); import PA handover.

  **Expected result:** Validation summary indicates **Valid** / eligible for Engineering Agent handover (equivalent to MVR-0002 valid fixture semantics).

- [x] [[#^mvt-4-record|MVT-4]] — Session intents (explicit)

  **Procedure:** Set **Project Architect session** and **Engineering Agent session** per [ui-session-settings-for-valid-fixtures.md](../Fixtures/MVR-0002/ui-session-settings-for-valid-fixtures.md).

  **Expected result:** ComboBox selections stick (not blank); status reflects intent recorded.

- [x] [[#^mvt-5-record|MVT-5]] — Cursor `agent` CLI readiness

  **Procedure:** With Desktop **quit**, in a terminal run `agent --version`, `agent acp --help`. Confirm authentication/readiness for your environment (sign-in, API key, or org policy as applicable). Re-launch Desktop with `PROJECTCONCORD_CURSOR_AGENT_PATH` if needed.

  **Expected result:** CLI commands succeed; verifier attests agent is **ready to run** `agent acp` for forward. If not ready, set **Blocked** for MVT-6–MVT-11 and STOP Group A for PA disposition.

- [x] [[#^mvt-6-record|MVT-6]] — Enable production Cursor provider for Project

  **Procedure:** Quit Desktop. Apply sqlite preference keys for recorded **Project ID** per [MVR-0003 fixture README](../Fixtures/MVR-0003/README.md) (`cursor-acp-reference` selected + enabled). Re-launch Desktop; re-open same disposable root.

  **Expected result:** **Automated transport** status no longer shows sole cause “No provider selected”; Attention does **not** show `plugin-unavailable` solely due to missing selection (auth/health Attention may still appear until forward).

  **After Desktop quit/relaunch (operator addendum — binding for Retest #1):**

  1. Re-open the **same** disposable Project Root: `/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A`.
  2. Confirm the **same Project ID**: `a5e26be6-9769-46f6-bd94-1669764fe8af` (fixture sqlite keys must match this GUID).
  3. Verify **session intents** restored in UI (MVT-4 values); re-set if blank.
  4. **Re-import** valid PA handover ([MVR-0002 fixture](../Fixtures/MVR-0002/pa-handover-valid.relay.txt)) — import/Prepare eligibility is **not** persisted across restart.
  5. **Prepare / validate** Engineering Agent handover per operator workflow (valid PA → eligible for forward).
  6. Re-verify **Cursor CLI** authentication (`agent status`; service reachability) before forward.
  7. Re-verify provider preferences (`cursor-acp-reference` selected + enabled) per [fixture README](../Fixtures/MVR-0003/README.md).

  Original MVT-6 observation (initialization-failure Attention before remediation) remains part of the historical record.

- [x] [[#^mvt-7-record|MVT-7]] — Production provider is Cursor reference plugin (**original execution — Fail**)

  **Procedure:** After MVT-6, attempt **Forward handover (automated)** once (may fail fast if auth pending). Observe **Automated transport** status text and any **Attention** detail referencing provider initialization, auth, or forward (not a separate governance tranche grant).

  **Expected result:** Forward path targets the registered production provider (**`cursor-acp-reference`** semantics — ACP subprocess). Status/Attention remain **derived** reporting, not canonical governance state. If forward cannot start, record observation; do not infer Pass without provider engagement.

  **Status:** **Fail** (2026-10-02) — see execution record; **do not** re-execute as MVT-7.

- [x] [[#^mvt-7-retest-1-record|MVT-7 Retest #1]] — Forward after A4-T6 corrective remediation (**authorized human execution**)

  **Scope:** **Only** one click **Forward handover (automated)** after corrected Desktop launch and restart procedure above. **Not** MVT-8; do not retry forward; do not import Engineering result; do not alter recovery state.

  **Procedure:** Launch Desktop per **MVT-7 Retest #1** prepare block; complete restart/re-establishment steps; click **Forward handover (automated)** **exactly once**; observe immediate transport/provider outcome; **STOP for PA review** (Pass/Fail/Blocked — do not predeclare Pass).

  **Success observation (minimum):** Evidence whether forward progresses **beyond** original **`ProviderUnavailable` / `initialization-failure`** boundary (e.g. transport row created, forward in progress, explicit provider engagement, or a different failure class). Overlap with MVT-8 evidence is acceptable; **do not** mark MVT-8 executed.

  **Failure observation:** If **`ProviderUnavailable`** or initialization-failure recurs — **do not retry**; preserve UI state and any `transport_operation` evidence; STOP to PA.

- [x] [[#^mvt-7-retest-2-record|MVT-7 Retest #2]] — Forward after Corrective Remediation #2 (**Fail** 2026-10-02 — transport remediation validated; governed result import failed; checkbox open until PA confirms disposition)

  **Reason:** Governed retest after Remediation #2 addressing ACP `session/prompt` message ordering/completion and authoritative **Project Root** propagation (`GovernedProjectRoot` → `session/new.cwd`).

  **Scope:** **Only** one click **Forward handover (automated)** after fresh Desktop launch and re-establishment below. **Not** MVT-8. **Not** a retry of Retest #1. Retest #2 must use a **new** `transport_operation` if persistence occurs. **Do not** mutate Retest #1 row `cac3c9c8-6817-4a4c-9c01-6c9d42cc1937`.

  **Procedure:** Follow [Retest #2 human procedure](#mvt-7-retest-2--human-procedure-binding). **STOP for PA / Engineering Agent evidence collection** after the single attempt returns or after the 5-minute observation window.

  **Success boundary (minimum):** Forward **returns** to orchestrator with lifecycle **beyond** `ForwardInProgress` (Retest #1 already proved row creation alone is insufficient). Coherent provider session/prompt completion or explicit failure per transport contract.

  **UI blocking:** Temporary UI unresponsiveness during in-flight ACP is **expected** (sync Forward not remediated). **Not** Fail solely for blocking or for stale `initialization-failure` Attention **while** Forward is executing. After return, record whether projections refresh.

- [ ] [[#^mvt-8-record|MVT-8]] — Automated forward reaches provider

  **Procedure:** With valid PA handover and MVT-5 readiness, click **Forward handover (automated)**. Allow operation to progress (may take minutes; provider may prompt for permissions in CLI — respond per your policy).

  **Expected result:** Transport status advances beyond idle (e.g. forward acknowledged / awaiting result / completed / or explicit failure Attention). **Blocked** if provider never receives handover due to environment — do not fabricate Pass.

- [ ] [[#^mvt-9-record|MVT-9]] — Transport status projection coherent

  **Procedure:** During and after MVT-8, compare **Automated transport** summary, relay status message, and **Attention (derived)** items.

  **Expected result:** Messages describe transport/provider state only; they do **not** assert PA implementation authorization or tranche publication.

- [ ] [[#^mvt-10-record|MVT-10]] — Result candidate passes governed import boundary

  **Procedure:** If forward yields a provider result candidate that is **valid** under relay rules, observe whether Engineering result is accepted via governed import (automated path may complete import in-session). If import is **rejected**, observe **ImportRejected** / attention reporting without treating provider text as canonical.

  **Expected result:** Canonical governed acceptance follows **relay validation** outcomes — provider output alone does not bypass validators.

- [ ] [[#^mvt-11-record|MVT-11]] — No provider-manufactured governance authority

  **Procedure:** Read **Authorization boundary notice** and relay provenance/summary after forward. Confirm no new “implementation authorized” or tranche grant appeared solely because Cursor returned text.

  **Expected result:** Notice unchanged in substance; no provider-specific governance semantics promoted to canonical ProjectConcord state.

### Group B — Recovery confirmation (AF-1)

_T5–T6 Desktop does not expose `ConfirmRecoveredResultImportAsync` as a button; human verification focuses on **operator-visible** recovery signals and **absence of silent import**. Application AF-1 confirm API is additionally covered by automated T4 tests (supporting only)._

- [ ] [[#^mvt-12-record|MVT-12]] — Establish recoverable transport state (interrupted forward)

  **Procedure:** Repeat MVT-3–MVT-4 on a **fresh** disposable session or after clearing transport state for the project if needed. Configure provider per MVT-6. Start **Forward handover (automated)**; while status indicates forward in progress (not yet completed import), **force-quit** Desktop (macOS Force Quit). Re-launch and re-open the same disposable root.

  **Expected result:** Application restarts without crash loop; relay panel loads.

- [ ] [[#^mvt-13-record|MVT-13]] — No automatic import on restart

  **Procedure:** After MVT-12, before any manual import, inspect Engineering result import area and relay provenance/summary for a **new** engineering result import attributable to recovery startup.

  **Expected result:** **No** automatic `ImportEngineeringResult` on startup; engineering result field not populated by recovery alone.

- [ ] [[#^mvt-14-record|MVT-14]] — Attention surfaces recoverable operation

  **Procedure:** Observe **Attention (derived)** list after MVT-12 restart.

  **Expected result:** Attention includes recoverable / ambiguous transport guidance (e.g. recoverable operation or forward-in-progress follow-up) with **Recommended** urgency — not a governance **Required** tranche action.

- [ ] [[#^mvt-15-record|MVT-15]] — Explicit operator action before governed acceptance

  **Procedure:** Without using any hidden automation, determine what is required to complete a recovered engineering result: either explicit P0 paste/import or documented operator follow-up. Confirm recovered candidate text was **not** promoted to canonical engineering result import solely because transport recovered.

  **Expected result:** Governance acceptance requires **explicit** operator import/follow-up consistent with AF-1; transport recovery alone does not imply acceptance.

### Group C — P0 manual fallback

- [x] [[#^mvt-16-record|MVT-16]] — Prepare engineering handover (manual)

  **Procedure:** With valid PA handover imported, click **Prepare engineering handover** (manual P0).

  **Expected result:** Rendered handover text appears; Copy path available.

- [x] [[#^mvt-17-record|MVT-17]] — Copy handover (manual transport)

  **Procedure:** Use **Copy** for rendered engineering handover.

  **Expected result:** Clipboard receives handover text; no automated transport required.

- [x] [[#^mvt-18-record|MVT-18]] — Import engineering result (manual)

  **Procedure:** Paste [engineering-result-import-thin.relay.txt](../Fixtures/MVR-0002/engineering-result-import-thin.relay.txt) into Engineering result import; import.

  **Expected result:** Governed import outcome reported (valid or explicit rejection) via existing relay validation — independent of Cursor transport.

- [x] [[#^mvt-19-record|MVT-19]] — P0 viable when automation unavailable

  **Procedure:** Disable Cursor plugin for project (clear enabled preference) or stop `agent` availability; re-open project. Confirm **Recommended** manual relay guidance and that Prepare/Copy/Import remain usable with valid PA handover.

  **Expected result:** P0 path works; PA package validity unchanged by transport/provider failure.

### Group D — Regression / boundaries

- [x] [[#^mvt-20-record|MVT-20]] — No DEBUG Engineering Agent mode in UI

  **Procedure:** Inspect **Engineering Agent mode** options.

  **Expected result:** Only **Plan** and **Agent** (or equivalent) — **no** DEBUG option; no silent DEBUG→AGENT remap in UI.

- [x] [[#^mvt-21-record|MVT-21]] — Attention is derived

  **Procedure:** Trigger a transport Attention (failed forward or plugin unavailable). Read section label.

  **Expected result:** UI shows **Attention (derived)** — informational, not canonical governance.

- [x] [[#^mvt-22-record|MVT-22]] — Manual fallback Recommended, not Required

  **Procedure:** When automation Attention present and P0 eligible, read manual relay guidance.

  **Expected result:** Wording reflects **Recommended** manual P0 — not **Required** governance command.

- [ ] [[#^mvt-23-record|MVT-23]] — Provider permission is not governance

  **Procedure:** During live forward (MVT-8), if provider requests permission via CLI/ACP, observe Desktop relay panel.

  **Expected result:** No Desktop control presents provider permission as ProjectConcord governance authorization.

- [x] [[#^mvt-24-record|MVT-24]] — Authorization boundary notice

  **Procedure:** Read gray **Authorization boundary notice** at top of relay panel.

  **Expected result:** Substance equivalent to MVR-0002: handover/PA acceptance do **not** grant implementation authorization.

## Execution record

### MVT-1 — Launch Desktop

| Field        | Value                                                          |
| ------------ | -------------------------------------------------------------- |
| **Result**   | Pass                                                           |
| **Executor** | Ed Becnel                                                      |
| **Date**     | 2026-10-02                                                     |
| **Evidence** | App opens normally. No crash and UI looks good with no errors. |
^mvt-1-record

### MVT-2 — Open disposable Project Root

| Field        | Value                                                           |
| ------------ | --------------------------------------------------------------- |
| **Result**   | Pass                                                            |
| **Executor** | Ed Becnel                                                       |
| **Date**     | 2026-10-02                                                      |
| **Evidence** | Relay panel is enabled and governed relay controls are visible. |
^mvt-2-record

### MVT-3 — Import valid PA handover

| Field        | Value                                                                                                      |
| ------------ | ---------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                       |
| **Executor** | Ed Becnel                                                                                                  |
| **Date**     | 2026-10-02                                                                                                 |
| **Evidence** | PA handover import: Valid<br>PA handover is Valid and eligible for Engineering Agent handover preparation. |
^mvt-3-record

### MVT-4 — Session intents

| Field        | Value                                                                                                                                                                                                                                                       |
| ------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                                                                                                                                                                        |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                   |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                  |
| **Evidence** | Project Architect session set to Continue; Engineering Agent session set to New; Engineering Agent mode set to Agent; Prior Engineering Agent mode set to Plan. All selections remained populated and the recorded session intent was reflected by the UI.` |
^mvt-4-record

### MVT-5 — Cursor agent CLI readiness

| Field        | Value                                                                                                                                                                                                                                                                                                               |
| ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                                                                                                                                                                                                                                |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                                           |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                                          |
| **Evidence** | Copied from the terminal:<br>(base) edbecnel@Eds-Mac-mini ~ % agent --version<br>2026.09.28-64d2043<br><br>(base) edbecnel@Eds-Mac-mini ~ % agent acp --help<br>Usage: agent acp [options]<br>Start the Cursor Agent as an ACP (Agent Client Protocol) server<br>Options:<br>  -h, --help  Display help for command |
^mvt-5-record

### MVT-6 — Enable Cursor provider preferences

| Field        | Value                                                                                                                                                                                                                                                                   |
| ------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                                                                                                                                                                                 |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                               |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                              |
| **Evidence** | cursor-acp-reference configured for the disposable project. Automated transport no longer reports "No provider selected" or plugin-unavailable due to missing selection. Attention reports provider initialization-failure / provider has not completed initialization. |
^mvt-6-record

### MVT-7 — Production provider selection semantics

| Field        | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| ------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Fail                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| **Evidence** | **Original governed execution only:** One **Forward handover (automated)** → `ProviderUnavailable` / Attention **`initialization-failure`**; SQLite `transport_operation` count **0**; Desktop child env included `PROJECTCONCORD_CURSOR_AGENT_PATH=/Users/edbecnel/.local/bin/agent`. **Failure cause:** published T6 Cursor ACP **`initialize`** request omitted/violated live required **`protocolVersion`** contract. Desktop process predated local corrective implementation. Remediation and Retest #1 context: [Remediation and retest context](#remediation-and-retest-context-does-not-alter-mvt-7-fail) and [A4-T6 corrective remediation notes](../../Handover/ProjectConcord-A4-T6-Corrective-Remediation-Notes.md).<br><br>Single governed Forward failed before transport dispatch with `ProviderUnavailable` / `initialization-failure`. No `transport_operation` was created. Root cause was the published T6 Cursor ACP initialization request not satisfying the live ACP `protocolVersion` contract. This is the original historical MVT-7 failure and remains preserved as Fail. |
^mvt-7-record

### MVT-7 Retest #1 — Forward after A4-T6 corrective remediation

| Field        | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | **Fail**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| **Reason**   | Retest after A4-T6 Cursor ACP initialize-protocol corrective remediation and restoration of valid Cursor CLI authentication (logout/login; **AUTH_OK** service proof).                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| **Evidence** | **Human (single Forward):** UI appeared unchanged immediately after one **Forward handover (automated)** click; derived Attention remained `operator.attention.engineering-agent.initialization-failure` (“provider has not completed initialization”); no Engineering result import; no second Forward. Provider prefs verified immediately before attempt: `engineering_agent.plugin.selected.a5e26be6-9769-46f6-bd94-1669764fe8af` and `engineering_agent.plugin.enabled…` = `cursor-acp-reference`. Desktop launched fresh from uncommitted corrective tree (`dotnet run … -c Release`) with `PROJECTCONCORD_CURSOR_AGENT_PATH=$HOME/.local/bin/agent`.<br><br>**SQLite `user-state.db` (`transport_operation`):** Count **1** (was **0** after original MVT-7). Row attributable to Retest #1 via `relay_package.project_id` = `a5e26be6-9769-46f6-bd94-1669764fe8af`: `transport_operation_id` = `cac3c9c8-6817-4a4c-9c01-6c9d42cc1937`; `source_package_id` = `34ed41c1-d515-4e2d-a794-461dc9cb1e67`; `correlation_id` = `786373c5-ae7b-4851-b851-773cf4a02f33`; `provider_plugin_id` = `cursor-acp-reference`; `attempt` = 1; `lifecycle_state` = **1 (`ForwardInProgress`)**; `provider_session_hint` = **NULL**; `created_utc` / `updated_utc` = `2026-10-02T05:42:11.9116210+00:00` / `2026-10-02T05:42:11.9132680+00:00` (no post-dispatch lifecycle update persisted).<br><br>**Engineering Agent inference (read-only, from orchestrator ordering + row):** Governed forward passed preflight, **`InitializePluginAsync`**, runtime readiness, transport **create**, and **pre-dispatch `ForwardInProgress` persistence** — i.e. **beyond** original MVT-7 pre-dispatch failure (`transport_operation` count 0). `Forward()` likely entered provider **`RunPromptAsync`** (ACP `session/new` / `session/prompt` path) before orchestrator could advance lifecycle or refresh UI outcome. **Not** conclusive successful automated forward completion.<br><br>**Attention correlation:** Persisted evidence contradicts a **Retest #1** failure at the same **pre-dispatch initialization** boundary as original MVT-7; visible `initialization-failure` is **consistent with pre-forward `PluginNotInitialized` readiness projection** and/or **UI not yet refreshed** while forward was in progress (see `EngineeringAgentTransportOperatorAttentionContributor` readiness path). Recoverable-transport Attention for `ForwardInProgress` may be deduped if same code family not shown.<br><br>**PA disposition:** Pending — Engineering Agent recommends **Fail** for required successful forward behavior; partial progress documented above. **MVT-8 not executed.**<br><br>Retest progressed beyond the original initialization failure: provider initialization and pre-dispatch processing succeeded and a new `transport_operation` was persisted. The operation then stalled at `ForwardInProgress` without completing the provider forward or returning a governed result. This exposed the subsequent ACP session/prompt message-ordering/completion defect. Retest #1 remains preserved as Fail. |
^mvt-7-retest-1-record

### MVT-7 Retest #2 — Preparation (2026-10-02)

Read-only prechecks performed during preparation (**no** `src/` / test changes; **no** Forward executed).

#### Transport-operation precheck (project `a5e26be6-9769-46f6-bd94-1669764fe8af`)

| Item | Finding (2026-10-02 preparation) |
|------|----------------------------------|
| Row count (via `relay_package.project_id`) | **1** |
| Retest #1 row identity | `transport_operation_id` = **`cac3c9c8-6817-4a4c-9c01-6c9d42cc1937`** |
| `source_package_id` | `34ed41c1-d515-4e2d-a794-461dc9cb1e67` |
| `provider_plugin_id` | `cursor-acp-reference` |
| `attempt` | **1** |
| `provider_session_hint` | **NULL** |
| `created_utc` / `updated_utc` | `2026-10-02 05:42:11` / `2026-10-02 05:51:03` (UTC storage) |
| `lifecycle_state` (sqlite integer) | **10** = `Ambiguous` per `TransportOperationLifecycleState` |
| Additional rows | **None** |

**Note:** Retest #1 execution record documents post-click persistence as **`ForwardInProgress` (1)** with no post-dispatch lifecycle advance. Preparation-time sqlite shows **`updated_utc` later than `created_utc`** and **`lifecycle_state` = `Ambiguous` (10)**. Treat the row as **immutable historical Retest #1 evidence**; **do not** reconcile or edit for Retest #2 cleanliness. Retest #2 must create a **distinct** operation if Forward reaches persistence.

#### Provider preference precheck (read-only)

| Key | Value |
|-----|-------|
| `engineering_agent.plugin.selected.a5e26be6-9769-46f6-bd94-1669764fe8af` | `cursor-acp-reference` |
| `engineering_agent.plugin.enabled.a5e26be6-9769-46f6-bd94-1669764fe8af` | `cursor-acp-reference` |

No preference rewrite required for preparation.

#### Authentication precheck (human)

```bash
agent status
```

Preparation observation: **`✓ Logged in as edbecnel@gmail.com`**. No logout/login required unless a later attempt shows invalid auth.

#### Disposable project (binding)

| Field | Value |
|-------|-------|
| Project Root | `/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A` |
| Project ID | `a5e26be6-9769-46f6-bd94-1669764fe8af` |

### MVT-7 Retest #2 — Human procedure (binding)

**When PA authorizes execution** (not during preparation):

1. **Quit** all ProjectConcord Desktop processes (including Retest #1 session).
2. Launch Desktop per **MVT-7 Retest #2** prepare block above (uncommitted tree with Remediation #2).
3. **Open** disposable **Root-A**; confirm Project ID **`a5e26be6-9769-46f6-bd94-1669764fe8af`**.
4. Set session intents: **PA = Continue**, **EA = New**, **EA mode = Agent** (see [ui-session-settings](../Fixtures/MVR-0002/ui-session-settings-for-valid-fixtures.md)).
5. If PA handover not in session: **re-import** [pa-handover-valid.relay.txt](../Fixtures/MVR-0002/pa-handover-valid.relay.txt).
6. **Prepare / validate** Engineering Agent handover; confirm automated Forward is **eligible** (do not click yet).
7. Optional: `agent status` — proceed only if logged in.
8. **Single observation window:** Click **Forward handover (automated)** **exactly once**.
   - Record **immediate** UI behavior, temporary unresponsiveness (expected), **Automated transport** / last result text if shown, **Attention (derived)**, Forward control state, any completion indication.
   - **Wait up to 5 minutes** for the operation to return (live diagnostic ~7s; provider I/O timeout is longer). **Do not** click Forward again; **do not** force-quit solely for UI block.
   - If still blocked with no explicit failure after **5 minutes**: record that fact; **do not retry**; **STOP for PA**.
9. After return: note whether UI responsiveness and Attention **change** (stale `initialization-failure` during flight is **not** Fail by itself).
10. **Do not** manipulate SQLite immediately; Engineering Agent collects `transport_operation` / provenance evidence read-only.

### MVT-7 Retest #2 — Forward after Corrective Remediation #2

| Field           | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| --------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**      | **Fail** (PA disposition 2026-10-02)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| **Executor**    | Ed Becnel                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| **Date**        | 2026-10-02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| **Reason**      | Governed retest after **Corrective Remediation #2** addressing ACP `session/prompt` message ordering/completion (`session/update` during RPC; terminal `stopReason` / `end_turn`) and authoritative **Project Root** propagation (`GovernedProjectRoot` → Cursor `session/new.cwd`; not `Environment.CurrentDirectory`).                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| **Disposition** | **Fail** — automated Forward completed through corrected Cursor ACP transport but provider candidate was **not** a valid governed **`EngineeringResultImport`**; rejected at existing import boundary. **Distinct from Retest #1** (`ForwardInProgress` / stall). **Remediation #2 transport objectives:** PA validated separately.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| **Evidence**    | **Human (single Forward):** Governed Forward **returned** to Desktop UI (~53s in-flight). Post-return: **Last automated transport** / lifecycle **`ImportRejected`**; status *“Automated transport completed with outcome ImportRejected.”*; Forward control re-enabled. Derived Attention: **`operator.attention.engineering-agent.result-retrieval-failure`** (“Automated result candidate could not be imported under governed relay rules.”) and **`operator.attention.engineering-agent.recoverable-transport`** (“Recoverable automated transport operation (Ambiguous) requires explicit operator follow-up.”). **Not** a `ForwardInProgress` stall (contrast Retest #1).<br><br>**SQLite `transport_operation` (post-execution, read-only):** Project `a5e26be6-9769-46f6-bd94-1669764fe8af` — **2** rows. **Retest #1 (historical, unchanged identity):** `cac3c9c8-6817-4a4c-9c01-6c9d42cc1937` — `lifecycle_state` **10 (`Ambiguous`)**; `provider_session_hint` NULL; `result_import_package_id` NULL; `created_utc` / `updated_utc` `2026-10-02T05:42:11Z` / `2026-10-02T05:51:03Z`. **Retest #2 (new operation):** `82d9d6ce-69a0-4f99-aded-00b350cdedf7` — `source_package_id` `34ed41c1-d515-4e2d-a794-461dc9cb1e67`; `correlation_id` `786373c5-ae7b-4851-b851-773cf4a02f33`; `provider_plugin_id` `cursor-acp-reference`; `attempt` **1**; `lifecycle_state` **7 (`ImportRejected`)**; `provider_session_hint` **`5f27f83f-f0f9-4d94-8a1d-1858b2e67a62`**; `result_import_package_id` **NULL**; `created_utc` / `updated_utc` `2026-10-02T06:16:12Z` / `2026-10-02T06:17:05Z`.<br><br>**Inferred lifecycle path (Retest #2, orchestrator + row):** Plugin initialization → `ForwardInProgress` persistence → provider `Forward` (ACP `session/prompt` path; session id persisted) → `ForwardAcknowledged` → result candidate retrieved → `ResultCandidateReceived` (transient) → `ImportEngineeringResult` on provider text → **governed parse/import boundary rejection** → terminal **`ImportRejected`**. First unsuccessful boundary: **step 13–14** (automated import of provider candidate), not provider transport stall.<br><br>**Import rejection (read-only/code correlation):** Candidate obtained via `CursorEngineeringAgentProviderPlugin.TryGetResultCandidate` (streamed ACP agent text marked `IsReadyForParse: true`). `GovernedRelayP0WorkflowService.ImportEngineeringResult` → `TryParseEngineeringResult` → `GovernedRelayV1Importer` requires governed-relay machine block; plain agent prose **does not** produce a `GovernedPackageKind.EngineeringResultImport` package → `import.Package is null` / non-`Valid` validation → orchestrator persists **`ImportRejected`** with default detail *“Engineering result import boundary rejected the provider candidate.”* No `relay_package` row created for engineering result import (`result_import_package_id` NULL).<br><br>**Handover / source package:** Both operations share `source_package_id` **`34ed41c1-d515-4e2d-a794-461dc9cb1e67`** (`package_kind` **2** = `EngineeringAgentHandoverExport` in sqlite; profile includes **`A2-MVR`** tranche markers — consistent with bound MVR PA handover fixture, not a separate wrong project). Forward used the governed prepared/rendered handover for that package; visible A2-MVR text reflects **fixture content**, not evidence of an unintended package swap.<br><br>**Ambiguous recoverable Attention:** `GetRecoverableOperations` includes operations in **`Ambiguous`** (and `ForwardInProgress`, etc.) but **not** `ImportRejected`. Retest #2 row is terminal `ImportRejected`. Recoverable **(Ambiguous)** Attention refers to **Retest #1 historical row** `cac3c9c8-…`, not Retest #2.<br><br>**EA mode persistence (human):** On fresh launch UI **Engineering Agent mode** reset to **Plan**; human set **Agent** before Forward. Session intents persisted in `relay_continuity` (PA Continue, EA New); **EA routing mode** is ViewModel-local default (`Plan`) and is **not** reloaded from sqlite on project open — separate continuity gap (no remediation this tranche).<br><br>Retest validated the corrected ACP transport path: the automated Forward reached Cursor, completed the provider interaction, and returned to ProjectConcord. The resulting provider candidate was rejected at the governed Engineering Result import boundary (`ImportRejected`) because Cursor returned ordinary agent prose rather than a valid `projectconcord-relay-v1` `EngineeringResultImport` document. Transport remediation was validated, but the governed end-to-end test remained Fail. |
^mvt-7-retest-2-record

### MVT-7 Retest #2 — Governed Engineering Result contract investigation (2026-10-02)

**Scope:** Investigation only — no `src/` / test changes; no Forward; importer boundary not weakened.

#### Q1 — Existing manual contract (canonical)

| Topic | Finding |
|-------|---------|
| **Canonical spec** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) **PC-PAR-022** — `IEngineeringAgentRelayBridge` **render/parse** of `EngineeringAgentHandoverExport` and **`EngineeringResultImport`** over **`projectconcord-relay-v1`**. [ADR-0021](../../Architecture/ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) §4, §6; [ADR-0022](../../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) §2–§3. |
| **Prepare handover** | `TryRenderValidatedHandover` → `GovernedRelayV1Renderer.Render` — machine block (`kind: engineeringAgentHandoverExport`) + governance projections + **single reminder** (`GovernedRelayV1Format.PaEngineeringAgentReminder`) about **next handover** mode fields — **not** about engineering **result** shape. |
| **Manual obtain result** | P0: human copies prepared handover to external EA; human **pastes** returned text into **Engineering result import** (`RelayWorkflowViewModel.ImportEngineeringResultAsync` → `ImportEngineeringResult`). MVR-0002 MVT-15 uses fixture [`engineering-result-import-thin.relay.txt`](../Fixtures/MVR-0002/engineering-result-import-thin.relay.txt) as **separate** paste — not embedded in handover render. |
| **Expected result structure** | `ProjectConcord-Relay-Render: 1` + fenced **` ```projectconcord-relay-v1` ** JSON with **`kind: engineeringResultImport`** (+ governance projections). Parsed by `GovernedRelayV1Importer` / `TryParseEngineeringResult` ([`EngineeringAgentManualRelayBridge`](../../../src/Edf.Application/Relay/EngineeringAgent/EngineeringAgentManualRelayBridge.cs)). |
| **EA instructed in handover?** | **No** — outbound handover render does **not** include canonical instructions to respond with an `EngineeringResultImport` relay document. |

#### Q2 — Retest #2 prompt / instructions

| Question | Answer |
|----------|--------|
| **Was Cursor instructed to return governed relay v1 Engineering Result?** | **NO** (for Retest #2 as implemented). |
| **Actual ACP prompt body** | `EngineeringAgentForwardRequest.RenderedHandoverBody` — byte-equivalent to P0 **`TryRenderValidatedHandover`** output for source package `34ed41c1-…` (includes A2-MVR **authorization/work-context** in machine JSON and human projections — expected MVR fixture content, not a wrong package). |
| **Where instruction is absent** | No layer augments outbound text with **result-import response contract** before `CursorEngineeringAgentProviderPlugin.Forward` passes body to `session/prompt`. Provider adds **no** result-format appendix. |
| **Why response did not comply** | Cursor returned normal **agent message chunks** (streamed prose). Importer correctly failed: **missing `projectconcord-relay-v1` machine block** (`RelayValidationCodes.MachineBlockMissing` path). |

#### Q3 — Responsibility options (evaluation)

| Option | Consistency |
|--------|-------------|
| **A — EA responsibility via handover alone** | **Insufficient today** — handover render does not state result format; manual P0 relies on **operator + separate fixture knowledge**, not handover text alone. |
| **B — Provider adapter adds format instructions** | Works mechanically but **weakens provider neutrality** if each plugin invents wording; ADR-0021 §6 requires **same rendered representation as P0** for forward payload — result **response** contract is a **separate** concern and should not be Cursor-only. |
| **C — Host/orchestrator or relay bridge augments outbound payload** | **Best fit** — provider-neutral **response contract** appended or composed before dispatch; all plugins receive identical requirement; parse path unchanged ([ADR-0022](../../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) §3). |
| **D — Host synthesizes package from prose** | **Rejected** — conflicts ADR-0021 §2 (must not manufacture governance-critical fields), §9 (untrusted intake), and `ProseCannotManufactureEngineeringAgentMode` tests. |

#### Q4 — `IsReadyForParse`

| Item | Finding |
|------|------|
| **Documented intent** | `EngineeringAgentTransportResultCandidate` XML: **does not imply** valid engineering result or governance acceptance ([ADR-0022](../../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) §3 — candidate ≠ acceptance). |
| **Runtime use** | Orchestrator checks `HasCandidate` / non-null candidate only — **flag is not read** in `EngineeringAgentTransportOrchestrator`. |
| **Cursor plugin** | Sets **`true`** whenever `ResultText` non-empty after ACP completion — effectively **(A) text available**, not **(B) parser-ready**. |
| **Tests** | `Forward_InvalidResultCandidate_ImportRejected` uses `IsReadyForParse: true` with `"not a governed relay block"` — confirms flag does **not** mean governed-valid. |

#### Q5 — ACP completion vs governed result validity

| Signal | Meaning |
|--------|---------|
| **`stopReason: end_turn` / prompt RPC complete** | **ACP transport turn complete** — `RunPromptAsync` may return. |
| **`EngineeringResultImport` parse + `RelayValidationState.Valid`** | **Separate** governed relay acceptance — required for `ImportCompleted`. |
| **Retest #2** | ACP path **succeeded**; governance import **failed** — expected distinction per ADR-0022 §3. |

#### Q6 — P0 / manual compatibility

A single **provider-neutral** “Engineering Result response contract” (relay v1 thin result template + rules) can serve **manual** (operator pastes contract into EA chat or references it) and **automated** (orchestrator appends to rendered handover) without changing `TryParseEngineeringResult`. P0 paste/import path remains the validation gate.

#### Q7 — Trust boundary (confirmed)

Provider output remains **untrusted** until `TryParseEngineeringResult` / `GovernedRelayV1Importer` succeed. **Must not** wrap prose into trusted packages, infer missing governance metadata, bypass importer for authenticated providers, or treat ACP completion as validation. **No architectural conflict** with desired invariant.

#### Smallest architecturally correct remediation (if authorized — not implemented here)

1. Define **canonical outbound Engineering Result response contract** (neutral relay-bridge or orchestration concern — aligned with **Option C**).  
2. Compose with validated **`RenderedHandover`** before plugin forward (or extend bridge with a dedicated render helper — still **E** relay semantics, not Cursor-specific).  
3. Keep **`TryParseEngineeringResult`** unchanged; tighten provider **`IsReadyForParse`** semantics/documentation in a later tranche (optional).  
4. **Do not** implement host synthesis (**D**).

#### Watch items (no remediation this tranche)

- EA **mode** ComboBox not persisted across restart (`RelayWorkflowViewModel` default `Plan`; only `relay_continuity` session intents persist).  
- Retest #1 **`Ambiguous`** recoverable Attention coexists with Retest #2 outcomes.  
- Sync Desktop `Forward()` UI blocking.

### MVT-8 — Automated forward reaches provider

| Field        | Value      |
| ------------ | ---------- |
| **Result**   | **Blocked** — external provider capability (Composer 2.5 non-fast ACP unavailable; Group A) |
| **Executor** | Ed Becnel  |
| **Date**     | 2026-10-02 |
| **Evidence** |            |
^mvt-8-record

### MVT-9 — Transport status projection

| Field        | Value                                                         |
| ------------ | ------------------------------------------------------------- |
| **Result**   | **Blocked** — external provider capability (depends on MVT-8) |
| **Executor** | Ed Becnel                                                     |
| **Date**     | 2026-10-02                                                    |
| **Evidence** |                                                               |
^mvt-9-record

### MVT-10 — Governed import boundary

| Field        | Value      |
| ------------ | ---------- |
| **Result**   | **Blocked** — external provider capability (depends on MVT-8) |
| **Executor** | Ed Becnel  |
| **Date**     | 2026-10-02 |
| **Evidence** |            |
^mvt-10-record

### MVT-11 — No provider governance authority

| Field        | Value      |
| ------------ | ---------- |
| **Result**   | **Blocked** — external provider capability (depends on MVT-8 forward) |
| **Executor** | Ed Becnel  |
| **Date**     | 2026-10-02 |
| **Evidence** |            |
^mvt-11-record

### MVT-12 — Interrupted forward / recoverable state

| Field        | Value      |
| ------------ | ---------- |
| **Result**   | **Blocked** — external provider capability (requires forward in progress) |
| **Executor** | — (not executed this tranche) |
| **Date**     | 2026-10-02 |
| **Evidence** | T7 blocker-independent tranche §Group B; automated T4 recovery tests PASS (supporting only) |
^mvt-12-record

### MVT-13 — No auto-import on restart

| Field        | Value      |
| ------------ | ---------- |
| **Result**   | **Blocked** — external provider capability (depends on MVT-12) |
| **Executor** | Ed Becnel  |
| **Date**     | 2026-10-02 |
| **Evidence** |            |
^mvt-13-record

### MVT-14 — Recoverable Attention

| Field        | Value      |
| ------------ | ---------- |
| **Result**   | **Blocked** — external provider capability (depends on MVT-12) |
| **Executor** | Ed Becnel  |
| **Date**     | 2026-10-02 |
| **Evidence** |            |
^mvt-14-record

### MVT-15 — Explicit confirmation before acceptance

| Field        | Value      |
| ------------ | ---------- |
| **Result**   | **Not executed** (human); MVT-12 chain **blocked** on provider |
| **Executor** | Ed Becnel  |
| **Date**     | 2026-10-02 |
| **Evidence** |            |
^mvt-15-record

### MVT-16 — Prepare manual handover

| Field        | Value                                                                                                                                                                                                                                                                                              |
| ------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | PASS                                                                                                                                                                                                                                                                                               |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                          |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                         |
| **Evidence** | Human observation: With a valid PA handover imported, selected Prepare Engineering Agent handover. ProjectConcord rendered the governed Engineering Agent handover successfully and displayed the Copy Engineering Agent handover control. Manual P0 handover preparation is available and usable. |
^mvt-16-record

### MVT-17 — Copy manual handover

| Field        | Value                                                                                                                                                                                                                                                                    |
| ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Result**   | Pass                                                                                                                                                                                                                                                                     |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                               |
| **Evidence** | Human observation: Selected Copy Engineering Agent handover. The complete rendered Engineering Agent handover was copied to the system clipboard and successfully pasted into Apple Notes for verification. Manual copy succeeded without requiring automated transport. |
^mvt-17-record

### MVT-18 — Manual engineering result import

| Field        | Value                                                                                                                                                                                                                                                                                                                                                                                                                           |
| ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                                                                                                                                                                                                                                                                                                                                            |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                                                                                                                                                       |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                                                                                                                                                      |
| **Evidence** | Human observation: Updated the thin Engineering Result fixture projectId to the current disposable Project ID `a5e26be6-9769-46f6-bd94-1669764fe8af`, pasted the complete fixture into Engineering result import, and selected Import engineering result. ProjectConcord reported `Engineering result import: Valid`. The manual P0 governed import path operated successfully and independently of automated Cursor transport. |
^mvt-18-record

### MVT-19 — P0 when automation unavailable

| Field        | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| ------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| **Evidence** | Human observation: With the project-scoped Cursor provider enablement cleared, ProjectConcord reported `operator.attention.engineering-agent.plugin-unavailable` because the selected provider plugin was disabled. The valid PA handover remained valid, the Engineering Agent handover remained valid, and the manual P0 Prepare and Copy controls remained available and usable. ProjectConcord displayed governed manual relay guidance to Prepare the Engineering Agent handover, Copy it, and Import the result when ready. Automated transport unavailability did not invalidate or prevent the manual governed relay workflow. |
^mvt-19-record

### MVT-20 — No DEBUG mode in UI

| Field        | Value                                                                                                                                             |
| ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                                                              |
| **Executor** |                                                                                                                                                   |
| **Date**     | 2026-10-02                                                                                                                                        |
| **Evidence** | Human observation: Opened the Engineering Agent mode selector. The only available modes were Plan and Agent. No DEBUG mode was present in the UI. |
^mvt-20-record

### MVT-21 — Attention derived label

| Field        | Value                                                                                                                                                                                                                                                                                                                                     |
| ------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                                                                                                                                                                                                                                                      |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                                                                 |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                                                                |
| **Evidence** | Human observation: With the Cursor provider disabled for the project, ProjectConcord displayed transport Attention under the section explicitly labeled `Attention (derived)`. The displayed plugin-unavailable and recoverable-transport conditions were presented as derived operator attention rather than canonical governance state. |
^mvt-21-record

### MVT-22 — Recommended manual fallback

| Field        | Value                                                                                                                                                                                                                                                                                                                                                              |
| ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Result**   | Pass                                                                                                                                                                                                                                                                                                                                                               |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                                                                                          |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                                                                                         |
| **Evidence** | Human observation: With automated transport unavailable and transport Attention present, ProjectConcord stated that manual P0 relay remained available and presented the governed manual relay path to Prepare the Engineering Agent handover, Copy it, and Import the result when ready. The UI did not present manual fallback as a Required governance command. |
^mvt-22-record

### MVT-23 — Provider permission ≠ governance

| Field        | Value      |
| ------------ | ---------- |
| **Result**   | **Blocked** — external provider capability (requires MVT-8 live forward) |
| **Executor** | Ed Becnel  |
| **Date**     | 2026-10-02 |
| **Evidence** |            |
^mvt-23-record

### MVT-24 — Authorization boundary notice

| Field        | Value                                                                                                                                                                                                                                                                                                                                                                              |
| ------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**   | Pass                                                                                                                                                                                                                                                                                                                                                                               |
| **Executor** | Ed Becnel                                                                                                                                                                                                                                                                                                                                                                          |
| **Date**     | 2026-10-02                                                                                                                                                                                                                                                                                                                                                                         |
| **Evidence** | Human observation: Read the Authorization boundary notice at the top of the Governed relay (P0 manual) panel. It states: `Governed relay packages present state only. Handover and PA acceptance do not grant implementation authorization.` The notice clearly preserves the authorization boundary and does not treat handover or PA acceptance as implementation authorization. |
^mvt-24-record

## T7 blocker-independent verification tranche (2026-10-02)

**PA disposition:** Closeout-preparation package **accepted**; execute maximum truthful MVR Groups **B–D** work that does **not** require successful Group A / Retest #3 / non-fast live ACP. **No** governed Forward; **no** new ACP experimentation; **no** `src/` changes.

**Release validation (local corrective tree):** `dotnet build -c Release` — **0 errors**; `dotnet test -c Release` — **298** passed / **0** failed (Application **231**, Desktop **11**, ProjectServices **56**). Relay/EA/Operator filter — **249** passed / **0** failed (Application **212**, Desktop **7**, ProjectServices **30**). NU1903 / pre-existing CS8625 / CA2264 warnings only — not T7 failure.

### Group A remainder (MVT-8–11)

| MVT | Precondition check | Tranche disposition |
| --- | --- | --- |
| MVT-8 | Requires successful automated forward to provider | **BLOCKED — EXTERNAL PROVIDER CAPABILITY** |
| MVT-9 | Requires MVT-8 in-progress/complete transport | **BLOCKED — EXTERNAL PROVIDER CAPABILITY** |
| MVT-10 | Requires MVT-8 result candidate | **BLOCKED — EXTERNAL PROVIDER CAPABILITY** |
| MVT-11 | Requires post-forward governance read | **BLOCKED — EXTERNAL PROVIDER CAPABILITY** |

### Group B (MVT-12–15)

| MVT | Precondition check | Tranche disposition |
| --- | --- | --- |
| MVT-12 | Requires **Forward in progress** then force-quit (Group A forward) | **BLOCKED — EXTERNAL PROVIDER CAPABILITY** |
| MVT-13 | Requires MVT-12 restart state | **BLOCKED — EXTERNAL PROVIDER CAPABILITY** (dependency) |
| MVT-14 | Requires MVT-12 restart Attention | **BLOCKED — EXTERNAL PROVIDER CAPABILITY** (dependency) |
| MVT-15 | Human AF-1 follow-up; often follows MVT-12 | **NOT EXECUTED** (human) — **BLOCKED** for full MVT chain |

**Machine support only (does not substitute for MVT-12–15 Pass):** `EngineeringAgentA4T4RecoveryTests` — `DiscoverRecoverableOperations_DoesNotForwardAutomatically`, `ConfirmRecoveredResultImport_CompletesAfterExplicitConfirmation`, terminal-state exclusion — Release **PASS** (2026-10-02).

### Group C (MVT-16–19)

| MVT | Precondition check | Tranche disposition |
| --- | --- | --- |
| MVT-16–18 | Valid PA import + P0 Prepare/Copy/Import — **independent** of Group A Pass | **Pass** (human 2026-10-02) — execution record |
| MVT-19 | Disable provider / stop agent — **independent** of successful forward | **Pass** (human 2026-10-02) — `engineering_agent.plugin.enabled.a5e26be6-9769-46f6-bd94-1669764fe8af` cleared for test; **selected** preference unchanged |

**Machine support only:** `RelayWorkflowViewModelAutomatedTransportTests.ValidImport_WithNoProvider_ShowsAttention_And_RecommendedManual_And_ManualPrepareStillWorks` — Release **PASS**; `EngineeringAgentManualRelayBridgeTests` / MVR-0002 fixture semantics — covered in Application relay suite **PASS**.

### Group D (MVT-20–24)

| MVT | Precondition check | Tranche disposition |
| --- | --- | --- |
| MVT-20 | Human UI mode list | **Pass** (human 2026-10-02) |
| MVT-21 | Human Attention label during transport Attention | **Pass** (human 2026-10-02) |
| MVT-22 | Human manual fallback wording | **Pass** (human 2026-10-02) |
| MVT-23 | Requires live forward (MVT-8) permission observation | **BLOCKED — EXTERNAL PROVIDER CAPABILITY** |
| MVT-24 | Human read Authorization boundary notice | **Pass** (human 2026-10-02) |

**Machine support only:** DEBUG routing unsupported — `EngineeringAgentA4T6CursorProviderTests` / `CursorRoutingIntentMapper` — **PASS**; `EngineeringAgentA4T5OperatorProjectionTests` — Attention codes/derivation — **PASS**; provider product names outside `Providers/Cursor/` — scope audit **unchanged PASS** ([A4-T7 evidence](../../Handover/ProjectConcord-A4-T7-Verification-Evidence.md) §J).

### New ProjectConcord production defect

**None identified** in automated validation tranche (Release tests **PASS**).

**Separate human-setup observation (not an MVR MVT failure):** macOS **File → Go to Folder…** path entry for `/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A` surfaced project ID **`ddde280f-38ad-4bfb-abb9-86be2c939d15`** instead of canonical Root-A **`a5e26be6-9769-46f6-bd94-1669764fe8af`**. Normal folder-browse selection of the same path opened the correct ID. Canonical Root-A identity unchanged. Classified **[GAP-053](../../Development/EDF_Gap_Register.md#gap-053--macos-go-to-folder-project-locator-vs-browse-open-identity)**; [A4-T7 §O](../../Handover/ProjectConcord-A4-T7-Verification-Evidence.md#o-macos-go-to-folder-locator-defect-observation-2026-10-02); **no** `src/` remediation this tranche.

## Human verifier confirmation

| Field             | Value                                                                |
| ----------------- | -------------------------------------------------------------------- |
| **Verifier name** | Ed Becnel                                                            |
| **Date**          | 2026-10-02                                                           |
| **Attestation**   | I executed the MVT procedures above and recorded results faithfully. |

## Parent

- [Verification Records](README.md)

## Related Documents

- [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
- [MVR-0002](MVR-0002-a2-p0-manual-governed-relay-workflow.md)
- [A4-T7 Verification Evidence](../../Handover/ProjectConcord-A4-T7-Verification-Evidence.md)
