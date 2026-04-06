---
phase: 01-modelagem-t-tica-ddd
plan: "02"
subsystem: domain

tags: [ddd, csharp, domain-model, bounded-context, aggregate, repository, domain-events]

requires: []

provides:
  - "Modelo tático completo do BC Acervo: 3 agregados (Conteudo, Coletanea, Categoria) com entidades, value objects, invariantes e repositórios"
  - "12 invariantes documentadas (I-01 a I-12) com mensagens de exceção explícitas"
  - "Algoritmo DFS de detecção de ciclos em coletâneas (I-08)"
  - "5 cenários do Apêndice A percorridos com caminho explícito no modelo"
  - "Interfaces C# para os 3 repositórios do BC Acervo"
  - "Interfaces publicadas para outros BCs: ISubscricaoFontesProvider, IConteudoRegistradoProvider, IConteudoParaReproducaoProvider, IDadosExportaveisProvider"
  - "PersistirItemFeedCommand com 7 invariantes do handler"
  - "Eventos de domínio: ConteudoCriadoNotification, ProgressoAlteradoNotification, ItemFeedPersistidoNotification"
  - "Tabela invariantes → testes futuros (Phase 2)"
  - "Diagramas Mermaid para agregados Conteudo e Coletanea"

affects:
  - "02-walking-skeleton"
  - "implementation phases (BC Acervo)"
  - "Phase 2+ TDD test planning"

tech-stack:
  added: []
  patterns:
    - "Aggregate root como unidade de consistência transacional (Evans, 2003)"
    - "UsuarioId obrigatório em toda query de acesso a dados do usuário"
    - "Paginação obrigatória em todas as listagens (PaginacaoParams)"
    - "Result<T> para commands — nunca lançar exceção para fluxos esperados"
    - "DFS para detecção de ciclos em grafo de coletâneas — O(V+E)"
    - "DomainException para violações de invariante com mensagem específica"

key-files:
  created:
    - "docs/domain/acervo.md"
  modified: []

key-decisions:
  - "Coletanea é implementada como Conteudo com Papel==Coletanea — sem classe separada, distinção é de responsabilidade"
  - "Detecção de ciclos via DFS usando IColetaneaRepository.ObterDescendentesAsync antes de toda adição de item-coletânea"
  - "Progresso é global ao conteúdo — compartilhado entre todas as coletâneas que contêm o item"
  - "Anotação contextual pertence à ConteudoColetanea (relação), não ao Conteudo nem à Coletanea isoladamente"
  - "RelacaoConteudo e ConteudoCategoria ficam fora dos agregados — gerenciadas por application service por motivo de bidirecionalidade"
  - "ObterOuCriarAsync atômico em ICategoriaRepository garante unicidade case-insensitive sem race condition"

patterns-established:
  - "Repositório inclui usuarioId em todo método — invariante de segurança multi-usuário"
  - "Commands retornam Result<T>; domain exceptions capturadas no handler"
  - "Notifications (INotification) para comunicação loosely coupled entre módulos"
  - "Interfaces em DiarioDeBordo.Core, implementações em DiarioDeBordo.Module.*"

requirements-completed: ["ARQ-01"]

duration: 9min
completed: 2026-04-02
---

# Phase 01, Plan 02: Modelo Tático — BC Acervo — Summary

**Modelo tático DDD completo do BC Acervo com 3 agregados, 12 invariantes, detecção de ciclos DFS, 5 cenários do Apêndice A cobertos e contratos C# para repositórios e interfaces entre bounded contexts**

## Performance

- **Duration:** 9 min
- **Started:** 2026-04-02T21:03:36Z
- **Completed:** 2026-04-02T21:12:24Z
- **Tasks:** 1 (Task 2.1)
- **Files modified:** 1

## Accomplishments

- Criado `docs/domain/acervo.md` com 560 linhas — modelo tático completo e verificável do BC Acervo
- 12 invariantes (I-01 a I-12) documentadas com mensagens de exceção explícitas e mapeadas para testes futuros
- 5 cenários do Apêndice A percorridos com caminho explícito: entidades, operações e invariantes verificadas em cada cenário
- Interfaces C# para IConteudoRepository, IColetaneaRepository, ICategoriaRepository com assinaturas completas
- PersistirItemFeedCommand e 3 domain events (ConteudoCriadoNotification, ProgressoAlteradoNotification, ItemFeedPersistidoNotification) com contratos e invariantes do handler
- Diagrama Mermaid classDiagram para agregados Conteudo e Coletanea
- Tabela invariantes → testes futuros como guia para Phase 2

## Task Commits

1. **Task 2.1: Criar docs/domain/acervo.md** - `782bbc1` (feat)

**Plan metadata:** (a seguir — docs commit)

## Files Created/Modified

- `docs/domain/acervo.md` — Modelo tático DDD completo do BC Acervo: 3 agregados, 12 invariantes, 5 cenários Apêndice A, interfaces C#, diagramas Mermaid, tabela invariantes → testes

## Decisions Made

- **Coletanea como Conteudo com Papel==Coletanea:** Evita duplicação de estrutura, simplifica o modelo de persistência sem perder distinção de responsabilidade.
- **DFS para detecção de ciclos:** Algoritmo O(V+E) via `IColetaneaRepository.ObterDescendentesAsync` — verificado antes de cada adição de item que seja coletânea.
- **Progresso global por conteúdo:** Estado de consumo é do conteúdo, não da coletânea — simplifica a lógica e evita sincronização entre coletâneas.
- **RelacaoConteudo fora do agregado:** Bidirecionalidade impede gerenciamento dentro de um único agregado — application service cria as duas direções em uma transação.
- **ObterOuCriarAsync atômico para Categoria:** Garante unicidade case-insensitive sem expor concorrência ao caller.

## Deviations from Plan

None — plan executed exactly as written. O conteúdo do documento foi extraído diretamente das especificações e do 01-RESEARCH.md conforme instrução do plano.

## Issues Encountered

None.

## User Setup Required

None — fase de documentação pura, sem código de produção ou serviços externos.

## Next Phase Readiness

- `docs/domain/acervo.md` pronto como referência base para qualquer implementação do BC Acervo
- Invariantes numeradas e mapeadas para testes — equipe/agente pode criar testes unitários diretamente a partir da tabela
- Interfaces publicadas definidas — BC Agregação (Plan 01-03) pode continuar sem depender da implementação do Acervo
- Ciclo de vida de `PersistirItemFeedCommand` completamente especificado — walking skeleton (Phase 2) pode implementar o contrato imediatamente

---
*Phase: 01-modelagem-t-tica-ddd*
*Completed: 2026-04-02*
