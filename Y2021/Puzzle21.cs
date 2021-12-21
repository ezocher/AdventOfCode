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
        private const short BoardSpaces = 10;

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

        private const short NumOutcomes3Rolls = 27;
        private const short PossibleTotals3Rolls = 7;
        private const short LowestTotal3Rolls = 3;
        private const short HighestTotal3Rolls = 9;
        private static long[] OutcomeCounts = new long[] { 1, 3, 6, 7, 6, 3, 1 };

        private static int numberOfGSNodes = 0;

        private static (short, short) Move(short pos, short score, short spaces)
        {
            short newSpace = (short)((pos + spaces) % BoardSpaces);
            if (newSpace == 0) newSpace = BoardSpaces;
            score += newSpace;
            pos = newSpace;
            return (pos, score);
        }

        private const int WinningScore = 21;

        private class GameState
        {
            public short Score1;
            public short Score2;
            public short Pos1;
            public short Pos2;
            public bool Player1Next;
            public bool Win;
            public GameState[] NextTurn;

            public GameState(short position1, short position2)
            {
                Score1 = Score2 = 0;
                Pos1 = position1;
                Pos2 = position2;
                Player1Next = true;
                Win = false;
                NextTurn = null;
                numberOfGSNodes++;
            }

            public GameState(GameState previous, short roll)
            {
                NextTurn = null;

                if (previous.Player1Next)
                {
                    (Pos1, Score1) = Move(previous.Pos1, previous.Score1, roll);
                    Win = (Score1 >= WinningScore);
                    Pos2 = previous.Pos2; Score2 = previous.Score2;
                }
                else
                {
                    (Pos2, Score2) = Move(previous.Pos2, previous.Score2, roll);
                    Win = (Score2 >= WinningScore);
                    Pos1 = previous.Pos1; Score1 = previous.Score1;
                }

                Player1Next = !previous.Player1Next;

                numberOfGSNodes++;
            }

            public void BuildGameTree()
            {
                if (Win) return;

                NextTurn = new GameState[PossibleTotals3Rolls];
                for (short i = 0; i < PossibleTotals3Rolls; i++)
                {
                    short nextRoll = (short)(i + LowestTotal3Rolls);
                    NextTurn[i] = new GameState(this, nextRoll);
                    NextTurn[i].BuildGameTree();
                }
            }

            public (long, long) CountWins(long numberOfOutcomes)
            {
                if (Win)
                {
                    if (Score1 >= WinningScore)
                        return (numberOfOutcomes, 0);
                    else
                        return (0, numberOfOutcomes);
                }
                else
                {
                    long totalWins1 = 0;
                    long totalWins2 = 0;

                    for (short i = 0; i < PossibleTotals3Rolls; i++)
                    {
                        (long wins1, long wins2) = NextTurn[i].CountWins(numberOfOutcomes * OutcomeCounts[i]);
                        totalWins1 += wins1;
                        totalWins2 += wins2;
                    }
                    return (totalWins1, totalWins2);
                }
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

        [Description("Find the player that wins in more universes; in how many universes does that player win?")]
        public override string SolvePart2()
        {
            Setup();
            GameState root = new GameState((short)player1.Position, (short)player2.Position);

            root.BuildGameTree();

            (long player1wins, long player2wins) = root.CountWins(1);

            return Math.Max(player1wins, player2wins).ToString();
        }
    }
}