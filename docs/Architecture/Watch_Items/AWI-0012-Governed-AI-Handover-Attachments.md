# AWI-0012 — Governed AI Handover Attachments

[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › [Watch Items](README.md) › AWI-0012

| | |
|---|---|
| **Status** | Active — **deferred**, **capture-only** |
| **Disposition** | **NOT architectural acceptance** — **NOT implementation authorization** |
| **Owner** | ProjectConcord |
| **Created** | 2026-10-07 |
| **Revisit Trigger** | Before any attachment schema, persistence, transport, or operator UX tranche for governed PA/Engineering Agent exchanges; PA request to promote to ADR/specification; material amendment to [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) relay package model |
| **Discovery source** | MVT-5 PA-response recovery remediation context (2026-10-07); ordinary real-world need for supporting artifacts beyond package/text handovers; PA handover `d85f48db` |
| **Related ADRs** | [ADR-0025](../ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md) (governed interaction boundaries); [ADR-0021](../ADRs/ADR-0021-Engineering-Agent-Provider-Plugin-Contract.md) (provider neutrality — **no** attachment API selection here) |
| **Related specs** | [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md) (current **text/package** relay — **does not** claim first-class governed attachments) |
| **Cross-reference** | [AWI-0011](AWI-0011-Governed-Interactive-Engineering-Session.md) (GIES — session artifacts and evidence; avoid overlapping mechanisms); [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md) (orchestration and external AI integration); [AWI-0006](AWI-0006-PAR-Cursor-Bridge-Transport.md) (automated transport — **closed/satisfied**; future attachment transport must remain provider-neutral); [AWI-0008](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md) (operator presentation — illustrative only) |

---

## Objective

Capture **deferred architectural investigation** for **first-class governed file attachments** associated with ProjectConcord **Project Architect** and **Engineering Agent** handovers and relay exchanges.

The current governed relay is fundamentally **package/text** based ([SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)). Operators may attach files using a **provider's native UI** (chat upload, drag-and-drop, etc.), but that is **not** equivalent to a **governed ProjectConcord attachment** with identity, integrity, provenance, and fail-closed semantics.

This watch item records **open questions** for a future **provider-neutral** investigation. It does **not** select representations, algorithms, persistence, package kinds, or provider APIs.

## Central architectural question

How should ProjectConcord **represent**, **govern**, **transport or reference**, **validate**, **present**, and **preserve** files that form part of a governed AI exchange — for both **inbound** (ProjectConcord / human → PA or Engineering Agent) and **outbound** (PA or Engineering Agent → ProjectConcord / human) directions?

## Current baseline (explicit non-claims)

While **Active** and **capture-only**:

- Published relay handovers (`paReviewExport`, `paHandoverImport`, Engineering Agent handover/result packages, etc.) are **text/package** artifacts as implemented.
- **Provider-native manual attachment** (e.g. attaching a file in a chat UI) is **outside** ProjectConcord governance unless and until a future architecture defines governed attachment semantics.
- **No** attachment schema, **no** new `GovernedPackageKind`, **no** persistence design, and **no** implementation are authorized by this AWI.

## Scope and non-goals

This watch item:

- **Does not** authorize `src/` changes, prototypes, upload APIs, or relay contract changes.
- **Does not** design specifically for ChatGPT attachments, Cursor attachments, or any single provider transport.
- **Does not** decide whether attachments are embedded in packages, sidecar governed artifacts, or references — investigation only.
- **Does not** confer governance authority merely because a file is present or attached.
- **Does not** reopen accepted MVT-5 recovery-loop architecture or PA handover text semantics.

## Investigation themes (deferred — no decisions)

The future investigation **should** address at least the following. Examples are **exploratory**; final taxonomies are **not** chosen here.

### 1. Attachment identity

How is an attachment uniquely identified? Consider (non-exhaustive): attachment identifier, filename, media/MIME type, size, cryptographic content hash, version/revision identity.

### 2. Handover relationship

How is an attachment associated with a governed package, handover, PA review, Engineering Agent request/response, correlation/session, or workflow instance? Are attachments **part of** the package or **separately governed** artifacts **referenced** by it?

### 3. Purpose / role

Should the handover describe **why** an attachment exists (e.g. required input, supporting evidence, reference material, diagnostic evidence, generated output, human-observation evidence)?

### 4. Required vs optional

Distinguish attachments **required** for the receiving AI to perform the requested work from **supplementary/optional** attachments.

### 5. Integrity

How does ProjectConcord establish that the file received or referenced by the AI is the file intended by the governed handover (e.g. content identity / hashing — **algorithm not prescribed**)?

### 6. Provenance

What provenance must be retained: supplier (PA, EA, ProjectConcord, human), time of entry into the exchange, referencing package, versions/replacements?

### 7. Receiver access / acknowledgement

ProjectConcord may know an operator **attempted** to attach a file without knowing whether the receiving AI **received** or could **access** it. Investigate whether governed exchange needs **acknowledgement** or other evidence of receiver access. **Do not** silently assume provider-native upload implies AI access.

### 8. Human-in-the-loop transport

How do attachments work when the human is transport (download/export, attach to AI conversation, return AI-generated files, select local artifacts) without understanding relay mechanics?

### 9. Automated transport

Future compatibility with transports that send files directly; model **must not** depend on human transport only.

### 10. Local files vs copied content

When should ProjectConcord govern an actual file, embed textual content, reference a repository artifact, reference a durable ProjectConcord artifact, or use an external/provider-native reference — avoiding unnecessary duplication of large blobs.

### 11. Security and trust boundaries

Investigate at minimum: untrusted file content; misleading filenames; MIME/type disagreement; path traversal; oversized files; executable content; provider-generated files; replacement after handover creation; prompt/instruction injection in attached documents. Attachment presence **must not** confer governance authority.

### 12. Governance semantics

Determine whether an attachment is part of the governed package, governed evidence associated with the package, presentation/transport material, or another category — **future PA/architecture decision**.

### 13. Fail-closed behavior

Expected behavior when a **required** attachment is missing, changed, corrupt, inaccessible, ambiguous, unsupported by the selected provider, or not acknowledged by the receiver. **Do not** silently treat required missing attachments as successfully delivered.

### 14. Large artifacts

Avoid forcing large logs/documents/files into textual handover payloads merely to preserve governance.

### 15. Output attachments

Cover **both directions**: ProjectConcord/human → PA/EA and PA/EA → ProjectConcord/human (reports, logs, patches, archives, diagrams, test artifacts, etc.).

### 16. PA and Engineering Agent parity

Investigate attachment support for **both** Project Architect exchanges and Engineering Agent exchanges — not EA-only.

### 17. Persistence / recovery

What must survive application restart, workflow pause/resume, relay recovery, provider/session loss, and operator continuation (where applicable)?

### 18. UI / operator experience (illustrative)

Future architecture may consider operator flows such as Handover → Attachments (file, purpose, required/optional, delivery/access status). **Not** an approved UI design.

### 19. Governed Interactive Engineering Session relationship

Relate to [AWI-0011](AWI-0011-Governed-Interactive-Engineering-Session.md): interactive sessions may need attachments as question context, human verification evidence, generated results, PA decision evidence, or resumable session artifacts — **avoid duplicate mechanisms** where reuse is appropriate.

### 20. Existing project artifact / knowledge relationship

Investigate reuse of repository files, governed artifacts, evidence, provenance, and knowledge/document references rather than inventing parallel primitives.

## Promotion path

```text
Deferred capture (this AWI)
    -> PA-governed architecture investigation
    -> Proposed ADR(s) / SPEC amendment(s) (separate)
    -> Implementation tranches (separately authorized)
```

## Parent

- [Watch Items](README.md)

## Related Documents

- [SPEC-006](../../Specifications/features/SPEC-006-par-project-root-and-governed-workflow-relay.md)
- [ADR-0025](../ADRs/ADR-0025-Governed-Interaction-Operational-Boundaries-and-Layer-Responsibilities.md)
- [AWI-0011](AWI-0011-Governed-Interactive-Engineering-Session.md)
- [AWI-0003](AWI-0003-Primary-Orchestration-and-External-AI-Engineering-Tool-Integration.md)
- [AWI-0008](AWI-0008-Governed-Operator-Experience-and-Product-Shell.md)
