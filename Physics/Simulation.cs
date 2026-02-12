using OpenTK.Mathematics;
using Physics.Constraints;
using Physics.Shapes;

namespace Physics;

public class Simulation
{
    public int Iterations { get; set; } = 20;

    public float DampingCoefficient { get; set; } = 1;

    public float FrictionCoefficient { get; set; } = 1;

    public float Gravity { get; set; } = 10;

    public List<Particle> Particles { get; set; } = [];

    public List<Constraint> Constraints { get; set; } = [];

    public List<Collider> Colliders { get; set; } = [];

    public List<CollisionConstraint> CollisionConstraints { get; set; } = [];

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
        foreach (var it in Particles)
        {
            foreach (var jt in Colliders)
            {
                var ray = new Ray(it.PreviousPosition, it.Position);
                var length = (it.Position - it.PreviousPosition).Length;

                if (ray.Overlaps(jt.Triangle, out var t) && t <= length)
                {
                    CollisionConstraints.Add(new(it, ray.GetPoint(t), jt.Triangle.Normal, 1));
                }
            }
        }

        // Solve constraints
        for (var i = 0; i < Iterations; i++)
        {
            foreach (var it in Constraints)
            {
                it.Project();
            }

            foreach (var it in CollisionConstraints)
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

        foreach (var it in CollisionConstraints)
        {
            var a = Vector3.Dot(it.Particles[0].Velocity, it.Normal) * it.Normal; // along normal
            var b = (it.Particles[0].Velocity - a) * FrictionCoefficient; // tangential to normal

            it.Particles[0].Velocity = a + b;
        }

        CollisionConstraints.Clear();
    }
}
