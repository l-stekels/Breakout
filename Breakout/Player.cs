using OpenTK.Mathematics;

namespace Breakout
{
    public class Player : GameObject
    {
        public readonly float InitialVelocity = 500.0f;
        private readonly Vector2 InitialPlayerSize = new(100.0f, 20.0f);
        private readonly int GameWidth;
        private readonly int GameHeight;

        public Player(int gameWidth, int gameHeight)
        {
            (GameWidth, GameHeight) = (gameWidth, gameHeight);
            Position = GetInitialPosition();
            Sprite = ResourceManager.GetTexture("paddle");
            Size = InitialPlayerSize;
        }

        public void Reset()
        {
            Size = InitialPlayerSize;
            Position = GetInitialPosition();
        }

        public Vector2 GetInitialPosition()
        {
            return new Vector2(
                GameWidth / 2.0f - InitialPlayerSize.X / 2.0f,
                GameHeight - InitialPlayerSize.Y
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
            if (Position.X > GameWidth - Size.X)
            {
                return false;
            }
            Position.X += velocity;
            return true;
        }
    }
}
