# Plano de Implementação — Sistema de Gestão e Consumo de Conteúdo com Agência do Usuário

**Versão:** 3.0
**Documentos base:** Definição de Domínio v3, Mapa de Domínio v1, Mapa de Contexto v1

---

## 1. Abordagem Geral

### 1.1. Estratégia

O desenvolvimento segue uma abordagem **incremental-iterativa com walking skeleton**: a primeira entrega é a implementação mais fina possível de funcionalidade real, atravessando todas as camadas da arquitetura de ponta a ponta. Cada incremento subsequente adiciona uma fatia vertical de funcionalidade utilizável. Entre incrementos, iterações de aprendizado permitem estudo, refatoração e validação do modelo contra a realidade de uso.

Fundamentação: Cockburn (2004, *Crystal Clear: A Human-Powered Methodology for Small Teams*) define walking skeleton como "a implementação mais fina possível de funcionalidade real que pode ser automaticamente construída, implantada e testada de ponta a ponta." O Modelo Espiral de Boehm (1987, *ACM SIGSOFT Software Engineering Notes*) formaliza que nem toda iteração produz um incremento — algumas servem primariamente como oportunidades de aprendizado, e essas são cruciais para reduzir incerteza. O padrão ISO/IEC/IEEE 24748-1 (2024) reconhece o desenvolvimento incremental como especialmente útil em projetos com alta incerteza ou requisitos em evolução.

### 1.2. Princípios do plano

**Excelência sobre velocidade.** Cada etapa pode incluir períodos de estudo dedicados quando o desenvolvedor identificar áreas onde não possui conhecimento suficiente. Esses períodos são parte do plano, não desvios dele.

**Design intencional + evolução emergente.** A arquitetura base (monolito modular, bounded contexts, separação de camadas, interfaces entre contextos) é definida intencionalmente. Os detalhes internos de cada contexto, os padrões específicos que surgem do código, e as soluções concretas emergem conforme o sistema é construído e usado. O design intencional define os limites; a evolução emergente preenche o conteúdo dentro deles.

**Decisões sobre detalhes no último momento possível.** Detalhes técnicos não são assunto deste plano. Este plano é sobre funcionalidades. Com interfaces, mocks, dummies, placeholders e inversão de dependência, é possível construir a aplicação inteira sem definir detalhes como banco de dados, protocolo de comunicação, framework de interface ou ferramenta de observabilidade até o momento em que a realidade concreta do desenvolvimento exigir — e esse momento pode ser tão tarde quanto a última etapa.

**Uso próprio como validação.** O sistema será usado pelo desenvolvedor o mais cedo possível. O feedback do uso real é o principal mecanismo de validação de qualidade do modelo de domínio e das decisões de design.

**Refatoração entre etapas.** Toda transição entre etapas inclui um momento de retrospectiva e refatoração. Código que funcionou na etapa anterior mas que a experiência de uso revelou como inadequado é corrigido antes de adicionar complexidade.

### 1.3. Estrutura de cada etapa

Toda etapa segue a mesma estrutura:

- **Objetivo:** O que essa etapa entrega e por que.
- **Preparação:** Informações e estudos que precisam estar resolvidos antes de iniciar.
- **Escopo:** O que está dentro e o que está fora desta etapa.
- **Condução:** Como proceder durante a etapa.
- **Validações:** O que precisa ser verificado durante e ao final da etapa.
- **Critério de conclusão:** Como saber que a etapa terminou com a qualidade esperada.
- **Transição:** O que fazer entre esta etapa e a próxima.

### 1.4. O que é intencional vs. o que emerge

| Aspecto | Intencional (definido de propósito) | Emergente (surge do código e do uso) |
|---|---|---|
| Arquitetura | Monolito modular, bounded contexts, separação de camadas | Padrões internos de cada módulo, granularidade de classes e métodos |
| Domínio | Entidades, agregados, invariantes, interfaces entre contextos | Implementação concreta de repositórios, helpers, utilitários |
| Persistência | Interface de repositório (contrato) | Implementação concreta, esquema, otimizações |
| Interface | Princípios (paginação, disclosure progressivo, sem scroll infinito) | Layout concreto, componentes específicos, fluxos de navegação |
| Testes | Cobertura de invariantes e cenários do Apêndice A da Definição de Domínio v3 | Granularidade dos testes, mocks específicos, fixtures |
| Integração externa | Contrato padronizado de adaptadores | Implementação de cada adaptador, tratamento de edge cases por plataforma |

---

## 2. Etapas

### Etapa 0 — Modelagem Tática DDD

**Objetivo:** Traduzir o design estratégico (Definição de Domínio v3, Mapa de Domínio v1, Mapa de Contexto v1) em design tático: identificar agregados, entidades, objetos de valor, eventos de domínio e repositórios para cada bounded context. Este é o último passo de design antes de qualquer código.

**Preparação:**
- Garantir domínio dos conceitos táticos de DDD (agregados, raízes de agregado, objetos de valor, eventos de domínio, repositórios). Se necessário, período de estudo dedicado.
- Revisar a definição de domínio v3 e o mapa de contexto para identificar ambiguidades não resolvidas que impediriam a modelagem tática.

**Escopo:**
- Dentro: modelagem tática dos contextos Acervo e Agregação (os dois cores). Modelagem leve dos contextos de suporte (suficiente para identificar interfaces, não para detalhar internamente).
- Fora: modelagem detalhada dos contextos genéricos (Identidade, Preferências) — serão detalhados quando forem implementados.

**Condução:**
- Para cada bounded context core, percorrer os cenários do Apêndice A da Definição de Domínio v3 e modelar as entidades que participam de cada cenário.
- Identificar invariantes (regras que nunca podem ser violadas) e usá-las para definir fronteiras de agregados.
- Documentar a linguagem ubíqua tática (nomes de entidades, operações, eventos) como extensão do glossário existente.

**Validações:**
- Cada cenário do Apêndice A da Definição de Domínio v3 pode ser percorrido no modelo tático sem ambiguidade.
- Nenhum agregado tem responsabilidades que pertencem a outro bounded context.
- As interfaces entre contextos (especialmente Acervo ↔ Agregação) estão definidas com clareza suficiente para serem implementadas independentemente.

**Critério de conclusão:** O modelo tático dos dois cores está documentado, os cenários foram percorridos, e as interfaces entre contextos estão definidas. Não há ambiguidades que bloqueariam a implementação.

**Transição para Etapa 1:**
- Retrospectiva: a modelagem tática revelou algo que exige correção na Definição de Domínio v3 ou no Mapa de Contexto v1? Se sim, corrigir antes de avançar.

---

### Etapa 1 — Walking Skeleton

**Objetivo:** Implementar a fatia mais fina possível de funcionalidade real, atravessando todas as camadas da arquitetura de ponta a ponta. O objetivo não é entregar valor ao usuário, mas provar que o design intencional (separação de camadas, bounded contexts, interfaces) funciona quando traduzido em código. A evolução emergente começa aqui — os padrões internos que surgem do primeiro código real informam tudo o que vem depois.

**Preparação:**
- Modelo tático da Etapa 0 completo.
- Escolher a funcionalidade mais simples que percorre todas as camadas: criar um conteúdo com título, salvar, recuperar, exibir.

**Escopo:**
- Dentro: criar um conteúdo com título (único campo obrigatório), persistir, recuperar, exibir. Testes automatizados cobrindo a funcionalidade.
- Fora: qualquer outro atributo, coletâneas, busca, reprodução, agregação.

**Condução:**
- Implementar de dentro para fora: domínio primeiro (entidade Conteúdo com título), depois persistência (salvar e recuperar), depois apresentação (exibir).
- Escrever testes desde o primeiro momento.
- Validar que a separação de camadas é real: a apresentação não conhece como a persistência funciona, o domínio não conhece a apresentação.

**Validações:**
- A funcionalidade roda de ponta a ponta.
- Testes automatizados passam.
- A separação de camadas é verificável: é possível trocar a apresentação sem alterar o domínio, e trocar a persistência sem alterar o domínio.

**Critério de conclusão:** Um conteúdo pode ser criado, salvo e recuperado de ponta a ponta, com testes. A arquitetura intencional está provada em código.

**Transição para Etapa 2:**
- Retrospectiva: a implementação revelou algum problema no design intencional ou no modelo tático? Se sim, corrigir agora — o custo é mínimo neste ponto.
- Documentar padrões que emergiram naturalmente do código e avaliar se devem ser mantidos ou são acidentais.

---

### Etapa 2 — Acervo Básico

**Objetivo:** Construir o contexto Acervo com funcionalidade suficiente para que o desenvolvedor comece a usar o sistema no dia-a-dia, substituindo o bloco de notas. Ao final desta etapa, o sistema tem valor real de uso.

**Preparação:**
- Priorizar quais atributos do conteúdo implementar primeiro. Sugestão: título, descrição, formato de mídia, subtipo, anotações, nota, classificação, progresso (estado + campo manual). Imagens e fontes podem ficar para a Etapa 3.
- Definir a interface do módulo de categorias e relações entre conteúdos.

**Escopo:**
- Dentro: atributos do conteúdo (priorizados), categorias com autocompletar e não-duplicação, relações entre conteúdos com bidirecionalidade e tipos de relação, progresso global (estado, campo manual, histórico de consumo), histórico de ações, paginação em todas as listagens.
- Fora: coletâneas (Etapa 3), fontes e fallback (Etapa 3), imagens (Etapa 3), reprodução, agregação, busca avançada.

**Condução:**
- Implementar incrementalmente: adicionar um atributo ou funcionalidade por vez, com teste, validar que o existente continua funcionando.
- Usar o sistema para cadastrar conteúdos reais (livros lidos, vídeos assistidos, jogos jogados). O uso próprio é a validação primária.
- A paginação deve estar presente desde a primeira listagem (uso saudável como preocupação transversal).
- Observar quais padrões de código emergem naturalmente e quais parecem forçados.

**Validações:**
- Cada funcionalidade implementada tem testes automatizados.
- O desenvolvedor consegue cadastrar e gerenciar conteúdos reais confortavelmente.
- Categorias não duplicam. Relações são bidirecionais. Progresso é global. Paginação funciona.
- O princípio de economia cognitiva está aplicado: formulário com disclosure progressivo, apenas título obrigatório.

**Critério de conclusão:** O desenvolvedor está usando o sistema diariamente para registrar conteúdos. O contexto Acervo (parte de gestão de conteúdo) está funcional e testado.

**Transição para Etapa 3:**
- Retrospectiva de uso: o que incomoda? O que falta para o uso ser confortável? O que o modelo de domínio assumiu que não se confirma na prática?
- Refatorar o que for necessário antes de adicionar coletâneas.
- Documentar ajustes no modelo de domínio se houver.

---

### Etapa 3 — Curadoria: Coletâneas e Fontes

**Objetivo:** Completar o contexto Acervo com coletâneas (todos os tipos e comportamentos), fontes com prioridade e fallback, e imagens. Ao final desta etapa, o pilar 1 (gestão de conteúdo pessoal) está completo.

**Preparação:**
- Revisar o modelo tático de coletâneas à luz da experiência de uso da Etapa 2. Ajustar se necessário.
- Priorizar tipos de coletânea: Guiada e Miscelânea primeiro (não dependem de internet), Subscrição depois (depende da Etapa 5).
- Definir a interface para fontes (tipos de fonte, prioridade, fallback).

**Escopo:**
- Dentro: coletâneas Guiada e Miscelânea com todos os comportamentos (ordenação, acompanhamento sequencial, composição, anotação contextual, proteção contra ciclos), fontes com prioridade e fallback, imagens do conteúdo (seleção manual do sistema de arquivos), deduplicação de conteúdo.
- Fora: coletânea tipo Subscrição (depende de Agregação e Integração), busca automática de imagens em fontes externas.

**Condução:**
- Implementar coletânea Guiada primeiro (caso mais complexo — ordenação, composição), depois Miscelânea.
- Testar proteção contra ciclos com cenários concretos (o cenário 5 do Apêndice A da Definição de Domínio v3 é um bom teste).
- Usar o sistema para criar planos de estudo, listas de filmes, organização de franquias com dados reais.

**Validações:**
- Todos os cenários do Apêndice A da Definição de Domínio v3 que não dependem de internet (cenários 1, 2, 3, 4, 5) são executáveis no sistema real.
- Proteção contra ciclos funciona. Anotações contextuais funcionam. Composição de coletâneas funciona.
- Deduplicação detecta conteúdos duplicados e permite correção manual.
- Fontes com fallback funcionam para fontes locais.

**Critério de conclusão:** O pilar 1 está completo para uso offline. O desenvolvedor organiza seu consumo de conteúdo inteiramente pelo sistema, com coletâneas, fontes locais, progresso e anotações.

**Transição para Etapa 4:**
- Retrospectiva de uso prolongado: o sistema é confortável de usar diariamente? A curadoria funciona como esperado?
- Documentar ajustes.
- Iniciar estudo sobre as plataformas externas que serão integradas na Etapa 4.

---

### Etapa 4 — Integração Externa

**Objetivo:** Construir o contexto de Integração Externa — os adaptadores que conectam o sistema às plataformas de conteúdo. Este contexto é o Anti-Corruption Layer que protege o domínio da instabilidade das plataformas externas.

**Preparação:**
- Estudar as plataformas prioritárias. Sugestão de prioridade: RSS (padrão aberto, mais estável), YouTube (mais documentada), Instagram (mais restritiva — pode ser adiada).
- Definir o contrato padronizado que os adaptadores devem entregar (formato de item de feed, formato de metadados de conteúdo).

**Escopo:**
- Dentro: adaptador RSS, adaptador YouTube (ao menos canais e playlists), contrato padronizado de item de feed e de metadados, tratamento de indisponibilidade, busca de metadados para conteúdos individuais.
- Fora: adaptadores para plataformas mais restritivas (Instagram, TikTok) — ficam para incrementos futuros. Montagem de feed (responsabilidade da Agregação, Etapa 5).

**Condução:**
- Implementar o contrato padronizado primeiro (a interface que todos os adaptadores devem cumprir).
- Implementar adaptador RSS (mais simples, valida o contrato).
- Implementar adaptador YouTube (mais complexo, valida escalabilidade do contrato).
- Testar com fontes reais que o desenvolvedor acompanha.

**Validações:**
- Adaptadores entregam dados no contrato padronizado.
- O sistema lida graciosamente com indisponibilidade (timeout, fora do ar, resposta inesperada).
- Metadados são obtidos corretamente e respeitam a hierarquia de autoridade.

**Critério de conclusão:** Os adaptadores RSS e YouTube estão funcionais e testados. O contrato está estabilizado. Novos adaptadores podem ser adicionados implementando a mesma interface.

**Transição para Etapa 5:**
- Verificar que o contrato é suficiente para o que a Agregação vai consumir.
- Documentar limitações conhecidas de cada plataforma.

---

### Etapa 5 — Agregação

**Objetivo:** Construir o contexto de Agregação — feeds de subscrição, agregador consolidado, persistência seletiva. Ao final desta etapa, o pilar 2 (agregação de fontes externas) está funcional. O sistema substitui o feed das redes sociais.

**Preparação:**
- Revisar o modelo tático da Agregação à luz do contrato padronizado estabilizado na Etapa 4.
- Implementar o tipo de coletânea Subscrição no contexto Acervo (a estrutura da coletânea pertence ao Acervo; o comportamento de montagem de feed pertence à Agregação).
- Definir a regra de transição: quando exatamente um item de feed se torna um conteúdo no Acervo.

**Escopo:**
- Dentro: coletânea tipo Subscrição (estrutura no Acervo, feed na Agregação), montagem de feed sob demanda, agregador como visão consolidada, persistência seletiva, filtros do agregador (por criador, esconder consumidos, palavras-chave), comportamento offline (itens com registro apenas), busca automática de imagens via Integração Externa.
- Fora: busca em plataformas externas (etapa posterior).

**Condução:**
- Implementar subscrição para RSS primeiro (fonte mais simples e estável).
- Implementar agregador consolidando múltiplas subscrições.
- Testar com feeds reais que o desenvolvedor acompanha.
- Validar que a distinção feed (visão efêmera) vs. registro (persistido) funciona na prática.

**Validações:**
- Feeds são montados sob demanda e não persistidos.
- Itens com interação do usuário geram registros no Acervo.
- O agregador consolida múltiplos feeds com paginação.
- Filtros funcionam.
- Offline: apenas itens com registro são exibidos, com indicador de incompletude.
- Cenários 6 e 7 do Apêndice A da Definição de Domínio v3 passam.

**Critério de conclusão:** O desenvolvedor usa o agregador diariamente para acompanhar criadores, substituindo o acesso direto às plataformas. Os dois pilares do sistema estão funcionais.

**Transição para Etapa 6:**
- Retrospectiva crítica: com ambos os pilares funcionais, o sistema cumpre seu propósito? O que falta para o uso ser completo?
- Esta é a transição mais importante do plano. O sistema já tem valor real e completo. Tudo a partir daqui é enriquecimento.
- Priorizar as próximas etapas com base no que o uso real revelou como mais necessário.

---

### Etapas 6 a 10 — Enriquecimento (ordem flexível)

A partir daqui, o sistema já entrega valor completo nos dois pilares. As próximas etapas são priorizadas pelo que o uso real revelar como mais necessário. A ordem abaixo é uma sugestão, não uma sequência fixa. A transição após a Etapa 5 é o momento de decidir qual vem primeiro.

---

#### Etapa 6 — Busca e Navegação

**Objetivo:** Tornar o sistema navegável com acervos grandes.

**Escopo:** Busca textual (título, descrição, anotações), todos os filtros da seção 5.4 da Definição de Domínio v3, combinação livre de filtros, operações em lote, busca em plataformas externas (delegando à Integração).

**Critério de conclusão:** O desenvolvedor encontra qualquer conteúdo no acervo em poucos segundos, independentemente do tamanho.

---

#### Etapa 7 — Reprodução

**Objetivo:** Consumir conteúdo por dentro do sistema quando tecnicamente viável.

**Escopo:** Leitor de texto (puro, Markdown, HTML), player de áudio para arquivos locais, embed de vídeo para plataformas que permitam, abertura externa (app padrão e app escolhido), ganchos (bookmarks dentro do conteúdo), marcação automática de progresso, configuração de comportamento por subtipo de formato.

**Critério de conclusão:** O desenvolvedor consome ao menos texto, áudio local e vídeo embed por dentro do sistema, com fallback para abertura externa.

---

#### Etapa 8 — Identidade, Preferências e Acessibilidade

**Objetivo:** Multi-usuário, personalização de interface, acessibilidade.

**Escopo:** Autenticação, roles (consumidor e admin), grupos, gerenciamento de usuários, separação de áreas (consumidor vs. admin), ocultação da área admin, temas (claro, escuro, customizáveis), configurações de fonte e acessibilidade, defaults globais configuráveis pelo admin, disclosure progressivo configurável. Migração dos dados do usuário único existente.

**Critério de conclusão:** Múltiplos usuários com dados independentes, interface personalizável, área admin funcional e invisível para não-admins.

---

#### Etapa 9 — Portabilidade

**Objetivo:** Exportação e importação de dados entre instâncias.

**Escopo:** Exportação de dados do usuário, exportação de configurações globais (admin), importação em outra instância, formato agnóstico de plataforma, legível e documentado.

**Critério de conclusão:** Um usuário pode migrar seus dados entre instâncias do sistema sem perda.

---

#### Etapa 10 — Refinamento e Robustez

**Objetivo:** Polir o sistema com base em toda a experiência de uso acumulada.

**Escopo:** Otimização de desempenho, robustez de tratamento de erros e recuperação de estados, intervenções de uso saudável (monitoramento de tempo, lembretes, relatórios, escala de cinza), alertas de configuração potencialmente não saudável, instalador para distribuição, preparação para extensibilidade futura.

**Critério de conclusão:** O sistema é robusto, performático, instalável e confortável para uso prolongado diário.

---

## 3. Visão Geral

| Etapa | Nome | Objetivo principal | Resultado utilizável |
|---|---|---|---|
| 0 | Modelagem Tática DDD | Traduzir design estratégico em tático | Não (design) |
| 1 | Walking Skeleton | Provar a arquitetura de ponta a ponta | Não (prova técnica) |
| 2 | Acervo Básico | Gestão de conteúdo utilizável | Sim — substitui o bloco de notas |
| 3 | Curadoria | Coletâneas, fontes, imagens | Sim — pilar 1 completo (offline) |
| 4 | Integração Externa | Adaptadores de plataformas | Não (infraestrutura para Etapa 5) |
| 5 | Agregação | Feeds, agregador, subscrição | Sim — pilar 2 completo (online) |
| 6+ | Enriquecimento | Busca, Reprodução, Identidade, Portabilidade, Refinamento | Sim — ordem flexível |

---

## 4. Nota sobre Ordem e Flexibilidade

A ordem das Etapas 0 a 5 é fixa — cada uma depende da anterior. A partir da Etapa 6, a ordem é ajustada com base no que o uso real revelar como mais prioritário. A transição entre cada etapa é o momento de recalcular prioridades. O plano é um guia, não uma obrigação.

O design intencional (bounded contexts, interfaces, princípios) é estável. O que emerge (implementações concretas, padrões internos, escolhas de detalhes) pode e deve mudar conforme o sistema evolui.
