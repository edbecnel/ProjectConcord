[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Development](README.md) › EDF Gap Register

# EDF Gap Register

> **Status:** Draft  
> **Owner:** ProjectConcord  
> **Applies To:** Deterministic EDF Engine design  
> **Last Reviewed:** 2026-09-15  
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
| GAP-006 | Milestones and gates representation | Medium | **Partial:** EDF [EGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md); ProjectConcord [EGR-G0/G1](../../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md); app UI for open gates deferred (M4+) |
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
| GAP-018 | Operational store schema for membership/audit/change sets | Medium | [ADR-0009](../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md); technology TBD; not canonical EDF |
| GAP-019 | Persona vs authorization role taxonomy | Medium | [ADR-0010](../Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md); UI personas documented separately from RBAC |
| GAP-020 | Independent EDF/EGR review vs Administrator UX | Medium | Never bypass EDF-prescribed independent review; surface in gate UI |
| GAP-021 | Concurrent canonical edit / change-set protocol | High | Optimistic concurrency + change sets; defer implementation to M6+ spec |

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

---

## Parent

- [Development](README.md)

## Related Documents

- [CRA Alignment and Responsibility Boundaries](../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md)
- [SPEC-002 — Referential Integrity](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)
- [PCON-0000 — Architectural Vision](../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)
- [AMD-0001 — Multi-User Amendment](../Architecture/AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md)
- [Multi-User Amendment Analysis](../Architecture/Multi_User_Amendment_Affected_Document_Analysis.md)
- [System Architecture Overview](../Architecture/System_Architecture_Overview.md)
- [Implementation Roadmap](Implementation_Roadmap.md)
- [EDF Documentation Information Architecture](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/Documentation_Information_Architecture.md)
