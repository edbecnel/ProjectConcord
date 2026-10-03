[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Development](README.md) › EDF Gap Register

# EDF Gap Register

> **Status:** Draft  
> **Owner:** ProjectConcord  
> **Applies To:** Deterministic EDF Engine design  
> **Last Reviewed:** 2026-10-03 (M7a-WF-1-doc GEW v1 topology / traversal semantics — GAP-054)  
> **Authoritative:** Yes — interim policies reference ADRs where binding

## Purpose

Record gaps between what ProjectConcord requires for **deterministic** EDF interpretation and what the authoritative [Engineering Documentation Framework](https://github.com/edbecnel/Engineering-Documentation-Framework) currently specifies. Answers derive from PCON-0000 §55 and inspection of EDF sources (Documentation Information Architecture, capabilities, `edf_profile.sh`, Framework Advisor).

## Validation Baseline

| Field | Value |
|---|---|
| **Report** | `reports/conformance/framework-advisor-20260915-095038.txt` |
| **Overall** | 58% |
| **Structure** | 97% |
| **Navigation** | 70% |
| **Governance** | 55% |
| **AI handbook** | 10% |

Framework Advisor checks directory presence, root files, AI handbook completeness, relative link integrity, breadcrumb patterns, orphan detection, and governance metadata — not full semantic EDF interpretation.

---

## Gap Summary

| ID | Topic | Severity | Interim policy |
|---|---|---|---|
| GAP-001 | Formal EDF project identity marker | High | ADR-0005; infer from `edf-adoption.yaml` + context |
| GAP-002 | EDF framework version on disk | High | User-configured EDF clone path + git tag |
| GAP-003 | Machine-readable artifact type registry | High | Heuristic classification + gap log; no silent policy |
| GAP-004 | Artifact lifecycle state machine | High | Parse Status tables/headings; validate known enums only |
| GAP-005 | Explicit relationship syntax | High | Link graph + heading refs; infer with confidence flag |
| GAP-006 | Milestones and gates representation | Medium | **Partial:** EDF [EGR-0001 v1.1](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md) (incl. Governed Dependency Override semantics); ProjectConcord [EGR-G0/G1](../../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md); AAR under [Audits/](../../Architecture/Audits/README.md) per [ADR-0012](../../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md); consumption guide [GDO handover](../Handover/EDF-Governed-Dependency-Override-Architecture-Handover.md); gate dashboard/UI deferred (M5+); see [GAP-040](#gap-040--gdo-dependency-evaluation-and-egr-round-trip) |
| GAP-007 | AWI / discovery record ID conventions | Low | Filename + metadata patterns per Architecture README |
| GAP-008 | SPEC / ADR identifier enforcement | Medium | Regex + location rules from DIA and domain READMEs |
| GAP-009 | Move/rename/supersession rules | Medium | Git + explicit user action; ADR-0004 cache invalidation |
| GAP-010 | Structured authoring ↔ Markdown mapping | High | MVP: templates; full schema deferred |
| GAP-011 | Agile entity canonical representation | Medium | Out of MVP; optional local store per ADR-0002 |
| GAP-012 | Validation evidence / acceptance records | Medium | Discover by convention; no formal schema |
| GAP-013 | Reconciliation baseline recording | Medium | Design in architecture; implement post-M3 |
| GAP-014 | Code ↔ EDF artifact association | Medium | Roslyn/git heuristics; human approval required |
| GAP-015 | Stable canonical artifact identity | High | ID-first resolution per [ADR-0007](../Architecture/ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md); see [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) §5 |
| GAP-016 | Formal vs navigational Markdown links | High | Relationship index with confidence; formal edges only when EDF defines or user confirms |
| GAP-017 | CRA semantics for deterministic import | Medium | Full analysis: [CRA ↔ ProjectConcord Gap Analysis](CRA_ProjectConcord_Gap_Analysis.md); [ADR-0008](../Architecture/ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md) |
| GAP-018 | Operational store schema for membership/audit/change sets | Medium | [ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md); PAR architecture: per-user SQLite in app data, partitions by Project ID ([ADR-0015](../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)); **not implemented** |
| GAP-019 | Persona vs authorization role taxonomy | Medium | [ADR-0010](../Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md); UI personas documented separately from RBAC |
| GAP-020 | Independent EDF/EGR review vs Administrator UX | Medium | Never bypass EDF-prescribed independent review; surface in gate UI |
| GAP-021 | Concurrent canonical edit / change-set protocol | High | Optimistic concurrency + change sets; defer implementation to M6+ spec |
| GAP-022 | Governed field registry (EDF) | High | [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md); [ADR-0011](../Architecture/ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md) |
| GAP-023 | Machine-readable lifecycle transitions | High | Semantic transition ops; flag external governed edits |
| GAP-024 | Transition prerequisites / acceptance rules | Medium | EGR human gates; app surfaces independent review |
| GAP-025 | Trusted integrity record portability | Medium | Rebuild from Git + validation; operational fingerprints optional |
| GAP-026 | AAR discovery in Engine/UI | Medium | Parse `docs/Architecture/Audits/AAR-*.md` status (Open/Complete/Superseded); EDF [AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md); post-M5 |
| GAP-027 | DevelopmentWorkAuthorization schema and identity | High | [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md); operational per [ADR-0013](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md); **architectural** correlation to prescribed workflow/profile/instance per [ADR-0024](../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03); M7+ |
| GAP-028 | Handover vs authorization rendering / rollover | Medium | Derived packages; [PCON-0001](../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) §23–24 |
| GAP-029 | HumanInitiatedWorkItem vs AWI / backlog / PWR | Medium | HIW = intake/triage (SPEC-004 normative name); vs **ProjectWorkRecord** ([ADR-0017](../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) **Accepted** — architectural boundary only); **OperationalIntakeRecord** = working terminology only; final intake name/ownership deferred |
| GAP-030 | Provider adapter interface, security, and Engineering Agent plugin boundary | Medium | **Closed / resolved** — [ADR-0021](../Architecture/ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) (2026-10-01); [GAP-030 detail](#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) |
| GAP-031 | Commit ↔ DevelopmentWorkAuthorization correlation | High | Scope conformance PC-AIGOV-010; M7b |
| GAP-032 | Cross-project relationships, launch, and inter-project continuity | Medium | **Partial policy:** Project ID per [ADR-0015](../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md); **Single-Project Focus** + multi-instance per [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) (**Accepted** 2026-10-01); IPH/CPD schemas [GAP-033](../Development/EDF_Gap_Register.md#gap-033--interprojecthandover-operational-schema)–[GAP-034](../Development/EDF_Gap_Register.md#gap-034--crossprojectdependency-and-source-notification); launcher/UX OPEN |
| GAP-033 | InterProjectHandover operational schema | High | Materialization to destination EDF; unloaded target behavior OPEN |
| GAP-034 | CrossProjectDependency and source notification | Medium | PC-AIGOV-027–028; event mechanism OPEN |
| GAP-035 | EDF upstream candidates (PC-AIGOV-022–028) | Low | Record only; no upstream in current tranche |
| GAP-036 | Persona workspace vs governance workspace naming | Low | AMD-0002 vs PCON-0001 §4F; UI glossary |
| GAP-037 | Candidate PC-AIGOV-029–051 (role/actor/domain profiles) | Low | Record only; [PCON-0002](../Architecture/PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md); [AWI-0001](../Architecture/Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md); not normative in SPEC-004 |
| GAP-038 | Candidate PC-AIGOV-052–059 (pause/continuation/resume) | Low | Record only; [PCON-0003](../Architecture/PCON-0003-Governed-Pause-Continuation-and-Resume.md); [AWI-0002](../Architecture/Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md); not normative in SPEC-004 |
| GAP-039 | Candidate PC-AIGOV-060–071 (primary orchestration / external integration) | Low | Record only; [PCON-0004](../Architecture/PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md); [AWI-0003](../Architecture/Watch_Items/AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md); not normative in SPEC-004 |
| GAP-040 | GDO dependency evaluation, EGR round-trip, governance debt UX | Medium | EDF normative via EGR-0001 v1.1; Concord consumption per [GDO handover](../Handover/EDF-Governed-Dependency-Override-Architecture-Handover.md); extends GAP-006; not AWI |
| GAP-041 | GMFP / GMR consumption and separate prescribed workflow integration | Medium | EDF normative via [GMFP-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Specifications/GMFP-0001-Governed-Maintenance-Fast-Path.md) at `b158f4a`; GMFP as **separate prescribed workflow** (`edf.governed-maintenance-fast-path`) per [ADR-0024](../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03); [GMFP handover](../Handover/EDF-Governed-Maintenance-Fast-Path-Architecture-Handover.md); [AWI-0004](../Architecture/Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md); **not implemented** |
| GAP-042 | MVR / MVT consumption, attestation, pending QA | Medium | EDF normative via [MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md) at `192fe5c`; Concord consumption per [MVR handover](../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md), [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md), [ADR-0014](../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md); [AWI-0005](../Architecture/Watch_Items/AWI-0005-Manual-Verification-Records.md); **not implemented** — future parser, pending-QA query, human attestation write-back, ATTENTION UI, read-only gate hints |
| GAP-043 | PAR runtime, Project ID registry, per-user SQLite, package validation | High | Normative [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md), [ADR-0015](../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md); [PAR plan](../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md); **A1 published**; **A2 P0 governed relay published**; remaining workflow breadth **A3** (not authorized) |
| GAP-044 | Engineering Agent automated transport architecture | Medium | **Closed / resolved** — [ADR-0022](../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) (**Accepted** 2026-10-01); A4 **not authorized** ([GAP-044 detail](#gap-044--engineering-agent-automated-transport)) |
| GAP-045 | Third-party NuGet dependency advisories (SQLite transitive) | Low | A1b: `NU1903` on `SQLitePCLRaw.lib.e_sqlite3` 2.1.10 via `Microsoft.Data.Sqlite` 9.0.3; [GHSA-2m69-gcr7-jv3q](https://github.com/advisories/GHSA-2m69-gcr7-jv3q); [AWI-0007](../Architecture/Watch_Items/AWI-0007-SQLite-Transitive-NuGet-Advisory.md); watch only — no mandatory A1b remediation |
| GAP-046 | Project Work Record — implementation, persistence, lifecycle states, relationship types, intake ownership | Medium | Core ownership **resolved** architecturally ([ADR-0017](../Architecture/ADRs/ADR-0017-Project-Work-Record-Core-Boundary.md) **Accepted**, [AMD-0004](../Architecture/AMD-0004-Project-Work-Record-and-Coordination-Capability-Model.md)); **not implemented** — persistence/APIs/lifecycle/relationship schemas deferred; effective-configuration provenance mechanism OPEN |
| GAP-047 | Terminology projection and optional enforcement | Medium | EDF TGR dependency **resolved** ([EDF Terminology handover](../Handover/EDF-Terminology-Governance-Architecture-Handover.md), [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) **Accepted** 2026-10-01); **not implemented** — presentation resolver, search aliases, enforcement/lint, living-doc SI migration separately governed |
| GAP-048 | Same Project ID — multiple local application processes (same user/installation) | High | [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) (**Accepted** 2026-10-01); expected behavior, coordination, warnings, read/write semantics, future sync interaction — **OPEN**; does **not** prohibit future multi-client collaborative Project access |
| GAP-049 | Operational synchronization architecture | High | Client sync subsystem; Cloud Service API contract; routine connected sync vs reconnection reconciliation — [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); **not implemented** |
| GAP-050 | Resource-level operational concurrency policy and version model | High | Optimistic concurrency direction per [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); per-resource semantics; **not implemented** |
| GAP-051 | Application-layer persistence / service-port isolation | Medium | Remove Application → concrete SQLite store coupling ([ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) §4); **not implemented** |
| GAP-052 | Multi-process local operational-store concurrency strategy and validation | Medium | Shared per-user `user-state.db` across processes — semantics not yet designed/validated ([ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md)); investigation OPEN — **not** a claim that SQLite shared access is inherently unsafe |
| GAP-053 | macOS **Go to Folder…** project locator vs browse-open identity | Medium | **Observed defect** (2026-10-02, A4-T7 human MVR) — path entry for disposable Root-A opened wrong Project ID; browse-open correct; **distinct from** Cursor ACP non-fast blocker; see [GAP-053 detail](#gap-053--macos-go-to-folder-project-locator-vs-browse-open-identity) |
| GAP-054 | Workflow framework runtime, registry, effective-configuration resolver | High | [PCON-0005](../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) (**Accepted** 2026-10-03), [ADR-0024](../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03); [AWI-0010](../Architecture/Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md); **M7a-WF-1 partial** (GEW v1 instance persistence, registry slice, recovery foundation); effective-configuration resolver, DWA, full orthogonal recovery **open** |
| GAP-055 | Governed Interactive Engineering Session runtime, hold persistence, routing | High | [PCON-0006](../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) (**Accepted** 2026-10-03), [ADR-0025](../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) (**Accepted** 2026-10-03); [AWI-0011](../Architecture/Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md); **not implemented** |

---

## Detailed Gaps

### GAP-001 — How is an EDF project formally identified?

| Field | Content |
|---|---|
| **Question (§55.1)** | How is an EDF project formally identified? |
| **Current EDF behavior** | Adopting projects use `docs/` as documentation root, optional `edf-adoption.yaml`, optional `edf-project-context.yaml`, and generated `ENGINEERING_DOCUMENTATION_FRAMEWORK.md`. No single mandatory “this is EDF” root marker file. |
| **Ambiguity** | Engine cannot distinguish an arbitrary Git repo from an EDF project without heuristics (presence of `docs/Architecture`, `PROJECT_INDEX.md`, etc.). |
| **Proposed EDF improvement** | Normative `edf-project.yaml` or mandatory adoption block with `schema_version` and `edf_core_version`. |
| **Interim ProjectConcord policy** | Treat as EDF project when `docs/` + (`edf-adoption.yaml` OR `edf-project-context.yaml` OR `ENGINEERING_DOCUMENTATION_FRAMEWORK.md`) exists; surface confidence level in UI. See ADR-0005. |

### GAP-002 — How is the EDF version represented?

| Field | Content |
|---|---|
| **Question (§55.2)** | How is the EDF version represented? |
| **Current EDF behavior** | EDF version lives in the framework repository (`CHANGELOG.md`, git tags), not in adopting project roots. |
| **Ambiguity** | Engine cannot know which rule set applies without external configuration. |
| **Proposed EDF improvement** | Adopter records `edf_framework_ref` (path or semver) in adoption config; optional compatibility matrix. |
| **Interim policy** | Settings: path to local EDF clone; run scripts from that path; display EDF git describe/tag in project dashboard. |

### GAP-003 — Project profiles and capabilities

| Field | Content |
|---|---|
| **Questions (§55.3)** | Profiles and capabilities representation. |
| **Current EDF behavior** | `edf-adoption.yaml` `profile:`; `edf-project-context.yaml` `capabilities:` and `legacy_profile:`; capability manifests under EDF `capabilities/*.yaml`; `edf_profile.sh` resolves required directories. |
| **Ambiguity** | Partially machine-readable; capability composition order and validation of unknown capability IDs not fully specified in one schema document in-repo. |
| **Proposed EDF improvement** | Single JSON Schema for adoption + context; validate capability IDs against registry URL or bundled index. |
| **Interim policy** | Parse YAML; resolve dirs via same logic as `edf_profile.sh` (invoke script or port rules); reject unknown capabilities with explicit error. |

### GAP-004 — Artifact types formally defined

| Field | Content |
|---|---|
| **Question (§55.4)** | Which artifact types are formally defined? |
| **Current EDF behavior** | Document types described in DIA, Architecture README (ADR, AWI, discovery record), Specifications README (SPEC), Governance templates — prose, not a unified machine registry. |
| **Ambiguity** | Classifier must infer type from path, filename, and headings. |
| **Proposed EDF improvement** | `edf-artifact-types.yaml` with detection rules, normative flag, allowed locations. |
| **Interim policy** | Rule table in Engine (versioned with app); log low-confidence classifications; never auto-fix without user confirm. |

### GAP-005 — Machine-readable metadata

| Field | Content |
|---|---|
| **Question (§55.5–6)** | Metadata and schemas. |
| **Current EDF behavior** | Templates define metadata tables; Framework Advisor checks some governance fields; interaction specs use YAML schema v2 for automation only. |
| **Ambiguity** | Required fields differ by document type; no JSON Schema per artifact type. |
| **Proposed EDF improvement** | Per-type JSON Schema + optional front matter YAML block. |
| **Interim policy** | MVP validation: required sections/headings from templates; YAML configs fully parsed. |

### GAP-006 — Relationships explicit vs inferred

| Field | Content |
|---|---|
| **Questions (§55.8–9)** | Relationships. |
| **Current EDF behavior** | Relative Markdown links; ADR/spec cross-refs; relationship diagrams in prose only. |
| **Ambiguity** | Graph edges require inference (link text, section “Related Documents”). |
| **Proposed EDF improvement** | Optional `edf-relations:` front matter or link convention (`rel:`). |
| **Interim policy** | Build derived graph from links; mark inferred edges; use for navigation not auto lifecycle. |

### GAP-007 — Milestones, gates, AWIs, ADRs, specifications

| Field | Content |
|---|---|
| **Questions (§55.10–16)** | Representation of program entities. |
| **Current EDF behavior** | ADRs: `docs/Architecture/ADRs/ADR-NNNN-*.md`; SPECs: suggested `docs/Specifications/features/SPEC-NNN-*.md`; AWIs: `docs/Architecture/Watch_Items/AWI-NNNN-*.md`; program gates: [EGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md) under `docs/Program/Gate_Reviews/`. |
| **Ambiguity** | Milestone entities beyond gate records still not fully schema-defined; checkbox state not machine-validated without tool support. |
| **Proposed EDF improvement** | Optional JSON Schema for EGR metadata; milestone index under `docs/Program/`. |
| **Interim policy** | Use EGR files per EGR-0001; ProjectConcord Engine SHOULD discover `docs/Program/Gate_Reviews/EGR-*.md` and parse Open/Satisfied status (post-M5). |

### GAP-008 — Lifecycle states and transitions

| Field | Content |
|---|---|
| **Questions (§55.17–18)** | Lifecycle. |
| **Current EDF behavior** | Status enums in templates (Draft, Accepted, etc.); Specifications lifecycle table in README. |
| **Ambiguity** | Valid transitions not formalized; ADR vs SPEC status vocabularies differ. |
| **Proposed EDF improvement** | State machine per artifact type in machine-readable form. |
| **Interim policy** | Authoring UI offers allowed statuses from template; Engine warns on unknown status strings. |

### GAP-009 — Deletion, supersession, move/rename

| Field | Content |
|---|---|
| **Questions (§55.19–22)** | CRUD safety. |
| **Current EDF behavior** | DIA: archive don’t delete; discovery vs normative separation; navigation update obligations described in prose. |
| **Ambiguity** | No automated link rewrite specification. |
| **Proposed EDF improvement** | Normative move/rename checklist + optional link-updater tool spec. |
| **Interim policy** | Authoring service performs move with user-approved link scan; invalidate derived caches (ADR-0004). |

### GAP-010 — Reuse of EDF validation scripts

| Field | Content |
|---|---|
| **Question (§55.23)** | Reusable scripts. |
| **Current EDF behavior** | `analyze_project_structure.sh`, `run_conformance_validation.sh`, `adopt-edf.sh validate`. |
| **Ambiguity** | Output is human text; no stable JSON protocol for embedding. |
| **Proposed EDF improvement** | `--format json` for Framework Advisor. |
| **Interim policy** | ADR-0003: invoke scripts, parse text; optional structured parser versioned with EDF tag. |

### GAP-011 — Agile representation

| Field | Content |
|---|---|
| **Questions (§55.27–28)** | Agile vs EDF. |
| **Current EDF behavior** | `tasks/` folder; no canonical Agile schema in Core. |
| **Ambiguity** | PCON-0000 optional Agile layer has no EDF canonical store. |
| **Proposed EDF improvement** | Optional capability manifest for work management artifacts. |
| **Interim policy** | Deferred past MVP; if added, non-canonical unless EDF adopts capability. |

### GAP-012 — Canonical vs cached

| Field | Content |
|---|---|
| **Question (§55.29)** | Canonical vs derived. |
| **Current EDF behavior** | DIA states Git/docs canonical; reports under `reports/` are transient evidence (Bootstrap Report template). |
| **Ambiguity** | Location for derived graph/index not specified. |
| **Interim policy** | ADR-0004: store under `.projectconcord/` or OS app data; never commit by default. |

### GAP-013 — Implementation change association

| Field | Content |
|---|---|
| **Questions (§55.30–33, 36)** | Reconciliation and Roslyn. |
| **Current EDF behavior** | Not specified in EDF Core. |
| **Ambiguity** | Entire reconciliation pipeline is product logic with EDF feedback loop. |
| **Interim policy** | Architecture overview § reconciliation; M6+ implementation; conflicts never auto-merge into canonical docs. |

### GAP-014 — Web reuse and repository abstraction

| Field | Content |
|---|---|
| **Questions (§55.34–37)** | Future web. |
| **Current EDF behavior** | N/A (product architecture). |
| **Ambiguity** | N/A |
| **Interim policy** | ADR-0001, ADR-0005; filesystem repository first. |

### GAP-015 — EDF improvements for this system

| Field | Content |
|---|---|
| **Question (§55.38)** | What must EDF improve? |
| **Summary** | Artifact type registry, adoption version pinning, structured validation output, lifecycle state machines, optional milestone/gate taxonomy, relationship metadata, JSON schemas for YAML configs. |
| **Interim policy** | Track as EDF upstream proposals after ProjectConcord M3 validates script-wrapper approach. |

### GAP-015 — Stable canonical artifact identity (SPEC-002)

| Field | Content |
|---|---|
| **Source** | [SPEC-002 §5](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) |
| **Current EDF behavior** | IDs embedded in filenames and headings by convention; no normative identity registry. |
| **Ambiguity** | Move/rename may change path while identity should persist; external tools may not preserve ID metadata. |
| **Proposed EDF improvement** | Optional `edf-id:` front matter or central artifact index schema. |
| **Interim policy** | [ADR-0007](../Architecture/ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md); derived Artifact Registry. |

### GAP-016 — Formal vs navigational links (SPEC-002)

| Field | Content |
|---|---|
| **Source** | [SPEC-002 §13–14](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) |
| **Current EDF behavior** | All relative links treated similarly by Framework Advisor (integrity only). |
| **Ambiguity** | Cannot distinguish `governed-by` from generic “see also” without inference. |
| **Proposed EDF improvement** | Relationship vocabulary + optional structured link syntax. |
| **Interim policy** | Derived Relationship Index; user confirmation for formal edges. |

### GAP-017 — CRA semantics (CRA alignment handover)

| Field | Content |
|---|---|
| **Source** | [CRA Alignment and Responsibility Boundaries](../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md) |
| **Current behavior** | CRA principles referenced in EDF; full CRA spec not bundled in EDF repo for machine import. |
| **Ambiguity** | Which CRA relationship types/cardinalities are binding for ProjectConcord vs deferred. |
| **Proposed improvement** | Published CRA normative modules linkable from EDF; optional `cra-concepts` manifest. |
| **Interim policy** | [ADR-0008](../Architecture/ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md); [CRA ↔ ProjectConcord Gap Analysis](CRA_ProjectConcord_Gap_Analysis.md). |

### GAP-018 — Operational persistence for shared projects

| Field | Content |
|---|---|
| **Source** | [AMD-0001](../Architecture/AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md) |
| **Question** | What belongs in shared operational storage vs Git canonical artifacts? |
| **Ambiguity** | EDF does not define ProjectConcord membership, notifications, or change-set tables. |
| **Interim policy** | Classify per [ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md); choose DB technology after service boundaries stabilize. |

### GAP-019 — Personas vs roles

| Field | Content |
|---|---|
| **Source** | [AMD-0002](../Architecture/AMD-0002-Single-User-Administrator-Model.md) |
| **Question** | How are UI personas (Architect view, Developer view) represented without duplicating RBAC? |
| **Interim policy** | Authorization roles in project services; personas as client presentation preferences; Administrator has all project permissions. |

### GAP-020 — Independent review requirements

| Field | Content |
|---|---|
| **Question** | When must ProjectConcord block an Administrator from self-approving an EDF gate? |
| **Interim policy** | Mirror [EGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md) and project EGR records; no silent bypass. |

### GAP-021 — Concurrent editing

| Field | Content |
|---|---|
| **Source** | [AMD-0001](../Architecture/AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md) §18–§21 |
| **Question** | How are conflicting canonical Markdown edits detected and resolved? |
| **Interim policy** | Change-set model; avoid permanent locks; detailed behavior in post-M6 specification. |

### GAP-022 — Governed canonical fields (SPEC-003)

| Field | Content |
|---|---|
| **Source** | [SPEC-003 §8–9](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) |
| **Question** | Which artifact fields are governed vs ordinary content? |
| **Current EDF behavior** | Templates and DIA imply governance; no single machine-readable governed-field manifest. |
| **Interim policy** | Heuristic list in SPEC-003; confidence flags; EDF improvement feedback. |

### GAP-023 — Lifecycle transition rules

| Field | Content |
|---|---|
| **Source** | SPEC-003 §10–11; overlaps GAP-004 |
| **Question** | Which lifecycle transitions are legal and how are they evaluated? |
| **Interim policy** | Known Status enums only; semantic transition API in ProjectConcord; external edits flagged. |

### GAP-024 — Prerequisites for acceptance / gate closure

| Field | Content |
|---|---|
| **Source** | SPEC-003; EGR-0001 patterns |
| **Question** | Are transition prerequisites machine-readable? |
| **Interim policy** | EGR records human-driven; app surfaces independent-review requirements ([ADR-0010](../Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)). |

### GAP-025 — Trusted integrity metadata

| Field | Content |
|---|---|
| **Source** | SPEC-003 §19–20; [ADR-0011](../Architecture/ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md) |
| **Question** | What integrity/trust state must travel with a repo clone vs remain operational? |
| **Interim policy** | Rebuild trust from Git + deterministic validation; optional `.projectconcord/` or shared store caches fingerprints. |

### GAP-026 — Architectural Audit Record discovery

| Field | Content |
|---|---|
| **Source** | EDF [AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md); [ADR-0012](../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md) |
| **Question** | How does the app list Open vs Complete implementation audits? |
| **Interim policy** | Human-maintained [Audits/README.md](../Architecture/Audits/README.md); Engine discovery post-M5. |

### GAP-040 — GDO dependency evaluation and EGR round-trip

| Field | Content |
|---|---|
| **Source** | EDF [EGR-0001 v1.1](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md); [GDO handover](../Handover/EDF-Governed-Dependency-Override-Architecture-Handover.md) |
| **Question** | How does ProjectConcord evaluate Blocking vs Non-Blocking prerequisites, persist GDO tables on EGR Markdown, surface governance debt, and validate downstream scope? |
| **Interim policy** | Instance EGRs include EDF-shaped GDO sections; semantics per EDF + handover; Engine/UI implementation deferred per [Implementation Roadmap](Implementation_Roadmap.md) (M5+ display, M6+ evaluation). |

### GAP-041 — GMFP / GMR consumption and workflow-profile representation

| Field | Content |
|---|---|
| **Source** | EDF commit `b158f4a382dfbea941435eeacd96beb729443687`; [GMFP-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Specifications/GMFP-0001-Governed-Maintenance-Fast-Path.md); [EDF ADR-0009 (GMFP)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/b158f4a382dfbea941435eeacd96beb729443687/docs/Architecture/ADRs/ADR-0009-Governed-Maintenance-Fast-Path.md); [GMFP handover](../Handover/EDF-Governed-Maintenance-Fast-Path-Architecture-Handover.md); [AWI-0004](../Architecture/Watch_Items/AWI-0004-Governed-Maintenance-Fast-Path.md) |
| **Question** | How does ProjectConcord register and run **GMFP** as a **separate prescribed workflow**, discover **GMR** instances, project Gate 1 authorization into operational **DevelopmentWorkAuthorization** (or successor) without embedding DWA IDs in GMR, visualize **two human gates** and **GMFP-2 execution interval**, present consolidated evidence, handle **Escalated** / STOP, and distinguish GMFP from EGR/GDO/AAR — without hard-coding provisional enums or redefining EDF GMR states? |
| **Interim policy** | EDF GMR remains canonical in Git; GMFP is **not** a GEW profile ([ADR-0024](../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) **Accepted** 2026-10-03); framework primitives per [PCON-0005](../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) **Accepted** 2026-10-03; no parser, validator, workflow engine, GMFP UI, or schema in current tranche. |

### GAP-042 — MVR / MVT consumption, human attestation, pending manual QA

| Field | Content |
|---|---|
| **Source** | EDF commit `192fe5c1c6254c51e257d24aefc09e127ce72464`; [MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md); [EDF ADR-0010 (MVR)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Architecture/ADRs/ADR-0010-Manual-Verification-Records.md); [MVR handover](../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md); [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md); [ADR-0014](../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md); [AWI-0005](../Architecture/Watch_Items/AWI-0005-Manual-Verification-Records.md) |
| **Question** | How does ProjectConcord discover MVR candidates, validate canonical Record ID, parse MVT Results and Human execution status deterministically, maintain rebuildable non-authoritative projections, surface pending manual QA, perform human-attestation write-back with representational checkbox sync, enforce the Human-Attestation Authority Boundary, and show read-only gate blocking hints — without inferring optional MVT semantics, silently repairing identity, auto-mutating EGR, or treating GDO/GMFP validation as human MVR execution? |
| **Interim policy** | Normative behavior in [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md); S0 docs + `docs/Verification/` bootstrap complete; **no application implementation** until M2+ authorized under [Implementation Roadmap](Implementation_Roadmap.md) and EGR-G1/milestone sequence. |

### GAP-043 — PAR runtime and per-user operational persistence

| Field | Content |
|---|---|
| **Question** | How does ProjectConcord assign stable Project IDs, persist per-user SQLite state (recent roots, session provenance, workflow partitions), implement PAR package validation (including INCOMPLETE handover rules), and Tier 0 awareness without creating `.projectconcord/` on open or depending on ChatGPT/API? |
| **Interim policy** | Architecture in [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) (including §19 applicability / PC-PAR-025 — [recovery binding tranche](../Handover/ProjectConcord-Durable-Governed-Work-State-Recovery-Binding-Tranche-Plan.md) **published** 2026-10-03), [ADR-0015](../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md), [ADR-0020](../Architecture/ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) §15, [PAR plan](../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md); **A1 closed**; **A2-T7 closed** (Desktop P0 relay UI — [T7 notes](../Handover/ProjectConcord-A2-T7-Implementation-Notes.md)); remaining PAR workflow UI for separately authorized A3; MVR A2-T8. |

### GAP-030 — Engineering Agent provider adapter and plugin boundary

| Field | Content |
|---|---|
| **Identified** | 2026-09-30 — A2-T7 Project Architect closeout |
| **Question** | How does ProjectConcord support multiple concrete Engineering Agent products without embedding product-specific integration in Core or the Software Engineering Extension? |
| **Architectural requirement (binding direction, not implemented)** | (1) Multiple concrete Engineering Agent products must be supportable. (2) **Core** remains provider-neutral. (3) **Software Engineering Extension** remains provider-neutral (generic Engineering Agent concepts only — not proprietary APIs, transport, auth, or IDE coupling). (4) Concrete integrations live in **separately bounded Engineering Agent plugins** (product-specific names only inside a plugin and its tooling/docs). (5) **Provider-neutral Engineering Agent plugin contract** — **Accepted** [ADR-0021](../Architecture/ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) (2026-10-01); distinct from relay render/parse (`IEngineeringAgentRelayBridge`). (6) **Generic** plugin hosting ownership — **application/infrastructure** boundary defined in ADR-0021; **physical** loader/discovery/sandboxing **deferred**. (7) Physical plugin architecture and discovery/loading/security/versioning/isolation remain **deferred** for implementation. (8) **A4** architectural reconciliation — **documentation complete** per ADR-0021; **A4 implementation** remains **not authorized**. (9) **No** plugin or loader **implementation** authorized by this record or ADR-0021. |
| **Resolution** | **Closed / resolved** (2026-10-01) by [ADR-0021](../Architecture/ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) — provider-neutral Engineering Agent **plugin architectural boundary and contract** (layering, relay bridge vs provider plugin split, capabilities, automated transport boundary, hosting boundary in application/infrastructure). |
| **Closure does not authorize** | A4 implementation; provider plugin implementation; plugin-host/loader implementation; provider SDK/API integration. |
| **Closure does not imply** | A4 implementation; concrete provider plugins; plugin-host/loader implementation. [GAP-044](#gap-044--engineering-agent-automated-transport) **closed / resolved** by [ADR-0022](../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) (**Accepted** 2026-10-01). [AWI-0006](../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md) **closed / satisfied** (2026-10-01). |
| **Deferred separately (not GAP-030)** | Physical plugin loader, concrete provider plugins, dynamic discovery, sandboxing, supply-chain controls; optional relocation of P0 manual adapter; ADR-0020 shell projection implementation for transport failures; A4 and related implementation tranches — each requires separate PA authorization. |
| **Interim policy (unchanged)** | A2 P0 manual relay **published**; `EngineeringAgentManualRelayBridge` interim reference adapter; `IEngineeringAgentRelayBridge` render/parse; automated transport only via future bounded plugins per ADR-0021 — see [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §11. |

### GAP-044 — Engineering Agent automated transport

| Field | Content |
|---|---|
| **Question** | What **automated transport architecture** satisfies security, mode routing, and evidence return for **automated** Engineering Agent relay **without** redefining governance semantics? |
| **Distinction** | **[GAP-030](#gap-030--engineering-agent-provider-adapter-and-plugin-boundary)** = provider/plugin **placement** — **resolved** ([ADR-0021](../Architecture/ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md)). **GAP-044** = provider-neutral **automated transport capability**, orchestration, and operation lifecycle — **resolved** by [ADR-0022](../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md). |
| **Resolution** | **Closed / resolved** (2026-10-01) — [ADR-0022](../Architecture/ADRs/ADR-0022-Engineering-Agent-Automated-Transport-Architecture.md) (**Accepted**). Provider-neutral automated transport capability; application/infrastructure orchestration and transport-operation records; concrete provider realizations behind ADR-0021. |
| **Closure does not authorize** | A4 implementation; A4 implementation plan; provider plugin implementation; plugin-host/loader implementation; transport ledger or Cursor ACP/CLI/MCP **implementation**. |
| **Related** | [AWI-0006](../Architecture/Watch_Items/AWI-0006-PAR-Cursor-Bridge-Transport.md) **closed / satisfied** (same publication gate); [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) PC-PAR-022c–022d. |

### GAP-045 — Third-party NuGet dependency advisories (SQLite transitive)

| Field | Content |
|---|---|
| **Source** | A1b `dotnet build -c Release` / `dotnet list package --vulnerable` on `Edf.ProjectServices` |
| **Question** | When and how should ProjectConcord remediate or accept transitive NuGet security advisories on infrastructure packages without expanding unrelated tranches? |
| **Interim policy** | Recorded in [AWI-0007](../Architecture/Watch_Items/AWI-0007-SQLite-Transitive-NuGet-Advisory.md): `NU1903`, `SQLitePCLRaw.lib.e_sqlite3` 2.1.10, [GHSA-2m69-gcr7-jv3q](https://github.com/advisories/GHSA-2m69-gcr7-jv3q), via `Microsoft.Data.Sqlite` 9.0.3; monitor upstream; no A1b package change unless PA/security gate requires. |

### GAP-047 — Terminology projection and optional enforcement

| Field | Content |
|---|---|
| **Identified** | 2026-10-01 — AWI-0008 terminology reconciliation and canonical documentation tranche |
| **Question** | How does ProjectConcord implement generic terminology projection (EDF glossary term reference + PC policy overlay), derived search aliases, and optional future enforcement — without duplicating EDF, hard-coding AI/SI, or rewriting historical records? |
| **Resolved dependency** | EDF [TGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/49d6b805af5f4a557c526a5c8f08e43fbab38601/docs/Specifications/TGR-0001-Terminology-Governance.md) pins in [EDF Terminology handover](../Handover/EDF-Terminology-Governance-Architecture-Handover.md); PC **Preferred** SI policy in [ADR-0018](../Architecture/ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) |
| **Interim policy** | Architecture **Accepted**; `src/` projection, UI labels, search index, lint/CI enforcement, and living-document SI migration **not authorized** |

### GAP-032 — Cross-project relationships, launch, and inter-project continuity

| Field | Content |
|---|---|
| **Identified** | 2026-10-01 — AWI-0008 persistence/sync documentation tranche (reframe of prior “multi-project workspace” gap) |
| **Question** | How does ProjectConcord support Project selection/launch, **Single-Project Focus** per instance, **Concurrent Project Work** across instances, and governed **inter-project** dependencies/handovers without a multi-project workspace shell? |
| **Interim policy** | Stable Project ID and locator model **Accepted** ([ADR-0015](../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md)); instance and continuity model **Accepted** ([ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) 2026-10-01); IPH/CPD operational schemas remain [GAP-033](#gap-033--interprojecthandover-operational-schema) / [GAP-034](#gap-034--crossprojectdependency-and-source-notification); product UX for launcher/multi-instance launch **not implemented** |

### GAP-048 — Same Project ID — multiple local application processes

| Field | Content |
|---|---|
| **Identified** | 2026-10-01 — AWI-0008 persistence architecture investigation |
| **Question** | When multiple ProjectConcord application processes belonging to the **same installation/user** open the **same ProjectConcord Project ID** concurrently, what are the expected behavior, coordination, warnings, read/write semantics, and interaction with future synchronization? |
| **Interim policy** | **OPEN** — no architectural prohibition on future multi-client collaborative Project access; local policy **not selected** in [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); relates to [GAP-052](#gap-052--multi-process-local-operational-store-concurrency-strategy-and-validation) |

### GAP-049 — Operational synchronization architecture

| Field | Content |
|---|---|
| **Identified** | 2026-10-01 — AWI-0008 persistence/sync documentation tranche |
| **Question** | How does ProjectConcord implement client-side synchronization (automatic routine connected sync vs reconnection reconciliation), Cloud Service API contracts, and operational-state replication without direct desktop-to-cloud-database access or conflating canonical Git sync? |
| **Interim policy** | Architecture direction **Accepted** in [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); **no implementation** authorized |

### GAP-050 — Resource-level operational concurrency policy and version model

| Field | Content |
|---|---|
| **Identified** | 2026-10-01 — AWI-0008 persistence/sync documentation tranche |
| **Question** | How are synchronizable operational resource categories assigned concurrency semantics (version-checked, append-only, governed merge, exclusivity, human resolution) and how is versioned optimistic concurrency represented at the service boundary? |
| **Interim policy** | Default direction **Accepted** in [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md); relay/operational schema versioning **not implemented** beyond published A2 subset |

### GAP-051 — Application-layer persistence / service-port isolation

| Field | Content |
|---|---|
| **Identified** | 2026-10-01 — AWI-0008 persistence architecture investigation |
| **Question** | How should `Edf.Application` depend only on persistence **contracts** with SQLite construction confined to composition/`Edf.ProjectServices`? |
| **Interim policy** | Documented as architectural debt in [ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) §4; **no refactor** until separately authorized |

### GAP-052 — Multi-process local operational-store concurrency strategy and validation

| Field | Content |
|---|---|
| **Identified** | 2026-10-01 — AWI-0008 persistence architecture investigation |
| **Question** | What are ProjectConcord’s governed operational semantics when multiple application processes share the per-user local operational store (transaction/contention behavior, process coordination, shared preference semantics, journaling configuration, same vs different Project access, validation strategy)? |
| **Interim policy** | ProjectConcord has **not** yet explicitly designed, validated, or governed this behavior ([ADR-0019](../Architecture/ADRs/ADR-0019-Local-First-Operational-Persistence-Service-Boundary-Synchronization-and-Concurrency.md) §13). SQLite provides inter-process locking and transactional behavior; **prescriptive** WAL/per-instance DB/process-lock architecture **not** selected in this tranche |

### GAP-053 — macOS Go to Folder project locator vs browse-open identity

| Field | Content |
|---|---|
| **Identified** | 2026-10-02 — A4-T7 blocker-independent human MVR setup ([MVR-0003](../Verification/Records/MVR-0003-a4-engineering-agent-automated-transport.md); [A4-T7 §O](../Handover/ProjectConcord-A4-T7-Verification-Evidence.md#o-macos-go-to-folder-locator-defect-observation-2026-10-02)) |
| **Observed defect** | macOS folder chooser **Go to Folder…** with absolute path `/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A` caused ProjectConcord to display/open Project ID **`ddde280f-38ad-4bfb-abb9-86be2c939d15`**. Normal folder **browse** selection of the same Root-A opened canonical disposable ID **`a5e26be6-9769-46f6-bd94-1669764fe8af`**. Canonical Root-A Project ID on disk **unchanged**. |
| **MVR impact** | Did **not** invalidate completed blocker-independent human MVTs (verifier used browse workflow). |
| **Distinction** | **Not** the external **Cursor ACP Composer 2.5 non-fast** capability blocker ([A4-T7 §E2](../Handover/ProjectConcord-A4-T7-Verification-Evidence.md#e2-cursor-acp-provider-capability-blocker-2026-10-02)). |
| **Question** | How should ProjectConcord normalize or reconcile locator paths from **Go to Folder…** vs hierarchical browse so the same physical Project Root resolves to a single stable Project ID ([ADR-0015](../Architecture/ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md); relates to [GAP-043](#gap-043--par-runtime-and-per-user-operational-persistence), [MVR-0001](../Verification/Records/MVR-0001-a1c-desktop-project-root-recent-workflow.md) Go to Folder cautions)? |
| **Interim policy** | **OPEN** — defect **recorded**; **no** root-cause investigation or `src/` remediation in A4-T7 closeout tranche; workaround: use normal browse for disposable Root-A until separately authorized fix. |

### GAP-027 — DevelopmentWorkAuthorization schema and identity

| Field | Content |
|---|---|
| **Source** | [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md); [ADR-0013](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md); [ADR-0024](../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03) |
| **Question** | How does operational **DevelopmentWorkAuthorization** correlate **PrescribedWorkflowId**, **ProfileId**, **WorkflowInstanceId**, bounded configuration, and effective-configuration snapshots without duplicating authority semantics or embedding workflow state unrelated to execution authorization? |
| **Interim policy** | Architecture direction in ADR-0024 **Accepted** 2026-10-03; SPEC-004 (Draft, GEW reconciliation **published** 2026-10-03) documents conceptual DWA ↔ workflow/profile/instance correlation; schema and APIs **not implemented** until M7+ / separately governed tranche. |

### GAP-054 — Workflow framework runtime and effective configuration

| Field | Content |
|---|---|
| **Source** | [PCON-0005](../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) (**Accepted** 2026-10-03); [ADR-0024](../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) (**Accepted** 2026-10-03); [AWI-0010](../Architecture/Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md) |
| **Question** | How does ProjectConcord implement prescribed-workflow **registry**, **definition versioning**, **effective-configuration resolver** (invariant + bounded + policy-derived), invalid-configuration rejection, and workflow/profile **resolution** at relay and operator-projection boundaries without a generic BPM engine? |
| **Interim policy** | SPEC-004 GEW reconciliation **published** 2026-10-03; [PCON-0005](../Architecture/PCON-0005-ProjectConcord-Workflow-Framework.md) **FW-8** + §2.7 and [ADR-0024](../Architecture/ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md) §9–§13 define **GEW v1** topology places, traversal (`TopologyPlaceId`, `TraversalOccurrenceId`), and orthogonal STOP/sync/AEI separation ([M7a-WF-1-doc tranche](../Handover/ProjectConcord-M7a-WF-1-doc-GEW-Topology-Documentation-Tranche-Plan.md) — **published**); **M7a-WF-1** delivers local **WorkflowInstance** persistence (Migration004), static GEW v1 registry, governed transition provenance boundary, recovery, and backend operator projection — **not** effective-configuration resolver, PC-PAR-025 enforcement, STOP/sync/AEI instance persistence, or production governed-advancement callers; **no** A3 authorization. |

### GAP-055 — Governed Interactive Engineering Session runtime and persistence

| Field | Content |
|---|---|
| **Source** | [PCON-0006](../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md) (**Accepted** 2026-10-03); [ADR-0025](../Architecture/ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) (**Accepted** 2026-10-03); [AWI-0011](../Architecture/Watch_Items/AWI-0011-Governed-Interactive-Engineering-Session.md) |
| **Question** | How does ProjectConcord implement **Interaction Hold**, context/set/item lifecycle, policy-governed **routing**, pre-resolution against authoritative state, operational **provenance**, recovery after app/provider interruption, and integration with relay STOP/Attention — without a chat platform or forms engine? |
| **Interim policy** | GIES architecture and SPEC-004 GEW consumption **published** 2026-10-03 ([PCON-0006](../Architecture/PCON-0006-Governed-Interactive-Engineering-Session-Framework.md), [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md), [tranche plan](../Handover/ProjectConcord-SPEC-004-GIES-Consumption-Reconciliation-Tranche-Plan.md)); SPEC-006 GIES interaction relay semantic contract **published** 2026-10-03 ([SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §18, [relay tranche plan](../Handover/ProjectConcord-SPEC-006-GIES-Interaction-Relay-Reconciliation-Tranche-Plan.md)); runtime/persistence and `projectconcord-relay-v1` GIES field schemas **not implemented**; **no** `src/` implementation; relates to [GAP-027](#gap-027--developmentworkauthorization-schema-and-identity), [GAP-054](#gap-054--workflow-framework-runtime-and-effective-configuration), [GAP-043](#gap-043--par-runtime-project-id-registry-per-user-sqlite-package-validation), [AWI-0002](../Architecture/Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md). |

---

## Parent

- [Development](README.md)

## Related Documents

- [CRA Alignment and Responsibility Boundaries](../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md)
- [SPEC-002 — Referential Integrity](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)
- [PCON-0000 — Architectural Vision](../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)
- [SPEC-003 — Canonical Artifact Integrity](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)
- [Architecture Audits](../Architecture/Audits/README.md)
- [ADR-0012](../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md)
- [Canonical Integrity Integration Analysis](../Architecture/Canonical_Integrity_Spec_Integration_Analysis.md)
- [Multi-User Amendment Analysis](../Architecture/Multi_User_Amendment_Affected_Document_Analysis.md)
- [System Architecture Overview](../Architecture/System_Architecture_Overview.md)
- [Implementation Roadmap](Implementation_Roadmap.md)
- [PAR Workflow Architecture Plan](../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md)
- [SPEC-006](../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [EDF Governed Dependency Override — Architecture Handover](../Handover/EDF-Governed-Dependency-Override-Architecture-Handover.md)
- [EDF Governed Maintenance Fast Path — Architecture Handover](../Handover/EDF-Governed-Maintenance-Fast-Path-Architecture-Handover.md)
- [EDF Documentation Information Architecture](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/Documentation_Information_Architecture.md)
