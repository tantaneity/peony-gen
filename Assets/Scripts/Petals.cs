using UnityEngine;

public static class Petals
{
    private const float GuardBudTilt = 30.0f;
    private const float GuardBudCurl = 150.0f;
    private const float GuardOpenTilt = 62.0f;
    private const float GuardOpenCurl = 22.0f;
    private const float GuardBudCup = 0.9f;
    private const float GuardOpenCup = 0.55f;
    private const float GuardStagger = 0.4f;
    private const float GuardRuffle = 0.12f;
    private const float GuardSink = 0.003f;
    private const float BudTaper = 0.45f;

    private const float InnerSpread = 0.9f;
    private const float InnerBudScale = 0.35f;
    private const float CentreTilt = 4.0f;
    private const float EdgeTilt = 72.0f;
    private const float InnerCurl = 75.0f;
    private const float InnerCup = 0.7f;
    private const float InnerRuffle = 0.22f;
    private const float CentreLength = 0.95f;
    private const float InnerDome = 0.014f;
    private const float ExpandStagger = 0.5f;
    private const float TiltJitter = 14.0f;
    private const float CurlJitter = 30.0f;
    private const float TwistJitter = 25.0f;

    private const float WiltStagger = 0.4f;
    private const float WiltSag = 35.0f;
    private const float WiltSlump = -30.0f;
    private const float WiltShrink = 0.85f;
    private const float WiltBrown = 0.35f;

    private struct PetalPlan
    {
        public Vector3 root;
        public Vector3 outward;
        public float tilt;
        public float length;
        public float width;
        public float taper;
        public float curl;
        public float cup;
        public float ruffle;
        public Color body;
        public Color edge;
    }

    public static void Build(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, Pose pose)
    {
        BuildInner(mesh, settings, palette, pose);
        BuildGuards(mesh, settings, palette, pose);
    }

    private static float Wilt(PeonySettings settings, int index, int salt)
    {
        float from = Noise.Hash(index, settings.seed + salt) * WiltStagger;
        return Curve.Step(from, from + 1.0f - WiltStagger, settings.wilt);
    }

    private static Color Fade(PeonySettings settings, PeonyPalette palette, Color colour, float wilt)
    {
        Color blushed = Color.Lerp(palette.petalBud, colour, settings.blush);
        Color faded = Color.Lerp(blushed, palette.petalFade, wilt * 0.7f);
        return Color.Lerp(faded, palette.petalDry, wilt * wilt * WiltBrown);
    }

    private static void BuildGuards(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, Pose pose)
    {
        for (int guard = 0; guard < settings.guardCount; guard++)
        {
            float angle = guard * Phyllotaxis.GoldenAngle * Mathf.Deg2Rad;
            Vector3 outward = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
            float late = Noise.Hash(guard, settings.seed + 71) * GuardStagger;
            float open = Curve.Step(late, late + 1.0f - GuardStagger, settings.bloom);
            float wilt = Wilt(settings, guard, 73);
            float shrink = Mathf.Lerp(1.0f, WiltShrink, wilt);
            Color body = Noise.Vary(palette.guard, guard, settings.seed + 75, 0.05f);

            Build(mesh, settings, pose, new PetalPlan
            {
                root = outward * settings.coreRadius + Vector3.down * GuardSink,
                outward = outward,
                tilt = Mathf.Lerp(GuardBudTilt, GuardOpenTilt, open) + WiltSag * wilt,
                length = settings.guardLength * shrink * (1.0f + Noise.Signed(guard, settings.seed + 77) * 0.08f),
                width = settings.guardWidth * shrink,
                taper = BudTaper * (1.0f - open),
                curl = Mathf.Lerp(GuardBudCurl, GuardOpenCurl, open),
                cup = Mathf.Lerp(GuardBudCup, GuardOpenCup, open),
                ruffle = GuardRuffle * open,
                body = Fade(settings, palette, body, wilt),
                edge = Fade(settings, palette, palette.petalEdge, wilt)
            });
        }
    }

    private static void BuildInner(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette, Pose pose)
    {
        for (int petal = 0; petal < settings.innerCount; petal++)
        {
            Vector2 spot = Phyllotaxis.OnDisc(petal, settings.innerCount);
            float reach = spot.magnitude;
            Vector3 outward = new Vector3(spot.x, 0.0f, spot.y) / Mathf.Max(reach, 1e-4f);
            Vector3 facing = Quaternion.AngleAxis(Noise.Signed(petal, settings.seed + 91) * TwistJitter, Vector3.up) * outward;
            float late = (1.0f - reach) * ExpandStagger;
            float expand = Curve.Step(late, late + 1.0f - ExpandStagger, settings.fluff);
            float wilt = Wilt(settings, petal, 81);
            float shrink = Mathf.Lerp(InnerBudScale, 1.0f, expand) * Mathf.Lerp(1.0f, WiltShrink, wilt);
            Color body = Noise.Vary(Color.Lerp(palette.innerDeep, palette.inner, reach * 0.7f + Noise.Hash(petal, settings.seed + 83) * 0.3f),
                petal, settings.seed + 85, 0.05f);

            Build(mesh, settings, pose, new PetalPlan
            {
                root = outward * (settings.coreRadius * InnerSpread * reach) + Vector3.up * (InnerDome * (1.0f - reach * reach)),
                outward = facing,
                tilt = (Mathf.Lerp(CentreTilt, EdgeTilt, reach) + Noise.Signed(petal, settings.seed + 93) * TiltJitter) * expand
                       + WiltSlump * wilt * reach,
                length = settings.innerLength * Mathf.Lerp(CentreLength, 1.0f, reach) * shrink
                         * (1.0f + Noise.Signed(petal, settings.seed + 87) * 0.15f),
                width = settings.innerWidth * shrink * (1.0f + Noise.Signed(petal, settings.seed + 89) * 0.2f),
                taper = 0.0f,
                curl = (InnerCurl + Noise.Signed(petal, settings.seed + 95) * CurlJitter) * Mathf.Lerp(1.0f, 0.5f, wilt),
                cup = InnerCup,
                ruffle = InnerRuffle * expand,
                body = Fade(settings, palette, body, wilt),
                edge = Fade(settings, palette, Color.Lerp(palette.inner, palette.petalEdge, 0.5f), wilt)
            });
        }
    }

    private static void Build(MeshBuffer mesh, PeonySettings settings, Pose pose, PetalPlan plan)
    {
        Vector3 flank = Vector3.Cross(plan.outward, Vector3.up).normalized;
        float tilt = plan.tilt * Mathf.Deg2Rad;
        Vector3 heading = Vector3.up * Mathf.Cos(tilt) + plan.outward * Mathf.Sin(tilt);
        float taper = plan.taper;

        Blade.Build(mesh, new BladePlan
        {
            root = pose.Place(plan.root),
            heading = pose.Aim(heading),
            flank = pose.Aim(flank),
            length = plan.length * pose.scale,
            width = plan.width * pose.scale,
            outline = along => Blade.Petal(along) * (1.0f - taper * along),
            curl = plan.curl,
            cup = plan.cup,
            ruffle = plan.ruffle,
            fill = plan.body,
            rib = plan.body,
            tip = plan.edge,
            ink = Outline.Contour
        }, settings.depthBias);
    }
}
