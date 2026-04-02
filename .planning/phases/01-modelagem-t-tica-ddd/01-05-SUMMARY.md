---
phase: 01
plan: 05
title: "Threat Model STRIDE — DFD e Tabela"
subsystem: security
tags: [threat-model, stride, dfd, security, documentation]
completed: 2026-04-02

dependency_graph:
  requires: ["01-01 (estrutura de diretórios)"]
  provides: ["docs/threat-model/overview.md", "docs/threat-model/dfd-nivel-1.md", "docs/threat-model/stride-table.md"]
  affects: ["Phase 2 (walking skeleton)", "Phase 5 (adaptadores de rede)", "Phase 8 (reprodutor externo)", "Phase 9 (identidade)", "Phase 11 (pentest)"]

tech_stack:
  added: []
  patterns: ["STRIDE threat modeling", "DFD Level 0/1", "Anti-SSRF DNS validation", "Argon2id password hashing", "DPAPI/libsecret credential storage", "DtdProcessing.Prohibit (XXE prevention)"]

key_files:
  created:
    - "docs/threat-model/overview.md"
    - "docs/threat-model/dfd-nivel-1.md"
    - "docs/threat-model/stride-table.md"
  modified: []

key_decisions:
  - "16 threats documented (T-01 to T-16) covering all 6 STRIDE categories — exceeds the 15-threat minimum"
  - "Anti-SSRF validated post-DNS-resolution (not just hostname) — prevents DNS rebinding bypass"
  - "Admin area returns NotFound (not Forbidden) for non-admins — prevents information disclosure about existence (T-12)"
  - "HistoricoAcao append-only by design — enables repudiation defense (T-08)"

metrics:
  duration: "~10 minutes"
  tasks_completed: 2
  files_created: 3
  files_modified: 0
---

# Phase 01 Plan 05: Threat Model STRIDE — DFD e Tabela Summary

**One-liner:** STRIDE threat model with DFD Level 0/1 Mermaid diagrams, 16 threats across all categories, and concrete mitigations (Argon2id, anti-SSRF DNS validation, DtdProcessing.Prohibit, DPAPI) with pentest traceability.

## What Was Built

Three threat model documents created in `docs/threat-model/` before any network or persistence code is written (SEG-01 compliance):

### overview.md
- DFD Level 0 Mermaid diagram showing the complete system and all 5 attack surfaces
- Table of 5 attack surfaces with trust boundaries
- Desktop-specific security characteristics (PostgreSQL port 15432, DPAPI/libsecret, ZeroMemory, code signing)
- References to ADR-005 and Padrões Técnicos v4

### dfd-nivel-1.md
- 5 subsystems with individual Mermaid flowcharts:
  1. **Authentication** — Argon2id, FixedTimeEquals, ZeroMemory, byte[] credentials
  2. **Database** — EF Core parametrized queries, DPAPI/libsecret, usuarioId invariant, PaginatedList
  3. **Network Adapters** — DtdProcessing.Prohibit (XXE), anti-SSRF DNS validation with full IP range list, 5MB limit, 10s timeout, circuit breaker
  4. **External Reproducer** — Process.Start protocol whitelist (https, http, file only)
  5. **File Import** — size check → checksum → parse with depth limit (JSON bomb prevention)
- Trust boundaries explicitly stated per subsystem
- Anti-SSRF algorithm documented (post-DNS resolution validation)

### stride-table.md
- 16 threats (T-01 to T-16) covering all 6 STRIDE categories:
  - **S** Spoofing: T-01 (session replay), T-02 (auth bypass via DI injection)
  - **T** Tampering: T-03 (SQLi), T-04 (JSON bomb), T-05 (XXE), T-06 (SSRF), T-07 (Process.Start injection)
  - **R** Repudiation: T-08 (HistoricoAcao append-only)
  - **I** Information Disclosure: T-09 (cross-user leak), T-10 (plaintext credentials), T-11 (PII in logs), T-12 (admin area discovery)
  - **D** Denial of Service: T-13 (giant RSS payload), T-14 (unpaginated queries)
  - **E** Elevation of Privilege: T-15 (RBAC bypass), T-16 (binary tampering)
- Traceability matrix (13 mitigations → implementation phase → verification method)
- Pentest checklist for Phase 11 (SEG-05) with 10 specific test scenarios

## Success Criteria Verification

- [x] `ls docs/threat-model/*.md | wc -l` → 3 ✓
- [x] `grep -c "| T-" docs/threat-model/stride-table.md` → 16 (≥15) ✓
- [x] `overview.md` contains Mermaid diagram with 5 attack surfaces ✓
- [x] `dfd-nivel-1.md` covers all 5 subsystems ✓
- [x] `stride-table.md` covers all 6 STRIDE categories ✓
- [x] `stride-table.md` contains SSRF mitigations (T-06, DNS validation section) ✓
- [x] `stride-table.md` contains Argon2id ✓

## Deviations from Plan

### Auto-extensions (within scope)

**1. T-16 added — Binary tampering / Elevation of Privilege**
- **Found during:** Task 5.2 (stride-table creation)
- **Issue:** The plan specified ≥15 threats; T-15 ended Elevation of Privilege but the RBAC bypass and binary tampering are distinct threat vectors
- **Decision:** Added T-16 (code signing + SHA-256 verification) for completeness — referenced in overview.md desktop security section
- **Files modified:** `docs/threat-model/stride-table.md`

**2. Anti-SSRF algorithm documented explicitly in dfd-nivel-1.md**
- **Found during:** Task 5.2
- **Issue:** RESEARCH.md and technical-standards.md both emphasize DNS-rebinding bypass risk
- **Addition:** Explicit algorithm block showing the 4-step post-DNS-resolution validation in Subsistema 3
- **Files modified:** `docs/threat-model/dfd-nivel-1.md`

## Known Stubs

None — this is a documentation plan. All content is substantive and complete. No placeholder text.

## Commits

| Task | Commit | Description |
|---|---|---|
| 5.1 — overview.md | `de30d44` | DFD nível 0 with 5 attack surfaces Mermaid diagram |
| 5.2 — dfd-nivel-1.md + stride-table.md | `f3cce95` | DFD nível 1 (5 subsystems) + STRIDE table (16 threats) |
