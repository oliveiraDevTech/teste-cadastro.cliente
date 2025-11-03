using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

/// <summary>
/// Interface para repositório de Usuário
/// Define contrato para operações de persistência de dados de usuário
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Adiciona um novo usuário
    /// </summary>
    Task<Usuario> AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    Task<Usuario> AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um usuário por ID
    /// </summary>
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário por login
    /// </summary>
    Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário por email
    /// </summary>
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário por login ou email (para login)
    /// </summary>
    Task<Usuario?> ObterPorLoginOuEmailAsync(string loginOuEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todos os usuários
    /// </summary>
    Task<List<Usuario>> ListarTodosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista usuários ativos
    /// </summary>
    Task<List<Usuario>> ListarAtivosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista usuários por tipo
    /// </summary>
    Task<List<Usuario>> ListarPorTipoAsync(string tipoUsuario, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista usuários com paginação
    /// </summary>
    Task<(List<Usuario> usuarios, int total)> ListarComPaginacaoAsync(int pagina, int itensPorPagina, string? filtro = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista administradores
    /// </summary>
    Task<List<Usuario>> ListarAdministradoresAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários por cliente ID
    /// </summary>
    Task<List<Usuario>> ListarPorClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um login já existe
    /// </summary>
    Task<bool> ExisteLoginAsync(string login, Guid? idExcluir = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um email já existe
    /// </summary>
    Task<bool> ExisteEmailAsync(string email, Guid? idExcluir = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários com email não confirmado
    /// </summary>
    Task<List<Usuario>> ListarNaoConfirmadosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários bloqueados
    /// </summary>
    Task<List<Usuario>> ListarBloqueadosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva as mudanças no banco de dados
    /// </summary>
    Task<int> SalvarMudancasAsync(CancellationToken cancellationToken = default);
}
