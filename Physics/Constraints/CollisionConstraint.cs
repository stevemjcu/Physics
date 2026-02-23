using OpenTK.Mathematics;

namespace Physics.Constraints;

public class CollisionConstraint(Particle source, Vector3 contact, Vector3 normal, float compliance)
    : Constraint([source], compliance, true)
{
    public Vector3 Contact { get; set; } = contact;

    public Vector3 Normal { get; set; } = normal;

    public override void CalculateError()
    {
        Error = Vector3.Dot(Particles[0].Position - Contact, -Normal);
        Gradient[0] = -Normal;
    }
}
