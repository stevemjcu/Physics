using OpenTK.Mathematics;

namespace Physics;

public abstract class Constraint(Particle[] particles, float stiffness)
{
    public Particle[] Particles { get; set; } = [.. particles];

    protected Vector3[] Gradient { get; set; } = new Vector3[particles.Length];

    public float Stiffness { get; set; } = stiffness;

    public void Project()
    {
        var (error, gradient) = CalculateError();
        var factor = CalculateScalingFactor(error, gradient);

        // If weights are zero, factor is infinity
        // If error is zero, factor (and gradient) is NaN
        if (float.IsInfinity(factor) || float.IsNaN(factor))
        {
            return;
        }

        for (var i = 0; i < Particles.Length; i++)
        {
            var correction = factor * Stiffness * Particles[i].InverseMass * gradient[i];
            Particles[i].Position += correction;
        }
    }

    public abstract (float Error, Vector3[] Gradient) CalculateError();

    public float CalculateScalingFactor(float Error, Vector3[] Gradient)
    {
        var denominator = 0f;

        for (var i = 0; i < Particles.Length; i++)
        {
            denominator += Particles[i].InverseMass * Gradient[i].LengthSquared;
        }

        return -Error / denominator;
    }
}
