# BC Busca — Esboço

**Classificação:** Suporte
**Projeto .NET:** `DiarioDeBordo.Module.Busca`
**Status de modelagem:** Esboço — modelagem tática completa será feita na Phase de implementação do BC Busca

## Responsabilidade

Fornecer busca textual e filtros combinados sobre os conteúdos do usuário (armazenados no BC Acervo), incluindo suporte a operações em lote sobre conjuntos de resultados. A busca é sempre escoped ao usuário autenticado — sem acesso a dados de outros usuários.

## O que este BC NÃO faz

- Não indexa conteúdo externo (feeds, plataformas)
- Não recomenda conteúdo (sem algoritmo de sugestão)
- Não faz busca semântica ou por similaridade (sem embeddings, sem IA)
- Não persiste dados próprios — lê do BC Acervo
- Não expõe resultados de outros usuários

## Interfaces publicadas (definidas em Core, implementadas aqui)

```csharp
// Consumida por ViewModels e commands que precisam de busca/lote:
public interface IBuscaConteudoService
{
    Task<Result<PaginatedList<ConteudoResumoDto>>> BuscarAsync(
        Guid usuarioId,
        FiltroBusca filtro,
        PaginacaoParams paginacao,
        CancellationToken ct);

    Task<Result<int>> ExecutarOperacaoEmLoteAsync(
        Guid usuarioId,
        IReadOnlyList<Guid> ids,
        OperacaoLote operacao,
        CancellationToken ct);
}

public sealed record FiltroBusca(
    string? TextoLivre,
    FormatoMidia? Formato,
    PapelConteudo? Papel,
    TipoColetanea? TipoColetanea,
    Guid? CategoriaId,
    decimal? NotaMinima,
    EstadoProgresso? Progresso,
    DateOnly? DataCriacaoApos,
    Guid? FonteId
);

public enum OperacaoLote
{
    MarcarComoLido,
    MarcarComoNaoLido,
    AdicionarCategoria,
    RemoverCategoria,
    ExcluirConteudos
}
```

## Interfaces consumidas (definidas em Core, implementadas em outro BC)

```csharp
// Implementada em Module.Acervo — Busca acessa dados de conteúdo via repositório somente leitura:
public interface IConteudoQueryRepository
{
    Task<IReadOnlyList<ConteudoResumoDto>> BuscarAsync(
        Guid usuarioId,
        FiltroBusca filtro,
        PaginacaoParams paginacao,
        CancellationToken ct);

    Task<int> ContarAsync(Guid usuarioId, FiltroBusca filtro, CancellationToken ct);
}
```

## Eventos de domínio relevantes

| Evento | Publicado por | Consumido por | Quando |
|---|---|---|---|
| Nenhum evento de domínio | — | — | Busca é uma operação de leitura; operações em lote delegam via MediatR ao BC Acervo |

## Contratos Transversais — DiarioDeBordo.Core

Interfaces e tipos definidos em `DiarioDeBordo.Core` que não pertencem a nenhum BC específico mas são consumidos por múltiplos módulos:

### PaginacaoParams (Value Object compartilhado)

```csharp
public sealed record PaginacaoParams(int Pagina, int ItensPorPagina)
{
    public static readonly PaginacaoParams Padrao = new(1, 20);
    // Invariante: ItensPorPagina máximo 100 — sem listagens ilimitadas
}

public sealed record PaginatedList<T>(
    IReadOnlyList<T> Itens,
    int TotalItens,
    int Pagina,
    int ItensPorPagina);
```

### Result\<T\> (tipo de retorno padronizado)

```csharp
// Usado em todos os commands e queries — sem lançar exceções para fluxos esperados:
public sealed record Result<T>(bool Sucesso, T? Valor, string? Erro)
{
    public static Result<T> Ok(T valor) => new(true, valor, null);
    public static Result<T> Failure(string erro) => new(false, default, erro);
}
```

> **Nota:** `PaginacaoParams` e `Result<T>` são os dois contratos transversais que aparecem em praticamente todos os módulos. Toda query paginada retorna `Result<PaginatedList<T>>`. Toda operação de escrita retorna `Result<T>` — nunca lança exceções para falhas esperadas de negócio.

## O que é adiado para a fase de implementação

- Implementação de full-text search: PostgreSQL `tsvector`/`tsquery` vs. extensão dedicada (ex: Meilisearch)
- Criação e manutenção de índices de busca
- Implementação concreta das operações em lote (validação, transações, limite de lote)
- Busca por relevância vs. busca exata
- Autocomplete e sugestões de busca
- Busca por Markdown/HTML dentro do corpo do conteúdo
