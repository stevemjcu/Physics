namespace Physics;

public class Simulation(int iterations)
{
    public int Iterations { get; set; } = iterations;

    public List<Particle> Particles { get; set; } = [];

    public List<Constraint> Constraints { get; set; } = [];

    public List<Collider> Colliders { get; set; } = [];

    public void Step(TimeSpan timestep)
    {
        var seconds = (float)timestep.TotalSeconds;

        // Integrate positions
        foreach (var it in Particles)
        {
            it.PreviousPosition = it.Position;
            it.Position += it.Velocity * seconds;
        }

        // Detect potential collisions (broad phase)
        // TODO

        // Solve constraints
        for (var i = 0; i < Iterations; i++)
        {
            // Generate collision constraints (narrow phase)
            // TODO

            // Project constraints
            foreach (var it in Constraints)
            {
                it.Calculate();
                it.Project();
            }
        }

        // Derive velocities
        foreach (var it in Particles)
        {
            it.Velocity = (it.Position - it.PreviousPosition) / seconds;
        }
    }
}
