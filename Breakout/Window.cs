using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Breakout
{
    public class Window : GameWindow
    {
        private readonly Game breakout;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, Game game) : base(gameWindowSettings, nativeWindowSettings)
        {
            breakout = game;
        }

        public static Window WindowFactory(string title, int width, int height)
        {
            GameWindowSettings gameWindowSettings = new();
            NativeWindowSettings nativeWindowSettings = new()
            {
                Title = title,
                Size = new Vector2i(width, height),
                APIVersion = new System.Version(4, 1),
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core
            };
            return new Window(gameWindowSettings, nativeWindowSettings, new Game(width, height));
        }

        protected override void OnLoad()
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            breakout.Init();
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            float deltaTime = (float)args.Time;
            breakout.ProcessInput(deltaTime);
            breakout.Update(deltaTime);
            breakout.Render();
            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused)
            {
                return;
            }
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            base.OnUpdateFrame(args);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if ((int)e.Key == 0 && (int)e.Key < 1024)
            {
                breakout.Keys[(int)e.Key] = true;
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if ((int)e.Key == 0 && (int)e.Key < 1024)
            {
                breakout.Keys[(int)e.Key] = false;
            }
            base.OnKeyUp(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Size.X, e.Size.Y);
            base.OnResize(e);
        }
    }
}
