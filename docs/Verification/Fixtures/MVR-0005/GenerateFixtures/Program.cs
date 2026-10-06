using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Tests.Workflow.PlanningEntry;
using Edf.Domain.Projects;

// Deterministic MVR-0005 planning-entry PA handover fixture (verification support — not human attestation).
// Regenerate: dotnet run --project docs/Verification/Fixtures/MVR-0005/GenerateFixtures

var outputDir = args.Length > 0
    ? args[0]
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

Directory.CreateDirectory(outputDir);

var paAdapter = new ProjectArchitectManualAdapter();
var projectId = ProjectConcordProjectId.Parse("f4f11783-2c48-44f0-9e24-50df4ab51e02");
var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(projectId);
var rendered = paAdapter.RenderPaReviewPackage(package);

File.WriteAllText(
    Path.Combine(outputDir, "pa-handover-planning-entry-valid.relay.txt"),
    rendered.TrimEnd() + Environment.NewLine);

File.WriteAllText(
    Path.Combine(outputDir, "README.md"),
    """
# MVR-0005 planning-entry PA handover fixture

Automation-prepared **test data** for [MVR-0005](../../Records/MVR-0005-operator-mvp-real-ui-1-human-interactive.md) Intake → governed planning entry.

**Not** the MVR-0002 implementation-authorized handover. Embodies:

- `planningAuthorized: true`
- `implementationAuthorized: false`
- `directsImplementationWork: false`
- no `developmentWorkAuthorization` projection
- STOP inactive

Regenerate from repository root:

```bash
dotnet run --project docs/Verification/Fixtures/MVR-0005/GenerateFixtures -- \
  docs/Verification/Fixtures/MVR-0005
```

""".TrimEnd() + Environment.NewLine);

Console.WriteLine($"Wrote MVR-0005 fixtures to: {outputDir}");
