using FluentValidation;

namespace DiarioDeBordo.Application.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login não pode ser vazio.")
            .MaximumLength(100).WithMessage("Login deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha não pode ser vazia.")
            .MaximumLength(200).WithMessage("Senha deve ter no máximo 200 caracteres.");
    }
}
