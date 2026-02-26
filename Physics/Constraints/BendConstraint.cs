using Physics.Colliders;

namespace Physics.Constraints;

public class BendConstraint(TriangleCollider source, TriangleCollider target, float distance, float compliance)
    : Constraint([.. source.Particles, target.Particles[^1]], compliance)
{
    public float Distance { get; set; } = distance;

    public override void CalculateError()
    {

    }
}
