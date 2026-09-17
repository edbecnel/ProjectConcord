[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Specifications](../README.md) › SPEC-003

# SPEC-003: Canonical Artifact Integrity and Authorized State Transitions

## Metadata

| Field | Value |
|---|---|
| **Spec ID** | SPEC-003 |
| **Status** | Draft |
| **Owner** | ProjectConcord |
| **Normative** | Yes — product architecture and behavior requirements |
| **Last Reviewed** | 2026-09-15 |
| **Governing Framework** | Engineering Documentation Framework (EDF) |
| **Classification** | Feature / platform specification (`docs/Specifications/features/`) |
| **Former location** | Repository root (handover); permanent path applied per EDF |

## Parent

- [Specifications](../README.md)

## Related Documents

- [System Architecture Overview](../../Architecture/System_Architecture_Overview.md)
- [SPEC-001 MVP Desktop Client](SPEC-001-mvp-edf-desktop-client.md)
- [SPEC-002 Referential Integrity](SPEC-002-canonical-artifact-relationships-referential-integrity.md)
- [ADR-0011 Canonical Artifact Integrity and Trusted State](../../Architecture/ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md)
- [ADR-0007 Semantic Artifact Identity](../../Architecture/ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md)
- [ADR-0006 AI Boundary](../../Architecture/ADRs/ADR-0006-AI-Boundary.md)
- [EDF Gap Register](../../Development/EDF_Gap_Register.md) — GAP-004, GAP-022–GAP-025
- [CRA Alignment and Responsibility Boundaries](../../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md)
- [Canonical Integrity Integration Analysis](../../Architecture/Canonical_Integrity_Spec_Integration_Analysis.md)

## Relationship to SPEC-002

[SPEC-002](SPEC-002-canonical-artifact-relationships-referential-integrity.md) defines semantic **relationships**, Artifact Registry, Relationship Index, identity resolution, and move/rename referential impact.

SPEC-003 defines **integrity state**, **governed fields**, **authorized lifecycle transitions**, **external change detection**, **fingerprints and trusted integrity records**, and the boundary between canonical artifact integrity and implementation reconciliation.

Both specs share artifact IDs and governed relationship vocabulary; neither duplicates the other's normative rules.

---

# 1. Handover Instructions to Cursor AI

**Handover status (2026-09-15):** Steps 1–3 and initial integration applied — permanent file [`SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md`](SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md). ADR-0011, architecture amendments, gap register updates, and [EGR-G0](../../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md) review per integration analysis; implementation gated per [Implementation Roadmap](../../Development/Implementation_Roadmap.md).

Original handover checklist (historical):

This document was supplied at the **ProjectConcord repository root** for architectural handover.

**The repository root is NOT its intended permanent location.**

The supplied filename and title are provisional.

Cursor AI must:

1. locate this document in the ProjectConcord repository root;
2. inspect the current ProjectConcord repository;
3. inspect all existing ProjectConcord architectural documentation;
4. inspect the authoritative current Engineering Documentation Framework (EDF);
5. inspect applicable Canonical Representation Architecture (CRA) documentation;
6. determine the proper EDF classification for this material;
7. determine its proper EDF-prescribed permanent location;
8. give it an EDF-compliant filename;
9. give it an EDF-compliant document title;
10. add or correct any required EDF metadata/header;
11. establish appropriate relationships to existing ProjectConcord architectural artifacts;
12. determine whether portions of this specification require formal ADRs, requirements, validation rules, or other EDF artifacts;
13. identify existing ProjectConcord documents that must be amended;
14. identify requirements that properly belong in EDF rather than ProjectConcord;
15. identify requirements that properly belong in CRA rather than EDF or ProjectConcord;
16. integrate this architecture without creating competing definitions of canonical identity, relationships, lifecycle, or integrity;
17. return the resulting architectural plan, affected-document analysis, ADR recommendations, and identified gaps for review before major implementation.

Do NOT assume:

- the repository root is the correct permanent location;
- the supplied filename is EDF-compliant;
- the supplied title is EDF-compliant;
- this document should necessarily remain a single permanent artifact;
- ProjectConcord should invent rules that properly belong to EDF or CRA.

Where authoritative EDF or CRA documentation already defines the required behavior, use that architecture rather than duplicating it.

Where requirements are missing, explicitly identify the architectural gap.

---

# 2. Purpose

ProjectConcord is intended to manage canonical EDF engineering artifacts stored as human-readable Markdown files.

This creates an important integrity problem.

Users who work on an engineering repository may legitimately have direct filesystem and Git access to the same repository containing canonical EDF Markdown artifacts.

Such users may use:

- IDEs;
- text editors;
- Cursor AI;
- Git tools;
- scripts;
- command-line tools;
- other engineering applications.

They can therefore modify canonical Markdown files without using ProjectConcord.

An external edit may alter:

- artifact identity;
- artifact type;
- lifecycle status;
- relationships;
- supersession state;
- acceptance state;
- validation state;
- governed metadata;
- required structure.

The resulting Markdown may remain syntactically valid while no longer representing an authorized or valid canonical engineering state.

ProjectConcord therefore requires an explicit **Canonical Artifact Integrity Model**.

---

# 3. Fundamental Integrity Principle

ProjectConcord must not assume:

> **A file is canonical merely because it exists in the canonical repository.**

The following concepts must remain distinct:

```text
Canonical Representation
        !=
Structurally Valid Representation
        !=
Semantically Valid Artifact
        !=
Authorized Canonical State
```

A Markdown document may:

- parse correctly;
- reside in the correct directory;
- have the correct filename;
- contain valid metadata;

and still represent an invalid or unauthorized engineering state.

---

# 4. Canonicality Is More Than File Validity

Consider an ADR:

```yaml
id: ADR-0017
status: Proposed
```

A user manually changes it to:

```yaml
id: ADR-0017
status: Accepted
```

The resulting Markdown may remain completely valid Markdown.

It may even satisfy a simple schema.

However, the change may represent an engineering lifecycle transition requiring:

- prerequisites;
- validation;
- review;
- approval;
- appropriate authority;
- relationship updates;
- acceptance evidence.

Therefore:

> **Syntax validation alone cannot establish canonical integrity.**

---

# 5. Repository Accessibility Must Be Preserved

ProjectConcord must NOT attempt to solve this problem by making EDF Markdown files inaccessible to normal engineering tools.

The repository should remain:

- human-readable;
- Git-compatible;
- editable with ordinary tools;
- accessible to developers;
- accessible to Cursor AI and other development tools;
- usable without ProjectConcord;
- portable.

ProjectConcord should protect canonical integrity without taking ownership of the repository away from the user.

---

# 6. External Editing Is Expected

Direct editing should not automatically be treated as malicious behavior.

Legitimate examples include:

```text
Developer edits specification in IDE

Cursor updates documentation

Architect edits ADR in text editor

Script updates generated references

Git merge changes documentation

Documentation technician corrects content
```

ProjectConcord must therefore distinguish:

> **externally modified**

from:

> **invalid**

and from:

> **unauthorized canonical transition**.

---

# 7. Canonical Integrity Pipeline

ProjectConcord should conceptually evaluate changed artifacts through multiple layers:

```text
Markdown Repository
        |
        v
Representation Validation
        |
        v
Structural Validation
        |
        v
Identity Validation
        |
        v
Semantic Validation
        |
        v
Relationship Validation
        |
        v
Lifecycle Validation
        |
        v
Authorization / Transition Validation
        |
        v
Canonical Integrity State
```

Not every layer necessarily requires a separate implementation component.

The architecture should nevertheless preserve these distinct concerns.

---

# 8. Governed vs Ordinary Content

ProjectConcord should investigate a distinction between:

```text
Ordinary Authored Content
```

and:

```text
Governed Canonical State
```

For example, changing:

```markdown
## Discussion

Three alternatives were evaluated.
```

to:

```markdown
## Discussion

Four alternatives were evaluated.
```

may be an ordinary content modification.

Changing:

```yaml
status: Proposed
```

to:

```yaml
status: Accepted
```

changes governed engineering state.

These changes should not necessarily have identical integrity consequences.

---

# 9. Candidate Governed Fields

EDF should determine which information constitutes governed canonical state.

Potential examples include:

- artifact identity;
- artifact type;
- lifecycle status;
- formal relationships;
- supersession relationships;
- acceptance state;
- validation state;
- required approval state;
- milestone state;
- gate state;
- closure state;
- required metadata;
- other lifecycle-critical information.

This list is illustrative.

ProjectConcord MUST NOT independently declare fields governed if EDF already defines or should define that distinction.

---

# 10. Canonical State Transitions

Governed fields should preferably be modified through semantic operations rather than arbitrary text replacement.

For example, instead of:

```text
Edit:
status: Proposed

to:

status: Accepted
```

ProjectConcord should conceptually perform:

```text
AcceptArtifact(ADR-0017)
```

The operation can then evaluate the meaning of the requested transition.

---

# 11. Semantic Transition Pipeline

A governed transition should conceptually follow:

```text
Requested Semantic Operation
        |
        v
Resolve Artifact Identity
        |
        v
Load Applicable EDF Rules
        |
        v
Validate Current State
        |
        v
Validate Requested Transition
        |
        v
Validate Preconditions
        |
        v
Validate Relationships
        |
        v
Validate User Authority
        |
        v
Construct Change Set
        |
        v
Preview / Review
        |
        v
Apply
        |
        v
Revalidate
        |
        v
Record Trusted State
```

The exact pipeline may vary by operation.

---

# 12. Authorization Does Not Override Validity

An Administrator may have authority to perform a transition.

That does not mean the Administrator can create an invalid EDF state.

Therefore:

```text
Authorization
      !=
Validity
```

For example:

```text
Administrator
      |
      v
Accept ADR
      |
      v
EDF prerequisites not satisfied
      |
      v
Operation rejected or explicitly blocked
```

Administrator authority must not silently bypass mandatory EDF rules.

---

# 13. Integrity State Model

ProjectConcord should consider explicit integrity states.

Candidate states include:

## VALIDATED

The artifact matches the last known validated and authorized canonical state.

## MODIFIED / UNVERIFIED

The artifact has changed outside the trusted ProjectConcord transition path and requires evaluation.

## INVALID

The artifact violates applicable EDF structure, metadata, identity, lifecycle, or semantic rules.

## RELATIONSHIP CONFLICT

One or more formal relationships are unresolved, invalid, contradictory, or inconsistent.

## UNAUTHORIZED TRANSITION

A governed state transition occurred without satisfying the required authorization or transition process.

## RECONCILIATION REQUIRED

The change may be legitimate but has not yet been adopted into trusted canonical state.

## CONFLICTED

The artifact conflicts with concurrent project changes or other canonical state.

The final vocabulary should be determined through architecture review.

---

# 14. Trusted State Does Not Mean Secret State

ProjectConcord should not require canonical truth to exist only inside a proprietary database.

Instead, the system may maintain enough trusted derived state to determine whether the repository representation has changed since its last validated state.

The canonical engineering information should remain portable according to EDF requirements.

---

# 15. Integrity Fingerprints

ProjectConcord should investigate cryptographic fingerprints for artifact integrity detection.

At minimum, two distinct concepts may be useful:

```text
Artifact Revision
      |
      +-- Representation Fingerprint
      |
      +-- Governed Semantic Fingerprint
```

---

# 16. Representation Fingerprint

A representation fingerprint detects changes to the file representation.

Conceptually:

```text
SHA-256(
    normalized canonical representation
)
```

or another appropriate cryptographic digest.

This may detect:

- prose edits;
- formatting changes;
- metadata changes;
- relationship changes;
- structural changes.

The precise canonicalization/normalization rules must be defined before such a fingerprint can be reliable.

---

# 17. Governed Semantic Fingerprint

A governed semantic fingerprint would cover only the normalized semantic state considered governance-critical.

Conceptually:

```text
Artifact ID
Artifact Type
Lifecycle State
Formal Relationships
Acceptance State
Validation State
Supersession State
Other Governed Fields
        |
        v
Canonical Semantic Serialization
        |
        v
Cryptographic Fingerprint
```

This permits ProjectConcord to distinguish:

```text
"Someone corrected spelling."
```

from:

```text
"Someone changed the accepted engineering state."
```

---

# 18. Fingerprints Are Detection, Not Authority

A cryptographic hash does not prove that a change was authorized.

It only proves that content differs from some recorded state.

Therefore:

> **Integrity fingerprints are change-detection mechanisms, not authorization mechanisms.**

ProjectConcord must combine them with:

- EDF rules;
- identity;
- lifecycle rules;
- relationships;
- authorization;
- revision history;
- change-set history;
- Git evidence where appropriate.

---

# 19. Trusted Integrity Record

ProjectConcord may maintain derived integrity records such as:

```text
Artifact:
ADR-0017

Known Revision:
27

Representation Fingerprint:
...

Governed Semantic Fingerprint:
...

Integrity State:
VALIDATED

Validated Against:
EDF version/profile ...

Validated At:
...

Transition:
AcceptArtifact

Actor:
User identity
```

The exact schema should be designed separately.

The record should not become a competing source of canonical engineering meaning.

---

# 20. External Change Detection

ProjectConcord's existing file-monitoring architecture should participate directly in integrity management.

Conceptually:

```text
External File Change
        |
        v
File Monitor
        |
        v
Identify Artifact
        |
        v
Parse Current Representation
        |
        v
Compare Previous State
        |
        v
Classify Semantic Differences
        |
        v
Validate
        |
        v
Assign Integrity State
        |
        v
Notify / Reconcile if Required
```

Incremental processing should be preferred over unnecessary full-repository analysis.

---

# 21. External Change Classification

ProjectConcord should classify external changes where practical.

Candidate categories:

```text
PROSE / CONTENT CHANGE

STRUCTURAL CHANGE

METADATA CHANGE

IDENTITY CHANGE

RELATIONSHIP CHANGE

LIFECYCLE CHANGE

ACCEPTANCE CHANGE

VALIDATION CHANGE

SUPERSESSION CHANGE

LOCATION / RENAME CHANGE

DELETION

UNKNOWN SEMANTIC CHANGE
```

Different categories may require different levels of review.

---

# 22. Identity Changes Require Special Handling

Changing:

```yaml
id: ADR-0017
```

to:

```yaml
id: ADR-0042
```

is not equivalent to correcting prose.

Potential consequences include:

- identity collision;
- relationship breakage;
- historical continuity loss;
- invalid references;
- duplicated canonical identity;
- accidental creation of a new artifact.

Identity changes should therefore receive heightened validation.

CRA should define foundational identity principles where applicable.

EDF should define engineering artifact identity requirements.

ProjectConcord should enforce them.

---

# 23. Relationship Changes Require Special Handling

Changing:

```text
SPEC-0032
    governed-by -> ADR-0017
```

to:

```text
SPEC-0032
    governed-by -> ADR-0042
```

changes engineering semantics.

A Markdown edit producing such a change must therefore be evaluated as a semantic relationship modification rather than merely a changed hyperlink.

ProjectConcord should use the previously specified:

- Artifact Registry;
- Relationship Index;
- Identity Resolver;
- Link Resolver;
- Referential Integrity Service;

to analyze the consequences.

---

# 24. Lifecycle Changes Require Special Handling

Examples include:

```text
Draft -> Proposed

Proposed -> Accepted

Accepted -> Superseded

Open -> Closed

Pending -> Validated
```

These transitions may have prerequisites.

ProjectConcord should not assume that because the final text represents a legal state, the transition into that state was legitimate.

Where EDF defines transition rules, ProjectConcord should validate the transition history.

---

# 25. Reconciliation of External Changes

A legitimate external edit should be capable of becoming trusted canonical state.

Example:

```text
Cursor modifies EDF artifact
        |
        v
ProjectConcord detects change
        |
        v
MODIFIED / UNVERIFIED
        |
        v
Parse semantic differences
        |
        v
Validate against EDF
        |
        v
Present reconciliation
        |
        +-- Accept
        |
        +-- Reject
        |
        +-- Edit
        |
        +-- Repair
        |
        +-- Defer
        |
        v
Accepted Change
        |
        v
New VALIDATED State
```

This allows interoperability without sacrificing integrity.

---

# 26. Cursor AI Integration

Cursor AI will remain an important external participant in ProjectConcord development workflows.

ProjectConcord must therefore treat Cursor-generated changes as external repository modifications unless Cursor is eventually integrated through a formal ProjectConcord service/API.

Cursor should not receive implicit authority merely because the change was AI-generated.

Likewise, ProjectConcord should not assume Cursor-generated changes are invalid.

They should be evaluated using the same canonical integrity rules.

---

# 27. AI Must Not Self-Authorize

ProjectConcord AI should not be able to:

```text
Generate change
        |
        v
Approve its own change
        |
        v
Declare canonical state valid
```

without the governance required by EDF/project policy.

Preferred model:

```text
AI Proposal
        |
        v
Deterministic Validation
        |
        v
Human Review / Authorized Workflow
        |
        v
Canonical Authoring Service
        |
        v
Trusted State
```

AI may assist with explanation and semantic analysis.

Authority remains separate.

---

# 28. Integration With Canonical Authoring Service

The Canonical Authoring Service should become one of the primary trusted mutation paths for governed EDF state.

Conceptually:

```text
User / AI Intent
        |
        v
Semantic Operation
        |
        v
Canonical Authoring Service
        |
        +-- EDF Validation
        +-- Identity Validation
        +-- Relationship Validation
        +-- Lifecycle Validation
        +-- Authorization
        |
        v
Validated Change Set
        |
        v
Canonical Markdown
        |
        v
Integrity Record
```

This architecture should integrate with the existing ProjectConcord CRUD specification.

---

# 29. CRUD Is Not Sufficient for Governed State

Operations such as:

```text
UpdateArtifact()
```

may be too generic for important lifecycle changes.

The architecture should support domain operations such as:

```text
AcceptArtifact()

SupersedeArtifact()

CloseArtifact()

ValidateArtifact()

ResolveAWI()

AdvanceMilestone()

SatisfyGate()
```

where EDF defines meaningful operations.

The exact vocabulary belongs to the appropriate EDF/domain architecture.

---

# 30. Delete Integrity

Deletion requires particular care.

If a user manually deletes:

```text
ADR-0017.md
```

ProjectConcord must determine:

- what canonical identity disappeared;
- which artifacts reference it;
- whether deletion is permitted;
- whether supersession was required;
- whether historical preservation is required;
- whether Git history contains the artifact;
- whether the deletion creates broken relationships.

The disappearance of a file should not automatically imply legitimate deletion of the canonical engineering artifact.

---

# 31. Move/Rename Integrity

A move or rename should preserve identity where appropriate.

Example:

```text
docs/Architecture/ADR-0017.md

        ->

docs/Architecture/Decisions/ADR-0017-Geometry-Kernel.md
```

If canonical identity remains:

```text
ADR-0017
```

ProjectConcord should classify this as a representation change rather than deletion plus unrelated creation.

This requirement should align with CRA identity principles.

---

# 32. Duplicate Identity Detection

External edits may accidentally create:

```text
docs/A/ADR-0017.md

docs/B/ADR-0017.md
```

ProjectConcord must detect ambiguous or duplicate canonical identities.

Such a condition should prevent the project from being considered fully validated until resolved.

---

# 33. Referential Integrity

Canonical integrity includes referential integrity.

ProjectConcord should detect:

- missing targets;
- stale Markdown paths;
- unresolved canonical references;
- ambiguous identities;
- illegal relationship types;
- illegal source/target combinations;
- cardinality violations;
- invalid reciprocal relationships where reciprocity is required;
- references to artifacts in incompatible lifecycle states.

The exact semantics must come from CRA and EDF where applicable.

---

# 34. Relationship Integrity Must Be Semantic

A Markdown link resolving successfully does not prove the relationship is valid.

For example:

```markdown
[ADR-0017](../Architecture/ADR-0017.md)
```

may resolve correctly while representing an illegal engineering relationship.

Therefore:

```text
Link Integrity
      !=
Relationship Integrity
```

ProjectConcord must validate both where formal EDF relationships are involved.

---

# 35. Git Integration

Git provides valuable integrity evidence.

ProjectConcord should use information such as:

- commit identity;
- author;
- timestamp;
- diff;
- branch;
- merge;
- repository state;
- revision history.

Conceptually:

```text
Git
"What changed?"

        +

ProjectConcord / EDF
"What does the change mean?"

        =

Engineering Integrity Analysis
```

Git should complement rather than replace semantic validation.

---

# 36. Git Commit Does Not Automatically Mean Authorized

A committed change may still violate EDF.

Likewise, a signed or authenticated Git commit proves something about source/authorship but does not inherently prove that an engineering lifecycle transition was authorized according to EDF.

Therefore:

```text
Committed
   !=
EDF Validated

Authenticated
   !=
Engineering Approved
```

---

# 37. Repository Baselines

ProjectConcord should use explicit baselines when evaluating integrity.

Potential baselines include:

- last validated ProjectConcord state;
- last reconciliation;
- previous commit;
- selected commit;
- branch point;
- task start;
- approved change set.

The appropriate baseline depends on the operation.

---

# 38. Multi-User Integrity

The shared ProjectConcord architecture increases the importance of integrity tracking.

Example:

```text
Developer A
     |
     v
Local Repository Change
     |
     v
Shared Project

Meanwhile:

Architect B
     |
     v
Accepted Architecture Change
```

ProjectConcord must determine whether Developer A's pending change is still compatible with the current canonical state.

This should integrate with the concurrency/change-set architecture already specified.

---

# 39. Shared Project Validation Boundary

A shared ProjectConcord deployment may establish a validation boundary before changes become part of authoritative shared project state.

Conceptually:

```text
Developer Repository
        |
        v
Commit / Push / Change Set
        |
        v
ProjectConcord Integrity Validation
        |
        +-- Valid
        |
        +-- Reconciliation Required
        |
        +-- Invalid
        |
        +-- Unauthorized Transition
        |
        +-- Conflict
        |
        v
Authoritative Shared Project State
```

The exact enforcement mechanism should be determined architecturally.

---

# 40. CI Integration

Future ProjectConcord CLI/CI capabilities should be able to validate canonical integrity.

Potential commands may eventually include:

```text
projectconcord validate

projectconcord integrity

projectconcord reconcile

projectconcord relationships

projectconcord status
```

Exact command names are illustrative.

The important requirement is that the same validation engine used by the desktop application should eventually be usable by automated repository workflows.

---

# 41. Pre-Commit vs Server-Side Validation

Cursor should evaluate appropriate validation points such as:

```text
During ProjectConcord Edit

Before Change Set Application

Before Commit

Pre-Commit Hook

Pre-Push

Pull/Merge Request

CI Pipeline

Shared Project Service
```

Not every validation must run at every point.

The architecture should support layered enforcement appropriate to deployment.

---

# 42. Hooks Must Not Be Sole Protection

Git hooks can be bypassed.

Therefore, local pre-commit hooks may improve usability but should not be treated as the ultimate integrity authority in a shared environment.

Server-side or CI validation may provide stronger enforcement.

---

# 43. Repository Without ProjectConcord

An EDF repository must remain useful without ProjectConcord.

Therefore, if ProjectConcord-specific derived integrity state is unavailable, the repository should still contain enough canonical information to:

- understand the engineering artifacts;
- reconstruct identities;
- reconstruct relationships where EDF prescribes them;
- perform EDF validation;
- rebuild ProjectConcord indexes;
- rebuild integrity state to the extent possible.

ProjectConcord must not create a proprietary dependency that makes EDF repositories unintelligible without the application.

---

# 44. Rebuilding Trusted State

There is an important distinction between rebuilding:

```text
Current Semantic State
```

and rebuilding:

```text
Historical Authorization Evidence
```

Current semantic state may often be reconstructed from the repository.

Historical authorization may require:

- Git history;
- ProjectConcord audit history;
- approval records;
- validation records;
- change-set history.

Cursor should explicitly address what integrity information must be portable and what may legitimately remain operational state.

---

# 45. Audit Trail

For governed changes, ProjectConcord should be able to answer where applicable:

```text
What changed?

Who initiated it?

What semantic operation was performed?

What was the previous state?

What is the new state?

Which EDF rule allowed it?

What prerequisites were evaluated?

Who approved it?

What repository revision contains it?

Was AI involved?

Was the change externally authored and later reconciled?
```

Do not duplicate information already reliably available from Git unless ProjectConcord requires additional semantic audit information.

**Architectural Audit Records (AAR):** Periodic **implementation conformance reviews** (code vs Accepted ADRs and normative specs) are recorded as AAR files under [Architecture Audits](../../Architecture/Audits/README.md) per [ADR-0012](../../Architecture/ADRs/ADR-0012-Adopt-EDF-Architectural-Audit-Records.md). AAR is **not** the same as operational audit event storage in [ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md).

---

# 46. Integrity and CRA

This specification must align with the previously established architectural hierarchy:

```text
CRA
Foundational Canonical Identity /
Relationship / Representation Semantics
        |
        v
EDF
Engineering Artifact Semantics /
Lifecycle / Governance
        |
        v
ProjectConcord
Validation / Authorization /
Authoring / Reconciliation /
Operational Enforcement
```

ProjectConcord should not independently redefine canonical identity or relationship theory.

---

# 47. Potential CRA Gaps

If this integrity architecture requires foundational concepts not yet specified by CRA, Cursor should identify them explicitly.

Potential examples include:

- canonical semantic normalization;
- identity continuity across representation changes;
- relationship continuity;
- canonical representation fingerprinting;
- canonical semantic fingerprints;
- representation equivalence.

Do not automatically add these concepts to CRA.

Identify the gap for architectural review.

---

# 48. Potential EDF Gaps

This architecture may reveal that EDF needs more machine-interpretable governance.

Potential requirements include:

- artifact schemas;
- governed fields;
- lifecycle states;
- legal state transitions;
- transition prerequisites;
- formal relationship definitions;
- relationship cardinality;
- authority requirements;
- acceptance rules;
- supersession rules;
- deletion rules;
- validation requirements;
- canonical normalization rules.

If these are absent or ambiguous, report them as **EDF gaps**.

ProjectConcord should not silently invent permanent EDF semantics.

---

# 49. ProjectConcord-Specific Responsibilities

ProjectConcord may legitimately own implementation concepts such as:

- integrity monitoring;
- file watchers;
- integrity dashboards;
- local fingerprints;
- cached validation results;
- change classification;
- reconciliation UI;
- user notifications;
- change-set orchestration;
- authorization enforcement;
- collaboration workflows;
- CI integration;
- operational audit storage.

These should implement CRA/EDF semantics rather than redefine them.

---

# 50. Integrity Dashboard

ProjectConcord should eventually expose project integrity clearly.

For example:

```text
PROJECT INTEGRITY

Validated Artifacts              142

Externally Modified                3

Reconciliation Required            2

Invalid Relationships              1

Unauthorized Transitions           0

Identity Conflicts                 0
```

The user should be able to navigate directly to each issue.

---

# 51. Artifact Integrity View

Selecting an artifact may show:

```text
ADR-0017

Integrity:
VALIDATED

Lifecycle:
Accepted

Representation:
Current

Relationships:
Valid

Last Validated:
...

Repository Revision:
...

External Modification:
None
```

If modified:

```text
ADR-0017

Integrity:
RECONCILIATION REQUIRED

Detected Changes:

    Discussion text changed

    Relationship added:
        governed-by -> SPEC-0041

    Lifecycle:
        unchanged
```

This provides transparency rather than silently accepting or rejecting edits.

---

# 52. Repair Workflows

ProjectConcord should help users repair integrity problems.

Examples:

```text
Broken Reference
    -> Select Correct Target

Duplicate Identity
    -> Resolve Identity Conflict

Illegal Lifecycle Transition
    -> Restore Previous State
       or
       Perform Required Transition Workflow

External Valid Change
    -> Review and Accept

Stale Markdown Link
    -> Regenerate From Canonical Relationship

Invalid Relationship
    -> Remove or Correct
```

Deterministic repair should be preferred where possible.

AI may assist where semantic reasoning is required.

---

# 53. Do Not Overuse AI for Integrity

Canonical integrity should be predominantly deterministic.

AI should NOT be required to determine:

- whether an ID is duplicated;
- whether a file moved;
- whether a hash changed;
- whether a Markdown link resolves;
- whether a required field exists;
- whether an explicit lifecycle transition is allowed;
- whether a relationship violates machine-readable cardinality;
- whether a user has permission.

AI may assist with:

- interpreting ambiguous external changes;
- explaining conflicts;
- assessing semantic consistency;
- proposing repairs;
- analyzing whether prose remains consistent with accepted architecture;
- documentation reconciliation.

This preserves economical AI usage.

---

# 54. Content Integrity vs Engineering Intent

A document may pass all structural checks while its prose contradicts engineering intent.

For example:

```text
ADR-0017:
Accepted decision says use Provider A.

SPEC-0032:
Edited prose now says implementation shall use Provider B.
```

This may require semantic analysis beyond deterministic validation.

ProjectConcord's Change Impact / Reconciliation architecture should address this higher-level consistency problem.

Therefore canonical integrity should be layered:

```text
Structural Integrity
        |
        v
Semantic Structural Integrity
        |
        v
Relationship / Lifecycle Integrity
        |
        v
Authorization Integrity
        |
        v
Engineering Intent Consistency
```

The final layer may sometimes require AI assistance.

---

# 55. No Silent Canonicalization

ProjectConcord must not silently treat an externally modified file as trusted merely because it passes validation.

Likewise, it should not silently rewrite externally modified files merely to restore expected state.

Externally detected governed changes should be surfaced through an explicit reconciliation or adoption process where appropriate.

---

# 56. No Silent Rejection

ProjectConcord should also avoid silently discarding legitimate external changes.

A developer or Cursor may have intentionally made a valid change.

The system should explain:

- what changed;
- why the change affects canonical state;
- which rule applies;
- what action is required.

---

# 57. Integrity Failure Must Be Explainable

When ProjectConcord reports an integrity problem, it should identify:

```text
Artifact

Changed Element

Previous State

Current State

Applicable Rule

Why It Matters

Affected Relationships

Recommended Action
```

The goal is engineering governance, not mysterious enforcement.

---

# 58. Relationship With Change Reconciliation

Canonical integrity and code-to-documentation reconciliation are related but distinct.

Canonical integrity asks:

> **Is the EDF artifact itself valid, internally consistent, and legitimately transitioned?**

Change reconciliation asks:

> **Does the documented engineering intent still agree with implementation and validation evidence?**

Conceptually:

```text
Canonical Artifact Integrity
           |
           +
           |
Implementation Reconciliation
           |
           v
Engineering Project Integrity
```

Both should feed the ProjectConcord dashboard.

---

# 59. Relationship With Multi-User Architecture

The multi-user architecture should use this integrity model as part of its collaboration boundary.

Role-based authorization determines:

```text
Who may request an operation?
```

EDF determines:

```text
Whether the engineering operation is valid.
```

ProjectConcord integrity determines:

```text
Whether the resulting project state can be trusted.
```

These responsibilities must remain distinct.

---

# 60. Initial Implementation Scope

The first practical integrity implementation should focus on high-value deterministic capabilities.

Candidate early scope:

1. artifact identity validation;
2. duplicate identity detection;
3. required metadata validation;
4. lifecycle-state validation;
5. formal relationship validation;
6. broken reference detection;
7. external change detection;
8. semantic change classification for governed fields;
9. previous/current state comparison;
10. representation fingerprinting;
11. governed-state fingerprinting if EDF provides sufficient semantics;
12. reconciliation-required state;
13. integration with Canonical Authoring Service;
14. Git baseline awareness;
15. user-visible integrity status.

More sophisticated authorization history, CI enforcement, server validation, signatures, and semantic AI analysis can follow incrementally.

---

# 61. Acceptance Scenarios

The architecture should eventually support scenarios such as the following.

## Scenario A — Ordinary Prose Edit

A developer fixes spelling in a specification using an IDE.

Expected:

```text
External change detected
        |
        v
No governed state changed
        |
        v
Structural validation passes
        |
        v
Low-risk content modification
```

Project policy determines whether explicit reconciliation is required.

---

## Scenario B — Manual Status Change

A developer manually changes:

```text
Proposed -> Accepted
```

Expected:

```text
Lifecycle change detected
        |
        v
Transition evaluated
        |
        v
Authorization / prerequisites checked
        |
        v
Reconciliation or rejection required
```

The change must not silently become trusted merely because the Markdown is valid.

---

## Scenario C — Relationship Removed

A user manually removes a formal relationship.

Expected:

```text
Relationship delta detected
        |
        v
Relationship Index updated provisionally
        |
        v
EDF relationship rules evaluated
        |
        v
Affected artifacts identified
        |
        v
Review / reconciliation required
```

---

## Scenario D — File Renamed

A user renames an ADR externally.

Expected:

```text
Old representation disappears
New representation appears
Same canonical identity detected
        |
        v
Classify as move/rename
        |
        v
Update derived representation mapping
        |
        v
Check Markdown links
```

Semantic identity remains intact.

---

## Scenario E — Duplicate ID

A copied Markdown file retains the original artifact ID.

Expected:

```text
Duplicate identity detected
        |
        v
Project integrity degraded
        |
        v
User directed to resolve conflict
```

---

## Scenario F — Cursor Updates Documentation

Cursor modifies several EDF documents.

Expected:

```text
External changes detected
        |
        v
Semantic deltas calculated
        |
        v
Governed changes identified
        |
        v
EDF validation
        |
        v
Relationship validation
        |
        v
Reconciliation view
        |
        v
User accepts / rejects / edits
```

Cursor remains fully usable without receiving implicit canonical authority.

---

## Scenario G — Administrator Makes Invalid Change

Administrator attempts a transition prohibited by EDF.

Expected:

```text
Administrator authorized
        |
        v
EDF transition invalid
        |
        v
Operation rejected
```

Authorization does not override engineering validity.

---

## Scenario H — Shared Project Conflict

Developer prepares a change set against revision 27.

Architect changes a related governing ADR producing revision 28.

Expected:

```text
Developer applies change set
        |
        v
Baseline mismatch detected
        |
        v
Semantic impact evaluated
        |
        v
Conflict / revalidation required
```

---

# 62. Required Cursor Investigation

Cursor should determine from the actual repositories:

1. What EDF currently defines as canonical.
2. How EDF currently identifies artifacts.
3. Which artifact fields are lifecycle/governance-critical.
4. What lifecycle states currently exist.
5. Which legal transitions are currently defined.
6. Whether transition prerequisites are machine-readable.
7. How relationships are currently represented.
8. Whether relationship semantics are machine-readable.
9. What CRA currently defines concerning canonical identity.
10. What CRA currently defines concerning canonical relationships.
11. Whether normalization rules exist.
12. Whether canonical representation fingerprints are already contemplated.
13. What existing ProjectConcord services should own integrity behavior.
14. Which existing architectural documents require amendment.
15. Which decisions require ADRs.
16. Which requirements reveal EDF gaps.
17. Which requirements reveal CRA gaps.

Do not infer answers that can be obtained from authoritative project documentation.

---

# 63. Required Affected-Document Analysis

Cursor must identify and review ProjectConcord documentation concerning at least:

- EDF Engine;
- Canonical Authoring Service;
- canonical artifact CRUD;
- Artifact Registry;
- Relationship Index;
- Identity Resolver;
- Link Resolver;
- Referential Integrity Service;
- Git integration;
- file monitoring;
- change sets;
- change reconciliation;
- AI-assisted authoring;
- multi-user architecture;
- authorization;
- shared project services;
- validation;
- future CLI/CI;
- project dashboard.

Update existing documents where this specification materially changes their architecture.

Do not create unnecessary duplicate specifications.

---

# 64. Required Architectural Decisions

Cursor should determine whether formal ProjectConcord ADRs are required for decisions such as:

- externally editable canonical Markdown;
- trusted semantic state;
- governed vs ordinary content;
- integrity fingerprint strategy;
- reconciliation of external changes;
- trusted mutation paths;
- lifecycle operation architecture;
- shared-project validation boundaries;
- CI/server enforcement.

Group related decisions appropriately.

Do not mechanically create one ADR per bullet.

---

# 65. Required EDF Feedback

If ProjectConcord cannot deterministically enforce canonical integrity because EDF lacks sufficient machine-readable semantics, document that explicitly.

Potential feedback to EDF may include requirements for:

```text
Artifact Schema

Governed Fields

Lifecycle Schema

Transition Rules

Relationship Schema

Cardinality Rules

Validation Rules

Supersession Rules

Deletion Rules

Acceptance Rules
```

ProjectConcord may become a valuable mechanism for discovering where EDF is clear to humans but insufficiently precise for deterministic tooling.

Such discoveries should feed back into EDF rather than being hidden inside ProjectConcord implementation code.

---

# 66. Required CRA Feedback

Likewise, if ProjectConcord exposes unresolved foundational questions concerning:

- identity continuity;
- representation equivalence;
- canonical relationship identity;
- relationship continuity;
- canonical semantic serialization;
- contextual identity;
- representation-independent fingerprints;

identify them as CRA architectural questions.

ProjectConcord must not establish a competing general theory merely to complete implementation.

---

# 67. Required Cursor Planning Procedure

Cursor should process this document in **Plan mode**.

## Step 1 — Locate This Document

Find this Markdown document in the ProjectConcord repository root.

The location is temporary.

## Step 2 — Review ProjectConcord

Inspect current ProjectConcord architecture and implementation planning.

## Step 3 — Review EDF

Inspect authoritative EDF rules governing canonical artifacts, lifecycle, validation, relationships, and repository structure.

## Step 4 — Review CRA

Inspect applicable CRA architecture concerning identity, relationships, representation, and context.

## Step 5 — Classify This Material

Determine its correct EDF artifact type and permanent disposition.

## Step 6 — Move / Rename / Retitle

If retained as a permanent artifact:

- move it to the EDF-prescribed location;
- assign an EDF-compliant filename;
- assign an EDF-compliant title;
- apply required EDF metadata/header.

## Step 7 — Identify Existing Documents to Amend

Do not merely add this document.

Propagate its requirements into existing ProjectConcord architecture where appropriate.

## Step 8 — Identify Architectural Gaps

Classify discoveries as:

```text
CRA GAP

EDF GAP

PROJECTCONCORD ARCHITECTURAL DECISION

IMPLEMENTATION DECISION
```

## Step 9 — Propose Incremental Implementation

Prioritize deterministic integrity capabilities before advanced AI or distributed enforcement.

## Step 10 — Define Validation Strategy

Include unit, integration, repository, concurrency, external-edit, relationship, lifecycle, and reconciliation testing.

## Step 11 — Return for Review

Return:

- architectural plan;
- affected-document analysis;
- proposed ADRs;
- EDF gaps;
- CRA gaps;
- implementation stages;
- validation strategy;

for architectural review before major implementation.

---

# 68. Final Architectural Principle

ProjectConcord must preserve the openness of EDF repositories without confusing openness with trust.

The core principle is:

> **Canonical Markdown remains open, portable, human-readable, Git-friendly, and externally editable — but changes to governed canonical engineering state must be detected, validated, authorized where required, reconciled, and explicitly adopted before ProjectConcord treats them as trusted canonical state.**

Conceptually:

```text
Open Repository
      |
      v
External or ProjectConcord Change
      |
      v
Detect
      |
      v
Understand
      |
      v
Validate
      |
      v
Authorize
      |
      v
Reconcile
      |
      v
Trusted Canonical State
```

This preserves both essential goals:

```text
Repository Freedom
        +
Canonical Integrity
```

rather than sacrificing one to achieve the other.

ProjectConcord should know not merely **what the Markdown files currently say**, but whether the engineering state they represent remains **valid, coherent, authorized, and trustworthy**.