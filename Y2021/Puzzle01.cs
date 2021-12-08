using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021
{
    public class Puzzle01 : ASolver
    {
        private List<int> depths;
        public Puzzle01(string input) : base(input) { Name = "Sonar Sweep"; }

        public override void Setup()
        {
            List<string> items = Tools.GetLines(Input);
            depths = new List<int>();
            for (int i = 0; i < items.Count; i++)
                depths.Add(Tools.ParseInt(items[i]));
        }

        [Description("How many measurements are larger than the previous measurement?")]
        public override string SolvePart1()
        {
            int countOfHigherDepths = 0;
            for (int i = 1; i < depths.Count; i++)
            {
                if (depths[i] > depths[i - 1])
                    countOfHigherDepths++;
            }
            return countOfHigherDepths.ToString();
        }

        [Description("How many sums are larger than the previous sum?")]
        public override string SolvePart2()
        {
            int countOfHigherMovingAverage = 0;

            for (int i = 3; i < depths.Count; i++)
            {
                int totalA = depths[i - 3] + depths[i - 2] + depths[i - 1];
                int totalB = depths[i - 2] + depths[i - 1] + depths[i];
                if (totalB > totalA)
                    countOfHigherMovingAverage++;
            }

            return countOfHigherMovingAverage.ToString();
        }
    }
}