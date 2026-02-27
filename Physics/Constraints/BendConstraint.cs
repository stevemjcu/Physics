using OpenTK.Mathematics;
using Physics.Shapes;

namespace Physics.Constraints;

/// <summary>
/// A constraint on the angle between two adjacent triangles ACB and ABD with the shared edge AB.
/// </summary>
public class BendConstraint(Particle a, Particle b, Particle c, Particle d, float angle, float compliance)
    : Constraint([a, b, c, d], compliance)
{
    public float Angle { get; set; } = angle;

    public override void CalculateError()
    {
        var source = new Triangle(a.Position, c.Position, b.Position);
        var target = new Triangle(a.Position, b.Position, d.Position);
        var product = Vector3.Dot(source.Normal, target.Normal);

        Error = MathF.Acos(product) - Angle;
        Gradient[0] = default;
        Gradient[1] = default;
        Gradient[2] = default;
        Gradient[3] = default;
    }
}
