[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Specifications](../README.md) › SPEC-006

# SPEC-006: Project Root, Identity, and Governed Workflow Relay (historical PAR track)

## Metadata

| Field | Value |
|---|---|
| **Spec ID** | SPEC-006 |
| **Status** | Accepted |
| **Accepted** | Project Architect — **2026-09-29** |
| **Owner** | ProjectConcord |
| **Normative** | Yes — Project identity/session (**A**), Core Governed Interaction Relay (**A**), per-user operational state (**A**), Software Development governance at relay boundary (**B**), provider/tool adapters (**E**); historical **Project Architect Relay (PAR)** track terminology |
| **Last Reviewed** | 2026-09-29 (reconciliation tranche; Project Architect acceptance) |
| **Target release** | Historical PAR track A1–A4 (separately authorized); not part of M1 |
| **Implementation** | **A1** (identity + per-user state): **published** 2026-09-28 — see [Implementation Roadmap](../../Development/Implementation_Roadmap.md). **A2–A4** (governed interaction relay, workflow MVP, Cursor bridge automation): **not authorized**. Accepted SPEC-006 satisfies the specification/governance prerequisite for Project Architect **consideration** of **A2**; acceptance does **not** authorize A2. |
| **Architecture baseline** | Accepted [ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), [ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md), [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md), [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) — repository baseline at reconciliation `084c6f043e27db2c9bdca558f14216de596f3c8d` |

## Parent

- [Specifications](../README.md)

## Related Documents

- [ADR-0015 — Project Identity, Project Root, and Per-User Operational State](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) — **Accepted** 2026-09-29
- [ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md) — **Accepted**; Core/domain/Working Environment decomposition; Governed Interaction Relay
- [ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) — **Accepted**; **ProjectWorkRecord** boundary (no material SPEC-006 dependency)
- [SPEC-004 — AI-assisted development governance workflow](SPEC-004-ai-assisted-development-governance-workflow.md) — PC-AIGOV-001–028; Software Development semantic source of truth; SPEC-006 enforces at relay boundary without redefining them
- [SPEC-001 — MVP EDF desktop client](SPEC-001-mvp-edf-desktop-client.md) — M2+ EDF engine depth
- [ADR-0002](../../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md), [ADR-0004](../../Architecture/ADRs/ADR-0004-Derived-Data-and-Cache.md), [ADR-0006](../../Architecture/ADRs/ADR-0006-AI-Boundary.md), [ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)
- [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) — **Accepted** 2026-09-29
- [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md) — historical/provenance
- [AWI-0006](../../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)
- [AMD-0003](../../Architecture/AMD-0003-Core-Domain-Extension-and-Working-Environment-Capability-Model.md)
- [SPEC-006 Reconciliation Documentation Tranche Plan](../../Handover/ProjectConcord-SPEC-006-Reconciliation-Documentation-Tranche-Plan.md)

---

## 1. Purpose

Define normative ProjectConcord product behavior across accepted component boundaries ([ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)):

**A — Core (application shell + Governed Interaction Relay)**

- Project Root lifecycle and stable **ProjectConcord Project ID** ([ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md));
- per-user operational application state (including Recent Project Roots);
- transport-neutral governed interaction packages: assembly, validation, correlation identity, provenance, relay-boundary **STOP** enforcement hooks, and **INCOMPLETE** metadata gating.

**B — Software Development / Engineering extension**

- Software governance package/profile content consumed at the relay boundary (for example **PA Review Package** semantics, **DevelopmentWorkAuthorization**-related context, submissions) per [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) and [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md).

**D — Working Environment / policy**

- Supervision, checkpoint, or escalation information **where configured** and referenced in relay packages — without defining new Working Environment facets or persistence.

**E — Provider / tool adapters**

- Provider-neutral **Project Architect** exchange; human-mediated and future automated providers;
- **Engineering Agent** relay to Cursor via a **CursorBridge** abstraction;
- adapter rendering and parsing (for example ChatGPT field names); not governance semantics.

**F — EDF canonical governance**

- Consumed and referenced at relay boundaries; **not** redefined or superseded by operational state ([ADR-0002](../../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)).

**Historical terminology:** **Project Architect Relay (PAR)** may appear as track/provenance naming. It is **not** a monolithic architectural component owner.

**Non-goals:**

- Replace [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) or duplicate its complete Software Development semantic model.
- Implement full M7a governance entities before separately authorized tranches.
- Require **ProjectWorkRecord** identity for relay or package correlation ([ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md)).

## 2. Scope

### Terminology (not synonyms)

| Term | Layer | Role in this spec |
|---|---|---|
| **ProjectConcord Project ID** | **A** | Durable logical identity and operational partition key |
| **Project Root** | **A** session | Currently selected filesystem **session locator** — not durable identity |
| **Repository identity / Git remote** | VCS | Locator or network hint — not Project ID |
| **Repository or tool label** (“workspace name” in PC-PAR-003) | Label | Human or tool display name — not authoritative identity; **not** IDE multi-root workspace hosting, **not** Working Environment |
| **Multi-project application context** | **A** product shell | Multiple Project IDs in one application instance ([ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md), PC-AIGOV-022 in [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md)) |
| **Working Environment** | **D** | Composable policy/methodology facets — **not** Project Root, **not** synonymous with “governed operational state” |
| **Provider / agent session** | **E** | Adapter session state |
| **Persona / UI workspace** | UI | UX layout — not governance identity |
| **Historical PAR** | Umbrella | Maps to **A** + **B** + **D** + **E** — not a single owner |

### In scope (when implementation authorized)

- Explicit Project Root selection as session context (**A**)
- Stable Project ID assignment and locator reconciliation (**A**)
- Per-user SQLite operational store (**A**; architecture per [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md); **A1 published**)
- Governed interaction package export/import and validation (**A** + **B** + **E**)
- P0 manual transport (copy/export, paste/import) (**E**)
- Tier 0 shallow canonical awareness (**A**; inputs for **B** package profiles)
- Cursor handover validation and mode routing intent (**A** + **E**; P0: human delivers to Cursor — no automated bridge required)
- Software Development evidence/result ingestion correlation when extension enabled (**B** — see [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-008, PC-AIGOV-016)
- Provisional transport attribution labels (**E**; [PCON-0002](../../Architecture/PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) deferred)

### Out of scope (this specification does not authorize)

- M2+ EDF discovery, parsing, validation UI ([SPEC-001](SPEC-001-mvp-edf-desktop-client.md))
- OpenAI or other AI API integration
- Cursor extension, CLI, MCP, ACP implementation ([AWI-0006](../../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md))
- Creating `.projectconcord/` on open/select
- Normative Actor/Role model ([PCON-0002](../../Architecture/PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) — **Proposed**)
- **ProjectWorkRecord** schema, lifecycle, relationships, persistence, API, or UI ([ADR-0017](../../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md))
- MVR execution instances (**STOP-2** binding; [SPEC-005](SPEC-005-manual-verification-record-consumption.md))
- Working Environment facet definitions or persistence schemas (**D** — deferred per ADR-0016)

### Architectural ownership (normative mapping)

Normative requirements (PC-PAR-001–022) map to owners per [ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md):

| Owner | Scope in SPEC-006 |
|---|---|
| **A — Core Governed Interaction Relay** | Package/correlation identity; structural validation pipeline; provenance chain (PC-PAR-021); relay-boundary STOP hooks; INCOMPLETE gate; provider-neutral relay host (capability negotiation where Core) |
| **B — Software Development extension** | Governance package/profile content; handoff vs **DevelopmentWorkAuthorization**; authorization/planning/implementation semantics at boundary — **semantic source:** [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) / [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) |
| **D — Working Environment / policy** | Supervision, checkpoint, escalation **where configured** — reference only; no new facets |
| **E — Provider adapters** | `IProjectArchitectProvider`, manual ChatGPT formatting, `CursorBridge`, transport field rendering/parsing |
| **F — EDF canonical governance** | Authoritative artifacts in Git; gates, AAR/MVR, architectural acceptance — referenced/correlated, not owned by relay operational store |

**A2 governance:** Accepted SPEC-006 satisfies the specification/governance prerequisite for Project Architect **consideration** of **A2** authorization. **A2 is not authorized** by this specification.

## 3. Relationship to SPEC-004

| Concern | SPEC-004 (semantic source) | SPEC-006 (relay boundary enforcement) |
|---|---|---|
| Governance state vs chat transcripts | PC-AIGOV-002 | Derived packages; enforce separation at relay boundary (**A**) |
| Handover vs DevelopmentWorkAuthorization | PC-AIGOV-003 | MUST NOT merge at boundary; validate separation (**A** + **B** refs) |
| Planning vs implementation | PC-AIGOV-004 | MUST NOT infer implementation authorization at boundary (**A** + **B** refs) |
| Manual vs integrated providers | PC-AIGOV-005, 006 | P0 manual first (**E**); same semantic objects |
| STOP | PC-AIGOV-007 | Relay-boundary STOP hooks (**A**) + binding STOP-2 respect |
| Context reconstructability | PC-AIGOV-015 | Tier 0 (**A**) + operational provenance (**A**) |

SPEC-006 adds relay, identity, session, and provider requirements (PC-PAR-001–022). Where [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) is not yet implemented, SPEC-006 defines **minimum relay-boundary behavior** for authorized tranches (**A2+**) only — without restating the full Software Development entity model.

## 4. Identifier requirements (PC-PAR-001–004)

| ID | Requirement |
|---|---|
| **PC-PAR-001** | Each managed logical project SHALL have a stable **ProjectConcord Project ID** independent of filesystem path. |
| **PC-PAR-002** | **Project Root** SHALL denote the currently selected absolute filesystem locator for that session context. |
| **PC-PAR-003** | Repository identity, Git remote URL, and repository or **tool display label** (including colloquial “workspace name”) SHALL NOT be treated as equivalent to ProjectConcord Project ID. Such labels are not **Working Environment**, IDE multi-root workspace hosting, or provider session identity. |
| **PC-PAR-004** | When a Project Root path changes for the same logical project, ProjectConcord SHALL support reconciliation so **Core operational state partitioned by Project ID** remains continuous. Software Development workflow entities and governed lifecycle state remain in the **B** extension ([SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md)), keyed within the same Project ID partition when implemented. |

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
| **PC-PAR-010** | Schema versioning, migration, backup, and recovery SHALL remain documented and testable for the operational store. **A1** established and published the v1 foundation (2026-09-28); subsequent persistence evolution or tranches beyond that baseline SHALL maintain the same obligations before authorization. |
| **PC-PAR-011** | Application operational state (**A**) MUST NOT hold canonical EDF content as an **authoritative substitute** for Git-resident canonical artifacts (**F** — [ADR-0002](../../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)). Operational state MAY reference, snapshot, or correlate to canonical governance for relay and workflow purposes; it MUST NOT redefine canonical acceptance, gate closure, or EDF semantics. |

## 7. Governed Interaction Relay — core requirements (PC-PAR-012–015)

Historical PAR track sections map here to **Core Governed Interaction Relay** (**A**) with **B** / **D** / **E** participation as tagged.

| ID | Requirement |
|---|---|
| **PC-PAR-012** | The **Core Governed Interaction Relay** SHALL assemble governed interaction packages as follows: **[A]** transport-neutral package assembly; package and correlation identity; incorporation of authorized operational inputs and Tier 0 relay-safe context ([§12](#12-canonical-markdown-awareness--tier-0)). **[B]** When the Software Development extension is in scope, **PA Review Package** profile and content — including **DevelopmentWorkAuthorization**-related context, submissions, and Software Development workflow payload — per [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) and [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md); such content is **not** universal Core semantics. **[D]** Where Working Environment policy is configured, supervision, checkpoint, or escalation information MAY be included by reference to configured policy — without defining new Working Environment facets. |
| **PC-PAR-013** | Imported **PA handover responses** SHALL be validated as follows: **[A]** structural validation, required relay fields, validation pipeline, and **INCOMPLETE** disposition when governance-critical metadata is missing ([§9](#9-pa-handover-schema--governance-critical-fields-pc-par-020)). **[B]** Software Development semantic rules — including handover vs **DevelopmentWorkAuthorization** separation — per [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) (for example PC-AIGOV-003, PC-AIGOV-005); this spec does not duplicate the complete semantic model. **[E]** Provider-specific serialization parsing (delimited blocks, Markdown sections, or equivalent). |
| **PC-PAR-014** | **Cursor-directed handovers** SHALL be validated before **automated** Cursor relay: **[A]** Core validation and **INCOMPLETE** status MUST block **automated** Cursor relay. **[E]** Automated relay path is a **CursorBridge** / provider automation concern ([AWI-0006](../../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)). **P0 manual** copy/export and human paste to Cursor MUST NOT be described as requiring an implemented automated bridge. |
| **PC-PAR-015** | Missing governance-critical authorization or mode metadata MUST NOT be silently inferred from free prose or model output. **[A]** Core relay validation and inference safety at the boundary. **[B]** Meaning of authorization, planning vs implementation, and governed Software Development state — semantic source [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) / [ADR-0013](../../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (for example PC-AIGOV-003, PC-AIGOV-004). |

## 8. Provider separation (PC-PAR-016–019)

| ID | Requirement |
|---|---|
| **PC-PAR-016** | **[E]** Project Architect integration SHALL use a **provider-neutral boundary** (conceptually `IProjectArchitectProvider`). |
| **PC-PAR-017** | **[E]** **Manual ChatGPT product** workflow SHALL be one adapter (`ProjectArchitectManualAdapter` or equivalent), responsible for export formatting, import parsing, and human paste — not governance semantics. |
| **PC-PAR-018** | **[E]** Future providers (OpenAI API, local AI, other) SHALL be substitutable without redesigning the **decomposed** architecture: Core Governed Interaction Relay (**A**), Software Development governance state (**B**), Working Environment policy references (**D**), other provider adapters (**E**), Project Root / Project ID session model (**A**), relay-boundary STOP enforcement (**A**), or canonical EDF handling (**F**). |
| **PC-PAR-019** | **[E]** Providers MAY differ in capability. **[A]** The Core relay host SHALL support **capability discovery/negotiation** where needed rather than assuming ChatGPT-specific features. |

**No Core PC-PAR requirement SHALL depend on the ChatGPT product, OpenAI API, Cursor, or any particular AI provider.**

## 9. PA handover schema — governance-critical fields (PC-PAR-020)

For handovers intended to direct Cursor work, the following are **governance-critical**:

| Field | Owner | Required when |
|---|---|---|
| `Cursor-Mode` | **E** (rendering) | Always (PLAN, AGENT, DEBUG, or documented enum) |
| `Cursor-Chat` | **E** (rendering) | Always (NEW or CONTINUE — user-selected lifecycle) |
| `ChatGPT-Chat` | **E** (rendering) | Always for current manual adapter (maps from core `ProjectArchitectSession` intent) |
| `Cursor-Mode-Transition` | **E** (rendering) | When mode changes from prior authorized state (for example `AGENT -> PLAN`) |

**[B]** Authorization disposition, applicable STOP markers, and Software Development tranche/work context SHALL be represented explicitly in structured or delimited blocks (exact serialization is implementation-defined; validation rules are normative) per [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md).

**[F]** Operational correlation to canonical EDF governance artifacts (for example gate or tranche identifiers) MAY appear in packages for traceability. Such correlation MUST NOT mean ProjectConcord locally accepts, closes, or replaces canonical governance in Git.

**[A]** If governance-critical metadata is missing, status is **INCOMPLETE** — the relay MAY surface for human correction; **automated** Cursor relay is forbidden.

**Advisory fields** (for example `Cursor-Chat-Advisory`, `ChatGPT-Chat-Advisory`) are optional recommendations; they MUST NOT override user-selected lifecycle actions.

Outbound PA packages SHALL remind the Project Architect that the next Cursor handover must include `Cursor-Mode` (and transition when applicable).

## 10. Work-session and conversation provenance (PC-PAR-021)

| ID | Requirement |
|---|---|
| **PC-PAR-021** | **[A]** The Core Governed Interaction Relay SHALL model provenance as: **observed/current work-session context** → **advisory** → **user decision** → **requested next action** → **package produce/consume event**. |

NEW/CONTINUE alone is insufficient to determine which conversation produced or consumed a package.

Core concepts (provider-neutral):

- `ProjectArchitectSession` — user-selected continuity intent for Project Architect interaction
- `ProjectArchitectSessionAdvisory` — relay recommendation (not a substitute for user decision)
- `EngineeringAgentSession` / advisory — parallel concepts for Cursor (**E** may render as `Cursor-Chat` / `Cursor-Chat-Advisory`)

**[E]** Provider-specific conversation IDs (for example ChatGPT thread IDs) MUST NOT be required in core provenance.

**[A]** Correlation SHALL use ProjectConcord-generated work-session and package identifiers, timestamps, Project ID, and optional content hashes. Optional future correlation with **ProjectWorkRecord** identifiers is permitted but **not required** for relay identity.

Provisional transport attribution ([§13](#13-provisional-attribution-pre-pcon-0002)) does not resolve Actor/Role ([PCON-0002](../../Architecture/PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md)).

## 11. Engineering agent / Cursor bridge (PC-PAR-022)

| ID | Requirement |
|---|---|
| **PC-PAR-022** | **[E]** Cursor integration SHALL use a **CursorBridge** abstraction. First implementation target is **P0 manual transport** (package generation, validation, copy/export, evidence import) unless later PA disposition changes it. P1 (extension/CLI) and P2 (MCP/ACP) are future investigations ([AWI-0006](../../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md)). |

**[A]** The relay SHALL map **validated** handovers to Cursor PLAN / AGENT / DEBUG routing intent.

**[B]** The bridge MUST NOT escalate modes or **authorize implementation** without governed Software Development workflow state per [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md) (for example PC-AIGOV-004, PC-AIGOV-007).

**A2 / A4** implementation remains **not authorized**.

## 12. Canonical Markdown awareness — Tier 0

Tier 0 is a shallow-awareness boundary per [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) §5.

Tier 0 MAY include:

- **[A]** Git HEAD / baseline commit identity
- **[A]** Known canonical path existence (for example `docs/Program/Gate_Reviews/`, `ARCHITECTURE_DECISIONS.md`)
- **[A]** Explicitly recognized governance documents and narrowly parsed status metadata (not full EDF semantic parse)
- **[A]** Relay-safe baseline pointers and context needed for safe session/relay operation; **[B]** may consume these inputs when assembling Software Development package profiles

Tier 0 MUST NOT become an EDF parser, EDF validator, conformance engine, profile-resolution engine, or competing canonical interpretation engine. Tier 0 MUST NOT duplicate M2+ EDF engine discovery, profile resolution, or conformance validation ([SPEC-001](SPEC-001-mvp-edf-desktop-client.md)).

## 13. Provisional attribution (pre–PCON-0002)

Minimal labels such as **Project Architect** and **Engineering Agent / Cursor** MAY appear in packages for routing and provenance.

They are **provisional transport attribution**, not normative Actor/Role assignments. Implementation that materially depends on Actor/Role semantics MUST wait for [PCON-0002](../../Architecture/PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) disposition (**Proposed**). [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) §7 records that unresolved PCON-0002 did not block ADR-0015 acceptance; **PCON-0002 did not block SPEC-006 acceptance** (Project Architect, 2026-09-29). Provisional attribution remains in effect until PCON-0002 disposition.

## 14. STOP and MVR boundaries

- **[A]** Workflow STOP per PC-AIGOV-007 and relay-boundary enforcement are in scope for the Core Governed Interaction Relay when implemented.
- **STOP-2** (execution MVR instance) remains **binding** and out of relay scope ([SPEC-005](SPEC-005-manual-verification-record-consumption.md); **F** attestation boundary).
- [ADR-0014](../../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) remains **Proposed** unless separately governed.

## 15. Acceptance criteria (future — not applicable until tranche authorization)

When a relay implementation tranche (for example **A2**) is authorized by the Project Architect, acceptance tests SHALL demonstrate at minimum:

- Project ID survives locator rename when reconciled (**A1** baseline)
- Open/select does not create `.projectconcord/`
- Recent roots stored per-user only
- INCOMPLETE PA handover blocks **automated** relay
- Complete handover routes correct Cursor mode intent on P0 manual path
- Advisory does not override user NEW/CONTINUE
- Package provenance links export to import without provider-specific IDs
- Manual ChatGPT adapter path works without API
- No Core behavior requires ChatGPT product or any particular provider

## 16. Open questions

- Exact PA handover serialization (Markdown blocks vs structured file)
- Project ID generation algorithm (UUID vs ULID)
- Multi-project application context — product-shell project hosting/switching UX (**A**; architectural prerequisite PC-AIGOV-022 in [SPEC-004](SPEC-004-ai-assisted-development-governance-workflow.md); identity/partitioning [ADR-0015](../../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md), [ADR-0016](../../Architecture/ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md)) — UI design deferred; **ADR-0013** does not own generic multi-project shell

## Maintenance

**Accepted** by Project Architect **2026-09-29** following reconciliation review. Reconciled against Accepted ADR-0016, ADR-0017, ADR-0013, ADR-0015. Tranche record: [SPEC-006 Reconciliation Documentation Tranche Plan](../../Handover/ProjectConcord-SPEC-006-Reconciliation-Documentation-Tranche-Plan.md). Update when relay implementation tranches (A2+) are authorized or architecture materially changes. Acceptance does **not** authorize **A2**, **A3**, **A4**, or `src/` implementation.
