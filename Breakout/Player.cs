using OpenTK.Mathematics;

namespace Breakout
{
    public class Player : GameObject
    {
        private readonly float PLayerVelocity = 500.0f;
        private readonly Vector2 InitialPlayerSize = new(100.0f, 20.0f);
        private readonly int GameWidth;
        private readonly int GameHeight;

        public Player(int gameWidth, int gameHeight)
        {
            Position = new(
                gameWidth / 2.0f - InitialPlayerSize.X / 2.0f,
                gameHeight - InitialPlayerSize.Y
            );
            (GameWidth, GameHeight) = (gameWidth, gameHeight);
            Sprite = ResourceManager.GetTexture("paddle");
            Size = InitialPlayerSize;
        }

        public void MoveLeft(float dt)
        {
            float velocity = PLayerVelocity * dt;
            if (Position.X < 0.0f)
            {
                return;
            }
            Position.X -= velocity;
        }

        public void MoveRight(float dt)
        {
            float velocity = PLayerVelocity * dt;
            if (Position.X > GameWidth - Size.X)
            {
                return;
            }
            Position.X += velocity;
        }
    }
}
