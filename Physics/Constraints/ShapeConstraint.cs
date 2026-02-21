namespace Physics.Constraints;

public class ShapeConstraint(Particle[] particles, float stiffness)
    : Constraint(particles, stiffness)
{
    public Particle[] Target { get; set; } = [.. particles];

    public override void CalculateError()
    {

    }
}
