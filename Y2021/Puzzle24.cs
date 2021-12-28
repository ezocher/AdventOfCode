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

        // Extracted from puzzle24--.txt using ExtractCoefficientsFromPuzzleInput() in Puzzle94.cs
        static readonly int[] A = new int[] { 1, 1, 1, 1, 26, 1, 1, 26, 1, 26, 26, 26, 26, 26 };
        static readonly int[] B = new int[] { 12, 11, 10, 10, -16, 14, 12, -4, 15, -7, -8, -4, -15, -8 };
        static readonly int[] C = new int[] { 6, 12, 5, 10, 7, 0, 4, 12, 14, 13, 10, 11, 9, 9 };
        public override void Setup()
        {
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart1()
        {
            return string.Empty; //  LargestValidSerialNumber().ToString();
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart2()
        {
            //Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}