using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneLoadInfo
{
    public string ScenePath { get; private set; }
    public int BuildIndex { get; private set; }
    public LoadSceneMode Mode { get; private set; }
    public bool WillBeActiveScene { get; private set; }

    public SceneLoadInfo (int buildIndex, LoadSceneMode mode = LoadSceneMode.Single, bool willBeActiveScene = false)
    {
        Initialize(buildIndex, mode, willBeActiveScene);
    }

    public SceneLoadInfo(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool willBeActiveScene = false)
    {
        Initialize(SceneUtility.GetBuildIndexByScenePath(sceneName), mode, willBeActiveScene);
    }

    private void Initialize(int buildIndex, LoadSceneMode mode, bool willBeActiveScene)
    {
        BuildIndex = buildIndex;
        ScenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        Mode = mode;
        WillBeActiveScene = willBeActiveScene;
    }
}
