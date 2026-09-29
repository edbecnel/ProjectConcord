namespace Edf.Application.Relay;

using Edf.Domain.Relay;

/// <summary>
/// B-layer Software Development profile validation seam (PC-AIGOV-003, PC-AIGOV-004).
/// Core orchestration invokes this without embedding B semantics in domain types.
/// </summary>
public interface ISoftwareDevelopmentRelayProfileValidator
{
    void Validate(GovernedRelayPackage package, RelayValidationAccumulator accumulator);
}
