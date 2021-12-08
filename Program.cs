using System;
using System.Collections.Generic;
using AdventOfCode.Core;

namespace AdventOfCode {
    public partial class Program {
        public static void Main(string[] args) {

            // TODO: Figure out manual and automated workflow wrt test input and description refetching for part 2
            //       Objective is to only edit Program.cs once per puzzle day 
            // TODO: Refactor into ?? FetchInput(), FetchDescription(), GeneratePuzzleTemplate()

            
            Tools.GeneratePuzzleTemplate(SESSION, 2021, 8);

            Runner.AdventYear(2021);

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