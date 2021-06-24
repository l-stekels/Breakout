using OpenTK.Mathematics;

namespace Breakout
{
    public class BallObject : GameObject
    {
        private readonly Vector3 _brightOrange = new(1.0f, 0.415f, 0.101f);
        public readonly Vector2 InitialVelocity = new(100.0f, -500.0f);
        public float Radius = 12.5f;
        public bool Stuck = true;
        public Vector2 StartPosition;

        public BallObject(Player player)
        {
            CalculateStartPosition(player);
            Velocity = InitialVelocity;
            Sprite = ResourceManager.GetTexture("ball");
            Position = StartPosition;
            Size = new(Radius * 2.0f, Radius * 2.0f);
            Color = _brightOrange;
        }

        public Vector2 Move(float dt, int windowWidth)
        {
            if (Stuck)
            {
                return Position;
            }
            Position += Velocity * dt;

            if (Position.X <= 0.0f)
            {
                Velocity.X = -Velocity.X;
                Position.X = 0.0f;
            }
            else if (Position.X + Size.X >= windowWidth)
            {
                Velocity.X = -Velocity.X;
                Position.X = windowWidth - Size.X;
            }

            if (Position.Y <= 0.0f)
            {
                Velocity.Y = -Velocity.Y;
                Position.Y = 0.0f;
            }
            return Position;
        }

        public void Reset()
        {
            Position = StartPosition;
            Velocity = InitialVelocity;
            Stuck = true;
        }

        public void MoveLeft(float velocity)
        {
            if (Stuck)
            {
                Position.X -= velocity;
            }
        }

        public void MoveRight(float velocity)
        {
            if (Stuck)
            {
                Position.X += velocity;
            }
        }

        private void CalculateStartPosition(Player player)
        {
            StartPosition = Vector2.Add(
                player.Position,
                new Vector2(player.Size.X / 2.0f - Radius, -Radius * 2.0f)
            );
        }
    }
}
