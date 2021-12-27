using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle25 : ASolver 
    {
        // Upper left of map is [0,0] and upper right is [0, width-1]
        private CharImage map;
        private bool[,] canMove;
        private int width, height;
        private const char EastCuc = '>';
        private const char SouthCuc = 'v';
        private const char EmptySquare = '.';

        public Puzzle25(string input) : base(input) { Name = "Sea Cucumber"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            width = lines[0].Length;
            height = lines.Count;

            map = new CharImage(lines, 0, width, height);
            canMove = new bool[height, width];
        }

        private void ScanCucumberHerd(char cuc, int deltaX, int deltaY)
        {
            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                    if (map.Data[row, col] == cuc)
                        canMove[row, col] = (LookNext(row, col, deltaX, deltaY) != EmptySquare);
        }

        private int MoveCucumberHerd(char cuc, int deltaX, int deltaY)
        {
            int movedCount = 0;
            for (int srcRow = 0; srcRow < height; srcRow++)
                for (int srcCol = 0; srcCol < width; srcCol++)
                    if (canMove[srcRow, srcCol])
                    {
                        map.Data[srcRow, srcCol] = EmptySquare;
                        (int destRow, int destCol) = WrappedDelta(srcRow, srcCol, deltaX, deltaY);
                        map.Data[destRow, destCol] = cuc;
                        movedCount++;
                    }
            return movedCount;
        }

        private (int, int) WrappedDelta(int r, int c, int deltaX, int deltaY)
        {
            int wrapX = (r + deltaX > height - 1) ? 0 : r + deltaX;
            int wrapY = (c + deltaY > width - 1) ? 0 : c + deltaY;
            return (wrapX, wrapY);
        }

        private char LookNext(int r, int c, int deltaX, int deltaY)
        {
            (int x, int y) = WrappedDelta(r, c, deltaX, deltaY);
            return map.Data[x, y];
        }

        private int MoveCucumbers()
        {
            int movedCount = 0;
            ResetCanMove();
            ScanCucumberHerd(EastCuc, 0, 1);
            movedCount += MoveCucumberHerd(EastCuc, 0, 1);

            ResetCanMove();
            ScanCucumberHerd(SouthCuc, 1, 0);
            movedCount += MoveCucumberHerd(SouthCuc, 1, 0);

            return movedCount;
        }


        private void ResetCanMove()
        {
            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                    canMove[row, col] = false;
        }

        [Description("What is the first step on which no sea cucumbers move?")]
        public override string SolvePart1()
        {
            int step = 0;

            map.Write();
            while (MoveCucumbers() > 0)
            {
                step++;
                map.Write();
            }

            return step.ToString(); ;
        }


        [Description("What is the first step on which no sea cucumbers move?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}