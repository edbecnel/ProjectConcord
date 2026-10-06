# MVR-0004: M7a-WF-1d governed eligibility and recovery projections

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Verification](../../Verification/README.md) › [Records](README.md) › MVR-0004

## Identity

| Field | Value |
|---|---|
| **Record ID** | MVR-0004 |
| **Scope** | M7a-WF-1d — partial governed eligibility, `CandidateFrontier`, Waiting On, workflow Next Action |
| **Verification classification** | **Automated** (Release regression tests; no human-interactive MVT execution for current WF-1d scope) |
| **MVR status** | **Complete** (automated evidence recorded) |
| **Owner** | Project owner |
| **Verification date** | 2026-10-06 |

**Published repository baseline (parent of WF-1d implementation delta):** `b6207d8e7a5109b3b736e9e4d15ba30538246a02` (PCON-0008 publication).

**WF-1d candidate:** Local unpublished working contents (implementation + tests + this record). No publication commit SHA is asserted here.

**WF-1d implementation review:** PA **PASS** (implementation). **Publication / commit:** not authorized.

**Record correction (`ccdc7057`):** A prior update incorrectly marked human execution **Complete** and MVT **Pass** based on operator-invoked `dotnet test` alone. That human Pass authority for MVT-1–MVT-4 is **withdrawn**. Operator runs on 2026-10-06 are retained below as **historical automated execution log** (provenance only).

**Verification correction tranche:** Handover `d536bed6-cc5c-476f-88dc-a1dce84ecfc5` (follows plan `da7bb9e7-3e45-454e-a666-2799d66929f0`).

## Verification basis

| Requirement | Link |
|---|---|
| M7a-WF-1d implementation authorization | Handover `f4ef54a8` |
| Corrected plan | `dcf55d72` |
| Post-submission Next Action correction | Handover `25c92ca2` |
| MVT-11 procedure correction | Handover `bce078ef` |
| Implementation notes | [M7a-WF-1d Implementation Notes](../../Handover/ProjectConcord-M7a-WF-1d-Implementation-Notes.md) |

## Implementation scope

| Anchor | Value |
|---|---|
| Code paths | `src/Edf.Application/Workflow/Eligibility/`, `src/Edf.Application/Operator/WorkState/` |
| Out of scope | Desktop UI; relay schema; PC-PAR-025; GSR runtime |

## Human-interactive verification

**Not required** for current WF-1d scope.

All MVT criteria below are deterministic application behavior exercised through Release automated tests with explicit assertions. No desktop walkthrough, manual relay leg, or subjective UI judgment is required to establish Pass for this tranche.

## Automated verification matrix (authoritative MVT evidence)

| MVT | Criterion (summary) | Classification | Automated test method(s) | Result | Final validation execution |
|---|---|---|---|---|---|
| MVT-1 | No Active instances: empty `CurrentWork` and `CandidateFrontier`; workflow Next Action unavailable with canonical no-suggestions reason | Automated | `GovernedWorkStateOperatorProjectionTests.EmptyProject_ReturnsEmptyCurrentWork` | Pass | See [Final validation](#final-validation-2026-10-06) |
| MVT-2 | Single GEW at Intake: evaluated satisfaction without Planning/Implementation DWA; `FullyGovernedActionability` Indeterminate; Next Action does not grant permission | Automated bundle | `M7aWf1dEligibilityTests.Intake_DoesNotRequirePlanningOrImplementationDwa_ForEvaluatedSatisfaction`; `FullyGovernedActionability_IsNeverFullyActionable_InWf1d`; `NextAction_Suggestion_DoesNotGrantPermission` | Pass | Final validation |
| MVT-3 | Multiple Active instances: `CandidateFrontier` lists only eligible instances; deterministic ordering | Automated | `M7aWf1dEligibilityTests.CandidateFrontier_MixedCandidateAndBlocked`; `TwoEligibleInstances_CandidateFrontier_OrderedByInstanceIdThenPlace` | Pass | Final validation |
| MVT-4 | Pending dependency: blocked instance off frontier; Waiting On dependency with related instance ids | Automated | `GovernedWorkStateOperatorProjectionTests.DependencyBlocked_ExposesWaitingOn_ForBlockedInstance`; frontier exclusion in `CandidateFrontier_MixedCandidateAndBlocked` | Pass | Final validation |
| MVT-5 | Instance STOP: canonical Waiting On; clear restores eligibility and frontier membership | Automated | `M7aWf1dEligibilityTests.Stop_BlocksEvaluatedEligibility_AndClearRestoresFrontier` | Pass | Final validation |
| MVT-6 | Planning-governed with Planning DWA at occurrence: authorization evaluated satisfied | Automated (existing) | `M7aWf1dEligibilityTests.PlanningGoverned_WithPlanningDwa_SatisfiesEvaluatedAuthorization` | Pass | Final validation |
| MVT-7 | Implementation-governed with Implementation DWA: `EligibleUnderEvaluatedConstraints` | Automated | `M7aWf1dEligibilityTests.ImplementationGoverned_WithImplementationDwa_SatisfiesEvaluatedAuthorization` | Pass | Final validation |
| MVT-8 | Planning DWA supersede without re-grant blocks; new grant at same occurrence restores eligibility | Automated | `M7aWf1dEligibilityTests.PlanningGoverned_SupersedeWithoutRegrant_BlocksUntilNewGrant` | Pass | Final validation |
| MVT-9 | Traversal re-entry: prior occurrence-bound DWA inapplicable; authorization blocker until new grant | Automated (existing) | `M7aWf1dEligibilityTests.ReEntry_DoesNotReusePriorOccurrenceBoundDwa` | Pass | Final validation |
| MVT-10 | Unresolved effective configuration: Waiting On `gew.waiting-on.effective-configuration` | Automated | `GovernedWorkStateOperatorProjectionTests.Projection_DoesNotFabricateWaitingOnOrNextAction_WhenInstancesExist` | Pass | Final validation |
| MVT-11 | Post-submission routing suggestions: NotApplicable when satisfied; Denied when known blocker (STOP, dependency, effective config) | Automated (existing) | `PostSubmission_RoutingSuggestions_NotApplicable_WhenEvaluatedConstraintsSatisfied`; `PostSubmission_RoutingSuggestions_Denied_WhenInstanceStopActive`; `PostSubmission_RoutingSuggestions_Denied_WhenPendingDependency`; `PostSubmission_RoutingSuggestions_Denied_WhenEffectiveConfigurationUnresolved` | Pass | Final validation |
| MVT-12 | `CandidateFrontier` membership does not imply `FullyGovernedActionability` FullyActionable | Automated bundle | `CandidateFrontier_MixedCandidateAndBlocked`; `FullyGovernedActionability_IsNeverFullyActionable_InWf1d` | Pass | Final validation |
| MVT-13 | No fabricated synchronization Waiting On; unevaluated dimensions not mis-listed | Automated (existing) | `UnevaluatedDimensions_Empty_WhenRegistryHasNoAttachments`; `WaitingOn_ContainsOnlyEvaluatedBlockers_NotSynchronization` | Pass | Final validation |
| MVT-14 | Workflow Next Action contributions independent of relay GovernedRelayManualP0 recommendation | Automated | `EngineeringAgentA4T5OperatorProjectionTests.WorkflowNextActionsAndRelayP0_AreIndependentProjections` (joint); relay path also covered by `EligiblePaHandover_WithPluginUnavailable_RecommendsManualP0` | Pass | Final validation |

### MVT criteria reference (checklist — evidence is automated matrix above)

- **MVT-1:** Zero Active workflows — empty projections; `NoWorkflowNextActionSuggestions`.
- **MVT-2:** Single Active at Intake — routing, evaluated satisfaction, Indeterminate actionability, non-granting Next Action.
- **MVT-3:** Multiple Active — frontier membership and stable order (`WorkflowInstanceId` then place).
- **MVT-4:** Dependency Pending — Waiting On dependency + related ids; blocked off frontier.
- **MVT-5:** STOP — `WaitingOnInstanceStop`; clear restores frontier when otherwise satisfied.
- **MVT-6:** Planning DWA at planning-governed place.
- **MVT-7:** Implementation DWA at implementation-governed place.
- **MVT-8:** Supersede / re-grant at same occurrence.
- **MVT-9:** Re-entry invalidates prior occurrence-bound DWA without auto-supersede.
- **MVT-10:** Effective configuration failure Waiting On item.
- **MVT-11:** Post-submission governed place routing suggestion permissions.
- **MVT-12:** Frontier vs `FullyGovernedActionability`.
- **MVT-13:** Unevaluated dimensions / no sync fabrication.
- **MVT-14:** Relay P0 vs workflow Next Action independence.

## Historical automated execution log (provenance only — not human attestation)

These entries document **who launched** automated tests on 2026-10-06 before the verification correction. They **do not** constitute human MVT Pass authority and **must not** be cited as interactive verification.

| When | Launcher | Command / filter (summary) | Outcome | Notes |
|---|---|---|---|---|
| 2026-10-06 | Ed Becnel | `EmptyProject_ReturnsEmptyCurrentWork` | 1 passed | Pre-correction; did not assert full MVT-1 surface |
| 2026-10-06 | Ed Becnel | `Intake_DoesNotRequirePlanningOrImplementationDwa_ForEvaluatedSatisfaction` | 1 passed | Pre-correction partial MVT-2 |
| 2026-10-06 | Ed Becnel | `CandidateFrontier_MixedCandidateAndBlocked` | 1 passed | Pre-correction partial MVT-3 (no multi-eligible ordering) |
| 2026-10-06 | Ed Becnel | `DependencyBlocked_ExposesWaitingOn_ForBlockedInstance` | 1 passed | Pre-correction; projection Waiting On items not fully asserted |
| 2026-10-06 | EA batch | WF-1d-related Application filters | 56 passed (reported) | Supporting only; superseded by final validation below |

**Withdrawn:** Former “Human execution record” rows that marked MVT-1–MVT-4 **Pass** as human verification are **historicalized** in this section and replaced by the [automated verification matrix](#automated-verification-matrix-authoritative-mvt-evidence).

## Final validation (2026-10-06)

**SDK:** 10.0.401 (`DOTNET_ROOT=$HOME/.dotnet`, `PATH` includes `$HOME/.dotnet`).

**Relevant warnings:** NU1903 (`SQLitePCLRaw.lib.e_sqlite3` 2.1.10); pre-existing CS8625 / CS8767 / CA2264 in Application and test projects — not WF-1d verification failures.

### Focused (verification correction test classes)

```bash
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$PATH"
dotnet test tests/Edf.Application.Tests/Edf.Application.Tests.csproj -c Release \
  --filter "FullyQualifiedName~GovernedWorkStateOperatorProjectionTests|FullyQualifiedName~M7aWf1dEligibilityTests|FullyQualifiedName~WorkflowNextActionsAndRelayP0_AreIndependentProjections"
```

| Passed | Failed | Skipped |
|---:|---:|---:|
| 23 | 0 | 0 |

### WF-1d broader Application slice

```bash
dotnet test tests/Edf.Application.Tests/Edf.Application.Tests.csproj -c Release \
  --filter "FullyQualifiedName~M7aWf1d|FullyQualifiedName~GovernedWorkStateOperatorProjectionTests|FullyQualifiedName~DevelopmentWorkAuthorizationApplicabilityTests|FullyQualifiedName~M7aWf1cAuthorizationBoundaryTests|FullyQualifiedName~WorkflowRelationshipServiceTests|FullyQualifiedName~WorkflowInstanceStopServiceTests|FullyQualifiedName~DevelopmentWorkAuthorizationServiceTests|FullyQualifiedName~EngineeringAgentA4T5OperatorProjectionTests"
```

| Passed | Failed | Skipped |
|---:|---:|---:|
| 59 | 0 | 0 |

### Full Release solution

```bash
dotnet test ProjectConcord.sln -c Release
```

| Passed | Failed | Skipped |
|---:|---:|---:|
| 371 | 0 | 0 |

(Application **298**, ProjectServices **62**, Desktop **11**.)

**Implementation defect from strengthened tests:** None — all strengthened assertions passed without production source changes.
