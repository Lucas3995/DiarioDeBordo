---
phase: "02"
plan: "08"
subsystem: "ui"
tags: [avalonia, mvvm, ux, wcag, navigation, walking-skeleton]
dependency_graph:
  requires: [02-05, 02-06, 02-07]
  provides: [navigation-shell, acervo-view, create-content-form, viewmodel-layer]
  affects: [DiarioDeBordo.Desktop, DiarioDeBordo.UI]
tech_stack:
  added:
    - Avalonia 11.2.4 AXAML views (MainWindow, AcervoView, CriarConteudoView)
    - CommunityToolkit.Mvvm 8.4.0 (ObservableProperty, RelayCommand, ObservableObject)
    - MediatR 12.4.1 in UI layer (IMediator injected into ViewModels)
    - FluentTheme (Avalonia.Themes.Fluent 11.2.4) replacing SukiUI for theming
    - Two-phase DI startup in App.axaml.cs (pre-container + full container)
  patterns:
    - MVVM with factory-pattern ViewModel creation (Func<T> in DI)
    - Progressive disclosure UX (D-03): CriarConteudoView shows title only by default
    - Event-based ViewModel communication (ConteudoCriado, Cancelado events)
    - DataTemplate-based ViewModel → View resolution via Application.DataTemplates
key_files:
  created:
    - src/DiarioDeBordo.Desktop/App.axaml
    - src/DiarioDeBordo.Desktop/App.axaml.cs
    - src/DiarioDeBordo.UI/ViewModels/MainViewModel.cs
    - src/DiarioDeBordo.UI/ViewModels/AcervoViewModel.cs
    - src/DiarioDeBordo.UI/ViewModels/CriarConteudoViewModel.cs
    - src/DiarioDeBordo.UI/Views/MainWindow.axaml
    - src/DiarioDeBordo.UI/Views/MainWindow.axaml.cs
    - src/DiarioDeBordo.UI/Views/AcervoView.axaml
    - src/DiarioDeBordo.UI/Views/AcervoView.axaml.cs
    - src/DiarioDeBordo.UI/Views/CriarConteudoView.axaml
    - src/DiarioDeBordo.UI/Views/CriarConteudoView.axaml.cs
  modified:
    - src/DiarioDeBordo.Core/Infraestrutura/IPostgresBootstrap.cs (added BuildConnectionStringAsync)
    - src/DiarioDeBordo.Desktop/Program.cs (replaced placeholder with Avalonia entry point)
    - src/DiarioDeBordo.Desktop/DiarioDeBordo.Desktop.csproj (added Avalonia.Themes.Fluent)
    - src/DiarioDeBordo.UI/DiarioDeBordo.UI.csproj (added MediatR + Module.Acervo reference)
decisions:
  - SukiUI 2.1.0 has no SukiWindow — it is a theme-only package built for Avalonia 0.10.12 with only Loading and CircleProgressBar controls; replaced with standard Avalonia Window + FluentTheme
  - BuildConnectionStringAsync added to IPostgresBootstrap interface — required for App.axaml.cs two-phase DI startup; was previously internal to PostgresBootstrap only
  - AcervoViewModel does NOT implement INotificationHandler<ConteudoCriadoNotification> — transient DI registration means MediatR would create a disconnected instance; event-based refresh (ConteudoCriado C# event) used instead
  - Func<AcervoViewModel> and Func<CriarConteudoViewModel> registered in DI — allows factory-pattern creation of new ViewModel instances per navigation/action
  - ProgressBar IsIndeterminate="True" used instead of suki:Loading — SukiUI 2.1.0 Loading control may be incompatible with Avalonia 11.x at runtime
  - CA1515 resolved by making App class internal — Avalonia AXAML loader works with internal App class in same assembly
metrics:
  duration_minutes: 45
  completed_date: "2026-04-03"
  tasks_completed: 3
  files_changed: 15
---

# Phase 02 Plan 08: UI Shell — Navigation, MainViewModel, Content List, Create Form — Summary

**One-liner:** Avalonia MVVM navigation shell with 4-item sidebar, AcervoView empty-state list, and progressive-disclosure CriarConteudoView — FluentTheme replacing SukiUI (2.1.0 theme-only, no SukiWindow).

## What Was Built

### Task 8.1: Application Entry Point and DI Setup

- **Program.cs**: `[STAThread] static int Main(string[] args)` using `App.BuildAvaloniaApp().StartWithClassicDesktopLifetime(args)`
- **App.axaml.cs**: Two-phase DI startup: Phase 1 creates minimal pre-container (logging + PostgresBootstrap); Phase 2 calls `EnsureRunningAsync` + `BuildConnectionStringAsync`; Phase 3 builds full container with `AddInfrastructure(connectionString)` + MediatR + ViewModels
- **App.axaml**: `FluentTheme` + `Application.DataTemplates` mapping `AcervoViewModel → AcervoView`, `CriarConteudoViewModel → CriarConteudoView`, `PlaceholderViewModel → placeholder TextBlock`
- **Desktop.csproj**: Added `Avalonia.Themes.Fluent 11.2.4`

### Task 8.2: Navigation Shell

- **MainViewModel**: 4-item sidebar navigation (`Acervo`, `Agregação`, `Busca`, `Configurações`). Starts at Acervo. Non-acervo items show `PlaceholderViewModel`. Uses `Func<AcervoViewModel>` factory via DI.
- **NavigationItem**: Exposes `IRelayCommand NavegarCommand` bound to sidebar buttons. `PlaceholderViewModel` shows section name + "disponível em versão futura" message.
- **MainWindow.axaml**: `<Window>` with `Grid ColumnDefinitions="220,*"`. Sidebar with ItemsControl over navigation items. ContentControl bound to `ActiveViewModel` resolved by DataTemplates.
- **UI.csproj**: Added `MediatR 12.4.1` + `Module.Acervo` ProjectReference.

### Task 8.3: Acervo Screen

- **AcervoViewModel**: `[RelayCommand] CarregarAsync()` sends `ListarConteudosQuery`. `AbrirFormularioCriar` creates `CriarConteudoViewModel` via factory, wires events. `FecharFormularioCriar` unwires events (prevents memory leaks). `IsEmpty` computed from `!IsLoading && Conteudos.Count == 0`.
- **CriarConteudoViewModel**: `[ObservableProperty] Titulo`, `Descricao`, `Anotacoes`, `MostrarDetalhes`. `TextoBotaoDetalhes` computed. `SalvarCommand` has `CanExecute = PodeSalvar`. Fires `ConteudoCriado` and `Cancelado` events.
- **AcervoView.axaml**: Header with `+ Adicionar` button; progressive-disclosure form (ContentControl bound to `CriarConteudoViewModel`); ScrollViewer with ItemsControl showing `ConteudoResumoDto.Titulo + Formato`; empty-state StackPanel with narrative text; ProgressBar `IsIndeterminate` loading indicator; status bar.
- **CriarConteudoView.axaml**: Título TextBox (always visible); error TextBlock; `ToggleDetalhesCommand` button showing `TextoBotaoDetalhes`; collapsible Descrição + Anotações TextBoxes; Cancelar + Adicionar buttons. All interactive controls have `AutomationProperties.Name` (WCAG 2.2 AAA).

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 2 - Missing] `BuildConnectionStringAsync` not in `IPostgresBootstrap` interface**
- **Found during:** Task 8.1
- **Issue:** Plan's `App.axaml.cs` calls `bootstrap.BuildConnectionStringAsync()` but the method was not declared in `IPostgresBootstrap` interface (it was an "internal helper" per design note in 02-07 STATE.md). The plan required it accessible through the interface for DI-resolved bootstrap usage.
- **Fix:** Added `Task<string> BuildConnectionStringAsync(CancellationToken ct)` to `IPostgresBootstrap` interface. The `PostgresBootstrap` implementation already had the method; the interface just needed to expose it.
- **Files modified:** `src/DiarioDeBordo.Core/Infraestrutura/IPostgresBootstrap.cs`
- **Commit:** f4413ef

**2. [Rule 3 - Blocking] SukiUI 2.1.0 incompatible with plan's AXAML — no SukiWindow, no SukiTheme class**
- **Found during:** Task 8.1 analysis (pre-implementation)
- **Issue:** The plan specified `<suki:SukiWindow>`, `<suki:SukiTheme ThemeColor="Blue" />`, and `<suki:Loading>`. Analysis of `SukiUI.dll` from NuGet cache revealed SukiUI 2.1.0 was built for Avalonia 0.10.12 (not 11.x). The only Avalonia resources in the package are `SukiUI.Controls.Loading`, `SukiUI.Controls.CircleProgressBar`, and `SukiUI.MessageBox.MessageBox`. No `SukiWindow` or `SukiTheme` class exists.
- **Fix:**
  - Replaced `<suki:SukiWindow>` with standard Avalonia `<Window>` in MainWindow.axaml
  - Replaced `<suki:SukiTheme>` with `<FluentTheme />` (from `Avalonia.Themes.Fluent 11.2.4`) in App.axaml
  - Replaced `<suki:Loading>` with `<ProgressBar IsIndeterminate="True">` in AcervoView.axaml
  - Added `Avalonia.Themes.Fluent 11.2.4` to Desktop.csproj
- **Files modified:** App.axaml, MainWindow.axaml, AcervoView.axaml, Desktop.csproj
- **Commit:** f4413ef + 228896c

**3. [Rule 2 - Missing] CA1515 analyzer: App class must not be public in application assembly**
- **Found during:** Task 8.1 build
- **Issue:** `AnalysisMode=All` enforces CA1515 — application types should be `internal` not `public`. `App` was declared `public partial class`.
- **Fix:** Changed to `internal sealed partial class App`. Avalonia AXAML loader resolves classes by reflection within the same assembly — `internal` works correctly.
- **Files modified:** `src/DiarioDeBordo.Desktop/App.axaml.cs`
- **Commit:** f4413ef

**4. [Rule 1 - Bug] `NavigationItem` missing null guard + CA1308 (ToLowerInvariant) on SectionKey**
- **Found during:** Task 8.2 build
- **Issue:** CA1308 (use ToUpperInvariant instead) + CA1062 (validate `titulo` parameter). SectionKey derivation from `Titulo.ToLowerInvariant()` triggered both.
- **Fix:** Removed `SectionKey` from `NavigationItem` (not needed — navigation is driven by the captured `Action` closure, not a key lookup). Added `ArgumentNullException.ThrowIfNull` for `titulo` and `navegar` parameters. Navigation switch in `MainViewModel.NavigateTo` now directly uses string literals.
- **Files modified:** `src/DiarioDeBordo.UI/ViewModels/MainViewModel.cs`
- **Commit:** 228896c

**5. [Rule 2 - Missing] `INotificationHandler<ConteudoCriadoNotification>` on AcervoViewModel removed**
- **Found during:** Task 8.3 design
- **Issue:** The plan included MediatR notification handler on `AcervoViewModel`. However, `AcervoViewModel` is registered as `AddTransient` — MediatR creates a NEW instance per notification, disconnected from the displayed UI. This would be a silent bug.
- **Fix:** Removed `INotificationHandler` implementation. Used only C# event-based refresh (`ConteudoCriado` event from `CriarConteudoViewModel` → `OnConteudoCriadoAsync` in `AcervoViewModel`).
- **Files modified:** `src/DiarioDeBordo.UI/ViewModels/AcervoViewModel.cs`

## Architecture Decisions Made

| Decision | Rationale |
|----------|-----------|
| FluentTheme over SukiUI theming | SukiUI 2.1.0 built for Avalonia 0.10.12; SukiWindow/SukiTheme don't exist in package |
| `Func<T>` factory in DI | Allows new ViewModel instances per navigation without service locator anti-pattern |
| Event-based AcervoViewModel refresh | MediatR notification handler on transient VM creates disconnected instance — C# events work correctly |
| `internal sealed partial class App` | CA1515 compliance; Avalonia AXAML loader uses reflection, works with internal types in same assembly |

## Build Results

- `dotnet build src/DiarioDeBordo.Desktop --configuration Release`: **0 errors, 1 MSB warning** (MSB3277: EF version conflict between Microsoft.Extensions.Hosting transitive dep and Infrastructure EFCore 9.0.4 — non-critical, MSBuild warning not treated as error by C# TreatWarningsAsErrors)
- `dotnet build DiarioDeBordo.sln --configuration Release`: **0 errors, 3 MSB warnings**

## Known Stubs

- **Hard-coded UsuarioId**: `Guid.Parse("00000000-0000-0000-0000-000000000001")` in `AcervoViewModel` and `CriarConteudoViewModel`. This is intentional for Phase 2 (walking skeleton — no auth). Will be replaced in Phase 8 (authentication).
- **PlaceholderViewModel**: Agregação, Busca, Configurações sections show placeholder text. Intentional — these modules are implemented in Phases 5, 6, 10.

## Self-Check: PASSED

| Item | Result |
|------|--------|
| `App.axaml` created | ✓ FOUND |
| `App.axaml.cs` created | ✓ FOUND |
| `MainWindow.axaml` created | ✓ FOUND |
| `AcervoView.axaml` created | ✓ FOUND |
| `CriarConteudoView.axaml` created | ✓ FOUND |
| `MainViewModel.cs` created | ✓ FOUND |
| `AcervoViewModel.cs` created | ✓ FOUND |
| `CriarConteudoViewModel.cs` created | ✓ FOUND |
| Commit f4413ef | ✓ FOUND |
| Commit 228896c | ✓ FOUND |
| Build: 0 C# errors | ✓ VERIFIED |
