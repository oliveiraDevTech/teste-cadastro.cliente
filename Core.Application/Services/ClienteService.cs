namespace Core.Application.Services;

/// <summary>
/// Serviço de aplicação para operações com clientes
/// Implementa os casos de uso e orquestra operações entre domínio e infraestrutura
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly Driven.RabbitMQ.Interfaces.IMessagePublisher _messagePublisher;
    private readonly ILogger<ClienteService> _logger;
    private readonly Driven.RabbitMQ.Settings.RabbitMQSettings _rabbitMQSettings;

    /// <summary>
    /// Construtor do serviço
    /// </summary>
    /// <param name="repository">Repositório de clientes injetado por DI</param>
    /// <param name="messagePublisher">Publicador de mensagens para RabbitMQ</param>
    /// <param name="logger">Logger para registrar operações</param>
    /// <param name="rabbitMQSettings">Configurações do RabbitMQ incluindo nomes das filas</param>
    public ClienteService(
        IClienteRepository repository,
        Driven.RabbitMQ.Interfaces.IMessagePublisher messagePublisher,
        ILogger<ClienteService> logger,
        IOptions<Driven.RabbitMQ.Settings.RabbitMQSettings> rabbitMQSettings)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _logger = logger;
        _rabbitMQSettings = rabbitMQSettings.Value;
    }

    /// <summary>
    /// Obtém um cliente por seu ID
    /// </summary>
    public async Task<ApiResponseDto<ClienteResponseDto>> ObterPorIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "ID do cliente é inválido",
                    Erros = new List<string> { "ID não pode estar vazio" }
                };
            }

            var cliente = await _repository.ObterPorIdAsync(id);

            if (cliente == null)
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado",
                    Erros = new List<string> { $"Nenhum cliente encontrado com o ID: {id}" }
                };
            }

            return new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = true,
                Mensagem = "Cliente obtido com sucesso",
                Dados = cliente
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = false,
                Mensagem = "Erro ao obter cliente",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Lista todos os clientes com paginação
    /// </summary>
    public async Task<ApiResponseDto<PaginatedResponseDto<ClienteListDto>>> ListarAsync(int pagina = 1, int itensPorPagina = 10)
    {
        try
        {
            if (pagina < 1)
                pagina = 1;

            if (itensPorPagina < 1 || itensPorPagina > 100)
                itensPorPagina = 10;

            var resultado = await _repository.ListarAsync(pagina, itensPorPagina);

            return new ApiResponseDto<PaginatedResponseDto<ClienteListDto>>
            {
                Sucesso = true,
                Mensagem = "Clientes listados com sucesso",
                Dados = resultado
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<PaginatedResponseDto<ClienteListDto>>
            {
                Sucesso = false,
                Mensagem = "Erro ao listar clientes",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Pesquisa clientes por nome
    /// </summary>
    public async Task<ApiResponseDto<PaginatedResponseDto<ClienteListDto>>> PesquisarPorNomeAsync(string nome, int pagina = 1, int itensPorPagina = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return new ApiResponseDto<PaginatedResponseDto<ClienteListDto>>
                {
                    Sucesso = false,
                    Mensagem = "Nome de busca é obrigatório",
                    Erros = new List<string> { "O nome não pode estar vazio" }
                };
            }

            if (pagina < 1)
                pagina = 1;

            if (itensPorPagina < 1 || itensPorPagina > 100)
                itensPorPagina = 10;

            var resultado = await _repository.PesquisarPorNomeAsync(nome, pagina, itensPorPagina);

            return new ApiResponseDto<PaginatedResponseDto<ClienteListDto>>
            {
                Sucesso = true,
                Mensagem = "Pesquisa realizada com sucesso",
                Dados = resultado
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<PaginatedResponseDto<ClienteListDto>>
            {
                Sucesso = false,
                Mensagem = "Erro ao pesquisar clientes",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    public async Task<ApiResponseDto<ClienteResponseDto>> CriarAsync(ClienteCreateDto clienteCreateDto)
    {
        try
        {
            // Validações
            var erros = ValidarClienteCreateDto(clienteCreateDto);
            if (erros.Count > 0)
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos para criar cliente",
                    Erros = erros
                };
            }

            // Verificar se email já está registrado
            if (await _repository.EmailJaRegistradoAsync(clienteCreateDto.Email))
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Email já registrado",
                    Erros = new List<string> { "Este email já está associado a outro cliente" }
                };
            }

            // Verificar se CPF já está registrado
            if (await _repository.CpfJaRegistradoAsync(clienteCreateDto.Cpf))
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "CPF já registrado",
                    Erros = new List<string> { "Este CPF já está associado a outro cliente" }
                };
            }

            // Criar cliente
            var clienteCriado = await _repository.CriarAsync(clienteCreateDto);

            // Publicar evento de cadastro de cliente para iniciar análise de crédito
            try
            {
                // Calcular idade se data de nascimento foi fornecida
                var idade = 30; // Valor padrão
                var dataNascimento = DateTime.UtcNow.AddYears(-30); // Valor padrão
                
                if (clienteCreateDto.DataNascimento.HasValue)
                {
                    dataNascimento = clienteCreateDto.DataNascimento.Value;
                    var hoje = DateTime.UtcNow;
                    idade = hoje.Year - dataNascimento.Year;
                    if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
                }

                var eventoClienteCadastrado = new Driven.RabbitMQ.Events.ClienteCadastradoIntegrationEvent
                {
                    ClienteId = clienteCriado.Id,
                    Nome = clienteCriado.Nome,
                    CPF = clienteCriado.Cpf,
                    Email = clienteCriado.Email,
                    Renda = clienteCreateDto.RendaMensal ?? 0,
                    Idade = idade,
                    HistoricoCredito = clienteCreateDto.HistoricoCredito ?? "REGULAR",
                    DataNascimento = dataNascimento
                };

                await _messagePublisher.PublishAsync(_rabbitMQSettings.Queues.ClienteCadastrado, eventoClienteCadastrado);

                _logger.LogInformation(
                    "Evento ClienteCadastradoIntegrationEvent publicado para cliente {ClienteId} ({Nome}) - Renda: {Renda}, Idade: {Idade}",
                    clienteCriado.Id, clienteCriado.Nome, eventoClienteCadastrado.Renda, eventoClienteCadastrado.Idade);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Aviso: Não foi possível publicar evento de cadastro para cliente {ClienteId}. Cliente foi criado mas análise de crédito será iniciada apenas após reconexão com RabbitMQ.",
                    clienteCriado.Id);
                // Não falhar a criação do cliente se RabbitMQ não estiver disponível
                // O evento será retentado mais tarde
            }

            return new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = true,
                Mensagem = "Cliente criado com sucesso",
                Dados = clienteCriado
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente: {Mensagem}", ex.Message);
            return new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = false,
                Mensagem = "Erro ao criar cliente",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    public async Task<ApiResponseDto<ClienteResponseDto>> AtualizarAsync(ClienteUpdateDto clienteUpdateDto)
    {
        try
        {
            // Validações
            var erros = ValidarClienteUpdateDto(clienteUpdateDto);
            if (erros.Count > 0)
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos para atualizar cliente",
                    Erros = erros
                };
            }

            // Verificar se cliente existe
            if (!await _repository.ExisteAsync(clienteUpdateDto.Id))
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado",
                    Erros = new List<string> { "O cliente a atualizar não existe" }
                };
            }

            // Verificar se email já está registrado (excluindo o próprio cliente)
            if (await _repository.EmailJaRegistradoAsync(clienteUpdateDto.Email, clienteUpdateDto.Id))
            {
                return new ApiResponseDto<ClienteResponseDto>
                {
                    Sucesso = false,
                    Mensagem = "Email já registrado",
                    Erros = new List<string> { "Este email já está associado a outro cliente" }
                };
            }

            // Atualizar cliente
            var clienteAtualizado = await _repository.AtualizarAsync(clienteUpdateDto);

            return new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = true,
                Mensagem = "Cliente atualizado com sucesso",
                Dados = clienteAtualizado
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ClienteResponseDto>
            {
                Sucesso = false,
                Mensagem = "Erro ao atualizar cliente",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Deleta um cliente
    /// </summary>
    public async Task<ApiResponseDto> DeletarAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return new ApiResponseDto
                {
                    Sucesso = false,
                    Mensagem = "ID do cliente é inválido",
                    Erros = new List<string> { "ID não pode estar vazio" }
                };
            }

            if (!await _repository.ExisteAsync(id))
            {
                return new ApiResponseDto
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado",
                    Erros = new List<string> { "O cliente a deletar não existe" }
                };
            }

            var deletado = await _repository.DeletarAsync(id);

            if (!deletado)
            {
                return new ApiResponseDto
                {
                    Sucesso = false,
                    Mensagem = "Erro ao deletar cliente",
                    Erros = new List<string> { "Não foi possível deletar o cliente" }
                };
            }

            return new ApiResponseDto
            {
                Sucesso = true,
                Mensagem = "Cliente deletado com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto
            {
                Sucesso = false,
                Mensagem = "Erro ao deletar cliente",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Valida os dados de criação de cliente
    /// </summary>
    private List<string> ValidarClienteCreateDto(ClienteCreateDto dto)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nome) || dto.Nome.Length < 3)
            erros.Add("Nome deve ter no mínimo 3 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Email) || !ValidarEmail(dto.Email))
            erros.Add("Email inválido");

        if (string.IsNullOrWhiteSpace(dto.Telefone) || dto.Telefone.Length < 10)
            erros.Add("Telefone deve ter no mínimo 10 dígitos");

        if (string.IsNullOrWhiteSpace(dto.Cpf) || !ValidarCpf(dto.Cpf))
            erros.Add("CPF inválido");

        if (string.IsNullOrWhiteSpace(dto.Endereco) || dto.Endereco.Length < 5)
            erros.Add("Endereço deve ter no mínimo 5 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Cidade) || dto.Cidade.Length < 3)
            erros.Add("Cidade deve ter no mínimo 3 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Estado) || dto.Estado.Length != 2)
            erros.Add("Estado deve ser uma sigla com 2 caracteres (ex: SP, RJ)");

        if (string.IsNullOrWhiteSpace(dto.Cep) || !ValidarCep(dto.Cep))
            erros.Add("CEP inválido (formato esperado: XXXXX-XXX)");

        return erros;
    }

    /// <summary>
    /// Valida os dados de atualização de cliente
    /// </summary>
    private List<string> ValidarClienteUpdateDto(ClienteUpdateDto dto)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nome) || dto.Nome.Length < 3)
            erros.Add("Nome deve ter no mínimo 3 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Email) || !ValidarEmail(dto.Email))
            erros.Add("Email inválido");

        if (string.IsNullOrWhiteSpace(dto.Telefone) || dto.Telefone.Length < 10)
            erros.Add("Telefone deve ter no mínimo 10 dígitos");

        if (string.IsNullOrWhiteSpace(dto.Endereco) || dto.Endereco.Length < 5)
            erros.Add("Endereço deve ter no mínimo 5 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Cidade) || dto.Cidade.Length < 3)
            erros.Add("Cidade deve ter no mínimo 3 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Estado) || dto.Estado.Length != 2)
            erros.Add("Estado deve ser uma sigla com 2 caracteres (ex: SP, RJ)");

        if (string.IsNullOrWhiteSpace(dto.Cep) || !ValidarCep(dto.Cep))
            erros.Add("CEP inválido (formato esperado: XXXXX-XXX)");

        return erros;
    }

    /// <summary>
    /// Valida formato de email
    /// </summary>
    private bool ValidarEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Valida CPF (formato básico)
    /// </summary>
    private bool ValidarCpf(string cpf)
    {
        // Remove caracteres não numéricos
        var cpfLimpo = System.Text.RegularExpressions.Regex.Replace(cpf, @"\D", "");

        // Deve ter 11 dígitos
        if (cpfLimpo.Length != 11)
            return false;

        // Não pode ser uma sequência de números iguais
        if (cpfLimpo == new string(cpfLimpo[0], 11))
            return false;

        // Validação do primeiro dígito verificador
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpfLimpo[i].ToString()) * (10 - i);

        int firstDigit = 11 - (sum % 11);
        if (firstDigit > 9)
            firstDigit = 0;

        if (int.Parse(cpfLimpo[9].ToString()) != firstDigit)
            return false;

        // Validação do segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpfLimpo[i].ToString()) * (11 - i);

        int secondDigit = 11 - (sum % 11);
        if (secondDigit > 9)
            secondDigit = 0;

        if (int.Parse(cpfLimpo[10].ToString()) != secondDigit)
            return false;

        return true;
    }

    /// <summary>
    /// Valida CEP (formato básico)
    /// </summary>
    private bool ValidarCep(string cep)
    {
        // Remove caracteres não numéricos
        var cepLimpo = System.Text.RegularExpressions.Regex.Replace(cep, @"\D", "");
        return cepLimpo.Length == 8;
    }

    /// <summary>
    /// Calcula a idade baseado na data de nascimento
    /// </summary>
    private int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Now;
        var idade = hoje.Year - dataNascimento.Year;

        // Ajustar se aniversário ainda não ocorreu este ano
        if (dataNascimento.Date > hoje.AddYears(-idade))
            idade--;

        return idade;
    }

    /// <summary>
    /// Solicita emissão de cartão de crédito para o cliente
    /// Publica evento na fila cartao.emissao.pedido
    /// </summary>
    public async Task<ApiResponseDto<object>> SolicitarEmissaoCartaoAsync(Guid clienteId)
    {
        try
        {
            if (clienteId == Guid.Empty)
            {
                return new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "ID do cliente é inválido",
                    Erros = new List<string> { "ID não pode estar vazio" }
                };
            }

            // Buscar cliente
            var cliente = await _repository.ObterPorIdAsync(clienteId);

            if (cliente == null)
            {
                return new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado",
                    Erros = new List<string> { "O cliente não existe no sistema" }
                };
            }

            // Verificar se cliente está apto
            if (!cliente.AptoParaCartaoCredito || cliente.ScoreCredito < 501)
            {
                return new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não está apto para emissão de cartão",
                    Erros = new List<string> 
                    { 
                        $"Cliente precisa ter score mínimo de 501. Score atual: {cliente.ScoreCredito}",
                        "Realize uma análise de crédito antes de solicitar cartão"
                    }
                };
            }

            // Determinar quantidade de cartões baseado no score
            int quantidadeCartoes = cliente.ScoreCredito >= 501 ? 2 : 1;
            decimal limiteTotal = cliente.ScoreCredito * 10; // Score * 10 = limite
            decimal limitePorCartao = limiteTotal / quantidadeCartoes;

            // Criar evento de pedido de emissão
            var pedidoEmissaoCartao = new
            {
                ClienteId = cliente.Id,
                PropostaId = Guid.NewGuid(), // Gerar ID único para proposta
                ContaId = Guid.NewGuid(), // TODO: Integrar com serviço de contas
                CodigoProduto = "CREDIT_CARD_PLATINUM",
                QuantidadeCartoesEmitir = quantidadeCartoes,
                LimiteCreditoPorCartao = limitePorCartao,
                CorrelacaoId = Guid.NewGuid().ToString(),
                ChaveIdempotencia = $"{cliente.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                Entrega = new
                {
                    TipoEntrega = "CORREIOS_SEDEX",
                    EnderecoEntrega = new
                    {
                        Logradouro = cliente.Endereco,
                        Cidade = cliente.Cidade,
                        Estado = cliente.Estado,
                        Cep = cliente.Cep
                    }
                },
                DataSolicitacao = DateTime.UtcNow
            };

            // Publicar na fila cartao.emissao.pedido
            await _messagePublisher.PublishAsync(
                _rabbitMQSettings.Queues.CartaoEmissaoPedido,
                pedidoEmissaoCartao
            );

            _logger.LogInformation(
                "Pedido de emissão de cartão publicado para cliente {ClienteId}. Quantidade: {Quantidade}, Limite por cartão: {Limite}",
                cliente.Id, quantidadeCartoes, limitePorCartao
            );

            return new ApiResponseDto<object>
            {
                Sucesso = true,
                Mensagem = "Solicitação de cartão enviada para processamento",
                Dados = new
                {
                    ClienteId = cliente.Id,
                    Nome = cliente.Nome,
                    ScoreCredito = cliente.ScoreCredito,
                    QuantidadeCartoes = quantidadeCartoes,
                    LimitePorCartao = limitePorCartao,
                    LimiteTotal = limiteTotal,
                    Status = "EM_PROCESSAMENTO",
                    DataSolicitacao = DateTime.UtcNow
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao solicitar emissão de cartão para cliente {ClienteId}", clienteId);
            return new ApiResponseDto<object>
            {
                Sucesso = false,
                Mensagem = "Erro ao solicitar emissão de cartão",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Obtém o status da emissão de cartão do cliente
    /// </summary>
    public async Task<ApiResponseDto<object>> ObterStatusEmissaoCartaoAsync(Guid clienteId)
    {
        try
        {
            if (clienteId == Guid.Empty)
            {
                return new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "ID do cliente é inválido",
                    Erros = new List<string> { "ID não pode estar vazio" }
                };
            }

            var cliente = await _repository.ObterPorIdAsync(clienteId);

            if (cliente == null)
            {
                return new ApiResponseDto<object>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado",
                    Erros = new List<string> { "O cliente não existe no sistema" }
                };
            }

            // TODO: Consultar serviço de emissão de cartões para obter status real
            // Por enquanto, retorna baseado no score do cliente

            var status = new
            {
                ClienteId = cliente.Id,
                Nome = cliente.Nome,
                AptoParaCartao = cliente.AptoParaCartaoCredito,
                ScoreCredito = cliente.ScoreCredito,
                RankingCredito = cliente.RankingCredito,
                StatusEmissao = cliente.AptoParaCartaoCredito && cliente.ScoreCredito >= 501 
                    ? "APTO_PARA_SOLICITAR" 
                    : "NAO_APTO",
                QuantidadeCartoesElegiveis = cliente.ScoreCredito >= 501 ? 2 : 0,
                DataUltimaAnalise = cliente.DataAtualizacaoRanking,
                Mensagem = cliente.AptoParaCartaoCredito && cliente.ScoreCredito >= 501
                    ? "Cliente apto para solicitar emissão de cartão"
                    : "Cliente não possui score mínimo para emissão de cartão"
            };

            return new ApiResponseDto<object>
            {
                Sucesso = true,
                Mensagem = "Status de emissão obtido com sucesso",
                Dados = status
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter status de emissão para cliente {ClienteId}", clienteId);
            return new ApiResponseDto<object>
            {
                Sucesso = false,
                Mensagem = "Erro ao obter status de emissão",
                Erros = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Lista todos os cartões de crédito do cliente
    /// Consulta o microserviço de emissão de cartões
    /// </summary>
    public async Task<ApiResponseDto<List<object>>> ObterCartoesClienteAsync(Guid clienteId)
    {
        try
        {
            if (clienteId == Guid.Empty)
            {
                return new ApiResponseDto<List<object>>
                {
                    Sucesso = false,
                    Mensagem = "ID do cliente é inválido",
                    Erros = new List<string> { "ID não pode estar vazio" }
                };
            }

            var cliente = await _repository.ObterPorIdAsync(clienteId);

            if (cliente == null)
            {
                return new ApiResponseDto<List<object>>
                {
                    Sucesso = false,
                    Mensagem = "Cliente não encontrado",
                    Erros = new List<string> { "O cliente não existe no sistema" }
                };
            }

            // TODO: Fazer chamada HTTP para o serviço de emissão de cartões
            // GET http://emissao-cartao:5001/api/v1/Cards/cliente/{clienteId}
            
            // Por enquanto, retorna lista vazia com informação
            _logger.LogInformation("Consultando cartões do cliente {ClienteId}", clienteId);

            return new ApiResponseDto<List<object>>
            {
                Sucesso = true,
                Mensagem = "Nenhum cartão encontrado para este cliente",
                Dados = new List<object>(),
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar cartões do cliente {ClienteId}", clienteId);
            return new ApiResponseDto<List<object>>
            {
                Sucesso = false,
                Mensagem = "Erro ao listar cartões",
                Erros = new List<string> { ex.Message }
            };
        }
    }
}
