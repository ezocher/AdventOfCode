using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle22 : ASolver 
    {
        private bool[,,] cuboids;

        private List<RebootStep> rebootSteps;

        public const int AreaMax = 50;
        public const int Size = AreaMax * 2 + 1;

        private struct IntRange
        {
            public int Low;
            public int High;

            public IntRange(int low, int high)
            {
                Low = low;
                High = high;
            }

            public void Clamp()
            {
                if (Low < -AreaMax)
                    Low = -AreaMax;
                if (High > AreaMax)
                    High = AreaMax;
            }

            public bool OutsideArea() => (High < -AreaMax) || (Low > AreaMax);
        }

        private struct RebootStep
        {
            public IntRange X;
            public IntRange Y;
            public IntRange Z;
            public bool On;

            public RebootStep(IntRange x, IntRange y, IntRange z, bool on)
            {
                X = x; Y = y; Z = z;
                On = on;
            }

            public RebootStep(string line)
            {
                string[] delimiters = new string[] { " ", "x=", "y=", "z=", "..", "," };
                string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                On = (parts[0] == "on");
                X = new IntRange(int.Parse(parts[1]), int.Parse(parts[2]));
                Y = new IntRange(int.Parse(parts[3]), int.Parse(parts[4]));
                Z = new IntRange(int.Parse(parts[5]), int.Parse(parts[6]));
            }

            public bool OutsideArea() => (X.OutsideArea() || Y.OutsideArea() || Z.OutsideArea());

            public void Clamp()
            {
                X.Clamp();
                Y.Clamp();
                Z.Clamp();
            }
        }

        public Puzzle22(string input) : base(input) { Name = "Reactor Reboot"; }

        public override void Setup()
        {

            cuboids = new bool[Size, Size, Size];

            List<string> lines = Tools.GetLines(Input);

            rebootSteps = new();
            foreach (string line in lines)
            {
                RebootStep step = new RebootStep(line);
                if (!step.OutsideArea())
                {
                    step.Clamp();
                    rebootSteps.Add(step);
                }
            }
        }

        private void SetCuboids(RebootStep step)
        {
            for (int x = step.X.Low + AreaMax; x <= step.X.High + AreaMax; x++)
                for (int y = step.Y.Low + AreaMax; y <= step.Y.High + AreaMax; y++)
                    for (int z = step.Z.Low + AreaMax; z <= step.Z.High + AreaMax; z++)
                        cuboids[x, y, z] = step.On;
        }

        private int CountCuboidsOn()
        {
            int count = 0;
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    for (int z = 0; z < Size; z++)
                        if (cuboids[x, y, z]) count++;
            return count;
        }

        [Description("how many cubes are on?")]
        public override string SolvePart1()
        {
            foreach (RebootStep step in rebootSteps)
                SetCuboids(step);

            return CountCuboidsOn().ToString();
        }


        [Description("how many cubes are on?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}