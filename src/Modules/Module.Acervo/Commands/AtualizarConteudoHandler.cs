using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class AtualizarConteudoHandler : IRequestHandler<AtualizarConteudoCommand, Resultado<Unit>>
{
    private readonly IConteudoRepository _repo;

    public AtualizarConteudoHandler(IConteudoRepository repo)
    {
        _repo = repo;
    }

    public async Task<Resultado<Unit>> Handle(AtualizarConteudoCommand cmd, CancellationToken ct)
    {
        // Fast-fail validation
        if (string.IsNullOrWhiteSpace(cmd.Titulo))
            return Resultado<Unit>.Failure(Erros.TituloObrigatorio);

        var conteudo = await _repo.ObterPorIdAsync(cmd.Id, cmd.UsuarioId, ct).ConfigureAwait(false);
        if (conteudo is null)
            return Resultado<Unit>.Failure(Erros.NaoEncontrado);

        // Update simple fields
        conteudo.Titulo = cmd.Titulo.Trim();
        conteudo.Descricao = cmd.Descricao;
        conteudo.Anotacoes = cmd.Anotacoes;
        conteudo.Formato = cmd.Formato;
        conteudo.Subtipo = cmd.Subtipo;

        // Nota — enforce I-03 via domain method
        try
        {
            if (cmd.Nota.HasValue)
                conteudo.DefinirNota(cmd.Nota.Value);
            else
                conteudo.LimparNota();
        }
        catch (DomainException ex)
        {
            return Resultado<Unit>.Failure(ex.ToErro());
        }

        // Classificacao — independent from Nota (D-06/D-07)
        conteudo.DefinirClassificacao(cmd.Classificacao);

        // Progresso
        conteudo.AlterarProgresso(cmd.EstadoProgresso, cmd.PosicaoAtual);

        // TotalEsperadoSessoes
        try
        {
            conteudo.DefinirTotalEsperadoSessoes(cmd.TotalEsperadoSessoes);
        }
        catch (DomainException ex)
        {
            return Resultado<Unit>.Failure(ex.ToErro());
        }

        await _repo.AtualizarAsync(conteudo, ct).ConfigureAwait(false);

        // Manage category associations (diff-based)
        await _repo.AtualizarCategoriasAsync(cmd.Id, cmd.UsuarioId, cmd.CategoriaIds, ct).ConfigureAwait(false);

        return Resultado<Unit>.Success(Unit.Value);
    }
}
