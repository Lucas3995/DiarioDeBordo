# Código Legado — Referência de Técnicas

Catálogo completo de **dependency-breaking techniques** (Feathers, 2004) e ferramentas de análise para trabalhar com código legado.

---

## §1 Decision Tree — Qual técnica usar

```
O código não compila em test harness?
├─ Erro no constructor?
│   ├─ Argumento não-instanciável → Parameterize Constructor
│   ├─ Construtor faz I/O / side-effect → Supersede Instance Variable
│   └─ Muitos argumentos complexos → Subclass and Override Method
├─ Dependência de global/static?
│   ├─ Global mutável → Encapsulate Global References
│   ├─ Singleton → Introduce Static Setter (temporário)
│   └─ Static method → Extract and Override Call
├─ Método privado que preciso testar?
│   ├─ OO puro → Subclass and Override Method
│   └─ Tornar testável → Extract and Override Call
├─ Classe depende de framework (HTTP, DB)?
│   ├─ Interface disponível → Extract Interface + Fake
│   └─ Classe concreta selada → Adapt Parameter / Skin and Wrap
└─ Classe/método muito grande?
    ├─ Método gigante → Break Out Method Object
    └─ Feature entrelaçada → Extract Method (com cobertura prévia)

O código compila mas não consigo fazer asserção (sensing)?
├─ Return value acessível → Asserção direta
├─ Valor escondido em side-effect → Extract and Override Call + Sensing Variable
└─ Comunicação com colaborador → Mock / Fake with sensing interface
```

---

## §2 Catálogo de 25 Dependency-Breaking Techniques

Cada técnica resolve **um tipo específico de obstáculo** à testabilidade. As 10 mais frequentes estão detalhadas; as demais em formato tabular.

---

### Técnicas detalhadas (10 mais frequentes)

#### 1. Extract Interface

**Obstáculo:** Classe depende de tipo concreto que é difícil de instanciar.
**Mecanismo:** Criar interface contendo os métodos usados pelo client; original implementa; em teste, usar fake.
**Passos:**
1. Identificar métodos da dependência que o client usa
2. Criar interface com esses métodos
3. Classe original implementa interface
4. Client passa a depender da interface (type declaration)
5. Em teste, injetar fake que implementa interface

**Risco:** Mínimo — only types mudam.
**Quando NÃO usar:** Quando classe tem poucos métodos e Parameterize Constructor é suficiente.

---

#### 2. Parameterize Constructor

**Obstáculo:** Constructor cria dependência internamente (new concreto).
**Mecanismo:** Adicionar parâmetro ao constructor para injetar dependência; manter constructor original delegando ao novo.
**Passos:**
1. Identificar criação interna de dependência
2. Criar novo constructor que aceita dependência como parâmetro
3. Constructor original chama novo constructor passando criação padrão
4. Em teste, usar novo constructor com fake

**Risco:** Mínimo — constructor original preservado (Preserve Signatures).
**Nota:** Técnica mais comum e segura; usar como primeira tentativa.

---

#### 3. Subclass and Override Method

**Obstáculo:** Método faz algo que impede teste (I/O, rede, DB) ou método protegido/virtual precisa ser isolado.
**Mecanismo:** Criar subclasse de teste que faz override do método problemático.
**Passos:**
1. Identificar método problemático
2. Torná-lo virtual/protected (se necessário)
3. Criar subclasse no teste (Testing Subclass)
4. Override retorna valor controlado ou registra chamada (sensing)
5. Instanciar subclasse no teste em vez da classe original

**Risco:** Baixo — mas requer que linguagem suporte herança/override.
**Nota:** Muito usado em Java/C#; em TypeScript, usar object seam via DI.

---

#### 4. Extract and Override Call

**Obstáculo:** Método contém chamada a static, global ou método que faz side-effect, misturado com lógica.
**Mecanismo:** Extrair a chamada para método separado; override em testing subclass.
**Passos:**
1. Identificar chamada problemática no meio do método
2. Extrair para método virtual com nome descritivo
3. Em testing subclass, override retorna valor fixo
4. Testar classe via testing subclass

**Risco:** Baixo.
**Nota:** Combinável com Subclass and Override Method.

---

#### 5. Encapsulate Global References

**Obstáculo:** Código depende de variáveis globais ou static mutáveis.
**Mecanismo:** Mover globais para classe dedicada; acessar via instância; em teste, substituir instância.
**Passos:**
1. Identificar todas as globais usadas pelo trecho
2. Criar classe que encapsula essas globais como membros de instância
3. Alterar acessos para usar instância da classe
4. Em teste, criar instância com valores controlados

**Risco:** Moderado — muitos call sites podem precisar de ajuste.

---

#### 6. Adapt Parameter

**Obstáculo:** Parâmetro de método é tipo complexo do framework (HttpContext, DbConnection, etc.) difícil de instanciar.
**Mecanismo:** Criar interface/wrapper minimalista que expõe apenas o que o método precisa.
**Passos:**
1. Identificar quais membros do parâmetro complexo são usados
2. Criar interface com apenas esses membros
3. Criar adapter que implementa interface delegando ao tipo real
4. Método passa a aceitar interface; em teste, usar fake

**Risco:** Baixo — similar a Extract Interface mas focado em parâmetros.

---

#### 7. Break Out Method Object

**Obstáculo:** Método muito longo/complexo para testar, com muitas variáveis locais.
**Mecanismo:** Transformar método em classe — variáveis locais viram campos; corpo vira método run/execute.
**Passos:**
1. Criar classe com nome descritivo para o que o método faz
2. Copiar variáveis locais como campos da classe
3. Copiar corpo do método para método run() da classe
4. Constructor recebe referência ao objeto original (para acessar campos)
5. No original, delegar para nova classe
6. Agora pode testar e refatorar a nova classe isoladamente

**Risco:** Baixo — técnica de IDE (automated refactoring).
**Nota:** Após extrair, refatorar internamente a nova classe com testes.

---

#### 8. Introduce Static Setter

**Obstáculo:** Singleton com constructor privado impede substituição em teste.
**Mecanismo:** Adicionar static setter que permite trocar instância em teste.
**Passos:**
1. Adicionar método estático que aceita instância (ou null para reset)
2. Em teste: set fake antes, reset após (tearDown)
3. Garantir que setter só é usado em testes

**Risco:** Moderado — enfraquece encapsulamento do singleton.
**Nota:** Técnica **temporária** — refatorar singleton para DI quando houver cobertura.

---

#### 9. Skin and Wrap the API

**Obstáculo:** Dependência de API de terceiro que não pode ser modificada (sealed class, framework).
**Mecanismo:** Criar wrapper fino (skin) ao redor da API; client depende do wrapper; em teste, substituir wrapper.
**Passos:**
1. Identificar métodos da API usados pelo client
2. Criar classe/interface wrapper com mesmos métodos
3. Wrapper delega ao real
4. Client usa wrapper em vez de API diretamente
5. Em teste, usar fake wrapper

**Risco:** Baixo.
**Nota:** Relacionado com padrão Adapter (GoF).

---

#### 10. Replace Function with Function Pointer / Strategy

**Obstáculo:** Em linguagens funcionais ou com first-class functions, chamada direta a função dificulta substituição.
**Mecanismo:** Trocar chamada direta por chamada via function pointer/strategy injetável.
**Passos:**
1. Criar parâmetro (ou campo) do tipo function/delegate
2. Default aponta para implementação real
3. Em teste, injetar implementação fake

**Risco:** Mínimo.
**Nota:** Equivalente funcional de Extract Interface.

---

### Tabela resumo — Todas as 25 técnicas

| # | Técnica | Obstáculo principal | Mecanismo |
|---|---------|---------------------|-----------|
| 1 | **Adapt Parameter** | Parâmetro tipo complexo de framework | Interface minimalista + adapter |
| 2 | **Break Out Method Object** | Método longo demais para testar | Transformar em classe com run() |
| 3 | **Definition Completion** | Tipos incompletos em compilação | Fornecer definição completa para teste |
| 4 | **Encapsulate Global References** | Globais/statics mutáveis | Mover para classe instanciável |
| 5 | **Expose Static Method** | Método usa estado de instância desnecessariamente | Tornar static; testar sem instanciar |
| 6 | **Extract and Override Call** | Chamada problemática no meio do método | Extrair chamada para método virtual |
| 7 | **Extract and Override Factory Method** | Constructor cria dependências internamente | Factory method virtual; override em teste |
| 8 | **Extract and Override Getter** | Campo inicializado em constructor dificulta teste | Getter virtual; override retorna fake |
| 9 | **Extract Implementer** | Classe concreta com lógica pesada é instanciada por muitos | Mover implementação para classe filha |
| 10 | **Extract Interface** | Dependência de tipo concreto | Interface + fake |
| 11 | **Introduce Instance Delegator** | Método estático precisa de override | Criar método de instância que delega ao static |
| 12 | **Introduce Static Setter** | Singleton impede substituição | Static setter para trocar instância em teste |
| 13 | **Link Substitution** | Dependência em link-time | Substituir .o/.dll/.jar no build |
| 14 | **Parameterize Constructor** | Constructor cria dependência internamente | Novo parâmetro para injetar dependência |
| 15 | **Parameterize Method** | Método busca dependência internamente | Adicionar parâmetro para injetar |
| 16 | **Primitivize Parameter** | Parâmetro complexo impossível de instanciar | Converter para tipos primitivos |
| 17 | **Pull Up Feature** | Feature presa em classe concreta pesada | Extrair para superclasse testável |
| 18 | **Push Down Dependency** | Dependência de framework na classe | Mover dependência para subclasse; testar superclasse |
| 19 | **Replace Function with Function Pointer** | Chamada direta a função (linguagens funcionais) | Injetar via function pointer/delegate |
| 20 | **Replace Global Reference with Getter** | Global acessada diretamente | Getter virtual; override em teste |
| 21 | **Skin and Wrap the API** | API de terceiro sealed/não-modificável | Wrapper fino + interface |
| 22 | **Subclass and Override Method** | Método faz I/O, DB, side-effect | Testing subclass com override |
| 23 | **Supersede Instance Variable** | Campo criado no constructor faz side-effect | Setter/método que substitui campo após construção |
| 24 | **Template Redefinition** | Em C++ com templates, dependência de tipo | Template parameter com tipo fake |
| 25 | **Text Redefinition** | Em linguagens interpretadas | Redefinir método/classe em runtime no teste |

---

## §3 Template — Seam Analysis

Modelo para análise de seams num componente antes de quebrá-lo para teste.

```
Componente: [nome da classe/módulo]
Razão da mudança: [o que precisa ser alterado e por quê]

Dependências identificadas:
1. [Dependência] → Tipo: [constructor / global / chamada direta / parâmetro]
   - Instanciável em teste? [sim/não]
   - Se não, obstáculo: [descrição]
   - Técnica candidata: [nome da técnica]
   - Tipo de seam: [object / link / preprocessing]

Pontos de teste:
- [método/ponto onde posso escrever characterization test]
- [método alternativo se o direto não for viável]

Efeitos (se eu mudar X):
- [lista de métodos/classes afetados pela mudança]

Plano de ação:
1. [Técnica X para dependência Y]
2. [Characterization tests para Z]
3. [Mudança propriamente dita]
4. [Refactoring pós-mudança]
```

---

## §4 Fake Objects — Padrão Two-Sides

Fake objects devem ter **dois lados**:

| Lado | Propósito | Quem usa |
|------|-----------|----------|
| **Lado produção** | Métodos da interface implementada; comportamento substitutivo | O código sob teste (via injeção) |
| **Lado teste** | Métodos de inspeção (getLastCall, wasCalledWith, etc.) | O teste, para asserções (sensing) |

**Exemplo conceitual:**
```
FakeMailSender implements IMailSender:
  // Lado produção
  send(to, subject, body):
    lastTo = to        // registra em vez de enviar
    lastSubject = subject
    sendCount++

  // Lado teste (sensing interface)
  getLastTo() → lastTo
  getSendCount() → sendCount
  wasCalledWith(to) → lastTo == to
```

**Regras:**
- Fake **nunca** contém lógica de negócio — apenas registra/retorna
- Sempre implementar interface production; nunca depender do tipo concreto do fake no teste
- Preferir fake simples a framework de mock quando a dependência é estável

---

## §5 Effect Propagation — Análise de Impacto

Quando precisa entender o que uma mudança pode afetar:

1. **Identificar variável/método alterado** — ponto de partida
2. **Rastrear usos diretos** — quem lê a variável / chama o método
3. **Rastrear usos transitivos** — se A muda e B usa A, quem usa B?
4. **Marcar efeitos visíveis** — retorno de método público, escrita em DB/rede, mudança de estado observável
5. **Para cada efeito visível** — esse é um ponto de teste válido?

**Effect sketch:**
```
mudança em X.calculate()
  ↓ afeta
  X.getTotal()
    ↓ usado por
    OrderProcessor.process()
      ↓ retorna
      resultado público ← PONTO DE TESTE ✅
    ↓ usado por
    Report.generate()
      ↓ escreve em
      arquivo/stream ← PONTO DE TESTE (com sensing) ✅
```

**Heurística:** Se a cadeia de efeitos é muito longa sem ponto de teste acessível, é sinal de que classes/métodos precisam ser decompostos (→ invocar mestre-freire após cobertura).
