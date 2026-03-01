using FluentValidation;

namespace DiarioDeBordo.Application.Obras.ObterPorIdOuNome;

/// <summary>
/// Validação da GetObraPorIdOuNomeQuery: pelo menos um de Id ou Nome obrigatório.
/// </summary>
public sealed class GetObraPorIdOuNomeQueryValidator : AbstractValidator<GetObraPorIdOuNomeQuery>
{
    public GetObraPorIdOuNomeQueryValidator()
    {
        RuleFor(x => x)
            .Must(q => q.Id.HasValue || !string.IsNullOrWhiteSpace(q.Nome))
            .WithMessage("Informe o Id ou o Nome da obra.");
    }
}
