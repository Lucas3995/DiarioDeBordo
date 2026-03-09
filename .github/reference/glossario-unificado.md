# Glossário Unificado

Termos usados pelas skills da rotina-completa. Referência centralizada para evitar ambiguidade e garantir consistência entre relatórios, análises e implementações.

---

## Engenharia de Software

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **Requisito funcional** | O que o sistema deve fazer (comportamentos, funções, capacidades). | tradutor, maestro |
| **Requisito não funcional (NFR)** | Como o sistema deve se comportar (desempenho, segurança, usabilidade). | tradutor, maestro |
| **Critério de aceite** | Condição mensurável que define quando um requisito está satisfeito. | tradutor, quadro |
| **Rastreabilidade** | Capacidade de ligar demanda → requisito → código → teste. | maestro, quadro |
| **Spec-driven development** | Especificação como fonte; código e testes derivam da spec. | todas |
| **SQA (Software Quality Assurance)** | Conjunto de atividades para garantir qualidade no processo e produto. | engenharia-de-software |
| **Dependabilidade** | Confiabilidade + disponibilidade + segurança (safety) + integridade. | engenharia-de-software |

---

## Clean Architecture e SOLID

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **SRP** | Uma classe muda por um único ator/razão. | batedor, mestre-freire |
| **OCP** | Aberto para extensão, fechado para modificação. | batedor, mestre-freire |
| **LSP** | Subtipos substituíveis pelo tipo base sem quebrar. | batedor, mestre-freire |
| **ISP** | Interfaces segregadas; consumidor vê só o que usa. | batedor, mestre-freire |
| **DIP** | Depender de abstrações, não de concretos. | batedor, mestre-freire, mercenario |
| **Camada** | Nível de abstração (domínio, aplicação, infraestrutura, apresentação). | todas |
| **Componente** | Unidade de deploy/release; agrupa classes coesas. | batedor, mestre-freire |
| **REP** | Reuse/Release Equivalence — componente = grupo coeso publicável. | batedor, mestre-freire |
| **CCP** | Common Closure — classes no mesmo componente mudam pela mesma razão. | batedor, mestre-freire |
| **CRP** | Common Reuse — não forçar dependência de algo que não é usado. | batedor, mestre-freire |
| **DA** | Dependências Acíclicas — sem ciclos no grafo de componentes. | batedor, mestre-freire |
| **SDP** | Stable Dependencies — depender na direção da estabilidade. | batedor, mestre-freire |
| **SAP** | Stable Abstractions — componentes estáveis devem ser abstratos. | batedor, mestre-freire |
| **Humble Object** | Separar parte testável da não testável. | mestre-freire |

---

## DDD (Domain-Driven Design)

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **Entidade** | Objeto com identidade e ciclo de vida; métodos de domínio. | mercenario, batedor |
| **Value Object (VO)** | Objeto imutável sem identidade; igualdade por atributos. | mercenario, batedor |
| **Agregado** | Cluster de entidades/VOs com uma raiz que protege consistência. | mercenario, batedor |
| **Aggregate Root** | Entidade raiz do agregado; único ponto de acesso externo. | mercenario, batedor, mestre-freire |
| **Domain Service** | Operação de domínio stateless que não pertence a uma entidade. | mercenario, batedor |
| **Repository** | Abstração de persistência; interface no domínio, implementação na infra. | mercenario, batedor |
| **Domain Event** | Notificação de algo que aconteceu no domínio. | mercenario |
| **Bounded Context** | Fronteira linguística e de modelo; cada contexto tem sua linguagem ubíqua. | batedor, tradutor |
| **Linguagem Ubíqua** | Vocabulário compartilhado entre negócio e código. | tradutor, maestro, mercenario |

---

## Testes

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **TDD** | Test-Driven Development: Red → Green → Refactor. | quadro, mestre-freire |
| **Teste unitário** | Testa unidade isolada (classe, função); rápido, barato. | quadro |
| **Teste de integração** | Testa interação entre componentes (API, DB, serviços). | quadro |
| **Teste E2E** | Testa fluxo completo do ponto de vista do usuário. | quadro |
| **Stub** | Fornece respostas predefinidas; não verifica interações. | quadro |
| **Mock** | Verifica interações esperadas (chamadas, argumentos). | quadro |
| **Fake** | Implementação simplificada (ex.: repositório in-memory). | quadro |
| **Spy** | Registra chamadas para inspeção posterior. | quadro |
| **AAA** | Arrange-Act-Assert: padrão de estrutura de teste. | quadro |
| **FIRST** | Fast, Independent, Repeatable, Self-Validating, Timely. | quadro |
| **Property-based testing** | Testa propriedades invariantes com inputs aleatórios. | quadro |
| **Mutation testing** | Altera código e verifica se testes detectam. | quadro |

---

## Segurança e Proteção de Dados

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **LGPD** | Lei Geral de Proteção de Dados (Lei 13.709/2018). | tradutor, maestro |
| **Dado pessoal** | Informação que identifica ou pode identificar pessoa natural. | tradutor, maestro |
| **Minimização** | Coletar apenas dados necessários para a finalidade. | tradutor |
| **Consentimento** | Manifestação livre do titular para tratamento de dados. | tradutor |
| **Anonimização** | Tornar dado não identificável de forma irreversível. | tradutor |
| **Safety** | Segurança contra danos não intencionais (falhas, acidentes). | engenharia-de-software |
| **Security** | Segurança contra ameaças intencionais (ataques, invasões). | engenharia-de-software |

---

## Análise e Refatoração

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **Inadequação / Achado** | Violação de princípio ou padrão identificada no código. | batedor |
| **Code smell** | Indicador de degradação de código; não é bug, mas sinal de problema. | batedor |
| **Smell de componente** | Violação em nível de módulo/componente (REP, CCP, DA, etc.). | batedor |
| **Anti-pattern DDD** | Padrão inadequado de modelagem de domínio (entidade anêmica, God Service). | batedor |
| **Refatoração** | Mudança de estrutura sem alterar comportamento. | mestre-freire |
| **Priorização por impacto** | Ordenar achados: Alta (camadas/DIP) → Média (SOLID/DDD) → Baixa (componentes/organização). | mestre-freire |
