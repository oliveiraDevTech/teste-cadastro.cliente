namespace Test.XUnit.Application;

/// <summary>
/// Testes unitários para ClienteService
/// </summary>
public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _mockRepository;
    private readonly Mock<Driven.RabbitMQ.Interfaces.IMessagePublisher> _mockMessagePublisher;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<ClienteService>> _mockLogger;
    private readonly Mock<Microsoft.Extensions.Options.IOptions<Driven.RabbitMQ.Settings.RabbitMQSettings>> _mockRabbitMQSettings;
    private readonly HttpClient _httpClient;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _mockRepository = new Mock<IClienteRepository>();
        _mockMessagePublisher = new Mock<Driven.RabbitMQ.Interfaces.IMessagePublisher>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<ClienteService>>();
        
        // Configurar mock das settings do RabbitMQ
        var rabbitMQSettings = new Driven.RabbitMQ.Settings.RabbitMQSettings
        {
            Queues = new Driven.RabbitMQ.Settings.RabbitMQQueuesSettings
            {
                ClienteCadastrado = "cliente.cadastrado",
                AnaliseCreditoComplete = "analise.credito.complete",
                CartaoEmissaoPedido = "cartao.emissao.pedido"
            }
        };
        _mockRabbitMQSettings = new Mock<Microsoft.Extensions.Options.IOptions<Driven.RabbitMQ.Settings.RabbitMQSettings>>();
        _mockRabbitMQSettings.Setup(m => m.Value).Returns(rabbitMQSettings);
        
        // Configurar HttpClient para testes
        _httpClient = new HttpClient();
        
        _service = new ClienteService(_mockRepository.Object, _mockMessagePublisher.Object, _mockLogger.Object, _mockRabbitMQSettings.Object, _httpClient);
    }

    #region ObterPorIdAsync Tests

    [Fact]
    public async Task ObterPorIdAsync_ComIdValido_DeveRetornarCliente()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var clienteDto = new ClienteResponseDto
        {
            Id = clienteId,
            Nome = "João Silva",
            Email = "joao@example.com",
            Ativo = true
        };

        _mockRepository.Setup(r => r.ObterPorIdAsync(clienteId))
            .ReturnsAsync(clienteDto);

        // Act
        var resultado = await _service.ObterPorIdAsync(clienteId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().Be(clienteDto);
        _mockRepository.Verify(r => r.ObterPorIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInvalido_DeveRetornarErro()
    {
        // Arrange
        var clienteId = Guid.Empty;

        // Act
        var resultado = await _service.ObterPorIdAsync(clienteId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("inválido");
        resultado.Erros.Should().HaveCount(1);
    }

    [Fact]
    public async Task ObterPorIdAsync_ClienteNaoEncontrado_DeveRetornarNulo()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        _mockRepository.Setup(r => r.ObterPorIdAsync(clienteId))
            .ReturnsAsync((ClienteResponseDto?)null);

        // Act
        var resultado = await _service.ObterPorIdAsync(clienteId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("não encontrado");
    }

    #endregion

    #region ListarAsync Tests

    [Fact]
    public async Task ListarAsync_DeveRetornarClientesPaginados()
    {
        // Arrange
        var clientesDto = new[]
        {
            new ClienteListDto { Id = Guid.NewGuid(), Nome = "Cliente 1", Email = "cliente1@example.com" },
            new ClienteListDto { Id = Guid.NewGuid(), Nome = "Cliente 2", Email = "cliente2@example.com" }
        };

        var paginatedResponse = new PaginatedResponseDto<ClienteListDto>
        {
            PaginaAtual = 1,
            ItensPorPagina = 10,
            TotalItens = 2,
            TotalPaginas = 1,
            Itens = clientesDto.ToList()
        };

        _mockRepository.Setup(r => r.ListarAsync(1, 10))
            .ReturnsAsync(paginatedResponse);

        // Act
        var resultado = await _service.ListarAsync(1, 10);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.Itens.Should().HaveCount(2);
        resultado.Dados.PaginaAtual.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ListarAsync_ComPaginaInvalida_DeveUsarPaginaPadrao(int pagina)
    {
        // Arrange
        var paginatedResponse = new PaginatedResponseDto<ClienteListDto>
        {
            PaginaAtual = 1,
            Itens = new List<ClienteListDto>()
        };

        _mockRepository.Setup(r => r.ListarAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginatedResponse);

        // Act
        await _service.ListarAsync(pagina, 10);

        // Assert
        _mockRepository.Verify(r => r.ListarAsync(1, 10), Times.Once);
    }

    #endregion

    #region CriarAsync Tests

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveRetornarClienteCriado()
    {
        // Arrange
        var clienteCreateDto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "52998224725",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234567"
        };

        var clienteResponseDto = new ClienteResponseDto
        {
            Id = Guid.NewGuid(),
            Nome = clienteCreateDto.Nome,
            Email = clienteCreateDto.Email,
            Ativo = true
        };

        _mockRepository.Setup(r => r.EmailJaRegistradoAsync(clienteCreateDto.Email, null))
            .ReturnsAsync(false);

        _mockRepository.Setup(r => r.CpfJaRegistradoAsync(clienteCreateDto.Cpf, null))
            .ReturnsAsync(false);

        _mockRepository.Setup(r => r.CriarAsync(It.IsAny<ClienteCreateDto>()))
            .ReturnsAsync(clienteResponseDto);

        // Act
        var resultado = await _service.CriarAsync(clienteCreateDto);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CriarAsync_ComEmailDuplicado_DeveRetornarErro()
    {
        // Arrange
        var clienteCreateDto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "52998224725",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234567"
        };

        _mockRepository.Setup(r => r.EmailJaRegistradoAsync(clienteCreateDto.Email, null))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.CriarAsync(clienteCreateDto);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("Email já registrado");
    }

    [Fact]
    public async Task CriarAsync_ComCpfDuplicado_DeveRetornarErro()
    {
        // Arrange
        var clienteCreateDto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "52998224725",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234567"
        };

        _mockRepository.Setup(r => r.EmailJaRegistradoAsync(clienteCreateDto.Email, null))
            .ReturnsAsync(false);

        _mockRepository.Setup(r => r.CpfJaRegistradoAsync(clienteCreateDto.Cpf, null))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.CriarAsync(clienteCreateDto);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("CPF já registrado");
    }

    [Fact]
    public async Task CriarAsync_ComDadosInvalidos_DeveRetornarErro()
    {
        // Arrange
        var clienteCreateDto = new ClienteCreateDto
        {
            Nome = "AB", // Muito curto
            Email = "email-invalido", // Email inválido
            Telefone = "123", // Muito curto
            Cpf = "123", // Inválido
            Endereco = "Rua", // Muito curto
            Cidade = "SP", // Muito curto
            Estado = "São Paulo", // Deve ser 2 caracteres
            Cep = "123" // Inválido
        };

        // Act
        var resultado = await _service.CriarAsync(clienteCreateDto);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erros.Should().NotBeEmpty();
    }

    #endregion

    #region DeletarAsync Tests

    [Fact]
    public async Task DeletarAsync_ComClienteValido_DeveRetornarSucesso()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExisteAsync(clienteId))
            .ReturnsAsync(true);

        _mockRepository.Setup(r => r.DeletarAsync(clienteId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.DeletarAsync(clienteId);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        _mockRepository.Verify(r => r.DeletarAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_ComClienteInexistente_DeveRetornarErro()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExisteAsync(clienteId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _service.DeletarAsync(clienteId);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Contain("não encontrado");
    }

    #endregion
}
