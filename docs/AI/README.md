# AI

> **Documentation path:** [Project Index](../../PROJECT_INDEX.md) → AI

## Purpose

AI-assisted engineering practices, governed development workflow (Architect AI ↔ repository agents), tool roles, prompting, verification, security, and governance boundaries.

## Authoritative Documents

- [ADR-0006 — AI Boundary](../Architecture/ADRs/ADR-0006-AI-Boundary.md) — proposals only; human approval for canonical EDF writes
- [SPEC-004 — AI-Assisted Development Governance Workflow](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) — **Draft; not implemented** (PC-AIGOV-001–028)
- [ADR-0013 — Governed Development Workflow and Workspace Model](../Architecture/ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) — **Proposed**

## Architectural discovery and analysis

- [PCON-0001 — AI-Assisted Architectural Governance and Repository Execution Workflow](../Architecture/PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) — Snaptara-derived workflow narrative (Proposed)
- [AI Governance Workflow Integration Analysis](../Architecture/AI_Governance_Workflow_Integration_Analysis.md)
- [PCON-0000 §48 — Transitional Cursor handover](../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) — lighter-weight reconciliation handover example

## Governed development cycle (conceptual)

1. Gather canonical project state (EDF, gates, baseline, defects).
2. Issue **handover** + **DevelopmentWorkAuthorization** (planning or implementation).
3. Repository agent inspects or executes per authorization; returns plan or submission.
4. Architectural review; human disposition; separate implementation authorization when applicable.
5. Evidence and validation with provenance; explicit STOP when required.
6. Cross-project discoveries route via **InterProjectHandover**; destination project governs acceptance (see PCON-0001 §4G–4K).

Manual copy/paste to Cursor or other agents is a first-class adapter ([SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) PC-AIGOV-006).

## What Belongs Here

Handbooks, prompting guides, and AI governance process docs whose primary responsibility is AI-assisted engineering — not normative EDF artifact rules (those remain under Specifications and Architecture).

## Navigation

- [Project Index](../../PROJECT_INDEX.md)
- [Project README](../../README.md)

## Maintenance

Update this index when SPEC-004, ADR-0013, or major AI governance documents change status.
