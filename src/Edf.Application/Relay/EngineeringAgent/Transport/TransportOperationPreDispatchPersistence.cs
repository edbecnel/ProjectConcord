namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Durable pre-dispatch boundary before external provider Forward (A4-T4 bounded T3 refinement).
/// </summary>
internal static class TransportOperationPreDispatchPersistence
{
    public static bool TryPersistForwardInProgress(
        ITransportOperationStore store,
        TransportOperation operation,
        TimeProvider clock,
        out TransportOperation inProgress,
        out Exception? failure)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(clock);

        inProgress = operation with
        {
            LifecycleState = TransportOperationLifecycleState.ForwardInProgress,
            UpdatedUtc = clock.GetUtcNow(),
        };

        try
        {
            store.Save(inProgress);
            failure = null;
            return true;
        }
        catch (Exception ex)
        {
            failure = ex;
            inProgress = operation;
            return false;
        }
    }
}
