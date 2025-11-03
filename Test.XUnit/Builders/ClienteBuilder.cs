namespace Test.XUnit.Builders;

/// <summary>
/// Builder pattern para criar instâncias de Cliente para testes
/// </summary>
public class ClienteBuilder
{
    private string _nome = "João Silva";
    private string _email = "joao@example.com";
    private string _telefone = "11987654321";
    private string _cpf = "123.456.789-10";
    private string _endereco = "Rua das Flores, 123";
    private string _cidade = "São Paulo";
    private string _estado = "SP";
    private string _cep = "01234-567";

    public ClienteBuilder WithNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public ClienteBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ClienteBuilder WithTelefone(string telefone)
    {
        _telefone = telefone;
        return this;
    }

    public ClienteBuilder WithCpf(string cpf)
    {
        _cpf = cpf;
        return this;
    }

    public ClienteBuilder WithEndereco(string endereco)
    {
        _endereco = endereco;
        return this;
    }

    public ClienteBuilder WithCidade(string cidade)
    {
        _cidade = cidade;
        return this;
    }

    public ClienteBuilder WithEstado(string estado)
    {
        _estado = estado;
        return this;
    }

    public ClienteBuilder WithCep(string cep)
    {
        _cep = cep;
        return this;
    }

    public Cliente Build()
    {
        return Cliente.Criar(_nome, _email, _telefone, _cpf, _endereco, _cidade, _estado, _cep);
    }
}
