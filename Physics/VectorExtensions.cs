using OpenTK.Mathematics;

namespace Physics;

public static class VectorExtensions
{
	public static Vector3 Along(this Vector3 a, Vector3 b, out Vector3 orthogonal)
	{
		var unit = b.Normalized();
		var along = Vector3.Dot(a, unit) * unit;
		orthogonal = (a - along);
		return along;
	}
}
