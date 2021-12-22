using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle22 : ASolver 
    {
        private List<RebootStep> rawSteps;

        private struct IntRange
        {
            public int Low;
            public int High;

            public IntRange(int low, int high)
            {
                Low = low; High = high;
            }

            public void Clamp()
            {
                if (Low < -AreaMax)
                    Low = -AreaMax;
                if (High > AreaMax)
                    High = AreaMax;
            }

            public bool OutsideArea() => (High < -AreaMax) || (Low > AreaMax);

            public bool Intersects(IntRange other) =>
                ((other.Low >= Low) && (other.Low <= High)) || ((other.High >= Low) && (other.High <= High)) ||
                ((Low >= other.Low) && (Low <= other.High)) || ((High >= other.Low) && (High <= other.High));

            public bool Contains(IntRange inner) => ((inner.Low >= Low) && (inner.Low <= High) && (inner.High >= Low) && (inner.High <= High));

            public IntRange Intersection(IntRange target) => new IntRange(Math.Max(Low, target.Low), Math.Min(High, target.High));
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
                X.Clamp(); Y.Clamp(); Z.Clamp();
            }
        }

        #region Part 1
        public const int AreaMax = 50;
        public const int Size = AreaMax * 2 + 1;
        private bool[,,] cuboids;
        private List<RebootStep> filteredSteps;

        private void SetCuboids(RebootStep step)
        {
            for (int x = step.X.Low + AreaMax; x <= step.X.High + AreaMax; x++)
                for (int y = step.Y.Low + AreaMax; y <= step.Y.High + AreaMax; y++)
                    for (int z = step.Z.Low + AreaMax; z <= step.Z.High + AreaMax; z++)
                        cuboids[x, y, z] = step.On;
        }

        private int CountOnCuboidsPart1()
        {
            int count = 0;

            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    for (int z = 0; z < Size; z++)
                        if (cuboids[x, y, z]) count++;

            return count;
        }
        #endregion

        #region Part 2
        private class Extent3D
        {
            public IntRange X;
            public IntRange Y;
            public IntRange Z;

            // When two "on" volumes overlap, count them both but add a -1 value volume of their intersection
            // When an "off" volume intersects a volume create an opposite polarity value volume of the intersection
            //   (this means that "off"s of -1 Extents create +1 Extents of the intersection)
            public int Value;

            public Extent3D(IntRange x, IntRange y, IntRange z, int value = 1)
            {
                X = x; Y = y; Z = z;
                Value = value;
            }

            public bool Intersects(Extent3D other) => X.Intersects(other.X) && Y.Intersects(other.Y) && Z.Intersects(other.Z);

            public bool Surrounds(Extent3D inner) => (X.Contains(inner.X) && Y.Contains(inner.Y) && Z.Contains(inner.Z));

            // Create and return a new Extent of opposite value (vs the target) of the volume where the intersector intersects the target
            public Extent3D Intersection(Extent3D target) => new Extent3D(X.Intersection(target.X), Y.Intersection(target.Y), 
                Z.Intersection(target.Z), target.Value * -1);

            public long CountCuboids() => (long)(X.High - X.Low + 1) * (Y.High - Y.Low + 1) * (Z.High - Z.Low + 1) * Value;
        }

        private List<Extent3D> ProcessExtent(List<Extent3D> inputList, RebootStep step)
        {
            Extent3D newExtent = new Extent3D(step.X, step.Y, step.Z);
            List<Extent3D> outputList = new();

            if ((inputList.Count == 0) && step.On)
            {
                outputList.Add(newExtent);
                return outputList;
            }

            foreach (Extent3D existingExtent in inputList)
            {
                if (!newExtent.Surrounds(existingExtent))   // Surrounded Extents of any value get eliminated by not being added to output
                {
                    outputList.Add(existingExtent);
                    if (newExtent.Intersects(existingExtent))
                        outputList.Add(newExtent.Intersection(existingExtent));
                }
            }

            if (step.On)
                outputList.Add(newExtent);

            return outputList;
        }

        private long CountOnCuboidsPart2(List<Extent3D> extentList)
        {
            long count = 0;

            foreach (Extent3D ext in extentList)
                count += ext.CountCuboids();

            return count;
        }

        #endregion

        public Puzzle22(string input) : base(input) { Name = "Reactor Reboot"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            rawSteps = new();
            foreach (string line in lines)
                rawSteps.Add(new RebootStep(line));
        }

        [Description("Considering only cubes in the region x=-50..50,y=-50..50,z=-50..50, how many cubes are on?")]
        public override string SolvePart1()
        {
            cuboids = new bool[Size, Size, Size];

            filteredSteps = new();
            foreach (RebootStep step in rawSteps)
                if (!step.OutsideArea())
                {
                    step.Clamp();
                    filteredSteps.Add(step);
                }

            foreach (RebootStep step in filteredSteps)
                SetCuboids(step);

            return CountOnCuboidsPart1().ToString();
        }

        [Description("Considering all cubes, how many cubes are on?")]
        public override string SolvePart2()
        {
            List<Extent3D> onExtents = new();

            foreach (RebootStep step in rawSteps)
                onExtents = ProcessExtent(onExtents, step);

            return CountOnCuboidsPart2(onExtents).ToString();
        }
    }
}