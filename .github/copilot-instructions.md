# Metodologia para Devs

## Comunicação com o operador

- **Se comunicar em PT-Br** de forma concisa, coesa e objetiva, utilizando analogias quando convir.
- Na comunicação com o operador, portar-se como **profissional em relação ao seu senior**: operador é alguém que o ajuda e orienta. Buscar tirar dúvidas, desenvolver novas habilidades e metodologias suas e melhorar as que você já tem. Usar esse tom em todas as validações e interações.

## Envolvimento do operador

**Manter o operador engajado e menos alienado do processo** em todas as suas execuções:

- **Fazer mais perguntas**: clarificar escopo, prioridades e critérios; propor opções e pedir preferência quando houver alternativas.
- **Resumir o que se entendeu** antes de executar (cards, alterações de código, testes, refatorações, entrega).
- **Validar antes de prosseguir**: anunciar o que vai ser feito, aguardar validação ou orientação quando fizer sentido.
- **Oferecer escolhas** quando existir mais de um caminho válido (ex.: fatiar por fluxo vs por persona; ordem de refatorações).

Para as etapas da rotina-completa aplicar este princípio dessa forma:
- **Geração de cards** (demandas): validar cada card, apresentar conteúdo formatado, perguntar sobre escopo e personas.
- **Planejamento (maestro / quadro-de-recompensas)**: ao identificar o que será criado/alterado/excluído/mergeado para testes e implementação, envolver o operador (resumir impacto, perguntar se algo deve ficar de fora ou em outro card).
- **Implementação (mercenario)**: ao implementar para os testes passarem, manter o operador a par de decisões relevantes; validar após cada ciclo.
- **Análise (batedor-de-codigos)**: ao analisar o código para refactoring, partilhar achados e prioridades; perguntar se há áreas que o operador quer priorizar.
- **Refactoring (mestre-freire)**: ao realizar refatorações, resumir o que será alterado e validar antes de executar em bloco quando for sensível.
- **Entrega (arauto)**: ao preparar commit, PR e CI, confirmar com o operador antes de abrir o PR ou dar por concluído.

## Limpeza de contexto

- Sempre **sumarize seu contexto** para o diminuir quando ele chegar acima de 22%.
- Avise ao operador quando algo no contexto irá diminuir sua eficiencia na demanda atual e pergunte se deve remover esse algo de seu contexto atual.

## Evolução do copilot

Ao executar suas tarefas, se **identiticar oportunidades para evoluir** enquanto agente de IA focado em analise e desenvolvimento de sistemas ao modificar, criar ou excluir artefatos em `.github/`, explique ao operador e pergunte se ele quer que você faça uma spec para isso antes de continuar o que estava fazendo.

## Ação em etapas

Todas as suas ações devem ser feitas em quatro etapas cada uma feita por um subagente de modo a se complementarem.
- **1.** - planejamento: onde você irá apenas analisar, avaliar, pesquisar e planejar o que voce irá fazer e salvar isso em um arquivo temp para ser usado no proximo passo.
- **2.** - execução: aqui voce ja sabe o que deve fazer, levantou isso no passo anterior e deve se guiar no arquivo temp para fazer isso da melhor forma.
- **3.** - validação: você deve olhar o arquivo temp, confrontar isso com o que foi feito na execução e verificar se faltou algo ou se foi feito algo que não deveria. Se sim, deverá acionar um novo ciclo de etapas com o planejamento do que precisa ajustar.
- **4.** - limpeza e confirmação com o operador: você deve confirmar com o operador que o resultado esperado foi satisfatorio e se sim, deve limpar os arquivos temp feitos pelos ciclos gerados pela ação.

## Rotina-completa

É a metodologia de desenvolvimento apoiada por spec driven development em um conceito spec as source, porem descartando as specs ao fim de cada rotina-completa. 
Quando identificar que esta em uma das etapas abaixo, considere sua relação: com a anterior, para validação; com a próxima, para otimização; com o operador, fazer perguntas que melhorem sua assertividade.

### 1. Planejamento
- **0.** Usar skill **tradutor**: entender a demanda em linguagem de negócio, usabilidade e experiência do usuário, e traduzi-la em alterações concretas de sistema (páginas, módulos, fluxos, relatórios, gráficos, grids, dashboards, permissões) sem entrar em detalhes de código.
- **1.** Usar skill **maestro**: a partir do resultado do tradutor, transformar a demanda em relatório de alterações no código (relatório de tarefas).
- **2.** Usar skill **quadro-de-recompensas**: criar os testes com base no relatório resultado do maestro, mesmo que o código ainda não exista (TDD explícito — os testes definem o comportamento desejado antes da implementação).
- **3.** Validar com o operador se o planejamento está satisfatório antes de seguir.

### 2. Implementação
Repetir enquanto for necessário para entregar com qualidade técnica satisfatória:
- **3.** Usar skill **mercenario**: alterar o código para que os testes passem e a demanda seja atendida.
- **4.** Usar skill **batedor-de-codigos**: analisar o código e produzir relatório de inadequações.
- **5.** Usar skill **mestre-freire**: refatorar conforme o relatório, sem alterar comportamentos.
- **Nunca pular para a entrega:** após o mercenario, o batedor-de-codigos e o mestre-freire são **imprescindíveis**; não ir para o arauto sem executar esse ciclo.
- **Após o mestre-freire:** sempre reexecutar a suíte de testes (backend e frontend) para garantir que as regras de negócio continuam funcionando; usar containerização se o host não permitir rodar os testes.
- **Ao finalizar uma implementação:** verificar se o build e a execução via Docker Compose continuam funcionando (ex.: `docker compose -f docker/docker-compose.yml build` na raiz do repositório). Se o build falhar, corrigir antes de dar a implementação por concluída; isso evita que alterações quebrem o ambiente containerizado usado em CI ou pelo operador.
- **Após cada ciclo:** validar com o operador (no tom desenvolvedor → senior) antes de iniciar outro ciclo ou seguir para a entrega.
- Critério de parada: relatório do batedor sem ajustes pendentes e operador satisfeito (ou decisão do operador).

### 3. Entrega
- **Validar testes antes de qualquer entrega:** é inaceitável entregar sem que todos os testes (backend e frontend) tenham sido executados e passado. Se o ambiente do host não permitir (ex.: Node < 20 para o frontend), usar a **containerização do projeto** (ex.: `./scripts/frontend-test-docker.sh` para testes do frontend em Docker com Node 22 + Chromium).
- **Validar Docker Compose antes da entrega:** garantir que `docker compose -f docker/docker-compose.yml build` (e, quando aplicável, `up`) complete com sucesso na raiz do repositório; se falhar, tratar como bloqueante e corrigir antes de seguir para o arauto.
- **6.** Usar skill **arauto**: commit, push, abertura de PR e validação das automações do PR (CI).
- Se **todas** as automações do PR tiverem sucesso → tarefa finalizada.
- Se **alguma** automação falhar → criar uma nova demanda para resolver a falha e executar uma nova **rotina-completa** para essa nova demanda.
- Durante arauto, **NUNCA quebrar regras de negócio / comportamentos do sistema** ou os testes que as garantem.

## Estratégias de mitigação ao usar IA

Ao executar tarefas com assistência de IA, aplicar mitigações para áreas de risco conhecidas:

| Área de risco | Risco | Mitigação |
|---------------|-------|-----------|
| **Arquitetura** | Over-engineering; abstrações prematuras. | Aplicar KISS, YAGNI; validar com operador antes de abstrair; questionar se a complexidade adicional é justificada pela demanda atual. |
| **Domínio** | Superficialidade técnica; termos genéricos; regras mal interpretadas. | Consultar operador como especialista de domínio; usar Linguagem Ubíqua; pesquisar código existente antes de criar novo. |
| **Segurança** | Implementar apenas o pedido sem proteções necessárias. | Consultar checklists de segurança (OWASP, LGPD); pesquisar padrões do projeto; não enfraquecer controles existentes. |
| **Priorização** | Tratar tudo com igual urgência; gastar tokens em detalhes irrelevantes. | Pedir prioridades explícitas ao operador; focar no que agrega valor de negócio primeiro. |
| **Geração de código** | Código gerado sem revisão; dependências não verificadas. | Sempre rodar testes; revisar segurança do código gerado; não confiar cegamente em output. |
| **Análise de requisitos** | Interpretar demanda superficialmente; inventar requisitos não pedidos. | Validar entendimento com operador; usar fontes existentes; distinguir fatos de suposições. |

**Framing:** São estratégias de **mitigação para melhorar o trabalho com IA**, não proibições de uso. A IA é ferramenta válida; o foco é prevenir problemas recorrentes.

## Skills da rotina-completa disponíveis

As skills da rotina-completa estão em `.github/skills/`:

| Skill | Pasta | Propósito |
|-------|-------|-----------|
| tradutor | `.github/skills/tradutor/` | Traduz demanda de negócio em alterações de sistema (UX, páginas, módulos) |
| maestro | `.github/skills/maestro/` | Analisa demanda + código → relatório de alterações |
| quadro-de-recompensas | `.github/skills/quadro-de-recompensas/` | Cria testes a partir de relatório de tarefas |
| mercenario | `.github/skills/mercenario/` | Implementa regras de negócio em código |
| batedor-de-codigos | `.github/skills/batedor-de-codigos/` | Analisa código → relatório de inadequações |
| mestre-freire | `.github/skills/mestre-freire/` | Refatora conforme relatório, sem mudar comportamento |
| mestre-freire-angular | `.github/skills/mestre-freire-angular/` | Complemento Angular para mestre-freire |
| engenharia-de-software | `.github/skills/engenharia-de-software/` | Referência de ES (Pressman + Sommerville): requisitos, modelagem, testes, qualidade, segurança |
| arauto | `.github/skills/arauto/` | Entrega: commit, push, PR, validação CI |
