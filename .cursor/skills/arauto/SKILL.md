---
name: arauto
description: Executa o fluxo completo de entrega ao repositório remoto: commit com mensagem descritiva, push, abertura de pull request (GitHub CLI) e validação obrigatória dos workflows do PR com gh run watch. Considera a entrega concluída somente quando todos os workflows críticos estiverem verdes; em falha, reporta o motivo e sugere correções. Usar quando o utilizador pedir para entregar no remoto, fazer commit e push, abrir PR, criar pull request, validar CI, ou verificar se as automações do PR passaram.
---

# Arauto — Entrega ao repositório remoto

Fluxo em 4 etapas. Executar em sequência; não pular etapas. Entrega só está concluída quando os workflows do PR estiverem verdes (ou o utilizador confirmar manualmente se `gh` não estiver disponível).

---

## 1. Commit

**Antes de gerar a mensagem:**

1. `git status` e `git diff --staged` (e `git diff` se necessário) para saber o que será commitado.
2. Incluir apenas ficheiros relevantes; nunca segredos, `.env`, nem o que deva estar no `.gitignore`.
3. `git add` apenas do que deve ir no commit.

**Mensagem:**

- Convenção: `tipo(escopo): descrição` (feat, fix, refactor, docs, chore, test, ci).
- Descritiva e informativa: um leitor deve entender **o que foi feito**, **impacto funcional** (o que o utilizador/negócio passa a ter) e **impacto técnico** (camadas, ficheiros, endpoints).
- Usar corpo do commit (linha em branco após o título + parágrafos) quando o título não bastar.

**Formato:**

```
tipo(escopo): título curto e claro (≤ 72 caracteres)

Parágrafo com o que foi feito, impacto funcional e técnico.

Refs: #<issue> (se houver)
```

Exemplos detalhados em [reference.md](reference.md).

---

## 2. Push

```bash
git push -u origin <branch>   # primeira vez
git push                       # branch já rastreada
```

- Nunca usar `--force` em `main` ou `develop`. Se a branch estiver protegida, reportar e usar PR.
- Se o push for rejeitado por restrição de branch principal, informar o utilizador e seguir para o passo 3.

---

## 3. Abrir PR

É obrigatório ter um **PR aberto** para este push e validar as automações **desse** PR. Se a branch já tiver um PR mas estiver **MERGED** ou **CLOSED**, criar um **novo** PR para o push atual (não reutilizar o antigo).

**Verificar estado do PR da branch:**

```bash
gh pr view --json state -q '.state'
```

- Se `state == OPEN`: o PR aberto já existe; usar e ir para o passo 4.
- Se não existir PR ou `state` for `MERGED` ou `CLOSED`: criar novo com `gh pr create` (título e corpo descritivos) e depois validar as automações desse novo PR.

**Corpo do PR:** descritivo — o que foi feito, impacto funcional, impacto técnico, como testar. Template em [reference.md](reference.md).

```bash
gh pr create --title "tipo(escopo): título descritivo" --body "$(cat <<'EOF'
## O que foi feito
[Descrição clara.]

## Impacto funcional
[O que o utilizador/negócio passa a ter.]

## Impacto técnico
- Camadas/ficheiros, endpoints, migrations, testes, dependências

## Como testar
[Passos para validar.]

Refs: #<issue> (se houver)
EOF
)"
```

---

## 4. Validação dos workflows (obrigatória)

Validar as automações do **PR aberto** que corresponde a este push (o que foi criado ou o que já estava OPEN). A entrega só está concluída quando todos os **workflows críticos** desse PR estiverem com conclusão **success**. Jobs com `continue-on-error: true` são informativos; falha neles não bloqueia.

### 4.1 Listar runs da branch

```bash
BRANCH=$(git branch --show-current)
gh run list --branch "$BRANCH" --limit 20
```

Se não aparecer nenhum run, aguardar ~10s e repetir (GitHub Actions pode demorar a enfileirar).

### 4.2 Monitorar jobs críticos primeiro

1. Identificar na listagem quais jobs são críticos (sem `continue-on-error`) e quais são opcionais (com `continue-on-error: true`). Exemplo neste projeto: `lint-build-test` = crítico; `e2e` = opcional.
2. Assistir apenas os runs/jobs críticos:

```bash
gh run watch <run-id-critico>
```

3. Se um job crítico **falhar**: obter logs com `gh run view <run-id> --log-failed`, diagnosticar, reportar ao utilizador com sugestões concretas de correção, e **não** marcar a entrega como concluída. Após correção (commit/push), voltar ao passo 4.1.
4. Se todos os críticos estiverem **success**: opcionalmente aguardar jobs opcionais para relato; não bloqueiam.

### 4.3 Resultado final

```bash
gh run list --branch "$BRANCH" --limit 20
```

- **Todos os runs críticos com `conclusion = success`:** informar «Entrega concluída. Todos os workflows do PR estão verdes.» e sugerir `gh pr view --web` para abrir no browser.
- **Algum run com `conclusion = failure` (ou `cancelled`):** usar `gh run view <run-id>` e `gh run view <run-id> --log-failed`, reportar workflow/job/step que falhou, resumo do erro e sugestões concretas de correção; não marcar como concluído.

---

## Escape: `gh` não disponível ou não autenticado

1. Informar o utilizador.
2. Omitir a validação automatizada (passo 4).
3. Indicar a URL do PR para verificação manual na aba **Actions** do GitHub.
4. Não considerar a entrega concluída sem confirmação manual do utilizador.

---

## Script (opcional)

O script `.cursor/skills/arauto/scripts/arauto.sh` implementa este fluxo: só reutiliza PR existente se `state == OPEN`; se MERGED/CLOSED ou inexistente, cria novo PR e faz `gh run watch` nos runs desse PR. Em caso de falha, emite `ARAUTO_RESULT=failure` e logs para o agente sugerir correções.

## Recursos adicionais

- Exemplos de commit, template de PR, classificação de workflows e alinhamento com regras do projeto: [reference.md](reference.md).
