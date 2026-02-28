using DiarioDeBordo.Domain.Obras;
using Microsoft.EntityFrameworkCore;

namespace DiarioDeBordo.Persistence.Seed;

/// <summary>
/// Seed de dados para ambiente Development.
/// Popula a tabela Obras com os mesmos exemplos usados no mock do frontend,
/// permitindo validação visual da tela /obras sem inserção manual de dados.
///
/// Não é executado em Production.
/// </summary>
public static class ObrasSeed
{
    public static async Task AplicarAsync(DiarioDeBordoDbContext context)
    {
        if (await context.Obras.AnyAsync())
            return;

        var obras = CriarObras();
        context.Obras.AddRange(obras);
        await context.SaveChangesAsync();
    }

    private static IEnumerable<Obra> CriarObras()
    {
        var obra1 = new Obra("One Piece", TipoObra.Manga, 1110,
            new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc), 1);
        obra1.DefinirDiasAteProxima(5);

        var obra2 = new Obra("Solo Leveling", TipoObra.Manhwa, 179,
            new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc), 2);
        obra2.DefinirDiasAteProxima(7);

        var obra3 = new Obra("Fullmetal Alchemist", TipoObra.Anime, 64,
            new DateTime(2026, 1, 30, 0, 0, 0, DateTimeKind.Utc), 3);

        var obra4 = new Obra("The Beginning After the End", TipoObra.Webnovel, 320,
            new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc), 4);
        obra4.DefinirPartesJaPublicadas(3);

        var obra5 = new Obra("Harry Potter e o Cálice de Fogo", TipoObra.Livro, 37,
            new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc), 5);

        var obra6 = new Obra("Breaking Bad", TipoObra.Serie, 47,
            new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc), 6);
        obra6.DefinirPartesJaPublicadas(1);

        var obra7 = new Obra("Interstellar", TipoObra.Filme, 95,
            new DateTime(2025, 12, 20, 0, 0, 0, DateTimeKind.Utc), 7);

        var obra8 = new Obra("Demon Slayer", TipoObra.Anime, 44,
            new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc), 8);
        obra8.DefinirDiasAteProxima(14);

        var obra9 = new Obra("Tower of God", TipoObra.Manhwa, 605,
            new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc), 9);
        obra9.DefinirDiasAteProxima(7);

        var obra10 = new Obra("O Nome do Vento", TipoObra.Livro, 22,
            new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc), 10);

        var obra11 = new Obra("Attack on Titan", TipoObra.Manga, 139,
            new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc), 11);

        var obra12 = new Obra("Vinland Saga", TipoObra.Manga, 198,
            new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc), 12);
        obra12.DefinirPartesJaPublicadas(2);

        return [obra1, obra2, obra3, obra4, obra5, obra6, obra7, obra8, obra9, obra10, obra11, obra12];
    }
}
