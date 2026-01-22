using System.Numerics;

namespace Physics;

public class Simulation
{
    public readonly List<Particle> Particles = [];
    public readonly List<Constraint> Constraints = [];
    public readonly int Iterations;

    public void Step(TimeSpan timestep)
    {
        var seconds = (float)timestep.TotalSeconds;

        // 1. Integrate positions
        foreach (var it in Particles)
        {
            it.PrevPosition = it.Position;
            it.Position += it.Velocity * seconds;
        }

        // 2. Detect collisions
        // TODO

        // 3. Solve constraints
        for (var i = 0; i < Iterations; i++)
        {
            foreach (var it in Constraints)
            {
                // TODO
            }
        }

        // 4. Derive velocities
        foreach (var it in Particles)
        {
            it.Velocity = (it.Position - it.PrevPosition) / seconds;
        }
    }
}

public class Particle
{
    public Vector3 Position;
    public Vector3 PrevPosition;
    public Vector3 Velocity;
    public float InverseMass;
}

public class Constraint
{
    public required List<Particle> Particles;
    public required Func<List<Particle>, float> Error;
    public float Stiffness;
}
