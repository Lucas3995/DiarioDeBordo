# Referência detalhada – Engenharia de Software

Conteúdo complementar à skill principal. Fontes: Pressman & Maxim, *Engenharia de software: uma abordagem profissional*, 9ª ed.; Sommerville, *Engenharia de software*, 10ª ed. (Pearson Brasil). Nos tópicos em overlap, a skill expõe a síntese dialética (Seção 7).

---

## 1. Princípios completos por atividade

### Processo (Cap. 6)

1. Seja ágil: economia de ações; abordagem técnica simples; produtos concisos; decisões locais.
2. Concentre-se na qualidade em todas as etapas.
3. Esteja pronto para adaptações; processo não é dogma.
4. Monte equipe eficiente; organize-se; confiança e respeito mútuos.
5. Estabeleça mecanismos para comunicação e coordenação.
6. Gerencie mudanças (solicitar, avaliar, aprovar, implementar).
7. Avalie riscos; planos de contingência.
8. Gere artefatos que forneçam valor para outros; sem ambiguidades ou omissões.

### Prática (Cap. 6)

1. Dividir e conquistar / separação de interesses (SoC).
2. Compreender o uso da abstração; múltiplos níveis.
3. Esforçar-se pela consistência (modelos, código, testes, UI).
4. Concentrar-se na transferência de informações e nas interfaces.
5. Construir software com modularidade efetiva; módulos com aspecto bem restrito; interconexão simples.
6. Buscar padrões; catalogar e reutilizar soluções.
7. Representar problema e solução sob perspectivas diferentes (ex.: UML).
8. Lembrar que alguém fará a manutenção.

### Comunicação (Cap. 6)

1. Ouvir antes de comunicar; evitar ser contestador.
2. Preparar-se antes de se comunicar; pesquisar jargão; pauta.
3. Alguém deve facilitar a atividade.
4. Comunicar-se pessoalmente é melhor; usar representação (desenho, esboço).
5. Anotar e documentar decisões.
6. Esforçar-se para colaboração e consenso.
7. Manter foco; criar módulos para discussão.
8. Faltando clareza, desenhar.
9. De acordo ou não, seguir em frente quando apropriado.
10. Negociação ganha-ganha.

### Planejamento (Cap. 6)

1. Entender o escopo do projeto.
2. Incluir envolvidos no planejamento.
3. Reconhecer que planejamento é iterativo.
4. Fazer estimativas baseadas no que se conhece.
5. Considerar riscos ao definir o plano.
6. Ser realista (interrupções, mudanças, erros).
7. Ajustar granularidade (detalhe no curto prazo; mais amplo no longo prazo).
8. Definir como garantir qualidade (revisões, pares).
9. Descrever como acomodar alterações.
10. Verificar o plano com frequência e ajustar.

### Modelagem (Cap. 6)

1. Objetivo principal é construir software, não criar modelos.
2. Ser objetivo; não criar mais modelos do que precisa.
3. Produzir o modelo mais simples possível.
4. Construir modelos que facilitem alterações.
5. Estabelecer propósito claro para cada modelo.
6. Adaptar modelos ao sistema em questão.
7. Criar modelos úteis, não perfeitos.
8. Não ser dogmático quanto à sintaxe.
9. Confiar em instintos se o modelo parecer incorreto.
10. Obter feedback cedo.

### Construção – Codificação (Cap. 6)

**Preparação:** (1) Compreender o problema. (2) Compreender princípios de projeto. (3) Linguagem adequada. (4) Ambiente com ferramentas. (5) Conjunto de testes de unidade prontos.

**Codificação:** (6) Programação estruturada. (7) Considerar programação em pares. (8) Estruturas de dados adequadas. (9) Arquitetura e interfaces coerentes.

**Validação:** (10) Revisão de código quando apropriado. (11) Testes de unidade; corrigir erros. (12) Refatorar para melhorar qualidade.

### Testes (Myers)

- Teste é processo de executar programa com intenção de **encontrar erro**.
- Bom pacote de testes: alta probabilidade de encontrar erro ainda não descoberto.
- Teste bem-sucedido: aquele que **revela** um novo erro.

### Entrega (Cap. 6)

1. Estrutura de suporte antes da entrega.
2. Planejamento de testes muito antes de iniciar.
3. Princípio de Pareto: 80% dos erros em ~20% dos componentes; isolar componentes suspeitos.

---

## 2. Técnicas por fase

| Fase | Técnicas / ações |
|------|-------------------|
| **Comunicação/Requisitos** | Concepção, levantamento, elaboração, negociação, especificação, validação, gerenciamento; coleta colaborativa; cenários; casos de uso; identificação de envolvidos; NFRs; rastreabilidade. |
| **Análise** | Modelagem por cenários (atores, casos de uso); modelagem por classes (UML, CRC); modelagem funcional (diagramas de sequência); modelagem comportamental (diagramas de estados, de atividade); padrões de análise. |
| **Projeto arquitetura** | Contexto do sistema; arquétipos; refinamento em componentes; descrição de instâncias; revisão de arquitetura; revisão baseada em padrões; verificação de conformidade. |
| **Projeto componentes** | Coesão; acoplamento; diretrizes de projeto; componentes WebApp/móvel/tradicional; refatoração de componentes. |
| **Teste componente** | Caminho básico (caixa-branca); estrutura de controle; particionamento de equivalência (caixa-preta); análise de valor limite; teste de interface; teste orientado a objetos (conjunto, comportamental). |
| **Teste integração** | Integração descendente, ascendente, contínua; testes baseados em cenários e em falhas. |
| **Estimativa** | Dimensionamento (LOC, FP, processo, pontos de caso de uso); estimativa baseada em problema; harmonização; ágil (velocidade, histórias). |
| **Riscos** | Identificação; tabela de riscos; avaliação de impacto; RMMM (mitigação, monitoramento, gestão). |

---

## 3. Glossário mínimo

- **Ação de engenharia de software:** conjunto de tarefas que atinge um objetivo dentro de uma atividade metodológica.
- **Artefato:** produto de trabalho (programa, documento, dado) produzido por atividades e tarefas do processo.
- **Atividade metodológica:** uma das cinco atividades genéricas (comunicação, planejamento, modelagem, construção, entrega).
- **Conjunto de tarefas:** trabalho necessário para atingir o objetivo de uma ação.
- **Envolvido (stakeholder):** pessoa ou grupo com interesse no software.
- **FTR (revisão técnica formal):** reunião de revisão com papeis definidos, relatório e registros.
- **SQA:** garantia da qualidade de software (Software Quality Assurance).
- **SCM:** gestão de configuração de software (Software Configuration Management).
- **W5HH:** princípio de Boehm – Why, What, When, Who, Where, How, How much – para definição do projeto e do plano.
- **RMMM:** mitigação, monitoramento e gestão de riscos (Risk Mitigation, Monitoring, Management).

---

## 4. Mapeamento livro → tópico

| Parte | Capítulos | Tópicos |
|-------|-----------|---------|
| I – Processo | 1–5 | Software e disciplina; modelos de processo (cascata, prototipação, espiral, PU); agilidade; Scrum, XP, Kanban, DevOps; modelo recomendado; aspectos humanos. |
| II – Modelagem | 6–14 | Princípios que orientam a prática; requisitos; modelagem de requisitos (cenários, classes, funcional, comportamental); conceitos de projeto; arquitetura; componentes; UX/UI; mobilidade; padrões. |
| III – Qualidade e segurança | 15–23 | Conceitos de qualidade; revisões; SQA; engenharia de segurança (SQUARE, etc.); teste componente/integração/especializado; SCM; métricas. |
| IV – Gerenciamento | 24–27 | Conceitos de gerenciamento; W5HH; plano viável; estimativas; cronograma; riscos; suporte e manutenção. |
| V – Avançados | 28–30 | Melhoria de processo (CMMI, SPICE, etc.); tendências; comentários finais. |

---

## 5. Spec-driven development (detalhe)

- **Fonte única de verdade:** especificação (requisitos, contrato de API, esquema de dados) como artefato primeiro; código e testes derivados ou validados em relação a ela.
- **Contratos:** APIs (ex.: OpenAPI), interfaces, comportamento esperado; design-by-contract quando aplicável.
- **Rastreabilidade:** requisitos → spec → implementação → testes; manter links explícitos.
- **Specs verificáveis:** testes de contrato; validação de schemas; specs executáveis quando apropriado.

---

## 6. Leituras complementares / desambiguação

- **Padrão arquitetural vs design pattern:** padrão arquitetural descreve estrutura global (ex.: camadas, cliente-servidor); design pattern resolve problema de projeto em nível de componentes/classes (ex.: Strategy, Observer). Ambos usam forma contexto–problema–solução.
- **TDD (test-driven development):** abordagem em que testes são escritos antes do código; ciclo vermelho–verde–refatorar.
- **Segurança (safety) vs segurança da informação (security):** Em PT usar "segurança (safety)" para sistemas críticos e dano não intencional; "segurança da informação (security)" para proteção contra ataques intencionais (confidencialidade, integridade, disponibilidade). Técnicas distintas: hazard analysis/processos de safety no eixo safety; SQUARE, casos de mau uso, codificação segura no eixo security.
- **COCOMO II:** modelo algorítmico de custo (Boehm et al.); estimativa de esforço a partir de tamanho (LOC ou pontos de função), tipo de projeto, fatores de ajuste. Usado para estimativas iniciais e comparação com planejamento ágil.

---

## 7. Dialética tese/antítese/síntese (overlaps Pressman × Sommerville)

Onde as duas fontes tratam do mesmo assunto, a skill expõe a **síntese**; esta tabela documenta a gênese para consulta e aprofundamento.

| Tópico | Tese (Pressman) | Antítese (Sommerville) | Síntese (visão integrada) |
|--------|------------------|-------------------------|----------------------------|
| **Processo e agilidade** | Processo como atividades metodológicas (comunicação, planejamento, modelagem, construção, entrega); conjuntos de tarefas; agilidade (Scrum, XP, Kanban); adaptação do processo. | Processos como "lidar com mudanças" e "melhoria de processo"; agilidade com foco em gerenciamento e escalabilidade; processos confiáveis para software crítico. | Processo como estrutura adaptável em que a mudança é constitutiva; agilidade escalável com atenção explícita a dependabilidade e certificação quando o domínio for crítico; melhoria contínua do processo como parte do ciclo. |
| **Requisitos** | Concepção, levantamento, elaboração, negociação, especificação, validação; cenários e casos de uso; NFRs; rastreabilidade. | Dois níveis de especificação (requisitos de usuário vs requisitos de sistema); stakeholders e leitores por tipo de documento; mudança de requisitos como seção própria. | Engenharia de requisitos em dois níveis (usuário e sistema) com mapeamento explícito de envolvidos e leitores; NFRs e rastreabilidade; gestão de mudança de requisitos integrada ao fluxo. |
| **Modelagem e projeto** | Análise (informação, função, comportamento); projeto (arquitetura, componentes, interface); UML; padrões de análise e de projeto. | Modelos de contexto, interação, estruturais e comportamentais; visões de arquitetura; MDD; projeto OO e padrões de projeto. | Múltiplas vistas (contexto, interação, estrutura, comportamento) alinhadas a visões arquiteturais; MDD quando reduzir inconsistência e custo de mudança; padrões de projeto como vocabulário comum (contexto–problema–solução). |
| **Teste** | V&V; caixa-branca/caixa-preta; integração (top-down, bottom-up, contínua); Myers (teste para encontrar erros). | Teste de desenvolvimento, TDD, teste de lançamento, teste de usuário (alfa, beta, aceitação); ambiente de trabalho do usuário como fator. | Teste como continuum (unidade → integração → lançamento → usuário); TDD como prática de construção; validação em ambiente real/usuário como necessária; objetivo de encontrar erros (Myers) em todos os níveis. |
| **Qualidade e SQA** | Fatores de qualidade; SQA; revisões (informais, formais, FTR); custo da qualidade. | Dependabilidade como guarda-chuva (disponibilidade, confiabilidade, safety, security); métricas (ROCOF, MTTF, disponibilidade); gestão da qualidade e ágil; padrões e medição. | Qualidade operacionalizada como dependabilidade (propriedades mensuráveis) + processo (revisões, padrões, SQA); revisões e inspeções adaptadas ao contexto (plano vs ágil); métricas de confiabilidade quando o sistema for crítico ou dependável. |
| **Safety e security** | Ciclo de vida de segurança; SQUARE; casos de mau uso; análise de risco; codificação segura. | Separação explícita: safety = sistemas críticos, dano não intencional; security = ataques intencionais, C-I-A; engenharia de resiliência. | Duas dimensões distintas (safety vs security) com vocabulário e técnicas próprias; SQUARE/casos de mau uso no eixo security; hazard analysis e processos de safety no eixo safety; resiliência (antecipar, responder, aprender) como ponte organizacional e técnica. |
| **Evolução e manutenção** | Tipos de manutenção; refatoração (dados, código, arquitetura); engenharia reversa/direta. | Processos de evolução; sistemas legados como categoria explícita; operação e evolução; custos de manutenção. | Evolução como fase contínua do ciclo de vida; legado e operação como preocupações de primeira classe; refatoração e reengenharia como instrumentos dentro desse processo; estimativa de custos de manutenção no planejamento. |
| **Gestão de projeto** | W5HH; escopo; estimativas (LOC, FP, pontos de caso de uso); cronograma (Gantt); riscos (tabela, RMMM). | Riscos, pessoas e trabalho em equipe; planejamento ágil (histórias, velocidade); COCOMO II; precificação; qualidade e configuração no plano. | Planejamento como diálogo entre abordagem algorítmica (COCOMO, FP, LOC) e ágil (velocidade, histórias); W5HH e RMMM com dimensões pessoas e equipe explícitas; riscos contínuos; qualidade e configuração integradas ao plano. |
| **Configuração (SCM)** | Itens de configuração; controle de versão; controle de alterações; integração contínua; auditoria. | Gerenciamento de versões (incl. distribuído); construção de sistemas; gerenciamento de mudanças; gerenciamento de lançamentos. | SCM unificado: versão (central e distribuída, e.g. Git), construção, controle de mudança e lançamentos; integração contínua e auditoria como práticas de garantia. |

---

## 8. Mapeamento Sommerville 10e (partes e capítulos)

| Parte | Capítulos | Tópicos (uma linha por cap.) |
|-------|-----------|------------------------------|
| 1 – Introdução | 1 | Introdução; desenvolvimento profissional; ética; estudos de caso. |
| | 2 | Processos de software; modelos; atividades; lidar com mudanças; melhoria de processo. |
| | 3 | Desenvolvimento ágil; métodos ágeis; técnicas; gerenciamento ágil; escalabilidade. |
| | 4 | Engenharia de requisitos; funcionais e não funcionais; processos; elicitação; especificação; validação; mudança. |
| | 5 | Modelagem de sistemas; contexto; interação; estruturais; comportamentais; MDD. |
| | 6 | Projeto de arquitetura; decisões; visões; padrões de arquitetura; arquiteturas de aplicações. |
| | 7 | Projeto e implementação; OO/UML; padrões de projeto; implementação; open source. |
| | 8 | Teste de software; teste de desenvolvimento; TDD; teste de lançamento; teste de usuário. |
| | 9 | Evolução de software; processos de evolução; sistemas legados; manutenção. |
| 2 – Dependabilidade e segurança | 10 | Dependabilidade de sistemas; propriedades; sociotécnicos; redundância e diversidade; processos confiáveis; métodos formais. |
| | 11 | Engenharia de confiabilidade; disponibilidade e confiabilidade; requisitos; arquiteturas tolerantes a defeitos; programação; medição. |
| | 12 | Engenharia de segurança (safety); sistemas críticos; requisitos; processos; casos de segurança. |
| | 13 | Engenharia de segurança da informação (security); dependabilidade; organizações; requisitos; projeto seguro; teste e garantia. |
| | 14 | Engenharia de resiliência; cibersegurança; resiliência sociotécnica; projeto de sistemas resilientes. |
| 3 – Avançada | 15 | Reúso de software; panorama; frameworks; linhas de produto; reúso de sistemas. |
| | 16 | Engenharia baseada em componentes; componentes e modelos; processos; composição. |
| | 17 | Engenharia distribuída; sistemas distribuídos; cliente-servidor; padrões; SaaS. |
| | 18 | Orientada a serviços; SOA; RESTful; engenharia de serviços; composição. |
| | 19 | Engenharia de sistemas; sociotécnicos; projeto conceitual; aquisição; desenvolvimento; operação e evolução. |
| | 20 | Sistemas de sistemas; complexidade; classificação; reducionismo; engenharia e arquitetura. |
| | 21 | Software de tempo real; sistemas embarcados; padrões; análise temporal; RTOS. |
| 4 – Gerenciamento | 22 | Gerenciamento de projetos; riscos; pessoas; trabalho em equipe. |
| | 23 | Planejamento; precificação; dirigido por plano; cronograma; planejamento ágil; estimativa; COCOMO. |
| | 24 | Gerenciamento da qualidade; qualidade de software; padrões; revisões e inspeções; ágil; medição. |
| | 25 | Gerenciamento de configuração; versões; construção; mudanças; lançamentos. |

Para resumo por parte, termos-chave por capítulo e trechos de princípios/definições do Sommerville 10e, ver [sommerville-reference.md](sommerville-reference.md).

---

## 9. Glossário unificado (Pressman + Sommerville)

**Da Seção 3:** ação de engenharia de software, artefato, atividade metodológica, conjunto de tarefas, envolvido (stakeholder), FTR, SQA, SCM, W5HH, RMMM.

**Acréscimos Sommerville / desambiguação:**

- **Dependabilidade:** propriedade que agrega disponibilidade, confiabilidade, segurança (safety) e segurança da informação (security); custo cresce com o nível de dependabilidade exigido.
- **Confiabilidade:** probabilidade de o sistema operar sem falha em dado contexto; métricas: ROCOF (taxa de ocorrência de falhas), MTTF (tempo médio até falha), disponibilidade (AVAIL).
- **Disponibilidade (AVAIL):** probabilidade de o sistema estar operacional quando demandado.
- **ROCOF:** rate of occurrence of failures – número esperado de falhas por unidade de tempo ou por execuções.
- **MTTF:** mean time to failure – tempo médio entre falhas observadas.
- **Segurança (safety):** ausência de dano não intencional a pessoas, bens ou ambiente; sistemas críticos em segurança.
- **Segurança da informação (security):** proteção contra ataques intencionais; confidencialidade, integridade, disponibilidade (C-I-A).
- **Sistema sociotécnico:** sistema que integra hardware, software e pessoas/organização; propriedades emergentes; requisitos e projeto influenciados por fatores humanos e organizacionais.
- **Resiliência (4 Rs):** reconhecimento (do problema), resistência, recuperação, restabelecimento; no nível organizacional: antecipar, responder, aprender.
- **Defeito / erro / falha:** defeito no sistema → erro de sistema (estado incorreto) → falha (manifestação observada pelo usuário).
- **Stakeholder:** ver envolvido (termo preferido na skill).

---

## 10. Código Legado e Evolução (Feathers)

Complemento à seção 7 (Evolução e manutenção) com abordagem operacional para código legado.

**Definição operacional:** Legacy code = código sem testes. Não importa idade, qualidade ou tecnologia. Sem testes, qualquer mudança é arriscada.

### Dois modos de trabalho

| Modo | Descrição | Risco |
|------|-----------|-------|
| **Edit and Pray** | Alterar sem testes; esperar que funcione | Alto — sem feedback; regressões silenciosas |
| **Cover and Modify** | Cobrir com testes antes de alterar; modificar com rede de segurança | Baixo — feedback rápido; regressões detectadas |

### Legacy Code Change Algorithm (5 passos)

1. **Identificar pontos de mudança** — onde no código a mudança precisa acontecer
2. **Encontrar pontos de teste** — onde posso escrever testes que cubram a mudança
3. **Quebrar dependências** — tornar código testável (25 técnicas no catálogo)
4. **Escrever characterization tests** — documentar comportamento existente
5. **Fazer mudança + refatorar** — com rede de segurança dos testes

### Modelo de Seams

Seam = ponto onde comportamento pode ser alterado sem editar o código no ponto.

| Tipo | Mecanismo | Preferência |
|------|-----------|-------------|
| **Object Seam** | Polimorfismo, override, DI | ✅ Preferido em OO |
| **Link Seam** | Substituição em link-time | Quando object seam impossível |
| **Preprocessing Seam** | Macros, #define | Último recurso |

### Relação com Evolução (Sommerville Cap. 9)

- Sommerville trata evolução como **processo organizacional** (sistemas legados, custos de manutenção)
- Feathers complementa com **técnica operacional** — como efetivamente alterar código legado no dia a dia
- **Síntese:** planejamento de evolução (Sommerville) + execução segura (Feathers) = evolução sustentável

**Skill dedicada:** Ver [codigo-legado](../codigo-legado/SKILL.md) para workflow completo e catálogo de 25 técnicas.

---

## 11. Rigor e Determinismo (Knuth — referência filosófica)

Complemento filosófico à engenharia de software. Fonte: *The Art of Computer Programming* (Donald Knuth).

### Princípios

- **Análise de correção** complementa testes — testes mostram presença de bugs, análise formal pode mostrar ausência (em escopo limitado)
- **Teste ≠ Prova:** testes verificam amostras; provas verificam todos os casos. Ambos são necessários; nenhum é suficiente sozinho
- **Complexidade graduada:** problemas devem ser abordados em níveis crescentes de dificuldade (Knuth usa exercícios graduados 1-50; equivalente em software: do unit test mais simples ao cenário mais complexo)
- **Literate Programming:** código deve ser legível como narrativa — "explicar para humanos o que queremos que o computador faça". Specs e skills devem seguir este princípio

### Aplicação prática no toolkit

| Princípio Knuth | Onde se aplica |
|-----------------|----------------|
| Complexidade graduada | Test List Management (Beck) — começar pelo mais simples |
| Literate Programming | SKILL.md como narrativa, não apenas lista de instruções |
| Análise de correção | Complementar TDD em caminhos críticos (ex: algoritmos financeiros) |
| Cross-referencing | Skills referenciam umas às outras (§N → §M) explicitamente |

---

## 12. Rigor Científico em Especificação (Wazlawick)

Complemento à seção 5 (Spec-driven development) com framework de rigor metodológico. Fonte: Wazlawick, *Metodologia de Pesquisa para Ciência da Computação*, 2ª ed.

### 5 Níveis de Maturidade de Especificação

Mapeamento dos níveis de maturidade de pesquisa (Wazlawick Cap. 2) para qualidade de specs em desenvolvimento de software.

| Nível | Pesquisa (Wazlawick) | Spec (equivalente) | Critério |
|-------|---------------------|---------------------|----------|
| **L1** | Apresentação de produto | Spec vaga — descreve o que será feito sem critérios verificáveis | Sem métricas, sem hipótese, sem baseline |
| **L2** | Survey / compilação | Spec baseada em análise de existentes — "fizemos como X faz" | Referência a soluções existentes, sem comparação mensurável |
| **L3** | Presumivelmente melhor | Spec com hipótese plausível — "acreditamos que será melhor" | Hipótese formulada, métricas definidas, sem evidência empírica |
| **L4** | Reconhecidamente melhor | Spec com evidência — "medimos e é melhor por X%" | Baseline + medição + comparação; resultado reproduzível |
| **L5** | Prova formal | Spec irrefutável — prova lógica/formal da correção | Demonstração formal; aplicável a algoritmos, protocolos, invariantes |

**Regra:** Toda spec deve declarar seu nível. O nível mínimo aceitável para entrega é **L3** (hipótese formulada + métricas definidas). L4 é o alvo para features de valor de negócio.

### 7 Regras Operacionais para Spec-Driven Development

1. **Definições operacionais obrigatórias** — Todo critério de aceite deve ter definição operacional (como medir), não apenas constitutiva (o que é).
   - ❌ Constitutiva: "O sistema deve ser fácil de usar"
   - ✅ Operacional: "O usuário completa o fluxo de cadastro em ≤ 5 cliques e ≤ 30 segundos"

2. **Specs orientadas por hipótese** — Toda spec expressa como: "Acredito que [X]; o mecanismo é [Y]; saberemos que funciona quando [Z]".
   - Hipótese é especulativa (pode estar errada), testável (pode ser verificada) e fundamentada (tem justificativa).

3. **Baselines obrigatórias para melhoria** — Toda alegação de melhoria requer baseline medida antes da alteração. Sem baseline, não há como verificar melhoria.

4. **Mapeamento de variáveis para cada critério:**
   - **Variável independente** — o que manipulamos (ex.: algoritmo de ordenação)
   - **Variável dependente** — o que medimos (ex.: tempo de resposta)
   - **Variável medida** — observada sem controle (ex.: carga real do sistema)
   - **Variável manipulada** — controlada pelo experimentador (ex.: tamanho do dataset)

5. **Limitações de escopo explícitas** — Cada spec declara o que NÃO será feito/verificado. Evita expansão de escopo não intencional.

6. **Justificativa com evidência** — Cada feature justificada com evidência (dados, feedback, métricas), não apenas intuição.

7. **Nível de maturidade reconhecido** — Cada spec declara explicitamente em que nível (L1-L5) se encontra.

### Taxonomia de Erros Comuns em Especificação

Erros recorrentes mapeados de Wazlawick Cap. 4 para contexto de desenvolvimento de software.

| Erro | Descrição | Exemplo em SW | Correção |
|------|-----------|---------------|----------|
| **Síndrome da Intersecção Esquecida** | Revisar domínio e ferramenta separadamente, ignorar a interseção | Estudar DDD e Angular sem analisar como DDD se aplica ao Angular | Focar na interseção: como conceitos se aplicam ao contexto específico |
| **Generalização de problema local** | Tratar problema específico como universal | "Toda API precisa de GraphQL" a partir de um caso | Delimitar escopo; não generalizar de caso único |
| **Seleção a priori de ferramenta** | Escolher solução antes de entender o problema | "Vamos usar MongoDB" antes de analisar o modelo de dados | Problema primeiro; ferramenta depois |
| **Métrica não operacionalizada** | Critério de aceite subjetivo, não mensurável | "Deve ser rápido", "fácil de usar" | Operacionalizar: tempo ≤ X, cliques ≤ Y |
| **Verbos triviais vs acionáveis** | Usar verbos vagos que não definem resultado verificável | "Propor", "estudar", "analisar" | Usar: "demonstrar", "provar", "medir", "reduzir para X" |
| **Objetivo = Tema** | Confundir objetivo com tópico geral | "O objetivo é performance" | Objetivo = verbo + objeto + critério: "Reduzir tempo de resposta do endpoint X para ≤ 200ms" |
| **Justificativa do tema, não da hipótese** | Justificar por que o assunto é importante, não por que a solução proposta funcionará | "Performance é importante" sem explicar por que a abordagem escolhida vai funcionar | Justificar o mecanismo: "Cache em memória reduzirá latência porque elimina I/O de disco" |
| **Ausência de limitações** | Não declarar o que está fora do escopo | Scope creep; expectativas não gerenciadas | Seção explícita de limitações em cada spec |

### Revisão Sistemática como Modelo para Exploração de Codebase

A revisão sistemática (Wazlawick §3.2) mapeia-se para exploração de codebase antes de implementar:

| Etapa de Revisão | Equivalente em Codebase |
|-----------------|------------------------|
| Definir pergunta de pesquisa | Definir o que precisa ser alterado e por quê |
| Estabelecer critérios de inclusão/exclusão | Definir quais módulos/camadas são relevantes |
| Buscar em bases de dados | Buscar no código existente (grep, semantic search) |
| Selecionar estudos relevantes | Selecionar arquivos/componentes a analisar |
| Extrair dados | Mapear dependências, contratos, comportamentos existentes |
| Sintetizar resultados | Produzir relatório de impacto (maestro) |

### Referência Estatística para Métricas

Fórmulas de Wazlawick §3.4.6-8 como referência para métricas quantitativas em specs L4+.

- **Variância** $s^2 = \frac{1}{n-1} \sum_{i=1}^{n}(x_i - \bar{x})^2$ — dispersão dos valores medidos; alta variância indica inconsistência.
- **Covariância** $\text{cov}(X,Y) = \frac{1}{n-1} \sum_{i=1}^{n}(x_i - \bar{x})(y_i - \bar{y})$ — relação linear entre variáveis; positiva = variam juntas.
- **Correlação** $r = \frac{\text{cov}(X,Y)}{s_X \cdot s_Y}$ — covariância normalizada (-1 a +1); |r| ≥ 0.7 = correlação forte.

**Uso prático:** ao comparar alternativas (L4), calcular variância para garantir consistência das medições; correlação para verificar se a variável independente de fato influencia a dependente.

**Skill dedicada para spec-driven:** Ver skills [tradutor](../tradutor/SKILL.md), [maestro](../maestro/SKILL.md) e [quadro-de-recompensas](../quadro-de-recompensas/SKILL.md) que aplicam estes princípios nas etapas de planejamento.
