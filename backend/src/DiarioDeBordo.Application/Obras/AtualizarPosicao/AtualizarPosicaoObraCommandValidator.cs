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
            .WithMessage(ObraValidationMessages.IdOuNomeObrigatorio);

        RuleFor(x => x.NovaPosicao)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ObraValidationMessages.PosicaoNaoPodeSerNegativa);

        When(x => x.CriarSeNaoExistir, () =>
        {
            RuleFor(x => x.NomeParaCriar)
                .NotEmpty()
                .WithMessage(ObraValidationMessages.NomeObrigatorioParaCriar);
            RuleFor(x => x.TipoParaCriar)
                .NotNull()
                .WithMessage(ObraValidationMessages.TipoObrigatorioParaCriar);
            RuleFor(x => x.OrdemPreferenciaParaCriar)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .WithMessage(ObraValidationMessages.OrdemObrigatoriaParaCriar);
        });
    }
}
