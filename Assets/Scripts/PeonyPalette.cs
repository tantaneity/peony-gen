using UnityEngine;

public sealed class PeonyPalette
{
    public Color guard;
    public Color inner;
    public Color innerDeep;
    public Color petalEdge;
    public Color petalBud;
    public Color petalFade;
    public Color petalDry;
    public Color sepal;
    public Color hip;
    public Color stem;
    public Color leaf;
    public Color leafRib;
    public Color leafYoung;
    public Color leafDry;
    public Color ink;
    public Color background;

    public static readonly PeonyPalette Blush = new PeonyPalette
    {
        guard = Hex(0xf2b2c2),
        inner = Hex(0xec8ea8),
        innerDeep = Hex(0xd4688a),
        petalEdge = Hex(0xfbdde6),
        petalBud = Hex(0xa9bb74),
        petalFade = Hex(0xf6e4e8),
        petalDry = Hex(0xb89a86),
        sepal = Hex(0x4f7a32),
        hip = Hex(0x5f8a38),
        stem = Hex(0x5e8a3c),
        leaf = Hex(0x3e6e30),
        leafRib = Hex(0x6c9a50),
        leafYoung = Hex(0x7a4434),
        leafDry = Hex(0x8c7a3a),
        ink = Hex(0x2a2722),
        background = Hex(0xf6f3ec)
    };

    private static Color Hex(uint value)
    {
        float red = ((value >> 16) & 0xff) / 255.0f;
        float green = ((value >> 8) & 0xff) / 255.0f;
        float blue = (value & 0xff) / 255.0f;
        return new Color(red, green, blue, 1.0f);
    }
}
