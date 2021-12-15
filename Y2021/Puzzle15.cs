using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle15 : ASolver 
    {
        private int[,] risks;
        private int[,] riskmap;
        private int width, height;

        private int tileWidth, tileHeight;
        public const int TilesMult = 5;

        public Puzzle15(string input) : base(input) { Name = "Chiton"; }

        // TODO: width and height are messed up for non-square arrays
        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            width = lines[0].Length;
            height = lines.Count;
            risks = new int[width, height];
            riskmap = new int[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    risks[x, y] = int.Parse(lines[x][y].ToString());
                    riskmap[x, y] = int.MaxValue;
                }
            riskmap[0, 0] = 0;
        }

        private void SetupPart2()
        {
            List<string> lines = Tools.GetLines(Input);

            tileWidth = lines[0].Length;
            tileHeight = lines.Count;
            width = tileWidth * TilesMult;
            height = tileHeight * TilesMult;
            
            risks = new int[width, height];
            riskmap = new int[width, height];

            int[,] tile = new int[tileWidth, tileHeight];
            for (int x = 0; x < tileWidth; x++)
                for (int y = 0; y < tileHeight; y++)
                    tile[x, y] = int.Parse(lines[x][y].ToString());

            for (int i = 0; i < TilesMult; i++)
                for (int j = 0; j < TilesMult; j++)
                    StampTile(tile, i, j);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    riskmap[x, y] = int.MaxValue;
            riskmap[0, 0] = 0;
        }

        private void StampTile(int[,] tile, int i, int j)
        {
            const int MaxRisk = 9;

            int xOffset = i * tileWidth;
            int yOffset = j * tileHeight;
            int valueIncrement = i + j;

            for (int x = 0; x < tileWidth; x++)
                for (int y = 0; y < tileHeight; y++)
                {
                    int newRisk = tile[x, y] + valueIncrement;
                    if (newRisk > MaxRisk)
                        newRisk %= MaxRisk;
                    risks[x + xOffset, y + yOffset] = newRisk;
                }
        }

        int[,] savedmap;

        private bool ValueChanged(int x, int y) => (riskmap[x, y] != savedmap[x, y]);

        private void WriteMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    Runner.Write($"{riskmap[x, y]:00} ", ValueChanged(x,y) ? ConsoleColor.Red : ConsoleColor.Green);
                Console.WriteLine();
            }
            Console.WriteLine();

            savedmap = (int[,])riskmap.Clone();
        }

        // === Original version: FindLowestRiskPathLoopDandRRescan(false)
        // Assumes that lowest risk paths only go down and to the right (as in the sample provided)
        // Used this to solve Part 1 and to get incorrect answer on Part 2 (and incorrect answer on any input where the solution
        //  path doesn't go strictly down and right)
        //
        // === Modified version: FindLowestRiskPathLoopDandRRescan(true)
        // Rescan idea from u/TcMaX on Reddit (and now correct for all inputs):
        // Ended up first iterating through the array in the pattern of[0][1], [1] [0], [1] [1] etc, and just filling out
        // the minimum cumulative cost of the one to the left and the one above plus the cost of the current cell.This gives
        // an array of the cumulative costs if you never go left or up. Obviously this was not sufficient, so I added an
        // extra step where I iterated through every cell in the cumulative cost matrix, and then took the lowest value of
        // each of the surrounding cells, added the cost of the current cell, and checked if that was less than the cumulative
        // cost already found for that cell. Repeated with a while loop until no changes were made in an entire pass over the
        // array. Not exactly elegant, but it did the job and wasn't too slow (took about 350ms on my machine (M1 Air) on part 2).
        //
        // This version took about 1.6s on part 2 versus 16s for the brute force queue
        private int FindLowestRiskPathLoopDandRRescan(bool rescan)
        {

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (x < width - 1) RiskNextDownRight(riskmap[x, y], x + 1, y);
                    if (y < height - 1) RiskNextDownRight(riskmap[x, y], x, y + 1);
                }

            if (rescan)
            {
                //savedmap = new int[width, height];
                //Console.WriteLine();
                //WriteMap();
                int numberNodesChanged;
                int numberOfRescans = 0;

                do
                {
                    numberNodesChanged = 0;
                    numberOfRescans++;
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                            int checkValue = LowestRiskMapNeighbor(x, y) + risks[x, y];
                            if (checkValue < riskmap[x, y])
                            {
                                riskmap[x, y] = checkValue;
                                numberNodesChanged++;
                            }
                        }
                    //Console.WriteLine($"Number of nodes changed = {numberNodesChanged}");
                    // WriteMap();
                } while (numberNodesChanged > 0);

                Console.WriteLine($"Number of Rescans = {numberOfRescans}; Total nodes visited = {(width * height) * (numberOfRescans + 1)}");
            }

            return riskmap[width - 1, height - 1];
        }

        private int LowestRiskMapNeighbor(int x, int y) => Tools.Min(RiskMapOrMaxInt(x + 1, y), RiskMapOrMaxInt(x - 1, y),
            RiskMapOrMaxInt(x, y + 1), RiskMapOrMaxInt(x, y - 1));

        private int RiskMapOrMaxInt(int x, int y)
        {
            if ((x >= 0) && (y >= 0) && (x < width) && (y < height))
                return riskmap[x, y];
            else
                return int.MaxValue;
        }

        private void RiskNextDownRight(int r, int x, int y) => riskmap[x, y] = Math.Min(riskmap[x, y], r + risks[x, y]);

        private Queue<(int, int, int)> q;
        private long nodesVisited = 0;

        private int FindLowestRiskPathQueue()
        {
            q = new();
            q.Enqueue((0, 0, 0));

            while (q.Count > 0)
            {
                (int r, int x, int y) = q.Dequeue();
                VisitAllNeighbors(r, x, y);
            }

            Console.WriteLine($"Nodes visted: {nodesVisited} = {height * width} nodes @ {nodesVisited / (height * width)} visits/node");

            return riskmap[width - 1, height -1];
        }

        private void VisitAllNeighbors(int r, int x, int y)
        {
            if (x < width - 1)  RiskNext(r, x + 1, y);
            if (y < height - 1) RiskNext(r, x, y + 1);
            if (x > 0) RiskNext(r, x - 1, y);
            if (y > 0) RiskNext(r, x, y - 1);
        }

        private void RiskNext(int r, int x, int y)
        {
            nodesVisited++;
            int newR = r + risks[x, y];
            if (newR < riskmap[x, y])
            {
                riskmap[x, y] = newR;
                q.Enqueue((riskmap[x, y], x, y));
            }
        }

        [Description("What is the lowest total risk of any path from the top left to the bottom right?")]
        public override string SolvePart1()
        {
            return FindLowestRiskPathLoopDandRRescan(true).ToString();
            //return FindLowestRiskPathQueue().ToString();
        }

        [Description("What is the lowest total risk of any path from the top left to the bottom right?")]
        public override string SolvePart2()
        {
            SetupPart2();
            return SolvePart1();
        }
    }
}