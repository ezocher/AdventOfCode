using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Common
{
    public class IntPoint
    {
        public int X, Y;

        public IntPoint(int x, int y)
        {
            this.X = x; this.Y = y;
        }

        public IntPoint(string s)
        {
            string[] coordinateStrings = s.Split(",", StringSplitOptions.RemoveEmptyEntries);
            X = Int32.Parse(coordinateStrings[0]);
            Y = Int32.Parse(coordinateStrings[1]);
        }

        public override string ToString() => ($"{X},{Y}");

        public static (int, int) MaxXY(List<IntPoint> points)
        {
            int maxX = 0; int maxY = 0;
            foreach (IntPoint pt in points)
            {
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y > maxY) maxY = pt.Y;
            }
            return (maxX, maxY);
        }
    }

    public class IntLine
    {
        public IntPoint Start, End;

        public IntLine(int x1, int y1, int x2, int y2)
        {
            Start = new IntPoint(x1, y1);
            End = new IntPoint(x2, y2);
        }

        public IntLine(string s)
        {
            string[] pointStrings = s.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
            Start = new IntPoint(pointStrings[0]);
            End = new IntPoint(pointStrings[1]);
        }

        public bool IsHorizontalOrVertical() => (IsHorizontal() || IsVertical());

        public bool IsVertical() => (Start.X == End.X);

        public bool IsHorizontal() => (Start.Y == End.Y);

        public int Length() => (Math.Max(Math.Abs(Start.X - End.X), Math.Abs(Start.Y - End.Y)) + 1);

        public override string ToString() => ($"{Start} -> {End}");

    }
}
