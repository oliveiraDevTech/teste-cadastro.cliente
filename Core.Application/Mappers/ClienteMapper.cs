namespace Core.Application.Mappers;

/// <summary>
/// Mapeador para conversão entre entidades de Cliente e seus DTOs
/// </summary>
public class ClienteMapper
{
    /// <summary>
    /// Converte um Cliente (entidade de domínio) para ClienteResponseDto
    /// </summary>
    /// <param name="cliente">Cliente a converter</param>
    /// <returns>ClienteResponseDto com os dados do cliente</returns>
    public static ClienteResponseDto ToResponseDto(Cliente cliente)
    {
        return new ClienteResponseDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            Cpf = cliente.Cpf,
            Endereco = cliente.Endereco,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado,
            Cep = cliente.Cep,
            DataCriacao = cliente.DataCriacao,
            DataAtualizacao = cliente.DataAtualizacao,
            Ativo = cliente.Ativo,
            RankingCredito = cliente.RankingCredito,
            ScoreCredito = cliente.ScoreCredito,
            DataAtualizacaoRanking = cliente.DataAtualizacaoRanking,
            AptoParaCartaoCredito = cliente.AptoParaCartaoCredito
        };
    }

    /// <summary>
    /// Converte um Cliente para ClienteListDto
    /// </summary>
    /// <param name="cliente">Cliente a converter</param>
    /// <returns>ClienteListDto com os dados resumidos do cliente</returns>
    public static ClienteListDto ToListDto(Cliente cliente)
    {
        return new ClienteListDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            Cidade = cliente.Cidade,
            Ativo = cliente.Ativo,
            DataCriacao = cliente.DataCriacao
        };
    }

    /// <summary>
    /// Converte um ClienteCreateDto para entidade Cliente
    /// </summary>
    /// <param name="createDto">Dados de criação do cliente</param>
    /// <returns>Entidade Cliente com os dados fornecidos</returns>
    public static Cliente ToEntity(ClienteCreateDto createDto)
    {
        return Cliente.Criar(
            createDto.Nome,
            createDto.Email,
            createDto.Telefone,
            createDto.Cpf,
            createDto.Endereco,
            createDto.Cidade,
            createDto.Estado,
            createDto.Cep
        );
    }

    /// <summary>
    /// Aplica os dados de atualização a uma entidade Cliente existente
    /// </summary>
    /// <param name="updateDto">Dados de atualização</param>
    /// <param name="clienteExistente">Cliente a atualizar</param>
    public static void ApplyUpdate(ClienteUpdateDto updateDto, Cliente clienteExistente)
    {
        clienteExistente.Atualizar(
            updateDto.Nome,
            updateDto.Email,
            updateDto.Telefone,
            updateDto.Endereco,
            updateDto.Cidade,
            updateDto.Estado,
            updateDto.Cep
        );

        // Atualiza ranking e score se fornecidos
        if (updateDto.RankingCredito.HasValue && updateDto.ScoreCredito.HasValue)
        {
            clienteExistente.AtualizarRankingCredito(
                updateDto.RankingCredito.Value,
                updateDto.ScoreCredito.Value,
                "SISTEMA");
        }
    }

    /// <summary>
    /// Converte uma lista de clientes para lista de DTOs
    /// </summary>
    /// <param name="clientes">Lista de clientes</param>
    /// <returns>Lista de ClienteResponseDto</returns>
    public static List<ClienteResponseDto> ToResponseDtoList(IEnumerable<Cliente> clientes)
    {
        return clientes.Select(ToResponseDto).ToList();
    }

    /// <summary>
    /// Converte uma lista de clientes para lista de DTOs de listagem
    /// </summary>
    /// <param name="clientes">Lista de clientes</param>
    /// <returns>Lista de ClienteListDto</returns>
    public static List<ClienteListDto> ToListDtoList(IEnumerable<Cliente> clientes)
    {
        return clientes.Select(ToListDto).ToList();
    }
}
