using System;
using System.Collections.Generic;
using UnityEngine;
using USM = UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneLoadManager : MonoBehaviour, ISceneManager
{
    public Action<USM.Scene> AppSceneInitializer { get; private set; }

    // Scene load events
    public event Action SceneLoadStarted;
    public event Action<USM.Scene> SceneLoadCompleted;
    public event Action<USM.Scene> SceneUnloadStarted;
    public event Action<USM.Scene> SceneUnloadCompleted;

    [SerializeField] private int firstSceneIndex = 1; // Will be loaded as soon as managers have been initialized

    private string nextActiveScenePath; // Will become the active scene once loaded
    private bool wasInitialized = false;

    private readonly Dictionary<string, Action<USM.Scene>> sceneInitializers = new Dictionary<string, Action<USM.Scene>>();
    private readonly List<GameObject> rootGOs = new List<GameObject>(10);
    private readonly List<USM.Scene> loadedScenes = new List<USM.Scene>();

    public void Initialize(_App app, Action onInitialized)
    {
        if (wasInitialized) { Debug.LogError(name + " should not be initialized twice."); return; }
        AppSceneInitializer = MakeInitializer(app);

        app.AllManagersInitialized += OnManagersInitialized;

        USM.SceneManager.sceneLoaded += OnSceneLoaded;
        USM.SceneManager.sceneUnloaded += OnSceneUnloaded;

        wasInitialized = true;
        onInitialized?.Invoke();
    }

    public void LoadScene(SceneLoadInfo loadInfo, Action<USM.Scene> initializer = null)
    {
        OnSceneLoadStarted(loadInfo, initializer);
        USM.SceneManager.LoadScene(loadInfo.ScenePath, loadInfo.Mode);
    }

    public void ReloadActiveScene(Action<USM.Scene> initializer = null) =>
        LoadScene(new SceneLoadInfo(USM.SceneManager.GetActiveScene().buildIndex), initializer);

    public AsyncOperation LoadSceneAsync(SceneLoadInfo loadInfo, Action<USM.Scene> initializer = null)
    {
        OnSceneLoadStarted(loadInfo, initializer);
        return USM.SceneManager.LoadSceneAsync(loadInfo.ScenePath, loadInfo.Mode);
    }

    public void UnloadScene(USM.Scene scene)
    {
        SceneUnloadStarted?.Invoke(scene); USM.SceneManager.UnloadSceneAsync(scene);
    }

    private void OnSceneLoadStarted(SceneLoadInfo loadInfo, Action<USM.Scene> initializer)
    {
        sceneInitializers.Add(loadInfo.ScenePath, initializer);

        if (loadInfo.WillBeActiveScene)
        {
            nextActiveScenePath = loadInfo.ScenePath;
        }

        if (loadInfo.Mode == USM.LoadSceneMode.Single && null != SceneUnloadStarted)
        {
            // Single load mode will unload other scenes, so manually trigger the unload events for them
            for (int i = 0; i < loadedScenes.Count; i++)
            {
                SceneUnloadStarted(loadedScenes[i]);
            }
        }

        SceneLoadStarted?.Invoke();
    }

    private void OnSceneLoaded(USM.Scene scene, USM.LoadSceneMode mode)
    {
        loadedScenes.Add(scene);

        if (scene.path == nextActiveScenePath)
        {
            USM.SceneManager.SetActiveScene(scene);
            nextActiveScenePath = null;
        }

        if (sceneInitializers.TryGetValue(scene.path, out Action<USM.Scene> initializer))
        {
            initializer?.Invoke(scene);
            sceneInitializers.Remove(scene.path);
        }

        if (null == SceneLoadCompleted) { return; }
        SceneLoadCompleted(scene);
    }

    private void OnSceneUnloaded(USM.Scene scene)
    {
        loadedScenes.Remove(scene);

        if (null == SceneUnloadCompleted) { return; }
        SceneUnloadCompleted(scene);
    }

    private void OnManagersInitialized(_App app)
    {
        app.AllManagersInitialized -= OnManagersInitialized;

        if (0 == USM.SceneManager.GetActiveScene().buildIndex)
        {
            // Only load first scene if launching from Init
            LoadScene(new SceneLoadInfo(firstSceneIndex));
        }
        else
        {
            // Otherwise try to initialize the scene with app
            AppSceneInitializer.Invoke(USM.SceneManager.GetActiveScene());
        }
    }

    public Action<USM.Scene> MakeInitializer<T>(T initializationData)
    {
        return scene =>
        {
            scene.GetRootGameObjects(rootGOs);
            var sceneController = rootGOs.Find(go => null != go.GetComponent<ISceneController<T>>());
            sceneController.GetComponent<ISceneController<T>>().Initialize(initializationData);
            rootGOs.Clear();
        };
    }
}
