using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Core
{
    // TODO: Review and organize AdventOfCode.Core.Tools
    public static partial class Tools
    {
        public static int IndexOf(this StringBuilder s, char c, int start)
        {
            if (start >= s.Length)
                return -1;

            while (s[start] != c)
            {
                start++;
                if (start == s.Length)
                    return -1;
            }
            return start;
        }

        public static (T, T) MinMax<T>(IEnumerable<T> values) where T : IComparable<T>
        {
            bool firstElement = true;
            T min = default; T max = default;
            foreach (T value in values)
            {
                if (firstElement)
                {
                    min = value; max = value; firstElement = false;
                }
                else
                {
                    if (value.CompareTo(min) < 0) { min = value; }
                    if (value.CompareTo(max) > 0) { max = value; }
                }
            }
            return (min, max);
        }

        public static T Max<T>(IEnumerable<T> values) where T : IComparable<T>
        {
            bool firstElement = true;
            T max = default;
            foreach (T value in values)
            {
                if (firstElement)
                {
                    max = value; firstElement = false;
                }
                else if (value.CompareTo(max) > 0)
                    max = value;
            }
            return max;
        }

        public static T Min<T>(IEnumerable<T> values) where T : IComparable<T>
        {
            bool firstElement = true;
            T min = default;
            foreach (T value in values)
            {
                if (firstElement)
                {
                    min = value; firstElement = false;
                }
                else if (value.CompareTo(min) < 0)
                    min = value;
            }
            return min;
        }

        public static long Sum(params long[] nums)
        {
            long sum = 0;
            foreach (long n in nums) sum += n;
            return sum;
        }

        // Returns -1, 0, or 1 based on a versus b
        // CompareTo() is not specified to return -1 or 1, just <0 or >0
        public static int LessEqGreater(int a, int b)
        {
            if      (a < b)    return -1;
            else if (a == b)   return  0;
            else /* (a > b) */ return  1;
        }

        public static int CountTrue(bool[,] boolarray)
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

        public static void InsertOrIncrement<T>(this Dictionary<T, int> dict, T key, int increment)
        {
            if (dict.ContainsKey(key))
                dict[key] += increment;
            else
                dict.Add(key, increment);
        }

        public static void InsertOrIncrement<T>(this Dictionary<T, long> dict, T key, long increment)
        {
            if (dict.ContainsKey(key))
                dict[key] += increment;
            else
                dict.Add(key, increment);
        }
    }
}
