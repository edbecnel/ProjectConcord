namespace Edf.Domain.Relay;

public enum RelayValidationDiagnosticSeverity
{
    Information = 0,
    Incomplete = 1,
    Malformed = 2,
}

public sealed record RelayValidationDiagnostic(
    string Code,
    string Message,
    RelayValidationDiagnosticSeverity Severity);
