namespace Test.XUnit.Builders;

/// <summary>
/// Builder pattern para criar instâncias de ClienteUpdateDto para testes
/// </summary>
public class ClienteUpdateDtoBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _nome = "João Silva";
    private string _email = "joao@example.com";
    private string _telefone = "11987654321";
    private string _endereco = "Rua das Flores, 123";
    private string _cidade = "São Paulo";
    private string _estado = "SP";
    private string _cep = "01234567"; // CEP sem formatação

    public ClienteUpdateDtoBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ClienteUpdateDtoBuilder WithNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public ClienteUpdateDtoBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ClienteUpdateDtoBuilder WithTelefone(string telefone)
    {
        _telefone = telefone;
        return this;
    }

    public ClienteUpdateDtoBuilder WithEndereco(string endereco)
    {
        _endereco = endereco;
        return this;
    }

    public ClienteUpdateDtoBuilder WithCidade(string cidade)
    {
        _cidade = cidade;
        return this;
    }

    public ClienteUpdateDtoBuilder WithEstado(string estado)
    {
        _estado = estado;
        return this;
    }

    public ClienteUpdateDtoBuilder WithCep(string cep)
    {
        _cep = cep;
        return this;
    }

    public ClienteUpdateDto Build()
    {
        return new ClienteUpdateDto
        {
            Id = _id,
            Nome = _nome,
            Email = _email,
            Telefone = _telefone,
            Endereco = _endereco,
            Cidade = _cidade,
            Estado = _estado,
            Cep = _cep
        };
    }
}
