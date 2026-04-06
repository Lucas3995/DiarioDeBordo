---
phase: "02"
plan: "09"
subsystem: tests
tags: [domain-tests, invariants, tdd, no-database]
dependency_graph:
  requires: [02-02, 02-06]
  provides: [domain-invariant-test-coverage]
  affects: [Tests.Domain]
tech_stack:
  added: []
  patterns: [xunit, NSubstitute, Given_When_Then naming]
key_files:
  created:
    - tests/Tests.Domain/Acervo/ConteudoInvariantTests.cs
    - tests/Tests.Domain/Acervo/ColetaneaInvariantTests.cs
    - tests/Tests.Domain/Acervo/CategoriaInvariantTests.cs
    - tests/Tests.Domain/Acervo/CriarConteudoHandlerTests.cs
    - tests/Tests.Domain/Acervo/AlterarProgressoTests.cs
    - tests/Tests.Domain/GlobalUsings.cs
  modified:
    - tests/Tests.Domain/Tests.Domain.csproj
    - src/Modules/Module.Acervo/DiarioDeBordo.Module.Acervo.csproj
decisions:
  - Suppressed CA1707 (no-underscores rule) in Tests.Domain — Given_When_Then is the xunit naming convention mandated by the plan
  - Added GlobalUsings.cs instead of per-file using Xunit — keeps test files clean
  - InternalsVisibleTo uses assembly name "Tests.Domain" (not namespace) — project file has no explicit AssemblyName
  - Did NOT duplicate xunit/TestSdk packages — Directory.Build.targets already provides them for IsTestProject=true projects
metrics:
  duration_minutes: 3
  completed_date: "2026-04-03"
  tasks_completed: 3
  files_changed: 8
---

# Phase 02 Plan 09: Tests.Domain — All Domain Invariant Tests Summary

**One-liner:** 44 domain invariant tests covering I-01 through I-12 (I-08/I-09 deferred), all passing without a database using DomainException-throws pattern and NSubstitute mocks.

## What Was Built

All domain invariant tests for the Acervo bounded context, running purely against domain logic with no database dependency (SC2 verified).

### Test Files Created

| File | Tests | Invariants Covered |
|------|-------|--------------------|
| `ConteudoInvariantTests.cs` | 24 | I-01 (título), I-02 (TipoColetanea/Papel), I-03 (nota 0-10), I-04 (max 20 imagens), I-05 (max 10MB), I-06 (única principal), I-07 (prioridade única) |
| `ColetaneaInvariantTests.cs` | 6 (+2 Skip) | I-08/I-09 deferred, I-10 (TipoColetanea imutável) |
| `CategoriaInvariantTests.cs` | 6 | I-11 (nome obrigatório), I-12 (normalização case-insensitive) |
| `CriarConteudoHandlerTests.cs` | 5 | Handler validation + persistence + notification |
| `AlterarProgressoTests.cs` | 3 | Progresso state transitions |

**Total: 44 passed, 2 skipped (Phase 3 deferred), 0 failures**

### Key Infrastructure Changes

- **`Module.Acervo.csproj`**: Added `<InternalsVisibleTo Include="Tests.Domain" />` to expose `internal sealed class CriarConteudoHandler` to the test project
- **`Tests.Domain.csproj`**: Added `CA1707` to NoWarn (Given_When_Then test naming convention uses underscores — this is intentional)
- **`GlobalUsings.cs`**: Added `global using Xunit;` — avoids per-file using directives

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Directory.Build.targets already provides xunit packages**
- **Found during:** Task 9.1 (first test run)
- **Issue:** Plan spec suggested adding xunit/TestSdk packages to Tests.Domain.csproj, but `Directory.Build.targets` already injects them for all `IsTestProject=true` projects. Adding them explicitly caused NU1504 "duplicate PackageReference" error treated as error.
- **Fix:** Reverted csproj to original package list (no xunit added explicitly)
- **Files modified:** `tests/Tests.Domain/Tests.Domain.csproj`
- **Commit:** 908e49e

**2. [Rule 2 - Missing] `using Xunit;` not globally available**
- **Found during:** Task 9.1 (second test run)
- **Issue:** Existing `PlaceholderTest.cs` uses fully-qualified `[Xunit.Fact]`. Test files from the plan use shorthand `[Fact]`. The generated GlobalUsings.g.cs doesn't include Xunit.
- **Fix:** Created `tests/Tests.Domain/GlobalUsings.cs` with `global using Xunit;`
- **Files modified:** `tests/Tests.Domain/GlobalUsings.cs` (new)
- **Commit:** 908e49e

**3. [Rule 2 - Missing] CA1707 suppression needed for test method naming**
- **Found during:** Task 9.1 (third test run)
- **Issue:** `TreatWarningsAsErrors=true` in `Directory.Build.props` makes CA1707 (remove underscores) a build error. xunit's standard Given_When_Then convention uses underscores.
- **Fix:** Added `CA1707` to `<NoWarn>` in Tests.Domain.csproj
- **Files modified:** `tests/Tests.Domain/Tests.Domain.csproj`
- **Commit:** 908e49e

**4. [Rule 1 - Adaptation] Actual API differs from plan prompt spec**
- **Found during:** Pre-implementation review (STEP 0)
- **Issue:** The user-provided test code in the prompt used wrong types: `PapelEstrutural` (→ `PapelConteudo`), `OrigemMetadado` (→ `OrigemImagem`), `AlterarNota()` (→ `DefinirNota()`), `TipoColetanea` property (→ `TipoColetaneaValor`), `Resultado<T>` return (→ `DomainException` throws), `AdicionarImagem()` with 2 params (→ 3 params including `tamanhoBytes`).
- **Fix:** Used the actual plan code from `02-09-PLAN.md` which correctly matched the production entities. Plan file's test code was accurate; only the prompt spec was outdated.
- **Commit:** 908e49e

## Verification

```
dotnet test tests/Tests.Domain --configuration Release

Aprovado! – Com falha: 0, Aprovado: 44, Ignorado: 2, Total: 46, Duração: 59 ms
```

- ✅ No `DbContext`, `Npgsql`, `Testcontainers` references in Tests.Domain
- ✅ I-01 through I-07 covered with explicit test methods
- ✅ I-08 and I-09 formally deferred with `[Fact(Skip = "Deferred to Phase 3: ...")]`
- ✅ I-10 (TipoColetanea immutability), I-11 (category name) covered
- ✅ I-12 (normalisation) partially covered in domain — DB uniqueness in Tests.Integration
- ✅ NSubstitute mocks for `IConteudoRepository` and `IPublisher` in handler tests

## Self-Check: PASSED
