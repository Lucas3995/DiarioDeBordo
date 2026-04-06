<div align="center">

# 📓 Diário de Bordo

**Gestão pessoal de acervo e agregação de fontes externas — sem dark patterns.**

[![CI](https://github.com/Lucas3995/DiarioDeBordo/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Lucas3995/DiarioDeBordo/actions/workflows/ci.yml)
[![Status](https://img.shields.io/badge/status-em%20desenvolvimento-orange?style=flat-square)](https://github.com)
[![Fase atual](https://img.shields.io/badge/fase-2%20Walking%20Skeleton-blue?style=flat-square)](#roadmap)
[![Plataforma](https://img.shields.io/badge/plataforma-Windows%20%7C%20Linux-informational?style=flat-square)](#stack)
[![.NET](https://img.shields.io/badge/.NET-9%20LTS-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com)
[![Licença](https://img.shields.io/badge/licen%C3%A7a-a%20definir-lightgrey?style=flat-square)](#)

</div>

---

## O que é isso?

Diário de Bordo é um **programa de computador nativo**, análogo ao Calibre. Roda localmente na sua máquina. Não tem servidor, não tem conta, não tem feed algorítmico.

Ele resolve dois problemas:

| Problema | Solução |
|---|---|
| Suas leituras, estudos e descobertas estão espalhados em abas abertas, bloco de notas e memória | Um acervo central onde você registra, anota, avalia e acompanha progresso de qualquer conteúdo |
| Você abre o YouTube ou o Twitter para ver um criador específico e 40 minutos depois ainda está lá | Um agregador de feeds controlado por você — sem algoritmo, sem scroll infinito, sem autoplay |

Os dois pilares funcionam de forma independente. Você pode usar só o acervo (totalmente offline), só o agregador, ou os dois integrados.

---

## Por que mais uma ferramenta?

> *"As plataformas digitais otimizam para engajamento da plataforma, não para o bem-estar de quem as utiliza."*

Diário de Bordo inverte essa lógica. Cada decisão de design é guiada por **três princípios invioláveis:**

- 🧭 **Agência do usuário** — você decide o que ver, quando, como e quanto. Nada é imposto.
- 🌿 **Uso saudável por design** — sem scroll infinito, sem autoplay, sem algoritmo de ranqueamento, sem métricas sociais.
- 🧠 **Economia cognitiva** — formulários com disclosure progressivo, complexidade revelada sob demanda, nunca jogada na sua cara.

---

## Funcionalidades planejadas

### 📚 Acervo pessoal
- Registre qualquer conteúdo: vídeos, livros, artigos, podcasts, cadernos físicos, jogos — o que for
- Anotações, notas pessoais, classificações (1–5), progresso rastreado por tipo de mídia
- Coletâneas ordenadas ou não, com relações entre conteúdos (sequência, derivado de, baseado em)
- Categorias hierárquicas com detecção automática de ciclo
- Funciona **100% offline**

### 📡 Agregador de fontes
- Subscreva canais RSS e canais do YouTube
- Feed por fonte ou agregado — ordenação cronológica, alfabética ou manual. Nunca algorítmica.
- Um item do feed só vai para o acervo quando **você** decidir — sem persistência automática
- Comportamento gracioso offline: o que já foi interagido está disponível

### 🔍 Busca e operações em lote
- Busca full-text com filtros combinados
- Operações em lote sobre resultados de busca

### 🔒 Privacidade e segurança by default
- Sem rastreamento, sem telemetria, sem conta obrigatória
- Senhas com Argon2id, credenciais no Secure Storage do SO (DPAPI / libsecret)
- Queries sempre parametrizadas, sempre filtradas por `usuario_id`
- Threat model STRIDE completo antes de qualquer camada de rede

### 🖥️ Multi-usuário local
- Vários perfis no mesmo computador com dados completamente independentes
- Área admin invisível para não-admins em todas as camadas — não só na UI

---

## Stack

| Camada | Tecnologia |
|---|---|
| Linguagem / Runtime | C# / .NET 9 LTS |
| UI | Avalonia UI + SukiUI (MVVM via CommunityToolkit.Mvvm) |
| Mensageria in-process | MediatR |
| Banco de dados | PostgreSQL bundled (porta 15432) via EF Core + Npgsql |
| Hash de senhas | Argon2id (Konscious.Security.Cryptography) |
| Secure Storage | DPAPI (Windows) / libsecret (Linux) |
| Empacotamento / atualização | Velopack |

> **Por que Avalonia?** É o único framework .NET com suporte nativo real a Windows e Linux via Skia, sem Electron, sem WebView. ([ADR-001](docs/adr/ADR-001-ui-framework.md))

> **Por que PostgreSQL bundled?** Confiabilidade de banco relacional com zero configuração para o usuário final. Porta 15432 para não conflitar com instâncias existentes. ([ADR-002](docs/adr/ADR-002-banco-de-dados.md))

---

## Arquitetura

Monolito modular com bounded contexts via DDD. Nenhum módulo depende diretamente de outro — toda comunicação passa por interfaces em `Core` ou eventos via MediatR.

```
Solution/
├── src/
│   ├── DiarioDeBordo.Desktop/        # Entry point, DI, bootstrap
│   ├── DiarioDeBordo.Core/           # Interfaces, DTOs, Result<T>, eventos
│   ├── DiarioDeBordo.Infrastructure/ # DbContext, repositórios, HTTP, criptografia
│   ├── DiarioDeBordo.UI/             # Views XAML, converters, recursos visuais
│   └── Modules/
│       ├── Module.Acervo/            # BC Principal — conteúdo, coletâneas, categorias
│       ├── Module.Agregacao/         # BC Principal — feeds, agregador, persistência seletiva
│       ├── Module.IntegracaoExterna/ # BC Suporte — adaptadores RSS/YouTube (ACL)
│       ├── Module.Busca/             # BC Suporte — full-text search, filtros, lote
│       ├── Module.Reproducao/        # BC Suporte — reprodutor, abertura externa
│       ├── Module.Portabilidade/     # BC Suporte — exportação/importação
│       ├── Module.Identidade/        # BC Genérico — autenticação, roles, grupos
│       ├── Module.Preferencias/      # BC Genérico — temas, acessibilidade, uso saudável
│       └── Module.Shared/            # Transversais — PaginatedList<T>, fila offline
└── tests/
    ├── Tests.Unit/
    ├── Tests.Integration/            # Inclui testes up/down de migrations
    ├── Tests.Domain/                 # Cenários do Apêndice A da Definição de Domínio v3
    ├── Tests.Security/               # Fuzzing, inputs maliciosos
    ├── Tests.Performance/
    ├── Tests.Contract/               # Contratos entre módulos
    └── Tests.E2E/
```

**Regras invioláveis de dependência:**
- Módulos só dependem de `Core` e `Module.Shared`
- Nenhum código fora de `Infrastructure` tem dependência direta no provider de banco
- Comunicação entre BCs: interfaces em `Core` (síncrono) ou eventos MediatR (assíncrono)

---

## Roadmap

| # | Fase | Status |
|---|---|---|
| 0 | Modelagem tática DDD | ✅ Concluída |
| **1** | **Walking skeleton** — criar conteúdo, persistir, exibir | 🔄 **Em planejamento** |
| 2 | CRUD completo de conteúdo, dashboard | ⏳ Pendente |
| 3 | Coletâneas, fontes, deduplicação | ⏳ Pendente |
| 4 | Adaptadores RSS e YouTube | ⏳ Pendente |
| 5 | Subscrição, feed, agregador, persistência seletiva | ⏳ Pendente |
| 6 | Busca full-text, filtros combinados, operações em lote | ⏳ Pendente |
| 7 | Reprodutor interno, abertura externa | ⏳ Pendente |
| 8 | Multi-usuário, autenticação, roles, área admin | ⏳ Pendente |
| 9 | Exportação/importação, portabilidade cross-OS | ⏳ Pendente |
| 10 | Intervenções de uso saudável (monitoramento, lembretes opt-in) | ⏳ Pendente |

---

## Especificações

A documentação de design vive em [`especificacoes/`](especificacoes/):

| Documento | Conteúdo |
|---|---|
| [1 — Definição de Domínio](especificacoes/1%20%20-%20definicao-de-dominio.md) | Regras de negócio, invariantes, cenários de validação (Apêndice A) |
| [2 — Mapa de Domínio](especificacoes/2%20-%20mapa-de-dominio.md) | Classificação de subdomínios |
| [3 — Mapa de Contexto](especificacoes/3%20-%20mapa-de-contexto.md) | Bounded contexts, linguagem ubíqua, padrões de relacionamento |
| [4 — Plano de Implementação](especificacoes/4%20-%20plano-de-implementacao.md) | 10 fases incrementais com walking skeleton |
| [5 — Padrões Técnicos](especificacoes/5%20-%20technical-standards.md) | Stack, arquitetura, segurança, padrões de código (fonte da verdade técnica) |

A documentação tática (ADRs, modelos de domínio, threat model) vive em [`docs/`](docs/) após a Fase 1.

---

## Padrões de qualidade

- **Zero warnings** — `TreatWarningsAsErrors=true`, `AnalysisMode=All`
- **Nullable habilitado** — sem `null` surpresa
- **Cobertura ≥ 95%** — unitário + integração + e2e, mantida continuamente
- **100% dos invariantes de domínio** cobertos em `Tests.Domain/`
- **Sem algoritmos proibidos** — MD5, SHA1, DES, `System.Random` para fins criptográficos são erros de compilação (`BannedSymbols.txt`)
- **Comparação de hashes** sempre com `CryptographicOperations.FixedTimeEquals()`
- **Strings visíveis ao usuário** nunca hardcoded — sempre em `.resx` (cultura `pt-BR`)

---

<div align="center">

*Feito para uso próprio, com cuidado.*

</div>
