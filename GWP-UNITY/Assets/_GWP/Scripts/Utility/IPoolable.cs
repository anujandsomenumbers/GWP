public interface IPoolable : System.IDisposable
{
    event System.Action<IPoolable> Disposed;
    IPoolable CreateCopy();
}