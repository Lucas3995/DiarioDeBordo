Ao terminar uma tarefa:
1. Garantir todos os testes (backend e frontend) passam localmente.
2. Executar `docker compose -f docker/docker-compose.yml build` para verificar build do frontend.
3. Rodar `./scripts/coverage.sh` para ver cobertura atual.
4. Commitar com mensagem clara descrevendo a alteração.
5. Push e abrir PR via `gh` ou manualmente.
6. Usar `gh run watch` para esperar workflows.
7. Se CI verde, checkout main, pull e considerar entrega concluída.
8. Se algum workflow falhar, criar nova demanda e reexecutar rotina-completa.
