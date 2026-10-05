namespace Edf.Application.Workflow;

public static class DevelopmentWorkAuthorizationTrancheKey
{
    public static string Normalize(string? authorizedTrancheId) =>
        string.IsNullOrWhiteSpace(authorizedTrancheId) ? string.Empty : authorizedTrancheId.Trim();
}
