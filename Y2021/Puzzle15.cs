using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle15 : ASolver 
    {
        private int[,] risks;
        private int[,] riskmap;
        private int width, height;

        private int tileWidth, tileHeight;
        public const int TilesMult = 5;

        public Puzzle15(string input) : base(input) { Name = "Chiton"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            width = lines[0].Length;
            height = lines.Count;
            risks = new int[width, height];
            riskmap = new int[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    risks[x, y] = int.Parse(lines[x][y].ToString());
                    riskmap[x, y] = int.MaxValue;
                }
            riskmap[0, 0] = 0;
        }

        private void SetupPart2()
        {
            List<string> lines = Tools.GetLines(Input);

            tileWidth = lines[0].Length;
            tileHeight = lines.Count;
            width = tileWidth * TilesMult;
            height = tileHeight * TilesMult;
            
            risks = new int[width, height];
            riskmap = new int[width, height];

            int[,] tile = new int[tileWidth, tileHeight];
            for (int x = 0; x < tileWidth; x++)
                for (int y = 0; y < tileHeight; y++)
                    tile[x, y] = int.Parse(lines[x][y].ToString());

            for (int i = 0; i < TilesMult; i++)
                for (int j = 0; j < TilesMult; j++)
                    StampTile(tile, i, j);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    riskmap[x, y] = int.MaxValue;
            riskmap[0, 0] = 0;
        }

        private void StampTile(int[,] tile, int i, int j)
        {
            const int MaxRisk = 9;

            int xOffset = i * tileWidth;
            int yOffset = j * tileHeight;
            int valueIncrement = i + j;

            for (int x = 0; x < tileWidth; x++)
                for (int y = 0; y < tileHeight; y++)
                {
                    int newRisk = tile[x, y] + valueIncrement;
                    if (newRisk > MaxRisk)
                        newRisk %= MaxRisk;
                    risks[x + xOffset, y + yOffset] = newRisk;
                }
        }

        // Assumes that lowest risk paths only go down and to the right (as in the example)
        // Used this to solve Part 1 and to get incorrect answer on Part 2
        private int FindLowestRiskPathDownRight()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (x < width - 1) RiskNextDownRight(riskmap[x, y], x + 1, y);
                    if (y < height - 1) RiskNextDownRight(riskmap[x, y], x, y + 1);
                }

            return riskmap[width - 1, height - 1];
        }

        private void RiskNextDownRight(int r, int x, int y) => riskmap[x, y] = Math.Min(riskmap[x, y], r + risks[x, y]);

        private Queue<(int, int, int)> q;
        private long nodesVisited = 0;

        private int FindLowestRiskPath()
        {
            q = new();
            q.Enqueue((0, 0, 0));

            while (q.Count > 0)
            {
                (int r, int x, int y) = q.Dequeue();
                VisitAllNeighbors(r, x, y);
            }

            Console.WriteLine($"Nodes visted: {nodesVisited} = {height * width} nodes @ {nodesVisited / (height * width)} visits/node");

            return riskmap[width - 1, height -1];
        }

        private void VisitAllNeighbors(int r, int x, int y)
        {
            if (x < width - 1)  RiskNext(r, x + 1, y);
            if (y < height - 1) RiskNext(r, x, y + 1);
            if (x > 0) RiskNext(r, x - 1, y);
            if (y > 0) RiskNext(r, x, y - 1);
        }

        private void RiskNext(int r, int x, int y)
        {
            nodesVisited++;
            int newR = r + risks[x, y];
            if (newR < riskmap[x, y])
            {
                riskmap[x, y] = newR;
                q.Enqueue((riskmap[x, y], x, y));
            }
        }

        [Description("What is the lowest total risk of any path from the top left to the bottom right?")]
        public override string SolvePart1()
        {
            return FindLowestRiskPath().ToString();
        }

        [Description("What is the lowest total risk of any path from the top left to the bottom right?")]
        public override string SolvePart2()
        {
            SetupPart2();

            return FindLowestRiskPath().ToString();
        }
    }
}