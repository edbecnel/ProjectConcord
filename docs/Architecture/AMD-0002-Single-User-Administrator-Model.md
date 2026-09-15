[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › AMD-0002

# AMD-0002: Single-User Administrator Model

> **Status:** Architectural Amendment — integrated 2026-09-15  
> **Document ID:** AMD-0002  
> **Companion:** [AMD-0001 — Multi-User Desktop and Shared Project Services](AMD-0001-Multi-User-Desktop-and-Shared-Project-Services.md)  
> **Normative decisions:** [ADR-0010](ADRs/ADR-0010-Single-User-Administrator-Default-Model.md)

Supplement to AMD-0001. Sections 48–58 capture the single-user administrator requirements that must coexist with the multi-user platform foundation.

---

# 48. Single-User Administrator Model

ProjectConcord's multi-user architecture must not make the application unnecessarily complicated for an individual developer, engineer, researcher, or other user operating a project alone.

A single-user ProjectConcord project should default to a simple model:

```text
Project Owner
     |
     v
Administrator
     |
     v
Full Project Permissions
```

The initial project owner should automatically receive the **Administrator** role unless project configuration explicitly specifies otherwise.

An Administrator should have permission to perform all normal ProjectConcord project operations without requiring assignment of separate roles such as:

- Architect
- Developer
- Development Manager
- Project Manager
- QA Tester
- Validation Engineer
- Documentation Technician
- Reviewer

This allows ProjectConcord to support a "one-person engineering team" without forcing the user to manage unnecessary role assignments.

---

# 49. Single-User Mode Must Use the Same Architecture

This convenience must NOT result in a separate single-user architecture.

Avoid:

```text
Single-User ProjectConcord
        +
Multi-User ProjectConcord
```

Prefer:

```text
             ProjectConcord
           Multi-User Model
                  |
        +---------+---------+
        |                   |
        v                   v
 Individual Project      Team Project
        |                   |
        v                   v
 Administrator         Multiple Users
  Full Access          Assigned Roles
```

A single-user project is therefore simply the simplest configuration of the normal ProjectConcord identity, authorization, and project-membership architecture.

This avoids creating separate code paths or security models that would later need to be reconciled.

---

# 50. Administrator Role

The Administrator role should conceptually provide full project-level permissions.

For example:

```text
Administrator
     |
     +-- View Artifacts
     +-- Create Artifacts
     +-- Edit Artifacts
     +-- Move/Rename Artifacts
     +-- Supersede Artifacts
     +-- Manage Relationships
     +-- Manage Architecture
     +-- Manage Development
     +-- Manage Validation
     +-- Manage Documentation
     +-- Manage Project Work
     +-- Approve Changes
     +-- Manage Project Configuration
     +-- Manage Users/Roles when applicable
```

The exact permission vocabulary should be determined by the ProjectConcord authorization architecture.

Do not duplicate every role internally merely to give an Administrator complete access.

Instead, Administrator should resolve to the necessary permissions through the authorization system.

---

# 51. Roles and Workspaces Should Remain Distinct

This requirement further supports distinguishing **authorization roles** from **working personas/workspaces**.

A role answers:

> **What is this user authorized to do?**

A workspace or persona answers:

> **What part of the project is this user currently working with?**

For example, a single Administrator may want to switch among:

```text
Architecture Workspace

Development Workspace

Project Management Workspace

QA / Validation Workspace

Documentation Workspace
```

without changing roles.

Conceptually:

```text
              Administrator
              Full Access
                   |
       +-----------+-----------+
       |           |           |
       v           v           v
 Architecture  Development   QA / Validation
  Workspace     Workspace      Workspace
       |
       +--------------------------+
                                  |
                                  v
                         Documentation /
                        Project Management
                           Workspaces
```

These workspaces may alter:

- dashboard emphasis;
- navigation;
- available shortcuts;
- default views;
- notifications displayed;
- workflow presentation.

They should not unnecessarily alter the Administrator's underlying authorization.

---

# 52. Simple Project Creation

When a user creates a new ProjectConcord project intended for individual use, onboarding should require little or no role configuration.

A reasonable default is:

```text
Create Project
      |
      v
Creating User
      |
      v
Project Owner
      |
      v
Administrator
      |
      v
Ready to Work
```

The user should not be forced through a role-management wizard merely to begin using ProjectConcord.

The multi-user architecture should remain largely invisible until collaboration is required.

---

# 53. Transition From Individual to Team Project

An important consequence is that an individual project should be able to become collaborative without architectural conversion.

For example:

```text
INITIAL

Project
   |
   v
Ed
Administrator


LATER

Project
   |
   +-- Ed
   |    +-- Administrator
   |    +-- Architect
   |
   +-- Developer A
   |    +-- Developer
   |
   +-- Tester B
   |    +-- QA Tester
   |
   +-- Documentation C
        +-- Documentation Technician
```

The project itself should not need to be converted from a "single-user project" into a fundamentally different "team project."

Instead, additional project members are invited or created and assigned appropriate roles and permissions.

---

# 54. Local Operation

Where ProjectConcord supports a purely local installation, the same conceptual model should still apply.

For example:

```text
Local ProjectConcord
       |
       v
Local User Identity
       |
       v
Administrator
       |
       v
Full Access
```

The implementation may simplify authentication where appropriate for a local-only deployment, but the domain model should remain compatible with the shared multi-user architecture.

This allows local projects to migrate to shared ProjectConcord services later without requiring fundamental changes to their project model.

---

# 55. Project Ownership

Cursor should evaluate whether **Project Owner** should be:

- a distinct role;
- a project-membership property;
- an ownership relationship;
- or another domain concept.

Do not automatically equate ownership with Administrator.

For an initial individual project, the creating user may normally be both:

```text
Project Owner
+
Administrator
```

But a future organization may have Administrators who do not own the project.

The architecture should preserve this distinction if it is meaningful.

---

# 56. No Artificial Workflow Restrictions for Administrators

A one-person engineering project should not become cumbersome because workflows were designed primarily for teams.

For example, ProjectConcord should not require:

```text
Developer Ed
    |
    v
Submit for Architect Approval
    |
    v
Architect Ed
    |
    v
Approve
```

merely because the architecture supports separate Developer and Architect roles.

Where EDF governance permits it, an Administrator working alone should be able to perform the necessary operation directly.

ProjectConcord may still record that the same individual authored and approved an action where audit information is useful.

If EDF itself requires independent review for a particular engineering process, ProjectConcord must respect that requirement rather than bypassing it through Administrator privileges.

---

# 57. Multi-User Capability Without Multi-User Burden

This leads to an important ProjectConcord usability principle:

> **ProjectConcord should provide multi-user capability without imposing multi-user administrative burden on a single user.**

The architecture should scale in both directions:

```text
One Person
    |
    v
Administrator
    |
    v
Simple Experience


Small Team
    |
    v
Users + Roles
    |
    v
Coordinated Experience


Larger Team
    |
    v
Users + Roles + Permissions + Workflows
    |
    v
Governed Collaborative Experience
```

The same underlying architecture supports all three.

---

# 58. Required Cursor Amendment

When processing the preceding multi-user architectural amendment, Cursor must incorporate this additional requirement into the affected ProjectConcord documentation.

Specifically, Cursor should ensure that the architecture establishes:

1. the creating user as the default Administrator for an individual project;
2. Administrator as a full-access project role;
3. no requirement for an individual user to assign themselves every functional role;
4. separation between authorization roles and role-oriented personas/workspaces where appropriate;
5. support for multiple workspaces/perspectives while retaining Administrator permissions;
6. simple individual-project onboarding;
7. seamless transition from an individual project to a collaborative project;
8. no separate or incompatible single-user architecture;
9. avoidance of unnecessary approval bureaucracy when one person legitimately performs multiple engineering responsibilities;
10. continued enforcement of any independent-review requirement actually prescribed by EDF.

Cursor should update existing ProjectConcord architectural documents, requirements, diagrams, role models, onboarding assumptions, and implementation planning where necessary.

The resulting architecture should satisfy both requirements simultaneously:

> **ProjectConcord is multi-user from its architectural foundation.**

and:

> **For a one-person project, ProjectConcord should feel like a straightforward single-user engineering application with full Administrator access.**