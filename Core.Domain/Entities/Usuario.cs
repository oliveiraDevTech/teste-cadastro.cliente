using System.Security.Cryptography;
using System.Text;
using Core.Domain.Common;

namespace Core.Domain.Entities;

/// <summary>
/// Entidade de domínio para Usuário
/// Representa um usuário do sistema com autenticação e autorização
/// </summary>
public class Usuario : BaseEntity
{
    /// <summary>
    /// Login único do usuário
    /// </summary>
    public string Login { get; private set; } = string.Empty;

    /// <summary>
    /// Email único do usuário
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Hash da senha do usuário
    /// </summary>
    public string SenhaHash { get; private set; } = string.Empty;

    /// <summary>
    /// Salt usado na geração do hash
    /// </summary>
    public string SenhaSalt { get; private set; } = string.Empty;

    /// <summary>
    /// ID do cliente associado ao usuário (opcional)
    /// </summary>
    public Guid? ClienteId { get; private set; }

    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    public string NomeCompleto { get; private set; } = string.Empty;

    /// <summary>
    /// Telefone do usuário
    /// </summary>
    public string Telefone { get; private set; } = string.Empty;

    /// <summary>
    /// Data do último acesso
    /// </summary>
    public DateTime? DataUltimoAcesso { get; private set; }

    /// <summary>
    /// Data em que a senha foi alterada pela última vez
    /// </summary>
    public DateTime? DataAlteracaoSenha { get; private set; }

    /// <summary>
    /// Tipo de usuário (Admin, Cliente, Operador, etc)
    /// </summary>
    public string TipoUsuario { get; private set; } = "Cliente";

    /// <summary>
    /// Permissões do usuário (separadas por vírgula)
    /// </summary>
    public string Permissoes { get; private set; } = string.Empty;

    /// <summary>
    /// Indica se o usuário é administrador
    /// </summary>
    public bool IsAdmin { get; private set; } = false;

    /// <summary>
    /// Número de tentativas de login falhadas
    /// </summary>
    public int TentativasLoginFalhadas { get; private set; } = 0;

    /// <summary>
    /// Data/hora em que o usuário foi bloqueado (se aplicável)
    /// </summary>
    public DateTime? DataBloqueio { get; private set; }

    /// <summary>
    /// Motivo do bloqueio
    /// </summary>
    public string? MotivoBloqueio { get; private set; }

    /// <summary>
    /// Indicador se o email foi confirmado
    /// </summary>
    public bool EmailConfirmado { get; private set; } = false;

    /// <summary>
    /// Token de confirmação de email
    /// </summary>
    public string? TokenConfirmacaoEmail { get; private set; }

    /// <summary>
    /// Construtor privado
    /// </summary>
    private Usuario() { }

    /// <summary>
    /// Cria um novo usuário com validações de domínio
    /// </summary>
    public static Usuario Criar(
        string login,
        string email,
        string senhaPlana,
        string nomeCompleto,
        string telefone,
        Guid? clienteId = null,
        string tipoUsuario = "Cliente",
        bool isAdmin = false)
    {
        // Validações rápidas (fast-fail)
        ValidarDadosUsuario(login, email, senhaPlana, nomeCompleto, telefone);

        // Gera salt e hash da senha
        var (senhaSalt, senhaHash) = GerarHashSenha(senhaPlana);

        return new Usuario
        {
            Id = Guid.NewGuid(),
            Login = login.ToLower().Trim(),
            Email = email.ToLower().Trim(),
            SenhaHash = senhaHash,
            SenhaSalt = senhaSalt,
            NomeCompleto = nomeCompleto,
            Telefone = telefone,
            ClienteId = clienteId,
            TipoUsuario = tipoUsuario,
            IsAdmin = isAdmin,
            DataCriacao = DateTime.UtcNow,
            Ativo = true,
            EmailConfirmado = false,
            TokenConfirmacaoEmail = GerarTokenConfirmacao()
        };
    }

    /// <summary>
    /// Valida os dados do usuário antes da criação
    /// </summary>
    private static void ValidarDadosUsuario(string login, string email, string senhaPlana, string nomeCompleto, string telefone)
    {
        var erros = new List<string>();

        // Validações de Login
        if (string.IsNullOrWhiteSpace(login))
            erros.Add("Login é obrigatório");
        else if (login.Length < 3)
            erros.Add("Login deve ter no mínimo 3 caracteres");
        else if (login.Length > 50)
            erros.Add("Login não pode ter mais de 50 caracteres");

        // Validações de Email
        if (string.IsNullOrWhiteSpace(email))
            erros.Add("Email é obrigatório");
        else if (!ValidarEmailFormatoBrasil(email))
            erros.Add("Email inválido");

        // Validações de Senha
        if (string.IsNullOrWhiteSpace(senhaPlana))
            erros.Add("Senha é obrigatória");
        else if (senhaPlana.Length < 8)
            erros.Add("Senha deve ter no mínimo 8 caracteres");
        else if (senhaPlana.Length > 100)
            erros.Add("Senha não pode ter mais de 100 caracteres");
        else if (!ValidarForcaSenha(senhaPlana))
            erros.Add("Senha deve conter números, letras maiúsculas e minúsculas");

        // Validações de Nome Completo
        if (string.IsNullOrWhiteSpace(nomeCompleto))
            erros.Add("Nome completo é obrigatório");
        else if (nomeCompleto.Length < 3)
            erros.Add("Nome deve ter no mínimo 3 caracteres");
        else if (nomeCompleto.Length > 150)
            erros.Add("Nome não pode ter mais de 150 caracteres");

        // Validações de Telefone
        if (string.IsNullOrWhiteSpace(telefone))
            erros.Add("Telefone é obrigatório");
        else if (!ValidarTelefoneBrasil(telefone))
            erros.Add("Telefone deve ter entre 10 e 11 dígitos");

        if (erros.Any())
            throw new ArgumentException($"Erro ao criar usuário: {string.Join("; ", erros)}");
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
    /// Valida a força da senha
    /// </summary>
    private static bool ValidarForcaSenha(string senha)
    {
        // Deve conter pelo menos uma letra maiúscula, uma minúscula e um número
        return System.Text.RegularExpressions.Regex.IsMatch(senha, @"[A-Z]") &&
               System.Text.RegularExpressions.Regex.IsMatch(senha, @"[a-z]") &&
               System.Text.RegularExpressions.Regex.IsMatch(senha, @"[0-9]");
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
    /// Verifica se a senha fornecida está correta
    /// </summary>
    public bool VerificarSenha(string senhaPlana)
    {
        if (string.IsNullOrWhiteSpace(senhaPlana))
            return false;

        // Regenera o hash com o salt armazenado
        var hashGerado = GerarHashComSalt(senhaPlana, SenhaSalt);
        return SenhaHash == hashGerado;
    }

    /// <summary>
    /// Altera a senha do usuário
    /// </summary>
    public void AlterarSenha(string senhaAtual, string novaSenha, string? atualizadoPor = null)
    {
        // Valida senha atual
        if (!VerificarSenha(senhaAtual))
            throw new ArgumentException("Senha atual está incorreta");

        // Valida nova senha
        if (string.IsNullOrWhiteSpace(novaSenha))
            throw new ArgumentException("Nova senha é obrigatória");

        if (novaSenha.Length < 8)
            throw new ArgumentException("Nova senha deve ter no mínimo 8 caracteres");

        if (!ValidarForcaSenha(novaSenha))
            throw new ArgumentException("Nova senha deve conter números, letras maiúsculas e minúsculas");

        if (VerificarSenha(novaSenha))
            throw new ArgumentException("Nova senha não pode ser igual à senha atual");

        // Gera novo hash e salt
        var (novoSalt, novoHash) = GerarHashSenha(novaSenha);
        SenhaSalt = novoSalt;
        SenhaHash = novoHash;
        DataAlteracaoSenha = DateTime.UtcNow;

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Registra um acesso bem-sucedido do usuário
    /// </summary>
    public void RegistrarAcessoSucesso(string? atualizadoPor = null)
    {
        DataUltimoAcesso = DateTime.UtcNow;
        TentativasLoginFalhadas = 0;
        DataBloqueio = null;
        MotivoBloqueio = null;

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Registra uma tentativa de login falhada
    /// </summary>
    public void RegistrarTentativaFalhada(string? atualizadoPor = null)
    {
        TentativasLoginFalhadas++;

        // Bloqueia usuário após 5 tentativas
        if (TentativasLoginFalhadas >= 5)
        {
            DataBloqueio = DateTime.UtcNow;
            MotivoBloqueio = "Bloqueado por múltiplas tentativas de login falhadas";
        }

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Desbloqueia o usuário
    /// </summary>
    public void Desbloquear(string? atualizadoPor = null)
    {
        if (DataBloqueio == null)
            return;

        DataBloqueio = null;
        MotivoBloqueio = null;
        TentativasLoginFalhadas = 0;

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Confirma o email do usuário
    /// </summary>
    public void ConfirmarEmail(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token de confirmação é obrigatório");

        if (TokenConfirmacaoEmail != token)
            throw new ArgumentException("Token de confirmação inválido");

        EmailConfirmado = true;
        TokenConfirmacaoEmail = null;
        MarcarComoAtualizada();
    }

    /// <summary>
    /// Gera um novo token de confirmação de email
    /// </summary>
    public void GerarNovoTokenConfirmacao()
    {
        TokenConfirmacaoEmail = GerarTokenConfirmacao();
        MarcarComoAtualizada();
    }

    /// <summary>
    /// Adiciona uma permissão ao usuário
    /// </summary>
    public void AdicionarPermissao(string permissao, string? atualizadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(permissao))
            throw new ArgumentException("Permissão não pode ser vazia");

        if (!Permissoes.Contains(permissao))
        {
            Permissoes = string.IsNullOrEmpty(Permissoes)
                ? permissao
                : $"{Permissoes},{permissao}";
        }

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Remove uma permissão do usuário
    /// </summary>
    public void RemoverPermissao(string permissao, string? atualizadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(permissao))
            throw new ArgumentException("Permissão não pode ser vazia");

        var permissoes = Permissoes.Split(',').Where(p => p != permissao).ToList();
        Permissoes = string.Join(",", permissoes);

        MarcarComoAtualizada(atualizadoPor);
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica
    /// </summary>
    public bool TemPermissao(string permissao)
    {
        if (IsAdmin)
            return true;

        return Permissoes.Contains(permissao);
    }

    /// <summary>
    /// Verifica se o usuário está bloqueado
    /// </summary>
    public bool EstaBloqueado()
    {
        return DataBloqueio.HasValue && !Ativo;
    }

    /// <summary>
    /// Gera hash SHA256 da senha com salt
    /// </summary>
    private static (string salt, string hash) GerarHashSenha(string senhaPlana)
    {
        // Gera um salt aleatório
        byte[] saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        string saltString = Convert.ToBase64String(saltBytes);
        string hashString = GerarHashComSalt(senhaPlana, saltString);

        return (saltString, hashString);
    }

    /// <summary>
    /// Gera hash da senha com salt fornecido
    /// </summary>
    private static string GerarHashComSalt(string senhaPlana, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using (var pbkdf2 = new Rfc2898DeriveBytes(senhaPlana, saltBytes, 10000, System.Security.Cryptography.HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>
    /// Gera um token de confirmação aleatório
    /// </summary>
    private static string GerarTokenConfirmacao()
    {
        var tokenBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }

        return Convert.ToBase64String(tokenBytes).Replace("+", "").Replace("/", "").Replace("=", "").Substring(0, 32);
    }
}
