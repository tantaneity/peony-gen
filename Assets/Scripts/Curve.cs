using UnityEngine;

public static class Curve
{
    public static float Step(float from, float to, float value)
    {
        return Mathf.SmoothStep(0.0f, 1.0f, Mathf.Clamp01((value - from) / Mathf.Max(to - from, 1e-5f)));
    }
}
