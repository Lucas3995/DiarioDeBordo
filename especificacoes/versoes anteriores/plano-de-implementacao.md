# Plano de Implementação — Sistema de Gestão e Consumo de Conteúdo com Agência do Usuário

**Versão:** 1.0
**Documentos base:** Definição de Domínio v3, Mapa de Domínio v1, Mapa de Contexto v1

---

## 1. Abordagem Geral

### 1.1. Estratégia

O desenvolvimento segue uma abordagem **incremental-iterativa com walking skeleton**: a primeira entrega é a implementação mais fina possível de funcionalidade real, atravessando todas as camadas da arquitetura de ponta a ponta. Cada incremento subsequente adiciona uma fatia vertical de funcionalidade utilizável. Entre incrementos, iterações de aprendizado permitem estudo, refatoração e validação do modelo contra a realidade de uso.

Fundamentação: Cockburn (2004, *Crystal Clear: A Human-Powered Methodology for Small Teams*) define walking skeleton como "a implementação mais fina possível de funcionalidade real que pode ser automaticamente construída, implantada e testada de ponta a ponta." O Modelo Espiral de Boehm (1987, *ACM SIGSOFT Software Engineering Notes*) formaliza que nem toda iteração produz um incremento — algumas servem primariamente como oportunidades de aprendizado, e essas são cruciais para reduzir incerteza. O padrão ISO/IEC/IEEE 24748-1 (2024) reconhece o desenvolvimento incremental como especialmente útil em projetos com alta incerteza ou requisitos em evolução.

### 1.2. Princípios do plano

**Excelência sobre velocidade.** Cada etapa pode incluir períodos de estudo dedicados quando o desenvolvedor identificar áreas onde não possui conhecimento suficiente. Esses períodos são parte do plano, não desvios dele.

**Decisões técnicas adiadas.** Conforme Martin (2017, *Clean Architecture*): decisões sobre detalhes (banco de dados, ORM, framework de UI, ferramentas de observabilidade, protocolos de comunicação) são tomadas o mais tarde que a realidade permitir. O plano indica quando cada decisão precisa ser tomada, mas não as toma agora.

**Uso próprio como validação.** O sistema será usado pelo desenvolvedor o mais cedo possível. O feedback do uso real é o principal mecanismo de validação de qualidade do modelo de domínio e das decisões de design.

**Refatoração entre etapas.** Toda transição entre etapas inclui um momento de retrospectiva e refatoração. Código que funcionou na etapa anterior mas que a experiência de uso revelou como inadequado é corrigido antes de adicionar complexidade.

### 1.3. Estrutura de cada etapa

Toda etapa segue a mesma estrutura:

- **Objetivo:** O que essa etapa entrega e por que.
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
- O modelo tático é o insumo para as decisões técnicas da próxima etapa.

---

### Etapa 1 — Decisões Técnicas e Infraestrutura

**Objetivo:** Tomar as decisões técnicas que foram adiadas até aqui (linguagem, framework de UI, banco de dados, estrutura do projeto, ferramentas de teste, CI/CD) e montar a infraestrutura de desenvolvimento (repositório, pipeline, containerização, estrutura de diretórios).

**Preparação:**
- O modelo tático da Etapa 0 está completo. Ele informa as decisões: a complexidade do domínio indica se é necessário um ORM robusto ou se algo mais simples basta; a necessidade de UI nativa indica quais frameworks são viáveis; o monolito modular indica como estruturar o projeto.
- Período de estudo dedicado se houver tecnologias candidatas que o desenvolvedor não conhece suficientemente para decidir com confiança.
- Levantar requisitos técnicos concretos derivados do domínio: reprodução de áudio/vídeo, embed de conteúdo web, leitura de feeds RSS, renderização de Markdown/HTML, acesso ao sistema de arquivos local.

**Escopo:**
- Dentro: escolha de linguagem, framework de UI desktop, banco de dados, ferramentas de teste (unitário, integração), setup do repositório, pipeline de CI/CD no GitHub Actions, containerização para desenvolvimento, estrutura de diretórios do projeto alinhada com os bounded contexts, setup do instalador (ao menos o esqueleto).
- Fora: implementação de funcionalidades do domínio (isso é a Etapa 2).

**Condução:**
- Para cada decisão técnica, documentar as opções avaliadas, os critérios de decisão (derivados do domínio e dos requisitos não-funcionais), e a escolha feita com justificativa. Decisões não são tomadas por popularidade da ferramenta, mas por adequação ao domínio.
- Montar o projeto vazio com a estrutura de diretórios refletindo os bounded contexts.
- Configurar o pipeline de CI: build, testes, linting.
- Verificar que o ambiente containerizado roda em Linux (ambiente de desenvolvimento) e que o instalador esqueleto funciona.

**Validações:**
- O projeto vazio compila/executa e o pipeline de CI roda sem erros.
- A estrutura de diretórios reflete os bounded contexts do mapa de contexto.
- Cada decisão técnica está documentada com justificativa.

**Critério de conclusão:** O ambiente de desenvolvimento está funcional, o pipeline roda, e o projeto vazio pode ser executado. As decisões técnicas estão documentadas.

**Transição para Etapa 2:**
- Verificar se as decisões técnicas impõem alguma restrição que exija revisão do modelo tático. Se sim, ajustar antes de avançar.

---

### Etapa 2 — Walking Skeleton

**Objetivo:** Implementar a fatia mais fina possível de funcionalidade real, atravessando todas as camadas da arquitetura: UI → Domínio → Persistência. O objetivo não é entregar valor ao usuário, mas provar que a arquitetura funciona de ponta a ponta e que o domínio pode ser implementado conforme modelado.

**Preparação:**
- Escolher a funcionalidade mais simples que percorre todas as camadas: criar um conteúdo com título, salvar, recuperar, exibir.
- Garantir que a separação UI/Domínio/Persistência está clara na estrutura do projeto.

**Escopo:**
- Dentro: criar um conteúdo com título (único campo obrigatório), persistir, recuperar, exibir em tela. Testes automatizados cobrindo a funcionalidade. Pipeline de CI rodando os testes.
- Fora: qualquer outro atributo de conteúdo, coletâneas, busca, reprodução, agregação.

**Condução:**
- Implementar de dentro para fora: domínio primeiro (entidade Conteúdo com título), depois persistência (salvar e recuperar), depois UI (tela mínima para criar e listar).
- Escrever testes desde o primeiro momento. O walking skeleton é o alicerce — qualquer defeito aqui se propaga para tudo.
- Validar que a separação de camadas está funcionando: a UI não conhece o banco de dados, o domínio não conhece a UI.

**Validações:**
- A funcionalidade roda de ponta a ponta: criar conteúdo na UI, verificar que foi persistido, verificar que aparece na listagem.
- Testes automatizados passam no pipeline de CI.
- A separação de camadas é real: é possível trocar a UI sem alterar o domínio, e trocar o banco sem alterar o domínio.

**Critério de conclusão:** Um conteúdo pode ser criado, salvo e recuperado de ponta a ponta, com testes automatizados, rodando no pipeline de CI. A arquitetura está provada.

**Transição para Etapa 3:**
- Retrospectiva: a implementação revelou algum problema na arquitetura ou no modelo tático? Se sim, corrigir agora — o custo de correção é mínimo neste ponto e cresce exponencialmente depois.
- Documentar lições aprendidas sobre a stack escolhida.

---

### Etapa 3 — Acervo Básico

**Objetivo:** Construir o contexto Acervo com funcionalidade suficiente para que o desenvolvedor comece a usar o sistema no dia-a-dia, substituindo o bloco de notas. Ao final desta etapa, o sistema tem valor real de uso.

**Preparação:**
- Priorizar quais atributos do conteúdo implementar primeiro. Sugestão: título, descrição, formato de mídia, subtipo, anotações, nota, classificação, progresso (estado + campo manual). Imagens e fontes podem ficar para a Etapa 4.
- Definir a interface do módulo de categorias e relações entre conteúdos.

**Escopo:**
- Dentro: atributos do conteúdo (priorizados), categorias com autocompletar e não-duplicação, relações entre conteúdos com bidirecionalidade e tipos de relação, progresso global (estado, campo manual, histórico de consumo), histórico de ações, paginação em todas as listagens.
- Fora: coletâneas (Etapa 4), fontes e fallback (Etapa 4), imagens (Etapa 4), reprodução, agregação, busca avançada.

**Condução:**
- Implementar incrementalmente: adicionar um atributo ou funcionalidade por vez, com teste, validar que o existente continua funcionando.
- Usar o sistema para cadastrar conteúdos reais (livros lidos, vídeos assistidos, jogos jogados). O uso próprio é a validação primária.
- A paginação deve estar presente desde a primeira listagem (princípio de uso saudável como preocupação transversal).

**Validações:**
- Cada funcionalidade implementada tem testes automatizados.
- O desenvolvedor consegue cadastrar e gerenciar conteúdos reais confortavelmente.
- Categorias não duplicam. Relações são bidirecionais. Progresso é global. Paginação funciona.
- O princípio de economia cognitiva está aplicado: formulário com disclosure progressivo, apenas título obrigatório.

**Critério de conclusão:** O desenvolvedor está usando o sistema diariamente para registrar conteúdos. O contexto Acervo (parte de gestão de conteúdo) está funcional e testado.

**Transição para Etapa 4:**
- Retrospectiva de uso: o que incomoda? O que falta para o uso ser confortável? O que o modelo de domínio assumiu que não se confirma na prática?
- Refatorar o que for necessário antes de adicionar coletâneas.
- Documentar ajustes no modelo de domínio se houver.

---

### Etapa 4 — Curadoria: Coletâneas e Fontes

**Objetivo:** Completar o contexto Acervo com coletâneas (todos os tipos e comportamentos), fontes com prioridade e fallback, e imagens. Ao final desta etapa, o pilar 1 (gestão de conteúdo pessoal) está completo.

**Preparação:**
- Revisar o modelo tático de coletâneas à luz da experiência de uso da Etapa 3. Ajustar se necessário.
- Priorizar tipos de coletânea: Guiada e Miscelânea primeiro (não dependem de internet), Subscrição depois (depende da Etapa 6).
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

**Transição para Etapa 5:**
- Retrospectiva de uso prolongado: o sistema é confortável de usar diariamente? A curadoria funciona como esperado? Algo precisa ser refatorado antes de adicionar a camada online?
- Documentar ajustes.
- Preparar estudo sobre as APIs das plataformas externas que serão integradas na Etapa 6 (YouTube, RSS, Instagram, etc.).

---

### Etapa 5 — Integração Externa

**Objetivo:** Construir o contexto de Integração Externa — os adaptadores que conectam o sistema às plataformas de conteúdo. Este contexto é o Anti-Corruption Layer que protege o domínio da instabilidade das APIs externas.

**Preparação:**
- Estudar as APIs das plataformas prioritárias. Sugestão de prioridade: RSS (padrão aberto, mais estável), YouTube Data API (mais documentada), Instagram (mais restritiva).
- Definir o contrato padronizado que os adaptadores devem entregar (formato de item de feed, formato de metadados de conteúdo).
- Tomar decisões técnicas adiadas relacionadas: como fazer requisições HTTP, como parsear RSS, como lidar com autenticação de APIs, como gerenciar rate limits.

**Escopo:**
- Dentro: adaptador RSS, adaptador YouTube (ao menos canais e playlists), contrato padronizado de item de feed e de metadados, tratamento de indisponibilidade (plataforma offline, API alterada), busca de metadados (título, descrição, thumbnail) para conteúdos individuais.
- Fora: adaptadores para plataformas mais restritivas (Instagram, TikTok) — ficam para incrementos futuros. Montagem de feed (responsabilidade da Agregação, Etapa 6).

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

**Transição para Etapa 6:**
- O contrato padronizado é o que a Agregação vai consumir. Verificar que é suficiente.
- Documentar limitações conhecidas de cada plataforma.

---

### Etapa 6 — Agregação

**Objetivo:** Construir o contexto de Agregação — feeds de subscrição, agregador consolidado, persistência seletiva. Ao final desta etapa, o pilar 2 (agregação de fontes externas) está funcional. O sistema substitui o feed das redes sociais.

**Preparação:**
- Revisar o modelo tático da Agregação à luz do contrato padronizado estabilizado na Etapa 5.
- Implementar o tipo de coletânea Subscrição no contexto Acervo (a estrutura da coletânea — quais fontes, configurações — pertence ao Acervo; o comportamento de montagem de feed pertence à Agregação).
- Definir a regra de transição: quando exatamente um item de feed se torna um conteúdo no Acervo.

**Escopo:**
- Dentro: coletânea tipo Subscrição (estrutura no Acervo, feed na Agregação), montagem de feed sob demanda, agregador como visão consolidada, persistência seletiva, filtros do agregador (por criador, esconder consumidos, palavras-chave), comportamento offline (itens com registro apenas), busca automática de imagens via Integração Externa.
- Fora: busca em plataformas externas (Etapa 7).

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

**Transição para Etapa 7:**
- Retrospectiva crítica: com ambos os pilares funcionais, o sistema cumpre seu propósito? O que falta para o uso ser completo?
- Esta é a transição mais importante do plano. O sistema já tem valor real e completo. Tudo a partir daqui é enriquecimento.
- Priorizar as próximas etapas com base no que o uso real revelou como mais necessário.

---

### Etapa 7 — Busca e Navegação

**Objetivo:** Construir o contexto de Busca — busca textual, filtros combinados, operações em lote. Tornar o sistema navegável com acervos grandes.

**Preparação:**
- Avaliar o tamanho real do acervo acumulado nas etapas anteriores. Isso informa decisões sobre indexação.
- Tomar decisões técnicas adiadas: mecanismo de busca textual (built-in do banco ou ferramenta dedicada), estratégia de indexação.

**Escopo:**
- Dentro: busca textual (título, descrição, anotações), todos os filtros listados na seção 5.4 da definição de domínio, combinação livre de filtros, operações em lote, busca em plataformas externas (delegando à Integração).
- Fora: busca semântica ou por similaridade (possibilidade futura, não no escopo atual).

**Condução:**
- Começar pela busca textual simples e filtros individuais.
- Adicionar combinação de filtros.
- Adicionar operações em lote.
- Testar com o acervo real acumulado.

**Validações:**
- Busca textual encontra conteúdos por termos em título, descrição e anotações.
- Filtros combinados funcionam corretamente (ex: formato "vídeo" + categoria "terror" + progresso "não iniciado").
- Operações em lote aplicam ações a múltiplos conteúdos sem erros.

**Critério de conclusão:** O desenvolvedor encontra qualquer conteúdo no acervo em poucos segundos, independentemente do tamanho.

---

### Etapa 8 — Reprodução

**Objetivo:** Construir o contexto de Reprodução — consumir conteúdo por dentro do sistema quando tecnicamente viável.

**Preparação:**
- Avaliar, com base no uso real, quais formatos de reprodução são mais prioritários (texto, vídeo embed, áudio local).
- Tomar decisões técnicas adiadas: como renderizar Markdown/HTML, como embutir vídeos, como reproduzir áudio.

**Escopo:**
- Dentro: leitor de texto (puro, Markdown, HTML), player de áudio para arquivos locais, embed de vídeo (YouTube), abertura externa (app padrão e app escolhido), ganchos, marcação automática de progresso, configuração por subtipo de formato.
- Fora: reprodução de formatos complexos ou plataformas muito restritivas.

**Condução:**
- Implementar leitor de texto primeiro (mais controlável).
- Adicionar player de áudio.
- Adicionar embed de vídeo.
- Implementar ganchos e marcação automática de progresso.
- Testar com conteúdos reais do acervo.

**Validações:**
- Cada formato suportado reproduz corretamente.
- Fallback entre fontes funciona (fonte primária indisponível → próxima na lista).
- Ganchos podem ser criados, editados e navegados.
- Progresso é marcado automaticamente quando aceito pelo usuário.
- Abertura externa funciona (app padrão e app escolhido).

**Critério de conclusão:** O desenvolvedor consome ao menos texto, áudio local e vídeo embed por dentro do sistema.

---

### Etapa 9 — Identidade, Preferências e Acessibilidade

**Objetivo:** Construir os contextos genéricos — autenticação, roles, multi-usuário, personalização de interface, acessibilidade.

**Preparação:**
- Até aqui o sistema roda com um único usuário implícito. Esta etapa adiciona multi-usuário.
- Tomar decisões técnicas: mecanismo de autenticação, armazenamento de credenciais, hashing de senhas.
- Definir o conjunto mínimo de configurações de personalização para a primeira versão.

**Escopo:**
- Dentro: autenticação, roles (consumidor e admin), grupos, gerenciamento de usuários, separação de áreas (consumidor vs. admin), ocultação da área admin, configurações de fonte/cor/tema, temas claro e escuro, configurações de acessibilidade, defaults globais configuráveis pelo admin, disclosure progressivo configurável.
- Fora: nada significativo — esta etapa completa os contextos genéricos.

**Condução:**
- Implementar autenticação e multi-usuário primeiro (impacta toda a estrutura de dados — cada usuário tem dados independentes).
- Implementar área admin separada.
- Implementar personalização de interface.
- Migrar os dados do usuário único existente para o novo modelo multi-usuário.

**Validações:**
- Múltiplos usuários podem usar o sistema com dados independentes.
- Área admin é invisível para não-admins (testar ativamente tentativas de acesso).
- Personalização funciona (temas, fontes, contraste).
- Defaults globais definidos pelo admin se aplicam a novos usuários.
- Acessibilidade: navegação por teclado, compatibilidade com leitor de tela (testar com ferramentas reais).

**Critério de conclusão:** O sistema é multi-usuário com área admin funcional e interface personalizável.

---

### Etapa 10 — Portabilidade

**Objetivo:** Construir o contexto de Portabilidade — exportação e importação de dados.

**Preparação:**
- Definir o formato de exportação (agnóstico de plataforma, legível, documentado).
- Identificar todos os dados que compõem o pacote de exportação de um usuário.

**Escopo:**
- Dentro: exportação de dados do usuário, exportação de configurações globais (admin), importação em outra instância, formato documentado.
- Fora: importação de dados de plataformas externas (planejada para futuro).

**Condução:**
- Implementar exportação primeiro, depois importação.
- Testar o ciclo completo: exportar de uma instância, importar em outra, verificar que tudo está intacto.
- Testar cross-OS se possível (exportar em Linux, importar em Windows ou vice-versa).

**Validações:**
- O pacote exportado contém todos os dados do usuário sem perda.
- A importação reconstrói o acervo completo na nova instância.
- O formato é legível (abrível com editor de texto) e documentado.

**Critério de conclusão:** Um usuário pode migrar seus dados entre instâncias do sistema sem perda.

---

### Etapa 11 — Refinamento e Robustez

**Objetivo:** Polir o sistema com base em toda a experiência de uso acumulada. Endereçar requisitos não-funcionais que foram implementados incrementalmente mas que agora precisam de atenção dedicada.

**Preparação:**
- Compilar todas as anotações de retrospectiva das etapas anteriores.
- Identificar os pontos de dor restantes no uso diário.
- Medir desempenho real com o acervo acumulado.

**Escopo:**
- Dentro: otimização de desempenho, robustez de tratamento de erros e recuperação de estados, intervenções de uso saudável (monitoramento de tempo, lembretes, relatórios de consumo, escala de cinza), alertas de configuração potencialmente não saudável, preparação para extensibilidade futura (importação de plataformas externas, novos adaptadores, compartilhamento entre usuários).
- Fora: funcionalidades novas — esta etapa melhora o que existe.

**Condução:**
- Priorizar pelas dores reais do uso diário.
- Implementar intervenções de uso saudável.
- Stress-test com volume de dados real.
- Revisar tratamento de erros em cada módulo.

**Validações:**
- O sistema não degrada desempenho com o acervo real do desenvolvedor.
- Erros são recuperáveis e as mensagens são informativas.
- Intervenções de uso saudável funcionam e são desativáveis.
- Alertas disparam quando configurações se aproximam de padrões não saudáveis.

**Critério de conclusão:** O sistema é robusto, performático e confortável para uso prolongado diário.

---

## 3. Visão Geral das Etapas

| Etapa | Nome | Objetivo principal | Resultado utilizável |
|---|---|---|---|
| 0 | Modelagem Tática DDD | Traduzir design estratégico em tático | Não (design) |
| 1 | Decisões Técnicas | Escolher stack e montar infraestrutura | Não (infraestrutura) |
| 2 | Walking Skeleton | Provar a arquitetura de ponta a ponta | Não (prova técnica) |
| 3 | Acervo Básico | Gestão de conteúdo utilizável | Sim — substitui o bloco de notas |
| 4 | Curadoria | Coletâneas, fontes, imagens | Sim — pilar 1 completo (offline) |
| 5 | Integração Externa | Adaptadores de plataformas | Não (infraestrutura para Etapa 6) |
| 6 | Agregação | Feeds, agregador, subscrição | Sim — pilar 2 completo (online) |
| 7 | Busca e Navegação | Encontrar e operar sobre conteúdos | Sim — navegação em acervos grandes |
| 8 | Reprodução | Consumir conteúdo dentro do sistema | Sim — ciclo completo sem sair do sistema |
| 9 | Identidade e Preferências | Multi-usuário, personalização | Sim — outras pessoas podem usar |
| 10 | Portabilidade | Exportação e importação | Sim — dados são portáveis |
| 11 | Refinamento | Polimento e robustez | Sim — sistema maduro |

---

## 4. Nota sobre Ordem e Flexibilidade

A ordem das Etapas 0 a 6 é fixa — cada uma depende da anterior. A partir da Etapa 7, a ordem pode ser ajustada com base no que o uso real revelar como mais prioritário. Se após a Etapa 6 o desenvolvedor sentir que a busca é mais urgente que a reprodução, inverte. Se multi-usuário for necessário antes da busca, antecipa.

A transição entre cada etapa é o momento de recalcular prioridades. O plano é um guia, não uma obrigação.
