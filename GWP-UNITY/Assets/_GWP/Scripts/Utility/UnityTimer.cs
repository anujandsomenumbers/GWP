using UnityEngine;

[System.Serializable]
public class UnityTimer
{
    public bool IsRunning { get; private set; }
    public float duration;
    [ReadOnlyInspector] public float currentTime;

    public event System.Action Completed;

    public UnityTimer(float duration, float startTime = 0)
    {
        this.duration = duration;
        currentTime = startTime;
    }

    public void Reset(float startTime = 0)
    {
        currentTime = startTime;
        IsRunning = true;
    }

    public void Update()
    {
        if (!IsRunning) return;
        if (currentTime <= duration)
        {
            currentTime += Time.deltaTime;
            return;
        }
        IsRunning = false;
        Completed?.Invoke();
    }
}