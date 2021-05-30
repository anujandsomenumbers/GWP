using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T RandomElement<T>(this IReadOnlyList<T> list) => list[Random.Range(0, list.Count)];
}
