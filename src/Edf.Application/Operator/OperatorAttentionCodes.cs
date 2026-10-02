namespace Edf.Application.Operator;

public static class OperatorAttentionCodes
{
    public const string EngineeringAgentPluginUnavailable = "operator.attention.engineering-agent.plugin-unavailable";
    public const string EngineeringAgentPluginIncompatible = "operator.attention.engineering-agent.plugin-incompatible";
    public const string EngineeringAgentAuthenticationFailure = "operator.attention.engineering-agent.authentication-failure";
    public const string EngineeringAgentInitializationFailure = "operator.attention.engineering-agent.initialization-failure";
    public const string EngineeringAgentForwardFailure = "operator.attention.engineering-agent.forward-failure";
    public const string EngineeringAgentModelConfigurationFailure =
        "operator.attention.engineering-agent.model-configuration-failure";
    public const string EngineeringAgentResultRetrievalFailure = "operator.attention.engineering-agent.result-retrieval-failure";
    public const string EngineeringAgentAmbiguousTransport = "operator.attention.engineering-agent.ambiguous-transport";
    public const string EngineeringAgentTransportPersistence = "operator.attention.engineering-agent.transport-persistence";
    public const string EngineeringAgentRecoverableTransport = "operator.attention.engineering-agent.recoverable-transport";
}
