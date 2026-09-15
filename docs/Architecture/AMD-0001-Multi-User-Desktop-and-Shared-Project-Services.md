# ProjectConcord — Multi-User Desktop and Shared Project Services Architectural Amendment

**Status:** Architectural Amendment — integrated 2026-09-15 (normative summary: [System Architecture Overview](System_Architecture_Overview.md), [ADR-0009](ADRs/ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [ADR-0010](ADRs/ADR-0010-Single-User-Administrator-Default-Model.md))  
**Project:** ProjectConcord  
**Governing Framework:** Engineering Documentation Framework (EDF)  
**Related Architecture:** Canonical Representation Architecture (CRA)  
**Document ID:** AMD-0001  
**Permanent Placement:** `docs/Architecture/AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md`  
**Companion:** [AMD-0002 — Single-User Administrator Model](AMD-0002-Single-User-Administrator-Model.md)

---

# 1. Instructions to Cursor AI

This Markdown document is intentionally being supplied in the **ProjectConcord repository root** as an architectural amendment.

**The repository root is NOT its intended permanent location.**

The supplied filename and title are provisional.

Cursor AI must:

1. locate this document in the ProjectConcord repository root;
2. inspect the current ProjectConcord repository;
3. inspect all ProjectConcord architectural documents previously created from earlier handovers;
4. inspect the authoritative current Engineering Documentation Framework (EDF);
5. inspect applicable Canonical Representation Architecture (CRA) documentation where relevant;
6. determine which existing ProjectConcord documents are affected by this amendment;
7. amend those existing documents rather than creating contradictory parallel architecture;
8. determine whether this amendment itself should remain as a permanent EDF artifact or whether its content should be incorporated into existing architectural artifacts;
9. if retained, move it to the proper EDF-prescribed location;
10. if retained, give it an EDF-compliant filename and title;
11. add or correct any required EDF metadata/header;
12. update affected relationships, references, plans, ADRs, specifications, milestones, gates, or other EDF artifacts as appropriate;
13. identify any architectural decisions requiring formal ADR treatment;
14. identify any EDF or CRA gaps exposed by these requirements;
15. return the resulting plan and affected-document analysis for architectural review before major implementation.

Do NOT assume that this handover should simply be added as another permanent document.

Its primary purpose is to **amend the ProjectConcord architecture already documented**.

---

# 2. Purpose of This Amendment

Earlier ProjectConcord architectural discussions characterized the initial application approximately as:

> **Single-user desktop first, with multi-user web/team capabilities later.**

That characterization is no longer correct.

It must be amended.

The revised architectural principle is:

> **ProjectConcord is desktop-first, but multi-user from its architectural foundation.**

The initial ProjectConcord client may remain a native cross-platform desktop application using C#/.NET and Avalonia.

However, the system architecture must support multiple authenticated users working concurrently with shared ProjectConcord projects and shared project services.

A desktop application MUST NOT imply a single-user system.

---

# 3. Revised Product Model

The conceptual model should change from:

```text
Desktop Application
      |
      v
Local User
      |
      v
Local Project
```

to:

```text
               ProjectConcord Platform
                         |
          +--------------+--------------+
          |              |              |
          v              v              v
     Desktop        Desktop        Desktop
      Client         Client         Client
     Architect      Developer         QA
          |              |              |
          +--------------+--------------+
                         |
                         v
                 Shared Services
                         |
          +--------------+--------------+
          |              |              |
          v              v              v
       Project       Identity &       Shared
       Services      Authorization    Data
          |
          v
     EDF Repository
```

The desktop application is the first client of the ProjectConcord system.

It should not define the complete system boundary.

---

# 4. Desktop-First Does Not Mean Local-Only

ProjectConcord should preserve the benefits of a native desktop application:

- responsive UI;
- strong desktop integration;
- filesystem integration where appropriate;
- Git integration;
- local development tooling;
- local caching;
- potential offline capabilities;
- integration with IDEs and engineering tools.

At the same time, it should be capable of participating in a shared ProjectConcord environment.

Therefore:

```text
Desktop-first
    !=
Single-user

Desktop-first
    !=
Local-data-only

Desktop-first
    !=
No server architecture
```

---

# 5. Multi-User Requirement

Multiple ProjectConcord users should eventually be able to access the same engineering project concurrently.

Example:

```text
                 ProjectConcord Project
                         |
        +----------------+----------------+
        |                |                |
        v                v                v
    Architect         Developer       QA Tester
        |                |                |
        +----------------+----------------+
                         |
                         v
              Same Engineering Project
```

Each user may have different responsibilities and permissions.

They must nevertheless operate against the same authoritative project state.

---

# 6. Users, Personas, Roles, and Permissions

ProjectConcord must support user identities and role-based responsibilities.

Candidate roles include:

- Architect
- Developer
- Development Manager
- Project Manager
- Product Owner
- QA Tester
- Validation Engineer
- Documentation Technician
- Documentation Manager
- Reviewer
- Stakeholder
- Administrator
- Read-Only Observer

These names are provisional.

Cursor should determine the appropriate domain terminology.

---

# 7. Persona vs Role

The term **persona** may describe how ProjectConcord presents information to a user.

The term **role** may be more appropriate for authorization.

Cursor should explicitly evaluate whether ProjectConcord should distinguish:

```text
User
 |
 +-- Roles
 |     |
 |     +-- permissions
 |
 +-- Persona / Workspace
       |
       +-- presentation
       +-- dashboard
       +-- workflow emphasis
```

For example, a user may have:

```text
User: Alice

Roles:
    Architect
    Developer

Current Workspace/Persona:
    Architect
```

This could allow the same person to work from different perspectives without changing their underlying permissions.

Do not conflate UI presentation with security authorization without architectural consideration.

---

# 8. Multiple Roles Per User

ProjectConcord must not assume one user equals one role.

Small engineering teams frequently require one person to perform several responsibilities.

For example:

```text
User
 |
 +-- Architect
 +-- Developer
 +-- Project Manager
 +-- Documentation
```

The authorization architecture should therefore support many-to-many relationships between users and roles where appropriate.

---

# 9. Role-Based Views

Different users should be able to see ProjectConcord through views appropriate to their responsibilities.

## Architect

May emphasize:

- architectural decisions;
- specifications;
- architectural conflicts;
- design reviews;
- open architectural questions;
- gates;
- implementation deviations;
- proposed architectural changes.

## Developer

May emphasize:

- assigned work;
- relevant specifications;
- governing ADRs;
- implementation status;
- source changes;
- tests;
- documentation reconciliation;
- blockers.

## Development Manager

May emphasize:

- development milestones;
- assignments;
- dependencies;
- progress;
- blockers;
- gates;
- workload;
- implementation status.

## Project Manager

May emphasize:

- overall project status;
- milestones;
- backlog;
- schedule;
- assignments;
- dependencies;
- risks;
- approvals.

## QA / Validation

May emphasize:

- validation requirements;
- tests;
- failures;
- defects;
- validation evidence;
- acceptance criteria;
- builds awaiting validation.

## Documentation

May emphasize:

- documents requiring reconciliation;
- unresolved references;
- documentation completeness;
- proposed updates;
- formatting/conformance;
- relationship integrity.

These are views of the **same project**, not independent project-management systems.

---

# 10. Authorization

ProjectConcord should conceptually distinguish:

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

Example operations might include:

```text
ViewArtifact
EditArtifact
CreateArtifact
MoveArtifact
RenameArtifact
SupersedeArtifact
DeleteArtifact
ApproveDecision
AcceptValidation
ManageTasks
AssignUsers
ManageRoles
AdministerProject
```

The exact permission model should be designed rather than assumed.

The EDF Engine itself should remain as authorization-independent as practical.

Authorization should generally occur at appropriate application/service boundaries.

---

# 11. Shared Cloud-Based Data

ProjectConcord should support shared cloud-hosted project data.

However, the introduction of a database must NOT automatically change the canonical-source principles already established.

In particular:

> **A shared ProjectConcord database must not silently replace canonical EDF repository artifacts as the authoritative engineering record.**

The architecture must explicitly classify information according to ownership and canonical status.

---

# 12. Canonical vs Operational Data

A likely distinction is:

## Canonical Engineering Information

Examples:

- ADRs;
- specifications;
- milestones where EDF prescribes them;
- gates;
- AWIs;
- validation records;
- acceptance records;
- canonical engineering documentation;
- other EDF-prescribed artifacts.

These should continue to follow EDF canonical-storage rules.

## Collaborative / Operational Information

Potential examples:

- users;
- authentication identities;
- role assignments;
- permissions;
- current sessions;
- notifications;
- task assignments;
- presence;
- personal UI preferences;
- workspace/persona selections;
- transient edit state;
- synchronization state;
- cached indexes;
- collaboration metadata.

These may be appropriate for shared database storage.

Cursor must determine the appropriate boundary rather than assuming all ProjectConcord information belongs in either Markdown or a database.

---

# 13. Shared Database Architecture

The architecture should permit a deployment such as:

```text
Desktop Clients
      |
      v
ProjectConcord Application Services
      |
      +----------------------+
      |                      |
      v                      v
Shared Operational       Repository /
Database                 Git Services
      |                      |
      |                      v
      |                 EDF Artifacts
      |
      v
Users / Roles /
Assignments /
Collaboration /
Derived State
```

This is conceptual.

It does NOT prescribe a particular database technology or cloud provider.

Those should be selected through appropriate architectural decisions.

---

# 14. Do Not Couple Desktop UI Directly to Shared Database

Avalonia ViewModels and UI controls should not become the primary owners of database logic.

Avoid architecture resembling:

```text
Avalonia Control
      |
      v
SQL
```

Prefer:

```text
Avalonia Client
      |
      v
Application Services
      |
      v
Domain / Project Services
      |
      v
Persistence / Repository Services
```

This preserves the ability to introduce:

- different database technologies;
- remote services;
- a future web client;
- testing;
- offline behavior;
- alternative deployment models.

---

# 15. Local and Remote Deployment

ProjectConcord should not unnecessarily require cloud infrastructure for every use case.

The architecture should evaluate support for both:

```text
LOCAL / INDIVIDUAL

Desktop Client
      |
      v
Local Project Services
      |
      +--> Local Repository
      +--> Local Operational Store
```

and:

```text
SHARED / TEAM

Desktop Clients
      |
      v
Shared Project Services
      |
      +--> Shared Repository Services
      +--> Shared Operational Database
      +--> Identity / Authorization
```

The same core domain architecture should support both where practical.

Do not create two independent ProjectConcord products.

---

# 16. Service Boundary

The earlier architectural requirement that core ProjectConcord logic remain outside Avalonia becomes more important.

The architecture should conceptually support:

```text
             Client Layer
                  |
          Avalonia Desktop
                  |
                  v
         Application Services
                  |
      +-----------+-----------+
      |           |           |
      v           v           v
 EDF Services   Project   Collaboration
                Services     Services
      |           |           |
      +-----------+-----------+
                  |
                  v
             Data Access
```

Initially, some application services may execute in-process.

The architecture should permit appropriate services to move behind a network boundary later without rewriting the core domain.

---

# 17. Concurrent Access

ProjectConcord must assume that multiple users may access the same project simultaneously.

This affects:

- artifact editing;
- relationship changes;
- task assignments;
- approvals;
- validation;
- change sets;
- repository changes;
- project state;
- dashboards;
- notifications.

Concurrency must therefore be an architectural concern from the beginning.

---

# 18. Concurrent Artifact Editing

Example:

```text
Architect
    |
    +------> SPEC-0032 <------+
                             |
                         Developer
```

Both may open the same artifact.

ProjectConcord must prevent silent overwriting.

A likely strategy is optimistic concurrency based on revisions or repository state.

Example:

```text
Architect opens:
SPEC-0032 revision 17

Developer opens:
SPEC-0032 revision 17

Architect saves:
revision 18

Developer attempts save:
revision 17 -> conflict

ProjectConcord:
Document changed since it was opened.
```

The user should then be given an appropriate review/merge/reapply workflow.

The exact mechanism should be determined architecturally.

---

# 19. Avoid Crude Permanent Locking

ProjectConcord should not default to permanent exclusive file locking merely because concurrent editing exists.

Some operations may justify temporary leases or locks.

Others may work better through optimistic concurrency.

Cursor should evaluate:

- revision checks;
- optimistic concurrency;
- short-lived edit leases;
- semantic merges;
- Git merge capabilities;
- change-set conflict detection.

The solution should match the type of operation.

---

# 20. Change Sets Become More Important

The previously proposed ProjectConcord **Change Set** architecture becomes particularly valuable in a multi-user system.

Instead of immediately modifying several canonical files:

```text
User Action
    |
    v
Proposed Change Set
    |
    +-- Artifact modifications
    +-- Relationship changes
    +-- Link updates
    +-- Metadata updates
    |
    v
Validate
    |
    v
Check Concurrency
    |
    v
Review / Approval
    |
    v
Apply
```

A change set can be checked against the current project revision before application.

---

# 21. Change Set Concurrency

Consider:

```text
User A prepares Change Set A

Meanwhile:

User B modifies ADR-0017

Then:

User A attempts to apply Change Set A
```

ProjectConcord should determine whether User B's change invalidates any assumptions in Change Set A.

This is stronger than simply checking whether a file timestamp changed.

Semantic identity and relationship information can assist with conflict detection.

---

# 22. Audit History

Multi-user ProjectConcord should make it possible to determine:

```text
Who changed something?

What changed?

When?

Why?

What project operation caused it?

What was reviewed?

Who approved it?
```

Git may already provide part of this history for canonical repository artifacts.

ProjectConcord should use Git history where appropriate rather than duplicating it unnecessarily.

Operational database state may require separate audit history.

---

# 23. Repository Access

Cursor must evaluate how multiple desktop clients interact with the canonical EDF repository.

Potential architectural approaches may include:

- local Git clones with synchronization;
- server-managed repository access;
- ProjectConcord repository services;
- controlled change-set application;
- combinations of these.

This amendment does not prescribe the final solution.

However:

> Multiple clients must not independently modify a shared filesystem repository in an uncontrolled manner.

The architecture must provide a coherent concurrency and synchronization model.

---

# 24. Relationship Architecture

The previously defined CRA-aligned relationship architecture remains applicable.

Conceptually:

```text
CRA
Canonical Identity and Relationship Principles
       |
       v
EDF
Engineering Artifact and Relationship Semantics
       |
       v
ProjectConcord
Operational Collaboration
```

Multi-user support does not change this hierarchy.

It makes stable semantic identity even more important.

---

# 25. Operational Relationships

Not every ProjectConcord relationship necessarily belongs in canonical EDF engineering knowledge.

For example:

```text
T-184
    assigned-to -> User A
```

may be operational project-management state.

Whereas:

```text
SPEC-0032
    governed-by -> ADR-0017
```

may represent durable engineering semantics.

Cursor must preserve the distinction among:

```text
CRA-level canonical semantics

EDF engineering semantics

ProjectConcord operational/project-management state
```

Do not put all relationships into one undifferentiated store merely because they can be represented as graph edges.

---

# 26. User Relationships Must Not Pollute CRA

CRA should not become responsible for application-specific concepts such as:

```text
User session
Current dashboard
Notification
Task assignment
UI preference
```

unless CRA independently has a reason to model them.

ProjectConcord should use CRA where canonical representation concepts apply without turning CRA into the ProjectConcord application schema.

---

# 27. Role-Based Dashboards

The ProjectConcord dashboard architecture should support role/persona-sensitive views.

Conceptually:

```text
                    Same Project
                         |
          +--------------+--------------+
          |              |              |
          v              v              v
     Architect        Developer         QA
     Dashboard        Dashboard      Dashboard
          |              |              |
          +--------------+--------------+
                         |
                         v
                 Same Project State
```

The dashboard changes.

The underlying engineering truth does not.

---

# 28. Notifications

Multi-user operation creates a natural need for notifications.

Examples:

- architectural review requested;
- specification changed;
- task assigned;
- validation required;
- change set awaiting approval;
- conflict detected;
- milestone changed;
- gate satisfied;
- documentation reconciliation required.

Notification infrastructure need not be fully implemented in the first milestone.

However, architecture should not make it difficult to introduce.

---

# 29. Approval Workflows

Some operations may require approval depending on role and project policy.

Example:

```text
Developer
    |
    v
Proposes architectural-impacting change
    |
    v
Architect Review
    |
    +-- Approve
    +-- Reject
    +-- Request Revision
```

ProjectConcord should permit such workflows without hard-coding one universal approval process.

EDF may define some governance requirements.

Project configuration may define others.

Cursor must determine the correct boundary.

---

# 30. AI and Multi-User Operation

AI-assisted changes become even more important to govern in a collaborative environment.

AI should generally produce:

```text
Proposal
    |
    v
Validated Change Set
    |
    v
Human Review
    |
    v
Authorized Application
```

rather than silently changing shared canonical engineering artifacts.

The existing principle remains:

> AI proposes consequential engineering changes; humans retain authority.

---

# 31. Economical AI Remains Required

Multi-user architecture must not result in unnecessary AI usage.

ProjectConcord should continue to use deterministic processing for:

- identity;
- authorization;
- role resolution;
- artifact lookup;
- relationship lookup;
- Markdown path resolution;
- Git state;
- concurrency checks;
- validation;
- database queries;
- change-set mechanics.

AI should be used where semantic reasoning adds value.

Shared project services can make targeted context selection more efficient.

---

# 32. Future Web Client

A future web application should now be viewed as an **additional ProjectConcord client**, not the moment when ProjectConcord becomes multi-user.

Conceptually:

```text
                 ProjectConcord Platform
                          |
             +------------+------------+
             |                         |
             v                         v
       Avalonia Desktop            Web Client
             |                         |
             +------------+------------+
                          |
                          v
                  Application Services
                          |
                          v
                  Shared Project State
```

Multi-user capability therefore precedes the web client.

---

# 33. Desktop and Web Must Share Domain Semantics

If a future web client requires rewriting:

- EDF Engine;
- canonical authoring;
- relationship semantics;
- validation;
- Change Impact Engine;
- reconciliation logic;
- project domain;
- role/permission model;

then the architecture should be reconsidered.

UI technologies may differ.

Core engineering semantics should remain reusable.

---

# 34. Offline Capability

Cursor should evaluate whether desktop clients should eventually support limited offline operation.

Possible model:

```text
Connected
    |
    v
Synchronize Project State
    |
    v
Local Working Cache
    |
    v
Offline Work
    |
    v
Reconnect
    |
    v
Conflict / Synchronization Analysis
```

Offline capability is not necessarily an MVP requirement.

However, desktop architecture should avoid unnecessary assumptions that make it impossible.

---

# 35. Security

Introducing shared services requires architectural consideration of:

- authentication;
- authorization;
- secure transport;
- credential handling;
- project isolation;
- least privilege;
- auditability;
- session management;
- server trust boundaries.

Do not implement elaborate enterprise security prematurely.

Do establish correct architectural boundaries.

---

# 36. Project Membership

The domain should support a concept approximately equivalent to:

```text
Project
   |
   +-- Members
          |
          +-- User
                 |
                 +-- Role(s)
                 +-- Permissions
```

A user's roles may potentially differ between projects.

For example:

```text
User A

Project Alpha:
    Architect

Project Beta:
    Developer
    Reviewer
```

The architecture should not assume roles are necessarily global across every ProjectConcord project.

---

# 37. Data Ownership Classification

Cursor should classify ProjectConcord data into categories such as:

```text
1. Canonical Engineering Data
2. Canonical Project-Management Data, if EDF/project rules prescribe it
3. Shared Operational Data
4. Derived/Cached Data
5. User-Specific Data
6. Transient Collaboration State
```

For each category determine:

- authoritative source;
- persistence mechanism;
- synchronization behavior;
- audit requirements;
- offline behavior;
- backup implications.

This classification should occur before choosing database schemas.

---

# 38. Database Technology Must Follow Architecture

Do NOT choose a cloud database simply because multi-user support requires shared state.

First determine:

```text
What data exists?
      |
      v
Who owns it?
      |
      v
Is it canonical?
      |
      v
How is it synchronized?
      |
      v
What concurrency does it require?
      |
      v
Then choose persistence technology.
```

Database selection should be an architectural decision based on requirements.

---

# 39. ProjectConcord Platform Boundary

The architecture should now explicitly recognize ProjectConcord as more than an Avalonia executable.

Conceptually:

```text
              ProjectConcord
                  Platform
                     |
       +-------------+-------------+
       |             |             |
       v             v             v
     Client      Application     Shared
   Experiences     Services      Services
       |             |             |
       v             v             v
   Avalonia       EDF/Project    Identity
   Future Web     Authoring      Collaboration
                 Validation     Persistence
                 Reconcile
```

This does not require all components to become separate deployed services immediately.

It establishes architectural responsibility boundaries.

---

# 40. Revised MVP Interpretation

The MVP should still remain achievable.

Multi-user architectural support does NOT mean the first milestone must include:

- enterprise identity federation;
- sophisticated workflow engines;
- distributed microservices;
- advanced presence;
- offline synchronization;
- every possible role;
- web UI;
- real-time collaborative text editing.

Instead:

> **Design the foundations correctly now and implement collaboration incrementally.**

An early ProjectConcord release may support:

```text
Desktop Client
+
Authentication
+
Project Membership
+
Basic Roles
+
Shared Project Services
+
Shared Operational Database
+
Controlled Repository Access
+
Basic Concurrent-Change Detection
```

with advanced collaboration following later.

Cursor should determine the appropriate milestone sequence.

---

# 41. Required Amendment to Earlier Architecture

Cursor must locate statements in existing ProjectConcord documentation equivalent to:

```text
single-user desktop application
```

or:

```text
multi-user capability later with web version
```

and revise them.

The intended replacement concept is:

> **ProjectConcord begins as a native cross-platform desktop client, but the ProjectConcord architecture is multi-user and collaboration-capable from its foundation. Multiple authenticated desktop users may participate concurrently in shared projects through shared project services and shared operational data. A future web interface is an additional client, not the prerequisite for multi-user operation.**

Do not mechanically replace text without reviewing its architectural context.

Update affected assumptions, diagrams, requirements, milestones, and implementation plans accordingly.

---

# 42. Existing Documents Likely Affected

Cursor should identify the actual documents rather than relying on this list, but likely affected areas include:

- ProjectConcord architectural vision;
- system architecture;
- desktop architecture;
- future web architecture;
- EDF Engine boundaries;
- canonical authoring architecture;
- relationship/referential-integrity architecture;
- Git/repository architecture;
- Change Impact Engine;
- documentation reconciliation;
- Agile/project-management architecture;
- security architecture;
- persistence architecture;
- implementation milestones;
- MVP definition;
- non-goals;
- testing strategy.

---

# 43. Required Architectural Decisions to Evaluate

Cursor should determine whether ADRs are required for decisions such as:

1. ProjectConcord is desktop-first but multi-user.
2. Desktop clients access shared project/application services.
3. Canonical EDF artifacts remain repository-based rather than moving wholesale into the operational database.
4. Shared operational data uses a separate persistence model.
5. Authorization uses role/permission concepts.
6. Users may hold multiple project-specific roles.
7. Concurrency uses revision-aware/optimistic mechanisms where appropriate.
8. ProjectConcord change sets form a concurrency and review boundary.
9. The future web UI is another client of shared application services.

Do not automatically create nine ADRs.

Group decisions appropriately according to EDF governance.

---

# 44. Questions Cursor Must Answer

The revised architecture plan should explicitly address:

1. What is the ProjectConcord system boundary?
2. Which components execute locally?
3. Which components may execute remotely?
4. What constitutes a ProjectConcord user?
5. How is project membership represented?
6. Are roles project-specific?
7. Can a user have multiple roles?
8. Should persona/workspace be distinct from authorization role?
9. How are permissions represented?
10. Which data remains canonical in EDF/Git?
11. Which data belongs in shared operational storage?
12. Which data is derived/cacheable?
13. How do multiple clients access canonical repository artifacts?
14. How are concurrent edits detected?
15. How are conflicting changes handled?
16. How do ProjectConcord change sets participate in concurrency?
17. How does Git participate?
18. How is audit history divided between Git and ProjectConcord?
19. What minimum server/shared-service infrastructure is required for initial multi-user operation?
20. Which infrastructure can remain in-process initially?
21. How does an individual/local deployment work?
22. Can the same core architecture support local and shared deployments?
23. What changes when a web client is introduced?
24. Which core components remain unchanged?
25. What security boundaries are required now?
26. Which security capabilities can wait?
27. What EDF gaps are exposed?
28. What CRA considerations are exposed?

---

# 45. Required Testing Considerations

The architecture plan should include eventual testing for:

- two users reading the same project;
- two users editing unrelated artifacts;
- two users editing the same artifact;
- stale revision detection;
- simultaneous relationship changes;
- conflicting change sets;
- role-based permissions;
- unauthorized operations;
- multi-role users;
- project-specific roles;
- external Git changes during active work;
- shared repository synchronization;
- audit-history correctness;
- reconnect/recovery behavior where applicable.

These do not all need implementation in the first milestone.

They should inform architecture.

---

# 46. Required Cursor Planning Procedure

Cursor should perform the following in **Plan mode**.

## Step 1 — Locate This Amendment

Find this Markdown file in the ProjectConcord repository root.

The root location is temporary.

## Step 2 — Inspect Existing ProjectConcord Documentation

Determine exactly which previously incorporated architectural documents contain the earlier single-user assumption.

## Step 3 — Inspect EDF

Determine the EDF-compliant method for amending those documents and recording any new architectural decisions.

## Step 4 — Inspect CRA Where Applicable

Ensure shared identity/relationship architecture remains consistent with the previously established CRA alignment.

## Step 5 — Produce Affected-Document Analysis

Identify every document requiring modification and explain why.

## Step 6 — Amend Existing Architecture

Replace the obsolete:

```text
single-user desktop first
```

assumption with:

```text
desktop-first, multi-user architecture
```

throughout the architecture where applicable.

## Step 7 — Determine Fate of This Handover

Determine whether this document should:

- become a permanent EDF artifact;
- be incorporated into another architectural document;
- be split according to EDF;
- become one or more ADR inputs;
- be retained as historical planning material.

If retained, move it to the proper EDF location and give it an EDF-compliant filename/title.

## Step 8 — Update Implementation Planning

Revise milestones and dependencies to establish multi-user foundations without unnecessarily implementing full distributed-system complexity immediately.

## Step 9 — Identify Gaps

Classify issues as:

```text
CRA GAP
EDF GAP
PROJECTCONCORD ARCHITECTURAL DECISION
IMPLEMENTATION DECISION
```

## Step 10 — Stop for Architectural Review

Return the amended plan, affected-document analysis, proposed ADRs, and identified gaps for review before major implementation.

---

# 47. Final Architectural Principle

The obsolete assumption is:

> **ProjectConcord starts as a single-user desktop application and becomes multi-user when a web application is developed.**

The revised principle is:

> **ProjectConcord is a multi-user engineering project-management platform whose first client is a native cross-platform desktop application.**

Multiple users should ultimately be able to work concurrently on the same ProjectConcord project according to their responsibilities and permissions.

The desktop client remains important.

It simply does not define the system boundary.

The architecture should therefore support:

```text
                  ProjectConcord
                      Platform
                         |
          +--------------+--------------+
          |              |              |
          v              v              v
      Architect      Developer          QA
       Desktop        Desktop         Desktop
          |              |              |
          +--------------+--------------+
                         |
                         v
                 Shared Services
                         |
              +----------+----------+
              |                     |
              v                     v
        Operational Data       Canonical EDF
                               Project State
```

A future web client should join this architecture rather than cause it to be redesigned.

ProjectConcord's goal remains the same:

> **Keep engineering intent, implementation, validation, documentation, and project work in concord — now across the entire engineering team.**