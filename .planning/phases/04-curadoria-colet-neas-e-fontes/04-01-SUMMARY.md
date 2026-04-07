---
phase: 04-curadoria-colet-neas-e-fontes
plan: 01
subsystem: acervo-core
tags: [domain, contracts, tdd, wave-0]
dependency-graph:
  requires: [02-walking-skeleton]
  provides: [ConteudoColetanea, IColetaneaQueryService, IDeduplicacaoService, FonteManagement]
  affects: [04-02, 04-03, 04-04, 04-05]
tech-stack:
  added: []
  patterns: [join-entity-with-payload, aggregate-mutations, cqrs-query-interfaces]
key-files:
  created:
    - src/DiarioDeBordo.Core/Entidades/ConteudoColetanea.cs
    - src/DiarioDeBordo.Core/Consultas/IColetaneaQueryService.cs
    - src/DiarioDeBordo.Core/Consultas/IDeduplicacaoService.cs
    - tests/Tests.Domain/Acervo/ConteudoColetaneaTests.cs
    - tests/Tests.Domain/Acervo/FonteManagementTests.cs
    - tests/Tests.Domain/Acervo/DeduplicacaoTests.cs
  modified:
    - src/DiarioDeBordo.Core/Entidades/Conteudo.cs
    - src/DiarioDeBordo.Core/Entidades/Fonte.cs
    - src/DiarioDeBordo.Core/Enums/Enums.cs
    - src/DiarioDeBordo.Core/FeatureFlags/IFeatureFlags.cs
    - src/DiarioDeBordo.Core/Consultas/IConteudoQueryService.cs
    - src/DiarioDeBordo.Core/Repositorios/IColetaneaRepository.cs
    - src/DiarioDeBordo.Infrastructure/Consultas/ConteudoQueryService.cs
    - tests/Tests.Domain/Acervo/ColetaneaInvariantTests.cs
decisions:
  - Fonte.Prioridade changed from init to set for ReordenarFontes mutation
  - NivelConfiancaDuplicata uses uppercase normalization (CA1308 compliance)
  - ConteudoDetalheData enriched with Fontes/Imagens lists directly
  - IConteudoQueryService.ListarAsync now accepts PapelConteudo? filter
metrics:
  duration: 9m
  completed: 2026-04-07
---

# Phase 04 Plan 01: Domain Contracts Summary

ConteudoColetanea join entity, Conteudo aggregate mutations, and CQRS query interfaces for Phase 4 curadoria.

## What Was Built

### Task 1: Domain Entities and Query Interfaces
- **ConteudoColetanea** join entity with composite PK (ColetaneaId, ConteudoId), Posicao, AnotacaoContextual, AdicionadoEm
- **Conteudo aggregate mutations**:
  - `RemoverFonte(Guid fonteId)` — removes fonte, throws if not found
  - `ReordenarFontes(IReadOnlyList<Guid> fonteIdsOrdenados)` — reassigns priorities 1..N
  - `RemoverImagem(Guid imagemId)` — removes imagem, throws if not found
- **Fonte.Prioridade** changed from `init` to `set` for reordering support
- **IColetaneaQueryService** — read-side queries for collections (ListarItensAsync, ObterDetalheAsync)
- **IDeduplicacaoService** — deduplication detection contract (VerificarAsync with Alta/Media confidence)
- **ConteudoResumoData** enriched with TipoColetanea?, QuantidadeItens, ProgressoPercentual, ImagemCapaCaminho
- **ConteudoDetalheData** enriched with TipoColetanea?, Fontes, Imagens lists
- **IConteudoQueryService.ListarAsync** now accepts `PapelConteudo? papelFiltro`
- **IColetaneaRepository** extended with item management methods (AdicionarItemAsync, RemoverItemAsync, etc.)
- **Feature flags** enabled: coletaneas, fontes_com_fallback, deduplicacao

### Task 2: Wave 0 Test Stubs
- **ConteudoColetaneaTests** — 3 tests for entity construction and mutation
- **FonteManagementTests** — 9 tests for fonte/imagem removal and reordering
- **DeduplicacaoTests** — 7 theory cases for title normalization + 3 skipped service stubs
- **ColetaneaInvariantTests** — I-08/I-09 tests activated with ConteudoColetanea assertions

## Commits

| Hash | Message |
|------|---------|
| 387ba9f | feat(04-01): add Phase 4 domain contracts and enrich DTOs |
| 5a84725 | test(04-01): add Wave 0 test stubs for Phase 4 domain behaviors |

## Test Results

```
Domain Tests: 173 total (170 passed, 3 skipped)
- Skipped: 3 DeduplicacaoService stubs (deferred to Plan 02)
- New: 23 tests added for Phase 4 behaviors
```

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] CA1308 compliance for title normalization**
- **Found during:** Task 2 - DeduplicacaoTests
- **Issue:** ToLowerInvariant() triggers CA1308 warning (prefer ToUpperInvariant for consistency)
- **Fix:** Changed to ToUpperInvariant() and updated expected test values
- **Files modified:** tests/Tests.Domain/Acervo/DeduplicacaoTests.cs

**2. [Rule 3 - Blocking] Breaking interface changes required downstream updates**
- **Found during:** Task 1 - interface enrichment
- **Issue:** Adding papelFiltro parameter and new record fields broke existing callers
- **Fix:** Updated ConteudoQueryService implementation, Module.Acervo handlers, and test mocks
- **Files modified:** 6 files in Infrastructure, Module.Acervo, and Tests.Domain

## Architecture Notes

- **ConteudoColetanea** has NO navigation properties to Conteudo (per RESEARCH.md Pitfall 1)
- **Join entity pattern**: composite PK stored as two Guid columns, payload fields (Posicao, AnotacaoContextual) attached
- **CQRS maintained**: IColetaneaQueryService for reads, IColetaneaRepository for writes
- **Feature flags** now return true for Phase 4 features — guard logic intact, gates open

## Self-Check: PASSED

- [x] src/DiarioDeBordo.Core/Entidades/ConteudoColetanea.cs exists
- [x] src/DiarioDeBordo.Core/Consultas/IColetaneaQueryService.cs exists
- [x] src/DiarioDeBordo.Core/Consultas/IDeduplicacaoService.cs exists
- [x] tests/Tests.Domain/Acervo/ConteudoColetaneaTests.cs exists
- [x] tests/Tests.Domain/Acervo/FonteManagementTests.cs exists
- [x] tests/Tests.Domain/Acervo/DeduplicacaoTests.cs exists
- [x] Commit 387ba9f exists (Task 1)
- [x] Commit 5a84725 exists (Task 2)
- [x] Solution builds with 0 warnings
- [x] All domain tests pass (170/170 + 3 skipped)
