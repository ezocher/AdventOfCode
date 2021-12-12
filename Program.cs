﻿using System;
using AdventOfCode.Core;

namespace AdventOfCode {
    public partial class Program {
        public static void Main(string[] args) {
            // Run all solutions or run solutions for a specific year
            // Runner.AdventYear(2021); return;

            const int Year = 2021;
            const int Day = 12;
            const bool Solved = false;

            // Before puzzle release:
            // * Log in to AoC and verify/update session cookie (if necessary)

            // At puzzle release:
            //   Get input and description and generate puzzle class file and empty test input file
            //   (this returns quickly if files have already been downloaded/generated)
            Tools.GetInputGeneratePuzzleTemplate(SESSION, Year, Day);

            // Developing and running solution:
            Runner.AdventYear(Year, Day);
            // TODO: If Solved is true and input file name doesn't include solutions, then rename input file with solutions

            // After Solving:
            if (Solved)
            {
                Tools.DownloadDescription(SESSION, Year, Day);   // Get description including part 2
                Runner.WriteLine("\nReminder: add final solutions to names of input files\n", ConsoleColor.Red);
                Runner.WriteLine("Reminder: update README.md", ConsoleColor.Red);
            }
        }
    }
}


// Put the lines below in a file "SessionCookie.cs" in the Secrets folder and customize with your personal session cookie
// The Secrets folder is in the .gitignore, so this file will not be published

//namespace AdventOfCode
//{
//    public partial class Program
//    {
//        private const string SESSION = "YOUR SESSION COOKIE";
//    }
//}