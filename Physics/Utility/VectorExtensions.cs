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

    public static Vector3 ParseFloat(string[] components)
    {
        return new(
            float.Parse(components[0]),
            float.Parse(components[1]),
            float.Parse(components[2]));
    }

    public static Vector3i ParseInt(string[] components, int offset = 0)
    {
        return new(
            int.Parse(components[0]) + offset,
            int.Parse(components[1]) + offset,
            int.Parse(components[2]) + offset);
    }
}
