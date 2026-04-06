namespace DiarioDeBordo.Core.Primitivos;

/// <summary>
/// Representa o resultado de uma operação de serviço — sucesso com valor ou falha com erro.
/// Baseado em Railway Oriented Programming (Wlaschin, 2014).
/// Nunca lança exceção para fluxos de negócio esperados — use DomainException para invariantes.
/// </summary>
public sealed record Resultado<T>
{
    public T? Value { get; }
    public Erro? Error { get; }
    public AlertaUsoSaudavel? Alerta { get; init; }
    public bool IsSuccess => Error is null;
    public bool IsFailure => Error is not null;

    private Resultado(T value) => Value = value;
    private Resultado(Erro error) => Error = error;

#pragma warning disable CA1000 // Factory pattern on generic type is intentional (Railway Oriented Programming)
    public static Resultado<T> Success(T value) => new(value);
    public static Resultado<T> Failure(Erro error) => new(error);

    /// <summary>Propaga falha de um Resultado de outro tipo — útil para converter erros.</summary>
    public static Resultado<T> Failure<TOther>(Resultado<TOther> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other.IsSuccess) throw new InvalidOperationException("Cannot convert a successful result to failure.");
        return Failure(other.Error!);
    }
#pragma warning restore CA1000
}

public sealed record Erro(string Codigo, string Mensagem);

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Phase 10: uso saudável — alertas de consumo não implementados até a fase final.")]
public sealed record AlertaUsoSaudavel(string Mensagem);
