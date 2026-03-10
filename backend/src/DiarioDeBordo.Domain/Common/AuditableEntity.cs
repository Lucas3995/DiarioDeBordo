namespace DiarioDeBordo.Domain.Common;

/// <summary>
/// Entidade auditável: registra criação e última atualização.
/// Apoia o registro de operações de tratamento (LGPD Art. 37).
/// Construtores de domínio exigem IClock para testabilidade;
/// construtor parameterless reservado para hidratação EF Core.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTime CriadoEm { get; protected set; }
    public DateTime AtualizadoEm { get; protected set; }

    /// <summary>EF Core hydration — datas vêm do banco.</summary>
    protected AuditableEntity() { }

    protected AuditableEntity(IClock clock)
    {
        var now = clock.UtcNow;
        CriadoEm = now;
        AtualizadoEm = now;
    }

    protected AuditableEntity(Guid id, IClock clock) : base(id)
    {
        var now = clock.UtcNow;
        CriadoEm = now;
        AtualizadoEm = now;
    }

    protected void MarcarAtualizado(IClock clock)
    {
        AtualizadoEm = clock.UtcNow;
    }
}
