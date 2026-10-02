using Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

public class CursorAcpModelResolverTests
{
    [Fact]
    public void Parse_FastTrueParameter_IsDistinctFromNonFast()
    {
        var fast = CursorAcpModelIdentity.Parse("composer-2.5[fast=true]");
        var nonFast = CursorAcpModelIdentity.Parse("composer-2.5");

        Assert.True(fast.FastEnabled);
        Assert.False(nonFast.FastEnabled);
        Assert.True(fast.Matches(new CursorEngineeringAgentModelSelection("composer-2.5", FastEnabled: true)));
        Assert.False(fast.Matches(CursorEngineeringAgentModelSelection.DefaultComposer25NonFast));
    }

    [Fact]
    public void Resolve_DefaultNonFast_SelectsPlainComposerWireId()
    {
        var available = new[]
        {
            new CursorAcpAdvertisedModel("composer-2.5[fast=true]", "composer-2.5"),
            new CursorAcpAdvertisedModel("composer-2.5", "composer-2.5"),
        };

        var ok = CursorAcpModelResolver.TryResolveWireModelId(
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            available,
            out var wire,
            out var message);

        Assert.True(ok);
        Assert.Equal("composer-2.5", wire);
        Assert.Null(message);
    }

    [Fact]
    public void Resolve_OnlyFastAdvertised_FailsClosed()
    {
        var available = new[] { new CursorAcpAdvertisedModel("composer-2.5[fast=true]", "composer-2.5") };

        var ok = CursorAcpModelResolver.TryResolveWireModelId(
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            available,
            out _,
            out var message);

        Assert.False(ok);
        Assert.Contains("fast=false", message, StringComparison.Ordinal);
    }

    [Fact]
    public void Resolve_MultipleNonFastMatches_FailsClosed()
    {
        var available = new[]
        {
            new CursorAcpAdvertisedModel("composer-2.5", "a"),
            new CursorAcpAdvertisedModel("composer-2.5[fast=false]", "b"),
        };

        var ok = CursorAcpModelResolver.TryResolveWireModelId(
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            available,
            out _,
            out var message);

        Assert.False(ok);
        Assert.Contains("ambiguous", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Verify_RejectsFastWireId_ForNonFastSelection()
    {
        var ok = CursorAcpModelResolver.VerifyCurrentModelId(
            "composer-2.5",
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            "composer-2.5[fast=true]",
            out var message);

        Assert.False(ok);
        Assert.NotNull(message);
    }
}
