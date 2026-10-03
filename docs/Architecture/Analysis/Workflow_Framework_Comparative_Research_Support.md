[Home](../../../README.md) › [Project Index](../../../PROJECT_INDEX.md) › [Architecture](../README.md) › Analysis › Workflow Framework Research Support

# Workflow Framework — Comparative Research Support

## Document Metadata

| Field | Value |
|---|---|
| **Document Type** | Architecture analysis / research support |
| **Normative** | **No** — informs [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md); does not define ProjectConcord semantics |
| **Status** | Active (2026-10-03) |
| **Date** | 2026-10-03 |

## Purpose

Preserve **provenance** for comparative workflow research used during the 2026-10 exploratory architecture phase. Normative conclusions are in [PCON-0005](../PCON-0005-ProjectConcord-Workflow-Framework.md) and [ADR-0024](../ADRs/ADR-0024-Governed-Engineering-Workflow-Prescribed-Workflow-Architecture.md).

## Source categories (representative)

| Domain | Primary sources (public) | Patterns extracted |
|---|---|---|
| Software PR / merge governance | [GitHub protected branches](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches); [GitLab merge request approvals](https://docs.gitlab.com/user/project/merge_requests/approvals/) | Parameterized review counts; role/code-owner gates; status-check satisfaction; fail-closed invalid policies |
| Deployment / environment | [GitLab deployment approvals](https://docs.gitlab.com/ci/environments/deployment_approvals/) | Environment-specific approval; separation of deploy vs approve |
| IT change management | [ServiceNow change types](https://github.com/ServiceNow/ServiceNowDocs/blob/australia/markdown/it-service-management/change-management/change-types.md); [Micro Focus emergency change ST 2.4](https://docs.microfocus.com/SM/9.61/Hybrid/Content/BestPracticesGuide_PD/ChMBestPractices/Emergency_Change_process_ST2.4.htm) | Standard vs normal vs emergency **models**; ECAB; PIR; recategorization |
| Regulated software | [IEC 62304](https://www.iso.org/standard/38421.html); [FAA AC 20-115D / DO-178C](https://www.faa.gov/documentLibrary/media/Advisory_Circular/AC_20-115D.pdf) | Risk class scales rigor; independence; objectives-based assurance |
| Security SDLC | [NIST SP 800-218](https://csrc.nist.gov/pubs/sp/800/218/final) | Risk-based practice subset; outcome-focused |
| Supply chain | [SLSA build levels](https://slsa.dev/spec/v1.1/levels) | Tiered evidence/provenance depth |
| Configuration management | [ISO 10007:2017](https://www.iso.org/standard/70400.html) | Baselines; controlled change evaluation; disposition authority |

## Architectural use

Research informed **invariant vs bounded vs policy-derived** classification, **workflow vs profile** identity rubric, and **YAGNI** exclusions. Vendor feature lists were **not** copied into normative architecture.

## Parent

- [Architecture README](../README.md)
