public static class UIHelper
{
    public static float FittedSize(float elementSize, int elementCount, float spacing, float padding)
    {
        float offset = elementSize * elementCount
            + spacing * (elementCount - 1)
            + padding;
        return offset;
    }
}
