namespace Edf.Domain.Projects;

public static class LocatorAvailabilityEvaluator
{
    public static LocatorAvailability Evaluate(string normalizedAbsolutePath) =>
        Directory.Exists(normalizedAbsolutePath)
            ? LocatorAvailability.Available
            : LocatorAvailability.MissingOnDisk;

    public static LocatorAvailability Evaluate(ProjectLocator locator) =>
        Evaluate(locator.NormalizedAbsolutePath);
}
