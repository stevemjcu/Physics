using OpenTK.Mathematics;

namespace Physics;

public abstract class Constraint(Particle[] particles, float compliance, int equality = 0)
{
    /// <summary>
    /// The particles participating in the constraint.
    /// </summary>
    public Particle[] Particles { get; set; } = [.. particles];

    /// <summary>
    /// The inverse stiffness of the constraint.
    /// </summary>
    public float Compliance { get; set; } = compliance;

    /// <summary>
    /// Introduces energy loss to reduce oscillations.
    /// </summary>
    public float Damping { get; set; } = 0;

    /// <summary>
    /// Indicates how the constraint is satisified - positive, negative, or zero error.
    /// </summary>
    public int Equality { get; set; } = equality;

    /// <summary>
    /// The current error shared by all particles.
    /// </summary>
    public float Error { get; protected set; }

    /// <summary>
    /// The current direction each particle should move to least satisfy the constraint.
    /// </summary>
    protected Vector3[] Gradient { get; set; } = new Vector3[particles.Length];

    public void Project(float timestep)
    {
        CalculateError();

        if (Equality * Error > 0)
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
