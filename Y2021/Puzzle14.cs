using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AdventOfCode.Y2021
{
    public class Puzzle14 : ASolver 
    {
        // Used StringBuilder for Part 1 and character pairs in a dictionary for Part 2
        private StringBuilder polymerSB;
        private Dictionary<string, long> polymerByPairs;
        private Dictionary<char, long> charFrequencyPBP;    // Count characters as they are inserted

        private Dictionary<string, char> pairInsertionRules;

        public Puzzle14(string input) : base(input) { Name = "Extended Polymerization"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            polymerSB = new StringBuilder(lines[0]);

            charFrequencyPBP = new Dictionary<char, long>();
            polymerByPairs = InitializePBP(lines[0]);

            pairInsertionRules = new Dictionary<string, char>();
            for (int i = 2; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split(" -> ");
                pairInsertionRules.Add(parts[0], parts[1][0]);
            }
        }

        private void PerformInsertionsSB()
        {
            int lengthAfterInsertions = polymerSB.Length * 2 - 1;
            for (int i = 0; i < lengthAfterInsertions - 1; i += 2)
            {
                string pair = polymerSB.ToString(i, 2);
                polymerSB.Insert(i + 1, pairInsertionRules[pair]);
            }
        }

        private (int mostCommon, int leastCommon) CountElementFrequencySB()
        {
            Dictionary<char, int> charFrequency = new Dictionary<char, int>();

            for (int i = 0; i < polymerSB.Length; i++)
                charFrequency.InsertOrIncrement(polymerSB[i], 1);

            int most = 0;
            int least = Int32.MaxValue;
            foreach (KeyValuePair<char, int> cf in charFrequency)
            {
                if (cf.Value > most)  most  = cf.Value;
                if (cf.Value < least) least = cf.Value;
            }

            return (most, least);
        }

        private Dictionary<string, long> InitializePBP(string starting)
        {
            Dictionary<string, long> polymerBP = new Dictionary<string, long>();

            for (int i = 0; i < starting.Length - 1; i++)
            {
                string pair = starting.Substring(i, 2);
                polymerBP.InsertOrIncrement(pair, 1);
            }

            // Count characters in starting string
            foreach (char c in starting)
                charFrequencyPBP.InsertOrIncrement(c, 1);

            return polymerBP;
        }

        private void PerformInsertionsPBP()
        {
            Dictionary<string, long> newPBP = new Dictionary<string, long>(polymerByPairs.Count * 2);

            foreach (KeyValuePair<string, long> pbp in polymerByPairs)
            {
                long count = pbp.Value;
                char insertChar = pairInsertionRules[pbp.Key];

                charFrequencyPBP.InsertOrIncrement(insertChar, count);  // Count characters as they're inserted

                string newKey1 = pbp.Key[0].ToString() + insertChar.ToString();
                newPBP.InsertOrIncrement(newKey1, count);

                string newKey2 = insertChar.ToString() + pbp.Key[1].ToString();
                newPBP.InsertOrIncrement(newKey2, count);
            }

            polymerByPairs = newPBP;
        }

        private (long mostCommon, long leastCommon) FindMostLeastPBP()
        {
            long most = 0;
            long least = Int64.MaxValue;

            foreach (KeyValuePair<char, long> cf in charFrequencyPBP)
            {
                if (cf.Value > most) most = cf.Value;
                if (cf.Value < least) least = cf.Value;
            }

            return (most, least);
        }

        [Description("What do you get if you take the quantity of the most common element and subtract the quantity of the least common element?")]
        public override string SolvePart1()
        {
            for (int step = 1; step <= 10; step++)
                PerformInsertionsSB();

            (int mostCommon, int leastCommon) = CountElementFrequencySB();

            return (mostCommon - leastCommon).ToString();
        }

        [Description("What do you get if you take the quantity of the most common element and subtract the quantity of the least common element?")]
        public override string SolvePart2()
        {
            for (int step = 1; step <= 40; step++)
                PerformInsertionsPBP();

            (long mostCommon, long leastCommon) = FindMostLeastPBP();

            return (mostCommon - leastCommon).ToString();
        }
    }
}