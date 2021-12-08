using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021
{
    // Test class for developing new Common and Core classes
    public class Puzzle99 : ASolver
    {
        private CharImage img, img2, img3, img4;

        public Puzzle99(string input) : base(input) { Name = "Test"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            img = new CharImage(lines, 0, 10, 10);
            img2 = new CharImage(lines, 10, 20, 5);
        }

        [Description("What???")]
        public override string SolvePart1()
        {
            img.Write();
            img2.Write();

            img.MapChar('.', ' ');
            img.Write();

            img3 = new CharImage(80, 10, 'o');
            img3.Write();
            img3.Randomize('a', '}');
            img3.Write();

            img4 = new CharImage(100, 20, ' ');
            img4.SequentialFill((char)0);
            img4.Write();

            return String.Empty;
        }

        [Description("What else???")]
        public override string SolvePart2()
        {
            return String.Empty;
        }

    }
}