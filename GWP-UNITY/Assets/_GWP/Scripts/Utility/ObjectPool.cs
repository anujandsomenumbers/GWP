using System.Collections.Generic;

public class ObjectPool<T> : ObjectPoolBase where T : IPoolable
{
    public ObjectPool(T prefab, System.Action<T> onReset = null, int initialSize = 10) 
        : base(prefab, initialSize)
    {
        Reset += p => onReset((T)p);
        Populate(initialSize);
    }

    public new T Pop() => (T)base.Pop();
}

public abstract class ObjectPoolBase
{
    public event System.Action<IPoolable> Reset;

    private readonly IPoolable prefab;
    private readonly Stack<IPoolable> poolCollection;
    private readonly HashSet<IPoolable> unused;

    public ObjectPoolBase(IPoolable prefab, int initialSize = 10, bool willPopulate = false)
    {
        this.prefab = prefab ?? throw new System.ArgumentNullException(nameof(prefab));

        poolCollection = new Stack<IPoolable>(initialSize);
        unused = new HashSet<IPoolable>();
        if (willPopulate) Populate(initialSize);
    }

    public IPoolable Pop()
    {
        if (1 > poolCollection.Count) CreateNew();
        IPoolable obj = poolCollection.Pop();
        unused.Remove(obj);
        return obj;
    }

    protected void Populate(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CreateNew();
        }
    }

    private void Push(IPoolable pooledObject)
    {
        if (unused.Contains(pooledObject)) return;
        Reset?.Invoke(pooledObject);
        unused.Add(pooledObject);
        poolCollection.Push(pooledObject);
    }

    private IPoolable CreateNew()
    {
        var obj = prefab.CreateCopy();
        obj.Disposed += o => Push(o);
        poolCollection.Push(obj);
        unused.Add(obj);

        Reset?.Invoke(obj);
        return obj;
    }
}
