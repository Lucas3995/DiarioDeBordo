---
name: engenharia-de-software
description: Orienta agentes de IA na prática da engenharia de software com conceitos, princípios, técnicas e padrões (Pressman & Maxim 9ª ed., Sommerville 10ª ed.). Usar em requisitos (usuário e sistema), especificação, spec-driven development, projeto e arquitetura, testes (incl. TDD), SQA, dependabilidade, confiabilidade, segurança (safety/security), resiliência, gestão de projeto, estimativas, manutenção, evolução e SCM.
---

# Engenharia de Software

Skill baseada em Pressman & Maxim (9ª ed.) e Sommerville (10ª ed.), com visão integrada nos tópicos em que as fontes se sobrepõem (ver dialética tese/antítese/síntese em [reference.md](reference.md)). Aplica-se quando o agente atua em tarefas de engenharia de software: requisitos, modelagem, projeto, testes, qualidade, dependabilidade, gestão de projeto ou spec-driven development.

*Definições de termos: [Glossário Unificado](../../reference/glossario-unificado.md)*

## Quando usar

- Análise e levantamento de requisitos; especificação em dois níveis (usuário e sistema); rastreabilidade; gestão de mudança de requisitos.
- Projeto de arquitetura, componentes ou interfaces; múltiplas vistas (contexto, interação, estrutura, comportamento); padrões de projeto.
- Planejamento e execução de testes (componente, integração, lançamento, usuário); TDD; validação em ambiente real.
- Garantia de qualidade (SQA); dependabilidade e métricas de confiabilidade (ROCOF, MTTF, disponibilidade); revisões técnicas.
- Sistemas críticos; dependabilidade; resiliência; engenharia de sistemas; estimativa de custos (COCOMO).
- Segurança (safety) e segurança da informação (security); SQUARE; hazard analysis; codificação segura.
- Gestão de projeto: planejamento (algorítmico e ágil), estimativas, riscos, pessoas e equipe, cronograma.
- Spec-driven development: especificação como artefato primeiro; contratos de API; alinhamento spec–código–testes.
- Evolução e manutenção; sistemas legados; configuração (SCM); melhoria de processo (SPI).

## Conceitos-chave (visão sintética)

- **Processo de software:** estrutura adaptável em que a mudança é constitutiva; atividades metodológicas (comunicação, planejamento, modelagem, construção, entrega); modelos (cascata, evolucionário, PU, ágil – Scrum, XP, Kanban, DevOps); agilidade escalável com atenção a dependabilidade e certificação em domínios críticos; melhoria contínua do processo.
- **Requisitos:** engenharia em dois níveis – requisitos de usuário e requisitos de sistema – com mapeamento explícito de envolvidos e leitores por documento; concepção, elicitação, elaboração, negociação, especificação, validação; cenários e casos de uso; NFRs; rastreabilidade; gestão de mudança de requisitos integrada ao fluxo.
- **Modelagem e projeto:** múltiplas vistas (contexto, interação, estrutura, comportamento) alinhadas a visões arquiteturais; UML (classes, sequência, estados, atividades); CRC; MDD quando reduzir inconsistência; padrões de projeto como vocabulário comum (contexto–problema–solução); separação de interesses, modularidade.
- **Testes:** continuum unidade → integração → lançamento → usuário; TDD como prática de construção; caixa-branca (caminho básico, estrutura de controle) e caixa-preta (equivalência, valor limite); integração (top-down, bottom-up, contínua); validação em ambiente real/usuário (alfa, beta, aceitação); objetivo de encontrar erros (Myers) em todos os níveis.
- **Qualidade e SQA:** qualidade operacionalizada como dependabilidade (propriedades mensuráveis) + processo (revisões, padrões, SQA); revisões e inspeções adaptadas ao contexto (plano vs ágil); métricas de confiabilidade (ROCOF, MTTF, disponibilidade) quando o sistema for crítico; custo da qualidade; ISO 9000:2015.
- **Segurança (safety) e segurança da informação (security):** duas dimensões distintas – safety = sistemas críticos, dano não intencional; security = ataques intencionais, confidencialidade/integridade/disponibilidade. Técnicas: hazard analysis e processos de safety; SQUARE, casos de mau uso, codificação segura para security; resiliência (antecipar, responder, aprender) como ponte organizacional e técnica.
- **Evolução e manutenção:** evolução como fase contínua; legado e operação como preocupações de primeira classe; tipos de manutenção; refatoração e reengenharia como instrumentos; estimativa de custos de manutenção no planejamento.
- **Gestão de projeto:** planejamento como diálogo entre abordagem algorítmica (COCOMO, FP, LOC) e ágil (velocidade, histórias); W5HH e RMMM; escopo; estimativas; cronograma (Gantt); riscos contínuos; pessoas e equipe explícitas; qualidade e configuração integradas ao plano.
- **Configuração (SCM):** versão (central e distribuída, e.g. Git), construção, controle de mudança e lançamentos; integração contínua e auditoria como práticas de garantia.

## Conceitos complementares (Sommerville)

- **Dependabilidade:** propriedades (disponibilidade, confiabilidade, segurança/safety, segurança da informação/security); sistemas sociotécnicos (hardware + software + pessoas/organização); redundância e diversidade; processos confiáveis; verificação formal de propriedades críticas (ex.: invariantes de segurança) quando a criticidade do sistema justificar.
- **Confiabilidade:** ROCOF (taxa de falhas), MTTF (tempo médio até falha), disponibilidade (AVAIL); requisitos de confiabilidade (verificação, recuperação, redundância); arquiteturas tolerantes a defeitos; defeito → erro → falha.
- **Resiliência:** reconhecimento, resistência, recuperação, restabelecimento (4 Rs); no nível organizacional: antecipar, responder, aprender; projeto de sistemas resilientes.
- **Engenharia de sistemas:** sistemas sociotécnicos; projeto conceitual; aquisição; desenvolvimento; operação e evolução; complexidade e classificação de sistemas de sistemas.

## Princípios (resumidos)

**Processo:** (1) Seja ágil; processo adaptável, mudança constitutiva. (2) Foco na qualidade e dependabilidade em todas as etapas. (3) Adapte o processo ao problema, pessoas e projeto; em domínios críticos, considere processos confiáveis e certificação. (4) Monte equipe eficiente; comunicação e coordenação. (5) Gerencie mudanças. (6) Avalie riscos; planos de contingência. (7) Gere artefatos que forneçam valor; melhoria contínua.

**Prática:** (1) Separação de interesses / dividir e conquistar. (2) Abstração. (3) Consistência. (4) Atenção a interfaces e transferência de informações. (5) Modularidade efetiva. (6) Busque padrões (contexto–problema–solução). (7) Múltiplas perspectivas (contexto, interação, estrutura, comportamento; UML). (8) Pense na manutenção e na evolução.

**Comunicação:** ouvir; preparar-se; facilitador; comunicação presencial com artefatos; documentar decisões; colaboração; foco modular; desenhar para clareza; seguir em frente quando bloqueado; negociação ganha-ganha.

**Planejamento:** escopo claro; envolver envolvidos; planejamento iterativo e diálogo entre estimativa algorítmica (COCOMO, FP, LOC) e ágil (velocidade, histórias); considerar riscos e dimensão pessoas/equipe; definir qualidade e configuração no plano; acomodar alterações; verificar e ajustar.

**Modelagem:** objetivo é construir software; modelo mais simples; propósito claro; adaptar ao sistema; útil, não perfeito; feedback cedo.

**Construção:** entender problema e projeto; linguagem e ambiente adequados; testes de unidade (TDD quando aplicável); programação estruturada; revisão de código; refatoração. **Testes (Myers):** teste é para encontrar erros; continuum até teste de usuário; teste bem-sucedido revela erro. **Entrega:** estrutura de suporte antes da entrega; planejamento de testes cedo; Pareto (80% erros em ~20% componentes).

## Spec-driven development

- Especificação como **fonte única de verdade**; contratos (APIs, interfaces, comportamento esperado).
- Tratar a spec como **artefato primeiro**; derivar ou validar implementação e casos de teste em relação à spec.
- Manter **rastreabilidade** spec–código–testes. Para APIs, considerar especificações formais (OpenAPI, protobuf, esquemas) como artefato central.
- Usar specs executáveis ou verificáveis quando apropriado (testes de contrato, validação de schemas); design-by-contract quando aplicável.

## Fluxos de trabalho sugeridos

1. **Requisitos:** identificar envolvidos e leitores por documento → dois níveis (usuário, sistema) → levantamento (cenários, casos de uso) → elaboração → negociação → especificação → validação; NFRs e rastreabilidade; gestão de mudança integrada.
2. **Projeto:** entender escopo → múltiplas vistas (contexto, interação, estrutura, comportamento) → arquitetura (arquétipos, componentes) → revisão; separação de interesses, modularidade, padrões (contexto–problema–solução).
3. **Testes:** planejar cedo; continuum (unidade → integração → lançamento → usuário); TDD quando aplicável; caixa-branca e caixa-preta; validação em ambiente real; Myers: objetivo é encontrar erros.
4. **Planejamento:** W5HH; escopo; diálogo estimativa algorítmica (COCOMO, FP, LOC) e ágil (velocidade, histórias); riscos (tabela, RMMM); pessoas e equipe; cronograma; qualidade e configuração no plano.
5. **Spec-driven:** especificação primeiro → implementação e testes alinhados à spec → rastreabilidade; OpenAPI/contratos como central para APIs.

## Boas práticas para o agente

- Ao analisar requisitos: dois níveis (usuário e sistema) e mapeamento de envolvidos/leitores; NFRs e rastreabilidade; gestão de mudança.
- Ao propor arquitetura ou projeto: separação de interesses, modularidade, padrões (contexto–problema–solução); múltiplas vistas (contexto, interação, estrutura, comportamento; UML).
- Ao sugerir testes: continuum (componente → integração → lançamento → usuário); TDD quando aplicável; equivalência e valor limite; validação em ambiente real quando relevante.
- Ao estimar ou planejar: W5HH; diálogo entre estimativa algorítmica (COCOMO, FP) e ágil (velocidade, histórias); riscos e dimensão pessoas/equipe; qualidade e configuração no plano.
- Em sistemas críticos ou dependáveis: considerar dependabilidade (disponibilidade, confiabilidade, safety, security); métricas (ROCOF, MTTF); processos confiáveis e certificação; resiliência (4 Rs).
- Em spec-driven: priorizar spec antes da implementação; testes e código alinhados à spec; rastreabilidade; para APIs, spec formal (ex.: OpenAPI) como central.
- Terminologia consistente: envolvidos (stakeholder), artefato, FTR, SQA, SCM; segurança (safety) vs segurança da informação (security).

## Referência detalhada

- **Pressman + síntese dialética:** princípios completos por atividade, dialética tese/antítese/síntese, técnicas por fase, glossário unificado e mapeamento de ambas as fontes: [reference.md](reference.md).
- **Sommerville 10e:** resumo por parte, termos-chave por capítulo e princípios/definições: [sommerville-reference.md](sommerville-reference.md).
