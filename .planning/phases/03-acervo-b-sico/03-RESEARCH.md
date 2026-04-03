# Phase 3: Acervo Básico - Research

**Researched:** 2026-04-03
**Domain:** EF Core bidirectional relations, Avalonia UI modal/dialog patterns, DDD aggregate enrichment, progressive disclosure
**Confidence:** HIGH

## Summary

Phase 3 transforms the walking skeleton into a usable daily-driver content management system. The core challenges are: (1) implementing bidirectional relations with typed relation kinds and automatic inverse creation in EF Core, (2) building a modal-based detail/edit form in Avalonia with progressive disclosure, (3) modeling consumption sessions as child Conteudo with a special relation type, and (4) filtering child content from the main listing.

The project already has strong established patterns: `Resultado<T>` ROP, MediatR CQRS, `CategoriaRepository` for case-insensitive dedup, `CriarConteudoHandler` for domain factory + event publishing, and `ConteudoConfiguration` for EF Core entity mapping. Phase 3 extends these patterns — no new frameworks or paradigms needed. The main risk is the bidirectional relation complexity (creating both directions atomically) and the UI complexity of the modal with multiple sections.

**Primary recommendation:** Follow the existing `Categoria` pattern exactly for `TipoRelacao` (entity with `NomeNormalizado`, `ObterOuCriarAsync`, case-insensitive unique index). Model `Relacao` as a separate entity outside the Conteudo aggregate (as documented in `docs/domain/acervo.md`), with an application service creating both directions in a single transaction. Use Avalonia `Window.ShowDialog<T>()` for the detail modal, not a custom popup. Use `AutoCompleteBox` (built-in Avalonia control since v0.10) for both categories and relation type selection.

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions
- **D-01:** Detalhe e edição via **modal popup** — não view dedicada, não painel lateral
- **D-02:** Salvar: botões Salvar + Cancelar explícitos. Confirmação de descarte se mudanças não salvas
- **D-03:** Excluir em dois lugares: botão no modal + ícone no card da lista. Ambos exigem confirmação
- **D-04:** Acesso ao modal via botão explícito 'Ver detalhe' no card — não clique no card inteiro
- **D-05:** Layout do modal: Claude's Discretion com **obrigação de pesquisa em literatura científica**
- **D-06/D-07/D-08:** Nota e Classificação são campos independentes. `Classificacao` = enum `{Nulo, Gostei, NaoGostei}`. Null = não classificado. Combinações como "Nota 9 + Não gostei" são válidas
- **D-09/D-10/D-11:** Categorias como chips inline, autocomplete, criação inline, distinção manual vs automática (cor + asterisco)
- **D-12/D-13/D-14:** TipoRelacao = tipos pré-definidos + criação user-defined com autocomplete. Cada tipo tem nome de ida + nome inverso. 8 pré-definidos (Sequência↔Continuação de, etc.)
- **D-15/D-16:** Bidirecionalidade automática: criar A→B cria B←A automaticamente. Múltiplas relações do mesmo tipo permitidas
- **D-17/D-18:** Sessões = Conteúdos filhos com relação "Contém"/"Parte de". Timeline cronológica no modal do pai
- **D-19:** Conteúdos filhos ocultos da lista principal
- **D-20:** Miniformulário de sessão: apenas título/posição + data obrigatórios. Disclosure progressivo para os demais
- **D-21:** Progresso calculado: `filhosConsumidos / totalEsperado * 100%`. TotalEsperado opcional

### Claude's Discretion
- Organização interna do modal (D-05): layout, tabs vs scroll — pesquisa científica obrigatória
- Representação visual da timeline de sessões filhas
- Animações e transições no modal
- Comportamento do autocomplete (debounce, prefixo mínimo, número de sugestões)

### Deferred Ideas (OUT OF SCOPE)
- Importação em bulk de sessões — Phase 10/11
- Heatmap/calendário de progresso estilo GitHub — Phase 11
- Estatísticas de consumo — Phase 11
- Exportação do histórico de sessões — Phase 10
</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|------------------|
| ACE-01 | Entidade Conteúdo com dois eixos: formato de mídia + papel estrutural | Already implemented in Conteudo.cs (FormatoMidia, PapelConteudo). Phase 3 adds Classificacao enum + TotalEsperadoSessoes. Migration needed |
| ACE-02 | Atributos completos: título, descrição, anotações, nota, classificação, progresso, histórico de consumo | Nota/Progresso/Descricao/Anotacoes exist. Add: Classificacao field, TotalEsperadoSessoes, consumption sessions as child Conteudo entities. See Architecture Patterns §1-3 |
| ACE-03 | Categorias como tags livres com autocompletar e não-duplicação | CategoriaRepository fully implemented (case-insensitive dedup, autocomplete). Phase 3 adds UI integration: AutoCompleteBox + chips display |
| ACE-04 | Relações entre conteúdos com bidirecionalidade e tipos de relação | New entities: TipoRelacao, Relacao. Application service for atomic bidirectional creation. See Architecture Patterns §4-5 |
| ACE-09 | Paginação obrigatória em todas as listagens | Already enforced (PaginacaoParams, PaginatedList<T>). Extend to new queries (sessões, relações) |
</phase_requirements>

## Standard Stack

### Core (Already in Project)
| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| Avalonia | 11.2.4 | UI framework | Only .NET framework with real Linux+Windows support |
| Avalonia.Themes.Fluent | 11.2.4 | FluentTheme | Established in Phase 2 (SukiUI abandoned) |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM bindings | Source generators for ObservableProperty, RelayCommand |
| MediatR | 12.4.1 | CQRS + Events | Commands/Queries/Notifications pattern |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.4 | EF Core PostgreSQL | Persistence layer |
| xUnit + NSubstitute | latest/5.3.0 | Testing | Domain + integration tests |

### No New Packages Required
This phase requires no additional NuGet packages. All needed UI controls (`AutoCompleteBox`, `Window`, `Expander`, `ToggleButton`, `ItemsRepeater`) are built into Avalonia 11.2.4. No third-party component library needed.

## Architecture Patterns

### Recommended Project Structure (New Files for Phase 3)
```
src/
├── DiarioDeBordo.Core/
│   ├── Entidades/
│   │   ├── TipoRelacao.cs          # NEW: entity with Nome, NomeInverso, NomeNormalizado
│   │   └── Relacao.cs              # NEW: entity with ConteudoOrigemId, ConteudoDestinoId, TipoRelacaoId
│   ├── Enums/
│   │   └── Enums.cs                # ADD: Classificacao enum (Nulo, Gostei, NaoGostei)
│   ├── Repositorios/
│   │   ├── ITipoRelacaoRepository.cs  # NEW: ObterOuCriarAsync + autocomplete pattern
│   │   └── IRelacaoRepository.cs      # NEW: CRUD + ObterPorConteudoAsync
│   └── Consultas/
│       └── IConteudoQueryService.cs   # MODIFY: add detalhe with relations/categories/sessions
├── DiarioDeBordo.Infrastructure/
│   ├── Persistencia/
│   │   ├── Configuracoes/
│   │   │   ├── ConteudoConfiguration.cs   # MODIFY: add Classificacao, TotalEsperadoSessoes, IsFilho
│   │   │   ├── TipoRelacaoConfiguration.cs # NEW
│   │   │   ├── RelacaoConfiguration.cs     # NEW
│   │   │   └── ConteudoCategoriaConfiguration.cs # NEW (join table)
│   │   ├── Migrations/
│   │   │   └── YYYYMMDD_AddRelacoesCategoriasSessoes.cs # NEW migration
│   │   └── DiarioDeBordoDbContext.cs  # MODIFY: add DbSets
│   ├── Repositorios/
│   │   ├── TipoRelacaoRepository.cs  # NEW
│   │   └── RelacaoRepository.cs      # NEW
│   └── Consultas/
│       └── ConteudoQueryService.cs    # MODIFY: enrich queries
├── Modules/Module.Acervo/
│   ├── Commands/
│   │   ├── EditarConteudoCommand.cs     # NEW
│   │   ├── RemoverConteudoCommand.cs    # NEW
│   │   ├── CriarRelacaoCommand.cs       # NEW: creates both directions atomically
│   │   ├── RemoverRelacaoCommand.cs     # NEW: removes both directions
│   │   ├── RegistrarSessaoCommand.cs    # NEW: creates child Conteudo + Relação "Contém"
│   │   └── DefinirClassificacaoCommand.cs # NEW
│   ├── Queries/
│   │   └── ObterConteudoDetalheQuery.cs # NEW: full detalhe with relations/cats/sessions
│   ├── DTOs/
│   │   ├── ConteudoDetalheDto.cs        # MODIFY: add categories, relations, sessions, classificação
│   │   └── RelacaoDto.cs                # NEW
│   └── Resources/
│       └── Strings.pt-BR.resx          # MODIFY: add modal labels
├── DiarioDeBordo.UI/
│   ├── Views/
│   │   ├── ConteudoDetalheWindow.axaml  # NEW: modal window
│   │   └── AcervoView.axaml            # MODIFY: add detail button + delete button on card
│   └── ViewModels/
│       ├── ConteudoDetalheViewModel.cs  # NEW: complex VM with sections
│       └── AcervoViewModel.cs           # MODIFY: open modal, handle delete
└── tests/
    ├── Tests.Domain/Acervo/
    │   ├── ClassificacaoTests.cs           # NEW
    │   ├── RelacaoBidirecionalTests.cs     # NEW
    │   ├── TipoRelacaoDeduplicacaoTests.cs # NEW
    │   ├── SessaoProgressoCalculadoTests.cs # NEW
    │   └── ConteudoFilhoFlagTests.cs       # NEW
    └── Tests.Integration/Repositorios/
        ├── TipoRelacaoRepositoryTests.cs   # NEW
        └── RelacaoRepositoryTests.cs       # NEW
```

### Pattern 1: Classificacao Enum Addition
**What:** Add `Classificacao` enum and nullable field to `Conteudo`
**When to use:** D-06/D-08: three-state classification independent of numeric Nota
**Confidence:** HIGH — follows existing enum pattern (FormatoMidia, PapelConteudo)

```csharp
// In Enums.cs — add to existing file
public enum Classificacao { Nulo, Gostei, NaoGostei }

// In Conteudo.cs — add property + operation
public Classificacao? Classificacao { get; private set; }

public void DefinirClassificacao(Classificacao? classificacao)
{
    Classificacao = classificacao;
    AtualizadoEm = DateTimeOffset.UtcNow;
}

// In ConteudoConfiguration.cs — EF Core mapping
builder.Property(c => c.Classificacao)
    .HasColumnName("classificacao")
    .HasConversion<string>()
    .HasMaxLength(50);
```

**Note on `Nulo` vs null:** The CONTEXT.md says `Null` represents "não classificado" (D-08). Two design options: (a) use nullable enum `Classificacao?` where `null` = not classified, and enum has only `Gostei` + `NaoGostei`; or (b) use non-nullable enum with `Nulo` value. Recommendation: use **nullable `Classificacao?`** with only `Gostei` and `NaoGostei` in the enum — this is idiomatic C# and avoids representing "no data" as a data value. The technical standards doc (§6.1) already shows `Classificacao?` as nullable.

### Pattern 2: TipoRelacao Entity (Follows Categoria Pattern)
**What:** Entity with predefined seeds + user creation, case-insensitive dedup
**When to use:** D-12/D-13/D-14
**Confidence:** HIGH — mirrors `Categoria` pattern already implemented

```csharp
// Core/Entidades/TipoRelacao.cs
public sealed class TipoRelacao
{
    public Guid Id { get; init; }
    public Guid UsuarioId { get; init; }
    public required string Nome { get; init; }           // "Sequência"
    public required string NomeInverso { get; init; }    // "Continuação de"
    public required string NomeNormalizado { get; init; } // "sequência" — lowercase
    public bool IsSistema { get; init; }                  // true for pre-defined, false for user-created

    public static TipoRelacao Criar(Guid usuarioId, string nome, string nomeInverso, bool isSistema = false)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("NOME_TIPO_RELACAO_OBRIGATORIO", "Nome do tipo de relação é obrigatório.");
        if (string.IsNullOrWhiteSpace(nomeInverso))
            throw new DomainException("NOME_INVERSO_OBRIGATORIO", "Nome inverso do tipo de relação é obrigatório.");

        return new TipoRelacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Nome = nome.Trim(),
            NomeInverso = nomeInverso.Trim(),
            NomeNormalizado = nome.Trim().ToLowerInvariant(),
            IsSistema = isSistema,
        };
    }
}

// Core/Repositorios/ITipoRelacaoRepository.cs
public interface ITipoRelacaoRepository
{
    Task<TipoRelacao> ObterOuCriarAsync(Guid usuarioId, string nome, string nomeInverso, CancellationToken ct);
    Task<IReadOnlyList<TipoRelacao>> ListarComAutocompletarAsync(Guid usuarioId, string prefixo, CancellationToken ct);
    Task<TipoRelacao?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct);
}
```

**Seed strategy:** Use EF Core migration `HasData()` or a seeder invoked at startup to insert the 8 pre-defined types (D-13). The seeder must be idempotent (check `NomeNormalizado` before inserting). Seed uses a well-known `UsuarioId = Guid.Empty` or a special system user, then each user's first access copies system types — OR use `IsSistema = true` flag and query both system and user-created types. **Recommendation:** use `IsSistema = true` with `UsuarioId = Guid.Empty` for seeds; queries filter by `UsuarioId == userId || IsSistema == true`.

### Pattern 3: Relacao Entity — Bidirectional with Atomic Creation
**What:** Junction entity with two FKs to Conteudo, FK to TipoRelacao. Two rows per logical relation (A→B with type's Nome, B→A with type's NomeInverso).
**When to use:** D-15/D-16
**Confidence:** HIGH — documented approach in `docs/domain/acervo.md` line 326

```csharp
// Core/Entidades/Relacao.cs
public sealed class Relacao
{
    public Guid Id { get; init; }
    public Guid ConteudoOrigemId { get; init; }
    public Guid ConteudoDestinoId { get; init; }
    public Guid TipoRelacaoId { get; init; }
    public bool IsInversa { get; init; } // false = direction as typed, true = inverse direction
    public Guid? ParId { get; init; }    // links the two sides of a bidirectional relation
    public Guid UsuarioId { get; init; }
    public DateTimeOffset CriadoEm { get; init; }
}

// Application service (in CriarRelacaoHandler):
public async Task<Resultado<Guid>> Handle(CriarRelacaoCommand cmd, CancellationToken ct)
{
    // 1. Validate: ConteudoOrigemId != ConteudoDestinoId (no self-reference)
    // 2. Validate: exact duplicate check (same pair + same type)
    // 3. Create forward relation (A→B, IsInversa=false)
    // 4. Create inverse relation (B→A, IsInversa=true, ParId=forward.Id)
    // 5. SaveChangesAsync — single transaction guarantees atomicity
}
```

**Key design decision — `IsInversa` + `ParId` pattern:**
- Each logical relation creates exactly 2 rows in the `relacoes` table
- `ParId` links the pair so deleting one side deletes both
- When querying for A's relations: `WHERE ConteudoOrigemId = A.Id` — gets both directions naturally
- Display: `IsInversa == false` → show `TipoRelacao.Nome`; `IsInversa == true` → show `TipoRelacao.NomeInverso`
- This is simpler than trying to query both `ConteudoOrigemId = X OR ConteudoDestinoId = X` with a single row per relation

**EF Core constraints:**
```csharp
// RelacaoConfiguration.cs
builder.HasIndex(r => new { r.ConteudoOrigemId, r.ConteudoDestinoId, r.TipoRelacaoId })
    .IsUnique()
    .HasDatabaseName("idx_relacoes_origem_destino_tipo_unique");

builder.HasIndex(r => new { r.ConteudoOrigemId, r.UsuarioId })
    .HasDatabaseName("idx_relacoes_origem_usuario");

// CHECK constraint: ConteudoOrigemId != ConteudoDestinoId
// Added via raw SQL in migration:
// migrationBuilder.Sql("ALTER TABLE relacoes ADD CONSTRAINT chk_relacao_sem_auto_referencia CHECK (conteudo_origem_id != conteudo_destino_id)");
```

### Pattern 4: Sessions as Child Conteudo — "Contém"/"Parte de" Special Relation
**What:** Sessions are regular Conteudo entities linked to parent via a special TipoRelacao "Contém"/"Parte de"
**When to use:** D-17/D-18/D-19/D-20/D-21
**Confidence:** HIGH

```csharp
// The "Contém"/"Parte de" type is one of the 8 pre-defined TipoRelacao seeds
// Additional pre-defined type NOT in D-13 but required by D-17/D-18:
// "Contém" ↔ "Parte de"

// Conteudo gets new optional fields:
public int? TotalEsperadoSessoes { get; set; }    // D-21: optional expected session count
public bool IsFilho { get; private set; }          // D-19: true when created as session

// Session creation handler:
// 1. Create child Conteudo (IsFilho = true)
// 2. Create bidirectional Relacao with TipoRelacao "Contém"/"Parte de"
// 3. Single transaction
```

### Pattern 5: IsFilho Flag and List Filtering
**What:** Filter child Conteudo from main listing using `IsFilho` flag
**When to use:** D-19
**Confidence:** HIGH

**Two approaches considered:**

| Approach | Pros | Cons |
|----------|------|------|
| EF Core Global Query Filter | Automatic — all queries exclude children by default | Requires `.IgnoreQueryFilters()` to show children; impacts eager loading of relations; can cause subtle bugs |
| Explicit `WHERE IsFilho == false` in query service | Transparent — no hidden behavior; easy to understand | Must remember to add filter in every query |

**Recommendation: Explicit filter in `ConteudoQueryService`, NOT Global Query Filter.**

Rationale:
1. The project already filters by `UsuarioId` explicitly in every query (SEG-02 pattern). Adding `IsFilho == false` follows the same explicit pattern.
2. Global Query Filters interact poorly with Include/ThenInclude for relations — when loading a parent's sessions, the filter would exclude the very children we want to show.
3. Only `ListarAsync` needs the filter. `ObterAsync` (detail view) must show children when accessed via parent modal.
4. The technical standards (§5.7) emphasize explicit over implicit behavior.

```csharp
// In ConteudoQueryService.ListarAsync:
var items = await _context.Conteudos
    .Where(c => c.UsuarioId == usuarioId && !c.IsFilho) // SEG-02 + D-19
    .OrderByDescending(c => c.CriadoEm)
    // ...
```

### Pattern 6: Conteudo-Categoria Many-to-Many Join Table
**What:** Explicit join entity `ConteudoCategoria` for the M:N relationship
**When to use:** ACE-03 — linking categories to content
**Confidence:** HIGH

The existing code has `Categoria` as a separate aggregate (no navigation from Conteudo to Categoria). Phase 3 needs a join table:

```csharp
// Core/Entidades/ConteudoCategoria.cs (documented in docs/domain/acervo.md line 327)
public sealed class ConteudoCategoria
{
    public Guid ConteudoId { get; init; }
    public Guid CategoriaId { get; init; }
    public DateTimeOffset AssociadaEm { get; init; }
}

// Configuration: composite PK on (ConteudoId, CategoriaId)
// No navigation properties on Conteudo — queried via ConteudoQueryService
```

### Pattern 7: Modal Window in Avalonia 11
**What:** Use `Window.ShowDialog<T>()` for the detail/edit modal
**When to use:** D-01: detail/edit via modal popup
**Confidence:** HIGH — Avalonia 11 has built-in dialog support via Window

```csharp
// In AcervoViewModel (or code-behind via interaction service):
// Avalonia pattern: create a Window, set DataContext, show as dialog

// Service abstraction (MVVM-friendly):
public interface IDialogService
{
    Task<TResult?> ShowDialogAsync<TResult>(object viewModel);
}

// Implementation (in UI project):
internal sealed class DialogService : IDialogService
{
    private readonly Window _mainWindow;

    public async Task<TResult?> ShowDialogAsync<TResult>(object viewModel)
    {
        var window = new ConteudoDetalheWindow
        {
            DataContext = viewModel
        };
        return await window.ShowDialog<TResult?>(_mainWindow);
    }
}
```

**Why Window, not Popup:**
- `Popup` in Avalonia is a lightweight overlay (like a tooltip/dropdown) — no title bar, no modal behavior, no keyboard trap
- `Window.ShowDialog()` provides true modal behavior: blocks parent interaction, returns a result, has system title bar, handles Escape/close
- `ContentDialog` exists in Avalonia 11 (FluentTheme) for simple confirmation dialogs but not for complex forms

### Pattern 8: Progressive Disclosure in Modal — HCI Research
**What:** Layout organization of the detail/edit modal based on scientific literature
**When to use:** D-05 — mandatory research
**Confidence:** HIGH — well-established HCI principles

**Literature review (as required by D-05):**

1. **Shneiderman (1996, ACM SIGCHI Bulletin)** — "Progressive disclosure" reduces complexity by revealing options incrementally. Users perform better when forms show only essential fields initially. Implementation: collapsible sections (Avalonia `Expander` control).

2. **Norman (2013, "The Design of Everyday Things")** — Affordance and signifiers: users need clear visual cues that more content exists. Collapsed sections must show clear expand indicators (chevron icons ▸/▾). Never hide information without indicating its existence.

3. **Cooper et al. (2014, "About Face")** — "Sovereign posture" applications (main workspace apps) benefit from: primary data always visible, secondary data in expandable sections, action buttons positioned consistently (bottom-right for Western reading flow).

4. **Tidwell et al. (2020, "Designing Interfaces", O'Reilly)** — "Responsive Disclosure" pattern: show/hide UI sections based on context. Groups of related fields should be in visually distinct sections. Maximum 7±2 visible groups (Miller's Law applied to form sections).

5. **WCAG 2.2 AAA Success Criterion 3.3.2** — Labels or Instructions: all form fields must have visible labels. **SC 2.4.6** — Headings and Labels: section headings describe the topic. **SC 1.3.1** — Info and Relationships: structure must be programmatically determinable.

**Recommended modal layout (single scrollable view with collapsible sections):**

```
┌──────────────────────────────────────────────┐
│ [Título do Conteúdo]                   [×]   │  ← Always visible header
├──────────────────────────────────────────────┤
│ ┌─ Informações Básicas ──────────── (sempre) │  ← Section 1: always expanded
│ │  Título: [__________________]              │
│ │  Formato: [dropdown]  Subtipo: [________]  │
│ │  Classificação: [👍] [👎] [—]   Nota: [__] │
│ └────────────────────────────────────────────│
│ ▸ Descrição e Anotações                      │  ← Section 2: collapsed by default
│ ▸ Categorias                                 │  ← Section 3: collapsed (chips + autocomplete)
│ ▸ Relações                                   │  ← Section 4: collapsed (list + add)
│ ▸ Sessões de Consumo                         │  ← Section 5: collapsed (timeline + add)
│ ▸ Progresso                                  │  ← Section 6: collapsed (state + calculated %)
├──────────────────────────────────────────────┤
│                    [Excluir]  [Cancelar] [Salvar] │  ← Fixed footer
└──────────────────────────────────────────────┘
```

**Why single scrollable view with Expanders, not tabs:**
- Tabs hide content behind a click — violates Norman's affordance principle for a form where users need to see what data exists
- Scrollable view with Expanders: users see all section headers at once, can expand any section, knows what data categories exist
- Cooper: tab interfaces are better for truly independent views (settings categories), not for a single entity's attributes
- Shneiderman: progressive disclosure works best when the collapsed state shows enough to understand what's hidden (section name)

**Implementation:** Use Avalonia `Expander` control for each collapsible section. First section ("Informações Básicas") starts expanded; others collapsed by default. Footer with action buttons uses a fixed position at bottom of the Window (not inside ScrollViewer).

### Pattern 9: AutoCompleteBox in Avalonia 11
**What:** Built-in control for category and relation type selection with inline creation
**When to use:** D-09/D-10/D-12/D-14
**Confidence:** HIGH — AutoCompleteBox is a standard Avalonia control

```xml
<!-- Avalonia AutoCompleteBox usage -->
<AutoCompleteBox
    Watermark="Buscar ou criar categoria…"
    FilterMode="Contains"
    ItemsSource="{Binding SugestoesCategoria}"
    Text="{Binding TextoPesquisaCategoria}"
    AutomationProperties.Name="Campo de busca e criação de categorias"
    MinimumPrefixLength="1"
    MinimumPopulateDelay="00:00:00.300" />
```

**Key properties:**
- `FilterMode="Contains"` — matches anywhere in the string (not just prefix)
- `MinimumPrefixLength="1"` — start searching after 1 character
- `MinimumPopulateDelay` — built-in debounce (300ms recommended)
- `Populating` event — async callback to fetch suggestions from repository
- `ItemSelector` — controls what text is inserted when item selected
- `TextFilter` — custom filter predicate

**Inline creation pattern:**
When user types a name not in the suggestions list, show a special item "Criar '{nome}'" at the end of the dropdown. When selected, call `ObterOuCriarAsync`. Implemented via a custom `IBinding` or by adding a sentinel item to the filtered results.

### Pattern 10: Chips/Tags Display
**What:** Visual display of assigned categories as removable chips
**When to use:** D-09 — categories displayed as chips
**Confidence:** MEDIUM — no native "Chip" control in Avalonia; use ItemsRepeater + styled Border

Avalonia 11 does not have a dedicated "Chip" or "Tag" control. Use `ItemsRepeater` (or `ItemsControl`) with a custom `DataTemplate`:

```xml
<ItemsControl ItemsSource="{Binding CategoriasAssociadas}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <WrapPanel Orientation="Horizontal" />
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border CornerRadius="12"
                    Padding="8,4"
                    Margin="4,2"
                    Background="{DynamicResource SystemAccentColor}">
                <StackPanel Orientation="Horizontal" Spacing="4">
                    <TextBlock Text="{Binding Nome}" VerticalAlignment="Center" />
                    <Button Content="×"
                            Command="{Binding RemoverCommand}"
                            Padding="2"
                            AutomationProperties.Name="Remover categoria" />
                </StackPanel>
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

**D-11 distinction (manual vs automatic):** Use different `Background` colors via a converter that checks `Origem`. Automatic categories prefix with `*`.

### Pattern 11: Timeline Visual for Sessions
**What:** Chronological list of child sessions in the parent modal
**When to use:** D-18 — timeline cronológica visual
**Confidence:** MEDIUM — no native timeline control; use ItemsControl with custom template

Avalonia has no dedicated Timeline control. Implement as a styled `ItemsControl` with a vertical line connector:

```xml
<ItemsControl ItemsSource="{Binding SessoesFilhas}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Grid ColumnDefinitions="Auto,12,*" Margin="0,0,0,8">
                <!-- Timeline dot -->
                <Ellipse Grid.Column="0" Width="10" Height="10"
                         Fill="{DynamicResource SystemAccentColor}"
                         VerticalAlignment="Top" Margin="0,4,0,0" />
                <!-- Vertical line (connector) -->
                <Border Grid.Column="1" Width="2" Background="{DynamicResource SystemControlForegroundBaseLowBrush}"
                        VerticalAlignment="Stretch" HorizontalAlignment="Center" />
                <!-- Session card -->
                <Border Grid.Column="2" Padding="8" CornerRadius="4"
                        BorderThickness="1"
                        BorderBrush="{DynamicResource SystemControlForegroundBaseLowBrush}">
                    <StackPanel Spacing="4">
                        <TextBlock Text="{Binding Titulo}" FontWeight="SemiBold" />
                        <TextBlock Text="{Binding CriadoEm, StringFormat='dd/MM/yyyy'}" Opacity="0.6" FontSize="12" />
                    </StackPanel>
                </Border>
            </Grid>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### Pattern 12: Calculated Progress
**What:** Progress percentage computed from child sessions
**When to use:** D-21
**Confidence:** HIGH

**Recommendation: Compute in the query service, not in the domain entity.**

Rationale:
- `TotalEsperadoSessoes` lives on the parent `Conteudo`
- Child count requires a database query (`COUNT(*)` of children with Relacao "Contém")
- Making it a computed property on the entity would require loading all children
- A query-side projection is more efficient:

```csharp
// In ConteudoQueryService.ObterAsync — enhanced for Phase 3
var sessoesConsumidas = await _context.Relacoes
    .CountAsync(r => r.ConteudoOrigemId == id && r.TipoRelacaoId == tipoContemId && !r.IsInversa, ct);

var progressoCalculado = conteudo.TotalEsperadoSessoes.HasValue
    ? (decimal)sessoesConsumidas / conteudo.TotalEsperadoSessoes.Value * 100
    : (decimal?)null;

// Return: "12 sessões registradas" or "50% — 12 de 24"
```

### Anti-Patterns to Avoid
- **Global Query Filter for IsFilho:** Don't use — interacts poorly with relation loading and creates hidden behavior
- **Single-row bidirectional relations:** Don't try to model A↔B as one row with `OR` queries — creates complex, slow queries. Use two rows per logical relation
- **Loading all children to compute progress:** Don't load full Conteudo entities just to count them — use `COUNT()` projection
- **Tab-based modal:** Don't use tabs for a single entity's form — use scrollable Expander sections
- **Hardcoded TipoRelacao names in code:** Don't match relation types by string — use `TipoRelacao.Id` references

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Case-insensitive dedup | Custom string comparison | `NomeNormalizado` + UNIQUE index (Categoria pattern) | Race conditions, locale issues |
| Autocomplete suggestions | Manual filtering + debounce | `AutoCompleteBox` with `MinimumPopulateDelay` | Built-in control handles debounce, keyboard nav, accessibility |
| Modal dialog | Custom overlay/popup | `Window.ShowDialog<T>()` | True modal blocking, system title bar, Escape handling, accessibility |
| Form validation display | Manual error tracking | CommunityToolkit.Mvvm `ObservableValidator` | Built-in `INotifyDataErrorInfo` support |
| Collapsible sections | Custom show/hide | Avalonia `Expander` control | Keyboard accessible, animated, WCAG compliant |

**Key insight:** Phase 3 is about composing existing Avalonia controls and established project patterns — not building custom UI components.

## Common Pitfalls

### Pitfall 1: Orphaned Inverse Relations on Delete
**What goes wrong:** Deleting a relation (A→B) without deleting its inverse (B→A) leaves orphaned records
**Why it happens:** Two separate rows in the database for one logical relation
**How to avoid:** Use `ParId` to link the two sides. `RemoverRelacaoHandler` deletes both rows in same transaction via `WHERE Id = relacaoId OR ParId = relacaoId`
**Warning signs:** Relations appearing in one content's detail but not its counterpart's

### Pitfall 2: Session Creation Without IsFilho Flag
**What goes wrong:** Child Conteudo appears in main listing despite being a session
**Why it happens:** Forgetting to set `IsFilho = true` when creating via `RegistrarSessaoCommand`
**How to avoid:** Make `IsFilho` settable only via a factory method (`Conteudo.CriarComoFilho(...)`) that guarantees the flag is set
**Warning signs:** "ghost" entries in the main acervo list

### Pitfall 3: EF Core SaveChanges Scope for Bidirectional Creation
**What goes wrong:** Forward relation saved but inverse fails — data inconsistency
**Why it happens:** Calling `SaveChangesAsync()` between the two `Add()` operations
**How to avoid:** Add both `Relacao` entities to the context before calling `SaveChangesAsync()` once — EF Core wraps in implicit transaction
**Warning signs:** Half-relations in the database

### Pitfall 4: AutoCompleteBox Async Population
**What goes wrong:** UI freezes or suggestions are stale
**Why it happens:** Querying the database synchronously on key press, or not canceling previous queries
**How to avoid:** Use the `Populating` event with async handler + `CancellationToken`. The `MinimumPopulateDelay` provides natural debounce. Cancel previous pending queries when new text is typed
**Warning signs:** Lag on typing in category/relation type fields

### Pitfall 5: Missing UsuarioId Filter in New Queries (SEG-02)
**What goes wrong:** User can see other users' relation types or relations
**Why it happens:** New repository methods forgetting the mandatory `UsuarioId` filter
**How to avoid:** Every new repository method MUST include `UsuarioId` parameter and filter — code review checklist item. For TipoRelacao, remember to also include `IsSistema == true` (system types visible to all)
**Warning signs:** Integration test with two users shows cross-contamination

### Pitfall 6: Seed Data TipoRelacao Idempotency
**What goes wrong:** Duplicate seed rows on repeated migrations or startup
**Why it happens:** `HasData()` in EF Core uses fixed Guid IDs — if these don't match existing data, duplicates occur. Or if seeder doesn't check existing records
**How to avoid:** Use `HasData()` with deterministic well-known Guids (e.g., `Guid.Parse("10000000-0000-0000-0000-000000000001")` for "Sequência"). Alternative: idempotent seeder that checks `NomeNormalizado` before inserting
**Warning signs:** Multiple "Sequência" entries in autocomplete

### Pitfall 7: Modal Window Closing Without Saving — Data Loss
**What goes wrong:** User modifies fields, closes window (X button), changes are lost silently
**Why it happens:** No unsaved-changes detection
**How to avoid:** Track `IsDirty` flag in ViewModel (set when any property changes). Override `Window.OnClosing()` to show confirmation dialog if dirty. D-02 explicitly requires this
**Warning signs:** User complaints about lost edits

## Code Examples

### Complete CriarRelacaoHandler (Bidirectional, Atomic)
```csharp
// Follows existing CriarConteudoHandler pattern
internal sealed class CriarRelacaoHandler : IRequestHandler<CriarRelacaoCommand, Resultado<Guid>>
{
    private readonly IRelacaoRepository _relacaoRepo;
    private readonly ITipoRelacaoRepository _tipoRelacaoRepo;
    private readonly IConteudoRepository _conteudoRepo;

    public async Task<Resultado<Guid>> Handle(CriarRelacaoCommand cmd, CancellationToken ct)
    {
        // Validate self-reference
        if (cmd.ConteudoOrigemId == cmd.ConteudoDestinoId)
            return Resultado<Guid>.Failure(Erros.AutoReferenciaProibida);

        // Validate both content exist and belong to user
        var origem = await _conteudoRepo.ObterPorIdAsync(cmd.ConteudoOrigemId, cmd.UsuarioId, ct);
        if (origem is null)
            return Resultado<Guid>.Failure(Erros.NaoEncontrado);

        var destino = await _conteudoRepo.ObterPorIdAsync(cmd.ConteudoDestinoId, cmd.UsuarioId, ct);
        if (destino is null)
            return Resultado<Guid>.Failure(Erros.NaoEncontrado);

        // Get or create TipoRelacao
        var tipo = await _tipoRelacaoRepo.ObterOuCriarAsync(
            cmd.UsuarioId, cmd.NomeTipoRelacao, cmd.NomeInverso, ct);

        // Check duplicate
        var existente = await _relacaoRepo.ExisteAsync(
            cmd.ConteudoOrigemId, cmd.ConteudoDestinoId, tipo.Id, cmd.UsuarioId, ct);
        if (existente)
            return Resultado<Guid>.Failure(Erros.RelacaoDuplicada);

        var now = DateTimeOffset.UtcNow;
        var forwardId = Guid.NewGuid();
        var inverseId = Guid.NewGuid();

        // Forward: A → B
        var forward = new Relacao
        {
            Id = forwardId,
            ConteudoOrigemId = cmd.ConteudoOrigemId,
            ConteudoDestinoId = cmd.ConteudoDestinoId,
            TipoRelacaoId = tipo.Id,
            IsInversa = false,
            ParId = inverseId,
            UsuarioId = cmd.UsuarioId,
            CriadoEm = now,
        };

        // Inverse: B → A
        var inverse = new Relacao
        {
            Id = inverseId,
            ConteudoOrigemId = cmd.ConteudoDestinoId,
            ConteudoDestinoId = cmd.ConteudoOrigemId,
            TipoRelacaoId = tipo.Id,
            IsInversa = true,
            ParId = forwardId,
            UsuarioId = cmd.UsuarioId,
            CriadoEm = now,
        };

        await _relacaoRepo.AdicionarParAsync(forward, inverse, ct); // single SaveChangesAsync

        return Resultado<Guid>.Success(forwardId);
    }
}
```

### ConteudoDetalheWindow.axaml (Modal Structure)
```xml
<Window xmlns="https://github.com/avaloniaui"
        Title="{Binding Titulo}"
        Width="700" Height="600" MinWidth="500" MinHeight="400"
        WindowStartupLocation="CenterOwner"
        CanResize="True">
    <Grid RowDefinitions="*,Auto">
        <!-- Scrollable content area -->
        <ScrollViewer Grid.Row="0" Padding="24,16">
            <StackPanel Spacing="16">
                <!-- Section 1: Informações Básicas (always expanded) -->
                <Expander Header="Informações Básicas" IsExpanded="True">
                    <!-- titulo, formato, classificação, nota -->
                </Expander>

                <!-- Section 2: Descrição e Anotações -->
                <Expander Header="Descrição e Anotações">
                    <!-- descrição TextBox, anotações TextBox -->
                </Expander>

                <!-- Section 3: Categorias -->
                <Expander Header="Categorias">
                    <!-- chips + AutoCompleteBox -->
                </Expander>

                <!-- Section 4: Relações -->
                <Expander Header="Relações">
                    <!-- list of relations + add button -->
                </Expander>

                <!-- Section 5: Sessões de Consumo (if not IsFilho) -->
                <Expander Header="Sessões de Consumo" IsVisible="{Binding !IsFilho}">
                    <!-- timeline + "Registrar sessão" button -->
                </Expander>

                <!-- Section 6: Progresso -->
                <Expander Header="Progresso">
                    <!-- estado, posição, calculated % -->
                </Expander>
            </StackPanel>
        </ScrollViewer>

        <!-- Fixed footer -->
        <Border Grid.Row="1" Padding="24,8" BorderThickness="0,1,0,0"
                BorderBrush="{DynamicResource SystemControlForegroundBaseLowBrush}">
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Spacing="8">
                <Button Content="Excluir" Command="{Binding ExcluirCommand}" />
                <Button Content="Cancelar" Command="{Binding CancelarCommand}" />
                <Button Content="Salvar" Command="{Binding SalvarCommand}" />
            </StackPanel>
        </Border>
    </Grid>
</Window>
```

### IsDirty Tracking Pattern (D-02)
```csharp
// In ConteudoDetalheViewModel — track unsaved changes
private string _tituloOriginal = string.Empty;
private string? _descricaoOriginal;
// ... for all editable fields

public bool IsDirty =>
    Titulo != _tituloOriginal ||
    Descricao != _descricaoOriginal ||
    // ... all fields compared to originals
    _categoriasAlteradas ||
    _relacoesAlteradas;

// On window closing:
protected override async void OnClosing(WindowClosingEventArgs e)
{
    if (IsDirty)
    {
        e.Cancel = true; // prevent close
        var result = await ShowConfirmDialog("Descartar alterações?");
        if (result == true)
            Close(); // close without saving
    }
}
```

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| SukiUI theme | FluentTheme (Avalonia built-in) | Phase 2 | No external theme dependency |
| Single-row bidirectional relations | Two-row with ParId linking | Industry standard | Simpler queries, atomic delete |
| Tab-based forms | Scrollable Expander sections | HCI best practice | Better discoverability, progressive disclosure |

**Deprecated/outdated:**
- SukiUI: abandoned in Phase 2 — only theme-only for Avalonia 0.10, no SukiWindow
- Global Query Filters for soft-delete patterns: increasingly recognized as source of subtle bugs in EF Core — prefer explicit filters

## Validation Architecture

### Test Framework
| Property | Value |
|----------|-------|
| Framework | xUnit (latest) + NSubstitute 5.3.0 |
| Config file | Tests defined in individual .csproj files |
| Quick run command | `dotnet test tests/Tests.Domain --no-build -v q` |
| Full suite command | `dotnet test --no-build -v q` |

### Phase Requirements → Test Map
| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|-------------|
| ACE-01 | Classificacao enum + field on Conteudo | unit | `dotnet test tests/Tests.Domain --filter "ClassName~ClassificacaoTests" -v q` | ❌ Wave 0 |
| ACE-02 | EditarConteudo handler — update all fields | unit | `dotnet test tests/Tests.Domain --filter "ClassName~EditarConteudoHandlerTests" -v q` | ❌ Wave 0 |
| ACE-02 | RemoverConteudo handler — with confirmation | unit | `dotnet test tests/Tests.Domain --filter "ClassName~RemoverConteudoHandlerTests" -v q` | ❌ Wave 0 |
| ACE-02 | Session (child Conteudo) creation via RegistrarSessao | unit | `dotnet test tests/Tests.Domain --filter "ClassName~SessaoTests" -v q` | ❌ Wave 0 |
| ACE-02 | Calculated progress from sessions | unit | `dotnet test tests/Tests.Domain --filter "ClassName~ProgressoCalculadoTests" -v q` | ❌ Wave 0 |
| ACE-03 | Category association via ConteudoCategoria | integration | `dotnet test tests/Tests.Integration --filter "ClassName~ConteudoCategoriaTests" -v q` | ❌ Wave 0 |
| ACE-04 | Bidirectional relation creation (atomic) | unit | `dotnet test tests/Tests.Domain --filter "ClassName~RelacaoBidirecionalTests" -v q` | ❌ Wave 0 |
| ACE-04 | TipoRelacao deduplication (case-insensitive) | unit+integration | `dotnet test tests/Tests.Domain --filter "ClassName~TipoRelacaoTests" -v q` | ❌ Wave 0 |
| ACE-04 | Relation deletion removes both sides | integration | `dotnet test tests/Tests.Integration --filter "ClassName~RelacaoRepositoryTests" -v q` | ❌ Wave 0 |
| ACE-04 | Self-reference prohibition | unit | `dotnet test tests/Tests.Domain --filter "ClassName~RelacaoBidirecionalTests" -v q` | ❌ Wave 0 |
| ACE-09 | Pagination includes IsFilho filter | integration | `dotnet test tests/Tests.Integration --filter "ClassName~ConteudoQueryServiceTests" -v q` | ❌ Wave 0 |

### Sampling Rate
- **Per task commit:** `dotnet test tests/Tests.Domain --no-build -v q`
- **Per wave merge:** `dotnet test --no-build -v q`
- **Phase gate:** Full suite green before `/gsd:verify-work`

### Wave 0 Gaps
- [ ] `tests/Tests.Domain/Acervo/ClassificacaoTests.cs` — covers ACE-01 (Classificacao enum invariants)
- [ ] `tests/Tests.Domain/Acervo/RelacaoBidirecionalTests.cs` — covers ACE-04 (bidirectional creation, self-ref prohibition, duplicate check)
- [ ] `tests/Tests.Domain/Acervo/TipoRelacaoDeduplicacaoTests.cs` — covers ACE-04 (case-insensitive dedup)
- [ ] `tests/Tests.Domain/Acervo/SessaoProgressoCalculadoTests.cs` — covers ACE-02 (session creation, IsFilho flag, progress calculation)
- [ ] `tests/Tests.Integration/Repositorios/TipoRelacaoRepositoryTests.cs` — covers ACE-04 (persistence + unique constraint)
- [ ] `tests/Tests.Integration/Repositorios/RelacaoRepositoryTests.cs` — covers ACE-04 (bidirectional persistence, delete both sides)
- [ ] Migration test in existing `MigrationTests.cs` — extended to cover new migration

## Open Questions

1. **Symmetric relation types (D-13)**
   - What we know: "Alternativa a" ↔ "Alternativa a" and "Do mesmo tipo que" ↔ "Do mesmo tipo que" are symmetric
   - What's unclear: Should symmetric types create 2 rows or 1 row? If 2 rows, both show the same label — is that confusing?
   - Recommendation: Always create 2 rows for consistency. When `Nome == NomeInverso`, both sides show the same label — no confusion. Simpler code than special-casing symmetric relations

2. **"Contém"/"Parte de" as 9th pre-defined TipoRelacao**
   - What we know: D-13 lists 8 pre-defined types. D-17/D-18 require "Contém"/"Parte de" for sessions
   - What's unclear: Is "Contém"/"Parte de" the 9th pre-defined seed, or should it be handled as a special enum?
   - Recommendation: Add as 9th pre-defined TipoRelacao seed with `IsSistema = true`. Mark it with an additional `IsEspecialSessao = true` flag (or use a well-known Guid) so the UI can identify session relations and display them as timeline rather than normal relations. Simpler than a parallel enum system

3. **Confirmation dialog for delete (D-03)**
   - What we know: Avalonia has no built-in `MessageBox`. FluentTheme provides `ContentDialog`
   - What's unclear: Exact API for showing confirmation dialogs in Avalonia 11
   - Recommendation: Use a custom `ConfirmDialog` Window with ShowDialog, or check if `ContentDialog` is available in FluentTheme. Alternatively, implement a reusable `IConfirmacaoService` abstraction

## Environment Availability

| Dependency | Required By | Available | Version | Fallback |
|------------|------------|-----------|---------|----------|
| .NET SDK | Build/Run | ✓ | 10.0.104 | — |
| EF Core CLI | Migrations | ✓ | 9.0.8 | — |
| Docker | Integration tests | ✓ | 29.3.1 | — |
| PostgreSQL (via Testcontainers) | Integration tests | ✓ | docker-based | — |

**Missing dependencies with no fallback:** None
**Missing dependencies with fallback:** None

## Sources

### Primary (HIGH confidence)
- Existing codebase: `Conteudo.cs`, `CategoriaRepository.cs`, `ConteudoConfiguration.cs` — established patterns
- `docs/domain/acervo.md` — tactical model with `RelacaoConteudo` as entity outside aggregates (line 326)
- `especificacoes/5 - technical-standards.md` §6.8 — Categorias e Relações Bidirecionais pattern
- `especificacoes/5 - technical-standards.md` §6.10 — Disclosure Progressivo with Shneiderman citation
- `especificacoes/1 - definicao-de-dominio.md` §4.6 — Conteúdos Relacionados bidirectional spec
- Avalonia 11 API: `AutoCompleteBox`, `Expander`, `Window.ShowDialog<T>()` — standard controls

### Secondary (MEDIUM confidence)
- HCI literature: Shneiderman (1996), Norman (2013), Cooper et al. (2014) — referenced in project's own technical standards
- EF Core Global Query Filter limitations — well-documented in Microsoft docs, community experience

### Tertiary (LOW confidence)
- Timeline visual implementation — no standard Avalonia component, custom template approach based on general WPF/Avalonia patterns

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH — no new packages needed, all controls built into Avalonia 11.2.4
- Architecture: HIGH — follows established project patterns (Categoria, MediatR handlers, ConteudoConfiguration)
- Pitfalls: HIGH — common EF Core patterns well-documented; bidirectional relation atomicity is a known challenge
- UI layout: MEDIUM — HCI principles are solid but Avalonia-specific implementation details (AutoCompleteBox async, Expander behavior) need validation during implementation

**Research date:** 2026-04-03
**Valid until:** 2026-05-03 (30 days — stable stack, no expected breaking changes)
