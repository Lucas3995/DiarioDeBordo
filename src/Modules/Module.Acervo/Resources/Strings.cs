using System.Globalization;
using System.Resources;

namespace DiarioDeBordo.Module.Acervo.Resources;

/// <summary>Acesso fortemente tipado às strings do Módulo Acervo (pt-BR).</summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Generated resource accessor — strings validated via UI/E2E tests, not unit coverage.")]
internal static class Strings
{
#pragma warning disable CA1810 // Initialization of _rm is straightforward and not performance-critical here
    private static readonly ResourceManager ResourceManager =
        new(
            "DiarioDeBordo.Module.Acervo.Resources.Strings",
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

    // Modal
    public static string Modal_BotaoSalvar => Get(nameof(Modal_BotaoSalvar));
    public static string Modal_BotaoCancelar => Get(nameof(Modal_BotaoCancelar));
    public static string Modal_BotaoExcluir => Get(nameof(Modal_BotaoExcluir));
    public static string Modal_Titulo_Existente => Get(nameof(Modal_Titulo_Existente));

    // Seções
    public static string Secao_Identificacao => Get(nameof(Secao_Identificacao));
    public static string Secao_Avaliacao => Get(nameof(Secao_Avaliacao));
    public static string Secao_Organizacao => Get(nameof(Secao_Organizacao));
    public static string Secao_Historico => Get(nameof(Secao_Historico));

    // Labels de campos
    public static string Label_Descricao => Get(nameof(Label_Descricao));
    public static string Label_Anotacoes => Get(nameof(Label_Anotacoes));
    public static string Label_Formato => Get(nameof(Label_Formato));
    public static string Label_Subtipo => Get(nameof(Label_Subtipo));
    public static string Label_Nota => Get(nameof(Label_Nota));
    public static string Label_Classificacao => Get(nameof(Label_Classificacao));
    public static string Label_Progresso => Get(nameof(Label_Progresso));
    public static string Label_Categorias => Get(nameof(Label_Categorias));
    public static string Label_Relacoes => Get(nameof(Label_Relacoes));
    public static string Label_TotalEsperado => Get(nameof(Label_TotalEsperado));
    public static string Label_Data => Get(nameof(Label_Data));

    // Ações
    public static string Relacao_BotaoAdicionar => Get(nameof(Relacao_BotaoAdicionar));
    public static string Relacao_BotaoVincular => Get(nameof(Relacao_BotaoVincular));
    public static string Sessao_BotaoRegistrar => Get(nameof(Sessao_BotaoRegistrar));
    public static string Formulario_MaisDetalhes => Get(nameof(Formulario_MaisDetalhes));
    public static string Formulario_MenosDetalhes => Get(nameof(Formulario_MenosDetalhes));
    public static string Card_VerDetalhe => Get(nameof(Card_VerDetalhe));

    // Estados vazios
    public static string Categorias_EstadoVazio => Get(nameof(Categorias_EstadoVazio));
    public static string Relacoes_EstadoVazio => Get(nameof(Relacoes_EstadoVazio));
    public static string Sessoes_EstadoVazio => Get(nameof(Sessoes_EstadoVazio));
    public static string Avaliacao_SemAvaliacao => Get(nameof(Avaliacao_SemAvaliacao));

    // Diálogos de confirmação
    public static string Dialog_ExcluirConteudo_Titulo => Get(nameof(Dialog_ExcluirConteudo_Titulo));
    public static string Dialog_ExcluirConteudo_Mensagem => Get(nameof(Dialog_ExcluirConteudo_Mensagem));
    public static string Dialog_DescartarAlteracoes_Titulo => Get(nameof(Dialog_DescartarAlteracoes_Titulo));
    public static string Dialog_DescartarAlteracoes_Mensagem => Get(nameof(Dialog_DescartarAlteracoes_Mensagem));
    public static string Dialog_ExcluirRelacao_Titulo => Get(nameof(Dialog_ExcluirRelacao_Titulo));
    public static string Dialog_ExcluirRelacao_Mensagem => Get(nameof(Dialog_ExcluirRelacao_Mensagem));

    // Watermarks
    public static string Categorias_Watermark => Get(nameof(Categorias_Watermark));
    public static string Relacao_Watermark_Busca => Get(nameof(Relacao_Watermark_Busca));
    public static string Sessao_Watermark_Titulo => Get(nameof(Sessao_Watermark_Titulo));

    // Progresso
    public static string Progresso_ComTotal => Get(nameof(Progresso_ComTotal));
    public static string Progresso_SemTotal => Get(nameof(Progresso_SemTotal));
    public static string Progresso_Vazio => Get(nameof(Progresso_Vazio));

    // Criação inline
    public static string Categoria_CriarInline => Get(nameof(Categoria_CriarInline));
    public static string TipoRelacao_CriarInline => Get(nameof(TipoRelacao_CriarInline));
    public static string TipoRelacao_NomeInverso => Get(nameof(TipoRelacao_NomeInverso));

    // Erros adicionais
    public static string Erro_FalhaAoSalvar => Get(nameof(Erro_FalhaAoSalvar));
    public static string Erro_FalhaAoExcluir => Get(nameof(Erro_FalhaAoExcluir));
    public static string Erro_FalhaAoCarregar => Get(nameof(Erro_FalhaAoCarregar));
    public static string Erro_CategoriaDuplicada => Get(nameof(Erro_CategoriaDuplicada));
    public static string Erro_RelacaoDuplicada => Get(nameof(Erro_RelacaoDuplicada));
    public static string Erro_NotaForaDaFaixa => Get(nameof(Erro_NotaForaDaFaixa));
}
