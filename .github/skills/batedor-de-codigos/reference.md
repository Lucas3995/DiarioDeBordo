# Catálogo de Code Smells — Referência para Análise

Este documento contém o catálogo completo de code smells para detecção de inadequações no código. Fonte: conceitos de entropia de software e refatoração.

---

## Bloaters

Código, métodos e classes que cresceram a proporções difíceis de trabalhar. Acumulam-se com o tempo quando não são erradicados.

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Long Method** | Método com excesso de linhas. | Mais de 10 linhas; vários níveis de indentação; necessidade de scroll para ver o fluxo. | SRP, legibilidade |
| **Large Class** | Classe que faz demais ou tem demasiados campos/métodos. | Muitos campos/métodos; responsabilidades múltiplas; difícil dar nome preciso à classe. | SRP |
| **Primitive Obsession** | Uso de tipos primitivos em vez de objetos de valor para tarefas simples. | Constantes para codificar info (USER_ADMIN_ROLE = 1); strings como nomes de campos em arrays; pares (moeda, valor) como primitivos. | DDD (Value Objects) |
| **Long Parameter List** | Método ou construtor com muitos parâmetros. | Mais de 3 parâmetros; difícil lembrar a ordem; alterações frequentes na assinatura. | — |
| **Data Clumps** | Mesmos parâmetros passados juntos em vários métodos. | Mesmos 2–3 campos em vários métodos ou classes; poderiam formar objeto de valor. | DRY, DDD |
| **Incomplete Library Class** | Biblioteca externa incompleta; métodos utilitários espalhados em vez de encapsular. | Extensões da lib espalhadas; wrappers frágeis ou repetidos. | Encapsulamento |

---

## Object-Orientation Abusers

Uso incompleto ou incorreto de princípios de OO.

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Switch Statements** | Uso excessivo de switch/if-else complexos quando polimorfismo seria apropriado. | Muitos switch/if sobre mesmo tipo ou enum; adicionar caso obriga tocar em vários sítios. | OCP, polimorfismo |
| **Temporary Field** | Campo que só recebe valor em certas circunstâncias; fora delas fica vazio/inútil. | Campos preenchidos apenas em alguns fluxos; objeto "incompleto" na maior parte do tempo. | Coesão |
| **Refused Bequest** | Subclasse herda mas rejeita metade dos métodos (NotImplementedException ou vazios). | Subclasse não substitui corretamente a superclasse. | LSP |
| **Alternative Classes with Different Interfaces** | Duas classes fazem o mesmo mas expõem interfaces diferentes. | Mesma responsabilidade; métodos com nomes/assinaturas diferentes. | ISP |
| **Type Checking** | Verificações constantes do tipo (if objeto is Type). | Verificações de tipo espalhadas; abstração inadequada. | Polimorfismo, LSP |
| **Fat Interface** | Interface com muitos métodos; implementador só precisa de poucos; resto fica vazio. | Interface com 20 métodos; classe implementa 3; 17 métodos vazios. | ISP |
| **Service Locator Smell** | Classe chama Global Registry/Service Locator em vez de receber dependências no construtor. | Chamadas a ServiceLocator.get(), container.get() no meio do código; dependências ocultas. | DIP |
| **Static Cling** | Uso excessivo de métodos e propriedades static. | Muitos static; difícil polimorfismo e IoC. | OCP, DIP |
| **Framework Coupling** | Classes fora de infrastructure/presentation herdam de classes do framework. | Entidades ou Casos de Uso herdando de BaseController, Entity, etc. | Clean Architecture, DIP |
| **Strengthening Preconditions** | Subclasse exige mais que a superclasse (lança erro para valor que o pai aceita). | Subclasse valida e lança exceção onde a superclasse aceitaria. | LSP |
| **Header Interface** | Interface que é espelho exato de uma única classe (1:1) só por "protocolo". | Interface com todos os métodos da única classe que a implementa; sem flexibilidade real. | ISP |
| **The Transitive Dependency** | A depende de B que depende de C; A precisa saber detalhes de C para usar B. | Vazamento de responsabilidade; cliente conhece detalhes de classes transitivas. | Encapsulamento |

---

## Change Preventers

Mudar uma coisa obriga mudar em muitos lugares. Desenvolvimento mais caro e complicado.

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Divergent Change** | Uma classe é alterada por razões diferentes em momentos diferentes. | Várias "famílias" de mudanças que tocam na mesma classe; mais de uma razão para mudar. | SRP |
| **Shotgun Surgery** | Uma mudança exige pequenas alterações em muitas classes. | Alteração de regra dispersa por vários ficheiros; impacto amplo para uma decisão. | CCP |
| **Parallel Inheritance Hierarchies** | Criar subclasse numa hierarquia obriga criar subclasse noutra. | Duas árvores de herança evoluem em paralelo; tipo novo implica criar em ambas. | — |

---

## Dispensables

Elemento sem propósito; ausência tornaria o código mais limpo.

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Duplicate Code** | Mesmo (ou quase) código em mais de um sítio. | Blocos idênticos ou muito semelhantes; cuidado: regras de negócio distintas podem evoluir separadamente (falso positivo). | DRY |
| **Lazy Class** | Classe que faz pouco; custo de manutenção não se justifica. | Classe com um ou poucos métodos; poderia ser incorporada noutra. | YAGNI |
| **Data Class** | Classe apenas com campos e getters/setters; sem responsabilidade de negócio. | Apenas dados e accessors; comportamento está noutras classes. | DDD (entidades anêmicas) |
| **Dead Code** | Variável, parâmetro, campo, método ou classe não utilizados. | Métodos nunca chamados; ramos sempre falsos; imports não usados. | — |
| **Speculative Generality** | Abstrações "para o futuro" que nunca ocorrem. | Hierarquias, parâmetros, hooks não usados; YAGNI violado. | YAGNI |

---

## Couplers

Acoplamento excessivo ou delegação excessiva.

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Feature Envy** | Método acessa dados de outro objeto mais que os próprios. | Método usa sobretudo getters de outro objeto; comportamento pertence à outra classe. | Coesão |
| **Inappropriate Intimacy** | Duas classes conhecem detalhes internos uma da outra. | Acesso excessivo a membros internos; mudar uma implica mudar a outra. | Encapsulamento |
| **Message Chains** | Encadeamento longo de chamadas: a.getB().getC().getD().doSomething() | Cliente depende da estrutura interna da cadeia; fragilidade perante mudanças. | Law of Demeter |
| **Middle Man** | Classe que só delega; não acrescenta valor. | Muitos métodos que apenas encaminham; "atravessador" inútil. | — |

---

## Arch and Struct

Smells de nível mais alto (arquitetura e estrutura).

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Circular Dependency** | Componente A depende de B que depende de A. | Ciclo nas dependências; difícil testar e fazer deploy independente. | Dependências acíclicas |
| **God Object** | Classe que "sabe tudo" e "manda em todos"; falha dela para o sistema. | Classe central com excesso de responsabilidades e dependências. | SRP, Clean Architecture |
| **Hard-Coded Dependencies** | Instanciar classes concretas (new) em vez de Injeção de Dependência. | new MinhaClasse() dentro de outra; impossível mockar; código rígido. | DIP |

---

## Test Smells

Smells em testes unitários e de integração.

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Fragile Tests** | Testes que quebram por qualquer mudança mínima. | Teste conhece detalhes internos da implementação. | Teste de comportamento |
| **Eager Test** | Um método testa vários comportamentos ao mesmo tempo. | Múltiplas asserções não relacionadas; falha não indica o que quebrou. | Um conceito por teste |
| **Slow Tests** | Testes que demoram demais. | Desestimulam execução frequente; entropia cresce silenciosamente. | FIRST (Fast) |
| **Assertion Roulette** | Várias asserções sem mensagens explicativas. | Falha não indica qual das 10 validações causou o erro. | Clareza |
| **Obscure Test** | Teste complexo, longo ou cheio de dados irrelevantes. | Não se identifica o que está sendo verificado. | Legibilidade |
| **Conditional Test Logic** | if, switch ou for dentro do teste. | Teste com lógica; está testando o próprio teste. | Caminho linear |
| **Mystery Guest** | Teste depende de informação externa não visível no código. | Arquivo no disco, linha no banco; resultado "mágico". | Repetibilidade |
| **Resource Optimism** | Teste assume recurso externo sempre disponível e rápido. | API, rede; falha sem motivo lógico quando ambiente oscila. | Isolamento |
| **Test Run War** | Dois testes falham quando rodados juntos; funcionam isolados. | Compartilham estado (variável global, tabela). | Independência |
| **Flaky Tests** | "Falha, mas se rodar de novo passa". | Concorrência ou falta de limpeza de estado. | Repetibilidade |
| **Sleepy Test** | Thread.sleep() para esperar processo assíncrono. | Suíte lenta e instável. | Polling/callbacks |
| **The Free Ride** | Adicionar asserção a teste existente em vez de novo teste. | Teste perde foco; "colcha de retalhos". | Um conceito por teste |
| **Happy Path Only** | Teste só verifica cenário onde tudo dá certo. | Ignora exceções, null, borda; falsa cobertura. | Cobertura |
| **The Mockery** | Uso excessivo de Mocks. | Teste passa mas sistema real quebrado na integração não é detectado. | Confiabilidade |

---

## Component Smells

Smells em nível de componente/módulo. Ver [principios-componentes.md](../../reference/principios-componentes.md) para fundamentos teóricos.

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Componente Incoeso (REP)** | Classes de temas/propósitos distintos agrupadas no mesmo componente. | Componente com domínios misturados; publicação sem sentido como unidade. | REP |
| **Fechamento Comum Violado (CCP)** | Classes que mudam por razões ou momentos diferentes no mesmo componente. | Alteração em uma feature obriga rebuild de componente não relacionado; componentes entram em deploy sem terem sido alterados. | CCP |
| **Dependência Desnecessária (CRP)** | Consumidor depende de componente mas só usa fração dele. | Import/referência a componente grande por causa de 1–2 classes; mudanças no componente forçam recompilação do consumidor. | CRP |
| **Dependência Cíclica (DA)** | Ciclo no grafo de dependências entre componentes (A→B→A ou cadeia). | Build falha ao isolar componente; impossível deployar independentemente; conflitos de alteração em cascata. | DA (Dependências Acíclicas) |
| **Instável Depende de Estável (SDP)** | Componente fácil de alterar depende de componente difícil de alterar. | Mudança simples "travada" por dependência de módulo estável/concreto. | SDP (Dependências Estáveis) |
| **Estável e Concreto (SAP)** | Componente altamente estável sem abstrações — difícil de estender. | Componente com muitos dependentes, mas composto apenas de classes concretas; extensão exige modificação. | SAP (Abstrações Estáveis) |
| **Split Prematuro** | Componentes separados que sempre são publicados e alterados juntos. | Dois componentes com mesmo motivo de mudança e deploy sempre simultâneo; overhead de build sem benefício. | Over-engineering |
| **Merge Necessário** | Componentes que deveriam estar juntos por coesão (REP/CCP), mas foram separados sem razão. | Classes fortemente relacionadas em componentes diferentes; mudanças sempre tocam ambos. | REP + CCP |

---

## Anti-patterns DDD

Smells que indicam modelagem de domínio inadequada. Ver [DDD-IA-GUIDE.md](../../../Validados/DDD-IA-GUIDE.md) para fundamentos.

| Smell | Definição | Sinais no código | Princípio violado |
|-------|-----------|------------------|-------------------|
| **Entidade Anêmica** | Entidade com apenas getters/setters; regras de negócio espalhadas em serviços. | Classe de domínio sem métodos de comportamento; lógica em Application Services ou Controllers. | DDD (Entidades Ricas) |
| **God Service** | Serviço que concentra regras de múltiplos agregados/domínios. | Classe de serviço com dezenas de métodos cobrindo contextos distintos; impossível nomear com precisão. | SRP + DDD (Domain Services) |
| **Repositório com Lógica de Negócio** | Repositório que filtra, calcula ou aplica regras além de persistência. | Métodos do repositório com condicionais de negócio, cálculos de valores; regras que deveriam estar no domínio. | DDD (Repositórios) |
| **Ausência de Bounded Context** | Módulo mistura termos e regras de subdomínios distintos. | Mesma classe/módulo com conceitos de Faturamento, Logística e Cadastro; linguagem ubíqua inconsistente. | DDD (Bounded Contexts) |
| **Linguagem Ubíqua Inconsistente** | Nomes técnicos genéricos em vez de termos do domínio de negócio. | Classes chamadas `Manager`, `Helper`, `Utils`, `Data`; métodos como `process()`, `handle()` sem semântica de domínio. | DDD (Linguagem Ubíqua) |
| **Primitive Obsession no Domínio** | Conceitos ricos do domínio representados como primitivos. | CPF como `string`, Dinheiro como `decimal`, Email como `string`; validações espalhadas. | DDD (Value Objects) |
| **Agregado sem Invariantes** | Aggregate Root que não protege regras de consistência. | Qualquer código externo pode alterar entidades internas do agregado diretamente; invariantes verificados fora do agregado. | DDD (Agregados) |

---

## Workflow de Análise (checklist sequencial)

Checklist adaptado para auditoria completa. Executar em ordem.

1. **Mapear estrutura:** projetos/módulos (componentes), namespaces/pastas (possíveis camadas).
2. **Verificar camadas:** direção de dependências; entidades/casos de uso não dependem de UI, DB ou frameworks.
3. **SRP:** cada classe/módulo alterado apenas por um ator/role.
4. **OCP:** níveis por frequência de mudança; interfaces para inversão.
5. **LSP:** dependência na classe base; subclasses substituíveis.
6. **ISP:** consumidor vê só o que usa; interfaces segregadas.
7. **DIP:** dependência de abstrações; concretas no ponto de uso (Main).
8. **REP:** componente = grupo coeso; publicação conjunta.
9. **CCP:** mesmo componente = mesmas razões e momento de mudança.
10. **CRP:** sem dependência desnecessária do consumidor do componente.
11. **Acoplamento (DA, SDP, SAP):** ciclos, estabilidade, abstração.
12. **Módulos existentes ou faltantes:** split/merge necessário? Over-engineering?
13. **DDD:** entidades anêmicas, God Services, repositórios com lógica, bounded contexts.
14. **Pontos adicionais:** Main, fronteiras, Humble Object, independências, arquitetura gritante.
