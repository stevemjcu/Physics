using OpenTK.Mathematics;

namespace Physics.Utility;

public static class VectorExtensions
{
    public static Vector3 Along(this Vector3 source, Vector3 target, out Vector3 orthogonal)
    {
        var unit = target.Normalized();
        var along = Vector3.Dot(source, unit) * unit;
        orthogonal = (source - along);
        return along;
    }
}
