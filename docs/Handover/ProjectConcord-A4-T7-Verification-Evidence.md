# ProjectConcord A4-T7 — Verification Evidence

**Tranche:** A4-T7 — End-to-end validation, MVR-0003, A4 closeout evidence  

## PA disposition — blocker-independent closeout (2026-10-02)

**ACCEPTED (not implying T7 Pass / MVR Complete / A4 closed):**

1. Blocker-independent **automated** verification evidence (Release build/tests; machine support for Groups B/D).
2. Human verification **Pass** for MVT-16, MVT-17, MVT-18, MVT-19, MVT-20, MVT-21, MVT-22, MVT-24 per [MVR-0003](../Verification/Records/MVR-0003-a4-engineering-agent-automated-transport.md).
3. MVR-0003 **In progress**; A4-T7 **not fully complete**; A4 **not closed**.
4. Historical MVT-7 / Retest #1 / Retest #2 **Fail** preserved; Retest #3 **NOT EXECUTED / BLOCKED** (Cursor ACP Composer 2.5 **non-fast**).
5. Group A/B live-forward remainder and MVT-23 **blocked** (external provider capability).
6. **GAP-053** — Go to Folder locator defect **separate** from ACP blocker; did not invalidate blocker-independent human tests.

**Status:** **Blocker-independent human MVR tranche complete** (2026-10-02) — Groups **C** / **D** human MVTs **Pass** per [MVR-0003](../Verification/Records/MVR-0003-a4-engineering-agent-automated-transport.md); **overall T7 / MVR-0003 remain open** on Cursor ACP Composer 2.5 **non-fast** external blocker (Group **A** MVT-8–11, Retest #3; Group **B** MVT-12–15; MVT-23). **MVT-7 / Retest #1–2 Fail preserved**; A4-T6 **#6 PA ACCEPTED** locally (not publication).  
**Authorization:** A4-T7 authorized 2026-10-01 (verification/docs only; **no `src/` remediation** under T7)  
**Baseline HEAD:** `eecf0538bd4623d837c93d06439a39b2b6c0abe0` (published A4-T6 on `main`)

## A. Starting baseline

| Check | Result |
| --- | --- |
| `git rev-parse HEAD` | `eecf0538bd4623d837c93d06439a39b2b6c0abe0` |
| Branch | `main` |
| `main` vs `origin/main` | **0 ahead / 0 behind** |
| Working tree at T7 start | **clean** |

## B. Environment / platform

| Field | Value |
| --- | --- |
| OS | macOS (darwin 25.6.0, arm64) |
| .NET SDK | **10.0.401** (`export DOTNET_ROOT="$HOME/.dotnet"`; `export PATH="$HOME/.dotnet:$PATH"`) |
| Desktop stack | Avalonia (`Edf.Desktop`) |

## C. Disposable Project Root (MVR subject)

| Field | Value |
| --- | --- |
| Path | `/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A` |
| Pre-open `.projectconcord/` | **Absent** (automation verified 2026-10-01) |
| Setup notes | [docs/Verification/Fixtures/MVR-0003/README.md](../Verification/Fixtures/MVR-0003/README.md) |

## D. Release validation (SDK 10.0.401)

| Step | Command | Result |
| --- | --- | --- |
| Release build | `dotnet build -c Release` | **PASS** — 0 errors |
| Release tests | `dotnet test -c Release` | **PASS** — **298** total (Application **231**, Desktop **11**, ProjectServices **56**); **0** failed (publication-candidate validation, 2026-10-02) |
| Relay / EA regression filter | `dotnet test -c Release --filter "FullyQualifiedName~Relay\|FullyQualifiedName~EngineeringAgent\|FullyQualifiedName~Operator"` | **PASS** — **200** tests |

### Warning categories (T7 run)

| Category | Count | Disposition |
| --- | ---: | --- |
| NU1903 (`SQLitePCLRaw.lib.e_sqlite3`) | 12 | Governed — GAP-045 / AWI-0007; not A4-T7 failure |
| CS8625 (`InMemoryTransportOperationStore`) | 1 | Pre-existing T6 baseline; not introduced in T7 |
| CA2264 (`SqliteTransportOperationStoreAdapter`) | 1 | Pre-existing T6 baseline; not introduced in T7 |

No new error or warning **category** observed versus published T6 baseline.

## E. Real Cursor environment / integration

| Check | Result |
| --- | --- |
| `cursor` on default `PATH` | **Not found** (IDE bundle only) |
| Cursor IDE CLI | `/Applications/Cursor.app/Contents/Resources/app/bin/cursor` — version **3.22.12** |
| `agent` on `PATH` | **`/Users/edbecnel/.local/bin/agent`** (installed via `cursor agent` bootstrap during T7 probe) |
| `agent --version` | **2026.09.28-64d2043** |
| `agent acp` | **Available** (`agent acp --help` succeeds) |
| Authentication / readiness | **Reconciled 2026-10-02** — see [A4-T6 corrective remediation notes](ProjectConcord-A4-T6-Corrective-Remediation-Notes.md) § ACP authentication reconciliation: `agent status` reports logged in but `agent -p` reports **invalid stored authentication**; documented standalone ACP flow fails at `authenticate` identically to ProjectConcord wire |
| `PROJECTCONCORD_CURSOR_AGENT_PATH` | Optional; provider default resolves executable name **`agent`** ([`CursorCliLocator`](../../src/Edf.Application/Relay/EngineeringAgent/Providers/Cursor/CursorAcpSubprocessTransport.cs)) |
| `[Trait("RequiresCursor")]` tests | **1** test — placeholder trait declaration only; **no live ACP integration test** in default suite |
| Filter `RequiresCursor=true` | **PASS** — 1 passed (trait smoke) |

**T7 note:** Automated forward MVTs **must not** be marked Pass without human observation of a successful authenticated forward. If the environment cannot authenticate, record **Blocked** on applicable MVTs and STOP for PA disposition per authorization.

## E2. Cursor ACP provider capability blocker (2026-10-02)

| Field | Value |
| --- | --- |
| Agent version | **2026.09.28-64d2043** |
| Blocker | Installed **ACP** cannot select/verify **Composer 2.5 non-fast** (`composer-2.5[fast=false]` / `composer-2.5[]` rejected or coerced to `composer-2.5[fast=true]`) |
| ProjectConcord model contract | **Locally complete** — default Composer 2.5 / `fast=false`; fail-closed; no silent fast fallback |
| MVT-7 Retest #3 | **NOT EXECUTED** — blocked until external provider resolves or PA changes policy |
| Canonical write-up | [A4-T6 corrective remediation notes — closeout](ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#a4-t6--t7--closeout-preparation-pa-2026-10-02) |
| Cursor escalation text | Same document — [provider issue report](ProjectConcord-A4-T6-Corrective-Remediation-Notes.md#cursor-provider-issue-report-reproducible-no-projectconcord-required) |

## F. MVR-0003

| Field | Value |
| --- | --- |
| Record | [MVR-0003](../Verification/Records/MVR-0003-a4-engineering-agent-automated-transport.md) |
| Human execution status | **In progress** (2026-10-02) — blocker-independent human tranche recorded; **Complete** withheld while Group A/B / MVT-23 blocked |
| MVT dispositions (human) | MVT-1–6 **Pass**; MVT-7 + Retest #1–2 **Fail** (historical); Retest #3 **not executed / blocked**; MVT-8–15 **blocked** / **not executed**; MVT-16–22, MVT-24 **Pass**; MVT-23 **blocked** — authoritative detail in MVR execution record |
| Fixtures | [MVR-0003 fixtures](../Verification/Fixtures/MVR-0003/README.md) |

## G. Automated forward evidence (machine support only)

| Area | Evidence |
| --- | --- |
| Cursor provider unit/integration (scripted ACP) | `EngineeringAgentA4T6CursorProviderTests` (**15**), `CursorAcpNdjsonCodecTests` (**2**) — Release **PASS** |
| Orchestrator import boundary | `EngineeringAgentA4T3OrchestrationTests` — governed `ImportEngineeringResult` on successful candidate |
| Production catalog | `EngineeringAgentPluginHostingFactory` registers **`cursor-acp-reference`** only |

Human Group A attestation: **blocked** (external provider) — MVT-8–11 **not executed**; Retest #3 **not executed** — see MVR-0003.

## H. Recovery confirmation evidence (machine support only)

| Area | Evidence |
| --- | --- |
| AF-1 explicit confirm path | `EngineeringAgentA4T4RecoveryTests` — `ConfirmRecoveredResultImport_CompletesAfterExplicitConfirmation` |
| No startup recovery in Desktop | `ProductionComposition_ExposesRecoveryWithoutStartupSideEffects`; T5 notes — recovery service **not** wired to Desktop auto-exec |
| Operator Attention for recoverable ops | `EngineeringAgentA4T5OperatorProjectionTests`; `EngineeringAgentTransportOperatorAttentionContributor` |

Human Group B attestation: **blocked** on forward-in-progress chain — MVT-12–15 **not executed** (machine AF-1 tests **PASS** only) — see MVR-0003.

## I. P0 fallback evidence

| Area | Evidence |
| --- | --- |
| Automated suite | `RelayWorkflowViewModelAutomatedTransportTests` — manual prepare after provider unavailable |
| MVR-0002 precedent | P0 Prepare/Copy/Import unchanged |
| Human Group C | **Pass** — MVT-16–19 (2026-10-02) per MVR-0003 execution record |

## J. Governance / boundary regression (machine support)

| Check | Result |
| --- | --- |
| DEBUG routing | `CursorRoutingIntentMapper` — DEBUG **unsupported** (tests in T6 suite) |
| Provider names in neutral `src/` (excl. Cursor provider folder) | **0** product-name matches outside `Providers/Cursor/` (composition factory import only) |
| A3 store/UI scope in `src/` | **No** accidental A3 implementation stores |
| AAR-0002 | **Not created** (per A4 plan §19) |

Human Group D attestation: **Pass** — MVT-20–22, MVT-24 (2026-10-02); MVT-23 **blocked** (requires MVT-8) — MVR-0003.

## K. A4 closeout criteria matrix (evidence preparation)

| # | Criterion | T7 evidence state |
| --- | --- | --- |
| 1 | A4-T0 … T7 authorized work complete | T0–T6 **PA published**; T6 **local implementation complete**; T7 **blocked** on Cursor ACP non-fast (Group A / Retest #3) |
| 2 | Release validation SDK 10.0.401 | **PASS** (298 tests, local corrective tree) |
| 3 | MVR-0003 Complete + applicable MVT Pass | **Incomplete** — Human execution status **In progress**; Group A/B + MVT-23 **blocked**; Groups C/D human **Pass**; machine support **PASS** (§N) |
| 4 | P0 manual relay regression | Automated **PASS**; MVR Group C human MVT-16–19 **Pass** (2026-10-02) |
| 5 | Roadmap ready for A4 complete after PA closeout | **Updated locally** — not “implementation complete” until PA |
| 6 | No accidental A3 in `src/` | **Scope audit PASS** (relay profile boundaries only) |
| 7 | Final PA accept/publish on A4 closeout | **Pending** |

## L. Scope audit (T7)

| Check | Result |
| --- | --- |
| A3 implementation | **Not authorized** — no new A3 scope in T7 docs |
| Unauthorized `src/` remediation | **None** — T7 changes are **docs only** |
| Accidental architecture change | **None** in `src/` |
| AAR-0002 | **Not created** |

## N. T7 blocker-independent verification tranche (2026-10-02)

**Authorization:** Complete truthful Groups **B–D** verification that does not require Group A / Retest #3 / non-fast live ACP. **No** `src/` changes; **no** live Cursor; **no** additional ACP experimentation.

| Activity | Result |
| --- | --- |
| `dotnet build -c Release` | **PASS** — 0 errors (NU1903 / CS8625 / CA2264 — pre-existing) |
| `dotnet test -c Release` | **PASS** — **298** total (App **231**, Desktop **11**, PS **56**); 0 failed |
| Relay / EA / Operator filter | **PASS** — **249** total (App **212**, Desktop **7**, PS **30**); 0 failed |
| MVR MVT-8–11 | **BLOCKED** — external provider capability |
| MVR MVT-12–14 | **BLOCKED** — external provider capability (forward-in-progress dependency) |
| MVR MVT-15 | **NOT EXECUTED** (human); chain blocked |
| MVR MVT-16–19 | **Pass** (human 2026-10-02); machine P0/unavailable-provider tests **PASS** |
| MVR MVT-20–22, 24 | **Pass** (human 2026-10-02); machine routing/Attention scope tests **PASS** |
| MVR MVT-23 | **BLOCKED** — external provider capability (requires MVT-8) |
| New production defect (automated tranche) | **None** |
| macOS Go to Folder… locator observation | **Recorded** — §O; **not** classified as MVT failure |

Detail and precondition table: [MVR-0003 § T7 blocker-independent tranche](../Verification/Records/MVR-0003-a4-engineering-agent-automated-transport.md#t7-blocker-independent-verification-tranche-2026-10-02).

**Engineering Agent consumption (2026-10-02):** Human results for MVT-16–22 and MVT-24 taken **only** from MVR-0003 execution record — no duplicate human attestation.

## O. macOS Go to Folder locator defect observation (2026-10-02)

| Field | Value |
| --- | --- |
| **Classification** | **Observed UI / locator defect** — **[GAP-053](../Development/EDF_Gap_Register.md#gap-053--macos-go-to-folder-project-locator-vs-browse-open-identity)**; separate from Cursor ACP non-fast blocker |
| **Symptom** | **File → Go to Folder…** with path `/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A` opened project ID **`ddde280f-38ad-4bfb-abb9-86be2c939d15`** |
| **Expected** | Canonical disposable Root-A project ID **`a5e26be6-9769-46f6-bd94-1669764fe8af`** (unchanged on disk) |
| **Workaround** | Normal folder-browse selection of the same Root-A path — verifier completed blocker-independent MVTs using browse workflow |
| **MVR impact** | **Did not block** MVT-16–22 / MVT-24 completion |
| **Remediation** | **Not authorized** in T7 closeout tranche — **no** `src/` change |
| **Investigation before T7 PA disposition** | **Recommended for PA** — root cause unknown (possible duplicate `managed_projects` locator registration vs Go-to-Folder path normalization); **not required** to accept blocker-independent human evidence already recorded |
| **Related precedent** | [MVR-0001 MVT-2](../../Verification/Records/MVR-0001-a1c-desktop-project-root-recent-workflow.md) — cautions on Go to Folder / `/private` path variants |

## M. Publication

**STOP:** **No commit / no push** per A4-T7 authorization. Do not record **A4-T7 PA ACCEPTED / PUBLISHED** or **overall A4 CLOSED** until PA review.

**Publication packaging readiness (local corrective tree):** **Ready for PA review** as an **uncommitted** bundle — blocker-independent **human** evidence **consumed**; **explicit external Cursor ACP blocker** remains for Group A/B/MVT-23 — not as full MVR **Complete**, T7 **closed**, or MVT-7/Retest #3 success.

## Related

- [A4 Implementation Plan](ProjectConcord-A4-Implementation-Plan.md) §17–§20, §24 A4-T7
- [MVR-0003](../Verification/Records/MVR-0003-a4-engineering-agent-automated-transport.md)
- [A4-T6 Implementation Notes](ProjectConcord-A4-T6-Implementation-Notes.md)
