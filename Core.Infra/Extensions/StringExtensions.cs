namespace Core.Infra.Extensions;

/// <summary>
/// Extensões para trabalhar com strings
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Valida se uma string é um email válido
    /// </summary>
    public static bool IsValidEmail(this string email)
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

    /// <summary>
    /// Valida se uma string é um CPF válido (apenas formato)
    /// </summary>
    public static bool IsValidCpfFormat(this string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        cpf = Regex.Replace(cpf, @"\D", "");

        // Deve ter 11 dígitos
        return cpf.Length == 11 && Regex.IsMatch(cpf, @"^\d{11}$");
    }

    /// <summary>
    /// Valida se uma string é um CEP válido (formato brasileiro)
    /// </summary>
    public static bool IsValidCepFormat(this string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return false;

        // Remove caracteres não numéricos
        cep = Regex.Replace(cep, @"\D", "");

        // Deve ter 8 dígitos
        return cep.Length == 8 && Regex.IsMatch(cep, @"^\d{8}$");
    }

    /// <summary>
    /// Valida se uma string é um telefone válido (formato brasileiro)
    /// </summary>
    public static bool IsValidPhoneFormat(this string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Remove caracteres não numéricos
        var cleanPhone = Regex.Replace(phone, @"\D", "");

        // Deve ter entre 10 e 11 dígitos
        return cleanPhone.Length >= 10 && cleanPhone.Length <= 11;
    }

    /// <summary>
    /// Remove máscara de um documento (CPF, CNPJ, etc)
    /// </summary>
    public static string RemoveMask(this string value)
    {
        return Regex.Replace(value ?? string.Empty, @"\D", "");
    }

    /// <summary>
    /// Trunca uma string para um tamanho máximo
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// Converte uma string para capitalização de título
    /// </summary>
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var textInfo = new System.Globalization.CultureInfo("pt-BR", false).TextInfo;
        return textInfo.ToTitleCase(value.ToLower());
    }
}
