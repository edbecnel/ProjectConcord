[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Specifications](../README.md) › SPEC-005

# SPEC-005: Manual Verification Record Consumption

## Metadata

| Field | Value |
|---|---|
| **Spec ID** | SPEC-005 |
| **Status** | Draft |
| **Owner** | ProjectConcord |
| **Normative** | Yes — ProjectConcord MVR capability |
| **Last Reviewed** | 2026-09-28 |
| **Target release** | M2–M5 (phased per [Implementation Roadmap](../../Development/Implementation_Roadmap.md)) |
| **Governing EDF baseline** | `192fe5c1c6254c51e257d24aefc09e127ce72464` ([MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md)) |

## Parent

- [Specifications](../README.md)

## Related Documents

- [SPEC-003 — Canonical artifact integrity](SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) — referenced for integrity / transitions; not duplicated here
- [ADR-0014 — MVR human attestation and AI boundary](../../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)
- [ADR-0002 — EDF canonical source of truth](../../Architecture/ADRs/ADR-0002-EDF-Canonical-Source-of-Truth.md)
- [ADR-0004 — Derived data and cache](../../Architecture/ADRs/ADR-0004-Derived-Data-and-Cache.md)
- [ADR-0006 — AI boundary](../../Architecture/ADRs/ADR-0006-AI-Boundary.md)
- [MVR handover](../../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md)
- [GAP-042](../../Development/EDF_Gap_Register.md)

---

## 1. Purpose

Define normative ProjectConcord behavior for **consuming** EDF **Manual Verification Records (MVR)** and **Manual Verification Tests (MVT)** so that:

- canonical EDF Markdown remains authoritative;
- required human manual QA is **operationally visible** without reading planning prose;
- humans record verification outcomes through an explicit attestation workflow;
- AI cannot impersonate human manual verification.

EDF owns MVR semantics. ProjectConcord **consumes and projects** them ([MVR handover](../../Handover/EDF-Manual-Verification-Record-Architecture-Handover.md)).

## 2. Scope

### In scope

- Discovery, deterministic parsing, and integrity handling for MVR instance files
- Derived projection contract (implementation-neutral)
- Pending manual-QA detection and ATTENTION surfacing (minimum UX)
- Human attestation write-back for MVT Results and Human execution status
- Representational checkbox synchronization
- Governing-work relationships and waiver separation
- Human-Attestation Authority Boundary ([ADR-0014](../../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md))

### Out of scope

- Redefining EDF MVR, EGR, AAR, GMFP, or GDO semantics
- Enterprise test-management features
- Automatic mutation of EGR gate state from MVR state
- Optional vs informational MVT classification unless EDF provides a deterministic mechanism
- Framework Advisor MVR recognition (EDF deferred)
- TRV CC-4B migration

## 3. Canonical authority

1. MVR instance Markdown under `docs/Verification/Records/` is **canonical** for procedure, execution record, Human execution status, and evidence references.
2. Derived projections ([ADR-0004](../../Architecture/ADRs/ADR-0004-Derived-Data-and-Cache.md)) are **rebuildable** and **non-authoritative**.
3. External edits (IDE, Git) MUST trigger re-parse and reconciliation per [SPEC-003](SPEC-003-canonical-artifact-integrity-and-authorized-state-transitions.md) where applicable.

## 4. MVT obligation model

1. For minimum adoption, the governed verification obligation is the set of **MVTs declared in the canonical MVR** (checklist section and execution record rows).
2. ProjectConcord MUST NOT infer **optional** or **informational** MVT semantics from narrative prose.
3. Future optional/informational MVT semantics require EDF-defined deterministic representation or an explicitly accepted ProjectConcord extension.

## 5. Discovery and canonical identity

1. Files matching `docs/Verification/Records/MVR-[0-9]{4}-*.md` MAY be treated as **MVR candidates**.
2. Canonical **Record ID** (`MVR-NNNN`) MUST be read from Identity metadata in the file.
3. When Record ID is **valid and consistent** with path/filename conventions, the Engine SHALL register the artifact as type **MVR**.
4. When Record ID is **absent, malformed, or inconsistent** with path/filename:
   - ProjectConcord SHALL surface an **integrity/conformance condition**;
   - ProjectConcord MAY retain candidate path information for diagnostics/navigation;
   - ProjectConcord MUST NOT silently manufacture canonical identity from filename;
   - Discovery MUST NOT mutate canonical Markdown.
5. Future human-approved repair MAY be offered separately; it is not part of discovery.

## 6. Deterministic parsing

The Engine SHALL identify without narrative NLP interpretation:

| Element | Source |
|---|---|
| **Manual QA** | Identity metadata (`Required` / `Not required`) |
| **Human execution status** | Identity metadata (`Pending` / `In progress` / `Complete`) |
| **MVT ID** | Checklist blocks and execution record table |
| **MVT Result** | Execution record table (`Pending` / `Pass` / `Fail` / `Blocked` only) |
| **Checkbox state** | Markdown task list adjacent to MVT ID |
| **Governing links** | Governing work / acceptance relationship section and metadata links |
| **Evidence references** | Execution record Evidence column and Evidence references section |

Parser implementations MUST NOT treat **Waived** as an MVT Result.

## 7. Derived projection contract

ProjectConcord SHALL maintain derived state that is:

| Property | Requirement |
|---|---|
| Rebuildable | From canonical Markdown on scan/refresh |
| Non-authoritative | UI and queries MUST label derived vs canonical |
| Invalidated | After canonical save, Git HEAD change affecting docs, explicit refresh, external-change detection |
| Capable of | MVR listing, pending-QA queries, relationship navigation |

**Implementation-neutral:** ProjectConcord MUST NOT require a dedicated `.projectconcord/mvr-index/` or MVR-only JSON subsystem in this specification. Projections MAY use the general artifact index, relationship index, in-memory structures, or ADR-0004 caches when implemented.

## 8. Pending manual-QA detection

An MVR SHALL appear in **pending manual QA** when:

1. **Manual QA** = `Required`; AND
2. Any of:
   - canonical **Human execution status** is `Pending` or `In progress`; OR
   - any MVT in the obligation set (§4) has **Result** `Pending`, `Fail`, or `Blocked`.

Records with **Manual QA** = `Not required` SHALL NOT appear as pending manual QA.

A governing **waiver** SHALL NOT remove an MVR from factual execution tracking; UI MAY annotate waiver separately (§14).

## 9. Minimum presentation and ATTENTION UX

1. The project dashboard SHALL include an **ATTENTION** (or equivalent) region listing pending manual QA items ([PCON-0000](../../Architecture/PCON-0000-EDF-Project-Management-System-Architectural-Vision-and-Bootstrap-Handover.md) §37).
2. Each entry SHALL show at minimum: MVR Record ID, short title, link to governing work when known, and indication of pending MVTs.
3. A dedicated MVR detail view SHALL present the operator checklist and execution record prominently.

## 10. Human attestation workflow

1. Recording MVT **Pass**, **Fail**, or **Blocked**, or setting **Human execution status** to **Complete**, SHALL require **authorized human interaction** with explicit confirmation and **human-attestation context** ([ADR-0014](../../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)).
2. M1–M5: the active **Administrator** session plus recorded **Executor** name satisfies authorized human for attestation.
3. The workflow SHALL show a preview of canonical Markdown changes before save.
4. Saves SHALL use **surgical write-back** to Identity metadata, execution record rows, evidence fields, optional Notes, and representational checkboxes (§11).

## 11. MVT Result authority and checkbox synchronization

1. **MVT Result** in the execution record is **authoritative** for governance.
2. Checkbox state is a **human-usability aid** only.
3. On **authorized human result save**, ProjectConcord SHALL perform **representational synchronization**:
   - if **Result** = `Pass`, checkbox MAY be set checked (`- [x]`);
   - if **Result** ≠ `Pass`, checkbox MAY be set unchecked (`- [ ]`).
4. Representational checkbox synchronization is **not** an independent verification transition and MUST NOT be exposed as “execute test” without attestation.
5. If checkbox and Result disagree on parse, **Result governs**; ProjectConcord SHALL surface a **discrepancy** until reconciled.
6. ProjectConcord MUST NOT infer **Pass** from a checked checkbox alone.

## 12. Human execution status

1. **Human execution status** in Identity metadata is **canonical MVR state**, not a replaceable derived cache field.
2. **HumanExecutionStatusValidator** (or equivalent) SHALL deterministically evaluate whether canonical status is **consistent** with MVT Results and MVR-0001 rules.
3. **HumanExecutionStatusEvaluator** (optional) MAY **recommend** `Pending` / `In progress` / `Complete`; it MUST NOT silently overwrite canonical status.
4. Transition to **Complete** SHALL occur only through the human attestation workflow (§10) when consistency rules pass.
5. **Complete** means required human execution actually occurred with outcomes satisfying the verification obligation — typically MVT **Pass** where applicable.
6. MVT Results **constrain** validity of **Complete** but do **not replace** the canonical Human execution status field.
7. A governance **waiver** MUST NOT manufacture **Complete** or MVT **Pass**.

## 13. Evidence references

1. Evidence SHALL be recorded as links in the execution record **Evidence** column and/or **Evidence references** section.
2. ProjectConcord MAY warn when linked paths are missing; warnings MUST NOT block attestation unless project policy requires it later.

## 14. Governing work and waiver separation

1. ProjectConcord SHALL parse governing-work relationships from the MVR and backlinks from planning/gate artifacts where present.
2. ProjectConcord SHALL NOT represent governing acceptance as fully satisfied while linked required manual QA remains pending, except when the **governing record** carries an explicit **waiver** (display only — do not write MVT Pass).
3. **GDO** MUST NOT substitute for human MVT execution.
4. **GMFP** validation success MUST NOT imply human manual QA **Complete** for a separate MVR obligation.
5. Later releases MAY show **read-only** blocking hints on gate/AAR views; they MUST NOT auto-mutate EGR checkbox or gate state.

## 15. Human-Attestation Authority Boundary

Normative requirements match [ADR-0014](../../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md):

- Protected transitions: MVT Result Pass/Fail/Blocked; Human execution status Complete.
- AI MUST NOT independently assert protected transitions.
- Final state-changing commands MUST originate from authorized human interaction with attestation context.

## 16. Phased delivery

| Phase | Milestone | Capability |
|---|---|---|
| **S1** | M2 | Discovery, parser, identity validation, unit tests |
| **S2** | M4 | Navigation to MVR and governing backlinks |
| **S3** | M5 | ATTENTION, detail view, attestation write-back, discrepancy surfacing |

M1 solution skeleton MUST NOT include speculative empty MVR domain types unless a concrete M1 contract requires them.

## 17. Acceptance criteria (S3 minimum)

- [ ] Engine discovers MVR candidates and registers MVRs only when canonical Record ID is valid and consistent.
- [ ] Parser extracts Manual QA, Human execution status, MVT IDs, Results, and governing links deterministically from template-conformant fixtures.
- [ ] Pending manual QA query matches §8 for fixture repositories.
- [ ] Human attestation records MVT Result with Executor and updates canonical Markdown; AI proposal path cannot set Pass/Complete without attestation dialog.
- [ ] Invalid **Complete** (MVT Results inconsistent) is blocked at save.
- [ ] Checkbox representational sync follows §11 on attested save; discrepancy shown when file disagrees with Result.
- [ ] Waiver on governing record does not change MVT Result or Human execution status in MVR.
- [ ] ATTENTION lists pending items without requiring user to open architecture or implementation-plan documents.

## 18. ProjectConcord execution MVR

Governed acceptance of the S3 capability SHALL reference a real `docs/Verification/Records/MVR-NNNN-*.md` instance created at **S3 completion** or PA-directed acceptance — **not** during S0 bootstrap (STOP-2).
