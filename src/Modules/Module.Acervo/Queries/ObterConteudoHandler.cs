using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.DTOs;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Queries;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class ObterConteudoHandler
    : IRequestHandler<ObterConteudoQuery, Resultado<ConteudoDetalheDto>>
{
    private readonly IConteudoQueryService _queryService;

    public ObterConteudoHandler(IConteudoQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Resultado<ConteudoDetalheDto>> Handle(
        ObterConteudoQuery query, CancellationToken ct)
    {
        var data = await _queryService
            .ObterAsync(query.Id, query.UsuarioId, ct)
            .ConfigureAwait(false);

        if (data is null)
            return Resultado<ConteudoDetalheDto>.Failure(Erros.NaoEncontrado);

        var dto = new ConteudoDetalheDto(
            data.Id,
            data.Titulo,
            data.Descricao,
            data.Anotacoes,
            data.Nota,
            data.Formato.ToString(),
            data.Papel.ToString(),
            data.CriadoEm,
            data.Classificacao,
            data.IsFilho,
            data.TotalEsperadoSessoes,
            data.Subtipo,
            data.EstadoProgresso.ToString(),
            data.PosicaoAtual,
            data.TipoColetanea?.ToString(),
            data.Categorias.Select(c => new CategoriaDto(c.Id, c.Nome, c.IsAutomatica)).ToList().AsReadOnly(),
            data.Relacoes.Select(r => new RelacaoDto(r.Id, r.ConteudoDestinoId, r.TituloDestino, r.NomeTipo, r.IsInversa)).ToList().AsReadOnly(),
            data.Sessoes.Select(s => new SessaoDto(s.Id, s.Titulo, s.CriadoEm, s.Classificacao, s.Nota, s.Anotacoes)).ToList().AsReadOnly(),
            data.SessoesContagem,
            data.Fontes.Select(f => new FonteDto(f.Id, f.Tipo.ToString(), f.Valor, f.Plataforma, f.Prioridade)).ToList().AsReadOnly(),
            data.Imagens.Select(i => new ImagemDto(i.Id, i.Caminho, i.Origem.ToString(), i.Principal)).ToList().AsReadOnly());

        return Resultado<ConteudoDetalheDto>.Success(dto);
    }
}
