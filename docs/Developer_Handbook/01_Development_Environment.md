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
| .NET SDK | **10.x** (.NET 10 LTS) | Application build (`net10.0`) | [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| Avalonia | **12.x stable** (via NuGet when `Edf.Desktop` exists) | Desktop UI framework for `Edf.Desktop` | [https://avaloniaui.net/](https://avaloniaui.net/) |

### Initial implementation baseline (greenfield)

ProjectConcord currently has **no** `src/`, **no** `ProjectConcord.sln`, **no** `Edf.Desktop`, and **no** existing Avalonia application. When M1a is authorized, implementation will establish the **first** .NET desktop client using:

| Setting | Value |
|---------|--------|
| Target framework | **.NET 10 LTS** |
| TFM | **`net10.0`** |
| UI | **Avalonia 12** stable package line (resolve current 12.x at project creation) |

This is an **initial implementation baseline**, not a migration, upgrade, modernization, or replacement of a prior UI.

**M1a UI scope (when implemented):** minimum Avalonia **application shell** plus minimum **open-folder / project-root** interaction per [Implementation Roadmap](../../Development/Implementation_Roadmap.md) and [ProjectConcord M1 / EGR-G1 Implementation Plan](../../Handover/ProjectConcord-M1-EGR-G1-Implementation-Plan.md). Navigation, dashboards, EDF artifact views, ATTENTION, MVR UI, authoring, and governance UX remain **deferred** to later milestones.

Do **not** target preview .NET releases or preview Avalonia packages for the baseline.

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

```bash
# [command]
```

### Verify installation

```bash
# [health check]
```

## IDE configuration

[Editor extensions and project settings.]

## Troubleshooting

[Common environment issues and resolutions.]

## Parent

- [Developer Handbook](README.md)
