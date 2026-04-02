# Documento de Padrões e Diretrizes Técnicas — v3.0

## Guia Técnico de Referência para Desenvolvimento de Aplicação Desktop Cross-Platform

**Versão:** 3.0  
**Classificação:** Agnóstico ao domínio — define *como* desenvolver, não *o que* desenvolver.  
**Plataformas-alvo:** Linux e Windows.

---

## 1. Propósito e Escopo

Este documento é a **fonte da verdade técnica** do projeto. Qualquer desenvolvedor que o leia deve conseguir produzir implementações seguras, testadas e manuteníveis sem depender de conhecimento prévio não documentado aqui.

**Critérios de qualidade garantidos por este documento:**

1. **Manutenibilidade:** código modular, legível e modificável com confiança.
2. **Segurança em profundidade:** resistência a ataques em todas as camadas.
3. **Eficiência de recursos:** uso otimizado de memória, CPU, disco, rede e créditos de serviços terceiros.
4. **Validação automatizada:** testes, linters e analisadores estáticos que impeçam regressões.
5. **Observabilidade:** detecção, registro e alerta sobre estados anômalos.
6. **Confiabilidade:** comportamento previsível, incluindo degradação graciosa.
7. **Resiliência offline:** operação completa das funcionalidades locais sem internet.
8. **Recuperabilidade:** capacidade de restaurar o sistema após falhas.

**Documentos complementares (mantidos separadamente):**
- Regras de negócio e requisitos funcionais.
- ADRs (Architecture Decision Records) em `docs/adr/`.
- Modelo de ameaças (threat model) em `docs/threat-model/`.

---

## 2. Fundamentação Científica e Normativa

Toda decisão neste documento é rastreável a publicações científicas peer-reviewed, padrões normativos reconhecidos, ou documentações técnicas oficiais. Autores seminais (Martin, Fowler, Evans) são referenciados apenas quando acompanhados de estudos empíricos que validam os conceitos citados.

A lista completa de referências encontra-se no **Apêndice D**. As mais relevantes para cada seção são indicadas inline ao longo do documento.

---

## 3. Stack Tecnológica

### 3.1 Linguagem e Runtime

**Decisão:** C# sobre .NET 9+ (acompanhar LTS mais recente).

Linguagem fortemente tipada com nullable reference types, async/await, pattern matching e source generators. Runtime com GC generacional, suporte a NativeAOT e cross-platform.

**Configuração obrigatória (.csproj via `Directory.Build.props`):**

```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <AnalysisLevel>latest</AnalysisLevel>
  <AnalysisMode>All</AnalysisMode>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
</PropertyGroup>
```

- `Nullable enable` — rastreia nulabilidade em compile-time. Elimina NullReferenceException como classe de bug.
- `TreatWarningsAsErrors` — código não compila com warnings. Warnings ignorados se acumulam e escondem problemas.
- `AnalysisMode All` — ativa todas as regras dos Roslyn Analyzers embutidos.

### 3.2 Framework de Interface (UI)

**Decisão:** Avalonia UI com SukiUI. Padrão MVVM via CommunityToolkit.Mvvm.

**Justificativa (ADR-001):** Avalonia é o único framework .NET com suporte nativo real a Linux e Windows via Skia, renderização pixel-perfect consistente entre plataformas, licença MIT e ecossistema maduro (adotado por JetBrains para modernização de suas ferramentas WPF). MAUI não suporta Linux. Uno é mais complexo para cenário desktop-only.

**Gerenciamento de estado entre ViewModels:**
- Usar MediatR ou event bus in-process para comunicação entre ViewModels sem acoplamento direto.
- State containers para estado compartilhado (ex: usuário logado, status de conectividade).
- Nenhum ViewModel deve instanciar ou referenciar diretamente outro ViewModel.

**Cancelamento de operações longas na UI:**
```csharp
// ViewModel com operação cancelável:
public partial class ImportViewModel : ObservableObject
{
    private CancellationTokenSource? _cts;

    [RelayCommand]
    private async Task ImportAsync()
    {
        _cts = new CancellationTokenSource();
        try
        {
            IsImporting = true;
            await _importService.ExecuteAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Importação cancelada pelo usuário.";
        }
        finally
        {
            IsImporting = false;
            _cts.Dispose();
            _cts = null;
        }
    }

    [RelayCommand]
    private void CancelImport() => _cts?.Cancel();
}
```

### 3.3 Banco de Dados

**Decisão:** PostgreSQL, instalado junto com a aplicação via instalador (ADR-002).

**Justificativa:**
- SQL:2011+ — o SQL mais completo no open-source. CTEs recursivos, window functions, JSONB, arrays, tipos customizados, full-text search avançado, domains para validação de tipos.
- MVCC real — múltiplos leitores e escritores simultâneos sem bloqueio. Crítico dado que a aplicação terá background services fazendo I/O no banco simultaneamente à UI.
- Criptografia — TDE via extensão pg_tde (Percona, open-source, PostgreSQL License) para criptografia transparente em repouso; ou criptografia por coluna via pgcrypto (incluído na distribuição padrão). Para dados sensíveis específicos, criptografia na camada de aplicação com AES-256-GCM antes de persistir.
- EF Core — Npgsql.EntityFrameworkCore.PostgreSQL é o provider EF Core mais maduro depois do SQL Server. Suporte completo a Migrations, LINQ, scaffolding.
- Licença — PostgreSQL License (tipo BSD), sem copyleft, sem restrições de distribuição.
- Tipagem estrita — um INTEGER é um INTEGER. Não aceita texto onde se espera número (diferente do SQLite).

**Instalação bundled — o que o instalador deve fazer:**

*Windows:*
1. Incluir binários do PostgreSQL no pacote de instalação.
2. Executar `initdb` com encoding UTF-8, locale adequado e autenticação SCRAM-SHA-256.
3. Configurar `pg_hba.conf` para aceitar apenas conexões locais (127.0.0.1, ::1).
4. Registrar PostgreSQL como Windows Service (inicialização automática).
5. Criar database e usuário da aplicação com senha gerada aleatoriamente (armazenada no Secure Storage).
6. Configurar porta não-padrão (ex: 15432) para evitar conflito com PostgreSQL existente.

*Linux:*
1. Incluir PostgreSQL como dependência do pacote (.deb/.rpm) ou bundlear binários.
2. `initdb` + configurar `pg_hba.conf` (apenas localhost).
3. Registrar como serviço systemd (user-level, não system-level — não requer root após instalação).
4. Porta não-padrão, database e usuário criados automaticamente.

*Detecção de conflito:*
- Verificar se a porta escolhida está disponível antes de iniciar.
- Se PostgreSQL já existir no sistema, usar instância separada com diretório de dados próprio.

*Desinstalação limpa:*
- Parar o serviço.
- Remover o serviço.
- Oferecer ao usuário a opção de manter ou remover os dados.
- Remover binários e configurações.

**Configurações obrigatórias do PostgreSQL:**

```sql
-- postgresql.conf (ajustes para desktop, não servidor web):
shared_buffers = '128MB'          -- 128MB é adequado para desktop
work_mem = '16MB'                 -- memória por operação de sort/hash
maintenance_work_mem = '64MB'     -- para VACUUM, CREATE INDEX
effective_cache_size = '512MB'    -- estimativa para o query planner
max_connections = 10              -- desktop: poucos conexões simultâneas
listen_addresses = 'localhost'    -- APENAS conexões locais
port = 15432                      -- porta não-padrão para evitar conflito
password_encryption = 'scram-sha-256'  -- autenticação segura

-- Habilitar extensão de criptografia:
CREATE EXTENSION IF NOT EXISTS pgcrypto;
```

**Regras invioláveis de acesso ao banco:**

1. Queries parametrizadas exclusivamente (EF Core ou Dapper com parâmetros):
```csharp
// PROIBIDO:
var sql = $"SELECT * FROM Users WHERE Name = '{input}'";

// OBRIGATÓRIO:
var user = await context.Users.FirstOrDefaultAsync(u => u.Name == input);
```
2. Senhas nunca em texto plano. Hash com Argon2id (ver seção 4).
3. Toda alteração de schema via EF Core Migrations versionadas.
4. Credenciais do banco no Secure Storage do SO, nunca no código ou config files.
5. `PRAGMA secure_delete` equivalente: para dados sensíveis deletados, executar `VACUUM` para garantir que páginas livres sejam sobreescritas.

**Abstração obrigatória para troca futura de banco:**

O acesso a dados deve ser abstraído via Repository Pattern + EF Core de modo que trocar o banco no futuro exija mudanças apenas em `App.Infrastructure`:

```csharp
// A troca de provider é uma única linha:
// PostgreSQL:
options.UseNpgsql(connectionString);
// SQLite (se necessário para cenário simplificado):
options.UseSqlite(connectionString);
// Firebird:
options.UseFirebird(connectionString);
```

Nenhum código fora de `App.Infrastructure` deve ter dependência direta no provider do banco.

### 3.4 Distribuição e Atualização

**Decisão:** Velopack para empacotamento, instalação e atualização automática.

Framework cross-platform escrito em Rust. Suporta instaladores nativos, delta packages, code signing, rollback automático.

**Mecanismo de atualização:**
1. Verificação periódica (e sob demanda) de nova versão.
2. Download em background sem bloquear uso.
3. Notificação ao usuário.
4. Usuário decide quando aplicar.
5. Rollback automático se atualização falhar.
6. **Smoke test pós-atualização:** após aplicar, a aplicação executa verificações de sanidade (integridade do banco, conectividade com serviço PostgreSQL, presença de arquivos críticos). Se qualquer verificação falhar, rollback automático.

**Requisitos de segurança:** pacotes assinados digitalmente, HTTPS exclusivo, SHA-256 antes de instalar, rollback em caso de falha.

**Atualização do PostgreSQL:** quando uma nova versão do PostgreSQL é necessária (major upgrade), a aplicação deve: criar backup via `pg_dump`, instalar nova versão, executar `pg_upgrade` ou restaurar dump, verificar integridade, remover versão antiga.

### 3.5 Gerenciamento de Configuração

**Configurações não-sensíveis:** arquivo JSON no diretório de dados.

**Localização por plataforma:**
- Linux: `$XDG_DATA_HOME/<AppName>/` (tipicamente `~/.local/share/<AppName>/`)
- Windows: `%LOCALAPPDATA%\<AppName>\`

**Configurações sensíveis:** Secure Storage do SO (ver seção 4.3).

**Feature Flags:** mecanismo para ativar/desativar funcionalidades sem rebuild:
```csharp
public interface IFeatureFlags
{
    bool IsEnabled(string featureName);
}

// Implementação via tabela no banco ou arquivo JSON:
// { "features": { "offline_queue": true, "experimental_import": false } }

// Uso nos serviços:
if (_featureFlags.IsEnabled("experimental_import"))
{
    // funcionalidade experimental
}
```

Feature flags permitem deploy incremental, testes A/B, e desativação rápida de funcionalidades problemáticas sem rebuild ou re-deploy.

---

## 4. Segurança

A segurança é propriedade do sistema inteiro, não feature adicionada depois — princípio de *security by design* (McGraw, 2004, IEEE Security & Privacy; validado empiricamente por Khan et al., 2022, IEEE Access, em revisão sistemática de 95 estudos primários sobre integração de segurança no SDLC).

### 4.1 OWASP Desktop App Security Top 10

**DA1 — Injections:** queries parametrizadas obrigatórias. `Process.Start()` validado contra whitelist. XML com `DtdProcessing.Prohibit`. Desserialização JSON: usar `System.Text.Json` com `JsonSerializerOptions` restritivo (sem `TypeNameHandling`, sem polimorfismo não-controlado). Para importação de dados: validar tamanho do JSON (prevenir JSON bombs), profundidade máxima de nesting, e schema antes de processar.

**DA2 — Broken Authentication:**

Hash de senhas com Argon2id (parâmetros conforme OWASP, validados pelo NIST SP 800-63B que recomenda *memory-hard functions*):

```csharp
using Konscious.Security.Cryptography;

public static class PasswordHasher
{
    private const int SaltSize = 16;    // 128 bits
    private const int HashSize = 32;    // 256 bits
    private const int MemorySize = 65536; // 64 MB
    private const int Iterations = 3;
    private const int Parallelism = 4;

    public static (byte[] hash, byte[] salt) Hash(string password)
    {
        var salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);
        var hash = DeriveKey(password, salt);
        return (hash, salt);
    }

    public static bool Verify(string password, byte[] storedHash, byte[] salt)
    {
        var computedHash = DeriveKey(password, salt);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = Parallelism;
        argon2.MemorySize = MemorySize;
        argon2.Iterations = Iterations;
        return argon2.GetBytes(HashSize);
    }
}
```

**Nota sobre NIST SP 800-63B:** o NIST recomenda explicitamente *memory-hard functions* para hash de senhas. Argon2id é listado como opção preferencial. O NIST também menciona PBKDF2 com SHA-256 e mínimo 10.000 iterações (recomendação de 2024: 600.000). Este documento adota Argon2id por ser superior a PBKDF2 em resistência a ataques com hardware especializado (GPU/ASIC).

**Rate limiting de login:** máximo 5 tentativas em 15 minutos. Lockout progressivo: 1 min → 5 min → 15 min → 1 hora.

**DA3 — Sensitive Data Exposure:** ver seção 4.3.

**DA4 — Improper Cryptography:**

Algoritmos proibidos (BannedApiAnalyzers configurado para erro de compilação):
- `MD5`, `SHA1`, `HMACMD5`, `HMACSHA1`, `MD5.HashData()`, `SHA1.HashData()` — comprometidos.
- `DES`, `3DES`, `RC2`, `RC4` — chaves/blocos insuficientes.
- `System.Random` para fins criptográficos — previsível.
- `Rfc2898DeriveBytes` com defaults — SHA1 e 1000 iterações.

Algoritmos obrigatórios:
- Criptografia simétrica: `AesGcm` (AES-256-GCM, autenticado).
- Hash de senhas: Argon2id (Konscious.Security.Cryptography).
- Hash de integridade: `SHA256.HashData()`.
- Comparação de hashes: `CryptographicOperations.FixedTimeEquals()` (tempo constante, previne timing attacks).
- Números aleatórios: `RandomNumberGenerator.Fill()`.

**Key separation:** nunca derivar a mesma chave para dois propósitos diferentes. Chave de login e chave de criptografia de exportação devem usar salt e contexto distintos.

**DA5 — Improper Authorization:**

RBAC na camada de serviço (nunca apenas na UI):
```csharp
public enum Permission { Read, Write, Admin, ManageUsers }

public async Task<Result<Data>> GetSensitiveData(int userId)
{
    if (!await _authService.HasPermission(userId, Permission.Read))
    {
        _logger.LogWarning("Unauthorized access attempt. UserId={UserId} Permission={Permission}", userId, Permission.Read);
        return Result<Data>.Failure(Errors.Unauthorized);
    }
    // ...
}
```

**DA6 — Security Misconfiguration:**
- `#if DEBUG` para funcionalidades de desenvolvimento; garantir que nunca vazem para release via configuração de build.
- Permissões de filesystem: diretório de dados com permissões `700` (Linux) / ACL restritiva (Windows).
- PostgreSQL: `listen_addresses = 'localhost'`, autenticação SCRAM-SHA-256, `pg_hba.conf` somente local.
- Remover extensões PostgreSQL não utilizadas. Desabilitar `log_statement = 'all'` em produção (loga queries com dados).

**DA7 — Insecure Communication:** TLS 1.2+ para todo tráfego externo. Certificate pinning obrigatório para APIs críticas. Verificar OCSP/CRL para revogação de certificados.

**DA8 — Poor Code Quality:** Seções 5 e 6.

**DA9 — Components with Known Vulnerabilities:** Seção 11.

**DA10 — Insufficient Logging:** Seção 8.

### 4.2 Proteção contra Ataques Específicos

| Ataque | Mitigação |
|---|---|
| **DLL Hijacking** | Caminhos absolutos para libs. Windows: `SetDefaultDllDirectories(LOAD_LIBRARY_SEARCH_APPLICATION_DIR)`. Assinar DLLs. |
| **Memory Dumping** | `CryptographicOperations.ZeroMemory()` após uso de dados sensíveis. Não usar `string` para senhas (imutável, fica no heap). Usar `byte[]` e zerar. |
| **Timing Attacks** | `CryptographicOperations.FixedTimeEquals()` para toda comparação de hash/token. |
| **Privilege Escalation** | Mínimo de privilégios. Nunca solicitar admin/root desnecessariamente. |
| **Tampering do banco** | PostgreSQL com checksums habilitados (`initdb --data-checksums`). Detecta corrupção automaticamente. |
| **Replay Attacks (APIs)** | Nonces + timestamps em requests autenticados. Tokens de curta duração. Rejeitar requests com timestamp > 5 minutos de drift. |
| **Supply Chain (NuGet)** | Verificar nome exato do pacote e publisher antes de instalar. Usar `dotnet nuget verify` para validar assinaturas. Monitorar dependências com Dependabot ou similar. Nunca adicionar pacotes de publishers desconhecidos sem auditoria. |
| **Clipboard** | Limpar clipboard automaticamente após 30s se a aplicação colocou dados sensíveis. Registrar no log quando dados sensíveis são copiados. |

### 4.3 Gerenciamento de Segredos (Cross-Platform)

```csharp
public interface ISecureStorage
{
    Task StoreAsync(string key, byte[] value);
    Task<byte[]?> RetrieveAsync(string key);
    Task DeleteAsync(string key);
}
```

**Windows — DPAPI:**
```csharp
public class WindowsSecureStorage : ISecureStorage
{
    // Entropy adicional específica da aplicação (previne acesso por outros processos do mesmo usuário):
    private static readonly byte[] AppEntropy =
        Encoding.UTF8.GetBytes("MyApp.SecureStorage.v1");

    public Task StoreAsync(string key, byte[] value)
    {
        var encrypted = ProtectedData.Protect(value, AppEntropy, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(GetPath(key), encrypted);
        return Task.CompletedTask;
    }

    public Task<byte[]?> RetrieveAsync(string key)
    {
        var path = GetPath(key);
        if (!File.Exists(path)) return Task.FromResult<byte[]?>(null);
        var encrypted = File.ReadAllBytes(path);
        var decrypted = ProtectedData.Unprotect(encrypted, AppEntropy, DataProtectionScope.CurrentUser);
        return Task.FromResult<byte[]?>(decrypted);
    }

    public Task DeleteAsync(string key) { File.Delete(GetPath(key)); return Task.CompletedTask; }

    private static string GetPath(string key) =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MyApp", "secrets", $"{key}.enc");
}
```

**Linux — libsecret (GNOME Keyring / KWallet):**
```csharp
public class LinuxSecureStorage : ISecureStorage
{
    // Comunicação via D-Bus com libsecret usando Tmds.DBus ou Process.Start("secret-tool"):
    public async Task StoreAsync(string key, byte[] value)
    {
        var base64 = Convert.ToBase64String(value);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "secret-tool",
                ArgumentList = { "store", "--label", $"MyApp:{key}", "application", "myapp", "key", key },
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        await process.StandardInput.WriteAsync(base64);
        process.StandardInput.Close();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new SecureStorageException($"secret-tool store failed with exit code {process.ExitCode}");
    }

    public async Task<byte[]?> RetrieveAsync(string key)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "secret-tool",
                ArgumentList = { "lookup", "application", "myapp", "key", key },
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0 || string.IsNullOrEmpty(output))
            return null;

        return Convert.FromBase64String(output.Trim());
    }

    public async Task DeleteAsync(string key)
    {
        var process = Process.Start("secret-tool", new[] { "clear", "application", "myapp", "key", key });
        if (process != null) await process.WaitForExitAsync();
    }
}

// Fallback se secret-tool não estiver disponível:
// Derivar chave da senha do usuário via Argon2id (salt fixo da aplicação + salt do usuário)
// e criptografar com AES-256-GCM em arquivo local.
```

**Regra absoluta:** segredos nunca em texto plano — nem em config files, nem em variáveis de ambiente, nem no código.

### 4.4 Sequência de Inicialização Segura

1. Bootstrap Velopack (verificar atualizações pendentes).
2. Inicializar logging.
3. Verificar permissões do diretório de dados.
4. Verificar que o serviço PostgreSQL está rodando; iniciar se necessário.
5. Conectar ao banco e verificar integridade (`SELECT 1` + verificar versão de schema).
6. Verificar espaço em disco disponível.
7. Inicializar container de DI.
8. Carregar configurações e feature flags.
9. Apresentar tela de login.

Se qualquer etapa falhar: logar, apresentar mensagem ao usuário, impedir operação normal.

### 4.5 Ciclo de Vida de Chaves Criptográficas

- **Senha esquecida:** não há recuperação (by design). Documentar claramente para o usuário.
- **Troca de senha:** re-hash com Argon2id. O banco não precisa ser re-criptografado (a criptografia do banco é via PostgreSQL, não via senha do usuário).
- **Credenciais do banco:** senha do PostgreSQL gerada aleatoriamente na instalação (32 caracteres, `RandomNumberGenerator`), armazenada no Secure Storage.
- **Exportação criptografada:** senha mínima 12 caracteres com verificação de entropia. Derivação via Argon2id com MemorySize ≥128MB. Salt e contexto distintos da senha de login (key separation).

---

## 5. Arquitetura

### 5.1 Monolito Modular

Conforme validado pela SLR de Al-Qora'n & Al-Said Ahmad (2025, Future Internet, MDPI — 15 estudos primários seguindo guidelines de Kitchenham) e por Su & Li (2024, ACM SATrends): monolito modular combina simplicidade operacional com modularidade. Drivers de adoção identificados: deployment simplificado, manutenibilidade e redução de overhead.

```
Solution/
├── src/
│   ├── App.Desktop/              # Entry point, DI, bootstrap Velopack, inicialização segura
│   ├── App.Core/                 # Interfaces, DTOs, value objects, Result<T>, ISecureStorage, IFeatureFlags
│   ├── App.Infrastructure/       # DbContext, repos, HTTP clients, criptografia, secure storage, PostgreSQL config
│   ├── App.UI/                   # Views XAML, ViewModels, converters, recursos visuais
│   └── Modules/
│       ├── Module.FeatureA/      # Módulo de negócio (serviços, repos, modelos próprios)
│       └── Module.Shared/        # Funcionalidades compartilhadas (fila offline, notificações)
├── tests/
│   ├── Tests.Unit/
│   ├── Tests.Integration/        # Inclui testes de migration (up e down)
│   ├── Tests.E2E/
│   ├── Tests.Security/           # Fuzzing, input malicioso, penetration checklist
│   ├── Tests.Performance/        # Importação de grandes volumes, queries pesadas
│   └── Tests.Contract/           # Validação de contratos entre módulos
├── installer/                    # Scripts de instalação/desinstalação do PostgreSQL por plataforma
├── docs/
│   ├── adr/
│   └── threat-model/
├── Directory.Build.props
├── .editorconfig
├── BannedSymbols.txt
└── Solution.sln
```

### 5.2 Princípios SOLID

Princípios propostos por Martin (2000), validados empiricamente por:
- Singh & Hassan (2015, IJSER) — estudo empírico usando métricas CK demonstrando redução de coupling e melhoria de qualidade em designs que seguem SOLID.
- Cabral et al. (2024, CAIN/IEEE-ACM) — experimento controlado com 100 data scientists demonstrando evidência estatisticamente significativa de que SOLID melhora compreensão de código.
- Turan & Tanrıöver (2018, AJIT-e) — avaliação experimental mapeando SOLID para sub-características de manutenibilidade ISO/IEC 9126 via VS Code Metrics.
- Simonetti et al. (2024, Journal of Systems and Software, Elsevier) — estudo industrial na ASML aplicando SOLID na refatoração de código legado com redução mensurável de dívida técnica.

**S — Single Responsibility:** cada classe tem uma única razão para mudar.
**O — Open/Closed:** extensão via composição e Strategy, não modificação.
**L — Liskov Substitution:** subtipos substituíveis sem quebrar comportamento.
**I — Interface Segregation:** interfaces pequenas e coesas.
**D — Dependency Inversion:** módulos dependem de abstrações, não de implementações.

```csharp
// DI — toda dependência externa injetada:
public class SyncService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IUserRepository _userRepo;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<SyncService> _logger;

    public SyncService(IHttpClientFactory h, IUserRepository r, TimeProvider t, ILogger<SyncService> l)
    {
        _httpFactory = h; _userRepo = r; _timeProvider = t; _logger = l;
    }
}
```

### 5.3 Result Pattern

```csharp
public sealed record Result<T>
{
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess => Error is null;
    private Result(T value) => Value = value;
    private Result(Error error) => Error = error;
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}
public sealed record Error(string Code, string Message);
```

### 5.4 Internacionalização (i18n)

- Nenhuma string visível ao usuário hardcoded. Todas em .resx.
- Datas no banco em UTC; converter para local na apresentação.
- `CultureInfo.CurrentCulture` para formatação.

### 5.5 Migração de Dados entre Versões

1. Antes de aplicar migrations: backup via `pg_dump`.
2. Aplicar migrations em transação (PostgreSQL suporta DDL transacional, diferente do SQLite).
3. Se falhar: rollback da transação (o banco permanece intacto).
4. Testar migrations up e down na suite de integração.
5. EF Core mantém histórico em `__EFMigrationsHistory`.

### 5.6 Concorrência UI Thread vs Background Services

PostgreSQL com MVCC resolve a contença de escritas simultâneas nativamente. Entretanto, regras de código:
- Nunca fazer queries na UI thread. Sempre via `Task.Run` ou async.
- Usar connection pooling do Npgsql (configurado automaticamente pelo EF Core).
- `CancellationToken` em toda cadeia de operações canceláveis.
- Para operações longas (importação): exibir progresso na UI via `IProgress<T>`.

---

## 6. Qualidade de Código

### 6.1 SAST (Análise Estática)

**Pacotes obrigatórios (NuGet em `Directory.Build.props`):**
- Roslyn Analyzers (built-in) — qualidade, design, performance.
- SonarAnalyzer.CSharp — SQL injection, XSS, crypto fraca, hardcoded secrets.
- Roslynator.Analyzers — 190+ analyzers de estilo.
- SecurityCodeScan.VS2019 — taint analysis (fluxo de dados inseguros).
- Microsoft.CodeAnalysis.BannedApiAnalyzers — proibir APIs via `BannedSymbols.txt`.

**`BannedSymbols.txt` completo:**
```
M:System.Security.Cryptography.MD5.Create();Use SHA256
M:System.Security.Cryptography.MD5.HashData(System.ReadOnlySpan{System.Byte});Use SHA256
M:System.Security.Cryptography.SHA1.Create();Use SHA256
M:System.Security.Cryptography.SHA1.HashData(System.ReadOnlySpan{System.Byte});Use SHA256
T:System.Security.Cryptography.HMACMD5;Use HMACSHA256
T:System.Security.Cryptography.HMACSHA1;Use HMACSHA256
T:System.Security.Cryptography.DESCryptoServiceProvider;Use AesGcm
T:System.Security.Cryptography.TripleDESCryptoServiceProvider;Use AesGcm
T:System.Security.Cryptography.RC2CryptoServiceProvider;Use AesGcm
M:System.Threading.Thread.Sleep(System.Int32);Use Task.Delay
```

### 6.2 Convenções

Indentação 4 espaços. UTF-8. Modificadores explícitos. Namespaces file-scoped. Classes PascalCase, campos privados `_camelCase`, interfaces `I` + PascalCase, métodos async com sufixo `Async`.

### 6.3 Práticas

- Métodos curtos (~20-30 linhas como guia, não dogma).
- Guard clauses. XML doc em APIs públicas. Zero magic numbers/strings.
- Sem `#region`. Sem catch que engole exceção. Todo `IDisposable` com `using`.

---

## 7. Estratégia de Testes

Pirâmide de testes conforme validada empiricamente por Contan et al. (2018, IEEE AQTR) e Wang et al. (2022, Journal of Systems and Software — correlação p=0.000624 entre maturidade de automação e qualidade).

### 7.1 Proporção

~60% unitários, ~25% integração, ~15% E2E + Security + Performance. Proporção ajustada para desktop monolítico (mais integração que web/microservices típico — validado pela observação de Contan et al. de que a pirâmide clássica nem sempre se aplica literalmente).

### 7.2 Unitários

**Ferramentas:** xUnit + FluentAssertions + NSubstitute.

Padrão AAA. Nome: `Method_Scenario_Expected`. Determinísticos (injetar `TimeProvider`). Zero I/O externo.

**O que testar por camada:**
- Validators: cada regra violada individualmente, boundary values, strings vazias/nulas/extremas.
- Services: happy path, cada cenário de falha, interação com dependências mockadas.
- ViewModels: propriedades bindadas, commands enable/disable, INotifyPropertyChanged.
- Criptografia: encrypt→decrypt=original, senhas diferentes→hashes diferentes, salt único por operação.

### 7.3 Integração

**Banco para testes:** PostgreSQL de teste (container Docker ou instância separada com banco temporário).

```csharp
public class UserRepositoryTests : IAsyncLifetime
{
    private AppDbContext _context;
    private NpgsqlConnection _connection;

    public async Task InitializeAsync()
    {
        // Criar banco de teste temporário:
        _connection = new NpgsqlConnection("Host=localhost;Port=15432;Database=postgres;Username=test;Password=test");
        await _connection.OpenAsync();
        var dbName = $"test_{Guid.NewGuid():N}";
        await new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", _connection).ExecuteNonQueryAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql($"Host=localhost;Port=15432;Database={dbName};Username=test;Password=test")
            .Options;
        _context = new AppDbContext(options);
        await _context.Database.MigrateAsync(); // Aplicar todas as migrations
    }

    [Fact]
    public async Task Insert_DuplicateEmail_ThrowsConstraintViolation()
    {
        var repo = new UserRepository(_context);
        await repo.AddAsync(new User { Name = "A", Email = "dup@test.com" });
        await _context.SaveChangesAsync();

        var act = () => { repo.AddAsync(new User { Name = "B", Email = "dup@test.com" }); return _context.SaveChangesAsync(); };
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        // Dropar banco de teste:
        await new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_context.Database.GetDbConnection().Database}\"", _connection).ExecuteNonQueryAsync();
        await _connection.DisposeAsync();
    }
}
```

**Testes de migration:** verificar que cada migration aplica (up) e reverte (down) sem erro.

**Contract tests entre módulos:** se Module.A consome `IServiceB` de Module.B, criar testes que validam que a implementação real de Module.B respeita o contrato (além dos testes unitários com mock).

### 7.4 End-to-End

**Framework:** Avalonia.Headless para testes de UI sem exibir janela.

```csharp
[Fact]
public async Task LoginFlow_ValidCredentials_NavigatesToHome()
{
    using var app = new TestApp(); // Inicializa Avalonia em modo headless
    var loginView = app.GetView<LoginView>();

    loginView.FindControl<TextBox>("UsernameInput").Text = "testuser";
    loginView.FindControl<TextBox>("PasswordInput").Text = "ValidPassword1!";
    loginView.FindControl<Button>("LoginButton").Command.Execute(null);

    await Task.Delay(500); // Aguardar navegação
    app.CurrentView.Should().BeOfType<HomeView>();
}
```

Cobrir apenas fluxos críticos. Se um teste E2E é instável (flaky), corrigir ou remover.

### 7.5 Testes de Segurança

Input malicioso parametrizado em todo ponto de entrada:
```csharp
[Theory]
[InlineData("' OR 1=1 --")]
[InlineData("'; DROP TABLE users;--")]
[InlineData("../../etc/passwd")]
[InlineData("\0")]
[InlineData("<script>alert(1)</script>")]
[MemberData(nameof(LongStrings))] // 10.000+ caracteres
public async Task ProcessInput_MaliciousPayload_HandledSafely(string payload)
{
    var result = await _service.ProcessAsync(payload);
    // Deve rejeitar ou processar sem side effects maliciosos
}
```

Mutation testing com **Stryker.NET** periodicamente.

### 7.6 Performance

Medir tempo e memória com datasets de 1k, 10k, 100k registros. Definir thresholds. Falhar se exceder.

### 7.7 Cobertura

**Coverlet.** Meta: 80% global (convenção da indústria — não há threshold cientificamente validado, mas Wang et al. 2022 demonstra correlação positiva entre cobertura e qualidade). 100% de branch em código de segurança.

---

## 8. Observabilidade

### 8.1 Logging Estruturado

**Serilog** sobre `Microsoft.Extensions.Logging`.

```csharp
// Configuração no Program.cs:
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MyApp")
    .WriteTo.File(
        path: Path.Combine(dataDir, "logs", "app-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,       // Manter 30 dias
        fileSizeLimitBytes: 50_000_000,   // 50MB por arquivo
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```

**Regras:** nunca logar senhas, tokens, dados pessoais. Rotação por dia, retenção 30 dias, 50MB max por arquivo.

### 8.2 Health Checks

Na inicialização e periodicamente: conectividade com PostgreSQL, espaço em disco, permissões de arquivo.

### 8.3 Métricas

Via `System.Diagnostics.Metrics`: tempo de inicialização, latência de operações, uso de memória, contagem de erros, retries de rede.

---

## 9. Performance

**Medir antes de otimizar** (Knuth, 1974, ACM Computing Surveys — "premature optimization is the root of all evil").

- Memória: `Span<T>`, `ArrayPool<T>.Shared`, streaming para arquivos grandes.
- CPU: operações pesadas em background, `async/await` para I/O, `CancellationToken`.
- PostgreSQL: connection pooling (Npgsql automático), índices em colunas consultadas, `EXPLAIN ANALYZE` para queries lentas.
- Rede: Polly para retry com exponential backoff + jitter, circuit breaker, cache, timeout.

**Configuração de Polly (circuit breaker + retry):**
```csharp
services.AddHttpClient("ExternalApi")
    .AddResilienceHandler("pipeline", builder =>
    {
        builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true
        });
        builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30),
            MinimumThroughput = 5,
            BreakDuration = TimeSpan.FromSeconds(30)
        });
        builder.AddTimeout(TimeSpan.FromSeconds(10));
    });
```

---

## 10. Offline-First

- Detecção de conectividade: verificar acesso real ao endpoint (não apenas "está na rede").
- Fila de operações pendentes: tabela PostgreSQL com status, attempts, next_retry_at.
- Processamento: FIFO, retry com backoff, desistir após max_attempts.
- Indicação visual: online/offline + operações pendentes.
- Conflitos: definir por funcionalidade (last-write-wins, merge, prompt ao usuário).

**Dados sensíveis na fila:** payloads que contêm tokens ou dados sensíveis devem ser criptografados na coluna antes de persistir, independentemente da criptografia do banco.

---

## 11. Gerenciamento de Dependências

**Critérios de adoção:** necessidade real, licença compatível (MIT/Apache/BSD, evitar GPL), manutenção ativa, zero vulnerabilidades conhecidas, mínimo de dependências transitivas.

**Manutenção:** `dotnet list package --vulnerable` em toda build. `dotnet-outdated` mensal. Patch versions imediatamente. `DEPENDENCIES.md` com inventário.

**Supply chain:** verificar nome exato e publisher. `dotnet nuget verify` para assinaturas. Monitorar com Dependabot.

---

## 12. Tratamento de Erros

Exceções para situações excepcionais. Result Pattern para falhas de negócio. Handler global (`AppDomain.UnhandledException` + Avalonia) que loga com stack trace e exibe mensagem genérica.

---

## 13. Importação / Exportação / Backup

- Formato self-contained com versão de schema.
- Exportação: `pg_dump` para backup do banco + criptografia AES-256-GCM com chave derivada de senha do usuário (Argon2id, MemorySize ≥128MB, salt e contexto distintos da senha de login).
- Senha de exportação: mínimo 12 caracteres com verificação de entropia.
- Importação: validar integridade (SHA-256), versão de schema, decriptação. Processar em transação. Rollback se falhar.
- Backups automáticos: via `pg_dump` antes de cada migration e periodicamente (configurável).

---

## 14. Controle de Versão e CI

### 14.1 Git

Conventional Commits. `.gitignore` adequado. Pre-commit hook: `dotnet format --verify-no-changes`.

### 14.2 Pipeline

1. Restore → 2. Build (analyzers = erros) → 3. Vulnerability scan → 4. Unitários → 5. Integração → 6. Cobertura (≥80%) → 7. E2E (branches principais) → 8. Security (nightly) → 9. Package (Velopack + assinatura).

**Etapas 1-6 são gate: falha = bloqueia merge.**

---

## 15. Recuperação de Desastres

| Cenário | Procedimento |
|---|---|
| Banco corrompido | PostgreSQL detecta via checksums. Alertar usuário. Restaurar último `pg_dump` automático. |
| Instalador falha | Velopack rollback automático. |
| PostgreSQL não inicia | Verificar logs, tentar restart automático. Se falhar, modo somente-leitura com dados em cache. |
| SO reinstalado | Importar backup exportado previamente. |
| Espaço em disco esgotado | Detectar antes de operações de escrita. Modo read-only. |

---

## 16. Diferenças entre Plataformas

| Aspecto | Linux | Windows |
|---|---|---|
| Diretório de dados | `$XDG_DATA_HOME/<App>/` | `%LOCALAPPDATA%\<App>\` |
| Secure Storage | secret-tool (GNOME Keyring/KWallet) | DPAPI + entropy |
| Permissões de arquivo | `chmod 700` | ACLs via System.Security.AccessControl |
| PostgreSQL service | systemd (user-level) | Windows Service |
| Code signing | GPG ou sigstore | Authenticode |
| Instalador | .deb/.rpm + scripts de setup do PostgreSQL | Setup.exe via Velopack + scripts de setup do PostgreSQL |

---

## 17. Acessibilidade

Navegação por teclado. Contraste WCAG AA (4.5:1). Tamanho mínimo 44x44dp. Informação não apenas por cor. `AutomationProperties` no Avalonia.

---

## 18. Documentação

XML doc em APIs públicas. README, CONTRIBUTING, CHANGELOG, DEPENDENCIES. ADRs em `docs/adr/`.

---

## 19. Checklist de Revisão

**Funcionalidade:** implementa requisito; cobre cenários de borda; funciona offline.
**Segurança:** inputs validados; nenhum dado sensível em logs/mensagens/código; queries parametrizadas; permissões na camada de serviço; comparações em tempo constante.
**Qualidade:** compila sem warnings; analyzers satisfeitos; nomes claros; DI.
**Testes:** unitários para cenários normais e de borda; integração; inputs maliciosos; cobertura mantida.
**Performance:** operações pesadas em background; recursos liberados.
**Observabilidade:** operações logadas; erros com contexto; nenhum dado sensível em logs.

---

## Apêndice A: Pacotes NuGet

| Categoria | Pacote |
|---|---|
| UI | Avalonia UI, SukiUI, CommunityToolkit.Mvvm |
| ORM | Npgsql.EntityFrameworkCore.PostgreSQL |
| Logging | Serilog, Serilog.Extensions.Logging, Serilog.Sinks.File |
| DI | Microsoft.Extensions.DependencyInjection |
| Resiliência | Polly, Microsoft.Extensions.Http.Resilience |
| Testes | xUnit, FluentAssertions, NSubstitute, Coverlet |
| Mutation | Stryker.NET |
| Analyzers | SonarAnalyzer.CSharp, Roslynator, SecurityCodeScan, BannedApiAnalyzers |
| Updates | Velopack |
| Serialization | System.Text.Json |
| Password Hashing | Konscious.Security.Cryptography |
| Mediator | MediatR |

## Apêndice B: APIs Proibidas

Ver `BannedSymbols.txt` na seção 6.1. Adicionalmente: `dynamic` (exceto interop), `GC.Collect()`, `#pragma warning disable` sem justificativa, `catch (Exception) { }`.

## Apêndice C: Threat Model STRIDE

| Categoria | Ameaça | Mitigação |
|---|---|---|
| Spoofing | Atacante finge ser outro usuário | Argon2id + rate limiting (4.1 DA2) |
| Tampering | Modificação do banco ou binários | PostgreSQL checksums + code signing (4.2) |
| Repudiation | Negação de ação | Audit trail via logging (8.1) |
| Info Disclosure | Vazamento de dados | Criptografia em repouso + secure storage (3.3, 4.3) |
| Denial of Service | Esgotamento de recursos | Validação de input + limites (4.1 DA1) |
| Elevation of Privilege | Acesso indevido | RBAC na camada de serviço (4.1 DA5) |

## Apêndice D: Referências

### Periódicos Científicos Peer-Reviewed

1. Wang, Y., Mäntylä, M., Liu, Z. & Markkula, J. (2022). "Test automation maturity improves product quality." *Journal of Systems and Software*, vol. 188, Elsevier. DOI: 10.1016/j.jss.2022.111262

2. Khan, R. A. et al. (2022). "Systematic Literature Review on Security Risks and its Practices in Secure Software Development." *IEEE Access*, vol. 10. DOI: 10.1109/ACCESS.2022.3140181

3. Amaro, R., Pereira, R. & da Silva, M. M. (2023). "Capabilities and practices in DevOps." *IEEE Transactions on Software Engineering*, vol. 49, pp. 883-901. DOI: 10.1109/TSE.2022.3170654

4. Pereira, R. et al. (2024). "DevSecOps practices and tools." *International Journal of Information Security*, Springer. DOI: 10.1007/s10207-024-00914-z

5. Simonetti, L. et al. (2024). "Applying SOLID principles for the refactoring of legacy code: An experience report." *Journal of Systems and Software*, Elsevier. DOI: 10.1016/j.jss.2024.112233

6. Al-Qora'n, L. & Al-Said Ahmad, A. (2025). "Modular Monolith Architecture in Cloud Environments: A SLR." *Future Internet*, vol. 17, no. 11, p. 496. MDPI. DOI: 10.3390/fi17110496

### Conferências Peer-Reviewed

7. Contan, C., Dehelean, C. & Miclea, L. (2018). "Test Automation Pyramid from Theory to Practice." *IEEE AQTR*. DOI: 10.1109/AQTR.2018.8402699

8. Su, R. & Li, X. (2024). "Modular Monolith: Is This the Trend in Software Architecture?" *ACM SATrends '24*. DOI: 10.1145/3643657.3643911

9. Cabral, R. et al. (2024). "Investigating the Impact of SOLID Design Principles on ML Code Understanding." *IEEE/ACM CAIN 2024*. — Experimento controlado com 100 participantes; evidência estatisticamente significativa de melhoria na compreensão de código.

10. Singh, H. & Hassan, S. I. (2015). "Effect of SOLID Design Principles on Quality of Software: An Empirical Assessment." *IJSER*, vol. 6, no. 4. — Avaliação empírica com métricas CK demonstrando redução de coupling.

11. Turan, O. & Tanrıöver, Ö. (2018). "An Experimental Evaluation of the Effect of SOLID Principles to Microsoft VS Code Metrics." *AJIT-e*. — Mapeamento de SOLID para ISO/IEC 9126.

### Autores Seminais (validados por estudos acima)

12. McGraw, G. (2004). "Software Security." *IEEE Security & Privacy*, vol. 2, no. 2. — Princípio de security by design. Validado por Khan et al. (2022, ref. 2).

13. Martin, R. C. (2000). "Design Principles and Design Patterns." — Princípios SOLID. Validados por Singh & Hassan (2015, ref. 10), Cabral et al. (2024, ref. 9), Simonetti et al. (2024, ref. 5).

14. Martin, R. C. (2017). *Clean Architecture*. Prentice Hall. — Separação de camadas e DI. Validados por Simonetti et al. (2024, ref. 5).

15. Evans, E. (2003). *Domain-Driven Design*. Addison-Wesley. — Bounded contexts e módulos. Validados por Al-Qora'n & Al-Said Ahmad (2025, ref. 6) que identifica DDD como prática de implementação de monolitos modulares.

16. Fowler, M. (2012). "The Practical Test Pyramid." martinfowler.com. — Pirâmide de testes. Validada por Contan et al. (2018, ref. 7) e Wang et al. (2022, ref. 1).

17. Bass, L., Clements, P. & Kazman, R. (2012). *Software Architecture in Practice*, 3rd ed. Pearson. — Quality attributes e trade-offs arquiteturais.

18. Humble, J. & Farley, D. (2010). *Continuous Delivery*. Addison-Wesley. — CI/CD e automação de testes.

### Padrões Normativos

19. OWASP Desktop App Security Top 10 (2021).
20. NIST SP 800-63B — Digital Identity Guidelines: Authentication.
21. NIST SP 800-132 — Password-Based Key Derivation.
22. IEEE Std 2675-2021 — Standard for DevOps.
23. ISO/IEC 25010:2011 — Software Quality Model.

### Documentações Técnicas

24. PostgreSQL Documentation — https://www.postgresql.org/docs/
25. Npgsql/EF Core Provider — https://www.npgsql.org/efcore/
26. Percona pg_tde — https://docs.percona.com/pg-tde/
27. Velopack — https://docs.velopack.io/
28. Avalonia UI — https://docs.avaloniaui.net/

---

*Documento vivo. Atualizar a cada decisão técnica, ameaça identificada ou revisão de padrão.*
