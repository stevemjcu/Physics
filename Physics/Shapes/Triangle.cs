using OpenTK.Mathematics;

namespace Physics.Shapes;

// P = (u - v - 1)A + uB + vC
public record struct Triangle(Vector3 A, Vector3 B, Vector3 C)
{
	public readonly Vector3 EdgeAb => B - A;

	public readonly Vector3 EdgeAc => C - A;

	public readonly Vector3 Normal => Vector3.Cross(EdgeAc, EdgeAb).Normalized();
}
