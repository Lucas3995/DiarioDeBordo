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
2. Escreva apenas o suficiente do teste para que ele falhe.
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

## Mapeamento ao projeto (DiarioDeBordo)

### Frontend (Angular)

| Tipo | Localização | Comando |
|------|-------------|---------|
| **Unitários** | `*.spec.ts` ao lado do ficheiro sob teste (ex.: `auth.service.spec.ts`, `obra-lista.component.spec.ts`) | `npm run test` ou `ng test` |
| **E2E** | `frontend/e2e/*.spec.ts` (ex.: `app.spec.ts`, `obras.spec.ts`) | Conforme config do projeto (ex.: `ng e2e`) |

Estrutura típica: `frontend/src/app/` com subpastas por camada (application, domain, infrastructure, features); cada módulo pode ter um ou mais ficheiros `.spec.ts`.

### Backend (.NET)

| Tipo | Localização | Comando |
|------|-------------|---------|
| **Unitários** | `backend/src/DiarioDeBordo.Tests/Unit/` (ex.: `GetStatusQueryHandlerTests.cs`, `LoginCommandHandlerTests.cs`) | `dotnet test` (na pasta do projeto de testes ou na solution) |
| **Integração** | `backend/src/DiarioDeBordo.Tests/Integration/` (ex.: `ObrasControllerTests.cs`, `AuthControllerTests.cs`) | `dotnet test` |

Executar a suíte a partir da raiz do backend ou da solution: `dotnet test` para todos os testes; ou especificar o projeto de testes.

---

## Normas citadas (referência)

- **ISO/IEC/IEEE 29119** (Software Testing): processos de teste, documentação, técnicas.
- **ISTQB Glossary:** terminologia (teste de aceite, regressão, cobertura).
- **IEEE 829:** estrutura de documentação de testes.
