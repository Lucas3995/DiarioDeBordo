using MediatR;
using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.Core.Eventos;

public sealed record ConteudoCriadoNotification(
    Guid ConteudoId, string Titulo, Guid UsuarioId) : INotification;

public sealed record ProgressoAlteradoNotification(
    Guid ConteudoId, EstadoProgresso Estado, string? PosicaoAtual) : INotification;

public sealed record ItemFeedPersistidoNotification(
    Guid ConteudoId, string Titulo) : INotification;
