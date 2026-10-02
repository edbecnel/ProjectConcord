using Edf.Domain.Projects;

namespace Edf.Application.Tests.Relay.EngineeringAgent;

internal static class EngineeringAgentTestWorkspace
{
    internal static ProjectLocator DefaultLocator =>
        ProjectLocator.FromPath(Path.GetTempPath());
}
