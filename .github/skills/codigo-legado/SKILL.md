---
name: codigo-legado
description: Workflow para alterar código legado com segurança — Legacy Code Change Algorithm (Feathers). Identifica pontos de mudança, encontra pontos de teste, quebra dependências usando catálogo de 25 técnicas, escreve characterization tests e faz mudanças com rede de segurança. Usar quando o código não tem testes suficientes, tem dependências ocultas ou classes difíceis de instanciar em test harness.
---

# Código Legado — Alteração Segura

Workflow para alterar **código legado** (código sem testes) com segurança. O foco é **colocar código sob teste antes de alterá-lo** usando seams, dependency-breaking e characterization tests.

**Definição operacional:** Código legado é **código sem testes** — independente de idade, qualidade ou tecnologia. Sem testes, qualquer mudança é arriscada.

*Definições de termos: [Glossário Unificado](../../reference/glossario-unificado.md)*
*Fonte: Working Effectively with Legacy Code — Michael C. Feathers (2004)*

---

## Princípios

- **Cover and Modify** (com testes) é sempre preferível a **Edit and Pray** (sem testes)
- O objetivo não é refatorar tudo — é criar **rede de segurança local** suficiente para a mudança necessária
- **Characterization tests** documentam comportamento existente (o que o código faz), não comportamento desejado (o que deveria fazer)
- **Feedback rápido** é essencial — lag time entre mudança e feedback deve ser minimizado (objetivo: < 10 segundos)

---

## Restrições obrigatórias

- **Não refatorar sem testes:** Nunca alterar estrutura de código legado sem antes ter characterization tests cobrindo o trecho afetado.
- **Não alterar comportamento durante dependency-breaking:** Ao quebrar dependências, o único objetivo é tornar o código testável — não melhorar design (isso vem depois, via mestre-freire).
- **Passos conservadores primeiro:** Quando não há testes, usar técnicas conservadoras (Sprout/Wrap) antes de reestruturar.
- **Preserve Signatures:** Ao manipular código sem cobertura, manter assinaturas de métodos intactas para reduzir risco.

---

## Workflow — Legacy Code Change Algorithm

### Fase 1: Identificar pontos de mudança

1. Localizar método/classe a alterar
2. Confirmar: "É realmente aqui que a mudança precisa acontecer?"
3. Mapear **efeitos**: se eu mudar X, o que mais é afetado? (effect propagation)

### Fase 2: Encontrar pontos de teste

1. Posso testar o ponto de mudança diretamente?
2. Senão, existe classe/método que o chama e que posso testar?
3. Senão, usar analysis de propagação de efeitos para encontrar ponto de teste mais próximo
4. **Resultado:** um ou mais locais onde posso escrever testes que cubram a mudança

### Fase 3: Quebrar dependências

1. Tentar instanciar classe em test harness
2. Se falhar, analisar o erro — consultar [reference.md](reference.md) §Catálogo de técnicas:
   - Constructor com argumento não-instanciável → **Parameterize Constructor**
   - Chamada a global/static → **Encapsulate Global References** ou **Extract and Override**
   - Dependência oculta → **Extract Interface**
   - Classe grande demais → **Break Out Method Object**
3. Escolher tipo de seam (preferir **object seam** em linguagens OO)
4. Repetir até código compilar e rodar em test harness

**Os 3 tipos de seam:**

| Tipo | Mecanismo | Enabling Point | Quando preferir |
|------|-----------|----------------|-----------------|
| **Object Seam** | Polimorfismo, override, DI | Criação do objeto (injeção) | ✅ Sempre em OO — mais explícito e mantível |
| **Link Seam** | Substituição em link-time | Build script, classpath | Quando object seam é impossível |
| **Preprocessing Seam** | Macros, #define, compilação condicional | Diretiva de preprocessador | Último recurso — menos visível |

### Fase 4: Escrever characterization tests

1. Escrever teste que chama o código como está
2. Colocar asserção com valor esperado **qualquer**
3. Rodar teste → ver valor real na falha
4. Ajustar asserção para valor real → teste passa → **comportamento documentado**
5. Repetir para cenários relevantes (fluxo feliz + exceções)

**Characterization test ≠ test prescritivo:**
- Test prescritivo: "O código **deveria** retornar X" (define comportamento novo)
- Characterization test: "O código **retorna** X" (documenta comportamento existente)

### Fase 5: Fazer mudança + refatorar

1. Com testes verdes cobrindo o trecho, aplicar a mudança desejada
2. Para features novas, usar TDD (Red-Green-Refactor)
3. Após mudança funcional, refatorar com segurança (invocar mestre-freire se necessário)
4. Rodar testes após cada passo

---

## Técnicas conservadoras para adicionar features

Quando o código não tem testes e é arriscado reestruturar, usar estas técnicas para **adicionar código novo sem tocar no existente**:

| Técnica | Quando usar | Como funciona |
|---------|-------------|---------------|
| **Sprout Method** | Feature nova é sequência clara e isolável | Extrair novo código para método separado; testar isoladamente; chamar do ponto original |
| **Sprout Class** | Não consegue instanciar classe original em test harness | Criar classe nova para feature; testar normalmente; usar da classe original |
| **Wrap Method** | Feature é antes/depois de código existente | Renomear método original; criar novo método com nome original que chama original + novo código |
| **Wrap Class** | Adicionar comportamento a muitos call sites (padrão Decorator) | Criar classe wrapper que implementa mesma interface; delega ao original + adiciona comportamento |

**Regra:** Sprout/Wrap são **transitórios** — após cobrir com testes, refatorar para design adequado.

---

## Práticas de segurança (sem testes)

| Prática | Descrição |
|---------|-----------|
| **Hyperaware Editing** | Tela dividida: código original + mudanças; máxima atenção |
| **Single-Goal Editing** | Apenas uma mudança por vez; compilar e testar após cada |
| **Preserve Signatures** | Manter assinaturas de métodos intactas; reduz risco de quebra |
| **Lean on Compiler** | Usar type-checking como validação; erros de compilação são feedback |

---

## Sensing vs Separation

Duas razões distintas para quebrar dependência — mesma técnica pode servir ambas:

| Conceito | Propósito | Mecanismo |
|----------|-----------|-----------|
| **Sensing** | Acessar valores que o código computa (para asserções no teste) | Fake objects com interface de teste (lado adicional que expõe dados) |
| **Separation** | Isolar código para rodar em test harness (sem I/O, DB, rede) | Mocks/stubs que substituem colaboradores externos |

---

## Índice rápido (reference.md)

| Precisa de… | Secção no reference |
|-------------|---------------------|
| Qual técnica de dependency-breaking usar | §1 Decision tree |
| Detalhes de uma técnica específica | §2 Catálogo de 25 técnicas |
| Como modelar seams num componente | §3 Template Seam Analysis |
| Padrão Fake Objects (two-sides) | §4 Fake Objects |
| Análise de propagação de efeitos | §5 Effect Propagation |

---

## Integração com outras skills

| Skill | Relação |
|-------|---------|
| **mestre-freire** | Após cobrir com characterization tests, invocar mestre-freire para refactoring seguro; esta skill fornece a pré-fase "Seam Analysis" |
| **batedor-de-codigos** | Detecta smells mas não lida com ausência de testes; esta skill complementa colocando código sob teste primeiro |
| **quadro-de-recompensas** | Characterization tests seguem padrão AAA; consultar quadro para boas práticas de teste |
| **padroes-de-design** | Após obter cobertura e refatorar, usar padrões GoF para melhorar design |
| **mercenario** | Após cobrir trecho, mercenario implementa features novas com rede de segurança |
