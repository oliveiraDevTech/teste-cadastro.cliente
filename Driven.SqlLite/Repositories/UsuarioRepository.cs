using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Driven.SqlLite.Data;
using Microsoft.EntityFrameworkCore;

namespace Driven.SqlLite.Repositories;

/// <summary>
/// Implementação do repositório de Usuário
/// Gerencia persistência de dados de usuário usando Entity Framework
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Usuario> _dbSet;

    /// <summary>
    /// Construtor do repositório
    /// </summary>
    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Usuarios;
    }

    /// <inheritdoc />
    public async Task<Usuario> AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        if (usuario == null)
            throw new ArgumentNullException(nameof(usuario), "Usuário não pode ser nulo");

        await _dbSet.AddAsync(usuario, cancellationToken);
        return usuario;
    }

    /// <inheritdoc />
    public Task<Usuario> AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        if (usuario == null)
            throw new ArgumentNullException(nameof(usuario), "Usuário não pode ser nulo");

        _dbSet.Update(usuario);
        return Task.FromResult(usuario);
    }

    /// <inheritdoc />
    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var usuario = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (usuario == null)
            return false;

        _dbSet.Remove(usuario);
        return true;
    }

    /// <inheritdoc />
    public async Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.Ativo, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(login))
            return null;

        return await _dbSet.FirstOrDefaultAsync(
            x => x.Login.ToLower() == login.ToLower() && x.Ativo,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await _dbSet.FirstOrDefaultAsync(
            x => x.Email.ToLower() == email.ToLower() && x.Ativo,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Usuario?> ObterPorLoginOuEmailAsync(string loginOuEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(loginOuEmail))
            return null;

        var loginOuEmailLower = loginOuEmail.ToLower();
        return await _dbSet.FirstOrDefaultAsync(
            x => (x.Login.ToLower() == loginOuEmailLower || x.Email.ToLower() == loginOuEmailLower) && x.Ativo,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Usuario>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(x => x.Ativo).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Usuario>> ListarAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(x => x.Ativo).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Usuario>> ListarPorTipoAsync(string tipoUsuario, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tipoUsuario))
            return new List<Usuario>();

        return await _dbSet
            .Where(x => x.TipoUsuario == tipoUsuario && x.Ativo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(List<Usuario> usuarios, int total)> ListarComPaginacaoAsync(
        int pagina,
        int itensPorPagina,
        string? filtro = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(x => x.Ativo);

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var filtroLower = filtro.ToLower();
            query = query.Where(x =>
                x.Login.ToLower().Contains(filtroLower) ||
                x.Email.ToLower().Contains(filtroLower) ||
                x.NomeCompleto.ToLower().Contains(filtroLower));
        }

        var total = await query.CountAsync(cancellationToken);

        var usuarios = await query
            .OrderByDescending(x => x.DataCriacao)
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync(cancellationToken);

        return (usuarios, total);
    }

    /// <inheritdoc />
    public async Task<List<Usuario>> ListarAdministradoresAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.IsAdmin && x.Ativo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Usuario>> ListarPorClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.ClienteId == clienteId && x.Ativo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExisteLoginAsync(string login, Guid? idExcluir = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(login))
            return false;

        var loginLower = login.ToLower();
        var query = _dbSet.Where(x => x.Login.ToLower() == loginLower);

        if (idExcluir.HasValue)
            query = query.Where(x => x.Id != idExcluir.Value);

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExisteEmailAsync(string email, Guid? idExcluir = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailLower = email.ToLower();
        var query = _dbSet.Where(x => x.Email.ToLower() == emailLower);

        if (idExcluir.HasValue)
            query = query.Where(x => x.Id != idExcluir.Value);

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Usuario>> ListarNaoConfirmadosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => !x.EmailConfirmado && x.Ativo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Usuario>> ListarBloqueadosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.DataBloqueio.HasValue && !x.Ativo)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> SalvarMudancasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
