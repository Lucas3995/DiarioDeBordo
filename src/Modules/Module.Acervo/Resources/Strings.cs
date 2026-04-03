using System.Globalization;
using System.Resources;

namespace DiarioDeBordo.Module.Acervo.Resources;

/// <summary>Acesso fortemente tipado às strings do Módulo Acervo (pt-BR).</summary>
internal static class Strings
{
#pragma warning disable CA1810 // Initialization of _rm is straightforward and not performance-critical here
    private static readonly ResourceManager ResourceManager =
        new(
            "DiarioDeBordo.Module.Acervo.Resources.Strings.pt-BR",
            typeof(Strings).Assembly);
#pragma warning restore CA1810

    private static string Get(string key) =>
        ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

    // Criar Conteúdo
    public static string CriarConteudo_Titulo => Get(nameof(CriarConteudo_Titulo));
    public static string CriarConteudo_CampoTitulo_Label => Get(nameof(CriarConteudo_CampoTitulo_Label));
    public static string CriarConteudo_CampoTitulo_Placeholder => Get(nameof(CriarConteudo_CampoTitulo_Placeholder));
    public static string CriarConteudo_BotaoCriar => Get(nameof(CriarConteudo_BotaoCriar));
    public static string CriarConteudo_BotaoMaisDetalhes => Get(nameof(CriarConteudo_BotaoMaisDetalhes));
    public static string CriarConteudo_Sucesso => Get(nameof(CriarConteudo_Sucesso));

    // Listagem
    public static string Acervo_Titulo => Get(nameof(Acervo_Titulo));
    public static string Acervo_EstadoVazio_Titulo => Get(nameof(Acervo_EstadoVazio_Titulo));
    public static string Acervo_EstadoVazio_Descricao => Get(nameof(Acervo_EstadoVazio_Descricao));

    // Erros
    public static string Erro_TituloObrigatorio => Get(nameof(Erro_TituloObrigatorio));
    public static string Erro_ConteudoNaoEncontrado => Get(nameof(Erro_ConteudoNaoEncontrado));

    // Labels
    public static string Label_NovoConteudo => Get(nameof(Label_NovoConteudo));
    public static string Label_Titulo => Get(nameof(Label_Titulo));
    public static string Label_Adicionar => Get(nameof(Label_Adicionar));
    public static string Label_Cancelar => Get(nameof(Label_Cancelar));
}
