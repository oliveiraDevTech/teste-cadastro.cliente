namespace Test.XUnit.Builders;

/// <summary>
/// Builder pattern para criar instâncias de ClienteCreateDto para testes
/// </summary>
public class ClienteCreateDtoBuilder
{
    private string _nome = "João Silva";
    private string _email = "joao@example.com";
    private string _telefone = "11987654321";
    private string _cpf = "123.456.789-10";
    private string _endereco = "Rua das Flores, 123";
    private string _cidade = "São Paulo";
    private string _estado = "SP";
    private string _cep = "01234-567";

    public ClienteCreateDtoBuilder WithNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public ClienteCreateDtoBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ClienteCreateDtoBuilder WithTelefone(string telefone)
    {
        _telefone = telefone;
        return this;
    }

    public ClienteCreateDtoBuilder WithCpf(string cpf)
    {
        _cpf = cpf;
        return this;
    }

    public ClienteCreateDtoBuilder WithEndereco(string endereco)
    {
        _endereco = endereco;
        return this;
    }

    public ClienteCreateDtoBuilder WithCidade(string cidade)
    {
        _cidade = cidade;
        return this;
    }

    public ClienteCreateDtoBuilder WithEstado(string estado)
    {
        _estado = estado;
        return this;
    }

    public ClienteCreateDtoBuilder WithCep(string cep)
    {
        _cep = cep;
        return this;
    }

    public ClienteCreateDto Build()
    {
        return new ClienteCreateDto
        {
            Nome = _nome,
            Email = _email,
            Telefone = _telefone,
            Cpf = _cpf,
            Endereco = _endereco,
            Cidade = _cidade,
            Estado = _estado,
            Cep = _cep
        };
    }
}
