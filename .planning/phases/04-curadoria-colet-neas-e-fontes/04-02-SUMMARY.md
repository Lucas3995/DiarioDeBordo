---
phase: 04-curadoria-colet-neas-e-fontes
plan: 02
subsystem: Infrastructure
tags: [ef-core, repositories, query-services, cycle-detection, deduplication, integration-tests]
dependency_graph:
  requires: [04-01]
  provides: [coletanea-persistence, cycle-detection, deduplication-service]
  affects: [module-acervo]
tech_stack:
  added: [testcontainers, bfs-cycle-detection]
  patterns: [cqrs, repository-pattern, two-level-deduplication]
key_files:
  created:
    - tests/Tests.Integration/Repositorios/ColetaneaRepositoryTests.cs
  modified:
    - src/DiarioDeBordo.Infrastructure/Persistencia/Configuracoes/ConteudoColetaneaConfiguration.cs
    - src/DiarioDeBordo.Infrastructure/Persistencia/Migrations/20260407135504_AddConteudoColetanea.cs
    - src/DiarioDeBordo.Infrastructure/Repositorios/ColetaneaRepository.cs
    - src/DiarioDeBordo.Infrastructure/Consultas/ColetaneaQueryService.cs
    - src/DiarioDeBordo.Infrastructure/Consultas/DeduplicacaoService.cs
    - src/DiarioDeBordo.Infrastructure/Consultas/ConteudoQueryService.cs
    - src/DiarioDeBordo.Infrastructure/DependencyInjection.cs
decisions:
  - "BFS cycle detection: O(V+E) traversal with HashSet visited tracking"
  - "Two-level deduplication: HIGH confidence via URL match, MEDIUM via normalized title"
  - "SEG-02 enforced at BFS entry: validate starting collection belongs to user before traversal"
  - "Composite PK (coletanea_id, conteudo_id) with Cascade on collection FK, Restrict on item FK"
metrics:
  duration_minutes: 6
  tasks_completed: 2
  files_modified: 8
  tests_added: 10
  completed: 2026-04-07
---

# Phase 04 Plan 02: Persistence Layer for Collections Summary

EF Core configuration, migration, repositories, query services with BFS cycle detection, two-level deduplication, and integration tests against real PostgreSQL.

## What Was Built

**Infrastructure persistence and query layer for Phase 4 collections:**

1. **ConteudoColetaneaConfiguration**: EF Core mapping with composite PK, Cascade/Restrict FK behaviors, no navigation properties (per Pitfall 1)
2. **AddConteudoColetanea migration**: Creates `conteudo_coletanea` table with proper constraints and indexes
3. **ColetaneaRepository**: Full CRUD for collections + BFS cycle detection via `ObterDescendentesAsync` with SEG-02 validation
4. **DeduplicacaoService**: Two-level duplicate detection (HIGH: URL match, MEDIUM: normalized title with diacritics/punctuation removal)
5. **ColetaneaQueryService**: Paginated item listing with sub-item counts, collection detail with progress calculation
6. **ConteudoQueryService enrichment**: PapelConteudo filtering, QuantidadeItens, ProgressoPercentual, ImagemCapaCaminho for collections
7. **Integration tests**: 10 tests validating cycle detection, item management, security boundaries against real PostgreSQL via Testcontainers

## Technical Decisions

**BFS Cycle Detection**: `ObterDescendentesAsync` uses breadth-first search with `HashSet<Guid>` for O(V+E) traversal. Filters children by `PapelConteudo.Coletanea` and `usuarioId` to scope to user-owned sub-collections only.

**SEG-02 Validation**: Added upfront check that starting collection belongs to querying user before BFS traversal. Returns empty list if unauthorized, preventing information leakage.

**Two-Level Deduplication**: HIGH confidence = exact URL match in any fonte (fast, authoritative). MEDIUM confidence = normalized title match after removing diacritics, punctuation, collapsing whitespace (slower, less certain). Uses `EF.Functions.ILike` prefix filter to limit DB result set before in-memory normalization.

**Delete Behaviors**: Collection FK uses `Cascade` (deleting collection removes associations), Item FK uses `Restrict` (items not auto-deleted when collection deleted, per D-12 Pitfall 2).

**No Navigation Properties**: ConteudoColetanea has no `Conteudo` nav properties to avoid EF Core confusion with self-referencing many-to-many (Pitfall 1).

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Missing SEG-02 validation in ObterDescendentesAsync**
- **Found during:** Task 2 - Integration test `Given_ColetaneaOutroUsuario_When_ObterDescendentes_Then_RetornaVazio` failed
- **Issue:** Method allowed querying descendants of collections owned by other users, violating SEG-02 security requirement
- **Fix:** Added `pertenceAoUsuario` check at method entry - returns empty list if starting collection doesn't belong to querying user
- **Files modified:** `src/DiarioDeBordo.Infrastructure/Repositorios/ColetaneaRepository.cs`
- **Commit:** 96909e7

## Verification Results

✅ All success criteria met:
- `dotnet build DiarioDeBordo.sln`: 0 warnings, 0 errors
- `dotnet test tests/Tests.Integration --filter "FullyQualifiedName~ColetaneaRepository"`: 10/10 tests passed
- Migration creates `conteudo_coletanea` table with composite PK `(coletanea_id, conteudo_id)` and correct FK constraints
- ConteudoColetanea entity has no navigation properties
- BFS cycle detection correctly identifies chains (A→B→C returns {B, C}) and potential cycles
- DeduplicacaoService detects duplicates by URL and normalized title
- ConteudoQueryService enriched with collection-specific data (QuantidadeItens, ProgressoPercentual, ImagemCapaCaminho)
- All services registered in DI container

## Test Coverage

**Integration Tests (Tests.Integration/Repositorios/ColetaneaRepositoryTests.cs):**
- ✅ `Given_CadeiaABC_When_ObterDescendentesA_Then_RetornaBEC`: BFS traversal correctness
- ✅ `Given_CicloABC_When_ObterDescendentesC_Then_RetornaConjuntoComA`: Cycle detection
- ✅ `Given_ColetaneaVazia_When_ObterDescendentes_Then_RetornaListaVazia`: Empty collection edge case
- ✅ `Given_ColetaneaCom3Itens_When_ObterProximaPosicao_Then_Retorna4`: Position calculation
- ✅ `Given_ColetaneaVazia_When_ObterProximaPosicao_Then_Retorna1`: Empty collection position
- ✅ `Given_ItemAdicionado_When_ObterItem_Then_RetornaTodosOsCampos`: Item roundtrip with all fields
- ✅ `Given_ItemRemovido_When_ObterItem_Then_RetornaNull`: Item removal
- ✅ `Given_ItemExisteNaColetanea_When_ItemExiste_Then_RetornaTrue`: Existence check true
- ✅ `Given_ItemNaoExiste_When_ItemExiste_Then_RetornaFalse`: Existence check false
- ✅ `Given_ColetaneaOutroUsuario_When_ObterDescendentes_Then_RetornaVazio`: SEG-02 security boundary

**All tests use Testcontainers.PostgreSql for real database validation.**

## Known Stubs

None. All functionality is fully wired with no placeholder data or mock implementations.

## Next Steps

Plan 04-03 will implement MediatR command/query handlers that consume these repositories and services. Handlers will enforce business rules (cycle prevention before adding items, duplicate detection before creating content, etc.).

---

**Duration:** 6 minutes
**Completed:** 2026-04-07
**Commits:**
- b4f4f09: feat(04-02): EF Core config, migration, repository and query services
- 96909e7: fix(04-02): add SEG-02 validation in ObterDescendentesAsync
