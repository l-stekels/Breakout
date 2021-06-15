using OpenTK.Mathematics;
using System.IO;

namespace Breakout
{
    public class Game
    {
        public int Width;
        public int Height;
        public GameState State = GameState.Active;
        public SpriteRenderer Renderer;

        public bool[] Keys = new bool[1024];

        public Game(int width, int height)
        {
            Width = width;
            Height = height;
        }

        ~Game()
        {
            ResourceManager.Clear();
        }

        public void Init()
        {
            string[] vertexPath = { "Shaders", "sprite.vs" };
            string[] fragmentPath = { "Shaders", "sprite.frag" };
            string[] ballTexturePath = { "Textures", "ball.png" };

            Shader sprite = ResourceManager.LoadShader("sprite", Path.Combine(vertexPath), Path.Combine(fragmentPath));

            Matrix4.CreateOrthographicOffCenter(0.0f, (float)Width, (float)Height, 0.0f, -1.0f, 1.0f, out Matrix4 projection);

            sprite.Use().SetInteger("image", 0).SetMatrix4("projection", projection);
            Renderer = new SpriteRenderer(sprite);
            ResourceManager.LoadTexture(Path.Combine(ballTexturePath), true, "ball");
        }

        public void ProcessInput(float dt)
        {

        }

        public void Update(float dt)
        {

        }

        public void Render()
        {
            Renderer.DrawSprite(
                ResourceManager.GetTexture("ball"),
                new Vector2(200, 200),
                new Vector2(300, 400),
                45,
                new Vector3(0, 1, 0)
            );
        }
    }
}
