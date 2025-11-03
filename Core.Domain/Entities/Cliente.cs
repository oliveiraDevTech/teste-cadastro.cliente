using Core.Domain.Common;

namespace Core.Domain.Entities;

/// <summary>
/// Entidade de domínio para Cliente
/// Representa um cliente no sistema
/// </summary>
public class Cliente : BaseEntity
{
    // Constantes de validação
    private const int NOME_MIN_LENGTH = 3;
    private const int NOME_MAX_LENGTH = 150;
    private const int ENDERECO_MIN_LENGTH = 5;
    private const int CIDADE_MIN_LENGTH = 2;
    private const int ESTADO_LENGTH = 2;
    private const int CPF_LENGTH = 11;
    private const int TELEFONE_MIN_LENGTH = 10;
    private const int TELEFONE_MAX_LENGTH = 11;
    private const int SCORE_MIN = 0;
    private const int SCORE_MAX = 1000;
    private const int RANKING_MIN = 0;
    private const int RANKING_MAX = 5;
    private const int RANKING_MINIMO_CARTAO = 3;
    private const int SCORE_MINIMO_CARTAO = 600;
    /// <summary>
    /// Nome completo do cliente
    /// </summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>
    /// Email do cliente
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Telefone do cliente
    /// </summary>
    public string Telefone { get; private set; } = string.Empty;

    /// <summary>
    /// CPF do cliente
    /// </summary>
    public string Cpf { get; private set; } = string.Empty;

    /// <summary>
    /// Endereço do cliente
    /// </summary>
    public string Endereco { get; private set; } = string.Empty;

    /// <summary>
    /// Cidade do cliente
    /// </summary>
    public string Cidade { get; private set; } = string.Empty;

    /// <summary>
    /// Estado/UF do cliente
    /// </summary>
    public string Estado { get; private set; } = string.Empty;

    /// <summary>
    /// CEP do cliente
    /// </summary>
    public string Cep { get; private set; } = string.Empty;

    /// <summary>
    /// Ranking de crédito do cliente (1-5)
    /// Determina se o cliente pode emitir cartões de crédito
    /// </summary>
    public int RankingCredito { get; private set; } = 0;

    /// <summary>
    /// Score de crédito do cliente (0-1000)
    /// </summary>
    public int ScoreCredito { get; private set; } = 0;

    /// <summary>
    /// Data da última atualização de ranking
    /// </summary>
    public DateTime? DataAtualizacaoRanking { get; private set; }

    /// <summary>
    /// Indica se o cliente está apto a receber cartão de crédito
    /// </summary>
    public bool AptoParaCartaoCredito { get; private set; } = false;

    /// <summary>
    /// Construtor privado para criar cliente vazio
    /// </summary>
    private Cliente() { }

    /// <summary>
    /// Cria uma nova instância de Cliente com os dados fornecidos
    /// Aplica validações de domínio antes de criar
    /// </summary>
    /// <exception cref="ArgumentException">Lançado quando os dados são inválidos</exception>
    public static Cliente Criar(string nome, string email, string telefone, string cpf, string endereco, string cidade, string estado, string cep)
    {
        // Validações rápidas (fast-fail)
        ValidarDados(nome, email, telefone, cpf, endereco, cidade, estado, cep);

        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Email = email,
            Telefone = telefone,
            Cpf = cpf,
            Endereco = endereco,
            Cidade = cidade,
            Estado = estado,
            Cep = cep,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };
    }

    /// <summary>
    /// Valida os dados do cliente antes da criação
    /// Realiza validações rápidas (fast-fail) para falhar no primeiro erro
    /// </summary>
    private static void ValidarDados(string nome, string email, string telefone, string cpf, string endereco, string cidade, string estado, string cep)
    {
        var erros = new List<string>();

        // Validações de Nome
        if (string.IsNullOrWhiteSpace(nome))
            erros.Add("Nome é obrigatório");
        else if (nome.Length < NOME_MIN_LENGTH)
            erros.Add($"Nome deve ter no mínimo {NOME_MIN_LENGTH} caracteres");
        else if (nome.Length > NOME_MAX_LENGTH)
            erros.Add($"Nome não pode ter mais de {NOME_MAX_LENGTH} caracteres");

        // Validações de Email
        if (string.IsNullOrWhiteSpace(email))
            erros.Add("Email é obrigatório");
        else if (!ValidarEmailFormatoBrasil(email))
            erros.Add("Email inválido");

        // Validações de Telefone
        if (string.IsNullOrWhiteSpace(telefone))
            erros.Add("Telefone é obrigatório");
        else if (!ValidarTelefoneBrasil(telefone))
            erros.Add($"Telefone deve ter entre {TELEFONE_MIN_LENGTH} e {TELEFONE_MAX_LENGTH} dígitos");

        // Validações de CPF
        if (string.IsNullOrWhiteSpace(cpf))
            erros.Add("CPF é obrigatório");
        else if (!ValidarCpfComDigitos(cpf))
            erros.Add("CPF inválido");

        // Validações de Endereço
        if (string.IsNullOrWhiteSpace(endereco))
            erros.Add("Endereço é obrigatório");
        else if (endereco.Length < ENDERECO_MIN_LENGTH)
            erros.Add($"Endereço deve ter no mínimo {ENDERECO_MIN_LENGTH} caracteres");
        else if (endereco.Length > 200)
            erros.Add("Endereço não pode ter mais de 200 caracteres");

        // Validações de Cidade
        if (string.IsNullOrWhiteSpace(cidade))
            erros.Add("Cidade é obrigatória");
        else if (cidade.Length < CIDADE_MIN_LENGTH)
            erros.Add($"Cidade deve ter no mínimo {CIDADE_MIN_LENGTH} caracteres");
        else if (cidade.Length > 100)
            erros.Add("Cidade não pode ter mais de 100 caracteres");

        // Validações de Estado
        if (string.IsNullOrWhiteSpace(estado))
            erros.Add("Estado é obrigatório");
        else if (estado.Length != ESTADO_LENGTH)
            erros.Add($"Estado deve ter {ESTADO_LENGTH} caracteres");

        // Validações de CEP
        if (string.IsNullOrWhiteSpace(cep))
            erros.Add("CEP é obrigatório");
        else if (!ValidarCepBrasil(cep))
            erros.Add("CEP deve ter 8 dígitos");

        // Fast-fail: lança exceção na primeira validação
        if (erros.Any())
        {
            throw new ArgumentException($"Erro ao criar cliente: {string.Join("; ", erros)}");
        }
    }

    /// <summary>
    /// Atualiza os dados do cliente com validações
    /// </summary>
    /// <exception cref="ArgumentException">Lançado quando os dados são inválidos</exception>
    public void Atualizar(string nome, string email, string telefone, string endereco, string cidade, string estado, string cep, string? atualizadoPor = null)
    {
        // Valida os novos dados antes de atualizar
        ValidarDados(nome, email, telefone, Cpf, endereco, cidade, estado, cep);

        Nome = nome;
        Email = email;
        Telefone = telefone;
        Endereco = endereco;
        Cidade = cidade;
        Estado = estado;
        Cep = cep;
        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Valida o formato de um email
    /// </summary>
    private static bool ValidarEmailFormatoBrasil(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 150)
            return false;

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
    /// Valida o CPF com algoritmo de dígitos verificadores
    /// </summary>
    private static bool ValidarCpfComDigitos(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = System.Text.RegularExpressions.Regex.Replace(cpf, @"\D", "");

        // Deve ter exatamente 11 dígitos
        if (cpf.Length != 11)
            return false;

        // Não pode ser sequência repetida
        if (cpf == new string(cpf[0], 11))
            return false;

        // Valida primeiro dígito verificador
        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += int.Parse(cpf[i].ToString()) * (10 - i);

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != digito1)
            return false;

        // Valida segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(cpf[i].ToString()) * (11 - i);

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpf[10].ToString()) == digito2;
    }

    /// <summary>
    /// Valida o formato de um telefone brasileiro
    /// </summary>
    private static bool ValidarTelefoneBrasil(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return false;

        var limpo = System.Text.RegularExpressions.Regex.Replace(telefone, @"\D", "");
        return limpo.Length >= 10 && limpo.Length <= 11;
    }

    /// <summary>
    /// Valida o formato de um CEP brasileiro
    /// </summary>
    private static bool ValidarCepBrasil(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return false;

        var limpo = System.Text.RegularExpressions.Regex.Replace(cep, @"\D", "");
        return limpo.Length == 8;
    }

    /// <summary>
    /// Atualiza o ranking de crédito do cliente
    /// </summary>
    /// <param name="rankingCredito">Ranking de 1-5</param>
    /// <param name="scoreCredito">Score de 0-1000</param>
    /// <param name="atualizadoPor">Usuário que atualizou</param>
    public void AtualizarRankingCredito(int rankingCredito, int scoreCredito, string? atualizadoPor = null)
    {
        // Validações
        if (rankingCredito < 0 || rankingCredito > 5)
            throw new ArgumentException("Ranking de crédito deve estar entre 0 e 5");

        if (scoreCredito < 0 || scoreCredito > 1000)
            throw new ArgumentException("Score de crédito deve estar entre 0 e 1000");

        RankingCredito = rankingCredito;
        ScoreCredito = scoreCredito;
        DataAtualizacaoRanking = DateTime.UtcNow;

        // Determina aptidão para cartão de crédito
        // Ranking >= 3 E Score >= 600 = Apto
        AptoParaCartaoCredito = rankingCredito >= 3 && scoreCredito >= 600;

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Verifica se o cliente está apto para emitir cartão de crédito
    /// </summary>
    public bool PodeEmitirCartaoCredito()
    {
        return AptoParaCartaoCredito && Ativo && RankingCredito >= 3 && ScoreCredito >= 600;
    }

    /// <summary>
    /// Obtém descrição textual do ranking de crédito
    /// </summary>
    public string ObterDescricaoRanking()
    {
        return RankingCredito switch
        {
            0 => "Sem avaliação",
            1 => "Muito Ruim - Score baixo",
            2 => "Ruim - Risco alto",
            3 => "Aceitável - Pode emitir cartão",
            4 => "Bom - Crédito aprovado",
            5 => "Excelente - Crédito premium",
            _ => "Desconhecido"
        };
    }
}
