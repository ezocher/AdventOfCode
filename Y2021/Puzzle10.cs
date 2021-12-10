using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle10 : ASolver 
    {
        private List<string> lines;

        private const string OpenChars = "([{<";
        private const char NoError = ' ';

        private enum ParseResult { WellFormed, Corrupted, Incomplete }

        private readonly Dictionary<char, int> errorScores = new Dictionary<char, int>() { { NoError, 0 }, { ')', 3 }, { ']', 57 }, { '}', 1197 }, { '>', 25137 } };
        private readonly Dictionary<char, int> completionScores = new Dictionary<char, int>() { { '(', 1 }, { '[', 2 }, { '{', 3 }, { '<', 4 } };
        private readonly Dictionary<char, char> matchingPairs = new Dictionary<char, char>() { { '(', ')' }, { '[', ']' }, { '{', '}' }, { '<', '>' } };

        public Puzzle10(string input) : base(input) { Name = "Syntax Scoring"; }

        public override void Setup()
        {
            lines = Tools.GetLines(Input);
        }

        private ParseResult ParseLine(string line, out char errorChar, out char[] incompleteChars)
        {
            Stack<char> expressionStack = new Stack<char>();
            errorChar = NoError;
            incompleteChars = null;

            foreach (char c in line)
            {
                if (OpenChars.Contains(c))
                    expressionStack.Push(c);
                else
                {
                    char openChar = expressionStack.Pop();
                    if (!MatchingPair(openChar, c))
                    {
                        errorChar = c;
                        return ParseResult.Corrupted;
                    }
                }
            }

            if (expressionStack.Count > 0)
            {
                incompleteChars = expressionStack.ToArray();
                return ParseResult.Incomplete;
            }
            else
                return ParseResult.WellFormed;
        }

        private bool MatchingPair(char openChar, char c) => (c == matchingPairs[openChar]);

        private int ErrorScore(char errorChar) => errorScores[errorChar];

        private long CompletionScore(char[] completionChars)
        {
            long score = 0;
            foreach (char c in completionChars)
                score = (score * 5) + completionScores[c];
            return score;
        }

        [Description("What is the total syntax error score for those errors?")]
        public override string SolvePart1()
        {
            char errorChar; char[] completionChars;
            int totalErrorScore = 0;

            foreach (string line in lines)
                if (ParseLine(line, out errorChar, out completionChars) == ParseResult.Corrupted)
                    totalErrorScore += ErrorScore(errorChar);

            return totalErrorScore.ToString();
        }

        [Description("What is the middle score")]
        public override string SolvePart2()
        {
            char errorChar; char[] completionChars;
            List<long> autocompleteScores = new List<long>();

            foreach (string line in lines)
                if (ParseLine(line, out errorChar, out completionChars) == ParseResult.Incomplete)
                    autocompleteScores.Add(CompletionScore(completionChars));

            autocompleteScores.Sort();
            long middleScore = autocompleteScores[autocompleteScores.Count / 2];
            return middleScore.ToString();
        }

    }
}