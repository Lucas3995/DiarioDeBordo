# Phase 1: Modelagem Tática DDD — Research

**Phase:** 01 — Modelagem Tática DDD
**Date:** 2026-04-02
**Status:** Complete

---

## Executive Summary

A Phase 1 é uma fase de documentação pura — sem código de produção. Os artefatos produzidos
são a base de contrato para todas as fases subsequentes, especialmente o walking skeleton (Phase 2).

Os maiores riscos desta fase são:
1. **Fronteiras de agregado mal definidas** — especialmente no BC Acervo, onde `Conteudo` tem
   muitos relacionamentos. Aggregate boundary errado força refatorações custosas na Phase 2+.
2. **Contrato Acervo ↔ Agregação ambíguo** — a persistência seletiva é a fronteira conceitual
   mais importante do sistema. Se o comando `PersistirItemFeedCommand` e seus invariantes não
   forem documentados com precisão, o walking skeleton não poderá ser implementado corretamente.
3. **Threat model incompleto antes das camadas de rede** — o Padrões Técnicos v4 já tem um
   Apêndice C com esboço STRIDE. A Phase 1 expande isso em DFD + tabela + mitigações rastreáveis.

---

## Dimension 1: Domain

### 1.1 Aggregate Boundaries — BC Acervo

**Fundamento:** Evans (2003) — o agregado é a unidade de consistência transacional. Tudo dentro
de um agregado muda junto em uma transação. Cruzar fronteiras de agregado exige eventos ou
referências por ID.

**Descoberta crítica:** O Padrões Técnicos v4 (seção 6.1) já apresenta `Conteudo` com todas as
suas entidades relacionadas — mas sem declarar explicitamente os agregados. A modelagem tática
precisa formalizar essas fronteiras.

#### Agregado: Conteudo (Aggregate Root)

`Conteudo` é o agregado raiz principal do BC Acervo. Inclui como parte do mesmo agregado:

| Entidade / Value Object | Motivo da inclusão no agregado |
|---|---|
| `Fonte` | Invariante: a ordem de prioridade das fontes pertence ao conteúdo. Não existe fora dele. |
| `Progresso` (value object) | Invariante: progresso é global ao conteúdo. Não existe independentemente. |
| `HistoricoAcao` | Invariante de auditoria: criado e gerenciado exclusivamente pelo próprio conteúdo. |
| `ImagemConteudo` | Invariante: imagem principal depende do conjunto de imagens do conteúdo. |
| `Gancho` | Relacionado ao conteúdo específico; sem identidade fora dele. |

**Fora do agregado Conteudo** (referenciados por ID):

| Entidade | Justificativa |
|---|---|
| `Categoria` | Entidade independente com ciclo de vida próprio. Compartilhada entre conteúdos. |
| `ConteudoCategoria` | Tabela de junção — acesso via consulta, não via agregado. |
| `RelacaoConteudo` | Bidirecionalidade exige que a relação seja gerenciada fora dos dois agregados. |
| `ConteudoColetanea` + anotação contextual | A associação entre conteúdo e coletânea pertence à fronteira de `Coletanea`. |

**Invariantes do agregado Conteudo:**
- `Titulo` nunca nulo ou vazio.
- `TipoColetanea` nulo quando `Papel == Item`; obrigatório quando `Papel == Coletanea`.
- `Nota` no intervalo [0, 10] quando presente.
- Máximo 20 imagens por conteúdo; máximo 10MB por imagem.
- Apenas uma imagem marcada como `Principal` quando houver ≥ 1.
- `Fontes` com `Prioridade` única por conteúdo (sem duas fontes com a mesma prioridade).

#### Agregado: Coletanea (Aggregate Root)

`Coletanea` é implementada como um `Conteudo` com `Papel == Coletanea` — mesma entidade, papel
diferente (Definição de Domínio v3, seção 4.1). A distinção tática relevante é que coletâneas
têm responsabilidades adicionais:

| Entidade / Value Object | Motivo da inclusão no agregado |
|---|---|
| `ConteudoColetanea` (associação) | Pertence ao ciclo de vida da coletânea. Gerenciada pela coletânea. |
| `AnotacaoContextual` (no ConteudoColetanea) | Pertence à relação, não ao conteúdo nem à coletânea isoladamente. |
| `OrdemItem` (value object) | Relevante apenas para coletâneas Guiadas. |

**Invariantes do agregado Coletanea:**
- Não pode conter a si mesma (diretamente ou transitivamente) — proteção contra ciclos usando
  DFS/Tarjan antes de toda adição de item (Tarjan, 1972, *SIAM J. on Computing*).
- Coletânea Guiada: itens têm ordem explícita sem lacunas ou duplicações de posição.
- `TipoColetanea` imutável após criação (mudança de tipo é uma operação de domínio explícita,
  não edição de campo).

**Detecção de ciclos — algoritmo:**
```
Ao adicionar ColetaneaFilha à ColetaneaMae:
1. Executar DFS a partir de ColetaneaFilha
2. Se ColetaneaMae for encontrada no grafo descendente → CICLO → rejeitar
3. Complexidade: O(V+E) — linear no número de coletâneas e relações
```

#### Agregado: Categoria (Aggregate Root)

Entidade com ciclo de vida independente. Compartilhada entre múltiplos conteúdos.

**Invariante:** Categorias são case-insensitive — `"Romance"` e `"romance"` são a mesma categoria.
Autocompletar e não-duplicação controlados no nível do agregado.

#### Value Objects identificados no BC Acervo

| Value Object | Campos | Contexto |
|---|---|---|
| `Progresso` | `Estado`, `PosicaoAtual`, `HistoricoConsumo[]`, `NotaManual` | Embutido em `Conteudo` |
| `OrdemItem` | `Posicao: int` | Embutido em `ConteudoColetanea` para Guiadas |
| `PaginacaoParams` | `Pagina`, `ItensPorPagina` | Transversal (Module.Shared) |

#### Repositórios do BC Acervo

```
IConteudoRepository
  - AdicionarAsync(Conteudo, CancellationToken)
  - ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken)
  - AtualizarAsync(Conteudo, CancellationToken)
  - RemoverAsync(Guid id, Guid usuarioId, CancellationToken)
  - BuscarPorUrlFonteAsync(Guid usuarioId, string urlNormalizada, CancellationToken)
  - BuscarPorIdentificadorFonteAsync(Guid usuarioId, string plataforma, string id, CancellationToken)

IColetaneaRepository
  - AdicionarAsync(Conteudo coletanea, CancellationToken)
  - ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken)
  - ObterDescendentesAsync(Guid coletaneaId, Guid usuarioId, CancellationToken) // para detecção de ciclos
  - AtualizarAsync(Conteudo coletanea, CancellationToken)

ICategoriaRepository
  - ObterOuCriarAsync(Guid usuarioId, string nome, CancellationToken) // garante não-duplicação
  - ListarComAutocompletarAsync(Guid usuarioId, string prefixo, CancellationToken)
```

**Regra transversal:** Toda query que acessa dados do usuário **inclui filtro por `usuarioId`** —
invariante de segurança (Padrões Técnicos v4, seção 4.6).

---

### 1.2 Aggregate Boundaries — BC Agregação

**Descoberta central:** O BC Agregação não tem entidades persistidas próprias — opera sobre
**visões efêmeras** (`ItemFeedDto`) e **delegações** ao BC Acervo.

| Conceito | Natureza | Onde vive |
|---|---|---|
| `FeedDeSubscricao` | Visão efêmera | Computada em memória, nunca em banco |
| `AgregadorConsolidado` | Visão efêmera | Computada em memória, nunca em banco |
| `ItemFeedDto` | DTO imutável | Em memória durante o ciclo de request |
| `FiltroAgregador` | Value Object | Parâmetro de entrada, não persistido |
| `EstadoOfflineSubscricao` | Value Object | Indicador de incompletude, não persistido |

**Implicação tática:** O BC Agregação não tem repositórios próprios. Sua única escrita ao banco é
via delegação ao Acervo através do `PersistirItemFeedCommand`.

**Repositórios consumidos (interfaces definidas em Core, implementadas em Acervo/Infraestrutura):**

```
ISubscricaoFontesProvider  // lê fontes da coletânea Subscrição (implementado em Acervo)
IConteudoRegistradoProvider // verifica se um ItemFeed já foi persistido (implementado em Acervo)
```

---

### 1.3 Eventos de Domínio — Integração Acervo ↔ Agregação

**Padrão:** Interfaces em `DiarioDeBordo.Core` + MediatR para comunicação entre BCs.

#### Command: PersistirItemFeedCommand

```csharp
// Definido em DiarioDeBordo.Core — BC Agregação envia, BC Acervo trata:
public sealed record PersistirItemFeedCommand(
    Guid UsuarioId,
    string Titulo,
    string? Descricao,
    string? UrlFonte,
    string? ThumbnailUrl,
    FormatoMidia Formato,
    Guid ColetaneaSubscricaoId
) : IRequest<Result<Guid>>;
// Retorna: Guid do Conteudo criado ou existente (deduplicação transparente)
```

**Invariantes do handler:**
1. Verificar deduplicação antes de criar (por URL de fonte, por identificador de plataforma).
2. Se duplicata encontrada: associar à coletânea e retornar o ID existente.
3. Se novo: criar `Conteudo` com os dados do `ItemFeedDto`.
4. Registrar `HistoricoAcao.Criacao` no novo conteúdo.
5. Retornar `Result<Guid>` — nunca lançar exceção para fluxos de negócio esperados.

#### Notification: ItemFeedPersistidoNotification

```csharp
// Publicado pelo handler de PersistirItemFeedCommand após persistência bem-sucedida:
public sealed record ItemFeedPersistidoNotification(
    Guid ConteudoId,
    string Titulo
) : INotification;
// Handlers: ViewModels do Acervo que precisam atualizar suas listas em tempo real
```

#### Notification: ProgressoAlteradoNotification

```csharp
// Publicado pelo BC Acervo quando progresso muda — consumido por Reprodução e UI:
public sealed record ProgressoAlteradoNotification(
    Guid ConteudoId,
    EstadoProgresso Estado,
    string? PosicaoAtual
) : INotification;
```

#### Interface: ISubscricaoFontesProvider

```csharp
// Definida em Core, implementada em Module.Acervo:
public interface ISubscricaoFontesProvider
{
    Task<IReadOnlyList<FonteSubscricao>> ObterFontesAsync(
        Guid usuarioId, Guid coletaneaSubscricaoId, CancellationToken ct);
}

public sealed record FonteSubscricao(
    Guid FonteId,
    string Tipo,          // "rss", "youtube", "identificador"
    string Valor,         // URL do feed, URL do canal, @ do criador
    string? Plataforma    // "youtube", "rss", "instagram"
);
```

#### Interface: IConteudoParaReproducaoProvider

```csharp
// Definida em Core, implementada em Module.Acervo — consumida por Module.Reproducao:
public interface IConteudoParaReproducaoProvider
{
    Task<ConteudoReproducaoDto?> ObterAsync(Guid id, Guid usuarioId, CancellationToken ct);
}

public sealed record ConteudoReproducaoDto(
    Guid Id, string Titulo, FormatoMidia Formato,
    IReadOnlyList<FonteOrdenada> FontesOrdenadas, IReadOnlyList<GanchoDto> Ganchos
);
```

---

### 1.4 Esboço dos BCs de Suporte

#### Module.Reprodução — Responsabilidades e contratos

| Recebe | Fornece |
|---|---|
| `ConteudoReproducaoDto` via `IConteudoParaReproducaoProvider` | `ProgressoAlteradoNotification` via MediatR |

Nenhuma entidade persistida própria. Utiliza o `Conteudo` via interface readonly.

**Eventos publicados:**
- `ReproducaoIniciadaNotification(ConteudoId, Posicao)` — para ofertar marcação de progresso
- `ReproducaoConcluidaNotification(ConteudoId)` — para ofertar marcação de conclusão

#### Module.IntegracaoExterna — Contratos

```csharp
// Implementado em Module.IntegracaoExterna, consumido por Module.Agregacao:
public interface IAdaptadorPlataforma
{
    bool SuportaPlataforma(string tipo);
    Task<Result<IReadOnlyList<ItemFeedDto>>> MontarFeedAsync(
        FonteSubscricao fonte, PaginacaoParams paginacao, CancellationToken ct);
    Task<Result<MetadadosExternosDto?>> ObterMetadadosAsync(
        string urlOuIdentificador, string plataforma, CancellationToken ct);
}
```

**Restrições de segurança invioláveis (Padrões Técnicos v4, seção 4.2):**
- `DtdProcessing.Prohibit` em todo XML/RSS
- Limite 5MB por payload, timeout 10s
- Rejeitar URLs que resolvam para IPs privados (SSRF prevention)
- Whitelist de protocolos: `https`, `http`, `file` apenas

#### Module.Busca — Interface de query

```csharp
// Consumida por ViewModels de busca:
public interface IBuscaConteudoService
{
    Task<Result<PaginatedList<ConteudoResumoDto>>> BuscarAsync(
        Guid usuarioId, FiltroBusca filtro, PaginacaoParams paginacao, CancellationToken ct);
    Task<Result<int>> ExecutarOperacaoEmLoteAsync(
        Guid usuarioId, IReadOnlyList<Guid> ids, OperacaoLote operacao, CancellationToken ct);
}
```

#### Module.Identidade — Contrato de autenticação

```csharp
// Definida em Core — consumida por todos os módulos que precisam do usuário atual:
public interface IUsuarioAutenticadoProvider
{
    Guid? UsuarioIdAtual { get; }
    Task<IReadOnlySet<Role>> RolesAtuais { get; }
}
```

**Invariante de segurança:** Área admin não existe para usuários sem role `Admin`. Qualquer
tentativa de acesso retorna comportamento genérico — sem indicação de que a área existe.

#### Module.Preferências — Contrato

```csharp
public interface IPreferenciasProvider
{
    Task<Tema> ObterTemaAsync(Guid usuarioId);
    Task<int> ObterItensPorPaginaAsync(Guid usuarioId);
    Task<bool> DisclosureProgressivoAtivo(Guid usuarioId);
    Task<ConfiguracaoUsoSaudavel> ObterUsoSaudavelAsync(Guid usuarioId);
}
```

#### Module.Portabilidade — Interfaces de exportação

```csharp
// Cada módulo com dados exportáveis implementa:
public interface IDadosExportaveisProvider
{
    Task<ExportacaoDto> ExportarAsync(Guid usuarioId, CancellationToken ct);
}
// Module.Acervo e Module.Preferencias implementam essa interface.
// Module.Portabilidade consome ambas para montar o pacote.
```

---

## Dimension 2: Architecture

### 2.1 Estrutura de Arquivos para Documentação

Baseado na decisão D-05 (CONTEXT.md) e confirmado pelos Padrões Técnicos v4:

```
docs/
├── domain/
│   ├── acervo.md              # Modelo tático completo do BC Acervo
│   ├── agregacao.md           # Modelo tático completo do BC Agregação
│   ├── reproducao-esboço.md   # Esboço do BC Reprodução
│   ├── integracao-externa-esboço.md
│   ├── busca-esboço.md
│   ├── portabilidade-esboço.md
│   ├── identidade-esboço.md
│   └── preferencias-esboço.md
├── adr/
│   ├── ADR-001-ui-framework.md      # Avalonia UI + SukiUI
│   ├── ADR-002-banco-de-dados.md    # PostgreSQL bundled porta 15432
│   ├── ADR-003-arquitetura.md       # Monolito modular com bounded contexts
│   ├── ADR-004-stack-tecnologica.md # C#/.NET 9, MediatR, EF Core, Velopack
│   └── ADR-005-segurança.md         # Argon2id, DPAPI/libsecret, BannedSymbols
└── threat-model/
    ├── overview.md            # Resumo e DFD nível 0
    ├── dfd-nivel-1.md         # DFDs por componente
    └── stride-table.md        # Tabela STRIDE com mitigações rastreáveis
```

### 2.2 Template de Documento de Bounded Context

Cada documento de BC segue a mesma estrutura para consistência:

```markdown
# BC [Nome] — Modelo Tático

**Classificação:** Principal / Suporte / Genérico
**Linguagem Ubíqua:** [termos definidos no Mapa de Contexto]

## Agregados

### [NomeAgregado] (Aggregate Root)
**Responsabilidade:** [O que este agregado protege]

#### Entidades
| Nome | Atributos | Invariantes |
|---|---|---|
| [Entidade] | campo: tipo | [invariante] |

#### Value Objects
| Nome | Campos | Contexto |
|---|---|---|

#### Eventos de Domínio Publicados
| Evento | Quando | Handlers |

#### Repositório
```csharp
public interface I[Nome]Repository { ... }
```

## Diagrama (Mermaid)

```mermaid
classDiagram
  class [AggregateRoot] { ... }
  [AggregateRoot] *-- [Entity] : contém
```

## Interfaces com Outros Contextos

### Recebe de
- `[Interface]` implementada por [BC]

### Envia para
- `[Command/Event]` consumido por [BC]

## Cenários do Apêndice A cobertosy
- Cenário N: [caminho no modelo]
```

### 2.3 Template ADR

```markdown
# ADR-NNN: [Título]

**Data:** YYYY-MM-DD
**Status:** Proposto / Aceito / Obsoleto / Substituído por ADR-NNN

## Contexto

[Por que esta decisão precisou ser tomada. Forças em tensão.]

## Decisão

[A decisão em si. Ativa, presente.]

## Consequências

### Positivas
- [Benefício]

### Negativas / Trade-offs
- [Custo ou risco aceito]

## Alternativas Consideradas

| Alternativa | Por que não adotada |
|---|---|
| [Opção B] | [Motivo] |
```

---

## Dimension 3: Security / Threat Model

### 3.1 Superfícies de Ataque — Mapeamento para DFD

O sistema possui as seguintes superfícies de ataque que precisam de DFD:

**Nível 0 — Sistema completo:**
- Usuário → Interface Desktop (Avalonia UI)
- Interface → Banco de Dados (PostgreSQL, porta 15432, localhost only)
- Interface → Secure Storage (DPAPI/libsecret)
- Interface → Rede (HTTP/HTTPS para fontes externas)
- Interface → Sistema de Arquivos (fontes locais, exportação/importação)

**Nível 1 — Detalhamento por subsistema:**

| Subsistema | Entradas | Saídas | Trust Boundary |
|---|---|---|---|
| Autenticação | Credenciais do usuário | Token de sessão | Processo → Banco |
| Banco de dados | EF Core queries parametrizadas | Dados do usuário | Aplicação → PostgreSQL |
| Adaptadores de rede | URL de fonte externa | `ItemFeedDto` | Processo → Internet |
| Reprodutor externo | Caminho/URL de conteúdo | Process.Start | Processo → OS |
| Importação | Arquivo do usuário | Registros no banco | FS → Aplicação |
| Secure Storage | Credenciais do banco | Credenciais em memória | OS → Processo |

### 3.2 STRIDE Expandido — Além do Apêndice C existente

O Apêndice C dos Padrões Técnicos v4 já tem uma tabela STRIDE de alto nível. A Phase 1 precisa
expandir com DFD e mitigações rastreáveis por nível de arquitetura:

**Ameaças identificadas que precisam de DFD detalhado:**

| Categoria STRIDE | Ameaça | DFD Relevante | Mitigação Específica |
|---|---|---|---|
| **S** — Spoofing | Replay de sessão entre reinicializações | Autenticação | Token com timestamp + rotação |
| **S** — Spoofing | Bypass de autenticação via manipulação de memória | Autenticação | Verificação na camada de serviço, não apenas UI |
| **T** — Tampering | Injeção SQL via campo de busca | Banco | EF Core parametrizado + BannedSymbols.txt |
| **T** — Tampering | Arquivo de importação malicioso (JSON bomb, zip bomb) | Importação | Validação de tamanho + profundidade antes de parse |
| **T** — Tampering | XML externo com XXE | Adaptadores RSS | `DtdProcessing.Prohibit` |
| **T** — Tampering | SSRF via URL de fonte | Adaptadores HTTP | Validação pós-resolução DNS + whitelist de protocolos |
| **R** — Repudiation | Negação de ação do usuário | Banco | `HistoricoAcao` imutável + logging auditável |
| **I** — Info Disclosure | Cross-user data leak | Banco | `usuario_id` obrigatório em toda query |
| **I** — Info Disclosure | Credenciais do banco em plaintext | Secure Storage | DPAPI/libsecret — nunca em config files |
| **I** — Info Disclosure | Dados sensíveis em logs | Todos | Regras de logging — sem senhas/tokens/PII |
| **I** — Info Disclosure | Admin area discovery | UI + Service | Área inexistente para não-admins em TODAS as camadas |
| **D** — DoS | Payload RSS gigante | Adaptadores | 5MB limit + 10s timeout + circuit breaker |
| **D** — DoS | Consulta sem paginação retorna milhares de itens | Banco | `PaginatedList<T>` obrigatório — testes falham sem paginação |
| **E** — Elevation | Consumidor acessa funções admin | Service layer | RBAC na camada de serviço (não apenas UI) |
| **E** — Elevation | Process.Start com protocolo arbitrário | Reprodutor | Whitelist: `https`, `http`, `file` apenas |

### 3.3 Considerações Específicas do Desktop App

Diferente de web apps, o vetor de ataque principal é **local** — arquivo de banco, config files,
credenciais em memória:

- **PostgreSQL na porta 15432 (localhost):** Outras aplicações no mesmo sistema podem tentar
  conectar. Mitigação: credenciais no Secure Storage do OS, não em config files. Senha forte
  gerada na instalação.
- **Memória:** `CryptographicOperations.ZeroMemory()` após uso de dados sensíveis (senhas, chaves).
  Senhas em `byte[]`, nunca `string` (strings são imutáveis e podem ficar no GC).
- **Binários:** Code signing (Velopack) + verificação SHA-256 antes de instalar atualizações.
- **Arquivos de exportação:** Validar integridade (checksum) antes de importar. Rejeitar arquivos
  acima de limite configurável.

---

## Dimension 4: Testing Strategy

### 4.1 O que Testar na Phase 1

A Phase 1 é pura documentação — não há código para testar diretamente. O que é verificável:

| O que verificar | Como verificar | Quando |
|---|---|---|
| Todos os cenários do Apêndice A percorridos no modelo | Revisão manual documentada em cada doc de BC | Durante criação dos docs |
| Fronteiras de agregado sem ambiguidade | Análise crítica: cada entidade pertence a exatamente um agregado | Review do modelo tático |
| Contratos de interface implementáveis independentemente | Verificar que interfaces em Core não criam dependência circular | Análise de dependências |
| Threat model cobre todas as superfícies identificadas | Checklist: cada superfície de ataque tem ≥1 ameaça documentada | Review do threat model |

### 4.2 Validation Architecture (Nyquist)

Esta fase não tem testes automatizados próprios — mas os artefatos produzidos devem incluir
um **mapeamento explícito de invariantes → testes futuros** para que a Phase 2 tenha uma
lista de testes a escrever desde o início:

| Invariante | Teste previsto em Phase 2 |
|---|---|
| `Titulo` nunca vazio | `ConteudoTests.CriarComTituloVazio_Falha()` |
| `TipoColetanea` obrigatório para Coletanea | `ConteudoTests.CriarColetaneaSemTipo_Falha()` |
| `Nota` no intervalo [0,10] | `ConteudoTests.CriarComNotaInvalida_Falha()` |
| Ciclo em coletâneas rejeitado | `ColetaneaTests.AdicionarColetaneaCriaCiclo_Falha()` |
| Deduplicação por URL idêntica | `DeduplicacaoTests.UrlIdentica_RetornaExistente()` |
| Persistência seletiva: ItemFeed não persiste sem interação | `AgregacaoTests.ItemFeedSemInteracao_NaoPersiste()` |
| Área admin invisível para não-admins | `IdentidadeTests.ConsumidorAcessaAdmin_Generico()` |
| Paginação obrigatória em listas | `AcervoTests.ListarSemPaginacao_Falha()` |

---

## Dimension 5: Implementation Path

### 5.1 Ordem recomendada de criação dos artefatos

```
Dia 1: Setup estrutural
  └── Criar estrutura docs/ + docs/adr/ + docs/threat-model/
  └── ADR-001 até ADR-005 (decisões já tomadas, formalizar)

Dia 2-3: Modelo tático — BC Acervo (foco principal)
  └── docs/domain/acervo.md — agregados completos com Mermaid
  └── Percorrer cenários 1-5 do Apêndice A no modelo

Dia 4: Modelo tático — BC Agregação
  └── docs/domain/agregacao.md — visões efêmeras + contratos
  └── Percorrer cenários 6-7 do Apêndice A no modelo

Dia 5: Esboço dos BCs de suporte
  └── 6 documentos de esboço (Reprodução, IntegracaoExterna, Busca, Portabilidade, Identidade, Preferências)

Dia 6: Interfaces entre contextos
  └── Listar todas as interfaces em Core + comandos + eventos de domínio

Dia 7: Threat model
  └── docs/threat-model/overview.md (DFD nível 0)
  └── docs/threat-model/dfd-nivel-1.md (DFDs por subsistema)
  └── docs/threat-model/stride-table.md (tabela expandida com rastreabilidade)
```

### 5.2 Riscos e Mitigações

| Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|
| Fronteira de agregado errada descoberta na Phase 2 | Média | Alto | Percorrer todos os cenários do Apêndice A no modelo antes de concluir |
| Contrato Acervo↔Agregação incompleto | Baixa | Crítico | Walking skeleton (Phase 2) vai exercitar todos os contratos imediatamente |
| Threat model superficial demais para o pentest | Média | Médio | Usar tabela STRIDE do Apêndice C como baseline; expandir DFD nivel-1 |
| Esboço dos BCs de suporte impede Phase 5+ | Baixa | Médio | Esboços são intencionalmente incompletos — serão completados nas fases respectivas |

---

## Dimension 6: External Dependencies

Nenhuma dependência externa de código nesta fase — é pura documentação.

Ferramentas necessárias:
- Qualquer editor com preview de Mermaid (VS Code + extensão Mermaid Preview)
- Git para versionar os artefatos

---

## Validation Architecture

**Critérios de saída da Phase 1** (mapeados dos Success Criteria do ROADMAP.md):

| Critério | Artefato que prova | Verificável como |
|---|---|---|
| BCs core com artefatos táticos completos | `docs/domain/acervo.md` + `docs/domain/agregacao.md` | Presença de: agregados, VOs, eventos, repositórios, invariantes |
| Cenários do Apêndice A percorridos | Seção "Cenários cobertos" em cada doc de BC | Todos os 7 cenários mapeados explicitamente |
| Interfaces entre contextos definidas | Seção "Interfaces" em cada doc de BC + lista em `acervo.md` | Cada interface tem: nome, contrato, quem implementa, quem consome |
| Threat model antes de rede/persistência | `docs/threat-model/stride-table.md` | DFD nível 0+1 presente; todas superfícies de ataque têm ≥1 ameaça |
| ADRs das decisões principais | 5 ADRs em `docs/adr/` | ADR-001 a ADR-005 existem com seções: contexto, decisão, consequências |

## RESEARCH COMPLETE
