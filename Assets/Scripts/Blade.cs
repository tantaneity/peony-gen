using System;
using UnityEngine;

public struct BladePlan
{
    public Vector3 root;
    public Vector3 heading;
    public Vector3 flank;
    public float length;
    public float width;
    public Func<float, float> outline;
    public float curl;
    public float cup;
    public float roll;
    public float ruffle;
    public Color fill;
    public Color rib;
    public Color tip;
    public float ink;
}

public static class Blade
{
    private const int Rows = 16;
    private const float RollFrom = 0.5f;
    private const float PetalClaw = 0.08f;
    private const float PetalShoulder = 0.82f;
    private const int Teeth = 5;
    private const float ToothDepth = 0.16f;
    private const int Columns = 8;
    private const float RuffleWaves = 2.5f;
    private const float RibShare = 0.18f;

    public static void Build(MeshBuffer mesh, BladePlan plan, float depthBias)
    {
        int first = mesh.VertexCount;
        Vector3 cursor = plan.root;
        Vector3 tangent = plan.heading.normalized;
        float step = plan.length / Rows;

        for (int row = 0; row <= Rows; row++)
        {
            float along = row / (float)Rows;
            Vector3 normal = Vector3.Cross(plan.flank, tangent).normalized;
            float half = plan.width * 0.5f * plan.outline(along);

            for (int column = 0; column <= Columns; column++)
            {
                float across = 2.0f * column / Columns - 1.0f;
                float wave = Mathf.Sin(Mathf.PI * (along * RuffleWaves + across * 0.35f));
                float lift = (plan.cup + plan.ruffle * wave * along) * across * across * half;
                Vector3 place = cursor + plan.flank * (across * half) + normal * lift;
                Vector3 shadeNormal = (normal - plan.flank * (2.0f * plan.cup * across)).normalized;
                Color body = Mathf.Abs(across) < RibShare ? plan.rib : plan.fill;
                Color fill = plan.tip.a > 0.0f ? Color.Lerp(body, plan.tip, along * along) : body;
                mesh.AddVertex(place, Vector3.zero, Vector4.zero, fill, StrokeKind.Card, 0.0f, plan.ink,
                    depthBias, Shading.Surface(shadeNormal));
            }

            if (row > 0)
            {
                int lower = first + (row - 1) * (Columns + 1);
                int upper = lower + Columns + 1;
                for (int column = 0; column < Columns; column++)
                {
                    mesh.AddQuad(lower + column, lower + column + 1, upper + column + 1, upper + column);
                }
            }

            float bend = plan.curl / Rows;
            if (along >= RollFrom)
            {
                bend += plan.roll / (Rows * (1.0f - RollFrom));
            }

            tangent = Quaternion.AngleAxis(bend, plan.flank) * tangent;
            cursor += tangent * step;
        }
    }

    public static float Lance(float along)
    {
        return Mathf.Pow(Mathf.Max(Mathf.Sin(Mathf.PI * Mathf.Lerp(0.18f, 1.0f, along)), 0.0f), 0.7f);
    }

    public static float Heart(float along)
    {
        float lobe = 0.55f + 0.45f * Mathf.Sin(Mathf.PI * Mathf.Min(along / 0.35f, 1.0f) * 0.5f);
        return lobe * Mathf.Pow(Mathf.Max(1.0f - along, 0.0f), 0.85f) * 1.25f;
    }

    public static float Point(float along)
    {
        return Mathf.Pow(Mathf.Max(Mathf.Sin(Mathf.PI * Mathf.Lerp(0.25f, 1.0f, along)), 0.0f), 1.3f);
    }

    public static float Petal(float along)
    {
        float body = Mathf.Sin(Mathf.PI * Mathf.Lerp(PetalClaw, PetalShoulder, along));
        return Mathf.Pow(Mathf.Max(body, 0.0f), 0.6f) * Mathf.Lerp(0.55f, 1.0f, along);
    }

    public static float Leaflet(float along)
    {
        float body = Mathf.Pow(Mathf.Max(Mathf.Sin(Mathf.PI * Mathf.Lerp(0.04f, 1.0f, along)), 0.0f), 0.75f);
        return body * (1.0f - ToothDepth * Mathf.Repeat(along * Teeth, 1.0f));
    }
}
