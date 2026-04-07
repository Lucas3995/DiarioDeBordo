---
phase: 4
slug: curadoria-colet-neas-e-fontes
status: draft
nyquist_compliant: false
wave_0_complete: false
created: 2026-04-06
---

# Phase 4 — Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | xUnit + NSubstitute 5.3.0 + Testcontainers.PostgreSql 4.11.0 |
| **Config file** | `tests/Tests.Domain/Tests.Domain.csproj`, `tests/Tests.Integration/Tests.Integration.csproj` |
| **Quick run command** | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Coletanea" -x` |
| **Full suite command** | `dotnet test` |
| **Estimated runtime** | ~5 seconds (quick) / ~30 seconds + container startup (full) |

---

## Sampling Rate

- **After every task commit:** Run `dotnet test tests/Tests.Domain/ -x`
- **After every plan wave:** Run `dotnet test`
- **Before `/gsd:verify-work`:** Full suite must be green
- **Max feedback latency:** 5 seconds (quick) / 30 seconds (full)

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|-----------|-------------------|-------------|--------|
| 4-W0-01 | Wave 0 | 0 | ACE-06, ACE-07, ACE-08 | unit | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~ConteudoColetanea" -x` | ❌ W0 | ⬜ pending |
| 4-W0-02 | Wave 0 | 0 | ACE-10 | unit | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Deduplicacao" -x` | ❌ W0 | ⬜ pending |
| 4-W0-03 | Wave 0 | 0 | ACE-05 | unit | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~FonteManagement" -x` | ❌ W0 | ⬜ pending |
| 4-W0-04 | Wave 0 | 0 | ACE-07 | integration | `dotnet test tests/Tests.Integration/ --filter "FullyQualifiedName~ColetaneaRepository" -x` | ❌ W0 | ⬜ pending |
| 4-W0-05 | Wave 0 | 0 | SEG-04 | integration | `dotnet test tests/Tests.Integration/ --filter "FullyQualifiedName~CenarioApendiceA" -x` | ❌ W0 | ⬜ pending |
| 4-01-xx | Plan 01 | 1 | ACE-06 | unit | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Coletanea" -x` | ✅ partial | ⬜ pending |
| 4-02-xx | Plan 02 | 1 | ACE-07 | integration | `dotnet test tests/Tests.Integration/ --filter "FullyQualifiedName~Ciclo" -x` | ❌ W0 | ⬜ pending |
| 4-03-xx | Plan 03 | 1 | ACE-08 | unit | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~AnotacaoContextual" -x` | ❌ W0 | ⬜ pending |
| 4-04-xx | Plan 04 | 2 | ACE-05 | unit | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Fonte" -x` | ✅ partial | ⬜ pending |
| 4-05-xx | Plan 05 | 2 | ACE-10 | unit | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Deduplicacao" -x` | ❌ W0 | ⬜ pending |
| 4-06-xx | Plan 06 | 3 | SEG-04 | integration | `dotnet test tests/Tests.Integration/ --filter "FullyQualifiedName~CenarioApendiceA" -x` | ❌ W0 | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

---

## Wave 0 Requirements

- [ ] `tests/Tests.Domain/Acervo/ConteudoColetaneaTests.cs` — stubs for ACE-06, ACE-07, ACE-08
- [ ] `tests/Tests.Domain/Acervo/DeduplicacaoTests.cs` — stubs for ACE-10
- [ ] `tests/Tests.Domain/Acervo/FonteManagementTests.cs` — stubs for ACE-05 (add/remove/reorder handlers)
- [ ] `tests/Tests.Integration/Repositorios/ColetaneaRepositoryTests.cs` — stubs for ACE-07 cycle detection
- [ ] `tests/Tests.Integration/CenarioApendiceATests.cs` — stubs for SEG-04 scenarios 1–5
- [ ] Activate skipped tests in `ColetaneaInvariantTests.cs` (I-08, I-09)

*Framework install: none needed — xUnit, NSubstitute, Testcontainers already configured.*

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| Dedup warning banner displayed on UI during content creation | ACE-10 | UI visual feedback requires visual inspection | Create a content with a URL/title that matches an existing one; verify amber banner appears with confidence level |
| Cover image renders in card thumbnail | ACE-08 | Visual rendering requires manual check | Add a cover image to content; verify 48×48 thumbnail visible in AcervoView card |
| Source reorder via arrow buttons persists after app restart | ACE-05 | Persistence across session requires manual verification | Reorder sources, close and reopen app, verify order preserved |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 30s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
