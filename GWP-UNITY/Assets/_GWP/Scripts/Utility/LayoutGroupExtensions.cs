using UnityEngine.UI;

public static class LayoutGroupExtensions
{
    public static void RefreshLayout(this LayoutGroup layoutGroup)
    {
        layoutGroup.CalculateLayoutInputHorizontal();
        layoutGroup.CalculateLayoutInputVertical();
        layoutGroup.SetLayoutHorizontal();
        layoutGroup.SetLayoutVertical();
    }
}
