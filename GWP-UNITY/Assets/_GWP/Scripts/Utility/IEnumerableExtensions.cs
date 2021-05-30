using System.Collections.Generic;

public static class IEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> ts, System.Action<T> action)
    {
        foreach (var t in ts) action(t);
    }
}
