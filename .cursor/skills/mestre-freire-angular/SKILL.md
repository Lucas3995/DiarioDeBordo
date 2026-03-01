---
name: mestre-freire-angular
description: Complemento Angular da skill mestre-freire. Aplica as mesmas regras de refatoração guiada por relatório em código frontend Angular: camadas domain/application/infrastructure, convenções de componentes (templateUrl, styleUrl), serviços por camada, testes *.spec.ts. Usar quando o utilizador solicitar refatoração em projeto ou pasta Angular ou quando mestre-freire for aplicada a frontend Angular.
---

# Mestre Freire — Angular

Skill **complementar** à [mestre-freire](../mestre-freire/SKILL.md). Todos os princípios e o fluxo da mestre-freire aplicam-se; esta skill acrescenta **critério e convenções específicos do frontend Angular**.

Usar **em conjunto** com mestre-freire quando o relatório de inadequações incluir ficheiros Angular (ex.: `frontend/`, `app/`, componentes e serviços Angular).

---

## O que esta skill acrescenta

- **Regra de critério técnico:** `regras/angular-frontend.mdc` — camadas (domain, application, infrastructure, core, shared, features), convenções de componentes, serviços por camada, testes proporcionais.
- **Ficheiros de teste:** em Angular não alterar `*.spec.ts`. A rede de segurança são estes ficheiros; mantê-los intocados.
- **Comando de testes:** `npm run test` ou `ng test`. Executar após cada passo de refatoração; todos os testes devem continuar a passar.
- **Convenções ao refatorar:** componentes com `templateUrl` e `styleUrl`; lógica de domínio em `application/` ou `domain/`; componentes de página não injetam `HttpClient` diretamente; dependências de interfaces do domain, implementações em infrastructure.

---

## Fontes de critério (Angular)

- **Guia Angular:** `regras/angular-frontend.mdc` — mapeamento de camadas, trio de ficheiros (.ts, .html, .scss), onde colocar serviços, regras duras (não template inline em páginas, não HttpClient em componentes de página, não camadas vazias).
- **reference.md:** [reference.md](reference.md) — mapeamento DIP/serviços por camada em Angular, comando de testes e ficheiros *.spec.ts.
- Para categorias de smell e princípios gerais (SRP, OCP, etc.), consultar [reference da mestre-freire](../mestre-freire/reference.md) quando necessário.

Consultar o guia Angular para **onde** colocar código refatorado (domain, application, infrastructure, features) e **como** respeitar as convenções (templateUrl, styleUrl, standalone, lazy loading).

---

## Relação com mestre-freire

- **mestre-freire** define o fluxo (relatório → planejar → refatorar em passos → executar testes) e as regras gerais (comportamento inalterado, testes intocáveis, análise limitada ao relatório).
- **mestre-freire-angular** adiciona a regra `angular-frontend.mdc`, a convenção de ficheiros de teste `*.spec.ts` e o comando `npm run test` / `ng test` para o frontend Angular.

Ao refatorar código Angular, seguir primeiro a mestre-freire e aplicar em seguida os critérios desta skill para cada achado que toque em componentes, serviços ou estrutura de pastas do frontend Angular.
