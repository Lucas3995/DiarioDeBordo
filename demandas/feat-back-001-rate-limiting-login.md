# Rate Limiting no Endpoint de Login

## Metadados

| Campo | Valor |
|-------|-------|
| **ID** | FEAT-BACK-001 |
| **Tipo** | Feature (segurança) |
| **Prioridade** | Alta (P2 — segurança) |
| **sourceArtifact** | `.github/reference/fonte-de-verdade-refactoring.md` — achado #19 |
| **Diretriz violada** | OWASP — Brute Force Protection, Rate Limiting |

---

## 1. Contexto

O endpoint `POST /api/auth/login` não tem rate limiting. BCrypt work factor 12 atenua força bruta (cada tentativa leva ~250ms), mas não substitui proteção de taxa. O runtime .NET 10 inclui `Microsoft.AspNetCore.RateLimiting` nativo.

**Estado atual:**
- `AuthController.Login`: aceita requests ilimitados por IP.
- `Program.cs`: CORS, JWT, Health Checks, Exception Middleware configurados; rate limiting ausente.
- Nenhuma menção a `RateLimiter`, `throttle` ou similar no projeto.

---

## 2. User Story

Como operador, quero que o endpoint de login limite tentativas por IP para impedir ataques de força bruta.

---

## 3. Critérios de aceitação

1. `POST /api/auth/login` limitado a **5 tentativas por minuto por IP** (sliding window).
2. Excedido o limite, retornar `429 Too Many Requests` com header `Retry-After`.
3. Demais endpoints não afetados (ou política separada se necessário).
4. Configuração via `appsettings.json` (PermitLimit, Window, etc.).
5. Testes de integração validam comportamento de rate limiting (< limite → 200/401; > limite → 429).
6. Header `X-RateLimit-Remaining` opcional para feedback ao frontend.

---

## 4. Alterações necessárias

1. **Configurar** `AddRateLimiter()` em `Program.cs` com `SlidingWindowRateLimiter`.
2. **Registrar** policy nomeada `"login"` com parâmetros do `appsettings.json`.
3. **Adicionar** `[EnableRateLimiting("login")]` ao método `Login` em `AuthController`.
4. **Adicionar** seção `RateLimiting` em `appsettings.json` e `appsettings.Development.json`.
5. **Middleware**: `app.UseRateLimiter()` no pipeline (após auth, antes de endpoints).
6. **Testes**: integração com `WebApplicationFactory` simulando burst de requests.

---

## 5. Requisitos técnicos/metodológicos aplicáveis

- OWASP: proteção contra brute force em endpoints de autenticação.
- .NET nativo: usar `Microsoft.AspNetCore.RateLimiting` (já incluso no runtime .NET 10).
- TDD: testes de integração com `WebApplicationFactory` para validar 429.
- Configuração externalizada: valores em `appsettings.json`, não hardcoded.
- Sliding window: mais suave que fixed window; evita burst na fronteira.
