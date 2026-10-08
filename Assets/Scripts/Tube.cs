using System.Collections.Generic;
using UnityEngine;

public static class Tube
{
    private const int Sides = 10;
    private const float FullTurn = 360.0f;

    public static void Build(MeshBuffer mesh, IReadOnlyList<Vector3> spine, IReadOnlyList<float> radii,
        Color fill, float outline, float depthBias)
    {
        int rings = spine.Count;
        int first = mesh.VertexCount;
        Vector3 tangent = (spine[1] - spine[0]).normalized;
        Vector3 side = Vector3.Cross(tangent, Vector3.forward);
        if (side.sqrMagnitude < 1e-6f)
        {
            side = Vector3.Cross(tangent, Vector3.right);
        }

        side.Normalize();

        for (int ring = 0; ring < rings; ring++)
        {
            Vector3 ahead = ring < rings - 1 ? spine[ring + 1] - spine[ring] : spine[ring] - spine[ring - 1];
            Vector3 next = ahead.normalized;
            side = Vector3.Cross(next, Vector3.Cross(side, next)).normalized;
            tangent = next;
            Vector3 outward = Vector3.Cross(tangent, side);

            for (int face = 0; face < Sides; face++)
            {
                float angle = FullTurn * face / Sides * Mathf.Deg2Rad;
                Vector3 rim = side * Mathf.Cos(angle) + outward * Mathf.Sin(angle);
                mesh.AddVertex(spine[ring] + rim * radii[ring], Vector3.zero, Vector4.zero, fill,
                    StrokeKind.Card, 0.0f, outline, depthBias, Shading.Surface(rim));
            }
        }

        for (int ring = 0; ring < rings - 1; ring++)
        {
            int lower = first + ring * Sides;
            int upper = lower + Sides;
            for (int face = 0; face < Sides; face++)
            {
                int next = (face + 1) % Sides;
                mesh.AddQuad(lower + face, lower + next, upper + next, upper + face);
            }
        }
    }
}
