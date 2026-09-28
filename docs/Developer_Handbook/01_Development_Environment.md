# Development Environment

> **Documentation path:** [Project Index](../../PROJECT_INDEX.md) → [Developer Handbook](README.md) → Development Environment

## Purpose

Canonical reference for local development environment setup, configuration, and troubleshooting.

New contributors should follow [00_First_Time_Setup.md](./00_First_Time_Setup.md) first.

## Prerequisites

### Required software

| Tool | Minimum version | Purpose | Install reference |
|------|-----------------|---------|-------------------|
| Git | | Version control | |
| .NET SDK | **10.0.401** (pinned in repository [`global.json`](../../global.json); .NET 10 LTS line) | Application build (`net10.0`) | [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| Avalonia | **12.x stable** (via NuGet when `Edf.Desktop` exists) | Desktop UI framework for `Edf.Desktop` | [https://avaloniaui.net/](https://avaloniaui.net/) |

### Initial implementation baseline (M1a)

ProjectConcord ships the **first** .NET desktop client (M1a skeleton) using:

| Setting | Value |
|---------|--------|
| SDK pin (repo root) | **`10.0.401`** via [`global.json`](../../global.json) |
| Target framework | **.NET 10 LTS** |
| TFM | **`net10.0`** (all solution projects) |
| UI | **Avalonia 12.1.3** in `Edf.Desktop` (12.x stable line) |

This is an **initial implementation baseline**, not a migration, upgrade, modernization, or replacement of a prior UI.

**M1a UI scope:** minimum Avalonia **application shell** plus **open-folder / project-root** interaction per [Implementation Roadmap](../../Development/Implementation_Roadmap.md) and [ProjectConcord M1 / EGR-G1 Implementation Plan](../../Handover/ProjectConcord-M1-EGR-G1-Implementation-Plan.md). Navigation, dashboards, EDF artifact views, ATTENTION, MVR UI, authoring, and governance UX remain **deferred**.

Do **not** target preview .NET releases or preview Avalonia packages for the baseline.

### Verify SDK from repository root

After clone, run commands from the repository root so `global.json` selects the pinned SDK:

```bash
cd <project-directory>
dotnet --version
```

Expected: **`10.0.401`** (or compatible roll-forward within the pinned policy in `global.json`).

If `dotnet --version` reports an older SDK (for example 8.x), install [.NET 10 SDK](https://dotnet.microsoft.com/download) and adjust **PATH** so the `dotnet` executable from that installation is found **before** any older SDK on your machine. This is ordinary development-environment setup — not a permanent architectural requirement for a specific install location.

## Repository setup

### Clone and initialize

```bash
git clone <repository-url>
cd <project-directory>
```

### Environment variables

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| | | | |

### Secrets handling

- Use `.env` (gitignored) and provide `.env.example`.

## Local services

### Running dependencies

[Describe how to start local databases, caches, or containers.]

### Seed data

[Describe how to load development data.]

## Running the application

### Start commands

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download) on `PATH` (see Prerequisites).

```bash
dotnet build ProjectConcord.sln
dotnet test ProjectConcord.sln
dotnet run --project src/Edf.Desktop/Edf.Desktop.csproj
```

**macOS app bundle (icon + Finder/Dock):** building `Edf.Desktop` on macOS produces `src/Edf.Desktop/bin/$(Configuration)/net10.0/ProjectConcord.app`. Open that `.app` in Finder to verify the bundle icon; `dotnet run` also sets the window and Dock icon at runtime via `AppBranding`. Source artwork: [`docs/Branding/ProjectConcord-Icon-WhiteBackground.png`](../Branding/ProjectConcord-Icon-WhiteBackground.png).

A1 Desktop: Project Root open, Recent Projects, explicit reopen, **Relocate Project…** (missing locator only), Remove from Recent, and Close Project — see [A1 Implementation Plan](../../Handover/ProjectConcord-A1-Implementation-Plan.md) and [Per-user application state](./02_Per_User_Application_State.md).

### Verify installation

```bash
dotnet build ProjectConcord.sln -c Release
dotnet test ProjectConcord.sln -c Release
```

## IDE configuration

[Editor extensions and project settings.]

## Troubleshooting

### Wrong .NET SDK version at repository root

Symptom: `dotnet --version` shows an older SDK when run from the repository root, or build fails with framework/SDK mismatch.

Resolution: Install .NET 10 SDK **10.0.401** or newer compatible with [`global.json`](../../global.json). Ensure your shell **PATH** prefers that installation’s `dotnet` over an older system or package-manager SDK. Re-run `dotnet --version` from the repository root to confirm **`10.0.401`** (or policy-allowed roll-forward).

## Parent

- [Developer Handbook](README.md)
