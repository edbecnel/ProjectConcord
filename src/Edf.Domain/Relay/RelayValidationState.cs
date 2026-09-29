namespace Edf.Domain.Relay;

/// <summary>
/// Neutral relay validation disposition for a governed package envelope.
/// </summary>
public enum RelayValidationState
{
    /// <summary>Structurally safe; required governance metadata and boundary rules satisfied.</summary>
    Valid = 0,

    /// <summary>Structurally interpretable but required governance information is absent or insufficient.</summary>
    Incomplete = 1,

    /// <summary>Structurally unsafe or contradictory; cannot be treated as a governed relay package.</summary>
    RejectedMalformed = 2,
}
