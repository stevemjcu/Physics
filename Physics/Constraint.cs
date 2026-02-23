using OpenTK.Mathematics;

namespace Physics;

public abstract class Constraint(Particle[] particles, float compliance, bool inequality = false)
{
    public Particle[] Particles { get; set; } = [.. particles];

    public float Compliance { get; set; } = compliance;

    public bool Inequality { get; set; } = inequality;

    public float Error { get; protected set; }

    public Vector3[] Gradient { get; protected set; } = new Vector3[particles.Length];

    public void Project(float timestep)
    {
        CalculateError();

        if (Inequality && Error < 0)
        {
            return;
        }

        var denominator = 0f;

        for (var i = 0; i < Particles.Length; i++)
        {
            denominator += Particles[i].InverseMass * Gradient[i].LengthSquared;
        }

        var x = Compliance / (timestep * timestep);
        var factor = -Error / (denominator + x);

        if (float.IsInfinity(factor) || float.IsNaN(factor))
        {
            return;
        }

        for (var i = 0; i < Particles.Length; i++)
        {
            var correction = factor * Particles[i].InverseMass * Gradient[i];
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
