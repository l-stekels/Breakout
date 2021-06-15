using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Breakout
{
    class Program
    {
        public const string Title = "Breakout";
        public const int Width = 1024;
        public const int Height = 800;

        static void Main(string[] args)
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

            using (Window window = new(gameWindowSettings, nativeWindowSettings, new Game(Width, Height)))
            {
                window.Run();
            };
        }
    }
}
