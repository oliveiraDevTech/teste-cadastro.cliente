namespace Core.Infra.Email;

/// <summary>
/// Interface para serviço de envio de emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia um email simples
    /// </summary>
    /// <param name="to">Email do destinatário</param>
    /// <param name="subject">Assunto do email</param>
    /// <param name="body">Corpo do email</param>
    /// <returns>True se enviado com sucesso, false caso contrário</returns>
    Task<bool> SendAsync(string to, string subject, string body);

    /// <summary>
    /// Envia uma mensagem de email completa
    /// </summary>
    /// <param name="message">Mensagem de email</param>
    /// <returns>True se enviado com sucesso, false caso contrário</returns>
    Task<bool> SendAsync(EmailMessage message);

    /// <summary>
    /// Envia múltiplos emails
    /// </summary>
    /// <param name="messages">Lista de mensagens de email</param>
    /// <returns>Número de emails enviados com sucesso</returns>
    Task<int> SendBatchAsync(IEnumerable<EmailMessage> messages);

    /// <summary>
    /// Valida um endereço de email
    /// </summary>
    /// <param name="email">Endereço de email</param>
    /// <returns>True se válido, false caso contrário</returns>
    bool IsValidEmail(string email);
}
