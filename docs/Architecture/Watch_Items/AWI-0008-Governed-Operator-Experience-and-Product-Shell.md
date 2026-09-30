# AWI-0008 — Governed Operator Experience and Product Shell

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0008

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-30 |
| **Revisit Trigger** | Before post-A2 product-shell or governed-workflow UX tranche authorization; PA request to analyze ownership |
| **Discovery source** | [MVR-0002](../../Verification/Records/MVR-0002-a2-p0-manual-governed-relay-workflow.md) human execution (2026-09-30); A2-T8 documentation closeout — **not** A2 implementation defects |
| **Related ADRs** | [ADR-0016](../ADRs/ADR-0016-Core-Domain-Extension-and-Working-Environment-Boundary.md); [ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) (Proposed) |
| **Related specs** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md); [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md) |
| **Cross-reference** | [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md); [AWI-0005](AWI-0005-Manual-Verification-Records.md); [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md) |

---

## Objective

Capture **follow-on product and operator-experience requirements** identified during A2 P0 manual governed relay verification. These items guide future architectural analysis and implementation tranches. They **do not** retroactively classify A2-T7/T8 delivery as defective.

## Scope and non-goals

This watch item:

- **Does not** authorize `src/` changes, UI redesign, persistence changes, Markdown rendering, form runtime, or plugin work.
- **Does not** decide whether capabilities belong to Core, a domain extension, EDF integration, or Software Engineering extension — see **Architectural ownership (deferred)** below.
- **Does not** authorize storing full external chat/provider transcripts as canonical ProjectConcord content.
- **Does not** commit ProjectConcord to a general-purpose Markdown editor without separate architectural analysis.

While **Active**, requirements here are **prospective** unless promoted by PA into specifications or ADRs.

## Requirements (PA disposition — A2-T8 closeout 2026-09-30)

### A. Governed workflow orientation

The user must be able to understand:

- where they currently are in a governed workflow;
- what has already been completed;
- what is currently awaiting action;
- what action is available next;
- what is blocking progression.

The production UI must not require the user to infer workflow state from a collection of controls or raw relay data alone.

Workflow stages must not be assumed to be universally hard-coded Core semantics. Their architectural ownership requires analysis.

### B. Structured workflow navigation

The P0 long vertically scrollable relay panel is **not** an acceptable target production UX.

Future architecture should evaluate wizard-style workflow navigation, tabbed/workspace navigation, or a combination. The UI should guide the user through governed work rather than expose the entire workflow as one long form.

### C. Durable governed relay/package working state

Useful governed handover/package working material should survive application close/reopen where appropriate.

Future architecture must distinguish at least:

- canonical governed artifacts;
- persisted operational relay/package state;
- provenance;
- ephemeral UI state;
- external chat/provider conversation content.

This requirement does **not** authorize storing full external chat transcripts as canonical ProjectConcord content.

### D. Markdown rendered / source view

ProjectConcord needs native Markdown viewing. For applicable Markdown/governed content, the UI should support a clear user-facing toggle such as **Rendered | Source**. Rendered Markdown should be available without requiring an external Markdown application. This does **not** yet authorize a general Markdown editor.

### E. Governed form runtime

Future architecture should evaluate a reusable runtime-generated Avalonia form capability for governed structured user input.

MVR execution is an initial motivating use case. Instead of requiring manual editing of MVR Markdown tables, ProjectConcord could present purpose-built controls (for example Result: Pending / Pass / Fail / Blocked; Executor; Date; Test data / fixture; Evidence) and write governed results to canonical Markdown.

Initial forms should be purpose-built / ProjectConcord-defined. Do **not** assume arbitrary user-defined form design in the initial architecture. Potential future extension-defined forms may be analyzed separately.

### F. Future project-aware constrained Markdown editing

Retain as a future architectural possibility: **project-aware constrained Markdown editing**. For recognized governed artifacts, editing could understand artifact type, permitted values, known actors, references, validation constraints, and governed state transitions.

Do not commit ProjectConcord to becoming a general-purpose Markdown editor without architectural analysis.

### G. Canonicality invariant

**Canonical Markdown remains primary.** Rendered views, structured forms, and constrained editing interfaces must be projections over canonical governed artifacts and must not silently create competing sources of truth.

### H. User-facing terminology

Avoid unnecessary acronyms in the ProjectConcord user interface. In particular, display **Project Architect** rather than **PA** where the term is presented to the user. Repository and internal architectural documentation may retain established acronyms where appropriate and unambiguous.

## Architectural ownership (deferred)

Do **not** use this watch item to decide:

- whether generic Markdown viewing belongs to Core or product shell;
- whether governed form runtime belongs to Core;
- whether EDF-specific forms belong to EDF integration;
- whether Software Engineering-specific forms belong to that extension;
- whether constrained Markdown editing belongs to Core or an extension;
- whether workflow navigation is generic or extension-owned.

Subsequent PA-governed analysis is required before implementation authorization.

## Signals to watch

- Workflow stage semantics embedded in a single extension without Core boundary review.
- Operational relay paste buffers treated as canonical artifacts.
- MVR or gate state mutated without human-attestation boundary ([ADR-0014](../ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)).
- User-facing shorthand (PA, EA, PAR) proliferating in Desktop without glossary or expansion.

## Parent

- [Watch Items](README.md)

## Related Documents

- [A2 Implementation Plan](../../Handover/ProjectConcord-A2-Implementation-Plan.md) §35–§36
- [A2-T8 verification evidence](../../Handover/ProjectConcord-A2-T8-Verification-Evidence.md)
- [MVR-0002](../../Verification/Records/MVR-0002-a2-p0-manual-governed-relay-workflow.md)
- [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) / [AWI-0006](AWI-0006-PAR-Cursor-Bridge-Transport.md) — Engineering Agent plugin boundary unchanged
