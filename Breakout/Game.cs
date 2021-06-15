﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Breakout
{
    public class Game
    {
        public int Width;
        public int Height;
        public GameState State = GameState.Active;
        public SpriteRenderer Renderer;
        public GameObject Player;
        public List<GameLevel> Levels = new();
        public int Level = 0;

        public bool[] Keys = new bool[1024];

        private readonly string[] SpriteVertexPath = { "Shaders", "sprite.vert" };
        private readonly string[] SpriteFragmentPath = { "Shaders", "sprite.frag" };

        private readonly Vector2 InitialPlayerSize = new(100.0f, 20.0f);
        private readonly float InitialPlayerVelocity = 500.0f;

        private readonly Dictionary<GameState, Action<float>> processInputStates = new()
        {
            { GameState.Active, (float dt) => ActiveInputProcess(dt) },
            { GameState.Menu, (float dt) => MenuInputProcess(dt) },
            { GameState.Pause, (float dt) => PauseInputProcess(dt) },
            { GameState.Win, (float dt) => WinInputProcess(dt) },
        };
        //private readonly Dictionary<GameState, Action<float>> updateStates = new()
        //{
        //    { GameState.Active, (float dt) => ActiveUpdate(dt) },
        //    { GameState.Menu, (float dt) => MenuUpdate(dt) },
        //    { GameState.Pause, (float dt) => PauseUpdate(dt) },
        //    { GameState.Win, (float dt) => WinUpdate(dt) },
        //};

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
            Vector2 playerPosition = new(Width / 2.0f - InitialPlayerSize.X / 2.0f, Height - InitialPlayerSize.Y);
            Player = new(playerPosition, InitialPlayerSize, ResourceManager.GetTexture("paddle"));
        }

        public void ProcessInput(float dt)
        {
            processInputStates[State](dt);
        }

        public void Update(float dt)
        {
            //updateStates[State](dt);
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
        }

        private static void ActiveInputProcess(float dt)
        {

        }

        private static void MenuInputProcess(float dt)
        {

        }

        private static void PauseInputProcess(float dt)
        {

        }

        private static void WinInputProcess(float dt)
        {

        }
    }
}
