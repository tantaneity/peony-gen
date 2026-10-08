using UnityEngine;

public static class Leaves
{
    private const int RachisNodes = 6;
    private const float RachisSag = 0.3f;
    private const float LastSprout = 0.8f;
    private const float SproutSpan = 0.2f;
    private const float MatureSpan = 0.3f;
    private const float DryDroop = -55.0f;
    private const float YoungLeaf = 0.3f;
    private const float SegmentShare = 0.35f;
    private const float CentreLeaflet = 1.1f;
    private const float SideLeaflet = 0.85f;

    private static readonly float[] SegmentFan = { -55.0f, 0.0f, 55.0f };
    private static readonly float[] LeafletFan = { -32.0f, 0.0f, 32.0f };

    private struct LeafPlan
    {
        public Vector3 node;
        public Vector3 heading;
        public float size;
        public float young;
        public float wither;
        public int index;
    }

    public static void Build(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, StemShape stem)
    {
        int count = Mathf.Max(settings.leafCount, 1);
        float rise = settings.leafRise * Mathf.Deg2Rad;
        for (int index = 0; index < count; index++)
        {
            float rank = count > 1 ? index / (float)(count - 1) : 0.0f;
            float sprout = Curve.Step(rank * LastSprout, rank * LastSprout + SproutSpan, settings.grow);
            if (sprout <= 0.0f)
            {
                continue;
            }

            float along = Mathf.Lerp(settings.leafLowest, settings.leafHighest, rank);
            float azimuth = index * Phyllotaxis.GoldenAngle * Mathf.Deg2Rad;
            Vector3 outward = new Vector3(Mathf.Cos(azimuth), 0.0f, Mathf.Sin(azimuth));
            Build(mesh, settings, palette, new LeafPlan
            {
                node = stem.At(along) + outward * (stem.RadiusAt(along) * 0.8f),
                heading = outward * Mathf.Cos(rise) + Vector3.up * Mathf.Sin(rise),
                size = sprout * Mathf.Lerp(YoungLeaf, 1.0f, settings.grow) * (1.0f + Noise.Signed(index, settings.seed + 3) * 0.08f),
                young = 1.0f - Curve.Step(rank * LastSprout, rank * LastSprout + MatureSpan, settings.grow),
                wither = Curve.Step(rank * 0.55f, rank * 0.55f + 0.45f, settings.dry),
                index = index
            });
        }
    }

    private static Vector3 RachisPoint(LeafPlan leaf, float length, float along)
    {
        return leaf.node + leaf.heading * (length * along) + Vector3.down * (RachisSag * length * along * along);
    }

    private static Vector3 RachisTangent(LeafPlan leaf, float length, float along)
    {
        return (leaf.heading * length + Vector3.down * (2.0f * RachisSag * length * along)).normalized;
    }

    private static void Build(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, LeafPlan leaf)
    {
        float length = settings.rachisLength * leaf.size;
        Vector3[] spine = new Vector3[RachisNodes];
        float[] radii = new float[RachisNodes];
        for (int node = 0; node < RachisNodes; node++)
        {
            float along = node / (float)(RachisNodes - 1);
            spine[node] = RachisPoint(leaf, length, along);
            radii[node] = settings.rachisRadius * Mathf.Lerp(1.3f, 0.7f, along) * Mathf.Sqrt(leaf.size);
        }

        Tube.Build(mesh, spine, radii, Color.Lerp(palette.stem, palette.leafDry, leaf.wither), Outline.Small, settings.depthBias);

        Vector3 end = spine[RachisNodes - 1];
        Vector3 tipward = RachisTangent(leaf, length, 1.0f);
        foreach (float fan in SegmentFan)
        {
            Vector3 segment = Quaternion.AngleAxis(fan, Vector3.up) * tipward;
            Vector3 fork = end + segment * (length * SegmentShare);
            Tube.Build(mesh, new[] { end, fork }, new[] { radii[RachisNodes - 1], radii[RachisNodes - 1] * 0.7f },
                Color.Lerp(palette.stem, palette.leafDry, leaf.wither), Outline.Small, settings.depthBias);
            for (int lobe = 0; lobe < LeafletFan.Length; lobe++)
            {
                Vector3 heading = Quaternion.AngleAxis(LeafletFan[lobe], Vector3.up) * segment;
                float share = lobe == 1 ? CentreLeaflet : SideLeaflet;
                BuildLeaflet(mesh, settings, palette, leaf, fork, heading, share, leaf.index * 11 + (int)fan + lobe);
            }
        }
    }

    private static void BuildLeaflet(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, LeafPlan leaf,
        Vector3 root, Vector3 heading, float share, int tone)
    {
        Color mature = Noise.Vary(palette.leaf, tone, settings.seed + 53, 0.08f);
        Color green = Color.Lerp(mature, palette.leafYoung, leaf.young * 0.6f);
        float size = leaf.size * share * (1.0f + Noise.Signed(tone, settings.seed + 59) * 0.07f);
        Blade.Build(mesh, new BladePlan
        {
            root = root,
            heading = heading,
            flank = Vector3.Cross(heading, Vector3.up).normalized,
            length = settings.leafletLength * size,
            width = settings.leafletWidth * size,
            outline = Blade.Lance,
            curl = settings.leafDroop + DryDroop * leaf.wither,
            cup = settings.leafletCup,
            fill = Color.Lerp(green, palette.leafDry, leaf.wither),
            rib = Color.Lerp(palette.leafRib, palette.leafDry, leaf.wither),
            tip = Color.Lerp(green * 0.9f, palette.leafDry * 0.8f, Mathf.Min(leaf.wither * 1.5f, 1.0f)),
            ink = Outline.Contour
        }, settings.depthBias);
    }
}
