namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;

/// <summary>
/// Durable Cursor model intent (family + fast mode), separate from ACP wire <c>modelId</c> strings.
/// </summary>
public sealed record CursorEngineeringAgentModelSelection(string FamilySlug, bool FastEnabled)
{
    public static CursorEngineeringAgentModelSelection DefaultComposer25NonFast =>
        new(CursorEngineeringAgentModelFamilies.Composer25, FastEnabled: false);
}
