using System;
using UnityEngine;

public interface IManager
{
    void Initialize(_App app, Action onInitialized);
}