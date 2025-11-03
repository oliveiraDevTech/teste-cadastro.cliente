namespace Test.XUnit.Domain;

/// <summary>
/// Testes unitários para a entidade Cliente
/// </summary>
public class ClienteTests
{
    [Fact]
    public void Criar_DeveRetornarClienteComDadosCorretos()
    {
        // Arrange
        var nome = "João Silva";
        var email = "joao@example.com";
        var telefone = "11987654321";
        var cpf = "123.456.789-10";
        var endereco = "Rua das Flores, 123";
        var cidade = "São Paulo";
        var estado = "SP";
        var cep = "01234-567";

        // Act
        var cliente = Cliente.Criar(nome, email, telefone, cpf, endereco, cidade, estado, cep);

        // Assert
        cliente.Should().NotBeNull();
        cliente.Id.Should().NotBe(Guid.Empty);
        cliente.Nome.Should().Be(nome);
        cliente.Email.Should().Be(email);
        cliente.Telefone.Should().Be(telefone);
        cliente.Cpf.Should().Be(cpf);
        cliente.Endereco.Should().Be(endereco);
        cliente.Cidade.Should().Be(cidade);
        cliente.Estado.Should().Be(estado);
        cliente.Cep.Should().Be(cep);
        cliente.Ativo.Should().BeTrue();
        cliente.DataCriacao.Should().NotBe(default(DateTime));
    }

    [Fact]
    public void Atualizar_DeveAlterarDadosDoCliente()
    {
        // Arrange
        var cliente = Cliente.Criar("João Silva", "joao@example.com", "11987654321",
            "123.456.789-10", "Rua das Flores, 123", "São Paulo", "SP", "01234-567");

        var novoNome = "João Silva Santos";
        var novoEmail = "joao.santos@example.com";
        var novoTelefone = "11999999999";
        var novoEndereco = "Avenida Paulista, 1000";
        var novaCidade = "São Paulo";
        var novoEstado = "SP";
        var novoCep = "01310-100";

        // Act
        cliente.Atualizar(novoNome, novoEmail, novoTelefone, novoEndereco,
            novaCidade, novoEstado, novoCep);

        // Assert
        cliente.Nome.Should().Be(novoNome);
        cliente.Email.Should().Be(novoEmail);
        cliente.Telefone.Should().Be(novoTelefone);
        cliente.Endereco.Should().Be(novoEndereco);
        cliente.Cidade.Should().Be(novaCidade);
        cliente.Estado.Should().Be(novoEstado);
        cliente.Cep.Should().Be(novoCep);
        cliente.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Desativar_DeveMarcarClienteComoInativo()
    {
        // Arrange
        var cliente = Cliente.Criar("João Silva", "joao@example.com", "11987654321",
            "123.456.789-10", "Rua das Flores, 123", "São Paulo", "SP", "01234-567");

        // Act
        cliente.Desativar();

        // Assert
        cliente.Ativo.Should().BeFalse();
        cliente.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Ativar_DeveMarcarClienteComoAtivo()
    {
        // Arrange
        var cliente = Cliente.Criar("João Silva", "joao@example.com", "11987654321",
            "123.456.789-10", "Rua das Flores, 123", "São Paulo", "SP", "01234-567");
        cliente.Desativar();

        // Act
        cliente.Ativar();

        // Assert
        cliente.Ativo.Should().BeTrue();
    }

    [Fact]
    public void ClienteNovo_DeveHerdarDaBaseEntity()
    {
        // Arrange & Act
        var cliente = Cliente.Criar("João Silva", "joao@example.com", "11987654321",
            "123.456.789-10", "Rua das Flores, 123", "São Paulo", "SP", "01234-567");

        // Assert
        cliente.Should().BeAssignableTo<BaseEntity>();
        cliente.Id.Should().NotBe(Guid.Empty);
        cliente.DataCriacao.Should().NotBe(default);
        cliente.Ativo.Should().BeTrue();
    }
}
