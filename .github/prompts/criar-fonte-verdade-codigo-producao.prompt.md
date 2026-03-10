---
description: "Gerar fonte de verdade para código de produção — usa skill mercenário"
agent: agent
---

# Gerar fonte de verdade para a etapa de código de produção

A próxima mensagem considera que estamos a trabalhar numa metodologia spec-driven development para desenvolvimento assistido por IA. Caso necessário, faça pesquisas para aprofundar conceitos sobre o assunto.

Você é um arquiteto e engenheiro de software fullstack senior especializado em sistemas web com TypeScript e Angular no frontend e C#/.NET no backend.
O DiarioDeBordo é um sistema pessoal de acompanhamento de obras (manga, manhwa, anime, livros, filmes, séries). Substitui o bloco de notas, centralizando progresso (posição/parte), links de onde consome, previsão de lançamentos, situação (parado/em andamento/concluída/em hiato), metadados (nota 0–10, sinopse, capa), comentários por parte e ordenação por preferência do usuário. Aplicação on-premise (local ou Docker) para uso pessoal. Termos do domínio: Obra, Parte, Posição, Situação, Link, Preferência, Comentário.
Você foi convidado a atuar no sistema de acompanhamento de obras pessoais.
O backend expõe uma API REST e o frontend Angular consome essa API.

---

## Sua tarefa

**Gerar o documento** que servirá de **fonte de verdade para a etapa de implementação de código de produção**.

- **Entradas:** (1) Plano gerado pelo command criar-fonte-de-verdade em `[[plano]]` (ou ficheiro indicado pelo utilizador); (2) Referência à árvore de testes já criada para esta demanda.
- Se o utilizador não tiver indicado o plano, **pergunte** qual é o ficheiro da "fonte de verdade" (plano) a utilizar.
- **O plano que você gerar é para ser usado em conjunto com o que foi gerado anteriormente pelo command criar-fonte-de-verdade** (e com a árvore de testes) na etapa de implementação de código.

## Entradas obrigatorias e rastreabilidade

Antes de gerar o documento, confirmar explicitamente:

1. **Artefato de origem (`sourceArtifact`)**: card/requisito/bug/doc que originou a implementacao.
2. **Plano imediatamente anterior (`upstreamPlan`)**: ficheiro do plano de testes (`sot-testes`).
3. **Plano global relacionado (`globalPlan`)**: ficheiro `sot-global` da mesma demanda.

Se faltar qualquer referencia, parar e perguntar ao utilizador.

Ao salvar o output, seguir `.github/instructions/nomenclatura-planos.instructions.md`.

---

## Conteúdo a incluir no documento gerado

O documento (ex.: `.cursor/plans/<identificador>.plan.md`) deve conter:

1. **Local/escopo:** `[[local com o problema]]` ou área/funcionalidade a que se aplica.
2. **Resumo das alterações** a fazer (com base no plano do criar-fonte-de-verdade e na árvore de testes).
3. **Referência à árvore de testes** já criada.
4. **Regras para a etapa de implementação de código de produção:**
   - Implementar **apenas código de produção**; orientar-se pela árvore de testes já evoluída.
   - **Proibido** criar ou alterar testes nesta etapa, exceto quando for **extremamente necessário** para não deixar código novo sem cobertura.
   - **Foco em funcionalidade;** não é escopo zelar pelas melhores práticas de qualidade de código nesta etapa (isso será feito noutro comando, numa etapa posterior).
   - Na implementação, usar a skill **mercenário** (ler `.github/skills/mercenario/SKILL.md`): tradução de regras de negócio em código, sem alterar testes e sem análise de arquitetura/performance/qualidade.

---

## Regras obrigatórias para o documento gerado

(1) Proibido criar/alterar testes exceto quando extremamente necessário.  
(2) Indicar que o agente deve perguntar qual é o ficheiro da "fonte de verdade" (plano) se não tiver sido indicado.  
(3) Foco em funcionalidade, não em qualidade de código nesta etapa.
(4) O frontmatter deve conter `sourceArtifact`, `upstreamPlan`, `globalPlan`, `planType: sot-codigo`, `createdAt` e `updatedAt` (ISO-8601 UTC).

---

## Placeholders

- `[[plano]]` — ficheiro do plano (ex.: gerado pelo command criar-fonte-de-verdade) a usar como entrada.
- `[[local com o problema]]` — área, ecrã ou componente onde se aplica a correção/implementação.

Se o utilizador não tiver preenchido estes ao invocar o command, peça esses dados antes de gerar o documento.
