using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

// Keeping the ALU from Puzzle 24 because it's cute and fun
// Ended up solving Puzzzle 24 a completely different way
namespace AdventOfCode.Y2021
{
    public class Puzzle24 : ASolver 
    {
        public Puzzle24(string input) : base(input) { Name = "Arithmetic Logic Unit"; }

        const int NumInputDigits = 14;

        // Extracted from puzzle24--.txt using ExtractCoefficientsFromPuzzleInput() in Puzzle24Original.cs
        static readonly int[] A = new int[] { 1, 1, 1, 1, 26, 1, 1, 26, 1, 26, 26, 26, 26, 26 };
        static readonly int[] B = new int[] { 12, 11, 10, 10, -16, 14, 12, -4, 15, -7, -8, -4, -15, -8 };
        static readonly int[] C = new int[] { 6, 12, 5, 10, 7, 0, 4, 12, 14, 13, 10, 11, 9, 9 };
        public override void Setup() { }

        Queue<(long, long, int)> candidates;
        bool found;
        long largestSerial;
        private void ReverseSearch(long z, long digits, int stage)
        {
            if (stage == 13)
            {
                for (int w = 9; w >= 1; w--)
                {
                    int z0 = w - B[stage];
                    candidates.Enqueue((z0, w, stage - 1));
                }
            }
        }

        private long NextZ(long z, int w, int stage)
        {
            long z1 = z / A[stage];
            if (w == (z % 26 + B[stage]))
                return z1;
            else
                return z1 * 26 + w + C[stage];
        }

        const int LastStage = 13;
        static long lowestZ;
        static long firstZero;

        private void DepthFirst(int stage, long z, long parent)
        {
            if (firstZero > 0) return;

            for (int w = 9; w >= 1; w--)
            {
                long digits = parent * 10 + w;
                long zNext = NextZ(z, w, stage);
                if (stage == LastStage)
                {
                    if (zNext == 0)
                        firstZero = digits;
                    else if (zNext < lowestZ)
                    {
                        lowestZ = zNext;
                        Console.WriteLine($"New lowest z = {zNext} at input: {digits}");
                    }
                }
                else
                    DepthFirst(stage + 1, zNext, digits);
            }
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart1()
        {
            //lowestZ = long.MaxValue;
            //firstZero = 0;

            //DepthFirst(0, 0, 0);

            found = false;
            candidates = new();
            candidates.Enqueue((0, 13, 0));

            while ((!found) && (candidates.Count > 0))
            {
                (long z, long digits, int stage) = candidates.Dequeue();
                ReverseSearch(z, digits, stage);
            }
            
            return largestSerial.ToString(); //  LargestValidSerialNumber().ToString();
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart2()
        {
            //Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}