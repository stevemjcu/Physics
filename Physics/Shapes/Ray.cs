using OpenTK.Mathematics;

namespace Physics.Shapes;

// P = O + tD
public record struct Ray
{
    public Ray(Vector3 origin, Vector3 direction)
    {
        (Origin, Direction) = (origin, direction);
    }

    public Vector3 Origin { get; set; }

    public Vector3 Direction
    {
        get;
        set => field = value.Normalized();
    }

    public readonly Vector3 GetPoint(float distance)
    {
        return Origin + distance * Direction;
    }

    public readonly bool Overlaps(Sphere sphere, out float distance)
    {
        // |O + tD - C|^2 = R^2
        // => t^2 + 2D(O - C)t + |O - C|^2 - R^2 = 0
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
        // O + tD = (u - v - 1)A + uB + vC
        // => O - A = -Dt + (B - A)u + (C - A)v
        // Creates system of linear equations with unknowns t, u, and v
        // Solved with Cramer's Rule for 0 or 1 solutions

        // (a) Check if ray is not parallel with plane
        // (b) Check if ray-plane intersection lies within triangle
        // (c) Check if ray-plane intersection is not behind ray

        distance = 0;
        const float epsilon0 = 0.00005f;
        const float epsilon1 = 0.005f;

        var edge0 = triangle.EdgeAb;
        var edge1 = triangle.EdgeAc;

        var pvec = Vector3.Cross(Direction, edge1);
        var det = Vector3.Dot(edge0, pvec);

        if (det > -epsilon0 && det < epsilon0)
        {
            return false;
        }

        var idet = 1 / det;
        var tvec = Origin - triangle.A;
        var u = Vector3.Dot(tvec, pvec) * idet;

        if (u < 0 || u > 1)
        {
            return false;
        }

        var qvec = Vector3.Cross(tvec, edge0);
        var v = Vector3.Dot(Direction, qvec) * idet;

        if (v < 0 || u + v > 1)
        {
            return false;
        }

        distance = Vector3.Dot(edge1, qvec) * idet;
        return distance > -epsilon1;
    }
}
