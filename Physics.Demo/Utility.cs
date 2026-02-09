using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Physics.Demo
{
    internal static class Utility
    {
        public static Vector3 GetDirection(Vector3 rotation)
        {
            var rotationX = Matrix3.CreateRotationX(rotation.X);
            var rotationY = Matrix3.CreateRotationY(rotation.Y);
            var rotationZ = Matrix3.CreateRotationZ(rotation.Z);

            return (-Vector3.UnitZ * rotationX * rotationY * rotationZ).Normalized();
        }

        public static int GetInputAxis(this KeyboardState state, Keys neg, Keys pos)
        {
            var a = state.IsKeyDown(neg) ? -1 : 0;
            var b = state.IsKeyDown(pos) ? +1 : 0;
            return a + b;
        }
    }
}
