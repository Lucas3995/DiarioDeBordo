using MediatR;
using DiarioDeBordo.Core.Enums;
using System.Diagnostics.CodeAnalysis;

namespace DiarioDeBordo.Core.Eventos;

public sealed record ConteudoCriadoNotification(
    Guid ConteudoId, string Titulo, Guid UsuarioId) : INotification;

[ExcludeFromCodeCoverage(Justification = "Phase 5: implementado quando o módulo de progresso for construído.")]
public sealed record ProgressoAlteradoNotification(
    Guid ConteudoId, EstadoProgresso Estado, string? PosicaoAtual) : INotification;

[ExcludeFromCodeCoverage(Justification = "Phase 5: implementado quando o módulo de agregação de feed for construído.")]
public sealed record ItemFeedPersistidoNotification(
    Guid ConteudoId, string Titulo) : INotification;
