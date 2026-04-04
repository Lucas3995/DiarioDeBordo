using System.Diagnostics.CodeAnalysis;
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

    public CriarConteudoHandler(IConteudoRepository repo, IPublisher mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<Resultado<Guid>> Handle(CriarConteudoCommand cmd, CancellationToken ct)
    {
        // Fast-fail validation before hitting the domain
        if (string.IsNullOrWhiteSpace(cmd.Titulo))
            return Resultado<Guid>.Failure(Erros.TituloObrigatorio);

        Conteudo conteudo;
        try
        {
            // Domain factory enforces invariants (I-01, I-02) via DomainException
            conteudo = Conteudo.Criar(
                usuarioId: cmd.UsuarioId,
                titulo: cmd.Titulo);

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
