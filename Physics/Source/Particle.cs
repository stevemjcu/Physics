using System.Numerics;

namespace Physics;

public class Particle
{
    public Vector3 Position { get; set; }

    public Vector3 PreviousPosition { get; set; }

    public Vector3 Velocity { get; set; }

    public float Mass { get; set; }

    public float InverseMass => (1 / Mass);
}