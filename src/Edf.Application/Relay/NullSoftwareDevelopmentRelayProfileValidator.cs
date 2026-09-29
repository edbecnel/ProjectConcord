namespace Edf.Application.Relay;

using Edf.Domain.Relay;

/// <summary>
/// Default no-op B validator for Core-only validation paths until T4 profile rules land.
/// </summary>
public sealed class NullSoftwareDevelopmentRelayProfileValidator : ISoftwareDevelopmentRelayProfileValidator
{
    public static NullSoftwareDevelopmentRelayProfileValidator Instance { get; } = new();

    public void Validate(GovernedRelayPackage package, RelayValidationAccumulator accumulator)
    {
    }
}
