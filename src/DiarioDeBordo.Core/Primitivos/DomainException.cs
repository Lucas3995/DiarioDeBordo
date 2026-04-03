namespace DiarioDeBordo.Core.Primitivos;

/// <summary>
/// Lançada quando uma invariante de domínio é violada.
/// Capturada pelos handlers de comando e transformada em Resultado.Failure.
/// NÃO use para erros de infraestrutura (IO, rede, banco) — esses propagam como exceções normais.
/// </summary>
public sealed class DomainException : Exception
{
    public string Codigo { get; }

    public DomainException(string codigo, string mensagem)
        : base(mensagem)
    {
        Codigo = codigo;
    }

    // Standard constructors required by CA1032 for serialization compatibility.
    // 'codigo' defaults to empty — only the parameterized constructor is intended for use.
    public DomainException() : this(string.Empty, string.Empty) { }
    public DomainException(string message) : this(string.Empty, message) { }
    public DomainException(string message, Exception innerException)
        : base(message, innerException) { Codigo = string.Empty; }

    public Erro ToErro() => new(Codigo, Message);
}
