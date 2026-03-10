# Referência — Mapeamento Achado → Técnica e Regra

Consulta rápida para aplicar refatoração a cada tipo de achado do relatório. O **princípio/ref violada** no achado indica qual regra e técnica usar.

---

## Por categoria de smell (relatório)

| Categoria (relatório) | Smells típicos | Regra principal | Técnicas (tecnicas-conhecidas) |
|----------------------|----------------|------------------|---------------------------------|
| **Bloaters** | Long Method, Large Class, Long Parameter List, Primitive Obsession, Data Clumps | clean-architecture (SRP), ddd (Value Objects) | Extract Method, Extract Class, Replace Data Value with Object, Introduce Parameter Object, Replace Magic Number with Symbolic Constant |
| **OO Abusers** | Switch Statements, Temporary Field, Refused Bequest, Alternative Classes, Feature Envy | clean-architecture (OCP, LSP, ISP), tecnicas-conhecidas | Replace Conditional with Polymorphism, Replace Type Code with State/Strategy, Move Method, Move Field, Extract Interface |
| **Change Preventers** | Divergent Change, Shotgun Surgery, Parallel Inheritance | clean-architecture (SRP, CCP) | Extract Class, Extract Superclass, Move Method/Field |
| **Dispensables** | Duplicate Code, Lazy Class, Data Class, Dead Code, Speculative Generality | ddd (entidades ricas), tecnicas-conhecidas | Extract Method, Inline Class, Move Method to domain, Remove Dead Code |
| **Couplers** | Feature Envy, Inappropriate Intimacy, Message Chains, Middle Man | clean-architecture (DIP), tecnicas-conhecidas | Move Method, Hide Delegate, Remove Middle Man, Introduce Parameter Object |
| **Arch and Struct** | Circular Dependency, God Object, Hard-Coded Dependencies | clean-architecture (camadas, DIP) | Extract Class, Introduce interfaces in domain/application, inject dependencies at composition root |
| **Test Smells** | (Relatório pode listar; mestre-freire **não altera testes**) | — | Nenhuma alteração em ficheiros de teste |

---

## Por princípio violado (campo do achado)

| Princípio/Ref | Ação de refatoração | Onde ver detalhes |
|---------------|---------------------|--------------------|
| **SRP** | Separar responsabilidades em classes/módulos distintos; Extract Class, Extract Method | clean-architecture §3.1, tecnicas-conhecidas §5.2 |
| **OCP** | Introduzir abstrações (interfaces/estratégias); evitar modificar código estável | clean-architecture §3.2, tecnicas-conhecidas §5.4 (Replace Conditional with Polymorphism) |
| **LSP** | Corrigir hierarquias para que subclasses sejam substituíveis; Replace Inheritance with Delegation se Refused Bequest | clean-architecture §3.3, tecnicas-conhecidas §5.6 |
| **ISP** | Segregar interfaces; Extract Interface com métodos usados pelo cliente | clean-architecture §3.4, tecnicas-conhecidas §5.6 |
| **DIP** | Depender de abstrações; interfaces em camadas internas, implementações na borda | clean-architecture §3.5, §5 |
| **Clean Architecture** | Respeitar direção de dependência; mover código para a camada correta (Entidades, Casos de Uso, Adaptadores, Drivers) | clean-architecture §2, §5, §7.2 |
| **DDD** | Value Objects para primitivos agrupados; regras em entidades/agregados; linguagem ubíqua | ddd-domain-driven-design § building blocks, § refatorar |
| **CQRS** | Separar comandos (alteram estado) de queries (só leem); handlers e DTOs distintos | cqrs-command-query-responsibility-segregation §2, §6 |

---

## Ordem sugerida ao tratar achados

1. **Dependências e arquitetura** (Circular Dependency, Hard-Coded Dependencies, Framework Coupling) — estabilizar camadas e injeção.
2. **God Object / Large Class** — extrair classes e mover para camadas corretas.
3. **Long Method / Duplicate Code** — Extract Method, Substitute Algorithm.
4. **Feature Envy / Data Class** — Move Method, enriquecer domínio (DDD).
5. **Primitive Obsession / Data Clumps** — Value Objects, Parameter Object.
6. **Switch Statements / Type Code** — State/Strategy ou polimorfismo.
7. **Dispensables** (Dead Code, Lazy Class, Middle Man) — remover ou consolidar.

---

---

## Refatoração em nível de componente

Técnicas para achados das categorias *Component Smells* e *Arch and Struct* que envolvem reorganização de módulos/componentes. Ver [principios-componentes.md](../../reference/principios-componentes.md) para fundamentos teóricos.

| Achado | Técnica | Detalhes |
|--------|---------|---------|
| **Componente Incoeso (REP)** | Extract Module / Move Class | Mover classes de temas distintos para componentes separados; cada componente deve ter tema/propósito coeso. |
| **Fechamento Comum Violado (CCP)** | Split Module | Separar classes que mudam por razões/momentos diferentes em componentes distintos; evitar rebuild/deploy desnecessário. |
| **Dependência Desnecessária (CRP)** | Extract Interface + Split Module | Extrair interface específica para o consumidor; mover implementação que o consumidor não usa para outro componente. |
| **Dependência Cíclica (DA)** | DIP + Interface em boundary | Introduzir interface no componente mais interno; componente externo implementa a interface; quebra o ciclo. |
| **Instável Depende de Estável (SDP)** | DIP + Extract Interface | Estável expõe apenas abstração; instável depende da abstração, não do concreto. |
| **Estável e Concreto (SAP)** | Extract Interface / Extract Superclass | Converter componente estável em interfaces/classes abstratas; implementações em componentes instáveis. |
| **Split Prematuro / Merge Necessário** | Merge Module | Quando dois componentes sempre mudam juntos pelo mesmo motivo → unificar em um componente. |

---

## Padrão Humble Object

Separar parte testável da não testável quando uma classe é difícil de testar. Ver [principios-componentes.md](../../reference/principios-componentes.md) §5.

| Contexto | Parte testável (extrair) | Humble (mínima) | Exemplo |
|----------|--------------------------|------------------|---------|
| **Apresentador/Visualizador** | `.ts` com lógica: formatação, regras de exibição, estados | `.html` que apenas encaixa dados já tratados | Angular: component.ts (apresentador) + template.html (humble) |
| **Repositório/Gateway de DB** | Repositório com lógica de quais consultas fazer, campos, filtros | DbSet/ORM que gera SQL e executa | .NET: Repository (testável) + DbContext/DbSet (humble) |
| **Service Listener** | Lógica de processamento de dados do serviço | Classes de entrada/saída que apenas convertem formato | Adapter HTTP que converte JSON ↔ tipos de domínio |

**Quando aplicar:** sempre que um achado indicar lógica testável misturada com código de I/O, UI ou driver.

---

## Refatoração para DDD

Técnicas para achados da categoria *Anti-patterns DDD*. Ver [DDD-IA-GUIDE.md](../../../Validados/DDD-IA-GUIDE.md) para fundamentos.

| Achado | Técnica | Detalhes |
|--------|---------|---------|
| **Entidade Anêmica** | Move Method to domain + Enrich Entity | Mover regras de Application Services/Controllers para métodos na Entidade; entidade passa a proteger invariantes via métodos de domínio. |
| **Primitive Obsession no Domínio** | Replace Data Value with Object (Value Object) | Criar VOs imutáveis (CPF, Email, Dinheiro, Endereço) com validação no construtor e igualdade por atributos. |
| **God Service** | Extract Class + Extract Domain Service | Dividir em Domain Services específicos por agregado/contexto; manter stateless; operações multi-entidade. |
| **Repositório com Lógica** | Move Method to domain | Mover condicionais de negócio e cálculos para Entities ou Domain Services; repositório fica apenas com persistência. |
| **Ausência de Bounded Context** | Extract Module por contexto | Separar classes por subdomínio; cada contexto com sua própria linguagem ubíqua e modelos isolados. |
| **Agregado sem Invariantes** | Introduce Aggregate Root | Definir raiz do agregado; todo acesso a entidades internas passa pela raiz; invariantes verificados em métodos da raiz. |

---

## Priorização por impacto

Ordenar achados antes de iniciar a refatoração. Prioridade determina em que ordem os achados são tratados.

| Prioridade | Tipos de achado | Justificativa |
|------------|-----------------|---------------|
| **Alta** | Dependências de camada (Circular Dependency, Hard-Coded Dependencies, Framework Coupling), violações de DIP, ciclos (DA) | Estabilizar camadas e injeção é pré-requisito para as demais refatorações. |
| **Média** | SRP, OCP, LSP, ISP; God Object, Large Class, Entidades Anêmicas, God Service | Melhoram manutenibilidade e expressividade do domínio. |
| **Baixa** | REP, CCP, CRP, SDP, SAP; Main, Humble Object, Limites Parciais; smells de componente finos | Refinamentos de granularidade e organização; impacto menor mas cumulativo. |

---

## Catálogo expandido violação → técnica

| Princípio/Ref | Violação | Ação de refatoração | Padrão / técnica |
|---------------|----------|---------------------|-------------------|
| **DA** | Ciclo entre componentes | Introduzir interface no componente mais interno; quebrar ciclo | DIP + Interface em boundary |
| **Humble Object** | Lógica testável misturada com UI/DB/driver | Separar classe em parte testável + humble | Extract Class; Humble Object pattern |
| **DDD (Entities)** | Entidade anêmica | Mover regras para a entidade | Move Method to domain |
| **DDD (Value Objects)** | Primitive Obsession | Criar Value Objects imutáveis | Replace Data Value with Object |
| **DDD (Aggregates)** | Agregado sem proteção | Definir Aggregate Root com invariantes | Introduce Aggregate Root |
| **DDD (Domain Services)** | God Service | Dividir em Domain Services específicos | Extract Class + Extract Domain Service |
| **DDD (Repositories)** | Repositório com lógica de negócio | Mover lógica para domínio | Move Method to domain |
| **REP** | Componente incoeso | Separar por tema/propósito | Extract Module / Move Class |
| **CCP** | Classes com razões de mudança distintas no mesmo componente | Separar em componentes | Split Module |
| **CRP** | Consumidor depende demais | Extrair interface + dividir | Extract Interface + Split Module |

---

## Comando de testes

Executar o comando de testes do projeto (ex.: frontend `npm run test`, backend `dotnet test`). Não alterar ficheiros de teste. Se o projeto usar outros comandos, seguir `package.json`, solution ou raiz do repositório.

---

## Refactoring para Padrões (GoF)

Quando um smell do relatório mapeia para um padrão GoF, aplicar refactoring **sequencial** — do smell ao padrão, sem saltos.

| Smell | Padrão GoF alvo | Sequência de refactoring |
|-------|-----------------|--------------------------|
| Múltiplos if/else baseados em tipo | **Strategy** | Extract Method → Extract Interface → Replace Conditional with Strategy |
| Switch sobre tipo/enum com comportamento | **State** ou **Strategy** | Extract Method por caso → Extract Class por caso → Introduzir interface |
| Subclasses explosivas (combinatórias) | **Decorator** | Identificar responsabilidades → Extract Class por responsabilidade → Compor via Decorator |
| Criação condicional de objetos | **Factory Method** ou **Abstract Factory** | Extract Method de criação → Parametrizar → Factory |
| Código que atravessa estrutura complexa | **Visitor** | Extract Method por operação → Definir interface Visitor → Accept + Visit |
| Notificação manual entre objetos | **Observer** | Identificar publishers/subscribers → Extract interface → Subscribe/Notify |
| Algoritmo com variações | **Template Method** | Extract Method para passos → Classe base com template → Override em subclasses |
| Requisição que passa por cadeia de verificações | **Chain of Responsibility** | Extract Class por handler → Interface Handler → Montar cadeia |
| Objeto com múltiplos estados e transições | **State** | Extract Class por estado → Interface State → Contexto delega ao estado atual |
| Acesso a subsistema complexo disperso | **Facade** | Identificar operações do cliente → Extract Class Facade → Encapsular subsistema |

**Consulta rápida:** Para lookup problema→padrão(ões), ver [padroes-de-design/selecao-por-problema.md](../padroes-de-design/selecao-por-problema.md).
**Catálogo completo:** Para detalhes de cada padrão (intenção, consequências, relacionados), ver [padroes-de-design/reference.md](../padroes-de-design/reference.md).

---

## Catálogo de Refatorações — Fowler (Refactoring 2nd Ed.)

65 técnicas organizadas em 7 categorias. Para cada técnica: nome, quando usar e smell que resolve.

### Composição de Métodos (11)

| Técnica | Quando usar | Smell que resolve |
|---------|-------------|-------------------|
| **Extract Function** | Fragmento de código pode ter nome significativo | Long Method, Duplicate Code |
| **Inline Function** | Corpo tão claro quanto o nome | Middle Man, indireção desnecessária |
| **Extract Variable** | Expressão complexa difícil de entender | — |
| **Inline Variable** | Variável não diz mais que a expressão | — |
| **Change Function Declaration** | Nome, parâmetros ou retorno imprecisos | Mysterious Name, Long Parameter List |
| **Encapsulate Variable** | Dados acessados amplamente precisam de controle | Data Class, acoplamento |
| **Rename Variable** | Nome não comunica propósito | Mysterious Name |
| **Introduce Parameter Object** | Grupo de parâmetros viajam sempre juntos | Data Clumps, Long Parameter List |
| **Combine Functions into Class** | Grupo de funções opera sobre os mesmos dados | Feature Envy |
| **Combine Functions into Transform** | Dados derivados calculados repetidamente | Duplicate Code |
| **Split Phase** | Código com duas responsabilidades sequenciais | Long Method, Divergent Change |

### Encapsulamento (9)

| Técnica | Quando usar | Smell que resolve |
|---------|-------------|-------------------|
| **Encapsulate Record** | Registro mutável com campos acessados diretamente | Data Class |
| **Encapsulate Collection** | Coleção exposta permite modificação externa | Inappropriate Intimacy |
| **Replace Primitive with Object** | Primitivo que carrega significado de domínio | Primitive Obsession |
| **Replace Temp with Query** | Variável temporária armazena expressão reutilizável | — |
| **Extract Class** | Classe com responsabilidades múltiplas | Large Class, Divergent Change |
| **Inline Class** | Classe faz muito pouco | Lazy Element |
| **Hide Delegate** | Cliente conhece a cadeia de delegação | Message Chains |
| **Remove Middle Man** | Classe apenas repassa chamadas | Middle Man |
| **Substitute Algorithm** | Algoritmo pode ser substituído por versão mais clara | — |

### Mover Features (9)

| Técnica | Quando usar | Smell que resolve |
|---------|-------------|-------------------|
| **Move Function** | Função mais próxima dos dados que usa | Feature Envy |
| **Move Field** | Campo usado mais por outra classe | Feature Envy |
| **Move Statements into Function** | Statements sempre executam com a função | Duplicate Code |
| **Move Statements to Callers** | Callers precisam de variações | Divergent Change |
| **Replace Inline Code with Function Call** | Código duplica lógica de função existente | Duplicate Code |
| **Slide Statements** | Statements relacionados estão distantes | — |
| **Split Loop** | Loop faz múltiplas coisas | Long Method |
| **Replace Loop with Pipeline** | Loop pode ser expresso como pipeline (map/filter/reduce) | Loops |
| **Remove Dead Code** | Código nunca executado | Dead Code |

### Organizar Dados (5)

| Técnica | Quando usar | Smell que resolve |
|---------|-------------|-------------------|
| **Split Variable** | Variável reatribuída para propósitos diferentes | — |
| **Rename Field** | Nome do campo não comunica propósito | Mysterious Name |
| **Replace Derived Variable with Query** | Variável derivada pode ser calculada sob demanda | — |
| **Change Reference to Value** | Referência mutável quando imutabilidade é mais adequada | Mutable Data |
| **Change Value to Reference** | Valor duplicado quando referência compartilhada é necessária | — |

### Simplificar Condicionais (6)

| Técnica | Quando usar | Smell que resolve |
|---------|-------------|-------------------|
| **Decompose Conditional** | If/else com condição e blocos complexos | Long Method, complexidade |
| **Consolidate Conditional Expression** | Múltiplas condições com mesmo resultado | Duplicate Code |
| **Replace Nested Conditional with Guard Clauses** | Condicionais aninhados obscurecem fluxo normal | Long Method |
| **Replace Conditional with Polymorphism** | Switch/if-else baseado em tipo | Repeated Switches |
| **Introduce Special Case** | Verificação null/especial repetida | Duplicate Code, Null checks |
| **Introduce Assertion** | Condição que deve ser verdadeira mas não é óbvia | — |

### Refatorar APIs (10)

| Técnica | Quando usar | Smell que resolve |
|---------|-------------|-------------------|
| **Separate Query from Modifier** | Função com efeito colateral E retorno | Command-Query Separation |
| **Parameterize Function** | Funções similares com valores literais diferentes | Duplicate Code |
| **Remove Flag Argument** | Boolean/flag altera comportamento da função | Long Parameter List |
| **Preserve Whole Object** | Extrair valores de objeto e passá-los individualmente | Data Clumps, Long Parameter List |
| **Replace Parameter with Query** | Parâmetro pode ser obtido pelo receptor | Long Parameter List |
| **Replace Query with Parameter** | Referência indesejada no corpo da função | Dependência indesejada |
| **Remove Setting Method** | Campo deveria ser imutável após criação | Mutable Data |
| **Replace Constructor with Factory Function** | Construtor com limitações (nome fixo, sem polimorfismo) | — |
| **Replace Function with Command** | Função complexa que precisa de undo/queue/compose | Long Method |
| **Replace Command with Function** | Command muito simples para justificar a classe | Lazy Element |

### Herança (11)

| Técnica | Quando usar | Smell que resolve |
|---------|-------------|-------------------|
| **Pull Up Method** | Subclasses têm métodos idênticos | Duplicate Code |
| **Pull Up Field** | Subclasses têm campos idênticos | Duplicate Code |
| **Pull Up Constructor Body** | Construtores de subclasses com código comum | Duplicate Code |
| **Push Down Method** | Método da superclasse usado por apenas uma subclasse | Refused Bequest |
| **Push Down Field** | Campo da superclasse usado por apenas uma subclasse | Refused Bequest |
| **Replace Type Code with Subclasses** | Tipo/enum determina comportamento | Repeated Switches |
| **Remove Subclass** | Subclasse faz muito pouco | Lazy Element |
| **Extract Superclass** | Classes com comportamento/dados comuns sem hierarquia | Duplicate Code |
| **Collapse Hierarchy** | Superclasse e subclasse não diferem significativamente | Lazy Element |
| **Replace Subclass with Delegate** | Herança limitada (só uma dimensão de variação) | — |
| **Replace Superclass with Delegate** | Herança inapropriada (subclasse não é subtipo) | Refused Bequest |

---

## Mapeamento Smell → Refatoração (Fowler)

Tabela cruzada: dado um smell, quais refatorações aplicar. Complementa §Por categoria de smell.

| Smell | Refatorações primárias | Refatorações secundárias |
|-------|----------------------|------------------------|
| **Mysterious Name** | Change Function Declaration, Rename Variable, Rename Field | — |
| **Duplicate Code** | Extract Function, Slide Statements, Pull Up Method | Move Statements into Function |
| **Long Function** | Extract Function, Replace Temp with Query, Decompose Conditional | Replace Function with Command, Split Phase |
| **Long Parameter List** | Replace Parameter with Query, Preserve Whole Object, Introduce Parameter Object | Remove Flag Argument, Combine Functions into Class |
| **Global Data** | Encapsulate Variable | — |
| **Mutable Data** | Encapsulate Variable, Split Variable, Change Reference to Value | Separate Query from Modifier, Remove Setting Method |
| **Divergent Change** | Split Phase, Extract Function, Extract Class | Move Function |
| **Shotgun Surgery** | Move Function, Move Field, Combine Functions into Class | Combine Functions into Transform, Inline Function/Class |
| **Feature Envy** | Move Function, Extract Function | — |
| **Data Clumps** | Extract Class, Introduce Parameter Object, Preserve Whole Object | — |
| **Primitive Obsession** | Replace Primitive with Object, Replace Type Code with Subclasses | Extract Class, Introduce Parameter Object |
| **Repeated Switches** | Replace Conditional with Polymorphism | — |
| **Loops** | Replace Loop with Pipeline | — |
| **Lazy Element** | Inline Function, Inline Class, Collapse Hierarchy | — |
| **Speculative Generality** | Collapse Hierarchy, Inline Function, Inline Class | Change Function Declaration, Remove Dead Code |
| **Temporary Field** | Extract Class, Move Function | Introduce Special Case |
| **Message Chains** | Hide Delegate | Extract Function, Move Function |
| **Middle Man** | Remove Middle Man | Inline Function, Replace Superclass with Delegate |
| **Insider Trading** | Move Function, Move Field, Hide Delegate | Replace Subclass/Superclass with Delegate |
| **Large Class** | Extract Class, Extract Superclass | Replace Type Code with Subclasses |
| **Alternative Classes with Different Interfaces** | Change Function Declaration, Move Function | Extract Superclass |
| **Data Class** | Encapsulate Record, Move Function | Remove Setting Method, Hide Delegate |
| **Refused Bequest** | Push Down Method, Push Down Field | Replace Subclass/Superclass with Delegate |
| **Comments (deodorant)** | Extract Function, Change Function Declaration | Introduce Assertion |

---

## Cadeias de Refactoring — Receitas para Problemas Comuns

Sequências pré-definidas de refactorings para resolver problemas recorrentes. Seguir na ordem indicada.

### 1. Simplificar lógica complexa
**Problema:** Método longo com condicionais aninhados.
**Cadeia:** Decompose Conditional → Extract Function → Move Function

### 2. Remover duplicação entre classes
**Problema:** Código repetido em múltiplas classes.
**Cadeia:** Extract Function → Move Function → Extract Class (se dados acompanham)

### 3. Encapsular dados expostos
**Problema:** Dados mutáveis acessados diretamente por múltiplos consumidores.
**Cadeia:** Encapsulate Variable → Encapsulate Collection → Extract Class

### 4. Substituir condicionais por polimorfismo
**Problema:** Switch/if-else baseado em tipo espalhado pelo código (Repeated Switches).
**Cadeia:** Replace Type Code with Subclasses → Replace Constructor with Factory Function → Replace Conditional with Polymorphism → Pull Up Method (comportamento comum)

### 5. Resolver explosão de parâmetros
**Problema:** Funções com muitos parâmetros, muitos deles viajando juntos.
**Cadeia:** Introduce Parameter Object → Preserve Whole Object → Combine Functions into Class

### 6. Separar responsabilidades (Divergent Change)
**Problema:** Classe muda por razões diferentes.
**Cadeia:** Split Phase (se sequencial) OU Extract Class + Move Function (se paralelo)

### 7. Consolidar lógica espalhada (Shotgun Surgery)
**Problema:** Uma mudança de negócio requer alterações em muitos arquivos.
**Cadeia:** Move Function + Move Field → Combine Functions into Class (ou Transform)

### 8. Limpar herança problemática
**Problema:** Hierarquia de herança com comportamento recusado ou classes vazias.
**Cadeia:** Pull Up / Push Down (Method + Field) → Collapse Hierarchy / Extract Superclass → Replace Subclass with Delegate (se herança fundamentalmente errada)
