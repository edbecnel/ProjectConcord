[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › PCON-0002

# PCON-0002: Actor–Role Model, Engineering Domain Neutrality, and Governance Abstraction

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural Discovery Record |
| **Normative** | No |
| **Status** | Proposed — post-closeout architectural input; queued for future Project Architect review |
| **Record ID** | PCON-0002 |
| **Date** | 2026-09-21 |
| **Owner** | ProjectConcord |
| **Authoritative** | No — candidate models and requirements only |
| **Precedes** | AI governance documentation tranche closeout (`2dfdc97c84c5c464ce7fe7263bbe400e6ba3dcdc`); does **not** amend that tranche |
| **Integration context** | Prior tranche integrated at `b728e2896992b58ee785d406ac93a6badf29c8c8`; see [AI Governance Workflow Integration Analysis](AI_Governance_Workflow_Integration_Analysis.md) |
| **Watch item** | [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md) |
| **Related discovery** | [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) (Proposed) |
| **Related decisions / specs** | [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (Proposed); [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (Draft / not implemented) |

---

## Handover disposition

This record captures **architectural discovery only**. It is **not** pre-authorized implementation work, **not** an architecture plan, and **not** an amendment silently folded into the closed AI-governance documentation tranche.

**Do not:** change ADR-0013 or SPEC-004 status; add PC-AIGOV-029–051 to SPEC-004 as normative requirements; rename or redesign existing concepts; begin M1, M7, or other implementation milestones.

Candidate requirements PC-AIGOV-029–051 are recorded in §8 and [EDF Gap Register](../Development/EDF_Gap_Register.md) GAP-037 for future disposition.

---

## 1. Purpose

The current AI-governance architecture has been described partly in terms of particular **participants** (for example, an AI Project Architect, a Cursor/repository execution agent, human authority).

The more general architectural model should instead distinguish:

| Concept | Meaning |
|---|---|
| **Role** | What responsibility and authority exists (a logical engineering responsibility). |
| **Actor** | The human, AI agent, automated service, team, or other entity capable of performing work. |
| **RoleAssignment** | The explicit association assigning an Actor to a Role for an applicable project/work scope. |

**Provider or actor identity must not define the engineering role itself.**

Examples:

- “Project Architect” is a **role**. ChatGPT may be the **actor** assigned to that role.
- “Software Developer” and “Test Developer” are separate **roles** even when Cursor is assigned to both.
- Replacing ChatGPT, Cursor, or a human with another actor should **not** require changing the underlying governance semantics.

---

## 2. Engineering role vocabulary (investigation scope)

The future architecture should investigate a standard software-engineering **role vocabulary** including, but not necessarily limited to:

- Project Authority
- Project Architect
- Requirements / Product Authority
- Software Developer
- Test Developer
- QA Tester
- Reviewer
- Software Documentation Engineer
- End-User Documentation Specialist
- Repository Operator
- Release / Configuration Manager
- Project Manager
- Development Manager

These are **logical responsibilities**, not requirements for separate people.

- One Actor may hold multiple Roles when project policy permits.
- One Role may potentially have multiple Actors.
- Project policy may impose **separation-of-duty** requirements between particular roles (for example, implementation versus independent review, implementation versus QA, or implementation versus final acceptance).

**Authority/capability must remain distinct from mere role membership.** An Actor assigned Software Developer, for example, must not thereby acquire authority to accept architecture, close a gate, authorize the next tranche, push, merge, or perform other privileged operations unless separately permitted.

---

## 3. Development, automated testing, QA, and acceptance

The future architecture must preserve the distinction among:

| Role / responsibility | Scope |
|---|---|
| **Software Developer** | Production implementation. |
| **Test Developer** | Unit, integration, regression, and other automated verification development. |
| **QA Tester** | Behavioral, interactive, workflow, acceptance, and other QA validation. |
| **Reviewer / Project Architect** | Review appropriate to the assigned review/architecture responsibility. |
| **Acceptance Authority** | Authority to accept the relevant result where explicitly granted by project governance. |

**Passing automated tests must not automatically constitute** QA acceptance, architectural acceptance, gate acceptance, or authorization of subsequent work.

Evidence should eventually be attributable to both the **Actor** and the **Role** under which that Actor produced or attested to it.

---

## 4. Documentation roles

### 4.1 Software Documentation Engineer

Responsible for developer-facing and integration-facing technical documentation such as:

- API/reference documentation
- Developer handbook
- SDK/integration documentation
- Extension/plugin documentation
- Developer-oriented architecture guidance
- Build/configuration/deployment documentation
- Technical examples
- Migration/upgrade documentation

This role documents accepted architecture and implementation; it does **not** gain architectural authority merely by documenting it.

### 4.2 End-User Documentation Specialist

Responsible for user-facing documentation such as:

- User manuals
- Getting-started documentation
- Feature documentation
- Tutorials
- Workflows
- UI help
- Troubleshooting
- Contextual help
- Release-facing user documentation

User documentation should be traceable to applicable requirements, accepted/released behavior, UI/workflow definitions, and validation evidence.

---

## 5. Engineering-domain neutrality

Software engineering is currently ProjectConcord's initial and reference engineering discipline. However, ProjectConcord may later support other engineering disciplines, for example:

- Electrical/electronic engineering
- Mechanical engineering
- Civil/structural engineering
- Systems engineering
- Manufacturing engineering
- Multidisciplinary engineering projects
- Additional future disciplines

These disciplines may differ in roles, responsibilities, workflows, artifacts, evidence, validation methods, repositories/version-management systems, reviews, acceptance processes, terminology, and separation-of-duty requirements.

**Therefore, ProjectConcord Core should eventually be evaluated for engineering-domain neutrality.** Software-specific assumptions must not automatically become universal ProjectConcord semantics merely because software engineering is the first implemented domain.

---

## 6. Candidate architectural direction (not accepted)

A possible future layering for investigation:

```text
ProjectConcord Core
    |
    +-- Actor / Identity
    +-- Role
    +-- RoleAssignment
    +-- Authority / Capability
    +-- Work Authorization
    +-- Evidence / Provenance
    +-- Review / Acceptance
    +-- Workflow / State
    +-- Project / Workspace
    +-- Dependency
    +-- Inter-Project Handover
    +-- Audit
            |
            v
    Engineering Domain Profiles
            |
            +-- Software Engineering
            +-- Electrical / Electronic Engineering
            +-- Mechanical Engineering
            +-- Civil / Structural Engineering
            +-- Systems Engineering
            +-- other future disciplines
```

This is a **candidate architectural direction**, not an accepted architecture.

A future **EngineeringDomainProfile** concept may need to define or extend: role definitions; artifact definitions; workflow definitions; evidence definitions; validation definitions; authority/capability policies; separation-of-duty policies; discipline-specific terminology.

Projects may eventually need **more than one** engineering-domain profile because multidisciplinary projects must not be forced into exactly one discipline.

---

## 7. Terminology and watch concerns (do not rename now)

Existing concepts must be reviewed later for accidental software-specific assumptions. Examples:

- DevelopmentWorkAuthorization
- Repository Execution Agent
- Software Developer (as a fixed participant label rather than a role)
- Source-code-centric workflows
- Git-centric assumptions
- Unit/regression testing
- Code-oriented evidence
- Repository terminology

For example, **DevelopmentWorkAuthorization** may eventually prove to be a Software Engineering specialization of a more general concept such as **EngineeringWorkAuthorization**.

Likewise, Git may be one repository/versioning provider for software projects rather than a universal ProjectConcord Core assumption. Other disciplines may use CAD/PDM/PLM or other engineering information-management systems.

**Do not rename or redesign these concepts in this capture.** Record them for future architectural investigation under [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md).

---

## 8. Candidate future requirements (non-normative)

The following are **candidate requirements for future Project Architect review**, not accepted normative requirements. They are **not** added to SPEC-004 in this task.

| ID | Summary |
|---|---|
| PC-AIGOV-029 | Role-based workflow — responsibilities through roles independently of assigned human, AI, service, or team |
| PC-AIGOV-030 | Actor identity — identifiable actors capable of governed responsibilities |
| PC-AIGOV-031 | Explicit role assignment — governed role authority via explicit assignments or project-authority rules |
| PC-AIGOV-032 | Scoped assignment — project, artifact-system, gate, stage, tranche, work, and/or authorization scope |
| PC-AIGOV-033 | Multiple roles per actor — when permitted by project policy |
| PC-AIGOV-034 | Multiple actors per role — when permitted by project policy |
| PC-AIGOV-035 | Separation of duties — independence among implementation, test, QA, review, acceptance |
| PC-AIGOV-036 | Capability-bounded roles — assignment does not imply unrestricted repository or governance authority |
| PC-AIGOV-037 | Distinct verification responsibilities — implementation, automated test, QA, review, acceptance remain distinguishable |
| PC-AIGOV-038 | Evidence attribution — evidence attributable to Actor and Role |
| PC-AIGOV-039 | Cross-project authority isolation — no automatic authority carry-over across managed projects |
| PC-AIGOV-040 | Provider independence — semantics independent of AI provider, IDE agent, title, or tool |
| PC-AIGOV-041 | Assignment auditability — material assignments, delegations, changes, revocations auditable |
| PC-AIGOV-042 | Software documentation role — technical documentation distinct from implementation and architectural authority |
| PC-AIGOV-043 | End-user documentation role — user-facing documentation distinct from software-development documentation |
| PC-AIGOV-044 | Engineering-domain neutrality — Core must not assume every project is software engineering |
| PC-AIGOV-045 | Engineering domain profiles — discipline-specific profiles for roles, terminology, artifacts, evidence, workflows, validation, policies |
| PC-AIGOV-046 | Software engineering as initial profile — reference domain without universalizing discipline-specific concepts |
| PC-AIGOV-047 | Extensible role vocabulary — profile/project defined roles, not a permanent hardcoded software enumeration |
| PC-AIGOV-048 | Extensible workflows — profiles express discipline-appropriate workflows, reviews, evidence, validation, acceptance |
| PC-AIGOV-049 | Multidisciplinary projects — path for multiple compatible engineering-domain profiles |
| PC-AIGOV-050 | Domain-specific evidence — discipline evidence coexists with common provenance, actor, role, authorization, review, acceptance |
| PC-AIGOV-051 | Core versus profile semantics — Core concepts not defined solely because the initial Software Engineering Profile requires them |

Indexed for discovery: [EDF Gap Register](../Development/EDF_Gap_Register.md) GAP-037.

---

## 9. Future reconciliation

When architecture is next reviewed for AI governance and workspace models, reconcile this discovery with:

- [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) participant-oriented narrative
- [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (remain Proposed until explicitly accepted)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (remain Draft / not implemented until explicitly implemented)
- [GAP-019](../Development/EDF_Gap_Register.md) persona vs authorization role taxonomy
- EDF upstream [AWI-0001 — Domain Independence](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/Watch_Items/AWI-0001-Domain-Independence.md) (methodology-level domain neutrality)

---

## Parent

- [Architecture](README.md)

## Related Documents

- [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [AI Governance Workflow Integration Analysis](AI_Governance_Workflow_Integration_Analysis.md)
- [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md)
- [EDF Gap Register](../Development/EDF_Gap_Register.md) (GAP-037)
- [Implementation Roadmap](../Development/Implementation_Roadmap.md)
