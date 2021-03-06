using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Common
{
    // TODO: Bugs from height and width confusion
    // TODO: WriteTransposed() is broken for non-square regular CharImage's
    // TODO: constructor from boolean array is broken, works with broken WriteTransposed() but not with Write()
    class CharImage
    {
        public char[,] Data;
        public int Width;
        public int Height;

        public CharImage(List<string> lines, int startingLineIndex, int width, int height)
        {
            Width = width;
            Height = height;
            Data = Parse.CharArray(lines, startingLineIndex, width, height);
        }

        public CharImage(int pad, char padChar, List<string> lines, int startingLineIndex, int size)
        {
            Width = size + (2 * pad);
            Height = size + (2 * pad);

            Data = new char[Height, Width];

            for (int row = 0; row < pad; row++)
                for (int column = 0; column < Width; column++)
                    Data[row, column] = padChar;

            for (int row = pad; row < (size + pad); row++)
                for (int column = 0; column < Width; column++)
                {
                    char nextChar;
                    if ((column < pad) || (column >= (Width - pad)))
                        nextChar = padChar;
                    else
                        nextChar = lines[startingLineIndex + (row - pad)][column - pad];
                    Data[row, column] = nextChar;
                }

            for (int row = size + pad; row < Height; row++)
                for (int column = 0; column < Width; column++)
                    Data[row, column] = padChar;
        }

        public CharImage(int width, int height, char value = ' ')
        {
            this.Width = width;
            this.Height = height;
            Data = new char[height, width];

            for (int row = 0; row < Height; row++)
                for (int column = 0; column < Width; column++)
                        Data[row, column] = value;
        }

        public CharImage(bool[,] values, char tchar = '#', char fchar = '.')
        {
            this.Width = values.GetLength(0);
            this.Height = values.GetLength(1);
            Data = new char[Width, Height];

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (values[i, j])
                        Data[i, j] = tchar;
                    else
                        Data[i, j] = fchar;
        }


        public void MapChar(char fromChar, char toChar)
        {
            for (int row = 0; row < Height; row++)
                for (int column = 0; column < Width; column++)
                    if (Data[row, column] == fromChar)
                        Data[row, column] = toChar;
        }

        public void Randomize(char lowChar, char highChar)
        {
            Random rand = new();

            for (int row = 0; row < Height; row++)
                for (int column = 0; column < Width; column++)
                    Data[row, column] = (char)rand.Next((int)lowChar, ((int)highChar) + 1);
        }

        public void SequentialFill(char startChar)
        {
            int fillValue = (int)startChar;

            for (int row = 0; row < Height; row++)
                for (int column = 0; column < Width; column++)
                    Data[row, column] = (char)fillValue++;
        }

        public void SetBorder(int border, char setChar)
        {
            for (int row = 0; row < border; row++)
                for (int column = 0; column < Width; column++)
                    Data[row, column] = setChar;

            for (int row = 0; row < Height; row++)
                for (int column = 0; column < border; column++)
                    Data[row, column] = setChar;

            for (int row = 0; row < Height; row++)
                for (int column = Width - border; column < Width; column++)
                    Data[row, column] = setChar;

            for (int row = Height - border; row < Height; row++)
                for (int column = 0; column < Width; column++)
                    Data[row, column] = setChar;
        }

        public int CountChar(char target)
        {
            int count = 0;
            for (int row = 0; row < Height; row++)
                for (int column = 0; column < Width; column++)
                    if (Data[row, column] == target) count++;

            return count;
        }

        public void Write()
        {
            Console.WriteLine();
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                    Console.Write(Data[row, column]);

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void WriteTransposed()
        {
            Console.WriteLine();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                    Console.Write(Data[j, i]);

                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
