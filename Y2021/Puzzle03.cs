using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021
{
    public class Puzzle03 : ASolver 
    {
        private List<string> numbers;
        private int binaryDigits;
        public Puzzle03(string input) : base(input) { Name = "Binary Diagnostic"; }

        public override void Setup()
        {
            numbers = Tools.GetLines(Input);
            binaryDigits = numbers[0].Length;
        }

        [Description("What is the power consumption of the submarine?")]
        public override string SolvePart1()
        {
            int[] countOfOnes = new int[binaryDigits];

            foreach (string number in numbers)
            {
                for (int i = 0; i < binaryDigits; i++)
                    if (number[i] == '1')
                        countOfOnes[i]++;
            }

            string gammaBinary = "";
            string epsilonBinary = "";

            for (int column = 0; column < binaryDigits; column++)
            {
                if (countOfOnes[column] > (numbers.Count - countOfOnes[column]))
                {
                    gammaBinary += "1";
                    epsilonBinary += "0";
                }
                else
                {
                    gammaBinary += "0";
                    epsilonBinary += "1";
                }
            }

            int gammaRate = Convert.ToInt32(gammaBinary, 2);
            int epsilonRate = Convert.ToInt32(epsilonBinary, 2);

            return (gammaRate * epsilonRate).ToString();
        }

        [Description("What is the life support rating of the submarine?")]
        public override string SolvePart2()
        {
            List<string> oxygenNumbers = new(numbers);
            List<string> co2Numbers = new(numbers);

            for (int digitNumber = 0; digitNumber < binaryDigits; digitNumber++)
            {
                int countOfOnes = CountColumnOfOnes(oxygenNumbers, digitNumber);
                char keepDigit;
                if (countOfOnes >= (oxygenNumbers.Count - countOfOnes))
                    keepDigit = '1';
                else
                    keepDigit = '0';

                for (int i = 0; i < oxygenNumbers.Count; i++)
                    if (oxygenNumbers[i][digitNumber] != keepDigit)
                        oxygenNumbers.RemoveAt(i--);

                if (oxygenNumbers.Count == 1)
                    break;
            }

            for (int digitNumber = 0; digitNumber < binaryDigits; digitNumber++)
            {
                int countOfOnes = CountColumnOfOnes(co2Numbers, digitNumber);
                char keepDigit;
                if (countOfOnes < (co2Numbers.Count - countOfOnes))
                    keepDigit = '1';
                else
                    keepDigit = '0';

                for (int i = 0; i < co2Numbers.Count; i++)
                    if (co2Numbers[i][digitNumber] != keepDigit)
                        co2Numbers.RemoveAt(i--);

                if (co2Numbers.Count == 1)
                    break;
            }

            int oxygenGeneratorRating = Convert.ToInt32(oxygenNumbers[0], 2);
            int co2ScrubberRating = Convert.ToInt32(co2Numbers[0], 2);

            return (oxygenGeneratorRating * co2ScrubberRating).ToString();
        }

        private static int CountColumnOfOnes(List<string> numbers, int digit)
        {
            int count = 0;

            foreach (string number in numbers)
                if (number[digit] == '1')
                    count++;

            return count;
        }
    }
}