using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Physics.Demo
{
    internal static class Utility
    {
        public static int GetInputAxis(this KeyboardState state, Keys neg, Keys pos)
        {
            var a = state.IsKeyDown(neg) ? -1 : 0;
            var b = state.IsKeyDown(pos) ? +1 : 0;
            return a + b;
        }
    }
}
