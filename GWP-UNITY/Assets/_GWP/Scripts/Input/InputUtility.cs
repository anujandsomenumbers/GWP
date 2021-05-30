using UnityEngine;

public static class InputUtility
{
    /// <summary>
    /// Converts Pixels to Inches.
    /// </summary>
    /// <param name="pixels">The amount to convert in pixels.</param>
    /// <returns>The converted amount in inches.</returns>
    public static float PixelsToInches(float pixels)
    {
        return pixels / Screen.dpi;
    }

    /// <summary>
    /// Converts Inches to Pixels.
    /// </summary>
    /// <param name="inches">The amount to convert in inches.</param>
    /// <returns>The converted amount in pixels.</returns>
    public static float InchesToPixels(float inches)
    {
        return inches * Screen.dpi;
    }

    /// <summary>
    /// Performs a Raycast from the camera.
    /// </summary>
    /// <param name="cam">The camera to perform the raycast from.</param>
    /// <param name="screenPos">The screen position to perform the raycast from.</param>
    /// <param name="result">The RaycastHit result.</param>
    /// <returns>True if an object was hit.</returns>
    public static bool RaycastFromCamera(Camera cam, Vector2 screenPos, out RaycastHit result)
    {
        if (cam == null)
        {
            result = new RaycastHit();
            return false;
        }

        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            result = hit;
            return true;
        }

        result = hit;
        return false;
    }
}
