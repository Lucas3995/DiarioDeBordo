# Referência — Testes de Software (para criação de testes)

Resumo extraído do documento [Engenharia de ponte de requisitos](../../../Engenharia%20de%20ponte%20de%20requisitos) — **apenas** a secção **Testes de Software** (1.1–1.7). Uso: guiar a criação de testes a partir do relatório de tarefas; garantir rastreabilidade requisito → teste e boas práticas.

---

## Pirâmide de Testes

| Nível | Tipo | Quantidade | Uso |
|-------|------|------------|-----|
| **Base** | Unitários | Muitos (rápidos, baratos) | Priorizar; feedback rápido. |
| **Meio** | Integração | Menos (mais lentos) | APIs, persistência, serviços externos. |
| **Topo** | E2E / UI | Poucos (caros, lentos) | Fluxos críticos e smoke. |

**Objetivo:** Feedback rápido e cobertura econômica. E2E para fluxos críticos; unitários e integração para a maior parte do comportamento.

---

## Conceitos centrais

| Conceito | Definição | Uso para esta skill |
|----------|-----------|----------------------|
| **Caixa Preta** | Testar funcionalidade sem conhecimento da implementação; entradas e saídas conforme especificação. | Derivação de casos a partir do relatório de tarefas (requisitos e critérios de aceite). |
| **Caixa Branca** | Testar lógica interna, caminhos e cobertura de código. | Quando for necessário localizar a unidade ou API a testar; não para análise de qualidade. |
| **Critério de aceite** | Condição mensurável e observável que define quando um requisito está satisfeito. | Cada teste deve ter critério claro e binário (pass/fail). |
| **Regressão** | Garantir que alterações não quebraram comportamento já validado. | Suíte automatizada; executar após criar/alterar testes. |

---

## Técnicas

| Técnica | Descrição | Quando usar |
|---------|-----------|-------------|
| **Particionamento de equivalência** | Agrupar entradas em classes; um representante de cada classe cobre a classe. | Reduzir casos mantendo cobertura; entradas numéricas, faixas, categorias. |
| **Análise de valor limite** | Testar limites das partições (mínimo, máximo, logo abaixo/acima). | Defeitos frequentes nos limites (off-by-one, comparações). |
| **Testes baseados em requisitos** | Derivar casos diretamente de requisitos e critérios de aceite. | Rastreabilidade requisito → teste; validação de completude. |
| **Tabela de decisão** | Combinar condições e ações em matriz; cada regra vira um caso. | Lógica de negócio com múltiplas condições. |

TDD/BDD como referência de estilo: teste como especificação executável; nomes que descrevem comportamento.

---

## Padrões e boas práticas

- **AAA (Arrange-Act-Assert):** Preparação → ação → verificação; legibilidade e consistência.
- **Um conceito por teste:** Cada caso verifica um único comportamento ou condição; falhas localizadas.
- **Nomes descritivos:** Nome do teste descreve cenário e expectativa (ex.: `deve_rejeitar_senha_curta_quando_tamanho_menor_que_8`).
- **Testar comportamento, não implementação:** Assertivas sobre saídas e efeitos observáveis; evitar acoplamento a detalhes internos.
- **Testes independentes e repetíveis:** Sem ordem de execução; sem dependência de estado global ou ambiente frágil.
- **Dublês (mocks/stubs):** Isolar unidade sob teste; usar para dependências externas, I/O e tempo.

---

## FIRST

- **F**ast: testes rápidos para execução frequente.
- **I**ndependent: sem dependência entre testes.
- **R**epeatable: mesmo resultado em qualquer ambiente.
- **S**elf-Validating: pass/fail claro, sem inspeção manual.
- **T**imely: escritos no contexto da demanda (relatório de tarefas).

---

## Checklist rápido para criação de testes

- [ ] Requisito ou comportamento tem critério de aceite claro e verificável?
- [ ] Casos derivados do relatório de tarefas (caixa preta) e/ou cobertura relevante (caixa branca apenas para localizar alvo)?
- [ ] Testes FIRST e AAA; nome descritivo; um conceito por teste?
- [ ] Testes de regressão: suíte executada após alterações?

*(Esta skill não inclui atividades de DevSecOps nem SRE; foco em testes funcionais/estruturais derivados do relatório de tarefas.)*

---

## Ciclo TDD (Red-Green-Refactor)

| Fase | O que fazer | Critério de passagem |
|------|-------------|----------------------|
| **Red** | Escrever teste que falha — descreve comportamento desejado que ainda não existe. | Teste compila e falha por razão esperada (não por erro de sintaxe ou import). |
| **Green** | Escrever o código mínimo necessário para o teste passar. | Teste verde; sem otimizações nem refatorações ainda. |
| **Refactor** | Melhorar código sem alterar comportamento; executar testes novamente. | Todos os testes continuam verdes; código mais limpo. |

**3 Leis do TDD (Uncle Bob):**
1. Não escreva código de produção sem antes ter um teste falhando.
2. Escreva apenas o suficiente do teste para que ele falha.
3. Escreva apenas o suficiente de código de produção para o teste passar.

---

## Dublês de teste (expandido)

| Tipo | Definição | Quando usar |
|------|-----------|-------------|
| **Stub** | Fornece respostas predefinidas a chamadas; não verifica interações. | Isolar dependência de I/O; fornecer dados controlados ao SUT. |
| **Mock** | Verifica que interações esperadas ocorreram (métodos chamados, argumentos). | Validar que o SUT interage corretamente com colaboradores. |
| **Fake** | Implementação funcional simplificada (ex.: repositório in-memory). | Testes de integração leves; quando Stub fica complexo demais. |
| **Spy** | Registra chamadas para inspeção posterior; não substitui comportamento. | Verificar efeitos colaterais sem alterar fluxo. |
| **Dummy** | Objeto passado para satisfazer assinatura, nunca usado. | Preencher parâmetros obrigatórios que o teste não exercita. |

---

## Técnicas avançadas

| Técnica | Definição | Quando usar |
|---------|-----------|-------------|
| **Property-based testing** | Gera inputs aleatórios; verifica propriedades invariantes (ex.: "resultado sempre ≥ 0"). | Domínios com grande espaço de entradas; encontrar edge cases não previstos. |
| **Mutation testing** | Altera código de produção (mutações) e verifica se testes detectam. | Avaliar qualidade real da suíte; identificar testes fracos que passam mesmo com bugs. |
| **Data Builders** | Padrão Builder para criar objetos de teste com defaults; alterar apenas o relevante. | Reduzir boilerplate no Arrange; tornar setup legível e focado. |
| **Object Mother** | Fábrica de objetos de teste com cenários nomeados (ex.: `PedidoMother.completo()`). | Reusar setup complexo em vários testes; um lugar para alterar cenários. |
| **Snapshot testing** | Compara saída com snapshot salvo; falha se diferente. | Detectar mudanças inesperadas em output complexo (JSON, HTML); requer revisão manual ao atualizar. |

---

## Testes de acessibilidade (a11y)

| Aspecto | O que testar | Ferramentas |
|---------|-------------|-------------|
| **Semântica HTML** | Elementos corretos (`<button>`, `<nav>`, `<label>`, headings hierárquicos). | axe-core, jest-axe, testing-library queries (`getByRole`). |
| **Navegação por teclado** | Todos os interativos acessíveis por Tab/Enter/Esc; foco visível. | Testes E2E com Tab simulation; `userEvent.tab()`. |
| **ARIA** | `aria-label`, `aria-live`, roles corretos quando HTML nativo não basta. | axe-core automatizado + revisão manual em fluxos dinâmicos. |
| **Contraste e tamanho** | Relação de contraste WCAG AA (4.5:1 texto, 3:1 elementos grandes). | Lighthouse, axe. |

Integrar `jest-axe` (Angular) ou equivalente nos testes de componente para cobertura automatizada de a11y.

---

## Mapeamento ao projeto

### Frontend (Angular)

| Tipo | Localização | Comando |
|------|-------------|---------|
| **Unitários** | `*.spec.ts` ao lado do ficheiro sob teste (ex.: `auth.service.spec.ts`, `feature-list.component.spec.ts`) | `npm run test` ou `ng test` |
| **E2E** | `frontend/e2e/*.spec.ts` (ex.: `app.spec.ts`, `feature-name.spec.ts`) | Conforme config do projeto (ex.: `ng e2e`) |

Estrutura típica: `frontend/src/app/` com subpastas por camada (application, domain, infrastructure, features); cada módulo pode ter um ou mais ficheiros `.spec.ts`.

### Backend (.NET)

| Tipo | Localização | Comando |
|------|-------------|---------|
| **Unitários** | `backend/src/<Project>.Tests/Unit/` (ex.: `GetStatusQueryHandlerTests.cs`, `LoginCommandHandlerTests.cs`) | `dotnet test` (na pasta do projeto de testes ou na solution) |
| **Integração** | `backend/src/<Project>.Tests/Integration/` (ex.: `FeatureControllerTests.cs`, `AuthControllerTests.cs`) | `dotnet test` |

Executar a suíte a partir da raiz do backend ou da solution: `dotnet test` para todos os testes; ou especificar o projeto de testes.

---

## Estratégias Red Path — Implementação no Green (Kent Beck)

Três estratégias para fazer o teste passar (fase Green do TDD). Escolher conforme confiança e complexidade.

| Estratégia | Como funciona | Quando usar |
|------------|---------------|-------------|
| **Fake It** | Retornar constante → depois generalizar para variável/cálculo real | Incerteza sobre o algoritmo; bootstrap de primeira implementação; quando precisa de "um passo de cada vez" |
| **Triangulation** | Escrever 2+ testes com exemplos distintos → forçar generalização | Quando não sabe qual abstração usar; padrão emerge de múltiplos exemplos |
| **Obvious Implementation** | Implementar diretamente a solução correta | Quando a solução é transparente e simples; alto nível de confiança |

**Regra de auto-calibração:** Se testes começam a falhar inesperadamente → reduzir o tamanho do passo (voltar para Fake It). Se tudo passa facilmente → aumentar o passo (usar Obvious Implementation).

---

## Test Data Strategies (Kent Beck)

| Estratégia | Descrição | Quando usar |
|------------|-----------|-------------|
| **Evident Data** | Dados no teste tornam explícita a relação input→output (ex: `conta(100, taxa=0.1) → 110`) | Sempre que possível — máxima legibilidade |
| **Constantes simbólicas** | Usar nomes de constantes significativos em vez de magic numbers no teste | Quando o valor em si não importa, mas a relação sim |
| **Dados realistas** | Dados do domínio real (CPFs válidos, emails reais) | Testes de integração ou quando formato é relevante para a regra |
| **One to Many** | Começar com 1 elemento → depois testar com N elementos → generalizar | Coleções, listas, agregações |

---

## Test List Management (Kent Beck)

Manter **lista dinâmica de testes** como ferramenta de escopo durante implementação:

1. Antes de implementar, listar todos os testes que expressam o comportamento desejado
2. Começar pelo mais simples (confiança + momentum)
3. À medida que implementa, novos testes surgem → adicionar à lista
4. Riscar testes implementados; adicionar novos que surgirem
5. A lista evita perda de foco — quando surgir ideia, anotar na lista e continuar no teste atual

**Anti-padrão:** Sair implementando sem lista → escopo se expande, foco se perde, testes ficam incompletos.

---

## Comfort Level e Step Size (Kent Beck)

Auto-calibração do tamanho do passo no TDD:

| Sinal | Ação |
|-------|------|
| Testes passam facilmente, solução óbvia | Aumentar passo (Obvious Implementation) |
| Teste falha inesperadamente | Reduzir passo (Fake It, baby steps) |
| Não sabe qual teste escrever primeiro | Escrever o mais simples possível (degenerate case) |
| Muitos testes falhando ao mesmo tempo | Reverter última mudança; passo menor |
| Feedback loop > 10 segundos | Dividir teste ou isolar dependência |

**Princípio:** Passos menores quando inseguro, maiores quando confiante. O tamanho do passo é variável, não fixo.

---

## Characterization Tests (Michael Feathers)

Testes que **documentam comportamento existente** — não prescrevem comportamento desejado.

| Aspecto | Characterization Test | Test Prescritivo (TDD normal) |
|---------|-----------------------|-------------------------------|
| **Propósito** | Documentar o que o código **faz** | Definir o que o código **deveria fazer** |
| **Quando escrever** | Antes de alterar código legado | Antes de implementar feature nova |
| **Asserção vem de…** | Executar código e observar resultado | Requisito/especificação |
| **Falha significa…** | Comportamento mudou (pode ser intencional) | Bug ou implementação incompleta |

**Workflow para criar characterization test:**
1. Chamar o código como está
2. Colocar asserção com valor qualquer
3. Rodar → ver valor real na falha
4. Ajustar asserção para valor real → teste passa
5. Comportamento documentado ✅

**Regra:** Characterization tests são **rede de segurança para legacy code** — criar antes de qualquer refatoração em código sem testes. Ver skill [codigo-legado](../codigo-legado/SKILL.md) para workflow completo.

---

## Normas citadas (referência)

- **ISO/IEC/IEEE 29119** (Software Testing): processos de teste, documentação, técnicas.
- **ISTQB Glossary:** terminologia (teste de aceite, regressão, cobertura).
- **IEEE 829:** estrutura de documentação de testes.

---

## Rigor nas Variáveis de Teste (Wazlawick)

Cada teste deve ter **variáveis claras** e, quando aplicável, **baseline definida**. Fonte: [engenharia-de-software reference.md](../engenharia-de-software/reference.md) §12.

### Mapeamento de variáveis por teste

| Conceito | Definição | Exemplo no teste |
|----------|-----------|-------------------|
| **Variável independente** | O que manipulamos (input) | Dados de entrada do Arrange |
| **Variável dependente** | O que medimos (output) | Resultado verificado no Assert |
| **Baseline** | Valor de referência antes da alteração | Comportamento documentado por characterization test |

### Checklist para criação de testes

- [ ] Cada teste identifica claramente: variável independente (input) e dependente (output esperado)?
- [ ] Para testes de melhoria: baseline medida antes da alteração?
- [ ] Critérios de aceite com definição operacional ("tempo ≤ 200ms") e não constitutiva ("deve ser rápido")?
- [ ] Limitações do teste documentadas (o que NÃO cobre)?

---

## Mapeamento ao projeto Diário de Bordo

- Backend: testes unitários e integração em `backend/src/DiarioDeBordo.Tests/DiarioDeBordo.Tests.csproj` (xUnit, FluentAssertions, coverlet)
  - Comando: `dotnet test backend/src/DiarioDeBordo.Tests/DiarioDeBordo.Tests.csproj -c Release`
- Frontend: testes unitários e E2E em `frontend/src/app/**/*.spec.ts` e `frontend/e2e/*.spec.ts` (Angular CLI, Playwright)
  - Comando unitário: `ng test --no-watch --browsers=ChromeHeadlessCI`
  - Comando E2E: `playwright test --config=e2e/playwright.config.ts`

### Exemplos de testes do domínio

- Obra: teste de atualização de posição, validação de status, histórico de comentários.
- Posição: teste de limites, validação de enum de status.
- Histórico: teste de ordenação, inclusão de comentários.
