# Verification

[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › Verification

## Purpose

This domain holds **Manual Verification Records (MVR)** — canonical executable human manual verification procedures and factual execution records when EDF-governed work requires human-executed manual verification.

Planning artifacts (specifications, architecture, implementation plans, gate records) declare **what** must be verified and link here. They do not replace MVRs as the operator checklist.

ProjectConcord consumes MVR semantics per [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) and [MVR handover](../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md). EDF normative rules remain in the Engineering Documentation Framework ([MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md)).

## What belongs here

| Artifact | Location |
|---|---|
| Manual Verification Record (MVR) | [Records/](Records/README.md) as `MVR-NNNN-<short-title>.md` |

## What does not belong here

- Architecture design and rationale → [docs/Architecture/](../Architecture/)
- Program gates and maintenance → [docs/Program/](../Program/)
- Normative framework rules → EDF [MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md) in the EDF Specifications tree

## ProjectConcord templates

- [Manual Verification Record Template](../Templates/Manual_Verification_Record_Template.md) — project copy derived from EDF baseline `192fe5c`

## Parent

- [Project Index](../../PROJECT_INDEX.md)

## Related Documents

- [SPEC-005 — MVR consumption](../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
- [ADR-0014 — MVR human attestation and AI boundary](../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)
- [EDF MVR architecture handover](../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md)
