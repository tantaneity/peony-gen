using System.Collections.Generic;
using UnityEngine;

public struct Pose
{
    public Vector3 seat;
    public Quaternion lean;
    public float scale;

    public Vector3 Place(Vector3 local)
    {
        return seat + lean * (local * scale);
    }

    public Vector3 Aim(Vector3 local)
    {
        return lean * local;
    }
}

public sealed class StemShape
{
    public readonly List<Vector3> spine = new List<Vector3>();
    public readonly List<float> radii = new List<float>();

    public Vector3 At(float along)
    {
        float index = Mathf.Clamp01(along) * (spine.Count - 1);
        int lower = Mathf.Min(Mathf.FloorToInt(index), spine.Count - 2);
        return Vector3.Lerp(spine[lower], spine[lower + 1], index - lower);
    }

    public float RadiusAt(float along)
    {
        float index = Mathf.Clamp01(along) * (radii.Count - 1);
        int lower = Mathf.Min(Mathf.FloorToInt(index), radii.Count - 2);
        return Mathf.Lerp(radii[lower], radii[lower + 1], index - lower);
    }
}

public static class Stem
{
    private const float FootFlare = 1.5f;
    private const float NeckSwell = 1.25f;

    public static float BendAt(PeonySettings settings, float along)
    {
        return settings.stemLean + settings.nod * Mathf.SmoothStep(0.0f, 1.0f,
            Mathf.Clamp01((along - settings.nodStart) / (1.0f - settings.nodStart)));
    }

    public static Vector3 Heading(float bendDegrees)
    {
        float bend = bendDegrees * Mathf.Deg2Rad;
        return new Vector3(0.0f, Mathf.Cos(bend), -Mathf.Sin(bend));
    }

    public static StemShape Trace(PeonySettings settings)
    {
        StemShape shape = new StemShape();
        int rings = Mathf.Max(settings.stemRings, 2);
        float step = settings.stemHeight * settings.grow / (rings - 1);
        float girth = Mathf.Lerp(0.35f, 1.0f, settings.grow);
        Vector3 cursor = Vector3.zero;

        for (int ring = 0; ring < rings; ring++)
        {
            float along = ring / (float)(rings - 1);
            shape.spine.Add(cursor);
            float flare = Mathf.Lerp(FootFlare, 1.0f, Mathf.Sqrt(along));
            float swell = Mathf.Lerp(1.0f, NeckSwell, Curve.Step(settings.nodStart, 1.0f, along));
            shape.radii.Add(settings.stemRadius * girth * flare * swell * Mathf.Lerp(1.0f, 0.75f, along));
            cursor += Heading(BendAt(settings, along)) * step;
        }

        return shape;
    }

    public static Pose Build(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, StemShape shape)
    {
        Tube.Build(mesh, shape.spine, shape.radii, palette.stem, Outline.Contour, settings.depthBias);
        Vector3 facing = Heading(BendAt(settings, 1.0f));
        return new Pose
        {
            seat = shape.spine[shape.spine.Count - 1] + facing * (settings.hipDepth * settings.headScale),
            lean = Quaternion.FromToRotation(Vector3.up, facing),
            scale = settings.headScale
        };
    }
}
