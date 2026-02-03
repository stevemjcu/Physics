using OpenTK.Mathematics;

namespace Physics;

public class Particle(Vector3 position, Vector3 velocity, float inverseMass)
{
    public Vector3 Position { get; set; } = position;

    public Vector3 PreviousPosition { get; set; }

    public Vector3 Velocity { get; set; } = velocity;

    public float InverseMass { get; set; } = inverseMass;

    public float Mass => InverseMass == 0 ? float.PositiveInfinity : 1 / InverseMass;
}