# Plano de Implementação — Sistema de Gestão e Consumo de Conteúdo com Agência do Usuário

**Versão:** 2.0
**Documentos base:** Definição de Domínio v3, Mapa de Domínio v1, Mapa de Contexto v1

---

## 1. Abordagem Geral

### 1.1. Estratégia

O desenvolvimento segue uma abordagem **incremental-iterativa com walking skeleton**: a primeira entrega é a implementação mais fina possível de funcionalidade real, atravessando todas as camadas da arquitetura de ponta a ponta. Cada incremento subsequente adiciona uma fatia vertical de funcionalidade utilizável. Entre incrementos, iterações de aprendizado permitem estudo, refatoração e validação do modelo contra a realidade de uso.

Fundamentação: Cockburn (2004, *Crystal Clear: A Human-Powered Methodology for Small Teams*) define walking skeleton como "a implementação mais fina possível de funcionalidade real que pode ser automaticamente construída, implantada e testada de ponta a ponta." O Modelo Espiral de Boehm (1987, *ACM SIGSOFT Software Engineering Notes*) formaliza que nem toda iteração produz um incremento — algumas servem primariamente como oportunidades de aprendizado, e essas são cruciais para reduzir incerteza. O padrão ISO/IEC/IEEE 24748-1 (2024) reconhece o desenvolvimento incremental como especialmente útil em projetos com alta incerteza ou requisitos em evolução.

### 1.2. Princípios do plano

**Excelência sobre velocidade.** Cada etapa pode incluir períodos de estudo dedicados quando o desenvolvedor identificar áreas onde não possui conhecimento suficiente. Esses períodos são parte do plano, não desvios dele.

**Design intencional + evolução emergente.** A arquitetura base (monolito modular, bounded contexts, separação UI/Domínio/Persistência, interfaces entre contextos) é definida intencionalmente. Os detalhes internos de cada contexto, os padrões específicos que surgem do código, e as soluções técnicas concretas emergem conforme o sistema é construído e usado. O design intencional define os limites; a evolução emergente preenche o conteúdo dentro deles.

**Decisões técnicas no momento mais tarde possível.** Decisões sobre detalhes (banco de dados, ORM, framework de UI, ferramentas de observabilidade, protocolos de comunicação) são tomadas apenas quando a etapa corrente as exige de fato. Com DDD (repository pattern, interfaces), TDD (testes guiando o design), SOLID (inversão de dependência) e mocks, a maioria das decisões técnicas pode ser adiada muito além do que o senso comum sugere. A seção 3 mapeia cada decisão ao momento onde ela é de fato necessária.

**Uso próprio como validação.** O sistema será usado pelo desenvolvedor o mais cedo possível. O feedback do uso real é o principal mecanismo de validação de qualidade do modelo de domínio e das decisões de design.

**Refatoração entre etapas.** Toda transição entre etapas inclui um momento de retrospectiva e refatoração. Código que funcionou na etapa anterior mas que a experiência de uso revelou como inadequado é corrigido antes de adicionar complexidade.

### 1.3. Estrutura de cada etapa

Toda etapa segue a mesma estrutura:

- **Objetivo:** O que essa etapa entrega e por que.
- **Decisões técnicas requeridas:** Quais decisões precisam ser tomadas nesta etapa e não antes (com justificativa do porquê agora).
- **Preparação:** Informações, estudos e decisões que precisam estar resolvidos antes de iniciar.
- **Escopo:** O que está dentro e o que está fora desta etapa.
- **Condução:** Como proceder durante a etapa.
- **Validações:** O que precisa ser verificado durante e ao final da etapa.
- **Critério de conclusão:** Como saber que a etapa terminou com a qualidade esperada.
- **Transição:** O que fazer entre esta etapa e a próxima.

---

## 2. Etapas

### Etapa 0 — Modelagem Tática DDD

**Objetivo:** Traduzir o design estratégico (definição de domínio, mapa de domínio, mapa de contexto) em design tático: identificar agregados, entidades, objetos de valor, eventos de domínio e repositórios para cada bounded context. Este é o último passo de design antes de qualquer código.

**Decisões técnicas requeridas:** Nenhuma. Esta etapa é inteiramente de design de domínio, agnóstica de tecnologia. As interfaces de repositório são definidas sem saber qual banco de dados as implementará. Os eventos de domínio são definidos sem saber qual mecanismo os transportará.

**Preparação:**
- Garantir domínio dos conceitos táticos de DDD (agregados, raízes de agregado, objetos de valor, eventos de domínio, repositórios). Se necessário, período de estudo dedicado.
- Revisar a definição de domínio v3 e o mapa de contexto para identificar ambiguidades não resolvidas que impediriam a modelagem tática.

**Escopo:**
- Dentro: modelagem tática dos contextos Acervo e Agregação (os dois cores). Modelagem leve dos contextos de suporte (suficiente para identificar interfaces, não para detalhar internamente).
- Fora: modelagem detalhada dos contextos genéricos (Identidade, Preferências) — serão detalhados quando forem implementados.

**Condução:**
- Para cada bounded context core, percorrer os cenários do Apêndice A da definição de domínio e modelar as entidades que participam de cada cenário.
- Identificar invariantes (regras que nunca podem ser violadas) e usá-las para definir fronteiras de agregados.
- Documentar a linguagem ubíqua tática (nomes de classes, métodos, eventos) como extensão do glossário existente.

**Validações:**
- Cada cenário do Apêndice A pode ser percorrido no modelo tático sem ambiguidade.
- Nenhum agregado tem responsabilidades que pertencem a outro bounded context.
- As interfaces entre contextos (especialmente Acervo ↔ Agregação) estão definidas com clareza suficiente para serem implementadas independentemente.

**Critério de conclusão:** O modelo tático dos dois cores está documentado, os cenários foram percorridos, e as interfaces entre contextos estão definidas. Não há ambiguidades que bloqueariam a implementação.

**Transição para Etapa 1:**
- Retrospectiva: a modelagem tática revelou algo que exige correção na definição de domínio ou no mapa de contexto? Se sim, corrigir antes de avançar.
- O modelo tático é o insumo para a próxima etapa.

---

### Etapa 1 — Walking Skeleton

**Objetivo:** Implementar a fatia mais fina possível de funcionalidade real, atravessando todas as camadas da arquitetura: UI → Domínio → Persistência. O objetivo não é entregar valor ao usuário, mas provar que o design intencional (separação de camadas, bounded contexts, interfaces) funciona de ponta a ponta. A evolução emergente começa aqui — os padrões internos que surgem do primeiro código real informam tudo o que vem depois.

**Decisões técnicas requeridas:**
- **Linguagem de programação:** Necessária agora porque é preciso escrever código. Justificativa: sem linguagem não há código. É a decisão mais fundamental e irreversível.
- **Framework de testes:** Necessário agora porque o walking skeleton é construído com TDD — os testes vêm antes da implementação. Justificativa: sem framework de teste, não há TDD.
- **Repositório e CI mínimo:** Necessário agora porque o walking skeleton deve ser "automaticamente construível e testável" (Cockburn). Justificativa: o pipeline pode ser mínimo (build + testes), mas precisa existir.
- **Mecanismo de persistência inicial:** Não decidir o banco de dados — implementar repositórios em memória (in-memory). Justificativa: o repository pattern do DDD permite adiar a escolha do banco. In-memory é suficiente para provar a arquitetura.
- **Interface mínima:** Não decidir o framework de UI definitivo — pode ser CLI, uma interface gráfica mínima ou testes como única interface. Justificativa: a separação UI/Domínio do design intencional significa que a UI pode ser trocada depois. O que importa agora é provar que a camada de domínio funciona. Se uma UI mínima for necessária para validar o ponta-a-ponta, escolher a solução mais simples possível com consciência de que pode ser substituída.

**Preparação:**
- Modelo tático da Etapa 0 completo.
- Escolher a funcionalidade mais simples que percorre todas as camadas: criar um conteúdo com título, salvar, recuperar, exibir.

**Escopo:**
- Dentro: criar um conteúdo com título (único campo obrigatório), persistir (in-memory), recuperar, exibir (interface mínima). Testes automatizados. Pipeline mínimo de CI.
- Fora: qualquer outro atributo, coletâneas, busca, reprodução, agregação. Decisão de banco de dados. Decisão de framework de UI definitivo. Decisão de ORM.

**Condução:**
- Implementar de dentro para fora: domínio primeiro (entidade Conteúdo com título), depois repositório in-memory (salvar e recuperar), depois interface mínima (exibir).
- Escrever testes desde o primeiro momento.
- Validar que a separação de camadas é real: a interface não conhece como a persistência funciona, o domínio não conhece a interface.

**Validações:**
- A funcionalidade roda de ponta a ponta.
- Testes automatizados passam no pipeline de CI.
- A separação de camadas é verificável: é possível trocar a interface sem alterar o domínio, e trocar o repositório sem alterar o domínio.

**Critério de conclusão:** Um conteúdo pode ser criado, salvo e recuperado de ponta a ponta, com testes, rodando no pipeline. A arquitetura intencional está provada. As decisões sobre banco, UI definitiva e ORM continuam em aberto.

**Transição para Etapa 2:**
- Retrospectiva: a implementação revelou algum problema no design intencional ou no modelo tático? Se sim, corrigir agora — o custo é mínimo neste ponto.
- Documentar padrões que emergiram naturalmente do código e avaliar se devem ser mantidos ou são acidentais.

---

### Etapa 2 — Acervo Básico

**Objetivo:** Construir o contexto Acervo com funcionalidade suficiente para que o desenvolvedor comece a usar o sistema no dia-a-dia, substituindo o bloco de notas. Ao final desta etapa, o sistema tem valor real de uso.

**Decisões técnicas requeridas:**
- **Persistência durável:** O sistema agora será usado diariamente — os dados precisam sobreviver a reinicializações. É necessário escolher um mecanismo de persistência durável. Justificativa: in-memory não serve mais para uso real, mas a escolha pode ser a mais simples que funcione (arquivo local, SQLite, etc.). Banco de dados robusto pode ser adiado se uma solução mais leve atender. A interface de repositório definida no walking skeleton permite trocar a implementação sem alterar o domínio.
- **Interface visual:** O desenvolvedor vai usar o sistema diariamente — precisa de uma interface confortável. Se a Etapa 1 usou CLI ou interface mínima, agora é o momento de decidir a interface visual. Se a Etapa 1 já usou uma interface gráfica mínima, avaliar se ela é adequada para uso prolongado ou se deve ser substituída. Justificativa: uso diário exige conforto visual. A separação de camadas garante que trocar a interface não impacta o domínio.

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
- Observar quais padrões de código emergem naturalmente e quais parecem forçados. Os naturais são mantidos; os forçados são questionados.

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

**Decisões técnicas requeridas:**
- Nenhuma nova obrigatória. As decisões de persistência e interface da Etapa 2 continuam servindo. Se a complexidade das coletâneas (composição, proteção contra ciclos, anotações contextuais) revelar limitações na persistência escolhida, esse é o momento de reavaliar — mas não antes de a limitação se manifestar concretamente.

**Preparação:**
- Revisar o modelo tático de coletâneas à luz da experiência de uso da Etapa 2. Ajustar se necessário.
- Priorizar tipos de coletânea: Guiada e Miscelânea primeiro (não dependem de internet), Subscrição depois (depende da Etapa 5).
- Definir a interface para fontes (tipos de fonte, prioridade, fallback).

**Escopo:**
- Dentro: coletâneas Guiada e Miscelânea com todos os comportamentos (ordenação, acompanhamento sequencial, composição, anotação contextual, proteção contra ciclos), fontes com prioridade e fallback, imagens do conteúdo (seleção manual do sistema de arquivos — busca automática depende de integração externa), deduplicação de conteúdo.
- Fora: coletânea tipo Subscrição (depende de Agregação e Integração), busca automática de imagens em fontes externas.

**Condução:**
- Implementar coletânea Guiada primeiro (caso mais complexo — ordenação, composição), depois Miscelânea.
- Testar proteção contra ciclos com cenários concretos (o cenário 5 do Apêndice A é um bom teste).
- Usar o sistema para criar planos de estudo, listas de filmes, organização de franquias com dados reais.

**Validações:**
- Todos os cenários do Apêndice A que não dependem de internet (cenários 1, 2, 3, 4, 5) são executáveis no sistema real.
- Proteção contra ciclos funciona. Anotações contextuais funcionam. Composição de coletâneas funciona.
- Deduplicação detecta conteúdos duplicados e permite correção manual.
- Fontes com fallback funcionam para fontes locais (ex: arquivo em dois caminhos diferentes).

**Critério de conclusão:** O pilar 1 está completo para uso offline. O desenvolvedor organiza seu consumo de conteúdo inteiramente pelo sistema, com coletâneas, fontes locais, progresso e anotações.

**Transição para Etapa 4:**
- Retrospectiva de uso prolongado: o sistema é confortável de usar diariamente? A curadoria funciona como esperado? Algo precisa ser refatorado antes de adicionar a camada online?
- Documentar ajustes.
- Iniciar estudo sobre as APIs das plataformas externas que serão integradas na Etapa 4 (YouTube, RSS, Instagram, etc.).

---

### Etapa 4 — Integração Externa

**Objetivo:** Construir o contexto de Integração Externa — os adaptadores que conectam o sistema às plataformas de conteúdo. Este contexto é o Anti-Corruption Layer que protege o domínio da instabilidade das APIs externas.

**Decisões técnicas requeridas:**
- **Cliente HTTP / mecanismo de requisições externas:** Necessário agora porque o sistema precisa se comunicar com APIs externas. Justificativa: até aqui o sistema era offline; agora precisa fazer requisições de rede.
- **Parser de RSS:** Necessário agora para o adaptador RSS. Justificativa: RSS é o primeiro protocolo externo a ser implementado.
- **Mecanismo de autenticação com APIs externas (se aplicável):** Algumas APIs (YouTube Data API) exigem chave de API ou OAuth. Necessário agora para esses adaptadores. Justificativa: sem autenticação, o adaptador não funciona.

**Preparação:**
- Estudar as APIs das plataformas prioritárias. Sugestão de prioridade: RSS (padrão aberto, mais estável), YouTube Data API (mais documentada), Instagram (mais restritiva — pode ser adiada).
- Definir o contrato padronizado que os adaptadores devem entregar (formato de item de feed, formato de metadados de conteúdo).

**Escopo:**
- Dentro: adaptador RSS, adaptador YouTube (ao menos canais e playlists), contrato padronizado de item de feed e de metadados, tratamento de indisponibilidade (plataforma offline, API alterada), busca de metadados (título, descrição, thumbnail) para conteúdos individuais.
- Fora: adaptadores para plataformas mais restritivas (Instagram, TikTok) — ficam para incrementos futuros. Montagem de feed (responsabilidade da Agregação, Etapa 5).

**Condução:**
- Implementar o contrato padronizado primeiro (a interface que todos os adaptadores devem cumprir).
- Implementar adaptador RSS (mais simples, valida o contrato).
- Implementar adaptador YouTube (mais complexo, valida escalabilidade do contrato).
- Testar com fontes reais que o desenvolvedor acompanha.

**Validações:**
- Adaptadores entregam dados no contrato padronizado.
- O sistema lida graciosamente com indisponibilidade (timeout, API fora do ar, resposta inesperada).
- Metadados são obtidos corretamente e respeitam a hierarquia de autoridade (metadado buscado é sugestão, manual sobrescreve).

**Critério de conclusão:** Os adaptadores RSS e YouTube estão funcionais e testados. O contrato está estabilizado. Novos adaptadores podem ser adicionados implementando a mesma interface.

**Transição para Etapa 5:**
- O contrato padronizado é o que a Agregação vai consumir. Verificar que é suficiente.
- Documentar limitações conhecidas de cada plataforma.

---

### Etapa 5 — Agregação

**Objetivo:** Construir o contexto de Agregação — feeds de subscrição, agregador consolidado, persistência seletiva. Ao final desta etapa, o pilar 2 (agregação de fontes externas) está funcional. O sistema substitui o feed das redes sociais.

**Decisões técnicas requeridas:**
- Nenhuma nova obrigatória. A Agregação consome o contrato padronizado da Integração e persiste no mecanismo já escolhido. Se o volume de dados de feeds revelar necessidade de cache ou estratégias de otimização, essas decisões emergem aqui — mas não são antecipadas.

**Preparação:**
- Revisar o modelo tático da Agregação à luz do contrato padronizado estabilizado na Etapa 4.
- Implementar o tipo de coletânea Subscrição no contexto Acervo (a estrutura da coletânea — quais fontes, configurações — pertence ao Acervo; o comportamento de montagem de feed pertence à Agregação).
- Definir a regra de transição: quando exatamente um item de feed se torna um conteúdo no Acervo.

**Escopo:**
- Dentro: coletânea tipo Subscrição (estrutura no Acervo, feed na Agregação), montagem de feed sob demanda, agregador como visão consolidada, persistência seletiva, filtros do agregador (por criador, esconder consumidos, palavras-chave), comportamento offline (itens com registro apenas), busca automática de imagens via Integração Externa.
- Fora: busca em plataformas externas (Etapa 6).

**Condução:**
- Implementar subscrição para RSS primeiro (fonte mais simples e estável).
- Implementar agregador consolidando múltiplas subscrições.
- Testar com feeds reais que o desenvolvedor acompanha.
- Validar que a distinção feed (visão efêmera) vs. registro (persistido) funciona na prática.
- Validar cenário 6 do Apêndice A (seguir criadores sem scroll infinito, sem likes, sem algoritmo).

**Validações:**
- Feeds são montados sob demanda e não persistidos.
- Itens com interação do usuário geram registros no Acervo.
- O agregador consolida múltiplos feeds com paginação.
- Filtros funcionam (esconder consumidos, palavras-chave).
- Offline: apenas itens com registro são exibidos, com indicador de incompletude.
- Cenário 6 e cenário 7 do Apêndice A passam.

**Critério de conclusão:** O desenvolvedor usa o agregador diariamente para acompanhar criadores, substituindo o acesso direto às plataformas. Os dois pilares do sistema estão funcionais.

**Transição para Etapa 6:**
- Retrospectiva crítica: com ambos os pilares funcionais, o sistema cumpre seu propósito? O que falta para o uso ser completo?
- Esta é a transição mais importante do plano. O sistema já tem valor real e completo. Tudo a partir daqui é enriquecimento.
- Priorizar as próximas etapas com base no que o uso real revelou como mais necessário.

---

### Etapa 6 em diante — Ordem Flexível

A partir daqui, o sistema já entrega valor completo nos dois pilares. As próximas etapas são priorizadas pelo que o uso real revelar como mais necessário. A ordem abaixo é uma sugestão, não uma sequência fixa.

#### Etapa 6 — Busca e Navegação

**Objetivo:** Tornar o sistema navegável com acervos grandes.

**Decisões técnicas requeridas:**
- **Mecanismo de busca textual:** Necessário agora. Opções: busca nativa do banco de dados escolhido, ou ferramenta dedicada. Justificativa: a busca precisa ser performática com o volume real de dados acumulado. A decisão depende do tamanho do acervo e da complexidade das consultas — dados que só existem agora.

**Escopo:** Busca textual (título, descrição, anotações), todos os filtros da seção 5.4 da definição de domínio, combinação livre de filtros, operações em lote, busca em plataformas externas (delegando à Integração).

**Critério de conclusão:** O desenvolvedor encontra qualquer conteúdo no acervo em poucos segundos.

---

#### Etapa 7 — Reprodução

**Objetivo:** Consumir conteúdo por dentro do sistema quando tecnicamente viável.

**Decisões técnicas requeridas:**
- **Renderizador de Markdown/HTML:** Necessário agora para o leitor de texto. Justificativa: a reprodução de texto é a primeira funcionalidade do reprodutor.
- **Mecanismo de reprodução de áudio:** Necessário agora para player de áudio local. Justificativa: reprodução de arquivos locais é um dos cenários fundadores (CDs copiados).
- **Mecanismo de embed de vídeo:** Necessário agora para embutir vídeos de plataformas. Justificativa: YouTube embed é o cenário mais comum do agregador.

**Escopo:** Leitor de texto (puro, Markdown, HTML), player de áudio local, embed de vídeo, abertura externa, ganchos, marcação automática de progresso, configuração por subtipo.

**Critério de conclusão:** O desenvolvedor consome ao menos texto, áudio local e vídeo embed por dentro do sistema.

---

#### Etapa 8 — Identidade, Preferências e Acessibilidade

**Objetivo:** Multi-usuário, personalização de interface, acessibilidade.

**Decisões técnicas requeridas:**
- **Mecanismo de autenticação local:** Necessário agora. Justificativa: o sistema passa de único usuário implícito para multi-usuário, exigindo autenticação.
- **Armazenamento seguro de credenciais:** Necessário agora. Justificativa: senhas precisam ser armazenadas com hashing adequado.
- **Framework/mecanismo de temas:** Necessário agora para temas configuráveis. Justificativa: a personalização visual exige um mecanismo para aplicar temas dinamicamente. Pode emergir algo simples se a interface já tiver sido construída com variáveis de estilo.

**Escopo:** Autenticação, roles, grupos, gerenciamento de usuários, separação de áreas, ocultação do admin, temas, fontes, acessibilidade, defaults globais, disclosure progressivo configurável. Migração dos dados do usuário único existente.

**Critério de conclusão:** Múltiplos usuários com dados independentes, interface personalizável, área admin funcional e invisível para não-admins.

---

#### Etapa 9 — Portabilidade

**Objetivo:** Exportação e importação de dados entre instâncias.

**Decisões técnicas requeridas:**
- **Formato de serialização:** Necessário agora. Justificativa: definir o formato do pacote de exportação (JSON, YAML, formato próprio, etc.). A escolha depende do modelo de dados real que emergiu das etapas anteriores — informação que não existia antes.

**Escopo:** Exportação de dados do usuário, exportação de configurações globais (admin), importação em outra instância, formato documentado.

**Critério de conclusão:** Um usuário pode migrar seus dados entre instâncias sem perda.

---

#### Etapa 10 — Refinamento e Robustez

**Objetivo:** Polir o sistema com base em toda a experiência de uso acumulada.

**Decisões técnicas requeridas:**
- **Ferramentas de observabilidade (se necessário):** Avaliado agora. Justificativa: apenas neste ponto há dados reais de uso para saber se ferramentas de monitoramento e logging estruturado são necessárias ou se logs simples bastam.
- **Mecanismo do instalador:** Necessário agora se outras pessoas vão usar o sistema. Justificativa: até aqui o desenvolvedor roda o sistema do ambiente de desenvolvimento. Para distribuir, é preciso empacotar.
- **Estratégia de containerização para distribuição (se aplicável):** Avaliado agora. Justificativa: depende de como o instalador funciona e se o Docker é viável no ambiente dos usuários finais.

**Escopo:** Otimização de desempenho, tratamento de erros, intervenções de uso saudável, alertas de configuração, instalador, preparação para extensibilidade futura.

**Critério de conclusão:** O sistema é robusto, performático, instalável e confortável para uso prolongado diário.

---

## 3. Mapa de Decisões Técnicas

| Decisão | Etapa mais tarde possível | Justificativa do momento |
|---|---|---|
| Linguagem de programação | 1 (Walking Skeleton) | Sem linguagem não há código |
| Framework de testes | 1 (Walking Skeleton) | Sem testes não há TDD |
| Repositório e CI mínimo | 1 (Walking Skeleton) | Walking skeleton deve ser automaticamente construível |
| Persistência durável (banco de dados) | 2 (Acervo Básico) | Dados precisam sobreviver a reinicializações para uso diário. Pode ser a solução mais simples que funcione |
| Interface visual | 2 (Acervo Básico) | Uso diário exige conforto visual |
| ORM ou mecanismo de mapeamento | Quando a persistência escolhida exigir | Pode nunca ser necessário dependendo da escolha de persistência |
| Cliente HTTP | 4 (Integração Externa) | Primeira comunicação com APIs externas |
| Parser de RSS | 4 (Integração Externa) | Primeiro adaptador de plataforma |
| Autenticação com APIs externas | 4 (Integração Externa) | APIs que exigem chave/OAuth |
| Mecanismo de busca textual | 6 (Busca e Navegação) | Depende do volume real do acervo |
| Renderizador Markdown/HTML | 7 (Reprodução) | Leitor de texto interno |
| Player de áudio | 7 (Reprodução) | Reprodução de arquivos locais |
| Embed de vídeo | 7 (Reprodução) | Reprodução de vídeos de plataformas |
| Autenticação local | 8 (Identidade) | Multi-usuário |
| Hashing de senhas | 8 (Identidade) | Armazenamento seguro de credenciais |
| Mecanismo de temas | 8 (Preferências) | Personalização visual dinâmica |
| Formato de serialização para export | 9 (Portabilidade) | Formato depende do modelo de dados real |
| Ferramentas de observabilidade | 10 (Refinamento) | Só com dados reais de uso se sabe se são necessárias |
| Mecanismo do instalador | 10 (Refinamento) | Só necessário para distribuição |
| Containerização para distribuição | 10 (Refinamento) | Depende da estratégia do instalador |

---

## 4. Visão Geral das Etapas

| Etapa | Nome | Objetivo principal | Resultado utilizável |
|---|---|---|---|
| 0 | Modelagem Tática DDD | Traduzir design estratégico em tático | Não (design) |
| 1 | Walking Skeleton | Provar a arquitetura de ponta a ponta | Não (prova técnica) |
| 2 | Acervo Básico | Gestão de conteúdo utilizável | Sim — substitui o bloco de notas |
| 3 | Curadoria | Coletâneas, fontes, imagens | Sim — pilar 1 completo (offline) |
| 4 | Integração Externa | Adaptadores de plataformas | Não (infraestrutura para Etapa 5) |
| 5 | Agregação | Feeds, agregador, subscrição | Sim — pilar 2 completo (online) |
| 6+ | Busca, Reprodução, Identidade, Portabilidade, Refinamento | Enriquecimento | Sim — ordem flexível |

---

## 5. O que é Intencional vs. o que Emerge

| Aspecto | Intencional (definido de propósito) | Emergente (surge do código e do uso) |
|---|---|---|
| Arquitetura | Monolito modular, bounded contexts, separação UI/Domínio/Persistência | Padrões internos de cada módulo, granularidade de classes e métodos |
| Domínio | Entidades, agregados, invariantes, interfaces entre contextos | Implementação concreta de repositórios, format de eventos, helpers |
| Persistência | Interface de repositório (contrato) | Escolha do banco, esquema de tabelas, queries específicas |
| Interface | Princípios (paginação, disclosure progressivo, sem scroll infinito) | Layout concreto, componentes específicos, fluxos de navegação |
| Testes | Cobertura de invariantes e cenários do Apêndice A | Granularidade dos testes, mocks específicos, fixtures |
| Integração | Contrato padronizado de adaptadores | Implementação de cada adaptador, tratamento de edge cases por plataforma |

---

## 6. Nota sobre Ordem e Flexibilidade

A ordem das Etapas 0 a 5 é fixa — cada uma depende da anterior. A partir da Etapa 6, a ordem é ajustada com base no que o uso real revelar como mais prioritário. A transição entre cada etapa é o momento de recalcular prioridades. O plano é um guia, não uma obrigação.

O design intencional (bounded contexts, interfaces, princípios) é estável. O que emerge (implementações concretas, padrões internos, decisões técnicas) pode e deve mudar conforme o sistema evolui. Essa é a distinção fundamental: o design intencional resiste à mudança porque foi pensado para isso; a evolução emergente abraça a mudança porque é alimentada pela realidade.
