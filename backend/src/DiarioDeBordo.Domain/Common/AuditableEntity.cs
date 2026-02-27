namespace DiarioDeBordo.Domain.Common;

/// <summary>
/// Entidade auditável: registra criação e última atualização.
/// Apoia o registro de operações de tratamento (LGPD Art. 37).
/// </summary>
public abstract class AuditableEntity : Entity
{
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

    protected void MarcarAtualizado()
    {
        AtualizadoEm = DateTime.UtcNow;
    }
}
