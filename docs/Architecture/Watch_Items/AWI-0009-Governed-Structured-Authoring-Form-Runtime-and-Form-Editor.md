# AWI-0009 — Governed Structured Authoring, Form Runtime, and Form Editor

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0009

| | |
|---|---|
| **Status** | Active — **deferred**, **capture-only** |
| **Disposition** | **NOT architectural acceptance** — **NOT implementation authorization** |
| **Owner** | ProjectConcord |
| **Created** | 2026-10-01 |
| **Revisit Trigger** | Before any Form Runtime, form-definition format, or Form Editor architecture tranche; PA request to promote to ADR/specification |
| **Discovery source** | AWI-0008 Operator Workflow + Workspace/Navigation investigation (2026-10-01); PA disposition — structured-authoring ownership consolidated here |
| **Related ADRs** | [ADR-0004](../ADRs/ADR-0004-Derived-Data-and-Cache.md) (canonical vs derived); [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) (**Proposed** — attestation boundary direction only, not acceptance); [ADR-0018](../ADRs/ADR-0018-Adopter-Terminology-Policy-and-Projection-Architecture.md) |
| **Related specs** | [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md) (Draft — MVR motivating use case); [SPEC-002](../../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md) (structured authoring direction) |
| **Cross-reference** | [AWI-0008](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) (product shell artifact detail host — **Form \| Rendered \| Source** accommodation only); [ADR-0020](../ADRs/ADR-0020-Operator-Projections-Product-Shell-and-Workspace-Navigation.md) (**Accepted** §12); [AWI-0005](AWI-0005-Manual-Verification-Records.md) |

---

## Objective

Capture **deferred architectural requirements** for governed **structured authoring**: purpose-built forms over canonical Markdown artifacts, a shared **Form Runtime**, companion **form definitions**, and a future **Form Editor** — without selecting definitive formats, without architectural acceptance, and without authorizing implementation.

This watch item is the **single** ProjectConcord track for structured-authoring architecture (not a parallel track in AWI-0008).

## Scope and non-goals

This watch item:

- **Does not** authorize `src/` changes, Form Runtime implementation, form-definition schema selection, Form Editor implementation, or AI/SI document generation pipelines.
- **Does not** create an ADR, specification, form schema, or implementation plan.
- **Does not** decide whether specific forms belong to Core, a domain extension, or EDF integration — ownership analysis remains for a future PA-governed tranche.
- **Does not** replace EDF artifact semantics; ProjectConcord consumes/projects EDF governance.

While **Active** and **capture-only**, requirements here are **prospective** unless promoted by PA into specifications or ADRs.

## Product shell integration (owned by AWI-0008)

The Core product shell **artifact detail host** must accommodate future **Form \| Rendered \| Source** interaction without assuming that manual Markdown source editing is the primary human interaction. Shell navigation and progressive disclosure are defined under [AWI-0008](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md). Form definitions, runtime, round-trip semantics, external editing, Form Editor, and AI/SI interoperability belong **here** (AWI-0009).

## Requirements (capture-only — PA handover 2026-10-01)

### 1. Human-friendly authoring

Users should not be required to manually create or correctly format governed Markdown documentation for routine governed input. Purpose-specific structured forms should provide the normal human authoring experience.

### 2. Canonicality

The persisted Markdown `.md` artifact remains the **canonical document**. The form is a **projection/editor** over the Markdown artifact. Form/UI state must not become a competing source of truth.

### 3. Document-oriented forms

Form presentation should, where practical, closely resemble the structure and presentation of the Markdown document itself. Editable regions are represented using controlled interactive inputs.

### 4. Companion definition (deferred format)

A companion **declarative definition** should describe how the canonical Markdown artifact is presented and safely edited. The exact name and serialization format are **deferred**.

The definition may eventually describe concepts such as:

- field/document bindings;
- text input;
- multiline input;
- checkbox;
- radio group;
- combo box / governed enumeration;
- date/time;
- number;
- known actor;
- artifact reference;
- evidence/file reference;
- repeatable values;
- required/optional state;
- allowed values;
- validation;
- read-only/instructional content;
- presentation hints.

Do **not** select the definitive definition format in this capture-only action.

### 5. Constrained editing

Form mode is **not** a free-form WYSIWYG Markdown editor. Users may modify only the fields/actions permitted by the applicable form definition. Governed document structure is not arbitrarily editable through Form mode.

### 6. Bidirectional operation

The architecture must eventually support:

- **Form → canonical Markdown**
- **canonical/known Markdown → populated Form**

A canonical/known supplied Markdown file should therefore be usable to populate the corresponding controls and values.

### 7. External editing

Users remain free to modify canonical Markdown using external Markdown tools such as Obsidian. ProjectConcord must eventually **detect/validate** externally modified documents rather than silently assuming conformity. Unknown or externally added content must not be silently destroyed by Form editing. Exact round-trip preservation mechanics remain **deferred**.

### 8. Built-in forms

Initial ProjectConcord forms should be purpose-built for known governed artifact types. Where practicable, built-in forms should use the same underlying form-definition/runtime mechanism intended for later custom forms. Avoid creating a disposable hard-coded form architecture that must later be replaced by a separate custom-form system.

### 9. Future Form Editor

A future visual **Form Editor** should allow form definitions to be created and modified without requiring application source-code changes. This should eventually support user-created custom forms. The Form Editor is **future architecture/work** — not architected or implemented in this tranche.

### 10. AI / SI interoperability

AI/SI systems should be able to produce **candidate Markdown** conforming to a known artifact contract. ProjectConcord should process that Markdown through the same deterministic parsing, validation, and form-population path used for other Markdown producers. There should **not** be a special AI-only document format.

Conceptual path:

```text
Human Form
External Markdown Editor
Imported Markdown
AI/SI-generated Markdown
        ↓
common governed Markdown representation
        ↓
ProjectConcord parser / validator
        ↓
structured form projection
```

AI/SI output is not authoritative merely because it is syntactically valid. Applicable validation, review, acceptance, authorization, and governance still apply. ProjectConcord remains authoritative for deterministic conformance validation. Future AI/SI assistance in generating candidate custom form definitions may be investigated later.

### 11. Separation of concerns

Preserve the conceptual distinction among:

- canonical Markdown artifact;
- artifact/document contract;
- form definition;
- form runtime;
- form editor.

Do not prematurely collapse these into one artifact or subsystem.

## Architectural ownership (deferred)

Placement among Core product shell, domain extensions, EDF integration, and presentation layers requires PA-governed analysis in a future tranche. MVR attestation and human-interaction boundaries remain governed by [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) (**Proposed**) and [AWI-0005](AWI-0005-Manual-Verification-Records.md) when promoted.

## Signals to watch

- Form control state or SQLite/UI caches treated as canonical artifact source.
- Free-form WYSIWYG Markdown marketed as “governed form mode.”
- AI-generated Markdown bypassing the same validator/parser path as human or external-editor input.
- Duplicate structured-authoring requirements reintroduced under AWI-0008 without cross-reference here.

## Parent

- [Watch Items](README.md)

## Related Documents

- [AWI-0008 — Governed Operator Experience and Product Shell](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
- [AWI-0005 — Manual Verification Records](AWI-0005-Manual-Verification-Records.md)
- [SPEC-005 — Manual Verification Record consumption](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
- [PCON-0000 § structured authoring](../../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)
