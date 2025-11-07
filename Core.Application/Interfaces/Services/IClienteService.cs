namespace Core.Application.Interfaces.Services;

/// <summary>
/// Interface do serviço de aplicação para clientes
/// Define os casos de uso relacionados a clientes
/// </summary>
public interface IClienteService
{
    /// <summary>
    /// Obtém um cliente por seu ID
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <returns>Resposta com dados do cliente ou erro</returns>
    Task<ApiResponseDto<ClienteResponseDto>> ObterPorIdAsync(Guid id);

    /// <summary>
    /// Lista todos os clientes com paginação
    /// </summary>
    /// <param name="pagina">Número da página (começando em 1)</param>
    /// <param name="itensPorPagina">Quantidade de itens por página</param>
    /// <returns>Resposta com lista paginada de clientes</returns>
    Task<ApiResponseDto<PaginatedResponseDto<ClienteListDto>>> ListarAsync(int pagina = 1, int itensPorPagina = 10);

    /// <summary>
    /// Pesquisa clientes por nome
    /// </summary>
    /// <param name="nome">Nome ou parte do nome do cliente</param>
    /// <param name="pagina">Número da página (começando em 1)</param>
    /// <param name="itensPorPagina">Quantidade de itens por página</param>
    /// <returns>Resposta com lista paginada de clientes encontrados</returns>
    Task<ApiResponseDto<PaginatedResponseDto<ClienteListDto>>> PesquisarPorNomeAsync(string nome, int pagina = 1, int itensPorPagina = 10);

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    /// <param name="clienteCreateDto">Dados do cliente a criar</param>
    /// <returns>Resposta com dados do cliente criado ou erros de validação</returns>
    Task<ApiResponseDto<ClienteResponseDto>> CriarAsync(ClienteCreateDto clienteCreateDto);

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    /// <param name="clienteUpdateDto">Dados atualizados do cliente</param>
    /// <returns>Resposta com dados do cliente atualizado ou erros de validação</returns>
    Task<ApiResponseDto<ClienteResponseDto>> AtualizarAsync(ClienteUpdateDto clienteUpdateDto);

    /// <summary>
    /// Deleta um cliente
    /// </summary>
    /// <param name="id">ID do cliente a deletar</param>
    /// <returns>Resposta indicando sucesso ou erro na exclusão</returns>
    Task<ApiResponseDto> DeletarAsync(Guid id);

    /// <summary>
    /// Solicita emissão de cartão de crédito para o cliente
    /// Publica evento na fila cartao.emissao.pedido para processamento
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <returns>Resposta indicando se a solicitação foi aceita</returns>
    Task<ApiResponseDto<object>> SolicitarEmissaoCartaoAsync(Guid clienteId);

    /// <summary>
    /// Obtém o status da emissão de cartão do cliente
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <returns>Resposta com status da emissão</returns>
    Task<ApiResponseDto<object>> ObterStatusEmissaoCartaoAsync(Guid clienteId);

    /// <summary>
    /// Lista todos os cartões de crédito do cliente
    /// Consulta o microserviço de emissão de cartões
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <returns>Resposta com lista de cartões</returns>
    Task<ApiResponseDto<List<object>>> ObterCartoesClienteAsync(Guid clienteId);
}
