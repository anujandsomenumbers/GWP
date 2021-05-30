using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class QueueSerializable<T> : IEnumerable<T>
{
    public int Count => items.Count;

    [SerializeField] private List<T> items = new List<T>();

    public void Enqueue(T item) => items.Add(item);

    public T Dequeue()
    {
        T item = items[0];
        items.RemoveAt(0);
        return item;
    }

    public void Clear() => items.Clear();

    public bool Peek(out T item)
    {
        if (0 < items.Count)
        {
            item = items[0];
            return true;
        }

        item = default;
        return false;
    }

    public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
}