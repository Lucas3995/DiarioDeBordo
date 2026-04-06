# Phase 3: Acervo Básico — Context

**Gathered:** 2026-04-03
**Status:** Ready for planning

<domain>
## Phase Boundary

O usuário pode registrar, anotar, classificar e acompanhar seus conteúdos no dia-a-dia — o sistema substitui o bloco de notas.

Entrega concreta:
- CRUD completo de Conteúdo: criar, editar (modal popup), excluir (com confirmação)
- Atributos completos: Nota [0-10], Classificação (Gostei/Não gostei/Nulo), campo manual de progresso
- Categorias como tags livres com autocomplete inline, criação inline e deduplicação case-insensitive
- Relações bidirecionais entre conteúdos com tipos configuráveis (fixos + criação pelo usuário) e nome inverso
- Histórico de consumo como Conteúdo filho (sessão = conteúdo filho ligado por relação especial "Contém"/"Parte de"), apresentado como timeline cronológica
- Progresso do pai calculado a partir das sessões filhas; total esperado opcional
- Conteúdos filhos ocultos da lista principal (acessíveis apenas via modal do pai)
- Paginação obrigatória (já enforced desde Phase 2)
- Disclosure progressivo no formulário (apenas título por padrão)

**Fora do escopo:**
- Coletâneas (Guiada, Miscelânea, Subscrição) — Phase 4
- Fontes com fallback e hierarquia de autoridade — Phase 4
- Deduplicação de conteúdo — Phase 4
- Busca full-text e filtros combinados — Phase 7

</domain>

<decisions>
## Implementation Decisions

### Edição e Detalhe de Conteúdo

- **D-01:** Detalhe e edição via **modal popup** — não view dedicada, não painel lateral. Contexto limpo, sem sobreposituras da lista.
- **D-02:** Salvar: botões **Salvar + Cancelar** explícitos. Se houver mudanças não salvas, confirmação de descarte antes de fechar (dialog: "Descartar alterações?").
- **D-03:** Excluir disponível em **dois lugares**: (a) botão dentro do modal de detalhe; (b) ícone/botão no card da lista. Ambos exigem confirmação via dialog.
- **D-04:** Acesso ao modal via **botão explícito 'Ver detalhe' (ou ícone) no card** — não clique no card inteiro.
- **D-05:** Organização interna do modal (layout dos campos, seções, tabs vs. scroll) — **Claude's Discretion com obrigação de pesquisa**: o researcher e o planner DEVEM consultar artigos científicos revisados por pares sobre organização de formulários complexos em interfaces desktop antes de propor o layout. Filosofia do projeto: disclosure progressivo, economia cognitiva (apenas campos necessários visíveis por padrão), conformidade WCAG 2.2 AAA.

### Classificação vs. Nota

- **D-06:** São dois campos **independentes** no modelo:
  - `Nota`: decimal? [0, 10] — avaliação pessoal numérica subjetiva
  - `Classificacao`: enum? `{Nulo, Gostei, NaoGostei}` — reação imediata (thumbs up/down)
- **D-07:** São **completamente independentes** — combinações como "Nota 9 + Não gostei" são válidas.
- **D-08:** `Classificacao` tem 3 estados: Nulo (não avaliado) | Gostei | Não gostei. Null representa "ainda não classificado".

### Categorias na UI

- **D-09:** Categorias exibidas como **chips inline** no formulário de criação e no modal de edição. Campo de texto com autocomplete; chips existentes removíveis com botão X.
- **D-10:** **Criação inline**: ao digitar um nome inexistente no campo de autocomplete, o sistema sugere "Criar categoria '{nome}'". Confirmar cria a categoria e a associa imediatamente.
- **D-11:** Categorias têm **origem**: manual (criada pelo usuário) ou automática (gerada pelo sistema/importação). Distinção visual: cor diferente + asterisco `*` no início do nome do chip. O usuário pode remover ambas; remover uma categoria automática apenas oculta ela para aquele usuário (não deleta do sistema).

### Relações Bidirecionais

- **D-12:** **Tipos de relação** funcionam como categorias: conjunto pré-definido inicial + o usuário pode criar novos tipos com autocomplete inline (sem duplicatas, case-insensitive). Cada tipo tem **nome de ida + nome inverso** (definidos ao criar o tipo).
- **D-13:** Tipos pré-definidos iniciais (com inversos):
  - "Sequência" ↔ "Continuação de"
  - "Derivado de" ↔ "Derivou"
  - "Referenciado em" ↔ "Referencia"
  - "Adaptação de" ↔ "Adaptado em"
  - "Alternativa a" ↔ "Alternativa a" (simétrico)
  - "Do mesmo tipo que" ↔ "Do mesmo tipo que" (simétrico)
  - "Complemento de" ↔ "Complementado por"
  - "Pré-requisito para" ↔ "Requer"
- **D-14:** Criar relação: dentro do modal de detalhe/edição, seção dedicada "Relações". Fluxo: (1) busca e seleção do conteúdo B navegando na lista do acervo, (2) seleção do tipo de relação (autocomplete com opção de criar novo tipo).
- **D-15:** Bidirecionalidade automática: ao criar A→B com tipo "Sequência", o modal de B automaticamente exibe "Continuação de A".
- **D-16:** Múltiplas relações do mesmo tipo com conteúdos diferentes: **permitido** (ex: A pode ser "Sequência" de B e de C simultaneamente).

### Histórico de Consumo (Sessões)

- **D-17:** Sessões de consumo são **Conteúdos filhos** — cada capítulo, episódio, faixa, fase etc. é um Conteúdo completo (com todos os campos: nota, classificação, anotações, progresso etc.) ligado ao pai.
- **D-18:** Ligação pai-filho via **relação especial** "Contém" (no pai) / "Parte de" (no filho). É uma relação com tratamento e apresentação diferenciados das relações comuns — exibida como **timeline cronológica visual** no modal do pai.
- **D-19:** Conteúdos filhos são **ocultos da lista principal do acervo** — acessíveis apenas via modal do pai na timeline.
- **D-20:** Criar filho: botão **"Registrar sessão"** no modal do pai → miniformulário rápido. Apenas o mínimo necessário é visível por padrão (ex: título/posição + data). Campos adicionais revelados sob demanda. Registrar múltiplos filhos deve ser ágil (sem obrigar preencher tudo para cada um).
- **D-21:** Progresso do pai calculado **automaticamente** com base nas sessões filhas: `filhosConsumidos / totalEsperado * 100%`.
  - `totalEsperado` é **opcional** — o usuário pode definir (ex: "24 episódios"), mas não é obrigado.
  - Se não definido: progresso mostra contagem absoluta (ex: "12 sessões registradas").
  - Se definido: mostra percentual (ex: "50% — 12 de 24").

### Claude's Discretion

- Organização interna do modal de detalhe/edição (D-05): layout dos campos, seções, tabs vs. scroll — **pesquisa em literatura científica obrigatória** antes de implementar.
- Representação visual da timeline de sessões filhas — Claude decide o componente Avalonia mais adequado.
- Animações e transições no modal (abertura, fechamento, confirmação de descarte).
- Comportamento do campo de autocomplete de categorias e tipos de relação (debounce, tamanho mínimo de prefixo, número de sugestões).

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Especificações de Domínio
- `especificacoes/1 - definicao-de-dominio.md` — Regras de negócio, invariantes (I-01 a I-12), Apêndice A. Atenção às invariantes I-08 e I-09 (stubs de Phase 2 que devem ser ativados aqui).
- `especificacoes/3 - mapa-de-contexto.md` — Bounded contexts, linguagem ubíqua, relação Acervo ↔ Agregação.
- `especificacoes/5 - technical-standards.md` — Stack, arquitetura, segurança, padrões de código. Fonte da verdade técnica.

### Decisões de Fases Anteriores
- `.planning/phases/01-modelagem-t-tica-ddd/01-CONTEXT.md` — Modelo tático do BC Acervo (agregados, entidades, value objects, repositórios).
- `.planning/phases/02-walking-skeleton/02-CONTEXT.md` — Decisões de arquitetura, UI shell, PostgreSQL bundled, estrutura de projetos.

### Estado Atual do Projeto
- `.planning/REQUIREMENTS.md` — ACE-01, ACE-02, ACE-03, ACE-04, ACE-09 são os requisitos desta fase.
- `.planning/PROJECT.md` — Key Decisions e contexto acumulado de fases anteriores.

### Código Existente (Infrastructure disponível)
- `src/DiarioDeBordo.Core/Entidades/Conteudo.cs` — Aggregate Root atual (já tem Nota, Progresso, Fontes, Imagens; falta Classificacao).
- `src/DiarioDeBordo.Core/Repositorios/IConteudoRepository.cs` — Contrato de persistência atual.
- `src/DiarioDeBordo.Core/Repositorios/ICategoriaRepository.cs` — ObterOuCriarAsync + ListarComAutocompletarAsync já definidos.
- `src/DiarioDeBordo.Core/Enums/Enums.cs` — FormatoMidia, PapelConteudo, TipoColetanea, EstadoProgresso etc.
- `src/DiarioDeBordo.Infrastructure/Repositorios/CategoriaRepository.cs` — Implementação pronta (case-insensitive, deduplicação).
- `src/DiarioDeBordo.UI/Views/AcervoView.axaml` — Lista com estado vazio, card básico, botão adicionar.
- `src/DiarioDeBordo.UI/Views/CriarConteudoView.axaml` — Formulário com disclosure progressivo (título + detalhes opcionais).
- `src/Modules/Module.Acervo/Commands/CriarConteudoCommand.cs` — Command + Handler para criação.
- `src/Modules/Module.Acervo/Queries/ListarConteudosQuery.cs` — Query paginada.

### Documentação Técnica
- `docs/adr/` — ADRs 001-005 com decisões arquiteturais consolidadas.
- `docs/domain/` — Modelo tático documentado na Phase 1.
- `docs/threat-model/` — Threat model STRIDE (referência para decisões de segurança desta fase).

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `CategoriaRepository`: implementação completa de ObterOuCriarAsync (case-insensitive, atômica) e ListarComAutocompletarAsync — pronto para uso na UI.
- `Conteudo.Criar()`: factory com validação de invariantes I-01 e I-02 — manter o padrão.
- `Conteudo.DefinirNota()`: invariante I-03 já enforced.
- `CriarConteudoView.axaml`: disclosure progressivo já implementado — estender para o modal de edição.
- `PaginacaoParams` e `PaginatedList<T>`: em DiarioDeBordo.Core.Primitivos e Module.Shared — usar em todas as novas queries.
- `Resultado<T>`: contrato de erro de todos os handlers — nunca lançar exceções para fluxos de negócio.

### Missing Pieces (Phase 3 must add)
1. `Classificacao` enum/campo no Conteudo (Gostei/NaoGostei/Nulo)
2. `Relacao` entity + `ITipoRelacaoRepository` + `IRelacaoRepository` contratos em Core
3. `SessionConsumo` concept — relação especial "Contém"/"Parte de" (via mesma entidade Relacao com IsSessionRelation flag ou tipo dedicado)
4. `TotalEsperadoSessoes` opcional no Conteudo (para cálculo de progresso %)
5. `IsFilho` flag ou query filter para ocultar conteúdos filhos da lista principal
6. Commands: EditarConteudoCommand, RemoverConteudoCommand, AdicionarCategoriaCommand, RemoverCategoriaCommand, CriarRelacaoCommand, RemoverRelacaoCommand, RegistrarSessaoCommand
7. Queries: ObterConteudoDetalheQuery (com categorias, relações, sessões)
8. UI: ConteudoDetalheModal (Avalonia Window ou Dialog), componente de chips de categorias, timeline de sessões

### Established Patterns
- Handlers: MediatR IRequestHandler — classes `internal sealed`, CA1812 suppressed.
- DI: Extension methods nomeados conforme convenção CA1724 (ex: AcervoServiceCollectionExtensions).
- Strings visíveis: sempre em `Resources/Strings.resx` — nunca hardcoded.
- Testes: `Given_When_Then` com underscores (CA1707 suppressed em test projects).
- Repos: todo método inclui `usuarioId` — SEG-02 obrigatório.
- Avalonia UI: FluentTheme (SukiUI abandonado); AutomationProperties em todos os elementos interativos (WCAG).

### Integration Points
- `AcervoView.axaml.cs` / `AcervoViewModel`: ponto de entrada para abrir modal de detalhe.
- `DiarioDeBordoDbContext`: adicionar DbSet para Relacao, configurações EF Core.
- Migrations: nova migration para Classificacao, Relacao, TipoRelacao, TotalEsperadoSessoes.
- `InternalsVisibleTo`: Tests.Integration e Tests.E2E já configurados em Infrastructure.csproj.

</code_context>

<specifics>
## Specific Ideas

- **Tipos de relação pré-definidos** são seeds de banco (Migrations ou seeder), não enums hardcoded — permite que o usuário adicione novos tipos.
- **Timeline de sessões**: relação do tipo "Contém"/"Parte de" com flag especial ou tipo dedicado `TipoRelacaoSessao`. A query de sessões filhas deve ordenar por `ConteudoFilho.CriadoEm` (data de registro da sessão).
- **Classificação no card da lista**: pode aparecer como ícone de thumbs up/down no card — feedback visual rápido sem abrir modal.
- **Progresso calculado**: `Progresso.PorcentagemCalculada` pode ser uma propriedade computed (não persistida) que lê a contagem de filhos vs. TotalEsperadoSessoes.
- **Miniformulário de sessão**: título/posição + data são os únicos campos obrigatórios; nota, classificação e anotações são opcionais e revelados por disclosure progressivo — mantém agilidade para registrar múltiplas sessões.

</specifics>

<deferred>
## Deferred Ideas

- Importação em bulk de sessões (ex: "já li até o capítulo 50" gera 50 filhos de uma vez) — Phase 10 (Portabilidade) ou Phase 11 (Refinamento).
- Visualização de progresso em heatmap/calendário estilo GitHub contributions — Phase 11 (Refinamento).
- Estatísticas de consumo (tempo médio por sessão, ritmo semanal) — Phase 11 (Refinamento).
- Exportação do histórico de sessões — Phase 10 (Portabilidade).

</deferred>

---

*Phase: 03-acervo-b-sico*
*Context gathered: 2026-04-03 via discuss-phase*
