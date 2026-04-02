# Definição de Domínio — Sistema de Gestão e Consumo de Conteúdo com Agência do Usuário

## 1. Propósito do Sistema

Este sistema existe para devolver ao usuário a agência sobre seu consumo de conteúdo. As plataformas digitais predominantes otimizam para o engajamento da plataforma, não para o bem-estar de quem as utiliza — empregando dark patterns, algoritmos de ranqueamento, scroll infinito, autoplay e métricas sociais que exploram vieses cognitivos e induzem comportamentos compulsivos. Este sistema inverte essa lógica: o usuário decide o quê, como e quanto consome, em um ambiente projetado para não sabotar — e, opcionalmente, proteger ativamente — seu bem-estar.

O sistema centraliza a gestão, o acompanhamento e a reprodução de conteúdos de qualquer natureza — digitais ou físicos, online ou offline — oferecendo ao usuário ferramentas para estruturar, relacionar e acompanhar informações sobre o que consome, estuda ou produz.

**O sistema não é uma rede social. Não é um serviço web. É um programa de computador nativo**, análogo ao Calibre ou à calculadora do sistema operacional, que roda localmente na máquina do usuário sem depender de internet para suas funcionalidades centrais.

---

## 2. Princípios Fundamentais

Estes princípios são invioláveis e guiam todas as decisões de design:

### 2.1. Agência e Soberania do Usuário

O usuário é soberano no uso do sistema. Toda possibilidade que amplie a agência e o empoderamento do usuário sobre como ele usa o sistema deve ser implementada, desde que não conflite com a agência dos demais usuários. O usuário pode:

- Escolher o que quer ver, quando, como e quanto.
- Configurar, reordenar, ocultar, destacar e personalizar qualquer elemento da interface.
- Marcar conteúdos consumidos como não consumidos.
- Furar a ordem de coletâneas ordenadas.
- Desativar qualquer intervenção de bem-estar.

Fundamentação: A Teoria da Autodeterminação (SDT), conforme revisada por Peters et al. (2024, *Interacting with Computers*, Oxford Academic), identifica a autonomia como necessidade psicológica básica que se traduz em design através de: oferecer escolha na tarefa e no resultado, oferecer escolha nos meios, aprimorar a usabilidade, e adaptar funcionalidades a diferenças individuais. Wang et al. (2024, *Humanities and Social Sciences Communications*, Nature) demonstram que ambientes algorítmicos que restringem o alcance de escolhas do usuário conduzem a um estreitamento do self e um estado de não-autonomia.

### 2.2. Uso Saudável por Design

O sistema não deve empregar técnicas, padrões ou práticas de design que influenciem o usuário a permanecer no sistema de forma não saudável ou compulsiva. Quando o sistema oferecer intervenções ativas de proteção, estas devem ser configuráveis e desativáveis pelo usuário.

Fundamentação: Gray et al. (2024, *ACM CHI*) produziram uma ontologia de dark patterns que categoriza estratégias manipulativas de interface em cinco categorias: Sneaking, Obstruction, Interface Interference, Forced Action e Social Engineering. Mathur et al. (2019, *Proceedings of the ACM*) demonstraram a prevalência dessas práticas em larga escala. Vanden Abeele et al. (2023, *Journal of Computer-Mediated Communication*, Oxford Academic) demonstraram com 1.315 participantes que scroll infinito combinado com feed dinâmico condiciona consumo sem objetivo, propósito ou consciência. Sharpe e Spooner (2025, *Journal of Public Health*) identificam que a base neurobiológica envolve dopamina liberada a cada movimento de rolagem acoplada a esquemas de recompensa variável. A meta-análise de Roffarello e De Russis (2023, *ACM Transactions on Computer-Human Interaction*) demonstrou que ferramentas de autocontrole digital têm efeito pequeno a médio na redução de tempo em fontes distrativas, mas que intervenções percebidas como infringindo a autonomia geram frustração (Purohit e Holzer, 2025, *Frontiers in Psychiatry*).

### 2.3. Conteúdo é Qualquer Coisa

Conteúdo no sistema não é sinônimo de mídia digital reproduzível. Conteúdo é qualquer coisa sobre a qual o usuário queira centralizar informação, acompanhar progresso e criar relações. Pode ser um vídeo do YouTube, um arquivo MP3 local, um caderno físico de desenhos, ou um jogo de videogame. A fonte e a reprodução são opcionais; o registro, a gestão e a relação são o núcleo.

### 2.4. Offline-First

O sistema roda sem internet. Conteúdos com fonte exclusivamente online devem ser gerenciáveis (progresso, anotações, metadados) mesmo offline. Quando recursos estiverem indisponíveis, o sistema deve sinalizar a indisponibilidade e apresentar o que for possível.

---

## 3. Modelo de Domínio

### 3.1. Conteúdo (Entidade Central)

Conteúdo é a entidade central do sistema. Todo o restante orbita em torno dela.

**Atributos:**

| Atributo | Descrição |
|---|---|
| Título | Nome do conteúdo |
| Descrição | Texto descritivo livre |
| Tipo | audio, texto, imagem, video, post, coletanea, serie, jogo, outro |
| Subtipo | Depende do tipo (ver tabela abaixo). Pode estar vazio |
| Anotações | Texto livre do usuário |
| Nota | Valor numérico de 0 a 10 |
| Classificação | Gostei, não gostei, inconclusivo |
| Categorias | Lista de categorias livres (ver seção 3.5) |
| Imagens | Zero ou mais imagens, sendo uma principal quando houver ao menos uma |
| Conteúdos relacionados | Lista de vínculos bidirecionais com tipo de relação (ver seção 3.6) |
| Fontes | Zero ou mais fontes com prioridade (ver seção 3.3) |
| Acompanhamento | Progresso global do conteúdo (ver seção 3.4) |

**Subtipos por tipo:**

| Tipo | Subtipos possíveis |
|---|---|
| audio | música |
| texto | livro, artigo, conto, matéria |
| video | (livre) |
| post | (livre) |
| coletânea | guiada, miscelânea, subscrição, franquia |
| série | (livre) |
| jogo | (livre) |
| imagem | (livre) |
| outro | (livre) |

O subtipo também pode definir comportamentos padrão configuráveis pelo usuário — por exemplo, em conteúdos do tipo "texto", o subtipo pode determinar se o comportamento padrão de reprodução é abrir internamente no leitor do sistema, abrir externamente no aplicativo padrão do OS, ou abrir em aplicativo escolhido pelo usuário.

### 3.2. Coletânea

Uma coletânea é formada de zero ou mais conteúdos. Uma coletânea é, ela mesma, um tipo de conteúdo — o que significa que pode ser adicionada como item de outra coletânea.

**Restrição de ciclo:** Uma coletânea não pode conter a si mesma, direta ou indiretamente. Se a Coletânea A contém a Coletânea B, então B não pode conter A (nem qualquer coletânea que contenha A). Relações cíclicas são proibidas.

**Tipos de coletânea:**

**Guiada:** Tutoriais, guias, planos de estudo. Possui ordenação definida pelo usuário que representa uma recomendação de fluxo de consumo, não uma imposição. O usuário pode furar a ordem livremente. Conteúdos normalmente são consumidos uma vez, revisitados apenas como referência.

Exemplo: Uma coletânea "Aprender Programação" contendo artigos científicos, vídeos do YouTube, playlists de YouTube, posts de LinkedIn, e outras coletâneas como "Fundamentos de Orientação a Objetos" ou "Introdução a Cybersecurity".

**Miscelânea:** Não há ordem obrigatória de consumo. Não importa se o usuário já consumiu um elemento antes. Possui ordenação padrão configurável (data de adição, alfabética, etc.) que o usuário pode alterar.

**Subscrição:** Alimentada por fontes externas configuradas na coletânea (canal do YouTube, RSS de um site, @ de um criador no Instagram, playlist de um canal, lista de desejos da Amazon, etc.). Os itens não são fixos — são montados sob demanda quando o usuário visualiza a coletânea, apresentados do mais recente ao mais antigo com paginação. O feed não é persistido: apenas itens com os quais o usuário interagiu (salvou metadados, registrou progresso, fez anotação, deu nota) geram registros persistentes. Quando offline, apenas itens com metadados salvos são apresentados, com indicador de que conteúdos online podem não carregar.

**Franquia:** Agrupa conteúdos de mídias diferentes pertencentes à mesma franquia. Exemplo: a Franquia "Resident Evil" contém jogos, filmes, séries de televisão e livros.

**Série vs. Franquia:** A distinção é: Série agrupa conteúdos da mesma mídia dentro de uma franquia (todos os jogos de Resident Evil). Franquia agrupa transversalmente entre mídias (jogos + filmes + livros de Resident Evil). Uma Série pode ser um item dentro de uma Franquia.

### 3.3. Fonte

Um conteúdo pode ter zero ou mais fontes. Cada fonte tem uma prioridade definida pelo usuário. Quando a fonte de maior prioridade não está disponível, o sistema tenta a próxima na lista (fallback).

**Tipos de fonte possíveis:**

- URL (link para YouTube, Instagram, site, etc.)
- Arquivo local (caminho no sistema de arquivos do PC)
- RSS
- Identificador de plataforma (@ de um criador, ID de playlist, etc.)
- Nenhuma (conteúdos como cadernos físicos, jogos sem fonte digital associada)

O sistema de arquivos do computador onde o sistema está rodando é uma fonte de conteúdo multimídia com o mesmo status de qualquer outra fonte — inclusive com a mesma possibilidade de ficar indisponível.

A lógica de fontes com prioridade e fallback se aplica a **todos os tipos de conteúdo**, não apenas áudio.

### 3.4. Acompanhamento (Progresso)

O progresso é **global**: pertence ao conteúdo, não à coletânea. Se um conteúdo está em duas coletâneas e o usuário consome 50%, ambas refletem 50%. Conteúdos não precisam estar em coletâneas para ter progresso.

**Estrutura do acompanhamento:**

- Estado: não iniciado, em andamento, concluído
- Posição atual: depende do tipo de conteúdo (timestamp para vídeo/áudio, posição/página para texto, campo de texto livre para tipos sem progresso estruturado como jogos)
- Histórico de consumo: lista de datas de consumo com porcentagem consumida em cada data, quando aplicável
- Campo de texto para progresso manual: para tipos de conteúdo onde a natureza do progresso depende inteiramente do gênero, subgênero ou título específico (ex: jogos onde progresso pode ser fases, pontos de mapa, ou algo completamente próprio)

**Marcação de progresso:**

- O usuário sempre pode marcar e editar o progresso manualmente.
- Quando o conteúdo é consumido pelo reprodutor interno do sistema, o sistema oferece a opção de marcar automaticamente o progresso.
- O usuário pode marcar conteúdos concluídos como não concluídos.

### 3.5. Categorias

Categorias são **tags livres** criadas pelo usuário. Não há taxonomia fixa nem hierarquia.

**Comportamento de autocompletar:** Conforme o usuário digita uma nova categoria, o sistema apresenta categorias existentes que contenham o texto digitado. Se o usuário seleciona uma já existente, o sistema utiliza o registro existente sem duplicar.

### 3.6. Conteúdos Relacionados

Um conteúdo pode ser vinculado a outros conteúdos com um **tipo de relação** textual (ex: "é sequência de", "é baseado em", "mesmo universo").

**Bidirecionalidade:** Se o Conteúdo A registra uma relação com o Conteúdo B:
- Na tela de dados do Conteúdo A, o Conteúdo B aparece na seção "Conteúdos relacionados".
- Na tela de dados do Conteúdo B, o Conteúdo A aparece na seção "Citado por" (ou equivalente).
- Ambas as seções são agrupadas por tipo de relação.

**Comportamento dos tipos de relação:** Mesmo comportamento das categorias — autocompletar com sugestões existentes, sem duplicação de registros.

### 3.7. Imagens do Conteúdo

Um conteúdo pode ter zero ou mais imagens, sendo uma designada como principal quando houver ao menos uma.

**Origem das imagens (configurável pelo usuário em suas preferências, com prioridade quando mais de uma estiver selecionada):**

- Busca automática na fonte do conteúdo (thumbnail do YouTube, capa do livro, etc.)
- Busca por nome em fontes de metadados cadastradas pelo usuário
- Seleção manual de arquivos no sistema de arquivos do PC

---

## 4. Funcionalidades Principais

### 4.1. Dashboard

A tela inicial do sistema é determinada pelo usuário. A apresentação padrão é análoga a uma tela de streaming com seções distintas:

- **Continuar:** Conteúdos iniciados mas não concluídos, com gancho para reproduzir de onde parou e opção de reiniciar do início.
- **Coletâneas — Conteúdos novos / não iniciados**
- **Coletâneas — Não vistos aleatórios**
- **Coletâneas — Conteúdos de miscelâneas:** Com opções de iniciar reprodução da miscelânea onde o conteúdo está contido (com gancho para o conteúdo), iniciar a miscelânea desde o início, ou reprodução isolada sem vínculo à miscelânea.
- Filtros, ordenação, categorias.

O usuário escolhe o que quer ver ao abrir o sistema. As seções acima são o padrão, não uma imposição.

### 4.2. Agregador de Conteúdo

O coração do sistema. Substitui o feed das redes sociais por um ambiente controlado.

**O que o agregador faz:**
- Monta feeds sob demanda dos criadores/fontes que o usuário segue.
- Apresenta conteúdos em cards com imagem, título e breve descrição quando possível.
- Quando a reprodução interna é possível, o card leva ao reprodutor.
- Quando apenas reprodução externa é possível, redireciona para o conteúdo externo, verificando se é possível usar gancho para o progresso existente.
- Quando apenas um link é possível, apresenta o link.

**O que o agregador NÃO faz (por design):**
- Não usa scroll infinito.
- Não recomenda conteúdos automaticamente.
- Não exibe métricas sociais (likes, comentários de terceiros, contadores de visualização).
- Não usa algoritmo de ranqueamento.
- Não puxa o usuário para um próximo conteúdo automaticamente.

**Filtros disponíveis ao usuário:**
- Por criador/fonte.
- Esconder conteúdos já assistidos/consumidos.
- Esconder conteúdos cujo título ou descrição contenham palavras-chave definidas pelo usuário.
- Ordenação cronológica (mais recente primeiro, por padrão).

**Cadastro de fontes:** O usuário cadastra fontes de formas diferentes conforme a plataforma:
- YouTube: URL do canal ou playlist.
- RSS/sites: URL do feed.
- Instagram: @ do criador.
- Outros: conforme aplicável à plataforma.

O sistema também deve permitir, quando as plataformas externas permitirem, que o usuário busque por dentro do sistema nas plataformas externas.

### 4.3. Reprodutor

O reprodutor consome conteúdo internamente quando tecnicamente viável. Deve suportar:

**Texto (post):**
- Texto puro, Markdown e HTML.
- Fonte (família, tamanho, cor, etc.) definida pelo usuário em configuração global.
- Respeita estilo quando intencionalmente aplicado ao texto pelo autor.

**Áudio:**
- Reprodução de arquivos locais.
- Fallback para fontes online (Spotify, YouTube, etc.) conforme prioridade configurada.

**Vídeo:**
- Embed de vídeos de plataformas que permitam (YouTube, etc.).
- Fallback para abertura externa.

**Abertura externa:**
- No aplicativo padrão do sistema operacional.
- Em aplicativo escolhido pelo usuário.
- Disponível para todos os tipos de conteúdo onde isso for pertinente.

**Ganchos (bookmarks dentro do conteúdo):**
- O usuário pode criar marcações em partes específicas do conteúdo sendo reproduzido.
- Exemplos: marcação de tempo em vídeos/áudios, links para posições específicas em textos, referência a elementos específicos de coletâneas.
- Ganchos podem ser usados em coletâneas para indicar "assista a partir do minuto 12:30".
- O progresso é sempre editável manualmente pelo usuário.
- Quando o conteúdo é consumido internamente, o sistema oferece marcação automática de progresso.

### 4.4. Busca e Filtragem

- A busca opera sobre conteúdos cadastrados no sistema.
- O sistema também permite buscar conteúdo novo nas plataformas externas que possibilitem isso, dentro das possibilidades do sistema.
- O usuário escolhe o escopo da busca (interno, externo, ambos) conforme as possibilidades oferecidas pelo sistema.

---

## 5. Regras de Negócio

### 5.1. Feed de Subscrição

- O feed é uma **visão**, não um dado. É montado sob demanda quando o usuário visualiza os itens da coletânea.
- O sistema verifica novidades de uma coletânea quando os itens dela são apresentados.
- Apenas itens com interação do usuário (progresso, anotação, nota, classificação) geram registros persistentes.
- Em estado offline: apenas itens com metadados salvos são apresentados. Conteúdos sem registro prévio não aparecem. Indicador de que a reprodução de conteúdos online pode não carregar.

### 5.2. Proteção contra Ciclos em Coletâneas

O sistema deve validar, no momento de adicionar um conteúdo a uma coletânea, que a adição não cria uma referência circular. Se a Coletânea A contém B, e B contém C, então C não pode conter A (nem B, nem qualquer coletânea que contenha A ou B na sua cadeia).

### 5.3. Progresso Global

O progresso pertence ao conteúdo, não ao contexto onde ele é exibido. Alterações no progresso de um conteúdo refletem em todos os lugares onde ele aparece.

### 5.4. Não Duplicação de Registros

Categorias e tipos de relação seguem o mesmo padrão: autocompletar com sugestões existentes durante a digitação, utilização do registro existente quando selecionado, sem criação de duplicatas.

### 5.5. Conteúdo sem Fonte

Um conteúdo pode existir no sistema sem nenhuma fonte associada. Nesses casos, o sistema atende à necessidade de centralizar, estruturar e relacionar informações sobre o conteúdo (anotações, progresso, categorias, relações, imagens) sem oferecer reprodução.

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

Fundamentação: O estudo de usabilidade em larga escala do Baymard Institute (publicado na Smashing Magazine com metodologia de teste com mais de 50 sites de e-commerce) encontrou que paginação é superior a scroll infinito para tarefas orientadas a objetivo, e que o botão "carregar mais" com paginação oferece a melhor experiência quando o usuário precisa examinar itens com atenção. O intervalo recomendado para resultados de busca é de 25 a 75 itens. Para navegação exploratória, 10 a 25 é razoável. O valor de 20 é um ponto de partida equilibrado.

### 6.3. Intervenções Ativas (Opcionais e Configuráveis)

O sistema pode oferecer intervenções ativas de bem-estar. Todas são **opcionais** e **desativáveis** pelo usuário. O princípio é que intervenções que ferem a autonomia percebida geram frustração e perdem eficácia.

**Intervenções sugeridas (baseadas na literatura):**

- **Monitoramento de tempo de uso:** Exibir tempo gasto no sistema na sessão atual. Fundamentação: Purohit e Holzer (2025, *Frontiers in Psychiatry*): rastreamento de tempo de tela como nudge comportamental demonstrou reduzir uso excessivo.
- **Lembrete de tempo configurável:** Notificação interna após período definido pelo usuário. Fundamentação: Roffarello e De Russis (2023, *ACM Trans. Comput.-Hum. Interact.*): lembretes e temporizadores são as estratégias mais comuns em ferramentas de autocontrole digital.
- **Relatório de consumo:** Resumo periódico de quanto o usuário consumiu (quantidade, tempo, tipos). Fundamentação: Olson et al. (2022, *Int. J. Ment. Health Addict.*): automonitoramento é componente central das intervenções com eficácia demonstrada.
- **Modo escala de cinza:** Opção de exibir a interface em escala de cinza. Fundamentação: Dekker e Baumgartner (2023) e ensaio randomizado de campo (Holte & Ferraro, 2020): modo escala de cinza reduz tempo de tela e uso problemático de smartphone.

### 6.4. Alertas de Configuração Potencialmente Não Saudável

Quando o usuário fizer configurações que se aproximem de padrões identificados como dark patterns ou uso não saudável, o sistema deve emitir um alerta informativo (não bloqueante). Exemplos:

- Configurar paginação com número muito alto de itens por página (ex: acima de 100), o que na prática se aproxima do comportamento de scroll infinito.
- Desativar todas as intervenções de bem-estar simultaneamente (alerta informativo, sem bloqueio).

O alerta informa, não impede. O usuário mantém a soberania.

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

## 9. Comportamento Offline

O sistema é offline-first. Sua operação principal não depende de internet.

**Quando online:**
- Fontes externas são acessíveis para reprodução e busca de metadados.
- Feeds de subscrição são montados com dados atualizados das fontes.

**Quando offline:**
- Conteúdos com fontes locais continuam totalmente acessíveis.
- Para conteúdos com fontes exclusivamente online: gerenciamento de informações (progresso, anotações, metadados) funciona normalmente. Reprodução pode não estar disponível.
- Feeds de subscrição apresentam apenas itens com metadados previamente salvos, com indicador claro de que a lista está incompleta e que a reprodução de conteúdos online pode falhar.
- Indicadores visuais claros de quais recursos estão indisponíveis.

---

## 10. Exportação e Importação

### 10.1. Dois Eixos

**Dados do usuário (role consumidor):** Qualquer usuário pode exportar seus dados pessoais (conteúdos, coletâneas, progresso, anotações, notas, configurações pessoais) para um arquivo ou conjunto de arquivos. Esse pacote pode ser importado em outra instância do sistema rodando em outro PC, possivelmente com sistema operacional diferente.

Cenário principal: um computador foi substituído e o usuário quer continuar usando o sistema na nova máquina.

**Configurações globais (role admin):** Usuários com role admin podem exportar as configurações globais do sistema para importar em outra instância.

### 10.2. Formato

O formato de exportação deve ser agnóstico de plataforma, legível e documentado. A containerização do sistema e a possibilidade de rodar em OSs diferentes reforçam essa necessidade.

---

## 11. Requisitos Não-Funcionais

### 11.1. Desempenho

O sistema deve ser otimizado para não degradar o desempenho do computador do usuário. Deve ser leve e eficiente em uso de recursos (CPU, memória, disco).

### 11.2. Resiliência e Recuperação de Erros

O sistema deve assumir que erros ocorrem e estar preparado para eles:

- Capacidade de recuperar estados anteriores.
- Mensagens de erro informativas que facilitem a identificação do problema por quem possa verificar.
- Continuidade do uso interrompido: se o sistema falhar durante uma operação, o usuário deve poder retomar de onde parou.

### 11.3. Segurança

O sistema deve ter proteção robusta:

- Proteção da integridade dos dados dos usuários.
- Proteção da integridade do sistema.
- Defesa contra ataques (considerando que o sistema roda localmente, o vetor de ataque é diferente de um serviço web, mas proteção é necessária mesmo assim).

### 11.4. Arquitetura

- Monolito modular: um único serviço, sem microsserviços.
- UI isolada: a camada de apresentação deve estar separada da lógica de negócio de forma que adicionar novos tipos de interface (web, API, etc.) no futuro seja prático.
- Containerização completa para o ciclo de desenvolvimento (CI/CD, testes, reprodutibilidade).
- Instalador que resolve todas as dependências, inclusive em cenários onde Docker não está disponível na máquina do usuário.

### 11.5. Idioma

O sistema, sua interface, mensagens, documentação e todo artefato produzido devem estar em **português brasileiro (pt-BR)**.

---

## 12. Referências Científicas

1. Gray, C. M., Santos, C., Bielova, N., Toth, M., & Clifford, D. (2024). An Ontology of Dark Patterns Knowledge. *Proceedings of the 2024 CHI Conference on Human Factors in Computing Systems*. ACM.
2. Mathur, A., Acar, G., Friedman, M. J., Lucherini, E., Mayer, J., Chetty, M., & Narayanan, A. (2019). Dark Patterns at Scale: Findings from a Crawl of 11K Shopping Websites. *Proceedings of the ACM on Human-Computer Interaction*, 3(CSCW), 1–32.
3. Vanden Abeele, M. M. P., Hendrickson, A. T., Pollmann, M. M. H., & Ling, R. (2023). Does mindless scrolling hamper well-being? Combining ESM and log-data. *Journal of Computer-Mediated Communication*, 29(1), zmad056. Oxford Academic.
4. Sharpe, B. T., & Spooner, R. A. (2025). Dopamine-scrolling: a modern public health challenge requiring urgent attention. *Journal of Public Health*. SAGE Publications.
5. Roffarello, A. M., & De Russis, L. (2023). Achieving Digital Wellbeing Through Digital Self-control Tools: A Systematic Review and Meta-analysis. *ACM Transactions on Computer-Human Interaction*, 30(4), 1–66.
6. Purohit, A. K., & Holzer, A. (2025). Active nudging towards digital well-being: reducing excessive screen time on mobile phones and potential improvement for sleep quality. *Frontiers in Psychiatry*, 16, 1602997.
7. Olson, J. A., Sandra, D. A., Colucci, É. S., Al Bikaii, A., Chmoulevitch, D., Nahas, J., Bhardwaj, A., & Bhatt, S. (2022). A Nudge-Based Intervention to Reduce Problematic Smartphone Use: Randomised Controlled Trial. *International Journal of Mental Health and Addiction*, 22, 1105–1130.
8. Peters, D., Ahmadpour, N., & Calvo, R. A. (2024). Designing for Sustained Motivation: A Review of Self-Determination Theory in Behaviour Change Technologies. *Interacting with Computers*, iwae040. Oxford Academic.
9. Wang, Y., Li, J., & Zhang, X. (2024). Inevitable challenges of autonomy: ethical concerns in personalized algorithmic decision-making. *Humanities and Social Sciences Communications*, 11, 1364. Nature.
10. Nie, X., Chen, S., Li, Z., et al. (2024). A Comprehensive Study on Dark Patterns. *Proceedings of the 2024 ACM International Conference on the Foundations of Software Engineering (FSE)*. ACM.
11. Kelly, D., & Rubin, V. L. (2024). Identifying Dark Patterns in User Account Disabling Interfaces: Content Analysis Results. *Social Media + Society*, 10(1). SAGE Journals.
12. Dekker, M. R., & Baumgartner, S. E. (2023). Digital Strategies for Screen Time Reduction: A Randomized Field Experiment. *Cyberpsychology, Behavior, and Social Networking*.
13. Kocyigit, E., Rossi, A., & Lenzini, G. (2024). A Systematic Approach for A Reliable Detection of Deceptive Design Patterns Through Measurable HCI Features. *Proceedings of the 2024 European Symposium on Usable Security (EuroUSEC)*. ACM.
14. Gray, C. M., Chamorro, L. S., Obi, I., & Duane, J.-N. (2023). Mapping the Landscape of Dark Patterns Scholarship: A Systematic Literature Review. *Companion Publication of the 2023 ACM Designing Interactive Systems Conference*. ACM.

---

## Glossário

| Termo | Definição |
|---|---|
| Agência | Capacidade do usuário de exercer controle efetivo sobre suas escolhas e ações dentro do sistema |
| Coletânea | Agrupamento de conteúdos, sendo ele mesmo um tipo de conteúdo |
| Conteúdo | Entidade central do sistema; qualquer coisa sobre a qual o usuário queira registrar e gerenciar informações |
| Dark pattern | Estratégia de design de interface que manipula o comportamento do usuário em benefício do provedor do serviço |
| Fonte | Origem de um conteúdo (URL, arquivo local, RSS, identificador de plataforma, ou nenhuma) |
| Gancho | Marcação em parte específica de um conteúdo que permite retornar àquele ponto |
| Nudge | Intervenção comportamental que altera a arquitetura de escolhas sem proibir opções |
| Progresso | Estado de acompanhamento do consumo de um conteúdo, pertencente ao conteúdo globalmente |
| Subscrição | Tipo de coletânea cujos itens são alimentados por fontes externas e montados sob demanda |
