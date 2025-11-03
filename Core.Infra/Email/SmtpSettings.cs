namespace Core.Infra.Email;

/// <summary>
/// Configurações do servidor SMTP
/// </summary>
public class SmtpSettings
{
    /// <summary>
    /// Host do servidor SMTP
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Porta do servidor SMTP
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// Nome de usuário para autenticação
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Senha para autenticação
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de email padrão do remetente
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Nome do remetente padrão
    /// </summary>
    public string FromName { get; set; } = "Sistema de Cadastro";

    /// <summary>
    /// Usar SSL/TLS
    /// </summary>
    public bool UseSsl { get; set; } = true;

    /// <summary>
    /// Usar StartTls
    /// </summary>
    public bool UseStartTls { get; set; } = false;

    /// <summary>
    /// Timeout em milissegundos
    /// </summary>
    public int Timeout { get; set; } = 10000;

    /// <summary>
    /// Habilitar para usar SMTP real, desabilitar para usar um mock
    /// </summary>
    public bool Enabled { get; set; } = false;
}
