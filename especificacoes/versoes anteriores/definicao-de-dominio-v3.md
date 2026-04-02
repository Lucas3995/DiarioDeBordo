# Definição de Domínio — Sistema de Gestão e Consumo de Conteúdo com Agência do Usuário

**Versão:** 3.0

---

## 1. Propósito do Sistema

Este sistema existe para devolver ao usuário a agência sobre seu consumo de conteúdo. As plataformas digitais predominantes otimizam para o engajamento da plataforma, não para o bem-estar de quem as utiliza — empregando dark patterns, algoritmos de ranqueamento, scroll infinito, autoplay e métricas sociais que exploram vieses cognitivos e induzem comportamentos compulsivos. Este sistema inverte essa lógica: o usuário decide o quê, como e quanto consome, em um ambiente projetado para não sabotar — e, opcionalmente, proteger ativamente — seu bem-estar.

O sistema é sustentado por **dois pilares funcionais:**

**Pilar 1 — Gestão de conteúdo pessoal:** Dar ao usuário um lugar centralizado para registrar, anotar, avaliar, acompanhar progresso e relacionar informações sobre qualquer coisa que ele consuma, estude ou produza — seja digital ou física, online ou offline. Este pilar funciona integralmente sem internet.

**Pilar 2 — Agregação de fontes externas:** Substituir o feed das redes sociais por um ambiente controlado pelo usuário, trazendo conteúdo de criadores e fontes selecionadas para dentro do sistema, sem algoritmo de ranqueamento, sem scroll infinito, sem métricas sociais. Este pilar depende de internet para montar feeds, mas os conteúdos com os quais o usuário já interagiu ficam disponíveis offline.

Os dois pilares se complementam: o agregador traz conteúdo para dentro do sistema; a gestão permite que o usuário organize, anote e acompanhe esse conteúdo. Mas cada pilar tem valor independente — um usuário que nunca usa o agregador ainda tem um sistema completo de gestão pessoal, e vice-versa.

**O sistema não é uma rede social. Não é um serviço web. É um programa de computador nativo**, análogo ao Calibre ou à calculadora do sistema operacional, que roda localmente na máquina do usuário sem depender de internet para suas funcionalidades centrais.

---

## 2. Princípios Fundamentais

Estes princípios são invioláveis e guiam todas as decisões de design.

### 2.1. Agência e Soberania do Usuário

O usuário é soberano no uso do sistema. Toda possibilidade que amplie a agência e o empoderamento do usuário sobre como ele usa o sistema deve ser implementada, desde que não conflite com a agência dos demais usuários (exemplo: um usuário não pode limitar o uso de armazenamento de outro usuário). O usuário pode:

- Escolher o que quer ver, quando, como e quanto.
- Configurar, reordenar, ocultar, destacar e personalizar qualquer elemento da interface.
- Reverter qualquer estado de conteúdo (ex: marcar concluído como não concluído).
- Ignorar ordenações recomendadas em coletâneas ordenadas.
- Desativar qualquer intervenção de bem-estar.

Fundamentação: A Teoria da Autodeterminação (SDT), conforme revisada por Peters et al. (2024, *Interacting with Computers*, Oxford Academic), identifica a autonomia como necessidade psicológica básica que se traduz em design através de: oferecer escolha na tarefa e no resultado, oferecer escolha nos meios, aprimorar a usabilidade, e adaptar funcionalidades a diferenças individuais. Wang et al. (2024, *Humanities and Social Sciences Communications*, Nature) demonstram que ambientes algorítmicos que restringem o alcance de escolhas do usuário conduzem a um estreitamento do self e um estado de não-autonomia.

Este princípio é referenciado ao longo de todo o documento. Quando uma decisão de design é justificada pela agência do usuário, ela deriva deste princípio.

### 2.2. Uso Saudável por Design

O sistema não deve empregar técnicas, padrões ou práticas de design que influenciem o usuário a permanecer no sistema de forma não saudável ou compulsiva. Quando o sistema oferecer intervenções ativas de proteção, estas devem ser configuráveis e desativáveis pelo usuário.

Fundamentação: Gray et al. (2024, *ACM CHI*) produziram uma ontologia de dark patterns que categoriza estratégias manipulativas de interface em cinco categorias: Sneaking, Obstruction, Interface Interference, Forced Action e Social Engineering. Mathur et al. (2019, *Proceedings of the ACM*) demonstraram a prevalência dessas práticas em larga escala. Vanden Abeele et al. (2023, *Journal of Computer-Mediated Communication*, Oxford Academic) demonstraram com 1.315 participantes que scroll infinito combinado com feed dinâmico condiciona consumo sem objetivo, propósito ou consciência. Sharpe e Spooner (2025, *Journal of Public Health*) identificam que a base neurobiológica envolve dopamina liberada a cada movimento de rolagem acoplada a esquemas de recompensa variável. A meta-análise de Roffarello e De Russis (2023, *ACM Transactions on Computer-Human Interaction*) demonstrou que ferramentas de autocontrole digital têm efeito pequeno a médio na redução de tempo em fontes distrativas, mas que intervenções percebidas como infringindo a autonomia geram frustração (Purohit e Holzer, 2025, *Frontiers in Psychiatry*).

### 2.3. Economia Cognitiva

O sistema deve minimizar a carga cognitiva extrínseca — aquela que vem do design da interface, não do conteúdo em si. Como comportamento padrão, o sistema apresenta apenas o necessário no momento necessário, com complexidade progressiva.

Isso se traduz em: formulários com disclosure progressivo (campos e seções que se revelam sob demanda em vez de apresentar tudo de uma vez), seções da dashboard configuráveis e colapsáveis, busca como caminho primário de navegação para acervos grandes, e não obrigar o usuário a preencher todos os campos de um conteúdo para poder cadastrá-lo.

**Tensão com o princípio de agência e resolução:** Este princípio envolve uma forma de choice architecture — o sistema decide o que mostrar primeiro e o que revelar sob demanda. Isso é inevitável: algum comportamento precisa vir por padrão para que o usuário não seja obrigado a configurar tudo antes de usar o sistema. A resolução dessa tensão é: o disclosure progressivo é sempre transparente (o usuário sabe que existem mais campos/opções e onde encontrá-los), sempre reversível (o usuário pode desativar o disclosure e ver tudo de uma vez nas configurações), e nunca oculta informação de forma que o usuário desconheça sua existência. O padrão existe para facilitar, não para restringir.

Fundamentação: A Teoria de Carga Cognitiva (Sweller, 2005) estabelece que a memória de trabalho humana é limitada a aproximadamente 7 ± 2 unidades de informação. A revisão sistemática de Arnold, Goldschmitt e Rigotti (2023, *Frontiers in Psychology*), baseada nos padrões PRISMA, identificou que sobrecarga de informação é causada por fatores pessoais, características da informação, parâmetros da tarefa e parâmetros da tecnologia da informação, resultando em decisões ruins, produtividade reduzida e pressões cognitivas. A mesma revisão encontrou que dashboards de visualização bem projetados reduzem a carga cognitiva e a taxa de erros. A revisão de escopo de Hämäläinen et al. (2024, *ScienceDirect*) identificou que as estratégias de filtragem, priorização e uso de ferramentas tecnológicas são centrais para lidar com sobrecarga informacional.

### 2.4. Conteúdo é Qualquer Coisa

Conteúdo no sistema não é sinônimo de mídia digital reproduzível. Conteúdo é qualquer coisa sobre a qual o usuário queira centralizar informação, acompanhar progresso e criar relações. Pode ser um vídeo do YouTube, um arquivo MP3 local, um caderno físico de desenhos, ou um jogo de videogame. A fonte e a reprodução são opcionais; o registro, a gestão e a relação são o núcleo.

### 2.5. Offline-First

O sistema roda sem internet. Conteúdos com fonte exclusivamente online devem ser gerenciáveis (progresso, anotações, metadados) mesmo offline. Quando recursos estiverem indisponíveis, o sistema deve sinalizar a indisponibilidade e apresentar o que for possível.

---

## 3. Conceitos Transversais

Estes conceitos são usados em todo o modelo de domínio e devem ser compreendidos antes de prosseguir.

### 3.1. Registro vs. Visão

O sistema distingue entre dois tipos de dados:

**Registro:** Dado persistido no banco de dados do sistema. Exemplos: um conteúdo cadastrado, o progresso de um conteúdo, uma anotação, uma configuração de usuário, uma categoria.

**Visão:** Dado montado sob demanda a partir de registros e/ou fontes externas, que não é armazenado em si. Exemplos: o feed de uma subscrição, a dashboard, um resultado de busca, a visão consolidada do agregador. Visões são computadas quando solicitadas e descartadas depois.

Essa distinção é relevante para toda decisão sobre o que salvar, o que computar, e o que está disponível offline (apenas registros estão disponíveis offline com certeza; visões que dependem de fontes externas podem estar incompletas).

### 3.2. Hierarquia de Autoridade de Metadados

Metadados de um conteúdo podem vir de três origens. A hierarquia de autoridade, da maior para a menor, é:

1. **Manual:** Inserido ou editado diretamente pelo usuário. Sempre sobrescreve qualquer metadado automático. É a verdade final.
2. **Automático de fonte:** Buscado da fonte do conteúdo (thumbnail do YouTube, título da página, descrição RSS). É sugestão, nunca imposição.
3. **Automático de fontes de metadados externas:** Buscado de fontes de metadados configuradas pelo usuário (APIs de capas de livros, bases de dados de filmes, etc.).

Metadados automáticos são buscados quando o conteúdo é adicionado ao sistema e quando o usuário solicita atualização explicitamente. O sistema nunca busca metadados automaticamente em segundo plano sem solicitação (conforme princípio 2.1).

O usuário pode editar qualquer metadado buscado automaticamente, e a edição manual passa a ter precedência.

### 3.3. Montagem de Feed vs. Busca de Metadados

O sistema interage com fontes externas de duas formas distintas que não devem ser confundidas:

**Montagem de feed (descoberta de existência):** Quando o usuário abre uma coletânea do tipo Subscrição ou o agregador, o sistema consulta as fontes externas configuradas para descobrir quais conteúdos existem (ex: quais vídeos novos um canal do YouTube publicou). Essa operação descobre a existência de itens e é gatilhada pela ação do usuário de visualizar a coletânea. Seus resultados são uma visão (seção 3.1) — não são persistidos, a menos que o usuário interaja com algum item.

**Busca de metadados (obtenção de informações sobre um conteúdo):** Quando um conteúdo é adicionado ao sistema (manualmente ou a partir de uma interação com um item do feed), o sistema pode buscar informações sobre ele — título, descrição, thumbnail, duração, etc. — na fonte de origem ou em fontes de metadados configuradas pelo usuário. Essa operação enriquece um registro existente e é regida pela hierarquia de autoridade de metadados (seção 3.2): só ocorre no momento da adição e quando o usuário solicita atualização explicitamente.

A distinção é importante porque a montagem de feed é uma operação de leitura efêmera (monta e descarta), enquanto a busca de metadados é uma operação que pode alterar registros persistidos. Ambas dependem de internet, mas têm semânticas e momentos de execução diferentes.

---

## 4. Modelo de Domínio

### 4.1. Conteúdo (Entidade Central)

Conteúdo é a entidade central do sistema. Todo o restante orbita em torno dela.

Um conteúdo é classificado em dois eixos independentes:

**Eixo 1 — Formato de Mídia:** Define a natureza material do conteúdo e determina qual reprodutor utilizar. Os formatos são:

| Formato | Descrição | Exemplos |
|---|---|---|
| áudio | Conteúdo sonoro | música, podcast, audiobook, gravação |
| texto | Conteúdo textual | livro, artigo, conto, matéria, documentação |
| vídeo | Conteúdo audiovisual | vídeo do YouTube, filme, episódio, aula gravada |
| imagem | Conteúdo visual estático | fotografia, ilustração, infográfico, quadrinho |
| interativo | Conteúdo que exige interação para consumo | jogo, aplicativo interativo, simulador |
| misto | Combinação de formatos em uma unidade | post de rede social (texto + imagem + vídeo), página web |
| nenhum | Conteúdo sem mídia digital | caderno físico, jogo de tabuleiro, experiência presencial |

O formato pode ter um **subtipo livre** que refina a classificação (ex: formato "áudio", subtipo "música"; formato "texto", subtipo "artigo"). Subtipos de formato definem **classificação** — o que o conteúdo é.

**Eixo 2 — Papel Estrutural:** Define como o conteúdo funciona no sistema e como ele se relaciona com outros conteúdos:

| Papel | Descrição |
|---|---|
| Item | Conteúdo atômico, não agrupa outros conteúdos |
| Coletânea | Agrupamento de conteúdos, com comportamento definido pelo tipo de coletânea (ver seção 4.2) |

**Nota de design:** Esses dois eixos são independentes e ortogonais. Um item pode ter qualquer formato. Uma coletânea pode conter itens de qualquer formato. O formato determina o reprodutor; o papel determina a estrutura.

**Atributos do conteúdo:**

| Atributo | Descrição |
|---|---|
| Título | Nome do conteúdo (obrigatório) |
| Descrição | Texto descritivo livre |
| Formato de mídia | Um dos formatos da tabela acima |
| Subtipo de formato | Refinamento livre do formato. Pode estar vazio |
| Papel estrutural | Item ou coletânea |
| Anotações | Texto livre do usuário |
| Nota | Valor numérico de 0 a 10 |
| Classificação | Gostei, não gostei, inconclusivo |
| Categorias | Lista de categorias livres (ver seção 4.5) |
| Imagens | Zero ou mais imagens, sendo uma principal quando houver ao menos uma (ver seção 4.7) |
| Conteúdos relacionados | Lista de vínculos bidirecionais com tipo de relação (ver seção 4.6) |
| Fontes | Zero ou mais fontes com prioridade (ver seção 4.3) |
| Acompanhamento | Progresso global do conteúdo (ver seção 4.4) |
| Histórico de ações | Log de eventos relevantes sobre o conteúdo (ver seção 4.4) |

Conforme o princípio de economia cognitiva (seção 2.3), o único atributo obrigatório para cadastrar um conteúdo é o título. Todos os demais podem ser preenchidos posteriormente ou nunca.

### 4.2. Coletânea

Uma coletânea é um conteúdo cujo papel estrutural é agrupar outros conteúdos. Uma coletânea pode conter itens e outras coletâneas.

**Anotação contextual:** A inclusão de um conteúdo em uma coletânea pode carregar uma anotação própria, além da anotação global do conteúdo. Essa anotação pertence à relação conteúdo-coletânea, não ao conteúdo nem à coletânea isoladamente. Exemplo: o mesmo filme está em duas listas; em uma, a anotação contextual é "assistir em dia chuvoso"; na outra, é "ver antes do documentário sobre o diretor".

**Restrição de ciclo:** Uma coletânea não pode conter a si mesma, direta ou indiretamente. Se a Coletânea A contém a Coletânea B, então B não pode conter A (nem qualquer coletânea que contenha A). Relações cíclicas são proibidas. O sistema valida essa restrição no momento da adição.

**Tipos de coletânea:**

Os tipos de coletânea definem **comportamento** — como a coletânea funciona — e não classificação de mídia. Essa é uma decisão de design consciente: enquanto os subtipos de formato classificam o que o conteúdo é, os tipos de coletânea determinam como ela opera.

**Guiada:** Tutoriais, guias, planos de estudo. Possui ordenação definida pelo usuário que representa uma recomendação de fluxo de consumo, não uma imposição. O usuário pode ignorar a ordem livremente (conforme princípio 2.1). Conteúdos normalmente são consumidos uma vez, revisitados apenas como referência.

Exemplo: Uma coletânea "Aprender Programação" contendo artigos científicos, vídeos do YouTube, playlists do YouTube, posts de LinkedIn, e outras coletâneas como "Fundamentos de Orientação a Objetos" ou "Introdução a Cybersecurity".

**Miscelânea:** Não há ordem obrigatória de consumo. Não importa se o usuário já consumiu um elemento antes. Possui ordenação padrão configurável (data de adição, alfabética, etc.) que o usuário pode alterar.

**Subscrição:** Alimentada por fontes externas configuradas na coletânea. Os itens são uma visão (ver seção 3.1) — montados sob demanda quando o usuário visualiza a coletânea, apresentados do mais recente ao mais antigo com paginação. Apenas itens com os quais o usuário interagiu (salvou metadados, registrou progresso, fez anotação, deu nota) geram registros persistentes. Quando offline, apenas itens com registros salvos são apresentados, com indicador de que a lista está incompleta e que a reprodução de conteúdos online pode falhar. O sistema verifica novidades quando os itens da coletânea são apresentados (ver seção 3.3 para a distinção entre montagem de feed e busca de metadados).

Cadastro de fontes para subscrição: o usuário fornece um identificador conforme a plataforma — URL de canal ou playlist do YouTube, URL de feed RSS para sites, @ de criador para Instagram, link de lista de desejos para Amazon, entre outros. O sistema também permite, quando as plataformas externas viabilizarem, que o usuário busque por dentro do sistema.

**Configurações comportamentais de qualquer coletânea:**

Além do tipo, toda coletânea possui configurações comportamentais que o usuário pode ajustar:

- **Ordenação:** O usuário pode definir se a coletânea tem uma ordenação recomendada (e qual) ou se a ordem é indiferente. Coletâneas Guiadas vêm com ordenação habilitada por padrão; Miscelâneas vêm sem.
- **Acompanhamento sequencial:** O usuário pode habilitar a apresentação de indicadores de progresso sequencial (qual é o próximo item, quantos faltam) para coletâneas onde ele quer consumir na ordem. Exemplo: uma saga de livros ou uma temporada de série.

Isso substitui a necessidade de tipos distintos como "Série" e "Franquia". A distinção entre agrupar conteúdos da mesma mídia (ex: todos os jogos de Resident Evil) e agrupar cross-mídia (ex: jogos + filmes + livros de Resident Evil) é feita pela composição: uma coletânea "Resident Evil" pode conter coletâneas filhas ("RE — Jogos", "RE — Filmes", "RE — Livros"), cada uma com seus próprios itens e configurações de ordenação. Não há restrição técnica de formato de mídia dentro de uma coletânea — o usuário organiza como preferir.

### 4.3. Fonte

Um conteúdo pode ter zero ou mais fontes. Cada fonte tem uma prioridade definida pelo usuário. Quando a fonte de maior prioridade não está disponível, o sistema tenta a próxima na lista (fallback). Esta lógica se aplica a todos os formatos de mídia.

**Tipos de fonte possíveis:**

- URL (link para YouTube, Instagram, site, etc.)
- Arquivo local (caminho no sistema de arquivos do PC)
- RSS
- Identificador de plataforma (@ de um criador, ID de playlist, etc.)

O sistema de arquivos do computador onde o sistema está rodando é uma fonte de conteúdo multimídia com o mesmo status de qualquer outra fonte — inclusive com a mesma possibilidade de ficar indisponível (arquivo deletado, HD externo desconectado, etc.).

Um conteúdo pode existir no sistema sem nenhuma fonte associada. Nesses casos, o sistema atende à necessidade de centralizar, estruturar e relacionar informações sobre o conteúdo (anotações, progresso, categorias, relações, imagens) sem oferecer reprodução.

### 4.4. Acompanhamento e Histórico

#### 4.4.1. Progresso

O progresso é **global**: pertence ao conteúdo, não à coletânea. Se um conteúdo está em duas coletâneas e o usuário consome 50%, ambas refletem 50%. Conteúdos não precisam estar em coletâneas para ter progresso. Alterações no progresso refletem em todos os lugares onde o conteúdo aparece.

**Estrutura do progresso:**

- Estado: não iniciado, em andamento, concluído.
- Posição atual: depende do formato de mídia (timestamp para vídeo/áudio, posição/página para texto, campo de texto livre para formatos sem progresso estruturado como jogos).
- Histórico de consumo: lista de datas de consumo com porcentagem consumida em cada data, quando aplicável.
- Campo de texto para progresso manual: para conteúdos onde a natureza do progresso depende inteiramente do gênero, subgênero ou título específico (ex: jogos onde progresso pode ser fases, pontos de mapa, ou algo completamente próprio).

**Marcação de progresso:**

- O usuário sempre pode marcar e editar o progresso manualmente.
- Quando o conteúdo é consumido pelo reprodutor interno do sistema, o sistema oferece a opção de marcar automaticamente o progresso.
- O usuário pode reverter qualquer estado de progresso (conforme princípio 2.1).

#### 4.4.2. Histórico de Ações

Todo conteúdo possui um log de ações relevantes que registra automaticamente:

- Data de adição ao sistema.
- Datas de edição de metadados (com indicação do campo alterado).
- Adições e remoções de/em coletâneas.
- Alterações de progresso.

O histórico serve tanto ao uso cotidiano ("quando eu adicionei isso?", "quando eu movi para essa coletânea?") quanto ao requisito de recuperação de erros e continuidade de uso interrompido (seção 9.2).

### 4.5. Categorias

Categorias são **tags livres** criadas pelo usuário. Não há taxonomia fixa nem hierarquia.

**Comportamento de autocompletar:** Conforme o usuário digita uma nova categoria, o sistema apresenta categorias existentes que contenham o texto digitado. Se o usuário seleciona uma já existente, o sistema utiliza o registro existente sem duplicar. Categorias e tipos de relação (seção 4.6) seguem este mesmo padrão de não duplicação.

### 4.6. Conteúdos Relacionados

Um conteúdo pode ser vinculado a outros conteúdos com um **tipo de relação** textual (ex: "é sequência de", "é baseado em", "mesmo universo").

**Bidirecionalidade:** Se o Conteúdo A registra uma relação com o Conteúdo B:
- Na tela de dados do Conteúdo A, o Conteúdo B aparece na seção "Conteúdos relacionados", agrupado pelo tipo de relação.
- Na tela de dados do Conteúdo B, o Conteúdo A aparece na seção "Citado por" (ou equivalente), também agrupado pelo tipo de relação.

**Tipos de relação** seguem o mesmo comportamento de não duplicação das categorias: autocompletar com sugestões existentes durante a digitação.

### 4.7. Imagens do Conteúdo

Um conteúdo pode ter zero ou mais imagens, sendo uma designada como principal quando houver ao menos uma.

Origem das imagens — configurável pelo usuário em suas preferências, com prioridade quando mais de uma estiver selecionada:

- Busca automática na fonte do conteúdo (thumbnail do YouTube, capa do livro, etc.)
- Busca por nome em fontes de metadados cadastradas pelo usuário.
- Seleção manual de arquivos no sistema de arquivos do PC.

As imagens automáticas seguem a hierarquia de autoridade de metadados (seção 3.2): se o usuário selecionar ou enviar uma imagem manualmente, ela tem precedência sobre as automáticas.

---

## 5. Funcionalidades Principais

### 5.1. Dashboard

A tela inicial do sistema é determinada pelo usuário (conforme princípio 2.1). A apresentação padrão é análoga a uma tela de streaming com seções distintas, todas configuráveis e colapsáveis (conforme princípio 2.3):

- **Continuar:** Conteúdos iniciados mas não concluídos, com gancho para reproduzir de onde parou e opção de reiniciar do início.
- **Coletâneas — Conteúdos novos / não iniciados.**
- **Descobrir no acervo:** Conteúdos do acervo do próprio usuário que ele ainda não consumiu, selecionados aleatoriamente. Esta seleção não é recomendação algorítmica — não otimiza para engajamento, não prioriza por perfil comportamental, não analisa padrões de consumo. É uma amostra aleatória simples do acervo não consumido, com a finalidade de facilitar ao usuário o descobrimento de conteúdos que ele mesmo cadastrou mas ainda não acessou. O usuário pode desativar ou configurar esta seção.
- **Coletâneas — Conteúdos de miscelâneas:** Com opções de iniciar reprodução da miscelânea onde o conteúdo está contido (com gancho para o conteúdo), iniciar a miscelânea desde o início, ou reprodução isolada sem vínculo à miscelânea.
- Filtros, ordenação, categorias.

O usuário escolhe quais seções quer ver ao abrir o sistema e em que ordem. As seções acima são o padrão, não uma imposição.

### 5.2. Agregador de Conteúdo

O agregador é uma **visão consolidada** (ver seção 3.1) de múltiplas coletâneas do tipo subscrição. Enquanto cada subscrição mostra o feed de uma fonte individual, o agregador combina múltiplas subscrições em uma visão unificada, mantendo todos os filtros e controles disponíveis ao usuário.

O agregador materializa o pilar de agregação (seção 1): substituir o feed das redes sociais por um ambiente que o usuário controla.

**Apresentação:**
- Conteúdos exibidos em cards com imagem, título e breve descrição quando a plataforma de origem permitir a obtenção desses metadados.
- Quando a reprodução interna é tecnicamente viável, o card leva ao reprodutor.
- Quando apenas reprodução externa é possível, redireciona para o conteúdo externo, verificando se é possível usar gancho para o progresso existente.
- Quando nem metadados são obteníveis, apresenta apenas o link.

**Padrões proibidos no agregador (ver seção 6.1 para fundamentação completa):**
- Sem scroll infinito.
- Sem recomendação automática de conteúdos.
- Sem métricas sociais (likes, comentários de terceiros, contadores de visualização).
- Sem algoritmo de ranqueamento.
- Sem autoplay entre conteúdos.

**Filtros disponíveis:**
- Por criador/fonte.
- Esconder conteúdos já consumidos.
- Esconder conteúdos cujo título ou descrição contenham palavras-chave definidas pelo usuário.
- Ordenação cronológica (mais recente primeiro, por padrão).

### 5.3. Reprodutor

O reprodutor consome conteúdo internamente quando tecnicamente viável. O comportamento padrão de reprodução pode ser configurado pelo usuário por subtipo de formato (ex: o subtipo "artigo" de formato "texto" abre por padrão no leitor interno; o subtipo "livro" abre no aplicativo padrão do OS). O usuário pode, ao abrir um conteúdo específico, escolher um comportamento diferente do padrão.

**Texto:**
- Texto puro, Markdown e HTML.
- Fonte (família, tamanho, cor, etc.) definida pelo usuário em configuração global.
- Respeita estilo quando intencionalmente aplicado ao texto pelo autor.

**Áudio:**
- Reprodução de arquivos locais.
- Fallback para fontes online conforme prioridade configurada nas fontes do conteúdo.

**Vídeo:**
- Embed de vídeos de plataformas que permitam (YouTube, etc.).
- Fallback para abertura externa.

**Abertura externa (disponível para todos os formatos pertinentes):**
- No aplicativo padrão do sistema operacional.
- Em aplicativo escolhido pelo usuário.

**Ganchos (bookmarks dentro do conteúdo):**
- O usuário pode criar marcações em partes específicas do conteúdo sendo reproduzido (marcação de tempo em vídeos/áudios, posições em textos, referência a elementos de coletâneas).
- Ganchos podem ser usados em coletâneas para indicar ponto de início (ex: "assista a partir do minuto 12:30").
- Quando o conteúdo é consumido internamente, o sistema oferece marcação automática de progresso.

### 5.4. Busca e Filtragem

A busca é o caminho primário de navegação para acervos grandes (conforme princípio 2.3).

**Busca interna (sobre conteúdos cadastrados no sistema):**

Filtros disponíveis:
- Por formato de mídia.
- Por papel estrutural (itens, coletâneas, ou ambos).
- Por tipo de coletânea (guiada, miscelânea, subscrição).
- Por categoria.
- Por nota (intervalo numérico).
- Por classificação (gostei, não gostei, inconclusivo).
- Por estado de progresso (não iniciado, em andamento, concluído).
- Por data de adição (intervalo de datas).
- Por fonte/criador.
- Busca textual em título, descrição e anotações do usuário.

Esses filtros podem ser combinados livremente.

**Busca externa (nas plataformas externas):**

O sistema permite buscar conteúdo novo nas plataformas externas que viabilizem isso. O usuário escolhe o escopo da busca (interno, externo, ambos) conforme as possibilidades oferecidas pelo sistema.

### 5.5. Operações em Lote

O sistema deve permitir que o usuário opere sobre múltiplos conteúdos simultaneamente. Exemplos: adicionar uma categoria a múltiplos conteúdos, mover vários itens para uma coletânea, marcar múltiplos como concluídos, remover múltiplos de uma coletânea. Isso é necessário para a viabilidade prática de gerenciar acervos com centenas ou milhares de conteúdos.

---

## 6. Uso Saudável

### 6.1. Padrões Proibidos por Design

O sistema **nunca** implementará os seguintes padrões, independentemente de configuração:

| Padrão proibido | Fundamentação científica |
|---|---|
| Scroll infinito | Vanden Abeele et al. (2023, *J. Comput.-Mediat. Commun.*, Oxford Academic): condiciona consumo sem objetivo ou consciência. Sharpe & Spooner (2025, *J. Public Health*): base neurobiológica em dopamina com esquemas de recompensa variável. |
| Autoplay entre conteúdos | Sharpe & Spooner (2025, *J. Public Health*): autoplay é um dos mecanismos que mantêm o loop de rolagem contínua. Gray et al. (2024, *ACM CHI*): classificado como Interface Interference na ontologia de dark patterns. |
| Algoritmo de ranqueamento | Wang et al. (2024, *Humanit. Soc. Sci. Commun.*, Nature): bolhas de filtro criam ambiente determinístico que restringe acesso a conteúdo diverso e estreita o self do usuário. |
| Métricas sociais de terceiros | Gray et al. (2024, *ACM CHI*): Social Engineering como categoria de dark pattern. Pressão social via likes/contadores explora vieses de conformidade. |
| Notificações não solicitadas | Olson et al. (2022, *Int. J. Ment. Health Addict.*): desabilitar notificações não essenciais é uma das estratégias de nudging com eficácia demonstrada em ensaio clínico randomizado. |

### 6.2. Paginação

Toda apresentação de elementos em feeds, listas e tabelas usa **paginação**. Quantidade de itens por página:

- Configurável pelo usuário no cabeçalho ou rodapé da paginação.
- Valor padrão definido pelo admin nas configurações globais.
- **Valor padrão sugerido: 20 itens.**

Fundamentação: Rixen et al. (2023, *Proceedings of the ACM on Human-Computer Interaction*, ACM) investigaram o comportamento de scroll infinito em aplicações de mídias sociais e identificaram que ele cria um "loop" de uso habitual e arrependido, onde o scroll infinito sequestra sessões que começaram com outros propósitos. O estudo de usabilidade de Murano e Lomas (2020, *First Monday*, revisado por pares) avaliou experimentalmente quatro métodos de rolagem (paginação, scroll infinito, "carregar mais", e híbrido) e encontrou resultados mistos — nenhum método único se destacou como universalmente superior, mas paginação ofereceu melhor orientação espacial e senso de posição para tarefas orientadas a objetivo.

**Nota de transparência sobre o valor padrão:** O valor específico de 20 itens por página não deriva de pesquisa experimental publicada em periódico científico. Estudos de usabilidade na literatura cinza de profissionais (Baymard Institute) sugerem intervalos de 10 a 75 itens dependendo do contexto, mas essas pesquisas não passaram por revisão por pares. O valor de 20 é um ponto de partida pragmático que deve ser validado com os usuários reais do sistema e ajustado conforme necessário.

### 6.3. Intervenções Ativas (Opcionais e Configuráveis)

O sistema pode oferecer intervenções ativas de bem-estar. Todas são **opcionais** e **desativáveis** pelo usuário (conforme princípio 2.1).

**Intervenções sugeridas (baseadas na literatura):**

- **Monitoramento de tempo de uso:** Exibir tempo gasto no sistema na sessão atual. Fundamentação: Purohit e Holzer (2025, *Frontiers in Psychiatry*): rastreamento de tempo de tela como nudge comportamental demonstrou reduzir uso excessivo.
- **Lembrete de tempo configurável:** Notificação interna após período definido pelo usuário. Fundamentação: Roffarello e De Russis (2023, *ACM Trans. Comput.-Hum. Interact.*): lembretes e temporizadores são as estratégias mais comuns em ferramentas de autocontrole digital.
- **Relatório de consumo:** Resumo periódico de quanto o usuário consumiu (quantidade, tempo, formatos). Fundamentação: Olson et al. (2022, *Int. J. Ment. Health Addict.*): automonitoramento é componente central das intervenções com eficácia demonstrada.
- **Modo escala de cinza:** Opção de exibir a interface em escala de cinza. Fundamentação: Dekker e Baumgartner (2023) e ensaio randomizado de campo (Holte & Ferraro, 2020): modo escala de cinza reduz tempo de tela e uso problemático.

### 6.4. Alertas de Configuração Potencialmente Não Saudável

Quando o usuário fizer configurações que se aproximem de padrões identificados como dark patterns ou uso não saudável, o sistema deve emitir um alerta informativo (não bloqueante). Exemplos:

- Configurar paginação com número muito alto de itens por página (ex: acima de 100), o que na prática se aproxima do comportamento de scroll infinito.
- Desativar todas as intervenções de bem-estar simultaneamente (alerta informativo, sem bloqueio).

O alerta informa, não impede. O usuário mantém a soberania (conforme princípio 2.1).

---

## 7. Modelo de Usuários e Permissões

### 7.1. Roles

O sistema possui duas roles fixas:

- **Consumidor:** Usa o sistema — consome, gerencia e acompanha conteúdos.
- **Admin:** Acessa a área de administração do sistema.

Todo usuário possui a role de consumidor. Alguns também possuem a role de admin.

As roles são fixas (o conjunto de roles não muda). O admin pode criar **grupos** que combinam roles livremente e atribuir esses grupos a usuários.

### 7.2. Áreas do Sistema

O sistema possui duas áreas completamente separadas:

**Área do consumidor:** Onde o usuário gerencia e consome seus conteúdos. Não há qualquer indicação da existência da área admin para usuários que não possuam a role admin.

**Área admin:** Acessível via menu visível apenas para usuários com role admin, a partir da área logada. A área admin é exclusivamente de configuração e gerenciamento — não é possível gerenciar ou consumir conteúdos pessoais a partir dela.

**Segurança da área admin:** Para usuários sem a role admin, a área admin deve se comportar como se não existisse. Qualquer tentativa de acesso (por URL, por manipulação, por qualquer meio) deve resultar em um comportamento genérico que não indique a existência de uma área administrativa.

### 7.3. Funcionalidades do Admin

- Gerenciar usuários: criar, ativar, inativar, resetar senhas, atribuir grupos de roles.
- Configurações globais: valores padrão para todas as configurações de usuário (itens por página, tema padrão, fontes de metadados de imagem padrão, etc.).
- Limites do sistema: limite de quantidade de usuários cadastrados, limites de conteúdos por usuário, etc.
- Exportação/importação de configurações globais.

### 7.4. Independência de Dados

Cada usuário possui seus dados completamente independentes: conteúdos, coletâneas, progresso, anotações, notas, categorias, configurações pessoais. Não há compartilhamento entre usuários neste momento. A arquitetura deve estar preparada para que compartilhamento possa ser adicionado no futuro sem reestruturação fundamental.

---

## 8. Acessibilidade

Acessibilidade é cidadã de primeira classe no sistema, não limitada a problemas genéricos. O sistema deve dar poder ao usuário para adaptar a interface às suas necessidades específicas.

**Configurações de acessibilidade controladas pelo usuário:**

- Família da fonte, tamanho, cor.
- Temas claro e escuro (padrão), com possibilidade de edições individuais e globalmente resetáveis.
- Ajustes para condições específicas: visão cansada, sensibilidade a estímulos visuais, necessidade de contraste elevado, etc.
- O sistema deve seguir princípios de design acessível (contraste, navegação por teclado, compatibilidade com leitores de tela) como base.

---

## 9. Requisitos Não-Funcionais

### 9.1. Desempenho

O sistema deve ser otimizado para não degradar o desempenho do computador do usuário. Deve ser leve e eficiente em uso de recursos (CPU, memória, disco).

### 9.2. Resiliência e Recuperação de Erros

O sistema deve assumir que erros ocorrem e estar preparado para eles:

- Capacidade de recuperar estados anteriores (apoiado pelo histórico de ações da seção 4.4.2).
- Mensagens de erro informativas que facilitem a identificação do problema por quem possa verificar.
- Continuidade do uso interrompido: se o sistema falhar durante uma operação, o usuário deve poder retomar de onde parou.

### 9.3. Segurança

O sistema deve ter proteção robusta:

- Proteção da integridade dos dados dos usuários.
- Proteção da integridade do sistema.
- Defesa contra ataques (considerando que o sistema roda localmente, o vetor de ataque é diferente de um serviço web, mas proteção é necessária mesmo assim).

### 9.4. Arquitetura

- Monolito modular: um único serviço, sem microsserviços.
- UI isolada: a camada de apresentação deve estar separada da lógica de negócio de forma que adicionar novos tipos de interface (web, API, etc.) no futuro seja prático.
- Containerização completa para o ciclo de desenvolvimento (CI/CD, testes, reprodutibilidade).
- Instalador que resolve todas as dependências, inclusive em cenários onde Docker não está disponível na máquina do usuário.

### 9.5. Comportamento Offline

O sistema é offline-first (conforme princípio 2.5). Sua operação principal não depende de internet.

**Quando online:**
- Fontes externas são acessíveis para reprodução e busca de metadados.
- Feeds de subscrição são montados com dados atualizados das fontes.

**Quando offline:**
- Conteúdos com fontes locais continuam totalmente acessíveis.
- Para conteúdos com fontes exclusivamente online: gerenciamento de informações (progresso, anotações, metadados) funciona normalmente. Reprodução pode não estar disponível.
- Feeds de subscrição apresentam apenas itens com registros previamente salvos, com indicador claro de que a lista está incompleta e que a reprodução de conteúdos online pode falhar.
- Indicadores visuais claros de quais recursos estão indisponíveis.

### 9.6. Idioma

O sistema, sua interface, mensagens, documentação e todo artefato produzido devem estar em **português brasileiro (pt-BR)**.

---

## 10. Exportação e Importação

### 10.1. Dados do Usuário (role consumidor)

Qualquer usuário pode exportar seus dados pessoais (conteúdos, coletâneas, progresso, anotações, notas, configurações pessoais) para um arquivo ou conjunto de arquivos. Esse pacote pode ser importado em outra instância do sistema rodando em outro PC, possivelmente com sistema operacional diferente.

Cenário principal: um computador foi substituído e o usuário quer continuar usando o sistema na nova máquina.

### 10.2. Configurações Globais (role admin)

Usuários com role admin podem exportar as configurações globais do sistema para importar em outra instância.

### 10.3. Importação de Dados de Plataformas Externas

O sistema deve prever um mecanismo de importação que permita ao usuário trazer dados de plataformas externas para dentro do sistema — por exemplo, histórico de vídeos assistidos do YouTube, biblioteca do Goodreads, watchlist de sites de streaming.

Essa funcionalidade é planejada para implementação futura, mas o sistema deve ser projetado com extensibilidade para isso: um mecanismo genérico de importação que permita adicionar novos parsers de plataformas sem reestruturar o núcleo. Cada parser transforma os dados da plataforma de origem nos registros do modelo de domínio do sistema (conteúdo, progresso, categorias, etc.).

### 10.4. Formato

O formato de exportação/importação deve ser agnóstico de plataforma, legível e documentado. A containerização do sistema e a possibilidade de rodar em OSs diferentes reforçam essa necessidade.

---

## 11. Embasamento Científico

### 11.1. Limites do Embasamento Científico

Este documento utiliza literatura científica revisada por pares para fundamentar decisões de design. É necessário declarar explicitamente onde o embasamento é forte, onde envolve transferência de contexto, e onde é ausente.

**Embasamento forte (evidência direta de periódicos com revisão por pares):**
- Dark patterns como fenômeno: ontologia, taxonomia e prevalência (Gray et al., 2024, ACM CHI; Mathur et al., 2019, ACM PACM HCI; Gray et al., 2023, ACM DIS).
- Scroll infinito e comportamento compulsivo: mecanismo neurobiológico e comportamental (Vanden Abeele et al., 2023, Oxford Academic; Sharpe & Spooner, 2025, SAGE; Rixen et al., 2023, ACM PACM HCI).
- Intervenções de autocontrole digital: eficácia de nudges, lembretes e monitoramento (Roffarello & De Russis, 2023, ACM TOCHI; Olson et al., 2022, Springer; Dekker & Baumgartner, 2023, Cyberpsychology).
- Autonomia e SDT em design de tecnologia (Peters et al., 2024, Oxford Academic; Wang et al., 2024, Nature).
- Sobrecarga cognitiva e informacional (Arnold et al., 2023, Frontiers in Psychology; Sweller, 2005, Cambridge).

**Embasamento com transferência de contexto (evidência existe mas em contexto diferente):**
- Toda a literatura de uso saudável e scroll infinito estuda smartphones e redes sociais — plataformas mobile com notificações push, componente social e acesso constante. O sistema que estamos projetando é um desktop app local, sem componente social e sem notificações externas. O mecanismo subjacente (recompensa variável, ausência de pontos de parada) é plausível em qualquer contexto de consumo de conteúdo sequencial, mas a magnitude do efeito em um desktop app de gestão pessoal não está demonstrada na literatura.

**Embasamento ausente (decisões pragmáticas sem evidência experimental publicada):**
- O valor específico de itens por página (20) não tem fundamento em pesquisa experimental publicada em periódico científico.
- A eficácia de alertas quando configurações se aproximam de dark patterns não foi estudada em nenhum artigo identificado.
- O limiar específico para alertar sobre paginação excessiva (ex: acima de 100 itens) é arbitrário e deve ser validado empiricamente.

---

### 11.2. Referências

1. Gray, C. M., Santos, C., Bielova, N., Toth, M., & Clifford, D. (2024). An Ontology of Dark Patterns Knowledge. *Proceedings of the 2024 CHI Conference on Human Factors in Computing Systems*. ACM.
2. Mathur, A., Acar, G., Friedman, M. J., Lucherini, E., Mayer, J., Chetty, M., & Narayanan, A. (2019). Dark Patterns at Scale: Findings from a Crawl of 11K Shopping Websites. *Proceedings of the ACM on Human-Computer Interaction*, 3(CSCW), 1–32.
3. Vanden Abeele, M. M. P., Hendrickson, A. T., Pollmann, M. M. H., & Ling, R. (2023). Does mindless scrolling hamper well-being? Combining ESM and log-data. *Journal of Computer-Mediated Communication*, 29(1), zmad056. Oxford Academic.
4. Sharpe, B. T., & Spooner, R. A. (2025). Dopamine-scrolling: a modern public health challenge requiring urgent attention. *Journal of Public Health*. SAGE Publications.
5. Roffarello, A. M., & De Russis, L. (2023). Achieving Digital Wellbeing Through Digital Self-control Tools: A Systematic Review and Meta-analysis. *ACM Transactions on Computer-Human Interaction*, 30(4), 1–66.
6. Purohit, A. K., & Holzer, A. (2025). Active nudging towards digital well-being: reducing excessive screen time on mobile phones and potential improvement for sleep quality. *Frontiers in Psychiatry*, 16, 1602997.
7. Olson, J. A., Sandra, D. A., Colucci, É. S., et al. (2022). A Nudge-Based Intervention to Reduce Problematic Smartphone Use: Randomised Controlled Trial. *International Journal of Mental Health and Addiction*, 22, 1105–1130.
8. Peters, D., Ahmadpour, N., & Calvo, R. A. (2024). Designing for Sustained Motivation: A Review of Self-Determination Theory in Behaviour Change Technologies. *Interacting with Computers*, iwae040. Oxford Academic.
9. Wang, Y., Li, J., & Zhang, X. (2024). Inevitable challenges of autonomy: ethical concerns in personalized algorithmic decision-making. *Humanities and Social Sciences Communications*, 11, 1364. Nature.
10. Nie, X., Chen, S., Li, Z., et al. (2024). A Comprehensive Study on Dark Patterns. *Proceedings of the 2024 ACM International Conference on the Foundations of Software Engineering (FSE)*. ACM.
11. Kelly, D., & Rubin, V. L. (2024). Identifying Dark Patterns in User Account Disabling Interfaces: Content Analysis Results. *Social Media + Society*, 10(1). SAGE Journals.
12. Dekker, M. R., & Baumgartner, S. E. (2023). Digital Strategies for Screen Time Reduction: A Randomized Field Experiment. *Cyberpsychology, Behavior, and Social Networking*.
13. Kocyigit, E., Rossi, A., & Lenzini, G. (2024). A Systematic Approach for A Reliable Detection of Deceptive Design Patterns Through Measurable HCI Features. *Proceedings of the 2024 European Symposium on Usable Security (EuroUSEC)*. ACM.
14. Gray, C. M., Chamorro, L. S., Obi, I., & Duane, J.-N. (2023). Mapping the Landscape of Dark Patterns Scholarship: A Systematic Literature Review. *Companion Publication of the 2023 ACM Designing Interactive Systems Conference*. ACM.
15. Sweller, J. (2005). Implications of Cognitive Load Theory for Multimedia Learning. In R. E. Mayer (Ed.), *The Cambridge Handbook of Multimedia Learning*. Cambridge University Press.
16. Arnold, M., Goldschmitt, M., & Rigotti, T. (2023). Dealing with information overload: a comprehensive review. *Frontiers in Psychology*, 14, 1122200.
17. Whittaker, S. (2011). Personal information management: From information consumption to curation. *Annual Review of Information Science and Technology*, 45, 1–62. Wiley.
18. Hämäläinen, E., et al. (2024). Causes, consequences, and strategies to deal with information overload: A scoping review. *Telematics and Informatics Reports*. ScienceDirect.
19. Rixen, J. O., Meinhardt, L.-M., Glöckler, M., et al. (2023). The Loop and Reasons to Break It: Investigating Infinite Scrolling Behaviour in Social Media Applications and Reasons to Stop. *Proceedings of the ACM on Human-Computer Interaction*, 7(MHCI), 228:1–228:22. ACM.
20. Murano, P., & Lomas, T. J. (2020). A usability evaluation of Web user interface scrolling types. *First Monday*, 25(3).

---

## Glossário

| Termo | Definição |
|---|---|
| Agência | Capacidade do usuário de exercer controle efetivo sobre suas escolhas e ações dentro do sistema |
| Anotação contextual | Anotação que pertence à relação entre um conteúdo e uma coletânea, não ao conteúdo nem à coletânea isoladamente |
| Busca de metadados | Operação que obtém informações sobre um conteúdo específico (título, descrição, imagem) de fontes externas, enriquecendo um registro existente |
| Coletânea | Conteúdo cujo papel estrutural é agrupar outros conteúdos, com comportamento definido pelo tipo (guiada, miscelânea, subscrição) e configurações comportamentais (ordenação, acompanhamento sequencial) |
| Conteúdo | Entidade central do sistema; qualquer coisa sobre a qual o usuário queira registrar e gerenciar informações |
| Dark pattern | Estratégia de design de interface que manipula o comportamento do usuário em benefício do provedor do serviço |
| Formato de mídia | Eixo de classificação que define a natureza material do conteúdo (áudio, texto, vídeo, imagem, interativo, misto, nenhum) |
| Fonte | Origem de um conteúdo (URL, arquivo local, RSS, identificador de plataforma, ou nenhuma) |
| Gancho | Marcação em parte específica de um conteúdo que permite retornar àquele ponto |
| Montagem de feed | Operação que consulta fontes externas para descobrir quais conteúdos existem, produzindo uma visão efêmera |
| Nudge | Intervenção comportamental que altera a arquitetura de escolhas sem proibir opções |
| Papel estrutural | Eixo de classificação que define como o conteúdo funciona no sistema (item ou coletânea) |
| Pilar de agregação | Funcionalidade de trazer conteúdo de fontes externas para um feed controlado pelo usuário |
| Pilar de gestão | Funcionalidade de registrar, anotar, avaliar e acompanhar conteúdos de qualquer natureza |
| Progresso | Estado de acompanhamento do consumo de um conteúdo, pertencente ao conteúdo globalmente |
| Registro | Dado persistido no banco de dados do sistema |
| Visão | Dado montado sob demanda a partir de registros e/ou fontes externas, não armazenado em si |

---

## Apêndice A — Validação por Cenários

Este apêndice percorre cenários concretos extraídos do esboço original do usuário e verifica se o modelo de domínio os atende. Quando o modelo falha ou atende parcialmente, a lacuna é identificada e resolvida.

### Cenário 1: "Crio listas de filmes temáticas e anoto 'esse não achei dublado' e 'assistir em dia chuvoso'"

Caminho no modelo: O usuário cria uma coletânea do tipo Miscelânea ("Filmes de Terror para Noites Chuvosas"). Adiciona conteúdos do formato "vídeo" com subtipo "filme". As anotações "não achei dublado" e "assistir em dia chuvoso" são registradas no campo Anotações do conteúdo.

**Lacuna identificada:** As anotações são atributos do conteúdo, não da relação conteúdo-coletânea. Se o mesmo filme está em duas listas e o usuário quer fazer uma anotação específica ao contexto de cada lista (ex: "nessa lista, assistir esse antes daquele"), o modelo não permite.

**Resolução:** Adicionar a possibilidade de anotações contextuais na relação conteúdo-coletânea. A inclusão de um conteúdo em uma coletânea pode carregar uma anotação própria, além da anotação global do conteúdo. Isso é um atributo da associação, não do conteúdo nem da coletânea.

### Cenário 2: "Comprei CDs físicos e crio playlists com músicas de diversos CDs, com fallback para Spotify ou YouTube"

Caminho no modelo: Cada música é um conteúdo com formato "áudio", subtipo "música". Fonte primária: arquivo local (caminho no sistema de arquivos). Fontes secundárias: URL do Spotify, URL do YouTube, com prioridades 2 e 3. A playlist é uma coletânea do tipo Miscelânea contendo as músicas. Quando o arquivo local não está disponível (HD externo desconectado), o sistema tenta a fonte seguinte na lista de prioridades.

**Resultado:** O modelo atende completamente este cenário.

### Cenário 3: "Um caderno físico de desenhos que quero registrar no sistema"

Caminho no modelo: O usuário cria um conteúdo com formato "nenhum" (sem mídia digital). Sem fontes. Preenche título, anotações, categorias. Pode subir fotos das páginas como imagens do conteúdo. Pode registrar progresso manual ("desenhei até a página 30"). Pode depois adicionar uma fonte (link do OneDrive com fotos escaneadas).

**Resultado:** O modelo atende completamente este cenário.

### Cenário 4: "Plano de estudo sobre programação contendo artigos, vídeos, playlists e outras coletâneas como 'Cybersecurity'"

Caminho no modelo: O plano de estudo é uma coletânea do tipo Guiada. Contém itens com formatos variados (texto/artigo, vídeo, misto/post) e outras coletâneas (ex: coletânea Guiada "Fundamentos de Cybersecurity"). A ordenação indica a sequência recomendada. O usuário pode ignorar a ordem. Progresso de cada item é global.

**Resultado:** O modelo atende completamente este cenário.

### Cenário 5: "Acompanhar a franquia Resident Evil com jogos, filmes e livros"

Caminho no modelo: O usuário cria uma coletânea "Resident Evil" com comportamento de agrupamento ordenado e cross-mídia habilitado. Dentro dela, pode adicionar diretamente os conteúdos ou criar coletâneas filhas ("RE — Jogos", "RE — Filmes") com comportamento de agrupamento ordenado e restrição de mesma mídia. Cada jogo, filme ou livro é um conteúdo com seu formato respectivo. Progresso é rastreado individualmente em cada conteúdo.

**Resultado:** O modelo atende completamente este cenário após a unificação de Série/Franquia (ver seção 4.2).

### Cenário 6: "Seguir criadores no YouTube e Instagram sem scroll infinito, sem likes, sem algoritmo"

Caminho no modelo: O usuário cria coletâneas do tipo Subscrição para cada criador (URL do canal YouTube, @ do Instagram). O agregador monta uma visão consolidada de todas as subscrições. Sem scroll infinito (paginação). Sem métricas sociais. Sem ranqueamento. Filtros por criador e palavras-chave disponíveis. Cards com metadados quando possível, links quando não.

**Lacuna condicional:** Plataformas como Instagram e TikTok podem dificultar ou impedir a obtenção de metadados. Nesses casos, o sistema apresenta o que for possível (conforme já documentado na seção 5.2).

**Resultado:** O modelo atende, com degradação graceful para plataformas restritivas.

### Cenário 7: "Primeiro uso do sistema sem internet"

Caminho no modelo: O agregador e as subscrições estão vazios (sem feeds para montar). Mas o usuário pode criar conteúdos manualmente, organizar coletâneas do tipo Guiada e Miscelânea, adicionar conteúdos com fontes locais, registrar progresso, fazer anotações. A gestão de conteúdo pessoal funciona integralmente offline.

**Resultado:** O pilar de gestão de conteúdo atende completamente. O pilar de agregação está indisponível mas não impede o uso do sistema. Isso está documentado na seção 1 (dois pilares).
