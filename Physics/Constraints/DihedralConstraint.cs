using OpenTK.Mathematics;
using Physics.Shapes;

namespace Physics.Constraints;

/// <summary>
/// A constraint on the angle between two adjacent triangles ACB and ABD with the shared edge AB.
/// </summary>
public class DihedralConstraint(Particle a, Particle b, Particle c, Particle d, float angle, float compliance)
    : Constraint([a, b, c, d], compliance)
{
    public float Angle { get; set; } = angle;

    public override void RecalculateError()
    {
        var source = new Triangle(a.Position, c.Position, b.Position);
        var target = new Triangle(a.Position, b.Position, d.Position);
        Error = Vector3.CalculateAngle(source.Normal, target.Normal) - Angle;

        // TODO: Set gradient
        Gradient[0] = Vector3.Zero;
        Gradient[1] = Vector3.Zero;
        Gradient[2] = Vector3.Zero;
        Gradient[3] = Vector3.Zero;
    }
}
