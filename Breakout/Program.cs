using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Breakout
{
    internal static class Program
    {
        private const string Title = "Breakout";
        private const int Width = 1024;
        private const int Height = 800;

        private static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = new();
            NativeWindowSettings nativeWindowSettings = new()
            {
                Title = Title,
                Size = new Vector2i(Width, Height),
                APIVersion = new System.Version(4, 1),
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core
            };

            using Window window = new(gameWindowSettings, nativeWindowSettings, new Game(Width, Height));
            window.Run();
        }
    }
}
