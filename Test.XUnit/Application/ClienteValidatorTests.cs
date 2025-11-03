namespace Test.XUnit.Application;

/// <summary>
/// Testes unitários para os validadores de Cliente
/// </summary>
public class ClienteCreateDtoValidatorTests
{
    private readonly ClienteCreateDtoValidator _validator;

    public ClienteCreateDtoValidatorTests()
    {
        _validator = new ClienteCreateDtoValidator();
    }

    [Fact]
    public void Validar_ComDadosValidos_DevePassarNaValidacao()
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validar_ComNomeVazio_DeveRetornarErro()
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Nome");
    }

    [Theory]
    [InlineData("AB")] // Muito curto
    [InlineData("João123")] // Contém números
    [InlineData("João @Silva")] // Contém caracteres especiais
    public void Validar_ComNomeInvalido_DeveRetornarErro(string nome)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = nome,
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("joao")] // Sem @
    [InlineData("joao@")] // Sem domínio
    [InlineData("@example.com")] // Sem usuário
    public void Validar_ComEmailInvalido_DeveRetornarErro(string email)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = email,
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("123")] // Muito curto
    [InlineData("1234567")] // Insuficiente
    public void Validar_ComTelefoneInvalido_DeveRetornarErro(string telefone)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = telefone,
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("123")] // Muito curto
    [InlineData("000.000.000-00")] // CPF inválido
    public void Validar_ComCpfInvalido_DeveRetornarErro(string cpf)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = cpf,
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("Rua")] // Muito curto
    [InlineData("")] // Vazio
    public void Validar_ComEnderecoInvalido_DeveRetornarErro(string endereco)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = endereco,
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("SP")] // Válido
    [InlineData("RJ")] // Válido
    public void Validar_ComEstadoValido_DevePassar(string estado)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = estado,
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("São Paulo")] // Muito longo
    [InlineData("S")] // Muito curto
    [InlineData("sp")] // Minúsculas
    public void Validar_ComEstadoInvalido_DeveRetornarErro(string estado)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = estado,
            Cep = "01234-567"
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("01234-567")] // Válido com hífen
    [InlineData("01234567")] // Válido sem hífen
    public void Validar_ComCepValido_DevePassar(string cep)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = cep
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("123")] // Muito curto
    [InlineData("01234")] // Insuficiente
    public void Validar_ComCepInvalido_DeveRetornarErro(string cep)
    {
        // Arrange
        var dto = new ClienteCreateDto
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Telefone = "11987654321",
            Cpf = "123.456.789-10",
            Endereco = "Rua das Flores, 123",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = cep
        };

        // Act
        var resultado = _validator.Validate(dto);

        // Assert
        resultado.IsValid.Should().BeFalse();
    }
}
