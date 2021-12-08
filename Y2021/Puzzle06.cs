using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021
{
    public class Puzzle06 : ASolver 
    {
        private List<int> lanternfish;
        private const int CountdownAtBirth = 8;
        private const int CountdownAfterGivingBirth = 6;

        public Puzzle06(string input) : base(input) { Name = "Lanternfish"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);
            lanternfish = new List<int>(Parse.LineOfDelimitedInts(lines[0], ","));
        }

        [Description("How many lanternfish would there be after 80 days?")]
        public override string SolvePart1()
        {
            for (int day = 1; day <= 80; day++)
            {
                int existingFishCount = lanternfish.Count;
                for (int i = 0; i < existingFishCount; i++)
                {
                    if (lanternfish[i] == 0)
                    {
                        lanternfish[i] = CountdownAfterGivingBirth;
                        lanternfish.Add(CountdownAtBirth);
                    }
                    else
                        lanternfish[i]--;
               }
            }

            return lanternfish.Count.ToString();
        }

        // Model counts of fish at each age rather than individual fish
        long[] lanternfishPerCountdown;

        [Description("How many lanternfish would there be after 256 days?")]
        public override string SolvePart2()
        {
            Setup();    // Reload starting lanternfish

            // Generate counts per age of starting lanternfish
            lanternfishPerCountdown = new long[CountdownAtBirth + 1];
            foreach (int f in lanternfish)
                lanternfishPerCountdown[f]++;
            
            for (int day = 1; day <= 256; day++)
            {
                long zeroCountdownCount = lanternfishPerCountdown[0];

                for (int countdown = 1; countdown <= CountdownAtBirth; countdown++)
                    lanternfishPerCountdown[countdown - 1] = lanternfishPerCountdown[countdown];

                // Birth new lanternfish and reset parent countdown
                lanternfishPerCountdown[CountdownAtBirth] = zeroCountdownCount;
                lanternfishPerCountdown[CountdownAfterGivingBirth] += zeroCountdownCount;
            }
 
            return Tools.Sum(lanternfishPerCountdown).ToString();
        }
    }
}