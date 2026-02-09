using Physics.Shapes;

namespace Physics;

public class Collider
{
    public Particle[] Particles { get; set; } = [];

    public Triangle Triangle => new(Particles[0].Position, Particles[1].Position, Particles[2].Position);
}
