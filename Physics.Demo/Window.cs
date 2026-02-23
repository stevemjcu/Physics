using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.Colliders;
using Physics.Constraints;
using Physics.Demo.Graphics;
using System.Drawing;

namespace Physics.Demo;

internal class Window : GameWindow
{
    private const string VertPath = @"Shaders\basic.vert";
    private const string FragPath = @"Shaders\basic.frag";
    private const string ModelPath = @"Models\cube.obj";
    private const int BufferSize = 16;

    private const int VerticalFovDeg = 80;
    private const float DepthNear = 0.1f;
    private const float DepthFar = 100f;

    private const float FixedTimestep = 1 / 60f;
    private float Accumulator;

    private static readonly Color4 PrimaryColor = Color4.White;
    private static readonly Color4 SecondaryColor = Color4.Green;
    private static readonly Color4 TertiaryColor = Color4.LightBlue;

    private readonly Simulation Simulation;
    private readonly Camera Camera;
    private readonly Shader Shader;
    private readonly Buffer<Vector3> Buffer;
    private readonly Controller Controller;

    public Window(
        GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        Simulation = new();

        Camera = new()
        {
            VerticalFov = MathHelper.DegreesToRadians(VerticalFovDeg),
            AspectRatio = Size.X / (float)Size.Y,
            DepthNear = DepthNear,
            DepthFar = DepthFar
        };

        Controller = new()
        {
            MouseState = MouseState,
            KeyboardState = KeyboardState,
            Camera = Camera,
            Simulation = Simulation
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
        //GL.Enable(EnableCap.CullFace);

        var scale = Matrix4.CreateScale(0.5f);
        var translation = Matrix4.CreateTranslation(new Vector3(0, 10, 0));

        Shader.Compile([VertPath, FragPath]);
        Buffer.Initialize();
        Camera.Position = new(0, 1, 3);

        var model = Model.Import(ModelPath);
        model.Load(Simulation, new(default, 1, true), 0, scale * translation);

        //var interval = 0.25f;
        //Simulation.Particles.Add(new(new(0, 1, 0)));

        //for (var i = 1; i < 24; i++)
        //{
        //    var p = new Particle(new(0, -i * interval, 0), Vector3.Zero, 1, true);
        //    var c = new DistanceConstraint(Simulation.Particles[i - 1], p, interval, 0.2f);
        //    Simulation.Particles.Add(p);
        //    Simulation.Constraints.Add(c);
        //}

        var u = new Particle(new Vector3(-1, 0, 1) * 30);
        var v = new Particle(new Vector3(1, 0, 1) * 30);
        var w = new Particle(new Vector3(0, 0, -1) * 30);
        Simulation.Colliders.Add(new TriangleCollider(u, w, v));
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

        Controller.UpdateCamera(timestep);
        Controller.UpdateGrabber();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Shader.Use();
        Buffer.Bind();

        var view = Camera.View;
        var projection = Camera.Projection;
        var identity = Matrix4.Identity;

        GL.PointSize(4.5f);
        GL.UniformMatrix4(Shader.GetUniform("view"), true, ref view);
        GL.UniformMatrix4(Shader.GetUniform("projection"), true, ref projection);
        GL.Uniform4(Shader.GetUniform("base_color"), PrimaryColor);

        foreach (var it in Simulation.Particles)
        {
            Buffer.Write(it.Position);
            Buffer.Flush();

            GL.DrawArrays(PrimitiveType.Points, 0, 1);
        }

        foreach (var it in Simulation.Constraints)
        {
            if (it is not DistanceConstraint)
            {
                continue;
            }

            // TODO: Interpolate between position and previous position
            Buffer.Write(it.Particles[0].Position);
            Buffer.Write(it.Particles[1].Position);
            Buffer.Flush();

            GL.DrawArrays(PrimitiveType.LineStrip, 0, 2);
        }

        GL.Uniform4(Shader.GetUniform("base_color"), TertiaryColor);

        foreach (var it in Simulation.Colliders)
        {
            Buffer.Write(it.Particles[0].Position);
            Buffer.Write(it.Particles[1].Position);
            Buffer.Write(it.Particles[2].Position);
            Buffer.Flush();

            GL.CullFace(TriangleFace.Front);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        GL.PointSize(2.5f);
        GL.UniformMatrix4(Shader.GetUniform("view"), true, ref identity);
        GL.UniformMatrix4(Shader.GetUniform("projection"), true, ref identity);
        GL.Uniform4(Shader.GetUniform("base_color"), SecondaryColor);

        Buffer.Write(Vector3.Zero);
        Buffer.Flush();

        GL.DrawArrays(PrimitiveType.Points, 0, 1);

        SwapBuffers();
    }
}
