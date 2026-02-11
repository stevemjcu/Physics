using OpenTK.Mathematics;

namespace Physics;

public abstract class Constraint(Particle[] particles, float stiffness, bool equality = true)
{
    public Particle[] Particles { get; set; } = [.. particles];

    protected Vector3[] Gradient { get; set; } = new Vector3[particles.Length];

    public float Stiffness { get; set; } = stiffness;

    public bool Equality { get; set; } = equality;

    public void Project()
    {
        var (error, gradient) = CalculateError();
        if (!Equality && error < 0)
        {
            return;
        }

        var factor = CalculateScalingFactor(error, gradient);
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
