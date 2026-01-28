using OpenTK.Graphics.OpenGL4;

namespace Physics.Demo.Graphics;

internal class Buffer
{
    private readonly int VertexArray; // VAO => layout
    private readonly int VertexBuffer; // VBO => vertices

    public Buffer(IList<float> vertices, IList<int> layout)
    {
        var size = vertices.Count * sizeof(float);
        var stride = layout.Sum() * sizeof(float);
        var type = VertexAttribPointerType.Float;

        VertexArray = GL.GenVertexArray();
        VertexBuffer = GL.GenBuffer();

        GL.BindVertexArray(VertexArray);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, size, vertices.ToArray(), BufferUsageHint.DynamicDraw);

        for (var (i, offset) = (0, 0); i < layout.Count; i++)
        {
            GL.VertexAttribPointer(i, layout[i], type, false, stride, offset);
            GL.EnableVertexAttribArray(i);
            offset += layout[i] * sizeof(float);
        }
    }
}
