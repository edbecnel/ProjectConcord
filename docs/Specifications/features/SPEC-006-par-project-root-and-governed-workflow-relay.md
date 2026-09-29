[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Specifications](../README.md) › SPEC-006

# SPEC-006: Project Root, Identity, PAR, and Governed Workflow Relay

## Metadata

| Field | Value |
|---|---|
| **Spec ID** | SPEC-006 |
| **Status** | Draft |
| **Owner** | ProjectConcord |
| **Normative** | Yes — PAR, Project Root lifecycle, per-user application state, and relay boundaries |
| **Last Reviewed** | 2026-09-29 (Stage 1 decomposition) |
| **Target release** | PAR track A1–A4 (separately authorized); not part of M1 |
| **Implementation** | **Not authorized** until explicit PAR implementation tranches |
| **Architecture baseline** | `c08af261ff323a0ddd54a84bd5c8b990a49fa84f` |

## Parent

- [Specifications](../README.md)

## Related Documents

- [ADR-0015 — Project identity, PAR boundaries, per-user operational state](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)
- [SPEC-004 — AI-assisted development governance workflow](SPEC-004-ai-assisted-development-governance-workflow.md) — PC-AIGOV-001–028; SPEC-006 specializes PAR without redefining them
- [SPEC-001 — MVP EDF desktop client](SPEC-001-mvp-edf-desktop-client.md) — M2+ EDF engine depth
- [ADR-0004](../../Architecture/ADRs/ADR-0004-Derived-Data-and-Cache.md), [ADR-0006](../../Architecture/ADRs/ADR-0006-AI-Boundary.md), [ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)
- [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) — **Accepted** 2026-09-29; not superseded
- [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md)
- [AWI-0006](../../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)
- [AMD-0003](../../Architecture/AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md), [ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)

---

## 1. Purpose

Define normative ProjectConcord behavior for:

- Project Root lifecycle and stable **ProjectConcord Project ID**;
- per-user application state (including Recent Project Roots);
- **Project Architect Relay (PAR)** orchestration;
- provider-neutral **Project Architect** exchange;
- human-mediated and future automated Project Architect providers;
- **Engineering Agent** relay to Cursor via a bridge abstraction;
- PA package and handover schema validation;
- work-session and conversation **provenance** (provider-neutral core; ChatGPT field names as adapter rendering);
- relay-boundary **STOP** behavior.

**Non-goal:** Replace [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) or implement full M7a governance entities before authorized tranches.

## 2. Scope

### In scope (when implementation authorized)

- Explicit Project Root selection as session context
- Stable Project ID assignment and locator reconciliation
- Per-user SQLite operational store (architecture per [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md))
- PAR package export/import and validation
- P0 manual transport (copy/export, paste/import)
- Tier 0 Canonical Markdown awareness
- Cursor handover validation and mode routing intent (P0: human delivers to Cursor)
- Evidence/result ingestion correlation
- Provisional transport attribution labels

### Out of scope (this specification does not authorize)

- M2+ EDF discovery, parsing, validation UI ([SPEC-001](SPEC-001-mvp-edf-desktop-client.md))
- OpenAI or other AI API integration
- Cursor extension, CLI, MCP, ACP implementation ([AWI-0006](../../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md))
- Creating `.projectconcord/` on open/select
- Normative Actor/Role model ([PCON-0002](../../Architecture/PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md))
- MVR execution instances (**STOP-2** binding; [SPEC-005](SPEC-005-manual-verification-record-consumption.md))

### Stage 1 ownership decomposition (relay vs PAR umbrella)

**Project Architect Relay (PAR)** remains historical/umbrella terminology in this specification. Normative requirements below map to owners per [ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md):

| Owner | Scope in SPEC-006 |
|---|---|
| **Core — Governed Interaction Relay** | Package/correlation identity; generic import/export validation; provenance chain (§10 PC-PAR-021); relay-boundary STOP enforcement hooks; INCOMPLETE metadata gate |
| **Software Development / Engineering Extension** | Software governance package/profile content; handoff vs **DevelopmentWorkAuthorization** separation; software execution context in packages |
| **Working Environment / project policy** | Supervision, checkpoint, and escalation behavior where configured for relay |
| **Provider adapter (E)** | `IProjectArchitectProvider`, manual ChatGPT formatting, `CursorBridge`, governance-critical handover fields (`Cursor-Mode`, `Cursor-Chat`, etc.) |

**A2 (documentation reframe):** Future authorized implementation tranche **A2** delivers manual P0 governed interaction relay per this decomposition. **A2 is not authorized** by this spec revision.

## 3. Relationship to SPEC-004

| Concern | SPEC-004 | SPEC-006 |
|---|---|---|
| Governance state vs chat transcripts | PC-AIGOV-002 | PAR packages are derived; enforces at relay boundary |
| Handover vs DevelopmentWorkAuthorization | PC-AIGOV-003 | PAR MUST keep separate fields/objects |
| Planning vs implementation | PC-AIGOV-004 | PAR MUST NOT infer implementation authorization |
| Manual vs integrated providers | PC-AIGOV-005, 006 | P0 manual first; same semantic objects |
| STOP | PC-AIGOV-007 | Relay-boundary STOP + binding STOP-2 respect |
| Context reconstructability | PC-AIGOV-015 | Tier 0 + operational provenance |

SPEC-006 adds **PAR-specific** requirements (PC-PAR-001–022 below). Where SPEC-004 is not yet implemented, SPEC-006 defines **minimum relay behavior** for authorized PAR tranches only.

## 4. Identifier requirements (PC-PAR-001–004)

| ID | Requirement |
|---|---|
| **PC-PAR-001** | Each managed logical project SHALL have a stable **ProjectConcord Project ID** independent of filesystem path. |
| **PC-PAR-002** | **Project Root** SHALL denote the currently selected absolute filesystem locator for that session context. |
| **PC-PAR-003** | Repository identity, Git remote URL, and repository/workspace name SHALL NOT be treated as equivalent to ProjectConcord Project ID. |
| **PC-PAR-004** | When a Project Root path changes for the same logical project, ProjectConcord SHALL support reconciliation so operational workflow state keyed by Project ID remains continuous. |

## 5. Project Root lifecycle (PC-PAR-005–008)

| ID | Requirement |
|---|---|
| **PC-PAR-005** | ProjectConcord SHALL start or resume user work in the context of an **explicitly selected** Project Root (or explicit “no project” state with no project-scoped workflow mutation). |
| **PC-PAR-006** | Opening or selecting a Project Root MUST NOT create `.projectconcord/`. |
| **PC-PAR-007** | **Recent Project Roots** SHALL be maintained in **per-user** application state, not in Git. |
| **PC-PAR-008** | Recent entries SHALL reference ProjectConcord Project ID and last-known locator; display MAY show path and friendly name. |

## 6. Per-user application state (PC-PAR-009–011)

| ID | Requirement |
|---|---|
| **PC-PAR-009** | Per-user operational persistence SHALL use a **SQLite database** in OS application data (not in the repository) when implemented, partitioned by Project ID for project-scoped records. |
| **PC-PAR-010** | Schema versioning, migration, backup, and recovery SHALL be documented and testable before A1 implementation authorization. |
| **PC-PAR-011** | Application state MUST NOT hold canonical EDF content as authoritative substitute for Git ([ADR-0002](../../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)). |

## 7. PAR core (PC-PAR-012–015)

| ID | Requirement |
|---|---|
| **PC-PAR-012** | **PAR** SHALL assemble **PA Review Packages** from governed operational state and Tier 0 canonical awareness. |
| **PC-PAR-013** | PAR SHALL validate imported **PA handover responses** against the governance-critical schema (§9). |
| **PC-PAR-014** | PAR SHALL validate **Cursor-directed handovers** before automated relay; **INCOMPLETE** handovers MUST block automated Cursor relay. |
| **PC-PAR-015** | PAR MUST NOT silently infer missing authorization or mode metadata from prose or model output. |

## 8. Provider separation (PC-PAR-016–019)

| ID | Requirement |
|---|---|
| **PC-PAR-016** | Project Architect integration SHALL use a **provider-neutral boundary** (conceptually `IProjectArchitectProvider`). |
| **PC-PAR-017** | **Manual ChatGPT product** workflow SHALL be one adapter (`ProjectArchitectManualAdapter` or equivalent), responsible for export formatting, import parsing, and human paste — not governance semantics. |
| **PC-PAR-018** | Future providers (OpenAI API, local AI, other) SHALL be substitutable without redesigning PAR, workflow state, Cursor bridge, Project Root architecture, STOP enforcement, or canonical EDF handling. |
| **PC-PAR-019** | Providers MAY differ in capability; PAR SHALL support **capability discovery/negotiation** where needed rather than assuming ChatGPT-specific features. |

**No core PC-PAR requirement SHALL depend on the ChatGPT product or OpenAI API.**

## 9. PA handover schema — governance-critical fields (PC-PAR-020)

For handovers intended to direct Cursor work, the following are **governance-critical**:

| Field | Required when |
|---|---|
| `Cursor-Mode` | Always (PLAN, AGENT, DEBUG, or documented enum) |
| `Cursor-Chat` | Always (NEW or CONTINUE — user-selected lifecycle) |
| `ChatGPT-Chat` | Always for current manual adapter rendering (maps from core `ProjectArchitectSession` intent) |
| `Cursor-Mode-Transition` | When mode changes from prior authorized state (for example `AGENT -> PLAN`) |

Authorization disposition, applicable STOP markers, and tranche/gate correlation SHALL be represented explicitly in structured or delimited blocks (exact serialization is implementation-defined; validation rules are normative).

**Advisory fields** (for example `Cursor-Chat-Advisory`, `ChatGPT-Chat-Advisory`) are optional recommendations; they MUST NOT override user-selected lifecycle actions.

If governance-critical metadata is missing, status is **INCOMPLETE** — PAR MAY surface for human correction; automated Cursor relay is forbidden.

Outbound PA packages SHALL remind the Project Architect that the next Cursor handover must include `Cursor-Mode` (and transition when applicable).

## 10. Work-session and conversation provenance (PC-PAR-021)

| ID | Requirement |
|---|---|
| **PC-PAR-021** | PAR SHALL model provenance as: **observed/current work-session context** → **advisory** → **user decision** → **requested next action** → **package produce/consume event**. |

NEW/CONTINUE alone is insufficient to determine which conversation produced or consumed a package.

Core concepts (provider-neutral):

- `ProjectArchitectSession` — user-selected continuity intent for Project Architect interaction
- `ProjectArchitectSessionAdvisory` — PAR recommendation
- `EngineeringAgentSession` / advisory — parallel concepts for Cursor (adapter may render as `Cursor-Chat` / `Cursor-Chat-Advisory`)

Provider-specific conversation IDs (for example ChatGPT thread IDs) MUST NOT be required in core provenance. Correlation SHALL use ProjectConcord-generated work-session and package identifiers, timestamps, Project ID, and optional content hashes.

## 11. Engineering agent / Cursor bridge (PC-PAR-022)

| ID | Requirement |
|---|---|
| **PC-PAR-022** | Cursor integration SHALL use a **CursorBridge** abstraction. First implementation target is **P0 manual transport** (package generation, validation, copy/export, evidence import) unless later PA disposition changes it. |

P1 (extension/CLI) and P2 (MCP/ACP) are future investigations ([AWI-0006](../../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)).

PAR SHALL map validated handovers to Cursor PLAN / AGENT / DEBUG routing intent; bridge MUST NOT escalate modes or authorize implementation without governed workflow state.

## 12. Canonical Markdown awareness — Tier 0 (PC-PAR-T0)

Tier 0 MAY include:

- Git HEAD / baseline commit identity
- Known canonical path existence (for example `docs/Program/Gate_Reviews/`, `ARCHITECTURE_DECISIONS.md`)
- Explicitly recognized governance documents and narrowly parsed status metadata (not full EDF semantic parse)
- Data required to build governed workflow packages

Tier 0 MUST NOT duplicate M2+ EDF engine discovery, profile resolution, or conformance validation.

## 13. Provisional attribution (pre–PCON-0002)

Minimal labels such as **Project Architect** and **Engineering Agent / Cursor** MAY appear in packages for routing and provenance.

They are **provisional transport attribution**, not normative Actor/Role assignments. Implementation that materially depends on Actor/Role semantics MUST wait for PCON-0002 reconciliation.

## 14. STOP and MVR boundaries

- Workflow STOP per PC-AIGOV-007 and relay-boundary enforcement are in scope for PAR when implemented.
- **STOP-2** (execution MVR instance) remains **binding** and out of PAR scope ([SPEC-005](SPEC-005-manual-verification-record-consumption.md)).
- [ADR-0014](../../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) remains **Proposed** unless separately governed.

## 15. Acceptance criteria (future — not applicable until tranche authorization)

When a PAR implementation tranche is authorized, acceptance tests SHALL demonstrate at minimum:

- Project ID survives locator rename when reconciled
- Open/select does not create `.projectconcord/`
- Recent roots stored per-user only
- INCOMPLETE PA handover blocks automated relay
- Complete handover routes correct Cursor mode intent on P0 path
- Advisory does not override user NEW/CONTINUE
- Package provenance links export to import without provider-specific IDs
- Manual ChatGPT adapter path works without API
- No core behavior requires ChatGPT product

## 16. Open questions

- Exact PA handover serialization (Markdown blocks vs structured file)
- Project ID generation algorithm (UUID vs ULID)
- Multi-root workspace UI (future ADR-0013 / PC-AIGOV-022)

## Maintenance

Update when PAR tranches are authorized. [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) **Accepted** 2026-09-29.
