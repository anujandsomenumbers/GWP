using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasGroupExtensions
{
    public static void Show(this CanvasGroup group, bool willShow)
    {
        group.blocksRaycasts = willShow;
        group.alpha = willShow ? 1 : 0;
    }
}
