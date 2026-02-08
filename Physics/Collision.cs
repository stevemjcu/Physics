using OpenTK.Mathematics;

namespace Physics;

public static class Collision
{
    public static Vector3 GetDirection(Vector3 rotation)
    {
        var rotationX = Matrix3.CreateRotationX(rotation.X);
        var rotationY = Matrix3.CreateRotationY(rotation.Y);
        var rotationZ = Matrix3.CreateRotationZ(rotation.Z);

        return (-Vector3.UnitZ * rotationX * rotationY * rotationZ).Normalized();
    }

    public static bool Overlaps(Ray ray, Sphere sphere, out float distance)
    {
        var a = 1;
        var b = Vector3.Dot(2 * ray.Direction, ray.Origin - sphere.Origin);
        var c = (ray.Origin - sphere.Origin).LengthSquared - sphere.Radius * sphere.Radius;
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

// P = O + tD
public record struct Ray(Vector3 Origin, Vector3 Direction)
{
    public readonly Vector3 GetPoint(float distance)
    {
        return Origin + distance * Direction;
    }
}

// |P - O|^2 - R^2 = 0
public record struct Sphere(Vector3 Origin, float Radius)
{
    public readonly bool Intersects(Vector3 Point)
    {
        return (Point - Origin).LengthSquared - Radius * Radius == 0;
    }
}
