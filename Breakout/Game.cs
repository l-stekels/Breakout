using OpenTK.Mathematics;
using Keyboard = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using System.Collections.Generic;

namespace Breakout
{
    public class Game
    {
        public int Width;
        public int Height;
        public GameState State = GameState.Active;
        public SpriteRenderer Renderer;
        public Player Player;
        public BallObject Ball;
        public List<GameLevel> Levels = new();
        public int Level = 0;

        public bool[] Keys = new bool[1024];

        private readonly string[] SpriteVertexPath = { "Shaders", "sprite.vert" };
        private readonly string[] SpriteFragmentPath = { "Shaders", "sprite.frag" };        

        public Game(int width, int height) => (Width, Height) = (width, height);

        ~Game()
        {
            ResourceManager.Clear();
            Renderer = null;
        }

        public void Init()
        {
            string[] texturePaths = ResourceManager.GetTexturePaths();
            string[] levelPaths = ResourceManager.GetLevelPaths();

            Shader sprite = ResourceManager.LoadShader("sprite", SpriteVertexPath, SpriteFragmentPath);

            Matrix4.CreateOrthographicOffCenter(0.0f, (float)Width, (float)Height, 0.0f, -1.0f, 1.0f, out Matrix4 projection);

            sprite.Use().SetInteger("image", 0).SetMatrix4("projection", projection);
            Renderer = new SpriteRenderer(sprite);
            foreach (string path in texturePaths)
            {
                ResourceManager.LoadTexture(path);
            }

            foreach (string path in levelPaths)
            {
                Levels.Add(new GameLevel(path, Width, Height / 2));
            }

            Player = new(Width, Height);
            Ball = new(Player);
        }

        public void ProcessInput(float dt)
        {
            if (Keys[(int)Keyboard.A])
            {
                Player.MoveLeft(dt);
            }
            if (Keys[(int)Keyboard.D])
            {
                Player.MoveRight(dt);
            }
            if (Keys[(int)Keyboard.Space])
            {
                Ball.Stuck = false;
            }
        }

        public void Update(float dt)
        {
            Ball.Move(dt, Width);
            DoCollisions();
        }

        public void Render()
        {
            Renderer.DrawSprite(
                ResourceManager.GetTexture("background"),
                new Vector2(0, 0),
                new Vector2(Width, Height),
                0,
                new Vector3(1.0f, 1.0f, 1.0f)
            );
            Levels[Level].Draw(Renderer);
            Player.Draw(Renderer);
            Ball.Draw(Renderer);
        }

        public void DoCollisions()
        {
            foreach (GameObject brick in Levels[Level].Bricks)
            {
                int key = Levels[Level].Bricks.IndexOf(brick);
                if (brick.Destroyed)
                {
                    continue;
                }
                if (!CheckCollision(brick, Ball))
                {
                    continue;
                }
                Levels[Level].Bricks[key].Destroyed = !brick.Solid;
            }
        }

        private bool CheckCollision(GameObject one, GameObject two)
        {
            bool oneXCollision = one.Position.X + one.Size.X >= two.Position.X;
            bool twoXCollision = two.Position.X + two.Size.X >= one.Position.X;

            bool oneYCollision = one.Position.Y + one.Size.Y >= two.Position.Y;
            bool twoYCollision = two.Position.Y + two.Size.Y >= one.Position.Y;

            return oneXCollision && twoXCollision && oneYCollision && twoYCollision;
        }


    }
}
