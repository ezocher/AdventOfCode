using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle20 : ASolver 
    {
        private List<string> lines;

        private char[] imageEnhancementMap;
        private CharImage image;

        private const char ZeroChar = '.';
        private const char OneChar = '#';

        public Puzzle20(string input) : base(input) { Name = "Trench Map"; }

        public override void Setup()
        {
            lines = Tools.GetLines(Input);

            imageEnhancementMap = new char[512];
            for (int i = 0; i < lines[0].Length; i++)
                imageEnhancementMap[i] = lines[0][i];
        }

        private static int FilterIndex(char[] srcChars)
        {
            int result = 0;
            for (int i = 0; i < srcChars.Length; i++)
                result = (result << 1) + ((srcChars[i] == OneChar) ? 1 : 0);
            return result;
        }

        private CharImage Filter(CharImage image, char[] imageEnhancementMap)
        {
            const int SourceSize = 3;

            CharImage output = new CharImage(image.Width, image.Height, ZeroChar);

            for (int row = 1; row < image.Height - 1; row++)
                for (int column = 1; column < image.Width - 1; column++)
                {
                    char[] sourceChars = new char[SourceSize * SourceSize];
                    int sourceIndex = 0;
                    for (int r = row - 1; r <= row + 1; r++)
                        for (int c = column - 1; c <= column + 1; c++)
                            sourceChars[sourceIndex++] = image.Data[r, c];

                    output.Data[row, column] = imageEnhancementMap[FilterIndex(sourceChars)];
                }

            return output;
        }

        private CharImage Enhance(CharImage image, char[] imageEnhancementMap, int passes)
        {
            const bool verbose = false;

            CharImage src = image;
            CharImage dest = null;

            if (verbose) { Console.WriteLine($"\n----- Input Image -----"); src.Write(); }

            for (int pass = 1; pass <= passes; pass++)
            {
                dest = Filter(src, imageEnhancementMap);
                src = dest;
                if (verbose) { Console.WriteLine($"\n----- Pass #{pass} -----"); dest.Write(); }
            }
            return dest;
        }

        [Description("How many pixels are lit in the resulting image?")]
        public override string SolvePart1()
        {
            const int EnhancementIterations = 2;
            const int Pad = EnhancementIterations * 2 + 5;

            image = new CharImage(Pad, ZeroChar, lines, 2, lines[2].Length);

            CharImage output = Enhance(image, imageEnhancementMap, EnhancementIterations);

            output.SetBorder(EnhancementIterations, ZeroChar);
            return output.CountChar(OneChar).ToString();
        }

        [Description("How many pixels are lit in the resulting image?")]
        public override string SolvePart2()
        {
            const int EnhancementIterations = 50;
            const int Pad = EnhancementIterations * 2 + 5;

            image = new CharImage(Pad, ZeroChar, lines, 2, lines[2].Length);

            CharImage output = Enhance(image, imageEnhancementMap, EnhancementIterations);

            output.SetBorder(EnhancementIterations, ZeroChar);
            return output.CountChar(OneChar).ToString();
        }
    }
}