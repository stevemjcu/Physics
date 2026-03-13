using OpenTK.Mathematics;
using Physics.Constraints;

namespace Physics.Demo;

internal class Model
{
    public List<Vector3> Vertices { get; set; } = [];

    public List<int[]> Faces { get; set; } = [];

    public static Model Import(string path)
    {
        var model = new Model();

        foreach (var it in File.ReadAllLines(path))
        {
            var line = it.Split(' ');

            switch (line[0])
            {
                case "v":
                    var l = line.Skip(1).Select(float.Parse).ToArray();
                    model.Vertices.Add(new Vector3(l[0], l[1], l[2]));
                    break;
                case "f":
                    model.Faces.Add([.. line.Skip(1).Select(s => int.Parse(s) - 1)]);
                    break;
                default:
                    break;
            }
        }

        return model;
    }

    public void Load(Simulation simulation, float mass, float compliance, float damping, Matrix4 transform)
    {
        var particles = new List<Particle>();
        mass /= Vertices.Count;

        foreach (var it in Vertices)
        {
            var position = (new Vector4(it, 1) * transform).Xyz;
            particles.Add(new(position, Vector3.Zero, mass, true));
            simulation.Particles.Add(particles[^1]);
        }

        foreach (var it in Faces)
        {
            if (it.Length == 3)
            {
                AddTri(it);
            }
            else
            {
                AddQuad(it);
            }
        }

        for (var i = 0; i < Faces.Count; i++)
        {
            for (var j = i + 1; j < Faces.Count; j++)
            {
                var it = Faces[i];
                var jt = Faces[j];

                if (it.Length != 3 || jt.Length != 3)
                {
                    continue;
                }

                var indices = (List<int>)[.. it, .. jt];
                var set = indices.Distinct().ToList();

                if (set.Count == 4)
                {
                    var cd = set.Where(it => indices.Count(jt => jt == it) == 1).ToList();
                    var ab = set.Where(it => !cd.Contains(it)).ToList();

                    var a = particles[ab[0]];
                    var b = particles[ab[1]];
                    var c = particles[cd[0]];
                    var d = particles[cd[1]];

                    var distance = (c.Position - d.Position).Length;
                    var constraint = new DistanceConstraint(c, d, distance, compliance) { DebugDraw = false };
                    simulation.Constraints.Add(constraint);
                }
            }
        }

        void AddQuad(int[] face)
        {
            var a = particles[face[0]];
            var b = particles[face[1]];
            var c = particles[face[2]];
            var d = particles[face[3]];

            simulation.Constraints.Add(new DistanceConstraint(a, b, compliance) { Damping = damping });
            simulation.Constraints.Add(new DistanceConstraint(b, c, compliance) { Damping = damping });
            simulation.Constraints.Add(new DistanceConstraint(c, d, compliance) { Damping = damping });
            simulation.Constraints.Add(new DistanceConstraint(d, a, compliance) { Damping = damping });
            simulation.Constraints.Add(new DistanceConstraint(a, c, compliance) { Damping = damping });
            simulation.Constraints.Add(new DistanceConstraint(b, d, compliance) { Damping = damping });
        }

        void AddTri(int[] face)
        {
            var a = particles[face[0]];
            var b = particles[face[1]];
            var c = particles[face[2]];

            simulation.Constraints.Add(new DistanceConstraint(a, b, compliance) { Damping = damping });
            simulation.Constraints.Add(new DistanceConstraint(b, c, compliance) { Damping = damping });
            simulation.Constraints.Add(new DistanceConstraint(c, a, compliance) { Damping = damping });

            //var u = particles[it[0]];
            //var v = particles[it[1]];
            //var w = particles[it[2]];
            //simulation.Colliders.Add(new TriangleCollider(u, v, w));
        }
    }
}
