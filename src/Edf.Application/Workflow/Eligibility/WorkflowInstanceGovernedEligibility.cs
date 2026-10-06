namespace Edf.Application.Workflow.Eligibility;

public sealed record WorkflowInstanceGovernedEligibility(
    bool EvaluatedConstraintsSatisfied,
    IReadOnlyList<string> EvaluatedConstraintViolationCodes,
    IReadOnlyList<GovernedEligibilityDimension> UnevaluatedApplicableDimensions,
    IReadOnlyList<GovernedEligibilityDimension> DimensionApplicabilityUnknown,
    bool EligibleUnderEvaluatedConstraints,
    GovernedActionabilityCompleteness FullyGovernedActionability,
    string FullyGovernedActionabilityReasonCode,
    WorkflowInstanceEvaluatedRoutingState RoutingState);
