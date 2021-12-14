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
        private StringBuilder polymerSB;
        private Dictionary<string, long> polymerByPairs;

        private Dictionary<string, char> pairInsertionRules;

        private Dictionary<char, long> charFrequencyPBP;

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

        private Dictionary<string, long> InitializePBP(string starting)
        {
            Dictionary<string, long> polymer = new Dictionary<string, long>();

            for (int i = 0; i < starting.Length - 1; i++)
            {
                string pair = starting.Substring(i, 2);
                if (polymer.ContainsKey(pair))
                    polymer[pair]++;
                else
                    polymer.Add(pair, 1);
                if (i != starting.Length - 2)
                    DuplicatedChar(pair[1], 1);
            }

            return polymer;
        }

        private void DuplicatedChar(char c, long count)
        {
            if (charFrequencyPBP.ContainsKey(c))
                charFrequencyPBP[c] -= count;
            else
                charFrequencyPBP.Add(c, -count);

        }

        private void PerformInsertionsPBP()
        {
            Dictionary<string, long> newPBP = new Dictionary<string, long>(polymerByPairs.Count * 2);

            foreach (KeyValuePair<string, long> pbp in polymerByPairs)
            {
                long count = pbp.Value;
                char insertChar = pairInsertionRules[pbp.Key];
                DuplicatedChar(insertChar, count);

                string newKey1 = pbp.Key[0].ToString() + insertChar.ToString();
                if (newPBP.ContainsKey(newKey1))
                    newPBP[newKey1] += count;
                else
                    newPBP.Add(newKey1, count);

                string newKey2 = insertChar.ToString() + pbp.Key[1].ToString();
                if (newPBP.ContainsKey(newKey2))
                    newPBP[newKey2] += count;
                else
                    newPBP.Add(newKey2, count);
            }

            polymerByPairs = newPBP;
        }


        private (long mostCommon, long leastCommon) CountElementFrequencyPBP()
        {
            foreach (KeyValuePair<string, long> pbp in polymerByPairs)
            {
                long count = pbp.Value;
                char c1 = pbp.Key[0];
                if (charFrequencyPBP.ContainsKey(c1))
                    charFrequencyPBP[c1] += count;
                else
                    charFrequencyPBP.Add(c1, count);

                char c2 = pbp.Key[1];
                if (charFrequencyPBP.ContainsKey(c2))
                    charFrequencyPBP[c2] += count;
                else
                    charFrequencyPBP.Add(c2, count);
            }

            long most = 0;
            long least = Int64.MaxValue;
            foreach (KeyValuePair<char, long> cf in charFrequencyPBP)
            {
                if (cf.Value > most) most = cf.Value;
                if (cf.Value < least) least = cf.Value;
            }

            return (most, least);
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
            {
                char c = polymerSB[i];
                if (charFrequency.ContainsKey(c))
                    charFrequency[c]++;
                else
                    charFrequency.Add(c, 1);
            }

            int most = 0;
            int least = Int32.MaxValue;
            foreach (KeyValuePair<char, int> cf in charFrequency)
            {
                if (cf.Value > most)  most  = cf.Value;
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

        [Description("What is the answer?")]
        public override string SolvePart2()
        {
            for (int step = 1; step <= 40; step++)
                PerformInsertionsPBP();

            (long mostCommon, long leastCommon) = CountElementFrequencyPBP();

            return (mostCommon - leastCommon).ToString();
        }

    }
}