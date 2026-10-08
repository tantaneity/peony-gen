using System.Collections.Generic;
using UnityEngine;

public static class Bloom
{
    private const int HipRings = 10;
    private const float HipLip = 1.05f;
    private const float HipBulge = 0.3f;
    private const float SepalTurn = 144.0f;
    private const float SepalStagger = 0.35f;
    private const float SepalWrap = 40.0f;
    private const float SepalReflex = 10.0f;
    private const float SepalHug = 1.2f;
    private const float SepalCover = 1.0f;
    private const float SepalSeat = 0.12f;

    public static void Build(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, Pose pose)
    {
        BuildHip(mesh, settings, palette, pose);
        BuildSepals(mesh, settings, palette, pose);
        Petals.Build(mesh, settings, palette, pose);
    }

    private static Vector2 HipPoint(PeonySettings settings, float along)
    {
        float radius = Mathf.Lerp(settings.coreRadius * HipLip, settings.neckRadius, along)
                       + settings.hipRadius * HipBulge * Mathf.Sin(Mathf.PI * along);
        return new Vector2(radius, -settings.hipDepth * along);
    }

    private static void BuildHip(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, Pose pose)
    {
        List<Vector2> profile = new List<Vector2> { Vector2.zero };
        for (int ring = 0; ring <= HipRings; ring++)
        {
            profile.Add(HipPoint(settings, ring / (float)HipRings));
        }

        profile.Add(new Vector2(0.0f, profile[profile.Count - 1].y));
        Color tone = Color.Lerp(palette.hip, palette.leafDry, settings.dry);
        Revolve.Build(mesh, pose, profile, tone, Outline.Silhouette, settings.depthBias);
    }

    private static void BuildSepals(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, Pose pose)
    {
        Vector2 seat = HipPoint(settings, SepalSeat);
        for (int sepal = 0; sepal < settings.sepalCount; sepal++)
        {
            float angle = sepal * SepalTurn * Mathf.Deg2Rad;
            Vector3 outward = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
            Vector3 flank = Vector3.Cross(outward, Vector3.up).normalized;
            float late = Noise.Hash(sepal, settings.seed + 41) * SepalStagger;
            float open = Curve.Step(late, late + 1.0f - SepalStagger, settings.sepalOpen);
            Vector3 shut = (Vector3.up + outward * 0.15f).normalized;
            Vector3 spread = (outward - Vector3.up * 0.2f).normalized;
            Color green = Noise.Vary(palette.sepal, sepal, settings.seed + 43, 0.08f);

            Blade.Build(mesh, new BladePlan
            {
                root = pose.Place(outward * seat.x + Vector3.up * seat.y),
                heading = pose.Aim(Vector3.Slerp(shut, spread, open)),
                flank = pose.Aim(flank),
                length = settings.sepalLength * pose.scale * (1.0f + Noise.Signed(sepal, settings.seed + 45) * 0.1f)
                         * Mathf.Lerp(SepalCover, 1.0f, open),
                width = settings.sepalWidth * pose.scale * Mathf.Lerp(SepalHug, 1.0f, open),
                outline = Blade.Petal,
                curl = Mathf.Lerp(SepalWrap, SepalReflex, open),
                cup = 0.5f,
                fill = Color.Lerp(green, palette.leafDry, settings.dry),
                rib = Color.Lerp(palette.leafRib, palette.leafDry, settings.dry),
                tip = Color.Lerp(green * 0.85f, palette.leafDry * 0.8f, settings.dry),
                ink = Outline.Small
            }, settings.depthBias);
        }
    }
}
