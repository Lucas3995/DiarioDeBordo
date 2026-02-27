using FluentValidation;

namespace DiarioDeBordo.Application.Obras.Listar;

/// <summary>
/// Validação de GetObrasAcompanhamentoQuery via FluentValidation.
/// Executada automaticamente pelo ValidationBehavior antes do handler.
/// </summary>
public sealed class GetObrasAcompanhamentoQueryValidator : AbstractValidator<GetObrasAcompanhamentoQuery>
{
    private static readonly int[] PageSizesPermitidos = [10, 25, 50, 100];

    public GetObrasAcompanhamentoQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O índice de página não pode ser negativo.");

        RuleFor(x => x.PageSize)
            .Must(ps => Array.IndexOf(PageSizesPermitidos, ps) >= 0)
            .WithMessage("O tamanho de página deve ser 10, 25, 50 ou 100.");
    }
}
