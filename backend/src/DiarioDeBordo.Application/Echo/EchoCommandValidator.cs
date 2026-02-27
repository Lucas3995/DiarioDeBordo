using FluentValidation;

namespace DiarioDeBordo.Application.Echo;

/// <summary>
/// Validação de EchoCommand via FluentValidation.
/// Executada automaticamente pelo ValidationBehavior antes do handler.
/// </summary>
public sealed class EchoCommandValidator : AbstractValidator<EchoCommand>
{
    public EchoCommandValidator()
    {
        RuleFor(x => x.Mensagem)
            .NotEmpty().WithMessage("A mensagem não pode ser vazia.")
            .MaximumLength(256).WithMessage("A mensagem deve ter no máximo 256 caracteres.");
    }
}
