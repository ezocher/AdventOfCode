using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle07 : ASolver 
    {
        private List<int> crabHPos;
        private int crabMinPos, crabMaxPos;

        public Puzzle07(string input) : base(input) { Name = "The Treachery of Whales"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            crabHPos = new List<int>(Parse.LineOfDelimitedInts(lines[0], ","));

            (crabMinPos, crabMaxPos) = Tools.MinMax(crabHPos);
        }

        [Description("How much fuel must they spend to align to that position?")]
        public override string SolvePart1()
        {
            int bestH;
            int leastTotalFuel = Int32.MaxValue;

            for (int candidateH = crabMinPos; candidateH <= crabMaxPos; candidateH++)
            {
                int totalFuel = 0;

                foreach (int hPos in crabHPos)
                {
                    int distanceMoved = Math.Abs(hPos - candidateH);
                    totalFuel += distanceMoved;
                }

                if (totalFuel < leastTotalFuel)
                {
                    leastTotalFuel = totalFuel;
                    bestH = candidateH;
                }
            }

            return leastTotalFuel.ToString();
        }

        [Description("How much fuel must they spend to align to that position?")]
        public override string SolvePart2()
        {
            int[] moveCost = new int[crabMaxPos + 1];

            int cumulativeCost = 0;
            for (int i = 0; i < moveCost.Length; i++)
            {
                moveCost[i] = cumulativeCost + i;
                cumulativeCost = moveCost[i];
            }

            int bestH;
            int leastTotalFuel = Int32.MaxValue;

            for (int candidateH = crabMinPos; candidateH <= crabMaxPos; candidateH++)
            {
                int totalFuel = 0;

                foreach (int hPos in crabHPos)
                {
                    int distanceMoved = Math.Abs(hPos - candidateH);
                    totalFuel += moveCost[distanceMoved];
                }

                if (totalFuel < leastTotalFuel)
                {
                    leastTotalFuel = totalFuel;
                    bestH = candidateH;
                }
            }

            return leastTotalFuel.ToString();
        }
    }
}