# MVR-0002 relay paste fixtures

Automation-prepared **test data** for [MVR-0002](../../Records/MVR-0002-a2-p0-manual-governed-relay-workflow.md).  
Copy file contents into the Desktop **PA handover import** or **Engineering result import** fields as directed by each MVT.

**Regenerate** (from ProjectConcord repository root, SDK 10.0.401):

```bash
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$PATH"
dotnet run --project docs/Verification/Fixtures/MVR-0002/GenerateFixtures -- \
  docs/Verification/Fixtures/MVR-0002
```
