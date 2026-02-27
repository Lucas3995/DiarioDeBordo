# Backend — Diário de Bordo

Backend da aplicação **Diário de Bordo** em .NET 9 / C# 13, seguindo Clean Architecture, CQRS, DDD, LGPD, DevSecOps e TDD.

---

## Tecnologias

| Área | Tecnologia |
|------|-----------|
| Runtime | .NET 9 / ASP.NET Core 9 |
| CQRS | MediatR 14 |
| Validação | FluentValidation 12 |
| ORM | Entity Framework Core 9 |
| Banco | PostgreSQL (Npgsql provider) |
| Documentação da API | Microsoft.AspNetCore.OpenApi 9 + Scalar |
| Autenticação | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer 9) |
| Testes | xUnit, FluentAssertions, Moq, Coverlet |
| CI/CD | GitHub Actions |

---

## Estrutura de projetos

```
backend/
├── DiarioDeBordo.sln
├── README.md                      ← este arquivo
└── src/
    ├── DiarioDeBordo.Domain/          # Entidades, Value Objects, interfaces — sem deps externas
    ├── DiarioDeBordo.Application/     # Casos de uso: Commands, Queries, Handlers, Validações
    ├── DiarioDeBordo.Persistence/     # DbContext, Migrations, repositórios concretos (EF Core)
    ├── DiarioDeBordo.Infrastructure/  # Serviços externos: MFA, email, Flowpag, criptografia
    ├── DiarioDeBordo.Api/             # Controllers, Middleware, DI, Swagger, JWT config
    └── DiarioDeBordo.Tests/           # Testes unitários e de integração
```

### Direção de dependência (Clean Architecture)

```
Api → Application → Domain
Api → Persistence → Domain
Api → Infrastructure → Domain
Tests → Application, Domain, Api
```

Camadas internas (`Domain`, `Application`) **não referenciam** frameworks, banco ou HTTP.

---

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (local ou via Docker)
- `dotnet-ef` tool: `dotnet tool install -g dotnet-ef`

---

## Executando localmente

### 1. Restaurar e buildar

```bash
cd backend
dotnet restore
dotnet build
```

### 2. Configurar variáveis de ambiente

**Nunca** coloque credenciais no `appsettings.json`. Use variáveis de ambiente:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=diariodebordo;Username=<user>;Password=<senha>;SSL Mode=Require"
export Jwt__Key="<chave-jwt-min-32-chars>"
export Jwt__Issuer="diariodebordo"
export Jwt__Audience="diariodebordo-clients"
```

> Para desenvolvimento local sem SSL, substitua `SSL Mode=Require` por `SSL Mode=Disable`.
> Em produção, a conexão com TLS é obrigatória (LGPD Art. 46).

### 3. Aplicar migrations

```bash
cd backend
dotnet ef database update \
  --project src/DiarioDeBordo.Persistence \
  --startup-project src/DiarioDeBordo.Api
```

### 4. Subir a API

```bash
cd backend
dotnet run --project src/DiarioDeBordo.Api
```

Endpoints disponíveis:

| Endpoint | Auth | Descrição |
|----------|------|-----------|
| `GET /health` | — | Health check básico |
| `GET /status` | — | Versão, ambiente e hora do servidor |
| `POST /echo` | — | Exemplo CQRS: ecoa mensagem (valida FluentValidation) |
| `GET /api/obras` | JWT Bearer | Lista obras paginadas (módulo Acompanhamento) |
| `GET /openapi/v1.json` | — | Documento OpenAPI (apenas em Development) |
| `GET /scalar/v1` | — | UI Scalar (apenas em Development) |

### 5. Executar testes

```bash
cd backend
dotnet test
```

Com cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

---

## TDD — fluxo para novas features

1. Escrever teste (unitário ou integração) **antes** do código de produção → fase RED.
2. Implementar o mínimo para o teste passar → fase GREEN.
3. Refatorar mantendo testes verdes → fase REFACTOR.

Para Commands/Queries:
- Criar `XxxCommand.cs` / `XxxQuery.cs` e `XxxResponse.cs` em `Application/<Feature>/`
- Criar validator em `XxxCommandValidator.cs`
- Criar handler em `XxxCommandHandler.cs` / `XxxQueryHandler.cs`
- Criar testes unitários em `Tests/Unit/`
- Criar controller em `Api/Controllers/` e testes de integração em `Tests/Integration/`

---

## Segurança e LGPD

| Princípio | Implementação |
|-----------|--------------|
| Autenticação | JWT Bearer (2FA a implementar na etapa de negócio) |
| Criptografia em trânsito | TLS na connection string (PostgreSQL) |
| Criptografia em campo | Interface `IDataProtectionService` (stub em `Infrastructure/Security/`) — implementar com vault externo antes de produção |
| Dados pessoais | Minimização aplicada no modelo de domínio |
| Direitos do titular | Operacionalizar via Commands (acesso, correção, eliminação, portabilidade) na etapa de negócio |
| Secrets | **Nunca** em código ou appsettings. Sempre via variáveis de ambiente ou GitHub Secrets |
| Logs | Sem dados pessoais sensíveis em mensagens de log |

---

## GitHub Actions

| Workflow | Descrição |
|----------|-----------|
| [`backend-ci.yml`](../.github/workflows/backend-ci.yml) | Restore → Build → Testes com cobertura. Executa em push/PR para `main` e `develop`. |
| [`backend-deploy.yml`](../.github/workflows/backend-deploy.yml) | Esqueleto de deploy serverless para AWS. Implementar a estratégia (Lambda, Fargate, App Runner) antes de ativar. |

### Secrets necessários para o pipeline

Configurar em **Settings → Secrets and variables → Actions** do repositório:

| Secret | Descrição |
|--------|-----------|
| `JWT_KEY_TESTS` | Chave JWT para ambiente de CI (mín. 32 chars) |
| `AWS_ACCESS_KEY_ID` | Credencial AWS (apenas para deploy) |
| `AWS_SECRET_ACCESS_KEY` | Credencial AWS (apenas para deploy) |
| `AWS_REGION` | Região AWS (ex.: `us-east-1`) |
| `DB_CONNECTION_STRING` | Connection string de produção (com TLS) |
| `JWT_KEY` | Chave JWT de produção |
| `JWT_ISSUER` | Issuer JWT de produção |
| `JWT_AUDIENCE` | Audience JWT de produção |

---

## Módulo — Acompanhamento de Obras

### Endpoint

`GET /api/obras?pageIndex=0&pageSize=10`

Requer JWT Bearer no cabeçalho `Authorization`.

Parâmetros:

| Parâmetro | Tipo | Valores permitidos | Padrão |
|-----------|------|--------------------|--------|
| `pageIndex` | int | ≥ 0 | 0 |
| `pageSize` | int | 10, 25, 50, 100 | 10 |

Resposta (200 OK):

```json
{
  "items": [
    {
      "id": "uuid",
      "nome": "One Piece",
      "tipo": "manga",
      "ultimaAtualizacaoPosicao": "2026-02-20T00:00:00Z",
      "posicaoAtual": 1110,
      "proximaInfo": { "tipo": "dias_ate_proxima", "dias": 5 },
      "ordemPreferencia": 1
    }
  ],
  "totalCount": 12
}
```

### Modelo de domínio — Obra

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | Guid | Identificador único |
| `Nome` | string | Nome da obra |
| `Tipo` | TipoObra (enum) | manga, manhwa, manhua, anime, livro, filme, serie, webnovel |
| `PosicaoAtual` | int | Número da parte atual |
| `DataUltimaAtualizacaoPosicao` | DateTime (UTC) | Data da última atualização |
| `OrdemPreferencia` | int | Ordenação default (menor = maior preferência) |
| `ProximaInfoTipo` | enum? | Discriminador para previsão |
| `DiasAteProximaParte` | int? | Dias até a próxima parte |
| `PartesJaPublicadas` | int? | Partes publicadas desde a última atualização |

### Seed de dados (Development)

Em `ASPNETCORE_ENVIRONMENT=Development`, a aplicação popula automaticamente a tabela `Obras` com 12 registros de exemplo (equivalentes ao mock do frontend) na primeira inicialização quando a tabela estiver vazia.

---

## Migrations — criar nova migration

```bash
cd backend
dotnet ef migrations add <NomeDaMigration> \
  --project src/DiarioDeBordo.Persistence \
  --startup-project src/DiarioDeBordo.Api
```
