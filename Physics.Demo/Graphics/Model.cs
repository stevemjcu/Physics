using OpenTK.Mathematics;

namespace Physics.Demo.Graphics;

internal class Model
{
    public List<Vector3> Vertices { get; set; } = [];

    public List<Vector3i> Faces { get; set; } = [];

    public void Import(string path)
    {
        foreach (var it in File.ReadAllLines(path))
        {
            var line = it.Split(' ');

            switch (line[0])
            {
                case "v":
                    Vertices.Add(new(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3])));
                    break;
                case "f":
                    Faces.Add(new(int.Parse(line[1]), int.Parse(line[2]), int.Parse(line[3])));
                    break;
                default:
                    break;
            }
        }
    }
}
