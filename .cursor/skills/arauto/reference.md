# Arauto — Referência

Conteúdo complementar à skill Arauto: script de execução, exemplos de commit, template de PR, classificação de workflows e alinhamento com o projeto.

---

## Script `scripts/arauto.sh`

Localização: `.cursor/skills/arauto/scripts/arauto.sh`. Executável; invocar **uma vez** a partir da raiz do repositório. O script faz tudo; o agente não executa `git add`, `git status` nem `git diff` separadamente.

- **No início da execução:** imprime `git status` e diffs (staged + unstaged) para o agente ter contexto na mesma saída.
- **Add:** executa `git add -A` (respeita `.gitignore`); o agente não faz add.
- **Preview (opcional):** `--preview` — só imprime status e diffs e sai. O agente pode usar esta saída para redigir mensagens de commit e corpo do PR mais claras, concisas e eficientes; em seguida executa o script com esses argumentos.
- **Commit:** `--commit-msg "mensagem"` (após o add do script).
- **PR:** `--pr-title "título"` e `--pr-body "corpo"` ou `--pr-body-file caminho`.
- **Workflows:** por defeito faz `gh run watch` no primeiro run da branch; `--no-watch` omite.
- **Ao terminar com sucesso:** `git checkout main`, `git fetch`, `git pull` (o script faz sozinho).

**Resultado para o agente (linhas parseáveis):** o script emite `ARAUTO_RESULT=...` e, quando aplicável, `ARAUTO_RUN_ID=...` e `ARAUTO_PR_URL=...`. O agente **deve** interpretar essas linhas e agir em conformidade (informar sucesso e URL, ou reportar falha com base nos logs e sugerir correções, ou pedir verificação manual conforme o resultado). Em caso de `failure`, os logs do run que falhou são impressos entre marcadores para o agente extrair e sugerir correções concretas.

---

## Exemplos de mensagens de commit

**Exemplo 1 — feature:**

```
feat(auth): adiciona endpoint de login com JWT e seed do usuário admin

Implementa POST /api/auth/login que valida credenciais via BCrypt e
retorna token JWT (HS256, 8h). Usuários com Requer2FA=true recebem
resposta sem token, indicando que o segundo fator deve ser informado.

Impacto funcional: permite que o frontend obtenha um token para acessar
rotas protegidas. Impacto técnico: novas camadas Auth em Domain,
Application e Infrastructure; migration CriarTabelaUsuarios aplicada.
```

**Exemplo 2 — fix:**

```
fix(docker): corrige range de IP no pg_hba.conf para subnets dinâmicas

Substitui 172.17.0.0/16 por 172.0.0.0/8 nas instruções do
COMO_RODAR_BACK_E_FRONT.txt. O Docker Compose aloca subnets
dinamicamente (172.17.x, 172.18.x, 172.19.x…), e o range restrito
impedia a conexão do container ao PostgreSQL do host.
```

---

## Template completo do corpo do PR

```markdown
## O que foi feito

Descrição clara e amigável do que foi implementado ou corrigido.

## Impacto funcional

O que o utilizador/negócio passa a conseguir fazer (ou que problema foi
resolvido) após esse PR.

## Impacto técnico

- Camadas / ficheiros adicionados ou alterados
- Novos endpoints, migrations, configurações
- Testes adicionados (unitários / integração)
- Dependências adicionadas ou removidas

## Como testar

Passos para validar as alterações localmente ou no ambiente de desenvolvimento.

Refs: #<issue> (se houver)
```

---

## Classificação de workflows (exemplo deste projeto)

| Job              | Tipo        | Critério                                              |
|------------------|------------|--------------------------------------------------------|
| `lint-build-test` | **crítico** | Sem `continue-on-error` — falha bloqueia o workflow   |
| `e2e`            | **opcional** | `continue-on-error: true` — falha é informacional     |

Obter IDs dos runs:

```bash
gh run list --branch "$BRANCH" --limit 20 --json databaseId,status,name \
  | jq '.[] | select(.status == "in_progress" or .status == "queued") | .databaseId'
```

---

## Alinhamento com as regras do projeto

Este fluxo está alinhado com as regras de DevSecOps do projeto (ex.: `regras/devsecops.mdc`):

- Mensagens de commit explícitas e descritivas.
- PR como gate obrigatório (sem push direto em `main`/`develop`).
- Build e testes devem estar verdes para considerar a entrega concluída.
- Segredos não devem ser commitados.

A validação pós-PR é obrigatória para que a entrega só seja considerada concluída quando as automações disparadas pelo PR tiverem sucesso.
