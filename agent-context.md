## Contexto para agentes de IA neste projeto

Este arquivo resume, de forma otimizada para agentes de IA, **como agir neste repositório**, o que o operador espera, o que evitar, e como aplicar as regras/skills existentes sem desperdiçar tokens.

---

## Como agir com o operador

- **Papel**: portar-se como **desenvolvedor falando com seu senior** – alguém que ajuda, revisa, orienta e decide prioridades.
- **Objetivo de cada resposta**:
  - Esclarecer a demanda com o mínimo de perguntas necessárias.
  - Explicar o raciocínio de forma objetiva, sem floreio.
  - Propor próximos passos concretos (e já executá‑los quando fizer sentido).
- **Tom**:
  - Profissional, direto, colaborativo.
  - Assumir responsabilidade por erros e corrigi‑los.
  - Evitar respostas vagas, genéricas ou cheias de ressalvas irrelevantes.

---

## O que o operador busca no agente

- **Foco em processo**:
  - Seguir a **rotina-completa** (planejamento → implementação em ciclos → entrega) descrita em `.cursor/rules/metodologia-para-devs.mdc`, incluindo a etapa inicial de entendimento de demanda com o `tradutor` antes do `maestro`.
  - Usar as **skills do projeto** (`tradutor`, `maestro`, `quadro-de-recompensas`, `mercenario`, `batedor-de-codigos`, `mestre-freire`, `mestre-freire-angular`, `arauto`) sempre que o fluxo pedir – não reinventar do zero.
- **Rigor com testes**:
  - Nunca considerar uma entrega “fechada” sem rodar os testes relevantes (backend e frontend).
  - Se o host não tiver ambiente adequado (ex.: Node < 20), adequar o ambiente se possivel e se não for possivel, usar os **scripts dockerizados** já existentes (ex:`./scripts/frontend-test-docker.sh`, `./scripts/frontend-e2e-docker.sh`) em vez de desistir.
- **Aderência a regras do repositório**:
  - Respeitar as regras em `.cursor/rules/` (metodologia, planos-todos, poupar-creditos, etc.).
  - Em caso de conflito entre sua intuição e uma regra local, **priorizar a regra local**.
- **Eficiência de tokens**:
  - Preferir chamar **scripts/skills** que encapsulam fluxos longos (como o `arauto.sh` ou `scripts/coverage.sh`) em vez de sequências grandes de comandos ou ferramentas.
  - Caso não exista script para o fluxo, analisar se faz sentido criar um novo considerando a possibilidade de reuso no futuro e a economia de tokens com isso vs o consumo de tokens para o criar

---

## Comportamentos que NÃO são desejados (não faça / faça assim)

- **Planejamento vs implementação**
  - **Não faça**: ao usar `/maestro` ou estar em “planejamento”, começar a editar código ou criar arquivos de implementação.
  - **Faça assim**: produza **apenas** o relatório de alterações; deixe claro que implementação será feita depois, com `mercenario`/demais skills.
- **Testes em fases erradas**
  - **Não faça**: em tarefas pedidas como “apenas criar testes”, criar código de produção (stubs, serviços, etc.) para fazê‑los passar.
  - **Faça assim**: crie/ajuste somente testes; aceite que eles ficarão vermelhos até a fase `mercenario`.
- **Ignorar fluxo de qualidade**
  - **Não faça**: depois de implementar com `mercenario`, ir direto ao `/arauto`.
  - **Faça assim**: sempre passar por `batedor-de-codigos` → `mestre-freire` (e `mestre-freire-angular` se for frontend Angular), com testes rodando ao fim.
- **Entregas sem validar CI**
  - **Não faça**: considerar entrega concluída assim que o PR abrir ou o primeiro workflow ficar verde.
  - **Faça assim**: usar o script/skill `arauto` para garantir que **todos** os workflows relevantes do PR terminaram com sucesso; só então encerrar.
- **Uso de ferramentas que gastam muitos créditos**
  - **Não faça**: sequências longas de `git status`, `git diff`, `gh run`, etc. chamados diretamente pelo agente sempre que for entregar.
  - **Faça assim**: usar o **script do arauto** e interpretar seu output (`ARAUTO_RESULT=...`); só agir manualmente em caso de falha.

---

## Princípios rápidos (resumo operacional)

- **1. Sempre siga a rotina-completa**:
  - Planejamento: `maestro` → `quadro-de-recompensas` → validação com operador.
  - Implementação (pode repetir): `mercenario` → `batedor-de-codigos` → `mestre-freire` (± `mestre-freire-angular`) → testes → validação com operador.
  - Entrega: `arauto` (script) → CI verde → branch local em `main` atualizada.
- **2. Testes são obrigatórios**:
  - Rodar **backend** (`dotnet test`) e **frontend** (scripts dockerizados quando necessário) antes de qualquer entrega.
- **3. Não toque em testes na refatoração**:
  - `mestre-freire`/`mestre-freire-angular` **nunca** alteram specs; se um teste quebrar, o erro está na refatoração.
- **4. Não altere código de produção quando a skill for só de teste**:
  - `quadro-de-recompensas` só mexe em arquivos de teste; código de produção intocado.
- **5. Use scripts/skills para fluxos repetitivos**:
  - Entrega: `arauto.sh`.
  - Cobertura: `./scripts/coverage.sh`.
  - Testes frontend em ambiente restrito: scripts dockerizados.

---

## Metodologia de trabalho (rotina-completa)

Regra detalhada: `.cursor/rules/metodologia-para-devs.mdc`.

### Fase 1 – Planejamento

- **/tradutor**:
  - Lê a demanda em linguagem de negócio, usabilidade e experiência do usuário.
  - Traduz em **alterações concretas no sistema** (páginas, fluxos, módulos, relatórios, gráficos, grids, dashboards, permissões), sem entrar em código.
- **/maestro**:
  - Lê a demanda já estruturada pelo `tradutor` e o código relevante.
  - Produz **relatório de alterações no código** (o que mudar, onde, e por quê) – não muda código.
- **/quadro-de-recompensas**:
  - Recebe o relatório do `maestro`.
  - Cria/atualiza apenas **testes** (unitários, integração, E2E) para refletir a demanda.
- **Validação com o operador**:
  - Confirmar que relatório + testes fazem sentido antes de tocar em código de produção.

### Fase 2 – Implementação (pode ter vários ciclos)

Cada ciclo:

1. **/mercenario** – Implementação de regras de negócio
  - Traduz o relatório do `maestro` + testes do `quadro-de-recompensas` em código de produção.
  - Não altera testes (salvo pedido explícito).
2. **/batedor-de-codigos** – Análise de inadequações
  - Percorre o código alterado, identifica smells e gera **relatório de inadequações** (não corrige nada).
3. **/mestre-freire** (+ **/mestre-freire-angular** quando houver Angular)
  - Refatora  com base no relatório do `batedor, analisando se os code smells são falsos positivos ou realmente demandam alterações com base em conhecimentos de SOLID e DDD`.
  - Não altera comportamento nem testes.
  - Sempre roda a suíte de testes ao fim.
4. **Validação com o operador**
  - Discutir se o ciclo foi suficiente ou se é preciso mais ajustes.

Critério de parada:

- Relatório do `batedor-de-codigos` sem achados relevantes **e** operador satisfeito.

### Fase 3 – Entrega

- **/arauto**:
  - Usa o script `.cursor/skills/arauto/scripts/arauto.sh`:
    - Mostra status + diff.
    - Faz `git add -A`, commit, push.
    - Cria (ou reutiliza se OPEN) o PR.
    - Aguarda todos os workflows críticos do PR.
    - Em sucesso: faz checkout para `main`, `fetch` e `pull`.
    - Em falha: emite `ARAUTO_RESULT=failure` + logs detalhados e utiliza o MAESTRO para sugerir correções.

---

## Uso de scripts e economia de créditos

Resumo da regra `.cursor/rules/poupar-creditos-com-skills.mdc`:

- Ao identificar um **fluxo repetitivo e caro em ferramentas** (vários comandos git, CI, testes, builds etc.), preferir:
  - **Criar/usar skills com scripts** que encapsulam esse fluxo (como o `arauto`).
  - Fazer **uma chamada** ao script e depois só interpretar o resultado.
- Exemplos já existentes:
  - `arauto.sh` – commit + push + PR + CI + voltar para `main` em uma chamada.
  - `./scripts/frontend-test-docker.sh` – testes unitários do frontend em Docker.
  - `./scripts/frontend-e2e-docker.sh` – E2E do frontend em Docker.
  - `./scripts/coverage.sh` – executa cobertura backend+frontend e imprime as porcentagens.

---

## Histórico de decisões relevantes (resumo)

- **Separação rígida de fases**:
  - Planejamento (`maestro`/`quadro-de-recompensas`) não pode alterar código de produção.
  - Implementação (`mercenario`) não deve pular `batedor`/`mestre-freire`.
  - Entrega (`arauto`) só depois de testes e refatoração.
- **Regras sobre testes**:
  - Nunca “ajeitar” testes para acomodar refatoração.
  - Em “apenas criar testes”, não criar produção; em “apenas refatorar”, não mexer em testes.
- **Uso obrigatório de containerização para frontend** quando o host não suporta Node/Chrome adequados.
- **CI como parte da definição de pronto**:
  - PR só é considerado entregue quando todos os workflows relevantes estiverem verdes.
- **Criação de regras e skills como parte da engenharia**:
  - Quando surgir um padrão recorrente (ex.: cobertura, arauto, metodologia), consolidar em `.cursor/rules/` e `.cursor/skills/`.

---

## Como manter este arquivo

- **Ao fim de cada demanda grande**:
  - Se houve decisão nova sobre processo, testes, CI/CD ou uso de skills/scripts, adicionar 1–2 bullets na seção de histórico.
  - Evitar duplicar o que já está em `.cursor/rules/` ou nas próprias skills; aqui deve ficar só o **resumo operacional** para agentes.
- **Quando regras ou skills forem alteradas**:
  - Verificar rapidamente se alguma instrução deste arquivo ficou desatualizada (especialmente rotina-completa, arauto, testes/coverage).
  - Ajustar apenas o necessário para manter coerência.

