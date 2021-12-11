using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle11 : ASolver 
    {
        private int[,] energyLevels;

        private const int GridSize = 10;
        private const int Steps = 100;
        private const int MaxEnergy = 9;
        private const int StartEnergy = 0;
        private const int HasFlashed = -1;

        public Puzzle11(string input) : base(input) { Name = "Dumbo Octopus"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            energyLevels = new int[GridSize, GridSize];

            for (int r = 0; r < GridSize; r++)
                for (int c = 0; c < GridSize; c++)
                    energyLevels[r, c] = Int32.Parse(lines[r][c].ToString());
        }

        private int RunStep()
        {
            int totalFlashes = 0;

            // Increase energy levels
            for (int r = 0; r < GridSize; r++)
                for (int c = 0; c < GridSize; c++)
                    energyLevels[r, c]++;

            // Flash and propagate until there are no more flashes
            int flashesThisPass;
            do
            {
                flashesThisPass = 0;
                for (int r = 0; r < GridSize; r++)
                    for (int c = 0; c < GridSize; c++)
                        if (energyLevels[r, c] > MaxEnergy)
                        {
                            flashesThisPass++;
                            energyLevels[r, c] = HasFlashed;
                            PropagateFlash(r, c);
                        }
                totalFlashes += flashesThisPass;
            } while (flashesThisPass > 0);
            
            // Reset energy levels for ones that flashed
            for (int r = 0; r < GridSize; r++)
                for (int c = 0; c < GridSize; c++)
                    if (energyLevels[r, c] == HasFlashed)
                        energyLevels[r, c] = StartEnergy;

            return totalFlashes;
        }

        private void PropagateFlash(int flashR, int flashC)
        {
            int startR, endR, startC, endC;
            startR = (flashR == 0) ? 0 : flashR - 1;
            startC = (flashC == 0) ? 0 : flashC - 1;
            endR   = (flashR == (GridSize - 1)) ? (GridSize - 1) : flashR + 1;
            endC   = (flashC == (GridSize - 1)) ? (GridSize - 1) : flashC + 1;

            for (int r = startR; r <= endR; r++)
                for (int c = startC; c <= endC; c++)
                    if (energyLevels[r, c] != HasFlashed)
                        energyLevels[r, c]++;
        }

        [Description("How many total flashes are there after 100 steps?")]
        public override string SolvePart1()
        {
            int totalFlashes = 0;

            for (int step = 1; step <= Steps; step++)
                totalFlashes += RunStep();

            return totalFlashes.ToString();
        }

        [Description("What is the first step during which all octopuses flash?")]
        public override string SolvePart2()
        {
            int step = 1;
            const int AllFlashed = GridSize * GridSize;

            Setup();

            while (RunStep() != AllFlashed)
                step++;

            return step.ToString();
        }
    }
}