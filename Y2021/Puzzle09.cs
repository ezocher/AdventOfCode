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
        private int rows, cols;

        private const int MaxDepth = 9;

        public Puzzle09(string input) : base(input) { Name = "Smoke Basin"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            cols = lines[0].Length;
            rows = lines.Count;

            heightmap = new int[rows, cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    heightmap[r, c] = Int32.Parse(lines[r][c].ToString());
        }

        private List<int> FindLowPointDepths()
        {
            List<int> lowPointDepths = new();

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    if (LowPointAt(r, c))
                        lowPointDepths.Add(heightmap[r, c]);

            return lowPointDepths;
        }

        private List<(int, int)> FindLowPointLocations()
        {
            List<(int, int)> lowPoints = new();

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    if (LowPointAt(r, c))
                        lowPoints.Add((r, c));

            return lowPoints;
        }


        private bool LowPointAt(int r, int c)
        {
            int depth = heightmap[r, c];

            // Check above (if there is a value above)
            if (r > 0)
                if (!(depth < heightmap[r - 1, c])) return false;

            // Check below
            if (r < (rows - 1))
                if (!(depth < heightmap[r + 1, c])) return false;

            // Check left
            if (c > 0)
                if (!(depth < heightmap[r, c - 1])) return false;

            // Check right
            if (c < (cols - 1))
                if (!(depth < heightmap[r, c + 1])) return false;

            return true;
        }

        private List<int> FindBasinSizes()
        {
            List<int> basinSizes = new();
            List<(int, int)> lowPoints = FindLowPointLocations();

            foreach ((int r, int c) in lowPoints)
                basinSizes.Add(BasinSize(r, c));

            return basinSizes;
        }

        private int BasinSize(int row, int column)
        {
            Queue<(int, int)> locations = new();
            locations.Enqueue((row, column));

            // Copy heightmap - basin size is determined with destructive flood fill
            int[,] basinMap = (int[,])heightmap.Clone();

            int accumulatedSize = 0;

            while (locations.Count > 0)
            {
                (int r, int c) = locations.Dequeue();
                if (basinMap[r, c] < MaxDepth)
                {
                    accumulatedSize++;
                    basinMap[r, c] = MaxDepth;

                    if (r > 0)          locations.Enqueue(( r - 1, c     ));
                    if (r < (rows - 1)) locations.Enqueue(( r + 1, c     ));
                    if (c > 0)          locations.Enqueue(( r,     c - 1 ));
                    if (c < (cols - 1)) locations.Enqueue(( r,     c + 1 ));
                }
            }

            return accumulatedSize;
        }

        [Description("What is the sum of the risk levels of all low points on your heightmap?")]
        public override string SolvePart1()
        {
            List<int> lowPoints = FindLowPointDepths();

            int totalOfRiskLevels = 0;
            foreach (int lowPoint in lowPoints)
                totalOfRiskLevels += lowPoint + 1;

            return totalOfRiskLevels.ToString();
        }

        [Description("What do you get if you multiply together the sizes of the three largest basins?")]
        public override string SolvePart2()
        {
            List<int> basinSizes = FindBasinSizes();
            basinSizes.Sort();
            basinSizes.Reverse();

            return (basinSizes[0] * basinSizes[1] * basinSizes[2]).ToString();
        }
    }
}