# AWI-0006-PAR-Cursor-Bridge-Transport

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0006

| | |
|---|---|
| **Status** | Active |
| **Owner** | ProjectConcord |
| **Created** | 2026-09-28 |
| **Revisit Trigger** | Before A4 implementation authorization or PA request to evaluate P1 transport |
| **Discovery source** | [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md); [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) |
| **Related ADRs** | [ADR-0015](../ADRs/ADR-0015-Project-Identity-PAR-and-Per-User-Operational-State.md) (**Accepted** 2026-09-29) |
| **Related specs** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) (**Accepted** 2026-09-29) |
| **Cross-reference** | [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md) — broader orchestration; AWI-0006 scopes PAR Cursor bridge transport only |

---

## Objective

Investigate **CursorBridge** transport options for PAR after **P0 manual** relay is implemented: extension/plugin, CLI, MCP, ACP, or hybrid — without making any transport normative until PA disposition.

## Scope and non-goals

- Does **not** authorize extension, MCP, ACP, or CLI implementation.
- Does **not** merge into AWI-0003 (compatibility only).
- Does **not** change SPEC-006 P0 manual-first requirement unless PA amends SPEC-006.

## Investigation themes

| Theme | Question |
|---|---|
| P0 parity | Manual path preserves same validation and provenance as future automated bridge |
| Security | Credential handling, repo scope, prompt injection via handover paste |
| Mode routing | How IDE receives PLAN/AGENT/DEBUG without inferring authorization |
| Evidence return | Structured vs transcript import; attachment to ArchitecturalReviewSubmission |
| Licensing / lifecycle | Cursor API stability for production dependency |

## Signals to watch

- Transport choice embedded in governance semantics (forbidden).
- Automated relay before INCOMPLETE handover rules are tested on P0.
- CursorBridge bypassing PAR validation.
- Concrete Engineering Agent provider logic embedded in **Core** or **Software Engineering Extension** (forbidden — see [GAP-030](../../Development/EDF_Gap_Register.md#gap-030--engineering-agent-provider-adapter-and-plugin-boundary)).

## PA disposition — A2-T7 closeout (2026-09-30)

Historical watch-item text may reference specific tools; **current normative implementation** (A2-T6/T7) is provider-neutral **Engineering Agent** source with P0 manual relay only.

Before **A4** implementation authorization:

- Reconcile automated transport plans against **GAP-030** (provider-neutral plugin contract + separately bounded concrete Engineering Agent plugins).
- Do **not** interpret A4 or this watch item as authorization to hard-code a concrete provider bridge inside Core or the Software Engineering Extension.
- Intended direction: generic Engineering Agent automation semantics + provider-neutral plugin contract + bounded provider plugins — **not** Extension → single hard-coded provider integration.

**No** plugin interfaces, loaders, registries, manifests, discovery, or provider SDK integration is authorized by T7 closeout.

## Parent

- [Watch Items](README.md)

## Related Documents

- [PAR Workflow Architecture Plan](../../Handover/ProjectConcord-PAR-Workflow-Architecture-Plan.md)
- [PCON-0004](../PCON-0004-Primary-Orchestration-UI-and-External-Engineering-AI-Integration.md)
- [GAP-044](../../Development/EDF_Gap_Register.md)
