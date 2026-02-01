using System.Numerics;

namespace Physics;

public abstract class Collider
{
    public record struct Contact(Vector3 Point, Vector3 Normal);

    public abstract Contact GetContact(Collider target);

    public abstract Constraint GenerateConstraint(Contact contact);
}
