namespace Physics.Constraints;

public class DistanceConstraint(
    Particle source, Particle target, float distance, float stiffness)
    : Constraint([source, target], stiffness)
{
    public float Distance { get; set; } = distance;

    public override void Calculate()
    {
        var displacement = Particles[1].Position - Particles[0].Position;
        var distance = displacement.Length;

        Error = distance - Distance;
        Gradient[0] = displacement / distance; // normalized
        Gradient[1] = -Gradient[0];
    }
}
