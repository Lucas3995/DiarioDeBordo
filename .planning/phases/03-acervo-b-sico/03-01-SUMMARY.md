# Plan 03-01 Summary

**Status:** DONE
**Completed:** 2026-04-03T12:00:00Z

## What Was Built

- `Classificacao` enum (`Gostei`, `NaoGostei`) — null represents "not classified" (D-08)
- Enriched `Conteudo` aggregate: added `Classificacao?`, `IsFilho`, `TotalEsperadoSessoes?` fields
- New operations on `Conteudo`: `DefinirClassificacao()`, `LimparNota()`, `CriarComoFilho()` factory, `DefinirTotalEsperadoSessoes()`
- `TipoRelacao` entity with `Criar()` factory enforcing non-empty Nome/NomeInverso and NomeNormalizado
- `Relacao` entity for bidirectional two-row pattern with `ParId` linking
- `ConteudoCategoria` join entity (explicit M:N for `AssociadaEm` tracking)
- `ITipoRelacaoRepository` interface with `ObterOuCriarAsync`, `ListarComAutocompletarAsync`, `ObterPorIdAsync`
- `IRelacaoRepository` interface with `AdicionarParAsync`, `ListarPorConteudoAsync`, `ExisteAsync`, `RemoverParAsync`
- Enriched `IConteudoQueryService` with `CategoriaData`, `RelacaoData`, `SessaoData` records and new fields in `ConteudoDetalheData`
- 5 new error codes in `Erros.cs` (auto-reference, duplicate relation, nome/inverso obrigatório, total inválido)
- Updated `ConteudoQueryService` to filter `IsFilho=false` and project new fields
- 46 new domain tests (ClassificacaoTests, TipoRelacaoTests, RelacaoBidirecionalTests, ConteudoFilhoTests)

## Files Modified

- `src/DiarioDeBordo.Core/Enums/Enums.cs` — added `Classificacao` enum
- `src/DiarioDeBordo.Core/Entidades/Conteudo.cs` — new fields, factory, operations
- `src/DiarioDeBordo.Core/Entidades/TipoRelacao.cs` — **created**
- `src/DiarioDeBordo.Core/Entidades/Relacao.cs` — **created**
- `src/DiarioDeBordo.Core/Entidades/ConteudoCategoria.cs` — **created**
- `src/DiarioDeBordo.Core/Repositorios/ITipoRelacaoRepository.cs` — **created**
- `src/DiarioDeBordo.Core/Repositorios/IRelacaoRepository.cs` — **created**
- `src/DiarioDeBordo.Core/Consultas/IConteudoQueryService.cs` — enriched records
- `src/DiarioDeBordo.Core/Primitivos/Erros.cs` — 5 new error codes
- `src/DiarioDeBordo.Infrastructure/Consultas/ConteudoQueryService.cs` — updated projections
- `tests/Tests.Domain/Acervo/ObterConteudoHandlerTests.cs` — updated for new constructor
- `tests/Tests.Domain/Acervo/ClassificacaoTests.cs` — **created**
- `tests/Tests.Domain/Acervo/TipoRelacaoTests.cs` — **created**
- `tests/Tests.Domain/Acervo/RelacaoBidirecionalTests.cs` — **created**
- `tests/Tests.Domain/Acervo/ConteudoFilhoTests.cs` — **created**

## Tests

- 46 new domain tests added
- 106 total tests passing (Tests.Domain)
- Build: 0 warnings (C# compiler), 0 errors

## Deviations

- Updated `ConteudoQueryService.cs` and `ObterConteudoHandlerTests.cs` to fix compilation after `ConteudoDetalheData` record signature changed (Rule 3 — blocking issue). Stubs used for Categorias/Relacoes/Sessoes in query service (empty lists; will be wired in Plan 02/03).
- Pre-existing MSBuild MSB3277 warnings (EF Core version conflict in integration tests) exist but are unrelated to this plan's changes.
