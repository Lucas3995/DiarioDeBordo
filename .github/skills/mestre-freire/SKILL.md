---
name: mestre-freire
description: Refatora código conforme relatório prévio de inadequações, sem alterar comportamentos nem testes. Aplica melhorias técnicas e conceituais (SOLID, Clean Architecture, DDD, CQRS, code smells) guiado pelo relatório. Executa a suíte de testes e garante que continue passando. Usar quando o utilizador solicitar refatoração guiada por relatório, execução de plano de correção ou melhoria técnica sem mudança de regras de negócio.
---

# Mestre Freire — Refatoração Guiada por Relatório

Refatora o código para **melhorias técnicas e conceituais** com base **exclusivamente** num relatório de inadequações (ex.: produzido pela skill batedor-de-codigos). Não altera comportamentos, não altera testes e não realiza nova análise em busca de necessidades de refatoração.

**Para código Angular**, esta skill deve ser complementada por [mestre-freire-angular](../mestre-freire-angular/SKILL.md), que adiciona critérios e convenções específicos do frontend.

*Definições de termos: [Glossário Unificado](../../reference/glossario-unificado.md)*

---

## Princípios

- **Comportamento inalterado:** regras de negócio e contratos públicos permanecem iguais. Nenhuma adição nem remoção de funcionalidade.
- **Testes intocáveis:** ficheiros de teste unitário e de integração **não são alterados**. Os testes são a rede de segurança; mudá-los invalidaria essa garantia.
- **Testes obrigatórios:** após cada refatoração (ou lote coerente), executar a suíte de testes. **Todos devem continuar a passar.** Se falharem, a causa é a refatoração — corrigir até passarem.
- **Análise limitada:** as únicas análises permitidas são (1) a **análise prévia**, fornecida pelo relatório, e (2) **análise para resolver problemas** introduzidos pela própria refatoração (ex.: teste que passou a falhar, compilação quebrada). **Proibido** fazer varreduras em busca de novos smells ou novas necessidades de refatoração.

### Princípios Fowler (Refactoring 2nd Ed.)

- **Two Hats (Kent Beck):** Alternar constantemente entre dois chapéus — Chapéu 1 (adicionar features, mudar comportamento) e Chapéu 2 (refatorar, melhorar estrutura sem mudar comportamento). Nunca usar os dois ao mesmo tempo. Esta skill opera **exclusivamente** com o Chapéu 2.
- **Design Stamina Hypothesis:** Bom design sustenta velocidade de desenvolvimento ao longo do tempo. Refatorar não é custo — é investimento que mantém a taxa de entrega sustentável.
- **Green Bar Discipline:** Nunca refatorar com testes falhando. O ciclo é: testes verdes → refatorar → testes verdes. Se um teste falha durante refatoração, desfazer e tentar passos menores.
- **Step Size:** Cada mudança deve ser pequena (~5 linhas), testável imediatamente e comitável individualmente. Se o teste fala, o passo foi grande demais — reduzir.
- **Quando refatorar — 5 gatilhos:**
  1. **Rule of Three** — terceira ocorrência de duplicação → refatorar.
  2. **Preparatória** — antes de adicionar feature, refatorar para facilitar a adição.
  3. **Compreensão** — ao ler código difícil, refatorar para deixar a compreensão no código.
  4. **Garbage collection** — ao encontrar código morto ou desnecessário, remover.
  5. **Oportunística** — ao tocar num ficheiro por outro motivo, melhorar o que estiver próximo.
- **Quando NÃO refatorar — 3 condições:**
  1. Código que **não será modificado** — se funciona e ninguém toca, deixar.
  2. **Reescrever é mais fácil** — custo de refatoração supera o de reescrita.
  3. **API pública** — alterar assinaturas públicas causa impacto nos consumidores; usar Expandable-Contractible.
- **Branch by Abstraction:** Para migrações longas (1-2 semanas), introduzir camada de abstração; implementar nova versão atrás da abstração; migrar consumidores gradualmente; remover abstração.
- **Expandable-Contractible Pattern:** Para mudanças em APIs ou BD — expandir (adicionar nova versão mantendo a antiga), migrar consumidores, contrair (remover versão antiga).

---

## Fluxo de trabalho

0. **Pré-fase: Seam Analysis (código legado)**  
   Quando o código a refatorar **não tem testes suficientes** (legacy code):
   1. Mapear seams disponíveis no componente (object, link, preprocessing)
   2. Quebrar dependências usando catálogo de 25 técnicas (ver skill [codigo-legado](../codigo-legado/SKILL.md))
   3. Escrever **characterization tests** que documentem comportamento atual
   4. Somente após essa cobertura, seguir para o passo 1 abaixo
   
   **Critério:** Se o relatório indica smells em código sem testes → esta pré-fase é **obrigatória** antes de refatorar.

1. **Receber o relatório**  
   Relatório de inadequações com achados (ID, smell, ficheiro, localização, evidência, princípio violado). Se o utilizador não fornecer relatório, solicitar ou indicar que esta skill exige relatório como entrada.

2. **Priorizar por impacto**  
   Agrupar achados do relatório por impacto antes de iniciar refatoração:
   - **Alta:** Dependências de camada (ciclos, DIP, Framework Coupling) — estabilizar é pré-requisito para as demais.
   - **Média:** SOLID (SRP, OCP, LSP, ISP), DDD (entidades anêmicas, God Services).
   - **Baixa:** Componentes (REP, CCP, CRP, SDP, SAP), organização fina (Humble Object, Main, Limites Parciais).
   
   Ver [reference.md](reference.md) §Priorização por impacto para detalhes.

3. **Planejar por achado**  
   Para cada achado (na ordem de prioridade), identificar a **técnica de refatoração** ou o **princípio** a aplicar (ver [reference.md](reference.md)). Para achados em nível de componente, consultar também [principios-componentes.md](../../reference/principios-componentes.md).

4. **Refatorar em passos pequenos com verificação TDD**  
   - Um achado (ou grupo coerente de achados no mesmo ficheiro) por vez.  
   - Aplicar apenas alterações que **mitiguem o achado** sem mudar comportamento.
   - **Ritmo Fowler:** mudança pequena (~5 linhas) → testar → commit → repetir. Dezenas de refactorings por dia em desenvolvimento ativo; commits granulares (1 refactoring por commit).
   - **Se teste falha:** desfazer a mudança; tentar passos menores. O teste falhando significa que o passo alterou comportamento.
   - **Ciclo TDD de segurança:** testes existentes protegem (Red = confiança na rede) → refatorar (Green = manter testes verdes) → verificar (todos os testes continuam verdes).  
   - Não alterar ficheiros de teste (conforme convenção do projeto).
   - Para achados que mapeiam a **cadeias de refactoring** (§Cadeias de Refactoring em [reference.md](reference.md)), seguir a sequência pré-definida em vez de decidir técnica a técnica.

5. **Executar testes**  
   Após cada passo (ou lote pequeno), executar a **suíte completa** de testes do projeto (backend e frontend). Objetivo: **verificar que as regras de negócio continuam funcionando** após a refatoração. Se algum teste falhar: tratar como **regressão da refatoração**; analisar a falha, ajustar o código de produção (não o teste) e repetir até todos passarem. Se o ambiente do host não permitir rodar os testes (ex.: Node &lt; 20 para o frontend), usar a **containerização do projeto** (ex.: `./scripts/frontend-test-docker.sh`).

6. **Repetir** até todos os achados do relatório estarem tratados e a suíte de testes verde.

---

## Regras duras

- **Não adicionar nem remover comportamentos.** Refatoração é mudança de estrutura, não de funcionalidade.
- **Não editar testes** para “acomodar” a refatoração. Se um teste quebra, a refatoração está a alterar comportamento — reverter ou ajustar o código de produção.
- **Não realizar nova análise** em busca de code smells ou necessidades de refatoração além do relatório. Análise extra só para diagnosticar falhas de teste ou build causadas pela refatoração.
- **Não deixar a suíte de testes falhar** ao concluir. Executar **todos** os testes (backend e frontend) antes de dar por concluída a tarefa; usar Docker/containerização quando o host não tiver o ambiente correto.

---

## Fontes de critério técnico

As **melhorias técnicas e conceituais** seguem as regras do projeto em `regras/`:

- **Clean Architecture e SOLID:** `.github/instructions/clean-architecture.instructions.md` — camadas, direção de dependência, SRP, OCP, LSP, ISP, DIP.
- **DDD:** `.github/instructions/ddd-domain-driven-design.instructions.md` — entidades, value objects, agregados, linguagem ubíqua.
- **CQRS:** `.github/instructions/cqrs-command-query-responsibility-segregation.instructions.md` — comandos vs queries, write/read model.
- **Técnicas e smells:** `.github/instructions/tecnicas-conhecidas.instructions.md` — catálogo de refatorações e code smells.
- **TDD:** `.github/instructions/tdd-test-driven-development.instructions.md` — testes como rede de proteção; não alterar testes nesta skill.
- **LGPD e segurança:** `.github/instructions/protecao-dados-lgpd-seguranca.instructions.md`, `.github/instructions/devsecops.instructions.md` — não enfraquecer controles ao refatorar.

Consultar estas regras para **como** aplicar a correção (onde colocar código, que padrão usar). O **o quê** corrigir vem do relatório.

---

## Checklist antes de dar por concluído

- [ ] Todos os achados do relatório foram tratados (ou documentada exceção justificada).
- [ ] Nenhum ficheiro de teste foi modificado.
- [ ] A suíte de testes foi executada e está verde.
- [ ] Nenhuma nova análise de smells foi feita além do relatório e do diagnóstico de falhas da refatoração.

---

## Relação com outras skills

- **batedor-de-codigos:** produz o relatório de inadequações que esta skill consome. Fluxo típico: batedor analisa → relatório → mestre-freire refatora.
- **mestre-freire-angular:** skill complementar para refatoração em frontend Angular; usar quando o escopo incluir código Angular.
- **create-skill / regras:** as regras em `regras/` definem o critério de “melhoria técnica e conceitual”; esta skill aplica-as apenas no âmbito do relatório, sem alterar comportamento nem testes.
