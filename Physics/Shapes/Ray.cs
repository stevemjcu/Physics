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
        var a = 1;
        var b = Vector3.Dot(2 * Direction, Origin - sphere.Origin);
        var c = (Origin - sphere.Origin).LengthSquared - sphere.Radius * sphere.Radius;
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
}
