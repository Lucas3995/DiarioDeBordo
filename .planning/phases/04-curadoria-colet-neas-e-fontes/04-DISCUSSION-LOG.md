# Phase 4: Curadoria — Coletâneas e Fontes - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions captured in CONTEXT.md — this log preserves the discussion.

**Date:** 2026-04-05
**Phase:** 04 — Curadoria — Coletâneas e Fontes
**Mode:** discuss
**Areas discussed:** Navegação de Coletâneas, Anotações Contextuais, Deduplicação, Fontes e Imagens na UI

---

## Gray Areas Presented

1. **Navegação de coletâneas** — Como coletâneas aparecem e são navegadas na UI? Seção dedicada vs. lista única com filtro? O que acontece quando o usuário "entra" numa coletânea?
2. **Anotações contextuais** — Como o usuário acessa e edita a anotação que pertence à relação conteúdo-coletânea (ACE-08)? Dentro do modal do conteúdo? Dentro da view da coletânea?
3. **Deduplicação** — Quais critérios definem um "duplicado"? Qual o fluxo: aviso na criação, lista de manutenção, merge manual? (ACE-10)
4. **Fontes e imagens na UI** — A entidade Fonte já existe mas sem UI. Como o usuário adiciona e reordena fontes com prioridade/fallback? Imagens de capa estão no escopo desta fase?

Nota do usuário: Pesquisa em artigos de periódicos estritamente científicos e validados por pares para melhor execução desta fase — capturada como D-18 (requisito mandatório para researcher e planner).

---

## Decisions Made

### Navegação de Coletâneas

| Decisão | Escolha | Motivo |
|---------|---------|--------|
| Onde aparecem na UI | Lista única com filtro [Itens][Coletâneas][Todos] na AcervoView | Usuário escolheu — evita salto de navegação entre seções |
| Abrir coletânea | Modal (similar ao ConteudoDetalheWindow) | Usuário escolheu — consistente com padrão modal da Phase 3 |

Implicação definida por Claude: coletâneas aninhadas navegam dentro do mesmo modal (substituição de lista + breadcrumb), sem modais empilhados.

### Anotações Contextuais

| Decisão | Escolha |
|---------|---------|
| Onde editar | Dentro do modal da coletânea — botão 📝 por item, campo inline |

### Deduplicação

| Decisão | Escolha |
|---------|---------|
| Critério | Dois níveis: URL exata (alta confiança) OU título normalizado (média confiança) |
| Fluxo de resolução | Aviso na criação + "Criar mesmo assim" (sem merge automático) |

### Fontes e Imagens na UI

| Decisão | Escolha |
|---------|---------|
| Fontes UI | Seção "Fontes" no accordion do ConteudoDetalheWindow, setas ↑↓ para reordenar |
| Imagens de capa | Sim — capa simples (1 imagem, arquivo local) nesta fase |

---

## No Corrections — Choices Confirmed

Todas as áreas foram discutidas sem contradição com fases anteriores.
