[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › PCON-0000

# PCON-0000: EDF Project Management System — Architectural Vision and Bootstrap Handover

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural Discovery Record |
| **Normative** | No |
| **Status** | Draft — bootstrap handover |
| **Record ID** | PCON-0000 |
| **Date** | 2026-09-15 |
| **Owner** | ProjectConcord |
| **Authoritative** | No — consolidated vision and planning context; normative specs and ADRs follow separately |

**Project Type:** EDF-Based Engineering / Software Project Management System  
**Initial Deployment:** Single-user cross-platform desktop application  
**Future Deployment:** Potential multi-user web application  
**Recommended Technology:** C# / .NET / Avalonia  
**Primary Governing Framework:** Engineering Documentation Framework (EDF)

---

# 1. Purpose

This document provides the consolidated architectural vision and project-bootstrap requirements for a new software system built around the **Engineering Documentation Framework (EDF)**.

The system is provisionally referred to as:

> **EDF Project Management System**

The permanent product name may be selected later.

The first implementation should be a single-user desktop application using C#/.NET and Avalonia.

However, the architecture should regard the desktop application as the **first client and deployment model of a broader EDF Project Management System**, rather than making desktop-specific behavior fundamental to the domain.

The system may eventually be implemented as a multi-user web application supporting engineering and software-development teams.

This document is intended specifically as a **handover to Cursor AI in Plan mode**.

Cursor must inspect the current EDF specification and the target repository before proposing implementation.

This document is architectural source material. Its initial placement in the repository root is temporary.

Cursor must determine its appropriate EDF-compliant permanent classification, location, filename, and/or decomposition.

Do not assume the repository root is its final location.

---

# 2. Fundamental Product Vision

EDF currently defines how engineering project documentation is organized, named, governed, validated, and maintained.

The EDF Project Management System should make that framework:

- visible;
- navigable;
- measurable;
- editable;
- actionable;
- machine-interpretable;
- AI-assistable; and
- operationally useful throughout the project lifecycle.

The fundamental concept is:

> **EDF Project = Project Repository + EDF Interpretation**

For the desktop implementation, the project repository will normally be identified by a local filesystem root.

The application should discover the EDF structure, interpret the project, validate it, build a semantic project model, and present the current engineering state.

The system should not merely **view EDF**.

It should help the user **operate EDF**.

---

# 3. Core Architectural Principle — EDF Remains Canonical

The application MUST NOT replace EDF as the canonical source of engineering project knowledge.

EDF-prescribed repository artifacts remain canonical.

The application provides:

- interpretation;
- navigation;
- visualization;
- validation;
- structured authoring;
- lifecycle management;
- relationship management;
- project-status analysis;
- optional work management;
- Git/change analysis;
- documentation reconciliation; and
- optional AI assistance.

A repository should remain understandable and usable without this application.

The application MUST NOT require a proprietary database to understand the canonical engineering state of an EDF project.

Derived indexes, caches, UI state, search indexes, and other non-canonical data may use local storage.

---

# 4. Recommended Technology

The recommended initial technology stack is:

- C#
- modern .NET
- Avalonia UI

Primary eventual desktop targets:

- Windows
- macOS
- Linux

C#/.NET is preferred because the application is expected to become a substantial engineering system rather than a short-lived prototype.

It also provides a strong future path toward ASP.NET Core or other .NET server technologies if a web implementation is developed later.

The core EDF functionality MUST NOT depend upon Avalonia.

---

# 5. System Rather Than Desktop-Only Architecture

The system should conceptually be:

```text
                  EDF Project Management System

                              |
             +----------------+----------------+
             |                                 |
             v                                 v
      Desktop Client                     Future Web Client
         Avalonia                             Browser
             |                                 |
             +----------------+----------------+
                              |
                              v
                    Application Services
                              |
           +------------------+------------------+
           |                  |                  |
           v                  v                  v
       EDF Engine       Authoring Engine   Work Management
           |                  |                  |
           +------------------+------------------+
                              |
                              v
                       Project Domain
                              |
                              v
                     Repository Services
```

Initially these components may execute within one desktop process.

Their architectural boundaries should nevertheless remain clear.

---

# 6. Major Architectural Subsystems

The initial architecture should investigate separation into capabilities such as:

```text
EDF Project Management System

    Project Domain

    EDF Engine

    Canonical Authoring Service

    EDF Validation Engine

    Relationship / Project Graph

    Repository Services

    Git Services

    Change Impact Engine

    Documentation Reconciliation

    Agile / Work Management

    AI Semantic Services

    Application Services

    Avalonia Desktop Client
```

The exact assemblies/projects should be determined by Cursor after repository and EDF analysis.

A possible conceptual .NET structure is:

```text
src/
    Edf.Domain/
    Edf.Engine/
    Edf.Documents/
    Edf.Validation/
    Edf.Authoring/
    Edf.Relationships/
    Edf.Repository/
    Edf.Git/
    Edf.ChangeAnalysis/
    Edf.ProjectManagement/
    Edf.AI/
    Edf.Application/
    Edf.Desktop/

tests/
    ...
```

This is illustrative, not prescriptive.

Avoid unnecessary project fragmentation if simpler boundaries are sufficient.

---

# 7. EDF Engine

A reusable **EDF Engine** should provide deterministic interpretation of EDF projects.

Potential responsibilities include:

- repository discovery;
- EDF version detection;
- project-profile detection;
- artifact discovery;
- artifact classification;
- metadata parsing;
- naming validation;
- location validation;
- relationship discovery;
- reference resolution;
- milestone discovery;
- gate discovery;
- AWI discovery;
- architectural-decision discovery;
- specification discovery;
- validation-evidence discovery;
- acceptance-record discovery;
- status computation;
- conformance checking;
- broken-reference detection;
- filesystem-change detection.

Clients should preferably interact with semantic concepts rather than filesystem assumptions.

For example:

```text
GetActiveMilestone()
GetOpenGates()
GetOutstandingAWIs()
GetRelatedSpecifications(...)
GetValidationStatus(...)
GetProjectStatus(...)
```

rather than independently searching directories.

---

# 8. EDF Must Become Sufficiently Machine-Interpretable

Development of this application will likely expose portions of EDF that humans and AI can interpret but deterministic software cannot.

This should be treated as valuable feedback into EDF.

Examples may include ambiguity surrounding:

- artifact types;
- lifecycle states;
- required metadata;
- allowed transitions;
- relationships;
- naming;
- locations;
- project profiles;
- gates;
- milestones;
- acceptance;
- validation requirements;
- supersession;
- deletion;
- canonical identity.

The application MUST NOT silently invent EDF policy.

If deterministic interpretation requires information EDF does not formally provide, that gap should be documented and proposed back to EDF.

The application may eventually become an important **EDF reference implementation**.

However:

> The implementation implements EDF. It must not silently redefine EDF.

---

# 9. EDF Versions and Project Profiles

The application MUST NOT hard-code one repository layout as though that layout were EDF itself.

Conceptually:

```text
EDF Specification
        |
        +-- Version
        |
        +-- Project Profile
        |      |
        |      +-- Software Engineering
        |      +-- Electronics R&D
        |      +-- Other Profiles
        |
        +-- Artifact Definitions
        +-- Structure Rules
        +-- Naming Rules
        +-- Lifecycle Rules
        +-- Relationship Rules
        +-- Validation Rules
```

Project opening should conceptually perform:

```text
Select Project
      |
      v
Discover EDF Configuration
      |
      v
Determine Version/Profile
      |
      v
Load Applicable Rules
      |
      v
Scan Repository
      |
      v
Build Semantic Project Model
      |
      v
Resolve Relationships
      |
      v
Validate
      |
      v
Compute Project State
      |
      v
Present Project
```

---

# 10. Semantic Project Model

The application should construct a semantic representation of the project.

Potential concepts include:

- Project
- Project Profile
- Artifact
- Milestone
- Gate
- Architectural Decision
- Specification
- Requirement
- Development Plan
- AWI / Watch Item
- Validation Requirement
- Validation Evidence
- Acceptance Record
- Report
- Repository Revision
- Task
- Sprint
- Relationship
- Project Event

Not every concept must be implemented immediately.

The model must reflect actual EDF semantics rather than assumptions made solely for UI convenience.

---

# 11. EDF Project Graph

The application should eventually construct a semantic **EDF Project Graph**.

For example:

```text
Milestone M10
    |
    +-- governed by ------> ADR
    |
    +-- implements -------> Specification
    |
    +-- blocked by -------> Gate
    |
    +-- tracks -----------> AWI
    |
    +-- produces ---------> Validation Evidence
    |
    +-- closes with ------> Acceptance Record
```

Selecting an entity should expose its engineering context regardless of where its source documents physically reside.

This graph should be derived from canonical EDF information wherever possible.

---

# 12. Relationship to CRA and CKES

The architecture should remain compatible with principles being developed through the **Canonical Representation Architecture (CRA)** and **Canonical Knowledge Engineering System (CKES)**.

However, CRA or CKES should not become mandatory dependencies for the initial implementation unless analysis demonstrates a compelling reason.

EDF repository artifacts remain canonical.

The application's semantic project graph is initially a derived representation.

Future CRA/CKES integration may provide stronger:

- semantic identity;
- canonical relationships;
- cross-project knowledge;
- semantic discovery;
- shared engineering knowledge.

Avoid architecture that unnecessarily prevents such integration.

---

# 13. Canonical EDF Artifact Management

Canonical EDF document management is a **first-class requirement**, not merely a future convenience.

The system must support lifecycle management of EDF artifacts.

At minimum, architecture should support concepts equivalent to:

- Create
- Read
- Update
- Delete where EDF permits
- Move
- Rename
- Supersede
- Change Status
- Add/Remove Relationships

These operations must respect EDF rules.

The application should not simply expose arbitrary filesystem CRUD.

---

# 14. Structured EDF Authoring

Users should not need to understand every Markdown convention, filename rule, directory rule, required section, identifier format, or relationship syntax in order to create valid EDF artifacts.

The system should support **structured EDF authoring**.

For example:

```text
New
 |
 +-- Architectural Decision
 +-- Specification
 +-- AWI
 +-- Validation Record
 +-- Acceptance Record
 +-- Other EDF-defined artifact
```

A structured ADR editor might expose:

```text
ID
Title
Status

Context
Decision
Rationale
Consequences

Related Milestone
Related Gate
Related Specifications
Related AWIs
```

The application then produces the EDF-compliant canonical Markdown representation.

---

# 15. Raw Markdown Editing

Advanced users should still be able to inspect and, where appropriate, directly edit canonical Markdown.

The application should support both:

```text
Structured EDF View
```

and:

```text
Canonical Markdown View
```

Changes made through either representation must be validated.

The structured representation and Markdown representation must not become competing sources of truth.

The Markdown artifact remains canonical unless EDF later prescribes another canonical representation.

---

# 16. Canonical Authoring Service

Introduce the architectural concept of an **EDF Canonical Authoring Service**.

Conceptually:

```text
                 User / AI
                     |
                     v
          Canonical Authoring API
                     |
        +------------+------------+
        |            |            |
        v            v            v
     Schema       EDF Rules   Relationships
        |            |            |
        +------------+------------+
                     |
                     v
                 Validation
                     |
                     v
             Canonical Artifact
                     |
                     v
                  Markdown
```

UI components should request semantic operations such as:

```text
CreateArtifact(...)
UpdateArtifact(...)
MoveArtifact(...)
RenameArtifact(...)
SupersedeArtifact(...)
DeleteArtifact(...)
ChangeStatus(...)
AddRelationship(...)
```

rather than directly manipulating arbitrary files.

---

# 17. Artifact Lifecycle Safety

Not every EDF artifact should necessarily be destructively deletable.

For example, an accepted architectural decision may have historical and governance significance.

Before destructive operations, the application should evaluate:

- incoming references;
- outgoing references;
- lifecycle state;
- EDF deletion rules;
- historical significance;
- supersession requirements.

Example:

```text
ADR-0017 is referenced by:

3 Specifications
1 Milestone
2 Acceptance Records

Destructive deletion may not be permitted.

Recommended action:
Supersede ADR-0017.
```

Actual behavior must come from EDF rules.

---

# 18. AI-Assisted EDF Authoring

AI should substantially reduce the burden of writing EDF documentation.

The user may provide an engineering intent such as:

```text
Create an ADR for using OCCT as the initial
geometry kernel while preserving provider neutrality.
```

The workflow should conceptually be:

```text
User Intent
     |
     v
Determine EDF Artifact Type/Schema
     |
     v
Select Relevant Project Context
     |
     v
AI Draft
     |
     v
Structured EDF Proposal
     |
     v
EDF Validation
     |
     v
User Review
     |
     v
Canonical Authoring Service
     |
     v
Canonical Markdown
```

AI should perform semantic writing and reasoning.

The deterministic EDF system should handle:

- naming;
- location;
- IDs;
- required metadata;
- required sections;
- allowed states;
- relationships;
- validation.

Do not waste AI tokens solving deterministic EDF mechanics.

---

# 19. AI-Assisted Editing

AI should preferably propose semantic modifications rather than blindly rewrite complete files.

Example:

```text
Revise the Rationale section of ADR-0017 to account
for the newly accepted provider-interface requirement.
```

Workflow:

```text
AI Proposal
     |
     v
Canonical Authoring Service
     |
     v
EDF Validation
     |
     v
Relationship Validation
     |
     v
Diff
     |
     v
User Approval
     |
     v
Canonical Update
```

Users should be able to:

- accept;
- reject;
- edit;
- ask AI to revise;
- request explanation.

---

# 20. Economical AI Principle

AI MUST NOT be used for operations deterministic software can reliably perform.

Normally no AI should be required for:

- filesystem discovery;
- naming validation;
- location validation;
- metadata parsing;
- explicit relationship resolution;
- Git status;
- lifecycle-state reading;
- structured CRUD;
- deterministic conformance checking;
- task/sprint management.

AI should be used where semantic reasoning adds value.

Relevant context should be selected before invoking AI.

Preferred model:

```text
Repository
    |
    v
Deterministic Analysis
    |
    v
Relevant Context Selection
    |
    v
AI
```

Avoid repeatedly sending entire repositories to AI.

---

# 21. Git Integration

Git should be a first-class repository service.

However:

> Git state and EDF state are different concepts.

For example:

```text
Git: Clean
EDF: Non-Conformant
```

is valid.

Potential Git capabilities include:

- branch;
- commit;
- dirty/clean state;
- changed EDF artifacts;
- changed source files;
- commit history;
- diff generation;
- task baseline commits;
- reconciliation baselines.

Git must not redefine EDF project state.

---

# 22. Implementation Change Detection

A major capability should be understanding changes to software implementation.

The system should be capable of examining changes relative to a baseline such as:

- previous commit;
- selected commit;
- beginning of task;
- last reconciliation;
- branch divergence;
- uncommitted working tree.

Example:

```text
Baseline: abc123
Current:  def456

Changed:
    SketchPlacementFrame.cs
    SketchController.cs
    SnapEngine.cs
    SketchPlacementTests.cs
```

Raw line diffs alone are insufficient.

The goal is semantic change analysis.

---

# 23. Deterministic Code Analysis

For C# projects, investigate use of **Roslyn** or equivalent compiler services to derive structural implementation changes.

Examples:

```text
Added:
    SketchPlacementFrame.TransformToWorld()

Modified:
    SketchController.BeginPlacement()

Removed:
    LegacyXYProjection()

Interface changed:
    ISketchPlacementProvider

Tests added:
    NonXYPlanePlacementTests
```

This deterministic analysis should reduce the amount of source code requiring AI interpretation.

The architecture should permit additional language analyzers later.

Do not make the entire system permanently dependent on C# source code simply because the first implementation may primarily manage C# projects.

---

# 24. Change Impact Engine

Introduce a dedicated **Change Impact Engine**.

Conceptually:

```text
                   EDF Project Management System
                               |
              +----------------+----------------+
              |                |                |
              v                v                v
          EDF Engine     Authoring Engine    Git / Code
              |                |                |
              +-----------+    |    +-----------+
                          v    v    v
                      CHANGE IMPACT
                          ENGINE
                             |
                 +-----------+-----------+
                 |                       |
                 v                       v
        Deterministic Analysis      AI Analysis
                 |                       |
                 +-----------+-----------+
                             |
                             v
                    Reconciliation Plan
```

The Change Impact Engine should evaluate three major domains:

```text
Engineering Intent
       EDF

Implementation
       Code

Validation
       Tests / Evidence
```

---

# 25. Engineering Reconciliation Model

The system should help determine whether:

> **documented engineering intent, implementation, and validation evidence agree.**

Conceptually:

```text
               ENGINEERING INTENT
                      EDF
                       |
             +---------+---------+
             |                   |
             v                   v
       IMPLEMENTATION        VALIDATION
            Code              Evidence
             |                   |
             +---------+---------+
                       |
                       v
                 RECONCILIATION
```

This is more important than merely "updating documentation after coding."

---

# 26. Documentation Reconciliation Workflow

When implementation changes, the system should determine likely EDF consequences.

Example:

```text
Code Changes
     |
     v
Deterministic Change Analysis
     |
     v
EDF Relationship / Context Lookup
     |
     v
AI Change-Impact Analysis
     |
     v
Proposed EDF Consequences
     |
     v
USER REVIEW
     |
     v
Approved Canonical Changes
     |
     v
EDF Validation
     |
     v
Git
```

Canonical documents should NOT be silently rewritten merely because code changed.

---

# 27. Relationship-Guided Reconciliation

Existing project relationships should be used to constrain analysis.

Example:

```text
Task T-184
   |
   +-- implements ---> SPEC-0032
   +-- governed by --> ADR-0017
   +-- advances -----> M10
   +-- resolves -----> AWI-0043
```

Implementation changes associated with T-184 should cause the system to examine these artifacts first.

This avoids sending irrelevant project information to AI.

---

# 28. Reconciliation Results

A reconciliation may produce classifications such as:

```text
REQUIRED UPDATE

M10
Implementation status should be updated.

AWI-0043
Implementation appears to resolve this item.

Validation Record
New tests provide relevant evidence.


REVIEW RECOMMENDED

SPEC-0032
Implementation introduces behavior not explicitly
covered by the current specification.


NO CHANGE REQUIRED

ADR-0017
Implementation remains consistent with the
accepted architecture.
```

The user must be able to review proposed consequences.

---

# 29. Code Does Not Automatically Override Architecture

This is a critical governance rule.

The system MUST NOT assume:

> implementation changed, therefore documentation should be modified to match.

Sometimes the implementation is incorrect.

Example:

```text
ARCHITECTURAL CONFLICT

Implementation appears inconsistent with ADR-0008.

ADR-0008 requires provider-neutral geometry access.

Detected:
Application code directly references provider-specific
geometry implementation.

Possible resolutions:

1. Correct the implementation.

2. Intentionally change the architecture and formally
   supersede/revise the applicable architectural decision.
```

The application should surface the conflict rather than automatically changing the architecture record.

---

# 30. Reconciliation Review UI

A future UI might provide:

```text
Implementation Reconciliation — T-184

4 EDF artifacts evaluated

[x] Update M10 implementation status
[x] Close AWI-0043
[x] Add validation evidence
[ ] Amend SPEC-0032

ADR-0017
No change recommended.

        [Review Proposed Changes]
```

Individual changes should expose diffs.

Users should be able to:

- accept;
- reject;
- edit;
- defer;
- ask AI to revise;
- ask why a change was proposed.

Only approved changes should reach canonical authoring.

---

# 31. Reconciliation Without Agile

Change reconciliation MUST NOT require Agile.

The application should support a command conceptually similar to:

```text
Project > Reconcile Changes
```

with possible baselines:

```text
Since:
    Last Commit
    Selected Commit
    Last Reconciliation
    Branch Divergence
    Current Uncommitted Changes
```

This allows any EDF project to benefit.

---

# 32. Optional Agile / Work Management

Agile project-management support should be optional.

EDF defines engineering documentation and governance.

Agile describes planning and execution of work.

Conceptually:

```text
EDF
 |
 +-- Engineering Intent
 +-- Architecture
 +-- Specifications
 +-- Decisions
 +-- Gates
 +-- Validation
 +-- Acceptance

Agile
 |
 +-- Backlog
 +-- Tasks
 +-- Sprints
 +-- Assignments
 +-- Estimates
 +-- Priorities
 +-- Work Status
```

Agile must not become mandatory merely because the application supports it.

---

# 33. Agile Project Model

Potential concepts include:

- Backlog
- Sprint
- Task
- Story
- Epic
- Assignee
- Priority
- Estimate
- Status
- Dependency
- Acceptance Criteria

A lightweight initial model may be sufficient:

```text
Project
   |
   +-- Backlog
   |
   +-- Sprint
         |
         +-- Task
         +-- Task
         +-- Task
```

Tasks should reference EDF entities where relevant.

---

# 34. EDF and Agile Relationship

A key principle is:

> Agile artifacts describe work to be performed. EDF artifacts describe engineering intent, governance, evidence, and accepted project state.

A task may:

- implement a specification;
- resolve an AWI;
- advance a milestone;
- satisfy part of a gate;
- produce validation evidence.

The task must not silently replace those EDF records.

---

# 35. Task-Based Reconciliation

Tasks provide useful boundaries for implementation analysis.

When a task begins:

```text
Task T-184

Status:
In Progress

Baseline:
abc123

Related:
SPEC-0032
ADR-0017
M10
AWI-0043
```

When the developer chooses:

```text
Complete Task
```

the application may initiate:

```text
Complete Task
     |
     v
Analyze Changes Since Baseline
     |
     v
Collect Validation Results
     |
     v
Analyze EDF Impact
     |
     v
Documentation Reconciliation
     |
     v
User Review
     |
     v
Update Approved EDF Artifacts
     |
     v
Complete Task
```

This workflow should be investigated carefully because it could become a major product capability.

---

# 36. Agile Persistence

Meaningful project-management state should preferably remain repository-portable.

Potential canonical representations may include:

- Markdown;
- YAML;
- JSON;
- other EDF-prescribed formats.

A proprietary local database should not become the sole copy of important project state.

Local storage may still be useful for:

- indexes;
- caches;
- search;
- UI preferences;
- derived relationships;
- temporary analysis.

Cursor should propose an approach consistent with EDF principles.

---

# 37. Project Dashboard

The desktop application should provide an operational project dashboard.

Conceptually:

```text
+---------------------------------------------------------------+
| Project Name                                  EDF: Conformant |
+--------------------+------------------------------------------+
| PROJECT            | PROJECT STATUS                           |
|                    |                                          |
| Overview           | Overall Status      Healthy              |
| Architecture       | EDF Conformance     Pass                 |
| Specifications     | Active Milestone    M12                  |
| Development        | Open Gates          2                    |
| Decisions          | Open AWIs           4                    |
| Gates              | Current Sprint      Sprint 14            |
| AWIs               |                                          |
| Validation         |------------------------------------------|
| Reports            | ATTENTION                                |
| Agile              | 2 documents need reconciliation          |
| Repository         | 1 architectural conflict                 |
|                    | 3 validation items pending               |
+--------------------+------------------------------------------+
```

The dashboard should expose engineering state, not merely file counts.

---

# 38. Semantic Document Navigation

Users should navigate the project semantically.

Selecting:

```text
M10
```

should show the milestone and associated:

- specifications;
- decisions;
- gates;
- AWIs;
- implementation activity;
- validation;
- acceptance evidence;
- tasks.

Selecting an artifact should permit opening its canonical source.

The UI should clearly distinguish:

- canonical information;
- derived information;
- computed status;
- cached information;
- AI-generated proposals.

---

# 39. File Monitoring

The desktop application should monitor repository changes.

External tools such as:

- Cursor;
- IDEs;
- Git;
- text editors;
- build systems;
- scripts

may modify the project.

Conceptually:

```text
Repository Change
      |
      v
Identify Affected Artifacts
      |
      v
Incremental Reparse
      |
      v
Update Relationships
      |
      v
Update Validation
      |
      v
Potential Reconciliation Flag
      |
      v
Refresh UI
```

Avoid unnecessary full rescans.

---

# 40. Validation

Validation should be a major system capability.

Potential problems include:

- missing required artifacts;
- incorrect locations;
- incorrect naming;
- invalid metadata;
- broken references;
- unresolved relationships;
- inconsistent lifecycle states;
- missing acceptance evidence;
- missing validation evidence;
- profile violations;
- EDF-version incompatibility.

Validation should ideally report:

1. what is wrong;
2. where;
3. applicable EDF rule;
4. why it matters;
5. possible correction.

---

# 41. Future Multi-User Web Application

Although the first implementation is desktop-based, the architecture MUST anticipate a possible multi-user web system.

Potential team roles include:

- Architect
- Developer
- QA / Test Engineer
- Documentation
- Project Manager
- Product Owner
- Stakeholder
- Reviewer
- Administrator
- Read-Only Observer

These roles are illustrative.

The eventual permission model should be configurable.

---

# 42. Role-Based Views

Different users may need different views of the same canonical project.

Architects may focus on:

- ADRs;
- specifications;
- architectural gates;
- design conflicts;
- milestone dependencies.

Developers may focus on:

- assigned tasks;
- current sprint;
- implementation requirements;
- governing specifications;
- related ADRs.

QA may focus on:

- validation requirements;
- acceptance criteria;
- builds awaiting validation;
- failed tests;
- acceptance evidence.

Documentation personnel may focus on:

- documentation impact;
- changed specifications;
- unresolved documentation items;
- release documentation.

Stakeholders may focus on:

- milestones;
- project health;
- risks;
- release targets;
- major decisions.

These remain representations of one canonical project.

---

# 43. Authentication and Authorization

Authentication is NOT required for the initial single-user desktop application.

The architecture should nevertheless recognize:

```text
Identity
   |
   v
Role
   |
   v
Permission
   |
   v
Operation
```

The EDF Engine itself should preferably remain independent of authentication.

Authorization should normally occur at the application/service boundary.

---

# 44. Repository Abstraction

The desktop implementation initially assumes:

```text
Project
   =
Local Repository Root
```

A future server may use:

```text
Project
   =
Server-Managed Repository
```

or:

```text
Project
   =
Connected Git Repository
```

Avoid embedding local-path assumptions throughout the domain.

Investigate a repository abstraction conceptually similar to:

```text
IProjectRepository
       |
       +-- LocalFileSystemRepository
       +-- FutureServerRepository
       +-- FutureGitRepositoryProvider
```

Do not over-engineer the abstraction prematurely.

---

# 45. Concurrency and Auditability

A future team system may have many users working against the same project.

Future concerns include:

- optimistic concurrency;
- conflicting edits;
- revision management;
- approvals;
- task ownership;
- notifications;
- audit history.

Important engineering actions may eventually need to answer:

```text
Who proposed this?

Who approved it?

Who implemented it?

Who validated it?

Who changed the specification?

When?

Against which repository revision?
```

Git should provide history where appropriate.

Do not duplicate Git history unnecessarily.

---

# 46. Project Events

Investigate whether a UI-independent project-event abstraction is useful.

Examples:

```text
MilestoneOpened
MilestoneClosed
GateOpened
GateSatisfied
SpecificationChanged
DecisionProposed
DecisionAccepted
TaskAssigned
TaskCompleted
ValidationSubmitted
ValidationFailed
ValidationAccepted
AWIOpened
AWIClosed
ReconciliationRequired
ReconciliationCompleted
```

Future uses may include:

- notifications;
- audit history;
- integrations;
- automation;
- live web updates;
- reporting.

Cursor should determine whether this abstraction belongs in the initial architecture or should be deferred.

---

# 47. AI Provider Boundary

AI should remain optional to core EDF interpretation.

Conceptually:

```text
EDF / Application Services
          |
          +-- Deterministic Services
          |
          +-- Semantic AI Services
                    |
                    v
             AI Provider Interface
```

The system should avoid unnecessary dependence upon one AI vendor.

Initial integration may also leverage external AI development tools such as Cursor rather than reproducing full source-code understanding immediately.

---

# 48. Transitional Cursor-Assisted Reconciliation

The first implementation does not need to recreate Cursor's complete code-understanding capability.

An early workflow may generate a structured handover for Cursor such as:

```text
Analyze implementation changes between commits
abc123 and def456.

Associated EDF context:
    SPEC-0032
    ADR-0017
    M10
    AWI-0043

Determine:

1. implementation changes;
2. documentation impact;
3. architectural conflicts;
4. validation implications;
5. proposed EDF updates.

Do not modify canonical EDF artifacts without approval.
```

Later, integrated AI services may perform more of this analysis directly.

This provides an incremental path.

---

# 49. Possible Future CLI

Because core functionality should remain UI-independent, future CLI support should be possible.

Potential commands:

```text
edf status
edf validate
edf milestones
edf gates
edf awi
edf graph
edf reconcile
edf sprint
```

This could support:

- developers;
- CI/CD;
- Git hooks;
- AI agents;
- automation.

The CLI is not required for the initial desktop MVP.

---

# 50. Possible Future CI/CD

The reusable EDF Engine may eventually support automated validation:

```text
Git Push
   |
   v
CI Pipeline
   |
   v
EDF Validation
   |
   +-- PASS
   |
   +-- FAIL
```

The same validation logic should be reused rather than independently reimplemented.

---

# 51. Initial MVP Priorities

The initial implementation should remain incremental.

Recommended priority order:

```text
1. EDF Project Bootstrap
2. Reusable EDF Engine
3. Repository Discovery
4. EDF Version/Profile Interpretation
5. Semantic Project Model
6. EDF Validation
7. Canonical Artifact CRUD
8. Structured EDF Authoring
9. Basic Avalonia Project UI
10. Semantic Navigation / Relationships
11. Git Integration
12. Change Detection
13. Basic Change Impact / Reconciliation
14. AI-Assisted EDF Authoring
15. AI-Assisted Reconciliation
16. Optional Agile Layer
17. Advanced Team/Web Preparation
```

Cursor should revise this ordering if architectural dependencies indicate a better sequence.

---

# 52. MVP Canonical Authoring Requirement

Basic canonical EDF authoring should be considered part of the product's early core capability.

The application should not merely tell the user:

```text
SPEC-0032 is invalid.
```

and force them to leave the application and manually repair Markdown.

Where EDF rules are sufficiently deterministic, the system should help the user correct the problem safely.

---

# 53. Non-Goals for Initial Implementation

The first implementation should NOT attempt to become:

- Jira;
- GitHub;
- GitLab;
- a complete IDE;
- a complete Git client;
- a universal Markdown editor;
- a general-purpose project-management platform;
- a full AI coding agent;
- a CRA implementation;
- a CKES implementation;
- a cloud collaboration platform.

The immediate goal is:

> **Provide an operational environment for understanding, authoring, validating, maintaining, and reconciling an EDF-managed project.**

---

# 54. Architectural Boundaries to Preserve

## 54.1 EDF vs Application

EDF defines the engineering framework.

The application interprets and operates against it.

## 54.2 Canonical vs Derived

EDF-prescribed repository artifacts are canonical.

Indexes, caches, graphs, UI models, and computed status are derived unless EDF says otherwise.

## 54.3 Engineering Intent vs Implementation

Code does not automatically redefine accepted engineering intent.

Conflicts must be surfaced.

## 54.4 Engineering Intent vs Validation

Validation evidence demonstrates implementation behavior but does not independently redefine architecture or specifications.

## 54.5 EDF vs Agile

EDF describes engineering knowledge and governance.

Agile optionally manages work.

## 54.6 EDF vs Git

EDF state and Git state are related but distinct.

## 54.7 Deterministic vs AI

Use deterministic software wherever sufficient.

Use AI where semantic reasoning adds meaningful value.

## 54.8 Core vs UI

EDF and application-domain logic must remain independent from Avalonia.

## 54.9 Desktop vs System

The desktop application is the initial client, not the entire conceptual architecture.

## 54.10 Specification vs Implementation

Application behavior must not silently create new EDF policy.

---

# 55. Important Questions Cursor Must Investigate

Cursor should inspect the current EDF specification and determine:

1. How is an EDF project formally identified?
2. How is the EDF version represented?
3. How are project profiles represented?
4. Which artifact types are formally defined?
5. Which metadata is machine-readable?
6. Which artifact schemas are formally defined?
7. Which required sections are formally defined?
8. Which relationships are explicit?
9. Which relationships currently require inference?
10. How are milestones represented?
11. How are gates represented?
12. How are AWIs represented?
13. How are ADRs represented?
14. How are specifications represented?
15. How are validation records represented?
16. How are acceptance records represented?
17. What lifecycle states exist for each artifact?
18. Which state transitions are valid?
19. What are the rules for deletion?
20. What are the rules for supersession?
21. What are the rules for move/rename?
22. How should references survive move/rename?
23. Which existing EDF validation scripts can be reused?
24. Which EDF concepts remain ambiguous for deterministic interpretation?
25. Does EDF need machine-readable artifact schemas?
26. How should structured authoring map to canonical Markdown?
27. How should Agile state be represented?
28. How should Agile entities reference EDF entities?
29. What should be canonical versus cached?
30. How should implementation changes be associated with EDF artifacts?
31. How should reconciliation baselines be recorded?
32. How can Roslyn or other analyzers contribute deterministic code-change summaries?
33. How should validation evidence be associated with implementation changes?
34. How should architectural conflicts be represented?
35. Which components must remain reusable for a future web application?
36. What repository abstraction is appropriate without premature complexity?
37. How should future identity/roles fit without implementing authentication now?
38. What portions of current EDF must be improved to support this system correctly?

Do not silently solve EDF ambiguities through application heuristics.

Document them.

---

# 56. Required Cursor AI Planning Actions

Cursor should now work in **Plan mode**.

## Step 1 — Inspect Current EDF

Review the authoritative Engineering Documentation Framework.

Determine the proper bootstrap procedure and applicable project profile.

Do not rely solely on this handover.

## Step 2 — Inspect the New Repository

Determine what currently exists.

Preserve valid work.

## Step 3 — Classify the Project

Determine the appropriate EDF profile.

If EDF lacks necessary support, identify the deficiency.

## Step 4 — Classify This Handover

This file is intentionally supplied in the repository root.

The root is not presumed to be its permanent location.

Determine whether its contents belong in:

- Project Introduction;
- Architectural Vision;
- ADRs;
- Specifications;
- Development Plans;
- Requirements;
- other EDF-prescribed artifacts.

Move, rename, split, incorporate, or supersede it as required by EDF.

## Step 5 — Develop the Architecture Plan

The plan should address:

- solution/project structure;
- EDF Engine;
- semantic project model;
- EDF version/profile interpretation;
- validation;
- canonical authoring;
- artifact schemas;
- CRUD/lifecycle rules;
- relationship graph;
- Git integration;
- code-change analysis;
- Change Impact Engine;
- reconciliation workflow;
- AI boundary;
- Agile boundary;
- future web boundary;
- testing;
- milestones;
- gates.

## Step 6 — Identify EDF Gaps

Explicitly list anything required by this system that EDF does not currently specify sufficiently for deterministic implementation.

These may become EDF improvement proposals.

## Step 7 — Evaluate Desktop-to-Web Evolution

Evaluate this scenario:

> The single-user Avalonia application succeeds and must later become or coexist with a multi-user web application supporting architects, developers, QA personnel, documentation personnel, project managers, and stakeholders.

Identify:

- components reusable unchanged;
- components requiring adaptation;
- components requiring replacement.

If moving to web requires rewriting the EDF Engine, domain model, validation engine, canonical authoring rules, relationship model, Agile model, or core application logic, reconsider the proposed architecture.

## Step 8 — Evaluate Implementation Reconciliation

Evaluate this scenario:

> A developer implements a feature or completes a task. Source code and tests have changed. The system must determine what EDF documentation may require updates, identify architectural conflicts, propose documentation changes, and present those changes to the user for approval.

The architecture should clearly explain how:

```text
EDF Intent
Code Changes
Validation Evidence
```

are reconciled.

## Step 9 — Propose Initial Milestones

Create an incremental implementation roadmap.

Prioritize the deterministic EDF foundation before advanced AI.

Do not attempt to implement the complete vision at once.

## Step 10 — Stop for Architectural Review

Do NOT proceed directly into major implementation.

Produce the Plan-mode architecture/bootstrap plan.

Clearly identify:

- assumptions;
- open questions;
- EDF deficiencies;
- architectural alternatives;
- recommended decisions;
- proposed ADRs;
- proposed milestones;
- proposed gates.

Then stop for review.

---

# 57. Guiding Product Principle

The central principle of the project is:

> **EDF is the canonical engineering framework. The EDF Project Management System makes EDF visible, navigable, editable, measurable, actionable, reconcilable, and easier to use without replacing it.**

The application should reduce the burden of maintaining rigorous engineering documentation while increasing its reliability.

AI should reduce human authoring effort.

Deterministic software should enforce deterministic rules.

Humans should retain authority over consequential engineering changes.

---

# 58. Long-Term Vision

The long-term system may evolve toward:

```text
                  EDF Project Management System

                             |
       +---------------------+----------------------+
       |                     |                      |
       v                     v                      v
Engineering State      Work Management       Repository State
       |                     |                      |
       v                     v                      v
      EDF                  Agile                   Git
       |
       v
Semantic Project Graph
       |
       +---------------------+
       |                     |
       v                     v
Canonical Authoring    Change Reconciliation
       |                     |
       v                     v
Structured UI            Code + Tests
       |                     |
       +----------+----------+
                  |
                  v
           AI Assistance
```

Future integrations may include:

- CRA;
- CKES;
- GitHub;
- GitLab;
- IDEs;
- CI/CD;
- engineering tools;
- issue trackers;
- AI development environments;
- multi-user web collaboration.

These are extensibility directions rather than immediate requirements.

---

# 59. Initial Success Criterion

A major initial success should be demonstrable as follows.

A user points the desktop application at an existing EDF-managed repository.

Without manually configuring the project, the application can explain:

- what the project is;
- which EDF version/profile governs it;
- what major artifacts exist;
- what milestone is active;
- which gates remain open;
- which AWIs remain unresolved;
- whether the repository conforms to EDF;
- where important canonical artifacts reside;
- how those artifacts relate;
- what changed recently;
- whether documentation may be out of sync with implementation.

The user can then:

- create an EDF artifact;
- edit it through a structured interface;
- optionally edit canonical Markdown;
- use AI to draft or revise content;
- validate the artifact;
- save it to the correct canonical location;
- inspect relationships;
- analyze implementation changes;
- review proposed documentation consequences;
- approve or reject those changes.

At that point the system has moved beyond an EDF viewer.

It has become:

> **an operational engineering environment for EDF.**

---

# 60. Final Instruction to Cursor

Treat this document as architectural input for planning.

Do not blindly implement it.

Inspect EDF first.

Where this vision conflicts with current EDF requirements, identify the conflict.

Where EDF lacks sufficient machine-readable semantics, identify the gap.

Where a proposed capability requires an architectural decision, propose an ADR rather than embedding the decision silently in implementation.

Where the vision is too large for one milestone, decompose it.

Preserve the distinction between:

```text
Engineering Intent
Implementation
Validation
Work Management
Repository State
Derived Analysis
AI Proposals
```

The system should bring these together without collapsing them into one undifferentiated source of truth.

**Produce the EDF-compliant architectural/bootstrap Plan first, then stop for architectural review before major implementation.**