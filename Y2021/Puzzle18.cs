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

        class Pair
        {
            const char Open =  '[';
            const char Close = ']';
            const int MaxValue = 9;
            
            bool LIsPair;
            int LeftValue;
            Pair LeftPair;
            bool RIsPair;
            int RightValue;
            Pair RightPair;

            public Pair(string line)
            {
                int centerCommaIndex = -1;

                // line[0] == Open by definition
                if (line[1] == Open)
                {
                    LIsPair = true;
                    LeftValue = 0;
                }
                else
                {
                    LIsPair = false;
                    LeftValue = int.Parse(new string(line[1], 1));
                    LeftPair = null;
                    centerCommaIndex = 1 + 1;
                }

                // line[Len - 1] == Close by definition
                if (line[line.Length - 2] == Close)
                {
                    RIsPair = true;
                    RightValue = 0;
                }
                else
                {
                    RIsPair = false;
                    RightValue = int.Parse(new string(line[line.Length - 2], 1));
                    RightPair = null;
                    centerCommaIndex = line.Length - 2 - 1;
                }

                if (LIsPair && RIsPair)
                    centerCommaIndex = FindCenterComma(line);

                if (LIsPair)
                    LeftPair = new Pair(line.Substring(1, centerCommaIndex - 1));

                if (RIsPair)
                    RightPair = new Pair(line.Substring(centerCommaIndex + 1, line.Length - centerCommaIndex - 2));
            }

            public Pair(int l, int r)
            {
                LIsPair = RIsPair = false;
                LeftValue = l;
                RightValue = r;
                LeftPair = RightPair = null;
            }


            public Pair Add(Pair p)
            {
                Pair sum = new Pair(0, 0);
                sum.LIsPair = sum.RIsPair = true;
                sum.LeftPair = this;
                sum.RightPair = p;
                sum.Reduce();
                return sum;
            }

            public override string ToString()
            {
                string lString = LIsPair ? LeftPair.ToString() : LeftValue.ToString();
                string rString = RIsPair ? RightPair.ToString() : RightValue.ToString();
                return new string("[" + lString + "," + rString + "]");
            }

            public void Reduce()
            {
                bool actionApplied;
                do
                {
                    //Console.WriteLine(this);
                    if (this.Explode())
                        actionApplied = true;
                    else if (this.Split())
                        actionApplied = true;
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
                this.CheckExplode(1, ref propagateLeft, ref propagateRight, ref didExplode);
                return didExplode;
            }

            private void PropogateRight(ref int propagateRight)
            {
                if (this.LIsPair)
                    this.LeftPair.PropogateRight(ref propagateRight);
                else
                {
                    this.LeftValue += propagateRight;
                    propagateRight = UsedValue;
                }
            }

            private void PropogateLeft(ref int propagateLeft)
            {
                if (this.RIsPair)
                    this.RightPair.PropogateLeft(ref propagateLeft);
                else
                {
                    this.RightValue += propagateLeft;
                    propagateLeft = UsedValue;
                }
            }

            private void CheckExplode(int nesting, ref int propagateLeft, ref int propagateRight, ref bool didExplode)
            {
                propagateLeft = propagateRight = 0;

                if (nesting == 5)
                {
                    propagateLeft = LeftValue;
                    propagateRight = RightValue;
                    didExplode = true;
                }

                if (this.LIsPair)
                {
                    this.LeftPair.CheckExplode(nesting + 1, ref propagateLeft, ref propagateRight, ref didExplode);

                    if (didExplode && (nesting == 4))
                    {
                        this.LeftPair = null;
                        this.LIsPair = false;
                        this.LeftValue = 0;
                    }

                    if (didExplode && (propagateRight != UsedValue))
                    {
                        if (!this.RIsPair)
                        {
                            this.RightValue += propagateRight;
                            propagateRight = UsedValue;
                        }
                        else
                            this.RightPair.PropogateRight(ref propagateRight);
                    }
                }

                if (!didExplode && this.RIsPair)
                {
                    this.RightPair.CheckExplode(nesting + 1, ref propagateLeft, ref propagateRight, ref didExplode);

                    if (didExplode && (nesting == 4))
                    {
                        this.RightPair = null;
                        this.RIsPair = false;
                        this.RightValue = 0;
                    }

                    if (didExplode && (propagateLeft != UsedValue))
                    {
                        if (!this.LIsPair)
                        {
                            this.LeftValue += propagateLeft;
                            propagateLeft = UsedValue;
                        }
                        else
                            this.LeftPair.PropogateLeft(ref propagateLeft);
                    }
                }
            }

            public bool Split()
            {
                bool didSplit = false;

                if (this.LIsPair)
                {
                    didSplit = this.LeftPair.Split();
                }
                else if (LeftValue > MaxValue)
                {
                    this.LIsPair = true;
                    this.LeftPair = CreateSplit(LeftValue);
                    didSplit = true;
                    return didSplit;
                }

                if (!didSplit && this.RIsPair)
                {
                    didSplit = this.RightPair.Split();
                }
                else if (RightValue > MaxValue)
                {
                    this.RIsPair = true;
                    this.RightPair = CreateSplit(RightValue);
                    didSplit = true;
                    return didSplit;
                }

                return didSplit;
            }

            private Pair CreateSplit(int i)
            {
                return new Pair(i / 2, i / 2 + i % 2);
            }

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

            public long Magnitude()
            {
                long mag = 0;

                return mag;
            }
        }



        public Puzzle18(string input) : base(input) { Name = "Snailfish"; }

        public override void Setup()
        {

            List<string> lines = Tools.GetLines(Input);

            numbers = new();

            foreach (string line in lines)
                numbers.Add(new Pair(line));
        }

        [Description("What is the magnitude of the final sum?")]
        public override string SolvePart1()
        {
            //Pair sum = numbers[0].Add(numbers[1]);
            //Console.WriteLine(sum);
            //for (int i = 2; i < numbers.Count; i++)
            //{
            //    sum = sum.Add(numbers[i]);
            //    Console.WriteLine(sum);
            //}

            Pair x = new Pair("[[[[[7,0],[7,7]],[[7,7],[7,8]]],[[[7,7],[8,8]],[[7,7],[8,7]]]],[7,[5,[[3, 8],[1, 4]]]]]");
            x.Reduce();

            return string.Empty;
        }

        [Description("What is the magnitude of the final sum?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}