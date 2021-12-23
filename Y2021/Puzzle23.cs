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
        private Dictionary<string, int> visitedStates;
        private Queue<GameState> gameStates;
        private int lowestCost;

        public Puzzle23(string input) : base(input) { Name = "Amphipod"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            startingState = new GameState(lines[2], lines[3]);

            visitedStates = new();
            visitedStates.Add(startingState.ToMinString(), 0);

            gameStates = new();
            gameStates.Enqueue(startingState);

            lowestCost = int.MaxValue;
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart1()
        {
            int cycle = 0;
            bool verbose = true;
            List<(int, int)> possibleMoves = null;

            while (gameStates.Count > 0)
            {
                cycle++;
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
            if (verbose) Console.WriteLine($"Lowest cost = {lowestCost}, cycles = {cycle}, visited = {visitedStates.Count}, queued = {gameStates.Count}");
            return lowestCost.ToString();
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}