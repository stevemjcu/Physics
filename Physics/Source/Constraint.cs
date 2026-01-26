using System.Numerics;

namespace Physics;

public abstract class Constraint(List<Particle> particles, float stiffness)
{
    public List<Particle> Particles { get; set; } = particles;

    public float Stiffness { get; set; } = stiffness;

    public abstract float Error { get; }

    public abstract List<Vector3> Gradient { get; }

    public float ScalingFactor
    {
        get
        {
            var gradient = Gradient;
            var denominator = 0f;

            for (var i = 0; i < Particles.Count; i++)
            {
                var component = Particles[i].InverseMass * gradient[i].LengthSquared();
                denominator += component;
            }

            return -Error / denominator;
        }
    }

    public void Project()
    {
        var gradient = Gradient;
        var factor = Stiffness * ScalingFactor;

        for (var i = 0; i < Particles.Count; i++)
        {
            var correction = factor * Particles[i].InverseMass * gradient[i];
            Particles[i].Position += correction;
        }
    }
}
