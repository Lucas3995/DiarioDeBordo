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
| **Definição Operacional** | Definição que especifica como medir/verificar um conceito (vs constitutiva, que apenas descreve). | tradutor, quadro |
| **Definição Constitutiva** | Definição que descreve conceito usando outros conceitos; não mensurável diretamente. | tradutor |
| **Hipótese Justificada** | Afirmação testável sobre causa-efeito de uma alteração: "Acredito que X / O mecanismo é Y / Saberemos quando Z". | maestro |
| **Variável Independente** | O que se altera deliberadamente (input, estímulo) num teste ou experimento. | quadro |
| **Variável Dependente** | O que se observa/mede como resultado da alteração na variável independente. | quadro |
| **Nível de Maturidade (L1–L5)** | Escala Wazlawick de rigor em especificação: L1 narrativa → L5 definições operacionais com baseline estatístico. | engenharia-de-software |
| **Revisão Sistemática** | Processo metódico de busca, seleção e análise de evidências; modelo para exploração de codebase. | engenharia-de-software |

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
| **Arquitetura Gritante** | Estrutura do sistema deve expressar o domínio, não o framework. Teste: lendo pastas, o leitor diz o que o sistema faz. | batedor, mestre-freire |
| **Paradigma como Disciplina Negativa** | Paradigmas removem capacidades (goto, ponteiros, mutação) — não adicionam. Cada restrição habilita raciocínio seguro. | engenharia-de-software |

---

## Pragmatismo (Hunt & Thomas)

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **Broken Windows** | Tolerância zero a código degradado; uma "janela quebrada" leva a abandono generalizado. | batedor, mestre-freire |
| **Ortogonalidade** | Componentes independentes — alterar um não afeta outros. Teste: "se mudo X, quantas coisas quebram?" | batedor, mestre-freire |
| **Reversibilidade** | Decisões devem ser reversíveis; isolar compromissos atrás de abstrações. | maestro, mestre-freire |
| **Tracer Bullets** | Fluxo ponta-a-ponta mínimo que atravessa todas as camadas; iterar sobre feedback real. | mercenario, maestro |
| **Design by Contract** | Pré-condições (caller garante) + pós-condições (função garante) + invariantes (sempre verdade). | mercenario, batedor |
| **Programação Assertiva** | Assertions para verificar suposições; falhar ruidosamente se violadas. | mercenario |
| **DRY** | Don't Repeat Yourself — cada conhecimento com representação única e autoritativa. Sobre conhecimento, não código. | batedor, mestre-freire |
| **Automação Ubíqua** | Automatizar tudo repetitivo e propenso a erro humano; manual é para pensamento criativo. | todas |

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
| **Two Hats** | Metáfora Fowler: chapéu de refatoração (mudar estrutura) vs chapéu de funcionalidade (mudar comportamento). Nunca usar ambos simultaneamente. | mestre-freire |
| **Design Stamina Hypothesis** | Bom design se paga: embora mais lento no início, a velocidade se mantém ao longo do tempo vs degradação sem design. | mestre-freire |
| **Green Bar Discipline** | Refatorar apenas com testes passando (barra verde). | mestre-freire |
| **Branch by Abstraction** | Substituir grande componente gradualmente: criar abstração → migrar consumidores → remover antigo. | mestre-freire |
| **Expandable-Contractible** | Padrão de deploy: expandir (suportar antigo + novo) → migrar → contrair (remover antigo). | mestre-freire |
| **Heurísticas Clean Code** | 62 heurísticas do Cap. 17 (G1-G34, C1-C5, E1-E2, F1-F4, N1-N7, T1-T9) — checklist diagnóstica de inadequações. | batedor |

---

## Padrões de Design (GoF)

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **Design Pattern** | Solução reutilizável para problema recorrente em contexto específico (contexto–problema–solução). | padroes-de-design, batedor, mestre-freire |
| **Abstract Factory** | Cria famílias de objetos relacionados sem especificar classes concretas. | padroes-de-design, mercenario |
| **Builder** | Separa construção de objeto complexo da sua representação. | padroes-de-design, mercenario |
| **Factory Method** | Define interface de criação; subclasses decidem qual classe instanciar. | padroes-de-design, mercenario |
| **Prototype** | Cria objetos copiando instância existente. | padroes-de-design |
| **Singleton** | Garante instância única com ponto de acesso global. | padroes-de-design |
| **Adapter** | Converte interface de classe para outra esperada pelo cliente. | padroes-de-design, mestre-freire |
| **Bridge** | Desacopla abstração de implementação para variarem independentemente. | padroes-de-design |
| **Composite** | Compõe objetos em estruturas de árvore (parte-todo). | padroes-de-design |
| **Decorator** | Adiciona responsabilidades dinamicamente via composição. | padroes-de-design, mestre-freire |
| **Facade** | Interface simplificada para subsistema complexo. | padroes-de-design |
| **Flyweight** | Compartilha estado para suportar grande número de objetos similares. | padroes-de-design |
| **Proxy** | Substituto que controla acesso ao objeto real. | padroes-de-design |
| **Chain of Responsibility** | Passa requisição por cadeia de handlers até um tratar. | padroes-de-design |
| **Command** | Encapsula requisição como objeto; permite desfazer, enfileirar, logar. | padroes-de-design, mercenario |
| **Iterator** | Acesso sequencial a elementos sem expor estrutura interna. | padroes-de-design |
| **Mediator** | Centraliza comunicações complexas entre objetos. | padroes-de-design |
| **Memento** | Captura e restaura estado interno sem violar encapsulamento. | padroes-de-design |
| **Observer** | Notificação automática de dependentes quando sujeito muda estado. | padroes-de-design |
| **State** | Altera comportamento do objeto quando estado interno muda. | padroes-de-design, mestre-freire |
| **Strategy** | Define família de algoritmos intercambiáveis. | padroes-de-design, mestre-freire |
| **Template Method** | Define esqueleto de algoritmo; subclasses implementam passos. | padroes-de-design |
| **Visitor** | Adiciona operações a estrutura de objetos sem modificá-los. | padroes-de-design |

---

## Código Legado (Feathers)

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **Legacy Code** | Código sem testes — independente de idade ou qualidade. | codigo-legado |
| **Seam** | Ponto onde comportamento pode ser alterado sem editar o código no ponto. | codigo-legado, mestre-freire |
| **Enabling Point** | Mecanismo que ativa a alteração no seam (ex: injeção no constructor). | codigo-legado |
| **Object Seam** | Seam baseado em polimorfismo/override; preferido em OO. | codigo-legado |
| **Link Seam** | Seam baseado em substituição em link-time (classpath, build). | codigo-legado |
| **Preprocessing Seam** | Seam baseado em macros/compilação condicional; último recurso. | codigo-legado |
| **Sensing** | Quebrar dependência para acessar valores que o código computa (asserções). | codigo-legado |
| **Separation** | Quebrar dependência para isolar código em test harness (sem I/O/DB). | codigo-legado |
| **Characterization Test** | Teste que documenta comportamento existente (não prescreve desejado). | codigo-legado, quadro |
| **Fake Object** | Objeto com dois lados: lado produção (interface) + lado teste (sensing). | codigo-legado, quadro |
| **Sprout Method** | Criar método novo isolado para feature nova; testar separadamente. | codigo-legado |
| **Sprout Class** | Criar classe nova para feature quando original é untestable. | codigo-legado |
| **Wrap Method** | Renomear método original + criar novo que chama original + adição. | codigo-legado |
| **Wrap Class** | Classe wrapper (Decorator) que envolve original + adiciona comportamento. | codigo-legado |
| **Legacy Code Change Algorithm** | 5 passos: identificar mudança → achar ponto de teste → quebrar deps → characterization tests → mudar. | codigo-legado |

---

## TDD Avançado (Kent Beck)

| Termo | Definição | Skills que usam |
|-------|-----------|-----------------|
| **Fake It** | Estratégia Green: retornar constante, depois generalizar. | quadro |
| **Triangulation** | Estratégia Green: 2+ exemplos forçam generalização do algoritmo. | quadro |
| **Obvious Implementation** | Estratégia Green: implementar diretamente quando solução é transparente. | quadro |
| **Test List** | Lista dinâmica de testes como ferramenta de escopo durante implementação. | quadro |
| **Evident Data** | Dados no teste tornam explícita relação input→output. | quadro |
| **Step Size** | Tamanho do passo no TDD — menor quando inseguro, maior quando confiante. | quadro |

---

## Domínio — Diário de Bordo

| Termo | Definição | Contexto / Bounded Context |
|-------|-----------|---------------------------|
| **Obra** | Item acompanhado (manga, anime, livro, filme, série) | Acompanhamento |
| **Posição** | Estado atual de leitura/visualização | Acompanhamento |
| **Comentário** | Observação sobre parte ou status | Histórico |
| **Preferência** | Ordem de prioridade do usuário | Acompanhamento |
| **Histórico** | Registro de mudanças de posição e comentários | Histórico |
| **Status** | Enum de situação (em andamento, concluído, pausado, dropado) | Acompanhamento |
| **Agregado** | Cluster de entidades (ex.: Obra + Histórico) | DDD |
| **Evento de domínio** | Mudança relevante (ex.: atualização de posição) | DDD |
| **Acompanhamento** | Bounded Context principal | Domínio |
| **Usuário** | Pessoa que acompanha obras | Domínio |
