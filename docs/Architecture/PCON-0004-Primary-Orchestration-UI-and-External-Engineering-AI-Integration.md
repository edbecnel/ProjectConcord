[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Architecture](README.md) › PCON-0004

# PCON-0004: Primary Orchestration UI and External Engineering/AI Integration

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architectural Discovery Record |
| **Normative** | No |
| **Status** | Proposed — post-closeout architectural input; queued for future Project Architect review |
| **Record ID** | PCON-0004 |
| **Date** | 2026-09-22 |
| **Owner** | ProjectConcord |
| **Authoritative** | No — candidate models and requirements only |
| **Distinct from** | [PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) / [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md); [PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md) / [AWI-0002](Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md) — **do not merge** |
| **Watch item** | [AWI-0003](Watch_Items/AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md) |
| **Related discovery** | [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) (Proposed) |
| **Related decisions / specs** | [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (Proposed); [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (Draft / not implemented) |

---

## Handover disposition

This record captures **architectural discovery only**. It is **not** pre-authorized implementation work, **not** an architecture plan, and **not** an amendment to the closed AI-governance documentation tranche.

**Do not:** change ADR-0013 or SPEC-004 status; add PC-AIGOV-060–071 to SPEC-004 as normative requirements; select ACP, MCP, CLI, extension APIs, OpenAI API, or any other integration transport as normative architecture; implement or prototype integrations; install or configure ACP/MCP; create a Cursor extension; begin M1, M7, or other implementation milestones.

Candidate requirements PC-AIGOV-060–071 are recorded in §16 and [EDF Gap Register](../Development/EDF_Gap_Register.md) GAP-039 for future disposition.

---

## 1. Purpose

ProjectConcord is intended eventually to become the user's **primary project-governance and orchestration user interface**, not merely a documentation viewer or a mechanism for manually generating handovers between independent AI tools.

Manual copy/paste handovers to and from ChatGPT, Cursor, GitHub Copilot, or other systems must remain supported because they provide:

- a bootstrap integration mechanism;
- a provider-independent fallback;
- a recovery mechanism when direct integration is unavailable;
- a transparent human-controlled workflow.

Manual handover is **not** the intended primary long-term operating model. The desired future experience is substantially more seamless: ProjectConcord orchestrates governed engineering work from its own UI while specialized engineering applications remain available for discipline-specific work they perform best.

---

## 2. Candidate architectural principle

ProjectConcord should be the **primary governance and orchestration environment**, while specialized engineering applications remain the **primary workbenches** for performing discipline-specific engineering activities.

For the initial Software Engineering profile, ProjectConcord should eventually coordinate activities involving Project Architect AI; software-development AI/agents; human Project Authority; QA; documentation; repository state; EDF artifacts; work authorization; architecture; requirements; planning; implementation; automated testing; evidence; review; acceptance; continuation/pause/resume; and inter-project dependencies and handovers.

**Cursor IDE** should remain a software-development workbench for source-code editing, detailed Markdown editing, debugging, test execution, repository inspection, Git operations, and interactive developer work. ProjectConcord should **not** attempt to replace Cursor as a source-code IDE. Instead, ProjectConcord should increasingly orchestrate what work Cursor or another engineering agent is authorized to perform and receive structured results and evidence back from that work.

This is a **candidate direction only**, not an accepted ADR.

---

## 3. Conceptual orchestration workflow (candidate)

```
User
  |
  v
ProjectConcord  ---- project / EDF state, architecture, requirements,
  |               gates / stages / tranches, work authorization,
  |               actor / role assignments, review / acceptance,
  |               QA / evidence, continuation state, orchestration
  |
  +--------------------+--------------------+
  v                    v
Project Architect    Engineering Agent
Provider             Provider
  |                    |
  v                    v
AI service           Cursor / Copilot / future agent
                       |
                       v
                 Engineering repository
                       |
                       | structured result + evidence
                       v
                 ProjectConcord
                       |
                       +--> QA
                       +--> Architectural Review
                       +--> Acceptance
                       +--> Next Authorization
```

The user should eventually be able to initiate an **authorized tranche** from ProjectConcord rather than manually reconstructing tranche context in Cursor. Exact workflow, UI, and contracts are **unresolved**.

---

## 4. Project Architect integration

The user currently uses ChatGPT as the Project Architect. This workflow must remain supported.

ProjectConcord should eventually support at least two **conceptual interaction modes**:

| Mode | Description |
|---|---|
| **Manual Project Architect interaction** | ProjectConcord generates a structured Project Architect context/handover package; the user transfers it to ChatGPT (or another tool); architectural work is performed; the response is brought back into ProjectConcord and reconciled with canonical project state. |
| **Direct Project Architect integration** | ProjectConcord communicates directly with an AI provider/API assigned to the Project Architect Role using the same or compatible governed context and response semantics as the manual mechanism. |

Manual versus direct AI integration should preferably be a **transport/provider distinction** rather than two different governance architectures. ChatGPT/OpenAI must **not** be hard-coded as the semantic definition of Project Architect.

Under the [PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) direction:

- **Role:** Project Architect
- **Actor:** ChatGPT/OpenAI-backed AI, or another assigned Actor
- **Integration:** Manual Conversation Adapter, or Direct Provider Adapter

Exact terminology remains subject to future architectural review.

---

## 5. Engineering agent and Cursor integration

Cursor is expected to remain a major Software Engineering workbench used alongside ProjectConcord. The desired architecture should investigate ProjectConcord **initiating and coordinating Cursor work directly** rather than relying primarily on manually pasted handovers.

Current Cursor capabilities that appear **potentially relevant** (candidates for investigation only) include: Cursor Agent/CLI integration; Agent Client Protocol (ACP); Model Context Protocol (MCP); Cursor extensions/plugins; programmatic interaction with Cursor agents; structured question/response and plan-approval interaction where supported.

**No protocol or product integration is selected in this capture.**

A promising **conceptual division** to investigate later (not an accepted protocol decision):

- ProjectConcord ---- orchestration/client channel ----> Cursor
- ProjectConcord <---- governed context/tool channel ---- Cursor

Potentially: ProjectConcord ---- ACP or equivalent ----> Cursor Agent; ProjectConcord <---- MCP or equivalent ----- Cursor.

Future investigation must verify current provider capabilities, stability, licensing, security, lifecycle, and suitability before an ADR selects an integration mechanism.

---

## 6. Illustrative governed operations (non-normative)

One candidate direction is for ProjectConcord eventually to expose governed operations/context to external engineering agents. **Illustrative examples only** — not an accepted API, MCP tool contract, or implementation specification:

- `get_current_work_authorization()`
- `get_governing_architecture()`
- `get_relevant_adrs()`
- `get_current_gate()` / `get_current_stage()` / `get_current_tranche()`
- `get_known_baseline_defects()`
- `get_required_validation()`
- `submit_evidence()`
- `report_scope_deviation()`
- `request_architectural_clarification()`
- `request_authorization_review()`

---

## 7. Cursor extension/plugin role (candidate)

A Cursor extension/plugin may still be useful even if another protocol provides the principal orchestration transport. Potential IDE-side functionality might include: displaying current ProjectConcord project context; active gate/stage/tranche; active work authorization; Actor/Role assignment; permitted and prohibited operations; link/open-current-item in ProjectConcord; submit evidence; report deviation; request clarification; synchronization status.

The extension should **not** automatically become the source of architectural or governance truth. ProjectConcord remains the candidate authoritative orchestration/governance environment.

---

## 8. GitHub Copilot and other engineering agents

ProjectConcord must not architect this workflow solely around Cursor. GitHub Copilot or other current/future engineering agents may be used instead of or alongside Cursor.

Investigate a **provider-neutral abstraction** such as (candidate names only):

```
EngineeringAgentAdapter
    +-- CursorAdapter
    +-- GitHubCopilotAdapter
    +-- FutureProviderAdapter

ProjectArchitectAdapter
    +-- ManualConversationAdapter
    +-- OpenAIAdapter
    +-- FutureProviderAdapter
```

The governance workflow must not depend upon the identity of a particular provider.

---

## 9. Semantic responsibilities versus transport

Future architecture should distinguish at least the following concerns (not necessarily separate processes or components):

| Concern | Responsibility |
|---|---|
| Project Architect Provider | Architectural reasoning/review |
| Engineering Agent Provider | Implementation/testing/documentation work |
| Engineering Tool Adapter | IDE/CAD/EDA/simulation/etc. |
| Artifact-System Adapter | Git/GitHub/PDM/PLM/etc. |
| Communication Transport | API/ACP/MCP/CLI/manual/etc. |

A transport protocol must **not** define governance semantics. Prior queued gaps [GAP-028](../Development/EDF_Gap_Register.md) (handover vs authorization) and [GAP-030](../Development/EDF_Gap_Register.md) (provider adapter interface) remain related but distinct from this discovery.

---

## 10. Relationship to Actor, Role, and RoleAssignment

This discovery depends strongly upon [PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md) / [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md). External AI systems and engineering tools should not themselves define governance roles.

Examples (not permanent assignments):

| Actor | Role (examples) |
|---|---|
| Cursor | Software Developer; Test Developer |
| ChatGPT/OpenAI-backed service | Project Architect |
| Human user | Project Authority; QA Tester |

An Actor's **technical ability** to perform an operation must remain distinguishable from **governance permission** to perform that operation (for example, Cursor may be capable of pushing a branch while the applicable Work Authorization prohibits push).

---

## 11. Human authority and interaction

Seamless orchestration must **not** mean autonomous removal of human authority. ProjectConcord should continue to preserve explicit human decision points where required by governance, including accepting/rejecting architectural decisions; approving Work Authorization; approving plans where required; accepting manual QA; granting exceptions/waivers; authorizing subsequent work; approving cross-project effects; and approving push/merge/release operations where policy requires.

The desired reduction is **manual transport and context reconstruction**, not human authority.

---

## 12. Structured interaction and evidence

Direct integrations should eventually favor **structured messages/contracts** rather than treating every interaction as unstructured chat. Existing candidate concepts such as DevelopmentWorkAuthorization; ArchitecturalReviewSubmission; Actor/Role/RoleAssignment; evidence; continuation records; and inter-project handovers should be considered when defining future provider contracts.

An engineering agent should eventually be able to return structured information identifying authorization under which it operated; Actor; Role; starting and resulting repository/artifact state; files/artifacts changed; tests/validation performed; evidence produced; baseline and new failures; deviations; unresolved questions; and requested disposition. Exact contracts remain **unresolved**.

---

## 13. Engineering-domain neutrality

This integration architecture must remain compatible with PCON-0002's engineering-domain-neutrality concern. Cursor and GitHub Copilot are particularly relevant to the initial Software Engineering domain. Other disciplines may use different specialized workbenches (for example KiCad, FreeCAD, or domain-specific tools).

ProjectConcord Core therefore should not equate Engineering Tool == IDE, Artifact System == Git, or Engineering Agent == Coding Agent. Software-specific integrations may belong partly or wholly to the Software Engineering Domain Profile. The exact Core/profile boundary is **unresolved**.

---

## 14. Manual handover remains mandatory

Even if seamless integration becomes the preferred operating mode, ProjectConcord should retain manual handover capability for unsupported providers; provider outages; debugging integration failures; bootstrap development; security-restricted environments; human inspection; portability; migration; and recovery.

Direct integration should **supplement** and become preferable to, rather than **eliminate**, the manual handover model.

---

## 15. Security and governance concerns (queued, unresolved)

Queue for future investigation without resolving here: API credentials and secrets; provider authentication; Actor identity; authorization freshness; stale sessions; repository/worktree selection; read versus write capability; source/test/documentation modification authority; commit/push/merge authority; branch restrictions; architecture modification; acceptance authority; gate/stage/tranche closure authority; next-work authorization; provider impersonation; auditability; provenance; replay/retry behavior; interrupted operations; provider failure; context leakage between projects; cross-project authority isolation; human override; reconciliation of out-of-band changes.

---

## 16. Candidate future requirements (non-normative)

| ID | Summary |
|---|---|
| PC-AIGOV-060 | Primary orchestration environment — ProjectConcord as primary governance/orchestration UI; specialized engineering applications remain discipline-specific workbenches |
| PC-AIGOV-061 | Manual integration fallback — structured manual handover/import for Project Architect and Engineering Agent interactions |
| PC-AIGOV-062 | Direct Project Architect integration — direct integration with providers capable of fulfilling an assigned Project Architect Role |
| PC-AIGOV-063 | Direct Engineering Agent integration — direct orchestration of compatible engineering agents under explicit RoleAssignments and Work Authorizations |
| PC-AIGOV-064 | Provider-neutral adapters — governance semantics independent of specific AI/agent/tool providers |
| PC-AIGOV-065 | Transport independence — API, ACP, MCP, CLI, extension APIs, and manual transport do not define governance semantics |
| PC-AIGOV-066 | Structured governed exchange — direct integrations support structured exchange of authorization, context, questions, results, evidence, deviations, and review submissions |
| PC-AIGOV-067 | Human authority preservation — direct orchestration preserves required human approval, intervention, exception, and acceptance authority |
| PC-AIGOV-068 | Capability enforcement — technical provider capabilities distinguishable from governance permissions for the applicable scope |
| PC-AIGOV-069 | Specialized workbench coexistence — ProjectConcord orchestrates rather than unnecessarily replacing discipline-specific engineering tools |
| PC-AIGOV-070 | Engineering-domain-neutral integration — Core integration abstractions do not assume all engineering work occurs in a source-code IDE or Git repository |
| PC-AIGOV-071 | Auditable external execution — externally executed governed work attributable to Actor, Role, authorization, provider/session where applicable, resulting artifact state, and evidence |

Indexed: [EDF Gap Register](../Development/EDF_Gap_Register.md) **GAP-039**. **Not** in SPEC-004 as normative requirements.

---

## 17. Candidate architectural principle (for future review)

ProjectConcord should orchestrate governed engineering activity through provider-neutral role, authorization, evidence, and lifecycle semantics. External AI providers, engineering agents, specialized engineering applications, artifact systems, and communication protocols should integrate through adapters or equivalent boundaries without becoming the source of ProjectConcord governance semantics.

This is **candidate architecture**, not an accepted ADR.

---

## 18. Future reconciliation

When architecture is next reviewed, reconcile this discovery with [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md), [PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md), [PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md), [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md) (remain Proposed until explicitly accepted), [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md) (remain Draft / not implemented), and [AI Governance Workflow Integration Analysis](AI_Governance_Workflow_Integration_Analysis.md).

**Resume ordering** among PCON-0002, PCON-0003, this discovery, ADR-0013, and SPEC-004 is **not** prescribed here; see [PCR-0001](../Development/PCR-0001-Project-Continuation-and-Pause-Record.md).

---

## Parent

- [Architecture](README.md)

## Related Documents

- [AWI-0003](Watch_Items/AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md)
- [PCR-0001](../Development/PCR-0001-Project-Continuation-and-Pause-Record.md)
- [PCON-0001](PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [PCON-0002](PCON-0002-Actor-Role-Model-Engineering-Domain-Neutrality-and-Governance-Abstraction.md)
- [PCON-0003](PCON-0003-Governed-Pause-Continuation-and-Resume.md)
- [AWI-0001](Watch_Items/AWI-0001-Actor-Role-Abstraction-and-Engineering-Domain-Profiles.md)
- [AWI-0002](Watch_Items/AWI-0002-Governed-Pause-Continuation-and-Resume.md)
- [ADR-0013](ADRs/ADR-0013-Governed-Development-Workflow-and-Workspace-Model.md)
- [SPEC-004](../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [AI Governance Workflow Integration Analysis](AI_Governance_Workflow_Integration_Analysis.md)
- [EDF Gap Register](../Development/EDF_Gap_Register.md) (GAP-039)
