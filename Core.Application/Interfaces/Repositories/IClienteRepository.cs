namespace Core.Application.Interfaces.Repositories;

/// <summary>
/// Interface do repositório de clientes
/// Define os contratos para operações de persistência de clientes
/// </summary>
public interface IClienteRepository
{
    /// <summary>
    /// Obtém um cliente por seu ID
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <returns>Cliente encontrado ou null se não existir</returns>
    Task<ClienteResponseDto?> ObterPorIdAsync(Guid id);

    /// <summary>
    /// Obtém um cliente por email
    /// </summary>
    /// <param name="email">Email do cliente</param>
    /// <returns>Cliente encontrado ou null se não existir</returns>
    Task<ClienteResponseDto?> ObterPorEmailAsync(string email);

    /// <summary>
    /// Obtém um cliente por CPF
    /// </summary>
    /// <param name="cpf">CPF do cliente</param>
    /// <returns>Cliente encontrado ou null se não existir</returns>
    Task<ClienteResponseDto?> ObterPorCpfAsync(string cpf);

    /// <summary>
    /// Lista todos os clientes com paginação
    /// </summary>
    /// <param name="pagina">Número da página (começando em 1)</param>
    /// <param name="itensPorPagina">Quantidade de itens por página</param>
    /// <returns>Lista paginada de clientes</returns>
    Task<PaginatedResponseDto<ClienteListDto>> ListarAsync(int pagina = 1, int itensPorPagina = 10);

    /// <summary>
    /// Pesquisa clientes por nome
    /// </summary>
    /// <param name="nome">Nome ou parte do nome do cliente</param>
    /// <param name="pagina">Número da página (começando em 1)</param>
    /// <param name="itensPorPagina">Quantidade de itens por página</param>
    /// <returns>Lista paginada de clientes encontrados</returns>
    Task<PaginatedResponseDto<ClienteListDto>> PesquisarPorNomeAsync(string nome, int pagina = 1, int itensPorPagina = 10);

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    /// <param name="cliente">Dados do cliente a criar</param>
    /// <returns>Cliente criado com ID preenchido</returns>
    Task<ClienteResponseDto> CriarAsync(ClienteCreateDto cliente);

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    /// <param name="cliente">Dados atualizados do cliente</param>
    /// <returns>Cliente atualizado</returns>
    Task<ClienteResponseDto> AtualizarAsync(ClienteUpdateDto cliente);

    /// <summary>
    /// Deleta um cliente (exclusão lógica)
    /// </summary>
    /// <param name="id">ID do cliente a deletar</param>
    /// <returns>True se deletado com sucesso, False caso contrário</returns>
    Task<bool> DeletarAsync(Guid id);

    /// <summary>
    /// Verifica se um cliente já existe
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <returns>True se existe, False caso contrário</returns>
    Task<bool> ExisteAsync(Guid id);

    /// <summary>
    /// Verifica se um email já está registrado
    /// </summary>
    /// <param name="email">Email a verificar</param>
    /// <param name="idClienteExcluir">ID do cliente para excluir da verificação (opcional)</param>
    /// <returns>True se email está registrado, False caso contrário</returns>
    Task<bool> EmailJaRegistradoAsync(string email, Guid? idClienteExcluir = null);

    /// <summary>
    /// Verifica se um CPF já está registrado
    /// </summary>
    /// <param name="cpf">CPF a verificar</param>
    /// <param name="idClienteExcluir">ID do cliente para excluir da verificação (opcional)</param>
    /// <returns>True se CPF está registrado, False caso contrário</returns>
    Task<bool> CpfJaRegistradoAsync(string cpf, Guid? idClienteExcluir = null);
}
