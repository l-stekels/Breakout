using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Breakout
{
    public class Window : GameWindow
    {
        private readonly Game _breakout;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, Game game) : base(gameWindowSettings, nativeWindowSettings)
        {
            _breakout = game;
        }

        protected override void OnLoad()
        {
            Focus();
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _breakout.Init();
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            float deltaTime = (float)args.Time;
            _breakout.ProcessInput(deltaTime);
            _breakout.Update(deltaTime);
            _breakout.Render();
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
            _breakout.Keys[(int)e.Key] = true;
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            _breakout.Keys[(int)e.Key] = false;
            base.OnKeyUp(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Size.X, e.Size.Y);
            base.OnResize(e);
        }
    }
}
