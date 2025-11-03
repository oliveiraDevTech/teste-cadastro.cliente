namespace Core.Infra.Extensions;

/// <summary>
/// Extensões para trabalhar com DateTime
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Formata um DateTime para o padrão brasileiro
    /// </summary>
    public static string ToDateBr(this DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Formata um DateTime para o padrão brasileiro com hora
    /// </summary>
    public static string ToDateTimeBr(this DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
    }

    /// <summary>
    /// Formata um DateTime para o padrão ISO 8601
    /// </summary>
    public static string ToIsoString(this DateTime dateTime)
    {
        return dateTime.ToString("o");
    }

    /// <summary>
    /// Obtém o tempo decorrido desde uma data até agora
    /// </summary>
    public static TimeSpan TimeAgo(this DateTime dateTime)
    {
        return DateTime.UtcNow - dateTime.ToUniversalTime();
    }

    /// <summary>
    /// Verifica se uma data está no futuro
    /// </summary>
    public static bool IsFuture(this DateTime dateTime)
    {
        return dateTime > DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica se uma data está no passado
    /// </summary>
    public static bool IsPast(this DateTime dateTime)
    {
        return dateTime < DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica se uma data é hoje
    /// </summary>
    public static bool IsToday(this DateTime dateTime)
    {
        return dateTime.Date == DateTime.Now.Date;
    }

    /// <summary>
    /// Obtém a descrição em texto do tempo decorrido (ex: "há 2 dias")
    /// </summary>
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime.ToUniversalTime();

        return timeSpan.TotalSeconds switch
        {
            < 60 => "agora mesmo",
            < 120 => "há 1 minuto",
            < 3600 => $"há {(int)timeSpan.TotalMinutes} minutos",
            < 7200 => "há 1 hora",
            < 86400 => $"há {(int)timeSpan.TotalHours} horas",
            < 172800 => "ontem",
            < 2592000 => $"há {(int)timeSpan.TotalDays} dias",
            < 5184000 => "há 1 mês",
            _ => $"há {(int)(timeSpan.TotalDays / 30)} meses"
        };
    }
}
