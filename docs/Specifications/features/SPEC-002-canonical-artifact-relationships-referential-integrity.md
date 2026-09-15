[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Specifications](../README.md) › SPEC-002

# SPEC-002: Canonical Artifact Relationships and Referential Integrity

## Metadata

| Field | Value |
|---|---|
| **Spec ID** | SPEC-002 |
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
- [ADR-0007 Semantic Artifact Identity](../../Architecture/ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md)
- [EDF Gap Register](../../Development/EDF_Gap_Register.md) — GAP-005, GAP-009, GAP-015, GAP-016
- [PCON-0000](../../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)
- [CRA Alignment and Responsibility Boundaries](../../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md)
- [ADR-0008 CRA/CKES boundary](../../Architecture/ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md)
- [SPEC-003 Canonical Artifact Integrity](SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)

## CRA grounding

Referential integrity in SPEC-002 **operationalizes EDF** in a way that is **CRA-aligned** but does not implement CRA or CKES. Responsibility boundaries: [CRA Alignment and Responsibility Boundaries](../../Architecture/CRA_Alignment_and_Responsibility_Boundaries.md). Dependency rule: [ADR-0008](../../Architecture/ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md).

## Relationship to SPEC-003

SPEC-002 covers semantic **relationships**, registry/index, resolvers, and move/rename referential impact. **Lifecycle authorization**, **trusted integrity state**, **external change classification**, and **fingerprints** are normative in [SPEC-003](SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) ([ADR-0011](../../Architecture/ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md)). Do not duplicate SPEC-003 rules here.

---

# 1. Handover Instructions to Cursor AI

**Handover status (2026-09-15):** Steps 1–3 and initial integration applied — this file is [`SPEC-002-canonical-artifact-relationships-referential-integrity.md`](SPEC-002-canonical-artifact-relationships-referential-integrity.md). Remaining steps 4–9: architectural review via [EGR-G0](../../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md); implementation gated per [Implementation Roadmap](../../Development/Implementation_Roadmap.md).

Original handover checklist (historical):

This document was supplied at the **ProjectConcord repository root** as architectural source material.

Cursor AI must:

Do NOT assume the filename supplied by the user is canonical.

Do NOT assume the title supplied in this handover is canonical.

Do NOT assume the repository root is the correct EDF location.

Do NOT silently invent EDF rules when EDF is ambiguous.

Where EDF does not provide sufficient machine-readable semantics to implement these requirements deterministically, identify the EDF gap for architectural review.

---

# 2. Purpose

ProjectConcord must maintain relationships among canonical EDF Markdown artifacts even when those artifacts are:

- created;
- edited;
- moved;
- renamed;
- superseded;
- externally modified;
- reorganized by EDF evolution;
- modified by Cursor or another AI coding environment;
- modified through Git operations;
- modified manually.

The system must not treat a Markdown filesystem path as equivalent to the semantic identity of an engineering artifact.

The core principle is:

> **Artifact relationships are semantic. Markdown links and filesystem paths are representations of those relationships.**

For example:

```text
SPEC-0032
    |
    +-- governed-by --> ADR-0017
```

is the semantic relationship.

A Markdown representation such as:

```markdown
[ADR-0017](../Architecture/Decisions/ADR-0017-Geometry-Kernel.md)
```

is merely one filesystem-dependent representation of that relationship.

If the location or filename of ADR-0017 changes, ADR-0017 remains the same engineering artifact unless EDF semantics indicate otherwise.

ProjectConcord must therefore be capable of maintaining referential integrity independently from incidental filesystem location.

---

# 3. Architectural Motivation

ProjectConcord is intended to provide canonical CRUD and structured authoring for EDF artifacts.

This introduces operations such as:

```text
CreateArtifact(...)
UpdateArtifact(...)
MoveArtifact(...)
RenameArtifact(...)
SupersedeArtifact(...)
DeleteArtifact(...)
ChangeStatus(...)
AddRelationship(...)
RemoveRelationship(...)
```

Many of these operations can affect references in other documents.

For example:

```text
ADR-0017.md
      |
      +------ referenced by ------> SPEC-0032.md
      |
      +------ referenced by ------> SPEC-0038.md
      |
      +------ referenced by ------> M10
      |
      +------ referenced by ------> AWI-0043
```

Moving or renaming ADR-0017 may invalidate multiple Markdown links.

ProjectConcord must understand these relationships before modifying the filesystem.

---

# 4. Identity Must Be Distinct from Location

A fundamental architectural principle should be:

```text
Artifact Identity != Filename
Artifact Identity != Directory
Artifact Identity != Relative Path
Artifact Identity != Markdown Link
```

Instead:

```text
                 Canonical Identity
                      ADR-0017
                          |
             +------------+------------+
             |            |            |
             v            v            v
          Markdown     UI Entity    Project Graph
             |
             v
      Filesystem Location
```

The filesystem location is a property or representation of the artifact.

It must not become the artifact's semantic identity unless EDF explicitly defines otherwise.

This principle is compatible with the broader Canonical Representation Architecture (CRA) principle that identity should remain independent from representation and location.

ProjectConcord must not require CRA or CKES as an initial dependency merely to implement this principle.

---

# 5. EDF Identity Requirement

Cursor must determine how the current EDF specification defines artifact identity.

Potential examples may include:

```text
ADR-0017
SPEC-0032
AWI-0043
M10
```

However, ProjectConcord MUST NOT assume that all EDF artifacts currently have sufficient stable identifiers.

Cursor must investigate:

- which EDF artifact types have stable identifiers;
- whether those identifiers are globally unique;
- whether they are unique only within artifact type;
- whether project scope contributes to identity;
- whether filenames currently encode identity;
- whether metadata contains identity;
- whether identity survives move/rename;
- whether identity survives document-title changes;
- how supersession affects identity;
- whether nested/subordinate artifacts require identities;
- whether relationships currently reference identity or paths.

If EDF lacks sufficient stable semantic identity for independently referenceable artifacts, this must be reported as an **EDF architectural gap**.

ProjectConcord must not silently invent permanent EDF identity policy.

---

# 6. Artifact Registry

ProjectConcord should maintain a runtime/derived **Artifact Registry**.

Conceptually:

```text
Artifact Registry

ADR-0017
    Type: Architectural Decision
    Title: Geometry Kernel Provider Strategy
    Location:
      docs/Architecture/Decisions/ADR-0017-Geometry-Kernel.md

SPEC-0032
    Type: Specification
    Title: Sketch Placement
    Location:
      docs/Specifications/SPEC-0032-Sketch-Placement.md
```

The Artifact Registry should normally be derived from canonical EDF artifacts.

It should NOT become an independent competing canonical database unless EDF explicitly defines such a registry in the future.

The registry may be cached for performance.

ProjectConcord must be capable of rebuilding it from canonical repository information.

---

# 7. Relationship Index

ProjectConcord should maintain a derived **Relationship Index**.

Example:

```text
ADR-0017

Incoming Relationships:

    SPEC-0032
        governed-by -> ADR-0017

    SPEC-0038
        governed-by -> ADR-0017

    M10
        governed-by -> ADR-0017


Outgoing Relationships:

    ADR-0017
        supersedes -> ADR-0008
```

Another representation may be:

```text
SOURCE       RELATIONSHIP       TARGET

SPEC-0032    governed-by        ADR-0017
SPEC-0038    governed-by        ADR-0017
M10          governed-by        ADR-0017
ADR-0017     supersedes         ADR-0008
```

The exact internal representation is an implementation decision.

The architectural requirement is that ProjectConcord can efficiently answer:

```text
What does this artifact reference?

What references this artifact?

What type of relationship exists?

Where is the target currently located?

Which Markdown links represent the relationship?

Would moving this artifact break anything?
```

---

# 8. Relationship Identity vs Markdown Link

ProjectConcord should conceptually distinguish:

```text
Semantic Relationship

SPEC-0032
    governed-by
ADR-0017
```

from:

```text
Markdown Representation

[ADR-0017](../Architecture/ADR-0017.md)
```

The first represents engineering meaning.

The second represents navigation in one particular repository layout.

This distinction is critical.

ProjectConcord should resolve semantic identity into the appropriate Markdown representation when generating canonical artifacts.

---

# 9. Identity Resolver

Introduce the architectural concept of an **Identity Resolver**.

Conceptually:

```text
Artifact Reference
      |
      v
Identity Resolver
      |
      v
Canonical Artifact Identity
      |
      v
Artifact Registry
      |
      v
Current Artifact Location
```

The resolver should allow ProjectConcord to determine the current canonical artifact corresponding to an EDF reference.

The resolver must detect situations such as:

- missing identity;
- duplicate identity;
- ambiguous identity;
- malformed identity;
- unsupported identity type;
- identity referring to a superseded artifact where special handling applies.

---

# 10. Link Resolver

Introduce the architectural concept of a **Link Resolver**.

Conceptually:

```text
Source Artifact
       |
       v
Semantic Target Identity
       |
       v
Artifact Registry
       |
       v
Target Location
       |
       v
Calculate Appropriate Link
       |
       v
Markdown Representation
```

For example:

```text
Source:
docs/Specifications/SPEC-0032.md

Target:
ADR-0017

Resolved Location:
docs/Architecture/Decisions/ADR-0017.md
```

may generate:

```markdown
[ADR-0017](../Architecture/Decisions/ADR-0017.md)
```

The AI should not normally need to calculate this relative path.

The deterministic Link Resolver should do so.

---

# 11. Referential Integrity Service

ProjectConcord should provide a **Referential Integrity** capability responsible for verifying that relationships remain valid.

Conceptually:

```text
                   EDF Engine
                       |
                       v
                Artifact Registry
                       |
                       v
               Relationship Index
                /             \
               v               v
       Identity Resolver    Link Resolver
                \             /
                 \           /
                  v         v
               Referential
                Integrity
```

Responsibilities may include:

- detecting broken Markdown links;
- detecting unresolved EDF identities;
- detecting stale paths;
- detecting duplicate identities;
- detecting ambiguous identities;
- validating relationship types;
- validating required reciprocal relationships where EDF requires them;
- determining incoming references before destructive operations;
- verifying references after move/rename;
- validating references after external repository changes.

---

# 12. Relationship Types

ProjectConcord should not assume that all links represent the same relationship.

Examples may include:

```text
governed-by
implements
supersedes
blocks
blocked-by
advances
resolves
validated-by
accepted-by
depends-on
references
related-to
```

These are illustrative only.

Cursor must determine which relationships EDF currently defines and how they are represented.

If EDF does not formally define relationship semantics sufficiently for ProjectConcord, identify this as an EDF gap.

Do not create a permanent relationship vocabulary solely because it is convenient for the application.

---

# 13. Markdown Links That Are Not EDF Relationships

Not every Markdown hyperlink necessarily represents a formal EDF relationship.

Examples may include:

- external websites;
- explanatory references;
- source citations;
- images;
- diagrams;
- repository resources;
- informal cross-references.

ProjectConcord should distinguish, where possible, between:

```text
Formal EDF Relationship
```

and:

```text
Ordinary Markdown Hyperlink
```

Both may require path maintenance if they point to repository files.

However, they should not automatically have the same semantic meaning.

---

# 14. Move Operation

When ProjectConcord moves a canonical artifact, the operation should conceptually perform:

```text
Move Requested
      |
      v
Resolve Artifact Identity
      |
      v
Determine Incoming References
      |
      v
Determine Outgoing References
      |
      v
Determine Affected Markdown Links
      |
      v
Validate Proposed Destination
      |
      v
Generate Change Set
      |
      v
User Review
      |
      v
Apply Move
      |
      v
Rewrite Affected Links
      |
      v
Rebuild/Update Registry
      |
      v
Validate Referential Integrity
      |
      v
Validate EDF
```

The user should not need to manually repair relative links.

---

# 15. Rename Operation

Rename should behave similarly to move.

Example:

```text
Current:

docs/Architecture/ADR-0017.md


Requested:

docs/Architecture/ADR-0017-Geometry-Kernel-Provider.md
```

ProjectConcord should determine every affected repository reference before applying the operation.

The UI may present:

```text
Rename ADR-0017

This operation affects:

    1 canonical artifact
    4 referencing artifacts
    7 Markdown links

Affected:

    SPEC-0032
    SPEC-0038
    M10
    AWI-0043

[Preview Changes]
```

---

# 16. Delete Operation

Before deletion, ProjectConcord must determine incoming relationships.

Example:

```text
Delete ADR-0017?

Referenced by:

    SPEC-0032
    SPEC-0038
    M10
    AWI-0043
```

EDF lifecycle rules may prohibit deletion.

ProjectConcord may instead recommend:

```text
Supersede
Archive
Deprecate
Cancel
```

depending on EDF semantics.

Do not define those lifecycle behaviors in the UI without EDF authority.

---

# 17. Supersession

Supersession must preserve historical relationships.

Example:

```text
ADR-0017
    superseded-by -> ADR-0029
```

Existing historical artifacts may legitimately continue referencing ADR-0017.

New artifacts may need to reference ADR-0029.

ProjectConcord should not automatically rewrite every historical relationship merely because an artifact has been superseded.

Cursor must determine EDF policy for this behavior.

If EDF does not specify it, report the ambiguity.

---

# 18. External File Changes

ProjectConcord must assume that repository artifacts can be changed outside the application.

Potential actors include:

- Cursor AI;
- another AI coding environment;
- IDE;
- text editor;
- Git;
- command-line scripts;
- filesystem operations;
- another developer in a future collaborative environment.

Example:

```text
File removed:

docs/Architecture/ADR-0017.md


File created:

docs/Architecture/Decisions/ADR-0017.md
```

If the canonical identity contained in both documents is ADR-0017, ProjectConcord should be capable of recognizing this as a probable move rather than necessarily treating it as:

```text
Delete ADR-0017

Create unrelated new artifact
```

---

# 19. External Move Detection

Conceptually:

```text
Filesystem Change
       |
       v
Old Artifact Missing
       |
       +
       |
New Artifact Discovered
       |
       v
Compare Canonical Identity
       |
       v
Same Identity?
       |
       +---- YES ----> External Move/Rename
       |
       +---- NO -----> Independent Delete/Create
```

Additional signals such as Git rename detection may assist.

Canonical identity should remain the strongest semantic signal where EDF supports it.

---

# 20. Stale Link Repair

After an external move, ProjectConcord may detect:

```text
Artifact ADR-0017 moved externally.

Old:
docs/Architecture/ADR-0017.md

New:
docs/Architecture/Decisions/ADR-0017.md

5 repository links still reference the old location.
```

The application should allow:

```text
[Review]
[Repair Links]
[Ignore]
```

Repair should be performed through the canonical authoring/referential-integrity services rather than blind string replacement.

---

# 21. Broken Reference Classification

ProjectConcord validation should distinguish among different problems.

Examples:

## Broken Markdown Link

The target filesystem resource does not exist.

## Unresolved EDF Reference

The referenced semantic artifact identity cannot be found.

## Stale Path

The target artifact exists but has moved.

## Duplicate Identity

More than one canonical artifact claims the same identity.

## Ambiguous Reference

The system cannot determine which artifact is intended.

## Invalid Relationship

The source and target exist, but the relationship violates applicable EDF rules.

## Missing Required Relationship

EDF requires a relationship that is absent.

## Reciprocal Relationship Problem

EDF requires corresponding relationship information that is missing or inconsistent.

These categories should be refined according to actual EDF semantics.

---

# 22. Transactional Change Sets

Operations affecting multiple files should be modeled as coherent **ProjectConcord Change Sets**.

Example:

```text
CHANGE SET

Operation:
Move ADR-0017

Repository Changes:

    Move:
        1 file

    Modify:
        4 artifacts

    Rewrite:
        7 Markdown links

Validation:

    No unresolved identities
    No broken links
    No duplicate identities
    EDF conformance maintained
```

The user should be able to inspect the change set before applying it.

Where practical, ProjectConcord should avoid leaving the repository partially modified if an operation fails.

The implementation strategy for transaction/rollback behavior should be determined during architecture design.

---

# 23. Git and Change Sets

Git provides a natural review mechanism.

After an operation, ProjectConcord should be able to show a coherent diff representing:

```text
Artifact Move
+
Reference Updates
+
Metadata Changes
```

The application should not attempt to replace Git.

ProjectConcord change sets and Git commits are related concepts but should remain distinct.

A change set may eventually be committed as one Git commit, but this should not be assumed as mandatory.

---

# 24. Canonical Authoring Integration

The Canonical Authoring Service should use the Artifact Registry and Relationship/Link services.

Conceptually:

```text
Structured UI / AI
        |
        v
Canonical Authoring Service
        |
        +-------------------+
        |                   |
        v                   v
   EDF Rules          Artifact Registry
        |                   |
        v                   v
Relationship Rules    Identity Resolver
        |                   |
        +---------+---------+
                  |
                  v
             Link Resolver
                  |
                  v
          Canonical Markdown
```

The user or AI should generally specify semantic relationships.

The system should generate filesystem-dependent Markdown representations.

---

# 25. AI Integration

AI should reason in terms of artifact identities and semantic relationships whenever possible.

Preferred AI context:

```text
SPEC-0032
    governed-by -> ADR-0017

M10
    implements -> SPEC-0032
```

rather than:

```text
../../Architecture/ADR-0017.md
```

The AI may request:

```text
Add a governed-by relationship from SPEC-0032 to ADR-0017.
```

The Canonical Authoring Service should determine how EDF requires that relationship to be represented.

The AI should not be responsible for calculating relative filesystem paths.

---

# 26. AI-Generated Documents

When AI creates a new artifact, it may propose semantic references such as:

```text
Related ADR:
ADR-0017

Implements:
SPEC-0032

Milestone:
M10
```

ProjectConcord should resolve these through deterministic services.

Before saving:

```text
AI Draft
    |
    v
Resolve Identities
    |
    v
Validate Relationships
    |
    v
Generate Links
    |
    v
EDF Validation
    |
    v
User Review
    |
    v
Canonical Markdown
```

This reduces AI errors and token usage.

---

# 27. Code-to-Documentation Reconciliation Integration

The Change Impact Engine and Documentation Reconciliation subsystem will depend heavily on semantic relationships.

Example:

```text
Changed Code
     |
     v
Task T-184
     |
     +-- implements ---> SPEC-0032
     |
     +-- governed-by --> ADR-0017
     |
     +-- advances -----> M10
```

ProjectConcord should follow semantic relationships rather than attempting to discover context purely through filenames or Markdown path strings.

This relationship model is therefore foundational to economical AI-assisted reconciliation.

---

# 28. File Monitoring Integration

The file-monitoring subsystem should update the Artifact Registry and Relationship Index incrementally.

Conceptually:

```text
Repository Change
       |
       v
Determine Affected Files
       |
       v
Parse Changed Artifacts
       |
       v
Update Artifact Registry
       |
       v
Update Relationship Index
       |
       v
Check Referential Integrity
       |
       v
Update Project Graph
       |
       v
Refresh Validation/UI
```

A complete repository rebuild should remain available when incremental state cannot be trusted.

---

# 29. Project Graph Integration

The EDF Project Graph should use semantic artifact identity.

For example:

```text
M10
 |
 +-- governed-by --> ADR-0017
 |
 +-- implements ---> SPEC-0032
 |
 +-- blocked-by ----> G-SKETCH
 |
 +-- resolves ------> AWI-0043
```

The graph should not fundamentally consist of filesystem paths.

Filesystem paths are resolved properties of graph entities.

---

# 30. Future Web / Multi-User Considerations

The Artifact Registry, Relationship Index, Identity Resolver, Link Resolver, and Referential Integrity services should remain independent of Avalonia.

A future web architecture may use the same semantic services.

This becomes particularly important when:

- several users edit the project;
- repositories are server-managed;
- users do not have direct filesystem access;
- canonical authoring occurs through web services;
- concurrent edits occur;
- audit history becomes important.

Do not implement full multi-user concurrent authoring in the MVP SPEC-002 delivery slice. Preserve service boundaries so shared project services can enforce change sets and conflict detection later ([ADR-0009](../../Architecture/ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md)).

Preserve the appropriate boundaries.

---

# 31. Potential Service Boundaries

Cursor should evaluate architectural concepts such as:

```text
EDF Engine
    |
    +-- Artifact Discovery
    +-- Artifact Classification
    +-- Identity Resolution
    +-- Relationship Resolution
    +-- Referential Integrity
    +-- Validation

Canonical Authoring
    |
    +-- Create
    +-- Update
    +-- Move
    +-- Rename
    +-- Supersede
    +-- Delete
    +-- Relationship Editing
    +-- Change Sets

Repository Services
    |
    +-- Filesystem
    +-- Monitoring
    +-- Git
```

The exact class/interface/project boundaries are not prescribed by this document.

Cursor should propose the simplest architecture that preserves the required semantics.

---

# 32. Derived State and Persistence

The following should normally be considered derived unless EDF specifies otherwise:

```text
Artifact Registry
Relationship Index
Reverse Reference Index
Resolved Filesystem Paths
Project Graph
Broken-Link Index
Search Index
UI Navigation Model
```

ProjectConcord may persist/cache these for performance.

However:

> Deleting the ProjectConcord cache should not destroy canonical project knowledge.

ProjectConcord must be capable of rebuilding derived state from canonical repository artifacts.

---

# 33. Repository Portability

A cloned EDF repository should retain its engineering meaning without ProjectConcord-specific local state.

For example:

```text
git clone ...
```

followed by:

```text
Open Project in ProjectConcord
```

should allow ProjectConcord to reconstruct:

- artifact identities;
- artifact locations;
- relationships;
- references;
- project graph;
- validation state.

This is an important acceptance principle.

---

# 34. EDF Machine-Readable Relationship Requirements

This feature may expose a need for stronger machine-readable relationship semantics in EDF.

Cursor must investigate whether EDF currently provides enough information to deterministically answer:

```text
What is this artifact's stable identity?

What artifact does this reference mean?

What relationship type exists?

Is this relationship permitted?

Is the relationship directional?

Does it require reciprocity?

Can this artifact be moved?

Can this artifact be renamed?

What happens to references when it is superseded?

Can it be deleted?

Which links are formal EDF relationships?
```

If these questions cannot be answered reliably from EDF, do not hide the problem inside ProjectConcord.

Document the deficiency.

---

# 35. Potential EDF Improvement Areas

This architecture may indicate future EDF enhancements involving:

- stable artifact identity;
- machine-readable artifact type;
- relationship vocabulary;
- relationship directionality;
- relationship cardinality;
- lifecycle semantics;
- supersession semantics;
- move/rename semantics;
- referential-integrity requirements;
- structured artifact schemas;
- explicit distinction between identity and path.

These are not automatically approved EDF changes.

They are areas Cursor should evaluate and report.

---

# 36. Relationship to CRA

This specification intentionally reflects an important CRA principle:

> **Identity is canonical; representation and location are distinct concerns.**

ProjectConcord should remain compatible with that principle.

However:

- ProjectConcord should not require CRA for its initial operation;
- ProjectConcord should not invent CRA behavior that EDF does not authorize;
- future CRA/CKES integration may provide stronger semantic identity and relationship infrastructure.

The immediate requirement is that ProjectConcord avoid architecture that equates semantic artifact identity with filesystem location.

---

# 37. Initial Implementation Scope

The first implementation does not require every advanced relationship feature.

A reasonable progression may be:

```text
1. Artifact discovery
2. Stable identity extraction
3. Artifact Registry
4. Markdown link discovery
5. Relationship Index
6. Broken-link detection
7. Reverse-reference lookup
8. Safe rename
9. Safe move
10. Referential-integrity validation
11. Canonical Authoring integration
12. AI semantic-reference integration
13. External move detection
14. Transactional change sets
15. Advanced relationship validation
```

Cursor should adjust this sequence according to EDF and implementation dependencies.

---

# 38. Initial Acceptance Scenario

A useful early acceptance scenario should be:

1. Open an existing EDF project.
2. ProjectConcord discovers ADR-0017.
3. ProjectConcord discovers several documents linking to ADR-0017.
4. The user requests that ADR-0017 be moved or renamed.
5. ProjectConcord identifies all affected references.
6. ProjectConcord displays the proposed change set.
7. The user approves the operation.
8. ProjectConcord moves/renames the artifact.
9. ProjectConcord updates affected Markdown links.
10. ProjectConcord validates the repository.
11. No stale references remain.
12. Git shows a coherent reviewable diff.

A second acceptance scenario should test external modification:

1. ProjectConcord has an EDF project open.
2. Cursor or another tool moves ADR-0017.
3. ProjectConcord detects the repository change.
4. ProjectConcord identifies the artifact through stable identity.
5. ProjectConcord recognizes a probable move/rename.
6. ProjectConcord detects stale incoming links.
7. ProjectConcord offers to repair them.
8. The user approves.
9. Referential integrity is restored.

---

# 39. Critical Architectural Rule

The following distinction MUST be preserved:

```text
Artifact Identity
        !=
Filesystem Location
        !=
Markdown Link
```

Instead:

```text
              Artifact Identity
                     |
                     v
             Semantic Relationship
                     |
                     v
               Link Resolution
                     |
                     v
             Markdown Representation
                     |
                     v
              Filesystem Location
```

This distinction is foundational to ProjectConcord's ability to safely manage EDF documentation.

---

# 40. Required Cursor Planning Actions

Cursor should process this document in **Plan mode**.

## Step 1 — Find This File

Locate this supplied Markdown document in the **ProjectConcord repository root**.

## Step 2 — Inspect EDF

Review the current authoritative Engineering Documentation Framework.

Determine how this architectural material should be classified.

## Step 3 — Move and Rename This Document

The repository root is temporary.

Determine and apply/propose:

- the proper EDF directory;
- an EDF-compliant filename;
- an EDF-compliant title;
- required EDF metadata/header;
- relationships to existing ProjectConcord documents.

Do not preserve the supplied filename/title merely because they were provided here.

## Step 4 — Review Existing ProjectConcord Architecture

Determine whether portions of this specification overlap with existing:

- architecture documents;
- specifications;
- ADRs;
- development plans;
- requirements.

Avoid unnecessary duplication.

## Step 5 — Identify Required ADRs

Determine which architectural decisions require formal ADR treatment.

Potential candidates include:

- identity independent of filesystem location;
- derived Artifact Registry;
- derived Relationship Index;
- semantic relationships vs Markdown links;
- referential-integrity architecture;
- transactional multi-file change sets.

Do not assume each requires a separate ADR.

Apply EDF governance.

## Step 6 — Identify EDF Gaps

Explicitly identify where current EDF semantics are insufficient for deterministic implementation.

Do not silently invent rules.

## Step 7 — Integrate With Existing Architecture Plan

Ensure this capability is incorporated into planning for:

- EDF Engine;
- Project Graph;
- Canonical Authoring;
- CRUD;
- validation;
- file monitoring;
- Git;
- AI authoring;
- Change Impact Engine;
- documentation reconciliation;
- future web architecture.

## Step 8 — Propose Incremental Implementation

Do not attempt to build the entire relationship system at once.

Propose appropriate milestones, dependencies, tests, and validation gates.

## Step 9 — Stop for Architectural Review

Do not proceed directly into major implementation.

Return the updated Plan for architectural review.

---

# 41. Final Guiding Principle

ProjectConcord should allow EDF artifacts to move through an evolving repository without losing their engineering identity or relationships.

The system should understand:

> **what an artifact is**

before reasoning about:

> **where its Markdown file happens to be located.**

Semantic relationships should drive canonical link generation.

Canonical Markdown should remain portable, readable, Git-trackable, and usable without ProjectConcord.

ProjectConcord's derived relationship infrastructure should make those canonical artifacts safer and easier to maintain without becoming a competing source of truth.