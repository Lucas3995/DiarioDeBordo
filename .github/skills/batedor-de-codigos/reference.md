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
15. **Padrões não-aplicados:** situações onde pattern GoF resolveria o problema (ver seção abaixo).

---

## Padrões Não-Aplicados (design pattern smells)

Situações onde um padrão GoF resolveria o problema mas não foi aplicado. Detectar como categoria de smell adicional.

| Sinal no código | Padrão que resolveria | Evidência |
|-----------------|----------------------|-----------|
| Múltiplas subclasses para combinações de comportamento | **Decorator** | N subclasses com combinatórias; herança explode |
| if/else ou switch baseado em tipo/enum (comportamento) | **Strategy**, **Command**, **Visitor** | Mesmo switch em vários locais; adicionar tipo obriga tocar em muitos arquivos |
| `new ConcreteClass()` espalhado por código de domínio | **Factory Method** ou **Abstract Factory** | Acoplamento a implementação concreta; impossível mockar |
| Método com múltiplos algoritmos selecionados por flag | **Strategy** | Flag booleana ou enum seleciona bloco de código |
| Objeto com estados e transições complexas | **State** | if/switch sobre estado; métodos com guard clauses por estado |
| Notificação manual (chamada direta) entre objetos | **Observer** | Objeto A chama B.notify() diretamente; acoplamento bidirecional |
| Subsistema complexo com múltiplos pontos de entrada | **Facade** | Clientes conhecem detalhes internos do subsistema |
| Criação condicional de objetos similares | **Factory Method** | if/switch para decidir qual classe instanciar |
| Herança profunda para reutilização | **Composição** (Bridge, Strategy) | Árvore > 3 níveis; classes-filhas usam poucos métodos do pai |
| Parsing/travessia de estrutura com lógica dispersa | **Visitor** ou **Iterator** | Código de processamento misturado com código de travessia |

**Referência cruzada:** Para lookup completo problema→padrão, ver [padroes-de-design/selecao-por-problema.md](../padroes-de-design/selecao-por-problema.md).

---

## Métricas Quantitativas de Referência (Clean Craftsmanship)

Thresholds sugeridos para elevar observação a smell. Não são regras absolutas — usar como referência junto com julgamento contextual.

| Métrica | Threshold sugerido | Severidade |
|---------|-------------------|------------|
| **Linhas por função** | > 20 linhas | Medium — considerar Extract Method |
| **Linhas por classe** | > 200 linhas | High — considerar Extract Class |
| **Complexidade ciclomática** | > 10 | High — considerar decomposição |
| **Parâmetros por método** | > 3 | Low — considerar Parameter Object ou Builder |
| **Profundidade de herança** | > 3 níveis | Medium — considerar composição |
| **Dependências de uma classe** | > 7 | Medium — possível God Object |
| **Cobertura de testes** | < 80% em código alterado | Blocker para entrega |

### Boy Scout Rule (operacionalizado)

Cada ciclo de análise (batedor) + refactoring (mestre-freire) deve deixar o código **mensuradamente melhor**:
- Reduzir pelo menos 1 smell de severidade alta
- Não introduzir novos smells
- Cobertura de testes não deve diminuir

### Quando elevar severity

| De | Para | Critério |
|----|------|----------|
| Improvement | Warning | Smell afeta mais de 1 arquivo ou módulo |
| Warning | Blocker | Smell impacta segurança, corretude ou bloqueia testabilidade |
| Qualquer | Ignore | Smell é intencional e documentado (ex: Facade longa por design) |

---

## Heurísticas Clean Code (Robert C. Martin — Cap. 17)

66 heurísticas acionáveis para diagnóstico. Cada heurística é um **sinal** de inadequação; usar como checklist complementar aos smells acima.

### General (G1-G34)

| Código | Nome | Diagnóstico | Ação |
|--------|------|-------------|------|
| **G1** | Multiple Languages in One Source File | Ficheiro mistura linguagens (HTML+JS+CSS inline, SQL em strings) | Separar por ficheiro; templates, queries externalizadas |
| **G2** | Obvious Behavior Is Unimplemented | Função não faz o que o nome promete (Principle of Least Surprise) | Implementar comportamento esperado ou renomear |
| **G3** | Incorrect Behavior at the Boundaries | Confiança na intuição em vez de testar limites | Testar todas as condições de fronteira explicitamente |
| **G4** | Overridden Safeties | Warnings desativados, testes ignorados, serialVersionUID removido | Restaurar mecanismos de segurança; investigar causa |
| **G5** | Duplication | Código duplicado em qualquer forma (blocos, cadeias, polimorfismo ausente) | Extrair; DRY — cada conhecimento em representação única |
| **G6** | Code at Wrong Level of Abstraction | Detalhe de implementação em classe base ou abstração | Separar alto nível de baixo nível; mover para camada adequada |
| **G7** | Base Classes Depending on Their Derivatives | Classe base conhece ou referencia subclasses | Inverter dependência; base não deve saber das derivadas |
| **G8** | Too Much Information | Interface expõe mais que o necessário | Reduzir superfície da API; esconder dados e utilitários |
| **G9** | Dead Code | Código nunca executado (if impossível, catch não alcançável) | Remover imediatamente |
| **G10** | Vertical Separation | Variável/função definida longe de onde é usada | Aproximar definição do uso |
| **G11** | Inconsistency | Coisas similares feitas de formas diferentes | Padronizar; consistência gera previsibilidade |
| **G12** | Clutter | Artefatos sem propósito (construtor default vazio, variável não usada) | Remover; manter código limpo |
| **G13** | Artificial Coupling | Coisas não relacionadas acopladas por conveniência | Mover para local semanticamente correto |
| **G14** | Feature Envy | Método usa mais dados de outra classe que da própria | Move Method |
| **G15** | Selector Arguments | Boolean/enum seleciona comportamento dentro da função | Separar em funções distintas |
| **G16** | Obscured Intent | Expressões run-on, magic numbers, nomes codificados | Explicitar intenção com nomes e variáveis intermediárias |
| **G17** | Misplaced Responsibility | Código em local inesperado (Principle of Least Surprise) | Mover para onde o leitor espera encontrar |
| **G18** | Inappropriate Static | Método static quando deveria ser de instância | Converter para método de instância (permite polimorfismo) |
| **G19** | Use Explanatory Variables | Cálculos complexos sem nomes intermediários | Extrair variáveis com nomes significativos |
| **G20** | Function Names Should Say What They Do | Nome não indica o que a função faz | Renomear para refletir ação completa |
| **G21** | Understand the Algorithm | Código funciona "por acaso"; testes passam sem compreensão | Refatorar até a lógica ser óbvia |
| **G22** | Make Logical Dependencies Physical | Módulo assume dependência sem declará-la | Explicitar dependência via parâmetro ou injeção |
| **G23** | Prefer Polymorphism to If/Else or Switch/Case | Switch sobre tipo quando polimorfismo é mais adequado | ONE SWITCH rule; criar objetos polimórficos |
| **G24** | Follow Standard Conventions | Desvio das convenções da equipa/indústria | Alinhar com padrões acordados |
| **G25** | Replace Magic Numbers with Named Constants | Números literais sem significado claro | Extrair para constante nomeada |
| **G26** | Be Precise | Decisões imprecisas (null não verificado, float para moeda) | Usar tipos corretos; verificar todas as condições |
| **G27** | Structure over Convention | Decisão de design depende de convenção de nomes | Usar estrutura (classes base, enums, tipos) para impor decisões |
| **G28** | Encapsulate Conditionals | Condicionais complexas inline | Extrair para função com nome descritivo |
| **G29** | Avoid Negative Conditionals | Negações obscurecem leitura | Reformular como positiva |
| **G30** | Functions Should Do One Thing | Função com múltiplas seções/responsabilidades | Dividir em funções de propósito único |
| **G31** | Hidden Temporal Couplings | Ordem de chamada implícita (deve chamar A antes de B) | Explicitar com bucket brigade (output de A é input de B) |
| **G32** | Don't Be Arbitrary | Estrutura sem justificativa clara | Documentar razão; estrutura deve comunicar intenção |
| **G33** | Encapsulate Boundary Conditions | +1 e -1 espalhados pelo código | Centralizar condições de fronteira em um local |
| **G34** | Functions Should Descend Only One Level of Abstraction | Statements em níveis de abstração diferentes | Separar por nível; cada função um degrau abaixo do nome |

### Comments (C1-C5)

| Código | Nome | Diagnóstico | Ação |
|--------|------|-------------|------|
| **C1** | Inappropriate Information | Metadados (autor, data, SPR) em comentários | Mover para controle de versão |
| **C2** | Obsolete Comment | Comentário desatualizado | Atualizar ou remover |
| **C3** | Redundant Comment | Comentário diz o que o código já diz | Remover |
| **C4** | Poorly Written Comment | Comentário confuso, mal escrito | Reescrever com clareza; conciso e ortográfico |
| **C5** | Commented-Out Code | Código comentado ("talvez precise depois") | Remover; controle de versão guarda histórico |

### Environment (E1-E2)

| Código | Nome | Diagnóstico | Ação |
|--------|------|-------------|------|
| **E1** | Build Requires More Than One Step | Build exige múltiplos passos manuais | Automatizar em um único comando |
| **E2** | Tests Require More Than One Step | Rodar testes exige múltiplos passos | Automatizar em um único comando |

### Functions (F1-F4)

| Código | Nome | Diagnóstico | Ação |
|--------|------|-------------|------|
| **F1** | Too Many Arguments | Função com >3 argumentos | Introduce Parameter Object; Combine into Class |
| **F2** | Output Arguments | Argumentos usados como saída (mutados pela função) | Alterar estado do próprio objeto; retornar resultado |
| **F3** | Flag Arguments | Boolean que seleciona comportamento | Separar em duas funções |
| **F4** | Dead Function | Método nunca chamado | Remover |

### Names (N1-N7)

| Código | Nome | Diagnóstico | Ação |
|--------|------|-------------|------|
| **N1** | Choose Descriptive Names | Nome genérico ou vago | Renomear com nome que revele intenção |
| **N2** | Choose Names at Appropriate Level of Abstraction | Nome expõe implementação em vez de conceito | Renomear para nível de abstração adequado |
| **N3** | Use Standard Nomenclature Where Possible | Nome ignora convenções conhecidas (Decorator, Factory) | Usar nomenclatura padrão quando aplicável |
| **N4** | Unambiguous Names | Nome pode significar mais de uma coisa | Renomear para ser inequívoco |
| **N5** | Use Long Names for Long Scopes | Variável de escopo longo com nome curto (i, x) | Nomes longos para escopos longos; curtos para 5 linhas |
| **N6** | Avoid Encodings | Prefixos de tipo/escopo (m_, f_, Hungarian) | Remover notação húngara e prefixos |
| **N7** | Names Should Describe Side-Effects | Nome esconde efeito colateral (getX() que também cria) | Renomear para revelar todos os efeitos |

### Tests (T1-T9)

| Código | Nome | Diagnóstico | Ação |
|--------|------|-------------|------|
| **T1** | Insufficient Tests | Testes não cobrem tudo que poderia quebrar | Adicionar testes até cobertura adequada |
| **T2** | Use a Coverage Tool | Sem ferramenta de cobertura | Adotar ferramenta; visualizar gaps |
| **T3** | Don't Skip Trivial Tests | Testes triviais pulados ("é óbvio") | Escrever; valor documental alto |
| **T4** | An Ignored Test Is a Question about an Ambiguity | @Ignore/skip como lembrete de requisito ambíguo | Resolver ambiguidade; ativar ou remover teste |
| **T5** | Test Boundary Conditions | Limites não testados | Testar min, max, zero, null, overflow |
| **T6** | Exhaustively Test Near Bugs | Bug encontrado; área não explorada exaustivamente | Testes exaustivos na vizinhança do bug |
| **T7** | Patterns of Failure Are Revealing | Padrão nos testes que falham não investigado | Analisar padrão; revela causa raiz |
| **T8** | Test Coverage Patterns Can Be Revealing | Código não coberto por testes que passam | Investigar; gaps de cobertura revelam lógica não testada |
| **T9** | Tests Should Be Fast | Testes lentos → equipe para de rodar | Otimizar; isolar I/O; paralelizar |

---

## Code Smells — Fowler (Refactoring 2nd Ed.)

Smells adicionais de Fowler **não cobertos** pelas categorias acima. Complementam o catálogo existente.

| Smell | Definição | Sinais no código | Refatoração primária |
|-------|-----------|------------------|---------------------|
| **Mysterious Name** | Nome de função, variável ou classe que não comunica propósito | Leitor precisa ler o corpo para entender o que faz | Change Function Declaration, Rename Variable, Rename Field |
| **Repeated Switches** | **Mesmo** switch/if-else sobre o mesmo discriminador em **múltiplos locais** | Adicionar enum/tipo obriga alterar N ficheiros; distinto de "Switch Statements" (único switch) | Replace Conditional with Polymorphism |
| **Loops** | Loop imperativo quando pipeline (map/filter/reduce) seria mais expressivo | for/while com acumuladores, filtros inline, transformações manuais | Replace Loop with Pipeline |
| **Lazy Element** | Função ou classe que faz tão pouco que não justifica existência (mais amplo que Lazy Class) | Função de uma linha que só repassa; classe com 1 método trivial | Inline Function, Inline Class, Collapse Hierarchy |
| **Insider Trading** | Módulos trocam dados internos em excesso; pior que Inappropriate Intimacy por envolver fronteiras de módulo | Módulos acessam internals um do outro; acoplamento entre boundaries | Move Function/Field, Hide Delegate, Replace with Delegate |
| **Global Data** | Dados acessíveis por qualquer ponto do programa | Variáveis globais, singletons mutáveis, static state compartilhado | Encapsulate Variable |
| **Mutable Data** | Dados alterados em múltiplos pontos sem controle | Variáveis reatribuídas em contextos diferentes; efeitos colaterais difíceis de rastrear | Encapsulate Variable, Split Variable, Change Reference to Value, Separate Query from Modifier |
| **Comments (deodorant)** | Comentário que mascara código ruim em vez de explicar "por quê" | Bloco de comentário antes de código confuso; se remover comentário, código fica incompreensível | Extract Function, Change Function Declaration (tornar código auto-explicativo) |

**Cross-reference:** Para técnicas de refatoração completas, ver [mestre-freire/reference.md](../mestre-freire/reference.md) §Catálogo de Refatorações e §Mapeamento Smell → Refatoração.
