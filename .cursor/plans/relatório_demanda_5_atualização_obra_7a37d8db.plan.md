---
name: Relatório demanda 5 atualização obra
overview: ""
todos:
  - id: tdd-testes-criados
    content: Testes TDD criados (backend ObrasControllerTests PATCH obra nova; frontend atualizar-posicao.component.spec.ts 404 + template)
    status: completed
  - id: backend-enum-json
    content: Configurar serialização de enum no backend (Program.cs) com JsonStringEnumConverter camelCase para aceitar tipoParaCriar como string
    status: completed
  - id: frontend-preview-404
    content: Em verPreview(), ao receber 404 e criarSeNaoExistir true, construir prévia sintética e atribuir a preview em vez de mostrar erro (atualizar-posicao.component.ts)
    status: completed
  - id: template-antes-obra-nova
    content: "No template, quando preview for de obra nova, exibir \"Antes: — (obra nova)\" em vez de posição/data (atualizar-posicao.component.html)"
    status: completed
  - id: tipo-obra-nova-opcional
    content: "(Opcional) Expor obraNova?: boolean em ObraDetalhe ou tipo estendido para template/componente (atualizar-posicao.service.ts)"
    status: completed
isProject: false
---

# Relatório de Alterações — Demanda 5: Prévia e Salvar em Obra Nova

## Resumo da demanda

Corrigir os botões **Ver prévia** e **Salvar** na situação de **obra nova** (obra que ainda não existe no sistema):

- **Prévia:** hoje, ao clicar em "Ver prévia" para um nome que não existe, aparece a mensagem *"Obra não encontrada. Marque Criar se não existir para cadastrar."* em vez de uma prévia útil.
- **Salvar:** ao salvar obra nova, a API devolve **400 Bad Request** em `PATCH /api/obras/posicao`, impedindo a criação.

O relatório abaixo descreve apenas as alterações necessárias para atender a essa demanda (não inclui refatoração, popup, autocomplete ou outras demandas do mesmo ficheiro).

---

## Âmbito da análise

- **Frontend:** componente [atualizar-posicao.component.ts](frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts), template [atualizar-posicao.component.html](frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.html), serviço [atualizar-posicao.service.ts](frontend/src/app/application/atualizar-posicao.service.ts).
- **Backend:** API [ObrasController.cs](backend/src/DiarioDeBordo.Api/Controllers/ObrasController.cs), modelo [AtualizarPosicaoObraRequest.cs](backend/src/DiarioDeBordo.Api/Models/AtualizarPosicaoObraRequest.cs), handler e validador do command AtualizarPosicao, configuração de JSON em [Program.cs](backend/src/DiarioDeBordo.Api/Program.cs).
- **Premissa:** mantém-se o fluxo atual (checkbox "Criar se não existir", campos nome/tipo/ordem para criação). A demanda 2 (remover o checkbox e usar prompt) é escopo futuro.

**Causa raiz do 400 no save (obra nova):** o frontend envia `tipoParaCriar` como string em camelCase (ex.: `"manga"`), enquanto a API não está configurada para deserializar enum como string; o binding de `TipoParaCriar` falha e o ASP.NET Core responde 400 antes de chegar ao handler.

**Causa raiz da prévia em obra nova:** o fluxo de prévia chama apenas `GET` por id/nome; para obra inexistente a API devolve 404 e o componente exibe sempre a mesma mensagem de erro, sem oferecer prévia sintética para “obra nova”.

---

## Alterações necessárias

### 1. Configurar serialização de enum no backend para aceitar string (camelCase)

**Onde:** API, [Program.cs](backend/src/DiarioDeBordo.Api/Program.cs) (ou ponto onde se configuram os controllers).

**Tipo:** Alterar.

**Descrição:** Configurar `AddControllers()` para usar `JsonStringEnumConverter` (com política de nomes em camelCase, para aceitar `"manga"`, `"manhwa"`, etc.) na serialização/deserialização JSON. Assim, o body do `PATCH /api/obras/posicao` com `tipoParaCriar: "manga"` será aceite e o 400 deixará de ocorrer na situação de obra nova.

**Requisito atendido:** Salvar em obra nova sem 400.

---

### 2. Tratar 404 na prévia como “obra nova” quando criar se não existir

**Onde:** [atualizar-posicao.component.ts](frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts) (método `verPreview()`).

**Tipo:** Alterar.

**Descrição:** No `error` do subscribe da prévia, quando `err?.status === 404` e `criarSeNaoExistir === true`, em vez de definir `erro` com a mensagem atual, construir um objeto de prévia sintético (compatível com `ObraDetalhe`: tipo, posicaoAtual, dataUltimaAtualizacaoPosicao, etc., usando `tipoParaCriar`, `novaPosicao`, `dataParaEnvio`/hoje e nome do formulário) e atribuir a `preview`, sem definir a mensagem “Obra não encontrada. Marque…”. Opcionalmente marcar essa prévia como “obra nova” (ex.: propriedade ou tipo discriminado) para o template poder exibir “Antes: —” ou “(obra nova)”.

**Requisito atendido:** Prévia em obra nova sem mostrar a mensagem de erro incorreta.

---

### 3. Exibir “Antes” adequado no template quando a prévia for de obra nova

**Onde:** [atualizar-posicao.component.html](frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.html) (bloco `@if (preview)`).

**Tipo:** Alterar.

**Descrição:** Quando a prévia for de obra nova (usar o critério definido no item 2, ex.: flag no objeto ou tipo), exibir na linha “Antes” um texto do tipo “— (obra nova)” ou “(nova obra)” em vez de “capítulo X — data”. Manter a linha “Depois” como está (nova posição e data).

**Requisito atendido:** Prévia em obra nova com mensagem clara para o utilizador.

---

### 4. (Opcional) Expor indicação de “obra nova” no tipo/interfaces do frontend

**Onde:** [atualizar-posicao.service.ts](frontend/src/app/application/atualizar-posicao.service.ts) e/ou componente, conforme convenção do projeto.

**Tipo:** Alterar.

**Descrição:** Se for usado um objeto sintético para prévia de obra nova, garantir que a interface `ObraDetalhe` (ou um tipo estendido/union) permita identificar prévia de obra nova (ex.: campo opcional `obraNova?: boolean`) para uso no template e no componente, sem quebrar usos atuais de `ObraDetalhe` vindos da API.

**Requisito atendido:** Consistência de tipos e manutenção.

---

## Resumo executivo

- **Total de itens de alteração:** 4 (item 4 opcional).
- **Por tipo:** Alterar (4).
- **Dependências:** Itens 2 e 3 dependem do critério “obra nova” na prévia; item 1 é independente e resolve o 400 no save. A ordem sugerida é: 1 (backend), 2 e 4 (frontend), 3 (template).

Após essas alterações, na situação de obra nova: (1) “Ver prévia” mostrará uma prévia sintética em vez da mensagem “Obra não encontrada. Marque…”; (2) “Salvar” deixará de receber 400 quando `tipoParaCriar` for enviado como string em camelCase.