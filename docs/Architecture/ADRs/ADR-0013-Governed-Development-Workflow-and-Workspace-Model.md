# ADR-0013: Governed Development Workflow and Workspace Model

## Status

Proposed

## Date

2026-09-21

## Context

[PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md) generalizes a Snaptara-style workflow: canonical governance state is primary; AI chats and provider prompts are derived transport. The proposal adds **multi-project workspace** coordination, **InterProjectHandover**, **CrossProjectDependency**, and **DevelopmentWorkAuthorization** distinct from EDF artifact lifecycle rules in [SPEC-003](../../Specifications/features/SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md).

[ADR-0006](ADR-0006-AI-Boundary.md) already limits AI to proposals for canonical EDF writes. This ADR addresses **governed repository execution** and **workspace boundaries** without authorizing implementation.

## Decision

1. **Canonical vs operational (development governance)**  
   - **EDF artifacts** in Git remain canonical engineering intent per [ADR-0002](ADR-0002-EDF-Canonical-Source-of-Truth.md).  
   - **DevelopmentWorkAuthorization**, **ArchitecturalReviewSubmission**, **HumanInitiatedWorkItem**, **InterProjectHandover**, and **CrossProjectDependency** default to **operational** persistence per [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), scoped per managed project or workspace as implemented later.  
   - **Handover** and provider message packages are **derived representations** of canonical state ([ADR-0004](ADR-0004-Derived-Data-and-Cache.md) principle); they are not authoritative transcripts.

2. **Handover vs authorization**  
   Handover (inherited context) and DevelopmentWorkAuthorization (permitted work) are **distinct concepts** even when rendered in one UI message (PC-AIGOV-003).

3. **Planning vs implementation**  
   Planning authorization does not imply implementation authorization (PC-AIGOV-004).

4. **DevelopmentWorkAuthorization naming**  
   Use **DevelopmentWorkAuthorization** in normative docs to avoid collision with SPEC-003 **authorized state transitions** on artifacts.

5. **HumanInitiatedWorkItem vs AWI**  
   **HumanInitiatedWorkItem** is an operational capture/triage path, **distinct** from Architectural Watch Items (AWI) in EDF. A HIW may promote to an AWI or escalate to **InterProjectHandover**.

6. **Inter-project governance**  
   - **InterProjectHandover** is operational by default; it preserves source provenance; **destination project governance** controls disposition and acceptance (PC-AIGOV-025).  
   - **CrossProjectDependency** is operational by default unless EDF later defines a canonical cross-project dependency capability.  
   - **Git branches, commits, and pull requests** on the target project are **contribution and evidence mechanisms**, not architectural acceptance (PC-AIGOV-026).

7. **Multi-project workspace**  
   ProjectConcord is architected toward a **workspace** of multiple **independently governed** projects (PC-AIGOV-022–023). Each project retains its own EDF/Git state, authorizations, gates, and evidence partitions.

8. **M1–M5 non-lock-in constraint (architectural)**  
   M1–M5 **MUST NOT** establish an implicit or irreversible assumption that one application runtime corresponds to exactly one project/repository. Foundational design must preserve a practical migration path to multiple governed projects in one workspace.  
   **This ADR does not mandate** specific implementation mechanisms (`ProjectId`, UUID registries, `IProjectScope`, repository factories, `workspaceId` schema columns, dedicated assemblies, or service-lifetime patterns). Those are **candidate approaches** for future specs or ADR amendments if repository work demonstrates necessity.

9. **AAR vs submission review**  
   [ADR-0012](ADR-0012-Adopt-EDF-Architectural-Audit-Records.md) AARs remain appropriate for **significant** architectural/milestone/gate conformance reviews. Routine implementation tranche review may use **ArchitecturalReviewSubmission** without requiring an AAR each time.

10. **EDF upstream**  
    **Do not** propose PC-AIGOV-022–028 to EDF in this tranche. Record EDF candidates in [EDF Gap Register](../../Development/EDF_Gap_Register.md) GAP-035 for later review.

11. **Provider neutrality**  
    Manual clipboard and integrated providers are adapters to the same governance semantics ([SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)).

## Alternatives Considered

### Store DevelopmentWorkAuthorization as canonical EDF artifacts

- Advantages: Git-auditable authorization text.
- Disadvantages: High churn; conflates execution workflow with engineering intent documents; awkward for inter-project operational state.
- Reason not selected: Operational default; export to EDF evidence optional later.

### Defer multi-project workspace to web-only client

- Advantages: Simpler desktop MVP.
- Disadvantages: Risk single-project lock-in in domain layer; contradicts PCON-0001 §4F direction.
- Reason not selected: Non-lock-in constraint applies at M1–M5 architecture level without implementing multi-project UI.

### Merge HIW with AWI

- Advantages: Fewer entity types.
- Disadvantages: Contaminates EDF watch-item semantics with inbox/triage noise.
- Reason not selected: Architect disposition — remain distinct.

## Consequences

### Positive

- Clear boundary between EDF integrity (SPEC-003) and governed development execution (SPEC-004).
- Traceability model for cross-project work without merging gate state across repos.

### Negative

- Additional operational schema design (extends GAP-018 family).
- Terminology discipline required (persona workspace vs governance workspace).

### Risks

- Premature implementation of inter-project or PR automation before M7 — mitigated by roadmap phasing and SPEC-004 “not implemented” status.

## References

- [PCON-0001](../PCON-0001-AI-Assisted-Architectural-Governance-and-Repository-Execution-Workflow.md)
- [AI Governance Workflow Integration Analysis](../AI_Governance_Workflow_Integration_Analysis.md)
- [SPEC-004](../../Specifications/features/SPEC-004-ai-assisted-development-governance-workflow.md)
- [ADR-0006](ADR-0006-AI-Boundary.md), [ADR-0009](ADR-0009-Multi-User-Platform-and-Shared-Project-Services.md), [ADR-0012](ADR-0012-Adopt-EDF-Architectural-Audit-Records.md)
- [Implementation Roadmap](../../Development/Implementation_Roadmap.md)
