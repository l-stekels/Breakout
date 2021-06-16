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
        public ParticleGenerator Particles;

        public bool[] Keys = new bool[1024];

        private readonly string[] SpriteVertexPath = { "Shaders", "sprite.vert" };
        private readonly string[] SpriteFragmentPath = { "Shaders", "sprite.frag" };

        private readonly string[] ParticleVertexPath = { "Shaders", "particle.vert" };
        private readonly string[] ParticleFragmentPath = { "Shaders", "particle.frag" };

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
            Shader particle = ResourceManager.LoadShader("particle", ParticleVertexPath, ParticleFragmentPath);

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
            Particles = new ParticleGenerator(particle, ResourceManager.GetTexture("particle"), 500);
        }

        public void ProcessInput(float dt)
        {
            if (Keys[(int)Keyboard.A])
            {
                Player.MoveLeft(dt);
                if (State.Equals(GameState.Active))
                {
                    Ball.MoveLeft(dt * Player.InitialVelocity);
                }
            }
            if (Keys[(int)Keyboard.D])
            {
                Player.MoveRight(dt);
                if (State.Equals(GameState.Active))
                {
                    Ball.MoveRight(dt * Player.InitialVelocity);
                }
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
            if (Ball.Position.Y >= Height)
            {
                ResetLevel();
                ResetPlayer();
            }
            Particles.Update(dt, Ball, 2, new Vector2(Ball.Radius / 2.0f));
        }

        private void ResetLevel()
        {
            Level = 0;
            Levels[Level].Reset();
        }

        private void ResetPlayer()
        {
            Player.Reset();
            Ball.Reset();
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
            Particles.Draw();
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

                (bool collision, Direction direction, Vector2 difference) = CheckCollision(Ball, brick);

                if (!collision)
                {
                    continue;
                }

                if (!brick.Solid)
                {
                    Levels[Level].Bricks[key].Destroyed = true;
                }

                float penetration;
                if (direction == Direction.Left || direction == Direction.Right)
                {
                    Ball.Velocity.X = -Ball.Velocity.X;
                    penetration = Ball.Radius - MathHelper.Abs(difference.X);
                } else
                {
                    Ball.Velocity.Y = -Ball.Velocity.Y;
                    penetration = Ball.Radius - MathHelper.Abs(difference.Y);
                }
                switch (direction)
                {
                    case Direction.Left:
                        Ball.Position.X += penetration;
                        break;
                    case Direction.Right:
                        Ball.Position.X -= penetration;
                        break;
                    case Direction.Up:
                        Ball.Position.Y -= penetration;
                        break;
                    default:
                        Ball.Position.Y += penetration;
                        break;
                }
            }
            if (Ball.Stuck)
            {
                return;
            }
            (bool playerBallCollision, _, _) = CheckCollision(Ball, Player);
            if (!playerBallCollision)
            {
                return;
            }
            float centerBoard = Player.Position.X + Player.Size.X / 2.0f;
            float distance = (Ball.Position.X + Ball.Radius) - centerBoard;
            float percentage = distance / (Player.Size.X / 2.0f);

            float strength = 2.0f;
            Vector2 oldVelocity = Ball.Velocity;
            Ball.Velocity.X = Ball.InitialVelocity.X * percentage * strength;
            Ball.Velocity.Y = -1.0f * MathHelper.Abs(Ball.Velocity.Y);
            Ball.Velocity = Vector2.Normalize(Ball.Velocity) * oldVelocity.Length;
        }

        private bool CheckCollision(GameObject one, GameObject two)
        {
            bool oneXCollision = one.Position.X + one.Size.X >= two.Position.X;
            bool twoXCollision = two.Position.X + two.Size.X >= one.Position.X;

            bool oneYCollision = one.Position.Y + one.Size.Y >= two.Position.Y;
            bool twoYCollision = two.Position.Y + two.Size.Y >= one.Position.Y;

            return oneXCollision && twoXCollision && oneYCollision && twoYCollision;
        }

        private (bool, Direction, Vector2) CheckCollision(BallObject one, GameObject two)
        {
            Vector2 center = new(
                one.Position.X + one.Radius,
                one.Position.Y + one.Radius
            );
            Vector2 aAbBHalfExtents = new(two.Size.X / 2.0f, two.Size.Y / 2.0f);
            Vector2 aAbBCenter = new(
                two.Position.X + aAbBHalfExtents.X,
                two.Position.Y + aAbBHalfExtents.Y
            );
            Vector2 difference = center - aAbBCenter;
            Vector2 clamped = Vector2.Clamp(difference, -aAbBHalfExtents, aAbBHalfExtents);
            Vector2 closest = aAbBCenter + clamped;
            difference = closest - center;

            if (difference.Length <= one.Radius)
            {
                return (true, VectorDirection(difference), difference);
            }

            return (false, Direction.Up, Vector2.Zero);
        }

        private Direction VectorDirection(Vector2 target)
        {
            Vector2[] compass =
            {
                new Vector2(0.0f, 1.0f),  // up
                new Vector2(1.0f, 0.0f),  // right
                new Vector2(0.0f, -1.0f), // down
                new Vector2(-1.0f, 0.0f), // left
            };
            float max = 0.0f;
            uint bestMatch = 0;
            for (uint i = 0; i < 4; i++)
            {
                float dotProduct = Vector2.Dot(Vector2.Normalize(target), compass[i]);
                if (dotProduct > max)
                {
                    max = dotProduct;
                    bestMatch = i;
                }
            }
            return (Direction)bestMatch;
        }
    }
}
