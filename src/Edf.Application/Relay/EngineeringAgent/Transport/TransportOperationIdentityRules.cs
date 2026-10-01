namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Relay;

/// <summary>
/// Identity separation invariants for transport operations (ADR-0022 §4).
/// </summary>
public static class TransportOperationIdentityRules
{
    public static bool SharesSameGuidValue(TransportOperationId operationId, GovernedPackageId packageId) =>
        operationId.Value == packageId.Value;

    public static bool SharesSameGuidValue(TransportOperationId operationId, GovernedCorrelationId correlationId) =>
        operationId.Value == correlationId.Value;

    public static bool SharesSameGuidValue(
        TransportOperationId operationId,
        EngineeringAgentProviderPluginId pluginId) =>
        Guid.TryParse(pluginId.Value, out var guid) && operationId.Value == guid;

    /// <summary>
    /// Transport operations must use their own identity; callers must not reuse package ids as operation ids.
    /// </summary>
    public static TransportOperationId RequireDistinctFromPackage(
        TransportOperationId operationId,
        GovernedPackageId sourcePackageId)
    {
        if (SharesSameGuidValue(operationId, sourcePackageId))
        {
            throw new InvalidOperationException(
                "TransportOperationId must not equal SourcePackageId (same Guid value).");
        }

        return operationId;
    }
}
