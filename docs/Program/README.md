[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › Program

# Program

## Purpose

Program-level documentation for ProjectConcord: milestones and **Engineering Gate Review Records (EGR)** per [EGR-0001 v1.1](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md) (normative EDF specification), including **Governed Dependency Override (GDO)** semantics for non-blocking prerequisites while gates remain Open.

ProjectConcord adopts EGR as the **authoritative** mechanism for gate approval (not chat-only).

## Gate Reviews

| Gate ID | Record | Status | Unblocks |
|---|---|---|---|
| **G0** | [EGR-G0 — Architecture Planning Gate](Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) | **Satisfied** | M1 solution creation |
| **G1** | [EGR-G1 — MVP Implementation Gate](Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md) | Open | M1 MVP implementation in earnest (after G0) |

## Bootstrap vs Program Gates

| Type | Meaning |
|---|---|
| **BVG-1 … BVG-6** | EDF bootstrap validation (Framework Advisor tiers) |
| **G0 / G1 (EGR)** | ProjectConcord program gates in [Gate_Reviews/](Gate_Reviews/) |
| **AAR (AAR-NNNN)** | Implementation conformance audits in [Architecture/Audits/](../Architecture/Audits/README.md) per EDF [AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md) |

## Parent

- [Project Index](../../PROJECT_INDEX.md)

## Related Documents

- [EDF Governed Dependency Override — Architecture Handover](../Handover/EDF-Governed-Dependency-Override-Architecture-Handover.md)
- [EDF Gap Register](../Development/EDF_Gap_Register.md) (GAP-006, GAP-040)
- [PCR-0001 — Project Continuation and Pause Record](../Development/PCR-0001-Project-Continuation-and-Pause-Record.md)
- [Implementation Roadmap](../Development/Implementation_Roadmap.md)
- [EDF Bootstrap Report](../../EDF_BOOTSTRAP_REPORT.md)
