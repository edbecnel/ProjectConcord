[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Specifications](../Specifications/README.md) › SPEC-001

# SPEC-001: MVP EDF Desktop Client

## Metadata

| Field | Value |
|---|---|
| **Spec ID** | SPEC-001 |
| **Status** | Draft |
| **Owner** | ProjectConcord |
| **Target release** | MVP (M1–M5) |

## Problem

Engineers adopting EDF must navigate complex repository conventions manually. EDF defines structure and validation tooling, but there is no integrated operational environment to discover project state, run conformance checks, navigate semantically, and author valid artifacts safely.

## Goals

- Open a local EDF project by filesystem path.
- Resolve EDF profile/capabilities and list major artifacts.
- Run Framework Advisor / conformance validation and present results.
- Navigate from project index through domains to artifacts and cross-links.
- Create or edit at least one specification artifact via structured authoring, validate, and save to canonical location.
- Preserve EDF as canonical; no proprietary **engineering-state** database ([ADR-0002](../../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)). Operational/collaboration persistence is allowed per [ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md).
- Align solution structure with multi-user platform seams ([ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [ADR-0010](../../Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)) even when running as a solo Administrator locally.

## Non-Goals

- Jira-like work management, full Git client, IDE features, cloud collaboration
- AI-assisted authoring and reconciliation (M6+)
- Roslyn-based change impact (M6+)
- Full shared project services, enterprise IAM, web UI, and real-time multi-user editing (post-MVP; see [Implementation Roadmap](../../Development/Implementation_Roadmap.md))

## User Stories

1. As an engineer, I want to open my EDF repo in the app so that I see profile, structure, and conformance without editing config files by hand.
2. As an engineer, I want to browse from PROJECT_INDEX to ADRs and SPECs so that I understand relationships without memorizing paths.
3. As an engineer, I want to create a SPEC from a template so that it saves to the correct folder and passes validation.

## Acceptance Criteria

- [ ] User selects project root; app detects EDF project with documented confidence rules ([EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-001).
- [ ] App displays resolved profile/capabilities and required directory checklist.
- [ ] App runs EDF conformance validation and shows overall/structure/navigation scores matching script output for same EDF path.
- [ ] User navigates to an ADR and SPEC discovered under `docs/`.
- [ ] User creates `docs/Specifications/features/SPEC-NNN-*.md` via UI; file contains required template sections; Framework Advisor shows no new critical violations for that artifact.
- [ ] Application runs on macOS; build documented in Developer Handbook.

## Technical Notes

- Implementation order: [Implementation Roadmap](../../Development/Implementation_Roadmap.md) M1–M5.
- Architecture: [System Architecture Overview](../../Architecture/System_Architecture_Overview.md).
- Canonical authoring requirement (PCON-0000 §52): invalid SPEC must be fixable inside app when rules are deterministic.

## Dependencies

- [EGR-G1](../../Program/Gate_Reviews/EGR-G1-MVP-Implementation-Gate.md) satisfied; ADR-0001 and ADR-0002 Accepted
- Local EDF clone path configuration

## Future product (post-MVP)

Platform collaboration (shared project services, membership beyond default Administrator, concurrent change detection) follows [ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md) and roadmap M6+.

Following EDF [EGR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/EGR-0001-Engineering-Gate-Review-Records.md), a later release SHOULD discover Engineering Gate Review Records under `docs/Program/Gate_Reviews/`, display open gates, and surface per-document and gate decision checkbox state in the UI (traceability for PCON-0000 `GetOpenGates()` — target after M5, detailed in a future SPEC).

## Open Questions

- [ ] Minimum supported .NET version for Avalonia LTS
- [ ] Ship embedded EDF script runner vs require user-configured EDF path only

## Parent

- [Specifications](../Specifications/README.md)

## Related Documents

- [SPEC-002](../../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)
- [NFR](../NFR.md)
- [PROJECT_CHARTER](../../../PROJECT_CHARTER.md)
