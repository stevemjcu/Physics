using OpenTK.Mathematics;

namespace Physics;

public class Particle
{
    public Vector3 Position { get; set; }

    public Vector3 PreviousPosition { get; set; }

    public Vector3 Velocity { get; set; }

    public float InverseMass { get; set; }

    public float Mass => InverseMass == 0 ? float.PositiveInfinity : 1 / InverseMass;
}