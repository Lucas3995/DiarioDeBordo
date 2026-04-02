# Documento de Padrões e Diretrizes Técnicas — v2.0

## Guia Técnico de Referência para Desenvolvimento de Aplicação Desktop Cross-Platform

**Versão:** 2.0  
**Classificação:** Documento agnóstico ao domínio — define *como* desenvolver, não *o que* desenvolver.  
**Plataformas-alvo:** Linux e Windows.  
**Público-alvo:** Qualquer desenvolvedor que precise implementar funcionalidades de forma segura, testada e manutenível. Este documento deve ser suficiente para que alguém sem experiência prévia no projeto produza implementações alinhadas com padrões de excelência em engenharia de software.

---

## 1. Propósito e Escopo

Este documento é a **fonte da verdade técnica** do projeto. Ele define tecnologias, padrões, técnicas, ferramentas, convenções e práticas obrigatórias. Documentos de regras de negócio são mantidos separadamente e definem *o que* o sistema faz. Este documento define *como* fazê-lo.

**Critérios de qualidade que este documento visa garantir:**

1. **Manutenibilidade:** código modular, legível e modificável com confiança.
2. **Segurança em profundidade:** resistência a ataques em todas as camadas — código, dados, binários, filesystem, comunicação de rede e processo de atualização.
3. **Eficiência de recursos:** uso otimizado de memória, CPU, disco, rede e créditos de serviços terceiros.
4. **Validação automatizada:** testes, linters e analisadores estáticos que impeçam regressões.
5. **Observabilidade:** detecção, registro e alerta sobre estados anômalos.
6. **Confiabilidade:** comportamento previsível, incluindo degradação graciosa em cenários inesperados.
7. **Resiliência offline:** operação completa das funcionalidades locais sem internet.
8. **Recuperabilidade:** capacidade de restaurar o sistema a um estado funcional após falhas.

---

## 2. Fundamentação Científica e Normativa

As decisões deste documento são fundamentadas em publicações científicas peer-reviewed, padrões normativos reconhecidos e documentações técnicas oficiais. A fundamentação completa encontra-se no **Apêndice D**, mas os pilares principais são:

**Segurança de software:**
- Alhazmi, O. H. & Malaiya, Y. K. (2005). "Quantitative vulnerability assessment of systems software." *IEEE Annual Reliability and Maintainability Symposium*. — Estudo empírico sobre densidade de vulnerabilidades em software, fundamentando a necessidade de SAST integrado ao build.
- Khan, R. A. et al. (2022). "Systematic Literature Review on Security Risks and its Practices in Secure Software Development." *IEEE Access*, vol. 10. — Revisão sistemática de 95 estudos primários sobre integração de segurança no SDLC, validando práticas como threat modeling, SAST/DAST e revisão de código seguro.
- Cruzes, D. S. et al. (2017). "How is Security Testing Done in Agile Teams?" *International Conference on Agile Software Development (XP 2017), Springer.* — Estudo empírico sobre práticas de teste de segurança em equipes ágeis, demonstrando que SAST automatizado combinado com revisão manual produz os melhores resultados.
- McGraw, G. (2004). "Software Security." *IEEE Security & Privacy*, vol. 2, no. 2, pp. 80-83. — Artigo seminal que estabelece que segurança deve ser uma propriedade projetada no software (security by design), não adicionada depois.

**Arquitetura de software:**
- Al-Qora'n, L. & Al-Said Ahmad, A. (2025). "Modular Monolith Architecture in Cloud Environments: A Systematic Literature Review." *Future Internet*, vol. 17, no. 11, p. 496. MDPI. — SLR com 15 estudos primários seguindo guidelines de Kitchenham, identificando que arquitetura monolítica modular combina simplicidade operacional com modularidade, tendo como drivers de adoção: deployment simplificado, manutenibilidade e redução de overhead de orquestração.
- Su, R. & Li, X. (2024). "Modular Monolith: Is This the Trend in Software Architecture?" *Proceedings of the 1st International Workshop on New Trends in Software Architecture (SATrends '24)*, ACM. — Revisão sistemática demonstrando que monolito modular é uma alternativa viável a microserviços, especialmente como etapa anterior a uma eventual migração.
- Bass, L., Clements, P. & Kazman, R. (2012). *Software Architecture in Practice*, 3rd ed. Pearson. — Referência canônica em arquitetura de software, fundamentando princípios de quality attributes e trade-offs arquiteturais.

**Estratégia de testes:**
- Contan, C., Dehelean, C. & Miclea, L. (2018). "Test Automation Pyramid from Theory to Practice." *IEEE International Conference on Automation, Quality and Testing, Robotics (AQTR)*. — Estudo empírico em 5 projetos ágeis analisando a distribuição real de testes unitários, de integração e E2E, validando (com nuances) o modelo piramidal.
- Wang, Y. et al. (2022). "Test automation maturity improves product quality — Quantitative study of open source projects using continuous integration." *Journal of Systems and Software*, vol. 188, Elsevier. — Estudo empírico com 37 projetos open-source demonstrando correlação estatisticamente significativa (p=0.000624) entre maturidade de automação de testes e qualidade do produto.

**DevSecOps e SAST:**
- Amaro, R. et al. (2023). "Capabilities and practices in DevOps: a multivocal literature review." *IEEE Transactions on Software Engineering*, vol. 49, pp. 883-901. — MLR abrangente sobre práticas DevOps, incluindo integração de segurança no pipeline.
- Pereira, R. et al. (2024). "DevSecOps practices and tools." *International Journal of Information Security*, Springer. — MLR identificando práticas e ferramentas de DevSecOps, incluindo SAST, SCA e gestão de vulnerabilidades de dependências.

**Padrões normativos referenciados:**
- OWASP Desktop App Security Top 10 (2021) — classificação de vulnerabilidades para aplicações desktop.
- OWASP Desktop Application Security Verification Standard (DASVS) — controles de segurança para thick clients.
- NIST SP 800-132 — recomendações para derivação de chaves baseada em senhas.
- NIST SP 800-63B — diretrizes de autenticação digital (parâmetros de hashing).
- IEEE Std 2675-2021 — Standard for DevOps.
- ISO/IEC 25010:2011 — modelo de qualidade de software (substitui ISO 9126).

---

## 3. Stack Tecnológica

### 3.1 Linguagem e Runtime

**Decisão:** C# sobre .NET 9+ (acompanhar LTS mais recente).

**Fundamentação:** linguagem fortemente tipada com suporte nativo a nullable reference types (eliminação estrutural de NullReferenceException), async/await, pattern matching, source generators. O runtime .NET possui garbage collector generacional, suporte a AOT compilation (NativeAOT) e é cross-platform (Windows/Linux/macOS).

**Configuração obrigatória do projeto (.csproj):**

```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <AnalysisLevel>latest</AnalysisLevel>
  <AnalysisMode>All</AnalysisMode>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
</PropertyGroup>
```

**O que cada configuração faz e por que é obrigatória:**
- `Nullable enable` — o compilador rastreia nulabilidade de todas as referências. Variáveis não-nulas não podem receber null sem warning (que é tratado como erro). Elimina a classe inteira de NullReferenceException.
- `TreatWarningsAsErrors` — nenhum código com warnings compila. Warnings ignorados se acumulam e escondem problemas reais.
- `AnalysisLevel latest` + `AnalysisMode All` — ativa todas as regras dos Roslyn Analyzers embutidos no SDK, incluindo regras de qualidade (CAxxxx) e estilo (IDExxxx).

### 3.2 Framework de Interface (UI)

**Decisão:** Avalonia UI com biblioteca de componentes SukiUI.

**Análise comparativa (ADR-001):**

| Critério | Avalonia UI | .NET MAUI | Uno Platform |
|---|---|---|---|
| Suporte Linux | Nativo (Skia) | Sem suporte oficial | Via Skia (recente) |
| Suporte Windows | Nativo | Nativo | Nativo (WinUI) |
| Licença | MIT | MIT | Apache 2.0 |
| Renderização | Pixel-perfect via Skia (consistente entre OSes) | Controles nativos do SO (aparência varia) | Híbrido (Skia + nativos) |
| Maturidade para desktop | Alta (sucessor espiritual do WPF, adotado por JetBrains) | Foco primário em mobile | Foco em web (WASM) + mobile |
| Complexidade | Baixa para desktop-only | Média | Alta (multi-target) |
| Ecossistema .NET puro | XAML/C# | XAML/C# | XAML/C#/JS/TS |

**Conclusão:** Avalonia é o framework com melhor adequação ao cenário (desktop-only, Linux obrigatório, renderização consistente entre OSes). MAUI é eliminado por não suportar Linux. Uno é mais complexo para um cenário que não necessita de web ou mobile.

**Padrão de UI:** MVVM (Model-View-ViewModel) via CommunityToolkit.Mvvm. Separa lógica de apresentação da lógica de negócio. Permite testar ViewModels sem instanciar UI.

### 3.3 Banco de Dados

**Análise comparativa (ADR-002):**

Foram avaliados todos os bancos de dados open-source embeddable com suporte a .NET e cross-platform:

| Critério | SQLite | LiteDB | Firebird Embedded | DuckDB |
|---|---|---|---|---|
| **Tipo** | Relacional (SQL) | Documento (NoSQL/BSON) | Relacional (SQL) | Analítico (OLAP) |
| **Linguagem nativa** | C (interop via P/Invoke) | 100% C# (managed) | C++ (interop) | C++ (interop) |
| **ACID** | Sim | Sim | Sim | Sim |
| **Arquivo único** | Sim | Sim | Sim | Sim |
| **Suporte EF Core** | Sim (Microsoft.Data.Sqlite) | Não (API própria) | Sim (FirebirdSql.Data.FirebirdClient) | Limitado |
| **Criptografia em repouso** | Via SQLite3 Multiple Ciphers (AES-256-CBC, gratuito, open-source) ou SQLCipher (AES-256-CBC, community edition gratuita) | Built-in (AES via Rfc2898DeriveBytes), mas com fragilidades documentadas: modo ECB em versões antigas, iterações PBKDF2 baixas por default, sem criptografia autenticada | Via plugin comercial (HQbird AES-256) ou implementação própria | Sem suporte nativo |
| **Migrations/Versionamento de schema** | Sim (EF Core Migrations) | Não nativo (lib externa LiteDB.Migration) | Sim (EF Core / manual) | N/A |
| **SQL compliance** | SQL-92 parcial | SQL-like (limitado) | SQL-99+ (stored procedures, triggers, views) | SQL-92+ (analítico) |
| **Concorrência** | WAL: múltiplos leitores + 1 escritor | Múltiplos leitores, 1 escritor por collection | MVCC (múltiplos leitores e escritores) | Múltiplos leitores, 1 escritor |
| **Tamanho do ecossistema** | Enorme (usado por Firefox, Chrome, Android, iOS) | Médio (9.3k stars GitHub, comunidade .NET) | Médio (40+ anos de histórico, comunidade menor) | Crescente (foco em analytics) |
| **Maturidade** | 25+ anos | ~10 anos | 40+ anos | ~6 anos |
| **Ferramentas de inspeção** | Dezenas (DB Browser, DBeaver, etc.) | LiteDB Studio (Windows only) | DBeaver, FlameRobin, IBExpert | DBeaver |
| **Adequação ao cenário** | Alta | Média | Alta | Baixa (foco OLAP) |

**Decisão:** SQLite com criptografia via SQLite3 Multiple Ciphers (`SQLitePCLRaw.bundle_e_sqlite3mc`), acessado via Entity Framework Core (`Microsoft.EntityFrameworkCore.Sqlite`).

**Justificativa detalhada:**

*Por que não LiteDB:* apesar da vantagem de ser 100% C# (eliminando interop), o LiteDB apresenta fragilidades de segurança documentadas na sua implementação criptográfica — há issues abertas no repositório oficial (#581) detalhando: uso de modo ECB (que não é criptografia autenticada), iterações PBKDF2 baixas por default, e ausência de IVs imprevisíveis por página. Para um sistema com requisitos elevados de segurança de dados, essas fragilidades são inaceitáveis. Adicionalmente, a ausência de suporte a EF Core impede o uso de Migrations para versionamento de schema, e a ferramenta de inspeção (LiteDB Studio) funciona apenas no Windows.

*Por que não Firebird Embedded:* apesar de ser o SGBD com SQL mais completo da lista (SQL-99+, stored procedures, triggers, MVCC real), a criptografia em repouso nativa requer um plugin comercial (HQbird Encryption Plugin Framework). A versão gratuita do Firebird não inclui criptografia de banco. Para o cenário deste projeto (segurança como prioridade, custo zero), isso é eliminatório. Adicionalmente, o suporte .NET via FirebirdSql.Data.FirebirdClient, embora funcional, tem um ecossistema menor que o SQLite.

*Por que não DuckDB:* projetado para workloads analíticos (OLAP), não transacionais (OLTP). Sem criptografia nativa. Não é o caso de uso deste projeto.

*Por que SQLite:* combina maturidade extrema (25+ anos, testado por bilhões de dispositivos), ecossistema enorme, suporte EF Core nativo com Migrations, e criptografia robusta via SQLite3 Multiple Ciphers (AES-256-CBC com HMAC, derivação de chaves configurável). O pacote `SQLitePCLRaw.bundle_e_sqlite3mc` integra perfeitamente com Microsoft.Data.Sqlite e EF Core, é gratuito, open-source e cross-platform.

**Configuração obrigatória do SQLite:**

```csharp
// Connection string com criptografia:
"Data Source=file:app.db?cipher=aes256cbc;Password=<derived-key>"

// PRAGMAs obrigatórios (executar após abrir conexão):
PRAGMA journal_mode=WAL;        -- Write-Ahead Logging: melhor concorrência leitura/escrita
PRAGMA synchronous=NORMAL;      -- Balance entre performance e durabilidade
PRAGMA foreign_keys=ON;         -- Enforce foreign key constraints (desativado por default!)
PRAGMA busy_timeout=5000;       -- Timeout de 5s em vez de falha imediata se banco locked
PRAGMA auto_vacuum=INCREMENTAL; -- Recuperar espaço de registros deletados
```

**Regras invioláveis para acesso ao banco:**

1. **Nunca** concatenar strings para queries. Sempre parametrizar:
```csharp
// PROIBIDO — vulnerável a SQL injection:
var sql = $"SELECT * FROM Users WHERE Name = '{input}'";

// OBRIGATÓRIO — via EF Core:
var user = await context.Users.FirstOrDefaultAsync(u => u.Name == input);

// OBRIGATÓRIO — via Dapper:
var user = await conn.QueryFirstOrDefaultAsync<User>(
    "SELECT * FROM Users WHERE Name = @Name", new { Name = input });
```

2. **Nunca** armazenar senhas em texto plano. Hashing obrigatório (ver seção 4.2).
3. **Toda** alteração de schema via EF Core Migrations versionadas e rastreáveis.
4. `PRAGMA foreign_keys=ON` em toda conexão (SQLite desativa por default).

### 3.4 Distribuição e Atualização

**Decisão:** Velopack para empacotamento, instalação e atualização automática.

**Fundamentação:** framework cross-platform (Windows, Linux) escrito em Rust. Suporta: instaladores nativos por plataforma, delta packages (download apenas do diff), code signing, rollback automático em caso de falha. CLI simples (`vpk`).

**Mecanismo de atualização:**
1. A aplicação verifica periodicamente (e sob demanda) se há nova versão disponível.
2. O download ocorre em background, sem bloquear o uso.
3. A aplicação notifica o usuário que uma atualização está disponível.
4. O usuário decide quando aplicar a atualização.
5. O rollback para a versão anterior é automático se a atualização falhar.

**Requisitos de segurança do mecanismo de atualização:**
- Pacotes assinados digitalmente. Verificação de assinatura antes de aplicar.
- Canal de download exclusivamente HTTPS com TLS 1.2+.
- Verificação de integridade via SHA-256 antes da instalação.
- Rollback automático em caso de falha durante atualização.

### 3.5 Gerenciamento de Configuração da Aplicação

Configurações não-sensíveis (preferências de UI, idioma, caminhos de diretórios) são armazenadas em arquivo JSON no diretório de dados da aplicação.

**Localização dos dados por plataforma:**
- **Linux:** `$XDG_DATA_HOME/<AppName>/` (tipicamente `~/.local/share/<AppName>/`)
- **Windows:** `%LOCALAPPDATA%\<AppName>\`

**Estrutura:**
```
<AppDataDir>/
├── config.json          # Configurações não sensíveis
├── app.db               # Banco de dados (criptografado)
├── logs/                # Arquivos de log (rotacionados)
└── temp/                # Diretório temporário da aplicação
```

Configurações sensíveis (chaves, tokens) são gerenciadas pelo Secure Storage (ver seção 4.3).

---

## 4. Segurança

A segurança é tratada como propriedade do sistema inteiro, seguindo o princípio de *security by design* estabelecido por McGraw (2004) em "Software Security" (IEEE Security & Privacy). A taxonomia de vulnerabilidades segue o OWASP Desktop App Security Top 10 (2021).

### 4.1 OWASP Desktop App Security Top 10 — Mitigações

**DA1 — Injections:** Queries parametrizadas obrigatórias (ver seção 3.3). Para `Process.Start()`: validar contra whitelist. Para XML: `DtdProcessing.Prohibit`.

**DA2 — Broken Authentication & Session Management:**
- Hash de senhas com **Argon2id** (parâmetros mínimos conforme OWASP e NIST SP 800-63B):

```csharp
// Usando Konscious.Security.Cryptography:
using Konscious.Security.Cryptography;

public static byte[] HashPassword(string password, byte[] salt)
{
    using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
    argon2.Salt = salt;               // 16 bytes de RandomNumberGenerator
    argon2.DegreeOfParallelism = 4;   // threads
    argon2.MemorySize = 65536;        // 64 MB
    argon2.Iterations = 3;
    return argon2.GetBytes(32);       // 256-bit hash
}

public static byte[] GenerateSalt()
{
    var salt = new byte[16];
    RandomNumberGenerator.Fill(salt);
    return salt;
}
```

- **Rate limiting de login:** máximo de 5 tentativas falhas em janela de 15 minutos. Após exceder: lockout progressivo (1 min → 5 min → 15 min → 1 hora).
- **Isolamento entre usuários:** cada usuário local tem perfil separado. Dados de um usuário não são acessíveis por outro, nem via queries ao banco nem via filesystem.

**DA3 — Sensitive Data Exposure:** ver seção 4.3 (Gerenciamento de Segredos).

**DA4 — Improper Cryptography:**

Algoritmos **proibidos** (configurar BannedApiAnalyzers para erro de compilação):

| Proibido | Razão | Alternativa obrigatória |
|---|---|---|
| `MD5` | Colisões triviais desde 2004 | SHA-256 / SHA-512 |
| `SHA1` | Colisão demonstrada (SHAttered, 2017) | SHA-256 / SHA-512 |
| `DES`, `3DES` | Tamanho de bloco/chave insuficiente | AES-256 |
| `RC4` | Múltiplas vulnerabilidades conhecidas | AES-256-GCM |
| `System.Random` (para criptografia) | Previsível, não criptográfico | `RandomNumberGenerator` |
| `Rfc2898DeriveBytes` com defaults | SHA1, 1000 iterações por default | Argon2id ou PBKDF2 com SHA-256 e ≥600.000 iterações |

Algoritmos **obrigatórios:**

| Finalidade | Algoritmo | Implementação .NET |
|---|---|---|
| Criptografia simétrica | AES-256-GCM (autenticado) | `AesGcm` class |
| Hash de senhas | Argon2id | `Konscious.Security.Cryptography` |
| Hash de integridade | SHA-256 | `SHA256.HashData()` |
| Comparação de hashes | Tempo constante | `CryptographicOperations.FixedTimeEquals()` |
| Números aleatórios criptográficos | CSPRNG | `RandomNumberGenerator.Fill()` |

**Comparação de hashes em tempo constante** (mitigação de timing attacks):
```csharp
// PROIBIDO — vulnerável a timing attack:
if (computedHash.SequenceEqual(storedHash)) { ... }

// OBRIGATÓRIO — tempo constante:
if (CryptographicOperations.FixedTimeEquals(computedHash, storedHash)) { ... }
```

**DA5 — Improper Authorization:** RBAC implementado na camada de serviço. Estrutura mínima:
```csharp
public enum Permission { Read, Write, Admin, ManageUsers }
public enum Role { User, Admin }

// Verificação na camada de serviço (nunca apenas na UI):
public async Task<Result<Data>> GetSensitiveData(int userId)
{
    if (!await _authService.HasPermission(userId, Permission.Read))
        return Result<Data>.Failure(Errors.Unauthorized);
    // ...
}
```

**DA6 — Security Misconfiguration:** Remover código debug em release. Permissões de filesystem restritivas no diretório de dados. Dependências atualizadas.

**DA7 — Insecure Communication:** TLS 1.2+ para todo tráfego. Certificate pinning para APIs críticas (obrigatório, não opcional). Nunca `ServerCertificateCustomValidationCallback = (_, _, _, _) => true`.

**DA8 — Poor Code Quality:** Seções 5 e 6 inteiras.

**DA9 — Components with Known Vulnerabilities:** Seção 12 (Gerenciamento de Dependências).

**DA10 — Insufficient Logging:** Seção 8 (Observabilidade).

### 4.2 Gerenciamento de Segredos (Cross-Platform)

A aplicação deve abstrair o armazenamento seguro de credenciais através de uma interface com implementações específicas por plataforma:

```csharp
public interface ISecureStorage
{
    Task StoreAsync(string key, byte[] value);
    Task<byte[]?> RetrieveAsync(string key);
    Task DeleteAsync(string key);
}

// Windows: implementação via DPAPI
public class WindowsSecureStorage : ISecureStorage
{
    public Task StoreAsync(string key, byte[] value)
    {
        var encrypted = ProtectedData.Protect(
            value, entropy: null, DataProtectionScope.CurrentUser);
        // Persistir 'encrypted' no filesystem (o conteúdo é ilegível sem DPAPI)
        var path = GetStoragePath(key);
        File.WriteAllBytes(path, encrypted);
        return Task.CompletedTask;
    }

    public Task<byte[]?> RetrieveAsync(string key)
    {
        var path = GetStoragePath(key);
        if (!File.Exists(path)) return Task.FromResult<byte[]?>(null);
        var encrypted = File.ReadAllBytes(path);
        var decrypted = ProtectedData.Unprotect(
            encrypted, entropy: null, DataProtectionScope.CurrentUser);
        return Task.FromResult<byte[]?>(decrypted);
    }
    // ...
}

// Linux: implementação via libsecret (GNOME Keyring / KWallet)
// Usar biblioteca Tmds.DBus para comunicação D-Bus com o keyring do sistema.
// Fallback: se keyring não estiver disponível, derivar chave da senha do
// usuário via Argon2id e criptografar com AES-256-GCM em arquivo local.
```

**Regra absoluta:** em nenhum cenário, em nenhuma plataforma, segredos podem ser armazenados em texto plano — nem em arquivos de config, nem em variáveis de ambiente, nem no código-fonte.

### 4.3 Segurança de Binários

- **Code signing** de todos os executáveis e pacotes de atualização.
- **Verificação de integridade** na inicialização: a aplicação calcula o hash SHA-256 dos seus próprios binários críticos e compara com hashes esperados embarcados em recurso protegido.
- **NativeAOT** em builds de release: reduz superfície de ataque ao eliminar JIT (impede certas classes de injeção de código em runtime) e dificulta engenharia reversa.

### 4.4 Segurança do Filesystem

- Diretório de dados: permissões `700` (Linux) / ACL equivalente (Windows) — apenas o usuário que executa a aplicação.
- **Path traversal prevention** — toda entrada que represente caminho de arquivo:
```csharp
public static bool IsPathSafe(string userInput, string allowedBaseDir)
{
    var fullPath = Path.GetFullPath(userInput);
    var baseDir = Path.GetFullPath(allowedBaseDir);
    return fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase);
}
```
- Validar tamanho máximo de arquivos de importação.
- Operações temporárias em diretório isolado com nome aleatório.

### 4.5 Segurança de Área de Transferência (Clipboard)

Dados sensíveis copiados para a clipboard ficam acessíveis a qualquer processo do SO. Mitigações:
- Limpar a clipboard automaticamente após 30 segundos se a aplicação colocou dados sensíveis nela.
- Registrar (logar) quando dados sensíveis são copiados.
- Nunca colocar dados sensíveis na clipboard programaticamente sem ação explícita do usuário.

### 4.6 Sequência de Inicialização Segura

A aplicação deve seguir uma sequência de inicialização que estabeleça um estado seguro antes de aceitar qualquer interação:

1. Bootstrap Velopack (verificar/aplicar atualizações pendentes).
2. Verificar integridade dos binários (hash check).
3. Verificar permissões do diretório de dados.
4. Inicializar logging.
5. Verificar integridade do banco (`PRAGMA integrity_check`).
6. Verificar espaço em disco disponível.
7. Inicializar container de DI.
8. Carregar configurações.
9. Apresentar tela de login.

Se qualquer etapa de 2-6 falhar: logar o erro, apresentar mensagem ao usuário, impedir operação normal.

### 4.7 Proteção contra Ataques Específicos a Desktop

| Ataque | Mitigação concreta |
|---|---|
| **DLL Hijacking** | Especificar caminhos absolutos para carregamento de bibliotecas. No Windows: `SetDefaultDllDirectories(LOAD_LIBRARY_SEARCH_APPLICATION_DIR)`. Assinar DLLs. |
| **Memory Dumping** | Limpar dados sensíveis via `CryptographicOperations.ZeroMemory()` após uso. Não manter senhas/chaves em `string` (imutável, fica no heap). Usar `byte[]` e zerar explicitamente. |
| **Timing Attacks** | `CryptographicOperations.FixedTimeEquals()` para toda comparação de hashes/tokens. |
| **Privilege Escalation** | Executar com mínimo de privilégios. Nunca solicitar admin/root desnecessariamente. |
| **Tampering do banco** | Hash SHA-256 do arquivo do banco armazenado no secure storage. Verificar na inicialização. Se divergir: alertar o usuário (possível corrupção ou substituição maliciosa). |

### 4.8 Ciclo de Vida de Chaves Criptográficas

- **Senha esquecida:** não há recuperação (by design). Documentar claramente para o usuário.
- **Troca de senha:** re-criptografar o banco com nova chave derivada. Manter backup do banco com chave antiga até confirmação de sucesso.
- **Rotação de chaves de API:** a aplicação deve suportar atualização de credenciais de APIs externas sem rebuild, via interface de configuração.
- **Exportação criptografada:** a senha de exportação deve atender requisitos mínimos de força (comprimento ≥12 caracteres, entropia mínima verificada). A derivação de chave deve usar Argon2id com parâmetros de custo altos (MemorySize ≥128MB) para tornar brute-force inviável mesmo com senhas mais fracas.

---

## 5. Arquitetura

### 5.1 Monolito Modular

Arquitetura monolítica modular conforme descrita pela SLR de Al-Qora'n & Al-Said Ahmad (2025): "combines operational simplicity with modularity and maintainability." Um único deployable, com módulos internos de fronteiras bem definidas.

**Estrutura de projetos:**

```
Solution/
├── src/
│   ├── App.Desktop/              # Entry point, composição DI, bootstrap Velopack, sequência segura de inicialização
│   ├── App.Core/                 # Abstrações: interfaces, DTOs, value objects, Result<T>, ISecureStorage, ITimeProvider
│   ├── App.Infrastructure/       # Implementações concretas: DbContext, repos, HTTP clients, criptografia, secure storage
│   ├── App.UI/                   # Views XAML, ViewModels, converters, recursos visuais
│   └── Modules/
│       ├── Module.FeatureA/      # Módulo de negócio com serviços, repositórios e modelos próprios
│       ├── Module.FeatureB/
│       └── Module.Shared/        # Funcionalidades compartilhadas (ex: notificações, fila offline)
├── tests/
│   ├── Tests.Unit/
│   ├── Tests.Integration/
│   ├── Tests.E2E/
│   ├── Tests.Security/           # Fuzzing, input malicioso, penetration checklist
│   └── Tests.Performance/        # Testes de carga local (importação de grandes volumes, etc.)
├── tools/
├── docs/
│   ├── adr/                      # Architecture Decision Records
│   └── threat-model/             # Modelo de ameaças
├── Directory.Build.props
├── .editorconfig
├── BannedSymbols.txt             # APIs proibidas para BannedApiAnalyzers
└── Solution.sln
```

**Regras de comunicação entre módulos:**
- Módulos se comunicam via interfaces definidas em `App.Core`.
- Um módulo **nunca** acessa classes internas de outro módulo diretamente.
- Para comunicação assíncrona entre módulos: padrão Event/Message via MediatR ou implementação própria de event bus in-process.

### 5.2 Princípios SOLID com Exemplos Concretos

**S — Single Responsibility:**
```csharp
// ERRADO — classe faz validação, persistência e notificação:
public class UserService {
    public void RegisterUser(UserDto dto) {
        Validate(dto); SaveToDb(dto); SendEmail(dto); // 3 razões para mudar
    }
}

// CORRETO — cada classe uma responsabilidade:
public class UserValidator { ... }
public class UserRepository { ... }
public class UserNotifier { ... }
public class UserRegistrationService {
    public UserRegistrationService(
        UserValidator validator, UserRepository repo, UserNotifier notifier) { ... }
}
```

**D — Dependency Inversion (com DI):**
```csharp
// Toda dependência externa injetada via interface:
public class SyncService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IUserRepository _userRepo;
    private readonly TimeProvider _timeProvider; // Abstração de tempo (.NET 8+)
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        IHttpClientFactory httpFactory,
        IUserRepository userRepo,
        TimeProvider timeProvider,
        ILogger<SyncService> logger)
    {
        _httpFactory = httpFactory;
        _userRepo = userRepo;
        _timeProvider = timeProvider;
        _logger = logger;
    }
}
```

**Result Pattern (substituir exceções para controle de fluxo):**
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

// Uso:
public async Task<Result<User>> GetUser(int id)
{
    var user = await _repo.FindByIdAsync(id);
    return user is null
        ? Result<User>.Failure(new Error("USER_NOT_FOUND", "Usuário não encontrado."))
        : Result<User>.Success(user);
}
```

### 5.3 Internacionalização (i18n)

A internacionalização deve ser considerada desde a primeira linha de código:

- **Nenhuma** string visível ao usuário deve estar hardcoded no código ou XAML. Todas em arquivos de recursos (.resx).
- Usar `CultureInfo.CurrentCulture` para formatação de datas, números e moeda.
- Armazenar datas no banco em UTC. Converter para local apenas na apresentação.
- Considerar textos RTL e comprimentos variáveis de tradução no layout.

### 5.4 Migração de Dados entre Versões

Quando o schema do banco muda entre versões da aplicação, a atualização do banco do usuário deve ser segura:

1. Antes de aplicar migrations: criar backup automático do banco.
2. Aplicar migrations em transação.
3. Se migration falhar: restaurar backup automaticamente e alertar o usuário.
4. Manter registro de versão do schema no banco (EF Core faz isso via tabela `__EFMigrationsHistory`).
5. Testar migrations em ambos os sentidos (up e down) na suite de testes de integração.

---

## 6. Qualidade de Código

### 6.1 Análise Estática (SAST)

**Pacotes NuGet obrigatórios em `Directory.Build.props`:**

| Pacote | Finalidade |
|---|---|
| Roslyn Analyzers (built-in) | Qualidade, design, performance |
| SonarAnalyzer.CSharp | SQL injection, XSS, crypto fraca, hardcoded secrets, code smells |
| Roslynator.Analyzers | 190+ analyzers de estilo e refactoring |
| SecurityCodeScan.VS2019 | Taint analysis para .NET (detecta fluxo de dados inseguros) |
| Microsoft.CodeAnalysis.BannedApiAnalyzers | Proibir APIs inseguras listadas em `BannedSymbols.txt` |

**Arquivo `BannedSymbols.txt` mínimo:**
```
M:System.Security.Cryptography.MD5.Create();Use SHA256 or SHA512
M:System.Security.Cryptography.SHA1.Create();Use SHA256 or SHA512
T:System.Security.Cryptography.DESCryptoServiceProvider;Use AesGcm
T:System.Security.Cryptography.TripleDESCryptoServiceProvider;Use AesGcm
T:System.Security.Cryptography.RC2CryptoServiceProvider;Use AesGcm
M:System.Threading.Thread.Sleep(System.Int32);Use Task.Delay
```

### 6.2 Formatação, Estilo e Convenções

Arquivo `.editorconfig` na raiz do repositório. Regras invioláveis: 4 espaços (sem tabs), UTF-8, modificador de acesso explícito, namespaces file-scoped.

**Convenções de nomenclatura:**

| Elemento | Convenção | Exemplo |
|---|---|---|
| Classes, records, structs | PascalCase | `UserRepository` |
| Interfaces | `I` + PascalCase | `IUserRepository` |
| Métodos | PascalCase | `GetActiveUsers()` |
| Async methods | Sufixo Async | `GetUsersAsync()` |
| Campos privados | `_camelCase` | `_userRepository` |
| Parâmetros, variáveis locais | camelCase | `userName` |
| Constantes | PascalCase | `MaxRetryAttempts` |
| Booleanos | Prefixo is/has/can/should | `isActive`, `hasPermission` |

### 6.3 Práticas de Código

- Métodos: 20-30 linhas como guia geral (não dogma — métodos de mapeamento/validação sequencial podem ser maiores se a divisão artificial prejudicar legibilidade).
- Classes: até 300 linhas. Se ultrapassar, considerar divisão.
- Máximo 3 parâmetros por método; agrupar em objeto se necessário.
- Guard clauses no início do método.
- XML doc comments (`///`) em toda API pública.
- Nenhuma magic number/string.
- Sem `#region`. Sem catch genérico que engole exceção. Todo `IDisposable` com `using`.

---

## 7. Estratégia de Testes

### 7.1 Pirâmide de Testes

Conforme validado empiricamente por Contan, Dehelean & Miclea (2018, IEEE AQTR) e por Wang et al. (2022, Journal of Systems and Software):

```
         /\
        /E2E\        ~10% — fluxos críticos de negócio
       /──────\
      /Integr. \     ~20% — fronteiras entre componentes
     /──────────\
    / Unitários  \   ~70% — lógica de negócio isolada
   /──────────────\
  /  + Security    \  Transversal — em todas as camadas
 /──────────────────\
```

**Nota sobre a proporção 70/20/10:** esta proporção origina-se de projetos web/microservices. Para aplicações desktop monolíticas com UI rica, a proporção pode tender a mais testes de integração (ex: 60/30/10). Ajustar conforme a natureza de cada módulo.

### 7.2 Testes Unitários

**Ferramentas:** xUnit.net + FluentAssertions + NSubstitute.

**Regras:**
- Padrão AAA (Arrange, Act, Assert).
- Nome: `MethodName_Scenario_ExpectedResult`.
- Determinísticos: injetar `TimeProvider` para tempo, seed para aleatoriedade.
- Zero acesso a banco, filesystem, rede.

**O que testar em cada camada:**

| Camada | O que testar |
|---|---|
| **Validators** | Input válido aceito; cada regra de validação violada individualmente; inputs nos limites (boundary values); strings vazias, nulas, extremamente longas, com caracteres especiais |
| **Services** | Fluxo normal (happy path); cada cenário de erro/falha; interação correta com dependências (mockadas); idempotência quando aplicável |
| **ViewModels** | Propriedades bindadas atualizam corretamente; commands habilitam/desabilitam conforme estado; notificações de mudança (INotifyPropertyChanged) disparam |
| **Mappers/Converters** | Mapping de todos os campos; valores nulos; valores nos limites; conversões de tipo |
| **Criptografia** | Encrypt → Decrypt retorna original; senhas diferentes produzem hashes diferentes; mesma senha + mesmo salt = mesmo hash; salt é único por operação |

### 7.3 Testes de Integração

**Cenários típicos com exemplos:**

```csharp
// Teste de integração: repositório + banco SQLite real
public class UserRepositoryIntegrationTests : IAsyncLifetime
{
    private AppDbContext _context;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:") // Banco em memória por teste
            .Options;
        _context = new AppDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task Insert_ValidUser_PersistsAndRetrievesCorrectly()
    {
        var repo = new UserRepository(_context);
        var user = new User { Name = "Test", Email = "test@example.com" };

        await repo.AddAsync(user);
        await _context.SaveChangesAsync();

        var retrieved = await repo.GetByEmailAsync("test@example.com");
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task Insert_DuplicateEmail_ThrowsConstraintViolation()
    {
        // Teste de cenário de borda: violação de unique constraint
        var repo = new UserRepository(_context);
        await repo.AddAsync(new User { Name = "A", Email = "dup@example.com" });
        await _context.SaveChangesAsync();

        var act = () => repo.AddAsync(new User { Name = "B", Email = "dup@example.com" });
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();
}
```

**Teste de resiliência (encerramento abrupto):**
- Simular crash durante escrita no banco (matar o processo após iniciar transação, antes de commit).
- Verificar que o banco se recupera na próxima inicialização (WAL + integrity check).

### 7.4 Testes End-to-End (E2E)

Cobrir apenas fluxos críticos (regra 80/20: 20% dos fluxos que representam 80% do valor).

### 7.5 Testes de Segurança

**Input malicioso — parametrizado em todo ponto de entrada do usuário:**
```csharp
[Theory]
[InlineData("' OR 1=1 --")]
[InlineData("'; DROP TABLE Users;--")]
[InlineData("../../etc/passwd")]
[InlineData("..\\..\\Windows\\System32\\config")]
[InlineData("\0")]                    // null byte
[InlineData("<script>alert(1)</script>")]
[InlineData("A" + /* string de 10.000 caracteres */)]
public async Task ProcessInput_MaliciousPayload_RejectsOrHandlesSafely(string payload)
{
    var result = await _service.ProcessAsync(payload);
    // Deve: rejeitar com erro de validação OU processar sem side effects maliciosos
    // Nunca: crash, SQL executado, path traversal, XSS renderizado
}
```

**Mutation testing com Stryker.NET:** executar periodicamente para validar que os testes de fato detectam mudanças no código. Cobertura de código sem mutation testing é uma métrica enganosa.

### 7.6 Testes de Performance Local

Para funcionalidades de processamento intensivo (importação, exportação, queries pesadas):
- Medir tempo com datasets de 1k, 10k, 100k registros.
- Monitorar pico de memória durante a operação.
- Definir thresholds aceitáveis e falhar o teste se exceder.

### 7.7 Cobertura de Código

**Ferramenta:** Coverlet. **Meta:** 80% de cobertura de linha como mínimo global, 100% de branch coverage em código de segurança.

---

## 8. Observabilidade

### 8.1 Logging Estruturado

**Framework:** Serilog sobre `Microsoft.Extensions.Logging`.

```csharp
// ERRADO — não estruturado, interpola dados sensíveis:
_logger.LogInformation($"User {userId} login attempt with password {password}");

// CORRETO — estruturado, sem dados sensíveis:
_logger.LogInformation("User login attempt. UserId={UserId} Result={Result}", userId, "Success");
```

**Níveis:** Verbose/Debug (dev only), Information (eventos significativos), Warning (anomalias não-fatais), Error (falhas), Fatal (sistema comprometido).

**Regra absoluta:** nunca logar senhas, tokens, dados pessoais, queries SQL com valores de parâmetros.

### 8.2 Health Checks

Na inicialização e periodicamente:
- `PRAGMA integrity_check` no banco.
- Espaço em disco disponível.
- Conectividade com serviços externos (com timeout).
- Permissões de arquivo/diretório.

### 8.3 Métricas

Via `System.Diagnostics.Metrics`: tempo de inicialização, tempo de operações significativas, uso de memória, tamanho do banco, contagem de erros, retries de rede.

---

## 9. Performance

### 9.1 Princípio: Medir Antes de Otimizar

"Premature optimization is the root of all evil" — Knuth (1974), "Structured Programming with go to Statements", ACM Computing Surveys.

Otimizações devem ser precedidas por profiling (`dotnet-counters`, `dotnet-trace`) e seguidas por medição.

### 9.2 Recomendações por Recurso

**Memória:** `Span<T>`/`Memory<T>` para buffers, `ArrayPool<T>.Shared` para alocações temporárias grandes, `StringBuilder` para concatenação em loops, streaming para arquivos grandes.

**CPU:** operações pesadas em background (`Task.Run`), `async/await` para I/O, `CancellationToken` em toda cadeia de chamadas canceláveis.

**Disco (SQLite):** agrupar operações em transações (um `SaveChangesAsync` em vez de um por registro), `PRAGMA journal_mode=WAL`, índices apenas em colunas consultadas em WHERE/JOIN/ORDER BY.

**Rede:** retry com exponential backoff + jitter (Polly), circuit breaker, cache de respostas, timeout em toda chamada, compressão (gzip/brotli).

**Créditos de terceiros:** monitorar consumo de APIs, rate limiting no cliente, cache agressivo.

### 9.3 Concorrência UI Thread vs Background

SQLite com WAL suporta múltiplos leitores e um escritor simultâneos. Porém, operações de escrita concorrentes da UI thread e de tasks de background podem causar `SQLiteBusyException`. Mitigações:
- `PRAGMA busy_timeout=5000` (retry automático por 5 segundos).
- Serializar escritas através de um `SemaphoreSlim` ou canal de mensagens.
- Nunca fazer queries diretamente na UI thread; sempre em task separada.

---

## 10. Arquitetura Offline-First

### 10.1 Padrão de Implementação

- **Detecção de conectividade:** verificar acesso real ao endpoint necessário (não confiar em "está na rede" = "tem internet").
- **Fila de operações pendentes:** tabela no banco local:
```sql
CREATE TABLE PendingOperations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OperationType TEXT NOT NULL,      -- ex: 'sync_data', 'fetch_update'
    Payload TEXT NOT NULL,            -- JSON serializado dos dados necessários
    Status TEXT NOT NULL DEFAULT 'pending', -- pending, in_progress, failed, completed
    Attempts INTEGER NOT NULL DEFAULT 0,
    MaxAttempts INTEGER NOT NULL DEFAULT 5,
    NextRetryAt TEXT,                 -- ISO 8601 UTC
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    ErrorMessage TEXT
);
```
- **Processamento:** FIFO, retry com backoff exponencial, desistir após MaxAttempts.
- **Indicação visual:** UI indica online/offline e operações pendentes.
- **Conflitos:** definir por funcionalidade (last-write-wins, merge, ou prompt ao usuário).

---

## 11. Tratamento de Erros

**Exceções para situações excepcionais** (falha de I/O, bug). **Result Pattern para falhas de negócio** (validação, recurso não encontrado).

**Handler global:** `AppDomain.CurrentDomain.UnhandledException` + handler do Avalonia. Registrar com stack trace completo. Exibir mensagem genérica ao usuário. Salvar estado se possível. Nunca expor stack traces ou queries SQL ao usuário.

**Categorias:**

| Categoria | Exemplo | Ação |
|---|---|---|
| Validação | Input inválido | Result com mensagem clara. Não logar. |
| Negócio | Operação não permitida | Result. Logar como Warning. |
| Infra recuperável | Timeout de rede | Retry + backoff. Logar Warning. |
| Infra não recuperável | Banco corrompido | Logar Critical. Alertar usuário. Oferecer recovery. |
| Bug | NullReference, IndexOutOfRange | Handler global. Logar Critical com stack trace. |

---

## 12. Gerenciamento de Dependências

**Critérios de adoção (antes de adicionar qualquer NuGet):**
- É necessário ou o SDK/.NET já fornece?
- Licença compatível (MIT, Apache 2.0, BSD. Evitar GPL).
- Manutenção ativa (commits recentes, issues respondidas).
- Vulnerabilidades conhecidas (NVD).
- Quantas dependências transitivas traz?

**Manutenção:** `dotnet list package --vulnerable` no CI (toda build). `dotnet-outdated` mensal. Patch versions imediatamente. Minor/major: avaliar changelog.

**Arquivo `DEPENDENCIES.md`:** listar cada dependência, versão, licença, motivo da inclusão.

---

## 13. Importação / Exportação / Backup

- Formato self-contained e versionado (metadata de versão de schema).
- Criptografado com AES-256-GCM, chave derivada de senha do usuário via Argon2id (MemorySize ≥128MB para exportação).
- **Requisito de força da senha de exportação:** mínimo 12 caracteres com verificação de entropia.
- Importação: validar integridade (SHA-256), versão de schema, decriptação antes de aplicar.
- Suportar migrations de schema para backups de versões anteriores.
- Processar em diretório temporário isolado.

---

## 14. Controle de Versão e CI

### 14.1 Git

- Conventional Commits (`feat:`, `fix:`, `security:`, `refactor:`, `test:`, `docs:`, `chore:`).
- `.gitignore` configurado. Nunca commitar segredos, binários, arquivos de banco, bin/obj.
- Pre-commit hook: `dotnet format --verify-no-changes`.

### 14.2 Pipeline de CI

Cada push/merge request:

1. `dotnet restore`
2. `dotnet build` (analyzers = erros. Se warning: falha.)
3. `dotnet list package --vulnerable` (se vulnerabilidade: falha.)
4. `dotnet test Tests.Unit` (se falha: falha.)
5. `dotnet test Tests.Integration` (se falha: falha.)
6. Cobertura de código (se <80%: falha.)
7. `dotnet test Tests.E2E` (branches principais e pre-release.)
8. `dotnet test Tests.Security` (nightly ou semanal.)
9. Packaging via Velopack com assinatura digital.

**Nenhum código chega ao branch principal se etapas 1-6 falharem.**

---

## 15. Recuperação de Desastres

| Cenário | Procedimento |
|---|---|
| **Banco corrompido** | Detectado por `PRAGMA integrity_check` na inicialização. Alertar usuário. Oferecer restauração do último backup automático (criado antes de cada migration e periodicamente). |
| **Instalador falha no meio** | Velopack faz rollback automático para versão anterior. |
| **SO reinstalado** | Usuário importa backup exportado previamente. Guia de backup deve estar na documentação do usuário. |
| **Espaço em disco esgotado** | Detectado na inicialização e antes de operações de escrita. Modo read-only até liberar espaço. |

**Backups automáticos:** antes de cada migration de schema e a cada N dias (configurável), manter últimos M backups no diretório de dados.

---

## 16. Diferenças entre Plataformas

| Aspecto | Linux | Windows |
|---|---|---|
| **Diretório de dados** | `$XDG_DATA_HOME/<App>/` | `%LOCALAPPDATA%\<App>\` |
| **Secure Storage** | libsecret (GNOME Keyring / KWallet) via D-Bus | DPAPI (`ProtectedData`) |
| **Permissões de arquivo** | `chmod 700` no diretório de dados | ACLs via `System.Security.AccessControl` |
| **Code signing** | GPG ou sigstore | Authenticode (certificado PFX) |
| **Instalador** | AppImage ou .deb/.rpm via Velopack | Setup.exe via Velopack |
| **Anti-DLL hijacking** | `LD_LIBRARY_PATH` restritivo | `SetDefaultDllDirectories` |

A abstração `ISecureStorage` e as diferenças de filesystem devem ser implementadas com platform detection (`RuntimeInformation.IsOSPlatform`).

---

## 17. Acessibilidade

- Navegação por teclado em toda a interface.
- Contraste adequado (WCAG AA mínimo, ratio 4.5:1).
- Tamanho mínimo de elementos interativos (44x44dp).
- Não transmitir informação apenas por cor.
- `AutomationProperties` em Avalonia para screen readers.

---

## 18. Documentação

- XML doc comments em toda API pública.
- `README.md`: propósito, setup, build, testes, instalador.
- `CONTRIBUTING.md`: padrões e processo.
- `CHANGELOG.md`: gerado de Conventional Commits.
- `DEPENDENCIES.md`: inventário de dependências.
- ADRs em `docs/adr/` para toda decisão técnica significativa.

---

## 19. Checklist de Revisão de Código

**Funcionalidade:** implementa o requisito; cobre cenários de borda; funciona offline (se aplicável).

**Segurança:** inputs validados/sanitizados; nenhum dado sensível em logs/mensagens de erro/código; queries parametrizadas; nenhum segredo no código; permissões verificadas na camada de serviço; comparações de hash em tempo constante.

**Qualidade:** compila sem warnings; analyzers satisfeitos; nomes claros; métodos curtos; dependências injetadas.

**Testes:** unitários para cenários normais e de borda; integração para fronteiras; inputs maliciosos para entradas do usuário; cobertura não diminuiu.

**Performance:** operações pesadas em background; transações agrupadas; recursos descartáveis liberados; nenhuma alocação desnecessária em hot paths.

**Observabilidade:** operações significativas logadas; erros com contexto suficiente; nenhum dado sensível em logs.

---

## Apêndice A: Pacotes NuGet

| Categoria | Pacote | Finalidade |
|---|---|---|
| UI | Avalonia UI, SukiUI, CommunityToolkit.Mvvm | Interface e MVVM |
| ORM | Microsoft.EntityFrameworkCore.Sqlite, Microsoft.Data.Sqlite.Core | Acesso a dados |
| DB Crypto | SQLitePCLRaw.bundle_e_sqlite3mc | Criptografia AES-256 para SQLite |
| Logging | Serilog, Serilog.Extensions.Logging, Serilog.Sinks.File | Logging estruturado |
| DI | Microsoft.Extensions.DependencyInjection | Container IoC |
| HTTP/Resiliência | Polly, Microsoft.Extensions.Http.Resilience | Retry, circuit breaker |
| Testes | xUnit, FluentAssertions, NSubstitute, Coverlet | Framework de testes |
| Mutation Testing | Stryker.NET | Qualidade dos testes |
| Analyzers | SonarAnalyzer.CSharp, Roslynator, SecurityCodeScan, BannedApiAnalyzers | SAST |
| Updates | Velopack | Instalação e atualização |
| Serialization | System.Text.Json (built-in) | JSON |
| Password Hashing | Konscious.Security.Cryptography | Argon2id |

## Apêndice B: APIs Proibidas

Configuradas em `BannedSymbols.txt` e detectadas como erros de compilação.

| Proibido | Alternativa |
|---|---|
| `MD5.Create()`, `SHA1.Create()` | `SHA256.HashData()`, `SHA512.HashData()` |
| `DES`, `3DES`, `RC2`, `RC4` | `AesGcm` |
| `System.Random` (para segurança) | `RandomNumberGenerator` |
| `Thread.Sleep` | `Task.Delay` |
| `GC.Collect()` | Confiar no GC |
| String concatenation em SQL | Queries parametrizadas |
| `dynamic` (exceto interop) | Tipagem forte |
| `#pragma warning disable` sem justificativa | Corrigir o warning |
| `catch (Exception) { }` | Log + re-throw ou tratamento específico |

## Apêndice C: Threat Model Summary

Modelo de ameaças STRIDE simplificado para o sistema:

| Categoria | Ameaça | Mitigação (seção) |
|---|---|---|
| **Spoofing** | Atacante finge ser outro usuário local | Autenticação com Argon2id (4.1 DA2) |
| **Tampering** | Modificação do banco ou binários | Hash de integridade + code signing (4.3, 4.7) |
| **Repudiation** | Negação de ação realizada | Logging estruturado com audit trail (8.1) |
| **Information Disclosure** | Vazamento de dados sensíveis | Criptografia em repouso + secure storage (3.3, 4.2) |
| **Denial of Service** | Esgotamento de recursos por input malicioso | Validação de input + limites de tamanho (4.1 DA1) |
| **Elevation of Privilege** | Acesso a dados/funções de outro perfil | RBAC na camada de serviço (4.1 DA5) |

## Apêndice D: Referências Científicas e Normativas Completas

### Periódicos e Conferências Peer-Reviewed

1. Al-Qora'n, L. & Al-Said Ahmad, A. (2025). "Modular Monolith Architecture in Cloud Environments: A Systematic Literature Review." *Future Internet*, vol. 17, no. 11, p. 496. MDPI. DOI: 10.3390/fi17110496

2. Contan, C., Dehelean, C. & Miclea, L. (2018). "Test Automation Pyramid from Theory to Practice." *IEEE International Conference on Automation, Quality and Testing, Robotics (AQTR)*. DOI: 10.1109/AQTR.2018.8402699

3. Wang, Y., Mäntylä, M., Liu, Z. & Markkula, J. (2022). "Test automation maturity improves product quality — Quantitative study of open source projects using continuous integration." *Journal of Systems and Software*, vol. 188, Elsevier. DOI: 10.1016/j.jss.2022.111262

4. Khan, R. A. et al. (2022). "Systematic Literature Review on Security Risks and its Practices in Secure Software Development." *IEEE Access*, vol. 10. DOI: 10.1109/ACCESS.2022.3140181

5. McGraw, G. (2004). "Software Security." *IEEE Security & Privacy*, vol. 2, no. 2, pp. 80-83. DOI: 10.1109/MSECP.2004.1281254

6. Amaro, R., Pereira, R. & da Silva, M. M. (2023). "Capabilities and practices in DevOps: a multivocal literature review." *IEEE Transactions on Software Engineering*, vol. 49, pp. 883-901. DOI: 10.1109/TSE.2022.3170654

7. Pereira, R. et al. (2024). "DevSecOps practices and tools." *International Journal of Information Security*, Springer. DOI: 10.1007/s10207-024-00914-z

8. Su, R. & Li, X. (2024). "Modular Monolith: Is This the Trend in Software Architecture?" *Proceedings of the 1st International Workshop on New Trends in Software Architecture (SATrends '24)*, ACM. DOI: 10.1145/3643657.3643911

9. Cruzes, D. S. et al. (2017). "How is Security Testing Done in Agile Teams? A Cross-Case Analysis of Four Software Teams." *International Conference on Agile Software Development (XP 2017)*, Springer.

10. Barde, K. (2023). "Modular Monoliths: Revolutionizing Software Architecture for Efficient Payment Systems in Fintech." *International Journal of Computer Trends and Technology (IJCTT)*, vol. 71, no. 10, pp. 20-27. DOI: 10.14445/22312803/IJCTT-V71I10P103

### Livros de Referência Acadêmica

11. Bass, L., Clements, P. & Kazman, R. (2012). *Software Architecture in Practice*, 3rd ed. Pearson Education.

12. Martin, R. C. (2017). *Clean Architecture: A Craftsman's Guide to Software Structure and Design*. Prentice Hall.

13. Evans, E. (2003). *Domain-Driven Design: Tackling Complexity in the Heart of Software*. Addison-Wesley.

14. Humble, J. & Farley, D. (2010). *Continuous Delivery: Reliable Software Releases through Build, Test, and Deployment Automation*. Addison-Wesley.

### Padrões Normativos

15. OWASP Desktop App Security Top 10 (2021). https://owasp.org/www-project-desktop-app-security-top-10/

16. OWASP Desktop Application Security Verification Standard (DASVS).

17. NIST SP 800-132 — Recommendation for Password-Based Key Derivation.

18. NIST SP 800-63B — Digital Identity Guidelines: Authentication and Lifecycle Management.

19. IEEE Std 2675-2021 — Standard for DevOps: Building Reliable and Secure Systems.

20. ISO/IEC 25010:2011 — Systems and software engineering: Systems and software Quality Requirements and Evaluation.

### Documentações Técnicas Oficiais

21. Microsoft .NET Security Guidelines. https://learn.microsoft.com/dotnet/standard/security/

22. SQLite Documentation. https://sqlite.org/docs.html

23. Velopack Documentation. https://docs.velopack.io/

24. Avalonia UI Documentation. https://docs.avaloniaui.net/

25. SQLite3 Multiple Ciphers. https://utelle.github.io/SQLite3MultipleCiphers/

---

*Documento vivo. Atualizar a cada nova decisão técnica, ameaça identificada ou revisão de padrão.*
