namespace Core.Infra.Extensions;

/// <summary>
/// Extensões para trabalhar com coleções
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Verifica se uma coleção é null ou está vazia
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }

    /// <summary>
    /// Aplica paginação a uma coleção
    /// </summary>
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Obtém informações de paginação de uma coleção
    /// </summary>
    public static (IEnumerable<T> items, int total, int pages) GetPaginationInfo<T>(
        this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        var items = source.ToList();
        var total = items.Count;
        var pages = (int)Math.Ceiling((double)total / pageSize);

        var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return (pagedItems, total, pages);
    }

    /// <summary>
    /// Executa uma ação para cada item em uma coleção
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    /// <summary>
    /// Executa uma ação assíncrona para cada item em uma coleção
    /// </summary>
    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
        foreach (var item in source)
        {
            await action(item);
        }
    }

    /// <summary>
    /// Agrupa itens por um valor de chave e os ordena
    /// </summary>
    public static IEnumerable<IGrouping<TKey, T>> GroupAndSort<T, TKey>(
        this IEnumerable<T> source, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
    {
        return source.GroupBy(keySelector).OrderBy(g => g.Key);
    }

    /// <summary>
    /// Obtém a mediana de uma sequência de números
    /// </summary>
    public static decimal GetMedian(this IEnumerable<decimal> source)
    {
        var sorted = source.OrderBy(x => x).ToList();
        if (sorted.Count == 0)
            return 0;

        int n = sorted.Count;
        if (n % 2 == 0)
            return (sorted[n / 2 - 1] + sorted[n / 2]) / 2;

        return sorted[n / 2];
    }

    /// <summary>
    /// Verifica se os valores de duas coleções são iguais, independentemente da ordem
    /// </summary>
    public static bool HasSameElements<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
        var firstList = first.OrderBy(x => x).ToList();
        var secondList = second.OrderBy(x => x).ToList();

        if (firstList.Count != secondList.Count)
            return false;

        return !firstList.Except(secondList).Any();
    }
}
