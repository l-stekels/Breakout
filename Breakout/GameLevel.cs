﻿using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Breakout
{
    public class GameLevel
    {
        public List<GameObject> Bricks = new();

        public GameLevel(string filePath, int levelWidth, int levelHeight)
        {
            ResourceManager.ThrowIfFileDoesNotExist(filePath);
            Bricks.Clear();
            List<List<int>> tileData = new();
            string line;
            System.IO.StreamReader file = new(filePath);
            while ((line = file.ReadLine()) != null)
            {
                List<int> row = new();
                foreach (string tileCode in line.Split(' '))
                {
                    row.Add(Convert.ToInt32(tileCode));
                }
                tileData.Add(row);
            }
            if (tileData.Count <= 0)
            {
                return;
            }
            Init(tileData, levelWidth, levelHeight);
        }

        public void Draw(SpriteRenderer renderer)
        {
            foreach (GameObject tile in Bricks)
            {
                if (tile.Destroyed)
                {
                    continue;
                }
                tile.Draw(renderer);
            }
        }

        public bool IsCompleted()
        {
            foreach (GameObject tile in Bricks)
            {
                if (tile.Exists())
                {
                    return false;
                }
            }
            return true;
        }

        private void Init(List<List<int>> tileData, int levelWidth, int levelHeight)
        {
            int height = tileData.Count;
            int width = tileData[0].Count;
            float unitWidth = levelWidth / width;
            float unitHeight = levelHeight / height;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int tileCode = tileData[y][x];

                    GameObject obj = new(
                        new(unitWidth * x, unitHeight * y),
                        new(unitWidth, unitHeight),
                        ParseTexture(tileCode),
                        ParseColor(tileCode)
                    )
                    {
                        Solid = tileCode == 1
                    };
                    Bricks.Add(obj);
                }
            }
        }

        private Texture2D ParseTexture(int code)
        {
            if (code == 1)
            {
                return ResourceManager.GetTexture("block_solid");
            }
            return ResourceManager.GetTexture("block");
        }

        private Vector3 ParseColor(int code)
        {
            if (code == 1)
            {
                return new Vector3(0.8f, 0.8f, 0.7f);
            }
            if (code == 2)
            {
                return new Vector3(0.2f, 0.6f, 1.0f);
            }
            if (code == 3)
            {
                return new Vector3(0.0f, 0.7f, 0.0f);
            }
            if (code == 4)
            {
                return new Vector3(0.8f, 0.8f, 0.7f);
            }
            if (code == 5)
            {
                return new Vector3(1.0f, 0.5f, 0.0f);
            }

            return new Vector3(1.0f);
        }
    }
}
