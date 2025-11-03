namespace Test.XUnit.Infrastructure;

/// <summary>
/// Testes unitários para BaseRepository
/// </summary>
public class BaseRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly ClienteRepository _repository;

    public BaseRepositoryTests()
    {
        // Usar in-memory database para testes
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ClienteRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DeveAdicionarClienteAoBancoDados()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "123.456.789-10",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234-567"
        );

        // Act
        await _repository.AddAsync(cliente);
        await _repository.SaveChangesAsync();

        // Assert
        var clienteRecuperado = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id);
        clienteRecuperado.Should().NotBeNull();
        clienteRecuperado.Nome.Should().Be(cliente.Nome);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarClientePorId()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "123.456.789-10",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234-567"
        );

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.GetByIdAsync(cliente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(cliente.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInexistente_DeveRetornarNulo()
    {
        // Act
        var resultado = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_DeveRetornarTodosOsClientes()
    {
        // Arrange
        var clientes = new List<Cliente>
        {
            Cliente.Criar("Cliente 1", "cliente1@example.com", "11987654321", "123.456.789-10",
                "Rua 1", "São Paulo", "SP", "01234-567"),
            Cliente.Criar("Cliente 2", "cliente2@example.com", "11987654322", "123.456.789-11",
                "Rua 2", "Rio de Janeiro", "RJ", "01234-568")
        };

        _context.Clientes.AddRange(clientes);
        await _context.SaveChangesAsync();

        // Act
        var resultado = _repository.GetAll().ToList();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllActive_DeveRetornarApenasClientesAtivos()
    {
        // Arrange
        var cliente1 = Cliente.Criar("Cliente 1", "cliente1@example.com", "11987654321", "123.456.789-10",
            "Rua 1", "São Paulo", "SP", "01234-567");
        var cliente2 = Cliente.Criar("Cliente 2", "cliente2@example.com", "11987654322", "123.456.789-11",
            "Rua 2", "Rio de Janeiro", "RJ", "01234-568");

        cliente2.Desativar();

        _context.Clientes.AddRange(cliente1, cliente2);
        await _context.SaveChangesAsync();

        // Act
        var resultado = _repository.GetAllActive().ToList();

        // Assert
        resultado.Should().HaveCount(1);
        resultado.First().Id.Should().Be(cliente1.Id);
    }

    [Fact]
    public async Task Update_DeveAtualizarCliente()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "123.456.789-10",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234-567"
        );

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        cliente.Atualizar("João Silva Santos", "joao.santos@example.com", "11999999999",
            "Avenida Paulista, 1000", "São Paulo", "SP", "01310-100");

        _repository.Update(cliente);
        await _repository.SaveChangesAsync();

        // Assert
        var clienteAtualizado = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id);
        clienteAtualizado.Nome.Should().Be("João Silva Santos");
        clienteAtualizado.Email.Should().Be("joao.santos@example.com");
    }

    [Fact]
    public async Task DeleteAsync_DeveRemoverCliente()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "123.456.789-10",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234-567"
        );

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var deletado = await _repository.DeleteAsync(cliente.Id);
        await _repository.SaveChangesAsync();

        // Assert
        deletado.Should().BeTrue();
        var clienteRecuperado = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id);
        clienteRecuperado.Should().BeNull();
    }

    [Fact]
    public async Task DeactivateAsync_DeveDesativarCliente()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "123.456.789-10",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234-567"
        );

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeactivateAsync(cliente.Id);
        await _repository.SaveChangesAsync();

        // Assert
        var clienteDesativado = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id);
        clienteDesativado.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_DeveRetornarTrueParaClienteExistente()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "123.456.789-10",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234-567"
        );

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var existe = await _repository.ExistsAsync(cliente.Id);

        // Assert
        existe.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_DeveRetornarFalseParaClienteInexistente()
    {
        // Act
        var existe = await _repository.ExistsAsync(Guid.NewGuid());

        // Assert
        existe.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_DeveRetornarQuantidadeDeClientes()
    {
        // Arrange
        var clientes = new List<Cliente>
        {
            Cliente.Criar("Cliente 1", "cliente1@example.com", "11987654321", "123.456.789-10",
                "Rua 1", "São Paulo", "SP", "01234-567"),
            Cliente.Criar("Cliente 2", "cliente2@example.com", "11987654322", "123.456.789-11",
                "Rua 2", "Rio de Janeiro", "RJ", "01234-568")
        };

        _context.Clientes.AddRange(clientes);
        await _context.SaveChangesAsync();

        // Act
        var quantidade = await _repository.CountAsync();

        // Assert
        quantidade.Should().Be(2);
    }

    [Fact]
    public async Task CountActiveAsync_DeveRetornarQuantidadeDeClientesAtivos()
    {
        // Arrange
        var cliente1 = Cliente.Criar("Cliente 1", "cliente1@example.com", "11987654321", "123.456.789-10",
            "Rua 1", "São Paulo", "SP", "01234-567");
        var cliente2 = Cliente.Criar("Cliente 2", "cliente2@example.com", "11987654322", "123.456.789-11",
            "Rua 2", "Rio de Janeiro", "RJ", "01234-568");

        cliente2.Desativar();

        _context.Clientes.AddRange(cliente1, cliente2);
        await _context.SaveChangesAsync();

        // Act
        var quantidade = await _repository.CountActiveAsync();

        // Assert
        quantidade.Should().Be(1);
    }
}
