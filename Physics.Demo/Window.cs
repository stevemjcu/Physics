using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.Constraints;
using Physics.Demo.Graphics;
using System.Drawing;

namespace Physics.Demo;

internal class Window : GameWindow
{
    private const string PointVertPath = @"Shaders\points.vert";
    private const string PointFragPath = @"Shaders\points.frag";
    private const int BufferSize = 16;

    private const float MouseSensitivity = 0.0015f;
    private const float CameraSpeed = 3.5f;
    private const int VerticalFovDeg = 80;
    private const float DepthNear = 0.1f;
    private const float DepthFar = 100f;

    private const int Iterations = 20;
    private const float DampingCoefficient = 0.995f;
    private const float Gravity = 10f;
    private const float FixedTimestep = 1 / 60f;
    private float Accumulator;

    private static readonly Color4 PrimaryColor = Color4.White;
    private static readonly Color4 SecondaryColor = Color4.Green;

    private readonly Simulation Simulation;
    private readonly Camera Camera;
    private readonly Shader Shader;
    private readonly Buffer<Vector3> Buffer;

    private readonly DistanceConstraint Grabber;
    private float GrabDistance;

    public Window(
        GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        Simulation = new()
        {
            Iterations = Iterations,
            DampingCoefficient = DampingCoefficient,
            Gravity = Gravity
        };

        Camera = new()
        {
            VerticalFov = MathHelper.DegreesToRadians(VerticalFovDeg),
            AspectRatio = Size.X / (float)Size.Y,
            DepthNear = DepthNear,
            DepthFar = DepthFar
        };

        Shader = new();
        Buffer = new(BufferSize, [3]);
        Grabber = new(new(), null!, 0, 0.1f);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        CursorState = CursorState.Grabbed;
        GL.ClearColor(Color.Black);
        GL.Enable(EnableCap.DepthTest);

        Shader.Compile([PointVertPath, PointFragPath]);
        Buffer.Initialize();
        Camera.Position = new(0, 0, 3);

        var anchor = new Particle(new(0, 0, 0), Vector3.Zero, float.PositiveInfinity, false);
        Simulation.Particles.Add(anchor);

        var distance = 0.25f;
        for (var i = 1; i < 8; i++)
        {
            var p = new Particle(new(0, -i * distance, 0), Vector3.Zero, 1);
            var c = new DistanceConstraint(Simulation.Particles[i - 1], p, distance, 0.5f);
            Simulation.Particles.Add(p);
            Simulation.Constraints.Add(c);
        }
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        Buffer.Dispose();
        Shader.Dispose();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (!IsFocused)
        {
            return;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        var timestep = (float)args.Time;
        Accumulator += timestep;

        while (Accumulator > FixedTimestep)
        {
            Simulation.Step(FixedTimestep);
            Accumulator -= FixedTimestep;
        }

        UpdateCamera(timestep);
        UpdateGrabber();
    }

    private void UpdateCamera(float timestep)
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

        var x = GetInputAxis(Keys.A, Keys.D);
        var y = GetInputAxis(Keys.LeftControl, Keys.Space);
        var z = GetInputAxis(Keys.W, Keys.S);

        var distance = CameraSpeed * timestep;
        Camera.Position += new Vector3(x, 0, z) * Matrix3.CreateRotationY(angle) * distance;
        Camera.Position += new Vector3(0, y, 0) * distance;
    }

    private void UpdateGrabber()
    {
        if (MouseState.IsButtonPressed(MouseButton.Left))
        {
            foreach (var it in Simulation.Particles)
            {
                if (Utility.Overlaps(Camera.Raycast, new(it.Position, 0.15f), out GrabDistance))
                {
                    Grabber.Particles[1] = it;
                    Simulation.Particles.Add(Grabber.Particles[0]);
                    Simulation.Constraints.Add(Grabber);
                    break;
                }
            }
        }

        if (MouseState.IsButtonDown(MouseButton.Left))
        {
            var arm = Camera.Position + Camera.Direction * GrabDistance;
            Grabber.Particles[0].Position = arm;
        }

        if (MouseState.IsButtonReleased(MouseButton.Left))
        {
            Grabber.Particles[1] = null!;
            Simulation.Particles.Remove(Grabber.Particles[0]);
            Simulation.Constraints.Remove(Grabber);
        }
    }

    private int GetInputAxis(Keys neg, Keys pos)
    {
        var a = KeyboardState.IsKeyDown(neg) ? -1 : 0;
        var b = KeyboardState.IsKeyDown(pos) ? +1 : 0;
        return a + b;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var view = Camera.View;
        var projection = Camera.Projection;
        var identity = Matrix4.Identity;
        var count = Simulation.Particles.Count;

        if (MouseState.IsButtonDown(MouseButton.Left) && Grabber.Particles[1] is not null)
        {
            count--;
        }

        Shader.Use();

        // Draw rope
        {
            GL.PointSize(4.5f);
            GL.UniformMatrix4(Shader.GetUniform("view"), true, ref view);
            GL.UniformMatrix4(Shader.GetUniform("projection"), true, ref projection);
            GL.Uniform4(Shader.GetUniform("base_color"), PrimaryColor);

            for (var i = 0; i < count; i++)
            {
                // TODO: Interpolate between position and previous position
                Buffer.Data[i] = Simulation.Particles[i].Position;
            }

            Buffer.Flush(count);
            Buffer.Bind();

            GL.DrawArrays(PrimitiveType.LineStrip, 0, count);
            GL.DrawArrays(PrimitiveType.Points, 0, count);
        }

        // Draw crosshair
        {
            GL.PointSize(2.5f);
            GL.UniformMatrix4(Shader.GetUniform("view"), true, ref identity);
            GL.UniformMatrix4(Shader.GetUniform("projection"), true, ref identity);
            GL.Uniform4(Shader.GetUniform("base_color"), SecondaryColor);

            Buffer.Data[0] = Vector3.Zero;
            Buffer.Flush(1);

            GL.DrawArrays(PrimitiveType.Points, 0, 1);
        }

        SwapBuffers();
    }
}
