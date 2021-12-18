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
            
            bool LIsPair;
            int LeftValue;
            Pair LeftPair;
            bool RIsPair;
            int RightValue;
            Pair RightPair;

            public Pair(string line)
            {
                Console.Write($"{line} ");

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

            public Pair(Pair p, Pair q)
            {
                LIsPair = RIsPair = true;
                LeftValue = RightValue = 0;
                LeftPair = p;
                RightPair = q;
                this.Reduce();
            }

            private void Reduce()
            {
                bool actionApplied;
                do
                {
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
                int propagateLeft, propagateRight;
                return this.CheckExplode(1, out propagateLeft, out propagateRight);
            }

            private bool CheckExplode(int nesting, out int propagateLeft, out int propagateRight)
            {
                propagateLeft = propagateRight = 0;
                bool didExplode = false;

                if (nesting == 5)
                {
                    propagateLeft = LeftValue;
                    propagateRight = RightValue;
                    return true;
                }

                if (this.LIsPair)
                {
                    didExplode = this.LeftPair.CheckExplode(nesting + 1, out propagateLeft, out propagateRight);

                    if (didExplode && (nesting == 4))
                    {
                        this.LeftPair = null;
                        this.LIsPair = false;
                        this.LeftValue = 0;
                    }

                    if (didExplode && !this.RIsPair && (propagateRight != UsedValue))
                    {
                        this.RightValue += propagateRight;
                        propagateRight = UsedValue;
                    }
                }

                if (!didExplode && this.RIsPair)
                {
                    didExplode = this.RightPair.CheckExplode(nesting + 1, out propagateLeft, out propagateRight);

                    if (didExplode && (nesting == 4))
                    {
                        this.RightPair = null;
                        this.RIsPair = false;
                        this.RightValue = 0;
                    }

                    if (didExplode && !this.LIsPair && (propagateLeft != UsedValue))
                    {
                        this.LeftValue += propagateLeft;
                        propagateLeft = UsedValue;
                    }
                }
                return didExplode;
            }

            private bool Split()
            {
                return false;
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

            public Pair Add(Pair q)
            {
                return new Pair("[1,1]");
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
            Pair x = new Pair("[[[[[9,8],1],2],3],4]");
            x.Explode();

            foreach (Pair number in numbers)
            {
                
            }
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