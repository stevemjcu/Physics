using OpenTK.Mathematics;

namespace Physics.Constraints;

public class ShapeConstraint(Particle[] particles, float compliance)
    : Constraint(particles, compliance)
{
    public Vector3[] Target { get; set; } = [];

    public Vector3 Origin { get; set; }

    public override void CalculateError()
    {
        // The target has an arbitrary position and orientation
        // Need to align target configuration with source configuration
        // Then can determine error by average displacement length
        // And gradient by direction of displacement

        // To align target:
        // (a) Move target by source's average displacement from target
        // (b) Rotate target by source's average rotation from target

        var displacement = AverageDisplacement();
        Origin += displacement;

        for (var i = 0; i < Particles.Length; i++)
        {
            Target[i] += displacement;
        }

        var rotation = AverageRotation();

        for (var i = 0; i < Particles.Length; i++)
        {
            // TODO: Rotate target by angle about origin
            Target[i] = Target[i];
        }
    }

    private Vector3 AverageDisplacement()
    {
        var sum = Vector3.Zero;

        foreach (var it in Particles)
        {
            sum += it.Position;
        }

        return sum / Particles.Length - Origin;
    }

    private Vector3 AverageRotation()
    {
        return default;
    }
}
