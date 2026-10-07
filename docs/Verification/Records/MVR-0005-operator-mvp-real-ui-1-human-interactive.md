# MVR-0005: ProjectConcord Operator MVP / Real-UI-1 human-interactive verification

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Verification](../../Verification/README.md) › [Records](README.md) › MVR-0005

## Identity

| Field | Value |
|---|---|
| **Record ID** | MVR-0005 |
| **Title** | ProjectConcord Operator MVP / Real-UI-1 — human-interactive verification |
| **Manual QA** | **Required** |
| **Human execution status** | **Pending** |
| **Verification classification** | **Human-interactive** (retained Operator Desktop surface) + **Automated** (prerequisite Release evidence; not re-executed by operator as human proof) |
| **Owner** | Project owner |
| **Verification date** | 2026-10-06 — execution in progress |

**Target surface:** The **retained Real-UI-1** ProjectConcord Operator UI (persistent project header; **Current Work** | **Exchange** | **Projects**). This MVR is **not** directed at a disposable scaffold, test harness, or pre–Real-UI-1 long-scroll relay panel.

**Human execution status:** Set **Complete** only when an authorized human has executed applicable MVTs and recorded **Pass** (or documented **Fail** / **Blocked** with PA disposition). Do **not** set **Complete** based on automated tests alone.

## Verification basis / obligation

| Requirement | Link | Notes |
|---|---|---|
| Operator MVP-1 + Real-UI-1 authorization | Handover `852bef74-006d-4f65-b963-1c69ec694bc3` | Retained product shell |
| MVR preparation authorization | Handover `38a85e3b-e884-4fd1-91ea-6ef65f8594d2` | Preparation only; execution separate |
| Real-UI readiness investigation | Handover `2b875e92-820f-4d20-8f70-fcf998cf00b2` | Context |
| Operator projections / shell direction | [ADR-0020](../../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) | Presentation must not invent governance |
| Per-user operational state | [02_Per_User_Application_State.md](../../Developer_Handbook/02_Per_User_Application_State.md) | `user-state.db` persistence |
| Disposable verification workspaces | [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/048cdc6bc9ee581bb60fea5e7d26f7de673e5569/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md) | Safe second project / empty GEW scenario |
| WF-1d machine criteria | [MVR-0004](MVR-0004-m7a-wf-1d-governed-eligibility-recovery-projections.md) | **Not** manually re-verified in this MVR |

## Implementation scope

| Anchor | Value |
|---|---|
| **Published baseline (architecture freeze)** | `671df4e753cedd53e69905c06c49ef2a45ef2d10` |
| **Candidate under test** | Local unpublished working tree: Operator **MVP-1** + **Real-UI-1** (not yet committed at MVR preparation) |
| **Repository paths** | `src/Edf.Desktop/` (shell), `src/Edf.Application/Composition/ApplicationCompositionRoot.cs`, `src/Edf.Application/Operator/WorkState/`, `src/Edf.Application/Relay/` |
| **Environment** | macOS; .NET SDK **10.0.401** per [global.json](../../../global.json) |
| **Out of scope (explicit)** | MVP-2 relay→DWA binding; hierarchical workflow view; MVR browser; WF-1d re-test; automated Cursor transport as **required** pass (optional observation only) |

## Automated prerequisite evidence (authoritative — not human re-execution)

Recorded per Engineering Agent return package (Handover `852bef74`). **Provenance: automated test execution.** Operator **SHALL NOT** treat re-running these suites as human MVT evidence.

| Evidence item | Command (reference) | Result | Date (prep) |
|---|---|---|---|
| Focused MVP-1 / Real-UI-1 Desktop | `dotnet test tests/Edf.Desktop.Tests/Edf.Desktop.Tests.csproj --filter "FullyQualifiedName~OperatorShellViewModelTests\|FullyQualifiedName~GovernedWorkStateViewModelTests"` | **8 passed** / 0 failed | 2026-10-06 |
| Complete Desktop suite | `dotnet test tests/Edf.Desktop.Tests/Edf.Desktop.Tests.csproj` | **19 passed** / 0 failed / 0 skipped | 2026-10-06 |
| Application regression | `dotnet test tests/Edf.Application.Tests/Edf.Application.Tests.csproj` | **298 passed** / 0 failed | 2026-10-06 |
| Full Release | `dotnet test -c Release` | **379 passed** / 0 failed / 0 skipped (Application 298, ProjectServices 62, Desktop 19) | 2026-10-06 |

Pre-MVP Release baseline was **371** tests; increase attributed to new Desktop coverage for MVP-1 / Real-UI-1.

**WF-1d:** Closed machine criteria remain covered by [MVR-0004](MVR-0004-m7a-wf-1d-governed-eligibility-recovery-projections.md). This MVR does **not** require the operator to manually re-verify WF-1d projection semantics.

## Operator environment and test data

### Prepare and launch ProjectConcord (required — first operator action)

Perform from the **ProjectConcord repository root** containing the **Real-UI-1** working tree.

1. **SDK:** `dotnet --version` → expect **10.0.401**. If not on PATH:
   ```bash
   export DOTNET_ROOT="$HOME/.dotnet"
   export PATH="$HOME/.dotnet:$PATH"
   ```
2. **Build and run (Debug):**
   ```bash
   dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Debug
   ```
3. **Expected:** Build succeeded; window title includes **ProjectConcord — Operator** (or equivalent retained shell branding).
4. NU1903 SQLite warnings are not MVT failure unless they prevent launch.

### Disposable Project Roots (recommended)

| Role | Purpose |
|---|---|
| **Project A** | Primary MVR subject — may be ProjectConcord repo root **only if** operator accepts any workflow state created there; otherwise use a disposable folder |
| **Project B** | Second governed root for switching (MVT-9) — **disposable DVW folder strongly recommended** |
| **Empty GEW subject** | Folder with **no** active workflow instances for MVT-3 — use **Project B** or a third disposable root; **do not** delete or reset authoritative ProjectConcord workflow state merely to manufacture empty state |

Record resolved DVW paths in execution notes when used.

### `user-state.db`

Location: [02_Per_User_Application_State.md](../../Developer_Handbook/02_Per_User_Application_State.md). Workflow instances, relay continuity, and recent projects persist per user profile.

### Recording results and STOP rules

- For each MVT: perform procedure → compare to **Expected result** → set **Result** in [[#Execution record]] (**Pending** / **Pass** / **Fail** / **Blocked** only).
- Check the MVT checkbox **only after** **Result** is **Pass**.
- **Evidence provenance** for human MVTs: **Human interactive observation** (optional screenshot path in **Evidence** field).
- **STOP** the affected path and record **Fail** or **Blocked** if any condition in [[#Failure and STOP conditions]] occurs. Do not work around defects during formal execution. Return to PA before treating implementation as accepted.

### Failure and STOP conditions

Record observed failure and **STOP** further MVTs on the affected path when:

- Application crashes or becomes unusable
- Project identity in the header does not match the selected project
- Current Work appears to show another project's workflow state
- Persisted workflow state unexpectedly disappears after normal quit/relaunch
- GEW bootstrap appears to grant implementation authorization or create unintended topology/DWA
- Candidate Frontier is presented as full implementation permission
- Suggested Next Action is presented as authorization
- Relay/package validation is presented as durable workflow DWA, or import **creates** durable DWA unexpectedly
- Routine operation requires expanding all diagnostic/raw package areas with no summary path
- A governance invariant appears violated in the UI
- Completing the criterion would require changing implementation during the MVR

## Manual verification — human execution required

- [x] [[#^mvt-1-record|MVT-1]] — Retained application shell

  **Purpose:** Confirm the real Operator product shell is in use.

  **Prerequisites:** ProjectConcord launched per **Prepare and launch** above.

  **Procedure:**
  1. Observe the main window title and overall layout.
  2. Confirm three primary areas exist as tabs (or equivalent): **Current Work**, **Exchange**, **Projects**.
  3. Confirm **Current Work** is the default/primary tab when a project is open (open a project from **Projects** if needed).
  4. With a project open, confirm the persistent header shows project identity (name/folder, Project ID, root path).

  **Expected result:** Operator recognizes the app as **ProjectConcord Operator** with the three retained areas; **Current Work** is the normal operating area; header identifies the active project.

  **Observed result:** Retained ProjectConcord Operator shell observed with Current Work, Exchange, and Projects. After opening the real ProjectConcord root, Current Work was primary and the persistent header correctly identified the active project.

  **Result:** **Pass**

  **Evidence provenance:** Human interactive observation

- [x] [[#^mvt-2-record|MVT-2]] — Project open and identity

  **Purpose:** Verify project selection through the retained UI.

  **Prerequisites:** MVT-1 Pass (or note dependency).

  **Procedure:**
  1. Open **Projects** → **Open project folder…** (or open from Recent).
  2. Select a governed Project Root (disposable or accepted subject).
  3. Confirm header: display name, Project ID, root path match the selected folder.
  4. Switch to **Current Work**; confirm content is scoped to that project (empty or existing instances).
  5. Close and reopen the same project from Recent; confirm header and **Current Work** context remain coherent.

  **Expected result:** Identity and **Current Work** follow the selected project without requiring terminal/Git inspection.

  **Observed result:** Real ProjectConcord root opened through the UI. Header identity and Current Work followed the selected project. Close/reopen from Recent restored the same project identity and coherent Current Work context.

  **Result:** **Pass**

  **Evidence provenance:** Human interactive observation

- [x] [[#^mvt-3-record|MVT-3]] — Empty Current Work and GEW bootstrap

  **Purpose:** Verify operator-initiated GEW bootstrap without implied authorization.

  **Prerequisites:** A Project Root with **no** active workflow instances (use safe disposable project if needed).

  **Procedure:**
  1. On **Current Work**, observe empty state messaging.
  2. Confirm **Start GEW — Standard profile** is offered.
  3. Confirm surrounding copy does **not** state that bootstrap grants implementation authorization.
  4. Invoke **Start GEW — Standard profile**.
  5. Confirm a workflow instance appears in **Current Work**.
  6. Confirm presentation does **not** claim DWA or implementation permission was created by bootstrap alone.

  **Expected result:** Bootstrap creates visible instance; authorization boundaries remain clear.

  **Observed result:** Real ProjectConcord initially had no active workflow instances. Operator intentionally started the real GEW Standard workflow. A visible retained instance was created without DWA or implementation authorization being claimed.

  **Result:** **Pass**

  **Evidence provenance:** Human interactive observation

- [x] [[#^mvt-4-record|MVT-4]] — Current Work comprehension

  **Purpose:** Operator can understand governed work situation without relying on raw codes as primary labels.

  **Prerequisites:** At least one active workflow instance (MVT-3 or pre-existing).

  **Procedure:**
  1. On **Current Work**, review instance card(s): place, baseline/drift, STOP, dependencies/blockers, DWA line, actionability.
  2. Review project-level **Waiting On**, **Candidate frontier**, and **Suggested next action** sections if shown.
  3. Ask: *Can I tell where work stands without reading `gew.*` codes as the main message?*
  4. Optionally expand **Technical detail** to confirm traceability to underlying ids/codes.

  **Expected result:** Routine comprehension from primary lines; technical detail available but not required for basic orientation.

  **Observed result (initial, 2026-10-06):** **FAIL — operator judgment:** "It uses too much technical and internal jargon which is needed. But we need a more user-friendly message of where we are at and what is expected next." Technical/governance detail should remain, but the primary Current Work presentation needs an ordinary-language summary of current position and expected next action.

  **Re-verification (2026-10-06, after guided Planning Entry + corrections):** Real ProjectConcord project `aff1297f-f0fe-475c-b2b4-ddf27663caff`, GEW `…8f396384`. Operator completed purpose-built Planning Entry guided flow (review → one-copy PA round trip → validate → explicit **Enter Governed Planning** → **Return to Current Work**). Post-transition Current Work showed **Where you are: Governed Planning**, implementation not authorized, **Waiting on: Development work authorization required**. Operator answered **Yes** to: *Did ProjectConcord make it clear enough where the operator was, what they needed to do next at each step, and what the result meant, without requiring understanding of relay/protocol mechanics?*

  **Result:** **Pass** (final; initial Fail preserved in history below)

  **Evidence provenance:** Human interactive observation

- [ ] [[#^mvt-5-record|MVT-5]] — Governance semantics (non-misleading presentation)

  **Purpose:** UI does not equate suggestion, eligibility, or relay validation with authorization.

  **Prerequisites:** Current Work visible; Exchange available.

  **Procedure:** Through observation (and bootstrap/import as needed), confirm:
  - **A.** Candidate frontier is **not** labeled or read as full implementation permission.
  - **B.** Suggested next action is **not** labeled as authorization.
  - **C.** Permission to execute is visibly distinct from the suggestion text.
  - **D.** Indeterminate actionability is not shown as fully known permission.
  - **E.** STOP reads as prohibition when active.
  - **F.** Exchange boundary notice distinguishes package validation from durable DWA in workflow store.
  - **G.** GEW bootstrap messaging does not imply implementation authorization.

  **Expected result:** All applicable items (A–G) hold for observed state.

  **Observed result (2026-10-06, paused — handover `5adbfdf3`):** Operator understood DWA was required but could not determine the expected next action from Current Work alone: *"The problem I have is that although it says 'Development work authorization required'. I have no idea what is expected of me to do next."* Formal A–G checklist **not completed**; execution **paused** for Engineering remediation (handover `89626783`). **Remediation implemented (unpublished):** Current Work **Obtain Planning Authorization** + guided exchange recording durable Planning DWA — **human MVT-5 retest pending**.

  **Result:** Pending / Pass / Fail / Blocked

  **Evidence provenance:** Human interactive observation

- [ ] [[#^mvt-6-record|MVT-6]] — Governed Exchange structure

  **Purpose:** Retained Exchange UX supports routine PA/EA workflow without raw package scroll.

  **Prerequisites:** Project open; **Exchange** tab.

  **Procedure:**
  1. Confirm staged sections approximately: (1) Session/mode, (2) Project Architect exchange, (3) Engineering Agent exchange, (4) Diagnostics/provenance.
  2. Confirm routine actions (generate, copy, import, prepare handover) are visible without expanding all raw text areas.
  3. Expand diagnostics/provenance only to confirm they exist; collapse again.
  4. Confirm raw package bodies live under disclosure (expanders), not as the only path.

  **Expected result:** Understandable progression; progressive disclosure for diagnostics.

  **Observed result:** _[operator]_

  **Result:** Pending / Pass / Fail / Blocked

  **Evidence provenance:** Human interactive observation

- [ ] [[#^mvt-7-record|MVT-7]] — Project Architect round trip (manual)

  **Purpose:** Real governed PA exchange through the retained UI.

  **Prerequisites:** Project open; ChatGPT Project Architect chat available.

  **Procedure:**
  1. **Exchange** → set PA and EA session intents (NEW/CONTINUE as appropriate).
  2. Generate PA review/request package → **Copy for PA**.
  3. Paste into active Project Architect chat; obtain a handover response suitable for import.
  4. Paste response into import field → **Import PA handover**.
  5. Confirm validation summary and app coherence; switch to **Current Work** and confirm refresh without fabricated workflow authorization.

  **Expected result:** Round trip completes; no durable DWA implied solely from import (MVT-5 F still holds).

  **Observed result:** _[operator]_

  **Result:** Pending / Pass / Fail / Blocked

  **Evidence provenance:** Human interactive observation

  **Notes:** Do not grade ChatGPT reasoning quality.

- [ ] [[#^mvt-8-record|MVT-8]] — Engineering Agent handoff (manual)

  **Purpose:** Prepare and copy EA handover for Cursor.

  **Prerequisites:** MVT-7 produced a valid import sufficient to prepare EA handover (or use prior valid state if documented).

  **Procedure:**
  1. **Exchange** → **Prepare Engineering Agent handover**.
  2. Confirm EA mode (and session intent if shown) match expectation.
  3. **Copy handover for EA**; confirm clipboard text is usable for Cursor paste.
  4. _(Optional)_ Automated forward — observe only; **not required** for Pass.

  **Expected result:** Manual handoff path works; handover is copyable and coherent.

  **Observed result:** _[operator]_

  **Result:** Pending / Pass / Fail / Blocked

  **Evidence provenance:** Human interactive observation

- [ ] [[#^mvt-9-record|MVT-9]] — Project switching

  **Purpose:** State isolation between projects.

  **Prerequisites:** Two registered Project Roots (A and B); safe disposable B recommended.

  **Procedure:**
  1. Open project **A** → note header + **Current Work**.
  2. Open project **B** from **Projects** → note header + **Current Work** change.
  3. Reopen project **A** → confirm header and **Current Work** match **A** again.

  **Expected result:** No cross-project misleading display of workflow state.

  **Observed result:** _[operator]_

  **Result:** Pending / Pass / Fail / Blocked

  **Evidence provenance:** Human interactive observation

- [ ] [[#^mvt-10-record|MVT-10]] — Persistence and restart

  **Purpose:** Durable state survives normal app restart.

  **Prerequisites:** Known project + visible Current Work (e.g., after MVT-3).

  **Procedure:**
  1. Note active project and Current Work summary.
  2. Quit ProjectConcord normally.
  3. Relaunch per **Prepare and launch**.
  4. Reopen the same project; confirm workflow instance(s) and identity persist without reconstructing from chat history.

  **Expected result:** Durable local operational state recovered through UI.

  **Observed result:** _[operator]_

  **Result:** Pending / Pass / Fail / Blocked

  **Evidence provenance:** Human interactive observation

- [ ] [[#^mvt-11-record|MVT-11]] — Overall operator acceptance

  **Purpose:** Principal Real-UI-1 human acceptance judgment.

  **Prerequisites:** MVT-1–MVT-10 executed or explicitly **Blocked** with PA note.

  **Procedure:** Answer honestly:

  From the retained UI, can I reasonably determine:
  - which project I am in;
  - whether governed work exists;
  - where work stands;
  - what is blocked or waiting;
  - what is suggested next vs authorized;
  - where PA and EA exchange occur;
  - how to continue without reconstructing state from chat history?

  **Expected result:** **Pass** only if the operator answers **yes** to the practical use of the retained shell for continued operation.

  **Observed result:** _[operator narrative]_

  **Result:** Pending / Pass / Fail / Blocked

  **Evidence provenance:** Human interactive observation

## Execution record

_Human execution began 2026-10-06. MVT-1 through MVT-4 **Pass** (MVT-4 initial Fail preserved in record). MVT-5 through MVT-11 remain **Pending**._

### MVT-1 — Retained application shell

| Field | Value |
|---|---|
| **Result** | Pass |
| **Executor** | Project owner |
| **Date** | 2026-10-06 |
| **Evidence** | Human observation: retained `ProjectConcord — Operator` shell launched; Current Work / Exchange / Projects present; Current Work default; after opening the real ProjectConcord root, persistent header showed ProjectConcord, Project ID `aff1297f-f0fe-475c-b2b4-ddf27663caff`, and root `/Users/edbecnel/Development/GitHub/ProjectConcord/`. |
| **Evidence provenance** | Human interactive observation |

^mvt-1-record

### MVT-2 — Project open and identity

| Field | Value |
|---|---|
| **Result** | Pass |
| **Executor** | Project owner |
| **Date** | 2026-10-06 |
| **Evidence** | Human observation: real ProjectConcord root opened through retained UI; identity matched selected root; project was closed and reopened from Recent; the same Project ID/root and coherent Current Work context were restored with no cross-project state displayed. |
| **Evidence provenance** | Human interactive observation |

^mvt-2-record

### MVT-3 — Empty Current Work and GEW bootstrap

| Field | Value |
|---|---|
| **Result** | Pass |
| **Executor** | Project owner |
| **Date** | 2026-10-06 |
| **Evidence** | Human observation against real ProjectConcord data. Operator intentionally chose ProjectConcord itself (not a DVW) to begin its real GEW Standard workflow. Bootstrap created a retained Standard-profile instance at `gew.v1.place.intake`; baseline `671df4e753cedd53e69905c06c49ef2a45ef2d10` matched repository HEAD; STOP inactive; no dependency block; no durable development-work authorization recorded; fully governed actionability indeterminate; candidate frontier labeled eligibility-only/not authorization; Permission to execute shown as NotApplicable (suggested step — not authorization). |
| **Evidence provenance** | Human interactive observation |

^mvt-3-record

### MVT-4 — Current Work comprehension

| Field                   | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Result**              | **Pass** (final re-verification 2026-10-06; initial Fail 2026-10-06 preserved in Notes)                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| **Executor**            | Project owner                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| **Date**                | 2026-10-06                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| **Evidence**            | **Initial Fail:** operator answered No — excessive jargon in primary Current Work message (handover `688da424`). **Final Pass:** guided Planning Entry on real project `aff1297f-f0fe-475c-b2b4-ddf27663caff` / GEW `…8f396384`; one-copy PA transport; Yes/No mismatch blocked transition then corrected; second PA response with `true`/`false` projections validated; Step 4 decision review (planning authorized, implementation not authorized, STOP none, no DWA); explicit **Enter Governed Planning**; completion and **Return to Current Work** showing Governed Planning and DWA waiting; operator **Yes** on comprehension without relay/protocol mechanics. |
| **Evidence provenance** | Human interactive observation                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |

^mvt-4-record

### MVT-5 — Governance semantics

| Field | Value |
|---|---|
| **Result** | **Pending** (human retest after remediation `89626783`) |
| **Executor** | |
| **Date** | |
| **Evidence** | **Paused observation (2026-10-06):** blocker understood; next action not — quote preserved in procedure section. **Remediation identity:** Engineering handover `89626783` (guided Planning DWA path; PA profile `PlanningDevelopmentWorkAuthorization`). **Post–Planning-DWA dead-end (2026-10-07, real GEW `…8f396384`):** PA response validated; Step 4 correctly distinguished Planning authorization from implementation authorization; **Record Planning Authorization** succeeded; Current Work showed Planning DWA on record but only generic WF-1d “continue planning-region work” guidance with *Whether you may execute is not determined…* and **no** primary operator action. PA asked: *Based only on this Current Work screen, do you know what ProjectConcord expects you to do next?* Operator answered **exactly:** *No. Not in the slightest.* PLAN `1f886d78`; remediation `04ad744b` (uncommitted). MVT-5 **Pending** — post-remediation human retest not yet performed. |
| **Evidence provenance** | Human interactive observation |

^mvt-5-record

### MVT-6 — Governed Exchange structure

| Field | Value |
|---|---|
| **Result** | Pending |
| **Executor** | |
| **Date** | |
| **Evidence** | |
| **Evidence provenance** | Human interactive observation |

^mvt-6-record

### MVT-7 — Project Architect round trip

| Field | Value |
|---|---|
| **Result** | Pending |
| **Executor** | |
| **Date** | |
| **Evidence** | |
| **Evidence provenance** | Human interactive observation |

^mvt-7-record

### MVT-8 — Engineering Agent handoff

| Field | Value |
|---|---|
| **Result** | Pending |
| **Executor** | |
| **Date** | |
| **Evidence** | |
| **Evidence provenance** | Human interactive observation |

^mvt-8-record

### MVT-9 — Project switching

| Field | Value |
|---|---|
| **Result** | Pending |
| **Executor** | |
| **Date** | |
| **Evidence** | |
| **Evidence provenance** | Human interactive observation |

^mvt-9-record

### MVT-10 — Persistence and restart

| Field | Value |
|---|---|
| **Result** | Pending |
| **Executor** | |
| **Date** | |
| **Evidence** | |
| **Evidence provenance** | Human interactive observation |

^mvt-10-record

### MVT-11 — Overall operator acceptance

| Field | Value |
|---|---|
| **Result** | Pending |
| **Executor** | |
| **Date** | |
| **Evidence** | |
| **Evidence provenance** | Human interactive observation |

^mvt-11-record

## Evidence references (human artifacts — optional)

| MVT ID | Artifact or reference |
|---|---|
| MVT-1 | |
| MVT-7 | PA chat export optional |
| MVT-10 | |

## Non-blocking post-MVR improvements (not MVT prerequisites)

Per PA disposition: per-instance Waiting On grouping, Attention strip, `OperatorNextActionClass` display, tab memory, typography polish, broader reason-code mapping, hierarchical workflow view, MVR browser, MVP-2 relay→DWA — **not** required to Pass this MVR unless human execution exposes a safety/usability defect.

## Governing work / acceptance relationship

| Governing item | Link | Role |
|---|---|---|
| Real-UI-1 PA acceptance | Handover `38a85e3b-e884-4fd1-91ea-6ef65f8594d2` | Retained surface designated |
| Operator MVP architecture freeze | `671df4e753cedd53e69905c06c49ef2a45ef2d10` | Published baseline |
| Publication of MVP-1 + Real-UI-1 | _Pending PA authorization after human MVR_ | Not asserted here |

## Notes

- MVR prepared **2026-10-06**; no human MVT had been executed at preparation time.
- Human execution began **2026-10-06** against the retained Real-UI-1 surface and real ProjectConcord project data.
- Environment preparation: a fresh shell initially resolved only Homebrew .NET 8.0.131 and could not satisfy `global.json`; setting `DOTNET_ROOT="$HOME/.dotnet"` and prepending `$HOME/.dotnet` to `PATH` resolved SDK **10.0.401**. This was environment preparation, not an MVT failure.
- MVT-1 **Pass**, MVT-2 **Pass**, MVT-3 **Pass**.
- MVT-3 intentionally created ProjectConcord's real retained GEW Standard workflow state; it is **not disposable test data** and must not be cleaned up as an MVR artifact.
- MVT-4 **initial Fail (2026-10-06):** operator-facing comprehension/usability — primary Current Work message too jargon-heavy without ordinary-language position/next-step summary.
- MVT-4 **final Pass (2026-10-06):** after Real-UI-1 presentation corrections, PA Handover Output Contract, single-copy transport, guided Planning Entry exchange, projection-parser compatibility, and guided validation messaging — operator **Yes** on guided-flow comprehension without relay/protocol mechanics. Real GEW `…8f396384` legitimately at **Governed Planning**; planning entry authorized; implementation **not** authorized; no DWA; STOP inactive.
- MVT-5 through MVT-11 remain **Pending** (MVT-4 Pass does not execute later MVTs automatically).
- PA issued Engineering Agent PLAN handover `688da424-b1dc-4f51-9766-0ab81e943486`; no architecture reopening authorized.
- Automated evidence accepted from Engineering Agent package; operator must not re-run Release suite as substitute for MVT-1–MVT-11.
- **Real PA round trip (2026-10-06, post presentation/action corrections):** ProjectConcord generated a valid PA review export; the Project Architect judged planning entry correctly (planning authorized, implementation not authorized, no DWA, STOP none). Import failed twice for protocol reasons, not operator error: (1) `relay.structural.machine_block_missing` — outer fence/copy-path loss; (2) `relay.profile.authorization_disposition.payload.missing` — governance-critical disposition without matching `softwareDevelopmentProfile.authorizationDisposition`. **Root cause:** outbound PA review export was not self-describing; PA Handover Output Contract v1 added to address this.
- **MVT-4 readability / operator-action correction (2026-10-06):** Bounded Real-UI-1 presentation work improved operator-facing Current Work messaging and added planning-entry capability (Enter Governed Planning path). At implementation time GEW `…8f396384` was left at Intake; final MVT-4 Pass explicitly transitioned it to **Governed Planning** with no DWA.
- **PA Handover Output Contract v1 (2026-10-06):** Outbound PA review export became self-describing (contract appended to same clipboard payload). Prior import failures included `relay.structural.machine_block_missing` (outer fence/copy-path loss) and `relay.profile.authorization_disposition.payload.missing` (disposition without matching profile payload).
- **PA response self-description retry (2026-10-06):** After contract v1, PA correctly self-described the intended `paHandoverImport` Planning Entry response (planning authorized, implementation not authorized, no DWA, projections aligned) using **only** the new export — protocol self-description **succeeded**.
- **Single-copy transport failure (2026-10-06):** Operator used the chat UI **inner** `projectconcord-relay-v1` code-block Copy control (not the complete relay document). Import failed: `RejectedMalformed`, diagnostic code **`relay.render.version.missing`** (clipboard lacked `ProjectConcord-Relay-Render: 1` and governance projections; JSON-only payload). **Root cause:** multi-region Markdown relay document; inner fenced block Copy copies machine JSON contents only.
- **PA Handover Output Contract correction (approved plan `pa_single-copy_transport_53ada24f.plan.md`, handover `84ec761d-b3a7-4b19-82b2-0e1f169e4c48`):** Engineering implemented shared outer plain-text copy fence, manual-paste normalizer (transport-only), and matching Engineering Result contract — automated proof of string bytes only; chat one-click Copy behavior is **not** claimed from tests.
- **Governance projection mismatch (2026-10-06, MVT-4 retry evidence):** Complete single-copy PA artifact pasted successfully; PA followed ProjectConcord-generated `pa-handover-response/v1` contract using **Yes**/**No** on Authorization-Disposition lines as contract prose instructed. Importer `ParseBool` treated **Yes** as false while machine JSON had `planningAuthorized: true` → `relay.structural.governance_projection.mismatch`; topology transition correctly blocked. Guided UI initially advised recopying complete artifact. **Bounded correction** (handover `b14b962e`): parser accepts `true`/`false`/`yes`/`no` fail-closed; new contracts instruct `true`/`false` only; guided message distinguishes mismatch from transport failures. Human re-verification **completed** (handover `65bf171e`; final Pass).
- **Operator rejection of static Exchange (2026-10-06):** Operator stated the Project Architect Exchange surface must be a **wizard-type explicit flow**, not a static control panel with buttons and edit boxes. MVT-4 human retry **paused** (handover `45e7222c`, implementation `1e8f2e34`) pending purpose-built **Planning Entry guided exchange**.
- **MVT-4 final human Pass (2026-10-06):** Sequence per handover `65bf171e`: Intake → guided Planning Entry → PA review copy → complete one-paste artifact → initial Yes/No mismatch (`relay.structural.governance_projection.mismatch`) → correction → new PA response with `true`/`false` → validate → decision review → explicit Enter Governed Planning → Governed Planning completion → Current Work post-state (DWA waiting, no implementation auth). Operator comprehension judgment: **Yes**.
- **MVT-5 paused (2026-10-06, handover `5adbfdf3`):** At Governed Planning with Planning DWA absent, operator could not determine expected next action despite "Development work authorization required" — see MVT-5 procedure **Observed result**. PLAN investigation `5adbfdf3`; implementation remediation `89626783` (unpublished at record edit time). Real GEW `…8f396384` reserved for post-remediation human retest; automated development used disposable fixtures only.
- **MVT-5 human retest (2026-10-06):** Current Work **Obtain Planning Authorization** path judged clear by operator (positive evidence preserved). Guided **Prepare Review** failed with *"The review could not be prepared."* at Step 1/4 — PLAN `45c28c98` root cause: invalid `authorizationDispositionPresent` on `PaReviewExport`; bounded correction `24e9a81f` (unpublished). MVT-5 remains **Pending** pending human retest after correction.
- **MVT-5 human retest (2026-10-07, continued):** After `24e9a81f`, **Prepare Review** succeeded; **Copy Review** succeeded (`packageId` `d3eed858-74c3-44e8-a698-b1a895a1eefc`, profile **PLANNING DEVELOPMENT WORK AUTHORIZATION**). At Step 3 **Validate PA Response**, operator pasted a normal Project Architect copy and received *"ProjectConcord could not read a complete Project Architect response…"* — operator report **"Failure!"**; Planning DWA **not** recorded. PLAN `6c48a24d` / implementation handover `67f59131`: presentation brittleness at shared manual AI-response ingestion boundary; **tolerant reader** corrective implementation (unpublished). **Automated validation (same tranche, unpublished):** `Edf.ProjectServices.Tests` 62, `Edf.Application.Tests` 373, `Edf.Desktop.Tests` 30 — **465 passed**, 0 failed. MVT-5 remains **Pending** for human retest after correction (tolerant reader not yet human-verified).
- **Published baseline `4c0ad185d576a0b75cf508c433e8720b2b224e01` (2026-10-07):** Human MVT-5 retest restarted from clean publication. Current Work retained correct actionable guidance; **Obtain Planning Authorization** led to retained Step 2 review (`correlationId` `26e708f2-c347-4aeb-ba11-666158c94fc2`); **Copy Review** succeeded. PA response **presentation fractured** in chat (machine block vs projections on separate copy surfaces); operator identified the copyable PA response as **incomplete**; broken response was **not** pasted into ProjectConcord. Operator observation: tolerant interpretation cannot reconstruct actually missing governance-significant content — recovery requires a governed **PA correction request** loop (PLAN `41a447a4`, implementation handover `34e60857`, **uncommitted**). MVT-5 remains **Pending**; no fabricated human recovery-loop success; Planning DWA still absent on real GEW `…8f396384`.
- **Automated remediation (2026-10-07, uncommitted):** Required recovery-path matrix tests exposed a guided-exchange defect: `Incomplete` parsed packages could reach `CommitConsumedPaHandoverImport` before eligibility. PA authorized narrow correction (handover `763c07b2`): guided Validate PA Response now commits only when `RelayValidationState.Valid`. Guided consumption-gate tests added; Release validation recorded after correction (see Engineering return package). MVT-5 remains **Pending**.
- **MVT-5 human retest (2026-10-07, continued):** After recovery-loop publication and successful Planning-authorization guided flow on real project `aff1297f-f0fe-475c-b2b4-ddf27663caff` / GEW `…8f396384`, durable **Planning** development work authorization recorded via ordinary UI. **Current Work post-authorization failure:** actionable continuation absent — operator judgment *No. Not in the slightest.* (see MVT-5 record evidence). Engineering PLAN `1f886d78-5a66-4749-885c-f835f01d7a67`; implementation handover `04ad744b-2796-4d08-b0bc-d3211dbca444` (Open Governed Exchange from Current Work; uncommitted). MVT-5 remains **Pending** pending human retest after remediation.

---

## Parent

- [Verification Records](README.md)

## Related Documents

- [MVR-0002](MVR-0002-a2-p0-manual-governed-relay-workflow.md) — prior relay-focused MVR (superseded for **retained shell** UX by this record for Operator MVP)
- [MVR-0004](MVR-0004-m7a-wf-1d-governed-eligibility-recovery-projections.md) — WF-1d automated only
- [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
