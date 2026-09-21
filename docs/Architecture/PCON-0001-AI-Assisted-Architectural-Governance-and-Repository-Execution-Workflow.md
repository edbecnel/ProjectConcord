[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › PCON-0001

# PCON-0001: AI-Assisted Architectural Governance and Repository Execution Workflow

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural Discovery Record |
| **Normative** | No |
| **Status** | Proposed — architectural input from Snaptara governance generalization |
| **Record ID** | PCON-0001 |
| **Date** | 2026-09-21 |
| **Owner** | ProjectConcord |
| **Authoritative** | No — candidate models and requirements; normative behavior in [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) and [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) when accepted |
| **Source** | Root handover `ProjectConcord_AI_Assisted_Architectural_Governance_and_Repository_Execution_Workflow.md` (integrated 2026-09-21) |

---

# ProjectConcord --- AI-Assisted Architectural Governance and Repository Execution Workflow

**Document Type:** Architectural Handover / Candidate ProjectConcord
Specification Input\
**Status:** PROPOSED --- FOR PROJECTCONCORD ARCHITECTURAL REVIEW\
**Origin:** Generalized from the development governance workflow used
during Snaptara development\
**Intended Consumer:** ProjectConcord architecture and implementation
planning\
**Canonical Status:** This document is an input to ProjectConcord. It
does not supersede existing accepted EDF artifacts, ADRs, AARs,
specifications, gates, or ProjectConcord architecture.

------------------------------------------------------------------------

## Handover Instructions to ProjectConcord / Repository Agent

This document is being handed to the ProjectConcord repository as an
**architectural input**, not as pre-authorized implementation work.

Before modifying the repository:

1.  Inspect the current ProjectConcord EDF structure, project context,
    accepted architecture, ADRs, AARs, specifications, roadmap, gates,
    watch items, and relevant implementation.
2.  Determine the correct EDF-prescribed destination, document
    classification, filename, and relationships for this material.
3.  Reconcile this proposal with existing ProjectConcord architecture
    rather than creating a competing architecture.
4.  Identify which portions belong in:
    -   architectural specifications;
    -   ADRs;
    -   workflow/process specifications;
    -   canonical domain/data models;
    -   integration/provider specifications;
    -   UI/UX specifications;
    -   security/audit specifications;
    -   roadmap/gate amendments;
    -   watch items.
5.  Preserve the architectural distinction between **canonical
    governance state** and **rendered AI prompts/messages**.
6.  Do not begin production implementation merely because this document
    describes implementation concepts.
7.  Return a repository-informed plan and any architectural
    questions/conflicts for Project Architect review before
    implementation.

------------------------------------------------------------------------

# 1. Purpose

ProjectConcord is intended to be more than an EDF document browser,
project dashboard, Markdown editor, Git viewer, or AI chat interface.

A major ProjectConcord capability should be the governed coordination
of:

-   human architectural authority;
-   an architectural/review AI such as GPT;
-   EDF canonical project state;
-   repository-aware coding agents such as Cursor, GitHub Copilot, or
    future providers;
-   Git repository state;
-   implementation plans;
-   authorized code/document changes;
-   automated validation;
-   operator/manual validation;
-   evidence;
-   architectural review;
-   gate/tranche acceptance.

This document specifies a provider-neutral workflow for that
coordination.

The workflow is derived from a development process exercised during
Snaptara development, where ChatGPT served as Project Architect support
and Cursor served as the repository-aware planning/implementation agent.

The key lesson is that the process should **not** be modeled merely as
two AI chats exchanging text.

It should be modeled as a governed architectural workflow with canonical
state, explicit authorization boundaries, structured handovers,
structured submissions, evidence, and human authority.

------------------------------------------------------------------------

# 2. Architectural Principle

The central principle is:

> **Canonical project governance is primary; AI conversations and
> provider-specific prompts are derived representations and transport
> mechanisms.**

ProjectConcord should not make a Cursor prompt, GPT conversation,
Copilot session, or chat transcript the canonical record of project
authorization.

Instead:

``` text
Canonical Project State
        |
        +-- EDF architecture
        +-- accepted decisions
        +-- gates/stages/tranches
        +-- work authorizations
        +-- repository baselines
        +-- known defects/waivers
        +-- submissions/evidence
        +-- review decisions
        |
        v
Derived Representations
        |
        +-- GPT architectural context
        +-- Cursor handover
        +-- Copilot handover
        +-- manual copy/paste package
        +-- direct provider request
        +-- human-readable report
```

This permits ProjectConcord to support different AI providers without
allowing any provider's conversation format to become the project
architecture.

------------------------------------------------------------------------

# 3. Human Authority

The human user remains the project authority.

AI systems may:

-   analyze;
-   recommend;
-   identify conflicts;
-   generate plans;
-   implement authorized work;
-   execute validation;
-   produce evidence;
-   propose architectural decisions;
-   review conformance.

AI systems must not implicitly acquire authority merely because they can
modify the repository.

ProjectConcord should therefore distinguish at least:

1.  **recommendation**;
2.  **authorization**;
3.  **execution**;
4.  **submission**;
5.  **review**;
6.  **acceptance**.

These states must not collapse into one another.

A successful implementation is not automatically accepted architecture.

A passing test suite is not automatically gate acceptance.

A commit is not automatically authorization for the next tranche.

------------------------------------------------------------------------

# 4. Logical Roles

## 4.1 Human Project Authority

The human:

-   owns final project authority;
-   approves or rejects architectural recommendations;
-   authorizes governed work;
-   performs manual validation when required;
-   may stop work;
-   accepts or rejects stage/gate completion;
-   resolves decisions requiring human judgment.

ProjectConcord may support role-based authority in future multi-user
deployments, but authority must remain explicit.

------------------------------------------------------------------------

## 4.2 Architectural AI

The Architectural AI performs the role currently demonstrated by GPT in
the Snaptara workflow.

Responsibilities may include:

-   requirements analysis;
-   architectural reasoning;
-   architectural specifications;
-   ADR/AAR review;
-   stage/tranche decomposition;
-   implementation-plan review;
-   architectural conflict analysis;
-   scope control;
-   baseline-defect classification;
-   evidence review;
-   remediation recommendations;
-   closeout review;
-   generation of repository-agent handovers;
-   generation of the next proposed authorization.

The Architectural AI should normally reason from canonical
ProjectConcord/EDF state rather than depending on the entire historical
chat transcript.

------------------------------------------------------------------------

## 4.3 Repository Execution Agent

Examples include:

-   Cursor;
-   GitHub Copilot;
-   another repository-aware AI development environment;
-   a future ProjectConcord-native coding agent.

Responsibilities may include:

-   repository inspection;
-   codebase discovery;
-   implementation planning;
-   implementation when explicitly authorized;
-   test execution;
-   validation;
-   Git operations when explicitly authorized;
-   evidence collection;
-   EDF updates when explicitly authorized;
-   reporting unexpected dependencies and conflicts.

Repository access does **not** grant architectural authority.

------------------------------------------------------------------------

## 4.4 ProjectConcord

ProjectConcord is the governed coordination and canonical-state layer.

It should eventually:

-   understand EDF project structure;
-   maintain canonical workflow state;
-   know accepted architectural artifacts;
-   know current gates/stages/tranches;
-   track authorizations;
-   track repository baselines;
-   track known defects and waivers;
-   generate provider-neutral handovers;
-   render provider-specific messages;
-   receive submissions;
-   validate submissions against authorization scope;
-   coordinate Architectural AI review;
-   record human decisions;
-   preserve audit history.

ProjectConcord is not merely a relay between two chat systems.

------------------------------------------------------------------------

# 4A. Human-Initiated Work and Out-of-Band Intervention

The Human Project Authority is not merely an approval checkpoint in the
active governed workflow.

Development is not linear. While one authorized tranche is being
planned, implemented, validated, or reviewed, the human may discover or
introduce work that is legitimate but does not originate from the active
WorkAuthorization.

Examples include:

-   documenting a new product idea;
-   proposing a future feature;
-   reporting a newly discovered bug;
-   requesting research;
-   recording an architectural concern;
-   requesting documentation unrelated to the current tranche;
-   identifying an EDF or ProjectConcord process improvement;
-   creating work intended for another project;
-   reporting a manual repository change made outside the active
    workflow;
-   reporting a change made by an AI coding agent that was not part of
    the authorized workflow;
-   asking the Architectural AI to evaluate whether an unexpected change
    affects current work.

The governing principle is:

> **Human-initiated work does not have to originate from the active
> governed workflow, but it must not silently alter the meaning,
> baseline, scope, evidence, or acceptance state of that workflow.**

ProjectConcord should provide a first-class capture and triage path for
such interventions.

Conceptually:

``` text
                     HUMAN AUTHORITY
                           |
             +-------------+-------------+
             |                           |
             v                           v
     Active Governed Work        Human Intervention
             |                           |
             |                  capture / classify
             |                           |
             |              +------------+------------+
             |              |            |            |
             |              v            v            v
             |          Current       Future       Other
             |          Project       Work         Project
             |              |            |            |
             |              +------> Backlog          |
             |              |       / AWI / Idea      |
             |              |                         |
             |              v                         v
             |        Impact Assessment         Route / Export
             |              |
             +--------------+
                    if relevant
                         |
                         v
                Architectural Review
```

ProjectConcord should therefore support an **Inbox / Triage** capability
where the human can capture ideas and observations immediately without
contaminating active authorized work.

A captured item may later become:

-   a bug;
-   backlog item;
-   feature request;
-   research task;
-   documentation task;
-   Architectural Watch Item;
-   ADR/AAR input;
-   EDF improvement;
-   new stage/tranche;
-   amendment to existing work;
-   separate project work;
-   cross-project concern.

Capture does not itself imply implementation authorization.

------------------------------------------------------------------------

# 4B. Candidate HumanInitiatedWorkItem Model

A candidate canonical model is:

``` text
HumanInitiatedWorkItem
{
    Id
    ProjectId?
    CreatedBy
    CreatedAt

    Type
    Description
    OriginContext

    RelationshipToActiveWork
    Target

    RelatedAuthorizationId?
    RelatedRepositoryState?
    RelatedArtifacts[]

    Disposition
}
```

Candidate types:

``` text
IDEA
FEATURE_REQUEST
BUG_REPORT
RESEARCH_REQUEST
DOCUMENTATION_REQUEST
ARCHITECTURAL_CONCERN
OUT_OF_BAND_CHANGE
PROCESS_IMPROVEMENT
CROSS_PROJECT_ITEM
OTHER
```

Candidate relationship classifications:

``` text
UNRELATED
RELATED_NONBLOCKING
POTENTIALLY_IMPACTING
BLOCKING
UNKNOWN
```

Candidate targets:

``` text
CURRENT_PROJECT
OTHER_PROJECT
EDF
PROJECTCONCORD
CROSS_PROJECT
```

Candidate dispositions:

``` text
CAPTURED
TRIAGE_REQUIRED
BACKLOG
WATCH_ITEM
ARCHITECT_REVIEW
NEW_WORKFLOW
MERGED_INTO_EXISTING_WORK
DEFERRED
REJECTED
COMPLETED
```

The exact vocabulary should be reconciled with existing EDF and
ProjectConcord concepts.

------------------------------------------------------------------------

# 4C. Human Authority vs. Workflow Integrity

Human authority and workflow integrity are compatible but distinct.

The human may intentionally act outside the active authorization.
ProjectConcord should not automatically characterize a human-originated
out-of-band change as an unauthorized violation.

Instead, it should detect and surface the divergence:

> **Repository or project state changed outside the active
> WorkAuthorization. Reconciliation is required before affected governed
> work proceeds.**

The Architectural AI can then help classify the change.

Possible dispositions include:

-   unrelated --- preserve separately and continue;
-   related but nonblocking --- record and continue;
-   baseline-changing --- update/supersede the active authorization;
-   plan-affecting --- return the active plan for review;
-   scope amendment required;
-   separate work item required;
-   architectural review required;
-   revert requested by the human;
-   active work must STOP pending reconciliation.

This protects governance without making ProjectConcord rigid or hostile
to legitimate human intervention.

------------------------------------------------------------------------

# 4D. AI-Agent Out-of-Band Changes

ProjectConcord must also distinguish **human-directed intervention**
from an AI repository agent exceeding its authorization.

If Cursor, Copilot, or another agent changes repository state outside
the authorized scope, the event should be captured and reported to the
Architectural AI and human authority.

ProjectConcord should preserve:

-   active authorization;
-   expected scope;
-   actual changed files/commits;
-   detected deviation;
-   agent explanation, if available;
-   repository state before and after;
-   architectural impact assessment;
-   human disposition.

The agent should not silently normalize the deviation by editing the
authorization after the fact.

A human may choose to accept the change, incorporate it through an
amendment, split it into separate work, or revert it.

------------------------------------------------------------------------

# 4E. Cross-Project Capture and Routing

Human-originated ideas may apply to a different project or to shared
infrastructure such as EDF.

ProjectConcord should allow capture without forcing the user to abandon
the current project context.

For example:

``` text
Current Project: Snaptara

Human observation:
"This authorization workflow should become a ProjectConcord feature."

Target:
ProjectConcord

Disposition:
Cross-project item / architectural input
```

Similarly, a ProjectConcord discovery may reveal an EDF improvement.

ProjectConcord should preserve the item's origin and route it to the
target project/framework without falsely making it part of the current
project's active gate.

This supports architectural learning across projects while preserving
project boundaries.

# 4F. Multi-Project Workspace

ProjectConcord should be architected as a **workspace containing
multiple independently governed projects**, rather than assuming only
one project can be known or active.

Each project retains its own canonical EDF state, repository and Git
history, branches/worktrees, accepted architecture,
gates/stages/tranches, DevelopmentWorkAuthorizations, defects/waivers,
evidence, audit history, and authority boundaries. The workspace
coordinates above those projects without collapsing them into one global
governance state.

A user should be able to move among ProjectConcord, EDF, Snaptara, CRA,
The Recipe Vault, and other registered projects within the same UI while
ProjectConcord preserves the selected project/repository/branch context
and prevents accidental cross-project mutation.

# 4G. Governed Inter-Project Handover

Work in one project may expose a requirement that properly belongs to
another project. For example, Snaptara may expose a missing EDF
capability.

The source project may discover, explain, and propose the change, but it
must not declare the target project's architecture accepted.

> **A cross-project discovery retains source provenance, but the
> destination project's governance controls disposition and
> acceptance.**

An inter-project handover should preserve the source project/context,
originating authorization or human work item where applicable, relevant
artifacts/commits, rationale, evidence, proposed target change, and
whether source work is blocked or can proceed with a workaround.

# 4H. Candidate InterProjectHandover Model

``` text
InterProjectHandover
{
    Id
    SourceProjectId
    SourceContext
    SourceAuthorizationId?
    SourceWorkItemId?
    SourceArtifactIds[]
    SourceCommitIds[]

    TargetProjectId
    HandoverType
    Rationale
    ProposedContent
    SupportingEvidence[]

    TargetDisposition
    TargetArtifactIds[]
    TargetBranch?
    PullRequestId?
    AcceptedCommitIds[]
    AcceptedTargetVersion?

    DependencyRelationship
}
```

Candidate handover types include `SPECIFICATION_PROPOSAL`,
`ARCHITECTURE_PROPOSAL`, `BUG_REPORT`, `FEATURE_REQUEST`,
`EDF_CAPABILITY_REQUEST`, `SHARED_COMPONENT_CHANGE`, `RESEARCH_FINDING`,
`DEPENDENCY_CHANGE`, and `OTHER`.

Candidate target dispositions include `RECEIVED`, `TRIAGE`, `PLANNING`,
`BRANCH_CREATED`, `PR_OPEN`, `REVISION_REQUESTED`, `ACCEPTED`,
`REJECTED`, and `DEFERRED`.

# 4I. Destination Governance and Git Contribution Workflow

An inter-project handover may be materialized through the destination
project's normal Git contribution process. A proposal discovered in
Snaptara and targeted at EDF could become an EDF-compliant specification
on an EDF branch, be validated, committed, and submitted through a pull
request.

A branch, commit, or pull request is evidence and a collaboration/review
mechanism. It is **not by itself architectural acceptance**.
Target-project governance remains authoritative.

Direct creation of target branches, commits, or pull requests remains
subject to applicable human permissions and
DevelopmentWorkAuthorization.

# 4J. Inter-Project Handover vs. Cross-Project Dependency

ProjectConcord should distinguish an **inter-project handover**---an
event transferring a proposal, discovery, defect, request, or
contribution---from a **cross-project dependency**---an ongoing
relationship indicating that source work depends on a target capability,
artifact, accepted change, commit, or version.

A handover may create a dependency, but not every handover is blocking
and not every dependency originates as a handover.

Dependencies should be typed so ProjectConcord can represent source work
that is informationally related, blocked by target acceptance, able to
proceed with a workaround, or dependent on a minimum target version.

# 4K. Cross-Project Provenance and Lifecycle

ProjectConcord should preserve traceability from source discovery
through target disposition:

``` text
Source discovery
  -> HumanInitiatedWorkItem / DevelopmentWorkAuthorization
  -> InterProjectHandover
  -> Target triage
  -> Target architecture/specification/work authorization
  -> Target branch / PR / implementation
  -> Target acceptance
  -> Accepted target commit/version
  -> Source dependency satisfied / source work resumed
```

This relationship should survive conversation rollover, provider
changes, branch changes, and movement of Markdown files.

# 5. Provider-Neutral Architecture

ProjectConcord should define a provider-neutral **Architect--Repository
Agent Protocol**.

Manual copy/paste and direct integrations are adapters to this protocol.

Conceptually:

``` text
                     Human Authority
                           |
                           v
                    ProjectConcord
                  Canonical Governance
                     /           \
                    /             \
                   v               v
          Architectural AI   Repository Agent
               GPT           Cursor / Copilot /
                              Other Provider
```

Provider-specific APIs, prompt formats, IDE capabilities, or
conversation models must remain outside the canonical governance model.

------------------------------------------------------------------------

# 6. Two Required Operating Modes

## 6.1 Mode A --- Manual Handover

This is the currently proven workflow.

Example:

``` text
ProjectConcord / GPT
        |
        | generates handover + authorization
        v
Human copies message
        |
        v
Cursor Plan / Agent
        |
        | returns plan/report
        v
Human copies response
        |
        v
ProjectConcord / GPT
        |
        | reviews
        v
Human authorizes next action
```

Manual mode is important because:

-   direct integrations may not exist;
-   a provider may not expose an API;
-   the user may prefer explicit control;
-   provider capabilities may vary;
-   it provides a universal fallback.

ProjectConcord should make manual mode efficient by producing complete
copy-ready packages and structured import surfaces for returned
responses.

------------------------------------------------------------------------

## 6.2 Mode B --- Integrated Agent

In integrated mode:

``` text
ProjectConcord
      |
      | governed request
      v
Provider Adapter
      |
      v
Repository Agent
      |
      | structured response/evidence
      v
Provider Adapter
      |
      v
ProjectConcord
```

The governance semantics must remain the same as manual mode.

Integration changes **transport**, not **authority**.

A direct Cursor or Copilot integration must not silently weaken
authorization boundaries that would exist in manual mode.

------------------------------------------------------------------------

# 7. Handover and Authorization Are Different Objects

One of the most important lessons from the Snaptara workflow is that a
new repository-agent conversation requires two logically separate
things.

## 7.1 Handover

The handover answers:

> **What project state am I inheriting?**

Typical content:

-   project/program identity;
-   accepted architecture;
-   current stage/gate/tranche;
-   accepted commit hashes;
-   canonical documents;
-   known baseline defects;
-   waivers;
-   recent unrelated work;
-   working-tree state;
-   branch/repository state;
-   important architectural invariants;
-   unresolved questions;
-   previous stop point.

------------------------------------------------------------------------

## 7.2 Authorization

The authorization answers:

> **What am I permitted to do now?**

Typical content:

-   authorization type;
-   exact scope;
-   permitted operations;
-   prohibited operations;
-   required repository baseline;
-   required inputs;
-   required outputs;
-   validation expectations;
-   evidence expectations;
-   stop conditions;
-   whether commits are permitted;
-   whether pushes are permitted;
-   whether implementation is permitted.

ProjectConcord should model these separately even when it renders them
into one message.

------------------------------------------------------------------------

# 8. Candidate Canonical WorkAuthorization Model

A future canonical model may resemble:

``` text
WorkAuthorization
{
    AuthorizationId
    ProjectId

    ProgramId?
    GateId?
    StageId?
    TrancheId?

    AuthorizationType

    Scope
    PermittedOperations[]
    ProhibitedOperations[]

    AcceptedBaseline
    RepositoryBaseline

    RequiredInputs[]
    RequiredOutputs[]

    KnownDefects[]
    ApplicableWaivers[]

    ValidationRequirements[]
    EvidenceRequirements[]
    StopConditions[]

    IssuedBy
    IssuedAt

    Status
}
```

Candidate authorization types:

``` text
RESEARCH
ARCHITECTURE
PLANNING
DOCUMENTATION
IMPLEMENTATION
VALIDATION
REMEDIATION
CLOSEOUT
```

Candidate status vocabulary:

``` text
DRAFT
AUTHORIZED
IN_PROGRESS
STOPPED
SUBMITTED_FOR_REVIEW
ACCEPTED
REJECTED
SUPERSEDED
CANCELLED
```

Exact terminology should be reconciled with EDF and existing
ProjectConcord architecture.

------------------------------------------------------------------------

# 9. Authorization Must Be Capability-Bounded

An authorization should define operations, not merely a natural-language
goal.

Examples:

## Planning authorization

May permit:

-   read repository;
-   inspect history;
-   inspect EDF documents;
-   inspect code/tests;
-   formulate plan.

May prohibit:

-   modifying files;
-   committing;
-   pushing;
-   implementing;
-   updating EDF status.

## Implementation authorization

May permit:

-   modify specified production surfaces;
-   add/modify specified tests;
-   run validation;
-   create evidence;
-   commit.

May prohibit:

-   unrelated refactors;
-   later-tranche implementation;
-   undocumented architectural changes;
-   push/rebase/reset unless separately allowed.

ProjectConcord should be able to compare returned actions against these
capabilities.

------------------------------------------------------------------------

# 10. STOP Semantics

`STOP` should be a first-class workflow state, not merely conversational
wording.

A stop may occur because:

-   architectural ambiguity is discovered;
-   repository reality contradicts the plan;
-   unrelated working-tree changes exist;
-   baseline is not clean;
-   a required manual validation cannot be performed;
-   implementation would exceed authorized scope;
-   an accepted artifact conflicts with another;
-   a test failure may be a baseline defect;
-   an unexpected dependency crosses a stage/tranche boundary.

When stopped:

1.  no additional unauthorized mutation occurs;
2.  repository state is reported;
3.  reason is recorded;
4.  required decision is identified;
5.  work resumes only after explicit disposition/authorization.

------------------------------------------------------------------------

# 11. Core Development Loop

The generalized workflow is:

``` text
Requirement / Architectural Need
              |
              v
      Architectural Analysis
              |
              v
     Proposed Architecture
              |
              v
     EDF Architectural State
              |
              v
    Planning Authorization
              |
              v
 Repository Read-Only Inspection
              |
              v
      Implementation Plan
              |
              v
   Project Architect Review
        /            \
   Amend              Accept
     |                  |
     +------------------+
              |
              v
  Implementation Authorization
              |
              v
      Repository Execution
              |
              v
     Tests / Validation
              |
              v
 Implementation + Evidence Submission
              |
              v
   Project Architect Review
        /            \
 Remediate            Accept
     |                  |
     +------------------+
              |
              v
        EDF Closeout
              |
              v
    Architectural Acceptance
              |
              v
      Next Authorized Work
```

No arrow should imply automatic authorization where human/project policy
requires explicit approval.

------------------------------------------------------------------------

# 12. Planning Phase

The repository agent should inspect actual repository state before
producing an implementation plan.

The planning phase should answer:

-   What existing types/surfaces are relevant?
-   What accepted architecture already exists?
-   What exact files are expected to change?
-   What tests are expected to change?
-   What persistence/API implications exist?
-   What dependencies cross the requested scope?
-   Are there contradictions with accepted architecture?
-   Are there known baseline defects?
-   Can the requested work remain one coherent tranche?
-   What questions require Project Architect disposition?

A planning response is a **proposal**, not implementation authorization.

------------------------------------------------------------------------

# 13. Project Architect Plan Review

The Architectural AI should evaluate the plan against:

-   accepted EDF architecture;
-   ADRs;
-   AARs;
-   specifications;
-   stage/gate scope;
-   architectural invariants;
-   separation of concerns;
-   dependency ownership;
-   persistence compatibility;
-   migration requirements;
-   testing strategy;
-   future architecture;
-   known watch items.

Possible outcomes:

``` text
ACCEPT PLAN
ACCEPT WITH AMENDMENTS
RETURN FOR REVISION
STOP / ARCHITECTURAL DECISION REQUIRED
```

ProjectConcord should record the disposition.

------------------------------------------------------------------------

# 14. Implementation Authorization

Only after plan acceptance should implementation normally be authorized.

The implementation authorization should reference:

-   accepted plan/version;
-   repository baseline;
-   exact scope;
-   permitted files or architectural surfaces where practical;
-   tests required;
-   evidence required;
-   commit policy;
-   explicit exclusions;
-   stop conditions.

The authorization should not be inferred from a plan being accepted.

------------------------------------------------------------------------

# 15. Repository Execution

The repository agent performs only authorized work.

If implementation reveals a material architectural deviation, the agent
should stop rather than silently redesign the system.

Examples:

-   required change crosses into a later tranche;
-   persistence architecture differs materially from planning
    assumptions;
-   a supposedly derived component actually owns canonical state;
-   a dependency requires modifying an explicitly excluded subsystem;
-   migration cannot preserve required identity;
-   a baseline defect blocks validation.

------------------------------------------------------------------------

# 16. Validation Model

ProjectConcord should distinguish:

## 16.1 Automated validation

Examples:

-   build;
-   unit tests;
-   integration tests;
-   persistence round-trip tests;
-   architecture/conformance scripts;
-   static analysis.

## 16.2 Agent-performable interactive validation

Available only where the repository agent can actually operate the
UI/environment.

## 16.3 Human/operator validation

Required where the agent cannot reliably perform the action.

Examples may include:

-   desktop UI behavior;
-   visual layout;
-   hardware interaction;
-   external system behavior;
-   subjective UX acceptance.

An agent must never claim PASS for validation it could not perform.

ProjectConcord should support a state such as:

``` text
AUTOMATED_PASS
OPERATOR_VALIDATION_REQUIRED
OPERATOR_PASS
OPERATOR_FAIL
NOT_APPLICABLE
```

------------------------------------------------------------------------

# 17. Baseline Defects vs Regressions

Known failures must be classified.

A failure may be:

-   pre-existing baseline defect;
-   known waived defect;
-   new regression;
-   environment/tool limitation;
-   unrelated failure;
-   direct blocker.

ProjectConcord should preserve baseline-defect identity across tranches.

An old baseline failure must not be silently counted as a new
regression.

Conversely, labeling a failure "baseline" should require evidence that
it predates the authorized change.

------------------------------------------------------------------------

# 18. Working Tree and Repository Hygiene

Before governed work begins, ProjectConcord should know:

-   branch;
-   HEAD;
-   upstream state;
-   working-tree cleanliness;
-   untracked files;
-   unrelated WIP;
-   accepted baseline hash.

Unrelated WIP should not be silently mixed into governed work.

Candidate disposition workflow:

``` text
Unrelated WIP Detected
        |
        v
Read-Only Inspection
        |
        +-- coherent completed work -> validate/commit separately
        |
        +-- temporary work -> stash if authorized
        |
        +-- accidental work -> discard if authorized
        |
        +-- uncertain -> STOP
```

The repository agent should never discard or commit unrelated user work
merely to obtain a clean tree without explicit authority.

------------------------------------------------------------------------

# 19. Candidate Submission for Architectural Review

Repository-agent output should eventually be structured.

Candidate model:

``` text
ArchitecturalReviewSubmission
{
    SubmissionId
    AuthorizationId

    StartingRepositoryHash
    ResultingRepositoryHashes[]

    FilesChanged[]
    FilesAdded[]
    FilesDeleted[]

    ArchitecturalSurfacesAffected[]
    EdfArtifactsAffected[]

    ValidationRuns[]
    ManualValidationRequired[]
    ManualValidationEvidence[]

    KnownBaselineFailures[]
    NewFailures[]

    ScopeDeviations[]
    UnexpectedDependencies[]
    Questions[]

    WorkingTreeState

    RequestedDisposition
}
```

Candidate requested dispositions:

``` text
REVIEW
ACCEPT
REMEDIATE
CLOSEOUT
```

ProjectConcord should not require these exact names if EDF already
provides better terminology.

------------------------------------------------------------------------

# 20. Evidence

Evidence should answer:

-   what was changed;
-   why;
-   under which authorization;
-   from which baseline;
-   what files changed;
-   what tests ran;
-   what passed;
-   what failed;
-   what could not be tested;
-   what manual validation occurred;
-   whether scope changed;
-   resulting commit hashes;
-   working-tree state.

Evidence should be tied to the authorization and resulting repository
state.

A prose agent response alone should not be the long-term canonical
evidence model if structured evidence can be captured.

------------------------------------------------------------------------

# 21. Architectural Acceptance

Architectural acceptance is a separate decision.

The Architectural AI may recommend acceptance, but ProjectConcord should
preserve the actual authority model defined by the project.

Acceptance should be traceable to:

-   authorization;
-   implementation submission;
-   evidence;
-   relevant commits;
-   required validation;
-   accepted amendments;
-   unresolved defects/waivers;
-   EDF closeout.

Acceptance of one tranche does not authorize the next.

------------------------------------------------------------------------

# 22. Gate / Stage / Tranche Progression

ProjectConcord should explicitly model progression.

Example:

``` text
T0 CLOSED / ACCEPTED
        |
        | no automatic transition
        v
T1 NOT AUTHORIZED

Human/authorized authority issues:
        |
        v
T1 PLANNING AUTHORIZATION
        |
        v
T1 PLAN REVIEW
        |
        v
T1 IMPLEMENTATION AUTHORIZATION
```

This prevents an AI agent from interpreting "T0 accepted" as "continue
with T1."

------------------------------------------------------------------------

# 23. Conversation Rollover

Long AI conversations eventually become undesirable or exceed practical
context.

ProjectConcord should support explicit **conversation rollover**.

A rollover package should contain:

-   canonical project identifiers;
-   current stage/gate/tranche;
-   accepted decisions;
-   relevant hashes;
-   normative documents;
-   unresolved defects;
-   current authorization state;
-   repository state;
-   current stop point;
-   immediate next expected action.

The new conversation should not require the full historical transcript
if ProjectConcord can reconstruct the required context from canonical
state.

This is a significant reason to avoid making chat history canonical.

------------------------------------------------------------------------

# 24. Context Packaging

ProjectConcord should generate context according to recipient.

## Architectural AI package

May emphasize:

-   accepted architecture;
-   unresolved architectural questions;
-   evidence;
-   implementation plan;
-   requested review.

## Repository-agent package

May emphasize:

-   repository baseline;
-   normative documents to inspect;
-   authorization;
-   permitted/prohibited actions;
-   required outputs;
-   stop conditions.

## Human package

May emphasize:

-   decision required;
-   impact;
-   alternatives;
-   evidence;
-   recommendation.

These are different representations of shared canonical state.

------------------------------------------------------------------------

# 25. Provider Adapter Architecture

A provider adapter should translate between the canonical protocol and
provider capabilities.

Conceptually:

``` text
Canonical Handover
Canonical Authorization
        |
        v
Provider Adapter
        |
        +-- Cursor
        +-- GitHub Copilot
        +-- Future AI IDE
        +-- Manual Clipboard
```

The adapter may know:

-   provider prompt conventions;
-   planning vs agent modes;
-   repository-context capabilities;
-   tool limitations;
-   response parsing;
-   API/session identifiers.

It must not own:

-   project architecture;
-   gate state;
-   acceptance state;
-   canonical work authorization;
-   canonical evidence.

------------------------------------------------------------------------

# 26. Manual Clipboard Adapter

Manual copy/paste should be treated as a legitimate adapter, not an
embarrassing temporary workaround.

ProjectConcord can improve it through:

-   "Copy Handover + Authorization";
-   provider-targeted formatting;
-   clear message boundaries;
-   import/paste response;
-   response classification;
-   hash extraction;
-   test-result extraction;
-   diff/commit verification;
-   explicit human confirmation.

Manual mode provides maximum compatibility and a useful fallback when
APIs change.

------------------------------------------------------------------------

# 27. Direct Integration

Where supported, direct integration could allow ProjectConcord to:

1.  create a provider session;
2.  transmit the generated handover;
3.  transmit authorization;
4.  receive plan/submission;
5.  attach response to authorization;
6.  ask Architectural AI for review;
7.  present decision to human;
8.  transmit approved amendments;
9.  later transmit implementation authorization.

Direct integration must not automatically skip human authorization
points merely because transport is automated.

------------------------------------------------------------------------

# 28. Security and Trust Boundary

Direct repository-agent integration introduces important security
requirements.

ProjectConcord should consider:

-   repository permissions;
-   read-only vs write credentials;
-   commit permissions;
-   push permissions;
-   branch restrictions;
-   secrets handling;
-   provider data exposure;
-   audit logging;
-   authorization replay;
-   stale authorization;
-   provider identity;
-   human approval requirements.

A planning authorization should ideally be enforceable with read-only
capabilities where technically possible.

------------------------------------------------------------------------

# 29. Authorization Freshness and Repository Drift

An authorization is issued against a baseline.

If repository state changes materially before execution:

``` text
Authorized Baseline = A

Current Repository = B
```

ProjectConcord should determine whether the authorization remains valid.

Possible outcomes:

-   no relevant drift -\> continue;
-   benign unrelated drift -\> require acknowledgement/rebase of
    authorization;
-   conflicting drift -\> invalidate/supersede authorization;
-   unknown -\> STOP.

This prevents an old implementation authorization from being executed
against a substantially different repository.

------------------------------------------------------------------------

# 30. Scope-Conformance Checking

ProjectConcord should eventually compare:

``` text
Authorized Scope
        vs
Actual Change Set
```

Examples:

-   Planning-only authorization produced a commit -\> violation.
-   Core/persistence tranche touched Desktop UI -\> review required.
-   Documentation-only tranche modified production code -\> violation.
-   Authorized files changed plus unrelated files -\> scope deviation.
-   Later-stage capability implemented early -\> architectural review.

This can combine deterministic checks with AI review.

------------------------------------------------------------------------

# 31. Relationship to Git

Git answers:

> What changed in the repository?

Git does not by itself answer:

-   Was it authorized?
-   Why was it changed?
-   Which architecture required it?
-   Which gate does it satisfy?
-   Which evidence validates it?
-   Was it architecturally accepted?

ProjectConcord should connect Git commits to canonical governance
records rather than treating commit history as the entire project
history.

------------------------------------------------------------------------

# 32. Relationship to EDF

EDF remains the canonical engineering-documentation/governance
framework.

ProjectConcord should use EDF to answer questions such as:

-   What architecture is accepted?
-   What decisions are binding?
-   What work is planned?
-   What gate is open?
-   What evidence is required?
-   What remains unresolved?
-   What watch items exist?

This workflow should be reconciled with EDF rather than creating a
parallel governance system.

Where new concepts are needed---such as WorkAuthorization or
ArchitecturalReviewSubmission---the ProjectConcord team should determine
whether they belong:

-   in EDF itself;
-   as ProjectConcord application concepts derived from EDF;
-   or as a combination of both.

------------------------------------------------------------------------

# 33. Relationship to CRA-Style Canonical Representation Principles

The workflow strongly benefits from a canonical-representation approach:

``` text
Canonical Knowledge
        |
        +-- derived Markdown
        +-- derived UI
        +-- derived AI prompts
        +-- derived provider requests
        +-- derived reports
```

ProjectConcord should avoid making duplicated prose authoritative in
multiple places.

For example, a stage status should ideally have one canonical
identity/state and be rendered into:

-   roadmap;
-   dashboard;
-   AI handover;
-   gate report;
-   closeout report.

The same principle applies to authorization and evidence.

------------------------------------------------------------------------

# 34. Architectural Audit Records

ProjectConcord's existing Architectural Audit Record direction should be
evaluated against this workflow.

Potential audit questions include:

-   Did implementation conform to accepted ADRs?
-   Did the agent cross an authorization boundary?
-   Did the implementation introduce a competing architectural
    authority?
-   Did persistence remain compatible with accepted identity rules?
-   Were required architectural invariants preserved?
-   Was evidence sufficient for acceptance?

AARs may therefore become one of the structured review mechanisms in the
Architect--Repository Agent loop.

This document does not prescribe the exact AAR integration;
repository-informed architectural planning should decide it.

------------------------------------------------------------------------

# 35. Example: Manual Planning Cycle

``` text
1. Human identifies next tranche.

2. ProjectConcord gathers:
   - accepted baseline;
   - normative documents;
   - current gate state;
   - known defects;
   - repository state.

3. Architectural AI generates:
   - handover;
   - PLANNING authorization.

4. ProjectConcord renders:
   "Cursor Plan Handover"

5. Human copies it to Cursor.

6. Cursor inspects repository read-only.

7. Cursor returns implementation plan.

8. Human pastes plan into ProjectConcord.

9. ProjectConcord associates response with authorization.

10. Architectural AI reviews plan.

11. Human accepts/rejects/amends.

12. ProjectConcord records disposition.

13. If accepted, ProjectConcord prepares separate
    IMPLEMENTATION authorization.
```

------------------------------------------------------------------------

# 36. Example: Integrated Planning Cycle

``` text
1. Human authorizes planning.

2. ProjectConcord creates WorkAuthorization.

3. ProjectConcord generates repository-agent context.

4. Cursor/Copilot adapter sends request.

5. Agent inspects repository.

6. Agent returns structured plan.

7. ProjectConcord validates:
   - no prohibited mutations;
   - baseline still valid;
   - required outputs present.

8. Architectural AI reviews.

9. Human sees:
   - plan;
   - AI architectural review;
   - questions/conflicts.

10. Human selects disposition.

11. ProjectConcord records decision.
```

The architectural semantics are the same as manual mode.

------------------------------------------------------------------------

# 37. Example: Interactive Validation Stop

``` text
Implementation complete
        |
        v
Build PASS
Unit Tests PASS
        |
        v
Agent cannot operate Desktop UI
        |
        v
STOP:
OPERATOR_VALIDATION_REQUIRED
        |
        v
Human performs checks
      /     \
   PASS     FAIL
    |        |
    v        v
 Resume    Remediate
```

The agent must not invent manual PASS evidence.

------------------------------------------------------------------------

# 38. Example: Unrelated WIP

``` text
New governed work requested
        |
        v
git status shows unrelated modifications
        |
        v
Repository agent performs READ-ONLY inspection
        |
        v
Reports:
- purpose
- completeness
- validation
- relationship to current stage
        |
        v
Human/Architect disposition
   /        |        \
commit    stash     discard
separate  authorized authorized
        |
        v
Clean governed baseline
```

This pattern should be supported explicitly because it protects user
work and evidence integrity.

------------------------------------------------------------------------

# 39. ProjectConcord UI Implications

Potential UI surfaces include:

## Workspace / Projects

-   register/open multiple governed projects;
-   show selected project, repository, branch/worktree context;
-   preserve project-specific governance isolation;
-   show cross-project dependencies;
-   prevent accidental mutation of the wrong repository.

## Inter-Project

-   incoming/outgoing handovers;
-   awaiting target review;
-   target branch/PR status where applicable;
-   blocking/nonblocking dependencies;
-   accepted target commits/versions;
-   source work awaiting target disposition.

## Current Governance

-   current gate/stage/tranche;
-   status;
-   accepted baseline;
-   open defects;
-   waivers;
-   next permitted action.

## Work Authorization

-   authorization type;
-   scope;
-   allowed operations;
-   forbidden operations;
-   baseline;
-   expected evidence;
-   issue/revoke/supersede.

## AI Handover

-   target provider;
-   generated handover;
-   generated authorization;
-   copy/send action.

## Submission Review

-   returned plan/report;
-   changed files;
-   commits;
-   tests;
-   deviations;
-   unresolved questions;
-   Architectural AI review;
-   human disposition.

## Validation

-   automated checks;
-   manual checks;
-   evidence;
-   operator attestations.

## Human Inbox / Triage

-   capture idea, bug, feature, research request, documentation need, or
    architectural concern;
-   identify target project/framework;
-   relate item to active work if applicable;
-   flag out-of-band repository changes;
-   request Architectural AI impact assessment;
-   route to backlog, watch item, new workflow, amendment, or another
    project;
-   preserve origin/provenance.

## Audit Timeline

``` text
Architecture Accepted
        ↓
Planning Authorized
        ↓
Plan Submitted
        ↓
Plan Accepted
        ↓
Implementation Authorized
        ↓
Implementation Submitted
        ↓
Manual QA Required
        ↓
Operator PASS
        ↓
Closeout
        ↓
Architectural Acceptance
```

------------------------------------------------------------------------

# 40. AI Conversation as Workspace, Not Canonical Record

AI chat remains useful for:

-   reasoning;
-   exploration;
-   drafting;
-   clarification;
-   review.

However, important outcomes should be promoted into canonical project
state.

Examples:

``` text
Chat reasoning
     |
     v
Architectural Decision
     |
     v
ADR / Specification / Authorization / Evidence
```

This reduces dependence on enormous historical chats and improves
reproducibility.

------------------------------------------------------------------------

# 41. Recovery After Lost AI Context

ProjectConcord should be capable of recreating enough context to
continue work even if:

-   a chat is closed;
-   a provider loses conversation history;
-   a different provider is selected;
-   a new developer takes over;
-   the AI model changes.

The recovery package should come from canonical state.

This is one of the strongest practical arguments for ProjectConcord
owning structured handover generation.

------------------------------------------------------------------------

# 42. Provider Replacement

Because providers are adapters, this should be possible:

``` text
Today:
Architectural AI = GPT
Repository Agent = Cursor

Future:
Architectural AI = Provider X
Repository Agent = Copilot

Later:
Architectural AI = GPT
Repository Agent = ProjectConcord Native Agent
```

Changing provider should not change project governance semantics.

------------------------------------------------------------------------

# 43. Multi-User Future

ProjectConcord is expected to evolve beyond a single-user desktop
workflow.

The architecture should therefore allow authorization authority to map
to roles such as:

-   Project Architect;
-   Development Manager;
-   Developer;
-   QA;
-   Documentation Technician;
-   Project Manager;
-   Stakeholder.

Examples:

-   Architect accepts ADR;
-   Development Manager authorizes implementation;
-   Developer/agent executes;
-   QA attests manual validation;
-   Architect accepts gate.

The initial single-user deployment may collapse these roles into one
Administrator, but the canonical model should avoid assuming that one
person always performs every action.

------------------------------------------------------------------------

# 44. Auditability

For significant governed actions, ProjectConcord should be able to
answer:

-   Who authorized this?
-   What was authorized?
-   Which baseline did it target?
-   Which agent/provider executed it?
-   What changed?
-   Which tests ran?
-   Which manual checks were required?
-   Who attested them?
-   What deviations occurred?
-   Who accepted the result?
-   Which EDF artifacts and commits prove it?

This audit trail is valuable even when all roles are initially performed
by one person.

------------------------------------------------------------------------

# 45. Non-Goals

This proposal does not require ProjectConcord to:

-   replace Git;
-   replace Cursor;
-   replace GitHub Copilot;
-   become an IDE;
-   make AI autonomous project authority;
-   automatically approve architecture;
-   eliminate human review;
-   store every AI token forever;
-   force one AI provider;
-   make chat transcripts canonical.

------------------------------------------------------------------------

# 46. Initial Implementation Strategy

A staged ProjectConcord implementation could be considered.

## Phase A --- Manual Governance Assistance

Implement:

-   canonical/current project state;
-   authorization records;
-   handover generation;
-   copy-ready provider messages;
-   paste/import of responses;
-   architectural review workflow;
-   manual evidence recording.

This reproduces the proven Snaptara workflow with less manual
bookkeeping.

## Phase B --- Structured Submission and Git Correlation

Add:

-   commit/hash correlation;
-   changed-file analysis;
-   test-result records;
-   scope-conformance checks;
-   baseline-drift detection;
-   operator-validation records.

## Future Multi-Project / Inter-Project Capability

Preserve architectural support for multiple projects in one workspace,
governed inter-project handovers, cross-project dependency tracking, and
target-project Git branch/PR contribution workflows. Repository-informed
planning should assign the implementation milestone; this document does
not authorize pulling these capabilities into the current MVP.

## Phase C --- Provider Integration

Add adapters for supported environments.

Preserve the same canonical workflow.

## Phase D --- Advanced Automation

Potentially add:

-   automated context assembly;
-   automatic plan ingestion;
-   authorization enforcement;
-   policy-based validation;
-   AAR generation;
-   architectural drift detection;
-   cross-document reconciliation.

This sequence is illustrative; ProjectConcord planning must reconcile it
with the existing roadmap.

------------------------------------------------------------------------

# 47. Key Architectural Requirements

The following should be treated as candidate requirements for
formalization:

**PC-AIGOV-001 --- Human authority**\
AI recommendations and execution shall not implicitly become project
authorization or acceptance.

**PC-AIGOV-002 --- Canonical governance state**\
ProjectConcord shall represent governance state independently of AI chat
transcripts and provider prompts.

**PC-AIGOV-003 --- Handover/authorization separation**\
Inherited project context and permitted work shall be modeled as
distinct concepts.

**PC-AIGOV-004 --- Planning/implementation separation**\
Planning authorization shall not imply implementation authorization.

**PC-AIGOV-005 --- Provider neutrality**\
Provider integrations shall adapt to a provider-neutral governance
protocol.

**PC-AIGOV-006 --- Manual mode parity**\
Manual copy/paste workflow shall preserve the same governance semantics
as direct integration.

**PC-AIGOV-007 --- Explicit stop**\
Repository agents shall be able to stop and request disposition without
continuing unauthorized mutation.

**PC-AIGOV-008 --- Evidence linkage**\
Implementation evidence shall be traceable to authorization and
repository state.

**PC-AIGOV-009 --- Baseline awareness**\
Authorizations shall be associated with a repository/project baseline
sufficient to detect material drift.

**PC-AIGOV-010 --- Scope conformance**\
ProjectConcord should be capable of identifying actual work outside
authorized scope.

**PC-AIGOV-011 --- Validation provenance**\
Automated, agent-interactive, and human/operator validation shall be
distinguishable.

**PC-AIGOV-012 --- No fabricated validation**\
An AI agent shall not record validation as passed when it lacked the
capability to perform that validation.

**PC-AIGOV-013 --- Unrelated WIP protection**\
Unrelated repository work shall not be silently committed, discarded, or
absorbed into governed work.

**PC-AIGOV-014 --- Explicit progression**\
Acceptance of one stage/tranche shall not implicitly authorize the next.

**PC-AIGOV-015 --- Conversation rollover**\
ProjectConcord shall support reconstruction of sufficient AI context
from canonical state without requiring complete historical transcripts.

**PC-AIGOV-016 --- Auditability**\
Governed work shall retain sufficient provenance to reconstruct
authorization, execution, validation, and acceptance.

------------------------------------------------------------------------

**PC-AIGOV-017 --- Human intervention channel**\
ProjectConcord shall allow the Human Project Authority to capture work,
observations, ideas, defects, and concerns outside the active governed
workflow without silently modifying that workflow.

**PC-AIGOV-018 --- Intervention triage**\
Human-initiated work should be classifiable by relationship to active
work and routed to the current project, future work, another project,
EDF, ProjectConcord, or cross-project review.

**PC-AIGOV-019 --- Out-of-band reconciliation**\
When repository or canonical project state changes outside an active
WorkAuthorization, ProjectConcord shall surface the divergence and
require reconciliation before affected governed work proceeds.

**PC-AIGOV-020 --- Agent deviation reporting**\
Repository-agent changes outside authorized scope shall be preserved as
explicit deviations for Architectural AI and human review rather than
silently incorporated into the authorization.

**PC-AIGOV-021 --- Cross-project provenance**\
Work captured in one project context but routed to another project or
shared framework shall preserve its origin without becoming part of the
originating project's active gate.

**PC-AIGOV-022 --- Multi-project workspace**\
ProjectConcord shall be architected to manage multiple independently
governed projects within a single workspace.

**PC-AIGOV-023 --- Project governance isolation**\
Each managed project shall retain its own canonical EDF state,
repository state, authorization state, governance lifecycle, and
applicable authority boundaries.

**PC-AIGOV-024 --- Governed inter-project handover**\
A project shall be able to originate a governed proposal, discovery,
defect, or work item targeted at another managed project while
preserving source provenance.

**PC-AIGOV-025 --- Destination governance authority**\
An inter-project handover shall not bypass the destination project's
governance. Target disposition and acceptance shall occur under the
destination project's applicable review and authorization process.

**PC-AIGOV-026 --- Git contribution workflow**\
Where managed projects use Git hosting, ProjectConcord architecture
shall permit inter-project proposals to be materialized through target
branches, commits, and pull requests without treating those Git
mechanisms themselves as architectural acceptance.

**PC-AIGOV-027 --- Cross-project dependency distinction**\
ProjectConcord shall distinguish an inter-project handover event from an
ongoing dependency between projects, capabilities, artifacts, commits,
or versions.

**PC-AIGOV-028 --- Inter-project traceability**\
ProjectConcord shall preserve traceability from originating discovery
through target proposal, target artifacts, review/contribution workflow,
accepted target commit/version, and originating work dependent upon the
result.

# 48. Open Architectural Questions for ProjectConcord

Repository-informed planning should answer:

1.  Which of these concepts already exist in ProjectConcord
    architecture?
2.  Should `WorkAuthorization` be an EDF artifact, a ProjectConcord
    canonical record, or both?
3.  Should `ArchitecturalReviewSubmission` become a formal artifact?
4.  How should authorizations relate to EDF gates/stages/tranches?
5.  How should authorizations relate to Git branches/commits?
6.  How should AARs participate in plan and implementation review?
7.  Which state belongs in Markdown versus structured storage?
8.  What is the canonical identity strategy for authorizations and
    submissions?
9.  How should manual operator attestations be represented?
10. How should authorization revocation/supersession work?
11. How should repository drift invalidate or amend an authorization?
12. What minimum provider-adapter interface is required?
13. Can Cursor or Copilot integrations enforce read-only planning
    technically, or must ProjectConcord detect violations afterward?
14. What security model is required for direct repository integration?
15. How should AI context packages reference canonical EDF artifacts
    without duplicating them?
16. How should ProjectConcord handle multiple concurrent authorized work
    streams in the future?
17. How should multi-user roles map to issue/review/accept permissions?
18. Which portions should be proposed upstream as EDF enhancements
    rather than remain ProjectConcord-specific?

------------------------------------------------------------------------

24. What is the canonical workspace/project identity model for multiple
    repositories under one ProjectConcord UI?
25. Should `InterProjectHandover` be operational, an EDF artifact, or
    operational state capable of materializing target EDF artifacts?
26. How should target-project permissions be verified before creating
    branches, commits, or pull requests?
27. How should cross-project dependencies reference target commits,
    releases, specifications, or capabilities?
28. When a target project accepts a handover, how should dependent
    source work be notified and re-evaluated?
29. How should handovers work when the destination is unloaded,
    read-only, or controlled by another organization?
30. Which inter-project concepts belong in ProjectConcord versus EDF
    itself?

# 49. Recommended ProjectConcord Planning Deliverable

After inspecting the existing repository and EDF state, the
ProjectConcord repository agent should prepare a plan that:

1.  maps this proposal to existing architecture;
2.  identifies overlap and conflicts;
3.  identifies required ADRs/specifications/amendments;
4.  proposes canonical domain entities and relationships;
5.  proposes workflow state machines;
6.  proposes manual-mode UX;
7.  proposes provider-adapter boundaries;
8.  proposes Git/evidence integration;
9.  proposes security/audit boundaries;
10. proposes staged implementation;
11. identifies EDF changes, if any;
12. identifies questions requiring Project Architect disposition.
13. evaluates whether the current application/project model can support
    multiple projects in one workspace without redesign;
14. proposes the boundary between `HumanInitiatedWorkItem`,
    `InterProjectHandover`, and cross-project dependency records;
15. identifies future Git branch/PR integration boundaries for
    target-project contributions.

No production implementation should begin solely from this handover.

------------------------------------------------------------------------

# 50. Architectural Summary

The target is not:

``` text
GPT <---- copy/paste ----> Cursor
```

The target is:

``` text
                         HUMAN AUTHORITY
                               |
                               v
                        PROJECTCONCORD
                 Canonical EDF/Governance State
                  /             |             \
                 /              |              \
                v               v               v
       Architectural AI   Git / Evidence   Repository Agent
             GPT                           Cursor / Copilot
                \                               /
                 \                             /
                  +---- governed protocol ----+
```

The manual workflow and integrated workflow are two transports over the
same architectural process.

ProjectConcord should preserve the essential discipline demonstrated in
actual development:

-   inspect before planning;
-   plan before implementation;
-   authorize explicitly;
-   keep scope bounded;
-   distinguish canonical state from derived representations;
-   preserve repository baselines;
-   protect unrelated work;
-   distinguish baseline failures from regressions;
-   never fabricate validation;
-   collect evidence;
-   review architecture after execution;
-   close work explicitly;
-   never infer authorization for the next stage.

That process is the capability this document proposes ProjectConcord
formalize.

------------------------------------------------------------------------

# 51. Handover Stop Point

This document is ready for handover to the ProjectConcord repository.

The next action should be:

**ProjectConcord repository inspection and planning only.**

The repository agent should determine how this proposal fits the current
accepted ProjectConcord/EDF architecture and return a formal plan for
Project Architect review.

**Production implementation is not authorized by this document.**
