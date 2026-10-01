namespace Edf.Application.Relay.EngineeringAgent.Hosting;

/// <summary>
/// Production T1 hosting/selection services (ADR-0023). Catalog may be empty until A4-T6.
/// </summary>
public sealed record EngineeringAgentPluginHostingServices(
    EngineeringAgentPluginCatalog Catalog,
    EngineeringAgentPluginHost Host,
    IEngineeringAgentPluginProjectPreferencesStore Preferences,
    EngineeringAgentPluginSelectionService Selection);
