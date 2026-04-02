---
phase: 01-modelagem-t-tica-ddd
plan: "03"
subsystem: domain
tags: [ddd, agregacao, bounded-context, ephemeral-views, csharp-contracts, mediatR]

requires: []
provides:
  - "Modelo tático BC Agregação com visões efêmeras, contratos C# e invariantes IA-01–IA-06"
  - "Contratos de integração: PersistirItemFeedCommand, ISubscricaoFontesProvider, IConteudoRegistradoProvider, IAdaptadorPlataforma"
  - "Cobertura explícita dos Cenários 6 e 7 do Apêndice A"
affects:
  - "02-walking-skeleton"
  - "docs/domain/acervo.md"
  - "DiarioDeBordo.Module.Agregacao"
  - "DiarioDeBordo.Module.IntegracaoExterna"

tech-stack:
  added: []
  patterns:
    - "Visão efêmera vs. Registro: FeedDeSubscricao/AgregadorConsolidado são DTOs sem DbSet; Conteudo é entidade persistida"
    - "Persistência seletiva: ItemFeedDto → Conteudo apenas por PersistirItemFeedCommand via ação explícita do usuário"
    - "BC Agregação sem repositórios próprios: nenhum DbContext, nenhuma migration, apenas interfaces consumidas"

key-files:
  created:
    - "docs/domain/agregacao.md"
  modified: []

key-decisions:
  - "BC Agregação não tem entidades persistidas próprias — ausência intencional por princípio de Uso Saudável e separação Registro vs. Visão"
  - "PersistirItemFeedCommand enviado exclusivamente por ação explícita do usuário — nunca automático"
  - "ItemFeedDto sem DbSet<T> — violação desse contrato é erro de arquitetura detectável em revisão de código"

patterns-established:
  - "Invariantes IA-01 a IA-06 como contrato verificável nos testes (Phase 6)"
  - "Interfaces em DiarioDeBordo.Core, implementadas em módulos concretos — inversão de dependência"

requirements-completed: ["ARQ-01"]

duration: 15min
completed: 2026-04-02
---

# Phase 01, Plan 03: Modelo Tático — BC Agregação Summary

**Modelo tático completo do BC Agregação com visões efêmeras (FeedDeSubscricao, AgregadorConsolidado), contratos C# (PersistirItemFeedCommand + 3 interfaces), invariantes IA-01–IA-06 e cobertura dos Cenários 6 e 7 do Apêndice A**

## Performance

- **Duration:** ~15 min
- **Started:** 2026-04-02T00:00:00Z
- **Completed:** 2026-04-02T00:15:00Z
- **Tasks:** 1
- **Files modified:** 1

## Accomplishments

- Documentado que BC Agregação não tem repositórios próprios — ausência intencional com justificativa arquitetural explícita (Uso Saudável + separação Registro vs. Visão)
- Contratos C# completos com assinaturas exatas: `PersistirItemFeedCommand`, `ISubscricaoFontesProvider`, `IConteudoRegistradoProvider`, `IAdaptadorPlataforma`
- Invariantes IA-01 a IA-06 documentadas com consequências de violação e mapeamento para testes futuros (Phase 6)
- Cenários 6 e 7 do Apêndice A percorridos com caminho explícito no modelo e entidades/interfaces envolvidas
- Fluxos "Montar Feed de Subscrição" e "Montar Agregador Consolidado" com algoritmos passo a passo
- Diagrama Mermaid de colaboração entre Module.Agregacao, Module.Acervo e Module.IntegracaoExterna
- Notificação `ItemFeedPersistidoNotification` documentada para atualização reativa do ViewModel

## Task Commits

1. **Task 3.1: Criar docs/domain/agregacao.md** - `a7b2d8f` (feat)

**Plan metadata:** _(final commit abaixo)_

## Files Created/Modified

- `docs/domain/agregacao.md` — Modelo tático do BC Agregação: visões efêmeras, contratos, invariantes, fluxos, cenários, diagrama Mermaid (336 linhas)

## Decisions Made

- **BC Agregação sem persistência própria:** Confirmado como decisão arquitetural intencional, não limitação técnica. Justificativa: princípio de Uso Saudável (item visto ≠ item consumido) + separação Registro vs. Visão do domínio.
- **Contratos via Core:** Interfaces definidas em `DiarioDeBordo.Core`, implementadas em módulos concretos — padrão de inversão de dependência estabelecido para todos os BCs de suporte.
- **Invariantes como contratos verificáveis:** IA-01 a IA-06 são verificáveis em revisão de código, análise estática e testes — não apenas documentação.

## Deviations from Plan

None — plano executado exatamente como escrito. Conteúdo do documento expandido com detalhes adicionais (notificação `ItemFeedPersistidoNotification`, loop no diagrama Mermaid, propriedades de garantia do Cenário 7) mas todos dentro do escopo da tarefa.

## Issues Encountered

None — diretório `docs/domain/` criado com `mkdir -p` antes da criação do arquivo (diretório não existia).

## User Setup Required

None — fase de documentação pura, sem código de produção, sem dependências externas.

## Known Stubs

None — documento de modelagem tática, sem código, sem stubs.

## Next Phase Readiness

- Modelo tático do BC Agregação pronto para consulta durante walking skeleton (Phase 2)
- Contratos `PersistirItemFeedCommand` e interfaces definidos — Phase 2 pode implementar handlers sem ambiguidade
- Invariantes IA-01 a IA-06 prontas para enforcement em revisão de código a partir da Phase 2
- Referência cruzada com `docs/domain/acervo.md` (Plan 02) via `ISubscricaoFontesProvider` e `IConteudoRegistradoProvider`

---
*Phase: 01-modelagem-t-tica-ddd*
*Completed: 2026-04-02*
