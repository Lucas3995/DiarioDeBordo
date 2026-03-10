# DialogService — ESC e Backdrop Click para Dismiss

## Metadados

| Campo | Valor |
|-------|-------|
| **ID** | FIX-FRONT-002 |
| **Tipo** | Correção (acessibilidade) |
| **Prioridade** | Média (P2) |
| **sourceArtifact** | `.github/reference/fonte-de-verdade-refactoring.md` — achado #25 |
| **Diretriz violada** | WCAG 2.1, WAI-ARIA dialog pattern |

---

## 1. Contexto

O `DialogService` cria overlay via DOM direto (`document.createElement`). O backdrop visual existe (`rgba(0,0,0,0.5)`), mas não há handler para fechar via tecla ESC nem clique no backdrop. Fechar o dialog exige interação com botões internos, violando o padrão WAI-ARIA para diálogos modais.

**Estado atual:**
- Backdrop visual: implementado.
- ESC key handler: não implementado.
- Backdrop click to dismiss: não implementado.
- Testes existentes: cobrem `close()` mas não ESC/backdrop.

---

## 2. User Story

Como operador, quero fechar diálogos com ESC ou clicando fora para manter o fluxo natural sem depender exclusivamente de botões internos.

---

## 3. Critérios de aceitação

1. Pressionar ESC fecha o dialog ativo e emite `afterClosed` com `undefined`.
2. Clicar no backdrop (overlay) fecha o dialog e emite `afterClosed` com `undefined`.
3. Comportamento configurável via opções no `open()` (ex.: `disableClose: true` para impedir ESC/backdrop).
4. Focus trap: ao abrir, foco vai para o dialog; ao fechar, retorna ao elemento que abriu.
5. Cleanup: listeners removidos ao fechar.
6. Testes unitários cobrem ESC e backdrop click (cenários com e sem `disableClose`).

---

## 4. Alterações necessárias

1. **Adicionar** `keydown` listener ao `document` para ESC quando dialog ativo.
2. **Adicionar** `click` listener no overlay para backdrop dismiss (com `event.target === overlay` para evitar fechar ao clicar no conteúdo).
3. **Criar** opção `disableClose` no `DialogConfig` (default: `false`).
4. **Cleanup** dos listeners no `close()`.
5. **Focus management**: salvar `document.activeElement` ao abrir; restaurar ao fechar.

---

## 5. Requisitos técnicos/metodológicos aplicáveis

- WCAG 2.1 Success Criterion 2.1.1 (Keyboard) e WAI-ARIA dialog pattern.
- TDD: testes unitários para cada comportamento (ESC, backdrop, disableClose).
- Gestão de memória: garantir que listeners são removidos ao destruir o dialog.
- Acessibilidade: role="dialog", aria-modal="true", focus trap.
