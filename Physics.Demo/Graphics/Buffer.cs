using OpenTK.Graphics.OpenGL4;

namespace Physics.Demo.Graphics;

internal class Buffer
{
    private readonly int VertexArray; // VAO describes layout
    private readonly int VertexBuffer; // VBO contains data

    public float[] Vertices { get; }

    public Buffer(IList<float> vertices, IList<int> layout)
    {
        Vertices = [.. vertices];
        var size = Vertices.Length * sizeof(float);

        VertexArray = GL.GenVertexArray();
        VertexBuffer = GL.GenBuffer();

        GL.BindVertexArray(VertexArray);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, size, Vertices, BufferUsageHint.DynamicDraw);

        for (var i = 0; i < layout.Count; i++)
        {
            var count = layout[i];
            var type = VertexAttribPointerType.Float;
            var stride = 8 * sizeof(float);
            var offset = i * count;

            GL.VertexAttribPointer(i, count, type, false, stride, offset);
            GL.EnableVertexAttribArray(i);
        }
    }
}
