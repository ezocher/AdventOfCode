using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle09 : ASolver 
    {
        private int[,] heightmap;
        private int rows, columns;

        private int MaxDepth = 9;

        public Puzzle09(string input) : base(input) { Name = "Smoke Basin"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            columns = lines[0].Length;
            rows = lines.Count;

            heightmap = new int[rows, columns];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < columns; c++)
                    heightmap[r, c] = Int32.Parse(lines[r][c].ToString());
        }

        private List<int> FindLowPoints()
        {
            List<int> lowPoints = new List<int>();

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < columns; c++)
                    if (LowPointAt(r, c))
                        lowPoints.Add(heightmap[r, c]);

            return lowPoints;
        }

        private bool LowPointAt(int r, int c)
        {
            int checkDepth = heightmap[r, c];

            // Check above (if there is a value above)
            if (r > 0)
                if (!(checkDepth < heightmap[r - 1, c]))
                    return false;

            // Check below
            if (r < (rows - 1))
                if (!(checkDepth < heightmap[r + 1, c]))
                    return false;

            // Check left
            if (c > 0)
                if (!(checkDepth < heightmap[r, c - 1]))
                    return false;

            // Check right
            if (c < (columns - 1))
                if (!(checkDepth < heightmap[r, c + 1]))
                    return false;

            return true;
        }

        [Description("What is the sum of the risk levels of all low points on your heightmap?")]
        public override string SolvePart1()
        {
            List<int> lowPoints = FindLowPoints();
            int totalOfRiskLevels = 0;
            foreach (int lowPoint in lowPoints)
            {
                totalOfRiskLevels += lowPoint + 1;
            }

            return totalOfRiskLevels.ToString();
        }

        [Description("What is the answer?")]
        public override string SolvePart2()
        {
            return string.Empty;
        }
    }
}