# MVR-0001: A1c Desktop Project Root and Recent Projects workflow

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Verification](../../Verification/README.md) › [Records](README.md) › MVR-0001

## Identity

| Field                      | Value                     |
| -------------------------- | ------------------------- |
| **Record ID**              | MVR-0001                  |
| **Manual QA**              | Required                  |
| **Human execution status** | Complete                  |
| **Owner**                  | Project owner (Ed Becnel) |
| **Verification date**      | 2026-09-28                |

**Human execution status:** Set **Complete** only when governed human manual verification was executed by an authorized human with outcomes that satisfy the verification obligation (typically **Pass** in the execution record for applicable MVTs). Do not set **Complete** because a governing record waived the obligation while MVT Results remain non-Pass.

## Verification basis / obligation

| Requirement | Link | Notes |
|---|---|---|
| A1c Desktop workflow (Recent, Open, Close, Relocate recovery, startup) | [ProjectConcord A1 Implementation Plan](../../Handover/ProjectConcord-A1-Implementation-Plan.md) §7–9, §18 | PA-A1-1 through PA-A1-8; Relocate **only** when locator **Missing on disk** |
| Per-user application state | [02_Per_User_Application_State.md](../../Developer_Handbook/02_Per_User_Application_State.md) | `user-state.db` — not in project repository |
| Supporting automated evidence (not a substitute for this MVR) | `Edf.Application.Tests`, `Edf.ProjectServices.Tests`, `Edf.Desktop.Tests` | Registry, SQLite, ViewModel startup/Relocate enablement |
| Disposable verification workspaces | [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/048cdc6bc9ee581bb60fea5e7d26f7de673e5569/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md) | Non-valuable filesystem subjects for open/switch/missing-locator/relocation/mutation checks |

## Implementation scope

| Anchor | Value |
|---|---|
| Repository paths | `src/Edf.Desktop/`, `src/Edf.Application/`, A1b persistence |
| Branch / commit / tag | `main`; A1b baseline `583de920121cd64e4d0c918ce1b37d403b13361d` plus local A1c Desktop changes under test |
| Environment or build | macOS recommended; .NET SDK **10.0.401**; single **build and run:** `dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Debug` (see **Prepare and launch ProjectConcord**) |
| Out of scope (explicit) | M2, PAR/A2–A4, `.projectconcord/` creation, EDF artifact navigation, multi-user sync |

## Operator environment and test data

Read this section before MVTs. Normative disposable-workspace semantics: [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/048cdc6bc9ee581bb60fea5e7d26f7de673e5569/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md). This MVR does not define a competing placement policy.

### Prepare and launch ProjectConcord (required — first operator action)

Perform from the **ProjectConcord repository root** before **MVT-1**. Repeat when you need a fresh Desktop build.

1. **Check SDK (optional diagnostic):** `dotnet --version`  
   - If output is **`10.0.401`**, use step 2a.  
   - If output is **8.x** or the command errors with *Requested SDK version: 10.0.401* while only **8.0.x** is listed, your shell is not selecting the .NET 10 install — use step **2b** (session fix; does not change `~/.zshrc`).
2. **Build and run (single execution):**
   - **2a** — when `dotnet --version` is already **10.0.401**:  
     ```bash
     dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Debug
     ```
   - **2b** — when SDK 10.0.401 is installed under **`~/.dotnet`** but not on PATH (common if `DOTNET_ROOT` points at Homebrew **dotnet@8**):  
     ```bash
     export DOTNET_ROOT="$HOME/.dotnet"
     export PATH="$HOME/.dotnet:$PATH"
     dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Debug
     ```  
     One line equivalent:  
     ```bash
     DOTNET_ROOT="$HOME/.dotnet" PATH="$HOME/.dotnet:$PATH" dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Debug
     ```
3. Expect **Build succeeded** then the ProjectConcord window (NU1903 SQLite warnings — [GAP-045](../../Development/EDF_Gap_Register.md); not MVT failure). Menu bar name **ProjectConcord**.
4. After quit/relaunch MVTs, repeat step **2** (same variant you used first).

If **`~/.dotnet`** does not contain SDK **10.0.401**, install .NET 10 per [01_Development_Environment.md](../../Developer_Handbook/01_Development_Environment.md) before this MVR.

**macOS `.app` bundle (optional):** after a successful run, `dotnet build src/Edf.Desktop/Edf.Desktop.csproj -c Debug && open src/Edf.Desktop/bin/Debug/net10.0/ProjectConcord.app` with the same **`DOTNET_ROOT` / `PATH`** as step 2b if required.

Then proceed to **MVT-1**.

| Field | Value |
|---|---|
| **Disposable filesystem subjects required** | **Yes** |
| **Resolved DVW path(s)** | **Session (automation-prepared, 2026-09-28):** `/var/folders/3n/2q1gqlt505b058rshfrgnbg00000gn/T/ProjectConcord-A1c-MVR-hBtcYv` — **first-open target:** `…/first-open-target` — **second-open target:** `…/second-open-target` — **relocation target:** `…/relocation-target` (full paths in Evidence references; not human attestation) |
| **Test data / fixture identification** | Role directories beneath the resolved session DVW for: **first open target**, **second open target**, **relocation target** (directory names are operator-chosen examples, not EDF-canonical names). Each may be an empty folder or contain a harmless marker file (for example `README.txt`). DVW subjects need not be Git repositories. |
| **Safety/reset prerequisites** | Use **disposable, non-valuable** Project Roots only. **Do not** use the ProjectConcord adopting repository, another valuable engineering repository, or production EDF work trees for missing-locator, relocation, or mutation tests. **Do not** rename, move, or delete valuable repositories to simulate missing locators. |
| **Cleanup expectation** | Dispose of DVW content after verification once debugging and evidence retention needs have ended. |

### Resolving a verification-session DVW (semantics-first)

1. Before MVT-2, resolve a dedicated **verification-session** workspace using **platform-appropriate user-writable temporary-storage semantics** (for example on macOS, a new folder under the system temporary directory or another user-writable temp location EDF does not prescribe a universal `~/tmp` or `./tmp` path).
2. Default the session **outside** valuable or adopting repository working trees.
3. Create the **role directories** listed in the table above inside that session.
4. Record the resolved session path and role paths in **Resolved DVW path(s)** when you execute this MVR.

### `user-state.db`

- Location: see [02_Per_User_Application_State.md](../../Developer_Handbook/02_Per_User_Application_State.md).
- **Effect:** Recent Projects, managed-project registry, and last-active preference persist across app restarts.
- **Optional fresh start (MVT-1 only):** Quit ProjectConcord, back up then delete `user-state.db` if you need a guaranteed empty Recent list. **Do not** delete production app data without understanding you will lose all local ProjectConcord registrations for that user profile.

### Missing-locator simulation (disposable subjects only)

- **While ProjectConcord is quit**, rename or move the **disposable first-open role directory** so the **registered path** no longer exists on disk.
- **Do not** delete the folder until after relocation is verified or you have recorded the Project ID for recovery.
- Recover via **Relocate Project…** to the **disposable relocation-target role directory** (or reset the DVW and repeat from a clean session only if you intentionally restart the procedure).

### Recording results

- For each MVT: perform the procedure, compare to **Expected result**, then update the **Execution record** table (**Pass** / **Fail** / **Blocked** only).
- Check the MVT checkbox **only after** you set **Result** to **Pass** (checkbox is representational; **Result** governs per SPEC-005).
- On **Fail** or unexpected behavior: set **Result** to **Fail**, describe what you observed in **Evidence**, **STOP** further MVTs, and return to the Project Architect before A1c publication.
- On **Blocked** (cannot build, app will not launch, environment broken): set **Blocked**, note reason, **STOP**, return to PA.

## Manual verification — human execution required

- [x] **MVT-1** — Optional clean `user-state` and first launch

  **Procedure:**  
  1. _(Optional, before launch)_ Quit ProjectConcord if running. Back up and delete `user-state.db` only if you need a guaranteed empty Recent list (see below).  
  2. Execute **Prepare and launch ProjectConcord** (above — **first** build/launch steps in this MVR).  
  3. Observe startup **without** opening any project (do not use Open, Recent **Open**, or **Go to Folder…** yet).

  **Expected result (PASS):**  
  - Active project shows **No project open** (or equivalent).  
  - Recent list is empty **or** shows prior entries only if you skipped step 2.  
  - **No** project root is opened automatically (**PA-A1-1**).  
  - Covers PA items **1–3**.

- [x] **MVT-2** — Open Project via native folder picker

  **Procedure:**  
  1. Click **Open project folder…**, **or** use menu **File → Go to Folder…** and paste the absolute path to the disposable **first-open target** role directory (see Operator environment and test data).  
  2. Confirm open succeeds and note the **Project ID** shown in the active project summary.

  **Expected result (PASS):**  
  - Open succeeds; active path matches the selected first-open target directory.  
  - **ProjectConcord Project ID** is visible (UUID form).  
  - Covers PA items **4–6**.

- [x] **MVT-3** — Recent list and last-active after first open

  **Procedure:**  
  1. Without closing the app, inspect **Recent Projects** for the first-open target.

  **Expected result (PASS):**  
  - The first-open target appears in Recent (most recent first).  
  - **Last active** indication on that entry (badge or label).  
  - Locator shows **Available** (not “Missing on disk”).  
  - **Relocate Project…** control is **disabled** or non-actionable for this row (**PA amendment**).  
  - Covers PA items **7–10**.

- [x] **MVT-4** — Quit and restart without auto-open

  **Procedure:**  
  1. Quit ProjectConcord completely.  
  2. Relaunch the app.  
  3. Do **not** click Open or Recent **Open** yet.

  **Expected result (PASS):**  
  - **No** active project open.  
  - Recent still lists the first-open target; last-active still indicated where applicable.  
  - Covers PA items **11–12**, **28** (first restart leg).

- [x] **MVT-5** — Explicit reopen from Recent preserves Project ID

  **Procedure:**  
  1. On the first-open target Recent row, click **Open**.  
  2. Compare Project ID to MVT-2.

  **Expected result (PASS):**  
  - Active project opens at the first-open target path.  
  - **Same Project ID** as MVT-2.  
  - Covers PA items **13–14**.

- [x] **MVT-6** — Close Project retains Recent

  **Procedure:**  
  1. Click **Close Project**.  
  2. Inspect Recent list.

  **Expected result (PASS):**  
  - Active session cleared (**No project open**).  
  - The first-open target **remains** in Recent.  
  - Covers PA items **15–16**.

- [x] **MVT-7** — Remove from Recent while project is open (if applicable)

  **Procedure:**  
  1. **Open** the first-open target from Recent again so a session is active.  
  2. Click **Remove from Recent** on that row **without** closing the project first.  
  3. Observe active session and Recent list.

  **Expected result (PASS):**  
  - Row removed from Recent.  
  - **Active session remains open** until you explicitly **Close Project**.  
  - Covers PA items **17–18**.

- [x] **MVT-8** — Managed registration survives Remove from Recent

  **Procedure:**  
  1. **Close Project** if still open.  
  2. Use **Open project folder…** or **File → Go to Folder…** and select the **same** first-open target (same physical folder). Prefer navigating in the picker; **do not** paste a Finder-copied path that adds a spurious **`/private`** prefix before **`/var`** — that registers a different locator string.  
  3. Note Project ID.

  **Expected result (PASS):**  
  - Open succeeds.  
  - **Same Project ID** as MVT-2 (managed registration retained).  
  - Entry reappears in Recent after open.  
  - Covers PA item **19**.

- [x] **MVT-9** — Second disposable root, session switch, and Recent ordering

  **Procedure:**  
  1. **Open project folder…** and select the disposable **second-open target** role directory under your DVW session.  
  2. Note the **Project ID** for the active session.  
  3. Inspect Recent order and the first-open target row.

  **Expected result (PASS):**  
  - Active session switches to the second-open target (path and summary update).  
  - Second-open target **Project ID** differs from MVT-2 (new registration — no silent identity equivalence from path or repository similarity).  
  - Second-open target is above the first-open target in Recent (most recently opened first).  
  - First-open target **remains** in Recent with appropriate availability.  
  - Supports PA Recent ordering policy (max 10 not fully stressed unless you choose to open 11 roots).

- [x] **MVT-10** — Missing locator presentation

  **Procedure:**  
  1. Quit ProjectConcord.  
  2. Rename or move the disposable **first-open target** directory so the registered path no longer exists (see Missing-locator simulation).  
  3. Relaunch app; find the Recent entry for that project.

  **Expected result (PASS):**  
  - Locator shows **Missing on disk** (derived; not a persisted DB status field).  
  - **Open** on that row is disabled or fails without opening a session.  
  - Covers PA item **20**.

- [x] **MVT-11** — Relocate available only for missing locator

  **Procedure:**  
  1. On the **missing** Recent row, confirm **Relocate Project…** is enabled.  
  2. On any **Available** Recent row (for example the second-open target), confirm **Relocate Project…** is **disabled**.

  **Expected result (PASS):**  
  - Relocate enabled **only** for **Missing on disk** entries.  
  - Covers PA items **21** and **10** (recheck).

- [x] **MVT-12** — Relocate via folder picker and reconciliation

  **Procedure:**  
  1. On the missing first-open target entry, click **Relocate Project…**.  
  2. Use the native folder picker to select the disposable **relocation-target** role directory (create it under your DVW session if needed).  
  3. Record Project ID after success.

  **Expected result (PASS):**  
  - Relocation succeeds; status message indicates success.  
  - **Same Project ID** as MVT-2.  
  - Locator updates to the new path; availability becomes **Available**.  
  - **Relocate Project…** is **disabled** again for that row.  
  - Covers PA items **22–26**.

- [x] **MVT-13** — Relocated locator persists across restart; no auto-open

  **Procedure:**  
  1. Quit and relaunch ProjectConcord.  
  2. Do not open any project.

  **Expected result (PASS):**  
  - Recent shows relocated path; still **Available**.  
  - **No** automatic open of last-active project.  
  - Covers PA items **27–28**.

- [x] **MVT-14** — No `.projectconcord/` in test Project Roots

  **Procedure:**  
  1. In Finder or terminal, inspect each disposable DVW role directory used in this session (relocation target, second-open target, and any other test roots).

  **Expected result (PASS):**  
  - **No** `.projectconcord/` directory exists under those roots after all prior MVTs.  
  - Covers PA item **29**.

- [x] **MVT-15** — No unexpected modification of test Project Root content

  **Procedure:**  
  1. If you placed a marker file (for example `README.txt`) in a test root before testing, confirm it still exists and was not altered by ProjectConcord operations.  
  2. Confirm ProjectConcord did not add EDF canonical artifacts (no new `docs/`, gate files, etc.) inside the test root.

  **Expected result (PASS):**  
  - Only normal filesystem changes you made (rename/move for missing-locator test) occurred; ProjectConcord did not write operational state into the repository tree.  
  - Covers PA item **30**.

## Execution record

| MVT ID | Result | Executor | Date | Evidence |
| ------ | ------ | -------- | ---- | -------- |
| MVT-1 | Pass | Ed Becnel | 2026-09-28 | Human launch: active **No project open**; Recent empty prompt; no auto-open; Close Project disabled. `user-state.db` not reset (PA session default). |
| MVT-2 | Pass | Ed Becnel | 2026-09-28 | **Open project folder…** + picker **Go to Folder…** to DVW first-open target. Project ID **71da98d5-0671-4db2-bad8-ecdcb03a0124**. Recent **Last active**, **Available**; Relocate disabled. |
| MVT-3 | Pass | Ed Becnel | 2026-09-28 | After MVT-2 without closing app: first-open target in Recent (most recent); **Last active**; **Available**; Relocate disabled. |
| MVT-4 | Pass | Ed Becnel | 2026-09-28 | Full quit + relaunch (.NET 10 session); no open after restart. **No project open.** Recent retains first-open target; **Last active**, **Available**; no auto-open. |
| MVT-5 | Pass | Ed Becnel | 2026-09-28 | Recent **Open** on first-open-target; path matched DVW; Project ID **71da98d5-0671-4db2-bad8-ecdcb03a0124** (same as MVT-2). |
| MVT-6 | Pass | Ed Becnel | 2026-09-28 | **Close Project**: active **No project open**; first-open target **remains** in Recent. |
| MVT-7 | Pass | Ed Becnel | 2026-09-28 | Reopened from Recent; **Remove from Recent** while session active; Recent row cleared; active session remained open until explicit close. |
| MVT-8 | Pass | Ed Becnel | 2026-09-28 | After close, reopened same first-open target (correct `/var/…` path; not Finder `/private/…` paste). Same Project ID **71da98d5-0671-4db2-bad8-ecdcb03a0124** as MVT-2; Recent entry restored. |
| MVT-9 | Pass | Ed Becnel | 2026-09-28 | Opened second-open target; active session switched; second-root Project ID **differs from MVT-2**; second-open above first-open in Recent; first-open remains in Recent. |
| MVT-10 | Pass | Ed Becnel | 2026-09-28 | Renamed/moved first-open target while quit; relaunched; Recent shows **Missing on disk**; **Open** disabled or non-opening. |
| MVT-11 | Pass | Ed Becnel | 2026-09-28 | **Relocate Project…** enabled on missing row; **disabled** on Available row (second-open target). |
| MVT-12 | Pass | Ed Becnel | 2026-09-28 | Relocate missing first-open entry to relocation-target; success; **same Project ID** as MVT-2 (**71da98d5-0671-4db2-bad8-ecdcb03a0124**); locator **Available**; Relocate disabled again. |
| MVT-13 | Pass | Ed Becnel | 2026-09-28 | Quit/relaunch; Recent shows relocated path **Available**; **no** auto-open of last-active project. |
| MVT-14 | Pass | Ed Becnel | 2026-09-28 | Inspected DVW role directories: **no** `.projectconcord/` under test roots. |
| MVT-15 | Pass | Ed Becnel | 2026-09-28 | Marker `README.txt` unchanged; no EDF/governance artifacts added inside test roots; only operator rename/move for missing-locator test. |

**MVT Result** MUST be **Pending**, **Pass**, **Fail**, or **Blocked** only. **Waived** is not an MVT result. Keep checkbox state consistent with **Result**; on conflict, **Result** governs.

**Clerical note (2026-09-28 PA closeout):** All MVT execution dates are **2026-09-28** (single verification session). Any prior draft date **2026-09-26** was a clerical error and is not authoritative.

## Evidence references

_Link screenshots, short screen recordings, or notes outside `docs/` per project policy._

| MVT ID | Artifact or reference |
|---|---|
| MVT-1 | Human attestation — see Execution record (2026-09-28) |
| MVT-2 | Project ID: **71da98d5-0671-4db2-bad8-ecdcb03a0124** (anchor for MVT-5, MVT-8, MVT-12) |
| MVT-9 | Second-root Project ID: **not separately recorded** — execution record MVT-9 attests ID **differs from MVT-2** (sufficient for Pass) |
| MVT-12 | Project ID after relocate: **71da98d5-0671-4db2-bad8-ecdcb03a0124** (matches MVT-2 per execution record) |
| _Session_ | **Automation support (2026-09-28, Cursor):** DVW session `/var/folders/3n/2q1gqlt505b058rshfrgnbg00000gn/T/ProjectConcord-A1c-MVR-hBtcYv`; roles: `first-open-target`, `second-open-target`, `relocation-target` (each contains `README.txt` sentinel). Not human attestation. |
| _Others_ | |

## Governing work / acceptance relationship

| Governing item | Link | Role |
|---|---|---|
| A1c implementation authorization | PA handover (A1c) | Manual UI verification required before A1c publication |
| A1 implementation plan | [ProjectConcord-A1-Implementation-Plan.md](../../Handover/ProjectConcord-A1-Implementation-Plan.md) | Declares A1c scope; this MVR is the operator checklist |

## Notes (optional)

_Informational only. Notes do not change MVT Result or Human execution status._

**Verification support (EDF MVR-0001 v1.1):** On 2026-09-28, Cursor (automation agent) created the disposable DVW session and role directories under macOS `$TMPDIR`, added harmless `README.txt` markers, and recorded absolute paths above.

**SDK / build preflight (automation, not human attestation):** Initial Cursor shell used `DOTNET_ROOT`/`PATH` from `~/.zshrc` (dotnet@8) — `dotnet build` failed with missing SDK 10.0.401; PA accepted root cause (SDK 10.0.401 present at `~/.dotnet`). Temporary verification session used `~/.dotnet` on PATH for `dotnet run`.

**A1c manual verification closeout (2026-09-28):** Project Owner attested **MVT-1–MVT-15 Pass**; Project Architect **A1c manual verification PASSED — PA ACCEPTED**. Human execution status **Complete**; verification date **2026-09-28**.

**DEFERRED — prospective DVW guidance (GMR-0002 / EDF DVW-0001 v1.1):** The completed A1c DVW session path remains valid evidence and is **not** altered retroactively. For **future** human-interactive ProjectConcord MVRs, verification guidance should follow **EDF DVW-0001 v1.1** and prefer short, recognizable, readily navigable disposable paths when practical rather than long platform-generated temporary paths.

---

## Parent

- [Verification Records](README.md)

## Related Documents

- [MVR-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md)
- [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/048cdc6bc9ee581bb60fea5e7d26f7de673e5569/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md)
- [Manual Verification Record Template](../../Templates/Manual_Verification_Record_Template.md)
- [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
- [ADR-0014](../../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)
