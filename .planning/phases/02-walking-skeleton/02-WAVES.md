# Phase 2 Execution Waves

## Summary

| Wave | Plans | Parallelizable | Depends On |
|------|-------|---------------|------------|
| 1 | 02-01, 02-02, 02-03, 02-04 | Yes — all 4 can run simultaneously | Nothing |
| 2 | 02-05, 02-06, 02-07 | Yes — all 3 can run simultaneously | Wave 1 complete |
| 3 | 02-08, 02-09, 02-10 | Yes — all 3 can run simultaneously | Wave 2 complete |

---

## Wave 1 (parallel — no dependencies)

All four plans can be executed simultaneously by separate Claude sessions or sequentially in any order.

- **02-01** — Solution structure: `.sln`, all 15 `.csproj` files, `Directory.Build.props`, `BannedSymbols.txt`, `.editorconfig`
- **02-02** — Core project: `Resultado<T>`, `Erro`, `DomainException`, `Conteudo` entity with invariants, `IConteudoRepository`, domain events, `IFeatureFlags`, `IArmazenamentoSeguro`
- **02-03** — Module.Shared: `PaginatedList<T>`, `PaginacaoParams` + unit tests
- **02-04** — CI pipeline: GitHub Actions workflow (matrix ubuntu × windows), coverage gate ≥95%, `dotnet-tools.json` with reportgenerator

**Wave 1 gate:** `dotnet build DiarioDeBordo.sln --configuration Release` → 0 errors, 0 warnings

---

## Wave 2 (parallel — depends on all Wave 1 complete)

All three plans can run simultaneously after Wave 1. Each modifies different files with no overlap.

- **02-05** — Infrastructure: `DiarioDeBordoDbContext`, EF Core entity configuration, `ConteudoRepository`, `CategoriaRepository`, first migration `InitialCreate`
- **02-06** — Module.Acervo: `CriarConteudoCommand` + handler, `ListarConteudosQuery` + handler, `ObterConteudoQuery` + handler, `Strings.pt-BR.resx`
- **02-07** — PostgreSQL bootstrap: `PostgresBootstrap` (pg_ctl + initdb), `ArmazenamentoSeguroWindows` (DPAPI), `ArmazenamentoSeguroLinux` (libsecret + AES-256-GCM fallback)

**Wave 2 gate:**
- `dotnet build src/DiarioDeBordo.Infrastructure --configuration Release` → 0 warnings
- `dotnet build src/Modules/Module.Acervo --configuration Release` → 0 warnings
- `dotnet ef migrations list` (from Infrastructure) → shows `InitialCreate`

---

## Wave 3 (parallel — depends on all Wave 2 complete)

All three plans can run simultaneously after Wave 2.

- **02-08** — UI shell: `MainWindow` (SukiWindow + SukiSideMenu), `MainViewModel`, `AcervoViewModel`, `CriarConteudoViewModel`, AXAML views with WCAG 2.2 AAA compliance, progressive disclosure form, `Desktop` DI setup + bootstrap
- **02-09** — Tests.Domain: All 12 invariant tests (I-01 through I-12), `CriarConteudoHandler` handler tests with NSubstitute mocks — no database
- **02-10** — Tests.Integration + Tests.E2E: Testcontainers base class, migration tests, repository tests, category I-12 uniqueness, walking skeleton E2E (SC1)

**Wave 3 gate (= Phase 2 complete):**
```bash
# Build: 0 warnings, 0 errors
dotnet build DiarioDeBordo.sln --configuration Release

# Domain tests (no infrastructure): all pass
dotnet test tests/Tests.Domain --configuration Release

# Unit tests (no infrastructure): all pass
dotnet test tests/Tests.Unit --configuration Release

# Integration tests (Docker required on Linux): all pass
dotnet test tests/Tests.Integration --configuration Release

# E2E walking skeleton: all pass
dotnet test tests/Tests.E2E --filter "WalkingSkeleton" --configuration Release

# Application launches (manual verification)
# - PostgreSQL starts on port 15432
# - Navigation shell appears with 4 sidebar items
# - "Acervo" shows empty state
# - Creating content with title "Dune" → appears in list
```

---

## Dependency Graph (visual)

```
02-01 ──┐
02-02 ──┤
02-03 ──┤──→ 02-05 ──┐
02-04 ──┘   02-06 ──┤──→ 02-08
             02-07 ──┘    02-09
                          02-10
```

---

## Critical Path

The minimum sequential path to Phase 2 completion:

```
02-02 (Core) → 02-05 (Infrastructure) → 02-10 (Integration + E2E)
```

This represents the absolute minimum to prove SC1 (walking skeleton works end-to-end).

All other plans add completeness (CI, UI, domain tests) but are not on the critical path for the core E2E proof.

---

## File Ownership (no overlaps between parallel plans)

| Plan | Files Owned (no other plan touches these) |
|------|------------------------------------------|
| 02-01 | `*.sln`, `*.csproj`, `Directory.Build.props`, `BannedSymbols.txt`, `.editorconfig`, `global.json` |
| 02-02 | `src/DiarioDeBordo.Core/**` |
| 02-03 | `src/Modules/Module.Shared/**`, `tests/Tests.Unit/Paginacao/**` |
| 02-04 | `.github/workflows/ci.yml`, `.config/dotnet-tools.json` |
| 02-05 | `src/DiarioDeBordo.Infrastructure/Persistencia/**`, `src/DiarioDeBordo.Infrastructure/Repositorios/**`, `src/DiarioDeBordo.Infrastructure/DependencyInjection.cs` |
| 02-06 | `src/Modules/Module.Acervo/**` |
| 02-07 | `src/DiarioDeBordo.Infrastructure/Postgres/**`, `src/DiarioDeBordo.Infrastructure/Seguranca/**`, `src/DiarioDeBordo.Infrastructure/DependencyInjectionBootstrap.cs` |
| 02-08 | `src/DiarioDeBordo.Desktop/**`, `src/DiarioDeBordo.UI/**` |
| 02-09 | `tests/Tests.Domain/**` |
| 02-10 | `tests/Tests.Integration/**`, `tests/Tests.E2E/**`, `src/DiarioDeBordo.Infrastructure/Repositorios/ConteudoQueryService.cs` (also updates `DependencyInjection.cs` — sequential after 02-05) |

No two plans in the same wave own overlapping files → safe for parallel execution.

---

## Notes for Executor

1. **Wave 1 first** — no code can be built without the `.sln` and `Directory.Build.props` (02-01). Run 02-01 before starting 02-02, 02-03, 02-04 in parallel.

2. **02-02 is the foundation** — Core entities are used by every downstream plan. Execute carefully; mistakes here cascade.

3. **Migration (02-05)** must be generated with `dotnet ef migrations add InitialCreate` — this is a CLI command, not a manually written file. The executor must have the .NET EF tools installed.

4. **PostgreSQL bootstrap (02-07)** installs to `src/DiarioDeBordo.Infrastructure/`. The `installer/postgres/` directory is a placeholder for bundled binaries — in Phase 2, these are expected to be downloaded separately (outside this plan's scope).

5. **UI (02-08)** requires SukiUI package to compile. Verify `dotnet restore` downloads the correct version before building.

6. **E2E tests (02-10)** require Docker on the CI runner (Linux). Windows CI will skip integration/E2E tests as documented in `ci.yml`.
