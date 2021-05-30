using System.Collections.Generic;

public class Pools
{
    private readonly Dictionary<IPoolable, ObjectPoolBase> pools = new Dictionary<IPoolable, ObjectPoolBase>();

    public bool TryCreatePool<T>(T prefab, System.Action<T> onReset = null, int initialSize = 10) 
        where T : IPoolable
    {
        if (HasPool(prefab)) return false;

        var pool = new ObjectPool<T>(prefab, onReset, initialSize);
        pools.Add(prefab, pool);
        return true;
    }

    public ObjectPool<T> GetPool<T>(T prefab) where T : IPoolable
    {
        if (!pools.TryGetValue(prefab, out ObjectPoolBase poolBase)) return null;
        return (ObjectPool<T>)poolBase;
    }

    public T GetObject<T>(T prefab) where T : IPoolable
    {
        if (!HasPool(prefab))
        {
            throw new System.Exception(
                $"Can't get clone of {nameof(prefab)} because its pool doesn't exist.");
        }

        return (T)pools[prefab].Pop();
    }

    public bool HasPool(IPoolable prefab)
    {
        if (null == prefab) return false;
        return pools.ContainsKey(prefab);
    }
}