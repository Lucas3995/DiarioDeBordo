using MediatR;

namespace DiarioDeBordo.Application.Echo;

/// <summary>
/// Handler do EchoCommand. Valida o fluxo completo do pipeline CQRS:
/// Api → Application (handler + validação) → resposta.
/// </summary>
public sealed class EchoCommandHandler : IRequestHandler<EchoCommand, EchoResponse>
{
    public Task<EchoResponse> Handle(EchoCommand request, CancellationToken cancellationToken)
    {
        var response = new EchoResponse(
            MensagemEcoada: request.Mensagem,
            ProcessadoEm: DateTime.UtcNow);

        return Task.FromResult(response);
    }
}
