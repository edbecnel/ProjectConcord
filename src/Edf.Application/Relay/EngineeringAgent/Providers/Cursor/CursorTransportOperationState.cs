namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;

internal sealed class CursorTransportOperationState
{
    public required TransportOperationId TransportOperationId { get; init; }

    public string? SessionId { get; set; }

    public EngineeringAgentTransportResultCandidate? ResultCandidate { get; set; }

    public bool ForwardAcknowledged { get; set; }
}
