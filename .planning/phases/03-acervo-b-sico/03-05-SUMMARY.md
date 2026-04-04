# Plan 03-05 Summary

**Status:** DONE
**Completed:** 2026-04-03T15:30:00Z

## What Was Built

- `BuscarCategoriasQuery/Handler`: autocomplete for categories by prefix
- `BuscarConteudosQuery/Handler`: content search for relation form
- `BuscarTiposRelacaoQuery/Handler`: relation type autocomplete
- `TipoRelacaoDto`: DTO for type autocomplete results
- `CategoriaChipViewModel`: chip display with automatic category tint + asterisk (D-08/D-11)
- `RelacaoItemViewModel`: relation list items
- `ConteudoDetalheViewModel` enriched with:
  - `IsGostei`/`IsNaoGostei`: three-state toggle properties (D-07/D-09)
  - `ClassificacaoTexto`: accessibility label for Classificação
  - `LimparNota` command: clears nota field
  - `EstadoProgresso` + `PosicaoAtual` as editable fields (now fully wired)
  - `CategoriasAssociadas` ObservableCollection of CategoriaChipViewModel
  - `Relacoes` ObservableCollection of RelacaoItemViewModel
  - `SugestoesCategoria`, `SugestoesConteudo`, `SugestoesTipoRelacao` for AutoCompleteBox
  - Category commands: `RemoverCategoria`, `SelecionarCategoriaAsync`, `PopularSugestoesCategoriasAsync`
  - Relation commands: `MostrarFormularioRelacao`, `CancelarFormularioRelacao`, `VincularConteudosAsync`, `RemoverRelacaoAsync`
  - Updated `IsDirty` includes Classificacao, Nota, EstadoProgresso, PosicaoAtual, category set
- Avaliação Expander wired: NumericUpDown [0-10], 👍/👎 ToggleButtons, Progresso ComboBox + TextBox
- Organização Expander wired: category chips WrapPanel + AutoCompleteBox + relations list + inline add form
- AutoCompleteBox Populating events wired in code-behind for async suggestions

## Files Modified

- `src/Modules/Module.Acervo/DTOs/TipoRelacaoDto.cs` — **created**
- `src/Modules/Module.Acervo/Queries/BuscarCategoriasQuery.cs` — **created**
- `src/Modules/Module.Acervo/Queries/BuscarConteudosQuery.cs` — **created**
- `src/Modules/Module.Acervo/Queries/BuscarTiposRelacaoQuery.cs` — **created**
- `src/DiarioDeBordo.UI/ViewModels/CategoriaChipViewModel.cs` — **created**
- `src/DiarioDeBordo.UI/ViewModels/RelacaoItemViewModel.cs` — **created**
- `src/DiarioDeBordo.UI/ViewModels/ConteudoDetalheViewModel.cs` — major additions
- `src/DiarioDeBordo.UI/Views/ConteudoDetalheWindow.axaml` — Avaliação + Organização sections wired
- `src/DiarioDeBordo.UI/Views/ConteudoDetalheWindow.axaml.cs` — AutoCompleteBox event wiring

## Tests

- 130 domain tests still passing
- Build: 0 warnings, 0 errors

## Deviations

- `SelecionarCategoriaAsync` creates/matches categories via BuscarCategoriasQuery; inline creation (ObterOuCriarAsync) deferred to save flow
- AutoCompleteBox uses `FilterMode="None"` with async Populating events (not client-side Contains filtering) — better UX for large datasets
- `PodeVincular` includes `NomeInversoNovoTipo` check for new-type-creation flow
