using OpenTK.Mathematics;

namespace Physics;

public class Particle(
    Vector3 position = default,
    Vector3 velocity = default,
    float mass = float.PositiveInfinity,
    bool hasGravity = false)
{
    public Vector3 Position { get; set; } = position;

    public Vector3 PreviousPosition { get; set; }

    public Vector3 Velocity { get; set; } = velocity;

    public float Mass { get; set; } = mass;

    public float InverseMass => 1 / Mass;

    public bool HasGravity { get; set; } = hasGravity;
}