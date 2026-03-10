# Seleção de Padrão por Problema

Tabela invertida: dado o problema ou causa de reformulação, quais padrões aplicar.

*Baseado na Tabela 1.2 (GoF, 1995) — "7 causas comuns de reformulação" e mapeamento problema→padrão.*

---

## 7 Causas de Reformulação → Padrões

| # | Causa de Reformulação | Descrição | Padrões Recomendados |
|---|----------------------|-----------|---------------------|
| 1 | **Criação acoplada a classe concreta** | Nomes de classes concretas hardcoded; mudar implementação exige mudar instanciação | Abstract Factory, Factory Method, Prototype |
| 2 | **Dependência de operações específicas** | Requisição só pode ser satisfeita de uma forma; impossível variar como tratar requisição | Chain of Responsibility, Command |
| 3 | **Dependência de plataforma** | Código acoplado a API/SO específico; difícil portar | Abstract Factory, Bridge |
| 4 | **Dependência de representação/implementação** | Clientes precisam saber como objeto é representado, armazenado ou implementado | Abstract Factory, Bridge, Memento, Proxy |
| 5 | **Dependências algorítmicas** | Algoritmos são estendidos, otimizados ou substituídos durante desenvolvimento; classes dependentes forçadas a mudar | Builder, Iterator, Strategy, Template Method, Visitor |
| 6 | **Acoplamento forte** | Classes fortemente acopladas difíceis de reusar isoladamente; sistema monolítico | Abstract Factory, Bridge, Chain of Responsibility, Command, Façade, Mediator, Observer |
| 7 | **Extensão via subclasses explosivas** | Herança exige conhecimento profundo da classe pai; explosão combinatória de subclasses | Bridge, Chain of Responsibility, Composite, Decorator, Observer, Strategy |

---

## Mapeamento Problema → Padrão

| Problema / Necessidade | Padrão Primário | Alternativa | Evitar |
|------------------------|----------------|-------------|--------|
| Múltiplos algoritmos intercambiáveis em runtime | **Strategy** | Template Method (se compile-time) | if/else ou switch por tipo |
| Objeto muda comportamento conforme estado | **State** | Strategy (se não há transições) | Condicionais baseados em enum |
| Criar famílias de objetos relacionados | **Abstract Factory** | Builder (se construção complexa) | new ConcreteClass() espalhado |
| Construir objeto complexo passo a passo | **Builder** | Abstract Factory (se família) | Telescoping constructor |
| Criar objeto sem conhecer classe concreta | **Factory Method** | Prototype (se evitar subclasses) | new hardcoded |
| Adicionar funcionalidade sem subclasses | **Decorator** | Strategy (se comportamento, não wrapper) | Herança profunda |
| Estrutura hierárquica parte-todo | **Composite** | - | Arrays flat para dados hierárquicos |
| Operações sobre estrutura sem modificar elementos | **Visitor** | Iterator + polimorfismo | Métodos em cada classe do elemento |
| Encapsular requisição como objeto (undo, log, queue) | **Command** | Memento (para estado do undo) | Callbacks soltas |
| Notificar múltiplos dependentes de mudança | **Observer** | Mediator (se comunicação complexa) | Polling |
| Reduzir acoplamento entre subsistema e clientes | **Façade** | Mediator (se bidirecional) | Clientes acessando N classes internas |
| Converter interface incompatível | **Adapter** | Bridge (se projetado desde o início) | Modificar classe existente |
| Separar abstração de implementação | **Bridge** | Adapter (se post-hoc) | Herança que mistura os dois |
| Controlar acesso a objeto | **Proxy** | Decorator (se adicionar funcionalidade) | Checks manuais espalhados |
| Evitar acoplar remetente a receptor | **Chain of Responsibility** | Mediator | if/else com tipos de handler |
| Encapsular interação entre objetos | **Mediator** | Observer (se simples) | Referências diretas entre pares |
| Salvar/restaurar estado sem violar encapsulamento | **Memento** | Command (se combinar com undo) | Getter/setter público do estado |
| Compartilhar objetos granulares eficientemente | **Flyweight** | - | Instanciar milhares de objetos idênticos |
| Garantir instância única | **Singleton** | - | Variável global |
| Definir esqueleto de algoritmo com passos variáveis | **Template Method** | Strategy (se composição preferida) | Duplicar algoritmo em subclasses |
| Definir gramática e interpretá-la | **Interpreter** | Visitor (para operações mais complexas) | Parser completo para gramáticas simples |
| Percorrer coleção sem expor estrutura | **Iterator** | Visitor (se operar, não apenas percorrer) | Índice manual com for |

---

## Decisão Rápida: Composição vs Herança

| Critério | Composição (objeto) | Herança (classe) |
|----------|---------------------|-------------------|
| Variação em runtime? | ✅ Sim → Strategy, State, Decorator | ❌ Não → Template Method, Factory Method |
| Flexibilidade futura? | ✅ Mais flexível | ❌ Mais rígido |
| Encapsulamento? | ✅ Caixa-preta | ❌ Caixa-branca (expõe internos do pai) |
| Simplicidade? | ❌ Mais objetos | ✅ Menos objetos |
| Reuso de implementação? | ❌ Precisa de delegação explícita | ✅ Herda implementação diretamente |

**Regra geral:** Preferir composição; usar herança quando a relação "é-um" é genuína e a variação é em compile-time.

---

## Combinações Frequentes

| Combinação | Cenário |
|------------|---------|
| Composite + Iterator + Visitor | Estruturas hierárquicas com percurso e operações variadas |
| Abstract Factory + Singleton | Factory única que cria famílias de objetos |
| Command + Memento | Encapsular requisição com suporte a undo via estado salvo |
| Strategy + Factory Method | Criar estratégia apropriada via factory |
| Observer + Mediator | Comunicação complexa entre objetos desacoplados |
| Decorator + Composite | Adicionar responsabilidades a toda uma estrutura hierárquica |
| State + Singleton | Estados compartilhados entre contextos (se stateless) |
| Bridge + Abstract Factory | Criar implementações abstraídas via factory |
