namespace Edf.Application.Relay.Serialization;

public sealed record GovernedRelayHumanReadablePackageOptions(
    GovernedRelayHumanReadableCounterparty Counterparty,
    string? OperatorWorkSubject = null);
