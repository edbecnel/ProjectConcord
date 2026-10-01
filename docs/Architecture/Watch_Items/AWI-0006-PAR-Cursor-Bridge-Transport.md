# AWI-0006-PAR-Cursor-Bridge-Transport

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0006

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-28 |
| **Revisit Trigger** | Before A4 implementation authorization or PA request to evaluate P1 transport |
| **Discovery source** | [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md); [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) |
| **Related ADRs** | [ADR-0015](../ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Accepted** 2026-09-29); [ADR-0021](../ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) (**Accepted** 2026-10-01) |
| **Related specs** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) (**Accepted** 2026-09-29) |
| **Cross-reference** | [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md) — broader orchestration; AWI-0006 scopes **Engineering Agent automated transport** investigation (reference P1 candidate: Cursor) |

---

## Objective

Investigate **automated Engineering Agent transport** options after **P0 manual** governed relay is **published** (A2): extension/plugin, CLI, MCP, ACP, or hybrid — **without** making any transport normative until PA disposition.

**Architectural framing (2026-10-01):** Engineering Agent integration is defined by the **provider-neutral plugin contract** ([ADR-0021](../ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md)) and **relay bridge** render/parse ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) §11). Historical **CursorBridge** naming in this watch item and the PAR plan refers to **one concrete / reference P1 provider candidate** — not the definition of Engineering Agent transport architecture.

## Scope and non-goals

- Does **not** authorize extension, MCP, ACP, CLI, plugin loader, or provider SDK implementation.
- Does **not** merge into AWI-0003 (compatibility only).
- Does **not** change SPEC-006 P0 manual-first requirement unless PA amends SPEC-006.
- Does **not** redefine [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary) (placement / contract — addressed by ADR-0021).

## Investigation themes

| Theme | Question |
|---|---|
| P0 parity | Automated path preserves same validation, eligibility (PC-PAR-014), and provenance as P0 manual |
| Security | Credential handling, repo scope, prompt injection via handover paste or automated delivery |
| Mode routing | How a reference provider receives PLAN/AGENT/DEBUG **routing intent** without inferring implementation authorization |
| Evidence return | Structured `EngineeringResultImport` vs transcript; future attachment to **ArchitecturalReviewSubmission** (**B**) |
| Licensing / lifecycle | Reference provider API stability for production dependency |
| GAP-030 alignment | Concrete provider logic remains in **bounded plugins** — not Core or Software Engineering Extension |

### Reference P1 candidate — Cursor (historical scope)

The following themes remain valid **for Cursor as a first/reference P1 investigation** only:

| Theme | Question |
|---|---|
| Extension vs CLI vs MCP/ACP | Which Cursor-facing mechanism best satisfies ADR-0021 automated boundary |
| IDE session | Mapping governed `EngineeringAgentSession` intent to Cursor agent/chat lifecycle without provider IDs in Core provenance |
| Product coupling | Keep Cursor SDK/API/auth inside a bounded plugin assembly |

## Signals to watch

- Transport choice embedded in governance semantics (forbidden).
- Automated relay before INCOMPLETE handover rules are tested on P0.
- Provider plugin bypassing Core relay validation.
- Concrete Engineering Agent provider logic embedded in **Core** or **Software Engineering Extension** (forbidden — [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary), [ADR-0021](../ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md)).

## PA disposition — A2-T7 closeout (2026-09-30)

Historical watch-item text may reference specific tools; **current normative implementation** (A2-T6/T7) is provider-neutral **Engineering Agent** source with P0 manual relay only.

Before **A4** implementation authorization:

- Reconcile automated transport plans against **GAP-030** (provider-neutral plugin contract + separately bounded concrete Engineering Agent plugins).
- Do **not** interpret A4 or this watch item as authorization to hard-code a concrete provider bridge inside Core or the Software Engineering Extension.
- Intended direction: generic Engineering Agent automation semantics + provider-neutral plugin contract + bounded provider plugins — **not** Extension → single hard-coded provider integration.

**No** plugin interfaces, loaders, registries, manifests, discovery, or provider SDK integration is authorized by T7 closeout.

## PA disposition — ADR-0021 documentation tranche (2026-10-01)

- **Architectural reconciliation** for plugin **placement** and **contract** — **Accepted** in [ADR-0021](../ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md).
- **AWI-0006 remains Active** for **transport mechanism** investigation ([GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport)).
- **A4 implementation** and **A4 implementation plan** remain **not authorized**.
- **Closeout** of AWI-0006 requires PA disposition selecting or deferring a **non-normative preferred P1 transport direction** (reference candidate may remain Cursor) without conflating mechanism with contract.

## Parent

- [Watch Items](README.md)

## Related Documents

- [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md)
- [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md)
- [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary), [GAP-044](../../Development/EDF_Gap_Register.md#gap-044--engineering-agent-automated-transport)
