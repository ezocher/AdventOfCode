using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle21 : ASolver
    {
        private Player player1, player2;
        private Die die;
        const int BoardSpaces = 10;

        private struct Player
        {
            public int Position;
            public int Score;

            public Player(int startingPosition)
            {
                Position = startingPosition;
                Score = 0;
            }

            public bool Win()
            {
                return (Score >= 1000);
            }

            public bool Move(int spaces)
            {
                int newSpace = (Position + spaces) % BoardSpaces;
                if (newSpace == 0) newSpace = BoardSpaces;
                Score += newSpace;
                Position = newSpace;
                return this.Win();
            }
        }

        private class Die
        {
            private int NextRoll;
            public int Rolls;

            public Die()
            {
                NextRoll = 1;
                Rolls = 0;
            }

            public int GetRoll()
            {
                Rolls++;
                int roll = NextRoll;
                NextRoll++;
                if (NextRoll > 100)
                    NextRoll = 1;
                return roll;
            }
        }

        public Puzzle21(string input) : base(input) { Name = "Dirac Dice"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            int startingPositionIndex = "Player X starting position: ".Length;
            player1 = new Player(int.Parse(lines[0].Substring(startingPositionIndex)));
            player2 = new Player(int.Parse(lines[1].Substring(startingPositionIndex)));

            die = new();
        }

        [Description("what do you get if you multiply the score of the losing player by the number of times the die was rolled during the game?")]
        public override string SolvePart1()
        {
            int turnNumber = 0;
            bool playerOnesTurn = false;
            bool win = false;

            do
            {
                turnNumber++;
                playerOnesTurn = !playerOnesTurn;
                if (playerOnesTurn)
                    win = player1.Move(die.GetRoll() + die.GetRoll() + die.GetRoll());
                else
                    win = player2.Move(die.GetRoll() + die.GetRoll() + die.GetRoll());
            } while (!win);

            int losersScore = playerOnesTurn ? player2.Score : player1.Score;

            return (losersScore * die.Rolls).ToString();
        }

        [Description("what do you get if you multiply the score of the losing player by the number of times the die was rolled during the game?")]
        public override string SolvePart2()
        {
            return string.Empty;
        }
    }
}