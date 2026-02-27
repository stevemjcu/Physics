using OpenTK.Mathematics;
using Physics.Colliders;
using Physics.Constraints;
using Physics.Shapes;

namespace Physics;

public class Simulation
{
    public int Substeps { get; set; } = 30;

    public float Damping { get; set; } = 0.99995f;

    public float Friction { get; set; } = 0.95f;

    public float Restitution { get; set; } = 1;

    public float Gravity { get; set; } = 10;

    public List<Particle> Particles { get; set; } = [];

    public List<Constraint> Constraints { get; set; } = [];

    public List<TriangleCollider> Colliders { get; set; } = [];

    public List<CollisionConstraint> CollisionConstraints { get; set; } = [];

    public void Reset()
    {
        Particles = [];
        Constraints = [];
        Colliders = [];
        CollisionConstraints = [];
    }

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

            // Solve constraints

            foreach (var it in Constraints)
            {
                it.Project(timestep);
            }

            // Solve collision constraints

            foreach (var it in Particles)
            {
                foreach (var jt in Colliders)
                {
                    var displacement = it.Position - it.PreviousPosition;
                    var ray = new Ray(it.PreviousPosition, displacement);
                    var length = displacement.Length;

                    if (ray.Overlaps(jt.Triangle, out var t) && t <= length)
                    {
                        // TODO: Use pool to avoid reallocation
                        CollisionConstraints.Add(new(it, ray.GetPoint(t), jt.Triangle.Normal, 0));
                    }
                }
            }

            foreach (var it in CollisionConstraints)
            {
                it.Project(timestep);
            }

            // Derive velocities

            foreach (var it in Particles)
            {
                it.Velocity = (it.Position - it.PreviousPosition) / timestep * Damping;
            }

            foreach (var it in Constraints)
            {
                it.Dampen(timestep);
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
