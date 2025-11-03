using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Driven.SqlLite.Data;
using Microsoft.EntityFrameworkCore;

namespace Driven.SqlLite.Repositories;

/// <summary>
/// Implementação do repositório de Informações Financeiras do Cliente
/// Gerencia persistência de dados financeiros usando Entity Framework
/// </summary>
public class InformacoesFinanceirasClienteRepository : IInformacoesFinanceirasClienteRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<InformacoesFinanceirasCliente> _dbSet;

    /// <summary>
    /// Construtor do repositório
    /// </summary>
    public InformacoesFinanceirasClienteRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.InformacoesFinanceirasClientes;
    }

    /// <inheritdoc />
    public async Task<InformacoesFinanceirasCliente> AdicionarAsync(InformacoesFinanceirasCliente informacoes, CancellationToken cancellationToken = default)
    {
        if (informacoes == null)
            throw new ArgumentNullException(nameof(informacoes), "Informações financeiras não pode ser nula");

        await _dbSet.AddAsync(informacoes, cancellationToken);
        return informacoes;
    }

    /// <inheritdoc />
    public Task<InformacoesFinanceirasCliente> AtualizarAsync(InformacoesFinanceirasCliente informacoes, CancellationToken cancellationToken = default)
    {
        if (informacoes == null)
            throw new ArgumentNullException(nameof(informacoes), "Informações financeiras não pode ser nula");

        _dbSet.Update(informacoes);
        return Task.FromResult(informacoes);
    }

    /// <inheritdoc />
    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var informacoes = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (informacoes == null)
            return false;

        _dbSet.Remove(informacoes);
        return true;
    }

    /// <inheritdoc />
    public async Task<InformacoesFinanceirasCliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.Ativo, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InformacoesFinanceirasCliente?> ObterPorClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.ClienteId == clienteId && x.Ativo, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<InformacoesFinanceirasCliente>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(x => x.Ativo).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<InformacoesFinanceirasCliente>> ListarPorRankingAsync(int ranking, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.RankingCredito == ranking && x.Ativo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<InformacoesFinanceirasCliente>> ListarComScoreAcimaAsync(int scoreMinimo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.ScoreCredito >= scoreMinimo && x.Ativo)
            .OrderByDescending(x => x.ScoreCredito)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<InformacoesFinanceirasCliente>> ListarAptosParaCartaoAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.AptoParaCartaoCredito && x.Ativo && x.RankingCredito >= 3 && x.ScoreCredito >= 600)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<InformacoesFinanceirasCliente>> ListarEmRiscoAsync(CancellationToken cancellationToken = default)
    {
        var informacoesList = await _dbSet.Where(x => x.Ativo).ToListAsync(cancellationToken);
        return informacoesList.Where(x => x.EstaEmRisco()).ToList();
    }

    /// <inheritdoc />
    public async Task<bool> ExisteParaClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(x => x.ClienteId == clienteId && x.Ativo, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<InformacoesFinanceirasCliente>> ListarPendentesAnaliseAsync(CancellationToken cancellationToken = default)
    {
        var informacoesList = await _dbSet.Where(x => x.Ativo).ToListAsync(cancellationToken);
        return informacoesList.Where(x => x.AnaliseExpirada()).ToList();
    }

    /// <inheritdoc />
    public async Task<int> SalvarMudancasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
