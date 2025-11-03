using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

/// <summary>
/// Interface para repositório de Informações Financeiras do Cliente
/// Define contrato para operações de persistência de dados financeiros
/// </summary>
public interface IInformacoesFinanceirasClienteRepository
{
    /// <summary>
    /// Adiciona novas informações financeiras
    /// </summary>
    Task<InformacoesFinanceirasCliente> AdicionarAsync(InformacoesFinanceirasCliente informacoes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza informações financeiras existentes
    /// </summary>
    Task<InformacoesFinanceirasCliente> AtualizarAsync(InformacoesFinanceirasCliente informacoes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove informações financeiras por ID
    /// </summary>
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém informações financeiras por ID
    /// </summary>
    Task<InformacoesFinanceirasCliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém informações financeiras por ID do cliente
    /// </summary>
    Task<InformacoesFinanceirasCliente?> ObterPorClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todas as informações financeiras
    /// </summary>
    Task<List<InformacoesFinanceirasCliente>> ListarTodosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista informações financeiras por ranking
    /// </summary>
    Task<List<InformacoesFinanceirasCliente>> ListarPorRankingAsync(int ranking, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista informações financeiras com score acima do especificado
    /// </summary>
    Task<List<InformacoesFinanceirasCliente>> ListarComScoreAcimaAsync(int scoreMinimo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista clientes aptos para cartão de crédito
    /// </summary>
    Task<List<InformacoesFinanceirasCliente>> ListarAptosParaCartaoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista clientes em risco
    /// </summary>
    Task<List<InformacoesFinanceirasCliente>> ListarEmRiscoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe informações financeiras para um cliente
    /// </summary>
    Task<bool> ExisteParaClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém clientes que necessitam nova análise de crédito
    /// </summary>
    Task<List<InformacoesFinanceirasCliente>> ListarPendentesAnaliseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva as mudanças no banco de dados
    /// </summary>
    Task<int> SalvarMudancasAsync(CancellationToken cancellationToken = default);
}
