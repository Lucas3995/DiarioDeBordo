---
name: padroes-de-design
description: Orienta seleção e aplicação de design patterns (GoF) em contexto de implementação ou refactoring. Recebe problema ou smell, identifica padrão aplicável, avalia consequências e orienta aplicação. Não implementa código nem analisa qualidade — apenas recomenda padrão e caminho. Usar quando mercenario, mestre-freire ou batedor-de-codigos precisarem de guidance sobre qual padrão aplicar a um problema de design.
---

# Padrões de Design — Seleção e Aplicação

Orienta a **seleção e aplicação de design patterns** (Gang of Four) quando um problema de design é identificado durante implementação ou refactoring. O foco é **recomendar o padrão certo para o problema certo** — não implementar código nem analisar qualidade.

*Definições de termos: [Glossário Unificado](../../reference/glossario-unificado.md)*
*Fonte: Padrões de Projetos — Gamma, Helm, Johnson, Vlissides (GoF, 1995)*

---

## Princípios fundantes

Três princípios permeiam todos os 23 padrões e devem guiar qualquer decisão de design:

1. **Programe para uma interface, não para uma implementação** — Declare variáveis usando tipos abstratos; clientes não conhecem classes concretas.
2. **Prefira composição a herança** — Obtenha funcionalidade compondo objetos (caixa-preta) em vez de herdar (caixa-branca); mais flexível em runtime.
3. **Encapsule o conceito que varia** — Identifique o que muda e isole em classes/objetos separados; o resto do design permanece estável.

---

## Restrições obrigatórias

- **Não implementar código:** Recomendar padrão e orientar aplicação — a implementação é responsabilidade do mercenario ou mestre-freire.
- **Não analisar qualidade:** Detectar ausência de padrão ou uso inadequado é responsabilidade do batedor-de-codigos.
- **Pseudocódigo nos exemplos:** Exemplos devem usar pseudocódigo ou ser language-agnostic; nunca amarrar a stack específica.

---

## Fluxo de trabalho

1. **Receber problema ou smell** — Descrição do problema de design (ex.: "múltiplos algoritmos intercambiáveis", "criação acoplada a classe concreta") ou smell detectado pelo batedor-de-codigos.
2. **Identificar aspecto que varia** — Qual conceito precisa mudar independentemente? Algoritmo? Estrutura? Criação? Estado?
3. **Consultar 7 causas de reformulação** — Ver [selecao-por-problema.md](selecao-por-problema.md) para mapear causa→padrão(ões) candidatos.
4. **Avaliar consequências** — Ver [reference.md](reference.md) para trade-offs de cada padrão candidato (flexibilidade vs complexidade, performance vs indireção).
5. **Recomendar padrão** — Indicar padrão, justificar com princípio violado/atendido, descrever estrutura esperada em alto nível.
6. **Orientar aplicação** — Descrever passos de refactoring sequencial para introduzir o padrão (ex.: "extrair interface → criar implementações → injetar via construtor").

---

## Índice rápido

| Precisa de… | Onde consultar |
|-------------|----------------|
| Qual padrão usar para problema X | [selecao-por-problema.md](selecao-por-problema.md) |
| Detalhes de um padrão específico | [reference.md](reference.md) §Catálogo |
| Trade-offs de um padrão | [reference.md](reference.md) §Consequências do padrão |
| Padrões relacionados | [reference.md](reference.md) §Relacionados |
| Princípios de design | §Princípios fundantes (acima) |

---

## Integração com outras skills

| Skill | Relação |
|-------|---------|
| **batedor-de-codigos** | Detecta ausência de padrão ou uso inadequado → invoca esta skill para recomendação |
| **mestre-freire** | Recebe recomendação desta skill → aplica padrão via refactoring seguro |
| **mercenario** | Consulta esta skill durante implementação quando precisa decidir estrutura |
| **codigo-legado** | Após quebrar dependências e obter cobertura, usa esta skill para melhorar design |
| **engenharia-de-software** | Referência teórica complementar (Pressman/Sommerville: design patterns contexto) |

---

## Classificação por escopo

| Escopo | Descrição | Padrões |
|--------|-----------|---------|
| **Classe** | Comportamento fixado em tempo de compilação (herança) | Factory Method, Adapter (classe), Interpreter, Template Method |
| **Objeto** | Comportamento configurável em runtime (composição) | Todos os demais (19 padrões) |

**Preferir padrões de objeto** quando flexibilidade em runtime é necessária — alinhado com o princípio "composição sobre herança".
