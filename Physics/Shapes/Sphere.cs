using OpenTK.Mathematics;

namespace Physics.Shapes;

// |P - O|^2 - R^2 = 0
public record struct Sphere(Vector3 Origin, float Radius)
{
    public readonly bool Intersects(Vector3 Point)
    {
        return (Point - Origin).LengthSquared - Radius * Radius == 0;
    }
}
