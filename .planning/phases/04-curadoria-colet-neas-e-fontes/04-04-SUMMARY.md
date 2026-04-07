---
phase: 04-curadoria-colet-neas-e-fontes
plan: 04
subsystem: api
tags: [mediatr, resultado, dto, resx, localization]
requires:
  - phase: 04-01
    provides: contratos de consulta IDeduplicacaoService/IColetaneaQueryService/IConteudoQueryService
provides:
  - queries de leitura para duplicatas, itens de coletânea e detalhe de coletânea
  - DTOs de coletânea/fonte/imagem/duplicata e enriquecimento de DTOs de conteúdo
  - 57 entradas de UI string pt-BR para curadoria de coletâneas e fontes
affects: [ui, acervo, phase-04-05]
tech-stack:
  added: []
  patterns: [MediatR internal sealed handlers com CA1812, Resultado<T>, DTO records com ExcludeFromCodeCoverage]
key-files:
  created:
    - src/Modules/Module.Acervo/Queries/VerificarDuplicataQuery.cs
    - src/Modules/Module.Acervo/Queries/VerificarDuplicataHandler.cs
    - src/Modules/Module.Acervo/Queries/ListarItensColetaneaQuery.cs
    - src/Modules/Module.Acervo/Queries/ListarItensColetaneaHandler.cs
    - src/Modules/Module.Acervo/Queries/ObterColetaneaDetalheQuery.cs
    - src/Modules/Module.Acervo/Queries/ObterColetaneaDetalheHandler.cs
    - src/Modules/Module.Acervo/DTOs/ColetaneaDetalheDto.cs
    - src/Modules/Module.Acervo/DTOs/ColetaneaItemDto.cs
    - src/Modules/Module.Acervo/DTOs/FonteDto.cs
    - src/Modules/Module.Acervo/DTOs/DuplicataDto.cs
    - src/Modules/Module.Acervo/DTOs/ImagemDto.cs
  modified:
    - src/Modules/Module.Acervo/Queries/ListarConteudosQuery.cs
    - src/Modules/Module.Acervo/Queries/ListarConteudosHandler.cs
    - src/Modules/Module.Acervo/Queries/BuscarConteudosQuery.cs
    - src/Modules/Module.Acervo/Queries/ObterConteudoHandler.cs
    - src/Modules/Module.Acervo/DTOs/ConteudoResumoDto.cs
    - src/Modules/Module.Acervo/DTOs/ConteudoDetalheDto.cs
    - src/Modules/Module.Acervo/Resources/Strings.resx
key-decisions:
  - "ListarConteudosQuery passou a aceitar PapelFiltro opcional para manter compatibilidade dos call sites existentes."
  - "ConteudoResumoDto recebeu parâmetros opcionais nos novos campos para evitar quebra imediata em testes existentes."
  - "Strings da fase foram aplicadas em Module.Acervo/Resources/Strings.resx, pois o caminho src/DiarioDeBordo.UI/Resources não existe no repositório."
patterns-established:
  - "Handlers de consulta seguem internal sealed + CA1812 + Resultado<T>."
  - "Mapeamento de enums para UI mantém ToString() no projection layer."
requirements-completed: [ACE-05, ACE-06, ACE-08, ACE-10]
duration: 2 min
completed: 2026-04-07
---

# Phase 04 Plan 04: Curadoria read-side queries, DTOs e strings Summary

**Read-side de curadoria concluído com queries MediatR para duplicação/coletâneas, DTOs enriquecidos para exibição de coleções e 57 strings pt-BR para fluxos de UI da fase 4.**

## Performance

- **Duration:** 2 min
- **Started:** 2026-04-07T14:41:39Z
- **Completed:** 2026-04-07T14:43:04Z
- **Tasks:** 3
- **Files modified:** 18

## Accomplishments
- Implementadas 3 novas query/handler pairs (`VerificarDuplicata`, `ListarItensColetanea`, `ObterColetaneaDetalhe`) e extensão de `ListarConteudosQuery` com `PapelFiltro`.
- Criados 5 novos DTO records puros com `ExcludeFromCodeCoverage` e enriquecidos `ConteudoResumoDto`/`ConteudoDetalheDto` com dados de coletânea/fontes/imagens.
- Adicionadas 57 entradas de string pt-BR em `Strings.resx` cobrindo ações, estados vazios/erro, dedup e labels de coletânea/fontes.

## Task Commits

Each task was committed atomically:

1. **Task 1: MediatR queries and handlers for dedup, collection items, and collection detail** - `8b8458b` (feat)
2. **Task 2: DTOs and enriched existing DTOs** - `f2405c3` (feat)
3. **Task 3: Strings.resx entries per UI-SPEC copywriting contract** - `1820e12` (feat)

## Files Created/Modified
- `src/Modules/Module.Acervo/Queries/*Coletanea*.cs` - queries/handlers para leitura de coletâneas.
- `src/Modules/Module.Acervo/Queries/VerificarDuplicata*.cs` - consulta de deduplicação por serviço read-side.
- `src/Modules/Module.Acervo/DTOs/*.cs` (novos/alterados) - contratos de transporte para listagem/detalhe.
- `src/Modules/Module.Acervo/Resources/Strings.resx` - contrato de copywriting da fase 4.

## Decisions Made
- `ObterColetaneaDetalheHandler` retorna `Resultado.Failure("Coletanea nao encontrada")` quando o read model não existe, conforme plano.
- Novos campos de `ConteudoResumoDto` foram adicionados com default nullable para não quebrar consumidores antigos durante transição.
- Strings da UI da fase foram mantidas no módulo Acervo (resource root existente e já consumido pelo código atual).

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Caminhos de arquivos do plano não existiam no repositório**
- **Found during:** Task 1 e Task 3
- **Issue:** Plano apontava `src/DiarioDeBordo.UI/Resources/Strings.resx` e `ObterConteudoQuery.cs`, mas o projeto usa `src/Modules/Module.Acervo/Resources/Strings.resx` e define `ObterConteudoQuery` dentro de `ListarConteudosQuery.cs`.
- **Fix:** Implementação aplicada nos caminhos reais mantendo o mesmo contrato funcional requerido.
- **Files modified:** `src/Modules/Module.Acervo/Resources/Strings.resx`, `src/Modules/Module.Acervo/Queries/ListarConteudosQuery.cs`
- **Verification:** `dotnet build src/Modules/Module.Acervo/DiarioDeBordo.Module.Acervo.csproj` e `dotnet build DiarioDeBordo.sln` com sucesso
- **Committed in:** `8b8458b`, `1820e12`

**2. [Rule 1 - Bug] Quebra de compilação em testes por mudança de assinatura do DTO**
- **Found during:** Verificação pós-Task 3 (`dotnet build DiarioDeBordo.sln`)
- **Issue:** `ConteudoResumoDto` recebeu novos parâmetros obrigatórios e quebrou `Tests.ViewModel`.
- **Fix:** Definidos valores padrão (`= null`) nos novos parâmetros para compatibilidade retroativa.
- **Files modified:** `src/Modules/Module.Acervo/DTOs/ConteudoResumoDto.cs`
- **Verification:** `dotnet build DiarioDeBordo.sln` passou após ajuste
- **Committed in:** `f2405c3`

---

**Total deviations:** 2 auto-fixed (1 blocking, 1 bug)  
**Impact on plan:** Ajustes necessários para executar o plano no código real sem alterar objetivo funcional.

## Issues Encountered
- Build completo inicialmente falhou por incompatibilidade em call site de teste; resolvido com assinatura backward-compatible no DTO.

## User Setup Required
None - no external service configuration required.

## Next Phase Readiness
- Pronto para o plano 04-05 consumir as queries/DTOs de leitura e strings de UI já localizadas.
- Sem bloqueadores técnicos pendentes para continuidade da fase.

## Self-Check: PASSED

- Files verificados em disco:
  - `.planning/phases/04-curadoria-colet-neas-e-fontes/04-04-SUMMARY.md`
  - `src/Modules/Module.Acervo/Queries/VerificarDuplicataQuery.cs`
  - `src/Modules/Module.Acervo/DTOs/ColetaneaDetalheDto.cs`
  - `src/Modules/Module.Acervo/Resources/Strings.resx`
- Commits verificados no histórico:
  - `8b8458b`
  - `f2405c3`
  - `1820e12`
