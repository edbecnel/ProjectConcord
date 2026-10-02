# MVR-0003 A4 automated transport fixtures and setup

Automation-prepared **support** for [MVR-0003](../../Records/MVR-0003-a4-engineering-agent-automated-transport.md).  
Governed relay paste fixtures are **reused** from [MVR-0002](../MVR-0002/) (same file paths and semantics).

## Disposable Project Root (binding)

| Field | Value |
|---|---|
| Path | `/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A` |
| Project ID (MVT-2 recorded) | `a5e26be6-9769-46f6-bd94-1669764fe8af` — sqlite preference keys **must** use this GUID |
| Pre-open `.projectconcord/` | **Must be absent** before MVT-2 |
| Forbidden subjects | ProjectConcord **development** repository; production EDF repositories |

Clone or refresh from the A2 DVW template if needed (markers: `PROJECT_INDEX.md`, `ARCHITECTURE_DECISIONS.md`, `docs/Program/Gate_Reviews/`, `docs/Development/Implementation_Roadmap.md`).

## Engineering Agent plugin preferences (required for Group A)

T5–T6 Desktop does **not** expose plugin selection UI. After **MVT-2** records the active **Project ID** (`xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`), quit Desktop and run (replace `PROJECT_ID`):

```bash
DB="$HOME/Library/Application Support/ProjectConcord/user-state.db"
PROJECT_ID="a5e26be6-9769-46f6-bd94-1669764fe8af"
SELECTED_KEY="engineering_agent.plugin.selected.${PROJECT_ID}"
ENABLED_KEY="engineering_agent.plugin.enabled.${PROJECT_ID}"
sqlite3 "$DB" "INSERT INTO user_preferences(key,value) VALUES('$SELECTED_KEY','cursor-acp-reference') ON CONFLICT(key) DO UPDATE SET value=excluded.value;"
sqlite3 "$DB" "INSERT INTO user_preferences(key,value) VALUES('$ENABLED_KEY','cursor-acp-reference') ON CONFLICT(key) DO UPDATE SET value=excluded.value;"
```

Re-launch Desktop before **MVT-6** (automated forward).

## Cursor CLI (Group A)

| Variable | Purpose |
|---|---|
| `PROJECTCONCORD_CURSOR_AGENT_PATH` | Optional override; default executable name is `agent` on `PATH` |
| `PATH` | Must include `~/.local/bin` when using the Cursor-installed `agent` CLI |

### Launch corrected Desktop (MVT-7 Retest #2 — binding when Forward authorized)

Same disposable **Root-A** and Project ID. **Quit** all Desktop instances (including Retest #1). Uncommitted tree must include **Corrective Remediation #2**:

```bash
cd /Users/edbecnel/Development/GitHub/ProjectConcord
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$HOME/.local/bin:$PATH"
export PROJECTCONCORD_CURSOR_AGENT_PATH="$HOME/.local/bin/agent"
dotnet build -c Release --project src/Edf.Desktop/Edf.Desktop.csproj
dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Release --no-build
```

Human procedure and success boundary: [MVR-0003 MVT-7 Retest #2](../../Records/MVR-0003-a4-engineering-agent-automated-transport.md#mvt-7-retest-2--human-procedure-binding).

### Launch corrected Desktop (MVT-7 Retest #1 — historical)

**Quit** all ProjectConcord Desktop instances first (including the process from the original MVT-7 Fail). From the **development** repository with **uncommitted** A4-T6 corrective remediation:

```bash
cd /Users/edbecnel/Development/GitHub/ProjectConcord
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$HOME/.local/bin:$PATH"
export PROJECTCONCORD_CURSOR_AGENT_PATH="$HOME/.local/bin/agent"
dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj -c Release
```

Do **not** launch a stale `ProjectConcord.app` from an older build without this environment.

Human verifier confirms `agent --version`, `agent acp --help`, and **authenticated** CLI (`agent status`; service reachability — e.g. `agent -p --trust` after re-login). **No API key** for this MVR.

### After Desktop restart (before Retest #2 forward — when authorized)

1. Re-open **Root-A** (same disposable root).
2. Confirm Project ID **`a5e26be6-9769-46f6-bd94-1669764fe8af`**.
3. Session intents: **PA Continue**, **EA New**, **EA mode Agent**; re-import PA handover if transient import absent.
4. Prepare / validate Engineering Agent handover; confirm Forward eligible.
5. `agent status` (logged in); prefs already `cursor-acp-reference` — read-only verify optional.
6. **One** Forward click; wait up to **5 minutes** for return (UI may block — expected).

**Retest #1 historical row** `cac3c9c8-6817-4a4c-9c01-6c9d42cc1937` — do not mutate. Retest #2 expects a **new** `transport_operation` if persistence occurs.

## Paste fixtures (reuse MVR-0002)

| Fixture | MVT use |
|---|---|
| [pa-handover-valid.relay.txt](../MVR-0002/pa-handover-valid.relay.txt) | Valid PA import before forward / P0 |
| [ui-session-settings-for-valid-fixtures.md](../MVR-0002/ui-session-settings-for-valid-fixtures.md) | Session intent values |
| [engineering-agent-handover-from-valid.relay.txt](../MVR-0002/engineering-agent-handover-from-valid.relay.txt) | Reference only (automated forward renders handover) |
| [engineering-result-import-thin.relay.txt](../MVR-0002/engineering-result-import-thin.relay.txt) | P0 manual import (Group C) |

## Automated transport envelope regression (A4-T6 #3D)

Deterministic mid-line render-marker envelope shape (sanitized; not live capture) is built in test helper `EngineeringResultMidLineEnvelopeFixtures` — conversational preamble, prose immediately adjacent to `ProjectConcord-Relay-Render: 1`, complete relay artifact, trailing commentary. Proves C2 extraction + unchanged importer acceptance.
