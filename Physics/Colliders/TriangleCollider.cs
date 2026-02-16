using Physics.Shapes;

namespace Physics.Colliders;

public class TriangleCollider(Particle a, Particle b, Particle c) : Collider([a, b, c])
{
    public Triangle Triangle => new(a.Position, b.Position, c.Position);
}
