using UnityEngine;

public static class Noise
{
    public static float Hash(int index, int seed)
    {
        uint hashed = (uint)(index * 73856093) ^ (uint)(seed * 19349663);
        hashed ^= hashed >> 13;
        hashed *= 0x85EBCA6B;
        hashed ^= hashed >> 16;
        return (hashed & 0xFFFFFF) / (float)0xFFFFFF;
    }

    public static Color Vary(Color colour, int index, int seed, float amount)
    {
        float lift = 1.0f + Signed(index, seed) * amount;
        float warm = Signed(index, seed + 1) * amount * 0.5f;
        return new Color(colour.r * (lift + warm), colour.g * lift, colour.b * (lift - warm), 1.0f);
    }

    public static float Signed(int index, int seed)
    {
        return Hash(index, seed) * 2.0f - 1.0f;
    }
}
