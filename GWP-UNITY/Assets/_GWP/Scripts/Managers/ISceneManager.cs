using System;
using UnityEngine;
using USM = UnityEngine.SceneManagement;

public interface ISceneManager : IManager
{
    Action<USM.Scene> AppSceneInitializer { get; }

    // Scene load events
    event Action SceneLoadStarted;
    event Action<USM.Scene> SceneLoadCompleted;
    event Action<USM.Scene> SceneUnloadStarted;
    event Action<USM.Scene> SceneUnloadCompleted;

    void LoadScene(SceneLoadInfo loadInfo, Action<USM.Scene> initializer = null);
    void ReloadActiveScene(Action<USM.Scene> initializer = null);
    AsyncOperation LoadSceneAsync(SceneLoadInfo loadInfo, Action<USM.Scene> initializer = null);
    void UnloadScene(USM.Scene scene);
    Action<USM.Scene> MakeInitializer<T>(T initializationData);
}
