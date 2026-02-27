using MediatR;

namespace DiarioDeBordo.Application.Echo;

/// <summary>
/// Comando de exemplo para validar o fluxo Api → Application → (Domain) → resposta.
/// Também valida o pipeline de FluentValidation no MediatR.
/// </summary>
public sealed record EchoCommand(string Mensagem) : IRequest<EchoResponse>;

public sealed record EchoResponse(string MensagemEcoada, DateTime ProcessadoEm);
