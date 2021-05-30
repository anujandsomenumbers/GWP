using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    [Tooltip("Disable instead of destroy.")]
    public bool willDisable = false;
    public UnityTimer timer;

    private void Awake() => timer.Completed += () => Destroy(gameObject);

    private void OnEnable() => timer.Reset();

    private void Update() => timer.Update();
}