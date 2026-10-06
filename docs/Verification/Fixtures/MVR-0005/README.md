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
