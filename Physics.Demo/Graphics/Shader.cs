using OpenTK.Graphics.OpenGL4;

namespace Physics.Demo.Graphics;

public class Shader
{
    private int Handle;
    private Dictionary<string, int> Attributes = [];
    private Dictionary<string, int> Uniforms = [];

    public void Compile(List<(string, ShaderType)> shaders)
    {
        var handles = new List<int>(shaders.Count);

        foreach (var it in shaders)
        {
            handles.Add(CompileShader(it.Item1, it.Item2));
        }

        Handle = GL.CreateProgram();

        foreach (var it in handles)
        {
            GL.AttachShader(Handle, it);
        }

        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var status);

        if (status == 0)
        {
            throw new Exception(GL.GetProgramInfoLog(Handle));
        }

        foreach (var it in handles)
        {
            GL.DetachShader(Handle, it);
            GL.DeleteShader(it);
        }

        Attributes = DiscoverAttributes(Handle);
        Uniforms = DiscoverUniforms(Handle);
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
