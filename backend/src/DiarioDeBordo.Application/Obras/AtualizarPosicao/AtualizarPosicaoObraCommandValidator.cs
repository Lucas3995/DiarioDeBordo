using FluentValidation;

namespace DiarioDeBordo.Application.Obras.AtualizarPosicao;

/// <summary>
/// Validação do AtualizarPosicaoObraCommand: pelo menos id ou nome; nova posição >= 0;
/// quando criar, nome/tipo/ordem obrigatórios.
/// </summary>
public sealed class AtualizarPosicaoObraCommandValidator : AbstractValidator<AtualizarPosicaoObraCommand>
{
    public AtualizarPosicaoObraCommandValidator()
    {
        RuleFor(x => x)
            .Must(c => c.IdObra.HasValue || !string.IsNullOrWhiteSpace(c.NomeObra))
            .WithMessage("Informe o Id ou o Nome da obra.");

        RuleFor(x => x.NovaPosicao)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A posição não pode ser negativa.");

        When(x => x.CriarSeNaoExistir, () =>
        {
            RuleFor(x => x.NomeParaCriar)
                .NotEmpty()
                .WithMessage("Para criar nova obra, o nome é obrigatório.");
            RuleFor(x => x.TipoParaCriar)
                .NotNull()
                .WithMessage("Para criar nova obra, o tipo é obrigatório.");
            RuleFor(x => x.OrdemPreferenciaParaCriar)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .WithMessage("Para criar nova obra, a ordem de preferência é obrigatória e não pode ser negativa.");
        });
    }
}
