using OpenTK.Mathematics;

namespace Physics;

public abstract class Constraint(List<Particle> particles, float stiffness)
{
    public List<Particle> Particles { get; set; } = particles;

    public float Stiffness { get; set; } = stiffness;

    public List<Vector3> Gradient { get; protected set; } = new(particles.Count);

    public float Error { get; protected set; }

    public float ScalingFactor
    {
        get
        {
            var denominator = 0f;

            for (var i = 0; i < Particles.Count; i++)
            {
                denominator += Particles[i].InverseMass * Gradient[i].LengthSquared;
            }

            return -Error / denominator;
        }
    }

    public abstract void Calculate();

    public void Project()
    {
        var factor = Stiffness * ScalingFactor;

        for (var i = 0; i < Particles.Count; i++)
        {
            Particles[i].Position += factor * Particles[i].InverseMass * Gradient[i];
        }
    }
}
