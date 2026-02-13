using OpenTK.Graphics.OpenGL4;

namespace Physics.Demo.Graphics;

internal class Shader() : IDisposable
{
	private static readonly Dictionary<string, ShaderType> ExtensionToType = new()
	{
		{ ".vert", ShaderType.VertexShader },
		{ ".frag", ShaderType.FragmentShader }
	};

	private int Program;
	private Dictionary<string, int> Attributes = [];
	private Dictionary<string, int> Uniforms = [];

	public void Compile(IList<string> files)
	{
		Program = CompileProgram(files);
		Attributes = DiscoverAttributes(Program);
		Uniforms = DiscoverUniforms(Program);
	}

	public void Use()
	{
		GL.UseProgram(Program);
	}

	public void Dispose()
	{
		GL.DeleteProgram(Program);
		GC.SuppressFinalize(this);
	}

	public int GetAttribute(string name)
	{
		return Attributes.GetValueOrDefault(name, -1);
	}

	public int GetUniform(string name)
	{
		return Uniforms.GetValueOrDefault(name, -1);
	}

	private static int CompileProgram(IList<string> files)
	{
		var program = GL.CreateProgram();
		var shaders = new List<int>(files.Count);

		foreach (var it in files)
		{
			var source = File.ReadAllText(it);
			var type = ExtensionToType[Path.GetExtension(it)];
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
			map[key] = GL.GetAttribLocation(program, key);
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
			map[key] = GL.GetUniformLocation(program, key); ;
		}

		return map;
	}
}
