using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;
using MediatR;

namespace DiarioDeBordo.Module.Acervo.Commands;

/// <summary>
/// Registra uma sessão de consumo como Conteudo filho (D-17/D-18).
/// Cria o filho com IsFilho=true e vincula ao pai via "Contém"/"Parte de".
/// </summary>
public sealed record RegistrarSessaoCommand(
    Guid UsuarioId,
    Guid ConteudoPaiId,
    string Titulo,
    string? Anotacoes,
    decimal? Nota,
    Classificacao? Classificacao,
    FormatoMidia Formato,
    DateTimeOffset? DataConsumo) : IRequest<Resultado<Guid>>;
