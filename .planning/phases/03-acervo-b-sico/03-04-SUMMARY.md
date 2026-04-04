# Plan 03-04 Summary

**Status:** DONE
**Completed:** 2026-04-03T14:15:00Z

## What Was Built

- `IDialogService` interface with `MostrarConteudoDetalheAsync` and `MostrarConfirmacaoAsync`
- `DialogService` (internal, DI-injected singleton) opening modals as owned child windows
- `ConfirmacaoDialog.axaml` + code-behind: reusable confirmation dialog (SizeToContent, configurable buttons, destructive style)
- `ConfirmacaoDialogViewModel`: pure data ViewModel for confirmation dialog
- `ConteudoDetalheWindow.axaml`: 4 Expander sections (Identificação expanded, Avaliação/Organização/Histórico collapsed), fixed footer with Excluir/Cancelar/Salvar buttons
- `ConteudoDetalheWindow.axaml.cs`: OnClosing intercept for dirty state, OnOpened triggers CarregarAsync
- `ConteudoDetalheViewModel`: dirty tracking (IsDirty, TituloJanela with '•' suffix), CarregarAsync, SalvarAsync, ExcluirAsync, CancelarAsync, expander summary properties
- `FormatosMidia` static enum list for ComboBox binding
- Enhanced AcervoView card: 14px title, metadata row (subtipo, nota), action buttons 👁 + 🗑 always visible at 0.6 opacity
- `AcervoViewModel`: IDialogService injection, AbrirDetalheAsync, ExcluirConteudoAsync with confirmation, pagination (PaginaAnterior/Proxima), StatusText
- Pagination controls (← Anterior / Próxima →) in AcervoView status bar
- DI registration of DialogService, ConteudoDetalheViewModel factory in App.axaml.cs
- InternalsVisibleTo wiring for cross-project access

## Files Modified

- `src/DiarioDeBordo.UI/Services/IDialogService.cs` — **created**
- `src/DiarioDeBordo.UI/Services/DialogService.cs` — **created**
- `src/DiarioDeBordo.UI/ViewModels/ConfirmacaoDialogViewModel.cs` — **created**
- `src/DiarioDeBordo.UI/ViewModels/ConteudoDetalheViewModel.cs` — **created**
- `src/DiarioDeBordo.UI/Views/ConfirmacaoDialog.axaml` — **created**
- `src/DiarioDeBordo.UI/Views/ConfirmacaoDialog.axaml.cs` — **created**
- `src/DiarioDeBordo.UI/Views/ConteudoDetalheWindow.axaml` — **created**
- `src/DiarioDeBordo.UI/Views/ConteudoDetalheWindow.axaml.cs` — **created**
- `src/DiarioDeBordo.UI/Views/AcervoView.axaml` — enhanced card template, pagination controls
- `src/DiarioDeBordo.UI/ViewModels/AcervoViewModel.cs` — IDialogService, new commands, pagination
- `src/DiarioDeBordo.UI/DiarioDeBordo.UI.csproj` — InternalsVisibleTo(DiarioDeBordo)
- `src/Modules/Module.Acervo/DiarioDeBordo.Module.Acervo.csproj` — InternalsVisibleTo(DiarioDeBordo.UI)
- `src/DiarioDeBordo.Desktop/App.axaml.cs` — DI registrations

## Tests

- 130 domain tests still passing
- Build: 0 warnings (C#), 0 errors

## Deviations

- Avaliação, Organização, Histórico sections have placeholder text (per plan: "filled in Plan 05-06")
- ConteudoDetalheViewModel constructor uses `Owner` property instead of passing Window reference — avoids circular dependency
- DialogService uses `internal sealed class` with `InternalsVisibleTo(DiarioDeBordo)` instead of making it public (better encapsulation)
- Strings class kept `internal`, InternalsVisibleTo added to Module.Acervo for UI project (CA1707 compliance)
