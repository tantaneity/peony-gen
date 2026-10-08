using System.Collections.Generic;
using UnityEngine;

public static class Revolve
{
    private const int Sides = 56;
    private const float FullTurn = 360.0f;

    public static void Build(MeshBuffer mesh, Pose pose, IReadOnlyList<Vector2> profile, Color fill,
        float outline, float depthBias)
    {
        int first = mesh.VertexCount;
        for (int ring = 0; ring < profile.Count; ring++)
        {
            Vector2 before = profile[Mathf.Max(ring - 1, 0)];
            Vector2 after = profile[Mathf.Min(ring + 1, profile.Count - 1)];
            Vector2 slope = (after - before).normalized;
            Vector2 normal2 = new Vector2(slope.y, -slope.x);

            for (int side = 0; side < Sides; side++)
            {
                float angle = FullTurn * side / Sides * Mathf.Deg2Rad;
                Vector3 outward = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
                Vector3 place = outward * profile[ring].x + Vector3.up * profile[ring].y;
                Vector3 normal = outward * normal2.x + Vector3.up * normal2.y;
                mesh.AddVertex(pose.Place(place), Vector3.zero, Vector4.zero, fill, StrokeKind.Card, 0.0f,
                    outline, depthBias, Shading.Surface(pose.Aim(normal)));
            }
        }

        for (int ring = 0; ring < profile.Count - 1; ring++)
        {
            int inner = first + ring * Sides;
            int outer = inner + Sides;
            for (int side = 0; side < Sides; side++)
            {
                int next = (side + 1) % Sides;
                mesh.AddQuad(inner + side, outer + side, outer + next, inner + next);
            }
        }
    }
}
