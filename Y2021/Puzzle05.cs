using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021
{
    public class Puzzle05 : ASolver 
    {
        private List<IntLine> lines;
        private int[,] overlapMap;
        private int mapSize;

        public Puzzle05(string input) : base(input) { Name = "Hydrothermal Venture"; }

        public override void Setup()
        {
            List<string> inputLines = Tools.GetLines(Input);

            lines = new List<IntLine>();

            foreach (string inputLine in inputLines)
                lines.Add(new IntLine(inputLine));

            mapSize = MaxCoordinate(lines) + 1;

            overlapMap = new int[mapSize, mapSize];

        }

        [Description("At how many points do at least two lines overlap?")]
        public override string SolvePart1()
        {
            foreach (IntLine line in lines)
            {
                if (line.IsHorizontalOrVertical())
                    AddToOverlapMap(line);
            }

            int countAtLeastTwoOverlaps = ScanOverlapMap(2);

            return countAtLeastTwoOverlaps.ToString();
        }


        [Description("At how many points do at least two lines overlap?")]
        public override string SolvePart2()
        {
            overlapMap = new int[mapSize, mapSize];

            foreach (IntLine line in lines)
                AddToOverlapMap(line);

            int countAtLeastTwoOverlaps = ScanOverlapMap(2);

            return countAtLeastTwoOverlaps.ToString();
        }

        private void AddToOverlapMap(IntLine line)
        {
            int count = line.Length();
            int columnIncrement, rowIncrement;
            int column = line.Start.X;
            int row = line.Start.Y;

            columnIncrement = Tools.LessEqGreater(line.End.X, line.Start.X);
            rowIncrement    = Tools.LessEqGreater(line.End.Y, line.Start.Y);

            for (int i = 0; i < count; i++)
            {
                overlapMap[row, column]++;
                row += rowIncrement;
                column += columnIncrement;
            }
        }

        private int ScanOverlapMap(int thresholdValue)
        {
            int overlapCount = 0;

            for (int row = 0; row < mapSize; row++)
                for (int column = 0; column < mapSize; column++)
                    if (overlapMap[row, column] >= thresholdValue)
                        overlapCount++;

            return overlapCount;
        }

        private static int MaxCoordinate(List<IntLine> lines)
        {
            int max = 0;

            foreach (IntLine line in lines)
                max = Tools.Max(new int[] { max, line.Start.X, line.Start.Y, line.End.X, line.End.Y } );

            return max;
        }

    }
}