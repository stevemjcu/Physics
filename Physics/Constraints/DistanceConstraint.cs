namespace Physics.Constraints;

public class DistanceConstraint(Particle source, Particle target, float distance, float compliance)
    : Constraint([source, target], compliance)
{
    public float Distance { get; set; } = distance;

    public override void RecalculateError()
    {
        var displacement = Particles[0].Position - Particles[1].Position;
        var distance = displacement.Length;

        Error = distance - Distance;
        Gradient[0] = displacement / distance; // normalized
        Gradient[1] = -Gradient[0];
    }
}
