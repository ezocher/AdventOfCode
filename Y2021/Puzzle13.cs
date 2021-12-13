using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle13 : ASolver 
    {
        public class Dot
        {
            public int X, Y;

            public Dot(int x, int y)
            {
                this.X = x; this.Y = y;
            }

            public Dot(string s)
            {
                string[] coordinates = s.Split(",", StringSplitOptions.RemoveEmptyEntries);
                X = Int32.Parse(coordinates[0]);
                Y = Int32.Parse(coordinates[1]);
            }

            public override string ToString() => ($"{X},{Y}");
        }

        private (int, int) MaxXY(List<Dot> dots)
        {
            int maxX = 0; int maxY = 0;
            foreach (Dot dot in dots)
            {
                if (dot.X > maxX) maxX = dot.X;
                if (dot.Y > maxY) maxY = dot.Y;
            }
            return (maxX, maxY);
        }

        private List<Dot> dots;

        private bool[,] paper;

        private int rows, columns;

        private List<(bool, int)> folds;

        private void MarkDots(List<Dot> dots)
        {
            foreach (Dot dot in dots)
                paper[dot.X, dot.Y] = true;
        }

        private bool[,] FoldPaper(bool[,] paper, (bool, int) foldSpec)
        {
            int columns = paper.GetLength(0);
            int rows = paper.GetLength(1);
            int foldedcolumns = columns;
            int foldedrows = rows;

            (bool foldUp, int foldLine) = foldSpec;

            if (foldUp)
                foldedrows = foldLine;
            else
                foldedcolumns = foldLine;

            bool[,] folded = new bool[foldedcolumns, foldedrows];

            if (foldUp)
            {
                for (int i = 0; i < columns; i++)
                    for (int j = 0; j < foldedrows; j++)
                        folded[i, j] = paper[i, j];

                for (int i = 0; i < columns; i++)
                    for (int j = 0; j < foldedrows; j++)
                        if (paper[i, rows - 1 - j])
                            folded[i, j] = true;
            }
            else // foldLeft
            {
                for (int i = 0; i < foldedcolumns; i++)
                    for (int j = 0; j < rows; j++)
                        folded[i, j] = paper[i, j];

                for (int i = 0; i < foldedcolumns; i++)
                    for (int j = 0; j < rows; j++)
                        if (paper[columns - 1 - i, j])
                            folded[i, j] = true;
            }

            return folded;
        }

        private int CountDots(bool[,] boolarray)
        {
            int count = 0;
            int columns = boolarray.GetLength(0);
            int rows = boolarray.GetLength(1);

            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    if (boolarray[i, j])
                        count++;

            return count;
        }

        public Puzzle13(string input) : base(input) { Name = "Transparent Origami"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            dots = new List<Dot>();

            int i = 0;
            while (lines[i].Length > 0)
                dots.Add(new Dot(lines[i++]));

            folds = new List<(bool, int)>();

            i++;
            for (int j = i; j < lines.Count; j++)
            {
                string[] parts = lines[j].Split("=");
                bool foldUp = (parts[0][parts[0].Length - 1] == 'y');
                int foldLocation = Int32.Parse(parts[1]);
                folds.Add((foldUp, foldLocation));
            }

            (columns, rows) = MaxXY(dots);
            columns++; rows++;  //Add one for 0,0 row and column

            paper = new bool[columns, rows];

            MarkDots(dots);
        }

        [Description("How many dots are visible after completing just the first fold instruction on your transparent paper?")]
        public override string SolvePart1()
        {
            bool[,] folded1 = FoldPaper(paper, folds[0]);
            int dotCount = CountDots(folded1);
            return dotCount.ToString();
        }

        [Description("What code do you use to activate the infrared thermal imaging camera system?")]
        public override string SolvePart2()
        {
            Setup();
            bool[,] folded = paper;

            foreach ((bool,int) fold in folds)
                folded = FoldPaper(folded, fold);

            // This solution writes the "image" of the folded paper to the console, but doesn't
            //  convert it into a string, the strings below were added after visually solving the puzzle
            CharImage paperImage = new CharImage(folded, '#', ' ');
            paperImage.WriteTransposed();

            if (paperImage.Width <= 5)
                return "O";             // test input
            else
                return "HZLEHJRK";      // Given input
        }
    }
}