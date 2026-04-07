# Phase 4: Curadoria — Coletâneas e Fontes - Context

**Gathered:** 2026-04-05
**Status:** Ready for planning

<domain>
## Phase Boundary

O Pilar 1 está completo: o usuário organiza seu acervo em coletâneas (Guiada e Miscelânea), registra fontes com prioridade/fallback, e o sistema detecta duplicações.

Entrega concreta:
- Coletâneas Guiada e Miscelânea: criar, visualizar, gerenciar itens, acompanhar progresso sequencial (Guiada)
- Composição de coletâneas (coletânea-mãe contendo coletâneas-filhas) com proteção contra ciclos DFS
- Anotações contextuais pertencentes à relação conteúdo-coletânea (ACE-08)
- Fontes com prioridade e fallback no modal de detalhe de conteúdo (ACE-05)
- Imagens de capa: upload de arquivo local, uma imagem por conteúdo (capa simples)
- Deduplicação: dois níveis de detecção + aviso na criação (ACE-10)
- Cenários offline 1–5 do Apêndice A da Definição de Domínio v3 funcionando (SEG-04)

**Fora do escopo:**
- Coletâneas do tipo Subscrição (alimentadas por fontes externas) — Phase 6
- Galeria de múltiplas imagens por conteúdo — Phase 11
- Merge manual de duplicatas — Phase 11 (Refinamento)
- Busca full-text e filtros combinados — Phase 7
- Operações em lote — Phase 7

</domain>

<decisions>
## Implementation Decisions

### Navegação de Coletâneas na UI

- **D-01:** Coletâneas e itens convivem na **mesma AcervoView** com filtro por tipo no topo: [Itens] [Coletâneas] [Todos]. Card de coletânea tem visual distinto — badge do tipo (Guiada/Miscelânea), contagem de itens, barra de progresso percentual para Guiadas.
- **D-02:** Abrir uma coletânea = **modal** (similar ao ConteudoDetalheWindow). O modal de coletânea exibe a lista de itens com paginação. Sub-coletâneas aninhadas navegam dentro do mesmo modal (substituição de lista com breadcrumb) — sem modais dentro de modais.
- **D-03:** Coletâneas Guiadas mostram no modal: indicador de "próximo item" e numeração sequencial dos itens. Coletâneas Miscelâneas mostram itens sem numeração nem indicador sequencial.
- **D-04:** O usuário adiciona itens a uma coletânea a partir do modal da coletânea (botão "Adicionar item" abre seleção via lista do acervo). Remove itens diretamente da lista interna do modal.

### Anotações Contextuais (ACE-08)

- **D-05:** Anotações contextuais são acessadas **dentro do modal da coletânea**: cada item na lista tem um botão 📝. Abre campo de texto inline (ou mini-modal) com label "Anotação nesta coletânea". A anotação global do conteúdo permanece separada e inalterada.
- **D-06:** Quando um item tem anotação contextual numa coletânea, o botão 📝 tem indicador visual diferenciado (ex: cor preenchida). Sem anotação = ícone vazio/outline.

### Deduplicação (ACE-10)

- **D-07:** Critério **dois níveis de confiança**:
  - **Alta confiança:** URL de fonte exata — mesmo valor em qualquer fonte (Url, Rss, Identificador). Zero falso positivo.
  - **Média confiança:** Título normalizado (trim, lowercase, diacríticos removidos, pontuação removida) — funciona para conteúdos sem fonte configurada.
- **D-08:** Fluxo: **aviso na criação**. Ao criar um conteúdo onde o sistema detecta um candidato a duplicata, mostra banner com: nome do candidato, data de criação, botões "Ver este" (abre modal do existente) e "Criar mesmo assim". O usuário decide — não há bloqueio nem merge automático.
- **D-09:** Nível de confiança exibido no aviso: "Fonte idêntica detectada" (alta) vs. "Título semelhante encontrado" (média). Ajuda o usuário a avaliar o aviso.
- **D-10:** Sem lista de manutenção de duplicatas nesta fase. Merge manual de registros — diferido para Phase 11.

### Fontes com Prioridade e Fallback (ACE-05)

- **D-11:** **Nova seção "Fontes"** no accordion do ConteudoDetalheWindow (ao lado de Identificação, Avaliação, Organização, Histórico). Lista de fontes onde **posição na lista = prioridade** (1ª = maior prioridade).
- **D-12:** Reordenação via **setas ↑↓** por item. Adicionar nova fonte via botão "+ Adicionar fonte" com formulário inline (tipo de fonte + valor). Remover via botão X por item.
- **D-13:** Tipos de fonte disponíveis: Url, ArquivoLocal, Rss, Identificador (alinhado com enum `TipoFonte` existente). Seleção por dropdown no formulário inline.
- **D-14:** **Hierarquia de autoridade de metadados** (manual > automático): metadados inseridos manualmente pelo usuário prevalecem sobre qualquer dado buscado automaticamente de fontes externas. Esta regra é enforced no application layer — não pergunta ao usuário em caso de conflito, apenas respeita o que ele definiu manualmente.

### Imagens de Capa

- **D-15:** **Capa simples**: o usuário pode adicionar uma imagem de capa por conteúdo via upload de arquivo local. Inserida no Expander de Identificação do ConteudoDetalheWindow (campo visual no topo do modal).
- **D-16:** A invariante I-04 (máx. 20 imagens) e I-05 (máx. 10MB) já existem em `Conteudo.AdicionarImagem()`. Nesta fase, a UI limita a 1 imagem (capa). Galeria de múltiplas imagens fica para Phase 11.
- **D-17:** Ao exibir o card da coletânea na lista, se houver imagem de capa, ela aparece como thumbnail no card (pequeno, lado esquerdo). Cards sem imagem mostram placeholder genérico com ícone de tipo.

### Pesquisa Científica Obrigatória

- **D-18:** Antes de qualquer decisão de layout ou interação para o modal de coletânea e o accordeon de fontes, o researcher e o planner **DEVEM consultar artigos científicos revisados por pares** sobre:
  - Organização de listas interativas em interfaces desktop (orderable lists, contextual annotations)
  - Padrões de navegação modal com estados aninhados (breadcrumb dentro de modal)
  - Indicadores de progresso sequencial em interfaces de gestão de conteúdo
  - Prevenção de erros na criação (duplicate warnings, confirmação não-intrusiva)
  Filosofia do projeto: disclosure progressivo, economia cognitiva, conformidade WCAG 2.2 AAA.

### Claude's Discretion

- Representação visual do badge de tipo de coletânea no card (ícone, cor, texto)
- Comportamento exato do breadcrumb dentro do modal de coletânea ao navegar coletâneas aninhadas
- Animações de transição ao trocar o filtro Itens/Coletâneas/Todos na AcervoView
- Debounce e threshold da detecção de duplicatas por título normalizado

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Especificações de Domínio
- `especificacoes/1  - definicao-de-dominio.md` §4.2 — Tipos de coletânea (Guiada, Miscelânea, Subscrição), configurações comportamentais (ordenação, acompanhamento sequencial), anotações contextuais, proteção de ciclos. LEITURA OBRIGATÓRIA.
- `especificacoes/1  - definicao-de-dominio.md` §4.3 — Fontes: tipos, prioridade, fallback, hierarquia de metadados. LEITURA OBRIGATÓRIA.
- `especificacoes/1  - definicao-de-dominio.md` §4.4.2 — Identidade e deduplicação: intenção de design, regra de domínio (um conteúdo = um registro).
- `especificacoes/1  - definicao-de-dominio.md` Apêndice A — Cenários 1–5 que devem passar ao final desta fase (SEG-04).
- `especificacoes/3 - mapa-de-contexto.md` — Bounded contexts, linguagem ubíqua, relação Acervo ↔ Agregação.
- `especificacoes/5 - technical-standards.md` — Stack, arquitetura, segurança, padrões de código. Fonte da verdade técnica.

### Requisitos desta Fase
- `.planning/REQUIREMENTS.md` — ACE-05, ACE-06, ACE-07, ACE-08, ACE-10, SEG-04 são os requisitos desta fase.

### Decisões de Fases Anteriores
- `.planning/phases/01-modelagem-t-tica-ddd/01-CONTEXT.md` — Modelo tático do BC Acervo (agregados, invariantes, repositórios).
- `.planning/phases/02-walking-skeleton/02-CONTEXT.md` — Arquitetura, UI shell, padrões estabelecidos.
- `.planning/phases/03-acervo-b-sico/03-CONTEXT.md` — Padrões de modal popup, disclosure progressivo, accordion, categorias chips, relações bidirecionais. LEITURA OBRIGATÓRIA para manter consistência de UI.

### Código Existente (Infrastructure disponível)
- `src/DiarioDeBordo.Core/Entidades/Conteudo.cs` — `Criar()` factory já suporta `Papel=Coletanea + TipoColetanea`. `AdicionarImagem()` com invariantes I-04/I-05 já implementado. `AdicionarFonte()` com I-07 já implementado.
- `src/DiarioDeBordo.Core/Entidades/Fonte.cs` — Entidade completa (TipoFonte, Valor, Plataforma, Prioridade).
- `src/DiarioDeBordo.Core/Repositorios/IColetaneaRepository.cs` — `ObterDescendentesAsync()` para DFS de ciclos já definido.
- `src/DiarioDeBordo.Core/Enums/Enums.cs` — `TipoColetanea`, `TipoFonte`, `PapelConteudo` já existem.
- `src/DiarioDeBordo.UI/Views/AcervoView.axaml` — View base a ser estendida com filtro Itens/Coletâneas/Todos.
- `src/DiarioDeBordo.UI/Views/ConteudoDetalheWindow.axaml` — Modal de detalhe base; nova seção Fontes + Capa a adicionar.
- `src/Modules/Module.Acervo/Commands/` — Padrão MediatR estabelecido (CriarConteudoCommand, AtualizarConteudoCommand, etc.).

### Documentação Técnica
- `docs/adr/` — ADRs 001-005 com decisões arquiteturais consolidadas.
- `docs/domain/acervo.md` — Modelo tático completo do BC Acervo.
- `docs/threat-model/` — Threat model STRIDE (referência para decisões de segurança).

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `IColetaneaRepository.ObterDescendentesAsync`: DFS cycle detection já definida — usar diretamente na validação de composição de coletâneas.
- `Conteudo.Criar(papel=PapelConteudo.Coletanea, tipoColetanea=...)`: factory pronta — invariante I-02 já enforced (TipoColetanea obrigatório para Coletanea).
- `Conteudo.AdicionarFonte()` com invariante I-07 (prioridade única por conteúdo): pronto. A UI define ordem → backend atribui números 1, 2, 3...
- `Conteudo.AdicionarImagem()` com invariantes I-04/I-05: pronto. UI limita a 1 imagem nesta fase.
- `ConteudoDetalheWindow.axaml` + `ConteudoDetalheViewModel`: estrutura de accordion com Expanders — estender com seções Fontes e Capa.
- `CategoriaChipViewModel` e `AutoCompleteBox`: padrão de autocomplete com chips — reutilizar para seleção de itens ao adicionar à coletânea.
- `PaginacaoParams` e `PaginatedList<T>`: paginação obrigatória em todas as listagens, inclusive lista de itens dentro da coletânea.
- `Resultado<T>`: contrato de erro de todos os handlers — nunca lançar exceções para fluxos de negócio.

### Established Patterns
- Handlers: MediatR `IRequestHandler` — classes `internal sealed`, CA1812 suppressed.
- DI: Extension methods nomeados conforme CA1724.
- Strings visíveis: sempre em `Resources/Strings.resx`.
- Testes: `Given_When_Then` com underscores (CA1707 suppressed em test projects).
- Repos: todo método inclui `usuarioId` — SEG-02 obrigatório.
- Avalonia UI: FluentTheme; `AutomationProperties` em todos os elementos interativos (WCAG).
- CA1812 suppressed nos handlers MediatR (DI container instancia).

### Integration Points
- `AcervoView.axaml` / `AcervoViewModel`: adicionar filtro Papel ao ViewModel + query parameter.
- `ListarConteudosQuery`: adicionar filtro `PapelConteudo?` para separar itens de coletâneas.
- `DiarioDeBordoDbContext`: nova tabela `ConteudoColetanea` (join table com anotação contextual) + configurações EF Core.
- Migrations: nova migration para tabela de associação conteúdo-coletânea com campo de anotação contextual.
- `InternalsVisibleTo`: Tests.Integration e Tests.E2E já configurados em Infrastructure.csproj.
- `CriarConteudoCommand/Handler`: adicionar lógica de detecção de duplicatas (dois níveis) antes de persistir.

</code_context>

<specifics>
## Specific Ideas

- **Modal de coletânea** funciona como o ConteudoDetalheWindow — mesma janela Avalonia, design análogo, mas com lista de itens em vez de campos de edição. Breadcrumb no header do modal para navegação em coletâneas aninhadas.
- **Setas ↑↓ para prioridade de fontes**: a UI não usa números explícitos de prioridade — o usuário reordena visualmente e o sistema atribui 1, 2, 3... internamente. Mantém simplicidade.
- **Aviso de duplicata** aparece abaixo do campo Título em tempo real (com debounce após typing) — não como dialog bloqueante. O usuário pode ignorar e continuar preenchendo o formulário. Botão "Ver este" abre o modal do conteúdo existente em sobreposição.
- **Thumbnail de coletânea no card**: se a coletânea tiver imagem de capa, exibe thumbnail. Sem imagem: placeholder com ícone 📂 e badge colorido por tipo (Guiada = azul, Miscelânea = cinza).
- **Cenários 1–5 do Apêndice A**: Cenário 2 (CDs com fallback Spotify/YouTube) depende diretamente das fontes com prioridade. Cenário 4 (plano de estudo com coletâneas aninhadas) e Cenário 5 (franquia RE) dependem da composição de coletâneas. Todos devem ser testados como testes de integração/E2E.

</specifics>

<deferred>
## Deferred Ideas

- Coletâneas do tipo Subscrição (alimentadas por fontes externas) — Phase 6
- Galeria de múltiplas imagens por conteúdo — Phase 11 (Refinamento)
- Merge manual de duplicatas (unir dois registros em um) — Phase 11
- Lista de manutenção de duplicatas pendentes — Phase 11
- Operações em lote (mover múltiplos itens para coletânea, etc.) — Phase 7
- Ganchos (bookmarks) dentro de conteúdo referenciando posição em coletânea — Phase 8

</deferred>

---

*Phase: 04-curadoria-coletaneas-e-fontes*
*Context gathered: 2026-04-05 via discuss-phase*
