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

    private const float MouseSensitivity = 0.0015f;
    private const float CameraSpeed = 3.5f;

    private const int VerticalFovDeg = 80;
    private const float DepthNear = 0.1f;
    private const float DepthFar = 100f;

    private const int Iterations = 20;

    private readonly Simulation Simulation;
    private readonly Camera Camera;
    private readonly Shader Shader;
    private readonly Buffer<Vector3> Buffer;

    public Window(GameWindowSettings a, NativeWindowSettings b) : base(a, b)
    {
        Simulation = new(Iterations);
        Camera = new()
        {
            VerticalFov = MathHelper.DegreesToRadians(VerticalFovDeg),
            AspectRatio = Size.X / (float)Size.Y,
            DepthNear = DepthNear,
            DepthFar = DepthFar
        };
        Shader = new([PointVertPath, PointFragPath]);
        Buffer = new(10, [3]);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        CursorState = CursorState.Grabbed;
        GL.ClearColor(Color.Black);
        GL.Enable(EnableCap.DepthTest);

        Shader.Compile();
        Buffer.Initialize();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

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

        UpdateCamera((float)args.Time);
    }

    private void UpdateCamera(float seconds)
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

        var distance = CameraSpeed * seconds;
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

        for (var i = 0; i < Simulation.Particles.Count; i++)
        {
            Buffer.Data[i] = Simulation.Particles[i].Position;
        }
        Buffer.Flush();

        Shader.Use();
        Buffer.Bind();
        GL.DrawArrays(PrimitiveType.Points, 0, Simulation.Particles.Count);

        SwapBuffers();
    }
}
