using UnityEngine;

public static class Lifecycle
{
    private const float SproutHeight = 0.12f;
    private const float GrowUntil = 0.30f;
    private const float HeadFrom = 0.12f;
    private const float HeadUntil = 0.50f;
    private const float BudHead = 0.25f;
    private const float BlushFrom = 0.30f;
    private const float BlushUntil = 0.46f;
    private const float SepalFrom = 0.38f;
    private const float SepalUntil = 0.48f;
    private const float BloomFrom = 0.44f;
    private const float BloomUntil = 0.62f;
    private const float FluffFrom = 0.48f;
    private const float FluffUntil = 0.76f;
    private const float WiltFrom = 0.78f;
    private const float WiltUntil = 0.97f;
    private const float DryFrom = 0.84f;
    private const float YoungNod = 2.0f;
    private const float BloomNod = 10.0f;
    private const float WiltNod = 30.0f;

    private static float Ease(float from, float to, float life)
    {
        return Curve.Step(from, to, life);
    }

    public static void Apply(PeonySettings settings, float life)
    {
        settings.grow = Mathf.Lerp(SproutHeight, 1.0f, Ease(0.0f, GrowUntil, life));
        settings.headScale = Mathf.Lerp(BudHead, 1.0f, Ease(HeadFrom, HeadUntil, life));
        settings.blush = Ease(BlushFrom, BlushUntil, life);
        settings.sepalOpen = Ease(SepalFrom, SepalUntil, life);
        settings.bloom = Ease(BloomFrom, BloomUntil, life);
        settings.fluff = Ease(FluffFrom, FluffUntil, life);
        settings.wilt = Ease(WiltFrom, WiltUntil, life);
        settings.dry = Ease(DryFrom, 1.0f, life);
        settings.nod = Mathf.Lerp(YoungNod, BloomNod, Ease(GrowUntil, FluffUntil, life)) + WiltNod * Ease(WiltFrom, 1.0f, life);
    }
}
