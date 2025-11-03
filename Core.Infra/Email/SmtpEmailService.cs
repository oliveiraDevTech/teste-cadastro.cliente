using Microsoft.Extensions.Options;
using Core.Infra.Logging;

namespace Core.Infra.Email;

/// <summary>
/// Implementação de serviço de email usando SMTP
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly IApplicationLogger _logger;

    public SmtpEmailService(IOptions<SmtpSettings> options, IApplicationLogger logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string to, string subject, string body)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = true
        };

        return await SendAsync(message);
    }

    public async Task<bool> SendAsync(EmailMessage message)
    {
        try
        {
            // Validações
            if (string.IsNullOrWhiteSpace(message.To) || !IsValidEmail(message.To))
            {
                _logger.LogWarning("Email inválido: {email}", message.To);
                return false;
            }

            if (string.IsNullOrWhiteSpace(message.Subject) || string.IsNullOrWhiteSpace(message.Body))
            {
                _logger.LogWarning("Email sem assunto ou corpo");
                return false;
            }

            // Se o SMTP não está habilitado, apenas registra
            if (!_settings.Enabled)
            {
                _logger.LogInformation(
                    "Email de {from} para {to} com assunto '{subject}' seria enviado (SMTP desabilitado)",
                    message.From ?? _settings.FromEmail, message.To, message.Subject);
                return true;
            }

            using var client = new System.Net.Mail.SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.UseSsl,
                Timeout = _settings.Timeout
            };

            if (!string.IsNullOrWhiteSpace(_settings.UserName))
            {
                client.Credentials = new System.Net.NetworkCredential(_settings.UserName, _settings.Password);
            }

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(message.From ?? _settings.FromEmail, _settings.FromName),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml
            };

            // Destinatários
            mailMessage.To.Add(message.To);

            // CC
            foreach (var cc in message.Cc)
            {
                if (IsValidEmail(cc))
                    mailMessage.CC.Add(cc);
            }

            // BCC
            foreach (var bcc in message.Bcc)
            {
                if (IsValidEmail(bcc))
                    mailMessage.Bcc.Add(bcc);
            }

            // Anexos
            foreach (var attachment in message.Attachments)
            {
                var stream = new System.IO.MemoryStream(attachment.FileContent);
                var mailAttachment = new System.Net.Mail.Attachment(stream, attachment.FileName, attachment.MimeType);
                mailMessage.Attachments.Add(mailAttachment);
            }

            // Prioridade
            mailMessage.Priority = message.Priority switch
            {
                EmailPriority.Low => System.Net.Mail.MailPriority.Low,
                EmailPriority.High => System.Net.Mail.MailPriority.High,
                _ => System.Net.Mail.MailPriority.Normal
            };

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation("Email enviado com sucesso para {email}", message.To);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao enviar email para {message.To}: {ex.Message}", ex);
            return false;
        }
    }

    public async Task<int> SendBatchAsync(IEnumerable<EmailMessage> messages)
    {
        int successCount = 0;

        foreach (var message in messages)
        {
            if (await SendAsync(message))
                successCount++;
        }

        _logger.LogInformation("Envio em lote completado: {sent}/{total} emails enviados com sucesso",
            successCount, messages.Count());

        return successCount;
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
