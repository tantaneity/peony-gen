public static class PeonyGeometry
{
    public static Pose Build(MeshBuffer mesh, PeonySettings settings, PeonyPalette palette)
    {
        mesh.SetInk(palette.ink);
        StemShape stem = Stem.Trace(settings);
        Pose head = Stem.Build(mesh, settings, palette, stem);
        Leaves.Build(mesh, settings, palette, stem);
        Bloom.Build(mesh, settings, palette, head);
        return head;
    }
}
