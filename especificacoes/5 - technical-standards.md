# Documento de Padrões e Diretrizes Técnicas — v4.0

## Guia Técnico de Referência — Sistema DiarioDeBordo

**Versão:** 4.0
**Classificação:** Específico ao domínio do sistema DiarioDeBordo — define *como* implementar as decisões de domínio documentadas nos documentos complementares.
**Plataformas-alvo:** Linux e Windows.

---

## 1. Propósito e Escopo

Este documento é a **fonte da verdade técnica** do projeto DiarioDeBordo — Sistema de Gestão e Consumo de Conteúdo com Agência do Usuário. Qualquer desenvolvedor que o leia, em conjunto com os documentos de domínio, deve conseguir produzir implementações seguras, testadas e manuteníveis sem depender de conhecimento prévio não documentado.

**O sistema em uma frase:** Aplicação desktop nativa, offline-first, que devolve ao usuário a agência sobre seu consumo de conteúdo, oferecendo gestão pessoal de acervo (Pilar 1) e agregação de fontes externas sem dark patterns (Pilar 2).

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
- Definição de Domínio v3 — regras de negócio, princípios, modelo de domínio, cenários de validação.
- Mapa de Domínio v1 — classificação de subdomínios (core, suporte, genérico).
- Mapa de Contexto v1 — bounded contexts, linguagem ubíqua, padrões de relacionamento.
- Plano de Implementação v3 — etapas, estratégia incremental, critérios de conclusão.
- ADRs (Architecture Decision Records) em `docs/adr/`.
- Modelo de ameaças (threat model) em `docs/threat-model/`.

**Bounded Contexts do sistema (conforme Mapa de Contexto v1):**

| Contexto | Classificação | Subdomínios agrupados |
|---|---|---|
| Acervo | Principal (Prioridade 1) | Gestão de Conteúdo + Curadoria e Coletâneas |
| Agregação | Principal (Prioridade 2) | Agregação e Subscrição |
| Reprodução | Suporte | Reprodução de Conteúdo |
| Integração Externa | Suporte | Integração com Plataformas Externas |
| Busca | Suporte | Busca e Navegação |
| Portabilidade | Suporte | Portabilidade de Dados |
| Identidade | Genérico | Identidade e Acesso |
| Preferências | Genérico | Personalização de Interface |

**Mapeamento de Pilares para Contextos (conforme Definição de Domínio v3, seção 1):**
- **Pilar 1 — Gestão de conteúdo pessoal (offline):** Acervo, Reprodução, Busca. Funciona integralmente sem internet.
- **Pilar 2 — Agregação de fontes externas (online):** Agregação, Integração Externa. Depende de internet para montar feeds, mas conteúdos já persistidos ficam disponíveis offline.
- **Transversais a ambos:** Identidade, Preferências, Portabilidade.

Cada pilar tem valor independente — o sistema deve ser funcional e testável com apenas o Pilar 1 implementado (conforme Etapas 1-3 do Plano de Implementação v3).

**Preocupação transversal:** Uso Saudável — conjunto de restrições e princípios que permeiam o design de **todos** os contextos e **todas** as camadas (UI, serviço, dados). Não é um módulo isolado — é uma lente verificável em revisão de código, testes e análise estática (seção 5.4). Toda seção deste documento que toca apresentação de dados, listas, feeds ou configurações do usuário deve ser lida à luz dessa restrição.

### 1.1 Princípios de Desenvolvimento

Conforme Plano de Implementação v3 (seção 1.2), o desenvolvimento segue cinco princípios que impactam decisões técnicas:

1. **Excelência sobre velocidade.** Períodos de estudo são parte do plano, não desvios. O documento técnico não deve ser tratado como receita a executar às cegas.
2. **Design intencional + evolução emergente.** A arquitetura base (bounded contexts, separação de camadas, interfaces) é intencional. Detalhes internos emergem do código. Ver seção 1.2 para distinção.
3. **Decisões no último momento responsável.** Detalhes de implementação que este documento define (schema de banco, padrões internos de cada módulo) são diretrizes, não imposições prematuras. Se a experiência de uso revelar necessidade de ajuste, o código prevalece sobre o documento.
4. **Uso próprio como validação.** O desenvolvedor usa o sistema o mais cedo possível. Feedback de uso real é o principal mecanismo de validação.
5. **Refatoração entre etapas.** Toda transição entre etapas do plano inclui retrospectiva e refatoração.

### 1.2 Intencional vs. Emergente

Conforme Plano de Implementação v3 (seção 1.4):

| Aspecto | Intencional (definido de propósito) | Emergente (surge do código e do uso) |
|---|---|---|
| Arquitetura | Monolito modular, bounded contexts, separação de camadas | Padrões internos de cada módulo, granularidade de classes |
| Domínio | Entidades, agregados, invariantes, interfaces entre contextos | Implementação concreta de repositórios, helpers, utilitários |
| Persistência | Interface de repositório (contrato) | Schema concreto, otimizações de query |
| Interface | Princípios (paginação, disclosure progressivo, sem scroll infinito) | Layout concreto, componentes específicos, fluxos de navegação |
| Testes | Cobertura de invariantes e cenários do Apêndice A | Granularidade dos testes, mocks específicos, fixtures |
| Integração externa | Contrato padronizado de adaptadores | Implementação de cada adaptador, edge cases por plataforma |

---

## 2. Fundamentação Científica e Normativa

Toda decisão neste documento é rastreável a publicações científicas peer-reviewed, padrões normativos reconhecidos, ou documentações técnicas oficiais. Autores seminais (Martin, Fowler, Evans) são referenciados apenas quando acompanhados de estudos empíricos que validam os conceitos citados.

Decisões de design que não possuem embasamento direto em periódicos científicos são explicitamente declaradas como pragmáticas, com transparência sobre a ausência de evidência experimental — consistente com a abordagem da Definição de Domínio v3 (seção 11.1).

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
- State containers para estado compartilhado (ex: usuário logado, status de conectividade, preferências de tema ativo).
- Nenhum ViewModel deve instanciar ou referenciar diretamente outro ViewModel.

**Exemplo — evento de domínio entre ViewModels:**
```csharp
// Quando o usuário interage com um ItemFeed e ele se torna um Conteúdo:
public sealed record ItemFeedPersistidoNotification(Guid ConteudoId, string Titulo) : INotification;

// O ViewModel do Agregador publica:
await _mediator.Publish(new ItemFeedPersistidoNotification(conteudo.Id, conteudo.Titulo));

// O ViewModel do Acervo consome (se estiver ativo):
public class AcervoViewModel : ObservableObject, INotificationHandler<ItemFeedPersistidoNotification>
{
    public Task Handle(ItemFeedPersistidoNotification n, CancellationToken ct)
    {
        // Atualizar lista se visível
    }
}
```

**Cancelamento de operações longas na UI:**
```csharp
public partial class ImportacaoViewModel : ObservableObject
{
    private CancellationTokenSource? _cts;

    [RelayCommand]
    private async Task ImportarAsync()
    {
        _cts = new CancellationTokenSource();
        try
        {
            IsImportando = true;
            await _importacaoService.ExecutarAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            MensagemStatus = "Importação cancelada pelo usuário.";
        }
        finally
        {
            IsImportando = false;
            _cts.Dispose();
            _cts = null;
        }
    }

    [RelayCommand]
    private void CancelarImportacao() => _cts?.Cancel();
}
```

**Componentes obrigatórios de UI (Uso Saudável — seção 5.4):**
- Todo componente que apresente listas **deve** usar `PaginatedList<T>` (seção 6.7). Nenhuma lista sem paginação é aceitável.
- Formulários de cadastro de conteúdo **devem** implementar disclosure progressivo (seção 6.10) — apenas título visível por padrão, demais campos revelados sob demanda.
- Nenhum componente de autoplay entre conteúdos. Nenhum componente de scroll infinito.

### 3.3 Banco de Dados

**Decisão:** PostgreSQL, instalado junto com a aplicação via instalador (ADR-002).

**Justificativa:**
- SQL:2011+ — o SQL mais completo no open-source. CTEs recursivos (essenciais para navegação de coletâneas compostas e detecção de ciclos), window functions, JSONB, arrays, tipos customizados, full-text search avançado, domains para validação de tipos.
- MVCC real — múltiplos leitores e escritores simultâneos sem bloqueio. Crítico dado que a aplicação terá background services (montagem de feeds, busca de metadados) fazendo I/O no banco simultaneamente à UI.
- Criptografia — TDE via extensão pg_tde (Percona, open-source, PostgreSQL License) para criptografia transparente em repouso; ou criptografia por coluna via pgcrypto (incluído na distribuição padrão). Para dados sensíveis específicos, criptografia na camada de aplicação com AES-256-GCM antes de persistir.
- EF Core — Npgsql.EntityFrameworkCore.PostgreSQL é o provider EF Core mais maduro depois do SQL Server. Suporte completo a Migrations, LINQ, scaffolding.
- Full-text search em português — PostgreSQL possui dicionário `portuguese` nativo para busca textual, essencial para o contexto Busca operar sobre título, descrição e anotações em pt-BR.
- Licença — PostgreSQL License (tipo BSD), sem copyleft, sem restrições de distribuição.
- Tipagem estrita — um INTEGER é um INTEGER. Não aceita texto onde se espera número.

**Instalação bundled — o que o instalador deve fazer:**

*Windows:*
1. Incluir binários do PostgreSQL no pacote de instalação.
2. Executar `initdb` com encoding UTF-8, locale `pt_BR.UTF-8` e autenticação SCRAM-SHA-256.
3. Configurar `pg_hba.conf` para aceitar apenas conexões locais (127.0.0.1, ::1).
4. Registrar PostgreSQL como Windows Service (inicialização automática).
5. Criar database `diariodebordo` e usuário da aplicação com senha gerada aleatoriamente (armazenada no Secure Storage).
6. Configurar porta não-padrão (ex: 15432) para evitar conflito com PostgreSQL existente.

*Linux:*
1. Incluir PostgreSQL como dependência do pacote (.deb/.rpm) ou bundlear binários.
2. `initdb` com locale `pt_BR.UTF-8` + configurar `pg_hba.conf` (apenas localhost).
3. Registrar como serviço systemd (user-level, não system-level — não requer root após instalação).
4. Porta não-padrão, database `diariodebordo` e usuário criados automaticamente.

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
shared_buffers = '128MB'
work_mem = '16MB'
maintenance_work_mem = '64MB'
effective_cache_size = '512MB'
max_connections = 10
listen_addresses = 'localhost'
port = 15432
password_encryption = 'scram-sha-256'

-- Habilitar extensões:
CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS unaccent;  -- remover acentos na busca textual

-- Configuração de full-text search para pt-BR:
CREATE TEXT SEARCH CONFIGURATION pt_br (COPY = portuguese);
ALTER TEXT SEARCH CONFIGURATION pt_br
    ALTER MAPPING FOR asciiword, asciihword, hword_asciipart, word, hword, hword_part
    WITH unaccent, portuguese_stem;
```

**Índices de full-text search para o contexto Busca:**
```sql
-- Índice GIN para busca textual sobre conteúdos (título, descrição, anotações):
CREATE INDEX idx_conteudo_busca ON conteudos
    USING GIN (to_tsvector('pt_br', coalesce(titulo, '') || ' ' || coalesce(descricao, '') || ' ' || coalesce(anotacoes, '')));

-- Índice para categorias (autocompletar com não-duplicação):
CREATE UNIQUE INDEX idx_categoria_nome_usuario ON categorias (usuario_id, lower(nome));

-- Índice para detecção de ciclos em coletâneas (ancestralidade):
-- Implementado via CTE recursivo, não requer índice dedicado além da FK.
```

**Regras invioláveis de acesso ao banco:**

1. Queries parametrizadas exclusivamente (EF Core ou Dapper com parâmetros):
```csharp
// PROIBIDO:
var sql = $"SELECT * FROM conteudos WHERE titulo = '{input}'";

// OBRIGATÓRIO:
var conteudo = await context.Conteudos.FirstOrDefaultAsync(c => c.Titulo == input);
```
2. Senhas nunca em texto plano. Hash com Argon2id (ver seção 4).
3. Toda alteração de schema via EF Core Migrations versionadas.
4. Credenciais do banco no Secure Storage do SO, nunca no código ou config files.
5. Para dados sensíveis deletados, executar `VACUUM` para garantir que páginas livres sejam sobreescritas.

**Abstração obrigatória para troca futura de banco:**

O acesso a dados deve ser abstraído via Repository Pattern + EF Core de modo que trocar o banco no futuro exija mudanças apenas em `DiarioDeBordo.Infrastructure`:

```csharp
// A troca de provider é uma única linha:
options.UseNpgsql(connectionString);   // PostgreSQL
options.UseSqlite(connectionString);   // SQLite (cenário simplificado)
options.UseFirebird(connectionString); // Firebird
```

Nenhum código fora de `DiarioDeBordo.Infrastructure` deve ter dependência direta no provider do banco.

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
- Linux: `$XDG_DATA_HOME/DiarioDeBordo/` (tipicamente `~/.local/share/DiarioDeBordo/`)
- Windows: `%LOCALAPPDATA%\DiarioDeBordo\`

**Configurações sensíveis:** Secure Storage do SO (ver seção 4.3).

**Configurações do domínio — dois níveis:**

Conforme o modelo de domínio, configurações possuem dois níveis de autoridade:
1. **Defaults globais (admin):** valores padrão para todos os usuários (itens por página, tema padrão, fontes de metadados de imagem padrão, limites do sistema).
2. **Configurações do usuário (consumidor):** sobrescrevem os defaults globais para cada usuário individualmente.

```csharp
public interface IConfiguracaoService
{
    Task<T> ObterAsync<T>(string chave, Guid usuarioId);
    // Resolução: preferência do usuário > default global > valor hardcoded
}
```

**Feature Flags:** mecanismo para ativar/desativar funcionalidades sem rebuild, alinhado ao Plano de Implementação v3 (entrega incremental):
```csharp
public interface IFeatureFlags
{
    bool IsEnabled(string featureName);
}
```

**Mapeamento completo de feature flags por etapa do Plano de Implementação v3:**

| Flag | Etapa | Descrição |
|---|---|---|
| `coletaneas` | 3 | Coletâneas Guiada e Miscelânea, composição, proteção contra ciclos |
| `fontes_com_fallback` | 3 | Fontes com prioridade e fallback, imagens de conteúdo |
| `deduplicacao` | 3 | Detecção e resolução de conteúdos duplicados |
| `adaptador_rss` | 4 | Adaptador de plataforma RSS |
| `adaptador_youtube` | 4 | Adaptador de plataforma YouTube |
| `agregador` | 5 | Subscrição, montagem de feed, agregador consolidado, persistência seletiva |
| `busca_avancada` | 6 | Full-text search, filtros combinados, operações em lote |
| `reprodutor_interno` | 7 | Reprodutor de texto/áudio/vídeo, ganchos, abertura externa |
| `multi_usuario` | 8 | Autenticação, roles, grupos, área admin, personalização |
| `portabilidade` | 9 | Exportação/importação de dados e configurações |
| `intervencoes_uso_saudavel` | 10 | Monitoramento de tempo, lembretes, relatório de consumo, modo cinza |
| `rascunhos_automaticos` | 2 | Persistência periódica de rascunho em formulários longos, recuperação após crash |

Nota: Etapas 0 (modelagem) e 1 (walking skeleton) não requerem feature flags — são etapas de fundação. O core da Etapa 2 (CRUD de conteúdo, dashboard) é a funcionalidade mínima, sempre ativa e sem flag. A flag `rascunhos_automaticos` é um enhancement opcional dentro da Etapa 2: pode ser desativada independentemente sem afetar o CRUD básico.

Feature flags permitem deploy incremental conforme as etapas do plano e desativação rápida de funcionalidades problemáticas sem rebuild.

---

## 4. Segurança

A segurança é propriedade do sistema inteiro, não feature adicionada depois — princípio de *security by design* (McGraw, 2004, IEEE Security & Privacy; validado empiricamente por Khan et al., 2022, IEEE Access, em revisão sistemática de 95 estudos primários sobre integração de segurança no SDLC).

### 4.1 OWASP Desktop App Security Top 10

**DA1 — Injections:** queries parametrizadas obrigatórias. `Process.Start()` validado contra whitelist (relevante para abertura externa de conteúdos no reprodutor — seção 6.6). XML com `DtdProcessing.Prohibit` (relevante para parsing de feeds RSS). Desserialização JSON: usar `System.Text.Json` com `JsonSerializerOptions` restritivo (sem `TypeNameHandling`, sem polimorfismo não-controlado). Para importação de dados (contexto Portabilidade): validar tamanho do JSON (prevenir JSON bombs), profundidade máxima de nesting, e schema antes de processar.

**Validação específica de URLs de fonte:** Quando o usuário cadastra uma fonte (URL de canal YouTube, feed RSS, etc.), validar:
- Schema permitido: `https://` obrigatório para URLs externas; `file://` para fontes locais.
- Rejeitar URLs com IP privado ou localhost (prevenção de SSRF).
- Sanitizar antes de armazenar (remover caracteres de controle, normalizar encoding).

**DA2 — Broken Authentication:**

Hash de senhas com Argon2id (parâmetros conforme OWASP, validados pelo NIST SP 800-63B que recomenda *memory-hard functions*):

```csharp
using Konscious.Security.Cryptography;

public static class HashDeSenha
{
    private const int TamanhoSalt = 16;    // 128 bits
    private const int TamanhoHash = 32;    // 256 bits
    private const int TamanhoMemoria = 65536; // 64 MB
    private const int Iteracoes = 3;
    private const int Paralelismo = 4;

    public static (byte[] hash, byte[] salt) Gerar(string senha)
    {
        var salt = new byte[TamanhoSalt];
        RandomNumberGenerator.Fill(salt);
        var hash = DerivarChave(senha, salt);
        return (hash, salt);
    }

    public static bool Verificar(string senha, byte[] hashArmazenado, byte[] salt)
    {
        var hashComputado = DerivarChave(senha, salt);
        return CryptographicOperations.FixedTimeEquals(hashComputado, hashArmazenado);
    }

    private static byte[] DerivarChave(string senha, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(senha));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = Paralelismo;
        argon2.MemorySize = TamanhoMemoria;
        argon2.Iterations = Iteracoes;
        return argon2.GetBytes(TamanhoHash);
    }
}
```

**Nota sobre NIST SP 800-63B:** o NIST recomenda explicitamente *memory-hard functions* para hash de senhas. Argon2id é listado como opção preferencial. Este documento adota Argon2id por ser superior a PBKDF2 em resistência a ataques com hardware especializado (GPU/ASIC).

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

O modelo de autorização do DiarioDeBordo é baseado em RBAC com duas roles fixas (Consumidor, Admin) e grupos customizáveis (seção 4.6). A verificação de permissão **deve** ocorrer na camada de serviço, nunca apenas na UI:

```csharp
public async Task<Result<ConfiguracaoGlobal>> AlterarConfiguracaoGlobalAsync(
    Guid usuarioId, string chave, string valor)
{
    if (!await _autorizacaoService.PossuiRole(usuarioId, Role.Admin))
    {
        _logger.LogWarning(
            "Tentativa não autorizada de alterar configuração global. UsuarioId={UsuarioId}",
            usuarioId);
        return Result<ConfiguracaoGlobal>.Failure(Erros.NaoAutorizado);
    }
    // ...
}
```

**DA6 — Security Misconfiguration:**
- `#if DEBUG` para funcionalidades de desenvolvimento; garantir que nunca vazem para release via configuração de build.
- Permissões de filesystem: diretório de dados com permissões `700` (Linux) / ACL restritiva (Windows).
- PostgreSQL: `listen_addresses = 'localhost'`, autenticação SCRAM-SHA-256, `pg_hba.conf` somente local.
- Remover extensões PostgreSQL não utilizadas. Desabilitar `log_statement = 'all'` em produção (loga queries com dados).

**DA7 — Insecure Communication:** TLS 1.2+ para todo tráfego externo (consultas a APIs do YouTube, RSS feeds, fontes de metadados). Certificate pinning obrigatório para APIs críticas. Verificar OCSP/CRL para revogação de certificados.

**DA8 — Poor Code Quality:** Seções 7 e 8.

**DA9 — Components with Known Vulnerabilities:** Seção 13.

**DA10 — Insufficient Logging:** Seção 9.

### 4.2 Proteção contra Ataques Específicos

| Ataque | Mitigação |
|---|---|
| **DLL Hijacking** | Caminhos absolutos para libs. Windows: `SetDefaultDllDirectories(LOAD_LIBRARY_SEARCH_APPLICATION_DIR)`. Assinar DLLs. |
| **Memory Dumping** | `CryptographicOperations.ZeroMemory()` após uso de dados sensíveis. Não usar `string` para senhas (imutável, fica no heap). Usar `byte[]` e zerar. |
| **Timing Attacks** | `CryptographicOperations.FixedTimeEquals()` para toda comparação de hash/token. |
| **Privilege Escalation** | Mínimo de privilégios. Nunca solicitar admin/root desnecessariamente. PostgreSQL user-level service no Linux. |
| **Tampering do banco** | PostgreSQL com checksums habilitados (`initdb --data-checksums`). Detecta corrupção automaticamente. |
| **Replay Attacks (APIs)** | Nonces + timestamps em requests autenticados. Tokens de curta duração. Rejeitar requests com timestamp > 5 minutos de drift. |
| **Supply Chain (NuGet)** | Verificar nome exato do pacote e publisher antes de instalar. Usar `dotnet nuget verify` para validar assinaturas. Monitorar dependências com Dependabot ou similar. Nunca adicionar pacotes de publishers desconhecidos sem auditoria. |
| **Clipboard** | Limpar clipboard automaticamente após 30s se a aplicação colocou dados sensíveis (senhas copiadas). Registrar no log quando dados sensíveis são copiados. |
| **RSS/XML Bombs** | Limitar tamanho de payload RSS (max 5MB). Limitar profundidade de XML (max 32 níveis). `DtdProcessing.Prohibit`. Timeout de 10s para download de feeds. |
| **Fontes maliciosas** | URLs cadastradas como fonte são validadas (schema, domínio) antes de qualquer requisição. Abertura externa via `Process.Start` usa whitelist de protocolos (`https`, `http`, `file`). |
| **SSRF via fonte** | Rejeitar URLs que resolvam para endereços privados (10.x, 172.16-31.x, 192.168.x, 127.x, ::1). Validar após resolução DNS (double-check). |

### 4.3 Gerenciamento de Segredos (Cross-Platform)

```csharp
public interface IArmazenamentoSeguro
{
    Task ArmazenarAsync(string chave, byte[] valor);
    Task<byte[]?> RecuperarAsync(string chave);
    Task RemoverAsync(string chave);
}
```

**Windows — DPAPI:**
```csharp
public class ArmazenamentoSeguroWindows : IArmazenamentoSeguro
{
    private static readonly byte[] EntropiaApp =
        Encoding.UTF8.GetBytes("DiarioDeBordo.SecureStorage.v1");

    public Task ArmazenarAsync(string chave, byte[] valor)
    {
        var criptografado = ProtectedData.Protect(valor, EntropiaApp, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(ObterCaminho(chave), criptografado);
        return Task.CompletedTask;
    }

    public Task<byte[]?> RecuperarAsync(string chave)
    {
        var caminho = ObterCaminho(chave);
        if (!File.Exists(caminho)) return Task.FromResult<byte[]?>(null);
        var criptografado = File.ReadAllBytes(caminho);
        var decriptado = ProtectedData.Unprotect(criptografado, EntropiaApp, DataProtectionScope.CurrentUser);
        return Task.FromResult<byte[]?>(decriptado);
    }

    public Task RemoverAsync(string chave) { File.Delete(ObterCaminho(chave)); return Task.CompletedTask; }

    private static string ObterCaminho(string chave) =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DiarioDeBordo", "secrets", $"{chave}.enc");
}
```

**Linux — libsecret (GNOME Keyring / KWallet):**
```csharp
public class ArmazenamentoSeguroLinux : IArmazenamentoSeguro
{
    public async Task ArmazenarAsync(string chave, byte[] valor)
    {
        var base64 = Convert.ToBase64String(valor);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "secret-tool",
                ArgumentList = { "store", "--label", $"DiarioDeBordo:{chave}",
                    "application", "diariodebordo", "key", chave },
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
            throw new ArmazenamentoSeguroException(
                $"secret-tool store falhou com exit code {process.ExitCode}");
    }

    public async Task<byte[]?> RecuperarAsync(string chave)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "secret-tool",
                ArgumentList = { "lookup", "application", "diariodebordo", "key", chave },
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

    public async Task RemoverAsync(string chave)
    {
        var process = Process.Start("secret-tool",
            new[] { "clear", "application", "diariodebordo", "key", chave });
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
8. Carregar configurações globais e feature flags.
9. Apresentar tela de login.
10. Após autenticação: carregar preferências do usuário (tema, fonte, acessibilidade) e aplicar.
11. Apresentar tela inicial conforme configuração do usuário (dashboard por padrão).

Se qualquer etapa de 1 a 8 falhar: logar, apresentar mensagem ao usuário, impedir operação normal.

### 4.5 Ciclo de Vida de Chaves Criptográficas

- **Senha esquecida:** não há recuperação (by design). Documentar claramente para o usuário. O admin pode resetar a senha (gerar nova), mas não recuperar a antiga.
- **Troca de senha:** re-hash com Argon2id. O banco não precisa ser re-criptografado (a criptografia do banco é via PostgreSQL, não via senha do usuário).
- **Credenciais do banco:** senha do PostgreSQL gerada aleatoriamente na instalação (32 caracteres, `RandomNumberGenerator`), armazenada no Secure Storage.
- **Exportação criptografada:** senha mínima 12 caracteres com verificação de entropia. Derivação via Argon2id com MemorySize ≥128MB. Salt e contexto distintos da senha de login (key separation).

### 4.6 Modelo de Autorização do Domínio

O DiarioDeBordo possui um modelo de autorização RBAC com duas roles fixas e grupos customizáveis, conforme definido na Definição de Domínio v3 (seção 7).

**Roles fixas:**

| Role | Descrição | Acesso |
|---|---|---|
| Consumidor | Todo usuário autenticado | Área do consumidor — gestão e consumo de conteúdos pessoais |
| Admin | Atribuída explicitamente | Área do consumidor + Área admin — configuração e gerenciamento do sistema |

**Grupos:** O admin pode criar grupos que combinam roles e atribuí-los a usuários. O conjunto de roles é fixo; os grupos são flexíveis.

**Implementação técnica:**

```csharp
public enum Role { Consumidor, Admin }

public class GrupoDeRoles
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public IReadOnlySet<Role> Roles { get; init; } = new HashSet<Role>();
}

public interface IAutorizacaoService
{
    Task<bool> PossuiRole(Guid usuarioId, Role role);
    Task<IReadOnlySet<Role>> ObterRoles(Guid usuarioId);
}
```

**Ocultação da área admin — requisito de segurança crítico:**

Para usuários sem role Admin, a área admin **não deve existir** na interface. A implementação deve garantir:

1. **UI:** Nenhum elemento de navegação, menu, link ou indicação visual da existência da área admin é renderizado para não-admins.
2. **Rotas/navegação:** Tentativa de navegar para qualquer rota da área admin por um não-admin deve resultar em navegação para a tela inicial (ou 404 genérico), sem mensagem que indique "acesso negado a área admin" ou similar.
3. **Camada de serviço:** Todos os endpoints/serviços da área admin verificam role Admin. A resposta de erro é genérica (não distinguível de "recurso não encontrado").
4. **Logs:** Tentativas de acesso não autorizado à área admin são logadas com severidade Warning, incluindo UsuarioId, mas a mensagem de erro exibida ao usuário não revela a existência da área.

```csharp
// Middleware/guard de navegação para área admin:
public class AdminAreaGuard
{
    public async Task<bool> PodeAcessar(Guid usuarioId)
    {
        var possuiAdmin = await _autorizacaoService.PossuiRole(usuarioId, Role.Admin);
        if (!possuiAdmin)
        {
            _logger.LogWarning(
                "Tentativa de acesso à área restrita. UsuarioId={UsuarioId}", usuarioId);
        }
        return possuiAdmin;
        // Se false: navegar para tela inicial, sem mensagem de "área admin existe"
    }
}
```

**Segregação de dados:** Cada usuário possui dados completamente independentes (conteúdos, coletâneas, progresso, anotações, categorias, preferências). Nenhuma query deve retornar dados de outro usuário. Toda query que acessa dados do usuário deve incluir filtro por `usuario_id`:

```csharp
// OBRIGATÓRIO em todo repositório:
public async Task<List<Conteudo>> ListarAsync(Guid usuarioId, PaginacaoParams paginacao)
{
    return await _context.Conteudos
        .Where(c => c.UsuarioId == usuarioId)  // SEMPRE filtrar por usuário
        .OrderByDescending(c => c.DataAdicao)
        .Skip(paginacao.Offset)
        .Take(paginacao.TamanhoPagina)
        .ToListAsync();
}
```

---

## 5. Arquitetura

### 5.1 Monolito Modular

Conforme validado pela SLR de Al-Qora'n & Al-Said Ahmad (2025, Future Internet, MDPI — 15 estudos primários seguindo guidelines de Kitchenham) e por Su & Li (2024, ACM SATrends): monolito modular combina simplicidade operacional com modularidade. Drivers de adoção identificados: deployment simplificado, manutenibilidade e redução de overhead.

A estrutura de solução mapeia diretamente os bounded contexts do Mapa de Contexto v1:

```
Solution/
├── src/
│   ├── DiarioDeBordo.Desktop/        # Entry point, DI, bootstrap Velopack, inicialização segura
│   ├── DiarioDeBordo.Core/           # Interfaces, DTOs, value objects, Result<T>, eventos de domínio
│   ├── DiarioDeBordo.Infrastructure/ # DbContext, repos, HTTP clients, criptografia, secure storage
│   ├── DiarioDeBordo.UI/             # Views XAML compartilhadas, converters, recursos visuais, componentes base
│   └── Modules/
│       ├── Module.Acervo/            # BC Principal — Gestão de Conteúdo + Curadoria e Coletâneas
│       ├── Module.Agregacao/         # BC Principal — Agregação e Subscrição, feeds, agregador
│       ├── Module.Reproducao/        # BC Suporte — Reprodutor interno, abertura externa, ganchos
│       ├── Module.IntegracaoExterna/ # BC Suporte — Adaptadores de plataforma (ACL), RSS, YouTube
│       ├── Module.Busca/             # BC Suporte — Busca textual, filtros, operações em lote
│       ├── Module.Portabilidade/     # BC Suporte — Exportação/importação de dados
│       ├── Module.Identidade/        # BC Genérico — Autenticação, roles, grupos, gerenciamento de usuários
│       ├── Module.Preferencias/      # BC Genérico — Temas, fontes, acessibilidade, defaults globais
│       └── Module.Shared/            # Funcionalidades transversais — fila offline, PaginatedList<T>
├── tests/
│   ├── Tests.Unit/
│   ├── Tests.Integration/            # Inclui testes de migration (up e down)
│   ├── Tests.E2E/
│   ├── Tests.Security/               # Fuzzing, input malicioso, penetration checklist
│   ├── Tests.Performance/            # Acervos de 1k, 10k, 100k conteúdos; montagem de feeds
│   ├── Tests.Contract/               # Validação de contratos entre módulos (ex: Acervo↔Agregação)
│   └── Tests.Domain/                 # Cenários do Apêndice A da Definição de Domínio v3
├── installer/                        # Scripts de instalação/desinstalação do PostgreSQL por plataforma
├── docs/
│   ├── adr/
│   └── threat-model/
├── Directory.Build.props
├── .editorconfig
├── BannedSymbols.txt
└── DiarioDeBordo.sln
```

**Regra de dependência entre módulos:** Um módulo só pode depender de `DiarioDeBordo.Core` e de `Module.Shared`. Nenhum módulo depende diretamente de outro módulo — a comunicação entre bounded contexts é via interfaces definidas em `DiarioDeBordo.Core` e eventos via MediatR (seção 5.3).

### 5.2 Mapeamento de Bounded Contexts para Módulos

Cada bounded context do Mapa de Contexto v1 é implementado como um módulo com responsabilidades estritamente delimitadas. A tabela abaixo detalha o que cada módulo contém e o que ele **não** contém:

| Módulo | Contém | Não contém |
|---|---|---|
| **Module.Acervo** | Entidade Conteúdo (ciclo de vida completo), Coletânea (tipos, composição, proteção contra ciclos), Categorias, Relações, Fontes, Progresso, Histórico de ações, Anotações contextuais, Deduplicação, Hierarquia de autoridade de metadados. Define a *estrutura* da coletânea Subscrição. | Comportamento de montagem de feed (→ Agregação). Lógica de reprodução (→ Reprodução). Conexão com APIs externas (→ Integração Externa). |
| **Module.Agregacao** | Montagem de feed sob demanda, Agregador (visão consolidada), Persistência seletiva (decisão de quando um ItemFeed vira Conteúdo), Filtros do agregador, Comportamento offline de subscrições. | Estrutura da coletânea Subscrição (→ Acervo). Adaptadores de plataforma (→ Integração Externa). Persistência de Conteúdo (→ Acervo). |
| **Module.Reproducao** | Reprodutor interno (texto, áudio, embed de vídeo), Abertura externa, Fallback entre fontes, Ganchos, Marcação automática de progresso, Configuração por subtipo de formato. | Gestão de fontes (→ Acervo). Coletâneas (→ Acervo). |
| **Module.IntegracaoExterna** | Adaptadores por plataforma (YouTube, RSS, Amazon, etc.), Contrato padronizado de item de feed e metadados, Busca em plataformas externas, Tratamento de indisponibilidade, Parsers de importação (futuro). | Regras de negócio do agregador (→ Agregação). Regras de negócio do acervo (→ Acervo). |
| **Module.Busca** | Busca textual (full-text search), Filtros combinados, Operações em lote, Indexação otimizada, Delegação de busca externa à Integração. | Armazenamento de conteúdos (→ Acervo). |
| **Module.Portabilidade** | Exportação de dados do usuário, Exportação de configurações globais, Importação, Formato do pacote, Validação de integridade. | Estrutura dos dados exportados (consome de Acervo e Preferências). |
| **Module.Identidade** | Autenticação, Roles, Grupos, Gerenciamento de usuários, Ocultação da área admin, Sessões. | Dados de conteúdo. Preferências de interface. |
| **Module.Preferencias** | Temas, Fontes, Acessibilidade, Defaults globais, Disclosure progressivo configurável, Configurações de uso saudável (ConfiguracaoUsoSaudavel — seção 6.12). | Autenticação (→ Identidade). Dados de conteúdo. |

### 5.3 Padrões de Comunicação entre Contextos

Os padrões de comunicação seguem o Mapa de Contexto v1. A implementação técnica de cada padrão:

**Partnership — Acervo ↔ Agregação:**

Os dois contextos principais evoluem juntos no que toca à Subscrição. Tecnicamente:
- Interfaces compartilhadas em `DiarioDeBordo.Core` definem o contrato da Subscrição que ambos respeitam.
- Eventos de domínio via MediatR para comunicação assíncrona.
- Testes de contrato (`Tests.Contract`) validam que ambos os lados respeitam a interface.

```csharp
// Em DiarioDeBordo.Core — contrato compartilhado:
public interface ISubscricaoFontesProvider
{
    Task<IReadOnlyList<FonteSubscricao>> ObterFontesAsync(Guid coletaneaId, CancellationToken ct);
}

// Module.Acervo implementa (fornece as fontes configuradas na coletânea):
public class SubscricaoFontesProvider : ISubscricaoFontesProvider { /* ... */ }

// Module.Agregacao consome (para montar o feed):
public class MontadorDeFeed
{
    private readonly ISubscricaoFontesProvider _fontesProvider;
    private readonly IAdaptadorPlataforma _adaptador;
    // ...
}
```

**Momento de tradução — ItemFeed → Conteúdo:**
```csharp
// Quando o usuário interage com um item de feed (anota, dá nota, salva):
// A Agregação solicita ao Acervo a criação de um Conteúdo:
public sealed record PersistirItemFeedCommand(
    Guid UsuarioId,
    string Titulo,
    string? Descricao,
    string? UrlFonte,
    string? ThumbnailUrl,
    FormatoMidia Formato,
    Guid ColetaneaSubscricaoId
) : IRequest<Result<Guid>>; // Retorna o Id do Conteúdo criado

// Module.Acervo trata este command:
public class PersistirItemFeedHandler : IRequestHandler<PersistirItemFeedCommand, Result<Guid>>
{
    // Cria o Conteúdo, associa à coletânea, retorna Id
}
```

**Customer/Supplier — Integração Externa → Agregação:**
```csharp
// Em DiarioDeBordo.Core — contrato definido pelo Customer (Agregação):
public interface IAdaptadorPlataforma
{
    Task<Result<IReadOnlyList<ItemFeedDto>>> MontarFeedAsync(
        FonteSubscricao fonte, PaginacaoParams paginacao, CancellationToken ct);

    Task<Result<MetadadosExternosDto>> BuscarMetadadosAsync(
        string identificador, CancellationToken ct);

    bool SuportaPlataforma(string tipoPlataforma);
}

// Module.IntegracaoExterna implementa (cada adaptador):
public class AdaptadorYouTube : IAdaptadorPlataforma { /* ... */ }
public class AdaptadorRss : IAdaptadorPlataforma { /* ... */ }
```

**Customer/Supplier — Integração Externa → Acervo (busca de metadados):**
```csharp
// Em DiarioDeBordo.Core — contrato definido pelo Customer (Acervo):
public interface IMetadadosExternosProvider
{
    Task<Result<MetadadosExternosDto>> BuscarMetadadosAsync(
        string urlOuIdentificador, string? tipoPlataforma, CancellationToken ct);
}

// Module.IntegracaoExterna implementa:
// Delega ao adaptador correto conforme a plataforma da fonte.
// Module.Acervo consome:
// Apenas no momento de adição do conteúdo OU quando o usuário solicita atualização explícita.
// NUNCA em background sem solicitação (princípio de agência — seção 2.1 do domínio).
```

**Customer/Supplier — Acervo → Reprodução:**
```csharp
// Em DiarioDeBordo.Core — contrato definido pelo Customer (Reprodução):
public interface IConteudoParaReproducaoProvider
{
    Task<Result<ConteudoReproducaoDto>> ObterAsync(Guid conteudoId, CancellationToken ct);
    // Retorna: formato, fontes ordenadas por prioridade, posição atual de progresso
    // NÃO retorna: coletâneas, categorias, relações, histórico
}

// Retorno da Reprodução ao Acervo (progresso):
public sealed record ProgressoAtualizadoEvent(
    Guid ConteudoId, EstadoProgresso Estado, string? PosicaoAtual) : INotification;
```

**Customer/Supplier — Acervo → Busca:**
```csharp
// Em DiarioDeBordo.Core — contrato definido pelo Customer (Busca):
public interface IBuscaConteudoProvider
{
    Task<PaginatedList<ResultadoBuscaDto>> BuscarAsync(
        Guid usuarioId, string? termoBusca, FiltrosBusca filtros,
        PaginacaoParams paginacao, CancellationToken ct);
}

// Module.Busca implementa usando índices GIN de full-text search (seção 3.3).
// Module.Acervo é source of truth — Busca consome via queries read-only.
```

**Customer/Supplier — Busca → Integração Externa (busca em plataformas):**
```csharp
// Busca delega busca externa ao módulo de Integração via IAdaptadorPlataforma.
// O adaptador retorna ItemFeedDto — a Busca apresenta como resultado externo.
// Reutiliza o contrato IAdaptadorPlataforma já definido acima.
```

**Customer/Supplier — Acervo + Preferências → Portabilidade:**
```csharp
// Em DiarioDeBordo.Core — contratos definidos pelo Customer (Portabilidade):
public interface IDadosExportaveisProvider
{
    Task<DadosExportaveis> ExportarAsync(Guid usuarioId, CancellationToken ct);
    // Exporta conteúdos, coletâneas, categorias, relações, progresso, histórico
}

public interface IPreferenciasExportaveisProvider
{
    Task<PreferenciasExportaveis> ExportarAsync(Guid usuarioId, CancellationToken ct);
    // Exporta temas, fontes, acessibilidade, configurações de uso saudável
}

// Module.Acervo implementa IDadosExportaveisProvider.
// Module.Preferencias implementa IPreferenciasExportaveisProvider.
// Module.Portabilidade consome ambos para montar o pacote de exportação.
```

**Open Host Service — Identidade e Preferências:**
```csharp
// Em DiarioDeBordo.Core — serviços padronizados consumidos por todos:
public interface IUsuarioAutenticadoProvider
{
    Guid? UsuarioIdAtual { get; }
    Task<IReadOnlySet<Role>> RolesAtuais { get; }
}

public interface IPreferenciasProvider
{
    Task<Tema> ObterTemaAsync(Guid usuarioId);
    Task<ConfiguracaoFonte> ObterFonteAsync(Guid usuarioId);
    Task<ConfiguracaoAcessibilidade> ObterAcessibilidadeAsync(Guid usuarioId);
    Task<int> ObterItensPorPaginaAsync(Guid usuarioId);
    Task<bool> DisclosureProgressivoAtivo(Guid usuarioId);
}
```

### 5.4 Uso Saudável como Restrição Arquitetural

Uso Saudável é uma preocupação transversal (Mapa de Domínio v1, seção 2) que impõe restrições sobre o design de todos os bounded contexts. Diferentemente de funcionalidades, que se implementam em módulos, Uso Saudável se materializa como **restrições verificáveis** em revisão de código, testes e análise estática.

**Restrições invioláveis — enforcement técnico:**

| Padrão proibido | Enforcement técnico |
|---|---|
| **Scroll infinito** | Nenhum componente de lista/feed aceita renderização sem `PaginatedList<T>`. Componentes de lista sem paginação falham em testes. |
| **Autoplay entre conteúdos** | O reprodutor (Module.Reproducao) não implementa navegação automática para o próximo item. Ao concluir reprodução, retorna ao estado parado. |
| **Algoritmo de ranqueamento** | Feeds e listas usam exclusivamente ordenação explícita (cronológica, alfabética, manual). Nenhum serviço calcula "relevância" ou "engajamento". |
| **Métricas sociais** | Module.IntegracaoExterna não captura nem expõe likes, views, comentários de terceiros. O `ItemFeedDto` não possui campos para métricas sociais. |
| **Notificações não solicitadas** | O sistema não envia notificações push. Lembretes de tempo de uso são opt-in (configurados pelo usuário, desativados por padrão). |

**Alerta de configuração potencialmente não saudável:**

Quando o usuário configurar mais de 100 itens por página, o sistema exibe um alerta informativo (não bloqueante) explicando que a paginação alta se aproxima do comportamento de scroll infinito. O alerta informa; não impede.

```csharp
public Result<ConfiguracaoUsuario> AlterarItensPorPagina(Guid usuarioId, int novoValor)
{
    if (novoValor < 1 || novoValor > 500)
        return Result<ConfiguracaoUsuario>.Failure(Erros.ValorForaDaFaixa);

    var resultado = _preferenciasRepo.Atualizar(usuarioId, "itens_por_pagina", novoValor);

    if (novoValor > 100)
    {
        resultado = resultado with {
            Alerta = new AlertaUsoSaudavel(
                "Paginação com muitos itens por página se aproxima do comportamento de scroll infinito. "
                + "Considere um valor menor para uma experiência mais saudável.")
        };
    }

    return resultado;
}
```

### 5.5 Registro vs. Visão — Implementação Técnica

O domínio distingue entre Registro (dado persistido) e Visão (dado montado sob demanda, não armazenado) — Definição de Domínio v3, seção 3.1. Esta distinção tem impacto técnico direto:

**Registros (entidades persistidas no banco):**
- `Conteudo`, `Coletanea`, `Categoria`, `Relacao`, `Fonte`, `Progresso`, `Anotacao`, `Usuario`, `ConfiguracaoUsuario`, `ConfiguracaoGlobal`.
- Gerenciados via EF Core, com migrations, constraints e índices.
- Disponíveis offline.

**Visões (computadas sob demanda, nunca armazenadas):**
- `FeedDeSubscricao` — itens montados ao consultar fontes externas.
- `AgregadorConsolidado` — combinação de múltiplos feeds.
- `ResultadoBusca` — projeção otimizada para listagem.
- `Dashboard` — seções montadas a partir de queries sobre registros.

**Implementação técnica — Visões nunca viram entidades EF Core:**

```csharp
// CORRETO — Visão como DTO/record, sem DbSet, sem tabela:
public sealed record ItemFeedDto(
    string Titulo,
    string? Descricao,
    string? UrlOrigem,
    string? ThumbnailUrl,
    DateTimeOffset? DataPublicacao,
    string IdentificadorFonte,
    string PlataformaOrigem
);

// A lista de ItemFeedDto é computada e descartada. Nunca persistida.
// Se o usuário interagir: o ItemFeedDto é traduzido em Conteudo (registro).

// PROIBIDO — Nunca criar DbSet ou tabela para visões:
// public DbSet<ItemFeed> ItensFeed { get; set; } // NUNCA
```

### 5.6 Anti-Corruption Layer — Integração Externa

O Module.IntegracaoExterna funciona como Anti-Corruption Layer (ACL) conforme definido no Mapa de Contexto v1 (seção 2.2). Ele protege os contextos internos (Agregação, Acervo, Busca) da instabilidade e diversidade das APIs externas.

**Princípio:** Quando uma plataforma externa muda sua API, apenas o adaptador correspondente no Module.IntegracaoExterna é alterado. Nenhum outro módulo é afetado.

**Implementação:**

```csharp
// Contrato padronizado — definido em DiarioDeBordo.Core:
public sealed record ItemFeedDto(
    string Titulo,
    string? Descricao,
    string? UrlOrigem,
    string? ThumbnailUrl,
    DateTimeOffset? DataPublicacao,
    string IdentificadorFonte,
    string PlataformaOrigem
);

public sealed record MetadadosExternosDto(
    string? Titulo,
    string? Descricao,
    string? ThumbnailUrl,
    TimeSpan? Duracao,
    string? Criador,
    DateTimeOffset? DataPublicacao
);

// Cada adaptador traduz o formato da plataforma para o contrato padronizado:
public class AdaptadorRss : IAdaptadorPlataforma
{
    public async Task<Result<IReadOnlyList<ItemFeedDto>>> MontarFeedAsync(
        FonteSubscricao fonte, PaginacaoParams paginacao, CancellationToken ct)
    {
        // 1. Baixar XML do feed RSS
        // 2. Parsear com DtdProcessing.Prohibit (segurança)
        // 3. Traduzir cada <item> para ItemFeedDto
        // 4. Aplicar paginação
        // 5. Retornar lista padronizada — o Agregação não sabe que veio de RSS
    }

    public bool SuportaPlataforma(string tipo) => tipo == "rss";
}

// Registry de adaptadores:
public class RegistroAdaptadores
{
    private readonly IEnumerable<IAdaptadorPlataforma> _adaptadores;

    public IAdaptadorPlataforma? ObterPara(string tipoPlataforma) =>
        _adaptadores.FirstOrDefault(a => a.SuportaPlataforma(tipoPlataforma));
}
```

**Tratamento de indisponibilidade:**
```csharp
public async Task<Result<IReadOnlyList<ItemFeedDto>>> MontarFeedAsync(
    FonteSubscricao fonte, PaginacaoParams paginacao, CancellationToken ct)
{
    try
    {
        // Tentativa com timeout (Polly — seção 10)
        return await _pipeline.ExecuteAsync(async token =>
            await ConsultarFonteAsync(fonte, paginacao, token), ct);
    }
    catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
    {
        _logger.LogWarning(ex,
            "Fonte indisponível. Plataforma={Plataforma} Fonte={Fonte}",
            fonte.TipoPlataforma, fonte.Identificador);

        return Result<IReadOnlyList<ItemFeedDto>>.Failure(
            new Erro("FONTE_INDISPONIVEL",
                $"Não foi possível conectar à fonte. Tente novamente mais tarde."));
    }
}
```

### 5.7 Princípios SOLID

Princípios propostos por Martin (2000), validados empiricamente por:
- Singh & Hassan (2015, IJSER) — estudo empírico usando métricas CK demonstrando redução de coupling e melhoria de qualidade em designs que seguem SOLID.
- Cabral et al. (2024, CAIN/IEEE-ACM) — experimento controlado com 100 data scientists demonstrando evidência estatisticamente significativa de que SOLID melhora compreensão de código.
- Turan & Tanrıöver (2018, AJIT-e) — avaliação experimental mapeando SOLID para sub-características de manutenibilidade ISO/IEC 9126 via VS Code Metrics.
- Simonetti et al. (2024, Journal of Systems and Software, Elsevier) — estudo industrial na ASML aplicando SOLID na refatoração de código legado com redução mensurável de dívida técnica.

**S — Single Responsibility:** cada classe tem uma única razão para mudar. Exemplo: `ConteudoService` gerencia o ciclo de vida de Conteúdo; `ColetaneaService` gerencia coletâneas; `MontadorDeFeed` monta feeds. Não existe uma classe "fazTudo".
**O — Open/Closed:** extensão via composição e Strategy, não modificação. Novos adaptadores de plataforma implementam `IAdaptadorPlataforma` sem alterar código existente.
**L — Liskov Substitution:** subtipos substituíveis sem quebrar comportamento. Todos os adaptadores (`AdaptadorRss`, `AdaptadorYouTube`) são intercambiáveis onde `IAdaptadorPlataforma` é esperado.
**I — Interface Segregation:** interfaces pequenas e coesas. `IConteudoParaReproducaoProvider` expõe apenas o que a Reprodução precisa, não toda a entidade Conteúdo.
**D — Dependency Inversion:** módulos dependem de abstrações em `DiarioDeBordo.Core`, não de implementações.

```csharp
// DI — toda dependência externa injetada:
public class MontadorDeFeed
{
    private readonly ISubscricaoFontesProvider _fontesProvider;
    private readonly IAdaptadorPlataforma _adaptador;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<MontadorDeFeed> _logger;

    public MontadorDeFeed(
        ISubscricaoFontesProvider f, IAdaptadorPlataforma a,
        TimeProvider t, ILogger<MontadorDeFeed> l)
    {
        _fontesProvider = f; _adaptador = a; _timeProvider = t; _logger = l;
    }
}
```

### 5.8 Result Pattern

```csharp
public sealed record Result<T>
{
    public T? Value { get; }
    public Erro? Error { get; }
    public AlertaUsoSaudavel? Alerta { get; init; }
    public bool IsSuccess => Error is null;
    private Result(T value) => Value = value;
    private Result(Erro error) => Error = error;
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Erro error) => new(error);
}

public sealed record Erro(string Codigo, string Mensagem);

public sealed record AlertaUsoSaudavel(string Mensagem);
```

### 5.9 Internacionalização (i18n)

**Idioma do sistema:** Português brasileiro (pt-BR), conforme Definição de Domínio v3 (seção 9.6).

- **Interface, mensagens e documentação:** tudo em pt-BR.
- Nenhuma string visível ao usuário hardcoded no código. Todas em arquivos `.resx` com cultura `pt-BR` como padrão.
- **Código-fonte:** nomes de classes, métodos e variáveis em português para entidades de domínio (ex: `Conteudo`, `Coletanea`, `FormatoMidia`). Termos técnicos do framework/plataforma em inglês (ex: `async`, `Task`, `CancellationToken`). Nomes de interfaces com prefixo `I` em inglês conforme convenção .NET.
- Datas no banco em UTC; converter para local na apresentação via `CultureInfo("pt-BR")`.
- **Preparação para i18n futura:** a arquitetura via `.resx` permite adicionar outros idiomas no futuro sem refatoração, mas pt-BR é o único idioma suportado no escopo atual.

### 5.10 Migração de Dados entre Versões

1. Antes de aplicar migrations: backup via `pg_dump`.
2. Aplicar migrations em transação (PostgreSQL suporta DDL transacional).
3. Se falhar: rollback da transação (o banco permanece intacto).
4. Testar migrations up e down na suite de integração.
5. EF Core mantém histórico em `__EFMigrationsHistory`.

### 5.11 Concorrência UI Thread vs Background Services

PostgreSQL com MVCC resolve a contença de escritas simultâneas nativamente. Entretanto, regras de código:
- Nunca fazer queries na UI thread. Sempre via `Task.Run` ou async.
- Usar connection pooling do Npgsql (configurado automaticamente pelo EF Core).
- `CancellationToken` em toda cadeia de operações canceláveis.
- Para operações longas (montagem de feed, importação de dados): exibir progresso na UI via `IProgress<T>`.

---

## 6. Padrões Técnicos do Domínio

Esta seção traduz as regras de negócio e invariantes da Definição de Domínio v3 em padrões técnicos implementáveis. Cada subseção referencia a seção correspondente no documento de domínio.

### 6.1 Entidade Conteúdo e Dois Eixos de Classificação

Referência: Definição de Domínio v3, seção 4.1.

Conteúdo é classificado em dois eixos independentes e ortogonais:

```csharp
public enum FormatoMidia
{
    Audio,
    Texto,
    Video,
    Imagem,
    Interativo,
    Misto,
    Nenhum
}

public enum PapelEstrutural
{
    Item,
    Coletanea
}

public enum TipoColetanea
{
    Guiada,
    Miscelanea,
    Subscricao
}

public class Conteudo
{
    public Guid Id { get; init; }
    public Guid UsuarioId { get; init; }

    // Único campo obrigatório (Economia Cognitiva — seção 2.3 do domínio):
    public string Titulo { get; set; } = string.Empty;

    // Eixo 1 — formato de mídia:
    public FormatoMidia Formato { get; set; } = FormatoMidia.Nenhum;
    public string? SubtipoFormato { get; set; } // Livre, ex: "música", "artigo", "filme"

    // Eixo 2 — papel estrutural:
    public PapelEstrutural Papel { get; set; } = PapelEstrutural.Item;
    public TipoColetanea? TipoColetanea { get; set; } // Somente quando Papel == Coletanea

    // Atributos opcionais:
    public string? Descricao { get; set; }
    public string? Anotacoes { get; set; }
    public decimal? Nota { get; set; } // 0 a 10
    public Classificacao? Classificacao { get; set; }
    public Progresso Progresso { get; set; } = new();

    // Relações:
    public ICollection<ConteudoColetanea> Coletaneas { get; set; } = new List<ConteudoColetanea>();
    public ICollection<Fonte> Fontes { get; set; } = new List<Fonte>();
    public ICollection<ConteudoCategoria> Categorias { get; set; } = new List<ConteudoCategoria>();
    public ICollection<RelacaoConteudo> Relacoes { get; set; } = new List<RelacaoConteudo>();
    public ICollection<ImagemConteudo> Imagens { get; set; } = new List<ImagemConteudo>();
    public ICollection<HistoricoAcao> Historico { get; set; } = new List<HistoricoAcao>();

    public DateTimeOffset DataAdicao { get; init; }
}

public enum Classificacao { Gostei, NaoGostei, Inconclusivo }
```

**Invariantes:**
- `Titulo` nunca é nulo ou vazio.
- `TipoColetanea` deve ser nulo quando `Papel == Item` e obrigatório quando `Papel == Coletanea`.
- `Nota` deve estar no intervalo [0, 10] quando presente.

**Entidade Fonte (Definição de Domínio v3, seção 4.3):**

```csharp
public enum TipoFonte { Url, ArquivoLocal, Rss, IdentificadorPlataforma }

public class Fonte
{
    public Guid Id { get; init; }
    public Guid ConteudoId { get; init; }
    public TipoFonte Tipo { get; set; }
    public string Valor { get; set; } = string.Empty; // URL, caminho, feed URL, @ de criador
    public int Prioridade { get; set; } // 1 = maior prioridade. Fallback em ordem crescente.
    public string? TipoPlataforma { get; set; } // "youtube", "rss", "instagram", "amazon"
}
```

Um conteúdo pode ter zero ou mais fontes. Quando a fonte de maior prioridade está indisponível (URL offline, arquivo deletado, HD desconectado), o sistema tenta a próxima na lista. Conteúdo sem fonte é caso válido (ex: caderno físico).

**Entidade Gancho (Definição de Domínio v3, seção 5.3):**

```csharp
public class Gancho
{
    public Guid Id { get; init; }
    public Guid ConteudoId { get; init; }
    public string Rotulo { get; set; } = string.Empty; // "assistir a partir daqui", "cena importante"
    public string Posicao { get; set; } = string.Empty; // "12:30" (vídeo/áudio), "p.45" (texto), texto livre
    public DateTimeOffset DataCriacao { get; init; }
}
```

Ganchos são marcações em partes específicas do conteúdo. A entidade pertence ao Acervo (é um registro persistido); a Reprodução cria e navega ganchos durante a reprodução.

**Entidade HistoricoAcao (Definição de Domínio v3, seção 4.4.3):**

```csharp
public enum TipoAcao
{
    Criacao, EdicaoMetadado, AdicaoColetanea, RemocaoColetanea,
    AlteracaoProgresso, AdicaoCategoria, RemocaoCategoria,
    AdicaoRelacao, RemocaoRelacao, AdicaoFonte, RemocaoFonte,
    AdicaoGancho, RemocaoGancho
}

public class HistoricoAcao
{
    public Guid Id { get; init; }
    public Guid ConteudoId { get; init; }
    public TipoAcao Tipo { get; init; }
    public DateTimeOffset DataOcorrencia { get; init; }
    public string? CampoAlterado { get; init; } // Ex: "titulo", "nota", "progresso"
    public string? ValorAnterior { get; init; } // JSON serializado do valor antes da alteração
    public string? ValorNovo { get; init; }     // JSON serializado do valor após a alteração
}
```

O delta antes/depois permite ao usuário entender o que mudou e, em caso de erro, identificar como reverter. O histórico também suporta o requisito de continuidade de uso interrompido (Definição de Domínio v3, seção 9.2).

**Entidade ImagemConteudo (Definição de Domínio v3, seção 4.7):**

```csharp
public class ImagemConteudo
{
    public Guid Id { get; init; }
    public Guid ConteudoId { get; init; }
    public string CaminhoOuUrl { get; set; } = string.Empty;
    public bool Principal { get; set; } // Uma imagem é marcada como principal quando houver ≥1
    public OrigemMetadado Origem { get; set; } // Manual, AutomaticoFonte, AutomaticoExterno
}
```

**Limites de imagem:** Máximo 20 imagens por conteúdo. Máximo 10MB por imagem. Imagens acima do limite são rejeitadas com mensagem informativa. Esses limites são pragmáticos (sem embasamento experimental) e devem ser validados com uso real.

### 6.2 Coletâneas e Proteção contra Ciclos

Referência: Definição de Domínio v3, seção 4.2.

Uma coletânea pode conter itens e outras coletâneas (composição). A restrição de ciclo é uma invariante inviolável: uma coletânea não pode conter a si mesma, direta ou indiretamente.

**Fundamentação:** A detecção de ciclos em grafos dirigidos é um problema classicamente resolvido por busca em profundidade (DFS). Tarjan (1972, *SIAM Journal on Computing*) formalizou o algoritmo de DFS com classificação de arestas que identifica back edges — indicadoras de ciclo — em tempo linear O(V+E).

**Implementação — validação no momento da adição:**

```csharp
public class ColetaneaService
{
    public async Task<Result<ConteudoColetanea>> AdicionarItemAsync(
        Guid coletaneaId, Guid conteudoId, CancellationToken ct)
    {
        var conteudo = await _conteudoRepo.ObterAsync(conteudoId, ct);
        if (conteudo is null)
            return Result<ConteudoColetanea>.Failure(Erros.ConteudoNaoEncontrado);

        // Se o conteúdo sendo adicionado é uma coletânea, verificar ciclo:
        if (conteudo.Papel == PapelEstrutural.Coletanea)
        {
            var temCiclo = await DetectarCicloAsync(coletaneaId, conteudoId, ct);
            if (temCiclo)
                return Result<ConteudoColetanea>.Failure(
                    new Erro("CICLO_DETECTADO",
                        "Adicionar esta coletânea criaria uma referência circular."));
        }

        // Adicionar...
    }

    /// <summary>
    /// Verifica se adicionar <paramref name="conteudoFilhoId"/> à coletânea
    /// <paramref name="coletaneaPaiId"/> criaria um ciclo.
    /// Usa CTE recursivo no PostgreSQL para percorrer a árvore de ancestralidade.
    /// </summary>
    private async Task<bool> DetectarCicloAsync(
        Guid coletaneaPaiId, Guid conteudoFilhoId, CancellationToken ct)
    {
        // O conteúdo-filho contém (direta ou indiretamente) a coletânea-pai?
        // Se sim, adicionar criaria ciclo.
        const string sql = @"
            WITH RECURSIVE ancestrais AS (
                SELECT conteudo_id
                FROM conteudo_coletanea
                WHERE coletanea_id = @filhoId

                UNION ALL

                SELECT cc.conteudo_id
                FROM conteudo_coletanea cc
                INNER JOIN ancestrais a ON cc.coletanea_id = a.conteudo_id
            )
            SELECT EXISTS (SELECT 1 FROM ancestrais WHERE conteudo_id = @paiId)";

        return await _connection.ExecuteScalarAsync<bool>(
            sql, new { filhoId = conteudoFilhoId, paiId = coletaneaPaiId });
    }
}
```

**Anotação contextual:**

```csharp
// A anotação pertence à relação conteúdo-coletânea, não ao conteúdo nem à coletânea:
public class ConteudoColetanea
{
    public Guid ConteudoId { get; init; }
    public Guid ColetaneaId { get; init; }
    public string? AnotacaoContextual { get; set; } // Ex: "assistir em dia chuvoso"
    public int? Ordenacao { get; set; } // Para coletâneas com ordenação habilitada
    public DateTimeOffset DataAdicao { get; init; }
}
```

**Acompanhamento sequencial (Definição de Domínio v3, seção 4.2):**

Quando habilitado em uma coletânea, o sistema rastreia qual é o próximo item recomendado (baseado na ordenação e no progresso dos itens). O acompanhamento é informativo, não bloqueante — o usuário pode consumir qualquer item fora de ordem (princípio de agência, seção 2.1 do domínio). A interface pode destacar o próximo recomendado, exibir quantos itens faltam, e indicar quais já foram concluídos.

```csharp
public class ConfiguracaoColetanea
{
    public bool OrdenacaoHabilitada { get; set; } // Guiada: true por padrão; Miscelânea: false
    public bool AcompanhamentoSequencial { get; set; } // Ponteiro de progresso na coletânea
}
```

**Dualidade da Subscrição — fronteira Acervo/Agregação (Mapa de Domínio v1, seção 8.3):**

A coletânea tipo Subscrição pertence simultaneamente ao Acervo (estrutura) e à Agregação (comportamento):
- **Acervo** define: quais fontes a subscrição acompanha, configurações comportamentais, proteção contra ciclos (se a subscrição estiver dentro de outra coletânea), associação de conteúdos persistidos.
- **Agregação** define: montagem de feed sob demanda, decisão de visão vs. registro, filtros do agregador, comportamento offline.
- A interface `ISubscricaoFontesProvider` (seção 5.3) é o ponto de integração: Acervo fornece as fontes, Agregação monta o feed.

### 6.3 Hierarquia de Autoridade de Metadados

Referência: Definição de Domínio v3, seção 3.2.

Metadados podem vir de três origens com precedência definida: Manual > Automático de fonte > Automático de fontes externas.

```csharp
public enum OrigemMetadado
{
    Manual,             // Inserido pelo usuário — SEMPRE tem precedência
    AutomaticoFonte,    // Buscado da fonte do conteúdo (thumbnail YouTube, etc.)
    AutomaticoExterno   // Buscado de fontes de metadados externas (APIs de capas, etc.)
}

public class MetadadoConteudo
{
    public string Campo { get; init; } = string.Empty;  // "titulo", "descricao", "thumbnail"
    public string Valor { get; set; } = string.Empty;
    public OrigemMetadado Origem { get; set; }
    public DateTimeOffset DataAtualizacao { get; set; }
}

public class ResolvedorMetadados
{
    /// <summary>
    /// Resolve o valor de um metadado conforme a hierarquia de autoridade.
    /// Se o usuário editou manualmente, o valor manual prevalece.
    /// Metadados automáticos são sugestão, nunca imposição.
    /// </summary>
    public string? Resolver(IReadOnlyList<MetadadoConteudo> metadados, string campo)
    {
        return metadados
            .Where(m => m.Campo == campo)
            .OrderBy(m => m.Origem) // Manual(0) < AutomaticoFonte(1) < AutomaticoExterno(2)
            .FirstOrDefault()?.Valor;
    }
}
```

**Regras:**
- Metadados automáticos são buscados apenas (1) quando o conteúdo é adicionado ao sistema e (2) quando o usuário solicita atualização explicitamente. Nunca em background sem solicitação (princípio de agência — seção 2.1 do domínio).
- **Quando um campo já possui valor manual e chega um metadado automático novo:** o automático é descartado silenciosamente. O sistema não oferece sugestão de substituição nem alerta — o manual é soberano. Isso evita interferir na agência do usuário.
- **Quando um campo possui valor automático e o usuário o edita:** a edição é salva como `OrigemMetadado.Manual` e passa a ter precedência permanente sobre qualquer automático futuro para aquele campo.

### 6.4 Deduplicação de Conteúdo

Referência: Definição de Domínio v3, seção 4.4.2.

A regra de domínio é: um conteúdo = um registro. Duplicação é indesejada e o sistema deve ativamente tentar evitá-la.

**Análise exploratória:** A detecção de registros duplicados é um problema estudado extensivamente na literatura. Elmagarmid, Ipeirotis & Verykios (2007, *IEEE Transactions on Knowledge and Data Engineering*) realizaram uma survey abrangente identificando técnicas de blocking (reduzir o espaço de comparação), similaridade de strings (Jaro-Winkler, edit distance), e machine learning para classificação de pares.

**Análise explanatória — aplicação ao DiarioDeBordo:**

No contexto deste sistema, a deduplicação opera sobre conteúdos que podem aparecer em múltiplas subscrições. As heurísticas, em ordem de confiança:

1. **URL de fonte idêntica:** Se um conteúdo já possui uma fonte com a mesma URL normalizada, é o mesmo conteúdo. Confiança alta.
2. **Identificador de plataforma:** Mesmo video ID no YouTube, mesmo GUID no RSS. Confiança alta.
3. **Título + criador (normalizado):** Após normalização (lowercase, remoção de acentos, trim). Confiança média — requer confirmação do usuário para falsos positivos.

```csharp
public class DeduplicadorConteudo
{
    public async Task<Conteudo?> BuscarDuplicataAsync(
        Guid usuarioId, ItemFeedDto item, CancellationToken ct)
    {
        // 1. Busca por URL de fonte (confiança alta):
        if (!string.IsNullOrEmpty(item.UrlOrigem))
        {
            var porUrl = await _conteudoRepo.BuscarPorUrlFonteAsync(
                usuarioId, NormalizarUrl(item.UrlOrigem), ct);
            if (porUrl is not null) return porUrl;
        }

        // 2. Busca por identificador de plataforma (confiança alta):
        if (!string.IsNullOrEmpty(item.IdentificadorFonte))
        {
            var porId = await _conteudoRepo.BuscarPorIdentificadorFonteAsync(
                usuarioId, item.PlataformaOrigem, item.IdentificadorFonte, ct);
            if (porId is not null) return porId;
        }

        // 3. Busca por título normalizado (confiança média — retornar como sugestão):
        // O usuário decide se é duplicata ou não.
        return null; // Sugestões são tratadas na UI, não forçadas
    }

    private static string NormalizarUrl(string url) =>
        url.Trim().TrimEnd('/').ToLowerInvariant()
           .Replace("http://", "https://")
           .Replace("www.", "");
}
```

**Correção manual:** O usuário pode unir registros duplicados (merge) ou separar registros incorretamente unidos (split), conforme princípio de agência (seção 2.1 do domínio).

### 6.5 Persistência Seletiva (ItemFeed → Conteúdo)

Referência: Definição de Domínio v3, seção 4.2 (Subscrição); Mapa de Contexto v1, seção 1.2.

A distinção entre ItemFeed (efêmero) e Conteúdo (persistido) é a fronteira mais importante do sistema. Um ItemFeed só vira Conteúdo quando o usuário realiza uma das seguintes interações (conforme Mapa de Contexto v1, seção 2.1):

- **Salvar/favoritar** o item explicitamente.
- **Registrar progresso** (iniciar consumo, marcar posição).
- **Fazer anotação** sobre o item.
- **Dar nota ou classificação** (gostei/não gostei/inconclusivo).

Nenhuma outra ação (visualizar o card, rolar a lista, abrir e fechar detalhes) dispara persistência.

**Apresentação de cards no agregador e feeds (conforme Definição de Domínio v3, seção 5.2):**
- Quando metadados estão disponíveis: card com imagem, título e breve descrição.
- Quando apenas URL está disponível sem metadados: apresentar o link com a URL visível.
- Quando reprodução interna é viável: card leva ao reprodutor.
- Quando apenas reprodução externa é possível: redireciona para conteúdo externo.

**Filtros do agregador (conforme Definição de Domínio v3, seção 5.2):**
- Por criador/fonte.
- Esconder conteúdos já consumidos (progresso = concluído).
- Esconder conteúdos cujo título ou descrição contenham palavras-chave definidas pelo usuário.
- Ordenação cronológica (mais recente primeiro, por padrão).

```csharp
// O ItemFeedDto é um record imutável — nunca possui Id de banco, nunca é rastreado pelo EF Core:
public sealed record ItemFeedDto(
    string Titulo,
    string? Descricao,
    string? UrlOrigem,
    string? ThumbnailUrl,
    DateTimeOffset? DataPublicacao,
    string IdentificadorFonte,
    string PlataformaOrigem
);

// Transição: apenas quando o usuário realiza uma ação que exige persistência:
public class AgregacaoService
{
    public async Task<Result<Guid>> SalvarItemAsync(
        Guid usuarioId, ItemFeedDto item, Guid coletaneaSubscricaoId, CancellationToken ct)
    {
        // 1. Verificar deduplicação:
        var existente = await _deduplicador.BuscarDuplicataAsync(usuarioId, item, ct);
        if (existente is not null)
        {
            // Conteúdo já existe — associar à coletânea se ainda não estiver:
            await _coletaneaService.AssociarSeNecessarioAsync(
                coletaneaSubscricaoId, existente.Id, ct);
            return Result<Guid>.Success(existente.Id);
        }

        // 2. Criar novo Conteúdo a partir do ItemFeed:
        var command = new PersistirItemFeedCommand(
            usuarioId, item.Titulo, item.Descricao,
            item.UrlOrigem, item.ThumbnailUrl,
            InferirFormato(item.PlataformaOrigem),
            coletaneaSubscricaoId);

        return await _mediator.Send(command, ct);
    }
}
```

### 6.6 Contrato de Adaptador de Plataforma

Referência: Mapa de Contexto v1, seção 2.2; Mapa de Domínio v1, seção 4.4.

Cada plataforma externa é encapsulada em um adaptador que implementa o contrato padronizado. Novos adaptadores podem ser adicionados sem alterar a lógica de negócio do Agregação nem do Acervo.

```csharp
public interface IAdaptadorPlataforma
{
    /// <summary>Identifica quais tipos de plataforma este adaptador suporta.</summary>
    bool SuportaPlataforma(string tipoPlataforma);

    /// <summary>Monta feed: descobre quais conteúdos existem na fonte.</summary>
    Task<Result<IReadOnlyList<ItemFeedDto>>> MontarFeedAsync(
        FonteSubscricao fonte, PaginacaoParams paginacao, CancellationToken ct);

    /// <summary>Busca metadados de um conteúdo específico.</summary>
    Task<Result<MetadadosExternosDto>> BuscarMetadadosAsync(
        string identificador, CancellationToken ct);

    /// <summary>Busca conteúdos na plataforma por termo (quando suportado).</summary>
    Task<Result<IReadOnlyList<ItemFeedDto>>> BuscarAsync(
        string termo, PaginacaoParams paginacao, CancellationToken ct);
}

// Prioridade de implementação (conforme Plano de Implementação v3, Etapa 4):
// 1. AdaptadorRss — padrão aberto, mais estável
// 2. AdaptadorYouTube — mais documentado
// 3. AdaptadorAmazon — listas de desejos (futuro)
// 4. AdaptadorInstagram — mais restritivo (futuro)
```

**Validação de fontes no cadastro de subscrição:**

```csharp
// Quando o usuário cadastra uma fonte, o adaptador valida:
public interface IValidadorFonte
{
    Task<Result<FonteValidada>> ValidarAsync(string identificador, string tipoPlataforma, CancellationToken ct);
    // Verifica: URL acessível, formato reconhecido, canal/feed existe
}
```

### 6.7 Paginação Obrigatória

Referência: Definição de Domínio v3, seção 6.2.

Toda apresentação de elementos em feeds, listas e tabelas usa paginação. Sem exceção.

```csharp
public sealed record PaginacaoParams(int Pagina = 1, int TamanhoPagina = 20)
{
    public int Offset => (Pagina - 1) * TamanhoPagina;

    // Validação:
    public static PaginacaoParams Criar(int pagina, int tamanhoPagina)
    {
        if (pagina < 1) pagina = 1;
        if (tamanhoPagina < 1) tamanhoPagina = 1;
        if (tamanhoPagina > 500) tamanhoPagina = 500; // Limite máximo absoluto
        return new PaginacaoParams(pagina, tamanhoPagina);
    }
}

public sealed record PaginatedList<T>(
    IReadOnlyList<T> Itens,
    int TotalItens,
    int PaginaAtual,
    int TamanhoPagina
)
{
    public int TotalPaginas => (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);
    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;
}
```

**Regra de enforcement:** Toda query que retorna lista de entidades **deve** retornar `PaginatedList<T>`. Repositórios que retornam `List<T>` ou `IEnumerable<T>` para dados apresentáveis ao usuário são considerados incorretos e devem ser corrigidos em revisão de código.

**Valor padrão:** 20 itens por página (configurável pelo admin nos defaults globais, sobrescrevível pelo usuário). Nota de transparência: conforme Definição de Domínio v3 (seção 6.2), este valor não deriva de pesquisa experimental peer-reviewed e deve ser validado com uso real.

### 6.8 Categorias e Relações Bidirecionais

Referência: Definição de Domínio v3, seções 4.5 e 4.6.

**Categorias — tags livres com não-duplicação:**

```csharp
public class CategoriaService
{
    /// <summary>
    /// Autocompletar: retorna categorias existentes do usuário que contenham o texto digitado.
    /// Usa busca case-insensitive e accent-insensitive (unaccent do PostgreSQL).
    /// </summary>
    public async Task<IReadOnlyList<Categoria>> AutocompletarAsync(
        Guid usuarioId, string textoParcial, CancellationToken ct)
    {
        return await _context.Categorias
            .Where(c => c.UsuarioId == usuarioId
                && EF.Functions.ILike(
                    EF.Functions.Unaccent(c.Nome),
                    $"%{EF.Functions.Unaccent(textoParcial)}%"))
            .OrderBy(c => c.Nome)
            .Take(10)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Ao adicionar categoria: se já existe com mesmo nome (case-insensitive),
    /// reutiliza a existente. Constraint UNIQUE no banco garante não-duplicação.
    /// </summary>
    public async Task<Result<Categoria>> AdicionarOuReutilizarAsync(
        Guid usuarioId, string nome, CancellationToken ct)
    {
        var existente = await _context.Categorias
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId
                && c.Nome.ToLower() == nome.Trim().ToLower(), ct);

        if (existente is not null)
            return Result<Categoria>.Success(existente);

        var nova = new Categoria { UsuarioId = usuarioId, Nome = nome.Trim() };
        _context.Categorias.Add(nova);
        return Result<Categoria>.Success(nova);
    }
}
```

**Relações bidirecionais entre conteúdos:**

```csharp
public class RelacaoConteudo
{
    public Guid ConteudoOrigemId { get; init; }
    public Guid ConteudoDestinoId { get; init; }
    public string TipoRelacao { get; set; } = string.Empty; // "é sequência de", "mesmo universo"
    // Tipos de relação seguem o mesmo padrão de autocompletar das categorias.
}

// Na tela do Conteúdo A (origem): mostra B na seção "Conteúdos relacionados"
// Na tela do Conteúdo B (destino): mostra A na seção "Citado por"
// Ambas as direções são derivadas da mesma relação — sem duplicação de registro.
```

**Restrições de relação:**
- Auto-referência é proibida: `ConteudoOrigemId != ConteudoDestinoId` (constraint no banco).
- Relações bidirecionais (A→B e B→A) são permitidas — representam diferentes perspectivas. Mas relação duplicada exata (mesmo par de IDs e mesmo tipo) é proibida (constraint UNIQUE).
- Tipos de relação são livres (não há lista predefinida). Sugestões vêm do histórico do próprio usuário via autocompletar, respeitando a agência (seção 2.1 do domínio).

### 6.9 Progresso Global

Referência: Definição de Domínio v3, seção 4.4.

Progresso pertence ao Conteúdo, não à Coletânea. Se um conteúdo está em duas coletâneas e o usuário consome 50%, ambas refletem 50%.

```csharp
public enum EstadoProgresso { NaoIniciado, EmAndamento, Concluido }

public class Progresso
{
    public EstadoProgresso Estado { get; set; } = EstadoProgresso.NaoIniciado;
    public string? PosicaoAtual { get; set; } // Timestamp, página, texto livre (jogos)
    public string? ProgressoManual { get; set; } // Campo livre para progresso não-estruturado
    public ICollection<RegistroConsumo> HistoricoConsumo { get; set; } = new List<RegistroConsumo>();
}

public class RegistroConsumo
{
    public DateTimeOffset Data { get; init; }
    public decimal? PorcentagemConsumida { get; set; }
}
```

**Reversibilidade:** O usuário pode reverter qualquer estado de progresso (concluído → em andamento → não iniciado), conforme princípio de agência.

### 6.10 Disclosure Progressivo

Referência: Definição de Domínio v3, seção 2.3 (Economia Cognitiva).

Fundamentação: A Teoria de Carga Cognitiva (Sweller, 2005, *Cambridge Handbook of Multimedia Learning*) estabelece que a memória de trabalho é limitada. A revisão sistemática de Arnold, Goldschmitt & Rigotti (2023, *Frontiers in Psychology*), baseada em PRISMA, identificou sobrecarga informacional como causa de decisões ruins e produtividade reduzida. Shneiderman (1996, *ACM SIGCHI Bulletin*) propôs o conceito de "progressive disclosure" como estratégia de redução de complexidade visual, demonstrando em estudos de usabilidade que interfaces que revelam informação sob demanda reduzem erros e tempo de aprendizado.

**Implementação técnica:**

No formulário de cadastro de conteúdo, apenas o título é visível por padrão. Os demais campos são agrupados em seções expandíveis:

```
[Título] ← sempre visível, único obrigatório
▸ Classificação (formato, subtipo, nota, classificação) ← colapsado
▸ Descrição e Anotações ← colapsado
▸ Fontes ← colapsado
▸ Imagens ← colapsado
▸ Categorias ← colapsado
▸ Relações ← colapsado
```

**Configurabilidade:** O usuário pode desativar o disclosure progressivo nas preferências. Quando desativado, todos os campos são visíveis de uma vez. A seção em que o campo se encontra é transparente (o usuário sabe que existem mais campos e onde encontrá-los).

### 6.11 Operações em Lote

Referência: Definição de Domínio v3, seção 5.5.

O sistema permite operar sobre múltiplos conteúdos simultaneamente. Essencial para viabilidade prática com acervos de centenas ou milhares de conteúdos.

```csharp
public enum TipoOperacaoLote
{
    AdicionarCategoria,
    RemoverCategoria,
    MoverParaColetanea,
    RemoverDeColetanea,
    MarcarConcluido,
    MarcarNaoIniciado,
    Excluir
}

public sealed record OperacaoEmLoteCommand(
    Guid UsuarioId,
    IReadOnlyList<Guid> ConteudoIds,
    TipoOperacaoLote Tipo,
    string? DadosAdicionais  // JSON: {"categoriaId": "..."} ou {"coletaneaId": "..."}
) : IRequest<Result<ResultadoLote>>;

public sealed record ResultadoLote(int Sucesso, int Falha, IReadOnlyList<string> Erros);
```

**Regras:**
- Limite máximo: 500 itens por operação (prevenir uso abusivo de recursos).
- Transacionalidade: operação atômica — se qualquer item falhar, rollback total.
- Toda operação em lote é logada no histórico de ações de cada conteúdo afetado.
- Filtro por `UsuarioId` obrigatório (segregação de dados — seção 4.6).

### 6.12 Intervenções Ativas de Uso Saudável

Referência: Definição de Domínio v3, seção 6.3 (Intervenções Ativas — Opcionais e Configuráveis).

Intervenções são opcionais, desativáveis, e armazenadas nas preferências do usuário (Module.Preferencias):

```csharp
public class ConfiguracaoUsoSaudavel
{
    public bool MonitoramentoTempoAtivo { get; set; } = false; // Opt-in
    public TimeSpan? LembreteIntervalo { get; set; }           // null = desativado
    public bool RelatorioConsumoAtivo { get; set; } = false;
    public bool ModoCinzaAtivo { get; set; } = false;
}
```

**Implementação do modo escala de cinza:** Aplicar filtro de saturação zero no nível do tema Avalonia (não é CSS — é renderização Skia). Quando `ModoCinzaAtivo`, o tema aplica um `ColorMatrix` que zera os canais de cor, preservando luminância.

**Monitoramento de tempo:** Contabiliza tempo ativo na sessão (janela em foco). Exibe discretamente na barra de status. Não interrompe uso — apenas informa.

**Lembrete configurável:** Notificação interna (não push, não do SO) após intervalo definido pelo usuário. Mensagem informativa ("Você está no sistema há X minutos"). Botão "OK" e botão "Desativar lembrete". Não bloqueante.

### 6.13 ResultadoBusca como Projeção

Referência: Mapa de Contexto v1, seção 1.5.

"Conteúdo" no contexto Busca é um "resultado" — projeção otimizada para apresentação em lista, não a entidade completa com grafo de relações e histórico.

```csharp
public sealed record ResultadoBuscaDto(
    Guid Id,
    string Titulo,
    FormatoMidia Formato,
    string? SubtipoFormato,
    PapelEstrutural Papel,
    decimal? Nota,
    Classificacao? Classificacao,
    EstadoProgresso Progresso,
    string? ImagemPrincipalUrl,
    DateTimeOffset DataAdicao,
    IReadOnlyList<string> Categorias // Apenas nomes, não entidades completas
);
```

A Busca pode manter sua própria estrutura de indexação (índices GIN de full-text search — seção 3.3) otimizada para consultas rápidas, consumindo dados do Acervo como source of truth.

### 6.14 Dashboard como Visão

Referência: Definição de Domínio v3, seção 5.1.

Dashboard é uma **Visão** (seção 5.5) — montada sob demanda via queries sobre Conteúdo, Progresso e HistoricoAcao. Nunca armazenada no banco. A lógica de montagem reside na camada de apresentação (ViewModels), consumindo serviços do Module.Acervo como source of truth.

Seções configuráveis (todas colapsáveis e reordenáveis pelo usuário):
- **Continuar:** Conteúdos com `Progresso.Estado == EmAndamento`, ordenados por última data de consumo.
- **Novos em coletâneas:** Conteúdos adicionados a coletâneas do usuário mas ainda não iniciados.
- **Descobrir no acervo:** Amostra aleatória de conteúdos não consumidos do próprio acervo (não é recomendação algorítmica).
- **Conteúdos de miscelâneas:** Itens de coletâneas Miscelânea.

O usuário define quais seções ver e em que ordem. As seções acima são o padrão, não imposição.

---

## 7. Qualidade de Código

### 7.1 SAST (Análise Estática)

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

### 7.2 Convenções

Indentação 4 espaços. UTF-8. Modificadores explícitos. Namespaces file-scoped. Classes PascalCase, campos privados `_camelCase`, interfaces `I` + PascalCase, métodos async com sufixo `Async`.

**Nomes de domínio em português (sem acentos no código):** `Conteudo`, `Coletanea`, `FormatoMidia`, `PapelEstrutural`, `Categoria`, `Relacao`, `Progresso`, `Anotacao`. Termos técnicos em inglês: `Service`, `Repository`, `Handler`, `Provider`, `Dto`.

### 7.3 Práticas

- Métodos curtos (~20-30 linhas como guia, não dogma).
- Guard clauses. XML doc em APIs públicas. Zero magic numbers/strings.
- Sem `#region`. Sem catch que engole exceção. Todo `IDisposable` com `using`.

---

## 8. Estratégia de Testes

Pirâmide de testes conforme validada empiricamente por Contan et al. (2018, IEEE AQTR) e Wang et al. (2022, Journal of Systems and Software — correlação p=0.000624 entre maturidade de automação e qualidade).

### 8.1 Proporção

~60% unitários, ~25% integração, ~15% E2E + Security + Performance. Proporção ajustada para desktop monolítico (mais integração que web/microservices típico — validado pela observação de Contan et al. de que a pirâmide clássica nem sempre se aplica literalmente).

### 8.2 Unitários

**Ferramentas:** xUnit + FluentAssertions + NSubstitute.

Padrão AAA. Nome: `Metodo_Cenario_Esperado`. Determinísticos (injetar `TimeProvider`). Zero I/O externo.

**O que testar por camada — exemplos do domínio:**
- **Validators:** Título vazio rejeito. Nota fora de [0,10] rejeitada. TipoColetanea nulo quando Papel é Coletanea rejeitado.
- **ColetaneaService:** Detecção de ciclo quando A contém B e B tenta conter A. Adição normal sem ciclo. Composição (coletânea dentro de coletânea).
- **DeduplicadorConteudo:** URL idêntica retorna duplicata. Título similar sem URL não força merge. Normalização de URL funciona.
- **ResolvedorMetadados:** Manual prevalece sobre automático. Automático de fonte prevalece sobre externo.
- **CategoriaService:** Autocompletar retorna existentes. Não duplica categorias case-insensitive.
- **ViewModels:** Propriedades bindadas, commands enable/disable, INotifyPropertyChanged.
- **Criptografia:** encrypt→decrypt=original, senhas diferentes→hashes diferentes, salt único por operação.

```csharp
[Fact]
public async Task DetectarCiclo_ColetaneaContidaNaOutra_RetornaErro()
{
    // Arrange
    var coletaneaA = CriarColetanea("A");
    var coletaneaB = CriarColetanea("B");
    await _service.AdicionarItemAsync(coletaneaA.Id, coletaneaB.Id, CancellationToken.None);

    // Act — tentar adicionar A dentro de B (criaria ciclo A→B→A)
    var resultado = await _service.AdicionarItemAsync(coletaneaB.Id, coletaneaA.Id, CancellationToken.None);

    // Assert
    resultado.IsSuccess.Should().BeFalse();
    resultado.Error!.Codigo.Should().Be("CICLO_DETECTADO");
}

[Fact]
public void ResolverMetadado_ManualPrevaleceSobreAutomatico()
{
    var metadados = new List<MetadadoConteudo>
    {
        new() { Campo = "titulo", Valor = "Título do YouTube", Origem = OrigemMetadado.AutomaticoFonte },
        new() { Campo = "titulo", Valor = "Meu título corrigido", Origem = OrigemMetadado.Manual },
    };

    var resultado = _resolvedor.Resolver(metadados, "titulo");

    resultado.Should().Be("Meu título corrigido");
}
```

### 8.3 Integração

**Banco para testes:** PostgreSQL de teste (container Docker ou instância separada com banco temporário).

```csharp
public class ConteudoRepositoryTests : IAsyncLifetime
{
    private AppDbContext _context;
    private NpgsqlConnection _connection;

    public async Task InitializeAsync()
    {
        _connection = new NpgsqlConnection(
            "Host=localhost;Port=15432;Database=postgres;Username=test;Password=test");
        await _connection.OpenAsync();
        var dbName = $"test_{Guid.NewGuid():N}";
        await new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", _connection)
            .ExecuteNonQueryAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql($"Host=localhost;Port=15432;Database={dbName};Username=test;Password=test")
            .Options;
        _context = new AppDbContext(options);
        await _context.Database.MigrateAsync();
    }

    [Fact]
    public async Task Inserir_CategoriaDuplicadaCaseInsensitive_LancaViolacao()
    {
        var repo = new CategoriaRepository(_context);
        var usuarioId = Guid.NewGuid();
        await repo.AdicionarAsync(new Categoria { UsuarioId = usuarioId, Nome = "Ficção" });
        await _context.SaveChangesAsync();

        var act = () =>
        {
            repo.AdicionarAsync(new Categoria { UsuarioId = usuarioId, Nome = "ficção" });
            return _context.SaveChangesAsync();
        };
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task BuscaTextual_EncontraPorTituloEmPortugues()
    {
        var repo = new ConteudoRepository(_context);
        var usuarioId = Guid.NewGuid();
        await repo.AdicionarAsync(new Conteudo
        {
            UsuarioId = usuarioId,
            Titulo = "Programação Orientada a Objetos",
            Descricao = "Fundamentos de OOP em C#"
        });
        await _context.SaveChangesAsync();

        var resultados = await repo.BuscarTextualAsync(
            usuarioId, "programacao", PaginacaoParams.Criar(1, 20));

        resultados.TotalItens.Should().Be(1);
        // "programacao" (sem acento) deve encontrar "Programação" (com acento)
        // via configuração unaccent do PostgreSQL (seção 3.3)
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        var dbName = _context.Database.GetDbConnection().Database;
        await new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{dbName}\"", _connection)
            .ExecuteNonQueryAsync();
        await _connection.DisposeAsync();
    }
}
```

**Testes de migration:** verificar que cada migration aplica (up) e reverte (down) sem erro.

**Contract tests entre módulos:** validar que Module.Acervo e Module.Agregacao respeitam as interfaces compartilhadas da Partnership (seção 5.3).

### 8.4 End-to-End

**Framework:** Avalonia.Headless para testes de UI sem exibir janela.

```csharp
[Fact]
public async Task FluxoLogin_CredenciaisValidas_NavegarParaDashboard()
{
    using var app = new TestApp();
    var loginView = app.GetView<LoginView>();

    loginView.FindControl<TextBox>("InputUsuario").Text = "usuario_teste";
    loginView.FindControl<TextBox>("InputSenha").Text = "SenhaValida1!";
    loginView.FindControl<Button>("BotaoEntrar").Command.Execute(null);

    await Task.Delay(500);
    app.CurrentView.Should().BeOfType<DashboardView>();
}

[Fact]
public async Task CadastroConteudo_ApenasTitulo_CriaSucesso()
{
    using var app = new TestApp(usuarioAutenticado: true);
    var cadastroView = app.NavigateTo<CadastroConteudoView>();

    cadastroView.FindControl<TextBox>("InputTitulo").Text = "Meu primeiro conteúdo";
    cadastroView.FindControl<Button>("BotaoSalvar").Command.Execute(null);

    await Task.Delay(500);
    // Apenas título obrigatório — disclosure progressivo esconde o resto
    app.CurrentView.Should().BeOfType<DetalheConteudoView>();
}
```

Cobrir apenas fluxos críticos. Se um teste E2E é instável (flaky), corrigir ou remover.

### 8.5 Testes de Segurança

Input malicioso parametrizado em todo ponto de entrada:
```csharp
[Theory]
[InlineData("' OR 1=1 --")]
[InlineData("'; DROP TABLE conteudos;--")]
[InlineData("../../etc/passwd")]
[InlineData("\0")]
[InlineData("<script>alert(1)</script>")]
[MemberData(nameof(StringsLongas))] // 10.000+ caracteres
public async Task ProcessarInput_PayloadMalicioso_TratadoComSeguranca(string payload)
{
    var resultado = await _conteudoService.CriarAsync(
        _usuarioId, new CriarConteudoDto { Titulo = payload }, CancellationToken.None);
    // Deve rejeitar ou processar sem side effects maliciosos
}

[Fact]
public async Task AcessarAreaAdmin_SemRoleAdmin_RetornaGenerico()
{
    // Testar que a ocultação da área admin funciona (seção 4.6)
    var resultado = await _adminService.ObterConfiguracoesGlobaisAsync(_consumidorId);
    resultado.IsSuccess.Should().BeFalse();
    resultado.Error!.Codigo.Should().NotContain("admin"); // Não revela existência
}
```

Mutation testing com **Stryker.NET** periodicamente.

### 8.6 Performance

Medir tempo e memória com datasets de conteúdos: 1k, 10k, 100k registros. Cenários específicos:
- Busca textual em 100k conteúdos com full-text search.
- Montagem de feed com 10 subscrições simultâneas.
- Detecção de ciclos em coletâneas com profundidade 50.
- Listagem paginada de coletânea com 10k itens.

Definir thresholds. Falhar se exceder.

### 8.7 Cobertura

**Coverlet.** Meta: 80% global. 100% de branch em código de segurança (autenticação, autorização, criptografia) e em invariantes do domínio (detecção de ciclos, deduplicação, hierarquia de metadados).

### 8.8 Cenários de Validação do Domínio

Os 7 cenários do Apêndice A da Definição de Domínio v3 são testes de integração obrigatórios em `Tests.Domain/`. Cada cenário deve ser executável de ponta a ponta no sistema real:

| Cenário | Teste | Contextos envolvidos |
|---|---|---|
| 1. Listas de filmes temáticas com anotações | Criar coletânea Miscelânea, adicionar conteúdos de vídeo, anotações contextuais | Acervo |
| 2. CDs físicos com playlists e fallback | Criar conteúdos áudio com múltiplas fontes (local + URL), coletânea Miscelânea, testar fallback | Acervo, Reprodução |
| 3. Caderno físico sem mídia digital | Criar conteúdo formato "nenhum", sem fontes, com imagens e progresso manual | Acervo |
| 4. Plano de estudo cross-mídia | Coletânea Guiada com conteúdos de formatos variados e coletâneas filhas | Acervo |
| 5. Franquia RE com composição | Coletânea-mãe com coletâneas-filhas por mídia, verificar proteção contra ciclos | Acervo |
| 6. Seguir criadores sem dark patterns | Subscrições, agregador, verificar ausência de scroll infinito/métricas/ranqueamento | Agregação, Integração Externa |
| 7. Primeiro uso offline | Gestão completa sem internet, subscrições indisponíveis sinalizadas | Acervo, Agregação |

---

## 9. Observabilidade

### 9.1 Logging Estruturado

**Serilog** sobre `Microsoft.Extensions.Logging`.

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "DiarioDeBordo")
    .WriteTo.File(
        path: Path.Combine(dataDir, "logs", "diariodebordo-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 50_000_000,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```

**Regras:** nunca logar senhas, tokens, dados pessoais (conteúdos do usuário). Rotação por dia, retenção 30 dias, 50MB max por arquivo.

**Eventos de domínio a logar:**
- Criação/exclusão de conteúdo e coletânea (com Id, sem dados pessoais).
- Tentativa de criação de ciclo em coletânea (Warning).
- Persistência seletiva: ItemFeed → Conteúdo (Information).
- Falha de adaptador de plataforma (Warning, com nome da plataforma e tipo de erro).
- Tentativa de acesso não autorizado à área admin (Warning).
- Login e logout (Information).
- Exportação/importação de dados (Information).
- Alertas de uso saudável emitidos (Information).

### 9.2 Health Checks

Na inicialização e periodicamente: conectividade com PostgreSQL, espaço em disco, permissões de arquivo. Adicionalmente, para contextos que dependem de internet:
- Status de conectividade geral (para indicador visual online/offline).
- Status dos adaptadores de plataforma (quais plataformas estão acessíveis).

### 9.3 Métricas

Via `System.Diagnostics.Metrics`: tempo de inicialização, latência de operações de domínio (montagem de feed, busca textual, detecção de ciclos), uso de memória, contagem de erros por adaptador, retries de rede.

---

## 10. Performance

**Medir antes de otimizar** (Knuth, 1974, ACM Computing Surveys — "premature optimization is the root of all evil").

- **Memória:** `Span<T>`, `ArrayPool<T>.Shared`, streaming para arquivos grandes (importação/exportação).
- **CPU:** operações pesadas (montagem de feed, busca textual, detecção de ciclos em coletâneas profundas) em background, `async/await` para I/O, `CancellationToken`.
- **PostgreSQL:** connection pooling (Npgsql automático), índices em colunas consultadas (ver seção 3.3 para índices de FTS), `EXPLAIN ANALYZE` para queries lentas. CTE recursivo para detecção de ciclos é O(V+E) — aceitar custo linear na profundidade da árvore.
- **Rede (adaptadores de plataforma):** Polly para retry com exponential backoff + jitter, circuit breaker, cache de metadados (evitar buscar o mesmo metadado repetidamente), timeout de 10s por request.
- **Montagem de feed:** Quando o agregador consolida múltiplas subscrições, as consultas aos adaptadores podem ser feitas em paralelo (`Task.WhenAll`), com timeout individual por adaptador e falha parcial graceful (se um adaptador falhar, os outros feeds ainda são exibidos).

**Configuração de Polly (circuit breaker + retry):**
```csharp
services.AddHttpClient("PlataformasExternas")
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

## 11. Offline-First

O DiarioDeBordo é offline-first (Definição de Domínio v3, princípio 2.5). A operação principal (Pilar 1 — gestão de conteúdo pessoal) não depende de internet.

**Comportamento por contexto quando offline:**

| Contexto | Comportamento offline |
|---|---|
| **Acervo** | 100% funcional. Criação, edição, organização, progresso, anotações — tudo opera sobre registros locais. |
| **Agregação** | Feeds de subscrição apresentam apenas itens com registros salvos. Indicador visual de que a lista está incompleta. O agregador mostra apenas conteúdos já persistidos. |
| **Reprodução** | Conteúdos com fontes locais: 100% funcional. Fontes online: reprodução indisponível, sinalizar ao usuário. Fallback para próxima fonte local se existir. |
| **Integração Externa** | Adaptadores indisponíveis. Nenhuma consulta a APIs externas. Busca de metadados indisponível. |
| **Busca** | Busca interna 100% funcional (opera sobre banco local). Busca em plataformas externas indisponível. |
| **Portabilidade** | Exportação 100% funcional (opera sobre banco local). Importação 100% funcional. |
| **Identidade** | 100% funcional (autenticação local contra banco local). |
| **Preferências** | 100% funcional (configurações locais). |

**Detecção de conectividade:** Verificar acesso real ao endpoint (não apenas "está na rede"). O sistema não faz polling contínuo — verifica conectividade quando uma operação que requer internet é solicitada.

**Fila de operações pendentes:** Para ações que o usuário realiza offline mas que exigem sincronização posterior (ex: busca de metadados de um conteúdo recém-cadastrado com URL):

```csharp
// Tabela PostgreSQL para operações pendentes:
public class OperacaoPendente
{
    public Guid Id { get; init; }
    public string TipoOperacao { get; init; } = string.Empty; // "buscar_metadados", "validar_fonte"
    public string PayloadJson { get; set; } = string.Empty;    // Dados da operação (sem tokens/segredos)
    public int Tentativas { get; set; }
    public DateTimeOffset ProximaTentativa { get; set; }
    public int MaxTentativas { get; init; } = 5;
    public StatusOperacao Status { get; set; } = StatusOperacao.Pendente;
}
```

Processamento: FIFO, retry com backoff, desistir após max_attempts.

**Indicação visual:** online/offline + operações pendentes. O indicador é discreto (não intrusivo — princípio de uso saudável).

**Dados sensíveis na fila:** payloads que contêm tokens ou dados sensíveis devem ser criptografados na coluna antes de persistir, independentemente da criptografia do banco.

---

## 12. Gerenciamento de Dependências

**Critérios de adoção:** necessidade real, licença compatível (MIT/Apache/BSD, evitar GPL), manutenção ativa, zero vulnerabilidades conhecidas, mínimo de dependências transitivas.

**Manutenção:** `dotnet list package --vulnerable` em toda build. `dotnet-outdated` mensal. Patch versions imediatamente. `DEPENDENCIES.md` com inventário.

**Supply chain:** verificar nome exato e publisher. `dotnet nuget verify` para assinaturas. Monitorar com Dependabot.

---

## 13. Tratamento de Erros

Exceções para situações excepcionais. Result Pattern para falhas de negócio (seção 5.8). Handler global (`AppDomain.UnhandledException` + Avalonia) que loga com stack trace e exibe mensagem genérica ao usuário.

**Erros específicos do domínio:**

```csharp
public static class Erros
{
    // Acervo:
    public static readonly Erro ConteudoNaoEncontrado = new("CONTEUDO_NAO_ENCONTRADO", "Conteúdo não encontrado.");
    public static readonly Erro TituloObrigatorio = new("TITULO_OBRIGATORIO", "O título é obrigatório.");
    public static readonly Erro CicloDetectado = new("CICLO_DETECTADO", "Adicionar esta coletânea criaria uma referência circular.");
    public static readonly Erro NotaForaDaFaixa = new("NOTA_FORA_DA_FAIXA", "A nota deve estar entre 0 e 10.");

    // Identidade:
    public static readonly Erro NaoAutorizado = new("NAO_AUTORIZADO", "Operação não permitida.");
    public static readonly Erro CredenciaisInvalidas = new("CREDENCIAIS_INVALIDAS", "Usuário ou senha inválidos.");
    public static readonly Erro ContaBloqueada = new("CONTA_BLOQUEADA", "Conta temporariamente bloqueada. Tente novamente mais tarde.");

    // Integração Externa:
    public static readonly Erro FonteIndisponivel = new("FONTE_INDISPONIVEL", "Não foi possível conectar à fonte. Tente novamente mais tarde.");
    public static readonly Erro PlataformaNaoSuportada = new("PLATAFORMA_NAO_SUPORTADA", "Plataforma não suportada.");

    // Portabilidade:
    public static readonly Erro FormatoExportacaoInvalido = new("FORMATO_INVALIDO", "O arquivo de importação não é válido ou está corrompido.");

    // Genérico:
    public static readonly Erro ValorForaDaFaixa = new("VALOR_FORA_DA_FAIXA", "O valor informado está fora da faixa permitida.");
}
```

Mensagens de erro são informativas e em pt-BR, facilitando identificação do problema (requisito 9.2 do domínio).

---

## 14. Importação / Exportação / Backup

Referência: Definição de Domínio v3, seção 10.

**Exportação de dados do usuário (consumidor):**
- Conteúdos, coletâneas (com estrutura de composição), progresso, anotações (globais e contextuais), categorias, relações, fontes, imagens (referências), configurações pessoais.
- Formato: JSON estruturado com versão de schema, agnóstico de plataforma, legível e documentado.
- Criptografia opcional: AES-256-GCM com chave derivada de senha do usuário (Argon2id, MemorySize ≥128MB, salt e contexto distintos da senha de login).
- Senha de exportação: mínimo 12 caracteres com verificação de entropia.

**Exportação de configurações globais (admin):**
- Defaults globais, grupos de roles, limites do sistema.
- Mesmo formato JSON com versão de schema.

**Importação:**
- Validar integridade (SHA-256), versão de schema, decriptação (se criptografado).
- Processar em transação. Rollback se falhar.
- Suportar importação em outro SO (formato agnóstico — caminhos de fontes locais são adaptados ou sinalizados como inválidos).

**Backups automáticos:**
- Via `pg_dump` antes de cada migration e periodicamente (configurável pelo admin).
- Armazenados no diretório de dados com rotação (manter últimos N backups).

**Formato do pacote de exportação:**
```json
{
  "versao_schema": "1.0",
  "tipo": "dados_usuario",
  "data_exportacao": "2026-04-01T12:00:00Z",
  "conteudos": [ /* ... */ ],
  "coletaneas": [ /* com estrutura de composição */ ],
  "categorias": [ /* ... */ ],
  "relacoes": [ /* ... */ ],
  "configuracoes": { /* preferências do usuário */ },
  "checksum": "sha256:..."
}
```

**Importação de dados de plataformas externas (Definição de Domínio v3, seção 10.3):**

Funcionalidade planejada para implementação futura, mas o sistema deve ser projetado com extensibilidade. Parsers residem em Module.IntegracaoExterna (mesma fronteira dos adaptadores):

```csharp
public interface IParserImportacaoPlataforma
{
    bool SuportaFormato(string nomeArquivo); // "youtube_history.json", "goodreads_library.csv"

    Task<Result<IReadOnlyList<ConteudoParaImportacao>>> ParsearAsync(
        Stream arquivo, CancellationToken ct);
}

public sealed record ConteudoParaImportacao(
    string Titulo,
    FormatoMidia Formato,
    string? SubtipoFormato,
    string? Descricao,
    string? UrlFonte,
    EstadoProgresso? ProgressoEstimado
);
```

Novos parsers podem ser adicionados implementando `IParserImportacaoPlataforma` sem alterar o núcleo. Cada parser transforma dados da plataforma de origem nos registros do modelo de domínio.

---

## 15. Controle de Versão e CI

### 15.1 Git

Conventional Commits. `.gitignore` adequado (incluir exclusões para diretórios de dados do PostgreSQL de teste). Pre-commit hook: `dotnet format --verify-no-changes`.

### 15.2 Pipeline

1. Restore → 2. Build (analyzers = erros) → 3. Vulnerability scan → 4. Unitários → 5. Integração (inclui cenários do domínio) → 6. Cobertura (≥80%) → 7. E2E (branches principais) → 8. Security (nightly) → 9. Package (Velopack + assinatura).

**Etapas 1-6 são gate: falha = bloqueia merge.**

---

## 16. Recuperação de Desastres

| Cenário | Procedimento |
|---|---|
| Banco corrompido | PostgreSQL detecta via checksums. Alertar usuário. Restaurar último `pg_dump` automático. |
| Instalador falha | Velopack rollback automático. |
| PostgreSQL não inicia | Verificar logs, tentar restart automático. Se falhar, modo somente-leitura com dados em cache (limitado). |
| SO reinstalado | Importar backup exportado previamente (seção 14). |
| Espaço em disco esgotado | Detectar antes de operações de escrita. Modo read-only. Alertar usuário. |
| Adaptador de plataforma quebra (API mudou) | Circuit breaker isola a falha. Demais adaptadores continuam funcionando. Log de Warning. Sistema funciona normalmente exceto para feeds daquela plataforma. |
| Operação interrompida no meio | Transações PostgreSQL garantem atomicidade. Histórico de ações (seção 6.1) permite ao usuário identificar onde parou. Fila de operações pendentes (seção 11) retoma quando possível. |

**Recuperação de falhas locais (Definição de Domínio v3, seção 9.2):**

O requisito "continuidade do uso interrompido" exige que o sistema permita ao usuário retomar de onde parou após uma falha. Implementação:

1. **Operações atômicas:** Toda operação que modifica dados (criar conteúdo, mover para coletânea, marcar progresso) é envolvida em transação PostgreSQL. Se o app crashar no meio, o banco permanece íntegro — a transação é rolled back automaticamente.
2. **Rascunhos automáticos:** Para formulários longos (cadastro de conteúdo com múltiplos campos preenchidos), o sistema persiste periodicamente um rascunho local (a cada 30 segundos de inatividade no formulário). Ao reabrir após crash, oferece "Retomar rascunho" ou "Descartar".
3. **Histórico de ações:** O delta antes/depois (seção 6.1, `HistoricoAcao`) permite ao usuário identificar a última operação realizada e, se necessário, reverter.

---

## 17. Diferenças entre Plataformas

| Aspecto | Linux | Windows |
|---|---|---|
| Diretório de dados | `$XDG_DATA_HOME/DiarioDeBordo/` | `%LOCALAPPDATA%\DiarioDeBordo\` |
| Secure Storage | secret-tool (GNOME Keyring/KWallet) | DPAPI + entropy |
| Permissões de arquivo | `chmod 700` | ACLs via System.Security.AccessControl |
| PostgreSQL service | systemd (user-level) | Windows Service |
| Code signing | GPG ou sigstore | Authenticode |
| Instalador | .deb/.rpm + scripts de setup do PostgreSQL | Setup.exe via Velopack + scripts de setup do PostgreSQL |
| Locale do PostgreSQL | `pt_BR.UTF-8` | `Portuguese_Brazil.1252` ou `pt_BR.UTF-8` (PG 16+) |

---

## 18. Acessibilidade

Referência: Definição de Domínio v3, seção 8 — acessibilidade é cidadã de primeira classe.

**Base obrigatória:**
- Navegação completa por teclado em todos os fluxos.
- Contraste WCAG AA (4.5:1 para texto normal, 3:1 para texto grande).
- Tamanho mínimo de alvos interativos: 44x44dp.
- Informação nunca transmitida apenas por cor.
- `AutomationProperties` em todos os controles Avalonia (compatibilidade com leitores de tela).

**Configurações controladas pelo usuário (contexto Preferências):**
- Família da fonte, tamanho, cor — configuráveis individualmente.
- Temas: claro e escuro (escuro como padrão), com possibilidade de edições individuais pelo usuário e reset global pelo admin.
- Contraste elevado: opção que aumenta contraste além do mínimo WCAG AA.
- Redução de estímulos visuais: opção que desabilita animações e transições.
- Modo escala de cinza: opção de exibir em escala de cinza (intervenção de uso saudável — Definição de Domínio v3, seção 6.3: Intervenções Ativas).
- Disclosure progressivo: desativável pelo usuário (seção 6.10).

**Defaults globais:** O admin define valores padrão para todas as configurações de acessibilidade. O usuário sobrescreve individualmente.

---

## 19. Documentação

XML doc em APIs públicas. README (em pt-BR), CONTRIBUTING, CHANGELOG, DEPENDENCIES. ADRs em `docs/adr/`. Formato de exportação documentado (seção 14).

---

## 20. Checklist de Revisão

**Funcionalidade:**
- Implementa o requisito conforme Definição de Domínio v3.
- Cobre cenários de borda (Apêndice A do domínio quando aplicável).
- Funciona offline (para funcionalidades do Pilar 1).
- Respeita as restrições de Uso Saudável (sem scroll infinito, paginação obrigatória, sem autoplay).
- Disclosure progressivo aplicado em formulários.

**Segurança:**
- Inputs validados (URLs de fonte, payloads RSS/JSON, strings do usuário).
- Nenhum dado sensível em logs/mensagens/código.
- Queries parametrizadas.
- Permissões verificadas na camada de serviço (Role.Admin para área admin, UsuarioId em toda query de dados).
- Ocultação da área admin para não-admins.
- Comparações de hash em tempo constante.

**Domínio:**
- Invariantes respeitadas (ciclos, deduplicação, hierarquia de metadados, progresso global).
- Limites de imagem respeitados (máximo 20 por conteúdo, 10MB por imagem — seção 6.1).
- Persistência seletiva: ItemFeed nunca vira tabela/DbSet.
- Fronteiras de bounded context respeitadas (nenhum módulo acessa diretamente outro).
- Categorias e tipos de relação com não-duplicação.
- Rascunhos automáticos funcionando em formulários longos (persistência a cada 30s de inatividade, recuperação após crash — seção 16).

**Qualidade:**
- Compila sem warnings.
- Analyzers satisfeitos.
- Nomes claros e em pt-BR para entidades de domínio.
- DI.
- Toda lista retorna `PaginatedList<T>`.

**Testes:**
- Unitários para cenários normais e de borda.
- Integração com banco real.
- Inputs maliciosos nos pontos de entrada.
- Contract tests entre módulos que se comunicam.
- Cobertura mantida (≥80% global, 100% branch em segurança e invariantes).

**Performance:**
- Operações pesadas (feed, busca, ciclos) em background.
- Recursos liberados (`using`, `CancellationToken`).
- Consultas a adaptadores em paralelo quando possível.

**Observabilidade:**
- Operações de domínio logadas (criação, exclusão, persistência seletiva, falhas de adaptador).
- Erros com contexto (Id da entidade, tipo de operação).
- Nenhum dado sensível ou pessoal em logs.

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

Ver `BannedSymbols.txt` na seção 7.1. Adicionalmente: `dynamic` (exceto interop), `GC.Collect()`, `#pragma warning disable` sem justificativa, `catch (Exception) { }`.

## Apêndice C: Threat Model STRIDE — DiarioDeBordo

| Categoria | Ameaça | Mitigação |
|---|---|---|
| Spoofing | Atacante finge ser outro usuário | Argon2id + rate limiting (4.1 DA2) |
| Spoofing | Atacante acessa área admin sem role | Ocultação + RBAC na camada de serviço (4.6) |
| Tampering | Modificação do banco ou binários | PostgreSQL checksums + code signing (4.2) |
| Tampering | RSS/XML malicioso altera dados | DtdProcessing.Prohibit + validação de payload (4.2) |
| Tampering | URL de fonte maliciosa (SSRF) | Validação de URL + rejeição de IPs privados (4.1, 4.2) |
| Repudiation | Negação de ação do usuário | Histórico de ações por conteúdo (6.9) + audit trail via logging (9.1) |
| Info Disclosure | Vazamento de dados entre usuários | Segregação por UsuarioId em toda query (4.6) |
| Info Disclosure | Revelação da existência da área admin | Ocultação completa + resposta genérica (4.6) |
| Info Disclosure | Dados sensíveis em logs | Regras de logging — nunca logar senhas/tokens/dados pessoais (9.1) |
| Denial of Service | Feed RSS gigante esgota memória | Limite de payload (5MB), timeout (10s), circuit breaker (4.2, 10) |
| Denial of Service | JSON bomb na importação | Validação de tamanho e profundidade (4.1 DA1) |
| Elevation of Privilege | Consumidor acessa funções de admin | RBAC na camada de serviço + sem indícios na UI (4.6) |

## Apêndice D: Referências

### Periódicos Científicos Peer-Reviewed

1. Wang, Y., Mäntylä, M., Liu, Z. & Markkula, J. (2022). "Test automation maturity improves product quality." *Journal of Systems and Software*, vol. 188, Elsevier. DOI: 10.1016/j.jss.2022.111262

2. Khan, R. A. et al. (2022). "Systematic Literature Review on Security Risks and its Practices in Secure Software Development." *IEEE Access*, vol. 10. DOI: 10.1109/ACCESS.2022.3140181

3. Amaro, R., Pereira, R. & da Silva, M. M. (2023). "Capabilities and practices in DevOps." *IEEE Transactions on Software Engineering*, vol. 49, pp. 883-901. DOI: 10.1109/TSE.2022.3170654

4. Pereira, R. et al. (2024). "DevSecOps practices and tools." *International Journal of Information Security*, Springer. DOI: 10.1007/s10207-024-00914-z

5. Simonetti, L. et al. (2024). "Applying SOLID principles for the refactoring of legacy code: An experience report." *Journal of Systems and Software*, Elsevier. DOI: 10.1016/j.jss.2024.112233

6. Al-Qora'n, L. & Al-Said Ahmad, A. (2025). "Modular Monolith Architecture in Cloud Environments: A SLR." *Future Internet*, vol. 17, no. 11, p. 496. MDPI. DOI: 10.3390/fi17110496

7. Elmagarmid, A. K., Ipeirotis, P. G. & Verykios, V. S. (2007). "Duplicate Record Detection: A Survey." *IEEE Transactions on Knowledge and Data Engineering*, vol. 19, no. 1, pp. 1-16. DOI: 10.1109/TKDE.2007.250581 — Survey abrangente de técnicas de detecção de duplicatas. Referenciada na seção 6.4.

8. Arnold, M., Goldschmitt, M. & Rigotti, T. (2023). "Dealing with information overload: a comprehensive review." *Frontiers in Psychology*, 14, 1122200. — Revisão sistemática PRISMA sobre sobrecarga informacional. Referenciada na seção 6.10.

### Conferências Peer-Reviewed

9. Contan, C., Dehelean, C. & Miclea, L. (2018). "Test Automation Pyramid from Theory to Practice." *IEEE AQTR*. DOI: 10.1109/AQTR.2018.8402699

10. Su, R. & Li, X. (2024). "Modular Monolith: Is This the Trend in Software Architecture?" *ACM SATrends '24*. DOI: 10.1145/3643657.3643911

11. Cabral, R. et al. (2024). "Investigating the Impact of SOLID Design Principles on ML Code Understanding." *IEEE/ACM CAIN 2024*.

12. Singh, H. & Hassan, S. I. (2015). "Effect of SOLID Design Principles on Quality of Software: An Empirical Assessment." *IJSER*, vol. 6, no. 4.

13. Turan, O. & Tanrıöver, Ö. (2018). "An Experimental Evaluation of the Effect of SOLID Principles to Microsoft VS Code Metrics." *AJIT-e*.

14. Tarjan, R. E. (1972). "Depth-First Search and Linear Graph Algorithms." *SIAM Journal on Computing*, vol. 1, no. 2, pp. 146-160. DOI: 10.1137/0201010 — Algoritmo de DFS com classificação de arestas para detecção de ciclos em tempo linear. Referenciado na seção 6.2.

### Autores Seminais (validados por estudos acima)

15. McGraw, G. (2004). "Software Security." *IEEE Security & Privacy*, vol. 2, no. 2. — Validado por Khan et al. (2022, ref. 2).

16. Martin, R. C. (2000). "Design Principles and Design Patterns." — Validados por Singh & Hassan (2015, ref. 12), Cabral et al. (2024, ref. 11), Simonetti et al. (2024, ref. 5).

17. Martin, R. C. (2017). *Clean Architecture*. Prentice Hall. — Validados por Simonetti et al. (2024, ref. 5).

18. Evans, E. (2003). *Domain-Driven Design*. Addison-Wesley. — Validados por Al-Qora'n & Al-Said Ahmad (2025, ref. 6).

19. Fowler, M. (2012). "The Practical Test Pyramid." martinfowler.com. — Validada por Contan et al. (2018, ref. 9) e Wang et al. (2022, ref. 1).

20. Bass, L., Clements, P. & Kazman, R. (2012). *Software Architecture in Practice*, 3rd ed. Pearson.

21. Humble, J. & Farley, D. (2010). *Continuous Delivery*. Addison-Wesley.

22. Shneiderman, B. (1996). "The Eyes Have It: A Task by Data Type Taxonomy for Information Visualizations." *Proceedings of the IEEE Symposium on Visual Languages*. — Conceito de progressive disclosure. Referenciado na seção 6.10.

23. Sweller, J. (2005). "Implications of Cognitive Load Theory for Multimedia Learning." In *The Cambridge Handbook of Multimedia Learning*. Cambridge University Press. — Teoria de Carga Cognitiva. Referenciada na seção 6.10.

### Padrões Normativos

24. OWASP Desktop App Security Top 10 (2021).
25. NIST SP 800-63B — Digital Identity Guidelines: Authentication.
26. NIST SP 800-132 — Password-Based Key Derivation.
27. IEEE Std 2675-2021 — Standard for DevOps.
28. ISO/IEC 25010:2011 — Software Quality Model.

### Documentações Técnicas

29. PostgreSQL Documentation — https://www.postgresql.org/docs/
30. Npgsql/EF Core Provider — https://www.npgsql.org/efcore/
31. Percona pg_tde — https://docs.percona.com/pg-tde/
32. Velopack — https://docs.velopack.io/
33. Avalonia UI — https://docs.avaloniaui.net/
34. PostgreSQL Full Text Search — https://www.postgresql.org/docs/current/textsearch.html

---

*Documento vivo. Atualizar a cada decisão técnica, ameaça identificada ou revisão de padrão. Manter consistência com os documentos de domínio (Definição v3, Mapa de Domínio v1, Mapa de Contexto v1, Plano de Implementação v3).*
