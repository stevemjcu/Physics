using OpenTK.Mathematics;

namespace Physics.Constraints;

public class DistanceConstraint(
    Particle source, Particle target, float distance, float stiffness)
    : Constraint([source, target], stiffness)
{
    public float Distance { get; set; } = distance;

    protected override (float Error, Vector3[] Gradient) CalculateError()
    {
        var displacement = Particles[0].Position - Particles[1].Position;
        var distance = displacement.Length;

        var error = distance - Distance;
        Gradient[0] = displacement / distance; // normalized
        Gradient[1] = -Gradient[0];

        return (error, Gradient);
    }
}
