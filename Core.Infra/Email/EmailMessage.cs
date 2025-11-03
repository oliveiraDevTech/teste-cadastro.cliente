namespace Core.Infra.Email;

/// <summary>
/// Representa uma mensagem de email
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Email do destinatário
    /// </summary>
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Assunto do email
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Corpo do email em HTML
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Corpo do email em texto simples (opcional)
    /// </summary>
    public string? PlainTextBody { get; set; }

    /// <summary>
    /// Email do remetente (opcional, usa padrão se não fornecido)
    /// </summary>
    public string? From { get; set; }

    /// <summary>
    /// Cópia (CC)
    /// </summary>
    public List<string> Cc { get; set; } = new();

    /// <summary>
    /// Cópia Oculta (BCC)
    /// </summary>
    public List<string> Bcc { get; set; } = new();

    /// <summary>
    /// Anexos
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = new();

    /// <summary>
    /// Prioridade do email
    /// </summary>
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    /// <summary>
    /// Indica se é HTML
    /// </summary>
    public bool IsHtml { get; set; } = true;
}

/// <summary>
/// Representa um anexo de email
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// Nome do arquivo
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Conteúdo do arquivo em bytes
    /// </summary>
    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Tipo MIME do arquivo
    /// </summary>
    public string MimeType { get; set; } = "application/octet-stream";
}

/// <summary>
/// Prioridade do email
/// </summary>
public enum EmailPriority
{
    Low = 0,
    Normal = 1,
    High = 2
}
