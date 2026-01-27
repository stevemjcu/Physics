using OpenTK.Graphics.OpenGL4;

namespace Physics.Demo.Graphics;

internal class Shader : IDisposable
{
    private static readonly Dictionary<string, ShaderType> Types = new()
    {
        { "vert", ShaderType.VertexShader },
        { "frag", ShaderType.FragmentShader }
    };

    private readonly int Program;
    private readonly Dictionary<string, int> Attributes = [];
    private readonly Dictionary<string, int> Uniforms = [];

    public Shader(IList<string> paths)
    {
        Program = CompileProgram(paths);
        Attributes = DiscoverAttributes(Program);
        Uniforms = DiscoverUniforms(Program);
    }

    void IDisposable.Dispose()
    {
        GL.DeleteProgram(Program);
        GC.SuppressFinalize(this);
    }

    public static int CompileProgram(IList<string> paths)
    {
        var program = GL.CreateProgram();
        var shaders = new List<int>(paths.Count);

        foreach (var it in paths)
        {
            var source = File.ReadAllText(it);
            var type = Types[Path.GetExtension(it)];

            var shader = CompileShader(source, type);
            shaders.Add(shader);
            GL.AttachShader(program, shader);
        }

        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var status);

        if (status == 0)
        {
            throw new Exception(GL.GetProgramInfoLog(program));
        }

        foreach (var it in shaders)
        {
            GL.DetachShader(program, it);
            GL.DeleteShader(it);
        }

        return program;
    }

    public int GetAttribute(string name)
    {
        return Attributes.TryGetValue(name, out var value) ? value : -1;
    }

    public int GetUniform(string name)
    {
        return Uniforms.TryGetValue(name, out var value) ? value : -1;
    }

    private static int CompileShader(string source, ShaderType type)
    {
        var handle = GL.CreateShader(type);
        GL.ShaderSource(handle, source);
        GL.CompileShader(handle);
        GL.GetShader(handle, ShaderParameter.CompileStatus, out var status);

        if (status == 0)
        {
            throw new Exception(GL.GetShaderInfoLog(handle));
        }

        return handle;
    }

    private static Dictionary<string, int> DiscoverAttributes(int program)
    {
        var map = new Dictionary<string, int>();
        GL.GetProgram(program, GetProgramParameterName.ActiveAttributes, out var count);

        for (var i = 0; i < count; i++)
        {
            var key = GL.GetActiveAttrib(program, i, out _, out _);
            var location = GL.GetAttribLocation(program, key);
            map[key] = location;
        }

        return map;
    }

    private static Dictionary<string, int> DiscoverUniforms(int program)
    {
        var map = new Dictionary<string, int>();
        GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out var count);

        for (var i = 0; i < count; i++)
        {
            var key = GL.GetActiveUniform(program, i, out _, out _);
            var location = GL.GetUniformLocation(program, key);
            map[key] = location;
        }

        return map;
    }
}
