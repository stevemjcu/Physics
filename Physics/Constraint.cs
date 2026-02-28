using OpenTK.Mathematics;

namespace Physics;

/// <summary>
/// Defines a constraint for a set of participating particles.
/// </summary>
/// <param name="particles">The participating particles.</param>
/// <param name="compliance">The inverse stiffness.</param>
/// <param name="equality">The type.</param>
public abstract class Constraint(Particle[] particles, float compliance, int equality = 0)
{
    /// <summary>
    /// The particles participating in the constraint.
    /// </summary>
    public Particle[] Particles { get; set; } = [.. particles];

    /// <summary>
    /// The inverse stiffness of the constraint.
    /// </summary>
    /// <remarks>0 is infinitely stiff.</remarks>
    public float Compliance { get; set; } = compliance;

    /// <summary>
    /// The artificial energy loss of the constraint to reduce oscillations.
    /// </summary>
    /// <remarks>0 is undamped and 1 is critically damped.</remarks>
    public float Damping { get; set; } = 0;

    /// <summary>
    /// The type of the constraint, indicating how it is satisfied.
    /// </summary>
    /// <remarks>0 is equality and -1 or 1 is inequality.</remarks>
    public int Equality { get; set; } = equality;

    /// <summary>
    /// The current error shared by all particles.
    /// </summary>
    public float Error { get; protected set; }

    /// <summary>
    /// The current direction each particle can move to increase <see cref="Error"/> the most.
    /// </summary>
    protected Vector3[] Gradient { get; set; } = new Vector3[particles.Length];

    /// <summary>
    /// Updates <see cref="Error"/> and <see cref="Gradient"/> based on current particle positions and weights. 
    /// </summary>
    public abstract void RecalculateError();

    /// <summary>
    /// Corrects the position of each particle to better satisfy the constraint.
    /// </summary>
    /// <param name="timestep">The time elapsed since the last call.</param>
    public void Project(float timestep)
    {
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

    /// <summary>
    /// Corrects the velocity of each particle to better follow their overall motion.
    /// </summary>
    /// <param name="timestep">The time elapsed since the last call.</param>
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
}
