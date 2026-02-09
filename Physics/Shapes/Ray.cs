using OpenTK.Mathematics;

namespace Physics.Shapes;

// P = O + tD
public record struct Ray(Vector3 Origin, Vector3 Direction)
{
    public readonly Vector3 GetPoint(float distance)
    {
        return Origin + distance * Direction;
    }

    public readonly bool Overlaps(Sphere sphere, out float distance)
    {
        // Ray: P = O + tD, Sphere: |P - C|^2 = R^2
        // Creates system of linear equations with unknown t
        // Solved with quadratic formula for 0, 1, or 2 solutions

        var a = 1;
        var b = Vector3.Dot(2 * Direction, Origin - sphere.Center);
        var c = (Origin - sphere.Center).LengthSquared - sphere.Radius * sphere.Radius;
        var d = b * b - 4 * a * c;

        if (d < 0)
        {
            distance = 0;
            return false;
        }

        if (d == 0)
        {
            distance = -b / 2 * a;
            return distance > 0;
        }

        d = MathF.Sqrt(d);
        var t0 = (-b + d) / 2 * a;
        var t1 = (-b - d) / 2 * a;

        distance = MathF.Max(t0, t1);
        return distance > 0;
    }

    public readonly bool Overlaps(Triangle triangle, out float distance)
    {
        // Ray: P = O + tD, Triangle: P = uA + vB + wC
        // Creates system of linear equations with unknowns t, u, and v
        // Solved with Cramer's Rule for 0 or 1 solutions

        distance = default;
        return default;
    }
}
