#!/usr/bin/env bash
# Arauto — script de entrega ao remoto: contexto (status/diff), add, commit, push, PR, watch workflows, checkout main + fetch + pull.
# Uso: invocar uma vez com --commit-msg, --pr-title e --pr-body; o script imprime status/diff no início, faz git add -A
# (respeitando .gitignore), commit, push, PR, watch e, em sucesso, checkout main + fetch + pull. O agente só precisa
# de uma invocação e lê o retorno; só age de novo se ARAUTO_RESULT=failure (reportar e sugerir correções).
#
# Resultado para o agente (linhas parseáveis; o agente deve interpretá-las e agir em conformidade):
#   ARAUTO_RESULT=success   — workflows verdes, main atualizada; agente informa "Entrega concluída" e PR_URL.
#   ARAUTO_RESULT=failure   — workflow(s) falharam; ARAUTO_RUN_ID e logs abaixo; agente reporta falha e sugere correções.
#   ARAUTO_RESULT=no_gh     — gh não disponível/autenticado; agente pede verificação manual na aba Actions.
#   ARAUTO_RESULT=pr_only   — PR criado com --no-watch; agente informa PR e que a validação não foi feita.
#   ARAUTO_RESULT=no_run    — nenhum run apareceu; main atualizada; agente pede verificação manual dos workflows.
#   ARAUTO_PR_URL=...       — URL do PR quando existir (para o agente informar ao utilizador).
#   ARAUTO_RUN_ID=...       — em caso de falha, ID do run que falhou (para o agente referenciar nos relatórios).
#   --preview               — só imprime git status e diffs (staged + unstaged) e sai; evita que o agente rode git separadamente.

set -euo pipefail

COMMIT_MSG=""
PR_TITLE=""
PR_BODY=""
PR_BODY_FILE=""
SKIP_WATCH=false
DRY_RUN=false
PREVIEW=false

while [[ $# -gt 0 ]]; do
  case "$1" in
    --preview)
      PREVIEW=true
      shift
      ;;
    --commit-msg)
      COMMIT_MSG="$2"
      shift 2
      ;;
    --pr-title)
      PR_TITLE="$2"
      shift 2
      ;;
    --pr-body)
      PR_BODY="$2"
      shift 2
      ;;
    --pr-body-file)
      PR_BODY_FILE="$2"
      shift 2
      ;;
    --no-watch)
      SKIP_WATCH=true
      shift
      ;;
    --dry-run)
      DRY_RUN=true
      shift
      ;;
    -h|--help)
      echo "Uso: arauto.sh [--preview] [--commit-msg \"mensagem\"] [--pr-title \"título\"] [--pr-body \"corpo\" | --pr-body-file caminho] [--no-watch] [--dry-run]"
      echo "  Sem --preview: imprime status/diffs, faz git add -A, commit (se --commit-msg), push, PR, watch e em sucesso checkout main + fetch + pull."
      echo "  --preview       Apenas imprime git status e diffs e sai (opcional)."
      echo "  --commit-msg    Mensagem para git commit (após add -A)."
      echo "  --pr-title     Título para gh pr create."
      echo "  --pr-body       Corpo do PR. Alternativa: --pr-body-file."
      echo "  --pr-body-file  Ficheiro com o corpo do PR."
      echo "  --no-watch      Não esperar pelos workflows."
      echo "  --dry-run       Apenas mostrar o que seria feito."
      exit 0
      ;;
    *)
      echo "Argumento desconhecido: $1" >&2
      exit 1
      ;;
  esac
done

run() {
  if [[ "$DRY_RUN" == true ]]; then
    echo "[dry-run] $*"
  else
    "$@"
  fi
}

# Emite linha de resultado para o agente interpretar e agir (sucesso, falha, ou necessidade de verificação manual).
emit_result() {
  echo "ARAUTO_RESULT=$1"
  [[ -n "${2:-}" ]] && echo "ARAUTO_RUN_ID=$2"
  [[ -n "${3:-}" ]] && echo "ARAUTO_PR_URL=$3"
}

BRANCH=$(git branch --show-current)
if [[ -z "$BRANCH" ]]; then
  echo "Erro: não foi possível obter a branch atual." >&2
  exit 1
fi

# Modo preview: só status e diffs (reduz créditos — o agente não precisa rodar git status/diff separadamente).
if [[ "$PREVIEW" == true ]]; then
  echo "=== Arauto: preview (git status e diffs) ==="
  echo "Branch: $BRANCH"
  echo "--- git status ---"
  git status
  echo "--- git diff --staged ---"
  git diff --staged || true
  echo "--- git diff (unstaged) ---"
  git diff || true
  echo "ARAUTO_PREVIEW=1"
  echo "=== Use esta saída para preparar --commit-msg, --pr-title e --pr-body; depois execute o script sem --preview. ==="
  exit 0
fi

echo "=== Arauto: início ==="
echo "Branch: $BRANCH"

# Contexto (status + diff) no início da mesma execução — o agente não precisa rodar git status/diff separadamente.
echo "--- git status ---"
git status
echo "--- git diff --staged ---"
git diff --staged || true
echo "--- git diff (unstaged) ---"
git diff || true

# Add: o script faz git add -A (respeita .gitignore); o agente não executa git add.
echo "=== git add -A (respeitando .gitignore) ==="
run git add -A

# 1) Commit (se pedido e houver staged)
if [[ -n "$COMMIT_MSG" ]]; then
  if git diff --staged --quiet 2>/dev/null; then
    echo "Nada em stage após add; a ignorar commit."
  else
    echo "=== Commit ==="
    if [[ "$DRY_RUN" == true ]]; then
      echo "[dry-run] git commit com mensagem fornecida"
    else
      TF=$(mktemp)
      printf '%s\n' "$COMMIT_MSG" > "$TF"
      git commit -F "$TF"
      rm -f "$TF"
    fi
  fi
fi

# 2) Push
echo "=== Push ==="
if run git push -u origin "$BRANCH" 2>&1; then
  true
else
  EXIT_PUSH=$?
  # Pode ser que a branch já exista no remoto; tentar só push
  if run git push 2>&1; then
    true
  else
    echo "Push falhou (exit $EXIT_PUSH). Verifique restrições da branch (ex.: main/develop protegidas)." >&2
    exit "$EXIT_PUSH"
  fi
fi

# gh disponível?
if ! command -v gh &>/dev/null; then
  echo "gh não encontrado. PR e validação de workflows omitidos. Faça o PR e a verificação manualmente na aba Actions." >&2
  emit_result "no_gh"
  echo "=== Arauto: fim (sem gh) ==="
  exit 0
fi

if ! gh auth status &>/dev/null; then
  echo "gh não autenticado. PR e validação de workflows omitidos. Faça o PR e a verificação manualmente." >&2
  emit_result "no_gh"
  echo "=== Arauto: fim (gh não autenticado) ==="
  exit 0
fi

# 3) PR
echo "=== PR ==="
if gh pr view &>/dev/null; then
  echo "PR já existe."
  gh pr view
else
  if [[ -z "$PR_TITLE" ]]; then
    echo "Aviso: --pr-title não fornecido; a usar título genérico." >&2
    PR_TITLE="Update $BRANCH"
  fi
  if [[ -n "$PR_BODY_FILE" ]] && [[ -f "$PR_BODY_FILE" ]]; then
    run gh pr create --title "$PR_TITLE" --body-file "$PR_BODY_FILE"
  elif [[ -n "$PR_BODY" ]]; then
    run gh pr create --title "$PR_TITLE" --body "$PR_BODY"
  else
    run gh pr create --title "$PR_TITLE"
  fi
fi
PR_URL=$(gh pr view --json url -q '.url' 2>/dev/null || true)

if [[ "$SKIP_WATCH" == true ]]; then
  emit_result "pr_only" "" "$PR_URL"
  echo "=== Arauto: fim (--no-watch) ==="
  exit 0
fi

# 4) Workflows: esperar run aparecer e fazer watch
echo "=== Workflows ==="
RUN_ID=""
for i in {1..12}; do
  RUN_ID=$(gh run list --branch "$BRANCH" --limit 1 --json databaseId,status --jq '.[0].databaseId // empty' 2>/dev/null || true)
  if [[ -n "$RUN_ID" ]]; then
    break
  fi
  echo "À espera de run na branch $BRANCH... (tentativa $i/12)"
  sleep 10
done

if [[ -z "$RUN_ID" ]]; then
  echo "Nenhum run encontrado para a branch. Verifique manualmente a aba Actions. A prosseguir para main." >&2
  run git checkout main
  run git fetch
  run git pull
  emit_result "no_run" "" "$PR_URL"
  echo "=== Arauto: main atualizada; validação de workflows não foi possível (nenhum run). ==="
  exit 0
fi

echo "A monitorar run ID: $RUN_ID"
if ! run gh run watch "$RUN_ID"; then
  echo "=== Workflow falhou ==="
  emit_result "failure" "$RUN_ID" "$PR_URL"
  echo "--- Logs do(s) step(s) que falharam (para o agente sugerir correções) ---"
  gh run view "$RUN_ID" --log-failed 2>/dev/null || gh run view "$RUN_ID"
  echo "--- Fim dos logs ---"
  echo "Entrega não concluída. Corrija e volte a executar o script após novo push." >&2
  exit 1
fi
echo "Workflows concluídos com sucesso."

# 5) Checkout main, fetch, pull
echo "=== Voltar à main e atualizar ==="
run git checkout main
run git fetch
run git pull

emit_result "success" "" "$PR_URL"
echo "=== Arauto: entrega concluída. Repositório local em main atualizada. ==="
