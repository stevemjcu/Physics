using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.Demo.Graphics;
using System.Drawing;

namespace Physics.Demo;

internal class Window : GameWindow
{
    private const string PointVertPath = @"Shaders\points.vert";
    private const string PointFragPath = @"Shaders\points.frag";
    private const int BufferSize = 64;

    private const float MouseSensitivity = 0.0015f;
    private const float CameraSpeed = 3.5f;
    private const int VerticalFovDeg = 80;
    private const float DepthNear = 0.1f;
    private const float DepthFar = 100f;

    private const int PhysicsIterations = 20;
    private const float PhysicsTimestep = 1 / 60f;

    private readonly Simulation Simulation;
    private readonly Camera Camera;
    private readonly Shader Shader;
    private readonly Buffer<Vector3> Buffer;

    public Window(GameWindowSettings a, NativeWindowSettings b) : base(a, b)
    {
        Simulation = new(PhysicsIterations);
        Camera = new()
        {
            VerticalFov = MathHelper.DegreesToRadians(VerticalFovDeg),
            AspectRatio = Size.X / (float)Size.Y,
            DepthNear = DepthNear,
            DepthFar = DepthFar
        };
        Shader = new();
        Buffer = new(BufferSize, [3]);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        CursorState = CursorState.Grabbed;
        GL.ClearColor(Color.Black);
        GL.Enable(EnableCap.DepthTest);

        Shader.Compile([PointVertPath, PointFragPath]);
        Buffer.Initialize();

        Camera.Position = new(0, 0, -3);
        Simulation.Particles.Add(new(Vector3.Zero, Vector3.Zero, 0));
        Simulation.Particles.Add(new(new(0, 0, 3), Vector3.Zero, 0));
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

        UpdateCamera();
        //Simulation.Step(PhysicsTimestep);
    }

    private void UpdateCamera()
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

        var distance = CameraSpeed * PhysicsTimestep;
        var x = GetInputAxis(Keys.A, Keys.D);
        var y = GetInputAxis(Keys.LeftControl, Keys.Space);
        var z = GetInputAxis(Keys.W, Keys.S);

        Camera.Position += new Vector3(x, 0, z) * Matrix3.CreateRotationY(angle) * distance;
        Camera.Position += new Vector3(0, y, 0) * distance;
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
        var count = Simulation.Particles.Count;

        for (var i = 0; i < count; i++)
        {
            Buffer.Data[i] = Simulation.Particles[i].Position;
        }

        Buffer.Flush(count);

        Shader.Use();
        Buffer.Bind();
        GL.DrawArrays(PrimitiveType.Points, 0, count);

        SwapBuffers();
    }
}
