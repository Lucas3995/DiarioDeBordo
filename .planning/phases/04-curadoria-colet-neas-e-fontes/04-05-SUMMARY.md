---
phase: 04-curadoria-colet-neas-e-fontes
plan: 05
subsystem: tests-integration
tags: [integration-tests, appendix-a, testcontainers, module-acervo]
dependency_graph:
  requires: [04-02, 04-03]
  provides: [appendix-a-scenarios-1-5-coverage, seg-04-validation]
  affects: [phase-04-completion]
tech_stack:
  added: []
  patterns: [xunit, testcontainers-postgresql, mediator-end-to-end, user-scoped-integration]
key_files:
  created:
    - tests/Tests.Integration/CenarioApendiceATests.cs
  modified:
    - src/DiarioDeBordo.Infrastructure/Consultas/ConteudoQueryService.cs
    - src/DiarioDeBordo.Infrastructure/Consultas/ColetaneaQueryService.cs
decisions:
  - "Scenarios 1-5 are validated through MediatR + real PostgreSQL, not repository-only tests."
  - "Owned collections (Fontes/Imagens) must be queried through Conteudo aggregate projection (SelectMany), not DbSet."
metrics:
  duration: 35 min
  tasks_completed: 1
  files_modified: 3
  completed: 2026-04-07
---

# Phase 04 Plan 05: Appendix A Scenario Integration Summary

**Integration tests cobrindo os cenários 1-5 do Apêndice A com execução real contra PostgreSQL via Testcontainers e fluxo completo de Commands/Queries.**

## Accomplishments

- Implementado `CenarioApendiceATests` com **5 testes de integração**, um por cenário do Apêndice A (1-5).
- Validado E2E por cenário com `IMediator` + handlers reais + banco PostgreSQL real:
  - Cenário 1: coletânea Miscelânea com anotações contextuais independentes da anotação global.
  - Cenário 2: músicas com fallback de fontes (local, Spotify, YouTube) e prioridade preservada.
  - Cenário 3: conteúdo sem mídia digital (`FormatoMidia.Nenhum`) com imagem de capa e progresso manual em anotação.
  - Cenário 4: plano Guiada com conteúdo heterogêneo e coletânea aninhada (`Cybersecurity`) com subitens.
  - Cenário 5: franquia Resident Evil com coletâneas filhas (Jogos/Filmes/Livros), estrutura aninhada e proteção contra ciclo.
- Corrigido bloqueio de integração em consultas de leitura para owned entities (`Fonte` e `ImagemConteudo`) usando projeção por agregado (`SelectMany`) em vez de `Set<T>()`.

## Verification

- `dotnet test tests/Tests.Integration/ --filter "FullyQualifiedName~CenarioApendiceA"`
- Resultado: **5/5 testes passando**.

## Task Commits

1. **Task 1: Implementar testes de integração dos cenários 1-5 do Apêndice A** — `9875aa1` (feat)

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Query de owned types quebrava execução real dos cenários**
- **Found during:** Task 1 verification
- **Issue:** `ConteudoQueryService` e `ColetaneaQueryService` consultavam `Fonte`/`ImagemConteudo` via `DbSet` (`Set<T>()`), mas os tipos são configured as owned; testes falhavam com `InvalidOperationException`.
- **Fix:** Reescritas queries para `SelectMany` a partir de `Conteudos`, mantendo filtro por `usuarioId`.
- **Files modified:**  
  - `src/DiarioDeBordo.Infrastructure/Consultas/ConteudoQueryService.cs`  
  - `src/DiarioDeBordo.Infrastructure/Consultas/ColetaneaQueryService.cs`
- **Commit:** `9875aa1`

## Authentication Gates

None.

## Known Stubs

None.


## Self-Check: PASSED

- FOUND: `.planning/phases/04-curadoria-colet-neas-e-fontes/04-05-SUMMARY.md`
- FOUND: commit `9875aa1`
