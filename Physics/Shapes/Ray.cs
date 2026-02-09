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
        // O + tD = wA + uB + vC
        // => O - A = -Dt + (B - A)u + (C - A)v
        // Creates system of linear equations with unknowns t, u, and v
        // Solved with Cramer's Rule for 0 or 1 solutions

        // Ax = b
        // x_i = det(A_i) / det(A) where A_i is A with ith column replaced by b

        // Copied from the following article (for now):
        // https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/moller-trumbore-ray-triangle-intersection.html

        // (a) Check if ray is behind or parallel with plane
        // (b) Check if ray-plane intersection lies within triangle

        distance = 0;
        var pvec = Vector3.Cross(Direction, triangle.EdgeAc);
        var det = Vector3.Dot(triangle.EdgeAb, pvec);
        if (det > float.Epsilon)
        {
            return false;
        }

        var invdet = 1 / det;
        var tvec = Origin - triangle.A;
        var u = Vector3.Dot(tvec, pvec) * invdet;
        if (u < 0 || u > 1)
        {
            return false;
        }

        var qvec = Vector3.Cross(tvec, triangle.EdgeAb);
        var v = Vector3.Dot(Direction, qvec) * invdet;
        if (v < 0 || u + v > 1)
        {
            return false;
        }

        distance = Vector3.Dot(triangle.EdgeAc, qvec) * invdet;
        return true;
    }
}
