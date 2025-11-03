using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Application.DTOs;
using Core.Application.Mappers;
using Driven.SqlLite.Data;

namespace Driven.SqlLite.Repositories;

/// <summary>
/// Repositório de Cliente implementando a interface IClienteRepository
/// Herda de BaseRepository para reutilizar operações genéricas CRUD
/// Responsável pela persistência de dados de clientes no SQLite
/// </summary>
public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    /// <summary>
    /// Construtor do repositório
    /// </summary>
    /// <param name="context">Contexto do banco de dados injetado por DI</param>
    public ClienteRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtém um cliente por seu ID
    /// </summary>
    public async Task<ClienteResponseDto?> ObterPorIdAsync(Guid id)
    {
        var cliente = await Context.Clientes
            .Where(c => c.Id == id && c.Ativo)
            .FirstOrDefaultAsync();

        return cliente == null ? null : ClienteMapper.ToResponseDto(cliente);
    }

    /// <summary>
    /// Obtém um cliente por email
    /// </summary>
    public async Task<ClienteResponseDto?> ObterPorEmailAsync(string email)
    {
        var cliente = await Context.Clientes
            .Where(c => c.Email == email && c.Ativo)
            .FirstOrDefaultAsync();

        return cliente == null ? null : ClienteMapper.ToResponseDto(cliente);
    }

    /// <summary>
    /// Obtém um cliente por CPF
    /// </summary>
    public async Task<ClienteResponseDto?> ObterPorCpfAsync(string cpf)
    {
        var cliente = await Context.Clientes
            .Where(c => c.Cpf == cpf && c.Ativo)
            .FirstOrDefaultAsync();

        return cliente == null ? null : ClienteMapper.ToResponseDto(cliente);
    }

    /// <summary>
    /// Lista todos os clientes com paginação
    /// </summary>
    public async Task<PaginatedResponseDto<ClienteListDto>> ListarAsync(int pagina = 1, int itensPorPagina = 10)
    {
        var totalItens = await Context.Clientes
            .Where(c => c.Ativo)
            .CountAsync();

        var clientes = await Context.Clientes
            .Where(c => c.Ativo)
            .OrderByDescending(c => c.DataCriacao)
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync();

        var clientesDto = ClienteMapper.ToListDtoList(clientes);

        return new PaginatedResponseDto<ClienteListDto>
        {
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalItens = totalItens,
            TotalPaginas = (totalItens + itensPorPagina - 1) / itensPorPagina,
            Itens = clientesDto
        };
    }

    /// <summary>
    /// Pesquisa clientes por nome
    /// </summary>
    public async Task<PaginatedResponseDto<ClienteListDto>> PesquisarPorNomeAsync(string nome, int pagina = 1, int itensPorPagina = 10)
    {
        var query = Context.Clientes
            .Where(c => c.Ativo && c.Nome.Contains(nome));

        var totalItens = await query.CountAsync();

        var clientes = await query
            .OrderByDescending(c => c.DataCriacao)
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync();

        var clientesDto = ClienteMapper.ToListDtoList(clientes);

        return new PaginatedResponseDto<ClienteListDto>
        {
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalItens = totalItens,
            TotalPaginas = (totalItens + itensPorPagina - 1) / itensPorPagina,
            Itens = clientesDto
        };
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    public async Task<ClienteResponseDto> CriarAsync(ClienteCreateDto clienteCreateDto)
    {
        var cliente = ClienteMapper.ToEntity(clienteCreateDto);

        Context.Clientes.Add(cliente);
        await Context.SaveChangesAsync();

        return ClienteMapper.ToResponseDto(cliente);
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    public async Task<ClienteResponseDto> AtualizarAsync(ClienteUpdateDto clienteUpdateDto)
    {
        var cliente = await Context.Clientes
            .FirstOrDefaultAsync(c => c.Id == clienteUpdateDto.Id);

        if (cliente == null)
            throw new InvalidOperationException($"Cliente com ID {clienteUpdateDto.Id} não encontrado");

        ClienteMapper.ApplyUpdate(clienteUpdateDto, cliente);

        Context.Clientes.Update(cliente);
        await Context.SaveChangesAsync();

        return ClienteMapper.ToResponseDto(cliente);
    }

    /// <summary>
    /// Deleta um cliente (exclusão lógica)
    /// </summary>
    public async Task<bool> DeletarAsync(Guid id)
    {
        var cliente = await Context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null)
            return false;

        cliente.Desativar();
        Context.Clientes.Update(cliente);
        await Context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Verifica se um cliente existe
    /// </summary>
    public async Task<bool> ExisteAsync(Guid id)
    {
        return await Context.Clientes
            .AnyAsync(c => c.Id == id);
    }

    /// <summary>
    /// Verifica se um email já está registrado
    /// </summary>
    public async Task<bool> EmailJaRegistradoAsync(string email, Guid? idClienteExcluir = null)
    {
        var query = Context.Clientes.Where(c => c.Email == email);

        if (idClienteExcluir.HasValue)
            query = query.Where(c => c.Id != idClienteExcluir.Value);

        return await query.AnyAsync();
    }

    /// <summary>
    /// Verifica se um CPF já está registrado
    /// </summary>
    public async Task<bool> CpfJaRegistradoAsync(string cpf, Guid? idClienteExcluir = null)
    {
        var query = Context.Clientes.Where(c => c.Cpf == cpf);

        if (idClienteExcluir.HasValue)
            query = query.Where(c => c.Id != idClienteExcluir.Value);

        return await query.AnyAsync();
    }
}
