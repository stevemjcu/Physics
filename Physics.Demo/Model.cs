using OpenTK.Mathematics;
using Physics.Constraints;

namespace Physics.Demo;

internal class Model
{
    public List<Vector3> Vertices { get; set; } = [];

    public List<Vector3i> Faces { get; set; } = [];

    public static Model Import(string path)
    {
        var model = new Model();

        foreach (var it in File.ReadAllLines(path))
        {
            var line = it.Split(' ');

            switch (line[0])
            {
                case "v":
                    model.Vertices.Add(VectorExtensions.ParseFloat(line[1..]));
                    break;
                case "f":
                    model.Faces.Add(VectorExtensions.ParseInt(line[1..], -1));
                    break;
                default:
                    break;
            }
        }

        return model;
    }

    public void Load(Simulation simulation, Particle template, float stiffness, Matrix4 transform)
    {
        var particles = new List<Particle>();

        foreach (var it in Vertices)
        {
            // FIXME: Transformations
            var position = (new Vector4(it) * transform).Xyz;
            particles.Add(new(position, Vector3.Zero, template.Mass, template.HasGravity));
            simulation.Particles.Add(particles[^1]);
        }

        foreach (var it in Faces)
        {
            for (var i = 0; i < 3; i++)
            {
                var (a, b) = (particles[it[i]], particles[it[(i + 1) % 3]]);
                var distance = (b.Position - a.Position).Length;
                simulation.Constraints.Add(new DistanceConstraint(a, b, distance, stiffness));
            }

            //var u = particles[it[0]];
            //var v = particles[it[1]];
            //var w = particles[it[2]];
            //simulation.Colliders.Add(new TriangleCollider(u, v, w));
        }
    }
}
