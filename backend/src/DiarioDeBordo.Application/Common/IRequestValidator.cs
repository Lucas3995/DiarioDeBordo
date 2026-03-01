namespace DiarioDeBordo.Application.Common;

/// <summary>
/// Abstração para validar um request antes do handler.
/// A implementação pode usar FluentValidation na borda; a Application não depende dele.
/// </summary>
public interface IRequestValidator<in TRequest> where TRequest : notnull
{
    ValidationResult Validate(TRequest request);
}

/// <summary>Resultado da validação de um request.</summary>
public sealed class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationError> Errors { get; }

    private ValidationResult(bool isValid, IReadOnlyList<ValidationError> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    public static ValidationResult Success() => new(true, Array.Empty<ValidationError>());

    public static ValidationResult Failure(IReadOnlyList<ValidationError> errors) =>
        new(false, errors);
}
