using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Core
{
    class Parse
    {
        // Example input:
        // 3 15  0  2 22
        // 9 18 13 17  5
        //19  8  7 25 23
        //20 11 10 24  4
        //14 21 16 12  6
        //
        //14 21 17 24  4
        //10 16 15  9 19
        //18  8 23 26 20
        //22 11 13  6  5
        // 2  0 12  3  7

        // Assumes: pre-determined and non-varying array dimensions, 1 row per line of input, and 1 empty line separating arrays (which is skipped)
        public static List<int[,]> ListOfIntArrays(List<string> lines, int startingLineIndex, int w, int h, string delimiter = " ")
        {
            List<int[,]> arrayList = new List<int[,]>();

            for (int line = startingLineIndex; line < lines.Count; line += h + 1)
            {
                int[,] nextArray = IntArray(lines, line, w, h, delimiter);
                arrayList.Add(nextArray);
            }

            return arrayList;
        }

        // See ListOfIntArrays for example and assumptions
        public static int[,] IntArray(List<string> lines, int startingLineIndex, int w, int h, string delimiter = " ")
        {
            int[,] newArray = new int[w, h];

            for (int row = 0; row < h; row++)
            {
                int[] rowArray = LineOfDelimitedInts(lines[startingLineIndex + row], delimiter);
                for (int column = 0; column < w; column++)
                    newArray[row, column] = rowArray[column];
            }

            return newArray;
        }

        // Example input:
        //7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1
        public static int[] LineOfDelimitedInts(string lineIn, string delimiter = " ")
        {
            string[] numStrings = lineIn.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            int[] numbers = new int[numStrings.Length];
            for (int i = 0; i < numStrings.Length; i++)
                numbers[i] = Int32.Parse(numStrings[i]);
            return numbers;
        }
        
        // Example input:
        //..........
        //.********.
        //.......*..
        //......*...
        //.....*....
        //....*.....
        //...*......
        //..*.......
        //.********.
        //..........
        public static char[,] CharArray(List<string> lines, int startingLineIndex, int w, int h)
        {
            char[,] newArray = new char[h, w];

            for (int row = 0; row < h; row++)
                for (int column = 0; column < w; column++)
                    newArray[row, column] = lines[startingLineIndex + row][column];

            return newArray;
        }
    }
}
