using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle18 : ASolver
    {
        List<Pair> numbers;

        struct Element
        {
            public bool IsValue;
            public int Value;
            public Pair P;

            public Element(int v)
            {
                IsValue = true;
                Value = v;
                P = null;
            }

            public Element(Pair p)
            {
                IsValue = false;
                Value = 0;
                P = p;
            }

            public bool IsPair => !IsValue;

            public override string ToString() => (IsValue ? Value.ToString() : P.ToString());
        }

        class Pair
        {
            const char Open = '[';
            const char Close = ']';
            const int MaxValue = 9;

            Element Left;
            Element Right;

            public Pair(string line)
            {
                int centerCommaIndex = -1;
                bool LeftIsPair, RightIsPair;

                // line[0] == Open by definition
                if (line[1] == Open)
                {
                    LeftIsPair = true;
                }
                else
                {
                    LeftIsPair = false;
                    Left = new Element(int.Parse(new string(line[1], 1)));
                    centerCommaIndex = 1 + 1;
                }

                // line[Len - 1] == Close by definition
                if (line[line.Length - 2] == Close)
                {
                    RightIsPair = true;
                }
                else
                {
                    RightIsPair = false;
                    Right = new Element(int.Parse(new string(line[line.Length - 2], 1)));
                    centerCommaIndex = line.Length - 2 - 1;
                }

                if (LeftIsPair && RightIsPair)
                    centerCommaIndex = FindCenterComma(line);

                if (LeftIsPair)
                    Left = new Element(new Pair(line.Substring(1, centerCommaIndex - 1)));

                if (RightIsPair)
                    Right = new Element(new Pair(line.Substring(centerCommaIndex + 1, line.Length - centerCommaIndex - 2)));
            }

            public Pair(int l, int r)
            {
                Left = new Element(l);
                Right = new Element(r);
            }

            public Pair Add(Pair p)
            {
                Pair sum = new Pair(0, 0);
                sum.Left = new Element(this);
                sum.Right = new Element(p);
                sum.Reduce();
                return sum;
            }

            public override string ToString() => ($"[{Left}, {Right}]");

            private int FindCenterComma(string line)
            {
                int index = 1;
                int openCount = 0;

                do
                {
                    if (line[index] == Open)
                        openCount++;
                    else if (line[index] == Close)
                        openCount--;
                    index++;
                } while (openCount > 0);

                return index;
            }

            public void Reduce()
            {
                const bool verbose = false;
                if (verbose) Console.WriteLine($"\nReduce {this}");

                bool actionApplied;
                do
                {
                    if (this.Explode())
                    {
                        actionApplied = true;
                        if (verbose) Console.WriteLine($"Exploded => {this}");
                    }
                    else if (this.Split())
                    {
                        actionApplied = true;
                        if (verbose) Console.WriteLine($"Split => {this}");
                    }
                    else
                        actionApplied = false;

                } while (actionApplied);
            }

            const int UsedValue = -1;

            public bool Explode()
            {
                int propagateLeft = UsedValue;
                int propagateRight = UsedValue;
                bool didExplode = false;
                CheckExplode(1, ref propagateLeft, ref propagateRight, ref didExplode);
                return didExplode;
            }

            private void PropogateRight(ref int propagateRight)
            {
                if (Left.IsPair)
                    Left.P.PropogateRight(ref propagateRight);
                else
                {
                    Left.Value += propagateRight;
                    propagateRight = UsedValue;
                }
            }

            private void PropogateLeft(ref int propagateLeft)
            {
                if (Right.IsPair)
                    Right.P.PropogateLeft(ref propagateLeft);
                else
                {
                    Right.Value += propagateLeft;
                    propagateLeft = UsedValue;
                }
            }

            private void CheckExplode(int nesting, ref int propagateLeft, ref int propagateRight, ref bool didExplode)
            {
                propagateLeft = propagateRight = 0;

                if (nesting == 5)
                {
                    propagateLeft = Left.Value;
                    propagateRight = Right.Value;
                    didExplode = true;
                }

                if (Left.IsPair)
                {
                    Left.P.CheckExplode(nesting + 1, ref propagateLeft, ref propagateRight, ref didExplode);

                    if (didExplode && (nesting == 4))
                        Left = new Element(0);


                    if (didExplode && (propagateRight != UsedValue))
                    {
                        if (Right.IsValue)
                        {
                            Right.Value += propagateRight;
                            propagateRight = UsedValue;
                        }
                        else
                            Right.P.PropogateRight(ref propagateRight);
                    }
                }

                if (!didExplode && Right.IsPair)
                {
                    Right.P.CheckExplode(nesting + 1, ref propagateLeft, ref propagateRight, ref didExplode);

                    if (didExplode && (nesting == 4))
                        Right = new Element(0);

                    if (didExplode && (propagateLeft != UsedValue))
                    {
                        if (Left.IsValue)
                        {
                            Left.Value += propagateLeft;
                            propagateLeft = UsedValue;
                        }
                        else
                            Left.P.PropogateLeft(ref propagateLeft);
                    }
                }
            }

            public bool Split()
            {
                bool didSplit = false;

                if (Left.IsPair)
                {
                    didSplit = Left.P.Split();
                }
                else if (Left.Value > MaxValue)
                {
                    Left = new Element(CreateSplit(Left.Value));
                    didSplit = true;
                    return didSplit;
                }

                if (!didSplit)
                {
                    if (Right.IsPair)
                    {
                        didSplit = Right.P.Split();
                    }
                    else if (Right.Value > MaxValue)
                    {
                        Right = new Element(CreateSplit(Right.Value));
                        didSplit = true;
                        return didSplit;
                    }
                }

                return didSplit;
            }

            private Pair CreateSplit(int i) => new Pair(i / 2, i / 2 + i % 2);

            public long Magnitude()
            {
                long left, right;

                left = (Left.IsValue) ? Left.Value : Left.P.Magnitude();
                right = (Right.IsValue) ? Right.Value : Right.P.Magnitude();

                return (3 * left) + (2 * right);
            }
        }

        public Puzzle18(string input) : base(input) { Name = "Snailfish"; }

        private List<string> lines;

        public override void Setup()
        {
            lines = Tools.GetLines(Input);

            numbers = new();

            foreach (string line in lines)
                numbers.Add(new Pair(line));
        }

        [Description("What is the magnitude of the final sum?")]
        public override string SolvePart1()
        {
            Pair sum = numbers[0].Add(numbers[1]);
            for (int i = 2; i < numbers.Count; i++)
                sum = sum.Add(numbers[i]);

            return sum.Magnitude().ToString();
        }

        [Description("What is the magnitude of the final sum?")]
        public override string SolvePart2()
        {
            long maxMagnitude = 0;

            for (int i = 0; i < lines.Count; i++)
                for (int j = 0; j < lines.Count; j++)
                    if (i != j)
                    {
                        Pair sum = new Pair(lines[i]).Add(new Pair(lines[j]));
                        maxMagnitude = Math.Max(sum.Magnitude(), maxMagnitude);
                    }

            return maxMagnitude.ToString();
        }
    }
}