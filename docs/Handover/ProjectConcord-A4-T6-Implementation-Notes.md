# ProjectConcord A4-T6 — Implementation Notes

**Tranche:** A4-T6 — Cursor provider and ACP adapter  
**Status:** **CLOSED / PROJECT ARCHITECT ACCEPTED / PUBLISHED** (2026-10-01)  
**Pre-T6 baseline:** `61d36a48b5e2c215a62752b540e7a45565115162` (A4-T5 publication on `main`)  
**T6 publication:** Recorded on `main` (2026-10-01 tranche — see plan [§33](ProjectConcord-A4-Implementation-Plan.md#33-a4-t6-closeout-2026-10-01))

## Authorization

| Item | Status |
|------|--------|
| A4-T6 implementation | **PA AUTHORIZED** (2026-10-01) |
| A4-T6 acceptance / publication | **CLOSED / PA ACCEPTED / PUBLISHED** (2026-10-01) |
| A4-T7 | **NOT AUTHORIZED** |
| Overall A4 | **In progress / not closed** |

## Cursor provider

| Item | Value |
|------|--------|
| Plugin type | `CursorEngineeringAgentProviderPlugin` |
| Identity | `cursor-acp-reference` (`CursorEngineeringAgentPluginIds.Reference`) |
| Registration | Static `EngineeringAgentPluginCatalog.FromRegistrations` in `EngineeringAgentPluginHostingFactory` |
| Routing | PLAN → `plan`, AGENT → `agent`, DEBUG → **unsupported** (`RoutingIntentUnsupported`; `SupportsDebugSemantically: false`) |
| Session hint | Opaque `EngineeringAgentProviderSessionHandle` — not parsed outside plugin |

## ACP adapter (provider-internal)

| Component | Role |
|-----------|------|
| `CursorAcpClient` | initialize, authenticate, session/new\|load, session/prompt, session/update, permission_response, session/cancel |
| `CursorAcpNdjsonCodec` | NDJSON JSON-RPC framing |
| `CursorAcpSubprocessTransport` | `agent acp` subprocess stdio (`PROJECTCONCORD_CURSOR_AGENT_PATH` optional) |
| `ICursorAcpTransport` | Provider-internal transport port (scripted in tests) |
| `CursorAcpPermissionPolicy` | Deny-by-default; allow-once supported |

No ACP types in provider-neutral orchestration, relay bridge, or Desktop. Headless `agent -p` **not** implemented.

## Boundaries preserved

- `IEngineeringAgentRelayBridge` unchanged (render/parse only).
- T4 durability ordering unchanged (orchestrator not modified).
- T4 recovery unchanged; no startup/background automation.
- P0 manual relay unchanged; fake provider remains tests-only.
- No dynamic plugin discovery, MVR-0003 execution, or A3.

## Tests

| Suite | Count | Notes |
|-------|------:|-------|
| `EngineeringAgentA4T6CursorProviderTests` | **15** | Scripted ACP transport fixtures |
| `CursorAcpNdjsonCodecTests` | **2** | Framing |
| Optional | **1** | `[Trait("RequiresCursor")]` — not required for default Release suite |

## Final validation (publication)

| Item | Value |
|------|--------|
| SDK | **.NET 10.0.401** |
| Build | `dotnet build -c Release` — **0** errors; **12× NU1903** (GAP-045 / AWI-0007) |
| Test | `dotnet test -c Release` — **249** passed, **0** failed (T5 baseline **233**; **+16** T6) |

## STOP

**A4-T7 NOT AUTHORIZED** pending separate Project Architect authorization. MVR-0003 scope reconfirm at T7 authorization per A4 plan §18.
