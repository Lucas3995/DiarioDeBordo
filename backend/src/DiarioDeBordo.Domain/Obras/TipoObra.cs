namespace DiarioDeBordo.Domain.Obras;

/// <summary>
/// Tipos de obra suportados pelo sistema.
/// Novos tipos são adicionados pela equipe de desenvolvimento (não pelo usuário),
/// pois cada tipo pode ter campos e funcionalidades diferentes no futuro.
/// </summary>
public enum TipoObra
{
    Manga,
    Manhwa,
    Manhua,
    Anime,
    Livro,
    Filme,
    Serie,
    Webnovel,
}
