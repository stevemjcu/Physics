using OpenTK.Mathematics;

namespace Physics.Constraints;

public class DistanceConstraint(
    Particle source, Particle target, float distance, float stiffness)
    : Constraint([source, target], stiffness)
{
    public float RestDistance { get; set; } = distance;

    protected override (float Error, Vector3[] Gradient) CalculateError()
    {
        var displacement = Particles[1].Position - Particles[0].Position;
        var distance = displacement.Length;

        // FIXME: Why is this flipped?
        var error = RestDistance - distance;
        Gradient[0] = displacement / distance; // normalized
        Gradient[1] = -Gradient[0];

        return (error, Gradient);
    }
}
