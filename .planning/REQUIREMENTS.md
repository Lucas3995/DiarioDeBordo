# Requirements: DiarioDeBordo

**Defined:** 2026-04-02
**Core Value:** O usuário decide o quê, como e quanto consome — em um sistema projetado para não sabotar seu bem-estar.

## v1 Requirements

Requirements for initial release. Each maps to roadmap phases.

### Arquitetura e Design

- [x] **ARQ-01**: Modelagem tática DDD completa para os contextos Acervo e Agregação (agregados, entidades, value objects, eventos de domínio, repositórios)
- [x] **ARQ-02**: Walking skeleton end-to-end: criar conteúdo com título, persistir, recuperar, exibir atravessando todas as camadas da arquitetura

### Acervo — Conteúdo

- [ ] **ACE-01**: Entidade Conteúdo com dois eixos: formato de mídia (livro, filme, série, podcast, vídeo, artigo, jogo, etc.) e papel estrutural (obra, episódio, capítulo, item)
- [ ] **ACE-02**: Atributos completos: título (único obrigatório), descrição, anotações, nota (avaliação pessoal), classificação (rating), progresso (estado + campo manual), histórico de consumo
- [ ] **ACE-03**: Categorias como tags livres com autocompletar e não-duplicação
- [ ] **ACE-04**: Relações entre conteúdos com bidirecionalidade e tipos de relação (ex: sequência, derivado de, referenciado em)
- [ ] **ACE-05**: Fontes com prioridade e fallback; hierarquia de autoridade de metadados (manual > automático)
- [ ] **ACE-09**: Paginação obrigatória em todas as listagens
- [ ] **ACE-10**: Deduplicação de conteúdo

### Acervo — Coletâneas

- [ ] **ACE-06**: Coletâneas: Guiada (sequencial com acompanhamento de progresso), Miscelânea (livre), Subscrição (alimentada por fontes externas via Agregação)
- [ ] **ACE-07**: Composição de coletâneas (coletânea contendo coletâneas) com proteção contra ciclos
- [ ] **ACE-08**: Anotações contextuais pertencentes à relação conteúdo-coletânea (não ao conteúdo em si)

### Integração Externa

- [ ] **INT-01**: Adaptador RSS (padrão aberto, prioritário)
- [ ] **INT-02**: Adaptador YouTube (canais e playlists)
- [ ] **INT-03**: Contrato padronizado de adaptadores (item de feed + metadados de conteúdo)
- [ ] **INT-04**: Tratamento gracioso de indisponibilidade (timeout, API fora do ar, resposta inesperada)

### Agregação

- [ ] **AGR-01**: Feed como visão efêmera montada sob demanda, não persistida
- [ ] **AGR-02**: Persistência seletiva: apenas itens com interação explícita do usuário viram registros no Acervo
- [ ] **AGR-03**: Agregador como visão consolidada de múltiplas subscrições
- [ ] **AGR-04**: Filtros do agregador: por criador/fonte, esconder consumidos, palavras-chave, ordenação cronológica
- [ ] **AGR-05**: Comportamento offline: exibir apenas itens com registro, sinalizar incompletude do feed

### Busca e Navegação

- [ ] **BUS-01**: Busca textual sobre título, descrição e anotações
- [ ] **BUS-02**: Filtros combinados: formato, papel, tipo de coletânea, categoria, nota, classificação, progresso, data, fonte
- [ ] **BUS-03**: Operações em lote: categorizar, mover, marcar concluído, remover

### Reprodução

- [ ] **REP-01**: Leitor de texto (puro, Markdown, HTML)
- [ ] **REP-02**: Player de áudio para arquivos locais
- [ ] **REP-03**: Embed de vídeo para plataformas que permitam (ex: YouTube)
- [ ] **REP-04**: Abertura externa (app padrão do OS ou app escolhido pelo usuário)
- [ ] **REP-05**: Ganchos (bookmarks dentro do conteúdo com posição e anotação)
- [ ] **REP-06**: Marcação automática de progresso (oferecida, não imposta)

### Identidade

- [ ] **IDN-01**: Autenticação local multi-usuário (credencial, sem OAuth/SSO)
- [ ] **IDN-02**: Roles fixas: consumidor e admin
- [ ] **IDN-03**: Grupos de roles criados pelo admin
- [ ] **IDN-04**: Área admin invisível para usuários sem a role de admin

### Preferências

- [ ] **PRF-01**: Temas: claro, escuro (padrão), customizáveis
- [ ] **PRF-02**: Configurações de fonte: família, tamanho, cor
- [ ] **PRF-03**: Acessibilidade: contraste elevado, redução de estímulos visuais
- [ ] **PRF-04**: Defaults globais configuráveis pelo admin, sobrescritíveis por cada usuário
- [ ] **PRF-05**: Disclosure progressivo configurável (desativar para ver todos os campos de uma vez)

### Portabilidade

- [ ] **POR-01**: Exportação de dados do usuário (conteúdos, coletâneas, progresso, anotações, configurações pessoais)
- [ ] **POR-02**: Exportação de configurações globais (admin)
- [ ] **POR-03**: Importação em outra instância (cross-OS: Linux ↔ Windows)
- [ ] **POR-04**: Formato de exportação agnóstico de plataforma, legível por humanos e documentado

### Segurança e Qualidade

- [x] **SEG-01**: Threat model criado antes da implementação das camadas de rede e persistência
- [ ] **SEG-02**: Cobertura de testes automatizados ≥ 95% (unitário + integração + e2e), mantida continuamente
- [ ] **SEG-03**: 100% dos invariantes de domínio cobertos por testes automatizados
- [ ] **SEG-04**: Todos os cenários do Apêndice A da Definição de Domínio v3 executáveis e testados de ponta a ponta
- [ ] **SEG-05**: Pentest full scope por milestone (superfícies locais, de rede, integrações externas, dados persistidos)
- [ ] **SEG-06**: Conformidade WCAG 2.2 AAA em toda a interface
- [x] **SEG-07**: ADRs documentados em docs/adr/ para cada decisão arquitetural relevante

## v2 Requirements

Deferred to future release. Tracked but not in current roadmap.

### Integrações Adicionais

- **INT-V2-01**: Adaptador Instagram (plataforma muito restritiva, deferida pós-v1)
- **INT-V2-02**: Adaptador TikTok (plataforma muito restritiva, deferida pós-v1)

### Plataforma

- **PLT-V2-01**: Aplicativo mobile (desktop-first, mobile deferido)

## Out of Scope

Explicitly excluded. Documented to prevent scope creep.

| Feature | Reason |
|---------|--------|
| Player de mídia completo | Não substitui VLC, Spotify ou leitores de e-book dedicados |
| Download/cache offline de conteúdo online | Gerencia metadados, não cópias de conteúdo |
| Componente social (perfis públicos, compartilhamento) | Ferramenta pessoal, não rede social |
| Recomendação de conteúdo novo | Não sugere conteúdo externo ao que o usuário já cadastrou |
| Plataforma de publicação | Ferramenta pessoal, não de distribuição |
| OAuth/SSO | Autenticação local por credencial é suficiente para v1 |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| ARQ-01 | Phase 1 | Complete |
| ARQ-02 | Phase 2 | Complete |
| ACE-01 | Phase 2 | Pending |
| ACE-02 | Phase 3 | Pending |
| ACE-03 | Phase 3 | Pending |
| ACE-04 | Phase 3 | Pending |
| ACE-05 | Phase 4 | Pending |
| ACE-06 | Phase 4 | Pending |
| ACE-07 | Phase 4 | Pending |
| ACE-08 | Phase 4 | Pending |
| ACE-09 | Phase 3 | Pending |
| ACE-10 | Phase 4 | Pending |
| INT-01 | Phase 5 | Pending |
| INT-02 | Phase 5 | Pending |
| INT-03 | Phase 5 | Pending |
| INT-04 | Phase 5 | Pending |
| AGR-01 | Phase 6 | Pending |
| AGR-02 | Phase 6 | Pending |
| AGR-03 | Phase 6 | Pending |
| AGR-04 | Phase 6 | Pending |
| AGR-05 | Phase 6 | Pending |
| BUS-01 | Phase 7 | Pending |
| BUS-02 | Phase 7 | Pending |
| BUS-03 | Phase 7 | Pending |
| REP-01 | Phase 8 | Pending |
| REP-02 | Phase 8 | Pending |
| REP-03 | Phase 8 | Pending |
| REP-04 | Phase 8 | Pending |
| REP-05 | Phase 8 | Pending |
| REP-06 | Phase 8 | Pending |
| IDN-01 | Phase 9 | Pending |
| IDN-02 | Phase 9 | Pending |
| IDN-03 | Phase 9 | Pending |
| IDN-04 | Phase 9 | Pending |
| PRF-01 | Phase 9 | Pending |
| PRF-02 | Phase 9 | Pending |
| PRF-03 | Phase 9 | Pending |
| PRF-04 | Phase 9 | Pending |
| PRF-05 | Phase 9 | Pending |
| POR-01 | Phase 10 | Pending |
| POR-02 | Phase 10 | Pending |
| POR-03 | Phase 10 | Pending |
| POR-04 | Phase 10 | Pending |
| SEG-01 | Phase 1 | Complete |
| SEG-02 | Phase 2 | Pending |
| SEG-03 | Phase 2 | Pending |
| SEG-04 | Phase 4 | Pending |
| SEG-05 | Phase 11 | Pending |
| SEG-06 | Phase 9 | Pending |
| SEG-07 | Phase 1 | Complete |

**Coverage:**
- v1 requirements: 47 total
- Mapped to phases: 47
- Unmapped: 0 ✓

---
*Requirements defined: 2026-04-02*
*Last updated: 2026-04-02 — initial creation from project specifications*
