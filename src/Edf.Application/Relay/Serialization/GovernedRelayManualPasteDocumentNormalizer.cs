namespace Edf.Application.Relay.Serialization;

using Edf.Domain.Relay;

/// <summary>
/// E-layer manual paste normalization before canonical relay v1 import. Does not grant authority.
/// </summary>
public static class GovernedRelayManualPasteDocumentNormalizer
{
    public static bool TryNormalizeToCanonicalRelayDocument(
        string pastedText,
        out string canonicalDocument,
        out RelayValidationResult failure) =>
        GovernedRelayTolerantManualPasteInterpreter.TryRecoverCanonicalDocument(
            pastedText,
            out canonicalDocument,
            out failure);
}
