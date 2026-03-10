---
description: "Configurar o toolkit .github para o projeto atual — adaptar workflows, agents, skills e prompts ao domínio, stack e estrutura do repositório"
agent: agent
---

# Configurar toolkit .github para o projeto

Este prompt **adapta todos os artefatos genéricos** da pasta `.github/` ao projeto onde foi adicionada. Deve ser executado **uma única vez** após copiar a pasta `.github/` para um novo repositório.

---

## 1. Descoberta do projeto

Antes de qualquer alteração, **coletar o contexto do projeto**:

1. **Ler `README.md`** na raiz do repositório. Se não existir, perguntar ao operador sobre domínio, stack e contexto e criar um `README.md` com essas informações.
2. **Mapear estrutura de pastas** — identificar:
   - Backend: localizar arquivos `.sln`, `.csproj`, `pom.xml`, `go.mod`, `Cargo.toml` ou equivalente.
   - Frontend: localizar `package.json`, `angular.json`, `next.config.*`, `vite.config.*` ou equivalente.
   - Docker: localizar `docker-compose.yml`, `Dockerfile`.
   - Testes: localizar projetos/pastas de teste (ex.: `*.Tests.csproj`, `*.spec.ts`, `__tests__/`).
3. **Identificar tecnologias e versões** — a partir dos arquivos de configuração encontrados (ex.: `.csproj` → .NET version, `package.json` → Node version, `angular.json` → Angular version).
4. **Identificar domínio** — a partir do README, nomes de entidades no código, namespaces e estrutura de pastas.

Apresentar ao operador um **resumo da descoberta** para validação antes de prosseguir:

```
Projeto: <nome>
Domínio: <descrição breve>
Backend: <stack e versão>
Frontend: <stack e versão>
Testes backend: <caminho do projeto de testes>
Testes frontend: <comando>
Solution: <caminho .sln>
API project: <caminho .csproj da API>
Docker: <caminho docker-compose>
```

---

## 2. Atualizar workflows

### 2.1. `.github/workflows/ci.yml`

Substituir os placeholders `YourProject` nas env vars:

```yaml
env:
  DOTNET_VERSION: "<versão detectada>"
  SOLUTION_PATH: <caminho real do .sln>
  TEST_PROJECT: <caminho real do .csproj de testes>
  RUNSETTINGS_PATH: <caminho real do coverlet.runsettings, se existir>
```

Se o projeto **não usar .NET**, considerar remover ou adaptar este workflow para a stack do projeto. Perguntar ao operador.

Se `coverlet.runsettings` **não existir** no projeto de testes, remover a linha `--settings` do step de testes ou criar o arquivo com configuração padrão.

### 2.2. `.github/workflows/backend-ci.yml`

- Atualizar a versão do .NET em `dotnet-version` para a versão detectada.
- Se o projeto tiver configuração JWT, anotar os valores recomendados para `vars.JWT_ISSUER_CI` e `vars.JWT_AUDIENCE_CI`.

### 2.3. `.github/workflows/backend-deploy.yml`

- Atualizar o path do projeto API em `vars.API_PROJECT` fallback.
- Se o operador souber a estratégia de deploy, implementar os steps correspondentes.
- Se não souber, deixar o TODO e informar ao operador.

### 2.4. Branches

Verificar quais branches o projeto usa (ex.: `main`/`develop` vs `Master`/`Dev`) e atualizar os triggers `on.push.branches` e `on.pull_request.branches` em todos os workflows.

---

## 3. Atualizar agents

### 3.1. Comandos de teste

Nos 3 agents que referenciam comandos de teste genéricos, atualizar o exemplo entre parênteses com o comando real do projeto:

- `.github/agents/3-mercenario.agent.md` — Linha do Backend
- `.github/agents/4-instrutor-mestre.agent.md` — Linha do Backend
- `.github/agents/instrutor-mestre.agent.md` — Linha do Backend

Exemplo: trocar `(ex.: dotnet test <projeto-de-testes> -c Release)` pelo comando real detectado.

Se o projeto **não tiver frontend**, remover a linha de Frontend dos agents. Se tiver, validar o comando (ex.: `npm run test`, `ng test`, `pnpm test`).

---

## 4. Atualizar skills

### 4.1. `quadro-de-recompensas/reference.md`

Na seção "Mapeamento ao projeto", atualizar:

- Frontend: validar se a estrutura de pastas corresponde (ex.: `frontend/src/app/`).
- Backend: substituir `<Project>` pelo nome real do projeto de testes nos caminhos de exemplo.
- Se o projeto não usar Angular ou .NET, adaptar as seções para a stack real.

### 4.2. `copilot-instructions.md`

- Verificar se o caminho `docker/docker-compose.yml` corresponde ao real.
- Verificar se o script `./scripts/frontend-test-docker.sh` existe ou se precisa ser criado.
- Atualizar referências de versão do Node se necessário.

---

## 5. Contextualizar para o domínio e regras de negócio

Esta seção garante que o toolkit entenda o **negócio** do projeto — não apenas a stack. Sem isto, skills como mercenario, maestro e tradutor operam com vocabulário genérico e perdem expressividade.

### 5.1. Mapeamento do domínio

Analisar o código-fonte e a documentação para identificar:

1. **Entidades e Agregados** — classes com identidade e ciclo de vida; quais são raízes de agregado.
2. **Value Objects** — tipos imutáveis (CPF, Email, Dinheiro, Período, etc.).
3. **Domain Services** — operações stateless que envolvem múltiplas entidades.
4. **Bounded Contexts** — subdomínios do sistema (ex.: Financeiro, Logística, Autenticação); mapear fronteiras e como se comunicam.
5. **Eventos de domínio** — se o projeto usa event-driven (Domain Events, mensageria, CQRS).
6. **Invariantes e regras de negócio centrais** — restrições que o domínio impõe (ex.: "um pedido só pode ser cancelado antes do faturamento"; "o saldo nunca fica negativo").

Apresentar ao operador um **mapa resumido do domínio** para validação:

```
Bounded Contexts identificados:
  - <Contexto A>: <entidades principais> — <responsabilidade>
  - <Contexto B>: <entidades principais> — <responsabilidade>

Entidades-chave:
  - <Entidade>: <papel no domínio> — Agregado root: sim/não

Regras de negócio centrais:
  - <Regra 1>: <descrição breve>
  - <Regra 2>: <descrição breve>

Eventos de domínio:
  - <Evento>: <quem publica> → <quem consome>
```

### 5.2. Atualizar glossário unificado com termos do domínio

No arquivo `reference/glossario-unificado.md`, **adicionar uma seção específica do projeto** após as seções genéricas existentes:

```markdown
## Domínio — <Nome do Projeto>

| Termo | Definição | Contexto / Bounded Context |
|-------|-----------|---------------------------|
| **<Termo 1>** | <Definição no vocabulário do negócio> | <Bounded Context> |
| **<Termo 2>** | <Definição> | <Bounded Context> |
```

**Regras para popular o glossário:**

- Extrair termos dos nomes de classes, enums, tabelas e endpoints — **não inventar**; usar o que o código já usa.
- Quando houver divergência entre código e negócio (ex.: código usa `Order`, negócio fala "Pedido"), registrar ambos e indicar o preferido.
- Incluir siglas e acrônimos que aparecem no código ou documentação.
- Se o operador fornecer um glossário existente (wiki, Confluence, documento), usá-lo como fonte primária.

### 5.3. Linguagem ubíqua — alinhamento código ↔ negócio

Verificar consistência entre os termos do domínio e o código:

| Onde | Como verificar |
|------|---------------|
| **Classes e interfaces** | Nomes refletem o vocabulário do domínio? `Client` vs `Customer`; `Invoice` vs `NotaFiscal`. |
| **Variáveis e métodos** | Métodos de domínio usam verbos do negócio? `aprovarPedido()`, não `processItem()`. |
| **Endpoints / rotas** | URLs espelham os termos? `/api/pedidos`, não `/api/items`. |
| **Banco de dados** | Nomes de tabelas/colunas alinham com entidades? |
| **Mensagens e labels** | UI usa os mesmos termos do código e do glossário? |

Registrar **inconsistências encontradas** no resumo final como recomendação ao operador — a correção pode ser escopo de uma demanda futura.

### 5.4. Criar instruction de domínio

Se o projeto tiver complexidade de negócio suficiente, criar o arquivo:

```
.github/instructions/dominio-<nome-projeto>.instructions.md
```

Com frontmatter `applyTo: "**"` e conteúdo:

```markdown
# Domínio — <Nome do Projeto>

## Bounded Contexts

- **<Contexto A>**: <descrição, responsabilidade, entidades-chave>
- **<Contexto B>**: <descrição, responsabilidade, entidades-chave>

## Regras de negócio fundamentais

1. <Regra em linguagem natural com contexto de quando se aplica>
2. <Regra>

## Fluxos de negócio críticos

- **<Fluxo>**: <passos resumidos, pré-condições, pós-condições>

## Termos-chave (linguagem ubíqua)

| Termo do negócio | Termo no código | Notas |
|-------------------|-----------------|-------|
| <termo> | <classe/tipo> | <observação> |
```

Perguntar ao operador se este nível de documentação é desejado. Para projetos simples (CRUD, sem domínio rico), pode ser excessivo — registrar os termos no glossário basta.

### 5.5. Enriquecer skills com contexto de domínio

Atualizar os references das skills para incluir **exemplos do domínio real** do projeto, substituindo exemplos genéricos:

| Skill | O que contextualizar |
|-------|---------------------|
| **tradutor** | Seção "Glossário de domínio" — pré-popular com termos do projeto. |
| **maestro** | Seção "Linguagem Ubíqua nos requisitos" — incluir termos e entidades do projeto como referência. |
| **mercenario** | Incluir exemplos de regras de negócio do projeto para orientar o padrão de implementação (ex.: como o projeto valida invariantes, onde coloca regras). |
| **quadro-de-recompensas** | Na seção "Mapeamento ao projeto", incluir exemplos de testes do domínio (ex.: teste de regra X usando entidade Y). |
| **batedor-de-codigos** | Registrar quais padrões de domínio o projeto usa (ex.: Rich Domain Model, Anemic Model, CQRS) para calibrar a análise. |

**Não alterar a lógica das skills** — apenas enriquecer examples/references com contexto específico.

### 5.6. Mapear padrões de regras de negócio do projeto

Identificar como o projeto **implementa** regras de negócio e registrar o padrão para que as skills sigam o mesmo estilo:

- **Onde ficam as regras?** Entidades (Rich Model)? Services? Use Cases? Handlers?
- **Como são validadas as invariantes?** Exceções de domínio? Result/Either? FluentValidation?
- **Há eventos de domínio?** MediatR? Mensageria? Event sourcing?
- **Padrão de persistência:** Repository? Unit of Work? Direto no ORM?
- **Padrão de erros de negócio:** Tipos de exceção de domínio? Códigos de erro? Notification pattern?

Registrar estes padrões no instruction de domínio (§5.4) ou no glossário como nota, para que mercenario e mestre-freire sigam a convenção do projeto ao implementar ou refatorar.

---

## 6. Validar estrutura de pastas

Verificar se as pastas referenciadas pelas instructions existem:

| Instruction | Pattern esperado | Pasta |
|-------------|-----------------|-------|
| `angular-frontend.instructions.md` | `frontend/**` | `frontend/` |
| `cards-demandas.instructions.md` | `demandas/**` | `demandas/` |

- Se `demandas/` não existir, perguntar ao operador se deve ser criada (com `demandas/README.md`).
- Se `frontend/` não existir e o projeto não tiver frontend, considerar remover a instruction correspondente.

---

## 7. Configuração do GitHub (manual)

Ao final, **listar para o operador** tudo que precisa ser configurado manualmente no GitHub:

### Variables (Settings → Secrets and variables → Variables)

| Variável | Valor sugerido | Obrigatória |
|----------|---------------|-------------|
| `JWT_ISSUER_CI` | `<nome-projeto>-ci` | Se usar JWT |
| `JWT_AUDIENCE_CI` | `<nome-projeto>-clients-ci` | Se usar JWT |
| `API_PROJECT` | `<caminho .csproj da API>` | Se diferente do fallback |

### Secrets (Settings → Secrets and variables → Secrets)

| Secret | Descrição | Obrigatório |
|--------|-----------|-------------|
| `JWT_KEY` | Chave JWT para CI (min 32 bytes) | Se usar JWT |
| `AWS_ACCESS_KEY_ID` | Credencial AWS | Se usar deploy AWS |
| `AWS_SECRET_ACCESS_KEY` | Credencial AWS | Se usar deploy AWS |
| `AWS_REGION` | Região AWS | Se usar deploy AWS |
| `DB_CONNECTION_STRING` | Connection string PostgreSQL prod | Se usar deploy |

---

## 8. Relatório final

Ao concluir, apresentar ao operador:

```
✅ Alterações realizadas:
  - [lista de arquivos editados com resumo da alteração]

🏢 Domínio contextualizado:
  - Bounded Contexts identificados: [lista]
  - Termos adicionados ao glossário: [quantidade]
  - Instruction de domínio: [criado / não necessário]
  - Skills enriquecidas: [lista]
  - Padrão de regras de negócio: [Rich Model / Anemic + Services / CQRS / outro]

⚠️ Configuração manual necessária:
  - [lista de variables/secrets a configurar no GitHub]

🔍 Inconsistências de linguagem ubíqua:
  - [divergências código ↔ negócio encontradas, se houver]

❌ Pendências (requerem decisão do operador):
  - [itens que não puderam ser resolvidos automaticamente]
```

---

## Restrições

- **Não alterar a lógica das skills, agents ou prompts** — apenas adaptar paths, nomes e valores de configuração.
- **Não remover artefatos** sem confirmação do operador — se algo não se aplica ao projeto, perguntar antes.
- **Validar com o operador** antes de executar as alterações (resumo da descoberta + plano).
- **Preservar os comentários `# CONFIGURE:`** nos workflows para facilitar futuras reconfigurações.
