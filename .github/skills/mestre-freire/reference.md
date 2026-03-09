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
