using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public sealed class PeonyBuilder : MonoBehaviour
{
    public PeonySettings settings = new PeonySettings();

    private Mesh mesh;

    private void OnEnable()
    {
        Rebuild();
    }

    private void OnValidate()
    {
        Rebuild();
    }

    public Pose Head { get; private set; }

    public void Rebuild()
    {
        if (mesh == null)
        {
            mesh = new Mesh { name = "Peony", hideFlags = HideFlags.DontSave };
        }

        MeshBuffer buffer = new MeshBuffer();
        Head = PeonyGeometry.Build(buffer, settings, PeonyPalette.Blush);
        buffer.WriteTo(mesh);
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
