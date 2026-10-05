using Edf.Application.Operator.WorkState;

namespace Edf.Application.Workflow;

public static class EffectiveConfigurationReasonCodes
{
    public static string ToUnavailableReason(EffectiveConfigurationResolveFailureCode code) =>
        code switch
        {
            EffectiveConfigurationResolveFailureCode.MissingProfile =>
                GovernedWorkStateUnavailableReasons.EffectiveConfigMissingProfile,
            EffectiveConfigurationResolveFailureCode.UnknownProfile =>
                GovernedWorkStateUnavailableReasons.EffectiveConfigUnknownProfile,
            EffectiveConfigurationResolveFailureCode.UnknownDefinition =>
                GovernedWorkStateUnavailableReasons.EffectiveConfigUnknownDefinition,
            EffectiveConfigurationResolveFailureCode.UnknownPlace =>
                GovernedWorkStateUnavailableReasons.EffectiveConfigUnknownPlace,
            _ => GovernedWorkStateUnavailableReasons.EffectiveConfigUnknownDefinition,
        };
}
