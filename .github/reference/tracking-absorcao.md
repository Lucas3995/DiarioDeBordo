# Tracking de Absorção — Recursos → `.github/`

Registro de quais recursos externos foram analisados e incorporados nas skills e referências de `.github/`. Evita retrabalho em futuras evoluções.

---

## `Validados/` → `.github/`

| Recurso | Status | Absorvido em | Detalhes |
|---------|--------|-------------|---------|
| `Validados/ArquiteturaLimpa` | ✅ Absorvido | `.github/reference/principios-componentes.md` | Princípios de componentes (REP, CCP, CRP, DA, SDP, SAP), Humble Object, limites parciais, conceitos complementares |
| `Validados/CleanArch` | ✅ Absorvido | `.github/reference/principios-componentes.md` + enriquecimento de batedor e mestre-freire reference.md | SOLID em nível de componente, coesão, acoplamento, arquitetura gritante |
| `Validados/DDD-IA-GUIDE.md` | ✅ Absorvido | batedor reference.md (§Anti-patterns DDD), mestre-freire reference.md (§Refatoração para DDD), mercenario reference.md (§Building Blocks DDD, §Linguagem Ubíqua) | Entidades, VOs, Agregados, Domain Services, Repositories, Bounded Contexts |
| `Validados/doc akita` | ✅ Absorvido | mercenario reference.md (§Qualidade na construção) | Funções totais, erros tipados, validação explícita, separação de efeitos |
| `Validados/protecao-e-seguranca` | ✅ Absorvido | tradutor SKILL.md (§9 LGPD), tradutor reference.md (§Proteção de Dados), maestro reference.md (§Requisitos LGPD) | Flui como requisitos pela rotina-completa: tradutor identifica → maestro especifica → quadro testa |
| `Validados/quando e para que usar` | ✅ Absorvido | copilot-instructions.md (§Estratégias de mitigação), tradutor SKILL.md (§10 Mitigação IA), tradutor reference.md (§Mitigação de Riscos ao Usar IA) | Estratégias de mitigação (não proibições); catálogo por área de risco |

---

## `/home/belo/.cursor/skills/` → `.github/`

| Recurso | Status | Absorvido em | Detalhes |
|---------|--------|-------------|---------|
| `software-engineering-practice/` | ✅ Absorvido | `.github/skills/engenharia-de-software/` (SKILL.md + reference.md + sommerville-reference.md) | Nova skill completa; Pressman 9th + Sommerville 10th; dialética entre fontes |
| `clean-architecture-analysis/` | ✅ Absorvido | batedor reference.md (§Workflow de Análise 14 etapas), batedor SKILL.md (template expandido com 9 categorias + resumo executivo) | Workflow de 14 etapas adaptado; categorias expandidas |
| `clean-architecture-remediation/` | ✅ Absorvido | mestre-freire reference.md (§Refatoração componente, §Priorização, §Catálogo expandido), mestre-freire SKILL.md (priorização + TDD no fluxo) | Priorização Alta/Média/Baixa; catálogo violação→técnica; ciclo TDD de segurança |

---

## Pressman 9th Ed. (referência bibliográfica)

- **Absorvido em:** `.github/skills/engenharia-de-software/reference.md` — princípios por atividade, técnicas por fase, glossário, mapeamento livro→tópico

## Sommerville 10th Ed. (referência bibliográfica)

- **Absorvido em:** `.github/skills/engenharia-de-software/sommerville-reference.md` — resumo por parte, capítulo→termos, princípios/definições

---

## Arquivos criados nesta evolução

| Arquivo | Tipo |
|---------|------|
| `.github/skills/engenharia-de-software/SKILL.md` | Nova skill |
| `.github/skills/engenharia-de-software/reference.md` | Nova referência |
| `.github/skills/engenharia-de-software/sommerville-reference.md` | Nova referência |
| `.github/reference/principios-componentes.md` | Referência centralizada (pré-existente, enriquecida) |
| `.github/skills/tradutor/reference.md` | Nova referência |
| `.github/reference/glossario-unificado.md` | Nova referência centralizada |
| `.github/reference/tracking-absorcao.md` | Este arquivo |

## Arquivos modificados nesta evolução

| Arquivo | Alteração |
|---------|-----------|
| `.github/skills/batedor-de-codigos/reference.md` | +Component Smells, +Anti-patterns DDD, +Workflow 14 etapas |
| `.github/skills/batedor-de-codigos/SKILL.md` | +9 categorias no template, +resumo executivo, +referências expandidas |
| `.github/skills/mestre-freire/reference.md` | +Refatoração componente, +Humble Object, +DDD, +Priorização, +Catálogo expandido |
| `.github/skills/mestre-freire/SKILL.md` | +Priorização por impacto, +TDD no ciclo, +ref principios-componentes |
| `.github/skills/mercenario/reference.md` | +Building Blocks DDD, +Qualidade, +Linguagem Ubíqua |
| `.github/skills/quadro-de-recompensas/reference.md` | +TDD cycle, +Dublês expandido, +Técnicas avançadas, +a11y tests |
| `.github/skills/maestro/reference.md` | +2 níveis, +Elicitação, +NFRs, +LGPD, +Rastreabilidade, +Linguagem Ubíqua |
| `.github/skills/mestre-freire-angular/reference.md` | +Testes async, +Gestão estado, +Erros/resiliência, +Lazy loading, +Domain Services |
| `.github/skills/tradutor/SKILL.md` | +3 steps (Linguagem Ubíqua, LGPD, Mitigação IA) |
| `.github/copilot-instructions.md` | +Estratégias de mitigação IA, +engenharia-de-software na tabela |

---

## `.github/reference/` (pré-existentes) → Absorção

| Recurso | Status | Absorvido em | Detalhes |
|---------|--------|-------------|----------|
| `.github/reference/guia-angular.txt` | ✅ Absorvido e removido | `.github/instructions/angular-frontend.instructions.md` | Todo conteúdo (estrutura modular, central de ações, a11y, signals, typed forms, ciclo de vida) já formalizado e expandido no instructions. Princípio "estrutura ≠ arquitetura" integrado como §Estrutura de pastas = navegação e pertencimento. Arquivo removido por ser órfão. |
| `.github/reference/principios-componentes.md` | Pré-existente | Enriquecido com conteúdo de `Validados/ArquiteturaLimpa` e `Validados/CleanArch` | Era documento pré-existente; não foi criado nesta evolução, foi enriquecido. |

## Evolução: Estrutura ≠ Arquitetura Angular

| Arquivo | Alteração |
|---------|----------|
| `.github/instructions/angular-frontend.instructions.md` | Reestruturado: +§Estrutura de pastas = navegação e pertencimento (princípio central), inversão de prioridade (estrutura modular antes de camadas), checklist com duas perguntas (pertencimento → papel), árvore de referência, camadas reposicionadas como regras conceituais |

---

## `/Documentos/eBooks/TreinamentoDeAgentesIA/` → `.github/`

| Recurso | Status | Absorvido em | Detalhes |
|---------|--------|-------------|---------|
| Padrões de Projetos (GoF — Gamma et al.) | ✅ Absorvido | `.github/skills/padroes-de-design/` (SKILL.md + reference.md + selecao-por-problema.md) | Nova skill: 23 padrões (3 categorias), lookup problema→padrão, 7 causas de redesign, composição vs herança |
| Working Effectively with Legacy Code (Feathers) | ✅ Absorvido | `.github/skills/codigo-legado/` (SKILL.md + reference.md) | Nova skill: Legacy Code Change Algorithm (5 passos), seam model (3 tipos), 25 dependency-breaking techniques, characterization tests, Sprout/Wrap |
| TDD — Desenvolvimento Guiado por Testes (Kent Beck) | ✅ Absorvido | `.github/skills/quadro-de-recompensas/reference.md` | +Estratégias Red Path (Fake It, Triangulation, Obvious Implementation), +Test Data Strategies, +Test List Management, +Step Size/Comfort Level |
| Clean Craftsmanship (Uncle Bob) | ✅ Absorvido | `.github/skills/batedor-de-codigos/reference.md`, `.github/copilot-instructions.md` | +Métricas quantitativas (thresholds), +Boy Scout Rule operacionalizado, +Craftsmanship como princípio metodológico |
| Literate Programming (Knuth) | ✅ Absorvido | `.github/copilot-instructions.md`, `.github/skills/engenharia-de-software/reference.md` | +Specs como Narrativa (cross-referencing, complexidade graduada), +§11 Rigor e Determinismo |
| Pedagogia da Indignação (Freire) | ✅ Absorvido | `.github/copilot-instructions.md` | +Método Dialógico (diálogo ≠ transferência, inéditos viáveis, consciência crítica, práxis) |
| Art of Computer Programming (Knuth TAOCP) | ✅ Parcial | `.github/skills/engenharia-de-software/reference.md` §11 | Referência filosófica: análise de correção, complexidade graduada. Conteúdo algorítmico (2494 pgs) não absorvido — fora do escopo de spec-driven development |
| Cache no dotnet | ✅ Absorvido | `.github/skills/referencia-dotnet/` (SKILL.md + reference.md §1) | In-Memory, Distributed, Response Caching — decision table, patterns, regras |
| EF Core funcionalidades | ✅ Absorvido | `.github/skills/referencia-dotnet/reference.md` §2 | 10 features: Shadow Properties, Query Tags, Compiled Queries, DbContext Pooling, Value Converters, Temporal Tables, Seeding, Split Queries, Raw SQL, Multi-DB Migrations |
| dotnet-versionamento | ✅ Absorvido | `.github/skills/referencia-dotnet/reference.md` §3 | API Versioning: Controller-based, Minimal APIs, ApiVersioningOptions, 4 estratégias |
| Automatize Tarefas com Python (Sweigart) | ⏭️ Ignorado | — | Já coberto pelo princípio "poupar-creditos-com-skills" em `.github/instructions/` |

### .mobi convertidos (pendente absorção)

Convertidos via `scripts/convert-mobi.sh` (ebook-convert/Calibre → .txt → .md).

| Recurso | Status | Arquivo extraído | Absorvido em |
|---------|--------|-----------------|--------------|
| Clean Code (Uncle Bob) | ✅ Absorvido | `temp/pdf-extracts/01 - Clean Code - A Handbook of Agile Software Craftsmanship by Robert C. Martin (z-lib.org).pdf.md` | batedor reference.md (+Heurísticas Cap. 17: G1-G34, C1-C5, E1-E2, F1-F4, N1-N7, T1-T9), mercenario reference.md (+§13 Naming, §14 Funções, §15 Error Handling/DbC) |
| Pragmatic Programmer (Hunt & Thomas) | ✅ Absorvido | `temp/pdf-extracts/01 - O programador pragmático de aprendiz a mestre by Andrew Hunt David Thomas (z-lib.org).pdf.md` | copilot-instructions.md (+Pragmatismo: 8 princípios), mercenario reference.md (+§15 DbC/Programação Assertiva), glossario-unificado.md (+8 termos) |
| Refatoração (Martin Fowler) | ✅ Absorvido | `temp/pdf-extracts/01 - Refatoração by Fowler, Martin (z-lib.org).pdf.md` | mestre-freire reference.md (+65 técnicas, +Smell→Refactoring mapping 24 smells, +8 chains), mestre-freire SKILL.md (+Two Hats, Design Stamina, Green Bar, Step Size, triggers), batedor reference.md (+8 Fowler smells novos), glossario-unificado.md (+6 termos) |
| Arquitetura Limpa (Uncle Bob) | ✅ Absorvido | `temp/pdf-extracts/01 - Arquitetura Limpa - O Guia do Artesão para Estrutura e Design de Software.md` | principios-componentes.md (+§8 Serviços expandido, +Paradigmas como Disciplinas Negativas, +Testes como Componentes, +BD/Web/Framework como Detalhes, +Organização Componente>Recurso>Camada, +Screaming Architecture expandida), glossario-unificado.md (+2 termos) |
| Metodologia de Pesquisa (Wazlawick) | ✅ Absorvido | `temp/pdf-extracts/Metodologia de Pesquisa para Ciencia da Computacao - Raul Sidnei Wazlawick.md` | engenharia-de-software reference.md (+§12 Rigor Científico: L1-L5, 7 regras, taxonomia erros), tradutor reference.md (+Definições Operacionais), maestro reference.md (+Hipótese Justificada), quadro reference.md (+Variáveis de Teste), glossario-unificado.md (+8 termos) |

> **Nota:** "Código Limpo" e "Clean Code" são o mesmo livro (Uncle Bob) — o .mobi disponível é a versão em inglês.

---

## Arquivos criados nesta evolução (TreinamentoDeAgentesIA)

| Arquivo | Tipo |
|---------|------|
| `.github/skills/padroes-de-design/SKILL.md` | Nova skill |
| `.github/skills/padroes-de-design/reference.md` | Nova referência |
| `.github/skills/padroes-de-design/selecao-por-problema.md` | Nova referência |
| `.github/skills/codigo-legado/SKILL.md` | Nova skill |
| `.github/skills/codigo-legado/reference.md` | Nova referência |
| `.github/skills/referencia-dotnet/SKILL.md` | Nova skill |
| `.github/skills/referencia-dotnet/reference.md` | Nova referência |

## Arquivos modificados nesta evolução (TreinamentoDeAgentesIA)

| Arquivo | Alteração |
|---------|-----------|
| `.github/skills/quadro-de-recompensas/reference.md` | +Red Path (Beck), +Test Data Strategies, +Test List Management, +Step Size, +Characterization Tests (Feathers) |
| `.github/skills/mestre-freire/SKILL.md` | +Pré-fase Seam Analysis para código legado |
| `.github/skills/mestre-freire/reference.md` | +Refactoring para Padrões GoF (10 smell→padrão com sequência) |
| `.github/skills/batedor-de-codigos/reference.md` | +Padrões Não-Aplicados (10 sinais→padrão), +Métricas Quantitativas, +Boy Scout Rule, +Severity escalation |
| `.github/skills/engenharia-de-software/reference.md` | +§10 Código Legado e Evolução (Feathers), +§11 Rigor e Determinismo (Knuth) |
| `.github/reference/glossario-unificado.md` | +23 padrões GoF, +15 termos Legacy Code (Feathers), +6 termos TDD Avançado (Beck) |
| `.github/copilot-instructions.md` | +Método Dialógico Freire, +Specs como Narrativa Knuth, +Craftsmanship Uncle Bob, +3 skills na tabela |
| `.github/reference/tracking-absorcao.md` | +Seção TreinamentoDeAgentesIA (11 recursos + 5 pendentes) |
| `.github/skills/mestre-freire-angular/reference.md` | +§Estrutura ≠ Arquitetura (com árvore de decisão), estrutura modular atualizada (áreas/módulos), Domain Services com camadas em qualquer nível |
| `.github/reference/guia-angular.txt` | Removido (100% absorvido) |

## Arquivos modificados nesta evolução (.mobi absorção)

| Arquivo | Alteração |
|---------|-----------|
| `.github/skills/mestre-freire/reference.md` | +65 técnicas Fowler (7 categorias), +Smell→Refactoring (24 smells), +8 chains |
| `.github/skills/mestre-freire/SKILL.md` | +Princípios Fowler (Two Hats, Design Stamina, Green Bar, Step Size, triggers, Branch by Abstraction) |
| `.github/skills/engenharia-de-software/reference.md` | +§12 Rigor Científico Wazlawick (L1-L5, 7 regras, taxonomia erros, revisão sistemática) |
| `.github/copilot-instructions.md` | +Pragmatismo (8 princípios Hunt & Thomas) |
| `.github/skills/tradutor/reference.md` | +Definições Operacionais (Wazlawick) |
| `.github/skills/maestro/reference.md` | +Hipótese Justificada (Wazlawick) |
| `.github/skills/quadro-de-recompensas/reference.md` | +Variáveis de Teste (Wazlawick) |
| `.github/skills/batedor-de-codigos/reference.md` | +Heurísticas Clean Code Cap. 17 (62 heurísticas), +8 Fowler smells novos |
| `.github/skills/mercenario/reference.md` | +§13 Naming (11 regras), +§14 Funções (9 regras), +§15 Error Handling/DbC/Assertiva |
| `.github/reference/principios-componentes.md` | +Serviços expandido, +Paradigmas Negativos, +Testes Arquiteturais, +BD/Web/Framework Detalhes, +Organização, +Screaming expandida |
| `.github/reference/glossario-unificado.md` | +24 termos novos (Fowler, Wazlawick, Pragmatismo, Arq. Limpa) |
| `.github/reference/tracking-absorcao.md` | 5 .mobi → ✅ Absorvido |
