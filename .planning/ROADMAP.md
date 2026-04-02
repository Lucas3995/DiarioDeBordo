# Roadmap: DiarioDeBordo

## Overview

Aplicação desktop nativa (C#/.NET 9 + Avalonia UI + PostgreSQL), offline-first, construída em 11 fases incrementais. As fases 1–2 estabelecem fundações de design e arquitetura sem entrega de valor ao usuário; as fases 3–6 entregam os dois pilares funcionais (gestão de acervo e agregação de fontes); as fases 7–11 enriquecem com reprodução, busca, identidade, portabilidade e refinamento — nessa ordem ajustada conforme o uso real revelar prioridades.

## Phases

**Phase Numbering:**
- Integer phases (1, 2, 3): Planned milestone work
- Decimal phases (2.1, 2.2): Urgent insertions (marked with INSERTED)

Decimal phases appear between their surrounding integers in numeric order.

- [ ] **Phase 1: Modelagem Tática DDD** - Traduzir design estratégico em modelo tático de domínio antes de qualquer código
- [ ] **Phase 2: Walking Skeleton** - Provar a arquitetura intencional de ponta a ponta com a fatia mínima de funcionalidade real
- [ ] **Phase 3: Acervo Básico** - Gestão de conteúdo utilizável no dia-a-dia, substituindo o bloco de notas
- [ ] **Phase 4: Curadoria — Coletâneas e Fontes** - Completar o Pilar 1 com coletâneas, fontes, imagens e deduplicação
- [ ] **Phase 5: Integração Externa** - Construir os adaptadores que conectam o sistema às plataformas de conteúdo externas
- [ ] **Phase 6: Agregação** - Construir o Pilar 2: feeds de subscrição, agregador consolidado, persistência seletiva
- [ ] **Phase 7: Busca e Navegação** - Tornar o sistema navegável com acervos grandes
- [ ] **Phase 8: Reprodução** - Consumir conteúdo por dentro do sistema quando tecnicamente viável
- [ ] **Phase 9: Identidade, Preferências e Acessibilidade** - Multi-usuário, personalização de interface e conformidade WCAG 2.2 AAA
- [ ] **Phase 10: Portabilidade** - Exportação e importação de dados entre instâncias
- [ ] **Phase 11: Refinamento e Robustez** - Polir o sistema com base na experiência de uso acumulada

## Phase Details

### Phase 1: Modelagem Tática DDD
**Goal**: O design estratégico (Definição de Domínio v3, Mapa de Contexto v1) está traduzido em design tático completo e sem ambiguidades que bloqueariam a implementação
**Depends on**: Nothing (first phase)
**Requirements**: ARQ-01, SEG-01, SEG-07
**Success Criteria** (what must be TRUE):
  1. Cada bounded context core (Acervo, Agregação) tem seus agregados, entidades, value objects, eventos de domínio e repositórios identificados e documentados
  2. Todos os cenários do Apêndice A da Definição de Domínio v3 podem ser percorridos no modelo tático sem ambiguidade nem contradição
  3. As interfaces entre contextos (especialmente Acervo ↔ Agregação) estão definidas com contratos claros e implementáveis independentemente
  4. O threat model está documentado antes de qualquer camada de rede ou persistência ser implementada
  5. Um ADR inicial documenta as decisões arquiteturais mais relevantes (monolito modular, bounded contexts, stack)
**Plans**: TBD

### Phase 2: Walking Skeleton
**Goal**: A arquitetura intencional está provada em código: uma funcionalidade real (criar conteúdo com título) atravessa todas as camadas de ponta a ponta, com separação verificável entre domínio, persistência e apresentação
**Depends on**: Phase 1
**Requirements**: ARQ-02, SEG-02, SEG-03
**Success Criteria** (what must be TRUE):
  1. Um conteúdo pode ser criado com título, salvo no banco e recuperado — de ponta a ponta — na aplicação desktop rodando em Linux e Windows
  2. A separação de camadas é verificável: trocar a apresentação não exige alterar o domínio; trocar a persistência não exige alterar o domínio
  3. Testes automatizados cobrem o fluxo completo e passam em CI
  4. Cobertura de testes ≥ 95% está configurada e mantida desde este ponto
**Plans**: TBD
**UI hint**: yes

### Phase 3: Acervo Básico
**Goal**: O usuário pode registrar, anotar, classificar e acompanhar seus conteúdos no dia-a-dia — o sistema substitui o bloco de notas
**Depends on**: Phase 2
**Requirements**: ACE-01, ACE-02, ACE-03, ACE-04, ACE-09
**Success Criteria** (what must be TRUE):
  1. O usuário cria um conteúdo com apenas o título preenchido e pode incrementalmente adicionar descrição, anotações, nota, classificação, progresso e histórico
  2. O usuário cria e atribui categorias (tags livres) com autocompletar — o sistema impede duplicações case-insensitive
  3. O usuário cria relações bidirecionais entre conteúdos com tipo de relação; ao editar um lado, o outro é atualizado automaticamente
  4. Todas as listagens usam paginação — scroll infinito é impossível por design
  5. O formulário usa disclosure progressivo: apenas o campo obrigatório (título) aparece por padrão
**Plans**: TBD
**UI hint**: yes

### Phase 4: Curadoria — Coletâneas e Fontes
**Goal**: O Pilar 1 está completo: o usuário organiza seu acervo em coletâneas (guiada e miscelânea), registra fontes com fallback, e o sistema detecta duplicações
**Depends on**: Phase 3
**Requirements**: ACE-05, ACE-06, ACE-07, ACE-08, ACE-10, SEG-04
**Success Criteria** (what must be TRUE):
  1. O usuário cria uma coletânea Guiada com ordem explícita e acompanha seu progresso sequencial
  2. O usuário cria uma coletânea Miscelânea com itens sem ordem definida
  3. O usuário compõe coletâneas dentro de coletâneas; o sistema rejeita composições que criariam ciclos
  4. O usuário adiciona anotações contextuais a um item dentro de uma coletânea (distintas das anotações do próprio conteúdo)
  5. O usuário registra fontes para um conteúdo com prioridade e fallback; metadados manuais prevalecem sobre os automáticos
  6. O sistema detecta conteúdos duplicados e permite resolução manual
  7. Todos os cenários offline do Apêndice A (cenários 1–5 da Definição de Domínio v3) passam
**Plans**: TBD
**UI hint**: yes

### Phase 5: Integração Externa
**Goal**: Os adaptadores RSS e YouTube estão funcionais e testados; novos adaptadores podem ser adicionados implementando um único contrato padronizado
**Depends on**: Phase 4
**Requirements**: INT-01, INT-02, INT-03, INT-04
**Success Criteria** (what must be TRUE):
  1. O usuário adiciona um feed RSS real e o sistema busca seus itens com os metadados no contrato padronizado
  2. O usuário adiciona um canal YouTube real e o sistema busca seus vídeos/playlists com os metadados no contrato padronizado
  3. O sistema lida graciosamente com indisponibilidade: timeout configurável, resposta inesperada, API fora do ar — sem crash, com mensagem ao usuário
  4. Um novo adaptador pode ser implementado criando uma classe que satisfaz o contrato; sem alterar código existente
**Plans**: TBD

### Phase 6: Agregação
**Goal**: O Pilar 2 está completo: o usuário acompanha criadores via agregador, substituindo o acesso direto às plataformas, com persistência seletiva e comportamento offline gracioso
**Depends on**: Phase 5
**Requirements**: AGR-01, AGR-02, AGR-03, AGR-04, AGR-05, ACE-06
**Success Criteria** (what must be TRUE):
  1. O usuário cria uma subscrição e o feed é montado sob demanda — nenhum item é persistido automaticamente
  2. O usuário interage com um item do feed (salvar, marcar, anotar) e esse item se torna um registro no Acervo
  3. O agregador consolida múltiplas subscrições em uma visão única com paginação
  4. O usuário filtra o agregador por criador, esconde consumidos, e busca por palavras-chave; a ordem é cronológica
  5. Offline: o agregador exibe apenas itens com registro existente e sinaliza visualmente que o feed está incompleto
  6. Os cenários 6 e 7 do Apêndice A da Definição de Domínio v3 passam
**Plans**: TBD
**UI hint**: yes

### Phase 7: Busca e Navegação
**Goal**: O usuário encontra qualquer conteúdo no acervo em poucos segundos, independentemente do tamanho, e pode operar em lote
**Depends on**: Phase 6
**Requirements**: BUS-01, BUS-02, BUS-03
**Success Criteria** (what must be TRUE):
  1. O usuário busca por texto livre sobre título, descrição e anotações com resultados relevantes
  2. O usuário combina livremente filtros de formato, papel, tipo de coletânea, categoria, nota, classificação, progresso, data e fonte
  3. O usuário seleciona múltiplos conteúdos e executa operações em lote (categorizar, mover, marcar concluído, remover)
**Plans**: TBD
**UI hint**: yes

### Phase 8: Reprodução
**Goal**: O usuário consome texto, áudio local e vídeo embed por dentro do sistema, com rastreamento de posição e marcação de progresso opcional
**Depends on**: Phase 6
**Requirements**: REP-01, REP-02, REP-03, REP-04, REP-05, REP-06
**Success Criteria** (what must be TRUE):
  1. O usuário lê conteúdo texto (puro, Markdown, HTML) dentro do sistema sem abrir app externo
  2. O usuário reproduz arquivos de áudio locais dentro do sistema
  3. O usuário assiste a vídeos embed (YouTube) dentro do sistema; para plataformas sem embed, a abertura externa funciona
  4. O usuário cria ganchos (bookmarks) com posição e anotação dentro de um conteúdo e os navega posteriormente
  5. Ao fechar um conteúdo, o sistema oferece marcar o progresso — o usuário pode recusar
**Plans**: TBD
**UI hint**: yes

### Phase 9: Identidade, Preferências e Acessibilidade
**Goal**: O sistema suporta múltiplos usuários com dados independentes, interface personalizável e conformidade WCAG 2.2 AAA verificada
**Depends on**: Phase 7
**Requirements**: IDN-01, IDN-02, IDN-03, IDN-04, PRF-01, PRF-02, PRF-03, PRF-04, PRF-05, SEG-06
**Success Criteria** (what must be TRUE):
  1. O admin cria usuários, define roles (consumidor/admin) e agrupa roles; a área admin é invisível para consumidores
  2. Dois usuários têm acervos completamente independentes; um não acessa dados do outro
  3. O usuário escolhe tema (claro, escuro ou customizado) e configura fonte (família, tamanho, cor); as preferências persistem
  4. O admin define defaults globais; cada usuário pode sobrescrever os seus
  5. O usuário ativa modo de contraste elevado e redução de estímulos visuais; a interface adapta-se imediatamente
  6. Toda a interface passa na auditoria WCAG 2.2 AAA
  7. O disclosure progressivo é configurável: o usuário pode desativá-lo para ver todos os campos de uma vez
**Plans**: TBD
**UI hint**: yes

### Phase 10: Portabilidade
**Goal**: O usuário migra seus dados entre instâncias do sistema (inclusive Linux ↔ Windows) sem perda
**Depends on**: Phase 9
**Requirements**: POR-01, POR-02, POR-03, POR-04
**Success Criteria** (what must be TRUE):
  1. O usuário exporta todos os seus dados (conteúdos, coletâneas, progresso, anotações, configurações) em um arquivo legível por humanos e documentado
  2. O admin exporta as configurações globais separadamente
  3. Uma exportação feita no Linux é importada com sucesso em uma instância Windows (e vice-versa) sem perda de dados
**Plans**: TBD
**UI hint**: yes

### Phase 11: Refinamento e Robustez
**Goal**: O sistema é robusto, performático, instalável e projetado para uso sustentável a longo prazo, com intervenções de uso saudável funcionais
**Depends on**: Phase 10
**Requirements**: SEG-05
**Success Criteria** (what must be TRUE):
  1. O pentest full scope (superfícies locais, rede, integrações, dados persistidos) está completo e sem vulnerabilidades críticas abertas
  2. O usuário configura intervenções de uso saudável (monitoramento de tempo, lembretes, escalas de cinza) e pode desativá-las individualmente
  3. O sistema se recupera graciosamente de erros conhecidos (integração offline, arquivo corrompido, banco inacessível) sem perda de dados
  4. O sistema tem um instalador funcional para Linux e Windows
**Plans**: TBD
**UI hint**: yes

## Progress

**Execution Order:**
Phases execute in numeric order: 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8 → 9 → 10 → 11

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Modelagem Tática DDD | 0/0 | Not started | - |
| 2. Walking Skeleton | 0/0 | Not started | - |
| 3. Acervo Básico | 0/0 | Not started | - |
| 4. Curadoria — Coletâneas e Fontes | 0/0 | Not started | - |
| 5. Integração Externa | 0/0 | Not started | - |
| 6. Agregação | 0/0 | Not started | - |
| 7. Busca e Navegação | 0/0 | Not started | - |
| 8. Reprodução | 0/0 | Not started | - |
| 9. Identidade, Preferências e Acessibilidade | 0/0 | Not started | - |
| 10. Portabilidade | 0/0 | Not started | - |
| 11. Refinamento e Robustez | 0/0 | Not started | - |

---
*Roadmap defined: 2026-04-02*
*Last updated: 2026-04-02 — initial creation from project specifications*
