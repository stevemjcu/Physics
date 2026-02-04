using OpenTK.Mathematics;

namespace Physics;

public class Simulation
{
    public required int Iterations { get; set; } = 20;

    public required float DampingCoefficient { get; set; } = 1;

    public required float Gravity { get; set; }

    public List<Particle> Particles { get; set; } = [];

    public List<Constraint> Constraints { get; set; } = [];

    public void Step(float timestep)
    {
        // Integrate particles
        foreach (var it in Particles)
        {
            if (it.HasGravity)
            {
                it.Velocity += new Vector3(0, -Gravity, 0) * timestep;
            }

            it.PreviousPosition = it.Position;
            it.Position += it.Velocity * timestep;
        }

        // Generate collision constraints

        // Solve constraints
        for (var i = 0; i < Iterations; i++)
        {
            foreach (var it in Constraints)
            {
                it.Project();
            }
        }

        // Derive velocities
        foreach (var it in Particles)
        {
            it.Velocity = (it.Position - it.PreviousPosition) / timestep;
            it.Velocity *= DampingCoefficient;
        }
    }
}
