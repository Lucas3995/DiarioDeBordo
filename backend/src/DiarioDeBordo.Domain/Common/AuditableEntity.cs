namespace DiarioDeBordo.Domain.Common;

/// <summary>
/// Entidade auditável: registra criação e última atualização.
/// Apoia o registro de operações de tratamento (LGPD Art. 37).
/// </summary>
public abstract class AuditableEntity : Entity
{
    private readonly IClock? _clock;

    public DateTime CriadoEm { get; protected set; }
    public DateTime AtualizadoEm { get; protected set; }

    protected AuditableEntity()
    {
        CriadoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }

    protected AuditableEntity(Guid id) : base(id)
    {
        CriadoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }

    protected AuditableEntity(IClock clock)
    {
        _clock = clock;
        var now = clock.UtcNow;
        CriadoEm = now;
        AtualizadoEm = now;
    }

    protected AuditableEntity(Guid id, IClock clock) : base(id)
    {
        _clock = clock;
        var now = clock.UtcNow;
        CriadoEm = now;
        AtualizadoEm = now;
    }

    protected void MarcarAtualizado()
    {
        AtualizadoEm = _clock?.UtcNow ?? DateTime.UtcNow;
    }
}
