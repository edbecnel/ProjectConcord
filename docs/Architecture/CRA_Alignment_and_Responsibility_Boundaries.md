[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › CRA Alignment and Responsibility Boundaries

# CRA Alignment and Responsibility Boundaries

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architecture — cross-cutting responsibility boundaries |
| **Normative** | Yes — for ProjectConcord planning and implementation |
| **Status** | Draft |
| **Owner** | ProjectConcord |
| **Last Reviewed** | 2026-09-15 |
| **Related** | CRA, EDF [CRA_CKES_EDF_Boundaries](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/main/docs/Architecture/CRA_CKES_EDF_Boundaries.md) |
| **Former location** | Repository root (handover) |
| **Permanent path** | `docs/Architecture/CRA_Alignment_and_Responsibility_Boundaries.md` |

## Parent

- [Architecture](README.md)

## Related Documents

- [System Architecture Overview](System_Architecture_Overview.md)
- [SPEC-002 — Referential Integrity](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md)
- [SPEC-003 — Canonical Artifact Integrity](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md)
- [ADR-0007 — Semantic Artifact Identity](ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md)
- [ADR-0008 — CRA and CKES Dependency Boundary](ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md)
- [PCON-0000](PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md)

---

# 1. Instructions to Cursor AI

**Handover status (2026-09-15):** Classification: architecture domain (normative boundaries). Moved to [`CRA_Alignment_and_Responsibility_Boundaries.md`](CRA_Alignment_and_Responsibility_Boundaries.md). Integrated with [SPEC-002](../Specifications/features/SPEC-002-canonical-artifact-relationships-referential-integrity.md), [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md), [ADR-0007](ADRs/ADR-0007-Semantic-Artifact-Identity-and-Referential-Integrity.md), [ADR-0008](ADRs/ADR-0008-CRA-and-CKES-Dependency-Boundary.md), [ADR-0011](ADRs/ADR-0011-Canonical-Artifact-Integrity-and-Trusted-State.md). Review via [EGR-G0](../Program/Gate_Reviews/EGR-G0-Architecture-Planning-Gate.md).

**Canonical integrity:** CRA informs identity and representation fidelity (CRA-0001–0003). [SPEC-003](../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) operationalizes EDF for authorized state, external edits, and trusted integrity records at the ProjectConcord layer. Integrity gaps feed [CRA ↔ ProjectConcord Gap Analysis](../Development/CRA_ProjectConcord_Gap_Analysis.md) and [EDF Gap Register](../Development/EDF_Gap_Register.md) without inventing competing CRA theory.

Original handover checklist (historical):

This Markdown document was supplied in the **ProjectConcord repository root** for handover purposes.

Cursor AI must:

Do NOT assume:

- the repository root is the correct permanent location;
- the supplied filename is EDF-compliant;
- the supplied title is EDF-compliant;
- this document should remain a single document if EDF requires another treatment.

Where architectural decisions require formal governance, identify and propose the appropriate ADRs.

Where EDF or CRA is ambiguous, identify the ambiguity rather than silently inventing ProjectConcord-specific rules.

---

# 2. Purpose

ProjectConcord is developing capabilities involving:

- canonical EDF artifact identity;
- semantic relationships among EDF artifacts;
- relationship mapping;
- artifact registries;
- relationship indexes;
- Markdown link resolution;
- referential integrity;
- safe move/rename operations;
- AI-assisted canonical authoring;
- code-to-documentation reconciliation.

These capabilities overlap directly with architectural principles already being developed through the **Canonical Representation Architecture (CRA)**.

ProjectConcord MUST NOT independently invent another general-purpose canonical identity and relationship architecture where CRA already defines or is intended to define those concepts.

This document establishes the architectural responsibility boundary among:

```text
CRA
 |
 v
EDF
 |
 v
ProjectConcord
```

---

# 3. Foundational Principle

The intended architectural hierarchy is:

> **CRA defines foundational canonical representation principles. EDF applies appropriate canonical concepts to engineering documentation and engineering project governance. ProjectConcord operationalizes those concepts for users and engineering projects.**

Conceptually:

```text
                 CRA
      Foundational Canonical Architecture
                    |
                    v
                  EDF
      Engineering-Domain Semantics
                    |
                    v
             ProjectConcord
       Operational Application
```

ProjectConcord should therefore consume and apply applicable CRA principles rather than creating competing foundational semantics.

---

# 4. CRA Responsibility

CRA should remain responsible for general architectural concepts concerning canonical representation.

Applicable areas include, where defined by CRA:

- canonical identity;
- representation-independent identity;
- canonical relationships;
- relationship identity;
- relationship definitions;
- relationship instances;
- relationship semantics;
- contextual relationships;
- relationship cardinality;
- directionality;
- canonical references;
- neutral references;
- representation derivation;
- location-independent identity;
- context separation.

These concepts are not specific to engineering documentation.

They potentially apply across many systems.

ProjectConcord should therefore avoid redefining them solely for EDF Markdown documents.

---

# 5. EDF Responsibility

EDF should define the **engineering-domain application** of appropriate canonical concepts.

For example, EDF may define engineering artifact types such as:

```text
Architectural Decision
Specification
Milestone
Gate
AWI
Validation Record
Acceptance Record
Development Plan
Report
```

EDF may also define engineering-domain relationships such as:

```text
governed-by
implements
supersedes
blocks
advances
resolves
validated-by
accepted-by
```

These relationship names are illustrative unless already formally defined by EDF.

EDF should determine which relationships have engineering meaning and which artifact types may participate in them.

CRA provides foundational relationship architecture.

EDF provides engineering semantics.

---

# 6. ProjectConcord Responsibility

ProjectConcord should operationalize the applicable CRA and EDF rules.

For example:

```text
User:
Move ADR-0017

ProjectConcord:
    Resolve canonical identity
    Find incoming relationships
    Find outgoing relationships
    Determine affected representations
    Calculate Markdown link changes
    Validate EDF rules
    Present change set
    Apply approved changes
```

ProjectConcord should not need to invent what "identity" fundamentally means.

Nor should it independently invent whether `governed-by` is an allowed engineering relationship.

Those responsibilities belong at the appropriate CRA/EDF architectural layers.

---

# 7. Canonical Identity

The following principle should be inherited from CRA where applicable:

```text
Canonical Identity
       !=
Filesystem Location
       !=
Filename
       !=
Markdown Link
```

For example:

```text
ADR-0017
```

may identify an engineering artifact.

Its current representation may be:

```text
docs/Architecture/ADR-0017.md
```

and later:

```text
docs/Architecture/Decisions/ADR-0017-Geometry-Kernel.md
```

The representation changed.

The engineering identity did not necessarily change.

ProjectConcord should preserve this distinction.

---

# 8. Canonical Relationships

The same distinction applies to relationships.

Conceptually:

```text
SPEC-0032
    |
    +-- governed-by --> ADR-0017
```

is the semantic relationship.

A Markdown representation might be:

```markdown
[ADR-0017](../Architecture/Decisions/ADR-0017.md)
```

The Markdown link is not itself the canonical meaning of the relationship.

It is a representation that allows navigation in a particular filesystem layout.

Therefore:

> **Semantic relationships should drive Markdown link generation, rather than Markdown paths defining semantic relationships.**

---

# 9. Representation Derivation

ProjectConcord may eventually expose multiple representations of the same canonical engineering knowledge.

For example:

```text
                    Canonical Identity
                         ADR-0017
                             |
          +------------------+------------------+
          |                  |                  |
          v                  v                  v
       Markdown         Desktop UI        Future Web UI
          |
          +------------------+
                             |
                             v
                       AI Context
```

The representations may differ.

They should resolve to the same engineering concept.

This is a direct reason ProjectConcord should align with CRA.

---

# 10. Project Graph

The ProjectConcord EDF Project Graph should represent semantic entities and relationships rather than filesystem paths.

Preferred conceptual model:

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

Not:

```text
M10.md
 |
 +--> ../../Architecture/ADR-0017.md
 +--> ../../Specifications/SPEC-0032.md
```

Filesystem paths belong to representation resolution.

They should not constitute the foundational project graph.

---

# 11. Artifact Registry

ProjectConcord may maintain a derived Artifact Registry such as:

```text
Identity:
ADR-0017

Type:
Architectural Decision

Current Representation:
docs/Architecture/Decisions/ADR-0017.md
```

The registry should map canonical identity to current representations.

Unless CRA or EDF explicitly defines the registry itself as canonical, ProjectConcord's registry should remain derived.

It should be rebuildable from canonical project information.

---

# 12. Relationship Index

Similarly, ProjectConcord may maintain a derived relationship index:

```text
SOURCE       RELATIONSHIP       TARGET

SPEC-0032    governed-by        ADR-0017
M10          implements         SPEC-0032
AWI-0043     resolved-by        M10
```

The index is an operational acceleration structure.

It should not silently become an independent competing canonical knowledge store.

---

# 13. Relationship Definitions vs Relationship Instances

Cursor should preserve the distinction between:

```text
Relationship Definition

governed-by
```

and:

```text
Relationship Instance

SPEC-0032
    governed-by
ADR-0017
```

The definition may specify semantics such as:

- permitted source types;
- permitted target types;
- cardinality;
- directionality;
- inverse relationship;
- lifecycle implications;
- contextual restrictions.

The instance states that the relationship exists between particular canonical entities.

This distinction is important and should align with CRA rather than being independently designed inside ProjectConcord.

---

# 14. Relationship Cardinality

Relationship definitions may have cardinality such as:

```text
one-to-one
one-to-many
many-to-one
many-to-many
```

Cardinality belongs to relationship semantics.

ProjectConcord may validate cardinality but should not invent it.

CRA should provide the foundational relationship model.

EDF may constrain that model for specific engineering relationships.

ProjectConcord should enforce the resulting rules.

---

# 15. Relationship Lifecycle Semantics

Some relationships may carry lifecycle implications.

For example, other engineering systems sometimes distinguish between relationships that:

- merely reference another entity;
- imply dependency;
- imply ownership;
- affect deletion;
- affect validity;
- affect lifecycle.

ProjectConcord should not assume all relationships are simple hyperlinks.

However, ProjectConcord should also not invent general relationship lifecycle semantics.

These should derive from CRA and/or EDF as appropriate.

---

# 16. Markdown Is a Representation

A fundamental architectural rule for ProjectConcord should be:

> **Markdown is a canonical EDF storage representation where EDF prescribes it, but Markdown syntax itself should not be mistaken for the complete semantic model.**

For example:

```markdown
[ADR-0017](../Architecture/ADR-0017.md)
```

contains navigation information.

Additional semantic information may be required to know whether the relationship means:

```text
governed-by
references
supersedes
depends-on
```

ProjectConcord should not infer engineering semantics from arbitrary Markdown links unless EDF explicitly defines that interpretation.

---

# 17. Ordinary Links vs Canonical Relationships

ProjectConcord must distinguish where possible between:

```text
Canonical/Semantic Relationship
```

and:

```text
Ordinary Markdown Hyperlink
```

An ordinary Markdown link may point to:

- an external website;
- an image;
- another document for convenience;
- source material;
- a diagram;
- explanatory information.

That does not necessarily create a canonical engineering relationship.

EDF must define which constructs establish formal engineering relationships.

CRA defines how canonical relationships are represented architecturally.

ProjectConcord interprets and maintains them.

---

# 18. CRA Conformance vs CKES Dependency

ProjectConcord should distinguish between:

> **conforming to CRA architectural principles**

and:

> **using CKES as its implementation infrastructure.**

These are not the same requirement.

An initial ProjectConcord implementation may be:

```text
ProjectConcord
      |
      +-- CRA-aligned identity semantics
      |
      +-- CRA-aligned relationship semantics
      |
      +-- ProjectConcord-local derived indexes
      |
      +-- EDF engineering semantics
```

without requiring:

```text
ProjectConcord
      |
      v
     CKES
```

from the beginning.

---

# 19. Future CKES Integration

If CKES becomes the standard implementation of CRA concepts, ProjectConcord may later use it for capabilities such as:

- canonical identity management;
- relationship storage/resolution;
- semantic indexing;
- canonical knowledge queries;
- cross-project knowledge;
- semantic discovery.

Conceptually:

```text
ProjectConcord
      |
      v
     CKES
      |
      v
     CRA
```

However, CKES integration should occur only when architecturally justified.

Do not introduce unnecessary infrastructure into the desktop MVP merely for theoretical purity.

---

# 20. Avoid Duplicate Architecture

A major requirement is:

> **ProjectConcord must not independently create another general canonical relationship architecture if CRA already owns that problem.**

Avoid:

```text
CRA Relationship Architecture

        +

EDF Relationship Architecture

        +

ProjectConcord Relationship Architecture
```

where each independently defines identity, relationships, directionality, cardinality, and semantics.

Instead prefer:

```text
                  CRA
       Foundational Architecture
                   |
                   v
                  EDF
      Engineering-Domain Application
                   |
                   v
            ProjectConcord
       Operational Implementation
```

---

# 21. Separation of Responsibilities

The intended division should approximately be:

| Concern | CRA | EDF | ProjectConcord |
|---|---|---|---|
| Canonical identity principles | Primary | Applies | Implements |
| Representation independence | Primary | Applies | Implements |
| General relationship model | Primary | Applies | Implements |
| Relationship identity | Primary | Applies | Implements |
| Relationship cardinality model | Primary | Constrains | Validates |
| Engineering artifact types | — | Primary | Implements |
| Engineering relationship vocabulary | — | Primary | Implements |
| EDF document structure | — | Primary | Implements |
| EDF naming/location rules | — | Primary | Implements |
| EDF lifecycle rules | — | Primary | Implements |
| Markdown generation | — | Defines requirements | Performs |
| Link resolution | Principles | Defines semantics | Performs |
| Referential integrity | Principles | Defines requirements | Performs |
| Structured authoring UI | — | — | Primary |
| AI-assisted authoring | — | — | Primary |
| Change reconciliation | — | Governs engineering meaning | Primary |
| Project dashboard | — | — | Primary |

This table is architectural guidance, not a substitute for reviewing the actual CRA and EDF specifications.

---

# 22. Implications for Canonical Authoring

ProjectConcord's Canonical Authoring Service should operate conceptually as:

```text
User / AI Intent
       |
       v
EDF Artifact Semantics
       |
       v
CRA-Aligned Identity / Relationships
       |
       v
EDF Validation
       |
       v
Representation Resolver
       |
       v
Canonical Markdown
```

The user or AI should normally express:

```text
SPEC-0032 governed-by ADR-0017
```

rather than manually calculating:

```text
../../Architecture/Decisions/ADR-0017.md
```

ProjectConcord should generate the appropriate representation.

---

# 23. Implications for AI

This separation is particularly important for AI economics and reliability.

AI should reason about:

```text
Artifact:
SPEC-0032

Relationship:
governed-by

Target:
ADR-0017
```

Deterministic ProjectConcord services should handle:

```text
Current target location
Relative path calculation
Markdown syntax
EDF naming
EDF location
Reference validation
```

This reduces AI context requirements and prevents AI from being used for deterministic filesystem mechanics.

---

# 24. Implications for Change Reconciliation

The Change Impact Engine should follow canonical relationships.

Example:

```text
Implementation Change
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

This provides much stronger semantic context than filesystem proximity.

The reconciliation engine should therefore operate against semantic project relationships rather than merely searching nearby Markdown files.

---

# 25. Implications for Move/Rename

When an artifact moves:

```text
OLD REPRESENTATION

docs/Architecture/ADR-0017.md

             |
             v

NEW REPRESENTATION

docs/Architecture/Decisions/ADR-0017.md
```

the canonical identity remains:

```text
ADR-0017
```

and semantic relationships remain intact.

ProjectConcord should update derived Markdown representations as necessary.

This behavior directly depends on keeping identity separate from representation.

---

# 26. Implications for Future Web Application

This architecture becomes even more important when ProjectConcord evolves beyond local files.

Today:

```text
Canonical EDF Artifact
       |
       v
Local Markdown File
```

Future:

```text
Canonical Engineering Entity
       |
       +-- Markdown Repository Representation
       |
       +-- Desktop Representation
       |
       +-- Web Representation
       |
       +-- API Representation
       |
       +-- AI Representation
```

The architecture should not assume the local filesystem is the only meaningful representation forever.

---

# 27. Questions Cursor Must Resolve

Cursor should inspect CRA and EDF and determine:

1. Which applicable CRA principles are already formally accepted?
2. How does CRA currently define canonical identity?
3. How does CRA define relationship definitions?
4. How does CRA define relationship instances?
5. How does CRA address cardinality?
6. How does CRA address directionality?
7. How does CRA address contextual relationships?
8. How does CRA distinguish representation from identity?
9. How does CRA address location changes?
10. What neutral-reference mechanism currently exists or is planned?
11. Which of these concepts should EDF explicitly adopt?
12. Which engineering relationship semantics belong specifically to EDF?
13. Which ProjectConcord requirements currently exceed what CRA specifies?
14. Which ProjectConcord requirements currently exceed what EDF specifies?
15. Are additional CRA architectural records required?
16. Are additional EDF specifications or ADRs required?
17. Should ProjectConcord initially implement these concepts locally?
18. At what point, if any, should CKES become a dependency?

Do not guess where the authoritative documents can answer these questions.

---

# 28. Architectural Gap Handling

If ProjectConcord requires behavior that CRA has not yet defined, Cursor should report:

```text
CRA GAP
```

If ProjectConcord requires engineering semantics EDF has not defined, report:

```text
EDF GAP
```

If ProjectConcord merely needs an implementation choice within established CRA/EDF rules, treat that as:

```text
PROJECTCONCORD ARCHITECTURAL DECISION
```

This distinction is important.

ProjectConcord should not accidentally solve a CRA-level problem locally.

Likewise, CRA should not become cluttered with ProjectConcord-specific UI or engineering workflow decisions.

---

# 29. Initial Dependency Principle

The initial ProjectConcord implementation should follow:

> **Architectural alignment without unnecessary runtime coupling.**

That means ProjectConcord should respect applicable CRA principles from the beginning.

However, do not require a CRA/CKES runtime service merely to open and manage an EDF repository unless architecture analysis demonstrates that such a dependency is justified.

The initial desktop application should remain practical and incrementally implementable.

---

# 30. Required Integration With Existing Relationship Specification

Cursor must review this document together with the previously supplied ProjectConcord specification concerning:

- canonical artifact relationships;
- artifact registry;
- relationship index;
- identity resolver;
- link resolver;
- referential integrity;
- move/rename;
- external file changes;
- AI integration.

That specification should be updated or interpreted so that its relationship architecture is explicitly grounded in CRA where appropriate.

If the earlier specification accidentally assigns a CRA responsibility directly to ProjectConcord, correct that boundary.

Do not duplicate equivalent architectural definitions across documents unnecessarily.

---

# 31. Required Cursor Actions

Cursor should process this document in **Plan mode**.

## Step 1 — Locate This Root-Level Handover

Find this Markdown document in the **ProjectConcord repository root**.

Its location is temporary.

## Step 2 — Review Authoritative Architecture

Review:

- current ProjectConcord documentation;
- current EDF;
- applicable CRA architecture;
- existing ProjectConcord relationship specification.

Do not rely solely on this handover.

## Step 3 — Determine EDF Classification

Determine what type of EDF architectural artifact this material represents.

## Step 4 — Move and Rename

Move this document to its proper EDF-prescribed permanent location.

Give it:

- an EDF-compliant filename;
- an EDF-compliant document title;
- required EDF metadata/header.

The supplied filename and title are provisional.

## Step 5 — Establish Document Relationships

Create/propose appropriate references to:

- relevant ProjectConcord architecture;
- relationship/referential-integrity specification;
- applicable CRA records;
- applicable EDF records.

Use EDF-prescribed mechanisms.

## Step 6 — Identify Formal Decisions

Determine whether any statements in this handover require:

- ProjectConcord ADRs;
- EDF architectural changes;
- CRA architectural changes.

Do not bury important decisions inside an informal handover document.

## Step 7 — Correct Responsibility Boundaries

Ensure the resulting architecture clearly distinguishes:

```text
CRA
Foundational canonical architecture

EDF
Engineering-domain semantics and governance

ProjectConcord
Operational application and implementation
```

## Step 8 — Update Planning

Incorporate this architectural boundary into the ProjectConcord implementation plan.

## Step 9 — Report Gaps

Clearly report:

- CRA gaps;
- EDF gaps;
- ProjectConcord-specific decisions.

## Step 10 — Stop for Review

Do not proceed directly into major implementation.

Return the resulting architecture/Plan changes for architectural review.

---

# 32. Final Architectural Principle

ProjectConcord should not invent its own isolated theory of canonical identity and relationships.

The intended architecture is:

```text
                   CRA
        Canonical Representation
             Architecture
                   |
                   v
                  EDF
        Engineering Documentation
              Framework
                   |
                   v
            ProjectConcord
      Engineering Project Management
              Environment
```

CRA provides foundational canonical concepts.

EDF defines how engineering projects and engineering artifacts use those concepts.

ProjectConcord makes them operational.

This separation should allow ProjectConcord to provide powerful structured authoring, relationship management, referential integrity, AI assistance, implementation reconciliation, and future team collaboration without creating a competing canonical architecture.