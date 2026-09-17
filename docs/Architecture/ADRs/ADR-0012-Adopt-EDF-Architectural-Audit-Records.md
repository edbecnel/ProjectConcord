# ADR-0012: Adopt EDF Architectural Audit Records

## Status

Accepted

## Date

2026-09-17

## Context

EDF introduced **Architectural Audit Records (AAR)** for structured **implementation conformance reviews** ([AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md), [EDF ADR-0008 — Architectural Audit Records](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/ADRs/ADR-0008-Architectural-Audit-Records.md)). ProjectConcord already uses **EGR** for document gates and Framework Advisor for **documentation** scoring; M1+ requires a canonical artifact for **code vs architecture** alignment.

**Naming:** ProjectConcord [ADR-0008](ADR-0008-CRA-and-CKES-Dependency-Boundary.md) is **CRA/CKES boundary** — unrelated to EDF ADR-0008 (AAR policy).

## Decision

1. Adopt EDF **AAR-0001** for every **implementation conformance review** recorded as authoritative in the charter, roadmap, or EGR.
2. Store AAR **instance** files under `docs/Architecture/Audits/` with IDs **AAR-NNNN** unique in this repository (separate from EGR gate IDs, BVG aliases, PCON-0000, CRA series).
3. Do **not** copy AAR-0001 into `docs/Specifications/`; framework spec remains authoritative via EDF.
4. AAR findings MUST NOT change ADR or normative SPEC **Status**; remediation follows normal ADR and document lifecycle ([AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md) §8).
5. **AAR** MUST NOT be confused with **operational audit** metadata (membership, integrity events) in [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) and [SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md).
6. First required audit: **AAR-0001** for M1 solution skeleton vs Accepted ADR-0001–ADR-0011 and SPEC-001 skeleton scope; **Complete** before [EGR-G1](../../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md) **Gate satisfied**.
7. Use [Architectural_Audit_Record_Template.md](../../Templates/Architectural_Audit_Record_Template.md) for new AAR files.

## Alternatives Considered

### Informal audit notes in roadmap only

- Advantages: Less paperwork.
- Disadvantages: No reproducible findings; conflicts with EDF AAR-0001 and EGR §13.
- Reason not selected: EDF adoption and G1 requirement.

## Consequences

### Positive

- Traceable implementation-vs-requirements analysis in Git.

### Negative

- Additional documents when re-auditing after major refactors.

## References

- [Architecture Audits README](../Audits/README.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
- [EGR-G1 — MVP Implementation Gate](../../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md)
