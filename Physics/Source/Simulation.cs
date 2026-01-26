using System.Numerics;

namespace Physics;

public class Simulation
{
    public int Iterations { get; init; }

    public List<Particle> Particles { get; set; } = [];

    public List<Constraint> Constraints { get; set; } = [];

    public void Step(TimeSpan timestep)
    {
        var seconds = (float)timestep.TotalSeconds;

        // 1. Integrate positions
        foreach (var it in Particles)
        {
            it.PreviousPosition = it.Position;
            it.Position += it.Velocity * seconds;
        }

        // 2. Detect collisions
        // TODO

        // 3. Solve constraints
        for (var i = 0; i < Iterations; i++)
        {
            foreach (var it in Constraints)
            {
                it.Project();
            }
        }

        // 4. Derive velocities
        foreach (var it in Particles)
        {
            it.Velocity = (it.Position - it.PreviousPosition) / seconds;
        }
    }
}

public class Particle
{
    public Vector3 Position { get; set; }

    public Vector3 PreviousPosition { get; set; }

    public Vector3 Velocity { get; set; }

    public float Mass { get; set; }
}

public abstract class Constraint(List<Particle> particles, float stiffness)
{
    public List<Particle> Particles { get; set; } = particles;

    public float Stiffness { get; set; } = stiffness;

    public abstract float GetError();

    public abstract List<Vector3> GetGradient();

    public float GetScalingFactor()
    {
        var gradient = GetGradient();
        var sum = 0f;

        for (var i = 0; i < Particles.Count; i++)
        {
            sum += (1 / Particles[i].Mass) * gradient[i].LengthSquared();
        }

        return -GetError() / sum;
    }

    public void Project()
    {
        var gradient = GetGradient();
        var factor = GetScalingFactor();

        for (var i = 0; i < Particles.Count; i++)
        {
            var coefficient = Stiffness * factor * (1 / Particles[i].Mass);
            Particles[i].Position += coefficient * gradient[i];
        }
    }
}
