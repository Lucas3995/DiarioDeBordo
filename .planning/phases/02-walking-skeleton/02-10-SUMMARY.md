---
phase: "02"
plan: "10"
subsystem: "tests-integration-e2e"
tags: [integration-tests, e2e, testcontainers, postgresql, walking-skeleton, sc1, sc2]
dependency_graph:
  requires: ["02-05", "02-06", "02-07"]
  provides: ["SC1-proof", "SC2-proof", "I-12-proof", "migration-verification"]
  affects: ["ci-pipeline", "dev-workflow"]
tech_stack:
  added: ["Testcontainers.PostgreSql 4.11.0", "xunit IAsyncLifetime"]
  patterns: ["PostgresTestBase reusable base class", "IAsyncLifetime per-class container lifecycle"]
key_files:
  created:
    - tests/Tests.Integration/Infrastructure/PostgresTestBase.cs
    - tests/Tests.Integration/Migrations/MigrationTests.cs
    - tests/Tests.Integration/Repositorios/ConteudoRepositoryTests.cs
    - tests/Tests.Integration/Repositorios/CategoriaRepositoryTests.cs
    - tests/Tests.E2E/WalkingSkeletonTests.cs
  modified:
    - src/DiarioDeBordo.Infrastructure/DiarioDeBordo.Infrastructure.csproj
    - tests/Tests.Integration/Tests.Integration.csproj
    - tests/Tests.E2E/Tests.E2E.csproj
decisions:
  - "InternalsVisibleTo added to Infrastructure.csproj for Tests.Integration and Tests.E2E assembly names"
  - "CA1707/CA2007/CS0618 suppressed in test projects: xunit naming convention allows underscores; ConfigureAwait not needed in tests; Testcontainers builder API in transition"
  - "AddInfrastructure() used in E2E tests (not manual DI wiring) — it already registers all repos and IConteudoQueryService"
  - "AddConsole() removed from E2E logging (missing package) — replaced with bare AddLogging() + SetMinimumLevel"
  - "ConteudoQueryService exists in Infrastructure.Consultas (not Repositorios) — plan had wrong namespace reference; correct namespace used"
metrics:
  duration: "~6 minutes"
  completed: "2026-04-03"
  tasks_completed: 4
  files_created: 5
  files_modified: 3
---

# Phase 02 Plan 10: Tests.Integration + Tests.E2E — Testcontainers, Migrations, Repository, Walking Skeleton E2E Summary

**One-liner:** Testcontainers-based integration tests (migrations, ConteudoRepository, I-12 CategoriaRepository) and 4-test E2E walking skeleton proving SC1 create→persist→list and SC2 user isolation on real PostgreSQL.

## Tasks Completed

| # | Task | Commit | Files |
|---|------|--------|-------|
| 10.1 | PostgresTestBase — reusable Testcontainers base class | `362f42b` | `Infrastructure/PostgresTestBase.cs` |
| 10.2 | MigrationTests — schema verification against real PostgreSQL | `362f42b` | `Migrations/MigrationTests.cs` |
| 10.3 | ConteudoRepository + CategoriaRepository (I-12) integration tests | `362f42b` | `Repositorios/ConteudoRepositoryTests.cs`, `CategoriaRepositoryTests.cs` |
| 10.4 | WalkingSkeletonTests — SC1/SC2 E2E proof via MediatR + real PostgreSQL | `3436aa8` | `Tests.E2E/WalkingSkeletonTests.cs` |

## Test Results

```
Tests.Integration: 20 passed (0 failed, 0 skipped)
Tests.E2E:          5 passed (0 failed, 0 skipped)
Total:             25 passed
```

### Integration Tests Breakdown

**MigrationTests (9 tests):**
- `Migration_Up_DatabaseAceitaConexao` — DB is reachable after `MigrateAsync`
- `Migration_Up_CriaTabela_Conteudos/Fontes/ImagensConteudo/Categorias` — all 4 tables exist
- `Migration_Up_Conteudos_TemColuna_UsuarioId/Titulo` — column verification
- `Migration_Up_IndiceUnicoFontes_Prioridade` — index `idx_fontes_conteudo_prioridade_unique` exists
- `Migration_ReAplicada_NaoFalha` — idempotency: double-migrate is safe

**ConteudoRepositoryTests (5 tests):**
- `AdicionarAsync_EObterPorId_RetornaConteudo` — persist and retrieve from fresh context
- `ObterPorId_UsuarioIdErrado_RetornaNull` — user isolation security
- `BuscarPorUrlFonte_EncontradaParaUsuarioCorreto` — URL deduplication
- `BuscarPorUrlFonte_UsuarioErrado_RetornaNull` — user isolation for URL search
- `RemoverAsync_ConteudoExistente_NaoEncontradoDepois` — delete works

**CategoriaRepositoryTests (5 tests):**
- `ObterOuCriar_PrimeiraVez_CriaNova` — creates new on first call
- `ObterOuCriar_NomeDuplicadoMesmoCase_RetornaExistente` — idempotent
- `ObterOuCriar_NomeDuplicadoCaseInsensitive_RetornaExistente` — I-12: "Romance"/"romance"/"ROMANCE" → same Guid
- `ObterOuCriar_UsuariosDistintos_CategoriasIndependentes` — user isolation: different users → different category IDs
- `ListarComAutocompletar_RetornaMatchesDePrefixo` — prefix search returns only matching categories

### E2E Tests Breakdown (WalkingSkeletonTests)

- `WalkingSkeleton_CriarConteudo_Aparece_NaListagem` — **SC1 PROOF**: CriarConteudoCommand → ListarConteudosQuery returns item
- `WalkingSkeleton_IsolamentoPorUsuario_UsuarioBNaoVeConteudoDeA` — **SC2 PROOF**: User B list = 0 items
- `WalkingSkeleton_MultiploConteudos_ListagemPaginada` — 3 items, pageSize=2 → TotalPages=2, page1 has 2 items
- `WalkingSkeleton_TituloVazio_RetornaErro_NaoSalva` — I-01: empty title → TITULO_OBRIGATORIO, nothing persisted

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Missing xunit using directives**
- **Found during:** Task 10.1 (first build attempt)
- **Issue:** `[Fact]` attribute not resolved — project uses `[Xunit.Fact]` pattern but tests used short form
- **Fix:** Added `using Xunit;` to all test files
- **Files modified:** All 5 test files
- **Commit:** `362f42b`

**2. [Rule 3 - Blocking] CA1707/CA2007/CS0618 treated as errors**
- **Found during:** Task 10.1 (build)
- **Issue:** `TreatWarningsAsErrors=true` globally caused test-convention method names (with `_`) and Testcontainers constructor deprecation to fail build
- **Fix:** Added `NoWarn` entries to both test `.csproj` files: `CA1707` (xunit naming), `CA2007` (ConfigureAwait in tests), `CS0618` (Testcontainers builder), `CA1307` (StringComparison)
- **Files modified:** `Tests.Integration.csproj`, `Tests.E2E.csproj`
- **Commit:** `362f42b`

**3. [Rule 3 - Blocking] Missing Microsoft.Extensions.Logging.Console package in Tests.E2E**
- **Found during:** Task 10.4 (build)
- **Issue:** `AddConsole()` requires `Microsoft.Extensions.Logging.Console` package not in test project
- **Fix:** Simplified logging to `services.AddLogging(l => l.SetMinimumLevel(LogLevel.Warning))` — no console sink needed for test runs
- **Files modified:** `WalkingSkeletonTests.cs`
- **Commit:** `3436aa8`

**4. [Rule 2 - Missing] InternalsVisibleTo not in Infrastructure.csproj**
- **Found during:** Task 10.1 (initial source reading)
- **Issue:** `ConteudoRepository`, `CategoriaRepository`, `ConteudoQueryService` are `internal sealed` — test projects couldn't instantiate them
- **Fix:** Added `InternalsVisibleTo` for `Tests.Integration` and `Tests.E2E` in `DiarioDeBordo.Infrastructure.csproj`
- **Files modified:** `DiarioDeBordo.Infrastructure.csproj`
- **Commit:** `362f42b`

**5. [Deviation] Plan had wrong namespace for ConteudoQueryService**
- **Plan stated:** Register `DiarioDeBordo.Infrastructure.Repositorios.ConteudoQueryService`
- **Reality:** `ConteudoQueryService` is in `DiarioDeBordo.Infrastructure.Consultas` namespace
- **Resolution:** Used `services.AddInfrastructure()` which registers everything correctly; no manual DI wiring needed

## Known Stubs

None — all E2E tests use real PostgreSQL via Testcontainers. No mock data, no in-memory substitutes.

## Self-Check: PASSED

```bash
[ -f "tests/Tests.Integration/Infrastructure/PostgresTestBase.cs" ] && echo "FOUND" # FOUND
[ -f "tests/Tests.Integration/Migrations/MigrationTests.cs" ] && echo "FOUND"       # FOUND
[ -f "tests/Tests.Integration/Repositorios/ConteudoRepositoryTests.cs" ] && echo "FOUND"  # FOUND
[ -f "tests/Tests.Integration/Repositorios/CategoriaRepositoryTests.cs" ] && echo "FOUND" # FOUND
[ -f "tests/Tests.E2E/WalkingSkeletonTests.cs" ] && echo "FOUND"                          # FOUND
git log --oneline | grep "362f42b\|3436aa8"
# 3436aa8 test(02-10): walking skeleton E2E
# 362f42b test(02-10): migration tests, ConteudoRepository, CategoriaRepository
```
