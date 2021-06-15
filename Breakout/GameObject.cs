﻿using OpenTK.Mathematics;

namespace Breakout
{
    public class GameObject
    {
        public Vector2 Positon = new(0.0f);
        public Vector2 Size = new(1.0f);
        public Vector2 Velocity = new(0.0f);
        public Vector3 Color = new(1.0f);
        public float Rotation = 0.0f;
        public bool Solid = false;
        public bool Destroyed = false;
        public Texture2D Sprite = new();

        public GameObject(Vector2 position, Vector2 size, Texture2D sprite, Vector3 color, Vector2 velocity) => (Positon, Size, Sprite, Color, Velocity) = (position, size, sprite, color, velocity);

        public GameObject(Vector2 position, Vector2 size, Texture2D sprite, Vector3 color) => (Positon, Size, Sprite, Color) = (position, size, sprite, color);

        public virtual void Draw(ref SpriteRenderer renderer)
        {
            renderer.DrawSprite(Sprite, Positon, Size, Rotation, Color);
        }

        public bool Exists()
        {
            return !Solid && !Destroyed;
        }
    }
}
