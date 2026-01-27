using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Physics.Demo.Graphics;

internal class Buffer<T> where T : unmanaged
{
    private readonly int VertexArray; // VAO => layout
    private readonly int VertexBuffer; // VBO => vertices

    public Buffer(IList<T> vertices, IList<int> layout)
    {
        var size = Marshal.SizeOf<T>();
        VertexArray = GL.GenVertexArray();
        VertexBuffer = GL.GenBuffer();

        GL.BindVertexArray(VertexArray);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * size, vertices.ToArray(), BufferUsageHint.DynamicDraw);

        for (var (i, offset) = (0, 0); i < layout.Count; i++)
        {
            var type = VertexAttribPointerType.Float;
            GL.VertexAttribPointer(i, layout[i], type, false, size, offset);
            GL.EnableVertexAttribArray(i);
            offset += layout[i];
        }
    }
}
