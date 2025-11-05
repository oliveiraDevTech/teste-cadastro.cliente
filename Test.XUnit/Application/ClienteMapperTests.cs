namespace Test.XUnit.Application;

/// <summary>
/// Testes unitários para ClienteMapper
/// </summary>
public class ClienteMapperTests
{
    [Fact]
    public void ToResponseDto_DeveConverterClienteParaDto()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "52998224725",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234567"
        );

        // Act
        var dto = ClienteMapper.ToResponseDto(cliente);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(cliente.Id);
        dto.Nome.Should().Be(cliente.Nome);
        dto.Email.Should().Be(cliente.Email);
        dto.Telefone.Should().Be(cliente.Telefone);
        dto.Cpf.Should().Be(cliente.Cpf);
        dto.Endereco.Should().Be(cliente.Endereco);
        dto.Cidade.Should().Be(cliente.Cidade);
        dto.Estado.Should().Be(cliente.Estado);
        dto.Cep.Should().Be(cliente.Cep);
        dto.Ativo.Should().Be(cliente.Ativo);
    }

    [Fact]
    public void ToListDto_DeveConverterClienteParaDtoLista()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "52998224725",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234567"
        );

        // Act
        var dto = ClienteMapper.ToListDto(cliente);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(cliente.Id);
        dto.Nome.Should().Be(cliente.Nome);
        dto.Email.Should().Be(cliente.Email);
        dto.Telefone.Should().Be(cliente.Telefone);
        dto.Cidade.Should().Be(cliente.Cidade);
        dto.Ativo.Should().Be(cliente.Ativo);
    }

    [Fact]
    public void ToEntity_DeveConverterDtoParaCliente()
    {
        // Arrange
        var createDto = new ClienteCreateDto
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

        // Act
        var cliente = ClienteMapper.ToEntity(createDto);

        // Assert
        cliente.Should().NotBeNull();
        cliente.Nome.Should().Be(createDto.Nome);
        cliente.Email.Should().Be(createDto.Email);
        cliente.Telefone.Should().Be(createDto.Telefone);
        cliente.Cpf.Should().Be(createDto.Cpf);
        cliente.Endereco.Should().Be(createDto.Endereco);
        cliente.Cidade.Should().Be(createDto.Cidade);
        cliente.Estado.Should().Be(createDto.Estado);
        cliente.Cep.Should().Be(createDto.Cep);
    }

    [Fact]
    public void ApplyUpdate_DeveAtualizarClienteComDadosDoDto()
    {
        // Arrange
        var cliente = Cliente.Criar(
            "João Silva",
            "joao@example.com",
            "11987654321",
            "52998224725",
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234567"
        );

        var updateDto = new ClienteUpdateDto
        {
            Id = cliente.Id,
            Nome = "João Silva Santos",
            Email = "joao.santos@example.com",
            Telefone = "11999999999",
            Endereco = "Avenida Paulista, 1000",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01310100"
        };

        // Act
        ClienteMapper.ApplyUpdate(updateDto, cliente);

        // Assert
        cliente.Nome.Should().Be(updateDto.Nome);
        cliente.Email.Should().Be(updateDto.Email);
        cliente.Telefone.Should().Be(updateDto.Telefone);
        cliente.Endereco.Should().Be(updateDto.Endereco);
        cliente.Cidade.Should().Be(updateDto.Cidade);
        cliente.Estado.Should().Be(updateDto.Estado);
        cliente.Cep.Should().Be(updateDto.Cep);
    }

    [Fact]
    public void ToResponseDtoList_DeveConverterListaDeClientes()
    {
        // Arrange
        var clientes = new List<Cliente>
        {
            Cliente.Criar("Cliente 1", "cliente1@example.com", "11987654321", "52998224725",
                "Rua 1", "São Paulo", "SP", "01234567"),
            Cliente.Criar("Cliente 2", "cliente2@example.com", "11987654322", "11144477735",
                "Rua 2", "São Paulo", "SP", "01234568")
        };

        // Act
        var dtos = ClienteMapper.ToResponseDtoList(clientes);

        // Assert
        dtos.Should().HaveCount(2);
        dtos.Should().AllSatisfy(d => d.Should().BeOfType<ClienteResponseDto>());
    }

    [Fact]
    public void ToListDtoList_DeveConverterListaParaDtosLista()
    {
        // Arrange
        var clientes = new List<Cliente>
        {
            Cliente.Criar("Cliente 1", "cliente1@example.com", "11987654321", "52998224725",
                "Rua 1", "São Paulo", "SP", "01234567"),
            Cliente.Criar("Cliente 2", "cliente2@example.com", "11987654322", "11144477735",
                "Rua 2", "São Paulo", "SP", "01234568")
        };

        // Act
        var dtos = ClienteMapper.ToListDtoList(clientes);

        // Assert
        dtos.Should().HaveCount(2);
        dtos.Should().AllSatisfy(d => d.Should().BeOfType<ClienteListDto>());
    }
}
