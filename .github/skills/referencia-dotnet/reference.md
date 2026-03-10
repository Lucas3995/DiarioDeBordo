# Referência .NET — Catálogo de Features e Padrões

---

## §1 Caching em ASP.NET Core

Três estratégias complementares de cache, cada uma para cenários distintos.

### §1.1 In-Memory Cache (IMemoryCache)

**O que é:** Cache em memória do processo da aplicação. Mais rápido e simples.
**Quando usar:** Servidor único ou sticky sessions; dados frequentemente acessados que toleram stale breve.

**Setup:**
```csharp
// Program.cs
builder.Services.AddMemoryCache();
```

**Uso — padrão GetOrCreateAsync (thread-safe):**
```csharp
var value = await cache.GetOrCreateAsync("key", async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    return await repository.GetExpensiveDataAsync();
});
```

**Regras:**
- Usar `GetOrCreateAsync` para operações atômicas (evita race conditions)
- Lifetimes curtos para dados que mudam frequentemente
- Não armazenar objetos grandes — consome memória do processo
- Não usar em web farms sem sticky sessions (cada instância tem cache próprio)

---

### §1.2 Distributed Cache (IDistributedCache)

**O que é:** Cache em serviço externo (Redis, SQL Server). Compartilhado entre instâncias.
**Quando usar:** Web farms, microservices, cloud — dados precisam sobreviver a restarts.

**Setup (Redis):**
```csharp
// Package: Microsoft.Extensions.Caching.StackExchangeRedis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
```

**Uso:**
```csharp
// Gravar
var json = JsonSerializer.Serialize(data);
await cache.SetStringAsync("key", json, new DistributedCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
});

// Ler
var cached = await cache.GetStringAsync("key");
if (cached is not null)
    return JsonSerializer.Deserialize<T>(cached);
```

**Regras:**
- Sempre usar métodos `Async` (evita thread blocking)
- Serialização JSON — tratar null/deserialize com cuidado
- Definir expiração explícita (prevenir memory leaks no Redis)
- Tratar falha de conexão ao Redis gracefully (fallback ao DB)

---

### §1.3 Response Caching

**O que é:** Cache a nível de resposta HTTP. Evita re-execução de código para requests idênticos.
**Quando usar:** GET requests idempotentes; conteúdo que não varia por usuário.

**Setup:**
```csharp
// Program.cs
builder.Services.AddResponseCaching();
app.UseResponseCaching(); // Middleware — antes de endpoints
```

**Uso:**
```csharp
[ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "category" })]
public IActionResult GetProducts(string category) { ... }
```

**Parâmetros do atributo:**

| Parâmetro | Propósito |
|-----------|-----------|
| `Duration` | Segundos de cache |
| `Location` | `Any` (public), `Client` (private), `None` |
| `VaryByQueryKeys` | Cacheia separado por query param |
| `VaryByHeader` | Cacheia separado por header |

**Regras:**
- Apenas para GET requests idempotentes
- Nunca cachear dados sensíveis ou user-specific em `Location = Any`
- Combinar com HTTP cache headers para controle de proxy/CDN

---

### §1.4 Decision Table — Qual cache usar

| Critério | In-Memory | Distributed | Response |
|----------|-----------|-------------|----------|
| **Arquitetura** | Servidor único | Web farm / microservices | Qualquer |
| **Granularidade** | Qualquer objeto | Qualquer objeto (serializado) | Resposta HTTP inteira |
| **Sobrevive a restart** | Não | Sim | Depende de proxy/CDN |
| **Compartilhado entre instâncias** | Não | Sim | Via proxy |
| **Velocidade** | Mais rápido | Network I/O | Mais rápido (evita código) |
| **Complexidade** | Baixa | Média (Redis, serialização) | Baixa |
| **Ideal para** | Lookups, config | Sessions, dados entre serviços | APIs públicas, assets |

---

## §2 EF Core — Features Avançadas

### §2.1 Shadow Properties

**O que é:** Propriedades que existem no banco mas **não** na classe C# da entidade. Mantém domain model limpo.
**Quando usar:** Campos de auditoria (CreatedAt, UpdatedAt), metadata, foreign keys implícitas.

```csharp
// Configuration
modelBuilder.Entity<Order>()
    .Property<DateTime>("CreatedAt")
    .HasDefaultValueSql("GETUTCDATE()");

// Acesso via EF
var createdAt = context.Entry(order).Property<DateTime>("CreatedAt").CurrentValue;

// Query
context.Orders.OrderBy(o => EF.Property<DateTime>(o, "CreatedAt"));
```

**Regra:** Usar para metadata do banco que não pertence ao domínio.

---

### §2.2 Query Tags

**O que é:** Comentários adicionados ao SQL gerado. Aparecem nos logs de SQL.
**Quando usar:** Debug de performance, identificação de queries em produção.

```csharp
var orders = context.Orders
    .TagWith("GetPendingOrders - OrderService")
    .Where(o => o.Status == Status.Pending)
    .ToListAsync();
// SQL gerado inclui: -- GetPendingOrders - OrderService
```

**Regra:** Adicionar em queries complexas ou críticas para rastreabilidade.

---

### §2.3 Compiled Queries

**O que é:** Queries pré-compiladas que eliminam custo de parsing e planning.
**Quando usar:** Queries executadas repetidamente em paths de alta frequência.

```csharp
private static readonly Func<AppDbContext, int, Task<Order?>> GetOrderById =
    EF.CompileAsyncQuery((AppDbContext ctx, int id) =>
        ctx.Orders.FirstOrDefault(o => o.Id == id));

// Uso
var order = await GetOrderById(context, orderId);
```

**Regra:** Usar em hot paths; queries raramente executadas não justificam overhead de declaração.

---

### §2.4 DbContext Pooling

**O que é:** Reutiliza instâncias de DbContext em vez de criar novas a cada request.
**Quando usar:** Aplicações de alta concorrência.

```csharp
// Program.cs — trocar AddDbContext por AddDbContextPool
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(connectionString),
    poolSize: 128); // default: 1024
```

**Regras:**
- Não usar estado em DbContext (será reutilizado!)
- Evitar configuração no constructor do DbContext (não será chamado a cada uso)
- Melhoria significativa de memória e throughput em alta carga

---

### §2.5 Value Converters

**O que é:** Conversão automática de valores entre .NET e banco de dados.
**Quando usar:** Enums armazenados como strings, tipos customizados, criptografia transparente.

```csharp
modelBuilder.Entity<Order>()
    .Property(o => o.Status)
    .HasConversion(
        v => v.ToString(),           // .NET → DB
        v => Enum.Parse<OrderStatus>(v) // DB → .NET
    );
```

**Regra:** Centraliza conversão — elimina lógica manual espalhada pelo código.

---

### §2.6 Temporal Tables

**O que é:** Tabelas com histórico automático de mudanças (SQL Server 2016+).
**Quando usar:** Auditoria, compliance, rollback de dados.

```csharp
// Configuration
modelBuilder.Entity<Order>()
    .ToTable("Orders", b => b.IsTemporal());

// Query de histórico
var history = context.Orders
    .TemporalAll()
    .Where(o => o.Id == orderId)
    .OrderBy(o => EF.Property<DateTime>(o, "PeriodStart"))
    .ToListAsync();

// Ponto no tempo
var snapshot = context.Orders
    .TemporalAsOf(specificDateTime)
    .ToListAsync();
```

**Versão mínima:** EF Core 6.0+
**Regra:** Substitui audit tables manuais; verificar se o banco suporta (SQL Server 2016+, PostgreSQL com extensão).

---

### §2.7 Database Seeding

**O que é:** População automática de dados iniciais.
**Quando usar:** Setup de ambiente, testes de integração, dados de referência.

```csharp
// EF Core 9+ — UseSeeding/UseAsyncSeeding
optionsBuilder.UseAsyncSeeding(async (context, _, ct) =>
{
    if (!await context.Set<Role>().AnyAsync(ct))
    {
        context.Set<Role>().AddRange(
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        );
        await context.SaveChangesAsync(ct);
    }
});
```

**Versão mínima:** EF Core 9.0 para `UseSeeding`/`UseAsyncSeeding`. Versões anteriores: `HasData()` no `OnModelCreating`.

---

### §2.8 Split Queries

**O que é:** Divide queries com Include() em múltiplas queries SQL separadas.
**Quando usar:** Relationships com muitos registros; evitar cartesian explosion de JOINs.

```csharp
// Per-query
var orders = context.Orders
    .Include(o => o.Items)
    .AsSplitQuery()
    .ToListAsync();

// Global
optionsBuilder.UseSqlServer(conn, o => o.UseQuerySplittingBehavior(
    QuerySplittingBehavior.SplitQuery));
```

**Trade-off:** Split queries fazem N round-trips ao banco vs 1 com JOIN. Melhor quando JOINs geram muitas linhas duplicadas.

---

### §2.9 Raw SQL

**O que é:** Execução de SQL direto via DbContext.
**Quando usar:** Features database-specific, queries complexas que EF não gera eficientemente.

```csharp
// Query tipada (EF Core 8+)
var orders = context.Database
    .SqlQuery<OrderSummary>($"SELECT Id, Total FROM Orders WHERE Total > {minValue}")
    .ToListAsync();

// Non-query
context.Database.ExecuteSqlInterpolated(
    $"UPDATE Orders SET Status = {newStatus} WHERE Id = {id}");
```

**Regras:**
- **Sempre** usar interpolação `$` (parameterizado) — nunca concatenar strings SQL
- Reservar para casos onde EF Core LINQ é insuficiente
- Documentar razão do uso de raw SQL no código

---

### §2.10 Multi-Database Migrations

**O que é:** Suporte a múltiplos providers de banco (SQL Server, PostgreSQL, SQLite) com migrations separadas.
**Quando usar:** Aplicação que roda em diferentes ambientes/bancos; testes com SQLite, produção com SQL Server.

```bash
# Migration para SQL Server
dotnet ef migrations add Init --context SqlServerContext --output-dir Migrations/SqlServer

# Migration para PostgreSQL
dotnet ef migrations add Init --context PostgresContext --output-dir Migrations/Postgres
```

**Regra:** Um DbContext por provider; migrations em diretórios separados.

---

## §3 API Versioning

### §3.1 Setup

```csharp
// Package: Asp.Versioning.Mvc (controllers) ou Asp.Versioning.Http (minimal)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Header com versões suportadas
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version")
    );
});
```

### §3.2 Controller-Based

```csharp
[ApiVersion(1.0)]
[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1.0)]
    public IActionResult GetV1() { ... }

    [HttpGet]
    [MapToApiVersion(2.0)]
    public IActionResult GetV2() { ... }
}
```

### §3.3 Minimal APIs

```csharp
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();

app.MapGet("/api/products", () => ...)
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(1, 0));
```

### §3.4 Estratégias de versão

| Estratégia | Mecanismo | Exemplo | Quando preferir |
|------------|-----------|---------|-----------------|
| **URL Segment** | Segmento na URL | `/api/v2/products` | APIs públicas — mais explícito |
| **Query String** | Parâmetro de query | `?api-version=2.0` | APIs internas — menos intrusivo |
| **Header** | HTTP header customizado | `X-Api-Version: 2.0` | APIs internas — URL limpa |
| **Media Type** | Accept header | `Accept: application/json;v=2` | API sofisticada — content negotiation |

**Regras:**
- Declarar versão explicitamente com `[ApiVersion]` — não depender de defaults
- URL segment para APIs públicas (mais discoverable)
- Ativar `ReportApiVersions` para informar clientes sobre versões disponíveis
- Marcar versões antigas como deprecated em vez de remover
