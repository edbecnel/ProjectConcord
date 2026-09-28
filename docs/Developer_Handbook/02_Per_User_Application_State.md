# Per-user application state (user-state.db)

> **Documentation path:** [Project Index](../../PROJECT_INDEX.md) → [Developer Handbook](README.md) → Per-user application state

## Purpose

ProjectConcord stores **per-user operational state** in a local SQLite database file named **`user-state.db`**. This file holds:

- registered ProjectConcord projects (`project_id`, display name, registered locator path, timestamps);
- Recent Projects ordering (per user, not per Git repository);
- user preferences such as **last active project** (for highlighting only — not auto-open).

It supports the A1 Project Root / Recent workflow. It is **not** canonical EDF project data.

## Location (OS application data)

The database is **never** stored inside an opened project repository.

| Platform | Convention |
|----------|--------------|
| **macOS** | `~/Library/Application Support/ProjectConcord/user-state.db` |
| **Linux** | `$XDG_DATA_HOME/ProjectConcord/user-state.db` or `~/.local/share/ProjectConcord/user-state.db` |
| **Windows** | `%LOCALAPPDATA%\ProjectConcord\user-state.db` |

Resolution is implemented in `Edf.ProjectServices` (`UserApplicationStatePathResolver`).

## What this is not

- **Not** canonical EDF Markdown, gates, ADRs, or repository content.
- **Not** written into the opened project’s Git tree.
- **Not** the future authoritative multi-user server database or synchronization store.

Locator **availability** (folder exists on disk) is **derived at runtime** and is **not** persisted in `user-state.db`.

## Development reset

To reset local ProjectConcord state during development:

1. Quit the ProjectConcord Desktop application.
2. Delete `user-state.db` in the path above (optionally back up the file first).
3. Restart the application.

**Effects:** Recent Projects, managed-project registry, and last-active preference for that user profile are cleared. Opened repositories are unchanged. No `.projectconcord/` folder is created or removed by this procedure.

## Corruption and migration failures

On startup, ProjectConcord runs forward-only schema migrations. If the database has an **unsupported newer schema version**, or the file is **corrupt**, startup fails with an explicit error rather than silently deleting or recreating the database.

If migration fails, inspect or remove `user-state.db` using the reset procedure above after backing up any data you need.

## Parent

- [Developer Handbook](README.md)

## Related Documents

- [Development Environment](./01_Development_Environment.md)
- [ProjectConcord A1 Implementation Plan](../../Handover/ProjectConcord-A1-Implementation-Plan.md)
