using FluentValidation;

namespace DiarioDeBordo.Application.Common;

/// <summary>
/// Adaptador que implementa IRequestValidator delegando ao FluentValidation IValidator.
/// Mantém a validação FluentValidation na borda; o pipeline usa apenas a abstração.
/// </summary>
public sealed class FluentValidationRequestValidatorAdapter<TRequest>(IValidator<TRequest> fluentValidator)
    : IRequestValidator<TRequest>
    where TRequest : notnull
{
    public ValidationResult Validate(TRequest request)
    {
        var result = fluentValidator.Validate(request);
        if (result.IsValid)
            return ValidationResult.Success();

        var errors = result.Errors
            .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
            .ToList();
        return ValidationResult.Failure(errors);
    }
}
