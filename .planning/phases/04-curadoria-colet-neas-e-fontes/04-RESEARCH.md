# Phase 4: Curadoria -- Coletaneas e Fontes - Research

**Researched:** 2026-04-06
**Domain:** C#/.NET 10, Avalonia UI 11.2.4, EF Core (PostgreSQL), MediatR -- CQRS, Domain-driven collection management
**Confidence:** HIGH

## Summary

Phase 4 completes Pilar 1 (personal content management) by adding collection management (Guiada/Miscelanea), source management with priority/fallback, cover images, and duplicate detection. The domain model (`Conteudo` with `Papel==Coletanea`) and core infrastructure (entities, invariants I-01 to I-10, EF Core mapping, MediatR command pattern) are already established. The key new deliverable is the `ConteudoColetanea` join entity with contextual annotation payload, the `ColetaneaRepository` implementation (interface exists, no implementation), the deduplication query service, and a new `ColetaneaDetalheWindow` modal with breadcrumb navigation for nested collections.

The codebase follows strict patterns: `Resultado<T>` for all service returns, `PaginacaoParams` mandatory on all listings, `usuarioId` on every repository method (SEG-02), Portuguese domain naming, `Strings.resx` for all UI text, `AutomationProperties` on every interactive element (WCAG 2.2 AAA). All new code must conform to these established patterns. A comprehensive UI-SPEC (`04-UI-SPEC.md`) has been approved and provides pixel-level specifications for all components.

**Primary recommendation:** Build from the inside out -- domain entities and join table first, then repository/query layer, then MediatR commands/queries, then ViewModels, then AXAML views. The `ConteudoColetanea` join entity is the architectural cornerstone; everything else depends on it.

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions
- **D-01:** Coletaneas and items coexist in the same AcervoView with filter tabs [Itens] [Coletaneas] [Todos]. Collection cards have distinct visual -- badge, item count, progress bar for Guiadas.
- **D-02:** Opening a collection = modal (similar to ConteudoDetalheWindow). Collection modal displays paginated item list. Nested sub-collections navigate within the same modal (content replacement with breadcrumb) -- no modals inside modals.
- **D-03:** Guiadas show "next item" indicator and sequential numbering. Miscelaneas show items without numbering or sequence indicator.
- **D-04:** User adds items from within the collection modal (button opens autocomplete-based item selector from acervo). Removes items directly from the internal list.
- **D-05:** Contextual annotations accessed within the collection modal: each item row has annotation icon button. Inline text area expands below item row with label "Anotacao nesta coletanea". Global annotation remains separate.
- **D-06:** When an item has a contextual annotation, the icon button has a filled visual. Without annotation = outline/empty icon.
- **D-07:** Two-level deduplication confidence: High = exact URL match in any source. Medium = normalized title (trim, lowercase, remove diacritics, remove punctuation).
- **D-08:** Flow: warning on creation. Banner below title field when duplicate candidate detected. Shows candidate name, creation date, buttons "Ver este" (opens existing) and "Criar mesmo assim". No blocking, no auto-merge.
- **D-09:** Confidence level displayed in warning: "Fonte identica detectada" (high) vs. "Titulo semelhante encontrado" (medium).
- **D-10:** No duplicate maintenance list this phase. Manual merge deferred to Phase 11.
- **D-11:** New "Fontes" section in ConteudoDetalheWindow accordion. Position in list = priority (1st = highest).
- **D-12:** Reorder via up/down arrow buttons per source row. Add via inline form (type + value). Remove via X button.
- **D-13:** Source types: Url, ArquivoLocal, Rss, Identificador (aligned with existing `TipoFonte` enum).
- **D-14:** Metadata authority hierarchy (manual > automatic) enforced in application layer -- no user prompt on conflict.
- **D-15:** Simple cover: one image per content via local file upload. Placed in Identificacao expander top.
- **D-16:** Invariants I-04 (max 20 images) and I-05 (max 10MB) already exist. UI limits to 1 image (cover). Gallery deferred to Phase 11.
- **D-17:** Collection card thumbnail shows cover image if exists; placeholder otherwise with folder icon and type-colored badge.
- **D-18:** Mandatory scientific research for modal layout, orderable lists, breadcrumb in modal, progress indicators, duplicate warnings. Fulfilled in UI-SPEC scientific foundation.

### Claude's Discretion
- Visual representation of collection type badge on card (icon, color, text)
- Exact breadcrumb behavior inside collection modal for nested navigation
- Transition animations when switching Itens/Coletaneas/Todos filter tabs
- Debounce and threshold for duplicate detection by normalized title

### Deferred Ideas (OUT OF SCOPE)
- Coletaneas do tipo Subscricao (feed-driven) -- Phase 6
- Gallery of multiple images per content -- Phase 11
- Manual merge of duplicates -- Phase 11
- Duplicate maintenance list -- Phase 11
- Batch operations -- Phase 7
- Hooks/bookmarks within content -- Phase 8
</user_constraints>

<phase_requirements>
## Phase Requirements

| ID | Description | Research Support |
|----|-------------|------------------|
| ACE-05 | Fontes com prioridade e fallback; hierarquia de autoridade de metadados (manual > automatico) | `Fonte` entity and `AdicionarFonte()` with I-07 already exist. New: UI section in ConteudoDetalheWindow, reorder commands, add/remove commands. `BuscarPorUrlFonteAsync` already in `IConteudoRepository`. |
| ACE-06 | Coletaneas: Guiada (sequential with progress tracking), Miscelanea (free-form) | `Conteudo.Criar(papel: Coletanea, tipoColetanea: Guiada/Miscelanea)` factory exists. New: `ConteudoColetanea` join entity, `ColetaneaRepository` implementation, `ColetaneaDetalheWindow` modal, item ordering for Guiadas. |
| ACE-07 | Composicao de coletaneas (collection containing collections) with cycle protection | `IColetaneaRepository.ObterDescendentesAsync()` interface exists. New: implementation with DFS cycle detection, validation in `AdicionarItemNaColetaneaCommand`. |
| ACE-08 | Anotacoes contextuais pertencentes a relacao conteudo-coletanea (not to content itself) | New: `AnotacaoContextual` field on `ConteudoColetanea` join entity. Inline editor in collection modal per D-05/D-06. |
| ACE-10 | Deduplicacao de conteudo | New: `IDeduplicacaoService` with two-level detection (URL match + normalized title). Banner component in CriarConteudoView. Uses existing `BuscarPorUrlFonteAsync`. |
| SEG-04 | Todos os cenarios do Apendice A (cenarios 1-5) executaveis e testados | Scenarios map to: C1 (Miscelanea + contextual annotations), C2 (sources with fallback), C3 (no-source content -- already works), C4 (Guiada with nested collections), C5 (franchise composition). Integration tests required. |
</phase_requirements>

## Standard Stack

### Core
| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| .NET | 10.0 | Target framework | Project standard (net10.0) |
| Avalonia | 11.2.4 | Desktop UI framework | Project standard -- FluentTheme, no SukiUI components used |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM source generators, ObservableObject | Project standard for ViewModels |
| MediatR | 12.4.1 | CQRS mediator | Project standard for command/query pattern |
| EF Core (Npgsql) | (project version) | ORM, migrations | Project standard for PostgreSQL |
| Testcontainers.PostgreSql | 4.11.0 | Integration test containers | Project standard for real-DB tests |
| NSubstitute | 5.3.0 | Mocking | Project standard for unit tests |
| xUnit | (project version) | Test framework | Project standard |

### Supporting
| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| Avalonia.Platform.Storage | (bundled with Avalonia 11.2.4) | File picker dialog for cover image | `IStorageProvider.OpenFilePickerAsync` for OS-native file selection |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| RadioButton GroupName for filter tabs | ToggleButton with manual exclusion | RadioButton GroupName is the standard Avalonia approach for exclusive selection; but UI-SPEC specifies ToggleButton with manual IsChecked management per the card visual style |
| Drag-and-drop for source reorder | Arrow buttons (up/down) | Arrow buttons chosen per D-12 and WCAG 2.1.1 keyboard accessibility |

**No new packages needed.** All dependencies are already in the project.

## Architecture Patterns

### New Entities and Data Model

```
ConteudoColetanea (NEW join entity)
├── ColetaneaId (Guid, FK -> Conteudos.Id)
├── ConteudoId (Guid, FK -> Conteudos.Id)
├── Posicao (int, sequential for Guiada, auto-assigned for Miscelanea)
├── AnotacaoContextual (string?, max 10000)
├── AdicionadoEm (DateTimeOffset)
└── Composite PK: (ColetaneaId, ConteudoId)
```

**Key constraint:** One content can appear in multiple collections, but only once per collection (composite PK prevents duplicates in same collection). Each instance has its own contextual annotation and position.

### Recommended Project Structure (new files)

```
src/
  DiarioDeBordo.Core/
    Entidades/
      ConteudoColetanea.cs          # NEW: join entity with payload
    Consultas/
      IColetaneaQueryService.cs     # NEW: read-side for collection data
      IDeduplicacaoService.cs       # NEW: duplicate detection service
    Repositorios/
      IColetaneaRepository.cs       # EXISTS: needs implementation
  DiarioDeBordo.Infrastructure/
    Persistencia/
      Configuracoes/
        ConteudoColetaneaConfiguration.cs  # NEW: EF config for join table
      Migrations/
        YYYYMMDDHHMMSS_AddConteudoColetanea.cs  # NEW: migration
    Repositorios/
      ColetaneaRepository.cs        # NEW: implements IColetaneaRepository
    Consultas/
      ColetaneaQueryService.cs      # NEW: read-side queries
      DeduplicacaoService.cs        # NEW: title normalization + URL match
  DiarioDeBordo.UI/
    Views/
      ColetaneaDetalheWindow.axaml  # NEW: collection detail modal
      ColetaneaDetalheWindow.axaml.cs
    ViewModels/
      ColetaneaDetalheViewModel.cs  # NEW: collection modal VM
      ColetaneaCardViewModel.cs     # NEW: card VM for collection items in list
      ColetaneaItemViewModel.cs     # NEW: item row VM inside collection modal
      FonteItemViewModel.cs         # NEW: source row VM for reorder/remove
  Modules/
    Module.Acervo/
      Commands/
        AdicionarItemNaColetaneaCommand.cs     # NEW
        AdicionarItemNaColetaneaHandler.cs      # NEW
        RemoverItemDaColetaneaCommand.cs        # NEW
        RemoverItemDaColetaneaHandler.cs         # NEW
        AtualizarAnotacaoContextualCommand.cs   # NEW
        AtualizarAnotacaoContextualHandler.cs    # NEW
        AdicionarFonteCommand.cs                # NEW
        AdicionarFonteHandler.cs                 # NEW
        RemoverFonteCommand.cs                  # NEW
        RemoverFonteHandler.cs                   # NEW
        ReordenarFontesCommand.cs               # NEW
        ReordenarFontesHandler.cs                # NEW
        AdicionarImagemCapaCommand.cs           # NEW
        AdicionarImagemCapaHandler.cs            # NEW
        RemoverImagemCapaCommand.cs             # NEW
        RemoverImagemCapaHandler.cs              # NEW
        VerificarDuplicataQuery.cs              # NEW (query, not command)
      Queries/
        ListarItensColetaneaQuery.cs            # NEW
        ListarItensColetaneaHandler.cs           # NEW
        ObterColetaneaDetalheQuery.cs           # NEW
        ObterColetaneaDetalheHandler.cs          # NEW
      DTOs/
        ColetaneaDetalheDto.cs                  # NEW
        ColetaneaItemDto.cs                     # NEW
        FonteDto.cs                             # NEW
        DuplicataDto.cs                         # NEW
```

### Pattern 1: ConteudoColetanea as Explicit Join Entity with Payload

**What:** A many-to-many relationship between `Conteudo` (as collection) and `Conteudo` (as item) with payload fields (position, contextual annotation, timestamp). EF Core requires an explicit join entity when the join table has payload columns.

**When to use:** When the association itself carries data (ACE-08 contextual annotations, position for Guiada ordering).

**Example:**
```csharp
// Source: EF Core official docs — Many-to-many with payload
public sealed class ConteudoColetanea
{
    public Guid ColetaneaId { get; init; }
    public Guid ConteudoId { get; init; }
    public int Posicao { get; set; }
    public string? AnotacaoContextual { get; set; }
    public DateTimeOffset AdicionadoEm { get; init; }
}
```

**EF Core Configuration:**
```csharp
builder.ToTable("conteudo_coletanea");
builder.HasKey(cc => new { cc.ColetaneaId, cc.ConteudoId });

builder.HasOne<Conteudo>()
    .WithMany()
    .HasForeignKey(cc => cc.ColetaneaId)
    .OnDelete(DeleteBehavior.Cascade);

builder.HasOne<Conteudo>()
    .WithMany()
    .HasForeignKey(cc => cc.ConteudoId)
    .OnDelete(DeleteBehavior.Restrict); // items NOT deleted when collection deleted
```

### Pattern 2: Cycle Detection via DFS in ColetaneaRepository

**What:** When adding a collection B as item of collection A, validate that B (or any descendant of B) does not contain A. Uses `ObterDescendentesAsync` DFS traversal.

**When to use:** Every time a `Conteudo` with `Papel==Coletanea` is added to another collection.

**Example:**
```csharp
// In AdicionarItemNaColetaneaHandler:
if (itemToAdd.Papel == PapelConteudo.Coletanea)
{
    var descendentes = await _coletaneaRepo
        .ObterDescendentesAsync(itemToAdd.Id, cmd.UsuarioId, ct);

    if (descendentes.Contains(cmd.ColetaneaId))
        return Resultado<Unit>.Failure(Erros.CicloDetetadoNaComposicao);
}
```

**Implementation of ObterDescendentesAsync:**
```csharp
// Recursive CTE or iterative BFS/DFS using ConteudoColetanea join table
// Returns all descendant collection IDs (transitive closure)
public async Task<IReadOnlyList<Guid>> ObterDescendentesAsync(
    Guid coletaneaId, Guid usuarioId, CancellationToken ct)
{
    var visited = new HashSet<Guid>();
    var queue = new Queue<Guid>();
    queue.Enqueue(coletaneaId);

    while (queue.Count > 0)
    {
        var current = queue.Dequeue();
        if (!visited.Add(current)) continue;

        var children = await _context.Set<ConteudoColetanea>()
            .Where(cc => cc.ColetaneaId == current)
            .Join(_context.Conteudos,
                cc => cc.ConteudoId, c => c.Id,
                (cc, c) => new { c.Id, c.Papel, c.UsuarioId })
            .Where(x => x.Papel == PapelConteudo.Coletanea
                        && x.UsuarioId == usuarioId)
            .Select(x => x.Id)
            .ToListAsync(ct);

        foreach (var child in children)
            queue.Enqueue(child);
    }

    visited.Remove(coletaneaId); // exclude self
    return visited.ToList().AsReadOnly();
}
```

### Pattern 3: Title Normalization for Deduplication

**What:** Normalize titles for medium-confidence duplicate detection: trim, lowercase, remove diacritics, remove punctuation.

**When to use:** During content creation (D-07/D-08/D-09 dedup check).

**Example:**
```csharp
// Source: .NET String.Normalize + Unicode category filtering
public static string NormalizarTitulo(string titulo)
{
    if (string.IsNullOrWhiteSpace(titulo))
        return string.Empty;

    var normalized = titulo.Trim().ToLowerInvariant();

    // Remove diacritics via Unicode decomposition
    normalized = string.Concat(
        normalized.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
                        != UnicodeCategory.NonSpacingMark))
        .Normalize(NormalizationForm.FormC);

    // Remove punctuation
    normalized = string.Concat(
        normalized.Where(c => !char.IsPunctuation(c)));

    // Collapse whitespace
    normalized = string.Join(' ',
        normalized.Split(default(char[]),
            StringSplitOptions.RemoveEmptyEntries));

    return normalized;
}
```

### Pattern 4: File Picker for Cover Image (Avalonia 11)

**What:** Use `IStorageProvider.OpenFilePickerAsync` for OS-native file selection.

**When to use:** When user clicks "Escolher imagem" in the Identificacao section.

**Example:**
```csharp
// Source: Avalonia 11 official docs — File Dialogs
var topLevel = TopLevel.GetTopLevel(this);
if (topLevel?.StorageProvider is not { } storage) return;

var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
{
    Title = Strings.EscolherImagemDeCapa,
    AllowMultiple = false,
    FileTypeFilter = new[]
    {
        new FilePickerFileType("Images")
        {
            Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.webp" }
        }
    }
});

if (files.Count == 1)
{
    var file = files[0];
    // Validate size (I-05: max 10MB), then proceed
}
```

### Pattern 5: Filter Tabs with RadioButton GroupName

**What:** Exclusive toggle for Todos/Itens/Coletaneas filter in AcervoView.

**When to use:** Extending AcervoView to support collection filtering.

**Implementation approach:** Use `RadioButton` with `GroupName="PapelFiltro"` or manually manage `ToggleButton.IsChecked` with one-at-a-time logic in ViewModel. The UI-SPEC specifies ToggleButton with accent tint background for active state. Recommendation: Use RadioButton styled as ToggleButton (custom Style) -- this gives GroupName exclusivity with ToggleButton visual. Alternatively, manage exclusion in ViewModel.

### Anti-Patterns to Avoid

- **Loading entire collection graph eagerly:** The nested collection structure can be deep. Load one level at a time via paginated query. Never fetch the entire tree.
- **Storing normalized title in the database:** The normalized form is only for comparison. Store original title; normalize on-the-fly during dedup checks. If performance becomes an issue (Phase 11), add a computed column later.
- **Using Drag-and-Drop for source reorder:** D-12 explicitly specifies arrow buttons. Drag-and-drop has poor keyboard accessibility (WCAG 2.1.1 violation) and is complex in Avalonia.
- **Opening nested modals:** D-02 explicitly forbids modals inside modals. Sub-collection navigation replaces content within the same modal window using breadcrumb.
- **Auto-merging duplicates:** D-08 specifies user-decides flow. The system warns, the user chooses. Never auto-merge.

## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Diacritics removal | Custom character mapping table | `string.Normalize(FormD)` + `UnicodeCategory.NonSpacingMark` filter | Unicode standard handles all scripts; a mapping table misses edge cases |
| File picker dialog | Custom file browser UI | `IStorageProvider.OpenFilePickerAsync` | OS-native, accessible, handles permissions |
| Cycle detection algorithm | Custom recursive SQL | Iterative BFS in C# using `ConteudoColetanea` queries | Recursive CTEs are PostgreSQL-specific and harder to test; BFS with visited set is clear and testable |
| Pagination | Custom skip/take logic | Existing `PaginacaoParams` + `ResultadoPaginado<T>` | Mandatory pattern -- already enforced in all listings |
| Error propagation | Exceptions for business logic | `Resultado<T>` pattern | Project-wide convention since Phase 2 |

**Key insight:** Phase 4 adds substantial new functionality but the architectural patterns are all established. The risk is in the EF Core join entity configuration and the cycle detection correctness, not in choosing tools.

## Common Pitfalls

### Pitfall 1: EF Core Owned Entity Conflict with Join Entity
**What goes wrong:** `ConteudoColetanea` references `Conteudo` twice (as collection and as item). EF Core can get confused about navigation properties if not configured explicitly.
**Why it happens:** EF Core conventions try to infer relationships; with self-referencing many-to-many, it needs explicit configuration.
**How to avoid:** Use `HasOne<Conteudo>().WithMany().HasForeignKey(cc => cc.ColetaneaId)` without navigation properties. Do NOT add `Conteudo` navigation properties to `ConteudoColetanea` -- use explicit joins in queries.
**Warning signs:** Migration generation fails or creates unexpected shadow foreign keys.

### Pitfall 2: Cascade Delete Destroying Items
**What goes wrong:** Deleting a collection cascades and deletes the items it contains.
**Why it happens:** Default EF Core cascade on FK relationships.
**How to avoid:** Use `OnDelete(DeleteBehavior.Restrict)` on the ConteudoId FK (item side). Use `OnDelete(DeleteBehavior.Cascade)` only on ColetaneaId FK (so removing the collection removes the associations, NOT the items). The confirmation dialog text must state: "Os itens dentro da coletanea NAO serao excluidos."
**Warning signs:** Items disappear from acervo after collection deletion.

### Pitfall 3: Priority Renumbering Race Condition
**What goes wrong:** When reordering sources, the unique constraint on (ConteudoId, Prioridade) can fail if priorities are updated one-by-one.
**Why it happens:** Unique index `idx_fontes_conteudo_prioridade_unique` is checked per-row during update.
**How to avoid:** Remove all existing sources from the aggregate, then re-add with new priorities in a single `SaveChangesAsync` call. Or use a temp-to-final renumbering (assign negative temp values, then final values). The owned entity pattern (`OwnsMany`) means the aggregate controls all mutations.
**Warning signs:** `DbUpdateException` with unique constraint violation during reorder.

### Pitfall 4: Breadcrumb State Becoming Stale
**What goes wrong:** User navigates deep into nested collections, then an item or sub-collection is deleted from another context (e.g., directly from AcervoView).
**Why it happens:** The breadcrumb maintains a local stack of collection IDs and titles.
**How to avoid:** When navigating back via breadcrumb, re-query the collection. If the collection no longer exists, pop the breadcrumb and navigate to the nearest valid ancestor.
**Warning signs:** Blank modal or crash after navigating back to a deleted collection.

### Pitfall 5: Feature Flags Placeholder Returns False
**What goes wrong:** `FeatureFlagsPlaceholder.IsEnabled()` always returns `false`. If Phase 4 code checks feature flags, nothing will work.
**Why it happens:** The placeholder was scaffolded in Phase 2 and never replaced.
**How to avoid:** Either (a) implement real feature flag storage with Phase 4 flags enabled, or (b) update the placeholder to return `true` for `coletaneas`, `fontes_com_fallback`, and `deduplicacao`. Option (b) is simpler for now -- Phase 9 (Preferencias) is when real user-configurable flags would matter.
**Warning signs:** UI components hidden or commands returning unexpected errors due to disabled flags.

### Pitfall 6: Dedup Query Performance on Large Datasets
**What goes wrong:** Normalized title comparison on every keystroke (even with 500ms debounce) scans the entire `conteudos` table.
**Why it happens:** `LOWER(titulo)` comparison without an index on normalized form.
**How to avoid:** For Phase 4 with personal-scale data (< 10k items per user), the simple approach works. Add a PostgreSQL index: `CREATE INDEX idx_conteudos_titulo_lower ON conteudos (usuario_id, LOWER(titulo))`. Full normalization (diacritics, punctuation) must happen in C# -- the DB index only helps with case-insensitive prefix matching.
**Warning signs:** Slow typing response in title field on larger datasets.

### Pitfall 7: ConteudoColetanea Not Added to DbContext
**What goes wrong:** New entity exists but EF Core doesn't know about it because there's no `DbSet` and no explicit configuration applied.
**Why it happens:** Forgetting to register the new entity in `DiarioDeBordoDbContext`.
**How to avoid:** Add `public DbSet<ConteudoColetanea> ConteudoColetaneas => Set<ConteudoColetanea>();` to `DiarioDeBordoDbContext`. Create `ConteudoColetaneaConfiguration` implementing `IEntityTypeConfiguration<ConteudoColetanea>` -- it will be auto-discovered by `ApplyConfigurationsFromAssembly`.
**Warning signs:** Migration doesn't generate the expected table.

## Code Examples

### Extending ListarConteudosQuery with PapelConteudo Filter
```csharp
// Source: existing ListarConteudosQuery pattern
public sealed record ListarConteudosQuery(
    Guid UsuarioId,
    PaginacaoParams Paginacao,
    PapelConteudo? PapelFiltro = null)  // NEW: null = Todos
    : IRequest<Resultado<PaginatedList<ConteudoResumoDto>>>;

// In ConteudoQueryService.ListarAsync:
var query = _context.Conteudos
    .Where(c => c.UsuarioId == usuarioId && !c.IsFilho);

if (papelFiltro.HasValue)
    query = query.Where(c => c.Papel == papelFiltro.Value);
```

### Extending ConteudoResumoData for Collection Info
```csharp
// Add to ConteudoResumoData or create separate ColetaneaResumoData
public sealed record ConteudoResumoData(
    Guid Id,
    string Titulo,
    FormatoMidia Formato,
    PapelConteudo Papel,
    DateTimeOffset CriadoEm,
    Classificacao? Classificacao,
    string? Subtipo,
    TipoColetanea? TipoColetanea,     // NEW
    int? QuantidadeItens,               // NEW: count of items in collection
    decimal? ProgressoPercentual,       // NEW: for Guiada progress display
    string? ImagemCapaCaminho);         // NEW: cover image path for thumbnail
```

### AdicionarItemNaColetaneaCommand Pattern
```csharp
public sealed record AdicionarItemNaColetaneaCommand(
    Guid ColetaneaId,
    Guid ConteudoId,
    Guid UsuarioId) : IRequest<Resultado<Unit>>;

// Handler:
// 1. Validate both exist and belong to user
// 2. If ConteudoId is a collection: run cycle detection
// 3. Determine next position (max Posicao + 1 for this collection)
// 4. Create ConteudoColetanea join record
// 5. Persist
```

### VerificarDuplicataQuery Pattern
```csharp
public sealed record VerificarDuplicataQuery(
    Guid UsuarioId,
    string Titulo,
    IReadOnlyList<string>? FonteUrls = null)
    : IRequest<Resultado<DuplicataDto?>>;

public sealed record DuplicataDto(
    Guid ConteudoId,
    string Titulo,
    DateTimeOffset CriadoEm,
    NivelConfiancaDuplicata Nivel);

public enum NivelConfiancaDuplicata { Alta, Media }
```

### CriarConteudoCommand Extension for Collections and Dedup Override
```csharp
// Extend existing command to support collection creation
public sealed record CriarConteudoCommand(
    Guid UsuarioId,
    string Titulo,
    string? Descricao = null,
    string? Anotacoes = null,
    PapelConteudo Papel = PapelConteudo.Item,            // NEW
    TipoColetanea? TipoColetanea = null,                 // NEW
    FormatoMidia Formato = FormatoMidia.Nenhum,          // NEW
    bool IgnorarDuplicata = false)                        // NEW: user clicked "Criar mesmo assim"
    : IRequest<Resultado<Guid>>;
```

### Source Management Commands
```csharp
// Add source
public sealed record AdicionarFonteCommand(
    Guid ConteudoId,
    Guid UsuarioId,
    TipoFonte Tipo,
    string Valor,
    string? Plataforma = null) : IRequest<Resultado<Unit>>;

// Reorder: accept full ordered list of source IDs
public sealed record ReordenarFontesCommand(
    Guid ConteudoId,
    Guid UsuarioId,
    IReadOnlyList<Guid> FonteIdsOrdenados) : IRequest<Resultado<Unit>>;

// Remove source
public sealed record RemoverFonteCommand(
    Guid ConteudoId,
    Guid UsuarioId,
    Guid FonteId) : IRequest<Resultado<Unit>>;
```

## State of the Art

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| EF Core implicit join tables | Explicit join entity with payload | EF Core 5+ | Required for contextual annotation on ConteudoColetanea |
| Avalonia 0.10 OpenFileDialog | IStorageProvider.OpenFilePickerAsync | Avalonia 11.0 | Must use new API -- old one removed |
| FeatureFlags placeholder (all false) | Must enable Phase 4 flags | Phase 4 | Technical standards require `coletaneas`, `fontes_com_fallback`, `deduplicacao` flags enabled |

**Deprecated/outdated:**
- `OpenFileDialog` class: Removed in Avalonia 11. Use `IStorageProvider.OpenFilePickerAsync` instead.
- SukiUI components: SukiUI 2.1.0 is theme-only for Avalonia 0.10.12. FluentTheme is the project standard since Phase 2.

## Open Questions

1. **Feature flag activation strategy**
   - What we know: `FeatureFlagsPlaceholder` always returns false. Tech standards mandate flags `coletaneas`, `fontes_com_fallback`, `deduplicacao` for Phase 4.
   - What's unclear: Whether to implement real feature flag storage now or just hardcode Phase 4 flags as enabled.
   - Recommendation: Update `FeatureFlagsPlaceholder` to return `true` for these three flags. Real flag storage is a Phase 9 concern.

2. **Source mutation strategy in aggregate**
   - What we know: `Fonte` is an owned entity of `Conteudo` (OwnsMany). The `_fontes` list is accessed via field. `AdicionarFonte` enforces I-07 (unique priority).
   - What's unclear: How to implement `RemoverFonte` and `ReordenarFontes` since `Fonte` properties are `init`-only (`Prioridade { get; init; }`).
   - Recommendation: Add `RemoverFonte(Guid fonteId)` and `ReordenarFontes(IReadOnlyList<Guid> fonteIdsOrdenados)` methods to `Conteudo`. For reorder, clear `_fontes` and re-add with new priorities (since Prioridade is init-only, reconstruct the objects). EF Core OwnsMany tracks by identity (Id), so removing and re-adding with same IDs may cause issues -- instead, use a `SetPrioridade(int)` setter on `Fonte` by changing to `{ get; set; }` (init was premature for a mutable ordering concern).

3. **Position management for Guiada collections**
   - What we know: D-04 says new items appended at end. UI-SPEC says reordering within Guiada deferred to Phase 11.
   - What's unclear: Whether to validate contiguous positions (I-09) now or defer.
   - Recommendation: Auto-assign positions as `MAX(Posicao) + 1` on add. When removing an item, do NOT compact positions (leave gaps). Compact only when loading for display (ORDER BY Posicao). This avoids rewriting all positions on every remove. I-09 validation (no gaps, no duplicates) can be enforced at display time rather than storage time, simplifying the implementation.

## Project Constraints (from CLAUDE.md)

- **Build:** `dotnet build` / `dotnet test` -- TreatWarningsAsErrors enforced globally.
- **Module dependency rule:** Modules depend ONLY on `DiarioDeBordo.Core` and `Module.Shared`. No inter-module references. Cross-module via MediatR.
- **Resultado<T>:** All service methods return `Resultado<T>`, never throw for expected business flows.
- **PaginatedList<T>:** Every list-returning component must use this. No unbounded lists.
- **SEG-02:** Every repository method includes `usuarioId`. Every query filters by `usuarioId`.
- **Portuguese naming:** Domain entities and domain-layer names in Portuguese. Infrastructure terms in English.
- **Strings.resx:** All user-visible strings in `.resx` files with `pt-BR` culture.
- **AutomationProperties:** On every interactive element (WCAG 2.2 AAA).
- **BannedSymbols.txt:** No insecure crypto. Build errors on violations.
- **CA1812:** Suppressed on MediatR handlers.
- **CA1707:** Suppressed in test projects (Given_When_Then).
- **InternalsVisibleTo:** Tests.Integration and Tests.E2E can access internal classes.

## Validation Architecture

### Test Framework
| Property | Value |
|----------|-------|
| Framework | xUnit + NSubstitute 5.3.0 + Testcontainers.PostgreSql 4.11.0 |
| Config file | `tests/Tests.Domain/Tests.Domain.csproj`, `tests/Tests.Integration/Tests.Integration.csproj` |
| Quick run command | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Coletanea"` |
| Full suite command | `dotnet test` |

### Phase Requirements to Test Map
| Req ID | Behavior | Test Type | Automated Command | File Exists? |
|--------|----------|-----------|-------------------|-------------|
| ACE-05 | Sources with priority, fallback, add/remove/reorder | unit + integration | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Fonte" -x` | Partial (invariant tests exist, handler tests needed) |
| ACE-06 | Create Guiada/Miscelanea, add items, progress tracking | unit + integration | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Coletanea" -x` | Partial (ColetaneaInvariantTests exists, needs expansion) |
| ACE-07 | Composition with cycle protection | integration | `dotnet test tests/Tests.Integration/ --filter "FullyQualifiedName~Ciclo" -x` | No (skipped test exists, needs real implementation) |
| ACE-08 | Contextual annotations on collection-content relation | unit + integration | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~AnotacaoContextual" -x` | No |
| ACE-10 | Duplicate detection (URL + normalized title) | unit + integration | `dotnet test tests/Tests.Domain/ --filter "FullyQualifiedName~Deduplicacao" -x` | No |
| SEG-04 | Appendix A scenarios 1-5 pass end-to-end | integration | `dotnet test tests/Tests.Integration/ --filter "FullyQualifiedName~CenarioApendiceA" -x` | No |

### Sampling Rate
- **Per task commit:** `dotnet test tests/Tests.Domain/ -x` (fast, ~5 seconds)
- **Per wave merge:** `dotnet test` (full suite, ~30 seconds + container startup)
- **Phase gate:** Full suite green before `/gsd:verify-work`

### Wave 0 Gaps
- [ ] `tests/Tests.Domain/Acervo/ConteudoColetaneaTests.cs` -- covers ACE-06, ACE-07, ACE-08
- [ ] `tests/Tests.Domain/Acervo/DeduplicacaoTests.cs` -- covers ACE-10
- [ ] `tests/Tests.Domain/Acervo/FonteManagementTests.cs` -- covers ACE-05 (add/remove/reorder handlers)
- [ ] `tests/Tests.Integration/Repositorios/ColetaneaRepositoryTests.cs` -- covers ACE-07 cycle detection
- [ ] `tests/Tests.Integration/CenarioApendiceATests.cs` -- covers SEG-04 scenarios 1-5
- [ ] Activate skipped tests in `ColetaneaInvariantTests.cs` (I-08, I-09)
- [ ] Framework install: none needed -- xUnit, NSubstitute, Testcontainers already configured

## Sources

### Primary (HIGH confidence)
- Codebase analysis: all source files in `src/` and `tests/` directories -- read directly
- `especificacoes/1 - definicao-de-dominio.md` sections 4.2 (Coletanea), 4.3 (Fonte), 4.4.2 (Deduplicacao), Appendix A scenarios 1-5
- `04-CONTEXT.md` -- locked decisions D-01 through D-18
- `04-UI-SPEC.md` -- approved visual and interaction contract

### Secondary (MEDIUM confidence)
- [Avalonia 11 File Dialogs docs](https://docs.avaloniaui.net/docs/basics/user-interface/file-dialogs) -- OpenFilePickerAsync API
- [Avalonia 11 RadioButton docs](https://docs.avaloniaui.net/docs/reference/controls/buttons/radiobutton) -- GroupName for exclusive selection
- [EF Core Many-to-Many with Payload](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many) -- join entity configuration
- [.NET String.Normalize for diacritics removal](https://www.meziantou.net/how-to-remove-diacritics-from-a-string-in-dotnet.htm) -- FormD + NonSpacingMark pattern

### Tertiary (LOW confidence)
- None -- all findings verified against codebase or official documentation

## Metadata

**Confidence breakdown:**
- Standard stack: HIGH -- no new packages needed, all patterns established in Phases 2-3
- Architecture: HIGH -- domain model exists, patterns proven, UI-SPEC approved
- Pitfalls: HIGH -- identified from direct codebase analysis (owned entities, FK constraints, feature flags placeholder)

**Research date:** 2026-04-06
**Valid until:** 2026-05-06 (stable -- all dependencies pinned, no fast-moving ecosystem concerns)
