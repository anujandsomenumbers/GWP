using UnityEngine;
using System;

[DisallowMultipleComponent]
public class _App : Singleton<_App>
{
    public bool AreAllManagersInitialized { get; private set; }

    // Service locators
    public IInputManager Input { get; private set; }
    public ISceneManager Scene { get; private set; }

    public event Action<_App> AllManagersInitialized;

    // Prefabs from which to spawn managers (services)
    [RequireInterface(typeof(IInputManager))][SerializeField] private GameObject inputPrefab = null;
    [RequireInterface(typeof(ISceneManager))][SerializeField] private GameObject scenePrefab = null;

    // Count of Managers that have started but not yet completed initialization.
    private int initializationsOutstanding;

    public void SubscribeInitialized(Action<_App> onInitialized)
    {
        if (AreAllManagersInitialized)
        {
            onInitialized(this);
        }
        else
        {
            AllManagersInitialized += onInitialized;
        }
    }

    protected override void CreatInstance()
    {
        base.CreatInstance();
        InitializeManagers(SpawnManagers());
        DontDestroyOnLoad(gameObject);
    }
    
    // Service providers
    private IManager[] SpawnManagers()
    {
        var spawnedManagers = new IManager[]
        {
            // Spawn managers here. Make sure to pass in this object's transform as the parent.
            Scene = Instantiate(scenePrefab, transform).GetComponent<ISceneManager>(),
            Input = Instantiate(inputPrefab, transform).GetComponent<IInputManager>(),
        };

        return spawnedManagers;
    }

    private void InitializeManagers(IManager[] managers)
    {
        initializationsOutstanding = managers.Length;
        foreach (IManager manager in managers)
        {
            manager.Initialize(this, OnManagerInitialized);
        }
    }

    private void OnManagerInitialized()
    {
        --initializationsOutstanding;
        if (0 == initializationsOutstanding)
        {
            AreAllManagersInitialized = true;
            AllManagersInitialized?.Invoke(this);
            AllManagersInitialized = null;
        }
    }
}
