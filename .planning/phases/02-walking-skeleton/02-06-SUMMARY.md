---
phase: "02"
plan: "06"
subsystem: Module.Acervo
tags: [cqrs, mediatr, commands, queries, handlers, resx, localization]
dependency_graph:
  requires: [02-02-core-entities, 02-03-module-shared]
  provides: [acervo-cqrs-layer]
  affects: [desktop-di-registration, infrastructure-query-service]
tech_stack:
  added: [MediatR 12.4.1, System.Resources.ResourceManager]
  patterns: [CQRS, Railway Oriented Programming, DomainException catch-and-convert, PaginatedList mapping]
key_files:
  created:
    - src/Modules/Module.Acervo/Commands/CriarConteudoCommand.cs
    - src/Modules/Module.Acervo/Commands/CriarConteudoHandler.cs
    - src/Modules/Module.Acervo/Queries/ListarConteudosQuery.cs
    - src/Modules/Module.Acervo/Queries/ListarConteudosHandler.cs
    - src/Modules/Module.Acervo/Queries/ObterConteudoHandler.cs
    - src/Modules/Module.Acervo/DTOs/ConteudoResumoDto.cs
    - src/Modules/Module.Acervo/DTOs/ConteudoDetalheDto.cs
    - src/Modules/Module.Acervo/DependencyInjection.cs
    - src/Modules/Module.Acervo/Resources/Strings.pt-BR.resx
    - src/Modules/Module.Acervo/Resources/Strings.cs
  modified:
    - src/Modules/Module.Acervo/DiarioDeBordo.Module.Acervo.csproj
decisions:
  - "CA1812 suppressed via [SuppressMessage] on internal handler classes — MediatR DI instantiates them at runtime"
  - "CA1716 suppressed project-wide (Module is VB.NET keyword) — consistent with Module.Shared approach"
  - "DependencyInjection class renamed to AcervoServiceCollectionExtensions to avoid CA1724 (namespace collision)"
  - "DTOs use string for Formato/Papel — enum→string conversion in handler mapping layer"
  - "ListarConteudosHandler maps ResultadoPaginado<ConteudoResumoData> (Core) → PaginatedList<ConteudoResumoDto> (Module.Shared)"
metrics:
  duration: "~15 minutes"
  completed: "2026-04-03"
  tasks_completed: 2
  files_created: 10
  files_modified: 1
---

# Phase 02 Plan 06: Module.Acervo — CQRS Handlers, DTOs, .resx Strings Summary

**One-liner:** MediatR CQRS layer for Acervo with CriarConteudoCommand, ListarConteudosQuery, ObterConteudoQuery, FormatoMidia/PapelConteudo DTO mapping, and pt-BR ResourceManager strings.

## What Was Built

### Task 6.1: Commands, Queries, DTOs, and Handlers

**Commands:**
- `CriarConteudoCommand(UsuarioId, Titulo)` — `sealed record : IRequest<Resultado<Guid>>`
- `CriarConteudoHandler` — fast-fail validation → `Conteudo.Criar()` with `DomainException` catch → `IConteudoRepository.AdicionarAsync()` → `IPublisher.Publish(ConteudoCriadoNotification)`

**Queries:**
- `ListarConteudosQuery(UsuarioId, PaginacaoParams)` — `sealed record : IRequest<Resultado<PaginatedList<ConteudoResumoDto>>>`
- `ObterConteudoQuery(Id, UsuarioId)` — `sealed record : IRequest<Resultado<ConteudoDetalheDto>>`
- `ListarConteudosHandler` — calls `IConteudoQueryService.ListarAsync()`, maps `ResultadoPaginado<ConteudoResumoData>` → `PaginatedList<ConteudoResumoDto>` (enum→string conversion)
- `ObterConteudoHandler` — calls `IConteudoQueryService.ObterAsync()`, returns `Erros.NaoEncontrado` if null

**DTOs:**
- `ConteudoResumoDto(Id, Titulo, Formato, Papel, CriadoEm)` — `string` for Formato/Papel (UI-friendly)
- `ConteudoDetalheDto(Id, Titulo, Descricao, Anotacoes, Nota, Formato, Papel, CriadoEm)` — complete detail view

**DI Registration:**
- `AcervoServiceCollectionExtensions.AddModuleAcervo()` — placeholder; MediatR handlers auto-registered by `AddMediatR` assembly scanning from host

### Task 6.2: Strings.pt-BR.resx

- 13 resource keys: CriarConteudo (6), Acervo (3), Erros (2), Labels (4)  
- `Strings.cs` strongly-typed accessor with `CultureInfo.CurrentUICulture` (CA1304 compliant)
- `EmbeddedResource Update` in csproj with explicit `LogicalName`

## Architecture Compliance

✅ `Module.Acervo` → depends only on `DiarioDeBordo.Core` + `Module.Shared`  
✅ No reference to `DiarioDeBordo.Infrastructure` or `DiarioDeBordoDbContext`  
✅ All queries include `UsuarioId` (SEG-03)  
✅ All listagens use `PaginacaoParams` — no bare `List<T>` returns  
✅ No hardcoded Portuguese strings in handlers or DTOs  

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] CA1812: Internal handler classes appear uninstantiated**
- **Found during:** Task 6.1 build
- **Issue:** `TreatWarningsAsErrors=true` with `AnalysisMode=All` flags `internal sealed class` handlers as never instantiated (CA1812), even though MediatR instantiates them via DI
- **Fix:** Added `[SuppressMessage("Performance", "CA1812:...", Justification = "Instantiated by MediatR via DI container at runtime.")]` to all three handler classes
- **Files modified:** CriarConteudoHandler.cs, ListarConteudosHandler.cs, ObterConteudoHandler.cs

**2. [Rule 1 - Bug] CA1716: Module namespace conflicts with VB.NET reserved keyword**
- **Found during:** Task 6.1 build
- **Issue:** `AnalysisMode=All` flags `Module` in namespace as VB.NET keyword conflict
- **Fix:** Added `<NoWarn>$(NoWarn);CA1716</NoWarn>` to csproj — same approach used by Module.Shared
- **Files modified:** DiarioDeBordo.Module.Acervo.csproj

**3. [Rule 1 - Bug] CA1724: DependencyInjection class name conflicts with namespace**
- **Found during:** Task 6.1 build
- **Issue:** Class named `DependencyInjection` collides with `Microsoft.Extensions.DependencyInjection` namespace (CA1724)
- **Fix:** Renamed to `AcervoServiceCollectionExtensions` — conventional naming for extension method classes
- **Files modified:** DependencyInjection.cs

**4. [Rule 1 - Bug] CA1304: ResourceManager.GetString without CultureInfo**
- **Found during:** Task 6.2 build
- **Issue:** `ResourceManager.GetString(key)` overload triggers CA1304 (locale-dependent behavior)
- **Fix:** Changed to `ResourceManager.GetString(key, CultureInfo.CurrentUICulture)` in `Strings.cs`
- **Files modified:** Resources/Strings.cs

**5. [Rule 1 - Bug] Duplicate EmbeddedResource NETSDK1022**
- **Found during:** Task 6.2 build
- **Issue:** SDK automatically includes `.resx` files; explicit `<Include>` caused duplicate item error
- **Fix:** Changed `Include` to `Update` in csproj — only sets metadata without re-declaring the item
- **Files modified:** DiarioDeBordo.Module.Acervo.csproj

## Build Verification

```
dotnet build src/Modules/Module.Acervo --configuration Release
→ Construir êxito em 1,4s — 0 errors, 0 warnings
```

## Self-Check: PASSED

- All 10 new files exist on disk ✅
- Commits dccef4c and e8dfeb8 exist in git log ✅
- No Infrastructure reference in csproj ✅
- No DiarioDeBordoDbContext reference in any .cs file ✅
