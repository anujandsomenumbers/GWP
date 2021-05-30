using UnityEngine;

[DisallowMultipleComponent]
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; protected set; }

    protected virtual void Awake()
    {
        if (null != Instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        CreatInstance();
    }

    protected virtual void CreatInstance()
    {
        Instance = (T)this;
    }
}
