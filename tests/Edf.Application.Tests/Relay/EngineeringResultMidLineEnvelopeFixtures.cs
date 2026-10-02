using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

/// <summary>
/// Deterministic structural regression for #3B-style mid-line render markers (sanitized; no live capture secrets).
/// </summary>
internal static class EngineeringResultMidLineEnvelopeFixtures
{
    public sealed record Fixture(string RawEnvelope);

    public static Fixture Load()
    {
        var package = RelaySerializationFixtures.CreateBasePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            authorizationDispositionPresent: false,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty) with
        {
            ProjectId = ProjectConcordProjectId.Parse("47038367-0279-456e-89bc-e22cba303502"),
            CorrelationId = GovernedCorrelationId.Parse("0adb436d-5046-473f-b5bd-6801fd9d1cde"),
            Kind = GovernedPackageKind.EngineeringResultImport,
            GovernanceCritical = new RelayGovernanceCriticalState(
                EngineeringAgentMode.Agent,
                EngineeringAgentMode.Plan,
                new EngineeringAgentModeTransition(EngineeringAgentMode.Plan, EngineeringAgentMode.Agent),
                new RelaySessionContinuity(
                    AgentSessionIntent.Continue,
                    AgentSessionAdvisory.None,
                    AgentSessionIntent.New,
                    AgentSessionAdvisory.None),
                RelayStopMetadata.None,
                false,
                false,
                new RelayGovernanceDirectiveFlags(false, false),
                new EdfGovernanceCorrelation(["ADR-0013", "SPEC-004"])),
        };

        var doc = new GovernedRelayV1Renderer().Render(package).TrimEnd();
        var firstNewline = doc.IndexOf('\n');
        var firstLine = doc[..firstNewline];
        var remainder = doc[(firstNewline + 1)..];
        var raw =
            "I read the handover. No repository file changes will be made.\n"
            + $"More prose.{firstLine}\n{remainder}\nAfterthought commentary.";

        return new Fixture(raw);
    }
}
