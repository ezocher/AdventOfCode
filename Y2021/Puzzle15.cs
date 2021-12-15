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

        private int FindLowestRiskPath()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (x < width  - 1) RiskNext(riskmap[x, y], x + 1, y);
                    if (y < height - 1) RiskNext(riskmap[x, y], x, y + 1);
                }

            return riskmap[width - 1, height -1];
        }

        private void RiskNext(int d, int x, int y)
        {
            int newD = d + risks[x, y];
            riskmap[x, y] = Math.Min(riskmap[x, y], newD);
        }

        [Description("What is the lowest total risk of any path from the top left to the bottom right?")]
        public override string SolvePart1()
        {
            return FindLowestRiskPath().ToString();
        }

        [Description("What is the lowest total risk of any path from the top left to the bottom right?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}