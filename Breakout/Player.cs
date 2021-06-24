using OpenTK.Mathematics;

namespace Breakout
{
    public class Player : GameObject
    {
        public readonly float InitialVelocity = 500.0f;
        private readonly Vector2 _initialPlayerSize = new(100.0f, 20.0f);
        private readonly int _gameWidth;
        private readonly int _gameHeight;

        public Player(int gameWidth, int gameHeight)
        {
            (_gameWidth, _gameHeight) = (gameWidth, gameHeight);
            Position = GetInitialPosition();
            Sprite = ResourceManager.GetTexture("paddle");
            Size = _initialPlayerSize;
        }

        public void Reset()
        {
            Size = _initialPlayerSize;
            Position = GetInitialPosition();
        }

        public Vector2 GetInitialPosition()
        {
            return new Vector2(
                _gameWidth / 2.0f - _initialPlayerSize.X / 2.0f,
                _gameHeight - _initialPlayerSize.Y
            );
        }

        public bool MoveLeft(float dt)
        {
            float velocity = InitialVelocity * dt;
            if (Position.X < 0.0f)
            {
                return false;
            }
            Position.X -= velocity;
            return true;
        }

        public bool MoveRight(float dt)
        {
            float velocity = InitialVelocity * dt;
            if (Position.X > _gameWidth - Size.X)
            {
                return false;
            }
            Position.X += velocity;
            return true;
        }
    }
}
