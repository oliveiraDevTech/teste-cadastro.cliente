namespace Core.Domain.Common;

/// <summary>
/// Classe base para todas as entidades do domínio
/// Fornece propriedades comuns como ID, datas de criação e auditoria
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Data e hora de criação da entidade
    /// </summary>
    public DateTime DataCriacao { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora da última atualização da entidade
    /// </summary>
    public DateTime? DataAtualizacao { get; protected set; }

    /// <summary>
    /// Usuário que criou a entidade
    /// </summary>
    public string? CriadoPor { get; protected set; }

    /// <summary>
    /// Usuário que atualizou a entidade
    /// </summary>
    public string? AtualizadoPor { get; protected set; }

    /// <summary>
    /// Indica se a entidade está ativa
    /// </summary>
    public bool Ativo { get; protected set; } = true;

    /// <summary>
    /// Construtor padrão
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
        Ativo = true;
    }

    /// <summary>
    /// Construtor com ID pré-definido
    /// </summary>
    /// <param name="id">ID da entidade</param>
    protected BaseEntity(Guid id) : this()
    {
        Id = id;
    }

    /// <summary>
    /// Marca a entidade para ser atualizada
    /// </summary>
    /// <param name="atualizadoPor">Usuário que está atualizando</param>
    public virtual void MarcarComoAtualizada(string? atualizadoPor = null)
    {
        DataAtualizacao = DateTime.UtcNow;
        AtualizadoPor = atualizadoPor;
    }

    /// <summary>
    /// Desativa a entidade
    /// </summary>
    /// <param name="atualizadoPor">Usuário que está desativando</param>
    public virtual void Desativar(string? atualizadoPor = null)
    {
        Ativo = false;
        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Ativa a entidade
    /// </summary>
    /// <param name="atualizadoPor">Usuário que está ativando</param>
    public virtual void Ativar(string? atualizadoPor = null)
    {
        Ativo = true;
        MarcarComoAtualizada(atualizadoPor);
    }
}
