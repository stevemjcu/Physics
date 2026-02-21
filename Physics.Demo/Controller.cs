using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.Constraints;
using Physics.Demo.Graphics;
using Physics.Shapes;

namespace Physics.Demo;

internal class Controller()
{
    private const float MouseSensitivity = 0.0015f;
    private const float CameraSpeed = 3.5f;

    public required MouseState MouseState;
    public required KeyboardState KeyboardState;
    public required Camera Camera;
    public required Simulation Simulation;

    private readonly DistanceConstraint Constraint = new(new(Vector3.Zero, 10, false), null!, 0, 0.1f);
    private float ArmLength;

    public void UpdateCamera(float timestep)
    {
        var rotation = (MouseState.Position - MouseState.PreviousPosition) * MouseSensitivity;
        Camera.Rotation = new(Camera.Rotation.X - rotation.Y, Camera.Rotation.Y - rotation.X, 0);

        var direction = (Camera.Direction with { Y = 0 }).Normalized();
        var angle = Vector3.CalculateAngle(-Vector3.UnitZ, direction);
        var cross = Vector3.Cross(-Vector3.UnitZ, direction).Normalized();

        if (Vector3.CalculateAngle(cross, Vector3.UnitY) > 0)
        {
            angle *= -1;
        }

        var x = KeyboardState.GetInputAxis(Keys.A, Keys.D);
        var y = KeyboardState.GetInputAxis(Keys.LeftControl, Keys.Space);
        var z = KeyboardState.GetInputAxis(Keys.W, Keys.S);

        var distance = CameraSpeed * timestep;
        Camera.Position += new Vector3(x, 0, z) * Matrix3.CreateRotationY(angle) * distance;
        Camera.Position += new Vector3(0, y, 0) * distance;
    }

    public void UpdateGrabber()
    {
        if (MouseState.IsButtonPressed(MouseButton.Left))
        {
            var particle = (Particle?)null;
            var length = float.MaxValue;

            foreach (var it in Simulation.Particles)
            {
                var sphere = new Sphere(it.Position, 0.15f);
                if (Camera.Ray.Overlaps(sphere, out var t) && t < length)
                {
                    particle = it;
                    length = t;
                }
            }

            if (particle != null)
            {
                Constraint.Particles[1] = particle;
                Simulation.Constraints.Add(Constraint);
                ArmLength = length;
            }
        }

        if (MouseState.IsButtonDown(MouseButton.Left))
        {
            var arm = Camera.Position + Camera.Direction * ArmLength;
            Constraint.Particles[0].Position = arm;
        }

        if (MouseState.IsButtonReleased(MouseButton.Left))
        {
            Simulation.Constraints.Remove(Constraint);
        }
    }
}
