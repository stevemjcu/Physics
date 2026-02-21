using OpenTK.Mathematics;

namespace Physics;

public class Particle(Vector3 position, Vector3 velocity, float mass, bool gravity)
{
    public Particle(Vector3 position, float mass, bool gravity) : this(position, default, mass, gravity) { }

    public Particle(Vector3 position) : this(position, float.PositiveInfinity, false) { }

    public Vector3 Position { get; set; } = position;

    public Vector3 PreviousPosition { get; set; }

    public Vector3 Displacement => Position - PreviousPosition;

    public Vector3 Velocity { get; set; } = velocity;

    public float Mass { get; set; } = mass;

    public float InverseMass => 1 / Mass;

    public bool HasGravity { get; set; } = gravity;
}