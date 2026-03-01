using MediatR;

namespace DiarioDeBordo.Application.Common;

/// <summary>
/// Pipeline behavior do MediatR que executa todas as validações antes de despachar o handler.
/// Garante que comandos inválidos sejam rejeitados antes de qualquer lógica de negócio ou persistência.
/// Não depende de FluentValidation; usa a abstração IRequestValidator.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IRequestValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validatorsList = validators.ToList();
        if (validatorsList.Count == 0)
            return await next(cancellationToken);

        var failures = new List<ValidationError>();
        foreach (var validator in validatorsList)
        {
            var result = validator.Validate(request);
            if (!result.IsValid)
                failures.AddRange(result.Errors);
        }

        if (failures.Count != 0)
            throw new RequestValidationException(failures);

        return await next(cancellationToken);
    }
}
