# Referência Sommerville – Engenharia de software 10e

Documento complementar com resumo por parte, termos-chave por capítulo e trechos de princípios/definições (Sommerville, *Engenharia de software*, 10ª ed., Pearson Brasil). Para dialética com Pressman e glossário unificado, ver [reference.md](reference.md).

---

## Resumo por parte

**Parte 1 – Introdução à engenharia de software (Cap. 1–9)**  
Cobre o ciclo de vida essencial: introdução e ética; processos de software (modelos, atividades, lidar com mudanças, melhoria de processo); desenvolvimento ágil (métodos, técnicas, gerenciamento, escalabilidade); engenharia de requisitos (dois níveis, elicitação, especificação, validação, mudança); modelagem de sistemas (contexto, interação, estrutura, comportamento, MDD); projeto de arquitetura (decisões, visões, padrões, arquiteturas de aplicações); projeto e implementação (OO/UML, padrões de projeto, open source); teste (desenvolvimento, TDD, lançamento, usuário); evolução (processos, legados, manutenção).

**Parte 2 – Dependabilidade e segurança (Cap. 10–14)**  
Foco em propriedades de dependabilidade (disponibilidade, confiabilidade, safety, security); sistemas sociotécnicos; redundância e diversidade; processos confiáveis e métodos formais; engenharia de confiabilidade (métricas ROCOF, MTTF, disponibilidade; arquiteturas tolerantes a defeitos); engenharia de segurança (safety) e de segurança da informação (security); engenharia de resiliência (4 Rs, cibersegurança, sistemas resilientes).

**Parte 3 – Engenharia de software avançada (Cap. 15–21)**  
Reúso (frameworks, linhas de produto); engenharia baseada em componentes (modelos, processos, composição); engenharia distribuída (cliente-servidor, padrões, SaaS); orientada a serviços (SOA, RESTful, composição); engenharia de sistemas (sociotécnicos, projeto conceitual, aquisição, operação); sistemas de sistemas (complexidade, classificação, arquitetura); software de tempo real e embarcado (padrões, análise temporal, RTOS).

**Parte 4 – Gerenciamento de software (Cap. 22–25)**  
Gerenciamento de projetos (riscos, pessoas, trabalho em equipe); planejamento (precificação, cronograma, planejamento ágil, estimativa, COCOMO); gerenciamento da qualidade (padrões, revisões, medição, ágil); gerenciamento de configuração (versões, construção, mudanças, lançamentos).

---

## Capítulo → termos-chave

| Cap. | Termos-chave |
|------|----------------|
| 1 | desenvolvimento profissional, ética, estudos de caso |
| 2 | modelos de processo, atividades, mudanças, melhoria de processo |
| 3 | métodos ágeis, TDD, gerenciamento ágil, escalabilidade |
| 4 | requisitos de usuário, requisitos de sistema, elicitação, validação, mudança de requisitos, stakeholders |
| 5 | modelos de contexto, interação, estruturais, comportamentais, MDD |
| 6 | decisões arquiteturais, visões, padrões de arquitetura, arquiteturas de aplicações |
| 7 | OO, UML, padrões de projeto, open source |
| 8 | teste de desenvolvimento, TDD, teste de lançamento, teste de usuário (alfa, beta, aceitação) |
| 9 | processos de evolução, sistemas legados, manutenção |
| 10 | dependabilidade, propriedades, sociotécnicos, redundância, diversidade, processos confiáveis |
| 11 | confiabilidade, disponibilidade, ROCOF, MTTF, requisitos de confiabilidade, tolerância a defeitos |
| 12 | segurança (safety), sistemas críticos, casos de segurança, hazard analysis |
| 13 | segurança da informação (security), C-I-A, projeto seguro, garantia |
| 14 | resiliência, 4 Rs, cibersegurança, sistemas resilientes |
| 15 | reúso, frameworks, linhas de produto |
| 16 | componentes, modelos de componentes, composição |
| 17 | sistemas distribuídos, cliente-servidor, SaaS |
| 18 | SOA, RESTful, engenharia de serviços, composição de serviços |
| 19 | engenharia de sistemas, sociotécnicos, projeto conceitual, aquisição |
| 20 | sistemas de sistemas, complexidade, classificação |
| 21 | tempo real, embarcados, análise temporal, RTOS |
| 22 | riscos, pessoas, trabalho em equipe |
| 23 | planejamento, COCOMO, cronograma, planejamento ágil |
| 24 | qualidade, padrões, revisões, inspeções, medição |
| 25 | versões, construção, mudanças, lançamentos |

---

## Princípios e definições (por capítulo)

**Cap. 4 – Requisitos**  
Requisitos de usuário: declarações em linguagem natural do que o sistema deve fazer; leitores: gerentes do cliente, usuários finais. Requisitos de sistema: especificação detalhada que pode ser base para contrato; leitores: engenheiros, arquitetos, desenvolvedores. Stakeholders: qualquer um afetado pelo sistema com interesse legítimo (usuários, médicos, equipe de TI, reguladores, etc.).

**Cap. 10 – Dependabilidade**  
Propriedades de dependabilidade: disponibilidade, confiabilidade, segurança (safety), segurança da informação (security). Sistemas sociotécnicos: confiabilidade influenciada por hardware, software e operador; propriedades emergentes. Redundância e diversidade para tolerância a falhas.

**Cap. 11 – Confiabilidade**  
ROCOF (rate of occurrence of failures): número esperado de falhas por unidade de tempo. MTTF (mean time to failure): tempo médio entre falhas. Disponibilidade: probabilidade de o sistema estar operacional quando demandado. Defeito → erro (estado incorreto) → falha (manifestação observada).

**Cap. 12–13 – Safety e security**  
Safety: ausência de dano não intencional; sistemas críticos em segurança; hazard analysis, casos de segurança. Security: proteção contra ataques intencionais; confidencialidade, integridade, disponibilidade (C-I-A).

**Cap. 14 – Resiliência**  
4 Rs (sistema): reconhecimento (do problema), resistência, recuperação, restabelecimento. Organizacional: antecipar ameaças e oportunidades; responder a ameaças e vulnerabilidades; aprender com a experiência.

**Cap. 19 – Engenharia de sistemas**  
Sistema sociotécnico: mais do que a soma das partes; hardware + software + pessoas; propriedades emergentes; projeto influenciado por fatores humanos e organizacionais. Projeto conceitual como primeiro estágio; aquisição; desenvolvimento; operação e evolução.

**Cap. 23 – Planejamento**  
Modelagem algorítmica de custo: fórmula matemática para prever custos (tamanho, tipo de software, fatores de equipe/processo/produto). COCOMO II: modelo de estimativa de custos; uso em estimativas iniciais e comparação com abordagens ágeis.
