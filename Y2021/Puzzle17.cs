using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle17 : ASolver 
    {
        public int xLeft, xRight, yBottom, yTop;

        public Puzzle17(string input) : base(input) { Name = "Trick Shot"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            string[] parts = lines[0].Split(new char[] { '=', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
            xLeft = int.Parse(parts[1]);
            xRight = int.Parse(parts[2]);
            yBottom = int.Parse(parts[4]);
            yTop = int.Parse(parts[5]);
        }

        bool TestHit(int x, int y) => ((x >= xLeft) && (x <= xRight) && (y >= yBottom) && (y <= yTop));

        public (bool, int) FireShot(int vx, int vy)
        {
            bool hit = false;
            int maxY = 0;
            int x = 0;
            int y = 0;

            do
            {
                x += vx;
                vx = (vx > 0) ? vx - 1 : 0;
                y += vy--;
                hit = TestHit(x, y);
                if (y > maxY) maxY = y;
            } while ((!hit) && (y >= yBottom) && (x <= xRight));
            return (hit, maxY);
        }

        private static double XVelocityToReachTarget(int x) => ((Math.Sqrt((8.0 * (double)x) + 1.0) - 1.0) / 2.0);

        [Description("What is the highest y position it reaches on this trajectory?")]
        public override string SolvePart1()
        {
            bool verbose = true;

            int minVX = (int)Math.Truncate(XVelocityToReachTarget(xLeft)) + 1;
            int maxVX = (int)Math.Truncate(XVelocityToReachTarget(xRight));
            if (verbose) Console.WriteLine($"Possible X velocity: {minVX}..{maxVX}");

            int highestY = 0;
            bool hit;
            int y;

            for (int vX = minVX; vX <= maxVX; vX++)
                for (int vY = 1; vY < 400; vY++)
                {
                    (hit, y) = FireShot(vX, vY);
                    if (verbose) Runner.Write($"Shot {vX}, {vY} {hit}", hit ? ConsoleColor.White : ConsoleColor.Red);
                    if ((hit) && (y > highestY))
                    {
                        highestY = y;
                        if (verbose) Runner.WriteLine($" - new high = {y}", ConsoleColor.Green);
                    }
                    else
                        if (verbose) Runner.WriteLine();
                }

            return highestY.ToString();
        }

        [Description("What is the highest y position it reaches on this trajectory?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}