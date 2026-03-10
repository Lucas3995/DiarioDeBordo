# Catálogo de Padrões de Design (GoF)

23 padrões organizados em 3 categorias. Para cada: intenção, aplicabilidade, consequências e padrões relacionados.

*Fonte: Gamma, Helm, Johnson, Vlissides — "Design Patterns: Elements of Reusable Object-Oriented Software" (1995)*

---

## 1. Padrões de Criação

Abstraem o processo de instanciação — tornam o sistema independente de como objetos são criados, compostos e representados.

### Abstract Factory (Fábrica Abstrata)

**Intenção:** Fornecer interface para criar famílias de objetos relacionados sem especificar classes concretas.

**Aplicabilidade:**
- Sistema deve ser independente de como seus produtos são criados
- Sistema deve ser configurado com uma entre várias famílias de produtos
- Produtos de uma família devem ser usados juntos e essa restrição precisa ser garantida

**Consequências:**
- ✅ Isola classes concretas — nomes nunca aparecem no código cliente
- ✅ Facilita trocar famílias inteiras de produtos
- ❌ Difícil suportar novos tipos de produtos (requer alterar interface da factory)

**Relacionados:** Factory Method (frequentemente usado para implementar), Prototype (alternativa), Singleton (factory muitas vezes é singleton)

---

### Builder (Construtor)

**Intenção:** Separar a construção de objeto complexo da sua representação; mesmo processo de construção cria representações diferentes.

**Aplicabilidade:**
- Algoritmo de criação deve ser independente das partes e de como são montadas
- Processo de construção deve permitir diferentes representações
- Objeto tem muitos parâmetros opcionais (evitar telescoping constructor)

**Consequências:**
- ✅ Permite variar representação interna do produto
- ✅ Isola código de construção e representação
- ✅ Controle fino sobre o processo de construção
- ❌ Adiciona complexidade (uma classe Builder por representação)

**Relacionados:** Abstract Factory (similar, mas Builder constrói passo a passo), Composite (frequentemente o que é construído)

---

### Factory Method (Método Fábrica)

**Intenção:** Definir interface para criar objeto, mas deixar subclasses decidirem qual classe instanciar.

**Aplicabilidade:**
- Classe não consegue antecipar o tipo de objeto a criar
- Classe quer que subclasses especifiquem os objetos que cria
- Classes delegam responsabilidade a uma entre várias subclasses auxiliares

**Consequências:**
- ✅ Elimina necessidade de amarrar código a classes concretas
- ✅ Conecta hierarquias paralelas de classes
- ❌ Requer subclasses para cada tipo — pode gerar hierarquias profundas

**Relacionados:** Abstract Factory (frequentemente implementado com Factory Methods), Template Method (Factory Methods frequentemente chamados dentro de Template Methods), Prototype (alternativa que não requer subclasses)

---

### Prototype (Protótipo)

**Intenção:** Especificar tipos usando instância protótipo; criar novos objetos copiando esse protótipo.

**Aplicabilidade:**
- Classes a instanciar são determinadas em runtime
- Evitar hierarquia paralela de factory classes
- Quando instâncias de classe podem ter poucos estados diferentes — clonar protótipos pré-configurados é mais conveniente

**Consequências:**
- ✅ Adicionar/remover produtos em runtime
- ✅ Reduz número de subclasses
- ✅ Configurar aplicação dinamicamente com classes
- ❌ Implementar Clone() pode ser complexo (referências circulares, deep vs shallow copy)

**Relacionados:** Abstract Factory (pode usar Prototype para criar produtos), Composite e Decorator (frequentemente beneficiam de Prototype)

---

### Singleton

**Intenção:** Garantir que classe tenha exatamente uma instância; fornecer ponto de acesso global.

**Aplicabilidade:**
- Deve existir exatamente uma instância de uma classe, acessível de ponto conhecido
- A instância única deve ser extensível por subclasses, e clientes devem poder usar instância estendida sem modificar código

**Consequências:**
- ✅ Acesso controlado à instância única
- ✅ Espaço de nomes reduzido (evita variáveis globais)
- ✅ Permite número variável de instâncias (se necessário)
- ❌ Dificulta testes (estado global compartilhado)
- ❌ Pode mascarar dependências (acoplamento oculto)

**Relacionados:** Abstract Factory, Builder e Prototype podem ser implementados como Singleton

---

## 2. Padrões Estruturais

Descrevem como compor classes e objetos para formar estruturas maiores — garantem flexibilidade mantendo a estrutura eficiente.

### Adapter (Adaptador)

**Intenção:** Converter interface de classe em outra esperada pelos clientes; permitir que classes com interfaces incompatíveis trabalhem juntas.

**Aplicabilidade:**
- Quer usar classe existente mas sua interface não corresponde à necessária
- Quer criar classe reutilizável que coopera com classes com interfaces incompatíveis
- (Adapter de objeto) Precisa usar várias subclasses existentes mas impraticável adaptar cada uma por subclassing

**Consequências:**
- ✅ Reutiliza classes existentes sem modificá-las
- ✅ (Objeto) Adapter pode trabalhar com múltiplas adaptees
- ❌ (Classe) Não funciona quando precisa adaptar subclasses da Adaptee

**Relacionados:** Bridge (similar estrutura, propósito diferente — Bridge separa interface de implementação; Adapter faz interfaces existentes trabalharem juntas), Decorator (acrescenta funcionalidade, Adapter muda interface), Proxy (mesma interface, Adapter muda)

---

### Bridge (Ponte)

**Intenção:** Separar abstração da implementação para que ambas possam variar independentemente.

**Aplicabilidade:**
- Evitar ligação permanente entre abstração e implementação
- Tanto abstração quanto implementação devem ser extensíveis por subclasses
- Mudanças na implementação não devem impactar código cliente
- Multiplicação excessiva de classes (combinação abstração × implementação)

**Consequências:**
- ✅ Desacopla interface de implementação
- ✅ Extensibilidade independente em ambos os lados
- ✅ Oculta detalhes de implementação dos clientes
- ❌ Adiciona nível de indireção

**Relacionados:** Abstract Factory (pode criar e configurar Bridge), Adapter (entre classes existentes; Bridge é projetado desde o início)

---

### Composite (Composto)

**Intenção:** Compor objetos em estruturas de árvore para representar hierarquias parte-todo; tratar individual e composto uniformemente.

**Aplicabilidade:**
- Representar hierarquias parte-todo de objetos
- Clientes devem poder tratar objetos individuais e composições uniformemente

**Consequências:**
- ✅ Define hierarquias de objetos simples e compostos recursivamente
- ✅ Simplifica o código cliente (trata tudo uniformemente)
- ✅ Facilita adicionar novos tipos de componentes
- ❌ Pode tornar design generalizado demais — difícil restringir componentes

**Relacionados:** Chain of Responsibility (frequentemente via link pai do Composite), Decorator (frequentemente usado com Composite), Iterator (percorrer), Visitor (operações sobre estrutura), Flyweight (compartilhar componentes)

---

### Decorator (Decorador)

**Intenção:** Acoplar responsabilidades adicionais a objeto dinamicamente; alternativa flexível a subclasses para estender funcionalidade.

**Aplicabilidade:**
- Adicionar responsabilidades a objetos individuais de forma dinâmica e transparente
- Para responsabilidades que podem ser retiradas
- Quando extensão por subclasses é impraticável (explosão de subclasses)

**Consequências:**
- ✅ Mais flexível que herança estática
- ✅ Evita classes sobrecarregadas de features no topo da hierarquia
- ❌ Decorator e componente não são idênticos (identidade de objeto muda)
- ❌ Muitos pequenos objetos — design pode ser difícil de depurar

**Relacionados:** Adapter (muda interface, Decorator muda responsabilidades), Composite (Decorator é Composite degenerado com um único componente), Strategy (Decorator muda pele, Strategy muda intestino)

---

### Façade (Fachada)

**Intenção:** Fornecer interface unificada para conjunto de interfaces em subsistema; define interface de nível mais alto.

**Aplicabilidade:**
- Fornecer interface simples para subsistema complexo
- Existem muitas dependências entre clientes e classes de implementação
- Quer organizar subsistemas em camadas

**Consequências:**
- ✅ Protege clientes de componentes do subsistema (menos objetos para lidar)
- ✅ Promove acoplamento fraco entre subsistema e clientes
- ✅ Não impede acesso direto quando necessário
- ❌ Pode se tornar god object se acumular responsabilidades

**Relacionados:** Abstract Factory (criar objetos do subsistema de forma independente), Mediator (abstrai comunicação entre objetos existentes; Façade cria interface nova), Singleton (Façade normalmente é Singleton)

---

### Flyweight (Peso-Leve)

**Intenção:** Usar compartilhamento para suportar grande quantidade de objetos de granulação fina eficientemente.

**Aplicabilidade:**
- Aplicação usa grande número de objetos
- Custos de armazenamento são altos pela quantidade
- Grande parte do estado pode ser extrínseco (movido para fora)
- Muitos grupos de objetos podem ser substituídos por poucos compartilhados

**Consequências:**
- ✅ Redução significativa de memória (troca armazenamento por tempo de computação)
- ❌ O estado extrínseco precisa ser calculado/passado — pode adicionar overhead

**Relacionados:** Composite (frequentemente combinado — árvores com nós-folha Flyweight), State e Strategy (frequentemente implementados como Flyweights)

---

### Proxy (Procurador)

**Intenção:** Fornecer substituto ou placeholder para controlar acesso a outro objeto.

**Aplicabilidade:**
- **Remote Proxy:** Representante local de objeto em espaço de endereçamento diferente
- **Virtual Proxy:** Cria objetos caros sob demanda (lazy loading)
- **Protection Proxy:** Controla acesso ao objeto original (permissões)
- **Smart Reference:** Substituto que executa ações adicionais no acesso (contagem de referências, carregamento lazy, lock)

**Consequências:**
- ✅ Nível de indireção no acesso — permite otimizações transparentes
- ✅ Controle de acesso sem modificar objeto real
- ❌ Pode introduzir latência (Remote Proxy)

**Relacionados:** Adapter (interface diferente para adaptee; Proxy fornece mesma interface), Decorator (similar estrutura mas propósito diferente — Decorator adiciona responsabilidades; Proxy controla acesso)

---

## 3. Padrões Comportamentais

Tratam de algoritmos e atribuição de responsabilidades entre objetos — descrevem padrões de comunicação e controle de fluxo.

### Chain of Responsibility (Cadeia de Responsabilidade)

**Intenção:** Evitar acoplar remetente ao recebedor passando requisição por cadeia de handlers até que um a trate.

**Aplicabilidade:**
- Mais de um objeto pode tratar uma requisição e o handler não é conhecido a priori
- Quer enviar requisição a um de vários objetos sem especificar destinatário explicitamente
- Conjunto de objetos que tratam requisição deve ser especificado dinamicamente

**Consequências:**
- ✅ Reduz acoplamento (remetente não conhece quem trata)
- ✅ Flexibilidade para atribuir responsabilidades
- ❌ Nenhuma garantia de que requisição será tratada (pode cair no vazio)

**Relacionados:** Composite (link pai frequentemente serve como cadeia)

---

### Command (Comando)

**Intenção:** Encapsular requisição como objeto para parametrizar clientes, enfileirar, registrar log e suportar undo.

**Aplicabilidade:**
- Parametrizar objetos com ação a executar (callback orientado a objetos)
- Especificar, enfileirar e executar requisições em momentos diferentes
- Suportar undo/redo (Command armazena estado para reverter operação)
- Suportar logging de mudanças para recuperação em caso de crash
- Estruturar sistema em torno de operações de alto nível (transações)

**Consequências:**
- ✅ Desacopla quem invoca de quem executa
- ✅ Commands são objetos first-class — podem ser manipulados e estendidos
- ✅ Facilita adicionar novos Commands sem mudar código existente
- ✅ Permite compor Commands em macro-commands (Composite)
- ❌ Cada comando é uma classe — pode proliferar classes pequenas

**Relacionados:** Composite (macro-commands), Memento (para undo: armazena estado que Command precisa reverter), Prototype (Command que precisa ser copiado)

---

### Interpreter (Intérprete)

**Intenção:** Dada uma linguagem, definir representação para sua gramática e interpretador para interpretar sentenças.

**Aplicabilidade:**
- Gramática é simples (gramáticas complexas requerem parsers, não Interpreter)
- Eficiência não é preocupação crítica

**Consequências:**
- ✅ Fácil mudar e estender a gramática (cada regra é uma classe)
- ✅ Implementar gramática é direto
- ❌ Gramáticas complexas são difíceis de manter (muitas classes)

**Relacionados:** Composite (árvore sintática abstrata é instância de Composite), Flyweight (compartilhar símbolos terminais), Iterator (percorrer estrutura), Visitor (manter interpretação em uma classe, não espalhada nas classes do nó)

---

### Iterator (Iterador)

**Intenção:** Fornecer maneira de acessar elementos de coleção sequencialmente sem expor representação subjacente.

**Aplicabilidade:**
- Acessar conteúdo de coleção sem expor representação interna
- Suportar múltiplas travessias simultâneas da mesma coleção
- Fornecer interface uniforme para percorrer diferentes estruturas (iteração polimórfica)

**Consequências:**
- ✅ Suporta variações na travessia de coleções
- ✅ Simplifica interface da coleção
- ✅ Permite múltiplos percursos simultâneos
- ❌ Pode haver overhead para coleções simples

**Relacionados:** Composite (Iterators frequentemente aplicados a estruturas Composite), Factory Method (Iterators polimórficos usam Factory Methods para instanciar subclasse do Iterator), Memento (capturar estado da iteração)

---

### Mediator (Mediador)

**Intenção:** Definir objeto que encapsula como conjunto de objetos interage; promover acoplamento fraco.

**Aplicabilidade:**
- Conjunto de objetos comunica de formas bem definidas mas complexas
- Reuso de objeto é difícil porque se refere e comunica com muitos outros
- Comportamento distribuído entre várias classes deve ser customizável sem subclassing demais

**Consequências:**
- ✅ Limita subclassing (centraliza comportamento que estaria distribuído)
- ✅ Desacopla colegas (colleagues)
- ✅ Simplifica protocolos entre objetos (many-to-many → one-to-many)
- ❌ Pode tornar-se god object se centralizar lógica demais

**Relacionados:** Façade (similar em abstração, mas Façade é unidirecional — não protocol bidirecional), Observer (colegas geralmente comunicam com Mediator via Observer)

---

### Memento (Memória)

**Intenção:** Capturar e externalizar estado interno de objeto sem violar encapsulamento; restaurar posteriormente.

**Aplicabilidade:**
- Snapshot do estado (ou parte dele) deve ser salvo para restauração posterior
- Interface direta para obter estado exporia detalhes de implementação quebrando encapsulamento

**Consequências:**
- ✅ Preserva fronteiras do encapsulamento
- ✅ Simplifica Originator (não precisa gerenciar versões do estado)
- ❌ Pode ser custoso se Originator precisar copiar grande quantidade de dados
- ❌ Custo oculto de cuidar de Mementos (caretaker não sabe quanto estado é armazenado)

**Relacionados:** Command (usa Mementos para estado necessário para undo), Iterator (Mementos podem capturar estado da iteração)

---

### Observer (Observador)

**Intenção:** Definir dependência um-para-muitos; quando objeto muda estado, todos dependentes são notificados e atualizados.

**Aplicabilidade:**
- Abstração tem dois aspectos, um dependente do outro (encapsular em objetos separados para variar/reusar independentemente)
- Mudança em um objeto requer mudar outros, sem saber quantos
- Objeto deve notificar outros sem suposições sobre quem são (acoplamento fraco)

**Consequências:**
- ✅ Acoplamento abstrato entre Subject e Observer
- ✅ Suporte para comunicação broadcast
- ❌ Atualizações inesperadas e em cascata (difícil rastrear o que desencadeia)

**Relacionados:** Mediator (Mediator encapsula comunicação complexa; Observer distribui comunicação), Singleton (Subject muitas vezes Singleton)

---

### State (Estado)

**Intenção:** Permitir que objeto altere seu comportamento quando estado interno muda; aparentar mudar de classe.

**Aplicabilidade:**
- Comportamento depende de estado e deve mudar em runtime
- Operações contêm condicionais grandes multipartidas que dependem do estado do objeto

**Consequências:**
- ✅ Localiza comportamento específico de estado e particiona para diferentes estados
- ✅ Torna transições de estado explícitas (objetos distintos, não variáveis)
- ✅ Objetos State podem ser compartilhados (se não tiverem variáveis de instância — Flyweight)
- ❌ Aumenta número de classes

**Relacionados:** Flyweight (compartilhar States), Singleton (States frequentemente são Singletons), Strategy (similar estrutura mas propósito diferente: State para transições de estado; Strategy para algoritmos intercambiáveis)

---

### Strategy (Estratégia)

**Intenção:** Definir família de algoritmos, encapsular cada um e torná-los intercambiáveis.

**Aplicabilidade:**
- Várias classes relacionadas diferem apenas no comportamento (configurar com um de vários comportamentos)
- Precisa de variantes de um algoritmo
- Algoritmo usa dados que clientes não devem conhecer
- Classe define muitos comportamentos via condicionais — mover ramos para suas próprias classes Strategy

**Consequências:**
- ✅ Famílias de algoritmos relacionados (hierarquia de Strategies)
- ✅ Alternativa a subclassing (composição vs herança)
- ✅ Elimina condicionais
- ❌ Clientes devem conhecer Strategies diferentes (para escolher)
- ❌ Overhead de comunicação entre Strategy e Context
- ❌ Aumento de número de objetos

**Relacionados:** Flyweight (Strategy objects frequentemente são bons Flyweights), State (similar estrutura, propósito diferente), Template Method (usa herança, Strategy usa composição)

---

### Template Method (Método Modelo)

**Intenção:** Definir esqueleto de algoritmo numa operação, adiando passos para subclasses.

**Aplicabilidade:**
- Implementar partes invariantes de algoritmo uma vez e deixar subclasses implementar comportamento variável
- Partes comuns entre subclasses devem ser fatoradas e localizadas em classe comum (evitar duplicação)
- Controlar extensões de subclasses (Hook Methods — pontos predefinidos para extensão)

**Consequências:**
- ✅ Técnica fundamental para reutilização de código
- ✅ Inversão de controle (Hollywood Principle: "Don't call us, we'll call you")
- ❌ Usa herança (menos flexível que composição em runtime)

**Relacionados:** Factory Method (frequentemente chamado por Template Methods), Strategy (Template Methods usam herança; Strategies usam composição/delegação)

---

### Visitor (Visitante)

**Intenção:** Representar operação a ser executada sobre elementos de estrutura; definir novas operações sem alterar classes dos elementos.

**Aplicabilidade:**
- Estrutura de objetos contém muitas classes com interfaces distintas e quero executar operações que dependem do tipo concreto
- Muitas operações distintas e não-relacionadas precisam ser executadas sobre a estrutura (e não quero "poluir" as classes com essas operações)
- Classes da estrutura raramente mudam, mas frequentemente quero definir novas operações

**Consequências:**
- ✅ Facilita adicionar novas operações (novo Visitor = nova operação)
- ✅ Visitor acumula estado durante percurso (sem variáveis globais)
- ❌ Dificulta adicionar novos tipos de elementos (cada Visitor precisa de novo método)
- ❌ Pode quebrar encapsulamento (Visitor pode precisar de acesso a dados internos)

**Relacionados:** Composite (Visitor pode operar sobre estrutura definida por Composite), Interpreter (Visitor pode substituir interpretação)
