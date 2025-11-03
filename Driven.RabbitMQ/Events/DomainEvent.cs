namespace Driven.RabbitMQ.Events;

/// <summary>
/// Classe base para eventos de domínio
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// ID único do evento
    /// </summary>
    public Guid EventId { get; protected set; }

    /// <summary>
    /// Data e hora de criação do evento
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Versão do evento
    /// </summary>
    public int Version { get; protected set; }

    /// <summary>
    /// Usuário que disparou o evento
    /// </summary>
    public string? CreatedBy { get; protected set; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Version = 1;
    }
}
