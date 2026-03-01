namespace DiarioDeBordo.Application.Common;

/// <summary>
/// Exceção de aplicação lançada quando a validação de um request falha.
/// Permite que a camada Application não dependa de tipos do FluentValidation.
/// </summary>
public sealed class RequestValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public RequestValidationException(IReadOnlyList<ValidationError> errors)
        : base("Um ou mais erros de validação ocorreram.")
    {
        Errors = errors;
    }
}

/// <summary>Representa um erro de validação em um campo.</summary>
public sealed record ValidationError(string PropertyName, string ErrorMessage);
