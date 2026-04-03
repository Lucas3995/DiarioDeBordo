# DiarioDeBordo

## What This Is

Aplicação desktop nativa (C#/.NET 10 + Avalonia UI + PostgreSQL), offline-first, que devolve ao usuário a agência sobre seu consumo de conteúdo. Dois pilares independentes: (1) gestão pessoal de acervo — registrar, anotar, organizar e acompanhar qualquer tipo de conteúdo sem internet; (2) agregação de fontes externas sem dark patterns — substituir o feed das redes sociais por um ambiente controlado pelo usuário, sem algoritmo de ranqueamento, scroll infinito ou métricas sociais. Não é uma rede social, não é um serviço web. É um programa local, análogo ao Calibre.

## Core Value

O usuário decide o quê, como e quanto consome — em um sistema projetado para não sabotar seu bem-estar.

## Requirements

### Validated

**Phase 1 (Modelagem Tática DDD):**
- [x] ARQ-01 — DDD tactical modeling complete (Acervo + Agregação contexts: aggregates, entities, VOs, domain events, repositories)
- [x] SEG-07 — ADRs documented in `docs/adr/`

**Phase 2 (Walking Skeleton) — Validated 2026-04-03:**
- [x] ARQ-02 — Walking skeleton E2E proven: `CriarConteudoCommand → PostgreSQL → ListarConteudosQuery` returns item (WalkingSkeletonTests, real PostgreSQL via Testcontainers)
- [x] ACE-09 — Pagination mandatory in all listings (`PaginacaoParams` enforced in `ListarConteudosQuery`; unit tests pass)
- [x] ACE-03 (partial) — Categories: case-insensitive deduplication, user isolation, prefix autocomplete verified via `CategoriaRepositoryTests`
- [x] SEG-02 (gate configured) — ≥95% coverage gate active in `.github/workflows/ci.yml` (fail if `line-rate < 0.95`)
- [x] SEG-03 (active invariants) — 44/44 active domain invariants (I-01 through I-12 except I08/I09 which are Phase 3 stubs) covered by `Tests.Domain`

### Active

**Contexto Acervo (Pilar 1 — offline-first)**
- [ ] Entidade Conteúdo com dois eixos: formato de mídia e papel estrutural
- [ ] Atributos completos: título (único obrigatório), descrição, anotações, nota, classificação, progresso, histórico
- [ ] Categorias como tags livres com autocompletar e não-duplicação
- [ ] Relações entre conteúdos com bidirecionalidade e tipos de relação
- [ ] Fontes com prioridade e fallback; hierarquia de autoridade de metadados (manual > automático)
- [ ] Coletâneas: Guiada (sequencial), Miscelânea (livre), Subscrição (alimentada por fontes externas)
- [ ] Composição de coletâneas (coletânea contendo coletâneas) com proteção contra ciclos
- [ ] Anotações contextuais (pertencentes à relação conteúdo-coletânea)
- [ ] Paginação obrigatória em todas as listagens
- [ ] Deduplicação de conteúdo

**Contexto Agregação (Pilar 2 — online)**
- [ ] Feed como visão efêmera (montado sob demanda, não persistido)
- [ ] Persistência seletiva: apenas itens com interação do usuário viram registros no Acervo
- [ ] Agregador como visão consolidada de múltiplas subscrições
- [ ] Filtros do agregador: por criador/fonte, esconder consumidos, palavras-chave, ordenação cronológica
- [ ] Comportamento offline: exibir apenas itens com registro, sinalizar incompletude

**Contexto Integração Externa**
- [ ] Adaptador RSS (padrão aberto, prioritário)
- [ ] Adaptador YouTube (canais e playlists)
- [ ] Contrato padronizado de adaptadores (item de feed + metadados)
- [ ] Tratamento gracioso de indisponibilidade (timeout, API fora do ar, resposta inesperada)

**Contexto Reprodução**
- [ ] Leitor de texto (puro, Markdown, HTML)
- [ ] Player de áudio para arquivos locais
- [ ] Embed de vídeo para plataformas que permitam
- [ ] Abertura externa (app padrão do OS ou app escolhido pelo usuário)
- [ ] Ganchos (bookmarks dentro do conteúdo)
- [ ] Marcação automática de progresso (oferecida, não imposta)

**Contexto Busca**
- [ ] Busca textual sobre título, descrição e anotações
- [ ] Filtros combinados: formato, papel, tipo de coletânea, categoria, nota, classificação, progresso, data, fonte
- [ ] Operações em lote: categorizar, mover, marcar concluído, remover

**Contexto Identidade**
- [ ] Autenticação local multi-usuário
- [ ] Roles fixas: consumidor e admin
- [ ] Grupos de roles criados pelo admin
- [ ] Área admin invisível para não-admins

**Contexto Preferências**
- [ ] Temas: claro, escuro (padrão), customizáveis
- [ ] Configurações de fonte: família, tamanho, cor
- [ ] Acessibilidade: contraste elevado, redução de estímulos visuais
- [ ] Defaults globais configuráveis pelo admin, sobrescritíveis pelo usuário
- [ ] Disclosure progressivo configurável (desativar para ver todos os campos de uma vez)

**Portabilidade**
- [ ] Exportação de dados do usuário (conteúdos, coletâneas, progresso, anotações, configurações)
- [ ] Exportação de configurações globais (admin)
- [ ] Importação em outra instância (cross-OS: Linux ↔ Windows)
- [ ] Formato agnóstico de plataforma, legível e documentado

**Segurança e Qualidade**
- [ ] Threat model criado antes da implementação das camadas de rede e persistência
- [ ] Cobertura de testes automatizados ≥ 95% (unitário + integração + e2e)
- [ ] 100% dos invariantes de domínio cobertos por testes
- [ ] Todos os cenários do Apêndice A da Definição de Domínio v3 executáveis e testados
- [ ] Pentest full scope por milestone (local + rede + integrações + dados persistidos)
- [ ] Conformidade WCAG 2.2 AAA em toda a interface
- [ ] ADRs documentados em docs/adr/ para cada decisão arquitetural relevante

### Out of Scope

- Player de mídia completo — não substitui VLC, Spotify ou leitores de e-book dedicados
- Download/cache offline de conteúdo online — gerencia metadados, não cópias
- Componente social — sem perfis públicos, compartilhamento entre instâncias ou comunicação entre usuários
- Recomendação de conteúdo novo — não sugere conteúdo externo ao que o usuário já cadastrou
- Plataforma de publicação — ferramenta pessoal, não de distribuição
- OAuth/SSO — autenticação local por credencial é suficiente para v1
- Adaptadores Instagram/TikTok — plataformas muito restritivas, deferidos pós-v1
- Aplicativo mobile — desktop-first, mobile deferido

## Context

O projeto possui design estratégico DDD completo em `especificacoes/`:
- **Definição de Domínio v3** — regras de negócio, princípios, modelo de domínio, cenários de validação (Apêndice A)
- **Mapa de Domínio v1** — classificação de subdomínios (core, suporte, genérico)
- **Mapa de Contexto v1** — bounded contexts, linguagem ubíqua, padrões de relacionamento
- **Plano de Implementação v3** — 10 etapas incrementais com walking skeleton
- **Padrões Técnicos v4** — stack, arquitetura, segurança, testes, padrões de código

**Bounded Contexts:**

| Contexto | Classificação | Prioridade |
|---|---|---|
| Acervo | Principal | 1 |
| Agregação | Principal | 2 |
| Reprodução | Suporte | — |
| Integração Externa | Suporte | — |
| Busca | Suporte | — |
| Portabilidade | Suporte | — |
| Identidade | Genérico | — |
| Preferências | Genérico | — |

**Preocupação transversal:** Uso Saudável — restrições de design que permeiam todos os contextos: sem scroll infinito, sem autoplay, sem algoritmo de ranqueamento, paginação obrigatória, intervenções configuráveis e desativáveis.

**Estratégia de desenvolvimento:** Incremental-iterativa com walking skeleton. Excelência sobre velocidade. Uso próprio como validação primária a partir da Etapa 2.

## Constraints

- **Tech Stack**: C#/.NET 10, Avalonia UI + FluentTheme (SukiUI dropped — incompatible with Avalonia 11 theming API), CommunityToolkit.Mvvm, MediatR — SukiUI replaced by Window + Avalonia built-in FluentTheme in Phase 2
- **Plataforma**: Linux e Windows (cross-platform obrigatório desde a Etapa 1)
- **Segurança**: Pentest full scope por milestone; threat model antes das camadas de rede e persistência
- **Acessibilidade**: WCAG 2.2 AAA — verificado em cada fase com componente de UI
- **Testes**: ≥ 95% cobertura mantida continuamente; TDD a partir da Etapa 1
- **Arquitetura**: Monolito modular com bounded contexts; DDD tático antes de qualquer código (Etapa 0)
- **Offline-first**: Pilar 1 completo sem internet; Pilar 2 degrada graciosamente offline
- **Nullable**: `Nullable enable` + `TreatWarningsAsErrors` obrigatórios desde o primeiro projeto

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Avalonia UI + SukiUI | Único framework .NET com suporte nativo real a Linux e Windows via Skia, licença MIT, adotado pelo JetBrains | SukiUI **dropped** — incompatível com Avalonia 11 theming API; substituído por Window + FluentTheme built-in |
| Monolito modular com bounded contexts | Evita complexidade de microserviços para aplicação desktop single-user; bounded contexts garantem coesão sem acoplamento | **Confirmed** — Module.Acervo has zero Infrastructure dependency (enforced by csproj) |
| Walking skeleton como Etapa 1 | Prova a arquitetura intencional antes de construir funcionalidade; erros de design custam mais se descobertos tarde | **Proven** — E2E test traverses all layers on real PostgreSQL |
| Etapas 0-5 com ordem fixa | Dependências reais: modelagem tática antes de código, Pilar 1 antes do Pilar 2, adaptadores antes da agregação | **On track** |
| Feed como visão efêmera | Respeita Uso Saudável: sem persistência passiva de conteúdo que o usuário não escolheu explicitamente salvar | — Pending |
| Threat model criado antes da implementação | Pentest full scope por milestone exige que ameaças sejam modeladas antes de superfícies de ataque serem implementadas | — Pending |
| .NET 10 (deviation from plan) | Plan written for .NET 9; .NET 10 available and used; `global.json` updated, CI updated to `10.0.x` | **Adopted** — all tests pass on .NET 10 |
| InternalsVisibleTo for test projects | `ConteudoRepository`, `CategoriaRepository`, `ConteudoQueryService` are `internal sealed`; test projects need access | **Applied** — `InternalsVisibleTo` in `DiarioDeBordo.Infrastructure.csproj` for `Tests.Integration` and `Tests.E2E` |

## Evolution

Este documento evolui a cada transição de fase e marco de milestone.

**Após cada transição de fase** (via `/gsd:transition`):
1. Requisitos invalidados? → Mover para Out of Scope com motivo
2. Requisitos validados? → Mover para Validated com referência de fase
3. Novos requisitos surgiram? → Adicionar em Active
4. Decisões a registrar? → Adicionar em Key Decisions
5. "What This Is" ainda preciso? → Atualizar se houver desvio

**Após cada milestone** (via `/gsd:complete-milestone`):
1. Revisão completa de todas as seções
2. Core Value check — ainda é a prioridade certa?
3. Auditoria de Out of Scope — os motivos ainda são válidos?
4. Atualizar Context com estado atual

---
*Last updated: 2026-04-03 after Phase 2 (Walking Skeleton) verification — PHASE COMPLETE*
