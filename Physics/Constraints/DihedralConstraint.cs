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

    public override void CalculateError()
    {
        var source = new Triangle(a.Position, c.Position, b.Position);
        var target = new Triangle(a.Position, b.Position, d.Position);
        var angle = Vector3.CalculateAngle(source.Normal, target.Normal);

        // C and D should move along CD (opposite directions)
        // A and B should move perpendicular to CD and AB (same direction)

        var ab = b.Position - a.Position;
        var cd = d.Position - c.Position;

        Error = angle - Angle;
        Gradient[0] = Vector3.Cross(ab, cd);
        Gradient[1] = Gradient[0];
        Gradient[2] = cd;
        Gradient[3] = -Gradient[2];
    }
}
