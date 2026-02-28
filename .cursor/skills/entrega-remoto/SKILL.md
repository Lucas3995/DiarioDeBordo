---
name: entrega-remoto
description: Executa o fluxo completo de entrega ao repositório remoto: commit com mensagem descritiva e amigável, push, abertura de pull request via GitHub CLI e validação obrigatória dos workflows pós-PR com gh run watch. Considera a entrega concluída somente quando todos os workflows do PR estiverem verdes; em caso de falha reporta o motivo e sugere correções concretas. Usar quando o usuário pedir para entregar no remoto, fazer commit e push, abrir PR, criar pull request, validar CI, ou verificar se os workflows passaram.
---

# Entrega ao repositório remoto

Fluxo obrigatório para toda entrega ao remoto. Seguir os 4 blocos em sequência; não pular etapas.

---

## 1. Commit

**Antes de gerar a mensagem:**

1. Executar `git status` para ver o estado atual.
2. Executar `git diff --staged` (e `git diff` se ainda não staged) para entender o que será commitado.
3. Revisar quais arquivos devem ir no commit; não incluir segredos, `.env`, nem arquivos que devam estar no `.gitignore`.
4. Fazer `git add` apenas dos arquivos relevantes.

**Mensagem de commit:**

- Usar a convenção `tipo(escopo): descrição` (feat, fix, refactor, docs, chore, test, ci).
- A mensagem deve ser **descritiva, amigável e informativa**: um leitor futuro deve ler apenas o commit e entender:
  - **O que foi feito** (funcionalidade nova, correção, refatoração, etc.).
  - **Qual a diferença para a funcionalidade** — o que o usuário ou o negócio passa a ter ou não tem mais.
  - **Qual a diferença para a parte técnica** — quais camadas, arquivos ou fluxos foram alterados.
- Usar o corpo do commit (linha em branco após o título + parágrafos) sempre que o título sozinho não for suficiente para transmitir contexto funcional e técnico.

**Formato de referência:**

```
tipo(escopo): título curto e claro (≤ 72 caracteres)

Parágrafo descrevendo o que foi feito e por quê, de forma amigável.
Incluir impacto funcional (o que o usuário passa a conseguir fazer ou
que bug foi corrigido) e técnico (camadas, classes, endpoints alterados).

Refs: #<issue> (se houver)
```

**Exemplos:**

```
feat(auth): adiciona endpoint de login com JWT e seed do usuário admin

Implementa POST /api/auth/login que valida credenciais via BCrypt e
retorna token JWT (HS256, 8h). Usuários com Requer2FA=true recebem
resposta sem token, indicando que o segundo fator deve ser informado.

Impacto funcional: permite que o frontend obtenha um token para acessar
rotas protegidas. Impacto técnico: novas camadas Auth em Domain,
Application e Infrastructure; migration CriarTabelaUsuarios aplicada.
```

```
fix(docker): corrige range de IP no pg_hba.conf para subnets dinâmicas

Substitui 172.17.0.0/16 por 172.0.0.0/8 nas instruções do
COMO_RODAR_BACK_E_FRONT.txt. O Docker Compose aloca subnets
dinamicamente (172.17.x, 172.18.x, 172.19.x…), e o range restrito
impedia a conexão do container ao PostgreSQL do host.
```

---

## 2. Push

```bash
git push -u origin <branch>   # primeira vez
git push                       # branch já rastreada
```

- **Nunca** usar `--force` na branch `main` ou `develop`. Se a branch estiver protegida, reportar e orientar a usar PR.
- Se o push for rejeitado por restrição de branch principal, não tentar contornar; informar o usuário e ir para o passo 3.

---

## 3. Abrir PR

**Verificar se o PR já existe:**

```bash
gh pr view
```

- Se existir: informar o link, não criar outro. Ir diretamente para o passo 4.
- Se não existir: criar com `gh pr create`.

**Corpo do PR — obrigatório ser descritivo, amigável e informativo:**

Um leitor futuro deve ler o PR e entender:
- **O que foi feito** nessas alterações.
- **Diferença funcional** — o que o usuário/negócio passa a ter depois desse PR.
- **Diferença técnica** — mudanças de arquitetura, endpoints, testes, migrations, configurações.

Usar este template como base (adaptar ao conteúdo real):

```bash
gh pr create --title "tipo(escopo): título descritivo" --body "$(cat <<'EOF'
## O que foi feito

Descrição clara e amigável do que foi implementado ou corrigido.

## Impacto funcional

O que o usuário/negócio passa a conseguir fazer (ou que problema foi
resolvido) após esse PR.

## Impacto técnico

- Camadas / arquivos adicionados ou alterados
- Novos endpoints, migrations, configurações
- Testes adicionados (unitários / integração)
- Dependências adicionadas ou removidas

## Como testar

Passos para validar as alterações localmente ou no ambiente de desenvolvimento.

Refs: #<issue> (se houver)
EOF
)"
```

---

## 4. Validação pós-criação de PR (obrigatória)

Esta etapa é **obrigatória**. A entrega só está concluída quando todos os workflows estiverem verdes.

### 4.1 Identificar a branch e listar runs

```bash
BRANCH=$(git branch --show-current)
gh run list --branch "$BRANCH" --limit 20
```

Se nenhum run aparecer ainda (GitHub Actions pode demorar alguns segundos para enfileirar), aguardar ~10s e repetir.

### 4.2 Monitorar runs em paralelo — críticos antes de opcionais

Os workflows deste projeto disparam jobs em paralelo. Classificar antes de aguardar:

| Job | Tipo | Critério |
|---|---|---|
| `lint-build-test` | **crítico** | sem `continue-on-error` — falha bloqueia o workflow |
| `e2e` | **opcional** | `continue-on-error: true` — falha é informacional, nunca bloqueia PR |

**Passo a passo:**

1. Identificar os IDs dos jobs críticos vs opcionais da listagem do passo 4.1.

2. Assistir **apenas os jobs críticos** primeiro:

```bash
gh run watch <run-id-critico>
```

3. Se um job crítico terminar com **falha**:
   - **Agir imediatamente** — obter logs via `gh run view <run-id> --log-failed`.
   - **Não aguardar** os jobs opcionais ainda em andamento.
   - Diagnosticar, corrigir, fazer novo push e retornar ao passo 4.1.

4. Se todos os jobs críticos terminaram com **sucesso**:
   - Verificar se algum job opcional ainda está rodando.
   - Aguardá-lo apenas para **relato informacional** (não bloqueia a entrega).

```bash
# Obter IDs separados por tipo
gh run list --branch "$BRANCH" --limit 20 --json databaseId,status,name \
  | jq '.[] | select(.status == "in_progress" or .status == "queued") | .databaseId'
```

Repetir até não haver mais jobs críticos pendentes.

### 4.3 Verificar resultado final

```bash
gh run list --branch "$BRANCH" --limit 20
```

Verificar a coluna `conclusion` de cada run disparado por esse PR/push.

### 4.4 Critério de conclusão

**Todos os runs com `conclusion = success`:**

Informar ao usuário:

> Entrega concluída. Todos os workflows do PR estão verdes.
> PR: `gh pr view --web` para abrir no browser.

**Algum run com `conclusion = failure` (ou `cancelled`):**

1. Obter detalhes do run que falhou:

```bash
gh run view <run-id>
```

2. Obter logs do(s) step(s) que falharam:

```bash
gh run view <run-id> --log-failed
```

3. Reportar ao usuário:
   - Qual workflow/job/step falhou.
   - Resumo do erro (mensagem do log).
   - **Sugestões concretas de correção** (ex.: teste falhando → qual teste, qual assertion; lint error → arquivo e regra; build error → dependência ou tipo).

4. **Não marcar a tarefa como concluída.** Aguardar o usuário corrigir, fazer novo commit/push e, após o push, retornar ao passo 4.1.

---

## Escape: `gh` não disponível ou não autenticado

Se `gh` não estiver instalado ou autenticado:

1. Informar ao usuário.
2. Pular o passo 4 de validação automatizada.
3. Indicar a URL do PR para que o usuário verifique manualmente a aba **Actions** no GitHub.
4. Não considerar a entrega como concluída sem confirmação manual do usuário.

---

## Alinhamento com as regras do projeto

Este fluxo está alinhado com [`devsecops.mdc`](.cursor/rules/devsecops.mdc):

- Mensagens de commit explícitas e descritivas.
- PR como gate obrigatório (sem push direto na `main`/`develop`).
- Build e testes devem estar verdes para considerar a entrega concluída.
- Segredos não devem ser commitados.

Contexto histórico: a validação pós-PR foi incluída porque anteriormente o fluxo encerrava após abrir o PR sem verificar se as automações passavam — ver [`docs/retrospectiva-modulo-acompanhamento-e-auth.md`](docs/retrospectiva-modulo-acompanhamento-e-auth.md).
