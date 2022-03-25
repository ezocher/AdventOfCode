using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

// A second/intermediate version of Puzzle 24 comingled with the final solution

namespace AdventOfCode.Y2021
{
    public class SecondVersionPuzzle24 : ASolver 
    {
        public SecondVersionPuzzle24(string input) : base(input) { Name = "Arithmetic Logic Unit"; }

        const int NumInputDigits = 14;

        const int MinW = 1, MaxW = 9;

        // Extracted from puzzle24--.txt using ExtractCoefficientsFromPuzzleInput() in Puzzle24Original.cs
        static readonly int[] A = new int[] {  1,  1,  1,  1,  26,  1,  1, 26,  1, 26, 26, 26,  26, 26 };
        static readonly int[] B = new int[] { 12, 11, 10, 10, -16, 14, 12, -4, 15, -7, -8, -4, -15, -8 };
        static readonly int[] C = new int[] {  6, 12,  5, 10,   7,  0,  4, 12, 14, 13, 10, 11,   9,  9 };
        // In stages where A = 1 and B >= 10, z always increases ("Up stages")
        // In stages where A = 26 and B is negative, z can decrease depending on value of w ("Down stages")
        // There are 7 up stages and 7 down stages, so down stages must always decrease z (TODO: confirm, may only need 6 down)

        // Run from stage 0 up to 13 - No longer used
        private long NextZ(long z, int w, int stage)
        {
            long z1 = z / A[stage];
            if (w == (z % 26 + B[stage]))
                return z1;
            else
                return z1 * 26 + w + C[stage];
        }

        // Run from stage 13 down to stage 0
        private List<(long, int)> PreviousZ(long z, int stage)
        {
            stagesRun++;
            List<(long, int)> pairs = new();

            long adjustedZ, previousZ;

            if (B[stage] > MaxW)
            {
                // Up stage - B doesn't come into play because the if determined that the w == z % 26 + B is always false
                // This always finds no pairs or 1 pair
                adjustedZ = z - C[stage];
                previousZ = adjustedZ / 26;  // this is also * A[stage], but A is always 1 when B > MaxW in my dataset (all datasets?)
                int w = (int)(adjustedZ % 26);
                if ((w >= MinW) && (w <= MaxW))
                    pairs.Add((previousZ, w));
            }
            else
            {
                // Down stage - assume for now that all down stages must be triggered to lower the value of z,
                //   so w == z % 26 + B is always true, which means C doesn't come into play because
                //   C * x is always 0 because x is always 0
                // This always finds 9 pairs
                if (largestFirst)
                    for (int w = MinW; w <= MaxW ; w++)
                    {
                        previousZ = z * 26 + w - B[stage];
                        pairs.Add((previousZ, w));
                    }
                else
                    for (int w = MaxW; w >= MinW; w--)
                    {
                        previousZ = z * 26 + w - B[stage];
                        pairs.Add((previousZ, w));
                    }
            }
            return pairs;
        }

        private void DisplayZWPairs(List<(long, int)> pairs, long z, int stage)
        {
            if (pairs.Count == 0)
                Console.WriteLine($"z = {z}, stage = {stage} - no solutions");
            else if (pairs.Count == 1)
                Console.WriteLine($"z = {z}, stage = {stage} -> previous z = {pairs[0].Item1} when w = {pairs[0].Item2}");
            else
            {
                Console.WriteLine($"z = {z}, stage = {stage} - {pairs.Count} solutions:");
                foreach (var pair in pairs)
                    Console.WriteLine($"    previous z = {pair.Item1} when w = {pair.Item2}");
            }
        }

        Stack<(string, long, int)> candidates; // digits, z, nextStage
        bool found;
        string firstSolution;

        public override void Setup() { }

        private void ReverseSearch(string digits, long z, int stage)
        {
            List<(long, int)> zwPairs;
            zwPairs = PreviousZ(z, stage);

            // DisplayZWPairs(zwPairs, z, stage);

            foreach (var zwPair in zwPairs)
            {
                (long previousZ, int w) = zwPair;

                if (stage == 0)
                {
                    if (previousZ == 0)
                    {
                        found = true;
                        firstSolution = w.ToString() + digits;
                        Console.WriteLine($"*** solution = {firstSolution}, stages run = {stagesRun}");
                        return;
                    }
                }
                else
                    candidates.Push((w.ToString() + digits, previousZ, stage - 1));
            }
        }

        // No longer used
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

        private bool largestFirst;
        private long stagesRun;

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart1()
        {
            found = false;
            candidates = new();
            largestFirst = true;
            stagesRun = 0;

            candidates.Push(("", 0, LastStage));

            while ((!found) && (candidates.Count > 0))
            {
                (string digits, long z, int stage) = candidates.Pop();
                ReverseSearch(digits, z, stage);
            }

            return firstSolution.ToString();
        }

        [Description("What is the smallest model number accepted by MONAD?")]
        public override string SolvePart2()
        {
            found = false;
            candidates = new();
            largestFirst = false;
            stagesRun = 0;

            candidates.Push(("", 0, LastStage));

            while ((!found) && (candidates.Count > 0))
            {
                (string digits, long z, int stage) = candidates.Pop();
                ReverseSearch(digits, z, stage);
            }

            return firstSolution.ToString();
        }
    }
}