using OpenTK.Mathematics;
using Physics.Colliders;
using Physics.Constraints;
using Physics.Shapes;

namespace Physics;

public class Simulation
{
    public int Substeps { get; set; } = 30;

    public int Iterations { get; set; } = 1;

    public float Damping { get; set; } = 0.99995f;

    public float Friction { get; set; } = 0.95f;

    public float Restitution { get; set; } = 1;

    public float Gravity { get; set; } = 10;

    public List<Particle> Particles { get; set; } = [];

    public List<Constraint> Constraints { get; set; } = [];

    public List<TriangleCollider> Colliders { get; set; } = [];

    public List<CollisionConstraint> CollisionConstraints { get; set; } = [];

    public void Step(float timestep)
    {
        timestep /= Substeps;

        for (var i = 0; i < Substeps; i++)
        {
            // Integrate particles

            foreach (var it in Particles)
            {
                if (it.HasGravity)
                {
                    it.Velocity += Vector3.Zero with { Y = -Gravity } * timestep;
                }

                it.PreviousPosition = it.Position;
                it.Position += it.Velocity * timestep;
            }

            // Detect collisions

            foreach (var it in Particles)
            {
                foreach (var jt in Colliders)
                {
                    var ray = new Ray(it.PreviousPosition, it.Displacement);
                    var length = it.Displacement.Length;

                    if (ray.Overlaps(jt.Triangle, out var t) && t <= length)
                    {
                        // TODO: Use pool to avoid reallocation
                        CollisionConstraints.Add(new(it, ray.GetPoint(t), jt.Triangle.Normal, 0));
                    }
                }
            }

            // Solve constraints

            for (var j = 0; j < Iterations; j++)
            {
                foreach (var it in Constraints)
                {
                    it.Project(timestep);
                }

                foreach (var it in CollisionConstraints)
                {
                    it.Project(timestep);
                }
            }

            // Derive velocities

            foreach (var it in Particles)
            {
                it.Velocity = (it.Displacement / timestep) * Damping;
            }

            foreach (var it in CollisionConstraints)
            {
                var a = it.Particles[0].Velocity.Along(it.Normal, out var b);

                if (Vector3.Dot(a, it.Normal) < 0)
                {
                    a *= -Restitution;
                }

                it.Particles[0].Velocity = a + b * Friction;
            }

            CollisionConstraints.Clear();
        }
    }
}
