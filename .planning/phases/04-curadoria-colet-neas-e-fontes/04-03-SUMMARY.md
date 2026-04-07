---
phase: 04-curadoria-colet-neas-e-fontes
plan: 03
subsystem: module-acervo
tags: [mediatr, commands, handlers, coletaneas, fontes, deduplicacao]
dependency_graph:
  requires: [04-01, 04-02]
  provides: [write-side-commands, cycle-protected-collection-management, source-image-management]
  affects: [04-04, 04-05]
tech_stack:
  added: []
  patterns: [resultado-t, internal-sealed-handlers, ca1812-suppression, repository-driven-handlers]
key_files:
  created:
    - src/Modules/Module.Acervo/Commands/AdicionarItemNaColetaneaCommand.cs
    - src/Modules/Module.Acervo/Commands/AdicionarItemNaColetaneaHandler.cs
    - src/Modules/Module.Acervo/Commands/RemoverItemDaColetaneaCommand.cs
    - src/Modules/Module.Acervo/Commands/RemoverItemDaColetaneaHandler.cs
    - src/Modules/Module.Acervo/Commands/AtualizarAnotacaoContextualCommand.cs
    - src/Modules/Module.Acervo/Commands/AtualizarAnotacaoContextualHandler.cs
    - src/Modules/Module.Acervo/Commands/AdicionarFonteCommand.cs
    - src/Modules/Module.Acervo/Commands/AdicionarFonteHandler.cs
    - src/Modules/Module.Acervo/Commands/RemoverFonteCommand.cs
    - src/Modules/Module.Acervo/Commands/RemoverFonteHandler.cs
    - src/Modules/Module.Acervo/Commands/ReordenarFontesCommand.cs
    - src/Modules/Module.Acervo/Commands/ReordenarFontesHandler.cs
    - src/Modules/Module.Acervo/Commands/AdicionarImagemCapaCommand.cs
    - src/Modules/Module.Acervo/Commands/AdicionarImagemCapaHandler.cs
    - src/Modules/Module.Acervo/Commands/RemoverImagemCapaCommand.cs
    - src/Modules/Module.Acervo/Commands/RemoverImagemCapaHandler.cs
    - tests/Tests.Domain/Acervo/AdicionarItemNaColetaneaHandlerTests.cs
    - tests/Tests.Domain/Acervo/FonteCommandHandlerTests.cs
  modified:
    - src/Modules/Module.Acervo/Commands/CriarConteudoCommand.cs
    - src/Modules/Module.Acervo/Commands/CriarConteudoHandler.cs
    - tests/Tests.Domain/Acervo/CriarConteudoHandlerTests.cs
key_decisions:
  - "Cycle detection for nested collections runs only when item papel is Coletanea and uses ObterDescendentesAsync."
  - "Source/image write operations mutate Conteudo aggregate methods and map DomainException to Resultado failure."
  - "CriarConteudoCommand expanded with Papel, TipoColetanea, Formato and IgnorarDuplicata, with pre-create dedup check."
requirements-completed: [ACE-05, ACE-06, ACE-07, ACE-08]
duration: 20 min
completed: 2026-04-07
---

# Phase 04 Plan 03: Write-side Commands Summary

**MediatR write pipeline for coleção items, fontes, imagens de capa e criação enriquecida de conteúdo com deduplicação opcional.**

## Performance

- **Duration:** 20 min
- **Started:** 2026-04-07T14:05:33Z
- **Completed:** 2026-04-07T14:25:33Z
- **Tasks:** 2
- **Files modified:** 21

## Accomplishments

- Implemented collection item management commands/handlers (add/remove/update contextual note) with cycle protection.
- Implemented source management (add/remove/reorder) and image cover management (add/remove) through Conteudo aggregate methods.
- Extended `CriarConteudoCommand` and handler with `Papel`, `TipoColetanea`, `Formato` and `IgnorarDuplicata`, including dedup gate via `IDeduplicacaoService`.

## Task Commits

1. **Task 1: Collection item management commands (add, remove, contextual annotation)** - `d116c8e` (feat)
2. **Task 2: Source and image management commands, enriched CriarConteudoCommand** - `c2bcd15` (feat)

## Files Created/Modified

- `src/Modules/Module.Acervo/Commands/AdicionarItemNaColetaneaHandler.cs` - add item flow with duplicate and cycle checks.
- `src/Modules/Module.Acervo/Commands/RemoverItemDaColetaneaHandler.cs` - remove association from collection.
- `src/Modules/Module.Acervo/Commands/AtualizarAnotacaoContextualHandler.cs` - persist contextual relation note.
- `src/Modules/Module.Acervo/Commands/AdicionarFonteHandler.cs` - add source with computed priority.
- `src/Modules/Module.Acervo/Commands/ReordenarFontesHandler.cs` - reorder source priorities.
- `src/Modules/Module.Acervo/Commands/AdicionarImagemCapaHandler.cs` - add manual cover image.
- `src/Modules/Module.Acervo/Commands/RemoverImagemCapaHandler.cs` - remove cover image.
- `src/Modules/Module.Acervo/Commands/CriarConteudoCommand.cs` - enriched command contract.
- `src/Modules/Module.Acervo/Commands/CriarConteudoHandler.cs` - dedup pre-check + enriched domain factory call.
- `tests/Tests.Domain/Acervo/AdicionarItemNaColetaneaHandlerTests.cs` - 4 add-item handler tests.
- `tests/Tests.Domain/Acervo/FonteCommandHandlerTests.cs` - 3 source handler tests.

## Decisions Made

- Kept all business-flow failures in `Resultado<T>` instead of throwing, preserving Phase conventions.
- Maintained `internal sealed` + `CA1812` suppression pattern across all new handlers.
- Used Portuguese domain naming and existing pagination/result conventions unchanged.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Verification commands in plan were incompatible with current environment**
- **Found during:** Task verification
- **Issue:** `dotnet ... -x` is unsupported and `src/Modules/Module.Acervo/Module.Acervo.csproj` path does not exist in repository.
- **Fix:** Used valid commands: `dotnet build src/Modules/Module.Acervo/DiarioDeBordo.Module.Acervo.csproj` and `dotnet test ... --filter ...`.
- **Files modified:** None (execution-only fix)
- **Verification:** Build and targeted tests completed successfully.
- **Committed in:** N/A (tooling-only execution adjustment)

---

**Total deviations:** 1 auto-fixed (1 blocking)
**Impact on plan:** No scope change; deviation only corrected verification command syntax/path.

## Issues Encountered

None.

## User Setup Required

None - no external service configuration required.

## Known Stubs

None. No placeholder/TODO wiring introduced in touched scope.

## Next Phase Readiness

- Plan 04-03 write-side application layer is complete and verified.
- Ready for downstream UI integration/query consumption in next plans.

## Self-Check: PASSED

- FOUND: `.planning/phases/04-curadoria-colet-neas-e-fontes/04-03-SUMMARY.md`
- FOUND: commit `d116c8e`
- FOUND: commit `c2bcd15`
