[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › AI Governance Workflow Integration Analysis

# AI-Assisted Development Governance — Integration Analysis

> **Status:** Integrated — documentation tranche **Project Architect accepted** (2026-09-21)  
> **Date:** 2026-09-21  
> **Source:** [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) (root handover integrated 2026-09-21)  
> **Integration commit:** `b728e2896992b58ee785d406ac93a6badf29c8c8`

## Purpose

Satisfy PCON-0001 §49: map the AI governance and inter-project workflow proposal to accepted ProjectConcord architecture; identify overlap, conflicts, gaps, lock-in risks, and documentation targets. **No implementation is authorized by this analysis.**

## Documents created or updated (this tranche)

| Document | Role |
|---|---|
| [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) | Architectural Discovery Record (Proposed) |
| [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) | Normative product requirements (Draft; **not implemented**) |
| [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) | Architecture decisions ( **Proposed** ) |
| [Implementation Roadmap](../Development/Implementation_Roadmap.md) | M7+ governance / inter-project phasing |
| [EDF Gap Register](../Development/EDF_Gap_Register.md) | GAP-027–GAP-036 |
| [docs/AI/README.md](../AI/README.md) | Domain index |
| Architecture / Specifications README, PROJECT_INDEX, ARCHITECTURE_DECISIONS | Cross-links |

Root handover removed per SPEC-003 precedent; content preserved in PCON-0001.

## Overlap with existing architecture

| Proposal concept | Existing anchor | Integration note |
|---|---|---|
| Human authority; AI proposals only | [ADR-0006](ADRs/ADR-0006-AI-Boundary.md), PCON-0000 §29 | SPEC-004 extends to **repository execution** governance, not only EDF authoring |
| Canonical vs derived (prompts, UI) | [ADR-0002](ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md), [ADR-0004](ADRs/ADR-0004-Derived-Data-and-Cache.md), [CRA Alignment](CRA_Alignment_and_Responsibility_Boundaries.md) | DevelopmentWorkAuthorization and handover packages are **derived/operational**, not chat transcripts |
| External Cursor/IDE edits | [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) §26–27 | Integrity + **execution authorization** are complementary; do not merge models |
| Gates / tranches; no auto-progression | EGR records, PCON-0000 | EGR satisfaction remains EDF artifacts; execution auth is operational |
| AAR | [ADR-0012](ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md) | **Significant** milestone/gate reviews; routine tranche review uses ArchitecturalReviewSubmission (architect disposition) |
| Git ≠ authorization | System Architecture Overview | Extended with branch/PR as **evidence**, not acceptance (PC-AIGOV-026) |
| Operational store | [ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) | Default home for governance runtime entities |
| AWI (watch items) | PCON-0000, System Overview | **HumanInitiatedWorkItem** remains **distinct** from AWI; may promote to AWI |
| Cursor handover example | PCON-0000 §48 | Subset of full handover + DevelopmentWorkAuthorization model |
| Multi-user within one project | ADR-0009, ADR-0010 | Distinct from **multi-project workspace** (PCON-0001 §4F) |

## Conflicts / tensions (resolved in ADR-0013 / SPEC-004)

| Tension | Resolution |
|---|---|
| “Authorization” in SPEC-003 (artifact lifecycle) vs execution authorization | Use **DevelopmentWorkAuthorization** for repository work; cross-reference SPEC-003 without shared vocabulary collision |
| Agile / backlog (GAP-011) vs HIW | HIW and inter-project records operational; optional `tasks/` conventions; not canonical Agile schema |
| AMD “persona workspace” vs governance workspace | Glossary: **persona workspace** (UI) ≠ **multi-project workspace** (PCON-0001 §4F) — see GAP-036 |
| M7+ bucket “Reconciliation & AI” | Split narrative: M7a–c single-project governance; M7d/M8 inter-project (no MVP pull-forward) |

## Entity boundaries (architect disposition applied in drafts)

| Entity | Default placement | Notes |
|---|---|---|
| DevelopmentWorkAuthorization | Operational | Per project |
| ArchitecturalReviewSubmission | Operational | Routine tranche review |
| HumanInitiatedWorkItem | Operational | Distinct from AWI |
| InterProjectHandover | Operational | May **materialize** destination EDF artifacts when accepted |
| CrossProjectDependency | Operational | EDF canonical capability **not** assumed; candidates recorded for later |
| Destination EDF artifacts | Canonical Git | Only via destination governance |
| Branch / commit / PR | Git + operational correlation | Contribution/evidence, not architectural acceptance |

## Single-project lock-in risks (repository inspection)

| Risk | Evidence | Severity |
|---|---|---|
| SPEC-001 UX “open one project root” | [SPEC-001](../Specifications/features/SPEC-001-mvp-edf-desktop-client.md) acceptance criteria | **Low** if M1–M5 avoid irreversible single-project **runtime** assumptions (see ADR-0013) |
| Illustrative `Open project root` in Engine table | [System Architecture Overview](System_Architecture_Overview.md) | **Medium** — documentation should not imply one global repository for all app state |
| `.projectconcord/` per repo ([ADR-0004](ADRs/ADR-0004-Derived-Data-and-Cache.md)) | Per-project derived cache | **Positive** — aligns with multi-project |
| `IProjectRepository` scoped to root ([ADR-0005](ADRs/ADR-0005-Repository-Abstraction.md)) | Root-relative abstraction | **Positive** — instance-per-project is a **candidate** seam, not yet mandated |
| No workspace registry in codebase | No `src/` yet | **None today** — M1 design must not bake in “only one project forever” |
| Operational store schema undefined (GAP-018) | Gap register | **Medium** — future schema must allow multiple project partitions |

## M1–M5 architectural constraint (accepted requirement)

**Normative:** M1–M5 architecture **MUST NOT** establish an implicit or irreversible assumption that one ProjectConcord application/runtime corresponds to exactly one project/repository. Foundational architecture must preserve a practical migration path to multiple independently governed projects within one ProjectConcord workspace.

**Not mandated in this tranche:** Specific mechanisms (stable `ProjectId`, UUID identity, `IProjectScope`, repository factories, `workspaceId` columns, dedicated `Edf.Workspace` assembly, service rebinding patterns). These are **candidate approaches** documented in ADR-0013 for future disposition.

**Issue for separate architect review if M1 proceeds:** If implementation inspection shows an unavoidable early seam choice, record as recommendation — do not silently mandate in SPEC-001 without disposition.

## Candidate early seams (not accepted requirements)

| Candidate | Rationale | Status |
|---|---|---|
| Project-scoped service instances (repository, Git, cache) | Avoid global mutable “current repo” | Candidate |
| Stable project identity separate from filesystem path | Supports registry and switching | Candidate |
| Application APIs that accept explicit project context | Avoid hidden singleton | Candidate |
| Workspace-level operational partition for inter-project records | IPH/DEP scope | Candidate (M7d+) |
| OS app data for workspace registry vs per-repo `.projectconcord/` | ADR-0004 already per-repo | Partially aligned (ADR-0004 accepted for cache location only) |

## EDF upstream

**Do not upstream PC-AIGOV-022–028 now.** Record candidates in gap register (GAP-035) for later EDF review. PC-AIGOV-001–021 remain primarily ProjectConcord product requirements unless EDF later adopts execution-authorization vocabulary.

## Implementation stages (planning)

| Stage | Scope | Authorized |
|---|---|---|
| M1–M5 | SPEC-001 MVP; **no** governance UI; honor non-lock-in constraint | Per EGR-G1 |
| M7a | Manual single-project governance (SPEC-004 Phase A) | Future |
| M7b | Structured submission, Git correlation | Future |
| M7c | Provider adapters | Future |
| M7d / M8 | Inter-project workspace, IPH, dependencies, traceability | Future |
| M8+ | PR automation, permissions | Future |

## Unresolved architectural questions (remain OPEN)

Per architect direction — do not close in this documentation tranche:

- Workspace/project identity representation (UUID vs path-keyed registry)
- Cloud synchronization of workspace registry
- Permission implementation for target branch/PR creation
- Event mechanism for source notification after target acceptance (GAP-034)
- Unloaded / read-only / external-organization target projects (GAP-033)
- Exact cross-project dependency addressing (commit vs spec ID vs version capability)
- Provider API enforcement of read-only planning (SPEC-004 PC-AIGOV scope)
- Whether any M1 seam becomes mandatory before skeleton merge (issue-driven)

## Validation performed (documentation tranche)

- Cross-read PCON-0001 against ADR-0001–0012, SPEC-001–003, Implementation Roadmap, EDF Gap Register
- Verified GAP-026 already allocated (AAR discovery); new gaps use GAP-027+
- No `src/` changes; no Framework Advisor re-run required for doc-only tranche

## Documentation tranche closeout

| Field | Value |
|---|---|
| **Disposition** | Project Architect **accepted** documentation integration scope |
| **Accepted commit** | `b728e2896992b58ee785d406ac93a6badf29c8c8` |
| **Closeout date** | 2026-09-21 |
| **PCON-0001** | Remains **Proposed** / non-normative |
| **SPEC-004** | Remains **Draft** / **not implemented** |
| **ADR-0013** | Remains **Proposed** (not Accepted) |
| **OPEN questions** | Unchanged — not resolved in this tranche |

**Acceptance does not authorize:** SPEC-004 implementation, M7+ work, multi-project/inter-project features, Git/PR automation, EGR changes, or ADR-0013 acceptance.

## Post-closeout architectural discovery (queued)

Discovery recorded **after** tranche closeout (`2dfdc97c84c5c464ce7fe7263bbe400e6ba3dcdc`); **not** part of the accepted integration scope above.

| Artifact | Role |
|---|---|
| [PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) | Architectural Discovery Record (Proposed) — Role/Actor/RoleAssignment, domain neutrality, candidate PC-AIGOV-029–051 |
| [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md) | Active watch — future reconciliation with PCON-0001, ADR-0013, SPEC-004 |
| [GAP-037](../Development/EDF_Gap_Register.md) | Candidate requirements index only |
| [PCR-0001](../Development/PCR-0001-Project-Continuation-and-Pause-Record.md) | Authoritative resume sequence and STOP state |
| [PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md) | Architectural Discovery (Proposed) — formal governed pause/continuation/resume capability |
| [AWI-0002](Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md) | Active watch — separate from AWI-0001 |
| [GAP-038](../Development/EDF_Gap_Register.md) | Candidate PC-AIGOV-052–059 index only |

## Parent

- [Architecture](README.md)

## Related Documents

- [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)
- [PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md)
- [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md)
- [PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md)
- [AWI-0002](Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md)
