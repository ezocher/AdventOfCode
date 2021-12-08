using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021
{
    // Terminology: instructions refer to "bingo boards", I call them "bingo cards" here
    public class Puzzle04 : ASolver 
    {
        private int[] calledNumbers;
        private List<BingoCard> bingoCards;
        private const BingoCard noWinner = null;

        private class BingoCard
        {
            public int[,] Numbers;
            public bool[,] Marked;
            public bool HasWon;

            public const int Size = 5;    // Bingo cards are square

            public BingoCard(List<string> lines, int startingLineIndex)
            {
                Numbers = Parse.IntArray(lines, startingLineIndex, Size, Size);
                Marked = new bool[Size, Size];
                HasWon = false;
            }

            public void Mark(int calledNumber)
            {
                for (int row = 0; row < Size; row++)
                    for (int column = 0; column < Size; column++)
                        if (this.Numbers[row, column] == calledNumber)
                            this.Marked[row, column] = true;
            }

            public bool CheckWinner()
            {
                // Check rows
                for (int row = 0; row < Size; row++)
                {
                    int column = 0;
                    while (this.Marked[row, column])
                    {
                        column++;
                        if (column == Size) // A full row is marked, winner found
                            return true;
                    }
                }

                // Check columns
                for (int column = 0; column < Size; column++)
                {
                    int row = 0;
                    while (this.Marked[row, column])
                    {
                        row++;
                        if (row == Size) // A full column is marked, winner found
                            return true;
                    }
                }

                // If this was standard Bingo: add check for diagonals and 4 corners here

                // No winning rows or columns found
                return false;
            }

            public int ScoreWinner()
            {
                int score = 0;

                for (int row = 0; row < Size; row++)
                    for (int column = 0; column < Size; column++)
                        if (!this.Marked[row, column])
                            score += this.Numbers[row, column];

                return score;
            }
        }

        public Puzzle04(string input) : base(input) { Name = "Giant Squid"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            calledNumbers = Parse.LineOfDelimitedInts(lines[0], ",");

            bingoCards = new List<BingoCard>();
            for (int startingLineIndex = 2; startingLineIndex < lines.Count; startingLineIndex += BingoCard.Size + 1)
                bingoCards.Add(new BingoCard(lines, startingLineIndex));
        }

        [Description("What will your final score be if you choose that board?")]
        public override string SolvePart1()
        {
            int numberIndex = 0;
            int calledNumber;
            BingoCard winningCard;

            do
            {
                calledNumber = calledNumbers[numberIndex++];
                MarkNumberOnAllCards(calledNumber);
                winningCard = CheckForFirstWinner();
            } while (winningCard == noWinner);

            int score = winningCard.ScoreWinner();

            return (score * calledNumber).ToString(); ;
        }

        [Description("Figure out which board will win last. Once it wins, what would its final score be?")]
        public override string SolvePart2()
        {
            int numberIndex = 0;
            int calledNumber;
            int numberOfWinningCards = 0;
            BingoCard lastWinner;

            Setup();    // Re-run setup since bingo cards have been marked in SolvePart1()

            do
            {
                calledNumber = calledNumbers[numberIndex++];
                MarkNumberOnAllCards(calledNumber);
                lastWinner = FindLastWinner(ref numberOfWinningCards);
            } while (lastWinner == noWinner);

            int score = lastWinner.ScoreWinner();

            return (score * calledNumber).ToString(); ;
        }

        private void MarkNumberOnAllCards(int calledNumber)
        {
            foreach (BingoCard card in bingoCards)
                card.Mark(calledNumber);
        }

        private BingoCard CheckForFirstWinner()
        {
            for (int cardIndex = 0; cardIndex < bingoCards.Count; cardIndex++)
                if (bingoCards[cardIndex].CheckWinner())
                    return bingoCards[cardIndex];

            return noWinner;
        }

        private BingoCard FindLastWinner(ref int numberOfWinningCards)
        {
            BingoCard lastWinner = noWinner;

            for (int cardIndex = 0; cardIndex < bingoCards.Count; cardIndex++)
            {
                if (!bingoCards[cardIndex].HasWon)
                {
                    if (bingoCards[cardIndex].CheckWinner())
                    {
                        bingoCards[cardIndex].HasWon = true;
                        numberOfWinningCards++;
                        lastWinner = bingoCards[cardIndex];
                    }
                }
            }

            if (numberOfWinningCards == bingoCards.Count)
                return lastWinner;
            else
                return noWinner;
        }

    }
}