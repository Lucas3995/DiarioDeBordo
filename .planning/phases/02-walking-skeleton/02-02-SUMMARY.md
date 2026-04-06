---
phase: "02"
plan: "02"
subsystem: DiarioDeBordo.Core
tags: [core, domain, entities, result-type, repository-interfaces, events, feature-flags]
dependency_graph:
  requires: []
  provides: [Resultado<T>, Erro, DomainException, Erros, PaginacaoParams, ResultadoPaginado<T>, Conteudo, Categoria, Fonte, ImagemConteudo, Progresso, IConteudoRepository, IColetaneaRepository, ICategoriaRepository, IConteudoQueryService, domain-events, IFeatureFlags, IArmazenamentoSeguro, IPostgresBootstrap]
  affects: [DiarioDeBordo.Infrastructure, DiarioDeBordo.Desktop, all-modules]
tech_stack:
  added: [MediatR 12.4.1, Microsoft.Extensions.Logging.Abstractions 9.0.0]
  patterns: [Railway Oriented Programming, DDD Aggregate Root, CQRS interfaces, Factory Method]
key_files:
  created:
    - src/DiarioDeBordo.Core/DiarioDeBordo.Core.csproj
    - src/DiarioDeBordo.Core/Primitivos/Resultado.cs
    - src/DiarioDeBordo.Core/Primitivos/DomainException.cs
    - src/DiarioDeBordo.Core/Primitivos/Erros.cs
    - src/DiarioDeBordo.Core/Primitivos/Paginacao.cs
    - src/DiarioDeBordo.Core/Enums/Enums.cs
    - src/DiarioDeBordo.Core/Entidades/Conteudo.cs
    - src/DiarioDeBordo.Core/Entidades/Categoria.cs
    - src/DiarioDeBordo.Core/Entidades/Fonte.cs
    - src/DiarioDeBordo.Core/Entidades/ImagemConteudo.cs
    - src/DiarioDeBordo.Core/Entidades/Progresso.cs
    - src/DiarioDeBordo.Core/Repositorios/IConteudoRepository.cs
    - src/DiarioDeBordo.Core/Repositorios/IColetaneaRepository.cs
    - src/DiarioDeBordo.Core/Repositorios/ICategoriaRepository.cs
    - src/DiarioDeBordo.Core/Consultas/IConteudoQueryService.cs
    - src/DiarioDeBordo.Core/Eventos/EventosDeDominio.cs
    - src/DiarioDeBordo.Core/FeatureFlags/IFeatureFlags.cs
    - src/DiarioDeBordo.Core/Infraestrutura/IArmazenamentoSeguro.cs
    - src/DiarioDeBordo.Core/Infraestrutura/IPostgresBootstrap.cs
  modified: []
decisions:
  - "Resultado<T> (Portuguese naming) with Railway Oriented Programming — all service operations return Resultado, never throw for expected business failures"
  - "Conteudo.Criar() factory enforces I-01/I-02 invariants via DomainException (not Resultado) for internal aggregate consistency"
  - "IConteudoQueryService added to Core (CQRS read contracts) with ResultadoPaginado<T> to avoid circular deps with Module.Shared"
  - "NoWarn for CA1054/CA1308/CA1822: URL as string (design choice), ToLowerInvariant for names (intentional), AlterarTipoColetanea instance method (API consistency)"
metrics:
  duration_minutes: 8
  completed_date: "2026-04-03T03:47:00Z"
  tasks_completed: 2
  files_created: 19
  files_modified: 0
---

# Phase 02 Plan 02: DiarioDeBordo.Core Summary

**One-liner:** Core shared kernel with Resultado<T> (ROP), DomainException, Conteudo aggregate root (invariants I-01–I-10), repository/query interfaces, domain events via MediatR INotification, and infrastructure contracts.

## What Was Built

`DiarioDeBordo.Core` — the shared kernel that all other projects depend on. Zero dependencies on other projects in the solution.

### Primitivos
- **`Resultado<T>`** — Railway Oriented Programming result type with `IsSuccess`, `IsFailure`, `Value`, `Error`, `Alerta`. Includes `Failure<TOther>()` for error propagation across types.
- **`Erro`** — Immutable record `(Codigo, Mensagem)`.
- **`AlertaUsoSaudavel`** — Healthy-use nudge that can accompany success results.
- **`DomainException`** — For invariant violations; carries `Codigo` and `ToErro()` conversion method.
- **`Erros`** — Centralized catalog of all domain error codes (no magic strings in handlers).
- **`PaginacaoParams`** + **`ResultadoPaginado<T>`** — Pagination contracts native to Core.

### Domain Entities
- **`Conteudo`** (aggregate root) — `Criar()` factory enforces I-01 (title required) and I-02 (TipoColetanea constraint). Methods `DefinirNota()` (I-03), `AdicionarImagem()` (I-04/I-05), `DefinirImagemPrincipal()` (I-06), `AdicionarFonte()` (I-07), `AlterarProgresso()`, `AlterarTipoColetanea()` (I-10, always throws).
- **`Categoria`** — Factory `Criar()` enforces I-11, normalizes name to lowercase.
- **`Fonte`**, **`ImagemConteudo`**, **`Progresso`** — Child entities/value objects owned by Conteudo aggregate.

### Repository Interfaces
- **`IConteudoRepository`** — 6 methods, all include `usuarioId` (security invariant §4.6).
- **`IColetaneaRepository`** — Includes `ObterDescendentesAsync` for DFS cycle detection.
- **`ICategoriaRepository`** — Atomic `ObterOuCriarAsync` pattern.
- **`IConteudoQueryService`** — CQRS read side with `ConteudoResumoData` / `ConteudoDetalheData`.

### Domain Events
`ConteudoCriadoNotification`, `ProgressoAlteradoNotification`, `ItemFeedPersistidoNotification` — all implement `MediatR.INotification`.

### Infrastructure Contracts
- **`IFeatureFlags`** + `FeatureFlagsPlaceholder` (all flags off until Phase 3)
- **`IArmazenamentoSeguro`** — OS-native secret storage (DPAPI/libsecret)
- **`IPostgresBootstrap`** — Bundled PostgreSQL lifecycle management

## Verification

```
dotnet build src/DiarioDeBordo.Core --configuration Release
→ Build succeeded. (0 errors, 0 warnings)
```

All success criteria from PLAN.md:
- ✅ `dotnet build src/DiarioDeBordo.Core --configuration Release` → 0 errors, 0 warnings
- ✅ `Resultado<T>.IsSuccess` is `true` via `Success()`, `false` via `Failure()`
- ✅ `DomainException` inherits from `Exception` and carries `Codigo` property
- ✅ `IConteudoRepository` has 6 methods, all include `usuarioId` parameter
- ✅ Domain event records defined: `ConteudoCriadoNotification`, `ProgressoAlteradoNotification`, `ItemFeedPersistidoNotification`

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Code analyzer errors (TreatWarningsAsErrors)**
- **Found during:** Build step
- **Issue:** Three Roslyn analyzers fired as errors: CA1054 (URL string param), CA1308 (ToLowerInvariant), CA1822 (non-static instance method)
- **Fix:** Added `<NoWarn>CA1054;CA1308;CA1822</NoWarn>` to csproj with explanatory XML comments. All three are intentional design choices (normalized URLs as strings, lowercase normalization for persistence, instance method for API surface consistency).
- **Files modified:** `src/DiarioDeBordo.Core/DiarioDeBordo.Core.csproj`

### Additions (not in plan but mentioned in execution context)

**1. [Rule 2 - Missing functionality] `IConteudoQueryService` and query DTOs**
- **Added:** `src/DiarioDeBordo.Core/Consultas/IConteudoQueryService.cs` — CQRS read-side contract with `ConteudoResumoData` and `ConteudoDetalheData`. Mentioned in execution instructions and part of the designed Core contracts.

**2. [Rule 2 - Missing functionality] `PaginacaoParams` and `ResultadoPaginado<T>`**
- **Added:** `src/DiarioDeBordo.Core/Primitivos/Paginacao.cs` — Pagination types native to Core to avoid circular dependencies with Module.Shared. Explicitly required by execution instructions.

## Known Stubs

- `FeatureFlagsPlaceholder.IsEnabled()` always returns `false` — intentional placeholder until Phase 3 implements feature flag storage.

## Commits

| Hash | Message |
|------|---------|
| 2c40ffa | feat(core): implement Core — Result<T>, domain entities, repository interfaces, domain events |

## Self-Check: PASSED
