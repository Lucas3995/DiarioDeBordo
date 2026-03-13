## Comandos comuns

### Backend
- `dotnet build` / `dotnet test` dentro de `backend/` ou subpastas.
- `cd backend && dotnet test src/DiarioDeBordo.Tests` (unitários/integr) 
- `./scripts/coverage.sh` (cobertura completa). Relatórios em `backend/TestResults`.
- `docker compose -f docker/docker-compose.yml build` / `up` para frontend local.
- `./scripts/frontend-test-docker.sh` para testes do frontend em container (Node22+Chromium).

### Frontend
- `npm install` depois no `frontend/`.
- `npm run start` / `ng serve` para dev.
- `npm run test` (karma/vite) / `npm run e2e` para testes.
- `npm run lint` / `npm run build`.

### Geral
- `git` para versionamento, PRs com mensagens descritivas.
- `gh` CLI para criar PRs; workflow API/back/cover/ci.
- `grep`, `find`, `ls`, etc. padrão Linux.
- `./scripts/coverage.sh` para cobertura backend+frontend.
- Docker commands como `docker build` etc. para componentes.  

### Execução
- Backend: `dotnet run --project backend/src/DiarioDeBordo.Api` (development).
- Frontend: `npm run start` ou via Docker Compose (`docker compose -f docker/docker-compose.yml up`).
