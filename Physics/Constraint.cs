using OpenTK.Mathematics;

namespace Physics;

public abstract class Constraint(Particle[] particles, float stiffness, bool inequality = false)
{
    public Particle[] Particles { get; set; } = [.. particles];

    public float Stiffness { get; set; } = stiffness;

    public bool Inequality { get; set; } = inequality;

    public float Error { get; protected set; }

    public Vector3[] Gradient { get; protected set; } = new Vector3[particles.Length];

    public float ScalingFactor
    {
        get
        {
            var denominator = 0f;

            for (var i = 0; i < Particles.Length; i++)
            {
                denominator += Particles[i].InverseMass * Gradient[i].LengthSquared;
            }

            return -Error / denominator;
        }
    }

    public void Project()
    {
        CalculateError();

        if (Inequality && Error < 0)
        {
            return;
        }

        var factor = ScalingFactor;

        if (float.IsInfinity(factor) || float.IsNaN(factor))
        {
            return;
        }

        for (var i = 0; i < Particles.Length; i++)
        {
            var correction = factor * Stiffness * Particles[i].InverseMass * Gradient[i];
            Particles[i].Position += correction;
        }
    }

    /// <summary>
    /// Updates <see cref="Error"/> and <see cref="Gradient"/> based on current particle positions. 
    /// </summary>
    /// <remarks>
    /// Called automatically by <see cref="Project"/>.
    /// </remarks>
    public abstract void CalculateError();
}
