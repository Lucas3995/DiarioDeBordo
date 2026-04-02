# Mapa de Domínio — Sistema de Gestão e Consumo de Conteúdo com Agência do Usuário

**Versão:** 1.0
**Documento base:** Definição de Domínio v3

---

## 1. Critérios de Classificação

Conforme Evans (2003, *Domain-Driven Design*) e a revisão sistemática de Özkan et al. (2023, *arXiv/peer-reviewed*), os subdomínios são classificados em:

**Principal (Core):** O que torna o sistema único. Sem excelência aqui, o sistema não tem motivo para existir. É onde o melhor esforço deve ser investido. Um sistema pode ter mais de um subdomínio principal desde que a prioridade entre eles esteja clara.

**Suporte (Supporting):** Necessário para o core funcionar, mas não define a identidade do sistema. Pode exigir customização específica para este domínio, mas não é o diferencial.

**Genérico (Generic):** Funcionalidade padrão que não contém nada especial ao domínio. Pode ser resolvida com soluções prontas ou abordagens bem estabelecidas na indústria.

Além dos subdomínios, o sistema possui uma **preocupação transversal** que impõe restrições e princípios sobre o design de todos os subdomínios sem ter fronteiras próprias.

---

## 2. Preocupação Transversal

### Uso Saudável

**Natureza:** Não é um subdomínio. É um conjunto de princípios e restrições que permeiam o design e a arquitetura de usabilidade de todo o sistema.

**O que define:**
- Padrões proibidos por design em qualquer subdomínio (scroll infinito, autoplay, algoritmo de ranqueamento, métricas sociais, notificações não solicitadas).
- Paginação obrigatória em toda apresentação de listas.
- Intervenções ativas opcionais e configuráveis (monitoramento de tempo, lembretes, relatórios, escala de cinza).
- Alertas quando configurações do usuário se aproximam de padrões não saudáveis.

**Como atua:** Todo subdomínio que apresente conteúdo ao usuário, monte listas, construa feeds ou ofereça opções de configuração deve respeitar as restrições de Uso Saudável. Não é uma funcionalidade que se implementa em um módulo isolado — é uma restrição que se verifica em cada decisão de design e implementação dos módulos.

**Por que não é subdomínio:** Se fosse tratado como subdomínio, puxaria para si responsabilidades que pertencem a outros (apresentação de listas pertence a cada subdomínio que as utiliza; configurações de paginação pertencem à Personalização). Uso Saudável é a lente, não a peça.

---

## 3. Subdomínios Principais (Core)

### 3.1. Curadoria e Coletâneas — Prioridade 1

**O que é:** A forma como o usuário estrutura, organiza, sequencia e dá sentido ao seu consumo de conteúdo.

**Responsabilidades:**
- Tipos de coletânea e seus comportamentos distintos: Guiada (ordenação recomendada, consumo sequencial), Miscelânea (sem ordem, acesso livre), Subscrição (alimentada por fontes externas, montagem sob demanda).
- Configurações comportamentais de coletâneas: ordenação habilitada ou não, acompanhamento sequencial ativo ou não.
- Composição de coletâneas: coletânea contendo outras coletâneas para resolver cenários como franquias (coletânea-mãe com coletâneas-filhas por mídia).
- Anotação contextual: anotações que pertencem à relação conteúdo-coletânea, não ao conteúdo isoladamente.
- Proteção contra ciclos: validação de que nenhuma adição cria referência circular.
- Regras de apresentação de itens dentro de coletâneas: paginação, ordenação, filtros internos.

**Por que é core e prioridade 1:** A curadoria é o que transforma uma lista de registros em um sistema de organização pessoal com significado. Planos de estudo, sagas de livros, playlists cross-mídia, acompanhamento de franquias — nada disso existe em um bloco de notas ou em um gerenciador de bookmarks genérico. A composição de coletâneas, os comportamentos configuráveis e as anotações contextuais são decisões específicas deste domínio que não existem em soluções prontas. É a funcionalidade onde a maior complexidade de regras de negócio reside.

**Depende de:** Gestão de Conteúdo (os conteúdos que são organizados), Busca e Navegação (para encontrar conteúdos dentro de coletâneas grandes).

---

### 3.2. Agregação e Subscrição — Prioridade 2

**O que é:** O mecanismo pelo qual o usuário traz conteúdo de criadores e fontes externas para dentro do sistema, substituindo o feed das redes sociais por um ambiente controlado.

**Responsabilidades:**
- Subscrição como coletânea alimentada por fontes externas: a lógica de configurar fontes (canal YouTube, RSS, @ de Instagram, etc.) e o comportamento de montagem sob demanda.
- Feed como visão: itens montados ao visualizar, não persistidos, com persistência seletiva apenas para itens com interação do usuário.
- Agregador como visão consolidada: combinação de múltiplas subscrições em um feed unificado.
- Montagem de feed (descoberta de existência de itens) como operação distinta de busca de metadados (obtenção de informações sobre um conteúdo).
- Filtros do agregador: por criador/fonte, esconder consumidos, esconder por palavras-chave, ordenação cronológica.
- Comportamento offline de subscrições: apresentar apenas itens com registros salvos, sinalizar incompletude.
- Regras de apresentação: cards com metadados quando possível, links quando não, redirecionamento para reprodutor interno ou externo.

**Por que é core e prioridade 2:** É o pilar 2 do sistema — a razão pela qual alguém usaria este sistema em vez de simplesmente abrir o YouTube ou Instagram. A decisão de que o feed é uma visão efêmera sem algoritmo de ranqueamento, de que itens só são persistidos quando o usuário interage, e de que não há métricas sociais nem autoplay — tudo isso é o diferencial. É prioridade 2 (e não 1) porque o sistema ainda tem valor sem o agregador (pilar de gestão funciona sozinho), mas o agregador sem a curadoria seria apenas um leitor RSS.

**Depende de:** Gestão de Conteúdo (para persistir itens com interação), Integração com Plataformas Externas (para conectar às fontes), Curadoria e Coletâneas (a subscrição é um tipo de coletânea).

---

## 4. Subdomínios de Suporte (Supporting)

### 4.1. Gestão de Conteúdo

**O que é:** O modelo central de dados sobre o qual o sistema inteiro opera — a entidade Conteúdo e tudo o que a define.

**Responsabilidades:**
- Entidade Conteúdo com seus dois eixos de classificação: formato de mídia (áudio, texto, vídeo, imagem, interativo, misto, nenhum) e papel estrutural (item, coletânea).
- Subtipos livres de formato.
- Atributos do conteúdo: título, descrição, anotações, nota, classificação.
- Fontes com prioridade e fallback.
- Progresso global: pertence ao conteúdo, reflete em todos os contextos.
- Histórico de ações: log de eventos sobre o conteúdo.
- Categorias como tags livres com autocompletar e não duplicação.
- Conteúdos relacionados com bidirecionalidade e tipos de relação.
- Imagens com hierarquia de origem.
- Hierarquia de autoridade de metadados (manual > automático de fonte > automático de fontes externas).
- Identidade e deduplicação de conteúdo.
- Conteúdo sem fonte como caso válido.

**Por que é suporte e não core:** A Gestão de Conteúdo é o alicerce — sem ela, nada funciona. Porém, registrar informações sobre coisas que se consome é algo que se pode fazer em um bloco de notas, em uma planilha, ou em qualquer gerenciador de notas. O que torna este sistema necessário não é o registro em si, mas como ele é organizado (Curadoria) e de onde vem (Agregação). A Gestão fornece a infraestrutura de dados que os dois cores consomem.

**Consumido por:** Curadoria e Coletâneas, Agregação e Subscrição, Reprodução de Conteúdo, Busca e Navegação, Portabilidade de Dados.

---

### 4.2. Reprodução de Conteúdo

**O que é:** A capacidade de consumir conteúdo por dentro do sistema, com fallback para reprodução externa.

**Responsabilidades:**
- Reprodutor interno: leitor de texto (puro, Markdown, HTML), player de áudio para arquivos locais, embed de vídeo para plataformas que permitam.
- Abertura externa: no aplicativo padrão do OS ou em aplicativo escolhido pelo usuário.
- Fallback entre fontes conforme prioridade configurada.
- Ganchos (bookmarks dentro do conteúdo): marcações de tempo, posições em texto, referências a elementos de coletâneas.
- Marcação automática de progresso quando consumido internamente (oferecida ao usuário, não imposta).
- Configuração de comportamento padrão de reprodução por subtipo de formato.

**Por que é suporte:** O sistema tem valor completo sem o reprodutor — o cenário do caderno físico e o primeiro uso offline demonstram isso. Porém, o reprodutor exige customização específica do domínio (fallback entre fontes, ganchos, configuração por subtipo) e não pode ser resolvido com uma solução genérica pronta.

**Depende de:** Gestão de Conteúdo (o conteúdo a ser reproduzido e suas fontes).

---

### 4.3. Busca e Navegação

**O que é:** Os mecanismos pelos quais o usuário encontra, filtra e opera sobre conteúdos no sistema.

**Responsabilidades:**
- Busca textual interna: sobre título, descrição e anotações.
- Filtros combinados: por formato, papel, tipo de coletânea, categoria, nota, classificação, progresso, data de adição, fonte/criador.
- Busca em plataformas externas (quando viabilizado por elas).
- Operações em lote: adicionar categoria, mover para coletânea, marcar como concluído, remover — sobre múltiplos conteúdos simultaneamente.

**Por que é suporte:** Essencial para a usabilidade com acervos grandes (princípio de economia cognitiva), mas os mecanismos de busca e filtragem em si são padrões conhecidos. A customização para este domínio (filtrar por progresso, por classificação pessoal, busca em anotações) é específica mas a mecânica subjacente é bem compreendida.

**Consumido por:** Curadoria e Coletâneas (encontrar conteúdos para adicionar), Agregação (filtros do agregador), Dashboard (montagem de seções).

---

### 4.4. Integração com Plataformas Externas

**O que é:** A camada técnica que conecta o sistema às plataformas de origem do conteúdo.

**Responsabilidades:**
- Adaptadores por plataforma: YouTube (canais, playlists), Instagram (@), RSS, Amazon (listas de desejos), e outros.
- Montagem de feed: consulta às fontes para descobrir quais conteúdos existem.
- Busca de metadados: obtenção de título, descrição, thumbnail e outros dados de fontes.
- Busca de conteúdo em plataformas externas por dentro do sistema.
- Importação de dados de plataformas existentes (futuro): parsers que transformam dados externos em registros do modelo de domínio.
- Tratamento de indisponibilidade: plataformas que bloqueiam acesso, mudam APIs, ou estão offline.

**Por que é suporte e não core:** A Agregação define o que o sistema faz com as fontes externas (regras de negócio); a Integração é como ele se conecta tecnicamente a cada plataforma (adaptadores). A lógica de negócio do agregador não muda quando o Instagram muda sua API — o adaptador muda. Porém, os adaptadores são específicos para este sistema (não são uma solução genérica de integração) e exigem manutenção contínua.

**Consumido por:** Agregação e Subscrição (montagem de feeds, busca de metadados), Gestão de Conteúdo (enriquecimento de metadados).

---

### 4.5. Portabilidade de Dados

**O que é:** A capacidade de exportar e importar dados entre instâncias do sistema.

**Responsabilidades:**
- Exportação de dados do usuário: conteúdos, coletâneas, progresso, anotações, configurações pessoais.
- Exportação de configurações globais (admin).
- Importação em outra instância, possivelmente em outro sistema operacional.
- Formato agnóstico de plataforma, legível e documentado.

**Por que é suporte:** Necessário para a soberania do usuário sobre seus dados e para o cenário de migração entre computadores, mas os mecanismos de serialização e deserialização não são o diferencial do sistema. Exige alguma customização (o formato reflete o modelo de domínio) mas a mecânica é padrão.

**Depende de:** Gestão de Conteúdo (os dados a exportar), Curadoria e Coletâneas (estrutura das coletâneas), Personalização de Interface (configurações do usuário).

---

## 5. Subdomínios Genéricos (Generic)

### 5.1. Identidade e Acesso

**O que é:** Autenticação, autorização e gerenciamento de usuários.

**Responsabilidades:**
- Autenticação de usuários.
- Roles fixas: consumidor e admin.
- Grupos de roles criados pelo admin.
- Gerenciamento de usuários: criar, ativar, inativar, resetar senhas, atribuir grupos.
- Separação de áreas: consumidor vs. admin.
- Ocultação da área admin para usuários sem a role admin (comportamento genérico que não revela a existência da área).

**Por que é genérico:** Autenticação e controle de acesso baseado em roles é um problema resolvido com soluções e padrões bem estabelecidos. A ocultação da área admin é uma decisão de segurança específica mas implementável com técnicas conhecidas.

---

### 5.2. Personalização de Interface

**O que é:** A capacidade do usuário customizar a aparência e o comportamento da interface.

**Responsabilidades:**
- Configurações de fonte: família, tamanho, cor.
- Temas: claro, escuro (padrão), customizáveis pelo usuário, globalmente resetáveis.
- Configurações de acessibilidade: contraste elevado, redução de estímulos visuais, adaptações para necessidades específicas.
- Mecanismo de defaults globais: admin define valores padrão, usuário pode sobrescrever.
- Disclosure progressivo configurável: o usuário pode desativar e ver todos os campos/opções de uma vez.

**Por que é genérico:** A capacidade de personalizar a interface é importante para a agência do usuário, mas os mecanismos (temas, preferências de fonte, contraste, defaults configuráveis) são padrões bem conhecidos de desenvolvimento de interfaces. Não há lógica de negócio específica do domínio aqui.

---

## 6. Mapa Visual de Dependências

```
┌─────────────────────────────────────────────────────────────────────┐
│                    USO SAUDÁVEL (transversal)                       │
│         Impõe restrições sobre o design de todos os subdomínios     │
└─────────────────────────────────────────────────────────────────────┘

┌─── PRINCIPAL ────────────────────────────────────────────────────────┐
│                                                                      │
│  ┌─────────────────────────┐     ┌──────────────────────────────┐   │
│  │  CURADORIA E COLETÂNEAS │     │  AGREGAÇÃO E SUBSCRIÇÃO      │   │
│  │  (Prioridade 1)         │◄───►│  (Prioridade 2)              │   │
│  │                         │     │                              │   │
│  │  Tipos de coletânea     │     │  Subscrição como coletânea   │   │
│  │  Composição             │     │  Feed como visão             │   │
│  │  Comportamentos config. │     │  Agregador consolidado       │   │
│  │  Anotação contextual    │     │  Filtros do agregador        │   │
│  │  Proteção contra ciclos │     │  Persistência seletiva       │   │
│  └────────────┬────────────┘     └──────────────┬───────────────┘   │
│               │                                  │                   │
└───────────────┼──────────────────────────────────┼───────────────────┘
                │                                  │
┌─── SUPORTE ───┼──────────────────────────────────┼───────────────────┐
│               │                                  │                   │
│  ┌────────────▼────────────┐     ┌──────────────▼───────────────┐   │
│  │  GESTÃO DE CONTEÚDO     │     │  INTEGRAÇÃO COM PLATAFORMAS  │   │
│  │                         │     │  EXTERNAS                    │   │
│  │  Entidade Conteúdo      │     │  Adaptadores por plataforma  │   │
│  │  Dois eixos             │     │  Montagem de feed            │   │
│  │  Progresso global       │     │  Busca de metadados          │   │
│  │  Fontes e fallback      │     │  Importação de dados         │   │
│  │  Deduplicação           │     │                              │   │
│  │  Categorias e relações  │     │                              │   │
│  └──────┬──────────────────┘     └──────────────────────────────┘   │
│         │                                                            │
│  ┌──────▼──────────────────┐     ┌──────────────────────────────┐   │
│  │  REPRODUÇÃO DE CONTEÚDO │     │  BUSCA E NAVEGAÇÃO           │   │
│  │                         │     │                              │   │
│  │  Reprodutor interno     │     │  Busca textual               │   │
│  │  Abertura externa       │     │  Filtros combinados          │   │
│  │  Ganchos                │     │  Operações em lote           │   │
│  │  Fallback de fontes     │     │                              │   │
│  └─────────────────────────┘     └──────────────────────────────┘   │
│                                                                      │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │  PORTABILIDADE DE DADOS                                      │   │
│  │                                                              │   │
│  │  Exportação/importação de dados do usuário e config. globais │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘

┌─── GENÉRICO ─────────────────────────────────────────────────────────┐
│                                                                      │
│  ┌─────────────────────────┐     ┌──────────────────────────────┐   │
│  │  IDENTIDADE E ACESSO    │     │  PERSONALIZAÇÃO DE INTERFACE │   │
│  │                         │     │                              │   │
│  │  Autenticação           │     │  Temas e fontes              │   │
│  │  Roles e grupos         │     │  Acessibilidade              │   │
│  │  Gerenciamento de       │     │  Defaults globais            │   │
│  │  usuários               │     │  Disclosure progressivo      │   │
│  └─────────────────────────┘     └──────────────────────────────┘   │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

---

## 7. Relações entre Subdomínios

| De | Para | Natureza da relação |
|---|---|---|
| Curadoria e Coletâneas | Gestão de Conteúdo | Depende: conteúdos são os itens organizados pelas coletâneas |
| Curadoria e Coletâneas | Busca e Navegação | Depende: busca é usada para encontrar conteúdos dentro de coletâneas grandes |
| Agregação e Subscrição | Gestão de Conteúdo | Depende: itens com interação do usuário são persistidos como conteúdos |
| Agregação e Subscrição | Integração com Plataformas | Depende: adaptadores conectam às fontes externas |
| Agregação e Subscrição | Curadoria e Coletâneas | Depende: subscrição é um tipo de coletânea |
| Reprodução de Conteúdo | Gestão de Conteúdo | Depende: consome o conteúdo e suas fontes |
| Busca e Navegação | Gestão de Conteúdo | Depende: busca opera sobre os registros de conteúdo |
| Portabilidade de Dados | Gestão de Conteúdo | Depende: exporta/importa os registros de conteúdo |
| Portabilidade de Dados | Curadoria e Coletâneas | Depende: exporta/importa a estrutura de coletâneas |
| Portabilidade de Dados | Personalização de Interface | Depende: exporta/importa configurações do usuário |
| Todos | Identidade e Acesso | Depende: autenticação e autorização são pré-requisito |
| Todos | Personalização de Interface | Consome: preferências de apresentação afetam a renderização |

---

## 8. Notas de Decisão

### 8.1. Gestão de Conteúdo como suporte — não como core

A entidade Conteúdo é o alicerce de todo o sistema, mas o registro de informações sobre coisas consumidas é algo que qualquer ferramenta (planilha, bloco de notas, app de notas) pode fazer. O que justifica a existência deste sistema — e portanto o que é core — é como essas informações são organizadas (Curadoria) e de onde elas vêm (Agregação). Gestão de Conteúdo é a fundação necessária, não o diferencial.

### 8.2. Uso Saudável como preocupação transversal — não como subdomínio

Uso Saudável não é uma funcionalidade isolável com fronteiras próprias. É uma lente que se aplica a toda decisão de design em qualquer subdomínio. Se fosse tratado como subdomínio, puxaria para si responsabilidades que pertencem a outros (apresentação de listas, configurações de paginação, regras de interface). Ele é materializado como restrições de design impostas sobre todos os subdomínios, não como um módulo com entidades e regras de negócio próprias.

### 8.3. Subscrição pertence simultaneamente a dois subdomínios

A Subscrição é um tipo de coletânea (portanto pertence à Curadoria) e é o mecanismo de alimentação por fontes externas (portanto pertence à Agregação). Essa dualidade é intencional e reflete que os dois cores se complementam. Na implementação, será necessário definir uma fronteira clara — provavelmente a Curadoria define a estrutura da coletânea e suas regras (ordenação, composição, proteção contra ciclos), enquanto a Agregação define o comportamento de montagem de feed e persistência seletiva.

### 8.4. Integração com Plataformas como suporte separado da Agregação

A Agregação define as regras de negócio (o que fazer com os feeds); a Integração implementa a conexão técnica (como se conectar ao YouTube, Instagram, RSS). Essa separação permite que a Agregação permaneça estável quando APIs externas mudam — apenas os adaptadores da Integração precisam ser atualizados. Também permite adicionar novas plataformas sem alterar a lógica de negócio do agregador.
