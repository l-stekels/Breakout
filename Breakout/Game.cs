using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Keyboard = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace Breakout
{
    public class Game
    {
        private readonly int _width;
        private readonly int _height;
        private GameState _state = GameState.Active;
        private SpriteRenderer _renderer;
        private Player _player;
        private BallObject _ball;
        private List<GameLevel> _levels = new();
        private int _level;
        private ParticleGenerator _particles;
        private PostProcessor _effects;
        private float _shakeTime;

        public bool[] Keys = new bool[1024];

        private readonly string[] _spriteVertexPath = { "Shaders", "sprite.vert" };
        private readonly string[] _spriteFragmentPath = { "Shaders", "sprite.frag" };

        private readonly string[] _particleVertexPath = { "Shaders", "particle.vert" };
        private readonly string[] _particleFragmentPath = { "Shaders", "particle.frag" };

        private readonly string[] _postProcessingVertexPath = { "Shaders", "post_processing.vert" };
        private readonly string[] _postProcessingFragmentPath = { "Shaders", "post_processing.frag" };

        public Game(int width, int height) => (_width, _height) = (width, height);

        ~Game()
        {
            ResourceManager.Clear();
            _renderer = null;
            _player = null;
            _ball = null;
            _particles = null;
            _effects = null;
        }

        public void Init()
        {
            foreach (var path in ResourceManager.GetTexturePaths())
            {
                ResourceManager.LoadTexture(path);
            }

            var sprite = ResourceManager.LoadShader("sprite", _spriteVertexPath, _spriteFragmentPath);
            var particle = ResourceManager.LoadShader("particle", _particleVertexPath, _particleFragmentPath);
            var postprocessing = ResourceManager.LoadShader("postprocessing", _postProcessingVertexPath, _postProcessingFragmentPath);

            Matrix4.CreateOrthographicOffCenter(0.0f, _width, _height, 0.0f, -1.0f, 1.0f, out var projection);

            sprite.SetInteger("image", 0).SetMatrix4("projection", projection);
            particle.SetInteger("sprite", 0).SetMatrix4("projection", projection);

            _renderer = new SpriteRenderer(sprite);
            _particles = new ParticleGenerator(particle, ResourceManager.GetTexture("particle"), 500);
            _effects = new PostProcessor(postprocessing, _width, _height);

            foreach (var path in ResourceManager.GetLevelPaths())
            {
                _levels.Add(new GameLevel(path, _width, _height / 2));
            }

            _player = new Player(_width, _height);
            _ball = new BallObject(_player);

            _level = (new Random()).Next(4);
        }

        public void Update(float dt)
        {
            _ball.Move(dt, _width);
            DoCollisions();
            _particles.Update(dt, _ball, 2, new Vector2(_ball.Radius / 2.0f));
            if (_shakeTime > 0.0f)
            {
                _shakeTime -= dt;
                if (_shakeTime <= 0.0f)
                {
                    _effects.Shake = false;
                }
            }

            if (!(_ball.Position.Y >= _height)) return;
            ResetLevel();
            ResetPlayer();
        }

        public void ProcessInput(float dt)
        {
            if (Keys[(int)Keyboard.A])
            {
                if (_player.MoveLeft(dt) && _state.Equals(GameState.Active))
                {
                    _ball.MoveLeft(dt * _player.InitialVelocity);
                }
            }
            if (Keys[(int)Keyboard.D])
            {
                if (_player.MoveRight(dt) && _state.Equals(GameState.Active))
                {
                    _ball.MoveRight(dt * _player.InitialVelocity);
                }
            }
            if (Keys[(int)Keyboard.Space])
            {
                _ball.Stuck = false;
            }
        }

        public void Render()
        {
            _effects.BeginRender();
            {
                _renderer.DrawSprite(
                    ResourceManager.GetTexture("background"),
                    new Vector2(0, 0),
                    new Vector2(_width, _height),
                    0,
                    new Vector3(1.0f, 1.0f, 1.0f)
                );
                _levels[_level].Draw(_renderer);
                _player.Draw(_renderer);
                _particles.Draw();
                _ball.Draw(_renderer);
            }
            _effects.EndRender();
            _effects.Render((float)GLFW.GetTime());
        }

        private void DoCollisions()
        {
            foreach (var brick in _levels[_level].Bricks)
            {
                var key = _levels[_level].Bricks.IndexOf(brick);

                if (brick.Destroyed)
                {
                    continue;
                }

                var (collision, direction, (x, y)) = CheckCollision(_ball, brick);

                if (!collision)
                {
                    continue;
                }

                if (_levels[_level].Bricks[key].Destroy())
                {
                    _shakeTime = 0.05f;
                    _effects.Shake = true;
                }

                float penetration;
                if (direction == Direction.Left || direction == Direction.Right)
                {
                    _ball.Velocity.X = -_ball.Velocity.X;
                    penetration = _ball.Radius - MathHelper.Abs(x);
                } else
                {
                    _ball.Velocity.Y = -_ball.Velocity.Y;
                    penetration = _ball.Radius - MathHelper.Abs(y);
                }
                switch (direction)
                {
                    case Direction.Left:
                        _ball.Position.X += penetration;
                        break;
                    case Direction.Right:
                        _ball.Position.X -= penetration;
                        break;
                    case Direction.Up:
                        _ball.Position.Y -= penetration;
                        break;
                    default:
                        _ball.Position.Y += penetration;
                        break;
                }
            }
            if (_ball.Stuck)
            {
                return;
            }
            var (playerBallCollision, _, _) = CheckCollision(_ball, _player);
            if (!playerBallCollision)
            {
                return;
            }
            var centerBoard = _player.Position.X + _player.Size.X / 2.0f;
            var distance = (_ball.Position.X + _ball.Radius) - centerBoard;
            var percentage = distance / (_player.Size.X / 2.0f);

            const float strength = 2.0f;
            var oldVelocity = _ball.Velocity;
            _ball.Velocity.X = _ball.InitialVelocity.X * percentage * strength;
            _ball.Velocity.Y = -1.0f * MathHelper.Abs(_ball.Velocity.Y);
            _ball.Velocity = Vector2.Normalize(_ball.Velocity) * oldVelocity.Length;
        }

        private bool CheckCollision(GameObject one, GameObject two)
        {
            var oneXCollision = one.Position.X + one.Size.X >= two.Position.X;
            var twoXCollision = two.Position.X + two.Size.X >= one.Position.X;

            var oneYCollision = one.Position.Y + one.Size.Y >= two.Position.Y;
            var twoYCollision = two.Position.Y + two.Size.Y >= one.Position.Y;

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
            var difference = center - aAbBCenter;
            var clamped = Vector2.Clamp(difference, -aAbBHalfExtents, aAbBHalfExtents);
            var closest = aAbBCenter + clamped;
            difference = closest - center;

            return difference.Length <= one.Radius ? (true, VectorDirection(difference), difference) : (false, Direction.Up, Vector2.Zero);
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
            var max = 0.0f;
            uint bestMatch = 0;
            for (uint i = 0; i < 4; i++)
            {
                var dotProduct = Vector2.Dot(Vector2.Normalize(target), compass[i]);
                if (!(dotProduct > max)) continue;
                max = dotProduct;
                bestMatch = i;
            }
            return (Direction)bestMatch;
        }

        private void ResetLevel()
        {
            _level = (new Random()).Next(4);
            _levels[_level].Reset();
        }

        private void ResetPlayer()
        {
            _player.Reset();
            _ball.Reset();
        }
    }
}
