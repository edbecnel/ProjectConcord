[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › M7a-WF-1d

# ProjectConcord — M7a-WF-1d Implementation Notes

## Governance

| Item | Value |
|---|---|
| **Authorization** | Handover `f4ef54a8-638c-4d30-aac6-b24a2c605430` |
| **Plan** | Corrected plan `dcf55d72-82ad-4ab2-a95d-9c60817a68fd` |
| **Baseline** | `ec850fa03c188d987de584948adb03da829cb052` |
| **Persistence** | None (derived projections only) |
| **Manual QA** | [MVR-0004](../Verification/Records/MVR-0004-m7a-wf-1d-governed-eligibility-recovery-projections.md) — **not executed** in implementation tranche |

## Delivered

| Area | Location |
|---|---|
| Eligibility evaluator | `src/Edf.Application/Workflow/Eligibility/` |
| DWA place/occurrence applicability | `DevelopmentWorkAuthorizationApplicability` |
| Operator projections | `GovernedWorkStateOperatorProjectionService`, `WorkflowNextActionContributor` |
| Tests | `tests/Edf.Application.Tests/Workflow/M7aWf1dEligibilityTests.cs`, `DevelopmentWorkAuthorizationApplicabilityTests.cs` |

## Semantic contract (WF-1d)

- **`EligibleUnderEvaluatedConstraints`** — evaluated blockers clear; **not** full permission.
- **`CandidateFrontierInstanceIds`** — Active instances with `EligibleUnderEvaluatedConstraints == true`.
- **`FullyGovernedActionability`** — always **`Indeterminate`** in WF-1d (`gew.actionability.fully-governed.indeterminate-wf1d-scope`).
- **`UnevaluatedApplicableDimensions`** — empty when GEW v1 registry has no sync/AEI/evidence/Control attachments (deferred dimensions are **not** falsely marked applicable per instance).
- **Workflow Next Action** — `WorkflowOperatorNextActionItem` with `PermissionToExecute` (`Indeterminate` / `Denied` / `NotApplicable`); relay P0 Next Action unchanged.

## Explicitly not delivered

PC-PAR-025; relay binding; sync/AEI persistence; CPA/CCA; GIES; PA Exchange; topology mutation from projections; Desktop UI.

## Parent

- [AWI-0010](../Architecture/Watch_Items/AWI-0010-Workflow-Framework-and-GEW-Architectural-Follow-Through.md)
