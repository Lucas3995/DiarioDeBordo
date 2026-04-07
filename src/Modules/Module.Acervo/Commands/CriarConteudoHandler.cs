using System.Diagnostics.CodeAnalysis;
using DiarioDeBordo.Core.Consultas;
using DiarioDeBordo.Core.Entidades;
using DiarioDeBordo.Core.Eventos;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Core.Repositorios;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Instantiated by MediatR via DI container at runtime.")]
internal sealed class CriarConteudoHandler : IRequestHandler<CriarConteudoCommand, Resultado<Guid>>
{
    private readonly IConteudoRepository _repo;
    private readonly IPublisher _mediator;
    private readonly IDeduplicacaoService _deduplicacaoService;

    public CriarConteudoHandler(
        IConteudoRepository repo,
        IPublisher mediator,
        IDeduplicacaoService deduplicacaoService)
    {
        _repo = repo;
        _mediator = mediator;
        _deduplicacaoService = deduplicacaoService;
    }

    public async Task<Resultado<Guid>> Handle(CriarConteudoCommand cmd, CancellationToken ct)
    {
        // Fast-fail validation before hitting the domain
        if (string.IsNullOrWhiteSpace(cmd.Titulo))
            return Resultado<Guid>.Failure(Erros.TituloObrigatorio);

        if (!cmd.IgnorarDuplicata && cmd.Titulo.Trim().Length >= 3)
        {
            var duplicata = await _deduplicacaoService
                .VerificarAsync(cmd.UsuarioId, cmd.Titulo, null, ct)
                .ConfigureAwait(false);
            if (duplicata is not null)
                return Resultado<Guid>.Failure(new Erro(
                    Erros.DuplicataDetectada.Codigo,
                    $"{Erros.DuplicataDetectada.Mensagem} Use VerificarDuplicataQuery para inspecionar o candidato."));
        }

        Conteudo conteudo;
        try
        {
            // Domain factory enforces invariants (I-01, I-02) via DomainException
            conteudo = Conteudo.Criar(
                usuarioId: cmd.UsuarioId,
                titulo: cmd.Titulo,
                papel: cmd.Papel,
                tipoColetanea: cmd.TipoColetanea,
                formato: cmd.Formato);

            // Optional fields filled at creation via progressive disclosure
            if (!string.IsNullOrWhiteSpace(cmd.Descricao))
                conteudo.Descricao = cmd.Descricao.Trim();
            if (!string.IsNullOrWhiteSpace(cmd.Anotacoes))
                conteudo.Anotacoes = cmd.Anotacoes.Trim();
        }
        catch (DomainException ex)
        {
            return Resultado<Guid>.Failure(ex.ToErro());
        }

        await _repo.AdicionarAsync(conteudo, ct).ConfigureAwait(false);

        // Notify listeners (e.g., UI ViewModels) that a new Conteudo was created
        await _mediator.Publish(
            new ConteudoCriadoNotification(conteudo.Id, conteudo.Titulo, conteudo.UsuarioId),
            ct).ConfigureAwait(false);

        return Resultado<Guid>.Success(conteudo.Id);
    }
}
