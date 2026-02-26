using OpenTK.Mathematics;

namespace Physics;

public abstract class Constraint(Particle[] particles, float compliance, bool inequality = false)
{
    public Particle[] Particles { get; set; } = [.. particles];

    public float Compliance { get; set; } = compliance;

    public float Damping { get; set; } = 0;

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

        var denominator = Compliance / (timestep * timestep);

        for (var i = 0; i < Particles.Length; i++)
        {
            denominator += Particles[i].InverseMass * Gradient[i].LengthSquared;
        }

        var scalingFactor = -Error / denominator;

        for (var i = 0; i < Particles.Length; i++)
        {
            var correction = scalingFactor * Particles[i].InverseMass * Gradient[i];
            Particles[i].Position += correction;
        }
    }

    public void Dampen(float timestep)
    {
        var average = Vector3.Zero;

        foreach (var it in Particles)
        {
            average += it.Velocity;
        }

        average /= Particles.Length;

        foreach (var it in Particles)
        {
            var correction = (average - it.Velocity) * timestep * Damping;
            it.Velocity += correction;
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
