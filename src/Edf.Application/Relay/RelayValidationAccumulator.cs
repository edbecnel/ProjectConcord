namespace Edf.Application.Relay;

using Edf.Domain.Relay;

public sealed class RelayValidationAccumulator
{
    private readonly List<RelayValidationDiagnostic> _diagnostics = [];

    public IReadOnlyList<RelayValidationDiagnostic> Diagnostics => _diagnostics;

    public void Add(RelayValidationDiagnosticSeverity severity, string code, string message) =>
        _diagnostics.Add(new RelayValidationDiagnostic(code, message, severity));

    public void AddIncomplete(string code, string message) =>
        Add(RelayValidationDiagnosticSeverity.Incomplete, code, message);

    public void AddMalformed(string code, string message) =>
        Add(RelayValidationDiagnosticSeverity.Malformed, code, message);

    public RelayValidationResult ToResult()
    {
        if (_diagnostics.Any(d => d.Severity == RelayValidationDiagnosticSeverity.Malformed))
        {
            return RelayValidationResult.RejectedMalformed(_diagnostics);
        }

        if (_diagnostics.Any(d => d.Severity == RelayValidationDiagnosticSeverity.Incomplete))
        {
            return RelayValidationResult.Incomplete(_diagnostics);
        }

        return RelayValidationResult.Valid(_diagnostics);
    }
}
