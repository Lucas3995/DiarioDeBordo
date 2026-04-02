# Copilot Instructions — DiarioDeBordo

## What This Project Is

Aplicação desktop nativa **offline-first** (C#/.NET 9 + Avalonia UI + PostgreSQL) para gestão pessoal de acervo de conteúdo e agregação de fontes externas sem dark patterns. Análogo ao Calibre — programa local, sem componente social, sem algoritmo de ranqueamento, sem scroll infinito. O código ainda não existe; este repositório contém apenas as especificações de design. O desenvolvimento começa pela **Etapa 0 (modelagem tática DDD)**.

Especificações em `especificacoes/`:
- `1 - definicao-de-dominio.md` — regras de negócio, invariantes, cenários de validação (Apêndice A)
- `2 - mapa-de-dominio.md` — classificação de subdomínios
- `3 - mapa-de-contexto.md` — bounded contexts, linguagem ubíqua, padrões de relacionamento
- `4 - plano-de-implementacao.md` — 10 etapas incrementais com walking skeleton
- `5 - technical-standards.md` — stack, arquitetura, segurança, padrões de código (fonte da verdade técnica)

Estado atual do projeto: `.planning/PROJECT.md` e `.planning/REQUIREMENTS.md`.

---

## Stack

- **Linguagem/Runtime:** C# / .NET 9 LTS
- **UI:** Avalonia UI + SukiUI, padrão MVVM via `CommunityToolkit.Mvvm`
- **Mensageria in-process:** MediatR (comunicação entre ViewModels e entre bounded contexts)
- **Banco:** PostgreSQL (porta 15432, bundled na instalação), acesso via EF Core + Npgsql
- **Empacotamento/atualização:** Velopack
- **Hash de senhas:** Argon2id (`Konscious.Security.Cryptography`)
- **Secure Storage:** DPAPI no Windows, `secret-tool` (libsecret) no Linux

---

## Estrutura de Solução

```
Solution/
├── src/
│   ├── DiarioDeBordo.Desktop/        # Entry point, DI, bootstrap Velopack
│   ├── DiarioDeBordo.Core/           # Interfaces, DTOs, Result<T>, eventos de domínio, value objects
│   ├── DiarioDeBordo.Infrastructure/ # DbContext, repositórios, HTTP clients, criptografia, secure storage
│   ├── DiarioDeBordo.UI/             # Views XAML compartilhadas, converters, recursos visuais
│   └── Modules/
│       ├── Module.Acervo/            # BC Principal — conteúdo, coletâneas, categorias, relações
│       ├── Module.Agregacao/         # BC Principal — feeds, agregador, persistência seletiva
│       ├── Module.Reproducao/        # BC Suporte — reprodutor, abertura externa, ganchos
│       ├── Module.IntegracaoExterna/ # BC Suporte — adaptadores RSS/YouTube (ACL)
│       ├── Module.Busca/             # BC Suporte — full-text search, filtros, operações em lote
│       ├── Module.Portabilidade/     # BC Suporte — exportação/importação
│       ├── Module.Identidade/        # BC Genérico — autenticação, roles, grupos
│       ├── Module.Preferencias/      # BC Genérico — temas, fontes, acessibilidade, uso saudável
│       └── Module.Shared/            # Transversais — PaginatedList<T>, fila offline
├── tests/
│   ├── Tests.Unit/
│   ├── Tests.Integration/            # Inclui testes up/down de migrations
│   ├── Tests.E2E/
│   ├── Tests.Security/               # Fuzzing, inputs maliciosos
│   ├── Tests.Performance/
│   ├── Tests.Contract/               # Validação de contratos entre módulos
│   └── Tests.Domain/                 # Cenários do Apêndice A da Definição de Domínio v3
├── installer/
├── docs/
│   ├── adr/                          # Architecture Decision Records
│   └── threat-model/
├── Directory.Build.props
├── .editorconfig
└── BannedSymbols.txt
```

---

## Regras de Dependência entre Módulos

**Regra absoluta:** nenhum módulo depende diretamente de outro módulo. Toda comunicação entre bounded contexts passa por:
1. **Interfaces em `DiarioDeBordo.Core`** — contratos definidos pelo Consumer (Customer/Supplier pattern)
2. **Eventos via MediatR** — para comunicação assíncrona (ex: `ItemFeedPersistidoNotification`)

Módulos só podem depender de `DiarioDeBordo.Core` e `Module.Shared`. Implementações concretas ficam em `DiarioDeBordo.Infrastructure` ou no próprio módulo.

Nenhum código fora de `DiarioDeBordo.Infrastructure` pode ter dependência direta no provider de banco.

---

## Convenções de Código

### Configuração obrigatória (`Directory.Build.props`)
```xml
<Nullable>enable</Nullable>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
<AnalysisLevel>latest</AnalysisLevel>
<AnalysisMode>All</AnalysisMode>
<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
<CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
```
O código não compila com warnings. Não há exceções.

### Idioma
- **Entidades de domínio:** português brasileiro (`Conteudo`, `Coletanea`, `FormatoMidia`, `UsuarioId`)
- **Termos técnicos do framework:** inglês (`async`, `Task`, `CancellationToken`, `IRepository`)
- **Interfaces:** prefixo `I` em inglês, conforme convenção .NET
- **Strings visíveis ao usuário:** nunca hardcoded — sempre em arquivos `.resx` (cultura `pt-BR`)

### Result Pattern — obrigatório em toda operação de serviço
```csharp
public sealed record Result<T>
{
    public T? Value { get; }
    public Erro? Error { get; }
    public AlertaUsoSaudavel? Alerta { get; init; }
    public bool IsSuccess => Error is null;
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Erro error) => new(error);
}
public sealed record Erro(string Codigo, string Mensagem);
```
Nunca lançar exceções para fluxos de negócio esperados. Exceções apenas para falhas genuinamente inesperadas.

### Acesso ao banco — regras invioláveis
- Queries parametrizadas exclusivamente (EF Core LINQ ou Dapper com parâmetros). String interpolation em SQL é proibido.
- Toda query que acessa dados do usuário **deve** incluir filtro por `usuario_id`.
- Senhas nunca em texto plano. Hash com Argon2id.
- Credenciais do banco no Secure Storage do SO — nunca em código ou config files.
- Toda alteração de schema via EF Core Migrations versionadas.

### Algoritmos proibidos (`BannedSymbols.txt`, erro de compilação)
MD5, SHA1, DES, 3DES, RC2, RC4, `System.Random` para fins criptográficos, `Rfc2898DeriveBytes` com defaults.

### Comparação de hashes
Sempre `CryptographicOperations.FixedTimeEquals()` — nunca `==` ou `SequenceEqual`.

---

## Padrões de Domínio Críticos

### Registro vs. Visão
- **Registros** (entidades EF Core com tabela): `Conteudo`, `Coletanea`, `Categoria`, `Relacao`, `Fonte`, `Progresso`, `Anotacao`, `Usuario`, configurações.
- **Visões** (computadas sob demanda, nunca persistidas): `FeedDeSubscricao`, `AgregadorConsolidado`, `ResultadoBusca`, `Dashboard`. Nunca criar `DbSet<T>` para visões.

```csharp
// PROIBIDO:
public DbSet<ItemFeed> ItensFeed { get; set; } // visão não tem tabela

// CORRETO:
public sealed record ItemFeedDto(string Titulo, string? Descricao, ...); // DTO efêmero
```

### Persistência seletiva no contexto Agregação
Um `ItemFeedDto` só vira `Conteudo` (registro) quando o usuário interagir explicitamente. A Agregação solicita ao Acervo via MediatR command:
```csharp
public sealed record PersistirItemFeedCommand(
    Guid UsuarioId, string Titulo, string? Descricao, string? UrlFonte,
    FormatoMidia Formato, Guid ColetaneaSubscricaoId
) : IRequest<Result<Guid>>;
```

### Paginação obrigatória
Todo componente de lista usa `PaginatedList<T>`. Nenhuma lista sem paginação é aceitável. Listas sem paginação falham em testes.

### Disclosure progressivo
Formulários de cadastro de conteúdo mostram apenas o título por padrão. Demais campos revelados sob demanda.

### Uso Saudável — restrições invioláveis em todos os contextos
- Sem scroll infinito
- Sem autoplay entre conteúdos
- Sem algoritmo de ranqueamento (apenas ordenação explícita: cronológica, alfabética, manual)
- `ItemFeedDto` não possui campos para métricas sociais (likes, views, comentários)
- Notificações push: não existem. Lembretes de tempo de uso são opt-in, desativados por padrão.

### Anti-Corruption Layer — Module.IntegracaoExterna
Quando uma API externa muda, apenas o adaptador correspondente é alterado. Nenhum outro módulo é afetado.

### Área admin
Para não-admins, a área admin **não deve existir** na interface — nenhum link, nenhuma indicação visual. Tentativas de acesso retornam para a tela inicial com erro genérico (sem revelar a existência da área).

---

## Segurança

- **SSRF:** rejeitar URLs que resolvam para endereços privados (10.x, 172.16-31.x, 192.168.x, 127.x, ::1). Validar após resolução DNS.
- **XML/RSS:** `DtdProcessing.Prohibit`. Limite de 5MB por payload. Timeout de 10s.
- **Process.Start:** whitelist de protocolos (`https`, `http`, `file`) para abertura externa de conteúdos.
- **Memória:** `CryptographicOperations.ZeroMemory()` após uso de dados sensíveis. Senhas em `byte[]`, nunca `string`.
- **Autorização:** verificação de permissão na **camada de serviço**, nunca apenas na UI.

---

## Testes

Cobertura obrigatória: ≥ 95% (unitário + integração + e2e), mantida continuamente. 100% dos invariantes de domínio cobertos.

Os cenários do **Apêndice A da Definição de Domínio v3** são o conjunto mínimo que deve estar coberto em `Tests.Domain/` antes de considerar qualquer fase concluída.

Migrations devem ter testes de up e down em `Tests.Integration/`.

---

## Plano de Implementação (10 Etapas)

| Etapa | Entrega |
|-------|---------|
| 0 | Modelagem tática DDD (sem código) |
| 1 | Walking skeleton: criar conteúdo com título, persistir, recuperar, exibir |
| 2 | CRUD completo de conteúdo, dashboard |
| 3 | Coletâneas, fontes com fallback, deduplicação |
| 4 | Adaptadores RSS e YouTube, integração externa |
| 5 | Subscrição, feed, agregador, persistência seletiva |
| 6 | Busca full-text, filtros combinados, operações em lote |
| 7 | Reprodutor interno, ganchos, abertura externa |
| 8 | Multi-usuário, autenticação, roles, área admin |
| 9 | Exportação/importação, portabilidade cross-OS |
| 10 | Intervenções de uso saudável (monitoramento, lembretes) |

Feature flags controlam cada etapa (ver `especificacoes/5 - technical-standards.md`, seção 3.5).

---

## ADRs e Decisões Já Tomadas

| ADR | Decisão |
|-----|---------|
| ADR-001 | Avalonia UI + SukiUI (único framework .NET com suporte nativo real a Linux e Windows via Skia) |
| ADR-002 | PostgreSQL bundled na porta 15432 (para não conflitar com instâncias existentes) |

Novas decisões arquiteturais relevantes devem gerar um ADR em `docs/adr/`.

O threat model deve ser criado antes de implementar as camadas de rede e persistência.

---

## GSD Workflow

Este projeto usa o framework **GSD** (Get Shit Done) via comandos `/gsd:*` no Claude. Os comandos estão em `.claude/commands/gsd/`. Use `/gsd:help` para a referência completa.

### Ciclo principal por fase

```
/gsd:discuss-phase <N>   → Coleta decisões de implementação (gera CONTEXT.md)
/gsd:plan-phase <N>      → Cria PLAN.md com pesquisa e verificação integradas
/gsd:execute-phase <N>   → Executa os planos em waves paralelas
/gsd:verify-work <N>     → UAT conversacional — valida o que foi construído
/gsd:ship <N>            → Cria PR e prepara para merge
```

### Ciclo de milestone

```
/gsd:new-milestone       → Inicia novo milestone (requirements → roadmap)
/gsd:audit-milestone     → Audita conclusão contra a intenção original
/gsd:complete-milestone <version>  → Arquiva milestone, taga git, prepara próxima versão
```

### Tarefas avulsas

| Comando | Uso |
|---------|-----|
| `/gsd:next` | Detecta o estado atual e avança automaticamente para o próximo passo |
| `/gsd:do <descrição>` | Rota linguagem natural para o comando GSD correto |
| `/gsd:quick` | Tarefa pequena com garantias GSD (commit atômico, rastreamento) |
| `/gsd:fast <descrição>` | Tarefa trivial inline — sem subagentes, sem PLAN.md |
| `/gsd:progress` | Mostra contexto atual e roteia para a próxima ação |
| `/gsd:debug <problema>` | Debug sistemático com estado persistente entre resets de contexto |

### Utilitários

| Comando | Uso |
|---------|-----|
| `/gsd:note <texto>` | Captura rápida de ideia (sem perguntas) |
| `/gsd:health` | Valida integridade do diretório `.planning/` |
| `/gsd:stats` | Estatísticas do projeto (fases, planos, requisitos, git) |
| `/gsd:session-report` | Gera SESSION_REPORT.md com resumo da sessão |
| `/gsd:cleanup` | Arquiva diretórios de fases concluídas |
| `/gsd:thread` | Contexto persistente cross-session para trabalho que não pertence a uma fase |

### Estado e artefatos do planejamento

- `.planning/PROJECT.md` — verdade do projeto (atualizado após cada transição de fase e milestone)
- `.planning/REQUIREMENTS.md` — requisitos rastreáveis por fase
- `.planning/config.json` — configuração do GSD (modo, granularidade, estratégia de branches)
- `.planning/phases/<N>/` — PLAN.md, CONTEXT.md, VERIFICATION.md de cada fase
- Após cada fase: `.planning/PROJECT.md` deve ser atualizado (requisitos validados/invalidados, novas decisões)
