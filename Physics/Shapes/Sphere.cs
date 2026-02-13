using OpenTK.Mathematics;

namespace Physics.Shapes;

// |P - C|^2 - R^2 = 0
public record struct Sphere(Vector3 Center, float Radius)
{
	public readonly bool Intersects(Vector3 Point)
	{
		var value = (Point - Center).LengthSquared - Radius * Radius;
		return MathF.Abs(value) < float.Epsilon;
	}
}
