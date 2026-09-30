# MVR-0002: A2 P0 manual governed relay workflow

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Verification](../../Verification/README.md) › [Records](README.md) › MVR-0002

## Identity

| Field                      | Value                     |
| -------------------------- | ------------------------- |
| **Record ID**              | MVR-0002                  |
| **Manual QA**              | Required                  |
| **Human execution status** | Complete                  |
| **Owner**                  | Project owner (Ed Becnel) |
| **Verification date**      | 2026-09-30                |

**Human execution status:** Set **Complete** only when governed human manual verification was executed by an authorized human with outcomes that satisfy the verification obligation (typically **Pass** in the execution record for applicable MVTs). Do not set **Complete** because automated preparation ran or because a governing record waived the obligation while MVT Results remain non-Pass.

## Verification basis / obligation

| Requirement | Link | Notes |
|---|---|---|
| A2 P0 governed relay (Desktop + manual transport) | [ProjectConcord A2 Implementation Plan](../../Handover/ProjectConcord-A2-Implementation-Plan.md) §16, §18, §20 | T1–T7 published |
| T7 Desktop relay UI | [A2-T7 implementation notes](../../Handover/ProjectConcord-A2-T7-Implementation-Notes.md) | Explicit Copy; import paths |
| Disposable verification workspace | [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md) | Binding MVR subject root |
| Machine verification (supporting only) | [A2-T8 verification evidence](../../Handover/ProjectConcord-A2-T8-Verification-Evidence.md) | PA accepted; not a substitute for this MVR |

## Implementation scope

| Anchor | Value |
|---|---|
| Repository paths | `src/Edf.Desktop/`, `src/Edf.Application/Relay/`, T3 persistence |
| Branch / commit / tag | `main` @ **`673aa8b767e4371f22da35acdc54d2e6c014bc7d`** |
| Environment or build | **macOS 26.6.2 arm64**; .NET SDK **10.0.401**; Avalonia Desktop |
| Out of scope (explicit) | A3/A4; Engineering Agent plugin implementation; automated provider transport; Migration003 |

## Operator environment and test data

Normative disposable-workspace semantics: [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md).

### Preconditions (operator — verify before MVT-1)

| Precondition | Expected state |
|---|---|
| ProjectConcord repository | Checked out at commit **`673aa8b767e4371f22da35acdc54d2e6c014bc7d`** (or equivalent published T8 baseline) |
| Disposable Project Root | **`/Users/edbecnel/tmp/ProjectConcord-A2-MVR/Root-A`** exists and is safe to discard |
| Pre-open `.projectconcord/` under disposable root | **Absent** (automation verified 2026-09-30; re-check with `ls -la` if unsure) |
| .NET SDK | **10.0.401** available (`global.json` in repository root) |
| Paste fixtures | Present under [docs/Verification/Fixtures/MVR-0002/](../Fixtures/MVR-0002/) (automation-prepared; see **Test data** below) |
| Forbidden MVR subjects | Operator **SHALL NOT** open as test Project Root: the **ProjectConcord development repository**; any **production EDF repository**; any repository relied upon for real work |

### Prepare and launch ProjectConcord (required — first operator action)

Perform from the **ProjectConcord repository root** (the repo containing `ProjectConcord.sln`), **not** from the disposable Project Root.

1. **Check SDK:** `dotnet --version` → expect **10.0.401**. If the command reports 8.x or “Requested SDK version: 10.0.401”, set:
   ```bash
   export DOTNET_ROOT="$HOME/.dotnet"
   export PATH="$HOME/.dotnet:$PATH"
   ```
2. **Launch Desktop (Debug):**
   ```bash
   dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Debug
   ```
3. **Expected:** `Build succeeded`, then the **ProjectConcord** window. NU1903 SQLite package warnings ([GAP-045](../../Development/EDF_Gap_Register.md)) are not MVT failure by themselves.

### Disposable Project Root (binding)

| Field | Value |
|---|---|
| **Disposable filesystem subjects required** | **Yes** |
| **Resolved DVW path(s)** | **`/Users/edbecnel/tmp/ProjectConcord-A2-MVR/Root-A`** |
| **Test data / fixture identification** | [docs/Verification/Fixtures/MVR-0002/](../Fixtures/MVR-0002/) — see table in **Test data** below |
| **Safety/reset prerequisites** | Disposable root only; do not use development or production repositories |
| **Cleanup expectation** | Discard DVW when finished; `user-state.db` retains per-user registry (see below) |

### Test data (paste fixtures — automation-prepared, not human attestation)

Open files from the repository at the paths below. **Copy the entire file contents** into the Desktop paste field named in each MVT.

| Fixture file (relative to repo root) | Use |
|---|---|
| [pa-handover-incomplete.relay.txt](../Fixtures/MVR-0002/pa-handover-incomplete.relay.txt) | MVT-9 — **Incomplete** PA import |
| [pa-handover-rejected-malformed-missing-machine-block.relay.txt](../Fixtures/MVR-0002/pa-handover-rejected-malformed-missing-machine-block.relay.txt) | MVT-10 — **RejectedMalformed** PA import |
| [pa-handover-valid.relay.txt](../Fixtures/MVR-0002/pa-handover-valid.relay.txt) | MVT-11, MVT-13 — **Valid** PA import (STOP clear) |
| [pa-handover-valid-stop-active.relay.txt](../Fixtures/MVR-0002/pa-handover-valid-stop-active.relay.txt) | MVT-12 — **Valid** + active **STOP** |
| [ui-session-settings-for-valid-fixtures.md](../Fixtures/MVR-0002/ui-session-settings-for-valid-fixtures.md) | **Required UI control values** before MVT-11–MVT-13 |
| [engineering-result-import-thin.relay.txt](../Fixtures/MVR-0002/engineering-result-import-thin.relay.txt) | MVT-15 — Engineering result import |

Regenerate fixtures (optional): see [Fixtures/MVR-0002/README.md](../Fixtures/MVR-0002/README.md).

### `user-state.db`

- Location: `~/Library/Application Support/ProjectConcord/user-state.db` ([02_Per_User_Application_State.md](../../Developer_Handbook/02_Per_User_Application_State.md)).
- Relay continuity and provenance for the active project are stored here (per-user), **not** in the disposable Project Root Git tree.

### Recording results

- For each MVT: perform the procedure, compare to **Expected result**, then update the **Execution record** table (**Pass** / **Fail** / **Blocked** only).
- Check the MVT checkbox **only after** you set **Result** to **Pass** (checkbox is a usability aid; **Result** governs per [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)).
- Record the **fixture filename** (or “none”) in **Test data / fixture** when applicable.
- Each checklist `[[#^mvt-n-record|MVT-n]]` link jumps to that MVT block in [[#Execution record]] (**Obsidian** block reference; `^mvt-n-record` ids are on the line after each result table).
- On **Fail**: set **Fail**, describe observation in **Evidence**, **STOP** further MVTs, return to Project Architect (T8 does **not** authorize `src/` remediation).
- On **Blocked**: set **Blocked**, note reason, **STOP**.

## Manual verification — human execution required

_Executable operator checklist. Automated fixture preparation and machine verification are support only — they are not human attestation._

- [x] [[#^mvt-1-record|MVT-1]] — Launch ProjectConcord Desktop

  **Procedure:**  
  1. Confirm **Preconditions** (above).  
  2. Execute **Prepare and launch ProjectConcord** from the repository root.  
  3. Observe the main window without opening a project yet.

  **Expected result:**  
  - Application window opens; menu **File** includes **Open Project Folder…**.  
  - No unhandled startup exception.

  **Evidence to record:** Build/launch outcome; macOS version if Blocked.

- [x] [[#^mvt-2-record|MVT-2]] — Open disposable Project Root

  **Procedure:**  
  1. **File → Open Project Folder…** (or **Open project folder…** button).  
  2. Select **`/Users/edbecnel/tmp/ProjectConcord-A2-MVR/Root-A`**.  
  3. Note the displayed **Project ID**.

  **Expected result:**  
  - Active project shows path and **Project ID**.  
  - **Governed relay (P0 manual)** panel is visible; relay controls are **enabled** (not the orange “Open a project to enable relay workflow” message).

  **Evidence to record:** Project ID; screenshot or path text optional.

- [x] [[#^mvt-3-record|MVT-3]] — Authorization boundary notice (handover ≠ authorization)

  **Procedure:**  
  1. In the relay panel, read the gray **AuthorizationBoundaryNotice** text at the top of the panel.

  **Expected result:**  
  - Text communicates that governed relay packages present state only and that **handover / PA acceptance do not grant implementation authorization** (substance equivalent to: “Handover and PA acceptance do not grant implementation authorization.”).  
  - No control in the panel grants a tranche or implementation authority.

  **Evidence to record:** Exact notice text observed (copy/paste acceptable).

- [x] [[#^mvt-4-record|MVT-4]] — Project Architect session ComboBox (macOS Avalonia)

  **Procedure:**  
  1. Locate **Project Architect session** ComboBox.  
  2. Select **NEW**, then **CONTINUE**.  
  3. Confirm each selection appears in the control (not blank).

  **Expected result:**  
  - Both **NEW** and **CONTINUE** are selectable on macOS Avalonia.  
  - No value is inferred from package prose in this step.

  **Evidence to record:** Values selected; any ComboBox display anomaly.

- [x] [[#^mvt-5-record|MVT-5]] — Engineering Agent session ComboBox (macOS Avalonia)

  **Procedure:**  
  1. Locate **Engineering Agent session** ComboBox.  
  2. Select **NEW**, then **CONTINUE**.

  **Expected result:**  
  - Both values selectable; control behaves correctly on macOS Avalonia.

  **Evidence to record:** As MVT-4.

- [x] [[#^mvt-6-record|MVT-6]] — Engineering Agent mode ComboBoxes (PLAN / AGENT)

  **Procedure:**  
  1. Set **Engineering Agent mode** to **PLAN**, then **AGENT**.  
  2. Set **Prior Engineering Agent mode** to **PLAN** (optional control).  
  3. Change **Engineering Agent mode** again and confirm prior mode does not auto-change from pasted text (no import in this MVT).

  **Expected result:**  
  - **PLAN** and **AGENT** explicitly selectable.  
  - Prior mode remains operator-selected only.

  **Evidence to record:** Mode combinations exercised.

- [x] [[#^mvt-7-record|MVT-7]] — Generate PA review package (no auto-copy)

  **Procedure:**  
  1. Optionally copy a unique sentinel string to the clipboard (e.g. `MVR-0002-CLIPBOARD-SENTINEL`).  
  2. Click **Generate PA review package**.  
  3. **Do not** click **Copy PA review**.  
  4. Inspect the read-only **PA review** text area and validation summary.

  **Expected result:**  
  - Rendered text appears in the PA review area; validation summary visible.  
  - Content includes fenced **`projectconcord-relay-v1`** and **`ChatGPT-Chat`** in governance projection.  
  - Clipboard **unchanged** from sentinel (generate alone does not copy).

  **Evidence to record:** Substring `projectconcord-relay-v1`; clipboard observation.

- [x] [[#^mvt-8-record|MVT-8]] — Explicit Copy PA review

  **Procedure:**  
  1. Click **Copy PA review** once.  
  2. Paste clipboard into a text editor and confirm it matches the PA review area.

  **Expected result:**  
  - Clipboard updates **only** after explicit **Copy PA review**.  
  - No automatic paste/send to any external application.

  **Evidence to record:** Copy occurred on button click only.

- [x] [[#^mvt-9-record|MVT-9]] — Import **Incomplete** PA handover

  **Test data / fixture:** `docs/Verification/Fixtures/MVR-0002/pa-handover-incomplete.relay.txt`

  **Procedure:**  
  1. Clear **PA handover import** text box.  
  2. Paste **entire** contents of `pa-handover-incomplete.relay.txt`.  
  3. Click **Import PA handover**.  
  4. Click **Prepare Engineering Agent handover**.

  **Expected result:**  
  - Import validation state **Incomplete** (summary/diagnostics visible).  
  - Engineering Agent handover **not** ready; **Prepare** does not produce transferable handover text (or status indicates not actionable).

  **Evidence to record:** Validation state text; diagnostic codes if shown.

- [x] [[#^mvt-10-record|MVT-10]] — Import **RejectedMalformed** PA handover

  **Test data / fixture:** `docs/Verification/Fixtures/MVR-0002/pa-handover-rejected-malformed-missing-machine-block.relay.txt`

  **Procedure:**  
  1. Clear **PA handover import** text box.  
  2. Paste **entire** contents of `pa-handover-rejected-malformed-missing-machine-block.relay.txt`.  
  3. Click **Import PA handover**.

  **Expected result:**  
  - Validation state **RejectedMalformed** with diagnostics (e.g. machine block missing).  
  - Not eligible for Engineering Agent handover.

  **Evidence to record:** Validation state; at least one diagnostic message/code.

- [x] [[#^mvt-11-record|MVT-11]] — Import **Valid** PA handover (eligible)

  **Test data / fixture:** `pa-handover-valid.relay.txt` + [ui-session-settings-for-valid-fixtures.md](../Fixtures/MVR-0002/ui-session-settings-for-valid-fixtures.md)

  **Procedure:**  
  1. Set relay UI controls exactly per **ui-session-settings-for-valid-fixtures.md** (PA **CONTINUE**, EA **NEW**, mode **AGENT**, prior **PLAN**).  
  2. Clear **PA handover import**; paste **entire** `pa-handover-valid.relay.txt`.  
  3. Click **Import PA handover**.

  **Expected result:**  
  - Validation state **Valid**.  
  - Engineering Agent handover status indicates preparation may proceed when actionable (STOP clear).

  **Evidence to record:** Validation state **Valid**; fixture filename.

- [x] [[#^mvt-12-record|MVT-12]] — **Valid** + active **STOP** blocks handover without reclassifying Valid

  **Test data / fixture:** `pa-handover-valid-stop-active.relay.txt` + **ui-session-settings-for-valid-fixtures.md**

  **Procedure:**  
  1. Set UI per **ui-session-settings-for-valid-fixtures.md**.  
  2. Clear **PA handover import**; paste **entire** `pa-handover-valid-stop-active.relay.txt`.  
  3. Click **Import PA handover**.  
  4. Click **Prepare Engineering Agent handover**.

  **Expected result:**  
  - Import remains **Valid** (not downgraded to Incomplete or RejectedMalformed).  
  - Handover **not** ready; STOP blocks preparation (status/diagnostic references active STOP).  
  - **Valid ≠ actionable** while STOP is active.

  **Evidence to record:** Validation **Valid**; handover blocked message/diagnostic.

- [x] [[#^mvt-13-record|MVT-13]] — Prepare Engineering Agent handover (Valid, STOP clear)

  **Test data / fixture:** Re-import `pa-handover-valid.relay.txt` (same UI settings as MVT-11)

  **Procedure:**  
  1. Set UI per **ui-session-settings-for-valid-fixtures.md**.  
  2. Clear **PA handover import**; paste **entire** `pa-handover-valid.relay.txt`; **Import PA handover**.  
  3. Click **Prepare Engineering Agent handover**.  
  4. Inspect read-only handover text area.

  **Expected result:**  
  - Handover text includes **`projectconcord-relay-v1`**, **`Engineering-Agent-Mode`**, **`Engineering-Agent-Chat`**, **`Engineering-Agent-Mode-Transition`**.  
  - Authorization boundary notice still visible.

  **Evidence to record:** Substrings present in handover area.

- [x] [[#^mvt-14-record|MVT-14]] — Explicit Copy Engineering Agent handover (no auto paste/send)

  **Procedure:**  
  1. With handover from MVT-13 visible, click **Copy Engineering Agent handover** once.  
  2. Confirm no external application opens automatically.  
  3. Paste clipboard into a text editor to confirm content copied.

  **Expected result:**  
  - Clipboard updates **only** on explicit Copy.  
  - **Prepare** alone does not copy or paste/send.

  **Evidence to record:** Copy on button only; no provider auto-launch.

- [x] [[#^mvt-15-record|MVT-15]] — Engineering result import (thin P0 path)

  **Test data / fixture:** `engineering-result-import-thin.relay.txt`

  **Procedure:**  
  1. Paste **entire** `engineering-result-import-thin.relay.txt` into **Engineering result import**.  
  2. Click **Import engineering result**.  
  3. Re-read authorization boundary notice.

  **Expected result:**  
  - Import completes with **Valid** or explicit governed diagnostic (not silent success as implementation authorization).  
  - UI still does not grant implementation/tranche authority.

  **Evidence to record:** Result validation summary; notice unchanged in meaning.

- [x] [[#^mvt-16-record|MVT-16]] — Provenance visibility (no full chat transcript)

  **Procedure:**  
  1. After MVT-7–MVT-15, inspect **Provenance** list in the relay panel.  
  2. Open `~/Library/Application Support/ProjectConcord/user-state.db` is **not** required; judge from UI list only.

  **Expected result:**  
  - Recent provenance events for the active **Project ID** (event type, package/correlation identifiers, UTC).  
  - Entries correspond to export/import/handover actions performed.  
  - **No** complete chat transcript presented as canonical stored content in the list.

  **Evidence to record:** Example event line(s) or count; note absence of transcript dump.

- [x] [[#^mvt-17-record|MVT-17]] — Manual P0 / no automation (session observation)

  **Procedure:**  
  1. Across MVT-1–MVT-16, confirm: no automatic provider IDE launch, no automatic paste/send after Generate/Prepare/Import, no MCP/ACP/provider API activity attributable to ProjectConcord.

  **Expected result:**  
  - Manual copy/paste only for cross-app transfer.  
  - Clipboard used only via explicit **Copy** commands for rendered governed text.

  **Evidence to record:** Factual observation statement.

- [x] [[#^mvt-18-record|MVT-18]] — Disposable Project Root mutation and `.projectconcord/`

  **Procedure:**  
  1. Quit ProjectConcord (or leave running — inspect from Terminal).  
  2. In Terminal:
     ```bash
     ls -la /Users/edbecnel/tmp/ProjectConcord-A2-MVR/Root-A
     ls -la /Users/edbecnel/tmp/ProjectConcord-A2-MVR/Root-A/.projectconcord 2>&1
     ```
  3. Compare to pre-test expectation (no `.projectconcord/` before open; see Preconditions).

  **Expected result:**  
  - **No** `.projectconcord/` directory created under the disposable root by open/select/workflow alone (PC-PAR-006).  
  - No unexpected new repository artifacts beyond harmless MVR use (operator may note `user-state.db` changes separately).

  **Evidence to record:** Terminal output or “`.projectconcord` absent” observation.

- [x] [[#^mvt-19-record|MVT-19]] — Close project and reopen (session continuity)

  **Procedure:**  
  1. Note current **Project Architect session** and **Engineering Agent session** ComboBox values.  
  2. Click **Close Project**.  
  3. Reopen **`/Users/edbecnel/tmp/ProjectConcord-A2-MVR/Root-A`** via **Open project folder…** or Recent **Open**.  
  4. Inspect relay panel and session ComboBoxes.

  **Expected result:**  
  - Relay panel enabled again for same Project ID (same registration).  
  - Session intents restored per T3 continuity where applicable (values match pre-close or documented persisted state).  
  - macOS Avalonia ComboBoxes remain usable after reopen.

  **Evidence to record:** Project ID match; session values after reopen.

## Execution record

_Obsidian: checklist links use block references `[[#^mvt-n-record|MVT-n]]`. Each subsection below ends with `^mvt-n-record` on the result table._

### MVT-1 — Launch ProjectConcord Desktop

| Field | Value |
| --- | --- |
| **Result** | Pass |
| **Executor** | Ed Becnel |
| **Date** | 2026-09-30 |
| **Test data / fixture** | none |
| **Evidence** | Followed instructions. App opens File > Open Project Folder... is present. No errors. |
^mvt-1-record

### MVT-2 — Open disposable Project Root

| Field | Value |
| --- | --- |
| **Result** | Pass |
| **Executor** | Ed Becnel |
| **Date** | 2026-09-30 |
| **Test data / fixture** | DVW Root-A |
| **Evidence** | Project ID is: def065b0-81ff-49cb-95fb-b35843268b1c |

^mvt-2-record

### MVT-3 — Authorization boundary notice (handover ≠ authorization)

| Field                   | Value                                                                                                                                              |
| ----------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                                                               |
| **Executor**            | Ed Becnel                                                                                                                                          |
| **Date**                | 2026-09-30                                                                                                                                         |
| **Test data / fixture** | none                                                                                                                                               |
| **Evidence**            | The text is displayed correctly: Governed relay packages present state only. Handover and PA acceptance do not grant implementation authorization. |

^mvt-3-record

### MVT-4 — Project Architect session ComboBox (macOS Avalonia)

| Field                   | Value                                        |
| ----------------------- | -------------------------------------------- |
| **Result**              | Pass                                         |
| **Executor**            | Ed Becnel                                    |
| **Date**                | 2026-09-30                                   |
| **Test data / fixture** | none                                         |
| **Evidence**            | Both **NEW** and **CONTINUE** are selectable |

^mvt-4-record

### MVT-5 — Engineering Agent session ComboBox (macOS Avalonia)

| Field                   | Value                      |
| ----------------------- | -------------------------- |
| **Result**              | Pass                       |
| **Executor**            | Ed Becnel                  |
| **Date**                | 2026-09-30                 |
| **Test data / fixture** | none                       |
| **Evidence**            | Both values are selectable |

^mvt-5-record

### MVT-6 — Engineering Agent mode ComboBoxes (PLAN / AGENT)

| Field                   | Value                                                                                                |
| ----------------------- | ---------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                 |
| **Executor**            | Ed Becnel                                                                                            |
| **Date**                | 2026-09-30                                                                                           |
| **Test data / fixture** | none                                                                                                 |
| **Evidence**            | **PLAN** and **AGENT** remain explicitly selectable.  <br>Prior mode remains operator-selected only. |

^mvt-6-record

### MVT-7 — Generate PA review package (no auto-copy)

| Field                   | Value                                                                     |
| ----------------------- | ------------------------------------------------------------------------- |
| **Result**              | Pass                                                                      |
| **Executor**            | Ed Becnel                                                                 |
| **Date**                | 2026-09-30                                                                |
| **Test data / fixture** | none                                                                      |
| **Evidence**            | String copied to clipboard (- MVR-0002-CLIPBOARD-SENTINEL) remains intact |

^mvt-7-record

### MVT-8 — Explicit Copy PA review

| Field                   | Value                               |
| ----------------------- | ----------------------------------- |
| **Result**              | Pass                                |
| **Executor**            | Ed Becnel                           |
| **Date**                | 2026-09-30                          |
| **Test data / fixture** | none                                |
| **Evidence**            | Copy occurred on button click only. |

^mvt-8-record

### MVT-9 — Import Incomplete PA handover

| Field                   | Value                                                                                                                                                                                                                                                                |
| ----------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                                                                                                                                                                                 |
| **Executor**            | Ed Becnel                                                                                                                                                                                                                                                            |
| **Date**                | 2026-09-30                                                                                                                                                                                                                                                           |
| **Test data / fixture** | pa-handover-incomplete.relay.txt                                                                                                                                                                                                                                     |
| **Evidence**            | PA handover is Incomplete — Engineering Agent handover cannot be marked ready. Note: I had to paste in the correct Project ID into the sample supplied in pa-handover-incomplete.relay.txt file because the one generated has no knowledge of the correct Project ID |

^mvt-9-record

### MVT-10 — Import RejectedMalformed PA handover

| Field                   | Value                                                                                                                                                                                         |
| ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                                                                                                          |
| **Executor**            | Ed Becnel                                                                                                                                                                                     |
| **Date**                | 2026-09-30                                                                                                                                                                                    |
| **Test data / fixture** | pa-handover-rejected-malformed-missing-machine-block.relay.txt                                                                                                                                |
| **Evidence**            | PA handover import: RejectedMalformed. <br>Validation diagnostics: Malformed: relay.structural.machine_block.missing — Required machine block fenced with projectconcord-relay-v1 is missing. |

^mvt-10-record

### MVT-11 — Import Valid PA handover (eligible)

| Field                   | Value                                                                                                      |
| ----------------------- | ---------------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                       |
| **Executor**            | Ed Becnel                                                                                                  |
| **Date**                | 2026-09-30                                                                                                 |
| **Test data / fixture** | pa-handover-valid.relay.txt + ui-session-settings                                                          |
| **Evidence**            | PA handover import: Valid<br>PA handover is Valid and eligible for Engineering Agent handover preparation. |

^mvt-11-record

### MVT-12 — Valid + active STOP blocks handover without reclassifying Valid

| Field                   | Value                                                                                                                |
| ----------------------- | -------------------------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                                 |
| **Executor**            | Ed Becnel                                                                                                            |
| **Date**                | 2026-09-30                                                                                                           |
| **Test data / fixture** | pa-handover-valid-stop-active.relay.txt                                                                              |
| **Evidence**            | PA handover import: Valid<br>PA handover remains Valid, but active STOP blocks Engineering Agent handover readiness. |

^mvt-12-record

### MVT-13 — Prepare Engineering Agent handover (Valid, STOP clear)

| Field                   | Value                                                                                                                                                                                                                                                                        |
| ----------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                                                                                                                                                                                         |
| **Executor**            | Ed Becnel                                                                                                                                                                                                                                                                    |
| **Date**                | 2026-09-30                                                                                                                                                                                                                                                                   |
| **Test data / fixture** | pa-handover-valid.relay.txt                                                                                                                                                                                                                                                  |
| **Evidence**            | Authorization boundary notice remains: Governed relay packages present state only. Handover and PA acceptance do not grant implementation authorization.<br>Note: Static `AuthorizationBoundaryNotice`; unchanged by design; MVT-13 boundary check N/A for dynamic behavior. |

^mvt-13-record

### MVT-14 — Explicit Copy Engineering Agent handover (no auto paste/send)

| Field                   | Value                                                                                  |
| ----------------------- | -------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                   |
| **Executor**            | Ed Becnel                                                                              |
| **Date**                | 2026-09-30                                                                             |
| **Test data / fixture** | none                                                                                   |
| **Evidence**            | Clipboard updates **only** on explicit Copy.  Confirmed by pasting into a text editor. |

^mvt-14-record

### MVT-15 — Engineering result import (thin P0 path)

| Field                   | Value                                                                                                                                                                                                                                                                                                            |
| ----------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                                                                                                                                                                                                                             |
| **Executor**            | Ed Becnel                                                                                                                                                                                                                                                                                                        |
| **Date**                | 2026-09-30                                                                                                                                                                                                                                                                                                       |
| **Test data / fixture** | engineering-result-import-thin.relay.txt                                                                                                                                                                                                                                                                         |
| **Evidence**            | Engineering result import: Valid<br>Authorization boundary notice remains: Governed relay packages present state only. Handover and PA acceptance do not grant implementation authorization.<br>Note: Static `AuthorizationBoundaryNotice`; unchanged by design; MVT-13 boundary check N/A for dynamic behavior. |

^mvt-15-record

### MVT-16 — Provenance visibility (no full chat transcript)

| Field                   | Value                                                         |
| ----------------------- | ------------------------------------------------------------- |
| **Result**              | Pass                                                          |
| **Executor**            | Ed Becnel                                                     |
| **Date**                | 2026-09-30                                                    |
| **Test data / fixture** | none                                                          |
| **Evidence**            | Everything is intact. I'm not wasting my time counting lines! |

^mvt-16-record

### MVT-17 — Manual P0 / no automation (session observation)

| Field                   | Value                                            |
| ----------------------- | ------------------------------------------------ |
| **Result**              | Pass                                             |
| **Executor**            | Ed Becnel                                        |
| **Date**                | 2026-09-30                                       |
| **Test data / fixture** | none                                             |
| **Evidence**            | Everything is as expected. This is overkill, IMO |

^mvt-17-record

### MVT-18 — Disposable Project Root mutation and `.projectconcord/`

| Field                   | Value                                                                                                                                                                             |
| ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                                                                                                              |
| **Executor**            | Ed Becnel                                                                                                                                                                         |
| **Date**                | 2026-09-30                                                                                                                                                                        |
| **Test data / fixture** | DVW Root-A                                                                                                                                                                        |
| **Evidence**            | The folder, /Users/edbecnel/tmp/ProjectConcord-A2-MVR/Root-A, contains copies of some of ProjectConcord files and a few docs subfolders and contents. There is no .projectconcord |

^mvt-18-record

### MVT-19 — Close project and reopen (session continuity)

| Field                   | Value                                                                                          |
| ----------------------- | ---------------------------------------------------------------------------------------------- |
| **Result**              | Pass                                                                                           |
| **Executor**            | Ed Becnel                                                                                      |
| **Date**                | 2026-09-30                                                                                     |
| **Test data / fixture** | DVW Root-A                                                                                     |
| **Evidence**            | The project ID and path are restored. All the governed relays are reset to their start values. |

^mvt-19-record

**MVT Result** MUST be **Pending**, **Pass**, **Fail**, or **Blocked** only. **Waived** is not an MVT result. Keep checkbox state consistent with **Result**; on conflict, **Result** governs.

## Evidence references

_Link screenshots, short recordings, or notes outside `docs/` per project policy. Do not paste large binaries into this file._

| MVT ID | Artifact or reference |
|---|---|
| _Session_ | **Automation support (2026-09-30):** DVW `/Users/edbecnel/tmp/ProjectConcord-A2-MVR/Root-A`; fixtures under `docs/Verification/Fixtures/MVR-0002/`. Not human attestation. |
| _Others_ | _(operator fills on execution)_ |

## Governing work / acceptance relationship

| Governing item | Link | Role |
|---|---|---|
| A2-T8 verification / A2 closeout | [A2 Implementation Plan](../../Handover/ProjectConcord-A2-Implementation-Plan.md) §35–§36 | MVR **Pass** — PA accepted A2 closeout 2026-09-30 |
| SPEC-005 consumption | [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md) | Canonical MVR/MVT semantics |

## Notes (optional)

_Informational only. Notes do not change MVT Result or Human execution status._

**EDF conformance reconciliation (2026-09-30):** MVR-0002 structure aligned to [Manual Verification Record Template](../../Templates/Manual_Verification_Record_Template.md), [MVR-0001](MVR-0001-a1c-desktop-project-root-recent-workflow.md), and [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md). Machine verification accepted by Project Architect; human execution **Complete**; **Project Architect accepted** human execution for A2 closeout (2026-09-30).

**Fixture regeneration:** [GenerateFixtures](../Fixtures/MVR-0002/GenerateFixtures/Program.cs) — support tool only.

---

## Parent

- [Verification Records](README.md)

## Related Documents

- [MVR-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md)
- [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md)
- [Manual Verification Record Template](../../Templates/Manual_Verification_Record_Template.md)
- [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
- [ADR-0014](../../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)
- [A2-T8 verification evidence](../../Handover/ProjectConcord-A2-T8-Verification-Evidence.md)
