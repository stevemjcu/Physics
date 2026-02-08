using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Physics.Demo.Graphics;

internal class Camera
{
    public Vector3 Position { get; set; }

    public Vector3 Rotation { get; set; }

    public float VerticalFov { get; set; }

    public float AspectRatio { get; set; }

    public float DepthNear { get; set; }

    public float DepthFar { get; set; }

    public Vector3 Direction => Collision.GetDirection(Rotation);

    public Matrix4 View => Matrix4.LookAt(Position, Position + Direction, Vector3.UnitY);

    public Matrix4 Projection => Matrix4.CreatePerspectiveFieldOfView(VerticalFov, AspectRatio, DepthNear, DepthFar);

    public Ray Raycast => new(Position, Direction);
}
