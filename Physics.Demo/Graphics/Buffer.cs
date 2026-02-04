using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Physics.Demo.Graphics;

internal class Buffer<T>(int length, IList<int> layout) : IDisposable where T : unmanaged
{
    private readonly int Stride = Marshal.SizeOf<T>();

    private int VertexArray; // VAO = layout
    private int VertexBuffer; // VBO = data

    public T[] Data { get; } = new T[length];

    public void Initialize()
    {
        VertexArray = GL.GenVertexArray();
        GL.BindVertexArray(VertexArray);

        VertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, length * Stride, 0, BufferUsageHint.DynamicDraw);

        for (var (i, offset) = (0, 0); i < layout.Count; i++)
        {
            // TODO: Use reflection to determine layout of T, to allow mixed/interleaved types?
            GL.VertexAttribPointer(i, layout[i], VertexAttribPointerType.Float, false, Stride, offset);
            GL.EnableVertexAttribArray(i);
            offset += layout[i] * sizeof(float);
        }
    }

    public void Flush(int count)
    {
        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, count * Stride, Data);
    }

    public void Bind()
    {
        GL.BindVertexArray(VertexArray);
    }

    public void Dispose()
    {
        GL.DeleteBuffers(2, [VertexBuffer, VertexArray]);
    }
}
