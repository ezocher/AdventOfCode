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
        private List<long> autocompleteScores;

        public Puzzle10(string input) : base(input) { Name = "Syntax Scoring"; }

        public override void Setup()
        {
            lines = Tools.GetLines(Input);
            autocompleteScores = new List<long>();
        }

        private char FindFirstIllegalChar(string line)
        {
            Stack<char> expressionStack = new Stack<char>();

            foreach (char c in line)
            {
                if (IsOpenChar(c))
                    expressionStack.Push(c);
                else
                {
                    char openChar = expressionStack.Pop();
                    if (!MatchingPair(openChar, c))
                        return c;
                }
            }
            return ' ';
        }

        private bool MatchingPair(char openChar, char c)
        {
            switch (openChar)
            {
                case '(':
                    return (c == ')');
                case '[':
                    return (c == ']');
                case '{':
                    return (c == '}');
                case '<':
                    return (c == '>');
                default:
                    return false;
            }
        }

        private bool IsOpenChar(char c) => ("([{<".Contains(c));

        private int ErrorScore(char errorChar)
        {
            switch (errorChar)
            {
                case ')':
                    return 3;
                case ']':
                    return 57;
                case '}':
                    return 1197;
                case '>':
                    return 25137;
                default:
                    return 0;
            }
        }

        private char[] FindIncompleteLine(string line)
        {
            Stack<char> expressionStack = new Stack<char>();

            foreach (char c in line)
            {
                if (IsOpenChar(c))
                    expressionStack.Push(c);
                else
                {
                    char openChar = expressionStack.Pop();
                    if (!MatchingPair(openChar, c))
                        return null;
                }
            }
            return expressionStack.ToArray();
        }

        // completionChars contains the "open" characters that we need to match
        private long CompletionScore(char[] completionChars)
        {
            long score = 0;

            foreach (char c in completionChars)
            {
                score *= 5;
                switch (c)  
                {
                    case '(':
                        score += 1; break;
                    case '[':
                        score += 2; break;
                    case '{':
                        score += 3; break;
                    case '<':
                        score += 4; break;
                    default:
                        break;
                }
            }
            return score;
        }

        [Description("What is the total syntax error score for those errors?")]
        public override string SolvePart1()
        {
            int totalErrorScore = 0;
            foreach (string line in lines)
                totalErrorScore += ErrorScore(FindFirstIllegalChar(line));
            return totalErrorScore.ToString();
        }

        [Description("What is the middle score")]
        public override string SolvePart2()
        {
            foreach (string line in lines)
            {
                char[] completionChars = FindIncompleteLine(line);
                if (completionChars != null)
                    autocompleteScores.Add(CompletionScore(completionChars));
            }

            autocompleteScores.Sort();
            long middleScore = autocompleteScores[autocompleteScores.Count / 2];
            return middleScore.ToString();
        }

    }
}