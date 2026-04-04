---
status: complete
phase: 03-acervo-basico
source: 03-01-SUMMARY.md, 03-02-SUMMARY.md, 03-03-SUMMARY.md, 03-04-SUMMARY.md, 03-05-SUMMARY.md, 03-06-SUMMARY.md
started: 2026-04-04T03:00:00Z
updated: 2026-04-04T03:30:00Z
---

## Current Test

[testing complete]

## Tests

### 1. Abrir modal de detalhe a partir da lista
expected: Clicar no ícone 👁 em um card abre a janela ConteudoDetalheWindow com título, seções Identificação/Avaliação/Organização/Histórico. Identificação começa expandida, as demais recolhidas.
result: pass

### 2. Editar conteúdo — dirty tracking
expected: Alterar qualquer campo exibe '•' no título da janela. Clicar em Cancelar com mudanças abertas exibe diálogo de confirmação antes de fechar. Clicar em Salvar persiste e fecha sem diálogo.
result: pass

### 3. Criação de conteúdo — disclosure progressivo
expected: Formulário de criação exibe apenas o campo Título por padrão. Campos adicionais aparecem conforme expandidos.
result: pass

### 4. Avaliação — Nota e Classificação
expected: NumericUpDown aceita valores 0–10 com uma casa decimal. Botões 👍/👎 funcionam como toggle de três estados (Gostei / NaoGostei / nenhum). Limpar nota define como null.
result: pass

### 5. Categorias — autocomplete e chips
expected: Digitar na caixa de categoria exibe sugestões case-insensitive. Selecionar cria chip na WrapPanel. Clicar no × do chip remove a categoria. Criar categoria nova que já existe (case diferente) não duplica.
result: pass

### 6. Relações — formulário inline e bidirecional
expected: Botão "Vincular conteúdos" abre form inline. Busca de conteúdo exibe sugestões. Ao vincular, relação aparece na lista dos dois lados (A→B e B→A). Remover relação remove ambos os lados.
result: pass

### 7. Registrar sessão — sem crash
expected: Preencher título + data no miniformulário e clicar em Registrar cria a sessão sem crash. Sessão aparece no topo da timeline do Histórico. Formulário fica aberto para entrada rápida.
result: issue
reported: "o botao registrar sessao crasha o sistema"
severity: blocker
fixed: true
fix_applied: "RegistrarSessaoHandler.cs — DataConsumo.ToUniversalTime() antes de criar filho; raiz era DateTimeOffset.Now com offset -03:00 rejeitado pelo Postgres timestamptz"

### 8. Timeline de sessões
expected: Sessões exibidas em ordem cronológica inversa (mais recente primeiro) com título, data, classificação e nota. Progresso calculado corretamente ("X de Y (Z%)" quando TotalEsperadoSessoes definido, ou "X sessões registradas").
result: pass

### 9. Paginação — navegação por números de página
expected: Rodapé exibe botões ← [1][2][3]… → com a página atual em negrito. Clicar em um número navega diretamente para aquela página. Setas ← e → ficam desabilitadas nas bordas (não somem).
result: issue
reported: "a paginacao nao tem paginas, tem só proximo e anterior. se eu quiser acessar a quinta pagina tenho que ir apertando proximo ou anterior até chegar lá"
severity: major
fixed: true
fix_applied: "AcervoViewModel: PaginasVisiveis + PaginaBotao com AsyncRelayCommand por página, janela de 9 páginas centrada no atual. AcervoView: ItemsControl com botões numéricos, setas usam IsEnabled ao invés de IsVisible."

### 10. Exclusão de conteúdo com confirmação
expected: Clicar em 🗑 exibe diálogo de confirmação com nome do conteúdo. Confirmar exclui e recarrega a lista. Cancelar não altera nada.
result: pass

## Summary

total: 10
passed: 8
issues: 2
pending: 0
skipped: 0
blocked: 0

## Gaps

- truth: "Registrar sessão persiste sem crash"
  status: fixed
  reason: "User reported: o botao registrar sessao crasha o sistema"
  severity: blocker
  test: 7
  root_cause: "DateTimeOffset.Now no SessaoData carrega offset local (-03:00); Postgres timestamptz rejeita timestamps não-UTC via npgsql"
  artifacts:
    - path: "src/Modules/Module.Acervo/Commands/RegistrarSessaoHandler.cs"
      issue: "cmd.DataConsumo passado diretamente sem ToUniversalTime()"
  missing:
    - "cmd.DataConsumo?.ToUniversalTime() na chamada de CriarComoFilho"
  fix_status: applied

- truth: "Paginação exibe números de página clicáveis"
  status: fixed
  reason: "User reported: paginação sem números, só anterior/próximo"
  severity: major
  test: 9
  root_cause: "AcervoViewModel expunha apenas TemPaginaAnterior/TemProximaPagina; a view tinha só dois botões de navegação sequencial"
  artifacts:
    - path: "src/DiarioDeBordo.UI/ViewModels/AcervoViewModel.cs"
      issue: "sem PaginasVisiveis nem lógica de geração de números de página"
    - path: "src/DiarioDeBordo.UI/Views/AcervoView.axaml"
      issue: "sem ItemsControl para botões numéricos"
  missing:
    - "PaginaBotao class com Numero/IsAtual/Comando"
    - "RecalcularPaginas() chamado após cada carga"
    - "ItemsControl em AcervoView renderizando os botões"
  fix_status: applied
