# Documento de Padrões e Diretrizes Técnicas

## Guia Técnico de Referência para Desenvolvimento de Aplicação Desktop Cross-Platform

**Versão:** 1.0  
**Classificação:** Documento agnóstico ao domínio — define *como* desenvolver, não *o que* desenvolver.  
**Público-alvo:** Qualquer desenvolvedor (inclusive iniciante) que precise implementar funcionalidades de forma segura, testada e alinhada com padrões de engenharia de software de alto nível.

---

## 1. Propósito deste Documento

Este documento é a **fonte da verdade técnica** do projeto. Ele define as tecnologias, padrões, técnicas, ferramentas, convenções e práticas obrigatórias para o desenvolvimento do sistema. Qualquer pessoa que leia este documento deve ser capaz de produzir implementações que atendam aos seguintes critérios de qualidade:

- **Manutenibilidade:** código legível, modular e fácil de modificar.
- **Segurança:** resistência a ataques maliciosos em todas as camadas (código, dados, infraestrutura local, comunicação de rede, binários).
- **Eficiência de recursos:** uso otimizado de memória, CPU, disco, rede e créditos de serviços terceiros.
- **Validação automatizada:** testes, linters e analisadores que garantam a integridade funcional e estrutural do código.
- **Observabilidade:** capacidade de detectar, registrar e alertar sobre estados anômalos do sistema.
- **Confiabilidade:** comportamento previsível em cenários esperados e inesperados.
- **Resiliência offline:** funcionamento pleno das funcionalidades locais mesmo sem conexão à internet.

---

## 2. Stack Tecnológica

### 2.1 Linguagem e Runtime

| Componente | Decisão | Justificativa |
|---|---|---|
| **Linguagem principal** | C# | Linguagem fortemente tipada, com ecossistema maduro para desktop. Suporte nativo a padrões como async/await, LINQ, nullable reference types e source generators. |
| **Runtime** | .NET 9+ (LTS quando disponível) | Suporte cross-platform (Windows e Linux), performance otimizada via JIT/AOT, garbage collector de gerações. Sempre acompanhar a versão LTS mais recente. |
| **Compilação AOT** | Avaliar NativeAOT para releases de produção | Reduz tempo de inicialização e tamanho do binário. Elimina dependência do JIT em runtime, o que melhora a segurança contra certas classes de ataques de injeção em memória. |

**Configuração obrigatória do projeto (.csproj):**

```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <AnalysisLevel>latest</AnalysisLevel>
  <AnalysisMode>All</AnalysisMode>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

- `Nullable enable`: elimina a classe inteira de bugs de NullReferenceException ao forçar o desenvolvedor a lidar explicitamente com nulabilidade.
- `TreatWarningsAsErrors`: nenhum warning é tolerado no build — warnings ignorados se acumulam e escondem problemas reais.
- `AnalysisMode All`: ativa todas as regras dos Roslyn Analyzers embutidos no SDK.

### 2.2 Framework de Interface (UI)

| Componente | Decisão | Justificativa |
|---|---|---|
| **Framework de UI** | **Avalonia UI** | Framework open-source (licença MIT), cross-platform real (Windows, Linux, macOS), baseado em XAML/C#. Usa Skia como engine de renderização, garantindo aparência pixel-perfect consistente entre plataformas. Ecossistema ativo com mais de 170.000 empresas adotantes, incluindo JetBrains. Considerado o sucessor espiritual do WPF. |
| **Biblioteca de componentes** | **SukiUI** ou **Material.Avalonia** | Bibliotecas de componentes visuais que fornecem um design system pronto e consistente, eliminando a necessidade de construir componentes básicos do zero. Avaliar qual oferece melhor cobertura de componentes para as necessidades do projeto. |
| **Padrão arquitetural de UI** | **MVVM (Model-View-ViewModel)** | Separa lógica de apresentação da lógica de negócio. Permite testar ViewModels sem instanciar a UI. Padrão nativo do ecossistema XAML. Usar CommunityToolkit.Mvvm para reduzir boilerplate. |

**Por que não as alternativas:**
- **.NET MAUI:** Sem suporte oficial a Linux. Foco primário em mobile. Relatos consistentes de instabilidade em desktop.
- **Uno Platform:** Mais complexo para cenários exclusivamente desktop. Seu ponto forte é o alcance web+mobile, que não é necessário aqui.

### 2.3 Banco de Dados

| Componente | Decisão | Justificativa |
|---|---|---|
| **SGBD** | **SQLite** | Banco embutido (embedded), sem servidor separado, zero-configuração. O banco inteiro é um único arquivo no filesystem. Ideal para aplicações desktop single-user/low-concurrency. Não requer instalação separada, simplificando distribuição. Suporta transações ACID. Usado em produção por Firefox, Chrome, Android, iOS e centenas de milhares de aplicações. |
| **ORM** | **Entity Framework Core** com `Microsoft.Data.Sqlite` | ORM oficial da Microsoft para .NET. Suporta migrations (versionamento de schema), LINQ queries, change tracking. Facilita manutenção do schema ao longo do tempo. |
| **Criptografia em repouso** | **SQLite3 Multiple Ciphers** via `SQLitePCLRaw.bundle_e_sqlite3mc` | Extensão que adiciona criptografia AES-256 ao SQLite. Suporte cross-platform. Compatível com Entity Framework Core via `Microsoft.Data.Sqlite.Core`. Gratuito e open-source. |

**Por que não PostgreSQL:**
- PostgreSQL é um SGBD cliente-servidor projetado para ambientes multi-usuário com alta concorrência. Para uma aplicação desktop local com um único usuário ativo por vez, ele introduz complexidade desnecessária: exige instalação e execução de um serviço separado, configuração de rede, gerenciamento de processos. SQLite elimina tudo isso enquanto atende perfeitamente os requisitos de escala do projeto.

**Regras para acesso ao banco:**
- **Nunca** concatenar strings para montar queries SQL. Sempre usar queries parametrizadas ou o ORM.
- **Nunca** armazenar senhas em texto plano. Usar algoritmos de hash com salt (Argon2id preferencialmente, ou bcrypt).
- Aplicar o princípio do menor privilégio: a aplicação deve ter acesso apenas às operações de banco que necessita.
- Toda alteração de schema deve ser feita via migrations versionadas e rastreáveis.

### 2.4 Distribuição e Atualização

| Componente | Decisão | Justificativa |
|---|---|---|
| **Empacotamento e instalação** | **Velopack** | Framework cross-platform (Windows, Linux) para criação de instaladores e atualização automática. Escrito em Rust (performance nativa). Suporte a delta packages (usuário baixa apenas o que mudou). CLI simples (`vpk`) que integra facilmente em pipelines CI/CD. Suporta code signing. |
| **Mecanismo de atualização** | Atualização automática com confirmação do usuário | O sistema verifica periodicamente (e sob demanda) se há novas versões. Exibe notificação ao usuário. A atualização só é aplicada após confirmação explícita de um usuário com permissão. |

**Requisitos de segurança para atualização:**
- Pacotes de atualização devem ser assinados digitalmente. A aplicação deve verificar a assinatura antes de aplicar qualquer atualização.
- O canal de download deve usar HTTPS exclusivamente.
- A integridade do pacote deve ser verificada via hash (SHA-256 mínimo) antes da instalação.
- Em caso de falha na atualização, o sistema deve realizar rollback automático para a versão anterior.

---

## 3. Arquitetura

### 3.1 Monolito Modular

A aplicação segue a arquitetura de **Monolito Modular**: um único deployable composto por módulos internos com fronteiras bem definidas.

**Princípio central:** cada módulo encapsula uma área de responsabilidade com interfaces explícitas. Módulos se comunicam via contratos (interfaces), nunca acessando diretamente as classes internas uns dos outros.

**Estrutura de projetos (solução .NET):**

```
Solution/
├── src/
│   ├── App.Desktop/              # Entry point, composição de DI, bootstrap Velopack
│   ├── App.Core/                 # Abstrações compartilhadas, interfaces, DTOs, value objects
│   ├── App.Infrastructure/       # Implementações: banco de dados, filesystem, HTTP clients, criptografia
│   ├── App.UI/                   # Views (XAML), ViewModels, converters, componentes visuais
│   └── Modules/
│       ├── Module.FeatureA/      # Módulo de negócio A (regras, serviços, repositórios próprios)
│       ├── Module.FeatureB/      # Módulo de negócio B
│       └── Module.Shared/        # Funcionalidades compartilhadas entre módulos
├── tests/
│   ├── Tests.Unit/               # Testes unitários
│   ├── Tests.Integration/        # Testes de integração
│   ├── Tests.E2E/                # Testes end-to-end
│   └── Tests.Security/           # Testes de segurança (fuzzing, penetration)
├── tools/                        # Scripts auxiliares, seed data, utilitários de desenvolvimento
├── docs/                         # Documentação do projeto
├── Directory.Build.props         # Configurações globais de build (analyzers, warnings, etc.)
├── .editorconfig                 # Regras de estilo e formatação
└── Solution.sln
```

### 3.2 Princípios Arquiteturais Obrigatórios

**SOLID:**
- **S (Single Responsibility):** cada classe tem uma única razão para mudar. Se uma classe faz parsing E validação E persistência, ela deve ser dividida.
- **O (Open/Closed):** entidades abertas para extensão, fechadas para modificação. Preferir composição e padrões como Strategy a modificar classes existentes.
- **L (Liskov Substitution):** subtipos devem ser substituíveis por seus tipos base sem quebrar o comportamento. Não sobrescrever métodos de forma que viole o contrato da classe base.
- **I (Interface Segregation):** interfaces pequenas e coesas. Nenhum consumidor deve depender de métodos que não usa.
- **D (Dependency Inversion):** módulos de alto nível dependem de abstrações (interfaces), não de implementações concretas. Toda dependência externa (banco, filesystem, HTTP, clock) deve ser injetada.

**Injeção de Dependência (DI):**
- Usar o container de DI nativo do .NET (`Microsoft.Extensions.DependencyInjection`).
- Todo serviço, repositório e cliente externo deve ser registrado e resolvido via DI.
- **Nunca** instanciar dependências com `new` dentro de classes de negócio (exceto para value objects e DTOs).
- Isso permite que testes substituam qualquer dependência por mocks/stubs.

**Imutabilidade preferencial:**
- Preferir records e readonly structs para transferência de dados.
- Preferir `IReadOnlyList<T>`, `IReadOnlyDictionary<K,V>` em interfaces públicas.
- Mutação de estado deve ser explícita, centralizada e controlada.

### 3.3 Padrões de Design Aplicáveis

| Padrão | Onde usar | Por que |
|---|---|---|
| **Repository** | Acesso a dados | Abstrai o acesso ao banco, permitindo trocar a implementação sem afetar a lógica de negócio. Facilita mocking em testes. |
| **Unit of Work** | Transações | Agrupa múltiplas operações de banco em uma transação atômica. EF Core já implementa isso via DbContext. |
| **Strategy** | Algoritmos variáveis | Quando uma operação pode ser realizada de múltiplas formas (ex: diferentes provedores de criptografia, diferentes fontes de dados). |
| **Observer/Event** | Comunicação entre módulos | Módulos publicam eventos; outros módulos se inscrevem. Desacopla emissor de receptor. |
| **Factory** | Criação complexa de objetos | Quando a construção de um objeto envolve lógica condicional ou configuração. |
| **Decorator** | Cross-cutting concerns | Para adicionar logging, caching, retry, etc. sem modificar a classe original. |
| **Result Pattern** | Tratamento de erros | Substituir exceptions para controle de fluxo por objetos `Result<T>` que representam sucesso ou falha. Exceptions devem ser reservadas para situações verdadeiramente excepcionais. |

---

## 4. Segurança

Este é o capítulo mais extenso e crítico do documento. A segurança deve ser tratada como uma propriedade do sistema inteiro, não como um recurso a ser adicionado depois.

### 4.1 Referência: OWASP Desktop App Security Top 10

O OWASP (Open Worldwide Application Security Project) mantém uma lista das 10 vulnerabilidades mais críticas para aplicações desktop. Todo desenvolvedor **deve** conhecer e mitigar cada uma delas.

**DA1 — Injections (SQL, OS Command, LDAP, XML, etc.):**

Ocorre quando dados não confiáveis são enviados a um interpretador como parte de um comando ou query.

*Mitigação obrigatória:*
- Usar **exclusivamente** queries parametrizadas ou o ORM para acesso ao banco. Nunca construir queries com concatenação de strings.
- Sanitizar e validar **toda** entrada do usuário antes de processá-la. Aplicar whitelist (permitir apenas o esperado) em vez de blacklist (bloquear o indesejado).
- Para execução de processos do sistema operacional via `Process.Start()`: nunca passar input do usuário diretamente como argumento. Validar contra uma whitelist de comandos e parâmetros permitidos.
- Para XML: desabilitar processamento de entidades externas (XXE). Usar `XmlReaderSettings` com `DtdProcessing = DtdProcessing.Prohibit`.

```csharp
// ERRADO — vulnerável a SQL injection:
var sql = $"SELECT * FROM Users WHERE Name = '{userInput}'";

// CORRETO — query parametrizada:
var user = await context.Users
    .Where(u => u.Name == userInput)
    .FirstOrDefaultAsync();

// Ou com Dapper:
var user = await connection.QueryFirstOrDefaultAsync<User>(
    "SELECT * FROM Users WHERE Name = @Name",
    new { Name = userInput });
```

**DA2 — Broken Authentication & Session Management:**

Falhas na autenticação ou gerenciamento de sessões que permitem a um atacante comprometer senhas, chaves ou tokens.

*Mitigação obrigatória:*
- Armazenar senhas usando **Argon2id** (ou bcrypt/scrypt como segunda opção) com salt único por senha. Nunca usar MD5, SHA-1 ou SHA-256 simples para hashing de senhas.
- Derivar chaves de criptografia usando **PBKDF2** com pelo menos 600.000 iterações (recomendação OWASP 2023) ou Argon2id.
- Limitar tentativas de login (rate limiting/lockout após N falhas).
- Invalidar sessões em caso de inatividade prolongada.
- No contexto desktop multi-usuário local: cada usuário deve ter um perfil separado com dados isolados. Um usuário não deve conseguir acessar dados de outro.

**DA3 — Sensitive Data Exposure (Exposição de Dados Sensíveis):**

Dados sensíveis armazenados ou transmitidos sem proteção adequada.

*Mitigação obrigatória:*
- **Criptografia em repouso:** o banco SQLite deve ser criptografado com AES-256 via SQLite3 Multiple Ciphers.
- **Criptografia em trânsito:** toda comunicação com APIs externas deve usar TLS 1.2+ exclusivamente. Validar certificados SSL. Nunca desabilitar validação de certificados, mesmo em desenvolvimento.
- **Gerenciamento de segredos:** nunca armazenar senhas, API keys, tokens ou chaves criptográficas no código-fonte, em arquivos de configuração em texto plano ou em variáveis de ambiente acessíveis.
  - No Windows: usar DPAPI (Data Protection API) via `System.Security.Cryptography.ProtectedData`.
  - No Linux: usar o keyring do sistema via bibliotecas como `Tmds.DBus` para integrar com GNOME Keyring ou KWallet.
  - Alternativa cross-platform: derivar uma chave de criptografia da senha do usuário via Argon2id e usá-la para proteger os segredos armazenados localmente.
- **Limpeza de memória:** dados sensíveis (senhas, chaves) devem ser limpos da memória assim que não forem mais necessários. Usar `SecureString` onde aplicável, ou `CryptographicOperations.ZeroMemory()` em spans.
- **Logs:** nunca registrar dados sensíveis em logs (senhas, tokens, dados pessoais, números de documentos).

**DA4 — Improper Cryptography:**

Uso de algoritmos criptográficos fracos, obsoletos ou implementados incorretamente.

*Mitigação obrigatória:*
- **Nunca** implementar algoritmos criptográficos próprios. Usar exclusivamente implementações padronizadas e auditadas.
- Algoritmos proibidos: MD5, SHA-1, DES, 3DES, RC4. Estes são considerados quebrados ou obsoletos.
- Algoritmos permitidos:

| Finalidade | Algoritmo | Parâmetros mínimos |
|---|---|---|
| Criptografia simétrica | AES-256 | Modo GCM (autenticado) preferencialmente; CBC com HMAC como alternativa |
| Hash de senhas | Argon2id | Memory: 64MB, Iterations: 3, Parallelism: 4 (ajustar conforme hardware) |
| Hash de integridade | SHA-256 ou SHA-512 | — |
| Derivação de chaves | Argon2id ou PBKDF2 | PBKDF2: mínimo 600.000 iterações com SHA-256 |
| Assinatura digital | RSA-2048+ ou Ed25519 | — |
| Geração de números aleatórios | `RandomNumberGenerator` (.NET) | Nunca usar `System.Random` para fins criptográficos |

**DA5 — Improper Authorization:**

Falhas no controle de autorização que permitem usuários acessarem recursos ou funcionalidades além de suas permissões.

*Mitigação obrigatória:*
- Implementar RBAC (Role-Based Access Control) ou ABAC (Attribute-Based Access Control) conforme a complexidade necessária.
- Validar permissões no lado do servidor (neste caso, na camada de serviço/negócio), nunca apenas na UI.
- Não confiar em dados controlados pelo cliente (inputs, estados de UI) para decisões de autorização.
- Registrar (logar) toda tentativa de acesso não autorizado.

**DA6 — Security Misconfiguration:**

Configurações padrão inseguras, serviços desnecessários habilitados, permissões excessivas.

*Mitigação obrigatória:*
- Remover qualquer código de debug, endpoints de teste, ou funcionalidades de desenvolvimento antes do build de release.
- Desabilitar compilação condicional com `#if DEBUG` para funcionalidades sensíveis e garantir que nunca vazem para produção.
- Definir permissões de arquivo do banco de dados e diretório de dados da aplicação restritamente: apenas o usuário do sistema operacional que executa a aplicação deve ter acesso.
- Manter todas as dependências atualizadas (ver seção sobre gerenciamento de dependências).

**DA7 — Insecure Communication:**

Dados transmitidos em texto plano ou com proteção inadequada.

*Mitigação obrigatória:*
- **Todo** tráfego de rede deve usar TLS 1.2 ou superior.
- Implementar certificate pinning para APIs críticas quando possível.
- Validar todos os certificados SSL/TLS. Nunca usar `ServerCertificateCustomValidationCallback` retornando `true`.
- Para comunicação entre processos (IPC), se necessário: usar named pipes com ACLs restritivas ou Unix domain sockets com permissões adequadas.

**DA8 — Poor Code Quality:**

Código de baixa qualidade que introduz vulnerabilidades por descuido.

*Mitigação obrigatória:* toda a seção 5 (Qualidade de Código) e seção 6 (Testes) deste documento.

**DA9 — Using Components with Known Vulnerabilities:**

Uso de bibliotecas ou dependências com vulnerabilidades conhecidas e publicadas.

*Mitigação obrigatória:*
- Manter um SBOM (Software Bill of Materials) — uma lista completa de todas as dependências do projeto e suas versões.
- Executar `dotnet list package --vulnerable` regularmente (e no pipeline de CI).
- Usar ferramentas como `dotnet-outdated` para detectar dependências desatualizadas.
- Configurar alertas automáticos (ex: GitHub Dependabot, ou equivalente) para vulnerabilidades em dependências.
- Antes de adicionar qualquer nova dependência: verificar a licença, a frequência de manutenção, o número de vulnerabilidades conhecidas e a reputação do mantenedor.

**DA10 — Insufficient Logging & Monitoring:**

Ausência ou inadequação de logs e monitoramento que impede a detecção de ataques ou anomalias.

*Mitigação obrigatória:* toda a seção 8 (Observabilidade) deste documento.

### 4.2 Segurança de Binários

- **Code signing:** assinar digitalmente todos os executáveis e pacotes de instalação/atualização. Isso garante ao sistema operacional e ao usuário que o binário não foi adulterado.
- **Ofuscação:** avaliar o uso de ferramentas de ofuscação (ex: ConfuserEx, .NET Reactor) para dificultar engenharia reversa. Isso é uma camada adicional, não uma substituição para segurança real.
- **Integridade em runtime:** implementar verificação de integridade do próprio executável na inicialização para detectar tampering.
- **Proteção contra debugging:** em builds de release, considerar técnicas anti-debug para dificultar análise dinâmica por atacantes. Entretanto, priorizar segurança por design sobre obscuridade.

### 4.3 Segurança do Filesystem

- O diretório de dados da aplicação deve ter permissões restritivas (readable/writable apenas pelo usuário que executa a aplicação).
- Validar **todo** caminho de arquivo recebido como input (do usuário ou de fontes externas) contra path traversal:
  - Canonicalizar o path (`Path.GetFullPath`) e verificar que ele está dentro do diretório permitido.
  - Rejeitar paths contendo `..`, caracteres nulos ou caracteres especiais do sistema operacional.
- Ao ler arquivos fornecidos pelo usuário (importação): validar formato, tamanho e conteúdo antes de processar.
- Limitar tamanho máximo de arquivos aceitos pela funcionalidade de importação.
- Criar e usar diretórios temporários com nomes aleatórios para operações intermediárias.

### 4.4 Proteção contra Ataques Específicos a Desktop

| Ataque | Descrição | Mitigação |
|---|---|---|
| **DLL Hijacking/Preloading** | Atacante coloca DLL maliciosa em diretório de busca do loader | Especificar caminhos absolutos para carregamento de bibliotecas. Usar `SetDefaultDllDirectories` no Windows. Assinar DLLs. |
| **Memory Dumping** | Extração de dados sensíveis da memória do processo | Limpar dados sensíveis da memória imediatamente após uso. Evitar manter senhas/chaves em strings (que são imutáveis e permanecem no heap). |
| **Binary Patching** | Modificação do executável para alterar comportamento | Code signing + verificação de integridade em runtime. |
| **Keylogging** | Interceptação de teclas digitadas | Considerar input protegido para campos de senha (APIs do SO). Educar usuários sobre segurança do ambiente. |
| **Privilege Escalation** | Exploração para obter privilégios elevados | Aplicação deve rodar com o mínimo de privilégios necessários. Nunca solicitar elevação (admin/root) para operações que não a necessitam. |
| **Local File Inclusion** | Inclusão de arquivos maliciosos via manipulação de caminhos | Validação de paths (ver seção 4.3). |

---

## 5. Qualidade de Código

### 5.1 Análise Estática (SAST)

A análise estática examina o código-fonte sem executá-lo para detectar vulnerabilidades, code smells, e violações de padrões.

**Ferramentas obrigatórias (NuGet packages):**

| Pacote | Finalidade |
|---|---|
| **Roslyn Analyzers** (built-in do SDK .NET) | Qualidade de código, design, performance, segurança. Ativados via `AnalysisLevel` e `AnalysisMode` no `.csproj`. |
| **SonarAnalyzer.CSharp** | Regras adicionais de qualidade e segurança (SQL injection, XSS, CSRF, crypto fraca, hardcoded secrets). Desenvolvido pela mesma equipe do SonarQube. |
| **Roslynator** | 190+ analyzers para estilo, refactoring e boas práticas em C#. |
| **SecurityCodeScan** | Scanner de segurança dedicado para .NET com análise de fluxo de dados (taint analysis). Detecta SQL injection, XSS, XXE, Open Redirect, hardcoded credentials, crypto fraca. |
| **Microsoft.CodeAnalysis.BannedApiAnalyzers** | Permite proibir explicitamente APIs inseguras (ex: `MD5.Create()`, `Process.Start()` sem validação). |

**Configuração em `Directory.Build.props` (aplica a todos os projetos):**

```xml
<Project>
  <PropertyGroup>
    <AnalysisLevel>latest</AnalysisLevel>
    <AnalysisMode>All</AnalysisMode>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  </PropertyGroup>

  <ItemGroup Condition="'$(MSBuildProjectExtension)' != '.dcproj'">
    <PackageReference Include="SonarAnalyzer.CSharp" Version="*-*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Roslynator.Analyzers" Version="*-*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="SecurityCodeScan.VS2019" Version="*-*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.CodeAnalysis.BannedApiAnalyzers" Version="*-*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

**O que isso significa na prática:** o código não compila se contiver qualquer violação de qualidade ou segurança detectada por esses analyzers. Não existe "a gente corrige depois".

### 5.2 Formatação e Estilo (.editorconfig)

O arquivo `.editorconfig` na raiz do repositório garante formatação consistente independentemente do IDE utilizado.

**Regras mínimas obrigatórias:**
- Indentação: 4 espaços (sem tabs).
- Encoding: UTF-8 com BOM.
- Nova linha no final de cada arquivo.
- Uso de `var` quando o tipo é evidente (configurável).
- Modificador de acesso explícito sempre (nunca omitir `private`).
- `this.` somente quando necessário para desambiguação.
- Namespaces file-scoped (`namespace X;` em vez do bloco).
- Expressões corporais (expression bodies) quando o método tem uma única linha.
- Ordering: usings do System primeiro, depois os demais em ordem alfabética.

**Ferramenta de formatação:** `dotnet format` — executar antes de cada commit. Pode ser configurado como git hook.

### 5.3 Convenções de Nomenclatura

| Elemento | Convenção | Exemplo |
|---|---|---|
| Classes, records, structs | PascalCase | `UserRepository` |
| Interfaces | PascalCase com prefixo I | `IUserRepository` |
| Métodos | PascalCase | `GetActiveUsers()` |
| Propriedades públicas | PascalCase | `FirstName` |
| Parâmetros e variáveis locais | camelCase | `userName` |
| Campos privados | _camelCase (prefixo underscore) | `_userRepository` |
| Constantes | PascalCase | `MaxRetryAttempts` |
| Enums | PascalCase (singular) | `UserRole.Admin` |
| Async methods | Sufixo Async | `GetUsersAsync()` |
| Booleanos | Prefixo is/has/can/should | `isActive`, `hasPermission` |

### 5.4 Práticas de Código Obrigatórias

**Princípio geral:** código é lido muito mais vezes do que é escrito. Priorizar legibilidade e clareza sobre brevidade.

- **Uma responsabilidade por método:** se um método faz mais de uma coisa, divida-o.
- **Métodos curtos:** um método não deve ultrapassar 20-30 linhas. Se ultrapassar, provavelmente está fazendo coisas demais.
- **Classes coesas:** uma classe não deve ter mais de 200-300 linhas. Se ultrapassar, provavelmente tem responsabilidades demais.
- **Máximo de 3 parâmetros por método:** se precisa de mais, agrupar em um objeto (DTO, record).
- **Guard clauses:** validar pré-condições no início do método e retornar/lançar cedo, evitando indentação excessiva.
- **Evitar comentários óbvios:** o código deve ser auto-explicativo. Comentários devem explicar o *porquê*, não o *o que*.
- **XML documentation comments** em toda API pública (métodos, classes, interfaces públicos).
- **Nenhuma magic number/string:** toda constante com significado semântico deve ser nomeada.
- **Não usar `#region`:** regiões escondem código. Se a classe é grande demais para ser lida sem regiões, ela deve ser dividida.
- **Não fazer catch genérico sem re-throw:** `catch (Exception)` que engole a exceção esconde bugs.
- **Disposable pattern:** todo recurso que implementa `IDisposable` deve ser usado com `using` statement/declaration.

---

## 6. Estratégia de Testes

### 6.1 Pirâmide de Testes

A estratégia de testes segue a pirâmide de testes, proposta originalmente por Mike Cohn e amplamente validada pela comunidade de engenharia de software:

```
        /\
       /E2E\        ~10% — poucos, lentos, alto custo, alta confiança
      /──────\
     / Integr.\     ~20% — moderados, validam fronteiras entre componentes
    /──────────\
   / Unitários  \   ~70% — muitos, rápidos, baratos, feedback imediato
  /──────────────\
```

**Proporção alvo:** ~70% unitários, ~20% integração, ~10% E2E + Security. Estes percentuais são orientações, não regras rígidas — ajustar conforme a natureza da funcionalidade.

### 6.2 Testes Unitários

**O que são:** testam uma única unidade de código (método, classe) isolada de suas dependências.

**Framework:** xUnit.net (padrão da indústria .NET, usado pelo próprio .NET runtime).

**Biblioteca de mocking:** NSubstitute ou Moq para criar substitutos de interfaces injetadas.

**Biblioteca de assertions:** FluentAssertions para assertions legíveis e expressivas.

**Regras:**
- Toda classe de serviço/negócio deve ter cobertura de testes unitários.
- Padrão AAA (Arrange, Act, Assert) em todo teste.
- Um assert por teste (preferencialmente). Se necessário mais, eles devem validar aspectos da mesma operação.
- Nome do teste deve descrever o cenário e o resultado esperado: `MethodName_Scenario_ExpectedResult` (ex: `ValidatePassword_WhenTooShort_ReturnsFalse`).
- Testes devem ser determinísticos: nunca depender de data/hora atual, ordem de execução, estado global, ou rede.
- Para dependências de tempo: injetar uma abstração `ITimeProvider` (ou usar `TimeProvider` do .NET 8+).
- Para dependências de aleatoriedade: injetar uma abstração com seed controlável.
- **Nenhum** teste unitário deve acessar banco, filesystem, rede ou qualquer recurso externo.

```csharp
// Exemplo de teste unitário:
public class PasswordValidatorTests
{
    private readonly PasswordValidator _sut; // System Under Test

    public PasswordValidatorTests()
    {
        _sut = new PasswordValidator();
    }

    [Theory]
    [InlineData("abc", false)]          // Muito curta
    [InlineData("abcdefgh", false)]     // Sem número
    [InlineData("Abcdef1!", true)]      // Válida
    public void Validate_WithVariousPasswords_ReturnsExpectedResult(
        string password, bool expected)
    {
        // Act
        var result = _sut.Validate(password);

        // Assert
        result.IsValid.Should().Be(expected);
    }
}
```

### 6.3 Testes de Integração

**O que são:** testam a interação entre dois ou mais componentes reais (sem mocking da fronteira sendo testada).

**Cenários típicos:**
- Repositório + banco de dados SQLite real (in-memory ou em arquivo temporário).
- Serviço + HttpClient contra um servidor mock (WireMock.Net).
- Serviço + filesystem real (em diretório temporário).
- Pipeline completa de um módulo: input → processamento → persistência → leitura.

**Regras:**
- Cada teste de integração cria e destrói seu próprio estado. Nenhum teste depende de outro.
- Usar `IClassFixture<>` do xUnit para compartilhar setup pesado (ex: container de banco) entre testes da mesma classe.
- Banco de dados para testes: criar um SQLite in-memory por teste ou por classe de teste. Aplicar migrations automaticamente.
- Testar cenários de borda: dados corrompidos, banco locked, disco cheio (simulado), timeout de rede.

### 6.4 Testes End-to-End (E2E)

**O que são:** testam o sistema completo da perspectiva do usuário, simulando interações reais com a interface.

**Framework:** Avalonia possui APIs de teste de UI. Avaliar também o uso de Appium para automação cross-platform.

**Regras:**
- Cobrir apenas os fluxos críticos de negócio (os que, se falharem, comprometem a proposta de valor do sistema).
- Não tentar cobrir todas as combinações — isso é papel dos testes unitários e de integração.
- Testes E2E devem rodar em ambientes isolados com dados seedados e previsíveis.
- Tolerância a falhas intermitentes (flakiness): se um teste E2E é instável, corrigi-lo ou removê-lo. Testes flaky são piores que nenhum teste porque corroem a confiança na suite.

### 6.5 Testes de Segurança

**Tipos:**

| Tipo | Ferramenta/Abordagem | Quando rodar |
|---|---|---|
| **SAST (Análise Estática)** | Roslyn Analyzers + SecurityCodeScan (ver seção 5.1) | A cada build |
| **Dependency Scanning** | `dotnet list package --vulnerable` | A cada build no CI |
| **Fuzzing** | SharpFuzz, AFL.NET ou similar | Periodicamente (semanal/mensal) |
| **Penetration Testing Manual** | Checklist baseado no OWASP Desktop App Security Top 10 | Antes de cada release major |
| **Input Validation Testing** | Testes parametrizados com inputs maliciosos | Junto com testes unitários |

**Testes de input malicioso obrigatórios:**

Para toda funcionalidade que aceita input do usuário (campos de texto, importação de arquivos, etc.), criar testes parametrizados com:
- Strings de SQL injection (`' OR 1=1 --`, `'; DROP TABLE Users;--`)
- Strings de path traversal (`../../etc/passwd`, `..\..\Windows\System32`)
- Strings com caracteres especiais e Unicode (null bytes, caracteres de controle, emojis compostos, RTL override)
- Strings extremamente longas (testar limites de buffer)
- Valores numéricos nos limites (0, -1, int.MaxValue, int.MinValue)
- Payloads de XSS (`<script>alert(1)</script>`) — mesmo em desktop, se houver renderização de HTML/web content.

### 6.6 Mutation Testing

**O que é:** modifica intencionalmente o código-fonte (introduz "mutantes" — ex: troca `>` por `>=`, remove uma linha) e verifica se os testes existentes detectam a mudança. Se um mutante sobrevive (testes continuam passando), significa que os testes não cobrem aquele comportamento.

**Ferramenta:** Stryker.NET

**Quando usar:** periodicamente (após implementação de uma feature e sua suite de testes) para validar que os testes realmente testam o que deveriam testar. Uma cobertura de código de 100% não significa nada se os testes não verificam os resultados.

### 6.7 Cobertura de Código

**Ferramenta:** Coverlet (integrado com `dotnet test`).

**Meta:** 
- Mínimo de 80% de cobertura de linha para o projeto como um todo.
- 100% de cobertura de branch para código de segurança (criptografia, autenticação, autorização, validação de input).
- Cobertura de código é uma métrica necessária mas não suficiente. Um teste que executa uma linha sem verificar o resultado conta como cobertura mas não valida nada.

**Execução:**

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Gerar relatório legível com `reportgenerator`.

---

## 7. Tratamento de Erros

### 7.1 Filosofia

- **Exceções são para situações excepcionais** — falhas de rede, corrupção de dados, bugs no código. Não usar exceções para controle de fluxo (ex: "usuário não encontrado" não é uma exceção).
- **Result Pattern** para operações que podem falhar como parte do fluxo normal:

```csharp
public sealed record Result<T>
{
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(T value) { Value = value; }
    private Result(Error error) { Error = error; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}

public sealed record Error(string Code, string Message);
```

### 7.2 Tratamento Global de Exceções

- Configurar um handler global de exceções não capturadas (`AppDomain.CurrentDomain.UnhandledException` e equivalente do Avalonia).
- Este handler deve: registrar a exceção completa (com stack trace) no log; exibir uma mensagem genérica e amigável ao usuário (sem expor detalhes técnicos); salvar estado crítico se possível; nunca expor stack traces, nomes de tabelas, queries SQL ou paths internos ao usuário.
- Toda exceção capturada em camadas intermediárias deve ser logada com contexto (operação sendo executada, parâmetros não sensíveis).

### 7.3 Categorização de Erros

| Categoria | Exemplo | Ação |
|---|---|---|
| **Erro de validação** | Input inválido do usuário | Retornar mensagem clara indicando o problema. Não logar como erro. |
| **Erro de negócio** | Operação não permitida no estado atual | Retornar via Result Pattern. Logar como warning. |
| **Erro de infraestrutura recuperável** | Timeout de rede, API indisponível | Retry com backoff exponencial. Logar como warning. Informar o usuário com possibilidade de retry manual. |
| **Erro de infraestrutura não recuperável** | Banco corrompido, disco cheio | Logar como error/critical. Informar o usuário e sugerir ações (ex: "libere espaço em disco"). |
| **Erro inesperado (bug)** | NullReferenceException, IndexOutOfRange | Logar como critical com stack trace completo. Capturar via handler global. |

---

## 8. Observabilidade

### 8.1 Logging Estruturado

**Framework:** `Microsoft.Extensions.Logging` com **Serilog** como provider.

**Por que logging estruturado:** logs em texto plano são difíceis de pesquisar, filtrar e analisar. Logging estruturado registra cada evento como um objeto com propriedades nomeadas.

```csharp
// ERRADO — log não estruturado:
_logger.LogInformation($"User {userId} logged in at {DateTime.Now}");

// CORRETO — log estruturado:
_logger.LogInformation("User logged in. UserId={UserId}", userId);
```

No exemplo correto, `UserId` é uma propriedade pesquisável, não um texto incorporado em uma string.

**Sinks (destinos) de log:**
- **Arquivo local** (rotacionado por tamanho/data) — sempre ativo.
- **Console** — apenas em modo de desenvolvimento.
- **Eventualmente:** integração com serviço de monitoramento se/quando houver.

**Níveis de log e quando usar:**

| Nível | Quando usar | Exemplo |
|---|---|---|
| **Verbose/Trace** | Detalhamento extremo, apenas para debugging ativo | "Entering method X with params Y" |
| **Debug** | Informações úteis para diagnóstico durante desenvolvimento | "Cache miss for key X, fetching from DB" |
| **Information** | Eventos normais significativos do sistema | "User logged in", "Sync completed" |
| **Warning** | Situações anômalas que não impediram a operação | "API retry attempt 2/3", "Deprecated feature used" |
| **Error** | Falha em uma operação que deveria ter sucedido | "Failed to save record: DB constraint violation" |
| **Fatal/Critical** | Falha que compromete o funcionamento do sistema | "Database file corrupted", "Unhandled exception" |

**Regras:**
- Builds de produção/release: nível mínimo Information.
- Builds de debug: nível mínimo Debug.
- **Nunca** logar dados sensíveis (senhas, tokens, dados pessoais, queries SQL com valores de parâmetros).
- Todo log de erro deve incluir: timestamp, nível, operação/contexto, mensagem, exceção (se houver), correlation ID (se aplicável).

### 8.2 Health Checks e Auto-Diagnóstico

A aplicação deve ser capaz de diagnosticar seu próprio estado de saúde:

- **Verificação de integridade do banco** na inicialização (`PRAGMA integrity_check`).
- **Verificação de espaço em disco** disponível antes de operações de escrita significativas.
- **Verificação de conectividade** com serviços externos (APIs) — com timeout adequado e sem bloquear a UI.
- **Verificação de versão** de dependências internas e externas.
- **Verificação de permissões** de arquivo/diretório necessárias.

Em caso de falha em qualquer verificação: logar o problema, notificar o usuário de forma não intrusiva, e degradar gracefully (funcionalidades que dependem do recurso com problema ficam indisponíveis, mas o resto do sistema continua operando).

### 8.3 Telemetria e Métricas de Performance

**Coletar métricas internas (mesmo que apenas para log):**

- Tempo de inicialização da aplicação.
- Tempo de resposta de operações significativas (queries pesadas, sincronizações, importações).
- Uso de memória do processo.
- Tamanho do banco de dados.
- Contagem de erros por categoria.
- Quantidade de retry em operações de rede.

**Framework:** `System.Diagnostics.Metrics` (API nativa do .NET) para instrumentação. Armazenar métricas no banco local para auto-análise.

---

## 9. Performance e Otimização de Recursos

### 9.1 Princípio Fundamental

**Não otimizar prematuramente.** Escrever código correto e legível primeiro. Otimizar apenas quando houver evidência (medição) de que um trecho é um gargalo.

"Medir antes de otimizar" — toda otimização deve ser precedida por profiling e seguida por medição para verificar que a mudança teve o efeito desejado.

### 9.2 Memória

- Usar `Span<T>` e `Memory<T>` para operações de leitura/escrita em buffers sem alocações no heap.
- Preferir `ArrayPool<T>.Shared.Rent()` para buffers temporários grandes em vez de `new byte[]`.
- Usar `StringBuilder` para concatenação de strings em loops (nunca `+=` em loops).
- Implementar `IDisposable` corretamente em classes que possuem recursos não gerenciados.
- Para processamento de arquivos grandes: usar streaming (read/write incremental) em vez de carregar o arquivo inteiro na memória.
- Monitorar alocações com `dotnet-counters` e `dotnet-trace` durante o desenvolvimento.

### 9.3 CPU

- Operações pesadas (processamento de arquivos, criptografia, queries complexas) devem rodar em threads de background (`Task.Run`), nunca na UI thread.
- Usar `async/await` corretamente para I/O bound operations. Não criar tasks desnecessárias para operações que já são assíncronas.
- Evitar `ConfigureAwait(false)` em código de UI (Avalonia precisa da UI thread). Usar em código de infraestrutura/serviço.
- Para operações canceláveis: propagar `CancellationToken` em toda a cadeia de chamadas.

### 9.4 Disco (I/O)

- Agrupar operações de escrita no banco em transações. Cada transação individual em SQLite envolve um fsync, que é caro. Agrupar 100 operações em uma transação é ordens de magnitude mais rápido que 100 transações individuais.
- Configurar o SQLite com `PRAGMA journal_mode=WAL` (Write-Ahead Logging) para melhor performance de leitura concorrente com escrita.
- Configurar `PRAGMA synchronous=NORMAL` (em vez de FULL) para balance entre performance e durabilidade. Para dados críticos, manter FULL.
- Criar índices no banco apenas para colunas que são frequentemente usadas em cláusulas WHERE, JOIN e ORDER BY. Índices desnecessários consomem espaço e tornam escritas mais lentas.

### 9.5 Rede

- Implementar retry com exponential backoff e jitter para chamadas a APIs externas.
- Implementar circuit breaker para evitar chamadas repetidas a um serviço que está down.
- Cachear respostas de APIs quando o conteúdo não muda frequentemente (respeitar cache headers).
- Timeout adequado para todas as chamadas de rede (não usar timeout infinito).
- Batch requests quando a API suportar (enviar N itens em uma chamada em vez de N chamadas de 1 item).
- Compressão (gzip/brotli) para requests e responses grandes.

### 9.6 Créditos e Uso de Serviços Terceiros

- Monitorar e logar o consumo de APIs terceiras (número de chamadas, dados transferidos).
- Implementar rate limiting no lado do cliente para respeitar limites da API e evitar custos inesperados.
- Cachear agressivamente respostas que permitem cache.
- Implementar degradação graciosa: se uma API paga está indisponível ou o limite foi atingido, o sistema deve informar o usuário e continuar operando com funcionalidades reduzidas.

---

## 10. Arquitetura Offline-First

### 10.1 Princípio

A aplicação funciona completamente offline para todas as funcionalidades locais. Funcionalidades que dependem de internet operam em modo assíncrono: a operação é enfileirada localmente e executada quando a conexão estiver disponível.

### 10.2 Padrão de Implementação

- **Detecção de conectividade:** verificar conectividade periodicamente (polling) e por eventos do sistema operacional. Não assumir que "conectado à rede" significa "tem internet" — verificar acesso real ao endpoint necessário.
- **Fila de operações pendentes:** operações que requerem internet são enfileiradas em uma tabela do banco local com status (pendente, em progresso, falha, concluído), timestamp, número de tentativas e dados necessários para retry.
- **Processamento da fila:** quando a conectividade é restaurada, processar a fila em ordem (FIFO), com retry e backoff para itens que falham.
- **Resolução de conflitos:** definir estratégia por funcionalidade. Em caso de dados que podem ter sido modificados durante o período offline, usar timestamps e regras de negócio para resolver conflitos (last-write-wins, merge, ou prompt ao usuário).
- **Indicação visual:** a interface deve indicar claramente ao usuário se o sistema está online ou offline e se há operações pendentes.

---

## 11. Controle de Versão e CI/CD

### 11.1 Git

- **Branching model:** Git Flow simplificado ou Trunk-Based Development (avaliar conforme tamanho da equipe).
- **Commits:** mensagens no formato Conventional Commits (`feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `chore:`, `security:`).
- **Nunca** commitar: segredos (senhas, API keys, certificados), binários compilados, arquivos de banco de dados, diretórios de output (bin/, obj/).
- `.gitignore` configurado adequadamente desde o primeiro commit.
- **Git hooks (pre-commit):** rodar `dotnet format --verify-no-changes` para garantir formatação antes do commit.

### 11.2 Pipeline de CI

A cada push/merge request, a pipeline deve executar (nesta ordem):

1. **Restore** — baixar dependências.
2. **Build** — compilar com analyzers habilitados. Falha se houver qualquer warning.
3. **Vulnerability scan** — `dotnet list package --vulnerable`. Falha se houver vulnerabilidades conhecidas.
4. **Testes unitários** — todos devem passar.
5. **Testes de integração** — todos devem passar.
6. **Cobertura de código** — gerar relatório. Falha se abaixo do threshold (80%).
7. **Testes E2E** — nos branches principais (main/develop) e antes de releases.
8. **Testes de segurança** — periodicamente (nightly ou semanal).
9. **Packaging** — gerar artefato instalável (via Velopack) com assinatura digital.

**Nenhum código chega ao branch principal se qualquer etapa de 1 a 6 falhar.**

---

## 12. Gerenciamento de Dependências

### 12.1 Critérios para Adoção de Dependência

Antes de adicionar qualquer pacote NuGet ao projeto, avaliar:

| Critério | O que verificar |
|---|---|
| **Necessidade** | É realmente necessário ou posso implementar com o que já existe no SDK/.NET? |
| **Licença** | É compatível com uso gratuito e não comercial? Preferir MIT, Apache 2.0, BSD. Evitar GPL (obriga derivados a serem GPL). |
| **Manutenção** | O projeto tem commits recentes? Issues são respondidas? Há releases regulares? |
| **Segurança** | Tem vulnerabilidades conhecidas? Verificar no NVD (National Vulnerability Database). |
| **Popularidade** | Quantos downloads? Usado por projetos relevantes? Comunidade ativa? |
| **Transitividade** | Quantas dependências transitivas ele traz? Cada uma é um ponto de risco adicional. |

### 12.2 Manutenção Contínua

- Revisar dependências desatualizadas mensalmente (`dotnet-outdated`).
- Atualizar para patch versions (x.y.**Z**) imediatamente (geralmente contêm apenas fixes de segurança/bugs).
- Para minor/major updates: avaliar changelog, breaking changes e testar antes de adotar.
- Manter um arquivo `DEPENDENCIES.md` listando cada dependência, sua versão, sua licença e o motivo de sua inclusão.

---

## 13. Importação e Exportação de Dados

### 13.1 Funcionalidade de Backup/Restore Offline

- O formato de exportação deve ser self-contained e versionado (incluir metadata sobre versão do schema).
- Exportação deve produzir um arquivo criptografado (AES-256) protegido por senha definida pelo usuário.
- Importação deve validar: integridade (hash), versão de schema (compatibilidade), e decriptação antes de aplicar.
- Implementar migrations de schema para suportar importação de backups criados em versões anteriores do sistema.
- Limitar tamanho máximo de arquivo aceito para importação.
- Processar arquivos de importação em um diretório temporário isolado, nunca diretamente no diretório de dados da aplicação.

---

## 14. Acessibilidade

Mesmo sendo um sistema para uso pessoal/restrito, boas práticas de acessibilidade melhoram a usabilidade para todos.

- Suportar navegação por teclado em toda a interface.
- Manter contraste adequado entre texto e fundo.
- Elementos interativos devem ter tamanho mínimo clicável/tocável.
- Não transmitir informações apenas por cor (usar também texto/ícones).
- Suportar AutomationProperties no Avalonia para leitores de tela.

---

## 15. Documentação

### 15.1 Código

- XML documentation comments (`///`) em toda API pública.
- README.md na raiz com: propósito do projeto, como configurar o ambiente de desenvolvimento, como buildar, como rodar testes, como gerar instalador.
- CONTRIBUTING.md com padrões de código e processo de contribuição.
- CHANGELOG.md com histórico de mudanças por versão (gerado automaticamente a partir de Conventional Commits).

### 15.2 Architecture Decision Records (ADRs)

Para cada decisão técnica significativa (ex: "por que Avalonia em vez de MAUI", "por que SQLite em vez de PostgreSQL"), criar um ADR no diretório `docs/adr/` com o formato:

```markdown
# ADR-001: Título da Decisão

## Status
Aceito | Proposto | Depreciado | Substituído por ADR-XXX

## Contexto
O que motivou essa decisão? Qual o problema?

## Decisão
O que foi decidido?

## Consequências
Positivas, negativas e trade-offs.

## Alternativas Consideradas
O que mais foi avaliado e por que foi descartado.
```

### 15.3 Documentação de Regras de Negócio

Conforme definido no escopo: regras de negócio são tratadas em documentos separados. Este documento técnico assume que as regras de negócio estão especificadas em outro lugar e se limita a definir *como* implementá-las de forma segura, testada e manutenível.

---

## 16. Checklist de Revisão de Código

Toda implementação deve ser verificada contra este checklist antes de ser considerada pronta:

**Funcionalidade:**
- [ ] Implementa o requisito conforme especificado nas regras de negócio.
- [ ] Cobre cenários de borda (inputs inválidos, estados inesperados, valores limite).
- [ ] Funciona offline (se aplicável à funcionalidade).

**Segurança:**
- [ ] Inputs do usuário são validados e sanitizados.
- [ ] Nenhum dado sensível é logado, exibido ao usuário em mensagens de erro, ou armazenado em texto plano.
- [ ] Queries ao banco são parametrizadas.
- [ ] Nenhum segredo está no código-fonte.
- [ ] Permissões de acesso são verificadas na camada de serviço (não apenas na UI).

**Qualidade:**
- [ ] O código compila sem warnings.
- [ ] Analyzers estáticos não reportam violações.
- [ ] Nomes são claros e seguem as convenções.
- [ ] Métodos são curtos e com responsabilidade única.
- [ ] Dependências são injetadas (não instanciadas com `new`).

**Testes:**
- [ ] Testes unitários cobrem os cenários normais e de borda.
- [ ] Testes de integração validam a interação com dependências externas.
- [ ] Testes de input malicioso foram incluídos para campos que aceitam entrada do usuário.
- [ ] Cobertura de código não diminuiu.

**Performance:**
- [ ] Operações pesadas rodam em background thread.
- [ ] Operações de banco são agrupadas em transações quando apropriado.
- [ ] Recursos descartáveis são liberados (`using`).
- [ ] Não há alocações desnecessárias em hot paths.

**Observabilidade:**
- [ ] Operações significativas são logadas com nível e contexto adequados.
- [ ] Erros são logados com detalhes suficientes para diagnóstico.
- [ ] Nenhum dado sensível aparece nos logs.

---

## Apêndice A: Ferramentas e Pacotes NuGet Recomendados

| Categoria | Pacote / Ferramenta | Finalidade |
|---|---|---|
| **UI Framework** | Avalonia UI | Interface cross-platform |
| **UI Components** | SukiUI ou Material.Avalonia | Componentes visuais |
| **MVVM** | CommunityToolkit.Mvvm | Redução de boilerplate MVVM |
| **ORM** | Microsoft.EntityFrameworkCore.Sqlite | Acesso ao banco via EF Core |
| **DB Encryption** | SQLitePCLRaw.bundle_e_sqlite3mc | Criptografia AES-256 para SQLite |
| **Logging** | Serilog + Serilog.Extensions.Logging + Serilog.Sinks.File | Logging estruturado em arquivo |
| **DI** | Microsoft.Extensions.DependencyInjection | Container de injeção de dependência |
| **HTTP** | System.Net.Http (built-in) + Polly | HTTP client com retry/circuit breaker |
| **Testing** | xUnit + FluentAssertions + NSubstitute | Framework de testes |
| **Coverage** | Coverlet | Cobertura de código |
| **Mutation Testing** | Stryker.NET | Validação de qualidade dos testes |
| **Analyzers** | SonarAnalyzer.CSharp + Roslynator + SecurityCodeScan | Análise estática |
| **Updates** | Velopack | Instalação e atualização automática |
| **Serialization** | System.Text.Json (built-in) | Serialização JSON performática |
| **Password Hashing** | Konscious.Security.Cryptography (Argon2) | Hash de senhas |
| **Resilience** | Polly / Microsoft.Extensions.Http.Resilience | Retry, circuit breaker, timeout |

---

## Apêndice B: APIs e Padrões Proibidos

A seguinte lista de APIs e padrões é **proibida** no projeto. O BannedApiAnalyzers deve ser configurado para reportar erros se qualquer uma for usada:

| API/Padrão Proibido | Motivo | Alternativa |
|---|---|---|
| `System.Random` para fins criptográficos | Previsível, não criptograficamente seguro | `RandomNumberGenerator` |
| `MD5`, `SHA1` para hashing de segurança | Algoritmos comprometidos | SHA-256, SHA-512, Argon2id |
| `DES`, `3DES`, `RC4` | Algoritmos fracos/quebrados | AES-256-GCM |
| Concatenação de strings para SQL | Vulnerável a SQL injection | Queries parametrizadas / ORM |
| `dynamic` keyword (exceto interop) | Elimina verificação de tipos em compile-time | Tipagem forte |
| `Thread.Sleep` | Bloqueia thread | `await Task.Delay()` |
| `GC.Collect()` | Quase nunca necessário e prejudica performance | Confiar no GC |
| `#pragma warning disable` sem justificativa | Esconde problemas | Corrigir o warning |
| `catch (Exception) { }` (engolir exceção) | Esconde bugs completamente | Log + re-throw ou tratamento específico |
| Hardcoded strings de conexão/senhas/keys | Exposição de segredos | Secure storage do SO / derivação de chaves |

---

## Apêndice C: Referências e Fundamentação

Este documento foi construído com base nos seguintes padrões, frameworks e corpos de conhecimento reconhecidos pela comunidade de engenharia de software:

1. **OWASP Desktop App Security Top 10 (2021)** — Framework de referência para segurança de aplicações desktop, mantido pela comunidade OWASP com base em dados de CVEs e validação por pares da comunidade de segurança. Disponível em: https://owasp.org/www-project-desktop-app-security-top-10/

2. **OWASP Top Ten (Web, 2021/2025)** — Complementar ao Desktop Top 10 para vulnerabilidades em comunicação com APIs e serviços web. Baseado em dados de mais de 500.000 aplicações analisadas por 13 contribuidores de segurança. Disponível em: https://owasp.org/www-project-top-ten/

3. **OWASP Desktop Application Security Verification Standard (DASVS)** — Padrão de verificação de segurança específico para aplicações thick-client, definindo controles adaptados a ambientes locais não confiáveis.

4. **NIST SP 800-132 (Recommendation for Password-Based Key Derivation)** — Padrão do National Institute of Standards and Technology para derivação de chaves a partir de senhas, fundamentando as escolhas de parâmetros de Argon2id e PBKDF2 neste documento.

5. **Martin Fowler — Testing Pyramid (2012)** — Conceito original da pirâmide de testes amplamente adotado pela indústria, documentado em "The Practical Test Pyramid" e validado em publicações como o livro "Continuous Delivery" (Humble & Farley, 2010).

6. **Robert C. Martin — Clean Architecture (2017)** — Princípios SOLID e arquitetura limpa que fundamentam as decisões de modularização e separação de responsabilidades.

7. **Eric Evans — Domain-Driven Design (2003)** — Conceitos de bounded contexts, value objects e separação de domínio que influenciam a organização modular do projeto.

8. **Microsoft .NET Security Guidelines** — Documentação oficial de práticas de segurança para .NET, incluindo Roslyn Analyzers, Data Protection API, e configuração de criptografia. Disponível em: https://learn.microsoft.com/en-us/dotnet/standard/security/

9. **SQLite Documentation — Security, Encryption, and WAL mode** — Documentação oficial do SQLite sobre configuração de performance e segurança. Disponível em: https://sqlite.org/docs.html

10. **Velopack Documentation** — Documentação do framework de distribuição e atualização automática. Disponível em: https://docs.velopack.io/

11. **Avalonia UI Documentation** — Documentação do framework de UI cross-platform. Disponível em: https://docs.avaloniaui.net/

---

*Este é um documento vivo. Deve ser atualizado sempre que uma nova decisão técnica for tomada, uma nova ameaça de segurança for identificada, ou um padrão for revisado pela indústria.*
