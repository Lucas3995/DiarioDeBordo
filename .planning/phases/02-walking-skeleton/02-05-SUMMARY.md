---
phase: "02"
plan: "05"
subsystem: "infrastructure"
tags: [efcore, postgresql, repository, migrations, dbcontext]
dependency_graph:
  requires: ["02-01", "02-02"]
  provides: ["IConteudoRepository", "ICategoriaRepository", "IConteudoQueryService", "DiarioDeBordoDbContext", "InfrastructureServiceCollectionExtensions"]
  affects: ["02-06", "02-07", "Desktop DI wiring"]
tech_stack:
  added:
    - "Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4 (already in csproj)"
    - "Microsoft.EntityFrameworkCore.Design 9.0.4 (already in csproj)"
    - "Microsoft.Extensions.DependencyInjection 9.0.4 (added)"
    - "System.Security.Cryptography.ProtectedData 9.0.0 (added — fixes Windows DPAPI)"
  patterns:
    - "Fluent API entity configuration via IEntityTypeConfiguration<T>"
    - "OwnsOne for Progresso record (value object inline in conteudos table)"
    - "OwnsMany for Fontes and Imagens (separate tables with FK)"
    - "internal sealed repositories registered via DI — never exposed directly"
    - "CQRS split: ConteudoRepository (commands) + ConteudoQueryService (queries)"
key_files:
  created:
    - "src/DiarioDeBordo.Infrastructure/Persistencia/DiarioDeBordoDbContext.cs"
    - "src/DiarioDeBordo.Infrastructure/Persistencia/Configuracoes/ConteudoConfiguration.cs"
    - "src/DiarioDeBordo.Infrastructure/Persistencia/Configuracoes/CategoriaConfiguration.cs"
    - "src/DiarioDeBordo.Infrastructure/Persistencia/DesignTimeDbContextFactory.cs"
    - "src/DiarioDeBordo.Infrastructure/Repositorios/ConteudoRepository.cs"
    - "src/DiarioDeBordo.Infrastructure/Repositorios/CategoriaRepository.cs"
    - "src/DiarioDeBordo.Infrastructure/Consultas/ConteudoQueryService.cs"
    - "src/DiarioDeBordo.Infrastructure/DependencyInjection.cs"
    - "src/DiarioDeBordo.Infrastructure/Persistencia/Migrations/20260403043803_InitialCreate.cs (via dotnet ef)"
    - "src/DiarioDeBordo.Infrastructure/Persistencia/Migrations/20260403043803_InitialCreate.Designer.cs"
    - "src/DiarioDeBordo.Infrastructure/Persistencia/Migrations/DiarioDeBordoDbContextModelSnapshot.cs"
  modified:
    - "src/DiarioDeBordo.Infrastructure/DiarioDeBordo.Infrastructure.csproj"
    - "src/DiarioDeBordo.Infrastructure/Seguranca/ArmazenamentoSeguroWindows.cs (async fix)"
decisions:
  - "DependencyInjection class renamed to InfrastructureServiceCollectionExtensions — CA1724 (class name conflicted with Microsoft.Extensions.DependencyInjection namespace)"
  - "CA1062/CA1861 added to NoWarn — migration files are auto-generated; null checks on MigrationBuilder are unnecessary and CA1861 penalizes generated one-time DDL arrays"
  - "Migration generated via dotnet ef migrations add (DesignTimeDbContextFactory uses fallback placeholder password so it works without live DB in CI)"
  - "SEG-02 enforced: every repository method and query includes .Where(c => c.UsuarioId == usuarioId)"
metrics:
  duration: "~30 minutes"
  completed_date: "2026-04-03"
  tasks_completed: 2
  files_created: 11
  files_modified: 2
---

# Phase 02 Plan 05: Infrastructure — DbContext, EF Core Config, Migration, Repositories Summary

**One-liner:** EF Core DbContext with Fluent API config (OwnsOne/OwnsMany, snake_case), ConteudoRepository + CategoriaRepository (SEG-02 enforced), CQRS ConteudoQueryService, and auto-generated InitialCreate migration via dotnet ef.

## What Was Built

### DiarioDeBordoDbContext
`DiarioDeBordoDbContext` exposes two DbSets: `Conteudos` and `Categorias`. All entity type configurations are applied via `ApplyConfigurationsFromAssembly`.

### Entity Configuration (Fluent API)
- **ConteudoConfiguration**: `conteudos` table with snake_case columns; `Progresso` as inline owned entity (OwnsOne); `Fontes` in `fontes` table (OwnsMany, with unique index on `(conteudo_id, prioridade)` — I-07); `Imagens` in `imagens_conteudo` table (OwnsMany). Backing fields `_fontes`/`_imagens` explicitly referenced via `HasField`.
- **CategoriaConfiguration**: `categorias` table with unique index on `(usuario_id, nome_normalizado)` — I-12.

### Repositories (command side)
- `ConteudoRepository` — implements `IConteudoRepository`. Every query includes `.Where(c => c.Id == id && c.UsuarioId == usuarioId)` — SEG-02 absolute rule.
- `CategoriaRepository` — implements `ICategoriaRepository`. `ObterOuCriarAsync` uses `.ToLowerInvariant()` for case-insensitive uniqueness (I-12).

### Query Service (CQRS query side)
- `ConteudoQueryService` — implements `IConteudoQueryService`. Uses `.Skip(paginacao.Offset).Take(paginacao.ItensPorPagina)` for pagination. All queries filter by `usuarioId`.

### DI Registration
`InfrastructureServiceCollectionExtensions.AddInfrastructure(connectionString)` registers `DbContext`, `IConteudoRepository`, `ICategoriaRepository`, `IConteudoQueryService`.

### Migration
`InitialCreate` migration (auto-generated via `dotnet ef migrations add`) creates tables `conteudos`, `fontes`, `imagens_conteudo`, `categorias`. The `DesignTimeDbContextFactory` uses `DIARIODEBORDO_EF_DEV_PG_PASSWORD` env var with fallback for CI/design-time use.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] `DbContextOptionsBuilder.Build()` → `.Options`**
- **Found during:** Task 5.2
- **Issue:** Plan's template used non-existent `.Build()` method on `DbContextOptionsBuilder<T>`
- **Fix:** Changed to `.Options` property
- **Files modified:** `DesignTimeDbContextFactory.cs`

**2. [Rule 2 - Missing Critical] Added null check for `modelBuilder` in `OnModelCreating`**
- **Found during:** Task 5.1 (build)
- **Issue:** CA1062 (TreatWarningsAsErrors=true) — public override method must validate non-null parameter
- **Fix:** Added `ArgumentNullException.ThrowIfNull(modelBuilder)`
- **Files modified:** `DiarioDeBordoDbContext.cs`

**3. [Rule 1 - Bug] Class name `DependencyInjection` conflicts with namespace**
- **Found during:** Task 5.1 (build)
- **Issue:** CA1724 — class name `DependencyInjection` conflicts with `Microsoft.Extensions.DependencyInjection`
- **Fix:** Renamed to `InfrastructureServiceCollectionExtensions`
- **Files modified:** `DependencyInjection.cs`

**4. [Rule 2 - Missing Critical] Added `System.Security.Cryptography.ProtectedData` NuGet package**
- **Found during:** Task 5.2 (build with migrations)
- **Issue:** CS0103 — `ProtectedData` and `DataProtectionScope` not resolvable without package reference
- **Fix:** Added `System.Security.Cryptography.ProtectedData 9.0.0` package
- **Files modified:** `DiarioDeBordo.Infrastructure.csproj`

**5. [Rule 3 - Blocking] Migration CA1062/CA1861 warnings as errors**
- **Found during:** Task 5.2 (build after migration generation)
- **Issue:** Auto-generated migration code triggers CA1062 (null check `MigrationBuilder`) and CA1861 (constant arrays in CreateIndex); both are `TreatWarningsAsErrors`
- **Fix:** Added `CA1062;CA1861` to `<NoWarn>` in csproj with documented rationale
- **Files modified:** `DiarioDeBordo.Infrastructure.csproj`

**6. [Rule 3 - Blocking] `Microsoft.Extensions.DependencyInjection` version downgrade**
- **Found during:** Task 5.1 (first build)
- **Issue:** NU1605 — added version 9.0.0 but transitive deps require 9.0.4
- **Fix:** Changed to 9.0.4
- **Files modified:** `DiarioDeBordo.Infrastructure.csproj`

## Verification

```
dotnet build src/DiarioDeBordo.Infrastructure --configuration Release
→ Compilação com êxito. 0 Aviso(s), 0 Erro(s)

grep -c "usuario_id" Persistencia/Migrations/*InitialCreate.cs
→ 6 (found)

ConteudoRepository.ObterPorIdAsync contains:
  .Where(c => c.Id == id && c.UsuarioId == usuarioId) ✓

CategoriaRepository.ObterOuCriarAsync contains:
  .ToLowerInvariant() ✓
```

## Known Stubs

None — all methods are fully implemented.

## Self-Check

- [x] `DiarioDeBordoDbContext.cs` exists
- [x] `ConteudoConfiguration.cs` exists
- [x] `CategoriaConfiguration.cs` exists
- [x] `ConteudoRepository.cs` exists
- [x] `CategoriaRepository.cs` exists
- [x] `ConteudoQueryService.cs` exists
- [x] `DependencyInjection.cs` exists
- [x] Migration files exist in `Persistencia/Migrations/`
- [x] Commits `5519880` and `aae39f1` exist

## Self-Check: PASSED
