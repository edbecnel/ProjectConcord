# Manual Verification Records

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Verification](../README.md) › Records

## Purpose

Store **one MVR Markdown file per governed human-manual-verification obligation**.

## Naming

```text
MVR-NNNN-<short-title>.md
```

Example: `MVR-0001-mvr-consumption-feature-acceptance.md`

## Record identity

| Concept | Convention |
|---|---|
| Record ID | `MVR-NNNN` (unique in this directory; declared in Identity metadata) |
| Test ID | `MVT-1`, `MVT-2`, … (local to the MVR) |
| Global reference | `MVR-NNNN / MVT-n` |

ProjectConcord discovery **must not** treat filename alone as canonical Record ID when Identity metadata is absent or inconsistent ([SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)).

## Template

Copy [Manual_Verification_Record_Template.md](../../Templates/Manual_Verification_Record_Template.md) when creating a new record.

## Index

| Record ID | Title | Human execution status |
|---|---|---|
| [MVR-0001](MVR-0001-a1c-desktop-project-root-recent-workflow.md) | A1c Desktop Project Root and Recent Projects workflow | Complete (2026-09-28) |
| [MVR-0002](MVR-0002-a2-p0-manual-governed-relay-workflow.md) | **A2 P0 manual governed relay workflow** — executable operator procedure + execution record | **Complete** (2026-09-30; MVT-1–MVT-19 Pass; PA accepted for A2 closeout) |

## Parent

- [Verification](../README.md)

## Related Documents

- [MVR-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md)
- [SPEC-005](../../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
