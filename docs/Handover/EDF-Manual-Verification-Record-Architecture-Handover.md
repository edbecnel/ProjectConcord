[Home](../../README.md) › [Project Index](../../PROJECT_INDEX.md) › [Handover](README.md) › EDF Manual Verification Record

# EDF Manual Verification Records — ProjectConcord Architecture Handover

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architecture handover (inbound from EDF) |
| **Normative** | No — **EDF owns MVR semantics**; ProjectConcord implements consumption and projection |
| **Status** | Active |
| **Date** | 2026-09-28 |
| **Owner** | ProjectConcord |
| **Source framework** | [Engineering Documentation Framework](https://github.com/edbecnel/Engineering-Documentation-Framework) |
| **Source commit** | `192fe5c1c6254c51e257d24aefc09e127ce72464` on `main` |

## Canonical authority statement

**EDF is canonical.** ProjectConcord **consumes and projects** MVR semantics; it **must not** redefine MVR structure, MVT Result enums, Human execution status rules, waiver law, or gate integration requirements. When EDF and this handover diverge, **EDF wins**.

**Out of scope for EDF (ProjectConcord’s job):** parser implementation, UI, derived index technology, human-attestation dialog design, operational schema — unless later authorized and aligned with EDF and [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md).

---

## 1. Why ProjectConcord needs this

When EDF-governed work requires **human-executed manual verification**, the **MVR** is the canonical executable procedure and factual execution record. Planning artifacts must link the MVR, not embed operator checklists ([MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md)).

ProjectConcord must eventually:

- discover and parse MVRs deterministically;
- surface **pending human manual QA** prominently ([MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md) downstream tooling);
- support **human attestation** write-back without AI impersonation;
- integrate with EGR/AAR/GMFP **without** a second gate system or automatic EGR mutation.

---

## 2. Authoritative EDF sources (read first)

| Artifact | Role |
|----------|------|
| [MVR-0001](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Specifications/MVR-0001-Manual-Verification-Records.md) | **Normative** MVR requirements |
| [EDF ADR-0010 (MVR)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Architecture/ADRs/ADR-0010-Manual-Verification-Records.md) | Architectural decision — **Accepted** at source commit |
| [Manual_Verification_Record_Template (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Templates/Manual_Verification_Record_Template.md) | Instance shape |
| [Verification README (EDF)](https://github.com/edbecnel/Engineering-Documentation-Framework/blob/192fe5c1c6254c51e257d24aefc09e127ce72464/docs/Verification/README.md) | Core subdomain |

**Naming collision:** ProjectConcord [ADR-0010](../Architecture/ADRs/ADR-0010-Single-User-Administrator-Default-Model.md) is **not** EDF ADR-0010. When citing MVR rules, refer to **EDF ADR-0010** and **MVR-0001**.

**ProjectConcord consumption spec:** [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md). **Attestation boundary:** [ADR-0014](../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md).

---

## 3. Location and identity

- Instance path: `docs/Verification/Records/MVR-NNNN-<short-title>.md`
- Record ID: `MVR-NNNN` in Identity metadata — **not** filename alone
- MVT IDs: local `MVT-1`, `MVT-2`, …; global reference `MVR-NNNN / MVT-n`

ProjectConcord discovery MUST NOT silently manufacture Record ID from filename ([SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) §5).

---

## 4. Execution semantics (consumption binding)

| Concept | Rule |
|---|---|
| **MVT Result** | `Pending`, `Pass`, `Fail`, `Blocked` only — **not** `Waived` |
| **Result vs checkbox** | Result authoritative; checkbox representational |
| **Human execution status** | Canonical field; Complete only via human workflow; waiver must not manufacture Complete |
| **AI** | Must not impersonate human execution ([ADR-0014](../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md)) |

ProjectConcord must not infer optional/informational MVT classes from prose without EDF-deterministic representation ([SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) §4).

---

## 5. MVR vs EGR, GDO, AAR, GMFP

| Mechanism | Relationship to MVR |
|---|---|
| **EGR** | Governing acceptance must not appear fully satisfied while linked required MVR unresolved, except explicit waiver on governing record |
| **GDO** | Does **not** substitute for human MVT execution |
| **AAR** | Complementary; AAR charter may require manual QA via MVR |
| **GMFP** | GMFP validation does **not** imply separate MVR human execution Complete |

ProjectConcord MUST NOT auto-mutate EGR gate state from MVR state. Read-only blocking hints MAY appear in later milestones ([SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) §14).

---

## 6. ProjectConcord documentation map

| Artifact | Role |
|---|---|
| [SPEC-005](../Specifications/features/SPEC-005-manual-verification-record-consumption.md) | Normative product behavior |
| [ADR-0014](../Architecture/ADRs/ADR-0014-MVR-Human-Attestation-and-AI-Boundary.md) | Human-Attestation Authority Boundary |
| [GAP-042](../Development/EDF_Gap_Register.md) | Engine consumption gap |
| [AWI-0005](../Architecture/Watch_Items/AWI-0005-Manual-Verification-Records.md) | Deferred implementation watch |
| [Implementation Roadmap](../Development/Implementation_Roadmap.md) | S0 → M2–M5 phasing |
| [Adoption plan](ProjectConcord-MVR-Adoption-Architecture-Plan.md) | Accepted architecture plan (STOP-1 closed) |

---

## 7. S0 vs implementation

**S0 (complete):** ProjectConcord `docs/Verification/` structure, templates, architecture docs — **no** ProjectConcord feature execution MVR instance.

**S1–S5:** Application implementation per roadmap — authorized only under normal milestone/gate sequence (**EGR-G1** / M2+). **No separate MVR-adoption gate.**

**STOP-2:** ProjectConcord execution MVR (`MVR-NNNN-*.md` for feature acceptance) deferred to **S3 completion** or PA-directed acceptance.

---

## 8. TRV

TRV CC-4B MVR migration is **paused** and **out of scope** for this handover.

---

## Parent

- [Handover](README.md)

## Related Documents

- [System Architecture Overview](../Architecture/System_Architecture_Overview.md)
- [EDF Gap Register](../Development/EDF_Gap_Register.md)
