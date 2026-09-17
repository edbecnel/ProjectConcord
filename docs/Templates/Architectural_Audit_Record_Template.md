# AAR-[NNNN]: [Audit Title]

[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](../Architecture/README.md) › [Audits](../Architecture/Audits/) › AAR-[NNNN]

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural Audit Record |
| **Normative** | No (findings are assessments; requirements remain in ADRs and specifications) |
| **Audit ID** | AAR-[NNNN] |
| **Audit Status** | Open / Complete / Superseded |
| **Scope** | [Brief scope statement] |
| **Audit Date** | YYYY-MM-DD (or period) |
| **Owner** | [Role or name] |
| **Superseded By** | [Link to replacement AAR if Superseded] |

## Purpose

Describe why this audit was conducted and what implementation areas or release it supports.

## Requirements Basis

| Requirement | Link | Status (if ADR) | In scope |
|---|---|---|---|
| [Title] | [relative/path.md](relative/path.md) | Accepted / Proposed / … | Yes / No |

List every ADR, normative specification, or other authoritative requirement referenced by this audit.

## Implementation Scope

| Anchor | Value |
|---|---|
| Repository paths | `src/...` |
| Branch / tag / commit | |
| Modules or components | |
| Out of scope (explicit) | |

Record enough detail that another engineer can reproduce the audit.

## Findings

Repeat the following block for each finding.

### Finding [N]: [Short title]

| Field | Value |
|---|---|
| **Classification** | Conformant / Gap / Violation / Deferred / Out of scope |
| **Requirement** | Link or ID from Requirements basis |
| **Expected** | What the requirement calls for |
| **Observed** | What the implementation does |
| **Evidence** | Paths, symbols, commits |
| **Remediation** | None / PR / task / Proposed ADR / AWI / … |

## Summary (optional)

| Classification | Count |
|---|---|
| Conformant | |
| Gap | |
| Violation | |
| Deferred | |
| Out of scope | |

## Remediation Tracker

After audit **Complete**, track follow-up:

- [ ] [Remediation item — link to PR, task, or ADR]
- [ ] Update this AAR when superseded by a re-run

| Field | Value |
|---|---|
| **Completed by** | |
| **Completion date** | |

## Parent

- [Architecture Audits](../Architecture/Audits/README.md)

## Related Documents

- [EDF AAR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Specifications/AAR-0001-Architectural-Audit-Records.md)
- [ADR-0012](../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md)
