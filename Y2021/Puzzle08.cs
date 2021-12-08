using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle08 : ASolver 
    {
        private class ActiveWires
        {
            public HashSet<char> Wires;

            public ActiveWires(string characterCodeString)
            {
                Wires = new HashSet<char>();
                foreach (char c in characterCodeString)
                    Wires.Add(c);
            }
        }

        private class MalfunctioningDisplay
        {
            public const int Segments = 7;
            public const int Numerals = 10;

            public const int NumSignalPatterns = 10;
            public const int NumDigits = 4;

            //                                                           0  1  2  3  4  5  6  7  8  9
            public static int[] segmentsPerNumeral = new int[Numerals] { 6, 2, 5, 5, 4, 5, 6, 3, 7, 6 };

            public static List<List<int>> numeralsForNumSegments = new List<List<int>>()
            {
                new List<int>{ },
                new List<int>{ },
                new List<int>{ 1 },
                new List<int>{ 7 },
                new List<int>{ 4 },
                new List<int>{ 2, 3, 5 },
                new List<int>{ 0, 6, 9 },
                new List<int>{ 8 }
            };

            // Segment numbers
            //   0000
            //  1    2
            //  1    2
            //   3333
            //  4    5
            //  4    5
            //   6666
            public static List<List<int>> numeralSegments = new List<List<int>>()
            {
                new List<int> { 0, 1, 2, 4, 5, 6 },
                new List<int> { 2, 5 },
                new List<int> { 0, 2, 3, 4, 6 },
                new List<int> { 0, 2, 3, 5, 6 },
                new List<int> { 1, 2, 3, 5 },
                new List<int> { 0, 1, 3, 5, 6 },
                new List<int> { 0, 1, 3, 4, 5, 6 },
                new List<int> { 0, 2, 5 },
                new List<int> { 0, 1, 2, 3, 4, 5, 6 },                
                new List<int> { 0, 1, 2, 3, 5, 6 }
            };

            public ActiveWires[] UniqueSignalPatterns;
            public ActiveWires[] Digits;
            public string[] DigitStrings;
            public List<ActiveWires>[] SignalPatternsByNumSegments;
            public char[] SegmentMap;
            public Dictionary<string, int> NumeralMap;

            public MalfunctioningDisplay(string inputLine)
            {
                string[] parts = inputLine.Split(" | ");
                string[] inputSignalPatterns = parts[0].Split(" ");
                string[] inputDigits = parts[1].Split(" ");

                UniqueSignalPatterns = new ActiveWires[NumSignalPatterns];
                for (int i = 0; i < NumSignalPatterns; i++)
                {
                    UniqueSignalPatterns[i] = new ActiveWires(inputSignalPatterns[i]);
                }

                SignalPatternsByNumSegments = new List<ActiveWires>[Segments + 1];
                for (int i = 0; i < Segments + 1; i++)
                {
                    SignalPatternsByNumSegments[i] = new List<ActiveWires>();
                }
                for (int i = 0; i < NumSignalPatterns; i++)
                {
                    SignalPatternsByNumSegments[UniqueSignalPatterns[i].Wires.Count].Add(UniqueSignalPatterns[i]);
                }

                Digits = new ActiveWires[NumDigits];
                for (int i = 0; i < NumDigits; i++)
                {
                    Digits[i] = new ActiveWires(inputDigits[i]);
                }

                DigitStrings = new string[NumDigits];
                for (int i = 0; i < NumDigits; i++)
                {
                        char[] d = inputDigits[i].ToCharArray();
                        Array.Sort(d);
                        DigitStrings[i] = new string(d);
                }

                SegmentMap = new char[Segments];
                NumeralMap = new Dictionary<string, int>();
            }

            public int CountDigitsWithUniqueSegments()
            {
                int count = 0;

                foreach (ActiveWires digit in Digits)
                {
                    if (numeralsForNumSegments[digit.Wires.Count].Count == 1)
                        count++;
                }

                return count;
            }

            // Segment numbers
            //   0000
            //  1    2
            //  1    2
            //   3333
            //  4    5
            //  4    5
            //   6666
            // Assumes list of provided scrambled digits includes an example of each possible numeral
            public void SolveSegmentMap()
            {
                // Start with numerals we know
                HashSet<char> oneNumeral = SignalPatternsByNumSegments[2][0].Wires;
                HashSet<char> sevenNumeral = SignalPatternsByNumSegments[3][0].Wires;
                HashSet<char> fourNumeral = SignalPatternsByNumSegments[4][0].Wires;
                HashSet<char> eightNumeral = SignalPatternsByNumSegments[7][0].Wires;

                // Character appearing in 7 but not in 1 must be top segment
                HashSet<char> segmentTop = new HashSet<char>(sevenNumeral);
                segmentTop.ExceptWith(oneNumeral);
                SegmentMap[0] = GetOnly<char>(segmentTop);

                // Characters appearing in all of 2, 3, and 5 are the three crossbars
                HashSet<char> crossbarSegments = new HashSet<char>(SignalPatternsByNumSegments[5][0].Wires);
                for (int i = 1; i < SignalPatternsByNumSegments[5].Count; i++)
                    crossbarSegments.IntersectWith(SignalPatternsByNumSegments[5][i].Wires);

                // Character in crossbars and in 4 must be middle segment
                HashSet<char> segmentMiddle = new HashSet<char>(fourNumeral);
                segmentMiddle.IntersectWith(crossbarSegments);
                SegmentMap[3] = GetOnly<char>(segmentMiddle);

                // Remove top and middle from crossbars to find bottom
                HashSet<char> segmentBottom = new HashSet<char>(crossbarSegments);
                segmentBottom.ExceptWith(segmentTop);
                segmentBottom.ExceptWith(segmentMiddle);
                SegmentMap[6] = GetOnly<char>(segmentBottom);

                // Remove 1 and middle from 4 to find top left
                HashSet<char> segmentTopLeft = new HashSet<char>(fourNumeral);
                segmentTopLeft.ExceptWith(oneNumeral);
                segmentTopLeft.ExceptWith(segmentMiddle);
                SegmentMap[1] = GetOnly<char>(segmentTopLeft);

                // Use TopLeft segment to find numeral 5
                int j = 0;
                while (!SignalPatternsByNumSegments[5][j].Wires.Contains(SegmentMap[1]))
                    j++;
                HashSet<char> fiveNumeral = SignalPatternsByNumSegments[5][j].Wires;

                // Remove topLeft and crossbars from 5 to find bottom right
                HashSet<char> segmentBottomRight = new HashSet<char>(fiveNumeral);
                segmentBottomRight.ExceptWith(crossbarSegments);
                segmentBottomRight.ExceptWith(segmentTopLeft);
                SegmentMap[5] = GetOnly<char>(segmentBottomRight);

                // Remove bottom right from 1 to find top right
                HashSet<char> segmentTopRight = new HashSet<char>(oneNumeral);
                segmentTopRight.ExceptWith(segmentBottomRight);
                SegmentMap[2] = GetOnly<char>(segmentTopRight); 

                // Bottom left is 8 with 5 and top right removed
                HashSet<char> segmentBottomLeft = new HashSet<char>(eightNumeral);
                segmentBottomLeft.ExceptWith(fiveNumeral);
                segmentBottomLeft.ExceptWith(segmentTopRight);
                SegmentMap[4] = GetOnly<char>(segmentBottomLeft);
            }

            public void GenerateNumeralMap()
            {
                for (int i = 0; i < Numerals; i++)
                {
                    List<char> numeralChars = new List<char>();
                    foreach (int ci in numeralSegments[i])
                        numeralChars.Add(SegmentMap[ci]);
                    char[] chars = new char[numeralChars.Count];
                    int j = 0;
                    foreach (char c in numeralChars) chars[j++] = c;
                    Array.Sort(chars);
                    NumeralMap.Add(new string(chars), i);
                }
            }

            public int DecodeDigits()
            {
                int total = 0;

                foreach (string d in DigitStrings)
                {
                    total *= Numerals;
                    int digit;
                    if (!NumeralMap.TryGetValue(d, out digit))
                        throw new Exception($"Digit lookup failed for '{d}'");
                    total += digit;
                }

                return total;
            }

            private T GetOnly<T>(HashSet<T> set)
            {
                T retval = default;
                if (set.Count != 1)
                    throw new Exception($"Expecting 1 member in set where there are {set.Count}");
                foreach (T element in set)
                    retval = element;     
                return retval;
            }
        }

        private List<MalfunctioningDisplay> displays;

        public Puzzle08(string input) : base(input) { Name = "Seven Segment Search"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            displays = new List<MalfunctioningDisplay>();

            foreach (string line in lines)
                displays.Add(new MalfunctioningDisplay(line));
        }

        [Description("In the output values, how many times do digits 1, 4, 7, or 8 appear?")]
        public override string SolvePart1()
        {
            int countDigitsWithUniqueNumberOfSegments = 0;

            foreach (MalfunctioningDisplay display in displays)
            {
                countDigitsWithUniqueNumberOfSegments += display.CountDigitsWithUniqueSegments();
            }

            return countDigitsWithUniqueNumberOfSegments.ToString();
        }

        [Description("What do you get if you add up all of the output values?")]
        public override string SolvePart2()
        {
            int digitsTotal = 0;

            foreach (MalfunctioningDisplay display in displays)
            {
                display.SolveSegmentMap();
                display.GenerateNumeralMap();
                digitsTotal += display.DecodeDigits();
            }

            return digitsTotal.ToString();
        }
    }
}