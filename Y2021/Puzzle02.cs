using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021
{
    public class Puzzle02 : ASolver
    {
        private List<(char, int)> commands;

        public Puzzle02(string input) : base(input) { Name = "Dive!"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);
            commands = new List<(char, int)>();
            foreach (string line in lines)
            {
                int separatorIndex = line.IndexOf(' ');
                commands.Add((line[0], Tools.ParseInt(line, separatorIndex + 1)));
            }
        }

        [Description("What do you get if you multiply your final horizontal position by your final depth?")]
        public override string SolvePart1()
        {
            int horizontalPosition = 0;
            int depth = 0;

            foreach ((char, int) command in commands)
            {
                (char direction, int value) = command;
                switch (direction)
                {
                    case 'f':
                        horizontalPosition += value; break;
                    case 'd':
                        depth += value; break;
                    case 'u':
                        depth -= value; break;
                }
            }
            return (horizontalPosition * depth).ToString();
        }

        [Description("What do you get if you multiply your final horizontal position by your final depth?")]
        public override string SolvePart2()
        {
            int horizontalPosition = 0;
            int depth = 0;
            int aim = 0;

            foreach ((char, int) command in commands)
            {
                (char direction, int value) = command;
                switch (direction)
                {
                    case 'f':
                        horizontalPosition += value;
                        depth += aim * value;
                        break;
                    case 'd':
                        aim += value; break;
                    case 'u':
                        aim -= value; break;
                }
            }
            return (horizontalPosition * depth).ToString();
        }
    }
}