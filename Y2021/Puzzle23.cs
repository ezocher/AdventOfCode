using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public partial class Puzzle23 : ASolver 
    {
        private GameState startingState;
        private Queue<GameState> gameStates;

        private GameState2 startingState2;
        private Queue<GameState2> gameStates2;

        private Dictionary<string, int> visitedStates;
        private int lowestCost;

        public Puzzle23(string input) : base(input) { Name = "Amphipod"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);
            string[] Part2InsertedLines = new string[] { "  #D#C#B#A#", "  #D#B#A#C#" };

            startingState = new GameState(lines[2], lines[3]);
            startingState2 = new GameState2(new string[] { lines[2], Part2InsertedLines[0], Part2InsertedLines[1], lines[3] });

            gameStates = new();
            gameStates.Enqueue(startingState);

            gameStates2 = new();
            gameStates2.Enqueue(startingState2);

            visitedStates = new();
            visitedStates.Add(startingState.ToMinString(), 0);

            lowestCost = int.MaxValue;
        }

        private void SetupPart2(GameState2 startingState)
        {
            visitedStates = new();
            visitedStates.Add(startingState.ToMinString(), 0);

            lowestCost = int.MaxValue;
        }

        // TODO: For Part 1 - Replace GameState with a parameterized GameState2
        // TODO: For both parts - move state driver loop to a new method
        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart1()
        {
            List<(int, int)> possibleMoves = null;

            while (gameStates.Count > 0)
            {
                GameState gs = gameStates.Dequeue();
                possibleMoves = gs.GetPossibleMoves();
                
                // If a GameState has no legal moves it will return a zero length possible moves
                if (possibleMoves.Count > 0)
                    foreach ((int from, int to) in possibleMoves)
                    {
                        GameState nextMove = new GameState(gs);
                        bool solved = nextMove.Move(from, to);

                        if ((solved) && (nextMove.CumulativeCost < lowestCost))
                            lowestCost = nextMove.CumulativeCost;

                        if (visitedStates.ContainsKey(nextMove.ToMinString()))
                        {
                            if (nextMove.CumulativeCost < visitedStates[nextMove.ToMinString()])
                            {
                                visitedStates[nextMove.ToMinString()] = nextMove.CumulativeCost;
                                gameStates.Enqueue(nextMove); // Re-run from this state using lower cost
                            }
                        }
                        else
                        {
                            visitedStates.Add(nextMove.ToMinString(), nextMove.CumulativeCost);
                            if (!solved)
                                gameStates.Enqueue(nextMove);
                        }
                    }
            }
            return lowestCost.ToString();
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart2()
        {
            SetupPart2(startingState2);

            List<(int, int)> possibleMoves = null;

            while (gameStates2.Count > 0)
            {
                GameState2 gs = gameStates2.Dequeue();
                possibleMoves = gs.GetPossibleMoves();

                // If a GameState has no legal moves it will return a zero length possible moves
                if (possibleMoves.Count > 0)
                    foreach ((int from, int to) in possibleMoves)
                    {
                        GameState2 nextMove = new GameState2(gs);
                        bool solved = nextMove.Move(from, to);

                        if ((solved) && (nextMove.CumulativeCost < lowestCost))
                            lowestCost = nextMove.CumulativeCost;

                        if (visitedStates.ContainsKey(nextMove.ToMinString()))
                        {
                            if (nextMove.CumulativeCost < visitedStates[nextMove.ToMinString()])
                            {
                                visitedStates[nextMove.ToMinString()] = nextMove.CumulativeCost;
                                gameStates2.Enqueue(nextMove); // Re-run from this state using lower cost
                            }
                        }
                        else
                        {
                            visitedStates.Add(nextMove.ToMinString(), nextMove.CumulativeCost);
                            if (!solved)
                                gameStates2.Enqueue(nextMove);
                        }
                    }
            }
            return lowestCost.ToString();
        }
    }
}