---
todos:
  - id: frontend-dialog-mechanism
    content: Introduzir mecanismo de dialog/popup no frontend (ex.: overlay + container ou CDK Dialog) para exibir conteúdo em modal
  - id: frontend-remover-rota
    content: Remover a rota 'atualizar-posicao' de obras.routes.ts e qualquer referência direta à URL /obras/atualizar-posicao
  - id: frontend-lista-abrir-popup
    content: Na lista de obras, substituir o link "Atualizar posição" por ação que abre a popup com o formulário de atualização
  - id: frontend-componente-dialog
    content: Adaptar AtualizarPosicaoComponent para funcionar dentro da popup (fechar via callback em vez de router.navigate; opcionalmente receber referência para fechar)
  - id: frontend-atualizar-listagem
    content: Ao fechar a popup após sucesso, atualizar a listagem de obras (recarregar dados ou emissão de evento) sem navegar para outra página
  - id: frontend-tests-e2e
    content: Ajustar testes (unit e E2E) que dependem da rota /obras/atualizar-posicao para cobrir abertura/fechamento da popup e fluxo na lista
---

# Relatório de Alterações para Demanda — Atualização de obra em popup (primeira demanda)

## Resumo da demanda

A primeira demanda apontada no ficheiro *docs/demandas para a tela de atualização de obra* é: **a funcionalidade de atualizar obra deve estar em uma popup e não em uma página separada.** Ou seja, o utilizador deve abrir o formulário de “Atualizar posição” num diálogo/modal sobre a lista de obras, em vez de ser levado para uma rota dedicada (`/obras/atualizar-posicao`).

## Âmbito da análise

- **Frontend:** feature obras — rotas (`obras.routes.ts`), lista de obras (`obra-lista.component`), componente de atualização de posição (`atualizar-posicao/`), serviço `AtualizarPosicaoService`, testes unitários e E2E que referenciam a rota ou o link.
- **Backend:** sem impacto para esta demanda; a API (`GET /api/obras/:id`, `GET /api/obras/por-nome`, `PATCH /api/obras/posicao`) permanece igual.
- **Premissas:** “Popup” significa um overlay/modal na mesma página (lista de obras), com possibilidade de fechar (cancelar ou após sucesso) e, após sucesso, atualizar a listagem sem navegação. O projeto atualmente não utiliza Angular Material nem CDK; não há dialogs existentes no codebase — será necessário introduzir um mecanismo de dialog/overlay (por exemplo, overlay + container com o componente, ou adoção de `@angular/cdk/dialog`).

## Alterações necessárias

### 1. Introduzir mecanismo de dialog/popup no frontend

**Onde:** frontend, aplicação Angular (ex.: pasta `shared/` ou dentro de `features/obras/`)  
**Tipo:** Criar  
**Descrição:** Garantir que exista um mecanismo para exibir conteúdo em popup/modal (overlay + container que exibe um componente, com fundo bloqueando a página). Opções possíveis: (a) criar um serviço/componente de dialog genérico que use um container global e projete o conteúdo (ex.: com `ViewContainerRef` e um wrapper com overlay em CSS); (b) adicionar `@angular/cdk` e usar `Dialog` do CDK. O resultado deve permitir “abrir” um modal com um componente (no caso, o formulário de atualização de posição) e “fechar” programaticamente ou por ação do utilizador.  
**Requisito atendido:** Permitir que a atualização de obra seja exibida em popup em vez de página.

---

### 2. Remover a rota da página de atualização de posição

**Onde:** frontend, `features/obras/obras.routes.ts`  
**Tipo:** Remover  
**Descrição:** Remover a entrada de rota `path: 'atualizar-posicao'` que carrega `AtualizarPosicaoComponent`. Manter apenas a rota vazia que carrega a lista de obras. Garantir que nenhum outro ficheiro dependa da URL `/obras/atualizar-posicao` para navegação (ver itens 3 e 5).  
**Requisito atendido:** Deixar de ter página separada para atualização de obra.

---

### 3. Abrir a popup a partir da lista de obras

**Onde:** frontend, `features/obras/obra-lista.component.ts` e `obra-lista.component.html`  
**Tipo:** Alterar  
**Descrição:** No template, substituir o link `<a routerLink="/obras/atualizar-posicao" ...>Atualizar posição</a>` por um controlo (ex.: botão) que, ao ser clicado, abre o dialog/popup com o conteúdo do formulário de atualização de posição (uso do mecanismo do item 1). No componente da lista, injetar o que for necessário para abrir o dialog (ex.: serviço de dialog ou referência ao container) e, no handler do clique, abrir a popup passando `AtualizarPosicaoComponent` (ou um wrapper que o inclua) como conteúdo. Manter o `data-testid` adequado para testes (ex.: `link-atualizar-posicao` ou `btn-atualizar-posicao`).  
**Requisito atendido:** Acesso à funcionalidade “Atualizar posição” via popup a partir da lista.

---

### 4. Adaptar o componente de atualização para uso dentro da popup

**Onde:** frontend, `features/obras/atualizar-posicao/atualizar-posicao.component.ts` e template  
**Tipo:** Alterar  
**Descrição:** O componente hoje usa `Router` para, após sucesso ao salvar, navegar para `/obras` (`setTimeout(() => this.router.navigate(['/obras']), 1500)`). Quando estiver dentro de uma popup, em vez de navegar deve fechar a popup e, se existir callback ou evento, notificar sucesso (para a lista poder atualizar). Alterar para: (1) receber uma forma de fechar o dialog (ex.: injetar um token ou callback `DialogRef`/função `fechar()` fornecida por quem abre o dialog); (2) em caso de sucesso no salvar, após a mensagem de sucesso, chamar esse fechar e, se aplicável, sinalizar que houve alteração (ex.: emitir evento ou callback com “salvou”). Remover ou substituir o link “Voltar à lista” do template por um botão “Fechar” ou “Cancelar” que apenas fecha o dialog (sem navegação). Manter a lógica de negócio (prévia, salvar, validação) inalterada.  
**Requisito atendido:** Comportamento correto da atualização de obra dentro da popup (fechar e sinalizar sucesso em vez de mudar de rota).

---

### 5. Atualizar a listagem após fechar a popup com sucesso

**Onde:** frontend, `features/obras/obra-lista.component.ts` (e, se aplicável, o código que abre o dialog)  
**Tipo:** Alterar  
**Descrição:** Quando a popup for fechada após uma atualização (ou criação) bem-sucedida, a lista de obras deve refletir os dados novos sem que o utilizador precise recarregar a página. Isso pode ser feito: (a) passando um callback ao abrir o dialog que é chamado ao fechar com sucesso, e nesse callback recarregar os dados da lista (ex.: chamar o método que já carrega a página atual de obras); ou (b) o serviço de dialog retornando um resultado (ex.: Observable ou Promise que resolve com “salvou”/“cancelou”) e a lista recarregando quando “salvou”. Garantir que o recarregamento use a paginação/estado atuais da lista.  
**Requisito atendido:** Experiência consistente: usar a popup e ver a lista atualizada sem navegação.

---

### 6. Ajustar testes que dependem da rota ou do link

**Onde:** frontend, testes unitários (ex.: `obra-lista.component.spec.ts`, `atualizar-posicao.component.spec.ts`) e E2E (ex.: `e2e/obras.spec.ts`)  
**Tipo:** Alterar  
**Descrição:** (a) Nos testes da lista: onde se verifica `href` ou `routerLink` para `/obras/atualizar-posicao`, alterar para verificar que o botão/link existe e que, ao ser acionado, abre a popup (ex.: que o componente de atualização ou o overlay está presente no DOM). (b) Nos testes do componente de atualização: remover ou adaptar expectativas que dependam de `Router.navigate` para `/obras`; em vez disso, verificar que a ação de fechar (callback/ref) é chamada após sucesso. (c) Em E2E: se existir fluxo que navega para `/obras/atualizar-posicao`, alterar para abrir a popup a partir da lista e interagir com o formulário dentro do modal; garantir que o fechamento e a atualização da lista são cobertos se for caso de teste.  
**Requisito atendido:** Testes alinhados ao novo fluxo (popup em vez de página).

---

## Resumo executivo

- **Total de itens de alteração:** 6  
- **Por tipo:** Criar (1), Alterar (4), Remover (1)  
- **Dependências:** O item 1 (mecanismo de dialog) é pré-requisito para os itens 3, 4 e 5. Os itens 2 e 3 podem ser feitos em paralelo após o 1. O item 4 depende do contrato de “como fechar e sinalizar sucesso” definido no item 1. O item 5 depende de 3 e 4. O item 6 depende da implementação final dos itens 1–5.
