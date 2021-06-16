using OpenTK.Mathematics;

namespace Breakout
{
    public class Particle
    {
        public Vector2 Position = new(0.0f);
        public Vector2 Velocity = new(0.0f);
        public Vector4 Color = new(1.0f);
        public float Life = 0.0f;

        public Particle()
        {
        }
    }
}
