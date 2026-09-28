# MVR-NNNN: [Short title]

[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Verification](../Verification/README.md) › [Records](../Verification/Records/README.md) › MVR-NNNN

## Identity

| Field | Value |
|---|---|
| **Record ID** | MVR-NNNN |
| **Manual QA** | Required |
| **Human execution status** | Pending / In progress / Complete |
| **Owner** | [Role or name] |
| **Verification date** | YYYY-MM-DD (or period) |

**Human execution status:** Set **Complete** only when governed human manual verification was executed by an authorized human with outcomes that satisfy the verification obligation (typically **Pass** in the execution record for applicable MVTs). Do not set **Complete** because a governing record waived the obligation while MVT Results remain non-Pass.

## Verification basis / obligation

| Requirement | Link | Notes |
|---|---|---|
| [Title] | [relative/path.md](relative/path.md) | Acceptance condition |

State **what** must be verified and **why** (acceptance criteria, gate condition, or tranche requirement).

## Implementation scope

| Anchor | Value |
|---|---|
| Repository paths | |
| Branch / commit / tag | |
| Environment or build | |
| Out of scope (explicit) | |

## Operator environment and test data (optional)

_Use when manual verification requires disposable filesystem subjects, fixtures, or environment reset. Reference [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/048cdc6bc9ee581bb60fea5e7d26f7de673e5569/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md) for normative disposable-workspace semantics; do not invent project-local placement policy._

| Field | Value |
|---|---|
| **Disposable filesystem subjects required** | Yes / No |
| **Resolved DVW path(s)** | _Record concrete path(s) during execution (evidence metadata)_ |
| **Test data / fixture identification** | |
| **Safety/reset prerequisites** | |
| **Cleanup expectation** | |

## Manual verification — human execution required

_Executable operator checklist. Do not bury tests in narrative prose._

- [ ] **MVT-1** — [Short test name]

  **Procedure:**  
  [What the human does — concise steps]

  **Expected result:**  
  [Explicit pass condition]

- [ ] **MVT-2** — [Short test name]

  **Procedure:**  
  [Steps]

  **Expected result:**  
  [Pass condition]

_Add one MVT block per test in the governed obligation. Checkbox is a usability aid; the execution record below is authoritative._

## Execution record

| MVT ID | Result | Executor | Date | Evidence |
|---|---|---|---|---|
| MVT-1 | Pending / Pass / Fail / Blocked | | | [links] |
| MVT-2 | Pending / Pass / Fail / Blocked | | | |

**MVT Result** MUST be **Pending**, **Pass**, **Fail**, or **Blocked** only. **Waived** is not an MVT result. Keep checkbox state consistent with **Result**; on conflict, **Result** governs.

## Evidence references

_Link engineering artifacts outside `docs/` (screenshots, recordings, exports) per project policy. Do not paste large binaries into this file._

| MVT ID | Artifact or reference |
|---|---|
| MVT-1 | |

## Governing work / acceptance relationship

| Governing item | Link | Role |
|---|---|---|
| [EGR / tranche / GMR / AAR / PA acceptance] | [relative/path.md](relative/path.md) | Acceptance depends on this MVR |

## Notes (optional)

_Informational only. May reference a governing waiver (for example EGR gate waiver) for traceability. Notes do not change MVT Result or Human execution status._

---

## Parent

- [Verification Records](../Verification/Records/README.md)

## Related Documents

- [MVR-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md)
- [DVW-0001 (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/048cdc6bc9ee581bb60fea5e7d26f7de673e5569/docs/Specifications/DVW-0001-Disposable-Verification-Workspaces.md)
- [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md)
