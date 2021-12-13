using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Common
{
    class CharImage
    {
        public char[,] Data;
        public int Width;
        public int Height;

        public CharImage(List<string> lines, int startingLineIndex, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Data = Parse.CharArray(lines, startingLineIndex, width, height);
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
            Random rand = new Random();

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
